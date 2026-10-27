using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VernyomasNaplo.Data;

namespace VernyomasNaplo.Pages.User
{
    public class NewMeasurementModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public NewMeasurementModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Measurement NewMeasurement { get; set; } = default!;

        public IList<Patient> Patients { get; set; } = default!;
        public IList<Nurse> Nurses { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? patientId)
        {
            await LoadDropdownDataAsync();

            var now = DateTime.Now;
            NewMeasurement = new Measurement
            {
                Date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0)
            };

            if (patientId.HasValue)
            {
                NewMeasurement.PatientId = patientId.Value;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            ModelState.Remove("NewMeasurement.Patient");
            ModelState.Remove("NewMeasurement.Nurse");


            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return Page();
            }

            try
            {
                _context.Measurements.Add(NewMeasurement);
                await _context.SaveChangesAsync();

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Hiba történt a mentés során: " + ex.Message);
                await LoadDropdownDataAsync();
                return Page();
            }
        }

        private async Task LoadDropdownDataAsync()
        {
            Patients = await _context.Patients.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
            Nurses = await _context.Nurses.AsNoTracking().OrderBy(n => n.Name).ToListAsync();
        }
    }
}