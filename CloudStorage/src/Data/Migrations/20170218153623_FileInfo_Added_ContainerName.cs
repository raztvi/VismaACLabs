using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class FileInfo_Added_ContainerName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "ContainerName",
                "FileInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "ContainerName",
                "FileInfos");
        }
    }
}