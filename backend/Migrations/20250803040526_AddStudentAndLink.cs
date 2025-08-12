using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnnamHS_App_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentAndLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_StudentId",
                table: "users",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_Students_StudentId",
                table: "users",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_Students_StudentId",
                table: "users");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropIndex(
                name: "IX_users_StudentId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "users");
        }
    }
}
