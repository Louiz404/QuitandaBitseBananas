using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuitandaBitseBananas.Migrations
{
    /// <inheritdoc />
    public partial class AddValorTotalToProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "Produto",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "Produto");
        }
    }
}
