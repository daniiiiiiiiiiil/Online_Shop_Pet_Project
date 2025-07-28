using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public class ProfileHelper
    {
        private MainMenuForm form;
        public ProfileHelper(MainMenuForm form) { this.form = form; }

        public void ShowProfilePanel()
        {
            form.UIHelper.ClearPanels();

            form.profilePanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 60),
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
            form.profilePanel.Controls.Add(title);

            var profileContainer = new Panel
            {
                Location = new Point(20, 40),
                Size = new Size(form.ClientSize.Width - 40, form.ClientSize.Height - 120),
                BackColor = Color.White
            };

            var topPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(profileContainer.Width, 150),
                BackColor = Color.White
            };

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
                Image = form.UIHelper.LoadImageOrDefault(form.userProfile.PhotoPath, 110, 110)
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

            var infoPanel = new Panel
            {
                Location = new Point(130, 0),
                Size = new Size(profileContainer.Width - 130, 150),
                BackColor = Color.White
            };

            var personalInfoLabel = new Label
            {
                Text = "Личная информация",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(0, 0)
            };
            infoPanel.Controls.Add(personalInfoLabel);

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
                Text = form.userProfile.Name,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 30),
                BorderStyle = BorderStyle.FixedSingle
            };
            infoPanel.Controls.Add(nameValue);

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
                Text = form.userProfile.Phone,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 60),
                BorderStyle = BorderStyle.FixedSingle
            };
            infoPanel.Controls.Add(phoneValue);

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
                Text = form.userProfile.Email,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 90),
                BorderStyle = BorderStyle.FixedSingle
            };
            infoPanel.Controls.Add(emailValue);

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
                Text = form.userProfile.Password,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Size = new Size(200, 20),
                Location = new Point(80, 120),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '*'
            };
            infoPanel.Controls.Add(passwordValue);

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
                form.userProfile.Name = nameValue.Text;
                form.userProfile.Phone = phoneValue.Text;
                form.userProfile.Email = emailValue.Text;
                form.userProfile.Password = passwordValue.Text;
                MessageBox.Show("Изменения сохранены!", "Профиль", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            infoPanel.Controls.Add(saveButton);

            topPanel.Controls.Add(infoPanel);
            profileContainer.Controls.Add(topPanel);

            var sectionsPanel = new Panel
            {
                Location = new Point(0, 160),
                Size = new Size(profileContainer.Width, profileContainer.Height - 160),
                BackColor = Color.White
            };

            int buttonWidth = (sectionsPanel.Width - 60) / 3;
            int buttonHeight = 60;

            var supportQuestionsButton = new Button
            {
                Text = "Часто задаваемые вопросы",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 9)
            };
            supportQuestionsButton.Click += (s, e) => form.SupportHelper.ShowSupportHelpPanel();
            sectionsPanel.Controls.Add(supportQuestionsButton);

            var supportButton = new Button
            {
                Text = "Подать заявку в тех поддержку",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(1250, 90),
                Font = new Font("Segoe UI", 9)
            };
            supportButton.Click += (s, e) => form.SupportHelper.ShowNewTicketForm();
            sectionsPanel.Controls.Add(supportButton);

            var answerSupport = new Button
            {
                Text = "Ответ на заявку",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(20, 165),
                Font = new Font("Segoe UI", 9)
            };
            answerSupport.Click += (s, e) => form.SupportHelper.ShowTicketDetails(1);
            sectionsPanel.Controls.Add(answerSupport);

            var ChatSupport = new Button
            {
                Text = "Чат с тех поддержкой",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(30 + buttonWidth, 165),
                Font = new Font("Segoe UI", 9)
            };
            ChatSupport.Click += (s, e) => form.SupportHelper.ShowChatWithSupport();
            sectionsPanel.Controls.Add(ChatSupport);

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
            ordersButton.Click += (s, e) => form.OrderHelper.ShowOrdersPanel();
            sectionsPanel.Controls.Add(ordersButton);

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
            personalInfoButton.Click += (s, e) => form.UIHelper.ShowMessage("Раздел личной информации");
            sectionsPanel.Controls.Add(personalInfoButton);

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
            addressButton.Click += (s, e) => form.UIHelper.ShowMessage("Раздел адреса");
            sectionsPanel.Controls.Add(addressButton);

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
            paymentButton.Click += (s, e) => form.UIHelper.ShowMessage("Раздел способов оплаты");
            sectionsPanel.Controls.Add(paymentButton);

            profileContainer.Controls.Add(sectionsPanel);
            form.profilePanel.Controls.Add(profileContainer);
            form.Controls.Add(form.profilePanel);
        }

        public void ChangeProfilePhoto()
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
                    form.userProfile.PhotoPath = openFileDialog.FileName;
                    ShowProfilePanel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
