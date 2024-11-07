using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inmobiliaria_api.Migrations
{
    /// <inheritdoc />
    public partial class CreateInmobiliariaSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "password",
                table: "Propietario",
                newName: "Password");

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "Inmueble",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tipo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Inmueble_TipoId",
                table: "Inmueble",
                column: "TipoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inmueble_tipo_TipoId",
                table: "Inmueble",
                column: "TipoId",
                principalTable: "tipo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inmueble_tipo_TipoId",
                table: "Inmueble");

            migrationBuilder.DropTable(
                name: "tipo");

            migrationBuilder.DropIndex(
                name: "IX_Inmueble_TipoId",
                table: "Inmueble");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "Inmueble");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Propietario",
                newName: "password");
        }
    }
}
