namespace ConsoleAppWithDI2.Services;

public interface IExampleService
{
    Task RunAsync(CancellationToken cancellationToken);
}