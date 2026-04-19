namespace Enterprise.Framework.Infrastructure.Persistence.Configurations;

using Enterprise.Framework.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Title).HasMaxLength(100);
        builder.Property(e => e.Department).HasMaxLength(100);
    }
}

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasMaxLength(200).IsRequired();
        builder.Property(a => a.SerialNumber).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Status).HasMaxLength(50);
        
        builder.HasOne(a => a.AssignedEmployee)
            .WithMany(e => e.Assets)
            .HasForeignKey(a => a.AssignedEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class MaintenanceConfiguration : IEntityTypeConfiguration<Maintenance>
{
    public void Configure(EntityTypeBuilder<Maintenance> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Notes).HasMaxLength(1000);
        
        builder.HasOne(m => m.Asset)
            .WithMany(a => a.Maintenances)
            .HasForeignKey(m => m.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class LaborConfiguration : IEntityTypeConfiguration<Labor>
{
    public void Configure(EntityTypeBuilder<Labor> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.HoursWorked).HasColumnType("decimal(18,2)");
        builder.Property(l => l.HourlyRate).HasColumnType("decimal(18,2)");
        
        builder.HasOne(l => l.Maintenance)
            .WithMany(m => m.Labors)
            .HasForeignKey(l => l.MaintenanceId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(l => l.Employee)
            .WithMany(e => e.Labors)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.FileName).HasMaxLength(255).IsRequired();
        builder.Property(d => d.ContentType).HasMaxLength(100);
        builder.Property(d => d.Path).HasMaxLength(1000).IsRequired();
        builder.Property(d => d.RelatedEntityType).HasMaxLength(100).IsRequired();
    }
}
