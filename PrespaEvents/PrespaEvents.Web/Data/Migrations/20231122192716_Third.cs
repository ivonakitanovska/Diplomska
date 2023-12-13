using Microsoft.EntityFrameworkCore.Migrations;

namespace PrespaEvents.Web.Data.Migrations
{
    public partial class Third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventInCarts_Events_CartId",
                table: "EventInCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_EventInCarts_Carts_EventId",
                table: "EventInCarts");

            migrationBuilder.AddForeignKey(
                name: "FK_EventInCarts_Carts_CartId",
                table: "EventInCarts",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventInCarts_Events_EventId",
                table: "EventInCarts",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventInCarts_Carts_CartId",
                table: "EventInCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_EventInCarts_Events_EventId",
                table: "EventInCarts");

            migrationBuilder.AddForeignKey(
                name: "FK_EventInCarts_Events_CartId",
                table: "EventInCarts",
                column: "CartId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventInCarts_Carts_EventId",
                table: "EventInCarts",
                column: "EventId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
