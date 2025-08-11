using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class ProfileHelper
    {
        private MainMenuForm form;
        private SQLiteConnection connection;

        public ProfileHelper(MainMenuForm form)
        {
            this.form = form;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            connection = new SQLiteConnection("Data Source=online_shop.db;Version=3;");
            connection.Open();

            // Создаем таблицу Users, если она не существует
            using (var cmd = new SQLiteCommand(
                @"CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT,
            Phone TEXT,
            Email TEXT,
            Password TEXT,
            PhotoPath TEXT,
            IsEmployee INTEGER DEFAULT 0,
            PaymentMethod TEXT
        )", connection))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SQLiteCommand(
                @"CREATE TABLE IF NOT EXISTS Employees (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER,
            Position TEXT,
            Salary REAL,
            ShiftsWorked INTEGER,
            TotalEarned REAL,
            FOREIGN KEY(UserId) REFERENCES Users(Id)
        )", connection))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SQLiteCommand(
                @"CREATE TABLE IF NOT EXISTS SupportTickets (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER,
            Subject TEXT,
            Category TEXT,
            Priority TEXT,
            Status TEXT,
            Date DATETIME,
            Description TEXT,
            Answer TEXT,
            ResponseDate DATETIME,
            FOREIGN KEY(UserId) REFERENCES Users(Id)
        )", connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void ShowProfilePanel()
        {
            form.UIHelper.ClearPanels();

            form.profilePanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Личный кабинет",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 10)
            };
            form.profilePanel.Controls.Add(title);

            var profileContainer = new Panel
            {
                Location = new Point(20, 40),
                Size = new Size(form.ClientSize.Width - 40, form.ClientSize.Height - 120),
                BackColor = Color.White
            };

            var topPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(profileContainer.Width, 150),
                BackColor = Color.White
            };

            var photoPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(120, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            var photoPicture = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(110, 110),
                Location = new Point(5, 5),
                Image = form.UIHelper.LoadImageOrDefault(form.userProfile.PhotoPath, 110, 110)
            };
            photoPanel.Controls.Add(photoPicture);

            var changePhotoButton = new Button
            {
                Text = "Изменить фото",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 30),
                Location = new Point(5, 115),
                Font = new Font("Segoe UI", 8)
            };
            changePhotoButton.FlatAppearance.BorderSize = 0;
            changePhotoButton.Click += (s, e) => ChangeProfilePhoto();
            photoPanel.Controls.Add(changePhotoButton);

            topPanel.Controls.Add(photoPanel);

            var infoPanel = new Panel
            {
                Location = new Point(130, 0),
                Size = new Size(profileContainer.Width - 130, 150),
                BackColor = Color.White
            };

            var personalInfoLabel = new Label
            {
                Text = "Личная информация",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(0, 0)
            };
            infoPanel.Controls.Add(personalInfoLabel);

            var nameLabel = new Label
            {
                Text = "Имя:",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 30)
            };
            infoPanel.Controls.Add(nameLabel);

            var nameValue = new TextBox
            {
                Text = form.userProfile.Name,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 30),
                BorderStyle = BorderStyle.FixedSingle
            };
            infoPanel.Controls.Add(nameValue);

            var phoneLabel = new Label
            {
                Text = "Телефон:",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 60)
            };
            infoPanel.Controls.Add(phoneLabel);

            var phoneValue = new TextBox
            {
                Text = form.userProfile.Phone,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 60),
                BorderStyle = BorderStyle.FixedSingle
            };
            infoPanel.Controls.Add(phoneValue);

            var emailLabel = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 90)
            };
            infoPanel.Controls.Add(emailLabel);

            var emailValue = new TextBox
            {
                Text = form.userProfile.Email,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 90),
                BorderStyle = BorderStyle.FixedSingle
            };
            infoPanel.Controls.Add(emailValue);

            var passwordLabel = new Label
            {
                Text = "Пароль:",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 120)
            };
            infoPanel.Controls.Add(passwordLabel);

            var passwordValue = new TextBox
            {
                Text = form.userProfile.Password,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 120),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '*'
            };
            infoPanel.Controls.Add(passwordValue);

            var saveProfileButton = new Button
            {
                Text = "Сохранить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 25),
                Location = new Point(infoPanel.Width - 130, 120),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            saveProfileButton.FlatAppearance.BorderSize = 0;
            saveProfileButton.Click += (s, e) =>
            {
                form.userProfile.Name = nameValue.Text;
                form.userProfile.Phone = phoneValue.Text;
                form.userProfile.Email = emailValue.Text;
                form.userProfile.Password = passwordValue.Text;

                SaveUserProfile(form.userProfile);

                MessageBox.Show("Изменения сохранены!", "Профиль", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            infoPanel.Controls.Add(saveProfileButton);

            topPanel.Controls.Add(infoPanel);
            profileContainer.Controls.Add(topPanel);

            var sectionsPanel = new Panel
            {
                Location = new Point(0, 160),
                Size = new Size(profileContainer.Width, profileContainer.Height - 160),
                BackColor = Color.White
            };

            int buttonWidth = (sectionsPanel.Width - 60) / 3;
            int buttonHeight = 60;

            // Кнопка "Ответ на заявку" (только для сотрудников)
            if (form.userProfile.IsEmployee)
            {
                var answerSupportBtn = new Button
                {
                    Text = "Ответ на заявку",
                    BackColor = Color.FromArgb(240, 240, 240),
                    ForeColor = Color.Black,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(buttonWidth, buttonHeight),
                    Location = new Point(20, 20),
                    Font = new Font("Segoe UI", 9)
                };
                answerSupportBtn.Click += (s, e) => ShowSupportResponseForm();
                sectionsPanel.Controls.Add(answerSupportBtn);
            }

            // Кнопка "Чат с тех поддержкой"
            var chatSupportBtn = new Button
            {
                Text = "Чат с тех поддержкой",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(30 + buttonWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            chatSupportBtn.Click += (s, e) => form.SupportHelper.ShowChatWithSupport();
            sectionsPanel.Controls.Add(chatSupportBtn);

            // Кнопка "Способ оплаты"
            var paymentMethodBtn = new Button
            {
                Text = "Способ оплаты",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(40 + buttonWidth * 2, 20),
                Font = new Font("Segoe UI", 9)
            };
            paymentMethodBtn.Click += (s, e) => ShowPaymentMethodForm();
            sectionsPanel.Controls.Add(paymentMethodBtn);

            // Кнопка "Часто задаваемые вопросы"
            var supportQuestionsBtn = new Button
            {
                Text = "Часто задаваемые вопросы",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(20, 90),
                Font = new Font("Segoe UI", 9)
            };
            supportQuestionsBtn.Click += (s, e) => form.SupportHelper.ShowSupportHelpPanel();
            sectionsPanel.Controls.Add(supportQuestionsBtn);

            // Кнопка "Подать заявку"
            var supportTicketBtn = new Button
            {
                Text = "Подать заявку",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(30 + buttonWidth, 90),
                Font = new Font("Segoe UI", 9)
            };
            supportTicketBtn.Click += (s, e) => form.SupportHelper.ShowNewTicketForm();
            sectionsPanel.Controls.Add(supportTicketBtn);

            // Кнопка "История заказов"
            var ordersHistoryBtn = new Button
            {
                Text = "История заказов",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(40 + buttonWidth * 2, 90),
                Font = new Font("Segoe UI", 9)
            };
            ordersHistoryBtn.Click += (s, e) => form.OrderHelper.ShowOrdersPanel();
            sectionsPanel.Controls.Add(ordersHistoryBtn);

            // Добавляем панель сотрудника, если пользователь - сотрудник
            if (form.userProfile.IsEmployee)
            {
                var employeeStats = GetEmployeeStats(form.userProfile.Id);

                // Проверяем, что employeeStats не null
                if (employeeStats != null)
                {
                    var employeePanel = new Panel
                    {
                        Location = new Point(20, 165),
                        Size = new Size(profileContainer.Width - 40, 120),
                        BackColor = Color.FromArgb(240, 248, 255),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    var employeeTitle = new Label
                    {
                        Text = "Информация сотрудника",
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        ForeColor = Color.FromArgb(70, 130, 180),
                        AutoSize = true,
                        Location = new Point(10, 10)
                    };
                    employeePanel.Controls.Add(employeeTitle);

                    // Добавляем проверку на null для каждого поля
                    var positionLabel = new Label
                    {
                        Text = $"Должность: {employeeStats.Position ?? "Не указана"}",
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        Location = new Point(10, 35)
                    };
                    employeePanel.Controls.Add(positionLabel);

                    var salaryLabel = new Label
                    {
                        Text = $"Зарплата: {employeeStats.Salary:C}",
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        Location = new Point(10, 55)
                    };
                    employeePanel.Controls.Add(salaryLabel);

                    var shiftsLabel = new Label
                    {
                        Text = $"Отработано смен: {employeeStats.ShiftsWorked}",
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        Location = new Point(10, 75)
                    };
                    employeePanel.Controls.Add(shiftsLabel);

                    var earnedLabel = new Label
                    {
                        Text = $"Всего заработано: {employeeStats.TotalEarned:C}",
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        Location = new Point(10, 95)
                    };
                    employeePanel.Controls.Add(earnedLabel);

                    sectionsPanel.Controls.Add(employeePanel);
                }
            }

            profileContainer.Controls.Add(sectionsPanel);
            form.profilePanel.Controls.Add(profileContainer);
            form.Controls.Add(form.profilePanel);
        }
        private void ShowSupportResponseForm()
        {
            var responseForm = new Form
            {
                Text = "Ответ на заявку клиента",
                Size = new Size(500, 300),
                StartPosition = FormStartPosition.CenterParent
            };

            var ticketLabel = new Label
            {
                Text = "Номер заявки:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            var ticketBox = new TextBox
            {
                Location = new Point(150, 20),
                Size = new Size(100, 20)
            };

            var responseLabel = new Label
            {
                Text = "Ответ:",
                Location = new Point(20, 60),
                AutoSize = true
            };
            var responseBox = new TextBox
            {
                Multiline = true,
                Location = new Point(20, 90),
                Size = new Size(440, 120),
                ScrollBars = ScrollBars.Vertical
            };

            var submitResponseBtn = new Button
            {
                Text = "Отправить ответ",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(20, 220)
            };
            submitResponseBtn.Click += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(responseBox.Text))
                {
                    SaveSupportResponse(Convert.ToInt32(ticketBox.Text), responseBox.Text);
                    MessageBox.Show($"Ответ на заявку #{ticketBox.Text} отправлен!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    responseForm.Close();
                }
            };

            responseForm.Controls.AddRange(new Control[] {
        ticketLabel, ticketBox, responseLabel, responseBox, submitResponseBtn
    });
            responseForm.ShowDialog();
        }

        private void ShowPaymentMethodForm()
        {
            var paymentForm = new Form
            {
                Text = "Выбор способа оплаты",
                Size = new Size(300, 200),
                StartPosition = FormStartPosition.CenterParent
            };

            var paymentLabel = new Label
            {
                Text = "Выберите способ оплаты:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            var cardOnlineRadio = new RadioButton
            {
                Text = "Банковской картой онлайн",
                Location = new Point(20, 50),
                AutoSize = true,
                Checked = form.userProfile.PaymentMethod == "Картой онлайн"
            };

            var cardOnDeliveryRadio = new RadioButton
            {
                Text = "Банковской картой при получении",
                Location = new Point(20, 80),
                AutoSize = true,
                Checked = form.userProfile.PaymentMethod == "Картой при получении"
            };

            var cashRadio = new RadioButton
            {
                Text = "Наличными при получении",
                Location = new Point(20, 110),
                AutoSize = true,
                Checked = form.userProfile.PaymentMethod == "Наличными"
            };

            var savePaymentBtn = new Button
            {
                Text = "Сохранить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(100, 150)
            };
            savePaymentBtn.Click += (sender, args) =>
            {
                string paymentMethod = cardOnlineRadio.Checked ? "Картой онлайн" :
                                    cardOnDeliveryRadio.Checked ? "Картой при получении" : "Наличными";

                SavePaymentMethod(form.userProfile.Id, paymentMethod);

                MessageBox.Show($"Способ оплаты '{paymentMethod}' сохранен", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                paymentForm.Close();
            };

            paymentForm.Controls.AddRange(new Control[] {
        paymentLabel, cardOnlineRadio, cardOnDeliveryRadio, cashRadio, savePaymentBtn
    });
            paymentForm.ShowDialog();
        }

        private void SaveSupportResponse(int ticketId, string response)
        {
            using (var cmd = new SQLiteCommand(
                @"UPDATE SupportTickets 
          SET Answer = @Answer, 
              Status = 'Ответ отправлен',
              ResponseDate = @ResponseDate
          WHERE Id = @Id", connection))
            {
                cmd.Parameters.AddWithValue("@Answer", response);
                cmd.Parameters.AddWithValue("@ResponseDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@Id", ticketId);
                cmd.ExecuteNonQuery();
            }
        }

        private void SavePaymentMethod(int userId, string paymentMethod)
        {
            try
            {
                // Проверяем существование таблицы Users
                using (var checkCmd = new SQLiteCommand(
                    "SELECT name FROM sqlite_master WHERE type='table' AND name='Users'", connection))
                {
                    var tableExists = checkCmd.ExecuteScalar() != null;

                    if (!tableExists)
                    {
                        // Если таблицы нет, создаем ее
                        using (var createCmd = new SQLiteCommand(
                            @"CREATE TABLE Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Phone TEXT,
                        Email TEXT,
                        Password TEXT,
                        PhotoPath TEXT,
                        IsEmployee INTEGER DEFAULT 0,
                        PaymentMethod TEXT
                    )", connection))
                        {
                            createCmd.ExecuteNonQuery();
                        }
                    }
                }

                // Обновляем способ оплаты
                using (var cmd = new SQLiteCommand(
                    @"UPDATE Users 
              SET PaymentMethod = @PaymentMethod
              WHERE Id = @UserId", connection))
                {
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }

                // Обновляем профиль в памяти
                form.userProfile.PaymentMethod = paymentMethod;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении способа оплаты: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveUserProfile(UserProfile profile)
        {
            try
            {
                using (var cmd = new SQLiteCommand(
                    @"INSERT OR REPLACE INTO Users 
              (Id, Name, Phone, Email, Password, PhotoPath, IsEmployee, PaymentMethod) 
              VALUES (@Id, @Name, @Phone, @Email, @Password, @PhotoPath, @IsEmployee, @PaymentMethod)", connection))
                {
                    cmd.Parameters.AddWithValue("@Id", profile.Id);
                    cmd.Parameters.AddWithValue("@Name", profile.Name);
                    cmd.Parameters.AddWithValue("@Phone", profile.Phone);
                    cmd.Parameters.AddWithValue("@Email", profile.Email);
                    cmd.Parameters.AddWithValue("@Password", profile.Password);
                    cmd.Parameters.AddWithValue("@PhotoPath", profile.PhotoPath);
                    cmd.Parameters.AddWithValue("@IsEmployee", profile.IsEmployee ? 1 : 0);
                    cmd.Parameters.AddWithValue("@PaymentMethod", profile.PaymentMethod ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении профиля: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private EmployeeStats GetEmployeeStats(int userId)
        {
            // Если пользователь не сотрудник - сразу возвращаем null
            if (!form.userProfile.IsEmployee)
                return null;

            try
            {
                using (var cmd = new SQLiteCommand(
                    "SELECT Position, Salary, ShiftsWorked, TotalEarned FROM Employees WHERE UserId = @UserId",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new EmployeeStats
                            {
                                Position = reader["Position"]?.ToString() ?? "Не указана",
                                Salary = reader["Salary"] != DBNull.Value ? Convert.ToDecimal(reader["Salary"]) : 0,
                                ShiftsWorked = reader["ShiftsWorked"] != DBNull.Value ? Convert.ToInt32(reader["ShiftsWorked"]) : 0,
                                TotalEarned = reader["TotalEarned"] != DBNull.Value ? Convert.ToDecimal(reader["TotalEarned"]) : 0
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных сотрудника: {ex.Message}");
            }

            // Возвращаем объект с значениями по умолчанию, если запись не найдена
            return new EmployeeStats
            {
                Position = "Новый сотрудник",
                Salary = 30000,
                ShiftsWorked = 0,
                TotalEarned = 0
            };
        }

        public void ChangeProfilePhoto()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите фото профиля"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    form.userProfile.PhotoPath = openFileDialog.FileName;
                    SaveUserProfile(form.userProfile);
                    ShowProfilePanel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    public class EmployeeStats
    {
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public int ShiftsWorked { get; set; }
        public decimal TotalEarned { get; set; }
    }
}