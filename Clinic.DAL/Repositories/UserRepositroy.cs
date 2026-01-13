using Clinic.Entities;
using Clinic.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
namespace Clinic.DAL.Repositories
{
    public class clsUserRepositroy
    {
        public int AddUser(User user, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            string query = @"INSERT INTO Users (UserID, UserName, PasswordHash, Role, IsActive, LastLogin)
                    VALUES(@UserID, @UserName, @PasswordHash, @Role, @IsActive, @LastLogin)";

            SqlParameter[] parameters =
            {
                new SqlParameter("@UserID", user.UserId),
                new SqlParameter("@UserName", user.UserName),
                new SqlParameter("@PasswordHash", user.PasswordHash),
                new SqlParameter("@Role", user.Role),
                new SqlParameter("@IsActive", user.IsActive),
                new SqlParameter("@LastLogin", (object)user.LastLogin ?? DBNull.Value),
            };

            SqlConnection connToUse = connection ?? DBHelper.GetOpenConnection();

            return DBHelper.ExecuteNonQuery(query, parameters, connToUse, transaction);
        }
        
        public int UpdateUsername(int userId, string newUsername)
        {
            string query = "UPDATE Users SET Username = @Username WHERE UserID = @UserID";

            SqlParameter[] parameters =
            {
                new SqlParameter ("@UserID", userId),
                new SqlParameter ("@Username", newUsername),
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int UpdateUserRole(int userId, enRole newRole)
        {
            string query = "UPDATE dbo.Users SET Role = @Role WHERE UserID = @UserID";

            SqlParameter[] parameters =
            {
                new SqlParameter ("@UserID", userId),
                new SqlParameter ("@Role", newRole)
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int ChangePassword(int userId, string newPasswordHash)
        {
            string query = "UPDATE dbo.Users SET PasswordHash = @PasswordHash WHERE UserID = @UserID";

            SqlParameter[] parameters =
            {
                new SqlParameter ("@UserID", userId),
                new SqlParameter ("@PasswordHash", newPasswordHash)
            };

            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public void UpdateLastLogin(int userId)
        {
            string query = "UPDATE dbo.Users SET LastLogin = GETDATE() WHERE UserID = @UserID";
            SqlParameter[] parameters =
            {
                new SqlParameter ("@UserID", userId),
            };

            DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int ActivateUser(int userId)
        {
            string query = "UPDATE Users SET IsActive = 1 WHERE UserID = @UserID";
            SqlParameter[] parameters =
            {
                new SqlParameter ("@UserID", userId),
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public int DeactivateUser(int userId)
        {
            string query = "UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";
            SqlParameter[] parameters =
            {
                new SqlParameter ("@UserID", userId),
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public User GetUserById(int userId)
        {
            string query = @"SELECT u.*, p.FirstName, p.LastName, p.Email, p.ContactNumber, p.IsDeleted 
                             FROM Users u JOIN People p ON u.UserID = p.PersonID 
                             WHERE u.UserID = @UserID";
            SqlParameter[] parameters =
            {
                new SqlParameter ("@UserID", userId),
            };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return dt.Rows.Count > 0 ? MapRowToUser(dt.Rows[0]) : null;
        }

        public List<User> GetActiveUsers()
        {
            string query = @"SELECT u.*, p.FirstName, p.LastName, p.Email 
                             FROM dbo.Users u JOIN dbo.People p ON u.UserID = p.PersonID 
                             WHERE u.IsActive = 1 AND p.IsDeleted = 0";
            DataTable dt = DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection());
            return MapTableToUserList(dt);
        }

        public List<User> GetUsersPaged(int pageNumber, int pageSize)
        {
            int offSet = (pageNumber - 1) * pageSize;
            string query = @"SELECT u.*, p.FirstName, p.LastName, p.Email, p.ContactNumber 
                             FROM dbo.Users u JOIN dbo.People p ON u.UserID = p.PersonID 
                             WHERE p.IsDeleted = 0 
                             ORDER BY u.UserID 
                             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            SqlParameter[] parameters = {
                new SqlParameter("@Offset", offSet),
                new SqlParameter("@PageSize", pageSize)
            };
            return MapTableToUserList(DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection()));
        }

        public User GetUserByUsername(string username)
        {
            string query = @"SELECT u.*, p.FirstName, p.LastName, p.Email, p.ContactNumber 
                     FROM dbo.Users u 
                     JOIN dbo.People p ON u.UserID = p.PersonID 
                     WHERE u.Username = @Username AND u.IsActive = 1 AND p.IsDeleted = 0";

            SqlParameter[] parameters =
            {
                new SqlParameter ("@Username", username),
            };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters, DBHelper.GetOpenConnection());
            return dt.Rows.Count > 0 ? MapRowToUser(dt.Rows[0]) : null;
        }

        public int UserExists(int userId)
        {
            string query = "SELECT 1 FROM Users WHERE UserID = @UserID";

            SqlParameter[] parameters =
            {
                new SqlParameter ("@UserID", userId),
            };
            
            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int UsernameExists(string username, int excludeUserId = 0, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            string query = "SELECT 1 FROM dbo.Users WHERE Username = @Username AND UserID != @ExcludeID";
            SqlParameter[] parameters = {
                new SqlParameter ("@ExcludeID", excludeUserId),
                new SqlParameter ("@Username", username)
            };

            object result = DBHelper.ExecuteScalar(query, parameters, connection ?? DBHelper.GetOpenConnection(), transaction);

            return result != null ? 1 : 0;
        }

        public int IsUserActive(int userId)
        {
            string query = "SELECT IsActive FROM dbo.Users WHERE UserID = @UserID";
            SqlParameter[] parameters =
            {
                new SqlParameter ("@UserID", userId),
            };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());

            return result != null ? Convert.ToInt32(result) : 0;
        }

        private List<User> MapTableToUserList(DataTable dt)
        {
            List<User> list = new List<User>();
            foreach (DataRow row in dt.Rows) list.Add(MapRowToUser(row));
            return list;
        }

        private User MapRowToUser(DataRow row)
        {
            return new User
            {
                UserId = row["UserID"] != DBNull.Value ? (int)row["UserID"] : 0,
                UserName = row["Username"]?.ToString() ?? "",
                PasswordHash = row["PasswordHash"]?.ToString() ?? "",
                Role = row["Role"] != DBNull.Value ? (enRole)Convert.ToInt32(row["Role"]) : 0,
                IsActive = row["IsActive"] != DBNull.Value && (bool)row["IsActive"],
                LastLogin = row["LastLogin"] != DBNull.Value ? (DateTime)row["LastLogin"] : DateTime.MinValue,
                FirstName = row["FirstName"]?.ToString() ?? "",
                LastName = row["LastName"]?.ToString() ?? "",
                Email = row["Email"]?.ToString() ?? ""
            };
        }
    }
}