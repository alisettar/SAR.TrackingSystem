using Microsoft.EntityFrameworkCore.Migrations;

namespace SAR.TrackingSystem.Infrastructure.Migrations;

public partial class AddRescueCountsToSector : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "RescuedCount",
            table: "Sectors",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "ExtricatedCount",
            table: "Sectors",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "LastUpdated",
            table: "Sectors",
            type: "TEXT",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RescuedCount",
            table: "Sectors");

        migrationBuilder.DropColumn(
            name: "ExtricatedCount",
            table: "Sectors");

        migrationBuilder.DropColumn(
            name: "LastUpdated",
            table: "Sectors");
    }
}
