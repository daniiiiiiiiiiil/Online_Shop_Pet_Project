using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class SellerHelper
    {
        private MainMenuForm form;
        private SQLiteConnection connection;
        private string dbPath = "OnlineShop.db";
        public List<Product> products = new List<Product>();

        public SellerHelper(MainMenuForm form)
        {
            this.form = form;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            connection.Open();

            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CustomerName TEXT NOT NULL,
                CustomerPhone TEXT NOT NULL,
                OrderDate TEXT NOT NULL,
                TotalAmount REAL NOT NULL,
                Status TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS OrderItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER NOT NULL,
                ProductName TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                Price REAL NOT NULL,
                FOREIGN KEY(OrderId) REFERENCES Orders(Id)
            );

            CREATE TABLE IF NOT EXISTS Returns (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER NOT NULL,
                ReturnDate TEXT NOT NULL,
                TotalAmount REAL NOT NULL,
                Status TEXT NOT NULL,
                FOREIGN KEY(OrderId) REFERENCES Orders(Id)
            );

            CREATE TABLE IF NOT EXISTS ReturnItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ReturnId INTEGER NOT NULL,
                ProductName TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                Price REAL NOT NULL,
                FOREIGN KEY(ReturnId) REFERENCES Returns(Id)
            );";
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Orders", connection))
            {
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 0)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var cmdOrder = new SQLiteCommand(
                                "INSERT INTO Orders (CustomerName, CustomerPhone, OrderDate, TotalAmount, Status) " +
                                "VALUES (@name, @phone, @date, @total, @status); " +
                                "SELECT last_insert_rowid();", connection, transaction))
                            {
                                cmdOrder.Parameters.AddWithValue("@name", "Иванов Иван Иванович");
                                cmdOrder.Parameters.AddWithValue("@phone", "+7 (123) 456-78-90");
                                cmdOrder.Parameters.AddWithValue("@date", DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"));
                                cmdOrder.Parameters.AddWithValue("@total", 12500);
                                cmdOrder.Parameters.AddWithValue("@status", "Завершен");
                                int orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());

                                var orderItems = new List<OrderItem>
                            {
                                new OrderItem { ProductName = "Смартфон Samsung Galaxy S23", Quantity = 1, Price = 79990 },
                                new OrderItem { ProductName = "Чехол для Samsung Galaxy S23", Quantity = 1, Price = 2500 }
                            };

                                foreach (var item in orderItems)
                                {
                                    using (var cmdItem = new SQLiteCommand(
                                        "INSERT INTO OrderItems (OrderId, ProductName, Quantity, Price) " +
                                        "VALUES (@orderId, @name, @quantity, @price)", connection, transaction))
                                    {
                                        cmdItem.Parameters.AddWithValue("@orderId", orderId);
                                        cmdItem.Parameters.AddWithValue("@name", item.ProductName);
                                        cmdItem.Parameters.AddWithValue("@quantity", item.Quantity);
                                        cmdItem.Parameters.AddWithValue("@price", item.Price);
                                        cmdItem.ExecuteNonQuery();
                                    }
                                }
                            }

                            using (var cmdOrder = new SQLiteCommand(
                                "INSERT INTO Orders (CustomerName, CustomerPhone, OrderDate, TotalAmount, Status) " +
                                "VALUES (@name, @phone, @date, @total, @status); " +
                                "SELECT last_insert_rowid();", connection, transaction))
                            {
                                cmdOrder.Parameters.AddWithValue("@name", "Петров Петр Петрович");
                                cmdOrder.Parameters.AddWithValue("@phone", "+7 (987) 654-32-10");
                                cmdOrder.Parameters.AddWithValue("@date", DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd"));
                                cmdOrder.Parameters.AddWithValue("@total", 34990);
                                cmdOrder.Parameters.AddWithValue("@status", "Завершен");
                                int orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());

                                var orderItems = new List<OrderItem>
                            {
                                new OrderItem { ProductName = "Наушники Sony WH-1000XM5", Quantity = 1, Price = 34990 }
                            };

                                foreach (var item in orderItems)
                                {
                                    using (var cmdItem = new SQLiteCommand(
                                        "INSERT INTO OrderItems (OrderId, ProductName, Quantity, Price) " +
                                        "VALUES (@orderId, @name, @quantity, @price)", connection, transaction))
                                    {
                                        cmdItem.Parameters.AddWithValue("@orderId", orderId);
                                        cmdItem.Parameters.AddWithValue("@name", item.ProductName);
                                        cmdItem.Parameters.AddWithValue("@quantity", item.Quantity);
                                        cmdItem.Parameters.AddWithValue("@price", item.Price);
                                        cmdItem.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        public void ProcessOfflineOrder()
        {
            var form = new Form
            {
                Text = "Оформление оффлайн заказа",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var customerGroup = new GroupBox
            {
                Text = "Информация о клиенте",
                Location = new Point(20, 20),
                Size = new Size(550, 100),
                Font = new Font("Segoe UI", 10)
            };

            var nameLabel = new Label { Text = "ФИО:", Location = new Point(10, 25), AutoSize = true };
            var nameBox = new TextBox { Location = new Point(100, 25), Size = new Size(200, 20) };

            var phoneLabel = new Label { Text = "Телефон:", Location = new Point(10, 55), AutoSize = true };
            var phoneBox = new TextBox { Location = new Point(100, 55), Size = new Size(200, 20) };

            customerGroup.Controls.AddRange(new Control[] { nameLabel, nameBox, phoneLabel, phoneBox });

            var productsGroup = new GroupBox
            {
                Text = "Товары в заказе",
                Location = new Point(20, 140),
                Size = new Size(550, 250),
                Font = new Font("Segoe UI", 10)
            };

            var productsList = new ListBox
            {
                Location = new Point(10, 25),
                Size = new Size(250, 150),
                SelectionMode = SelectionMode.MultiExtended
            };

            using (var cmd = new SQLiteCommand("SELECT Name, Price FROM Products", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    productsList.Items.Add($"{reader.GetString(0)} - {reader.GetDecimal(1)} ₽");
                }
            }

            if (productsList.Items.Count == 0)
            {
                productsList.Items.AddRange(new[] {
                "Смартфон Samsung Galaxy S23 - 79990 ₽",
                "Наушники Sony WH-1000XM5 - 34990 ₽",
                "Чехол для Samsung Galaxy S23 - 2500 ₽",
                "Кабель USB-C - 990 ₽",
                "Внешний аккумулятор 10000 mAh - 4990 ₽"
            });
            }

            var selectedProductsList = new ListBox
            {
                Location = new Point(280, 25),
                Size = new Size(250, 150)
            };

            var addButton = new Button
            {
                Text = "Добавить →",
                Location = new Point(280, 180),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 8)
            };
            addButton.Click += (s, e) =>
            {
                foreach (var item in productsList.SelectedItems)
                {
                    if (!selectedProductsList.Items.Contains(item))
                        selectedProductsList.Items.Add(item);
                }
            };

            var removeButton = new Button
            {
                Text = "← Удалить",
                Location = new Point(380, 180),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 8)
            };
            removeButton.Click += (s, e) =>
            {
                while (selectedProductsList.SelectedItems.Count > 0)
                {
                    selectedProductsList.Items.Remove(selectedProductsList.SelectedItems[0]);
                }
            };

            productsGroup.Controls.AddRange(new Control[] {
            productsList, selectedProductsList, addButton, removeButton
        });

            var cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(150, 410),
                Size = new Size(100, 30),
                DialogResult = DialogResult.Cancel
            };

            var confirmButton = new Button
            {
                Text = "Подтвердить заказ",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(400, 410),
                DialogResult = DialogResult.OK
            };
            confirmButton.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(nameBox.Text) || string.IsNullOrWhiteSpace(phoneBox.Text))
                {
                    MessageBox.Show("Заполните информацию о клиенте", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (selectedProductsList.Items.Count == 0)
                {
                    MessageBox.Show("Добавьте товары в заказ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        decimal totalAmount = 0;
                        var orderItems = new List<OrderItem>();

                        foreach (var item in selectedProductsList.Items)
                        {
                            string itemStr = item.ToString();
                            int priceStart = itemStr.LastIndexOf('-') + 1;
                            int priceEnd = itemStr.LastIndexOf('₽') - 1;
                            string priceStr = itemStr.Substring(priceStart, priceEnd - priceStart).Trim();
                            decimal price = decimal.Parse(priceStr);

                            string productName = itemStr.Substring(0, priceStart - 1).Trim();

                            totalAmount += price;
                            orderItems.Add(new OrderItem
                            {
                                ProductName = productName,
                                Quantity = 1,
                                Price = price
                            });
                        }

                        using (var cmdOrder = new SQLiteCommand(
                            "INSERT INTO Orders (CustomerName, CustomerPhone, OrderDate, TotalAmount, Status) " +
                            "VALUES (@name, @phone, @date, @total, @status); " +
                            "SELECT last_insert_rowid();", connection, transaction))
                        {
                            cmdOrder.Parameters.AddWithValue("@name", nameBox.Text);
                            cmdOrder.Parameters.AddWithValue("@phone", phoneBox.Text);
                            cmdOrder.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                            cmdOrder.Parameters.AddWithValue("@total", totalAmount);
                            cmdOrder.Parameters.AddWithValue("@status", "Завершен");
                            int orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());

                            foreach (var item in orderItems)
                            {
                                using (var cmdItem = new SQLiteCommand(
                                    "INSERT INTO OrderItems (OrderId, ProductName, Quantity, Price) " +
                                    "VALUES (@orderId, @name, @quantity, @price)", connection, transaction))
                                {
                                    cmdItem.Parameters.AddWithValue("@orderId", orderId);
                                    cmdItem.Parameters.AddWithValue("@name", item.ProductName);
                                    cmdItem.Parameters.AddWithValue("@quantity", item.Quantity);
                                    cmdItem.Parameters.AddWithValue("@price", item.Price);
                                    cmdItem.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show($"Оффлайн заказ успешно оформлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        form.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            form.Controls.AddRange(new Control[] { customerGroup, productsGroup, cancelButton, confirmButton });
            form.ShowDialog();
        }

        public void ProcessProductReturn()
        {
            var returnForm = new Form
            {
                Text = "Возврат товара",
                Size = new Size(800, 600), 
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var orderGroup = new GroupBox
            {
                Text = "Информация о заказе",
                Location = new Point(20, 20),
                Size = new Size(750, 100),
                Font = new Font("Segoe UI", 10)
            };

            var orderLabel = new Label { Text = "Номер заказа:", Location = new Point(10, 25), AutoSize = true };
            var orderBox = new TextBox { Location = new Point(120, 25), Size = new Size(150, 25) };

            var phoneLabel = new Label { Text = "Телефон клиента:", Location = new Point(10, 55), AutoSize = true };
            var phoneBox = new TextBox { Location = new Point(120, 55), Size = new Size(200, 25), ReadOnly = true };

            var findButton = new Button
            {
                Text = "Найти заказ",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = new Point(330, 25),
                Font = new Font("Segoe UI", 10)
            };

            var orderInfoLabel = new Label
            {
                Text = "",
                Location = new Point(460, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            orderGroup.Controls.AddRange(new Control[] { orderLabel, orderBox, phoneLabel, phoneBox, findButton, orderInfoLabel });

            var productsGroup = new GroupBox
            {
                Text = "Товары для возврата",
                Location = new Point(20, 140),
                Size = new Size(750, 300),
                Font = new Font("Segoe UI", 10)
            };

            var productsList = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(720, 200),
                ReadOnly = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true
            };

            productsList.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "Select",
                HeaderText = "Выбрать",
                Width = 50
            });
            productsList.Columns.Add("ProductName", "Товар");
            productsList.Columns.Add("Quantity", "Количество");
            productsList.Columns.Add("Price", "Цена");
            productsList.Columns.Add("Total", "Сумма");

            productsList.Columns["Price"].DefaultCellStyle.Format = "C2";
            productsList.Columns["Total"].DefaultCellStyle.Format = "C2";
            productsList.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            productsList.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            productsList.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var returnsHistory = new DataGridView
            {
                Location = new Point(10, 230),
                Size = new Size(720, 60),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false
            };
            returnsHistory.Columns.Add("ReturnId", "Номер возврата");
            returnsHistory.Columns.Add("ReturnDate", "Дата возврата");
            returnsHistory.Columns.Add("Amount", "Сумма");

            productsGroup.Controls.Add(productsList);
            productsGroup.Controls.Add(returnsHistory);

            findButton.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(orderBox.Text))
                {
                    MessageBox.Show("Введите номер заказа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(orderBox.Text, out int orderId))
                {
                    MessageBox.Show("Некорректный номер заказа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (var cmd = new SQLiteCommand(
                        "SELECT CustomerName, CustomerPhone, OrderDate, TotalAmount FROM Orders WHERE Id = @orderId",
                        connection))
                    {
                        cmd.Parameters.AddWithValue("@orderId", orderId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                phoneBox.Text = reader.GetString(1);
                                orderInfoLabel.Text = $"Клиент: {reader.GetString(0)}, Дата: {reader.GetDateTime(2):dd.MM.yyyy}, Сумма: {reader.GetDecimal(3):C2}";
                                productsList.Rows.Clear();
                                using (var cmdItems = new SQLiteCommand(
                                    "SELECT ProductName, Quantity, Price FROM OrderItems WHERE OrderId = @orderId",
                                    connection))
                                {
                                    cmdItems.Parameters.AddWithValue("@orderId", orderId);
                                    using (var itemsReader = cmdItems.ExecuteReader())
                                    {
                                        while (itemsReader.Read())
                                        {
                                            decimal price = itemsReader.GetDecimal(2);
                                            int quantity = itemsReader.GetInt32(1);
                                            productsList.Rows.Add(
                                                false, 
                                                itemsReader.GetString(0),
                                                quantity,
                                                price,
                                                price * quantity
                                            );
                                        }
                                    }
                                }

                                returnsHistory.Rows.Clear();
                                using (var cmdReturns = new SQLiteCommand(
                                    "SELECT Id, ReturnDate, TotalAmount FROM Returns WHERE OrderId = @orderId",
                                    connection))
                                {
                                    cmdReturns.Parameters.AddWithValue("@orderId", orderId);
                                    using (var returnsReader = cmdReturns.ExecuteReader())
                                    {
                                        while (returnsReader.Read())
                                        {
                                            returnsHistory.Rows.Add(
                                                returnsReader.GetInt32(0),
                                                returnsReader.GetDateTime(1).ToString("dd.MM.yyyy"),
                                                returnsReader.GetDecimal(2)
                                            );
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Заказ не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(150, 460),
                Size = new Size(120, 40),
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 10)
            };

            var confirmButton = new Button
            {
                Text = "Подтвердить возврат",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 40),
                Location = new Point(550, 460),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            confirmButton.FlatAppearance.BorderSize = 0;

            confirmButton.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(orderBox.Text) || !int.TryParse(orderBox.Text, out int orderId))
                {
                    MessageBox.Show("Некорректный номер заказа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedItems = new List<OrderItem>();
                decimal totalAmount = 0;

                foreach (DataGridViewRow row in productsList.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["Select"].Value))
                    {
                        string productName = row.Cells["ProductName"].Value.ToString();
                        int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                        decimal price = Convert.ToDecimal(row.Cells["Price"].Value);

                        selectedItems.Add(new OrderItem
                        {
                            ProductName = productName,
                            Quantity = quantity,
                            Price = price
                        });

                        totalAmount += price * quantity;
                    }
                }

                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("Выберите товары для возврата", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var cmdReturn = new SQLiteCommand(
                            "INSERT INTO Returns (OrderId, ReturnDate, TotalAmount, Status) " +
                            "VALUES (@orderId, @date, @total, @status); " +
                            "SELECT last_insert_rowid();", connection, transaction))
                        {
                            cmdReturn.Parameters.AddWithValue("@orderId", orderId);
                            cmdReturn.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                            cmdReturn.Parameters.AddWithValue("@total", totalAmount);
                            cmdReturn.Parameters.AddWithValue("@status", "Завершен");
                            int returnId = Convert.ToInt32(cmdReturn.ExecuteScalar());

                            foreach (var item in selectedItems)
                            {
                                using (var cmdItem = new SQLiteCommand(
                                    "INSERT INTO ReturnItems (ReturnId, ProductName, Quantity, Price) " +
                                    "VALUES (@returnId, @name, @quantity, @price)", connection, transaction))
                                {
                                    cmdItem.Parameters.AddWithValue("@returnId", returnId);
                                    cmdItem.Parameters.AddWithValue("@name", item.ProductName);
                                    cmdItem.Parameters.AddWithValue("@quantity", item.Quantity);
                                    cmdItem.Parameters.AddWithValue("@price", item.Price);
                                    cmdItem.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show($"Возврат успешно оформлен на сумму {totalAmount:C2}!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        returnForm.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при оформлении возврата: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            returnForm.Controls.AddRange(new Control[] { orderGroup, productsGroup, cancelButton, confirmButton });
            returnForm.ShowDialog();
        }
        public void ShowPurchaseHistory()
        {
            form.UIHelper.ClearPanels();

            form.mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = "История покупок",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20),
                ForeColor = Color.FromArgb(70, 130, 180)
            };
            form.mainPanel.Controls.Add(titleLabel);

            var dataGridView = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(form.ClientSize.Width - 40, form.ClientSize.Height - 150),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };

            dataGridView.Columns.Add("Id", "Номер заказа");
            dataGridView.Columns.Add("CustomerName", "Клиент");
            dataGridView.Columns.Add("CustomerPhone", "Телефон");
            dataGridView.Columns.Add("OrderDate", "Дата заказа");
            dataGridView.Columns.Add("TotalAmount", "Сумма");
            dataGridView.Columns.Add("Status", "Статус");

            dataGridView.Columns["OrderDate"].DefaultCellStyle.Format = "dd.MM.yyyy";
            dataGridView.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
            dataGridView.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            var refreshButton = new Button
            {
                Text = "Обновить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(20, 30),
                Font = new Font("Segoe UI", 10)
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += (s, e) => LoadPurchaseData(dataGridView);

            var detailsButton = new Button
            {
                Text = "Детали заказа",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = new Point(130, 30),
                Font = new Font("Segoe UI", 10)
            };
            detailsButton.Click += (s, e) => ShowOrderDetails(dataGridView);

            LoadPurchaseData(dataGridView);

            form.mainPanel.Controls.Add(refreshButton);
            form.mainPanel.Controls.Add(detailsButton);
            form.mainPanel.Controls.Add(dataGridView);
            form.Controls.Add(form.mainPanel);
            form.mainPanel.BringToFront();
        }

        private void LoadPurchaseData(DataGridView dataGridView)
        {
            try
            {
                dataGridView.Rows.Clear();

                using (var cmd = new SQLiteCommand(
                    "SELECT Id, CustomerName, CustomerPhone, OrderDate, TotalAmount, Status FROM Orders ORDER BY OrderDate DESC",
                    connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataGridView.Rows.Add(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetDateTime(3),
                                reader.GetDecimal(4),
                                reader.GetString(5)
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке истории покупок: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowOrderDetails(DataGridView dataGridView)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заказ для просмотра деталей", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int orderId = (int)dataGridView.SelectedRows[0].Cells["Id"].Value;

            var detailsForm = new Form
            {
                Text = $"Детали заказа №{orderId}",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var dataGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false
            };

            dataGrid.Columns.Add("ProductName", "Товар");
            dataGrid.Columns.Add("Quantity", "Количество");
            dataGrid.Columns.Add("Price", "Цена");
            dataGrid.Columns.Add("Total", "Сумма");

            dataGrid.Columns["Price"].DefaultCellStyle.Format = "C2";
            dataGrid.Columns["Total"].DefaultCellStyle.Format = "C2";
            dataGrid.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGrid.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGrid.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            try
            {
                using (var cmd = new SQLiteCommand(
                    "SELECT ProductName, Quantity, Price FROM OrderItems WHERE OrderId = @orderId",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal price = reader.GetDecimal(2);
                            int quantity = reader.GetInt32(1);
                            dataGrid.Rows.Add(
                                reader.GetString(0),
                                quantity,
                                price,
                                price * quantity
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке деталей заказа: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            detailsForm.Controls.Add(dataGrid);
            detailsForm.ShowDialog();
        }
    }
}