using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

public partial class RemoveNewRefreshTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "User_RefreshTokens",
            schema: "public");

        migrationBuilder.DropSequence(
            name: "reservationseq",
            schema: "public");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateSequence(
            name: "reservationseq",
            schema: "public",
            incrementBy: 10);

        migrationBuilder.CreateTable(
            name: "User_RefreshTokens",
            schema: "public",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false),
                date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                date_expires_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                token = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_refresh_tokens", x => x.id);
                table.ForeignKey(
                    name: "fk_user_refresh_tokens_asp_net_users_id",
                    column: x => x.id,
                    principalSchema: "public",
                    principalTable: "AspNetUsers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });
    }
}
