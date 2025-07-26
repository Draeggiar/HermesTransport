using Hermes.Core;
using Hermes.Core.InMemory;
using Hermes.Core.Examples;

namespace Hermes.Core.Tests;

public class HermesMessagingServiceTests
{
    [Fact]
    public async Task PublishEventAsync_ShouldPublishEventThroughBroker()
    {
        // Arrange
        using var broker = new InMemoryMessageBroker();
        using var service = new HermesMessagingService(broker);
        var receivedEvents = new List<UserCreatedEvent>();
        var handler = new TestEventHandler(receivedEvents);
        
        var subscription = service.Subscribe<UserCreatedEvent>(handler);
        await subscription.StartAsync();
        
        // Act
        var userEvent = new UserCreatedEvent("user123", "test@example.com", "Test User");
        await service.PublishEventAsync(userEvent);
        
        // Give some time for message processing
        await Task.Delay(100);
        
        // Assert
        Assert.Single(receivedEvents);
        Assert.Equal("user123", receivedEvents[0].UserId);
        
        // Cleanup
        await subscription.StopAsync();
        subscription.Dispose();
    }

    [Fact]
    public async Task SendCommandAsync_ShouldSendCommandThroughBroker()
    {
        // Arrange
        using var broker = new InMemoryMessageBroker();
        using var service = new HermesMessagingService(broker);
        var receivedCommands = new List<CreateUserCommand>();
        var handler = new TestCommandHandler(receivedCommands);
        
        var subscription = service.Subscribe<CreateUserCommand>(handler);
        await subscription.StartAsync();
        
        // Act
        var command = new CreateUserCommand("test@example.com", "Test User");
        await service.SendCommandAsync(command);
        
        // Give some time for message processing
        await Task.Delay(100);
        
        // Assert
        Assert.Single(receivedCommands);
        Assert.Equal("test@example.com", receivedCommands[0].Email);
        
        // Cleanup
        await subscription.StopAsync();
        subscription.Dispose();
    }

    [Fact]
    public async Task PublishAsync_WithTopic_ShouldPublishToSpecificTopic()
    {
        // Arrange
        using var broker = new InMemoryMessageBroker();
        using var service = new HermesMessagingService(broker);
        var receivedEvents = new List<UserCreatedEvent>();
        var handler = new TestEventHandler(receivedEvents);
        var customTopic = "custom-events";
        
        var subscription = service.Subscribe<UserCreatedEvent>(customTopic, handler);
        await subscription.StartAsync();
        
        // Act
        var userEvent = new UserCreatedEvent("user123", "test@example.com", "Test User");
        await service.PublishAsync(customTopic, userEvent);
        
        // Give some time for message processing
        await Task.Delay(100);
        
        // Assert
        Assert.Single(receivedEvents);
        
        // Cleanup
        await subscription.StopAsync();
        subscription.Dispose();
    }

    [Fact]
    public async Task EnsureConnectedAsync_WhenBrokerIsConnected_ShouldNotThrow()
    {
        // Arrange
        using var broker = new InMemoryMessageBroker();
        using var service = new HermesMessagingService(broker);
        
        // Act & Assert
        await service.EnsureConnectedAsync();
        Assert.True(broker.IsConnected);
    }

    [Fact]
    public void Broker_ShouldReturnUnderlyingBroker()
    {
        // Arrange
        using var broker = new InMemoryMessageBroker();
        using var service = new HermesMessagingService(broker);
        
        // Act & Assert
        Assert.Same(broker, service.Broker);
    }

    private class TestEventHandler : IEventHandler<UserCreatedEvent>
    {
        private readonly List<UserCreatedEvent> _receivedEvents;

        public TestEventHandler(List<UserCreatedEvent> receivedEvents)
        {
            _receivedEvents = receivedEvents;
        }

        public Task HandleAsync(UserCreatedEvent message, CancellationToken cancellationToken = default)
        {
            _receivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }

    private class TestCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly List<CreateUserCommand> _receivedCommands;

        public TestCommandHandler(List<CreateUserCommand> receivedCommands)
        {
            _receivedCommands = receivedCommands;
        }

        public Task HandleAsync(CreateUserCommand message, CancellationToken cancellationToken = default)
        {
            _receivedCommands.Add(message);
            return Task.CompletedTask;
        }
    }
}