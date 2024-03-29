using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DataFactory;
using CDTControl;
using DevExpress.XtraGrid;

namespace FormFactory
{
    public partial class FrmPQ : DevExpress.XtraEditors.XtraForm
    {
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private SimpleButton simpleButton3;
        private SimpleButton simpleButton2;
        private SimpleButton btChkAll;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem Status;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private SplitterControl splitterControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.Columns.GridColumn gColUserName;
        private DevExpress.XtraGrid.Columns.GridColumn gColFullName;
        private DataTable UserList;
        private BindingSource dbsUser;
        private BindingSource dbsDM;
        CDTData _Data;
        GridControl grDM;
        DevExpress.XtraGrid.Views.Grid.GridView gvDM;
        private string UserGrID = string.Empty;
        public FrmPQ(FormDesigner f,CDTData _data)
        {
            InitializeComponent();
            this.KeyUp += new KeyEventHandler(KeyPess);
            _Data = _data;
            UserList = (new SysUser()).GetUserGroupList();
            dbsUser = new BindingSource();

            dbsDM = new BindingSource();
            dbsUser.DataSource = UserList;
            dbsUser.PositionChanged +=new EventHandler(dbsUser_PositionChanged);
            
            gridControl1.DataSource = UserList;
            
            
            grDM= f.GenGridControl(_data.DsStruct.Tables[0],false, DockStyle.Fill);
            gvDM = grDM.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
            gvDM.OptionsBehavior.Editable = true;
            gvDM.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            this.Controls.Add(grDM);
            
            grDM.BringToFront();
            grDM.DataSource = dbsDM;
            
            //Thêm cột nữa
                     
            
            addChkCol();
            Status.Text = "";
            dbsUser_PositionChanged(dbsUser, new EventArgs());
        }
        void addChkCol()
        {
            ((ISupportInitialize)grDM).BeginInit();
            ((ISupportInitialize)gvDM).BeginInit();
            if (gvDM.Columns["_Chk"] != null)
                gvDM.Columns.Remove(gvDM.Columns.ColumnByFieldName("_Chk"));
            DataColumn chkDC = new DataColumn("_Chk", typeof(Boolean));
            chkDC.DefaultValue = false;
            _Data.DsData.Tables[0].Columns.Add(chkDC);
            DevExpress.XtraGrid.Columns.GridColumn chkGC = new DevExpress.XtraGrid.Columns.GridColumn();
            chkGC.ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            chkGC.Caption = " Quyền";
            chkGC.FieldName = "_Chk";
            chkGC.VisibleIndex = 0;

            gvDM.Columns.Add(chkGC);
            DataView vDM = new DataView(_Data.DsData.Tables[0], (_Data as DataFactory.DataSingle).extraWS, "", DataViewRowState.CurrentRows);
            dbsDM.DataSource = vDM;
            ((ISupportInitialize)grDM).EndInit();
            ((ISupportInitialize)gvDM).EndInit();
        }
        void  dbsUser_PositionChanged(object sender, EventArgs e)
        {

            if (UserGrID != string.Empty)
            {
                (_Data as DataSingle).WSGr = UserGrID;
                //if (MessageBox.Show("Bạn có muốn update dữ liệu phân quyền không?", "??!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //{
                    updateWS();
                //}
            }
            UserGrID = (dbsUser.Current as System.Data.DataRowView)[0].ToString();
            (_Data as DataSingle).WSGr = UserGrID;
            (_Data as DataSingle).GetData();
            addChkCol();
            ChkIfAllow();

        }
        private void updateWS()
        {
            (_Data as DataSingle).updateWS();
        }
        private void ChkIfAllow()
        {
            try
            {
                DataRow[] drChked = _Data.DsData.Tables[0].Select("Grws like '%_" + UserGrID + "_%'");
                if (_Data.DsData.Tables[0].PrimaryKey.Length == 0)
                    _Data.DsData.Tables[0].PrimaryKey = new DataColumn[] { _Data.DsData.Tables[0].Columns[_Data.PkMaster.FieldName] };
                foreach (DataRow dr in drChked)
                {
                    dr["_Chk"] = 1;
                    dr.AcceptChanges();
                }
            }
            catch (Exception e)
            { }
            grDM.Refresh();

        }
       

        private void KeyPess(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (UserGrID == string.Empty)
                        return;
                    break;
                case Keys.Escape:                    
                    this.Close();
                    break;
            }
        }
        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.btChkAll = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.Status = new DevExpress.XtraLayout.EmptySpaceItem();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColUserName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColFullName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Status)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.simpleButton3);
            this.layoutControl1.Controls.Add(this.simpleButton2);
            this.layoutControl1.Controls.Add(this.btChkAll);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.layoutControl1.Location = new System.Drawing.Point(0, 815);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1573, 39);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // simpleButton3
            // 
            this.simpleButton3.Location = new System.Drawing.Point(1391, 6);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Size = new System.Drawing.Size(176, 22);
            this.simpleButton3.StyleController = this.layoutControl1;
            this.simpleButton3.TabIndex = 6;
            this.simpleButton3.Text = "Cập nhật";
            this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(1115, 6);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(266, 22);
            this.simpleButton2.StyleController = this.layoutControl1;
            this.simpleButton2.TabIndex = 5;
            this.simpleButton2.Text = "Bỏ chọn tất cả";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // btChkAll
            // 
            this.btChkAll.Location = new System.Drawing.Point(888, 6);
            this.btChkAll.Name = "btChkAll";
            this.btChkAll.Size = new System.Drawing.Size(217, 22);
            this.btChkAll.StyleController = this.layoutControl1;
            this.btChkAll.TabIndex = 4;
            this.btChkAll.Text = "Chọn tất cả";
            this.btChkAll.Click += new System.EventHandler(this.btChkAll_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.Status});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1573, 39);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btChkAll;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(882, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem1.Size = new System.Drawing.Size(227, 37);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.simpleButton2;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(1109, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem2.Size = new System.Drawing.Size(276, 37);
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.simpleButton3;
            this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            this.layoutControlItem3.Location = new System.Drawing.Point(1385, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem3.Size = new System.Drawing.Size(186, 37);
            this.layoutControlItem3.Text = "layoutControlItem3";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // Status
            // 
            this.Status.AllowHotTrack = false;
            this.Status.CustomizationFormText = "emptySpaceItem1";
            this.Status.Location = new System.Drawing.Point(0, 0);
            this.Status.Name = "Status";
            this.Status.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.Status.Size = new System.Drawing.Size(882, 37);
            this.Status.Text = "Status";
            this.Status.TextSize = new System.Drawing.Size(0, 0);
            this.Status.TextVisible = true;
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(370, 815);
            this.gridControl1.TabIndex = 1;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColUserName,
            this.gColFullName});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsView.EnableAppearanceEvenRow = true;
            this.gridView1.OptionsView.EnableAppearanceOddRow = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView1_FocusedRowChanged);
            // 
            // gColUserName
            // 
            this.gColUserName.Caption = "Group Name";
            this.gColUserName.FieldName = "GroupName";
            this.gColUserName.Name = "gColUserName";
            this.gColUserName.Visible = true;
            this.gColUserName.VisibleIndex = 0;
            this.gColUserName.Width = 109;
            // 
            // gColFullName
            // 
            this.gColFullName.Caption = "Group Full Name";
            this.gColFullName.FieldName = "DienGiai";
            this.gColFullName.Name = "gColFullName";
            this.gColFullName.Visible = true;
            this.gColFullName.VisibleIndex = 1;
            this.gColFullName.Width = 240;
            // 
            // splitterControl1
            // 
            this.splitterControl1.Location = new System.Drawing.Point(370, 0);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(5, 815);
            this.splitterControl1.TabIndex = 2;
            this.splitterControl1.TabStop = false;
            // 
            // FrmPQ
            // 
            this.ClientSize = new System.Drawing.Size(1573, 854);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.layoutControl1);
            this.Name = "FrmPQ";
            this.Text = "Phân quyền";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPQ_FormClosing);
            this.Load += new System.EventHandler(this.FrmPQ_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Status)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        void FrmPQ_FormClosing(object sender, FormClosingEventArgs e)
        {
            _Data.DsData.Tables[0].Columns.Remove("_Chk");
            (this._Data as DataSingle).WS = string.Empty;
        }

        void  gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
 	        //throw new Exception("The method or operation is not implemented.");
            if (e.FocusedRowHandle>=0)
            {
                dbsUser.Position=e.FocusedRowHandle;
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            updateWS();
        }

        private void btChkAll_Click(object sender, EventArgs e)
        {
            foreach (DataRowView dv in (dbsDM.DataSource as DataView))
            {
                dv["_Chk"] = 1;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            foreach (DataRowView dv in (dbsDM.DataSource as DataView))
            {
                dv["_Chk"] = 0;
            }
        }

        private void FrmPQ_Load(object sender, EventArgs e)
        {

        }





    }
}