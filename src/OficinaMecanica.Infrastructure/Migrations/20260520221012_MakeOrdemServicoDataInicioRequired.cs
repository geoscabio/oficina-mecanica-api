using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OficinaMecanica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeOrdemServicoDataInicioRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [GestaoOrdemServico].[OrdensServico]
                SET [DataInicio] = SYSUTCDATETIME()
                WHERE [DataInicio] IS NULL
                """);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInicio",
                schema: "GestaoOrdemServico",
                table: "OrdensServico",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInicio",
                schema: "GestaoOrdemServico",
                table: "OrdensServico",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
