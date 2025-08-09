using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAR.TrackingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNullVolunteerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update NULL QRId values to empty string
            migrationBuilder.Sql("UPDATE Volunteers SET QRId = '' WHERE QRId IS NULL");
            
            // Update NULL Role values to 'Görevli'
            migrationBuilder.Sql("UPDATE Volunteers SET Role = 'Görevli' WHERE Role IS NULL");
            
            // Make columns non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "QRId",
                table: "Volunteers",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Volunteers",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "Görevli",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "QRId",
                table: "Volunteers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Volunteers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);
        }
    }
}
