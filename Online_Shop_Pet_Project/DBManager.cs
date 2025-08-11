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

        static DBManager()
        {
            InitializeDatabase();
        }

        private static void InitializeDatabase()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Email TEXT UNIQUE,
                    Phone TEXT UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    IsEmployee BOOLEAN NOT NULL DEFAULT 0,
                    DocumentsPath TEXT,
                    PhotoPath TEXT,
                    RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

                string createProfilesTable = @"
                CREATE TABLE IF NOT EXISTS Profiles (
                    UserId INTEGER PRIMARY KEY,
                    FullName TEXT,
                    Address TEXT,
                    PaymentMethod TEXT,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                )";

                string createEmployeesTable = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    UserId INTEGER PRIMARY KEY,
                    Position TEXT,
                    Salary REAL,
                    ShiftsWorked INTEGER DEFAULT 0,
                    TotalEarned REAL DEFAULT 0,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                )";

                using (var command = new SQLiteCommand(createUsersTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createProfilesTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createEmployeesTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool RegisterUser(string username, string email, string phone, string password,
                                      bool isEmployee, string documentsPath = null, string photoPath = null)
        {
            try
            {
                if (UserExists(username, email, phone))
                {
                    string conflictField = GetConflictField(username, email, phone);
                    MessageBox.Show($"Пользователь с таким {conflictField} уже существует",
                                  "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string insertUser = @"
                                INSERT INTO Users (Username, Email, Phone, PasswordHash, IsEmployee, DocumentsPath, PhotoPath)
                                VALUES (@username, @email, @phone, @passwordHash, @isEmployee, @documentsPath, @photoPath)";

                            using (var command = new SQLiteCommand(insertUser, connection))
                            {
                                command.Parameters.AddWithValue("@username", username);
                                command.Parameters.AddWithValue("@email", email ?? "");
                                command.Parameters.AddWithValue("@phone", phone ?? "");
                                command.Parameters.AddWithValue("@passwordHash", passwordHash);
                                command.Parameters.AddWithValue("@isEmployee", isEmployee ? 1 : 0);
                                command.Parameters.AddWithValue("@documentsPath", documentsPath ?? "");
                                command.Parameters.AddWithValue("@photoPath", photoPath ?? "");
                                command.ExecuteNonQuery();
                            }

                            int userId = (int)connection.LastInsertRowId;

                            string insertProfile = @"
                                INSERT INTO Profiles (UserId, FullName)
                                VALUES (@userId, @fullName)";

                            using (var command = new SQLiteCommand(insertProfile, connection))
                            {
                                command.Parameters.AddWithValue("@userId", userId);
                                command.Parameters.AddWithValue("@fullName", username);
                                command.ExecuteNonQuery();
                            }

                            if (isEmployee)
                            {
                                string insertEmployee = @"
                                    INSERT INTO Employees (UserId, Position, Salary)
                                    VALUES (@userId, 'Новый сотрудник', 30000)";

                                using (var command = new SQLiteCommand(insertEmployee, connection))
                                {
                                    command.Parameters.AddWithValue("@userId", userId);
                                    command.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private static string GetConflictField(string username, string email, string phone)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                if (!string.IsNullOrEmpty(username))
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                            return "именем пользователя";
                    }
                }

                if (!string.IsNullOrEmpty(email))
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE Email = @email";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                            return "email";
                    }
                }

                if (!string.IsNullOrEmpty(phone))
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE Phone = @phone";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@phone", phone);
                        if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                            return "номером телефона";
                    }
                }

                return "данными";
            }
        }
        public static bool UserExists(string username, string email, string phone)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT COUNT(*) 
                FROM Users 
                WHERE (Username = @username AND @username != '')
                   OR (Email = @email AND @email != '')
                   OR (Phone = @phone AND @phone != '')";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username ?? "");
                        command.Parameters.AddWithValue("@email", email ?? "");
                        command.Parameters.AddWithValue("@phone", phone ?? "");

                        return Convert.ToInt32(command.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проверки пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }

        public static (bool success, bool isEmployee, int userId) AuthenticateUser(string usernameOrEmail, string password)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = @"
                        SELECT Id, PasswordHash, IsEmployee 
                        FROM Users 
                        WHERE Username = @usernameOrEmail OR Email = @usernameOrEmail";

                    using (var command = new SQLiteCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@usernameOrEmail", usernameOrEmail);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["PasswordHash"].ToString();
                                bool isEmployee = Convert.ToBoolean(reader["IsEmployee"]);
                                int userId = Convert.ToInt32(reader["Id"]);

                                if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                                {
                                    return (true, isEmployee, userId);
                                }
                            }
                        }
                    }
                }
                return (false, false, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка аутентификации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false, false, 0);
            }
        }

        public static UserProfile LoadUserProfile(int userId)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT u.Username, u.Email, u.Phone, u.PhotoPath, u.IsEmployee, 
                               p.FullName, p.Address, p.PaymentMethod
                        FROM Users u
                        LEFT JOIN Profiles p ON u.Id = p.UserId
                        WHERE u.Id = @userId";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserProfile
                                {
                                    Id = userId,
                                    Name = reader["FullName"].ToString(),
                                    Email = reader["Email"]?.ToString(),
                                    Phone = reader["Phone"]?.ToString(),
                                    PhotoPath = reader["PhotoPath"]?.ToString() ?? "default_profile.png",
                                    IsEmployee = Convert.ToBoolean(reader["IsEmployee"]),
                                    Address = reader["Address"]?.ToString(),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки профиля: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        public static bool UpdateUserProfile(UserProfile profile)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string updateUser = @"
                                UPDATE Users 
                                SET Email = @email, Phone = @phone, PhotoPath = @photoPath
                                WHERE Id = @userId";

                            using (var command = new SQLiteCommand(updateUser, connection))
                            {
                                command.Parameters.AddWithValue("@email", profile.Email ?? "");
                                command.Parameters.AddWithValue("@phone", profile.Phone ?? "");
                                command.Parameters.AddWithValue("@photoPath", profile.PhotoPath ?? "");
                                command.Parameters.AddWithValue("@userId", profile.Id);
                                command.ExecuteNonQuery();
                            }

                            string updateProfile = @"
                                UPDATE Profiles 
                                SET FullName = @fullName, Address = @address, PaymentMethod = @paymentMethod
                                WHERE UserId = @userId";

                            using (var command = new SQLiteCommand(updateProfile, connection))
                            {
                                command.Parameters.AddWithValue("@fullName", profile.Name ?? "");
                                command.Parameters.AddWithValue("@address", profile.Address ?? "");
                                command.Parameters.AddWithValue("@paymentMethod", profile.PaymentMethod ?? "");
                                command.Parameters.AddWithValue("@userId", profile.Id);
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления профиля: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
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
                        return command.ExecuteScalar()?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения имени пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static EmployeeInfo GetEmployeeInfo(int userId)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT Position, Salary, ShiftsWorked, TotalEarned 
                        FROM Employees 
                        WHERE UserId = @userId";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new EmployeeInfo
                                {
                                    Position = reader["Position"].ToString(),
                                    Salary = Convert.ToDecimal(reader["Salary"]),
                                    ShiftsWorked = Convert.ToInt32(reader["ShiftsWorked"]),
                                    TotalEarned = Convert.ToDecimal(reader["TotalEarned"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки информации о сотруднике: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }
    }
}