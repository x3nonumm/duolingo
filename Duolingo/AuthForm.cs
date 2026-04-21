using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Duolingo
{
    public class AuthForm : Form
    {
        private DatabaseHelper dbHelper;
        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnGuest;
        private Label lblStatus;
        private Panel loginPanel;
        private Panel registerPanel;
        private User currentUser;

        public AuthForm(DatabaseHelper dbHelper)
        {
            this.dbHelper = dbHelper;
            InitializeForm();
            ShowLoginPanel();
        }      
        public User GetAuthenticatedUser()
        {
            if (currentUser != null)
            
                return dbHelper.GetUserById(currentUser.Id);
            }
            return currentUser;
        }

        private void InitializeForm()
        {
            this.Text = "Duolingo - Вход";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var titleLabel = new Label
            {
                Text = "Duolingo",
                Font = new Font("Arial", 32, FontStyle.Bold),
                Location = new Point(150, 30),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            var subtitleLabel = new Label
            {
                Text = "Изучай языки бесплатно",
                Font = new Font("Arial", 14),
                Location = new Point(140, 80),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            lblStatus = new Label
            {
                Location = new Point(50, 120),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10)
            };

            loginPanel = new Panel
            {
                Location = new Point(50, 160),
                Size = new Size(400, 300),
                Visible = false
            };

            CreateLoginPanel();

            registerPanel = new Panel
            {
                Location = new Point(50, 160),
                Size = new Size(400, 350),
                Visible = false
            };

            CreateRegisterPanel();

            // Кнопка гостя
            btnGuest = new Button
            {
                Text = "Продолжить как гость",
                Location = new Point(150, 520),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10),
                ForeColor = Color.FromArgb(88, 206, 138),
                BackColor = Color.Transparent
            };
            btnGuest.FlatAppearance.BorderSize = 0;
            btnGuest.Click += (s, e) => LoginAsGuest();

            // Кнопка отладки (скрытая для разработки)
            var debugButton = new Button
            {
                Text = "🐞",
                Location = new Point(450, 520),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent
            };
            debugButton.FlatAppearance.BorderSize = 0;
            debugButton.Click += (s, e) => DebugUsers();

            this.Controls.Add(titleLabel);
            this.Controls.Add(subtitleLabel);
            this.Controls.Add(lblStatus);
            this.Controls.Add(loginPanel);
            this.Controls.Add(registerPanel);
            this.Controls.Add(btnGuest);
            this.Controls.Add(debugButton);
        }

        private void CreateLoginPanel()
        {
            loginPanel.Controls.Clear();

            var lblLoginTitle = new Label
            {
                Text = "Вход в аккаунт",
                Font = new Font("Arial", 18, FontStyle.Bold),
                Location = new Point(100, 10),
                AutoSize = true
            };

            var lblUsername = new Label
            {
                Text = "Имя пользователя или Email:",
                Location = new Point(0, 60),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Location = new Point(0, 85),
                Size = new Size(400, 30),
                Font = new Font("Arial", 12)
            };

            var lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(0, 125),
                AutoSize = true
            };

            txtPassword = new TextBox
            {
                Location = new Point(0, 150),
                Size = new Size(400, 30),
                Font = new Font("Arial", 12),
                PasswordChar = '*',
                Text = "demo" // Дефолтный пароль для удобства
            };

            // Информация о демо-пароле
            var demoInfo = new Label
            {
                Text = "Для демо используйте пароль: 'demo'",
                Font = new Font("Arial", 9, FontStyle.Italic),
                Location = new Point(0, 185),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            // Кнопка входа
            btnLogin = new Button
            {
                Text = "Войти",
                Location = new Point(100, 220),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += (s, e) => Login();

            // Кнопка регистрации (в той же панели)
            var btnRegisterInLogin = new Button
            {
                Text = "Зарегистрироваться",
                Location = new Point(100, 270),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnRegisterInLogin.FlatAppearance.BorderSize = 0;
            btnRegisterInLogin.Click += (s, e) => ShowRegisterPanel();

            loginPanel.Controls.Add(lblLoginTitle);
            loginPanel.Controls.Add(lblUsername);
            loginPanel.Controls.Add(txtUsername);
            loginPanel.Controls.Add(lblPassword);
            loginPanel.Controls.Add(txtPassword);
            loginPanel.Controls.Add(demoInfo);
            loginPanel.Controls.Add(btnLogin);
            loginPanel.Controls.Add(btnRegisterInLogin);
        }

        private void CreateRegisterPanel()
        {
            registerPanel.Controls.Clear();

            var lblRegisterTitle = new Label
            {
                Text = "Регистрация",
                Font = new Font("Arial", 18, FontStyle.Bold),
                Location = new Point(120, 10),
                AutoSize = true
            };

            var lblRegUsername = new Label
            {
                Text = "Имя пользователя:",
                Location = new Point(0, 60),
                AutoSize = true
            };

            var txtRegUsername = new TextBox
            {
                Location = new Point(0, 85),
                Size = new Size(400, 30),
                Font = new Font("Arial", 12)
            };

            var lblRegEmail = new Label
            {
                Text = "Email:",
                Location = new Point(0, 125),
                AutoSize = true
            };

            txtEmail = new TextBox
            {
                Location = new Point(0, 150),
                Size = new Size(400, 30),
                Font = new Font("Arial", 12)
            };

            var lblRegPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(0, 190),
                AutoSize = true
            };

            txtPassword = new TextBox
            {
                Location = new Point(0, 215),
                Size = new Size(400, 30),
                Font = new Font("Arial", 12),
                PasswordChar = '*',
                Text = "demo"
            };

            var lblRegConfirmPassword = new Label
            {
                Text = "Подтвердите пароль:",
                Location = new Point(0, 255),
                AutoSize = true
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(0, 280),
                Size = new Size(400, 30),
                Font = new Font("Arial", 12),
                PasswordChar = '*',
                Text = "demo"
            };

            // Информация о демо-пароле
            var demoInfo = new Label
            {
                Text = "В демо-версии пароль всегда 'demo'",
                Font = new Font("Arial", 9, FontStyle.Italic),
                Location = new Point(0, 315),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            btnRegister = new Button
            {
                Text = "Зарегистрироваться",
                Location = new Point(100, 340),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += (s, e) => Register(txtRegUsername.Text, txtEmail.Text,
                txtPassword.Text, txtConfirmPassword.Text);

            var switchToLogin = new LinkLabel
            {
                Text = "Уже есть аккаунт? Войти",
                Location = new Point(120, 390),
                AutoSize = true,
                Font = new Font("Arial", 10)
            };
            switchToLogin.LinkClicked += (s, e) => ShowLoginPanel();

            registerPanel.Controls.Add(lblRegisterTitle);
            registerPanel.Controls.Add(lblRegUsername);
            registerPanel.Controls.Add(txtRegUsername);
            registerPanel.Controls.Add(lblRegEmail);
            registerPanel.Controls.Add(txtEmail);
            registerPanel.Controls.Add(lblRegPassword);
            registerPanel.Controls.Add(txtPassword);
            registerPanel.Controls.Add(lblRegConfirmPassword);
            registerPanel.Controls.Add(txtConfirmPassword);
            registerPanel.Controls.Add(demoInfo);
            registerPanel.Controls.Add(btnRegister);
            registerPanel.Controls.Add(switchToLogin);
        }

        private void ShowLoginPanel()
        {
            loginPanel.Visible = true;
            registerPanel.Visible = false;
            lblStatus.Text = "";
            txtUsername?.Focus();
        }

        private void ShowRegisterPanel()
        {
            loginPanel.Visible = false;
            registerPanel.Visible = true;
            lblStatus.Text = "";
        }

        private void Login()
        {
            string usernameOrEmail = txtUsername.Text.Trim();

            if (string.IsNullOrWhiteSpace(usernameOrEmail))
            {
                ShowError("Введите имя пользователя или email");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowError("Введите пароль");
                return;
            }

            // Ищем пользователя по имени пользователя или email
            User user = dbHelper.GetUserByUsernameOrEmail(usernameOrEmail);

            if (user == null)
            {
                ShowError("Пользователь не найден. Проверьте имя пользователя или email");
                return;
            }

            // В демо-версии проверка пароля упрощена
            string inputPassword = txtPassword.Text.Trim();

            // Допустимые пароли для демо
            bool isPasswordValid = inputPassword == "demo" ||
                                  inputPassword == user.PasswordHash;

            if (!isPasswordValid)
            {
                ShowError("Неверный пароль. Для демо используйте 'demo'");
                return;
            }

            currentUser = user; // Сохраняем пользователя
            ShowSuccess($"Добро пожаловать, {user.Username}!");

            // Закрываем форму через 1 секунду
            var timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            timer.Start();
        }

        private void Register(string username, string email, string password, string confirmPassword)
        {
            // Валидация имени пользователя
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Введите имя пользователя");
                return;
            }

            if (username.Length < 3)
            {
                ShowError("Имя пользователя должно содержать минимум 3 символа");
                return;
            }

            if (username.Length > 20)
            {
                ShowError("Имя пользователя не должно превышать 20 символов");
                return;
            }

            // Проверяем допустимые символы
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            {
                ShowError("Имя пользователя может содержать только буквы, цифры и символ подчеркивания");
                return;
            }

            // Валидация email
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Введите email");
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowError("Введите корректный email");
                return;
            }

            // Валидация пароля
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Введите пароль");
                return;
            }

            if (password.Length < 3)
            {
                ShowError("Пароль должен содержать минимум 3 символа");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Пароли не совпадают");
                return;
            }

            // Проверяем, не существует ли уже пользователь с таким именем
            if (dbHelper.GetUser(username) != null)
            {
                ShowError("Пользователь с таким именем уже существует");
                return;
            }

            // Проверяем, не существует ли уже пользователь с таким email
            var allUsers = dbHelper.GetAllUsers();
            if (allUsers.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                ShowError("Пользователь с таким email уже зарегистрирован");
                return;
            }

            // Создаем нового пользователя
            var newUser = dbHelper.CreateUser(username, email);
            if (newUser != null)
            {
                ShowSuccess($"Аккаунт {username} успешно создан!");
                currentUser = newUser;

                // Автоматически входим после успешной регистрации
                ShowSuccess($"Аккаунт {username} создан. Автоматический вход...");

                // Закрываем форму через 1.5 секунды
                var timer = new Timer { Interval = 1500 };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };
                timer.Start();
            }
            else
            {
                ShowError("Ошибка при создании аккаунта. Возможно, пользователь уже существует");
            }
        }

        private void LoginAsGuest()
        {
            // Создаем гостевого пользователя или используем существующего
            var guestUser = dbHelper.GetUser("Гость");
            if (guestUser == null)
            {
                guestUser = dbHelper.CreateUser("Гость", "guest@example.com");
                if (guestUser == null)
                {
                    ShowError("Ошибка создания гостевого аккаунта");
                    return;
                }
            }

            currentUser = guestUser;
            ShowSuccess("Вы вошли как гость");

            var timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            timer.Start();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void DebugUsers()
        {
            var allUsers = dbHelper.GetAllUsers();
            string debugInfo = $"Всего пользователей в базе: {dbHelper.GetUserCount()}\n";
            debugInfo += $"Зарегистрированных пользователей: {allUsers.Count}\n\n";

            var allUsersFromDb = dbHelper.GetAllUsersForDebug();
            foreach (var user in allUsersFromDb)
            {
                debugInfo += $"- {user.Username} (ID: {user.Id}, Email: {user.Email}, XP: {user.Experience}, Password: {user.PasswordHash})\n";
            }

            MessageBox.Show(debugInfo, "Отладочная информация о пользователях",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowError(string message)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = Color.Red;
        }

        private void ShowSuccess(string message)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = Color.Green;
        }
    }
}