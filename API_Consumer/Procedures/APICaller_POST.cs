using SQLAPI_Consumer;
using System;
using System.Data.SqlTypes;

/// <summary>
/// Generic Post Api Consumer thought CLR Proc
/// </summary>
public partial class StoredProcedures
{
    /// <summary>
    /// It's a generic procedure used to consume Api throught POST method.
    /// It could either returns a result or not.
    /// </summary>
    /// <param name="URL">Consumer POST Method of Api</param>
    /// <param name="JsonBody">Json to be sent as body</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 APICaller_POST(SqlString URL, SqlString JsonBody)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;
        try
        {
            string Result = APIConsumer.POSTMethod(URL.ToString(), JsonBody.ToString());

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
    /// It's a generic procedure used to consume Api throught POST method.
    /// It could either returns a result or not.
    /// </summary>
    /// <param name="URL">Consumer POST Method of Api</param>
    /// <param name="Authorization">Authorization Header</param>
    /// <param name="JsonBody">Json to be sent as body</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 APICaller_POST_Auth(SqlString URL, SqlString Authorization, SqlString JsonBody)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;
        try
        {
            string Result = APIConsumer.POSTMethod(URL.ToString(), JsonBody.ToString(), Authorization.ToString());

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
    /// It's a generic procedure used to consume Api throught POST method.
    /// It could either returns a result or not.
    /// </summary>
    /// <param name="URL">Consumer POST Method of Api</param>
    /// <param name="Headers">Authorization Header</param>
    /// <param name="JsonBody">Json to be sent as body</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 APICaller_POST_JsonBody_Headers(SqlString URL, SqlString Headers, SqlString JsonBody)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;

        try
        {
            string Result = APIConsumer.POSTMethod_Header(URL.ToString(), JsonBody.ToString(), Headers.ToString());

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
    /// It's a generic procedure used to consume Api throught POST method.
    /// It could either returns a result or not.
    /// </summary>
    /// <param name="URL">Consumer POST Method of Api</param>
    /// <param name="Headers">Authorization Header</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 APICaller_POST_Headers(SqlString URL, SqlString Headers)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;
        try
        {
            string Result = APIConsumer.POSTMethod_Header(URL.ToString(), Headers.ToString());

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
    /// It's a generic procedure used to consume API throught POST method.
    /// It could either returns a result or not.
    /// Content Type must be suplied as headers
    /// </summary>
    /// <param name="URL">Consumer POST Method of Api</param>
    /// <param name="Headers">Authorization Header</param>
    /// <param name="JsonBody">Json to be sent as body</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 APICaller_POST_Extended(SqlString URL, SqlString Headers, SqlString JsonBody)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;
        try
        {
            API_Consumer.ExtendedResult ExtResult = new API_Consumer.ExtendedResult();

            string Result = APIConsumer.POSTMethod_Extended(ref ExtResult, URL.ToString(), JsonBody.ToString(),  Headers.ToString());

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
    /// It's a generic procedure used to consume Api throught POST method.
    /// It could either returns a result or not.
    /// Content type application/x-www-form-urlencoded
    /// </summary>
    /// <param name="URL">Consumer POST Method of Api</param>
    /// <param name="Headers">Authorization Header</param>
    /// <param name="JsonBody">Json to be sent as body</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 APICaller_POST_Encoded(SqlString URL, SqlString Headers, SqlString JsonBody)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;
        try
        {
            string Result = APIConsumer.POSTMethod_urlencoded(URL.ToString(), JsonBody.ToString(), Headers.ToString());

            Helper.SendResultValue(APIConsumer.DEFAULT_COLUMN_RESULT, Result);

        }
        catch (Exception ex)
        {
            Helper.SendResultValue(APIConsumer.DEFAULT_COLUMN_ERROR, ex.Message.ToString());
            ExecutionResult = APIConsumer.FAILED_EXECUTION_RESULT;
        }

        return ExecutionResult;
    }
}
