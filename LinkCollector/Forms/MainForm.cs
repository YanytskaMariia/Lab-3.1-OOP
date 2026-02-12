using System;
using System.Drawing;
using System.Windows.Forms;
using LinkCollector.Models;
using LinkCollector.Services;

namespace LinkCollector.Forms
{
    /// <summary>
    /// Головна форма програми. Відповідає за відображення списку ресурсів та навігацію.
    /// </summary>
    public partial class MainForm : Form
    {
        // Елементи інтерфейсу
        private DataGridView grid;      // Таблиця для даних
        private TextBox txtSearch;      // Поле пошуку
        private Panel topPanel;         // Верхня панель для кнопок

        public MainForm()
        {
            InitializeComponent(); // Ініціалізація стандартних компонентів VS

            // Налаштування властивостей головного вікна
            this.Text = "Link Collector — Головне вікно";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White; // Білий фон для сучасного вигляду
            this.Font = new Font("Segoe UI", 9); // Встановлюємо читабельний шрифт

            // Виклик методу побудови інтерфейсу
            InitializeCustomUI();

            // Завантаження даних у таблицю при старті
            RefreshGrid();
        }

        /// <summary>
        /// Метод створення та розміщення елементів керування (Frontend).
        /// </summary>
        private void InitializeCustomUI()
        {
            this.Controls.Clear(); // Очищення форми перед малюванням

            // 1. Верхня панель (Header)
            topPanel = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.WhiteSmoke };

            // Елементи пошуку
            Label lblSearch = new Label { Text = "Пошук:", Location = new Point(15, 25), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtSearch = new TextBox { PlaceholderText = "Введіть назву або автора...", Width = 250, Location = new Point(70, 22), Font = new Font("Segoe UI", 10) };
            // Подія: при введенні тексту оновлюємо таблицю
            txtSearch.TextChanged += (s, e) => RefreshGrid(txtSearch.Text);

            // Кнопки меню (використовуємо допоміжний метод для стилізації)
            Button btnAdd = CreateStyledButton("Додати", 340, Color.SeaGreen, (s, e) => {
                // Відкриваємо форму додавання. Якщо результат OK - оновлюємо таблицю.
                if (new AddEditForm().ShowDialog() == DialogResult.OK) RefreshGrid();
            });

            Button btnDelete = CreateStyledButton("Видалити", 440, Color.IndianRed, BtnDelete_Click);

            Button btnCategories = CreateStyledButton("Категорії", 540, Color.SteelBlue, (s, e) => new CategoryManagerForm().ShowDialog());

            Button btnExport = CreateStyledButton("Експорт", 640, Color.SlateGray, (s, e) => new ExportForm().ShowDialog());

            Button btnRefresh = CreateStyledButton("Оновити", 740, Color.Gray, (s, e) => RefreshGrid());

            // Додаємо елементи на панель
            topPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnAdd, btnDelete, btnCategories, btnExport, btnRefresh });
            this.Controls.Add(topPanel);

            // 2. Налаштування таблиці (Data Grid)
            grid = new DataGridView
            {
                Dock = DockStyle.Fill, // Заповнити весь простір, що залишився
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, // Виділяти весь рядок
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                GridColor = Color.LightGray
            };

            // Стилізація заголовків колонок
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(230, 230, 230),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Padding = new Padding(5)
            };
            grid.EnableHeadersVisualStyles = false;

            // Визначення колонок таблиці та прив'язка до властивостей моделі (DataPropertyName)
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Назва", DataPropertyName = "Title", Width = 250 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Автор", DataPropertyName = "Author", Width = 180 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Рік", DataPropertyName = "Year", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Категорія", DataPropertyName = "Category", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Тип", DataPropertyName = "Type", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Джерело", DataPropertyName = "UrlOrSource", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            // Подія: подвійний клік відкриває редагування
            grid.DoubleClick += Grid_DoubleClick;

            this.Controls.Add(grid);
        }

        /// <summary>
        /// Допоміжний метод для створення кнопок з єдиним стилем.
        /// </summary>
        private Button CreateStyledButton(string text, int x, Color color, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, 18),
                Width = 90,
                Height = 35,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, // Плоский дизайн
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand // Курсор у вигляді руки
            };
            btn.FlatAppearance.BorderSize = 0; // Прибираємо рамку
            btn.Click += onClick; // Прив'язуємо подію кліку
            return btn;
        }

        /// <summary>
        /// Оновлення даних у таблиці з урахуванням пошукового запиту.
        /// </summary>
        private void RefreshGrid(string query = "")
        {
            grid.DataSource = null; // Скидаємо старе джерело
            grid.DataSource = LinkRepository.Search(query); // Отримуємо нові дані з репозиторію
        }

        /// <summary>
        /// Обробка видалення запису.
        /// </summary>
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count > 0)
            {
                var link = (ResourceLink)grid.SelectedRows[0].DataBoundItem;
                // Запитуємо підтвердження у користувача
                if (MessageBox.Show($"Видалити '{link.Title}'?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    LinkRepository.Remove(link);
                    RefreshGrid();
                }
            }
            else MessageBox.Show("Оберіть рядок!");
        }

        /// <summary>
        /// Обробка редагування запису (подвійний клік).
        /// </summary>
        private void Grid_DoubleClick(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count > 0)
            {
                var link = (ResourceLink)grid.SelectedRows[0].DataBoundItem;
                // Передаємо вибраний лінк у форму редагування
                if (new AddEditForm(link).ShowDialog() == DialogResult.OK) RefreshGrid();
            }
        }
    }
}