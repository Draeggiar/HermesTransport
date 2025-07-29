# HermesTransport [![Build status](https://github.com/Draeggiar/HermesTransport/actions/workflows/ci.yml/badge.svg)](https://github.com/Draeggiar/HermesTransport/actions)

A .NET library that provides core abstractions for event-driven architecture and messaging. It offers clean interfaces and patterns for building messaging systems with pluggable broker implementations.

[![Serilog](https://github.com/Draeggiar/HermesTransport/blob/master/assets/icon.png)](https://github.com/Draeggiar/HermesTransport)

## Features

- **Core Abstractions**: Clean interfaces for messages, events, commands, and handlers
- **Message Envelopes**: Generic context support for storing any custom context information
- **Publisher/Subscriber Pattern**: Send and receive messages asynchronously
- **Event-Driven Architecture**: Support for events and commands with proper separation
- **Pluggable Brokers**: Abstraction layer for different messaging systems (RabbitMQ, SQL Server, etc.)
- **Type Safety**: Strongly-typed message handling with generics
- **Separated Responsibilities**: Focused interfaces that are easy to implement

## Core Concepts

### Messages
- `IMessage`: Base interface for all messages with ID, timestamp, and correlation support
- `IEvent`: Represents something that has happened in the system
- `ICommand`: Represents a request for action to be performed

### Message Envelopes
- `IMessageEnvelope<TMessage, TContext>`: Wraps messages with additional context information
- Generic context support for any custom context type
- Extension methods for easy envelope creation with fluent API

### Handlers
- `IMessageHandler<T>`: Processes messages of type T
- `IEventHandler<T>`: Processes events of type T  
- `ICommandHandler<T>`: Processes commands of type T

### Publishers/Senders
- `IEventPublisher`: Publishes events to subscribers
- `ICommandSender`: Sends commands for processing
- `IMessagePublisher`: Generic message publishing
- `IMessageSubscriber`: Subscribe to messages with handlers

### Brokers
- `IMessageBroker`: Abstraction for message transport and storage management
- Provides publisher and subscriber instances
- Implementations available via separate plugin libraries

## Quick Start

### 1. Create Messages

```csharp
using HermesTransport;

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
public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"User created: {message.Name} ({message.Email})");
        return Task.CompletedTask;
    }
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    public Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Creating user: {command.Name} ({command.Email})");
        // Process the command...
        return Task.CompletedTask;
    }
}
```

### 3. Working with Message Context

```csharp
// Create custom context
public class UserOperationContext
{
    public string AdminUserId { get; set; }
    public string RequestSource { get; set; }
    public DateTime OperationTime { get; set; }
}

// Create messages with context
var context = new UserOperationContext 
{ 
    AdminUserId = "admin-123", 
    RequestSource = "WebAPI",
    OperationTime = DateTime.UtcNow
};

var envelope = new UserCreatedEvent("user-123", "john@example.com", "John")
    .WithContext(context);
```

### 4. Set Up Messaging with External Broker

```csharp
// Use with a pluggable broker implementation
// (Example with hypothetical RabbitMQ plugin)
using var broker = new RabbitMQMessageBroker(connectionString);
await broker.ConnectAsync();

// Get publisher and subscriber instances from broker
var eventPublisher = broker.GetEventPublisher();
var commandSender = broker.GetCommandSender();
var subscriber = broker.GetSubscriber();

// Subscribe to messages
var handler = new UserCreatedEventHandler();
var subscription = subscriber.Subscribe<UserCreatedEvent>(handler);
await subscription.StartAsync();

// Publish messages
var userEvent = new UserCreatedEvent("user-123", "john@example.com", "John");
await eventPublisher.PublishEventAsync(userEvent);

var createCommand = new CreateUserCommand("jane@example.com", "Jane");
await commandSender.SendCommandAsync(createCommand);
```

## Architecture

The library provides a clean separation between messaging abstractions and transport implementations:

```
┌─────────────────────────────────────────────────────┐
│                 Application Layer                   │
├─────────────────────────────────────────────────────┤
│                IMessageBroker                       │
│           (Connection Management)                   │
│  ┌─────────────────────┬─────────────────────────┐  │
│  │   IEventPublisher   │   IMessageSubscriber    │  │
│  │   ICommandSender    │                         │  │
│  │   IMessagePublisher │                         │  │
│  └─────────────────────┴─────────────────────────┘  │
├─────────────────────────────────────────────────────┤
│  RabbitMQBroker │ SqlServerBroker │ AzureServiceBus │
│  (plugin)       │ (plugin)        │ (plugin)        │
└─────────────────────────────────────────────────────┘
```

## Message Context and Envelopes

HermesTransport supports rich context information that travels with messages using any custom context type:

```csharp
// Custom context types
public class OrderContext
{
    public string CustomerId { get; set; }
    public string OrderId { get; set; }
    public DateTime OrderDate { get; set; }
}

public class RequestContext
{
    public string CorrelationId { get; set; }
    public string UserId { get; set; }
    public string ClientIP { get; set; }
}

var orderCommand = new ProcessOrderCommand(orderId)
    .WithContext(new OrderContext { CustomerId = "cust-123", OrderId = orderId });

var userEvent = new UserCreatedEvent("user-123", "john@example.com", "John")
    .WithContext(new RequestContext { CorrelationId = "req-123", UserId = "admin" });
```

## Extending with External Systems

This core library is designed to be extended with plugin libraries that implement `IMessageBroker`:

- **HermesTransport.RabbitMQ**: RabbitMQ implementation
- **HermesTransport.SqlServer**: SQL Server Service Broker implementation  
- **HermesTransport.AzureServiceBus**: Azure Service Bus implementation
- **HermesTransport.Redis**: Redis Streams implementation

## Project Structure

```
src/
└── HermesTransport/
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