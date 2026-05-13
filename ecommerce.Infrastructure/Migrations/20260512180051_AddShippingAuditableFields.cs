using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingAuditableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Shippings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Shippings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "Shippings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Shippings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shippings_CreatedById",
                table: "Shippings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Shippings_UpdatedById",
                table: "Shippings",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Shippings_AspNetUsers_CreatedById",
                table: "Shippings",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shippings_AspNetUsers_UpdatedById",
                table: "Shippings",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shippings_AspNetUsers_CreatedById",
                table: "Shippings");

            migrationBuilder.DropForeignKey(
                name: "FK_Shippings_AspNetUsers_UpdatedById",
                table: "Shippings");

            migrationBuilder.DropIndex(
                name: "IX_Shippings_CreatedById",
                table: "Shippings");

            migrationBuilder.DropIndex(
                name: "IX_Shippings_UpdatedById",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Shippings");
        }
    }
}
