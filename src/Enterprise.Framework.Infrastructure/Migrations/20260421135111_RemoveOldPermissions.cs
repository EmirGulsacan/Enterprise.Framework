using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enterprise.Framework.Infrastructure.Migrations
{
    public partial class RemoveOldPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"AppRolePermissions\" WHERE \"PermissionId\" IN (SELECT \"Id\" FROM \"AppPermissions\" WHERE \"ModuleId\" IN (SELECT \"Id\" FROM \"AppModules\" WHERE \"Name\" IN ('Organization', 'Organizasyon')));");
            migrationBuilder.Sql("DELETE FROM \"AppPermissions\" WHERE \"ModuleId\" IN (SELECT \"Id\" FROM \"AppModules\" WHERE \"Name\" IN ('Organization', 'Organizasyon'));");
            migrationBuilder.Sql("DELETE FROM \"AppModules\" WHERE \"Name\" IN ('Organization', 'Organizasyon');");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
