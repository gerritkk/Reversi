using Microsoft.EntityFrameworkCore.Migrations;

namespace ReversiApp.Migrations.Identity
{
    public partial class AanpassingSpeler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HuidigeKleur",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HuidigeKleur",
                table: "AspNetUsers");
        }
    }
}
