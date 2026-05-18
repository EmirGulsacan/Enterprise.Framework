namespace Enterprise.Framework.Infrastructure.Persistence.Configurations;

using Enterprise.Framework.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OperationRuleMappingConfiguration : IEntityTypeConfiguration<OperationRuleMapping>
{
    public void Configure(EntityTypeBuilder<OperationRuleMapping> builder)
    {
        builder.ToTable("OperationRuleMappings");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.OperationId, x.RuleDefinitionId })
            .IsUnique();

        builder.HasOne(x => x.Operation)
            .WithMany(x => x.RuleMappings)
            .HasForeignKey(x => x.OperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RuleDefinition)
            .WithMany()
            .HasForeignKey(x => x.RuleDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
