using Dapr.Client;

namespace ConsoleAppWithDI.Services
{
    internal class ExampleService : IExampleService
    {
        const string key = "counter";
        const string StoreNameSecret = "local-secret-store";
        const string StoreName = "statestore-azure-storage-account";

        private readonly DaprClient _daprClient;

        public ExampleService(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public async Task RunAsync()
        {
            var accountKey = await _daprClient.GetSecretAsync(StoreNameSecret, "DaprAzureStorageAccountKey");
            Console.WriteLine($"DaprAzureStorageAccountKey = '{accountKey.Values.First()}'");

            try
            {
                var forecasts = await _daprClient.InvokeMethodAsync<IEnumerable<string>>(
                     HttpMethod.Get,
                     "daprbackend",
                     "weatherforecast");
            }
            catch { }

            var counter = await _daprClient.GetStateAsync<int>(StoreName, key);
            while (true)
            {
                Console.WriteLine($"Counter = {counter++}");

                await _daprClient.SaveStateAsync(StoreName, key, counter);
                await Task.Delay(2000);
            }
        }
    }
}
