﻿namespace CusPOS
{
    partial class fPOSKM
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
            this.label1 = new System.Windows.Forms.Label();
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            this.caPtCK = new DevExpress.XtraEditors.CalcEdit();
            this.grLoaiGia = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn16 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn17 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.tlLoaiMon = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.grdmMon = new DevExpress.XtraGrid.GridControl();
            this.gvMain = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.grPOSGia = new DevExpress.XtraGrid.GridControl();
            this.gvGia = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.caPtCK.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grLoaiGia.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlLoaiMon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdmMon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grPOSGia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvGia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCalcEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.label1);
            this.panelControl1.Controls.Add(this.simpleButton3);
            this.panelControl1.Controls.Add(this.caPtCK);
            this.panelControl1.Controls.Add(this.grLoaiGia);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(2855, 116);
            this.panelControl1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Chương trình KM";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // simpleButton3
            // 
            this.simpleButton3.Location = new System.Drawing.Point(2056, 12);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Size = new System.Drawing.Size(267, 79);
            this.simpleButton3.TabIndex = 2;
            this.simpleButton3.Text = "Update";
            this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
            // 
            // caPtCK
            // 
            this.caPtCK.Location = new System.Drawing.Point(671, 44);
            this.caPtCK.Name = "caPtCK";
            this.caPtCK.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.caPtCK.Size = new System.Drawing.Size(152, 32);
            this.caPtCK.TabIndex = 1;
            this.caPtCK.EditValueChanged += new System.EventHandler(this.caPtCK_EditValueChanged);
            // 
            // grLoaiGia
            // 
            this.grLoaiGia.Location = new System.Drawing.Point(191, 44);
            this.grLoaiGia.Name = "grLoaiGia";
            this.grLoaiGia.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.grLoaiGia.Properties.DisplayMember = "TenPOSLoaiGia";
            this.grLoaiGia.Properties.NullText = "";
            this.grLoaiGia.Properties.ValueMember = "MaPOSLoaiGia";
            this.grLoaiGia.Properties.View = this.gridLookUpEdit1View;
            this.grLoaiGia.Size = new System.Drawing.Size(469, 32);
            this.grLoaiGia.TabIndex = 0;
            this.grLoaiGia.EditValueChanged += new System.EventHandler(this.grLoaiGia_EditValueChanged);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn15,
            this.gridColumn16,
            this.gridColumn17});
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsBehavior.Editable = false;
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.EnableAppearanceEvenRow = true;
            this.gridLookUpEdit1View.OptionsView.EnableAppearanceOddRow = true;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn15
            // 
            this.gridColumn15.Caption = "Mã";
            this.gridColumn15.FieldName = "MaPOSLoaiGia";
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.Visible = true;
            this.gridColumn15.VisibleIndex = 0;
            // 
            // gridColumn16
            // 
            this.gridColumn16.Caption = "Diễn giải";
            this.gridColumn16.FieldName = "TenPOSLoaiGia";
            this.gridColumn16.Name = "gridColumn16";
            this.gridColumn16.Visible = true;
            this.gridColumn16.VisibleIndex = 1;
            // 
            // gridColumn17
            // 
            this.gridColumn17.Caption = "Tỉ lệ";
            this.gridColumn17.DisplayFormat.FormatString = "##0";
            this.gridColumn17.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn17.FieldName = "Tile";
            this.gridColumn17.Name = "gridColumn17";
            this.gridColumn17.Visible = true;
            this.gridColumn17.VisibleIndex = 2;
            // 
            // tlLoaiMon
            // 
            this.tlLoaiMon.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn2});
            this.tlLoaiMon.Dock = System.Windows.Forms.DockStyle.Left;
            this.tlLoaiMon.Location = new System.Drawing.Point(0, 116);
            this.tlLoaiMon.Name = "tlLoaiMon";
            this.tlLoaiMon.OptionsBehavior.Editable = false;
            this.tlLoaiMon.OptionsView.EnableAppearanceEvenRow = true;
            this.tlLoaiMon.OptionsView.EnableAppearanceOddRow = true;
            this.tlLoaiMon.Size = new System.Drawing.Size(479, 1088);
            this.tlLoaiMon.TabIndex = 1;
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "Loại món";
            this.treeListColumn1.FieldName = "MaLoaiMon";
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            this.treeListColumn1.Width = 164;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "Tên";
            this.treeListColumn2.FieldName = "TenLoaiMon";
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 1;
            this.treeListColumn2.Width = 297;
            // 
            // splitterControl1
            // 
            this.splitterControl1.Location = new System.Drawing.Point(479, 116);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(5, 1088);
            this.splitterControl1.TabIndex = 2;
            this.splitterControl1.TabStop = false;
            // 
            // grdmMon
            // 
            this.grdmMon.Dock = System.Windows.Forms.DockStyle.Left;
            this.grdmMon.Location = new System.Drawing.Point(484, 116);
            this.grdmMon.MainView = this.gvMain;
            this.grdmMon.Name = "grdmMon";
            this.grdmMon.Size = new System.Drawing.Size(1056, 1088);
            this.grdmMon.TabIndex = 3;
            this.grdmMon.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvMain});
            // 
            // gvMain
            // 
            this.gvMain.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8});
            this.gvMain.GridControl = this.grdmMon;
            this.gvMain.Name = "gvMain";
            this.gvMain.OptionsBehavior.Editable = false;
            this.gvMain.OptionsSelection.MultiSelect = true;
            this.gvMain.OptionsView.EnableAppearanceEvenRow = true;
            this.gvMain.OptionsView.EnableAppearanceOddRow = true;
            this.gvMain.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Mã món ";
            this.gridColumn5.FieldName = "MaMon";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 0;
            this.gridColumn5.Width = 135;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Tên Món";
            this.gridColumn6.FieldName = "TenMon";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 1;
            this.gridColumn6.Width = 483;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Giá bán";
            this.gridColumn7.DisplayFormat.FormatString = "### ### ### ##0";
            this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn7.FieldName = "GiaBan";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 2;
            this.gridColumn7.Width = 156;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "DVT";
            this.gridColumn8.FieldName = "MaDVT";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 3;
            this.gridColumn8.Width = 124;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.simpleButton2);
            this.panelControl2.Controls.Add(this.simpleButton1);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControl2.Location = new System.Drawing.Point(1540, 116);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(94, 1088);
            this.panelControl2.TabIndex = 4;
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(6, 302);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 41);
            this.simpleButton2.TabIndex = 1;
            this.simpleButton2.Text = "<";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(6, 236);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 43);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = ">";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // grPOSGia
            // 
            this.grPOSGia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grPOSGia.Location = new System.Drawing.Point(1634, 116);
            this.grPOSGia.MainView = this.gvGia;
            this.grPOSGia.Name = "grPOSGia";
            this.grPOSGia.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCalcEdit1});
            this.grPOSGia.Size = new System.Drawing.Size(1221, 1088);
            this.grPOSGia.TabIndex = 5;
            this.grPOSGia.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvGia});
            // 
            // gvGia
            // 
            this.gvGia.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn9,
            this.gridColumn14,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn13});
            this.gvGia.GridControl = this.grPOSGia;
            this.gvGia.Name = "gvGia";
            this.gvGia.OptionsSelection.MultiSelect = true;
            this.gvGia.OptionsView.EnableAppearanceEvenRow = true;
            this.gvGia.OptionsView.EnableAppearanceOddRow = true;
            this.gvGia.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Mã Món";
            this.gridColumn9.FieldName = "MaMon";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.OptionsColumn.AllowEdit = false;
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 0;
            this.gridColumn9.Width = 177;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "Tên món";
            this.gridColumn14.FieldName = "TenMon";
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.OptionsColumn.AllowEdit = false;
            this.gridColumn14.Visible = true;
            this.gridColumn14.VisibleIndex = 1;
            this.gridColumn14.Width = 386;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "Giá bán";
            this.gridColumn10.DisplayFormat.FormatString = "### ### ##0";
            this.gridColumn10.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn10.FieldName = "GiaBan";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.OptionsColumn.AllowEdit = false;
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 2;
            this.gridColumn10.Width = 158;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "Tỉ lệ ck";
            this.gridColumn11.DisplayFormat.FormatString = "#0";
            this.gridColumn11.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn11.FieldName = "Tile";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.OptionsColumn.AllowEdit = false;
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 3;
            this.gridColumn11.Width = 158;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "Giá";
            this.gridColumn12.DisplayFormat.FormatString = "### ### ##0";
            this.gridColumn12.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn12.FieldName = "Gia";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.OptionsColumn.AllowEdit = false;
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 4;
            this.gridColumn12.Width = 158;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "Số lương min";
            this.gridColumn13.ColumnEdit = this.repositoryItemCalcEdit1;
            this.gridColumn13.DisplayFormat.FormatString = "##0";
            this.gridColumn13.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn13.FieldName = "SlMin";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 5;
            this.gridColumn13.Width = 166;
            // 
            // repositoryItemCalcEdit1
            // 
            this.repositoryItemCalcEdit1.AutoHeight = false;
            this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemCalcEdit1.DisplayFormat.FormatString = "##0.##";
            this.repositoryItemCalcEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repositoryItemCalcEdit1.EditFormat.FormatString = "##0.##";
            this.repositoryItemCalcEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repositoryItemCalcEdit1.Mask.EditMask = "##0.##";
            this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Mã";
            this.gridColumn1.FieldName = "MaPOSLoaiGia";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Tên";
            this.gridColumn2.FieldName = "TenPOSLoaiGia";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Ghi chú";
            this.gridColumn3.FieldName = "GhiChu";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Tỉ lệ";
            this.gridColumn4.FieldName = "Tile";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // fPOSKM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2855, 1204);
            this.Controls.Add(this.grPOSGia);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.grdmMon);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.tlLoaiMon);
            this.Controls.Add(this.panelControl1);
            this.Name = "fPOSKM";
            this.Text = "fPOSKM";
            this.Load += new System.EventHandler(this.fPOSKM_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.caPtCK.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grLoaiGia.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlLoaiMon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdmMon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grPOSGia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvGia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCalcEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton3;
        private DevExpress.XtraEditors.CalcEdit caPtCK;
        private DevExpress.XtraEditors.GridLookUpEdit grLoaiGia;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraTreeList.TreeList tlLoaiMon;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraGrid.GridControl grdmMon;
        private DevExpress.XtraGrid.Views.Grid.GridView gvMain;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraGrid.GridControl grPOSGia;
        private DevExpress.XtraGrid.Views.Grid.GridView gvGia;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn16;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn17;
        private DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit repositoryItemCalcEdit1;
    }
}