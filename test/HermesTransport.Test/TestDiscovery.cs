using HermesTransport.Discovery;
using System.Reflection;

var discoveryService = new HandlerDiscoveryService();
var currentAssembly = Assembly.GetExecutingAssembly();

Console.WriteLine($"Scanning assembly: {currentAssembly.FullName}");

var handlers = discoveryService.DiscoverHandlers(currentAssembly);

Console.WriteLine($"Found {handlers.Count()} handlers:");
foreach (var handler in handlers)
{
    Console.WriteLine($"  - {handler.HandlerType.Name} handles {handler.MessageType.Name} ({handler.HandlerInterfaceType.Name})");
}

Console.WriteLine("\nDone!");