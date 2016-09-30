using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Configuration;
using System.Data.SqlClient;

namespace TSMC14B.Models
{
    public class DBConnector
    {

        // 執行SQL操作指令：INSERT、UPDATE、DELETE…
        static public int executeSQL(string DBName, string SQLStr, ArrayList Parameter = null)
        {
            int rowCount = 0;
            IDbCommand DBCmd = null;

            try
            {
                if (ConfigurationManager.ConnectionStrings[DBName].ConnectionString.IndexOf("Provider=SQLOLEDB") >= 0)
                {
                    DBCmd = new OleDbCommand(SQLStr, new OleDbConnection(ConfigurationManager.ConnectionStrings[DBName].ConnectionString));
                    if (Parameter != null)                  // 設定查詢參數
                        for (int i = 0; i < Parameter.Count; i++)
                            DBCmd.Parameters.Add(new OleDbParameter(i.ToString(), Parameter[i]));
                }
                else
                {
                    DBCmd = new SqlCommand(SQLStr, new SqlConnection(ConfigurationManager.ConnectionStrings[DBName].ConnectionString));
                    if (Parameter != null)                  // 設定查詢參數
                        for (int i = 0; i < Parameter.Count; i++)
                            DBCmd.Parameters.Add(new SqlParameter(i.ToString(), Parameter[i]));
                }

                DBCmd.Connection.Open();
                rowCount = DBCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBCmd.Connection.Close();
            }

            return rowCount;
        }

        // 執行SQL查詢指令：SELECT
        static public DataSet executeQuery(string DBName, string SQLStr, ArrayList Parameter = null)
        {
            DataSet ds = new DataSet();
            IDbDataAdapter oda = null;
            IDbCommand DBCmd = null;

            try
            {
                if (ConfigurationManager.ConnectionStrings[DBName].ConnectionString.IndexOf("Provider=SQLOLEDB") >= 0)
                {
                    DBCmd = new OleDbCommand(SQLStr, new OleDbConnection(ConfigurationManager.ConnectionStrings[DBName].ConnectionString));
                    DBCmd.CommandTimeout = 30000;
                    if (Parameter != null)                  // 設定查詢參數
                        for (int i = 0; i < Parameter.Count; i++)
                            DBCmd.Parameters.Add(new OleDbParameter(i.ToString(), Parameter[i]));
                    oda = new OleDbDataAdapter(DBCmd as OleDbCommand);
                }
                else
                {
                    DBCmd = new SqlCommand(SQLStr, new SqlConnection(ConfigurationManager.ConnectionStrings[DBName].ConnectionString));
                    DBCmd.CommandTimeout = 30000;
                    if (Parameter != null)                  // 設定查詢參數
                        for (int i = 0; i < Parameter.Count; i++)
                            DBCmd.Parameters.Add(new SqlParameter(i.ToString(), Parameter[i]));
                    oda = new SqlDataAdapter(DBCmd as SqlCommand);
                }
                oda.Fill(ds);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBCmd.Connection.Close();
            }

            return ds;
        }
    

    }
}