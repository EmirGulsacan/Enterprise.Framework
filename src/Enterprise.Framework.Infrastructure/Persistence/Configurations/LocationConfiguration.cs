namespace Enterprise.Framework.Infrastructure.Persistence.Configurations;

using Enterprise.Framework.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;



public class LocationConfiguration : IEntityTypeConfiguration<Location> {

public void Configure(EntityTypeBuilder<Location> builder) {
    
builder.ToTable("Locations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);

        builder.Property(x => x.RegistryNumbers).HasMaxLength(1000);

        builder.HasOne(x => x.Parent) {
            .WithMany(x => x.Children) {
            .HasForeignKey(x => x.ParentId) {
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Code).IsUnique();

    }

}







}
}
}



