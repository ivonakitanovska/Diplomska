using Microsoft.EntityFrameworkCore.Migrations;

namespace PrespaEvents.Web.Data.Migrations
{
    public partial class UpdateModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventPrice",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "EventInCarts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventPrice",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "EventInCarts");
        }
    }
}
