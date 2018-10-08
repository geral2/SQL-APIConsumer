# SQL-APIConsumer
Welcome to SQL-APIConsumer project!. It's Database Project built in C# whose main purpose it's allow consuming API GET/POST methods on SQL Server through CLR generics stored procedures.

This project has two main procedures defined below:

1. **APICaller_GET(SqlString URL)**
1. **APICaller_POST(SqlString URL, SqlString JsonBody)**

THe same also support Authentications header like Token or JWT.

1. **APICaller_GETAuth(SqlString URL, SqlString Token)**
1. **APICaller_POSTAuth(SqlString URL, SqlString JsonBody, SqlString Token)**
(More info in the wiki)

It include 3rd one, that is basically an example of how to implement a customized method that return a result set based in a Data transfer object (DTO).

PD:
It uses HttpWebRequest instead of HttpClient in order to avoid having to use unsupported assemblies by SQL Server.

# How to...

### **Deployment Instruction**
###### **STEP 1**
`CREATE DATABASE TestDB;`
`GO`
###### **STEP 2**
`USE TestDB`
`GO`
`sp_configure 'clr enabled',1`
`RECONFIGURE`
`GO`
###### **STEP 3**
`ALTER DATABASE TESTDB SET TRUSTWORTHY ON`
`GO`
###### **STEP 4**
`CREATE ASSEMBLY [System.Runtime.Serialization]`
`AUTHORIZATION	dbo`
`FROM  N'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Runtime.Serialization.dll'`
`WITH PERMISSION_SET = UNSAFE--external_access`
`GO`
###### **STEP 5**
`CREATE ASSEMBLY [Newtonsoft.Json]`
`AUTHORIZATION dbo`
`FROM  N'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Newtonsoft.Json.dll'`
`WITH PERMISSION_SET = UNSAFE`
 `go`
###### **STEP 6**
 
`CREATE ASSEMBLY [API_Consumer]`
`AUTHORIZATION dbo`
`FROM  N'C:\CLR\API_Consumer.dll'`
`WITH PERMISSION_SET = UNSAFE`

`GO`
###### **STEP 7**
`PRINT N'Creating [dbo].[APICaller_GET_Json]...';`
`GO`
`CREATE PROCEDURE [dbo].[APICaller_GET_Json]`
`@URL NVARCHAR (MAX) NULL`
`AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_GET_Json]`

### **Sample of calling Get Method**
-- How to consume GET API
-- How to show Json results.

`DECLARE @RoutingNumber AS VARCHAR(50) = '122242597'`

`--Public API: routingnumbers.info`
`DECLARE @Url  VARCHAR(200) = CONCAT('https://www.routingnumbers.info/api/name.json?','rn=',@RoutingNumber) `

`DECLARE @Results AS TABLE`
`(`
	`Context varchar(max)`
`)`

`DECLARE @Result AS VARCHAR(MAX)`

`INSERT INTO @Results`
`EXEC  [dbo].[APICaller_GET_Json] @Url`

`--Result: Row per value `

 `SELECT  B.*`
  `FROM (`
			`SELECT Context `
			  `from @Results`
		`)tb`
	`OUTER APPLY OPENJSON  (context) B`

`--Result: column per value.`
`SELECT `
		`[name]	`
		`,[rn]		`
		`,[message]	`
		`,[code]	`
 `FROM (`
			`SELECT Context `
			  `from @Results`
		`)tb`
	`OUTER APPLY OPENJSON  (context)  `
  `WITH`
    `( [name]		VARCHAR(20) '$.name'`
	`, [rn]			VARCHAR(20) '$.rn'`
	`, [message]		VARCHAR(20) '$.message'`
	`, [code]		INT			'$.code'`
    `);`

### **Sample of calling Authentication Get/POST Method**

    DECLARE @Result AS TABLE
    (
        Token VARCHAR(MAX)
    )
    
    INSERT INTO @Result
    
     exec  [dbo].[APICaller_POST]
    	 @URL = 'http://localhost:5000/api/auth/login'
    	,@BodyJson = '{"Username":"gdiaz","Password":"password"}'
    
    DECLARE @Token AS VARCHAR(MAX)
    
    SELECT TOP 1 @Token = CONCAT('Bearer ',Json.Token)
     FROM @Result
      CROSS APPLY ( SELECT value AS Token FROM OPENJSON(Result)) AS [Json]
    
    EXEC [dbo].[APICaller_GETAuth] 
         @URL	  = 'http://localhost:5000/api/values'
       , @Token = @Token
