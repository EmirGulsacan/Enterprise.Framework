namespace Enterprise.Framework.Infrastructure.Migrations;

using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable


}
);

            migrationBuilder.CreateTable(
                name: "AppRoles",
                columns: table => new
                
Id = table.Column<long>(type: "bigint", nullable: false) {
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true) {
}



