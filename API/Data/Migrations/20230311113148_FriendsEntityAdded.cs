using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    public partial class FriendsEntityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    AddingToFriendsUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddedToFriendsUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => new { x.AddingToFriendsUserId, x.AddedToFriendsUserId });
                    table.ForeignKey(
                        name: "FK_Friends_Users_AddedToFriendsUserId",
                        column: x => x.AddedToFriendsUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Friends_Users_AddingToFriendsUserId",
                        column: x => x.AddingToFriendsUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friends_AddedToFriendsUserId",
                table: "Friends",
                column: "AddedToFriendsUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friends");
        }
    }
}
