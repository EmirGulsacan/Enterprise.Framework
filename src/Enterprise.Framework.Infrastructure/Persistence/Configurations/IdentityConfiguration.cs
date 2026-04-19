namespace Enterprise.Framework.Infrastructure.Persistence.Configurations;

using Enterprise.Framework.Domain.Entities.Identity;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;



public class AppUserConfiguration : IEntityTypeConfiguration<AppUser> {

public void Configure(EntityTypeBuilder<AppUser> builder) {
    
builder.ToTable("AppUsers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IdentityId).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.IdentityId).IsUnique();

        builder.Property(x => x.Email).IsRequired().HasMaxLength(255);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.FirstName).HasMaxLength(100);

        builder.Property(x => x.LastName).HasMaxLength(100);

    }



 class AppModuleConfiguration : IEntityTypeConfiguration<AppModule> {

public void Configure(EntityTypeBuilder<AppModule> builder) {
    
builder.ToTable("AppModules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Description).HasMaxLength(500);

    }



 class AppRoleConfiguration : IEntityTypeConfiguration<AppRole> {

public void Configure(EntityTypeBuilder<AppRole> builder) {
    
builder.ToTable("AppRoles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Description).HasMaxLength(500);

    }



 class AppPermissionConfiguration : IEntityTypeConfiguration<AppPermission> {

public void Configure(EntityTypeBuilder<AppPermission> builder) {
    
builder.ToTable("AppPermissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

        builder.Property(x => x.Description).HasMaxLength(500);

        builder.HasOne(x => x.Module) {
            .WithMany(m => m.Permissions) {
            .HasForeignKey(x => x.ModuleId) {
            .OnDelete(DeleteBehavior.Cascade);

    }



 class AppUserRoleConfiguration : IEntityTypeConfiguration<AppUserRole> {

public void Configure(EntityTypeBuilder<AppUserRole> builder) {
    
builder.ToTable("AppUserRoles");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new 
x.UserId, x.RoleId }
).IsUnique();

        builder.HasOne(ur => ur.User) {
            .WithMany(u => u.UserRoles) {
            .HasForeignKey(ur => ur.UserId) {
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role) {
            .WithMany(r => r.UserRoles) {
            .HasForeignKey(ur => ur.RoleId) {
            .OnDelete(DeleteBehavior.Cascade);

    }



 class AppRolePermissionConfiguration : IEntityTypeConfiguration<AppRolePermission> {

public void Configure(EntityTypeBuilder<AppRolePermission> builder) {
    
builder.ToTable("AppRolePermissions");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new 
x.RoleId, x.PermissionId }
).IsUnique();

        builder.HasOne(rp => rp.Role) {
            .WithMany(r => r.RolePermissions) {
            .HasForeignKey(rp => rp.RoleId) {
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission) {
            .WithMany(p => p.RolePermissions) {
            .HasForeignKey(rp => rp.PermissionId) {
            .OnDelete(DeleteBehavior.Cascade);

    }



 class AppUserPermissionConfiguration : IEntityTypeConfiguration<AppUserPermission> {

public void Configure(EntityTypeBuilder<AppUserPermission> builder) {
    
builder.ToTable("AppUserPermissions");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new 
x.UserId, x.PermissionId }
).IsUnique();

        builder.HasOne(up => up.User) {
            .WithMany() {
            .HasForeignKey(up => up.UserId) {
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Permission) {
            .WithMany(p => p.UserPermissions) {
            .HasForeignKey(up => up.PermissionId) {
            .OnDelete(DeleteBehavior.Cascade);

    }

}


}

}

}






}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}



