DRIVENOW - DELIVERABLE 3
========================

Project
-------
ASP.NET Core MVC (.NET 9), SQL Server, ASP.NET Core Identity, Dapper,
Repository Pattern, Dependency Injection, jQuery AJAX and Bootstrap 5.

What is included
----------------
1. AJAX
   - $.get: live vehicle search; server returns the shared _CarCards partial.
   - $.post: vehicle creation (jQuery serializes the form and anti-forgery token).
   - $.ajax: vehicle deletion with an anti-forgery request header.
   - Add/delete update the Admin page without forcing a full page reload.

2. Dapper
   - CarRepository, BookingRepository and DashboardRepository use parameterized Dapper SQL.

3. Repository Pattern + DI
   - ICarRepository / CarRepository
   - IBookingRepository / BookingRepository
   - IDashboardRepository / DashboardRepository
   - All registered as scoped services in Program.cs and injected into controllers.

4. Identity customization
   - Custom Login, Register and AccessDenied views.
   - Admin and Customer roles, role-based controller authorization and custom cookie paths.
   - Public registration always creates a Customer; it cannot grant Admin privileges.
   - Optional administrator seeding is configured using AdminSeed:Email and AdminSeed:Password.

5. Layout sections
   - Shared _Layout renders optional Styles and Scripts sections.
   - Home and Admin views use the Scripts section.

6. Local IIS deployment
   - This project is prepared for publishing; deployment itself must be performed on the target Windows/IIS machine.

7. UI refresh
   - Updated palette: midnight navy, teal and cool mist.
   - Responsive home, authentication, customer dashboard, booking and admin screens.
   - Local Corolla/Civic/Prado images are used; other car names show a neutral fallback graphic.

Requirements
------------
- Visual Studio 2022 with .NET 9 SDK/workload, or .NET 9 SDK installed.
- SQL Server LocalDB for local development, or SQL Server Express/SQL Server for IIS.

Run locally
-----------
1. Extract the ZIP.
2. Open DriveNow.sln in Visual Studio.
3. Restore NuGet packages.
4. Check appsettings.json -> ConnectionStrings:DefaultConnection.
5. Build the solution and run with the HTTPS profile.
6. On first run, EnsureCreated creates the database and starter cars/roles.

Create an Admin account (recommended)
--------------------------------------
Do NOT register admin@drivenow.com as a normal customer expecting it to become Admin.
For a fresh setup, configure secrets before running the application:

  dotnet user-secrets init
  dotnet user-secrets set "AdminSeed:Email" "admin@drivenow.com"
  dotnet user-secrets set "AdminSeed:Password" "Choose-A-Strong-Password"

Then run the application. Program.cs creates the configured account and assigns the
Admin role. Never commit a real admin password to source control. The application
also preserves the legacy admin@drivenow.com account from the previous deliverable
if that account already exists in the database.

IIS deployment outline
----------------------
1. Install the .NET 9 Hosting Bundle on the IIS server.
2. Install/configure SQL Server Express or SQL Server on the server.
3. Create a database and set the connection string using an environment variable:

   ConnectionStrings__DefaultConnection
   Server=.\SQLEXPRESS;Database=DriveNowDb;Trusted_Connection=True;TrustServerCertificate=True

4. Grant the IIS App Pool identity appropriate SQL database permissions. For a
   coursework/demo environment db_owner may be used; use least privilege in production.
5. Publish from Visual Studio (Release -> Folder) or run:

   dotnet publish -c Release -o ./publish

6. In IIS, create an Application Pool (No Managed Code), create a Website pointing
   to the publish folder, bind an available HTTP/HTTPS port, and assign the App Pool.
7. Set ASPNETCORE_ENVIRONMENT=Production and the connection-string/admin seed
   environment variables in the IIS application configuration.
8. Browse the site and test registration, login, role access, search, add/delete,
   booking and booking history.

Important notes
---------------
- The project targets .NET 9 and EF Core/Identity 9 packages.
- EnsureCreated is suitable for this fresh coursework database. If you evolve an
  existing schema after changing entity definitions, use EF Core migrations rather
  than expecting EnsureCreated to alter existing tables.
- The booking model currently records rental duration (days), not pickup/return dates.
  Date-overlap availability cannot be enforced until those fields are part of the
  assignment's data model and database schema.
- External Google Fonts, Bootstrap CDN, Bootstrap Icons CDN and jQuery CDN require
  internet access. Vehicle images themselves are stored locally in wwwroot/images.
- Build/run/IIS deployment must be verified in Visual Studio/on the target machine.
  The environment used to prepare this ZIP did not have the .NET SDK installed.
