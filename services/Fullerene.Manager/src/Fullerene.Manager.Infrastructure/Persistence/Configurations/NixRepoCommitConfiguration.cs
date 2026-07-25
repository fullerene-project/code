using Fullerene.Manager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations;

internal sealed class NixRepoCommitConfiguration : IEntityTypeConfiguration<NixRepoCommit>
{
    public void Configure(EntityTypeBuilder<NixRepoCommit> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasAlternateKey(x => new { x.NixRepoId, x.CommitHash });

        builder.Property(x => x.CommitHash)
            .IsRequired();

        builder.Property(x => x.CommitDateTimeOffset)
            .IsRequired();

        builder.Property(x => x.NixRepoId).IsRequired();
        builder.HasOne(x => x.NixRepo)
            .WithMany(x => x.NixRepoCommits)
            .HasForeignKey(x => x.NixRepoId);

        builder.HasMany(x => x.AndroidAppPackageVersions)
            .WithOne(x => x.NixRepoCommit)
            .HasForeignKey(x => new { x.NixPackageRepoId, x.CommitHash });
    }
}