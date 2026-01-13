/*
 * ==============================================================================================
 * Script Name: SmartClinicDB Setup Script
 * Description: Complete setup for Clinic Management System (Tables, Views, Relationships).
 * Author:      [Your Name/Organization]
 * Date:        2026-01-10
 * ==============================================================================================
 */

-- 1. Setup Database
USE [master];
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SmartClinicDB')
BEGIN
    CREATE DATABASE [SmartClinicDB];
END
GO

USE [SmartClinicDB];
GO

-- Set standard settings
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ==============================================================================================
   SECTION 1: BASE TABLES (Lookup & Master Data)
   ============================================================================================== */

-- Table: People (Base table for Patients, Doctors, and Users)
CREATE TABLE [dbo].[People](
    [PersonID]      INT IDENTITY(1,1) NOT NULL,
    [FirstName]     NVARCHAR(50) NOT NULL,
    [LastName]      NVARCHAR(50) NOT NULL,
    [DateOfBirth]   DATE NOT NULL,
    [Gender]        TINYINT NOT NULL, -- 0: Male, 1: Female, etc.
    [ContactNumber] NVARCHAR(20) NOT NULL,
    [Email]         NVARCHAR(100) NOT NULL,
    [Address]       NVARCHAR(255) NULL,
    [CreatedAt]     DATETIME CONSTRAINT [DF_People_CreatedAt] DEFAULT (GETDATE()),
    [IsDeleted]     BIT CONSTRAINT [DF_People_IsDeleted] DEFAULT (0),

    CONSTRAINT [PK_People] PRIMARY KEY CLUSTERED ([PersonID]),
    CONSTRAINT [UQ_People_Email] UNIQUE NONCLUSTERED ([Email]),
    CONSTRAINT [CK_People_Gender] CHECK ([Gender] IN (0, 1, 2))
);
GO

-- Table: Specializations
CREATE TABLE [dbo].[Specializations](
    [SpecializationID]   INT IDENTITY(1,1) NOT NULL,
    [SpecializationName] NVARCHAR(100) NOT NULL,
    [Description]        NVARCHAR(255) NULL,

    CONSTRAINT [PK_Specializations] PRIMARY KEY CLUSTERED ([SpecializationID]),
    CONSTRAINT [UQ_Specializations_Name] UNIQUE NONCLUSTERED ([SpecializationName])
);
GO

/* ==============================================================================================
   SECTION 2: DERIVED TABLES (Inheritance & Roles)
   ============================================================================================== */

-- Table: Patients (Linked to People)
CREATE TABLE [dbo].[Patients](
    [PatientID]             INT NOT NULL,
    [InsuranceProvider]     NVARCHAR(100) NULL,
    [InsurancePolicyNumber] NVARCHAR(50) NULL,
    [EmergencyContactName]  NVARCHAR(100) NULL,
    [EmergencyContactPhone] NVARCHAR(20) NULL,

    CONSTRAINT [PK_Patients] PRIMARY KEY CLUSTERED ([PatientID]),
    CONSTRAINT [FK_Patients_People] FOREIGN KEY ([PatientID]) REFERENCES [dbo].[People] ([PersonID])
);
GO

-- Table: Doctors (Linked to People & Specializations)
CREATE TABLE [dbo].[Doctors](
    [DoctorID]          INT NOT NULL,
    [SpecializationID]  INT NOT NULL,
    [Bio]               NVARCHAR(MAX) NULL,
    [ConsultationFee]   DECIMAL(10, 2) NOT NULL,
    [IsAvailable]       BIT CONSTRAINT [DF_Doctors_IsAvailable] DEFAULT (1),
    [JoinedAt]          DATETIME CONSTRAINT [DF_Doctors_JoinedAt] DEFAULT (GETDATE()),

    CONSTRAINT [PK_Doctors] PRIMARY KEY CLUSTERED ([DoctorID]),
    CONSTRAINT [CK_Doctors_ConsultationFee] CHECK ([ConsultationFee] >= 0),
    CONSTRAINT [FK_Doctors_People] FOREIGN KEY ([DoctorID]) REFERENCES [dbo].[People] ([PersonID]),
    CONSTRAINT [FK_Doctors_Specializations] FOREIGN KEY ([SpecializationID]) REFERENCES [dbo].[Specializations] ([SpecializationID])
);
GO

-- Table: Users (System Access)
CREATE TABLE [dbo].[Users](
    [UserID]        INT NOT NULL,
    [Username]      NVARCHAR(50) NOT NULL,
    [PasswordHash]  NVARCHAR(255) NOT NULL,
    [Role]          TINYINT NOT NULL, -- Enum: Admin, Doctor, Receptionist, etc.
    [IsActive]      BIT CONSTRAINT [DF_Users_IsActive] DEFAULT (1),
    [LastLogin]     DATETIME NULL,

    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserID]),
    CONSTRAINT [UQ_Users_Username] UNIQUE NONCLUSTERED ([Username]),
    CONSTRAINT [FK_Users_People] FOREIGN KEY ([UserID]) REFERENCES [dbo].[People] ([PersonID])
);
GO

/* ==============================================================================================
   SECTION 3: TRANSACTIONAL TABLES
   ============================================================================================== */

-- Table: Appointments
CREATE TABLE [dbo].[Appointments](
    [AppointmentID]      INT IDENTITY(1,1) NOT NULL,
    [PatientID]          INT NOT NULL,
    [DoctorID]           INT NOT NULL,
    [AppointmentDate]    DATETIME NOT NULL,
    [DurationMinutes]    INT CONSTRAINT [DF_Appointments_Duration] DEFAULT (30),
    [AppointmentEndDate] AS (DATEADD(MINUTE, [DurationMinutes], [AppointmentDate])) PERSISTED,
    [Status]             INT CONSTRAINT [DF_Appointments_Status] DEFAULT (0) NOT NULL,
    [ReasonForVisit]     NVARCHAR(255) NULL,
    [CreatedAt]          DATETIME CONSTRAINT [DF_Appointments_CreatedAt] DEFAULT (GETDATE()),
    [CreatedBy]          INT NULL,
    [LastUpdatedAt]      DATETIME NULL,
    [LastUpdatedBy]      INT NULL,

    CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED ([AppointmentID]),
    CONSTRAINT [CK_Appointments_Status] CHECK ([Status] BETWEEN 0 AND 4),
    CONSTRAINT [FK_Appointments_Patients] FOREIGN KEY ([PatientID]) REFERENCES [dbo].[Patients] ([PatientID]),
    CONSTRAINT [FK_Appointments_Doctors] FOREIGN KEY ([DoctorID]) REFERENCES [dbo].[Doctors] ([DoctorID])
);
GO

-- Table: MedicalRecords
CREATE TABLE [dbo].[MedicalRecords](
    [RecordID]      INT IDENTITY(1,1) NOT NULL,
    [AppointmentID] INT NOT NULL,
    [Diagnosis]     NVARCHAR(MAX) NOT NULL,
    [Prescription]  NVARCHAR(MAX) NULL,
    [Notes]         NVARCHAR(MAX) NULL,
    [CreatedDate]   DATETIME CONSTRAINT [DF_MedicalRecords_CreatedDate] DEFAULT (GETDATE()),

    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY CLUSTERED ([RecordID]),
    CONSTRAINT [UQ_MedicalRecords_Appointment] UNIQUE NONCLUSTERED ([AppointmentID]),
    CONSTRAINT [FK_MedicalRecords_Appointments] FOREIGN KEY ([AppointmentID]) REFERENCES [dbo].[Appointments] ([AppointmentID])
);
GO

-- Table: Invoices
CREATE TABLE [dbo].[Invoices](
    [InvoiceID]      INT IDENTITY(1,1) NOT NULL,
    [InvoiceNumber]  AS ('INV-' + CONVERT(NVARCHAR(10), [InvoiceID])),
    [AppointmentID]  INT NOT NULL,
    [PatientID]      INT NOT NULL,
    [TotalAmount]    DECIMAL(10, 2) CONSTRAINT [DF_Invoices_TotalAmount] DEFAULT (0) NOT NULL,
    [TaxAmount]      DECIMAL(10, 2) CONSTRAINT [DF_Invoices_TaxAmount] DEFAULT (0) NULL,
    [DiscountAmount] DECIMAL(10, 2) CONSTRAINT [DF_Invoices_DiscountAmount] DEFAULT (0) NULL,
    [NetAmount]      AS (([TotalAmount] + ISNULL([TaxAmount], 0)) - ISNULL([DiscountAmount], 0)),
    [InvoiceDate]    DATETIME CONSTRAINT [DF_Invoices_InvoiceDate] DEFAULT (GETDATE()),
    [DueDate]        DATETIME NULL,
    [InvoiceStatus]  TINYINT CONSTRAINT [DF_Invoices_Status] DEFAULT (1) NOT NULL,

    CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([InvoiceID]),
    CONSTRAINT [UQ_Invoices_Appointment] UNIQUE NONCLUSTERED ([AppointmentID]),
    CONSTRAINT [CK_Invoices_Status] CHECK ([InvoiceStatus] IN (0, 1, 2, 3, 4)),
    CONSTRAINT [FK_Invoices_Appointments] FOREIGN KEY ([AppointmentID]) REFERENCES [dbo].[Appointments] ([AppointmentID]),
    CONSTRAINT [FK_Invoices_Patients] FOREIGN KEY ([PatientID]) REFERENCES [dbo].[Patients] ([PatientID])
);
GO

-- Table: InvoiceItems
CREATE TABLE [dbo].[InvoiceItems](
    [ItemID]          INT IDENTITY(1,1) NOT NULL,
    [InvoiceID]       INT NOT NULL,
    [ItemDescription] NVARCHAR(255) NOT NULL,
    [UnitPrice]       DECIMAL(10, 2) NOT NULL,
    [Quantity]        INT CONSTRAINT [DF_InvoiceItems_Quantity] DEFAULT (1) NULL,
    [LineTotal]       AS ([UnitPrice] * ISNULL([Quantity], 1)),

    CONSTRAINT [PK_InvoiceItems] PRIMARY KEY CLUSTERED ([ItemID]),
    CONSTRAINT [FK_InvoiceItems_Invoices] FOREIGN KEY ([InvoiceID]) REFERENCES [dbo].[Invoices] ([InvoiceID])
);
GO

-- Table: Payments
CREATE TABLE [dbo].[Payments](
    [PaymentID]      INT IDENTITY(1,1) NOT NULL,
    [InvoiceID]      INT NOT NULL,
    [PaymentAmount]  DECIMAL(10, 2) NOT NULL,
    [PaymentDate]    DATETIME CONSTRAINT [DF_Payments_PaymentDate] DEFAULT (GETDATE()),
    [PaymentMethod]  INT NOT NULL,
    [TransactionRef] NVARCHAR(100) NULL,

    CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([PaymentID]),
    CONSTRAINT [CK_Payments_Method] CHECK ([PaymentMethod] IN (1, 2, 3, 4)),
    CONSTRAINT [FK_Payments_Invoices] FOREIGN KEY ([InvoiceID]) REFERENCES [dbo].[Invoices] ([InvoiceID])
);
GO

/* ==============================================================================================
   SECTION 4: DATABASE VIEWS
   ============================================================================================== */

-- View: All Patients (Combines People and Patients)
CREATE VIEW [dbo].[vw_AllPatients] AS
SELECT 
    p.PersonID AS PatientID,
    p.FirstName, 
    p.LastName, 
    p.Email, 
    p.ContactNumber, 
    p.DateOfBirth,
    pat.InsuranceProvider, 
    pat.EmergencyContactName
FROM dbo.People p
INNER JOIN dbo.Patients pat ON p.PersonID = pat.PatientID;
GO

-- View: All Doctors (Combines People, Doctors and Specialization)
CREATE VIEW [dbo].[vw_AllDoctors] AS
SELECT 
    p.PersonID AS DoctorID,
    p.FirstName, 
    p.LastName, 
    p.Email, 
    p.ContactNumber,
    d.SpecializationID, 
    s.SpecializationName,
    d.Bio, 
    d.ConsultationFee, 
    d.IsAvailable
FROM dbo.People p
INNER JOIN dbo.Doctors d ON p.PersonID = d.DoctorID
INNER JOIN dbo.Specializations s ON d.SpecializationID = s.SpecializationID;
GO

-- View: Appointment Details (Full details for display)
CREATE VIEW [dbo].[vw_AppointmentDetails] AS
SELECT 
    a.AppointmentID,
    (pat_p.FirstName + ' ' + pat_p.LastName) AS PatientName,
    (doc_p.FirstName + ' ' + doc_p.LastName) AS DoctorName,
    s.SpecializationName,
    a.AppointmentDate,
    a.Status AS AppointmentStatus,
    a.ReasonForVisit,
    d.ConsultationFee
FROM dbo.Appointments a
INNER JOIN dbo.Patients pat ON a.PatientID = pat.PatientID
INNER JOIN dbo.People pat_p ON pat.PatientID = pat_p.PersonID
INNER JOIN dbo.Doctors d ON a.DoctorID = d.DoctorID
INNER JOIN dbo.People doc_p ON d.DoctorID = doc_p.PersonID
INNER JOIN dbo.Specializations s ON d.SpecializationID = s.SpecializationID;
GO


/* ==============================================================================================
   SECTION 5: PERFORMANCE OPTIMIZATION (INDEXES)
============================================================================================== */

CREATE INDEX IX_People_FullName ON [dbo].[People] ([FirstName], [LastName]);
GO

CREATE INDEX IX_Appointments_Date_Status ON [dbo].[Appointments] ([AppointmentDate], [Status]);
CREATE INDEX IX_Appointments_PatientID ON [dbo].[Appointments] ([PatientID]);
GO

CREATE INDEX IX_Users_IsActive ON [dbo].[Users] ([IsActive]) INCLUDE ([Username]);
GO

CREATE INDEX IX_Invoices_Patient_Date ON [dbo].[Invoices] ([PatientID], [InvoiceDate]);
CREATE INDEX IX_Invoices_Status ON [dbo].[Invoices] ([InvoiceStatus]);
GO


PRINT 'Database [SmartClinicDB1] setup completed successfully.';



-- ##############################################################################################


                                      -- INSERT DATA --


-- ##############################################################################################
/*
 * ==============================================================================================
 * Script Name: Seed Data for SmartClinicDB
 * Description: Populates the database with realistic sample data using C# Enums mappings.
 * Note:        Order of execution is critical to maintain Referential Integrity.
 * ==============================================================================================
 */

USE [ClinicDB];
GO

-- A variable to ensure dates are relative to "Now"
DECLARE @CurrentDate DATETIME = GETDATE();

-- ==============================================================================================
-- 1. Insert Specializations 
-- ==============================================================================================
PRINT 'Seeding Specializations...';
SET IDENTITY_INSERT [dbo].[Specializations] ON;

INSERT INTO [dbo].[Specializations] ([SpecializationID], [SpecializationName], [Description]) VALUES 
(1, 'Cardiology', 'Heart and blood vessel disorders'),
(2, 'Dermatology', 'Skin, hair, and nail conditions'),
(3, 'Pediatrics', 'Medical care of infants, children, and adolescents'),
(4, 'Orthopedics', 'Musculoskeletal system'),
(5, 'General Practice', 'Primary health care and general medicine');

SET IDENTITY_INSERT [dbo].[Specializations] OFF;
GO

-- ==============================================================================================
-- 2. Insert People 
-- Strategy: IDs 1-5 (Doctors), IDs 6-10 (Patients), IDs 11-13 (Staff)
-- Enums: Gender -> Male=1, Female=2
-- ==============================================================================================
PRINT 'Seeding People...';
SET IDENTITY_INSERT [dbo].[People] ON;

INSERT INTO [dbo].[People] 
([PersonID], [FirstName], [LastName], [DateOfBirth], [Gender], [ContactNumber], [Email], [Address], [CreatedAt]) VALUES 
-- Doctors (1-5)
(1, 'John', 'Smith', '1980-05-15', 1, '555-0101', 'dr.john@clinic.com', '123 Medical Blvd', GETDATE()),
(2, 'Sarah', 'Connor', '1985-08-20', 2, '555-0102', 'dr.sarah@clinic.com', '456 Healing Way', GETDATE()),
(3, 'Emily', 'Davis', '1979-11-05', 2, '555-0103', 'dr.emily@clinic.com', '789 Care Ln', GETDATE()),
(4, 'Michael', 'Brown', '1975-02-14', 1, '555-0104', 'dr.michael@clinic.com', '321 Health St', GETDATE()),
(5, 'David', 'Wilson', '1982-06-30', 1, '555-0105', 'dr.david@clinic.com', '654 Wellness Ave', GETDATE()),

-- Patients (6-10)
(6, 'Alice', 'Wonder', '1995-04-10', 2, '555-0201', 'alice.w@email.com', '10 Wonderland Park', GETDATE()),
(7, 'Bob', 'Builder', '1990-12-01', 1, '555-0202', 'bob.b@email.com', '20 Construction Rd', GETDATE()),
(8, 'Charlie', 'Chaplin', '1988-03-25', 1, '555-0203', 'charlie.c@email.com', '30 Silent Movie Dr', GETDATE()),
(9, 'Diana', 'Prince', '1992-07-15', 2, '555-0204', 'diana.p@email.com', '40 Amazonian Isle', GETDATE()),
(10, 'Ethan', 'Hunt', '1985-09-09', 1, '555-0205', 'ethan.h@email.com', '50 Mission St', GETDATE()),

-- Staff (11-13)
(11, 'Admin', 'User', '1980-01-01', 1, '555-9999', 'admin@clinic.com', 'Server Room 1', GETDATE()),
(12, 'Reception', 'Lady', '1990-05-05', 2, '555-8888', 'reception@clinic.com', 'Front Desk', GETDATE()),
(13, 'Nurse', 'Joy', '1993-02-02', 2, '555-7777', 'joy@clinic.com', 'Nurse Station', GETDATE());

SET IDENTITY_INSERT [dbo].[People] OFF;
GO

-- ==============================================================================================
-- 3. Insert Doctors 
-- Linking People (1-5) to Specializations (1-5)
-- ==============================================================================================
PRINT 'Seeding Doctors...';
INSERT INTO [dbo].[Doctors] ([DoctorID], [SpecializationID], [Bio], [ConsultationFee], [IsAvailable], [JoinedAt]) VALUES 
(1, 1, 'Senior Cardiologist with 15 years experience.', 150.00, 1, DATEADD(YEAR, -5, GETDATE())), -- Cardio
(2, 2, 'Expert in cosmetic dermatology.', 120.00, 1, DATEADD(YEAR, -4, GETDATE())),             -- Derma
(3, 3, 'Loves kids and specialized in newborns.', 100.00, 1, DATEADD(YEAR, -3, GETDATE())),       -- Peds
(4, 4, 'Sports injury specialist.', 180.00, 1, DATEADD(YEAR, -2, GETDATE())),                     -- Ortho
(5, 5, 'Family medicine and regular checkups.', 80.00, 1, DATEADD(YEAR, -1, GETDATE()));          -- GP
GO

-- ==============================================================================================
-- 4. Insert Patients 
-- Linking People (6-10) with Insurance info
-- ==============================================================================================
PRINT 'Seeding Patients...';
INSERT INTO [dbo].[Patients] ([PatientID], [InsuranceProvider], [InsurancePolicyNumber], [EmergencyContactName], [EmergencyContactPhone]) VALUES 
(6, 'BlueCross', 'POL-1001', 'Mad Hatter', '555-0901'),
(7, NULL, NULL, 'Wendy', '555-0902'), -- Cash Patient
(8, 'Aetna', 'POL-1002', 'Oona ONeill', '555-0903'),
(9, 'MetLife', 'POL-1003', 'Hippolyta', '555-0904'),
(10, 'Cigna', 'POL-1004', 'Julia Meade', '555-0905');
GO

-- ==============================================================================================
-- 5. Insert Users 
-- Enums: Role -> Admin=1, Doctor=2, Receptionist=3
-- ==============================================================================================
PRINT 'Seeding Users...';
INSERT INTO [dbo].[Users] ([UserID], [Username], [PasswordHash], [Role], [IsActive]) VALUES 
(11, 'admin', 'hashed_secret_password_123', 1, 1),      -- Admin
(1, 'dr.john', 'hashed_secret_password_456', 2, 1),     -- Doctor User
(12, 'reception', 'hashed_secret_password_789', 3, 1);  -- Receptionist
GO

-- ==============================================================================================
-- 6. Insert Appointments 
-- Enums: Status -> Pending=1, Confirmed=2, Completed=3, Canceled=4
-- ==============================================================================================
PRINT 'Seeding Appointments...';
SET IDENTITY_INSERT [dbo].[Appointments] ON;

INSERT INTO [dbo].[Appointments] 
([AppointmentID], [PatientID], [DoctorID], [AppointmentDate], [DurationMinutes], [Status], [ReasonForVisit], [CreatedBy]) VALUES 
-- Appt 1: Completed (Past) - Will be Paid
(1, 6, 1, DATEADD(DAY, -10, GETDATE()), 30, 3, 'Chest pain checkup', 12), 

-- Appt 2: Completed (Past) - Will be Partially Paid
(2, 7, 5, DATEADD(DAY, -5, GETDATE()), 30, 3, 'Seasonal Flu', 12),

-- Appt 3: Confirmed (Future) - Issued Invoice
(3, 8, 3, DATEADD(DAY, 1, GETDATE()), 45, 2, 'Vaccination', 12),

-- Appt 4: Canceled (Past)
(4, 9, 2, DATEADD(DAY, -2, GETDATE()), 30, 4, 'Skin rash - Patient cancelled', 12),

-- Appt 5: Pending (Future) - Draft Invoice
(5, 10, 4, DATEADD(DAY, 5, GETDATE()), 60, 1, 'Knee pain consultation', 12);

SET IDENTITY_INSERT [dbo].[Appointments] OFF;
GO

-- ==============================================================================================
-- 7. Insert Medical Records 
-- Only for Completed appointments (Appt 1 & 2)
-- ==============================================================================================
PRINT 'Seeding Medical Records...';
INSERT INTO [dbo].[MedicalRecords] ([AppointmentID], [Diagnosis], [Prescription], [Notes]) VALUES 
(1, 'Mild Angina', 'Aspirin 81mg daily', 'Patient advised to reduce stress and monitor BP.'),
(2, 'Influenza A', 'Tamiflu, Rest, Fluids', 'Temperature 38.5C. Sick leave for 3 days granted.');
GO

-- ==============================================================================================
-- 8. Insert Invoices 
-- Enums: InvoiceStatus -> Draft=0, Issued=1, PartiallyPaid=2, Paid=3, Cancelled=4
-- ==============================================================================================
PRINT 'Seeding Invoices...';
SET IDENTITY_INSERT [dbo].[Invoices] ON;

INSERT INTO [dbo].[Invoices] 
([InvoiceID], [AppointmentID], [PatientID], [TotalAmount], [TaxAmount], [DiscountAmount], [InvoiceDate], [InvoiceStatus]) VALUES 
-- Inv 1 (For Appt 1): Paid (150 Fee + 50 Tests)
(1, 1, 6, 200.00, 20.00, 0.00, DATEADD(DAY, -10, GETDATE()), 3), 

-- Inv 2 (For Appt 2): Partially Paid (80 Fee)
(2, 2, 7, 80.00, 8.00, 0.00, DATEADD(DAY, -5, GETDATE()), 2), 

-- Inv 3 (For Appt 3): Issued (100 Fee)
(3, 3, 8, 100.00, 10.00, 5.00, DATEADD(DAY, -1, GETDATE()), 1), 

-- Inv 4 (For Appt 4): Cancelled (0 Fee usually, but record exists)
(4, 4, 9, 0.00, 0.00, 0.00, DATEADD(DAY, -2, GETDATE()), 4), 

-- Inv 5 (For Appt 5): Draft (180 Fee)
(5, 5, 10, 180.00, 18.00, 0.00, GETDATE(), 0);

SET IDENTITY_INSERT [dbo].[Invoices] OFF;
GO

-- ==============================================================================================
-- 9. Insert Invoice Items 
-- ==============================================================================================
PRINT 'Seeding Invoice Items...';
INSERT INTO [dbo].[InvoiceItems] ([InvoiceID], [ItemDescription], [UnitPrice], [Quantity]) VALUES 
-- For Invoice 1 (Cardio)
(1, 'Consultation Fee', 150.00, 1),
(1, 'ECG Test', 50.00, 1),

-- For Invoice 2 (GP)
(2, 'General Consultation', 80.00, 1),

-- For Invoice 3 (Peds)
(3, 'Pediatric Checkup', 100.00, 1),

-- For Invoice 5 (Ortho - Draft)
(5, 'Specialist Consultation', 180.00, 1);
GO

-- ==============================================================================================
-- 10. Insert Payments 
-- Enums: PaymentMethod -> Cash=1, Card=2, Insurance=3, BankTransfer=4
-- Only for Invoice 1 (Paid) and Invoice 2 (Partially Paid)
-- ==============================================================================================
PRINT 'Seeding Payments...';
INSERT INTO [dbo].[Payments] ([InvoiceID], [PaymentAmount], [PaymentDate], [PaymentMethod], [TransactionRef]) VALUES 
-- Full Payment for Invoice 1 (Total was 220 incl tax)
(1, 220.00, DATEADD(DAY, -10, GETDATE()), 2, 'TXN-123456789'), -- Card

-- Partial Payment for Invoice 2 (Total was 88 incl tax, paid 40)
(2, 40.00, DATEADD(DAY, -5, GETDATE()), 1, NULL); -- Cash
GO

PRINT '==================================================';
PRINT '      DATA SEEDING COMPLETED SUCCESSFULLY         ';
PRINT '==================================================';
