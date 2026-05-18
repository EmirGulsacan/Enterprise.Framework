namespace Enterprise.Framework.Infrastructure.Persistence.Configurations;

using Enterprise.Framework.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OperationDefinitionConfiguration : IEntityTypeConfiguration<OperationDefinition>
{
    public void Configure(EntityTypeBuilder<OperationDefinition> builder)
    {
        builder.ToTable("OperationDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
