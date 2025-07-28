namespace Hermes.Core;

/// <summary>
/// Provides helper methods for creating message envelopes.
/// </summary>
public static class MessageEnvelopeExtensions
{
    /// <summary>
    /// Wraps a message in an envelope with the specified context.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to wrap.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="message">The message to wrap.</param>
    /// <param name="context">The context information to include.</param>
    /// <returns>A message envelope containing the message and context.</returns>
    public static MessageEnvelope<TMessage, TContext> WithContext<TMessage, TContext>(this TMessage message, TContext context)
        where TMessage : IMessage
    {
        return new MessageEnvelope<TMessage, TContext>(message, context);
    }
}