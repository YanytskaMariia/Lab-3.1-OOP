namespace LinkCollector.Forms
{
    partial class AddEditForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // AddEditForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(464, 441);
            Name = "AddEditForm";
            Text = "Додати/Редагувати";
            Load += AddEditForm_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}