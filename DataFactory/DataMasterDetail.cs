using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTLib;
using CDTControl;
using System.Threading;
using ErrorManager;
using System.Collections;
using System.Windows.Forms;
namespace DataFactory
{
    public class DataMasterDetail : CDTData
    {
        public bool isApp;
        public bool isUyQuyen=false;
        public bool isWF = false;
        public event DoAction JustDoaction;
        
        public override event EventHandler DataSoureChanged;
        public event CheckTable eventCheckRulesDetail;
        public DataMasterDetail(string sysTableID)
        {
            this._dataType = DataType.MasterDetail;
            this.GetInfor(sysTableID);
            this.GetStruct();
            this._formulaCaculator = new FormulaCaculator(_dataType, _dsStruct, _dsStructDt);
            this._customize = new Customize(_dataType, DbData, _drTable, _drTableMaster, PkMaster.FieldName);
            this._formulaCaculator.drTable = this._drTable;
            this._formulaCaculator.drTableMaster = this._drTableMaster;
            this._formulaCaculator.drTableDt = this._drTableDt;
            this._formulaCaculator.LstDrCurrentDetails = this.LstDrCurrentDetails;
            //this.JustDoaction += DataMasterDetail_JustDoaction;
            //this.JustUpdated += DataMasterDetail_JustUpdated;
        }

        private void DataMasterDetail_JustUpdated()
        {
            
        }

        private void DataMasterDetail_JustDoaction(DataRow dataRow)
        {
            
        }

        public DataMasterDetail(string TableName, string sysPackageID)
        {
            this._dataType = DataType.MasterDetail;
            this.GetInfor(TableName, sysPackageID);
            if (this._drTable == null) return;
            this.GetStruct();
            if (this._drTableMaster == null) return;
            this._formulaCaculator = new FormulaCaculator(_dataType, _dsStruct, _dsStructDt);
            this._formulaCaculator.LstDrCurrentDetails = this.LstDrCurrentDetails;
            this._formulaCaculator.drTable = this._drTable;
            this._formulaCaculator.drTableMaster = this._drTableMaster;
            this._formulaCaculator.drTableDt = this._drTableDt;
            this._customize = new Customize(_dataType, DbData, _drTable, _drTableMaster, PkMaster.FieldName);
            //this.JustDoaction += DataMasterDetail_JustDoaction;
            //this.JustUpdated += DataMasterDetail_JustUpdated;
        }

        public DataMasterDetail(DataRow drTable)
        {
            this._dataType = DataType.MasterDetail;
            this.GetInfor(drTable);
            this.GetStruct();
            if (this._drTableMaster == null) return;
            this._formulaCaculator = new FormulaCaculator(_dataType, _dsStruct, _dsStructDt);
            this._formulaCaculator.LstDrCurrentDetails = this.LstDrCurrentDetails;
            this._formulaCaculator.drTable = this.DrTable;
            this._formulaCaculator.drTableMaster = this._drTableMaster;
            this._formulaCaculator.drTableDt = this._drTableDt;
            this._customize = new Customize(_dataType, DbData, _drTable, _drTableMaster, PkMaster.FieldName);
            this.JustDoaction += DataMasterDetail_JustDoaction;
            //this.JustUpdated += DataMasterDetail_JustUpdated;
        }

        private void GetInforForMaster()
        {
            string mtTableName = this._drTable["MasterTable"].ToString();
            string sysPackageID = Config.GetValue("sysPackageID").ToString();
            string userid = Config.GetValue("sysUserID").ToString().Trim();
            string sql = "select * from sysTable t left join (select * from sysUserTable where sysuserpackageid =" + Config.GetValue("sysUserPackageID").ToString() + ") ut on t.sysTableID = ut.sysTableID where t.TableName = '" + mtTableName +
                "' and t.sysPackageID = " + sysPackageID ;
                DataTable dt = _dbStruct.GetDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                this._drTableMaster = dt.Rows[0];
                DbData.MasterPk = _drTableMaster["Pk"].ToString();
            }
            else
            {   //trường hợp dữ liệu nằm ở CDT
                dt = _dbStruct.GetDataTable("select * from sysTable t left join sysUserTable ut on t.sysTableID = ut.sysTableID where t.TableName = '" + mtTableName + "' and t.sysPackageID = 5");
                if (dt != null && dt.Rows.Count > 0)
                {
                    this._drTableMaster = dt.Rows[0];
                    DbData.MasterPk = _drTableMaster["Pk"].ToString();
                }
            }
            
            //lấy thông tin Approved
            isApp = false;
            //if (_drTableMaster["App"] == DBNull.Value) isApp = false;
            //else isApp = bool.Parse(_drTableMaster["App"].ToString());
            //Lấy thông tin ủy quyền
            object b = _dbStruct.GetValue("select count(*) from systable where tableName='sysUyquyen'");
            if (b == null) return;
            int exits = int.Parse(b.ToString());
            if (exits > 0)
            {
                object o = _dbStruct.GetValue("select count(*) from sysUyquyen where systableid=" + DrTable["systableid"].ToString() + " and sysUserID=" + Config.GetValue("sysUserID").ToString().Trim());
                int countUQ = int.Parse(o.ToString());
                if (countUQ > 0)
                {
                    isUyQuyen = true;
                }
            }
        }
        //Đây là phần thêm vào để quản lý các Detail đi kèm detail chính
        private void GetInforForDetail()
        {
            string mtTableName = this._drTable["MasterTable"].ToString();
            string sysPackageID = Config.GetValue("sysPackageID").ToString();

            _dtDetail = _dbStruct.GetDataTable("select * from sysDetail t where t.sysTableID =" + _drTable["systableID"].ToString());

            _dtDtReport = _dbStruct.GetDataTable("select v.*,t1.* from sysDtReport t left join sysreport v on t.sysreportid=v.sysreportid left join systable t1 on v.mtTableID=t1.systableid left join systable t2 on v.dtTableID=t1.systableid  where t.sysTableID =" + _drTable["systableID"].ToString());
            if (_dtDetail != null)
            {
                foreach (DataRow dr in _dtDetail.Rows)
                {
                    DataTable dt = _dbStruct.GetDataTable("select * from systable t left join  sysUserTable ut on t.sysTableID = ut.sysTableID where t.sysTableID=" + dr["sysDetailID"].ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        _drTableDt.Add(dt.Rows[0]);
                        string queryString = "select * from sysField f left join sysUserField uf on f.sysFieldID = uf.sysFieldID " +
                            " where f.sysTableID = " + dr["sysDetailID"].ToString() + " order by TabIndex";
                        DataTable dtStruct = _dbStruct.GetDataTable(queryString);
                        
                        if (dtStruct != null)
                        {
                            _dsStructDt.Tables.Add(dtStruct);
                            dtStruct.TableName = dt.Rows[0]["TableName"].ToString();
                        }
                        if (dt.Columns.Contains("useBand") && bool.Parse(dt.Rows[0]["useBand"].ToString()))
                        {
                            string sql = "select * from sysband where MTID=" + dt.Rows[0]["sysTableID"].ToString();
                            DataTable tbband = this._dbStruct.GetDataTable(sql);
                            tbband.TableName = dt.Rows[0]["TableName"].ToString();
                            if (tbband != null)
                                _dsBand.Tables.Add(tbband);

                        }
                    }
                }
            }
        }
        //Đây là phần thêm vào để quản lý các Detail đi kèm detail chính
        public override void GetInfor(string sysTableID)
        {
            base.GetInfor(sysTableID);
            if (this._drTable == null) return;
            DbData.DetailPk = _drTable["Pk"].ToString();
            GetInforForMaster();
            GetInforForDetail();
        }

        public override void GetInfor(DataRow drTable)
        {
            base.GetInfor(drTable);
            if (this._drTable == null) return;
            DbData.DetailPk = _drTable["Pk"].ToString();
            GetInforForMaster();
            GetInforForDetail();

        }

        public override void GetInfor(string TableName, string sysPackageID)
        {
            base.GetInfor(TableName, sysPackageID);
            if (this._drTable == null) return;
            DbData.DetailPk = _drTable["Pk"].ToString();
            GetInforForMaster();
            GetInforForDetail();
        }

        public override void GetStruct()
        {
            if (_drTableMaster == null) return;
            string sysTableID = _drTableMaster["SysTableID"].ToString();
            string queryString ;
            if(Config.Variables.Contains("MaCN"))
                queryString = "select *, d.CurrValue  from sysField f left join (select * from sysUserField where  sysuserpackageid =" + Config.GetValue("sysUserPackageID").ToString() + ") uf " +
                    " on f.sysFieldID = uf.sysFieldID left join (select * from sysCurValue  where MaCN='" + Config.GetValue("MaCN").ToString() + "' and sysdbid=" + Config.GetValue("sysDBID").ToString() + ")  d on f.sysfieldid=d.sysfieldid " + 
                    " where  f.sysTableID = " + sysTableID + " order by TabIndex";
            else
                queryString = "select *, d.CurrValue  from sysField f left join (select * from sysUserField where  sysuserpackageid  =" + Config.GetValue("sysUserPackageID").ToString() + ") uf " +
                   " on f.sysFieldID = uf.sysFieldID left join (select * from sysCurValue  where sysdbid=" + Config.GetValue("sysDBID").ToString() + ")  d on f.sysfieldid=d.sysfieldid " +
                   " where  f.sysTableID = " + sysTableID + " order by TabIndex";
            DataTable dtStruct = _dbStruct.GetDataTable(queryString);
            if (dtStruct != null)
                    _dsStruct.Tables.Add(dtStruct);
            base.GetStruct();
        }
        private void CheckRules1Detail(DataAction dataAction, DataTable dtStruct, ref DataRow drData)
        {
            string note = "";
            string DtKeyName = "";
            DataRow[] drDtKey = dtStruct.Select("type=0 or type=3 or type=6");
            if (drDtKey.Length > 0)
            {
                DtKeyName = drDtKey[0]["FieldName"].ToString();
            }
            if (drDtKey.Length > 0 && (drDtKey[0]["type"].ToString() == "0" || drDtKey[0]["type"].ToString() == "6"))
            {
                note = "'";
            }
            foreach (DataRow drField in dtStruct.Rows)
            {//Kiểm tra ẩn hiện theo điều kiện
                //if (drField["Visible"].ToString()=="0")
                //    continue;
                string fieldName = drField["FieldName"].ToString();
                int pType = Int32.Parse(drField["Type"].ToString());
                if (pType == 3 || pType == 6 || pType == 4)
                {
                    drData.SetColumnError(fieldName, string.Empty);
                    continue;
                }

                if (drData.RowState == DataRowState.Deleted || drData.RowState == DataRowState.Detached)
                    continue;
                string fieldValue = drData[fieldName].ToString();
                bool AllowNull = drField["AllowNull"].ToString() == "1";
                if (drField["AllowNull"] != DBNull.Value && drField["AllowNull"].ToString() != "0" && drField["AllowNull"].ToString() != "1")
                {
                    string conditionNull = DtKeyName + "=" + note + drData[DtKeyName].ToString() + note + " and (" + drField["AllowNull"].ToString() + ")";
                    try
                    {
                        DataRow[] ldrMt = drData.Table.Select(conditionNull);
                        AllowNull = (ldrMt.Length != 0);
                    }
                    catch
                    {
                        conditionNull = PkMaster.FieldName + "=" + quote + DrCurrentMaster[this.PkMaster.FieldName] + quote + " and (" + drField["AllowNull"].ToString() + ")";
                        DataRow[] ldrMt = DrCurrentMaster.Table.Select(conditionNull);
                        AllowNull = (ldrMt.Length != 0);
                    }
                }


                if (!AllowNull && fieldValue == string.Empty)
                {
                    drData.SetColumnError(fieldName, "Phải nhập");
                    //LogFile.AppendToFile("CheckErr.txt", fieldName + "_" + "Phải nhập");
                }
                else
                    drData.SetColumnError(fieldName, string.Empty);

                if (fieldValue == string.Empty)
                    continue;

                decimal value = 0;
                if (!decimal.TryParse(drData[fieldName].ToString(), out value))
                    continue;
                //Kiểm tra min - max
                if (drField["MinValue"].ToString() != string.Empty)
                {
                    decimal minValue = 0;
                    if (drField["MinValue"].ToString().Contains("@"))
                    {
                        string field = drField["MinValue"].ToString().Replace("@", "");

                        if (_drCurrentMaster.Table.Columns.Contains(field))
                        {
                            try
                            {
                                minValue = decimal.Parse(_drCurrentMaster[field].ToString());
                            }
                            catch
                            {
                                //LogFile.AppendToFile("CheckErr.txt", fieldName + "_" + "Sai định dạng số");
                            }
                        }
                        else if (drData.Table.Columns.Contains(field))
                        {
                            try
                            { minValue = decimal.Parse(drData[field].ToString()); }
                            catch
                            { //LogFile.AppendToFile("CheckErr.txt", fieldName + "_" + "Sai định dạng số"); 
                            }
                        }

                    }
                    else
                    {
                        minValue = Int32.Parse(drField["MinValue"].ToString());
                    }
                    if (minValue > value)
                    {
                        drData.SetColumnError(fieldName, "Phải lớn hơn hoặc bằng " + minValue.ToString());
                        //LogFile.AppendToFile("CheckErr.txt", fieldName + "_" + "Nhỏ hơn min");
                        continue;
                    }
                    else
                        drData.SetColumnError(fieldName, string.Empty);
                }
                if (drField["MaxValue"].ToString() != string.Empty)
                {
                    decimal maxValue = 0;
                    if (drField["MaxValue"].ToString().Contains("@"))
                    {
                        string field = drField["MaxValue"].ToString().Replace("@", "");

                        if (_drCurrentMaster.Table.Columns.Contains(field))
                        {
                            try
                            {
                                maxValue = decimal.Parse(_drCurrentMaster[field].ToString());
                            }
                            catch { }
                        }
                        else if (drData.Table.Columns.Contains(field))
                        {
                            try
                            {
                                maxValue = decimal.Parse(drData[field].ToString());
                            }
                            catch { }
                        }

                    }
                    else
                    {
                        maxValue = Int32.Parse(drField["MaxValue"].ToString());
                    }
                    if (maxValue < value)
                    {
                        drData.SetColumnError(fieldName, "Phải nhỏ hơn hoặc bằng " + maxValue.ToString());
                        //LogFile.AppendToFile("CheckErr.txt", fieldName + "_" + "Lớn hơn Max");
                    }
                    else
                        drData.SetColumnError(fieldName, string.Empty);
                }
            }
        }
        public override void CheckRules(DataAction dataAction)
        {

            base.CheckRules(dataAction);



            foreach (DataRow drData in _lstDrCurrentDetails)
            {
                DataRow dr = drData;
                CheckRules1Detail(dataAction, _dsStruct.Tables[1], ref dr);
            }
            //Kiểm tra duy nhất
            foreach (DataRow drField in _dsStruct.Tables[1].Rows)
            {
                try
                {
                    if (bool.Parse(drField["isUnique"].ToString()))
                    {
                        string filedName = drField["fieldName"].ToString();

                        for (int i = 0; i < _lstDrCurrentDetails.Count; i++)
                        {
                            DataRow drData = _lstDrCurrentDetails[i];

                            for (int j = i + 1; j < _lstDrCurrentDetails.Count; j++)
                            {
                                DataRow drData1 = _lstDrCurrentDetails[j];
                                if (drData.RowState == DataRowState.Deleted || drData.RowState == DataRowState.Detached || drData1.RowState == DataRowState.Deleted || drData1.RowState == DataRowState.Detached) continue;
                                if (drData[filedName].ToString() == drData1[filedName].ToString())
                                {
                                    drData.SetColumnError(filedName, "Trùng");
                                    drData1.SetColumnError(filedName, "Trùng");
                                    //LogFile.AppendToFile("CheckErr.txt", filedName + "_" + "Trùng");
                                    return;
                                }
                            }
                        }


                    }
                }catch
                {

                }
            }
            foreach (CurrentRowDt CRDt in _lstCurRowDetail)
            {
                DataRow dr = CRDt.RowDetail;
                CheckRules1Detail(dataAction, _dsStructDt.Tables[CRDt.TableName], ref dr);
            }
            bool result = eventCheckRulesDetail?.Invoke(_drTable["TableName"].ToString()) ?? false;
     
        }

        private void GetQuery(ref string queryMain, ref string queryDetail, ref string SubQuery)
        {
            if (this.DrTableMaster == null) return;
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
                if (_drTable["Extrasql"] != null)
                {
                    extrasql = _drTable["Extrasql"].ToString();
                }

            }
            string extraWs = string.Empty;
            if (_drTableMaster["sysUserID"] != DBNull.Value)
            {
                string adminList = _drTableMaster["sysUserID"].ToString().Trim();
                if (adminList != string.Empty)
                {
                    if (adminList != Config.GetValue("sysUserID").ToString().Trim())
                    {
                        string dk = "(1=1)";// NotAdminListCondition();
                      
                       // extraWs = " ((charindex('" + Config.GetValue("sysUserID").ToString().Trim() + "_',ws)>0 ";
                        //extraWs += " and  Approved=0 ) or Approved>0";
                       // if (dk != string.Empty)
                         //   extraWs += " or (" + dk + "))";
                        extraWs = dk; 
                    }
                    else
                    { extraWs = "1=1 "; }//and Approved <> -1
                }
                else
                {
                    extraWs = "1=1";
                }                
            }
            else
            {
                extraWs = "1=1";
            }
            if (maCT != string.Empty && mtSortOrder != string.Empty && !mtSortOrder.ToUpper().Contains("DESC") && !mtSortOrder.ToUpper().Contains("ASC"))
            {
                mtSortOrder = mtSortOrder.Replace(",", " desc ,");
                mtSortOrder += " desc";
            }
            queryMain = "select 0 as HasComment,* from " + mtTableName;
            queryMain += " where " + extraWs;
            //if (extrasql != string.Empty)
            //    queryMain += " and " + extrasql;

            queryDetail = "select * from " + dtTableName;
            SubQuery = queryMain.Replace("0 as HasComment,*", mtPk);
            //queryMain = UpdateSpecialCondition(queryMain);
            string fkName = _drTableMaster["Pk"].ToString();
            DataRow[] RelaRow = this._dsStruct.Tables[1].Select("refTable='" + mtTableName + "'");
            string RelaCol = fkName;
            if (RelaRow.Length > 0) RelaCol = RelaRow[0]["FieldName"].ToString();
           
           // DataColumn fk = _DsData.Tables[1].Columns[RelaCol];

            if (this._conditionMaster == string.Empty && this._condition == string.Empty)   //truong hop mac dinh
            {
                if (maCT == string.Empty)
                {
                    if (mtSortOrder != string.Empty)
                        queryMain += " order by " + mtSortOrder;
                   // queryMain = "select * from (" + queryMain + " )x order by Approved";
                }
                else
                {
                    int rowCount = 30;
                    object oRowCount = Config.GetValue("RowCount");
                    if (oRowCount != null)
                        rowCount = Int32.Parse(oRowCount.ToString());
                    queryMain = "select top " + rowCount.ToString() + "  0 as HasComment,* from " + mtTableName;
                    //Thêm vào điều kiện ws
                    queryMain += " where " + extraWs;
                    if (extrasql != string.Empty)
                        queryMain += " and " + extrasql;
                    if (_conditionTask != string.Empty)
                        queryMain += " and " + _conditionTask;
                    queryMain = UpdateSpecialCondition(queryMain);

                    if (mtSortOrder != string.Empty)
                        queryMain += " order by " + mtSortOrder;
                    else
                        queryMain += " order by " + mtPk + " desc";

                    SubQuery = queryMain.Replace("0 as HasComment,*", mtPk);
                    queryDetail += " where " + RelaCol + " in (" + SubQuery + ")";
                    queryMain = "select * from (" + queryMain + " )x order by Approved";
                    if (mtSortOrder != string.Empty)
                        queryMain += ", " + mtSortOrder;
                }
                if (dtSortOrder != string.Empty)
                    queryDetail += " order by " + dtSortOrder;
                

            }
            if (this._conditionMaster != null && this._conditionMaster != string.Empty)  //truong hop tim kiem theo bang master
            {
                queryMain += " and (" + this._conditionMaster + ")";
                if (_conditionTask != string.Empty)
                    queryMain += " and " + _conditionTask;
                queryMain = UpdateSpecialCondition(queryMain);
                string subQuery = queryMain.Replace("0 as HasComment,*", mtPk);
                queryDetail += " where " + mtPk + " in (" + subQuery + ")";
                SubQuery = queryMain.Replace("0 as HasComment,*", mtPk);
                if (mtSortOrder != string.Empty)
                    queryMain += " order by Approved," + mtSortOrder;
                else queryMain += " order by Approved";
                if (dtSortOrder != string.Empty)
                    queryDetail += " order by " + dtSortOrder;
            }

            if (this._condition != null && this._condition != string.Empty)    //truong hop tim kiem theo bang detail
            {
                string subQuery = queryDetail + " where (" + this._condition + ")";
                subQuery = subQuery.Replace("*", mtPk);
                queryMain += " and " + mtPk + " in (" + subQuery + ")";
                if (_conditionTask != string.Empty)
                    queryMain += " and " + _conditionTask;
                subQuery = queryMain.Replace("0 as HasComment,*", mtPk);
                queryMain = UpdateSpecialCondition(queryMain);
                SubQuery = queryMain.Replace("0 as HasComment,*", mtPk);
                queryDetail += " where " + RelaCol + " in (" + SubQuery + ")";
                if (mtSortOrder != string.Empty)
                    queryMain += " order by " + mtSortOrder;
                if (dtSortOrder != string.Empty)
                    queryDetail += " order by " + dtSortOrder;
            }
            
        }
        private string NotAdminListCondition()
        {
            string dk = string.Empty;
            string ws = Config.GetValue("sysUserID").ToString().Trim();
            string tableid = this.DrTableMaster["systableid"].ToString().Trim();
            string sql = "select condition from sysAdminDM where systableid=" + tableid + " and (sysUserID=" + ws + " or sysUserGroupID  in (select sysUserGroupID from sysUser where sysUserID=" + ws + " ))";
            DataTable tbCon = this._dbStruct.GetDataTable(sql);
            
            if (tbCon.Rows.Count > 0)
            {
                dk = tbCon.Rows[0]["condition"].ToString();
            }
            else
            {
                dk = "1=0";
            }
            return dk;
        }
        public override void GetData()
        {
            DateTime btime = DateTime.Now;
            DateTime etime = DateTime.Now;
            ConditionForPackage();
            string query = string.Empty, queryMaster = string.Empty; string SubQuery = string.Empty;
            this.GetQuery(ref queryMaster, ref query, ref SubQuery);
            if (query == string.Empty) return;
            DbData.BeginMultiTrans();
            DataSet _DsData = DbData.GetDataSetMasterDetail(queryMaster, query);
            //LogFile.AppendToFile("Log.txt", queryMaster);
            //LogFile.AppendToFile("Log.txt", query);
            DbData.EndMultiTrans();
            if (_DsData == null)
                return;
            string fkName = _drTableMaster["Pk"].ToString();

            string mtTableName = this._drTableMaster["TableName"].ToString();
            DataRow[] RelaRow = this._dsStruct.Tables[1].Select("refTable='" + mtTableName + "'");
            string RelaCol = fkName;
            if (RelaRow.Length > 0) RelaCol = RelaRow[0]["FieldName"].ToString();
            DataColumn pk = _DsData.Tables[0].Columns[fkName];
            DataColumn fk = _DsData.Tables[1].Columns[RelaCol];
            if (PkMaster.DbType != SqlDbType.Int)
                _DsData.Tables[0].PrimaryKey = new DataColumn[] { _DsData.Tables[0].Columns[fkName] };
            if (pk != null && fk != null)
            {
                DataRelation dr = new DataRelation(_drTable["TableName"].ToString(), pk, fk, true);
                try
                {
                    _DsData.Relations.Add(dr);
                }
                catch
                {
                    foreach(DataRow drdt in _DsData.Tables[1].Rows)
                    {
                        if(_DsData.Tables[0].Rows.Find(drdt[fkName]) == null)
                        {
                            ErrMessageBox.Show("Không tìm thấy khóa chính", drdt[fkName].ToString());
                        }
                    }
                }
            }
            //Đây là phần thêm vào để lấy dữ liệu từ các detail phụ
            string mtPk = this._drTableMaster["Pk"].ToString();
            int i = 0;
            foreach (DataRow dr in _drTableDt)
            {
                DataRow[] isNhomDk = _dsStructDt.Tables[i].Select("FieldName='NhomDK'");
                //isNhomDK này dùng để xác định bảng chi tiết này có được config dữ liệu từ nơi khác không, nếu có thì loại nó ra
                string queryDt = "select * from " + dr["TableName"].ToString() + " where MTID in (" + SubQuery + ")";
                if (isNhomDk.Length > 0) queryDt += " and NhomDK is null";
                if (dr["SortOrder"] != DBNull.Value && dr["SortOrder"].ToString().Trim() != string.Empty) queryDt += " order by " + dr["SortOrder"].ToString().Trim();
                DataTable dtDt = DbData.GetDataTable(queryDt);
                dtDt.TableName = dr["TableName"].ToString();
                if (dtDt != null) _DsData.Tables.Add(dtDt);


                if (!bool.Parse(_dtDetail.Rows[i]["ChildOf"].ToString()))
                {
                    DataColumn pk1 = _DsData.Tables[1].Columns[_drTable["Pk"].ToString()];
                    DataColumn fkCol1 = dtDt.Columns["DTID"];
                    DataRelation dre1 = new DataRelation(dr["TableName"].ToString() + "1", pk1, fkCol1, true);
                    try
                    {
                        _DsData.Relations.Add(dre1);
                    }
                    catch
                    {
                        foreach (DataRow drdt in dtDt.Rows)
                        {
                            if (_DsData.Tables[1].Rows.Find(drdt[fkCol1]) == null)
                            {
                                ErrMessageBox.Show("Không tìm thấy khóa chính", drdt[fkName].ToString());
                            }
                        }
                    }

                }
                
                    DataColumn fkCol = dtDt.Columns["MTID"];
                    DataRelation dre = new DataRelation(dr["TableName"].ToString(), pk, fkCol, true);
                    _DsData.Relations.Add(dre);
                
                i++;

            }
            //Đây là phần thêm vào để lấy dữ liệu từ các detail phụ
            DsData = _DsData;
            if (DsData != null)
            {
                DsData.Tables[0].TableName = _drTableMaster["TableName"].ToString();
                DsData.Tables[1].TableName = _drTable["TableName"].ToString();
                _dsDataTmp = DsData.Copy();
            }
            etime = DateTime.Now;
            base.InsertHistory(DataAction.IUD, _dsDataTmp, btime, etime);

        }


        private string GenConditionForLookup(CDTData ParentData)
        {
            string s = string.Empty;
            string tableName = _drTableMaster["TableName"].ToString().ToUpper();
            foreach (DataRow drField in ParentData.DsStruct.Tables[0].Rows)
            {
                string refTable = drField["RefTable"].ToString().ToUpper();
                string datatype = drField["type"].ToString();
                if (refTable == string.Empty || tableName != refTable || !"147".Contains(datatype))
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
                    if (i > 1000) 
                        break;
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
            for (int itemp = 0; itemp < ParentData._dsStructDt.Tables.Count; itemp++)
            {
                DataTable tbStruct = ParentData._dsStructDt.Tables[itemp];



                foreach (DataRow drField in tbStruct.Rows)
                {
                    string refTable = drField["RefTable"].ToString().ToUpper();
                    if (refTable == string.Empty || tableName != refTable)
                        continue;
                    if (s == string.Empty)
                        s = drField["refField"].ToString() + " in (";
                    string fieldName = drField["FieldName"].ToString();
                    //if (fieldName.ToUpper() == "REFTABLE" || fieldName.ToUpper() == "REFFIELD")
                    //    continue;
                    int n = ParentData.DsData.Tables[itemp + 2].Rows.Count;
                    if (first && n > 0 && !s.EndsWith("("))
                    {
                        s += ",";
                    }
                    first = false;
                    for (int i = 0; i < n; i++)
                    {
                        DataRow drData = ParentData.DsData.Tables[itemp + 2].Rows[i];
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
        public bool doAction(DataRow drAction)
        {
            //     

            if (!DataChanged)
           //     if(true)
            {
                string condition = "";
                if (drAction["Condition"] != DBNull.Value && drAction["Condition"].ToString() != string.Empty)
                {

                    condition = drAction["Condition"].ToString();
                    condition = "(" + condition + ") and " + this.PkMaster.FieldName + "=" + quote + this.DrCurrentMaster[PkMaster.FieldName].ToString() + quote;
                    DataRow[] lstDr = DrCurrentMaster.Table.Select(condition);
                    if (lstDr.Length == 0)
                    {

                        return true;
                    }
                }
                try
                {


                    if (!doActionCommand(drAction)) return false;

                    DataRow[] lETask = tbTask.Select("Id='" + drAction["ETId"].ToString() + "'");

                    if (lETask.Length > 0)
                    {
                        //sql = "update " + DrTableMaster["TableName"].ToString() + " set TaskID='" + drAction["ETId"].ToString() + "' where " + PkMaster.FieldName + "='" + this.DrCurrentMaster[PkMaster.FieldName].ToString() + "'";
                        //if (sql != string.Empty) DbData.UpdateByNonQuery(sql)
                        //sql = "update " + DrTableMaster["TableName"].ToString() + " set Approved='" + lETask[0]["ApprovedStt"].ToString() + "' where " + PkMaster.FieldName + "='" + this.DrCurrentMaster[PkMaster.FieldName].ToString() + "'";
                        // DbData.UpdateByNonQuery(sql);
                        DrCurrentMaster["TaskID"] = drAction["ETid"];
                        DrCurrentMaster["Approved"] = lETask[0]["ApprovedStt"];
                        //Set para
                        DataRow[] lPara = this.tbActionPara.Select("ActionID='" + drAction["ID"].ToString() + "'");
                        foreach(DataRow drPara in lPara)
                        {
                            if (DrCurrentMaster.Table.Columns.Contains(drPara["FieldName"].ToString()))
                            {
                                if (DrCurrentMaster.Table.Columns[drPara["FieldName"].ToString()].DataType == typeof(string))
                                {
                                    DrCurrentMaster[drPara["FieldName"].ToString()] = drPara["Value"].ToString();
                                }
                                else if (DrCurrentMaster.Table.Columns[drPara["FieldName"].ToString()].DataType == typeof(Guid))
                                {
                                    DrCurrentMaster[drPara["FieldName"].ToString()] =Guid.Parse(drPara["Value"].ToString());
                                }
                                else if (DrCurrentMaster.Table.Columns[drPara["FieldName"].ToString()].DataType == typeof(decimal))
                                {
                                    if (drPara["Value"] != DBNull.Value && drPara["Value"].ToString()!=string.Empty)
                                        DrCurrentMaster[drPara["FieldName"].ToString()] = double.Parse(drPara["Value"].ToString());
                                    else if (drPara["Formular"] != DBNull.Value)
                                    {
                                        try
                                        {
                                           Formula.BieuThuc bt = new Formula.BieuThuc(drPara["Formular"].ToString());
                                            Hashtable h = new Hashtable();
                                            foreach (string variable in bt.variables)
                                            {
                                                if (DrCurrentMaster.Table.Columns.Contains(variable))
                                                {
                                                    h.Add(variable, double.Parse(DrCurrentMaster[variable].ToString()));
                                                }
                                                else
                                                {
                                                    h.Add(variable, 0);
                                                }
                                            }
                                            DrCurrentMaster[drPara["FieldName"].ToString()] = bt.Evaluate(h);
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }


                                }
                                else if (DrCurrentMaster.Table.Columns[drPara["FieldName"].ToString()].DataType == typeof(bool))
                                {
                                    DrCurrentMaster[drPara["FieldName"].ToString()] = drPara["Value"].ToString() == "1" ? true : false;
                                }
                            }
                        }
                        if (!UpdateDataWithAction(DataAction.Update, drAction))
                        {
                            DrCurrentMaster["TaskID"] = drAction["BTid"];
                            DataRow[] lBTask = tbTask.Select("Id='" + drAction["BTId"].ToString() + "'");
                            DrCurrentMaster["Approved"] = lBTask[0]["ApprovedStt"];
                            DataChanged = false;
                            return false;
                        }
                        else
                        {
                            //this.JustDoaction?.Invoke(drAction);
                        }

                    }


                }
                catch(Exception ex)
                {
                    LogFile.AppendToFile("log.txt","Lỗi:\n" + ex.Message);
                    return false;
                }
               string sql = "insert into sysActionHistory (ActionID, BTID, ETID,PkID,sysUserID) values ('" + drAction["ID"].ToString() + "','" + drAction["BTID"].ToString() + "','" + drAction["ETID"].ToString() + "','" + this.DrCurrentMaster[PkMaster.FieldName].ToString() + "'," + Config.GetValue("sysUserID").ToString() + ")";
                _dbStruct.UpdateByNonQuery(sql);
                
               // this.JustDoaction(drAction);
            }
            return true;
        }
        public bool doActionCommand(DataRow drAction)
        {
            string sql = drAction["Command"].ToString().ToUpper();
            if (sql.Trim() == "") return true;
            sql = sql.Replace("@@" + PkMaster.FieldName.ToUpper(), "'" + this.DrCurrentMaster[PkMaster.FieldName].ToString() + "'");
            sql = UpdateSpecialCondition(sql);
            if (sql != string.Empty) DbData.UpdateByNonQuery(sql);

            if (DbData.HasErrors) return false;
            else
            {
               
                    Thread sendmail = new Thread(SendMail);
                    sendmail.Start(drAction);
               // }
                
                return true;
            }
        }
        public bool doActionCommandAfter(DataRow drAction)
        {
            string sql = drAction["AfterUpdate"].ToString().ToUpper();
            if (sql.Trim() == "") return true;
            sql = sql.Replace("@@" + PkMaster.FieldName.ToUpper(), "'" + this.DrCurrentMaster[PkMaster.FieldName].ToString() + "'");
            sql = UpdateSpecialCondition(sql);
            if (sql != string.Empty) DbData.UpdateByNonQuery(sql);

            if (DbData.HasErrors)
                return false;
            else
            {
               // LogFile.AppendToFile("log.txt", sql);
                Thread sendmail = new Thread(SendMail);
                sendmail.Start(drAction);
                sql = "insert into sysActionHistory (ActionID, BTID, ETID,PkID,sysUserID) values ('" + drAction["ID"].ToString() + "','" + drAction["BTID"].ToString() + "','" + drAction["ETID"].ToString() + "','" + this.DrCurrentMaster[PkMaster.FieldName].ToString() + "'," + Config.GetValue("sysUserID").ToString() + ")";
                _dbStruct.UpdateByNonQuery(sql);
                return true;
            }
        }
        private void SendMail(object drA)
        {
            DataRow drAction = drA as DataRow;
            if (drAction == null) return;
            if (drAction["SendMail"] != DBNull.Value && bool.Parse(drAction["SendMail"].ToString()) && drAction["MailContent"] != DBNull.Value)
            {
                //Send mail for staff
                string Content = drAction["MailContent"].ToString();
                Content = UpdateSpecialCondition(Content);
                MailControl mailcontrol = new MailControl(1);
                string ETask = drAction["ETID"].ToString();
                string sql = "select Email from (" +
                "select b.Email from sysuserTask a inner join sysuser b on a.sysuserID = b.sysUserID " +
                "  where getmail = 1 and b.Email is not null and a.TaskID ='" + ETask + "'" + 
                " union all " +
                " select c.Email from sysuserGrTask a inner join sysUserGroup b on a.sysUserGroupID = b.sysUserGroupID inner join sysUser c on b.sysUserGroupID = c.sysuserGroupID " +
                " where a.getmail = 1 and c.Email is not null and a.TaskID ='" + ETask + "'" +
                " )x group by Email";
                CDTDatabase.Database dbS = CDTDatabase.Database.NewStructDatabase();
                DataTable lEmail = dbS.GetDataTable(sql);
                if (lEmail == null) return ;
                foreach (DataRow dr in lEmail.Rows)
                {
                    mailcontrol.SendMail(dr["Email"].ToString(), Content, drAction["CommandName"].ToString() + " has complete!");
                }
            }
            if (_drCurrentMaster.Table.Columns.Contains("MaKH") && drAction["SendMailKH"] != DBNull.Value && bool.Parse(drAction["SendMailKH"].ToString()) && drAction["MailContentKH"] != DBNull.Value)
            {
                //Send mail for staff
                string Content = drAction["MailContentKH"].ToString();
                Content = UpdateSpecialCondition(Content);
                MailControl mailcontrol = new MailControl(1);
                string ETask = drAction["ETID"].ToString();
                string sql = "select Email from dmKH where MaKH='" + DrCurrentMaster["MaKH"].ToString() + "'";
                CDTDatabase.Database db = CDTDatabase.Database.NewDataDatabase();
                DataTable lEmail = db.GetDataTable(sql);
                if (lEmail == null) return;
                foreach (DataRow dr in lEmail.Rows)
                {
                    mailcontrol.SendMail(dr["Email"].ToString(), Content, Config.GetValue("CompanyName").ToString());
                }
            }
        }
        private new string UpdateSpecialCondition(string query)
        {
            if (DsData == null || DsData.Tables.Count == 0 || DsData.Tables[0].Rows.Count == 0)
            {
                query = base.UpdateSpecialCondition(query);
                return query;
            }

            DataRow drData = DrCurrentMaster;
            if (drData != null)
            {
                query = base.UpdateSpecialCondition(query);
                foreach (DataRow drField in _dsStruct.Tables[0].Rows)
                {
                    string fieldName = drField["FieldName"].ToString();

                    string value = drData[fieldName].ToString();
                    if (drData.Table.Columns[fieldName].DataType == typeof(System.Int32))
                    {
                        query = query.Replace("@@" + fieldName, value);
                    }
                    else //if(drData.Table.Columns[fieldName].DataType == typeof(string) || drData.Table.Columns[fieldName].DataType == typeof(System.DateTime))
                    {
                        query = query.Replace("@@" + fieldName, "'" + value + "'");
                    }

                }
            }
           
            return query;
        }
        public void UpdateBeginTask()
        {
            if (tbTask != null)
            {
                foreach (DataRow dr in tbTask.Rows)
                    if (dr["isBegin"].ToString() == "True")
                    {
                        string sql = "update " + DrTableMaster["TableName"] + " set TaskID='" + dr["ID"].ToString() + "' where " + PkMaster.FieldName + " =" + quote + DrCurrentMaster[PkMaster.FieldName].ToString() + quote;
                        DbData.UpdateByNonQuery(sql);
                        DrCurrentMaster["TaskID"] = dr["ID"];
                        DrCurrentMaster.AcceptChanges();
                        this.DataChanged = false;
                        break;
                    }
            }
            foreach (DataRow dr in tbAction.Rows)
            {
                if (bool.Parse(dr["AutoDo"].ToString()) && dr["BTId"].ToString() == DrCurrentMaster["TaskID"].ToString())
                {
                    doAction(dr);
                }
            }
        }
        private bool UpdateDetail()
        {
            if (_sInsertDetail == string.Empty)
                GenSqlString();
            bool success = true;
            bool isDelete = false;
            if (_drCurrentMaster.RowState == DataRowState.Deleted)
            {
                isDelete = true;
                _drCurrentMaster.RejectChanges();
            }
            foreach (DataRow drDetail in _lstDrCurrentDetails)
            {
                List<SqlField> tmp = new List<SqlField>();
                List<string> paraNames = new List<string>();
                List<object> paraValues = new List<object>();
                List<SqlDbType> paraTypes = new List<SqlDbType>();
                string sql = string.Empty;
                bool isDeleteDt = false,  updateIdentity = false;
                success = false;
                switch (drDetail.RowState)
                {
                    case DataRowState.Added:
                        if (_identityPkDt)
                            updateIdentity = true;
                        if (_identityPk)
                        {
                            string pk = _drTableMaster["Pk"].ToString();
                            if (drDetail.Table.Columns.Contains(pk))
                                drDetail[pk] = _drCurrentMaster[pk];
                        }
                        tmp = _vInsertDetail;
                        sql = _sInsertDetail;
                        break;
                    case DataRowState.Modified:
                        tmp = _vUpdateDetail;
                        sql = _sUpdateDetail;
                        break;
                    case DataRowState.Deleted:
                        tmp = _vDeleteDetail;
                        sql = _sDeleteDetail;
                        
                        drDetail.RejectChanges();
                        isDeleteDt = true;
                        break;
                }
                
                if (sql != string.Empty)
                {
                    foreach (SqlField sqlField in tmp)
                    {
                        string fieldName = sqlField.FieldName;
                        paraNames.Add(fieldName);
                        if (drDetail[fieldName].ToString() != string.Empty)
                            paraValues.Add(drDetail[fieldName]);
                        else
                            paraValues.Add(DBNull.Value);
                        paraTypes.Add(sqlField.DbType);
                    }
                    
                    if (isDeleteDt)
                    {
                        success = UpdateMultiDetail(drDetail);
                        if (!success) return success;
                        success = success && DbData.UpdateData(sql, paraNames.ToArray(), paraValues.ToArray(), paraTypes.ToArray());
                        flagRowdeleteHand = true;
                        drDetail.Delete();
                        flagRowdeleteHand = false;
                    }
                    else
                    {
                        success = UpdateMultiDetail(drDetail);
                        if (!success) return success;
                        success =success && DbData.UpdateData(sql, paraNames.ToArray(), paraValues.ToArray(), paraTypes.ToArray());
                    }
                    
                    //success = DbData.UpdateData(sql, paraNames.ToArray(), paraValues.ToArray(), paraTypes.ToArray());
                    if (success && updateIdentity)
                    {
                        string pk = _drTable["Pk"].ToString();
                        object o = DbData.GetValue("select @@identity");
                        if (o != null)
                            drDetail[pk] = o;
                    }
                    if (!success)
                        break;

                        
                }
                else
                {
                    if (drDetail.RowState != DataRowState.Detached)
                        success = UpdateMultiDetail(drDetail);
                    else
                    {
                        success = true;
                    }
                }
                    
            }
            if (isDelete)
                _drCurrentMaster.Delete();
            success = success && UpdateMultiDetail();
            return success;
        }
        private bool UpdateMultiDetail(DataRow drDT)
        {

            bool success = true;
            for (int i = 0; i < _dtDetail.Rows.Count; i++)
            {
                string tableName = _drTableDt[i]["TableName"].ToString();
                DataRow _drDetail = _dtDetail.Rows[i];
                if (bool.Parse(_drDetail["ChildOf"].ToString())) continue;
                foreach (CurrentRowDt CrdrDetail in _lstCurRowDetail)
                {
                    bool isDeleteDt = false, isDelete = false, updateIdentity = false;
                    DataRow drDetail = CrdrDetail.RowDetail;
                    List<SqlField> tmp = new List<SqlField>();
                    List<string> paraNames = new List<string>();
                    List<object> paraValues = new List<object>();
                    List<SqlDbType> paraTypes = new List<SqlDbType>();
                    string sql = string.Empty;
                    if (CrdrDetail.RowDetail.RowState == DataRowState.Detached) continue;
                    bool isDeleteDetail = false;

                    if (CrdrDetail.fkKey.ToString() != drDT[_drTable["pk"].ToString()].ToString()) continue;
                    
                    if (CrdrDetail.RowDetail.RowState == DataRowState.Deleted)
                    {
                        CrdrDetail.RowDetail.RejectChanges();
                        isDeleteDt = true;
                    }
                    if (CrdrDetail.TableName != tableName || drDetail["DTID"].ToString() != drDT[_drTable["pk"].ToString()].ToString())
                    {
                        if (isDeleteDt) drDetail.Delete();
                        continue;
                    }

                    if (isDeleteDt) drDetail.Delete();

                     success = false;
                    switch (drDetail.RowState)
                    {
                        case DataRowState.Added:
                            if (_identityPkMulDt[i])
                                updateIdentity = true;
                            if (_identityPk)
                            {
                                string pk = _drTableMaster["Pk"].ToString();
                                if (drDetail.Table.Columns.Contains(pk))
                                    drDetail[pk] = _drCurrentMaster[pk];
                            }
                            tmp = _vInsertDt[i];
                            sql = _sInsertDt[i];
                            break;
                        case DataRowState.Modified:
                            tmp = _vUpdateDt[i];
                            sql = _sUpdateDt[i];
                            break;
                        case DataRowState.Deleted:
                            tmp = _vDeleteDt[i];
                            sql = _sDeleteDt[i];
                            if (drDT.RowState == DataRowState.Deleted)
                            {
                                isDelete = true;
                                drDT.RejectChanges();
                            }
                            drDetail.RejectChanges();
                            isDeleteDt = true;
                            break;
                    }
                    if (sql != string.Empty)
                    {
                        foreach (SqlField sqlField in tmp)
                        {
                            string fieldName = sqlField.FieldName;
                            paraNames.Add(fieldName);
                            if (drDetail[fieldName].ToString() != string.Empty)
                                paraValues.Add(drDetail[fieldName]);
                            else
                                paraValues.Add(DBNull.Value);
                            paraTypes.Add(sqlField.DbType);
                        }

                        if (isDeleteDt)
                        {
                            flagRowdeleteHand = true;
                            drDetail.Delete();
                            flagRowdeleteHand = false;

                        }
                        success = DbData.UpdateData(sql, paraNames.ToArray(), paraValues.ToArray(), paraTypes.ToArray());
                        if (success && updateIdentity)
                        {
                            string pk = _drTableDt[i]["Pk"].ToString();
                            object o = DbData.GetValue("select @@identity");
                            if (o != null)
                                drDetail[pk] = o;
                        }
                        if (!success)
                            break;
                    }
                    else
                        success = true;
                }
                if (!success) return success;
            }
            return success;
        }
        private bool UpdateMultiDetail()
        {

            bool success = true;
            for (int i = 0; i < _dtDetail.Rows.Count; i++)
            {
                string tableName = _drTableDt[i]["TableName"].ToString();
                DataRow _drDetail = _dtDetail.Rows[i];
                if (!bool.Parse(_drDetail["ChildOf"].ToString())) continue;
                foreach (CurrentRowDt CrdrDetail in _lstCurRowDetail)
                {
                    if (CrdrDetail.TableName != tableName) continue;
                    DataRow drDetail = CrdrDetail.RowDetail;
                    List<SqlField> tmp = new List<SqlField>();
                    List<string> paraNames = new List<string>();
                    List<object> paraValues = new List<object>();
                    List<SqlDbType> paraTypes = new List<SqlDbType>();
                    string sql = string.Empty;
                    bool isDeleteDt = false, isDelete = false, updateIdentity = false;
                    success = false;
                    switch (drDetail.RowState)
                    {
                        case DataRowState.Added:
                            if (_identityPkMulDt[i])
                                updateIdentity = true;
                            if (_identityPk)
                            {
                                string pk = _drTableMaster["Pk"].ToString();
                                if (drDetail.Table.Columns.Contains(pk))
                                    drDetail[pk] = _drCurrentMaster[pk];
                            }
                            tmp = _vInsertDt[i];
                            sql = _sInsertDt[i];
                            break;
                        case DataRowState.Modified:
                            tmp = _vUpdateDt[i];
                            sql = _sUpdateDt[i];
                            break;
                        case DataRowState.Deleted:
                            tmp = _vDeleteDt[i];
                            sql = _sDeleteDt[i];
                            if (_drCurrentMaster.RowState == DataRowState.Deleted)
                            {
                                isDelete = true;
                                _drCurrentMaster.RejectChanges();
                            }
                            drDetail.RejectChanges();
                            isDeleteDt = true;
                            break;
                    }
                    if (sql != string.Empty)
                    {
                        foreach (SqlField sqlField in tmp)
                        {
                            string fieldName = sqlField.FieldName;
                            paraNames.Add(fieldName);
                            if (drDetail[fieldName].ToString() != string.Empty)
                                paraValues.Add(drDetail[fieldName]);
                            else
                                paraValues.Add(DBNull.Value);
                            paraTypes.Add(sqlField.DbType);
                        }
                        if (isDelete)
                            _drCurrentMaster.Delete();
                        if (isDeleteDt)
                        {
                            flagRowdeleteHand = true;
                            drDetail.Delete();
                            flagRowdeleteHand = false;
                        }
                        success = DbData.UpdateData(sql, paraNames.ToArray(), paraValues.ToArray(), paraTypes.ToArray());
                        if (success && updateIdentity)
                        {
                            string pk = _drTableDt[i]["Pk"].ToString();
                            object o = DbData.GetValue("select @@identity");
                            if (o != null)
                                drDetail[pk] = o;
                        }
                        if (!success)
                            break;
                    }
                    else
                        success = true;
                }
                if (!success) return success;
            }
            return success;
        }

        public override bool UpdateData(DataAction dataAction)
        {
            DateTime btime = DateTime.Now;
            DateTime etime = DateTime.Now;
            TimeSpan configDataTime;
            TimeSpan updateTime;
            if (!_dataChanged)
                return true;
            bool isError = false;
            bool isAlreadyTrans = false;
            try
            {
                
                if (DbData.Connection.State == ConnectionState.Open && DbData.multiStatement)
                {
                    isAlreadyTrans = true;
                }
                else
                {
                    btime = DateTime.Now;
                    DbData.BeginMultiTrans();
                    isAlreadyTrans = false;
                }
                this.CheckRules(dataAction);
                foreach (DataRow dr in _lstDrCurrentDetails)
                {
                    if (dr.HasErrors)
                    {
                        DbData.RollbackMultiTrans();
                        LogFile.AppendToFile("CheckErr.txt", dr.GetColumnsInError()[0].ColumnName + "_" + "Có lỗi: " + _drCurrentMaster.GetColumnError(_drCurrentMaster.GetColumnsInError()[0]));
                        return false;
                    }
                }
                if (_drCurrentMaster.Table.DataSet.HasErrors)
                {
                    if (!isAlreadyTrans)
                        DbData.RollbackMultiTrans();
                    else
                    {

                    }    
                    LogFile.AppendToFile("CheckErr.txt", _drCurrentMaster.GetColumnsInError()[0].ColumnName + "_" + "Có lỗi: " + _drCurrentMaster.GetColumnError(_drCurrentMaster.GetColumnsInError()[0]));
                    return false;
                }

                int index = _drCurrIndex;
                if (index == -1)
                {
                    index = DsData.Tables[0].Rows.IndexOf(_drCurrentMaster);
                    if (index == -1)
                    {
                  DataRow dataRowtmp=     DsData.Tables[0].Rows.Find(_drCurrentMaster[PkMaster.FieldName].ToString());
                        if(dataRowtmp !=null) index = DsData.Tables[0].Rows.IndexOf(dataRowtmp);
                    }
                }
                if (index == -1)
                {
                    DbData.RollbackMultiTrans();
                    LogFile.AppendToFile("CheckErr.txt", "Lỗi không xác định được Index");
                    return false;
                }
                if (!_customize.BeforeUpdate(index, DsData))
                {
                    //if (!isAlreadyTrans)
                    DbData.RollbackMultiTrans();
                    //LogFile.AppendToFile("CheckErr.txt", "Lỗi custome");
                    return false;
                }

                DataRow[] arrDrCurrentDetails = new DataRow[_lstDrCurrentDetails.Count];
                _lstDrCurrentDetails.CopyTo(arrDrCurrentDetails);
                bool isNew = _drCurrentMaster.RowState == DataRowState.Added;
                bool isAllowUpdate = true;
                this._formulaCaculator.Active = false;

                //Update Begin Task
                if (isNew && tbTask != null && _drCurrentMaster.Table.Columns.Contains("TaskID") && _drCurrentMaster["TaskID"] == DBNull.Value)
                {
                    foreach (DataRow dr in tbTask.Rows)
                        if (dr["isBegin"].ToString() == "True")
                        {
                            _drCurrentMaster["TaskID"] = dr["ID"].ToString();
                            _drCurrentMaster["Approved"] = int.Parse(dr["ApprovedStt"].ToString());
                        }
                }
                if(dataAction != DataAction.Delete)
                {
                    bool UpdateMt = Update(_drCurrentMaster);
                    if (UpdateMt)
                    {
                        bool UpdateDt = UpdateDetail();
                        if (!UpdateDt)
                        {
                           // ErrMessageBox.Show("Lỗi update DT", "Lỗi update DT");
                            isError = true;
                        }
                        
                    }
                    else
                    {
                       // ErrMessageBox.Show("Lỗi update MT", "Lỗi update MT");
                        isError = true;
                    }                        
                }
                else
                {
                    bool UpdateDt = UpdateDetail();
                    
                    if (UpdateDt)
                    {
                        bool UpdateMt = Update(_drCurrentMaster);
                        if (!UpdateMt)
                        {
                           // ErrMessageBox.Show("Lỗi update MT", "Lỗi update MT");
                            isError = true;
                        }
                    }
                    else
                    {
                      //  ErrMessageBox.Show("LLỗi update DT", "Lỗi update DT");
                        isError = true;
                    }    
                }
                if(!isError)
                {
                    etime = DateTime.Now;
                    updateTime = etime - btime;
                    //isError = false;
                    //if(int.Parse(this.DrCurrentMaster["Approved"].ToString() == 1))
                    isError = !TransferData(dataAction, index);
                    isAllowUpdate = _customize.AfterUpdate();
                    configDataTime = DateTime.Now - etime;
                    isError = isError || !DoUpdating();
                }
                else
                {
                   
                    if (!DbData.HasErrors)
                    {
                        ErrMessageBox.Show("Lỗi Tranfer", " Không biết vì sao lỗi");
                    }
                    isError = true;
                }
              //  this._formulaCaculator.Active = true;
               //isError = DbData.HasErrors || !isAllowUpdate;
                if (!isError)
                {
                    //Tạo sự kiện JustUpdate-> plugins
                    DbData.EndMultiTrans();
                    DoAfterUpdate();
                    _formulaCaculator.Active = false;
                    //_formulaCaculator.Active = true;
                    etime = DateTime.Now;
                    base.InsertHistory(dataAction, DsData, btime, etime);
                    DsData.AcceptChanges();
                    _dsDataTmp = DsData.Copy();
                    this.DataChanged = false;
                    bool ActionResult = true;
                    //Update Begin Task

                    if (!isError && dataAction != DataAction.Delete)
                    {
                        foreach (DataRow dr in tbAction.Rows)
                        {
                            if (bool.Parse(dr["AutoDo"].ToString()) && dr["BTId"].ToString() == _drCurrentMaster["TaskID"].ToString())
                            {
                                ActionResult = doAction(dr);
                                if (!ActionResult) break;
                            }
                        }
                    }
                    if (ActionResult && !isError)
                    {

                    }
                    else
                    {
                        LogFile.AppendToFile("CheckErr.txt", "Lỗi Doaction");
                        if (!isAlreadyTrans && DbData.strMain != null)
                            DbData.RollbackMultiTrans();
                    }
                    
                    
                }
                //
                else
                {
                    //if (!isAlreadyTrans)
                    DbData.RollbackMultiTrans();
                    LogFile.AppendToFile("CheckErr.txt", "Lỗi Update");
                }

                if (isNew && !isError)
                    _autoIncreValues.UpdateNewStruct(_drCurrentMaster);

            }
            catch(Exception ex)
            {
                if (!isAlreadyTrans )
                    DbData.RollbackMultiTrans();
                ErrMessageBox.Show(ex.Message, ex.Source + "\n" + ex.StackTrace);
                isError = true;
            }
            finally
            {

                if (this.DbData.Connection.State != ConnectionState.Closed && !isAlreadyTrans)
                    this.DbData.Connection.Close();
                if (this._formulaCaculator != null)
                {
                    this._formulaCaculator.Active = true;
                }
            }
            return (!isError);
            
        }
        public  bool UpdateDataWithAction(DataAction dataAction,DataRow drAction)
        {
            DateTime btime = DateTime.Now;
            DateTime etime = DateTime.Now;
            TimeSpan configDataTime=new TimeSpan();
            TimeSpan updateTime = new TimeSpan();
            TimeSpan Docommandtime = new TimeSpan();
            DataSet dsDatatmp;
            if (!_dataChanged)
                return true;
            bool isError = false;
            bool isAlreadyTrans = false;
            try
            {
                if (DbData.Connection.State == ConnectionState.Open && DbData.multiStatement)
                {
                    //isAlreadyTrans = true;
                }
                else
                {
                    btime = DateTime.Now;
                    DbData.BeginMultiTrans();
                    isAlreadyTrans = false;
                }
                this.CheckRules(dataAction);
                
                foreach (DataRow dr in _drCurrentMaster.Table.DataSet.Tables[1].Rows)
                {
                    if (dr.HasErrors)
                    {
                        DbData.RollbackMultiTrans();
                        LogFile.AppendToFile("CheckErr.txt", dr.GetColumnsInError()[0].ColumnName + "_" + "Có lỗi: " + _drCurrentMaster.GetColumnError(_drCurrentMaster.GetColumnsInError()[0]));
                        return false;
                    }
                }


                
                if (_drCurrentMaster.Table.DataSet.HasErrors)
                {
                    string Err = "";
                    string CollErr = "";
                    if (DrCurrentMaster.Table.HasErrors)
                    {
                        foreach (DataColumn col in _drCurrentMaster.Table.Columns)
                        {
                            string err = _drCurrentMaster.GetColumnError(col);
                            if (err != string.Empty)
                            {
                                Err = col.ColumnName +" " +  err;
                                CollErr = col.ColumnName;
                                break;
                            }
                        }
                    }
                    if (DsData.Tables[1].HasErrors)
                    {
                        foreach(DataRow dr in DsData.Tables[1].Rows)
                        {
                            foreach (DataColumn col in dr.Table.Columns)
                            {
                                string err = dr.GetColumnError(col);
                                if (err != string.Empty)
                                {
                                    Err = col.ColumnName + " " + err;
                                    CollErr = col.ColumnName;
                                    break;
                                }
                            }
                        }
                    }
                    foreach (CurrentRowDt CRDt in _lstCurRowDetail)
                    {
                        foreach (DataColumn col in CRDt.RowDetail.Table.Columns)
                        {
                            string err = CRDt.RowDetail.GetColumnError(col);
                            if (err != string.Empty)
                            {
                                Err = col.ColumnName + " " + err;
                                CollErr = col.ColumnName;
                                break;
                            }
                        }
                            DataRow dr = CRDt.RowDetail;
                        CheckRules1Detail(dataAction, _dsStructDt.Tables[CRDt.TableName], ref dr);
                    }
                    
                     LogFile.AppendToFile("CheckErr.txt", CollErr + "_" + "Có lỗi: " + Err);
                    if (!isAlreadyTrans)
                        DbData.RollbackMultiTrans();
                    return false;
                }
                int index = DsData.Tables[0].Rows.IndexOf(_drCurrentMaster);
                if (index == -1)
                {
                    index = DsData.Tables[0].Rows.IndexOf(_drCurrentMaster);
                    if (index == -1)
                    {
                        DataRow dataRowtmp = DsData.Tables[0].Rows.Find(_drCurrentMaster[PkMaster.FieldName].ToString());
                        if (dataRowtmp != null) index = DsData.Tables[0].Rows.IndexOf(dataRowtmp);
                    }
                }
                if (!_customize.BeforeUpdate(index, DsData))
                {
                    //if (!isAlreadyTrans)
                    DbData.RollbackMultiTrans();
                    return false;
                }

                DataRow[] arrDrCurrentDetails = new DataRow[_lstDrCurrentDetails.Count];
                _lstDrCurrentDetails.CopyTo(arrDrCurrentDetails);
                bool isNew = _drCurrentMaster.RowState == DataRowState.Added;
                bool isAllowUpdate = true;
                this._formulaCaculator.Active = false;

                //Update Begin Task
                if (isNew && tbTask != null && _drCurrentMaster.Table.Columns.Contains("TaskID") && _drCurrentMaster["TaskID"] == DBNull.Value)
                {
                    foreach (DataRow dr in tbTask.Rows)
                        if (dr["isBegin"].ToString() == "True")
                        {
                            _drCurrentMaster["TaskID"] = dr["ID"].ToString();
                            _drCurrentMaster["Approved"] = int.Parse(dr["ApprovedStt"].ToString());
                        }
                }
                if (dataAction != DataAction.Delete)
                {
                    bool UpdateMt = Update(_drCurrentMaster);
                    if (UpdateMt)
                    {
                        bool UpdateDt = UpdateDetail();
                        if (!UpdateDt)
                        {
                            // ErrMessageBox.Show("Lỗi update DT", "Lỗi update DT");
                            isError = true;
                        }

                    }
                    else
                    {
                        // ErrMessageBox.Show("Lỗi update MT", "Lỗi update MT");
                        isError = true;
                    }
                }
                else
                {
                    bool UpdateDt = UpdateDetail();

                    if (UpdateDt)
                    {
                        bool UpdateMt = Update(_drCurrentMaster);
                        if (!UpdateMt)
                        {
                            // ErrMessageBox.Show("Lỗi update MT", "Lỗi update MT");
                            isError = true;
                        }
                    }
                    else
                    {
                        //  ErrMessageBox.Show("LLỗi update DT", "Lỗi update DT");
                        isError = true;
                    }
                }
                if (!isError)
                {
                    //if(int.Parse(this.DrCurrentMaster["Approved"].ToString() == 1))
                    etime = DateTime.Now;
                    updateTime = etime - btime;
                    if (bool.Parse(drAction["DoConfigData"].ToString()))
                    {
                       isError =isError || !TransferData(dataAction, index);
                    }
                    isAllowUpdate = _customize.AfterUpdate();
                    configDataTime = DateTime.Now - etime;
                  
                }
                else
                {
                    if (!DbData.HasErrors)
                    {
                        ErrMessageBox.Show("Lỗi Tranfer with action", " Không biết vì sao lỗi");
                    }
                    isError = true;
                }                    
                
                
                if (!isError)
                {
                    //Tạo sự kiện JustUpdate-> plugins
                    
                    bool ActionResult = true;
                    //Update Begin Task
                    etime = DateTime.Now;
                    bool docommand= doActionCommandAfter(drAction);
                    Docommandtime = DateTime.Now - etime;
                    if (!isError) isError = isError || !docommand;
                    
                    
                    //isError = isError || !ActionResult;
                    if (ActionResult && !isError)
                    {
                        //if (!isAlreadyTrans)
                        // {
                        DbData.EndMultiTrans();
                        _formulaCaculator.Active = false;
                        DsData.AcceptChanges();
                        this.DataChanged = false;
                        _formulaCaculator.Active = true;
                        etime = DateTime.Now;
                        base.InsertHistory(dataAction, DsData, btime, etime);
                        if (!isError)
                        {
                            this.JustDoaction?.Invoke(drAction);
                        }
                        if (!isError && dataAction != DataAction.Delete)
                        {
                            foreach (DataRow dr in tbAction.Rows)
                            {
                                if (bool.Parse(dr["AutoDo"].ToString()) && dr["BTId"].ToString() == _drCurrentMaster["TaskID"].ToString())
                                {
                                    //Chỗ này phải cho DsData Acceptchange
                                    dsDatatmp = DsData.Copy();
                                    //DsData.AcceptChanges();
                                    this.DataChanged = false;
                                    ActionResult = doAction(dr);
                                    if (!ActionResult)
                                    {
                                        DsData = _dsDataTmp;
                                        break;
                                    }
                                }
                            }
                        }
                        _dsDataTmp = DsData.Copy();
                        //}
                    }
                    else
                    {
                        if (!isAlreadyTrans && DbData.strMain != null)
                            DbData.RollbackMultiTrans();
                    }
                }
                //
                else
                {
                    //if (!isAlreadyTrans)
                    DbData.RollbackMultiTrans();
                }

                if (isNew && !isError)
                    _autoIncreValues.UpdateNewStruct(_drCurrentMaster);
                string s = "update time:" + updateTime.TotalMilliseconds.ToString() + "   Config data:" + configDataTime.TotalMilliseconds.ToString() + "  Do Command time" + Docommandtime.TotalMilliseconds.ToString();
                   // LogFile.AppendToFile("log.txt", s);
            }
            catch(Exception ex)
            {
                if (!isAlreadyTrans)
                    DbData.RollbackMultiTrans();
                ErrMessageBox.Show(ex.Message, ex.Source + "\n" + ex.StackTrace);
                isError = true;
            }
            finally
            {

                if (this.DbData.Connection.State != ConnectionState.Closed && !isAlreadyTrans)
                    this.DbData.Connection.Close();
                if (this._formulaCaculator != null)
                {
                    this._formulaCaculator.Active = true;
                }
            }
            return (!isError);
        }
        private void CreatePrintVoucher()
        {
            string mtTableID = this._drTableMaster["sysTableID"].ToString();
            string dtTableID = this._drTable["sysTableID"].ToString();
            this._printData = new DataMasterDetailPrint(mtTableID, dtTableID);

        }

        public override DataTable GetDataForPrint(int index)
        {
            
            string pk = _drTableMaster["Pk"].ToString();
            string dataID = DsData.Tables[0].Rows[index][pk].ToString();
            if (_printData == null)
                CreatePrintVoucher();
            DataTable tb=_printData.GetData(dataID);
            PrintSQL = _printData.GetSQL;
            return tb;
        }
        public string PrintSQL = string.Empty;
        public override DataTable GetDataForPrint(int index, string _Script)
        {

            string pk = _drTableMaster["Pk"].ToString();
            string dataID = DsData.Tables[0].Rows[index][pk].ToString();
            
            if (_printData == null)
                CreatePrintVoucher();
            if (DsData.Tables[0].Columns[pk].DataType == typeof(int))
                _Script = _Script.Replace("@@" + pk, dataID);
            else
                _Script = _Script.Replace("@@" + pk, "'" + dataID + "'");
            PrintSQL = _Script;
            return (_printData.GetData(dataID, _Script));
        }
        public void UpdateLanIn(int index)
        {
            try
            {
                string pk = _drTableMaster["Pk"].ToString();
                string dataID = DsData.Tables[0].Rows[index][pk].ToString();
                string mtTableName = _printData._mtTableName;
                string sql = "update " + mtTableName + " set Lanin=Lanin+1 where " + pk + " ='" + dataID + "'";
                this.DbData.UpdateByNonQuery(sql);
            }
            catch  { }
        }
        public DataTable GetReportFile(string TableIDid)
        {
            string sql = "select * from sysReportFile where sysTableID=" + TableIDid + " order by stt";
            DataTable tbMau = _dbStruct.GetDataTable(sql);
            return tbMau;
        }
        //mới thêm vào
        public DataTable UpdateDetailFromMTDT(int i)
        {
           
            if (_dtDetail != null&& _dtDetail.Rows.Count>i)
            {
                try
                {
                    DbData.BeginMultiTrans();
                    int DetailIndex = 0;
                    
                    DataRow dr = _dtDetail.Rows[i];

                    if (dr["InsertQuery"].ToString() == string.Empty || dr["InsertQuery"] == DBNull.Value)
                    {                        
                        return null;
                    }
                    CreateTableDetailTemp(dr);
                    string[] lstSaveField = dr["lstSaveField"].ToString().Replace(" ", "").Split(",".ToCharArray());
                    foreach (DataRow drDetail in _lstDrCurrentDetails)
                    {
                        if (drDetail.RowState == DataRowState.Deleted || drDetail.RowState == DataRowState.Detached) continue;
                        InsertRowTotable(drDetail, lstSaveField);
                    }

                    DataTable tmp = DbData.GetDataTable(dr["InsertQuery"].ToString());
                    if (tmp != null)
                    {
                        tmp.TableName = dr["sysDetailID"].ToString();                        
                        string sql = "drop table dttmp";
                        DbData.UpdateByNonQuery(sql);
                        DetailIndex++;
                    }


                    if (DbData.HasErrors)
                        DbData.RollbackMultiTrans();
                    else
                        DbData.EndMultiTrans();
                    return tmp;
                }
                finally
                {
                    if (DbData.Connection.State != ConnectionState.Closed)
                        DbData.Connection.Close();
                }
            }
            return null;
        }
        private void InsertRowTotable(DataRow drDetail, string[] lstSaveField)
        {
            string s = "insert into dttmp (";
            string va = " values(";
            foreach (string f in lstSaveField)
            {
                if (f.Trim() == string.Empty) continue;
                if (drDetail.RowState == DataRowState.Deleted || drDetail.RowState == DataRowState.Detached) continue;
                if (drDetail[f] == DBNull.Value) continue;
                s += f + ",";
                if (_dsData.Tables[1].Columns.Contains(f))
                {
                    if (_dsData.Tables[1].Columns[f].DataType == typeof(string) )
                    {
                        va += "N'" + drDetail[f].ToString() + "',";
                    }
                    else if (_dsData.Tables[1].Columns[f].DataType == typeof(Guid))
                    {
                        va += "'" + drDetail[f].ToString() + "',";
                    }
                    else
                    {
                        va += drDetail[f].ToString().Replace(",", ".") + ",";
                    }
                }
                else if (_dsData.Tables[0].Columns.Contains(f))
                {
                    if (_dsData.Tables[0].Columns[f].DataType == typeof(string))
                    {
                        va += "N'" + _drCurrentMaster[f].ToString() + "',";
                    }
                    else if  ( _dsData.Tables[0].Columns[f].DataType == typeof(Guid))
                    {
                        va += "'" + _drCurrentMaster[f].ToString() + "',";
                    }
                        else
                    {
                        va += _drCurrentMaster[f].ToString().Replace(",", ".") + ",";
                    }
                }
            }
            s += "ws)" + va + Config.GetValue("sysUserID").ToString() + ")";

            DbData.UpdateByNonQuery(s);
        }

        private void CreateTableDetailTemp(DataRow dr)
        {
            string[] lstSaveField = dr["lstSaveField"].ToString().Replace(" ","").Split(",".ToCharArray());
            string queryString = GenStructString(lstSaveField);
            this.DbData.UpdateByNonQuery(queryString);

        }
        private string GenStructString(string[] lstSaveField)
        {

            string s = "create table dttmp( ";
            for (int i = 0; i < lstSaveField.Length; i++)
            {
                DataRow[] dr = DsStruct.Tables[1].Select("FieldName='" + lstSaveField[i].ToLower().Trim() + "'");
                if (dr.Length > 0)
                {
                    s += GenField(dr[0], false) + ",";
                }
                else
                {
                    DataRow[] drMT = DsStruct.Tables[0].Select("FieldName='" + lstSaveField[i].ToLower().Trim() + "'");
                    if (drMT.Length > 0)
                    {
                        s += GenField(drMT[0], false) + ",";
                    }
                }
                // if (i != drColumns.Count - 1)
                //   s += ",";
            }
            s += " ws nvarchar(4000) )";
            //s += ")";
            return s;
        }
        /// <summary>
        /// Phát sinh chuỗi cú pháp tạo cột trong bảng từ số liệu 1 dòng trong sysField
        /// </summary>
        private string GenField(DataRow dr, bool withConstraint)
        {
            string s = " " + dr["FieldName"].ToString() + " ";

            string strType = string.Empty;
            int pType = Int32.Parse(dr["Type"].ToString());
            //0: text(pk); 1: text(fk); 2: text; 3: int(pk); 4: int(fk); 5: int; 6: unique identifier; 
            //7: unique identifier(fk); 8: decimal; 9: date; 10: boolean; 11: time; 12: image;
            switch (pType)
            {
                case 0:
                case 1:
                    strType = "varchar(32)";
                    break;
                case 2:
                    strType = "nvarchar(128)";
                    break;
                case 3:
                    strType = "int IDENTITY ";
                    break;
                case 4:
                case 5:
                    strType = "int ";
                    break;
                case 6:
                case 7:
                    strType = "uniqueidentifier ";
                    break;
                case 8:
                    strType = "decimal(20,6)";
                    break;
                case 9:
                case 14:
                    strType = "smalldatetime";
                    break;
                case 10:
                    strType = "bit";
                    break;
                case 11:
                    strType = "smalldatetime";
                    break;
                case 12:
                    strType = "image";
                    break;
                case 13:
                    strType = "ntext";
                    break;
            }
            s += strType;

            //if (dr["AllowNull"].ToString() != string.Empty)
            //{
            //    bool isNull = Boolean.Parse(dr["AllowNull"].ToString());
            //    if (isNull == false)
            //        s += " not null ";
            //}

            //if (withConstraint)
            //{
            //    if (dr["DefaultValue"].ToString() != string.Empty)
            //        s += " default " + dr["DefaultValue"].ToString();
            //    if (dr["RefTable"].ToString() != string.Empty)
            //    {
            //        string RefTable = dr["RefTable"].ToString();
            //        string RefField = dr["RefField"].ToString();
            //        s += " references " + RefTable + "(" + RefField + ")";
            //    }
            //}
            return s;
        }

        ///<summary>
        ///Approve
        ///</summary>

        public bool Approve()
        {

            //this.DrCurrentMaster["Approved"] = 1;
            //if (this.DrCurrentMaster.Table.Columns.Contains("isReturn"))
            //    this.DrCurrentMaster["isReturn"] = 0;
            //UpdateData(DataAction.Update);
            return true;
        }
        public bool Cancel()
        {
            //try
            //{
            //    this.DrCurrentMaster["Approved"] = -1;
            //    UpdateData(DataAction.Update);
               return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }
        public bool Return(int re)
        {
            //try
            //{
            //    this.DrCurrentMaster["Approved"] = re;
            //    if(this.DrCurrentMaster.Table.Columns.Contains("isReturn"))
            //        this.DrCurrentMaster["isReturn"] = 1;

            //    UpdateData(DataAction.Update);
                return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        
        
    }
}
