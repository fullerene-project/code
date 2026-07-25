using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Fullerene.Shared.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations.ConcreteArtifactDataRecordsConfigurations;

public sealed class StandaloneUniversalArtifactDataRecordConfiguration
    : IEntityTypeConfiguration<StandaloneUniversalArtifactDataRecord>
{
    public void Configure(EntityTypeBuilder<StandaloneUniversalArtifactDataRecord> builder)
    {
        var cpuArchitectureComparer = new ValueComparer<ICollection<CpuArchitecture>>(
            (c1, c2) => IsCpuArchCollectionsEqual(c1, c2),
            c => GetCpuArchCollectionHash(c),
            c => c.ToList());

        builder.Property(x => x.CpuArchitectures)
            .HasConversion(
                x => x.Select(y => (int)y).ToArray(),
                x => x.Select(y => (CpuArchitecture)y).ToList())
            .HasColumnType("integer[]")
            .IsRequired()
            .Metadata.SetValueComparer(cpuArchitectureComparer);
    }


    private static int GetCpuArchCollectionHash(ICollection<CpuArchitecture> cpuArchitectures)
    {
        return cpuArchitectures
            .Select(x => x.GetHashCode())
            .Aggregate(0, (hash1, hash2) => hash1 ^ hash2);
    }

    private static bool IsCpuArchCollectionsEqual(
        ICollection<CpuArchitecture> c1, ICollection<CpuArchitecture> c2)
    {
        return new HashSet<CpuArchitecture>(c1).SetEquals(c2);
    }
}