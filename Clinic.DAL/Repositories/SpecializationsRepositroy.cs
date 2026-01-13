using Clinic.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace Clinic.DAL.Repositories
{
    public class clsSpecializationsRepositroy
    {
        public int AddSpecialization(Specialization spec)
        {
            string query = @"INSERT INTO dbo.Specializations (SpecializationName, Description)
                             VALUES (@Name, @Desc);
                             SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", spec.SpecializationName),
                new SqlParameter("@Desc", (object)spec.SpecializationDescription ?? DBNull.Value)
            };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int UpdateSpecialization(Specialization spec)
        {
            string query = @"UPDATE dbo.Specializations 
                             SET SpecializationName = @Name, Description = @Desc 
                             WHERE SpecializationID = @ID";

            SqlParameter[] parameters = {
                new SqlParameter("@ID", spec.SpecializationId),
                new SqlParameter("@Name", spec.SpecializationName),
                new SqlParameter("@Desc", (object)spec.SpecializationDescription ?? DBNull.Value)
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public Specialization GetById(int specId)
        {
            string query = "SELECT * FROM Specializations WHERE SpecializationID = @ID";

            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", specId)
            };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return dt.Rows.Count > 0 ? MapRowToSpec(dt.Rows[0]) : null;
        }

        public List<Specialization> GetAllSpecializations()
        {
            string query = "SELECT * FROM dbo.Specializations ORDER BY SpecializationName";
            DataTable dt = DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection());
            return MapTableToList(dt);
        }

        public List<Specialization> SearchSpecializations(string keyword)
        {
            string query = "SELECT * FROM dbo.Specializations WHERE SpecializationName LIKE @Key OR Description LIKE @Key";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Key", "%" + keyword + "%")
            };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return MapTableToList(dt);
        }

        public int SpecializationExists(string name, int excludeId = 0)
        {
            string query = "SELECT 1 FROM Specializations WHERE SpecializationName = @Name AND SpecializationID != @ID";
            SqlParameter[] parameters = {
            new SqlParameter("@Name", name),
            new SqlParameter("@ID", excludeId)
        };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? 1 : 0;
        }
        public int GetDoctorsCountInSpecialization(int specId)
        {
            string query = "SELECT COUNT(1) FROM dbo.Doctors WHERE SpecializationID = @ID";
            SqlParameter[] parameters = { new SqlParameter("@ID", specId) };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public bool CanDeleteSpecialization(int specId)
        {
            return GetDoctorsCountInSpecialization(specId) == 0;
        }

        public int DeleteSpecialization(int specId)
        {
            if (!CanDeleteSpecialization(specId)) return -1; 

            string query = "DELETE FROM dbo.Specializations WHERE SpecializationID = @ID";
            SqlParameter[] parameters = { new SqlParameter("@ID", specId) };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        private List<Specialization> MapTableToList(DataTable dt)
        {
            List<Specialization> list = new List<Specialization>();
            foreach (DataRow row in dt.Rows) list.Add(MapRowToSpec(row));
            return list;
        }

        private Specialization MapRowToSpec(DataRow row)
        {
            return new Specialization
            {
                SpecializationId = (int)row["SpecializationID"],
                SpecializationName = row["SpecializationName"].ToString(),
                SpecializationDescription = row["Description"]?.ToString()
            };
        }
    }
}
