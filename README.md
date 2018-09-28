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
