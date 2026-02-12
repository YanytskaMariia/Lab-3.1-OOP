using System;
using System.Drawing;
using System.Windows.Forms;
using LinkCollector.Models;
using LinkCollector.Services;

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

        /// <summary>
        /// Конструктор без параметрів (потрібен для коректної роботи VS Designer).
        /// </summary>
        public AddEditForm() : this(null) { }

        /// <summary>
        /// Основний конструктор.
        /// </summary>
        /// <param name="link">Об'єкт для редагування або null для нового запису.</param>
        public AddEditForm(ResourceLink link = null)
        {
            InitializeComponent();
            _linkToEdit = link;

            // Змінюємо заголовок вікна залежно від режиму
            this.Text = link == null ? "Створити запис" : "Редагування";
            this.Size = new Size(480, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Заборона зміни розміру
            this.MaximizeBox = false; // Прибрати кнопку розгортання
            this.Font = new Font("Segoe UI", 10);

            InitializeCustomUI();

            // Якщо редагуємо - заповнюємо поля даними
            if (link != null) LoadData();
        }

        /// <summary>
        /// Побудова інтерфейсу форми.
        /// </summary>
        private void InitializeCustomUI()
        {
            this.Controls.Clear();
            int y = 20;

            // Локальна функція для швидкого додавання рядків (Label + Control)
            void AddField(string label, Control ctrl)
            {
                this.Controls.Add(new Label { Text = label, Location = new Point(20, y + 3), Width = 110, ForeColor = Color.DarkSlateGray });
                ctrl.Location = new Point(140, y);
                ctrl.Width = 280;
                this.Controls.Add(ctrl);
                y += 50; // Зміщення вниз для наступного елемента
            }

            txtTitle = new TextBox(); AddField("Назва:", txtTitle);
            txtAuthor = new TextBox(); AddField("Автор:", txtAuthor);
            txtUrl = new TextBox(); AddField("URL / Видавн.:", txtUrl);

            numYear = new NumericUpDown { Minimum = 1800, Maximum = 2200, Value = DateTime.Now.Year };
            AddField("Рік видання:", numYear);

            // Заповнюємо випадаючі списки даними
            cmbCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, DataSource = LinkRepository.GetCategories() };
            AddField("Категорія:", cmbCategory);

            cmbType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, DataSource = Enum.GetValues(typeof(LinkType)) };
            AddField("Тип ресурсу:", cmbType);

            // Кнопки дій
            Button btnSave = new Button
            {
                Text = "Зберегти",
                Location = new Point(140, y + 10),
                Width = 130,
                Height = 40,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK // Вказує, що форма закриється успішно
            };
            btnSave.Click += BtnSave_Click;

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
            this.AcceptButton = btnSave; // Клавіша Enter спрацює як "Зберегти"
        }

        /// <summary>
        /// Завантаження даних з об'єкта у поля форми.
        /// </summary>
        private void LoadData()
        {
            txtTitle.Text = _linkToEdit.Title;
            txtAuthor.Text = _linkToEdit.Author;
            txtUrl.Text = _linkToEdit.UrlOrSource;
            numYear.Value = _linkToEdit.Year;
            cmbCategory.SelectedItem = _linkToEdit.Category;
            cmbType.SelectedItem = _linkToEdit.Type;
        }

        /// <summary>
        /// Обробка натискання кнопки "Зберегти".
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Валідація: Назва обов'язкова
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Назва не може бути порожньою!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None; // Не закривати форму
                return;
            }

            if (_linkToEdit == null)
            {
                // Логіка додавання нового запису
                LinkRepository.Add(new ResourceLink
                {
                    Title = txtTitle.Text,
                    Author = txtAuthor.Text,
                    UrlOrSource = txtUrl.Text,
                    Year = (int)numYear.Value,
                    Category = cmbCategory.SelectedItem?.ToString() ?? "",
                    Type = (LinkType)cmbType.SelectedItem
                });
            }
            else
            {
                // Логіка редагування існуючого
                _linkToEdit.Title = txtTitle.Text; _linkToEdit.Author = txtAuthor.Text;
                _linkToEdit.UrlOrSource = txtUrl.Text; _linkToEdit.Year = (int)numYear.Value;
                _linkToEdit.Category = cmbCategory.SelectedItem?.ToString();
                _linkToEdit.Type = (LinkType)cmbType.SelectedItem;
            }
        }
    }
}