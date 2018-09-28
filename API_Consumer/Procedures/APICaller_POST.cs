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
    public static void APICaller_POST(SqlString URL, SqlString JsonBody)
    {
        try
        {
            string Result = APIConsumer.POSTMethod(URL.ToString(), JsonBody.ToString());

            Helper.SendResultValue("Result", Result);

        }
        catch (Exception ex)
        {
            Helper.SendResultValue("Error", ex.Message.ToString());
        }
    }

    /// <summary>
    /// It's a generic procedure used to consume Api throught POST method.
    /// It could either returns a result or not.
    /// </summary>
    /// <param name="URL">Consumer POST Method of Api</param>
    /// <param name="Token">Authorization Token</param>
    /// <param name="JsonBody">Json to be sent as body</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void APICaller_POST_Auth(SqlString URL, SqlString Token, SqlString JsonBody)
    {
        try
        {
            string Result = APIConsumer.POSTMethod(URL.ToString(), JsonBody.ToString(), Token.ToString());

            Helper.SendResultValue("Result", Result);

        }
        catch (Exception ex)
        {
            Helper.SendResultValue("Error", ex.Message.ToString());
        }
    }
}
