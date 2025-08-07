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
        private DirectorHelper directorHelper;
        public EmployeeHelper(MainMenuForm form)
        {
            this.form = form;
            this.directorHelper = new DirectorHelper(form); 
        }
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
                hireEmployeeButton.Click += (s, e) => directorHelper.ShowHireEmployeeForm();
                bottomPanel.Controls.Add(hireEmployeeButton);

                var receiveGoodsButton = form.UIHelper.CreateBottomButton("Принятие товара", 1);
                receiveGoodsButton.Click += (s, e) => directorHelper.ShowReceiveGoodsForm();
                bottomPanel.Controls.Add(receiveGoodsButton);

                var shiftsButton = form.UIHelper.CreateBottomButton("Смены сотрудников", 2);
                shiftsButton.Click += (s, e) => directorHelper.ShowShiftsManagementForm();
                bottomPanel.Controls.Add(shiftsButton);

                var disposeGoodsButton = form.UIHelper.CreateBottomButton("Утилизация товара", 3);
                disposeGoodsButton.Click += (s, e) => directorHelper.ShowDisposeGoodsForm();
                bottomPanel.Controls.Add(disposeGoodsButton);

                var revenueButton = form.UIHelper.CreateBottomButton("Выручка", 4);
                revenueButton.Click += (s, e) => directorHelper.ShowRevenueOptions();
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

    }

}
