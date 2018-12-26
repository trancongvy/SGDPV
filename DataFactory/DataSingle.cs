using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTDatabase;
using DataFactory;
using CDTControl;
using CDTLib;

namespace DataFactory
{
    public class DataSingle : CDTData
    {
        public string extraWS = string.Empty;
        public string WS = string.Empty;
        public string WSGr = string.Empty;
        public DataSingle(string sysTableID)
        {
            this._dataType = DataType.Single;
            this.GetInfor(sysTableID);
            this.GetStruct();
            this._formulaCaculator = new FormulaCaculator(_dataType, _dsStruct);
            this._customize = new Customize(_dataType, DbData, _drTable, _drTableMaster);
        }

        public DataSingle(string TableName, string sysPackageID)
        {
            this._dataType = DataType.Single;
            this.GetInfor(TableName, sysPackageID);
            this.GetStruct();
            this._formulaCaculator = new FormulaCaculator(_dataType, _dsStruct);
            this._customize = new Customize(_dataType, DbData, _drTable, _drTableMaster);
            
        }

        public DataSingle(DataRow drTable)
        {
            this._dataType = DataType.Single;
            this.GetInfor(drTable);
            this.GetStruct();
            this._formulaCaculator = new FormulaCaculator(_dataType, _dsStruct);
            this._customize = new Customize(_dataType, DbData, _drTable, _drTableMaster);
        }

        public override void GetInfor(string sysTableID)
        {
            base.GetInfor(sysTableID);
            DbData.MasterPk = _drTable["Pk"].ToString();
            this.DbData.QueryMaster = "select * from " + _drTable["TableName"].ToString();
        }

        public override void GetInfor(string TableName, string sysPackageID)
        {
            base.GetInfor(TableName, sysPackageID);
            DbData.MasterPk = _drTable["Pk"].ToString();
            this.DbData.QueryMaster = "select * from " + _drTable["TableName"].ToString();
        }

        public override void GetInfor(DataRow drTable)
        {
            base.GetInfor(drTable);
            DbData.MasterPk = _drTable["Pk"].ToString();
            this.DbData.QueryMaster = "select * from " + _drTable["TableName"].ToString();
        }

        public override void GetData()
        {
            ConditionForPackage();
            string extrasql = string.Empty;
            //xét trường hợp phân toàn quyền 
            //
            string extraWs = string.Empty;
            if (DrTable["sysUserID"] != null)
            {
                string adminList = DrTable["sysUserID"].ToString().Trim();
                if (adminList != string.Empty)
                {
                    if (adminList != Config.GetValue("sysUserID").ToString().Trim())
                    {
                        string dk = NotAdminListCondition();
                        dk = UpdateSpecialCondition(dk);
                        extraWs = " (charindex('_" + Config.GetValue("sysUserID").ToString().Trim() + "_',ws)>0 or charindex('_" + Config.GetValue("sysUserGroupID").ToString().Trim() + "_',Grws)>0)";
                        if (dk != string.Empty)
                            extraWs += " or " + dk;
                        extraWS = dk;
                    }
                }
            }
            //
            if (_drTable.Table.Columns.Contains("ExtraSql"))
                if (_drTable["ExtraSql"] != null)
                    extrasql = _drTable["Extrasql"].ToString();

            if (extraWs != string.Empty)
            {
                if (extrasql == string.Empty)
                {
                    extrasql = extraWs;
                }
                else
                {
                    extrasql += " and (" + extraWs + ")";
                }
            }

            string queryData = "select * from " + _drTable["TableName"].ToString();
            if (_condition != string.Empty && !(_condition.Contains("@")))
            {
                queryData += " where " + _condition;
                if (extrasql != string.Empty)
                    queryData += " and (" + extrasql + ")";
            }
            else
            {
                fullData = true;
                if (extrasql != string.Empty)
                    queryData += " where " + extrasql;
            }
            if (_drTable["SortOrder"].ToString() != string.Empty)
                queryData += " order by " + DrTable["SortOrder"].ToString();
            DsData = DbData.GetDataSet(queryData);
            if (DsData != null)
            {
                string fkName = _drTable["Pk"].ToString();
                DsData.Tables[0].Columns[fkName].Unique = true;
                DsData.Tables[0].PrimaryKey = new DataColumn[] { DsData.Tables[0].Columns[fkName] };
                DsData.Tables[0].TableName = _drTable["TableName"].ToString();
                _dsDataTmp = DsData.Copy();
            }
        }
        public override void DataTable0_ColChanged(object sender, DataColumnChangeEventArgs e)
        {
            _dataChanged = true;
            if (WSGr == string.Empty) return;
            if (e.Column.ColumnName != "_Chk") return;
            if (this.DsData.Tables[0].Columns.Contains("_Chk") && this.DsData.Tables[0].Columns.Contains("Grws") && this.DrTable["sysUserID"] != null)
            {
                if (e.Row["Grws"] == null) e.Row["Grws"] = string.Empty;
                e.Row["Grws"] = e.Row["Grws"].ToString().Replace("_" +WSGr + "_", "");
                if (e.Row["_chk"].ToString().ToLower() == "true")
                    e.Row["Grws"] = e.Row["Grws"].ToString() + "_" + WSGr + "_";
            }
        }
        public void GetData(CDTData ParentData)
        {
            ConditionForPackage();
            string extrasql = string.Empty;

            if (_drTable.Table.Columns.Contains("ExtraSql"))
                if (_drTable["ExtraSql"] != null)
                    extrasql = _drTable["Extrasql"].ToString();

            string queryData = "select * from " + _drTable["TableName"].ToString();
            if (_condition != string.Empty && !(_condition.Contains("@")))
            {
                queryData += " where " + _condition;
                if (extrasql != string.Empty)
                    queryData += " and (" + extrasql + ")";
            }
            else
                if (extrasql != string.Empty)
                    queryData += " where " + extrasql;

            string lkCondition = GenConditionForLookup(ParentData);
            if (lkCondition != string.Empty )
            {
                if ((_condition == string.Empty && extrasql == string.Empty) || (_condition != string.Empty && _condition.Contains("@")))
                    queryData += " where " + lkCondition;
                else
                    queryData += " and ( " + lkCondition + ")";
            }

            if (_drTable["SortOrder"].ToString() != string.Empty)
                queryData += " order by " + DrTable["SortOrder"].ToString();

            DsData = DbData.GetDataSet(queryData);
            if (DsData != null)
            {
                DsData.Tables[0].TableName = _drTable["TableName"].ToString();
                _dsDataTmp = DsData.Copy();
            }
        }

        private string GenConditionForLookup(CDTData ParentData)
        {
            string s = string.Empty;
            string tableName = _drTable["TableName"].ToString().ToUpper();
            foreach (DataRow drField in ParentData.DsStruct.Tables[0].Rows)
            {
                string refTable = drField["RefTable"].ToString().ToUpper();
                string datatype = drField["type"].ToString();
                if (refTable == string.Empty || tableName != refTable || ! "147".Contains(datatype) )
                    continue;
                if (s == string.Empty)
                    s = drField["refField"].ToString() + " in (";
                string fieldName = drField["FieldName"].ToString();
                //if (fieldName.ToUpper() == "REFTABLE" || fieldName.ToUpper() == "REFFIELD")
                //    continue;
                int n = ParentData.DsData.Tables[0].Rows.Count;
                for (int i = 0; i < n; i++)
                {
                    DataRow drData = ParentData.DsData.Tables[0].Rows[i];
                    if (drData[fieldName].ToString() == string.Empty)
                        continue;
                    string newValue = "'" + drData[fieldName].ToString() + "'";
                    if (!s.Contains(newValue))
                    {
                        s += newValue + ",";
                    }
                }
            }
            if (s.EndsWith(","))
                s = s.Remove(s.Length - 1);
            if (ParentData.DsData.Tables.Count == 1 || ParentData.DsData.Tables[1].Rows.Count == 0)
            {
                if (s.EndsWith("(") || s == string.Empty)
                    s = "1 = 0";
                else
                    s += ")";
                return s;
            }
            bool first = true;
            foreach (DataRow drField in ParentData.DsStruct.Tables[1].Rows)
            {
                string refTable = drField["RefTable"].ToString().ToUpper();
                if (refTable == string.Empty || tableName != refTable)
                    continue;
                if (s == string.Empty)
                    s = drField["refField"].ToString() + " in (";
                string fieldName = drField["FieldName"].ToString();
                //if (fieldName.ToUpper() == "REFTABLE" || fieldName.ToUpper() == "REFFIELD")
                //    continue;
                int n = ParentData.DsData.Tables[1].Rows.Count;
                if (first && n > 0 && !s.EndsWith("("))
                {
                    s += ",";
                }
                first = false;
                for (int i = 0; i < n; i++)
                {
                    DataRow drData = ParentData.DsData.Tables[1].Rows[i];
                    if (drData[fieldName].ToString() == string.Empty)
                        continue;
                    string newValue = "'" + drData[fieldName].ToString() + "'";
                    if (!s.Contains(newValue))
                    {
                        s += newValue + ",";
                    }
                }
            }
            for(int itemp=0;itemp<ParentData._dsStructDt.Tables.Count; itemp++)
            {
                DataTable tbStruct = ParentData._dsStructDt.Tables[itemp];


                
                foreach (DataRow drField in tbStruct.Rows )
                {
                    string refTable = drField["RefTable"].ToString().ToUpper();
                    if (refTable == string.Empty || tableName != refTable)
                        continue;
                    if (s == string.Empty)
                        s = drField["refField"].ToString() + " in (";
                    string fieldName = drField["FieldName"].ToString();
                    //if (fieldName.ToUpper() == "REFTABLE" || fieldName.ToUpper() == "REFFIELD")
                    //    continue;
                    int n = ParentData.DsData.Tables[itemp+2].Rows.Count;
                    if (first && n > 0 && !s.EndsWith("("))
                    {
                        s += ",";
                    }
                    first = false;
                    for (int i = 0; i < n; i++)
                    {
                        DataRow drData = ParentData.DsData.Tables[itemp+2].Rows[i];
                        if (drData[fieldName].ToString() == string.Empty)
                            continue;
                        string newValue = "'" + drData[fieldName].ToString() + "'";
                        if (!s.Contains(newValue))
                        {
                            s += newValue + ",";
                        }
                    }
                }
            }
    
            if (s.EndsWith(","))
                s = s.Remove(s.Length - 1);
            if (s.EndsWith("(") || s == string.Empty)
                s = "1 = 0";
            else
                s += ")";
            return s;
        }

        public override bool UpdateData(DataAction dataAction)
        {
            DateTime btime = DateTime.Now;
            DateTime etime = DateTime.Now;
            if (!_dataChanged)
                return true;
            bool isNew = _drCurrentMaster.RowState == DataRowState.Added;
            //kiểm tra Record trước khi update có thỏa điều kiện phân quyền không
            Boolean chkOk = false;
            if (extraWS != string.Empty)
            {
                object pkValue = null;

                string fieldName = string.Empty;

                bool isDelete = false;
                bool isModify = false;
                DataView vOrg = new DataView(_drCurrentMaster.Table);

                if (_drCurrentMaster.RowState == DataRowState.Deleted)
                {
                    _drCurrentMaster.RejectChanges();
                    isDelete = true;
                }
                if (_drCurrentMaster.RowState == DataRowState.Modified)
                {
                    vOrg.RowStateFilter = DataViewRowState.ModifiedOriginal;
                    vOrg.RowFilter = extraWS;
                    isModify = true;
                }

                pkValue = _drCurrentMaster[PkMaster.FieldName];
                DataRow[] DRowtmp = DsData.Tables[0].Select(extraWS);
                foreach (DataRow drtmp in DRowtmp)
                {
                    if (pkValue == drtmp[PkMaster.FieldName])
                    {
                        if (isModify)
                        {
                            if (vOrg.Count > 0) chkOk = true;
                        }
                        else
                        {
                            chkOk = true;
                        }
                    }
                }
                if (isDelete) _drCurrentMaster.Delete();
            }
            else
            {
                chkOk = true;
            }
            if (!chkOk)
                return false;
            //----
            bool isError = false;
            try
            {
                btime = DateTime.Now;
                DbData.BeginMultiTrans();
                this.CheckRules(dataAction);
                if (_drCurrentMaster.Table.DataSet.HasErrors)
                {
                    DbData.RollbackMultiTrans();
                    return false;
                }
                int index = DsData.Tables[0].Rows.IndexOf(_drCurrentMaster);

                if (!_customize.BeforeUpdate(index, DsData))
                {
                    DbData.EndMultiTrans();
                    return false;
                }

                if (Update(_drCurrentMaster))
                {
                    TransferData(dataAction, index);
                    _customize.AfterUpdate();
                }
                isError = DbData.HasErrors;
                if (!isError)
                {
                    etime = DateTime.Now;
                    DbData.EndMultiTrans();
                }
                else
                    DbData.RollbackMultiTrans();

                if (isNew && !isError)
                    _autoIncreValues.UpdateNewStruct(_drCurrentMaster);
                if (!isError)
                {
                    base.InsertHistory(dataAction, DsData,btime,etime);
                    DsData.AcceptChanges();
                    _dsDataTmp = DsData.Copy();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (this.DbData.Connection.State != ConnectionState.Closed)
                    this.DbData.Connection.Close();
            }
            return (!isError);
        }

        public override DataTable GetDataForPrint(int index)
        {
            return (this.DsData.Tables[0]);
        }
        public override DataTable GetDataForPrint(int index,string _script)
        {
            return (this.DbData.GetDataTable(_script));
        }
        private string NotAdminListCondition()
        {
            string dk = "(";
            string ws = Config.GetValue("sysUserID").ToString().Trim();
            string tableid = DrTable["sysTableID"].ToString().Trim();
            string sql = "select condition from sysAdminDM where systableid=" + tableid + " and (sysUserID=" + ws + " or sysUserGroupID  in (select sysUserGroupID from sysUser where sysUserID=" + ws + " ))"  ;
            sql = UpdateSpecialCondition(sql);
            DataTable tbCon = this._dbStruct.GetDataTable(sql);
            
            foreach (DataRow dr in tbCon.Rows)
            {
                dk += "(" + dr["condition"].ToString() + ") or "; 
            }
            if (dk.Contains("or")) dk = dk.Substring(0, dk.Length - 3) + ")";
            else
            {
                dk = "1=0";
            }
            return dk;
        }
        public void updateWS()
        {
            try
            {
                DbData.BeginMultiTrans();
                string sql;// = "update " + _drTable["TableName"].ToString() + " set ws=null ";
                // if (extraWS != string.Empty)
                //     sql += " where " + extraWS;
                // DbData.UpdateByNonQuery(sql);
                DataRow[] drQ;
                if (extraWS != string.Empty)
                    drQ = DsData.Tables[0].Select(extraWS);
                else
                    drQ = DsData.Tables[0].Select();
                string extraForType = string.Empty;
                if (PkMaster.DbType == SqlDbType.VarChar || PkMaster.DbType == SqlDbType.UniqueIdentifier)
                    extraForType = "'";

                foreach (DataRow dr in drQ)
                {
                    if (dr.RowState == DataRowState.Modified)
                    {
                        sql = "update " + _drTable["TableName"].ToString() + " set Grws='" + dr["Grws"].ToString() + "' where " + PkMaster.FieldName + "=" + quote + dr[PkMaster.FieldName] + quote;
                        DbData.UpdateByNonQuery(sql);
                        dr.AcceptChanges();
                    }
                }
                if (DbData.HasErrors)
                    DbData.RollbackMultiTrans();
                else

                    DbData.EndMultiTrans();

            }
            finally
            {
                if (DbData.Connection.State != ConnectionState.Closed)
                    DbData.Connection.Close();
            }
        }
        public void ChangeCode(DataRow OldRow, DataRow NewRow)
        {
            string pk = _drTable["pk"].ToString();
            string OldCode = OldRow[pk].ToString();
            string NewCode = NewRow[pk].ToString();

            DbData.BeginMultiTrans();
            try
            {
                string sql = "select * from systable where systableid in(select systableid from sysfield where RootTable='" + _drTable["TableName"].ToString() + "' group by systableid) and Collecttype<>-1";
                DataTable dsTable = _dbStruct.GetDataTable(sql);
                sql = "select * from sysfield where RootTable='" + _drTable["TableName"].ToString() + "'";
                DataTable dsField = _dbStruct.GetDataTable(sql);

                foreach (DataRow dr in dsTable.Rows)
                {
                    DataRow[] lstField = dsField.Select("sysTableID=" + dr["sysTableID"].ToString());
                    foreach (DataRow drField in lstField)
                    {
                        sql = "Update " + dr["TableName"] + " set " + drField["FieldName"].ToString() + " ='" + NewCode + "' where " + drField["FieldName"].ToString() + " ='" + OldCode + "'";
                        DbData.UpdateByNonQuery(sql);
                    }
                }
                DbData.EndMultiTrans();
            }
            catch (Exception ex)
            {
                DbData.RollbackMultiTrans();
            }
            finally
            {
                if (DbData.Connection.State != ConnectionState.Closed)
                    DbData.Connection.Close();
            }

        }
    }
}
