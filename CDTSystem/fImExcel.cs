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
    public partial class fImExcel : DevExpress.XtraEditors.XtraForm
    {
        public fImExcel()
        {
            InitializeComponent();
        }
        ImportExcel IEx;
        DataTable MapStruct ;
        Database _db = Database.NewDataDatabase();
        Database _dbStruct = Database.NewStructDatabase();
        DataTable dmBang;
        DataTable dataType;
        private void fImExcel_Load(object sender, EventArgs e)
        {
            dmBang = _dbStruct.GetDataTable("select * from systable where (type=1 or type=2) and sysPackageID=" + Config.GetValue("sysPackageID").ToString() );
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
                if (sheets.Count == 1 && sheets[0] == "FileOpening")
                {
                    MessageBox.Show("File is open, please close before importing");
                    return;
                }
                lSheet.Properties.Items.Clear();
                lSheet.Properties.Items.AddRange(sheets.ToArray());
            }

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            string sqlUpdate;
            string sqlInsert;
            if(gTable.EditValue==null) return;
            string tableName=gTable.EditValue.ToString();
            DataRow[] lTb = dmBang.Select("systableID=" + tableName);
            if (lTb.Length < 1) return;
            tableName = lTb[0]["TableName"].ToString();
            
            if (IEx == null || IEx.Db == null) return;
            if (MapStruct == null) return;
            getPkInfor();
            _db.BeginMultiTrans();
            string sql;
            if (ckDelete.Checked && !cUpdate.Checked)
            {
                sql = "delete " + tableName;
                _db.UpdateByNonQuery(sql);
            }
            foreach (DataRow dr in IEx.Db.Rows)
            {
                                
                    
                   // if (dr[PkValueName].ToString() == string.Empty) continue;
                    if (kiemtraKhoachinh(tableName, dr))
                    {
                        try
                        {
                            if (PkValueName == string.Empty) continue;
                            sqlUpdate = UpdateSql(tableName, dr);
                            _db.UpdateByNonQuery(sqlUpdate);
                            if (_db.HasErrors) { _db.RollbackMultiTrans(); return; }
                        }
                        catch { }
                    }
                    else
                    {
                        sqlInsert = CreateSql(tableName, dr);    
                        _db.UpdateByNonQuery(sqlInsert);
                        if (_db.HasErrors) { _db.RollbackMultiTrans(); return; }

                    }
               
            }
            if (!_db.HasErrors)
            {
                _db.EndMultiTrans();
                MessageBox.Show("Update th�nh c�ng");
            }
        }

        private bool kiemtraKhoachinh(string tableName, DataRow dr)
        {
            string note="";
            if (PkValueName == string.Empty) return false;
            if(PkType==0 ||PkType==6) note="'";
            string sql = "select count(*) from " + tableName + " where " + PkName + " =" + note + dr[PkValueName] + note;
            DataTable tbTmp = _db.GetDataTable(sql);
            if (tbTmp.Rows.Count == 0) return false;
            else if (tbTmp.Rows[0][0] == DBNull.Value) return false;
            else if (int.Parse(tbTmp.Rows[0][0].ToString()) == 0) return false;
            else return true;

        }

        private string UpdateSql(string tableName, DataRow RowData)
        {
            string sql = "Update " + tableName + " set ";
            string where="";
            string note="";

            foreach (DataRow dr in MapStruct.Rows)
            {
                int Type = int.Parse(dr["Type"].ToString());


                List<int> chars = new List<int> { 0, 1, 2 };
                List<int> unid = new List<int> { 6, 7, 15 };

                if (Type == 0 || Type == 3 || Type == 6)
                {
                    note=getnote(dr["Type"].ToString());
                    where = " where " + PkName + " =" + (chars.Contains(Type) ? "N" : "") + note + RowData[PkValueName] + note;
                    continue;
                }
                if (dr["fieldName"].ToString() != string.Empty && dr["ColName"] != DBNull.Value && dr["ColName"].ToString() != string.Empty)
                {
                    if (RowData[dr["ColName"].ToString()].ToString() == "" && note == "")
                        continue;
                    note = getnote(dr["Type"].ToString());
                    sql += dr["fieldName"].ToString() + " = " + (chars.Contains(Type) ? "N" : "")  + note + RowData[dr["ColName"].ToString()].ToString() + note + ",";
                }
            }
            sql = sql.Substring(0, sql.Length - 1);
            return sql + where  ;
            
        }
        string PkName="";
        int PkType=-1;
        string PkValueName = "";
        private void getPkInfor()
        {
            DataRow[] ldr = MapStruct.Select("Type=0 or Type=3 or Type=6");
            if (ldr.Length < 1) return;
            else
            {
                PkName = ldr[0]["FieldName"].ToString();
                PkType = int.Parse(ldr[0]["Type"].ToString());
                PkValueName = ldr[0]["ColName"].ToString();
            }
        }


        private string CreateSql(string tableName,DataRow RowData)
        {
            string sql = "insert into " + tableName + "(";
            string values = " values (";

            foreach (DataRow dr in MapStruct.Rows)
            {
                int Type = int.Parse(dr["Type"].ToString());
                if (Type == 3 ) continue;
                if (dr["fieldName"].ToString() != string.Empty)
                {

                    if ((dr["ColName"] == DBNull.Value || dr["ColName"].ToString() == string.Empty) && dr["AllowNull"].ToString() == "False" && dr["DefaultValue"] == DBNull.Value) continue;
                    string note = getnote(dr["Type"].ToString());
                    List<int> chars = new List<int> { 0, 1, 2 };
                    List<int> unid = new List<int> { 6, 7, 15 };
                    if (dr["ColName"] == DBNull.Value || dr["ColName"].ToString()==string.Empty) continue;
                    if (Type == 1 && RowData[dr["ColName"].ToString()].ToString() == "")
                        RowData[dr["ColName"].ToString()] = DBNull.Value;
                    if (unid.Contains(Type) && note == "'" && RowData[dr["ColName"].ToString()].ToString() == "")
                        RowData[dr["ColName"].ToString()] = DBNull.Value;

                    else if (dr["ColName"] != DBNull.Value && dr["ColName"].ToString() != string.Empty)
                    {
                        if (RowData[dr["ColName"].ToString().Trim()].ToString() == "" && note == "")
                            continue;
                        sql += dr["fieldName"].ToString() + ",";
                        if (RowData[dr["ColName"].ToString().Trim()] == DBNull.Value || RowData[dr["ColName"].ToString().Trim()].ToString().Trim() == string.Empty)
                        {
                            values += "NULL,";
                        }
                        else
                            values += (chars.Contains(Type) ? "N" : "") + note + RowData[dr["ColName"].ToString().Trim()].ToString() + note + ",";
                    }
                    else if (dr["DefaultValue"] != DBNull.Value)
                    {
                        sql += dr["fieldName"].ToString() + ",";
                        values += (chars.Contains(Type) ? "N" : "") + note + dr["DefaultValue"].ToString() + note + ",";
                    }
                }
                
            }
            sql = sql.Substring(0, sql.Length - 1) +") " + values.Substring(0, values.Length - 1) + ")";
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
            if (lSheet.EditValue == null || lSheet.EditValue.ToString()==string.Empty) return;
            List<string> cols= IEx.GetCol(lSheet.EditValue.ToString());
            if (cols == null) return;
            RiCom.Items.AddRange(cols.ToArray());
            foreach (DataRow dr in MapStruct.Rows)
            {
                if (cols.Exists(x => x.ToString().ToUpper().Trim() == dr["FieldName"].ToString().ToUpper().Trim()))
                    dr["ColName"] = dr["FieldName"].ToString().Trim();
                else if (cols.Exists(x => x.ToString().ToUpper().Trim() == dr["LabelName"].ToString().ToUpper().Trim()))
                    dr["ColName"] = dr["LabelName"].ToString().Trim();
            }

            //foreach (DataRow dr in MapStruct.Rows)
            //{
            //    if (cols.Exists(x => x.ToString().ToUpper() == dr["FieldName"].ToString().ToUpper()))
            //        dr["ColName"] = dr["FieldName"].ToString();
            //    else if (cols.Exists(x => x.ToString().ToUpper() == dr["LabelName"].ToString().ToUpper()))
            //        dr["ColName"] = dr["LabelName"].ToString();
            //}
        }
        DataTable dmField;
        private void gTable_EditValueChanged(object sender, EventArgs e)
        {
            dmField = _dbStruct.GetDataTable("select * from sysfield where  Type<>3 and sysTableid=" + gTable.EditValue.ToString() + " order by TabIndex");
            if (dmField == null) return;
            MapStruct = new DataTable();
            MapStruct.Columns.Add("FieldName", typeof(string));
            MapStruct.Columns.Add("Type", typeof(int));
            MapStruct.Columns.Add("ColName", typeof(string));
            MapStruct.Columns.Add("LabelName", typeof(string));
            //DataColumn cdef=new DataColumn("DefaultValue", typeof(string));
            //MapStruct.Columns.Add(cdef);
            MapStruct.Columns.Add("DefaultValue", typeof(string));
            MapStruct.Columns.Add("AllowNull", typeof(bool));
            foreach (DataRow rF in dmField.Rows)
            {
                DataRow dr = MapStruct.NewRow();
                dr["FieldName"] = rF["FieldName"];
                dr["Type"] = rF["Type"];
                dr["DefaultValue"] = rF["DefaultValue"];
                dr["AllowNull"] = rF["AllowNull"].ToString() == "0" ? false : true;
                dr["LabelName"] = rF["LabelName"].ToString();
                MapStruct.Rows.Add(dr);
            }                
            
            gridControl1.DataSource = MapStruct;
            gridControl1.DataMember = MapStruct.TableName;
        }

        private void tFileName_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}