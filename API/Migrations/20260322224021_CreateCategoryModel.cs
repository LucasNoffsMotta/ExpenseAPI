using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UnitTests_ExpenseAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateCategoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Expenses");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Expenses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    HexadecimalColor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "ID", "Description", "HexadecimalColor" },
                values: new object[,]
                {
                    { 1, "Ifood", "#FF0000" },
                    { 2, "AppleStore", "#eb4034" }
                });

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CategoryId",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CategoryId",
                table: "Expenses",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Category_CategoryId",
                table: "Expenses",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Category_CategoryId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_CategoryId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Expenses");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Expenses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: 1,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: 2,
                column: "Description",
                value: "Hamburguer");
        }
    }
}
