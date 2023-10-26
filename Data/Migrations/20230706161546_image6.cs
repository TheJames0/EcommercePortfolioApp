using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApp.Data.Migrations
{
    public partial class image6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posting_PostedImage_ImageId",
                table: "Posting");

            migrationBuilder.DropTable(
                name: "PostedImage");

            migrationBuilder.DropIndex(
                name: "IX_Posting_ImageId",
                table: "Posting");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Posting");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Posting",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Posting");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Posting",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PostedImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostedImage", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posting_ImageId",
                table: "Posting",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posting_PostedImage_ImageId",
                table: "Posting",
                column: "ImageId",
                principalTable: "PostedImage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
