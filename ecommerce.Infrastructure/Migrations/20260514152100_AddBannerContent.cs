using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBannerContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Banners",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Banners");
        }
    }
}
