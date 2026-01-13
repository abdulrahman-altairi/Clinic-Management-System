using Clinic.Entities;
using Clinic.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.DAL.Repositories
{
    public class clsPaymentRepositroy
    {
        public int AddPayment(Payment payment)
        {
            string query = @"INSERT INTO Payments (InvoiceID, PaymentAmount, PaymentMethod, TransactionRef)
                             VALUES (@InvoiceID, @Amount, @Method, @Ref);
                             SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                new SqlParameter("@InvoiceID", payment.InvoiceId),
                new SqlParameter("@Amount", payment.PaymentAmount),
                new SqlParameter("@Method", payment.PaymentMethod),
                new SqlParameter("@Ref", (object)payment.TransactionRef ?? DBNull.Value)
            };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : -1;
        }

        public List<Payment> GetPaymentsByInvoiceId(int invoiceId)
        {
            string query = "SELECT * FROM Payments WHERE InvoiceID = @InvoiceID ORDER BY PaymentDate DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@InvoiceID", invoiceId)
            };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());

            List<Payment> list = new List<Payment>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Payment
                {
                    PaymentId = (int)row["PaymentID"],
                    InvoiceId = (int)row["InvoiceID"],
                    PaymentAmount = (decimal)row["PaymentAmount"],
                    PaymentDate = (DateTime)row["PaymentDate"],
                    PaymentMethod = (enPaymentMethod)row["PaymentMethod"],
                    TransactionRef = row["TransactionRef"]?.ToString()
                });
            }
            return list;
        }

        public DataTable GetDailyIncomeByMethod(DateTime date)
        {
            string query = @"SELECT PaymentMethod, SUM(PaymentAmount) as TotalIncome 
                             FROM Payments 
                             WHERE CAST(PaymentDate AS DATE) = CAST(@Date AS DATE)
                             GROUP BY PaymentMethod";
            SqlParameter[] parameters = {
                new SqlParameter("@Date", date)
            };

            return DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public Payment GetById(int paymentId)
        {
            string query = "SELECT * FROM Payments WHERE PaymentID = @PaymentID";
            SqlParameter[] parameters = { new SqlParameter("@PaymentID", paymentId) };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new Payment
                {
                    PaymentId = (int)row["PaymentID"],
                    InvoiceId = (int)row["InvoiceID"],
                    PaymentAmount = (decimal)row["PaymentAmount"],
                    PaymentDate = (DateTime)row["PaymentDate"],
                    PaymentMethod = (enPaymentMethod)row["PaymentMethod"],
                    TransactionRef = row["TransactionRef"]?.ToString()
                };
            }
            return null;
        }

        public decimal GetTotalPaidForInvoice(int invoiceId)
        {
            string query = "SELECT ISNULL(SUM(PaymentAmount), 0) FROM Payments WHERE InvoiceID = @InvoiceID";
            SqlParameter[] parameters = { new SqlParameter("@InvoiceID", invoiceId) };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return Convert.ToDecimal(result);
        }

        public DataTable GetPaymentsByPeriod(DateTime fromDate, DateTime toDate)
        {
            string query = @"SELECT * FROM Payments 
                     WHERE CAST(PaymentDate AS DATE) BETWEEN @From AND @To
                     ORDER BY PaymentDate DESC";
            SqlParameter[] parameters = {
        new SqlParameter("@From", fromDate),
        new SqlParameter("@To", toDate)
    };
            return DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
        }
    }
}
