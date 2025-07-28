using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class CookHelper
    {
        private MainMenuForm form;
        public CookHelper(MainMenuForm form) { this.form = form; }

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

            var title = new Label
            {
                Text = "Заказы на кухню",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.cookOrdersPanel.Controls.Add(title);

            var kitchenOrders = new List<KitchenOrder>
            {
                new KitchenOrder { Id = 1001, TableNumber = 5, Items = "Пицца Маргарита, Салат Цезарь", Status = "В ожидании", Time = DateTime.Now.AddMinutes(-15) },
                new KitchenOrder { Id = 1002, TableNumber = 3, Items = "Стейк средней прожарки", Status = "Готовится", Time = DateTime.Now.AddMinutes(-5) },
                new KitchenOrder { Id = 1003, TableNumber = 8, Items = "Паста Карбонара", Status = "Готово", Time = DateTime.Now }
            };

            int yPos = 60;
            foreach (var order in kitchenOrders)
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

                form.cookOrdersPanel.Controls.Add(orderPanel);
                yPos += 110;
            }

            form.Controls.Add(form.cookOrdersPanel);
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

            var menuItems = new List<MenuItem>
            {
                new MenuItem { Name = "Пицца Маргарита", Price = 599, Ingredients = "Тесто, томатный соус, моцарелла, базилик", CookingTime = 15 },
                new MenuItem { Name = "Стейк", Price = 1299, Ingredients = "Говядина, специи, соус", CookingTime = 20 },
                new MenuItem { Name = "Салат Цезарь", Price = 399, Ingredients = "Курица, салат, сухарики, соус", CookingTime = 10 }
            };

            int yPos = 60;
            foreach (var item in menuItems)
            {
                var itemPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 120),
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
                    Size = new Size(form.ClientSize.Width - 60, 50)
                };
                itemPanel.Controls.Add(ingredientsLabel);

                form.cookMenuPanel.Controls.Add(itemPanel);
                yPos += 130;
            }

            form.Controls.Add(form.cookMenuPanel);
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
                new Ingredient { Name = "Томатный соус", Quantity = 10, Unit = "л", MinQuantity = 3 }
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

            form.Controls.Add(form.ingredientsPanel);
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
            saveButton.Click += (s, e) => form.Close();

            form.Controls.AddRange(new Control[] { statusLabel, statusComboBox, saveButton });
            form.ShowDialog();
        }

        public void ChangeOrderStatus(int orderId)
        {
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
            statusComboBox.Items.AddRange(new[] { "В ожидании", "Готовится", "Готово", "Отменено" });

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
                MessageBox.Show($"Статус заказа #{orderId} изменен на: {statusComboBox.Text}", "Статус изменен");
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
                Size = new Size(300, 200),
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
                Maximum = 100
            };

            var commentLabel = new Label
            {
                Text = "Комментарий:",
                Location = new Point(20, 80),
                AutoSize = true
            };

            var commentBox = new TextBox
            {
                Location = new Point(20, 110),
                Size = new Size(250, 20)
            };

            var saveButton = new Button
            {
                Text = "Отправить запрос",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(70, 140),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) =>
            {
                MessageBox.Show($"Запрос на {quantityBox.Value} единиц {ingredientName} отправлен", "Запрос отправлен");
                form.Close();
            };

            form.Controls.AddRange(new Control[] { quantityLabel, quantityBox, commentLabel, commentBox, saveButton });
            form.ShowDialog();
        }
    }
}