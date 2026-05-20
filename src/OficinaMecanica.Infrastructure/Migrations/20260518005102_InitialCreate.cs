using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OficinaMecanica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Atendimento");

            migrationBuilder.EnsureSchema(
                name: "GestaoEstoque");

            migrationBuilder.EnsureSchema(
                name: "Administrativo");

            migrationBuilder.EnsureSchema(
                name: "GestaoOrdemServico");

            migrationBuilder.CreateTable(
                name: "Clientes",
                schema: "Atendimento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    EnderecoLogradouro = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    EnderecoNumero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EnderecoBairro = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    EnderecoCidade = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    EnderecoCEP = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estoques",
                schema: "GestaoEstoque",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estoques", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mecanicos",
                schema: "Administrativo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Funcional = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mecanicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PecasInsumosCatalogo",
                schema: "Administrativo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PecasInsumosCatalogo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServicosCatalogo",
                schema: "Administrativo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicosCatalogo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                schema: "Atendimento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veiculos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "Atendimento",
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensEstoque",
                schema: "GestaoEstoque",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PecaInsumoCatalogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantidadeDisponivel = table.Column<int>(type: "int", nullable: false),
                    QuantidadeReservada = table.Column<int>(type: "int", nullable: false),
                    EstoqueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensEstoque_Estoques_EstoqueId",
                        column: x => x.EstoqueId,
                        principalSchema: "GestaoEstoque",
                        principalTable: "Estoques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensEstoque_PecasInsumosCatalogo_PecaInsumoCatalogoId",
                        column: x => x.PecaInsumoCatalogoId,
                        principalSchema: "Administrativo",
                        principalTable: "PecasInsumosCatalogo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdensServico",
                schema: "GestaoOrdemServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoCancelamento = table.Column<int>(type: "int", nullable: true),
                    VeiculoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MecanicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Mecanicos_MecanicoId",
                        column: x => x.MecanicoId,
                        principalSchema: "Administrativo",
                        principalTable: "Mecanicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalSchema: "Atendimento",
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PecasInsumosOrdemServico",
                schema: "GestaoOrdemServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PecaInsumoCatalogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PecasInsumosOrdemServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PecasInsumosOrdemServico_OrdensServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalSchema: "GestaoOrdemServico",
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PecasInsumosOrdemServico_PecasInsumosCatalogo_PecaInsumoCatalogoId",
                        column: x => x.PecaInsumoCatalogoId,
                        principalSchema: "Administrativo",
                        principalTable: "PecasInsumosCatalogo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServicosOrdemServico",
                schema: "GestaoOrdemServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ServicoCatalogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicosOrdemServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicosOrdemServico_OrdensServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalSchema: "GestaoOrdemServico",
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServicosOrdemServico_ServicosCatalogo_ServicoCatalogoId",
                        column: x => x.ServicoCatalogoId,
                        principalSchema: "Administrativo",
                        principalTable: "ServicosCatalogo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Documento",
                schema: "Atendimento",
                table: "Clientes",
                column: "Documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensEstoque_EstoqueId",
                schema: "GestaoEstoque",
                table: "ItensEstoque",
                column: "EstoqueId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensEstoque_PecaInsumoCatalogoId",
                schema: "GestaoEstoque",
                table: "ItensEstoque",
                column: "PecaInsumoCatalogoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_MecanicoId",
                schema: "GestaoOrdemServico",
                table: "OrdensServico",
                column: "MecanicoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_Numero",
                schema: "GestaoOrdemServico",
                table: "OrdensServico",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_VeiculoId",
                schema: "GestaoOrdemServico",
                table: "OrdensServico",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_PecasInsumosOrdemServico_OrdemServicoId",
                schema: "GestaoOrdemServico",
                table: "PecasInsumosOrdemServico",
                column: "OrdemServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_PecasInsumosOrdemServico_PecaInsumoCatalogoId",
                schema: "GestaoOrdemServico",
                table: "PecasInsumosOrdemServico",
                column: "PecaInsumoCatalogoId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicosOrdemServico_OrdemServicoId",
                schema: "GestaoOrdemServico",
                table: "ServicosOrdemServico",
                column: "OrdemServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicosOrdemServico_ServicoCatalogoId",
                schema: "GestaoOrdemServico",
                table: "ServicosOrdemServico",
                column: "ServicoCatalogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_ClienteId",
                schema: "Atendimento",
                table: "Veiculos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_Placa",
                schema: "Atendimento",
                table: "Veiculos",
                column: "Placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensEstoque",
                schema: "GestaoEstoque");

            migrationBuilder.DropTable(
                name: "PecasInsumosOrdemServico",
                schema: "GestaoOrdemServico");

            migrationBuilder.DropTable(
                name: "ServicosOrdemServico",
                schema: "GestaoOrdemServico");

            migrationBuilder.DropTable(
                name: "Estoques",
                schema: "GestaoEstoque");

            migrationBuilder.DropTable(
                name: "PecasInsumosCatalogo",
                schema: "Administrativo");

            migrationBuilder.DropTable(
                name: "OrdensServico",
                schema: "GestaoOrdemServico");

            migrationBuilder.DropTable(
                name: "ServicosCatalogo",
                schema: "Administrativo");

            migrationBuilder.DropTable(
                name: "Mecanicos",
                schema: "Administrativo");

            migrationBuilder.DropTable(
                name: "Veiculos",
                schema: "Atendimento");

            migrationBuilder.DropTable(
                name: "Clientes",
                schema: "Atendimento");
        }
    }
}
