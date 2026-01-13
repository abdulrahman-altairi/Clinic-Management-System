using Clinic.DAL;
using Clinic.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SmartClinic.DAL
{
    public class clsAppointmentView
    {

        public List<AppointmentView> GetAllAppointments()
        {
            string query = "SELECT * FROM vw_AppointmentDetails ORDER BY AppointmentDate DESC";
            return MapTableToList(DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection()));
        }

        public AppointmentView GetAppointmentById(int appointmentId)
        {
            string query = "SELECT * FROM vw_AppointmentDetails WHERE AppointmentID = @ID";

            SqlParameter[] parameters = { new SqlParameter("@ID", appointmentId) };


            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return dt.Rows.Count > 0 ? MapRowToView(dt.Rows[0]) : null;
        }


        public List<AppointmentView> GetTodaysAppointments()
        {
            string query = "SELECT * FROM vw_AppointmentDetails WHERE CAST(AppointmentDate AS DATE) = CAST(GETDATE() AS DATE) ORDER BY AppointmentDate";
            return MapTableToList(DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection()));
        }

        public List<AppointmentView> GetDoctorAppointmentsByDate(int doctorId, DateTime date)
        {
            string query = @"SELECT * FROM vw_AppointmentDetails 
                             WHERE AppointmentID IN (SELECT AppointmentID FROM Appointments WHERE DoctorID = @DocID)
                             AND CAST(AppointmentDate AS DATE) = CAST(@Date AS DATE)
                             ORDER BY AppointmentDate";

            SqlParameter[] parameters = {
                new SqlParameter("@DocID", doctorId),
                new SqlParameter("@Date", date)
            };
            return MapTableToList(DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection()));
        }

        public List<AppointmentView> GetAppointmentsByStatus(string status)
        {
            string query = "SELECT * FROM vw_AppointmentDetails WHERE AppointmentStatus = @Status";

            SqlParameter[] parameters = { new SqlParameter("@Status", status) };

            return MapTableToList(DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection()));
        }

        public List<AppointmentView> SearchAppointments(string keyword)
        {
            string query = @"SELECT * FROM vw_AppointmentDetails 
                             WHERE PatientName LIKE @Key OR DoctorName LIKE @Key OR SpecializationName LIKE @Key";

            SqlParameter[] parameters = { new SqlParameter("@Key", "%" + keyword + "%") };
            return MapTableToList(DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection()));
        }

        public decimal GetExpectedRevenue(DateTime startDate, DateTime endDate)
        {
            string query = @"SELECT SUM(ConsultationFee) FROM vw_AppointmentDetails 
                             WHERE AppointmentStatus IN ('Confirmed', 'Completed') 
                             AND AppointmentDate BETWEEN @Start AND @End";

            SqlParameter[] parameters = {
                new SqlParameter("@Start", startDate),
                new SqlParameter("@End", endDate)
            };
            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        public List<AppointmentView> GetPatientAppointmentHistory(int patientId)
        {
            string query = "SELECT * FROM vw_AppointmentDetails WHERE AppointmentID IN (SELECT AppointmentID FROM Appointments WHERE PatientID = @PatID) ORDER BY AppointmentDate DESC";
            SqlParameter[] parameters = { new SqlParameter("@PatID", patientId) };
            return MapTableToList(DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection()));
        }

        public DataTable GetAppointmentCountBySpecialization()
        {
            string query = @"SELECT SpecializationName, COUNT(*) as TotalAppointments 
                             FROM vw_AppointmentDetails 
                             GROUP BY SpecializationName";
            return DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection());
        }

        private List<AppointmentView> MapTableToList(DataTable dt)
        {
            List<AppointmentView> list = new List<AppointmentView>();
            foreach (DataRow row in dt.Rows) list.Add(MapRowToView(row));
            return list;
        }

        private AppointmentView MapRowToView(DataRow row)
        {
            return new AppointmentView
            {
                AppointmentID = (int)row["AppointmentID"],
                PatientName = row["PatientName"].ToString(),
                DoctorName = row["DoctorName"].ToString(),
                SpecializationName = row["SpecializationName"].ToString(),
                AppointmentDate = (DateTime)row["AppointmentDate"],
                AppointmentStatus = row["AppointmentStatus"].ToString(),
                ReasonForVisit = row["ReasonForVisit"]?.ToString(),
                ConsultationFee = (decimal)row["ConsultationFee"]
            };
        }
    }
}