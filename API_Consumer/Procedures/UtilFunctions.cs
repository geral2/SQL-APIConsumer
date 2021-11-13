using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using API_Consumer;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using SQLAPI_Consumer;

public partial class UserDefinedFunctions
{
    /// <summary>
    /// Get TimeStamp from server.
    /// </summary>
    /// <returns>string Timestamp</returns>
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GetTimestamp()
    {
        SqlString valueReturned = Helper.GetTimestamp();
        return valueReturned;
    }

    /// <summary>
    /// Get TimeStamp from server.
    /// </summary>
    /// <returns>string Timestamp</returns>
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString Create_HMACSHA256(SqlString message, SqlString SecretKey)
    {
        SqlString valueReturned = Helper.CreateSignature(
                                                          message.ToString()
                                                        , SecretKey.ToString()
                                                        );
        return valueReturned;
    }

    /// <summary>
    /// Get Enconde ASCII string.
    /// </summary>
    /// <returns>string enconded</returns>
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString fn_GetBytes(SqlString value)
    {
        SqlString valueReturned = Helper.GetBytes_Encoding(
                                                            ""
                                                           ,value.ToString()
                                                        );
        return valueReturned;
    }

    /// <summary>
    /// Get Enconde string ASCII-UTF8 from server.
    /// </summary>
    /// <returns>string enconded</returns>
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString fn_GetBytes_Ext(SqlString Enconde_type, SqlString value)
    {
        SqlString valueReturned = Helper.GetBytes_Encoding( 
                                                            Enconde_type.ToString(),
                                                            value.ToString()
                                                        );
        return valueReturned;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpMethod"></param>
    /// <param name="URL"></param>
    /// <param name="Headers"></param>
    /// <param name="JsonBody"></param>
    /// <returns></returns>
    [Microsoft.SqlServer.Server.SqlFunction(
       FillRowMethodName = "FillResult",
       TableDefinition   = "Result nvarchar(max), StatusCode nvarchar(50), StatusDescription nvarchar(200),ContentType nvarchar(200),Server nvarchar(200), Headers nvarchar(MAX)")]

    public static IEnumerable tvf_APICaller_Web_Extended(SqlString httpMethod, SqlString URL, SqlString Headers, SqlString JsonBody)
    {
        SqlInt32 ExecutionResult = APIConsumer.DEFAULT_EXECUTION_RESULT;
        API_Consumer.ExtendedResult ExtResult = new API_Consumer.ExtendedResult();
        try
        {
            ArrayList _webResults = new ArrayList();

            string Result = APIConsumer.WebMethod_Extended(ref ExtResult, httpMethod.ToString(), URL.ToString(), JsonBody.ToString(), Headers.ToString());

            _webResults.Add(new WebExtendedResult(    (SqlString) ExtResult.Result
                                                    , (SqlString) ExtResult.StatusCode
                                                    , (SqlString) ExtResult.StatusDescription
                                                    , (SqlString) ExtResult.ContentType
                                                    , (SqlString) ExtResult.Server
                                                    , (SqlString) JsonConvert.SerializeObject(ExtResult.headers)
                                                    ));
            return _webResults;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    //FillRow method. The method name has been specified above as a SqlFunction attribute property
    public static void FillResult(object objWebResult 
                                , out SqlString Result
                                , out SqlString StatusCode
                                , out SqlString StatusDescription
                                , out SqlString ContentType
                                , out SqlString Server
                                , out SqlString Headers
                                )
    {
        WebExtendedResult _webResult = (WebExtendedResult)objWebResult;
        Result = _webResult.Result;
        StatusCode = _webResult.StatusCode;
        StatusDescription = _webResult.StatusDescription;
        ContentType = _webResult.ContentType;
        Server = _webResult.Server;
        Headers = _webResult.Headers;
    }
}

