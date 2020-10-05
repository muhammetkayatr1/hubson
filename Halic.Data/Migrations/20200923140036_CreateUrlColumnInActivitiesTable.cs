using Microsoft.EntityFrameworkCore.Migrations;

namespace Halic.Data.Migrations
{
    public partial class CreateUrlColumnInActivitiesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Activities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Activities");
        }
    }
}
