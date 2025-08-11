using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class CartHelper
    {
        private MainMenuForm form;
        private SQLiteConnection connection;

        public CartHelper(MainMenuForm form)
        {
            this.form = form;
            this.connection = new SQLiteConnection("Data Source=online_shop.db;Version=3;");
            InitializeCartTable();
        }

        private void InitializeCartTable()
        {
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS CartItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    ProductId INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Price DECIMAL NOT NULL,
                    ProductName TEXT,
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    FOREIGN KEY (ProductId) REFERENCES Products(Id),
                    UNIQUE(UserId, ProductId)
                )";

            using (var command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void LoadCart()
        {
            form.currentOrder.Items.Clear();

            string query = "SELECT * FROM CartItems WHERE UserId = @UserId";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", CurrentUser.Id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new OrderItem
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Price = Convert.ToDecimal(reader["Price"]),
                            ProductName = reader["ProductName"].ToString()
                        };

                        form.currentOrder.Items.Add(item);
                    }
                }
            }

            foreach (var item in form.currentOrder.Items)
            {
                var product = form.products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                    item.ProductName = product.Name;
                    item.Price = product.Price;
                    UpdateCartItemInDatabase(item);
                }
            }
        }

        public void SaveCartItems(List<OrderItem> items)
        {
            string deleteQuery = "DELETE FROM CartItems WHERE UserId = @UserId";
            using (var command = new SQLiteCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@UserId", CurrentUser.Id);
                command.ExecuteNonQuery();
            }

            foreach (var item in items)
            {
                string insertQuery = @"
                    INSERT INTO CartItems (UserId, ProductId, Quantity, Price, ProductName)
                    VALUES (@UserId, @ProductId, @Quantity, @Price, @ProductName)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", CurrentUser.Id);
                    command.Parameters.AddWithValue("@ProductId", item.ProductId);
                    command.Parameters.AddWithValue("@Quantity", item.Quantity);
                    command.Parameters.AddWithValue("@Price", item.Price);
                    command.Parameters.AddWithValue("@ProductName", item.ProductName);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateCartItemInDatabase(OrderItem item)
        {
            string query = @"
                UPDATE CartItems 
                SET Price = @Price, ProductName = @ProductName
                WHERE UserId = @UserId AND ProductId = @ProductId";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", CurrentUser.Id);
                command.Parameters.AddWithValue("@ProductId", item.ProductId);
                command.Parameters.AddWithValue("@Price", item.Price);
                command.Parameters.AddWithValue("@ProductName", item.ProductName);
                command.ExecuteNonQuery();
            }
        }

        public void ShowCartPanel()
        {
            SaveCartItems(form.currentOrder.Items);
            form.UIHelper.ClearPanels();

            form.cartPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Корзина",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 10)
            };
            form.cartPanel.Controls.Add(title);

            if (form.currentOrder.Items.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "Ваша корзина пуста",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(20, 50)
                };
                form.cartPanel.Controls.Add(emptyLabel);
            }
            else
            {
                var orderItemsPanel = new Panel
                {
                    Location = new Point(20, 50),
                    Size = new Size(form.ClientSize.Width - 40, form.ClientSize.Height - 180),
                    AutoScroll = true,
                    BackColor = Color.White
                };

                int yPos = 0;
                decimal total = 0;

                foreach (var item in form.currentOrder.Items)
                {
                    var product = form.products.Find(p => p.Id == item.ProductId);
                    if (product == null) continue;

                    var itemPanel = new Panel
                    {
                        Location = new Point(0, yPos),
                        Size = new Size(orderItemsPanel.Width - 20, 70),
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    var productImage = new PictureBox
                    {
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Size = new Size(50, 50),
                        Location = new Point(10, 10),
                        Image = form.UIHelper.LoadImageOrDefault(product.ImagePath, 50, 50)
                    };
                    itemPanel.Controls.Add(productImage);

                    var nameLabel = new Label
                    {
                        Text = product.Name,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        ForeColor = Color.Black,
                        AutoSize = false,
                        Size = new Size(250, 20),
                        Location = new Point(70, 10)
                    };
                    itemPanel.Controls.Add(nameLabel);

                    var priceLabel = new Label
                    {
                        Text = $"{item.Price} ₽ x {item.Quantity} = {item.Price * item.Quantity} ₽",
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.FromArgb(70, 130, 180),
                        AutoSize = true,
                        Location = new Point(70, 35)
                    };
                    itemPanel.Controls.Add(priceLabel);

                    var minusButton = new Button
                    {
                        Text = "-",
                        BackColor = Color.FromArgb(220, 220, 220),
                        ForeColor = Color.Black,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(25, 25),
                        Location = new Point(330, 20),
                        Font = new Font("Segoe UI", 8),
                        Tag = item.ProductId
                    };
                    minusButton.FlatAppearance.BorderSize = 0;
                    minusButton.Click += (s, e) => UpdateCartItemQuantity(item.ProductId, -1);
                    itemPanel.Controls.Add(minusButton);

                    var quantityLabel = new Label
                    {
                        Text = item.Quantity.ToString(),
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        Location = new Point(360, 25)
                    };
                    itemPanel.Controls.Add(quantityLabel);

                    var plusButton = new Button
                    {
                        Text = "+",
                        BackColor = Color.FromArgb(220, 220, 220),
                        ForeColor = Color.Black,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(25, 25),
                        Location = new Point(390, 20),
                        Font = new Font("Segoe UI", 8),
                        Tag = item.ProductId
                    };
                    plusButton.FlatAppearance.BorderSize = 0;
                    plusButton.Click += (s, e) => UpdateCartItemQuantity(item.ProductId, 1);
                    itemPanel.Controls.Add(plusButton);

                    var removeButton = new Button
                    {
                        Text = "Удалить",
                        BackColor = Color.FromArgb(255, 100, 100),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(70, 25),
                        Location = new Point(430, 20),
                        Font = new Font("Segoe UI", 8),
                        Tag = item.ProductId
                    };
                    removeButton.FlatAppearance.BorderSize = 0;
                    removeButton.Click += (s, e) => RemoveFromCart(item.ProductId);
                    itemPanel.Controls.Add(removeButton);

                    orderItemsPanel.Controls.Add(itemPanel);
                    yPos += 80;
                    total += item.Price * item.Quantity;
                }

                form.cartPanel.Controls.Add(orderItemsPanel);

                var totalPanel = new Panel
                {
                    Location = new Point(0, form.ClientSize.Height - 120),
                    Size = new Size(form.ClientSize.Width, 60),
                    BackColor = Color.FromArgb(240, 240, 240)
                };

                var totalLabel = new Label
                {
                    Text = $"Итого: {total} ₽",
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = Color.FromArgb(70, 130, 180),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };
                totalPanel.Controls.Add(totalLabel);

                var checkoutButton = new Button
                {
                    Text = "Оформить заказ",
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(200, 40),
                    Location = new Point(form.ClientSize.Width - 230, 10),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                checkoutButton.FlatAppearance.BorderSize = 0;
                checkoutButton.Click += (s, e) => form.DeliveryHelper.ShowDeliveryOptions();
                totalPanel.Controls.Add(checkoutButton);

                form.cartPanel.Controls.Add(totalPanel);
            }

            form.Controls.Add(form.cartPanel);
        }

        public void UpdateCartItemQuantity(int productId, int change)
        {
            var item = form.currentOrder.Items.Find(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity += change;
                if (item.Quantity <= 0)
                {
                    form.currentOrder.Items.Remove(item);
                    RemoveFromCart(productId);
                }
                else
                {
                    SaveCartItems(form.currentOrder.Items);
                }

                ShowCartPanel();
            }
        }

        public void RemoveFromCart(int productId)
        {
            string query = "DELETE FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", CurrentUser.Id);
                command.Parameters.AddWithValue("@ProductId", productId);
                command.ExecuteNonQuery();
            }

            var item = form.currentOrder.Items.Find(i => i.ProductId == productId);
            if (item != null)
            {
                form.currentOrder.Items.Remove(item);
            }

            ShowCartPanel();
        }

        public void ClearCart()
        {
            string query = "DELETE FROM CartItems WHERE UserId = @UserId";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", CurrentUser.Id);
                command.ExecuteNonQuery();
            }

            form.currentOrder.Items.Clear();
        }
    }
}