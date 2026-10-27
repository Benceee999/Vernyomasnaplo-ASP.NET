using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VernyomasNaplo.Data;

namespace VernyomasNaplo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Patient> Patients { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string UserFilter { get; set; } = "all";

        public async Task OnGetAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var query = _context.Patients
                .AsNoTracking()
                .Include(p => p.Measurements.Where(m => m.Date >= today && m.Date < tomorrow))
                .OrderBy(p => p.RoomNumber)
                .AsQueryable();

            var patientList = await query.ToListAsync();

            if (UserFilter == "missing_morning")
            {
                Patients = patientList.Where(p => !p.Measurements.Any(m => m.Date.Hour < 12)).ToList();
            }
            else if (UserFilter == "missing_evening")
            {
                Patients = patientList.Where(p => !p.Measurements.Any(m => m.Date.Hour >= 12)).ToList();
            }
            else if (UserFilter == "done_all")
            {
                Patients = patientList.Where(p =>
                    p.Measurements.Any(m => m.Date.Hour < 12) &&
                    p.Measurements.Any(m => m.Date.Hour >= 12)
                ).ToList();
            }
            else
            {
                Patients = patientList;
            }
        }
    }
}