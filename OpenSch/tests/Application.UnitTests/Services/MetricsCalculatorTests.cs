using OpenSch.Application.Services;
using NUnit.Framework;
using Shouldly;

namespace OpenSch.Application.UnitTests.Services;

public class MetricsCalculatorTests
{
    private static SchoolDto School(
        string id,
        string level = "SMP",
        string city = "Jakarta Selatan",
        decimal literacy = 80m,
        decimal numeracy = 80m) =>
        new(
            id,
            $"School {id}",
            city,
            "District",
            level,
            literacy,
            numeracy,
            decimal.Round((literacy + numeracy) / 2, 1),
            1);

    [Test]
    public void CalculateReportsLeadersAndRanges()
    {
        var schools = new[]
        {
            School("A", literacy: 95m, numeracy: 60m),
            School("B", literacy: 60m, numeracy: 95m),
            School("C", literacy: 85m, numeracy: 85m),
        };

        var metrics = MetricsCalculator.Calculate(schools);

        metrics.LiteracyLeaderId.ShouldBe("A");
        metrics.NumeracyLeaderId.ShouldBe("B");
        metrics.CompositeLeaderId.ShouldBe("C");
        metrics.LiteracyRange.ShouldBe(35m);
        metrics.NumeracyRange.ShouldBe(35m);
        metrics.IsFairComparisonGroup.ShouldBeTrue();
    }

    [Test]
    public void CalculateRoundsRangesToOneDecimalPlace()
    {
        var schools = new[]
        {
            School("A", literacy: 84.55m, numeracy: 80.05m),
            School("B", literacy: 80.05m, numeracy: 76.45m),
        };

        var metrics = MetricsCalculator.Calculate(schools);

        metrics.LiteracyRange.ShouldBe(4.5m);
        metrics.NumeracyRange.ShouldBe(3.6m);
    }

    [Test]
    public void CalculateFlagsDifferentLevelsAsUnfair()
    {
        var schools = new[]
        {
            School("A", level: "SD"),
            School("B", level: "SMP"),
        };

        MetricsCalculator.Calculate(schools).IsFairComparisonGroup.ShouldBeFalse();
    }

    [Test]
    public void CalculateFlagsDifferentCitiesAsUnfair()
    {
        var schools = new[]
        {
            School("A", city: "Jakarta Selatan"),
            School("B", city: "Jakarta Timur"),
        };

        MetricsCalculator.Calculate(schools).IsFairComparisonGroup.ShouldBeFalse();
    }

    [Test]
    public void CalculateRejectsAnEmptyList()
    {
        Should.Throw<ArgumentException>(() => MetricsCalculator.Calculate([]));
    }

    [Test]
    public void SafeScoreHelpersTreatNullAsZero()
    {
        MetricsCalculator.GetSafeLiteracyScore(null).ShouldBe(0m);
        MetricsCalculator.GetSafeNumeracyScore(null).ShouldBe(0m);
        MetricsCalculator.GetSafeCompositeScore(null, 82m).ShouldBe(41m);
        MetricsCalculator.GetSafeCompositeScore(80m, 81m).ShouldBe(80.5m);
    }
}
