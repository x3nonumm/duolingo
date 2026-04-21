using System;
using System.Windows.Forms;

namespace Duolingo
{
    static class Program
    {
        private static DatabaseHelper dbHelper;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Создаем DatabaseHelper (он загрузит данные из файла)
                dbHelper = new DatabaseHelper();

                // Запускаем главную форму
                var mainForm = new Form1();

                // Подписываемся на событие закрытия приложения
                Application.ApplicationExit += OnApplicationExit;

                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска приложения: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            try
            {
                if (dbHelper != null)
                {
                    dbHelper.SaveData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных при выходе: {ex.Message}",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static DatabaseHelper GetDatabaseHelper()
        {
            return dbHelper;
        }
    }
}