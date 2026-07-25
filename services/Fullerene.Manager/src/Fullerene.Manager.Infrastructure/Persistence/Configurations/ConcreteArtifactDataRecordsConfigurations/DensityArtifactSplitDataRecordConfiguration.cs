using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations.ConcreteArtifactDataRecordsConfigurations;

public sealed class DensityArtifactSplitDataRecordConfiguration
    : IEntityTypeConfiguration<DensityArtifactSplitDataRecord>
{
    public void Configure(EntityTypeBuilder<DensityArtifactSplitDataRecord> builder)
    {
        builder.ComplexProperty(x => x.Density, densityBuilder =>
        {
            densityBuilder.Property(x => x.Dpi)
                .IsRequired(false);

            densityBuilder.Property(x => x.Alias)
                .HasColumnType("integer")
                .IsRequired(false);
        });
    }
}