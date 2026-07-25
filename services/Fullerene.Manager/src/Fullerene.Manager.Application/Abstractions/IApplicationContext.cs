using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Microsoft.EntityFrameworkCore;

namespace Fullerene.Manager.Application.Abstractions;

public interface IApplicationContext
{
    public DbSet<BuildWorkflow> BuildWorkflows { get; }
    public DbSet<AndroidAppPackage> AndroidAppPackages { get; }
    public DbSet<AndroidAppPackageVersion> AndroidAppPackageVersions { get; }
    public DbSet<Artifact> Artifacts { get; }
    public DbSet<ArtifactDataRecord> ArtifactDataRecords { get; }
    public DbSet<NixPackageRepo> NixPackageRepos { get; }
    public DbSet<NixRepoCommit> NixRepoCommits { get; }
    public DbSet<WorkflowEvent> WorkflowEvents { get; }

    public Task SaveChangesAsync(CancellationToken ct);
}