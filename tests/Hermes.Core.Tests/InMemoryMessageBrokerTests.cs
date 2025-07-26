using Hermes.Core;
using Hermes.Core.InMemory;
using Hermes.Core.Examples;

namespace Hermes.Core.Tests;

public class InMemoryMessageBrokerTests
{
    [Fact]
    public async Task PublishAsync_ShouldStoreMessageInTopic()
    {
        // Arrange
        using var broker = new InMemoryMessageBroker();
        var userEvent = new UserCreatedEvent("user123", "test@example.com", "Test User");
        
        // Act
        await broker.PublishAsync(userEvent);
        
        // Assert
        var topics = await broker.GetTopicsAsync();
        Assert.Contains("UserCreatedEvent", topics);
    }

    [Fact]
    public async Task PublishAsync_WithSpecificTopic_ShouldUseSpecifiedTopic()
    {
        // Arrange
        using var broker = new InMemoryMessageBroker();
        var userEvent = new UserCreatedEvent("user123", "test@example.com", "Test User");
        var customTopic = "custom-user-events";
        
        // Act
        await broker.PublishAsync(customTopic, userEvent);
        
        // Assert
        var topics = await broker.GetTopicsAsync();
        Assert.Contains(customTopic, topics);
    }

    [Fact]
    public async Task Subscribe_ShouldReceivePublishedMessages()
    {
        // Arrange
        using var broker = new InMemoryMessageBroker();
        var receivedMessages = new List<UserCreatedEvent>();
        var handler = new TestUserCreatedEventHandler(receivedMessages);
        
        // Act
        var subscription = broker.Subscribe<UserCreatedEvent>(handler);
        await subscription.StartAsync();
        
        var userEvent = new UserCreatedEvent("user123", "test@example.com", "Test User");
        await broker.PublishAsync(userEvent);
        
        // Give some time for message processing
        await Task.Delay(100);
        
        // Assert
        Assert.Single(receivedMessages);
        Assert.Equal("user123", receivedMessages[0].UserId);
        Assert.Equal("test@example.com", receivedMessages[0].Email);
        Assert.Equal("Test User", receivedMessages[0].Name);
        
        // Cleanup
        await subscription.StopAsync();
        subscription.Dispose();
    }

    [Fact]
    public async Task Subscribe_WithSpecificTopic_ShouldReceiveMessagesFromTopic()
    {
        // Arrange
        using var broker = new InMemoryMessageBroker();
        var receivedMessages = new List<UserCreatedEvent>();
        var handler = new TestUserCreatedEventHandler(receivedMessages);
        var customTopic = "custom-user-events";
        
        // Act
        var subscription = broker.Subscribe<UserCreatedEvent>(customTopic, handler);
        await subscription.StartAsync();
        
        var userEvent = new UserCreatedEvent("user123", "test@example.com", "Test User");
        await broker.PublishAsync(customTopic, userEvent);
        
        // Give some time for message processing
        await Task.Delay(100);
        
        // Assert
        Assert.Single(receivedMessages);
        
        // Cleanup
        await subscription.StopAsync();
        subscription.Dispose();
    }

    [Fact]
    public async Task CreateTopicAsync_ShouldCreateTopic()
    {
        // Arrange
        using var broker = new InMemoryMessageBroker();
        var topicName = "test-topic";
        
        // Act
        await broker.CreateTopicAsync(topicName);
        
        // Assert
        var topics = await broker.GetTopicsAsync();
        Assert.Contains(topicName, topics);
    }

    [Fact]
    public async Task DeleteTopicAsync_ShouldRemoveTopic()
    {
        // Arrange
        using var broker = new InMemoryMessageBroker();
        var topicName = "test-topic";
        await broker.CreateTopicAsync(topicName);
        
        // Act
        await broker.DeleteTopicAsync(topicName);
        
        // Assert
        var topics = await broker.GetTopicsAsync();
        Assert.DoesNotContain(topicName, topics);
    }

    [Fact]
    public void IsConnected_ShouldReturnTrueByDefault()
    {
        // Arrange & Act
        using var broker = new InMemoryMessageBroker();
        
        // Assert
        Assert.True(broker.IsConnected);
    }

    private class TestUserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        private readonly List<UserCreatedEvent> _receivedMessages;

        public TestUserCreatedEventHandler(List<UserCreatedEvent> receivedMessages)
        {
            _receivedMessages = receivedMessages;
        }

        public Task HandleAsync(UserCreatedEvent message, CancellationToken cancellationToken = default)
        {
            _receivedMessages.Add(message);
            return Task.CompletedTask;
        }
    }
}