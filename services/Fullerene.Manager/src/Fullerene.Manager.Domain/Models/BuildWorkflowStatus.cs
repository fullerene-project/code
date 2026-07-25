namespace Fullerene.Manager.Domain.Models;

public enum BuildWorkflowStatus
{
    Pending = 100,
    BuildRunning = 200,
    Building = 300,
    Success = 400,
    BuildFailed = 500,
}