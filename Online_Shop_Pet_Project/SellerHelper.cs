using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class SellerHelper
    {
        private MainMenuForm form;
        public List<Product> products = new List<Product>();
        public SellerHelper(MainMenuForm form) { this.form = form; }

        public void ShowPurchaseHistory()
        {
            form.UIHelper.ClearPanels();

            form.historyPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "История покупок",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.historyPanel.Controls.Add(title);

            var filterPanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(form.ClientSize.Width - 40, 80),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            var phoneLabel = new Label
            {
                Text = "Телефон клиента:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 15),
                AutoSize = true
            };
            filterPanel.Controls.Add(phoneLabel);

            var phoneTextBox = new TextBox
            {
                Location = new Point(150, 15),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10)
            };
            filterPanel.Controls.Add(phoneTextBox);

            var dateLabel = new Label
            {
                Text = "Дата заказа:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 45),
                AutoSize = true
            };
            filterPanel.Controls.Add(dateLabel);

            var datePicker = new DateTimePicker
            {
                Location = new Point(150, 45),
                Size = new Size(150, 20),
                Format = DateTimePickerFormat.Short
            };
            filterPanel.Controls.Add(datePicker);

            var searchButton = new Button
            {
                Text = "Поиск",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(400, 25),
                Font = new Font("Segoe UI", 10)
            };
            searchButton.Click += (s, e) => SearchPurchases(phoneTextBox.Text, datePicker.Value);
            filterPanel.Controls.Add(searchButton);

            form.historyPanel.Controls.Add(filterPanel);

            var resultsPanel = new Panel
            {
                Location = new Point(20, 160),
                Size = new Size(form.ClientSize.Width - 40, form.ClientSize.Height - 240),
                AutoScroll = true,
                BackColor = Color.White
            };

            var orders = new List<Order>
            {
                new Order { Id = 1001, Date = DateTime.Now.AddDays(-2), CustomerPhone = "+7 (123) 456-78-90", Total = 12500, Status = "Завершен" },
                new Order { Id = 1002, Date = DateTime.Now.AddDays(-5), CustomerPhone = "+7 (987) 654-32-10", Total = 8700, Status = "Завершен" }
            };

            int yPos = 10;
            foreach (var order in orders)
            {
                var orderPanel = new Panel
                {
                    Location = new Point(0, yPos),
                    Size = new Size(resultsPanel.Width - 20, 80),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var orderLabel = new Label
                {
                    Text = $"Заказ #{order.Id} от {order.Date:dd.MM.yyyy}",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                orderPanel.Controls.Add(orderLabel);

                var phoneLabel2 = new Label
                {
                    Text = $"Телефон: {order.CustomerPhone}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 35),
                    AutoSize = true
                };
                orderPanel.Controls.Add(phoneLabel2);

                var totalLabel = new Label
                {
                    Text = $"Сумма: {order.Total} ₽",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(resultsPanel.Width - 150, 35),
                    AutoSize = true
                };
                orderPanel.Controls.Add(totalLabel);

                var detailsButton = new Button
                {
                    Text = "Подробнее",
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(100, 25),
                    Location = new Point(resultsPanel.Width - 120, 10),
                    Font = new Font("Segoe UI", 9)
                };
                detailsButton.Click += (s, e) => form.OrderHelper.ShowOrderDetails(order.Id);
                orderPanel.Controls.Add(detailsButton);

                resultsPanel.Controls.Add(orderPanel);
                yPos += 90;
            }

            form.historyPanel.Controls.Add(resultsPanel);
            form.Controls.Add(form.historyPanel);
        }

        public void ProcessOfflineOrder()
        {
            var form = new Form
            {
                Text = "Оформление оффлайн заказа",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var customerGroup = new GroupBox
            {
                Text = "Информация о клиенте",
                Location = new Point(20, 20),
                Size = new Size(450, 100),
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
                Size = new Size(450, 200),
                Font = new Font("Segoe UI", 10)
            };

            var productsList = new ListBox
            {
                Location = new Point(10, 25),
                Size = new Size(200, 150),
                SelectionMode = SelectionMode.MultiExtended
            };
            foreach (var product in products)
            {
                productsList.Items.Add($"{product.Name} - {product.Price} ₽");
            }

            var selectedProductsList = new ListBox
            {
                Location = new Point(220, 25),
                Size = new Size(200, 150)
            };

            var addButton = new Button
            {
                Text = "Добавить →",
                Location = new Point(220, 180),
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
                Location = new Point(320, 180),
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

            productsGroup.Controls.AddRange(new Control[] { productsList, selectedProductsList, addButton, removeButton });

            var cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(150, 360),
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
                Location = new Point(300, 360),
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

                MessageBox.Show("Оффлайн заказ успешно оформлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            form.Controls.AddRange(new Control[] { customerGroup, productsGroup, cancelButton, confirmButton });
            form.ShowDialog();
        }

        public void ProcessProductReturn()
        {
            var form = new Form
            {
                Text = "Возврат товара",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var orderGroup = new GroupBox
            {
                Text = "Информация о заказе",
                Location = new Point(20, 20),
                Size = new Size(450, 100),
                Font = new Font("Segoe UI", 10)
            };

            var orderLabel = new Label { Text = "Номер заказа:", Location = new Point(10, 25), AutoSize = true };
            var orderBox = new TextBox { Location = new Point(120, 25), Size = new Size(100, 20) };

            var phoneLabel = new Label { Text = "Телефон клиента:", Location = new Point(10, 55), AutoSize = true };
            var phoneBox = new TextBox { Location = new Point(120, 55), Size = new Size(200, 20) };

            var findButton = new Button
            {
                Text = "Найти",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 25),
                Location = new Point(330, 25)
            };
            findButton.Click += (s, e) =>
            {
                MessageBox.Show("Заказ найден", "Результат поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            orderGroup.Controls.AddRange(new Control[] { orderLabel, orderBox, phoneLabel, phoneBox, findButton });

            var productsGroup = new GroupBox
            {
                Text = "Товары для возврата",
                Location = new Point(20, 140),
                Size = new Size(450, 150),
                Font = new Font("Segoe UI", 10)
            };

            var productsList = new CheckedListBox
            {
                Location = new Point(10, 25),
                Size = new Size(400, 100)
            };
            productsList.Items.AddRange(new[] { "Смартфон Samsung Galaxy S23 - 79990 ₽", "Наушники Sony WH-1000XM5 - 34990 ₽" });

            productsGroup.Controls.Add(productsList);

            var cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(150, 310),
                Size = new Size(100, 30),
                DialogResult = DialogResult.Cancel
            };

            var confirmButton = new Button
            {
                Text = "Подтвердить возврат",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(300, 310),
                DialogResult = DialogResult.OK
            };
            confirmButton.Click += (s, e) =>
            {
                if (productsList.CheckedItems.Count == 0)
                {
                    MessageBox.Show("Выберите товары для возврата", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Возврат товара успешно оформлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            form.Controls.AddRange(new Control[] { orderGroup, productsGroup, cancelButton, confirmButton });
            form.ShowDialog();
        }

        public void SearchPurchases(string phone, DateTime date)
        {
            MessageBox.Show($"Поиск покупок для телефона: {phone}, дата: {date.ToShortDateString()}", "Результаты поиска");
        }
    }
}