using Clinic.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Clinic.DAL
{
    public class clsMedicalRecordRepositroy
    {
        public int AddRecord(MedicalRecord record)
        {
            string query = @"INSERT INTO MedicalRecords (AppointmentID, Diagnosis, Prescription, Notes)
                             VALUES (@AppID, @Diagnosis, @Prescription, @Notes);
                             SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                new SqlParameter("@AppID", record.AppointmentID),
                new SqlParameter("@Diagnosis", record.Diagnosis),
                new SqlParameter("@Prescription", (object)record.Prescription ?? DBNull.Value),
                new SqlParameter("@Notes", (object)record.Notes ?? DBNull.Value)
            };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : -1;
        }

        public int UpdateRecord(MedicalRecord record)
        {
            string query = @"UPDATE MedicalRecords 
                             SET Diagnosis = @Diagnosis, Prescription = @Prescription, Notes = @Notes 
                             WHERE RecordID = @ID";

            SqlParameter[] parameters = {
                new SqlParameter("@ID", record.RecordId),
                new SqlParameter("@Diagnosis", record.Diagnosis),
                new SqlParameter("@Prescription", (object)record.Prescription ?? DBNull.Value),
                new SqlParameter("@Notes", (object)record.Notes ?? DBNull.Value)
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public MedicalRecord GetById(int recordId)
        {
            string query = @"SELECT mr.*, p.FirstName + ' ' + p.LastName as PatientName, a.AppointmentDate
                             FROM MedicalRecords mr
                             JOIN Appointments a ON mr.AppointmentID = a.AppointmentID
                             JOIN People p ON a.PatientID = p.PersonID
                             WHERE mr.RecordID = @ID";

            SqlParameter[] parameters = { new SqlParameter("@ID", recordId) };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return dt.Rows.Count > 0 ? MapRowToRecord(dt.Rows[0]) : null;
        }

        public MedicalRecord GetByAppointmentId(int appointmentId)
        {
            string query = "SELECT * FROM MedicalRecords WHERE AppointmentID = @AppID";
            SqlParameter[] parameters = { new SqlParameter("@AppID", appointmentId) };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return dt.Rows.Count > 0 ? MapRowToRecord(dt.Rows[0]) : null;
        }

        public List<MedicalRecord> GetPatientHistory(int patientId)
        {
            string query = @"SELECT mr.*, d_p.FirstName + ' ' + d_p.LastName as DoctorName, a.AppointmentDate
                             FROM MedicalRecords mr
                             JOIN Appointments a ON mr.AppointmentID = a.AppointmentID
                             JOIN People d_p ON a.DoctorID = d_p.PersonID
                             WHERE a.PatientID = @PatID
                             ORDER BY a.AppointmentDate DESC";

            SqlParameter[] parameters = { new SqlParameter("@PatID", patientId) };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return MapTableToList(dt);
        }

        public List<MedicalRecord> SearchByDiagnosis(string keyword)
        {
            string query = "SELECT * FROM MedicalRecords WHERE Diagnosis LIKE @Key";
            SqlParameter[] parameters = { new SqlParameter("@Key", "%" + keyword + "%") };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return MapTableToList(dt);
        }

        public int RecordExistsForAppointment(int appointmentId)
        {
            string query = "SELECT COUNT(1) FROM MedicalRecords WHERE AppointmentID = @AppID";
            SqlParameter[] parameters = { new SqlParameter("@AppID", appointmentId) };
            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int DeleteRecord(int recordId)
        {
            string query = "DELETE FROM MedicalRecords WHERE RecordID = @ID";
            SqlParameter[] parameters = { new SqlParameter("@ID", recordId) };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public List<MedicalRecord> GetAllRecordsPaged(int pageNumber, int pageSize)
        {
            string query = @"SELECT mr.*, p.FirstName + ' ' + p.LastName as PatientName, a.AppointmentDate
                             FROM MedicalRecords mr
                             JOIN Appointments a ON mr.AppointmentID = a.AppointmentID
                             JOIN People p ON a.PatientID = p.PersonID
                             ORDER BY mr.RecordID
                             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            SqlParameter[] parameters = {
                new SqlParameter("@Offset", (pageNumber - 1) * pageSize),
                new SqlParameter("@PageSize", pageSize)
            };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return MapTableToList(dt);
        }

        public List<MedicalRecord> GetRecordsByDateRange(DateTime fromDate, DateTime toDate)
        {
            string query = @"SELECT mr.*, p.FirstName + ' ' + p.LastName as PatientName, a.AppointmentDate
                             FROM MedicalRecords mr
                             JOIN Appointments a ON mr.AppointmentID = a.AppointmentID
                             JOIN People p ON a.PatientID = p.PersonID
                             WHERE a.AppointmentDate BETWEEN @From AND @To
                             ORDER BY a.AppointmentDate DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@From", fromDate),
                new SqlParameter("@To", toDate)
            };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return MapTableToList(dt);
        }

        public int GetTotalRecordsCount()
        {
            string query = "SELECT COUNT(1) FROM MedicalRecords";
            object result = DBHelper.ExecuteScalar(query, null, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }


        private List<MedicalRecord> MapTableToList(DataTable dt)
        {
            List<MedicalRecord> list = new List<MedicalRecord>();
            foreach (DataRow row in dt.Rows) list.Add(MapRowToRecord(row));
            return list;
        }

        private MedicalRecord MapRowToRecord(DataRow row)
        {
            var record = new MedicalRecord
            {
                RecordId = (int)row["RecordID"],
                AppointmentID = (int)row["AppointmentID"],
                Diagnosis = row["Diagnosis"].ToString(),
                Prescription = row["Prescription"]?.ToString(),
                Notes = row["Notes"]?.ToString(),
                CreatedAt = (DateTime)row["CreatedDate"]
            };

            if (row.Table.Columns.Contains("PatientName")) record.PatientName = row["PatientName"].ToString();
            if (row.Table.Columns.Contains("DoctorName")) record.DoctorName = row["DoctorName"].ToString();
            if (row.Table.Columns.Contains("AppointmentDate")) record.AppointmentDate = (DateTime)row["AppointmentDate"];

            return record;
        }
    }
}