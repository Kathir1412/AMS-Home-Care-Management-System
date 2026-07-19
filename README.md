# AMS Home Care Attendance Management System

A production-ready, full-stack **Home Care Attendance & Schedule Management System** built for **AMS Home Care**. Designed to manage healthcare providers, scheduling, daily attendance, leaves, and custom reporting.

## 🚀 Technology Stack & Architectures

This project is built using modern enterprise design patterns and technologies:
* **Framework**: ASP.NET Core 9 MVC
* **Database Access**: Entity Framework Core 9
* **Database**: MS SQL Server (LocalDB / Express)
* **Authentication & Membership**: ASP.NET Core Identity
* **Styling & Layout**: Bootstrap 5, Font Awesome Icons, and Custom Medical Theme Variables
* **Architecture Pattern**: Clean Architecture (Domain-Driven, Decoupled Layers)
* **Design Patterns**: Repository Pattern & Unit of Work for structured data persistence.
* **Reporting & Analytics**: Interactive jQuery DataTables with HTML5 exports (CSV, Excel, PDF, Print).

---

## 🏛️ Project Directory Structure

The solution contains three primary projects following the Clean Architecture principle:

1. **AmsHomeCare.Core**: Core Domain Layer containing all business models/entities, custom Enums, and Repository definitions.
2. **AmsHomeCare.Infrastructure**: Data & Identity Layer containing `AppDbContext`, Identity user extensions, Repository/Unit of Work implementations, and Database Seeding logic.
3. **AmsHomeCare.Web**: Presentation Layer hosting MVC Controllers, ViewModels, Razor Views, dynamic themes, and asset files.

---

## 📂 Key Features & Modules

* **Role-Based Authentication**: Secure login workflows with Admin and Supervisor levels.
* **Interactive Dashboard**: Operational metrics, timeline activities feed, and scheduled duty listings.
* **Employee Management**: Care provider profiles with auto-generated Employee Codes (`EMP-001`).
* **Patient Management**: Patient health profiles, diagnostics, and caregiver assignments.
* **Shift Manager**: Morning, Evening, Night, or Custom schedule timings configurations.
* **Duty Assignment**: Date range scheduling and supervisor instructions.
* **Daily Attendance Sheet**: Single-screen grid to mark daily attendance status, timespans, and overtime hours for all employees.
* **Leave Requests**: Applied leaves automatically log attendance as "Leave" upon Supervisor approval.
* **Dynamic Theme Selector**: Choose between **Light Medical**, **Dark Clinical**, and **Classic Teal** themes with persistence.
* **Administrative Utilities**: Profile customization, Holiday Masters, and an integrated User Password Reset modal.

---

## 🛠️ Getting Started & Local Setup

### Prerequisites
* .NET SDK 9.0+
* MS SQL Server / SQL Express

### Configuration
1. Open the [appsettings.json](AmsHomeCare.Web/appsettings.json) file.
2. Update the connection string to target your active SQL Server instance:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=AmsHomeCareDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### Run Project
Navigate to the Web project and run the server:
```powershell
cd AmsHomeCare.Web
dotnet run
```
Open your browser and navigate to: **`http://localhost:6100`**

---

## 🔑 Default Accounts (Seeded Data)

On first run, the database is automatically created, migrated, and seeded with these default user credentials:

| Role | Username / Email | Password |
|---|---|---|
| **Admin** | `admin@amshomecare.com` | `Admin@123` |
| **Supervisor** | `supervisor@amshomecare.com` | `Supervisor@123` |
