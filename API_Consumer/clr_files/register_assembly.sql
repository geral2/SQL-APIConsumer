-- SQL Server CLR Assembly Registration Script - Basic Version
-- SQL-APIConsumer

-- =============================================
-- CONFIGURATION - CHANGE THESE VALUES
-- =============================================
DECLARE @dll_path NVARCHAR(260) = N'C:\CLR\API_Consumer.dll';  -- <<<< SET YOUR PATH HERE

-- =============================================
-- Enable CLR (run once per instance)
-- =============================================
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'clr enabled', 1;
RECONFIGURE;

PRINT 'CLR integration enabled';

-- =============================================
-- Trust assemblies (run once per instance)
-- =============================================
USE [master];

-- Verify DLL exists before processing
IF NOT EXISTS (SELECT * FROM OPENROWSET(BULK 'C:\CLR\API_Consumer.dll', SINGLE_BLOB) AS x)
BEGIN
    PRINT 'ERROR: DLL file not found at: C:\CLR\API_Consumer.dll';
    RETURN;
END

-- Trust API_Consumer assembly
DECLARE @hash VARBINARY(64);
SELECT @hash = HASHBYTES('SHA2_512', BulkColumn)
FROM OPENROWSET(BULK 'C:\CLR\API_Consumer.dll', SINGLE_BLOB) AS x;

IF NOT EXISTS (SELECT * FROM sys.trusted_assemblies WHERE [hash] = @hash)
BEGIN
    EXEC sys.sp_add_trusted_assembly @hash = @hash, @description = N'SQL-APIConsumer Assembly';
    PRINT 'API_Consumer assembly hash added to trusted assemblies.';
END
ELSE
BEGIN
    PRINT 'API_Consumer assembly hash already exists in trusted assemblies.';
END

-- =============================================
-- Create assembly
-- =============================================

-- Drop existing assembly if it exists 
IF EXISTS (SELECT * FROM sys.assemblies WHERE name = 'API_Consumer') 
BEGIN 
    PRINT 'Dropping existing API_Consumer assembly...';
    DROP ASSEMBLY [API_Consumer];
END 

-- Create new assembly
DECLARE @sql NVARCHAR(MAX) = N'CREATE ASSEMBLY [API_Consumer] FROM ''' + @dll_path + ''' WITH PERMISSION_SET = UNSAFE';
EXEC(@sql);

PRINT 'API_Consumer assembly created successfully';

-- =============================================
-- Verify assembly creation
-- =============================================
IF EXISTS (SELECT * FROM sys.assemblies WHERE name = 'API_Consumer')
    PRINT 'SUCCESS: Assembly registration completed successfully';
ELSE
    PRINT 'ERROR: Assembly registration failed';
