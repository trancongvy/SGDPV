using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Net;

namespace CusAccounting
{

    public partial class fImportHDDauvao_Minv : DevExpress.XtraEditors.XtraForm
    {


        public fImportHDDauvao_Minv()
        {
            InitializeComponent();
            gridControl1.KeyUp += GridControl1_KeyUp;
        }

        private void GridControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4 && bs!=null)
            {
                bs.RemoveCurrent();
                ds.AcceptChanges();

            }
            
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
        DataSingle dbdmThueSuat;
        DataSingle DictionaryName;
        DevControls.CDTRepGridLookup reHTTT;
        DevControls.CDTRepGridLookup reDMKH;
        DevControls.CDTRepGridLookup reDMKH1;
        DevControls.CDTRepGridLookup reDMTK;
        DevControls.CDTRepGridLookup reMaThue;
        DevControls.CDTRepGridLookup reDMVT;
        DevControls.CDTRepGridLookup reDMVT1;
        DevControls.CDTRepGridLookup reDMKHO;
        DevControls.CDTRepGridLookup reDMDVT;
        DataMasterDetail _dataMt22;
        DataMasterDetail _dataMt21;
        BindingSource bsMT22 = new BindingSource();
        BindingSource bsMT21 = new BindingSource();

        DataMasterDetail _dataMt12;
        DataMasterDetail _dataMt16;
        BindingSource bsMT12 = new BindingSource();
        BindingSource bsMT16 = new BindingSource();


        FormDesigner _frmDesign;
        private async Task<string> GetData(int page)
        {
            string token = "";
            if (Config.Variables.ContainsKey("InvToken")) token = Config.GetValue("InvToken").ToString();
            else return "";
            string MST = "";
            if (Config.Variables.ContainsKey("MaSoThue")) MST = Config.GetValue("MaSoThue").ToString();
            else return "";
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            string requestUri = "";
            if (Config.Variables.ContainsKey("MInvoiceUrl")) requestUri = Config.GetValue("MInvoiceUrl").ToString();
            requestUri += "?page=" + page.ToString().Trim() + "&size=50&invoiceType=INPUT_ELECTRONIC_INVOICE&invoiceReleaseDateFrom=" + DateTime.Parse(dTungay.EditValue.ToString()).ToString("dd/MM/yyyy") + "&invoiceReleaseDateTo=" + DateTime.Parse(dDenngay.EditValue.ToString()).ToString("dd/MM/yyyy") + "&buyerTaxNo=" + MST;
            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Add("apiToken", token);
            try
            {
                System.Net.Http.HttpResponseMessage response = await client.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }

            //if (response.IsSuccessStatusCode)
            //{
            //    // Lấy dữ liệu trả về từ phản hồi
            //    string responseString = await response.Content.ReadAsStringAsync();
            //    return responseString;
            //    // Xử lý dữ liệu trả về ở đây
            //}
            //else

        }
        private async void btLoadData_Click(object sender, EventArgs e)
        {
            lstid = new string[] { };
            string data = "";
            try
            {
                data = await GetData(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            if (data == "" || data == null) return;
            tbDT.Rows.Clear();
            tbMT.Rows.Clear();
            _dataMt22 = new DataMasterDetail("DT22", "7");
            _dataMt22.ConditionMaster = "1=0";
            _dataMt22.GetData();
            _dataMt21 = new DataMasterDetail("DT21", "7");
            _dataMt21.ConditionMaster = "1=0";
            _dataMt21.GetData();
            bsMT22.DataSource = _dataMt22.DsData;
            bsMT22.DataMember = _dataMt22.DsData.Tables[0].TableName;
            bsMT21.DataSource = _dataMt21.DsData;
            bsMT21.DataMember = _dataMt21.DsData.Tables[0].TableName;

            _dataMt12 = new DataMasterDetail("DT12", "7");
            _dataMt12.ConditionMaster = "1=0";
            _dataMt12.GetData();
            _dataMt16 = new DataMasterDetail("DT16", "7");
            _dataMt16.ConditionMaster = "1=0";
            _dataMt16.GetData();

            bsMT22.DataSource = _dataMt22.DsData;
            bsMT22.DataMember = _dataMt22.DsData.Tables[0].TableName;
            bsMT21.DataSource = _dataMt21.DsData;
            bsMT21.DataMember = _dataMt21.DsData.Tables[0].TableName;

            bsMT12.DataSource = _dataMt12.DsData;
            bsMT12.DataMember = _dataMt12.DsData.Tables[0].TableName;
            bsMT16.DataSource = _dataMt16.DsData;
            bsMT16.DataMember = _dataMt16.DsData.Tables[0].TableName;

            MInvoiceList mInvoiceList = JsonConvert.DeserializeObject<MInvoiceList>(data);
            if (mInvoiceList != null)
            {
                int pagetotal = (int)mInvoiceList.totalPage;
                CreateData1page(mInvoiceList);
                if (pagetotal > 1) {
                    for (int i = 2; i <= pagetotal; i++)
                    {
                        data = await GetData(i);
                        MInvoiceList mInvoiceList1 = JsonConvert.DeserializeObject<MInvoiceList>(data);
                        CreateData1page(mInvoiceList1);
                    }
                }
            }
            gridControl1.DataSource = bs;
            gridControl2.DataSource = bs;
            gridControl2.DataMember = tbDT.TableName;// ds.Relations[0].RelationName;

        }
        string[] lstid = new string[] { };
        private void CreateData1page(MInvoiceList mInvoiceList)
        {
            if (mInvoiceList != null)
            {
                foreach (MInvoice inv in mInvoiceList.listInvoice)
                {
                    if (inv.id == null) continue;

                    if (lstid.Contains(inv.id))
                    {

                        LogFile.AppendToFile("dulieutrung.txt", inv.shdon + "  " + inv.ntnhan);
                        continue;
                    }
                    else
                    {
                        Array.Resize(ref lstid, lstid.Length + 1);
                        lstid[lstid.Length - 1] = inv.id;
                    }
                    if (inv.tthai == 6)
                        continue;
                    DataRow drMT = tbMT.NewRow();
                    drMT["MTID"] = inv.id;
                    tbMT.Rows.Add(CreateMTRow(drMT, inv));
                    int firstRow = 0;
                    if (inv.hdhhdvu != null)
                    {
                        foreach (HHDVu_Minv HHDV_minv in inv.hdhhdvu)
                        {
                            if (firstRow == 0) { drMT["DienGiai"] = HHDV_minv.ten; firstRow++; }
                            DataRow drDT = tbDT.NewRow();
                            tbDT.Rows.Add(CreateDTRow(drMT, drDT, HHDV_minv));
                        }
                    }
                    else
                    {

                    }    
                }
            }
        }
        private DataRow CreateMTRow(DataRow drMT, MInvoice hd)
        {
            if (hd.nky !=null && hd.ntao > DateTime.Parse( hd.nky.ToString()).AddDays(1) )
            {

            }
            if (hd.mhdon != null)
                drMT["MCCQT"] = hd.mhdon;

            if (hd.nky != null)
                drMT["Ngayhd"] = DateTime.Parse(hd.nky.ToString()).Date;
            else
                drMT["Ngayhd"] = DateTime.Parse(hd.tdlap.ToString()).Date;
            drMT["Sohoadon"] = "00000000".Substring(0, 8 - hd.shdon.ToString().Length) + hd.shdon.ToString();
            drMT["Kyhieu"] = hd.khhdon;
            drMT["HTTToan"] = hd.thtttoan;
            drMT["TenKH"] = hd.nbten;
            drMT["MST"] = hd.nbmst;
          //  drMT["DiaChi"] = hd.nbdchi == null || hd.nbdchi == string.Empty ? "." : hd.nbdchi;

            if (hd.nbdchi != null)
            {
                drMT["DiaChi"] = hd.nbdchi;
            }
            else
            {
                foreach (ttkhac Ttkhac in hd.ttkhac)
                {
                    if (Ttkhac.ttruong.ToLower() == "địa chỉ" && Ttkhac.dlieu != null)
                    {
                        drMT["DiaChi"] = Ttkhac.dlieu;
                        break;
                    }
                }
            }

            drMT["Ongba"] = hd.nmtnmua;

            drMT["TTienH"] =  hd.tgtcthue==null?0: hd.tgtcthue;
            drMT["TThue"] = hd.tgtthue == null ? 0 : hd.tgtthue;
            drMT["TTien"] = hd.tgtttbso == null ? 0 : hd.tgtttbso;

            if (hd.thttltsuat != null && hd.thttltsuat.Length > 0)
            {
                drMT["MaThue"] = hd.thttltsuat[0].tsuat.Replace("%", "");
            }
            else
            {
                drMT["MaThue"] = "KCT";
            }

            // drMT["MaThue"] = hd.DLHDon.NDHDon.TToan.THTTLTSuat.TTSuat.TSuat.Replace("%", "");
            drMT["TkCo"] = geTkCo.EditValue.ToString();
            drMT.EndEdit();
            return drMT;
        }
        private DataRow CreateDTRow(DataRow drMT, DataRow drDT, HHDVu_Minv hh)
        {

            drDT["MTID"] = drMT["MTID"];
            drDT["Sohoadon"] = drMT["Sohoadon"];
            drDT["MTIDDT"] = hh.id;
            drDT["MaKho"] = geMaKho.EditValue.ToString();
            drDT["TenVT"] = hh.ten;
            drDT["DVT"] = hh.dvtinh;
            if (hh.sluong == null) drDT["Soluong"] = 0;
            else drDT["Soluong"] = hh.sluong;
            //drDT["Soluong"] = (hh.SLuong==null)? 0: hh.SLuong;
            if (hh.dgia == null) drDT["DonGia"] = 0;
            else drDT["DonGia"] = hh.dgia;
            if (hh.tlckhau != null)
                drDT["TileCK"] = hh.tlckhau;
            if (hh.stckhau != null)
                drDT["CK"] = hh.stckhau;
            else
            {
                hh.stckhau = 0;
                drDT["CK"] = hh.stckhau;
            }
            hh.thtien = hh.thtien == null ? 0 : hh.thtien;
            drDT["TTien"] = hh.thtien;

            if (hh.ltsuat != null)
                drDT["MaThueCT"] = hh.ltsuat.Replace("%", "");
            else
                drDT["MaThueCT"] = "KCT";
            double Thuesuat = 0;
            if (hh.tsuat != null)
                Thuesuat = (double)hh.tsuat * 100;
            else
                Thuesuat = 0;
            drDT["Thuesuat"] = Thuesuat;




            drDT["TienThue"] = Math.Round((double)((hh.thtien - hh.stckhau) * Thuesuat / 100), 0);
            drDT["Tkgv"] = geTkCK.EditValue.ToString();
            drDT["Tkkho"] = geTkkho.EditValue.ToString();
            drDT["isDV"] = 0;
            drDT.EndEdit();
            return drDT;
        }
        private void fImportHDDauVao_Load(object sender, EventArgs e)
        {
            if (Config.Variables.Contains("MaSoThue")) this.Text += ": " + Config.GetValue("MaSoThue").ToString();

            //Lấy dữ liệu MT22
            _dataMt22 = new DataMasterDetail("DT22", "7");
            _dataMt22.ConditionMaster = "1=0";
            _dataMt22.GetData();
            bsMT22.DataSource = _dataMt22.DsData;
            bsMT22.DataMember = _dataMt22.DsData.Tables[0].TableName;
            _frmDesign = new FormDesigner(_dataMt22, bsMT22);
            dxErrorProviderMain.DataSource = bs;
            
            foreach(DataRow drCol in _dataMt22.DsStruct.Tables[0].Rows)
            {
                switch (drCol["FieldName"].ToString().ToLower())
                {
                    case "mahttt":
                        reHTTT = _frmDesign.GenRIGridLookupEdit(drCol);
                        reHTTT.DisplayMember = "MaHTTT";
                        break;
                    case "makh":
                        reDMKH = _frmDesign.GenRIGridLookupEdit(drCol);
                        reDMKH1 = _frmDesign.GenRIGridLookupEdit(drCol);
                        reDMKH1.DisplayMember = "TenKH";
                        break;
                    case "tkco":
                      reDMTK= _frmDesign.GenRIGridLookupEdit(drCol);
                        break;
                    case "mathue":
                        reMaThue = _frmDesign.GenRIGridLookupEdit(drCol);
                        break;
                }
            }
            foreach (DataRow drCol in _dataMt22.DsStruct.Tables[1].Rows)
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
            dbdmThueSuat = publicCDTData.findCDTData("DMThueSuat", "", "");
            if (dbdmThueSuat == null) dbdmThueSuat = new DataSingle("DMThueSuat", "7");
            DataFactory.publicCDTData.AddCDTData(dbdmThueSuat);
            if (!dbdmKH.FullData) dbdmKH.GetData();
            if (!dbdmVT.FullData) dbdmVT.GetData();
            if (!dbdmKho.FullData) dbdmKho.GetData();
            if (!dbdmDVT.FullData) dbdmDVT.GetData();
            if (!dbdmHTTT.FullData) dbdmHTTT.GetData();
            if (!dbdmTk.FullData) dbdmTk.GetData();
            reDMTK.Data.GetData();
            if (!dbdmThueSuat.FullData) dbdmThueSuat.GetData();

            //Thiết kế repository
            //reHTTT.DataSource = dbdmHTTT.DsData.Tables[0]; reHTTT.ValueMember = "MaHTTT"; reHTTT.DisplayMember = "MaHTTT";
            gCMaHTTT.ColumnEdit = reHTTT;

            //reDMKH.DataSource = dbdmKH.DsData.Tables[0]; reDMKH.ValueMember = "MaKH"; reDMKH.DisplayMember = "MaKH";
            gCMaKH.ColumnEdit = reDMKH;gCTenKH.ColumnEdit = reDMKH1;
            //reDMTK.DataSource = dbdmTk.DsData.Tables[0]; reDMTK.ValueMember = "TK"; reDMKH.DisplayMember = "TK";
            gCTkCo.ColumnEdit = reDMTK; gCTkDT.ColumnEdit = reDMTK; gCTkGV.ColumnEdit = reDMTK; gCTkKho.ColumnEdit = reDMTK; gCTkNo.ColumnEdit = reDMTK;
 
            //reDMVT.DataSource = dbdmVT.DsData.Tables[0]; reDMVT.ValueMember = "TK"; reDMVT.DisplayMember = "MaVT";
            gCMaVT.ColumnEdit = reDMVT; gCTenVT.ColumnEdit = reDMVT1;
            //reDMKHO.DataSource = dbdmKho.DsData.Tables[0]; reDMKHO.ValueMember = "MaKho"; reDMKHO.DisplayMember = "MaKho";
            gCMaKho.ColumnEdit = reDMKHO;
            //reDMDVT.DataSource = dbdmDVT.DsData.Tables[0]; reDMDVT.ValueMember = "MaDVT"; reDMDVT.DisplayMember = "MaDVT";
            gCMaDVT.ColumnEdit = reDMDVT;
            gcMaThue.ColumnEdit = reMaThue;
            
            geMaKho.Properties.DataSource = dbdmKho.DsData.Tables[0]; geMaKho.Properties.ValueMember = "MaKho"; geMaKho.Properties.DisplayMember = "TenKho"; geMaKho.EditValue = "HH";
            geTkCo.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkCo.Properties.ValueMember = "TK"; geTkCo.Properties.DisplayMember = "TK"; geTkCo.EditValue = "331";
            geTkTM.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkTM.Properties.ValueMember = "TK"; geTkTM.Properties.DisplayMember = "TK"; geTkTM.EditValue = "1111";
            geTkNH.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkNH.Properties.ValueMember = "TK"; geTkNH.Properties.DisplayMember = "TK"; geTkNH.EditValue = "11211";
            geTkNo.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkNo.Properties.ValueMember = "TK"; geTkNo.Properties.DisplayMember = "TK"; geTkNo.EditValue = "6421";

            geTkCK.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkCK.Properties.ValueMember = "TK"; geTkCK.Properties.DisplayMember = "TK"; geTkCK.EditValue = "632";
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
            reDMVT.EditValueChanging += ReDMVT_EditValueChanging;
            reDMDVT.KeyUp += ReDMDVT_KeyUp;
            tbDT.ColumnChanged += TbDT_ColumnChanged;
            tbDT.ColumnChanging += TbDT_ColumnChanging;
            DictionaryName = new DataSingle("DictionaryName", "7");
            DictionaryName.GetData();
            gridControl2.KeyUp += GridControl2_KeyUp;
            gridView1.CustomDrawRowIndicator += GridView1_CustomDrawRowIndicator;
            gridView2.CustomDrawRowIndicator += GridView1_CustomDrawRowIndicator;
            gridView1.IndicatorWidth = 40;
            gridView2.IndicatorWidth = 40;

            rEisDV.MouseUp += REisDV_MouseUp;
        }

        private void ReDMVT_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue != null && e.OldValue == DBNull.Value)
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                string Code = e.NewValue.ToString();
                string Name = dr["TenVT"].ToString();
                string TableName = "DMVT";
                DataRow[] ldr = DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'");
                if (ldr.Length == 0)
                {
                    DataRow drDic = DictionaryName.DsData.Tables[0].NewRow();
                    drDic["Code"] = Code; drDic["Name"] = Name; drDic["TableName"] = TableName;
                    DictionaryName.DsData.Tables[0].Rows.Add(drDic);
                    DictionaryName.UpdateData(DataAction.Insert);
                }
                else
                {
                    ldr[0]["Code"] = Code; ldr[0]["Name"] = Name; ldr[0]["TableName"] = TableName;
                    DictionaryName.DrCurrentMaster = ldr[0];
                    DictionaryName.UpdateData(DataAction.Update);
                }
                tbDT.ColumnChanging += TbDT_ColumnChanging;
            }
            if (e.NewValue == null && e.OldValue != DBNull.Value)
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                string Name = dr["TenVT"].ToString();
                string TableName = "DMVT";
                DataRow[] ldr = DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'");
                if (ldr.Length >= 0)
                {

                    DictionaryName.DrCurrentMaster = ldr[0];
                    DictionaryName.DrCurrentMaster.Delete();
                    DictionaryName.UpdateData(DataAction.Delete);
                }
            }
        }

        private void TbDT_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            //if (e.ProposedValue != null && e.Column.ColumnName.ToLower() == "mavt" && e.Row["MaVT"] != DBNull.Value)
            //{
            //    e.Row.BeginEdit();
            //    tbDT.ColumnChanging -= TbDT_ColumnChanging;
            //    e.Row["MaVT"] = e.ProposedValue;
            //    e.Row.EndEdit();
            //    string Code = e.ProposedValue.ToString();
            //    string Name = e.Row["TenVT"].ToString();
            //    string TableName = "DMVT";
            //    DataRow[] ldr = DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'");
            //    if (ldr.Length == 0)
            //    {
            //        DataRow drDic = DictionaryName.DsData.Tables[0].NewRow();
            //        drDic["Code"] = Code; drDic["Name"] = Name; drDic["TableName"] = TableName;
            //        DictionaryName.DsData.Tables[0].Rows.Add(drDic);
            //        DictionaryName.UpdateData(DataAction.Insert);
            //    }
            //    else
            //    {
            //        ldr[0]["Code"] = Code; ldr[0]["Name"] = Name; ldr[0]["TableName"] = TableName;
            //        DictionaryName.DrCurrentMaster = ldr[0];
            //        DictionaryName.UpdateData(DataAction.Update);
            //    }
            //    tbDT.ColumnChanging += TbDT_ColumnChanging;
            //}
        }

        private void TbDT_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName.ToLower() == "mavt" && e.Row["MaVT"] != DBNull.Value)
            {
                string mavt = e.Row["MaVT"].ToString();
                DataRow[] lstMaVT = dbdmVT.DsData.Tables[0].Select("MaVT='" + mavt + "'");
                if (lstMaVT.Length > 0)
                {
                    e.Row["MaDVT"] = lstMaVT[0]["MaDVT"];
                    e.Row["TkKho"] = lstMaVT[0]["TkKho"];
                    e.Row["TkDthu"] = lstMaVT[0]["TkDT"];
                    e.Row["TkGV"] = lstMaVT[0]["TkGV"];
                    e.Row["isDV"] = lstMaVT[0]["isDV"];
                }
            }
        }

       

        private void REisDV_MouseUp(object sender, MouseEventArgs e)
        {

            if ((sender as CheckEdit).Checked)
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                if (dr == null) return;
                dr["isDV"] = (sender as CheckEdit).Checked;
                dr.EndEdit();
                string Name = dr["TenVT"].ToString();
                string TableName = "DMVT";
                DataRow[] ldr = DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'");
                if (ldr.Length == 0)
                {
                    DataRow drDic = DictionaryName.DsData.Tables[0].NewRow();

                    drDic["Code"] = dr["MaVT"]; drDic["Name"] = Name; drDic["TableName"] = TableName; drDic["isDV"] = 1;
                    DictionaryName.DsData.Tables[0].Rows.Add(drDic);
                    DictionaryName.UpdateData(DataAction.Insert);
                }
                else
                {
                    DictionaryName.DrCurrentMaster = ldr[0];
                    DictionaryName.UpdateData(DataAction.Update);
                }
            }
            else
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                if (dr == null) return;
                dr["isDV"] = (sender as CheckEdit).Checked;
                dr.EndEdit();
                string Name = dr["TenVT"].ToString();
                string TableName = "DMVT";
                DataRow[] ldr = DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'");
                if (ldr.Length == 1)
                {
                    DictionaryName.DrCurrentMaster = ldr[0];
                    DictionaryName.DrCurrentMaster.Delete();
                    DictionaryName.UpdateData(DataAction.Delete);
                }

            }
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
                DataRow[] ldr = DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'");
                if (ldr.Length == 0)
                {
                    DataRow drDic = DictionaryName.DsData.Tables[0].NewRow();
                    drDic["Code"] = Code; drDic["Name"] = Name; drDic["TableName"] = TableName;
                    DictionaryName.DsData.Tables[0].Rows.Add(drDic);
                    DictionaryName.UpdateData(DataAction.Insert);
                }
                else
                {
                    ldr[0]["Code"] = Code; ldr[0]["Name"] = Name; ldr[0]["TableName"] = TableName;
                    DictionaryName.UpdateData(DataAction.Update);
                }
            }
        }

        private void GridControl2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                if (gridControl2.DataMember != "")
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
        }
        private void ReDMVT_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                string Code = "";
                if ((sender as GridLookUpEdit).EditValue == null)
                    Code = null;
                else Code = (sender as GridLookUpEdit).EditValue.ToString();
                string Name = dr["TenVT"].ToString();
                string TableName = "DMVT";
                DataRow[] ldr = DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'");
                if (ldr.Length == 0)
                {
                    DataRow drDic = DictionaryName.DsData.Tables[0].NewRow();
                    drDic["Code"] = Code; drDic["Name"] = Name; drDic["TableName"] = TableName; drDic["isDV"] = 0;
                    DictionaryName.DsData.Tables[0].Rows.Add(drDic);
                    DictionaryName.UpdateData(DataAction.Insert);
                }
                else
                {
                    ldr[0]["Code"] = Code; ldr[0]["Name"] = Name; ldr[0]["TableName"] = TableName;
                    DictionaryName.UpdateData(DataAction.Update);
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
                DataRow[] ldr = DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'");
                if (ldr.Length == 0)
                {
                    DataRow drDic = DictionaryName.DsData.Tables[0].NewRow();
                    drDic["Code"] = Code; drDic["Name"] = Name; drDic["TableName"] = TableName;
                    DictionaryName.DsData.Tables[0].Rows.Add(drDic);
                    DictionaryName.UpdateData(DataAction.Insert);
                }
                else
                {
                    ldr[0]["Code"] = Code; ldr[0]["Name"] = Name; ldr[0]["TableName"] = TableName;
                    DictionaryName.UpdateData(DataAction.Update);
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
                DataRow[] ldr = DictionaryName.DsData.Tables[0].Select("TableName='" + TableName + "' and Name='" + Name + "'");
                if (ldr.Length == 0)
                {
                    DataRow drDic = DictionaryName.DsData.Tables[0].NewRow();
                    drDic["Code"] = Code; drDic["Name"] = Name; drDic["TableName"] = TableName;
                    DictionaryName.DsData.Tables[0].Rows.Add(drDic);
                    DictionaryName.UpdateData(DataAction.Insert);
                }
                else
                {
                    ldr[0]["Code"] = Code; ldr[0]["Name"] = Name; ldr[0]["TableName"] = TableName;
                    DictionaryName.UpdateData(DataAction.Update);
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
            DictionaryName.GetData();
        }

        private void BtCheckMaKH_Click(object sender, EventArgs e)
        {
            string[] arrHTTT = ThietKedulieu.ConvertDataTableToArray(dbdmKH.DsData.Tables[0], "TenKH");

            foreach (DataRow drMT in tbMT.Rows)
            {
                DataRow[] lstRows = dbdmKH.DsData.Tables[0].Select("TenKH='" + drMT["TenKH"].ToString() + "'");
                if (lstRows.Length > 0)
                {
                    drMT["MaKH"] = lstRows[0]["MaKH"];
                    drMT.EndEdit();
                    if (drMT["MaKH"] != DBNull.Value)
                    {
                        DataRow[] ldr = dbdmKH.DsData.Tables[0].Select("MaKH='" + drMT["MaKH"].ToString() + "'");
                        if (ldr.Length == 0) drMT["MaKH"] = DBNull.Value;
                        drMT.EndEdit();
                    }
                    continue;
                }
                else
                {
                    lstRows = DictionaryName.DsData.Tables[0].Select("Name='" + drMT["TenKH"].ToString() + "' and TableName='DMKH'");
                    if (lstRows.Length > 0)
                    {
                        drMT["MaKH"] = lstRows[0]["Code"];
                        drMT.EndEdit();
                        if (drMT["MaKH"] != DBNull.Value)
                        {
                            DataRow[] ldr = dbdmKH.DsData.Tables[0].Select("MaKH='" + drMT["MaKH"].ToString() + "'");
                            if (ldr.Length == 0) drMT["MaKH"] = DBNull.Value;
                            drMT.EndEdit();
                        }
                        continue;
                    }
                }
                int j = ThietKedulieu.LevantEdit(arrHTTT, drMT["TenKH"].ToString());
                if (j >= 0 && j < arrHTTT.Length)
                {
                    if (dbdmKH.DsData.Tables[0].Rows[j]["MST"] == DBNull.Value || dbdmKH.DsData.Tables[0].Rows[j]["MST"].ToString().Trim() == "" || dbdmKH.DsData.Tables[0].Rows[j]["MST"].ToString().Trim() == drMT["MST"].ToString().Trim())
                    {
                        drMT["MaKH"] = dbdmKH.DsData.Tables[0].Rows[j]["MaKH"];
                        drMT.EndEdit();
                    }
                }
                else
                {
                    int j1 = ThietKedulieu.FindClosestString(arrHTTT, drMT["TenKH"].ToString().ToLower());

                    if (j1 >= 0 && j1 < arrHTTT.Length)
                    {
                        if (dbdmKH.DsData.Tables[0].Rows[j1]["MST"] == DBNull.Value || dbdmKH.DsData.Tables[0].Rows[j1]["MST"].ToString().Trim() == "" || dbdmKH.DsData.Tables[0].Rows[j1]["MST"].ToString().Trim() == drMT["MST"].ToString().Trim())
                        {
                            drMT["MaKH"] = dbdmKH.DsData.Tables[0].Rows[j1]["MaKH"];
                            drMT.EndEdit();
                        }
                    }

                }
                if (drMT["MaKH"] != DBNull.Value)
                {
                    DataRow[] ldr = dbdmKH.DsData.Tables[0].Select("MaKH='" + drMT["MaKH"].ToString() + "'");
                    if (ldr.Length == 0) drMT["MaKH"] = DBNull.Value;
                    drMT.EndEdit();
                }
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
                    drKH["Doitac"] = drMT["TenKH"];// drMT["Ongba"];
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
            string[] arrVT = ThietKedulieu.ConvertDataTableToArray(dbdmVT.DsData.Tables[0], "TenVT");

            DataRow[] lstRowsDic = DictionaryName.DsData.Tables[0].Select("TableName='DMVT'");//Name='" + drDT["TenVT"].ToString() + "' and 
            string[] arrVTDic = ThietKedulieu.ConvertDataRowsToArray(lstRowsDic, "Name");
            double dic2;
            double dic1;
            foreach (DataRow drDT in tbDT.Rows)
            {
                check1VT(drDT, arrVTDic, arrVT, lstRowsDic);
                if (drDT["MaVT"] != DBNull.Value)
                {
                    DataRow[] ldr = dbdmVT.DsData.Tables[0].Select("MaVT='" + drDT["MaVT"].ToString() + "'");
                    if (ldr.Length == 0) drDT["MaVT"] = DBNull.Value;
                    drDT.EndEdit();
                }

            }
        }

        private void check1VT(DataRow drDT, string[] arrVTDic, string[] arrVT, DataRow[] lstRowsDic)
        {
            double dic2;
            double dic1;
            string TenVT = drDT["TenVT"].ToString().ToLower();


            //Xử lý dấu ngoặc
            TenVT = ThietKedulieu.RemoveTextBetweenParentheses(TenVT).Trim().Replace("'", "");
            DataRow[] lstRows = dbdmVT.DsData.Tables[0].Select("TenVT='" + TenVT + "'");// Tìm chính xác trên danh mục
            if (lstRows.Length > 0)
            {
                drDT["MaVT"] = lstRows[0]["MaVT"];
                drDT.EndEdit();
                return;
            }
            else
            {
                if (Array.IndexOf(arrVTDic, TenVT) >= 0)
                {
                    DataRow[] ldrdic = DictionaryName.DsData.Tables[0].Select("Name='" + TenVT + "'");
                    if (ldrdic.Length > 0)
                    {
                        DataRow drDic = ldrdic[0];
                        {
                            if (drDic["Code"] == DBNull.Value && drDic["isDV"].ToString() == "True")
                            {
                                drDT["isDV"] = drDic["isDV"];
                            }
                            else
                            {
                                drDT["MaVT"] = drDic["Code"];
                            }
                            drDT.EndEdit();
                            return;
                        }
                    }
                }
            }
            int j2 = ThietKedulieu.LevantEditWithWeightHasDic(arrVTDic, TenVT, 1000, out dic2); ; //Tìm khoảng cách trên Dic
            int j1 = ThietKedulieu.LevantEditWithWeightHasDic(arrVT, TenVT, 1000, out dic1);//Khoảng cách trên vật tư
            if (dic2 <= dic1)//lấy trên mã 
            {
                if (j2 >= 0 && j2 < arrVTDic.Length)
                {

                    drDT["MaVT"] = lstRowsDic[j2]["Code"];
                    drDT["dictance"] = dic2;
                    // drDT["isDV"] = lstRowsDic[j2]["isDV"];
                    drDT.EndEdit();
                    return;
                }
                else
                {
                    drDT["dictance"] = dic2;
                    drDT.EndEdit();
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
                else
                {
                    drDT["dictance"] = dic1;
                    drDT.EndEdit();
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
                if(drv["sohoadon"].ToString()== "3032")
                {

                }
                if (drv["KieuHD"] == DBNull.Value)
                {
                    MessageBox.Show("Chưa phân loại hóa đơn nên không phân biệt được dịch vụ hay hàng hóa!");
                    break;
                }
                if (drv["KieuHD"] != DBNull.Value && drv["KieuHD"].ToString() != "0")
                {
                    bs.MoveNext();
                    idx++;
                    continue;
                }

                foreach (DataRow drDT in lstDT)
                {
                    if(drDT["TenVT"].ToString()== "QUẠT MÀU NGOẠI THẤT - 576 MÀU")
                    {

                    }
                    if (drDT["MaVT"] == DBNull.Value || drDT["MaVT"].ToString() == string.Empty)
                    {
                        if (bool.Parse(drDT["isDV"].ToString()))
                        {
                            continue;
                        }
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
                            MessageBox.Show("Vật tư " + TenVT + " có lỗi");
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
                            else
                            {
                                MessageBox.Show("Vật tư " + TenVT + " có lỗi");
                                bs.MoveLast();
                                idx = bs.Count;
                                break;
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
        private void btPhanloai_Click(object sender, EventArgs e)
        {
            foreach (DataRow drMt in tbMT.Rows)
            {
                if (drMt["MaHTTT"] == DBNull.Value) continue;
                DataRow[] lstRowHH = tbDT.Select("isDV=0 and MTID='" + drMt["MTID"].ToString() +"'");
                if (lstRowHH.Length == 0)// Không có dòng hàng hóa nào chắc chắn phải vào hóa đơn dịch vụ
                {
                    if (drMt["MaHTTT"].ToString() == "TM")//Chắc chắn là phiếu chi
                    {
                        drMt["KieuHD"] = 2;
                        drMt["TKCo"] = geTkTM.EditValue.ToString();
                        drMt["TKNo"] = geTkNo.EditValue.ToString();
                    }
                    else if (drMt["MaHTTT"].ToString() == "CK")//Chắc chắn là chuyển khoản trực tiếp
                    {
                        drMt["KieuHD"] = 3;
                        drMt["TKCo"] = geTkNH.EditValue.ToString();
                        drMt["TKNo"] = geTkNo.EditValue.ToString();
                    }
                    else if (drMt["MaHTTT"].ToString() == "CN")//Chắc chắn phải nhập vào mua dịch vụ
                    {
                        drMt["KieuHD"] = 1;
                        drMt["TKCo"] = geTkCo.EditValue.ToString();
                        drMt["TKNo"] = geTkNo.EditValue.ToString();
                    }
                    else
                    {
                        drMt["KieuHD"] = 1;
                    }
                }
                else drMt["KieuHD"] = 0;
            }
        }

        private bool InsertMT22(DataRowView drv)
        {
            _dataMt22.LstDrCurrentDetails.Clear();
            _dataMt22._lstCurRowDetail.Clear();
            if (_dataMt22.DsData.Tables[0].Select("MT22ID='" + drv["MTID"].ToString() + "'").Length > 0)
            {

                return true;
            }
            string sql = "select count(*) from mt22 where mccqt='" + drv["MCCQT"].ToString() + "'";
            object i = _dataMt22.DbData.GetValue(sql);
            if (i != null && int.Parse(i.ToString()) > 0)
            {
                 return true;
            }
            bsMT22.AddNew();
            bsMT22.EndEdit();
            bsMT22.MoveLast();
            DataRowView drMT = (bsMT22.Current as DataRowView);
            DataRow[] lstDT = tbDT.Select("MTID='" + drv["MTID"].ToString() + "'");
            if (drMT != null)
                _dataMt22.DrCurrentMaster = drMT.Row;
            importMT22Row(_dataMt22.DrCurrentMaster, drv.Row);
            foreach (DataRow drDT in lstDT)
            {
                DataRow drDT22 = _dataMt22.DsData.Tables[1].NewRow();

                if (drDT22.RowState == DataRowState.Detached)
                    _dataMt22.DsData.Tables[1].Rows.Add(drDT22);
                importDT22Row(drDT22, drDT);
                drDT22.EndEdit();
            }
            //Thêm dòng thuế VAT vào hóa đơn đầu vào
            if (_dataMt22.DsData.Tables.Count > 2)
            {
                DataRow drVat = _dataMt22.DsData.Tables[2].NewRow();
                if (drVat.RowState == DataRowState.Detached)
                    _dataMt22.DsData.Tables[2].Rows.Add(drVat);
                importVatInRowMT22(drVat, drv.Row);
                drVat.EndEdit();
            }
            _dataMt22.CheckRules(DataAction.Insert);
            if (_dataMt22.DsData.HasErrors)
            {
                foreach (DataColumn col in _dataMt22.DsData.Tables[0].Columns)
                {
                    string err = _dataMt22.DrCurrentMaster.GetColumnError(col);
                    if (err != string.Empty)
                    {
                        MessageBox.Show("Cột " + col.ColumnName + " Có lỗi: " + err);
                        _dataMt22.CancelUpdate();
                        return false;

                    }
                }
                foreach (DataColumn col in _dataMt22.DsData.Tables[1].Columns)
                {
                    foreach (DataRow drCurrentDT in _dataMt22.LstDrCurrentDetails)
                    {
                        string err = drCurrentDT.GetColumnError(col.ColumnName);
                        if (err != string.Empty)
                        {
                            MessageBox.Show("Cột " + col.ColumnName + " Có lỗi");
                            _dataMt22.CancelUpdate();
                            return false;

                        }
                    }

                }
                _dataMt22.CancelUpdate();
                return false;
            }
            else
            {

                if (_dataMt22.UpdateData(DataAction.Insert))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Thêm vào hóa đơn bị lỗi. Hóa đơn số :" + drv["SoHoaDon"].ToString());
                    return false;
                }
            }

            return true;
        }
        private bool InsertMT21(DataRowView drv)
        {
            _dataMt21.LstDrCurrentDetails.Clear();
            _dataMt21._lstCurRowDetail.Clear();
            if (_dataMt21.DsData.Tables[0].Select("MT21ID='" + drv["MTID"].ToString() + "'").Length > 0)
            {
                return true;
            }
            string sql = "select count(*) from mt21 where mccqt='" + drv["MCCQT"].ToString() + "'";
            object i = _dataMt21.DbData.GetValue(sql);
            if (i != null && int.Parse(i.ToString()) > 0)
            {
                return true;
            }
            bsMT21.AddNew();
            bsMT21.EndEdit();
            bsMT21.MoveLast();
            DataRowView drMT = (bsMT21.Current as DataRowView);
            DataRow[] lstDT = tbDT.Select("MTID='" + drv["MTID"].ToString() + "'");
            if (drMT != null)
                _dataMt21.DrCurrentMaster = drMT.Row;
            importMT21Row(_dataMt21.DrCurrentMaster, drv.Row);

            foreach (DataRow drDT in lstDT)
            {
                DataRow drDT21 = _dataMt21.DsData.Tables[1].NewRow();
                if (drDT21.RowState == DataRowState.Detached)
                    _dataMt21.DsData.Tables[1].Rows.Add(drDT21);
                importDT21Row(drDT21, drDT);
                drDT21.EndEdit();
            }
            _dataMt21.CheckRules(DataAction.Insert);
            if (_dataMt21.DsData.HasErrors)
            {
                foreach (DataColumn col in _dataMt21.DsData.Tables[0].Columns)
                {
                    if (_dataMt21.DrCurrentMaster.GetColumnError(col) != string.Empty)
                    {
                        MessageBox.Show("Cột " + col.ColumnName + "Có lỗi");
                        _dataMt21.CancelUpdate();
                        return false;

                    }
                }
                foreach (DataColumn col in _dataMt21.DsData.Tables[1].Columns)
                {
                    foreach (DataRow drCurrentDT in _dataMt21.LstDrCurrentDetails)
                    {
                        if (drCurrentDT.GetColumnError(col.ColumnName) != string.Empty)
                        {
                            MessageBox.Show("Cột " + col.ColumnName + "Có lỗi");
                            _dataMt21.CancelUpdate();
                            return false;

                        }
                    }

                }
                _dataMt21.CancelUpdate();
                return false;
            }
            else
            {

                if (_dataMt21.UpdateData(DataAction.Insert))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Thêm vào hóa đơn bị lỗi. Hóa đơn số :" + drv["SoHoaDon"].ToString());
                    return false;
                }
            }
            return true;
        }
        private bool InsertMT12(DataRowView drv)
        {
            _dataMt12.LstDrCurrentDetails.Clear();
            _dataMt12._lstCurRowDetail.Clear();

            if (_dataMt12.DsData.Tables[0].Select("MT12ID='" + drv["MTID"].ToString() + "'").Length > 0)
            {
                return true;
            }
            string sql = "select count(*) from mt12 where mccqt='" + drv["MCCQT"].ToString() + "'";
            object i = _dataMt12.DbData.GetValue(sql);
            if (i != null && int.Parse(i.ToString()) > 0)
            {
                return true;
            }
            bsMT12.AddNew();
            bsMT12.EndEdit();
            bsMT12.MoveLast();
            DataRowView drMT = (bsMT12.Current as DataRowView);
            DataRow[] lstDT = tbDT.Select("MTID='" + drv["MTID"].ToString() + "'");
            if (drMT != null)
                _dataMt12.DrCurrentMaster = drMT.Row;
            importMT12Row(_dataMt12.DrCurrentMaster, drv.Row);
            DataRow drDT12 = _dataMt12.DsData.Tables[1].NewRow();
            if (drDT12.RowState == DataRowState.Detached)
                _dataMt12.DsData.Tables[1].Rows.Add(drDT12);
            importDT12Row(drDT12, drv);
            drDT12.EndEdit();

            //Thêm dòng thuế VAT vào hóa đơn đầu vào
            if (_dataMt12.DsData.Tables.Count > 2)
            {
                DataRow drVat = _dataMt12.DsData.Tables[2].NewRow();
                if (drVat.RowState == DataRowState.Detached)
                    _dataMt12.DsData.Tables[2].Rows.Add(drVat);
                importVatInRowMT12(drVat, drv.Row);
                drVat.EndEdit();
            }
            _dataMt12.CheckRules(DataAction.Insert);
            if (_dataMt12.DsData.HasErrors)
            {
                foreach (DataColumn col in _dataMt12.DsData.Tables[0].Columns)
                {
                    string err = _dataMt12.DrCurrentMaster.GetColumnError(col);
                    if (err != string.Empty)
                    {
                        MessageBox.Show("Cột " + col.ColumnName + " Có lỗi: " + err);
                        _dataMt12.CancelUpdate();
                        return false;

                    }
                }
                foreach (DataColumn col in _dataMt12.DsData.Tables[1].Columns)
                {
                    foreach (DataRow drCurrentDT in _dataMt12.LstDrCurrentDetails)
                    {
                        string err = drCurrentDT.GetColumnError(col.ColumnName);
                        if (err != string.Empty)
                        {
                            MessageBox.Show("Cột " + col.ColumnName + " Có lỗi");
                            _dataMt12.CancelUpdate();
                            return false;

                        }
                    }

                }
                _dataMt12.CancelUpdate();
                return false;
            }
            else
            {

                if (_dataMt12.UpdateData(DataAction.Insert))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Thêm vào hóa đơn bị lỗi. Hóa đơn số :" + drv["SoHoaDon"].ToString());
                    return false;
                }
            }

            return false;
        }
        private bool InsertMT16(DataRowView drv)
        {
            _dataMt16.LstDrCurrentDetails.Clear();
            _dataMt16._lstCurRowDetail.Clear();
            if (_dataMt16.DsData.Tables[0].Select("MT16ID='" + drv["MTID"].ToString() + "'").Length > 0)
            {
                return true;
            }
            string sql = "select count(*) from mt16 where mccqt='" + drv["MCCQT"].ToString() + "'";
            object i = _dataMt16.DbData.GetValue(sql);
            if (i != null && int.Parse(i.ToString()) > 0)
            {
                return true;
            }

            bsMT16.AddNew();
            bsMT16.EndEdit();
            bsMT16.MoveLast();
            DataRowView drMT = (bsMT16.Current as DataRowView);
            DataRow[] lstDT = tbDT.Select("MTID='" + drv["MTID"].ToString() + "'");
            if (drMT != null)
                _dataMt16.DrCurrentMaster = drMT.Row;
            importMT16Row(_dataMt16.DrCurrentMaster, drv.Row);
            DataRow drDT16 = _dataMt16.DsData.Tables[1].NewRow();
            if (drDT16.RowState == DataRowState.Detached)
                _dataMt16.DsData.Tables[1].Rows.Add(drDT16);
            importDT16Row(drDT16, drv);
            drDT16.EndEdit();

            //Thêm dòng thuế VAT vào hóa đơn đầu vào
            if (_dataMt16.DsData.Tables.Count > 2)
            {
                DataRow drVat = _dataMt16.DsData.Tables[2].NewRow();
                if (drVat.RowState == DataRowState.Detached)
                    _dataMt16.DsData.Tables[2].Rows.Add(drVat);
                importVatInRowMT16(drVat, drv.Row);
                drVat.EndEdit();
            }
            _dataMt16.CheckRules(DataAction.Insert);
            //_dataMt16.DsData.HasErrors = false;
            if (_dataMt16.DsData.HasErrors)
            {
                foreach (DataColumn col in _dataMt16.DsData.Tables[0].Columns)
                {
                    string err = _dataMt16.DrCurrentMaster.GetColumnError(col);
                    if (err != string.Empty)
                    {
                        MessageBox.Show("Cột " + col.ColumnName + " Có lỗi: " + err);
                        _dataMt16.CancelUpdate();
                        return false;

                    }
                }
                foreach (DataColumn col in _dataMt16.DsData.Tables[1].Columns)
                {
                    foreach (DataRow drCurrentDT in _dataMt16.LstDrCurrentDetails)
                    {
                        string err = drCurrentDT.GetColumnError(col.ColumnName);
                        if (err != string.Empty)
                        {
                            MessageBox.Show("Cột " + col.ColumnName + " Có lỗi");
                            _dataMt16.CancelUpdate();
                            return false;

                        }
                    }

                }
                _dataMt16.CancelUpdate();
                return false;
            }
            else
            {

                if (_dataMt16.UpdateData(DataAction.Insert))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Thêm vào hóa đơn bị lỗi. Hóa đơn số :" + drv["SoHoaDon"].ToString());
                    _dataMt16.CancelUpdate();
                    return false;
                }
            }

            return false;
        }

        private async void InserttoData()
        {
            bs.MoveFirst();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = bs.Count;
            progressBar1.Step = 1;
            progressBar1.Value = 0;
            bsMT22.DataSource = _dataMt22.DsData;
            bsMT22.DataMember = _dataMt22.DsData.Tables[0].TableName;
            bsMT21.DataSource = _dataMt21.DsData;
            bsMT21.DataMember = _dataMt21.DsData.Tables[0].TableName;

            bsMT12.DataSource = _dataMt12.DsData;
            bsMT12.DataMember = _dataMt12.DsData.Tables[0].TableName;
            bsMT16.DataSource = _dataMt16.DsData;
            bsMT16.DataMember = _dataMt16.DsData.Tables[0].TableName;

            bool HasErr = false;
            for (int idx = 0; idx < bs.Count; idx++)
            {
                if (HasErr) break;
                

                DateTime t1 = DateTime.Now;
                UpdateProgressBar(bs.Position + 1);
                await Task.Delay(1);
                

                DataRowView drv = (bs.Current as DataRowView);
               
                if (drv["KieuHD"] == DBNull.Value) break;

                switch (drv["KieuHD"].ToString())
                {
                    case "0":// Mua hàng
                        HasErr = !InsertMT22(drv);
                        if (HasErr) _dataMt22.DsData.RejectChanges();
                        break;
                    case "1": // Mua dịch vụ
                        HasErr = !InsertMT21(drv);
                        if (HasErr) _dataMt21.DsData.RejectChanges();
                        break;
                    case "2":// Phiếu Chi
                        HasErr = !InsertMT12(drv);
                        if (HasErr) _dataMt12.DsData.RejectChanges();
                        break;
                    case "3": // Hóa đơn dịch vụ
                        HasErr = !InsertMT16(drv);
                        if (HasErr) _dataMt16.DsData.RejectChanges();
                        break;
                }
                bs.MoveNext();
                DateTime t2 = DateTime.Now;
                double tPro = (t2 - t1).TotalSeconds;
               
            }
            if (!HasErr) MessageBox.Show("Đã hoàn thành import dữ liệu");
        }
        private void btThemHoaDon_Click(object sender, EventArgs e)
        {
            
            progressBar1.Minimum = 0;
            progressBar1.Maximum = bs.Count;
            progressBar1.Step = 1;
            progressBar1.Value = 0;
            InserttoData();
        }
        private void UpdateProgressBar(int value)
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
        private void importDT22Row(DataRow drDT22, DataRow drDT)
        {
            drDT22["MT22ID"] = drDT["MTID"];
            drDT22["DT22ID"] = drDT["MTIDDT"];
            drDT22["MaKho"] = drDT["MaKho"];
            drDT22["MaVT"] = drDT["MaVT"];
            drDT22["Soluong"] = drDT["Soluong"];
            drDT22["MaThueCT"] = drDT["MaThueCT"];
            double thuesuat = 0;
            if (double.TryParse(drDT["Thuesuat"].ToString().Replace("%", ""), out thuesuat))
                drDT22["Thuesuat"] = thuesuat;
            else drDT22["Thuesuat"] = 0;

            drDT22["GiaNT"] = drDT["DonGia"];
            drDT22["PsNT"] = drDT["TTien"];
            if (drDT["TileCK"] != DBNull.Value)
                drDT22["TileCK"] = drDT["TileCK"].ToString().Replace("%", "");
            double ck = 0;
            if (double.TryParse(drDT["CK"].ToString().Replace("%", ""), out ck))
                drDT22["CK"] = ck;
            else drDT22["CK"] = 0;
            
            
            //drDT["TienThue"] = Math.Round((hh.ThTien - hh.STCKhau) * double.Parse(hh.TSuat.Replace("%", "")) / 100, 0);

            //drDT32["TkNo"] = geTkkho.EditValue.ToString();
            //if(drDT["Soluong"]==DBNull.Value || drDT["Soluong"].ToString() == "0")
            //{
            //    drDT22["isDV"] = 1;
            //}
            //else
            //    drDT22["isDV"] = 0;
            drDT22.EndEdit();
        }

        private void importMT22Row(DataRow drCurrentMaster, DataRow row)
        {
            drCurrentMaster["MCCQT"] = row["MCCQT"];
            drCurrentMaster["MT22ID"] = row["MTID"];
            drCurrentMaster["MaCT"] = "PNM";
            drCurrentMaster["NgayCT"]=row["Ngayhd"];
            drCurrentMaster["NgayHD"] = row["Ngayhd"];
            drCurrentMaster["SoHoadon"] = row["SoHoadon"];
            drCurrentMaster["Soseri"] = row["kyhieu"];
            drCurrentMaster["MaHTTT"] = row["MaHTTT"];
            drCurrentMaster["MaKH"] = row["MaKH"];
            //drCurrentMaster["MST"] = row["MST"];
            drCurrentMaster["Ongba"] = row["Ongba"];
            drCurrentMaster["DiaChi"] = row["DiaChi"];
            drCurrentMaster["MaNT"] = "VND";
            drCurrentMaster["TyGia"] = 1;
            drCurrentMaster["DienGiai"] = "Mua hàng nhập kho";
            drCurrentMaster["TkCo"] = row["TkCo"];
            drCurrentMaster["MaThueMT"] = row["MaThue"];
            drCurrentMaster["TkCK"] = "711";
            
            drCurrentMaster["TkThue"] = "1331";
            drCurrentMaster["PrintIndex"] =0;
            drCurrentMaster.EndEdit();

        }
        private void importDT21Row(DataRow drDT21, DataRow drDT)
        {
            drDT21["MT21ID"] = drDT["MTID"];
            drDT21["DT21ID"] = drDT["MTIDDT"];

            drDT21["DiengiaiCT"] = drDT["TenVT"];
            drDT21["MaThueCT"] = drDT["MaThueCT"];
            double thuesuat = 0;
            if (double.TryParse(drDT["Thuesuat"].ToString().Replace("%", ""), out thuesuat))
                drDT21["Thuesuat"] = thuesuat;
            else drDT21["Thuesuat"] = 0;
            double psnt = double.Parse(drDT["TTien"].ToString());
            drDT21["PsNT"] =psnt;

            drDT21["ThueNT"] = Math.Round(psnt*thuesuat / 100, 0);
            if (drDT["TkKho"] == DBNull.Value && (bsMT21.Current as DataRowView)!=null)
            {
                drDT21["TkNo"] = (bsMT21.Current as DataRowView)["TkNo"];
            }
            else
                drDT21["TkNo"] = drDT["TkKho"];

            drDT21.EndEdit();
        }

        private void importMT21Row(DataRow drCurrentMaster, DataRow row)
        {
            drCurrentMaster["MCCQT"] = row["MCCQT"];
            drCurrentMaster["MT21ID"] = row["MTID"];
            drCurrentMaster["MaCT"] = "MDV";
            drCurrentMaster["NgayCT"] = row["Ngayhd"];
            drCurrentMaster["NgayHD"] = row["Ngayhd"];
            drCurrentMaster["SoHoadon"] = row["SoHoadon"];
            drCurrentMaster["Soseri"] = row["kyhieu"];
            //drCurrentMaster["MaHTTT"] = row["MaHTTT"];
            drCurrentMaster["MaKH"] = row["MaKH"];
            //drCurrentMaster["MST"] = row["MST"];
            drCurrentMaster["Ongba"] = row["Ongba"];
            drCurrentMaster["DiaChi"] = row["DiaChi"];
            drCurrentMaster["MaNT"] = "VND";
            drCurrentMaster["TyGia"] = 1;
            drCurrentMaster["DienGiai"] = "Mua dịch vụ";
            drCurrentMaster["TkCo"] = row["TkCo"];
            drCurrentMaster["MaThue"] = row["MaThue"];
            //drCurrentMaster["TkCK"] = "5211";

            drCurrentMaster["MaThue"] = "10";
            drCurrentMaster["TkThue"] = "1331";
            drCurrentMaster.EndEdit();
        }

        private void importMT12Row(DataRow drCurrentMaster, DataRow row)
        {
            drCurrentMaster["MCCQT"] = row["MCCQT"];
            drCurrentMaster["MT12ID"] = row["MTID"];
            drCurrentMaster["MaCT"] = "PC";
            drCurrentMaster["NgayCT"] = row["Ngayhd"];
            drCurrentMaster["MaKH"] = row["MaKH"];

            drCurrentMaster["Ongba"] = row["Ongba"];
            drCurrentMaster["DiaChi"] = row["DiaChi"];
            drCurrentMaster["MaNT"] = "VND";
            drCurrentMaster["TyGia"] = 1;
            drCurrentMaster["DienGiai"] = row["DienGiai"];
            drCurrentMaster["TkCo"] = row["TkCo"];
            drCurrentMaster["MaThueMT"] = row["MaThue"];
           // drCurrentMaster["TkCK"] = "711";

            drCurrentMaster["TkThue"] = "1331";
            drCurrentMaster["PrintIndex"] = 0;
            drCurrentMaster.EndEdit();

        }
        private void importDT12Row(DataRow drDT12, DataRowView row)
        {
            drDT12["MT12ID"] = row["MTID"];
            drDT12["DT12ID"] = Guid.NewGuid();

            drDT12["DiengiaiCt"] = row["DienGiai"];
            //drDT12["MaNT"] = "VND";
            //drDT12["TyGia"] = 1;
            drDT12["PsNT"] = row["TTienH"];

            drDT12["TkNo"] = row["TkNo"];
            drDT12["MaThue"] = row["MaThue"];
            drDT12["TienThue"] = row["TThue"];
            drDT12.EndEdit();

        }
        private void importMT16Row(DataRow drCurrentMaster, DataRow row)
        {
            drCurrentMaster["MCCQT"] = row["MCCQT"];
            drCurrentMaster["MT16ID"] = row["MTID"];
            drCurrentMaster["MaCT"] = "PBN";
            drCurrentMaster["NgayCT"] = row["Ngayhd"];
            drCurrentMaster["MaKH"] = row["MaKH"];

            drCurrentMaster["Ongba"] = row["Ongba"];
            drCurrentMaster["DiaChi"] = row["DiaChi"];
            drCurrentMaster["MaNT"] = "VND";
            drCurrentMaster["TyGia"] = 1;
            drCurrentMaster["DienGiai"] = row["DienGiai"];
            drCurrentMaster["TkCo"] = row["TkCo"];
            drCurrentMaster["MaThueMT"] = row["MaThue"];
            // drCurrentMaster["TkCK"] = "711";

            drCurrentMaster["TkThue"] = "1331";
            //drCurrentMaster["PrintIndex"] = 0;
            drCurrentMaster.EndEdit();

        }
        private void importDT16Row(DataRow drDT16, DataRowView row)
        {
            drDT16["MT16ID"] = row["MTID"];
            drDT16["DT16ID"] = Guid.NewGuid();

            drDT16["DiengiaiCt"] = row["DienGiai"];
            
            drDT16["PsNT"] = row["TTienH"];            
            drDT16["TkNo"] = geTkNo.EditValue.ToString();
            //drDT16["MaThue"] = row["MaThue"];
            //drDT16["TienThue"] = row["TThue"];
            drDT16.EndEdit();

        }
        private void importVatInRowMT22(DataRow drVatin, DataRow row)
        {
            drVatin["MTID"] = row["MTID"];
            drVatin["NgayHD"] = row["NgayHD"];
            drVatin["NgayCt"] = row["NgayHD"];
            drVatin["Soseries"] = row["kyhieu"];
            drVatin["TTien"] = row["TTienH"];
            drVatin["MaThue"] = row["MaThue"];
            drVatin["TThue"] = row["TThue"];
            
        }
        private void importVatInRowMT12(DataRow drVatin, DataRow row)
        {
            drVatin["MTID"] = row["MTID"];
            drVatin["NgayHD"] = row["NgayHD"];
            drVatin["sohoadon"] = row["Sohoadon"];
            drVatin["NgayCt"] = row["NgayHD"];
            drVatin["Soseries"] = row["Kyhieu"];
            drVatin["TTien"] = row["TTienH"];
            drVatin["MaThue"] = row["MaThue"];
            drVatin["TThue"] = row["TThue"];
            
        }
        private void importVatInRowMT16(DataRow drVatin, DataRow row)
        {
            drVatin["MTID"] = row["MTID"];
            drVatin["NgayHD"] = row["NgayHD"];
            drVatin["sohoadon"] = row["Sohoadon"];
            drVatin["NgayCt"] = row["NgayHD"];
            drVatin["Soseries"] = row["Kyhieu"];
            drVatin["TTien"] = row["TTienH"];
            drVatin["MaThue"] = row["MaThue"];
            drVatin["TThue"] = row["TThue"];

        }
        private void labelControl5_Click(object sender, EventArgs e)
        {

        }

        private void geTkCK_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btXoaHD_Click(object sender, EventArgs e)
        {
            string condition = " in ('";

            foreach (DataRow dr in tbMT.Rows)
            {
                condition += dr["MTID"].ToString() + "','";

            }
            if (tbMT.Rows.Count > 0) condition = condition.Substring(0, condition.Length - 2) + ")";
            else condition = "1=0";
            _dataMt22.ConditionMaster = "MT22ID " + condition;
            _dataMt22.GetData();
            bsMT22.DataSource = _dataMt22.DsData;
            bsMT22.DataMember = _dataMt22.DsData.Tables[0].TableName;
            
           
             
            _dataMt21.ConditionMaster = "MT21ID " + condition;
            _dataMt21.GetData();
            bsMT21.DataSource = _dataMt21.DsData;
            bsMT21.DataMember = _dataMt21.DsData.Tables[0].TableName;

            
            
            _dataMt12.ConditionMaster = "MT12ID " + condition;
            _dataMt12.GetData();
            bsMT12.DataSource = _dataMt12.DsData;
            bsMT12.DataMember = _dataMt12.DsData.Tables[0].TableName;
            _dataMt16.ConditionMaster = "MT16ID " + condition;
            _dataMt16.GetData();
            bsMT16.DataSource = _dataMt16.DsData;
            bsMT16.DataMember = _dataMt16.DsData.Tables[0].TableName;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = bsMT22.Count + bsMT12.Count + bsMT21.Count +bsMT16.Count;
            progressBar1.Step = 1;
            progressBar1.Value = 0;
            DeleteHD();
            DeleteHDDV();
            DeletePC();
            DeleteHPBN();
        }
        private async void DeleteHD()
        {

            while (bsMT22.Count > 0)
            {
                bsMT22.MoveLast();
                DataRowView drMT = (bsMT22.Current as DataRowView);

                if (drMT != null)
                    _dataMt22.DrCurrentMaster = drMT.Row;
                _dataMt22.LstDrCurrentDetails.Clear();
                _dataMt22._formulaCaculator.Active = false;

                bsMT22.RemoveCurrent();
                bool isError = !_dataMt22.UpdateData(DataAction.Delete);
                UpdateProgressBar(bsMT22.Count + bsMT12.Count + bsMT21.Count + bsMT16.Count);
                await Task.Delay(1);
                if (isError) break;
                // bs.MovePrevious();
            }


        }
        private async void DeleteHDDV()
        {

            while (bsMT21.Count > 0)
            {
                bsMT21.MoveLast();
                DataRowView drMT = (bsMT21.Current as DataRowView);

                if (drMT != null)
                    _dataMt21.DrCurrentMaster = drMT.Row;
                _dataMt21.LstDrCurrentDetails.Clear();
                _dataMt21._formulaCaculator.Active = false;

                bsMT21.RemoveCurrent();
                bool isError = !_dataMt21.UpdateData(DataAction.Delete);
                UpdateProgressBar(bsMT22.Count + bsMT12.Count + bsMT21.Count + bsMT16.Count);
                await Task.Delay(1);
                if (isError) break;
                // bs.MovePrevious();
            }
        }
        private async void DeletePC()
        {

            while (bsMT12.Count > 0)
            {
                bsMT12.MoveLast();
                DataRowView drMT = (bsMT12.Current as DataRowView);

                if (drMT != null)
                    _dataMt12.DrCurrentMaster = drMT.Row;
                _dataMt12.LstDrCurrentDetails.Clear();
                _dataMt12._formulaCaculator.Active = false;

                bsMT12.RemoveCurrent();
                bool isError = !_dataMt12.UpdateData(DataAction.Delete);
                UpdateProgressBar(bsMT22.Count + bsMT12.Count + bsMT21.Count + bsMT16.Count);
                await Task.Delay(1);
                if (isError) break;
                // bs.MovePrevious();
            }
        }
        private async void DeleteHPBN()
        {

            while (bsMT16.Count > 0)
            {
                bsMT16.MoveLast();
                DataRowView drMT = (bsMT16.Current as DataRowView);

                if (drMT != null)
                    _dataMt16.DrCurrentMaster = drMT.Row;
                _dataMt16.LstDrCurrentDetails.Clear();
                _dataMt16._formulaCaculator.Active = false;

                bsMT16.RemoveCurrent();
                bool isError = !_dataMt16.UpdateData(DataAction.Delete);
                UpdateProgressBar(bsMT22.Count + bsMT12.Count + bsMT21.Count + bsMT16.Count);
                await Task.Delay(1);
                if (isError) break;
                // bs.MovePrevious();
            }
        }


    }


}
