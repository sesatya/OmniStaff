-- SQL Server script to create the OmniStaff leave-management database and core schema
-- Save and run this in SSMS, Azure Data Studio, or via your deployment pipeline

-- Create database
-- Drop dependent objects if present so script is re-runnable (use with caution in production)
IF OBJECT_ID(N'dbo.LeaveRequestApprovals') IS NOT NULL DROP TABLE dbo.LeaveRequestApprovals;
IF OBJECT_ID(N'dbo.LeaveRequests') IS NOT NULL DROP TABLE dbo.LeaveRequests;
IF OBJECT_ID(N'dbo.LeaveBalances') IS NOT NULL DROP TABLE dbo.LeaveBalances;
IF OBJECT_ID(N'dbo.LeaveTypes') IS NOT NULL DROP TABLE dbo.LeaveTypes;
IF OBJECT_ID(N'dbo.LeaveRequestStatuses') IS NOT NULL DROP TABLE dbo.LeaveRequestStatuses;
IF OBJECT_ID(N'dbo.Employees') IS NOT NULL DROP TABLE dbo.Employees;
IF OBJECT_ID(N'dbo.Users') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID(N'dbo.Roles') IS NOT NULL DROP TABLE dbo.Roles;
IF OBJECT_ID(N'dbo.AuditLogs') IS NOT NULL DROP TABLE dbo.AuditLogs;

IF DB_ID(N'OmniStaff_Leave') IS NULL
BEGIN
	CREATE DATABASE OmniStaff_Leave;
END
GO

USE OmniStaff_Leave;
GO

-- Roles (Admin / Manager / Employee)
CREATE TABLE Roles (
	Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL UNIQUE,
	Description NVARCHAR(500) NULL,
	CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- Users (for auth linkage; integrate with your auth provider or ASP.NET Identity)
CREATE TABLE Users (
	Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
	UserName NVARCHAR(256) NOT NULL UNIQUE,
	Email NVARCHAR(256) NULL,
	PasswordHash NVARCHAR(MAX) NULL,
	IsActive BIT NOT NULL DEFAULT 1,
	CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- Employee profile (one-to-one with Users)
CREATE TABLE Employees (
	Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
	UserId UNIQUEIDENTIFIER NOT NULL UNIQUE,
	EmployeeNumber NVARCHAR(50) NULL UNIQUE,
	FirstName NVARCHAR(100) NOT NULL,
	LastName NVARCHAR(100) NOT NULL,
	ManagerId UNIQUEIDENTIFIER NULL,
	HireDate DATE NULL,
	Department NVARCHAR(200) NULL,
	CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	-- avoid cascade from Users -> Employees to prevent multiple cascade path issues
	CONSTRAINT FK_Employees_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ,
	CONSTRAINT FK_Employees_Manager FOREIGN KEY (ManagerId) REFERENCES dbo.Employees(Id)
);
CREATE INDEX IX_Employees_ManagerId ON Employees(ManagerId);
GO

-- Leave types (Vacation, Sick, etc.)
CREATE TABLE LeaveTypes (
	Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL UNIQUE,
	Description NVARCHAR(500) NULL,
	IsPaid BIT NOT NULL DEFAULT 1,
	CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- Leave balances per employee per year and type
CREATE TABLE LeaveBalances (
	Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
	EmployeeId UNIQUEIDENTIFIER NOT NULL,
	LeaveTypeId UNIQUEIDENTIFIER NOT NULL,
	Year INT NOT NULL,
	Entitlement DECIMAL(6,2) NOT NULL DEFAULT 0,
	Used DECIMAL(6,2) NOT NULL DEFAULT 0,
	UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	CONSTRAINT UQ_LeaveBalances_Employee_Type_Year UNIQUE (EmployeeId, LeaveTypeId, Year),
	-- avoid cascading deletes from Employees to LeaveBalances to prevent multiple cascade paths
	CONSTRAINT FK_LeaveBalances_Employee FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ,
	CONSTRAINT FK_LeaveBalances_LeaveType FOREIGN KEY (LeaveTypeId) REFERENCES LeaveTypes(Id) 
);
CREATE INDEX IX_LeaveBalances_EmployeeId ON LeaveBalances(EmployeeId);
GO

-- Leave request status lookup
CREATE TABLE LeaveRequestStatuses (
	Id TINYINT NOT NULL PRIMARY KEY,
	Name NVARCHAR(50) NOT NULL UNIQUE
);
GO
IF NOT EXISTS (SELECT 1 FROM LeaveRequestStatuses WHERE Id = 0)
INSERT INTO LeaveRequestStatuses (Id, Name) VALUES
(0, 'Pending'),
(1, 'Approved'),
(2, 'Rejected'),
(3, 'Cancelled');
GO

-- Leave requests
CREATE TABLE LeaveRequests (
	Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
	EmployeeId UNIQUEIDENTIFIER NOT NULL,
	LeaveTypeId UNIQUEIDENTIFIER NOT NULL,
	StartDate DATE NOT NULL,
	EndDate DATE NOT NULL,
	TotalDays DECIMAL(6,2) NOT NULL,
	Status TINYINT NOT NULL DEFAULT 0,
	ManagerId UNIQUEIDENTIFIER NULL,
	ManagerComment NVARCHAR(1000) NULL,
	CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	-- avoid cascading deletes from Employees to LeaveRequests to prevent multiple cascade paths
	CONSTRAINT FK_LeaveRequests_Employee FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
	CONSTRAINT FK_LeaveRequests_LeaveType FOREIGN KEY (LeaveTypeId) REFERENCES LeaveTypes(Id) ,
	CONSTRAINT FK_LeaveRequests_Status FOREIGN KEY (Status) REFERENCES LeaveRequestStatuses(Id),
	CONSTRAINT FK_LeaveRequests_Manager FOREIGN KEY (ManagerId) REFERENCES Employees(Id) ,
	CHECK (EndDate >= StartDate)
);
CREATE INDEX IX_LeaveRequests_EmployeeId ON LeaveRequests(EmployeeId);
CREATE INDEX IX_LeaveRequests_Status ON LeaveRequests(Status);
GO

-- Approval history / audit for requests
CREATE TABLE LeaveRequestApprovals (
	Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
	LeaveRequestId UNIQUEIDENTIFIER NOT NULL,
	ApproverEmployeeId UNIQUEIDENTIFIER NULL,
	ActionAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	Action TINYINT NOT NULL,
	Comment NVARCHAR(1000) NULL,
	CONSTRAINT FK_Approvals_Request FOREIGN KEY (LeaveRequestId) REFERENCES LeaveRequests(Id) ,
	-- Change approver FK to SET NULL on delete to provide soft cleanup
	CONSTRAINT FK_Approvals_Approver FOREIGN KEY (ApproverEmployeeId) REFERENCES Employees(Id) 
);
CREATE INDEX IX_Approvals_LeaveRequestId ON LeaveRequestApprovals(LeaveRequestId);
GO

-- Audit logs
CREATE TABLE AuditLogs (
	Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
	EntityName NVARCHAR(200) NOT NULL,
	EntityId UNIQUEIDENTIFIER NULL,
	Action NVARCHAR(50) NOT NULL,
	PerformedBy UNIQUEIDENTIFIER NULL,
	PerformedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	Details NVARCHAR(MAX) NULL
);
GO

-- Seed minimal roles
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
INSERT INTO Roles (Name, Description) VALUES
('Admin', 'System administrator with full access'),
('Manager', 'Manager who can approve/reject leaves'),
('Employee', 'Regular employee who can apply for leave');
GO

-- Seed leave types
IF NOT EXISTS (SELECT 1 FROM LeaveTypes WHERE Name = 'Vacation')
INSERT INTO LeaveTypes (Name, Description, IsPaid) VALUES
('Vacation', 'Paid vacation leave', 1),
('Sick', 'Sick leave', 1),
('Unpaid', 'Unpaid leave', 0);
GO

-- Optional view: employee leave summary
IF OBJECT_ID('vw_EmployeeLeaveSummary') IS NOT NULL
	DROP VIEW vw_EmployeeLeaveSummary;
GO
CREATE VIEW vw_EmployeeLeaveSummary AS
SELECT e.Id AS EmployeeId, e.FirstName, e.LastName, lt.Name AS LeaveType, lb.Year, lb.Entitlement, lb.Used, (lb.Entitlement - lb.Used) AS Remaining
FROM LeaveBalances lb
JOIN Employees e ON lb.EmployeeId = e.Id
JOIN LeaveTypes lt ON lb.LeaveTypeId = lt.Id;
GO

-- End of script
