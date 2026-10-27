using Microsoft.EntityFrameworkCore;
using VernyomasNaplo.Data;
using VernyomasNaplo.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=vernyomas.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddRazorPages();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();

    if (!db.Patients.Any())
    {
        var nurse1 = new Nurse { Name = "Kovács Anna", Email = "kovacs.anna@otthon.hu", PhoneNumber = "+36301112233", TaxId = "1234567890" };
        var nurse2 = new Nurse { Name = "Nagy Péter", Email = "nagy.peter@otthon.hu", PhoneNumber = "+36304445566", TaxId = "0987654321" };
        db.Nurses.AddRange(nurse1, nurse2);

        var p1 = new Patient { Name = "Szabó János", RoomNumber = "101", RelativePhoneNumber = "+36209998877", BirthDate = new DateTime(1948, 4, 12) };
        var p2 = new Patient { Name = "Tóth Ilona", RoomNumber = "102", RelativePhoneNumber = "+36205556644", BirthDate = new DateTime(1952, 9, 23) };
        var p3 = new Patient { Name = "Varga Béla", RoomNumber = "103", RelativePhoneNumber = "+36307771122", BirthDate = new DateTime(1945, 1, 15) };
        db.Patients.AddRange(p1, p2, p3);

        db.SaveChanges();

        var today = DateTime.Today;
        db.Measurements.AddRange(
            new Measurement { PatientId = p1.Id, NurseId = nurse1.Id, Date = today.AddHours(8).AddMinutes(15), Systolic = 125, Diastolic = 82, Pulse = 68 },
            new Measurement { PatientId = p1.Id, NurseId = nurse2.Id, Date = today.AddHours(18).AddMinutes(30), Systolic = 130, Diastolic = 85, Pulse = 72 },

            new Measurement { PatientId = p2.Id, NurseId = nurse1.Id, Date = today.AddHours(9).AddMinutes(0), Systolic = 145, Diastolic = 92, Pulse = 80 },

            new Measurement { PatientId = p3.Id, NurseId = nurse2.Id, Date = today.AddDays(-1).AddHours(10), Systolic = 118, Diastolic = 76, Pulse = 65 }
        );

        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();