using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint9_Certificates_History_Notifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inspection_certificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    certificate_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    issued_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    issued_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspection_certificates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "inspection_status_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    previous_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    new_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    changed_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspection_status_history", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    reference_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_certificates_inspection_id",
                table: "inspection_certificates",
                column: "inspection_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_certificates_number",
                table: "inspection_certificates",
                column: "certificate_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inspection_history_inspection_id",
                table: "inspection_status_history",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_unread",
                table: "notifications",
                columns: new[] { "user_id", "is_read" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inspection_certificates");

            migrationBuilder.DropTable(
                name: "inspection_status_history");

            migrationBuilder.DropTable(
                name: "notifications");
        }
    }
}
