using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;

namespace Online_Shop_Pet_Project
{
    public class DatabaseHelper
    {
        private string databasePath = "shop_database.db";
        private string connectionString;

        public DatabaseHelper()
        {
            connectionString = $"Data Source={databasePath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(databasePath))
            {
                SQLiteConnection.CreateFile(databasePath);
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createProductsTable = @"
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Price DECIMAL NOT NULL,
                    ImagePath TEXT,
                    Description TEXT,
                    Calories INTEGER,
                    Protein INTEGER,
                    Fat INTEGER,
                    Carbohydrates INTEGER,
                    Weight DECIMAL,
                    Dimensions TEXT
                )";

                string createOrdersTable = @"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id INTEGER PRIMARY KEY,
                    Date TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    Total DECIMAL NOT NULL,
                    DeliveryMethod TEXT,
                    PaymentMethod TEXT
                )";

                string createOrderItemsTable = @"
                CREATE TABLE IF NOT EXISTS OrderItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderId INTEGER,
                    ProductId INTEGER,
                    Quantity INTEGER NOT NULL,
                    Price DECIMAL NOT NULL,
                    ProductName TEXT,
                    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
                    FOREIGN KEY (ProductId) REFERENCES Products(Id)
                )";

                string createCartItemsTable = @"
                CREATE TABLE IF NOT EXISTS CartItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductId INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Price DECIMAL NOT NULL,
                    ProductName TEXT,
                    FOREIGN KEY (ProductId) REFERENCES Products(Id)
                )";
                string createChatMessagesTable = @"
                CREATE TABLE IF NOT EXISTS ChatMessages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ChatId INTEGER NOT NULL,
                    Sender TEXT NOT NULL,
                    Message TEXT NOT NULL,
                    Timestamp TEXT NOT NULL,
                    IsSupport INTEGER DEFAULT 0,
                    IsRead INTEGER DEFAULT 0,
                    CustomerName TEXT
                )";
                                string createComplaintsTable = @"
                CREATE TABLE IF NOT EXISTS Complaints (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerName TEXT NOT NULL,
                    CustomerPhone TEXT,
                    Subject TEXT NOT NULL,
                    Message TEXT NOT NULL,
                    OrderId INTEGER,
                    Status TEXT DEFAULT 'Новая',
                    Response TEXT,
                    CreatedDate TEXT NOT NULL,
                    ResolvedDate TEXT
                )";

                                string createSupportTicketsTable = @"
                CREATE TABLE IF NOT EXISTS SupportTickets (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Subject TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Priority TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Answer TEXT,
                    CreatedDate TEXT NOT NULL,
                    CustomerName TEXT NOT NULL
                )";

                                string createKnowledgeBaseTable = @"
                CREATE TABLE IF NOT EXISTS KnowledgeBase (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    Category TEXT NOT NULL
)";

                using (var command = new SQLiteCommand(createComplaintsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createSupportTicketsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createKnowledgeBaseTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SQLiteCommand(createChatMessagesTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SQLiteCommand(createProductsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createOrdersTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createOrderItemsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createCartItemsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                string countProducts = "SELECT COUNT(*) FROM Products";
                using (var command = new SQLiteCommand(countProducts, connection))
                {
                    long count = (long)command.ExecuteScalar();
                    if (count == 0)
                    {
                        InsertSampleProducts(connection);
                    }
                }
            }
        }

        private void InsertSampleProducts(SQLiteConnection connection)
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Смартфон Samsung Galaxy S23",
                    Price = 79990,
                    ImagePath = "E:\\с#\\Online_Shop_Pet_Project\\Online_Shop_Pet_Project\\art\\phone.png",
                    Description = "Флагманский смартфон с AMOLED-экраном 6.1\" и тройной камерой",
                    Calories = 0,
                    Protein = 0,
                    Fat = 0,
                    Carbohydrates = 0,
                    Weight = 168,
                    Dimensions = "70.9 x 146.3 x 7.6 мм"
                },
                new Product
                {
                    Id = 2,
                    Name = "Наушники Sony WH-1000XM5",
                    Price = 34990,
                    ImagePath = "E:\\с#\\Online_Shop_Pet_Project\\Online_Shop_Pet_Project\\art\\headphones1.jpg",
                    Description = "Беспроводные наушники с шумоподавлением",
                    Calories = 0,
                    Protein = 0,
                    Fat = 0,
                    Carbohydrates = 0,
                    Weight = 250,
                    Dimensions = "20.4 x 24.9 x 18.7 см"
                },
                new Product
                {
                    Id = 3,
                    Name = "Пицца Маргарита",
                    Price = 599,
                    ImagePath = "E:\\с#\\Online_Shop_Pet_Project\\Online_Shop_Pet_Project\\art\\pizza.png",
                    Description = "Классическая пицца с томатным соусом, моцареллой и базиликом",
                    Calories = 850,
                    Protein = 35,
                    Fat = 30,
                    Carbohydrates = 100,
                    Weight = 450,
                    Dimensions = "30 см"
                },
                new Product
                {
                    Id = 4,
                    Name = "Фитнес-браслет Xiaomi Mi Band 7",
                    Price = 3990,
                    ImagePath = "E:\\с#\\Online_Shop_Pet_Project\\Online_Shop_Pet_Project\\art\\Xiaomi_Mi_Band_7.jpg",
                    Description = "Умный браслет с мониторингом активности и сна",
                    Calories = 0,
                    Protein = 0,
                    Fat = 0,
                    Carbohydrates = 0,
                    Weight = 13.5,
                    Dimensions = "46.5 x 20.7 x 12.25 мм"
                },
                new Product
                {
                    Id = 5,
                    Name = "Кофе зерновой Lavazza",
                    Price = 899,
                    ImagePath = "E:\\с#\\Online_Shop_Pet_Project\\Online_Shop_Pet_Project\\art\\coffe.jpg",
                    Description = "Итальянский кофе в зернах, 1 кг",
                    Calories = 0,
                    Protein = 0,
                    Fat = 0,
                    Carbohydrates = 0,
                    Weight = 1000,
                    Dimensions = "Упаковка"
                },
                new Product
                {
                    Id = 6,
                    Name = "Книга 'Clean Code'",
                    Price = 2490,
                    ImagePath = "E:\\с#\\Online_Shop_Pet_Project\\Online_Shop_Pet_Project\\art\\book.jpg",
                    Description = "Роберт Мартин. Чистый код: создание, анализ и рефакторинг",
                    Calories = 0,
                    Protein = 0,
                    Fat = 0,
                    Carbohydrates = 0,
                    Weight = 680,
                    Dimensions = "23.5 x 17.7 x 2.5 см"
                }
            };

            foreach (var product in products)
            {
                string insertProduct = @"
                INSERT INTO Products (Id, Name, Price, ImagePath, Description, Calories, Protein, Fat, Carbohydrates, Weight, Dimensions)
                VALUES (@Id, @Name, @Price, @ImagePath, @Description, @Calories, @Protein, @Fat, @Carbohydrates, @Weight, @Dimensions)";

                using (var command = new SQLiteCommand(insertProduct, connection))
                {
                    command.Parameters.AddWithValue("@Id", product.Id);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@ImagePath", product.ImagePath);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@Calories", product.Calories);
                    command.Parameters.AddWithValue("@Protein", product.Protein);
                    command.Parameters.AddWithValue("@Fat", product.Fat);
                    command.Parameters.AddWithValue("@Carbohydrates", product.Carbohydrates);
                    command.Parameters.AddWithValue("@Weight", product.Weight);
                    command.Parameters.AddWithValue("@Dimensions", product.Dimensions);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Product> LoadProducts()
        {
            var products = new List<Product>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Products";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            ImagePath = reader["ImagePath"].ToString(),
                            Description = reader["Description"].ToString(),
                            Calories = Convert.ToInt32(reader["Calories"]),
                            Protein = Convert.ToInt32(reader["Protein"]),
                            Fat = Convert.ToInt32(reader["Fat"]),
                            Carbohydrates = Convert.ToInt32(reader["Carbohydrates"]),
                            Weight = Convert.ToDouble(reader["Weight"]),
                            Dimensions = reader["Dimensions"].ToString()
                        });
                    }
                }
            }

            return products;
        }
        public string GetConnectionString()
        {
            return connectionString;
        }
        public List<Order> LoadOrders()
        {
            var orders = new List<Order>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Orders ORDER BY Date DESC";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new Order
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Date = DateTime.Parse(reader["Date"].ToString()),
                            Status = reader["Status"].ToString(),
                            Total = Convert.ToDecimal(reader["Total"]),
                            DeliveryMethod = reader["DeliveryMethod"].ToString(),
                            PaymentMethod = reader["PaymentMethod"].ToString(),
                            Items = new List<OrderItem>()
                        };

                        string itemsQuery = "SELECT * FROM OrderItems WHERE OrderId = @OrderId";
                        using (var itemsCommand = new SQLiteCommand(itemsQuery, connection))
                        {
                            itemsCommand.Parameters.AddWithValue("@OrderId", order.Id);
                            using (var itemsReader = itemsCommand.ExecuteReader())
                            {
                                while (itemsReader.Read())
                                {
                                    order.Items.Add(new OrderItem
                                    {
                                        ProductId = Convert.ToInt32(itemsReader["ProductId"]),
                                        Quantity = Convert.ToInt32(itemsReader["Quantity"]),
                                        Price = Convert.ToDecimal(itemsReader["Price"]),
                                        ProductName = itemsReader["ProductName"].ToString()
                                    });
                                }
                            }
                        }

                        orders.Add(order);
                    }
                }
            }

            return orders;
        }

        public List<OrderItem> LoadCartItems()
        {
            var cartItems = new List<OrderItem>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM CartItems";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cartItems.Add(new OrderItem
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Price = Convert.ToDecimal(reader["Price"]),
                            ProductName = reader["ProductName"].ToString()
                        });
                    }
                }
            }

            return cartItems;
        }

        public void SaveOrder(Order order)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertOrder = @"
                INSERT INTO Orders (Id, Date, Status, Total, DeliveryMethod, PaymentMethod)
                VALUES (@Id, @Date, @Status, @Total, @DeliveryMethod, @PaymentMethod)";

                using (var command = new SQLiteCommand(insertOrder, connection))
                {
                    command.Parameters.AddWithValue("@Id", order.Id);
                    command.Parameters.AddWithValue("@Date", order.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@Status", order.Status);
                    command.Parameters.AddWithValue("@Total", order.Total);
                    command.Parameters.AddWithValue("@DeliveryMethod", order.DeliveryMethod);
                    command.Parameters.AddWithValue("@PaymentMethod", order.PaymentMethod);
                    command.ExecuteNonQuery();
                }

                foreach (var item in order.Items)
                {
                    string insertItem = @"
                    INSERT INTO OrderItems (OrderId, ProductId, Quantity, Price, ProductName)
                    VALUES (@OrderId, @ProductId, @Quantity, @Price, @ProductName)";

                    using (var command = new SQLiteCommand(insertItem, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", order.Id);
                        command.Parameters.AddWithValue("@ProductId", item.ProductId);
                        command.Parameters.AddWithValue("@Quantity", item.Quantity);
                        command.Parameters.AddWithValue("@Price", item.Price);
                        command.Parameters.AddWithValue("@ProductName", item.ProductName);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void SaveCartItems(List<OrderItem> cartItems)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string clearCart = "DELETE FROM CartItems";
                using (var command = new SQLiteCommand(clearCart, connection))
                {
                    command.ExecuteNonQuery();
                }

                foreach (var item in cartItems)
                {
                    string insertItem = @"
                    INSERT INTO CartItems (ProductId, Quantity, Price, ProductName)
                    VALUES (@ProductId, @Quantity, @Price, @ProductName)";

                    using (var command = new SQLiteCommand(insertItem, connection))
                    {
                        command.Parameters.AddWithValue("@ProductId", item.ProductId);
                        command.Parameters.AddWithValue("@Quantity", item.Quantity);
                        command.Parameters.AddWithValue("@Price", item.Price);
                        command.Parameters.AddWithValue("@ProductName", item.ProductName);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void ClearCart()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string clearCart = "DELETE FROM CartItems";
                using (var command = new SQLiteCommand(clearCart, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

