using Fullerene.Manager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations;

internal sealed class AndroidAppPackageConfiguration : IEntityTypeConfiguration<AndroidAppPackage>
{
    public void Configure(EntityTypeBuilder<AndroidAppPackage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.NixPackageRepoId, x.NixPackageName, x.AndroidApplicationId })
            .IsUnique();

        builder.Property(x => x.NixPackageRepoId)
            .IsRequired();

        builder.Property(x => x.NixPackageName)
            .IsRequired();

        builder.Property(x => x.AndroidApplicationId)
            .IsRequired();

        builder.Property(x => x.IsTracked)
            .IsRequired();

        builder.HasOne(x => x.NixRepo)
            .WithMany(x => x.AndroidApps)
            .HasForeignKey(x => x.NixPackageRepoId);

        builder.HasMany(x => x.AndroidAppPackageVersions)
            .WithOne(x => x.AndroidAppPackage)
            .HasForeignKey(x => new { x.NixPackageRepoId, x.NixPackageName, x.AndroidApplicationId })
            .HasPrincipalKey(x => new { x.NixPackageRepoId, x.NixPackageName, x.AndroidApplicationId });
    }
}