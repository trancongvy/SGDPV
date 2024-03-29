using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using CDTLib;
using CDTDatabase;
using System.Data.SqlClient;
using DataFactory;
using FormFactory;
using DevControls;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace CusAccounting
{
    public partial class fBanhang : DevExpress.XtraEditors.XtraForm
    {
        Database db = Database.NewDataDatabase();
        DataTable dt = new DataTable();
        DataTable dmvt;
        DataTable dmdvt;
        DataRow drcr = null;
        BindingSource bs = new BindingSource();
        double Thanhtoan = 0;
        double TTtien = 0;
        double TTtienH = 0;
        double TThue = 0;
        double Conlai = 0;
        Guid id;
        int Trangthai = 0;
        string MaKH = "KL";
        string sysDBID;
        string MaCN;
        string SoCT;
        string MaKho = "HH";
        string MaVT = "";
        string MaDVT = "";
        double Thuesuat = 0;
        Guid Id
        {
            get { return id; }
            set
            {
                id = value;
                txtCode.Text = id.ToString();
            }

        }
        public fBanhang()
        {
            InitializeComponent();
            _data = new DataMasterDetail("DT3B", "7");

            _frmDesign = new FormDesigner(_data,bs);
            _data.GetData();
            dt = Getstruct();
            getdata();
            bs.DataSource = _data.DsData;
            this.bs.DataMember = _data.DsData.Tables[0].TableName;
            _data.DsData.Tables[0].ColumnChanged += FBanhang_ColumnChanged;
            gridControl1.DataSource = bs;
            bs.CurrentChanged += Bs_CurrentChanged;
            gridControl1.DataMember = this._data.DrTable["TableName"].ToString();

            gridLookUpEdit1.Properties.ImmediatePopup = true;
            gridView1.KeyUp += new KeyEventHandler(gridView1_KeyUp);
            this.KeyUp += new KeyEventHandler(fBanhang_KeyUp);

            MaCN = Config.GetValue("MaCN").ToString();
            sysDBID = Config.GetValue("sysDBID").ToString();
            NewCT();
        }

        private void Bs_CurrentChanged(object sender, EventArgs e)
        {
            if ( (this._data.DrCurrentMaster.RowState == DataRowState.Deleted || this._data.DrCurrentMaster.RowState == DataRowState.Detached)) return;
            DataRowView drv = (bs.Current as DataRowView);
            if (drv == null) return;
            this._data.DrCurrentMaster = drv.Row;
            if (_data.DrCurrentMaster["TTien"] != DBNull.Value)
            {
                TTtien = double.Parse(_data.DrCurrentMaster["TTien"].ToString());
                cTongtien.Text = TTtien.ToString("### ### ### ##0");
            }

        }

        private void FBanhang_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "TTien")
            {
                TTtien = double.Parse( e.Row["TTien"].ToString());
                cTongtien.Text = TTtien.ToString("### ### ### ##0");
            }
        }

        void fBanhang_KeyUp(object sender, KeyEventArgs e)
        {
            if (((e.KeyCode == Keys.I) || (e.KeyCode == Keys.S) || (e.KeyCode == Keys.P)) && e.Control)
            {
                sIn.Select();
                sIn_Click(sIn, new EventArgs());
                sThanhtoan.Select();
            }
        }

        void gridView1_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.F4 && bs.Current != null)
            {
                GridView gvMain = sender as GridView;
               gvMain.DeleteSelectedRows();

            }
        }
        
        CDTGridLookUpEdit dmkhoGrid;
        FormDesigner _frmDesign;
        DataMasterDetail _data;
        DataSingle _dataKho;
        private void getdata()
        {
            string sql = "select * from dmvt";
            dmvt = db.GetDataTable(sql);
            gridLookUpEdit1.Properties.DataSource = dmvt;
            repositoryItemGridLookUpEdit1.DataSource = dmvt;
            sql = "select * from dmdvt";
            dmdvt = db.GetDataTable(sql);
            if (DataFactory.publicCDTData.findCDTData("DMVT", "", "") == null)
            {
                DataSingle _dbdmvt = new DataSingle("DMVT", "7");
                _dbdmvt.GetData();
                DataFactory.publicCDTData.AddCDTData(_dbdmvt);
            }
            if(_data!=null && _data.DsStruct.Tables.Count > 1)
            {
                DataRow[] lstDr = _data.DsStruct.Tables[1].Select("FieldName='MaKho'") ;
                if (lstDr.Length > 0)
                {
                    _dataKho = new DataSingle("DMKho", "7");
                    _dataKho.GetData();
                    DataFactory.publicCDTData.AddCDTData(_dataKho);
                    dmkhoGrid = _frmDesign.GenGridLookupEdit(lstDr[0], true);
                    dmkhoGrid.EditValueChanged += DmkhoGrid_EditValueChanged;
                    LayoutControlItem iKho = new LayoutControlItem(layoutControl1, dmkhoGrid);
                    iKho.Text = "Chọn kho:";

                    if (_dataKho.DsData != null && _dataKho.DsData.Tables[0].Rows.Count > 0)
                    {
                        MaKho = _dataKho.DsData.Tables[0].Rows[0]["MaKho"].ToString();
                        dmkhoGrid.EditValue = MaKho;
                    }
                        
                }
            }


        }

        private void DmkhoGrid_EditValueChanged(object sender, EventArgs e)
        {
            CDTGridLookUpEdit tmp = sender as CDTGridLookUpEdit;
            tmp.Refresh();
            string value = tmp.EditValue.ToString();
                if (value != null) MaKho = value;
        }

        private DataTable Getstruct()
        {
            DataTable t = new DataTable();
            string sql = "select a.*, b.TenVT from dt3B a inner join dmvt b on a.mavt=b.mavt where 1=0";
            t = db.GetDataTable(sql);
            
            return t;
        }

        private void gridLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (gridLookUpEdit1.EditValue == null) return;
            
            string mavt = gridLookUpEdit1.EditValue.ToString();
            DataRow[] drvt = dmvt.Select("MaVT='" + mavt + "'");
            
            if (drvt.Length == 0) return;
            if (drvt[0]["GiaBan"] == DBNull.Value) return;
            //string tenvt = drvt[0]["TenVT"].ToString();
            MaVT = drvt[0]["MaVT"].ToString();
            MaDVT= drvt[0]["MaDVT"].ToString();
            cDongia.EditValue = double.Parse(drvt[0]["GiaBan"].ToString());
            csoluong.EditValue = 1;
            Thuesuat = double.Parse(drvt[0]["Thuesuat"].ToString()); 
            
        }

        private void sThem_Click(object sender, EventArgs e)
        {
            if (MaVT!=null && MaVT!=string.Empty)
            {
                gridView1.AddNewRow();
                DataRow drDt = _data.LstDrCurrentDetails[_data.LstDrCurrentDetails.Count - 1];
                //string tenvt = drvt[0]["TenVT"].ToString();
                drDt["MaVT"] = MaVT;
                drDt["GiaCT"] = double.Parse(cDongia.EditValue.ToString());
                drDt["MaDVT"] = MaDVT;
                drDt["Thuesuat"] = Thuesuat;
                drDt["Soluong"]= double.Parse(csoluong.EditValue.ToString());
                drDt["MaKho"] = MaKho;
                drDt.Table.Rows.Add(drDt);
                drDt.EndEdit();
                gridLookUpEdit1.EditValue = null;
                csoluong.EditValue = 1;
                cDongia.EditValue = 0;
                gridView1.RefreshData();
            }
            gridLookUpEdit1.Focus();
        }

        private void sIn_Click(object sender, EventArgs e)
        {
            if (_data.DataChanged)
                if (Save())
                {

                    sNew.Enabled = true;
                    Print(isPrint.Checked);
                }

           
            sThanhtoan.Select();
        }
        private void New_Click(object sender, EventArgs e)
        {
            if (_data.DataChanged)
            {
                MessageBox.Show("Lưu trước khi thêm mới");
                return;
            }
            else
            {
                NewCT();
                
                sNew.Enabled = false;
                gridControl1.Focus();
            }
           
        }
        private void NewCT()
        {
            _frmDesign.formAction = FormAction.New;

            _data.LstDrCurrentDetails.Clear();
            bs.AddNew();

            bs.EndEdit();
           // _frmDesign.RefreshGridLookupEdit();
            //object o = db.GetValueByStore("AutoCreate", new string[] { "@tbName", "@sysDBID", "@newvalue" }, new object[] { "MT3B", 7, "0" }, new ParameterDirection[] { ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Output }, 2);
            //if (o != null) SoCT = o.ToString();
            //else SoCT = "BL" + DateTime.Now.Year.ToString().Substring(2, 2) + "01.00001";
        }
        private bool Save()
        {
            CDTControl.DataAction Dataaction = _data.DrCurrentMaster.RowState == DataRowState.Added ? CDTControl.DataAction.Insert : CDTControl.DataAction.Update;
            if (_data.UpdateData(Dataaction))
                return true;
            else
                return false;

        }
        private void Print(bool isprint)
        {
            int[] newIndex = { bs.Position };
            BeforePrint bp = new BeforePrint(_data, newIndex);
            if (isprint)
                bp.simpleButtonIn_Click(sIn, new EventArgs());
            else
                bp.ShowDialog();
        }
        private void SetVariables(DevExpress.XtraReports.UI.XtraReport rptTmp)
        {
            foreach (DictionaryEntry de in Config.Variables)
            {
                string key = de.Key.ToString();
                if (key.Contains("@"))
                    key = key.Remove(0, 1);
                XRControl xrc = rptTmp.FindControl(key, true);
                if (xrc != null)
                {
                    string value = de.Value.ToString();
                    try
                    {
                        if (value.Contains("/"))
                            xrc.Text = DateTime.Parse(value).ToShortDateString();
                        else
                            xrc.Text = value;
                    }
                    catch
                    {
                        xrc.Text = value;
                    }
                    xrc = null;
                }
            }
        }
        private void editPrint()
        {
            //string path;
            //DevExpress.XtraReports.UI.XtraReport rptTmp = null;
            //if (Config.GetValue("DuongDanBaoCao") != null)
            //    path = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("Package").ToString() + "\\" +  "billBH.repx";
            //else
            //    path = Application.StartupPath + "\\Reports\\" + Config.GetValue("Package").ToString() + "\\" + "billBH.repx";
            
            //if (System.IO.File.Exists(path))
            //    rptTmp = DevExpress.XtraReports.UI.XtraReport.FromFile(path, true);            
            //else
            //    rptTmp = new DevExpress.XtraReports.UI.XtraReport();
            //if (rptTmp != null)
            //{
            //    rptTmp.DataSource = dt;
            //    XRDesignFormEx designForm = new XRDesignFormEx();
            //    designForm.OpenReport(rptTmp);
            //    if (System.IO.File.Exists(path))
            //        designForm.FileName = path;
            //    designForm.KeyPreview = true;
            //    designForm.Show();
            //}
        }
        private void calcEdit1_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
            
                cThanhtien.EditValue = Math.Round(double.Parse(csoluong.EditValue.ToString()) * double.Parse(cDongia.EditValue.ToString()), 0);

            }
            catch { }
        }

        private void calcEdit2_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                cThanhtien.EditValue = Math.Round(double.Parse(csoluong.EditValue.ToString()) * double.Parse(cDongia.EditValue.ToString()), 0);
            }
            catch { }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            editPrint();
        }

        private void fBanhang_Load(object sender, EventArgs e)
        {

        }

        private void sThanhtoan_Click(object sender, EventArgs e)
        {
            fThanhtoan fthanhtoan = new fThanhtoan();
            fthanhtoan.Sotien = TTtien;

            fthanhtoan.ShowDialog();
            Conlai = TTtien - fthanhtoan.Sotien;
            if (_data.DrCurrentMaster != null)
                _data.DrCurrentMaster["TienKT"] = fthanhtoan.Sotien;
            cConlai.EditValue = Conlai;
            cDaTT.EditValue = fthanhtoan.Sotien;
            sNew.Select();
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            cDongia.Enabled = !checkEdit1.Checked;
        }

        private void sTim_Click(object sender, EventArgs e)
        {
            _data.ConditionMaster = "SoCT='" + txtCode.Text +"'";
            _data.Condition = string.Empty;
            _data.GetData();
            bs.DataSource = _data.DsData;
            this.bs.DataMember = _data.DsData.Tables[0].TableName;
            _data.DsData.Tables[0].ColumnChanged += FBanhang_ColumnChanged;
            gridControl1.DataSource = bs;
            gridControl1.DataMember = this._data.DrTable["TableName"].ToString();
            bs.MoveFirst();

            //gridControl1.RefreshDataSource();
        }

       
    }
}