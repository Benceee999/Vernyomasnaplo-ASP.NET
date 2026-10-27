# Blood Pressure Tracker (VérnyomásNapló)

A web application built with ASP.NET Core Razor Pages to record, track, and analyze daily blood pressure measurements for care home residents.

## Features

- **Nurse View:** Track daily required measurements (morning and evening slots), visual alerts for abnormal readings, and patient history.
- **Admin Dashboard:** Manage patients and staff, filter measurements by date range and patient.
- **Statistics & Charts:** Aggregated metrics (average, min, max) and visual reports using Chart.js (averages comparison and weekly measurements per nurse).
- **XML Export:** Export filtered measurement data to structured XML.
- **Automated Database Setup:** SQLite database with EF Core migrations and seed data generated automatically on first run.

## Tech Stack

- **Backend:** .NET 10, ASP.NET Core Razor Pages, Entity Framework Core
- **Database:** SQLite
- **Frontend:** Bootstrap 5, Chart.js
- **Testing:** xUnit

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/)

### Running the App

```bash
cd VernyomasNaplo
dotnet run
```

The app will automatically create the local SQLite database (`vernyomas.db`) and seed initial mock data. Open `https://localhost:7141` in your browser.

### Running Tests

```bash
dotnet test
```

---

# Vérnyomás Napló

ASP.NET Core Razor Pages alapú webalkalmazás idősotthoni vérnyomásmérések rögzítésére, napi követésére és kiértékelésére.

## Funkciók

- **Ápolói felület:** Napi kötelező mérések követése (reggeli/esti bontásban), figyelmeztetés kiugró értékeknél, mérési előzmények páciensenként.
- **Adminisztráció:** Páciensek és ápolók kezelése, szűrések időszakra és személyre.
- **Statisztika és diagramok:** Átlag, min, max értékek számítása, valamint Chart.js alapú oszlop- és kördiagramok.
- **XML export:** A szűrt mérési adatok exportálása és letöltése XML fájlként.
- **Automatikus adatbázis:** EF Core SQLite migráció és mintaadat-feltöltés első indításkor.

## Technológiák

- .NET 10 (C#)
- ASP.NET Core Razor Pages
- Entity Framework Core (SQLite)
- Bootstrap 5, Chart.js
- xUnit tesztprojekt

## Futtatás

```bash
cd VernyomasNaplo
dotnet run
```

Indításkor a program automatikusan létrehozza a helyi SQLite adatbázist és betölti a kezdeti tesztadatokat.

Tesztek futtatása:
```bash
dotnet test
```