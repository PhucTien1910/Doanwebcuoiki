using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doanwebcuoiki.Migrations
{
    public partial class AddCreatedAtToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "tbl_product",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "tbl_product");
        }
    }
}
