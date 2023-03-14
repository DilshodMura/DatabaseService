using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExternalDb.Migrations
{
    /// <inheritdoc />
    public partial class testExt2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalCountries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalCountries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalCities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalCities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalCities_ExternalCountries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "ExternalCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalOffices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalOffices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalOffices_ExternalCities_CityId",
                        column: x => x.CityId,
                        principalTable: "ExternalCities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalCities_CountryId",
                table: "ExternalCities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalOffices_CityId",
                table: "ExternalOffices",
                column: "CityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalOffices");

            migrationBuilder.DropTable(
                name: "ExternalCities");

            migrationBuilder.DropTable(
                name: "ExternalCountries");
        }
    }
}
