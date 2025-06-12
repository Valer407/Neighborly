using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neighborly.Migrations
{
    /// <inheritdoc />
    public partial class ThirdCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Cities_CityId",
                table: "Listings");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Cities_CityId",
                table: "Listings",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Cities_CityId",
                table: "Listings");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Cities_CityId",
                table: "Listings",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
