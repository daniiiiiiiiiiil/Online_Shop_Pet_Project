using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class OrderHelper
    {
        private MainMenuForm form;
        public OrderHelper(MainMenuForm form) { this.form = form; }

        public void InitializeOrders()
        {
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

            form.orders.Add(order1);
            form.orders.Add(order2);
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
                foreach (var order in form.orders)
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
                    detailsButton.Tag = order.Id;
                    detailsButton.Click += (s, e) => ShowOrderDetails(order.Id);
                    orderPanel.Controls.Add(detailsButton);

                    form.ordersPanel.Controls.Add(orderPanel);
                    yPos += 160;
                }
            }

            form.Controls.Add(form.ordersPanel);
        }

        public void ShowOrderDetails(int orderId)
        {
            var order = form.orders.Find(o => o.Id == orderId);
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
                Height = 120,
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
                ForeColor = form.UIHelper.GetStatusColor(order.Status),
                AutoSize = true,
                Location = new Point(300, 15)
            };
            headerPanel.Controls.Add(statusLabel);

            var deliveryLabel = new Label
            {
                Text = $"Способ получения: {order.DeliveryMethod}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(20, 70)
            };
            headerPanel.Controls.Add(deliveryLabel);

            var paymentLabel = new Label
            {
                Text = $"Способ оплаты: {order.PaymentMethod}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(300, 45)
            };
            headerPanel.Controls.Add(paymentLabel);

            var separator = new Label
            {
                Text = "Состав заказа:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(20, 95)
            };
            headerPanel.Controls.Add(separator);

            detailsForm.Controls.Add(headerPanel);

            var itemsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            int yPos = 10;
            foreach (var item in order.Items)
            {
                var product = form.products.Find(p => p.Id == item.ProductId);
                if (product == null) continue;

                var itemPanel = new Panel
                {
                    Location = new Point(10, yPos),
                    Size = new Size(550, 100),
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

                var nameLabel = new Label
                {
                    Text = product.Name,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.Black,
                    AutoSize = false,
                    Size = new Size(300, 20),
                    Location = new Point(100, 15)
                };
                itemPanel.Controls.Add(nameLabel);

                var priceLabel = new Label
                {
                    Text = $"{item.Price} ₽ x {item.Quantity} = {item.Price * item.Quantity} ₽",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Black,
                    AutoSize = true,
                    Location = new Point(100, 40)
                };
                itemPanel.Controls.Add(priceLabel);

                var descLabel = new Label
                {
                    Text = product.Description,
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Gray,
                    AutoSize = false,
                    Size = new Size(300, 40),
                    Location = new Point(100, 60)
                };
                itemPanel.Controls.Add(descLabel);

                itemsPanel.Controls.Add(itemPanel);
                yPos += 110;
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

        public string GetOrderItemsText(Order order)
        {
            var itemsText = new StringBuilder();
            foreach (var item in order.Items)
            {
                var product = form.products.Find(p => p.Id == item.ProductId);
                if (product != null)
                {
                    itemsText.AppendLine($"{product.Name} - {item.Quantity} x {item.Price} ₽ = {item.Quantity * item.Price} ₽");
                }
            }
            return itemsText.ToString().Trim();
        }

        public void ConfirmOrder()
        {
            form.currentOrder.Id = new Random().Next(1000, 9999);
            form.currentOrder.Date = DateTime.Now;
            form.currentOrder.Status = "В обработке";
            form.currentOrder.Total = 0;
            form.currentOrder.DeliveryMethod = form.deliveryMethod;
            form.currentOrder.PaymentMethod = form.paymentMethod;

            foreach (var item in form.currentOrder.Items)
            {
                form.currentOrder.Total += item.Price * item.Quantity;
            }

            form.orders.Insert(0, new Order
            {
                Id = form.currentOrder.Id,
                Date = form.currentOrder.Date,
                Status = form.currentOrder.Status,
                Total = form.currentOrder.Total,
                Items = new List<OrderItem>(form.currentOrder.Items),
                DeliveryMethod = form.currentOrder.DeliveryMethod,
                PaymentMethod = form.currentOrder.PaymentMethod
            });

            form.currentOrder.Items.Clear();

            MessageBox.Show($"Заказ #{form.currentOrder.Id} успешно оформлен!\n" +
                          $"Способ получения: {form.deliveryMethod}\n" +
                          $"Способ оплаты: {form.paymentMethod}",
                          "Оформление заказа", MessageBoxButtons.OK, MessageBoxIcon.Information);

            form.deliveryPanel.Visible = false;
            ShowOrdersPanel();
        }
    }

}
