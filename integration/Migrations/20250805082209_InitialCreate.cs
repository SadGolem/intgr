using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace integration.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdAsuPro = table.Column<int>(type: "integer", nullable: false),
                    Ext_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ConsumerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Bik = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MailAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Inn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Kpp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Ogrn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Root_company = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Boss = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Person_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Doc_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type_ka = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmitterEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WasteSource_Id = table.Column<int>(type: "integer", nullable: false),
                    WasteSource_Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    WasteSource_Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    WasteSource_Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WasteSource_Normative = table.Column<bool>(type: "boolean", nullable: false),
                    WasteSource_Ext_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Containers_IDs = table.Column<List<int>>(type: "integer[]", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    ContractNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Location_Mt_Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExecutorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IdContract = table.Column<int>(type: "integer", nullable: false),
                    ContractStatus = table.Column<string>(type: "text", nullable: false),
                    Participant_Id = table.Column<int>(type: "integer", nullable: false),
                    TypeConsumer = table.Column<string>(type: "text", nullable: false),
                    NameConsumer = table.Column<string>(type: "text", nullable: false),
                    IdConsumer = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmitterEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntryEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAsuPro = table.Column<int>(type: "integer", nullable: false),
                    PlanDateRO = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Author = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IdLocation = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Agreement = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Volume = table.Column<float>(type: "float", nullable: true),
                    Capacity = table.Column<float>(type: "float", nullable: true),
                    Count = table.Column<int>(type: "integer", nullable: true),
                    IdContainerType = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdAsuPro = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric(9,6)", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric(9,6)", nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ExtId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IdParticipant = table.Column<int>(type: "integer", nullable: true),
                    IdClient = table.Column<int>(type: "integer", nullable: true),
                    AuthorUpdate = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdAsuPro = table.Column<int>(type: "integer", nullable: false),
                    IdLocation = table.Column<int>(type: "integer", nullable: false),
                    Containers_IDs = table.Column<List<int>>(type: "integer[]", nullable: true),
                    Schedule = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Dates = table.Column<string[]>(type: "text[]", maxLength: 500, nullable: false),
                    IdEmitter = table.Column<int>(type: "integer", nullable: true),
                    idContainerType = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientEntities_ExpirationDate",
                table: "ClientEntity",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmitterEntities_ExpirationDate",
                table: "EmitterEntity",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_EntryEntities_ExpirationDate",
                table: "EntryEntity",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_LocationEntities_ExpirationDate",
                table: "LocationEntity",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntities_ExpirationDate",
                table: "ScheduleEntity",
                column: "ExpirationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientEntity");

            migrationBuilder.DropTable(
                name: "EmitterEntity");

            migrationBuilder.DropTable(
                name: "EntryEntity");

            migrationBuilder.DropTable(
                name: "LocationEntity");

            migrationBuilder.DropTable(
                name: "ScheduleEntity");
        }
    }
}
