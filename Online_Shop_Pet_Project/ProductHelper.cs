using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class ProductHelper
    {
        private MainMenuForm form;
        public ProductHelper(MainMenuForm form) { this.form = form; }
        
        public void InitializeProducts()
        {
            form.products.Add(new Product
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
            });

            form.products.Add(new Product
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
            });

            form.products.Add(new Product
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
            });

            form.products.Add(new Product
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
            });

            form.products.Add(new Product
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
            });

            form.products.Add(new Product
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
            });
        }

        public Panel CreateProductItem(Product product, int x, int y)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(220, 220),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Tag = product.Id
            };

            var picture = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(150, 100),
                Location = new Point(25, 10),
                Image = form.UIHelper.LoadImageOrDefault(product.ImagePath, 150, 100),
                Tag = product.Id
            };
            picture.Click += (s, e) => ShowProductDetails(product.Id);
            panel.Controls.Add(picture);

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

            var cartButton = new Button
            {
                Text = "В корзину",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(90, 25),
                Location = new Point(55, 180),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Tag = product.Id
            };
            cartButton.FlatAppearance.BorderSize = 0;
            cartButton.Click += (s, e) => AddToCart(product.Id);
            panel.Controls.Add(cartButton);

            panel.Click += (s, e) => ShowProductDetails(product.Id);

            return panel;
        }

        public void ShowProductDetails(int productId)
        {
            var product = form.products.Find(p => p.Id == productId);
            if (product == null) return;

            form.UIHelper.ClearPanels();
            form.productsPanel?.Hide();

            form.productDetailsPanel = new Panel
            {
                Location = new Point(0, 180),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 260),
                AutoScroll = true,
                BackColor = Color.White
            };

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
                form.productDetailsPanel.Visible = false;
                form.productsPanel?.Show();
            };
            form.productDetailsPanel.Controls.Add(backButton);

            var detailsContainer = new Panel
            {
                Location = new Point(100, 50),
                Size = new Size(800, 500),
                BackColor = Color.White
            };

            var productImage = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(350, 350),
                Location = new Point(50, 50),
                Image = form.UIHelper.LoadImageOrDefault(product.ImagePath, 350, 350)
            };
            detailsContainer.Controls.Add(productImage);

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
            form.productDetailsPanel.Controls.Add(detailsContainer);
            form.Controls.Add(form.productDetailsPanel);
        }
        public void ShowProductsPanel()
        {
            form.UIHelper.ClearPanels();

            form.productsPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 80),
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
            form.productsPanel.Controls.Add(title);

            int yPos = 50;
            int xPos = 20;
            int itemsPerRow = Math.Max(1, form.ClientSize.Width / 220);

            for (int i = 0; i < form.products.Count; i++)
            {
                var product = form.products[i];
                var productItem = CreateProductItem(product, xPos, yPos);
                form.productsPanel.Controls.Add(productItem);

                xPos += 220;

                if ((i + 1) % itemsPerRow == 0)
                {
                    xPos = 20;
                    yPos += 220;
                }
            }

            form.Controls.Add(form.productsPanel);
        }
        public void AddToCart(int productId)
        {
            var product = form.products.Find(p => p.Id == productId);
            if (product != null)
            {
                var existingItem = form.currentOrder.Items.Find(i => i.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    form.currentOrder.Items.Add(new OrderItem
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
    }
}