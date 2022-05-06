using System.Net.Http.Json;
using ConsoleAppWithDI2.Models;
using Dapr.Client;

namespace ConsoleAppWithDI2.Services;

internal class ExampleService : IExampleService
{
    const string key = "ComplexCounter";
    const string StoreNameSecret = "local-secret-store";
    const string StoreName = "statestore-azure-storage-account";

    private readonly IDaprClient _daprClient;

    public ExampleService(IDaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var accountKey = await _daprClient.GetSecretAsync(StoreNameSecret, "DaprAzureStorageAccountKey", cancellationToken: cancellationToken);
        Console.WriteLine($"DaprAzureStorageAccountKey = '{accountKey.Values.First()}'");
        
        var counter = await _daprClient.GetStateAsync<ComplexCounter>(StoreName, key, ConsistencyMode.Strong, null, cancellationToken) ?? new ComplexCounter { Time = DateTime.UtcNow };
        while (true)
        {
            Console.WriteLine($"Current Counter = {counter.Value} @ {counter.Time}");

            counter.Value++;
            counter.Time = DateTime.UtcNow;

            await _daprClient.SaveStateAsync(StoreName, key, counter, null, null, cancellationToken);

            await Task.Delay(3000, cancellationToken);

            // Via normal Http call
            try
            {
                var client = new HttpClient();
                var forecasts =
                    await client.GetFromJsonAsync<List<WeatherForecast>>(
                        "http://localhost:3500/v1.0/invoke/MyBackend/method/weatherforecast", cancellationToken);

                foreach (var forecast in forecasts!)
                {
                    Console.WriteLine($"HttpClient --> Date:{forecast.Date}, TemperatureC:{forecast.TemperatureC}, Summary:{forecast.Summary}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpClient Exception = {0}", ex.Message);
            }

            await Task.Delay(3000, cancellationToken);

            // Via real call via Dapr service
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