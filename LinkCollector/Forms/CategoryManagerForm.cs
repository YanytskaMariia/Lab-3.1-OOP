using System;
using System.Drawing;
using System.Windows.Forms;
using LinkCollector.Services;

namespace LinkCollector.Forms
{
    /// <summary>
    /// Форма для керування категоріями (контекстами).
    /// Дозволяє додавати нові категорії в репозиторій та видаляти їх.
    /// </summary>
    public partial class CategoryManagerForm : Form
    {
        private ListBox lstCategories;  // Список категорій
        private TextBox txtNewCategory; // Поле для нової назви

        public CategoryManagerForm()
        {
            InitializeComponent();

            this.Text = "Керування категоріями";
            this.Size = new Size(360, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10);

            InitializeCustomUI();
        }

        private void InitializeCustomUI()
        {
            this.Controls.Clear();

            Label lblInfo = new Label { Text = "Список доступних категорій:", Location = new Point(15, 10), AutoSize = true, ForeColor = Color.Gray };
            this.Controls.Add(lblInfo);

            // Список (ListBox)
            lstCategories = new ListBox { Location = new Point(15, 35), Size = new Size(310, 300) };
            RefreshList();
            this.Controls.Add(lstCategories);

            // Поле вводу
            txtNewCategory = new TextBox { Location = new Point(15, 350), Size = new Size(200, 27), PlaceholderText = "Нова назва..." };
            this.Controls.Add(txtNewCategory);

            // Кнопка додавання
            Button btnAdd = new Button
            {
                Text = "Додати",
                Location = new Point(225, 349),
                Size = new Size(100, 29),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.Click += (s, e) => {
                if (!string.IsNullOrWhiteSpace(txtNewCategory.Text))
                {
                    LinkRepository.AddCategory(txtNewCategory.Text);
                    RefreshList(); // Оновлюємо список на екрані
                    txtNewCategory.Clear();
                }
            };
            this.Controls.Add(btnAdd);

            // Кнопка видалення
            Button btnRemove = new Button
            {
                Text = "Видалити обрану категорію",
                Location = new Point(15, 390),
                Size = new Size(310, 35),
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRemove.Click += (s, e) => {
                if (lstCategories.SelectedItem != null)
                {
                    LinkRepository.RemoveCategory(lstCategories.SelectedItem.ToString());
                    RefreshList();
                }
            };
            this.Controls.Add(btnRemove);
        }

        /// <summary>
        /// Оновлення списку категорій з репозиторію.
        /// </summary>
        private void RefreshList()
        {
            lstCategories.DataSource = null;
            lstCategories.DataSource = LinkRepository.GetCategories();
        }
    }
}