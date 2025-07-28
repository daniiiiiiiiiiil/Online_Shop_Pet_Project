using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class UIHelper
    {
        private MainMenuForm form;
        public UIHelper(MainMenuForm form) { this.form = form; }

        public void InitializeUI()
        {
            form.Text = "Online Shop";
            form.Size = new Size(1000, 700);
            form.BackColor = Color.White;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;

            if (form.isEmployee)
            {
                form.EmployeeHelper.ShowEmployeeRoleSelection();
            }
            else
            {
                ShowCustomerMenu();
                form.ProductHelper.ShowProductsPanel();
            }
        }

        public void ClearPanels()
        {
            if (form.productsPanel != null) form.Controls.Remove(form.productsPanel);
            if (form.productDetailsPanel != null) form.Controls.Remove(form.productDetailsPanel);
            if (form.ordersPanel != null) form.Controls.Remove(form.ordersPanel);
            if (form.cartPanel != null) form.Controls.Remove(form.cartPanel);
            if (form.deliveryPanel != null) form.Controls.Remove(form.deliveryPanel);
            if (form.profilePanel != null) form.Controls.Remove(form.profilePanel);
            if (form.deliveriesPanel != null) form.Controls.Remove(form.deliveriesPanel);
            if (form.routePanel != null) form.Controls.Remove(form.routePanel);
            if (form.earningsPanel != null) form.Controls.Remove(form.earningsPanel);
            if (form.cookOrdersPanel != null) form.Controls.Remove(form.cookOrdersPanel);
            if (form.cookMenuPanel != null) form.Controls.Remove(form.cookMenuPanel);
            if (form.ingredientsPanel != null) form.Controls.Remove(form.ingredientsPanel);
            if (form.hallStaffOrdersPanel != null) form.Controls.Remove(form.hallStaffOrdersPanel);
            if (form.storeMapPanel != null) form.Controls.Remove(form.storeMapPanel);
            if (form.hallStaffHistoryPanel != null) form.Controls.Remove(form.hallStaffHistoryPanel);
            if (form.helpPanel != null) form.Controls.Remove(form.helpPanel);
            if (form.ticketsPanel != null) form.Controls.Remove(form.ticketsPanel);
            if (form.historyPanel != null) form.Controls.Remove(form.historyPanel);
            if (form.offlineOrderPanel != null) form.Controls.Remove(form.offlineOrderPanel);
            if (form.returnPanel != null) form.Controls.Remove(form.returnPanel);
            if (form.chatPanel != null) form.Controls.Remove(form.chatPanel);
            if (form.complaintsPanel != null) form.Controls.Remove(form.complaintsPanel);
            if (form.chatSupportPanel != null) form.Controls.Remove(form.chatSupportPanel);
            if (form.knowledgePanel != null) form.Controls.Remove(form.knowledgePanel);
        }

        public Button CreateBottomButton(string text, int index, int width = 0)
        {
            int buttonWidth = width > 0 ? width : form.ClientSize.Width / 4;
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

        public Image LoadImageOrDefault(string path, int width, int height)
        {
            try
            {
                if (File.Exists(path))
                    return Image.FromFile(path);
            }
            catch { }

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

        public Color GetStatusColor(string status)
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

        public Color GetDeliveryStatusColor(string status)
        {
            switch (status)
            {
                case "В пути": return Color.Blue;
                case "Доставлено": return Color.Green;
                case "Ожидает": return Color.Orange;
                case "Отменено": return Color.Red;
                default: return Color.Black;
            }
        }

        public Color GetKitchenOrderStatusColor(string status)
        {
            switch (status)
            {
                case "В ожидании": return Color.Orange;
                case "Готовится": return Color.Blue;
                case "Готово": return Color.Green;
                case "Отменено": return Color.Red;
                default: return Color.Black;
            }
        }

        public Color GetHallOrderStatusColor(string status)
        {
            switch (status)
            {
                case "В обработке": return Color.Orange;
                case "Готов к сборке": return Color.Blue;
                case "Собран": return Color.Green;
                case "Отменен": return Color.Red;
                default: return Color.Black;
            }
        }

        public Color GetTicketStatusColor(string status)
        {
            switch (status)
            {
                case "Новый": return Color.Blue;
                case "В обработке": return Color.Orange;
                case "Решено": return Color.Green;
                case "Отклонено": return Color.Red;
                default: return Color.Black;
            }
        }

        public Color GetComplaintStatusColor(string status)
        {
            switch (status)
            {
                case "Новая": return Color.Red;
                case "В обработке": return Color.Orange;
                case "Решена": return Color.Green;
                case "Отклонена": return Color.Gray;
                default: return Color.Black;
            }
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowCustomerMenu()
        {
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            int buttonCount = 4;
            int buttonWidth = form.ClientSize.Width / buttonCount;

            var productsButton = CreateBottomButton("Товары", 0, buttonWidth);
            productsButton.Dock = DockStyle.Left;
            productsButton.Click += (s, e) => form.ProductHelper.ShowProductsPanel();

            var ordersButton = CreateBottomButton("Заказы", 1, buttonWidth);
            ordersButton.Dock = DockStyle.Left;
            ordersButton.Click += (s, e) => form.OrderHelper.ShowOrdersPanel();

            var cartButton = CreateBottomButton("Корзина", 2, buttonWidth);
            cartButton.Dock = DockStyle.Left;
            cartButton.Click += (s, e) => form.CartHelper.ShowCartPanel();

            var accountButton = CreateBottomButton("Профиль", 3, buttonWidth);
            accountButton.Dock = DockStyle.Left;
            accountButton.Click += (s, e) => form.ProfileHelper.ShowProfilePanel();

            bottomPanel.Controls.Add(accountButton);
            bottomPanel.Controls.Add(cartButton);
            bottomPanel.Controls.Add(ordersButton);
            bottomPanel.Controls.Add(productsButton);

            form.Controls.Add(bottomPanel);
        }
    }
}
