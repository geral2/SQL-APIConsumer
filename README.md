# SQL-APIConsumer

Welcome to SQL-APIConsumer project!. It's Database Project built in C# whose main purpose it's allow consuming API GET/POST methods on SQL Server through CLR generics stored procedures. Keep in mind that it was developed and tested on **SQL Server 2016 and later versions**. If you need to deploy in an older version read the **Built with** section.

![alt text](https://github.com/geral2/SQL-APIConsumer/blob/master/Diagram.png)

## Getting Started

This project has two main procedures defined below:

1. **APICaller_GET(SqlString URL)**
1. **APICaller_POST(SqlString URL, SqlString JsonBody)**

The same also support Authentications header like Token or JWT.

1. **APICaller_GETAuth(SqlString URL, SqlString Authorization)**
1. **APICaller_POSTAuth(SqlString URL, SqlString JsonBody, SqlString Authorization)**
(More info in the wiki)

It even support sending multiples headers in a Json Format.

1. **APICaller_GET_headers(SqlString URL, SqlString Headers)**
1. **APICaller_POST_headers(SqlString URL, SqlString Headers)**
1. **APICaller_GET_JsonBody_Header(SqlString URL, SqlString Headers, SqlString JsonBody)**
1. **APICaller_POST_JsonBody_Header(SqlString URL, SqlString Headers, SqlString JsonBody)**

There are two Utilities methods or procedures;

1. **GetTimestamp**
1. **Create_HMACSHA256(SqlString value, SqlString Key)**
1. **fn_GetBytes(SqlString value)**

It include another one, that is basically an example of how to implement a customized method that return a result set based in a Data transfer object (DTO).

PD:
It uses HttpWebRequest instead of HttpClient in order to avoid having to use unsupported assemblies by SQL Server.

### Prerequisites

Before you deploy the CLR you should set up some configuration in your SQL instance.

###### **STEP 1**
Confirm that your have enable this option 'clr enabled'.

```
USE TestDB
GO
sp_configure 'clr enabled',1
RECONFIGURE
```
###### **STEP 2**
Set your database to TRUSTWORTHY mode on.

```
ALTER DATABASE TESTDB SET TRUSTWORTHY ON
```

###### **STEP 3**
Create Assembly System.Runtim.Serialization from .Net Framework. Confirm what version of .Net you have installed and modify the path below with the correct one.

```
CREATE ASSEMBLY [System.Runtime.Serialization]
AUTHORIZATION	dbo
FROM  N'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Runtime.Serialization.dll'
WITH PERMISSION_SET = UNSAFE--external_access
```
If you do not know the path where this dll is located. There are two ways you can get it deployed. 

1. You can take it from SQL-APIConsumer\API_Consumer\dll\
1. Try running the script with the binary attached in tag v2.0.

If neither of those steps works you can reach me out.

If you are getting the error msg 15404, you should try the script below first. After that runs Step 3 again. 

```
USE [TestDB]
GO
EXEC dbo.sp_changedbowner @loginame = N'sa', @map = false
GO
WITH PERMISSION_SET = UNSAFE--external_access
```
Error mentioned above:
Msg 15404, Level 16, State 11, Line 1
Could not obtain information about Windows NT group/user 'xxxuser', error code 0x534.
 

###### **STEP 4**
Create Assembly Newtonsoft.Json. If It doesn't exists you need to download and copy it in this path. Keep in mind that the compiled CLR of this project uses the version 11.0 of Newtonsoft. If you want to update it you would need to recompiled the code.

```
CREATE ASSEMBLY [Newtonsoft.Json]
AUTHORIZATION dbo
FROM  N'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Newtonsoft.Json.dll'
WITH PERMISSION_SET = UNSAFE
```
If you do not know the path where this dll is located or this command above doesn't work. You could try with attached script in Tag v2.0. 

###### **STEP 5**
Create a new folder named CLR to the following path "C:\" or any another desired path which you can get access throught SQL.
And copy the .dll below from ...\API_Consumer\bin\Debug:
1. API_Consumer.dll
1. Newtonsoft.Json.dll
1. System.Net.Http.dll  

```
C:\CLR
```
Keep in mind, if you used a different path you will also neeed to modify the script of Installing/STEP 1   

### Installing

Now we are ready to install (create) the clr objects of SQL-APIConsumer. Let's do it!.


###### **STEP 1**
First, Let's create our Assembly:

```
CREATE ASSEMBLY [API_Consumer]
AUTHORIZATION dbo
FROM  N'C:\CLR\API_Consumer.dll'
WITH PERMISSION_SET = UNSAFE
```

If you do not know the path where this dll is located or this command above doesn't work. You could try with attached version in tag 2.0; 
 
###### **STEP 2**
After that we can create our CLR Stored procedures:

```
PRINT N'Creating [dbo].[Create_HMACSHA256]...';

GO
CREATE FUNCTION [dbo].[Create_HMACSHA256]
(@message NVARCHAR (MAX) NULL, @SecretKey NVARCHAR (MAX) NULL)
RETURNS NVARCHAR (MAX)
AS
 EXTERNAL NAME [API_Consumer].[UserDefinedFunctions].[Create_HMACSHA256]

 GO
PRINT N'Creating [dbo].[GetTimestamp]...';

GO
CREATE FUNCTION [dbo].[GetTimestamp]
( )
RETURNS NVARCHAR (MAX)
AS
 EXTERNAL NAME [API_Consumer].[UserDefinedFunctions].[GetTimestamp]

GO
	PRINT N'Creating [dbo].[fn_GetBytes]...';
GO
CREATE FUNCTION [dbo].fn_GetBytes
(@value NVARCHAR (MAX) NULL )
RETURNS NVARCHAR (MAX)
AS
 EXTERNAL NAME [API_Consumer].[UserDefinedFunctions].fn_GetBytes
GO
```

```
PRINT N'Creating [dbo].[APICaller_GET]...';
GO
CREATE PROCEDURE [dbo].[APICaller_GET]
@URL NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_GET]
```

```
PRINT N'Creating [dbo].[APICaller_POST]...';
GO
CREATE PROCEDURE [dbo].[APICaller_POST]
@URL NVARCHAR (MAX) NULL
,@JsonBody	NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_POST]
```

```
PRINT N'Creating [dbo].[APICaller_POSTAuth]...';
GO
CREATE PROCEDURE [dbo].[APICaller_POSTAuth]
@URL NVARCHAR (MAX) NULL
,@Token NVARCHAR (MAX) NULL
,@JsonBody	NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_POST_Auth]
```

```
PRINT N'Creating [dbo].[APICaller_GETAuth]...';
GO
CREATE PROCEDURE [dbo].[APICaller_GETAuth]
@URL NVARCHAR (MAX) NULL
,@Token NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_GET_Auth]

GO

PRINT N'Creating [dbo].[APICaller_GET_Headers]...';

GO
CREATE PROCEDURE [dbo].[APICaller_GET_Headers]
@URL NVARCHAR (MAX) NULL, @Headers NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_GET_Headers]

GO

GO

PRINT N'Creating [dbo].[APICaller_GET_Headers_BODY]...';

CREATE PROCEDURE [dbo].[APICaller_GET_Headers_BODY]
@URL NVARCHAR (MAX) NULL, @JsonBody NVARCHAR (MAX) NULL, @Headers NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].APICaller_GET_JsonBody_Header
GO

PRINT N'Creating [dbo].[APICaller_POST_Headers]...';

GO
CREATE PROCEDURE [dbo].[APICaller_POST_Headers]
@URL NVARCHAR (MAX) NULL, @Headers NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].APICaller_POST_Headers


```
### **Sample of calling Get Method**
-- How to consume GET API
-- How to show Json results.

```
DECLARE @RoutingNumber AS VARCHAR(50) = '122242597'

--Public API: routingnumbers.info
DECLARE @Url  VARCHAR(200) = CONCAT('https://www.routingnumbers.info/api/name.json?','rn=',@RoutingNumber) 

DECLARE @Results AS TABLE
(
	Context varchar(max)
)

DECLARE @Result AS VARCHAR(MAX)

INSERT INTO @Results
EXEC  [dbo].[APICaller_GET] @Url

--Result: Row per value 

 SELECT  B.*
  FROM (
			SELECT Context 
			  from @Results
		)tb
	OUTER APPLY OPENJSON  (context) B

--Result: column per value.
SELECT 
		[name]	
		,[rn]		
		,[message]	
		,[code]	
 FROM (
			SELECT Context 
			  from @Results
		)tb
	OUTER APPLY OPENJSON  (context)  
  WITH
    ( [name]		VARCHAR(20) '$.name'
	, [rn]			VARCHAR(20) '$.rn'
	, [message]		VARCHAR(20) '$.message'
	, [code]		INT			'$.code'
    );
```

![alt text](https://github.com/geral2/SQL-APIConsumer/blob/master/APICaller_GET%20ResultSet.png)

### **Sample of calling multiples headers Get Method**
```
  use TESTER
go 
--Set Header
Declare @header nvarchar(max) = '[{
								  "Name": "X-RapidAPI-Host",
								  "Value" :"restcountries-v1.p.rapidapi.com"
								},{
								  "Name": "X-RapidAPI-Key",
								  "Value" :"c56b333d25mshdbfec15f02f096ep19fa94jsne5189032cf7d"
								}]';
--Set URL
Declare @wurl varchar(max) = 'https://restcountries-v1.p.rapidapi.com/all' 

Declare @ts as table(Json_Table nvarchar(max))

 insert into @ts
 --Get Account Data
 exec [dbo].APICaller_GET_headers
							@wurl
							,@header

SELECT  * 
 FROM OPENJSON((select * from @ts))  
		WITH (   
				 name				nvarchar(max) '$."name"'      
				,alpha2Code			nvarchar(max) '$."alpha2Code"'      
				,alpha3Code			nvarchar(max) '$."alpha3Code"'      
				,callingCodes		nvarchar(max) '$."callingCodes"'  as JSON       
				,capital			nvarchar(max) '$."capital"'      
				,region				nvarchar(max) '$."region"'      
				,subregion			nvarchar(max) '$."subregion"'  
				,timezones			nvarchar(max) '$."timezones"'	  as JSON     
				,population			nvarchar(max) '$."population"'      
				,"currencies"		nvarchar(max) '$."currencies"'	  as JSON 
				,languages			nvarchar(max) '$."languages"'	  as JSON         
				) a
	
```

![alt text](https://github.com/geral2/SQL-APIConsumer/blob/master/APICaller_GET_headers%20ResultSet.png)

### **Sample of calling Authentication Get/POST Method**
```
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
```

## Deployment

Make sure that the user on your SQL Server instance have grant access to CLR Folder where you stored the files.

## Built With

* [C#](https://www.microsoft.com/en-us/download/details.aspx?id=7029/) - CLR develop in C# in .net framework 4.5.
* [SQL Server 2016+](https://www.microsoft.com/es-es/sql-server//) - Could be deployed in SQL server 2016 or later.

If you are working in an older version like 2008 or 2012 you would need to keep this in mind:
1. SQL 2012 does not allow default parameters for CLR (Default parameter values for CLR types, nvarchar(max), varbinary(max), and xml are not supported.)
1. OPENJSON is not available for SQL 2012. You won't be able to use OPENJSON statement since it was introduced in SQL 2016. As alternative you could use this [function](https://www.red-gate.com/simple-talk/sql/t-sql-programming/consuming-json-strings-in-sql-server/).

## Contributing

Actually we don't have any code of conduct... do whatever you want.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [v1.0.0](https://github.com/geral2/SQL-APIConsumer/releases/tag/v1.0). 

## Authors

* **Geraldo Diaz** - *SQL Developer* - [geral2](https://github.com/geral2)

See also the list of [contributors](https://github.com/geral2/SQL-APIConsumer/projects/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/geral2/SQL-APIConsumer/blob/master/LICENSE) file for details

## Acknowledgments

* [readme-template](https://github.com/kingdomax/readme-template) 
