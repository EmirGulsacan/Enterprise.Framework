namespace Enterprise.Framework.Infrastructure.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable


migrationBuilder.RenameTable(
                name: "TodoItem",
                newName: "TodoItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoItems",
                table: "TodoItems",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                
Id = table.Column<long>(type: "bigint", nullable: false) {
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    OrganizationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistryNumbers = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true) {
                }
,
                constraints: table =>
                
table.PrimaryKey("PK_Locations", x => x.Id);

                    table.ForeignKey(
                        name: "FK_Locations_Locations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
}



