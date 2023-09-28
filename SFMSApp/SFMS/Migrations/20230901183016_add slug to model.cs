using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SFMS.Migrations
{
    public partial class addslugtomodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Stage",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Stage");
        }
    }
}
