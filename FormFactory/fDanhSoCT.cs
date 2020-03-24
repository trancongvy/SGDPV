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

using DataFactory;
using CDTDatabase;
using CDTLib;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using CDTControl;
using DevExpress.XtraGrid.Views.Grid;

namespace FormFactory
{
    public partial class fDanhSoCT : XtraForm
    {
        public fDanhSoCT()
        {
           

        }
        Database _dbStruct = Database.NewStructDatabase();
        Database _dbData = Database.NewDataDatabase();
        public DataMasterDetail _data;
        public FormDesigner _designer;
        public DevExpress.XtraGrid.GridControl gcMain =new GridControl();
        public DevExpress.XtraGrid.Views.Grid.GridView gvMain;
        public ImageList ImgList;
        DataTable tb;
        DataTransfer _tranfer;

        public fDanhSoCT(DataMasterDetail data, FormDesigner designer)
        {
            InitializeComponent();
             
            _data = data;
            _designer = designer;
        }


        private void grLoaiCT_EditValueChanged(object sender, EventArgs e)
        {

  

        }
        private Image GetImage(byte[] b)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(b);
            if (ms == null)
                return null;
            Image im = Image.FromStream(ms);
            return (im);
        }
        private void setImage()
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvMain.Columns)
            {
                if (col.FieldName.ToUpper() == "TASKID")
                {
                    DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repTask = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
                    this.gcMain.RepositoryItems.Add(repTask);

                    repTask.AutoHeight = false;
                    repTask.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
                    if (_data.tbTask != null)
                    {
                        for (int i = 0; i < _data.tbTask.Rows.Count; i++)
                        {
                            DataRow drTask = _data.tbTask.Rows[i];
                            if (drTask["Icon"] != DBNull.Value)
                            {
                                this.ImgList.Images.Add(GetImage(drTask["Icon"] as byte[]));
                                repTask.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
                            new DevExpress.XtraEditors.Controls.ImageComboBoxItem(drTask["TaskLabel"].ToString(), drTask["ID"], repTask.Items.Count)});
                            }
                        }
                    }
                    repTask.SmallImages = this.ImgList;

                    col.ColumnEdit = repTask;
                    col.Caption = "";
                }
            }
        }

        private void fDanhSoCT_Load(object sender, EventArgs e)
        {
            
                gcMain=_designer.GenGridControl(_data.DsStruct.Tables[0],false,DockStyle.Fill);
                gvMain = gcMain.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                setImage();
                tb = _data.DsData.Tables[0].Copy();
                gcMain.DataSource = tb;
            panelControl3.Controls.Add(gcMain);
                textEdit1.Text = _data.DrTable["MaCT"].ToString();
                textEdit2.Text = _data.DrTable["MasterTable"].ToString();
                DataRow[] ldrSoCT = _data.DsStruct.Tables[0].Select("FieldName='SoCT'");
                if(ldrSoCT !=null && ldrSoCT.Length > 0)
                {
                    textEdit3.Text = ldrSoCT[0]["EditMask"].ToString();
                }
                //getRelationConfig();
            
        }

        private void getRelationConfig()
        {
            
            string pk = _data.DrTableMaster["Pk"].ToString();
            
            
            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            AutoIncrementValues _autoIncreValues;
                string sysTableID = _data.DrTable["SysTableID"].ToString();
            string soct = textEdit3.Text;
           

            for (int i = 0; i< tb.Rows.Count; i++)
            { 
                DataRow dr = gvMain.GetDataRow(i);
                if (dr != null)
                {
                    _autoIncreValues = new AutoIncrementValues(sysTableID, _data.DsStruct.Tables[0], dr);
                    string NewSoCT = _autoIncreValues.GetNewValueByOldvalue(soct, textEdit3.Text, DateTime.Parse(dr["NgayCT"].ToString()));
                    dr["SoCT"] = NewSoCT;
                    dr.EndEdit();
                    soct = NewSoCT;
                    
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            string pk= _data.DrTableMaster["Pk"].ToString();
            string mtTableID = _data.DrTableMaster["sysTableID"].ToString();
            _tranfer = new DataTransfer(_dbData, mtTableID, pk);
            DataTable tbConfig = _tranfer.GetDtConfig();
            try
            {
                _dbData.BeginMultiTrans();
                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    DataRow dr = gvMain.GetDataRow(i);
                    if (dr != null)
                    {
                        string sql = "update " + _data.DrTable["MasterTable"].ToString() + " set SoCT='" + dr["SoCT"].ToString() + "' where " + pk + "='" + dr[pk].ToString() + "'";
                        _dbData.UpdateByNonQuery(sql);
                        if (_dbData.HasErrors)
                        {
                            _dbData.RollbackMultiTrans();
                            MessageBox.Show("Lỗi xảy ra!");
                            return;
                        }
                        //Update config

                        foreach (DataRow drConfig in tbConfig.Rows)
                        {
                            string blTableName = drConfig["blTableName"].ToString();
                            string mtTableName = drConfig["mtTableName"].ToString();
                            string nhomDk = drConfig["NhomDk"].ToString();
                            string mtIDName = drConfig["RootIDName"].ToString();

                            DataTable tbConfigdt = _tranfer.GetDtConfigDetail(drConfig["blConfigID"].ToString());
                            foreach (DataRow drConfigDt in tbConfigdt.Rows)
                            {
                                if (drConfigDt["mtFieldName"].ToString().ToLower() == "soct")
                                {
                                    string sqlUpdate = "update " + blTableName + " set " + drConfigDt["blFieldName"].ToString() + " = '" + dr["SoCT"].ToString() + "' where " + mtIDName + "='" + dr[pk].ToString() + "' and NhomDK='" + nhomDk + "'";
                                    _dbData.UpdateByNonQuery(sqlUpdate);
                                    if (_dbData.HasErrors)
                                    {
                                        _dbData.RollbackMultiTrans();
                                        MessageBox.Show("Lỗi xảy ra!");
                                        return;
                                    }
                                    break;
                                }
                            }
                        }
                        
                    }
                }
                if (!_dbData.HasErrors)
                {
                    _dbData.EndMultiTrans();
                    MessageBox.Show("Hoàn thành");
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
