using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogApp.Dotnet.DAL.Migrations
{
    public partial class PostForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "BlogPosts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_UserID",
                table: "BlogPosts",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserID",
                table: "BlogPosts",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserID",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_UserID",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "BlogPosts");
        }
    }
}
