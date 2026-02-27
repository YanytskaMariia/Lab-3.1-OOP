using LinkCollector.Models;
using LinkCollector.Services;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace LinkCollector.Forms
{
    /// <summary>
    /// Форма для додавання та редагування записів.
    /// Працює в двох режимах залежно від переданого параметра в конструктор.
    /// </summary>
    public partial class AddEditForm : Form
    {
        // Поля вводу
        private TextBox txtTitle, txtAuthor, txtUrl;
        private NumericUpDown numYear;
        private ComboBox cmbCategory, cmbType;

        // Посилання на об'єкт, який редагуємо (null, якщо створюємо новий)
        private ResourceLink _linkToEdit;

        // Інжектований репозиторій (не використовуємо статичний клас)
        private readonly ILinkRepository _repo;

        /// <summary>
        /// Конструктор без параметрів (потрібен для коректної роботи VS Designer).
        /// Делегує до основного конструктора з дефолтним InMemory репозиторієм.
        /// </summary>
        public AddEditForm() : this(new LinkRepository(), null) { }

        /// <summary>
        /// Конструктор, що відкриває форму в режимі редагування (для зручності виклику).
        /// Делегує до основного конструктора, передаючи репозиторій за замовчуванням.
        /// </summary>
        /// <param name="link">Об'єкт для редагування або null для нового запису.</param>
        public AddEditForm(ResourceLink link) : this(new LinkRepository(), link) { }

        /// <summary>
        /// Основний конструктор з підтримкою Dependency Injection.
        /// </summary>
        /// <param name="repo">Репозиторій для збереження/отримання даних.</param>
        /// <param name="link">Об'єкт для редагування або null для нового запису.</param>
        public AddEditForm(ILinkRepository repo, ResourceLink link = null)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _linkToEdit = link;

            InitializeComponent();

            // Налаштування зовнішнього вигляду вікна
            this.Text = link == null ? "Створити запис" : "Редагування посилання";
            this.Size = new Size(480, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 10);

            InitializeCustomUI();

            // Якщо редагуємо - завантажуємо дані в поля
            if (link != null) LoadData();
        }

        /// <summary>
        /// Побудова інтерфейсу форми.
        /// </summary>
        private void InitializeCustomUI()
        {
            this.Controls.Clear();
            int y = 20;

            // Локальна функція для створення рядків "Назва: [Контрол]"
            void AddField(string label, Control ctrl)
            {
                this.Controls.Add(new Label { Text = label, Location = new Point(20, y + 3), Width = 110, ForeColor = Color.DarkSlateGray });
                ctrl.Location = new Point(140, y);
                ctrl.Width = 280;
                this.Controls.Add(ctrl);
                y += 50;
            }

            txtTitle = new TextBox(); AddField("Назва:", txtTitle);
            txtAuthor = new TextBox(); AddField("Автор:", txtAuthor);
            txtUrl = new TextBox(); AddField("URL / Джерело:", txtUrl);

            // Максимальний рік обмежений поточним, згідно з правилами репозиторію
            numYear = new NumericUpDown { Minimum = 1800, Maximum = DateTime.Now.Year, Value = DateTime.Now.Year };
            AddField("Рік видання:", numYear);

            // Використовуємо інжектований репозиторій замість статичного виклику
            cmbCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, DataSource = _repo.GetCategories() };
            AddField("Категорія:", cmbCategory);

            cmbType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, DataSource = Enum.GetValues(typeof(LinkType)) };
            AddField("Тип ресурсу:", cmbType);

            // Кнопка Зберегти
            Button btnSave = new Button
            {
                Text = "Зберегти",
                Location = new Point(140, y + 10),
                Width = 130,
                Height = 40,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.Click += BtnSave_Click;

            // Кнопка Скасувати
            Button btnCancel = new Button
            {
                Text = "Скасувати",
                Location = new Point(290, y + 10),
                Width = 130,
                Height = 40,
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] { btnSave, btnCancel });
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadData()
        {
            txtTitle.Text = _linkToEdit.Title;
            txtAuthor.Text = _linkToEdit.Author;
            txtUrl.Text = _linkToEdit.UrlOrSource;
            numYear.Value = Math.Min(Math.Max(_linkToEdit.Year, numYear.Minimum), numYear.Maximum);
            cmbCategory.SelectedItem = _linkToEdit.Category;
            cmbType.SelectedItem = _linkToEdit.Type;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Базова перевірка на рівні UI
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Будь ласка, заповніть обов'язкові поля: Назва та Автор.", "Валідація", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_linkToEdit == null)
                {
                    // Додавання нового запису через інжектований репозиторій
                    _repo.Add(new ResourceLink
                    {
                        Title = txtTitle.Text.Trim(),
                        Author = txtAuthor.Text.Trim(),
                        UrlOrSource = txtUrl.Text.Trim(),
                        Year = (int)numYear.Value,
                        Category = cmbCategory.SelectedItem?.ToString() ?? "Без категорії",
                        Type = cmbType.SelectedItem is LinkType type ? type : LinkType.WebResource
                    });
                }
                else
                {
                    // Редагування існуючого об'єкта
                    _linkToEdit.Title = txtTitle.Text.Trim();
                    _linkToEdit.Author = txtAuthor.Text.Trim();
                    _linkToEdit.UrlOrSource = txtUrl.Text.Trim();
                    _linkToEdit.Year = (int)numYear.Value;
                    _linkToEdit.Category = cmbCategory.SelectedItem?.ToString();
                    _linkToEdit.Type = (LinkType)cmbType.SelectedItem;
                }

                // Якщо код вище не викинув Exception, закриваємо форму з успіхом
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (ArgumentException ex)
            {
                // Обробка помилок валідації з репозиторію (наприклад, рік з майбутнього)
                MessageBox.Show(ex.Message, "Помилка збереження", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася непередбачена помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddEditForm_Load(object sender, EventArgs e)
        {

        }
    }
}