using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificatePdfData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "file_path",
                table: "inspection_certificates",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<byte[]>(
                name: "pdf_data",
                table: "inspection_certificates",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pdf_data",
                table: "inspection_certificates");

            migrationBuilder.AlterColumn<string>(
                name: "file_path",
                table: "inspection_certificates",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
