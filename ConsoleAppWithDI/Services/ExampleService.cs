using ConsoleAppWithDI.Models;
using Dapr.Client;

namespace ConsoleAppWithDI.Services;

internal class ExampleService : IExampleService
{
    const string key = "ComplexCounter";
    const string StoreNameSecret = "local-secret-store";
    const string StoreName = "statestore-azure-storage-account";

    private readonly DaprClient _daprClient;

    public ExampleService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var accountKey = await _daprClient.GetSecretAsync(StoreNameSecret, "DaprAzureStorageAccountKey", cancellationToken: cancellationToken);
        Console.WriteLine($"DaprAzureStorageAccountKey = '{accountKey.Values.First()}'");

        try
        {
            var forecasts = await _daprClient.InvokeMethodAsync<IEnumerable<string>>(
                HttpMethod.Get,
                "daprbackend",
                "weatherforecast",
                cancellationToken);
        }
        catch { }

        var counter = await _daprClient.GetStateAsync<ComplexCounter>(StoreName, key, ConsistencyMode.Strong, null, cancellationToken) ?? new ComplexCounter { Time = DateTime.UtcNow };
        while (true)
        {
            Console.WriteLine($"Current Counter = {counter.Value} @ {counter.Time}");

            counter.Value++;
            counter.Time = DateTime.UtcNow;

            await _daprClient.SaveStateAsync(StoreName, key, counter, null, null, cancellationToken);
            await Task.Delay(2000, cancellationToken);
        }
    }
}