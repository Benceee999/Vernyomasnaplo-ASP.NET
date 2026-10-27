using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace VernyomasNaplo.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
}

public class PastOrTodayDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            // Pár perces türelmi idő a lokális gép / szerver óraeltérésre
            if (date > DateTime.Now.AddMinutes(5))
            {
                return new ValidationResult(ErrorMessage ?? "A dátum nem lehet a jövőben!");
            }
        }
        return ValidationResult.Success;
    }
}

public class Patient
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A név megadása kötelező!")]
    [StringLength(100, ErrorMessage = "A név maximum 100 karakter lehet!")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "A szobaszám megadása kötelező!")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "A szobaszám csak számokat tartalmazhat!")]
    [StringLength(10, ErrorMessage = "A szobaszám maximum 10 karakter lehet!")]
    public string RoomNumber { get; set; } = null!;

    [RegularExpression(@"^[\+\-\s0-9]+$", ErrorMessage = "Érvénytelen telefonszám formátum!")]
    [StringLength(20, ErrorMessage = "A telefonszám maximum 20 karakter lehet!")]
    public string RelativePhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "A születési dátum kötelező!")]
    [DataType(DataType.Date)]
    [PastOrTodayDate(ErrorMessage = "A születési dátum nem lehet jövőbeli!")]
    public DateTime BirthDate { get; set; }

    public List<Measurement> Measurements { get; set; } = new();
}

public class Nurse
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A név megadása kötelező!")]
    [StringLength(100, ErrorMessage = "A név maximum 100 karakter lehet!")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Az e-mail cím megadása kötelező!")]
    [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím formátum!")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "A telefonszám megadása kötelező!")]
    [RegularExpression(@"^[\+\-\s0-9]+$", ErrorMessage = "Érvénytelen telefonszám formátum!")]
    [StringLength(20, ErrorMessage = "A telefonszám maximum 20 karakter lehet!")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Az adószám megadása kötelező!")]
    [RegularExpression("^[0-9]{10}$", ErrorMessage = "Az adószámnak pontosan 10 számjegyből kell állnia!")]
    public string TaxId { get; set; } = null!;

    public List<Measurement> Measurements { get; set; } = new();
}

public class Measurement : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A dátum és idő megadása kötelező!")]
    [PastOrTodayDate(ErrorMessage = "A mérés időpontja nem lehet a jövőben!")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "A szisztolés érték kötelező!")]
    [Range(50, 250, ErrorMessage = "A szisztolés érték (Sys) 50 és 250 között lehet!")]
    public int Systolic { get; set; }

    [Required(ErrorMessage = "A diasztolés érték kötelező!")]
    [Range(30, 150, ErrorMessage = "A diasztolés érték (Dia) 30 és 150 között lehet!")]
    public int Diastolic { get; set; }

    [Required(ErrorMessage = "A pulzus megadása kötelező!")]
    [Range(30, 200, ErrorMessage = "A pulzusnak 30 és 200 között kell lennie!")]
    public int Pulse { get; set; }

    [Required]
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    [Required]
    public int NurseId { get; set; }
    public Nurse Nurse { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Systolic <= Diastolic)
        {
            yield return new ValidationResult(
                "A szisztolés értéknek (Sys) nagyobbnak kell lennie a diasztolés értéknél (Dia)!",
                new[] { nameof(Systolic), nameof(Diastolic) });
        }
    }
}