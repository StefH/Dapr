using System.Net.Http.Json;
using Man.Dapr.Sidekick;
using Man.Dapr.Sidekick.Logging;
using Man.Dapr.Sidekick.Threading;

namespace DaprCounterWithSidekick;

class Program
{
    static string StoreName = "statestore-azure-storage-account"; // This StoreName is defined in "azureblob.yaml"

    static async Task Main(string[] args)
    {
        const string key = "counter";
        const int appPort = 8500;

        // Set options
        var options = new DaprOptions
        {
            Sidecar = new DaprSidecarOptions
            {
                AppPort = appPort
            }
        };

        // Build the default Sidekick controller
        var sidekick = new DaprSidekickBuilder().Build();

        var logger = sidekick.LoggerFactory.CreateLogger("my-logging");

        // Start the Dapr Sidecar, this will come up in the background
        sidekick.Sidecar.Start(() => options, DaprCancellationToken.None);

        // 1. Get the current state from the counter for that Redis StoreName
        var counterAsString = await sidekick
            .HttpClientFactory
            .CreateDaprHttpClient().GetStringAsync($"http://localhost:3500/v1.0/state/{StoreName}/{key}");
        var counter = int.TryParse(counterAsString, out var counterInitial) ? counterInitial : 0;
        logger.Log(DaprLogLevel.Information, "counter = {0}", counter);

        // 2. Set new value
        var state = new
        {
            key = key,
            value = counter + 1
        };
        var states = new[]
        {
            state
        };
        await sidekick
           .HttpClientFactory
           .CreateDaprHttpClient().PostAsJsonAsync($"http://localhost:3500/v1.0/state/{StoreName}", states);

        // 3. Get updated value
        counterAsString = await sidekick
            .HttpClientFactory
            .CreateDaprHttpClient().GetStringAsync($"http://localhost:3500/v1.0/state/{StoreName}/{key}");
        counter = int.TryParse(counterAsString, out var counterNew) ? counterNew : 0;
        logger.Log(DaprLogLevel.Information, "counter new = {0}", counter);


        Console.WriteLine("Press ENTER to quit...");
        Console.ReadLine();

        // Shut down the Dapr Sidecar - this is a blocking call
        sidekick.Sidecar.Stop(DaprCancellationToken.None);
    }
}