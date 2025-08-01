using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public partial class RegistrationForm : Form
    {
        private TextBox contactTextBox;
        private TextBox passwordTextBox;

        public bool IsEmployee { get; private set; } = false;
        private bool isEmployeeRegistration = false;
        private Color primaryColor = Color.FromArgb(30, 144, 255);

        // Поля для управления видимостью элементов
        private Label employeeDocumentsLabel;
        private Button employeeDocumentsButton;
        private Label employeePhotoLabel;
        private Button employeePhotoButton;
        private Label customerOptionalPhotoLabel;
        private Button customerOptionalPhotoButton;

        // Поля для хранения выбранных файлов
        private string employeeDocumentsPath = "";
        private string employeePhotoPath = "";
        private string customerPhotoPath = "";

        public RegistrationForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Настройки формы
            this.Text = "Регистрация в Online Shop";
            this.Size = new Size(520, 900);
            this.BackColor = Color.WhiteSmoke;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Градиентный фон
            var gradientPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke
            };
            this.Controls.Add(gradientPanel);

            // Логотип
            var logo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(150, 150),
                Location = new Point(185, 20),
                Image = LoadImageOrDefault("E:\\с#\\Online_Shop_Pet_Project\\Online_Shop_Pet_Project\\art\\logo.png", 150, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Заголовок
            var title = new Label
            {
                Text = "Регистрация в Online Shop",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = primaryColor,
                AutoSize = true,
                Location = new Point(80, 180)
            };

            // Группа выбора типа регистрации
            var registrationTypeGroup = new Panel
            {
                Location = new Point(50, 230),
                Size = new Size(420, 60),
                BackColor = Color.Transparent
            };
            registrationTypeGroup.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, registrationTypeGroup.Width - 1, registrationTypeGroup.Height - 1);
            };

            var employeeRadio = new RadioButton
            {
                Text = "Сотрудник",
                Location = new Point(20, 20),
                Checked = false,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true
            };
            var customerRadio = new RadioButton
            {
                Text = "Покупатель",
                Location = new Point(200, 20),
                Checked = true,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true
            };

            employeeRadio.CheckedChanged += (s, e) =>
            {
                if (employeeRadio.Checked)
                {
                    isEmployeeRegistration = true;
                    ToggleRegistrationFields();
                }
            };
            customerRadio.CheckedChanged += (s, e) =>
            {
                if (customerRadio.Checked)
                {
                    isEmployeeRegistration = false;
                    ToggleRegistrationFields();
                }
            };

            registrationTypeGroup.Controls.Add(employeeRadio);
            registrationTypeGroup.Controls.Add(customerRadio);

            // Основные поля
            int labelX = 50;
            int inputX = 50;
            int currentY = 310;
            int verticalSpacing = 50;

            // Email/Телефон
            var contactLabel = CreateLabel("Номер телефона / Email:", new Point(labelX, currentY));
            contactTextBox = CreateTextBox(new Point(inputX, currentY + 25), 400);

            currentY += verticalSpacing;

            // Пароль
            var passwordLabel = CreateLabel("Пароль:", new Point(labelX, currentY));
            passwordTextBox = CreateTextBox(new Point(inputX, currentY + 25), 400, true);

            currentY += verticalSpacing;

            // Поля для сотрудников
            employeeDocumentsLabel = CreateLabel("Документы для присутствия:", new Point(labelX, currentY));
            employeeDocumentsButton = CreateButton("Загрузить документы", new Point(inputX, currentY + 25));
            employeeDocumentsButton.Click += (s, e) =>
            {
                var path = OpenFileDialogAndGetPath();
                if (!string.IsNullOrEmpty(path))
                {
                    employeeDocumentsPath = path;
                    employeeDocumentsButton.Text = "Документы выбраны";
                }
            };

            currentY += verticalSpacing;

            employeePhotoLabel = CreateLabel("Фото сотрудника:", new Point(labelX, currentY));
            employeePhotoButton = CreateButton("Загрузить фото", new Point(inputX, currentY + 25));
            employeePhotoButton.Click += (s, e) =>
            {
                var path = OpenFileDialogAndGetPath();
                if (!string.IsNullOrEmpty(path))
                {
                    employeePhotoPath = path;
                    employeePhotoButton.Text = "Фото выбрано";
                }
            };

            // Для покупателей (по желанию)
            currentY += verticalSpacing;
            customerOptionalPhotoLabel = CreateLabel("Фото (по желанию):", new Point(labelX, currentY));
            customerOptionalPhotoButton = CreateButton("Загрузить фото", new Point(inputX, currentY + 25));
            customerOptionalPhotoButton.Click += (s, e) =>
            {
                var path = OpenFileDialogAndGetPath();
                if (!string.IsNullOrEmpty(path))
                {
                    customerPhotoPath = path;
                    customerOptionalPhotoButton.Text = "Фото выбрано";
                }
            };

            // CheckBox для согласия с условиями
            currentY += verticalSpacing;
            var termsCheckBox = new CheckBox
            {
                Text = "Я согласен с условиями использования магазина",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(labelX + 5, currentY + 10),
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
                Location = new Point(labelX + 5, currentY + 40),
                Cursor = Cursors.Hand
            };
            termsLink.Click += (s, e) => ShowTermsDialog();

            currentY += 30;

            // Кнопка регистрации
            var registerButton = new Button
            {
                Text = "Зарегистрироваться",
                BackColor = primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(420, 50),
                Location = new Point(50, currentY + 50),
                Cursor = Cursors.Hand
            };
            registerButton.FlatAppearance.BorderSize = 0;
            registerButton.Click += (s, e) =>
            {
                if (!termsCheckBox.Checked)
                {
                    MessageBox.Show("Для регистрации необходимо согласиться с условиями использования магазина",
                                  "Требуется подтверждение",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return;
                }

                ProcessRegistration();
            };

            // Текст с ссылкой на вход
            var loginPrompt = new Label
            {
                Text = "Уже есть аккаунт? ",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(150, currentY + 110)
            };

            var loginLink = new LinkLabel
            {
                Text = "Войти",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                LinkColor = primaryColor,
                ActiveLinkColor = Color.FromArgb(0, 100, 200),
                AutoSize = true,
                Location = new Point(280, currentY + 110),
                Cursor = Cursors.Hand
            };
            loginLink.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            // Добавляем все элементы на главный контейнер
            gradientPanel.Controls.Add(logo);
            gradientPanel.Controls.Add(title);
            gradientPanel.Controls.Add(registrationTypeGroup);
            gradientPanel.Controls.Add(contactLabel);
            gradientPanel.Controls.Add(contactTextBox);
            gradientPanel.Controls.Add(passwordLabel);
            gradientPanel.Controls.Add(passwordTextBox);
            gradientPanel.Controls.Add(employeeDocumentsLabel);
            gradientPanel.Controls.Add(employeeDocumentsButton);
            gradientPanel.Controls.Add(employeePhotoLabel);
            gradientPanel.Controls.Add(employeePhotoButton);
            gradientPanel.Controls.Add(customerOptionalPhotoLabel);
            gradientPanel.Controls.Add(customerOptionalPhotoButton);
            gradientPanel.Controls.Add(termsCheckBox);
            gradientPanel.Controls.Add(termsLink);
            gradientPanel.Controls.Add(registerButton);
            gradientPanel.Controls.Add(loginPrompt);
            gradientPanel.Controls.Add(loginLink);

            // Изначально скрываем поля для сотрудников
            ToggleRegistrationFields();
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

        private Label CreateLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true,
                Location = location
            };
        }

        private TextBox CreateTextBox(Point location, int width, bool isPassword = false)
        {
            return new TextBox
            {
                Location = location,
                Size = new Size(width, 30),
                Font = new Font("Segoe UI", 12),
                UseSystemPasswordChar = isPassword
            };
        }

        private Button CreateButton(string text, Point location)
        {
            return new Button
            {
                Text = text,
                Size = new Size(180, 30),
                Location = location,
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
        }

        private string OpenFileDialogAndGetPath()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Выберите файл";
                ofd.Filter = "Все файлы (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return ofd.FileName;
                }
            }
            return "";
        }

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
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                g.DrawString("Лого", new Font("Arial", 14, FontStyle.Bold), Brushes.DarkGray, 10, height / 2 - 10);
            }
            return bmp;
        }

        private void ProcessRegistration()
        {
            string username = contactTextBox.Text;
            string password = passwordTextBox.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля");
                return;
            }

            try
            {
                if (DbManager.RegisterUser(username, password, isEmployee: isEmployeeRegistration))
                {
                    IsEmployee = isEmployeeRegistration;
                    MessageBox.Show(
                        IsEmployee ? "Регистрация сотрудника выполнена" : "Регистрация покупателя выполнена",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким именем уже существует");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}");
            }
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {

        }
    }
}