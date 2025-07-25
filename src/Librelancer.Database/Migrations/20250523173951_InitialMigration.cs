﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibreLancer.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountIdentifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    LastLogin = table.Column<double>(type: "REAL", nullable: false),
                    BanExpiry = table.Column<double>(type: "REAL", nullable: true),
                    CreationDate = table.Column<double>(type: "REAL", nullable: false),
                    UpdateDate = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT COLLATE NOCASE", nullable: true),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    Rank = table.Column<uint>(type: "INTEGER", nullable: false),
                    Money = table.Column<long>(type: "INTEGER", nullable: false),
                    Time = table.Column<double>(type: "REAL", nullable: false),
                    Voice = table.Column<string>(type: "TEXT", nullable: true),
                    Costume = table.Column<string>(type: "TEXT", nullable: true),
                    ComCostume = table.Column<string>(type: "TEXT", nullable: true),
                    System = table.Column<string>(type: "TEXT", nullable: true),
                    Base = table.Column<string>(type: "TEXT", nullable: true),
                    X = table.Column<float>(type: "REAL", nullable: false),
                    Y = table.Column<float>(type: "REAL", nullable: false),
                    Z = table.Column<float>(type: "REAL", nullable: false),
                    RotationX = table.Column<float>(type: "REAL", nullable: false),
                    RotationY = table.Column<float>(type: "REAL", nullable: false),
                    RotationZ = table.Column<float>(type: "REAL", nullable: false),
                    RotationW = table.Column<float>(type: "REAL", nullable: false),
                    Ship = table.Column<string>(type: "TEXT", nullable: true),
                    Affiliation = table.Column<string>(type: "TEXT", nullable: true),
                    AffiliationLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    FightersKilled = table.Column<long>(type: "INTEGER", nullable: false),
                    FreightersKilled = table.Column<long>(type: "INTEGER", nullable: false),
                    TransportsKilled = table.Column<long>(type: "INTEGER", nullable: false),
                    CapitalKills = table.Column<long>(type: "INTEGER", nullable: false),
                    PlayersKilled = table.Column<long>(type: "INTEGER", nullable: false),
                    MissionsCompleted = table.Column<long>(type: "INTEGER", nullable: false),
                    MissionsFailed = table.Column<long>(type: "INTEGER", nullable: false),
                    AccountId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<double>(type: "REAL", nullable: false),
                    UpdateDate = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CargoItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemName = table.Column<string>(type: "TEXT", nullable: true),
                    ItemCount = table.Column<long>(type: "INTEGER", nullable: false),
                    Hardpoint = table.Column<string>(type: "TEXT", nullable: true),
                    Health = table.Column<float>(type: "REAL", nullable: false),
                    IsMissionItem = table.Column<bool>(type: "INTEGER", nullable: false),
                    CharacterId = table.Column<long>(type: "INTEGER", nullable: true),
                    CreationDate = table.Column<double>(type: "REAL", nullable: false),
                    UpdateDate = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoItem_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reputation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterId = table.Column<long>(type: "INTEGER", nullable: false),
                    ReputationValue = table.Column<float>(type: "REAL", nullable: false),
                    RepGroup = table.Column<string>(type: "TEXT COLLATE NOCASE", nullable: true),
                    CreationDate = table.Column<double>(type: "REAL", nullable: false),
                    UpdateDate = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reputation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reputation_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitEntry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterId = table.Column<long>(type: "INTEGER", nullable: false),
                    VisitValue = table.Column<int>(type: "INTEGER", nullable: false),
                    Hash = table.Column<uint>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<double>(type: "REAL", nullable: false),
                    UpdateDate = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitEntry_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountIdentifier",
                table: "Accounts",
                column: "AccountIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_CargoItem_CharacterId",
                table: "CargoItem",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_AccountId",
                table: "Characters",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Reputation_CharacterId",
                table: "Reputation",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reputation_CharacterId_RepGroup",
                table: "Reputation",
                columns: new[] { "CharacterId", "RepGroup" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitEntry_CharacterId",
                table: "VisitEntry",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitEntry_CharacterId_Hash",
                table: "VisitEntry",
                columns: new[] { "CharacterId", "Hash" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CargoItem");

            migrationBuilder.DropTable(
                name: "Reputation");

            migrationBuilder.DropTable(
                name: "VisitEntry");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
