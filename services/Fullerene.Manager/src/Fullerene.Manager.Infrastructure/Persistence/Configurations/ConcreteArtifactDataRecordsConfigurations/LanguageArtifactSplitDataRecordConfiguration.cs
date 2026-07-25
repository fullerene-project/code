using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations.ConcreteArtifactDataRecordsConfigurations;

public sealed class LanguageArtifactSplitDataRecordConfiguration
    : IEntityTypeConfiguration<LanguageArtifactSplitDataRecord>
{
    public void Configure(EntityTypeBuilder<LanguageArtifactSplitDataRecord> builder)
    {
        builder.Property(x => x.LanguageTargeting).IsRequired();
    }
}