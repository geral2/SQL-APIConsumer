//using Microsoft.SqlServer.Server;
//using Newtonsoft.Json;
using SQLAPI_Consumer;
using System;
using System.Data;
using System.Data.SqlTypes;

/// <summary>
/// Generic Get Api Consumer thought CLR Proc
/// </summary>
public partial class StoredProcedures
{
    /// <summary>
    /// It's a generic procedure used to consume Api throught GET method.
    /// Returns the result as a varchar(max). Could be used to return Json.
    /// </summary>
    /// <param name="URL">Api GET Method</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 APICaller_GET (SqlString URL)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;

        try
        {
            string Result = APIConsumer.GETMethod(URL.ToString());

            Helper.SendResultValue(APIConsumer.DEFAULT_COLUMN_RESULT, Result);

        }
        catch (Exception ex)
        {
            Helper.SendResultValue(APIConsumer.DEFAULT_COLUMN_ERROR, ex.Message.ToString());
            ExecutionResult = APIConsumer.FAILED_EXECUTION_RESULT;
        }

        return ExecutionResult;
    }

    /// <summary>
    /// It's a generic procedure used to consume Api throught GET method.
    /// Returns the result as a varchar(max). Could be used to return Json.
    /// </summary>
    /// <param name="URL">Api GET Method</param>
    /// <param name="Authorization">Authorization Header</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 APICaller_GET_Auth(SqlString URL, SqlString Authorization)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;

        try
        {
            string Result = APIConsumer.GETMethod(URL.ToString(), Authorization.ToString());

            Helper.SendResultValue(APIConsumer.DEFAULT_COLUMN_RESULT, Result);

        }
        catch (Exception ex)
        {
            Helper.SendResultValue(APIConsumer.DEFAULT_COLUMN_ERROR, ex.Message.ToString());
            ExecutionResult = APIConsumer.FAILED_EXECUTION_RESULT;
        }

        return ExecutionResult;
    }

    /// <summary>
    /// It's a generic procedure used to consume Api throught GET method.
    /// Returns the result as a varchar(max). Could be used to return Json.
    /// </summary>
    /// <param name="URL">Api GET Method</param>
    /// <param name="Headers">Json Headers</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 APICaller_GET_Headers(SqlString URL, SqlString Headers)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;

        try
        {
            string Result = APIConsumer.GETMethod_Headers(URL.ToString(), Headers.ToString());

            Helper.SendResultValue(APIConsumer.DEFAULT_COLUMN_RESULT, Result);

        }
        catch (Exception ex)
        {
            Helper.SendResultValue(APIConsumer.DEFAULT_COLUMN_ERROR, ex.Message.ToString());
            ExecutionResult = APIConsumer.FAILED_EXECUTION_RESULT;
        }

        return ExecutionResult;
    }

    /// <summary>
    /// It's a generic procedure used to consume Api throught GET method.
    /// Returns the result as a varchar(max). Could be used to return Json.
    /// </summary>
    /// <param name="URL">Api GET Method</param>
    /// <param name="Headers">Json Headers</param>
    /// <param name="JsonBody">Json Body</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 APICaller_GET_JsonBody_Header(SqlString URL, SqlString Headers, SqlString JsonBody)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;
        try
        {
            string Result = APIConsumer.GETMethod_Headers(URL.ToString(),JsonBody.ToString(), Headers.ToString());

            Helper.SendResultValue(APIConsumer.DEFAULT_COLUMN_RESULT, Result);

        }
        catch (Exception ex)
        {
            Helper.SendResultValue(APIConsumer.DEFAULT_COLUMN_ERROR, ex.Message.ToString());
            ExecutionResult = APIConsumer.FAILED_EXECUTION_RESULT;
        }

        return ExecutionResult;
    }

    /// <summary>
    /// It's a generic procedure used to consume Api throught GET method.
    /// Returns the result as a varchar(max). Could be used to return Json.
    /// Content Type must be suplied as headers
    /// </summary>
    /// <param name="URL">Api GET Method</param>
    /// <param name="Headers">Json Headers</param>
    /// <param name="JsonBody">Json Body</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 APICaller_GET_Extended(SqlString URL,  SqlString JsonBody, SqlString Headers)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;
        API_Consumer.ExtendedResult ExtResult = new API_Consumer.ExtendedResult();

        try
        {
            string Result = APIConsumer.GETMethod_Extended(ref ExtResult, URL.ToString(), JsonBody.ToString(), Headers.ToString());

            Helper.SendResultValue(ExtResult);
            
        }
        catch (Exception ex)
        {
            Helper.SendResultValue(ExtResult);
            ExecutionResult = APIConsumer.FAILED_EXECUTION_RESULT;
        }

        return ExecutionResult;
    }
}
