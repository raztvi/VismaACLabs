using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "FileInfos",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContentType = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    FileSizeInBytes = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_FileInfos", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "FileInfos");
        }
    }
}