using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using LinkCollector.Forms;
using LinkCollector.Services;

namespace LinkCollector
{
    /// <summary>
    /// Головний клас програми, що містить точку входу.
    /// Відповідає за початкове налаштування, конфігурацію сервісів (DI) та запуск додатку.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Головна точка входу для програми.
        /// </summary>
        /// <remarks>
        /// Цей метод виконує:
        /// <list type="bullet">
        /// <item><description>Увімкнення візуальних стилів Windows Forms.</description></item>
        /// <item><description>Створення та налаштування контейнера впровадження залежностей (Dependency Injection).</description></item>
        /// <item><description>Реєстрацію сервісів (<see cref="ILinkRepository"/>, <see cref="ICitationService"/>) та форм.</description></item>
        /// <item><description>Запуск головної форми через DI-провайдер.</description></item>
        /// </list>
        /// </remarks>
        [STAThread] // Вказує, що модель потоків COM для програми є однопотоковою (необхідно для коректної роботи Windows Forms)
        static void Main()
        {
            // Вмикає візуальні стилі для елементів керування (сучасний вигляд кнопок тощо)
            Application.EnableVisualStyles();
            // Встановлює сумісність рендерингу тексту (GDI+)
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. Створюємо колекцію сервісів (контейнер для реєстрації залежностей)
            var services = new ServiceCollection();

            // 2. Реєструємо сервіси та форми

            // Singleton: створюється один раз на весь час роботи програми.
            // Ідеально підходить для репозиторіїв, що зберігають стан у пам'яті.
            services.AddSingleton<ILinkRepository, LinkRepository>();
            services.AddSingleton<ICitationService, CitationService>();

            // Transient: створюється новий екземпляр кожного разу, коли його запитують.
            // Форми зазвичай реєструються як Transient, хоча для головної форми це не критично (вона створюється 1 раз).
            services.AddTransient<MainForm>();

            // 3. Будуємо провайдер сервісів (DI Container)
            using var provider = services.BuildServiceProvider();

            // 4. Отримуємо екземпляр MainForm із контейнера.
            // Контейнер автоматично знайде і передасть необхідні залежності (ILinkRepository, ICitationService) у конструктор MainForm.
            var mainForm = provider.GetRequiredService<MainForm>();

            // 5. Запускаємо програму
            Application.Run(mainForm);
        }
    }
}