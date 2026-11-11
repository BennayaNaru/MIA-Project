using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OpenSch.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolComparisonSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "impor_data",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nama_file = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    tanggal_impor = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    jumlah_baris = table.Column<int>(type: "integer", nullable: false),
                    sukses = table.Column<bool>(type: "boolean", nullable: false),
                    pesan = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_impor_data", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sekolah",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nama = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    kota = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    kabupaten_kota = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    skor_melek_huruf = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    skor_melek_angka = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    skor_komposit = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    peringkat = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sekolah", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "perbandingan",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sekolah_sumber_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sekolah_tujuan_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    tanggal_perbandingan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    metode_perbandingan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    pemimpin_huruf_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    pemimpin_angka_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    pemimpin_komposit_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    rentang_huruf = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    rentang_angka = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    perbandingan_adil = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perbandingan", x => x.id);
                    table.ForeignKey(
                        name: "FK_perbandingan_sekolah_sekolah_sumber_id",
                        column: x => x.sekolah_sumber_id,
                        principalTable: "sekolah",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_perbandingan_sekolah_sekolah_tujuan_id",
                        column: x => x.sekolah_tujuan_id,
                        principalTable: "sekolah",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "detail_perbandingan",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    perbandingan_id = table.Column<long>(type: "bigint", nullable: false),
                    sekolah_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    metrik = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nilai_asli = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    nilai_normalisasi = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    rangking = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detail_perbandingan", x => x.id);
                    table.ForeignKey(
                        name: "FK_detail_perbandingan_perbandingan_perbandingan_id",
                        column: x => x.perbandingan_id,
                        principalTable: "perbandingan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detail_perbandingan_sekolah_sekolah_id",
                        column: x => x.sekolah_id,
                        principalTable: "sekolah",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_detail_perbandingan_perbandingan_id",
                table: "detail_perbandingan",
                column: "perbandingan_id");

            migrationBuilder.CreateIndex(
                name: "IX_detail_perbandingan_sekolah_id",
                table: "detail_perbandingan",
                column: "sekolah_id");

            migrationBuilder.CreateIndex(
                name: "IX_perbandingan_sekolah_sumber_id",
                table: "perbandingan",
                column: "sekolah_sumber_id");

            migrationBuilder.CreateIndex(
                name: "IX_perbandingan_sekolah_tujuan_id",
                table: "perbandingan",
                column: "sekolah_tujuan_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detail_perbandingan");

            migrationBuilder.DropTable(
                name: "impor_data");

            migrationBuilder.DropTable(
                name: "perbandingan");

            migrationBuilder.DropTable(
                name: "sekolah");
        }
    }
}
