using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public partial class LoginForm : Form
    {
        private TextBox phoneEmailTextBox;
        private TextBox passwordTextBox;

        public LoginForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Настройки формы
            this.Text = "Вход в Online Shop";
            this.Size = new Size(500, 500);
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
                Text = "Вход в систему",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(180, 180)
            };

            // Поле для ввода email/телефона
            var phoneEmailLabel = new Label
            {
                Text = "Номер телефона/email:",
                Location = new Point(50, 230),
                Font = new Font("Segoe UI", 10)
            };

            phoneEmailTextBox = new TextBox
            {
                Location = new Point(50, 255),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 12)
            };

            // Поле для ввода пароля
            var passwordLabel = new Label
            {
                Text = "Пароль:",
                Location = new Point(50, 295),
                Font = new Font("Segoe UI", 10)
            };

            passwordTextBox = new TextBox
            {
                Location = new Point(50, 320),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 12),
                PasswordChar = '*'
            };

            // Кнопка входа
            var loginButton = new Button
            {
                Text = "Войти",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(400, 40),
                Location = new Point(50, 370),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Click += (s, e) => ProcessLogin();

            // Ссылка на регистрацию
            var registerLink = new LinkLabel
            {
                Text = "Нет аккаунта? Зарегистрироваться",
                AutoSize = true,
                Location = new Point(150, 420),
                Font = new Font("Segoe UI", 10)
            };
            registerLink.LinkClicked += (s, e) => ShowRegistrationForm();

            // Добавление элементов на форму
            this.Controls.Add(logo);
            this.Controls.Add(title);
            this.Controls.Add(phoneEmailLabel);
            this.Controls.Add(phoneEmailTextBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextBox);
            this.Controls.Add(loginButton);
            this.Controls.Add(registerLink);
        }

        private Image LoadImageOrDefault(string path, int width, int height)
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
                g.DrawString("Лого", new Font("Arial", 12), Brushes.Black, 10, 10);
            }
            return bmp;
        }

        private void ProcessLogin()
        {
            // Проверяем введенные данные
            string login = phoneEmailTextBox.Text;
            string password = passwordTextBox.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }

            // Проверяем, является ли пользователь сотрудником
            bool isEmployee = CheckIfUserIsEmployee(login, password);

            // Открываем главное меню с соответствующими правами
            var mainMenu = new MainMenuForm(isEmployee);
            mainMenu.Show();
            this.Hide();
        }

        private bool CheckIfUserIsEmployee(string login, string password)
        {
            // В реальном приложении здесь должна быть проверка в базе данных
            // Для демонстрации используем простую логику:

            // Пример: если вводит "admin" и "admin" - считаем сотрудником
            if (login == "admin" && password == "admin")
                return true;

            // Пример: если вводит "employee" и "123" - считаем сотрудником
            if (login == "employee" && password == "123")
                return true;

            // Все остальные - покупатели
            return false;
        }

        private void ShowRegistrationForm()
        {
            using (var registrationForm = new RegistrationForm())
            {
                if (registrationForm.ShowDialog() == DialogResult.OK)
                {
                    // После регистрации сразу входим под новым пользователем
                    // В реальном приложении нужно запросить пароль для входа
                    bool isEmployee = registrationForm.IsEmployee;
                    var mainMenu = new MainMenuForm(isEmployee);
                    mainMenu.Show();
                    this.Hide();
                }
            }
        }
    }
}