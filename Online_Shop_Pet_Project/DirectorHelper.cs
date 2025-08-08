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
    internal class DirectorHelper
    {
        private MainMenuForm form;
        private SQLiteConnection connection;
        private string dbPath = "OnlineShop.db";

        public DirectorHelper(MainMenuForm form)
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
        CREATE TABLE IF NOT EXISTS Employees (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Position TEXT NOT NULL,
            Salary REAL NOT NULL,
            HireDate TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Shifts (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            EmployeeId INTEGER NOT NULL,
            ShiftDate TEXT NOT NULL,
            ShiftType TEXT NOT NULL,
            ShiftSalary REAL NOT NULL,
            FOREIGN KEY(EmployeeId) REFERENCES Employees(Id)
        );

        CREATE TABLE IF NOT EXISTS Products (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Quantity INTEGER NOT NULL,
            Price REAL NOT NULL
        );

        CREATE TABLE IF NOT EXISTS GoodsReceipt (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            ProductId INTEGER,
            Quantity INTEGER NOT NULL,
            Supplier TEXT NOT NULL,
            ReceiptDate TEXT NOT NULL,
            FOREIGN KEY(ProductId) REFERENCES Products(Id)
        );

        CREATE TABLE IF NOT EXISTS GoodsDisposal (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            ProductId INTEGER NOT NULL,
            Quantity INTEGER NOT NULL,
            Reason TEXT NOT NULL,
            DisposalDate TEXT NOT NULL,
            FOREIGN KEY(ProductId) REFERENCES Products(Id)
        );

        CREATE TABLE IF NOT EXISTS Revenue (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Amount REAL NOT NULL,
            RecordDate TEXT NOT NULL
        );";
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Products", connection))
            {
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 0)
                {
                    var exampleProducts = new List<Product>
            {
                new Product { Name = "Молоко Простоквашино 2.5%", Quantity = 50, Price = 85.50m },
                new Product { Name = "Хлеб Бородинский нарезной", Quantity = 30, Price = 45.30m },
                new Product { Name = "Яйца куриные С1 (10 шт)", Quantity = 25, Price = 120.00m },
                new Product { Name = "Сыр Российский 45%", Quantity = 20, Price = 350.75m },
                new Product { Name = "Колбаса Докторская в/с", Quantity = 15, Price = 280.50m },
                new Product { Name = "Чай Greenfield классический", Quantity = 40, Price = 150.00m },
                new Product { Name = "Кофе Jacobs Monarch", Quantity = 20, Price = 450.00m },
                new Product { Name = "Сахар песок 1кг", Quantity = 35, Price = 65.00m },
                new Product { Name = "Масло подсолнечное", Quantity = 25, Price = 110.00m },
                new Product { Name = "Макароны Barilla", Quantity = 30, Price = 95.00m }
            };

                    foreach (var product in exampleProducts)
                    {
                        using (var insertCmd = new SQLiteCommand(
                            "INSERT INTO Products (Name, Quantity, Price) VALUES (@name, @quantity, @price)",
                            connection))
                        {
                            insertCmd.Parameters.AddWithValue("@name", product.Name);
                            insertCmd.Parameters.AddWithValue("@quantity", product.Quantity);
                            insertCmd.Parameters.AddWithValue("@price", product.Price);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            LoadProducts();
        }

        private void LoadProducts()
        {
            products.Clear();
            using (var cmd = new SQLiteCommand("SELECT * FROM Products", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Quantity = reader.GetInt32(2),
                        Price = reader.GetDecimal(3)
                    });
                }
            }
        }

        public List<Product> products = new List<Product>();

        public void ShowHireEmployeeForm()
        {
            var form = new Form
            {
                Text = "Принятие нового сотрудника",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var nameLabel = new Label { Text = "ФИО:", Location = new Point(20, 20), AutoSize = true };
            var nameBox = new TextBox { Location = new Point(120, 20), Size = new Size(300, 20) };

            var positionLabel = new Label { Text = "Должность:", Location = new Point(20, 60), AutoSize = true };
            var positionBox = new ComboBox { Location = new Point(120, 60), Size = new Size(300, 20) };
            positionBox.Items.AddRange(new[] { "Продавец", "Курьер", "Повар", "Работник зала", "Техподдержка" });

            var salaryLabel = new Label { Text = "Зарплата:", Location = new Point(20, 100), AutoSize = true };
            var salaryBox = new NumericUpDown { Location = new Point(120, 100), Size = new Size(100, 20), Minimum = 10000, Maximum = 1000000 };

            var startDateLabel = new Label { Text = "Дата приема:", Location = new Point(20, 140), AutoSize = true };
            var startDatePicker = new DateTimePicker { Location = new Point(120, 140), Size = new Size(150, 20) };

            var saveButton = new Button
            {
                Text = "Сохранить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(150, 200),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) =>
            {
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO Employees (Name, Position, Salary, HireDate) VALUES (@name, @position, @salary, @hireDate)",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@name", nameBox.Text);
                    cmd.Parameters.AddWithValue("@position", positionBox.Text);
                    cmd.Parameters.AddWithValue("@salary", salaryBox.Value);
                    cmd.Parameters.AddWithValue("@hireDate", startDatePicker.Value.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();
                }
                form.Close();
            };

            form.Controls.AddRange(new Control[] { nameLabel, nameBox, positionLabel, positionBox,
                                salaryLabel, salaryBox, startDateLabel, startDatePicker,
                                saveButton });

            form.ShowDialog();
        }

        public void ShowShiftsManagementForm()
        {
            var form = new Form
            {
                Text = "Управление сменами",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var employeesList = new ListBox
            {
                Location = new Point(20, 20),
                Size = new Size(200, 300),
                SelectionMode = SelectionMode.MultiSimple
            };

            using (var cmd = new SQLiteCommand("SELECT Id, Name, Position FROM Employees", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    employeesList.Items.Add(new EmployeeItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Position = reader.GetString(2)
                    });
                }
            }

            var dateLabel = new Label { Text = "Дата смены:", Location = new Point(250, 20), AutoSize = true };
            var datePicker = new DateTimePicker { Location = new Point(350, 20), Size = new Size(150, 20) };

            var shiftTypeLabel = new Label { Text = "Тип смены:", Location = new Point(250, 60), AutoSize = true };
            var shiftTypeBox = new ComboBox { Location = new Point(350, 60), Size = new Size(150, 20) };
            shiftTypeBox.Items.AddRange(new[] { "Утро (8:00-16:00)", "День (12:00-20:00)", "Вечер (16:00-24:00)" });

            var salaryLabel = new Label { Text = "Зарплата за смену:", Location = new Point(250, 100), AutoSize = true };
            var salaryBox = new NumericUpDown { Location = new Point(350, 100), Size = new Size(100, 20), Minimum = 500, Maximum = 10000 };

            var saveButton = new Button
            {
                Text = "Назначить смены",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(350, 140),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) =>
            {
                foreach (EmployeeItem selected in employeesList.SelectedItems)
                {
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Shifts (EmployeeId, ShiftDate, ShiftType, ShiftSalary) VALUES (@employeeId, @shiftDate, @shiftType, @shiftSalary)",
                        connection))
                    {
                        cmd.Parameters.AddWithValue("@employeeId", selected.Id);
                        cmd.Parameters.AddWithValue("@shiftDate", datePicker.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@shiftType", shiftTypeBox.Text);
                        cmd.Parameters.AddWithValue("@shiftSalary", salaryBox.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                form.Close();
            };

            var viewHistoryButton = new Button
            {
                Text = "История смен",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(350, 180),
                DialogResult = DialogResult.OK
            };
            viewHistoryButton.Click += (s, e) => ShowShiftsHistory();

            form.Controls.AddRange(new Control[] { employeesList, dateLabel, datePicker,
                                shiftTypeLabel, shiftTypeBox, salaryLabel, salaryBox,
                                saveButton, viewHistoryButton });

            form.ShowDialog();
        }

        public void ShowShiftsHistory()
        {
            var form = new Form
            {
                Text = "История смен сотрудников",
                Size = new Size(800, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };

            dataGridView.Columns.Add("EmployeeName", "Сотрудник");
            dataGridView.Columns.Add("Position", "Должность");
            dataGridView.Columns.Add("ShiftDate", "Дата смены");
            dataGridView.Columns.Add("ShiftType", "Тип смены");
            dataGridView.Columns.Add("ShiftSalary", "Зарплата за смену");

            using (var cmd = new SQLiteCommand(
                @"SELECT e.Name, e.Position, s.ShiftDate, s.ShiftType, s.ShiftSalary 
                  FROM Shifts s 
                  JOIN Employees e ON s.EmployeeId = e.Id 
                  ORDER BY s.ShiftDate DESC", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    dataGridView.Rows.Add(
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetDecimal(4)
                    );
                }
            }

            var totalSalary = 0m;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["ShiftSalary"].Value != null)
                {
                    totalSalary += Convert.ToDecimal(row.Cells["ShiftSalary"].Value);
                }
            }

            var totalLabel = new Label
            {
                Text = $"Общая сумма выплат за смены: {totalSalary} ₽",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Height = 30
            };

            var panel = new Panel { Dock = DockStyle.Fill };
            panel.Controls.Add(dataGridView);
            panel.Controls.Add(totalLabel);

            form.Controls.Add(panel);
            form.ShowDialog();
        }

        public void ShowReceiveGoodsForm()
        {
            var form = new Form
            {
                Text = "Прием товара",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var productLabel = new Label { Text = "Товар:", Location = new Point(20, 20), AutoSize = true };
            var productBox = new ComboBox { Location = new Point(120, 20), Size = new Size(300, 20) };
            productBox.Items.AddRange(products.Select(p => p.Name).ToArray());

            var quantityLabel = new Label { Text = "Количество:", Location = new Point(20, 60), AutoSize = true };
            var quantityBox = new NumericUpDown { Location = new Point(120, 60), Size = new Size(100, 20), Minimum = 1, Maximum = 1000 };

            var supplierLabel = new Label { Text = "Поставщик:", Location = new Point(20, 100), AutoSize = true };
            var supplierBox = new TextBox { Location = new Point(120, 100), Size = new Size(300, 20) };

            var dateLabel = new Label { Text = "Дата приема:", Location = new Point(20, 140), AutoSize = true };
            var datePicker = new DateTimePicker { Location = new Point(120, 140), Size = new Size(150, 20) };

            var saveButton = new Button
            {
                Text = "Подтвердить прием",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(150, 200),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) =>
            {
                var selectedProduct = products.First(p => p.Name == productBox.Text);

                using (var cmd = new SQLiteCommand(
                    "INSERT INTO GoodsReceipt (ProductId, Quantity, Supplier, ReceiptDate) VALUES (@productId, @quantity, @supplier, @receiptDate)",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@productId", selectedProduct.Id);
                    cmd.Parameters.AddWithValue("@quantity", quantityBox.Value);
                    cmd.Parameters.AddWithValue("@supplier", supplierBox.Text);
                    cmd.Parameters.AddWithValue("@receiptDate", datePicker.Value.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SQLiteCommand(
                    "UPDATE Products SET Quantity = Quantity + @quantity WHERE Id = @productId",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@quantity", quantityBox.Value);
                    cmd.Parameters.AddWithValue("@productId", selectedProduct.Id);
                    cmd.ExecuteNonQuery();
                }

                LoadProducts();
                form.Close();
            };

            form.Controls.AddRange(new Control[] { productLabel, productBox, quantityLabel, quantityBox,
                                supplierLabel, supplierBox, dateLabel, datePicker,
                                saveButton });

            form.ShowDialog();
        }

        public void ShowDisposeGoodsForm()
        {
            var form = new Form
            {
                Text = "Утилизация товара",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var productLabel = new Label { Text = "Товар:", Location = new Point(20, 20), AutoSize = true };
            var productBox = new ComboBox
            {
                Location = new Point(120, 20),
                Size = new Size(300, 20),
                DisplayMember = "Name",
                ValueMember = "Id"
            };

            var availableProducts = products.Where(p => p.Quantity > 0).ToList();

            if (availableProducts.Count == 0)
            {
                availableProducts = new List<Product>
        {
            new Product { Id = -1, Name = "Молоко Простоквашино 2.5%", Quantity = 10, Price = 85.50m },
            new Product { Id = -2, Name = "Хлеб Бородинский нарезной", Quantity = 15, Price = 45.30m },
            new Product { Id = -3, Name = "Яйца куриные С1 (10 шт)", Quantity = 8, Price = 120.00m },
            new Product { Id = -4, Name = "Сыр Российский 45%", Quantity = 5, Price = 350.75m },
            new Product { Id = -5, Name = "Колбаса Докторская в/с", Quantity = 7, Price = 280.50m }
        };
            }

            productBox.DataSource = availableProducts;

            var quantityLabel = new Label { Text = "Количество:", Location = new Point(20, 60), AutoSize = true };
            var quantityBox = new NumericUpDown
            {
                Location = new Point(120, 60),
                Size = new Size(100, 20),
                Minimum = 1,
                Maximum = 1000,
                Value = 1
            };

            productBox.SelectedIndexChanged += (s, e) =>
            {
                if (productBox.SelectedItem is Product selectedProduct)
                {
                    quantityBox.Maximum = selectedProduct.Quantity;
                }
            };

            if (productBox.SelectedItem is Product initialProduct)
            {
                quantityBox.Maximum = initialProduct.Quantity;
            }

            var reasonLabel = new Label { Text = "Причина утилизации:", Location = new Point(20, 100), AutoSize = true };
            var reasonBox = new ComboBox
            {
                Location = new Point(120, 100),
                Size = new Size(300, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            reasonBox.Items.AddRange(new[] { "Истек срок годности", "Повреждение", "Бракованный товар", "Возврат от покупателя", "Другая причина" });
            reasonBox.SelectedIndex = 0;

            var dateLabel = new Label { Text = "Дата утилизации:", Location = new Point(20, 140), AutoSize = true };
            var datePicker = new DateTimePicker
            {
                Location = new Point(120, 140),
                Size = new Size(150, 20),
                Value = DateTime.Today
            };

            var saveButton = new Button
            {
                Text = "Подтвердить утилизацию",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 30),
                Location = new Point(150, 200),
                DialogResult = DialogResult.OK
            };

            saveButton.Click += (s, e) =>
            {
                if (productBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите товар", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var selectedProduct = (Product)productBox.SelectedItem;

                if (quantityBox.Value <= 0)
                {
                    MessageBox.Show("Количество должно быть больше 0", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (selectedProduct.Quantity < quantityBox.Value)
                {
                    MessageBox.Show($"Недостаточно товара на складе. Доступно: {selectedProduct.Quantity}",
                                  "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(reasonBox.Text))
                {
                    MessageBox.Show("Пожалуйста, укажите причину утилизации", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new SQLiteCommand(
                                "INSERT INTO GoodsDisposal (ProductId, Quantity, Reason, DisposalDate) " +
                                "VALUES (@productId, @quantity, @reason, @disposalDate)",
                                connection, transaction))
                            {
                                object productIdParam = selectedProduct.Id > 0 ? (object)selectedProduct.Id : DBNull.Value;
                                cmd.Parameters.AddWithValue("@productId", productIdParam);
                                cmd.Parameters.AddWithValue("@quantity", quantityBox.Value);
                                cmd.Parameters.AddWithValue("@reason", reasonBox.Text);
                                cmd.Parameters.AddWithValue("@disposalDate", datePicker.Value.ToString("yyyy-MM-dd"));
                                cmd.ExecuteNonQuery();
                            }

                            if (selectedProduct.Id > 0)
                            {
                                using (var cmd = new SQLiteCommand(
                                    "UPDATE Products SET Quantity = Quantity - @quantity WHERE Id = @productId",
                                    connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@quantity", quantityBox.Value);
                                    cmd.Parameters.AddWithValue("@productId", selectedProduct.Id);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            LoadProducts();

                            MessageBox.Show("Утилизация товара успешно зарегистрирована",
                                          "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            form.Close();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка при утилизации товара: {ex.Message}",
                                          "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (SQLiteException sqlEx)
                {
                    MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}",
                                  "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var viewHistoryButton = new Button
            {
                Text = "История утилизации",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 30),
                Location = new Point(150, 250),
                DialogResult = DialogResult.OK
            };
            viewHistoryButton.Click += (s, e) => ShowDisposedGoodsHistory();

            form.Controls.AddRange(new Control[] { productLabel, productBox, quantityLabel, quantityBox,
        reasonLabel, reasonBox, dateLabel, datePicker,
        saveButton, viewHistoryButton });

            form.ShowDialog();
        }

        public void ShowDisposedGoodsHistory()
{
    var form = new Form
    {
        Text = "История утилизации товаров",
        Size = new Size(800, 500),
        StartPosition = FormStartPosition.CenterParent,
        FormBorderStyle = FormBorderStyle.FixedDialog,
        MaximizeBox = false
    };

    var dataGridView = new DataGridView
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        AllowUserToAddRows = false,
        RowHeadersVisible = false
    };

    dataGridView.Columns.Add("ProductName", "Товар");
    dataGridView.Columns.Add("Quantity", "Количество");
    dataGridView.Columns.Add("Reason", "Причина");
    dataGridView.Columns.Add("DisposalDate", "Дата утилизации");

    using (var cmd = new SQLiteCommand(
        @"SELECT 
            CASE WHEN p.Name IS NULL THEN 'Пример товара' ELSE p.Name END as Name, 
            gd.Quantity, gd.Reason, gd.DisposalDate 
          FROM GoodsDisposal gd 
          LEFT JOIN Products p ON gd.ProductId = p.Id 
          ORDER BY gd.DisposalDate DESC", connection))
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            dataGridView.Rows.Add(
                reader.GetString(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetString(3)
            );
        }
    }

    var totalDisposed = 0;
    foreach (DataGridViewRow row in dataGridView.Rows)
    {
        if (row.Cells["Quantity"].Value != null)
        {
            totalDisposed += Convert.ToInt32(row.Cells["Quantity"].Value);
        }
    }

    var totalLabel = new Label
    {
        Text = $"Всего утилизировано товаров: {totalDisposed}",
        Dock = DockStyle.Bottom,
        TextAlign = ContentAlignment.MiddleRight,
        Font = new Font("Arial", 10, FontStyle.Bold),
        Height = 30
    };

    var panel = new Panel { Dock = DockStyle.Fill };
    panel.Controls.Add(dataGridView);
    panel.Controls.Add(totalLabel);

    form.Controls.Add(panel);
    form.ShowDialog();
}

        public void ShowRevenueOptions()
        {
            form.UIHelper.ClearPanels();

            var revenuePanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Просмотр выручки",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            revenuePanel.Controls.Add(title);

            var dayButton = new Button
            {
                Text = "За день",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(20, 70),
                Font = new Font("Segoe UI", 10)
            };
            dayButton.Click += (s, e) => ShowRevenueReport("day");
            revenuePanel.Controls.Add(dayButton);

            var weekButton = new Button
            {
                Text = "За неделю",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(190, 70),
                Font = new Font("Segoe UI", 10)
            };
            weekButton.Click += (s, e) => ShowRevenueReport("week");
            revenuePanel.Controls.Add(weekButton);

            var monthButton = new Button
            {
                Text = "За месяц",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(360, 70),
                Font = new Font("Segoe UI", 10)
            };
            monthButton.Click += (s, e) => ShowRevenueReport("month");
            revenuePanel.Controls.Add(monthButton);

            var yearButton = new Button
            {
                Text = "За год",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(530, 70),
                Font = new Font("Segoe UI", 10)
            };
            yearButton.Click += (s, e) => ShowRevenueReport("year");
            revenuePanel.Controls.Add(yearButton);

            var specificDateLabel = new Label
            {
                Text = "Конкретная дата:",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 140),
                AutoSize = true
            };
            revenuePanel.Controls.Add(specificDateLabel);

            var datePicker = new DateTimePicker
            {
                Location = new Point(20, 170),
                Size = new Size(150, 20),
                Format = DateTimePickerFormat.Short
            };
            revenuePanel.Controls.Add(datePicker);

            var specificDateButton = new Button
            {
                Text = "Показать выручку",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 30),
                Location = new Point(190, 170),
                Font = new Font("Segoe UI", 10)
            };
            specificDateButton.Click += (s, e) => ShowRevenueReport("specific", datePicker.Value);
            revenuePanel.Controls.Add(specificDateButton);

            var periodLabel = new Label
            {
                Text = "Период:",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 220),
                AutoSize = true
            };
            revenuePanel.Controls.Add(periodLabel);

            var fromDateLabel = new Label
            {
                Text = "С:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 250),
                AutoSize = true
            };
            revenuePanel.Controls.Add(fromDateLabel);

            var fromDatePicker = new DateTimePicker
            {
                Location = new Point(50, 250),
                Size = new Size(150, 20),
                Format = DateTimePickerFormat.Short
            };
            revenuePanel.Controls.Add(fromDatePicker);

            var toDateLabel = new Label
            {
                Text = "По:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(220, 250),
                AutoSize = true
            };
            revenuePanel.Controls.Add(toDateLabel);

            var toDatePicker = new DateTimePicker
            {
                Location = new Point(260, 250),
                Size = new Size(150, 20),
                Format = DateTimePickerFormat.Short
            };
            revenuePanel.Controls.Add(toDatePicker);

            var periodButton = new Button
            {
                Text = "Показать выручку за период",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(220, 30),
                Location = new Point(20, 290),
                Font = new Font("Segoe UI", 10)
            };
            periodButton.Click += (s, e) => ShowRevenueReport("period", fromDatePicker.Value, toDatePicker.Value);
            revenuePanel.Controls.Add(periodButton);

            form.Controls.Add(revenuePanel);
        }

        public void ShowRevenueReport(string period, DateTime? fromDate = null, DateTime? toDate = null)
        {
            decimal revenue = 0;
            string periodText = "";
            string query = "";

            switch (period)
            {
                case "day":
                    query = "SELECT SUM(Amount) FROM Revenue WHERE date(RecordDate) = date('now')";
                    periodText = "за день";
                    break;
                case "week":
                    query = "SELECT SUM(Amount) FROM Revenue WHERE date(RecordDate) BETWEEN date('now', '-7 days') AND date('now')";
                    periodText = "за неделю";
                    break;
                case "month":
                    query = "SELECT SUM(Amount) FROM Revenue WHERE strftime('%Y-%m', RecordDate) = strftime('%Y-%m', 'now')";
                    periodText = "за месяц";
                    break;
                case "year":
                    query = "SELECT SUM(Amount) FROM Revenue WHERE strftime('%Y', RecordDate) = strftime('%Y', 'now')";
                    periodText = "за год";
                    break;
                case "specific":
                    query = $"SELECT SUM(Amount) FROM Revenue WHERE date(RecordDate) = date('{fromDate.Value.ToString("yyyy-MM-dd")}')";
                    periodText = $"за {fromDate.Value.ToShortDateString()}";
                    break;
                case "period":
                    query = $"SELECT SUM(Amount) FROM Revenue WHERE date(RecordDate) BETWEEN date('{fromDate.Value.ToString("yyyy-MM-dd")}') AND date('{toDate.Value.ToString("yyyy-MM-dd")}')";
                    periodText = $"с {fromDate.Value.ToShortDateString()} по {toDate.Value.ToShortDateString()}";
                    break;
            }

            using (var cmd = new SQLiteCommand(query, connection))
            {
                var result = cmd.ExecuteScalar();
                revenue = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }

            MessageBox.Show($"Выручка {periodText}: {revenue} ₽", "Финансовый отчет", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

}