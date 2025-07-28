# Hermes

A .NET library that provides core abstractions for event-driven architecture and messaging. It offers clean interfaces and patterns for building messaging systems with pluggable broker implementations.

## Features

- **Core Abstractions**: Clean interfaces for messages, events, commands, and handlers
- **Message Envelopes**: Generic context support for storing correlation IDs, user information, and custom data
- **Publisher/Subscriber Pattern**: Send and receive messages asynchronously
- **Event-Driven Architecture**: Support for events and commands with proper separation
- **Pluggable Brokers**: Abstraction layer for different messaging systems (RabbitMQ, SQL Server, etc.)
- **Type Safety**: Strongly-typed message handling with generics

## Core Concepts

### Messages
- `IMessage`: Base interface for all messages with ID, timestamp, and correlation support
- `IEvent`: Represents something that has happened in the system
- `ICommand`: Represents a request for action to be performed

### Message Envelopes
- `IMessageEnvelope<TMessage, TContext>`: Wraps messages with additional context information
- `MessageContext`: Built-in context type for common scenarios (correlation ID, user info, request data)
- Extension methods for easy envelope creation with fluent API

### Handlers
- `IMessageHandler<T>`: Processes messages of type T
- `IEventHandler<T>`: Processes events of type T  
- `ICommandHandler<T>`: Processes commands of type T
- `IMessageEnvelopeHandler<T, TContext>`: Processes message envelopes with context

### Publishers/Senders
- `IEventPublisher`: Publishes events to subscribers
- `ICommandSender`: Sends commands for processing
- `IMessagePublisher`: Generic message publishing with context support

### Brokers
- `IMessageBroker`: Abstraction for message transport and storage
- Implementations available via separate plugin libraries

## Quick Start

### 1. Create Messages

```csharp
using Hermes.Core;

public class UserCreatedEvent : EventBase
{
    public UserCreatedEvent(string userId, string email, string name) 
        : base("UserService", "1.0")
    {
        UserId = userId;
        Email = email;
        Name = name;
    }

    public string UserId { get; }
    public string Email { get; }
    public string Name { get; }
}

public class CreateUserCommand : CommandBase
{
    public CreateUserCommand(string email, string name) 
        : base("UserService", "CreateUser")
    {
        Email = email;
        Name = name;
    }

    public string Email { get; }
    public string Name { get; }
}
```

### 2. Create Handlers

```csharp
// Simple message handler
public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"User created: {message.Name} ({message.Email})");
        return Task.CompletedTask;
    }
}

// Envelope handler with context
public class UserCreatedEnvelopeHandler : IEventEnvelopeHandler<UserCreatedEvent, MessageContext>
{
    public Task HandleAsync(IMessageEnvelope<UserCreatedEvent, MessageContext> envelope, CancellationToken cancellationToken = default)
    {
        var message = envelope.Message;
        var context = envelope.Context;
        
        Console.WriteLine($"User {message.Name} created by user {context.UserId} in request {context.RequestId}");
        return Task.CompletedTask;
    }
}
```

### 3. Working with Message Context

```csharp
// Create messages with context using extension methods
var command = new CreateUserCommand("john@example.com", "John Doe")
    .WithCorrelationId("req-123")
    .WithUser("admin-user", "Admin")
    .WithRequestContext("req-123", "corr-456", "admin-user");

// Create custom context
var customContext = new { RequestSource = "API", Version = "2.1" };
var envelope = new UserCreatedEvent("user-123", "john@example.com", "John")
    .WithContext(customContext);

// Publish with context
await broker.PublishAsync(command);
await broker.PublishAsync(envelope);
```

### 4. Set Up Messaging with External Broker

```csharp
// Use with a pluggable broker implementation
// (Example with hypothetical RabbitMQ plugin)
using var broker = new RabbitMQMessageBroker(connectionString);
await broker.ConnectAsync();

// Subscribe to messages
var handler = new UserCreatedEventHandler();
var subscription = broker.Subscribe<UserCreatedEvent>(handler);
await subscription.StartAsync();

// Subscribe to envelopes with context
var envelopeHandler = new UserCreatedEnvelopeHandler();
var envelopeSubscription = broker.Subscribe<UserCreatedEvent, MessageContext>(envelopeHandler);
await envelopeSubscription.StartAsync();

// Publish messages
var userEvent = new UserCreatedEvent("user-123", "john@example.com", "John")
    .WithRequestContext("req-123", "corr-456", "admin-user");
    
await broker.PublishAsync(userEvent);
```

## Architecture

The library provides a clean separation between messaging abstractions and transport implementations:

```
┌─────────────────────────────────────────────────────┐
│                 Application Layer                   │
├─────────────────────────────────────────────────────┤
│  IEventPublisher │ ICommandSender │ IMessagePublisher │
├─────────────────────────────────────────────────────┤
│               IMessageBroker                        │
│           (Pluggable Abstraction)                   │
├─────────────────────────────────────────────────────┤
│  RabbitMQBroker │ SqlServerBroker │ AzureServiceBus │
│  (plugin)       │ (plugin)        │ (plugin)        │
└─────────────────────────────────────────────────────┘
```

## Message Context and Envelopes

Hermes supports rich context information that travels with messages:

```csharp
// Built-in MessageContext
var context = new MessageContext
{
    CorrelationId = "corr-123",
    UserId = "user-456", 
    UserName = "John Doe",
    SessionId = "sess-789",
    RequestId = "req-abc",
    ClientIpAddress = "192.168.1.1",
    Properties = { ["source"] = "api", ["version"] = "2.1" }
};

// Custom context types
public class OrderContext
{
    public string CustomerId { get; set; }
    public string OrderId { get; set; }
    public DateTime OrderDate { get; set; }
}

var orderCommand = new ProcessOrderCommand(orderId)
    .WithContext(new OrderContext { CustomerId = "cust-123", OrderId = orderId });
```

## Extending with External Systems

This core library is designed to be extended with plugin libraries that implement `IMessageBroker`:

- **Hermes.RabbitMQ**: RabbitMQ implementation
- **Hermes.SqlServer**: SQL Server Service Broker implementation  
- **Hermes.AzureServiceBus**: Azure Service Bus implementation
- **Hermes.Redis**: Redis Streams implementation

## Project Structure

```
src/
└── Hermes.Core/
    ├── IMessage.cs              # Core message interface
    ├── IEvent.cs                # Event interface
    ├── ICommand.cs              # Command interface
    ├── IMessageHandler.cs       # Handler interfaces (message & envelope)
    ├── IMessagePublisher.cs     # Publisher interfaces with context support
    ├── IMessageSubscriber.cs    # Subscriber interfaces with context support
    ├── IMessageBroker.cs        # Broker abstraction
    ├── IMessageEnvelope.cs      # Message envelope interfaces
    ├── MessageBase.cs           # Base message implementations
    └── MessageContext.cs        # Context types and extension methods
```

## Building

```bash
# Build the solution
dotnet build

# Pack NuGet package
dotnet pack
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.