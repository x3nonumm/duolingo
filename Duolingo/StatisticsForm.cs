using System;
using System.Drawing;
using System.Windows.Forms;

namespace Duolingo
{
    public class StatisticsForm : Form
    {
        private DatabaseHelper dbHelper;
        private User currentUser;

        public StatisticsForm(DatabaseHelper dbHelper, User user)
        {
            this.dbHelper = dbHelper;
            this.currentUser = user;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Детальная статистика";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Padding = new Padding(20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Главная панель с TableLayoutPanel
            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 70)); // Заголовок
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Контент
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); // Кнопки

            // Заголовок
            var titlePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = "📊 Детальная статистика",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(88, 206, 138),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 70
            };

            titlePanel.Controls.Add(titleLabel);

            // Панель контента с прокруткой
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            // Пример статистики
            var statsTable = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 8,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                BackColor = Color.White
            };

            // Настройка колонок
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

            // Заполняем статистику
            var stats = new[]
            {
                new { Title = "Всего уроков пройдено", Value = "12" },
                new { Title = "Средний балл", Value = "87%" },
                new { Title = "Выучено слов", Value = "156" },
                new { Title = "Освоено слов", Value = "89" },
                new { Title = "Текущая серия", Value = "7 дней" },
                new { Title = "Общее время обучения", Value = "45 ч 30 мин" },
                new { Title = "Всего опыта", Value = "2450 XP" },
                new { Title = "Уровень", Value = "5" }
            };

            for (int i = 0; i < stats.Length; i++)
            {
                var titleCell = new Label
                {
                    Text = stats[i].Title,
                    Font = new Font("Arial", 11),
                    ForeColor = Color.FromArgb(64, 64, 64),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 10, 5, 10),
                    AutoSize = false,
                    Height = 45
                };

                var valueCell = new Label
                {
                    Text = stats[i].Value,
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    ForeColor = Color.FromArgb(88, 206, 138),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Padding = new Padding(5, 10, 10, 10),
                    AutoSize = false,
                    Height = 45
                };

                statsTable.Controls.Add(titleCell, 0, i);
                statsTable.Controls.Add(valueCell, 1, i);
            }

            // Прогресс-бар уровня
            var levelPanel = new Panel
            {
                Size = new Size(640, 100),
                BackColor = Color.White,
                Margin = new Padding(0, 20, 0, 0)
            };

            var levelLabel = new Label
            {
                Text = "Прогресс до следующего уровня",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(0, 10),
                AutoSize = true,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            var progressBar = new ProgressBar
            {
                Location = new Point(0, 40),
                Size = new Size(500, 25),
                Minimum = 0,
                Maximum = 100,
                Value = 75
            };

            var progressLabel = new Label
            {
                Text = "75% (375/500 XP)",
                Font = new Font("Arial", 10),
                Location = new Point(510, 40),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 206, 138)
            };

            levelPanel.Controls.Add(levelLabel);
            levelPanel.Controls.Add(progressBar);
            levelPanel.Controls.Add(progressLabel);

            // Добавляем элементы в contentPanel
            var contentFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };

            // Центрируем statsTable
            var statsContainer = new Panel
            {
                Height = statsTable.Height,
                Width = contentPanel.Width - 40,
                Padding = new Padding(0, 10, 0, 10)
            };
            statsTable.Location = new Point(
                (statsContainer.Width - statsTable.Width) / 2,
                10
            );
            statsContainer.Controls.Add(statsTable);

            contentFlow.Controls.Add(statsContainer);
            contentFlow.Controls.Add(levelPanel);
            contentPanel.Controls.Add(contentFlow);

            // Панель кнопок
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            var closeButton = new Button
            {
                Text = "Закрыть",
                Size = new Size(140, 45),
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(88, 206, 138),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            // Размещаем кнопку по центру
            closeButton.Location = new Point(
                (buttonPanel.Width - closeButton.Width) / 2,
                (buttonPanel.Height - closeButton.Height) / 2
            );

            // Обработчик изменения размера для центрирования кнопки
            buttonPanel.SizeChanged += (s, e) =>
            {
                closeButton.Location = new Point(
                    (buttonPanel.Width - closeButton.Width) / 2,
                    (buttonPanel.Height - closeButton.Height) / 2
                );
            };

            buttonPanel.Controls.Add(closeButton);

            // Добавляем все в mainTable
            mainTable.Controls.Add(titlePanel, 0, 0);
            mainTable.Controls.Add(contentPanel, 0, 1);
            mainTable.Controls.Add(buttonPanel, 0, 2);

            this.Controls.Add(mainTable);
        }
    }
}