-- Stored Procedures for HousingDB
-- Run this script to create all required stored procedures

-- 1. Add Representative
CREATE OR ALTER PROCEDURE dbo.sp_AddRepresentative
    @name NVARCHAR(100),
    @email NVARCHAR(100),
    @phone NVARCHAR(20),
    @license NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.representative (repres_name, email, phone, license_no)
    VALUES (@name, @email, @phone, @license);
END;
GO

-- 2. Add Client
CREATE OR ALTER PROCEDURE dbo.sp_AddClient
    @name NVARCHAR(100),
    @phone NVARCHAR(20),
    @email NVARCHAR(100),
    @nationalId NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.client (name, phone, email, national_id)
    VALUES (@name, @phone, @email, @nationalId);
END;
GO

-- 3. Add Housing Unit
CREATE OR ALTER PROCEDURE dbo.sp_AddHousingUnit
    @style NVARCHAR(100),
    @longitude DECIMAL(10, 8),
    @latitude DECIMAL(10, 8),
    @valuation DECIMAL(18, 2),
    @represId INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.housing_unit (architectural_style, longitude, latitude, market_valuation, status, repres_id)
    VALUES (@style, @longitude, @latitude, @valuation, 'available', @represId);
END;
GO

-- 4. Update Unit Status
CREATE OR ALTER PROCEDURE dbo.sp_UpdateUnitStatus
    @unitId INT,
    @newStatus NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.housing_unit 
    SET status = @newStatus
    WHERE unit_id = @unitId;
END;
GO

-- Verify stored procedures creation
SELECT name, type_desc 
FROM sys.objects 
WHERE type = 'P' 
AND name LIKE 'sp_%'
ORDER BY name;
