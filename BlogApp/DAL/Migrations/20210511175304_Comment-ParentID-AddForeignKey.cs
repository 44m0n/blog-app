using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogApp.Dotnet.DAL.Migrations
{
    public partial class CommentParentIDAddForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ParentID",
                table: "Comments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentID",
                table: "Comments",
                column: "ParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentID",
                table: "Comments",
                column: "ParentID",
                principalTable: "Comments",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentID",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentID",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "ParentID",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
