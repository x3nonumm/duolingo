using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Duolingo
{
    public class ProfileForm : Form
    {
        private DatabaseHelper dbHelper;
        private User currentUser;

        public ProfileForm(DatabaseHelper dbHelper, User user)
        {
            this.dbHelper = dbHelper;
            this.currentUser = user; // Получаем уже обновленного пользователя

            // Проверяем пользователя
            if (currentUser == null)
            {
                MessageBox.Show("Ошибка: пользователь не найден", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            InitializeForm();
            LoadProfileData();
        }

        private void InitializeForm()
        {
            this.Text = $"Мой профиль - {currentUser.Username}";
            this.Size = new Size(850, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Заголовок формы
            var titleLabel = new Label
            {
                Text = "Мой профиль",
                Font = new Font("Arial", 24, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            // Кнопка закрытия
            var closeButton = new Button
            {
                Text = "✕",
                Location = new Point(800, 10),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            this.Controls.Add(titleLabel);
            this.Controls.Add(closeButton);
        }

        private void LoadProfileData()
        {
            if (currentUser == null)
            {
                ShowErrorMessage("Пользователь не найден");
                return;
            }

            // Основная информация
            CreateUserInfoSection();

            // Статистика
            CreateStatisticsSection();

            // Прогресс уровня
            CreateLevelProgressSection();

            // Недавние слова
            CreateRecentWordsSection();

            // Достижения
            CreateAchievementsSection();

            // Управление аккаунтом (только для не гостевых аккаунтов)
            if (currentUser.Username != "Гость")
            {
                CreateAccountManagementSection();
            }

            // Кнопки действий (ДОБАВЛЕНА КНОПКА ТЕОРИИ)
            CreateActionButtons();
        }

        private void CreateUserInfoSection()
        {
            var userCard = CreateCard("👤 Личная информация", 30, 70, 350, 180);

            var userInfo = new Label
            {
                Text = $"Имя: {currentUser.Username}\n\n" +
                       $"Email: {currentUser.Email}\n\n" +
                       $"Опыт: {currentUser.Experience} XP\n\n" +
                       $"Уровень: {currentUser.Level}\n\n" +
                       $"Регистрация: {currentUser.RegistrationDate:dd.MM.yyyy}",
                Location = new Point(20, 40),
                AutoSize = true,
                Font = new Font("Arial", 11)
            };

            // Аватар пользователя
            var avatarPanel = CreateRoundedPanel(80, 80, 40, Color.FromArgb(88, 206, 138));
            avatarPanel.Location = new Point(250, 40);

            var avatarText = new Label
            {
                Text = GetInitials(currentUser.Username),
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 25),
                AutoSize = true
            };

            avatarPanel.Controls.Add(avatarText);
            userCard.Controls.Add(userInfo);
            userCard.Controls.Add(avatarPanel);
            this.Controls.Add(userCard);
        }

        private void CreateStatisticsSection()
        {
            var statsCard = CreateCard("📊 Статистика обучения", 400, 70, 400, 180);

            var stats = dbHelper.GetUserStatistics(currentUser.Id);
            int y = 40;
            foreach (var stat in stats)
            {
                var statPanel = new Panel
                {
                    Location = new Point(20, y),
                    Size = new Size(350, 25)
                };

                var iconLabel = new Label
                {
                    Text = stat.Icon,
                    Location = new Point(0, 0),
                    AutoSize = true,
                    Font = new Font("Arial", 12)
                };

                var titleLabel = new Label
                {
                    Text = stat.Title,
                    Location = new Point(30, 0),
                    AutoSize = true,
                    Font = new Font("Arial", 10),
                    ForeColor = Color.Gray
                };

                var valueLabel = new Label
                {
                    Text = stat.Value,
                    Location = new Point(250, 0),
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(88, 206, 138)
                };

                statPanel.Controls.Add(iconLabel);
                statPanel.Controls.Add(titleLabel);
                statPanel.Controls.Add(valueLabel);
                statsCard.Controls.Add(statPanel);
                y += 28;
            }

            this.Controls.Add(statsCard);
        }

        private void CreateLevelProgressSection()
        {
            var levelCard = CreateCard("📈 Прогресс уровня", 30, 270, 350, 120);

            var levelLabel = new Label
            {
                Text = $"Уровень {currentUser.Level}",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 40),
                AutoSize = true
            };

            var progressBar = new ProgressBar
            {
                Location = new Point(20, 70),
                Size = new Size(300, 20),
                Minimum = 0,
                Maximum = 500,
                Value = Math.Min(currentUser.Experience % 500, 500),
                Style = ProgressBarStyle.Continuous
            };

            var progressLabel = new Label
            {
                Text = $"{currentUser.Experience % 500}/500 XP до уровня {currentUser.Level + 1}",
                Location = new Point(20, 95),
                AutoSize = true,
                Font = new Font("Arial", 9),
                ForeColor = Color.Gray
            };

            levelCard.Controls.Add(levelLabel);
            levelCard.Controls.Add(progressBar);
            levelCard.Controls.Add(progressLabel);
            this.Controls.Add(levelCard);
        }

        private void CreateRecentWordsSection()
        {
            var wordsCard = CreateCard("📝 Недавно изученные слова", 400, 270, 400, 120);

            var recentWords = dbHelper.GetRecentlyLearnedWords(currentUser.Id, 4);
            int y = 40;
            foreach (var word in recentWords)
            {
                var wordLabel = new Label
                {
                    Text = $"• {word}",
                    Location = new Point(20, y),
                    AutoSize = true,
                    Font = new Font("Arial", 11)
                };
                wordsCard.Controls.Add(wordLabel);
                y += 25;
            }

            var viewAllButton = new Button
            {
                Text = "Все слова →",
                Location = new Point(300, 85),
                Size = new Size(80, 25),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8),
                ForeColor = Color.FromArgb(88, 206, 138),
                BackColor = Color.Transparent
            };
            viewAllButton.FlatAppearance.BorderSize = 0;
            viewAllButton.Click += (s, e) => ShowAllWords();
            wordsCard.Controls.Add(viewAllButton);

            this.Controls.Add(wordsCard);
        }

        private void CreateAchievementsSection()
        {
            var achievementsPanel = new Panel
            {
                Location = new Point(30, 410),
                Size = new Size(770, 250),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                AutoScroll = true
            };

            var achievementsTitle = new Label
            {
                Text = "🏆 Достижения",
                Font = new Font("Arial", 18, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var achievements = dbHelper.GetUserAchievements(currentUser.Id);
            var achievementsFlow = new FlowLayoutPanel
            {
                Location = new Point(20, 50),
                Size = new Size(730, 180),
                AutoScroll = true,
                WrapContents = true
            };

            foreach (var achievement in achievements)
            {
                var achPanel = CreateAchievementCard(achievement);
                achievementsFlow.Controls.Add(achPanel);
            }

            achievementsPanel.Controls.Add(achievementsTitle);
            achievementsPanel.Controls.Add(achievementsFlow);
            this.Controls.Add(achievementsPanel);
        }

        private void CreateAccountManagementSection()
        {
            var accountCard = CreateCard("🔐 Управление аккаунтом", 30, 670, 350, 150);

            var changePasswordBtn = new Button
            {
                Text = "✏️ Сменить пароль",
                Location = new Point(20, 40),
                Size = new Size(150, 35),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9)
            };
            changePasswordBtn.FlatAppearance.BorderSize = 0;
            changePasswordBtn.Click += (s, e) => ChangePassword();

            var switchAccountBtn = new Button
            {
                Text = "🔄 Сменить аккаунт",
                Location = new Point(180, 40),
                Size = new Size(150, 35),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9)
            };
            switchAccountBtn.FlatAppearance.BorderSize = 0;
            switchAccountBtn.Click += (s, e) => SwitchAccount();

            var deleteAccountBtn = new Button
            {
                Text = "🗑️ Удалить аккаунт",
                Location = new Point(20, 85),
                Size = new Size(150, 35),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9),
                ForeColor = Color.Red
            };
            deleteAccountBtn.FlatAppearance.BorderSize = 0;
            deleteAccountBtn.Click += (s, e) => DeleteAccount();

            var exportDataBtn = new Button
            {
                Text = "📤 Экспорт данных",
                Location = new Point(180, 85),
                Size = new Size(150, 35),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9)
            };
            exportDataBtn.FlatAppearance.BorderSize = 0;
            exportDataBtn.Click += (s, e) => ExportData();

            accountCard.Controls.Add(changePasswordBtn);
            accountCard.Controls.Add(switchAccountBtn);
            accountCard.Controls.Add(deleteAccountBtn);
            accountCard.Controls.Add(exportDataBtn);

            this.Controls.Add(accountCard);
        }

        private void CreateActionButtons()
        {
            var buttonPanel = new Panel
            {
                Location = new Point(400, 670),
                Size = new Size(400, 150)
            };

            var editButton = new Button
            {
                Text = "✏️ Редактировать профиль",
                Location = new Point(0, 0),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            editButton.FlatAppearance.BorderSize = 0;
            editButton.Click += (s, e) => EditProfile();

            // ========== КНОПКА ТЕОРИИ ==========
            var theoryButton = new Button
            {
                Text = "📚 Изучить теорию",
                Location = new Point(190, 0),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            theoryButton.FlatAppearance.BorderSize = 0;
            theoryButton.Click += (s, e) => ShowTheoryForm();

            var statsButton = new Button
            {
                Text = "📊 Подробная статистика",
                Location = new Point(0, 50),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Arial", 10),
                FlatStyle = FlatStyle.Flat
            };
            statsButton.FlatAppearance.BorderSize = 0;
            statsButton.Click += (s, e) => ShowDetailedStatistics();

            var helpButton = new Button
            {
                Text = "❓ Помощь и поддержка",
                Location = new Point(190, 50),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.White,
                Font = new Font("Arial", 10),
                FlatStyle = FlatStyle.Flat
            };
            helpButton.FlatAppearance.BorderSize = 0;
            helpButton.Click += (s, e) => ShowHelp();

            buttonPanel.Controls.Add(editButton);
            buttonPanel.Controls.Add(theoryButton);
            buttonPanel.Controls.Add(statsButton);
            buttonPanel.Controls.Add(helpButton);
            this.Controls.Add(buttonPanel);
        }

        // ========== МЕТОД ДЛЯ ПОКАЗА ТЕОРИИ ==========
        private void ShowTheoryForm()
        {
            try
            {
                if (currentUser == null)
                {
                    MessageBox.Show("Пожалуйста, войдите в систему", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var theoryForm = new DuolingoTheoryForm(dbHelper, currentUser);
                theoryForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть теорию: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateCard(string title, int x, int y, int width, int height)
        {
            var card = new Panel
            {
                Size = new Size(width, height),
                Location = new Point(x, y),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var cardTitle = new Label
            {
                Text = title,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            card.Controls.Add(cardTitle);
            return card;
        }

        private Panel CreateAchievementCard(Achievement achievement)
        {
            var card = new Panel
            {
                Size = new Size(220, 70),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = achievement.IsUnlocked ?
                           Color.FromArgb(240, 255, 240) :
                           Color.FromArgb(250, 250, 250),
                Margin = new Padding(5)
            };

            var iconLabel = new Label
            {
                Text = achievement.Icon,
                Font = new Font("Arial", 20),
                Location = new Point(10, 20),
                AutoSize = true
            };

            var nameLabel = new Label
            {
                Text = achievement.Name,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(50, 10),
                AutoSize = true,
                ForeColor = achievement.IsUnlocked ? Color.Green : Color.Gray
            };

            var descLabel = new Label
            {
                Text = achievement.Description,
                Font = new Font("Arial", 8),
                Location = new Point(50, 30),
                Size = new Size(150, 30),
                ForeColor = Color.Gray
            };

            string statusText;
            if (achievement.IsUnlocked)
            {
                statusText = $"✅ {achievement.UnlockedDate:dd.MM.yy}";
            }
            else
            {
                statusText = "🔒 Не получено";
            }

            var statusLabel = new Label
            {
                Text = statusText,
                Font = new Font("Arial", 8),
                Location = new Point(150, 10),
                AutoSize = true,
                ForeColor = achievement.IsUnlocked ? Color.Green : Color.Red
            };

            card.Controls.Add(iconLabel);
            card.Controls.Add(nameLabel);
            card.Controls.Add(descLabel);
            card.Controls.Add(statusLabel);
            return card;
        }

        private RoundedPanel CreateRoundedPanel(int width, int height, int borderRadius, Color color)
        {
            var panel = new RoundedPanel
            {
                Size = new Size(width, height),
                BackColor = color
            };
            panel.BorderRadius = borderRadius;
            return panel;
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return "??";

            var parts = fullName.Split(' ');
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();

            return fullName.Length >= 2 ?
                   fullName.Substring(0, 2).ToUpper() :
                   fullName.ToUpper();
        }

        private void ShowErrorMessage(string message)
        {
            var errorLabel = new Label
            {
                Text = message,
                Font = new Font("Arial", 14),
                Location = new Point(50, 50),
                AutoSize = true,
                ForeColor = Color.Red
            };
            this.Controls.Add(errorLabel);
        }

        // Методы для кнопок
        private void EditProfile()
        {
            using (var editForm = new EditProfileForm(currentUser))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    dbHelper.UpdateUserProfile(currentUser.Id, editForm.Username, editForm.Email);
                    MessageBox.Show("Профиль успешно обновлен!", "Успех",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
        }

        private void ShareProgress()
        {
            string shareText = $"Я изучаю языки в Duolingo!\n" +
                              $"👤 {currentUser.Username}\n" +
                              $"📈 Уровень {currentUser.Level}\n" +
                              $"⭐ {currentUser.Experience} XP\n" +
                              $"🔥 {currentUser.StreakDays} дней подряд\n" +
                              $"📚 {currentUser.WordsLearned} выученных слов\n" +
                              $"⏱️ {currentUser.TotalTimeMinutes} минут практики\n" +
                              $"#Duolingo #ИзучениеЯзыков";

            Clipboard.SetText(shareText);
            MessageBox.Show("Прогресс скопирован в буфер обмена!\n\n" +
                          "Теперь вы можете поделиться им в социальных сетях.",
                          "Поделиться прогрессом",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowDetailedStatistics()
        {
            var statsForm = new StatisticsForm(dbHelper, currentUser);
            statsForm.ShowDialog();
        }

        private void ShowAllWords()
        {
            var wordsForm = new WordsForm(dbHelper, currentUser);
            wordsForm.ShowDialog();
        }

        private void ShowHelp()
        {
            MessageBox.Show(
                "📚 Помощь и поддержка\n\n" +
                "• Для смены пароля используйте кнопку 'Сменить пароль'\n" +
                "• Для смены аккаунта используйте 'Сменить аккаунт'\n" +
                "• Все данные хранятся локально на вашем устройстве\n" +
                "• Для экспорта данных используйте соответствующую кнопку\n" +
                "• Вопросы и предложения отправляйте на support@duolingo.demo\n\n" +
                "Приятного обучения! 🎓",
                "Помощь",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        // Методы управления аккаунтом
        private void ChangePassword()
        {
            using (var passwordForm = new ChangePasswordForm(currentUser.Id))
            {
                if (passwordForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Пароль успешно изменен!", "Успех",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void SwitchAccount()
        {
            if (MessageBox.Show("Вы уверены, что хотите сменить аккаунт?\n\n" +
                               "Текущий сеанс будет завершен.",
                               "Смена аккаунта",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Retry;
                this.Close();
            }
        }

        private void DeleteAccount()
        {
            if (MessageBox.Show("⚠️ ВНИМАНИЕ: Вы уверены, что хотите удалить аккаунт?\n\n" +
                               "• Все ваши данные будут безвозвратно удалены\n" +
                               "• Прогресс обучения будет утерян\n" +
                               "• Достижения будут сброшены\n" +
                               "• Это действие нельзя отменить\n\n" +
                               "Для подтверждения введите 'УДАЛИТЬ' в поле ниже:",
                               "Удаление аккаунта",
                               MessageBoxButtons.OKCancel,
                               MessageBoxIcon.Warning) == DialogResult.OK)
            {
                using (var confirmForm = new ConfirmDeleteForm())
                {
                    if (confirmForm.ShowDialog() == DialogResult.OK)
                    {
                        if (dbHelper.DeleteUser(currentUser.Id))
                        {
                            MessageBox.Show("Аккаунт успешно удален", "Успех",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.Abort;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при удалении аккаунта", "Ошибка",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void ExportData()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Text Files|*.txt",
                FileName = $"Duolingo_Data_{currentUser.Username}_{DateTime.Now:yyyyMMdd}.txt",
                Title = "Экспорт данных"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string data = GenerateExportData();
                    System.IO.File.WriteAllText(saveDialog.FileName, data);
                    MessageBox.Show("Данные успешно экспортированы в файл:\n" +
                                  saveDialog.FileName,
                                  "Экспорт данных",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте данных: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
            }
        }

        private string GenerateExportData()
        {
            var stats = dbHelper.GetUserStatistics(currentUser.Id);
            var achievements = dbHelper.GetUserAchievements(currentUser.Id);
            var words = dbHelper.GetRecentlyLearnedWords(currentUser.Id, 100);

            string exportData = "=== Duolingo - Экспорт данных ===\n";
            exportData += $"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n";
            exportData += $"Пользователь: {currentUser.Username}\n";
            exportData += $"Email: {currentUser.Email}\n";
            exportData += $"Уровень: {currentUser.Level}\n";
            exportData += $"Опыт: {currentUser.Experience} XP\n";
            exportData += $"Серия дней: {currentUser.StreakDays}\n";
            exportData += $"Завершено уроков: {currentUser.CompletedLessons}\n";
            exportData += $"Выучено слов: {currentUser.WordsLearned}\n";
            exportData += $"Общее время: {currentUser.TotalTimeMinutes} мин\n";
            exportData += $"Дата регистрации: {currentUser.RegistrationDate:dd.MM.yyyy}\n";
            exportData += $"Дата последней практики: {currentUser.LastPracticeDate:dd.MM.yyyy HH:mm}\n\n";

            exportData += "=== Статистика ===\n";
            foreach (var stat in stats)
            {
                exportData += $"{stat.Icon} {stat.Title}: {stat.Value}\n";
            }
            exportData += "\n";

            exportData += "=== Достижения ===\n";
            foreach (var achievement in achievements)
            {
                string status = achievement.IsUnlocked ?
                    $"✅ Получено {achievement.UnlockedDate:dd.MM.yyyy}" :
                    "❌ Не получено";
                exportData += $"{achievement.Icon} {achievement.Name}: {achievement.Description} - {status}\n";
            }
            exportData += "\n";

            exportData += "=== Изученные слова ===\n";
            foreach (var word in words)
            {
                exportData += $"{word}\n";
            }

            exportData += "\n=== Конец данных ===\n";
            exportData += $"Всего записей: {words.Count}\n";
            exportData += $"Экспорт завершен: {DateTime.Now:HH:mm:ss}";

            return exportData;
        }
    }

    // Форма для редактирования профиля
    public class EditProfileForm : Form
    {
        private User currentUser;
        private TextBox txtUsername;
        private TextBox txtEmail;

        public string Username => txtUsername.Text;
        public string Email => txtEmail.Text;

        public EditProfileForm(User user)
        {
            currentUser = user;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Редактирование профиля";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "✏️ Редактирование профиля",
                Font = new Font("Arial", 18, FontStyle.Bold),
                Location = new Point(50, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            var lblUsername = new Label
            {
                Text = "Имя пользователя:",
                Location = new Point(50, 70),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Text = currentUser.Username,
                Location = new Point(50, 95),
                Size = new Size(300, 25)
            };

            var lblEmail = new Label
            {
                Text = "Email:",
                Location = new Point(50, 130),
                AutoSize = true
            };

            txtEmail = new TextBox
            {
                Text = currentUser.Email,
                Location = new Point(50, 155),
                Size = new Size(300, 25)
            };

            var saveButton = new Button
            {
                Text = "Сохранить",
                Location = new Point(150, 200),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Имя пользователя не может быть пустым", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(260, 200),
                Size = new Size(80, 35),
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);
        }
    }

    // Форма для подтверждения удаления
    public class ConfirmDeleteForm : Form
    {
        private TextBox txtConfirmation;

        public ConfirmDeleteForm()
        {
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Подтверждение удаления";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            var warningLabel = new Label
            {
                Text = "⚠️ Введите 'УДАЛИТЬ' для подтверждения:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(50, 30),
                AutoSize = true,
                ForeColor = Color.Red
            };

            txtConfirmation = new TextBox
            {
                Location = new Point(50, 60),
                Size = new Size(300, 25),
                Font = new Font("Arial", 12)
            };

            var confirmButton = new Button
            {
                Text = "Подтвердить",
                Location = new Point(150, 100),
                Size = new Size(100, 35),
                BackColor = Color.Red,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            confirmButton.FlatAppearance.BorderSize = 0;
            confirmButton.Click += (s, e) =>
            {
                if (txtConfirmation.Text == "УДАЛИТЬ")
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Введено неправильное подтверждение", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(260, 100),
                Size = new Size(80, 35),
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.Add(warningLabel);
            this.Controls.Add(txtConfirmation);
            this.Controls.Add(confirmButton);
            this.Controls.Add(cancelButton);
        }
    }

    // Форма для просмотра всех слов
    public class WordsForm : Form
    {
        private DatabaseHelper dbHelper;
        private User currentUser;

        public WordsForm(DatabaseHelper dbHelper, User user)
        {
            this.dbHelper = dbHelper;
            this.currentUser = user; // Получаем уже обновленного пользователя

            // Проверяем пользователя
            if (currentUser == null)
            {
                MessageBox.Show("Ошибка: пользователь не найден", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Мой словарь";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "📖 Мой словарь",
                Font = new Font("Arial", 20, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            var words = dbHelper.GetRecentlyLearnedWords(currentUser.Id, 100);
            var wordsList = new ListBox
            {
                Location = new Point(20, 60),
                Size = new Size(550, 380),
                Font = new Font("Arial", 11),
                BorderStyle = BorderStyle.FixedSingle
            };

            foreach (var word in words)
            {
                wordsList.Items.Add(word);
            }

            var closeButton = new Button
            {
                Text = "Закрыть",
                Location = new Point(250, 450),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            this.Controls.Add(titleLabel);
            this.Controls.Add(wordsList);
            this.Controls.Add(closeButton);
        }
    }
}