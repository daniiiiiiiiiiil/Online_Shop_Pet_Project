
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class SupportHelper
    {
        public UIHelper uIHelper;
        private DatabaseHelper databaseHelper;
        private MainMenuForm form;
        private TcpClient client;
        private NetworkStream stream;
        private bool isConnected = false;

        public SupportHelper(MainMenuForm form)
        {
            this.form = form;
            this.databaseHelper = new DatabaseHelper();
            InitializeNetwork();
        }

        private void InitializeNetwork()
        {
            try
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 8888);
                stream = client.GetStream();
                isConnected = true;

                // Запускаем поток для получения сообщений
                Task.Run(() => ReceiveMessages());
            }
            catch (Exception ex)
            {
                isConnected = false;
                MessageBox.Show($"Ошибка подключения к чату: {ex.Message}");
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            while (isConnected)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        form.Invoke((MethodInvoker)delegate
                        {
                            var chatMessage = new ChatMessage
                            {
                                Sender = "Поддержка",
                                Text = message,
                                Time = DateTime.Now,
                                IsSupport = true,
                                IsRead = false
                            };

                            SaveChatMessage(form.currentChat.Id, chatMessage);
                            form.currentChat.Messages.Add(chatMessage);

                            if (form.chatPanel != null)
                            {
                                ShowChatWithSupport();
                            }
                            else
                            {
                                MessageBox.Show("Новое сообщение от поддержки", "Уведомление",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    isConnected = false;
                    form.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Соединение с поддержкой потеряно: " + ex.Message);
                    });
                    break;
                }
            }
        }

        private void SendNetworkMessage(string message)
        {
            if (isConnected && stream != null)
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    // Сохраняем сообщение в базу данных
                    var chatMessage = new ChatMessage
                    {
                        Sender = form.userProfile.Name,
                        Text = message,
                        Time = DateTime.Now,
                        IsSupport = false,
                        IsRead = false
                    };

                    SaveChatMessage(form.currentChat.Id, chatMessage);

                    // Добавляем в текущий чат
                    form.currentChat.Messages.Add(chatMessage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка отправки сообщения: " + ex.Message);
                }
            }
        }
        private void MarkMessageAsUnread(int chatId)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "UPDATE ChatMessages SET IsRead = 0 WHERE ChatId = @ChatId AND IsSupport = 0";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.ExecuteNonQuery();
                }
            }
        }
        private void LoadChatHistory()
        {
            if (form.currentChat == null)
            {
                form.currentChat = new ChatTicket
                {
                    Id = new Random().Next(1000, 9999),
                    Subject = "Общий вопрос",
                    CreatedDate = DateTime.Now,
                    Messages = new List<ChatMessage>()
                };
            }

            // Загружаем историю из базы данных
            form.currentChat.Messages = LoadChatMessages(form.currentChat.Id);

            // Если нет сообщений, добавляем приветствие от поддержки
            if (form.currentChat.Messages.Count == 0)
            {
                form.currentChat.Messages.Add(new ChatMessage
                {
                    Sender = "Поддержка",
                    Text = "Здравствуйте! Чем мы можем вам помочь?",
                    Time = DateTime.Now.AddMinutes(-5),
                    IsSupport = true
                });
                SaveChatMessage(form.currentChat.Id, "Поддержка", "Здравствуйте! Чем мы можем вам помочь?", true);
            }
        }
        public void CloseConnection()
        {
            isConnected = false;
            stream?.Close();
            client?.Close();
        }

        public void ShowSupportHelpPanel()
        {
            form.UIHelper.ClearPanels();

            form.helpPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Техническая поддержка",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.helpPanel.Controls.Add(title);

            var chatButton = new Button
            {
                Text = "Открыть чат с поддержкой",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(250, 50),
                Location = new Point(20, 500),
                Font = new Font("Segoe UI", 12)
            };
            chatButton.Click += (s, e) => ShowChatWithSupport();
            form.helpPanel.Controls.Add(chatButton);

            var ticketsButton = new Button
            {
                Text = "Мои заявки",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(250, 50),
                Location = new Point(20, 550),
                Font = new Font("Segoe UI", 12)
            };
            ticketsButton.Click += (s, e) => ShowUserTicketsPanel();
            form.helpPanel.Controls.Add(ticketsButton);

            var faqSections = new List<FaqSection>
            {
                new FaqSection
                {
                    Title = "Общие вопросы",
                    Questions = new List<FaqQuestion>
                    {
                        new FaqQuestion { Question = "Как изменить пароль?", Answer = "Перейдите в профиль -> Личная информация -> Пароль" },
                        new FaqQuestion { Question = "Где найти историю заказов?", Answer = "В разделе 'Заказы' или в профиле" }
                    }
                },
                new FaqSection
                {
                    Title = "Технические проблемы",
                    Questions = new List<FaqQuestion>
                    {
                        new FaqQuestion { Question = "Не сканируется штрих-код", Answer = "Проверьте чистоту сканера и качество печати кода" },
                        new FaqQuestion { Question = "Система зависает", Answer = "Попробуйте перезапустить приложение. Если не помогает - создайте заявку в техподдержку" }
                    }
                }
            };

            int yPos = 60;
            foreach (var section in faqSections)
            {
                var sectionLabel = new Label
                {
                    Text = section.Title,
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    Location = new Point(20, yPos),
                    AutoSize = true
                };
                form.helpPanel.Controls.Add(sectionLabel);
                yPos += 40;

                foreach (var question in section.Questions)
                {
                    var questionPanel = new Panel
                    {
                        Location = new Point(20, yPos),
                        Size = new Size(form.ClientSize.Width - 60, 80),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.WhiteSmoke
                    };

                    var questionLabel = new Label
                    {
                        Text = $"Q: {question.Question}",
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        Location = new Point(10, 10),
                        AutoSize = true
                    };
                    questionPanel.Controls.Add(questionLabel);

                    var answerLabel = new Label
                    {
                        Text = $"A: {question.Answer}",
                        Font = new Font("Segoe UI", 10),
                        Location = new Point(10, 35),
                        AutoSize = false,
                        Size = new Size(form.ClientSize.Width - 90, 40)
                    };
                    questionPanel.Controls.Add(answerLabel);

                    form.helpPanel.Controls.Add(questionPanel);
                    yPos += 90;
                }
            }

            form.Controls.Add(form.helpPanel);
        }

        public void ShowTicketDetails(int ticketId)
        {
            SupportTicket ticket = GetTicketFromDatabase(ticketId);

            var form = new Form
            {
                Text = $"Заявка #{ticket.Id}",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            // Основные метки
            var titleLabel = new Label
            {
                Text = ticket.Title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var dateLabel = new Label
            {
                Text = $"Создана: {ticket.CreatedDate:g}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 50),
                AutoSize = true
            };

            // Информационная панель
            var infoPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(550, 70),
                BorderStyle = BorderStyle.FixedSingle
            };

            var categoryLabel = new Label
            {
                Text = $"Категория: {ticket.Category}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var priorityLabel = new Label
            {
                Text = $"Приоритет: {ticket.Priority}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 35),
                AutoSize = true
            };

            infoPanel.Controls.AddRange(new Control[] { categoryLabel, priorityLabel });

            // Описание
            var descriptionLabel = new Label
            {
                Text = "Описание:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 170),
                AutoSize = true
            };

            var descriptionBox = new TextBox
            {
                Text = ticket.Description,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(20, 195),
                Size = new Size(550, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Статус (отдельно, так как он может меняться)
            var statusLabel = new Label
            {
                Text = $"Статус: {ticket.Status}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 360),
                AutoSize = true
            };

            // Кнопка закрытия
            var closeButton = new Button
            {
                Text = "Закрыть",
                DialogResult = DialogResult.OK,
                Location = new Point(470, 420),
                Size = new Size(100, 30)
            };
            closeButton.Click += (s, e) => form.Close();

            form.Controls.AddRange(new Control[] {
        titleLabel,
        dateLabel,
        infoPanel,
        descriptionLabel,
        descriptionBox,
        statusLabel,
        closeButton
    });

            form.ShowDialog();
        }

        private void SaveSupportTicketToDatabase(string subject, string category, string priority, string description)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
            INSERT INTO SupportTickets 
            (Subject, Category, Priority, Description, CreatedDate, CustomerName, Status) 
            VALUES 
            (@Subject, @Category, @Priority, @Description, @CreatedDate, @CustomerName, 'Новая')";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Subject", subject);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Priority", priority);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@CustomerName", form.userProfile.Name);
                    command.ExecuteNonQuery();
                }
            }
        }
        public void ShowNewTicketForm()
        {
            // Создаем форму для новой заявки
            Form newTicketForm = new Form()
            {
                Text = "Новая заявка в поддержку",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            // Поле для заголовка заявки
            Label titleLabel = new Label()
            {
                Text = "Заголовок:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            TextBox titleTextBox = new TextBox()
            {
                Location = new Point(120, 20),
                Size = new Size(350, 20)
            };

            // Выбор категории
            Label categoryLabel = new Label()
            {
                Text = "Категория:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            ComboBox categoryComboBox = new ComboBox()
            {
                Location = new Point(120, 60),
                Size = new Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            categoryComboBox.Items.AddRange(new string[] { "Техническая", "Финансовая", "Учетная запись", "Другое" });
            categoryComboBox.SelectedIndex = 0;

            // Выбор приоритета
            Label priorityLabel = new Label()
            {
                Text = "Приоритет:",
                Location = new Point(20, 100),
                AutoSize = true
            };

            ComboBox priorityComboBox = new ComboBox()
            {
                Location = new Point(120, 100),
                Size = new Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            priorityComboBox.Items.AddRange(new string[] { "Низкий", "Средний", "Высокий", "Критический" });
            priorityComboBox.SelectedIndex = 1;

            // Поле для описания
            Label descriptionLabel = new Label()
            {
                Text = "Описание:",
                Location = new Point(20, 140),
                AutoSize = true
            };

            TextBox descriptionTextBox = new TextBox()
            {
                Multiline = true,
                Location = new Point(20, 160),
                Size = new Size(450, 150),
                ScrollBars = ScrollBars.Vertical
            };

            // Кнопки
            Button createButton = new Button()
            {
                Text = "Создать",
                DialogResult = DialogResult.OK,
                Location = new Point(300, 320),
                Size = new Size(80, 30)
            };

            Button cancelButton = new Button()
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new Point(390, 320),
                Size = new Size(80, 30)
            };

            // Обработчики событий
            createButton.Click += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(titleTextBox.Text))
                {
                    MessageBox.Show("Введите заголовок заявки", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
                {
                    MessageBox.Show("Введите описание проблемы", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Сохраняем заявку в базу данных
                try
                {
                    SaveSupportTicketToDatabase(
                        titleTextBox.Text,
                        categoryComboBox.SelectedItem?.ToString() ?? "Другое",
                        priorityComboBox.SelectedItem?.ToString() ?? "Средний",
                        descriptionTextBox.Text);

                    MessageBox.Show("Заявка успешно создана!", "Успех",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    newTicketForm.DialogResult = DialogResult.OK;
                    newTicketForm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании заявки: {ex.Message}", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            cancelButton.Click += (sender, e) => newTicketForm.Close();

            // Добавляем элементы на форму
            newTicketForm.Controls.Add(titleLabel);
            newTicketForm.Controls.Add(titleTextBox);
            newTicketForm.Controls.Add(categoryLabel);
            newTicketForm.Controls.Add(categoryComboBox);
            newTicketForm.Controls.Add(priorityLabel);
            newTicketForm.Controls.Add(priorityComboBox);
            newTicketForm.Controls.Add(descriptionLabel);
            newTicketForm.Controls.Add(descriptionTextBox);
            newTicketForm.Controls.Add(createButton);
            newTicketForm.Controls.Add(cancelButton);

            // Показываем форму
            newTicketForm.ShowDialog();
        }

        // Пример использования при обработке заявки
        public void ProcessTicket(int ticketId, string answer)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
            UPDATE SupportTickets 
            SET Answer = @Answer, 
                Status = 'Решена'
            WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", ticketId);
                    command.Parameters.AddWithValue("@Answer", answer);
                    command.ExecuteNonQuery();
                }
            }
        }

        private SupportTicket GetTicketFromDatabase(int ticketId)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM SupportTickets WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", ticketId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new SupportTicket
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Subject = reader["Subject"].ToString(),
                                Category = reader["Category"].ToString(),
                                Priority = reader["Priority"].ToString(),
                                Description = reader["Description"].ToString(),
                                Answer = reader["Answer"] != DBNull.Value ? reader["Answer"].ToString() : "Ответ еще не предоставлен",
                                Date = DateTime.Parse(reader["CreatedDate"].ToString()),
                                CustomerName = reader["CustomerName"].ToString()
                            };
                        }
                    }
                }
            }
            return new SupportTicket { Id = ticketId };
        }

        public void ShowComplaintsPanel()
        {
            form.UIHelper.ClearPanels();

            form.complaintsPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Жалобы и заявки клиентов",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.complaintsPanel.Controls.Add(title);

            // Загружаем и жалобы, и заявки
            var complaints = LoadComplaintsFromDatabase();
            var tickets = LoadSupportTicketsFromDatabase();

            // Объединяем в один список для отображения
            var allIssues = complaints.Concat(tickets.Select(t => new Complaint
            {
                Id = t.Id,
                CustomerName = t.CustomerName,
                Subject = t.Subject,
                Message = t.Description,
                Date = t.Date,
            })).OrderByDescending(x => x.Date).ToList();

            int yPos = 60;
            foreach (var complaint in allIssues)
            {
                var complaintPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 120),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var idLabel = new Label
                {
                    Text = $"Жалоба #{complaint.Id}",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                complaintPanel.Controls.Add(idLabel);

                var customerLabel = new Label
                {
                    Text = $"Клиент: {complaint.CustomerName}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 35),
                    AutoSize = true
                };
                complaintPanel.Controls.Add(customerLabel);

                var subjectLabel = new Label
                {
                    Text = $"Тема: {complaint.Subject}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 55),
                    AutoSize = true
                };
                complaintPanel.Controls.Add(subjectLabel);

                var dateLabel = new Label
                {
                    Text = $"Дата: {complaint.Date:dd.MM.yyyy HH:mm}",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(form.ClientSize.Width - 150, 15),
                    AutoSize = true
                };
                complaintPanel.Controls.Add(dateLabel);


                var detailsButton = new Button
                {
                    Text = "Подробнее",
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(100, 25),
                    Location = new Point(form.ClientSize.Width - 270, 15),
                    Font = new Font("Segoe UI", 9),
                    Tag = complaint.Id
                };
                detailsButton.Click += (s, e) => ShowComplaintDetails(complaint.Id);
                complaintPanel.Controls.Add(detailsButton);

                form.complaintsPanel.Controls.Add(complaintPanel);
                yPos += 130;
            }

            form.Controls.Add(form.complaintsPanel);
        }
        private List<SupportTicket> LoadSupportTicketsFromDatabase()
        {
            var tickets = new List<SupportTicket>();

            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM SupportTickets ORDER BY CreatedDate DESC";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tickets.Add(new SupportTicket
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Subject = reader["Subject"].ToString(),
                            Description = reader["Description"].ToString(),
                            Status = reader["Status"].ToString(),
                            Date = DateTime.Parse(reader["CreatedDate"].ToString()),
                            CustomerName = reader["CustomerName"].ToString(),
                            Category = reader["Category"].ToString(),
                            Priority = reader["Priority"].ToString()
                        });
                    }
                }
            }

            return tickets;
        }
        private List<Complaint> LoadComplaintsFromDatabase()
        {
            var complaints = new List<Complaint>();

            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Complaints ORDER BY CreatedDate DESC";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        complaints.Add(new Complaint
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            CustomerName = reader["CustomerName"].ToString(),
                            CustomerPhone = reader["CustomerPhone"].ToString(),
                            Subject = reader["Subject"].ToString(),
                            Message = reader["Message"].ToString(),
                            Response = reader["Response"] != DBNull.Value ? reader["Response"].ToString() : null,
                            Date = DateTime.Parse(reader["CreatedDate"].ToString()),
                            OrderId = reader["OrderId"] != DBNull.Value ? Convert.ToInt32(reader["OrderId"]) : (int?)null
                        });
                    }
                }
            }

            return complaints;
        }

        public void ShowSupportChatPanel()
        {
            form.UIHelper.ClearPanels();

            form.chatSupportPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Чат с клиентами",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.chatSupportPanel.Controls.Add(title);

            var chatsList = new ListBox
            {
                Location = new Point(20, 60),
                Size = new Size(200, form.ClientSize.Height - 180),
                Font = new Font("Segoe UI", 10)
            };

            var activeChats = LoadActiveChatsFromDatabase();

            // Добавляем проверку на наличие чатов
            if (activeChats.Count == 0)
            {
                var noChatsLabel = new Label
                {
                    Text = "Нет активных чатов",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    Location = new Point(240, 100),
                    AutoSize = true
                };
                form.chatSupportPanel.Controls.Add(noChatsLabel);
                form.Controls.Add(form.chatSupportPanel);
                return;
            }

            foreach (var chat in activeChats)
            {
                chatsList.Items.Add($"{chat.CustomerName} ({chat.UnreadCount} новых)");
            }

            var messagesPanel = new Panel
            {
                Location = new Point(240, 60),
                Size = new Size(form.ClientSize.Width - 260, form.ClientSize.Height - 180),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            // Выбираем первый чат (теперь мы уверены, что он есть)
            var selectedChat = activeChats[0];
            int yPos = 10;

            var chatMessages = LoadChatMessages(selectedChat.Id);

            foreach (var message in chatMessages)
            {
                var messagePanel = new Panel
                {
                    Location = new Point(message.IsSupport ? messagesPanel.Width - 110 : 10, yPos),
                    Size = new Size(message.IsSupport ? 100 : messagesPanel.Width - 120, 60),
                    BackColor = message.IsSupport ? Color.LightBlue : Color.LightGreen,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var senderLabel = new Label
                {
                    Text = message.Sender,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                messagePanel.Controls.Add(senderLabel);

                var timeLabel = new Label
                {
                    Text = message.Time.ToString("HH:mm"),
                    Font = new Font("Segoe UI", 8),
                    Location = new Point(messagePanel.Width - 50, 10),
                    AutoSize = true
                };
                messagePanel.Controls.Add(timeLabel);

                var textLabel = new Label
                {
                    Text = message.Text,
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(10, 30),
                    AutoSize = false,
                    Size = new Size(messagePanel.Width - 20, 30)
                };
                messagePanel.Controls.Add(textLabel);

                messagesPanel.Controls.Add(messagePanel);
                yPos += 70;
            }

            var messageBox = new TextBox
            {
                Location = new Point(240, form.ClientSize.Height - 110),
                Size = new Size(form.ClientSize.Width - 340, 40),
                Multiline = true,
                Font = new Font("Segoe UI", 10)
            };

            var sendButton = new Button
            {
                Text = "Отправить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 40),
                Location = new Point(form.ClientSize.Width - 90, form.ClientSize.Height - 110),
                Font = new Font("Segoe UI", 10)
            };
            sendButton.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(messageBox.Text))
                {
                    // Теперь мы уверены, что activeChats не пустой
                    SaveChatMessage(activeChats[0].Id, "Поддержка", messageBox.Text, true);
                    messageBox.Text = "";
                    ShowSupportChatPanel();
                }
            };

            form.chatSupportPanel.Controls.Add(chatsList);
            form.chatSupportPanel.Controls.Add(messagesPanel);
            form.chatSupportPanel.Controls.Add(messageBox);
            form.chatSupportPanel.Controls.Add(sendButton);

            form.Controls.Add(form.chatSupportPanel);
        }

        private List<SupportChat> LoadActiveChatsFromDatabase()
        {
            var chats = new List<SupportChat>();

            try
            {
                using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
                {
                    connection.Open();
                    string query = @"
                SELECT 
                    c.ChatId, 
                    MAX(c.CustomerName) as CustomerName,
                    SUM(CASE WHEN c.IsSupport = 0 AND c.IsRead = 0 THEN 1 ELSE 0 END) as UnreadCount,
                    MAX(c.Timestamp) as LastMessageTime
                FROM ChatMessages c
                GROUP BY c.ChatId
                ORDER BY LastMessageTime DESC";

                    using (var command = new SQLiteCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            chats.Add(new SupportChat
                            {
                                Id = Convert.ToInt32(reader["ChatId"]),
                                CustomerName = reader["CustomerName"]?.ToString() ?? "Неизвестный клиент",
                                UnreadCount = Convert.ToInt32(reader["UnreadCount"]),
                                LastActivity = DateTime.Parse(reader["LastMessageTime"].ToString())
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке чатов: {ex.Message}");
            }

            return chats;
        }
        public void CheckForNewMessages()
        {
            if (form.currentChat != null)
            {
                var lastMessage = form.currentChat.Messages.LastOrDefault();
                if (lastMessage != null && lastMessage.IsSupport && !lastMessage.IsRead)
                {
                    MessageBox.Show($"Новое сообщение от поддержки: {lastMessage.Text}",
                        "Новое сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Помечаем как прочитанное
                    MarkMessageAsRead(lastMessage.Id);
                }
            }
        }

        private void MarkMessageAsRead(int messageId)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "UPDATE ChatMessages SET IsRead = 1 WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", messageId);
                    command.ExecuteNonQuery();
                }
            }
        }
        private void CheckConnection()
        {
            if (!isConnected)
            {
                try
                {
                    client = new TcpClient();
                    client.Connect("127.0.0.1", 8888);
                    stream = client.GetStream();
                    isConnected = true;
                    Task.Run(() => ReceiveMessages());
                }
                catch
                {
                    // Пробуем снова через 5 секунд
                    Task.Delay(5000).ContinueWith(t => CheckConnection());
                }
            }
        }
        private int GetUnreadMessagesCount(int chatId)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM ChatMessages WHERE ChatId = @ChatId AND IsSupport = 0 AND IsRead = 0";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        private List<ChatMessage> LoadChatMessages(int chatId)
        {
            var messages = new List<ChatMessage>();

            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM ChatMessages WHERE ChatId = @ChatId ORDER BY Timestamp";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new ChatMessage
                            {
                                Sender = reader["Sender"].ToString(),
                                Text = reader["Message"].ToString(),
                                Time = DateTime.Parse(reader["Timestamp"].ToString()),
                                IsSupport = Convert.ToBoolean(reader["IsSupport"])
                            });
                        }
                    }
                }

                // Помечаем сообщения как прочитанные
                string updateQuery = "UPDATE ChatMessages SET IsRead = 1 WHERE ChatId = @ChatId AND IsSupport = 0";
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.ExecuteNonQuery();
                }
            }

            return messages;
        }

        public void SaveComplaint(string customerName, string phone, string subject, string message, int? orderId)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
        INSERT INTO Complaints 
        (CustomerName, CustomerPhone, Subject, Message, OrderId, CreatedDate) 
        VALUES 
        (@CustomerName, @CustomerPhone, @Subject, @Message, @OrderId,@CreatedDate)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerName", customerName);
                    command.Parameters.AddWithValue("@CustomerPhone", phone);
                    command.Parameters.AddWithValue("@Subject", subject);
                    command.Parameters.AddWithValue("@Message", message);
                    command.Parameters.AddWithValue("@OrderId", orderId.HasValue ? (object)orderId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.ExecuteNonQuery();
                }
            }
        }
        public void ShowKnowledgeBase()
        {
            form.UIHelper.ClearPanels();

            form.knowledgePanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "База знаний техподдержки",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.knowledgePanel.Controls.Add(title);

            var categories = LoadKnowledgeCategories();

            int yPos = 60;
            foreach (var category in categories)
            {
                var categoryLabel = new Label
                {
                    Text = category.Name,
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    Location = new Point(20, yPos),
                    AutoSize = true
                };
                form.knowledgePanel.Controls.Add(categoryLabel);
                yPos += 40;

                foreach (var article in category.Articles)
                {
                    var articlePanel = new Panel
                    {
                        Location = new Point(40, yPos),
                        Size = new Size(form.ClientSize.Width - 80, 50),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.WhiteSmoke
                    };

                    var articleLink = new LinkLabel
                    {
                        Text = article.Title,
                        Font = new Font("Segoe UI", 10),
                        Location = new Point(10, 15),
                        AutoSize = true,
                        Tag = article.Id
                    };
                    articleLink.Click += (s, e) => ShowArticle((int)articleLink.Tag);
                    articlePanel.Controls.Add(articleLink);

                    form.knowledgePanel.Controls.Add(articlePanel);
                    yPos += 60;
                }
            }

            form.Controls.Add(form.knowledgePanel);
        }

        private List<KnowledgeCategory> LoadKnowledgeCategories()
        {
            var categories = new List<KnowledgeCategory>();

            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();

                // Загружаем категории
                string categoryQuery = "SELECT DISTINCT Category FROM KnowledgeBase";
                using (var command = new SQLiteCommand(categoryQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var category = new KnowledgeCategory
                        {
                            Name = reader["Category"].ToString(),
                            Articles = new List<KnowledgeArticle>()
                        };

                        // Загружаем статьи для каждой категории
                        string articleQuery = "SELECT * FROM KnowledgeBase WHERE Category = @Category";
                        using (var articleCommand = new SQLiteCommand(articleQuery, connection))
                        {
                            articleCommand.Parameters.AddWithValue("@Category", category.Name);
                            using (var articleReader = articleCommand.ExecuteReader())
                            {
                                while (articleReader.Read())
                                {
                                    category.Articles.Add(new KnowledgeArticle
                                    {
                                        Id = Convert.ToInt32(articleReader["Id"]),
                                        Title = articleReader["Title"].ToString()
                                    });
                                }
                            }
                        }

                        categories.Add(category);
                    }
                }
            }

            return categories;
        }

        public void ShowArticle(int articleId)
        {
            var article = GetArticleFromDatabase(articleId);

            var form = new Form
            {
                Text = article.Title,
                Size = new Size(600, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var titleLabel = new Label
            {
                Text = article.Title,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var contentLabel = new Label
            {
                Text = article.Content,
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 60),
                AutoSize = false,
                Size = new Size(550, 300)
            };

            form.Controls.Add(titleLabel);
            form.Controls.Add(contentLabel);
            form.ShowDialog();
        }

        private KnowledgeArticle GetArticleFromDatabase(int articleId)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM KnowledgeBase WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", articleId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new KnowledgeArticle
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = reader["Title"].ToString(),
                                Content = reader["Content"].ToString(),
                                Category = reader["Category"].ToString()
                            };
                        }
                    }
                }
            }
            return new KnowledgeArticle { Id = articleId };
        }

        public void ShowComplaintDetails(int complaintId)
        {
            var complaint = GetComplaintFromDatabase(complaintId);

            var form = new Form
            {
                Text = $"Жалоба #{complaint.Id}",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            // Заголовок жалобы
            var idLabel = new Label
            {
                Text = $"Жалоба #{complaint.Id}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Информация о клиенте
            var customerLabel = new Label
            {
                Text = $"Клиент: {complaint.CustomerName}",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 50),
                AutoSize = true
            };

            var phoneLabel = new Label
            {
                Text = $"Телефон: {complaint.CustomerPhone}",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 80),
                AutoSize = true
            };

            var orderLabel = new Label
            {
                Text = complaint.OrderId > 0 ? $"Номер заказа: {complaint.OrderId}" : "Без привязки к заказу",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 110),
                AutoSize = true
            };

            // Тема жалобы
            var subjectLabel = new Label
            {
                Text = $"Тема: {complaint.Subject}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 140),
                AutoSize = true
            };

            // Сообщение клиента
            var messageLabel = new Label
            {
                Text = "Сообщение клиента:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 170),
                AutoSize = true
            };

            var messageBox = new TextBox
            {
                Text = complaint.Message,
                Location = new Point(20, 200),
                Size = new Size(550, 80),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Ответ поддержки
            var responseLabel = new Label
            {
                Text = "Ваш ответ:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 290),
                AutoSize = true
            };

            var responseBox = new TextBox
            {
                Text = complaint.Response ?? string.Empty,
                Location = new Point(20, 320),
                Size = new Size(550, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            var saveButton = new Button
            {
                Text = "Сохранить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(350, 410),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(responseBox.Text))
                {
                    MessageBox.Show("Введите ответ клиенту", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    // Обновляем жалобу в базе данных
                    UpdateComplaintInDatabase(complaint.Id, responseBox.Text, "Решена");

                    // Отправляем уведомление пользователю
                    SendNotificationToUser(complaint.CustomerName,
                                        $"Ответ на вашу жалобу #{complaint.Id}: {responseBox.Text}");

                    MessageBox.Show("Ответ сохранен и отправлен клиенту", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    form.DialogResult = DialogResult.OK;
                    form.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Кнопка закрытия
            var closeButton = new Button
            {
                Text = "Закрыть",
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(470, 410),
                DialogResult = DialogResult.Cancel
            };
            closeButton.Click += (s, e) => form.Close();

            // Добавляем все элементы на форму
            form.Controls.Add(idLabel);
            form.Controls.Add(customerLabel);
            form.Controls.Add(phoneLabel);
            form.Controls.Add(orderLabel);
            form.Controls.Add(subjectLabel);
            form.Controls.Add(messageLabel);
            form.Controls.Add(messageBox);
            form.Controls.Add(responseLabel);
            form.Controls.Add(responseBox);
            form.Controls.Add(saveButton);
            form.Controls.Add(closeButton);

            // Настройка поведения формы
            form.AcceptButton = saveButton;
            form.CancelButton = closeButton;

            form.ShowDialog();
        }

        private void SendNotificationToUser(string userName, string message)
        {
            try
            {
                SaveNotificationToDatabase(userName, message);

                if (isConnected)
                {
                    SendNetworkMessage($"Уведомление: {message}");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке уведомления: {ex.Message}");
            }
        }

        private void SaveNotificationToDatabase(string userName, string message)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
            INSERT INTO Notifications 
            (UserName, Message, CreatedDate, IsRead) 
            VALUES 
            (@UserName, @Message, @CreatedDate, 0)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@Message", message);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ShowUserTicketsPanel()
        {
            form.UIHelper.ClearPanels();

            form.ticketsPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Мои заявки в поддержку",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.ticketsPanel.Controls.Add(title);

            var tickets = LoadUserTicketsFromDatabase(form.userProfile.Name);

            int yPos = 60;
            foreach (var ticket in tickets)
            {
                var ticketPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 120),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var idLabel = new Label
                {
                    Text = $"Заявка #{ticket.Id}",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                ticketPanel.Controls.Add(idLabel);

                var subjectLabel = new Label
                {
                    Text = $"Тема: {ticket.Subject}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 35),
                    AutoSize = true
                };
                ticketPanel.Controls.Add(subjectLabel);

                var statusLabel = new Label
                {
                    Text = $"Статус: {ticket.Status}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 55),
                    AutoSize = true
                };
                ticketPanel.Controls.Add(statusLabel);

                var dateLabel = new Label
                {
                    Text = $"Дата: {ticket.Date:dd.MM.yyyy HH:mm}",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(form.ClientSize.Width - 150, 15),
                    AutoSize = true
                };
                ticketPanel.Controls.Add(dateLabel);

                var detailsButton = new Button
                {
                    Text = "Подробнее",
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(100, 25),
                    Location = new Point(form.ClientSize.Width - 270, 15),
                    Font = new Font("Segoe UI", 9),
                    Tag = ticket.Id
                };
                detailsButton.Click += (s, e) => ShowUserTicketDetails((int)detailsButton.Tag);
                ticketPanel.Controls.Add(detailsButton);

                // Кнопка просмотра ответа (если он есть)
                if (!string.IsNullOrEmpty(ticket.Answer))
                {
                    var viewAnswerButton = new Button
                    {
                        Text = "Ответ поддержки",
                        BackColor = Color.FromArgb(0, 128, 0),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(120, 25),
                        Location = new Point(form.ClientSize.Width - 270, 45),
                        Font = new Font("Segoe UI", 9),
                        Tag = ticket.Id
                    };
                    viewAnswerButton.Click += (s, e) => ShowSupportAnswer((int)viewAnswerButton.Tag);
                    ticketPanel.Controls.Add(viewAnswerButton);
                }

                form.ticketsPanel.Controls.Add(ticketPanel);
                yPos += 130;
            }

            form.Controls.Add(form.ticketsPanel);
        }
        private List<SupportTicket> LoadUserTicketsFromDatabase(string userName)
        {
            var tickets = new List<SupportTicket>();

            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM SupportTickets WHERE CustomerName = @CustomerName ORDER BY CreatedDate DESC";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerName", userName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tickets.Add(new SupportTicket
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Subject = reader["Subject"].ToString(),
                                Description = reader["Description"].ToString(),
                                Status = reader["Status"].ToString(),
                                Answer = reader["Answer"] != DBNull.Value ? reader["Answer"].ToString() : null,
                                Date = DateTime.Parse(reader["CreatedDate"].ToString()),
                                CustomerName = reader["CustomerName"].ToString(),
                                Category = reader["Category"].ToString(),
                                Priority = reader["Priority"].ToString()
                            });
                        }
                    }
                }
            }

            return tickets;
        }
        public void ShowUserTicketDetails(int ticketId)
        {
            var ticket = GetTicketFromDatabase(ticketId);

            var form = new Form
            {
                Text = $"Заявка #{ticket.Id}",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            // Основные метки
            var titleLabel = new Label
            {
                Text = ticket.Subject,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var dateLabel = new Label
            {
                Text = $"Создана: {ticket.Date:g}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 50),
                AutoSize = true
            };

            // Информационная панель
            var infoPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(550, 70),
                BorderStyle = BorderStyle.FixedSingle
            };

            var categoryLabel = new Label
            {
                Text = $"Категория: {ticket.Category}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var priorityLabel = new Label
            {
                Text = $"Приоритет: {ticket.Priority}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 35),
                AutoSize = true
            };

            infoPanel.Controls.AddRange(new Control[] { categoryLabel, priorityLabel });

            // Описание
            var descriptionLabel = new Label
            {
                Text = "Описание:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 170),
                AutoSize = true
            };

            var descriptionBox = new TextBox
            {
                Text = ticket.Description,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(20, 195),
                Size = new Size(550, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Кнопка просмотра ответа (если он есть)
            Button viewAnswerButton = null;
            if (!string.IsNullOrEmpty(ticket.Answer))
            {
                viewAnswerButton = new Button
                {
                    Text = "Посмотреть ответ",
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(150, 30),
                    Location = new Point(20, 360),
                    Tag = ticket.Id
                };
                viewAnswerButton.Click += (s, e) => ShowSupportAnswer((int)viewAnswerButton.Tag);
            }

            // Кнопка закрытия
            var closeButton = new Button
            {
                Text = "Закрыть",
                DialogResult = DialogResult.OK,
                Location = new Point(470, 420),
                Size = new Size(100, 30)
            };
            closeButton.Click += (s, e) => form.Close();

            var controls = new List<Control> {
        titleLabel,
        dateLabel,
        infoPanel,
        descriptionLabel,
        descriptionBox,
        closeButton
    };

            if (viewAnswerButton != null)
            {
                controls.Add(viewAnswerButton);
            }

            form.Controls.AddRange(controls.ToArray());
            form.ShowDialog();
        }
        public void ShowSupportAnswer(int ticketId)
        {
            var ticket = GetTicketFromDatabase(ticketId);

            var form = new Form
            {
                Text = $"Ответ на заявку #{ticket.Id}",
                Size = new Size(500, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var titleLabel = new Label
            {
                Text = $"Ответ поддержки на заявку #{ticket.Id}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var answerBox = new TextBox
            {
                Text = ticket.Answer,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(20, 60),
                Size = new Size(450, 180),
                BorderStyle = BorderStyle.FixedSingle
            };

            var closeButton = new Button
            {
                Text = "Закрыть",
                DialogResult = DialogResult.OK,
                Location = new Point(370, 250),
                Size = new Size(100, 30)
            };
            closeButton.Click += (s, e) => form.Close();

            form.Controls.Add(titleLabel);
            form.Controls.Add(answerBox);
            form.Controls.Add(closeButton);
            form.ShowDialog();
        }
        private ChatTicket LoadChatByCustomerName(string customerName)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();

                // Находим последний чат для этого клиента
                string query = @"
            SELECT ChatId 
            FROM ChatMessages 
            WHERE CustomerName = @CustomerName 
            GROUP BY ChatId 
            ORDER BY MAX(Timestamp) DESC 
            LIMIT 1";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerName", customerName);
                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        int chatId = Convert.ToInt32(result);
                        return new ChatTicket
                        {
                            Id = chatId,
                            CustomerName = customerName,
                            Messages = LoadChatMessages(chatId)
                        };
                    }
                }
            }
            return null;
        }
        private Complaint GetComplaintFromDatabase(int complaintId)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Complaints WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", complaintId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Complaint
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                CustomerPhone = reader["CustomerPhone"].ToString(),
                                Subject = reader["Subject"].ToString(),
                                Date = DateTime.Parse(reader["CreatedDate"].ToString()),
                                Message = reader["Message"].ToString(),
                                OrderId = reader["OrderId"] != DBNull.Value ? Convert.ToInt32(reader["OrderId"]) : 0
                            };
                        }
                    }
                }
            }
            return new Complaint { Id = complaintId };
        }

        private void UpdateComplaintInDatabase(int complaintId, string response, string status)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
                UPDATE Complaints 
                SET Response = @Response, 
                    Status = @Status, 
                    ResolvedDate = @ResolvedDate 
                WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", complaintId);
                    command.Parameters.AddWithValue("@Response", response);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@ResolvedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ShowChatWithSupport()
        {
            form.UIHelper.ClearPanels();

            // Загружаем историю чата
            LoadChatHistory();

            form.chatPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                BackColor = Color.White
            };

            var chatTitle = new Label
            {
                Text = $"Чат с поддержкой #{form.currentChat.Id}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.chatPanel.Controls.Add(chatTitle);

            var messagesPanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(form.ClientSize.Width - 40, form.ClientSize.Height - 180),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            // Отображаем все сообщения
            int yPos = 10;
            foreach (var message in form.currentChat.Messages.OrderBy(m => m.Time))
            {
                var messagePanel = new Panel
                {
                    Location = new Point(message.IsSupport ? messagesPanel.Width - 210 : 10, yPos),
                    Size = new Size(200, 80),
                    BackColor = message.IsSupport ? Color.LightBlue : Color.LightGreen,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var senderLabel = new Label
                {
                    Text = message.Sender,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                messagePanel.Controls.Add(senderLabel);

                var timeLabel = new Label
                {
                    Text = message.Time.ToString("HH:mm"),
                    Font = new Font("Segoe UI", 8),
                    Location = new Point(messagePanel.Width - 50, 10),
                    AutoSize = true
                };
                messagePanel.Controls.Add(timeLabel);

                var textLabel = new Label
                {
                    Text = message.Text,
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(10, 30),
                    AutoSize = false,
                    Size = new Size(messagePanel.Width - 20, 40),
                    TextAlign = ContentAlignment.TopLeft
                };
                messagePanel.Controls.Add(textLabel);

                messagesPanel.Controls.Add(messagePanel);
                yPos += 90;
            }

            // Прокручиваем к последнему сообщению
            messagesPanel.ScrollControlIntoView(messagesPanel.Controls[messagesPanel.Controls.Count - 1]);

            form.chatPanel.Controls.Add(messagesPanel);

            var messageTextBox = new TextBox
            {
                Location = new Point(20, form.ClientSize.Height - 110),
                Size = new Size(form.ClientSize.Width - 150, 40),
                Multiline = true,
                Font = new Font("Segoe UI", 10)
            };

            var sendButton = new Button
            {
                Text = "Отправить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 40),
                Location = new Point(form.ClientSize.Width - 120, form.ClientSize.Height - 110),
                Font = new Font("Segoe UI", 10)
            };
            sendButton.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(messageTextBox.Text))
                {
                    // Добавляем сообщение пользователя
                    var userMessage = new ChatMessage
                    {
                        Sender = form.userProfile.Name,
                        Text = messageTextBox.Text,
                        Time = DateTime.Now,
                        IsSupport = false
                    };

                    // Сохраняем в базу данных
                    SaveChatMessage(form.currentChat.Id, userMessage.Sender, userMessage.Text, false);

                    // Добавляем в текущий чат
                    form.currentChat.Messages.Add(userMessage);

                    // Отправляем на сервер
                    if (isConnected)
                    {
                        SendNetworkMessage(messageTextBox.Text);
                    }

                    messageTextBox.Text = "";
                    ShowChatWithSupport(); // Обновляем чат
                }
            };

            // Отправка по Enter
            messageTextBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(messageTextBox.Text))
                {
                    sendButton.PerformClick();
                    e.SuppressKeyPress = true;
                }
            };

            var backButton = new Button
            {
                Text = "Назад",
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(20, form.ClientSize.Height - 160),
                Font = new Font("Segoe UI", 9)
            };
            backButton.Click += (s, e) => ShowSupportHelpPanel();

            form.chatPanel.Controls.Add(messageTextBox);
            form.chatPanel.Controls.Add(sendButton);
            form.chatPanel.Controls.Add(backButton);

            form.Controls.Add(form.chatPanel);
        }
        private ChatTicket LoadExistingChat(string customerName)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();

                // Находим последний чат для этого пользователя
                string query = @"
        SELECT ChatId, MAX(Timestamp) as LastMessageTime 
        FROM ChatMessages 
        WHERE CustomerName = @CustomerName 
        GROUP BY ChatId 
        ORDER BY LastMessageTime DESC 
        LIMIT 1";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerName", customerName);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int chatId = Convert.ToInt32(reader["ChatId"]);
                            return new ChatTicket
                            {
                                Id = chatId,
                                Subject = "Продолжение разговора",
                                CreatedDate = DateTime.Parse(reader["LastMessageTime"].ToString()),
                                Status = "Открыт",
                                CustomerName = customerName,
                                Messages = LoadChatMessages(chatId)
                            };
                        }
                    }
                }
            }
            return null;
        }

        private int GenerateNewChatId()
        {
            return new Random().Next(1000, 9999);
        }
        private void SaveChatMessage(int chatId, ChatMessage message)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
        INSERT INTO ChatMessages 
        (ChatId, Sender, Message, Timestamp, IsSupport, IsRead, CustomerName) 
        VALUES 
        (@ChatId, @Sender, @Message, @Timestamp, @IsSupport, @IsRead, @CustomerName)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@Sender", message.Sender);
                    command.Parameters.AddWithValue("@Message", message.Text);
                    command.Parameters.AddWithValue("@Timestamp", message.Time.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@IsSupport", message.IsSupport ? 1 : 0);
                    command.Parameters.AddWithValue("@IsRead", message.IsRead ? 1 : 0);
                    command.Parameters.AddWithValue("@CustomerName", form.currentChat?.CustomerName ?? "Неизвестный клиент");
                    command.ExecuteNonQuery();
                }
            }
        }
        private void SaveChatMessage(int chatId, string sender, string message, bool isSupport)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
        INSERT INTO ChatMessages 
        (ChatId, Sender, Message, Timestamp, IsSupport, IsRead, CustomerName) 
        VALUES 
        (@ChatId, @Sender, @Message, @Timestamp, @IsSupport, 0, @CustomerName)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@Sender", sender);
                    command.Parameters.AddWithValue("@Message", message);
                    command.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@IsSupport", isSupport ? 1 : 0);
                    command.Parameters.AddWithValue("@CustomerName", form.currentChat?.CustomerName ?? "Неизвестный клиент");
                    command.ExecuteNonQuery();
                }
            }
        }

        private void SaveChatMessageToDatabase(int chatId, ChatMessage message)
        {
            using (var connection = new SQLiteConnection(databaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
                INSERT INTO ChatMessages 
                (ChatId, Sender, Message, Timestamp, IsSupport) 
                VALUES 
                (@ChatId, @Sender, @Message, @Timestamp, @IsSupport)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@Sender", message.Sender);
                    command.Parameters.AddWithValue("@Message", message.Text);
                    command.Parameters.AddWithValue("@Timestamp", message.Time.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@IsSupport", message.IsSupport ? 1 : 0);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}