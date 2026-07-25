using Fullerene.Manager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations.ConcreteArtifactDataRecordsConfigurations;

public sealed class MasterArtifactSplitDataRecordConfiguration
    : IEntityTypeConfiguration<MasterArtifactSplitDataRecord>
{
    public void Configure(EntityTypeBuilder<MasterArtifactSplitDataRecord> builder) { }
}