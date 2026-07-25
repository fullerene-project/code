namespace Fullerene.Manager.Domain.Models.WorkflowEvents;

public enum WorkflowEventType
{
    BuildQueued = 1,
    BuildStarted = 2,
    BuildSucceeded = 3,
    BuildFailed = 4,
    SigningQueued = 5,
    SigningStarted = 6,
    SigningSucceeded = 7,
    SigningFailed = 8,
}