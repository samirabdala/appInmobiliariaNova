using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inmobiliaria_api.Migrations
{
    /// <inheritdoc />
    public partial class AddContratoPropietarioRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Inmueble_InmuId",
                table: "Contratos");

            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Inquilinos_InquiId",
                table: "Contratos");

            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Propietario_PropId",
                table: "Contratos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inquilinos",
                table: "Inquilinos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contratos",
                table: "Contratos");

            migrationBuilder.RenameTable(
                name: "Inquilinos",
                newName: "Inquilino");

            migrationBuilder.RenameTable(
                name: "Contratos",
                newName: "Contrato");

            migrationBuilder.RenameIndex(
                name: "IX_Contratos_PropId",
                table: "Contrato",
                newName: "IX_Contrato_PropId");

            migrationBuilder.RenameIndex(
                name: "IX_Contratos_InquiId",
                table: "Contrato",
                newName: "IX_Contrato_InquiId");

            migrationBuilder.RenameIndex(
                name: "IX_Contratos_InmuId",
                table: "Contrato",
                newName: "IX_Contrato_InmuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inquilino",
                table: "Inquilino",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contrato",
                table: "Contrato",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contrato_Inmueble_InmuId",
                table: "Contrato",
                column: "InmuId",
                principalTable: "Inmueble",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contrato_Inquilino_InquiId",
                table: "Contrato",
                column: "InquiId",
                principalTable: "Inquilino",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contrato_Propietario_PropId",
                table: "Contrato",
                column: "PropId",
                principalTable: "Propietario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contrato_Inmueble_InmuId",
                table: "Contrato");

            migrationBuilder.DropForeignKey(
                name: "FK_Contrato_Inquilino_InquiId",
                table: "Contrato");

            migrationBuilder.DropForeignKey(
                name: "FK_Contrato_Propietario_PropId",
                table: "Contrato");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inquilino",
                table: "Inquilino");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contrato",
                table: "Contrato");

            migrationBuilder.RenameTable(
                name: "Inquilino",
                newName: "Inquilinos");

            migrationBuilder.RenameTable(
                name: "Contrato",
                newName: "Contratos");

            migrationBuilder.RenameIndex(
                name: "IX_Contrato_PropId",
                table: "Contratos",
                newName: "IX_Contratos_PropId");

            migrationBuilder.RenameIndex(
                name: "IX_Contrato_InquiId",
                table: "Contratos",
                newName: "IX_Contratos_InquiId");

            migrationBuilder.RenameIndex(
                name: "IX_Contrato_InmuId",
                table: "Contratos",
                newName: "IX_Contratos_InmuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inquilinos",
                table: "Inquilinos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contratos",
                table: "Contratos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Inmueble_InmuId",
                table: "Contratos",
                column: "InmuId",
                principalTable: "Inmueble",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Inquilinos_InquiId",
                table: "Contratos",
                column: "InquiId",
                principalTable: "Inquilinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Propietario_PropId",
                table: "Contratos",
                column: "PropId",
                principalTable: "Propietario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
