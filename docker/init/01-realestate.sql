/* Linux / Docker-friendly schema (no Windows file paths). Idempotent. */
SET NOCOUNT ON;

IF DB_ID(N'RealEstateDB') IS NULL
BEGIN
    CREATE DATABASE RealEstateDB;
END
GO

USE RealEstateDB;
GO

IF OBJECT_ID(N'dbo.Agents', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Agents (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        FullName NVARCHAR(200) NOT NULL,
        LicenseNumber NVARCHAR(100) NULL,
        Email NVARCHAR(255) NULL,
        PhoneNumber NVARCHAR(50) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Clients', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clients (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        FullName NVARCHAR(200) NOT NULL,
        PhoneNumber NVARCHAR(50) NULL,
        Email NVARCHAR(255) NULL,
        Address NVARCHAR(500) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Properties', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Properties (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        Address NVARCHAR(500) NULL,
        Price DECIMAL(13, 2) NOT NULL,
        Bedrooms INT NULL,
        Bathrooms INT NULL,
        SquareFeet INT NULL
    );
END
ELSE IF COL_LENGTH(N'dbo.Properties', N'SquareFeet') IS NULL
BEGIN
    ALTER TABLE dbo.Properties ADD SquareFeet INT NULL;
END
GO

IF OBJECT_ID(N'dbo.PropertyTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PropertyTypes (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL
    );
    CREATE NONCLUSTERED INDEX IX_PropertyTypes_Name ON dbo.PropertyTypes (Name);
END
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        UserName NVARCHAR(255) NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL,
        FullName NVARCHAR(200) NULL,
        Role NVARCHAR(50) NOT NULL DEFAULT ('admin'),
        CreatedAt DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT UQ_Users_UserName UNIQUE (UserName)
    );
END
GO
