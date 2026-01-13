using Clinic.Entities;
using Clinic.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Clinic.DAL.Repositories
{
    public class clsPatientRepositroy
    {
        public int AddPatient(Patient patient, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            string query = @"INSERT INTO Patients (PatientID, InsuranceProvider, InsurancePolicyNumber, EmergencyContactName, EmergencyContactPhone)
                             VALUES (@PatientID, @InsuranceProvider, @InsurancePolicyNumber, @EmergencyContactName, @EmergencyContactPhone); SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                new SqlParameter("@PatientID", patient.PatintId),
                new SqlParameter("@InsuranceProvider", (object)patient.InsuranceProvider ?? DBNull.Value),
                new SqlParameter("@InsurancePolicyNumber", (object)patient.InsurancePolicyNumber ?? DBNull.Value),
                new SqlParameter("@EmergencyContactName", (object)patient.EmergencyContactName ?? DBNull.Value),
                new SqlParameter("@EmergencyContactPhone", (object)patient.EmergencyContactPhone ?? DBNull.Value)
            };

            SqlConnection connToUse = connection ?? DBHelper.GetOpenConnection();
            return DBHelper.ExecuteNonQuery(query, parameters, connToUse, transaction);
        }

        public int UpdatePatient(Patient patient, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            string query = @"UPDATE Patients 
                             SET InsuranceProvider = @InsuranceProvider, 
                                 InsurancePolicyNumber = @InsurancePolicyNumber, 
                                 EmergencyContactName = @EmergencyContactName, 
                                 EmergencyContactPhone = @EmergencyContactPhone
                             WHERE PatientID = @PatientID";

            SqlParameter[] parameters = {
                new SqlParameter("@PatientID", patient.PatintId),
                new SqlParameter("@InsuranceProvider", (object)patient.InsuranceProvider ?? DBNull.Value),
                new SqlParameter("@InsurancePolicyNumber", (object)patient.InsurancePolicyNumber ?? DBNull.Value),
                new SqlParameter("@EmergencyContactName", (object)patient.EmergencyContactName ?? DBNull.Value),
                new SqlParameter("@EmergencyContactPhone", (object)patient.EmergencyContactPhone ?? DBNull.Value)
            };

            SqlConnection connToUse = connection ?? DBHelper.GetOpenConnection();
            return DBHelper.ExecuteNonQuery(query, parameters, connToUse, transaction);
        }

        public int UpdateInsuranceProvider(int patientId, string insuranceProvider)
        {
            string query = @"UPDATE Patients SET InsuranceProvider = @InsuranceProvider WHERE PatientID = @PatientID";
            SqlParameter[] parameters = {
                new SqlParameter("@PatientID", patientId),
                new SqlParameter("@InsuranceProvider", (object)insuranceProvider ?? DBNull.Value)
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateInsurancePolicyNumber(int patientId, string insurancePolicyNumber)
        {
            string query = @"UPDATE Patients SET InsurancePolicyNumber = @InsurancePolicyNumber WHERE PatientID = @PatientID";
            SqlParameter[] parameters = {
                new SqlParameter("@PatientID", patientId),
                new SqlParameter("@InsurancePolicyNumber", (object)insurancePolicyNumber ?? DBNull.Value)
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateEmergencyContactName(int patientId, string emergencyContactName)
        {
            string query = @"UPDATE Patients SET EmergencyContactName = @EmergencyContactName WHERE PatientID = @PatientID";
            SqlParameter[] parameters = {
                new SqlParameter("@PatientID", patientId),
                new SqlParameter("@EmergencyContactName", (object)emergencyContactName ?? DBNull.Value)
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateEmergencyContactPhone(int patientId, string emergencyContactPhone)
        {
            string query = @"UPDATE Patients SET EmergencyContactPhone = @EmergencyContactPhone WHERE PatientID = @PatientID";
            SqlParameter[] parameters = {
                new SqlParameter("@PatientID", patientId),
                new SqlParameter("@EmergencyContactPhone", (object)emergencyContactPhone ?? DBNull.Value)
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public Patient GetPatientById(int patientId)
        {
            string query = @"SELECT PT.*, P.FirstName, P.LastName, P.Email, P.ContactNumber, P.DateOfBirth, P.Gender, P.Address
                             FROM Patients PT
                             INNER JOIN People P ON PT.PatientID = P.PersonID 
                             WHERE PT.PatientID = @PatientID";

            SqlParameter[] parameters = { new SqlParameter("@PatientID", patientId) };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());

            return dt.Rows.Count > 0 ? MapRowToPatient(dt.Rows[0]) : null;
        }

        public List<Patient> GetAllPatients()
        {
            string query = @"SELECT PT.*, P.FirstName, P.LastName, P.Email, P.ContactNumber, P.DateOfBirth, P.Gender, P.Address
                             FROM Patients PT
                             INNER JOIN People P ON PT.PatientID = P.PersonID";

            DataTable dt = DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection());
            return MapTableToPatientList(dt);
        }

        public List<Patient> GetPatientsPaged(int page, int pageSize)
        {
            int offset = (page - 1) * pageSize;
            string query = @"SELECT PT.*, P.FirstName, P.LastName, P.Email, P.ContactNumber, P.DateOfBirth, P.Gender, P.Address
                             FROM Patients PT
                             INNER JOIN People P ON PT.PatientID = P.PersonID
                             ORDER BY PT.PatientID 
                             OFFSET @Offset ROWS FETCH NEXT @Size ROWS ONLY";

            SqlParameter[] parameters = {
                new SqlParameter("@Offset", offset),
                new SqlParameter("@Size", pageSize)
            };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return MapTableToPatientList(dt);
        }

        public List<Patient> SearchPatients(string keyword)
        {
            string query = @"SELECT PT.*, P.FirstName, P.LastName, P.Email, P.ContactNumber, P.DateOfBirth, P.Gender, P.Address
                             FROM Patients PT
                             INNER JOIN People P ON PT.PatientID = P.PersonID 
                             WHERE P.FirstName LIKE @Key OR P.LastName LIKE @Key 
                             OR P.ContactNumber LIKE @Key OR PT.InsurancePolicyNumber LIKE @Key";

            SqlParameter[] parameters = { new SqlParameter("@Key", "%" + keyword + "%") };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return MapTableToPatientList(dt);
        }

        public int GetTotalPatientsCount()
        {
            string query = "SELECT COUNT(*) FROM Patients";
            object result = DBHelper.ExecuteScalar(query, null, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int PatientExists(int patientId)
        {
            string query = "SELECT COUNT(1) FROM Patients WHERE PatientID = @PatientID";
            SqlParameter[] parameters = { new SqlParameter("@PatientID", patientId) };
            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int InsurancePolicyExists(string policyNumber, int excludePatientId = -1)
        {
            string query = "SELECT COUNT(1) FROM Patients WHERE InsurancePolicyNumber = @Policy AND PatientID <> @ID";
            SqlParameter[] parameters = {
                new SqlParameter("@Policy", policyNumber),
                new SqlParameter("@ID", excludePatientId)
            };
            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        private List<Patient> MapTableToPatientList(DataTable dt)
        {
            List<Patient> list = new List<Patient>();
            if (dt == null) return list;
            foreach (DataRow row in dt.Rows) list.Add(MapRowToPatient(row));
            return list;
        }

        private Patient MapRowToPatient(DataRow row)
        {
            return new Patient
            {
                PatintId = Convert.ToInt32(row["PatientID"]),

                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                Email = row["Email"].ToString(),
                ContactNumber = row["ContactNumber"].ToString(),
                Address = row["Address"] != DBNull.Value ? row["Address"].ToString() : string.Empty,

                DateOfBirth = Convert.ToDateTime(row["DateOfBirth"]),

                Gender = (enGender)Convert.ToInt32(row["Gender"]),

                InsuranceProvider = row["InsuranceProvider"]?.ToString() ?? string.Empty,
                InsurancePolicyNumber = row["InsurancePolicyNumber"]?.ToString() ?? string.Empty,

                EmergencyContactName = row["EmergencyContactName"]?.ToString() ?? string.Empty,
                EmergencyContactPhone = row["EmergencyContactPhone"]?.ToString() ?? string.Empty
            };
        }

    }
}