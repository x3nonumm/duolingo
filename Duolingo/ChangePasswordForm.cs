using System;
using System.Drawing;
using System.Windows.Forms;

namespace Duolingo
{
    public class ChangePasswordForm : Form
    {
        private int userId;
        private TextBox txtCurrentPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;

        public ChangePasswordForm(int userId)
        {
            this.userId = userId;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Смена пароля";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Padding = new Padding(20);

            var titleLabel = new Label
            {
                Text = "Смена пароля",
                Font = new Font("Arial", 18, FontStyle.Bold),
                Location = new Point(120, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            var lblCurrent = new Label
            {
                Text = "Текущий пароль:",
                Location = new Point(50, 70),
                AutoSize = true
            };

            txtCurrentPassword = new TextBox
            {
                Location = new Point(50, 95),
                Size = new Size(300, 25),
                PasswordChar = '*',
                Text = "demo"
            };

            var lblNew = new Label
            {
                Text = "Новый пароль:",
                Location = new Point(50, 130),
                AutoSize = true
            };

            txtNewPassword = new TextBox
            {
                Location = new Point(50, 155),
                Size = new Size(300, 25),
                PasswordChar = '*'
            };

            var lblConfirm = new Label
            {
                Text = "Подтвердите новый пароль:",
                Location = new Point(50, 190),
                AutoSize = true
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(50, 215),
                Size = new Size(300, 25),
                PasswordChar = '*'
            };

            // Информация о демо-пароле
            var demoInfo = new Label
            {
                Text = "В демо-версии используйте 'demo' как текущий пароль",
                Font = new Font("Arial", 9, FontStyle.Italic),
                Location = new Point(50, 245),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            var btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(150, 280),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => SavePassword();

            this.Controls.Add(titleLabel);
            this.Controls.Add(lblCurrent);
            this.Controls.Add(txtCurrentPassword);
            this.Controls.Add(lblNew);
            this.Controls.Add(txtNewPassword);
            this.Controls.Add(lblConfirm);
            this.Controls.Add(txtConfirmPassword);
            this.Controls.Add(demoInfo);
            this.Controls.Add(btnSave);
        }

        private void SavePassword()
        {
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
            {
                MessageBox.Show("Введите текущий пароль", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text) || txtNewPassword.Text.Length < 3)
            {
                MessageBox.Show("Новый пароль должен содержать минимум 3 символа", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Новые пароли не совпадают", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // В демо-версии пароль всегда "demo" для существующих пользователей
            string currentPassword = txtCurrentPassword.Text.Trim();
            if (currentPassword != "demo")
            {
                MessageBox.Show("Текущий пароль неверен. Для демо используйте 'demo'", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Пароль успешно изменен!\n\n" +
                          "В демо-версии изменения не сохраняются.\n" +
                          "В реальном приложении пароль был бы сохранен в базе данных.",
                          "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}