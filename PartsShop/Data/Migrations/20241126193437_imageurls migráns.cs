using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsShop.Data.Migrations
{
    public partial class imageurlsmigráns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "Parts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "Parts");
        }
    }
}
