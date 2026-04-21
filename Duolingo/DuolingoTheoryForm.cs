using System;
using System.Drawing;
using System.Windows.Forms;

namespace Duolingo
{
    public class DuolingoTheoryForm : Form
    {
        private DatabaseHelper dbHelper;
        private User currentUser;

        public DuolingoTheoryForm(DatabaseHelper dbHelper, User user)
        {
            this.dbHelper = dbHelper;
            this.currentUser = user;
            InitializeForm();
            LoadEnglishTheory();
        }

        private void InitializeForm()
        {
            this.Text = "📚 Теория английского языка - Duolingo";
            this.Size = new Size(1100, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Padding = new Padding(15);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Главный контейнер
            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));  // Заголовок
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Контент
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));  // Кнопки

            // ===================== ЗАГОЛОВОК =====================
            var headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(88, 206, 138), // Зеленый Duolingo
                Padding = new Padding(10)
            };

            var titleLabel = new Label
            {
                Text = "📚 Теория английского языка",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var levelLabel = new Label
            {
                Text = $"👤 {currentUser?.Username} | Уровень: {currentUser?.Level ?? 1}",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true,
                Margin = new Padding(0, 0, 10, 0)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(levelLabel);

            // ===================== КОНТЕНТ =====================
            var contentScrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(10)
            };

            var contentPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(15)
            };

            contentScrollPanel.Controls.Add(contentPanel);

            // ===================== КНОПКИ =====================
            var footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var closeButton = new Button
            {
                Text = "✕ Закрыть теорию",
                Size = new Size(150, 40),
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            // Центрируем кнопку
            closeButton.Location = new Point(
                (footerPanel.Width - closeButton.Width) / 2,
                (footerPanel.Height - closeButton.Height) / 2
            );

            footerPanel.Controls.Add(closeButton);
            footerPanel.SizeChanged += (s, e) =>
            {
                closeButton.Location = new Point(
                    (footerPanel.Width - closeButton.Width) / 2,
                    (footerPanel.Height - closeButton.Height) / 2
                );
            };

            // ===================== СБОРКА =====================
            mainContainer.Controls.Add(headerPanel, 0, 0);
            mainContainer.Controls.Add(contentScrollPanel, 0, 1);
            mainContainer.Controls.Add(footerPanel, 0, 2);

            this.Controls.Add(mainContainer);

            // Сохраняем ссылку на contentPanel
            this.Tag = contentPanel;
        }

        private void LoadEnglishTheory()
        {
            var contentPanel = this.Tag as FlowLayoutPanel;
            if (contentPanel == null) return;

            // Очищаем панель
            contentPanel.Controls.Clear();

            // ===================== РАЗДЕЛ 1: ОСНОВЫ =====================
            AddTheorySection(contentPanel,
                "🎯 ОСНОВЫ АНГЛИЙСКОГО ЯЗЫКА",
                CreateBasicsSection());

            // ===================== РАЗДЕЛ 2: ГРАММАТИКА =====================
            AddTheorySection(contentPanel,
                "📖 ГРАММАТИКА ДЛЯ НАЧИНАЮЩИХ",
                CreateGrammarSection());

            // ===================== РАЗДЕЛ 3: ВРЕМЕНА =====================
            AddTheorySection(contentPanel,
                "⏰ ВРЕМЕНА В АНГЛИЙСКОМ",
                CreateTensesSection());

            // ===================== РАЗДЕЛ 4: СЛОВАРНЫЙ ЗАПАС =====================
            AddTheorySection(contentPanel,
                "🔤 ПОПОЛНЕНИЕ СЛОВАРНОГО ЗАПАСА",
                CreateVocabularySection());

            // ===================== РАЗДЕЛ 5: ПРАКТИКА =====================
            AddTheorySection(contentPanel,
                "💪 ПРАКТИЧЕСКИЕ СОВЕТЫ ОТ Duolingo",
                CreatePracticeSection());

            // ===================== РАЗДЕЛ 6: ЧАСТЫЕ ОШИБКИ =====================
            AddTheorySection(contentPanel,
                "⚠️ 10 САМЫХ ЧАСТЫХ ОШИБОК",
                CreateCommonMistakesSection());
        }

        private void AddTheorySection(FlowLayoutPanel parent, string title, Panel content)
        {
            var sectionPanel = new Panel
            {
                Size = new Size(1020, content.Height + 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 20),
                Padding = new Padding(10)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(88, 206, 138), // Зеленый Duolingo
                Location = new Point(10, 10),
                AutoSize = true
            };

            content.Location = new Point(10, 50);
            sectionPanel.Controls.Add(titleLabel);
            sectionPanel.Controls.Add(content);

            parent.Controls.Add(sectionPanel);
        }

        private Panel CreateBasicsSection()
        {
            var panel = new Panel
            {
                Size = new Size(980, 350),
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var basicsText = @"📌 АНГЛИЙСКИЙ АЛФАВИТ: 26 букв
   • Гласные буквы: A, E, I, O, U (иногда Y)
   • Согласные буквы: все остальные
   • Произношение букв отличается от русского

📌 АРТИКЛИ (The Articles):
   • A/AN - неопределенный артикль
     Используется, когда предмет упоминается впервые
     Пример: I saw a cat. (Я видел кошку - какую-то)
     
   • THE - определенный артикль
     Используется, когда предмет конкретный или уже известен
     Пример: The cat is black. (Эта кошка черная)

📌 ЛИЧНЫЕ МЕСТОИМЕНИЯ (Personal Pronouns):
   • I (я) - всегда с большой буквы
   • You (ты/вы) - для одного или нескольких людей
   • He (он), She (она), It (оно)
   • We (мы), They (они)

📌 БАЗОВЫЕ ФРАЗЫ ДЛЯ ОБЩЕНИЯ:
   • Hello! / Hi! - Привет!
   • How are you? - Как дела?
   • What's your name? - Как тебя звать?
   • Nice to meet you! - Приятно познакомиться!
   • Thank you! / Thanks! - Спасибо!
   • You're welcome! - Пожалуйста! (в ответ на спасибо)
   • Goodbye! / Bye! - До свидания!
   • See you later! - Увидимся позже!";

            var textLabel = new Label
            {
                Text = basicsText,
                Font = new Font("Consolas", 11),
                Location = new Point(10, 10),
                Size = new Size(960, 330),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            panel.Controls.Add(textLabel);
            return panel;
        }

        private Panel CreateGrammarSection()
        {
            var panel = new Panel
            {
                Size = new Size(980, 450),
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var grammarText = @"✅ ПРАВИЛО 1: Порядок слов в предложении
   Подлежащее + Сказуемое + Дополнение + Обстоятельство
   • I read books every day. - Я читаю книги каждый день.
   • She speaks English well. - Она хорошо говорит по-английски.

✅ ПРАВИЛО 2: Образование множественного числа
   • Большинство слов: +s (cat → cats, book → books)
   • Слова на -s, -ss, -sh, -ch, -x, -o: +es (box → boxes)
   • Слова на согласная + y: y → ies (city → cities)
   • Исключения: man → men, woman → women, child → children

✅ ПРАВИЛО 3: Притяжательный падеж
   • Для людей и животных: 's (John's car, cat's tail)
   • Для множественного числа: s' (students' books)
   • Для неодушевленных предметов: of (the door of the room)

✅ ПРАВИЛО 4: Степени сравнения прилагательных
   • Положительная: big - большой
   • Сравнительная: bigger - больше (для коротких слов)
   • Превосходная: the biggest - самый большой

✅ ПРАВИЛО 5: Вопросительные слова (Question Words)
   • What? - Что? (про предмет)
   • Who? - Кто? (про человека)
   • Where? - Где? (про место)
   • When? - Когда? (про время)
   • Why? - Почему? (про причину)
   • How? - Как? (про способ)
   • How much/many? - Сколько?

✅ ПРАВИЛО 6: Построение вопросов
   • Вспомогательный глагол + подлежащее + основной глагол
   • Do you like music? - Ты любишь музыку?
   • Does she work here? - Она работает здесь?
   • Are they students? - Они студенты?

✅ ПРАВИЛО 7: Отрицательные предложения
   • don't/doesn't + глагол: I don't know
   • am not/is not/are not: She isn't at home
   • never: I never smoke
   • no: There is no milk in the fridge";

            var textLabel = new Label
            {
                Text = grammarText,
                Font = new Font("Consolas", 11),
                Location = new Point(10, 10),
                Size = new Size(960, 430),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            panel.Controls.Add(textLabel);
            return panel;
        }

        private Panel CreateTensesSection()
        {
            var panel = new Panel
            {
                Size = new Size(980, 550),
                BackColor = Color.FromArgb(240, 248, 255),
                Padding = new Padding(10)
            };

            var tensesTable = new TableLayoutPanel
            {
                Location = new Point(10, 10),
                Size = new Size(960, 530),
                ColumnCount = 4,
                RowCount = 7,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                BackColor = Color.White
            };

            // Настройка колонок
            tensesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tensesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tensesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tensesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Заголовки таблицы
            AddTableHeader(tensesTable, "ВРЕМЯ", 0, 0, Color.FromArgb(88, 206, 138));
            AddTableHeader(tensesTable, "ФОРМУЛА И ИСПОЛЬЗОВАНИЕ", 1, 0, Color.FromArgb(88, 206, 138));
            AddTableHeader(tensesTable, "УТВЕРДИТЕЛЬНОЕ", 2, 0, Color.FromArgb(88, 206, 138));
            AddTableHeader(tensesTable, "ВОПРОСИТЕЛЬНОЕ", 3, 0, Color.FromArgb(88, 206, 138));

            // Данные о временах
            var tensesData = new[]
            {
                new {
                    Time = "Present Simple",
                    Formula = "V/Vs\n(регулярные действия, факты)",
                    Positive = "I work every day",
                    Question = "Do you work every day?"
                },
                new {
                    Time = "Present Continuous",
                    Formula = "am/is/are + V-ing\n(действие сейчас)",
                    Positive = "I am working now",
                    Question = "Are you working now?"
                },
                new {
                    Time = "Past Simple",
                    Formula = "V2/V-ed\n(действие в прошлом)",
                    Positive = "I worked yesterday",
                    Question = "Did you work yesterday?"
                },
                new {
                    Time = "Past Continuous",
                    Formula = "was/were + V-ing\n(действие в процессе в прошлом)",
                    Positive = "I was working at 5pm",
                    Question = "Were you working at 5pm?"
                },
                new {
                    Time = "Future Simple",
                    Formula = "will + V\n(будущие действия)",
                    Positive = "I will work tomorrow",
                    Question = "Will you work tomorrow?"
                },
                new {
                    Time = "Present Perfect",
                    Formula = "have/has + V3\n(опыт, результат)",
                    Positive = "I have worked here",
                    Question = "Have you worked here?"
                }
            };

            for (int i = 0; i < tensesData.Length; i++)
            {
                AddTableCell(tensesTable, tensesData[i].Time, 0, i + 1, Color.FromArgb(255, 253, 231));
                AddTableCell(tensesTable, tensesData[i].Formula, 1, i + 1, Color.FromArgb(240, 255, 240));
                AddTableCell(tensesTable, tensesData[i].Positive, 2, i + 1, Color.FromArgb(255, 250, 240));
                AddTableCell(tensesTable, tensesData[i].Question, 3, i + 1, Color.FromArgb(240, 240, 255));
            }

            // Подпись под таблицей
            var noteLabel = new Label
            {
                Text = "💡 Совет: Начинайте с Present Simple и постепенно добавляйте другие времена",
                Font = new Font("Arial", 10, FontStyle.Italic),
                Location = new Point(10, 545),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            panel.Controls.Add(tensesTable);
            panel.Controls.Add(noteLabel);
            return panel;
        }

        private Panel CreateVocabularySection()
        {
            var panel = new Panel
            {
                Size = new Size(980, 400),
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var vocabularyText = @"🎯 СТРАТЕГИИ ЗАПОМИНАНИЯ СЛОВ:

1. КАТЕГОРИИ СЛОВ (учите группами):
   • Семья: mother, father, sister, brother, parents
   • Еда: apple, bread, water, milk, coffee
   • Цвета: red, blue, green, yellow, black, white
   • Животные: dog, cat, bird, fish, horse

2. КАРТОЧКИ ДЛЯ ЗАПОМИНАНИЯ:
   • На одной стороне - английское слово
   • На другой - перевод и картинка
   • Повторяйте по системе интервального повторения

3. АССОЦИАЦИИ И МНЕМОНИКА:
   • Связывайте новые слова с уже известными
   • Придумывайте смешные ассоциации
   • Создавайте истории с новыми словами

4. СЛОВООБРАЗОВАНИЕ:
   • Существительное → Прилагательное: beauty → beautiful
   • Глагол → Существительное: teach → teacher
   • Прилагательное → Наречие: quick → quickly

5. ФРАЗОВЫЕ ГЛАГОЛЫ (Phrasal Verbs):
   • look for - искать
   • give up - сдаваться
   • get up - вставать
   • turn on - включать
   • turn off - выключать

6. ИДИОМЫ И ВЫРАЖЕНИЯ:
   • It's raining cats and dogs - Льет как из ведра
   • Break a leg! - Ни пуха ни пера!
   • Piece of cake - Проще простого";

            var textLabel = new Label
            {
                Text = vocabularyText,
                Font = new Font("Consolas", 11),
                Location = new Point(10, 10),
                Size = new Size(960, 380),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            panel.Controls.Add(textLabel);
            return panel;
        }

        private Panel CreatePracticeSection()
        {
            var panel = new Panel
            {
                Size = new Size(980, 350),
                BackColor = Color.FromArgb(240, 255, 240),
                Padding = new Padding(15),
                BorderStyle = BorderStyle.FixedSingle
            };

            var practiceText = @"🌟 СОВЕТЫ ДЛЯ ЭФФЕКТИВНОГО ОБУЧЕНИЯ:

🎯 1. РЕГУЛЯРНОСТЬ ВАЖНЕЕ ПРОДОЛЖИТЕЛЬНОСТИ
   • Занимайтесь по 20-30 минут каждый день
   • Лучше 7 дней по 20 минут, чем 1 день 2 часа
   • Создайте привычку: английский с утра за завтраком

🎯 2. ПОГРУЖЕНИЕ В ЯЗЫКОВУЮ СРЕДУ
   • Слушайте английскую музыку и подпевайте
   • Смотрите фильмы с английскими субтитрами
   • Читайте новости на английском (BBC, CNN)
   • Измените язык телефона на английский

🎯 3. ПРАКТИКА ГОВОРЕНИЯ
   • Разговаривайте сами с собой на английском
   • Описывайте вслух то, что делаете
   • Найдите языкового партнера (Tandem, HelloTalk)
   • Запишите свой голос и проанализируйте произношение

🎯 4. ИСПОЛЬЗУЙТЕ ТЕХНОЛОГИИ
   • Приложения: Duolingo, Memrise, Anki
   • YouTube каналы: BBC Learning English, EnglishClass101
   • Подкасты: 6 Minute English, The English We Speak
   • Игры: Scrabble, Words with Friends

🎯 5. НЕ БОЙТЕСЬ ОШИБАТЬСЯ
   • Ошибки - это часть процесса обучения
   • Носители языка ценят попытки говорить
   • Каждая ошибка - шаг к совершенству
   • Помните: идеального акцента не существует!";

            var textLabel = new Label
            {
                Text = practiceText,
                Font = new Font("Consolas", 10),
                Location = new Point(10, 10),
                Size = new Size(950, 330),
                ForeColor = Color.FromArgb(0, 100, 0)
            };

            panel.Controls.Add(textLabel);
            return panel;
        }

        private Panel CreateCommonMistakesSection()
        {
            var panel = new Panel
            {
                Size = new Size(980, 400),
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var mistakesTable = new TableLayoutPanel
            {
                Location = new Point(10, 10),
                Size = new Size(960, 380),
                ColumnCount = 3,
                RowCount = 11,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                BackColor = Color.White
            };

            mistakesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mistakesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mistakesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            // Заголовки
            AddTableHeader(mistakesTable, "ОШИБКА ❌", 0, 0, Color.FromArgb(255, 100, 100));
            AddTableHeader(mistakesTable, "ПРАВИЛЬНО ✅", 1, 0, Color.FromArgb(100, 200, 100));
            AddTableHeader(mistakesTable, "ОБЪЯСНЕНИЕ", 2, 0, Color.FromArgb(100, 100, 255));

            // Данные об ошибках
            var mistakesData = new[]
            {
                new {
                    Error = "I am agree",
                    Correct = "I agree",
                    Explanation = "agree - самостоятельный глагол, не требует be"
                },
                new {
                    Error = "How to say?",
                    Correct = "How do you say?",
                    Explanation = "Нужен вспомогательный глагол do для вопроса"
                },
                new {
                    Error = "I have 20 years",
                    Correct = "I am 20 years old",
                    Explanation = "Возраст передается через глагол be, а не have"
                },
                new {
                    Error = "She go to school",
                    Correct = "She goes to school",
                    Explanation = "3 лицо, единственное число требует окончания -s"
                },
                new {
                    Error = "I very like",
                    Correct = "I like it very much",
                    Explanation = "very ставится перед прилагательным, а не глаголом"
                },
                new {
                    Error = "It's depend",
                    Correct = "It depends",
                    Explanation = "depend требует -s в Present Simple для 3 лица"
                },
                new {
                    Error = "I'm coming from Russia",
                    Correct = "I come from Russia",
                    Explanation = "Для постоянного места жительства используем Present Simple"
                },
                new {
                    Error = "He said me",
                    Correct = "He told me",
                    Explanation = "say требует предлога to: said to me, tell - без предлога"
                },
                new {
                    Error = "I want that you come",
                    Correct = "I want you to come",
                    Explanation = "want + объект + to + глагол"
                },
                new {
                    Error = "I'm thinking to go",
                    Correct = "I'm thinking of going",
                    Explanation = "think of/about + -ing форма"
                }
            };

            for (int i = 0; i < mistakesData.Length; i++)
            {
                AddTableCell(mistakesTable, mistakesData[i].Error, 0, i + 1, Color.FromArgb(255, 240, 240));
                AddTableCell(mistakesTable, mistakesData[i].Correct, 1, i + 1, Color.FromArgb(240, 255, 240));
                AddTableCell(mistakesTable, mistakesData[i].Explanation, 2, i + 1, Color.FromArgb(240, 240, 255));
            }

            panel.Controls.Add(mistakesTable);
            return panel;
        }

        private void AddTableHeader(TableLayoutPanel table, string text, int column, int row, Color color)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                BackColor = color,
                Padding = new Padding(5)
            };

            table.Controls.Add(label, column, row);
        }

        private void AddTableCell(TableLayoutPanel table, string text, int column, int row, Color backColor)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Arial", 10),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(64, 64, 64),
                BackColor = backColor,
                Padding = new Padding(8, 5, 5, 5)
            };

            table.Controls.Add(label, column, row);
        }

        // ===================== СТАТИЧЕСКИЕ МЕТОДЫ =====================

        public static void ShowTheory(DatabaseHelper dbHelper, User user)
        {
            var theoryForm = new DuolingoTheoryForm(dbHelper, user);
            theoryForm.ShowDialog();
        }

        public static Button CreateTheoryButton(DatabaseHelper dbHelper, User user)
        {
            var theoryButton = new Button
            {
                Text = "📚 Теория английского",
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            theoryButton.FlatAppearance.BorderSize = 0;
            theoryButton.Click += (s, e) =>
            {
                ShowTheory(dbHelper, user);
            };

            return theoryButton;
        }

        // ===================== ДОПОЛНИТЕЛЬНЫЕ КЛАССЫ =====================

        public class EnglishWord
        {
            public string English { get; set; }
            public string Russian { get; set; }
            public string Transcription { get; set; }
            public string Example { get; set; }
            public string Category { get; set; }

            public EnglishWord(string english, string russian, string transcription, string example, string category)
            {
                English = english;
                Russian = russian;
                Transcription = transcription;
                Example = example;
                Category = category;
            }

            public Panel CreateWordCard()
            {
                var card = new Panel
                {
                    Size = new Size(280, 140),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(5),
                    Padding = new Padding(10)
                };

                var englishLabel = new Label
                {
                    Text = English,
                    Font = new Font("Arial", 16, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(88, 206, 138)
                };

                var transcriptionLabel = new Label
                {
                    Text = $"[{Transcription}]",
                    Font = new Font("Arial", 10, FontStyle.Italic),
                    Location = new Point(10, 40),
                    AutoSize = true,
                    ForeColor = Color.Gray
                };

                var russianLabel = new Label
                {
                    Text = Russian,
                    Font = new Font("Arial", 12),
                    Location = new Point(10, 60),
                    AutoSize = true
                };

                var exampleLabel = new Label
                {
                    Text = $"Пример: {Example}",
                    Font = new Font("Arial", 9),
                    Location = new Point(10, 85),
                    Size = new Size(260, 45),
                    ForeColor = Color.FromArgb(100, 100, 100)
                };

                var categoryLabel = new Label
                {
                    Text = Category,
                    Font = new Font("Arial", 8, FontStyle.Bold),
                    Location = new Point(10, 115),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(33, 150, 243)
                };

                card.Controls.Add(englishLabel);
                card.Controls.Add(transcriptionLabel);
                card.Controls.Add(russianLabel);
                card.Controls.Add(exampleLabel);
                card.Controls.Add(categoryLabel);

                return card;
            }
        }

        // ===================== ПРИМЕР ИСПОЛЬЗОВАНИЯ СЛОВ =====================

        public static EnglishWord[] GetCommonWords()
        {
            return new EnglishWord[]
            {
                new EnglishWord("Hello", "Привет", "/həˈloʊ/", "Hello, how are you?", "Приветствие"),
                new EnglishWord("Thank you", "Спасибо", "/ˈθæŋk juː/", "Thank you for your help!", "Вежливость"),
                new EnglishWord("Please", "Пожалуйста", "/pliːz/", "Please, sit down.", "Вежливость"),
                new EnglishWord("Goodbye", "До свидания", "/ˌɡʊdˈbaɪ/", "Goodbye, see you tomorrow!", "Прощание"),
                new EnglishWord("Yes", "Да", "/jes/", "Yes, I understand.", "Согласие"),
                new EnglishWord("No", "Нет", "/noʊ/", "No, thank you.", "Отрицание"),
                new EnglishWord("Sorry", "Извините", "/ˈsɑːri/", "Sorry, I'm late.", "Извинение"),
                new EnglishWord("Excuse me", "Простите", "/ɪkˈskjuːz miː/", "Excuse me, where is the station?", "Внимание")
            };
        }
    }
}