using Clinic.Entities;
using Clinic.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Clinic.DAL.Repositories
{
    public class clsPeopleRepository
    {
        public int AddPerson(Person person, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            string query = @"INSERT INTO People(FirstName, LastName, DateOfBirth, Gender, ContactNumber, Email, Address)
                     VALUES(@FirstName, @LastName, @DateOfBirth, @Gender, @ContactNumber, @Email, @Address); 
                     SELECT SCOPE_IDENTITY();";

            SqlParameter[] parametars =
            {
                new SqlParameter("@FirstName", person.FirstName),
                new SqlParameter("@LastName", person.LastName),
                new SqlParameter("@DateOfBirth", (object)person.DateOfBirth ?? DBNull.Value),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@ContactNumber", person.ContactNumber),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@Address", (object) person.Address ?? DBNull.Value)
            };

            SqlConnection connToUse = connection ?? DBHelper.GetOpenConnection();

            object result = DBHelper.ExecuteScalar(query, parametars, connToUse, transaction);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int UpdatePerson(Person person)
        {
            string query = @"UPDATE People 
                         SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, 
                             Gender = @Gender, ContactNumber = @ContactNumber, Email = @Email, Address = @Address
                         WHERE PersonID = @PersonID";

            SqlParameter[] parameters = {
            new SqlParameter("@PersonID", person.PersonId),
            new SqlParameter("@FirstName", person.FirstName),
            new SqlParameter("@LastName", person.LastName),
            new SqlParameter("@DateOfBirth", (object)person.DateOfBirth ?? DBNull.Value),
            new SqlParameter("@Gender", person.Gender),
            new SqlParameter("@ContactNumber", person.ContactNumber),
            new SqlParameter("@Email", person.Email),
            new SqlParameter("@Address", (object)person.Address ?? DBNull.Value)
            };

            return DBHelper.ExecuteNonQuery(query,parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateFirstName(int personId, string firstName)
        {
            string query = @"UPDATE People 
                         SET FirstName = @FirstName
                         WHERE PersonID = @PersonID";

            SqlParameter[] parameters = {
            new SqlParameter("@PersonID", personId),
            new SqlParameter("@FirstName", firstName),
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateLastName(int personId, string lastName)
        {
            string query = @"UPDATE People 
                         SET LastName = @LastName
                         WHERE PersonID = @PersonID";

            SqlParameter[] parameters = {
            new SqlParameter("@PersonID", personId),
            new SqlParameter("@LastName", lastName),
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateGender(int personId, enGender gender)
        {
            string query = @"UPDATE People 
                         SET Gender = @Gender
                         WHERE PersonID = @PersonID";

            SqlParameter[] parameters = {
            new SqlParameter("@PersonID", personId),
            new SqlParameter("@Gender", (object)gender),
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateEmail(int personId, string email)
        {
            string query = @"UPDATE People 
                         SET Email = @Email
                         WHERE PersonID = @PersonID";

            SqlParameter[] parameters = {
            new SqlParameter("@PersonID", personId),
            new SqlParameter("@Email", email),
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateContactNumber(int personId, string contactNumber)
        {
            string query = @"UPDATE People 
                         SET ContactNumber = @ContactNumber
                         WHERE PersonID = @PersonID";

            SqlParameter[] parameters = {
            new SqlParameter("@PersonID", personId),
            new SqlParameter("@ContactNumber", contactNumber),
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }
        public int UpdateDateOfBirth(int personId, DateTime dateOfBirth)
        {
            string query = @"UPDATE People 
                         SET DateOfBirth = @DateOfBirth
                         WHERE PersonID = @PersonID";

            SqlParameter[] parameters = {
            new SqlParameter("@PersonID", personId),
            new SqlParameter("@DateOfBirth", dateOfBirth),
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateAddress(int personId, string address)
        {
            string query = @"UPDATE People 
                         SET Address = @Address
                         WHERE PersonID = @PersonID";

            SqlParameter[] parameters = {
            new SqlParameter("@PersonID", personId),
            new SqlParameter("@Address", address),
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int SoftDeletePerson(int personId)
        {
            string query = "UPDATE People SET IsDeleted = 1 WHERE PersonID = @PersonID";
            SqlParameter[] parameters = { new SqlParameter("@PersonID", personId) };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public Person GetPersonById(int personId)
        {
            string query = "SELECT * FROM People WHERE PersonID = @PersonID AND IsDeleted = 0";
            SqlParameter[] parameters = { new SqlParameter("@PersonID", personId) };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());

            if (dt.Rows.Count > 0)
            {
                return MapRowToPerson(dt.Rows[0]);
            }
            return null;
        }

        public List<Person> GetAllPeople()
        {
            string query = "SELECT * FROM People WHERE IsDeleted = 0 ORDER BY PersonID DESC";
            DataTable dt = DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection());

            return MapTableToPersonList(dt);
        }

        public List<Person> GetPeoplePaged(int pageNumber, int pageSize)
        {
            int offSet = (pageNumber - 1) * pageSize;
            string query = @"SELECT * FROM People 
                         WHERE IsDeleted = 0 
                         ORDER BY PersonID 
                         OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@Offset", offSet),
                new SqlParameter("@PageSize", pageSize)
            };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            
            return MapTableToPersonList(dt);
        }

        public List<Person> SearchPeople(string keyWord)
        {
            string query = @"SELECT * FROM People 
                         WHERE IsDeleted = 0 
                         AND (FirstName LIKE @Search OR LastName LIKE @Search 
                              OR ContactNumber LIKE @Search OR Email LIKE @Search)";

            SqlParameter[] parameters = { new SqlParameter("@Search", "%" + keyWord + "%") };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return MapTableToPersonList(dt);
        }

        public Person GetPersonByEmail(string email)
        {
            string query = "SELECT * FROM People WHERE Email = @Email AND IsDeleted = 0";
            SqlParameter[] parameters = { new SqlParameter("@Email", email) };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());

            return dt.Rows.Count > 0 ? MapRowToPerson(dt.Rows[0]) : null;
        }

        public int GetTotalPeopleCount()
        {
            string query = "SELECT COUNT(1) FROM People WHERE IsDeleted = 0";
            object result = DBHelper.ExecuteScalar(query, null, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int UpdateContactInfo(int personId, string phone, string email)
        {
            string query = "UPDATE People SET ContactNumber = @Phone, Email = @Email WHERE PersonID = @ID";


            SqlParameter[] parameters = {
                new SqlParameter("@ID", personId),
                new SqlParameter("@Phone", phone),
                new SqlParameter("@Email", email)
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int RestorePerson(int personId)
        {
            string query = "UPDATE People SET IsDeleted = 0 WHERE PersonID = @PersonID";
            SqlParameter[] parameters = { new SqlParameter("@PersonID", personId) };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int PersonExists(int personId)
        {
            string query = "SELECT COUNT(1) FROM People WHERE PersonID = @PersonID";
            SqlParameter[] parameters = { new SqlParameter("@PersonID", personId) };
            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int EmailExists(string email, int excludePersonId = -1, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            string query = @"SELECT COUNT(1) 
                     FROM People 
                     WHERE Email = @Email 
                     AND PersonID <> @ExcludeID 
                     AND IsDeleted = 0";

            SqlParameter[] parameters = {
                new SqlParameter("@Email", (object)email ?? DBNull.Value),
                new SqlParameter("@ExcludeID", excludePersonId)
             };

            SqlConnection connToUse = connection ?? DBHelper.GetOpenConnection();

            object result = DBHelper.ExecuteScalar(query, parameters, connToUse, transaction);

            return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
        }

        public int ContactNumberExists(string contactNumber, int excludePersonId = 0)
        {
            string query = "SELECT 1 FROM People WHERE ContactNumber = @ContactNumber AND PersonID != @ExcludeID AND IsDeleted = 0";
            SqlParameter[] parameters = {
            new SqlParameter("@ContactNumber", contactNumber),
            new SqlParameter("@ExcludeID", excludePersonId) };
            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        private List<Person> MapTableToPersonList(DataTable dt)
        {
            List<Person> peopleList = new List<Person>();
            foreach (DataRow row in dt.Rows)
            {
                peopleList.Add(MapRowToPerson(row));
            }
            return peopleList;
        }

        private Person MapRowToPerson(DataRow row)
        {
            return new Person
            {
                PersonId = (int)row["PersonID"],
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                DateOfBirth = (DateTime)row["DateOfBirth"],
                Gender = (enGender)row["Gender"],
                ContactNumber = row["ContactNumber"].ToString(),
                Email = row["Email"].ToString(),
                Address = row["Address"]?.ToString(),
                CreatedAt = (DateTime)row["CreatedAt"],
                IsDeleted = (bool)row["IsDeleted"]
            };
        }
    }
}
