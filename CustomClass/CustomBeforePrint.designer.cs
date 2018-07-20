namespace CustomClass
{
    partial class CustomBeforePrint
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gridLookUpEdit1 = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gCRDes = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gCRFile = new DevExpress.XtraGrid.Columns.GridColumn();
            this.simpleButtonIn = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonSuaMau = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonExit = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonChapNhan = new DevExpress.XtraEditors.SimpleButton();
            this.textEditSoCTGoc = new DevExpress.XtraEditors.TextEdit();
            this.textEditSoLien = new DevExpress.XtraEditors.TextEdit();
            this.textEditTitle = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditSoCTGoc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditSoLien.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTitle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridLookUpEdit1);
            this.layoutControl1.Controls.Add(this.simpleButtonIn);
            this.layoutControl1.Controls.Add(this.simpleButtonSuaMau);
            this.layoutControl1.Controls.Add(this.simpleButtonExit);
            this.layoutControl1.Controls.Add(this.simpleButtonChapNhan);
            this.layoutControl1.Controls.Add(this.textEditSoCTGoc);
            this.layoutControl1.Controls.Add(this.textEditSoLien);
            this.layoutControl1.Controls.Add(this.textEditTitle);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(646, 141);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridLookUpEdit1
            // 
            this.gridLookUpEdit1.EditValue = "[ Chọn mẫu]";
            this.gridLookUpEdit1.Location = new System.Drawing.Point(107, 38);
            this.gridLookUpEdit1.Name = "gridLookUpEdit1";
            this.gridLookUpEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.gridLookUpEdit1.Properties.DisplayMember = "RDes";
            this.gridLookUpEdit1.Properties.ValidateOnEnterKey = true;
            this.gridLookUpEdit1.Properties.ValueMember = "RFile";
            this.gridLookUpEdit1.Properties.View = this.gridLookUpEdit1View;
            this.gridLookUpEdit1.Size = new System.Drawing.Size(533, 20);
            this.gridLookUpEdit1.StyleController = this.layoutControl1;
            this.gridLookUpEdit1.TabIndex = 10;
            this.gridLookUpEdit1.EditValueChanged += new System.EventHandler(this.gridLookUpEdit1_EditValueChanged);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gCRDes,
            this.gCRFile});
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gCRDes
            // 
            this.gCRDes.Caption = "Tên mẫu";
            this.gCRDes.FieldName = "RDes";
            this.gCRDes.Name = "gCRDes";
            this.gCRDes.Visible = true;
            this.gCRDes.VisibleIndex = 0;
            // 
            // gCRFile
            // 
            this.gCRFile.Caption = "File mẫu";
            this.gCRFile.FieldName = "RFile";
            this.gCRFile.Name = "gCRFile";
            this.gCRFile.Visible = true;
            this.gCRFile.VisibleIndex = 1;
            // 
            // simpleButtonIn
            // 
            this.simpleButtonIn.Location = new System.Drawing.Point(7, 100);
            this.simpleButtonIn.Name = "simpleButtonIn";
            this.simpleButtonIn.Size = new System.Drawing.Size(231, 22);
            this.simpleButtonIn.StyleController = this.layoutControl1;
            this.simpleButtonIn.TabIndex = 0;
            this.simpleButtonIn.Text = "F7-In";
            this.simpleButtonIn.Click += new System.EventHandler(this.simpleButtonIn_Click);
            // 
            // simpleButtonSuaMau
            // 
            this.simpleButtonSuaMau.Location = new System.Drawing.Point(342, 100);
            this.simpleButtonSuaMau.Name = "simpleButtonSuaMau";
            this.simpleButtonSuaMau.Size = new System.Drawing.Size(134, 22);
            this.simpleButtonSuaMau.StyleController = this.layoutControl1;
            this.simpleButtonSuaMau.TabIndex = 9;
            this.simpleButtonSuaMau.Text = "Sửa mẫu";
            this.simpleButtonSuaMau.Click += new System.EventHandler(this.simpleButtonSuaMau_Click);
            // 
            // simpleButtonExit
            // 
            this.simpleButtonExit.Location = new System.Drawing.Point(487, 100);
            this.simpleButtonExit.Name = "simpleButtonExit";
            this.simpleButtonExit.Size = new System.Drawing.Size(153, 22);
            this.simpleButtonExit.StyleController = this.layoutControl1;
            this.simpleButtonExit.TabIndex = 8;
            this.simpleButtonExit.Text = "Quay ra";
            this.simpleButtonExit.Click += new System.EventHandler(this.simpleButtonExit_Click);
            // 
            // simpleButtonChapNhan
            // 
            this.simpleButtonChapNhan.Location = new System.Drawing.Point(249, 100);
            this.simpleButtonChapNhan.Name = "simpleButtonChapNhan";
            this.simpleButtonChapNhan.Size = new System.Drawing.Size(82, 22);
            this.simpleButtonChapNhan.StyleController = this.layoutControl1;
            this.simpleButtonChapNhan.TabIndex = 7;
            this.simpleButtonChapNhan.Text = "Xem";
            this.simpleButtonChapNhan.Click += new System.EventHandler(this.simpleButtonChapNhan_Click);
            // 
            // textEditSoCTGoc
            // 
            this.textEditSoCTGoc.Location = new System.Drawing.Point(385, 69);
            this.textEditSoCTGoc.Name = "textEditSoCTGoc";
            this.textEditSoCTGoc.Size = new System.Drawing.Size(255, 20);
            this.textEditSoCTGoc.StyleController = this.layoutControl1;
            this.textEditSoCTGoc.TabIndex = 6;
            // 
            // textEditSoLien
            // 
            this.textEditSoLien.Location = new System.Drawing.Point(107, 69);
            this.textEditSoLien.Name = "textEditSoLien";
            this.textEditSoLien.Size = new System.Drawing.Size(167, 20);
            this.textEditSoLien.StyleController = this.layoutControl1;
            this.textEditSoLien.TabIndex = 5;
            // 
            // textEditTitle
            // 
            this.textEditTitle.Location = new System.Drawing.Point(107, 7);
            this.textEditTitle.Name = "textEditTitle";
            this.textEditTitle.Size = new System.Drawing.Size(533, 20);
            this.textEditTitle.StyleController = this.layoutControl1;
            this.textEditTitle.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.layoutControlItem6,
            this.layoutControlItem3,
            this.layoutControlItem5,
            this.layoutControlItem7,
            this.layoutControlItem8});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(646, 141);
            this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.textEditTitle;
            this.layoutControlItem1.CustomizationFormText = "Tiêu đề";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem1.Size = new System.Drawing.Size(644, 31);
            this.layoutControlItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Text = "Tiêu đề";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(95, 20);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.textEditSoLien;
            this.layoutControlItem2.CustomizationFormText = "Số liên";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 62);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem2.Size = new System.Drawing.Size(278, 31);
            this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Text = "Số liên";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(95, 20);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.simpleButtonChapNhan;
            this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
            this.layoutControlItem4.Location = new System.Drawing.Point(242, 93);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem4.Size = new System.Drawing.Size(93, 46);
            this.layoutControlItem4.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem4.Text = "layoutControlItem4";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextToControlDistance = 0;
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.simpleButtonSuaMau;
            this.layoutControlItem6.CustomizationFormText = "layoutControlItem6";
            this.layoutControlItem6.Location = new System.Drawing.Point(335, 93);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem6.Size = new System.Drawing.Size(145, 46);
            this.layoutControlItem6.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem6.Text = "layoutControlItem6";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextToControlDistance = 0;
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.textEditSoCTGoc;
            this.layoutControlItem3.CustomizationFormText = "Số CT gốc kèm theo";
            this.layoutControlItem3.Location = new System.Drawing.Point(278, 62);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem3.Size = new System.Drawing.Size(366, 31);
            this.layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem3.Text = "Số CT gốc kèm theo";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(95, 20);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.simpleButtonExit;
            this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
            this.layoutControlItem5.Location = new System.Drawing.Point(480, 93);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem5.Size = new System.Drawing.Size(164, 46);
            this.layoutControlItem5.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem5.Text = "layoutControlItem5";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextToControlDistance = 0;
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.simpleButtonIn;
            this.layoutControlItem7.CustomizationFormText = "layoutControlItem7";
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 93);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem7.Size = new System.Drawing.Size(242, 46);
            this.layoutControlItem7.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem7.Text = "layoutControlItem7";
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextToControlDistance = 0;
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.gridLookUpEdit1;
            this.layoutControlItem8.CustomizationFormText = "Mẫu in";
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 31);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem8.Size = new System.Drawing.Size(644, 31);
            this.layoutControlItem8.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem8.Text = "Mẫu in";
            this.layoutControlItem8.TextSize = new System.Drawing.Size(95, 20);
            // 
            // BeforePrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 141);
            this.Controls.Add(this.layoutControl1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "BeforePrint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thông số in";
            this.Load += new System.EventHandler(this.BeforePrint_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BeforePrint_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditSoCTGoc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditSoLien.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTitle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton simpleButtonExit;
        private DevExpress.XtraEditors.SimpleButton simpleButtonChapNhan;
        private DevExpress.XtraEditors.TextEdit textEditSoCTGoc;
        private DevExpress.XtraEditors.TextEdit textEditSoLien;
        private DevExpress.XtraEditors.TextEdit textEditTitle;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SimpleButton simpleButtonSuaMau;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.SimpleButton simpleButtonIn;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.GridLookUpEdit gridLookUpEdit1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraGrid.Columns.GridColumn gCRDes;
        private DevExpress.XtraGrid.Columns.GridColumn gCRFile;
    }
}