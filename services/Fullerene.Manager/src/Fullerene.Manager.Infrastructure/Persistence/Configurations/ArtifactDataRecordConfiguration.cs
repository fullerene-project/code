using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Fullerene.Shared.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations;

internal sealed class ArtifactDataRecordConfiguration : IEntityTypeConfiguration<ArtifactDataRecord>
{
    public void Configure(EntityTypeBuilder<ArtifactDataRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ArtifactType)
            .HasColumnType("integer")
            .IsRequired();

        builder.HasDiscriminator(x => x.ArtifactType)
            .HasValue<StandaloneUniversalArtifactDataRecord>(ArtifactType.StandaloneUniversal)
            .HasValue<StandaloneSingleAbiArtifactDataRecord>(ArtifactType.StandaloneSingleAbi)
            .HasValue<BaseArtifactSplitDataRecord>(ArtifactType.BaseSplit)
            .HasValue<AbiArtifactSplitDataRecord>(ArtifactType.AbiSplit)
            .HasValue<DensityArtifactSplitDataRecord>(ArtifactType.DensitySplit)
            .HasValue<FeatureArtifactSplitDataRecord>(ArtifactType.FeatureSplit)
            .HasValue<LanguageArtifactSplitDataRecord>(ArtifactType.LanguageSplit)
            .HasValue<AssetsArtifactSplitDataRecord>(ArtifactType.AssetsSplit);

        builder.Property(x => x.VersionCode).IsRequired();
        builder.Property(x => x.MinApiLevel).IsRequired();
        builder.Property(x => x.TargetApiLevel).IsRequired();
    }
}