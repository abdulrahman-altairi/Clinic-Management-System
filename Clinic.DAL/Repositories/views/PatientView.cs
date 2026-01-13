using Clinic.DAL;
using Clinic.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SmartClinic.DAL
{
    public class clsPatientView
    {

        public List<PatientView> GetAllPatients()
        {
            string query = "SELECT * FROM vw_AllPatients";
            return MapTableToList(DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection()));
        }


        public PatientView GetPatientDetails(int patientId)
        {
            string query = "SELECT * FROM vw_AllPatients WHERE PatientID = @ID";
            SqlParameter[] parameters = { new SqlParameter("@ID", patientId) };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return dt.Rows.Count > 0 ? MapRowToPatientView(dt.Rows[0]) : null;
        }


        public List<PatientView> SearchPatients(string keyword)
        {
            string query = @"SELECT * FROM vw_AllPatients 
                             WHERE FirstName LIKE @Key OR LastName LIKE @Key 
                             OR ContactNumber LIKE @Key OR Email LIKE @Key 
                             OR InsuranceProvider LIKE @Key";

            SqlParameter[] parameters = { new SqlParameter("@Key", "%" + keyword + "%") };
            return MapTableToList(DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection()));
        }

        public List<PatientView> GetPatientsByInsurance(string providerName)
        {
            string query = "SELECT * FROM vw_AllPatients WHERE InsuranceProvider = @Provider";

            SqlParameter[] parameters = { new SqlParameter("@Provider", providerName) };
            return MapTableToList(DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection()));
        }

        public List<PatientView> GetPatientsWithEmergencyContact()
        {
            string query = "SELECT * FROM vw_AllPatients WHERE EmergencyContactName IS NOT NULL";
            return MapTableToList(DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection()));
        }



        public DataTable GetPatientsCountByInsurance()
        {
            string query = @"SELECT ISNULL(InsuranceProvider, 'Self-Paid') as Provider, COUNT(*) as PatientCount 
                             FROM vw_AllPatients 
                             GROUP BY InsuranceProvider";
            return DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection());
        }

        private List<PatientView> MapTableToList(DataTable dt)
        {
            List<PatientView> list = new List<PatientView>();
            foreach (DataRow row in dt.Rows) list.Add(MapRowToPatientView(row));
            return list;
        }

        private PatientView MapRowToPatientView(DataRow row)
        {
            return new PatientView
            {
                PatientID = (int)row["PatientID"],
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                Email = row["Email"].ToString(),
                ContactNumber = row["ContactNumber"].ToString(),
                DateOfBirth = (DateTime)row["DateOfBirth"],
                InsuranceProvider = row["InsuranceProvider"]?.ToString(),
                EmergencyContactName = row["EmergencyContactName"]?.ToString()
            };
        }
    }
}