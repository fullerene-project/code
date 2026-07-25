using Fullerene.Shared.Infrastructure.Services;

namespace Fullerene.Worker.Infrastructure.StartupTasks;

public sealed class PodmanPresenceCheckTask() : DefaultCliUtilityPresenceChecker("podman", "Podman");