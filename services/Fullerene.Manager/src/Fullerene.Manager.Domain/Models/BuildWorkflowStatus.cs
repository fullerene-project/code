namespace Fullerene.Manager.Domain.Models;

public enum BuildWorkflowStatus
{
    Pending = 100,
    Building = 200,
    BuildSucceeded = 300, 
    Signing = 400,
    SigningSucceeded = 500,
    BuildFailed = 600,
    SigningFailed = 700
}