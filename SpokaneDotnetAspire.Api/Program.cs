using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

using SpokaneDotnetAspire.Api.Meetups;
using SpokaneDotnetAspire.Api.Services.Repositories;
using SpokaneDotnetAspire.Api.Services.Storage;
using SpokaneDotnetAspire.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
});

builder.Services.AddOpenTelemetry()
    .WithMetrics(configure =>
    {
        configure
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    })
    .WithTracing(configure =>
    {
        if (builder.Environment.IsDevelopment())
        {
            configure.SetSampler(new AlwaysOnSampler()); // Always enable tracing in development
        }

        configure
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("SpokaneDotnetAspire")
                              ?? throw new InvalidOperationException("Must supply a valid connection string in configuration");
    options.UseNpgsql(connectionString, o => o.MigrationsAssembly("SpokaneDotnetAspire.Data"));
});

builder.Services.AddTransient<IMeetupRepository, MeetupRepository>();

builder.Services.AddAzureClients(clientBuilder
    => clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnectionString:blob"]!));

builder.Services.AddOptions<StorageOptions>()
    .Bind(builder.Configuration.GetSection("StorageOptions"));
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
