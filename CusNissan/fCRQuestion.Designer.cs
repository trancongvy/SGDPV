namespace CusNissan
{
    partial class fCRQuestion
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
            this.components = new System.ComponentModel.Container();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btAccept = new DevExpress.XtraEditors.SimpleButton();
            this.dNgayCt = new DevExpress.XtraEditors.DateEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.tbUpdate = new DevExpress.XtraEditors.SimpleButton();
            this.grMain = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.grDetail = new DevExpress.XtraGrid.GridControl();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn16 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.gridColumn17 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn18 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dNgayCt.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dNgayCt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCalcEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btAccept);
            this.panelControl1.Controls.Add(this.dNgayCt);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(2724, 102);
            this.panelControl1.TabIndex = 0;
            this.panelControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.panelControl1_Paint);
            // 
            // btAccept
            // 
            this.btAccept.Location = new System.Drawing.Point(450, 19);
            this.btAccept.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btAccept.Name = "btAccept";
            this.btAccept.Size = new System.Drawing.Size(150, 44);
            this.btAccept.TabIndex = 2;
            this.btAccept.Text = "Chấp nhận";
            this.btAccept.Click += new System.EventHandler(this.btAccept_Click);
            // 
            // dNgayCt
            // 
            this.dNgayCt.EditValue = null;
            this.dNgayCt.Location = new System.Drawing.Point(170, 25);
            this.dNgayCt.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dNgayCt.Name = "dNgayCt";
            this.dNgayCt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dNgayCt.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dNgayCt.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dNgayCt.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dNgayCt.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dNgayCt.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dNgayCt.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dNgayCt.Size = new System.Drawing.Size(200, 32);
            this.dNgayCt.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(80, 40);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(54, 25);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Ngày:";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.tbUpdate);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(0, 840);
            this.panelControl2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(2724, 81);
            this.panelControl2.TabIndex = 1;
            // 
            // tbUpdate
            // 
            this.tbUpdate.Location = new System.Drawing.Point(2306, 13);
            this.tbUpdate.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tbUpdate.Name = "tbUpdate";
            this.tbUpdate.Size = new System.Drawing.Size(150, 44);
            this.tbUpdate.TabIndex = 0;
            this.tbUpdate.Text = "Cập nhật";
            this.tbUpdate.Click += new System.EventHandler(this.tbUpdate_Click);
            // 
            // grMain
            // 
            this.grMain.Dock = System.Windows.Forms.DockStyle.Left;
            this.grMain.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.grMain.Location = new System.Drawing.Point(0, 102);
            this.grMain.MainView = this.gridView1;
            this.grMain.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.grMain.Name = "grMain";
            this.grMain.Size = new System.Drawing.Size(922, 738);
            this.grMain.TabIndex = 2;
            this.grMain.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn14,
            this.gridColumn15,
            this.gridColumn10});
            this.gridView1.GridControl = this.grMain;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsCustomization.AllowRowSizing = true;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.EnableAppearanceEvenRow = true;
            this.gridView1.OptionsView.EnableAppearanceOddRow = true;
            this.gridView1.OptionsView.ShowDetailButtons = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.RowHeight = 80;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Tên khách hàng";
            this.gridColumn1.FieldName = "tenkh";
            this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 171;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Tài xế";
            this.gridColumn2.FieldName = "taixe";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 96;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Số điện thoại";
            this.gridColumn3.FieldName = "sodt";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Địa chỉ";
            this.gridColumn4.FieldName = "diachi";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 170;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Số vin";
            this.gridColumn5.FieldName = "maxe";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            this.gridColumn5.Width = 126;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Số xe";
            this.gridColumn6.FieldName = "soxe";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            this.gridColumn6.Width = 65;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Loại xe";
            this.gridColumn7.FieldName = "loaixe";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            this.gridColumn7.Width = 61;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Model";
            this.gridColumn8.FieldName = "model";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 7;
            this.gridColumn8.Width = 53;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Số km";
            this.gridColumn9.DisplayFormat.FormatString = "### ###";
            this.gridColumn9.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn9.FieldName = "sokm";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 8;
            this.gridColumn9.Width = 47;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "Loại hình DV";
            this.gridColumn14.FieldName = "malhdv";
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Visible = true;
            this.gridColumn14.VisibleIndex = 9;
            // 
            // gridColumn15
            // 
            this.gridColumn15.Caption = "Nội dung";
            this.gridColumn15.FieldName = "noidung";
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.Visible = true;
            this.gridColumn15.VisibleIndex = 10;
            this.gridColumn15.Width = 166;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "Tiền dịch vụ";
            this.gridColumn10.DisplayFormat.FormatString = "### ### ### ###";
            this.gridColumn10.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn10.FieldName = "ttienh";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 11;
            // 
            // splitterControl1
            // 
            this.splitterControl1.Location = new System.Drawing.Point(922, 102);
            this.splitterControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(5, 738);
            this.splitterControl1.TabIndex = 3;
            this.splitterControl1.TabStop = false;
            // 
            // grDetail
            // 
            this.grDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grDetail.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.grDetail.Location = new System.Drawing.Point(927, 102);
            this.grDetail.MainView = this.gridView2;
            this.grDetail.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.grDetail.Name = "grDetail";
            this.grDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCalcEdit1,
            this.repositoryItemDateEdit1});
            this.grDetail.Size = new System.Drawing.Size(1797, 738);
            this.grDetail.TabIndex = 4;
            this.grDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView2});
            // 
            // gridView2
            // 
            this.gridView2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn11,
            this.gridColumn16,
            this.gridColumn17,
            this.gridColumn18,
            this.gridColumn12,
            this.gridColumn13});
            this.gridView2.GridControl = this.grDetail;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsView.ColumnAutoWidth = false;
            this.gridView2.OptionsView.EnableAppearanceEvenRow = true;
            this.gridView2.OptionsView.EnableAppearanceOddRow = true;
            this.gridView2.OptionsView.ShowFooter = true;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "Nội dung câu hỏi";
            this.gridColumn11.FieldName = "noidung";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.OptionsColumn.AllowEdit = false;
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 0;
            this.gridColumn11.Width = 268;
            // 
            // gridColumn16
            // 
            this.gridColumn16.Caption = "Gọi điện lần 1";
            this.gridColumn16.ColumnEdit = this.repositoryItemDateEdit1;
            this.gridColumn16.DisplayFormat.FormatString = "dd/MM/yy HH:mm";
            this.gridColumn16.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn16.FieldName = "Ngaygd1";
            this.gridColumn16.Name = "gridColumn16";
            this.gridColumn16.Visible = true;
            this.gridColumn16.VisibleIndex = 1;
            this.gridColumn16.Width = 173;
            // 
            // repositoryItemDateEdit1
            // 
            this.repositoryItemDateEdit1.AutoHeight = false;
            this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDateEdit1.DisplayFormat.FormatString = "dd/MM/yy HH:mm";
            this.repositoryItemDateEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.repositoryItemDateEdit1.EditFormat.FormatString = "dd/MM/yy HH:mm";
            this.repositoryItemDateEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.repositoryItemDateEdit1.Mask.EditMask = "dd/MM/yy HH:mm";
            this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
            this.repositoryItemDateEdit1.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            // 
            // gridColumn17
            // 
            this.gridColumn17.Caption = "Gọi điện lần 2";
            this.gridColumn17.ColumnEdit = this.repositoryItemDateEdit1;
            this.gridColumn17.DisplayFormat.FormatString = "dd/MM/yy HH:mm";
            this.gridColumn17.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn17.FieldName = "Ngaygd2";
            this.gridColumn17.Name = "gridColumn17";
            this.gridColumn17.Visible = true;
            this.gridColumn17.VisibleIndex = 2;
            this.gridColumn17.Width = 162;
            // 
            // gridColumn18
            // 
            this.gridColumn18.Caption = "Gọi điện lần 3";
            this.gridColumn18.ColumnEdit = this.repositoryItemDateEdit1;
            this.gridColumn18.DisplayFormat.FormatString = "dd/MM/yy HH:mm";
            this.gridColumn18.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn18.FieldName = "Ngaygd3";
            this.gridColumn18.Name = "gridColumn18";
            this.gridColumn18.Visible = true;
            this.gridColumn18.VisibleIndex = 3;
            this.gridColumn18.Width = 273;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "Điểm";
            this.gridColumn12.ColumnEdit = this.repositoryItemCalcEdit1;
            this.gridColumn12.DisplayFormat.FormatString = "#0";
            this.gridColumn12.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn12.FieldName = "Diem";
            this.gridColumn12.GroupFormat.FormatString = "##";
            this.gridColumn12.GroupFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Diem", "", 0)});
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 4;
            this.gridColumn12.Width = 69;
            // 
            // repositoryItemCalcEdit1
            // 
            this.repositoryItemCalcEdit1.AutoHeight = false;
            this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemCalcEdit1.DisplayFormat.FormatString = "#0";
            this.repositoryItemCalcEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repositoryItemCalcEdit1.EditFormat.FormatString = "#0";
            this.repositoryItemCalcEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repositoryItemCalcEdit1.Mask.EditMask = "#0";
            this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "Nhận xét";
            this.gridColumn13.FieldName = "NhanXet";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 5;
            this.gridColumn13.Width = 275;
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // fCRQuestion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2724, 921);
            this.Controls.Add(this.grDetail);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.grMain);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "fCRQuestion";
            this.Text = "fCRQuestion";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dNgayCt.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dNgayCt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCalcEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.DateEdit dNgayCt;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraGrid.GridControl grMain;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraGrid.GridControl grDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraEditors.SimpleButton btAccept;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraEditors.SimpleButton tbUpdate;
        private DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit repositoryItemCalcEdit1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn16;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repositoryItemDateEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn17;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn18;
    }
}