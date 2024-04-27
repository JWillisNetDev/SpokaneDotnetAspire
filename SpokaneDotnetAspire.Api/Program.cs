using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

using SpokaneDotnetAspire.Api;
using SpokaneDotnetAspire.Api.Endpoints.ImageUpload;
using SpokaneDotnetAspire.Api.Endpoints.Meetups;
using SpokaneDotnetAspire.Api.Services.Storage;
using SpokaneDotnetAspire.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddSpokaneDotnetAspire();

builder.Services.AddAzureClients(clientBuilder
    => clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnectionString:blob"]!));

var app = builder.Build();

app.MapDefaultEndpoints();

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

app.MapMeetupEndpoints()
    .MapImageUploadEndpoints();

app.Run();
