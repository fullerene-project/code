using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Util;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace Fullerene.Manager.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions options) : DbContext(options), IApplicationContext
{
    public DbSet<BuildWorkflow> BuildWorkflows { get; set; }
    public DbSet<AndroidAppPackage> AndroidAppPackages { get; set; }
    public DbSet<AndroidAppPackageVersion> AndroidAppPackageVersions { get; set; }
    public DbSet<Artifact> Artifacts { get; set; }
    public DbSet<ArtifactDataRecord> ArtifactDataRecords { get; set; }
    public DbSet<NixPackageRepo> NixPackageRepos { get; set; }
    public DbSet<NixRepoCommit> NixRepoCommits { get; set; }
    public DbSet<WorkflowEvent> WorkflowEvents { get; set; }

    async Task IApplicationContext.SaveChangesAsync(CancellationToken ct)
    {
        await base.SaveChangesAsync(ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("pg_trgm");

        modelBuilder
            .HasDbFunction(typeof(ApplicationDbFunctions).GetMethod(nameof(ApplicationDbFunctions.FuzzySimilar)))
            .HasTranslation(args => new PgUnknownBinaryExpression(
                left: args[0],
                right: args[1],
                binaryOperator: "%",
                type: typeof(bool)));

        modelBuilder
            .HasDbFunction(typeof(ApplicationDbFunctions).GetMethod(nameof(ApplicationDbFunctions.FuzzySimilarityDistance)))
            .HasTranslation(args => new PgUnknownBinaryExpression(
                left: args[0],
                right: args[1],
                binaryOperator: "<->",
                type: typeof(double)));

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}