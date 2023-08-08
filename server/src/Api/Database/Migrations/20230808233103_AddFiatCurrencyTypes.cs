using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFiatCurrencyTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceCurrency",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "FiatCurrencyTypeId",
                table: "Users",
                type: "integer",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "FiatCurrencyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiatCurrencyTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FiatCurrencyTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "USD" },
                    { 2, "EUR" },
                    { 3, "JPY" },
                    { 4, "GBP" },
                    { 5, "CNY" },
                    { 6, "AUD" },
                    { 7, "CAD" },
                    { 8, "CHF" },
                    { 9, "HKD" },
                    { 10, "SGD" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_FiatCurrencyTypeId",
                table: "Users",
                column: "FiatCurrencyTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_FiatCurrencyTypes_FiatCurrencyTypeId",
                table: "Users",
                column: "FiatCurrencyTypeId",
                principalTable: "FiatCurrencyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_FiatCurrencyTypes_FiatCurrencyTypeId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "FiatCurrencyTypes");

            migrationBuilder.DropIndex(
                name: "IX_Users_FiatCurrencyTypeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FiatCurrencyTypeId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PriceCurrency",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
