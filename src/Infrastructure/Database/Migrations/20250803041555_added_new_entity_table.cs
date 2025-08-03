using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added_new_entity_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "todo_items",
                schema: "public");

            migrationBuilder.RenameColumn(
                name: "address_postcode",
                schema: "public",
                table: "AspNetUsers",
                newName: "address_postal_code");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                schema: "public",
                table: "AspNetUsers",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                schema: "public",
                table: "AspNetUsers",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "alternative_contact",
                schema: "public",
                table: "AspNetUsers",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "service_entity_id",
                schema: "public",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "open_iddict_applications",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    application_type = table.Column<string>(type: "text", nullable: true),
                    client_id = table.Column<string>(type: "text", nullable: true),
                    client_secret = table.Column<string>(type: "text", nullable: true),
                    client_type = table.Column<string>(type: "text", nullable: true),
                    concurrency_token = table.Column<string>(type: "text", nullable: true),
                    consent_type = table.Column<string>(type: "text", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    json_web_key_set = table.Column<string>(type: "text", nullable: true),
                    permissions = table.Column<string>(type: "text", nullable: true),
                    post_logout_redirect_uris = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redirect_uris = table.Column<string>(type: "text", nullable: true),
                    requirements = table.Column<string>(type: "text", nullable: true),
                    settings = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table => table.PrimaryKey("pk_open_iddict_applications", x => x.id));


            migrationBuilder.CreateTable(
                name: "open_iddict_scopes",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    concurrency_token = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    descriptions = table.Column<string>(type: "text", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    resources = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table => table.PrimaryKey("pk_open_iddict_scopes", x => x.id));

            migrationBuilder.CreateTable(
                name: "service_entities",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    contact_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    website_url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    address_street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    address_suburb = table.Column<string>(type: "text", nullable: true),
                    address_state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    address_country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    time_zone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    last_modified_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table => table.PrimaryKey("pk_service_entities", x => x.id));

            migrationBuilder.CreateTable(
                name: "open_iddict_authorizations",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    application_id = table.Column<string>(type: "text", nullable: true),
                    concurrency_token = table.Column<string>(type: "text", nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    scopes = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    subject = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_open_iddict_authorizations", x => x.id);
                    table.ForeignKey(
                        name: "fk_open_iddict_authorizations_open_iddict_applications_applica",
                        column: x => x.application_id,
                        principalSchema: "public",
                        principalTable: "open_iddict_applications",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "open_iddict_tokens",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    application_id = table.Column<string>(type: "text", nullable: true),
                    authorization_id = table.Column<string>(type: "text", nullable: true),
                    concurrency_token = table.Column<string>(type: "text", nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    payload = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redemption_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reference_id = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    subject = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_open_iddict_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_open_iddict_tokens_open_iddict_applications_application_id",
                        column: x => x.application_id,
                        principalSchema: "public",
                        principalTable: "open_iddict_applications",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_open_iddict_tokens_open_iddict_authorizations_authorization",
                        column: x => x.authorization_id,
                        principalSchema: "public",
                        principalTable: "open_iddict_authorizations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_service_entity_id",
                schema: "public",
                table: "AspNetUsers",
                column: "service_entity_id");

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_authorizations_application_id",
                schema: "public",
                table: "open_iddict_authorizations",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_tokens_application_id",
                schema: "public",
                table: "open_iddict_tokens",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_tokens_authorization_id",
                schema: "public",
                table: "open_iddict_tokens",
                column: "authorization_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_service_entities_service_entity_id",
                schema: "public",
                table: "AspNetUsers",
                column: "service_entity_id",
                principalSchema: "public",
                principalTable: "service_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_service_entities_service_entity_id",
                schema: "public",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "open_iddict_scopes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "open_iddict_tokens",
                schema: "public");

            migrationBuilder.DropTable(
                name: "service_entities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "open_iddict_authorizations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "open_iddict_applications",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_service_entity_id",
                schema: "public",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "service_entity_id",
                schema: "public",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "address_postal_code",
                schema: "public",
                table: "AspNetUsers",
                newName: "address_postcode");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                schema: "public",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                schema: "public",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "alternative_contact",
                schema: "public",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.CreateTable(
                name: "todo_items",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    labels = table.Column<List<string>>(type: "text[]", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table => table.PrimaryKey("pk_todo_items", x => x.id));
        }
    }
}
