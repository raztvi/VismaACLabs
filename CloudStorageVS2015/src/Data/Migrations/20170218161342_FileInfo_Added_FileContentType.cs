using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class FileInfo_Added_FileContentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "FileSizeInBytes",
                table: "FileInfos",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "FileContentType",
                table: "FileInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileContentType",
                table: "FileInfos");

            migrationBuilder.AlterColumn<int>(
                name: "FileSizeInBytes",
                table: "FileInfos",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
