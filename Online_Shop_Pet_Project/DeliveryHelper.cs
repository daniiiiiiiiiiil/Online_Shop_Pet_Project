using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class DeliveryHelper
    {
        private MainMenuForm form;
        public UIHelper uIHelper;
        public DeliveryHelper(MainMenuForm form) { this.form = form; }

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

            var deliveries = new List<Delivery>
            {
                new Delivery { Id = 1001, Address = "ул. Ленина, д. 10, кв. 5", CustomerName = "Иванов Иван", Status = "В пути", Payment = 500 },
                new Delivery { Id = 1002, Address = "пр. Мира, д. 25, кв. 12", CustomerName = "Петрова Анна", Status = "Ожидает", Payment = 450 }
            };

            int yPos = 60;
            foreach (var delivery in deliveries)
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
                    ForeColor = form.UIHelper.GetDeliveryStatusColor(delivery.Status),
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

            var routePoints = new List<string>
            {
                "1. ул. Ленина, д. 10, кв. 5 (Иванов Иван)",
                "2. пр. Мира, д. 25, кв. 12 (Петрова Анна)",
                "3. ул. Гагарина, д. 7, кв. 33 (Сидоров Петр)"
            };

            int yPos = 60;
            foreach (var point in routePoints)
            {
                var pointPanel = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(form.routePanel.Width - 40, 50),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var pointLabel = new Label
                {
                    Text = point,
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(10, 15),
                    AutoSize = true
                };
                pointPanel.Controls.Add(pointLabel);

                var deliveredButton = new Button
                {
                    Text = "Доставлено",
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(100, 25),
                    Location = new Point(pointPanel.Width - 120, 12),
                    Font = new Font("Segoe UI", 9)
                };
                deliveredButton.Click += (s, e) => MarkAsDelivered(point);
                pointPanel.Controls.Add(deliveredButton);

                form.routePanel.Controls.Add(pointPanel);
                yPos += 60;
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

            var todayEarnings = new Label
            {
                Text = "Сегодня: 950 ₽",
                Font = new Font("Segoe UI", 14),
                Location = new Point(20, 60),
                AutoSize = true
            };
            form.earningsPanel.Controls.Add(todayEarnings);

            var weekEarnings = new Label
            {
                Text = "За неделю: 6,500 ₽",
                Font = new Font("Segoe UI", 14),
                Location = new Point(20, 100),
                AutoSize = true
            };
            form.earningsPanel.Controls.Add(weekEarnings);

            var monthEarnings = new Label
            {
                Text = "За месяц: 28,750 ₽",
                Font = new Font("Segoe UI", 14),
                Location = new Point(20, 140),
                AutoSize = true
            };
            form.earningsPanel.Controls.Add(monthEarnings);

            var deliveriesCount = new Label
            {
                Text = "Всего доставок: 125",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 190),
                AutoSize = true
            };
            form.earningsPanel.Controls.Add(deliveriesCount);

            form.Controls.Add(form.earningsPanel);
        }

        public void MarkAsDelivered(string point)
        {
            MessageBox.Show($"Доставка по адресу {point} отмечена как выполненная", "Доставка завершена", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowDeliveryDetails(int deliveryId)
        {
            var delivery = new Delivery
            {
                Id = deliveryId,
                Address = "ул. Ленина, д. 10, кв. 5",
                CustomerName = "Иванов Иван",
                CustomerPhone = "+7 (123) 456-78-90",
                Status = "В пути",
                Payment = 500,
                OrderTime = DateTime.Now.AddHours(-1),
                EstimatedDeliveryTime = DateTime.Now.AddHours(1),
                OrderItems = new List<string> { "Пицца Маргарита", "Кока-кола 1л" }
            };

            var form = new Form
            {
                Text = $"Детали доставки #{delivery.Id}",
                Size = new Size(500, 400),
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
                Text = $"Статус: {delivery.Status}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = uIHelper.GetDeliveryStatusColor(delivery.Status),
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
                Text = $"Время заказа: {delivery.OrderTime:HH:mm}\nОриентировочное время доставки: {delivery.EstimatedDeliveryTime:HH:mm}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 200),
                AutoSize = true
            };

            var itemsLabel = new Label
            {
                Text = "Состав заказа:\n" + string.Join("\n", delivery.OrderItems),
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 250),
                AutoSize = true
            };

            var completeButton = new Button
            {
                Text = "Завершить доставку",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(300, 320),
                DialogResult = DialogResult.OK
            };
            completeButton.Click += (s, e) => form.Close();

            form.Controls.AddRange(new Control[] { idLabel, addressLabel, customerLabel, phoneLabel,
                            statusLabel, paymentLabel, timeLabel, itemsLabel, completeButton });
            form.ShowDialog();
        }

        public void ChangeCourierStatus()
        {
            var form = new Form
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
                Size = new Size(250, 20)
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
            saveButton.Click += (s, e) => form.Close();

            form.Controls.AddRange(new Control[] { statusLabel, statusComboBox, saveButton });
            form.ShowDialog();
        }
    }
}
