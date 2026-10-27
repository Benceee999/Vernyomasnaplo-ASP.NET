using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VernyomasNaplo.Data;

namespace VernyomasNaplo.Pages.Admin
{
    public class PatientsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PatientsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Patient> Patients { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Patients = await _context.Patients.Include(p => p.Measurements).ToListAsync();
        }
    }
}