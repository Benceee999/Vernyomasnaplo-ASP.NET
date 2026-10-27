using VernyomasNaplo.Data;
using VernyomasNaplo.Pages.Admin;

namespace VernyomasNaplo.Services;

public interface IStatisticsService
{
    void CalculateStats(IEnumerable<Measurement> measurements, StatsSummary summary);
    bool IsMorningMeasurement(DateTime measuredAt);
}

public class StatisticsService : IStatisticsService
{
    public void CalculateStats(IEnumerable<Measurement> measurements, StatsSummary summary)
    {
        var list = measurements?.ToList() ?? new List<Measurement>();
        if (!list.Any())
        {
            summary.Count = 0;
            summary.AvgSys = summary.AvgDia = summary.AvgPulse = 0;
            summary.MinSys = summary.MaxSys = 0;
            summary.MinDia = summary.MaxDia = 0;
            summary.MinPulse = summary.MaxPulse = 0;
            return;
        }

        summary.Count = list.Count;
        summary.AvgSys = Math.Round(list.Average(m => m.Systolic), 1);
        summary.MinSys = list.Min(m => m.Systolic);
        summary.MaxSys = list.Max(m => m.Systolic);

        summary.AvgDia = Math.Round(list.Average(m => m.Diastolic), 1);
        summary.MinDia = list.Min(m => m.Diastolic);
        summary.MaxDia = list.Max(m => m.Diastolic);

        summary.AvgPulse = Math.Round(list.Average(m => m.Pulse), 1);
        summary.MinPulse = list.Min(m => m.Pulse);
        summary.MaxPulse = list.Max(m => m.Pulse);
    }

    public bool IsMorningMeasurement(DateTime measuredAt)
    {
        return measuredAt.TimeOfDay < TimeSpan.FromHours(12);
    }
}