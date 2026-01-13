using Clinic.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.DAL.Repositories
{
    public class clsInvoiceItemRepositroy
    {
        public int AddItem(InvoiceItem item)
        {
            string query = @"INSERT INTO InvoiceItems (InvoiceID, ItemDescription, UnitPrice, Quantity)
                             VALUES (@InvoiceID, @Description, @Price, @Qty);
                             SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                new SqlParameter("@InvoiceID", item.InvoiceId),
                new SqlParameter("@Description", item.ItemDescription),
                new SqlParameter("@Price", item.UnitPrice),
                new SqlParameter("@Qty", item.Quantity)
            };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : -1;
        }

        public int UpdateItem(InvoiceItem item)
        {
            string query = @"UPDATE InvoiceItems 
                             SET ItemDescription = @Description, UnitPrice = @Price, Quantity = @Qty 
                             WHERE ItemID = @ItemID";

            SqlParameter[] parameters = {
                new SqlParameter("@ItemID", item.ItemId),
                new SqlParameter("@Description", item.ItemDescription),
                new SqlParameter("@Price", item.UnitPrice),
                new SqlParameter("@Qty", item.Quantity)
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public InvoiceItem GetItemById(int itemId)
        {
            string query = "SELECT * FROM InvoiceItems WHERE ItemID = @ItemID";
            SqlParameter[] parameters = { new SqlParameter("@ItemID", itemId) };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return dt.Rows.Count > 0 ? _MapRowToItem(dt.Rows[0]) : null;
        }

        public int DeleteAllItemsByInvoiceId(int invoiceId)
        {
            string query = "DELETE FROM InvoiceItems WHERE InvoiceID = @InvoiceID";
            SqlParameter[] parameters = { new SqlParameter("@InvoiceID", invoiceId) };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int DeleteItem(int itemId)
        {
            string query = @"DELETE FROM InvoiceItems 
                     WHERE ItemID = @ItemID 
                     AND InvoiceID IN (SELECT InvoiceID FROM Invoices WHERE InvoiceStatus < 3)";

            SqlParameter[] parameters = { new SqlParameter("@ItemID", itemId) };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public List<InvoiceItem> GetItemsByInvoiceId(int invoiceId)
        {
            string query = "SELECT * FROM InvoiceItems WHERE InvoiceID = @InvoiceID";

            SqlParameter[] parameters =
            {
                new SqlParameter("@InvoiceID", invoiceId)
            };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());

            List<InvoiceItem> items = new List<InvoiceItem>();
 
            return _MapTableToList(dt);
        }

        public decimal CalculateInvoiceSubTotal(int invoiceId)
        {
            string query = "SELECT SUM(LineTotal) FROM InvoiceItems WHERE InvoiceID = @InvoiceID";
            SqlParameter[] parameters =
            {
                new SqlParameter("@InvoiceID", invoiceId)
            };
            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        private List<InvoiceItem> _MapTableToList(DataTable dt)
        {
            List<InvoiceItem> list = new List<InvoiceItem>();
            foreach (DataRow row in dt.Rows) list.Add(_MapRowToItem(row));
            return list;
        }
        private InvoiceItem _MapRowToItem(DataRow row)
        {
            return new InvoiceItem
            {
                ItemId = (int)row["ItemID"],
                InvoiceId = (int)row["InvoiceID"],
                ItemDescription = row["ItemDescription"].ToString(),
                UnitPrice = (decimal)row["UnitPrice"],
                Quantity = (int)row["Quantity"],
                LineTotal = (decimal)row["LineTotal"]
            };
        }
    }
}
