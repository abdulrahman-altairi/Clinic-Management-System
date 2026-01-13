using Clinic.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Clinic.DAL
{
    public class clsDoctorRepository
    {
        public int AddDoctor(Doctor doctor, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            string query = @"INSERT INTO Doctors (DoctorID, SpecializationID, Bio, ConsultationFee, IsAvailable)
                     VALUES (@DoctorID, @SpecializationID, @Bio, @ConsultationFee, @IsAvailable)";

            SqlParameter[] parameters = {
                new SqlParameter("@DoctorID", doctor.DoctorId),
                new SqlParameter("@SpecializationID", doctor.SpecializationId),
                new SqlParameter("@Bio", (object)doctor.Bio ?? DBNull.Value),
                new SqlParameter("@ConsultationFee", doctor.ConsultationFee),
                new SqlParameter("@IsAvailable", doctor.IsAvilable)
            };

            SqlConnection connToUse = connection ?? DBHelper.GetOpenConnection();

            return DBHelper.ExecuteNonQuery(query, parameters, connToUse, transaction);
        }
        public int UpdateFullProfile(Doctor doctor)
        {
            string query = @"UPDATE Doctors 
                             SET SpecializationID = @SpecializationID, Bio = @Bio, ConsultationFee = @ConsultationFee 
                             WHERE DoctorID = @DoctorID";

            SqlParameter[] parameters = {
                new SqlParameter("@DoctorID", doctor.DoctorId),
                new SqlParameter("@SpecializationID", doctor.SpecializationId),
                new SqlParameter("@Bio", (object)doctor.Bio ?? DBNull.Value),
                new SqlParameter("@ConsultationFee", doctor.ConsultationFee)
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int ChangeSpecialization(int doctorId, int newSpecializationId)
        {
            string query = "UPDATE Doctors SET SpecializationID = @SpecializationID WHERE DoctorID = @DoctorID";
            SqlParameter[] parameters = {
                new SqlParameter("@DoctorID", doctorId),
                new SqlParameter("@SpecializationID", newSpecializationId)
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateConsultationFee(int doctorId, decimal newFee)
        {
            string query = "UPDATE Doctors SET ConsultationFee = @Fee WHERE DoctorID = @DoctorID";
            SqlParameter[] parameters = {
                new SqlParameter("@DoctorID", doctorId),
                new SqlParameter("@Fee", newFee)
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateBio(int doctorId, string newBio)
        {
            string query = "UPDATE Doctors SET Bio = @Bio WHERE DoctorID = @DoctorID";
            SqlParameter[] parameters = {
                new SqlParameter("@DoctorID", doctorId),
                new SqlParameter("@Fee", newBio)
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int SetAvailability(int doctorId, bool isAvailable)
        {
            string query = "UPDATE Doctors SET IsAvailable = @IsAvailable WHERE DoctorID = @DoctorID";

            SqlParameter[] parameters = {
                new SqlParameter("@DoctorID", doctorId),
                new SqlParameter("@IsAvailable", isAvailable)
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public List<Doctor> GetAvailableDoctors()
        {
            string query = @"SELECT D.*, P.FirstName, P.LastName, P.Email, P.ContactNumber, S.SpecializationName 
                     FROM Doctors D
                     INNER JOIN People P ON D.DoctorID = P.PersonID
                     INNER JOIN dbo.Specializations S ON D.SpecializationID = S.SpecializationID
                     WHERE D.IsAvailable = 1";

            DataTable dt = DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection());
            return _MapTableToDoctorList(dt);
        }

        public Doctor GetDoctorById(int doctorId)
        {
            string query = @"SELECT D.*, P.FirstName, P.LastName, P.Email, P.ContactNumber, S.SpecializationName 
                     FROM Doctors D
                     INNER JOIN People P ON D.DoctorID = P.PersonID
                     INNER JOIN dbo.Specializations S ON D.SpecializationID = S.SpecializationID WHERE D.DoctorID = @DoctorID";

            SqlParameter[] parameters = {
                new SqlParameter("@DoctorID", doctorId),
            };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());

            return _MapRowToDoctor(dt.Rows[0]);
        }

        public List<Doctor> GetAllDoctors()
        {
            string query = @"SELECT D.*, P.FirstName, P.LastName, P.Email, P.ContactNumber, S.SpecializationName 
                     FROM Doctors D
                     INNER JOIN People P ON D.DoctorID = P.PersonID
                     INNER JOIN dbo.Specializations S ON D.SpecializationID = S.SpecializationID";

            DataTable dt = DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection());
            return _MapTableToDoctorList(dt);
        }

        public List<Doctor> GetDoctorsBySpecialization(int specializationId)
        {
            string query = @"SELECT D.*, P.FirstName, P.LastName, P.Email, P.ContactNumber, S.SpecializationName 
                     FROM Doctors D
                     INNER JOIN People P ON D.DoctorID = P.PersonID
                     INNER JOIN dbo.Specializations S ON D.SpecializationID = S.SpecializationID
                     WHERE D.SpecializationID = @SpecializationID";

            SqlParameter[] parameters = { new SqlParameter("@SpecializationID", specializationId) };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return _MapTableToDoctorList(dt);
        }

        public List<Doctor> SearchDoctors(string keyword)
        {
            string query = @"SELECT D.*, P.FirstName, P.LastName, P.Email, P.ContactNumber, S.SpecializationName 
                     FROM Doctors D
                     INNER JOIN People P ON D.DoctorID = P.PersonID
                     INNER JOIN dbo.Specializations S ON D.SpecializationID = S.SpecializationID 
                             WHERE P.FirstName LIKE @Key OR P.LastName LIKE @Key 
                             OR S.SpecializationName LIKE @Key OR D.Bio LIKE @Key";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@Key", "%" + keyword + "%") 
            };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());

            return _MapTableToDoctorList(dt);
        }

        public List<Doctor> GetDoctorsPaged(int page, int pageSize)
        {
            string query = @"SELECT D.*, P.FirstName, P.LastName, P.Email, P.ContactNumber, S.SpecializationName 
                     FROM Doctors D
                     INNER JOIN People P ON D.DoctorID = P.PersonID
                     INNER JOIN dbo.Specializations S ON D.SpecializationID = S.SpecializationID 
                             ORDER BY D.DoctorID 
                             OFFSET @Offset ROWS FETCH NEXT @Size ROWS ONLY";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@Offset", (page - 1) * pageSize),
                new SqlParameter("@Size", pageSize)
            };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());

            return _MapTableToDoctorList(dt);
        }

        public int GetTotalDoctorsCount()
        {
            string query = "SELECT COUNT(*) FROM Doctors";
            object result = DBHelper.ExecuteScalar(query, null, DBHelper.GetOpenConnection());
            return (result != null) ? Convert.ToInt32(result) : 0;
        }

        public Dictionary<string, int> GetCountPerSpecialization()
        {
            Dictionary<string, int> stats = new Dictionary<string, int>();
            string query = @"SELECT S.SpecializationName, COUNT(D.DoctorID) as DoctorCount
                     FROM Specializations S
                     LEFT JOIN Doctors D ON S.SpecializationID = D.SpecializationID
                     GROUP BY S.SpecializationName";

            DataTable dt = DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection());

            foreach (DataRow row in dt.Rows)
            {
                stats.Add(row["SpecializationName"].ToString(), Convert.ToInt32(row["DoctorCount"]));
            }
            return stats;
        }

        public int DoctorExists(int doctorId)
        {
            string query = "SELECT 1 FROM Doctors WHERE DoctorID = @DoctorID";

            SqlParameter[] parameters = { new SqlParameter("@DoctorID", doctorId) };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());

            return result != null ? Convert.ToInt32(result) : 0;
        }

        private List<Doctor> _MapTableToDoctorList(DataTable dt)
        {
            List<Doctor> list = new List<Doctor>();
            foreach (DataRow row in dt.Rows) list.Add(_MapRowToDoctor(row));
            return list;
        }

        private Doctor _MapRowToDoctor(DataRow row)
        {
            return new Doctor
            {
                DoctorId = (int)row["DoctorID"],
                FirstName = row["FirstName"]?.ToString() ?? "",
                LastName = row["LastName"]?.ToString() ?? "",
                Email = row["Email"]?.ToString() ?? "",
                ContactNumber = row["ContactNumber"]?.ToString() ?? "",
                SpecializationId = (int)row["SpecializationID"],
                SpecializationName = row["SpecializationName"]?.ToString() ?? "",
                Bio = row["Bio"] == DBNull.Value ? "No Bio available" : row["Bio"].ToString(),
                ConsultationFee = row["ConsultationFee"] == DBNull.Value ? 0m : (decimal)row["ConsultationFee"],
                IsAvilable = row["IsAvailable"] != DBNull.Value && (bool)row["IsAvailable"]
            };
        }
    }
}
