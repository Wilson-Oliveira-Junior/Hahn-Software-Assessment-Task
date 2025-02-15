using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MySolution.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
        private readonly GraphQLHttpClient _graphQLClient;

        public DataController(ILogger<DataController> logger)
        {
            _logger = logger;
            _graphQLClient = new GraphQLHttpClient("https://graphql.anilist.co", new NewtonsoftJsonSerializer());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Media>>> GetData()
        {
            _logger.LogInformation("GetData method called");

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
                _logger.LogInformation("Sending GraphQL request with query: {query} and variables: {variables}", query, variables);
                // Send the request and get the response
                var response = await _graphQLClient.SendQueryAsync<AniListResponse>(request);
                _logger.LogInformation("GraphQL response received: {response}", response);

                if (response.Errors != null)
                {
                    _logger.LogError("GraphQL errors: {errors}", response.Errors);
                    return StatusCode(500, "Internal server error");
                }

                if (response.Data == null)
                {
                    _logger.LogError("GraphQL response data is null");
                    return StatusCode(500, "Internal server error");
                }

                var data = response.Data.Page.Media;
                _logger.LogInformation("Data fetched: {data}", data);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class AniListResponse
    {
        public Page Page { get; set; } = new Page();
    }

    public class Page
    {
        public List<Media> Media { get; set; } = new List<Media>();
    }

    public class Media
    {
        public int Id { get; set; }
        public Title Title { get; set; } = new Title();
        public string Description { get; set; } = string.Empty;
    }

    public class Title
    {
        public string Romaji { get; set; } = string.Empty;
        public string English { get; set; } = string.Empty;
    }
}
