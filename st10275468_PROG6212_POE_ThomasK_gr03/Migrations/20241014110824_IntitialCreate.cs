using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace st10275468_PROG6212_POE_ThomasK_gr03.Migrations
{
    /// <inheritdoc />
    public partial class IntitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userID);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    claimID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    claimAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    claimMonth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    submissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    claimStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.claimID);
                    table.ForeignKey(
                        name: "FK_Claims_Users_userID",
                        column: x => x.userID,
                        principalTable: "Users",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    documentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    claimID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.documentID);
                    table.ForeignKey(
                        name: "FK_Documents_Claims_claimID",
                        column: x => x.claimID,
                        principalTable: "Claims",
                        principalColumn: "claimID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_userID",
                table: "Claims",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_claimID",
                table: "Documents",
                column: "claimID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
