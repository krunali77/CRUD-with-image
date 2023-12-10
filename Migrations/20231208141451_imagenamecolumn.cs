using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cars.Migrations
{
    /// <inheritdoc />
    public partial class imagenamecolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "CarModels",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "CarModels");
        }
    }
}
