using Microsoft.EntityFrameworkCore.Migrations;

namespace SAR.TrackingSystem.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddVolunteerCurrentState : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "CurrentState",
            table: "Volunteers",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CurrentState",
            table: "Volunteers");
    }
}
