using Hermes.Core;
using Hermes.Core.InMemory;
using Hermes.Core.Examples;

Console.WriteLine("Hermes Messaging Demo");
Console.WriteLine("====================");

// Create broker and messaging service
using var broker = new InMemoryMessageBroker();
using var messagingService = new HermesMessagingService(broker);

// Set up event handler
var eventHandler = new UserCreatedEventHandler();
var eventSubscription = messagingService.Subscribe<UserCreatedEvent>(eventHandler);
await eventSubscription.StartAsync();

Console.WriteLine("✓ Event subscription started");

// Set up command handler
var commandHandler = new CreateUserCommandHandler(messagingService);
var commandSubscription = messagingService.Subscribe<CreateUserCommand>(commandHandler);
await commandSubscription.StartAsync();

Console.WriteLine("✓ Command subscription started");

// Demonstrate command processing
Console.WriteLine("\n1. Sending a command:");
var command = new CreateUserCommand("john.doe@example.com", "John Doe");
await messagingService.SendCommandAsync(command);

// Wait a bit for processing
await Task.Delay(200);

// Demonstrate direct event publishing
Console.WriteLine("\n2. Publishing an event directly:");
var directEvent = new UserCreatedEvent("user-456", "jane.smith@example.com", "Jane Smith");
await messagingService.PublishEventAsync(directEvent);

// Wait a bit for processing
await Task.Delay(200);

// Demonstrate custom topic usage
Console.WriteLine("\n3. Using custom topics:");

// Subscribe to a custom topic
var customTopicHandler = new UserCreatedEventHandler();
var customSubscription = messagingService.Subscribe<UserCreatedEvent>("premium-users", customTopicHandler);
await customSubscription.StartAsync();

Console.WriteLine("✓ Custom topic subscription started");

// Publish to the custom topic
var premiumUserEvent = new UserCreatedEvent("premium-789", "premium@example.com", "Premium User");
await messagingService.PublishAsync("premium-users", premiumUserEvent);

// Wait for processing
await Task.Delay(200);

// Show broker information
Console.WriteLine("\n4. Broker Information:");
var topics = await broker.GetTopicsAsync();
Console.WriteLine($"Available topics: {string.Join(", ", topics)}");
Console.WriteLine($"Broker connected: {broker.IsConnected}");

// Cleanup
Console.WriteLine("\n5. Cleaning up...");
await eventSubscription.StopAsync();
await commandSubscription.StopAsync();
await customSubscription.StopAsync();

eventSubscription.Dispose();
commandSubscription.Dispose();
customSubscription.Dispose();

Console.WriteLine("✓ All subscriptions stopped");
Console.WriteLine("\nDemo completed successfully!");
