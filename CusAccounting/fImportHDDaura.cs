using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using ErrorManager;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using CDTControl;
using CDTDatabase;
using CDTLib;
using DataFactory;
using CBSControls;
using FormFactory;
using DevControls;
namespace CusAccounting
{

    public partial class fImportHDDaura : DevExpress.XtraEditors.XtraForm
    {
        [Obsolete]
        public fImportHDDaura()
        {
            InitializeComponent();
            DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;

        }

        Database _db = Database.NewDataDatabase();
        Database _dbStruct = Database.NewStructDatabase();
        BindingSource bs = new BindingSource();

        DataTable tbMT = ThietKedulieu.CreateHoadon();
        DataTable tbDT = ThietKedulieu.CreateHHDV();
        DataSet ds = new DataSet();

        DataSingle dbdmKH;//= new DataSingle("DMKH","7");
        DataSingle dbdmVT;//= new DataSingle("DMVT", "7");
        DataSingle dbdmKho;// = new DataSingle("DMKho", "7");
        DataSingle dbdmDVT;//= new DataSingle("DMDVT", "7");
        DataSingle dbdmHTTT;// = new DataSingle("DMHTTT", "7");
        DataSingle dbdmTk;//= new DataSingle("DMtk", "7");
        DataSingle DictionaryName;
        DevControls.CDTRepGridLookup reHTTT;
        DevControls.CDTRepGridLookup reDMKH;
        DevControls.CDTRepGridLookup reDMKH1;
        DevControls.CDTRepGridLookup reDMTK;
        DevControls.CDTRepGridLookup reDMVT;
        DevControls.CDTRepGridLookup reDMVT1;
        DevControls.CDTRepGridLookup reDMKHO;
        DevControls.CDTRepGridLookup reDMDVT;
        DataMasterDetail _dataMt32;
        BindingSource bsMT32 = new BindingSource();
        FormDesigner _frmDesign;
        private void btSelectPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                tPath.Text = dialog.SelectedPath;
            }
        }

        private void btLoadData_Click(object sender, EventArgs e)
        {
            if (tPath.Text == string.Empty) return;
            string[] files = Directory.GetFiles(tPath.Text);
            tbDT.Rows.Clear();
            tbMT.Rows.Clear();
            _dataMt32 = new DataMasterDetail("DT32", "7");
            _dataMt32.ConditionMaster = "1=0";
            _dataMt32.GetData();
            bsMT32.DataSource = _dataMt32.DsData;
            bsMT32.DataMember = _dataMt32.DsData.Tables[0].TableName;
            foreach (string s in files)
            {
                if (Path.GetExtension(s) != ".xml") continue;
                DataRow drMT = tbMT.NewRow();
                drMT["MTID"] = Guid.NewGuid();
                XmlDocument doc = new XmlDocument();
                doc.Load(s);
                List<string> lstArtt = new List<string>();
                // var hoadon = doc.DocumentElement.InnerXml.ParseJSON<HDon>();
                string jsonText = JsonConvert.SerializeXmlNode(doc.DocumentElement, Newtonsoft.Json.Formatting.None, true);
                XmlNodeList dsdv = doc.DocumentElement.GetElementsByTagName("DSHHDVu");

                jsonText= jsonText.Replace("\"@Id\"","\"Id\""); jsonText = jsonText.Replace("\"#text\"", "\"Text\"");

                HDon hd = JsonConvert.DeserializeObject<HDon>(jsonText);

                tbMT.Rows.Add(CreateMTRow(drMT, hd));
                List<HHDVu> lstHHDV = new List<HHDVu>();
                if (dsdv.Count >= 1)
                {
                    foreach (XmlNode nodehhdv in dsdv[0].ChildNodes)
                    {
                        string jsonDSDV = JsonConvert.SerializeXmlNode(nodehhdv, Newtonsoft.Json.Formatting.None, true);
                        HHDVu hh = JsonConvert.DeserializeObject<HHDVu>(jsonDSDV);
                        if (hh != null)// lstHHDV.Add(hh)
                        {
                            DataRow drDT = tbDT.NewRow();
                            tbDT.Rows.Add(CreateDTRow(drMT, drDT, hh));
                        }     

                    }

                }
            }
            gridControl1.DataSource = bs;
            gridControl2.DataSource = bs;
            gridControl2.DataMember = tbDT.TableName;// ds.Relations[0].RelationName;

        }
        private DataRow CreateMTRow(DataRow drMT,HDon hd)
        {


            if (hd.MCCQT != null)
                drMT["MCCQT"] = hd.MCCQT.text;
            drMT["Ngayhd"] = hd.DLHDon.TTChung.NLap;
            drMT["Sohoadon"] = hd.DLHDon.TTChung.SHDon;
            drMT["Kyhieu"] = hd.DLHDon.TTChung.KHHDon;
            drMT["HTTToan"] = hd.DLHDon.TTChung.HTTToan;
            drMT["TenKH"] = hd.DLHDon.NDHDon.NMua.Ten;
            drMT["MST"] = hd.DLHDon.NDHDon.NMua.MST;
            drMT["DiaChi"] = hd.DLHDon.NDHDon.NMua.DChi;
            drMT["Ongba"] = hd.DLHDon.NDHDon.NMua.HVTNMHang;
            if (hd.DLHDon.NDHDon.TToan != null)
            {
                drMT["TTienH"] = hd.DLHDon.NDHDon.TToan.TgTTTBSo;
                drMT["TThue"] = hd.DLHDon.NDHDon.TToan.TgTThue;
                drMT["TTien"] = hd.DLHDon.NDHDon.TToan.TgTCThue;
            }

           // drMT["MaThue"] = hd.DLHDon.NDHDon.TToan.THTTLTSuat.TTSuat.TSuat.Replace("%", "");
            drMT["TkNo"] = geTkNo.EditValue.ToString();
            drMT.EndEdit();
            return drMT;
        }
        private DataRow CreateDTRow(DataRow drMT, DataRow drDT, HHDVu hh)
        {

            drDT["MTID"] = drMT["MTID"];
            drDT["MTIDDT"] = Guid.NewGuid();
            drDT["MaKho"] = geMaKho.EditValue.ToString();
            drDT["TenVT"] = hh.THHDVu;
            drDT["DVT"] = hh.DVTinh;
            if (hh.SLuong == null) drDT["Soluong"] = 0;
            else drDT["Soluong"] = hh.SLuong;
            //drDT["Soluong"] = (hh.SLuong==null)? 0: hh.SLuong;
            if (hh.DGia == null) drDT["DonGia"] = 0;
            else drDT["DonGia"] = hh.DGia;
            if (hh.TLCKhau != null)
                drDT["TileCK"] = hh.TLCKhau.Replace("%", "");
            if (hh.STCKhau != null)
                drDT["CK"] = hh.STCKhau;
            else
            {
                hh.STCKhau = 0;
                drDT["CK"] = hh.STCKhau;
            }
            drDT["TTien"] = hh.ThTien;
            if (hh.TSuat != null)
                drDT["MaThueCT"] = hh.TSuat.Replace("%", "");
            else
                drDT["MaThueCT"] = "KT";
            double Thuesuat = 0;
            if (hh.TSuat != null)
                double.TryParse(hh.TSuat.Replace("%", ""), out Thuesuat);
            else
                Thuesuat = 0;
            drDT["Thuesuat"] = Thuesuat;
          
            
            
            
            drDT["TienThue"] = Math.Round((double)((hh.ThTien - hh.STCKhau) * Thuesuat / 100), 0);
            drDT["TkDthu"] = geTkdthu.EditValue.ToString();
            drDT["Tkgv"] = geTkgv.EditValue.ToString();
            drDT["Tkkho"] = geTkkho.EditValue.ToString();
            drDT["isDV"] = 0;
            drDT.EndEdit();
            return drDT;
        }
        private void fImportHDDauRa_Load(object sender, EventArgs e)
        {

            //Lấy dữ liệu MT32
            _dataMt32 = new DataMasterDetail("DT32", "7");
            _dataMt32.ConditionMaster = "1=0";
            _dataMt32.GetData();
            bsMT32.DataSource = _dataMt32.DsData;
            bsMT32.DataMember = _dataMt32.DsData.Tables[0].TableName;
            _frmDesign = new FormDesigner(_dataMt32, bsMT32);
            dxErrorProviderMain.DataSource = bs;
            
            foreach(DataRow drCol in _dataMt32.DsStruct.Tables[0].Rows)
            {
                switch (drCol["FieldName"].ToString().ToLower())
                {
                    case "mahttt":
                        reHTTT = _frmDesign.GenRIGridLookupEdit(drCol);
                        break;
                    case "makh":
                        reDMKH = _frmDesign.GenRIGridLookupEdit(drCol);
                        reDMKH1 = _frmDesign.GenRIGridLookupEdit(drCol);
                        reDMKH1.DisplayMember = "TenKH";
                        break;
                    case "tkno":
                      reDMTK= _frmDesign.GenRIGridLookupEdit(drCol);
                        break;
                }
            }
            foreach (DataRow drCol in _dataMt32.DsStruct.Tables[1].Rows)
            {
                switch (drCol["FieldName"].ToString().ToLower())
                {
                    case "mavt":
                        reDMVT = _frmDesign.GenRIGridLookupEdit(drCol);
                        reDMVT1 = _frmDesign.GenRIGridLookupEdit(drCol);
                        reDMVT1.DisplayMember = "TenVT";
                        break;
                    case "makho":
                        reDMKHO = _frmDesign.GenRIGridLookupEdit(drCol);
                        break;
                    case "madvt":
                        reDMDVT = _frmDesign.GenRIGridLookupEdit(drCol);
                        break;
                }
            }
            dbdmKH = publicCDTData.findCDTData("DMKH", "", "");
            if (dbdmKH == null) dbdmKH = new DataSingle("DMKH", "7");
            dbdmVT = publicCDTData.findCDTData("DMVT", "", "");
            if (dbdmVT == null) dbdmVT = new DataSingle("DMVT", "7");
            dbdmKho = publicCDTData.findCDTData("DMKho", "", "");
            if (dbdmKho == null) dbdmKho = new DataSingle("DMKho", "7");
            dbdmDVT = publicCDTData.findCDTData("DMDVT", "", "");
            if (dbdmDVT == null) dbdmDVT = new DataSingle("DMDVT", "7");
            dbdmHTTT = publicCDTData.findCDTData("DMHTTT", "", "");
            if (dbdmHTTT == null) dbdmHTTT = new DataSingle("DMHTTT", "7");
            dbdmTk = publicCDTData.findCDTData("DMTK", "TK not in (select  TK=case when TKMe is null then '' else TKMe end from DMTK group by TKMe)", "");
            if (dbdmTk == null) dbdmTk = new DataSingle("DMtk", "7");
            if (!dbdmKH.FullData) dbdmKH.GetData();
            if (!dbdmVT.FullData) dbdmVT.GetData();
            if (!dbdmKho.FullData) dbdmKho.GetData();
            if (!dbdmDVT.FullData) dbdmDVT.GetData();
            if (!dbdmHTTT.FullData) dbdmHTTT.GetData();
            if (!dbdmTk.FullData) dbdmTk.GetData();


            //Thiết kế repository
            //reHTTT.DataSource = dbdmHTTT.DsData.Tables[0]; reHTTT.ValueMember = "MaHTTT"; reHTTT.DisplayMember = "MaHTTT";
            gCMaHTTT.ColumnEdit = reHTTT;

            //reDMKH.DataSource = dbdmKH.DsData.Tables[0]; reDMKH.ValueMember = "MaKH"; reDMKH.DisplayMember = "MaKH";
            gCMaKH.ColumnEdit = reDMKH;gCTenKH.ColumnEdit = reDMKH1;
            //reDMTK.DataSource = dbdmTk.DsData.Tables[0]; reDMTK.ValueMember = "TK"; reDMKH.DisplayMember = "TK";
            gCTkNo.ColumnEdit = reDMTK; gCTkDT.ColumnEdit = reDMTK; gCTkGV.ColumnEdit = reDMTK; gCTkKho.ColumnEdit = reDMTK;
            //reDMVT.DataSource = dbdmVT.DsData.Tables[0]; reDMVT.ValueMember = "TK"; reDMVT.DisplayMember = "MaVT";
            gCMaVT.ColumnEdit = reDMVT; gCTenVT.ColumnEdit = reDMVT1;
            //reDMKHO.DataSource = dbdmKho.DsData.Tables[0]; reDMKHO.ValueMember = "MaKho"; reDMKHO.DisplayMember = "MaKho";
            gCMaKho.ColumnEdit = reDMKHO;
            //reDMDVT.DataSource = dbdmDVT.DsData.Tables[0]; reDMDVT.ValueMember = "MaDVT"; reDMDVT.DisplayMember = "MaDVT";
            gCMaDVT.ColumnEdit = reDMDVT;


            geMaKho.Properties.DataSource = dbdmKho.DsData.Tables[0]; geMaKho.Properties.ValueMember = "MaKho"; geMaKho.Properties.DisplayMember = "TenKho"; geMaKho.EditValue = "HH";
            geTkNo.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkNo.Properties.ValueMember = "TK"; geTkNo.Properties.DisplayMember = "TK"; geTkNo.EditValue = "131";
            geTkdthu.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkdthu.Properties.ValueMember = "TK"; geTkdthu.Properties.DisplayMember = "TK"; geTkdthu.EditValue = "5111";
            geTkgv.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkgv.Properties.ValueMember = "TK"; geTkgv.Properties.DisplayMember = "TK"; geTkgv.EditValue = "632";
            geTkkho.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkkho.Properties.ValueMember = "TK"; geTkkho.Properties.DisplayMember = "TK"; geTkkho.EditValue = "1561";
            tbMT.PrimaryKey = new DataColumn[] { tbMT.Columns["MTID"] };
            tbDT.PrimaryKey = new DataColumn[] { tbDT.Columns["MTIDDT"] };

            ds.Tables.AddRange(new DataTable[] { tbMT, tbDT });

            DataColumn pk = tbMT.Columns["MTID"];
            DataColumn fk = tbDT.Columns["MTID"];
            DataRelation dr = new DataRelation(tbDT.TableName, pk, fk, true);
            try
            {
                ds.Relations.Add(dr);
            }
            catch
            { }
            bs.DataSource = ds;
            bs.DataMember = tbMT.TableName;
            reHTTT.KeyUp += ReHTTT_KeyUp;
            reDMKH.KeyUp += ReDMKH_KeyUp;
            reDMVT.KeyUp += ReDMVT_KeyUp;
            reDMDVT.KeyUp += ReDMDVT_KeyUp;
            DictionaryName = new DataSingle("DictionaryName", "7");
            DictionaryName.GetData();
            gridControl2.KeyUp += GridControl2_KeyUp;
            gridView1.CustomDrawRowIndicator += GridView1_CustomDrawRowIndicator;
            gridView2.CustomDrawRowIndicator += GridView1_CustomDrawRowIndicator;
            gridView1.IndicatorWidth = 40;
            gridView2.IndicatorWidth = 40;
        }

        private void GridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && (e.RowHandle >= 0))
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

       
        private void ReDMDVT_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                string Code = (sender as GridLookUpEdit).EditValue.ToString();
                string Name = dr["DVT"].ToString();
                string TableName = "DMDVT";
                if (DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'").Length == 0)
                {
                    DataRow drDic = DictionaryName.DsData.Tables[0].NewRow();
                    drDic["Code"] = Code; drDic["Name"] = Name; drDic["TableName"] = TableName;
                    DictionaryName.DsData.Tables[0].Rows.Add(drDic);
                    DictionaryName.UpdateData(DataAction.Insert);
                }
            }
        }

        private void GridControl2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11 && gridControl2.DataMember!="")
            {
                gridControl2.DataSource = tbDT;
                gridControl2.DataMember = "";
            }
            else
            {
                gridControl2.DataSource = bs;
                gridControl2.DataMember = tbDT.TableName;
            }
        }

        private void ReDMVT_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                string Code = (sender as GridLookUpEdit).EditValue.ToString();
                string Name = dr["TenVT"].ToString();
                string TableName = "DMVT";
                if (DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'").Length == 0)
                {
                    DataRow drDic = DictionaryName.DsData.Tables[0].NewRow();
                    drDic["Code"] = Code; drDic["Name"] = Name; drDic["TableName"] = TableName;
                    DictionaryName.DsData.Tables[0].Rows.Add(drDic);
                    DictionaryName.UpdateData(DataAction.Insert);
                }
            }
        }

        private void ReDMKH_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                DataRow dr = (bs.Current as DataRowView).Row;
                string Code = (sender as GridLookUpEdit).EditValue.ToString();
                string Name = dr["TenKH"].ToString();
                string TableName = "DMKH";
                if (DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'").Length == 0)
                {
                    DataRow drDic = DictionaryName.DsData.Tables[0].NewRow();
                    drDic["Code"] = Code; drDic["Name"] = Name; drDic["TableName"] = TableName;
                    DictionaryName.DsData.Tables[0].Rows.Add(drDic);
                    DictionaryName.UpdateData(DataAction.Insert);
                }
            }
        }

        private void ReHTTT_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                DataRow dr = (bs.Current as DataRowView).Row;
                string Code = (sender as GridLookUpEdit).EditValue.ToString();
                string Name = dr["HTTToan"].ToString();
                string TableName = "dmHTTT";
                if(DictionaryName.DsData.Tables[0].Select("TableName='" + TableName +"' and Name='" + Name + "'").Length == 0)
                {
                    DataRow drDic= DictionaryName.DsData.Tables[0].NewRow();
                    drDic["Code"] = Code; drDic["Name"] = Name; drDic["TableName"] = TableName;
                    DictionaryName.DsData.Tables[0].Rows.Add(drDic);
                    DictionaryName.UpdateData(DataAction.Insert);
                }
            }
        }

        private void btKiemTraHTTT_Click(object sender, EventArgs e)
        {
            
            
            string[] arrHTTT = ThietKedulieu.ConvertDataTableToArray(dbdmHTTT.DsData.Tables[0], "TenHTTT");
            foreach (DataRow drMT in tbMT.Rows)
            {
                //Tìm kiếm chính xác và trong từ điển trước
                DataRow[] lstRows = dbdmHTTT.DsData.Tables[0].Select("TenHTTT='" + drMT["HTTToan"].ToString() + "'");
                if (lstRows.Length > 0)
                {
                    drMT["MaHTTT"] = lstRows[0]["MaHTTT"];
                    drMT.EndEdit();
                    continue;
                }
                else
                {
                    lstRows = DictionaryName.DsData.Tables[0].Select("Name='" + drMT["HTTToan"].ToString() + "' and TableName='dmHTTT'");
                    if (lstRows.Length > 0)
                    {
                        drMT["MaHTTT"] = lstRows[0]["Code"];
                        drMT.EndEdit();
                        continue;
                    }
                }


                int i = ThietKedulieu.BestMaxIndex(arrHTTT, drMT["HTTToan"].ToString());
                if (i >= 0 && i < arrHTTT.Length)
                {
                    drMT["MaHTTT"] = dbdmHTTT.DsData.Tables[0].Rows[i]["MaHTTT"];
                    drMT.EndEdit();
                }
                else
                {
                    int j = ThietKedulieu.LevantEdit(arrHTTT, drMT["HTTToan"].ToString());
                    if (j >= 0 && j < arrHTTT.Length)
                    {
                        drMT["MaHTTT"] = dbdmHTTT.DsData.Tables[0].Rows[j]["MaHTTT"];
                        drMT.EndEdit();
                    }
                    else
                    {
                        int j1 = ThietKedulieu.FindClosestString(arrHTTT, drMT["HTTToan"].ToString().ToLower());
                            
                            if (j1 >= 0 && j1 < arrHTTT.Length)
                            {
                                drMT["MaHTTT"] = dbdmHTTT.DsData.Tables[0].Rows[j1]["MaHTTT"];
                                drMT.EndEdit();
                            }
                        
                    }
                }
            }
        }

        private void btReload_Click(object sender, EventArgs e)
        {
            dbdmKH.GetData();
            dbdmVT.GetData();
            dbdmKho.GetData();
            dbdmDVT.GetData();
            dbdmHTTT.GetData();
            dbdmTk.GetData();
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            string[] arrHTTT = ThietKedulieu.ConvertDataTableToArray(dbdmKH.DsData.Tables[0], "TenKH");

            foreach (DataRow drMT in tbMT.Rows)
            {
                DataRow[] lstRows = dbdmKH.DsData.Tables[0].Select("TenKH='" + drMT["TenKH"].ToString() + "'");
                if (lstRows.Length > 0)
                {
                    drMT["MaKH"] = lstRows[0]["MaKH"];
                    drMT.EndEdit();
                    continue;
                }
                else
                {
                    lstRows = DictionaryName.DsData.Tables[0].Select("Name='" + drMT["TenKH"].ToString() + "' and TableName='DMKH'");
                    if (lstRows.Length > 0)
                    {
                        drMT["MaKH"] = lstRows[0]["Code"];
                        drMT.EndEdit();
                        continue;
                    }
                }
                int j = ThietKedulieu.LevantEdit(arrHTTT, drMT["TenKH"].ToString());
                    if (j >= 0 && j < arrHTTT.Length)
                    {
                        drMT["MaKH"] = dbdmKH.DsData.Tables[0].Rows[j]["MaKH"];
                        drMT.EndEdit();
                    }
                    else
                    {
                        int j1 = ThietKedulieu.FindClosestString(arrHTTT, drMT["TenKH"].ToString().ToLower());

                        if (j1 >= 0 && j1 < arrHTTT.Length)
                        {
                            drMT["MaKH"] = dbdmKH.DsData.Tables[0].Rows[j1]["MaKH"];
                            drMT.EndEdit();
                        }

                    }
               // }
            }
        }

        private void btAddMaKH_Click(object sender, EventArgs e)
        {
            foreach(DataRow drMT in tbMT.Rows)
            {
                if(drMT["MaKH"]==DBNull.Value || drMT["MaKH"].ToString() == string.Empty)
                {
                    if (dbdmKH.DsData.Tables[0].Select("MaKH='" + drMT["MST"].ToString() + "'").Length > 0) continue;
                    DataRow drKH = dbdmKH.DsData.Tables[0].NewRow();
                    drKH["MaKH"] = drMT["MST"];
                    drKH["TenKH"] = drMT["TenKH"];
                    drKH["DiaChi"] = drMT["DiaChi"];
                    drKH["Doitac"] = drMT["Ongba"];
                    drKH["MST"] = drMT["MST"];
                    drKH["SDT"] = drMT["SDThoai"];
                    drKH["Email"] = drMT["DCTDTu"];
                    drKH["TkNganHang"] = drMT["STKNHang"];
                    drKH["NganHang"] = drMT["TNHang"];
                    drKH.EndEdit();
                    dbdmKH.DsData.Tables[0].Rows.Add(drKH);
                }
                dbdmKH.UpdateData();
            }
        }

        private void btCheckVT_Click(object sender, EventArgs e)
        {
            DataRow[] lstRowsDic = DictionaryName.DsData.Tables[0].Select("TableName='DMVT'");//Name='" + drDT["TenVT"].ToString() + "' and 
            string[] arrVTDic= ThietKedulieu.ConvertDataRowsToArray(lstRowsDic, "Name");
            double dic2 = 1000;
            double dic1 = 1000;
            string[] arrVT=ThietKedulieu.ConvertDataTableToArray(dbdmVT.DsData.Tables[0], "TenVT");
            foreach (DataRow drDT in tbDT.Rows)
            {
                string TenVT = drDT["TenVT"].ToString();

                //Xử lý dấu ngoặc
                TenVT = ThietKedulieu.RemoveTextBetweenParentheses(TenVT).Trim();
                DataRow[] lstRows = dbdmVT.DsData.Tables[0].Select("TenVT='" + TenVT + "'");// Tìm chính xác trên danh mục
                if (lstRows.Length > 0)
                {
                    drDT["MaVT"] = lstRows[0]["MaVT"];
                    drDT.EndEdit();
                    continue;
                }
                else
                {
                    lstRows = DictionaryName.DsData.Tables[0].Select("Name='" + drDT["TenVT"].ToString() + "' and TableName='DMVT'");
                    if (lstRows.Length > 0)
                    {
                        drDT["MaVT"] = lstRows[0]["MaVT"];
                        drDT.EndEdit();
                        continue;
                    }
                    else
                    {
                        //  dic = 1000;
                        int j2 = ThietKedulieu.LevantEditWithWeightHasDic(arrVTDic, TenVT, 1000, out dic2); ; //Tìm khoảng cách trên Dic
                        int j1 = ThietKedulieu.LevantEditWithWeightHasDic(arrVT, TenVT,1000, out dic1);//Khoảng cách trên vật tư
                        if (dic2 < dic1)//lấy trên mã 
                        {
                            if (j2 >= 0 && j2 < arrVTDic.Length)
                            {
                                drDT["MaVT"] = lstRowsDic[0]["Code"];
                                drDT["dictance"] = dic2;
                                drDT.EndEdit();
                                continue;
                            }
                        }
                        else
                        {
                            if (j1 >= 0 && j1 < arrVT.Length)
                            {
                                drDT["MaVT"] = dbdmVT.DsData.Tables[0].Rows[j1]["MaVT"];
                                drDT["TKKho"] = dbdmVT.DsData.Tables[0].Rows[j1]["TkKho"];
                                drDT["dictance"] = dic1;
                                drDT.EndEdit();
                            }
                        }
                       
                    }
                }

                
                        //int j1 = ThietKedulieu.FindClosestString(arrVT, drDT["TenVT"].ToString().ToLower());

                        //if (j1 >= 0 && j1 < arrVT.Length)
                        //{
                        //    drDT["MaVT"] = dbdmVT.DsData.Tables[0].Rows[j1]["MaVT"];
                        //    drDT["TKKho"] = dbdmVT.DsData.Tables[0].Rows[j1]["TkKho"];
                        //    drDT.EndEdit();
                        //}

                // }
            }
        }
        BindingSource bsVT = new BindingSource();
        private void btThemVT_Click(object sender, EventArgs e)
        {
            dxErrorVT.DataSource = bsVT;
            bsVT.DataSource = dbdmVT.DsData.Tables[0];
            bs.MoveFirst();
            List<string> lsTenVT = new List<string>();
            int idx = 0;
            while (idx < bs.Count)
            {
                DataRowView drv = (bs.Current as DataRowView);
                DataRow[] lstDT = tbDT.Select("MTID='" + drv["MTID"].ToString() + "'");

                foreach (DataRow drDT in lstDT)
                {
                    if (drDT["MaVT"] == DBNull.Value || drDT["MaVT"].ToString() == string.Empty)
                    {
                        string TenVT = drDT["TenVT"].ToString();
                        //Xử lý dấu ngoặc
                        TenVT = ThietKedulieu.RemoveTextBetweenParentheses(TenVT);
                        //Kiểm tra tên vật tư đã tồn tại chưa
                        string[] arrTenVT = lsTenVT.ToArray();

                        int j = ThietKedulieu.LevantEditWithWeight(arrTenVT,TenVT);


                        if (j >= 0 && j < arrTenVT.Length) //Đã tồn tại tên vật tư đã thêm rồi
                        { 
                            //dbdmVT.CancelUpdate();
                            //bsVT.RemoveCurrent();
                            //bsVT.DataSource = dbdmVT.DsData.Tables[0];
                            continue;
                        }

                        bsVT.AddNew();
                        bsVT.EndEdit();
                        bsVT.MoveLast();

                        DataRowView drvT = (bsVT.Current as DataRowView);
                        
                            if (drvT != null)
                            dbdmVT.DrCurrentMaster = drvT.Row;

                        dbdmVT.DrCurrentMaster["TenVT"] = TenVT;
                        dbdmVT.DrCurrentMaster["MaDVT"] = drDT["MaDVT"];
                        dbdmVT.DrCurrentMaster.EndEdit();
                       
                        dbdmVT.CheckRules(DataAction.Insert);
                        if (dbdmVT.DsData.HasErrors)
                        {
                            bs.MoveLast();
                            idx = bs.Count;
                            break;
                        }
                        else
                        {
                            if (dbdmVT.UpdateData())
                            {
                                lsTenVT.Add(TenVT);
                            }
                        }
                    }
                }
                bs.MoveNext();
                idx++;
            }
            

            
            
        }

        private void btCheckDVT_Click(object sender, EventArgs e)
        {
            string[] arrDVT = ThietKedulieu.ConvertDataTableToArray(dbdmDVT.DsData.Tables[0], "TenDVT");

            foreach (DataRow drDT in tbDT.Rows)
            {
                DataRow[] lstRows = dbdmDVT.DsData.Tables[0].Select("TenDVT='" + drDT["DVT"].ToString() + "'");
                if (lstRows.Length > 0)
                {
                    drDT["MaDVT"] = lstRows[0]["MaDVT"];
                    drDT.EndEdit();
                    continue;
                }
                else
                {
                    lstRows = DictionaryName.DsData.Tables[0].Select("Name='" + drDT["DVT"].ToString() + "' and TableName='DMDVT'");
                    if (lstRows.Length > 0)
                    {
                        drDT["MaDVT"] = lstRows[0]["Code"];
                        drDT.EndEdit();
                        continue;
                    }
                }
                if (drDT["DVT"].ToString() == "")
                {
                    drDT["DVT"] = "Lần";
                }
                int j = ThietKedulieu.LevantEdit(arrDVT, drDT["DVT"].ToString());
                if (j >= 0 && j < arrDVT.Length)
                {
                    drDT["MaDVT"] = dbdmDVT.DsData.Tables[0].Rows[j]["MaDVT"];
                   
                    drDT.EndEdit();
                }
                else
                {
                    int j1 = ThietKedulieu.FindClosestString(arrDVT, drDT["DVT"].ToString().ToLower());

                    if (j1 >= 0 && j1 < arrDVT.Length)
                    {
                        drDT["MaDVT"] = dbdmDVT.DsData.Tables[0].Rows[j1]["MaDVT"];
                       
                        drDT.EndEdit();
                    }

                }
                // }
            }
        }
        BindingSource bsDVT = new BindingSource();
        private void btThemDVT_Click(object sender, EventArgs e)
        {
            bsDVT.DataSource = dbdmDVT.DsData.Tables[0];
            bs.MoveFirst();
            int idx = 0;
            while (idx < bs.Count)
            {
                DataRowView drv = (bs.Current as DataRowView);
                DataRow[] lstDT = tbDT.Select("MTID='" + drv["MTID"].ToString() + "'");
                foreach (DataRow drDT in lstDT)
                {
                    if (drDT["MaDVT"] == DBNull.Value || drDT["MaDVT"].ToString() == string.Empty)
                    {
                        //Create VaVT
                        // if (dbdmVT.DsData.Tables[0].Select("MaKH='" + dDT["MST"].ToString() + "'").Length > 0) continue;
                        string MaDVT = ThietKedulieu.RemoveVietnameseSigns(drDT["DVT"].ToString()).ToUpper();
                        if (dbdmDVT.DsData.Tables[0].Select("MaDVT='" + MaDVT + "'").Length > 0) break;
                        bsDVT.AddNew();
                        bsDVT.EndEdit();
                        bsDVT.MoveLast();
                        DataRowView drDVT = (bsDVT.Current as DataRowView);
                        if (drDVT != null)
                            dbdmDVT.DrCurrentMaster = drDVT.Row;
                       
                        dbdmDVT.DrCurrentMaster["MaDVT"] = MaDVT.IndexOf(" ") == -1 ? MaDVT : MaDVT.Substring(0, MaDVT.IndexOf(" "));
                        dbdmDVT.DrCurrentMaster["TenDVT"] = drDT["DVT"].ToString();
                        dbdmDVT.DrCurrentMaster.EndEdit();
                        //dbdmKH.DsData.Tables[0].Rows.Add(drVT);
                        dbdmDVT.CheckRules(DataAction.Insert);
                        if (dbdmDVT.DsData.HasErrors)
                        {
                            bs.MoveLast();
                            idx = bs.Count;
                            break;
                        }
                        else
                        {
                            dbdmDVT.UpdateData();
                        }
                    }
                }
                bs.MoveNext();
                idx++;
            }
        }
        private async void InserttoData()
        {
            bs.MoveFirst();
            bool tieptuc = true;
            //_dataMt32.ConditionMaster = "1=0";
            //_dataMt32.GetData();
            //bsMT32.DataSource = _dataMt32.DsData;
            //bsMT32.DataMember = _dataMt32.DsData.Tables[0].TableName;
            //_dataMt32.LstDrCurrentDetails.Clear();

            bool HasErr = false;
            for (int idx = 0; idx < bs.Count; idx++)
            {
                DateTime t1 = DateTime.Now;
                 UpdateProgressBar(bs.Position + 1);
                await Task.Delay(1);
                _dataMt32.LstDrCurrentDetails.Clear();

                DataRowView drv = (bs.Current as DataRowView);
                DataRow[] lstDT = tbDT.Select("MTID='" + drv["MTID"].ToString() + "'");
                if (_dataMt32.DsData.Tables[0].Select("MT32ID='" + drv["MTID"].ToString() + "'").Length > 0)
                {
                    bs.MoveNext();

                    continue;
                }
                bsMT32.AddNew();
                bsMT32.EndEdit();
                bsMT32.MoveLast();


                DataRowView drMT = (bsMT32.Current as DataRowView);

                if (drMT != null)
                    _dataMt32.DrCurrentMaster = drMT.Row;
                importMT32Row(_dataMt32.DrCurrentMaster, drv.Row);
                foreach (DataRow drDT in lstDT)
                {
                    if (!tieptuc) break;
                    DataRow drDT32 = _dataMt32.DsData.Tables[1].NewRow();

                    if (drDT32.RowState == DataRowState.Detached)
                        _dataMt32.DsData.Tables[1].Rows.Add(drDT32);
                    importDT32Row(drDT32, drDT);
                    drDT32.EndEdit();


                }
                _dataMt32.CheckRules(DataAction.Insert);
                if (_dataMt32.DsData.HasErrors)
                {
                    foreach (DataColumn col in _dataMt32.DsData.Tables[0].Columns)
                    {
                        string err = _dataMt32.DrCurrentMaster.GetColumnError(col.ColumnName);
                        if (err != string.Empty)
                        {
                            MessageBox.Show("Cột " + col.ColumnName + " Có lỗi: " + err);
                            _dataMt32.Cancel();
                            HasErr = true;
                            idx = bs.Count;
                            break;
                        }
                    }
                    foreach (DataColumn col in _dataMt32.DsData.Tables[1].Columns)
                    {
                        foreach (DataRow drCurrentDT in _dataMt32.LstDrCurrentDetails)
                        {
                            string err = drCurrentDT.GetColumnError(col.ColumnName);
                            if (err!= string.Empty)
                            {
                                MessageBox.Show("Cột " + col.ColumnName +  " có lỗi: " + err);
                                _dataMt32.Cancel();
                                HasErr = true;
                                tieptuc = false;
                                break;
                            }
                        }

                    }
                }
                else
                {

                    if (_dataMt32.UpdateData(DataAction.Insert))
                    {
                        bs.MoveNext();
                    }
                    else
                    {
                        MessageBox.Show("Thêm vào hóa đơn bị lỗi. Hóa đơn số :" + _dataMt32.DrCurrentMaster["SoHoaDon"].ToString());
                        idx = bs.Count;
                        break;
                    }
                }
                DateTime t2 = DateTime.Now;
                double tPro = (t2 - t1).TotalSeconds;
               
            }
            if (!HasErr) MessageBox.Show("Đã hoàn thành import dữ liệu");
        }
        private  void UpdateProgressBar(int value)
        {
            bool reInv = progressBar1.InvokeRequired;
            if (reInv)
            {                
                progressBar1.Invoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                progressBar1.Value = value;
            }
        }
        private void btThemHoaDon_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = bs.Count;
            progressBar1.Step = 1;
            progressBar1.Value = 0;
            InserttoData();
        }

        private void importDT32Row(DataRow drDT32, DataRow drDT)
        {
            drDT32["MT32ID"] = drDT["MTID"];
            drDT32["DT32ID"] = drDT["MTIDDT"];
            drDT32["MaKho"] = drDT["MaKho"];
            drDT32["MaVT"] = drDT["MaVT"];
            drDT32["Soluong"] = drDT["Soluong"];
            drDT32["MaThueCT"] = drDT["MaThueCT"];
            double thuesuat = 0;
            if (double.TryParse(drDT["Thuesuat"].ToString().Replace("%", ""), out thuesuat))
                drDT32["Thuesuat"] = thuesuat;
            else drDT32["Thuesuat"] = 0;

            drDT32["GiaNT"] = drDT["DonGia"];
            drDT32["PsNT"] = drDT["TTien"];
            if (drDT["TileCK"] != DBNull.Value)
                drDT32["TileCK"] = drDT["TileCK"].ToString().Replace("%", "");
            double ck = 0;
            if (double.TryParse(drDT["CK"].ToString().Replace("%", ""), out ck))
                drDT32["CK"] = ck;
            else drDT32["CK"] = 0;
            
            
            //drDT["TienThue"] = Math.Round((hh.ThTien - hh.STCKhau) * double.Parse(hh.TSuat.Replace("%", "")) / 100, 0);
            drDT32["TkDt"] = geTkdthu.EditValue.ToString();
            drDT32["Tkgv"] = geTkgv.EditValue.ToString();
            drDT32["Tkkho"] = geTkkho.EditValue.ToString();
            if(drDT["Soluong"]==DBNull.Value || drDT["Soluong"].ToString() == "0")
            {
                drDT32["isDV"] = 1;
            }
            else
                drDT32["isDV"] = 0;
            drDT32.EndEdit();
        }

        private void importMT32Row(DataRow drCurrentMaster, DataRow row)
        {
            drCurrentMaster["MCCQT"] = row["MCCQT"];
            drCurrentMaster["MT32ID"] = row["MTID"];
            drCurrentMaster["MaCT"] = "HDB";
            drCurrentMaster["NgayCT"]=row["Ngayhd"];
            drCurrentMaster["NgayHD"] = row["Ngayhd"];
            drCurrentMaster["SoHoadon"] = row["SoHoadon"];
            drCurrentMaster["Soseri"] = row["kyhieu"];
            drCurrentMaster["MaHTTT"] = row["MaHTTT"];
            drCurrentMaster["MaKH"] = row["MaKH"];
            drCurrentMaster["MST"] = row["MST"];
            drCurrentMaster["Ongba"] = row["Ongba"];
            drCurrentMaster["DiaChi"] = row["DiaChi"];
            drCurrentMaster["MaNT"] = "VND";
            drCurrentMaster["TyGia"] = 1;
            drCurrentMaster["DienGiai"] = "Xuất bán hàng";
            drCurrentMaster["TkNo"] = row["TkNo"];
            drCurrentMaster["MaThue"] = "10";
            drCurrentMaster["TkCK"] = "5211";
            drCurrentMaster["MaThue"] = "10";
           
            drCurrentMaster["PrintIndex"] =0;
            drCurrentMaster.EndEdit();

        }

        private void btXoaHD_Click(object sender, EventArgs e)
        {
            string condition = "MCCQT in ('";
            
            foreach (DataRow dr in tbMT.Rows)
            {
                condition += dr["MCCQT"].ToString() + "','";

            }
            if (tbMT.Rows.Count > 0) condition = condition.Substring(0, condition.Length - 2) + ")";
            else condition = "1=0";
            _dataMt32.ConditionMaster = condition;
            _dataMt32.GetData();
            bsMT32.DataSource = _dataMt32.DsData;
            bsMT32.DataMember = _dataMt32.DsData.Tables[0].TableName;
            bs.MoveLast();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = bsMT32.Count;
            progressBar1.Step = 1;
            progressBar1.Value = 0;
            DeleteHD();
            
        }
        private  async void DeleteHD()
        {

            while (bsMT32.Count > 0)
            {               
                bsMT32.MoveLast();
                DataRowView drMT = (bsMT32.Current as DataRowView);

                if (drMT != null)
                    _dataMt32.DrCurrentMaster = drMT.Row;
                _dataMt32.LstDrCurrentDetails.Clear();
                _dataMt32._formulaCaculator.Active = false;

                bsMT32.RemoveCurrent();
                bool isError = !_dataMt32.UpdateData(DataAction.Delete);
               UpdateProgressBar(bsMT32.Count);
                await Task.Delay(1);
                if (isError) break;
                // bs.MovePrevious();
            }
            
           
        }
    }


}
