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
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using ErrorManager;
namespace CusAccounting
{

    public partial class fImportHDDaura_Minv : DevExpress.XtraEditors.XtraForm
    {
        [Obsolete]
        public fImportHDDaura_Minv()
        {
            InitializeComponent();
            DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
        
            rEisDV.MouseUp += REisDV_MouseUp;
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
        DataSingle dbdmThueSuat;//= new DataSingle("DMtk", "7");
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
        DataMasterDetail _dataMt32;
        BindingSource bsMT32 = new BindingSource();
        DataMasterDetail _dataMt31;
        BindingSource bsMT31 = new BindingSource();
        FormDesigner _frmDesign;
        private async Task<string> GetData(int page)
        {
            string token = "";
            if (Config.Variables.ContainsKey("InvToken")) token = Config.GetValue("InvToken").ToString();
            else return "";
            string MST = "";
            if (Config.Variables.ContainsKey("MaSoThue")) MST = Config.GetValue("MaSoThue").ToString();
            else return "" ;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            string requestUri = "";
            if (Config.Variables.ContainsKey("MInvoiceUrl")) requestUri = Config.GetValue("MInvoiceUrl").ToString();
            requestUri += "?page=" + page.ToString().Trim() + "&size=50&invoiceType=OUTPUT_ELECTRONIC_INVOICE&invoiceReleaseDateFrom=" + DateTime.Parse(dTungay.EditValue.ToString()).ToString("dd/MM/yyyy") + "&invoiceReleaseDateTo=" + DateTime.Parse(dDenngay.EditValue.ToString()).ToString("dd/MM/yyyy") + "&sellerTaxNo=" + MST;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("apiToken", token); 
            try
            {
                HttpResponseMessage response = await client.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException ex)
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
            return "";
        }

        private async void btLoadData_ClickAsync(object sender, EventArgs e)
        {
            lstid = new string[] { };
            string data = "";
            try
            {
                data = await  GetData(1);
            }
            catch(Exception ex)
            {

            }
            if (data == "" || data==null) return;
            tbDT.Rows.Clear();
            tbMT.Rows.Clear();
            _dataMt32 = new DataMasterDetail("DT32", "7");
            _dataMt32.ConditionMaster = "1=0";
            _dataMt32.GetData();
            _dataMt31 = new DataMasterDetail("DT31", "7");
            _dataMt31.ConditionMaster = "1=0";
            _dataMt31.GetData();

            bsMT32.DataSource = _dataMt32.DsData;
            bsMT32.DataMember = _dataMt32.DsData.Tables[0].TableName;
            bsMT31.DataSource = _dataMt31.DsData;
            bsMT31.DataMember = _dataMt31.DsData.Tables[0].TableName;

            MInvoiceList mInvoiceList = JsonConvert.DeserializeObject<MInvoiceList>(data);
            if (mInvoiceList != null)
            {
                int pagetotal = (int)mInvoiceList.totalPage;
                CreateData1page(mInvoiceList);
                if (pagetotal > 1)
                {
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
                        DataRow[] lstTrung = tbMT.Select("MTID='" + inv.id + "'");

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
                            if (HHDV_minv.tchat == "4") continue;
                            if (firstRow == 0) { drMT["DienGiai"] = HHDV_minv.ten; firstRow++; }

                            DataRow drDT = tbDT.NewRow();
                            tbDT.Rows.Add(CreateDTRow(drMT, drDT, HHDV_minv));
                        }
                    }
                }
            }
        }
        private DataRow CreateMTRow(DataRow drMT, MInvoice hd)
        {

            if (hd.nky != null && hd.ntao > DateTime.Parse(hd.nky.ToString()).AddDays(1))
            {

            }
            if (hd.mhdon != null)
                drMT["MCCQT"] = hd.mhdon;
            if (hd.ntao != null)
                drMT["Ngayhd"] = DateTime.Parse(hd.ntao.ToString()).Date; 
            else
                drMT["Ngayhd"] = DateTime.Parse(hd.nky.ToString()).Date;
            drMT["Sohoadon"] ="0000000".Substring(0,7- hd.shdon.ToString().Length) + hd.shdon.ToString();
            drMT["Kyhieu"] = hd.khhdon;
            drMT["HTTToan"] = hd.thtttoan;
            drMT["TenKH"] = hd.nmten;
            drMT["MST"] = hd.nmmst;
            
            if (hd.nmdchi != null)
            {
                drMT["DiaChi"] = hd.nmdchi;
            }
            else
            {
                foreach(ttkhac Ttkhac in hd.ttkhac)
                {
                    if(Ttkhac.ttruong.ToLower()=="địa chỉ" && Ttkhac.dlieu!=null)
                    {
                        drMT["DiaChi"] = Ttkhac.dlieu;
                        break;
                    }
                }
            }    
            drMT["Ongba"] = hd.nmtnmua;

                drMT["TTienH"] = hd.tgtcthue==null?0: hd.tgtcthue;
                drMT["TThue"] = hd.tgtthue==null ? 0 : hd.tgtthue;
            drMT["TTien"] = hd.tgtttbso==null ? 0 : hd.tgtttbso;
            if(hd.thttltsuat!=null && hd.thttltsuat.Length > 0)
            {
                drMT["MaThue"] = hd.thttltsuat[0].tsuat.Replace("%", "");
            }
            else
            {
                drMT["MaThue"] = "KCT";
            }

            // drMT["MaThue"] = hd.DLHDon.NDHDon.TToan.THTTLTSuat.TTSuat.TSuat.Replace("%", "");
            drMT["TkNo"] = geTkNo.EditValue.ToString();
            drMT.EndEdit();
            return drMT;
        }
        private DataRow CreateDTRow(DataRow drMT, DataRow drDT, HHDVu_Minv hh)
        {
            drDT["MTID"] = drMT["MTID"];
            drDT["Sohoadon"] = drMT["Sohoadon"];
            drDT["MTIDDT"] =hh.id;
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
            drDT["TTien"] = hh.thtien;
            if (hh.ltsuat != null)
                drDT["MaThueCT"] = hh.ltsuat.Replace("%", "");
            else
                drDT["MaThueCT"] = "KT";
            double Thuesuat = 0;
            if (hh.tsuat != null)
                Thuesuat = (double)hh.tsuat * 100;
            else
                Thuesuat = 0;
            drDT["Thuesuat"] = Thuesuat;
          
            
            
            
            drDT["TienThue"] = Math.Round((double)((hh.thtien - hh.stckhau) * Thuesuat / 100), 0);
            drDT["TkDthu"] = geTkdthu.EditValue.ToString();
            drDT["Tkgv"] = geTkgv.EditValue.ToString();
            drDT["Tkkho"] = geTkkho.EditValue.ToString();
            drDT["isDV"] = 0;
            drDT.EndEdit();
            return drDT;
        }
        private void fImportHDDauRa_Load(object sender, EventArgs e)
        {

            if (Config.Variables.Contains("MaSoThue")) this.Text += ": " + Config.GetValue("MaSoThue").ToString();
            //Lấy dữ liệu MT32
            _dataMt32 = new DataMasterDetail("DT32", "7");
            _dataMt32.ConditionMaster = "1=0";
            _dataMt32.GetData();
            bsMT32.DataSource = _dataMt32.DsData;
            bsMT32.DataMember = _dataMt32.DsData.Tables[0].TableName;

            //Lấy dữ liệu MT31
            _dataMt31 = new DataMasterDetail("DT31", "7");
            _dataMt31.ConditionMaster = "1=0";
            _dataMt31.GetData();
            bsMT31.DataSource = _dataMt31.DsData;
            bsMT31.DataMember = _dataMt31.DsData.Tables[0].TableName;
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
                    case "mathue":
                        reMaThue = _frmDesign.GenRIGridLookupEdit(drCol);
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
            dbdmThueSuat = publicCDTData.findCDTData("dmThueSuat", "", "");
            if (dbdmThueSuat == null) dbdmThueSuat = new DataSingle("dmThueSuat", "7");
            DataFactory.publicCDTData.AddCDTData(dbdmThueSuat);
            if (!dbdmKH.FullData) dbdmKH.GetData();
            if (!dbdmVT.FullData) dbdmVT.GetData();
            if (!dbdmKho.FullData) dbdmKho.GetData();
            if (!dbdmDVT.FullData) dbdmDVT.GetData();
            if (!dbdmHTTT.FullData) dbdmHTTT.GetData();
            if (!dbdmTk.FullData) dbdmTk.GetData();
            if (!dbdmThueSuat.FullData) dbdmThueSuat.GetData();

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
            gcMaThue.ColumnEdit = reMaThue;


            geMaKho.Properties.DataSource = dbdmKho.DsData.Tables[0]; geMaKho.Properties.ValueMember = "MaKho"; geMaKho.Properties.DisplayMember = "TenKho"; geMaKho.EditValue = "HH";
            geTkNo.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkNo.Properties.ValueMember = "TK"; geTkNo.Properties.DisplayMember = "TK"; geTkNo.EditValue = "131";
            geTkdthu.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkdthu.Properties.ValueMember = "TK"; geTkdthu.Properties.DisplayMember = "TK"; geTkdthu.EditValue = "5111";
            geTkdthuDV.Properties.DataSource = dbdmTk.DsData.Tables[0]; geTkdthuDV.Properties.ValueMember = "TK"; geTkdthuDV.Properties.DisplayMember = "TK"; geTkdthuDV.EditValue = "5113";
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
            reDMVT.EditValueChanging += ReDMVT_EditValueChanging;
            reDMDVT.KeyUp += ReDMDVT_KeyUp;
            tbDT.ColumnChanged += TbDT_ColumnChanged;
            tbDT.ColumnChanging += TbDT_ColumnChanging;
            DictionaryName = new DataSingle("DictionaryName", "7");
            DictionaryName.GetData();
            gridControl2.KeyUp += GridControl2_KeyUp;
            gridControl1.KeyUp += GridControl1_KeyUp;
            gridView1.CustomDrawRowIndicator += GridView1_CustomDrawRowIndicator;
            gridView2.CustomDrawRowIndicator += GridView1_CustomDrawRowIndicator;
            gridView1.IndicatorWidth = 40;
            gridView2.IndicatorWidth = 40;
        }

        private void ReDMVT_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue != null &&   e.OldValue== DBNull.Value)
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



        private void GridControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4 && bs != null)
            {
                bs.RemoveCurrent();
                ds.AcceptChanges();

            }
        }

        private void GridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && (e.RowHandle >= 0))
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
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
                    DictionaryName.DrCurrentMaster = ldr[0];
                    DictionaryName.UpdateData(DataAction.Update);
                }
            }
        }

        private void GridControl2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11 && gridControl2.DataMember != "")
            {
                gridControl2.DataSource = tbDT;
                gridControl2.DataMember = "";
            }
            else if (e.KeyCode == Keys.F11)
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
            DictionaryName.GetData();
        }

        private void btCheckMaKH_Click(object sender, EventArgs e)
        {
            string[] arrHTTT = ThietKedulieu.ConvertDataTableToArray(dbdmKH.DsData.Tables[0], "TenKH");

            foreach (DataRow drMT in tbMT.Rows)
            {
                DataRow[] lstRows = dbdmKH.DsData.Tables[0].Select("TenKH='" + drMT["TenKH"].ToString().Replace("'","") + "'");
                if (lstRows.Length > 0)
                {
                    drMT["MaKH"] = lstRows[0]["MaKH"];
                    drMT.EndEdit();
                    continue;
                }
                else
                {
                    if (drMT["TenKH"].ToString() == "")
                    {

                    }

                    lstRows = DictionaryName.DsData.Tables[0].Select("Name='" + drMT["TenKH"].ToString().Replace("'", "") + "' and TableName='DMKH'");
                    if (lstRows.Length > 0)
                    {
                        drMT["MaKH"] = lstRows[0]["Code"];
                        drMT.EndEdit();
                        continue;
                    }
                }
                int j = ThietKedulieu.LevantEdit(arrHTTT, drMT["TenKH"].ToString().Replace("'", ""));
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
                    int j1 = ThietKedulieu.FindClosestString(arrHTTT, drMT["TenKH"].ToString().Replace("'", "").ToLower());

                    if (j1 >= 0 && j1 < arrHTTT.Length)
                    {
                        if (dbdmKH.DsData.Tables[0].Rows[j1]["MST"] == DBNull.Value || dbdmKH.DsData.Tables[0].Rows[j1]["MST"].ToString().Trim() == "" || dbdmKH.DsData.Tables[0].Rows[j1]["MST"].ToString().Trim() == drMT["MST"].ToString().Trim())
                        {
                            drMT["MaKH"] = dbdmKH.DsData.Tables[0].Rows[j1]["MaKH"];
                            drMT.EndEdit();
                        }
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
                    if (drMT["MST"] == DBNull.Value) continue;
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
        private  void  check1VT(DataRow drDT, string[] arrVTDic, string[] arrVT, DataRow[] lstRowsDic)
        {
            double dic2;
            double dic1;
            string TenVT = drDT["TenVT"].ToString().ToLower();

            if (TenVT == "AQUAFINA SODA LON 320x24".ToLower())
            {

            }
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
            int j1 = ThietKedulieu.LevantEditWithWeightHasDic(arrVT, TenVT,1000, out dic1);//Khoảng cách trên vật tư
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
    
        private void btCheckVT_Click(object sender, EventArgs e)
        {
            DataRow[] lstRowsDic = DictionaryName.DsData.Tables[0].Select("TableName='DMVT'");//Name='" + drDT["TenVT"].ToString() + "' and 
            string[] arrVTDic = ThietKedulieu.ConvertDataRowsToArray(lstRowsDic, "Name");

            string[] arrVT = ThietKedulieu.ConvertDataTableToArray(dbdmVT.DsData.Tables[0], "TenVT");
            //DateTime t1 = DateTime.Now;

            //DateTime t2 = DateTime.Now;
            //MessageBox.Show((t2 - t1).TotalMilliseconds.ToString());



            //test với 1 nhân
            //t1 = DateTime.Now;
            foreach ( DataRow drDT in tbDT.Rows)
            {
                check1VT(drDT, arrVTDic, arrVT, lstRowsDic);



                //int j1 = ThietKedulieu.FindClosestString(arrVT, drDT["TenVT"].ToString().ToLower());

                //if (j1 >= 0 && j1 < arrVT.Length)
                //{
                //    drDT["MaVT"] = dbdmVT.DsData.Tables[0].Rows[j1]["MaVT"];
                //    drDT["TKKho"] = dbdmVT.DsData.Tables[0].Rows[j1]["TkKho"];
                //    drDT.EndEdit();
                //}


                // }
            }
            // t2 = DateTime.Now;
            //MessageBox.Show((t2 - t1).TotalMilliseconds.ToString());
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
                if (drv["KieuHD"] == DBNull.Value)
                {
                    MessageBox.Show("Chưa phân loại hóa đơn nên không phân biệt được dịch vụ hay hàng hóa!");
                    break;
                }
                if (drv["KieuHD"] != DBNull.Value && drv["KieuHD"].ToString() == "1")
                {
                    bs.MoveNext();
                    continue;
                }
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
                if (drDT["DVT"].ToString() == "") 
                {
                }
                if (lstRows.Length > 0)
                {

                    drDT["MaDVT"] = lstRows[0]["MaDVT"];
                    if (dbdmDVT.DsData.Tables[0].Columns.Contains("isDV"))
                    {
                        if(lstRows[0]["IsDV"]!=null && lstRows[0]["isDV"].ToString().ToLower() == "true") {
                            drDT["isDV"] = 1;
                        }
                    }
                    drDT.EndEdit();
                    continue;
                }
                else
                {
                    lstRows = DictionaryName.DsData.Tables[0].Select("Name='" + drDT["DVT"].ToString() + "' and TableName='DMDVT'");
                    if (lstRows.Length > 0)
                    {
                        drDT["MaDVT"] = lstRows[0]["Code"];
                        if (DictionaryName.DsData.Tables[0].Columns.Contains("isDV"))
                        {
                            if (lstRows[0]["IsDV"] != null && lstRows[0]["isDV"].ToString().ToLower() == "true")
                            {
                                drDT["isDV"] = 1;
                            }
                        }
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
        private bool InsertMT32(DataRowView drv)
        {
            _dataMt32.LstDrCurrentDetails.Clear();

            DataRow[] lstDT = tbDT.Select("MTID='" + drv["MTID"].ToString() + "'");
            if (_dataMt32.DsData.Tables[0].Select("MT32ID='" + drv["MTID"].ToString() + "'").Length > 0)
            {

                return true;
            }
            string sql = "select count(*) from mt32 where mccqt='" + drv["MCCQT"].ToString() + "'";
            object i = _dataMt32.DbData.GetValue(sql);
            if (i != null && int.Parse(i.ToString()) > 0)
            {
                return true;
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
                        _dataMt32.CancelUpdate();
                        return false;
                    }
                }
                foreach (DataColumn col in _dataMt32.DsData.Tables[1].Columns)
                {
                    foreach (DataRow drCurrentDT in _dataMt32.LstDrCurrentDetails)
                    {
                        string err = drCurrentDT.GetColumnError(col.ColumnName);
                        if (err != string.Empty)
                        {
                            MessageBox.Show("Cột " + col.ColumnName + " có lỗi: " + err);
                            _dataMt32.CancelUpdate();
                            return false;
                        }
                    }

                }
                _dataMt32.CancelUpdate();
                return false;

            }
            else
            {
                if (_dataMt32.UpdateData(DataAction.Insert))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Thêm vào hóa đơn bị lỗi. Hóa đơn số :" + _dataMt32.DrCurrentMaster["SoHoaDon"].ToString());
                    return false;
                }
            }
        }
        private bool InsertMT31(DataRowView drv)
        {
            _dataMt31.LstDrCurrentDetails.Clear();

            DataRow[] lstDT = tbDT.Select("MTID='" + drv["MTID"].ToString() + "'");
            if (_dataMt31.DsData.Tables[0].Select("MT31ID='" + drv["MTID"].ToString() + "'").Length > 0)
            {
                return true;
            }
            string sql = "select count(*) from mt31 where mccqt='" + drv["MCCQT"].ToString() + "'";
            object i = _dataMt31.DbData.GetValue(sql);
            if (i != null && int.Parse(i.ToString()) > 0)
            {
                
                return true;
            }

            bsMT31.AddNew();
            bsMT31.EndEdit();
            bsMT31.MoveLast();


            DataRowView drMT = (bsMT31.Current as DataRowView);

            if (drMT != null)
                _dataMt31.DrCurrentMaster = drMT.Row;
            importMT31Row(_dataMt31.DrCurrentMaster, drv.Row);
            foreach (DataRow drDT in lstDT)
            {
                DataRow drDT31 = _dataMt31.DsData.Tables[1].NewRow();

                if (drDT31.RowState == DataRowState.Detached)
                    _dataMt31.DsData.Tables[1].Rows.Add(drDT31);
                importDT31Row(drDT31, drDT);
                drDT31.EndEdit();
            }
            _dataMt31.CheckRules(DataAction.Insert);
            if (_dataMt31.DsData.HasErrors)
            {
                foreach (DataColumn col in _dataMt31.DsData.Tables[0].Columns)
                {
                    string err = _dataMt31.DrCurrentMaster.GetColumnError(col.ColumnName);
                    if (err != string.Empty)
                    {
                        MessageBox.Show("Cột " + col.ColumnName + " Có lỗi: " + err);
                        _dataMt31.CancelUpdate();
                        return false;
                    }
                }
                foreach (DataColumn col in _dataMt31.DsData.Tables[1].Columns)
                {
                    foreach (DataRow drCurrentDT in _dataMt31.LstDrCurrentDetails)
                    {
                        string err = drCurrentDT.GetColumnError(col.ColumnName);
                        if (err != string.Empty)
                        {
                            MessageBox.Show("Cột " + col.ColumnName + " có lỗi: " + err);
                            _dataMt31.CancelUpdate();
                            return false;
                        }
                    }

                }
                _dataMt31.CancelUpdate();
                return false;
            }
            else
            {
                if (_dataMt31.UpdateData(DataAction.Insert))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Thêm vào hóa đơn bị lỗi. Hóa đơn số :" + _dataMt31.DrCurrentMaster["SoHoaDon"].ToString());
                    return false;
                }
            }
        }
        private async void InserttoData()
        {
            bs.MoveFirst();

            //_dataMt32.ConditionMaster = "1=0";
            //_dataMt32.GetData();
            //bsMT32.DataSource = _dataMt32.DsData;
            //bsMT32.DataMember = _dataMt32.DsData.Tables[0].TableName;
            //_dataMt32.LstDrCurrentDetails.Clear();

            bool HasErr = false;
            for (int idx = 0; idx < bs.Count; idx++)
            {
                if (HasErr) break;                
                DateTime t1 = DateTime.Now;
                 UpdateProgressBar(bs.Position + 1);
                await Task.Delay(1);

                DataRowView drv = (bs.Current as DataRowView);
                switch (drv["KieuHD"].ToString())
                {
                    case "0":// Hóa đơn bán hàng
                        HasErr = !InsertMT32(drv);
                        if (HasErr) _dataMt32.DsData.RejectChanges();
                        break;
                    case "1": // Hóa đơn dịch vụ
                        HasErr = !InsertMT31(drv);
                        if (HasErr) _dataMt31.DsData.RejectChanges();
                        break;
                }

                bs.MoveNext();
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
            drCurrentMaster["MaThue"] = row["MaThue"];
           
            drCurrentMaster["PrintIndex"] =0;
            drCurrentMaster.EndEdit();

        }
        private void importDT31Row(DataRow drDT31, DataRow drDT)
        {
            drDT31["MT31ID"] = drDT["MTID"];
            drDT31["DT31ID"] = drDT["MTIDDT"];
            //drDT31["MaKho"] = drDT["MaKho"];
            drDT31["MaKHCt"] = _dataMt31.DrCurrentMaster["MaKH"];
            drDT31["DienGiaiCt"] = drDT["TenVT"];
            //drDT32["Soluong"] = drDT["Soluong"];
            drDT31["MaThueCT"] = drDT["MaThueCT"];
            double thuesuat = 0;
            if (double.TryParse(drDT["Thuesuat"].ToString().Replace("%", ""), out thuesuat))
                drDT31["Thuesuat"] = thuesuat;
            else drDT31["Thuesuat"] = 0;

            //drDT31["GiaNT"] = drDT["DonGia"]==DBNull.Value?0: drDT["DonGia"];
            drDT31["PsNT"] = drDT["TTien"] == DBNull.Value ? 0 : drDT["TTien"];
            //if (drDT["TileCK"] != DBNull.Value)
            //    drDT31["TileCK"] = drDT["TileCK"].ToString().Replace("%", "");
            //double ck = 0;
            //if (double.TryParse(drDT["CK"].ToString().Replace("%", ""), out ck))
            //    drDT31["CK"] = ck;
            //else drDT31["CK"] = 0;


            //drDT["TienThue"] = Math.Round((hh.ThTien - hh.STCKhau) * double.Parse(hh.TSuat.Replace("%", "")) / 100, 0);
            drDT31["TkCo"] = geTkdthuDV.EditValue.ToString();
            //drDT31["Tkgv"] = geTkgv.EditValue.ToString();
           // drDT31["Tkkho"] = geTkkho.EditValue.ToString();
            //if (drDT["Soluong"] == DBNull.Value || drDT["Soluong"].ToString() == "0")
            //{
            //    drDT31["isDV"] = 1;
            //}
            //else
            //    drDT31["isDV"] = 0;
            drDT31.EndEdit();
        }

        private void importMT31Row(DataRow drCurrentMaster, DataRow row)
        {
            drCurrentMaster["MCCQT"] = row["MCCQT"];
            drCurrentMaster["MT31ID"] = row["MTID"];
            drCurrentMaster["MaCT"] = "HDV";
            drCurrentMaster["NgayCT"] = row["Ngayhd"];
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
            drCurrentMaster["DienGiai"] = row["DienGiai"];
            drCurrentMaster["TkNo"] = row["TkNo"];
            drCurrentMaster["MaThue"] = "10";
            drCurrentMaster["TkCK"] = "5211";
            drCurrentMaster["MaThue"] = row["MaThue"];

           // drCurrentMaster["PrintIndex"] = 0;
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

            _dataMt31.ConditionMaster = condition;
            _dataMt31.GetData();
            bsMT31.DataSource = _dataMt31.DsData;
            bsMT31.DataMember = _dataMt31.DsData.Tables[0].TableName;
            bs.MoveLast();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = bsMT32.Count + bsMT31.Count;
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
               UpdateProgressBar(bsMT32.Count + bsMT31.Count);
                await Task.Delay(1);
                if (isError) break;
                // bs.MovePrevious();
            }
            while (bsMT31.Count > 0)
            {
                bsMT31.MoveLast();
                DataRowView drMT = (bsMT31.Current as DataRowView);

                if (drMT != null)
                    _dataMt31.DrCurrentMaster = drMT.Row;
                _dataMt31.LstDrCurrentDetails.Clear();
                _dataMt31._formulaCaculator.Active = false;

                bsMT31.RemoveCurrent();
                bool isError = !_dataMt31.UpdateData(DataAction.Delete);
                UpdateProgressBar(bsMT31.Count);
                
                if (isError) break;
                // bs.MovePrevious();
            }

        }

        private void btPhanloai_Click(object sender, EventArgs e)
        {
            foreach (DataRow drMt in tbMT.Rows)
            {
                DataRow[] lstRowHH = tbDT.Select("isDV=0 and MTID='" + drMt["MTID"].ToString() + "'");
                if (lstRowHH.Length == 0)// Không có dòng hàng hóa nào chắc chắn phải vào hóa đơn dịch vụ
                    drMt["KieuHD"] = 1;
                else drMt["KieuHD"] = 0;
            }
        }


    }


}
