using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class student_have_only_legal_guardian : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentUser");

            migrationBuilder.AddColumn<Guid>(
                name: "LegalGuardianId",
                table: "Students",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Students_LegalGuardianId",
                table: "Students",
                column: "LegalGuardianId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Users_LegalGuardianId",
                table: "Students",
                column: "LegalGuardianId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Users_LegalGuardianId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_LegalGuardianId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LegalGuardianId",
                table: "Students");

            migrationBuilder.CreateTable(
                name: "StudentUser",
                columns: table => new
                {
                    ChildsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LegalGuardiansId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentUser", x => new { x.ChildsId, x.LegalGuardiansId });
                    table.ForeignKey(
                        name: "FK_StudentUser_Students_ChildsId",
                        column: x => x.ChildsId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentUser_Users_LegalGuardiansId",
                        column: x => x.LegalGuardiansId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StudentUser_LegalGuardiansId",
                table: "StudentUser",
                column: "LegalGuardiansId");
        }
    }
}
