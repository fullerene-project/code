using Fullerene.Manager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations;

internal sealed class AndroidAppPackageVersionConfiguration : IEntityTypeConfiguration<AndroidAppPackageVersion>
{
    public void Configure(EntityTypeBuilder<AndroidAppPackageVersion> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.AppName)
            .HasMethod("gist")
            .HasOperators("gist_trgm_ops")
            .IsUnique(false);

        builder.Property(x => x.AppVersionString).IsRequired();
        builder.Property(x => x.NixPackageRevision).IsRequired();
        builder.Property(x => x.NixDerivationHash).IsRequired();
        builder.Property(x => x.AppVersionReleaseDate).IsRequired();
        builder.Property(x => x.ReleaseChannel)
            .HasColumnType("integer")
            .IsRequired();
        builder.Property(x => x.BaseVersionCode).IsRequired();
        builder.Property(x => x.AppLogoUrl).IsRequired();
        builder.Property(x => x.AppName).IsRequired();
        builder.Property(x => x.AppDescription).IsRequired();
        builder.Property(x => x.AppSummary).IsRequired();
        builder.Property(x => x.AppLicense).IsRequired();


        builder.Property(x => x.NixPackageRepoId).IsRequired();
        builder.HasOne(x => x.NixPackageRepo)
            .WithMany()
            .HasForeignKey(x => x.NixPackageRepoId);

        builder.Property(x => x.CommitHash).IsRequired();

        builder.HasOne(x => x.NixRepoCommit)
            .WithMany(x => x.AndroidAppPackageVersions)
            .HasForeignKey(x => new { x.NixPackageRepoId, x.CommitHash })
            .HasPrincipalKey(x => new { x.NixRepoId, x.CommitHash });

        builder.HasMany(x => x.BuildWorkflows)
            .WithOne(x => x.AndroidAppPackageVersion)
            .HasForeignKey(x => x.AndroidAppPackageVersionId);
    }
}