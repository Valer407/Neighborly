using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neighborly.Migrations
{
    /// <inheritdoc />
    public partial class FixDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Categories_CategoryId",
                table: "Listings");

            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Districts_DistrictId",
                table: "Listings");

            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Listing_Types_ListingTypeId",
                table: "Listings");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Categories_CategoryId",
                table: "Listings",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Districts_DistrictId",
                table: "Listings",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Listing_Types_ListingTypeId",
                table: "Listings",
                column: "ListingTypeId",
                principalTable: "Listing_Types",
                principalColumn: "ListingTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Categories_CategoryId",
                table: "Listings");

            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Districts_DistrictId",
                table: "Listings");

            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Listing_Types_ListingTypeId",
                table: "Listings");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Categories_CategoryId",
                table: "Listings",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Districts_DistrictId",
                table: "Listings",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Listing_Types_ListingTypeId",
                table: "Listings",
                column: "ListingTypeId",
                principalTable: "Listing_Types",
                principalColumn: "ListingTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
