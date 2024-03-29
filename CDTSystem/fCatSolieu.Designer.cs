namespace CDTSystem
{
    partial class fCatSolieu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.cDateEdit1 = new CBSControls.CDateEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.vCheckEdit1 = new CBSControls.VCheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.cDateEdit1.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cDateEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vCheckEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(115, 57);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "Cắt";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // cDateEdit1
            // 
            this.cDateEdit1.EditValue = null;
            this.cDateEdit1.EnterMoveNextControl = true;
            this.cDateEdit1.Location = new System.Drawing.Point(115, 19);
            this.cDateEdit1.Margin = new System.Windows.Forms.Padding(2);
            this.cDateEdit1.Name = "cDateEdit1";
            this.cDateEdit1.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cDateEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cDateEdit1.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.cDateEdit1.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.cDateEdit1.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.cDateEdit1.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.cDateEdit1.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.cDateEdit1.Size = new System.Drawing.Size(100, 20);
            this.cDateEdit1.TabIndex = 3;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(18, 21);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(82, 13);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "Ngày chốt số liệu";
            // 
            // vCheckEdit1
            // 
            this.vCheckEdit1.EditValue = true;
            this.vCheckEdit1.Location = new System.Drawing.Point(246, 20);
            this.vCheckEdit1.Margin = new System.Windows.Forms.Padding(2);
            this.vCheckEdit1.Name = "vCheckEdit1";
            this.vCheckEdit1.Properties.Caption = "Tạo phía máy client";
            this.vCheckEdit1.Size = new System.Drawing.Size(142, 19);
            this.vCheckEdit1.TabIndex = 5;
            // 
            // fCatSolieu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 96);
            this.Controls.Add(this.vCheckEdit1);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.cDateEdit1);
            this.Controls.Add(this.simpleButton1);
            this.Name = "fCatSolieu";
            this.Text = "Chốt số liệu & Copy qua database mới";
            this.Load += new System.EventHandler(this.fCopyPackage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cDateEdit1.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cDateEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vCheckEdit1.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private CBSControls.CDateEdit cDateEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private CBSControls.VCheckEdit vCheckEdit1;
    }
}