using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using BCrypt.Net;

namespace Online_Shop_Pet_Project
{
    public static class DBManager
    {
        private static string dbPath = Path.Combine(Environment.CurrentDirectory, "OnlineShop.db");
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Email TEXT,
                        Phone TEXT,
                        PasswordHash TEXT NOT NULL,
                        IsEmployee BOOLEAN NOT NULL DEFAULT 0,
                        DocumentsPath TEXT,
                        PhotoPath TEXT,
                        RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";

                    using (var command = new SQLiteCommand(createUsersTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static bool RegisterUser(string username, string email, string phone, string password,
                                      bool isEmployee, string documentsPath = null, string photoPath = null)
        {
            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = @"
                    INSERT INTO Users (Username, Email, Phone, PasswordHash, IsEmployee, DocumentsPath, PhotoPath)
                    VALUES (@username, @email, @phone, @passwordHash, @isEmployee, @documentsPath, @photoPath)";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(email) ? DBNull.Value : (object)email);
                        command.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone);
                        command.Parameters.AddWithValue("@passwordHash", passwordHash);
                        command.Parameters.AddWithValue("@isEmployee", isEmployee);
                        command.Parameters.AddWithValue("@documentsPath", string.IsNullOrEmpty(documentsPath) ? DBNull.Value : (object)documentsPath);
                        command.Parameters.AddWithValue("@photoPath", string.IsNullOrEmpty(photoPath) ? DBNull.Value : (object)photoPath);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}");
                return false;
            }
        }

        public static (bool success, bool isEmployee) AuthenticateUser(string usernameOrEmail, string password)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = @"
                    SELECT PasswordHash, IsEmployee FROM Users 
                    WHERE Username = @username OR Email = @username";

                    using (var command = new SQLiteCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@username", usernameOrEmail);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["PasswordHash"].ToString();
                                bool isEmployee = Convert.ToBoolean(reader["IsEmployee"]);

                                return (BCrypt.Net.BCrypt.Verify(password, storedHash), isEmployee);
                            }
                        }
                    }
                }
                return (false, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка аутентификации: {ex.Message}");
                return (false, false);
            }
        }
        public static UserProfile LoadUserProfile(string username)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT Username, Email, Phone, PhotoPath, IsEmployee 
                FROM Users 
                WHERE Username = @username";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserProfile
                                {
                                    Name = reader["Username"].ToString(),
                                    Email = reader["Email"]?.ToString(),
                                    Phone = reader["Phone"]?.ToString(),
                                    PhotoPath = reader["PhotoPath"]?.ToString() ?? "default_profile.png",
                                    Password = "********",
                                    IsEmployee = Convert.ToBoolean(reader["IsEmployee"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки профиля: {ex.Message}");
            }

            return new UserProfile
            {
                Name = username,
                Email = "",
                Phone = "",
                PhotoPath = "default_profile.png",
                Password = "********",
                IsEmployee = false
            };
        }
        public static string GetUsernameByLogin(string login)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Username FROM Users WHERE Username = @login OR Email = @login";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);

                        object result = command.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения имени пользователя: {ex.Message}");
                return null;
            }
        }
        public static bool UserExists(string username, string email)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                    SELECT COUNT(*) FROM Users 
                    WHERE Username = @username OR Email = @email";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(email) ? DBNull.Value : (object)email);

                        return (long)command.ExecuteScalar() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проверки пользователя: {ex.Message}");
                return false;
            }
        }
    }
}