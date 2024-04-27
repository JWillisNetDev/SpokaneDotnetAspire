using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

using SpokaneDotnetAspire.Api.Services.Repositories;
using SpokaneDotnetAspire.Api.Services.Storage;
using SpokaneDotnetAspire.Data;

namespace SpokaneDotnetAspire.Api;
public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddSpokaneDotnetAspire(this WebApplicationBuilder builder)
    {
        AddDatabase(builder);
        AddRepositories(builder);
        AddAzureClients(builder);
        AddStorageService(builder);

        return builder;
    }

    private static void AddDatabase(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContextFactory<AppDbContext>(options =>
        {
            string connectionString = builder.Configuration.GetConnectionString("SpokaneDotnetAspire")
                                      ?? throw new InvalidOperationException("Must supply a valid connection string in configuration");
            options.UseNpgsql(connectionString, o => o.MigrationsAssembly("SpokaneDotnetAspire.Data"));
        });
    }

    private static void AddRepositories(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IMeetupRepository, MeetupRepository>();
    }

    private static void AddAzureClients(WebApplicationBuilder builder)
    {
        builder.Services.AddAzureClients(clientBuilder
            => clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnectionString:blob"]!));
    }

    private static void AddStorageService(WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<StorageOptions>().Bind(builder.Configuration.GetSection("StorageOptions"));
        builder.Services.AddSingleton<IStorageService, StorageService>();
    }
}
