using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
//using DevExpress.XtraEditors;
using CDTDatabase;
using CDTControl;
using CDTLib;
using Plugins;
using System.Windows.Forms;

namespace CDTControl
{
    public class Customize
    {
        private DataType _dataType;
        private Database _dbData;
        private DataRow _drTable;
        private DataRow _drTableMaster;
        private DataSet _dsData;
        private string _tableName;
        private int _curMasterIndex;
        private DataSet _dsDataCopy;
        private List<ICustomData> _lstDll;
        private StructPara _structPara;
        PluginManager pm = new PluginManager();
        private string _pkMaster;
        public Customize(DataType dataType, Database dbData, DataRow drTable, DataRow drTableMaster,string pkMaster)
        {
            _dataType = dataType;
            _dbData = dbData;
            _drTable = drTable;
            _drTableMaster = drTableMaster;
            _pkMaster = pkMaster;
        }

        private void PrepareData(DataSet dsData)
        {
            if (_lstDll == null)
            {
                pm.AddICustomData();
                _lstDll = pm.LstICustomData;
            }

            _dsData = dsData;
            _dsDataCopy = dsData.Copy();

            if (_dataType == DataType.MasterDetail)
                _tableName = _drTableMaster["TableName"].ToString();
            else
                _tableName = _drTable["TableName"].ToString();

            _structPara = new StructPara(_dbData, _drTable, _drTableMaster, _dsData, _tableName, _curMasterIndex, _dsDataCopy,_pkMaster);
        }

        public bool BeforeUpdate(int curMasterIndex, DataSet dsData)
        {
            _curMasterIndex = curMasterIndex;
            PrepareData(dsData);
            switch (_tableName)
            {
                case "sysUser":
                    SysUser(_dbData, _drTable, _dsData.Tables[0]);
                    break;
                case "sysPackage":
                    return (SysPackage(_dsData.Tables[0]));
                
            }
            return ExecuteDll(true);
        }

        //private bool KiemtraMaDVQS()
        //{
        //    if (_dsData.Tables[0].Rows[_curMasterIndex]["Approved"].ToString() == "1")
        //    {
        //        _dsData.Tables[0].DefaultView.RowStateFilter = DataViewRowState.ModifiedOriginal;
        //        if (_dsData.Tables[0].DefaultView.Count == 1 && _dsData.Tables[0].DefaultView[0]["Approved"].ToString() == "0")
        //        {
        //            //Đây là trường hợp approved dữ liệu báo giá
        //            DataRow [] lst=_dsData.Tables[1].Select("MTQSID='" + _dsData.Tables[0].Rows[_curMasterIndex]["MTQSID"].ToString()+"'");
        //            foreach (DataRow dr in lst)
        //            {
        //                if (dr["MaDV"] == DBNull.Value  )
        //                {                            
        //                    MessageBox.Show("Thông tin báo giá chưa đầy đủ, Cần nhập mã vật tư phụ tùng đầy đủ!", "Cảnh báo", MessageBoxButtons.OK);
        //                    return false;
        //                }
        //            }
        //        }
        //        _dsData.Tables[0].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
        //    }
        //    return true;
        //}

        private void UpdateDataCopy()
        {
            DataView dvCopy = new DataView(_dsDataCopy.Tables[0]);
            dvCopy.RowStateFilter = DataViewRowState.CurrentRows;
            for (int i = 0; i < dvCopy.Count; i++)
            {
                DataRow drCopy = dvCopy[i].Row;
                if (drCopy.RowState != DataRowState.Added)
                    continue;
                DataRow drData = _dsData.Tables[0].Rows[i];
                for (int j = 0; j < drCopy.Table.Columns.Count; j++)
                    if (drCopy[j] != drData[j])
                        drCopy[j] = drData[j];
            }
            if (_dataType == DataType.MasterDetail)
            {
                dvCopy = new DataView(_dsDataCopy.Tables[1]);
                dvCopy.RowStateFilter = DataViewRowState.CurrentRows;
                for (int i = 0; i < dvCopy.Count; i++)
                {
                    DataRow drCopy = dvCopy[i].Row;
                    if (drCopy.RowState != DataRowState.Added)
                        continue;
                    DataRow drData = _dsData.Tables[1].Rows[i];
                    if (drData.RowState != DataRowState.Deleted)
                        for (int j = 0; j < drCopy.Table.Columns.Count; j++)                     
                         if ( drCopy[j] != drData[j])
                            drCopy[j] = drData[j];
                }
            }
        }

        public bool AfterUpdate()
        {
            UpdateDataCopy();
            if (_dataType == DataType.MasterDetail && _dsDataCopy.Tables[0].Rows[_curMasterIndex].RowState == DataRowState.Added)
            {
                try
                {
                    if (_dsDataCopy.Tables[0].Columns.Contains("Soct"))
                    {
                       // string sql = "insert into sysnotify (hdatetime,soct,sysmenuid, systableid) values (getdate(),'" + _dsDataCopy.Tables[0].Rows[_curMasterIndex]["Soct"].ToString();
                       // sql += "'," + _drTable["sysMenuid"].ToString() + "," + _drTable["systableid"].ToString() + ")";
                       // Database dbSt = Database.NewStructDatabase();
                       // dbSt.UpdateByNonQuery(sql);
                    }
                }
                catch { }
            }
            switch (_tableName)
            {
                case "sysTable":
                    if (_curMasterIndex < 0)
                        return true;
                    if (_dsDataCopy.Tables[0].Rows[_curMasterIndex].RowState != DataRowState.Deleted)
                    {
                        if (_dsData.Tables[0].Rows[_curMasterIndex]["CollectType"].ToString() != string.Empty &&
                            int.Parse(_dsData.Tables[0].Rows[_curMasterIndex]["CollectType"].ToString()) != -1)
                        {
                            return (SysTable(_curMasterIndex, _dsDataCopy));
                        }
                    }
                    else
                    {
                        return (SysTable(_curMasterIndex, _dsDataCopy));
                    }
                    break;
                case "sysReport":
                    SysReport(_dsDataCopy.Tables[0]);
                    break;
                case "sysDataConfig":
                    SysDataConfig(_dsDataCopy.Tables[0]);
                    break;
                case "sysUserPackage":
                    SysUserPackage(_dsDataCopy.Tables[0]);
                    break;
            }
            return ExecuteDll(false);
        }

        private bool ExecuteDll(bool isBefore)
        {
            for (int i = 0; i < _lstDll.Count; i++)
            {
                if (_lstDll[i] == null)
                    continue;
                _lstDll[i].Info = _structPara;
                if (isBefore)
                    _lstDll[i].ExecuteBefore();
                else
                    _lstDll[i].ExecuteAfter();
                if (_lstDll[i].Result == false)
                    return false;
            }
            return true;
        }

        private bool SysTable(int curMasterIndex, DataSet dsData)
        {
            if (curMasterIndex < 0)
                return true;
            DataView dv = new DataView(dsData.Tables[0]);
            dv.RowStateFilter = DataViewRowState.Deleted | DataViewRowState.CurrentRows;
            DataRowView drvMaster = dv[curMasterIndex];
            if (int.Parse(drvMaster["CollectType"].ToString()) < 0) return true;
            DataView dvDetail = new DataView(dsData.Tables[1]);
            dvDetail.RowStateFilter = DataViewRowState.Deleted | DataViewRowState.CurrentRows;
            DataView dvDetailOld = new DataView(dsData.Tables[1]);
            dvDetailOld.RowStateFilter = DataViewRowState.Deleted | DataViewRowState.OriginalRows | DataViewRowState.ModifiedOriginal;
            if (drvMaster["sysTableID"].ToString() == string.Empty)
            {
                dvDetail.RowFilter = "sysTableID is null";
                dvDetailOld.RowFilter = "sysTableID is null";
            }
            else
            {
                dvDetail.RowFilter = "sysTableID = " + drvMaster["sysTableID"].ToString();
                dvDetailOld.RowFilter = "sysTableID = " + drvMaster["sysTableID"].ToString();
            }
            if (drvMaster.Row.RowState == DataRowState.Deleted && Boolean.Parse(drvMaster["System"].ToString()))
                return false;
            string sysPackageID = drvMaster["sysPackageID"].ToString();
            string pk = drvMaster["Pk"].ToString();
            string TableName = drvMaster["TableName"].ToString();
            bool isRemote = bool.Parse(Config.GetValue("isRemote").ToString());
            string currentConn;
            if (isRemote)
                currentConn = _dbData.GetValue("select DbPathRemote  from sysdb a  where sysPackageID = " + sysPackageID).ToString();
            else
            {
                currentConn = _dbData.GetValue("select DbPath  from sysdb a  where sysPackageID = " + sysPackageID).ToString();
            }
            string crrrentDbName = _dbData.GetValue("select top 1 DatabaseName  from sysdb where sysPackageID = " + sysPackageID).ToString();
            currentConn = Security.DeCode64(currentConn) + ";Database=" + crrrentDbName;

            Database currentDb = Database.NewCustomDatabase(currentConn);
            TableDesigner td = new TableDesigner(currentDb, TableName, pk);
            //kiểm tra dữ liệu trước khi tạo bảng
            if (drvMaster.Row.RowState != DataRowState.Deleted && !td.IsValid(drvMaster, dvDetail))
                return false;

            //currentDb.BeginMultiTrans();
            bool result = true;
            switch (drvMaster.Row.RowState)
            {
                case DataRowState.Added:
                    result = td.CreateTable(dvDetail);

                    break;
                case DataRowState.Deleted:

                    result = td.DropTable(dvDetail);

                    break;
                case DataRowState.Modified:
                case DataRowState.Unchanged:

                    for (int i = 0; i < dvDetail.Count; i++)
                    {
                        DataRowView dr = dvDetail[i];
                        switch (dr.Row.RowState)
                        {
                            case DataRowState.Added:
                                result = td.AddColumn(dr, dr.Row["AllowNull"].ToString() == "0");
                                break;
                            case DataRowState.Deleted:
                                result = td.DropColumn(TableName, dr);
                                result = true;
                                break;
                            case DataRowState.Modified:
                                result = td.AlterColumn(dr, dvDetailOld[i]);
                                break;
                        }
                    }
                    break;
            }
            //if (result)
            //    currentDb.EndMultiTrans();
            return result;
        }

        /// <summary>
        /// Các chức năng customize cho bảng sysUser
        /// </summary>
        private void SysUser(Database db, DataRow drTable, DataTable dtData)
        {
            DataView dv = new DataView(dtData);
            dv.RowStateFilter = DataViewRowState.Added;
            for (int i = 0; i < dv.Count; i++)
                if (dv[i]["Password"].ToString() != string.Empty)
                    dv[i].Row["Password"] = Security.EnCode(dv[i]["Password"].ToString());
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
            DataView dvOriginal = new DataView(dtData);
            dvOriginal.RowStateFilter = DataViewRowState.ModifiedOriginal;
            for (int i = 0; i < dv.Count; i++)
                if (dv[i]["Password"].ToString() != dvOriginal[i]["Password"].ToString())
                    dv[i].Row["Password"] = Security.EnCode(dv[i]["Password"].ToString());
        }

        /// <summary>
        /// Các chức năng customize cho bảng sysDataConfig
        /// </summary>
        private void SysUserPackage(DataTable dtData)
        {
            Database dbStruct = this._dbData; //Database.NewStructDatabase();
            DataView dv = new DataView(dtData);
            dv.RowStateFilter = DataViewRowState.Added;
            foreach (DataRowView drv in dv)
            {
                if (Boolean.Parse(drv["IsAdmin"].ToString()))
                    continue;
                string sysUserPackageID = drv["sysUserPackageID"].ToString();
                string sysPackageID = drv["sysPackageID"].ToString();

                string s = "select sysMenuID from sysMenu where sysPackageID is null or sysPackageID = " + sysPackageID;
                DataTable dtMenu = dbStruct.GetDataTable(s);
                foreach (DataRow drMenu in dtMenu.Rows)
                {
                    string sysMenuID = drMenu["sysMenuID"].ToString();
                    s = "insert into sysUserMenu(sysUserPackageID, sysMenuID) values (" + sysUserPackageID + "," + sysMenuID + ")";
                    dbStruct.UpdateByNonQuery(s);
                }

                s = "select sysTableID from sysTable where type not in (3,6) and sysPackageID = " + sysPackageID;
                DataTable dtTable = dbStruct.GetDataTable(s);
                foreach ( DataRow drTable in dtTable.Rows)
                {
                    string sysTableID = drTable["sysTableID"].ToString();
                    s = "insert into sysUserTable(sysUserPackageID, sysTableID) values (" + sysUserPackageID + "," + sysTableID + ")";
                    dbStruct.UpdateByNonQuery(s);
                }
            }
        }

        /// <summary>
        /// Các chức năng customize cho bảng sysDataConfig
        /// </summary>
        private void SysDataConfig(DataTable dtData)
        {
            DataView dv = new DataView(dtData);
            DataRow[] drx = dtData.Select("NhomDK='PTT3'");

            dv.RowStateFilter = DataViewRowState.Added;
            foreach (DataRowView drDetail in dv)
            {
                string blConfigID = drDetail["blConfigID"].ToString();
                if (blConfigID == string.Empty)
                    return;
                string sysTableID = drDetail["sysTableID"].ToString();
                if (sysTableID == string.Empty)
                    return;
                string sql1 = "select * from sysField where Type <> 3 and sysTableID = " + sysTableID + " order by TabIndex";
                DataTable dt = _dbData.GetDataTable(sql1);
                if (dt == null)
                    return;
                foreach (DataRow dr in dt.Rows)
                {
                    string sysFieldID = dr["sysFieldID"].ToString();
                    string sql2 = "insert into sysDataConfigDt(blConfigID, blFieldID) values(" + blConfigID + "," + sysFieldID + ")";
                    _dbData.UpdateByNonQuery(sql2);
                }
            }
        }

        /// <summary>
        /// Các chức năng customize cho bảng sysReport
        /// </summary>
        private void SysReport(DataTable dtData)
        {
            DataView dv = new DataView(dtData);
            dv.RowStateFilter = DataViewRowState.Added;
            foreach (DataRowView drMaster in dv)
            {
                if (Int32.Parse(drMaster["RpType"].ToString()) == 2)
                    continue;
                if (drMaster["sysReportParentID"].ToString() != string.Empty)
                    continue;
                string sysReportID = drMaster["sysReportID"].ToString();
                if (sysReportID == string.Empty)
                    continue;
                string mtTableID = drMaster["mtTableID"].ToString();
                string dtTableID = drMaster["dtTableID"].ToString();
                if (mtTableID == string.Empty)
                    continue;

                string sql1 = "select * from sysField where Visible <>'0' and sysTableID = " + mtTableID + " order by TabIndex";
                DataTable dt = _dbData.GetDataTable(sql1);
                if (dt == null)
                    return;
                foreach (DataRow dr in dt.Rows)
                {
                    string sysFieldID = dr["sysFieldID"].ToString();
                    string sql2 = "insert into sysReportFilter(sysReportID, sysFieldID, IsMaster) values(" + sysReportID + "," + sysFieldID + ",1)";
                    _dbData.UpdateByNonQuery(sql2);
                }

                if (dtTableID != string.Empty)
                {

                    sql1 = "select * from sysField where Visible <> '0' and Visible<>'1=0' and sysTableID = " + dtTableID + " order by TabIndex";
                    dt = _dbData.GetDataTable(sql1);
                    if (dt == null)
                        return;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sysFieldID = dr["sysFieldID"].ToString();
                        string sql2 = "insert into sysReportFilter(sysReportID, sysFieldID, IsMaster) values(" + sysReportID + "," + sysFieldID + ",0)";
                        _dbData.UpdateByNonQuery(sql2);
                    }
                }
            }
        }

        /// <summary>
        /// Các chức năng customize cho bảng sysPackage
        /// </summary>
        private bool SysPackage(DataTable dtData)
        {
            DataView dv = new DataView(dtData);
            dv.RowStateFilter = DataViewRowState.Added | DataViewRowState.Deleted;
            for (int i = 0; i < dv.Count; i++)
            {
                string dbName = dv[i]["DbName"].ToString();
                Database db = Database.NewCustomDatabase(dv[i]["DbPath"].ToString());
                if (dbName == string.Empty)
                    return false;
                bool result = true;
                if (dv[i].Row.RowState == DataRowState.Added)
                    result = db.UpdateByNonQueryNoTrans("create database " + dbName);
                else
                    if (dv[i].Row.RowState == DataRowState.Deleted && dbName != "CDT")
                        result = db.UpdateByNonQueryNoTrans("if exists (SELECT name FROM master.dbo.sysdatabases WHERE name = '" + dbName + "') drop database " + dbName);
                if (!result)
                    return false;
                if (dv[i].Row.RowState == DataRowState.Deleted)
                    DeletePackage(dv[i]["sysPackageID"].ToString());
            }
            return true;
        }

        private void DeletePackage(string sysPackageID)
        {
            string sql = "delete from sysConfig where sysPackageID = " + sysPackageID;
            sql += "\n delete from sysFormReport where sysReportID in (select sysReportID from sysReport where sysPackageid = " + sysPackageID + ")";
            sql += "\n delete from sysReportFilter where sysReportID in (select sysReportID from sysReport where sysPackageid = " + sysPackageID + ")";
            sql += "\n delete from sysMenu where sysPackageID = " + sysPackageID;
            sql += "\n delete from sysReport where sysPackageID = " + sysPackageID;
            sql += "\n delete from sysDataConfigDt where BlConfigID in (select BlConfigID from sysDataConfig where " +
                " sysTableID in (select sysTableID from sysTable where sysPackageid = " + sysPackageID + "))";
            sql += "\n delete from sysDataConfig where sysTableID in (select sysTableID from sysTable where sysPackageid = " + sysPackageID + ")";
            sql += "\n delete from sysField where systableid in (select systableid from systable where syspackageid = " + sysPackageID + ")";
            sql += "\n delete from sysTable where sysPackageID = " + sysPackageID;
            Database db = Database.NewStructDatabase();
            db.UpdateByNonQuery(sql);
        }
    }
}
