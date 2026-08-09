using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Marketplace.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

        migrationBuilder.CreateTable(
            name: "boost_packages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                tier = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                name = table.Column<string>(type: "text", nullable: true),
                priority_level = table.Column<int>(type: "integer", nullable: false),
                duration_days = table.Column<int>(type: "integer", nullable: false),
                price_amount = table.Column<long>(type: "bigint", nullable: false),
                active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_boost_packages", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "categories",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                slug = table.Column<string>(type: "text", nullable: true),
                parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                label_vi = table.Column<string>(type: "text", nullable: true),
                label_en = table.Column<string>(type: "text", nullable: true),
                icon = table.Column<string>(type: "text", nullable: true),
                sort_order = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_categories", x => x.id);
                table.ForeignKey(
                    name: "fk_categories_categories_parent_id",
                    column: x => x.parent_id,
                    principalTable: "categories",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                email = table.Column<string>(type: "text", nullable: false),
                password_hash = table.Column<string>(type: "text", nullable: true),
                google_id = table.Column<string>(type: "text", nullable: true),
                role = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                display_name = table.Column<string>(type: "text", nullable: false),
                phone = table.Column<string>(type: "text", nullable: true),
                location_province = table.Column<string>(type: "text", nullable: true),
                description = table.Column<string>(type: "text", nullable: true),
                cover_image_url = table.Column<string>(type: "text", nullable: true),
                verified = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "listings",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                category_id = table.Column<Guid>(type: "uuid", nullable: false),
                subcategory_id = table.Column<Guid>(type: "uuid", nullable: true),
                title = table.Column<string>(type: "text", nullable: true),
                slug = table.Column<string>(type: "text", nullable: true),
                condition = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                price_amount = table.Column<long>(type: "bigint", nullable: true),
                price_contact = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true, defaultValue: "VND"),
                location_province = table.Column<string>(type: "text", nullable: true),
                description = table.Column<string>(type: "text", nullable: true),
                status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                view_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                search_vector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: true)
                    .Annotation("Npgsql:TsVectorConfig", "simple")
                    .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" }),
                published_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                expires_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                specs = table.Column<string>(type: "jsonb", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_listings", x => x.id);
                table.ForeignKey(
                    name: "fk_listings_categories_category_id",
                    column: x => x.category_id,
                    principalTable: "categories",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_listings_categories_subcategory_id",
                    column: x => x.subcategory_id,
                    principalTable: "categories",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_listings_users_seller_id",
                    column: x => x.seller_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "boosts",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                package_id = table.Column<Guid>(type: "uuid", nullable: false),
                priority_level = table.Column<int>(type: "integer", nullable: false),
                status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                payment_reference = table.Column<string>(type: "text", nullable: true),
                requested_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                activated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                activated_by_admin_id = table.Column<Guid>(type: "uuid", nullable: true),
                starts_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                expires_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_boosts", x => x.id);
                table.ForeignKey(
                    name: "fk_boosts_boost_packages_package_id",
                    column: x => x.package_id,
                    principalTable: "boost_packages",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_boosts_listings_listing_id",
                    column: x => x.listing_id,
                    principalTable: "listings",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_boosts_users_activated_by_admin_id",
                    column: x => x.activated_by_admin_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_boosts_users_seller_id",
                    column: x => x.seller_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "inquiries",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                buyer_name = table.Column<string>(type: "text", nullable: true),
                buyer_phone = table.Column<string>(type: "text", nullable: true),
                buyer_email = table.Column<string>(type: "text", nullable: true),
                message = table.Column<string>(type: "text", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_inquiries", x => x.id);
                table.ForeignKey(
                    name: "fk_inquiries_listings_listing_id",
                    column: x => x.listing_id,
                    principalTable: "listings",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_inquiries_users_seller_id",
                    column: x => x.seller_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "listing_photos",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                url = table.Column<string>(type: "text", nullable: true),
                sort_order = table.Column<int>(type: "integer", nullable: false),
                width = table.Column<int>(type: "integer", nullable: true),
                height = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_listing_photos", x => x.id);
                table.ForeignKey(
                    name: "fk_listing_photos_listings_listing_id",
                    column: x => x.listing_id,
                    principalTable: "listings",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "moderation_decisions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                admin_id = table.Column<Guid>(type: "uuid", nullable: false),
                decision = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                reason = table.Column<string>(type: "text", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_moderation_decisions", x => x.id);
                table.ForeignKey(
                    name: "fk_moderation_decisions_listings_listing_id",
                    column: x => x.listing_id,
                    principalTable: "listings",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_moderation_decisions_users_admin_id",
                    column: x => x.admin_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "reports",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                reason = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                details = table.Column<string>(type: "text", nullable: true),
                reporter_contact = table.Column<string>(type: "text", nullable: true),
                status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                resolved_by_admin_id = table.Column<Guid>(type: "uuid", nullable: true),
                resolved_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_reports", x => x.id);
                table.ForeignKey(
                    name: "fk_reports_listings_listing_id",
                    column: x => x.listing_id,
                    principalTable: "listings",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_reports_users_resolved_by_admin_id",
                    column: x => x.resolved_by_admin_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "ix_boosts_activated_by_admin_id",
            table: "boosts",
            column: "activated_by_admin_id");

        migrationBuilder.CreateIndex(
            name: "ix_boosts_expires_at",
            table: "boosts",
            column: "expires_at");

        migrationBuilder.CreateIndex(
            name: "ix_boosts_listing_id",
            table: "boosts",
            column: "listing_id");

        migrationBuilder.CreateIndex(
            name: "ix_boosts_package_id",
            table: "boosts",
            column: "package_id");

        migrationBuilder.CreateIndex(
            name: "ix_boosts_seller_id",
            table: "boosts",
            column: "seller_id");

        migrationBuilder.CreateIndex(
            name: "ix_boosts_status",
            table: "boosts",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "ix_categories_parent_id",
            table: "categories",
            column: "parent_id");

        migrationBuilder.CreateIndex(
            name: "ix_categories_slug",
            table: "categories",
            column: "slug",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_inquiries_listing_id",
            table: "inquiries",
            column: "listing_id");

        migrationBuilder.CreateIndex(
            name: "ix_inquiries_seller_id",
            table: "inquiries",
            column: "seller_id");

        migrationBuilder.CreateIndex(
            name: "ix_listing_photos_listing_id",
            table: "listing_photos",
            column: "listing_id");

        migrationBuilder.CreateIndex(
            name: "ix_listings_category_id",
            table: "listings",
            column: "category_id");

        migrationBuilder.CreateIndex(
            name: "ix_listings_location_province",
            table: "listings",
            column: "location_province");

        migrationBuilder.CreateIndex(
            name: "ix_listings_price_amount",
            table: "listings",
            column: "price_amount");

        migrationBuilder.CreateIndex(
            name: "ix_listings_published_at",
            table: "listings",
            column: "published_at");

        migrationBuilder.CreateIndex(
            name: "ix_listings_search_vector",
            table: "listings",
            column: "search_vector")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_listings_seller_id",
            table: "listings",
            column: "seller_id");

        migrationBuilder.CreateIndex(
            name: "ix_listings_slug",
            table: "listings",
            column: "slug",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_listings_status",
            table: "listings",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "ix_listings_subcategory_id",
            table: "listings",
            column: "subcategory_id");

        migrationBuilder.CreateIndex(
            name: "ix_listings_title",
            table: "listings",
            column: "title")
            .Annotation("Npgsql:IndexMethod", "gin")
            .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

        migrationBuilder.CreateIndex(
            name: "ix_moderation_decisions_admin_id",
            table: "moderation_decisions",
            column: "admin_id");

        migrationBuilder.CreateIndex(
            name: "ix_moderation_decisions_listing_id",
            table: "moderation_decisions",
            column: "listing_id");

        migrationBuilder.CreateIndex(
            name: "ix_reports_listing_id",
            table: "reports",
            column: "listing_id");

        migrationBuilder.CreateIndex(
            name: "ix_reports_resolved_by_admin_id",
            table: "reports",
            column: "resolved_by_admin_id");

        migrationBuilder.CreateIndex(
            name: "ix_reports_status",
            table: "reports",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            table: "users",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_google_id",
            table: "users",
            column: "google_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_role",
            table: "users",
            column: "role");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "boosts");

        migrationBuilder.DropTable(
            name: "inquiries");

        migrationBuilder.DropTable(
            name: "listing_photos");

        migrationBuilder.DropTable(
            name: "moderation_decisions");

        migrationBuilder.DropTable(
            name: "reports");

        migrationBuilder.DropTable(
            name: "boost_packages");

        migrationBuilder.DropTable(
            name: "listings");

        migrationBuilder.DropTable(
            name: "categories");

        migrationBuilder.DropTable(
            name: "users");
    }
}
