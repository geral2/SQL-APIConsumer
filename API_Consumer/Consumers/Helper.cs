using API_Consumer;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace SQLAPI_Consumer
{
    /// <summary>
    /// Utility used for return result set to SQL.
    /// </summary>
    public  static class Helper
    {
        /// <summary>
        /// Static method used to Send multiple columns as result set thought Lists of string.
        /// </summary>
        /// <param name="Header">Columns names showed in SQL Result set</param>
        /// <param name="Data">Values to be sent</param>
        public static void SendResults(SqlMetaData[] Header, List<string> Data)
        {
            SqlDataRecord Record = new SqlDataRecord(Header);

            if (!SqlContext.Pipe.IsSendingResults)
                SqlContext.Pipe.SendResultsStart(Record);

            if (SqlContext.Pipe.IsSendingResults)
            {
                foreach (var item in Data)
                {
                    Record.SetValues(item);

                    SqlContext.Pipe.SendResultsRow(Record);
                }

                SqlContext.Pipe.SendResultsEnd();
            }
        }

        /// <summary>
        /// Static method used to Send multiples columns as result set thought multiples variables.
        /// </summary>
        /// <param name="Header">Columns names showed in SQL Result set </param>
        /// <param name="Data">Values to be sent</param>
        public static void SendResults(SqlMetaData[] Header, params object[] values)
        {
            SqlDataRecord Record = new SqlDataRecord(Header);

            if (!SqlContext.Pipe.IsSendingResults)
                SqlContext.Pipe.SendResultsStart(Record);

            if (SqlContext.Pipe.IsSendingResults)
            {
                Record.SetValues(values);

                SqlContext.Pipe.SendResultsRow(Record);

                SqlContext.Pipe.SendResultsEnd();
            }
        }

        /// <summary>
        /// Static method used to send and specific value as a result.
        /// </summary>
        /// <param name="ColumnName">Name of column showed in SQL Result set.</param>
        /// <param name="Value">Value to be sent.</param>
        public static void SendResultValue(string ColumnName, string Value)
        {
            SqlDataRecord Record = new SqlDataRecord(new SqlMetaData[] { new SqlMetaData(ColumnName, SqlDbType.VarChar, SqlMetaData.Max) });

            if (!SqlContext.Pipe.IsSendingResults)
                SqlContext.Pipe.SendResultsStart(Record);

            if (SqlContext.Pipe.IsSendingResults)
            {
                Record.SetValues(Value);

                SqlContext.Pipe.SendResultsRow(Record);

                SqlContext.Pipe.SendResultsEnd();
            }
        }

        /// <summary>
        /// Static method used to send and specific value as a result.
        /// </summary>
        /// <param name="ColumnName">Name of column showed in SQL Result set.</param>
        /// <param name="Value">Value to be sent.</param>
        public static void SendResultValue(ExtendedResult extResult)
        {
            var Header = new SqlMetaData[]
            {
                     new SqlMetaData(nameof(extResult.Result), SqlDbType.VarChar,SqlMetaData.Max),
                     new SqlMetaData(nameof(extResult.ContentType), SqlDbType.VarChar,100),
                     new SqlMetaData(nameof(extResult.Server), SqlDbType.VarChar,100),
                     new SqlMetaData(nameof(extResult.StatusCode), SqlDbType.VarChar,100),
                     new SqlMetaData(nameof(extResult.StatusDescription), SqlDbType.VarChar,100),
                     new SqlMetaData(nameof(extResult.headers), SqlDbType.VarChar,SqlMetaData.Max)
            };

            SqlDataRecord Record = new SqlDataRecord(Header);

            if (!SqlContext.Pipe.IsSendingResults)
                SqlContext.Pipe.SendResultsStart(Record);

            if (SqlContext.Pipe.IsSendingResults)
            {
                Record.SetValues(
                                 extResult.Result
                                , extResult.ContentType
                                , extResult.Server
                                , extResult.StatusCode
                                , extResult.StatusDescription
                                , JsonConvert.SerializeObject(extResult.headers)
                                );

                SqlContext.Pipe.SendResultsRow(Record);

                SqlContext.Pipe.SendResultsEnd();
            }
        }

        /// <summary>
        /// Static method used to send an empty Result to SQL.
        /// </summary>
        /// <param name="ColumnName">Name of column showed in SQL Result set.</param>
        public static void SendEmptyResult(string ColumnName)
        {
            SqlDataRecord Record = new SqlDataRecord(new SqlMetaData[] { new SqlMetaData(ColumnName, SqlDbType.VarChar, SqlMetaData.Max) });

            if (!SqlContext.Pipe.IsSendingResults)
                SqlContext.Pipe.SendResultsStart(Record);

            if (SqlContext.Pipe.IsSendingResults)
            {
                SqlContext.Pipe.SendResultsRow(Record);

                SqlContext.Pipe.SendResultsEnd();
            }
        }

        /// <summary>
        /// Static method used to send an empty ResultSet to SQL.
        /// </summary>
        /// <param name="ColumnNames">Set of columns to be showed in SQL Result set.</param>
        public static void SendEmptyResult(SqlMetaData[] Header)
        {
            SqlDataRecord Record = new SqlDataRecord(Header);

            if (!SqlContext.Pipe.IsSendingResults)
                SqlContext.Pipe.SendResultsStart(Record);

            if (SqlContext.Pipe.IsSendingResults)
            {
                SqlContext.Pipe.SendResultsRow(Record);

                SqlContext.Pipe.SendResultsEnd();
            }
        }

        private static readonly Encoding SignatureEncoding = Encoding.UTF8;

        /// <summary>
        /// public method to return that return SHA256
        /// </summary>
        /// <param name="message">parameters in URL</param>
        /// <param name="secret">SK</param>
        /// <returns>string SHA256</returns>
        public static string CreateSignature(string message, string secret)
        {

            byte[] keyBytes = SignatureEncoding.GetBytes(secret);
            byte[] messageBytes = SignatureEncoding.GetBytes(message);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);

            byte[] bytes = hmacsha256.ComputeHash(messageBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Timestamp for signature
        /// </summary>
        /// <returns>string</returns>
        public static string GetTimestamp()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timestamp  = (long)(DateTime.Now.ToUniversalTime() - epoch).TotalMilliseconds;
            return timestamp.ToString();
            //long milliseconds = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();  
            //return milliseconds.ToString();
        }

        /// <summary>
        /// Get string's array of bytes 
        /// </summary>
        /// <returns>Base64 string</returns>
        public static string GetBytes_Encoding(string _type, string _value)
        {
            string byteArray;

            if (_type == "UTF8")
            {
                byteArray = GetBytes_Encoding_UTF8(_value);
            }
            else
            {
                byteArray = GetBytes_Encoding_ASCII(_value);
            }

            return byteArray;
        }

        /// <summary>
        /// Get string's array of bytes  Encoded ASCII
        /// </summary>
        /// <returns>Base64 string</returns>
        public static string GetBytes_Encoding_ASCII(string _value)
        {
            var byteArray = Encoding.ASCII.GetBytes(_value);

            return Convert.ToBase64String(byteArray);
        }

        /// <summary>
        /// Get string's array of bytes Encoded UTF8
        /// </summary>
        /// <returns>Base64 string</returns>
        public static string GetBytes_Encoding_UTF8(string _value)
        {
            var byteArray = Encoding.UTF8.GetBytes(_value);

            return Convert.ToBase64String(byteArray);
        }
    }
}
