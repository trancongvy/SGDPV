namespace CusPOS
{
    partial class fPOS
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tTimer = new DevExpress.XtraEditors.LabelControl();
            this.tws = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.tTimer);
            this.panelControl1.Controls.Add(this.tws);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(6);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1827, 287);
            this.panelControl1.TabIndex = 1;
            // 
            // tTimer
            // 
            this.tTimer.Location = new System.Drawing.Point(706, 25);
            this.tTimer.Margin = new System.Windows.Forms.Padding(6);
            this.tTimer.Name = "tTimer";
            this.tTimer.Size = new System.Drawing.Size(97, 25);
            this.tTimer.TabIndex = 4;
            this.tTimer.Text = "Thu ngân:";
            // 
            // tws
            // 
            this.tws.Location = new System.Drawing.Point(136, 23);
            this.tws.Margin = new System.Windows.Forms.Padding(6);
            this.tws.Name = "tws";
            this.tws.Size = new System.Drawing.Size(97, 25);
            this.tws.TabIndex = 3;
            this.tws.Text = "Thu ngân:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(490, 25);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(6);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(181, 25);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Ngày giờ hệ thống:";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(26, 25);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(6);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(97, 25);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Thu ngân:";
            // 
            // panelControl2
            // 
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 287);
            this.panelControl2.Margin = new System.Windows.Forms.Padding(6);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1827, 663);
            this.panelControl2.TabIndex = 2;
            // 
            // fPOS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1827, 950);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "fPOS";
            this.Text = "fPOS";
            this.Load += new System.EventHandler(this.fPOS_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl tTimer;
        private DevExpress.XtraEditors.LabelControl tws;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
    }
}