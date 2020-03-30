using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CVweb.Migrations
{
    public partial class cv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

           

            migrationBuilder.CreateTable(
                name: "CVzs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cvpath = table.Column<string>(nullable: true),
                    photopath = table.Column<string>(nullable: true),
                    notes = table.Column<string>(nullable: true),
                    mid = table.Column<bool>(nullable: false),
                    know = table.Column<bool>(nullable: false),
                    arabic = table.Column<bool>(nullable: false),
                    linkedin = table.Column<bool>(nullable: false),
                    fast = table.Column<bool>(nullable: false),
                    cato = table.Column<int>(nullable: false),
                    userID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVzs", x => x.id);
                });

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "CVzs");

         
        }
    }
}
