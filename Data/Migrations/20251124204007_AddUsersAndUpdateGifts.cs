using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HousewarmingRegistry.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersAndUpdateGifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchasedByUserId",
                table: "Gifts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_PurchasedByUserId",
                table: "Gifts",
                column: "PurchasedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Users_PurchasedByUserId",
                table: "Gifts",
                column: "PurchasedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Users_PurchasedByUserId",
                table: "Gifts");

            migrationBuilder.DropIndex(
                name: "IX_Gifts_PurchasedByUserId",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "PurchasedByUserId",
                table: "Gifts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
