using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations.ConcreteArtifactDataRecordsConfigurations;

public sealed class AbiArtifactSplitDataRecordConfiguration
    : IEntityTypeConfiguration<AbiArtifactSplitDataRecord>
{
    public void Configure(EntityTypeBuilder<AbiArtifactSplitDataRecord> builder)
    {
        builder.Property(x => x.CpuArchitecture)
            .HasColumnType("integer")
            .IsRequired();
    }
}