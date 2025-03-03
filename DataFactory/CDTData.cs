using System;
using System.Data;
using System.Collections.Generic;
using CDTDatabase;
using CDTControl;
using CDTLib;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Threading;
using ErrorManager;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Remoting;
using System.Diagnostics;

namespace DataFactory
{

    public delegate bool CheckTable(string TableName);
    public delegate void DoAction(DataRow drAction);
    public delegate void CloneDTrow(DataRow drDes);
    public delegate bool DataUpdating();
    public delegate void Addhistory();
    public abstract class CDTData
    {
        // Fields
        internal AutoIncrementValues _autoIncreValues;
        protected string _condition = string.Empty;
        protected string _conditionMaster = string.Empty;
        internal Customize _customize;
        protected bool _dataChanged = false;
        internal DataTransfer _dataTransfer;
        protected DataType _dataType;
        private Database _dbData = Database.NewDataDatabase();
        protected Database _dbStruct = Database.NewStructDatabase();
        protected DataRow _drCurrentMaster;
        public int _drCurrIndex;
        protected DataRow _drTable;
        public List<DataRow> _drTableDt = new List<DataRow>();
        protected DataRow _drTableMaster;
        protected DataSet _dsData;
        protected DataSet _dsDataTmp;
        protected DataSet _dsStruct = new DataSet();
        public DataSet _dsStructDt = new DataSet();
        public DataSet _dsBand = new DataSet();
        public DataTable _dtDetail;
        public DataTable _dtDtReport;
        protected string _DynCondition = string.Empty;
        public FormulaCaculator _formulaCaculator;
        protected bool _identityPk = false;
        protected bool _identityPkDt = false;
        protected List<bool> _identityPkMulDt = new List<bool>();
        public List<CurrentRowDt> _lstCurRowDetail = new List<CurrentRowDt>();
        protected List<DataRow> _lstDrCurrentDetails = new List<DataRow>();
        internal DataMasterDetailPrint _printData;
        protected string _sDelete = string.Empty;
        protected string _sDeleteDetail = string.Empty;
        protected List<string> _sDeleteDt = new List<string>();
        protected string _sInsert = string.Empty;
        protected string _sInsertDetail = string.Empty;
        protected List<string> _sInsertDt = new List<string>();
        protected string _sUpdate = string.Empty;
        protected string _sUpdateDetail = string.Empty;
        protected List<string> _sUpdateDt = new List<string>();
        protected string _sUpdateImage = string.Empty;
        protected string _sUpdateFile = string.Empty; protected string _sInsertFile = string.Empty;
        protected string _sUpdateWs = string.Empty;
        protected List<SqlField> _vDelete = new List<SqlField>();
        protected List<SqlField> _vDeleteDetail = new List<SqlField>();
        protected List<List<SqlField>> _vDeleteDt = new List<List<SqlField>>();
        protected List<SqlField> _vInsert = new List<SqlField>();
        protected List<SqlField> _vInsertDetail = new List<SqlField>();
        protected List<List<SqlField>> _vInsertDt = new List<List<SqlField>>();
        protected List<SqlField> _vUpdate = new List<SqlField>();
        protected List<SqlField> _vUpdateDetail = new List<SqlField>();
        protected List<List<SqlField>> _vUpdateDt = new List<List<SqlField>>();
        protected List<SqlField> _vUpdateImage = new List<SqlField>();
        protected List<SqlField> _vInsertFile = new List<SqlField>();
        protected List<SqlField> _vUpdateFile = new List<SqlField>();
        public List<FileData> _fileData = new List<FileData>();
        internal bool fullData = false;
        public SqlField PkMaster;
        public DataTable tbAction = new DataTable();
        public DataTable tbTask = new DataTable();
        public DataTable tbWF;
        public DataTable tbUTask;
        public DataTable tbUAction;
        public DataTable tbActionPara;
        public string quote = "";
        protected string _conditionTask = string.Empty;
        string _conditionEditTask = string.Empty;
        // Events
        public event EventHandler SetDetailValue;

        public string _tableName;
        // Methods
        public int DataIndexList = 0;
        public event Addhistory AddHis;
        public event Action DataChangedEvent;
        public event Action JustUpdated;
        public event DataUpdating eventDataUpdating;
        public event CheckTable eventCheckRules;
        public event CloneDTrow eventClonedtRow;
        public List<historyCurrent> HistoryCurrents = new List<historyCurrent>();
        public bool DataChanged
        {
            get { return _dataChanged; }
            set { _dataChanged = value;
                if (DataChangedEvent != null)
                    DataChangedEvent?.Invoke();
            }
        }
        protected CDTData()
        {
        }
        public string ConditionMaster
        {
            get
            {
                return this._conditionMaster;
            }
            set
            {
                this._conditionMaster = value;
            }
        }
        public string ConditionEditTask
        {
            get
            {
                return this._conditionEditTask;
            }
            set
            {
                this._conditionEditTask = value;
            }
        }
        public string ConditionTask
        {
            get
            {
                return this._conditionTask;
            }
            set
            {
                this._conditionTask = value;
            }
        }

        public void CancelUpdate()
        {
            if (this._dataChanged)
            {
                this.DsData = this._dsDataTmp.Copy();
            }
        }
        private void GetAction()
        {
            if (DrTable == null) return;
            if (!DrTable.Table.Columns.Contains("Systableid") || DrTable["systableID"] == DBNull.Value) return;
            string sql = "select * from sysWF where sysTableID=" + DrTable["systableID"].ToString();
            tbWF = dbStruct.GetDataTable(sql);
            if (tbWF == null || tbWF.Rows.Count == 0) return;
            sql = "select * from sysAction where WFID='" + tbWF.Rows[0]["ID"].ToString() + "'";
            tbAction = dbStruct.GetDataTable(sql);
            sql = "select * from sysActionPara where ActionID in  (select id from sysAction where WFID='" + tbWF.Rows[0]["ID"].ToString() + "')";
            tbActionPara = dbStruct.GetDataTable(sql);
            sql = "select * from sysTask where WFID='" + tbWF.Rows[0]["ID"].ToString() + "'";
            tbTask = dbStruct.GetDataTable(sql);
            sql = " select TaskID, CView, CEdit, CDelete, Cprint from sysUserTask where sysUserID =" + Config.GetValue("sysUserID").ToString() + " and TaskID in (select Id from sysTask where WFID='" + tbWF.Rows[0]["ID"].ToString() + "')";
            sql += " union all ";
            sql += " select a.TaskID, a.CView, CEdit, CDelete, Cprint from sysUserGrTask a inner join sysuser b on a.sysUserGroupID=b.sysUserGroupID where b.sysUserID=  " + Config.GetValue("sysUserID").ToString() + " and TaskID in (select Id from sysTask where WFID='" + tbWF.Rows[0]["ID"].ToString() + "')";
            tbUTask = dbStruct.GetDataTable(sql);
            sql = " select ActionID, CAllow from sysUserAction where sysUserID =" + Config.GetValue("sysUserID").ToString() + " and ActionID in  (select id from sysAction where WFID='" + tbWF.Rows[0]["ID"].ToString() + "')";
            sql += " union all ";
            sql += " select a.ActionID, a.CAllow  from sysUserGrAction a inner join sysuser b on a.sysUserGroupID=b.sysUserGroupID where b.sysUserID=  " + Config.GetValue("sysUserID").ToString() + " and ActionID in (select Id from sysAction where WFID='" + tbWF.Rows[0]["ID"].ToString() + "')";
            tbUAction = dbStruct.GetDataTable(sql);
            GetUserTaskCondition();

        }
        public void GetUserTaskCondition()
        {
            if (tbUTask == null)
            {
                return;
            }
            if (tbUTask.Rows.Count == 0)
            {
                _conditionTask = "(TaskID is null)";
                _conditionEditTask = "(TaskID is null)";
                return;
            }
            if (tbUTask.Rows.Count > 0)
            {
                _conditionTask = "(TaskID is null ";
                _conditionEditTask = "(TaskID is null ";
                foreach (DataRow dr in tbUTask.Rows)
                {
                    if (dr["CView"] != DBNull.Value && dr["CView"].ToString() != string.Empty)
                    {
                        _conditionTask += " or (TaskID='" + dr["TaskID"].ToString() + "' and (" + dr["CView"].ToString() + "))";
                    }
                    else
                    {
                        _conditionTask += " or (TaskID='" + dr["TaskID"].ToString() + "')";
                    }
                    if (dr["CEdit"] != DBNull.Value && dr["CEdit"].ToString() != string.Empty)
                    {
                        _conditionEditTask += " or (TaskID='" + dr["TaskID"].ToString() + "' and (" + dr["CEdit"].ToString() + "))";
                    }
                    else
                    {
                        _conditionEditTask += " or (TaskID='" + dr["TaskID"].ToString() + "')";
                    }
                }
                _conditionTask += ")";
                _conditionEditTask += ")";
            }
        }
        //Thực thi Action
        public bool InsertNotify(DataRow drAction)
        {
            if (_drCurrentMaster != null && _drCurrentMaster.Table.Columns.Contains("Soct"))
            {
                string sql = "insert into sysnotify (hdatetime,soct, systableid, PkID, TaskID) values (getdate(),'" + _drCurrentMaster["Soct"].ToString();
                sql += "'," + _drTable["systableid"].ToString() + ",'" + _drCurrentMaster[PkMaster.FieldName].ToString() + "','" + drAction["ETId"].ToString() + "')";
                _dbStruct.UpdateByNonQuery(sql);
                return true;
            }
            return false;
        }




        public void Get_fileData4Record()
        {
            _fileData.Clear();
            if (DrCurrentMaster == null) return;
            if (DrCurrentMaster.RowState == DataRowState.Unchanged || DrCurrentMaster.RowState == DataRowState.Modified)
            {
                foreach (DataRow drfield in DsStruct.Tables[0].Rows)
                {
                    if (int.Parse(drfield["Type"].ToString()) == 16|| int.Parse(drfield["Type"].ToString()) ==12)
                    {
                        string sql = "select * from fileList where PkID='" + DrCurrentMaster[PkMaster.FieldName].ToString() + "' and sysFieldID =" + drfield["sysFieldID"].ToString();
                        DataTable tbfileList = dbStruct.GetDataTable(sql);
                        if (tbfileList.Rows.Count > 0)
                        {
                            _fileData.Add(new FileData(drfield, tbfileList.Rows[tbfileList.Rows.Count - 1]["fData"] as byte[], false));
                        }
                    }
                }
            }
        }
       
       
        public virtual void CheckRules(DataAction dataAction)
        {
            if (this._drCurrentMaster.RowState != DataRowState.Deleted)
            {
                foreach (DataRow drField in this._dsStruct.Tables[0].Rows)
                {
                    if (drField["Visible"].ToString() == "0")
                    {
                       // continue;
                    }
                   

                    string fieldName = drField["FieldName"].ToString();
                    int pType = int.Parse(drField["Type"].ToString());
                    switch (pType)
                    {
                        case 3:
                        case 6:
                            {
                                continue;
                            }
                    }
                    string fieldValue = this._drCurrentMaster[fieldName].ToString();

                    // Kiểm tra AllowNull theo điều kiện   

                    bool AllowNull = drField["AllowNull"].ToString() == "1";
                    if (drField["AllowNull"] != DBNull.Value && drField["AllowNull"].ToString() != "0" && drField["AllowNull"].ToString() != "1")
                    {
                        string conditionNull = PkMaster.FieldName + "=" + quote + DrCurrentMaster[this.PkMaster.FieldName] + quote + " and (" + drField["AllowNull"].ToString() + ")";
                        DataRow[] ldrMt = DrCurrentMaster.Table.Select(conditionNull);
                        AllowNull = (ldrMt.Length != 0);
                    }
                    if (!AllowNull && fieldValue == string.Empty)
                    {
                        this._drCurrentMaster.SetColumnError(fieldName, "Phải nhập");
                        //LogFile.AppendToFile("CheckErr.txt",, fieldName + "_" + "Phải nhập");
                    }
                    else
                    {
                        this._drCurrentMaster.SetColumnError(fieldName, string.Empty);
                    }

                    if (fieldValue != string.Empty)
                    {
                        if (bool.Parse(drField["IsUnique"].ToString()))
                        {
                            string tableName = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["TableName"].ToString() : this._drTable["TableName"].ToString();
                            string pk = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["Pk"].ToString() : this._drTable["Pk"].ToString();
                            string pkValue = this._drCurrentMaster[pk].ToString();
                            if (this.IsUnique(dataAction, fieldValue, fieldName, tableName, pk, pkValue, drField))
                            {
                                this._drCurrentMaster.SetColumnError(fieldName, string.Empty);
                            }
                            else
                            {
                                string editMask = drField["EditMask"].ToString();
                                if ((((pType == 0) || (pType == 2)) && (dataAction == DataAction.Insert)) && (editMask != string.Empty))
                                {
                                    if (drField.Table.Columns.Contains("AutoCreate"))
                                    {
                                        if (bool.Parse(drField["AutoCreate"].ToString()))
                                        {
                                            this._autoIncreValues.MakeNewStruct();
                                            this._drCurrentMaster[fieldName] = drField["DefaultValue"].ToString();
                                            //this._drCurrentMaster.SetColumnError(fieldName, "Lưu lại lần nữa");
                                            this.CheckRules(dataAction);
                                        }
                                        else
                                        {
                                            this._autoIncreValues.MakeNewStruct();
                                            this._drCurrentMaster.SetColumnError(fieldName, "Đ\x00e3 c\x00f3 số liệu tr\x00f9ng");
                                            //LogFile.AppendToFile("CheckErr.txt",, fieldName + "_" + "Trùng");
                                        }
                                    }
                                    else
                                    {
                                        this._drCurrentMaster.SetColumnError(fieldName, "Đ\x00e3 c\x00f3 số liệu tr\x00f9ng");
                                        //LogFile.AppendToFile("CheckErr.txt",, fieldName + "_" + "Trùng");
                                    }
                                }
                                else
                                {
                                    this._drCurrentMaster.SetColumnError(fieldName, "Đ\x00e3 c\x00f3 số liệu tr\x00f9ng");
                                    //LogFile.AppendToFile("CheckErr.txt",, fieldName + "_" + "Trùng");
                                }
                            }
                        }
                        decimal value = 0;
                        if (decimal.TryParse(this._drCurrentMaster[fieldName].ToString(), out value))
                        {
                            if (drField["MinValue"].ToString() != string.Empty)
                            {
                                //decimal minValue = decimal.Parse(drField["MinValue"].ToString());
                                decimal minValue = decimal.MinValue;
                                if (drField["minValue"].ToString().Contains("@"))
                                {
                                    minValue = decimal.Parse(this._drCurrentMaster[drField["MaxValue"].ToString().Remove(0, 1)].ToString());
                                }
                                else
                                {
                                    minValue = decimal.Parse(drField["minValue"].ToString());
                                }
                                if (minValue > value)
                                {
                                    this._drCurrentMaster.SetColumnError(fieldName, "Phải lớn hơn hoặc bằng " + minValue.ToString());
                                    //LogFile.AppendToFile("CheckErr.txt",, fieldName + "_" + "Nhỏ hơn Min");
                                    continue;
                                }
                                this._drCurrentMaster.SetColumnError(fieldName, string.Empty);
                            }
                            if (drField["MaxValue"].ToString() != string.Empty)
                            {
                                decimal maxValue=decimal.MaxValue;
                                if (drField["MaxValue"].ToString().Contains("@"))
                                {
                                    maxValue = decimal.Parse(this._drCurrentMaster[drField["MaxValue"].ToString().Remove(0, 1)].ToString());
                                }
                                else
                                {
                                    maxValue = decimal.Parse(drField["MaxValue"].ToString());
                                }
                                if (maxValue < value)
                                {
                                    this._drCurrentMaster.SetColumnError(fieldName, "Phải nhỏ hơn hoặc bằng " + maxValue.ToString());
                                    //LogFile.AppendToFile("CheckErr.txt",, fieldName + "_" + "Lớn hơn Max");
                                }
                                else
                                {
                                    this._drCurrentMaster.SetColumnError(fieldName, string.Empty);
                                }
                            }
                        }
                        //Check rule theo điều kiện
                    }

                }
                bool result = eventCheckRules?.Invoke(_drTable["TableName"].ToString()) ?? false;
            }
        }
        public void DoAfterUpdate()
        {
             this.JustUpdated?.Invoke();
        }
        public bool DoUpdating()
        {
            return eventDataUpdating?.Invoke() ?? true;
        }
        public void CloneData()
        {
            //try
            //{
            DataRow drMasterSource = this._drCurrentMaster;
            
            DataRow drMasterDes = this._dsData.Tables[0].NewRow();
            this._formulaCaculator.Active = false;

            
            //drMasterDes.ItemArray = (object []) _drCurrentMaster.ItemArray.Clone();
            string pkMaster = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["Pk"].ToString() : this._drTable["Pk"].ToString();
            Guid id = Guid.NewGuid();
            if (drMasterDes[pkMaster].GetType() == typeof(Guid))
            {
                drMasterDes[pkMaster] = id;
            }
            if (drMasterDes[pkMaster].GetType() == typeof(int))
            {
                drMasterDes[pkMaster] = 0x7fffffff;
            }
            if (drMasterDes[pkMaster].GetType() == typeof(string))
            {
                drMasterDes[pkMaster] = "";
            }
            
            foreach (DataRow drSt in _dsStruct.Tables[0].Rows)
            {
                if (pkMaster.ToLower() == drSt["FieldName"].ToString().ToLower()) continue;
                if (drSt.Table.Columns.Contains("AllowCopy") && drSt["AllowCopy"] != DBNull.Value && bool.Parse(drSt["AllowCopy"].ToString()))
                {
                    drMasterDes[drSt["FieldName"].ToString()] = drMasterSource[drSt["FieldName"].ToString()];
                }
            }
            this._dsData.Tables[0].Rows.Add(drMasterDes);



            this.DrCurrentMaster = drMasterDes;
            if (DrCurrentMaster.Table.Columns.Contains("Approved")) DrCurrentMaster["Approved"] = 0;
            if (DrCurrentMaster.Table.Columns.Contains("TaskID"))
            {
                DrCurrentMaster["TaskID"] = DBNull.Value;
            }


            if (this._dataType != DataType.MasterDetail)
            {
                this._formulaCaculator.Active = true;
            }
            else
            {
                string quoteDt0 = "";
                if (DrTableMaster["Pk"].GetType() == typeof(string) || DrTableMaster["Pk"].GetType() == typeof(Guid)) quoteDt0 = "'";

                DataRow[] arrDrCurrentDetails = DsData.Tables[1].Select(DrTableMaster["Pk"].ToString() + "="+ quoteDt0+ drMasterSource[DrTableMaster["Pk"].ToString()].ToString() + quoteDt0);
                this._lstDrCurrentDetails.CopyTo(arrDrCurrentDetails);
                this._lstDrCurrentDetails.Clear();
                for (int i = 0; i < arrDrCurrentDetails.Length; i++)
                {
                    DataRow drDetailSource = arrDrCurrentDetails[i];
                    DataRow drDetailDes = this._dsData.Tables[1].NewRow();
                    this._formulaCaculator.Active = false;
                    foreach (DataRow drSt in _dsStruct.Tables[1].Rows)
                    {
                        if (drSt.Table.Columns.Contains("AllowCopy") && bool.Parse(drSt["AllowCopy"].ToString()))
                        {
                            drDetailDes[drSt["FieldName"].ToString()] = drDetailSource[drSt["FieldName"].ToString()];
                        }
                    }
                    //drDetailDes.ItemArray = (object[])drDetailSource.ItemArray.Clone();
                    string pkDetail = this._drTable["Pk"].ToString();
                    if (drDetailDes[pkDetail].GetType() == typeof(Guid))
                    {
                        drDetailDes[pkDetail] = Guid.NewGuid();
                    }
                    else
                    {
                        drDetailDes[pkDetail] = DBNull.Value;
                    }
                    if (drMasterDes[pkMaster].GetType() == typeof(Guid))
                    {
                        drDetailDes[pkMaster] = id;
                    }
                    else if (drMasterDes[pkMaster].GetType() == typeof(int))
                    {
                        drDetailDes[pkMaster] = 0x7fffffff;
                    }
                    else
                    {
                        drDetailDes[pkMaster] = DBNull.Value;
                    }
                    if (drDetailDes.RowState == DataRowState.Detached)
                    {
                        this._dsData.Tables[1].Rows.Add(drDetailDes);
                    }
                    for (int j = 0; j < this._drTableDt.Count; j++)
                    {
                        DataRow _drTableDt = this._drTableDt[j];
                        DataRow _drDetail = this._dtDetail.Rows[j];
                        if (!bool.Parse(_drDetail["ChildOf"].ToString()))
                        {
                            string quoteDt = "";
                            if (drDetailDes[pkDetail].GetType() == typeof(string) || drDetailDes[pkDetail].GetType() == typeof(Guid)) quoteDt = "'";
                            DataRow[] _lstRowDt = DsData.Tables[_drTableDt["TableName"].ToString()].Select("DTID=" + quoteDt + drDetailSource[pkDetail] + quoteDt);
                            foreach (DataRow drDtSource in _lstRowDt)
                            {
                                DataRow drDtDes = DsData.Tables[_drTableDt["TableName"].ToString()].NewRow();
                                foreach (DataRow drSt in _dsStructDt.Tables[j].Rows)
                                {
                                    if (drSt.Table.Columns.Contains("AllowCopy") && bool.Parse(drSt["AllowCopy"].ToString()))
                                    {
                                        if (drSt["FieldName"].ToString() == "Lien")
                                        {

                                        }
                                        drDtDes[drSt["FieldName"].ToString()] = drDtSource[drSt["FieldName"].ToString()];
                                    }
                                }
                                //drDtDes.ItemArray = (object[])drDtSource.ItemArray.Clone();
                                string pkDt = _drTableDt["Pk"].ToString();
                                //Cấp ID cho Khóa chính CT
                                if (drDtDes[pkDt].GetType() == typeof(Guid))
                                {
                                    drDtDes[pkDt] = Guid.NewGuid();
                                }
                                else
                                {
                                    drDtDes[pkDt] = DBNull.Value;
                                }
                                //Cấp ID cho khóa với Master
                                if (drDtDes["MTID"].GetType() == typeof(Guid))
                                {
                                    drDtDes["MTID"] = id;
                                }
                                else if (drDtDes["MTID"].GetType() == typeof(int))
                                {
                                    drDtDes["MTID"] = 0x7fffffff;
                                }
                                else
                                {
                                    drDtDes["MTID"] = DBNull.Value;
                                }
                                //Cấp ID cho khóa với Detail
                                if (drDtDes["DTID"].GetType() == typeof(Guid))
                                {
                                    drDtDes["DTID"] = drDetailDes[pkDetail];
                                }
                                else if (drDtDes["DTID"].GetType() == typeof(int))
                                {
                                    drDtDes["DTID"] = drDetailDes[pkDetail];
                                }
                                else
                                {
                                    drDtDes["DTID"] = DBNull.Value;
                                }
                                if (drDtDes.RowState == DataRowState.Detached)
                                {
                                    DsData.Tables[_drTableDt["TableName"].ToString()].Rows.Add(drDtDes);
                                }
                            }
                        }


                    }


                }
                for (int j = 0; j < this._drTableDt.Count; j++)
                {
                    DataRow _drTableDt = this._drTableDt[j];
                    DataRow _drDetail = this._dtDetail.Rows[j];
                    if (bool.Parse(_drDetail["ChildOf"].ToString()))
                    {
                        DataRow[] _lstRowDt = DsData.Tables[_drTableDt["TableName"].ToString()].Select("MTID=" + this.quote + drMasterSource[pkMaster] + this.quote);
                        foreach (DataRow drDtSource in _lstRowDt)
                        {
                            DataRow drDtDes = DsData.Tables[_drTableDt["TableName"].ToString()].NewRow();
                            foreach (DataRow drSt in _dsStructDt.Tables[j].Rows)
                            {
                                if (drSt.Table.Columns.Contains("AllowCopy") && bool.Parse(drSt["AllowCopy"].ToString()))
                                {
                                    drDtDes[drSt["FieldName"].ToString()] = drDtSource[drSt["FieldName"].ToString()];
                                }
                            }
                            // drDtDes.ItemArray = (object[])drDtSource.ItemArray.Clone();
                            string pkDt = _drTableDt["Pk"].ToString();
                            //Cấp ID cho Khóa chính CT
                            if (drDtDes[pkDt].GetType() == typeof(Guid))
                            {
                                drDtDes[pkDt] = Guid.NewGuid();
                            }
                            else
                            {
                                drDtDes[pkDt] = DBNull.Value;
                            }
                            //Cấp ID cho khóa với Master
                            if (drDtDes["MTID"].GetType() == typeof(Guid))
                            {
                                drDtDes["MTID"] = id;
                            }
                            else if (drDtDes["MTID"].GetType() == typeof(int))
                            {
                                drDtDes["MTID"] = 0x7fffffff;
                            }
                            else
                            {
                                drDtDes["MTID"] = DBNull.Value;
                            }
                            if (drDtDes.RowState == DataRowState.Detached)
                            {
                                DsData.Tables[_drTableDt["TableName"].ToString()].Rows.Add(drDtDes);
                            }
                        }
                    }
                }
                this.LstDrCurrentDetails = this._lstDrCurrentDetails;
                this._formulaCaculator.Active = true;
            }
            //}
            //catch(Exception ex)
            //{
            //    if (this._formulaCaculator != null)
            //    {
            //        this._formulaCaculator.Active = true;
            //    }
            //}
        }



        public DataRow Clone1Row(DataRow dr)
        {
            try
            {
                DataRow drDetailDes=null;
                string pkMaster = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["Pk"].ToString() : this._drTable["Pk"].ToString();
                DataRow drDetailSource = dr;
                
                this._formulaCaculator.Active = false;
                if (dr.Table.TableName == this.DrTable["TableName"].ToString())
                {
                     drDetailDes = dr.Table.NewRow();
                    foreach (DataRow drSt in _dsStruct.Tables[1].Rows)
                    {
                        if (drSt.Table.Columns.Contains("AllowCopy") && bool.Parse(drSt["AllowCopy"].ToString()))
                        {
                            if (drDetailSource[drSt["FieldName"].ToString()] != DBNull.Value)
                                drDetailDes[drSt["FieldName"].ToString()] = drDetailSource[drSt["FieldName"].ToString()];
                        }
                    }
                    //drDetailDes.ItemArray = (object[])drDetailSource.ItemArray.Clone();
                    string pkDetail = this._drTable["Pk"].ToString();
                    if (drDetailDes[pkDetail].GetType() == typeof(Guid))
                    {
                        drDetailDes[pkDetail] = Guid.NewGuid();
                    }
                    else
                    {
                        drDetailDes[pkDetail] = DBNull.Value;
                    }
                    if (this._drCurrentMaster[pkMaster].GetType() == typeof(Guid))
                    {
                        drDetailDes[pkMaster] = this._drCurrentMaster[pkMaster];
                    }
                    else if (this._drTableMaster[pkMaster].GetType() == typeof(int))
                    {
                        drDetailDes[pkMaster] = 0x7fffffff;
                    }
                    else
                    {
                        drDetailDes[pkMaster] = DBNull.Value;
                    }
                    if (drDetailDes.RowState == DataRowState.Detached)
                    {
                        this._dsData.Tables[1].Rows.Add(drDetailDes);
                    }

                    for (int j = 0; j < this._drTableDt.Count; j++)
                    {
                        DataRow _drTableDt = this._drTableDt[j];
                        DataRow _drDetail = this._dtDetail.Rows[j];
                        if (!bool.Parse(_drDetail["ChildOf"].ToString()))
                        {
                            string quoteDt = "";
                            if (drDetailDes[pkDetail].GetType() == typeof(string) || drDetailDes[pkDetail].GetType() == typeof(Guid)) quoteDt = "'";
                            DataRow[] _lstRowDt = DsData.Tables[_drTableDt["TableName"].ToString()].Select("DTID=" + quoteDt + drDetailSource[pkDetail] + quoteDt);
                            foreach (DataRow drDtSource in _lstRowDt)
                            {
                                DataRow drDtDes = DsData.Tables[_drTableDt["TableName"].ToString()].NewRow();
                                foreach (DataRow drSt in _dsStructDt.Tables[j].Rows)
                                {
                                    if (drSt.Table.Columns.Contains("AllowCopy") && bool.Parse(drSt["AllowCopy"].ToString()))
                                    {
                                        drDtDes[drSt["FieldName"].ToString()] = drDtSource[drSt["FieldName"].ToString()];
                                    }
                                }
                                //drDtDes.ItemArray = (object[])drDtSource.ItemArray.Clone();
                                string pkDt = _drTableDt["Pk"].ToString();
                                //Cấp ID cho Khóa chính CT
                                if (drDtDes[pkDt].GetType() == typeof(Guid))
                                {
                                    drDtDes[pkDt] = Guid.NewGuid();
                                }
                                else
                                {
                                    drDtDes[pkDt] = DBNull.Value;
                                }
                                //Cấp ID cho khóa với Master
                                if (drDtDes["MTID"].GetType() == typeof(Guid))
                                {
                                    drDtDes["MTID"] = this._drCurrentMaster[pkMaster];
                                }
                                else if (drDtDes["MTID"].GetType() == typeof(int))
                                {
                                    drDtDes["MTID"] = 0x7fffffff;
                                }
                                else
                                {
                                    drDtDes["MTID"] = DBNull.Value;
                                }
                                //Cấp ID cho khóa với Detail
                                if (drDtDes["DTID"].GetType() == typeof(Guid))
                                {
                                    drDtDes["DTID"] = drDetailDes[pkDetail];
                                }
                                else if (drDtDes["DTID"].GetType() == typeof(int))
                                {
                                    drDtDes["DTID"] = drDetailDes[pkDetail];
                                }
                                else
                                {
                                    drDtDes["DTID"] = DBNull.Value;
                                }
                                if (drDtDes.RowState == DataRowState.Detached)
                                {
                                    DsData.Tables[_drTableDt["TableName"].ToString()].Rows.Add(drDtDes);
                                }
                            }
                        }
                    }
                }
                for (int j = 0; j < this._drTableDt.Count; j++)
                {
                    DataRow _drTableDt = this._drTableDt[j];
                    DataRow _drDetail = this._dtDetail.Rows[j];
                    if (_drTableDt["TableName"].ToString() == dr.Table.TableName)
                    {
                        if (bool.Parse(_drDetail["ChildOf"].ToString()))
                        {
                            DataRow[] _lstRowDt = DsData.Tables[_drTableDt["TableName"].ToString()].Select("MTID=" + this.quote + _drCurrentMaster[pkMaster] + this.quote);
                            //foreach (DataRow drDtSource in _lstRowDt)
                            //{
                                DataRow drDtDes = DsData.Tables[_drTableDt["TableName"].ToString()].NewRow();
                                foreach (DataRow drSt in _dsStructDt.Tables[j].Rows)
                                {
                                    if (drSt.Table.Columns.Contains("AllowCopy") && bool.Parse(drSt["AllowCopy"].ToString()))
                                    {
                                        drDtDes[drSt["FieldName"].ToString()] = drDetailSource[drSt["FieldName"].ToString()];
                                    }
                                }
                                // drDtDes.ItemArray = (object[])drDtSource.ItemArray.Clone();
                                string pkDt = _drTableDt["Pk"].ToString();
                                //Cấp ID cho Khóa chính CT
                                if (drDtDes[pkDt].GetType() == typeof(Guid))
                                {
                                    drDtDes[pkDt] = Guid.NewGuid();
                                }
                                else
                                {
                                    drDtDes[pkDt] = DBNull.Value;
                                }
                                //Cấp ID cho khóa với Master
                                if (drDtDes["MTID"].GetType() == typeof(Guid))
                                {
                                    drDtDes["MTID"] = this._drCurrentMaster[pkMaster];
                                }
                                else if (drDtDes["MTID"].GetType() == typeof(int))
                                {
                                    drDtDes["MTID"] = 0x7fffffff;
                                }
                                else
                                {
                                    drDtDes["MTID"] = DBNull.Value;
                                }
                                if (drDtDes.RowState == DataRowState.Detached)
                                {
                                    DsData.Tables[_drTableDt["TableName"].ToString()].Rows.Add(drDtDes);
                                }
                            drDetailDes = drDtDes;
                            //}
                        }
                    }

                }
                this.LstDrCurrentDetails = this._lstDrCurrentDetails;
                this._formulaCaculator.Active = true;
                
                return drDetailDes;
            }
            catch (Exception ex) {
                return null;
            }

        }

        public void CDTData_eventClonedtRow(DataRow drDes)
        {
            this.eventClonedtRow?.Invoke(drDes);
        }

        protected void ConditionForPackage()
        {
            object o = Config.GetValue("curPackageID");
            if ((o != null) && (o.ToString().Trim() != string.Empty))
            {
                string and;
                string curPackageID = o.ToString();
                if (this._dataType == DataType.MasterDetail)
                {
                    and = (this._conditionMaster == string.Empty) ? string.Empty : " and ";
                    if (this._drTableMaster["TableName"].ToString().ToUpper() == "SYSTABLE")
                    {
                        this._conditionMaster = this._conditionMaster + and + "sysPackageID = " + curPackageID;
                    }
                }
                else
                {
                    and = (this._condition == string.Empty) ? string.Empty : " and ";
                    string tableName = this._drTable["TableName"].ToString().ToUpper();
                    switch (tableName)
                    {
                        case "SYSTABLE":
                        case "SYSMENU":
                        case "SYSREPORT":
                            this._condition = this._condition + and + "sysPackageID = " + curPackageID;
                            break;
                    }
                    if (tableName == "SYSFIELD")
                    {
                        _conditionMaster += and + "sysPackageID = " + curPackageID;
                    }
                    if (this._dataType == DataType.Detail)
                    {
                        and = (this._conditionMaster == string.Empty) ? string.Empty : " and ";
                        tableName = this._drTableMaster["TableName"].ToString().ToUpper();
                        switch (tableName)
                        {
                            case "SYSTABLE":
                            case "SYSMENU":
                            case "SYSREPORT":
                                this._conditionMaster = this._conditionMaster + and + "sysPackageID = " + curPackageID;
                                break;
                        }
                        if (tableName == "SYSDATACONFIG")
                        {

                            this._condition += and + "sysTableID in (select sysTableID from sysTable where sysPackageID = " + curPackageID + ")";
                        }
                    }
                }
            }
        }

        private void DataTable_i_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            this.DataChanged = true;
            
            //if (this._tableName == "DT35" && e.Row.Table.TableName == "CT35" && e.Row.RowState!=DataRowState.Unchanged)
            //{
            //    bool isDelete = false;
            //    if (e.Row.RowState == DataRowState.Deleted || e.Row.RowState == DataRowState.Detached)
            //    {
            //        e.Row.RejectChanges();
            //        isDelete = true;
            //    }
            //    string sql = "select count(*) from MT3A a inner join DT3A b on a.MT3AID=b.MT3AID where b.CT35ID='" + e.Row["CT35ID"].ToString() + "' and a.Approved>=0";
            //    object o = DbData.GetValue(sql);
            //    if (o != null && int.Parse(o.ToString()) > 0)
            //    {

            //        MessageBox.Show("Dòng này đã tạo phiếu xuất");
            //    }

            //}
        }
        public bool saveHistory = true;
        public bool flagRowdeleteHand = false;
        public bool flagRowdeleteHandTotal = false;
        private void DataTable_i_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (flagRowdeleteHand || flagRowdeleteHandTotal)
            {
                flagRowdeleteHand = false;

            }
            else
            {
                if (e.Row.RowState == DataRowState.Deleted)
                {
                    e.Row.RejectChanges();
                    flagRowdeleteHand = true;
                }
                string TableName = e.Row.Table.TableName;
                List<DataRow> _Tbdt = this._drTableDt.Where<DataRow>(dr => dr["TableName"].ToString() == TableName).ToList();
                if (_Tbdt.Count > 0)
                {
                    string sysDetailID = _Tbdt[0]["sysTableID"].ToString();
                    DataRow[] DetailRow = _dtDetail.Select("sysDetailID=" + sysDetailID);
                    if (DetailRow.Length > 0) TableName = DetailRow[0]["DetailName"].ToString();
                }
                List<CurrentRowDt> lsDRdt = _lstCurRowDetail.Where(DRdt => DRdt.TableName == (sender as DataTable).TableName).ToList();
                List<DataRow> CurDT = new List<DataRow>();
                foreach (CurrentRowDt DRdt in lsDRdt)
                {
                    CurDT.Add(DRdt.RowDetail);
                }
                if (saveHistory)
                {
                    int currentIndex = CurDT.IndexOf(e.Row);
                    int deletedRowCount = CurDT.Take(currentIndex).Count(row => row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached);
                    string tip = "Trước khi xóa dòng số " + (currentIndex - deletedRowCount + 1).ToString() + " trên bảng " + TableName;
                    if (HistoryCurrents.Count > 0)
                    {
                        DateTime lastSavehis = HistoryCurrents.Last().datetime;
                        if ((DateTime.Now - lastSavehis).TotalMilliseconds >= 3000)
                        {

                            HistoryCurrents.Add(new historyCurrent(DateTime.Now, tip, this.GenSQLDataLog()));
                            if (HistoryCurrents.Count > 20) HistoryCurrents.RemoveAt(0);
                            if (this.AddHis != null)
                                this.AddHis.Invoke();
                        }
                    }
                    else
                    {
                        HistoryCurrents.Add(new historyCurrent(DateTime.Now, tip, this.GenSQLDataLog()));
                        if (this.AddHis != null)
                            this.AddHis.Invoke();
                    }
                }
                e.Row.Delete();
                flagRowdeleteHand = false;
            }
            this.DataChanged = true;
            _formulaCaculator.Active = false;
            if (this._tableName == "DT35" && e.Row.Table.TableName == "CT35" && e.Row.RowError != "Deleted")
            {
                //e.Row.RejectChanges();
                //string sql = "select count(*) from MT3A a inner join DT3A b on a.MT3AID=b.MT3AID where b.CT35ID='" + e.Row["CT35ID"].ToString() + "' and a.Approved>=0";
                //object o = DbData.GetValue(sql);
                //if (o != null && int.Parse(o.ToString()) > 0)
                //{
                //    MessageBox.Show("Dòng này đã tạo phiếu xuất");
                //}
                //else
                //{
                //    e.Row.RowError = "Deleted";
                //    e.Row.Delete();
                //}
            }
            _formulaCaculator.Active = true;

        }

        private void DataTable_i_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            string TableName = e.Row.Table.TableName;
            List<DataRow> _Tbdt = this._drTableDt.Where<DataRow>( dr => dr["TableName"].ToString() ==TableName).ToList();
            if (_Tbdt.Count > 0)
            {
                string sysDetailID = _Tbdt[0]["sysTableID"].ToString();
                DataRow[] DetailRow = _dtDetail.Select("sysDetailID=" + sysDetailID);
                if (DetailRow.Length > 0) TableName = DetailRow[0]["DetailName"].ToString();
            }
            //Lấy thứ tự dòng trong grid
            List<CurrentRowDt> lsDRdt = _lstCurRowDetail.Where(DRdt => DRdt.TableName == (sender as DataTable).TableName).ToList();
            List<DataRow> CurDT = new List<DataRow>();
            foreach(CurrentRowDt DRdt in lsDRdt)
            {
                CurDT.Add(DRdt.RowDetail);
            }
            if (saveHistory)
            {
                int currentIndex = lsDRdt.Count;
                int deletedRowCount = CurDT.Take(currentIndex).Count(row => row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached);
                //
                string tip = "Trước khi thêm dòng số " + (currentIndex - deletedRowCount + 1).ToString() + " trên bảng " + TableName;
                if (HistoryCurrents.Count > 0)
                {
                    DateTime lastSavehis = HistoryCurrents.Last().datetime;
                    if ((DateTime.Now - lastSavehis).TotalMilliseconds >= 3000)
                    {
                        HistoryCurrents.Add(new historyCurrent(DateTime.Now, tip, this.GenSQLDataLog()));
                        if (HistoryCurrents.Count > 20) HistoryCurrents.RemoveAt(0);
                        if (this.AddHis != null)
                            this.AddHis.Invoke();
                    }
                }
                else
                {
                    HistoryCurrents.Add(new historyCurrent(DateTime.Now, tip, this.GenSQLDataLog()));
                    if (this.AddHis != null)
                        this.AddHis.Invoke();
                }
            }
            this.DataChanged = true;
            CurrentRowDt CRdt = new CurrentRowDt();
            CRdt.TableName = (sender as DataTable).TableName;
            CRdt.RowDetail = e.Row;


            this.SetDefaultValues(this._dsStructDt.Tables[(sender as DataTable).TableName], e.Row);

            this._lstCurRowDetail.Add(CRdt);

        }

        public virtual void DataTable0_ColChanged(object sender, DataColumnChangeEventArgs e)
        {
           // LogFile.AppendToFile("Datachange", "change on CDTData" + e.Column.ColumnName);
            if (e.Column.ColumnName == "MaHTTT")
            {

            }
            this.DataChanged = true;
            //try
            //{
            //    e.Row.EndEdit();
            //}
            //catch (Exception ex) { }
            //Đoạn code này thay thế GridLookupEdit, RiGridLookupEdit value changed hoặc validating nhằm set value from list
            string value = null;
            if (e.Column.DataType == typeof(bool))
            {
                value = e.Row[e.Column].ToString() == "True" ? "1" : "0";
            }
            else
            {
                value = e.Row[e.Column].ToString();
            }
            foreach (DataRow drField in this._dsStruct.Tables[0].Rows)
            {
                if (e.Row[e.Column] == DBNull.Value) break;


                DataRow[] RPk = null;
                if (drField["refTable"] != DBNull.Value && drField["refTable"].ToString() != string.Empty)
                {
                    if (e.Column.ColumnName != drField["fieldName"].ToString()) continue;
                    string tableName = drField["refTable"].ToString();

                    //value = e.Row[e.Column].ToString();
                    string con = drField["refCriteria"].ToString();
                    string dyncondition = drField["DynCriteria"].ToString();
                    CDTData dataRef = publicCDTData.findCDTData(tableName, con, dyncondition);

                    if (dataRef == null)
                    {
                        DataSingle _dbdm = new DataSingle(tableName, "7");
                        _dbdm.Condition = con;
                        _dbdm.DynCondition = dyncondition;
                        _dbdm.GetData();
                        publicCDTData.AddCDTData(_dbdm);
                    }
                    dataRef = publicCDTData.findCDTData(tableName, con, dyncondition);
                    if (dataRef == null) break;
                    if (!dataRef.FullData & dataRef.dataType == DataType.Single)
                        dataRef.GetData();
                    string RefPk = null; DataTable TableRef = null;

                    string quote1 = "";
                    if (e.Column.DataType == typeof(string) || e.Column.DataType == typeof(Guid)) quote1 = "'";
                    if (dataRef.dataType == DataType.Single || (dataRef.dataType == DataType.MasterDetail && dataRef.DrTableMaster["tableName"].ToString() == tableName))
                    {
                        RefPk = drField["reffield"].ToString();// dataRef.DrTable["pk"].ToString();
                        TableRef = dataRef.DsData.Tables[0];
                        //if (RefPk == e.Column.ColumnName)
                        RPk = TableRef.Select(RefPk + "=" + quote1 + value + quote1);
                    }
                    else if (dataRef.dataType == DataType.Detail || (dataRef.dataType == DataType.MasterDetail && dataRef.DrTable["tableName"].ToString() == tableName))
                    {
                        RefPk = drField["reffield"].ToString();//  dataRef.DrTableMaster["pk"].ToString();
                        TableRef = dataRef.DsData.Tables[1];
                        //if(RefPk == e.Column.ColumnName)
                        RPk = TableRef.Select(RefPk + "=" + quote1 + value + quote1);
                    }
                    if (RPk == null || RPk.Length == 0) break;
                    if (this.dataType == DataType.Detail) this.DrCurrentMaster = e.Row;
                    SetValuesFromList(e.Column.ColumnName, value, RPk[0], false);

                    //Cần thay đổi để tìm đúng dòng lọc trường hợp 2 ổ cứng
                }


            }
            //SetDefaulValueForDetail(e.Column.ColumnName, value);
            if (e.Row.RowState != DataRowState.Added) return;
            if (e.Column.ColumnName.ToLower() == "ngayct")
            {
                //Set lại số chứng từ nếu định dạng nó chứa "MM"

                foreach (DataRow drField in this._dsStruct.Tables[0].Rows)
                {
                    string editMask = drField["EditMask"].ToString();
                    int pType = int.Parse(drField["Type"].ToString());
                    if (!editMask.Contains("MM") && !editMask.Contains("YY")) continue;
                    if ((pType != 0) && (pType != 2)) continue;
                    string fieldName = drField["FieldName"].ToString();

                    if (bool.Parse(drField["IsUnique"].ToString()))
                    {
                        string tableName = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["TableName"].ToString() : this._drTable["TableName"].ToString();
                        string pk = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["Pk"].ToString() : this._drTable["Pk"].ToString();
                        string pkValue = this._drCurrentMaster[pk].ToString();



                        if (drField.Table.Columns.Contains("AutoCreate"))
                        {
                            //if (bool.Parse(drField["AutoCreate"].ToString()))
                            //{
                            this._autoIncreValues.MakeNewStruct();
                            e.Row[fieldName] = drField["DefaultValue"].ToString();
                            e.Row.EndEdit();
                            //this._drCurrentMaster.SetColumnError(fieldName, "Lưu lại lần nữa");
                            //}
                        }
                    }

                }
            }
            //Không nhảy value from list cho những trường hợp giá trị mặc định hoặc khởi tạo từ bảng chính

        }

        private void SetDefaulValueForDetail(string columnName, string value)
        {
            string formulaDetail;
            string[] str;
            List<object> ob;
            List<string> lstStr = new List<string>();
            if (this._drCurrentMaster != null)
            {


                if (this._dataType == DataType.MasterDetail)
                {
                    foreach (DataRow drField in this._dsStruct.Tables[1].Rows)
                    {
                        formulaDetail = drField["FormulaDetail"].ToString();
                        if (formulaDetail == columnName)
                        {

                            drField["DefaultValue"] = value;


                            //Trường hợp cho thay đổi detail theo trên



                        }
                    }
                }
            }
            if (this._dtDetail != null)
            {
                for (int i = 0; i < this._dtDetail.Rows.Count; i++)
                {
                    foreach (DataRow drField in this._dsStructDt.Tables[i].Rows)
                    {
                        formulaDetail = drField["FormulaDetail"].ToString();
                        if (formulaDetail.ToUpper() == columnName.ToUpper())
                        {
                            drField["DefaultValue"] = value;
                        }
                    }
                }
            }
        }

        private void DataTable0_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            this.DataChanged = true;
        }

        private void DataTable0_RowDeleted(object sender, DataRowChangeEventArgs e)
        {

            this._drCurrentMaster = e.Row;
            this.DataChanged = true;
        }

        private void DataTable0_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            
            if (this._dataType != DataType.Report)
            {
                string sysTableID = this._drTable["SysTableID"].ToString();
                this.LstDrCurrentDetails = new List<DataRow>();                
                this._lstCurRowDetail = new List<CurrentRowDt>();
                this._formulaCaculator.LstCurrentRowDt = this._lstCurRowDetail;
                //if (this._autoIncreValues == null)
                //{
                this._autoIncreValues = new AutoIncrementValues(sysTableID, this._dsStruct.Tables[0], e.Row);
                //}

                this._autoIncreValues._dbStruct = this._dbStruct;
                this._autoIncreValues.MakeNewStruct();
                if (((this._drTable["Type"].ToString() == "1") || (this._drTable["Type"].ToString() == "3") || (this._drTable["Type"].ToString() == "4")) || (this._drTable["Type"].ToString() == "5"))
                {
                    this.DrCurrentMaster = e.Row;
                }
                Guid id = Guid.NewGuid();
                if (PkMaster.DbType == SqlDbType.UniqueIdentifier)
                {
                    e.Row[PkMaster.FieldName] = id;
                }
                if (PkMaster.DbType == SqlDbType.Int)
                {
                    //e.Row[PkMaster.FieldName] = null;
                }
                if (PkMaster.DbType == SqlDbType.NVarChar || PkMaster.DbType == SqlDbType.VarChar)
                {
                    e.Row[PkMaster.FieldName] = "";
                }
            }
            this.SetDefaultValues(this._dsStruct.Tables[0], e.Row);
            if (this._dataType == DataType.MasterDetail)
            {
                foreach (DataRow dr in tbTask.Rows)
                    if (dr["isBegin"].ToString() == "True")
                    {
                        e.Row["TaskID"] = dr["ID"].ToString();
                        e.Row["Approved"] = int.Parse(dr["ApprovedStt"].ToString());
                        break;
                    }
            }
            this.DataChanged = true;
        }
        private string GetDynFilterString(string Dyncondition, DataRow drMaster, DataRow drDetail)
        {
            string returnValue = "";
            if (Dyncondition == null || Dyncondition == string.Empty) return returnValue;
            var variables = new Dictionary<string, string>();
            List<string> fvar = Config.GetVariableList(Dyncondition.ToUpper());
            if (drMaster != null)
            {

                foreach (DataColumn dcMater in DrCurrentMaster.Table.Columns)
                {
                    string fieldName = dcMater.ColumnName;
                    if (!fvar.Contains("@" + fieldName.ToUpper())) continue;
                    string valueFilter = DrCurrentMaster[fieldName].ToString();

                    //if (!filter.ToUpper().Contains("@" + fieldName.ToUpper())) continue;
                    if (dcMater.DataType == typeof(bool))
                    {
                        if (valueFilter == "'True'") valueFilter = "1";
                        else if (valueFilter == "'False'") valueFilter = "0";
                        else valueFilter = "null ";
                    }

                    if (valueFilter == null || valueFilter == string.Empty)
                    {
                        if ((dcMater.DataType == typeof(decimal) || dcMater.DataType == typeof(int)))
                            valueFilter = "0";
                        else if (dcMater.DataType == typeof(string))
                            valueFilter = "''";
                        else if (dcMater.DataType == typeof(Guid))
                            valueFilter = " NULL ";
                    }
                    else
                    {
                        if (!(dcMater.DataType == typeof(decimal) || dcMater.DataType == typeof(int)))
                            if (!valueFilter.StartsWith("'"))
                                valueFilter = "'" + valueFilter + "'";
                    }
                    // filter = filter.ToUpper().Replace("@" + fieldName.ToUpper(), value);
                    variables.Add("@" + fieldName.ToUpper(), valueFilter);
                }
            }
            if (drDetail != null)
            {
                foreach (DataColumn dcDetail in drDetail.Table.Columns)
                {
                    string fieldName = dcDetail.ColumnName;
                    if (!fvar.Contains("@" + fieldName.ToUpper())) continue;
                    string valueFilter = drDetail[fieldName].ToString();
                    if (valueFilter == null || valueFilter == string.Empty) valueFilter = "";
                    if (valueFilter == null || valueFilter == string.Empty)
                    {
                        if ((dcDetail.DataType == typeof(decimal) || dcDetail.DataType == typeof(int)))
                            valueFilter = "0";
                        else if (dcDetail.DataType == typeof(string))
                            valueFilter = "''";
                        else if (dcDetail.DataType == typeof(Guid))
                            valueFilter = " NULL ";
                    }
                    else
                    {
                        if (!(dcDetail.DataType == typeof(decimal) || dcDetail.DataType == typeof(int)))
                            if (!valueFilter.StartsWith("('"))//Biểu thị cho kiểu list
                                valueFilter = "'" + valueFilter + "'";
                    }
                    //filter = filter.ToUpper().Replace("@" + fieldName.ToUpper(), value);
                    if (variables.ContainsKey("@" + fieldName.ToUpper()))
                    {
                        if (variables[("@" + fieldName.ToUpper())].ToString() == " NULL " || variables[("@" + fieldName.ToUpper())].ToString() == "''")
                        {
                            variables["@" + fieldName.ToUpper()] = valueFilter;
                        }
                        else continue;
                    }
                    else
                        variables.Add("@" + fieldName.ToUpper(), valueFilter);
                }
            }
            var sortedVariables = variables.OrderByDescending(v => v.Key.Length);

            foreach (var variable in sortedVariables)
            {
                string pattern = $@"\{variable.Key}\b"; // Sử dụng \b để đảm bảo khớp với từ nguyên vẹn
                Dyncondition = Regex.Replace(Dyncondition.ToUpper(), pattern, variable.Value);
            }
            Dyncondition = Dyncondition.Replace("= NULL", " IS NULL");
            return Dyncondition;
        }
        public virtual void DataTable1_ColChanged(object sender, DataColumnChangeEventArgs e)        
        {
            //Trường hợp sửa vào trực tiếp Dt mafko sửa trên MT,phải setdefault vaflue 
            this.DataChanged = true;
            try
            {
                e.Row.EndEdit();
            }
            catch (Exception ex) { }
            foreach (DataRow drField in this._dsStruct.Tables[1].Rows)
            {
                if (e.Row[e.Column] == DBNull.Value) break;
                if (drField["refTable"] != DBNull.Value && drField["refTable"].ToString() != string.Empty)
                {
                    if (e.Column.ColumnName != drField["fieldName"].ToString()) continue;
                    string tableName = drField["refTable"].ToString();
                    string value = e.Row[e.Column].ToString();
                    string con = drField["refCriteria"].ToString();
                    string Dyncondition = drField["DynCriteria"].ToString();
                    CDTData dataRef = publicCDTData.findCDTData(tableName, con, Dyncondition);

                    if (dataRef == null) break;
                    string RefPk = null; DataTable TableRef = null; DataRow[] RPk = null;
                    string quote1 = "";
                    if (!dataRef.FullData & dataRef.dataType == DataType.Single)
                    {
                        dataRef.GetData();
                    }
                    if (e.Column.DataType == typeof(string) || e.Column.DataType == typeof(Guid)) quote1 = "'";
                    string filterCondition = GetDynFilterString(Dyncondition, DrCurrentMaster, e.Row);

                    if (dataRef.dataType == DataType.Single)
                    {
                        RefPk = drField["RefField"].ToString(); //dataRef.DrTable["pk"].ToString();
                        TableRef = dataRef.DsData.Tables[0];
                        if (e.Column.DataType == dataRef.DsData.Tables[0].Columns[RefPk].DataType)
                        {
                            filterCondition = "(" + RefPk + "=" + quote1 + value + quote1 + ")" + (filterCondition == "" ? "" : " and (" + filterCondition + ")");
                            RPk = TableRef.Select(filterCondition);
                        }
                    }
                    else if (dataRef.dataType == DataType.MasterDetail && dataRef.DrTableMaster["tableName"].ToString() == tableName)
                    {
                        RefPk = drField["RefField"].ToString(); //dataRef.DrTable["pk"].ToString();
                        TableRef = dataRef.DsData.Tables[0];
                        if (e.Column.DataType == dataRef.DsData.Tables[0].Columns[RefPk].DataType)
                        {
                            filterCondition = "(" + RefPk + "=" + quote1 + value + quote1 + ")" + (filterCondition == "" ? "" : " and (" + filterCondition + ")");
                            RPk = TableRef.Select(filterCondition);
                        }
                    }
                    else if (dataRef.dataType == DataType.Detail || (dataRef.dataType == DataType.MasterDetail && dataRef.DrTable["tableName"].ToString() == tableName))
                    {
                        RefPk = drField["RefField"].ToString(); //dataRef.DrTable["pk"].ToString();
                        TableRef = dataRef.DsData.Tables[1];
                        if (e.Column.DataType == dataRef.DsData.Tables[1].Columns[RefPk].DataType) {
                            filterCondition = "(" + RefPk + "=" + quote1 + value + quote1 + ")" + (filterCondition == "" ? "" : " and (" + filterCondition + ")");
                            RPk = TableRef.Select(filterCondition);
                        }
                    }

                    if (RPk == null || RPk.Length == 0) break;
                    SetValuesFromListDt(e.Row, e.Column.ColumnName, value, RPk[0], this._dsStruct.Tables[1]);
                    e.Row.EndEdit();

                }
            }
        }

        private void DataTable1_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Detached || e.Row.RowState == DataRowState.Deleted || e.Row.RowState == DataRowState.Unchanged) return;
            foreach (DataRow drstruct in _dsStruct.Tables[1].Rows)
            {
                if (drstruct["EditMask"].ToString() == "&")
                    drstruct["DefaultValue"] = e.Row[drstruct["fieldName"].ToString()];
            }
            this.DataChanged = true;
        }

        private void DataTable1_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (flagRowdeleteHand || flagRowdeleteHandTotal)
            {
                flagRowdeleteHand = false;

            }
            else
            {
                if (e.Row.RowState == DataRowState.Deleted)
                {
                    e.Row.RejectChanges();

                    flagRowdeleteHand = true;
                }
                if (saveHistory)
                {
                    int currIndex = this._lstDrCurrentDetails.IndexOf(e.Row);
                    int deletedRowCount = this._lstDrCurrentDetails.Take(currIndex).Count(row => row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached);
                    string tip = "Trước khi xóa dòng số " + (currIndex - deletedRowCount + 1).ToString() + " trên bảng chi tiết";
                    if (HistoryCurrents.Count > 0)
                    {
                        DateTime lastSavehis = HistoryCurrents.Last().datetime;
                        if ((DateTime.Now - lastSavehis).TotalMilliseconds >= 3000)
                        {
                            HistoryCurrents.Add(new historyCurrent(DateTime.Now, tip, this.GenSQLDataLog()));
                            if (HistoryCurrents.Count > 20) HistoryCurrents.RemoveAt(0);
                            if (this.AddHis != null)
                                this.AddHis.Invoke();
                        }
                    }
                    else
                    {
                        HistoryCurrents.Add(new historyCurrent(DateTime.Now, tip, this.GenSQLDataLog()));
                        if (this.AddHis != null)
                            this.AddHis.Invoke();
                    }
                }
                e.Row.Delete();
            }
            flagRowdeleteHand = false;
            if (!this._lstDrCurrentDetails.Contains(e.Row))
            {
                this._lstDrCurrentDetails.Add(e.Row);
            }
            this.DataChanged = true;
        }

        private void DataTable1_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            int deletedRowCount = this._lstDrCurrentDetails.Count(row => row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached);
            string tip = "Trước khi thêm dòng số " + (this._lstDrCurrentDetails.Count - deletedRowCount + 1).ToString() + " trên bảng chi tiết" ;
            if (saveHistory)
            {
                if (HistoryCurrents.Count > 0)
                {
                    DateTime lastSavehis = HistoryCurrents.Last().datetime;
                    if ((DateTime.Now - lastSavehis).TotalMilliseconds >= 3000)
                    {
                        HistoryCurrents.Add(new historyCurrent(DateTime.Now, tip, this.GenSQLDataLog()));
                        if (HistoryCurrents.Count > 20) HistoryCurrents.RemoveAt(0);
                        if (this.AddHis != null)
                            this.AddHis.Invoke();
                    }
                }
                else
                {
                    HistoryCurrents.Add(new historyCurrent(DateTime.Now, tip, this.GenSQLDataLog()));
                    if (this.AddHis != null)
                        this.AddHis.Invoke();
                }
            }
            this.SetDefaultValues(this._dsStruct.Tables[1], e.Row);
            this.DataChanged = true;
            this._lstDrCurrentDetails.Add(e.Row);

        }

        private void GenSQLMulDetail()
        {
            this._sInsertDt.Clear();
            this._sUpdateDt.Clear();
            this._sDeleteDt.Clear();
            this._vInsertDt.Clear();
            this._vUpdateDt.Clear();
            this._vDeleteDt.Clear();
            this._identityPkMulDt.Clear();
            for (int i = 0; i < this._dtDetail.Rows.Count; i++)
            {
                this._identityPkMulDt.Add(false);
                string _si = "insert into " + this._drTableDt[i]["TableName"].ToString() + "(";
                string _su = "update " + this._drTableDt[i]["TableName"].ToString() + " set ";
                string _sd = "delete from " + this._drTableDt[i]["TableName"].ToString();
                List<SqlField> _vi = new List<SqlField>();
                List<SqlField> _vu = new List<SqlField>();
                List<SqlField> _vd = new List<SqlField>();
                string condition = string.Empty;
                string tmp = " values(";
                foreach (DataRow drField in this._dsStructDt.Tables[i].Rows)
                {
                    string fieldName = drField["FieldName"].ToString();
                    int type = int.Parse(drField["Type"].ToString());
                    switch (type)
                    {
                        case 0:
                        case 3:
                        case 6:
                            condition = " where " + fieldName + " = @" + fieldName;
                            _vd.Add(new SqlField(fieldName, this.GetDbType(type)));
                            break;
                    }
                    _vu.Add(new SqlField(fieldName, this.GetDbType(type)));
                    if (type == 3)
                    {
                        this._identityPkMulDt[i] = true;
                    }
                    else
                    {
                        _si = _si + fieldName + ",";
                        tmp = tmp + "@" + fieldName + ",";
                        _vi.Add(new SqlField(fieldName, this.GetDbType(type)));
                        _su += fieldName + " = @" + fieldName + ",";
                    }
                }
                _si = _si.Remove(_si.Length - 1) + ")" + tmp.Remove(tmp.Length - 1) + ")";
                _su = _su.Remove(_su.Length - 1) + condition;
                _sd = _sd + condition;
                this._sInsertDt.Add(_si);
                this._sUpdateDt.Add(_su);
                this._sDeleteDt.Add(_sd);
                this._vInsertDt.Add(_vi);
                this._vUpdateDt.Add(_vu);
                this._vDeleteDt.Add(_vd);
            }
        }

        protected void GenSqlString()
        {
            string fieldName;
            int type;

            string tableName = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["TableName"].ToString() : this._drTable["TableName"].ToString();
            this._sInsert = "insert into " + tableName + "(";
            this._sUpdate = "update " + tableName + " set ";
            this._sDelete = "delete from " + tableName;
            this._sUpdateImage = "update " + tableName + " set ";
            //Thao tác với file
            //----------------
            string userID = Config.GetValue("sysUserID").ToString();
            this._sUpdateWs = "update " + tableName + " set ws ='" + userID + "_'";
            string condition = string.Empty;
            string tmp = " values(";
            foreach (DataRow drField in this._dsStruct.Tables[0].Rows)
            {
                fieldName = drField["FieldName"].ToString();
                type = int.Parse(drField["Type"].ToString());
                switch (type)
                {
                    case 0:
                    case 6:
                        if (fieldName == this.PkMaster.FieldName)
                        {
                            condition = " where " + fieldName + " = @" + fieldName;
                        }
                        this._vUpdate.Add(new SqlField(fieldName, this.GetDbType(type)));
                        this._vDelete.Add(new SqlField(fieldName, this.GetDbType(type)));
                        break;
                }
                if (type == 3)
                {
                    if (fieldName == this.PkMaster.FieldName)
                    {
                        this._identityPk = true;
                        condition = " where " + fieldName + " = @" + fieldName;
                    }
                   
                    
                    this._vUpdate.Add(new SqlField(fieldName, this.GetDbType(type)));
                    this._vDelete.Add(new SqlField(fieldName, this.GetDbType(type)));
                }
                else
                {
                    if (type == 12)
                    {
                        this._vUpdateImage.Add(new SqlField(fieldName, this.GetDbType(type)));
                        this._sUpdateImage += fieldName + "=@" + fieldName + ",";
                        continue;
                    }

                    if ((((type != 0) && (type != 6)) && (type != 3)))
                    {
                        this._vUpdate.Add(new SqlField(fieldName, this.GetDbType(type)));
                        this._sUpdate += fieldName + " = @" + fieldName + ",";
                    }
                    this._sInsert = this._sInsert + fieldName + ",";
                    tmp = tmp + "@" + fieldName + ",";
                    this._vInsert.Add(new SqlField(fieldName, this.GetDbType(type)));
                }
            }
            this._sInsert = this._sInsert.Remove(this._sInsert.Length - 1) + ")" + tmp.Remove(tmp.Length - 1) + ")";
            this._sUpdate = this._sUpdate.Remove(this._sUpdate.Length - 1) + condition;
            this._sUpdateImage = this._sUpdateImage.Remove(this._sUpdateImage.Length - 1);
            this._sDelete = this._sDelete + condition;
            if (this._dataType == DataType.MasterDetail)
            {
                this._sInsertDetail = "insert into " + this._drTable["TableName"].ToString() + "(";
                this._sUpdateDetail = "update " + this._drTable["TableName"].ToString() + " set ";
                this._sDeleteDetail = "delete from " + this._drTable["TableName"].ToString();
                condition = string.Empty;
                tmp = " values(";
                foreach (DataRow drField in this._dsStruct.Tables[1].Rows)
                {
                    fieldName = drField["FieldName"].ToString();
                    type = int.Parse(drField["Type"].ToString());
                    switch (type)
                    {
                        case 0:
                        case 3:
                        case 6:
                            condition = " where " + fieldName + " = @" + fieldName;
                            this._vDeleteDetail.Add(new SqlField(fieldName, this.GetDbType(type)));
                            break;
                    }
                    this._vUpdateDetail.Add(new SqlField(fieldName, this.GetDbType(type)));
                    if (type == 3)
                    {
                        this._identityPkDt = true;
                    }
                    else
                    {
                        this._sInsertDetail = this._sInsertDetail + fieldName + ",";
                        tmp = tmp + "@" + fieldName + ",";
                        this._vInsertDetail.Add(new SqlField(fieldName, this.GetDbType(type)));
                        this._sUpdateDetail += fieldName + " = @" + fieldName + ",";
                    }
                }
                this._sInsertDetail = this._sInsertDetail.Remove(this._sInsertDetail.Length - 1) + ")" + tmp.Remove(tmp.Length - 1) + ")";
                this._sUpdateDetail = this._sUpdateDetail.Remove(this._sUpdateDetail.Length - 1) + condition;
                this._sDeleteDetail = this._sDeleteDetail + condition;
                this.GenSQLMulDetail();
            }
        }

        protected virtual string GetContentForHistory(DataSet dsDataCopy, ref string pkValue)
        {
            int fType;
            string fieldName;
            string fieldValue;
            string labelName;

            string s = string.Empty;
            DataView dvData = new DataView(dsDataCopy.Tables[0]);
            dvData.RowStateFilter = DataViewRowState.ModifiedOriginal | DataViewRowState.Deleted;
            if (dvData.Count > 0)
            {
                string pk = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["Pk"].ToString() : this._drTable["Pk"].ToString();
                pkValue = dvData[0][pk].ToString();
                foreach (DataRow drField in this._dsStruct.Tables[0].Rows)
                {
                    fType = int.Parse(drField["Type"].ToString());
                    if (((((fType != 3) && (fType != 4)) && ((fType != 6) && (fType != 7))) && (fType != 12)) && (fType != 13))
                    {
                        fieldName = drField["FieldName"].ToString();
                        // if (dvData[0][fieldName] != null) 
                        fieldValue = dvData[0][fieldName].ToString();
                        if ((dvData[0].Row.RowState != DataRowState.Modified) || !(dvData[0].Row[fieldName].ToString() == fieldValue))
                        {
                            labelName = drField["LabelName"].ToString();
                            s += labelName + ":" + fieldValue + "; ";
                        }
                    }
                }
            }
            if (this._dataType != DataType.Single)
            {
                dvData = new DataView(dsDataCopy.Tables[1]);
                dvData.RowStateFilter = DataViewRowState.ModifiedOriginal;

                foreach (DataRowView drDetail in dvData)
                {
                    s = s + "\n";
                    foreach (DataRow drField in this._dsStruct.Tables[1].Rows)
                    {
                        fType = int.Parse(drField["Type"].ToString());
                        if ((((fType != 3) && (fType != 4)) && (fType != 6)) && (fType != 7))
                        {
                            fieldName = drField["FieldName"].ToString();
                            fieldValue = drDetail[fieldName].ToString();
                            if ((drDetail.Row.RowState != DataRowState.Modified) || !(drDetail.Row[fieldName].ToString() == fieldValue))
                            {
                                labelName = drField["LabelName"].ToString();
                                s += labelName + ":" + fieldValue + "; ";
                            }
                        }
                    }
                }
            }
            return s;
        }
        public abstract event EventHandler DataSoureChanged;
        public abstract void GetData();

        public void GetDataForLookup(CDTData ParentData)
        { try
            {
                if (this.dataType == DataType.Single)
                    (this as DataSingle).GetData(ParentData);

            }
            catch (Exception ex) {
                //MessageBox.Show(ex.Message);
            }


        }


        public abstract DataTable GetDataForPrint(int index);
        public abstract DataTable GetDataForPrint(int index, string _Script);

        private SqlDbType GetDbType(int fType)
        {
            SqlDbType tmp = SqlDbType.VarChar;
            switch (fType)
            {
                case 0:
                case 1:
                    return SqlDbType.NVarChar;

                case 2:
                case 16:
                    return SqlDbType.NVarChar;
                case 3:
                case 4:
                case 5:
                    return SqlDbType.Int;

                case 6:
                case 7:
                case 15:
                    return SqlDbType.UniqueIdentifier;

                case 8:
                    return SqlDbType.Decimal;

                case 9:
                case 11:
                case 14:
                    return SqlDbType.DateTime;

                case 10:
                    return SqlDbType.Bit;

                case 12:
                    return SqlDbType.Image;

                case 13:
                    return SqlDbType.NText;
            }
            return tmp;
        }

        public virtual void GetInfor(DataRow drTable)
        {
            if (drTable.Table.Columns.Contains("TableName")) _tableName = drTable["TableName"].ToString();
            else _tableName = string.Empty;
            this._drTable = drTable;
            if (this._drTable.Table.Columns.Contains("sysPackageID2") && (this._drTable["sysPackageID2"].ToString() == string.Empty))
            {
                this._dbData = this._dbStruct;
            }
            GetAction();
            this.InsertHistory();
        }

        public virtual void GetInfor(string sysTableID)
        {
            DataTable dt = this._dbStruct.GetDataTable("select * from sysTable t left join sysUserTable ut on t.sysTableID = ut.sysTableID where t.sysTableID = " + sysTableID);
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                GetAction();
                this._drTable = dt.Rows[0];
                _tableName = this._drTable["TableName"].ToString();
                this.InsertHistory();
            }
            else
            {
                dt = this._dbStruct.GetDataTable("select * from sysTable t left join sysUserTable ut on t.sysTableID = ut.sysTableID where t.sysTableID = '" + sysTableID + "' and t.sysPackageID = 5");
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    this._drTable = dt.Rows[0];
                    _tableName = this._drTable["TableName"].ToString();
                    this._dbData = this._dbStruct;
                    this.InsertHistory();
                }
            }

        }

        public virtual void GetInfor(string TableName, string sysPackageID)
        {
            _tableName = TableName;
            DataTable dt = this._dbStruct.GetDataTable("select * from sysTable t left join sysUserTable ut on t.sysTableID = ut.sysTableID where t.TableName = '" + TableName + "' and t.sysPackageID = " + sysPackageID);
            if ((dt != null) && (dt.Rows.Count > 0))
            {

                this._drTable = dt.Rows[0];
                GetAction();
                this.InsertHistory();
            }
            else
            {
                dt = this._dbStruct.GetDataTable("select * from sysTable t left join sysUserTable ut on t.sysTableID = ut.sysTableID where t.TableName = '" + TableName + "' and t.sysPackageID = 5");
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    this._drTable = dt.Rows[0];
                    this._dbData = this._dbStruct;
                    this.InsertHistory();
                }
            }

        }

        private void GetPkMaster()
        {
            foreach (DataRow drField in this.DsStruct.Tables[0].Rows)
            {
                string fieldName = drField["FieldName"].ToString();
                int type = int.Parse(drField["Type"].ToString());
                switch (type)
                {
                    case 0:
                    case 6:
                        this.PkMaster = new SqlField(fieldName, this.GetDbType(type));
                        this.quote = "'";
                        break;
                    case 3:
                        this.PkMaster = new SqlField(fieldName, this.GetDbType(type));
                        break;
                }
            }
        }
        private string getQuote(DataRow drField)
        {
            int type = int.Parse(drField["type"].ToString());
            int[] ltype = new int[] { 0, 1, 2, 6, 7, 9, 13, 14, 15 };

            if (ltype.Contains(type))
            {
                return "'";
            }
            else
            {
                return "";
            }    
        }
        public DataTable GetRelativeData()
        {
            string sysPackageID = Config.GetValue("sysPackageID").ToString();
            string s = "select * from sysTable t, sysField f where t.sysTableID = f.sysTableID and t.sysPackageID = " + sysPackageID + " and f.refTable like '" + this.DrTable["TableName"].ToString() + "' order by DienGiai";
            return this._dbStruct.GetDataTable(s);
        }

        public DataTable GetRelativeFunction()
        {
            string sysPackageID = Config.GetValue("sysPackageID").ToString();
            string s = "select * from sysTable where sysPackageID = " + sysPackageID + " and MasterTable like '" + this.DrTable["TableName"].ToString() + "' order by DienGiai";
            return this._dbStruct.GetDataTable(s);
        }

        public virtual void GetStruct()
        {
            if (this._drTable == null) return;
            string sysTableID = this._drTable["SysTableID"].ToString();
            string queryString = "select *, d.CurrValue from sysField f left join  sysUserField uf on f.sysFieldID = uf.sysFieldID left join (select * from sysCurValue where sysdbid=" + Config.GetValue("sysDBID").ToString() + ") d on f.sysfieldid = d.sysfieldid where  f.sysTableID = " + sysTableID + "     order by TabIndex";
            DataTable dtStruct = this._dbStruct.GetDataTable(queryString);
            if (dtStruct != null)
            {
                dtStruct.TableName = _drTable["TableName"].ToString();
                this._dsStruct.Tables.Add(dtStruct);
                this.GetPkMaster();
            }
            if (DrTable.Table.Columns.Contains("useBand") && bool.Parse(DrTable["useBand"].ToString()))
            {
                string sql = "select * from sysband where MTID=" + DrTable["sysTableID"].ToString();
                DataTable tb = this._dbStruct.GetDataTable(sql);

                if (tb != null)
                {
                    tb.TableName = DrTable["TableName"].ToString();
                    _dsBand.Tables.Add(tb);
                }

            }
        }

        private void InsertHistory()
        {
            if (this._drTable.Table.Columns.Contains("sysMenuID"))
            {
                string sysMenuID = this._drTable["sysMenuID"].ToString();
                string action = "Xem";
                new SysHistory().InsertHistory(sysMenuID, action, string.Empty, string.Empty);
            }
        }
        protected void InsertHistory(DataAction dataAction, DataSet dsDataCopy)
        {
            string sysMenuID;
            if (this._drTable.Table.Columns.Contains("sysMenuID"))
            {
                sysMenuID = this._drTable["sysMenuID"].ToString();
            }
            else
            {
                sysMenuID = "1";
            }
            string action = string.Empty;
            string content = string.Empty;
            string pkValue = string.Empty;
            try
            {

                string pk = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["Pk"].ToString() : this._drTable["Pk"].ToString();
                string sysTableID = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["sysTableID"].ToString() : this._drTable["sysTableID"].ToString();
                DataView dvData = new DataView(dsDataCopy.Tables[0]);
                dvData.RowStateFilter = DataViewRowState.ModifiedOriginal | DataViewRowState.Deleted;
                DataView dvCrData = new DataView(dsDataCopy.Tables[0]);
                //dvCrData.RowStateFilter = DataViewRowState.Added | DataViewRowState.ModifiedCurrent;
                
                switch (dataAction)
                {
                    case DataAction.Insert:
                        action = "Mới";
                        dvCrData.RowStateFilter = DataViewRowState.Added;
                        break;
                    case DataAction.Update:
                        action = "Sửa";

                        dvCrData.RowStateFilter = DataViewRowState.ModifiedCurrent;
                        break;

                    case DataAction.Delete:
                        action = "Xóa";
                        dvData.RowStateFilter = DataViewRowState.Deleted;
                        break;

                    case DataAction.IUD:
                        action = "Xem";
                        return;
                       // break;
                }
                //Insert vào bảng sysHistory
                // content = this.GetContentForHistory(dsDataCopy, ref pkValue);
                if (dataAction == DataAction.IUD) return;
                SysHistory sH = new SysHistory();

                // dvCrData.RowStateFilter = DataViewRowState.CurrentRows;
                bool isMasterUnchange = false;
                if (dvData.Count == 0 && dvCrData.Count == 0) //Không thêm sửa xóa gì 
                {
                    pkValue = dsDataCopy.Tables[0].Rows[_drCurrIndex][pk].ToString();
                    isMasterUnchange = true;
                }
                else if (dataAction == DataAction.Delete)
                    pkValue = dvData[0][pk].ToString();
                else
                    pkValue = dvCrData[0][pk].ToString();
                object o = sH.InsertHistory(sysMenuID, action, pkValue, content, sysTableID);
                int fType;
                string fieldName;
                string fieldID;
                string fieldValue = "";
                string OldValue = "";

                string labelName;
                if (o != null && dataAction != DataAction.Delete)
                {
                    string sysHistoryID = o.ToString();
                    foreach (DataRow drField in this._dsStruct.Tables[0].Rows)
                    {
                        fType = int.Parse(drField["Type"].ToString());
                        if (fType != 12 && fType != 13)
                        {
                            fieldID = drField["sysFieldID"].ToString();
                            fieldName = drField["FieldName"].ToString();
                            // if (dvData[0][fieldName] != null) 
                            if (dataAction != DataAction.Delete && !isMasterUnchange) fieldValue = dvCrData[0][fieldName].ToString();
                            if (dataAction != DataAction.Insert && !isMasterUnchange) OldValue = dvData[0][fieldName].ToString();
                            if (dataAction != DataAction.Update || fieldValue != OldValue)
                                sH.InsertHistoryDt(o.ToString(), fieldID, fieldValue, OldValue);
                            //s += labelName + ":" + fieldValue + "; ";

                        }
                    }
                    if (this._dataType == DataType.MasterDetail)
                    {
                        InsertHistoryDt(sysMenuID, dsDataCopy.Tables[1], _dsStruct.Tables[1], this._drTable["Pk"].ToString(), pkValue);
                        for (int i = 0; i < _drTableDt.Count; i++)
                        {
                            InsertHistoryDt(sysMenuID, dsDataCopy.Tables[i + 2], _dsStructDt.Tables[i], _drTableDt[i]["Pk"].ToString(), pkValue);
                        }

                    }

                }
            }
            catch
            {
            }
        }
        protected void InsertHistory(DataAction dataAction, DataSet dsDataCopy, DateTime Btime, DateTime Etime)
        {
            string sysMenuID;
            if (this._drTable.Table.Columns.Contains("sysMenuID"))
            {
                sysMenuID = this._drTable["sysMenuID"].ToString();
            }
            else
            {
                sysMenuID = "1";
            }
            string action = string.Empty;
            string content = string.Empty;
            string pkValue = string.Empty;
            try
            {

                string pk = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["Pk"].ToString() : this._drTable["Pk"].ToString();
                string sysTableID = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["sysTableID"].ToString() : this._drTable["sysTableID"].ToString();
                DataView dvData = new DataView(dsDataCopy.Tables[0]);
                dvData.RowStateFilter = DataViewRowState.ModifiedOriginal | DataViewRowState.Deleted;
                DataView dvCrData = new DataView(dsDataCopy.Tables[0]);
                //dvCrData.RowStateFilter = DataViewRowState.Added | DataViewRowState.ModifiedCurrent;
                switch (dataAction)
                {
                    case DataAction.Insert:
                        action = "Mới";
                        dvCrData.RowStateFilter = DataViewRowState.Added;
                        break;
                    case DataAction.Update:
                        action = "Sửa";

                        dvCrData.RowStateFilter = DataViewRowState.ModifiedCurrent;
                        break;

                    case DataAction.Delete:
                        action = "Xóa";
                        dvData.RowStateFilter = DataViewRowState.Deleted;
                        break;

                    case DataAction.IUD:
                        action = "Xem";
                        return;
                       // break;
                }
                //Insert vào bảng sysHistory
                // content = this.GetContentForHistory(dsDataCopy, ref pkValue);
                // if (dataAction == DataAction.IUD) return;
                SysHistory sH = new SysHistory();





                // dvCrData.RowStateFilter = DataViewRowState.CurrentRows;
                bool isMasterUnchange = false;
                if (dvData.Count == 0 && dvCrData.Count == 0) //Không thêm sửa xóa gì 
                {
                    pkValue = dsDataCopy.Tables[0].Rows[_drCurrIndex][pk].ToString();
                    isMasterUnchange = true;
                }
                else if (dataAction == DataAction.Delete)
                    pkValue = dvData[0][pk].ToString();
                else if (dataAction == DataAction.IUD && this.dataType == DataType.Report)
                    pkValue = "";
                else

                    pkValue = dvCrData[0][pk].ToString();
                object o = sH.InsertHistory(sysMenuID, action, pkValue, content, sysTableID, Btime, Etime);
                int fType;
                string fieldName;
                string fieldID;
                string fieldValue = "";
                string OldValue = "";

                string labelName;
                if (o != null && dataAction != DataAction.Delete)
                {
                    string sysHistoryID = o.ToString();
                    foreach (DataRow drField in this._dsStruct.Tables[0].Rows)
                    {
                        fType = int.Parse(drField["Type"].ToString());
                        if (fType != 12 && fType != 13)
                        {
                            fieldID = drField["sysFieldID"].ToString();
                            fieldName = drField["FieldName"].ToString();
                            // if (dvData[0][fieldName] != null) 
                            if (dataAction != DataAction.Delete && !isMasterUnchange) fieldValue = dvCrData[0][fieldName].ToString();
                            if (dataAction != DataAction.Insert && !isMasterUnchange) OldValue = dvData[0][fieldName].ToString();
                            if (dataAction != DataAction.Update || fieldValue != OldValue)
                                sH.InsertHistoryDt(o.ToString(), fieldID, fieldValue, OldValue);
                            //s += labelName + ":" + fieldValue + "; ";

                        }
                    }
                    if (this._dataType == DataType.MasterDetail)
                    {
                        InsertHistoryDt(sysMenuID, dsDataCopy.Tables[1], _dsStruct.Tables[1], this._drTable["Pk"].ToString(), pkValue);
                        for (int i = 0; i < _drTableDt.Count; i++)
                        {
                            InsertHistoryDt(sysMenuID, dsDataCopy.Tables[i + 2], _dsStructDt.Tables[i], _drTableDt[i]["Pk"].ToString(),pkValue);
                        }

                    }

                }
            }
            catch
            {
            }
        }
        protected void InsertHistoryDt(string sysMenuID, DataTable tbData, DataTable tbStruct, string pkName,string pkValue)
        {
            DataView dvDataAdd = new DataView(tbData);
            dvDataAdd.RowStateFilter = DataViewRowState.Added;

            DataView dvDataModi = new DataView(tbData);
            dvDataModi.RowStateFilter = DataViewRowState.ModifiedOriginal;
            DataView dvDataModiCr = new DataView(tbData);
            dvDataModiCr.RowStateFilter = DataViewRowState.ModifiedCurrent;
            DataView dvDataDe = new DataView(tbData);
            dvDataDe.RowStateFilter = DataViewRowState.Deleted;
            string pkNameDt = pkName;
            string pkValueDt;
            object o = null;
            int fType;
            string fieldID;
            string fieldName = "";
            string fieldValue = "";
            string OldValue = "";
            SysHistory sH = new SysHistory();
            foreach (DataRowView drData in dvDataAdd)
            {
                pkValueDt = drData[pkNameDt].ToString();

                o = sH.InsertHistory(sysMenuID, "Mới", pkValueDt, "", this._drTable["sysTableID"].ToString());
                foreach (DataRow drField in tbStruct.Rows)
                {
                    fType = int.Parse(drField["Type"].ToString());
                    if (fType != 12 && fType != 13)
                    {
                        fieldID = drField["sysFieldID"].ToString();
                        fieldName = drField["FieldName"].ToString();

                        fieldValue = drData[fieldName].ToString();
                        if (fieldValue != string.Empty || OldValue != string.Empty)
                            sH.InsertHistoryDt(o.ToString(), fieldID, fieldValue, OldValue);


                    }
                }

            }
            for (int i = 0; i < dvDataModi.Count; i++)
            {
                if (dvDataModi.Count != dvDataModiCr.Count) break;
                DataRowView drData = dvDataModi[i];
                DataRowView drDataCr = dvDataModiCr[i];
                pkValueDt = drData[pkNameDt].ToString();
                o = sH.InsertHistory(sysMenuID, "Sửa", pkValueDt, "", this._drTable["sysTableID"].ToString());
                foreach (DataRow drField in tbStruct.Rows)
                {
                    fType = int.Parse(drField["Type"].ToString());
                    if (fType != 12 && fType != 13)
                    {
                        fieldID = drField["sysFieldID"].ToString();
                        fieldName = drField["FieldName"].ToString();
                        fieldValue = drDataCr[fieldName].ToString();
                        OldValue = drData[fieldName].ToString();
                        if ((fieldValue != string.Empty || OldValue != string.Empty) && fieldValue != OldValue)
                            sH.InsertHistoryDt(o.ToString(), fieldID, fieldValue, OldValue);


                    }
                }
            }

            foreach (DataRowView drData in dvDataDe)
            {
                pkValueDt = drData[pkNameDt].ToString();


                o = sH.InsertHistory(sysMenuID, "Xóa", pkValueDt, "", this._drTable["sysTableID"].ToString(), pkValue);
                foreach (DataRow drField in tbStruct.Rows)
                {
                    fType = int.Parse(drField["Type"].ToString());
                    if (fType != 12 && fType != 13)
                    {
                        fieldID = drField["sysFieldID"].ToString();
                        fieldName = drField["FieldName"].ToString();

                        OldValue = drData[fieldName].ToString();
                        if (fieldValue != string.Empty || OldValue != string.Empty)
                            sH.InsertHistoryDt(o.ToString(), fieldID, "", OldValue);


                    }
                }

            }
        }
        protected bool IsUnique(DataAction dataAction, string value, string fieldName, string tableName, string pk, string pkValue, DataRow drField)
        {
            string quotefield = getQuote(drField);
            string sql = "select " + fieldName + " from " + tableName + " where " + fieldName + " = " + quotefield + value + quotefield;
            if (dataAction == DataAction.Update)
            {
                sql += " and " + pk + " <> " + quotefield + pkValue + quotefield;
            }
            if (DrCurrentMaster.Table.Columns.Contains("NgayCt") && DrCurrentMaster["NgayCt"] != DBNull.Value)
            {
                if (drField["EditMask"] != DBNull.Value && drField["EditMask"].ToString().Contains("MM"))
                    sql += " and ngayct between dbo.LayNgayDauthang(cast('" + DrCurrentMaster["NgayCt"].ToString() + "' as datetime)) and dbo.LayNgayGhiSo(cast('" + DrCurrentMaster["NgayCt"].ToString() + "' as datetime)) ";
                else if (drField["EditMask"] != DBNull.Value && drField["EditMask"].ToString().Contains("YY"))
                {
                    sql += " and ngayct between '01/01/" + DateTime.Parse(DrCurrentMaster["NgayCt"].ToString()).Year.ToString() + "' and   '12/31/" + DateTime.Parse(DrCurrentMaster["NgayCt"].ToString()).Year.ToString() + "'";
                }    
            }
            DataTable dtData = this._dbData.GetDataTable(sql);
            return ((dtData == null) || (dtData.Rows.Count == 0));
        }

        public bool KiemtraDemo()
        {
            if (Config.GetValue("isDemo").ToString() == "1")
            {
                string sql = "select thang from (select cast(month(ngayct)as nvarchar(2)) + cast(year(ngayct)as nvarchar(4))    as thang from bltk with(nolock) ) x group by thang";
                if (this._dbData.GetDataTable(sql).Rows.Count >= 2)
                {
                    MessageBox.Show("Bản Demo chỉ sử dụng trong 1 th\x00e1ng số liệu");
                    return false;
                }
            }
            return true;
        }

        public void Reset()
        {
            this._lstDrCurrentDetails = new List<DataRow>();
            this._drCurrentMaster = null;
            this._lstCurRowDetail = new List<CurrentRowDt>();
            this._formulaCaculator.LstCurrentRowDt = this._lstCurRowDetail;
            this.DataChanged = false;
        }

        protected virtual void SetDefaultValues(DataTable dtStruct, DataRow drData)
        {
            if ((Config.GetValue("sysReportID") != null) && drData.Table.Columns.Contains("sysReportID"))
            {
                drData["sysReportID"] = Config.GetValue("sysReportID");
            }

            if (this._formulaCaculator != null)
            {
                this._formulaCaculator.Active = false;
            }
            int c = 0;

            foreach (DataRow drField in dtStruct.Rows)
            {
                try
                {
                    //if (this._formulaCaculator != null && c == dtStruct.Rows.Count - 1)
                    //    this._formulaCaculator.Active = true;
                    string fieldName = drField["FieldName"].ToString();
                    string defaultValue = drField["DefaultValue"].ToString();
                    //if (fieldName.ToLower() == "mahttt")
                    //{

                    //}
                    if (drData[fieldName] != DBNull.Value && drData[fieldName].ToString() != "" && !defaultValue.Contains("@@"))
                        continue;
                    //if (fieldName == "HDDV")
                    //{

                    //}
                    if (this._drCurrentMaster != null)
                    {
                        string formulaDetail = drField["FormulaDetail"].ToString();
                        if (((formulaDetail != string.Empty) && !formulaDetail.Contains(".")) && this._drCurrentMaster.Table.Columns.Contains(formulaDetail))
                        {
                            drField["DefaultValue"] = this._drCurrentMaster[formulaDetail];
                            defaultValue= drField["DefaultValue"].ToString();
                        }
                        if (((formulaDetail != string.Empty) && !formulaDetail.Contains(".")) && this.DsData.Tables.Count > 1 && this.DsData.Tables[1].Columns.Contains(formulaDetail))
                        {

                        }
                       
                    }


                    if ((this._drTable.Table.Columns.Contains("TableName") && (this._drTable["TableName"].ToString().ToUpper() != "SYSPACKAGE")) && (fieldName.ToUpper() == "SYSPACKAGEID"))
                    {
                        if (this._drTable["TableName"].ToString().ToUpper() == "SYSCONFIG")
                        {
                            drData[fieldName] = Config.GetValue("sysPackageID").ToString();
                        }
                        else if ((Config.GetValue("curPackageID") != null) && (Config.GetValue("curPackageID").ToString().Trim() != string.Empty))
                        {
                            drData[fieldName] = Config.GetValue("curPackageID").ToString();
                        }
                    }

                    
                    //if (defaultValue == string.Empty) continue;
                    if (defaultValue.Contains("@@"))
                    {
                        int ib = defaultValue.IndexOf("(");
                        int ie = defaultValue.IndexOf(")");
                        string para = defaultValue.Substring(ib + 3, ie - ib - 3);

                        foreach (DictionaryEntry de in Config.Variables)
                        {
                            if (de.Key.ToString() == para)
                            {
                                defaultValue = defaultValue.Replace("(@@" + para + ")", de.Value.ToString());
                                break;
                            }
                        }
                    }
                    if (defaultValue != string.Empty && defaultValue.Substring(0, 1) == "@" && !defaultValue.Contains("@@"))
                    {
                        string para = defaultValue.Substring(1, defaultValue.Length - 1);
                        if (DrCurrentMaster.Table.Columns.Contains(para))
                            drData[fieldName] = DrCurrentMaster[para];
                        else if (dataType == DataType.MasterDetail)// Lấy mặc định từ dòng detail đang thực hiện
                        {
                            if (DsData.Tables[1].Columns.Contains(para))//
                            {
                                string valueID = drData["DTID"].ToString();
                                //_lstDrCurrentDetails
                            }
                        }
                    }
                    int pType = int.Parse(drField["Type"].ToString());
                    if (((pType == 9) && (drField["EditMask"].ToString() == "n")) && (this.dataType != DataType.Report))
                    {
                        drData[fieldName] = Config.GetValue("NgayHethong").ToString();
                    }
                    if ((pType == 14) && (this.dataType != DataType.Report))
                    {
                        drData[fieldName] = DateTime.Now;
                    }
                    if ((defaultValue != string.Empty) || (pType == 6))
                    {
                        if (pType == 6)
                        {
                            drData[fieldName] = Guid.NewGuid();
                        }
                        else if(pType == 15 && drField["DefaultValue"].ToString().ToLower() == "newid()")
                        {
                            drData[fieldName] = Guid.NewGuid();
                        }
                        else if (pType == 10)
                        {
                            drData[fieldName] = defaultValue == "1" || defaultValue == "True";
                        }
                        else if ((pType == 9 || pType == 14) && (defaultValue == "n"))
                        {
                            drData[fieldName] = Config.GetValue("NgayHethong").ToString();
                        }
                        else if (defaultValue == "&")
                        {
                        }
                        else if (defaultValue != string.Empty && defaultValue.Substring(0, 1) == "@")
                        {
                        }
                        else
                        {
                            drData[fieldName] = defaultValue;
                        }
                    }
                    c++;
                }
                catch (Exception e)
                {
                    

                }
                //drData.EndEdit();
            }
            if (this._formulaCaculator != null)
            {
                this._formulaCaculator.Active = true;
            }
        }

    

        public void SetValuesFromList(string controlFrom, string value, DataRow drDataFrom, bool Refresh)
        {
            string formulaDetail;
            string[] str;
            List<object> ob;
            List<string> lstStr = new List<string>();
            if (this._drCurrentMaster != null)
            {   
                if (!Refresh)
                {
                    if (this._drCurrentMaster.Table.Columns.Contains(controlFrom) && this._drCurrentMaster[controlFrom].ToString() != value)
                        this._drCurrentMaster[controlFrom] = value;
                    foreach (DataRow drField in this._dsStruct.Tables[0].Rows)
                    {
                        formulaDetail = drField["FormulaDetail"].ToString();
                        if (!(formulaDetail == string.Empty))
                        {
                            str = formulaDetail.Split(".".ToCharArray());
                            if  ( str.Length>1 && !(controlFrom.ToUpper() != str[0].ToUpper())  )
                            {
                                string fieldName = drField["FieldName"].ToString();
                                if (this._drCurrentMaster[fieldName].ToString() != drDataFrom[str[1]].ToString())
                                    this._drCurrentMaster[fieldName] = drDataFrom[str[1]];
                                this._drCurrentMaster.EndEdit();
                            }
                        }
                    }
                }
                
                if (this._dataType == DataType.MasterDetail)
                {
                    foreach (DataRow drField in this._dsStruct.Tables[1].Rows)
                    {
                        formulaDetail = drField["FormulaDetail"].ToString();
                        if (formulaDetail != string.Empty)
                        {
                            str = formulaDetail.Split(".".ToCharArray());
                            if (!(controlFrom.ToUpper() != str[0].ToUpper()) || lstStr.Contains(str[0].ToUpper()))
                            {
                                lstStr.Add(drField["FieldName"].ToString().ToUpper());
                                
                                try
                                {
                                    if(str.Length==1)
                                         drField["DefaultValue"] = drDataFrom[str[0]];
                                    else
                                    drField["DefaultValue"] = drDataFrom[str[1]];// gán lại default value cho detail
                                }
                                catch { }
                                //Trường hợp cho thay đổi detail theo trên
                                ob = new List<object>();

                                ob.Add(drField["FieldName"]);
                                ob.Add(drField["DefaultValue"]);
                                ob.Add(0);
                                if (this.SetDetailValue != null)
                                    this.SetDetailValue(ob, new EventArgs());
                            }
                            
                        }
                    }
                }
            }
            if (this._dtDetail != null)
            {
                for (int i = 0; i < this._dtDetail.Rows.Count; i++)
                {
                    foreach (DataRow drField in this._dsStructDt.Tables[i].Rows)
                    {
                        formulaDetail = drField["FormulaDetail"].ToString();
                        if (formulaDetail != string.Empty)
                        {
                            str = formulaDetail.Split(".".ToCharArray());
                            if (str.Length != 2) continue;
                            if (!(controlFrom.ToUpper() != str[0].ToUpper()) || lstStr.Contains(str[0].ToUpper()))
                            {
                                lstStr.Add(drField["FieldName"].ToString().ToUpper());
                                drField["DefaultValue"] = drDataFrom[str[1]];
                                ob = new List<object>();
                                ob.Add(drField["FieldName"]);
                                ob.Add(drField["DefaultValue"]);
                                ob.Add(i + 1);
                                if (this.SetDetailValue != null)
                                    this.SetDetailValue(ob, new EventArgs());
                            }
                        }
                    }
                }
            }
        }


        public void SetValuesFromListDetail(DataRow drDetail, string controlFrom, string value, DataRow drDataFrom)
        {
            if ((this._lstDrCurrentDetails.Count != 0) && (drDetail != null))
            {
                drDetail[controlFrom] = value;
                foreach (DataTable tbStruct in this._dsStructDt.Tables)
                {
                    foreach (DataRow drField in tbStruct.Rows)
                    {
                        string formulaDetail = drField["FormulaDetail"].ToString();
                        if (formulaDetail != string.Empty)
                        {
                            string[] formulaDetail_arr = formulaDetail.Split("|".ToCharArray());
                            foreach (string formulaDetail_x in formulaDetail_arr)
                            {
                                string[] str = formulaDetail_x.Split(".".ToCharArray());
                                if (controlFrom.ToUpper() == str[0].ToUpper())
                                {
                                    string fieldName = drField["FieldName"].ToString();
                                    if (drDetail.Table.Columns[fieldName] != null)
                                    {
                                        drDetail[fieldName] = drDataFrom[str[1]];
                                    }
                                    drDetail.EndEdit();
                                }
                            }
                        }
                    }
                }
            }
        }
        public void SetValuesFromListDt(DataRow drDetail, string controlFrom, string value, DataRow drDataFrom)
        {
            if ((this._lstDrCurrentDetails.Count != 0) && (drDetail != null))
            {
                drDetail[controlFrom] = value;
                foreach (DataRow drField in this._dsStruct.Tables[1].Rows)
                {
                    string formulaDetail = drField["FormulaDetail"].ToString();
                    if (formulaDetail != string.Empty)
                    {
                        string[] formulaDetail_arr = formulaDetail.Split("|".ToCharArray());
                        foreach (string formulaDetail_x in formulaDetail_arr)
                        {
                            string[] str = formulaDetail_x.Split(".".ToCharArray());
                            if (controlFrom.ToUpper() == str[0].ToUpper())
                            {
                                string fieldName = drField["FieldName"].ToString();
                                if (drDetail.Table.Columns[fieldName] != null)
                                {
                                    drDetail[fieldName] = drDataFrom[str[1]];
                                }
                                drDetail.EndEdit();
                            }
                        }
                    }
                }
            }
        }
        public void SetValuesFromListDt(DataRow drDetail, string controlFrom, string value, DataRow drDataFrom, DataTable tbStruct)
        {
            
            if ( drDetail != null)//(this._lstDrCurrentDetails.Count != 0) &&
            {
                //drDetail[controlFrom] = value;
                foreach (DataRow drField in tbStruct.Rows)
                {
                    string formulaDetail = drField["FormulaDetail"].ToString();
                    if (formulaDetail != string.Empty)
                    {
                        string[] formulaDetail_arr = formulaDetail.Split("|".ToCharArray());
                        foreach (string formulaDetail_x in formulaDetail_arr)
                        {
                            string[] str = formulaDetail_x.Split(".".ToCharArray());
                            if (controlFrom.ToUpper() == str[0].ToUpper())
                            {
                                string fieldName = drField["FieldName"].ToString();
                                if (str.Length == 1) str = new string[] { str[0], str[0] };
                                if (drDetail.Table.Columns[fieldName] != null)
                                {
                                    if (fieldName == "GiaNT")
                                    {

                                    }
                                    if (drDetail[fieldName].ToString() != drDataFrom[str[1]].ToString())
                                        drDetail[fieldName] = drDataFrom[str[1]];
                                    //Công thức theo detail tiếp theo add vào chỗ này
                                    if(drField["refTable"]!=DBNull.Value)
                                    {
                                        //Tìm Data
                                        CDTTable.Table4Lookup tbRep = CDTTable.FindTable(drField["refTable"].ToString());
                                        if(tbRep.drTable!=null)
                                        {
                                            if (tbRep.Table.PrimaryKey.Length == 0)
                                                tbRep.Table.PrimaryKey = new DataColumn[] { tbRep.Table.Columns[tbRep.drTable["pk"].ToString()] };
                                            DataRow drFrom=null;
                                            try
                                            {
                                                drFrom = tbRep.Table.Rows.Find(drDetail[fieldName]);
                                            }
                                            catch
                                            {
                                            }
                                            if (drFrom != null && fieldName!=controlFrom)
                                                    SetValuesFromListDt(drDetail, fieldName, drDetail[fieldName].ToString(), drFrom, tbStruct);
                                        }
                                    }
                                }
                                drDetail.EndEdit();
                            }
                        }
                    }
                }
                DataRow[] rowst = tbStruct.Select("FieldName='" + controlFrom + "'");
                 if (rowst.Length > 0)
                 {
                     if (rowst[0]["EditMask"].ToString() == "&" ) 
                     { 
                        rowst[0]["defaultValue"] = value;
                     }
                 }
            }
        }

        protected bool TransferData(DataAction dataAction, int index)
        {
            if (this._drTable["TableName"].ToString() != "sysDataConfig")
            {
                string mtTableID;
                string pk;
                string PkValue;
                string RefKey;
                List<DataRow> drDetails = new List<DataRow>();
                if (this._dataType == DataType.MasterDetail)
                {
                    mtTableID = this._drTableMaster["sysTableID"].ToString();
                    pk = this._drTableMaster["Pk"].ToString();
                   DataRow[] ldrRef = this.DsStruct.Tables[1].Select("RefField='" + pk + "'");
                   if (ldrRef.Length > 0) RefKey = ldrRef[0]["FieldName"].ToString();
                   else RefKey = pk;
                }
                else
                {
                    mtTableID = this._drTable["sysTableID"].ToString();
                    pk = this._drTable["Pk"].ToString();
                    RefKey = pk;
                }
                if (this._dataTransfer == null)
                {
                    this._dataTransfer = new DataTransfer(this._dbData, mtTableID, pk);
                }
                if (dataAction == DataAction.Delete)
                {
                    PkValue = this._dsDataTmp.Tables[0].Rows[index][pk].ToString();
                }
                else
                {
                    PkValue = (this.dataType == DataType.MasterDetail) ? this._drCurrentMaster[pk].ToString() : this._dsData.Tables[0].Rows[index][pk].ToString();
                }
                
                bool masterEdit = false;
                if (this._dataType == DataType.MasterDetail)
                {
                    DataView dvMaster = new DataView(this._dsData.Tables[0]);

                    dvMaster.RowStateFilter = DataViewRowState.ModifiedCurrent;
                    
                    masterEdit = dvMaster.Count > 0;
                    DataView dv = new DataView(this._dsData.Tables[1]);
                    dv.RowFilter = RefKey + " = " + quote + PkValue + quote;
                    foreach (DataRowView dr in dv)
                    {
                        if (dr.Row.RowState != DataRowState.Unchanged)
                        {
                            drDetails.Add(dr.Row);
                        }
                        else if (dvMaster.Count > 0)
                        {
                            drDetails.Add(dr.Row);
                        }
                    }
                    dv.RowFilter = string.Empty;
                    dv.RowStateFilter = DataViewRowState.Deleted;
                    foreach (DataRowView dr in dv)
                    {
                        if (dr.Row.RowState != DataRowState.Unchanged)
                        {
                            drDetails.Add(dr.Row);
                        }
                    }
                }
               return this._dataTransfer.Transfer(dataAction, PkValue, drDetails, masterEdit);
            }
            return true;
        }

        protected bool Update(DataRow drData)
        {
            string fieldName;
            if (this._sInsert == string.Empty)
            {
                this.GenSqlString();
            }
            List<SqlField> tmp = new List<SqlField>();
            List<string> paraNames = new List<string>();
            List<object> paraValues = new List<object>();
            List<SqlDbType> paraTypes = new List<SqlDbType>();
            string sql = string.Empty;
            bool updateIdentity = false;
            bool isDelete = false;
            switch (drData.RowState)
            {
                case DataRowState.Added:
                    if (this._identityPk)
                    {
                        updateIdentity = true;
                    }
                    tmp = this._vInsert;
                    sql = this._sInsert;
                    break;

                case DataRowState.Deleted:
                    tmp = this._vDelete;
                    sql = this._sDelete;
                    drData.RejectChanges();
                    isDelete = true;
                    break;

                case DataRowState.Modified:
                    tmp = this._vUpdate;
                    sql = this._sUpdate;
                    break;
            }
            foreach (SqlField sqlField in tmp)
            {
                fieldName = sqlField.FieldName;
                paraNames.Add(fieldName);
                if (drData[fieldName] != DBNull.Value)
                {
                    paraValues.Add(drData[fieldName]);
                }
                else
                {
                    paraValues.Add(DBNull.Value);
                }
                paraTypes.Add(sqlField.DbType);
            }
            bool updateWsCompleted = true;
           

            if (sql == string.Empty)
            {
                return true;
            }
            bool result = this._dbData.UpdateData(sql, paraNames.ToArray(), paraValues.ToArray(), paraTypes.ToArray());
            string pk = string.Empty;
            pk = (this._dataType == DataType.MasterDetail) ? this._drTableMaster["Pk"].ToString() : this._drTable["Pk"].ToString();
            if (result && updateIdentity)
            {
                object o = this._dbData.GetValue("select @@identity");
                if (o != null)
                {
                    drData[pk] = o;
                }
            }
            //Update dữ liệu File
            for (int i = 0; i < this._fileData.Count; i++)
            {
                FileData fData = this._fileData[i];
                if (fData.isNew || (drData.RowState == DataRowState.Modified))
                {
                    string FfieldName = fData.drField["FieldName"].ToString();
                    string fileName = drData[FfieldName].ToString();
                    if (fileName == string.Empty) continue;
                    string pkID = drData[this.PkMaster.FieldName].ToString();
                    if (drData.RowState == DataRowState.Modified)
                    {
                        string sqldelete = "Delete FileList where sysfieldID=" + fData.drField["sysFieldID"].ToString() + " and PkID='" + pkID + "'";
                        this._dbStruct.UpdateByNonQuery(sqldelete);
                    }
                    this._sInsertFile = "Insert into FileList (sysfieldID,FileName,PkID) values (@sysFieldID,@FileName,@PkID)";
                    List<string> pNames = new List<string>();
                    pNames.Add("@sysFieldID");
                    pNames.Add("@FileName");
                    pNames.Add("@PkID");
                    List<object> pValues = new List<object>();
                    pValues.Add(int.Parse(fData.drField["sysFieldID"].ToString()));
                    pValues.Add(FfieldName);
                    pValues.Add(pkID);
                    List<SqlDbType> pTypes = new List<SqlDbType>();
                    pTypes.Add(SqlDbType.Int);
                    pTypes.Add(SqlDbType.NVarChar);
                    pTypes.Add(SqlDbType.NVarChar);
                    result = this._dbStruct.UpdateData(this._sInsertFile, pNames.ToArray(), pValues.ToArray(), pTypes.ToArray());

                    this._sUpdateFile = "update FileList set fData=@fData where sysFieldID=@sysFielDID and PkID=@PkID";
                    List<string> pNames1 = new List<string>();
                    pNames1.Add("@fData");
                    pNames1.Add("@sysFieldID");
                    pNames1.Add("@PkID");
                    List<object> pValues1 = new List<object>();
                    pValues1.Add(fData.fData);
                    pValues1.Add(int.Parse(fData.drField["sysFieldID"].ToString()));
                    pValues1.Add(pkID);
                    List<SqlDbType> pTypes1 = new List<SqlDbType>();
                    pTypes1.Add(SqlDbType.Image);
                    pTypes1.Add(SqlDbType.Int);
                    pTypes1.Add(SqlDbType.NVarChar);
                    result = result && this._dbStruct.UpdateData(this._sUpdateFile, pNames1.ToArray(), pValues1.ToArray(), pTypes1.ToArray());

                }
                if (isDelete)
                {
                    string pkID = drData[this.PkMaster.FieldName].ToString();
                    string sqldelete = "Delete FileList where sysfieldID=" + fData.drField["sysFieldID"].ToString() + " and PkID='" + pkID + "'";
                    this._dbStruct.UpdateByNonQuery(sqldelete);
                    this._fileData.Remove(fData);
                    i--;
                }
            }
            if (isDelete )
            {
                drData.Delete();
            }
            
            if (this._drCurrentMaster != null)
            {
                if (((this._dataType == DataType.MasterDetail) && this._drCurrentMaster.Table.Columns.Contains("ws")) && ((drData.RowState == DataRowState.Added) ))
                {
                    sql = this._sUpdateWs + " where " + pk + "='" + drData[pk].ToString() + "'";
                    updateWsCompleted = this._dbData.UpdateByNonQuery(sql);
                }
                if (((this._dataType == DataType.Single) && this._drCurrentMaster.Table.Columns.Contains("ws")) && (drData.RowState == DataRowState.Added))
                {
                    sql = "update " + this._drTable["TableName"].ToString() + " set ws='_" + Config.GetValue("sysUserID").ToString() + "_' where " + pk + "='" + drData[pk].ToString() + "'";
                    updateWsCompleted = this._dbData.UpdateByNonQuery(sql);
                    //sql = "update " + this._drTable["TableName"].ToString() + " set Grws='_" + Config.GetValue("sysUserGroupID").ToString() + "_' where " + pk + "='" + drData[pk].ToString() + "'";
                    //updateWsCompleted =updateWsCompleted && this._dbData.UpdateByNonQuery(sql);
                }
                if (((this._dataType == DataType.MasterDetail) && this._drCurrentMaster.Table.Columns.Contains("sysDBID")) && (drData.RowState == DataRowState.Added))
                {
                    sql = "update " + this._drTableMaster["TableName"].ToString() + " set sysDBID=" + Config.GetValue("sysDBID").ToString() + " where " + pk + "='" + drData[pk].ToString() + "'";
                    updateWsCompleted = this._dbData.UpdateByNonQuery(sql);
                }
                bool isApproved = true;
                //if ((this._dataType == DataType.MasterDetail) && _drCurrentMaster.RowState!=DataRowState.Deleted && _drCurrentMaster["Approved"].ToString() == "1")
                //{
                //    sql = "update " + this._drTableMaster["TableName"].ToString() + " set Approved=1 where " + pk + "='" + drData[pk].ToString() + "'";
                //    isApproved = this._dbData.UpdateByNonQuery(sql);
                //}
                //if ((this._dataType == DataType.MasterDetail) && _drCurrentMaster.RowState != DataRowState.Deleted && _drCurrentMaster["Approved"].ToString() == "-1")
                //{
                //    sql = "update " + this._drTableMaster["TableName"].ToString() + " set Approved=-1 where " + pk + "='" + drData[pk].ToString() + "'";
                //    isApproved = this._dbData.UpdateByNonQuery(sql);
                //}
                //if ((this._dataType == DataType.MasterDetail) && _drCurrentMaster.RowState != DataRowState.Deleted && _drCurrentMaster["Approved"].ToString() == "0")
                //{
                //    sql = "update " + this._drTableMaster["TableName"].ToString() + " set Approved=0 where " + pk + "='" + drData[pk].ToString() + "'";
                //    isApproved = this._dbData.UpdateByNonQuery(sql);
                //}
            }
            result = result && updateWsCompleted;
            if ((!result || (this._vUpdateImage.Count <= 0)) || isDelete)
            {
                return result;
            }
            string exsql = string.Empty;
            if (drData[pk].GetType() == typeof(int))
            {
                exsql = "";
            }
            else
            {
                exsql = "'";
            }
            if ((drData.RowState == DataRowState.Added) || (drData.RowState == DataRowState.Modified))
            {
                sql = this._sUpdateImage + " where " + pk + "=" + exsql + drData[pk].ToString() + exsql;
            }
            List<object> pImValue = new List<object>();
            List<SqlDbType> pImType = new List<SqlDbType>();
            List<string> pImName = new List<string>();
            foreach (SqlField sqlField in this._vUpdateImage)
            {
                fieldName = sqlField.FieldName;
                pImName.Add(fieldName);
                if (drData[fieldName].ToString() != string.Empty)
                {
                    pImValue.Add(drData[fieldName]);
                }
                else
                {
                    pImValue.Add(DBNull.Value);
                }
                pImType.Add(sqlField.DbType);
            }
            //  return this._dbData.UpdateData(sql, pImName.ToArray(), pImValue.ToArray(), pImType.ToArray());
            return result;
        }

        public bool UpdateData()
        {
            DateTime btime;
            DateTime etime;
            if (!this._dataChanged)
            {
                return true;
            }
            bool isError = false;
            try
            {
                btime = DateTime.Now;
                this._dbData.BeginMultiTrans();
                DataAction da = DataAction.IUD;
                if (!this._customize.BeforeUpdate(-1, this._dsData))
                {
                    this.CancelUpdate();
                    this._dbData.RollbackMultiTrans();
                    return false;
                }
                DataSet dsDataCopy = this._dsData.Copy();
                DataView dv = this._dsData.Tables[0].DefaultView;
                dv.RowStateFilter = DataViewRowState.CurrentRows | DataViewRowState.Deleted;
                foreach (DataRowView drvData in dv)
                {
                    if (drvData.Row.RowState == DataRowState.Unchanged) continue;
                    bool success = Update(drvData.Row);
                    if (success)
                    {
                        switch (drvData.Row.RowState)
                        {
                            case DataRowState.Added:
                                da = DataAction.Insert;
                                break;
                            case DataRowState.Deleted:
                                da = DataAction.Delete;
                                break;
                            case DataRowState.Modified:
                                da = DataAction.Update;
                                break;
                        }
                        int i = _dsData.Tables[0].Rows.IndexOf(drvData.Row);
                        success = success && TransferData(da, i);
                    }
                }
                dv.RowStateFilter = DataViewRowState.CurrentRows;
                 isError = this._dbData.HasErrors;
                if (!isError)
                {
                    this._dsData.AcceptChanges();
                    this._dsDataTmp = this._dsData.Copy();
                    this._customize.AfterUpdate();
                    this.DataChanged = false;
                }
                else
                {
                    this.CancelUpdate();
                }
                etime = DateTime.Now;
                isError = isError || !DoUpdating();
                if (this._dbData.HasErrors || isError)
                {
                    this._dbData.RollbackMultiTrans();
                    etime = DateTime.Now;
                }
                else
                {
                    this._dbData.EndMultiTrans();
                    etime = DateTime.Now;
                    DoAfterUpdate();
                }
                if (!isError)
                {
                    this.InsertHistory(da, dsDataCopy,btime, etime);
                }
            }
            finally
            {
                
                if (this._dbData.Connection.State != ConnectionState.Closed)
                    this._dbData.Connection.Close();
            }
            return !isError;
        }

        public abstract bool UpdateData(DataAction dataAction);
       // public abstract bool UpdateData(DataAction dataAction, DataRow drAction);
        public string UpdateSpecialCondition(string query)
        {
            if (Config.Variables.Contains("NamLamViec"))
            {
                query = query.Replace("@@NAM", Config.GetValue("NamLamViec").ToString());
            }
            foreach (DictionaryEntry o in Config.Variables)
            {
                if (Config.GetValue(o.Key.ToString()) == null) continue;
                query = query.Replace("@@" + o.Key.ToString(), Config.GetValue(o.Key.ToString()).ToString());
                query = query.Replace("@@" + o.Key.ToString().ToUpper(), Config.GetValue(o.Key.ToString()).ToString());
            }
            return query;
        }

        // Properties
        public string Condition
        {
            get
            {
                return this._condition;
            }
            set
            {
                this._condition = value;
            }
        }

        



        public DataType dataType
        {
            get
            {
                return this._dataType;
            }
            set
            {
                this._dataType = value;
            }
        }

        public Database DbData
        {
            get
            {
                return this._dbData;
            }
            set
            {
                this._dbData = value;
            }
        }

        public Database dbStruct
        {
            get
            {
                return this._dbStruct;
            }
            set
            {
                this._dbStruct = value;
            }
        }

        public DataRow DrCurrentMaster
        {
            get
            {
                return this._drCurrentMaster;
            }
            set
            {
                this._drCurrentMaster = value;
                this._drCurrIndex = this._dsData.Tables[0].Rows.IndexOf(this._drCurrentMaster);
                if (this._formulaCaculator != null)
                {
                    this._formulaCaculator.DrCurrentMaster = this._drCurrentMaster;
                    this._formulaCaculator.LstCurrentRowDt = this._lstCurRowDetail;
                }
            }
        }

        public DataRow DrTable
        {
            get
            {
                return this._drTable;
            }
            set
            {
                this._drTable = value;
            }
        }

        public DataRow DrTableMaster
        {
            get
            {
                return this._drTableMaster;
            }
            set
            {
                this._drTableMaster = value;
            }
        }

        public DataSet DsData
        {
            get
            {
                return this._dsData;
            }
            set
            {
                this._dsData = value;
                if (this._dsData != null)
                {
                    this._dsData.Tables[0].TableNewRow += new DataTableNewRowEventHandler(this.DataTable0_TableNewRow);
                    this._dsData.Tables[0].RowDeleted += new DataRowChangeEventHandler(this.DataTable0_RowDeleted);
                    this._dsData.Tables[0].RowChanged += new DataRowChangeEventHandler(this.DataTable0_RowChanged);
                    this._dsData.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(this.DataTable0_ColChanged);
                    if ((this._dataType != DataType.Single) && (this._dsData.Tables.Count > 1))
                    {
                        this._dsData.Tables[1].TableNewRow += new DataTableNewRowEventHandler(this.DataTable1_TableNewRow);
                        this._dsData.Tables[1].RowDeleted += new DataRowChangeEventHandler(this.DataTable1_RowDeleted);
                        this._dsData.Tables[1].RowChanged += new DataRowChangeEventHandler(this.DataTable1_RowChanged);
                        this._dsData.Tables[1].ColumnChanged += new DataColumnChangeEventHandler(this.DataTable1_ColChanged);
                    }
                    for (int i = 2; i < this._dsData.Tables.Count; i++)
                    {
                        this._dsData.Tables[i].TableNewRow += new DataTableNewRowEventHandler(this.DataTable_i_TableNewRow);
                        this._dsData.Tables[i].RowDeleted += new DataRowChangeEventHandler(this.DataTable_i_RowDeleted);
                        this._dsData.Tables[i].RowChanged += new DataRowChangeEventHandler(this.DataTable_i_RowChanged);
                        this._dsData.Tables[i].ColumnChanged += DataTable_i_ColumnChanged;
                        this._dsData.Tables[i].RowDeleting += CDTData_RowDeleting;
                    }
                    if (this._dataType != DataType.Report)
                    {
                        this._formulaCaculator.DsData = this._dsData;
                        //this._formulaCaculator.LstCurrentRowDt = this._lstCurRowDetail;
                    }
                }
            }
        }

        private void DataTable_i_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "MTID")
            {
                if (e.Row["MTID"] == DBNull.Value)
                {
                    var stackTrace = new StackTrace();
                    string s = "";
                    var frames = stackTrace.GetFrames();
                    foreach (var frame in frames)
                    {
                        var method = frame.GetMethod();
                        var declaringType = method.DeclaringType;
                        s += "\n" + declaringType + "." + method.Name;
                    }
                    LogFile.AppendToFile("log.txt", s);
                    MessageBox.Show("Phần mềm phát sinh lỗi. Giữ nguyên hiện trạng và nhờ hỗ trợ!");
                }
            }
            if (e.Column.ColumnName == "DTID")
            {
                DataRow drData = e.Row;
                DataTable dtStruct = this._dsStructDt.Tables[(sender as DataTable).TableName];
                int c = 0;
                foreach (DataRow drField in dtStruct.Rows)
                {
                    if (this._formulaCaculator != null && c == dtStruct.Rows.Count - 1)
                        this._formulaCaculator.Active = true;
                    if (this._drCurrentMaster != null)
                    {
                        string formulaDetail = drField["FormulaDetail"].ToString();

                        if (((formulaDetail != string.Empty) && !formulaDetail.Contains(".")) && this.DsData.Tables.Count > 1 && this.DsData.Tables[1].Columns.Contains(formulaDetail))
                        {
                            //drField["DefaultValue"]=this._drCurrentMaster[formulaDetail];
                            //this.DsData.Tables[1].Columns
                            DataRow[] dtRows = this.DsData.Tables[1].Select(_drTable["Pk"].ToString() + "='" + drData["DTID"].ToString() + "'");
                            if (dtRows.Length == 0) continue;


                            drField["DefaultValue"] = dtRows[0][formulaDetail];
                            string fieldName = drField["FieldName"].ToString();
                            drData[fieldName] = drField["DefaultValue"];
                        }
                    }


                    c++;
                }
                CurrentRowDt Crdt = _lstCurRowDetail.Find(x => x.TableName == (sender as DataTable).TableName && x.RowDetail == e.Row);
                int i = _lstCurRowDetail.IndexOf(Crdt);
                if (i > -1)
                {
                    DataRow _drTableDt0 = _drTableDt.Find(x => x["TableName"].ToString() == _lstCurRowDetail[i].TableName);
                    DataRow[] _drDetail0 = _dtDetail.Select("sysDetailID=" + _drTableDt0["sysTableID"].ToString());
                    if (_drDetail0.Length > 0)
                    {
                        if (bool.Parse(_drDetail0[0]["ChildOf"].ToString()))
                        {
                            _lstCurRowDetail[i].fkKey = e.Row["MTID"];
                        }
                        else
                        {
                            //DataRow[] prRow = e.Row.GetParentRows(_drTableDt0["TableName"].ToString() + "1");
                            //if (prRow.Length > 0)
                            e.Row["MTID"] = _drCurrentMaster[PkMaster.FieldName];
                            _lstCurRowDetail[i].fkKey = e.Row["DTID"];
                        }
                    }
                    else
                    {
                        _lstCurRowDetail[i].fkKey = e.Row["MTID"];
                    }
                }
            }
            DataTable dtStruct_i = this._dsStructDt.Tables[(sender as DataTable).TableName];
            foreach (DataRow drField in dtStruct_i.Rows)
            {
                if (e.Row[e.Column] == DBNull.Value) break;
                if (drField["refTable"] != DBNull.Value && drField["refTable"].ToString() != string.Empty)
                {
                    if (e.Column.ColumnName != drField["fieldName"].ToString()) continue;
                    string tableName = drField["refTable"].ToString();

                    string value = e.Row[e.Column].ToString();
                    string con = drField["refCriteria"].ToString();
                    string Dyncondition = drField["DynCriteria"].ToString();
                    CDTData dataRef = publicCDTData.findCDTData(tableName, con, DynCondition);
                    if (!dataRef.FullData & dataRef.dataType == DataType.Single)
                        dataRef.GetData();
                    if (dataRef == null) break; string RefPk = null; DataTable TableRef = null; DataRow[] RPk = null;
                    string quote1 = "";

                    if (e.Column.DataType == typeof(string) || e.Column.DataType == typeof(Guid)) quote1 = "'";
                    if (dataRef.dataType == DataType.Single)
                    {
                        RefPk = dataRef.DrTable["pk"].ToString();
                        TableRef = dataRef.DsData.Tables[0];
                        if (e.Column.DataType == dataRef.DsData.Tables[0].Columns[RefPk].DataType)
                        {
                            RPk = TableRef.Select(RefPk + "=" + quote1 + value + quote1);
                        }
                    }
                    else if (dataRef.dataType == DataType.MasterDetail && dataRef.DrTableMaster["tableName"].ToString() == tableName)
                    {
                        RefPk = dataRef.DrTableMaster["pk"].ToString();
                        TableRef = dataRef.DsData.Tables[0];
                        if (e.Column.DataType == dataRef.DsData.Tables[0].Columns[RefPk].DataType)
                        {
                            RPk = TableRef.Select(RefPk + "=" + quote1 + value + quote1);
                        }
                    }
                    else if (dataRef.dataType == DataType.Detail || (dataRef.dataType == DataType.MasterDetail && dataRef.DrTable["tableName"].ToString() == tableName))
                    {

                        RefPk = dataRef.DrTableMaster["pk"].ToString();
                        TableRef = dataRef.DsData.Tables[1];
                        if (e.Column.DataType == dataRef.DsData.Tables[1].Columns[RefPk].DataType)
                        {
                            RPk = TableRef.Select(RefPk + "=" + quote1 + value + quote1);
                        }
                    }

                    if (RPk == null || RPk.Length == 0) break;
                    SetValuesFromListDt(e.Row, e.Column.ColumnName, value, RPk[0], dtStruct_i);


                }
            }
        }
        void CDTData_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            e.Row.RowError = string.Empty;
            foreach (CurrentRowDt Crdt in this._lstCurRowDetail)
            {
                if ((Crdt.TableName == (sender as DataTable).TableName) && (Crdt.RowDetail == e.Row))
                {
                    return;
                }
            }
            CurrentRowDt CRdt = new CurrentRowDt();
            CRdt.TableName = (sender as DataTable).TableName;
            CRdt.RowDetail = e.Row;
            //Get drTableDetail
            DataRow _drTableDt0 = _drTableDt.Find(x => x["TableName"].ToString() == CRdt.TableName);
            DataRow[] _drDetail0 = _dtDetail.Select("sysDetailID=" + _drTableDt0["sysTableID"].ToString());
            if (_drDetail0.Length > 0)
            {
                if (bool.Parse(_drDetail0[0]["ChildOf"].ToString()))
                {
                    CRdt.fkKey = e.Row["MTID"];
                }
                else
                {
                    CRdt.fkKey = e.Row["DTID"];
                }
            }
            else
            {
                CRdt.fkKey = e.Row["MTID"];
            }
           
            this._lstCurRowDetail.Add(CRdt);
            //if (this._tableName == "DT35" && e.Row.Table.TableName == "CT35")
            //{
            //    string sql = "select count(*) from MT3A a inner join DT3A b on a.MT3AID=b.MT3AID where b.CT35ID='" + e.Row["CT35ID"].ToString() + "' and a.Approved>=0";
            //    object o = DbData.GetValue(sql);
            //    if (o != null && int.Parse(o.ToString()) > 0)
            //    {

            //        MessageBox.Show("Dòng này đã tạo phiếu xuất");
            //    }
            //}
        }



        public DataSet DsStruct
        {
            get
            {
                return this._dsStruct;
            }
            set
            {
                this._dsStruct = value;
                this.GetPkMaster();
            }
        }

        public string DynCondition
        {
            get
            {
                return this._DynCondition;
            }
            set
            {
                this._DynCondition = value;
            }
        }

        public bool FullData
        {
            get
            {
                return this.fullData;
            }
            set
            {
                this.fullData = value;
            }
        }

        public List<DataRow> LstDrCurrentDetails
        {
            get
            {
                return this._lstDrCurrentDetails;
            }
            set
            {
                this._lstDrCurrentDetails = value;
                if (this._formulaCaculator != null)
                {
                    this._formulaCaculator.LstDrCurrentDetails = this._lstDrCurrentDetails;
                }
            }
        }
        
        // Nested Types
       




        public void UpdateReportFile(DataRow dr)
        {
            try
            {
                dbStruct.BeginMultiTrans();
                string sql = "update sysFormReport set FileName=@FileName where sysFormReportID=@sysFormReportID";
                this.dbStruct.UpdateDatabyPara(sql, new string[] { "@FileName", "@sysFormReportID" }, new object[] { dr["FileName"], dr["sysFormReportID"] });
                if (this.dbStruct.HasErrors)
                {
                    dbStruct.RollbackMultiTrans();
                }
                else
                {
                    dbStruct.EndMultiTrans();
                }
            }
            catch
            {
            }
            finally
            {
                if (dbStruct.Connection.State != ConnectionState.Closed)
                    dbStruct.Connection.Close();
            }
        }
        public void updatePrintFile(DataRow drMau)
        {
            try
            {
                this.dbStruct.BeginMultiTrans();
                string sql = "update sysReportFile set FileName=@FileName where stt=@stt";
                this.dbStruct.UpdateDatabyPara(sql, new string[] { "@FileName", "@stt" }, new object[] { drMau["FileName"], drMau["stt"] });
                if (this.dbStruct.HasErrors)
                {
                    this.dbStruct.RollbackMultiTrans();
                }
                else
                {
                    this.dbStruct.EndMultiTrans();
                }


            }
            catch { }
            finally
            {
                if (dbStruct.Connection.State != ConnectionState.Closed)
                    dbStruct.Connection.Close();
            }
        }

        public void InsertPrintFile(DataRow drMau)
        {
            try
            {
                dbStruct.BeginMultiTrans();
                string sql = "insert into sysReportFile (sysTableID,RDes, RFile,RecordCount, FileName) values(@sysTableID,@RDes, @RFile,@RecordCount,@FileName)";
               
                this.dbStruct.UpdateDatabyPara(sql, new string[] { "@sysTableID", "@RDes", "@RFile", "@RecordCount", "@FileName" }, new object[] { drMau["sysTableID"], drMau["RDes"], drMau["RFile"], drMau["RecordCount"], drMau["FileName"] });
                if (dbStruct.HasErrors)
                    dbStruct.RollbackMultiTrans();
                else
                    dbStruct.EndMultiTrans();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dbStruct.Connection.State != ConnectionState.Closed)
                    dbStruct.Connection.Close();
            }
        }

        public void updateLayoutFile(DataRow dr)
        {
            if (dr["FileLayout"] != DBNull.Value)
            {
                this.dbStruct.BeginMultiTrans();
                try
                {
                    string sql = "update systable set FileLayout=@FileLayout where systableID=@systableID";
                    this.dbStruct.UpdateDatabyPara(sql, new string[] { "@FileLayout", "@systableID" }, new object[] { dr["FileLayout"], dr["systableID"] });
                    string data = dr["systableID"].ToString();
                    this.dbStruct.EndMultiTrans();
                }
                catch (Exception ex)
                {
                    this.dbStruct.RollbackMultiTrans();
                }
                finally
                {
                    if (dbStruct.Connection.State != ConnectionState.Closed)
                        dbStruct.Connection.Close();
                }
            }
            if (dr["FileLayout_E"] != DBNull.Value)
            {
                this.dbStruct.BeginMultiTrans();
                try
                {
                    string sql = "update systable set FileLayout_E=@FileLayout_E where systableID=@systableID";
                    this.dbStruct.UpdateDatabyPara(sql, new string[] { "@FileLayout_E", "@systableID" }, new object[] { dr["FileLayout_E"], dr["systableID"] });
                    this.dbStruct.EndMultiTrans();
                }
                catch (Exception ex)
                {
                    this.dbStruct.RollbackMultiTrans();
                }
                finally
                {
                    if (dbStruct.Connection.State != ConnectionState.Closed)
                        dbStruct.Connection.Close();
                }
            }
        }
        public string GenSQLDataLog()
        {
            string sql1 = "";
            if (this._sInsert == string.Empty)
            {
                this.GenSqlString();
            }
            List<SqlField> tmp = new List<SqlField>();
            tmp = this._vInsert;
            List<string> paraNames = new List<string>();
            List<object> paraValues = new List<object>();
            sql1 = "{";
            foreach (SqlField sqlField in tmp)
            {
                sql1 += "\"" + sqlField.FieldName + "\" : ";
                string q;
                if (sqlField.DbType == SqlDbType.Decimal || sqlField.DbType == SqlDbType.Bit || sqlField.DbType == SqlDbType.Int)
                {

                    if (sqlField.DbType == SqlDbType.Bit)
                    {
                        if (_drCurrentMaster[sqlField.FieldName] == DBNull.Value)
                            sql1 += "null,";
                        else if (bool.Parse(_drCurrentMaster[sqlField.FieldName].ToString()))
                            sql1 += "1,";
                        else
                            sql1 += "0,";
                    }
                    else
                    {
                        sql1 += (_drCurrentMaster[sqlField.FieldName] == DBNull.Value ? "null" : _drCurrentMaster[sqlField.FieldName].ToString().Replace(",", ".")) + ",";
                    }
                }
                else
                {
                    q = "\"";
                    sql1 += (_drCurrentMaster[sqlField.FieldName] == DBNull.Value ? "null" : q + _drCurrentMaster[sqlField.FieldName].ToString() + q) + ",";

                }

            }


            sql1 += "\n \"Detail\": \n";
            sql1 += "[";
            tmp = _vInsertDetail;
            int removelastChar = 0;
            foreach (DataRow drDetail in _lstDrCurrentDetails)
            {

                if (drDetail.RowState != DataRowState.Deleted && drDetail.RowState != DataRowState.Detached)
                {
                    removelastChar = 1;
                    sql1 += "{";
                    foreach (SqlField sqlField in tmp)
                    {
                        sql1 += "\"" + sqlField.FieldName + "\" : ";
                        string q;
                        if (sqlField.DbType == SqlDbType.Decimal || sqlField.DbType == SqlDbType.Bit || sqlField.DbType == SqlDbType.Int)
                        {

                            if (sqlField.DbType == SqlDbType.Bit)
                            {
                                if (drDetail[sqlField.FieldName] == DBNull.Value)
                                    sql1 += "null,";
                                else if (bool.Parse(drDetail[sqlField.FieldName].ToString()))
                                    sql1 += "1,";
                                else
                                    sql1 += "0,";
                            }
                            else
                            {
                                sql1 += (drDetail[sqlField.FieldName] == DBNull.Value ? "null" : drDetail[sqlField.FieldName].ToString().Replace(",", ".")) + ",";
                            }
                        }
                        else
                        {
                            q = "\"";
                            sql1 += (drDetail[sqlField.FieldName] == DBNull.Value ? "null" : q + drDetail[sqlField.FieldName].ToString() + q) + ",";
                        }
                    }
                    sql1 = sql1.Substring(0, sql1.Length - 1) + "},";
                }
            }
            sql1 = sql1.Substring(0, sql1.Length - removelastChar) + "]";
            if (this._dtDetail.Rows.Count > 0)
            {
                sql1 += "\n ,\"MultiDetail\": \n[";
                string sOj1DetailTale = "";
                for (int i = 0; i < this._dtDetail.Rows.Count; i++)
                {
                    string dtTableName = this._drTableDt[i]["tablename"].ToString();
                    sOj1DetailTale += "{\"TableName\":\"" + dtTableName + "\", \"Rows\":[";
                    string sOj1Rowdt = "";
                    int removeEnd = 0;
                    foreach (CurrentRowDt CrdrDetail in _lstCurRowDetail)
                    {
                        if (CrdrDetail.TableName != dtTableName) continue;
                        if (CrdrDetail.RowDetail.RowState == DataRowState.Deleted || CrdrDetail.RowDetail.RowState == DataRowState.Detached) continue;
                        
                            removeEnd = 1;
                        sOj1Rowdt = "{";
                        DataRow drDetail = CrdrDetail.RowDetail;
                        tmp = _vInsertDt[i];
                        foreach (SqlField sqlField in tmp)
                        {
                            sOj1Rowdt += "\"" + sqlField.FieldName + "\" : ";
                            string q;
                            if (sqlField.DbType == SqlDbType.Decimal || sqlField.DbType == SqlDbType.Bit || sqlField.DbType == SqlDbType.Int)
                            {

                                if (sqlField.DbType == SqlDbType.Bit)
                                {
                                    if (drDetail[sqlField.FieldName] == DBNull.Value)
                                        sOj1Rowdt += "null,";
                                    else if (bool.Parse(drDetail[sqlField.FieldName].ToString()))
                                        sOj1Rowdt += "1,";
                                    else
                                        sOj1Rowdt += "0,";
                                }
                                else
                                {
                                    sOj1Rowdt += (drDetail[sqlField.FieldName] == DBNull.Value ? "null" : drDetail[sqlField.FieldName].ToString().Replace(",", ".")) + ",";
                                }
                            }
                            else
                            {
                                q = "\"";
                                sOj1Rowdt += (drDetail[sqlField.FieldName] == DBNull.Value ? "null" : q + drDetail[sqlField.FieldName].ToString() + q) + ",";
                            }

                        }
                        sOj1Rowdt = sOj1Rowdt.Substring(0, sOj1Rowdt.Length - 1) + "}";
                        sOj1DetailTale += sOj1Rowdt + ",";

                    }
                    sOj1DetailTale = sOj1DetailTale.Substring(0, sOj1DetailTale.Length - removeEnd) + "]},";
                }
                sql1 += sOj1DetailTale.Substring(0, sOj1DetailTale.Length - 1) + "],";
            }
            sql1 = sql1.Substring(0, sql1.Length - (this._dtDetail.Rows.Count > 0 ? 1 : 0)) + "}";

            return sql1;
        }

    }
    public struct FileData
    {
        public byte[] fData;
        public DataRow drField;
        public bool isNew;
        public FileData(DataRow dr, byte[] _fdata, bool _isnew)
        {
            fData = _fdata;
            drField = dr;
            isNew = _isnew;
        }
    }
    public struct historyCurrent
    {
        public DateTime datetime;
        public string jsonData;
        public string tip;
        public historyCurrent(DateTime _date, string _tip, string _json)
        {
            datetime = _date;
            tip = _tip;
            jsonData = _json;
        }
    }
    public struct SqlField
    {
        public string FieldName;
        public SqlDbType DbType;

        public SqlField(string fieldName, SqlDbType dbType)
        {
            this.FieldName = fieldName;
            this.DbType = dbType;

        }
    }
}





