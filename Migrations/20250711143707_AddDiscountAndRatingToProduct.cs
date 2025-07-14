using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doanwebcuoiki.Migrations
{
    public partial class AddDiscountAndRatingToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "product_price",
                table: "tbl_product",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "product_discount_price",
                table: "tbl_product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "product_rating",
                table: "tbl_product",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "product_review_count",
                table: "tbl_product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "product_sold",
                table: "tbl_product",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "product_discount_price",
                table: "tbl_product");

            migrationBuilder.DropColumn(
                name: "product_rating",
                table: "tbl_product");

            migrationBuilder.DropColumn(
                name: "product_review_count",
                table: "tbl_product");

            migrationBuilder.DropColumn(
                name: "product_sold",
                table: "tbl_product");

            migrationBuilder.AlterColumn<string>(
                name: "product_price",
                table: "tbl_product",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
