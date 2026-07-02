using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint5Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "checklist_responses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    checklist_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text_value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    bool_value = table.Column<bool>(type: "boolean", nullable: true),
                    numeric_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    complies = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checklist_responses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "evidences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    storage_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    checklist_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    uploaded_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "findings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    requires_correction = table.Column<bool>(type: "boolean", nullable: false),
                    corrective_action = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_resolved = table.Column<bool>(type: "boolean", nullable: false),
                    checklist_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_findings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "inspection_signatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    signer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    signer_document = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    signature_data = table.Column<string>(type: "text", nullable: false),
                    signed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspection_signatures", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_checklist_responses_inspection_item",
                table: "checklist_responses",
                columns: new[] { "inspection_id", "checklist_item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_evidences_inspection_id",
                table: "evidences",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "ix_findings_inspection_id",
                table: "findings",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "ix_inspection_signatures_inspection_id",
                table: "inspection_signatures",
                column: "inspection_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checklist_responses");

            migrationBuilder.DropTable(
                name: "evidences");

            migrationBuilder.DropTable(
                name: "findings");

            migrationBuilder.DropTable(
                name: "inspection_signatures");
        }
    }
}
