using SQLAPI_Consumer;
using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.SqlTypes;

/// <summary>
/// It's a sample procedure that returns specific result: Deserializing DTO.
/// </summary>
public partial class StoredProcedures
{
    /// <summary>
    /// Sample of consuming Get API returning specific results using DTO.
    /// </summary>
    /// <param name="URL">Api Get Method</param>
    /// <param name="rn">Routing number</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void GET_BankInfoBasicByRN (SqlString URL, SqlString rn)
    {
        try
        {
            var BankInfoResult = new BankInfoBasic();
            string Result = APIConsumer.GETMethod<BankInfoBasic>(URL.ToString(), rn.ToString(), ref BankInfoResult);

            var Header = new SqlMetaData[]
            {
                 new SqlMetaData(nameof(BankInfoResult.Code), SqlDbType.VarChar,100),
                 new SqlMetaData(nameof(BankInfoResult.Name), SqlDbType.VarChar,50),
                 new SqlMetaData(nameof(BankInfoResult.Message), SqlDbType.VarChar,100),
                new SqlMetaData(nameof(BankInfoResult.rn), SqlDbType.VarChar,100),
            };

            Helper.SendResults(Header
                            ,  BankInfoResult.Code
                            ,  BankInfoResult.Name
                            ,  BankInfoResult.Message
                            ,  BankInfoResult.rn
                            );

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message.ToString());
        }
    }
}
