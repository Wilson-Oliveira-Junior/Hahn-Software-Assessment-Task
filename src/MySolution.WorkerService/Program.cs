using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using MySolution.Jobs;
using MySolution.WorkerService;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

var builder = Host.CreateApplicationBuilder(args);

// Configure Hangfire services
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();
builder.Services.AddHostedService<Worker>();

// Register MyJob
builder.Services.AddTransient<MyJob>();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register GraphQL Client
builder.Services.AddSingleton(s => new GraphQLHttpClient("https://graphql.anilist.co", new NewtonsoftJsonSerializer()));

var host = builder.Build();

// Configure Hangfire jobs
using (var scope = host.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    var job = scope.ServiceProvider.GetRequiredService<MyJob>();
    recurringJobManager.AddOrUpdate("my-recurring-job", () => job.Execute(), Cron.Hourly);
}

host.Run();
