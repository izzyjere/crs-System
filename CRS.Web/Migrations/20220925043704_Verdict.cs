using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRS.Web.Migrations
{
    public partial class Verdict : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Verdict",
                table: "Judgements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Verdict",
                table: "Judgements");
        }
    }
}
