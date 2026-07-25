using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations.ConcreteArtifactDataRecordsConfigurations;

public sealed class StandaloneSingleAbiArtifactDataRecordConfiguration
    : IEntityTypeConfiguration<StandaloneSingleAbiArtifactDataRecord>
{
    public void Configure(EntityTypeBuilder<StandaloneSingleAbiArtifactDataRecord> builder)
    {
        builder.Property(x => x.CpuArchitecture)
            .HasColumnType("integer")
            .IsRequired();
    }
}