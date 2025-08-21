using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Data.SQLite;
using System.IO;

namespace Online_Shop_Pet_Project
{
    public class CookHelper
    {
        private MainMenuForm form;
        private SQLiteConnection connection;
        private string dbPath = "RestaurantDB.sqlite";

        public CookHelper(MainMenuForm form)
        {
            this.form = form;
            InitializeDatabase();
            InitializeSampleData();
        }

        private void InitializeDatabase()
        {
            bool dbExists = File.Exists(dbPath);

            connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            connection.Open();

            if (!dbExists)
            {
                CreateDatabaseTables();
            }
        }

        private void CreateDatabaseTables()
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = @"CREATE TABLE KitchenOrders (
                                    Id INTEGER PRIMARY KEY,
                                    TableNumber INTEGER NOT NULL,
                                    Items TEXT NOT NULL,
                                    Status TEXT NOT NULL,
                                    Time DATETIME NOT NULL)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE OrderStatusHistory (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    OrderId INTEGER NOT NULL,
                                    Status TEXT NOT NULL,
                                    Time DATETIME NOT NULL,
                                    FOREIGN KEY(OrderId) REFERENCES KitchenOrders(Id))";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE MenuItems (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Name TEXT NOT NULL,
                                    Price INTEGER NOT NULL,
                                    Ingredients TEXT NOT NULL,
                                    CookingTime INTEGER NOT NULL)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE IngredientRequirements (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    MenuItemId INTEGER NOT NULL,
                                    Name TEXT NOT NULL,
                                    Quantity REAL NOT NULL,
                                    Unit TEXT NOT NULL,
                                    FOREIGN KEY(MenuItemId) REFERENCES MenuItems(Id))";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE IngredientRequests (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    IngredientName TEXT NOT NULL,
                                    Quantity REAL NOT NULL,
                                    Unit TEXT NOT NULL,
                                    Comment TEXT,
                                    RequestTime DATETIME NOT NULL,
                                    Status TEXT NOT NULL)";
                cmd.ExecuteNonQuery();
            }
        }

        private void InitializeSampleData()
        {
            if (GetOrdersCount() == 0)
            {
                AddSampleOrders();
            }

            if (GetMenuItemsCount() == 0)
            {
                AddSampleMenu();
            }
        }

        private int GetOrdersCount()
        {
            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM KitchenOrders", connection))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private int GetMenuItemsCount()
        {
            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM MenuItems", connection))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void AddSampleOrders()
        {
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    AddOrder(1001, 5, "Пицца Маргарита, Салат Цезарь", "Поступил", DateTime.Now.AddMinutes(-15));
                    AddStatusHistory(1001, "Поступил", DateTime.Now.AddMinutes(-15));

                    AddOrder(1002, 3, "Стейк средней прожарки", "Поступил", DateTime.Now.AddMinutes(-5));
                    AddStatusHistory(1002, "Поступил", DateTime.Now.AddMinutes(-5));

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private void AddOrder(int id, int tableNumber, string items, string status, DateTime time)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = @"INSERT INTO KitchenOrders (Id, TableNumber, Items, Status, Time)
                                  VALUES (@id, @tableNumber, @items, @status, @time)";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@tableNumber", tableNumber);
                cmd.Parameters.AddWithValue("@items", items);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@time", time);
                cmd.ExecuteNonQuery();
            }
        }

        private void AddStatusHistory(int orderId, string status, DateTime time)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = @"INSERT INTO OrderStatusHistory (OrderId, Status, Time)
                                  VALUES (@orderId, @status, @time)";
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@time", time);
                cmd.ExecuteNonQuery();
            }
        }

        private void AddSampleMenu()
        {
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    int pizzaId = AddMenuItem("Пицца Маргарита", 599, "Тесто, томатный соус, моцарелла, базилик", 15);
                    AddIngredientRequirement(pizzaId, "Тесто", 0.3, "кг");
                    AddIngredientRequirement(pizzaId, "Томатный соус", 0.1, "л");
                    AddIngredientRequirement(pizzaId, "Моцарелла", 0.2, "кг");
                    AddIngredientRequirement(pizzaId, "Базилик", 0.01, "кг");

                    int steakId = AddMenuItem("Стейк", 1299, "Говядина, специи, соус", 20);
                    AddIngredientRequirement(steakId, "Говядина", 0.3, "кг");
                    AddIngredientRequirement(steakId, "Специи", 0.01, "кг");
                    AddIngredientRequirement(steakId, "Стейк соус", 0.05, "л");

                    int saladId = AddMenuItem("Салат Цезарь", 399, "Курица, салат, сухарики, соус", 10);
                    AddIngredientRequirement(saladId, "Курица", 0.15, "кг");
                    AddIngredientRequirement(saladId, "Салат", 0.1, "кг");
                    AddIngredientRequirement(saladId, "Сухарики", 0.03, "кг");
                    AddIngredientRequirement(saladId, "Соус Цезарь", 0.05, "л");

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private int AddMenuItem(string name, int price, string ingredients, int cookingTime)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = @"INSERT INTO MenuItems (Name, Price, Ingredients, CookingTime)
                                  VALUES (@name, @price, @ingredients, @cookingTime);
                                  SELECT last_insert_rowid();";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@ingredients", ingredients);
                cmd.Parameters.AddWithValue("@cookingTime", cookingTime);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void AddIngredientRequirement(int menuItemId, string name, double quantity, string unit)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = @"INSERT INTO IngredientRequirements (MenuItemId, Name, Quantity, Unit)
                                  VALUES (@menuItemId, @name, @quantity, @unit)";
                cmd.Parameters.AddWithValue("@menuItemId", menuItemId);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@unit", unit);
                cmd.ExecuteNonQuery();
            }
        }

        public void ShowCookOrders()
        {
            form.UIHelper.ClearPanels();

            form.cookOrdersPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var profileButton = new Button
            {
                Text = "👤 Профиль",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(form.ClientSize.Width - 120, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            profileButton.FlatAppearance.BorderSize = 0;
            profileButton.Click += (s, e) => form.ProfileHelper.ShowProfilePanel();
            form.cookOrdersPanel.Controls.Add(profileButton);

            var title = new Label
            {
                Text = "Заказы на кухню",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.cookOrdersPanel.Controls.Add(title);

            var activeOrders = GetActiveOrders();

            int yPos = 60;
            foreach (var order in activeOrders)
            {
                var orderPanel = CreateOrderPanel(order, ref yPos);
                form.cookOrdersPanel.Controls.Add(orderPanel);
                yPos += 110;
            }

            var historyButton = new Button
            {
                Text = "История заказов",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(20, yPos + 20),
                Font = new Font("Segoe UI", 10)
            };
            historyButton.Click += (s, e) => ShowOrderHistory();
            form.cookOrdersPanel.Controls.Add(historyButton);

            form.Controls.Add(form.cookOrdersPanel);
        }

        private List<KitchenOrder> GetActiveOrders()
        {
            var orders = new List<KitchenOrder>();

            using (var cmd = new SQLiteCommand(
                "SELECT * FROM KitchenOrders WHERE Status NOT IN ('Готово', 'Отменено')", connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new KitchenOrder
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            TableNumber = Convert.ToInt32(reader["TableNumber"]),
                            Items = reader["Items"].ToString(),
                            Status = reader["Status"].ToString(),
                            Time = Convert.ToDateTime(reader["Time"]),
                            StatusHistory = GetOrderStatusHistory(Convert.ToInt32(reader["Id"]))
                        };
                        orders.Add(order);
                    }
                }
            }

            return orders;
        }

        private List<StatusChange> GetOrderStatusHistory(int orderId)
        {
            var history = new List<StatusChange>();

            using (var cmd = new SQLiteCommand(
                "SELECT * FROM OrderStatusHistory WHERE OrderId = @orderId ORDER BY Time", connection))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        history.Add(new StatusChange
                        {
                            Status = reader["Status"].ToString(),
                            Time = Convert.ToDateTime(reader["Time"])
                        });
                    }
                }
            }

            return history;
        }

        private Panel CreateOrderPanel(KitchenOrder order, ref int yPos)
        {
            var orderPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(form.ClientSize.Width - 40, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var orderLabel = new Label
            {
                Text = $"Заказ #{order.Id} | Стол: {order.TableNumber}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            orderPanel.Controls.Add(orderLabel);

            var itemsLabel = new Label
            {
                Text = $"Блюда: {order.Items}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 35),
                AutoSize = true
            };
            orderPanel.Controls.Add(itemsLabel);

            var timeLabel = new Label
            {
                Text = $"Время заказа: {order.Time:HH:mm}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(10, 55),
                AutoSize = true
            };
            orderPanel.Controls.Add(timeLabel);

            var statusLabel = new Label
            {
                Text = $"Статус: {order.Status}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = form.UIHelper.GetKitchenOrderStatusColor(order.Status),
                Location = new Point(form.ClientSize.Width - 150, 40),
                AutoSize = true
            };
            orderPanel.Controls.Add(statusLabel);

            var changeStatusButton = new Button
            {
                Text = "Изменить статус",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 25),
                Location = new Point(form.ClientSize.Width - 280, 40),
                Font = new Font("Segoe UI", 9),
                Tag = order.Id
            };
            changeStatusButton.Click += (s, e) => ChangeOrderStatus(order.Id);
            orderPanel.Controls.Add(changeStatusButton);

            return orderPanel;
        }

        public void ShowOrderHistory()
        {
            form.UIHelper.ClearPanels();

            form.orderHistoryPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "История заказов",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.orderHistoryPanel.Controls.Add(title);

            var completedOrders = GetCompletedOrders();

            int yPos = 60;
            foreach (var order in completedOrders)
            {
                var orderPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 120),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var orderLabel = new Label
                {
                    Text = $"Заказ #{order.Id} | Стол: {order.TableNumber} | Статус: {order.Status}",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                orderPanel.Controls.Add(orderLabel);

                var itemsLabel = new Label
                {
                    Text = $"Блюда: {order.Items}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 35),
                    AutoSize = true
                };
                orderPanel.Controls.Add(itemsLabel);

                var timeLabel = new Label
                {
                    Text = $"Время заказа: {order.Time:HH:mm}",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(10, 55),
                    AutoSize = true
                };
                orderPanel.Controls.Add(timeLabel);

                var historyLabel = new Label
                {
                    Text = "История статусов:",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Location = new Point(10, 75),
                    AutoSize = true
                };
                orderPanel.Controls.Add(historyLabel);

                string statusHistory = string.Join(" → ", order.StatusHistory.Select(sh => $"{sh.Status} ({sh.Time:HH:mm})"));
                var statusHistoryLabel = new Label
                {
                    Text = statusHistory,
                    Font = new Font("Segoe UI", 8),
                    Location = new Point(20, 95),
                    AutoSize = true
                };
                orderPanel.Controls.Add(statusHistoryLabel);

                form.orderHistoryPanel.Controls.Add(orderPanel);
                yPos += 130;
            }

            var backButton = new Button
            {
                Text = "Назад к текущим заказам",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 30),
                Location = new Point(20, yPos + 20),
                Font = new Font("Segoe UI", 10)
            };
            backButton.Click += (s, e) => ShowCookOrders();
            form.orderHistoryPanel.Controls.Add(backButton);

            form.Controls.Add(form.orderHistoryPanel);
        }

        private List<KitchenOrder> GetCompletedOrders()
        {
            var orders = new List<KitchenOrder>();

            using (var cmd = new SQLiteCommand(
                "SELECT * FROM KitchenOrders WHERE Status IN ('Готово', 'Отменено') ORDER BY Time DESC", connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new KitchenOrder
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            TableNumber = Convert.ToInt32(reader["TableNumber"]),
                            Items = reader["Items"].ToString(),
                            Status = reader["Status"].ToString(),
                            Time = Convert.ToDateTime(reader["Time"]),
                            StatusHistory = GetOrderStatusHistory(Convert.ToInt32(reader["Id"]))
                        };
                        orders.Add(order);
                    }
                }
            }

            return orders;
        }

        public void ShowCookMenu()
        {
            form.UIHelper.ClearPanels();

            form.cookMenuPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Меню ресторана",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.cookMenuPanel.Controls.Add(title);

            var menuItems = GetMenuItems();

            int yPos = 60;
            foreach (var item in menuItems)
            {
                var itemPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 150),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var nameLabel = new Label
                {
                    Text = item.Name,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                itemPanel.Controls.Add(nameLabel);

                var priceLabel = new Label
                {
                    Text = $"{item.Price} ₽ | Время приготовления: {item.CookingTime} мин",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 35),
                    AutoSize = true
                };
                itemPanel.Controls.Add(priceLabel);

                var ingredientsLabel = new Label
                {
                    Text = $"Ингредиенты: {item.Ingredients}",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(10, 60),
                    AutoSize = false,
                    Size = new Size(form.ClientSize.Width - 60, 30)
                };
                itemPanel.Controls.Add(ingredientsLabel);

                var requirementsLabel = new Label
                {
                    Text = "Нормы ингредиентов на порцию:",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Location = new Point(10, 90),
                    AutoSize = true
                };
                itemPanel.Controls.Add(requirementsLabel);

                var requirements = GetIngredientRequirements(item.Id);
                string requirementsText = string.Join(", ", requirements.Select(i => $"{i.Name} - {i.Quantity}{i.Unit}"));
                var requirementsList = new Label
                {
                    Text = requirementsText,
                    Font = new Font("Segoe UI", 8),
                    Location = new Point(20, 110),
                    AutoSize = false,
                    Size = new Size(form.ClientSize.Width - 80, 30)
                };
                itemPanel.Controls.Add(requirementsList);

                form.cookMenuPanel.Controls.Add(itemPanel);
                yPos += 160;
            }

            form.Controls.Add(form.cookMenuPanel);
        }

        private List<MenuItem> GetMenuItems()
        {
            var menuItems = new List<MenuItem>();

            using (var cmd = new SQLiteCommand("SELECT * FROM MenuItems", connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        menuItems.Add(new MenuItem
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Price = Convert.ToInt32(reader["Price"]),
                            Ingredients = reader["Ingredients"].ToString(),
                            CookingTime = Convert.ToInt32(reader["CookingTime"])
                        });
                    }
                }
            }

            return menuItems;
        }

        private List<IngredientRequirement> GetIngredientRequirements(int menuItemId)
        {
            var requirements = new List<IngredientRequirement>();

            using (var cmd = new SQLiteCommand(
                "SELECT * FROM IngredientRequirements WHERE MenuItemId = @menuItemId", connection))
            {
                cmd.Parameters.AddWithValue("@menuItemId", menuItemId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        requirements.Add(new IngredientRequirement
                        {
                            Name = reader["Name"].ToString(),
                            Quantity = Convert.ToDouble(reader["Quantity"]),
                            Unit = reader["Unit"].ToString()
                        });
                    }
                }
            }

            return requirements;
        }

        public void ChangeOrderStatus(int orderId)
        {
            using (var cmd = new SQLiteCommand(
                "SELECT * FROM KitchenOrders WHERE Id = @orderId", connection))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return;

                    var form = new Form
                    {
                        Text = $"Изменение статуса заказа #{orderId}",
                        Size = new Size(300, 200),
                        StartPosition = FormStartPosition.CenterParent,
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        MaximizeBox = false
                    };

                    var statusLabel = new Label
                    {
                        Text = "Выберите новый статус:",
                        Location = new Point(20, 20),
                        AutoSize = true
                    };

                    var statusComboBox = new ComboBox
                    {
                        Location = new Point(20, 50),
                        Size = new Size(250, 20)
                    };

                    string currentStatus = reader["Status"].ToString();
                    if (currentStatus == "Поступил")
                    {
                        statusComboBox.Items.AddRange(new[] { "Принят", "Готовится", "Отменено" });
                    }
                    else if (currentStatus == "Принят")
                    {
                        statusComboBox.Items.AddRange(new[] { "Готовится", "Готово", "Отменено" });
                    }
                    else if (currentStatus == "Готовится")
                    {
                        statusComboBox.Items.AddRange(new[] { "Готово", "Отменено" });
                    }

                    statusComboBox.SelectedIndex = 0;

                    var saveButton = new Button
                    {
                        Text = "Сохранить",
                        BackColor = Color.FromArgb(70, 130, 180),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(100, 30),
                        Location = new Point(100, 100),
                        DialogResult = DialogResult.OK
                    };
                    saveButton.Click += (s, e) =>
                    {
                        string newStatus = statusComboBox.Text;

                        using (var updateCmd = new SQLiteCommand(connection))
                        {
                            updateCmd.CommandText = "UPDATE KitchenOrders SET Status = @status WHERE Id = @orderId";
                            updateCmd.Parameters.AddWithValue("@status", newStatus);
                            updateCmd.Parameters.AddWithValue("@orderId", orderId);
                            updateCmd.ExecuteNonQuery();
                        }

                        AddStatusHistory(orderId, newStatus, DateTime.Now);

                        MessageBox.Show($"Статус заказа #{orderId} изменен на: {newStatus}", "Статус изменен");
                        form.Close();
                        ShowCookOrders();
                    };

                    form.Controls.AddRange(new Control[] { statusLabel, statusComboBox, saveButton });
                    form.ShowDialog();
                }
            }
        }

        public void ShowIngredients()
        {
            form.UIHelper.ClearPanels();

            form.ingredientsPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Ингредиенты на складе",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.ingredientsPanel.Controls.Add(title);

            var ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "Моцарелла", Quantity = 5, Unit = "кг", MinQuantity = 2 },
                new Ingredient { Name = "Говядина", Quantity = 8, Unit = "кг", MinQuantity = 5 },
                new Ingredient { Name = "Салат", Quantity = 3, Unit = "кг", MinQuantity = 2 },
                new Ingredient { Name = "Томатный соус", Quantity = 10, Unit = "л", MinQuantity = 3 },
                new Ingredient { Name = "Тесто", Quantity = 15, Unit = "кг", MinQuantity = 5 },
                new Ingredient { Name = "Курица", Quantity = 7, Unit = "кг", MinQuantity = 3 },
                new Ingredient { Name = "Соус Цезарь", Quantity = 4, Unit = "л", MinQuantity = 1 }
            };

            int yPos = 60;
            foreach (var ingredient in ingredients)
            {
                var ingredientPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 60),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = ingredient.Quantity < ingredient.MinQuantity ? Color.FromArgb(255, 200, 200) : Color.White
                };

                var nameLabel = new Label
                {
                    Text = ingredient.Name,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                ingredientPanel.Controls.Add(nameLabel);

                var quantityLabel = new Label
                {
                    Text = $"{ingredient.Quantity} {ingredient.Unit} (мин: {ingredient.MinQuantity})",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(200, 10),
                    AutoSize = true
                };
                ingredientPanel.Controls.Add(quantityLabel);

                var requestButton = new Button
                {
                    Text = "Заказать",
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(80, 25),
                    Location = new Point(form.ClientSize.Width - 120, 15),
                    Font = new Font("Segoe UI", 9),
                    Tag = ingredient.Name
                };
                requestButton.Click += (s, e) => RequestIngredient(ingredient.Name);
                ingredientPanel.Controls.Add(requestButton);

                form.ingredientsPanel.Controls.Add(ingredientPanel);
                yPos += 70;
            }

            var historyButton = new Button
            {
                Text = "История заказов ингредиентов",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(250, 30),
                Location = new Point(20, yPos + 20),
                Font = new Font("Segoe UI", 10)
            };
            historyButton.Click += (s, e) => ShowIngredientRequestHistory();
            form.ingredientsPanel.Controls.Add(historyButton);

            form.Controls.Add(form.ingredientsPanel);
        }
        private List<IngredientRequest> GetIngredientRequests()
        {
            var requests = new List<IngredientRequest>();

            using (var cmd = new SQLiteCommand("SELECT * FROM IngredientRequests", connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        requests.Add(new IngredientRequest
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            IngredientName = reader["IngredientName"].ToString(),
                            Quantity = Convert.ToDouble(reader["Quantity"]),
                            Unit = reader["Unit"].ToString(),
                            Comment = reader["Comment"]?.ToString(),
                            RequestTime = Convert.ToDateTime(reader["RequestTime"]),
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }

            return requests;
        }
        public void ShowIngredientRequestHistory()
        {
            form.UIHelper.ClearPanels();

            form.ingredientHistoryPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "История заказов ингредиентов",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.ingredientHistoryPanel.Controls.Add(title);

            var requests = GetIngredientRequests();

            int yPos = 60;
            foreach (var request in requests.OrderByDescending(r => r.RequestTime))
            {
                var requestPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 80),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var nameLabel = new Label
                {
                    Text = $"Ингредиент: {request.IngredientName}",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                requestPanel.Controls.Add(nameLabel);

                var quantityLabel = new Label
                {
                    Text = $"Количество: {request.Quantity} {request.Unit}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 35),
                    AutoSize = true
                };
                requestPanel.Controls.Add(quantityLabel);

                var timeLabel = new Label
                {
                    Text = $"Время запроса: {request.RequestTime:dd.MM.yyyy HH:mm}",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(form.ClientSize.Width - 250, 10),
                    AutoSize = true
                };
                requestPanel.Controls.Add(timeLabel);

                var statusLabel = new Label
                {
                    Text = $"Статус: {request.Status}",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = request.Status == "Выполнен" ? Color.Green :
                               request.Status == "Отменен" ? Color.Red : Color.Blue,
                    Location = new Point(form.ClientSize.Width - 250, 35),
                    AutoSize = true
                };
                requestPanel.Controls.Add(statusLabel);

                form.ingredientHistoryPanel.Controls.Add(requestPanel);
                yPos += 90;
            }

            var backButton = new Button
            {
                Text = "Назад к ингредиентам",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 30),
                Location = new Point(20, yPos + 20),
                Font = new Font("Segoe UI", 10)
            };
            backButton.Click += (s, e) => ShowIngredients();
            form.ingredientHistoryPanel.Controls.Add(backButton);

            form.Controls.Add(form.ingredientHistoryPanel);
        }
        public void ChangeCookStatus()
        {
            var form = new Form
            {
                Text = "Изменение статуса повара",
                Size = new Size(300, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var statusLabel = new Label
            {
                Text = "Выберите новый статус:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            var statusComboBox = new ComboBox
            {
                Location = new Point(20, 50),
                Size = new Size(250, 20)
            };
            statusComboBox.Items.AddRange(new[] { "Доступен", "Занят", "Перерыв", "Недоступен" });

            var saveButton = new Button
            {
                Text = "Сохранить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(100, 100),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) =>
            {
                MessageBox.Show($"Статус повара изменен на: {statusComboBox.Text}", "Статус изменен");
                form.Close();
            };

            form.Controls.AddRange(new Control[] { statusLabel, statusComboBox, saveButton });
            form.ShowDialog();
        }


        public void RequestIngredient(string ingredientName)
        {
            var form = new Form
            {
                Text = $"Заказ ингредиента: {ingredientName}",
                Size = new Size(350, 250),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var quantityLabel = new Label
            {
                Text = "Количество:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            var quantityBox = new NumericUpDown
            {
                Location = new Point(20, 50),
                Size = new Size(100, 20),
                Minimum = 1,
                Maximum = 100,
                DecimalPlaces = 2,
                Increment = 0.5m
            };

            var unitLabel = new Label
            {
                Text = "Единица измерения:",
                Location = new Point(150, 20),
                AutoSize = true
            };

            var unitComboBox = new ComboBox
            {
                Location = new Point(150, 50),
                Size = new Size(100, 20),
                Items = { "кг", "л", "шт", "г", "мл" },
                SelectedIndex = 0
            };

            var commentLabel = new Label
            {
                Text = "Комментарий:",
                Location = new Point(20, 90),
                AutoSize = true
            };

            var commentBox = new TextBox
            {
                Location = new Point(20, 120),
                Size = new Size(300, 20),
                Multiline = true,
                Height = 50
            };

            var saveButton = new Button
            {
                Text = "Отправить запрос",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(100, 180),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) =>
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = @"INSERT INTO IngredientRequests 
                                      (IngredientName, Quantity, Unit, Comment, RequestTime, Status)
                                      VALUES (@name, @quantity, @unit, @comment, @time, 'В обработке')";
                    cmd.Parameters.AddWithValue("@name", ingredientName);
                    cmd.Parameters.AddWithValue("@quantity", (double)quantityBox.Value);
                    cmd.Parameters.AddWithValue("@unit", unitComboBox.Text);
                    cmd.Parameters.AddWithValue("@comment", commentBox.Text);
                    cmd.Parameters.AddWithValue("@time", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show($"Запрос на {quantityBox.Value} {unitComboBox.Text} {ingredientName} отправлен", "Запрос отправлен");
                form.Close();
                ShowIngredients();
            };

            form.Controls.AddRange(new Control[] {
                quantityLabel, quantityBox,
                unitLabel, unitComboBox,
                commentLabel, commentBox,
                saveButton
            });
            form.ShowDialog();
        }
    }
}

