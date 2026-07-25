using Fullerene.Manager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations;

internal sealed class NixPackageRepoConfiguration : IEntityTypeConfiguration<NixPackageRepo>
{
    public void Configure(EntityTypeBuilder<NixPackageRepo> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .HasMethod("gist")
            .HasOperators("gist_trgm_ops")
            .IsUnique(false);

        builder.Property(x => x.GitRepositoryUrl)
            .IsRequired();

        builder.HasMany(x => x.NixRepoCommits)
            .WithOne(x => x.NixRepo)
            .HasForeignKey(x => x.NixRepoId);

        builder.HasMany(x => x.AndroidApps)
            .WithOne(x => x.NixRepo)
            .HasForeignKey(x => x.NixPackageRepoId);
    }
}