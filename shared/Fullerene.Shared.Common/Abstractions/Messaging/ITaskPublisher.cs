using Fullerene.Shared.Contracts;

namespace Fullerene.Shared.Common.Abstractions.Messaging;

public interface ITaskPublisher
{
    Task PublishTaskAsync<TTask>(TTask task, CancellationToken ct) where TTask : FullereneMessage;
}