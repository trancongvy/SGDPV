namespace FormFactory
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonAccept = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // dxErrorProviderMain
            // 
            this.dxErrorProviderMain.ContainerControl = this;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.simpleButtonCancel);
            this.layoutControl1.Controls.Add(this.simpleButtonAccept);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.layoutControl1.Location = new System.Drawing.Point(0, 292);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(481, 41);
            this.layoutControl1.TabIndex = 10;
            this.layoutControl1.TabStop = false;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // simpleButtonCancel
            // 
            this.simpleButtonCancel.Location = new System.Drawing.Point(315, 6);
            this.simpleButtonCancel.Name = "simpleButtonCancel";
            this.simpleButtonCancel.Size = new System.Drawing.Size(149, 22);
            this.simpleButtonCancel.StyleController = this.layoutControl1;
            this.simpleButtonCancel.TabIndex = 5;
            this.simpleButtonCancel.Text = "Esc - Quay ra";
            this.simpleButtonCancel.Click += new System.EventHandler(this.simpleButtonCancel_Click);
            // 
            // simpleButtonAccept
            // 
            this.simpleButtonAccept.Location = new System.Drawing.Point(123, 6);
            this.simpleButtonAccept.Name = "simpleButtonAccept";
            this.simpleButtonAccept.Size = new System.Drawing.Size(182, 22);
            this.simpleButtonAccept.StyleController = this.layoutControl1;
            this.simpleButtonAccept.TabIndex = 4;
            this.simpleButtonAccept.Text = "F12 - Chấp nhận";
            this.simpleButtonAccept.Click += new System.EventHandler(this.simpleButtonAccept_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.emptySpaceItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(481, 41);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.simpleButtonAccept;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(117, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem1.Size = new System.Drawing.Size(192, 39);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.simpleButtonCancel;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(309, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem2.Size = new System.Drawing.Size(159, 39);
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.emptySpaceItem1.Size = new System.Drawing.Size(117, 39);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
            this.emptySpaceItem2.Location = new System.Drawing.Point(468, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.emptySpaceItem2.Size = new System.Drawing.Size(11, 39);
            this.emptySpaceItem2.Text = "emptySpaceItem2";
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // ReportFilter
            // 
            this.ClientSize = new System.Drawing.Size(481, 333);
            this.Controls.Add(this.layoutControl1);
            this.KeyPreview = true;
            this.Name = "ReportFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmTest";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ReportFilter_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProviderMain;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButtonCancel;
        private DevExpress.XtraEditors.SimpleButton simpleButtonAccept;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;


    }
}