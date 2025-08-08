using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class CookHelper
    {
        private MainMenuForm form;
        private DatabaseHelper dbHelper;

        public CookHelper(MainMenuForm form)
        {
            this.form = form;
            this.dbHelper = new DatabaseHelper();
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

            var kitchenOrders = GetKitchenOrdersFromDatabase();

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
                    Text = $"Заказ #{order.OrderId}",
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
                    Text = $"Время заказа: {order.OrderTime:HH:mm}",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(10, 55),
                    AutoSize = true
                };
                orderPanel.Controls.Add(timeLabel);

                var statusLabel = new Label
                {
                    Text = $"Статус: {order.OrderStatus}",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = form.UIHelper.GetKitchenOrderStatusColor(order.OrderStatus),
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
                    Tag = order.OrderId
                };
                changeStatusButton.Click += (s, e) => ChangeOrderStatus(order.OrderId);
                orderPanel.Controls.Add(changeStatusButton);

                form.cookOrdersPanel.Controls.Add(orderPanel);
                yPos += 110;
            }

            form.Controls.Add(form.cookOrdersPanel);
        }

        private List<KitchenOrderData> GetKitchenOrdersFromDatabase()
        {
            var orders = new List<KitchenOrderData>();

            using (var connection = new SQLiteConnection(dbHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
                    SELECT o.Id, o.Date as OrderTime, o.Status as OrderStatus, 
                           GROUP_CONCAT(oi.ProductName, ', ') as Items
                    FROM Orders o
                    JOIN OrderItems oi ON o.Id = oi.OrderId
                    WHERE o.Status IN ('В ожидании', 'Готовится')
                    GROUP BY o.Id, o.Date, o.Status
                    ORDER BY o.Date";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new KitchenOrderData
                            {
                                OrderId = reader.GetInt32(0),
                                OrderTime = DateTime.Parse(reader.GetString(1)),
                                OrderStatus = reader.GetString(2),
                                Items = reader.GetString(3)
                            });
                        }
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

            var menuItems = dbHelper.LoadProducts();

            int yPos = 60;
            foreach (var item in menuItems)
            {
                if (item.Calories > 0)
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
                        Text = $"{item.Price} ₽ | Время приготовления: {CalculateCookingTime(item)} мин",
                        Font = new Font("Segoe UI", 10),
                        Location = new Point(10, 35),
                        AutoSize = true
                    };
                    itemPanel.Controls.Add(priceLabel);

                    var ingredientsLabel = new Label
                    {
                        Text = $"Ингредиенты: {item.Description}",
                        Font = new Font("Segoe UI", 9),
                        Location = new Point(10, 60),
                        AutoSize = false,
                        Size = new Size(form.ClientSize.Width - 60, 50)
                    };
                    itemPanel.Controls.Add(ingredientsLabel);

                    form.cookMenuPanel.Controls.Add(itemPanel);
                    yPos += 130;
                }
            }

            form.Controls.Add(form.cookMenuPanel);
        }

        private int CalculateCookingTime(Product item)
        {
            if (item.Name.Contains("Пицца")) return 15;
            if (item.Name.Contains("Салат")) return 10;
            if (item.Name.Contains("Стейк")) return 20;
            return 15;
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

            var ingredients = GetIngredientsData();

            int yPos = 60;
            foreach (var ingredient in ingredients)
            {
                var ingredientPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 60),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = ingredient.IngredientQuantity < ingredient.IngredientMinQuantity
                        ? Color.FromArgb(255, 200, 200)
                        : Color.White
                };

                var nameLabel = new Label
                {
                    Text = ingredient.IngredientName,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                ingredientPanel.Controls.Add(nameLabel);

                var quantityLabel = new Label
                {
                    Text = $"{ingredient.IngredientQuantity} {ingredient.IngredientUnit} (мин: {ingredient.IngredientMinQuantity})",
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
                    Tag = ingredient.IngredientName
                };
                requestButton.Click += (s, e) => RequestIngredient(ingredient.IngredientName);
                ingredientPanel.Controls.Add(requestButton);

                form.ingredientsPanel.Controls.Add(ingredientPanel);
                yPos += 70;
            }

            form.Controls.Add(form.ingredientsPanel);
        }

        private List<IngredientData> GetIngredientsData()
        {
            return new List<IngredientData>
            {
                new IngredientData {
                    IngredientName = "Моцарелла",
                    IngredientQuantity = 5,
                    IngredientUnit = "кг",
                    IngredientMinQuantity = 2
                },
                new IngredientData {
                    IngredientName = "Говядина",
                    IngredientQuantity = 8,
                    IngredientUnit = "кг",
                    IngredientMinQuantity = 5
                },
                new IngredientData {
                    IngredientName = "Салат",
                    IngredientQuantity = 3,
                    IngredientUnit = "кг",
                    IngredientMinQuantity = 2
                },
                new IngredientData {
                    IngredientName = "Томатный соус",
                    IngredientQuantity = 10,
                    IngredientUnit = "л",
                    IngredientMinQuantity = 3
                }
            };
        }

        // Остальные методы остаются без изменений
        public void ChangeCookStatus() { /* ... */ }
        public void ChangeOrderStatus(int orderId) { /* ... */ }
        public void RequestIngredient(string ingredientName) { /* ... */ }
    }

    public class KitchenOrderData
    {
        public int OrderId { get; set; }
        public string Items { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderTime { get; set; }
    }

    public class IngredientData
    {
        public string IngredientName { get; set; }
        public int IngredientQuantity { get; set; }
        public string IngredientUnit { get; set; }
        public int IngredientMinQuantity { get; set; }
    }
}