using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class DeliveryHelper
    {
        private MainMenuForm form;
        public UIHelper uIHelper;
        private List<Delivery> allDeliveries = new List<Delivery>();
        private List<Delivery> completedDeliveries = new List<Delivery>();
        private Image routeMapImage;

        public DeliveryHelper(MainMenuForm form)
        {
            this.form = form;
            InitializeSampleData();
            LoadMapImage();
        }

        private void InitializeSampleData()
        {
            allDeliveries = new List<Delivery>
            {
                new Delivery
                {
                    Id = 1001,
                    Address = "ул. Ленина, д. 10, кв. 5",
                    CustomerName = "Иванов Иван",
                    CustomerPhone = "+7 (123) 456-78-90",
                    Status = "Поступил",
                    StatusHistory = new List<DeliveryStatus>
                    {
                        new DeliveryStatus("Поступил", DateTime.Now.AddHours(-2))
                    },
                    Payment = 500,
                    OrderItems = new List<string> { "Пицца Маргарита", "Кока-кола 1л" }
                },
                new Delivery
                {
                    Id = 1002,
                    Address = "пр. Мира, д. 25, кв. 12",
                    CustomerName = "Петрова Анна",
                    CustomerPhone = "+7 (987) 654-32-10",
                    Status = "Поступил",
                    StatusHistory = new List<DeliveryStatus>
                    {
                        new DeliveryStatus("Поступил", DateTime.Now.AddHours(-1))
                    },
                    Payment = 450,
                    OrderItems = new List<string> { "Ролл Калифорния", "Суп Том Ям" }
                }
            };
        }

        private void LoadMapImage()
        {
            try
            {
                routeMapImage = Image.FromFile("map_placeholder.png");
            }
            catch
            {
                routeMapImage = new Bitmap(600, 400);
                using (var g = Graphics.FromImage(routeMapImage))
                {
                    g.Clear(Color.White);
                    g.DrawString("Карта маршрута", new Font("Arial", 20), Brushes.Black, 200, 180);
                }
            }
        }

        public void ShowDeliveryOptions()
        {
            if (form.currentOrder.Items.Count == 0)
            {
                MessageBox.Show("Ваша корзина пуста", "Оформление заказа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            form.UIHelper.ClearPanels();

            form.deliveryPanel = new Panel
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
                form.deliveryPanel.Visible = false;
                form.CartHelper.ShowCartPanel();
            };
            form.deliveryPanel.Controls.Add(backButton);

            var titleLabel = new Label
            {
                Text = "Оформление заказа",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 50)
            };
            form.deliveryPanel.Controls.Add(titleLabel);

            var deliveryGroup = new GroupBox
            {
                Text = "Способ получения",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 100),
                Size = new Size(form.ClientSize.Width - 60, 100),
                BackColor = Color.White
            };

            var pickupRadio = new RadioButton
            {
                Text = "Самовывоз",
                Checked = form.deliveryMethod == "Самовывоз",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 30),
                AutoSize = true
            };
            pickupRadio.CheckedChanged += (s, e) => { if (pickupRadio.Checked) form.deliveryMethod = "Самовывоз"; };
            deliveryGroup.Controls.Add(pickupRadio);

            var deliveryRadio = new RadioButton
            {
                Text = "Доставка",
                Checked = form.deliveryMethod == "Доставка",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 60),
                AutoSize = true
            };
            deliveryRadio.CheckedChanged += (s, e) => { if (deliveryRadio.Checked) form.deliveryMethod = "Доставка"; };
            deliveryGroup.Controls.Add(deliveryRadio);

            form.deliveryPanel.Controls.Add(deliveryGroup);

            var paymentGroup = new GroupBox
            {
                Text = "Способ оплаты",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 220),
                Size = new Size(form.ClientSize.Width - 60, 100),
                BackColor = Color.White
            };

            var payNowRadio = new RadioButton
            {
                Text = "Оплатить сразу",
                Checked = form.paymentMethod == "Сразу",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 30),
                AutoSize = true
            };
            payNowRadio.CheckedChanged += (s, e) => { if (payNowRadio.Checked) form.paymentMethod = "Сразу"; };
            paymentGroup.Controls.Add(payNowRadio);

            var payLaterRadio = new RadioButton
            {
                Text = "Оплатить при получении",
                Checked = form.paymentMethod == "После получения товара",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 60),
                AutoSize = true
            };
            payLaterRadio.CheckedChanged += (s, e) => { if (payLaterRadio.Checked) form.paymentMethod = "После получения товара"; };
            paymentGroup.Controls.Add(payLaterRadio);

            form.deliveryPanel.Controls.Add(paymentGroup);

            var confirmOrderButton = new Button
            {
                Text = "Подтвердить заказ",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 40),
                Location = new Point(form.ClientSize.Width / 2 - 100, 350),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            confirmOrderButton.FlatAppearance.BorderSize = 0;
            confirmOrderButton.Click += (s, e) => form.OrderHelper.ConfirmOrder();
            form.deliveryPanel.Controls.Add(confirmOrderButton);

            form.Controls.Add(form.deliveryPanel);
        }

        public void ShowCourierDeliveries()
        {
            form.UIHelper.ClearPanels();

            form.deliveriesPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Мои доставки",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.deliveriesPanel.Controls.Add(title);

            int yPos = 60;
            foreach (var delivery in allDeliveries)
            {
                var deliveryPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.ClientSize.Width - 40, 100),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var idLabel = new Label
                {
                    Text = $"Доставка #{delivery.Id}",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                deliveryPanel.Controls.Add(idLabel);

                var addressLabel = new Label
                {
                    Text = $"Адрес: {delivery.Address}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 35),
                    AutoSize = true
                };
                deliveryPanel.Controls.Add(addressLabel);

                var customerLabel = new Label
                {
                    Text = $"Клиент: {delivery.CustomerName}",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 55),
                    AutoSize = true
                };
                deliveryPanel.Controls.Add(customerLabel);

                var statusLabel = new Label
                {
                    Text = $"Статус: {delivery.Status}",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(form.ClientSize.Width - 150, 35),
                    AutoSize = true
                };
                deliveryPanel.Controls.Add(statusLabel);

                var paymentLabel = new Label
                {
                    Text = $"Оплата: {delivery.Payment} ₽",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(form.ClientSize.Width - 150, 55),
                    AutoSize = true
                };
                deliveryPanel.Controls.Add(paymentLabel);

                var detailsButton = new Button
                {
                    Text = "Подробнее",
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(100, 25),
                    Location = new Point(form.ClientSize.Width - 270, 10),
                    Font = new Font("Segoe UI", 9)
                };
                detailsButton.Click += (s, e) => ShowDeliveryDetails(delivery.Id);
                deliveryPanel.Controls.Add(detailsButton);

                form.deliveriesPanel.Controls.Add(deliveryPanel);
                yPos += 110;
            }

            form.Controls.Add(form.deliveriesPanel);
        }

        public void ShowCourierRoute()
        {
            form.UIHelper.ClearPanels();

            form.routePanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Мой маршрут",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.routePanel.Controls.Add(title);

            var mapBox = new PictureBox
            {
                Image = routeMapImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(form.routePanel.Width - 40, 200),
                Location = new Point(20, 50),
                BorderStyle = BorderStyle.FixedSingle
            };
            form.routePanel.Controls.Add(mapBox);

            var historyLabel = new Label
            {
                Text = "История доставок:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 270),
                AutoSize = true
            };
            form.routePanel.Controls.Add(historyLabel);

            var routePoints = allDeliveries
                .OrderBy(d => d.StatusHistory.Last().Timestamp)
                .ToList();

            int yPos = 310;
            foreach (var delivery in routePoints)
            {
                var pointPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.routePanel.Width - 40, 80),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var pointLabel = new Label
                {
                    Text = $"{delivery.Address} ({delivery.CustomerName})",
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                pointPanel.Controls.Add(pointLabel);

                var statusHistory = string.Join(" → ", delivery.StatusHistory
                    .OrderBy(s => s.Timestamp)
                    .Select(s => $"{s.Status} ({s.Timestamp:HH:mm})"));

                var historyLabel2 = new Label
                {
                    Text = $"Статусы: {statusHistory}",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(10, 30),
                    AutoSize = true
                };
                pointPanel.Controls.Add(historyLabel2);

                var paymentLabel = new Label
                {
                    Text = $"Оплата: {delivery.Payment} ₽",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(10, 50),
                    AutoSize = true
                };
                pointPanel.Controls.Add(paymentLabel);

                form.routePanel.Controls.Add(pointPanel);
                yPos += 90;
            }

            form.Controls.Add(form.routePanel);
        }

        public void ShowCourierEarnings()
        {
            form.UIHelper.ClearPanels();

            form.earningsPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Мой заработок",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            form.earningsPanel.Controls.Add(title);

            decimal todayEarnings = allDeliveries
                .Where(d => d.StatusHistory.Any(s => s.Timestamp.Date == DateTime.Today))
                .Sum(d => d.Payment);

            decimal weekEarnings = allDeliveries
                .Where(d => d.StatusHistory.Any(s => s.Timestamp.Date >= DateTime.Today.AddDays(-7)))
                .Sum(d => d.Payment);

            decimal monthEarnings = allDeliveries
                .Where(d => d.StatusHistory.Any(s => s.Timestamp.Date >= DateTime.Today.AddMonths(-1)))
                .Sum(d => d.Payment);

            int completedCount = allDeliveries.Count(d => d.Status == "Доставлен" || d.Status == "Отменен");

            var todayLabel = new Label
            {
                Text = $"Сегодня: {todayEarnings} ₽",
                Font = new Font("Segoe UI", 14),
                Location = new Point(20, 60),
                AutoSize = true
            };
            form.earningsPanel.Controls.Add(todayLabel);

            var weekLabel = new Label
            {
                Text = $"За неделю: {weekEarnings} ₽",
                Font = new Font("Segoe UI", 14),
                Location = new Point(20, 100),
                AutoSize = true
            };
            form.earningsPanel.Controls.Add(weekLabel);

            var monthLabel = new Label
            {
                Text = $"За месяц: {monthEarnings} ₽",
                Font = new Font("Segoe UI", 14),
                Location = new Point(20, 140),
                AutoSize = true
            };
            form.earningsPanel.Controls.Add(monthLabel);

            var deliveriesLabel = new Label
            {
                Text = $"Всего доставок: {completedCount}",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 190),
                AutoSize = true
            };
            form.earningsPanel.Controls.Add(deliveriesLabel);

            var chartTitle = new Label
            {
                Text = "График заработка за последние 7 дней:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 230),
                AutoSize = true
            };
            form.earningsPanel.Controls.Add(chartTitle);

            var chartPanel = new Panel
            {
                Location = new Point(20, 260),
                Size = new Size(form.earningsPanel.Width - 60, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var chartPlaceholder = new Label
            {
                Text = "[График заработка]",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                AutoSize = true
            };
            chartPanel.Controls.Add(chartPlaceholder);

            form.earningsPanel.Controls.Add(chartPanel);

            form.Controls.Add(form.earningsPanel);
        }

        public void MarkAsDelivered(string point)
        {
            var delivery = allDeliveries.FirstOrDefault(d => point.Contains(d.Address));
            if (delivery != null)
            {
                delivery.Status = "Доставлен";
                delivery.StatusHistory.Add(new DeliveryStatus("Доставлен", DateTime.Now));
                completedDeliveries.Add(delivery);
                allDeliveries.Remove(delivery);

                MessageBox.Show($"Доставка по адресу {delivery.Address} отмечена как выполненная",
                    "Доставка завершена", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ShowCourierRoute();
            }
        }

        public void ShowDeliveryDetails(int deliveryId)
        {
            var delivery = allDeliveries.FirstOrDefault(d => d.Id == deliveryId) ??
                         completedDeliveries.FirstOrDefault(d => d.Id == deliveryId);

            if (delivery == null)
            {
                MessageBox.Show("Доставка не найдена", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var detailsForm = new Form
            {
                Text = $"Детали доставки #{delivery.Id}",
                Size = new Size(500, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var idLabel = new Label
            {
                Text = $"Доставка #{delivery.Id}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var addressLabel = new Label
            {
                Text = $"Адрес: {delivery.Address}",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 50),
                AutoSize = true
            };

            var customerLabel = new Label
            {
                Text = $"Клиент: {delivery.CustomerName}",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 80),
                AutoSize = true
            };

            var phoneLabel = new Label
            {
                Text = $"Телефон: {delivery.CustomerPhone}",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 110),
                AutoSize = true
            };

            var statusLabel = new Label
            {
                Text = $"Текущий статус: {delivery.Status}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 140),
                AutoSize = true
            };

            var paymentLabel = new Label
            {
                Text = $"Оплата за доставку: {delivery.Payment} ₽",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 170),
                AutoSize = true
            };

            var timeLabel = new Label
            {
                Text = $"Время заказа: {delivery.StatusHistory.First().Timestamp:HH:mm}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 200),
                AutoSize = true
            };

            var itemsLabel = new Label
            {
                Text = "Состав заказа:\n" + string.Join("\n", delivery.OrderItems),
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 230),
                AutoSize = true
            };

            var statusHistoryLabel = new Label
            {
                Text = "История статусов:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 280),
                AutoSize = true
            };

            var statusHistoryList = new ListBox
            {
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 310),
                Size = new Size(440, 100),
                IntegralHeight = false
            };

            foreach (var status in delivery.StatusHistory.OrderBy(s => s.Timestamp))
            {
                statusHistoryList.Items.Add($"{status.Timestamp:HH:mm} - {status.Status}");
            }

            if (delivery.Status != "Доставлен" && delivery.Status != "Отменен")
            {
                var statusPanel = new Panel
                {
                    Location = new Point(20, 420),
                    Size = new Size(440, 40),
                    BorderStyle = BorderStyle.None
                };

                var statusComboBox = new ComboBox
                {
                    Location = new Point(0, 5),
                    Size = new Size(200, 20),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                statusComboBox.Items.AddRange(new[] { "Курьер получил заказ", "Курьер везет заказ", "Заказ отдан", "Заказ отменен" });

                var updateStatusButton = new Button
                {
                    Text = "Обновить статус",
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(120, 25),
                    Location = new Point(210, 5),
                    Font = new Font("Segoe UI", 9)
                };
                updateStatusButton.Click += (s, e) =>
                {
                    if (statusComboBox.SelectedItem != null)
                    {
                        string newStatus = statusComboBox.SelectedItem.ToString();
                        delivery.Status = newStatus;
                        delivery.StatusHistory.Add(new DeliveryStatus(newStatus, DateTime.Now));
                        statusHistoryList.Items.Add($"{DateTime.Now:HH:mm} - {newStatus}");
                        statusLabel.Text = $"Текущий статус: {newStatus}";

                        if (newStatus == "Заказ отдан" || newStatus == "Заказ отменен")
                        {
                            completedDeliveries.Add(delivery);
                            allDeliveries.Remove(delivery);
                        }

                        MessageBox.Show("Статус обновлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                };

                statusPanel.Controls.Add(statusComboBox);
                statusPanel.Controls.Add(updateStatusButton);
                detailsForm.Controls.Add(statusPanel);
            }

            var closeButton = new Button
            {
                Text = "Закрыть",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(360, 420),
                DialogResult = DialogResult.OK
            };
            closeButton.Click += (s, e) => detailsForm.Close();

            detailsForm.Controls.AddRange(new Control[] { idLabel, addressLabel, customerLabel, phoneLabel,
                statusLabel, paymentLabel, timeLabel, itemsLabel, statusHistoryLabel, statusHistoryList, closeButton });

            detailsForm.ShowDialog();
        }

        public void ChangeCourierStatus()
        {
            var statusForm = new Form
            {
                Text = "Изменение статуса",
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
                Size = new Size(250, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusComboBox.Items.AddRange(new[] { "Доступен", "Не доступен", "На доставке", "Перерыв" });

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
                if (statusComboBox.SelectedItem != null)
                {
                    MessageBox.Show($"Статус курьера изменен на: {statusComboBox.SelectedItem}",
                        "Статус обновлен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    statusForm.Close();
                }
            };

            statusForm.Controls.AddRange(new Control[] { statusLabel, statusComboBox, saveButton });
            statusForm.ShowDialog();
        }
    }

    

    
}