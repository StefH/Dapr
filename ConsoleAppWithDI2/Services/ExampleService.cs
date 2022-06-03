using System.Net.Http.Json;
using ConsoleAppWithDI2.Models;
using Dapr.Client;

namespace ConsoleAppWithDI2.Services;

internal class ExampleService : IExampleService
{
    private const string Key = "ComplexCounter";
    private const string StoreNameSecret = "local-secret-store";
    private const string StoreName = "statestore-azure-storage-account";

    private readonly IDaprClient _daprClient;

    public ExampleService(IDaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var accountKey = await _daprClient.GetSecretAsync(StoreNameSecret, "DaprAzureStorageAccountKey", cancellationToken: cancellationToken);
        Console.WriteLine($"DaprAzureStorageAccountKey = '{accountKey.Values.First()}'");
        
        var counter = await _daprClient.GetStateAsync<ComplexCounter>(StoreName, Key, ConsistencyMode.Strong, null, cancellationToken) ?? new ComplexCounter { Time = DateTime.UtcNow };
        while (true)
        {
            Console.WriteLine($"Current Counter = {counter.Value} @ {counter.Time}");

            counter.Value++;
            counter.Time = DateTime.UtcNow;

            await _daprClient.SaveStateAsync(StoreName, Key, counter, null, null, cancellationToken);

            await Task.Delay(3000, cancellationToken);

            try
            {
                var forecasts = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(
                    HttpMethod.Get,
                    "MyBackend",
                    "weatherforecast",
                    cancellationToken);

                foreach (var forecast in forecasts!)
                {
                    Console.WriteLine($"DaprClient --> Date:{forecast.Date}, TemperatureC:{forecast.TemperatureC}, Summary:{forecast.Summary}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DaprClient Exception = {0}", ex.Message);
            }

            await Task.Delay(3000, cancellationToken);
        }
    }
}