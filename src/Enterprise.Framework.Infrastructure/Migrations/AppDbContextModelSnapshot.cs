namespace Enterprise.Framework.Infrastructure.Migrations;

using System;

using Enterprise.Framework.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Infrastructure;

using Microsoft.EntityFrameworkCore.Metadata;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable


SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Enterprise.Framework.Domain.Entities.Branch", b =>
                
b.Property<long>("Id") {
                        .ValueGeneratedOnAdd() {
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Code") {
                        .IsRequired() {
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name") {
                        .IsRequired() {
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganizationId") {
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Branches");

                




}
}
}
}
}
}
}



