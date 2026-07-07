using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificatePublicToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "public_token",
                table: "inspection_certificates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Asigna un UUID único a cada fila existente antes de crear el índice único
            migrationBuilder.Sql("UPDATE inspection_certificates SET public_token = gen_random_uuid() WHERE public_token = '00000000-0000-0000-0000-000000000000'");

            migrationBuilder.CreateIndex(
                name: "ix_certificates_public_token",
                table: "inspection_certificates",
                column: "public_token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_certificates_public_token",
                table: "inspection_certificates");

            migrationBuilder.DropColumn(
                name: "public_token",
                table: "inspection_certificates");
        }
    }
}
