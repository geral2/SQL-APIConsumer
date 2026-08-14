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
    /// <param name="Headers">Authorization Header</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static SqlInt32 GraphQLCaller(SqlString URL, SqlString Headers, SqlString query, SqlString variablesJson)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;
        try
        {
            string Result = APIConsumer.GraphQL(URL.ToString(), query.ToString(), variablesJson.ToString(), Headers.ToString());

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
