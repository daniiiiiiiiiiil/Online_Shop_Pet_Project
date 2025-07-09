using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public partial class MainMenuForm : Form
    {
        private bool isEmployee;
        private string userRole;
        private Panel productsPanel;
        private Panel productDetailsPanel;
        private Panel ordersPanel;
        private Panel cartPanel;
        private Panel deliveryPanel;
        private Panel profilePanel;
        private List<Product> products = new List<Product>();
        private List<Order> orders = new List<Order>();
        private Order currentOrder = new Order();
        private string deliveryMethod = "Самовывоз";
        private string paymentMethod = "Сразу";

        // Данные пользователя
        private UserProfile userProfile = new UserProfile
        {
            Name = "Иван Иванов",
            Phone = "+7 (123) 456-78-90",
            Email = "ivan.ivanov@example.com",
            Password = "********",
            PhotoPath = "art/profile_photo.jpg"
        };

        public MainMenuForm(bool isEmployee)
        {
            InitializeComponent();
            this.isEmployee = isEmployee;
            this.WindowState = FormWindowState.Maximized; // Полноэкранный режим
            InitializeProducts();
            InitializeOrders();
            InitializeUI();
        }

        private void InitializeProducts()
        {
            // Заполняем список товаров (в реальном приложении это бы бралось из базы данных)
            products.Add(new Product
            {
                Id = 1,
                Name = "Смартфон Samsung Galaxy S23",
                Price = 79990,
                ImagePath = "art/products/phone1.jpg",
                Description = "Флагманский смартфон с AMOLED-экраном 6.1\" и тройной камерой",
                Calories = 0, // Для электроники калории не применимы
                Protein = 0,
                Fat = 0,
                Carbohydrates = 0,
                Weight = 168,
                Dimensions = "70.9 x 146.3 x 7.6 мм"
            });

            products.Add(new Product
            {
                Id = 2,
                Name = "Наушники Sony WH-1000XM5",
                Price = 34990,
                ImagePath = "art/products/headphones1.jpg",
                Description = "Беспроводные наушники с шумоподавлением",
                Calories = 0,
                Protein = 0,
                Fat = 0,
                Carbohydrates = 0,
                Weight = 250,
                Dimensions = "20.4 x 24.9 x 18.7 см"
            });

            products.Add(new Product
            {
                Id = 3,
                Name = "Пицца Маргарита",
                Price = 599,
                ImagePath = "art/products/pizza1.jpg",
                Description = "Классическая пицца с томатным соусом, моцареллой и базиликом",
                Calories = 850,
                Protein = 35,
                Fat = 30,
                Carbohydrates = 100,
                Weight = 450,
                Dimensions = "30 см"
            });

            products.Add(new Product
            {
                Id = 4,
                Name = "Фитнес-браслет Xiaomi Mi Band 7",
                Price = 3990,
                ImagePath = "art/products/band1.jpg",
                Description = "Умный браслет с мониторингом активности и сна",
                Calories = 0,
                Protein = 0,
                Fat = 0,
                Carbohydrates = 0,
                Weight = 13.5,
                Dimensions = "46.5 x 20.7 x 12.25 мм"
            });

            products.Add(new Product
            {
                Id = 5,
                Name = "Кофе зерновой Lavazza",
                Price = 899,
                ImagePath = "art/products/coffee1.jpg",
                Description = "Итальянский кофе в зернах, 1 кг",
                Calories = 0,
                Protein = 0,
                Fat = 0,
                Carbohydrates = 0,
                Weight = 1000,
                Dimensions = "Упаковка"
            });

            products.Add(new Product
            {
                Id = 6,
                Name = "Книга 'Clean Code'",
                Price = 2490,
                ImagePath = "art/products/book1.jpg",
                Description = "Роберт Мартин. Чистый код: создание, анализ и рефакторинг",
                Calories = 0,
                Protein = 0,
                Fat = 0,
                Carbohydrates = 0,
                Weight = 680,
                Dimensions = "23.5 x 17.7 x 2.5 см"
            });
        }

        private void InitializeOrders()
        {
            // Заполняем историю заказов (в реальном приложении это бы бралось из базы данных)
            var order1 = new Order
            {
                Id = 1001,
                Date = DateTime.Now.AddDays(-5),
                Status = "Доставлен",
                Total = 83980,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 1, Price = 79990 },
                    new OrderItem { ProductId = 5, Quantity = 1, Price = 899 }
                }
            };

            var order2 = new Order
            {
                Id = 1002,
                Date = DateTime.Now.AddDays(-2),
                Status = "В пути",
                Total = 3990,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 4, Quantity = 1, Price = 3990 }
                }
            };

            orders.Add(order1);
            orders.Add(order2);
        }

        private void InitializeUI()
        {
            this.Text = "Online Shop";
            this.Size = new Size(1000, 700);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            if (isEmployee)
            {
                ShowEmployeeRoleSelection();
            }
            else
            {
                ShowCustomerMenu();
                ShowProductsPanel();
            }
        }


        private void ShowProductsPanel()
        {
            ClearPanels();

            productsPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 80), // Учитываем нижнюю панель
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Товары",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 10)
            };
            productsPanel.Controls.Add(title);

            int yPos = 50;
            int xPos = 20;
            int itemsPerRow = Math.Max(1, this.ClientSize.Width / 220);

            for (int i = 0; i < products.Count; i++)
            {
                var product = products[i];
                var productItem = CreateProductItem(product, xPos, yPos);
                productsPanel.Controls.Add(productItem);

                xPos += 220;

                if ((i + 1) % itemsPerRow == 0)
                {
                    xPos = 20;
                    yPos += 220;
                }
            }

            this.Controls.Add(productsPanel);
        }

        private void ClearPanels()
        {
            if (productsPanel != null) this.Controls.Remove(productsPanel);
            if (productDetailsPanel != null) this.Controls.Remove(productDetailsPanel);
            if (ordersPanel != null) this.Controls.Remove(ordersPanel);
            if (cartPanel != null) this.Controls.Remove(cartPanel);
            if (deliveryPanel != null) this.Controls.Remove(deliveryPanel);
            if (profilePanel != null) this.Controls.Remove(profilePanel);
        }

        private Panel CreateProductItem(Product product, int x, int y)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(200, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Tag = product.Id
            };

            // Картинка товара
            var picture = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(150, 100),
                Location = new Point(25, 10),
                Image = LoadImageOrDefault(product.ImagePath, 150, 100),
                Tag = product.Id
            };
            picture.Click += (s, e) => ShowProductDetails(product.Id);
            panel.Controls.Add(picture);

            // Название товара
            var nameLabel = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = false,
                Size = new Size(180, 40),
                Location = new Point(10, 120),
                TextAlign = ContentAlignment.TopCenter,
                Tag = product.Id
            };
            nameLabel.Click += (s, e) => ShowProductDetails(product.Id);
            panel.Controls.Add(nameLabel);

            // Цена товара
            var priceLabel = new Label
            {
                Text = $"{product.Price} ₽",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(70, 160),
                Tag = product.Id
            };
            priceLabel.Click += (s, e) => ShowProductDetails(product.Id);
            panel.Controls.Add(priceLabel);

            // Кнопка "В корзину"
            var cartButton = new Button
            {
                Text = "В корзину",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 25),
                Location = new Point(60, 185),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Tag = product.Id
            };
            cartButton.FlatAppearance.BorderSize = 0;
            cartButton.Click += (s, e) => AddToCart(product.Id);
            panel.Controls.Add(cartButton);

            panel.Click += (s, e) => ShowProductDetails(product.Id);

            return panel;
        }

        private void ShowProductDetails(int productId)
        {
            var product = products.Find(p => p.Id == productId);
            if (product == null) return;

            // Скрываем панель с товарами
            if (productsPanel != null) productsPanel.Visible = false;
            if (ordersPanel != null) ordersPanel.Visible = false;
            if (cartPanel != null) cartPanel.Visible = false;
            if (deliveryPanel != null) deliveryPanel.Visible = false;
            if (profilePanel != null) profilePanel.Visible = false;

            // Создаем панель с деталями товара
            productDetailsPanel = new Panel
            {
                Location = new Point(0, 180),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 260),
                AutoScroll = true,
                BackColor = Color.White
            };

            // Кнопка "Назад"
            var backButton = new Button
            {
                Text = "← Назад",
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(70, 130, 180),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 30),
                Location = new Point(20, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            backButton.FlatAppearance.BorderSize = 0;
            backButton.Click += (s, e) =>
            {
                productDetailsPanel.Visible = false;
                if (productsPanel != null) productsPanel.Visible = true;
            };
            productDetailsPanel.Controls.Add(backButton);

            // Основной контейнер для информации о товаре
            var detailsContainer = new Panel
            {
                Location = new Point(100, 50),
                Size = new Size(800, 500),
                BackColor = Color.White
            };

            // Картинка товара
            var productImage = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(350, 350),
                Location = new Point(50, 50),
                Image = LoadImageOrDefault(product.ImagePath, 350, 350)
            };
            detailsContainer.Controls.Add(productImage);

            // Информация о товаре
            var infoPanel = new Panel
            {
                Location = new Point(450, 50),
                Size = new Size(350, 400),
                BackColor = Color.White
            };

            var nameLabel = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = false,
                Size = new Size(350, 50),
                Location = new Point(0, 0)
            };
            infoPanel.Controls.Add(nameLabel);

            var priceLabel = new Label
            {
                Text = $"{product.Price} ₽",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(0, 60)
            };
            infoPanel.Controls.Add(priceLabel);

            var descLabel = new Label
            {
                Text = product.Description,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                AutoSize = false,
                Size = new Size(350, 60),
                Location = new Point(0, 100)
            };
            infoPanel.Controls.Add(descLabel);

            // Блок с характеристиками
            var specsLabel = new Label
            {
                Text = "Характеристики:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 180)
            };
            infoPanel.Controls.Add(specsLabel);

            var weightLabel = new Label
            {
                Text = $"Вес: {product.Weight} г",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 210)
            };
            infoPanel.Controls.Add(weightLabel);

            var dimLabel = new Label
            {
                Text = $"Размеры: {product.Dimensions}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 235)
            };
            infoPanel.Controls.Add(dimLabel);

            // Блок с БЖУ (если это пищевой продукт)
            if (product.Calories > 0)
            {
                var nutritionLabel = new Label
                {
                    Text = "Пищевая ценность:",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.Black,
                    AutoSize = true,
                    Location = new Point(0, 265)
                };
                infoPanel.Controls.Add(nutritionLabel);

                var caloriesLabel = new Label
                {
                    Text = $"Калории: {product.Calories} ккал",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Black,
                    AutoSize = true,
                    Location = new Point(0, 295)
                };
                infoPanel.Controls.Add(caloriesLabel);

                var proteinLabel = new Label
                {
                    Text = $"Белки: {product.Protein} г",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Black,
                    AutoSize = true,
                    Location = new Point(0, 320)
                };
                infoPanel.Controls.Add(proteinLabel);

                var fatLabel = new Label
                {
                    Text = $"Жиры: {product.Fat} г",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Black,
                    AutoSize = true,
                    Location = new Point(0, 345)
                };
                infoPanel.Controls.Add(fatLabel);

                var carbsLabel = new Label
                {
                    Text = $"Углеводы: {product.Carbohydrates} г",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Black,
                    AutoSize = true,
                    Location = new Point(0, 370)
                };
                infoPanel.Controls.Add(carbsLabel);
            }

            // Кнопка "В корзину"
            var addToCartButton = new Button
            {
                Text = "Добавить в корзину",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 40),
                Location = new Point(75, 450),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            addToCartButton.FlatAppearance.BorderSize = 0;
            addToCartButton.Click += (s, e) => AddToCart(product.Id);
            detailsContainer.Controls.Add(addToCartButton);

            detailsContainer.Controls.Add(infoPanel);
            productDetailsPanel.Controls.Add(detailsContainer);
            this.Controls.Add(productDetailsPanel);
        }

        private void AddToCart(int productId)
        {
            var product = products.Find(p => p.Id == productId);
            if (product != null)
            {
                // Добавляем товар в текущий заказ
                var existingItem = currentOrder.Items.Find(i => i.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    currentOrder.Items.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        Quantity = 1
                    });
                }

                MessageBox.Show($"Товар \"{product.Name}\" добавлен в корзину!", "Корзина", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowCartPanel()
        {
            // Очищаем предыдущие панели
            ClearPanels();

            cartPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 60), // Учитываем нижнюю панель
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
            cartPanel.Controls.Add(title);

            if (currentOrder.Items.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "Ваша корзина пуста",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(20, 50)
                };
                cartPanel.Controls.Add(emptyLabel);
            }
            else
            {
                var orderItemsPanel = new Panel
                {
                    Location = new Point(20, 50),
                    Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 180),
                    AutoScroll = true,
                    BackColor = Color.White
                };

                int yPos = 0;
                decimal total = 0;

                foreach (var item in currentOrder.Items)
                {
                    var product = products.Find(p => p.Id == item.ProductId);
                    if (product == null) continue;

                    var itemPanel = new Panel
                    {
                        Location = new Point(0, yPos),
                        Size = new Size(orderItemsPanel.Width - 20, 70),
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    // Картинка товара
                    var productImage = new PictureBox
                    {
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Size = new Size(50, 50),
                        Location = new Point(10, 10),
                        Image = LoadImageOrDefault(product.ImagePath, 50, 50)
                    };
                    itemPanel.Controls.Add(productImage);

                    // Название и цена товара
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

                    // Кнопки управления количеством
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

                    // Кнопка удаления
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

                cartPanel.Controls.Add(orderItemsPanel);

                // Панель итого и оформления
                var totalPanel = new Panel
                {
                    Location = new Point(0, this.ClientSize.Height - 120),
                    Size = new Size(this.ClientSize.Width, 60),
                    BackColor = Color.FromArgb(240, 240, 240)
                };

                // Итого
                var totalLabel = new Label
                {
                    Text = $"Итого: {total} ₽",
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = Color.FromArgb(70, 130, 180),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };
                totalPanel.Controls.Add(totalLabel);

                // Кнопка оформления заказа
                var checkoutButton = new Button
                {
                    Text = "Оформить заказ",
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(200, 40),
                    Location = new Point(this.ClientSize.Width - 230, 10),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                checkoutButton.FlatAppearance.BorderSize = 0;
                checkoutButton.Click += (s, e) => ShowDeliveryOptions();
                totalPanel.Controls.Add(checkoutButton);

                cartPanel.Controls.Add(totalPanel);
            }

            this.Controls.Add(cartPanel);
        }

        private void ShowOrdersPanel()
        {
            // Очищаем предыдущие панели
            if (productsPanel != null) this.Controls.Remove(productsPanel);
            if (productDetailsPanel != null) this.Controls.Remove(productDetailsPanel);
            if (cartPanel != null) this.Controls.Remove(cartPanel);
            if (ordersPanel != null) this.Controls.Remove(ordersPanel);
            if (deliveryPanel != null) this.Controls.Remove(deliveryPanel);
            if (profilePanel != null) this.Controls.Remove(profilePanel);

            ordersPanel = new Panel
            {
                Location = new Point(0, 180),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 260),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Мои заказы",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 10)
            };
            ordersPanel.Controls.Add(title);

            // История заказов
            var historyLabel = new Label
            {
                Text = "История заказов",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 50)
            };
            ordersPanel.Controls.Add(historyLabel);

            if (orders.Count == 0)
            {
                var emptyHistoryLabel = new Label
                {
                    Text = "У вас пока нет завершенных заказов",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(20, 90)
                };
                ordersPanel.Controls.Add(emptyHistoryLabel);
            }
            else
            {
                int orderYPos = 90;
                foreach (var order in orders)
                {
                    var orderPanel = new Panel
                    {
                        Location = new Point(20, orderYPos),
                        Size = new Size(this.ClientSize.Width - 60, 120),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.White
                    };

                    var orderHeader = new Label
                    {
                        Text = $"Заказ #{order.Id} от {order.Date:dd.MM.yyyy}",
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        Location = new Point(10, 10)
                    };
                    orderPanel.Controls.Add(orderHeader);

                    var statusLabel = new Label
                    {
                        Text = $"Статус: {order.Status}",
                        Font = new Font("Segoe UI", 10),
                        ForeColor = GetStatusColor(order.Status),
                        AutoSize = true,
                        Location = new Point(300, 12)
                    };
                    orderPanel.Controls.Add(statusLabel);

                    var totalLabel = new Label
                    {
                        Text = $"Сумма: {order.Total} ₽",
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        Location = new Point(this.ClientSize.Width - 200, 12)
                    };
                    orderPanel.Controls.Add(totalLabel);

                    var itemsList = new Label
                    {
                        Text = GetOrderItemsText(order),
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.Gray,
                        AutoSize = false,
                        Size = new Size(this.ClientSize.Width - 80, 70),
                        Location = new Point(10, 40)
                    };
                    orderPanel.Controls.Add(itemsList);

                    var detailsButton = new Button
                    {
                        Text = "Подробнее",
                        BackColor = Color.FromArgb(70, 130, 180),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(100, 25),
                        Location = new Point(this.ClientSize.Width - 130, 85),
                        Font = new Font("Segoe UI", 9)
                    };
                    detailsButton.FlatAppearance.BorderSize = 0;
                    detailsButton.Tag = order.Id;
                    detailsButton.Click += (s, e) => ShowOrderDetails(order.Id);
                    orderPanel.Controls.Add(detailsButton);

                    ordersPanel.Controls.Add(orderPanel);
                    orderYPos += 130;
                }
            }

            this.Controls.Add(ordersPanel);
        }

        private void ShowDeliveryOptions()
        {
            if (currentOrder.Items.Count == 0)
            {
                MessageBox.Show("Ваша корзина пуста", "Оформление заказа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Скрываем другие панели
            if (productsPanel != null) productsPanel.Visible = false;
            if (productDetailsPanel != null) productDetailsPanel.Visible = false;
            if (ordersPanel != null) ordersPanel.Visible = false;
            if (cartPanel != null) cartPanel.Visible = false;
            if (profilePanel != null) profilePanel.Visible = false;

            // Создаем панель с опциями доставки и оплаты
            deliveryPanel = new Panel
            {
                Location = new Point(0, 180),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 260),
                AutoScroll = true,
                BackColor = Color.White
            };

            // Кнопка "Назад"
            var backButton = new Button
            {
                Text = "← Назад",
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(70, 130, 180),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 30),
                Location = new Point(20, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            backButton.FlatAppearance.BorderSize = 0;
            backButton.Click += (s, e) =>
            {
                deliveryPanel.Visible = false;
                ShowCartPanel();
            };
            deliveryPanel.Controls.Add(backButton);

            var titleLabel = new Label
            {
                Text = "Оформление заказа",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 50)
            };
            deliveryPanel.Controls.Add(titleLabel);

            // Секция способа получения
            var deliveryGroup = new GroupBox
            {
                Text = "Способ получения",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 100),
                Size = new Size(this.ClientSize.Width - 60, 100),
                BackColor = Color.White
            };

            var pickupRadio = new RadioButton
            {
                Text = "Самовывоз",
                Checked = deliveryMethod == "Самовывоз",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 30),
                AutoSize = true
            };
            pickupRadio.CheckedChanged += (s, e) => { if (pickupRadio.Checked) deliveryMethod = "Самовывоз"; };
            deliveryGroup.Controls.Add(pickupRadio);

            var deliveryRadio = new RadioButton
            {
                Text = "Доставка",
                Checked = deliveryMethod == "Доставка",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 60),
                AutoSize = true
            };
            deliveryRadio.CheckedChanged += (s, e) => { if (deliveryRadio.Checked) deliveryMethod = "Доставка"; };
            deliveryGroup.Controls.Add(deliveryRadio);

            deliveryPanel.Controls.Add(deliveryGroup);

            // Секция способа оплаты
            var paymentGroup = new GroupBox
            {
                Text = "Способ оплаты",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 220),
                Size = new Size(this.ClientSize.Width - 60, 100),
                BackColor = Color.White
            };

            var payNowRadio = new RadioButton
            {
                Text = "Оплатить сразу",
                Checked = paymentMethod == "Сразу",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 30),
                AutoSize = true
            };
            payNowRadio.CheckedChanged += (s, e) => { if (payNowRadio.Checked) paymentMethod = "Сразу"; };
            paymentGroup.Controls.Add(payNowRadio);

            var payLaterRadio = new RadioButton
            {
                Text = "Оплатить при получении",
                Checked = paymentMethod == "После получения товара",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 60),
                AutoSize = true
            };
            payLaterRadio.CheckedChanged += (s, e) => { if (payLaterRadio.Checked) paymentMethod = "После получения товара"; };
            paymentGroup.Controls.Add(payLaterRadio);

            deliveryPanel.Controls.Add(paymentGroup);

            // Кнопка подтверждения заказа
            var confirmOrderButton = new Button
            {
                Text = "Подтвердить заказ",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 40),
                Location = new Point(this.ClientSize.Width / 2 - 100, 350),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            confirmOrderButton.FlatAppearance.BorderSize = 0;
            confirmOrderButton.Click += (s, e) => ConfirmOrder();
            deliveryPanel.Controls.Add(confirmOrderButton);

            this.Controls.Add(deliveryPanel);
        }

        private void ConfirmOrder()
        {
            // В реальном приложении здесь была бы логика оформления заказа
            currentOrder.Id = new Random().Next(1000, 9999);
            currentOrder.Date = DateTime.Now;
            currentOrder.Status = "В обработке";
            currentOrder.Total = 0;
            currentOrder.DeliveryMethod = deliveryMethod;
            currentOrder.PaymentMethod = paymentMethod;

            foreach (var item in currentOrder.Items)
            {
                currentOrder.Total += item.Price * item.Quantity;
            }

            orders.Insert(0, new Order
            {
                Id = currentOrder.Id,
                Date = currentOrder.Date,
                Status = currentOrder.Status,
                Total = currentOrder.Total,
                Items = new List<OrderItem>(currentOrder.Items),
                DeliveryMethod = currentOrder.DeliveryMethod,
                PaymentMethod = currentOrder.PaymentMethod
            });

            currentOrder.Items.Clear();

            MessageBox.Show($"Заказ #{currentOrder.Id} успешно оформлен!\n" +
                          $"Способ получения: {deliveryMethod}\n" +
                          $"Способ оплаты: {paymentMethod}",
                          "Оформление заказа", MessageBoxButtons.OK, MessageBoxIcon.Information);

            deliveryPanel.Visible = false;
            ShowOrdersPanel();
        }

        private void UpdateCartItemQuantity(int productId, int change)
        {
            var item = currentOrder.Items.Find(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity += change;
                if (item.Quantity <= 0)
                {
                    currentOrder.Items.Remove(item);
                }
                ShowCartPanel();
            }
        }

        private void RemoveFromCart(int productId)
        {
            var item = currentOrder.Items.Find(i => i.ProductId == productId);
            if (item != null)
            {
                currentOrder.Items.Remove(item);
                ShowCartPanel();
            }
        }

        private void ShowOrderDetails(int orderId)
        {
            var order = orders.Find(o => o.Id == orderId);
            if (order == null) return;

            var detailsForm = new Form
            {
                Text = $"Детали заказа #{order.Id}",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                BackColor = Color.White
            };

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            var orderNumberLabel = new Label
            {
                Text = $"Заказ #{order.Id}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            headerPanel.Controls.Add(orderNumberLabel);

            var dateLabel = new Label
            {
                Text = $"Дата: {order.Date:dd.MM.yyyy HH:mm}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(20, 45)
            };
            headerPanel.Controls.Add(dateLabel);

            var statusLabel = new Label
            {
                Text = $"Статус: {order.Status}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = GetStatusColor(order.Status),
                AutoSize = true,
                Location = new Point(300, 45)
            };
            headerPanel.Controls.Add(statusLabel);

            // Добавляем информацию о доставке и оплате
            var deliveryLabel = new Label
            {
                Text = $"Способ получения: {order.DeliveryMethod}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(20, 65)
            };
            headerPanel.Controls.Add(deliveryLabel);

            var paymentLabel = new Label
            {
                Text = $"Способ оплаты: {order.PaymentMethod}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(300, 65)
            };
            headerPanel.Controls.Add(paymentLabel);

            detailsForm.Controls.Add(headerPanel);

            var itemsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White
            };

            int yPos = 10;
            foreach (var item in order.Items)
            {
                var product = products.Find(p => p.Id == item.ProductId);
                if (product == null) continue;

                var itemPanel = new Panel
                {
                    Location = new Point(10, yPos),
                    Size = new Size(550, 80),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var productImage = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(60, 60),
                    Location = new Point(10, 10),
                    Image = LoadImageOrDefault(product.ImagePath, 60, 60)
                };
                itemPanel.Controls.Add(productImage);

                var nameLabel = new Label
                {
                    Text = product.Name,
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Black,
                    AutoSize = false,
                    Size = new Size(300, 20),
                    Location = new Point(80, 15)
                };
                itemPanel.Controls.Add(nameLabel);

                var priceLabel = new Label
                {
                    Text = $"{item.Price} ₽ x {item.Quantity} = {item.Price * item.Quantity} ₽",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Black,
                    AutoSize = true,
                    Location = new Point(80, 40)
                };
                itemPanel.Controls.Add(priceLabel);

                itemsPanel.Controls.Add(itemPanel);
                yPos += 90;
            }

            var totalPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            var totalLabel = new Label
            {
                Text = $"Итого: {order.Total} ₽",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(400, 20)
            };
            totalPanel.Controls.Add(totalLabel);

            detailsForm.Controls.Add(itemsPanel);
            detailsForm.Controls.Add(totalPanel);

            detailsForm.ShowDialog();
        }

        private void ShowProfilePanel()
        {
            ClearPanels();

            profilePanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 60), // Учитываем нижнюю панель
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Личный кабинет",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 10)
            };
            profilePanel.Controls.Add(title);

            // Основной контейнер профиля
            var profileContainer = new Panel
            {
                Location = new Point(20, 40),
                Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 120),
                BackColor = Color.White
            };

            // Фото профиля и личная информация в одной строке
            var topPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(profileContainer.Width, 150),
                BackColor = Color.White
            };

            // Фото профиля
            var photoPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(120, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            var photoPicture = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(110, 110),
                Location = new Point(5, 5),
                Image = LoadImageOrDefault(userProfile.PhotoPath, 110, 110)
            };
            photoPanel.Controls.Add(photoPicture);

            var changePhotoButton = new Button
            {
                Text = "Изменить фото",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 30),
                Location = new Point(5, 115),
                Font = new Font("Segoe UI", 8)
            };
            changePhotoButton.FlatAppearance.BorderSize = 0;
            changePhotoButton.Click += (s, e) => ChangeProfilePhoto();
            photoPanel.Controls.Add(changePhotoButton);

            topPanel.Controls.Add(photoPanel);

            // Информация о пользователе
            var infoPanel = new Panel
            {
                Location = new Point(130, 0),
                Size = new Size(profileContainer.Width - 130, 150),
                BackColor = Color.White
            };

            // Личная информация
            var personalInfoLabel = new Label
            {
                Text = "Личная информация",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(0, 0)
            };
            infoPanel.Controls.Add(personalInfoLabel);

            // Имя
            var nameLabel = new Label
            {
                Text = "Имя:",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 30)
            };
            infoPanel.Controls.Add(nameLabel);

            var nameValue = new TextBox
            {
                Text = userProfile.Name,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 30),
                BorderStyle = BorderStyle.FixedSingle
            };
            infoPanel.Controls.Add(nameValue);

            // Телефон
            var phoneLabel = new Label
            {
                Text = "Телефон:",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 60)
            };
            infoPanel.Controls.Add(phoneLabel);

            var phoneValue = new TextBox
            {
                Text = userProfile.Phone,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 60),
                BorderStyle = BorderStyle.FixedSingle
            };
            infoPanel.Controls.Add(phoneValue);

            // Email
            var emailLabel = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 90)
            };
            infoPanel.Controls.Add(emailLabel);

            var emailValue = new TextBox
            {
                Text = userProfile.Email,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 90),
                BorderStyle = BorderStyle.FixedSingle
            };
            infoPanel.Controls.Add(emailValue);

            // Пароль
            var passwordLabel = new Label
            {
                Text = "Пароль:",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 120)
            };
            infoPanel.Controls.Add(passwordLabel);

            var passwordValue = new TextBox
            {
                Text = userProfile.Password,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 120),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '*'
            };
            infoPanel.Controls.Add(passwordValue);

            // Кнопка сохранения
            var saveButton = new Button
            {
                Text = "Сохранить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 25),
                Location = new Point(infoPanel.Width - 130, 120),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += (s, e) =>
            {
                userProfile.Name = nameValue.Text;
                userProfile.Phone = phoneValue.Text;
                userProfile.Email = emailValue.Text;
                userProfile.Password = passwordValue.Text;
                MessageBox.Show("Изменения сохранены!", "Профиль", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            infoPanel.Controls.Add(saveButton);

            topPanel.Controls.Add(infoPanel);
            profileContainer.Controls.Add(topPanel);

            // Разделы профиля
            var sectionsPanel = new Panel
            {
                Location = new Point(0, 160),
                Size = new Size(profileContainer.Width, profileContainer.Height - 160),
                BackColor = Color.White
            };

            // Размеры кнопок
            int buttonWidth = (sectionsPanel.Width - 60) / 3;
            int buttonHeight = 60;

            // Техподдержка
            var supportButton = new Button
            {
                Text = "Техподдержка",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 9)
            };
            supportButton.Click += (s, e) => ShowMessage("Раздел техподдержки");
            sectionsPanel.Controls.Add(supportButton);

            // История заказов
            var ordersButton = new Button
            {
                Text = "История заказов",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(30 + buttonWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            ordersButton.Click += (s, e) => ShowOrdersPanel();
            sectionsPanel.Controls.Add(ordersButton);

            // Личная информация
            var personalInfoButton = new Button
            {
                Text = "Личная информация",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(40 + buttonWidth * 2, 20),
                Font = new Font("Segoe UI", 9)
            };
            personalInfoButton.Click += (s, e) => ShowMessage("Раздел личной информации");
            sectionsPanel.Controls.Add(personalInfoButton);

            // Адрес
            var addressButton = new Button
            {
                Text = "Адрес",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(20, 90),
                Font = new Font("Segoe UI", 9)
            };
            addressButton.Click += (s, e) => ShowMessage("Раздел адреса");
            sectionsPanel.Controls.Add(addressButton);

            // Способ оплаты
            var paymentButton = new Button
            {
                Text = "Способ оплаты",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(30 + buttonWidth, 90),
                Font = new Font("Segoe UI", 9)
            };
            paymentButton.Click += (s, e) => ShowMessage("Раздел способов оплаты");
            sectionsPanel.Controls.Add(paymentButton);

            profileContainer.Controls.Add(sectionsPanel);
            profilePanel.Controls.Add(profileContainer);
            this.Controls.Add(profilePanel);
        }
        private void ChangeProfilePhoto()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите фото профиля"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    userProfile.PhotoPath = openFileDialog.FileName;
                    ShowProfilePanel(); // Обновляем панель профиля
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Color GetStatusColor(string status)
        {
            switch (status)
            {
                case "Доставлен": return Color.Green;
                case "В пути": return Color.Blue;
                case "В обработке": return Color.Orange;
                case "Отменен": return Color.Red;
                default: return Color.Black;
            }
        }

        private string GetOrderItemsText(Order order)
        {
            var itemsText = "";
            foreach (var item in order.Items)
            {
                var product = products.Find(p => p.Id == item.ProductId);
                if (product != null)
                {
                    itemsText += $"{product.Name} - {item.Quantity} x {item.Price} ₽\n";
                }
            }
            return itemsText.Trim();
        }

        private void ShowCustomerMenu()
        {
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            int buttonCount = 4;
            int buttonWidth = this.ClientSize.Width / buttonCount;

            var productsButton = CreateBottomButton("Товары", 0, buttonWidth);
            productsButton.Dock = DockStyle.Left; // Растягиваем кнопку по левому краю
            productsButton.Click += (s, e) => ShowProductsPanel();

            var ordersButton = CreateBottomButton("Заказы", 1, buttonWidth);
            ordersButton.Dock = DockStyle.Left; // Растягиваем кнопку по левому краю
            ordersButton.Click += (s, e) => ShowOrdersPanel();

            var cartButton = CreateBottomButton("Корзина", 2, buttonWidth);
            cartButton.Dock = DockStyle.Left; // Растягиваем кнопку по левому краю
            cartButton.Click += (s, e) => ShowCartPanel();

            var accountButton = CreateBottomButton("Профиль", 3, buttonWidth);
            accountButton.Dock = DockStyle.Left; // Растягиваем кнопку по левому краю
            accountButton.Click += (s, e) => ShowProfilePanel();

            // Добавляем кнопки в обратном порядке для правильного расположения
            bottomPanel.Controls.Add(accountButton);
            bottomPanel.Controls.Add(cartButton);
            bottomPanel.Controls.Add(ordersButton);
            bottomPanel.Controls.Add(productsButton);

            this.Controls.Add(bottomPanel);
        }


        private Button CreateBottomButton(string text, int index, int buttonWidth)
        {
            return new Button
            {
                Text = text,
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = buttonWidth, // Ширина равна вычисленной ширине
                Height = 60, // Высота равна высоте панели
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Tag = index,
                Margin = new Padding(0) // Убираем отступы
            };
        }
        private void ShowEmployeeRoleSelection()
        {
            var roleGroup = new GroupBox
            {
                Text = "Выберите роль",
                Font = new Font("Segoe UI", 10),
                Location = new Point(this.ClientSize.Width / 2 - 200, this.ClientSize.Height / 2 - 125),
                Size = new Size(400, 250)
            };

            var directorRadio = new RadioButton { Text = "Директор", Location = new Point(20, 30), Font = new Font("Segoe UI", 10) };
            var sellerRadio = new RadioButton { Text = "Продавец", Location = new Point(20, 60), Font = new Font("Segoe UI", 10) };
            var courierRadio = new RadioButton { Text = "Курьер", Location = new Point(20, 90), Font = new Font("Segoe UI", 10) };
            var cookRadio = new RadioButton { Text = "Повар", Location = new Point(20, 120), Font = new Font("Segoe UI", 10) };
            var hallStaffRadio = new RadioButton { Text = "Работник зала", Location = new Point(20, 150), Font = new Font("Segoe UI", 10) };
            var supportStaffRadio = new RadioButton { Text = "Техподдержка", Location = new Point(20, 180), Font = new Font("Segoe UI", 10) };

            var confirmButton = new Button
            {
                Text = "Подтвердить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(150, 210),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            confirmButton.FlatAppearance.BorderSize = 0;
            confirmButton.Click += (s, e) =>
            {
                if (directorRadio.Checked) userRole = "Директор";
                else if (sellerRadio.Checked) userRole = "Продавец";
                else if (courierRadio.Checked) userRole = "Курьер";
                else if (cookRadio.Checked) userRole = "Повар";
                else if (hallStaffRadio.Checked) userRole = "Работник зала";
                else if (supportStaffRadio.Checked) userRole = "Техподдержка";

                if (!string.IsNullOrEmpty(userRole))
                {
                    ShowEmployeeMenu();
                    roleGroup.Visible = false;
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите вашу роль");
                }
            };

            roleGroup.Controls.Add(directorRadio);
            roleGroup.Controls.Add(sellerRadio);
            roleGroup.Controls.Add(courierRadio);
            roleGroup.Controls.Add(cookRadio);
            roleGroup.Controls.Add(hallStaffRadio);
            roleGroup.Controls.Add(supportStaffRadio);
            roleGroup.Controls.Add(confirmButton);

            this.Controls.Add(roleGroup);
        }
        private void ShowEmployeeMenu()
        {
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            var profileButton = CreateBottomButton("Профиль", 0);
            profileButton.Click += (s, e) => ShowMessage("Переход в профиль");

            var ordersButton = CreateBottomButton("Заказы", 1);
            ordersButton.Click += (s, e) => ShowMessage("Переход к заказам");

            Button roleSpecificButton1 = null;
            Button roleSpecificButton2 = null;

            switch (userRole)
            {
                case "Директор":
                    roleSpecificButton1 = CreateBottomButton("Отчеты", 2);
                    roleSpecificButton2 = CreateBottomButton("Управление", 3);
                    break;
                case "Продавец":
                    roleSpecificButton1 = CreateBottomButton("Касса", 2);
                    roleSpecificButton2 = CreateBottomButton("Товары", 3);
                    break;
                case "Курьер":
                    roleSpecificButton1 = CreateBottomButton("Доставки", 2);
                    roleSpecificButton2 = CreateBottomButton("Маршрут", 3);
                    break;
                case "Повар":
                    roleSpecificButton1 = CreateBottomButton("Заказы", 2);
                    roleSpecificButton2 = CreateBottomButton("Меню", 3);
                    break;
                case "Работник зала":
                    roleSpecificButton1 = CreateBottomButton("Столики", 2);
                    roleSpecificButton2 = CreateBottomButton("Бронирование", 3);
                    break;
                case "Техподдержка":
                    roleSpecificButton1 = CreateBottomButton("Заявки", 2);
                    roleSpecificButton2 = CreateBottomButton("Помощь", 3);
                    break;
            }

            bottomPanel.Controls.Add(profileButton);
            bottomPanel.Controls.Add(ordersButton);

            if (roleSpecificButton1 != null)
            {
                roleSpecificButton1.Click += (s, e) => ShowMessage($"Функция {roleSpecificButton1.Text} для {userRole}");
                bottomPanel.Controls.Add(roleSpecificButton1);
            }

            if (roleSpecificButton2 != null)
            {
                roleSpecificButton2.Click += (s, e) => ShowMessage($"Функция {roleSpecificButton2.Text} для {userRole}");
                bottomPanel.Controls.Add(roleSpecificButton2);
            }

            this.Controls.Add(bottomPanel);
        }

        private Button CreateBottomButton(string text, int index)
        {
            int buttonWidth = this.ClientSize.Width / 4;
            return new Button
            {
                Text = text,
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth - 10, 60),
                Location = new Point(5 + index * buttonWidth, 10),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Tag = index
            };
        }

        private Image LoadImageOrDefault(string path, int width, int height)
        {
            try
            {
                if (File.Exists(path))
                    return Image.FromFile(path);
            }
            catch { }

            // Создаем изображение-заглушку
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                using (var brush = new SolidBrush(Color.DarkGray))
                {
                    g.DrawString("Нет изображения", new Font("Arial", 10), brush, 10, 10);
                }
            }
            return bmp;
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}