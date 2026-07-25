using Fullerene.Shared.Contracts;

namespace Fullerene.Shared.Common.Abstractions.Messaging;

public interface IEventPublisher
{
    Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : FullereneMessage;
}