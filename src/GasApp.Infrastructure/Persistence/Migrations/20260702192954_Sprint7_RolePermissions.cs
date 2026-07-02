using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint7_RolePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    permission = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_granted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_role_permission",
                table: "role_permissions",
                columns: new[] { "role", "permission" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_permissions");
        }
    }
}
