using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations.ConcreteArtifactDataRecordsConfigurations;

public sealed class AssetsArtifactSplitDataRecordConfiguration
    : IEntityTypeConfiguration<AssetsArtifactSplitDataRecord>
{
    public void Configure(EntityTypeBuilder<AssetsArtifactSplitDataRecord> builder)
    {
        builder.Property(x => x.DeliveryType)
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(x => x.AssetModuleType)
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(x => x.TextureCompressionFormat)
            .HasColumnType("integer")
            .IsRequired(false);

        builder.Property(x => x.LanguageTargeting)
            .IsRequired(false);
    }
}