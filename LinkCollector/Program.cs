using System;
using System.Windows.Forms;
using LinkCollector.Forms;

namespace LinkCollector
{
    /// <summary>
    /// Головний клас програми, що містить точку входу.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Головна точка входу для програми.
        /// </summary>
        [STAThread] // Вказує, що модель потоків COM для програми є однопотоковою (необхідно для Windows Forms)
        static void Main()
        {
            // Вмикає візуальні стилі для елементів керування (щоб кнопки виглядали сучасно)
            Application.EnableVisualStyles();

            // Встановлює сумісність рендерингу тексту (використання GDI+ для кращого вигляду тексту)
            Application.SetCompatibleTextRenderingDefault(false);

            // Створює екземпляр головної форми та запускає цикл повідомлень програми
            Application.Run(new MainForm());
        }
    }
}