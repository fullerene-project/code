using System.Text.Json;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fullerene.Manager.Infrastructure.Persistence.Configurations;

internal sealed class WorkflowEventConfiguration : IEntityTypeConfiguration<WorkflowEvent>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<WorkflowEvent> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DateTimeOffset)
            .IsRequired();

        builder.Property(x => x.BuildWorkflowId)
            .IsRequired();

        builder.Property(x => x.EventType)
            .IsRequired();

        builder.Property(x => x.Payload)
            .HasColumnType("jsonb")
            .IsRequired()
            .HasConversion(
                x => JsonSerializer.Serialize(x, JsonOptions),
                x => JsonSerializer.Deserialize<WorkflowEventPayload>(x, JsonOptions));

        builder.HasOne(x => x.BuildWorkflow)
            .WithMany(x => x.WorkflowEvents)
            .HasForeignKey(x => x.BuildWorkflowId);
    }
}