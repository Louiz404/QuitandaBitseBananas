using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuitandaBitseBananas.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarImagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Produto",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Produto");
        }
    }
}
