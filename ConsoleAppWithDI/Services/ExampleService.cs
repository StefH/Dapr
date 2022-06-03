using System.Net.Http.Json;
using ConsoleAppWithDI.Models;
using Dapr.Client;

namespace ConsoleAppWithDI.Services;

internal class ExampleService : IExampleService
{
    private const string Key = "ComplexCounter";
    private const string StoreNameSecret = "local-secret-store";
    private const string StoreName = "statestore-azure-storage-account";

    private readonly DaprClient _daprClient;

    public ExampleService(DaprClient daprClient)
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

            // 1. Calling Dapr sidecar with .NET HttpClient
            try
            {
                var client = new HttpClient();
                var forecasts = await client.GetFromJsonAsync<List<WeatherForecast>>(
                        "http://localhost:3500/v1.0/invoke/mybackend/method/weatherforecast", cancellationToken);

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


            // 2. Via Dapr's rich integration with HttpClient
            try
            {
                var client = DaprClient.CreateInvokeHttpClient("mybackend");
                var forecasts = await client.GetFromJsonAsync<List<WeatherForecast>>("/weatherforecast", cancellationToken);

                foreach (var forecast in forecasts!)
                {
                    Console.WriteLine($"InvokeHttpClient --> Date:{forecast.Date}, TemperatureC:{forecast.TemperatureC}, Summary:{forecast.Summary}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("InvokeHttpClient Exception = {0}", ex.Message);
            }

            await Task.Delay(3000, cancellationToken);


            // 3. Calling Dapr sidecar with DaprClient
            try
            {
                var forecasts = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(
                    HttpMethod.Get,
                    "mybackend",
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