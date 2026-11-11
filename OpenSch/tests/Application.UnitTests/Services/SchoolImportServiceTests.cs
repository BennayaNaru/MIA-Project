using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenSch.Application.Services;
using OpenSch.Infrastructure.Data;
using OpenSch.Infrastructure.Identity;
using NUnit.Framework;
using Shouldly;

namespace OpenSch.Application.UnitTests.Services;

public class SchoolImportServiceTests
{
    private const string Header = "npsn,nama,jenjang,status,wilayah,kecamatan,murid,guru,ruang_kelas,literasi,numerasi,tanggal_data";

    private SqliteConnection _connection = null!;
    private ApplicationDbContext _context = null!;

    [SetUp]
    public async Task SetUp()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        // ImporData keeps a foreign key to the Identity user that uploaded the file.
        _context.Users.Add(new ApplicationUser
        {
            Id = "user-1",
            UserName = "admin@test.local",
            NormalizedUserName = "ADMIN@TEST.LOCAL",
            Email = "admin@test.local",
            NormalizedEmail = "ADMIN@TEST.LOCAL",
            EmailConfirmed = true,
        });
        await _context.SaveChangesAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Test]
    public async Task ImportInsertsNewSchoolsAndWritesASuccessLog()
    {
        var report = await Import(
            """
            npsn,nama,jenjang,status,wilayah,kecamatan,murid,guru,ruang_kelas,literasi,numerasi,tanggal_data
            20100001,SDN Uji Satu,SD,Negeri,Jakarta Selatan,Tebet,300,20,12,85.5,80.0,2026-01-15
            20100002,SMP Uji Dua,SMP,Swasta,Jakarta Barat,Kebon Jeruk,400,25,15,75.0,82.5,2026-01-15
            """);

        report.Succeeded.ShouldBeTrue();
        report.Inserted.ShouldBe(2);
        report.Updated.ShouldBe(0);
        report.Errors.ShouldBeEmpty();

        _context.Sekolah.Count().ShouldBe(2);

        var school = _context.Sekolah.Single(school => school.Npsn == "20100001");
        school.Id.ShouldBe("20100001");
        school.Nama.ShouldBe("SDN Uji Satu");
        school.Kota.ShouldBe("Jakarta Selatan");
        school.KabupatenKota.ShouldBe("Tebet");
        school.SkorMelekHuruf.ShouldBe(85.5m);
        school.SkorMelekAngka.ShouldBe(80.0m);
        school.SkorKomposit.ShouldBe(82.8m);
        school.JumlahMurid.ShouldBe(300);

        var import = _context.ImporData.Single();
        import.StatusImpor.ShouldBe(SchoolImportService.StatusSucceeded);
        import.JumlahBerhasil.ShouldBe(2);
        import.JumlahGagal.ShouldBe(0);
        import.Sukses.ShouldBeTrue();
        import.NamaFile.ShouldBe("schools.csv");
    }

    [Test]
    public async Task ImportUpdatesAnExistingSchoolMatchedByNpsn()
    {
        await Import(
            """
            npsn,nama,jenjang,status,wilayah,kecamatan,murid,guru,ruang_kelas,literasi,numerasi,tanggal_data
            20100001,SDN Uji Satu,SD,Negeri,Jakarta Selatan,Tebet,300,20,12,85.5,80.0,2026-01-15
            """);

        var report = await Import(
            """
            npsn,nama,jenjang,status,wilayah,kecamatan,murid,guru,ruang_kelas,literasi,numerasi,tanggal_data
            20100001,SDN Uji Satu Revisi,SD,Negeri,Jakarta Selatan,Tebet,350,22,13,88.0,84.0,2026-02-01
            """);

        report.Succeeded.ShouldBeTrue();
        report.Inserted.ShouldBe(0);
        report.Updated.ShouldBe(1);

        _context.Sekolah.Count().ShouldBe(1);
        var school = _context.Sekolah.Single();
        school.Nama.ShouldBe("SDN Uji Satu Revisi");
        school.JumlahMurid.ShouldBe(350);
        school.SkorKomposit.ShouldBe(86.0m);
    }

    [Test]
    public async Task ImportRejectsTheWholeFileWhenAnyRowIsInvalid()
    {
        var report = await Import(
            """
            npsn,nama,jenjang,status,wilayah,kecamatan,murid,guru,ruang_kelas,literasi,numerasi,tanggal_data
            20100001,SDN Valid,SD,Negeri,Jakarta Selatan,Tebet,300,20,12,85.5,80.0,2026-01-15
            20100002,SMK Tidak Valid,SMK,Swasta,Jakarta Barat,Kebon Jeruk,400,25,15,150.0,82.5,2026-01-15
            """);

        report.Succeeded.ShouldBeFalse();
        report.Inserted.ShouldBe(0);
        report.Updated.ShouldBe(0);
        report.Errors.ShouldContain(error => error.Field == "literasi");

        _context.Sekolah.ShouldBeEmpty();

        var import = _context.ImporData.Single();
        import.StatusImpor.ShouldBe(SchoolImportService.StatusFailed);
        import.Sukses.ShouldBeFalse();
        import.JumlahBerhasil.ShouldBe(0);
        import.Pesan.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task ImportRejectsFilesMissingRequiredColumns()
    {
        var report = await Import(
            """
            npsn,nama,jenjang
            20100001,SDN Uji Satu,SD
            """);

        report.Succeeded.ShouldBeFalse();
        report.Errors.ShouldContain(error => error.Field == "header");
        _context.Sekolah.ShouldBeEmpty();
    }

    [Test]
    public async Task ImportRejectsDuplicateNpsnWithinTheSameFile()
    {
        var report = await Import(
            """
            npsn,nama,jenjang,status,wilayah,kecamatan,murid,guru,ruang_kelas,literasi,numerasi,tanggal_data
            20100001,SDN Pertama,SD,Negeri,Jakarta Selatan,Tebet,300,20,12,85.5,80.0,2026-01-15
            20100001,SDN Kedua,SD,Negeri,Jakarta Selatan,Tebet,310,21,12,84.0,81.0,2026-01-15
            """);

        report.Succeeded.ShouldBeFalse();
        report.Errors.ShouldContain(error => error.Field == "npsn" && error.Message.Contains("lebih dari sekali"));
        _context.Sekolah.ShouldBeEmpty();
    }

    [Test]
    public async Task ImportRejectsAnEmptyFile()
    {
        var report = await Import(string.Empty);

        report.Succeeded.ShouldBeFalse();
        report.Errors.ShouldNotBeEmpty();
        _context.Sekolah.ShouldBeEmpty();
    }

    private async Task<ImportReport> Import(string csv)
    {
        var service = new SchoolImportService(_context);
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        return await service.ImportAsync(stream, "schools.csv", "user-1");
    }
}
