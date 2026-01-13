using Clinic.Entities;
using Clinic.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.DAL.Repositories
{
    public class clsInvoiceRepositroy
    {
        public int CreateInvoice(Invoice inv)
        {

            string query = @"INSERT INTO Invoices 
                            (AppointmentID, PatientID, TotalAmount, TaxAmount, DiscountAmount, DueDate, InvoiceStatus)
                            VALUES (@AppID, @PatID, @Total, @Tax, @Discount, @Due, @Status);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                new SqlParameter("@AppID", inv.AppointmentId),
                new SqlParameter("@PatID", inv.PatientId),
                new SqlParameter("@Total", inv.TotalAmount),
                new SqlParameter("@Tax", inv.TaxAmount),
                new SqlParameter("@Discount", inv.DiscountAmount),
                new SqlParameter("@Due", (object)inv.DueDate ?? DBNull.Value),
                new SqlParameter("@Status", (int)(inv.InvoiceStatus ?? enInvoiceStatus.Issued)),
            };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : -1;
        }

        public int UpdateInvoice(Invoice inv)
        {
            var currentInvoice = GetById(inv.InvoiceId);

            if (currentInvoice != null)
            {
                if (currentInvoice.InvoiceStatus == enInvoiceStatus.PartiallyPaid ||
                    currentInvoice.InvoiceStatus == enInvoiceStatus.Paid ||
                    currentInvoice.InvoiceStatus == enInvoiceStatus.Cancelled) 
                {
                    return -1;
                }
            }

            string query = @"UPDATE Invoices 
                     SET TotalAmount = @Total, TaxAmount = @Tax, DiscountAmount = @Discount 
                     WHERE InvoiceID = @ID";

            SqlParameter[] parameters = {
        new SqlParameter("@ID", inv.InvoiceId),
        new SqlParameter("@Total", inv.TotalAmount),
        new SqlParameter("@Tax", inv.TaxAmount),
        new SqlParameter("@Discount", inv.DiscountAmount)
    };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }
        public int UpdateStatus(int invoiceId, enInvoiceStatus newStatus)
        {
            var currentInvoice = GetById(invoiceId);
            if (currentInvoice == null) return 0;

            if (currentInvoice.InvoiceStatus >= enInvoiceStatus.Paid)
            {
                if (newStatus < currentInvoice.InvoiceStatus)
                {
                    return -1; 
                }
            }

            string query = "UPDATE Invoices SET InvoiceStatus = @Status WHERE InvoiceID = @ID";
            SqlParameter[] parameters = {
        new SqlParameter("@ID", invoiceId),
        new SqlParameter("@Status", (byte)newStatus)
    };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public Invoice GetById(int invoiceId)
        {
            string query = @"SELECT i.*, p.FirstName + ' ' + p.LastName as PatientName 
                             FROM Invoices i
                             JOIN People p ON i.PatientID = p.PersonID
                             WHERE i.InvoiceID = @ID";
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", invoiceId),
            };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return dt.Rows.Count > 0 ? MapRowToInvoice(dt.Rows[0]) : null;
        }

        public List<Invoice> GetPatientInvoices(int patientId)
        {
            string query = "SELECT * FROM Invoices WHERE PatientID = @PatID ORDER BY InvoiceDate DESC";
            SqlParameter[] parameters =
            {
                new SqlParameter("@PatID", patientId),
            };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());

            return MapTableToList(dt);
        }

        public List<Invoice> GetInvoicesByStatus(string status)
        {
            string query = "SELECT * FROM Invoices WHERE InvoiceStatus = @Status";
            SqlParameter[] parameters =
            {
                new SqlParameter("@Status", status)
            };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());

            return MapTableToList(dt);
        }

        public List<Invoice> GetInvoicesByDateRange(DateTime startDate, DateTime endDate)
        {
            string query = "SELECT * FROM Invoices WHERE InvoiceDate BETWEEN @Start AND @End ORDER BY InvoiceDate DESC";
            SqlParameter[] parameters = {
            new SqlParameter("@Start", startDate),
            new SqlParameter("@End", endDate)
        };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return MapTableToList(dt);
        }

        public int InvoiceExistsForAppointment(int appointmentId)
        {
            string query = "SELECT COUNT(1) FROM Invoices WHERE AppointmentID = @AppID";
            SqlParameter[] parameters = { new SqlParameter("@AppID", appointmentId) };
            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public decimal GetTotalOutstandingBalance(int patientId)
        {
            string query = @"SELECT SUM(NetAmount) FROM Invoices 
                         WHERE PatientID = @PatID 
                         AND InvoiceStatus NOT IN (@Paid, @Cancelled)";

            SqlParameter[] parameters = {
            new SqlParameter("@PatID", patientId),
            new SqlParameter("@Paid", (byte)enInvoiceStatus.Paid),
            new SqlParameter("@Cancelled", (byte)enInvoiceStatus.Cancelled)
        };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        public decimal GetTotalRevenue(DateTime startDate, DateTime endDate)
        {
            string query = "SELECT SUM(TotalAmount) FROM Invoices WHERE InvoiceDate BETWEEN @Start AND @End AND InvoiceStatus = @Paid";
            SqlParameter[] parameters = {
            new SqlParameter("@Start", startDate),
            new SqlParameter("@End", endDate),
            new SqlParameter("@Paid", (byte)enInvoiceStatus.Paid)
        };
            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        private List<Invoice> MapTableToList(DataTable dt)
        {
            List<Invoice> list = new List<Invoice>();
            foreach (DataRow row in dt.Rows) list.Add(MapRowToInvoice(row));
            return list;
        }

        private Invoice MapRowToInvoice(DataRow row)
        {
            var inv = new Invoice
            {
                InvoiceId = Convert.ToInt32(row["InvoiceID"]),
                InvoiceNumber = row["InvoiceNumber"].ToString(),
                AppointmentId = Convert.ToInt32(row["AppointmentID"]),
                PatientId = Convert.ToInt32(row["PatientID"]),
                TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                TaxAmount = Convert.ToDecimal(row["TaxAmount"]),
                DiscountAmount = Convert.ToDecimal(row["DiscountAmount"]),
                NetAmount = Convert.ToDecimal(row["NetAmount"]),
                InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                DueDate = row["DueDate"] != DBNull.Value ? (DateTime?)row["DueDate"] : null,
                InvoiceStatus = (enInvoiceStatus)Convert.ToByte(row["InvoiceStatus"])
            };

            if (row.Table.Columns.Contains("PatientName"))
                inv.PatientName = row["PatientName"].ToString();

            return inv;
        }
    }
}
