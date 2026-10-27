using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VernyomasNaplo.Data;

namespace VernyomasNaplo.Pages.Admin
{
    public class NewPatientModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public NewPatientModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Patient NewPatient { get; set; } = default!;

        public void OnGet()
        {
            NewPatient = new Patient
            {
                BirthDate = DateTime.Today.AddYears(-50)
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _context.Patients.Add(NewPatient);

                await _context.SaveChangesAsync();

                return RedirectToPage("/Admin/Patients");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Hiba történt a mentés során: " + ex.Message);
                return Page();
            }
        }
    }
}