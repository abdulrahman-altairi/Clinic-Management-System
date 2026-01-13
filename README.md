# 🏥 Smart Clinic Management System

> **A robust, enterprise-grade Clinic Management System built with .NET and SQL Server, strictly following the N-Tier Architecture.**

## 📖 Overview

The **Smart Clinic Management System** is a robust **Back-End Engine** designed to manage the core operations of medical clinics. It provides a strong infrastructure for managing patient records, appointments, and financial operations, with a primary focus on business logic stability and high-performance data handling.

The project is engineered as a **Library-Based System** utilizing a pure **Layered Architecture (N-Tier)**. It relies on complete **Separation of Concerns (SoC)**, making it decoupled and capable of integrating with any user interface (Desktop, Web, or Mobile). Currently, the solution includes a **Console UI** (`Clinic.Presentation`) dedicated solely to **Integration Testing** of the backend layers without affecting the production logic.

---

## 🏗 Software Architecture

This project is built upon a strict **4-Layer Architecture** to ensure maintainability, scalability, and testability:

1.  **Presentation Layer (UI)**: (Windows Forms / WPF - *Implied*) Handles user interaction and data visualization.
2.  **Contracts & DTOs Layer (`Clinic.Contracts`)**: Defines data structures (Data Transfer Objects) and Enums to decouple the internal database entities from the external layers.
3.  **Business Logic Layer (BLL) (`Clinic.BLL`)**: Contains the core business rules, complex validations, and workflow coordination. It creates a firewall between the UI and Data.
4.  **Data Access Layer (DAL) (`Clinic.DAL`)**: Manages high-performance database interactions using `ADO.NET`.
5.  **Entities Layer (`Clinic.Entities`)**: Represents the persistent domain objects (POCOs).

### 🏛 Technical Stack

*   **Language**: C# (.NET Framework)
*   **Database**: Microsoft SQL Server
*   **Data Access**: ADO.NET (Raw SQL for maximum control & performance)
*   **Design Patterns**: Repository Pattern, DTO Pattern, Static Factory Methods.
*   **Security**: SHA-256 Hashing for passwords, Parameterized Queries (Anti-SQL Injection).

---

## � Project Structure

```text
Clinic/
├── Clinic.BLL/             # Business Logic Layer
│   ├── Common/             # Shared utilities and constants
│   ├── Helper/             # Helper classes (e.g., Validation, Regex)
│   ├── Results/            # Operation result wrappers
│   ├── Services/           # Core business services
│   └── Validators/         # Data validation logic
├── Clinic.Contracts/       # Data Contracts (Shared)
│   ├── DTOs/               # Data Transfer Objects
│   ├── Enums/              # Enumerations
│   ├── Identity/           # Identity-related contracts
│   ├── Medical/            # Medical records/appointments contracts
│   ├── People/             # Personnel contracts
│   └── Views/              # View-specific contracts
├── Clinic.DAL/             # Data Access Layer
│   ├── DB/                 # Database helpers (DBHelper)
│   ├── Repositories/       # Data repositories
│   └── Views/              # Database views mapping
├── Clinic.Entities/        # Domain Entities
│   ├── Enums/              # Entity-level enums
│   └── Views/              # Entity views models
├── Clinic.Presentation/    # Console UI (Integration Testing Suite)
│   ├── Messages/           # Standardized result messages
│   ├── Tests/              # Functional test modules
│   └── Program.cs          # Test runner entry point
├── ClinicDB.sql            # Database schema script
├── Clinic.sln              # Solution file
└── README.md               # Project documentation
```

---

## �🚀 Key Features

### 👤 Identity & Access Management (IAM)
*   **Role-Based Access Control (RBAC)**: Secure capability management for Admins, Doctors, and Receptionists.
*   **Secure Authentication**: Industry-standard password hashing and secure login flows.
*   **User Auditing**: Tracking of `LastLogin`, `CreatedBy`, and active status monitoring.

### 👨‍⚕️ Doctor Management
*   **Comprehensive Profiles**: Management of specializations, bios, and contact info.
*   **Availability Tracking**: Real-time toggling of doctor availability.
*   **Performance Metrics**: Tracking of consultation fees and patient volume.
*   **Advanced Search**: Filter doctors by specialization, fee range, or availability.

### 🏥 Patient Administration
*   **Electronic Health Records (EHR)**: Centralized storage for patient demographics, contact details, and insurance info.
*   **Emergency Protocols**: Dedicated fields for emergency contacts and insurance policy validation.
*   **History Tracking**: Full history of appointments and invoices per patient.

### 📅 Appointment Scheduling
*   **Smart Booking System**: Conflict detection logic to prevent double-booking.
*   **Lifecycle Management**: Full state machine for appointments (Pending → Confirmed → Completed / Cancelled).
*   **View Layers**: specialized Views (`AppointmentView`) for optimized data retrieval and reporting.

### 💊 Medical Records
*   **Clinical Documentation**: Structured input for Diagnosis, Prescriptions, and internal Notes.
*   **History Constraints**: Medical records are strictly linked to confirmed appointments to ensure data integrity.

### 💰 Financial Management
*   **Invoicing System**: Auto-generation of invoices with tax, discount, and net amount calculations.
*   **Line-Item Detail**: Granular control over invoice items (services, medications).
*   **Payment Processing**: Support for multiple payment methods and partial payments.
*   **Financial Reporting**: Daily income reports and revenue tracking.

### 🧪 Integration Testing Suite (Console UI)
The solution includes a dedicated **Console Application (`Clinic.Presentation`)** that acts as a rigorous **System Integrity Checker**.
*   **Purpose**: To verify all backend logic (DAL + BLL) in isolation without GUI dependencies.
*   **Functionality**: It runs comprehensive test scenarios for every service (Doctors, Patients, Finances), validating success paths, error handling, and business rule enforcement.
*   **Safety**: Designed to test functions securely without affecting production configurations.

---

## 💾 Database Design

The database is designed with **Third Normal Form (3NF)** principals to ensure data integrity and reduce redundancy.

*   **People Inheritance**: A `People` table acts as the base for `Users`, `Doctors`, and `Patients`, implementing a Table-per-Type inheritance strategy in SQL.
*   **Relationships**: Strongly typed Foreign Key constraints ensure referential integrity across all modules.
*   **Views**: SQL Views (`vw_AllPatients`, `vw_AppointmentDetails`) are utilized to abstract complex joins for faster read operations.

---

## 🛠 Getting Started

### Prerequisites
*   Visual Studio 2019/2022
*   Microsoft SQL Server 2017+
*   .NET Framework / .NET Core SDK

### Installation
1.  **Clone the repository**:
    ```bash
    git clone https://github.com/abdulrahman-altairi/Clinic-Management-System.git
    ```
2.  **Database Setup**:
    *   Open SQL Server Management Studio (SSMS).
    *   Execute the `ClinicDB.sql` script located in the root directory to build the schema and seed initial data.
3.  **Configuration**:
    *   Open `App.config` or `Web.config`.
    *   Update the `SmartClinicConnection` connection string to point to your local SQL instance.
4.  **Run**:
    *   Build the solution in Visual Studio.
    *   Start the application.

---

## 👨‍💻 Code Examples

**Safe Data Access (Preventing SQL Injection):**
```csharp
 public int AddUser(User user, SqlConnection connection = null, SqlTransaction transaction = null)
{
    string query = @"INSERT INTO Users (UserID, UserName, PasswordHash, Role, IsActive, LastLogin)
            VALUES(@UserID, @UserName, @PasswordHash, @Role, @IsActive, @LastLogin)";
            
            // Using SqlParameter to strictly type and sanitize input

    SqlParameter[] parameters =
    {
        new SqlParameter("@UserID", user.UserId),
        new SqlParameter("@UserName", user.UserName),
        new SqlParameter("@PasswordHash", user.PasswordHash),
        new SqlParameter("@Role", user.Role),
        new SqlParameter("@IsActive", user.IsActive),
        new SqlParameter("@LastLogin", (object)user.LastLogin ?? DBNull.Value),
    };
    SqlConnection connToUse = connection ?? DBHelper.GetOpenConnection();
    return DBHelper.ExecuteNonQuery(query, parameters, connToUse, transaction);
}
```

**Business Logic Validation:**
```csharp
public bool IsAppointmentAvailable(int doctorId, DateTime date)
{
    // Business rule: Verify no overlapping appointments exist
    int conflictCount = _appointmentRepo.GetConflictCount(doctorId, date, DefaultDuration);
    return conflictCount == 0;
}
```

---

## 🤝 Contact
**Abdulrahman Al-Tairi**  
.NET Developer  
[[LinkedIn/Portfolio Link](https://www.linkedin.com/in/a-altairi/)]

