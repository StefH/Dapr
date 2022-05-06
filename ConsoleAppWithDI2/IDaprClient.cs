using Dapr.Client;

namespace ConsoleAppWithDI2
{
    [ProxyInterfaceGenerator.Proxy(typeof(DaprClient))]
    public partial interface IDaprClient
    {
    }
}