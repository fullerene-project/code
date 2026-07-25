using Fullerene.Shared.Infrastructure.Services;

namespace Fullerene.Signer.Infrastructure.StartupTasks;

public sealed class ApkSignerPresenceCheckTask() : DefaultCliUtilityPresenceChecker("apksigner");