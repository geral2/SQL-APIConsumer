using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
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
}
