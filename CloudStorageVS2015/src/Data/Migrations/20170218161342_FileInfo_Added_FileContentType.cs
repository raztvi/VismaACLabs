using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class FileInfo_Added_FileContentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                "FileSizeInBytes",
                "FileInfos",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                "FileContentType",
                "FileInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "FileContentType",
                "FileInfos");

            migrationBuilder.AlterColumn<int>(
                "FileSizeInBytes",
                "FileInfos",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}