using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enterprise.Framework.Infrastructure.Migrations
{
    public partial class ArchitectureRefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Employees_AssignedEmployeeId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Labors_Employees_EmployeeId",
                table: "Labors");

            migrationBuilder.DropForeignKey(
                name: "FK_Labors_Maintenances_MaintenanceId",
                table: "Labors");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Assets_AssetId",
                table: "Maintenances");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "TodoItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Maintenances",
                table: "Maintenances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Labors",
                table: "Labors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Assets",
                table: "Assets");

            migrationBuilder.RenameTable(
                name: "Maintenances",
                newName: "Maintenance");

            migrationBuilder.RenameTable(
                name: "Labors",
                newName: "Labor");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Employee");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "Document");

            migrationBuilder.RenameTable(
                name: "Assets",
                newName: "Asset");

            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "Maintenance",
                newName: "IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Maintenances_AssetId",
                table: "Maintenance",
                newName: "IX_Maintenance_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_Labors_MaintenanceId",
                table: "Labor",
                newName: "IX_Labor_MaintenanceId");

            migrationBuilder.RenameIndex(
                name: "IX_Labors_EmployeeId",
                table: "Labor",
                newName: "IX_Labor_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_AssignedEmployeeId",
                table: "Asset",
                newName: "IX_Asset_AssignedEmployeeId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAtUtc",
                table: "Maintenance",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Maintenance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Maintenance",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAtUtc",
                table: "Labor",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Labor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Labor",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAtUtc",
                table: "Employee",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAtUtc",
                table: "Document",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Document",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Document",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAtUtc",
                table: "Asset",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Asset",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Asset",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Maintenance",
                table: "Maintenance",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Labor",
                table: "Labor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employee",
                table: "Employee",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Document",
                table: "Document",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Asset",
                table: "Asset",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Employee_AssignedEmployeeId",
                table: "Asset",
                column: "AssignedEmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Labor_Employee_EmployeeId",
                table: "Labor",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Labor_Maintenance_MaintenanceId",
                table: "Labor",
                column: "MaintenanceId",
                principalTable: "Maintenance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenance_Asset_AssetId",
                table: "Maintenance",
                column: "AssetId",
                principalTable: "Asset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Employee_AssignedEmployeeId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Labor_Employee_EmployeeId",
                table: "Labor");

            migrationBuilder.DropForeignKey(
                name: "FK_Labor_Maintenance_MaintenanceId",
                table: "Labor");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenance_Asset_AssetId",
                table: "Maintenance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Maintenance",
                table: "Maintenance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Labor",
                table: "Labor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employee",
                table: "Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Document",
                table: "Document");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Asset",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Maintenance");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Maintenance");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Maintenance");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Labor");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Labor");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Labor");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Asset");

            migrationBuilder.RenameTable(
                name: "Maintenance",
                newName: "Maintenances");

            migrationBuilder.RenameTable(
                name: "Labor",
                newName: "Labors");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "Document",
                newName: "Documents");

            migrationBuilder.RenameTable(
                name: "Asset",
                newName: "Assets");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Maintenances",
                newName: "IsCompleted");

            migrationBuilder.RenameIndex(
                name: "IX_Maintenance_AssetId",
                table: "Maintenances",
                newName: "IX_Maintenances_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_Labor_MaintenanceId",
                table: "Labors",
                newName: "IX_Labors_MaintenanceId");

            migrationBuilder.RenameIndex(
                name: "IX_Labor_EmployeeId",
                table: "Labors",
                newName: "IX_Labors_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Asset_AssignedEmployeeId",
                table: "Assets",
                newName: "IX_Assets_AssignedEmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Maintenances",
                table: "Maintenances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Labors",
                table: "Labors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Assets",
                table: "Assets",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistryNumbers = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Locations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TodoItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Code",
                table: "Locations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ParentId",
                table: "Locations",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Employees_AssignedEmployeeId",
                table: "Assets",
                column: "AssignedEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Labors_Employees_EmployeeId",
                table: "Labors",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Labors_Maintenances_MaintenanceId",
                table: "Labors",
                column: "MaintenanceId",
                principalTable: "Maintenances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Assets_AssetId",
                table: "Maintenances",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
