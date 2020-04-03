using Microsoft.EntityFrameworkCore.Migrations;

namespace CVweb.Migrations
{
    public partial class newup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "inwork",
                table: "CVzs",
                nullable: false,
                defaultValue: false);

           

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "CVzs",
                nullable: false,
                defaultValue: 0);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "inwork",
                table: "CVzs");

           

            migrationBuilder.DropColumn(
                name: "type",
                table: "CVzs");
            
        }
    }
}
