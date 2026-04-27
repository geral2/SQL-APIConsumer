-- SQL Server CLR Assembly Deployment Script - Single Database Version
-- SQL-APIConsumer
-- Run this script once per target database by changing the @target_db variable

-- =============================================
-- CONFIGURATION - CHANGE THESE VALUES
-- =============================================
USE [db1];
DECLARE @target_db NVARCHAR(128) = N'db1';  -- <<<< CHANGE THIS FOR EACH DATABASE
DECLARE @dll_path NVARCHAR(260) = N'C:\CLR\API_Consumer.dll';  -- <<<< SET YOUR PATH HERE

-- =============================================
-- Enable CLR (run once per instance)
-- =============================================
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'clr enabled', 1;
RECONFIGURE;

-- =============================================
-- Trust assemblies (run once per instance)
-- =============================================

-- Assembly configuration table
DECLARE @Assemblies TABLE (
    AssemblyName NVARCHAR(200),
    AssemblyPath NVARCHAR(500),
    Description NVARCHAR(500)
);

-- Define all assemblies to be trusted
INSERT INTO @Assemblies (AssemblyName, AssemblyPath, Description)
VALUES 
    ('API_Consumer', 
     @dll_path, 
     N'SQL-APIConsumer Assembly'),
    
    ('System.Runtime.Serialization', 
     'C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Runtime.Serialization\v4.0_4.0.0.0__b77a5c561934e089\System.Runtime.Serialization.dll', 
     N'System.Runtime.Serialization from GAC'),
    
    ('SMDiagnostics', 
     'C:\Windows\Microsoft.NET\assembly\GAC_MSIL\SMDiagnostics\v4.0_4.0.0.0__b77a5c561934e089\SMDiagnostics.dll', 
     N'SMDiagnostics from GAC'),
    
    ('System.ServiceModel.Internals', 
     'C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.ServiceModel.Internals\v4.0_4.0.0.0__31bf3856ad364e35\System.ServiceModel.Internals.dll', 
     N'System.ServiceModel.Internals from GAC');

-- Process each assembly
DECLARE @AssemblyName NVARCHAR(200);
DECLARE @AssemblyPath NVARCHAR(500);
DECLARE @Description NVARCHAR(500);
DECLARE @Hash VARBINARY(64);
DECLARE @ErrorMsg NVARCHAR(MAX);

DECLARE assembly_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT AssemblyName, AssemblyPath, Description
    FROM @Assemblies;

OPEN assembly_cursor;
FETCH NEXT FROM assembly_cursor INTO @AssemblyName, @AssemblyPath, @Description;

WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY
        -- Calculate hash for the assembly
        SET @Hash = NULL;
        
        DECLARE @SQL1 NVARCHAR(MAX) = N'
            SELECT @Hash = HASHBYTES(''SHA2_512'', BulkColumn)
            FROM OPENROWSET(BULK ''' + @AssemblyPath + ''', SINGLE_BLOB) AS x;';
        
        EXEC sp_executesql @SQL1, N'@Hash VARBINARY(64) OUTPUT', @Hash OUTPUT;
        -- Check if hash already exists
        IF @Hash IS NOT NULL
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM sys.trusted_assemblies WHERE [hash] = @Hash)
            BEGIN
                EXEC sys.sp_add_trusted_assembly @hash = @Hash, @description = @Description;
                PRINT @AssemblyName + ' assembly hash added to trusted assemblies.';
            END
            ELSE
            BEGIN
                PRINT @AssemblyName + ' assembly hash already exists in trusted assemblies.';
            END
        END
        ELSE
        BEGIN
            PRINT 'WARNING: Could not calculate hash for ' + @AssemblyName + '. File may not exist.';
        END
    END TRY
    BEGIN CATCH
        SET @ErrorMsg = 'Error processing ' + @AssemblyName + ': ' + ERROR_MESSAGE();
        PRINT @ErrorMsg;
    END CATCH;

    FETCH NEXT FROM assembly_cursor INTO @AssemblyName, @AssemblyPath, @Description;
END

CLOSE assembly_cursor;
DEALLOCATE assembly_cursor;

PRINT '';
PRINT 'Assembly trust configuration completed.';

-- Verify trusted assemblies
SELECT [description], 
       CONVERT(VARCHAR(MAX), [hash], 2) AS hash_hex,
       create_date
FROM sys.trusted_assemblies
WHERE [description] LIKE '%API_Consumer%' 
   OR [description] LIKE '%System.Runtime.Serialization%'
   OR [description] LIKE '%SMDiagnostics%'
   OR [description] LIKE '%System.ServiceModel.Internals%'
ORDER BY create_date DESC;

-- =============================================
-- Switch to target database
-- =============================================
DECLARE @sql NVARCHAR(MAX) = N'USE ' + QUOTENAME(@target_db);
EXEC(@sql);

PRINT 'Deploying to database: ' + @target_db;

-- =============================================
-- Clean up existing objects
-- =============================================

-- Drop existing stored procedures
IF OBJECT_ID('dbo.APICaller_WebMethod') IS NOT NULL DROP PROCEDURE dbo.APICaller_WebMethod;
IF OBJECT_ID('dbo.APICaller_Web_Extended') IS NOT NULL DROP PROCEDURE dbo.APICaller_Web_Extended;
IF OBJECT_ID('dbo.APICaller_GET') IS NOT NULL DROP PROCEDURE dbo.APICaller_GET;
IF OBJECT_ID('dbo.APICaller_POST') IS NOT NULL DROP PROCEDURE dbo.APICaller_POST;
IF OBJECT_ID('dbo.APICaller_POSTAuth') IS NOT NULL DROP PROCEDURE dbo.APICaller_POSTAuth;
IF OBJECT_ID('dbo.APICaller_GETAuth') IS NOT NULL DROP PROCEDURE dbo.APICaller_GETAuth;
IF OBJECT_ID('dbo.APICaller_GET_Headers') IS NOT NULL DROP PROCEDURE dbo.APICaller_GET_Headers;
IF OBJECT_ID('dbo.APICaller_GET_Headers_BODY') IS NOT NULL DROP PROCEDURE dbo.APICaller_GET_Headers_BODY;
IF OBJECT_ID('dbo.APICaller_POST_Headers') IS NOT NULL DROP PROCEDURE dbo.APICaller_POST_Headers;
IF OBJECT_ID('dbo.APICaller_POST_JsonBody_Header') IS NOT NULL DROP PROCEDURE dbo.APICaller_POST_JsonBody_Header;
IF OBJECT_ID('dbo.APICaller_GET_Extended') IS NOT NULL DROP PROCEDURE dbo.APICaller_GET_Extended;
IF OBJECT_ID('dbo.APICaller_POST_Extended') IS NOT NULL DROP PROCEDURE dbo.APICaller_POST_Extended;
IF OBJECT_ID('dbo.APICaller_POST_Encoded') IS NOT NULL DROP PROCEDURE dbo.APICaller_POST_Encoded;

-- Drop existing functions
IF OBJECT_ID('dbo.Create_HMACSHA256') IS NOT NULL DROP FUNCTION dbo.Create_HMACSHA256;
IF OBJECT_ID('dbo.GetTimestamp') IS NOT NULL DROP FUNCTION dbo.GetTimestamp;
IF OBJECT_ID('dbo.fn_GetBytes') IS NOT NULL DROP FUNCTION dbo.fn_GetBytes;

PRINT 'Dropped existing procedures and functions';

-- Drop existing assemblies
IF EXISTS (SELECT * FROM sys.assemblies WHERE name = 'API_Consumer') 
    DROP ASSEMBLY [API_Consumer];

PRINT 'Dropped existing assemblies';

-- =============================================
-- Create assembly
-- =============================================

-- Create API_Consumer assembly
SET @sql = N'CREATE ASSEMBLY [API_Consumer] 
FROM ''' + @dll_path + ''' 
WITH PERMISSION_SET = UNSAFE';
EXEC(@sql);
PRINT 'Created API_Consumer assembly';

-- =============================================
-- Create stored procedures
-- =============================================

PRINT 'Creating stored procedures...';
GO

-- APICaller_WebMethod
CREATE PROCEDURE dbo.APICaller_WebMethod
    @httpMethod NVARCHAR(MAX) NULL, 
    @URL NVARCHAR(MAX) NULL, 
    @JsonBody NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_WebMethod];
GO

-- APICaller_Web_Extended
CREATE PROCEDURE dbo.APICaller_Web_Extended
    @httpMethod NVARCHAR(MAX) NULL, 
    @URL NVARCHAR(MAX) NULL, 
    @Headers NVARCHAR(MAX) NULL, 
    @JsonBody NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_Web_Extended];
GO

-- APICaller_GET
CREATE PROCEDURE dbo.APICaller_GET
    @URL NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_GET];
GO

-- APICaller_POST
CREATE PROCEDURE dbo.APICaller_POST
    @URL NVARCHAR(MAX) NULL, 
    @JsonBody NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_POST];
GO

-- APICaller_POSTAuth
CREATE PROCEDURE dbo.APICaller_POSTAuth
    @URL NVARCHAR(MAX) NULL, 
    @Token NVARCHAR(MAX) NULL, 
    @JsonBody NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_POST_Auth];
GO

-- APICaller_GETAuth
CREATE PROCEDURE dbo.APICaller_GETAuth
    @URL NVARCHAR(MAX) NULL, 
    @Token NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_GET_Auth];
GO

-- APICaller_GET_Headers
CREATE PROCEDURE dbo.APICaller_GET_Headers
    @URL NVARCHAR(MAX) NULL, 
    @Headers NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_GET_Headers];
GO

-- APICaller_GET_Headers_BODY
CREATE PROCEDURE dbo.APICaller_GET_Headers_BODY
    @URL NVARCHAR(MAX) NULL, 
    @JsonBody NVARCHAR(MAX) NULL, 
    @Headers NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].APICaller_GET_JsonBody_Header;
GO

-- APICaller_POST_Headers
CREATE PROCEDURE dbo.APICaller_POST_Headers
    @URL NVARCHAR(MAX) NULL, 
    @Headers NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].APICaller_POST_Headers;
GO

-- APICaller_POST_JsonBody_Header
CREATE PROCEDURE dbo.APICaller_POST_JsonBody_Header
    @URL NVARCHAR(MAX), 
    @Headers NVARCHAR(MAX), 
    @jSON NVARCHAR(MAX)
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].APICaller_POST_JsonBody_Headers;
GO

-- APICaller_GET_Extended
CREATE PROCEDURE dbo.APICaller_GET_Extended
    @URL NVARCHAR(MAX) NULL, 
    @JsonBody NVARCHAR(MAX) NULL, 
    @Headers NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_GET_Extended];
GO

-- APICaller_POST_Extended
CREATE PROCEDURE dbo.APICaller_POST_Extended
    @URL NVARCHAR(MAX) NULL, 
    @Headers NVARCHAR(MAX) NULL, 
    @JsonBody NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_POST_Extended];
GO

-- APICaller_POST_Encoded
CREATE PROCEDURE dbo.APICaller_POST_Encoded
    @URL NVARCHAR(MAX) NULL, 
    @Headers NVARCHAR(MAX) NULL, 
    @JsonBody NVARCHAR(MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].APICaller_POST_Encoded;
GO

PRINT 'Stored procedures created';

-- =============================================
-- Create functions
-- =============================================

PRINT 'Creating functions...';
Go

-- Create_HMACSHA256
CREATE FUNCTION dbo.Create_HMACSHA256 
(
    @message NVARCHAR(MAX) NULL, 
    @SecretKey NVARCHAR(MAX) NULL
) 
RETURNS NVARCHAR(MAX) 
AS EXTERNAL NAME [API_Consumer].[UserDefinedFunctions].[Create_HMACSHA256];
GO

-- GetTimestamp
CREATE FUNCTION dbo.GetTimestamp() 
RETURNS NVARCHAR(MAX) 
AS EXTERNAL NAME [API_Consumer].[UserDefinedFunctions].[GetTimestamp];
GO

-- fn_GetBytes
CREATE FUNCTION dbo.fn_GetBytes 
(
    @value NVARCHAR(MAX) NULL
) 
RETURNS NVARCHAR(MAX) 
AS EXTERNAL NAME [API_Consumer].[UserDefinedFunctions].fn_GetBytes;
GO

PRINT 'Functions created';

PRINT 'Deployment completed for database: ' + DB_NAME(); 