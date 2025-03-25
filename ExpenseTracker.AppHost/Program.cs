var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.ExpenseTracker_ApiService>("apiservice");

builder.AddProject<Projects.ExpenseTracker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
