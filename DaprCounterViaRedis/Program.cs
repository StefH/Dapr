using Dapr.Client;

namespace DaprCounterViaRedis;

/// <summary>
/// Run:
/// - dapr run --app-id DaprCounterViaRedis --components-path ./components dotnet run
/// </summary>
class Program
{
    private const string StoreName = "statestore"; // This StoreName is the default Redis store

    static async Task Main(string[] args)
    {
        const string key = "counter";

        var daprClient = new DaprClientBuilder().Build();

        // 1. Get the current state from the counter for that Redis StoreName
        var counter = await daprClient.GetStateAsync<int>(StoreName, key);

        while (true)
        {
            Console.WriteLine($"Counter = {counter++}");

            // 2. Update the value to ++
            await daprClient.SaveStateAsync(StoreName, key, counter);
            await Task.Delay(2000);
        }
    }
}