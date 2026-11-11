using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenSch.Application.Services;
using OpenSch.Domain.Entities;
using OpenSch.Infrastructure.Data;
using NUnit.Framework;
using Shouldly;

namespace OpenSch.Application.UnitTests.Services;

public class SchoolQueryServiceTests
{
    private SqliteConnection _connection = null!;
    private ApplicationDbContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Test]
    public async Task GetAllAsyncReadsSchoolsFromTheDatabaseInsteadOfTheStaticCatalog()
    {
        _context.Sekolah.Add(School("DKI-99", "Sekolah Basis Data", 90m, 90m));
        await _context.SaveChangesAsync();

        var schools = await new SchoolQueryService(_context).GetAllAsync();

        schools.Count.ShouldBe(1);
        schools[0].Name.ShouldBe("Sekolah Basis Data");
    }

    [Test]
    public async Task GetAllAsyncRanksSchoolsByCompositeThenLiteracy()
    {
        _context.Sekolah.AddRange(
            School("DKI-01", "Alpha", 80m, 90m),
            School("DKI-02", "Beta", 70m, 70m),
            School("DKI-03", "Gamma", 90m, 80m));
        await _context.SaveChangesAsync();

        var schools = await new SchoolQueryService(_context).GetAllAsync();

        schools.Select(school => school.Id).ShouldBe(["DKI-03", "DKI-01", "DKI-02"]);
        schools.Select(school => school.Rank).ShouldBe([1, 2, 3]);
    }

    [Test]
    public async Task ResolveForComparisonAsyncPreservesRequestedOrder()
    {
        _context.Sekolah.AddRange(
            School("DKI-01", "Alpha", 80m, 80m),
            School("DKI-02", "Beta", 90m, 90m));
        await _context.SaveChangesAsync();

        var selection = await new SchoolQueryService(_context)
            .ResolveForComparisonAsync(["DKI-02", "DKI-01"]);

        selection.Succeeded.ShouldBeTrue();
        selection.Error.ShouldBeNull();
        selection.Schools.Select(school => school.Id).ShouldBe(["DKI-02", "DKI-01"]);
    }

    [Test]
    public async Task ResolveForComparisonAsyncTrimsAndDeDuplicatesIds()
    {
        _context.Sekolah.AddRange(
            School("DKI-01", "Alpha", 80m, 80m),
            School("DKI-02", "Beta", 90m, 90m));
        await _context.SaveChangesAsync();

        var selection = await new SchoolQueryService(_context)
            .ResolveForComparisonAsync([" DKI-01 ", "DKI-01", "DKI-02"]);

        selection.Succeeded.ShouldBeTrue();
        selection.Schools.Select(school => school.Id).ShouldBe(["DKI-01", "DKI-02"]);
    }

    [Test]
    public async Task ResolveForComparisonAsyncRejectsIdsMissingFromTheDatabase()
    {
        _context.Sekolah.AddRange(
            School("DKI-01", "Alpha", 80m, 80m),
            School("DKI-02", "Beta", 90m, 90m));
        await _context.SaveChangesAsync();

        var selection = await new SchoolQueryService(_context)
            .ResolveForComparisonAsync(["DKI-01", "DKI-404"]);

        selection.Succeeded.ShouldBeFalse();
        selection.Schools.ShouldBeEmpty();
        selection.Error.ShouldBe("ID sekolah tidak ditemukan: DKI-404.");
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(4)]
    public async Task ResolveForComparisonAsyncRejectsWrongNumberOfSchools(int count)
    {
        var ids = Enumerable.Range(1, count).Select(index => $"DKI-{index:00}").ToArray();

        var selection = await new SchoolQueryService(_context).ResolveForComparisonAsync(ids);

        selection.Succeeded.ShouldBeFalse();
        selection.Schools.ShouldBeEmpty();
        selection.Error.ShouldBe("Pilih tepat 2 atau 3 sekolah.");
    }

    private static Sekolah School(string id, string name, decimal literacy, decimal numeracy) => new()
    {
        Id = id,
        Nama = name,
        Kota = "Jakarta",
        KabupatenKota = "Jakarta Selatan",
        Level = "SMP",
        SkorMelekHuruf = literacy,
        SkorMelekAngka = numeracy,
        SkorKomposit = decimal.Round((literacy + numeracy) / 2m, 1),
        Peringkat = 0,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };
}
