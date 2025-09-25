using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Core.Infraestructure.Repositories.Sql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Automoviles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Fabricacion = table.Column<int>(type: "int", nullable: false),
                    NumeroMotor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumeroChasis = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Automoviles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Automoviles",
                columns: new[] { "Id", "Color", "Fabricacion", "FechaActualizacion", "FechaCreacion", "Marca", "Modelo", "NumeroChasis", "NumeroMotor" },
                values: new object[,]
                {
                    { 1, "Blanco", 2022, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Toyota", "Corolla", "1HGCM82633A123456", "TOY2022001" },
                    { 2, "Rojo", 2023, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ford", "Mustang", "1FA6P8CF5H5123457", "FOR2023001" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Automovil_NumeroChasis",
                table: "Automoviles",
                column: "NumeroChasis",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Automovil_NumeroMotor",
                table: "Automoviles",
                column: "NumeroMotor",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Automoviles");
        }
    }
}
