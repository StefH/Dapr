using Dapr.Client;

namespace DaprCounter;

/// <summary>
/// Run:
/// - dapr run --app-id DaprCounter --components-path ./components dotnet run
/// </summary>
class Program
{
    // static string StoreNameSecret = "envvar-secret-store";
    static string StoreNameSecret = "local-secret-store";
    static string StoreName = "statestore-azure-storage-account";

    static async Task Main(string[] args)
    {
        const string key = "counter";

        var daprClient = new DaprClientBuilder().Build();

        var accountKey = await daprClient.GetSecretAsync(StoreNameSecret, "DaprAzureStorageAccountKey");
        Console.WriteLine($"DaprAzureStorageAccountKey = '{accountKey.Values.First()}'");

        var counter = await daprClient.GetStateAsync<int>(StoreName, key);

        while (true)
        {
            Console.WriteLine($"Counter = {counter++}");

            await daprClient.SaveStateAsync(StoreName, key, counter);
            await Task.Delay(2000);
        }
    }
}