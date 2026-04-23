using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HamburguerDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDatasPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Pedidos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFinalizacao",
                table: "Pedidos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "DataFinalizacao",
                table: "Pedidos");
        }
    }
}
