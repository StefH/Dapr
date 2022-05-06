using Dapr.Client;

namespace DaprCounter;

/// <summary>
/// Run:
/// - dapr run --app-id DaprCounter --components-path ./components dotnet run
/// </summary>
class Program
{
    // static string StoreNameSecret = "envvar-secret-store";
    static string StoreNameSecret = "local-secret-store"; // The location from the local json file is defined in "secretstores.local.file.yaml"
    static string StoreName = "statestore-azure-storage-account"; // This StoreName is defined in "azureblob.yaml"

    static async Task Main(string[] args)
    {
        const string key = "counter";

        var daprClient = new DaprClientBuilder().Build();

        // 1. Get the DaprAzureStorageAccountKey from the local secrets (as an example)
        var accountKey = await daprClient.GetSecretAsync(StoreNameSecret, "DaprAzureStorageAccountKey");
        Console.WriteLine($"DaprAzureStorageAccountKey = '{accountKey.Values.First()}'");

        // 2. Get the current state from the counter for that StoreName (see "azureblob.yaml" the auth --> secretStore property)
        var counter = await daprClient.GetStateAsync<int>(StoreName, key);

        while (true)
        {
            Console.WriteLine($"Counter = {counter++}");

            // 3. Update the value to ++
            await daprClient.SaveStateAsync(StoreName, key, counter);
            await Task.Delay(2000);
        }
    }
}