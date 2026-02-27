using System;
using System.Drawing;
using System.Windows.Forms;
using LinkCollector.Models;
using LinkCollector.Services;

namespace LinkCollector.Forms
{
    /// <summary>
    /// Головна форма програми. Відповідає за відображення списку ресурсів та навігацію.
    /// Функцію пошуку видалено для спрощення інтерфейсу.
    /// </summary>
    public partial class MainForm : Form
    {
        // Елементи інтерфейсу
        private DataGridView grid;      // Таблиця для виводу списку ресурсів
        private Panel topPanel;         // Верхня панель інструментів

        // DI services
        private readonly ILinkRepository _repo;
        private readonly ICitationService _citationService;

        /// <summary>
        /// Ініціалізує новий екземпляр форми <see cref="MainForm"/>.
        /// </summary>
        public MainForm(ILinkRepository repo, ICitationService citationService)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _citationService = citationService ?? throw new ArgumentNullException(nameof(citationService));

            InitializeComponent();

            this.Text = "Link Collector — Керування джерелами";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);

            InitializeCustomUI();

            // Первинне завантаження всіх даних без фільтрації
            RefreshGrid();
        }

        /// <summary>
        /// Побудова графічного інтерфейсу (панель кнопок та таблиця).
        /// </summary>
        private void InitializeCustomUI()
        {
            this.Controls.Clear();

            // 1. Створення верхньої панелі (Header)
            topPanel = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.WhiteSmoke };

            // Додавання кнопок (зміщено вліво, оскільки пошук видалено)
            Button btnAdd = CreateStyledButton("Додати", 15, Color.SeaGreen, (s, e) =>
            {
                // pass injected repository so forms share the same in-memory state
                using var form = new AddEditForm(_repo);
                if (form.ShowDialog(this) == DialogResult.OK) RefreshGrid();
            });

            Button btnDelete = CreateStyledButton("Видалити", 115, Color.IndianRed, BtnDelete_Click);

            Button btnCategories = CreateStyledButton("Категорії", 215, Color.SteelBlue, (s, e) =>
            {
                using var catForm = new CategoryManagerForm(_repo);
                catForm.ShowDialog(this);
                RefreshGrid();
            });

            Button btnExport = CreateStyledButton("Експорт", 315, Color.SlateGray, (s, e) =>
            {
                using var exportForm = new ExportForm(_repo);
                exportForm.ShowDialog(this);
            });

            Button btnRefresh = CreateStyledButton("Оновити", 415, Color.Gray, (s, e) => RefreshGrid());

            topPanel.Controls.AddRange(new Control[] { btnAdd, btnDelete, btnCategories, btnExport, btnRefresh });
            this.Controls.Add(topPanel);

            // 2. Налаштування DataGridView
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                GridColor = Color.LightGray
            };

            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(240, 240, 240),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Padding = new Padding(5)
            };
            grid.EnableHeadersVisualStyles = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Назва", DataPropertyName = "Title", Width = 200 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Автор", DataPropertyName = "Author", Width = 150 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Рік", DataPropertyName = "Year", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Категорія", DataPropertyName = "Category", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Тип", DataPropertyName = "Type", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Джерело", DataPropertyName = "UrlOrSource", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            grid.DoubleClick += Grid_DoubleClick;
            this.Controls.Add(grid);
        }

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
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;
            return btn;
        }

        /// <summary>
        /// Оновлює вміст таблиці. Завжди відображає повний список з репозиторію.
        /// </summary>
        private void RefreshGrid()
        {
            grid.DataSource = null;
            // Викликаємо Search з порожнім рядком, що згідно з логікою репозиторію повертає всі записи
            grid.DataSource = _repo.Search("");
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count > 0)
            {
                var link = (ResourceLink)grid.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show($"Видалити запис '{link.Title}'?", "Підтвердження",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _repo.Remove(link);
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, оберіть рядок для видалення.", "Інфо", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Grid_DoubleClick(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count > 0)
            {
                var link = (ResourceLink)grid.SelectedRows[0].DataBoundItem;
                using var form = new AddEditForm(_repo, link);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshGrid();
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e) { }
    }
}