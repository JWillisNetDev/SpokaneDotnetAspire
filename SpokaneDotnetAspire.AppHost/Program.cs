var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SpokaneDotnetAspire_Site>("spokanedotnetaspire-site");

builder.AddProject<Projects.SpokaneDotnetAspire_Api>("spokanedotnetaspire-api");

builder.Build().Run();
