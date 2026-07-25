using Fullerene.Manager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations;

internal sealed class BuildWorkflowConfiguration : IEntityTypeConfiguration<BuildWorkflow>
{
    public void Configure(EntityTypeBuilder<BuildWorkflow> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AndroidAppPackageVersionId).IsRequired();
        builder.HasOne(x => x.AndroidAppPackageVersion)
            .WithMany(x => x.BuildWorkflows)
            .HasForeignKey(x => x.AndroidAppPackageVersionId);

        builder.HasMany(x => x.Artifacts)
            .WithOne(x => x.BuildWorkflow)
            .HasForeignKey(x => x.BuildWorkflowId);

        builder.HasMany(x => x.WorkflowEvents)
            .WithOne(x => x.BuildWorkflow)
            .HasForeignKey(x => x.BuildWorkflowId);

        // builder.Property(x => x.Status)
        //     .HasConversion(
        //         x => (int)x,
        //         x => (BuildWorkflowStatus)x)
        //     .HasColumnType("integer")
        //     .IsRequired();
        //
        // builder.Property(x => x.StatusDateTimeOffset)
        //     .IsRequired();
    }
}