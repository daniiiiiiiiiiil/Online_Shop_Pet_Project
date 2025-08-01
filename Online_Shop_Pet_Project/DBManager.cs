using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace Online_Shop_Pet_Project
{
    internal static class DbManager
    {
        private static string connectionString = "Data Source=database.db;Version=3;";

        static DbManager()
        {
            InitializeDatabase();
        }

        private static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Создаем таблицу пользователей, если её нет
                string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Salt TEXT NOT NULL,
                    Email TEXT,
                    Phone TEXT,
                    IsEmployee INTEGER DEFAULT 0,
                    RegistrationDate TEXT DEFAULT CURRENT_TIMESTAMP
                )";

                using (var command = new SQLiteCommand(createUsersTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool RegisterUser(string username, string password, string email = null, string phone = null, bool isEmployee = false)
        {
            if (UserExists(username))
                return false;

            var salt = PasswordHasher.GenerateSalt();
            var passwordHash = PasswordHasher.HashPassword(password, salt);

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertUser = @"
                INSERT INTO Users (Username, PasswordHash, Salt, Email, Phone, IsEmployee)
                VALUES (@username, @passwordHash, @salt, @email, @phone, @isEmployee)";

                using (var command = new SQLiteCommand(insertUser, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@passwordHash", passwordHash);
                    command.Parameters.AddWithValue("@salt", salt);
                    command.Parameters.AddWithValue("@email", email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@phone", phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@isEmployee", isEmployee ? 1 : 0);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public static bool AuthenticateUser(string username, string password)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectUser = @"
                SELECT PasswordHash, Salt, IsEmployee 
                FROM Users 
                WHERE Username = @username";

                using (var command = new SQLiteCommand(selectUser, connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader["PasswordHash"].ToString();
                            string salt = reader["Salt"].ToString();
                            bool isEmployee = Convert.ToInt32(reader["IsEmployee"]) == 1;

                            string inputHash = PasswordHasher.HashPassword(password, salt);

                            return inputHash == storedHash;
                        }
                    }
                }
            }

            return false;
        }

        public static bool UserExists(string username)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string checkUser = "SELECT 1 FROM Users WHERE Username = @username LIMIT 1";

                using (var command = new SQLiteCommand(checkUser, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    return command.ExecuteScalar() != null;
                }
            }
        }

        public static bool IsEmployee(string username)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string checkRole = "SELECT IsEmployee FROM Users WHERE Username = @username LIMIT 1";

                using (var command = new SQLiteCommand(checkRole, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    var result = command.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) == 1;
                }
            }
        }
    }

    internal static class PasswordHasher
    {
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
