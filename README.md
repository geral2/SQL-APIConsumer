# SQL-APIConsumer

Welcome to SQL-APIConsumer project!. It's Database Project built in C# whose main purpose it's allow consuming API GET/POST methods on SQL Server through CLR generics stored procedures. Keep in mind that it was developed and tested on **SQL Server 2016 and later versions**. If you need to deploy in an older version read the **Built with** section.

![alt text](https://github.com/geral2/SQL-APIConsumer/blob/master/Diagram.png)

## Getting Started

The main procedure of this project is APICaller_Web_Extended, which can be used to call API of different web methods. It return an extended result including headers, and data of the server, within the result. In case we do need all these details, just need the results, instead we could use APICaller_WebMethod.

With these two extended procedures we are able to change the content-type, through the header parameter. 

1. **SqlInt32 APICaller_Web_Extended(SqlString httpMethod, SqlString URL, SqlString Headers, SqlString JsonBody)**
1. **SqlInt32 APICaller_WebMethod(SqlString httpMethod, SqlString URL, SqlString JsonBody)**

**APICaller_Web_Extended**

Below parameters received. This procedure return an integer depending on the execution. 0: Sucess. -1: Failed. 

|Parameter	|Description						|Posible Value  |Sample		
|    :---:     	|     :---:      					|:---           |:---           |
|@httpMethod   	| HTTP Method that would be call    			|GET, POST, PUT,DELETE,PATCH|'GET'|
|@URL		| URL intended to call					|Valid URL      |'https://www.routingnumbers.info/api/name.json?rn=122242597'|  
|@Headers 	| Header related to request, if needed. 		|''             |'[{"Name": "Content-Type", "Value" :"text/javascript; charset=utf-8" }]'|
|@JsonBody	| Json Body if needed. HTTP Get required a blank body	|''             |''|

Returned information related to HTTP Response by APICaller_Web_Extended:

|Parameter	|Description						|
|    :---:     	|:---      						|
|JsonResult   	| Result returned by API called				|
|ContentType	| Returned Content Type					|
|ServerName	| Server Name called					|
|StatusCode	| HTTP Status Code reponse. Sample: 200,404,500		|
|Description    | HTTP Status response. Sample: OK			|
|Json_Headers   | Header result 					|


### **Sample calling APICaller_Web_Extended: GET**

![alt text](https://github.com/geral2/SQL-APIConsumer/blob/release_v2.3/images/Web_GET_Extended_Result.png)

![alt text](https://github.com/geral2/SQL-APIConsumer/blob/release_v2.3/images/Web_GET_Extended_Query.png)

```
GO
DECLARE @httpMethod nvarchar(max)	 = 'GET'
DECLARE @URL nvarchar(max)			 = 'https://www.routingnumbers.info/api/name.json?rn=122242597'
DECLARE @Headers nvarchar(max)		 = '[{"Name": "Content-Type", "Value" :"text/javascript; charset=utf-8" }]';

DECLARE @JsonBody nvarchar(max)		 =  ''

Declare @ts as table
(
	Json_Result nvarchar(max),
	ContentType varchar(100),
	ServerName varchar(100),
	Statuscode varchar(100),
	Descripcion varchar(100),
	Json_Headers nvarchar(max)
)

DECLARE @i AS INT 
 
INSERT INTO @ts

EXECUTE @i =  [dbo].[APICaller_Web_Extended] 
			   @httpMethod
			  ,@URL
			  ,@Headers
			  ,@JsonBody

SELECT * FROM @ts

SELECT 
		[name]	
		,[rn]		
		,[message]	
		,[code]	
 FROM (
			SELECT Context = Json_Result 
			  from @ts
		)tb
	OUTER APPLY OPENJSON  (context)  
  WITH
    ( [name]		VARCHAR(20) '$.name'
	, [rn]			VARCHAR(20) '$.rn'
	, [message]		VARCHAR(20) '$.message'
	, [code]		INT			'$.code'
    );

SELECT  * 
 FROM OPENJSON((select Json_Headers from @ts))  
			WITH (   
					 Header		NVARCHAR(MAX) '$."Name"'      
					,Value		NVARCHAR(MAX) '$."Value"'      
					) a
```

### **Sample calling APICaller_Web_Extended: POST**

![alt text](https://github.com/geral2/SQL-APIConsumer/blob/release_v2.3/images/POST_Extended_Result.png)

![alt text](https://github.com/geral2/SQL-APIConsumer/blob/release_v2.3/images/POST_Extended_query.png)

```
GO
DECLARE @httpMethod nvarchar(max)	 = 'POST'
DECLARE @URL nvarchar(max)			 = 'https://url-shortener-service.p.rapidapi.com/shorten'
DECLARE @Headers nvarchar(max)		 =  '[{ "Name": "Content-Type", "Value" :"application/x-www-form-urlencoded" }
										 ,{ "Name": "X-RapidAPI-Host","Value" :"url-shortener-service.p.rapidapi.com"}
										 ,{ "Name": "X-RapidAPI-Key", "Value" :"c56b333d25mshdbfec15f02f096ep19fa94jsne5189032cf7d"}
										 ,{"Name": "useQueryString","Value" :"true"}]';

DECLARE @JsonBody nvarchar(max)		 =  'url=https://www.linkedin.com/in/geraldo-diaz/'

Declare @ts as table
(
	Json_Result  NVARCHAR(MAX),
	ContentType  VARCHAR(100),
	ServerName   VARCHAR(100),
	Statuscode   VARCHAR(100),
	Descripcion  VARCHAR(100),
	Json_Headers NVARCHAR(MAX)
)

DECLARE @i AS INT 
 
INSERT INTO @ts
EXECUTE @i =  [dbo].[APICaller_Web_Extended] 
			   @httpMethod
			  ,@URL
			  ,@Headers
			  ,@JsonBody

 SELECT * FROM @ts

SELECT 
		Result = [name]	
 FROM (
			SELECT Context = Json_Result 
			  from @ts
		)tb
	OUTER APPLY OPENJSON  (context)  
  WITH
    ( [name]		VARCHAR(20) '$.result_url' );

SELECT  * 
 FROM OPENJSON((select Json_Headers from @ts))  
			WITH (   
					 Header		NVARCHAR(MAX) '$."Name"'      
					,Value		NVARCHAR(MAX) '$."Value"'      
					) a
```

Initially the procedures below were the main objects of this project, but these were deprecated due the generic webmethod above:

1. **APICaller_GET(SqlString URL)** 
1. **APICaller_POST(SqlString URL, SqlString JsonBody)**

The same also support Authentications header like Token or JWT (Deprecated).

1. **APICaller_GETAuth(SqlString URL, SqlString Authorization)**
1. **APICaller_POSTAuth(SqlString URL, SqlString Authorization, SqlString JsonBody)**
(More info in the wiki)

It even support sending multiples headers in a Json Format (Deprecated).

1. **APICaller_GET_headers(SqlString URL, SqlString Headers)**
1. **APICaller_POST_headers(SqlString URL, SqlString Headers)**
1. **APICaller_GET_JsonBody_Header(SqlString URL, SqlString Headers, SqlString JsonBody)**
1. **APICaller_POST_JsonBody_Header(SqlString URL, SqlString Headers, SqlString JsonBody)**

1. **APICaller_POST_Encoded(SqlString URL, SqlString Headers, SqlString JsonBody)**
This new procedure is exclusive for Calling API with enconded contentType (application/x-www-form-urlencoded).

1. **APICaller_GET_Extended(SqlString URL, SqlString Headers, SqlString JsonBody)**
1. **APICaller_POST_Extended(SqlString URL, SqlString Headers, SqlString JsonBody)**

There are a few Utilities functions;

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
GO
	PRINT N'Creating [dbo].[APICaller_WebMethod]...';
GO

CREATE PROCEDURE [dbo].[APICaller_WebMethod]
@httpMethod NVARCHAR (MAX) NULL, @URL NVARCHAR (MAX) NULL, @JsonBody NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_WebMethod]


GO
	PRINT N'Creating [dbo].[APICaller_Web_Extended]...';
GO

CREATE PROCEDURE [dbo].[APICaller_Web_Extended]
@httpMethod NVARCHAR (MAX) NULL, @URL NVARCHAR (MAX) NULL, @Headers NVARCHAR (MAX) NULL, @JsonBody NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_Web_Extended]

GO

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

PRINT N'Creating [dbo].[APICaller_GET_Headers_BODY]...';

GO

CREATE PROCEDURE [dbo].[APICaller_GET_Headers_BODY]
@URL NVARCHAR (MAX) NULL, @JsonBody NVARCHAR (MAX) NULL, @Headers NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].APICaller_GET_JsonBody_Header
GO

PRINT N'Creating [dbo].[APICaller_POST_Headers]...';
GO
CREATE PROCEDURE [dbo].[APICaller_POST_Headers]
@URL NVARCHAR (MAX) NULL, @Headers NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].APICaller_POST_Headers

GO
PRINT N'Creating [dbo].[APICaller_POST_JsonBody_Header]...';
GO
CREATE PROCEDURE [dbo].[APICaller_POST_JsonBody_Header]
	@URL NVARCHAR (MAX)  
  , @Headers NVARCHAR (MAX)  
  , @jSON NVARCHAR (MAX)  
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].APICaller_POST_JsonBody_Headers

GO
PRINT N'Creating [dbo].[APICaller_GET_Extended]...';
GO
CREATE PROCEDURE [dbo].[APICaller_GET_Extended]
@URL NVARCHAR (MAX) NULL, @JsonBody NVARCHAR (MAX) NULL, @Headers NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_GET_Extended]

GO
PRINT N'Creating [dbo].[APICaller_POST_Extended]...';
GO
CREATE PROCEDURE [dbo].[APICaller_POST_Extended]
@URL NVARCHAR (MAX) NULL, @Headers NVARCHAR (MAX) NULL, @JsonBody NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].[APICaller_POST_Extended]

GO
PRINT N'Creating [dbo].[APICaller_POST_Encoded]...';
GO

CREATE PROCEDURE [dbo].APICaller_POST_Encoded
  @URL		NVARCHAR (MAX) NULL
, @Headers	NVARCHAR (MAX) NULL
, @JsonBody NVARCHAR (MAX) NULL
AS EXTERNAL NAME [API_Consumer].[StoredProcedures].APICaller_POST_Encoded

```
### **Sample calling Get Method**
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

### **Sample calling multiples headers Get Method**
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

### **Sample calling Get Header Method Extended**

```
 --Script sample execution Calling Rapid API.

--Set Header
Declare @header nvarchar(max) = 
  '[{
		"Name": "Content-Type",
		"Value" :"application/json; charset=utf-8"
	},
	{
		"Name": "X-RapidAPI-Host",
		"Value" :"restcountries-v1.p.rapidapi.com"
	},{
		"Name": "X-RapidAPI-Key",
		"Value" :"c56b333d25mshdbfec15f02f096ep19fa94jsne5189032cf7d"
	}]';
--Set URL
Declare @wurl varchar(max) = 'https://restcountries-v1.p.rapidapi.com/all' 

Declare @ts as table
(
	Json_Result nvarchar(max),
	ContentType varchar(100),
	ServerName varchar(100),
	Statuscode varchar(100),
	Descripcion varchar(100),
	Json_Headers nvarchar(max)
)
declare @i as int 
 
 insert into @ts
 --Get Account Data
 exec @i = [dbo].[APICaller_GET_Extended]
							@wurl
							,''
							,@header

select * from @ts

SELECT  * 
 FROM OPENJSON((select Json_Result from @ts))  
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

SELECT  * 
 FROM OPENJSON((select Json_Headers from @ts))  
		WITH (   
				 Header				nvarchar(max) '$."Name"'      
				,Value		nvarchar(max) '$."Value"'      
				) a
```
![Extended GET](https://user-images.githubusercontent.com/5836150/84343055-21835580-ab75-11ea-8353-e4178bdf85d2.png)


### **Sample calling Authentication Get/POST Method**
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
### **ADVICE**
There are an issue reported related to the GAC, after Windows install .Net Framework updates sometimes cause the error below;

```
Could not load file or assembly 'System.Runtime.Serialization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' or one of its dependencies. Assembly in host store has a different signature than assembly in GAC. (Exception from HRESULT: 0x80131050)
```
It can be fixed with the code below;

```
ALTER  ASSEMBLY [System.Runtime.Serialization] 
FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Runtime.Serialization.dll' 
WITH PERMISSION_SET = UNSAFE
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
