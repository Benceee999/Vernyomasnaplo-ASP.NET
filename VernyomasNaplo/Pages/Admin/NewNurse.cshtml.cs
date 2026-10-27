using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VernyomasNaplo.Data;

namespace VernyomasNaplo.Pages.Admin
{
    public class NewNurseModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public NewNurseModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Nurse NewNurse { get; set; } = default!;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _context.Nurses.Add(NewNurse);
                await _context.SaveChangesAsync();

                return RedirectToPage("/Admin/Nurses");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Hiba történt a mentés során: " + ex.Message);
                return Page();
            }
        }
    }
}