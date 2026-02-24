using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using LinkCollector.Models;
using LinkCollector.Services;

namespace LinkCollector.Forms
{
    /// <summary>
    /// Форма експорту накопичених посилань у файл.
    /// Дозволяє користувачеві вибрати один із підтримуваних стандартів цитування (ДСТУ, Harvard, BibTeX).
    /// </summary>
    public partial class ExportForm : Form
    {
        private RadioButton rbDstu, rbHarvard, rbBibtex; // Перемикачі вибору формату
        private readonly CitationService _citationService = new CitationService(); // Сервіс генерації тексту

        /// <summary>
        /// Ініціалізує новий екземпляр форми <see cref="ExportForm"/>.
        /// </summary>
        public ExportForm()
        {
            InitializeComponent();

            this.Text = "Експорт посилань";
            this.Size = new Size(350, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            InitializeCustomUI();
        }

        /// <summary>
        /// Динамічна побудова графічного інтерфейсу для вибору формату експорту.
        /// </summary>
        private void InitializeCustomUI()
        {
            this.Controls.Clear();

            // Група перемикачів для вибору стилю цитування
            GroupBox box = new GroupBox
            {
                Text = "Оберіть формат цитування",
                Location = new Point(20, 15),
                Size = new Size(290, 140),
                ForeColor = Color.DarkSlateGray
            };

            rbDstu = new RadioButton { Text = "ДСТУ 8302:2015 (Україна)", Location = new Point(20, 30), Checked = true, AutoSize = true };
            rbHarvard = new RadioButton { Text = "Harvard Style (Міжнародний)", Location = new Point(20, 65), AutoSize = true };
            rbBibtex = new RadioButton { Text = "BibTeX (для LaTeX документів)", Location = new Point(20, 100), AutoSize = true };

            box.Controls.AddRange(new Control[] { rbDstu, rbHarvard, rbBibtex });
            this.Controls.Add(box);

            // Кнопка запуску процесу експорту
            Button btnExport = new Button
            {
                Text = "Зберегти у файл...",
                Location = new Point(60, 175),
                Width = 210,
                Height = 45,
                BackColor = Color.SlateGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExport.Click += BtnExport_Click;
            this.Controls.Add(btnExport);
        }

        /// <summary>
        /// Обробка натискання кнопки експорту. 
        /// Відкриває діалогове вікно збереження файлу та записує згенерований список.
        /// </summary>
        private void BtnExport_Click(object sender, EventArgs e)
        {
            // 1. Визначаємо обраний стиль та розширення файлу
            CitationStyle style = CitationStyle.DSTU_8302;
            string extension = "txt";

            if (rbHarvard.Checked)
            {
                style = CitationStyle.Harvard;
            }
            else if (rbBibtex.Checked)
            {
                style = CitationStyle.BibTeX;
                extension = "bib";
            }

            // 2. Налаштування та відкриття діалогу збереження
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = $"Файли (*.{extension})|*.{extension}|Усі файли (*.*)|*.*";
                sfd.FileName = $"references.{extension}";
                sfd.Title = "Зберегти бібліографічний список";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 3. Генерація контенту через сервіс цитування
                        // Використовуємо всі посилання з репозиторію
                        string content = _citationService.GenerateList(LinkRepository.GetAll(), style);

                        // 4. Запис у файл із підтримкою UTF-8 для коректного відображення кирилиці
                        File.WriteAllText(sfd.FileName, content, Encoding.UTF8);

                        MessageBox.Show("Дані успішно експортовані!", "Експорт завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при збереженні файлу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportForm_Load(object sender, EventArgs e) { }
    }
}