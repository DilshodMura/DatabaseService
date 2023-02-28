using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternalDb.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InternalCountries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalCountries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InternalCities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalCities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternalCities_InternalCountries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "InternalCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InternalOffices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    InternalCountryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalOffices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternalOffices_InternalCities_CityId",
                        column: x => x.CityId,
                        principalTable: "InternalCities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InternalOffices_InternalCountries_InternalCountryId",
                        column: x => x.InternalCountryId,
                        principalTable: "InternalCountries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InternalCities_CountryId",
                table: "InternalCities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalOffices_CityId",
                table: "InternalOffices",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalOffices_InternalCountryId",
                table: "InternalOffices",
                column: "InternalCountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternalOffices");

            migrationBuilder.DropTable(
                name: "InternalCities");

            migrationBuilder.DropTable(
                name: "InternalCountries");
        }
    }
}
