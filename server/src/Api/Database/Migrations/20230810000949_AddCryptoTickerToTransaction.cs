using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCryptoTickerToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CryptoTicker",
                table: "Transactions",
                type: "text",
                nullable: false
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "CryptoTicker", table: "Transactions");
        }
    }
}
