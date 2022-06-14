// Run:
// - dapr run --app-id mybackend --dapr-http-port 3500 --app-port 5124 --app-ssl --components-path ./components -- dotnet run (does not work...)
// - dapr run --app-id mybackend --dapr-http-port 3500 --app-port 5124 --components-path ./components -- dotnet run

// Test if running
// - curl http://localhost:3500/v1.0/invoke/mybackend/method/weatherforecast

// Verify 
// - curl http://localhost:5124/dapr/subscribe

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()

    // Registers the necessary services to integrate Dapr into the MVC pipeline.
    // It also registers a DaprClient instance into the dependency injection container, which then can be injected anywhere into your service.
    .AddDapr();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Pub-Sub
app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();

app.Run();