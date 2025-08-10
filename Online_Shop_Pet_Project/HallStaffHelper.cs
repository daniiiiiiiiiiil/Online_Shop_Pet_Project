using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class HallStaffHelper
    {
        private MainMenuForm form;
        private SQLiteConnection connection;
        private string dbPath = "shop_database.db";

        public HallStaffHelper(MainMenuForm form)
        {
            this.form = form;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            connection.Open();

            // Создаем таблицу заказов, если она не существует
            string createOrdersTable = @"
                CREATE TABLE IF NOT EXISTS HallOrders (
                    Id INTEGER PRIMARY KEY,
                    Type TEXT NOT NULL,
                    Items TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    Location TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            // Создаем таблицу истории заданий
            string createTasksTable = @"
                CREATE TABLE IF NOT EXISTS HallTasks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderId INTEGER,
                    Description TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY(OrderId) REFERENCES HallOrders(Id)
                )";

            using (var command = new SQLiteCommand(createOrdersTable, connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(createTasksTable, connection))
            {
                command.ExecuteNonQuery();
            }

            // Добавляем тестовые данные, если таблица пуста
            if (IsTableEmpty("HallOrders"))
            {
                InsertSampleData();
            }
        }

        private bool IsTableEmpty(string tableName)
        {
            string query = $"SELECT COUNT(*) FROM {tableName}";
            using (var command = new SQLiteCommand(query, connection))
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count == 0;
            }
        }

        private void InsertSampleData()
        {
            string insertOrders = @"
                INSERT INTO HallOrders (Id, Type, Items, Status, Location)
                VALUES 
                (1001, 'Самовывоз', 'Смартфон Samsung, Наушники Sony', 'Поступил', 'Зал 1'),
                (1002, 'Доставка', 'Пицца Маргарита, Салат Цезарь', 'Поступил', 'Кухня'),
                (1003, 'Самовывоз', 'Книга ''Clean Code''', 'Поступил', 'Секция 5')";

            string insertTasks = @"
                INSERT INTO HallTasks (OrderId, Description, Status)
                VALUES 
                (1001, 'Сбор заказа #1001 для самовывоза', 'Выполнено'),
                (1002, 'Подготовка товаров для доставки #1002', 'Выполнено'),
                (1003, 'Вынос товаров в торговый зал', 'Выполнено')";

            using (var command = new SQLiteCommand(insertOrders, connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(insertTasks, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void ShowHallStaffOrdersPanel()
        {
            form.UIHelper.ClearPanels();

            form.hallStaffOrdersPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Заказы для сборки",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.hallStaffOrdersPanel.Controls.Add(title);

            // Получаем заказы из базы данных
            List<HallOrder> orders = GetOrdersFromDatabase();

            int yPos = 60;
            foreach (var order in orders)
            {
                var orderPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 120),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var idLabel = new Label
                {
                    Text = $"Заказ #{order.Id}",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                orderPanel.Controls.Add(idLabel);

                var typeLabel = new Label
                {
                    Text = $"Тип: {order.Type}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(150, 12),
                    AutoSize = true
                };
                orderPanel.Controls.Add(typeLabel);

                var itemsLabel = new Label
                {
                    Text = $"Товары: {order.Items}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 35),
                    AutoSize = false,
                    Size = new Size(form.ClientSize.Width - 200, 40)
                };
                orderPanel.Controls.Add(itemsLabel);

                var locationLabel = new Label
                {
                    Text = $"Расположение: {order.Location}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 75),
                    AutoSize = true
                };
                orderPanel.Controls.Add(locationLabel);

                var statusLabel = new Label
                {
                    Text = $"Статус: {order.Status}",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = form.UIHelper.GetHallOrderStatusColor(order.Status),
                    Location = new Point(form.ClientSize.Width - 150, 75),
                    AutoSize = true
                };
                orderPanel.Controls.Add(statusLabel);

                // Кнопки для управления статусом заказа
                if (order.Status == "Поступил")
                {
                    var startButton = new Button
                    {
                        Text = "Начать сборку",
                        BackColor = Color.FromArgb(70, 130, 180),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(120, 30),
                        Location = new Point(form.ClientSize.Width - 280, 75),
                        Font = new Font("Segoe UI", 9),
                        Tag = order.Id
                    };
                    startButton.Click += (s, e) => UpdateOrderStatus(order.Id, "Собирается");
                    orderPanel.Controls.Add(startButton);
                }
                else if (order.Status == "Собирается")
                {
                    var readyButton = new Button
                    {
                        Text = "Заказ собран",
                        BackColor = Color.FromArgb(70, 180, 130),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(120, 30),
                        Location = new Point(form.ClientSize.Width - 280, 75),
                        Font = new Font("Segoe UI", 9),
                        Tag = order.Id
                    };
                    readyButton.Click += (s, e) => UpdateOrderStatus(order.Id, "Собран");
                    orderPanel.Controls.Add(readyButton);
                }
                else if (order.Status == "Собран")
                {
                    var deliveredButton = new Button
                    {
                        Text = "Заказ выдан",
                        BackColor = Color.FromArgb(180, 70, 130),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(120, 30),
                        Location = new Point(form.ClientSize.Width - 280, 75),
                        Font = new Font("Segoe UI", 9),
                        Tag = order.Id
                    };
                    deliveredButton.Click += (s, e) => UpdateOrderStatus(order.Id, "Отдан");
                    orderPanel.Controls.Add(deliveredButton);
                }

                var locationButton = new Button
                {
                    Text = "Показать на карте",
                    BackColor = Color.FromArgb(100, 150, 200),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(140, 25),
                    Location = new Point(form.ClientSize.Width - 280, 40),
                    Font = new Font("Segoe UI", 8),
                    Tag = order.Id
                };
                locationButton.Click += (s, e) => ShowProductLocation(order.Id);
                orderPanel.Controls.Add(locationButton);

                form.hallStaffOrdersPanel.Controls.Add(orderPanel);
                yPos += 130;
            }

            form.Controls.Add(form.hallStaffOrdersPanel);
        }

        private List<HallOrder> GetOrdersFromDatabase()
        {
            List<HallOrder> orders = new List<HallOrder>();

            string query = "SELECT Id, Type, Items, Status, Location FROM HallOrders WHERE Status != 'Отдан' ORDER BY Status, CreatedAt";
            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new HallOrder
                        {
                            Id = reader.GetInt32(0),
                            Type = reader.GetString(1),
                            Items = reader.GetString(2),
                            Status = reader.GetString(3),
                            Location = reader.GetString(4)
                        });
                    }
                }
            }

            return orders;
        }

        private void UpdateOrderStatus(int orderId, string newStatus)
        {
            string updateQuery = "UPDATE HallOrders SET Status = @Status, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id";
            using (var command = new SQLiteCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@Status", newStatus);
                command.Parameters.AddWithValue("@Id", orderId);
                command.ExecuteNonQuery();
            }

            // Добавляем запись в историю
            string description = $"Изменение статуса заказа #{orderId} на '{newStatus}'";
            string insertQuery = "INSERT INTO HallTasks (OrderId, Description, Status) VALUES (@OrderId, @Description, 'Выполнено')";
            using (var command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Parameters.AddWithValue("@Description", description);
                command.ExecuteNonQuery();
            }

            MessageBox.Show($"Статус заказа #{orderId} изменен на '{newStatus}'", "Обновление статуса");
            ShowHallStaffOrdersPanel();
        }

        public void ShowStoreMap()
        {
            form.UIHelper.ClearPanels();

            form.storeMapPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Карта магазина",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.storeMapPanel.Controls.Add(title);

            var mapImage = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(form.ClientSize.Width - 40, form.ClientSize.Height - 100),
                Location = new Point(20, 60),
                Image = form.UIHelper.LoadImageOrDefault("art/store_map.jpg", form.ClientSize.Width - 40, form.ClientSize.Height - 100)
            };
            form.storeMapPanel.Controls.Add(mapImage);

            var legend = new Label
            {
                Text = "Легенда:\n" +
                       "🟥 - Электроника\n" +
                       "🟦 - Продукты\n" +
                       "🟩 - Книги\n" +
                       "🟨 - Одежда",
                Font = new Font("Segoe UI", 12),
                Location = new Point(form.ClientSize.Width - 200, 70),
                AutoSize = true,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10)
            };
            form.storeMapPanel.Controls.Add(legend);

            form.Controls.Add(form.storeMapPanel);
        }

        public void ShowProductLocation(int orderId)
        {
            string location = "";
            string query = "SELECT Location FROM HallOrders WHERE Id = @Id";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", orderId);
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    location = result.ToString();
                }
            }

            if (!string.IsNullOrEmpty(location))
            {
                MessageBox.Show($"Товары заказа #{orderId} находятся:\n{location}", "Расположение товаров");
            }
            else
            {
                MessageBox.Show($"Расположение для заказа #{orderId} не найдено", "Ошибка");
            }
        }

        public void ShowHallStaffHistory()
        {
            form.UIHelper.ClearPanels();

            form.hallStaffHistoryPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "История выполненных заданий",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.hallStaffHistoryPanel.Controls.Add(title);

            // Получаем историю из базы данных
            List<HallTask> historyItems = GetHistoryFromDatabase();

            int yPos = 60;
            foreach (var task in historyItems)
            {
                var taskPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 60),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var dateLabel = new Label
                {
                    Text = task.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                taskPanel.Controls.Add(dateLabel);

                var descLabel = new Label
                {
                    Text = task.Description,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(150, 10),
                    AutoSize = true
                };
                taskPanel.Controls.Add(descLabel);

                var statusLabel = new Label
                {
                    Text = task.Status,
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Green,
                    Location = new Point(form.ClientSize.Width - 100, 10),
                    AutoSize = true
                };
                taskPanel.Controls.Add(statusLabel);

                form.hallStaffHistoryPanel.Controls.Add(taskPanel);
                yPos += 70;
            }

            form.Controls.Add(form.hallStaffHistoryPanel);
        }

        private List<HallTask> GetHistoryFromDatabase()
        {
            List<HallTask> tasks = new List<HallTask>();

            string query = "SELECT Description, Status, CreatedAt FROM HallTasks ORDER BY CreatedAt DESC LIMIT 50";
            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new HallTask
                        {
                            Description = reader.GetString(0),
                            Status = reader.GetString(1),
                            CreatedAt = reader.GetDateTime(2)
                        });
                    }
                }
            }

            return tasks;
        }
    }
}