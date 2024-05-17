using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsStockCommandToChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStockCommand",
                table: "ChatMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStockCommand",
                table: "ChatMessages");
        }
    }
}
