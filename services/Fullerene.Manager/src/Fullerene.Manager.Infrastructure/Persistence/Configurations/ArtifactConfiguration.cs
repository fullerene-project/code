using Fullerene.Manager.Domain.Models;
using Fullerene.Shared.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations;

internal sealed class ArtifactConfiguration : IEntityTypeConfiguration<Artifact>
{
    public void Configure(EntityTypeBuilder<Artifact> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsSigned).IsRequired();

        builder.ComplexProperty(x => x.FileData, fileDataBuilder =>
        {
            fileDataBuilder.IsRequired();
            ConfigureStorageFileData(fileDataBuilder);
        });

        builder.ComplexProperty(x => x.IdSigFileData, fileDataBuilder =>
        {
            fileDataBuilder.IsRequired(false);
            ConfigureStorageFileData(fileDataBuilder);
        });

        builder.Property(x => x.ArtifactDataRecordId).IsRequired();
        builder.HasOne(x => x.ArtifactDataRecord)
            .WithMany(x => x.Artifacts)
            .HasForeignKey(x => x.ArtifactDataRecordId);

        builder.Property(x => x.BuildWorkflowId).IsRequired();
        builder.HasOne(x => x.BuildWorkflow)
            .WithMany(x => x.Artifacts)
            .HasForeignKey(x => x.BuildWorkflowId);
    }

    private static void ConfigureStorageFileData(ComplexPropertyBuilder<StorageFileData> fileDataBuilder)
    {
        fileDataBuilder.Property(x => x.FileName)
            .IsRequired();
        fileDataBuilder.Property(x => x.FileStorageKey)
            .IsRequired();
        fileDataBuilder.Property(x => x.FileSha256)
            .HasMaxLength(64)
            .IsRequired();
        fileDataBuilder.Property(x => x.FileSizeBytes)
            .IsRequired();
    }
}