using OpenSch.Application.Services;
using NUnit.Framework;
using Shouldly;

namespace OpenSch.Application.UnitTests.Services;

public class SchoolCatalogTests
{
    [Test]
    public void GetAllAssignsSequentialRanksInCompositeThenLiteracyOrder()
    {
        var schools = SchoolCatalog.GetAll();

        schools.Select(school => school.Rank).ShouldBe(Enumerable.Range(1, schools.Count));

        for (var index = 1; index < schools.Count; index++)
        {
            var previous = schools[index - 1];
            var current = schools[index];

            previous.CompositeScore.ShouldBeGreaterThanOrEqualTo(current.CompositeScore);
            if (previous.CompositeScore == current.CompositeScore)
            {
                previous.LiteracyScore.ShouldBeGreaterThanOrEqualTo(current.LiteracyScore);
            }
        }
    }

    [Test]
    public void GetAllRecomputesCompositeFromLiteracyAndNumeracy()
    {
        foreach (var school in SchoolCatalog.GetAll())
        {
            school.CompositeScore.ShouldBe(decimal.Round((school.LiteracyScore + school.NumeracyScore) / 2, 1));
        }
    }

    [Test]
    public void TryResolveReturnsSchoolsInRequestedOrder()
    {
        var resolved = SchoolCatalog.TryResolve(["DKI-02", "DKI-01"], out var schools, out var error);

        resolved.ShouldBeTrue();
        error.ShouldBeNull();
        schools.Select(school => school.Id).ShouldBe(["DKI-02", "DKI-01"]);
    }

    [Test]
    public void TryResolveDeDuplicatesAndTrimsIds()
    {
        var resolved = SchoolCatalog.TryResolve([" DKI-01 ", "DKI-01", "DKI-02"], out var schools, out _);

        resolved.ShouldBeTrue();
        schools.Count.ShouldBe(2);
        schools.Select(school => school.Id).ShouldBe(["DKI-01", "DKI-02"]);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(4)]
    public void TryResolveRejectsWrongNumberOfSchools(int count)
    {
        var ids = Enumerable.Range(1, count).Select(index => $"DKI-{index:00}").ToArray();

        var resolved = SchoolCatalog.TryResolve(ids, out var schools, out var error);

        resolved.ShouldBeFalse();
        schools.ShouldBeEmpty();
        error.ShouldBe("Pilih tepat 2 atau 3 sekolah.");
    }

    [Test]
    public void TryResolveRejectsUnknownIds()
    {
        var resolved = SchoolCatalog.TryResolve(["DKI-01", "DKI-999"], out var schools, out var error);

        resolved.ShouldBeFalse();
        schools.ShouldBeEmpty();
        error.ShouldBe("ID sekolah tidak ditemukan: DKI-999.");
    }
}
