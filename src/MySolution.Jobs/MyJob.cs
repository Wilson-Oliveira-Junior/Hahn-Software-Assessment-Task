using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySolution.Data;

namespace MySolution.Jobs;

public class MyJob
{
    private readonly ILogger<MyJob> _logger;
    private readonly MyDbContext _dbContext;
    private readonly GraphQLHttpClient _graphQLClient;

    public MyJob(ILogger<MyJob> logger, MyDbContext dbContext, GraphQLHttpClient graphQLClient)
    {
        _logger = logger;
        _dbContext = dbContext;
        _graphQLClient = graphQLClient;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task Execute()
    {
        _logger.LogInformation("Executing job at: {time}", DateTimeOffset.Now);

        // Define the GraphQL query
        var query = @"
        query ($page: Int, $perPage: Int) {
            Page(page: $page, perPage: $perPage) {
                media(type: ANIME, sort: POPULARITY_DESC) {
                    id
                    title {
                        romaji
                        english
                    }
                    description
                }
            }
        }";

        // Define the variables for the query
        var variables = new
        {
            page = 1,
            perPage = 10
        };

        // Create the GraphQL request
        var request = new GraphQLHttpRequest
        {
            Query = query,
            Variables = variables
        };

        try
        {
            // Send the request and get the response
            var response = await _graphQLClient.SendQueryAsync<AniListResponse>(request);
            var data = response.Data.Page.Media;

            // Upsert data into the database
            foreach (var item in data)
            {
                var existingItem = await _dbContext.MyData.FindAsync(item.Id);
                if (existingItem != null)
                {
                    _dbContext.Entry(existingItem).CurrentValues.SetValues(new MyData
                    {
                        Id = item.Id,
                        Name = item.Title.Romaji,
                        Description = item.Description
                    });
                }
                else
                {
                    await _dbContext.MyData.AddAsync(new MyData
                    {
                        Id = item.Id,
                        Name = item.Title.Romaji,
                        Description = item.Description
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Job completed at: {time}", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing job");
        }
    }
}

public class AniListResponse
{
    public Page Page { get; set; }
}

public class Page
{
    public List<Media> Media { get; set; }
}

public class Media
{
    public int Id { get; set; }
    public Title Title { get; set; }
    public string Description { get; set; }
}

public class Title
{
    public string Romaji { get; set; }
    public string English { get; set; }
}
