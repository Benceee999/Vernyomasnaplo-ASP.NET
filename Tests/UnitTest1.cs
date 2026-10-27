using VernyomasNaplo.Data;
using VernyomasNaplo.Pages.Admin;
using VernyomasNaplo.Services;
using Xunit;

namespace Tests;

public class StatisticsServiceTests
{
    private readonly StatisticsService _service = new();

    [Fact]
    public void CalculateStats_HaromElemuListaval_HelyesAtlagotEsExtremumokatSzamol()
    {
        var summary = new StatsSummary();
        var testMeasurements = new List<Measurement>
        {
            new Measurement { Systolic = 120, Diastolic = 80, Pulse = 70 },
            new Measurement { Systolic = 140, Diastolic = 90, Pulse = 80 },
            new Measurement { Systolic = 100, Diastolic = 70, Pulse = 60 }
        };

        _service.CalculateStats(testMeasurements, summary);

        Assert.Equal(3, summary.Count);
        Assert.Equal(120.0, summary.AvgSys);
        Assert.Equal(100, summary.MinSys);
        Assert.Equal(140, summary.MaxSys);
    }

    [Theory]
    [InlineData(7, 30, true)]
    [InlineData(11, 59, true)]
    [InlineData(12, 0, false)]
    [InlineData(20, 15, false)]
    public void IsMorningMeasurement_HelyesenAzonositjaAReggeliEsEstiIdoszakot(int hour, int minute, bool expectedIsMorning)
    {
        var time = new DateTime(2026, 10, 27, hour, minute, 0);

        var result = _service.IsMorningMeasurement(time);

        Assert.Equal(expectedIsMorning, result);
    }

    [Fact]
    public void CalculateStats_UresListaEseten_NemDobHibatasNullakatAdVissza()
    {
        var summary = new StatsSummary();
        var emptyList = new List<Measurement>();

        _service.CalculateStats(emptyList, summary);

        Assert.Equal(0, summary.Count);
        Assert.Equal(0.0, summary.AvgSys);
        Assert.Equal(0.0, summary.AvgDia);
        Assert.Equal(0, summary.MaxSys);
    }
}