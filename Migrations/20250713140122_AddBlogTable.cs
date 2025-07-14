using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doanwebcuoiki.Migrations
{
    public partial class AddBlogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_blog",
                columns: table => new
                {
                    blog_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    blog_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    blog_image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    blog_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_blog", x => x.blog_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_blog");
        }
    }
}
