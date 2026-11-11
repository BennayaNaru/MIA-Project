using OpenSch.Application.Services;
using NUnit.Framework;
using Shouldly;

namespace OpenSch.Application.UnitTests.Services;

public class SchoolCatalogTests
{
    [Test]
    public void CatalogContainsFiftyDemoSchoolsWithUniqueNpsn()
    {
        SchoolCatalog.All.Count.ShouldBe(50);
        SchoolCatalog.All.Select(school => school.Npsn).Distinct().Count().ShouldBe(50);
    }

    [Test]
    public void CatalogCompositeScoreIsTheRoundedAverage()
    {
        foreach (var school in SchoolCatalog.All)
        {
            school.CompositeScore.ShouldBe(decimal.Round((school.LiteracyScore + school.NumeracyScore) / 2, 1));
        }
    }

    [Test]
    public void TryNormalizeIdsTrimsAndDeDuplicatesIds()
    {
        var normalized = SchoolCatalog.TryNormalizeIds([" DKI-01 ", "DKI-01", "DKI-02"], out var ids, out var error);

        normalized.ShouldBeTrue();
        error.ShouldBeNull();
        ids.ShouldBe(["DKI-01", "DKI-02"]);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(4)]
    public void TryNormalizeIdsRejectsWrongNumberOfSchools(int count)
    {
        var requested = Enumerable.Range(1, count).Select(index => $"DKI-{index:00}").ToArray();

        var normalized = SchoolCatalog.TryNormalizeIds(requested, out var ids, out var error);

        normalized.ShouldBeFalse();
        ids.ShouldBeEmpty();
        error.ShouldBe("Pilih tepat 2 atau 3 sekolah.");
    }
}
