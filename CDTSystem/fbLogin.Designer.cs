namespace CDTSystem
{
    partial class fbLogin
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
            this.tbGo = new DevExpress.XtraEditors.SimpleButton();
            this.tUrl = new DevExpress.XtraEditors.TextEdit();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tBack = new DevExpress.XtraEditors.SimpleButton();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.tUrl.Properties)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbGo
            // 
            this.tbGo.Dock = System.Windows.Forms.DockStyle.Right;
            this.tbGo.Location = new System.Drawing.Point(1519, 0);
            this.tbGo.Margin = new System.Windows.Forms.Padding(6);
            this.tbGo.Name = "tbGo";
            this.tbGo.Size = new System.Drawing.Size(159, 36);
            this.tbGo.TabIndex = 33;
            this.tbGo.Text = "Go";
            this.tbGo.Click += new System.EventHandler(this.tbGo_Click);
            // 
            // tUrl
            // 
            this.tUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tUrl.Location = new System.Drawing.Point(0, 0);
            this.tUrl.Margin = new System.Windows.Forms.Padding(6);
            this.tUrl.Name = "tUrl";
            this.tUrl.Size = new System.Drawing.Size(1519, 32);
            this.tUrl.TabIndex = 32;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tBack);
            this.panel1.Controls.Add(this.tUrl);
            this.panel1.Controls.Add(this.tbGo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1678, 36);
            this.panel1.TabIndex = 34;
            // 
            // tBack
            // 
            this.tBack.Dock = System.Windows.Forms.DockStyle.Left;
            this.tBack.Location = new System.Drawing.Point(0, 0);
            this.tBack.Margin = new System.Windows.Forms.Padding(6);
            this.tBack.Name = "tBack";
            this.tBack.Size = new System.Drawing.Size(78, 36);
            this.tBack.TabIndex = 34;
            this.tBack.Text = "<<";
            this.tBack.Click += new System.EventHandler(this.tBack_Click);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 36);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1678, 1246);
            this.panel2.TabIndex = 35;
            // 
            // fbLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1678, 1282);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "fbLogin";
            this.Text = "fbLogin";
            this.Load += new System.EventHandler(this.fbLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tUrl.Properties)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton tbGo;
        private DevExpress.XtraEditors.TextEdit tUrl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraEditors.SimpleButton tBack;
    }
}