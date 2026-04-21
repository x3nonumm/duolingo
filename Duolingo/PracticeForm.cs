using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Duolingo
{
    public class PracticeForm : Form
    {
        private DatabaseHelper dbHelper;
        private User currentUser;
        private string practiceType;
        private List<Word> words;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private int totalQuestions = 10;
        private DateTime startTime;
        private int wordsPracticedInSession = 0;

        private Label questionLabel;
        private TextBox answerTextBox;
        private List<Button> optionButtons;
        private Button submitButton;
        private Button nextButton;
        private ProgressBar progressBar;
        private Label scoreLabel;
        private Label timerLabel;
        private Label typeLabel;
        private Label resultLabel;

        // Конструктор для обратной совместимости (старый код)
        public PracticeForm(DatabaseHelper dbHelper)
        {
            this.dbHelper = dbHelper;
            InitializeSimpleForm();
        }

        // Новый конструктор с полной функциональностью
        public PracticeForm(DatabaseHelper dbHelper, User user, string practiceType)
        {
            this.dbHelper = dbHelper;
            this.currentUser = user; // Получаем уже обновленного пользователя
            this.practiceType = practiceType;

            // Проверяем пользователя
            if (currentUser == null)
            {
                MessageBox.Show("Ошибка: пользователь не найден", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            InitializeForm();
            LoadPractice();
        }

        private void InitializeSimpleForm()
        {
            this.Text = "Практика слов";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "⚡ Практика",
                Font = new Font("Arial", 20, FontStyle.Bold),
                Location = new Point(100, 50),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            var infoLabel = new Label
            {
                Text = "Эта функция доступна\nв новой версии приложения",
                Font = new Font("Arial", 12),
                Location = new Point(80, 100),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var closeButton = new Button
            {
                Text = "Закрыть",
                Location = new Point(150, 180),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            this.Controls.Add(titleLabel);
            this.Controls.Add(infoLabel);
            this.Controls.Add(closeButton);
        }

        private void InitializeForm()
        {
            this.Text = $"Практика: {practiceType}";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            startTime = DateTime.Now;

            // Заголовок
            var titleLabel = new Label
            {
                Text = $"⚡ {practiceType}",
                Font = new Font("Arial", 24, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            typeLabel = new Label
            {
                Text = GetPracticeDescription(),
                Font = new Font("Arial", 12),
                Location = new Point(30, 60),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            // Панель вопроса
            var questionPanel = new Panel
            {
                Location = new Point(30, 100),
                Size = new Size(740, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            questionLabel = new Label
            {
                Font = new Font("Arial", 28, FontStyle.Bold),
                Location = new Point(50, 40),
                Size = new Size(640, 70),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(33, 33, 33)
            };

            questionPanel.Controls.Add(questionLabel);

            // Панель ответа
            var answerPanel = new Panel
            {
                Location = new Point(30, 270),
                Size = new Size(740, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            if (practiceType == "Перевод слов" || practiceType == "Тест на время" || practiceType == "Аудирование")
            {
                answerTextBox = new TextBox
                {
                    Font = new Font("Arial", 20),
                    Location = new Point(20, 30),
                    Size = new Size(600, 40),
                    BorderStyle = BorderStyle.FixedSingle
                };
                answerPanel.Height = 100;
                answerPanel.Controls.Add(answerTextBox);
            }
            else if (practiceType == "Выбор ответа" || practiceType == "Сопоставление")
            {
                optionButtons = new List<Button>();
                int buttonCount = practiceType == "Выбор ответа" ? 4 : 3;

                for (int i = 0; i < buttonCount; i++)
                {
                    var button = new Button
                    {
                        Tag = i,
                        Size = new Size(340, 40),
                        Font = new Font("Arial", 14),
                        BackColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Padding = new Padding(10, 0, 0, 0)
                    };
                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.BorderColor = Color.LightGray;
                    button.Click += OptionButton_Click;

                    optionButtons.Add(button);
                }

                // Располагаем кнопки
                if (practiceType == "Выбор ответа")
                {
                    optionButtons[0].Location = new Point(20, 20);
                    optionButtons[1].Location = new Point(380, 20);
                    optionButtons[2].Location = new Point(20, 70);
                    optionButtons[3].Location = new Point(380, 70);
                }
                else // Сопоставление
                {
                    answerPanel.Height = 150;
                    optionButtons[0].Location = new Point(20, 20);
                    optionButtons[1].Location = new Point(20, 70);
                    optionButtons[2].Location = new Point(20, 120);
                }

                foreach (var button in optionButtons)
                {
                    answerPanel.Controls.Add(button);
                }
            }

            // Кнопка проверки
            submitButton = new Button
            {
                Text = "Проверить",
                Size = new Size(90, 40),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            if (practiceType == "Выбор ответа")
            {
                submitButton.Location = new Point(630, 130);
            }
            else if (practiceType == "Сопоставление")
            {
                submitButton.Location = new Point(630, 120);
            }
            else
            {
                submitButton.Location = new Point(630, 25);
            }

            submitButton.FlatAppearance.BorderSize = 0;
            submitButton.Click += SubmitButton_Click;

            answerPanel.Controls.Add(submitButton);

            // Панель управления
            int controlPanelY = practiceType == "Выбор ответа" ? 490 :
                               practiceType == "Сопоставление" ? 440 : 390;

            var controlPanel = new Panel
            {
                Location = new Point(30, controlPanelY),
                Size = new Size(740, 100)
            };

            progressBar = new ProgressBar
            {
                Location = new Point(20, 10),
                Size = new Size(700, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            scoreLabel = new Label
            {
                Text = "Счет: 0",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 40),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            timerLabel = new Label
            {
                Text = "Время: 00:00",
                Font = new Font("Arial", 12),
                Location = new Point(600, 40),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            nextButton = new Button
            {
                Text = "Следующий вопрос →",
                Location = new Point(300, 35),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Arial", 12),
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            nextButton.FlatAppearance.BorderSize = 0;
            nextButton.Click += NextButton_Click;

            controlPanel.Controls.Add(progressBar);
            controlPanel.Controls.Add(scoreLabel);
            controlPanel.Controls.Add(timerLabel);
            controlPanel.Controls.Add(nextButton);

            // Метка для результата
            resultLabel = new Label
            {
                Font = new Font("Arial", 12),
                Location = new Point(30, controlPanelY - 30),
                Size = new Size(740, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            // Кнопка выхода
            var exitButton = new Button
            {
                Text = "✕ Выход",
                Location = new Point(670, 20),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Click += (s, e) => this.Close();

            // Таймер обновления времени
            var timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) =>
            {
                var elapsed = DateTime.Now - startTime;
                timerLabel.Text = $"Время: {elapsed.Minutes:00}:{elapsed.Seconds:00}";

                // Для теста на время добавляем давление
                if (practiceType == "Тест на время" && elapsed.TotalSeconds > 60)
                {
                    FinishPractice();
                }
            };
            timer.Start();

            // Добавляем элементы
            this.Controls.Add(titleLabel);
            this.Controls.Add(typeLabel);
            this.Controls.Add(exitButton);
            this.Controls.Add(questionPanel);
            this.Controls.Add(answerPanel);
            this.Controls.Add(resultLabel);
            this.Controls.Add(controlPanel);

            // Назначаем обработчики клавиш
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && submitButton.Visible)
                    SubmitButton_Click(s, e);
                else if (e.KeyCode == Keys.Space && nextButton.Visible)
                    NextButton_Click(s, e);
            };
        }

        private string GetPracticeDescription()
        {
            if (practiceType == "Перевод слов")
                return "Введите перевод слова";
            else if (practiceType == "Выбор ответа")
                return "Выберите правильный перевод";
            else if (practiceType == "Сопоставление")
                return "Сопоставьте слова с переводами";
            else if (practiceType == "Тест на время")
                return "Решайте задания как можно быстрее!";
            else if (practiceType == "Аудирование")
                return "Слушайте и пишите услышанное";
            else if (practiceType == "Чтение")
                return "Читайте тексты и отвечайте на вопросы";
            else
                return "Закрепляйте изученный материал";
        }

        private void LoadPractice()
        {
            if (currentUser == null || dbHelper == null)
            {
                MessageBox.Show("Не удалось загрузить слова для практики", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            try
            {
                // Получаем случайные слова для практики
                var allWords = dbHelper.GetWordsForLesson(0); // 0 = все слова
                if (allWords == null || allWords.Count == 0)
                {
                    MessageBox.Show("В базе данных нет слов для практики", "Информация",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                var rnd = new Random();
                words = allWords.OrderBy(x => rnd.Next()).Take(totalQuestions).ToList();

                ShowNextQuestion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке слов: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void ShowNextQuestion()
        {
            if (currentQuestionIndex >= totalQuestions || words == null || currentQuestionIndex >= words.Count)
            {
                FinishPractice();
                return;
            }

            var currentWord = words[currentQuestionIndex];
            bool englishToRussian = new Random().Next(0, 2) == 0;

            resultLabel.Visible = false;

            if (practiceType == "Перевод слов" || practiceType == "Тест на время" || practiceType == "Аудирование")
            {
                questionLabel.Text = englishToRussian ? currentWord.English : currentWord.Russian;
                if (answerTextBox != null)
                {
                    answerTextBox.Text = "";
                    var tagData = new PracticeWordData
                    {
                        CorrectAnswer = englishToRussian ? currentWord.Russian.ToLower() : currentWord.English.ToLower(),
                        WordId = currentWord.Id,
                        Word = currentWord
                    };
                    answerTextBox.Tag = tagData;
                    answerTextBox.BackColor = Color.White;
                    answerTextBox.Focus();
                }
            }
            else if (practiceType == "Выбор ответа")
            {
                questionLabel.Text = englishToRussian ? currentWord.English : currentWord.Russian;

                // Создаем варианты ответов
                var correctAnswer = englishToRussian ? currentWord.Russian : currentWord.English;
                var wrongAnswers = GetWrongAnswers(currentWord, englishToRussian);
                var allAnswers = new List<string> { correctAnswer };
                allAnswers.AddRange(wrongAnswers);

                // Перемешиваем
                var rnd = new Random();
                allAnswers = allAnswers.OrderBy(x => rnd.Next()).ToList();

                for (int i = 0; i < 4; i++)
                {
                    var optionData = new PracticeOptionData
                    {
                        Text = allAnswers[i],
                        IsCorrect = allAnswers[i] == correctAnswer,
                        WordId = currentWord.Id,
                        Word = currentWord
                    };
                    optionButtons[i].Text = $"{(char)('A' + i)}) {allAnswers[i]}";
                    optionButtons[i].Tag = optionData;
                    optionButtons[i].BackColor = Color.White;
                    optionButtons[i].Enabled = true;
                }
            }
            else if (practiceType == "Сопоставление")
            {
                // Для сопоставления показываем несколько слов
                var wordsToMatch = new List<Word>();

                // Начинаем с текущего индекса
                int startIndex = currentQuestionIndex;
                for (int i = 0; i < 3 && (startIndex + i) < words.Count; i++)
                {
                    wordsToMatch.Add(words[startIndex + i]);
                }

                // Если не хватает слов, берем случайные
                if (wordsToMatch.Count < 3)
                {
                    var allWords = dbHelper.GetWordsForLesson(0);
                    var rnd = new Random();
                    while (wordsToMatch.Count < 3)
                    {
                        int randomIndex = rnd.Next(allWords.Count);
                        var randomWord = allWords[randomIndex];
                        if (!wordsToMatch.Any(w => w.Id == randomWord.Id))
                            wordsToMatch.Add(randomWord);
                    }
                }

                questionLabel.Text = "Сопоставьте переводы:";

                for (int i = 0; i < 3; i++)
                {
                    var word = wordsToMatch[i];
                    bool showEnglish = new Random().Next(0, 2) == 0;
                    var matchData = new PracticeMatchData
                    {
                        DisplayText = showEnglish ? word.English : word.Russian,
                        CorrectMatch = showEnglish ? word.Russian : word.English,
                        WordId = word.Id,
                        Word = word
                    };
                    optionButtons[i].Text = showEnglish ? word.English : word.Russian;
                    optionButtons[i].Tag = matchData;
                    optionButtons[i].BackColor = Color.White;
                    optionButtons[i].Enabled = true;
                }
            }

            submitButton.Visible = true;
            nextButton.Visible = false;
            progressBar.Value = (int)((double)currentQuestionIndex / totalQuestions * 100);

            currentQuestionIndex++;
        }

        private List<string> GetWrongAnswers(Word correctWord, bool englishToRussian)
        {
            try
            {
                var allWords = dbHelper.GetWordsForLesson(0);
                var rnd = new Random();
                var wrongWords = allWords
                    .Where(w => w.Id != correctWord.Id)
                    .OrderBy(x => rnd.Next())
                    .Take(3)
                    .ToList();

                return wrongWords.Select(w => englishToRussian ? w.Russian : w.English).ToList();
            }
            catch
            {
                // Возвращаем заглушки в случае ошибки
                return new List<string> { "Вариант 1", "Вариант 2", "Вариант 3" };
            }
        }

        private void OptionButton_Click(object sender, EventArgs e)
        {
            if (practiceType != "Выбор ответа" && practiceType != "Сопоставление")
                return;

            var button = (Button)sender;

            if (practiceType == "Выбор ответа")
            {
                var optionData = button.Tag as PracticeOptionData;
                if (optionData == null) return;

                bool isCorrect = optionData.IsCorrect;

                foreach (var btn in optionButtons)
                {
                    var btnData = btn.Tag as PracticeOptionData;
                    if (btnData != null)
                    {
                        if (btnData.IsCorrect)
                        {
                            btn.BackColor = Color.FromArgb(144, 238, 144); // Светло-зеленый для правильного
                        }
                        else
                        {
                            btn.BackColor = Color.FromArgb(255, 182, 193); // Светло-красный для неправильного
                        }
                        btn.Enabled = false;
                    }
                }

                if (isCorrect)
                {
                    score += 10;
                    scoreLabel.Text = $"Счет: {score}";
                    resultLabel.Text = "✅ Правильно! +10 очков";
                    resultLabel.ForeColor = Color.Green;

                    // Обновляем статистику слова
                    if (optionData.Word != null)
                    {
                        dbHelper.UpdateWordPractice(optionData.WordId, currentUser.Id);
                        wordsPracticedInSession++;
                    }
                }
                else
                {
                    resultLabel.Text = "❌ Неправильно!";
                    resultLabel.ForeColor = Color.Red;
                }
                resultLabel.Visible = true;

                submitButton.Visible = false;
                nextButton.Visible = true;
            }
            else if (practiceType == "Сопоставление")
            {
                // Для сопоставления это более сложная логика
                submitButton.Visible = false;
                nextButton.Visible = true;
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (practiceType == "Перевод слов" || practiceType == "Тест на время" || practiceType == "Аудирование")
            {
                if (answerTextBox == null) return;

                var tagData = answerTextBox.Tag as PracticeWordData;
                if (tagData == null) return;

                string userAnswer = answerTextBox.Text.Trim().ToLower();
                string correctAnswer = tagData.CorrectAnswer;

                bool isCorrect = CheckAnswer(userAnswer, correctAnswer);

                if (isCorrect)
                {
                    score += 10;
                    answerTextBox.BackColor = Color.FromArgb(144, 238, 144);
                    resultLabel.Text = "✅ Правильно! +10 очков";
                    resultLabel.ForeColor = Color.Green;

                    // Обновляем статистику слова
                    if (tagData.Word != null)
                    {
                        dbHelper.UpdateWordPractice(tagData.WordId, currentUser.Id);
                        wordsPracticedInSession++;
                    }
                }
                else
                {
                    answerTextBox.BackColor = Color.FromArgb(255, 182, 193);
                    resultLabel.Text = $"❌ Неправильно! Правильно: {correctAnswer}";
                    resultLabel.ForeColor = Color.Red;
                }

                scoreLabel.Text = $"Счет: {score}";
                resultLabel.Visible = true;
            }

            submitButton.Visible = false;
            nextButton.Visible = true;
        }

        private bool CheckAnswer(string userAnswer, string correctAnswer)
        {
            if (string.IsNullOrEmpty(userAnswer) || string.IsNullOrEmpty(correctAnswer))
                return false;

            // Убираем лишние пробелы
            userAnswer = userAnswer.Trim().ToLower();
            correctAnswer = correctAnswer.Trim().ToLower();

            // Разрешаем несколько вариантов ответа (разделенных запятыми или точкой с запятой)
            char[] separators = new char[] { ',', ';' };
            string[] correctVariants = correctAnswer.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string variant in correctVariants)
            {
                if (userAnswer == variant.Trim().ToLower())
                    return true;
            }

            // Проверяем похожие ответы (можно добавить более сложную логику)
            if (userAnswer.Replace(" ", "") == correctAnswer.Replace(" ", ""))
                return true;

            return false;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (answerTextBox != null)
            {
                answerTextBox.BackColor = Color.White;
            }

            if (optionButtons != null)
            {
                foreach (var button in optionButtons)
                {
                    button.BackColor = Color.White;
                }
            }

            resultLabel.Visible = false;
            ShowNextQuestion();
        }

        private void FinishPractice()
        {
            // Скрываем элементы вопроса
            questionLabel.Visible = false;
            if (answerTextBox != null) answerTextBox.Visible = false;
            if (optionButtons != null)
            {
                foreach (var button in optionButtons)
                    button.Visible = false;
            }
            submitButton.Visible = false;
            nextButton.Visible = false;
            progressBar.Visible = false;
            timerLabel.Visible = false;
            typeLabel.Visible = false;

            // Показываем результаты
            var resultPanel = new Panel
            {
                Location = new Point(100, 100),
                Size = new Size(600, 400),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var elapsed = DateTime.Now - startTime;
            int finalScore = (int)((double)score / (totalQuestions * 10) * 100);

            // Сохраняем прогресс пользователя
            if (currentUser != null)
            {
                try
                {
                    int xpEarned = score / 2; // Опыт = половина очков
                    int wordsCount = wordsPracticedInSession;

                    // Обновляем прогресс пользователя
                    dbHelper.UpdateUserProgress(currentUser.Id, xpEarned, wordsCount, (int)elapsed.TotalMinutes);
                    dbHelper.UpdateStreak(currentUser.Id);

                    // Получаем обновленные данные пользователя
                    var updatedUser = dbHelper.GetUserById(currentUser.Id);

                    // Текст результата
                    var resultTitle = new Label
                    {
                        Text = "🏁 Практика завершена!",
                        Font = new Font("Arial", 28, FontStyle.Bold),
                        Location = new Point(150, 50),
                        AutoSize = true,
                        ForeColor = Color.FromArgb(88, 206, 138)
                    };

                    var resultText = new Label
                    {
                        Text = $"🎯 Тип практики: {practiceType}\n" +
                               $"📊 Очки: {score} из {totalQuestions * 10}\n" +
                               $"⭐ Итоговый балл: {finalScore}%\n" +
                               $"⏱️ Время: {elapsed.Minutes:00}:{elapsed.Seconds:00}\n" +
                               $"📈 Получено опыта: {xpEarned} XP\n" +
                               $"🔥 Серия дней: {updatedUser.StreakDays}\n" +
                               $"📚 Практиковано слов: {wordsPracticedInSession}\n" +
                               $"🎯 Всего опыта: {updatedUser.Experience} XP\n" +
                               $"🏆 Уровень: {updatedUser.Level}",
                        Font = new Font("Arial", 16),
                        Location = new Point(150, 120),
                        AutoSize = true
                    };

                    // Прогресс бар результата
                    var resultProgress = new ProgressBar
                    {
                        Location = new Point(150, 220),
                        Size = new Size(300, 30),
                        Value = finalScore,
                        Style = ProgressBarStyle.Continuous
                    };

                    var progressLabel = new Label
                    {
                        Text = $"{finalScore}%",
                        Font = new Font("Arial", 14, FontStyle.Bold),
                        Location = new Point(460, 220),
                        AutoSize = true,
                        ForeColor = Color.FromArgb(88, 206, 138)
                    };

                    // Кнопки
                    var restartButton = new Button
                    {
                        Text = "🔄 Повторить",
                        Location = new Point(150, 280),
                        Size = new Size(180, 40),
                        BackColor = Color.FromArgb(33, 150, 243),
                        ForeColor = Color.White,
                        Font = new Font("Arial", 12),
                        FlatStyle = FlatStyle.Flat
                    };
                    restartButton.FlatAppearance.BorderSize = 0;
                    restartButton.Click += (s, ev) =>
                    {
                        // Сброс и перезапуск
                        currentQuestionIndex = 0;
                        score = 0;
                        wordsPracticedInSession = 0;
                        startTime = DateTime.Now;
                        resultPanel.Visible = false;
                        questionLabel.Visible = true;
                        if (answerTextBox != null) answerTextBox.Visible = true;
                        if (optionButtons != null)
                        {
                            foreach (var button in optionButtons)
                                button.Visible = true;
                        }
                        submitButton.Visible = true;
                        progressBar.Visible = true;
                        timerLabel.Visible = true;
                        typeLabel.Visible = true;
                        scoreLabel.Text = "Счет: 0";
                        progressBar.Value = 0;
                        ShowNextQuestion();
                    };

                    var closeButton = new Button
                    {
                        Text = "🏠 В главное меню",
                        Location = new Point(350, 280),
                        Size = new Size(180, 40),
                        BackColor = Color.FromArgb(88, 206, 138),
                        ForeColor = Color.White,
                        Font = new Font("Arial", 12),
                        FlatStyle = FlatStyle.Flat
                    };
                    closeButton.FlatAppearance.BorderSize = 0;
                    closeButton.Click += (s, ev) => this.Close();

                    resultPanel.Controls.Add(resultTitle);
                    resultPanel.Controls.Add(resultText);
                    resultPanel.Controls.Add(resultProgress);
                    resultPanel.Controls.Add(progressLabel);
                    resultPanel.Controls.Add(restartButton);
                    resultPanel.Controls.Add(closeButton);

                    this.Controls.Add(resultPanel);

                    // Показываем сообщение
                    if (finalScore >= 70)
                    {
                        MessageBox.Show($"Отличная работа! Вы набрали {finalScore}%!\n" +
                                       $"Получено {xpEarned} XP.\n" +
                                       $"Текущий уровень: {updatedUser.Level}\n" +
                                       $"Общий опыт: {updatedUser.Experience} XP\n" +
                                       $"Продолжайте в том же духе!",
                                       "Практика завершена",
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Вы набрали {finalScore}%.\n" +
                                       $"Получено {xpEarned} XP.\n" +
                                       $"Повторите материал и попробуйте снова!",
                                       "Практика завершена",
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении прогресса: {ex.Message}", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Вспомогательные классы для хранения данных
        private class PracticeWordData
        {
            public string CorrectAnswer { get; set; }
            public int WordId { get; set; }
            public Word Word { get; set; }
        }

        private class PracticeOptionData
        {
            public string Text { get; set; }
            public bool IsCorrect { get; set; }
            public int WordId { get; set; }
            public Word Word { get; set; }
        }

        private class PracticeMatchData
        {
            public string DisplayText { get; set; }
            public string CorrectMatch { get; set; }
            public int WordId { get; set; }
            public Word Word { get; set; }
        }
    }
}