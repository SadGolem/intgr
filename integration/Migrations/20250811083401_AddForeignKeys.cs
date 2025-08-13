using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace integration.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmitterId",
                table: "ScheduleEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "ScheduleEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "LocationEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntity_EmitterId",
                table: "ScheduleEntity",
                column: "EmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntity_LocationId",
                table: "ScheduleEntity",
                column: "LocationId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntity_EmitterEntity_EmitterId",
                table: "ScheduleEntity",
                column: "EmitterId",
                principalTable: "EmitterEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntity_LocationEntity_LocationId",
                table: "ScheduleEntity",
                column: "LocationId",
                principalTable: "LocationEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationEntity_ClientEntity_ClientId",
                table: "LocationEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntity_EmitterEntity_EmitterId",
                table: "ScheduleEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntity_LocationEntity_LocationId",
                table: "ScheduleEntity");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntity_EmitterId",
                table: "ScheduleEntity");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntity_LocationId",
                table: "ScheduleEntity");

            migrationBuilder.DropIndex(
                name: "IX_LocationEntity_ClientId",
                table: "LocationEntity");

            migrationBuilder.DropColumn(
                name: "EmitterId",
                table: "ScheduleEntity");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "ScheduleEntity");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "LocationEntity");
        }
    }
}
