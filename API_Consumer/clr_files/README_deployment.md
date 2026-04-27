# SQL-APIConsumer Deployment Guide

This guide provides multiple options for deploying the SQL-APIConsumer CLR assembly to multiple SQL Server databases.

## Prerequisites

1. SQL Server with CLR integration enabled
2. API_Consumer.dll compiled and available
3. Administrative privileges on SQL Server
4. .NET Framework (the assembly uses merged dependencies via ILRepack)

## Deployment Options

### Option 1: Single Database Script (Recommended)
**File**: `deploy_single_db.sql`

This is the simplest and most reliable approach. Run the script once per database by changing the `@target_db` variable.

**How to use**:
1. Open `deploy_single_db.sql`
2. Edit the configuration section at the top:
   ```sql
   USE [db1]; -- Change this
   DECLARE @target_db NVARCHAR(128) = N'db1';  -- Change this
   DECLARE @dll_path NVARCHAR(260) = N'C:\CLR\API_Consumer.dll';  -- Set your path
   ```
3. Run the script in SQL Server Management Studio
4. Repeat for each target database by changing `@target_db`

**Advantages**:
- Simple and reliable
- Easy to debug issues
- Works with any SQL client
- No external dependencies

### Option 2: PowerShell Automation
**File**: `deploy_multiple.ps1`

Automates deployment to multiple databases using PowerShell.

**Prerequisites**:
- PowerShell 5.0 or later
- SqlServer PowerShell module: `Install-Module -Name SqlServer`

**How to use**:
```powershell
# Basic usage with default settings
.\deploy_multiple.ps1

# Custom server and databases
.\deploy_multiple.ps1 -ServerInstance "MyServer\Instance" -TargetDatabases @("db1", "db2", "db3")

# Custom DLL path
.\deploy_multiple.ps1 -DllPath "D:\MyPath\API_Consumer.dll"
```

**Advantages**:
- Fully automated
- Parallel execution possible
- Detailed error reporting
- Professional logging

### Option 3: Basic Assembly Registration
**File**: `register_assembly.sql`

A simple script that only registers the assembly without creating procedures/functions.

**How to use**:
1. Edit the DLL path in the script
2. Run the script to register the assembly
3. Use the individual CREATE PROCEDURE/FUNCTION statements from the README

## Configuration

### DLL Path
Ensure your `API_Consumer.dll` is accessible to SQL Server:
- Default location: `C:\CLR\API_Consumer.dll`
- Must be accessible by SQL Server service account
- Consider using a shared network path for multiple servers

### Target Databases
Edit the database lists in each script:
- **Single script**: Change `@target_db` variable
- **PowerShell**: Modify `$TargetDatabases` array

### Server Configuration
The scripts will automatically:
- Enable CLR integration
- Add assemblies to trusted assemblies list (SQL Server 2017+)
- Create required assembly and all procedures/functions

## Functions and Procedures Created

Each deployment creates these 16 objects:

### Core Procedures (Recommended):
1. `dbo.APICaller_WebMethod` - Generic web method caller
2. `dbo.APICaller_Web_Extended` - Extended web method with headers and response details

### Specific HTTP Method Procedures:
3. `dbo.APICaller_GET` - Simple GET requests
4. `dbo.APICaller_POST` - Simple POST requests
5. `dbo.APICaller_POSTAuth` - POST with authentication token
6. `dbo.APICaller_GETAuth` - GET with authentication token

### Header-Enabled Procedures:
7. `dbo.APICaller_GET_Headers` - GET with custom headers
8. `dbo.APICaller_GET_Headers_BODY` - GET with headers and body
9. `dbo.APICaller_POST_Headers` - POST with custom headers
10. `dbo.APICaller_POST_JsonBody_Header` - POST with JSON body and headers

### Extended Procedures:
11. `dbo.APICaller_GET_Extended` - GET with extended response details
12. `dbo.APICaller_POST_Extended` - POST with extended response details
13. `dbo.APICaller_POST_Encoded` - POST with URL-encoded content

### Utility Functions:
14. `dbo.Create_HMACSHA256` - Create HMAC-SHA256 hash
15. `dbo.GetTimestamp` - Get current timestamp
16. `dbo.fn_GetBytes` - Convert string to bytes representation

## Troubleshooting

### Common Issues

1. **Assembly dependency errors**:
   - This version uses ILRepack to merge all dependencies
   - If you see dependency errors, ensure you're using the merged DLL

2. **Permission errors**:
   - Run with SQL Server administrator privileges
   - Ensure service account can access DLL file

3. **CLR not enabled**:
   - Scripts automatically enable CLR integration
   - May require server restart in some cases

4. **Trust assembly errors**:
   - Scripts automatically add assemblies to trusted list (SQL Server 2017+)
   - For older versions, remove the trusted assembly section

### Legacy Version Support

If you're using the older version with separate assemblies:
- Use `install_assembly.sql` for multiple database deployment
- Ensure all dependency DLLs are available
- See original README.md for detailed legacy instructions

### Verification

After deployment, verify success:
```sql
-- Check assembly
SELECT name, permission_set_desc 
FROM sys.assemblies 
WHERE name = 'API_Consumer';

-- Check procedures
SELECT name, type_desc 
FROM sys.objects 
WHERE type = 'P' 
AND name LIKE '%APICaller%';

-- Check functions
SELECT name, type_desc 
FROM sys.objects 
WHERE type = 'FN' 
AND name IN ('Create_HMACSHA256', 'GetTimestamp', 'fn_GetBytes');
```

## Usage Examples

### Basic GET Request:
```sql
DECLARE @result AS TABLE (Context varchar(max))
INSERT INTO @result
EXEC dbo.APICaller_GET 'https://api.example.com/data'
SELECT * FROM @result;
```

### Extended POST with Headers:
```sql
DECLARE @results AS TABLE (
    Json_Result nvarchar(max),
    ContentType varchar(100),
    ServerName varchar(100),
    Statuscode varchar(100),
    Descripcion varchar(100),
    Json_Headers nvarchar(max)
)

INSERT INTO @results
EXEC dbo.APICaller_Web_Extended 
    'POST',
    'https://api.example.com/submit',
    '[{"Name": "Content-Type", "Value": "application/json"}]',
    '{"key": "value"}';
    
SELECT * FROM @results;
```

## Choosing the Right Option

- **New users / Simple deployment**: Use Option 1 (Single Database Script)
- **Multiple databases / Automation**: Use Option 2 (PowerShell)
- **Assembly registration only**: Use Option 3 (Basic Registration)
- **Legacy installations**: Use `install_assembly.sql`

## Support

If you encounter issues:
1. Check SQL Server error logs
2. Verify DLL accessibility and integrity
3. Ensure proper permissions
4. Test with a single database first
5. Review the main README.md for usage examples 