namespace Hermes.Core.Examples;

/// <summary>
/// Example event representing when a user is created.
/// </summary>
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

/// <summary>
/// Example command to create a user.
/// </summary>
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

/// <summary>
/// Example handler for user created events.
/// </summary>
public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"User created: {message.Name} ({message.Email}) with ID: {message.UserId}");
        return Task.CompletedTask;
    }
}

/// <summary>
/// Example handler for create user commands.
/// </summary>
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IEventPublisher _eventPublisher;

    public CreateUserCommandHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(CreateUserCommand message, CancellationToken cancellationToken = default)
    {
        // Simulate creating a user
        var userId = Guid.NewGuid().ToString();
        
        Console.WriteLine($"Processing create user command for: {message.Name} ({message.Email})");
        
        // Simulate some work
        await Task.Delay(100, cancellationToken);
        
        // Publish an event to notify that the user was created
        var userCreatedEvent = new UserCreatedEvent(userId, message.Email, message.Name);
        await _eventPublisher.PublishEventAsync(userCreatedEvent, cancellationToken);
    }
}