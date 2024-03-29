using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTControl;
using CDTDatabase;
using CDTLib;
namespace CDTSystem
{
    public partial class fImExcelMT : DevExpress.XtraEditors.XtraForm
    {
        public fImExcelMT()
        {
            InitializeComponent();
        }
        ImportExcel IEx;
        DataTable MapStruct ;
        DataTable MapStructDT;
        DataTable MapStructVAT;
        Database _db = Database.NewDataDatabase();
        Database _dbStruct = Database.NewStructDatabase();
        DataTable dmBang;
        DataTable dataType;
        DataTable ConflicID;
        string MTName=null;
        string DtName;
        string VatName;
        string MTPk = null;
        string DtPk = null;
        string soctField;
        string SoHDField=null;
        private void fImExcel_Load(object sender, EventArgs e)
        {
            dmBang = _dbStruct.GetDataTable("select * from systable where type=3 and sysPackageID=" + Config.GetValue("sysPackageID").ToString() );
            if (dmBang == null) return;
            this.gTable.Properties.DataSource = dmBang;
            gTable.Properties.View.BestFitColumns();
            dataType=_dbStruct.GetDataTable("select * from dataType");
            RiType.DataSource=dataType;
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "AllExel|*.xls;*.xlsx";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                tFileName.EditValue = dialog.FileName;
                IEx = new ImportExcel(dialog.FileName);
                List<string> sheets = IEx.GetSheets();
                lSheet.Properties.Items.Clear();
                lSheet.Properties.Items.AddRange(sheets.ToArray());
            }
        }
        
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            //Kiểm tra những mã vật tư, mã khách xem đã có trong danh mục chưa, nếu chưa thì thông báo để họ add vào
            ConflicID = new DataTable();
            ConflicID.Columns.Add("TableName", typeof(string));
            ConflicID.Columns.Add("ID", typeof(string));
            foreach (DataRow dr in MapStruct.Rows)
            {
                if (dr["type"].ToString() == "0" || dr["type"].ToString() == "3" || dr["type"].ToString() == "6")
                    MTPk = dr["FieldName"].ToString();
                if (dr["SoCT"] != DBNull.Value && dr["SoCT"].ToString().ToUpper() == "TRUE")
                    soctField = dr["ColName"].ToString();
                if (dr["refTable"] != DBNull.Value && dr["ColName"] != DBNull.Value && dr["ColName"].ToString() != "")
                {
                    string pk="";
                    string sql = "select pk from systable where tableName='" + dr["refTable"].ToString() + "'";
                    object o = _dbStruct.GetValue(sql);
                    if (o == null) continue;
                    pk = o.ToString();
                    sql = "select count(*) from " + dr["refTable"].ToString() + " where " + pk + "='" ;
                    foreach (DataRow drDB in IEx.Db.Rows)
                    {
                        string sql1 = sql + drDB[dr["ColName"].ToString()].ToString() + "'";
                        o = _db.GetValue(sql1);
                        if (o != null && o.ToString() == "0")
                        {
                            DataRow[] ldr = ConflicID.Select("tableName='" + dr["refTable"].ToString() + "' and  ID='" + drDB[dr["ColName"].ToString()].ToString() + "'");
                            if (ldr.Length == 0)
                            {
                                DataRow drConflic = ConflicID.NewRow();
                                drConflic["TableName"] = dr["refTable"];
                                drConflic["ID"] = drDB[dr["ColName"].ToString()];
                                ConflicID.Rows.Add(drConflic);
                            }
                        }
                    }
                }
            }
            foreach (DataRow dr1 in MapStructDT.Rows)
            {
                if (dr1["type"].ToString() == "0" || dr1["type"].ToString() == "3" || dr1["type"].ToString() == "6")
                    DtPk = dr1["FieldName"].ToString();
                if (dr1["refTable"] != DBNull.Value && dr1["ColName"] != DBNull.Value && dr1["ColName"].ToString() != "")
                {
                    string pk = "";
                    string sql = "select pk from systable where tableName='" + dr1["refTable"].ToString() + "'";
                    object o = _dbStruct.GetValue(sql);
                    if (o == null) continue;
                    pk = o.ToString();
                    sql = "select count(*) from " + dr1["refTable"].ToString() + " where " + pk + "='";
                    foreach (DataRow drDB in IEx.Db.Rows)
                    {
                        string sql1 = sql + drDB[dr1["ColName"].ToString()].ToString() + "'";
                        o = _db.GetValue(sql1);
                        if (o != null && o.ToString() == "0")
                        {
                            DataRow[] ldr = ConflicID.Select("tableName='" + dr1["refTable"].ToString() + "' and  ID='" + drDB[dr1["ColName"].ToString()].ToString() + "'");
                            if (ldr.Length == 0)
                            {
                                DataRow drConflic = ConflicID.NewRow();
                                drConflic["TableName"] = dr1["refTable"];
                                drConflic["ID"] = drDB[dr1["ColName"].ToString()];
                                ConflicID.Rows.Add(drConflic);
                            }
                        }
                    }
                }
            }
            if (isHasVat)
            {
                foreach (DataRow dr2 in MapStructVAT.Rows)
                    if (dr2["SoCT"] != DBNull.Value && dr2["SoCT"].ToString().ToUpper() == "TRUE")
                        SoHDField = dr2["ColName"].ToString();
            }
            gridControl4.DataSource = ConflicID;
            if (ConflicID.Rows.Count == 0) simpleButton2.Enabled = true;
            else simpleButton2.Enabled = false;
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {

            string sqlUpdate;
            if (ConflicID==null || ConflicID.Rows.Count > 0)
            {
                MessageBox.Show("Dữ liệu chưa hợp lệ");
                return;
            }
            if (gTable.EditValue == null)
            {
                MessageBox.Show("Không có bảng dữ liệu");
                return;
            }
            if (MTName == null) return;
            if (IEx == null || IEx.Db == null)
            {
                MessageBox.Show("Không có dữ liệu");
                return;
            }
            if (MapStruct == null || MapStructDT == null)
            {
                MessageBox.Show("Không có cấu trúc dữ liệu");
                return;
            }
            if (soctField == null) return;
            _db.BeginMultiTrans();
            string sql;
            Guid MTID;
            MTID = Guid.NewGuid();
            string soct = "";
            string sohd = "";
            foreach (DataRow dr in IEx.Db.Rows)
            {
                //Insert MT - 
                if (dr[soctField].ToString() != "" || soct != dr[soctField].ToString())
                {
                    //Thuc hien insert vao MT
                    MTID = Guid.NewGuid();
                    string sqlMT = CreateMTSql(MTName, dr, MTID);
                    _db.UpdateByNonQuery(sqlMT);
                    if (_db.HasErrors)
                    {
                        _db.RollbackMultiTrans();
                        MessageBox.Show("Lỗi xảy ra");
                        return;
                    }
                }
                
                //thuc hien insert vao DT
                string sqlDT = CreateDTsql(DtName, dr, MTID);
                _db.UpdateByNonQuery(sqlDT);
                if (_db.HasErrors)
                {
                    _db.RollbackMultiTrans();
                    MessageBox.Show("Lỗi xảy ra");
                    return;
                }
                soct = dr[soctField].ToString();
                if (SoHDField != null && isHasVat && dr[SoHDField] != DBNull.Value && dr[SoHDField].ToString() != "")
                {
                    string sqlVat = CreateVATsql(VatName, dr, MTID);
                    _db.UpdateByNonQuery(sqlVat);
                    if (_db.HasErrors)
                    {
                        _db.RollbackMultiTrans();
                        MessageBox.Show("Lỗi xảy ra");
                        return;
                    }
                }
            }
            if (!_db.HasErrors)
            {
                _db.EndMultiTrans();
                MessageBox.Show("Update thành công");
            }
            else
            {
                _db.RollbackMultiTrans();
            }
        }


        private string CreateMTSql(string tableName,DataRow RowData, Guid MTID)
        {
            string sql = "insert into " + tableName + "(" + MTPk + ",";
            string values = " values ('" + MTID.ToString() + "',";

            foreach (DataRow dr in MapStruct.Rows)
            {
                int Type = int.Parse(dr["Type"].ToString());
                if (Type == 3 || Type == 6) continue;
                if (dr["fieldName"].ToString() != string.Empty )
                {
                    
                    if ((dr["ColName"] == DBNull.Value || dr["ColName"].ToString() == string.Empty) && dr["AllowNull"].ToString() == "False" && dr["DefaultValue"] == DBNull.Value) continue;
                    string note = getnote(dr["Type"].ToString());
                    if (dr["ColName"] != DBNull.Value && dr["ColName"].ToString() != string.Empty)
                    {
                        sql += dr["fieldName"].ToString() + ",";
                        if (dr["Type"].ToString() == "9")
                        {
                            values += (note == "'" ? "N" + note : note) + DateTime.Parse(RowData[dr["ColName"].ToString()].ToString()).ToString("MM/dd/yyyy") + note + ",";
                        }
                        else if (dr["Type"].ToString() == "8")
                        {
                            if (RowData[dr["ColName"].ToString()] == DBNull.Value)
                            {
                                RowData[dr["ColName"].ToString()] = 0;
                            }
                            values += (note == "'" ? "N" + note : note) + double.Parse(RowData[dr["ColName"].ToString()].ToString()).ToString("###########0.##") + note + ",";
                        }
                        else
                        {
                            values += (note == "'" ? "N" + note : note) + RowData[dr["ColName"].ToString()].ToString() + note + ",";
                        }
                    }
                    else if (dr["DefaultValue"] != DBNull.Value)
                    {
                        sql += dr["fieldName"].ToString() + ",";
                        values += ((Type == 1 || Type == 2) ? "N" + note : note) + dr["DefaultValue"].ToString() + note + ",";
                    }
                }                
            }
            sql = sql.Substring(0, sql.Length - 1) +") " + values.Substring(0, values.Length - 1) + ")";
            return sql;
        }
        private string CreateDTsql(string tableName, DataRow RowData, Guid MTID)
        {
            string sql = "insert into " + tableName + "(" + MTPk + "," + DtPk + ",";
            string values = " values ('" + MTID.ToString() + "','" + Guid.NewGuid().ToString() + "',";

            foreach (DataRow dr in MapStructDT.Rows)
            {
                int Type = int.Parse(dr["Type"].ToString());
                if (Type == 3 || Type == 6) continue;
                if (dr["fieldName"].ToString() != string.Empty)
                {

                    if ((dr["ColName"] == DBNull.Value || dr["ColName"].ToString() == string.Empty) && dr["AllowNull"].ToString() == "False" && dr["DefaultValue"] == DBNull.Value) continue;
                    string note = getnote(dr["Type"].ToString());
                    if (dr["ColName"] != DBNull.Value && dr["ColName"].ToString() != string.Empty)
                    {
                        sql += dr["fieldName"].ToString() + ",";
                        values += (note == "'" ? "N" + note : note) + RowData[dr["ColName"].ToString()].ToString() + note + ",";
                    }
                    else if (dr["DefaultValue"] != DBNull.Value)
                    {
                        sql += dr["fieldName"].ToString() + ",";
                        if (dr["Type"].ToString() == "9")
                        {
                            values += (note == "'" ? "N" + note : note) + DateTime.Parse(dr["DefaultValue"].ToString()).ToString("MM/dd/yyyy") + note + ",";
                        }
                        else if (dr["Type"].ToString() == "8")
                        {
                            values += (note == "'" ? "N" + note : note) + double.Parse(dr["DefaultValue"].ToString()).ToString("###########0.##") + note + ",";
                        }
                        else
                        {
                            values += ((Type == 1 || Type == 2) ? "N" + note : note) + dr["DefaultValue"].ToString() + note + ",";
                        }
                    }
                }
            }
            sql = sql.Substring(0, sql.Length - 1) + ") " + values.Substring(0, values.Length - 1) + ")";
            return sql;
        }
        private string CreateVATsql(string tableName, DataRow RowData, Guid MTID)
        {
            string sql = "insert into " + tableName + "(MTID," ;
            string values = " values ('" + MTID.ToString() + "',";

            foreach (DataRow dr in MapStructVAT.Rows)
            {
                int Type = int.Parse(dr["Type"].ToString());
                if (Type == 3 || Type == 6) continue;
                if (dr["fieldName"].ToString() != string.Empty)
                {

                    if ((dr["ColName"] == DBNull.Value || dr["ColName"].ToString() == string.Empty) && dr["AllowNull"].ToString() == "False" && dr["DefaultValue"] == DBNull.Value) continue;
                    string note = getnote(dr["Type"].ToString());
                    if (dr["ColName"] != DBNull.Value && dr["ColName"].ToString() != string.Empty)
                    {
                        sql += dr["fieldName"].ToString() + ",";
                        if (dr["Type"].ToString() == "9")
                        {
                            values += (note == "'" ? "N" + note : note) + DateTime.Parse(RowData[dr["ColName"].ToString()].ToString()).ToString("MM/dd/yyyy") + note + ",";
                        }
                        else if (dr["Type"].ToString() == "8")
                        {
                            if (RowData[dr["ColName"].ToString()] == DBNull.Value)
                            {
                                RowData[dr["ColName"].ToString()] = 0;
                            }
                            values += (note == "'" ? "N" + note : note) + double.Parse(RowData[dr["ColName"].ToString()].ToString()).ToString("###########0.##") + note + ",";
                        }
                        else
                        {
                        values += (note == "'" ? "N" + note : note) + RowData[dr["ColName"].ToString()].ToString() + note + ",";
                        }
                    }
                    else if (dr["DefaultValue"] != DBNull.Value)
                    {
                        sql += dr["fieldName"].ToString() + ",";
                        
                            values += ((Type == 1 || Type == 2) ? "N" + note : note) + dr["DefaultValue"].ToString() + note + ",";
                        
                    }
                }
            }
            sql = sql.Substring(0, sql.Length - 1) + ") " + values.Substring(0, values.Length - 1) + ")";
            return sql;
        }
        private string getnote(string p)
        {
            DataRow[] ldr = dataType.Select("DataTypeID=" + p);
            if (ldr.Length < 1) return "";
            if (ldr[0]["Note"] == DBNull.Value) return "";
            return ldr[0]["Note"].ToString();
        }


        private void lSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lSheet.EditValue == null) return;
            List<string> cols = IEx.GetCol(lSheet.EditValue.ToString());
            if (cols == null) return;
            foreach (DataRow dr in MapStruct.Rows)
            {
                if (cols.Exists(x => x.ToString().ToUpper() == dr["FieldName"].ToString().ToUpper()))
                    dr["ColName"] = dr["FieldName"].ToString();
                if (dr["FieldName"].ToString().ToUpper() == "SOCT") dr["SoCT"] = true;
            }
            RiCom.Items.AddRange(cols.ToArray());
            foreach (DataRow dr in MapStructDT.Rows)
            {
                if (cols.Exists(x => x.ToString().ToUpper() == dr["FieldName"].ToString().ToUpper()))
                    dr["ColName"] = dr["FieldName"].ToString();
            }
            if (MapStructVAT != null)
            {
                foreach (DataRow dr in MapStructVAT.Rows)
                {
                    if (cols.Exists(x => x.ToString().ToUpper() == dr["FieldName"].ToString().ToUpper()))
                        dr["ColName"] = dr["FieldName"].ToString();
                }
            }
            riCom1.Items.AddRange(cols.ToArray());
            RiCom2.Items.AddRange(cols.ToArray());
        }
       
        DataTable dmField;
        DataTable dmFieldDetail;
        DataTable dmFieldVat;
        bool isHasVat = false;
        private void gTable_EditValueChanged(object sender, EventArgs e)
        {
            string sql = "select tableName from systable where sysTableID=" + gTable.EditValue.ToString();
            DtName = (_dbStruct.GetValue(sql)).ToString();
            sql = "select c.*,b.TableName from systable a inner join systable b on a.masterTable=b.TableName inner join sysfield c on b.systableid=c.systableid where a.systableid=" + gTable.EditValue.ToString() + " order by TabIndex";

            dmField = _dbStruct.GetDataTable(sql);
            if (dmField.Rows.Count > 0) MTName = dmField.Rows[0]["TableName"].ToString();
            sql = "select * from sysfield where systableid=" + gTable.EditValue.ToString() + " order by TabIndex";
            dmFieldDetail = _dbStruct.GetDataTable(sql);
            sql = "select b.*, c.TableName from  sysdetail a inner join sysfield b on a.sysDetailid=b.systableid inner join sysTable c on a.sysDetailID=c.sysTableID where a.systableid=" + gTable.EditValue.ToString() + " order by TabIndex";

            dmFieldVat = _dbStruct.GetDataTable(sql);

            if (dmFieldVat.Rows.Count > 0)
            {
                isHasVat = true;
                VatName = dmFieldVat.Rows[0]["TableName"].ToString();
            }
            if (dmField == null) return;
            MapStruct = new DataTable();
            MapStruct.Columns.Add("FieldName", typeof(string));
            MapStruct.Columns.Add("Type", typeof(int));
            MapStruct.Columns.Add("ColName", typeof(string));
            MapStruct.Columns.Add("LabelName", typeof(string));
            MapStruct.Columns.Add("RefTable", typeof(string));
            MapStruct.Columns.Add("DefaultValue", typeof(string));
            MapStruct.Columns.Add("AllowNull", typeof(bool));
            MapStruct.Columns.Add("SoCT", typeof(bool));
            foreach (DataRow rF in dmField.Rows)
            {
                DataRow dr = MapStruct.NewRow();
                dr["FieldName"] = rF["FieldName"];
                if (rF["RefTable"] != DBNull.Value)
                    dr["RefTable"] = rF["RefTable"];
                dr["Type"] = rF["Type"];
                dr["DefaultValue"] = rF["DefaultValue"];
                dr["AllowNull"] = rF["AllowNull"].ToString()=="0"? false:true;
                dr["LabelName"] = rF["LabelName"].ToString();
                MapStruct.Rows.Add(dr);
            }                
            
            gridControl1.DataSource = MapStruct;
            gridControl1.DataMember = MapStruct.TableName;


            MapStructDT = MapStruct.Clone();
            foreach (DataRow rF in dmFieldDetail.Rows)
            {
                DataRow dr = MapStructDT.NewRow();
                dr["FieldName"] = rF["FieldName"];
                if (rF["RefTable"] != DBNull.Value)
                    dr["RefTable"] = rF["RefTable"];
                dr["Type"] = rF["Type"];
                dr["DefaultValue"] = rF["DefaultValue"];
                dr["LabelName"] = rF["LabelName"];
                dr["AllowNull"] = rF["AllowNull"].ToString() == "0" ? false : true;
                MapStructDT.Rows.Add(dr);
            }
            gridControl2.DataSource = MapStructDT;
            gridControl2.DataMember = MapStructDT.TableName;
            if (isHasVat)
            {
                MapStructVAT = MapStruct.Clone();
                foreach (DataRow rF in dmFieldVat.Rows)
                {
                    DataRow dr = MapStructVAT.NewRow();
                    dr["FieldName"] = rF["FieldName"];
                    if (rF["RefTable"] != DBNull.Value)
                        dr["RefTable"] = rF["RefTable"];
                    dr["Type"] = rF["Type"];
                    dr["DefaultValue"] = rF["DefaultValue"];
                    dr["LabelName"] = rF["LabelName"];
                    dr["AllowNull"] = rF["AllowNull"].ToString() == "0" ? false : true;
                    MapStructVAT.Rows.Add(dr);
                }
                gridControl3.DataSource = MapStructVAT;
                gridControl3.DataMember = MapStructVAT.TableName;
            }
        }

        private void tFileName_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            if (lSheet.EditValue == null) return;
            List<string> cols = IEx.GetCol(lSheet.EditValue.ToString());
            if (cols == null) return;
            RiCom.Items.Clear();
            riCom1.Items.Clear();
            RiCom2.Items.Clear();
            RiCom.Items.AddRange(cols.ToArray());
            riCom1.Items.AddRange(cols.ToArray());
            RiCom2.Items.AddRange(cols.ToArray());
        }




    }
}