namespace ConsoleAppWithDI.Services
{
    public interface IExampleService
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}