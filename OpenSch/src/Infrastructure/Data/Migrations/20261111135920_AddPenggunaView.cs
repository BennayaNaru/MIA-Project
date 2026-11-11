using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenSch.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPenggunaView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Read-only view for ERD alignment. ASP.NET Identity keeps ownership of writes
            // to AspNetUsers; password hashes deliberately stay out of this view.
            migrationBuilder.Sql(
                """
                DROP VIEW IF EXISTS pengguna;

                CREATE VIEW pengguna AS
                SELECT
                    u."Id" AS id,
                    u."UserName" AS nama,
                    u."Email" AS email,
                    COALESCE(
                        (
                            SELECT MIN(r."Name")
                            FROM "AspNetUserRoles" ur
                            JOIN "AspNetRoles" r ON r."Id" = ur."RoleId"
                            WHERE ur."UserId" = u."Id"
                        ),
                        'Pengguna'
                    ) AS peran,
                    u."EmailConfirmed" AS email_terkonfirmasi
                FROM "AspNetUsers" u;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS pengguna;");
        }
    }
}
