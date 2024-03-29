using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTLib;
using DataFactory;
using CDTControl;

namespace DataFactory
{
    internal class DataDetail : CDTData
    {
        public DataDetail(string sysTableID)
        {
            this._dataType = DataType.Detail;
            this.GetInfor(sysTableID);
            this.GetStruct();
            this._formulaCaculator = new FormulaCaculator(_dataType, _dsStruct);
            this._customize = new Customize(_dataType, DbData, _drTable, _drTableMaster, PkMaster.FieldName);
        }

        public DataDetail(string TableName, string sysPackageID)
        {
            this._dataType = DataType.Detail;
            this.GetInfor(TableName, sysPackageID);
            this.GetStruct();
            this._formulaCaculator = new FormulaCaculator(_dataType, _dsStruct);
            this._customize = new Customize(_dataType, DbData, _drTable, _drTableMaster, PkMaster.FieldName);
        }

        public DataDetail(DataRow drTable)
        {
            this._dataType = DataType.Detail;
            this.GetInfor(drTable);
            this.GetStruct();
            this._formulaCaculator = new FormulaCaculator(_dataType, _dsStruct);
            this._customize = new Customize(_dataType, DbData, _drTable, _drTableMaster, PkMaster.FieldName);
        }

        public override event EventHandler DataSoureChanged;

        private void GetInforForMaster()
        {
            string mtTableName = this._drTable["MasterTable"].ToString();
            string sysPackageID = Config.GetValue("sysPackageID").ToString();
            DataTable dt = _dbStruct.GetDataTable("select * from sysTable t left join sysUserTable ut on t.sysTableID = ut.sysTableID where t.TableName = '" + mtTableName + "' and t.sysPackageID = " + sysPackageID);
            if (dt != null && dt.Rows.Count > 0)
                this._drTableMaster = dt.Rows[0];
            else
            {   //trường hợp dữ liệu nằm ở CDT
                dt = _dbStruct.GetDataTable("select * from sysTable t left join sysUserTable ut on t.sysTableID = ut.sysTableID where t.TableName = '" + mtTableName + "' and t.sysPackageID = 5");
                if (dt != null && dt.Rows.Count > 0)
                    _drTableMaster = dt.Rows[0];
            }
        }

        public override void GetInfor(string sysTableID)
        {
            base.GetInfor(sysTableID);
            DbData.DetailPk = _drTable["Pk"].ToString();
            GetInforForMaster();
        }

        public override void GetInfor(DataRow drTable)
        {
            base.GetInfor(drTable);
            DbData.DetailPk = _drTable["Pk"].ToString();
            GetInforForMaster();

        }

        public override void GetInfor(string TableName, string sysPackageID)
        {
            base.GetInfor(TableName, sysPackageID);
            DbData.DetailPk = _drTable["Pk"].ToString();
            GetInforForMaster();
        }

        public override void GetStruct()
        {
            base.GetStruct();
            string sysTableID = _drTableMaster["SysTableID"].ToString();
            string queryString = "select * from sysField f left join sysUserField uf on f.sysFieldID = uf.sysFieldID " +
                " where f.sysTableID = " + sysTableID + " order by TabIndex";
            DataTable dtStruct = _dbStruct.GetDataTable(queryString);
            if (dtStruct != null)
                _dsStruct.Tables.Add(dtStruct);
        }

        private void GetQuery(ref string queryMain, ref string queryDetail)
        {
            string mtTableName = this._drTableMaster["TableName"].ToString();
            string dtTableName = this._drTable["TableName"].ToString();
            string mtSortOrder = this._drTableMaster["SortOrder"].ToString();
            string dtSortOrder = this._drTable["SortOrder"].ToString();
            string maCT = this._drTable["MaCT"].ToString();
            string mtPk = this._drTableMaster["Pk"].ToString();
            string dtPk = this._drTable["Pk"].ToString();
            string extrasql = string.Empty;
            if (_drTable.Table.Columns.Contains("Extrasql"))
            {
                if (_drTable["Extrasql"] != null  )
                {
                    extrasql = _drTable["Extrasql"].ToString();
                }

            }
            queryMain = "select * from " + mtTableName;
            queryDetail = "select * from " + dtTableName;
            if (this._conditionMaster == string.Empty && this._condition == string.Empty)   //truong hop mac dinh
            {
                if (maCT == string.Empty)
                {
                    if (extrasql != string.Empty)
                        queryDetail += " where " + extrasql;
                    if (mtSortOrder != string.Empty)
                        queryMain += " order by " + mtSortOrder;
                }
                else
                {
                    int rowCount = 5;
                    object oRowCount = Config.GetValue("RowCount");
                    if (oRowCount != null)
                        rowCount = Int32.Parse(oRowCount.ToString());
                    queryMain = "select top " + rowCount.ToString() + " * from " + mtTableName;
                    if (mtSortOrder != string.Empty)
                        queryMain += " order by " + mtSortOrder;
                    else
                        queryMain += " order by " + mtPk + " desc";
                    string subQuery = queryMain.Replace("*", mtPk);
                    queryDetail += " where " + mtPk + " in (" + subQuery + ")";
                    if (extrasql != string.Empty)
                        queryDetail += " and " + extrasql;
                }
                
                if (dtSortOrder != string.Empty)
                    queryDetail += " order by " + dtSortOrder;
            }
            if (this._conditionMaster != string.Empty)  //truong hop tim kiem theo bang master
            {
                queryMain += " where " + this._conditionMaster;
                string subQuery = queryMain.Replace("*", mtPk);
                queryDetail += " where " + mtPk + " in (" + subQuery + ")";
                if (extrasql != string.Empty)
                    queryDetail += " and " + extrasql;
                if (mtSortOrder != string.Empty)
                    queryMain += " order by " + mtSortOrder;
                if (dtSortOrder != string.Empty)
                    queryDetail += " order by " + dtSortOrder;
            }
            if (this._condition != string.Empty)    //truong hop tim kiem theo bang detail
            {
                string subQuery = queryDetail + " where " + this._condition;
                subQuery = subQuery.Replace("*", mtPk);
                queryMain += " where " + mtPk + " in (" + subQuery + ")";
                queryDetail += " where " + mtPk + " in (" + queryMain.Replace("*", mtPk) + ")";
                if (extrasql != string.Empty)
                    queryDetail += " and " + extrasql;
                if (mtSortOrder != string.Empty)
                    queryMain += " order by " + mtSortOrder;
                if (dtSortOrder != string.Empty)
                    queryDetail += " order by " + dtSortOrder;
            }
        }

        public override void GetData()
        {
            ConditionForPackage();
            string query = string.Empty, queryMaster = string.Empty;
            this.GetQuery(ref queryMaster, ref query);
            DsData = DbData.GetDataSetDetail(queryMaster, query);

            string fkName = _drTableMaster["Pk"].ToString();
            DataColumn pk = DsData.Tables[1].Columns[fkName];
            DataColumn fk = DsData.Tables[0].Columns[fkName];
            if (pk != null && fk != null)
            {
                DataRelation dr = new DataRelation(_drTable["TableName"].ToString(), pk, fk, true);
                DsData.Relations.Add(dr);
            }
           // DsData.Tables[0].PrimaryKey = new DataColumn[] { DsData.Tables[0].Columns[_drTable["pk"].ToString()] };
            if (DsData != null)
                _dsDataTmp = DsData.Copy();
        }

        public override bool UpdateData(DataAction dataAction)
        {
            if (!_dataChanged)
                return true;
            
            bool isError = false;
            try
            {
                DbData.BeginMultiTrans();
                int index = DsData.Tables[0].Rows.IndexOf(_drCurrentMaster);
               
                if (!_customize.BeforeUpdate(index, DsData))
                {
                    DbData.RollbackMultiTrans();
                    return false;
                }

                bool isNew = _drCurrentMaster.RowState == DataRowState.Added;
                if (Update(_drCurrentMaster))
                {

                    isError = isError || !TransferData(dataAction, index);
                    _customize.AfterUpdate();
                    isError = isError || !DoUpdating();
                }
                else
                {
                    isError = true;
                }
                //
                if (!isError)
                {
                    DbData.EndMultiTrans();
                    DoAfterUpdate();
                }
                else
                    DbData.RollbackMultiTrans();
                if (isNew && !isError)
                    _autoIncreValues.UpdateNewStruct(_drCurrentMaster);
                if (!isError)
                {
                    base.InsertHistory(dataAction, DsData);
                    DsData.AcceptChanges();
                    _dsDataTmp = DsData.Copy();
                }
                DataChanged = false;
            }
            finally
            {
                if (DbData.Connection.State != ConnectionState.Closed)
                    DbData.Connection.Close();
            }
            return (!isError);
        }

        public override DataTable GetDataForPrint(int index)
        {
            return (this.DsData.Tables[0]);
        }
        public override DataTable GetDataForPrint(int index,string _script)
        {
            return (this.DsData.Tables[0]);
        }
    }
}
