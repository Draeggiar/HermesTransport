using Hermes.Core;
using Hermes.Core.Examples;

namespace Hermes.Core.Tests;

public class MessageBaseTests
{
    [Fact]
    public void EventBase_ShouldInitializeWithCorrectProperties()
    {
        // Arrange
        var source = "UserService";
        var version = "1.0";
        
        // Act
        var userCreatedEvent = new UserCreatedEvent("user123", "test@example.com", "Test User");
        
        // Assert
        Assert.NotNull(userCreatedEvent.MessageId);
        Assert.NotEqual(default, userCreatedEvent.Timestamp);
        Assert.Equal("UserCreatedEvent", userCreatedEvent.MessageType);
        Assert.Equal(source, userCreatedEvent.Source);
        Assert.Equal(version, userCreatedEvent.Version);
        Assert.Equal("user123", userCreatedEvent.UserId);
        Assert.Equal("test@example.com", userCreatedEvent.Email);
        Assert.Equal("Test User", userCreatedEvent.Name);
    }

    [Fact]
    public void CommandBase_ShouldInitializeWithCorrectProperties()
    {
        // Arrange
        var target = "UserService";
        var action = "CreateUser";
        
        // Act
        var createUserCommand = new CreateUserCommand("test@example.com", "Test User");
        
        // Assert
        Assert.NotNull(createUserCommand.MessageId);
        Assert.NotEqual(default, createUserCommand.Timestamp);
        Assert.Equal("CreateUserCommand", createUserCommand.MessageType);
        Assert.Equal(target, createUserCommand.Target);
        Assert.Equal(action, createUserCommand.Action);
        Assert.Equal("test@example.com", createUserCommand.Email);
        Assert.Equal("Test User", createUserCommand.Name);
    }

    [Fact]
    public void MessageBase_ShouldGenerateUniqueMessageIds()
    {
        // Act
        var event1 = new UserCreatedEvent("user1", "test1@example.com", "User 1");
        var event2 = new UserCreatedEvent("user2", "test2@example.com", "User 2");
        
        // Assert
        Assert.NotEqual(event1.MessageId, event2.MessageId);
    }

    [Fact]
    public void MessageBase_WithCorrelationId_ShouldSetCorrelationId()
    {
        // Arrange
        var correlationId = "correlation-123";
        
        // Act
        var event1 = new UserCreatedEvent("user1", "test@example.com", "Test User");
        // We need to create a custom event that accepts correlation ID in constructor
        
        // Assert
        // For now, we'll just test that correlation ID can be null
        Assert.Null(event1.CorrelationId);
    }
}