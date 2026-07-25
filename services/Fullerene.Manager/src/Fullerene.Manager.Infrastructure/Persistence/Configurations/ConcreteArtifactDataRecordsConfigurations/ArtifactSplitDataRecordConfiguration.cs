using Fullerene.Manager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations.ConcreteArtifactDataRecordsConfigurations;

public sealed class ArtifactSplitDataRecordConfiguration : IEntityTypeConfiguration<ArtifactSplitDataRecord>
{
    public void Configure(EntityTypeBuilder<ArtifactSplitDataRecord> builder)
    {
        builder.Property(x => x.SplitId).IsRequired();
        builder.Property(x => x.ModuleName).IsRequired();
    }
}