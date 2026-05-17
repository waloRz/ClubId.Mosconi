using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubId.Migrations
{
    /// <inheritdoc />
    public partial class AddColorToCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "categorias",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "categorias");
        }
    }
}
