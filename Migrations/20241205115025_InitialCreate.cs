using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotatryhard.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    match_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    match_seq_num = table.Column<long>(type: "bigint", nullable: true),
                    start_time = table.Column<long>(type: "bigint", nullable: true),
                    cluster = table.Column<int>(type: "int", nullable: true),
                    dire_score = table.Column<short>(type: "smallint", nullable: true),
                    radiant_score = table.Column<short>(type: "smallint", nullable: true),
                    duration = table.Column<short>(type: "smallint", nullable: true),
                    radiant_win = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.match_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    account_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    personaname = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    avatarfull = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    loccountrycode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.account_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayersMatches",
                columns: table => new
                {
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    last_hits = table.Column<short>(type: "smallint", nullable: true),
                    denies = table.Column<short>(type: "smallint", nullable: true),
                    assists = table.Column<short>(type: "smallint", nullable: true),
                    deaths = table.Column<short>(type: "smallint", nullable: true),
                    kills = table.Column<short>(type: "smallint", nullable: true),
                    hero_damage = table.Column<int>(type: "int", nullable: true),
                    hero_healing = table.Column<int>(type: "int", nullable: true),
                    net_worth = table.Column<int>(type: "int", nullable: true),
                    tower_damage = table.Column<int>(type: "int", nullable: true),
                    gold_per_min = table.Column<short>(type: "smallint", nullable: true),
                    xp_per_min = table.Column<short>(type: "smallint", nullable: true),
                    ability_0 = table.Column<int>(type: "int", nullable: true),
                    ability_1 = table.Column<int>(type: "int", nullable: true),
                    ability_2 = table.Column<int>(type: "int", nullable: true),
                    ability_3 = table.Column<int>(type: "int", nullable: true),
                    hero_level = table.Column<short>(type: "smallint", nullable: true),
                    team = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    leaver_status = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    aghanims_scepter = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    aghanims_shard = table.Column<short>(type: "smallint", nullable: true),
                    backpack_0 = table.Column<short>(type: "smallint", nullable: true),
                    backpack_1 = table.Column<short>(type: "smallint", nullable: true),
                    backpack_2 = table.Column<short>(type: "smallint", nullable: true),
                    item_0 = table.Column<short>(type: "smallint", nullable: true),
                    item_1 = table.Column<short>(type: "smallint", nullable: true),
                    item_2 = table.Column<short>(type: "smallint", nullable: true),
                    item_3 = table.Column<short>(type: "smallint", nullable: true),
                    item_4 = table.Column<short>(type: "smallint", nullable: true),
                    item_5 = table.Column<short>(type: "smallint", nullable: true),
                    item_neutral = table.Column<short>(type: "smallint", nullable: true),
                    moonshard = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    hero_id = table.Column<short>(type: "smallint", nullable: true),
                    player_slot = table.Column<short>(type: "smallint", nullable: true),
                    win = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    score = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayersMatches", x => new { x.account_id, x.match_id });
                    table.ForeignKey(
                        name: "FK_PlayersMatches_Matches_match_id",
                        column: x => x.match_id,
                        principalTable: "Matches",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayersMatches_Players_account_id",
                        column: x => x.account_id,
                        principalTable: "Players",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PlayersMatches_match_id",
                table: "PlayersMatches",
                column: "match_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayersMatches");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
