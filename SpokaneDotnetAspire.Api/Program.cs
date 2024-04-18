using Microsoft.EntityFrameworkCore;

using SpokaneDotnetAspire.Api.Meetups;
using SpokaneDotnetAspire.Services.Data;
using SpokaneDotnetAspire.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("SpokaneDotnetAspire")
                              ?? throw new InvalidOperationException("Must supply a valid connection string in configuration");
    options.UseNpgsql(connectionString, o => o.MigrationsAssembly("SpokaneDotnetAspire.Api"));
});

builder.Services.AddTransient<IMeetupRepository, MeetupRepository>();

var app = builder.Build();

// Migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext();
    await db.Database.MigrateAsync();
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
