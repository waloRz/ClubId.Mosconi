using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubId.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertaPaseToSanciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlertaPase",
                table: "sanciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertaPase",
                table: "sanciones");
        }
    }
}
