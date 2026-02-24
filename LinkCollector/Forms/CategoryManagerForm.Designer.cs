namespace LinkCollector.Forms
{
    partial class CategoryManagerForm
    {
        /// <summary>
        /// Обов'язкова змінна дизайнера.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Звільнення всіх ресурсів, що використовуються.
        /// </summary>
        /// <param name="disposing">true, якщо керовані ресурси мають бути видалені; інакше false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматично згенерований дизайнером форм

        /// <summary>
        /// Метод для підтримки дизайнера — не змінюйте 
        /// вміст цього методу за допомогою редактора коду.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CategoryManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 461);
            this.Name = "CategoryManagerForm";
            this.Text = "Керування категоріями";
            this.Load += new System.EventHandler(this.CategoryManagerForm_Load);
            this.ResumeLayout(false);
        }

        #endregion
    }
}