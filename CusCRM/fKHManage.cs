using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CDTDatabase;
using FormFactory;
using DataFactory;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using CBSControls;
using DevControls;
using CDTLib;
using DevExpress.XtraGrid.Views.Grid;

using DevExpress.XtraGrid.Columns;
using CDTControl;

namespace CusCRM
{
    public partial class fKHManage : Form
    {
        public fKHManage()
        {
            InitializeComponent();
        }
        DateTime Tungay = DateTime.Now;
        DateTime Denngay = DateTime.Now;
        
        Database db = Database.NewDataDatabase();
        DataSingle dmKH;
        DataSingle dtGDHistory;
        BindingSource bsDMKH = new BindingSource();
        string MaKH = "";
        GridControl gcHistory;
        BindingSource bsGDHistory = new BindingSource();
        FormDesigner designer_history;
        FormDesigner designer_MT35;
        DataMasterDetail MT35;
        private System.Windows.Forms.ImageList imageList_Order;
        FormDesigner designer_MT3A;
        DataMasterDetail MT3A;
        private System.Windows.Forms.ImageList imageList_Salse;
        private void fKHManage_Load(object sender, EventArgs e)
        {
            vDateEdit2.EditValue = DateTime.Parse(DateTime.Now.ToShortDateString());
            vDateEdit1.EditValue = DateTime.Parse(DateTime.Now.ToShortDateString()).AddYears(-1);
            ckCAuto.Checked = false;
            dmKH = new DataSingle("dmKH", "7");
            dmKH.GetData();
            bsDMKH.DataSource = dmKH.DsData.Tables[0];
            bsDMKH.CurrentItemChanged += BsDMKH_CurrentItemChanged;
            FormDesigner designer = new FormDesigner(dmKH);
            GridControl gc = designer.GenGridControl(dmKH.DsStruct.Tables[0], false, DockStyle.Fill);
            panelControl9.Controls.Add(gc);
            gc.DataSource = bsDMKH;
            
            TabControl.SelectedPageChanged += TabControl_SelectedPageChanged;
            
        }
        private void Init_History()
        {
            dtGDHistory = new DataSingle("dtGDHistory", "7");
            dtGDHistory.Condition = "1=0";
            dtGDHistory.GetData();
            designer_history = new FormDesigner(dtGDHistory);
            gcHistory = designer_history.GenGridControl(dtGDHistory.DsStruct.Tables[0], false, DockStyle.Fill);
            designer_history.bindingSource = bsGDHistory;
            foreach (CDTRepGridLookup glc in designer_history.rlist)
            {
                if (glc.Data != null) glc.Data.GetData();
            }
            tbHistory.Controls.Add(gcHistory);
            setRight_History();
        }
        GridControl gc_MT35;
        GridView gv_MT35;
        GridControl gc_DT35;
        GridView gv_DT35;
        GridControl gc_MT3A;
        GridView gv_MT3A;
        GridControl gc_DT3A;
        GridView gv_DT3A;
        private void Init_MT35()
        {
            MT35 = new DataMasterDetail("DT35", "7");
            designer_MT35 = new FormDesigner(MT35);
            if (Tungay == null || Denngay == null) return;
            if (MT35.DsData == null) GetData_MT35();
            gc_MT35 = designer_MT35.GenGridControl(MT35.DsStruct.Tables[0], false, DockStyle.Fill);
            gv_MT35 = gc_MT35.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
            gv_MT35.OptionsView.ShowDetailButtons = false;
            gv_MT35.OptionsBehavior.Editable = true;
            gv_MT35.OptionsBehavior.ReadOnly = true;
            //foreach (GridColumn gc in gv_MT35.Columns)
            //{
            //    if (gc.ColumnEdit != null && (gc.ColumnEdit as CDTRepGridLookup) != null)
            //    {
            //        foreach (EditorButton b in (gc.ColumnEdit as CDTRepGridLookup).Buttons)
            //            if (b.Kind == ButtonPredefines.Plus) b.Visible = false;
            //    }
            //}
            pOrder.Controls.Add(gc_MT35);
            SplitterControl spc = new SplitterControl();
            spc.Dock = DockStyle.Bottom;
            pOrder.Controls.Add(spc);
            gc_DT35 = designer_MT35.GenGridControl(MT35.DsStruct.Tables[1], false, DockStyle.Bottom);
            gv_DT35 = gc_DT35.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
            gv_DT35.OptionsBehavior.Editable = true;
            gv_DT35.OptionsBehavior.ReadOnly = true;
            gv_DT35.OptionsView.ShowFooter = false;
            gv_DT35.OptionsView.ShowGroupPanel = false;
            gv_DT35.OptionsView.ShowDetailButtons = false;
            //foreach (GridColumn gc in gvDegv_DT35tail.Columns)
            //{
            //    if (gc.ColumnEdit != null && (gc.ColumnEdit as CDTRepGridLookup) != null)
            //    {
            //        foreach (EditorButton b in (gc.ColumnEdit as CDTRepGridLookup).Buttons)
            //            if (b.Kind == ButtonPredefines.Plus) b.Visible = false;
            //    }
            //}
            pOrder.Controls.Add(gc_DT35);
            try
            {
                int i = MT35.DsData.Tables[0].Rows.IndexOf(MT35.DrCurrentMaster);
                i = gv_MT35.GetRowHandle(i);
                if (!gv_MT35.IsGroupRow(i))
                {
                    RowGroup_MT35 = gv_MT35.GetParentRowHandle(i);
                }
            }
            catch { }
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gv_MT35.Columns)
            {
                if (col.FieldName.ToUpper() == "TASKID")
                {
                    DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repTask = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
                    this.gc_MT35.RepositoryItems.Add(repTask);

                    repTask.AutoHeight = false;
                    repTask.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
                    if (MT35.tbTask != null)
                    {
                        for (int i = 0; i < MT35.tbTask.Rows.Count; i++)
                        {
                            DataRow drTask = MT35.tbTask.Rows[i];
                            if (drTask["Icon"] != DBNull.Value)
                            {
                                if (imageList_Order == null) imageList_Order = new ImageList();
                                this.imageList_Order.Images.Add(GetImage(drTask["Icon"] as byte[]));
                                repTask.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
                            new DevExpress.XtraEditors.Controls.ImageComboBoxItem(drTask["TaskLabel"].ToString(), drTask["ID"], repTask.Items.Count)});
                            }
                        }
                    }
                    repTask.SmallImages = this.imageList_Order;

                    col.ColumnEdit = repTask;
                    col.Caption = "";
                }
            }

        }
        private void Init_MT3A()
        {
            MT3A = new DataMasterDetail("DT3A", "7");
            designer_MT3A = new FormDesigner(MT3A);
            if (Tungay == null || Denngay == null) return;
            if (MT3A.DsData == null) GetData_MT3A();
            gc_MT3A = designer_MT3A.GenGridControl(MT3A.DsStruct.Tables[0], false, DockStyle.Fill);
            gv_MT3A = gc_MT3A.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
            gv_MT3A.OptionsView.ShowDetailButtons = false;
            gv_MT3A.OptionsBehavior.Editable = true;
            gv_MT3A.OptionsBehavior.ReadOnly = true;
            //foreach (GridColumn gc in gv_MT35.Columns)
            //{
            //    if (gc.ColumnEdit != null && (gc.ColumnEdit as CDTRepGridLookup) != null)
            //    {
            //        foreach (EditorButton b in (gc.ColumnEdit as CDTRepGridLookup).Buttons)
            //            if (b.Kind == ButtonPredefines.Plus) b.Visible = false;
            //    }
            //}
            pSales.Controls.Add(gc_MT3A);
            SplitterControl spc = new SplitterControl();
            spc.Dock = DockStyle.Bottom;
            pOrder.Controls.Add(spc);
            gc_DT3A = designer_MT3A.GenGridControl(MT3A.DsStruct.Tables[1], false, DockStyle.Bottom);
            gv_DT3A = gc_DT3A.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
            gv_DT3A.OptionsBehavior.Editable = true;
            gv_DT3A.OptionsBehavior.ReadOnly = true;
            gv_DT3A.OptionsView.ShowFooter = false;
            gv_DT3A.OptionsView.ShowGroupPanel = false;
            gv_DT3A.OptionsView.ShowDetailButtons = false;
            //foreach (GridColumn gc in gvDegv_DT35tail.Columns)
            //{
            //    if (gc.ColumnEdit != null && (gc.ColumnEdit as CDTRepGridLookup) != null)
            //    {
            //        foreach (EditorButton b in (gc.ColumnEdit as CDTRepGridLookup).Buttons)
            //            if (b.Kind == ButtonPredefines.Plus) b.Visible = false;
            //    }
            //}
            pSales.Controls.Add(gc_DT3A);
            try
            {
                int i = MT3A.DsData.Tables[0].Rows.IndexOf(MT3A.DrCurrentMaster);
                i = gv_MT3A.GetRowHandle(i);
                if (!gv_MT3A.IsGroupRow(i))
                {
                    RowGroup_MT3A = gv_MT3A.GetParentRowHandle(i);
                }
            }
            catch { }
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gv_MT3A.Columns)
            {
                if (col.FieldName.ToUpper() == "TASKID")
                {
                    DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repTask = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
                    this.gc_MT3A.RepositoryItems.Add(repTask);

                    repTask.AutoHeight = false;
                    repTask.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
                    if (MT3A.tbTask != null)
                    {
                        for (int i = 0; i < MT3A.tbTask.Rows.Count; i++)
                        {
                            DataRow drTask = MT3A.tbTask.Rows[i];
                            if (drTask["Icon"] != DBNull.Value)
                            {
                                if (imageList_Salse == null) imageList_Salse = new ImageList();
                                this.imageList_Salse.Images.Add(GetImage(drTask["Icon"] as byte[]));
                                repTask.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
                            new DevExpress.XtraEditors.Controls.ImageComboBoxItem(drTask["TaskLabel"].ToString(), drTask["ID"], repTask.Items.Count)});
                            }
                        }
                    }
                    repTask.SmallImages = this.imageList_Salse;

                    col.ColumnEdit = repTask;
                    col.Caption = "";
                }
            }

        }
        private Image GetImage(byte[] b)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(b);
            if (ms == null)
                return null;
            Image im = Image.FromStream(ms);
            return (im);
        }
        int RowGroup_MT35;
        int RowGroup_MT3A;
        private void GetData_MT35()
        {
            if (Tungay == null || Denngay == null) return;

                MT35.ConditionMaster = " (ngayct between '" + Tungay + "' and '" + Denngay + "') and MaKH='" + MaKH + "'";
                MT35.GetData();
        }
        private void GetData_MT3A()
        {
            if (Tungay == null || Denngay == null) return;

            MT3A.ConditionMaster = " (ngayct between '" + Tungay + "' and '" + Denngay + "') and MaKH='" + MaKH + "'";
            MT3A.GetData();
        }
        private void setRight_History()
        {
            if (Boolean.Parse(Config.GetValue("Admin").ToString()))
                return;
            string sSelect = dtGDHistory.DrTable["sSelect"].ToString();
            string sInsert = dtGDHistory.DrTable["sInsert"].ToString();
            string sUpdate = dtGDHistory.DrTable["sUpdate"].ToString();


            if (sInsert != string.Empty && !Boolean.Parse(sInsert))
                btNew.Enabled = false;
            if (sUpdate != string.Empty && !Boolean.Parse(sUpdate))
                btEdit.Enabled = false;
        }

        private void TabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page == tbHistory && ckCAuto.Checked)
            {
                btGetHistory_Click(sender, new EventArgs());
            }
            else if(e.Page==tbOrder && ckCAuto.Checked)
            {
                btGetOrder_Click(sender, new EventArgs());
            }
            else if (TabControl.SelectedTabPage == tbSale && ckCAuto.Checked)
            {
                btSales_Click(sender, new EventArgs());
            }
            else if (TabControl.SelectedTabPage == tbCongno && ckCAuto.Checked)
            {
                GetCongno_Click(sender, new EventArgs());
            }
        }

       

        private void BsDMKH_CurrentItemChanged(object sender, EventArgs e)
        {
            if (bsDMKH.Current != null)
                MaKH = (bsDMKH.Current as DataRowView)["MaKH"].ToString();

            if (ckCAuto.Checked)
            {
                if (TabControl.SelectedTabPage == tbSumary && ckCAuto.Checked)
                {
                    tbGetSumary_Click(sender, e);
                }

                else if (TabControl.SelectedTabPage == tbHistory && ckCAuto.Checked)
                {
                    btGetHistory_Click(sender, new EventArgs());
                }
                else if (TabControl.SelectedTabPage == tbOrder && ckCAuto.Checked)
                {
                    btGetOrder_Click(sender, new EventArgs());
                }
                else if (TabControl.SelectedTabPage == tbSale && ckCAuto.Checked)
                {
                    btSales_Click(sender, new EventArgs());
                }
                else if (TabControl.SelectedTabPage == tbCongno && ckCAuto.Checked)
                {
                    GetCongno_Click(sender, new EventArgs());
                }
            }
        }
        #region tbSumary


        private void tbGetSumary_Click(object sender, EventArgs e)
        {
            if (MaKH == null) return;
            
            DataTable tbSumary = db.GetDataSetByStore("CRM_getInfoKH", new string[] { "@MaKH" }, new object[] { MaKH });
            if (tbSumary == null || tbSumary.Rows.Count == 0) return;
            //set value for label
            lbTrangThaiKH.Text = tbSumary.Rows[0]["TrangThaiKH"].ToString();
            lbLastGD.Text = DateTime.Parse( tbSumary.Rows[0]["LastGD"].ToString()).ToString("dd/MM/yyyy");
            lbLastSale.Text = DateTime.Parse(tbSumary.Rows[0]["LastSale"].ToString()).ToString("dd/MM/yyyy");
            lbLastTT.Text = DateTime.Parse(tbSumary.Rows[0]["LastTT"].ToString()).ToString("dd/MM/yyyy");
            DataTable tbDSThang= db.GetDataSetByStore("CRM_getDSThang", new string[] { "@MaKH", "@NgayCT1", "@NgayCT2" }, new object[] { MaKH, Tungay, Denngay });
            ChartDSThang.DataSource = tbDSThang.DefaultView;
            ChartDSThang.Series[0].ArgumentDataMember = "thangnam";
            ChartDSThang.Series[0].ValueDataMembers.AddRange(new string[] { "DT" });

            DataTable tbDSNhomVT = db.GetDataSetByStore("CRM_GetData4DT_NHOMVT", new string[] { "@MaKH", "@NgayCT1", "@NgayCT2" }, new object[] { MaKH, Tungay, Denngay });
            ChartDSTungay.DataSource = tbDSNhomVT.DefaultView;
            ChartDSTungay.Series[0].ArgumentDataMember = "nhomVT";
            ChartDSTungay.Series[0].ValueDataMembers.AddRange(new string[] { "DT" });

        }
        #endregion
        private void vDateEdit1_EditValueChanged(object sender, EventArgs e)
        {
            Tungay = DateTime.Parse( vDateEdit1.EditValue.ToString());
        }

        private void vDateEdit2_EditValueChanged(object sender, EventArgs e)
        {
            Denngay = DateTime.Parse(vDateEdit2.EditValue.ToString());
        }

        private void tbGetData_Click(object sender, EventArgs e)
        {

        }

        private void btGetHistory_Click(object sender, EventArgs e)
        {
            if (MaKH != null)
            {
                if (gcHistory == null) Init_History();
                db.UpdateDatabyStore("CRM_DongboGiaoDich", new string[] { "@MaKH" }, new object[] { MaKH });
                dtGDHistory.Condition = "MaKH='" + MaKH + "'";
                dtGDHistory.GetData();
                bsGDHistory.DataSource = dtGDHistory.DsData.Tables[0];
                gcHistory.DataSource = bsGDHistory;

            }
        }
        FrmSingleDt FrmGDHistory;
        //Thêm - Sửa giao dịch
        private void btNew_Click(object sender, EventArgs e)
        {
            if (MaKH == null) return;
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            this.gcHistory.Refresh();
            //this.gvMain.EndDataUpdate();
            string s = (gcHistory.Views[0] as GridView).ActiveFilterString;
            if (dtGDHistory.DsData.Tables[0].PrimaryKey.Length > 0)
            {
                DataColumn colKey = dtGDHistory.DsData.Tables[0].PrimaryKey[0];
                dtGDHistory.DsData.Tables[0].PrimaryKey = null;
                colKey.AllowDBNull = true;
            }
           ( gcHistory.Views[0] as GridView).ClearColumnsFilter();
            designer_history.formAction = FormAction.New;
            bsGDHistory.AddNew();
            bsGDHistory.EndEdit();
            (bsGDHistory.Current as DataRowView).Row["MaKH"] = MaKH;
            if (FrmGDHistory == null)
                FrmGDHistory = new FrmSingleDt(designer_history);
            foreach(CDTGridLookUpEdit Gle in designer_history._glist)
            {
             if(Gle.Data.DrTable["TableName"].ToString() == "dmGDType")
                {
                    Gle.Data.Condition = "BangLK is null";
                    Gle.Data.GetData();
                }
            }
            FrmGDHistory.ShowDialog();
            (gcHistory.Views[0] as GridView).ActiveFilterString = s;
            (gcHistory.Views[0] as GridView).ApplyColumnsFilter();
            (gcHistory.Views[0] as GridView).BeginUpdate();
            gcHistory.DataSource = null;
            gcHistory.DataSource = bsGDHistory;
            (gcHistory.Views[0] as GridView).EndUpdate();
            (gcHistory.Views[0] as GridView).RefreshData();
            
            //this.bindingSource_CurrentChanged
            dtGDHistory.DataChanged = false;
            foreach (CDTGridLookUpEdit Gle in designer_history._glist)
            {
                if (Gle.Data.DrTable["TableName"].ToString() == "dmGDType")
                {
                    Gle.Data.Condition = "";
                    Gle.Data.GetData();
                }
            }
        }
        private void btEdit_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            string s = (gcHistory.Views[0] as GridView).ActiveFilterString;
            (gcHistory.Views[0] as GridView).ActiveFilterString = "";
            (gcHistory.Views[0] as GridView).ApplyColumnsFilter();
            foreach (CDTGridLookUpEdit Gle in designer_history._glist)
            {
                if (Gle.Data.DrTable["TableName"].ToString() == "dmGDType")
                {
                    Gle.Data.Condition = "BangLK is null";
                    Gle.Data.GetData();
                }
            }
            designer_history.formAction = FormAction.Edit;
            if (FrmGDHistory == null)
                FrmGDHistory = new FrmSingleDt(designer_history);
            FrmGDHistory.ShowDialog();
            (gcHistory.Views[0] as GridView).ActiveFilterString = s;
            (gcHistory.Views[0] as GridView).ApplyColumnsFilter();
            foreach (CDTGridLookUpEdit Gle in designer_history._glist)
            {
                if (Gle.Data.DrTable["TableName"].ToString() == "dmGDType")
                {
                    Gle.Data.Condition = "";
                    Gle.Data.GetData();
                }
            }
        }

        private void btGetOrder_Click(object sender, EventArgs e)
        {
            if (gc_MT35 == null) Init_MT35();
            GetData_MT35();
            DisplayData_MT35();
        }
        BindingSource bsMT35=new BindingSource();
        BindingSource bsMT3A = new BindingSource();
        private void DisplayData_MT35()
        {
            if (MT35.DsData == null)
                return;
            bsMT35.DataSource = MT35.DsData;

            bsMT35.CurrentChanged += new EventHandler(bsMT35_CurrentChanged);

            // bindingSource_CurrentChanged(_bindingSource, new EventArgs());
            this.bsMT35.DataMember = MT35.DsData.Tables[0].TableName;
            this.gc_MT35.DataSource = bsMT35;
            gc_DT35.DataSource = bsMT35;
            gc_DT35.DataMember = MT35.DrTable["TableName"].ToString();
            gv_MT35.BestFitColumns();
            if (!gv_MT35.IsGroupRow(RowGroup_MT35))
            {
                gv_MT35.ExpandGroupRow(-1);
            }
            else
            {
                gv_MT35.ExpandGroupRow(RowGroup_MT35);
            }
        }

        private void bsMT35_CurrentChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void btSales_Click(object sender, EventArgs e)
        {
            if (gc_MT3A == null) Init_MT3A();
            GetData_MT3A();
            DisplayData_MT3A();
        }
        private void DisplayData_MT3A()
        {
            if (MT3A.DsData == null)
                return;
            bsMT3A.DataSource = MT3A.DsData;

            bsMT3A.CurrentChanged += new EventHandler(bsMT3A_CurrentChanged);

            // bindingSource_CurrentChanged(_bindingSource, new EventArgs());
            this.bsMT3A.DataMember = MT3A.DsData.Tables[0].TableName;
            this.gc_MT3A.DataSource = bsMT3A;
            gc_DT3A.DataSource = bsMT3A;
            gc_DT3A.DataMember = MT3A.DrTable["TableName"].ToString();
            gv_MT3A.BestFitColumns();
            if (!gv_MT3A.IsGroupRow(RowGroup_MT3A))
            {
                gv_MT3A.ExpandGroupRow(-1);
            }
            else
            {
                gv_MT3A.ExpandGroupRow(RowGroup_MT3A);
            }
        }

        private void bsMT3A_CurrentChanged(object sender, EventArgs e)
        {
           // throw new NotImplementedException();
        }
        BindingSource bsCongno = new BindingSource();
        private void GetCongno_Click(object sender, EventArgs e)
        {
            try
            {
                if (MaKH == null) return;
                CDTData _data = DataFactory.DataFactory.Create(DataType.Report, "1453");
                _data.GetData();
                bsCongno.DataSource = _data.DsData.Tables[0];
                bsCongno.AddNew();
                bsCongno.EndEdit();
                (bsCongno.Current as DataRowView)["NgayCT1"] = Tungay;
                (bsCongno.Current as DataRowView)["NgayCT2"] = Denngay;
                (bsCongno.Current as DataRowView)["TK"] = "131";
                (bsCongno.Current as DataRowView)["MaKH"] = MaKH;
                DataReport __data = new DataReport((_data as DataReport).DrTable);
                __data.GetData();
                __data.DsData = _data.DsData;
                __data.reConfig = new ReConfig();
                __data.reConfig.Variables = (_data as DataReport).reConfig.Copy();
                __data.DrCurrentMaster = (bsCongno.Current as DataRowView).Row;
                (__data as DataReport).SaveVariables();
                (__data as DataReport).GenFilterString();
                FormFactory.ReportPreview rptPre = new FormFactory.ReportPreview(__data);
                rptPre.Dock = DockStyle.Fill;
                pCongno.Controls.Clear();// rptPre.Visible = true;
                pCongno.Controls.Add(rptPre);
            }
            catch (Exception ex)
            {

            }
            
        }
        //Phần report 1453
        //1. Tạo data
        //    CDTData data = DataFactory.DataFactory.Create(DataType.Report, sysReportID);
        //2. data.getData()
        //    3.DataReport __data = new DataReport((_data as DataReport).DrTable);
        //__data.GetData();
        //    __data.DsData = _data.DsData;
        //    __data.reConfig = new ReConfig();
        //__data.reConfig.Variables=(_data as DataReport).reConfig.Copy();
        //    if (drv != null)
        //        __data.DrCurrentMaster = drv.Row;
        //ReportPreview rptPre = new ReportPreview(__data);

    }
}
