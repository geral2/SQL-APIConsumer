using SQLAPI_Consumer;
using System;
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
    public static void APICaller_GET (SqlString URL)
    {
        try
        {
            string Result = APIConsumer.GETMethod(URL.ToString());

            Helper.SendResultValue("Result", Result);

        }
        catch (Exception ex)
        {
            Helper.SendResultValue("Error", ex.Message.ToString());
        }
    }

    /// <summary>
    /// It's a generic procedure used to consume Api throught GET method.
    /// Returns the result as a varchar(max). Could be used to return Json.
    /// </summary>
    /// <param name="URL">Api GET Method</param>
    /// <param name="Token">Authorization Token</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void APICaller_GET_Auth(SqlString URL, SqlString Token)
    {
        try
        {
            string Result = APIConsumer.GETMethod(URL.ToString(), Token.ToString());

            Helper.SendResultValue("Result", Result);

        }
        catch (Exception ex)
        {
            Helper.SendResultValue("Error", ex.Message.ToString());
        }
    }
}
