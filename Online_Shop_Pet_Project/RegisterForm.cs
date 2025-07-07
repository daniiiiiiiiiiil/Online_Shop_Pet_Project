using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public partial class RegistrationForm : Form
    {
        public bool IsEmployee { get; private set; } = false;
        private bool isEmployeeRegistration = false;

        public RegistrationForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Настройки формы
            this.Text = "Регистрация в Online Shop";
            this.Size = new Size(500, 700);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Логотип
            var logo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(150, 150),
                Location = new Point(175, 20),
                Image = LoadImageOrDefault("art/logo.png", 150, 150)
            };

            // Заголовок
            var title = new Label
            {
                Text = "Подтверждение директории",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(150, 180)
            };

            // Группа выбора типа регистрации
            var registrationTypeGroup = new GroupBox
            {
                Text = "Выбор работы",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 230),
                Size = new Size(400, 100)
            };

            var employeeRadio = new RadioButton
            {
                Text = "Сотрудник",
                Location = new Point(20, 30),
                Checked = false,
                Font = new Font("Segoe UI", 10)
            };
            employeeRadio.CheckedChanged += (s, e) =>
            {
                isEmployeeRegistration = employeeRadio.Checked;
                ToggleRegistrationFields();
            };

            var customerRadio = new RadioButton
            {
                Text = "Покупатель",
                Location = new Point(20, 60),
                Checked = true,
                Font = new Font("Segoe UI", 10)
            };

            registrationTypeGroup.Controls.Add(employeeRadio);
            registrationTypeGroup.Controls.Add(customerRadio);

            // Основные поля регистрации
            var phoneEmailLabel = new Label
            {
                Text = "Номер телефона/email:",
                Location = new Point(50, 350),
                Font = new Font("Segoe UI", 10)
            };

            var phoneEmailTextBox = new TextBox
            {
                Location = new Point(50, 375),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 12)
            };

            var passwordLabel = new Label
            {
                Text = "Пароль:",
                Location = new Point(50, 415),
                Font = new Font("Segoe UI", 10)
            };

            var passwordTextBox = new TextBox
            {
                Location = new Point(50, 440),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 12),
                PasswordChar = '*'
            };

            // Поля только для сотрудников (изначально скрыты)
            var documentsLabel = new Label
            {
                Text = "Документы для присутствия:",
                Location = new Point(50, 480),
                Font = new Font("Segoe UI", 10),
                Visible = false
            };

            var documentsButton = new Button
            {
                Text = "Загрузить документы",
                Location = new Point(50, 505),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 9),
                Visible = false
            };

            var photoLabel = new Label
            {
                Text = "Фото:",
                Location = new Point(250, 480),
                Font = new Font("Segoe UI", 10),
                Visible = false
            };

            var photoButton = new Button
            {
                Text = "Загрузить фото",
                Location = new Point(250, 505),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 9),
                Visible = false
            };

            // Поле фото для покупателей (по желанию)
            var customerPhotoLabel = new Label
            {
                Text = "Фото (по желанию):",
                Location = new Point(50, 480),
                Font = new Font("Segoe UI", 10)
            };

            var customerPhotoButton = new Button
            {
                Text = "Загрузить фото",
                Location = new Point(50, 505),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 9)
            };

            // Кнопка регистрации
            var registerButton = new Button
            {
                Text = "Регистрация",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(400, 40),
                Location = new Point(50, 550),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            registerButton.FlatAppearance.BorderSize = 0;
            registerButton.Click += (s, e) => ProcessRegistration();

            // Добавление элементов на форму
            this.Controls.Add(logo);
            this.Controls.Add(title);
            this.Controls.Add(registrationTypeGroup);
            this.Controls.Add(phoneEmailLabel);
            this.Controls.Add(phoneEmailTextBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextBox);
            this.Controls.Add(documentsLabel);
            this.Controls.Add(documentsButton);
            this.Controls.Add(photoLabel);
            this.Controls.Add(photoButton);
            this.Controls.Add(customerPhotoLabel);
            this.Controls.Add(customerPhotoButton);
            this.Controls.Add(registerButton);

            // Сохраняем ссылки на элементы для управления видимостью
            employeeDocumentsLabel = documentsLabel;
            employeeDocumentsButton = documentsButton;
            employeePhotoLabel = photoLabel;
            employeePhotoButton = photoButton;
            customerOptionalPhotoLabel = customerPhotoLabel;
            customerOptionalPhotoButton = customerPhotoButton;
        }

        private Label employeeDocumentsLabel;
        private Button employeeDocumentsButton;
        private Label employeePhotoLabel;
        private Button employeePhotoButton;
        private Label customerOptionalPhotoLabel;
        private Button customerOptionalPhotoButton;

        private void ToggleRegistrationFields()
        {
            employeeDocumentsLabel.Visible = isEmployeeRegistration;
            employeeDocumentsButton.Visible = isEmployeeRegistration;
            employeePhotoLabel.Visible = isEmployeeRegistration;
            employeePhotoButton.Visible = isEmployeeRegistration;

            customerOptionalPhotoLabel.Visible = !isEmployeeRegistration;
            customerOptionalPhotoButton.Visible = !isEmployeeRegistration;
        }

        private Image LoadImageOrDefault(string path, int width, int height)
        {
            try
            {
                if (File.Exists(path))
                    return Image.FromFile(path);
            }
            catch { }

            // Создаем placeholder, если изображение не найдено
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                g.DrawString("Лого", new Font("Arial", 12), Brushes.Black, 10, 10);
            }
            return bmp;
        }

        private void ProcessRegistration()
        {
            IsEmployee = isEmployeeRegistration;
            MessageBox.Show(IsEmployee ?
                "Регистрация сотрудника выполнена" :
                "Регистрация покупателя выполнена");

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}