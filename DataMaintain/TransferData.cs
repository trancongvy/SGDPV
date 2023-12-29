using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTDatabase;

namespace DataMaintain
{
    public class DataTransfer
    {
        private Database _dataDb;
        Database _structDb;
        string _sysPackageID;
        DataTable _blConfig;
        private DateTime _tuNgay;
        private DateTime _denNgay;
        private string _mtName = string.Empty;
        private string _Condition = string.Empty;
        public DataTransfer(string structDb, string dataDb, string sysPackageID)
        {
            _structDb = Database.NewCustomDatabase(structDb);
            _dataDb = Database.NewCustomDatabase(dataDb);

            _sysPackageID = sysPackageID;
            
        }

        private DataTable GetDtConfig()
        {
            string s = "select bl.*, tb1.TableName as blTableName, tb2.TableName as mtTableName,  tb2.Pk as Pk,  tb3.TableName as dtTableName,tb4.TableName as reTableName,tb3.Pk as dtPk ,tb4.pk as rePk " +
                " from sysDataConfig bl inner join sysTable tb1 on  bl.sysTableID = tb1.sysTableID " +
                " left join sysTable tb2 on bl.mtTableID = tb2.sysTableID " +
                " left join sysTable tb3 on bl.dtTableID = tb3.sysTableID " +
                " left join sysTable tb4 on bl.reTable = tb4.sysTableID " +
               " where tb1.sysPackageID = " + _sysPackageID + // " and bl.mtTableID = " + _mtTableID +
               " and tb1.TableName in ('BLVT','BLTK','VATIN','VATOUT','BLVTDA') ";
            if (_mtName != string.Empty)
            {
                s += " and tb2.TableName='" + _mtName + "'";
            }
            return (_structDb.GetDataTable(s));
        }

        private DataTable GetDtConfigDetail(string blConfigID)
        {
            string s = "select bld.*, sf1.FieldName as blFieldName, sf2.FieldName as mtFieldName, sf3.FieldName as dtFieldName  " +
            " from sysDataConfigDt bld left join sysField sf1 on  bld.blFieldID = sf1.sysFieldID " +
            " left join  sysField sf2 on bld.mtFieldID = sf2.sysFieldID " +
            " left join sysField sf3   on bld.dtFieldID = sf3.sysFieldID " +
                " where bld.blConfigID = " + blConfigID;
            

            return (_structDb.GetDataTable(s));
        }

        private string GenInsertString(DataRow drConfig, DataTable blConfigDetail, string Pk)//, string PkValue, string dtPkValue)
        {
            string blTableName = drConfig["blTableName"].ToString();
            string mtTableName = drConfig["mtTableName"].ToString();
            string dtTableName = drConfig["dtTableName"].ToString();
            string dtPk = drConfig["dtPk"].ToString();
            string reTableName = drConfig["reTableName"].ToString();

            string reKey = drConfig["Pk"].ToString();
            string dtPkRef = Pk;
            if (dtTableName != null && dtTableName != string.Empty)
            {
                string sqltmp = "select fieldname from sysfield where systableid= " + drConfig["dtTableID"].ToString() + " and refTable='" + mtTableName + "'";
                DataTable tabletmp = _structDb.GetDataTable(sqltmp);
                if (tabletmp != null && tabletmp.Rows.Count > 0)
                {
                    dtPkRef = tabletmp.Rows[0]["FieldName"].ToString();
                }
            }
            string nhomDk = drConfig["NhomDk"].ToString();
            string blFieldNames = string.Empty;
            string mtFieldNames = string.Empty;
            string mtdtFieldNames = string.Empty;
            string sql="";
            string dateFieldName = string.Empty;
            if (dtTableName != string.Empty)
            {
                foreach (DataRow dr in blConfigDetail.Rows)
                {
                    if (dr["mtFieldName"].ToString() == string.Empty
                        && dr["dtFieldName"].ToString() == string.Empty
                         && dr["Formula"].ToString() == string.Empty)
                        continue;
                    blFieldNames += dr["blFieldName"].ToString();
                    dateFieldName = "NGAYCT";
                    if (dr["blFieldName"].ToString().ToUpper() == "NGAYCT")
                    {
                        if (dr["mtFieldName"] != DBNull.Value)
                            dateFieldName = dr["mtFieldName"].ToString();
                    }
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
                string where = " where " + mtTableName + "." + dateFieldName + " between '" + _tuNgay.ToString() + "' and '" + _denNgay.ToString() + "'";
                if (reTableName == string.Empty)
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
                    dateFieldName = "NGAYCT";
                    if (dr["blFieldName"].ToString().ToUpper() == "NGAYCT")
                    {
                        if (dr["mtFieldName"] != DBNull.Value)
                            dateFieldName = dr["mtFieldName"].ToString();                        
                    }
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
                string where = " where " + mtTableName + "." + dateFieldName + " between '" + _tuNgay.ToString() + "' and '" + _denNgay.ToString() + "'";
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
            string condition = drConfig["Condition"].ToString();
            if (condition != string.Empty)
                sql += " and (" + condition + ")";
            if (_mtName != string.Empty && _Condition != string.Empty)
                sql += " and (" + _Condition + ")";
            return sql;

        }

        private string GenDelString(DataRow drConfig,DataTable blConfigDetail, string Pk)
        {
            string blTableName = drConfig["blTableName"].ToString();
            string mtTableName = drConfig["mtTableName"].ToString();
            string nhomDk = drConfig["NhomDK"].ToString();
            string reKey = drConfig["Pk"].ToString();
            string Rootkey= drConfig["RootIDName"].ToString();
            string dtPkRef = Pk;
            string dateFieldName = "NGAYCT";
            foreach (DataRow dr in blConfigDetail.Rows)
            {
                
                if (dr["blFieldName"].ToString().ToUpper() == "NGAYCT")
                {
                    if (dr["mtFieldName"] != DBNull.Value)
                        dateFieldName = dr["mtFieldName"].ToString();
                }
            }
            string selectString = "select " + Pk + " from " + mtTableName + " where (" + dateFieldName + " between '" + _tuNgay.ToString() + "' and '" + _denNgay.ToString() + "')";
            if (_mtName != string.Empty && _Condition != string.Empty)
                selectString += " and (" + _Condition + ")";
            string tmp = "delete from " + blTableName +
                " where (NhomDk = '" + nhomDk + "' and " + Rootkey + " in (" + selectString + "))";
            return tmp;
        }

        public void Maintain(DateTime tuNgay, DateTime denNgay, bool deleteOnly, string mtName, string Condition)
        {
            _tuNgay = tuNgay;
            _denNgay = denNgay;
            _mtName = mtName;
            _Condition = Condition;
            _blConfig = GetDtConfig();
            foreach (DataRowView drv in _blConfig.DefaultView)
            {
                
                DataRow dr = drv.Row;
                string blConfigID = dr["blConfigID"].ToString();
                string pk = dr["Pk"].ToString();
                DataTable blConfigDetail = GetDtConfigDetail(blConfigID);
                string sqlDel = GenDelString(dr, blConfigDetail, pk);
                _dataDb.UpdateByNonQuery(sqlDel);
                if (deleteOnly)
                    continue;
                string sqlIns = GenInsertString(dr, blConfigDetail, pk);
                _dataDb.UpdateByNonQuery(sqlIns);
                if (sqlIns.Contains("PXKC"))
                {

                }
                
            }
            //bao tri rieng cho DT25
            //string sql = "update blvt set ";
            //sql += " blvt.CPNT = blvt.CPNT + dt25.PsNt, ";
            //sql += " blvt.CP = blvt.CP + dt25.Ps, ";
            //sql += " blvt.PsNoNT = blvt.PsNoNT + dt25.PsNt, ";
            //sql += " blvt.PsNo = blvt.PsNo + dt25.Ps ";
            //sql += " from DT25 where blvt.mtid = dt25.mt22id and blvt.mtiddt = dt25.dt22id";
            //sql += " and blvt.ngayct between '" + _tuNgay.ToString() + "' and '" + _denNgay.ToString() + "'";
            //_dataDb.UpdateByNonQuery(sql);
            //sql = "update blvt set ";
            //sql += " blvt.dongia = blvt.psno/blvt.soluong, blvt.dongiaNT = blvt.psnoNT/blvt.soluong ";
            //sql += " from DT25 where blvt.mtid = dt25.mt22id and blvt.mtiddt = dt25.dt22id and blvt.soluong > 0";
            //sql += " and blvt.ngayct between '" + _tuNgay.ToString() + "' and '" + _denNgay.ToString() + "'";
            //_dataDb.UpdateByNonQuery(sql);
        }
    }
}
