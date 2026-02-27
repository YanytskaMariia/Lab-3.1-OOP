using System;
using System.Drawing;
using System.Windows.Forms;
using LinkCollector.Services;

namespace LinkCollector.Forms
{
    /// <summary>
    /// Форма для керування категоріями (контекстами).
    /// Дозволяє додавати нові унікальні категорії в репозиторій та видаляти існуючі.
    /// </summary>
    public partial class CategoryManagerForm : Form
    {
        private ListBox lstCategories;  // Список для відображення категорій
        private TextBox txtNewCategory; // Поле для введення назви нової категорії

        // Інжектований репозиторій (замість статичного використання)
        private readonly ILinkRepository _repo;

        /// <summary>
        /// Конструктор без параметрів (для VS Designer).
        /// Викликає конструктор з дефолтною інстанцією репозиторію.
        /// </summary>
        public CategoryManagerForm() : this(new LinkRepository()) { }

        /// <summary>
        /// Ініціалізує новий екземпляр форми <see cref="CategoryManagerForm"/> з інжектованим репозиторієм.
        /// </summary>
        public CategoryManagerForm(ILinkRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));

            InitializeComponent();

            this.Text = "Керування категоріями";
            this.Size = new Size(360, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 10);

            InitializeCustomUI();
        }

        /// <summary>
        /// Динамічна побудова графічного інтерфейсу форми.
        /// </summary>
        private void InitializeCustomUI()
        {
            this.Controls.Clear();

            Label lblInfo = new Label
            {
                Text = "Список доступних категорій:",
                Location = new Point(15, 10),
                AutoSize = true,
                ForeColor = Color.DarkSlateGray
            };
            this.Controls.Add(lblInfo);

            // Список категорій
            lstCategories = new ListBox
            {
                Location = new Point(15, 35),
                Size = new Size(315, 300),
                BorderStyle = BorderStyle.FixedSingle
            };
            RefreshList();
            this.Controls.Add(lstCategories);

            // Поле для введення нової категорії
            txtNewCategory = new TextBox
            {
                Location = new Point(15, 350),
                Size = new Size(205, 27),
                PlaceholderText = "Введіть назву..."
            };
            // Обробка натискання Enter у текстовому полі
            txtNewCategory.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) AddCategory(); };
            this.Controls.Add(txtNewCategory);

            // Кнопка додавання
            Button btnAdd = new Button
            {
                Text = "Додати",
                Location = new Point(230, 349),
                Size = new Size(100, 29),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.Click += (s, e) => AddCategory();
            this.Controls.Add(btnAdd);

            // Кнопка видалення
            Button btnRemove = new Button
            {
                Text = "Видалити обрану категорію",
                Location = new Point(15, 395),
                Size = new Size(315, 40),
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRemove.Click += (s, e) => RemoveCategory();
            this.Controls.Add(btnRemove);
        }

        /// <summary>
        /// Логіка додавання нової категорії до репозиторію.
        /// </summary>
        private void AddCategory()
        {
            string category = txtNewCategory.Text.Trim();
            if (!string.IsNullOrWhiteSpace(category))
            {
                _repo.AddCategory(category);
                RefreshList();
                txtNewCategory.Clear();
                txtNewCategory.Focus();
            }
        }

        /// <summary>
        /// Логіка видалення обраної категорії з репозиторію з перевіркою на мінімальну кількість.
        /// </summary>
        private void RemoveCategory()
        {
            if (lstCategories.SelectedItem != null)
            {
                string selected = lstCategories.SelectedItem.ToString();

                // Забороняємо видалення, якщо категорія остання (щоб не зламати UI створення посилань)
                if (_repo.GetCategories().Count <= 1)
                {
                    MessageBox.Show("У списку має залишатися хоча б одна категорія!", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _repo.RemoveCategory(selected);
                RefreshList();
            }
        }

        /// <summary>
        /// Оновлює дані у списку ListBox, синхронізуючи їх із репозиторієм.
        /// </summary>
        private void RefreshList()
        {
            lstCategories.DataSource = null;
            lstCategories.DataSource = _repo.GetCategories();
        }

        private void CategoryManagerForm_Load(object sender, EventArgs e) { }
    }
}