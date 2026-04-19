namespace Enterprise.Framework.Infrastructure.Persistence.Configurations;

using Enterprise.Framework.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;



public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem> {

public void Configure(EntityTypeBuilder<TodoItem> builder) {
    
builder.HasKey(t => t.Id);

        builder.Property(t => t.Title) {
            .HasMaxLength(200) {
            .IsRequired();

    }

}







}
}



