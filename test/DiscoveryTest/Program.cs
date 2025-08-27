using HermesTransport;
using HermesTransport.Discovery;
using HermesTransport.Messaging;
using System.Reflection;

var discoveryService = new HandlerDiscoveryService();
var currentAssembly = Assembly.GetExecutingAssembly();

Console.WriteLine($"Scanning assembly: {currentAssembly.FullName}");

var handlers = discoveryService.DiscoverHandlers(currentAssembly);

Console.WriteLine($"Found {handlers.Count()} handlers:");
foreach (var handler in handlers)
{
    Console.WriteLine($"  - {handler.HandlerType.Name} handles {handler.MessageType.Name} ({handler.HandlerInterfaceType.Name})");
}

Console.WriteLine("\nDone!");

// Create sample handlers for testing
public record TestEvent(string Id, string Message) : IEvent
{
    public string MessageId => Id;
    public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
    public string MessageType => nameof(TestEvent);
    public string? CorrelationId => null;
    public string Source => "TestApp";
    public string Version => "1.0";
}

public class TestEventHandler : IEventHandler<TestEvent>
{
    public Task HandleAsync(TestEvent message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Handling TestEvent: {message.Message}");
        return Task.CompletedTask;
    }
}
