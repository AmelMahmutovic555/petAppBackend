using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "toBabysit",
                table: "pets",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_pets_toBabysit",
                table: "pets",
                column: "toBabysit");

            migrationBuilder.AddForeignKey(
                name: "FK_pets_users_toBabysit",
                table: "pets",
                column: "toBabysit",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pets_users_toBabysit",
                table: "pets");

            migrationBuilder.DropIndex(
                name: "IX_pets_toBabysit",
                table: "pets");

            migrationBuilder.DropColumn(
                name: "toBabysit",
                table: "pets");
        }
    }
}
