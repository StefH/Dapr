using ConsoleAppWithDI.Services;
using Dapr.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DaprCounter
{
    /// <summary>
    /// Run:
    /// - dapr run --app-id DaprCounter --components-path ./components dotnet run
    /// </summary>
    class Program
    {
        private static readonly CancellationTokenSource _cts = new();

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("CTRL-C pressed...");
                _cts.Cancel();
            };

            using IHost host = CreateHostBuilder(args).Build();

            await Run(host.Services);

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services
                        .AddSingleton<IExampleService, ExampleService>()
                        .AddDaprClient()
                );

        static Task Run(IServiceProvider services)
        {
            using IServiceScope serviceScope = services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var exampleService = provider.GetRequiredService<IExampleService>();
            return exampleService.RunAsync(_cts.Token);
        }
    }
}