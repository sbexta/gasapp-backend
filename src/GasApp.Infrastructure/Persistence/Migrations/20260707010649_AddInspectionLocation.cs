using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "location_captured_at",
                table: "inspections",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "location_lat",
                table: "inspections",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "location_lng",
                table: "inspections",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "location_captured_at",
                table: "inspections");

            migrationBuilder.DropColumn(
                name: "location_lat",
                table: "inspections");

            migrationBuilder.DropColumn(
                name: "location_lng",
                table: "inspections");
        }
    }
}
