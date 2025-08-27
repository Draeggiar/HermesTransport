using HermesTransport;
using HermesTransport.Brokers;
using HermesTransport.Configuration;
using HermesTransport.Extensions;
using HermesTransport.Messaging;
using HermesTransport.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args);

builder.UseHermesTransport(options =>
{
    // Register the same in-memory broker for all message types for testing
    options.RegisterEventBroker(_ => new InMemoryBroker())
           .RegisterCommandBroker(_ => new InMemoryBroker())
           .RegisterMessageBroker(_ => new InMemoryBroker())
           .EnableAutoDiscovery(); // This will scan the current assembly for handlers and auto-register them
});

// No need to manually register handlers - they're auto-discovered and registered

var host = builder.Build();

Console.WriteLine("Starting HermesTransport with handler autodiscovery...");
await host.StartAsync();

Console.WriteLine("Host started. Handlers should be auto-discovered and registered.");
Console.WriteLine("Press any key to stop...");
Console.ReadKey();

await host.StopAsync();
Console.WriteLine("Host stopped.");

// Create sample messages and handlers for testing
public record TestEvent(string Id, string Message) : IEvent
{
    public string MessageId => Id;
    public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
    public string MessageType => nameof(TestEvent);
    public string? CorrelationId => null;
    public string Source => "TestApp";
    public string Version => "1.0";
}

public record TestCommand(string Id, string Action) : ICommand
{
    public string MessageId => Id;
    public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
    public string MessageType => nameof(TestCommand);
    public string? CorrelationId => null;
    public string Target => "TestService";
    string ICommand.Action => Action;
}

public record TestMessage(string Id, string Content) : IMessage
{
    public string MessageId => Id;
    public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
    public string MessageType => nameof(TestMessage);
    public string? CorrelationId => null;
}

public class TestEventHandler : IEventHandler<TestEvent>
{
    private readonly ILogger<TestEventHandler> _logger;

    public TestEventHandler(ILogger<TestEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(TestEvent message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling TestEvent: {Message}", message.Message);
        return Task.CompletedTask;
    }
}

public class TestCommandHandler : ICommandHandler<TestCommand>
{
    private readonly ILogger<TestCommandHandler> _logger;

    public TestCommandHandler(ILogger<TestCommandHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(TestCommand message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling TestCommand: {Action}", message.Action);
        return Task.CompletedTask;
    }
}

public class TestMessageHandler : IMessageHandler<TestMessage>
{
    private readonly ILogger<TestMessageHandler> _logger;

    public TestMessageHandler(ILogger<TestMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(TestMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling TestMessage: {Content}", message.Content);
        return Task.CompletedTask;
    }
}

// Simple in-memory broker for testing
public class InMemoryBroker : IMessageBroker
{
    public bool IsConnected => true;

    public Task ConnectAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task DisconnectAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task CreateTopicAsync(string topic, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task DeleteTopicAsync(string topic, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task<IEnumerable<string>> GetTopicsAsync(CancellationToken cancellationToken = default) => 
        Task.FromResult(Enumerable.Empty<string>());

    public IMessagePublisher GetPublisher() => new InMemoryPublisher();
    public IMessageSubscriber GetSubscriber() => new InMemorySubscriber();
    public IEventPublisher GetEventPublisher() => new InMemoryEventPublisher();
    public ICommandSender GetCommandSender() => new InMemoryCommandSender();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

public class InMemoryPublisher : IMessagePublisher
{
    public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        Console.WriteLine($"Published message: {message.MessageType}");
        return Task.CompletedTask;
    }

    public Task PublishAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        Console.WriteLine($"Published message to topic {topic}: {message.MessageType}");
        return Task.CompletedTask;
    }
}

public class InMemorySubscriber : IMessageSubscriber
{
    public ISubscription Subscribe<TMessage>(IMessageHandler<TMessage> handler, SubscriptionOptions? options = null) where TMessage : IMessage
    {
        Console.WriteLine($"Subscribed to message type: {typeof(TMessage).Name}");
        return new InMemorySubscription();
    }

    public ISubscription Subscribe<TMessage>(string topic, IMessageHandler<TMessage> handler, SubscriptionOptions? options = null) where TMessage : IMessage
    {
        Console.WriteLine($"Subscribed to topic {topic} for message type: {typeof(TMessage).Name}");
        return new InMemorySubscription();
    }
}

public class InMemoryEventPublisher : IEventPublisher
{
    public Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        Console.WriteLine($"Published event: {@event.MessageType}");
        return Task.CompletedTask;
    }
}

public class InMemoryCommandSender : ICommandSender
{
    public Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        Console.WriteLine($"Sent command: {command.MessageType}");
        return Task.CompletedTask;
    }
}

public class InMemorySubscription : ISubscription
{
    public string SubscriptionId => Guid.NewGuid().ToString();
    public bool IsActive => true;

    public void Complete() { }

    public Task DeliverMessageAsync(IMessage message, CancellationToken cancellation = default)
    {
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
