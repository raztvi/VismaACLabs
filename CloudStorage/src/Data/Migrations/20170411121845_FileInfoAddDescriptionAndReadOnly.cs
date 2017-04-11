using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class FileInfoAddDescriptionAndReadOnly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FileInfos",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReadOnly",
                table: "FileInfos",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "FileInfos");

            migrationBuilder.DropColumn(
                name: "ReadOnly",
                table: "FileInfos");
        }
    }
}
