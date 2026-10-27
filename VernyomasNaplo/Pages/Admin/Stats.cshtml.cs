using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using VernyomasNaplo.Data;
using VernyomasNaplo.Services;

namespace VernyomasNaplo.Pages.Admin
{
    public class StatsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IStatisticsService _statsService;

        public StatsModel(ApplicationDbContext context, IStatisticsService statsService)
        {
            _context = context;
            _statsService = statsService;
        }

        public IList<Patient> DropdownPatients { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int? SelectedPatientId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public List<Measurement> FilteredMeasurements { get; set; } = new();

        public StatsSummary Overall { get; set; } = new StatsSummary();
        public StatsSummary Morning { get; set; } = new StatsSummary();
        public StatsSummary Evening { get; set; } = new StatsSummary();

        public string PieChartLabelsJson { get; set; } = "[]";
        public string PieChartDataJson { get; set; } = "[]";

        public async Task OnGetAsync()
        {
            DropdownPatients = await _context.Patients
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();

            IQueryable<Measurement> baseQuery = _context.Measurements
                .AsNoTracking()
                .Include(m => m.Patient)
                .Include(m => m.Nurse);

            if (SelectedPatientId.HasValue)
            {
                baseQuery = baseQuery.Where(m => m.PatientId == SelectedPatientId.Value);
            }

            if (StartDate.HasValue)
            {
                baseQuery = baseQuery.Where(m => m.Date.Date >= StartDate.Value.Date);
            }
            if (EndDate.HasValue)
            {
                baseQuery = baseQuery.Where(m => m.Date.Date <= EndDate.Value.Date);
            }

            if (EndDate.HasValue && StartDate.HasValue && EndDate.Value.Date < StartDate.Value.Date)
            {
                (StartDate, EndDate) = (EndDate, StartDate);
            }

            FilteredMeasurements = await baseQuery
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            var morningMeasurements = FilteredMeasurements
                .Where(m => _statsService.IsMorningMeasurement(m.Date))
                .ToList();

            var eveningMeasurements = FilteredMeasurements
                .Where(m => !_statsService.IsMorningMeasurement(m.Date))
                .ToList();

            _statsService.CalculateStats(FilteredMeasurements, Overall);
            _statsService.CalculateStats(morningMeasurements, Morning);
            _statsService.CalculateStats(eveningMeasurements, Evening);

            var startOfWeek = DateTime.Today.AddDays(-6);
            var weeklyNurseGroups = await _context.Measurements
                .AsNoTracking()
                .Include(m => m.Nurse)
                .Where(m => m.Date.Date >= startOfWeek && m.Nurse != null)
                .GroupBy(m => m.Nurse!.Name)
                .Select(g => new { NurseName = g.Key, Count = g.Count() })
                .ToListAsync();

            var pieLabels = weeklyNurseGroups.Select(n => n.NurseName).ToList();
            var pieData = weeklyNurseGroups.Select(n => n.Count).ToList();

            PieChartLabelsJson = JsonSerializer.Serialize(pieLabels);
            PieChartDataJson = JsonSerializer.Serialize(pieData);
        }

        public async Task<IActionResult> OnGetExportXmlAsync()
        {
            IQueryable<Measurement> query = _context.Measurements
                .AsNoTracking()
                .Include(m => m.Patient)
                .Include(m => m.Nurse);

            if (SelectedPatientId.HasValue)
            {
                query = query.Where(m => m.PatientId == SelectedPatientId.Value);
            }
            if (StartDate.HasValue)
            {
                query = query.Where(m => m.Date.Date >= StartDate.Value.Date);
            }
            if (EndDate.HasValue)
            {
                query = query.Where(m => m.Date.Date <= EndDate.Value.Date);
            }

            var exportList = await query.OrderByDescending(m => m.Date).ToListAsync();

            var xDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("VernyomasNaploExport",
                    new XElement("ExportIdopont", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("SzuresiFeltetelek",
                        new XElement("PaciansId", SelectedPatientId.HasValue ? SelectedPatientId.Value.ToString() : "Osszes"),
                        new XElement("KezdoDatum", StartDate.HasValue ? StartDate.Value.ToString("yyyy-MM-dd") : "Nincs megadva"),
                        new XElement("VegDatum", EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : "Nincs megadva")
                    ),
                    new XElement("Meresek",
                        exportList.Select(m => new XElement("Meres",
                            new XElement("Id", m.Id),
                            new XElement("Datum", m.Date.ToString("yyyy-MM-dd HH:mm")),
                            new XElement("Idoszak", _statsService.IsMorningMeasurement(m.Date) ? "Reggel (0:00-12:00)" : "Este (12:00-24:00)"),
                            new XElement("Pacians",
                                new XElement("Nev", m.Patient?.Name ?? "Ismeretlen"),
                                new XElement("Szobaszam", m.Patient?.RoomNumber ?? "-"),
                                new XElement("SzuletesiDatum", m.Patient?.BirthDate.ToString("yyyy-MM-dd") ?? "-"),
                                new XElement("HozzatartozoTelefon", m.Patient?.RelativePhoneNumber ?? "-")
                            ),
                            new XElement("ApoloNeve", m.Nurse?.Name ?? "Ismeretlen"),
                            new XElement("Szisztoles", m.Systolic),
                            new XElement("Diasztoles", m.Diastolic),
                            new XElement("Pulzus", m.Pulse)
                        ))
                    )
                )
            );

            var bytes = Encoding.UTF8.GetBytes(xDoc.ToString());
            string fileName = $"vernyomas_riport_{DateTime.Now:yyyyMMdd_HHmm}.xml";
            return File(bytes, "application/xml", fileName);
        }

    }

    public class StatsSummary
    {
        public int Count { get; set; }
        public double AvgSys { get; set; }
        public double AvgDia { get; set; }
        public double AvgPulse { get; set; }
        public int MinSys { get; set; }
        public int MaxSys { get; set; }
        public int MinDia { get; set; }
        public int MaxDia { get; set; }
        public int MinPulse { get; set; }
        public int MaxPulse { get; set; }
    }
}