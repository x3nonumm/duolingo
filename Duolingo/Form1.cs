using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Duolingo
{
    public class Form1 : Form
    {
        private DatabaseHelper dbHelper;
        private Panel mainContentPanel;
        private User currentUser;
        private FlowLayoutPanel lessonsFlowPanel;
        private Button btnDailyPractice;
        private Button btnLogout;

        // Элементы верхней панели для обновления
        private Label xpLabel;
        private Label levelLabel;
        private Label streakLabel;
        private Label userNameLabel;

        public Form1()
        {
            // Получаем dbHelper из Program
            dbHelper = Program.GetDatabaseHelper();

            if (dbHelper == null)
            {
                MessageBox.Show("Ошибка инициализации базы данных", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            ShowAuthForm();

            // Подписываемся на событие закрытия формы
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Сохраняем данные при закрытии формы
                if (dbHelper != null)
                {
                    dbHelper.SaveData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных: {ex.Message}",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowAuthForm()
        {
            using (var authForm = new AuthForm(dbHelper))
            {
                var result = authForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Получаем пользователя из формы аутентификации
                    currentUser = authForm.GetAuthenticatedUser();

                    if (currentUser != null)
                    {
                        // Обновляем заголовок формы
                        this.Text = $"Duolingo - {currentUser.Username}";

                        // Если форма уже инициализирована, просто обновляем
                        if (this.Controls.Count > 0)
                        {
                            RefreshUserData();
                            ShowHomeScreen();
                        }
                        else
                        {
                            // Иначе инициализируем форму
                            InitializeMainForm();
                            ShowHomeScreen();
                            this.Show();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка аутентификации", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void InitializeMainForm()
        {
            this.Text = $"Duolingo - {currentUser?.Username ?? "Пользователь"}";
            this.Size = new Size(1100, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Верхняя панель
            var topPanel = new Panel
            {
                BackColor = Color.FromArgb(88, 206, 138),
                Dock = DockStyle.Top,
                Height = 90,
                Padding = new Padding(20, 10, 20, 10)
            };

            // Заголовок слева
            var titleLabel = new Label
            {
                Text = "Duolingo",
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Имя пользователя по центру
            userNameLabel = new Label
            {
                Text = currentUser?.Username ?? "Пользователь",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(400, 15),
                AutoSize = true
            };

            // Панель статистики пользователя
            var userStatsPanel = new Panel
            {
                Location = new Point(400, 40),
                Size = new Size(300, 40),
                BackColor = Color.Transparent
            };

            xpLabel = new Label
            {
                Text = $"⭐ {currentUser?.Experience ?? 0} XP",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 10),
                AutoSize = true
            };

            levelLabel = new Label
            {
                Text = $"📈 Уровень {currentUser?.Level ?? 1}",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(100, 10),
                AutoSize = true
            };

            streakLabel = new Label
            {
                Text = $"🔥 {currentUser?.StreakDays ?? 0} дней",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(200, 10),
                AutoSize = true
            };

            userStatsPanel.Controls.Add(xpLabel);
            userStatsPanel.Controls.Add(levelLabel);
            userStatsPanel.Controls.Add(streakLabel);

            // В методе InitializeMainForm() найдите этот код и исправьте:

            // Панель кнопок справа
            var rightButtonsPanel = new Panel
            {
                Location = new Point(900, 20),
                Size = new Size(160, 50)
            };

            // ========== КНОПКА ТЕОРИИ АНГЛИЙСКОГО ==========
            var theoryButton = new Button
            {
                Text = "📚 Теория",
                Location = new Point(0, 10),
                Size = new Size(85, 30),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(88, 206, 138) // Зеленый Duolingo
            };
            theoryButton.FlatAppearance.BorderSize = 0;
            theoryButton.Click += (s, e) => ShowTheoryForm(); // ИСПРАВЛЕНО!

            // Кнопка выхода
            btnLogout = new Button
            {
                Text = "🚪 Выйти",
                Location = new Point(85, 10),
                Size = new Size(75, 30),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            btnLogout.FlatAppearance.BorderSize = 1;
            btnLogout.FlatAppearance.BorderColor = Color.White;
            btnLogout.Click += (s, e) => Logout();

            rightButtonsPanel.Controls.Add(theoryButton);
            rightButtonsPanel.Controls.Add(btnLogout);

            topPanel.Controls.Add(titleLabel);
            topPanel.Controls.Add(userNameLabel);
            topPanel.Controls.Add(userStatsPanel);
            topPanel.Controls.Add(rightButtonsPanel);

            // Основная панель контента
            mainContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(20)
            };

            // Нижняя панель навигации
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 15, 20, 15)
            };

            // Кнопки навигации с равным распределением
            var navButtons = new[]
            {
                new { Text = "🏠 Главная", Handler = (EventHandler)((s, e) => ShowHomeScreen()) },
                new { Text = "📚 Учить", Handler = (EventHandler)((s, e) => ShowLearnScreen()) },
                new { Text = "⚡ Практика", Handler = (EventHandler)((s, e) => ShowPracticeScreen()) },
                new { Text = "👤 Профиль", Handler = (EventHandler)((s, e) => ShowProfileScreen()) },
                new { Text = "🏆 Рейтинг", Handler = (EventHandler)((s, e) => ShowLeaderboard()) }
            };

            int buttonWidth = (bottomPanel.Width - 40) / navButtons.Length;
            for (int i = 0; i < navButtons.Length; i++)
            {
                var button = CreateNavButton(navButtons[i].Text, i * buttonWidth, buttonWidth);
                button.Click += navButtons[i].Handler;
                bottomPanel.Controls.Add(button);
            }

            // Обработчик изменения размера для перерасчета позиций кнопок
            bottomPanel.SizeChanged += (s, e) =>
            {
                buttonWidth = (bottomPanel.Width - 40) / navButtons.Length;
                for (int i = 0; i < navButtons.Length && i < bottomPanel.Controls.Count; i++)
                {
                    var button = bottomPanel.Controls[i] as Button;
                    if (button != null)
                    {
                        button.Width = buttonWidth - 10;
                        button.Location = new Point(i * buttonWidth + 5, 15);
                    }
                }
            };

            // Добавляем все на форму
            this.Controls.Add(mainContentPanel);
            this.Controls.Add(topPanel);
            this.Controls.Add(bottomPanel);

            // Обновляем статистику при активации формы
            this.Activated += (s, e) => RefreshUserData();
            this.FormClosing += (s, e) =>
            {
                if (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            };
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

        private void Logout()
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти из аккаунта?", "Выход",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Очищаем данные текущего пользователя
                currentUser = null;

                // Очищаем главную панель
                if (mainContentPanel != null)
                {
                    mainContentPanel.Controls.Clear();
                }

                // Показываем форму аутентификации
                ShowAuthForm();
            }
        }

        private void RefreshUserData()
        {
            if (currentUser != null)
            {
                // Всегда получаем свежие данные из базы
                var updatedUser = dbHelper.GetUserById(currentUser.Id);
                if (updatedUser != null)
                {
                    currentUser = updatedUser;
                    UpdateTopPanelStats();

                    // Обновляем заголовок формы
                    this.Text = $"Duolingo - {currentUser.Username}";
                }
            }
        }

        private void UpdateTopPanelStats()
        {
            if (currentUser != null && xpLabel != null && levelLabel != null &&
                streakLabel != null && userNameLabel != null)
            {
                userNameLabel.Text = currentUser.Username;
                xpLabel.Text = $"⭐ {currentUser.Experience} XP";
                levelLabel.Text = $"📈 Уровень {currentUser.Level}";
                streakLabel.Text = $"🔥 {currentUser.StreakDays} дней";
            }
        }

        private Button CreateNavButton(string text, int x, int width)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x + 5, 15),
                Size = new Size(width - 10, 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(88, 206, 138),
                TextAlign = ContentAlignment.MiddleCenter
            };
            button.FlatAppearance.BorderSize = 0;

            // Эффект при наведении
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = Color.FromArgb(240, 255, 240);
                button.ForeColor = Color.FromArgb(66, 176, 116);
            };
            button.MouseLeave += (s, e) =>
            {
                button.BackColor = Color.White;
                button.ForeColor = Color.FromArgb(88, 206, 138);
            };

            return button;
        }

        private void ShowHomeScreen()
        {
            mainContentPanel.Controls.Clear();
            mainContentPanel.BackColor = Color.FromArgb(245, 245, 245);

            // Проверяем currentUser
            if (currentUser == null)
            {
                ShowErrorMessage("Ошибка: пользователь не найден");
                return;
            }

            string userName = currentUser.Username;

            // Приветствие
            var welcomePanel = new Panel
            {
                Location = new Point(30, 30),
                Size = new Size(1020, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var welcomeLabel = new Label
            {
                Text = $"Привет, {userName}!",
                Font = new Font("Arial", 24, FontStyle.Bold),
                Location = new Point(30, 30),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            var dateLabel = new Label
            {
                Text = DateTime.Now.ToString("dddd, d MMMM yyyy"),
                Font = new Font("Arial", 12),
                Location = new Point(30, 65),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            var refreshButton = new Button
            {
                Text = "🔄 Обновить",
                Location = new Point(880, 30),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9),
                ForeColor = Color.FromArgb(88, 206, 138),
                BackColor = Color.Transparent
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += (s, e) => RefreshHomeScreen();

            welcomePanel.Controls.Add(welcomeLabel);
            welcomePanel.Controls.Add(dateLabel);
            welcomePanel.Controls.Add(refreshButton);

            // Быстрые действия
            var actionsPanel = new Panel
            {
                Location = new Point(30, 150),
                Size = new Size(1020, 150)
            };

            var btnStartLesson = new Button
            {
                Text = "🎯 Начать новый урок",
                Location = new Point(0, 0),
                Size = new Size(330, 70),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            btnStartLesson.FlatAppearance.BorderSize = 0;
            btnStartLesson.Click += (s, e) => ShowLearnScreen();

            btnDailyPractice = new Button
            {
                Text = "🔥 Ежедневная практика",
                Location = new Point(345, 0),
                Size = new Size(330, 70),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            btnDailyPractice.FlatAppearance.BorderSize = 0;
            btnDailyPractice.Click += (s, e) => StartDailyPractice();

            var btnQuickReview = new Button
            {
                Text = "📝 Быстрый обзор",
                Location = new Point(690, 0),
                Size = new Size(330, 70),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            btnQuickReview.FlatAppearance.BorderSize = 0;
            btnQuickReview.Click += (s, e) => StartQuickReview();

            actionsPanel.Controls.Add(btnStartLesson);
            actionsPanel.Controls.Add(btnDailyPractice);
            actionsPanel.Controls.Add(btnQuickReview);

            // Статистика
            var statsPanel = new Panel
            {
                Location = new Point(30, 320),
                Size = new Size(1020, 200),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var statsTitle = new Label
            {
                Text = "📊 Ваша статистика",
                Font = new Font("Arial", 18, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            var lastUpdatedLabel = new Label
            {
                Text = $"Обновлено: {DateTime.Now:HH:mm}",
                Font = new Font("Arial", 9),
                Location = new Point(880, 25),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            var stats = dbHelper.GetUserStatistics(currentUser.Id);

            var statsTable = new TableLayoutPanel
            {
                Location = new Point(30, 60),
                Size = new Size(960, 120),
                ColumnCount = 4,
                RowCount = 2
            };

            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            statsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            statsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            for (int i = 0; i < Math.Min(8, stats.Count); i++)
            {
                var statCard = CreateStatCard(stats[i]);
                statsTable.Controls.Add(statCard, i % 4, i / 4);
            }

            statsPanel.Controls.Add(statsTable);

            // Прогресс уровня
            var levelProgressPanel = new Panel
            {
                Location = new Point(30, 190),
                Size = new Size(960, 30)
            };

            var levelProgressLabel = new Label
            {
                Text = $"Прогресс до уровня {currentUser.Level + 1}:",
                Font = new Font("Arial", 10),
                Location = new Point(0, 5),
                AutoSize = true
            };

            var levelProgressBar = new ProgressBar
            {
                Location = new Point(200, 5),
                Size = new Size(400, 20),
                Minimum = 0,
                Maximum = 500,
                Value = Math.Min(currentUser.Experience % 500, 500)
            };

            var levelProgressText = new Label
            {
                Text = $"{currentUser.Experience % 500}/500 XP до уровня {currentUser.Level + 1}",
                Location = new Point(610, 5),
                AutoSize = true,
                Font = new Font("Arial", 9),
                ForeColor = Color.Gray
            };

            levelProgressPanel.Controls.Add(levelProgressLabel);
            levelProgressPanel.Controls.Add(levelProgressBar);
            levelProgressPanel.Controls.Add(levelProgressText);
            statsPanel.Controls.Add(levelProgressPanel);

            statsPanel.Controls.Add(statsTitle);
            statsPanel.Controls.Add(lastUpdatedLabel);

            // Недавние достижения
            if (currentUser.Username != "Гость")
            {
                var achievementsPanel = new Panel
                {
                    Location = new Point(30, 540),
                    Size = new Size(1020, 150),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var achievementsTitle = new Label
                {
                    Text = "🏆 Недавние достижения",
                    Font = new Font("Arial", 18, FontStyle.Bold),
                    Location = new Point(30, 20),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(88, 206, 138)
                };

                var achievements = dbHelper.GetUserAchievements(currentUser.Id)
                    .Where(a => a.IsUnlocked)
                    .OrderByDescending(a => a.UnlockedDate)
                    .Take(3)
                    .ToList();

                if (achievements.Count > 0)
                {
                    int xPos = 30;
                    foreach (var achievement in achievements)
                    {
                        var achPanel = new Panel
                        {
                            Location = new Point(xPos, 60),
                            Size = new Size(300, 70),
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.FromArgb(240, 255, 240)
                        };

                        var achIcon = new Label
                        {
                            Text = achievement.Icon,
                            Font = new Font("Arial", 20),
                            Location = new Point(10, 20),
                            AutoSize = true
                        };

                        var achName = new Label
                        {
                            Text = achievement.Name,
                            Font = new Font("Arial", 11, FontStyle.Bold),
                            Location = new Point(60, 15),
                            AutoSize = true
                        };

                        var achDate = new Label
                        {
                            Text = achievement.UnlockedDate.ToString("dd.MM.yy"),
                            Font = new Font("Arial", 9),
                            Location = new Point(60, 35),
                            AutoSize = true,
                            ForeColor = Color.Gray
                        };

                        achPanel.Controls.Add(achIcon);
                        achPanel.Controls.Add(achName);
                        achPanel.Controls.Add(achDate);
                        achievementsPanel.Controls.Add(achPanel);

                        xPos += 320;
                    }
                }
                else
                {
                    var noAchievementsLabel = new Label
                    {
                        Text = "Начните учиться, чтобы получать достижения!",
                        Font = new Font("Arial", 12),
                        Location = new Point(30, 80),
                        AutoSize = true,
                        ForeColor = Color.Gray
                    };
                    achievementsPanel.Controls.Add(noAchievementsLabel);
                }

                achievementsPanel.Controls.Add(achievementsTitle);
                mainContentPanel.Controls.Add(achievementsPanel);
            }

            mainContentPanel.Controls.Add(welcomePanel);
            mainContentPanel.Controls.Add(actionsPanel);
            mainContentPanel.Controls.Add(statsPanel);

            UpdateDailyPracticeButton();
        }

        private void RefreshHomeScreen()
        {
            RefreshUserData();
            ShowHomeScreen();
        }

        private void ShowErrorMessage(string message)
        {
            var errorLabel = new Label
            {
                Text = message,
                Font = new Font("Arial", 16),
                Location = new Point(50, 50),
                AutoSize = true,
                ForeColor = Color.Red
            };
            mainContentPanel.Controls.Add(errorLabel);
        }

        private Panel CreateStatCard(UserStatistic stat)
        {
            var card = new Panel
            {
                Size = new Size(220, 50),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5)
            };

            var iconLabel = new Label
            {
                Text = stat.Icon,
                Font = new Font("Arial", 18),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var titleLabel = new Label
            {
                Text = stat.Title,
                Font = new Font("Arial", 9),
                Location = new Point(40, 5),
                Size = new Size(150, 15),
                ForeColor = Color.Gray
            };

            var valueLabel = new Label
            {
                Text = stat.Value,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(40, 25),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            card.Controls.Add(iconLabel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return card;
        }

        private void ShowLearnScreen()
        {
            mainContentPanel.Controls.Clear();
            mainContentPanel.BackColor = Color.White;

            // Проверяем currentUser
            if (currentUser == null)
            {
                ShowErrorMessage("Ошибка: пользователь не найден");
                return;
            }

            var titleLabel = new Label
            {
                Text = "📚 Выберите урок",
                Font = new Font("Arial", 24, FontStyle.Bold),
                Location = new Point(30, 30),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            // Обновляем статистику
            RefreshUserData();
            var userStatsLabel = new Label
            {
                Text = $"Ваш уровень: {currentUser.Level} | Опыт: {currentUser.Experience} XP",
                Font = new Font("Arial", 12),
                Location = new Point(30, 70),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            var descriptionLabel = new Label
            {
                Text = "Начните с первого урока и постепенно переходите к более сложным",
                Font = new Font("Arial", 12),
                Location = new Point(30, 95),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            // Панель для уроков с прокруткой
            var scrollPanel = new Panel
            {
                Location = new Point(30, 130),
                Size = new Size(1020, 550),
                AutoScroll = true
            };

            lessonsFlowPanel = new FlowLayoutPanel
            {
                Size = new Size(980, 1000),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true
            };

            var lessons = dbHelper.GetAvailableLessons();

            if (lessons.Count == 0)
            {
                var noLessonsLabel = new Label
                {
                    Text = "Уроки не найдены. Пожалуйста, попробуйте позже.",
                    Font = new Font("Arial", 14),
                    Location = new Point(50, 50),
                    AutoSize = true,
                    ForeColor = Color.Gray
                };
                lessonsFlowPanel.Controls.Add(noLessonsLabel);
            }
            else
            {
                foreach (var lesson in lessons)
                {
                    var lessonCard = CreateLessonCard(lesson);
                    lessonsFlowPanel.Controls.Add(lessonCard);
                }
            }

            scrollPanel.Controls.Add(lessonsFlowPanel);

            mainContentPanel.Controls.Add(titleLabel);
            mainContentPanel.Controls.Add(userStatsLabel);
            mainContentPanel.Controls.Add(descriptionLabel);
            mainContentPanel.Controls.Add(scrollPanel);
        }

        private Panel CreateLessonCard(Lesson lesson)
        {
            var card = new Panel
            {
                Size = new Size(940, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = currentUser.CompletedLessonIds.Contains(lesson.Id) ?
                           Color.FromArgb(240, 255, 240) :
                           Color.White,
                Margin = new Padding(0, 10, 0, 10)
            };

            // Номер урока
            var numberPanel = new RoundedPanel
            {
                Location = new Point(20, 25),
                Size = new Size(50, 50),
                BackColor = currentUser.CompletedLessonIds.Contains(lesson.Id) ?
                           Color.FromArgb(88, 206, 138) :
                           Color.FromArgb(200, 200, 200),
                BorderRadius = 25
            };

            var numberLabel = new Label
            {
                Text = lesson.OrderIndex.ToString(),
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 12),
                AutoSize = true
            };
            numberPanel.Controls.Add(numberLabel);

            // Информация об уроке
            var titleLabel = new Label
            {
                Text = lesson.Title,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(90, 20),
                AutoSize = true
            };

            var descLabel = new Label
            {
                Text = lesson.Description,
                Font = new Font("Arial", 11),
                Location = new Point(90, 45),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            // Статус урока для текущего пользователя
            bool isCompleted = currentUser.CompletedLessonIds.Contains(lesson.Id);
            string statusText = isCompleted ?
                               $"✅ Завершено" :
                               "🔒 Доступно";
            Color statusColor = isCompleted ? Color.Green : Color.Gray;

            var statusLabel = new Label
            {
                Text = statusText,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Location = new Point(90, 65),
                AutoSize = true,
                ForeColor = statusColor
            };

            // Кнопка начала урока
            var startButton = new Button
            {
                Text = isCompleted ? "Повторить" : "Начать",
                Location = new Point(810, 30),
                Size = new Size(100, 40),
                BackColor = isCompleted ?
                           Color.FromArgb(33, 150, 243) :
                           Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Tag = lesson.Id
            };
            startButton.FlatAppearance.BorderSize = 0;
            startButton.Click += (s, e) =>
            {
                var lessonId = (int)((Button)s).Tag;
                StartLesson(lessonId);
            };

            card.Controls.Add(numberPanel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(descLabel);
            card.Controls.Add(statusLabel);
            card.Controls.Add(startButton);

            return card;
        }

        private void StartLesson(int lessonId)
        {
            if (currentUser == null)
            {
                MessageBox.Show("Пожалуйста, войдите в систему", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Получаем свежего пользователя перед запуском урока
            var freshUser = dbHelper.GetUserById(currentUser.Id);
            if (freshUser == null)
            {
                MessageBox.Show("Ошибка загрузки данных пользователя", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Получаем информацию об уроке
            var lessons = dbHelper.GetAvailableLessons();
            var selectedLesson = lessons.FirstOrDefault(l => l.Id == lessonId);

            if (selectedLesson == null)
            {
                MessageBox.Show("Урок не найден", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Запускаем урок с ОБНОВЛЕННЫМ пользователем
            try
            {
                var lessonForm = new LessonForm(dbHelper, freshUser, selectedLesson.Id);
                lessonForm.FormClosed += (s, e) =>
                {
                    // Обновляем данные пользователя после урока
                    RefreshUserData();

                    // Обновляем статус кнопки ежедневной практики
                    UpdateDailyPracticeButton();

                    // Перезагружаем экран уроков, если он открыт
                    if (lessonsFlowPanel != null && lessonsFlowPanel.Visible)
                    {
                        ShowLearnScreen();
                    }
                    else
                    {
                        // Или обновляем домашний экран
                        ShowHomeScreen();
                    }
                };

                lessonForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске урока: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateDailyPracticeButton()
        {
            if (btnDailyPractice != null && currentUser != null)
            {
                bool hasPracticeToday = currentUser.LastPracticeDate.Date == DateTime.Today;
                btnDailyPractice.BackColor = hasPracticeToday ?
                    Color.FromArgb(200, 200, 200) :
                    Color.FromArgb(255, 193, 7);
                btnDailyPractice.Text = hasPracticeToday ?
                    "✅ Ежедневная практика выполнена" :
                    "🔥 Ежедневная практика";
                btnDailyPractice.Enabled = !hasPracticeToday;
            }
        }

        private void ShowPracticeScreen()
        {
            mainContentPanel.Controls.Clear();
            mainContentPanel.BackColor = Color.White;

            // Проверяем currentUser
            if (currentUser == null)
            {
                ShowErrorMessage("Ошибка: пользователь не найден");
                return;
            }

            var titleLabel = new Label
            {
                Text = "⚡ Практика",
                Font = new Font("Arial", 24, FontStyle.Bold),
                Location = new Point(30, 30),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            var descriptionLabel = new Label
            {
                Text = "Выберите тип практики для закрепления знаний",
                Font = new Font("Arial", 12),
                Location = new Point(30, 70),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            // Кнопки практики
            var practicePanel = new Panel
            {
                Location = new Point(30, 120),
                Size = new Size(1020, 400)
            };

            var btnTranslation = CreatePracticeButton("Перевод слов", "Переводите слова с русского на английский и обратно",
                Color.FromArgb(88, 206, 138), 0, 0);
            btnTranslation.Click += (s, e) => StartPractice("Перевод слов");

            var btnListening = CreatePracticeButton("Аудирование", "Слушайте и выбирайте правильный перевод",
                Color.FromArgb(33, 150, 243), 340, 0);
            btnListening.Click += (s, e) => StartPractice("Аудирование");

            var btnGrammar = CreatePracticeButton("Грамматика", "Практикуйте грамматические конструкции",
                Color.FromArgb(255, 193, 7), 680, 0);
            btnGrammar.Click += (s, e) => StartPractice("Грамматика");

            var btnVocabulary = CreatePracticeButton("Словарный запас", "Повторяйте изученные слова",
                Color.FromArgb(156, 39, 176), 0, 150);
            btnVocabulary.Click += (s, e) => StartPractice("Словарный запас");

            var btnSpeed = CreatePracticeButton("Скорость", "Отвечайте на вопросы на время",
                Color.FromArgb(244, 67, 54), 340, 150);
            btnSpeed.Click += (s, e) => StartPractice("Скорость");

            var btnMixed = CreatePracticeButton("Смешанная", "Разные типы заданий в одном упражнении",
                Color.FromArgb(76, 175, 80), 680, 150);
            btnMixed.Click += (s, e) => StartPractice("Смешанная");

            practicePanel.Controls.Add(btnTranslation);
            practicePanel.Controls.Add(btnListening);
            practicePanel.Controls.Add(btnGrammar);
            practicePanel.Controls.Add(btnVocabulary);
            practicePanel.Controls.Add(btnSpeed);
            practicePanel.Controls.Add(btnMixed);

            mainContentPanel.Controls.Add(titleLabel);
            mainContentPanel.Controls.Add(descriptionLabel);
            mainContentPanel.Controls.Add(practicePanel);
        }

        private Button CreatePracticeButton(string title, string description, Color color, int x, int y)
        {
            var button = new Button
            {
                Text = $"{title}\n\n{description}",
                Location = new Point(x, y),
                Size = new Size(320, 130),
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void StartPractice(string practiceType)
        {
            if (currentUser == null)
            {
                MessageBox.Show("Пожалуйста, войдите в систему", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Получаем свежего пользователя
            var freshUser = dbHelper.GetUserById(currentUser.Id);
            if (freshUser == null)
            {
                MessageBox.Show("Ошибка загрузки данных пользователя", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var practiceForm = new PracticeForm(dbHelper, freshUser, practiceType);
                practiceForm.FormClosed += (s, e) =>
                {
                    RefreshUserData();
                    UpdateDailyPracticeButton();
                };
                practiceForm.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Форма практики временно недоступна", "Информация",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void StartDailyPractice()
        {
            StartPractice("Ежедневная практика");
        }

        private void StartQuickReview()
        {
            StartPractice("Быстрый обзор");
        }

        private void ShowProfileScreen()
        {
            if (currentUser == null)
            {
                MessageBox.Show("Пользователь не найден", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Получаем свежего пользователя
            var freshUser = dbHelper.GetUserById(currentUser.Id);
            if (freshUser == null)
            {
                MessageBox.Show("Ошибка загрузки данных пользователя", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var profileForm = new ProfileForm(dbHelper, freshUser);
            var result = profileForm.ShowDialog();

            if (result == DialogResult.Retry) // Смена аккаунта
            {
                this.Hide();
                ShowAuthForm();
            }
            else if (result == DialogResult.Abort) // Удаление аккаунта
            {
                this.Hide();
                ShowAuthForm();
            }
            else
            {
                RefreshUserData();
            }
        }

        private void ShowDictionary()
        {
            mainContentPanel.Controls.Clear();
            mainContentPanel.BackColor = Color.White;

            // Проверяем currentUser
            if (currentUser == null)
            {
                ShowErrorMessage("Ошибка: пользователь не найден");
                return;
            }

            var titleLabel = new Label
            {
                Text = "📖 Словарь",
                Font = new Font("Arial", 24, FontStyle.Bold),
                Location = new Point(30, 30),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            var descriptionLabel = new Label
            {
                Text = "Просматривайте и изучайте слова из вашего словаря",
                Font = new Font("Arial", 12),
                Location = new Point(30, 70),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            // Если пользователь - гость
            if (currentUser.Username == "Гость")
            {
                var messageLabel = new Label
                {
                    Text = "Эта функция доступна только для зарегистрированных пользователей.\n\n" +
                           "Зарегистрируйтесь, чтобы:\n" +
                           "• Сохранять свой прогресс\n" +
                           "• Изучать слова\n" +
                           "• Получать достижения\n" +
                           "• Соревноваться с другими",
                    Font = new Font("Arial", 14),
                    Location = new Point(100, 150),
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var registerButton = new Button
                {
                    Text = "Зарегистрироваться",
                    Location = new Point(350, 300),
                    Size = new Size(200, 40),
                    BackColor = Color.FromArgb(88, 206, 138),
                    ForeColor = Color.White,
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat
                };
                registerButton.FlatAppearance.BorderSize = 0;
                registerButton.Click += (s, e) =>
                {
                    this.Hide();
                    ShowAuthForm();
                };

                mainContentPanel.Controls.Add(messageLabel);
                mainContentPanel.Controls.Add(registerButton);
            }
            else
            {
                // Реализация словаря для зарегистрированных пользователей
                var searchBox = CreateSearchBoxWithPlaceholder("Поиск слов...");
                searchBox.Location = new Point(30, 110);
                searchBox.Size = new Size(700, 30);
                searchBox.Font = new Font("Arial", 12);

                var searchButton = new Button
                {
                    Text = "🔍",
                    Location = new Point(740, 110),
                    Size = new Size(40, 30),
                    FlatStyle = FlatStyle.Flat
                };
                searchButton.FlatAppearance.BorderSize = 0;

                var wordsList = new ListBox
                {
                    Location = new Point(30, 150),
                    Size = new Size(1020, 400),
                    Font = new Font("Arial", 11)
                };

                // Загружаем слова пользователя
                var userWords = dbHelper.GetRecentlyLearnedWords(currentUser.Id, 100);
                foreach (var word in userWords)
                {
                    wordsList.Items.Add(word);
                }

                searchButton.Click += (s, e) =>
                {
                    wordsList.Items.Clear();
                    var searchText = searchBox.Text.ToLower();

                    // Проверяем, не является ли текст placeholder'ом
                    if (searchText == "поиск слов..." || string.IsNullOrWhiteSpace(searchText))
                    {
                        foreach (var word in userWords)
                        {
                            wordsList.Items.Add(word);
                        }
                    }
                    else
                    {
                        var filteredWords = userWords.Where(w => w.ToLower().Contains(searchText));
                        foreach (var word in filteredWords)
                        {
                            wordsList.Items.Add(word);
                        }
                    }
                };

                searchBox.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        searchButton.PerformClick();
                    }
                };

                mainContentPanel.Controls.Add(searchBox);
                mainContentPanel.Controls.Add(searchButton);
                mainContentPanel.Controls.Add(wordsList);
            }

            mainContentPanel.Controls.Add(titleLabel);
            mainContentPanel.Controls.Add(descriptionLabel);
        }

        // Метод для создания TextBox с имитацией placeholder
        private TextBox CreateSearchBoxWithPlaceholder(string placeholderText)
        {
            var textBox = new TextBox
            {
                Text = placeholderText,
                ForeColor = Color.Gray
            };

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholderText;
                    textBox.ForeColor = Color.Gray;
                }
            };

            return textBox;
        }

        // ВАЖНО: Обновленный метод ShowLeaderboard() для отображения всех зарегистрированных пользователей
        private void ShowLeaderboard()
        {
            mainContentPanel.Controls.Clear();
            mainContentPanel.BackColor = Color.White;

            // Проверяем currentUser
            if (currentUser == null)
            {
                ShowErrorMessage("Ошибка: пользователь не найден");
                return;
            }

            var titleLabel = new Label
            {
                Text = "🏆 Рейтинг",
                Font = new Font("Arial", 24, FontStyle.Bold),
                Location = new Point(30, 30),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            var descriptionLabel = new Label
            {
                Text = "Сравните свои результаты с другими пользователями",
                Font = new Font("Arial", 12),
                Location = new Point(30, 70),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            // Кнопка обновления рейтинга
            var refreshButton = new Button
            {
                Text = "🔄 Обновить",
                Location = new Point(900, 30),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9),
                ForeColor = Color.FromArgb(88, 206, 138),
                BackColor = Color.Transparent
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += (s, e) => RefreshLeaderboard();

            // Получаем список пользователей (включая гостя)
            var users = dbHelper.GetAllUsers();

            if (users.Count == 0)
            {
                var noUsersLabel = new Label
                {
                    Text = "Нет данных для отображения\n\nЗарегистрируйтесь, чтобы попасть в рейтинг!",
                    Font = new Font("Arial", 14),
                    Location = new Point(100, 150),
                    AutoSize = true,
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var registerButton = new Button
                {
                    Text = "Зарегистрироваться",
                    Location = new Point(350, 250),
                    Size = new Size(200, 40),
                    BackColor = Color.FromArgb(88, 206, 138),
                    ForeColor = Color.White,
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat
                };
                registerButton.FlatAppearance.BorderSize = 0;
                registerButton.Click += (s, e) =>
                {
                    this.Hide();
                    ShowAuthForm();
                };

                mainContentPanel.Controls.Add(noUsersLabel);
                mainContentPanel.Controls.Add(registerButton);
                mainContentPanel.Controls.Add(titleLabel);
                mainContentPanel.Controls.Add(descriptionLabel);
                mainContentPanel.Controls.Add(refreshButton);
                return;
            }

            // Создаем таблицу рейтинга
            var scrollPanel = new Panel
            {
                Location = new Point(30, 110),
                Size = new Size(1020, 600),
                AutoScroll = true
            };

            var leaderboardPanel = new FlowLayoutPanel
            {
                Size = new Size(980, users.Count * 80),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            // Показываем информацию о текущем пользователе
            var currentUserInfo = new Panel
            {
                Size = new Size(960, 80),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 255, 240),
                Margin = new Padding(0, 10, 0, 10)
            };

            var currentUserPosition = users.FindIndex(u => u.Id == currentUser.Id) + 1;
            if (currentUserPosition == 0) currentUserPosition = users.Count + 1;

            var currentUserLabel = new Label
            {
                Text = $"Ваше место в рейтинге: #{currentUserPosition}",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 25),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            var currentUserStats = new Label
            {
                Text = $"{currentUser.Username} | Уровень {currentUser.Level} | {currentUser.Experience} XP | {currentUser.WordsLearned} слов",
                Font = new Font("Arial", 11),
                Location = new Point(20, 50),
                AutoSize = true
            };

            currentUserInfo.Controls.Add(currentUserLabel);
            currentUserInfo.Controls.Add(currentUserStats);
            leaderboardPanel.Controls.Add(currentUserInfo);

            // Добавляем разделитель
            var separator = new Panel
            {
                Size = new Size(960, 2),
                BackColor = Color.LightGray,
                Margin = new Padding(0, 10, 0, 10)
            };
            leaderboardPanel.Controls.Add(separator);

            // Заголовок таблицы
            var tableHeader = new Panel
            {
                Size = new Size(960, 40),
                BackColor = Color.FromArgb(88, 206, 138),
                Margin = new Padding(0, 10, 0, 10)
            };

            var headerPosition = new Label
            {
                Text = "Место",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(20, 10),
                AutoSize = true,
                ForeColor = Color.White
            };

            var headerUser = new Label
            {
                Text = "Пользователь",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(120, 10),
                AutoSize = true,
                ForeColor = Color.White
            };

            var headerStats = new Label
            {
                Text = "Статистика",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(350, 10),
                AutoSize = true,
                ForeColor = Color.White
            };

            var headerXP = new Label
            {
                Text = "Опыт",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(650, 10),
                AutoSize = true,
                ForeColor = Color.White
            };

            tableHeader.Controls.Add(headerPosition);
            tableHeader.Controls.Add(headerUser);
            tableHeader.Controls.Add(headerStats);
            tableHeader.Controls.Add(headerXP);
            leaderboardPanel.Controls.Add(tableHeader);

            // Добавляем пользователей в рейтинг
            int position = 1;
            foreach (var user in users)
            {
                var userCard = CreateLeaderboardCard(user, position);
                leaderboardPanel.Controls.Add(userCard);
                position++;
            }

            scrollPanel.Controls.Add(leaderboardPanel);

            mainContentPanel.Controls.Add(titleLabel);
            mainContentPanel.Controls.Add(descriptionLabel);
            mainContentPanel.Controls.Add(refreshButton);
            mainContentPanel.Controls.Add(scrollPanel);
        }

        private void RefreshLeaderboard()
        {
            ShowLeaderboard();
        }

        private Panel CreateLeaderboardCard(User user, int position)
        {
            var card = new Panel
            {
                Size = new Size(960, 70),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = user.Id == currentUser?.Id ?
                           Color.FromArgb(240, 255, 240) :
                           Color.White,
                Margin = new Padding(0, 5, 0, 5)
            };

            // Медаль для первых трех мест
            string medal = "";
            if (position == 1) medal = "🥇";
            else if (position == 2) medal = "🥈";
            else if (position == 3) medal = "🥉";

            // Позиция
            var positionLabel = new Label
            {
                Text = $"{position}. {medal}",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = position <= 3 ? Color.Gold : Color.Gray
            };

            // Аватар пользователя
            var avatarPanel = new RoundedPanel
            {
                Location = new Point(80, 10),
                Size = new Size(50, 50),
                BackColor = user.Id == currentUser?.Id ?
                           Color.FromArgb(88, 206, 138) :
                           Color.FromArgb(100, 100, 100),
                BorderRadius = 25
            };

            var avatarText = new Label
            {
                Text = GetInitials(user.Username),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                AutoSize = true
            };
            avatarPanel.Controls.Add(avatarText);

            // Имя пользователя
            var nameLabel = new Label
            {
                Text = user.Username,
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(140, 15),
                AutoSize = true
            };

            // Статистика
            var statsLabel = new Label
            {
                Text = $"Уровень {user.Level} • {user.WordsLearned} слов • {user.StreakDays} дней серия",
                Font = new Font("Arial", 10),
                Location = new Point(140, 40),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            // Опыт
            var xpLabel = new Label
            {
                Text = $"{user.Experience} XP",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(650, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            // Если это текущий пользователь
            if (user.Id == currentUser?.Id)
            {
                var youLabel = new Label
                {
                    Text = "Вы",
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    Location = new Point(140, 0),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(88, 206, 138)
                };
                card.Controls.Add(youLabel);
            }

            card.Controls.Add(positionLabel);
            card.Controls.Add(avatarPanel);
            card.Controls.Add(nameLabel);
            card.Controls.Add(statsLabel);
            card.Controls.Add(xpLabel);

            return card;
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}