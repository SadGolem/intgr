using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace integration.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "stg");

            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.CreateTable(
                name: "apro_snapshots",
                schema: "stg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Source = table.Column<string>(type: "text", nullable: false),
                    Entity = table.Column<string>(type: "text", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false),
                    Payload = table.Column<string>(type: "jsonb", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now()) + interval '10 days'"),
                    Processed = table.Column<bool>(type: "boolean", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Hash = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_apro_snapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignees",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bosses",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bosses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RootCompanies",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WasteSourceCategories",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WasteSourceCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_asupro = table.Column<int>(type: "integer", nullable: false),
                    ExtId = table.Column<string>(type: "text", nullable: true),
                    ConsumerName = table.Column<string>(type: "text", nullable: false),
                    Bik = table.Column<string>(type: "text", nullable: true),
                    MailAddress = table.Column<string>(type: "text", nullable: true),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    Inn = table.Column<string>(type: "text", nullable: true),
                    Kpp = table.Column<string>(type: "text", nullable: true),
                    Ogrn = table.Column<string>(type: "text", nullable: true),
                    RootCompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    BossId = table.Column<Guid>(type: "uuid", nullable: true),
                    PersonId = table.Column<string>(type: "text", nullable: true),
                    DocTypeName = table.Column<string>(type: "text", nullable: true),
                    TypeKa = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Bosses_BossId",
                        column: x => x.BossId,
                        principalSchema: "core",
                        principalTable: "Bosses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_RootCompanies_RootCompanyId",
                        column: x => x.RootCompanyId,
                        principalSchema: "core",
                        principalTable: "RootCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WasteSources",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Normative = table.Column<bool>(type: "boolean", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WasteSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WasteSources_WasteSourceCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "core",
                        principalTable: "WasteSourceCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    RootId = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractTypeName = table.Column<string>(type: "text", nullable: false),
                    AssigneeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Assignees_AssigneeId",
                        column: x => x.AssigneeId,
                        principalSchema: "core",
                        principalTable: "Assignees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "core",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "core",
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false),
                    Lon = table.Column<decimal>(type: "numeric(9,6)", nullable: false),
                    Lat = table.Column<decimal>(type: "numeric(9,6)", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    AuthorUpdate = table.Column<string>(type: "text", nullable: true),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "core",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locations_Clients_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "core",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locations_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "core",
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Emitters",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<string>(type: "text", nullable: true),
                    ContractNumber = table.Column<string>(type: "text", nullable: true),
                    LocationMtId = table.Column<string>(type: "text", nullable: true),
                    ExecutorName = table.Column<string>(type: "text", nullable: true),
                    IdContract = table.Column<int>(type: "integer", nullable: false),
                    ContractStatus = table.Column<string>(type: "text", nullable: true),
                    ParticipantId = table.Column<int>(type: "integer", nullable: false),
                    TypeConsumer = table.Column<string>(type: "text", nullable: true),
                    NameConsumer = table.Column<string>(type: "text", nullable: true),
                    IdConsumer = table.Column<string>(type: "text", nullable: true),
                    WasteSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emitters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emitters_WasteSources_WasteSourceId",
                        column: x => x.WasteSourceId,
                        principalSchema: "core",
                        principalTable: "WasteSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContractPositions",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: true),
                    ValueManual = table.Column<double>(type: "double precision", nullable: true),
                    EstimationValue = table.Column<double>(type: "double precision", nullable: true),
                    DateStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ContractId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmitterId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractPositions_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "core",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractPositions_Emitters_EmitterId",
                        column: x => x.EmitterId,
                        principalSchema: "core",
                        principalTable: "Emitters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractPositions_Locations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "core",
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractPositions_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "core",
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GrW = table.Column<string>(type: "text", nullable: false),
                    Dates = table.Column<string[]>(type: "text[]", nullable: false),
                    ExtId = table.Column<string>(type: "text", nullable: true),
                    IdContainerType = table.Column<int>(type: "integer", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmitterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Emitters_EmitterId",
                        column: x => x.EmitterId,
                        principalSchema: "core",
                        principalTable: "Emitters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_Locations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "core",
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entries",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BtNumber = table.Column<int>(type: "integer", nullable: false),
                    PlanDateRo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Volume = table.Column<float>(type: "real", nullable: true),
                    Number = table.Column<int>(type: "integer", nullable: true),
                    IdContainerType = table.Column<int>(type: "integer", nullable: true),
                    StatusString = table.Column<string>(type: "text", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgreementId = table.Column<int>(type: "integer", nullable: false),
                    CapacityId = table.Column<int>(type: "integer", nullable: false),
                    AuthorName = table.Column<string>(type: "text", nullable: true),
                    ContractPositionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entries_ContractPositions_ContractPositionId",
                        column: x => x.ContractPositionId,
                        principalSchema: "core",
                        principalTable: "ContractPositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Entries_Locations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "core",
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Containers",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: true),
                    TypeId = table.Column<int>(type: "integer", nullable: true),
                    CapacityId = table.Column<int>(type: "integer", nullable: true),
                    ScheduleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Containers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Containers_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalSchema: "core",
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_apro_snapshots_Entity_ExternalId",
                schema: "stg",
                table: "apro_snapshots",
                columns: new[] { "Entity", "ExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_apro_snapshots_ExpiresAt",
                schema: "stg",
                table: "apro_snapshots",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_apro_snapshots_Hash",
                schema: "stg",
                table: "apro_snapshots",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_BossId",
                schema: "core",
                table: "Clients",
                column: "BossId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_id_asupro",
                schema: "core",
                table: "Clients",
                column: "id_asupro",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_RootCompanyId",
                schema: "core",
                table: "Clients",
                column: "RootCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ScheduleId",
                schema: "core",
                table: "Containers",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPositions_ContractId",
                schema: "core",
                table: "ContractPositions",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPositions_EmitterId",
                schema: "core",
                table: "ContractPositions",
                column: "EmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPositions_ExternalId",
                schema: "core",
                table: "ContractPositions",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractPositions_LocationId",
                schema: "core",
                table: "ContractPositions",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPositions_StatusId",
                schema: "core",
                table: "ContractPositions",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_AssigneeId",
                schema: "core",
                table: "Contracts",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClientId",
                schema: "core",
                table: "Contracts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ExternalId",
                schema: "core",
                table: "Contracts",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_StatusId",
                schema: "core",
                table: "Contracts",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Emitters_WasteSourceId",
                schema: "core",
                table: "Emitters",
                column: "WasteSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_ContractPositionId",
                schema: "core",
                table: "Entries",
                column: "ContractPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_LocationId",
                schema: "core",
                table: "Entries",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ClientId",
                schema: "core",
                table: "Locations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ExternalId",
                schema: "core",
                table: "Locations",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ParticipantId",
                schema: "core",
                table: "Locations",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_StatusId",
                schema: "core",
                table: "Locations",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_EmitterId",
                schema: "core",
                table: "Schedules",
                column: "EmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_LocationId",
                schema: "core",
                table: "Schedules",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Statuses_ExternalId_EntityType",
                schema: "core",
                table: "Statuses",
                columns: new[] { "ExternalId", "EntityType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WasteSources_CategoryId",
                schema: "core",
                table: "WasteSources",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WasteSources_ExternalId",
                schema: "core",
                table: "WasteSources",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "apro_snapshots",
                schema: "stg");

            migrationBuilder.DropTable(
                name: "Containers",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Entries",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Schedules",
                schema: "core");

            migrationBuilder.DropTable(
                name: "ContractPositions",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Contracts",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Emitters",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Locations",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Assignees",
                schema: "core");

            migrationBuilder.DropTable(
                name: "WasteSources",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Clients",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Statuses",
                schema: "core");

            migrationBuilder.DropTable(
                name: "WasteSourceCategories",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Bosses",
                schema: "core");

            migrationBuilder.DropTable(
                name: "RootCompanies",
                schema: "core");
        }
    }
}
