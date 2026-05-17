namespace Enterprise.Framework.Infrastructure.Persistence.Configurations;

using Enterprise.Framework.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RuleDefinitionConfiguration : IEntityTypeConfiguration<RuleDefinition>
{
    public void Configure(EntityTypeBuilder<RuleDefinition> builder)
    {
        builder.ToTable("RuleDefinitions");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasIndex(x => x.Domain);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Domain)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.WorkflowJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");
    }
}
