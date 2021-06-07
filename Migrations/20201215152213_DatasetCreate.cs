using Microsoft.EntityFrameworkCore.Migrations;

namespace Plan_io_T.Migrations
{
    public partial class DatasetCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArduinoData",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Moisture = table.Column<int>(nullable: false),
                    Humidity = table.Column<int>(nullable: false),
                    Temperature = table.Column<int>(nullable: false),
                    Illumination = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArduinoData", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArduinoData");
        }
    }
}
