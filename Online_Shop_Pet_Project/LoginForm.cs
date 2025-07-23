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
        private CheckBox termsCheckBox;
        private Color primaryColor = Color.FromArgb(70, 130, 180);

        public LoginForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Вход в Online Shop";
            this.Size = new Size(500, 550); // Увеличили высоту для новых элементов
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var logo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(150, 150),
                Location = new Point(175, 20),
                Image = LoadImageOrDefault("E:\\с#\\Online_Shop_Pet_Project\\Online_Shop_Pet_Project\\art\\logo.png", 150, 150)
            };

            var title = new Label
            {
                Text = "Вход в систему",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = primaryColor,
                AutoSize = true,
                Location = new Point(180, 180)
            };

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

            // CheckBox для согласия с условиями
            termsCheckBox = new CheckBox
            {
                Text = "Я согласен с условиями использования магазина",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(50, 370),
                Checked = false
            };

            // Ссылка на условия использования
            var termsLink = new LinkLabel
            {
                Text = "ознакомиться",
                Font = new Font("Segoe UI", 10),
                LinkColor = primaryColor,
                ActiveLinkColor = Color.FromArgb(0, 100, 200),
                AutoSize = true,
                Location = new Point(50, 400),
                Cursor = Cursors.Hand
            };
            termsLink.LinkClicked += (s, e) => ShowTermsDialog();

            var loginButton = new Button
            {
                Text = "Войти",
                BackColor = primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(400, 40),
                Location = new Point(50, 440),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Click += (s, e) => ProcessLogin();

            var registerLink = new LinkLabel
            {
                Text = "Нет аккаунта? Зарегистрироваться",
                AutoSize = true,
                Location = new Point(150, 490),
                Font = new Font("Segoe UI", 10)
            };
            registerLink.LinkClicked += (s, e) => ShowRegistrationForm();

            this.Controls.Add(logo);
            this.Controls.Add(title);
            this.Controls.Add(phoneEmailLabel);
            this.Controls.Add(phoneEmailTextBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextBox);
            this.Controls.Add(termsCheckBox);
            this.Controls.Add(termsLink);
            this.Controls.Add(loginButton);
            this.Controls.Add(registerLink);
        }

        private void ShowTermsDialog()
        {
            var termsForm = new Form
            {
                Text = "Условия использования",
                Size = new Size(600, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                BackColor = Color.White
            };

            var textBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Text = "Здесь будут условия использования сервиса...\n\n1. Пользовательское соглашение\n2. Политика конфиденциальности\n3. Условия возврата",
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Margin = new Padding(20)
            };

            var closeButton = new Button
            {
                Text = "Закрыть",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => termsForm.Close();

            termsForm.Controls.Add(textBox);
            termsForm.Controls.Add(closeButton);
            termsForm.ShowDialog(this);
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
            string login = phoneEmailTextBox.Text;
            string password = passwordTextBox.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }

            if (!termsCheckBox.Checked)
            {
                MessageBox.Show("Для входа необходимо согласиться с условиями использования магазина",
                              "Требуется подтверждение",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            bool isEmployee = CheckIfUserIsEmployee(login, password);

            var mainMenu = new MainMenuForm(isEmployee);
            mainMenu.Show();
            this.Hide();
        }

        private bool CheckIfUserIsEmployee(string login, string password)
        {
            if (login == "admin" && password == "admin")
                return true;

            if (login == "employee" && password == "123")
                return true;

            return false;
        }

        private void ShowRegistrationForm()
        {
            using (var registrationForm = new RegistrationForm())
            {
                if (registrationForm.ShowDialog() == DialogResult.OK)
                {
                    bool isEmployee = registrationForm.IsEmployee;
                    var mainMenu = new MainMenuForm(isEmployee);
                    mainMenu.Show();
                    this.Hide();
                }
            }
        }
    }
}