using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LinkCollector.Models;
using LinkCollector.Services;

namespace LinkCollector.Forms
{
    /// <summary>
    /// Форма експорту даних у файл.
    /// Дозволяє вибрати стандарт цитування (ДСТУ, Harvard, BibTeX).
    /// </summary>
    public partial class ExportForm : Form
    {
        private RadioButton rbDstu, rbHarvard, rbBibtex; // Перемикачі вибору формату
        private CitationService _citationService = new CitationService(); // Сервіс генерації тексту

        public ExportForm()
        {
            InitializeComponent();

            this.Text = "Експорт посилань";
            this.Size = new Size(350, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            InitializeCustomUI();
        }

        private void InitializeCustomUI()
        {
            this.Controls.Clear();

            // Група перемикачів
            GroupBox box = new GroupBox { Text = "Оберіть формат файлу", Location = new Point(20, 15), Size = new Size(290, 140) };

            rbDstu = new RadioButton { Text = "ДСТУ 8302:2015 (Україна)", Location = new Point(20, 30), Checked = true, AutoSize = true };
            rbHarvard = new RadioButton { Text = "Harvard Style", Location = new Point(20, 65), AutoSize = true };
            rbBibtex = new RadioButton { Text = "BibTeX (для LaTeX)", Location = new Point(20, 100), AutoSize = true };

            box.Controls.AddRange(new Control[] { rbDstu, rbHarvard, rbBibtex });
            this.Controls.Add(box);

            // Кнопка збереження
            Button btnExport = new Button
            {
                Text = "Зберегти у файл...",
                Location = new Point(60, 170),
                Width = 210,
                Height = 40,
                BackColor = Color.SlateGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExport.Click += BtnExport_Click;
            this.Controls.Add(btnExport);
        }

        /// <summary>
        /// Логіка збереження файлу через SaveFileDialog.
        /// </summary>
        private void BtnExport_Click(object sender, EventArgs e)
        {
            // 1. Визначаємо стиль
            CitationStyle style = CitationStyle.DSTU_8302;
            string ext = "txt"; // Розширення файлу за замовчуванням

            if (rbHarvard.Checked) style = CitationStyle.Harvard;
            if (rbBibtex.Checked) { style = CitationStyle.BibTeX; ext = "bib"; }

            // 2. Відкриваємо діалог вибору місця збереження
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = $"Text files (*.{ext})|*.{ext}|All files (*.*)|*.*";
                sfd.FileName = $"references.{ext}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // 3. Генеруємо текст і записуємо у файл
                    string content = _citationService.GenerateList(LinkRepository.GetAll(), style);
                    File.WriteAllText(sfd.FileName, content);

                    MessageBox.Show("Файл успішно збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close(); // Закриваємо форму після успішного експорту
                }
            }
        }
    }
}