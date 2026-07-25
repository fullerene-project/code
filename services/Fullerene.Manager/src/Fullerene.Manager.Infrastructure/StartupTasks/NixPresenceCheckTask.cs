using Fullerene.Shared.Infrastructure.Services;

namespace Fullerene.Manager.Infrastructure.StartupTasks;

public sealed class NixPresenceCheckTask() : DefaultCliUtilityPresenceChecker("nix", "Nix");