using Clinic.DAL;
using Clinic.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SmartClinic.DAL
{
    public class clsDoctorView
    {

        public List<DoctorView> GetAllDoctorsInfo()
        {
            string query = "SELECT * FROM vw_AllDoctors";
            return MapTableToList(DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection()));
        }

        public DoctorView GetDoctorFullDetails(int doctorId)
        {
            string query = "SELECT * FROM vw_AllDoctors WHERE DoctorID = @ID";
            SqlParameter[] parameters = { new SqlParameter("@ID", doctorId) };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return dt.Rows.Count > 0 ? MapRowToDoctorView(dt.Rows[0]) : null;
        }

        public List<DoctorView> SearchDoctorsGlobal(string keyword)
        {
            string query = @"SELECT * FROM vw_AllDoctors 
                             WHERE FirstName LIKE @Key OR LastName LIKE @Key 
                             OR SpecializationName LIKE @Key OR Bio LIKE @Key OR Email LIKE @Key";

            SqlParameter[] parameters = { new SqlParameter("@Key", "%" + keyword + "%") };
            return MapTableToList(DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection()));
        }

        public List<DoctorView> GetAvailableDoctorsBySpecialization(int specializationId)
        {
            string query = @"SELECT * FROM vw_AllDoctors 
                             WHERE SpecializationID = @SpecID AND IsAvailable = 1";

            SqlParameter[] parameters = {
                new SqlParameter("@SpecID", specializationId)
            };

            return MapTableToList(DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection()));
        }

        public List<DoctorView> FilterDoctorsByFeeRange(decimal minFee, decimal maxFee)
        {
            string query = "SELECT * FROM vw_AllDoctors WHERE ConsultationFee BETWEEN @Min AND @Max";

            SqlParameter[] parameters = {
                new SqlParameter("@Min", minFee),
                new SqlParameter("@Max", maxFee)
            };
            return MapTableToList(DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection()));
        }

        public DataTable GetDoctorCountPerSpecialization()
        {
            string query = @"SELECT SpecializationName, COUNT(*) as DoctorCount 
                             FROM vw_AllDoctors 
                             GROUP BY SpecializationName";
            return DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection());
        }

        private List<DoctorView> MapTableToList(DataTable dt)
        {
            List<DoctorView> list = new List<DoctorView>();
            foreach (DataRow row in dt.Rows) list.Add(MapRowToDoctorView(row));
            return list;
        }

        private DoctorView MapRowToDoctorView(DataRow row)
        {
            return new DoctorView
            {
                DoctorID = (int)row["DoctorID"],
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                Email = row["Email"].ToString(),
                ContactNumber = row["ContactNumber"].ToString(),
                SpecializationID = (int)row["SpecializationID"],
                SpecializationName = row["SpecializationName"].ToString(),
                Bio = row["Bio"] == DBNull.Value ? "" : row["Bio"].ToString(), // أكثر أماناً
                ConsultationFee = row["ConsultationFee"] == DBNull.Value ? 0 : (decimal)row["ConsultationFee"],
                IsAvailable = row["IsAvailable"] != DBNull.Value && (bool)row["IsAvailable"]
            };
        }
    }
}