using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VernyomasNaplo.Data;

namespace VernyomasNaplo.Pages.User
{
    public class EditMeasurementModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditMeasurementModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Measurement EditMeasurement { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EditMeasurement = await _context.Measurements
                .AsNoTracking()
                .Include(m => m.Patient)
                .Include(m => m.Nurse)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (EditMeasurement == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("EditMeasurement.Patient");
            ModelState.Remove("EditMeasurement.Nurse");

            if (!ModelState.IsValid)
            {
                var patient = await _context.Patients.FindAsync(EditMeasurement.PatientId);
                var nurse = await _context.Nurses.FindAsync(EditMeasurement.NurseId);
                EditMeasurement.Patient = patient!;
                EditMeasurement.Nurse = nurse!;
                return Page();
            }

            var measurementToUpdate = await _context.Measurements.FindAsync(EditMeasurement.Id);
            if (measurementToUpdate == null)
            {
                return NotFound();
            }

            measurementToUpdate.Systolic = EditMeasurement.Systolic;
            measurementToUpdate.Diastolic = EditMeasurement.Diastolic;
            measurementToUpdate.Pulse = EditMeasurement.Pulse;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/User/PatientHistory", new { id = measurementToUpdate.PatientId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Measurements.Any(e => e.Id == EditMeasurement.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var measurement = await _context.Measurements.FindAsync(id);
            if (measurement != null)
            {
                int patientId = measurement.PatientId;
                _context.Measurements.Remove(measurement);
                await _context.SaveChangesAsync();
                return RedirectToPage("/User/PatientHistory", new { id = patientId });
            }

            return RedirectToPage("/Index");
        }
    }
}