using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Duolingo
{
    public class LessonForm : Form
    {
        private DatabaseHelper dbHelper;
        private User currentUser;
        private int lessonId;
        private List<Word> words;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private int totalQuestions;
        private DateTime startTime;
        private int correctAnswers = 0;
        private int wordsLearnedInLesson = 0;

        private Label questionLabel;
        private TextBox answerTextBox;
        private Button submitButton;
        private Button nextButton;
        private ProgressBar progressBar;
        private Label scoreLabel;
        private Label timerLabel;
        private Label hintLabel;

        public LessonForm(DatabaseHelper dbHelper, User user, int lessonId)
        {
            this.dbHelper = dbHelper;
            this.currentUser = user;
            this.lessonId = lessonId;

            if (currentUser == null)
            {
                MessageBox.Show("Ошибка: пользователь не найден", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            InitializeForm();
            LoadLesson();
        }

        private void InitializeForm()
        {
            this.Text = "Урок - Duolingo";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Padding = new Padding(20);

            startTime = DateTime.Now;

            // Главный контейнер
            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); // Заголовок
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 40));  // Вопрос
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 120)); // Ответ
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 120)); // Контролы
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));  // Подсказка

            // === ВЕРХНЯЯ ПАНЕЛЬ (Заголовок и кнопка выхода) ===
            var topPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var titleLabel = new Label
            {
                Text = "🎯 Урок",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(88, 206, 138),
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true
            };

            var exitButton = new Button
            {
                Text = "✕ Выход",
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent,
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleCenter
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Click += (s, e) => this.Close();

            topPanel.Controls.Add(titleLabel);
            topPanel.Controls.Add(exitButton);

            // === ПАНЕЛЬ ВОПРОСА ===
            var questionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(30)
            };

            questionLabel = new Label
            {
                Font = new Font("Arial", 36, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = false
            };

            questionPanel.Controls.Add(questionLabel);

            // === ПАНЕЛЬ ОТВЕТА ===
            var answerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(30)
            };

            var answerContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            answerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            answerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));

            answerTextBox = new TextBox
            {
                Font = new Font("Arial", 20),
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 10, 0)
            };

            submitButton = new Button
            {
                Text = "Проверить",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(10, 0, 0, 0)
            };
            submitButton.FlatAppearance.BorderSize = 0;
            submitButton.Click += SubmitButton_Click;

            answerContainer.Controls.Add(answerTextBox, 0, 0);
            answerContainer.Controls.Add(submitButton, 1, 0);
            answerPanel.Controls.Add(answerContainer);

            // === ПАНЕЛЬ УПРАВЛЕНИЯ ===
            var controlPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(10, 20, 10, 10)
            };

            // Прогресс-бар
            progressBar = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 25,
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Style = ProgressBarStyle.Continuous
            };

            // Панель для счетов и времени
            var statsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(0, 10, 0, 0)
            };

            scoreLabel = new Label
            {
                Text = "Счет: 0",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(88, 206, 138),
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true
            };

            timerLabel = new Label
            {
                Text = "Время: 00:00",
                Font = new Font("Arial", 12),
                ForeColor = Color.Gray,
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true
            };

            statsPanel.Controls.Add(scoreLabel);
            statsPanel.Controls.Add(timerLabel);

            // Кнопка "Следующий"
            var nextButtonContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Margin = new Padding(0, 10, 0, 0)
            };

            nextButton = new Button
            {
                Text = "Следующий вопрос →",
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Arial", 12),
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            nextButton.FlatAppearance.BorderSize = 0;
            nextButton.Click += NextButton_Click;

            // Центрируем кнопку
            nextButton.Location = new Point(
                (nextButtonContainer.Width - nextButton.Width) / 2,
                (nextButtonContainer.Height - nextButton.Height) / 2
            );

            nextButtonContainer.Controls.Add(nextButton);
            nextButtonContainer.SizeChanged += (s, e) =>
            {
                nextButton.Location = new Point(
                    (nextButtonContainer.Width - nextButton.Width) / 2,
                    (nextButtonContainer.Height - nextButton.Height) / 2
                );
            };

            controlPanel.Controls.Add(progressBar);
            controlPanel.Controls.Add(statsPanel);
            controlPanel.Controls.Add(nextButtonContainer);

            // === ПАНЕЛЬ ПОДСКАЗКИ ===
            var hintPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            hintLabel = new Label
            {
                Text = "Введите перевод слова и нажмите 'Проверить'",
                Font = new Font("Arial", 11),
                ForeColor = Color.Gray,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };

            hintPanel.Controls.Add(hintLabel);

            // === ДОБАВЛЯЕМ ВСЕ В ГЛАВНЫЙ КОНТЕЙНЕР ===
            mainContainer.Controls.Add(topPanel, 0, 0);
            mainContainer.Controls.Add(questionPanel, 0, 1);
            mainContainer.Controls.Add(answerPanel, 0, 2);
            mainContainer.Controls.Add(controlPanel, 0, 3);
            mainContainer.Controls.Add(hintPanel, 0, 4);

            this.Controls.Add(mainContainer);

            // === ТАЙМЕР ОБНОВЛЕНИЯ ВРЕМЕНИ ===
            var timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) =>
            {
                var elapsed = DateTime.Now - startTime;
                timerLabel.Text = $"Время: {elapsed.Minutes:00}:{elapsed.Seconds:00}";
            };
            timer.Start();

            // === ОБРАБОТЧИКИ КЛАВИШ ===
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && submitButton.Visible)
                    SubmitButton_Click(s, e);
                else if (e.KeyCode == Keys.Space && nextButton.Visible)
                    NextButton_Click(s, e);
            };
        }

        private void LoadLesson()
        {
            words = dbHelper.GetWordsForLesson(lessonId);
            totalQuestions = Math.Min(words.Count, 10);

            if (words.Count == 0)
            {
                MessageBox.Show("В этом уроке нет слов для изучения.", "Информация",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            // Перемешиваем слова
            var rnd = new Random();
            words = words.OrderBy(x => rnd.Next()).ToList();

            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            if (currentQuestionIndex >= totalQuestions)
            {
                FinishLesson();
                return;
            }

            var currentWord = words[currentQuestionIndex];
            bool englishToRussian = new Random().Next(0, 2) == 0;

            if (englishToRussian)
            {
                questionLabel.Text = currentWord.English;
                hintLabel.Text = "Введите перевод на русский язык";
            }
            else
            {
                questionLabel.Text = currentWord.Russian;
                hintLabel.Text = "Введите перевод на английский язык";
            }

            answerTextBox.Text = "";
            answerTextBox.Tag = new
            {
                CorrectAnswer = englishToRussian ? currentWord.Russian.ToLower() : currentWord.English.ToLower(),
                WordId = currentWord.Id
            };
            answerTextBox.Focus();

            submitButton.Visible = true;
            nextButton.Visible = false;

            progressBar.Value = (int)((double)currentQuestionIndex / totalQuestions * 100);

            currentQuestionIndex++;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            var tag = answerTextBox.Tag as dynamic;
            if (tag == null) return;

            string userAnswer = answerTextBox.Text.Trim().ToLower();
            string correctAnswer = tag.CorrectAnswer;
            int wordId = tag.WordId;

            bool isCorrect = CheckAnswer(userAnswer, correctAnswer);

            if (isCorrect)
            {
                score += 10;
                correctAnswers++;
                questionLabel.ForeColor = Color.Green;
                hintLabel.Text = "✅ Правильно! +10 очков";

                // Обновляем статистику слова
                dbHelper.UpdateWordPractice(wordId, currentUser.Id);
                wordsLearnedInLesson++;
            }
            else
            {
                questionLabel.ForeColor = Color.Red;
                hintLabel.Text = $"❌ Неправильно. Правильный ответ: {correctAnswer}";
            }

            scoreLabel.Text = $"Счет: {score}";
            submitButton.Visible = false;
            nextButton.Visible = true;

            // Возвращаем цвет через 1 секунду
            var timer = new Timer { Interval = 1000 };
            timer.Tick += (s, ev) =>
            {
                questionLabel.ForeColor = Color.FromArgb(33, 33, 33);
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private bool CheckAnswer(string userAnswer, string correctAnswer)
        {
            userAnswer = userAnswer.Trim().ToLower();
            correctAnswer = correctAnswer.Trim().ToLower();

            string[] correctVariants = correctAnswer.Split(',');

            foreach (string variant in correctVariants)
            {
                if (userAnswer == variant.Trim().ToLower())
                    return true;
            }

            return false;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            ShowNextQuestion();
        }

        private void FinishLesson()
        {
            // Очищаем основную форму
            this.Controls.Clear();

            this.Text = "Результаты урока - Duolingo";
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.Padding = new Padding(40);

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(40)
            };

            var elapsed = DateTime.Now - startTime;
            bool isCompleted = score >= totalQuestions * 7;
            int finalScore = (int)((double)score / (totalQuestions * 10) * 100);

            // Сохраняем результат
            dbHelper.SaveLessonResult(currentUser.Id, lessonId, finalScore, isCompleted, (int)elapsed.TotalMinutes);

            // Начисляем опыт
            int xpEarned = finalScore * 2;
            dbHelper.UpdateUserProgress(currentUser.Id, xpEarned, wordsLearnedInLesson, (int)elapsed.TotalMinutes);

            // Обновляем серию
            dbHelper.UpdateStreak(currentUser.Id);

            // Заголовок результата
            var resultTitle = new Label
            {
                Text = isCompleted ? "🎉 Урок завершен!" : "📝 Урок пройден",
                Font = new Font("Arial", 28, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = isCompleted ? Color.FromArgb(88, 206, 138) : Color.FromArgb(255, 193, 7)
            };

            // Панель с результатами
            var resultsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250,
                Padding = new Padding(0, 20, 0, 20)
            };

            var resultsTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            resultsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            resultsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            var results = new[]
            {
                new { Title = "Набрано очков:", Value = $"{score} из {totalQuestions * 10}" },
                new { Title = "Правильных ответов:", Value = $"{correctAnswers} из {totalQuestions}" },
                new { Title = "Итоговый балл:", Value = $"{finalScore}%" },
                new { Title = "Затраченное время:", Value = $"{elapsed.Minutes:00}:{elapsed.Seconds:00}" },
                new { Title = "Получено опыта:", Value = $"{xpEarned} XP" },
                new { Title = "Выучено слов:", Value = $"{wordsLearnedInLesson}" }
            };

            for (int i = 0; i < results.Length; i++)
            {
                var titleLabel = new Label
                {
                    Text = results[i].Title,
                    Font = new Font("Arial", 12),
                    ForeColor = Color.FromArgb(64, 64, 64),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 5, 5),
                    Height = 35
                };

                var valueLabel = new Label
                {
                    Text = results[i].Value,
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(88, 206, 138),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Padding = new Padding(5, 5, 10, 5),
                    Height = 35
                };

                resultsTable.Controls.Add(titleLabel, 0, i);
                resultsTable.Controls.Add(valueLabel, 1, i);
            }

            resultsPanel.Controls.Add(resultsTable);

            // Прогресс-бар результата
            var progressPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(0, 20, 0, 0)
            };

            var resultProgress = new ProgressBar
            {
                Location = new Point(0, 10),
                Size = new Size(500, 30),
                Minimum = 0,
                Maximum = 100,
                Value = finalScore,
                Style = ProgressBarStyle.Continuous
            };

            var progressLabel = new Label
            {
                Text = $"{finalScore}%",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(510, 10),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            progressPanel.Controls.Add(resultProgress);
            progressPanel.Controls.Add(progressLabel);

            // Панель кнопок
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(0, 20, 0, 0)
            };

            var restartButton = new Button
            {
                Text = "🔄 Повторить урок",
                Size = new Size(180, 45),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Arial", 12),
                FlatStyle = FlatStyle.Flat
            };
            restartButton.FlatAppearance.BorderSize = 0;
            restartButton.Click += (s, e) =>
            {
                // Закрываем эту форму и открываем новую
                this.Close();
                var newLessonForm = new LessonForm(dbHelper, currentUser, lessonId);
                newLessonForm.ShowDialog();
            };

            var closeButton = new Button
            {
                Text = "🏠 В главное меню",
                Size = new Size(180, 45),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 12),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(20, 0, 0, 0)
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(restartButton);
            buttonPanel.Controls.Add(closeButton);

            // Центрируем кнопки
            buttonPanel.SizeChanged += (s, e) =>
            {
                int totalWidth = restartButton.Width + closeButton.Width + 20;
                int startX = (buttonPanel.Width - totalWidth) / 2;
                restartButton.Location = new Point(startX, (buttonPanel.Height - restartButton.Height) / 2);
                closeButton.Location = new Point(startX + restartButton.Width + 20, (buttonPanel.Height - closeButton.Height) / 2);
            };

            // Добавляем все элементы
            mainPanel.Controls.Add(resultTitle);
            mainPanel.Controls.Add(resultsPanel);
            mainPanel.Controls.Add(progressPanel);
            mainPanel.Controls.Add(buttonPanel);

            this.Controls.Add(mainPanel);

            // Показываем сообщение
            if (isCompleted)
            {
                MessageBox.Show($"Поздравляем! Вы завершили урок с результатом {finalScore}%!\n" +
                               $"Получено {xpEarned} XP.\n" +
                               $"Выучено {wordsLearnedInLesson} новых слов!",
                               "Урок завершен",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}