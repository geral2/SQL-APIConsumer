# SQL-APIConsumer

Welcome to SQL-APIConsumer project!. It's Database Project built in C# whose main purpose it's allow consuming API GET/POST methods on SQL Server through CLR generics stored procedures.

## Getting Started

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
Create Assembly System.Runtim.Serialization from .Net Framework.

```
CREATE ASSEMBLY [System.Runtime.Serialization]
AUTHORIZATION	dbo
FROM  N'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Runtime.Serialization.dll'
WITH PERMISSION_SET = UNSAFE--external_access
```

###### **STEP 4**
Create Assembly Newtonsoft.Json. If It doesn't exists you need to download and copy it in this path.

```
CREATE ASSEMBLY [Newtonsoft.Json]
AUTHORIZATION dbo
FROM  N'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Newtonsoft.Json.dll'
WITH PERMISSION_SET = UNSAFE
```

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
###### **STEP 2**
After that we can create our CLR Stored procedures:

```
PRINT N'Creating [dbo].[APICaller_GET]...';
GO
CREATE PROCEDURE [dbo].[APICaller_GET]
@URL NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_GET_Json]
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
EXEC  [dbo].[APICaller_GET_Json] @Url

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
* [SQL Server 2008+](https://www.microsoft.com/es-es/sql-server//) - Could be deployed in SQL server 2008 or later.

## Contributing

Actually we don't have any code of conduct... do whatever you want.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [v1.0.0](https://github.com/geral2/SQL-APIConsumer/releases/tag/v1.0). 

## Authors

* **Geraldo Diaz** - *SQL Developer* - [geral2](https://github.com/geral2)

See also the list of [contributors](https://github.com/geral2/SQL-APIConsumer/projects/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* [readme-template](https://github.com/kingdomax/readme-template) 
