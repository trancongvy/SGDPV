namespace ReportFactory
{
    partial class ReportFilter
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
            this.dxErrorProviderMain = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonAccept = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderMain)).BeginInit();
            this.SuspendLayout();
            // 
            // dxErrorProviderMain
            // 
            this.dxErrorProviderMain.ContainerControl = this;
            // 
            // simpleButtonCancel
            // 
            this.simpleButtonCancel.Location = new System.Drawing.Point(658, 598);
            this.simpleButtonCancel.Name = "simpleButtonCancel";
            this.simpleButtonCancel.Size = new System.Drawing.Size(651, 41);
            this.simpleButtonCancel.TabIndex = 5;
            this.simpleButtonCancel.Text = "Esc - Quay ra";
            this.simpleButtonCancel.Click += new System.EventHandler(this.simpleButtonCancel_Click);
            // 
            // simpleButtonAccept
            // 
            this.simpleButtonAccept.Location = new System.Drawing.Point(167, 616);
            this.simpleButtonAccept.Name = "simpleButtonAccept";
            this.simpleButtonAccept.Size = new System.Drawing.Size(355, 41);
            this.simpleButtonAccept.TabIndex = 4;
            this.simpleButtonAccept.Text = "F12 - Chấp nhận";
            this.simpleButtonAccept.Click += new System.EventHandler(this.simpleButtonAccept_Click);
            // 
            // ReportFilter
            // 
            this.ClientSize = new System.Drawing.Size(1376, 763);
            this.Controls.Add(this.simpleButtonAccept);
            this.Controls.Add(this.simpleButtonCancel);
            this.KeyPreview = true;
            this.Name = "ReportFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmTest";
            this.Load += new System.EventHandler(this.ReportFilter_Load_1);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ReportFilter_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProviderMain;
        private DevExpress.XtraEditors.SimpleButton simpleButtonCancel;
        private DevExpress.XtraEditors.SimpleButton simpleButtonAccept;


    }
}