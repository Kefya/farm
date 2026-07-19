using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmGame.Api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Users
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Login = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalEarned = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true); migrationBuilder.CreateTable(
                name: "Farms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Farms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Farms_UserId",
                table: "Farms",
                column: "UserId",
                unique: true);

            // CropTypes
            migrationBuilder.CreateTable(
                name: "CropTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                              .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SeedPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    GrowSeconds = table.Column<int>(type: "integer", nullable: false),
                    SellPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropTypes", x => x.Id);
                }); migrationBuilder.CreateTable(
                name: "CropTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                              .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SeedPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    GrowSeconds = table.Column<int>(type: "integer", nullable: false),
                    SellPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropTypes", x => x.Id);
                });
            migrationBuilder.CreateTable(
                            name: "Fields",
                            columns: table => new
                            {
                                Id = table.Column<Guid>(type: "uuid", nullable: false),
                                FarmId = table.Column<Guid>(type: "uuid", nullable: false),
                                SlotIndex = table.Column<int>(type: "integer", nullable: false),
                                BoughtAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                                PriceBought = table.Column<decimal>(type: "numeric(18,2)", nullable: true)
                            },
                            constraints: table =>
                            {
                                table.PrimaryKey("PK_Fields", x => x.Id);
                                table.ForeignKey(
                                    name: "FK_Fields_Farms_FarmId",
                                    column: x => x.FarmId,
                                    principalTable: "Farms",
                                    principalColumn: "Id",
                                    onDelete: ReferentialAction.Cascade);
                            });

            migrationBuilder.CreateIndex(
                name: "IX_Fields_FarmId_SlotIndex",
                table: "Fields",
                columns: new[] { "FarmId", "SlotIndex" },
                unique: true);

            // InventorySeeds
            migrationBuilder.CreateTable(
                name: "InventorySeeds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CropTypeId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventorySeeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventorySeeds_CropTypes_CropTypeId",
                        column: x => x.CropTypeId,
                        principalTable: "CropTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventorySeeds_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventorySeeds_UserId_CropTypeId",
                table: "InventorySeeds",
                columns: new[] { "UserId", "CropTypeId" },
                unique: true); migrationBuilder.CreateTable(
                name: "InventoryCrops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CropTypeId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCrops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCrops_CropTypes_CropTypeId",
                        column: x => x.CropTypeId,
                        principalTable: "CropTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryCrops_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCrops_UserId_CropTypeId",
                table: "InventoryCrops",
                columns: new[] { "UserId", "CropTypeId" },
                unique: true);

            // PlantedCrops
            migrationBuilder.CreateTable(
                name: "PlantedCrops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    CropTypeId = table.Column<int>(type: "integer", nullable: false),
                    PlantedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReadyAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HarvestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantedCrops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantedCrops_CropTypes_CropTypeId",
                        column: x => x.CropTypeId,
                        principalTable: "CropTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlantedCrops_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantedCrops_FieldId",
                table: "PlantedCrops",
                column: "FieldId",
                unique: false); migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            // Seed CropTypes
            migrationBuilder.InsertData(
                table: "CropTypes",
                columns: new[] { "Id", "Name", "SeedPrice", "GrowSeconds", "SellPrice" },
                values: new object[,]
                {
                    { 1, "Морковь", 10m, 60, 15m },
                    { 2, "Картофель", 25m, 180, 40m },
                    { 3, "Клубника", 50m, 300, 90m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CropTypes",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3 });

            migrationBuilder.DropTable(name: "PlantedCrops");
            migrationBuilder.DropTable(name: "InventoryCrops");
            migrationBuilder.DropTable(name: "InventorySeeds");
            migrationBuilder.DropTable(name: "Transactions");
            migrationBuilder.DropTable(name: "Fields");
            migrationBuilder.DropTable(name: "CropTypes");
            migrationBuilder.DropTable(name: "Farms");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}