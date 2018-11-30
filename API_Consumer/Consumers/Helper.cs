using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

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

            SqlContext.Pipe.SendResultsStart(Record);

            if (SqlContext.Pipe.IsSendingResults)
            {
                Record.SetValues(Value);

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

            SqlContext.Pipe.SendResultsStart(Record);

            if (SqlContext.Pipe.IsSendingResults)
            {
                SqlContext.Pipe.SendResultsRow(Record);

                SqlContext.Pipe.SendResultsEnd();
            }
        }
    }
}
