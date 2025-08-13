using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace integration.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexForIdAsuPro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationEntity_ClientEntity_ClientId",
                table: "LocationEntity");

            migrationBuilder.DropIndex(
                name: "IX_LocationEntity_ClientId",
                table: "LocationEntity");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "LocationEntity");

            migrationBuilder.RenameColumn(
                name: "IdClient",
                table: "LocationEntity",
                newName: "ClientIdAsuPro");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ClientEntity_IdAsuPro",
                table: "ClientEntity",
                column: "IdAsuPro");

            migrationBuilder.CreateIndex(
                name: "IX_LocationEntity_ClientIdAsuPro",
                table: "LocationEntity",
                column: "ClientIdAsuPro");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEntity_IdAsuPro",
                table: "ClientEntity",
                column: "IdAsuPro",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationEntity_ClientEntity_ClientIdAsuPro",
                table: "LocationEntity",
                column: "ClientIdAsuPro",
                principalTable: "ClientEntity",
                principalColumn: "IdAsuPro",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationEntity_ClientEntity_ClientIdAsuPro",
                table: "LocationEntity");

            migrationBuilder.DropIndex(
                name: "IX_LocationEntity_ClientIdAsuPro",
                table: "LocationEntity");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ClientEntity_IdAsuPro",
                table: "ClientEntity");

            migrationBuilder.DropIndex(
                name: "IX_ClientEntity_IdAsuPro",
                table: "ClientEntity");

            migrationBuilder.RenameColumn(
                name: "ClientIdAsuPro",
                table: "LocationEntity",
                newName: "IdClient");

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "LocationEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LocationEntity_ClientId",
                table: "LocationEntity",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationEntity_ClientEntity_ClientId",
                table: "LocationEntity",
                column: "ClientId",
                principalTable: "ClientEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
