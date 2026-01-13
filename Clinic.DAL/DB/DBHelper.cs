using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Clinic.DAL
{
    public static class DBHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["SmartClinicConnection"]?.ConnectionString
            ?? throw new Exception("ConnectionString 'SmartClinicConnection' not found");

        /*==============================================
         * Connection & Transaction
         ==============================================*/

        public static SqlConnection GetOpenConnection()
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static SqlTransaction BeginTransaction(SqlConnection connection)
        {
            if (connection == null || connection.State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must be open.");

            return connection.BeginTransaction();
        }

        /*==============================================
         * ExecuteNonQuery
         ==============================================*/

        public static int ExecuteNonQuery(
            string query,
            SqlParameter[] parameters,
            SqlConnection connection,
            SqlTransaction transaction = null)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (transaction == null && connection != null && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /*==============================================
         * ExecuteScalar
         ==============================================*/

        public static object ExecuteScalar(
            string query,
            SqlParameter[] parameters,
            SqlConnection connection,
            SqlTransaction transaction = null)
        {
           try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteScalar();
                }
            }

            finally
            {
                // الـ finally تضمن تنفيذ هذا الكود سواء نجحت العملية أو فشلت
                if (transaction == null && connection != null && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /*==============================================
         * ExecuteQuery
         ==============================================*/

        public static DataTable ExecuteQuery(
            string query,
            SqlParameter[] parameters,
            SqlConnection connection,
            SqlTransaction transaction = null)
        {
           try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        return dt;
                    }
                }
            }

            finally
            {
                if (transaction == null && connection != null && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
    }
}
