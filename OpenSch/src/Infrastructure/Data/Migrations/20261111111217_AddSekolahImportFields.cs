using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenSch.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSekolahImportFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "jumlah_guru",
                table: "sekolah",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "jumlah_murid",
                table: "sekolah",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "npsn",
                table: "sekolah",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ruang_kelas",
                table: "sekolah",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "sekolah",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "tanggal_data",
                table: "sekolah",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "jumlah_berhasil",
                table: "impor_data",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "jumlah_gagal",
                table: "impor_data",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "pengguna_id",
                table: "impor_data",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status_impor",
                table: "impor_data",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_sekolah_npsn",
                table: "sekolah",
                column: "npsn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_impor_data_pengguna_id",
                table: "impor_data",
                column: "pengguna_id");

            migrationBuilder.AddForeignKey(
                name: "FK_impor_data_AspNetUsers_pengguna_id",
                table: "impor_data",
                column: "pengguna_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_impor_data_AspNetUsers_pengguna_id",
                table: "impor_data");

            migrationBuilder.DropIndex(
                name: "IX_sekolah_npsn",
                table: "sekolah");

            migrationBuilder.DropIndex(
                name: "IX_impor_data_pengguna_id",
                table: "impor_data");

            migrationBuilder.DropColumn(
                name: "jumlah_guru",
                table: "sekolah");

            migrationBuilder.DropColumn(
                name: "jumlah_murid",
                table: "sekolah");

            migrationBuilder.DropColumn(
                name: "npsn",
                table: "sekolah");

            migrationBuilder.DropColumn(
                name: "ruang_kelas",
                table: "sekolah");

            migrationBuilder.DropColumn(
                name: "status",
                table: "sekolah");

            migrationBuilder.DropColumn(
                name: "tanggal_data",
                table: "sekolah");

            migrationBuilder.DropColumn(
                name: "jumlah_berhasil",
                table: "impor_data");

            migrationBuilder.DropColumn(
                name: "jumlah_gagal",
                table: "impor_data");

            migrationBuilder.DropColumn(
                name: "pengguna_id",
                table: "impor_data");

            migrationBuilder.DropColumn(
                name: "status_impor",
                table: "impor_data");
        }
    }
}
