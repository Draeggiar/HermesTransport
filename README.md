# Hermes

A .NET library that abstracts concepts used in event-driven architecture and messaging. It provides a clean API to send, broadcast, and subscribe to messages.

## Features

- **Core Abstractions**: Interfaces for messages, events, commands, and handlers
- **Publisher/Subscriber Pattern**: Send and receive messages asynchronously
- **Event-Driven Architecture**: Support for events and commands
- **Pluggable Brokers**: Abstraction layer for different messaging systems
- **In-Memory Implementation**: Built-in implementation for testing and development

## Core Concepts

### Messages
- `IMessage`: Base interface for all messages
- `IEvent`: Represents something that has happened
- `ICommand`: Represents a request for action

### Handlers
- `IMessageHandler<T>`: Processes messages of type T
- `IEventHandler<T>`: Processes events of type T  
- `ICommandHandler<T>`: Processes commands of type T

### Publishers/Senders
- `IEventPublisher`: Publishes events to subscribers
- `ICommandSender`: Sends commands for processing
- `IMessagePublisher`: Generic message publishing

### Brokers
- `IMessageBroker`: Abstraction for message transport and storage
- `InMemoryMessageBroker`: In-memory implementation for testing

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
    private readonly IEventPublisher _eventPublisher;

    public CreateUserCommandHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(CreateUserCommand message, CancellationToken cancellationToken = default)
    {
        // Process the command
        var userId = Guid.NewGuid().ToString();
        
        // Publish an event
        var userCreatedEvent = new UserCreatedEvent(userId, message.Email, message.Name);
        await _eventPublisher.PublishEventAsync(userCreatedEvent, cancellationToken);
    }
}
```

### 3. Set Up Messaging

```csharp
using Hermes.Core;
using Hermes.Core.InMemory;

// Create broker and messaging service
using var broker = new InMemoryMessageBroker();
using var messagingService = new HermesMessagingService(broker);

// Set up event handler
var eventHandler = new UserCreatedEventHandler();
var eventSubscription = messagingService.Subscribe<UserCreatedEvent>(eventHandler);
await eventSubscription.StartAsync();

// Set up command handler
var commandHandler = new CreateUserCommandHandler(messagingService);
var commandSubscription = messagingService.Subscribe<CreateUserCommand>(commandHandler);
await commandSubscription.StartAsync();

// Send a command
var command = new CreateUserCommand("john@example.com", "John Doe");
await messagingService.SendCommandAsync(command);

// Publish an event directly
var @event = new UserCreatedEvent("user123", "jane@example.com", "Jane Smith");
await messagingService.PublishEventAsync(@event);
```

## Architecture

The library is designed to be extensible and pluggable:

```
┌─────────────────────────────────────────────────────┐
│                 Application Layer                   │
├─────────────────────────────────────────────────────┤
│            HermesMessagingService                   │
├─────────────────────────────────────────────────────┤
│  IEventPublisher │ ICommandSender │ IMessageSubscriber │
├─────────────────────────────────────────────────────┤
│                IMessageBroker                       │
├─────────────────────────────────────────────────────┤
│  InMemoryBroker │ RabbitMQBroker │ SqlServerBroker   │
│  (built-in)     │ (plugin)       │ (plugin)          │
└─────────────────────────────────────────────────────┘
```

## Extending with External Systems

The core library provides abstractions that can be implemented by external plugin libraries:

- **RabbitMQ Plugin**: Implement `IMessageBroker` using RabbitMQ
- **SQL Server Plugin**: Implement `IMessageBroker` using SQL Server Service Broker
- **Azure Service Bus Plugin**: Implement `IMessageBroker` using Azure Service Bus
- **Memory Plugin**: Use the built-in `InMemoryMessageBroker`

## Project Structure

```
src/
└── Hermes.Core/
    ├── IMessage.cs              # Core message interface
    ├── IEvent.cs                # Event interface
    ├── ICommand.cs              # Command interface
    ├── IMessageHandler.cs       # Handler interfaces
    ├── IMessagePublisher.cs     # Publisher interfaces
    ├── IMessageSubscriber.cs    # Subscriber interfaces
    ├── IMessageBroker.cs        # Broker abstraction
    ├── MessageBase.cs           # Base message implementations
    ├── HermesMessagingService.cs # Main service facade
    ├── InMemory/
    │   ├── InMemoryMessageBroker.cs
    │   └── InMemorySubscription.cs
    └── Examples/
        └── UserExamples.cs      # Example implementations

tests/
└── Hermes.Core.Tests/
    ├── MessageBaseTests.cs
    ├── InMemoryMessageBrokerTests.cs
    └── HermesMessagingServiceTests.cs
```

## Building and Testing

```bash
# Build the solution
dotnet build

# Run tests
dotnet test

# Pack NuGet package
dotnet pack
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.