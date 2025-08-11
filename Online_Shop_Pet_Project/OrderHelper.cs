using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class OrderHelper
    {
        private MainMenuForm form;
        private DatabaseHelper dbHelper;

        public OrderHelper(MainMenuForm form)
        {
            this.form = form;
            this.dbHelper = new DatabaseHelper();
        }

        public void InitializeOrders()
        {
            form.orders = dbHelper.LoadOrders();
            if (form.orders.Count == 0)
            {
                var testOrder = new Order
                {
                    Id = 1001,
                    Date = DateTime.Now.AddDays(-5),
                    Status = "Доставлен",
                    Total = 83980,
                    DeliveryMethod = "Курьер",
                    PaymentMethod = "Картой",
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, Quantity = 1, Price = 79990, ProductName = "Смартфон Samsung Galaxy S23" },
                        new OrderItem { ProductId = 5, Quantity = 1, Price = 899, ProductName = "Кофе зерновой Lavazza" }
                    }
                };
                dbHelper.SaveOrder(testOrder);
                form.orders.Add(testOrder);
            }
        }

        public void ShowOrdersPanel()
        {
            form.UIHelper.ClearPanels();

            form.ordersPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
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
            form.ordersPanel.Controls.Add(title);

            var historyLabel = new Label
            {
                Text = "История заказов",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 50)
            };
            form.ordersPanel.Controls.Add(historyLabel);

            if (form.orders.Count == 0)
            {
                var emptyHistoryLabel = new Label
                {
                    Text = "У вас пока нет завершенных заказов",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(20, 90)
                };
                form.ordersPanel.Controls.Add(emptyHistoryLabel);
            }
            else
            {
                int yPos = 90;
                foreach (var order in form.orders.OrderByDescending(o => o.Date))
                {
                    var orderPanel = CreateOrderPanel(order, yPos);
                    form.ordersPanel.Controls.Add(orderPanel);
                    yPos += 160;
                }
            }

            form.Controls.Add(form.ordersPanel);
        }

        private Panel CreateOrderPanel(Order order, int yPos)
        {
            var orderPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(form.ClientSize.Width - 40, 150),
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
                ForeColor = form.UIHelper.GetStatusColor(order.Status),
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
                Location = new Point(form.ClientSize.Width - 200, 12)
            };
            orderPanel.Controls.Add(totalLabel);

            var itemsList = new Label
            {
                Text = GetOrderItemsText(order),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = false,
                Size = new Size(form.ClientSize.Width - 80, 80),
                Location = new Point(10, 40)
            };
            orderPanel.Controls.Add(itemsList);

            var detailsButton = new Button
            {
                Text = "Подробнее",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(form.ClientSize.Width - 170, 110),
                Font = new Font("Segoe UI", 9)
            };
            detailsButton.Click += (s, e) => ShowOrderDetails(order.Id);
            orderPanel.Controls.Add(detailsButton);

            return orderPanel;
        }

        public void ShowOrderDetails(int orderId)
        {
            var order = form.orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return;

            var detailsForm = new Form
            {
                Text = $"Детали заказа #{order.Id}",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                BackColor = Color.White,
                Size = new Size(600, 500)
            };

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(20)
            };

            headerPanel.Controls.Add(new Label
            {
                Text = $"Заказ #{order.Id}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(0, 0)
            });

            headerPanel.Controls.Add(new Label
            {
                Text = $"Дата: {order.Date:dd.MM.yyyy HH:mm}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 30)
            });

            headerPanel.Controls.Add(new Label
            {
                Text = $"Статус: {order.Status}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = form.UIHelper.GetStatusColor(order.Status),
                AutoSize = true,
                Location = new Point(300, 0)
            });

            headerPanel.Controls.Add(new Label
            {
                Text = $"Способ получения: {order.DeliveryMethod}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 60)
            });

            headerPanel.Controls.Add(new Label
            {
                Text = $"Способ оплаты: {order.PaymentMethod}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(300, 30)
            });

            var itemsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White
            };

            int yPos = 10;
            foreach (var item in order.Items)
            {
                var product = form.products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null) continue;

                var itemPanel = new Panel
                {
                    Size = new Size(540, 100),
                    Location = new Point(10, yPos),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var productImage = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(80, 80),
                    Location = new Point(10, 10),
                    Image = form.UIHelper.LoadImageOrDefault(product.ImagePath, 80, 80)
                };
                itemPanel.Controls.Add(productImage);

                itemPanel.Controls.Add(new Label
                {
                    Text = product.Name,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.Black,
                    Size = new Size(300, 20),
                    Location = new Point(100, 15)
                });

                itemPanel.Controls.Add(new Label
                {
                    Text = $"{item.Price} ₽ x {item.Quantity} = {item.Price * item.Quantity} ₽",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Black,
                    AutoSize = true,
                    Location = new Point(100, 40)
                });

                itemsPanel.Controls.Add(itemPanel);
                yPos += 110;
            }

            var totalPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            totalPanel.Controls.Add(new Label
            {
                Text = $"Итого: {order.Total} ₽",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(400, 20)
            });

            detailsForm.Controls.Add(headerPanel);
            detailsForm.Controls.Add(itemsPanel);
            detailsForm.Controls.Add(totalPanel);

            detailsForm.ShowDialog();
        }

        public string GetOrderItemsText(Order order)
        {
            var sb = new StringBuilder();
            foreach (var item in order.Items)
            {
                var product = form.products.FirstOrDefault(p => p.Id == item.ProductId);
                sb.AppendLine($"{product?.Name ?? "Товар"} - {item.Quantity} x {item.Price} ₽");
            }
            return sb.ToString();
        }

        public void ConfirmOrder()
        {
            if (form.currentOrder.Items.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            form.currentOrder.Id = new Random().Next(1000, 9999);
            form.currentOrder.Date = DateTime.Now;
            form.currentOrder.Status = "В обработке";
            form.currentOrder.Total = form.currentOrder.Items.Sum(i => i.Price * i.Quantity);
            form.currentOrder.DeliveryMethod = form.deliveryMethod;
            form.currentOrder.PaymentMethod = form.paymentMethod;

            dbHelper.SaveOrder(form.currentOrder);

            dbHelper.ClearCart();
            form.currentOrder.Items.Clear();

            form.orders.Insert(0, new Order(form.currentOrder));
            ShowOrdersPanel();

            MessageBox.Show(
                $"Заказ #{form.currentOrder.Id} оформлен!\n" +
                $"Сумма: {form.currentOrder.Total} ₽\n" +
                $"Статус: {form.currentOrder.Status}",
                "Успешно",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}