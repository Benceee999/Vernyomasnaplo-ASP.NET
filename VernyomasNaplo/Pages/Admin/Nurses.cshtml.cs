using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VernyomasNaplo.Data;

namespace VernyomasNaplo.Pages.Admin
{
    public class NursesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public NursesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Nurse> Nurses { get; set; } = default!;

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            Nurses = await _context.Nurses
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteNurseAsync(int id)
        {
            var hasMeasurements = await _context.Measurements.AnyAsync(m => m.NurseId == id);
            if (hasMeasurements)
            {
                ErrorMessage = "Az ápoló nem törölhető, mert már rögzített vérnyomásméréseket az adatbázisban!";
                return RedirectToPage();
            }

            var nurse = await _context.Nurses.FindAsync(id);
            if (nurse != null)
            {
                _context.Nurses.Remove(nurse);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}