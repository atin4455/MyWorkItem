using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyWorkItem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    Account = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserWorkItemStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WorkItemId = table.Column<int>(type: "int", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkItemStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWorkItemStatuses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWorkItemStatuses_WorkItems_WorkItemId",
                        column: x => x.WorkItemId,
                        principalTable: "WorkItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Account", "CreatedAt", "Password", "Role" },
                values: new object[,]
                {
                    { 1, "admin", new DateTime(2026, 4, 13, 0, 0, 0, 0, DateTimeKind.Utc), "admin123", "Admin" },
                    { 2, "user1", new DateTime(2026, 4, 13, 0, 0, 0, 0, DateTimeKind.Utc), "user123", "User" },
                    { 3, "user2", new DateTime(2026, 4, 13, 0, 0, 0, 0, DateTimeKind.Utc), "user123", "User" }
                });

            migrationBuilder.InsertData(
                table: "WorkItems",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Check IDE and DB connection.", true, "Prepare interview environment", null },
                    { 2, new DateTime(2026, 4, 12, 23, 50, 0, 0, DateTimeKind.Utc), "Prepare C4 context and container level explanation.", true, "Review system architecture", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Account",
                table: "Users",
                column: "Account",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkItemStatuses_UserId_WorkItemId",
                table: "UserWorkItemStatuses",
                columns: new[] { "UserId", "WorkItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkItemStatuses_WorkItemId",
                table: "UserWorkItemStatuses",
                column: "WorkItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserWorkItemStatuses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkItems");
        }
    }
}
