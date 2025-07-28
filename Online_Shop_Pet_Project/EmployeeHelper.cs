using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class EmployeeHelper
    {
        private MainMenuForm form;
        public EmployeeHelper(MainMenuForm form) { this.form = form; }
        public List<Product> products = new List<Product>();

        public void ShowEmployeeRoleSelection()
        {
            var roleGroup = new GroupBox
            {
                Text = "Выберите роль",
                Font = new Font("Segoe UI", 10),
                Location = new Point(form.ClientSize.Width / 2 - 200, form.ClientSize.Height / 2 - 125),
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
                if (directorRadio.Checked) form.userRole = "Директор";
                else if (sellerRadio.Checked) form.userRole = "Продавец";
                else if (courierRadio.Checked) form.userRole = "Курьер";
                else if (cookRadio.Checked) form.userRole = "Повар";
                else if (hallStaffRadio.Checked) form.userRole = "Работник зала";
                else if (supportStaffRadio.Checked) form.userRole = "Техподдержка";

                if (!string.IsNullOrEmpty(form.userRole))
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

            form.Controls.Add(roleGroup);
        }

        public void ShowEmployeeMenu()
        {
            form.UIHelper.ClearPanels();
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            if (form.userRole == "Директор")
            {
                var hireEmployeeButton = form.UIHelper.CreateBottomButton("Принятие сотрудников", 0);
                hireEmployeeButton.Click += (s, e) => ShowHireEmployeeForm();
                bottomPanel.Controls.Add(hireEmployeeButton);

                var receiveGoodsButton = form.UIHelper.CreateBottomButton("Принятие товара", 1);
                receiveGoodsButton.Click += (s, e) => ShowReceiveGoodsForm();
                bottomPanel.Controls.Add(receiveGoodsButton);

                var shiftsButton = form.UIHelper.CreateBottomButton("Смены сотрудников", 2);
                shiftsButton.Click += (s, e) => ShowShiftsManagementForm();
                bottomPanel.Controls.Add(shiftsButton);

                var disposeGoodsButton = form.UIHelper.CreateBottomButton("Утилизация товара", 3);
                disposeGoodsButton.Click += (s, e) => ShowDisposeGoodsForm();
                bottomPanel.Controls.Add(disposeGoodsButton);

                var revenueButton = form.UIHelper.CreateBottomButton("Выручка", 4);
                revenueButton.Click += (s, e) => ShowRevenueOptions();
                bottomPanel.Controls.Add(revenueButton);
            }
            else if (form.userRole == "Продавец")
            {
                var productInfoButton = form.UIHelper.CreateBottomButton("Инфо о товаре", 0);
                productInfoButton.Click += (s, e) => form.ProductHelper.ShowProductsPanel();
                bottomPanel.Controls.Add(productInfoButton);

                var historyButton = form.UIHelper.CreateBottomButton("История покупок", 1);
                historyButton.Click += (s, e) => form.SellerHelper.ShowPurchaseHistory();
                bottomPanel.Controls.Add(historyButton);

                var offlineOrderButton = form.UIHelper.CreateBottomButton("Оффлайн заказ", 2);
                offlineOrderButton.Click += (s, e) => form.SellerHelper.ProcessOfflineOrder();
                bottomPanel.Controls.Add(offlineOrderButton);

                var returnButton = form.UIHelper.CreateBottomButton("Возврат товара", 3);
                returnButton.Click += (s, e) => form.SellerHelper.ProcessProductReturn();
                bottomPanel.Controls.Add(returnButton);
            }
            else if (form.userRole == "Курьер")
            {
                var deliveriesButton = form.UIHelper.CreateBottomButton("Доставки", 0);
                deliveriesButton.Click += (s, e) => form.DeliveryHelper.ShowCourierDeliveries();
                bottomPanel.Controls.Add(deliveriesButton);

                var routeButton = form.UIHelper.CreateBottomButton("Маршрут", 1);
                routeButton.Click += (s, e) => form.DeliveryHelper.ShowCourierRoute();
                bottomPanel.Controls.Add(routeButton);

                var earningsButton = form.UIHelper.CreateBottomButton("Заработок", 2);
                earningsButton.Click += (s, e) => form.DeliveryHelper.ShowCourierEarnings();
                bottomPanel.Controls.Add(earningsButton);

                var statusButton = form.UIHelper.CreateBottomButton("Статус", 3);
                statusButton.Click += (s, e) => form.DeliveryHelper.ChangeCourierStatus();
                bottomPanel.Controls.Add(statusButton);
            }
            else if (form.userRole == "Повар")
            {
                var cookOrdersButton = form.UIHelper.CreateBottomButton("Заказы", 0);
                cookOrdersButton.Click += (s, e) => form.CookHelper.ShowCookOrders();
                bottomPanel.Controls.Add(cookOrdersButton);

                var cookMenuButton = form.UIHelper.CreateBottomButton("Меню", 1);
                cookMenuButton.Click += (s, e) => form.CookHelper.ShowCookMenu();
                bottomPanel.Controls.Add(cookMenuButton);

                var cookStatusButton = form.UIHelper.CreateBottomButton("Статус", 2);
                cookStatusButton.Click += (s, e) => form.CookHelper.ChangeCookStatus();
                bottomPanel.Controls.Add(cookStatusButton);

                var cookIngredientsButton = form.UIHelper.CreateBottomButton("Ингредиенты", 3);
                cookIngredientsButton.Click += (s, e) => form.CookHelper.ShowIngredients();
                bottomPanel.Controls.Add(cookIngredientsButton);
            }
            else if (form.userRole == "Работник зала")
            {
                var ordersButton = form.UIHelper.CreateBottomButton("Заказы", 0);
                ordersButton.Click += (s, e) => form.HallStaffHelper.ShowHallStaffOrdersPanel();
                bottomPanel.Controls.Add(ordersButton);

                var mapButton = form.UIHelper.CreateBottomButton("Карта магазина", 1);
                mapButton.Click += (s, e) => form.HallStaffHelper.ShowStoreMap();
                bottomPanel.Controls.Add(mapButton);

                var profileButton = form.UIHelper.CreateBottomButton("Профиль", 2);
                profileButton.Click += (s, e) => form.ProfileHelper.ShowProfilePanel();
                bottomPanel.Controls.Add(profileButton);

                var historyButton = form.UIHelper.CreateBottomButton("История", 3);
                historyButton.Click += (s, e) => form.HallStaffHelper.ShowHallStaffHistory();
                bottomPanel.Controls.Add(historyButton);
            }
            else if (form.userRole == "Техподдержка")
            {
                var complaintsButton = form.UIHelper.CreateBottomButton("Жалобы", 0);
                complaintsButton.Click += (s, e) => form.SupportHelper.ShowComplaintsPanel();
                bottomPanel.Controls.Add(complaintsButton);

                var chatButton = form.UIHelper.CreateBottomButton("Чат с клиентом", 1);
                chatButton.Click += (s, e) => form.SupportHelper.ShowSupportChatPanel();
                bottomPanel.Controls.Add(chatButton);

                var faqButton = form.UIHelper.CreateBottomButton("База знаний", 2);
                faqButton.Click += (s, e) => form.SupportHelper.ShowKnowledgeBase();
                bottomPanel.Controls.Add(faqButton);
            }
            else
            {
                var profileButton = form.UIHelper.CreateBottomButton("Профиль", 0);
                profileButton.Click += (s, e) => form.UIHelper.ShowMessage("Переход в профиль");

                var ordersButton = form.UIHelper.CreateBottomButton("Заказы", 1);
                ordersButton.Click += (s, e) => form.UIHelper.ShowMessage("Переход к заказам");

                bottomPanel.Controls.Add(profileButton);
                bottomPanel.Controls.Add(ordersButton);
            }

            form.Controls.Add(bottomPanel);
        }

        public void ShowHireEmployeeForm()
        {
            var form = new Form
            {
                Text = "Принятие нового сотрудника",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var nameLabel = new Label { Text = "ФИО:", Location = new Point(20, 20), AutoSize = true };
            var nameBox = new TextBox { Location = new Point(120, 20), Size = new Size(300, 20) };

            var positionLabel = new Label { Text = "Должность:", Location = new Point(20, 60), AutoSize = true };
            var positionBox = new ComboBox { Location = new Point(120, 60), Size = new Size(300, 20) };
            positionBox.Items.AddRange(new[] { "Продавец", "Курьер", "Повар", "Работник зала", "Техподдержка" });

            var salaryLabel = new Label { Text = "Зарплата:", Location = new Point(20, 100), AutoSize = true };
            var salaryBox = new NumericUpDown { Location = new Point(120, 100), Size = new Size(100, 20), Minimum = 10000, Maximum = 1000000 };

            var startDateLabel = new Label { Text = "Дата приема:", Location = new Point(20, 140), AutoSize = true };
            var startDatePicker = new DateTimePicker { Location = new Point(120, 140), Size = new Size(150, 20) };

            var saveButton = new Button
            {
                Text = "Сохранить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(150, 200),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) => form.Close();

            form.Controls.AddRange(new Control[] { nameLabel, nameBox, positionLabel, positionBox,
                                salaryLabel, salaryBox, startDateLabel, startDatePicker,
                                saveButton });

            form.ShowDialog();
        }

        public void ShowShiftsManagementForm()
        {
            var form = new Form
            {
                Text = "Управление сменами",
                Size = new Size(600, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var employeesList = new ListBox
            {
                Location = new Point(20, 20),
                Size = new Size(200, 300),
                SelectionMode = SelectionMode.MultiSimple
            };
            employeesList.Items.AddRange(new[] { "Иванов И.И.", "Петров П.П.", "Сидоров С.С.", "Кузнецова А.А." });

            var dateLabel = new Label { Text = "Дата смены:", Location = new Point(250, 20), AutoSize = true };
            var datePicker = new DateTimePicker { Location = new Point(350, 20), Size = new Size(150, 20) };

            var shiftTypeLabel = new Label { Text = "Тип смены:", Location = new Point(250, 60), AutoSize = true };
            var shiftTypeBox = new ComboBox { Location = new Point(350, 60), Size = new Size(150, 20) };
            shiftTypeBox.Items.AddRange(new[] { "Утро (8:00-16:00)", "День (12:00-20:00)", "Вечер (16:00-24:00)" });

            var saveButton = new Button
            {
                Text = "Назначить смены",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(350, 100),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) => form.Close();

            form.Controls.AddRange(new Control[] { employeesList, dateLabel, datePicker,
                                shiftTypeLabel, shiftTypeBox, saveButton });

            form.ShowDialog();
        }

        public void ShowReceiveGoodsForm()
        {
            var form = new Form
            {
                Text = "Прием товара",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var productLabel = new Label { Text = "Товар:", Location = new Point(20, 20), AutoSize = true };
            var productBox = new ComboBox { Location = new Point(120, 20), Size = new Size(300, 20) };
            productBox.Items.AddRange(products.Select(p => p.Name).ToArray());

            var quantityLabel = new Label { Text = "Количество:", Location = new Point(20, 60), AutoSize = true };
            var quantityBox = new NumericUpDown { Location = new Point(120, 60), Size = new Size(100, 20), Minimum = 1, Maximum = 1000 };

            var supplierLabel = new Label { Text = "Поставщик:", Location = new Point(20, 100), AutoSize = true };
            var supplierBox = new TextBox { Location = new Point(120, 100), Size = new Size(300, 20) };

            var dateLabel = new Label { Text = "Дата приема:", Location = new Point(20, 140), AutoSize = true };
            var datePicker = new DateTimePicker { Location = new Point(120, 140), Size = new Size(150, 20) };

            var saveButton = new Button
            {
                Text = "Подтвердить прием",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 30),
                Location = new Point(150, 200),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) => form.Close();

            form.Controls.AddRange(new Control[] { productLabel, productBox, quantityLabel, quantityBox,
                                supplierLabel, supplierBox, dateLabel, datePicker,
                                saveButton });

            form.ShowDialog();
        }

        public void ShowDisposeGoodsForm()
        {
            var form = new Form
            {
                Text = "Утилизация товара",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var productLabel = new Label { Text = "Товар:", Location = new Point(20, 20), AutoSize = true };
            var productBox = new ComboBox { Location = new Point(120, 20), Size = new Size(300, 20) };
            productBox.Items.AddRange(products.Select(p => p.Name).ToArray());

            var quantityLabel = new Label { Text = "Количество:", Location = new Point(20, 60), AutoSize = true };
            var quantityBox = new NumericUpDown { Location = new Point(120, 60), Size = new Size(100, 20), Minimum = 1, Maximum = 1000 };

            var reasonLabel = new Label { Text = "Причина утилизации:", Location = new Point(20, 100), AutoSize = true };
            var reasonBox = new ComboBox { Location = new Point(120, 100), Size = new Size(300, 20) };
            reasonBox.Items.AddRange(new[] { "Истек срок годности", "Повреждение", "Другая причина" });

            var dateLabel = new Label { Text = "Дата утилизации:", Location = new Point(20, 140), AutoSize = true };
            var datePicker = new DateTimePicker { Location = new Point(120, 140), Size = new Size(150, 20) };

            var saveButton = new Button
            {
                Text = "Подтвердить утилизацию",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 30),
                Location = new Point(150, 200),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) => form.Close();

            form.Controls.AddRange(new Control[] { productLabel, productBox, quantityLabel, quantityBox,
                                reasonLabel, reasonBox, dateLabel, datePicker,
                                saveButton });

            form.ShowDialog();
        }

        public void ShowRevenueOptions()
        {
            form.UIHelper.ClearPanels();

            var revenuePanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
                AutoScroll = true,
                BackColor = Color.White
            };

            var title = new Label
            {
                Text = "Просмотр выручки",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            revenuePanel.Controls.Add(title);

            var dayButton = new Button
            {
                Text = "За день",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(20, 70),
                Font = new Font("Segoe UI", 10)
            };
            dayButton.Click += (s, e) => ShowRevenueReport("day");
            revenuePanel.Controls.Add(dayButton);

            var weekButton = new Button
            {
                Text = "За неделю",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(190, 70),
                Font = new Font("Segoe UI", 10)
            };
            weekButton.Click += (s, e) => ShowRevenueReport("week");
            revenuePanel.Controls.Add(weekButton);

            var monthButton = new Button
            {
                Text = "За месяц",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(360, 70),
                Font = new Font("Segoe UI", 10)
            };
            monthButton.Click += (s, e) => ShowRevenueReport("month");
            revenuePanel.Controls.Add(monthButton);

            var yearButton = new Button
            {
                Text = "За год",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(530, 70),
                Font = new Font("Segoe UI", 10)
            };
            yearButton.Click += (s, e) => ShowRevenueReport("year");
            revenuePanel.Controls.Add(yearButton);

            var specificDateLabel = new Label
            {
                Text = "Конкретная дата:",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 140),
                AutoSize = true
            };
            revenuePanel.Controls.Add(specificDateLabel);

            var datePicker = new DateTimePicker
            {
                Location = new Point(20, 170),
                Size = new Size(150, 20),
                Format = DateTimePickerFormat.Short
            };
            revenuePanel.Controls.Add(datePicker);

            var specificDateButton = new Button
            {
                Text = "Показать выручку",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 30),
                Location = new Point(190, 170),
                Font = new Font("Segoe UI", 10)
            };
            specificDateButton.Click += (s, e) => ShowRevenueReport("specific", datePicker.Value);
            revenuePanel.Controls.Add(specificDateButton);

            var periodLabel = new Label
            {
                Text = "Период:",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 220),
                AutoSize = true
            };
            revenuePanel.Controls.Add(periodLabel);

            var fromDateLabel = new Label
            {
                Text = "С:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 250),
                AutoSize = true
            };
            revenuePanel.Controls.Add(fromDateLabel);

            var fromDatePicker = new DateTimePicker
            {
                Location = new Point(50, 250),
                Size = new Size(150, 20),
                Format = DateTimePickerFormat.Short
            };
            revenuePanel.Controls.Add(fromDatePicker);

            var toDateLabel = new Label
            {
                Text = "По:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(220, 250),
                AutoSize = true
            };
            revenuePanel.Controls.Add(toDateLabel);

            var toDatePicker = new DateTimePicker
            {
                Location = new Point(260, 250),
                Size = new Size(150, 20),
                Format = DateTimePickerFormat.Short
            };
            revenuePanel.Controls.Add(toDatePicker);

            var periodButton = new Button
            {
                Text = "Показать выручку за период",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(220, 30),
                Location = new Point(20, 290),
                Font = new Font("Segoe UI", 10)
            };
            periodButton.Click += (s, e) => ShowRevenueReport("period", fromDatePicker.Value, toDatePicker.Value);
            revenuePanel.Controls.Add(periodButton);

            form.Controls.Add(revenuePanel);
        }

        public void ShowRevenueReport(string period, DateTime? fromDate = null, DateTime? toDate = null)
        {
            decimal revenue = 0;
            string periodText = "";

            switch (period)
            {
                case "day":
                    revenue = 125000;
                    periodText = "за день";
                    break;
                case "week":
                    revenue = 850000;
                    periodText = "за неделю";
                    break;
                case "month":
                    revenue = 3500000;
                    periodText = "за месяц";
                    break;
                case "year":
                    revenue = 42000000;
                    periodText = "за год";
                    break;
                case "specific":
                    revenue = new Random().Next(100000, 200000);
                    periodText = $"за {fromDate.Value.ToShortDateString()}";
                    break;
                case "period":
                    revenue = new Random().Next(500000, 1000000);
                    periodText = $"с {fromDate.Value.ToShortDateString()} по {toDate.Value.ToShortDateString()}";
                    break;
            }

            MessageBox.Show($"Выручка {periodText}: {revenue} ₽", "Финансовый отчет", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

}
