using Fullerene.Manager.Domain.Models.WorkflowEvents;

namespace Fullerene.Manager.Domain.Models;

public sealed class BuildWorkflow
{
    public Guid Id { get; private set; }
    public Guid AndroidAppPackageVersionId { get; init; }
    public BuildWorkflowStatus Status { get; private set; }

    public AndroidAppPackageVersion AndroidAppPackageVersion { get; private set; }
    public ICollection<Artifact> Artifacts { get; private set; } = new List<Artifact>();
    public ICollection<WorkflowEvent> WorkflowEvents { get; private set; } = new List<WorkflowEvent>();

    private BuildWorkflow() {}
    
    private BuildWorkflow(Guid id, Guid androidAppPackageVersionId, BuildWorkflowStatus status)
    {
        Id = id;
        AndroidAppPackageVersionId = androidAppPackageVersionId;
        Status = status;
    }

    public static BuildWorkflow CreateNew(Guid androidAppPackageVersionId)
    {
        return new BuildWorkflow(
            id: Guid.CreateVersion7(),
            androidAppPackageVersionId: androidAppPackageVersionId,
            status: BuildWorkflowStatus.Pending);
    }
    
    public void BuildStarted()
    {
        TrySetStatus(BuildWorkflowStatus.Building);
    }

    public void BuildFinished(IEnumerable<Artifact> artifacts)
    {
        foreach (var artifact in artifacts)
        {
            if (artifact.IsSigned) throw new Exception("Build can not produce signed artifacts");
            Artifacts.Add(artifact);
        }
        
        TrySetStatus(BuildWorkflowStatus.BuildSucceeded);
    }
    
    public void BuildFailed()
    {
        TrySetStatus(BuildWorkflowStatus.BuildFailed);
    }
    
    public void SigningStarted()
    {
        TrySetStatus(BuildWorkflowStatus.Signing);
    }

    public void SigningFinished(IEnumerable<Artifact> artifacts)
    {
        var signedArtifacts = artifacts.ToList();
        
        if (signedArtifacts.Count != Artifacts.Count)
            throw new Exception("The number of signed artifacts must match the number of unsigned artifacts");
        
        if (signedArtifacts.Any(a => !a.IsSigned))
            throw new Exception("Signing must produce signed artifacts only");
        
        foreach (var signedArtifact in signedArtifacts)
            Artifacts.Add(signedArtifact);
        
        TrySetStatus(BuildWorkflowStatus.SigningSucceeded);
    }

    public void SigningFailed()
    {
        TrySetStatus(BuildWorkflowStatus.SigningFailed);
    }

    private bool TrySetStatus(BuildWorkflowStatus status)
    {
        if (IsTerminalStatus(Status)) return false;

        if (IsValidTransition(Status, status))
        {
            Status = status;
            return true;
        }
        
        return false;
    }

    private static bool IsTerminalStatus(BuildWorkflowStatus status)
    {
        return status == BuildWorkflowStatus.SigningSucceeded ||
               status == BuildWorkflowStatus.SigningFailed ||
               status == BuildWorkflowStatus.BuildFailed;
    }

    private static bool IsValidTransition(
        BuildWorkflowStatus currentStatus, BuildWorkflowStatus nextStatus)
    {
        return (currentStatus, nextStatus) switch
        {
            (BuildWorkflowStatus.Pending, BuildWorkflowStatus.Building) => true,
            (BuildWorkflowStatus.Pending, BuildWorkflowStatus.BuildSucceeded) => true,
            (BuildWorkflowStatus.Pending, BuildWorkflowStatus.BuildFailed) => true,
            (BuildWorkflowStatus.Building, BuildWorkflowStatus.Building) => true,
            (BuildWorkflowStatus.Building, BuildWorkflowStatus.BuildSucceeded) => true,
            (BuildWorkflowStatus.Building, BuildWorkflowStatus.BuildFailed) => true,
            (BuildWorkflowStatus.BuildSucceeded, BuildWorkflowStatus.Signing) => true,
            (BuildWorkflowStatus.BuildSucceeded, BuildWorkflowStatus.SigningSucceeded) => true,
            (BuildWorkflowStatus.BuildSucceeded, BuildWorkflowStatus.SigningFailed) => true,
            (BuildWorkflowStatus.Signing, BuildWorkflowStatus.Signing) => true,
            (BuildWorkflowStatus.Signing, BuildWorkflowStatus.SigningSucceeded) => true,
            (BuildWorkflowStatus.Signing, BuildWorkflowStatus.SigningFailed) => true,
            _ => false
        };
    }
}