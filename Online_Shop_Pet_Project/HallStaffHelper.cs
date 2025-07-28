using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class HallStaffHelper
    {
        private MainMenuForm form;
        public HallStaffHelper(MainMenuForm form) { this.form = form; }

        public void ShowHallStaffOrdersPanel()
        {
            form.UIHelper.ClearPanels();

            form.hallStaffOrdersPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Заказы для сборки",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.hallStaffOrdersPanel.Controls.Add(title);

            var orders = new List<HallOrder>
            {
                new HallOrder { Id = 1001, Type = "Самовывоз", Items = "Смартфон Samsung, Наушники Sony", Status = "В обработке", Location = "Зал 1" },
                new HallOrder { Id = 1002, Type = "Доставка", Items = "Пицца Маргарита, Салат Цезарь", Status = "Готов к сборке", Location = "Кухня" },
                new HallOrder { Id = 1003, Type = "Самовывоз", Items = "Книга 'Clean Code'", Status = "Готов к выдаче", Location = "Секция 5" }
            };

            int yPos = 60;
            foreach (var order in orders)
            {
                var orderPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 120),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var idLabel = new Label
                {
                    Text = $"Заказ #{order.Id}",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                orderPanel.Controls.Add(idLabel);

                var typeLabel = new Label
                {
                    Text = $"Тип: {order.Type}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(150, 12),
                    AutoSize = true
                };
                orderPanel.Controls.Add(typeLabel);

                var itemsLabel = new Label
                {
                    Text = $"Товары: {order.Items}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 35),
                    AutoSize = false,
                    Size = new Size(form.ClientSize.Width - 200, 40)
                };
                orderPanel.Controls.Add(itemsLabel);

                var locationLabel = new Label
                {
                    Text = $"Расположение: {order.Location}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 75),
                    AutoSize = true
                };
                orderPanel.Controls.Add(locationLabel);

                var statusLabel = new Label
                {
                    Text = $"Статус: {order.Status}",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = form.UIHelper.GetHallOrderStatusColor(order.Status),
                    Location = new Point(form.ClientSize.Width - 150, 75),
                    AutoSize = true
                };
                orderPanel.Controls.Add(statusLabel);

                if (order.Status == "Готов к сборке")
                {
                    var prepareButton = new Button
                    {
                        Text = "Собрать заказ",
                        BackColor = Color.FromArgb(70, 130, 180),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(120, 30),
                        Location = new Point(form.ClientSize.Width - 280, 75),
                        Font = new Font("Segoe UI", 9),
                        Tag = order.Id
                    };
                    prepareButton.Click += (s, e) => MarkOrderAsReady(order.Id);
                    form.hallStaffOrdersPanel.Controls.Add(prepareButton);
                }

                var locationButton = new Button
                {
                    Text = "Показать на карте",
                    BackColor = Color.FromArgb(100, 150, 200),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(140, 25),
                    Location = new Point(form.ClientSize.Width - 280, 40),
                    Font = new Font("Segoe UI", 8),
                    Tag = order.Id
                };
                locationButton.Click += (s, e) => ShowProductLocation(order.Id);
                form.hallStaffOrdersPanel.Controls.Add(locationButton);

                form.hallStaffOrdersPanel.Controls.Add(orderPanel);
                yPos += 130;
            }

            form.Controls.Add(form.hallStaffOrdersPanel);
        }

        public void ShowStoreMap()
        {
            form.UIHelper.ClearPanels();

            form.storeMapPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Карта магазина",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.storeMapPanel.Controls.Add(title);

            var mapImage = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(form.ClientSize.Width - 40, form.ClientSize.Height - 100),
                Location = new Point(20, 60),
                Image = form.UIHelper.LoadImageOrDefault("art/store_map.jpg", form.ClientSize.Width - 40, form.ClientSize.Height - 100)
            };
            form.storeMapPanel.Controls.Add(mapImage);

            var legend = new Label
            {
                Text = "Легенда:\n" +
                       "🟥 - Электроника\n" +
                       "🟦 - Продукты\n" +
                       "🟩 - Книги\n" +
                       "🟨 - Одежда",
                Font = new Font("Segoe UI", 12),
                Location = new Point(form.ClientSize.Width - 200, 70),
                AutoSize = true,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10)
            };
            form.storeMapPanel.Controls.Add(legend);

            form.Controls.Add(form.storeMapPanel);
        }

        public void ShowProductLocation(int orderId)
        {
            var locations = new Dictionary<int, string>
            {
                {1001, "Секция электроники, стеллаж A3"},
                {1002, "Кухня, холодильник B2"},
                {1003, "Секция книг, стеллаж D7"}
            };

            if (locations.ContainsKey(orderId))
            {
                MessageBox.Show($"Товары заказа #{orderId} находятся:\n{locations[orderId]}", "Расположение товаров");
            }
            else
            {
                MessageBox.Show($"Расположение для заказа #{orderId} не найдено", "Ошибка");
            }
        }

        public void MarkOrderAsReady(int orderId)
        {
            MessageBox.Show($"Заказ #{orderId} отмечен как собранный и готов к выдаче!", "Статус заказа");
            ShowHallStaffOrdersPanel();
        }

        public void ShowHallStaffHistory()
        {
            form.UIHelper.ClearPanels();

            form.hallStaffHistoryPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "История выполненных заданий",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.hallStaffHistoryPanel.Controls.Add(title);

            var historyItems = new List<HallTask>
            {
                new HallTask { Date = DateTime.Now.AddDays(-1), Description = "Сбор заказа #1001 для самовывоза", Status = "Выполнено" },
                new HallTask { Date = DateTime.Now.AddDays(-2), Description = "Подготовка товаров для доставки #1002", Status = "Выполнено" },
                new HallTask { Date = DateTime.Now.AddDays(-3), Description = "Вынос товаров в торговый зал", Status = "Выполнено" }
            };

            int yPos = 60;
            foreach (var task in historyItems)
            {
                var taskPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 60),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var dateLabel = new Label
                {
                    Text = task.Date.ToString("dd.MM.yyyy HH:mm"),
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                taskPanel.Controls.Add(dateLabel);

                var descLabel = new Label
                {
                    Text = task.Description,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(150, 10),
                    AutoSize = true
                };
                taskPanel.Controls.Add(descLabel);

                var statusLabel = new Label
                {
                    Text = task.Status,
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Green,
                    Location = new Point(form.ClientSize.Width - 100, 10),
                    AutoSize = true
                };
                taskPanel.Controls.Add(statusLabel);

                form.hallStaffHistoryPanel.Controls.Add(taskPanel);
                yPos += 70;
            }

            form.Controls.Add(form.hallStaffHistoryPanel);
        }
    }
}