using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShoppingCart.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cupom",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    CodigoCupom = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    PercentualDesconto = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cupom", x => x.ID);
                    table.CheckConstraint("CK_Cupom_PercentualDesconto_Valido", "[PercentualDesconto] > 0 AND [PercentualDesconto] <= 100");
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    DescricaoProduto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    QuantidadeEstoque = table.Column<int>(type: "int", nullable: false),
                    PrecoLiquido = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.ID);
                    table.CheckConstraint("CK_Produto_PrecoLiquido_NaoNegativo", "[PrecoLiquido] >= 0");
                    table.CheckConstraint("CK_Produto_QuantidadeEstoque_NaoNegativa", "[QuantidadeEstoque] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Carrinho",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CupomID = table.Column<int>(type: "int", nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Desconto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carrinho", x => x.ID);
                    table.CheckConstraint("CK_Carrinho_Desconto_NaoNegativo", "[Desconto] >= 0");
                    table.CheckConstraint("CK_Carrinho_Subtotal_NaoNegativo", "[Subtotal] >= 0");
                    table.CheckConstraint("CK_Carrinho_Total_Valido", "[Total] >= 0 AND [Total] <= [Subtotal]");
                    table.ForeignKey(
                        name: "FK_Carrinho_Cupom",
                        column: x => x.CupomID,
                        principalTable: "Cupom",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarrinhoItem",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProdutoID = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecoTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CarrinhoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarrinhoItem", x => x.ID);
                    table.CheckConstraint("CK_CarrinhoItem_PrecoTotal_NaoNegativo", "[PrecoTotal] >= 0");
                    table.CheckConstraint("CK_CarrinhoItem_PrecoUnitario_NaoNegativo", "[PrecoUnitario] >= 0");
                    table.CheckConstraint("CK_CarrinhoItem_Quantidade_Positiva", "[Quantidade] > 0");
                    table.ForeignKey(
                        name: "FK_CarrinhoItem_Carrinho",
                        column: x => x.CarrinhoID,
                        principalTable: "Carrinho",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarrinhoItem_Produto",
                        column: x => x.ProdutoID,
                        principalTable: "Produto",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Cupom",
                columns: new[] { "ID", "CodigoCupom", "PercentualDesconto" },
                values: new object[,]
                {
                    { 1, "10OFF", 10m },
                    { 2, "15OFF", 15m }
                });

            migrationBuilder.InsertData(
                table: "Produto",
                columns: new[] { "ID", "DescricaoProduto", "PrecoLiquido", "QuantidadeEstoque" },
                values: new object[,]
                {
                    { 1, "Notebook Dell Inspiron 15", 3499.90m, 8 },
                    { 2, "Mouse sem fio Logitech M170", 59.90m, 50 },
                    { 3, "Teclado Mecânico Redragon Kumara", 189.90m, 25 },
                    { 4, "Monitor LG 24' Full HD", 749.00m, 12 },
                    { 5, "Headset Gamer HyperX Cloud Stinger", 249.90m, 30 },
                    { 6, "Webcam Logitech C920", 399.00m, 15 },
                    { 7, "SSD Kingston 480GB", 219.90m, 40 },
                    { 8, "Cadeira Gamer ThunderX3", 1099.00m, 5 },
                    { 9, "Carregador USB-C 65W", 89.90m, 60 },
                    { 10, "Hub USB 4 Portas 3.0", 49.90m, 35 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carrinho_CupomID",
                table: "Carrinho",
                column: "CupomID");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoItem_ProdutoID",
                table: "CarrinhoItem",
                column: "ProdutoID");

            migrationBuilder.CreateIndex(
                name: "UX_CarrinhoItem_Carrinho_Produto",
                table: "CarrinhoItem",
                columns: new[] { "CarrinhoID", "ProdutoID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Cupom_CodigoCupom",
                table: "Cupom",
                column: "CodigoCupom",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarrinhoItem");

            migrationBuilder.DropTable(
                name: "Carrinho");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "Cupom");
        }
    }
}
