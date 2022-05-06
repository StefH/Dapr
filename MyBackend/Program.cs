// Run:
// - dapr run --app-id MyBackend --dapr-http-port 3500 --app-port 5124 --app-ssl -- dotnet run
// - dapr run --app-id MyBackend --dapr-http-port 3500 --app-port 5124 -- dotnet run

// Test if running
// - curl http://localhost:3500/v1.0/invoke/MyBackend/method/weatherforecast

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.MapControllers();

app.Run();
