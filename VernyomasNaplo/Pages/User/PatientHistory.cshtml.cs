using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VernyomasNaplo.Data;

namespace VernyomasNaplo.Pages.User
{
    public class PatientHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PatientHistoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Patient Patient { get; set; } = default!;
        public List<Measurement> Measurements { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Patient = await _context.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (Patient == null)
            {
                return NotFound();
            }

           
            Measurements = await _context.Measurements
                .AsNoTracking()
                .Include(m => m.Nurse)
                .Where(m => m.PatientId == id)
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            return Page();
        }
    }
}