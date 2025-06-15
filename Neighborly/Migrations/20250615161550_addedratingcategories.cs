using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neighborly.Migrations
{
    /// <inheritdoc />
    public partial class addedratingcategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "User_Ratings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Ratings_CategoryId",
                table: "User_Ratings",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Ratings_Categories_CategoryId",
                table: "User_Ratings",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Ratings_Categories_CategoryId",
                table: "User_Ratings");

            migrationBuilder.DropIndex(
                name: "IX_User_Ratings_CategoryId",
                table: "User_Ratings");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "User_Ratings");
        }
    }
}
