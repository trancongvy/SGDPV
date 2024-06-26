using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTDatabase;
using CDTLib;
using System.Windows.Forms;
namespace CDTControl
{
    public class DataTransfer
    {
        private Database _dataDb;
        private string _mtTableID;
        private string _pk;
        DataTable _blConfig;
        Database db = Database.NewStructDatabase();
        public DataTransfer(Database dataDb, string mtTableID, string pk)
        {
            _dataDb = dataDb;
            if(CDTLib.Config.GetValue("sysPackageID").ToString()=="5")
                db=_dataDb;
            _mtTableID = mtTableID;
            _pk = pk;
            _blConfig = GetDtConfig();
        }

        public DataTable GetDtConfig()
        {
            string sysPackageID = CDTLib.Config.GetValue("sysPackageID").ToString();
            //Database db = Database.NewStructDatabase();
            string s = "select bl.*, tb1.TableName as blTableName, tb2.TableName as mtTableName,  tb2.Pk as Pk, " +
                " tb3.TableName as dtTableName,tb4.TableName as reTableName,tb3.Pk as dtPk ,tb4.pk as rePk " +
                " from sysDataConfig bl inner join  sysTable tb1 on bl.sysTableID = tb1.sysTableID left join  sysTable tb2 on bl.mtTableID = tb2.sysTableID " +
                " left join sysTable tb3 on bl.dtTableID = tb3.sysTableID left join  sysTable as tb4 on bl.reTable = tb4.systableid " +
                " where tb1.sysPackageID = " + sysPackageID + " and bl.mtTableID = " + _mtTableID;
            return (db.GetDataTable(s));
        }

        public DataTable GetDtConfigDetail(string blConfigID)
        {
            string s = "select bld.*, sf1.FieldName as blFieldName, sf2.FieldName as mtFieldName, sf3.FieldName as dtFieldName " +
                " from sysDataConfigDt bld left join  sysField sf1 on bld.blFieldID = sf1.sysFieldID left join  sysField sf2 on bld.mtFieldID = sf2.sysFieldID left join sysField sf3 on bld.dtFieldID = sf3.sysFieldID" +
                " where bld.blConfigID = " + blConfigID ;
            //Database db = Database.NewStructDatabase();
            return (db.GetDataTable(s));
        }

        private string GenInsertString(DataRow drConfig, DataTable blConfigDetail, string Pk, string PkValue, string dtPkValue)
        {
            string blTableName = drConfig["blTableName"].ToString();
            string mtTableName = drConfig["mtTableName"].ToString();
            string dtTableName = drConfig["dtTableName"].ToString();
            string reTableName=drConfig["reTableName"].ToString();
             string dtPkRef = Pk;
             if (dtTableName != null && dtTableName != string.Empty)
             {
                 string sqltmp = "select fieldname from sysfield where systableid= " + drConfig["dtTableID"].ToString() + " and refTable='" + mtTableName + "'";
                 DataTable tabletmp = db.GetDataTable(sqltmp);
                 if (tabletmp != null && tabletmp.Rows.Count > 0)
                 {
                     dtPkRef = tabletmp.Rows[0]["FieldName"].ToString();
                 }
             }
           
           
            string dtPk = drConfig["dtPk"].ToString();
            string reKey = drConfig["reKey"].ToString();

            string nhomDk = drConfig["NhomDk"].ToString();
            string blFieldNames = string.Empty;
            string mtFieldNames = string.Empty;
            string mtdtFieldNames = string.Empty;
            
            string sql;
            string where = " where " + mtTableName + "." + Pk + " = '" + PkValue + "'";
            if (dtTableName != string.Empty)
            {
                foreach (DataRow dr in blConfigDetail.Rows)
                {
                    if (dr["mtFieldName"].ToString() == string.Empty 
                        && dr["dtFieldName"].ToString() == string.Empty
                         && dr["Formula"].ToString() == string.Empty)
                        continue;
                    blFieldNames += dr["blFieldName"].ToString();
                    if (dr["mtFieldName"].ToString() != string.Empty)
                        mtdtFieldNames += mtTableName + "." + dr["mtFieldName"].ToString();
                    else
                    {
                        if (dr["dtFieldName"].ToString() != string.Empty)
                            mtdtFieldNames += dtTableName + "." + dr["dtFieldName"].ToString();
                        else
                            mtdtFieldNames += dr["Formula"].ToString();
                    }
                    blFieldNames += ",";
                    mtdtFieldNames += ",";
                }
                if (nhomDk != string.Empty)
                {
                    blFieldNames += "NhomDk";
                    mtdtFieldNames += "'" + nhomDk + "'";
                }
                else
                {
                    blFieldNames = blFieldNames.Substring(0, blFieldNames.Length - 1);
                    mtdtFieldNames = mtdtFieldNames.Substring(0, mtdtFieldNames.Length - 1);
                }
                if (reTableName == string.Empty )
                {
                    sql = "insert into " + blTableName + "(" + blFieldNames + ")" +
                        " select " + mtdtFieldNames + " from " + mtTableName + " inner join " + dtTableName +
                        " on " + mtTableName + "." + Pk + " = " + dtTableName + "." + dtPkRef + where;
                        
                }
                else 
                {
                    sql = "insert into " + blTableName + "(" + blFieldNames + ")" +
                        " select " + mtdtFieldNames + " from " + mtTableName + " inner join " + dtTableName +
                        " on " + mtTableName + "." + Pk + " = " + dtTableName + "." + Pk +
                        " inner join " + reTableName + " on " + dtTableName + "." + reKey + " = " + reTableName + "." + reKey + where;
                        //" where " + mtTableName + "." + Pk + " = '" + PkValue + "'";
                }
                
            }
            else
            {
                foreach (DataRow dr in blConfigDetail.Rows)
                {
                    if (dr["mtFieldName"].ToString() == string.Empty && dr["Formula"].ToString() == string.Empty)
                        continue;
                    blFieldNames += dr["blFieldName"].ToString();
                    if (dr["mtFieldName"].ToString() != string.Empty)
                        mtFieldNames += mtTableName + "." + dr["mtFieldName"].ToString();
                    else
                        mtFieldNames += dr["Formula"].ToString();
                    blFieldNames += ",";
                    mtFieldNames += ",";
                }
                if (nhomDk != string.Empty)
                {
                    blFieldNames += "NhomDk";
                    mtFieldNames += "'" + nhomDk + "'";
                }
                else
                {
                    blFieldNames = blFieldNames.Substring(0, blFieldNames.Length - 1);
                    mtFieldNames = mtFieldNames.Substring(0, mtFieldNames.Length - 1);
                }
                    if (reTableName == string.Empty)
                    {
                        sql = "insert into " + blTableName + "(" + blFieldNames + ")" +
                            " select " + mtFieldNames + " from " + mtTableName + where;
                           // " where " + mtTableName + "." + Pk + " = '" + PkValue + "'";
                    }
                    else
                    {
                        sql = "insert into " + blTableName + "(" + blFieldNames + ")" +
                            " select " + mtFieldNames + " from " + mtTableName +
                             " inner join " + reTableName + " on " + mtTableName + "." + Pk + " = " + reTableName + "." + reKey + where;
                           // " where " + mtTableName + "." + Pk + " = '" + PkValue + "'";
                    }
            }
            if (dtPkValue != string.Empty)
                sql += " and " + dtPk + " = '" + dtPkValue + "'";
            string condition = drConfig["Condition"].ToString();
            if (condition != string.Empty)
                sql += " and (" + condition + ")";
            if (drConfig.Table.Columns.Contains("SortOrder") && drConfig["SortOrder"] != DBNull.Value && drConfig["SortOrder"].ToString() != string.Empty)
            {
                string order = drConfig["SortOrder"].ToString();
                sql += " order by " + order;
            }
         
            return sql;
        }

        private string GenUpdateString(DataRow drConfig, DataTable blConfigDetail, string Pk, string PkValue, string dtPkValue, ref string checkCond)
        {
            string blTableName = drConfig["blTableName"].ToString();
            string mtTableName = drConfig["mtTableName"].ToString();
            string dtTableName = drConfig["dtTableName"].ToString();
            string mtIDName = drConfig["RootIDName"].ToString();
            string mtdtID = drConfig["DTID"].ToString();
            string reTableName = drConfig["reTableName"].ToString();

            string dtPk = drConfig["dtPk"].ToString();
            string reKey = drConfig["reKey"].ToString();
            string nhomDk = drConfig["NhomDk"].ToString();
            string blFieldNames = string.Empty;
            string mtFieldNames = string.Empty;
            string mtdtFieldNames = string.Empty;
            string sql;
            if (dtTableName != string.Empty)
            {
                foreach (DataRow dr in blConfigDetail.Rows)
                {
                    if ((dr["mtFieldName"].ToString() == string.Empty || dr["mtFieldName"].ToString().ToUpper() == _pk.ToUpper())
                        && (dr["dtFieldName"].ToString() == string.Empty || dr["dtFieldName"].ToString().ToUpper() == dtPk.ToUpper())
                         && dr["Formula"].ToString() == string.Empty)
                        continue;
                    blFieldNames += dr["blFieldName"].ToString();
                    if (dr["mtFieldName"].ToString() != string.Empty)
                        mtdtFieldNames = mtTableName + "." + dr["mtFieldName"].ToString();
                    else
                    {
                        if (dr["dtFieldName"].ToString() != string.Empty)
                            mtdtFieldNames = dtTableName + "." + dr["dtFieldName"].ToString();
                        else
                            mtdtFieldNames = dr["Formula"].ToString();
                    }
                    blFieldNames += " = " + mtdtFieldNames + ",";
                    //mtdtFieldNames += ",";
                }
                if (nhomDk != string.Empty)
                {
                    blFieldNames += "NhomDk = '" + nhomDk + "'";
                    //mtdtFieldNames += "'" + nhomDk + "'";
                }
                else
                {
                    blFieldNames = blFieldNames.Substring(0, blFieldNames.Length - 1);
                    //mtdtFieldNames = mtdtFieldNames.Substring(0, mtdtFieldNames.Length - 1);
                }
                if (reTableName == string.Empty)
                {
                    sql = "update " + blTableName + " set " + blFieldNames +
                       " from " + mtTableName + " inner join " + dtTableName +
                       " on " + mtTableName + "." + Pk + " = " + dtTableName + "." + Pk +
                       " where " + blTableName + "." + mtIDName + " = '" + PkValue + "' and " +
                        blTableName + "." + mtdtID + " = '" + dtPkValue + "' and " +
                        mtTableName + "." + Pk + " = " + blTableName + "." + mtIDName + " and " +
                        dtTableName + "." + dtPk + " = " + blTableName + "." + mtdtID;
                    checkCond = "select " + mtTableName + ".* from " + mtTableName + " inner join " + dtTableName +
                   " on " + mtTableName + "." + Pk + " = " + dtTableName + "." + Pk +
                   " where " + mtTableName + "." + Pk + " = '" + PkValue + "' and " +
                   dtTableName + "." + dtPk + " = '" + dtPkValue + "'";
                }
                else
                {
                    sql = "update " + blTableName + " set " + blFieldNames +
                        " from " + mtTableName + " inner join " + dtTableName +
                        " on " + mtTableName + "." + Pk + " = " + dtTableName + "." + Pk +
                        " inner join " + reTableName + " on " + dtTableName + "." + reKey + " = " + reTableName + "." + reKey +
                        " where " + blTableName + "." + mtIDName + " = '" + PkValue + "' and " +
                         blTableName + "." + mtdtID + " = '" + dtPkValue + "' and " +
                         mtTableName + "." + Pk + " = " + blTableName + "." + mtIDName + " and " +
                         dtTableName + "." + dtPk + " = " + blTableName + "." + mtdtID;

                    checkCond = "select " + mtTableName + ".* from " + mtTableName + " inner join " + dtTableName +
                    " on " + mtTableName + "." + Pk + " = " + dtTableName + "." + Pk +
                    " inner join " + reTableName + " on " + dtTableName + "." + reKey + " = " + reTableName + "." + reKey +
                    " where " + mtTableName + "." + Pk + " = '" + PkValue + "' and " +
                    dtTableName + "." + dtPk + " = '" + dtPkValue + "'";
                }

            }
            else
            {
                foreach (DataRow dr in blConfigDetail.Rows)
                {
                    if ((dr["mtFieldName"].ToString() == string.Empty || dr["mtFieldName"].ToString().ToUpper() == _pk.ToUpper())
                        && dr["Formula"].ToString() == string.Empty)
                        continue;
                    blFieldNames += dr["blFieldName"].ToString();
                    if (dr["mtFieldName"].ToString() != string.Empty)
                        mtFieldNames = mtTableName + "." + dr["mtFieldName"].ToString();
                    else
                        mtFieldNames = dr["Formula"].ToString();
                    blFieldNames += " = " + mtFieldNames + ",";
                    //mtFieldNames += ",";
                }
                if (nhomDk != string.Empty)
                {
                    blFieldNames += "NhomDk = '" + nhomDk + "'";
                    //mtFieldNames += "'" + nhomDk + "'";
                }
                else
                {
                    blFieldNames = blFieldNames.Substring(0, blFieldNames.Length - 1);
                    //mtFieldNames = mtFieldNames.Substring(0, mtFieldNames.Length - 1);
                }
                if (reTableName == string.Empty)
                {
                    sql = "update " + blTableName + " set " + blFieldNames +
                        " from " + mtTableName +
                        " where " + blTableName + "." + mtIDName + " = '" + PkValue + "' and " +
                         mtTableName + "." + Pk + " = " + blTableName + "." + mtIDName;

                    checkCond = "select * from " + mtTableName +
                        " where " + mtTableName + "." + Pk + " = '" + PkValue + "'";
                }
                else
                {
                    sql = "update " + blTableName + " set " + blFieldNames +
                        " from " + mtTableName + " inner join " + reTableName + " on " + 
                            mtTableName + "." + Pk + " = " +reTableName + "." + reKey +
                        " where " + blTableName + "." + mtIDName + " = '" + PkValue + "' and " +
                         mtTableName + "." + Pk + " = " + blTableName + "." + mtIDName;

                    checkCond = "select * from " + mtTableName + " inner join " + reTableName + " on " +
                            mtTableName + "." + Pk + " = " + reTableName + "." + reKey +
                        " where " + mtTableName + "." + Pk + " = '" + PkValue + "'";
                }
            }
            if (nhomDk != string.Empty)
                sql += " and " + blTableName + ".NhomDk = '" + nhomDk + "'";
            string condition = drConfig["Condition"].ToString();
            if (condition != string.Empty)
            {
                sql += " and (" + condition + ")";
                checkCond += " and (" + condition + ")";
            }
            return sql;
        }

        private string GenDelString(DataRow drConfig, string PkValue, string dtPkValue)
        {
            string blTableName = drConfig["blTableName"].ToString();
            string rootIDName = drConfig["RootIDName"].ToString();
            string dtID = drConfig["DtID"].ToString();
            string nhomDk = drConfig["NhomDk"].ToString();
            string tmp = "delete from " + blTableName + " where " + rootIDName + " = '" + PkValue + "'";
            if (nhomDk != string.Empty)
                tmp += " and NhomDk = '" + nhomDk + "'";
            if (dtPkValue != string.Empty)
                tmp += " and " + dtID + " = '" + dtPkValue + "'";
            return tmp;
        }

        private bool TransferDataNew(string PkValue)
        {
            try { 
            foreach (DataRow dr in _blConfig.Rows)
            {
                string blConfigID = dr["blConfigID"].ToString();
                DataTable blConfigDetail = GetDtConfigDetail(blConfigID);
                string sql;

                sql = GenInsertString(dr, blConfigDetail, _pk, PkValue, string.Empty);
                _dataDb.UpdateByNonQuery(sql);
                if (_dataDb.HasErrors) return false;
            }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool TransferDataEdit(string PkValue, List<DataRow> drDetails, bool masterEdit)
        {
            string blTableName = string.Empty;
            foreach (DataRow dr in _blConfig.Rows)
            {
                string checkCond = string.Empty;
                string dtPkValue = string.Empty;
                string blConfigID = dr["blConfigID"].ToString();
                if (!Boolean.Parse(dr["EditSync"].ToString()))
                    continue;
                DataTable blConfigDetail = GetDtConfigDetail(blConfigID);
                string sql = string.Empty;

                string dtPk = dr["dtPk"].ToString();
                sql = GenDelString(dr, PkValue, string.Empty);
                //ErrorManager.LogFile.AppendToFile("Tranfer.txt", sql);
                _dataDb.UpdateByNonQuery(sql);
                if (_dataDb.HasErrors) return false;
                //if (dr["NhomDK"].ToString() == "PNM1")
                //{

                //}
                sql = GenInsertString(dr, blConfigDetail, _pk, PkValue, string.Empty);
                //ErrorManager.LogFile.AppendToFile("Tranfer.txt", sql);
                _dataDb.UpdateByNonQuery(sql);
                if (_dataDb.HasErrors) return false;
                //if (dtPk == string.Empty)   //truong hop khong co detail
                //{
                //    sql = GenUpdateString(dr, blConfigDetail, _pk, PkValue, string.Empty, ref checkCond);
                //    if (sql != string.Empty)
                //    {
                //        int rec = 0;
                //        _dataDb.UpdateByNonQuery(sql, ref rec);
                //        if (rec == 0)
                //        {
                //            DataTable dt = _dataDb.GetDataTable(checkCond);
                //            if (dt.Rows.Count == 0) //phải xóa khỏi bảng tổng hợp vì không còn đúng với điều kiện transfer
                //                sql = GenDelString(dr, PkValue, string.Empty);
                //            else  //phải thêm vào bảng tổng hợp vì đã đúng với điều kiện transfer
                //                sql = GenInsertString(dr, blConfigDetail, _pk, PkValue, string.Empty);
                //            _dataDb.UpdateByNonQuery(sql);
                //        }
                //    }
                //    continue;
                //}
                //foreach (DataRow drDetail in drDetails)
                //{
                //    switch (drDetail.RowState)
                //    {
                //        case DataRowState.Added:
                //            dtPkValue = drDetail[dtPk].ToString();
                //            sql = GenInsertString(dr, blConfigDetail, _pk, PkValue, dtPkValue);
                //            break;
                //        case DataRowState.Modified:
                //            dtPkValue = drDetail[dtPk].ToString();
                //            sql = GenUpdateString(dr, blConfigDetail, _pk, PkValue, dtPkValue, ref checkCond);
                //            break;
                //        case DataRowState.Deleted:
                //            drDetail.RejectChanges();
                //            dtPkValue = drDetail[dtPk].ToString();
                //            sql = GenDelString(dr, PkValue, dtPkValue);
                //            drDetail.Delete();
                //            break;
                //        case DataRowState.Unchanged:
                //            if (masterEdit)
                //            {
                //                dtPkValue = drDetail[dtPk].ToString();
                //                sql = GenUpdateString(dr, blConfigDetail, _pk, PkValue, dtPkValue, ref checkCond);
                //            }
                //            break;
                //    }
                //    if (sql != string.Empty)
                //    {
                //        int rec = 0;
                //        _dataDb.UpdateByNonQuery(sql, ref rec);
                //        if ((rec == 0 && drDetail.RowState == DataRowState.Modified)
                //            || (rec == 0 && drDetail.RowState == DataRowState.Unchanged && masterEdit))
                //        if ((drDetail.RowState == DataRowState.Modified)
                //            || ( drDetail.RowState == DataRowState.Unchanged && masterEdit))
                //        {
                //            DataTable dt = _dataDb.GetDataTable(checkCond);
                //            if (dt.Rows.Count == 0) //phai xoa khoi bang tong hop vi khong con dung dieu kien transfer
                //                sql = GenDelString(dr, PkValue, dtPkValue);
                //            else  //phai them vao bang tong hop vi truoc day ko dung dk transfer nhung bay gio dung
                //                sql = GenInsertString(dr, blConfigDetail, _pk, PkValue, dtPkValue);
                //            _dataDb.UpdateByNonQuery(sql);
                //        }
                //    }
                //}
                if (_dataDb.HasErrors) return false;
            }
            return true;
        }

        private bool TransferDataDelete(string PkValue)
        {
            try { 
            foreach (DataRow dr in _blConfig.Rows)
            {
                string blConfigID = dr["blConfigID"].ToString();
                DataTable blConfigDetail = GetDtConfigDetail(blConfigID);
                string sqlDel = GenDelString(dr, PkValue, string.Empty);
                _dataDb.UpdateByNonQuery(sqlDel);
                if (_dataDb.HasErrors) return false;
            }
            return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Transfer(DataAction dataAction, string PkValue, List<DataRow> drDetails, bool masterEdit)
        {
            if (_blConfig == null)
                return true;
            switch (dataAction)
            {
                case DataAction.Insert:
                   return TransferDataNew(PkValue);
                    
                case DataAction.Update:
                   return TransferDataEdit(PkValue, drDetails, masterEdit);
                    
                case DataAction.Delete:
                   return TransferDataDelete(PkValue);
                    
            }
            return true;
        }
    }
}
