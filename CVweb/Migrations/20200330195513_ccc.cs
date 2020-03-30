using Microsoft.EntityFrameworkCore.Migrations;

namespace CVweb.Migrations
{
    public partial class ccc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
   name: "paying",
   table: "CVzs",
   nullable: false,
   defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
