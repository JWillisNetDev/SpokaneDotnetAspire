using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

using SpokaneDotnetAspire.Api.Meetups;
using SpokaneDotnetAspire.Api.Storage;
using SpokaneDotnetAspire.Services.Data;
using SpokaneDotnetAspire.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("SpokaneDotnetAspire")
                              ?? throw new InvalidOperationException("Must supply a valid connection string in configuration");
    options.UseNpgsql(connectionString, o => o.MigrationsAssembly("SpokaneDotnetAspire.Api"));
});

builder.Services.AddTransient<IMeetupRepository, MeetupRepository>();

builder.Services.AddAzureClients(clientBuilder =>
{
    BlobClientBuilderExtensions.AddBlobServiceClient(clientBuilder, builder.Configuration["StorageConnectionString:blob"]);
});

builder.Services.AddOptions<StorageOptions>()
    .Bind(builder.Configuration.GetSection(StorageOptions.ConfigurationSource));
builder.Services.AddSingleton<IStorageService, StorageService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Migrate database
    var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext();
    await db.Database.MigrateAsync();

    // Ensure storage containers are created
    var storageService = scope.ServiceProvider.GetRequiredService<IStorageService>();
    await storageService.EnsureContainersCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapMeetupEndpoints();

app.Run();
