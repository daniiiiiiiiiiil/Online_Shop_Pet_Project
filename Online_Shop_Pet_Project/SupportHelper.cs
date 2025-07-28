using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class SupportHelper
    {
        public UIHelper uIHelper;
        private MainMenuForm form;
        public SupportHelper(MainMenuForm form) { this.form = form; }

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
                Location = new Point(20, 70),
                Font = new Font("Segoe UI", 12)
            };
            chatButton.Click += (s, e) => ShowChatWithSupport();
            form.helpPanel.Controls.Add(chatButton);

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

        public void ShowNewTicketForm()
        {
            var form = new Form
            {
                Text = "Новая заявка в техподдержку",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var subjectLabel = new Label { Text = "Тема:", Location = new Point(20, 20), AutoSize = true };
            var subjectBox = new TextBox { Location = new Point(120, 20), Size = new Size(350, 20) };

            var categoryLabel = new Label { Text = "Категория:", Location = new Point(20, 60), AutoSize = true };
            var categoryBox = new ComboBox { Location = new Point(120, 60), Size = new Size(200, 20) };
            categoryBox.Items.AddRange(new[] { "Техническая проблема", "Вопрос по функционалу", "Другое" });

            var priorityLabel = new Label { Text = "Приоритет:", Location = new Point(20, 100), AutoSize = true };
            var priorityBox = new ComboBox { Location = new Point(120, 100), Size = new Size(150, 20) };
            priorityBox.Items.AddRange(new[] { "Низкий", "Средний", "Высокий", "Критичный" });

            var descriptionLabel = new Label { Text = "Описание:", Location = new Point(20, 140), AutoSize = true };
            var descriptionBox = new TextBox
            {
                Location = new Point(120, 140),
                Size = new Size(350, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(150, 270),
                Size = new Size(100, 30),
                DialogResult = DialogResult.Cancel
            };

            var submitButton = new Button
            {
                Text = "Отправить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(300, 270),
                DialogResult = DialogResult.OK
            };
            submitButton.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(subjectBox.Text))
                {
                    MessageBox.Show("Укажите тему заявки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Заявка успешно отправлена в техподдержку", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                form.Close();
            };

            form.Controls.AddRange(new Control[] { subjectLabel, subjectBox, categoryLabel, categoryBox,
                            priorityLabel, priorityBox, descriptionLabel, descriptionBox,
                            cancelButton, submitButton });

            form.ShowDialog();
        }

        public void ShowTicketDetails(int ticketId)
        {
            var ticket = new SupportTicket
            {
                Id = ticketId,
                Subject = "Проблема с авторизацией",
                Category = "Техническая проблема",
                Priority = "Высокий",
                Status = "В обработке",
                Date = DateTime.Now.AddDays(-1),
                Description = "Не могу войти в систему, выдает ошибку 500 при попытке авторизации.",
                Answer = "Мы работаем над решением проблемы. Попробуйте очистить кеш браузера."
            };

            var form = new Form
            {
                Text = $"Заявка #{ticket.Id}",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var idLabel = new Label
            {
                Text = $"Заявка #{ticket.Id}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var subjectLabel = new Label
            {
                Text = $"Тема: {ticket.Subject}",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 50),
                AutoSize = true
            };

            var infoPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(550, 100),
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

            var statusLabel = new Label
            {
                Text = $"Статус: {ticket.Status}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = uIHelper.GetTicketStatusColor(ticket.Status),
                Location = new Point(10, 60),
                AutoSize = true
            };

            var dateLabel = new Label
            {
                Text = $"Дата создания: {ticket.Date:dd.MM.yyyy HH:mm}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(300, 10),
                AutoSize = true
            };

            infoPanel.Controls.AddRange(new Control[] { categoryLabel, priorityLabel, statusLabel, dateLabel });

            var descLabel = new Label
            {
                Text = "Описание проблемы:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 190),
                AutoSize = true
            };

            var descBox = new TextBox
            {
                Text = ticket.Description,
                Location = new Point(20, 220),
                Size = new Size(550, 100),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            var answerLabel = new Label
            {
                Text = "Ответ техподдержки:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 330),
                AutoSize = true
            };

            var answerBox = new TextBox
            {
                Text = ticket.Answer,
                Location = new Point(20, 360),
                Size = new Size(550, 60),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            var closeButton = new Button
            {
                Text = "Закрыть",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(470, 430),
                DialogResult = DialogResult.OK
            };

            form.Controls.AddRange(new Control[] { idLabel, subjectLabel, infoPanel, descLabel, descBox,
                           answerLabel, answerBox, closeButton });

            form.ShowDialog();
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
                Text = "Жалобы клиентов",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.complaintsPanel.Controls.Add(title);

            var complaints = new List<Complaint>
        {
            new Complaint
            {
                Id = 1001,
                CustomerName = "Иванов Иван",
                Subject = "Не пришел заказ",
                Date = DateTime.Now.AddDays(-1),
                Status = "Новая",
                Message = "Заказ №12345 не был доставлен в указанный срок"
            },
            new Complaint
            {
                Id = 1002,
                CustomerName = "Петрова Анна",
                Subject = "Некачественный товар",
                Date = DateTime.Now.AddHours(-3),
                Status = "В обработке",
                Message = "Полученный товар был поврежден"
            }
        };

            int yPos = 60;
            foreach (var complaint in complaints)
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

                var statusLabel = new Label
                {
                    Text = $"Статус: {complaint.Status}",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = form.UIHelper.GetComplaintStatusColor(complaint.Status),
                    Location = new Point(form.ClientSize.Width - 150, 40),
                    AutoSize = true
                };
                complaintPanel.Controls.Add(statusLabel);

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

            var activeChats = new List<SupportChat>
        {
            new SupportChat
            {
                Id = 1001,
                CustomerName = "Иванов Иван",
                LastMessage = "Когда придет мой заказ?",
                LastMessageTime = DateTime.Now.AddMinutes(-15),
                UnreadCount = 2
            },
            new SupportChat
            {
                Id = 1002,
                CustomerName = "Петрова Анна",
                LastMessage = "Спасибо за помощь!",
                LastMessageTime = DateTime.Now.AddHours(-2),
                UnreadCount = 0
            }
        };

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

            if (activeChats.Count > 0)
            {
                var selectedChat = activeChats[0];
                int yPos = 10;

                var clientMessage = new Panel
                {
                    Location = new Point(10, yPos),
                    Size = new Size(messagesPanel.Width - 120, 60),
                    BackColor = Color.LightGreen,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var clientLabel = new Label
                {
                    Text = $"{selectedChat.CustomerName} (клиент)",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Location = new Point(10, 5),
                    AutoSize = true
                };
                clientMessage.Controls.Add(clientLabel);

                var clientText = new Label
                {
                    Text = "Здравствуйте! У меня проблема с заказом #12345",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(10, 25),
                    AutoSize = false,
                    Size = new Size(clientMessage.Width - 20, 30)
                };
                clientMessage.Controls.Add(clientText);

                messagesPanel.Controls.Add(clientMessage);
                yPos += 70;

                var supportMessage = new Panel
                {
                    Location = new Point(messagesPanel.Width - 110, yPos),
                    Size = new Size(100, 60),
                    BackColor = Color.LightBlue,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var supportLabel = new Label
                {
                    Text = "Поддержка",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Location = new Point(10, 5),
                    AutoSize = true
                };
                supportMessage.Controls.Add(supportLabel);

                var supportText = new Label
                {
                    Text = "Здравствуйте! Чем могу помочь?",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(10, 25),
                    AutoSize = false,
                    Size = new Size(supportMessage.Width - 20, 30)
                };
                supportMessage.Controls.Add(supportText);

                messagesPanel.Controls.Add(supportMessage);
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
                    MessageBox.Show("Сообщение отправлено клиенту", "Успех");
                    messageBox.Text = "";
                }
            };

            form.chatSupportPanel.Controls.Add(chatsList);
            form.chatSupportPanel.Controls.Add(messagesPanel);
            form.chatSupportPanel.Controls.Add(messageBox);
            form.chatSupportPanel.Controls.Add(sendButton);

            form.Controls.Add(form.chatSupportPanel);
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

            var categories = new List<KnowledgeCategory>
        {
            new KnowledgeCategory
            {
                Name = "Частые вопросы",
                Articles = new List<string>
                {
                    "Как проверить статус заказа",
                    "Как оформить возврат",
                    "Как изменить данные профиля"
                }
            },
            new KnowledgeCategory
            {
                Name = "Технические проблемы",
                Articles = new List<string>
                {
                    "Ошибка при оплате",
                    "Не работает личный кабинет",
                    "Проблемы с мобильным приложением"
                }
            }
        };

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
                        Text = article,
                        Font = new Font("Segoe UI", 10),
                        Location = new Point(10, 15),
                        AutoSize = true
                    };
                    articleLink.Click += (s, e) => ShowArticle(article);
                    articlePanel.Controls.Add(articleLink);

                    form.knowledgePanel.Controls.Add(articlePanel);
                    yPos += 60;
                }
            }

            form.Controls.Add(form.knowledgePanel);
        }

        public void ShowArticle(string articleTitle)
        {
            var form = new Form
            {
                Text = articleTitle,
                Size = new Size(600, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var titleLabel = new Label
            {
                Text = articleTitle,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var contentLabel = new Label
            {
                Text = GetArticleContent(articleTitle),
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 60),
                AutoSize = false,
                Size = new Size(550, 300)
            };

            form.Controls.Add(titleLabel);
            form.Controls.Add(contentLabel);
            form.ShowDialog();
        }

        public string GetArticleContent(string title)
        {
            switch (title)
            {
                case "Как проверить статус заказа":
                    return "1. Перейдите в раздел 'Мои заказы'\n2. Найдите нужный заказ в списке\n3. Нажмите 'Подробнее' для просмотра статуса";
                case "Ошибка при оплате":
                    return "Если возникает ошибка при оплате:\n1. Проверьте данные карты\n2. Убедитесь, что на счету достаточно средств\n3. Попробуйте другой способ оплаты";
                default:
                    return "Инструкция по решению проблемы. Подробное описание шагов для решения указанной проблемы.";
            }
        }

        public void ShowComplaintDetails(int complaintId)
        {
            var complaint = new Complaint
            {
                Id = complaintId,
                CustomerName = "Иванов Иван",
                CustomerPhone = "+7 (123) 456-78-90",
                Subject = "Не пришел заказ",
                Date = DateTime.Now.AddDays(-1),
                Status = "Новая",
                Message = "Заказ №12345 не был доставлен в указанный срок. Обещали доставить вчера до 18:00, но курьер так и не приехал.",
                OrderId = 12345
            };

            var form = new Form
            {
                Text = $"Жалоба #{complaint.Id}",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var idLabel = new Label
            {
                Text = $"Жалоба #{complaint.Id}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

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
                Text = $"Номер заказа: {complaint.OrderId}",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 110),
                AutoSize = true
            };

            var subjectLabel = new Label
            {
                Text = $"Тема: {complaint.Subject}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 140),
                AutoSize = true
            };

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

            var responseLabel = new Label
            {
                Text = "Ваш ответ:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 290),
                AutoSize = true
            };

            var responseBox = new TextBox
            {
                Location = new Point(20, 320),
                Size = new Size(550, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            var statusLabel = new Label
            {
                Text = "Изменить статус:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 410),
                AutoSize = true
            };

            var statusCombo = new ComboBox
            {
                Location = new Point(150, 410),
                Size = new Size(150, 20)
            };
            statusCombo.Items.AddRange(new[] { "Новая", "В обработке", "Решена", "Отклонена" });
            statusCombo.SelectedItem = complaint.Status;

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

                MessageBox.Show("Ответ сохранен и отправлен клиенту", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            form.Controls.AddRange(new Control[] { idLabel, customerLabel, phoneLabel, orderLabel,
                       subjectLabel, messageLabel, messageBox, responseLabel, responseBox,
                       statusLabel, statusCombo, saveButton });

            form.ShowDialog();
        }

        public void ShowChatWithSupport()
        {
            form.UIHelper.ClearPanels();

            if (form.currentChat == null)
            {
                form.currentChat = new ChatTicket
                {
                    Id = new Random().Next(1000, 9999),
                    Subject = "Общий вопрос",
                    CreatedDate = DateTime.Now,
                    Status = "Открыт",
                    Messages = new List<ChatMessage>
                {
                    new ChatMessage
                    {
                        Sender = "Поддержка",
                        Text = "Здравствуйте! Чем мы можем вам помочь?",
                        Time = DateTime.Now.AddMinutes(-5),
                        IsSupport = true
                    },
                    new ChatMessage
                    {
                        Sender = form.userProfile.Name,
                        Text = "У меня проблема с...",
                        Time = DateTime.Now.AddMinutes(-2),
                        IsSupport = false
                    }
                }
                };
            }

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

            int yPos = 10;
            foreach (var message in form.currentChat.Messages.OrderBy(m => m.Time))
            {
                var messagePanel = new Panel
                {
                    Location = new Point(10, yPos),
                    Size = new Size(messagesPanel.Width - 30, 80),
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
                    Size = new Size(messagePanel.Width - 20, 40)
                };
                messagePanel.Controls.Add(textLabel);

                messagesPanel.Controls.Add(messagePanel);
                yPos += 90;
            }

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
                    form.currentChat.Messages.Add(new ChatMessage
                    {
                        Sender = form.userProfile.Name,
                        Text = messageTextBox.Text,
                        Time = DateTime.Now,
                        IsSupport = false
                    });

                    form.currentChat.Messages.Add(new ChatMessage
                    {
                        Sender = "Поддержка",
                        Text = "Спасибо за сообщение. Мы обработаем ваш запрос и ответим в ближайшее время.",
                        Time = DateTime.Now.AddSeconds(2),
                        IsSupport = true
                    });

                    messageTextBox.Text = "";
                    ShowChatWithSupport();
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
    }
}
