namespace CDTSystem
{
    partial class CheckData
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
            this.lbcTable = new DevExpress.XtraEditors.ListBoxControl();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.simpleButtonTrace = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonPreview = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonSearch = new DevExpress.XtraEditors.SimpleButton();
            this.checkEditTreeView = new DevExpress.XtraEditors.CheckEdit();
            this.simpleButtonUpdate = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciUpdate = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemTreeList = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemSearch = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciViewHistory = new DevExpress.XtraLayout.LayoutControlItem();
            this.lookUpEditCNLQ = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlItemCNLQ = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbcTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditTreeView.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUpdate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemTreeList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciViewHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lookUpEditCNLQ.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCNLQ)).BeginInit();
            this.SuspendLayout();
            // 
            // lbcTable
            // 
            this.lbcTable.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbcTable.Location = new System.Drawing.Point(0, 0);
            this.lbcTable.Name = "lbcTable";
            this.lbcTable.Size = new System.Drawing.Size(251, 566);
            this.lbcTable.TabIndex = 0;
            this.lbcTable.DoubleClick += new System.EventHandler(this.lbcTenForm_DoubleClick);
            this.lbcTable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbcTenForm_KeyDown);
            // 
            // splitterControl1
            // 
            this.splitterControl1.Location = new System.Drawing.Point(251, 0);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(6, 566);
            this.splitterControl1.TabIndex = 4;
            this.splitterControl1.TabStop = false;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lookUpEditCNLQ);
            this.layoutControl1.Controls.Add(this.simpleButtonTrace);
            this.layoutControl1.Controls.Add(this.simpleButtonPreview);
            this.layoutControl1.Controls.Add(this.simpleButtonSearch);
            this.layoutControl1.Controls.Add(this.checkEditTreeView);
            this.layoutControl1.Controls.Add(this.simpleButtonUpdate);
            this.layoutControl1.Controls.Add(this.simpleButtonCancel);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.layoutControl1.Location = new System.Drawing.Point(257, 529);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(646, 37);
            this.layoutControl1.TabIndex = 5;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // simpleButtonTrace
            // 
            this.simpleButtonTrace.Location = new System.Drawing.Point(279, 7);
            this.simpleButtonTrace.Name = "simpleButtonTrace";
            this.simpleButtonTrace.Size = new System.Drawing.Size(127, 22);
            this.simpleButtonTrace.StyleController = this.layoutControl1;
            this.simpleButtonTrace.TabIndex = 11;
            this.simpleButtonTrace.Text = "Xem lưu vết người dùng";
            this.simpleButtonTrace.Click += new System.EventHandler(this.simpleButtonTrace_Click);
            // 
            // simpleButtonPreview
            // 
            this.simpleButtonPreview.Location = new System.Drawing.Point(549, 7);
            this.simpleButtonPreview.Name = "simpleButtonPreview";
            this.simpleButtonPreview.Size = new System.Drawing.Size(39, 22);
            this.simpleButtonPreview.StyleController = this.layoutControl1;
            this.simpleButtonPreview.TabIndex = 10;
            this.simpleButtonPreview.Text = "F7-In";
            this.simpleButtonPreview.Click += new System.EventHandler(this.simpleButtonPreview_Click);
            // 
            // simpleButtonSearch
            // 
            this.simpleButtonSearch.Location = new System.Drawing.Point(485, 7);
            this.simpleButtonSearch.Name = "simpleButtonSearch";
            this.simpleButtonSearch.Size = new System.Drawing.Size(53, 22);
            this.simpleButtonSearch.StyleController = this.layoutControl1;
            this.simpleButtonSearch.TabIndex = 7;
            this.simpleButtonSearch.Text = "Tìm kiếm";
            this.simpleButtonSearch.Click += new System.EventHandler(this.simpleButtonSearch_Click);
            // 
            // checkEditTreeView
            // 
            this.checkEditTreeView.Location = new System.Drawing.Point(7, 7);
            this.checkEditTreeView.Name = "checkEditTreeView";
            this.checkEditTreeView.Properties.Caption = "Cấu trúc cây";
            this.checkEditTreeView.Size = new System.Drawing.Size(84, 19);
            this.checkEditTreeView.StyleController = this.layoutControl1;
            this.checkEditTreeView.TabIndex = 6;
            this.checkEditTreeView.CheckedChanged += new System.EventHandler(this.checkEditTreeView_CheckedChanged);
            // 
            // simpleButtonUpdate
            // 
            this.simpleButtonUpdate.Location = new System.Drawing.Point(417, 7);
            this.simpleButtonUpdate.Name = "simpleButtonUpdate";
            this.simpleButtonUpdate.Size = new System.Drawing.Size(57, 22);
            this.simpleButtonUpdate.StyleController = this.layoutControl1;
            this.simpleButtonUpdate.TabIndex = 4;
            this.simpleButtonUpdate.Text = "Cập nhật";
            this.simpleButtonUpdate.Click += new System.EventHandler(this.simpleButtonUpdate_Click);
            // 
            // simpleButtonCancel
            // 
            this.simpleButtonCancel.Location = new System.Drawing.Point(599, 7);
            this.simpleButtonCancel.Name = "simpleButtonCancel";
            this.simpleButtonCancel.Size = new System.Drawing.Size(41, 22);
            this.simpleButtonCancel.StyleController = this.layoutControl1;
            this.simpleButtonCancel.TabIndex = 5;
            this.simpleButtonCancel.Text = "Thoát";
            this.simpleButtonCancel.Click += new System.EventHandler(this.simpleButtonCancel_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.DefaultLayoutType = DevExpress.XtraLayout.Utils.LayoutType.Horizontal;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciUpdate,
            this.layoutControlItem2,
            this.layoutControlItemTreeList,
            this.layoutControlItemSearch,
            this.layoutControlItem8,
            this.lciViewHistory,
            this.layoutControlItemCNLQ});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(646, 37);
            this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciUpdate
            // 
            this.lciUpdate.Control = this.simpleButtonUpdate;
            this.lciUpdate.CustomizationFormText = "layoutControlItem1";
            this.lciUpdate.Location = new System.Drawing.Point(410, 0);
            this.lciUpdate.Name = "lciUpdate";
            this.lciUpdate.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.lciUpdate.Size = new System.Drawing.Size(68, 35);
            this.lciUpdate.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciUpdate.Text = "lciUpdate";
            this.lciUpdate.TextSize = new System.Drawing.Size(0, 0);
            this.lciUpdate.TextToControlDistance = 0;
            this.lciUpdate.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.simpleButtonCancel;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(592, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem2.Size = new System.Drawing.Size(52, 35);
            this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItemTreeList
            // 
            this.layoutControlItemTreeList.Control = this.checkEditTreeView;
            this.layoutControlItemTreeList.CustomizationFormText = "layoutControlItem8";
            this.layoutControlItemTreeList.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemTreeList.Name = "layoutControlItemTreeList";
            this.layoutControlItemTreeList.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItemTreeList.Size = new System.Drawing.Size(95, 35);
            this.layoutControlItemTreeList.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItemTreeList.Text = "layoutControlItemTreeList";
            this.layoutControlItemTreeList.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemTreeList.TextToControlDistance = 0;
            this.layoutControlItemTreeList.TextVisible = false;
            // 
            // layoutControlItemSearch
            // 
            this.layoutControlItemSearch.Control = this.simpleButtonSearch;
            this.layoutControlItemSearch.CustomizationFormText = "layoutControlItem7";
            this.layoutControlItemSearch.Location = new System.Drawing.Point(478, 0);
            this.layoutControlItemSearch.Name = "layoutControlItemSearch";
            this.layoutControlItemSearch.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItemSearch.Size = new System.Drawing.Size(64, 35);
            this.layoutControlItemSearch.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItemSearch.Text = "layoutControlItemSearch";
            this.layoutControlItemSearch.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemSearch.TextToControlDistance = 0;
            this.layoutControlItemSearch.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.simpleButtonPreview;
            this.layoutControlItem8.CustomizationFormText = "layoutControlItem8";
            this.layoutControlItem8.Location = new System.Drawing.Point(542, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem8.Size = new System.Drawing.Size(50, 35);
            this.layoutControlItem8.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem8.Text = "layoutControlItem8";
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextToControlDistance = 0;
            this.layoutControlItem8.TextVisible = false;
            // 
            // lciViewHistory
            // 
            this.lciViewHistory.Control = this.simpleButtonTrace;
            this.lciViewHistory.CustomizationFormText = "layoutControlItem3";
            this.lciViewHistory.Location = new System.Drawing.Point(272, 0);
            this.lciViewHistory.Name = "lciViewHistory";
            this.lciViewHistory.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.lciViewHistory.Size = new System.Drawing.Size(138, 35);
            this.lciViewHistory.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciViewHistory.Text = "lciViewHistory";
            this.lciViewHistory.TextSize = new System.Drawing.Size(0, 0);
            this.lciViewHistory.TextToControlDistance = 0;
            this.lciViewHistory.TextVisible = false;
            // 
            // lookUpEditCNLQ
            // 
            this.lookUpEditCNLQ.Location = new System.Drawing.Point(102, 7);
            this.lookUpEditCNLQ.Name = "lookUpEditCNLQ";
            this.lookUpEditCNLQ.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.OK, "Xem", -1, true, true, false, DevExpress.Utils.HorzAlignment.Center, null)});
            this.lookUpEditCNLQ.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DienGiai", "DienGiai", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None)});
            this.lookUpEditCNLQ.Properties.NullText = "Xem thông tin liên quan";
            this.lookUpEditCNLQ.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lookUpEditCNLQ_Properties_ButtonClick);
            this.lookUpEditCNLQ.Size = new System.Drawing.Size(166, 20);
            this.lookUpEditCNLQ.StyleController = this.layoutControl1;
            this.lookUpEditCNLQ.TabIndex = 10;
            // 
            // layoutControlItemCNLQ
            // 
            this.layoutControlItemCNLQ.Control = this.lookUpEditCNLQ;
            this.layoutControlItemCNLQ.CustomizationFormText = "layoutControlItemCNLQ";
            this.layoutControlItemCNLQ.Location = new System.Drawing.Point(95, 0);
            this.layoutControlItemCNLQ.Name = "layoutControlItemCNLQ";
            this.layoutControlItemCNLQ.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItemCNLQ.Size = new System.Drawing.Size(177, 35);
            this.layoutControlItemCNLQ.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItemCNLQ.Text = "layoutControlItemCNLQ";
            this.layoutControlItemCNLQ.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemCNLQ.TextToControlDistance = 0;
            this.layoutControlItemCNLQ.TextVisible = false;
            // 
            // CheckData
            // 
            this.ClientSize = new System.Drawing.Size(903, 566);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.lbcTable);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckData";
            this.Text = "Kiểm tra số liệu thô";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCheckData_KeyDown);
            this.Load += new System.EventHandler(this.FrmTest_Load);
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbcTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkEditTreeView.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUpdate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemTreeList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciViewHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lookUpEditCNLQ.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCNLQ)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ListBoxControl lbcTable;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButtonPreview;
        private DevExpress.XtraEditors.SimpleButton simpleButtonSearch;
        private DevExpress.XtraEditors.CheckEdit checkEditTreeView;
        private DevExpress.XtraEditors.SimpleButton simpleButtonUpdate;
        private DevExpress.XtraEditors.SimpleButton simpleButtonCancel;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciUpdate;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemTreeList;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemSearch;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraEditors.SimpleButton simpleButtonTrace;
        private DevExpress.XtraLayout.LayoutControlItem lciViewHistory;
        private DevExpress.XtraEditors.LookUpEdit lookUpEditCNLQ;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemCNLQ;
    }
}