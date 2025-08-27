# Handler Autodiscovery

The HermesTransport library now supports automatic discovery and registration of message handlers from assemblies. This feature eliminates the need to manually register each handler in the DI container and creates automatic subscriptions.

## Basic Usage

### Simple Autodiscovery

The easiest way to enable handler autodiscovery is to use the `EnableAutoDiscovery()` extension method:

```csharp
builder.UseHermesTransport(options =>
{
    options.RegisterEventBroker(_ => new MyEventBroker())
           .RegisterCommandBroker(_ => new MyCommandBroker())
           .RegisterMessageBroker(_ => new MyMessageBroker())
           .EnableAutoDiscovery(); // Scans calling assembly and auto-registers handlers
});
```

This will:
- Scan the calling assembly for handlers implementing `IMessageHandler<T>`, `IEventHandler<T>`, or `ICommandHandler<T>`
- Automatically register discovered handlers in the DI container as transient services
- Create automatic subscriptions for all discovered handlers during application startup

### Advanced Configuration

For more control over the discovery process:

```csharp
builder.UseHermesTransport(options =>
{
    options.RegisterEventBroker(_ => new MyEventBroker())
           .RegisterCommandBroker(_ => new MyCommandBroker())
           .RegisterMessageBroker(_ => new MyMessageBroker())
           .EnableHandlerDiscovery(discovery =>
           {
               discovery.ScanAssembly(typeof(MyHandler).Assembly)
                        .ScanAssemblies(additionalAssemblies)
                        .WithTypeFilter(type => type.Namespace?.StartsWith("MyApp.Handlers") == true)
                        .WithDefaultSubscriptionOptions(new SubscriptionOptions
                        {
                            Group = "my-consumer-group",
                            MaxConcurrency = 5
                        });
           });
});
```

### Alternative Extension Methods

```csharp
// Scan specific assembly
options.EnableAutoDiscovery(typeof(MyHandler).Assembly);

// Scan multiple assemblies
options.EnableAutoDiscovery(assembly1, assembly2, assembly3);

// Scan assembly containing specific type
options.EnableAutoDiscovery<MyHandler>();

// Chain scanning with other configuration
options.ScanAssembly(myAssembly)
       .ScanAssemblies(otherAssemblies)
       .WithHandlerFilter(type => !type.IsAbstract);
```

## Handler Discovery Rules

The autodiscovery system finds classes that:
- Are concrete (not abstract or interface)
- Implement one or more of:
  - `IMessageHandler<TMessage>`
  - `IEventHandler<TEvent>`
  - `ICommandHandler<TCommand>`
- Have message types that implement `IMessage`, `IEvent`, or `ICommand`

## Example Handlers

```csharp
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing order created event for order {OrderId}", @event.OrderId);
        // Handle the event
        return Task.CompletedTask;
    }
}

public class ProcessPaymentCommandHandler : ICommandHandler<ProcessPaymentCommand>
{
    public Task HandleAsync(ProcessPaymentCommand command, CancellationToken cancellationToken = default)
    {
        // Process the payment command
        return Task.CompletedTask;
    }
}
```

## Configuration Options

### HandlerDiscoveryOptions

- `IsEnabled`: Whether autodiscovery is enabled
- `ContinueOnError`: Whether to continue registration when an error occurs
- `AssembliesToScan`: List of assemblies to scan
- `DefaultSubscriptionOptions`: Default subscription options for discovered handlers
- `TypeFilter`: Optional filter to apply when discovering handler types

### Subscription Options

You can configure default subscription options for all discovered handlers:

```csharp
discovery.WithDefaultSubscriptionOptions(new SubscriptionOptions
{
    Group = "my-consumer-group",      // Consumer group name
    FromBeginning = false,            // Start from beginning of stream
    MaxConcurrency = 1,               // Max concurrent message processing
    AutoAcknowledge = true            // Auto-acknowledge messages
});
```

## Logging

The autodiscovery process includes comprehensive logging:

```
info: HermesTransport.Services.HandlerRegistrationService[0]
      Starting handler autodiscovery
info: HermesTransport.Services.HandlerRegistrationService[0]
      Registered handler OrderCreatedEventHandler for message OrderCreatedEvent
info: HermesTransport.Services.HandlerRegistrationService[0]
      Registered handler ProcessPaymentCommandHandler for message ProcessPaymentCommand
info: HermesTransport.Services.HandlerRegistrationService[0]
      Successfully registered 2 handlers from 1 assemblies
info: HermesTransport.Services.HandlerRegistrationService[0]
      Handler autodiscovery completed successfully
```

## Error Handling

By default, the autodiscovery process continues even if individual handler registration fails. You can change this behavior:

```csharp
discovery.ContinueOnError = false; // Stop on first error
```

Failed registrations are logged as errors with full exception details.