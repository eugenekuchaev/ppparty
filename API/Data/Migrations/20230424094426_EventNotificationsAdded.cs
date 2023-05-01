using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    public partial class EventNotificationsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EventNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NotificationMessage = table.Column<string>(type: "TEXT", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventName = table.Column<string>(type: "TEXT", nullable: false),
                    Read = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserEventNotification",
                columns: table => new
                {
                    EventNotificationsId = table.Column<int>(type: "INTEGER", nullable: false),
                    RecipientsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserEventNotification", x => new { x.EventNotificationsId, x.RecipientsId });
                    table.ForeignKey(
                        name: "FK_AppUserEventNotification_AspNetUsers_RecipientsId",
                        column: x => x.RecipientsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserEventNotification_EventNotifications_EventNotificationsId",
                        column: x => x.EventNotificationsId,
                        principalTable: "EventNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserEventNotification_RecipientsId",
                table: "AppUserEventNotification",
                column: "RecipientsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserEventNotification");

            migrationBuilder.DropTable(
                name: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Events");
        }
    }
}
