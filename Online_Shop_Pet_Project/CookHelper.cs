using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Online_Shop_Pet_Project
{
    public class CookHelper
    {
        private MainMenuForm form;
        private List<KitchenOrder> orderHistory = new List<KitchenOrder>();
        private List<IngredientRequest> ingredientRequests = new List<IngredientRequest>();

        public CookHelper(MainMenuForm form)
        {
            this.form = form;
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            orderHistory.Add(new KitchenOrder
            {
                Id = 1001,
                TableNumber = 5,
                Items = "Пицца Маргарита, Салат Цезарь",
                Status = "Поступил",
                Time = DateTime.Now.AddMinutes(-15),
                StatusHistory = new List<StatusChange>
                {
                    new StatusChange { Status = "Поступил", Time = DateTime.Now.AddMinutes(-15) }
                }
            });

            orderHistory.Add(new KitchenOrder
            {
                Id = 1002,
                TableNumber = 3,
                Items = "Стейк средней прожарки",
                Status = "Поступил",
                Time = DateTime.Now.AddMinutes(-5),
                StatusHistory = new List<StatusChange>
                {
                    new StatusChange { Status = "Поступил", Time = DateTime.Now.AddMinutes(-5) }
                }
            });
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

            var title = new Label
            {
                Text = "Заказы на кухню",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.cookOrdersPanel.Controls.Add(title);

            var activeOrders = orderHistory.Where(o => o.Status != "Готово" && o.Status != "Отменено").ToList();

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

            var completedOrders = orderHistory.Where(o => o.Status == "Готово" || o.Status == "Отменено")
                                             .OrderByDescending(o => o.Time)
                                             .ToList();

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
                new MenuItem
                {
                    Name = "Пицца Маргарита",
                    Price = 599,
                    Ingredients = "Тесто, томатный соус, моцарелла, базилик",
                    CookingTime = 15,
                    RequiredIngredients = new List<IngredientRequirement>
                    {
                        new IngredientRequirement { Name = "Тесто", Quantity = 0.3, Unit = "кг" },
                        new IngredientRequirement { Name = "Томатный соус", Quantity = 0.1, Unit = "л" },
                        new IngredientRequirement { Name = "Моцарелла", Quantity = 0.2, Unit = "кг" },
                        new IngredientRequirement { Name = "Базилик", Quantity = 0.01, Unit = "кг" }
                    }
                },
                new MenuItem
                {
                    Name = "Стейк",
                    Price = 1299,
                    Ingredients = "Говядина, специи, соус",
                    CookingTime = 20,
                    RequiredIngredients = new List<IngredientRequirement>
                    {
                        new IngredientRequirement { Name = "Говядина", Quantity = 0.3, Unit = "кг" },
                        new IngredientRequirement { Name = "Специи", Quantity = 0.01, Unit = "кг" },
                        new IngredientRequirement { Name = "Стейк соус", Quantity = 0.05, Unit = "л" }
                    }
                },
                new MenuItem
                {
                    Name = "Салат Цезарь",
                    Price = 399,
                    Ingredients = "Курица, салат, сухарики, соус",
                    CookingTime = 10,
                    RequiredIngredients = new List<IngredientRequirement>
                    {
                        new IngredientRequirement { Name = "Курица", Quantity = 0.15, Unit = "кг" },
                        new IngredientRequirement { Name = "Салат", Quantity = 0.1, Unit = "кг" },
                        new IngredientRequirement { Name = "Сухарики", Quantity = 0.03, Unit = "кг" },
                        new IngredientRequirement { Name = "Соус Цезарь", Quantity = 0.05, Unit = "л" }
                    }
                }
            };

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

                string requirements = string.Join(", ", item.RequiredIngredients.Select(i => $"{i.Name} - {i.Quantity}{i.Unit}"));
                var requirementsList = new Label
                {
                    Text = requirements,
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

            int yPos = 60;
            foreach (var request in ingredientRequests.OrderByDescending(r => r.RequestTime))
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
        public void ChangeOrderStatus(int orderId)
        {
            var order = orderHistory.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return;

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

            if (order.Status == "Поступил")
            {
                statusComboBox.Items.AddRange(new[] { "Принят", "Готовится", "Отменено" });
            }
            else if (order.Status == "Принят")
            {
                statusComboBox.Items.AddRange(new[] { "Готовится", "Готово", "Отменено" });
            }
            else if (order.Status == "Готовится")
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
                order.Status = newStatus;
                order.StatusHistory.Add(new StatusChange { Status = newStatus, Time = DateTime.Now });

                MessageBox.Show($"Статус заказа #{orderId} изменен на: {newStatus}", "Статус изменен");
                form.Close();
                ShowCookOrders();
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
                var request = new IngredientRequest
                {
                    IngredientName = ingredientName,
                    Quantity = (double)quantityBox.Value,
                    Unit = unitComboBox.Text,
                    Comment = commentBox.Text,
                    RequestTime = DateTime.Now,
                    Status = "В обработке"
                };

                ingredientRequests.Add(request);

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