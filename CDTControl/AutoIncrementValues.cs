using System;
using System.Collections.Generic;
using System.Data;
using CDTDatabase;
using CDTLib;
namespace CDTControl
{
    public class AutoIncrementValues
    {
        private string _sysTableID;
        private DataTable _dtStruct;
        private DataRow _drMaster;
        Database dbData = Database.NewDataDatabase();
        public DataTable DtStruct
        {
            get { return _dtStruct; }
            set { _dtStruct = value; }
        }
        public Database _dbStruct;
        public AutoIncrementValues(string sysTableID, DataTable dtStruct, DataRow drMaster)
        {
            _sysTableID = sysTableID;
            _dtStruct = dtStruct;
            _drMaster = drMaster;
        }
        public string GetNewValueByOldvalue(string OldValue, string editMask, DateTime Ngayct)
        {
            try
            {
                int i = editMask.Length - 1;
                for (; i > 0; i--)
                    if (!Char.IsNumber(editMask, i))
                        break;

                string SufValue = "";
                string PreMask = editMask.Substring(0, i + 1);
                string SufMask = editMask.Substring(i + 1);
                int j = editMask.IndexOf("MM");
                string NewSuff = "";
                SufValue = OldValue.Substring(PreMask.Length, OldValue.Length - PreMask.Length);
                if (Char.IsNumber(SufValue, SufValue.Length - 1))
                {
                    // NewSuff = "0".PadLeft(SufValue.Length - (int.Parse(SufValue) + 1).ToString().Length, '0') + (int.Parse(SufValue) + 1).ToString();
                    NewSuff = "0000000".Substring(0, SufMask.Length - (int.Parse(SufValue) + 1).ToString().Length) + (int.Parse(SufValue) + 1).ToString();
                }
                string newvalue = PreMask.Replace("YY", Ngayct.Year.ToString().Substring(2, 2)).Replace("MM", Ngayct.ToString("MM")) + NewSuff;
                return newvalue;
            }
            catch { return ""; }
        }
        private string GetNewValue(string TableName, string FieldName, string editMask, DateTime Ngayct)
        {
            string OldValue = "";
            try
            {
                int i = editMask.Length - 1;
                for (; i > 0; i--)
                    if (!Char.IsNumber(editMask, i))
                        break;

                string SufValue = "";
                string PreMask = editMask.Substring(0, i + 1);
                string SufMask = editMask.Substring(i + 1);
                int j = editMask.IndexOf("MM");
                string sql;
                if(j>-1)sql= "select top 5 " + FieldName + " from " + TableName + " where month(NgayCT)=month('" + Ngayct.ToShortDateString() + "') and year(NgayCT)=year('" + Ngayct.ToShortDateString() + "') and " + FieldName + " like '" + PreMask.Replace("YY",Ngayct.Year.ToString().Substring(2,2)).Replace("MM",Ngayct.Month.ToString("00")) + "%'  order by " + FieldName + " desc";
                else sql = "select top 5 " + FieldName + " from " + TableName + " where year(NgayCT)=year('" + Ngayct.ToShortDateString() + "') and " + FieldName + " like '" + PreMask.Replace("YY", Ngayct.Year.ToString().Substring(2, 2)) + "%'  order by " + FieldName + " desc";

                DataTable tb = dbData.GetDataTable(sql);
                string NewSuff = "";
                if (tb.Rows.Count == 0)
                    NewSuff = "0000000".Substring(0, SufMask.Length - (int.Parse(SufMask) + 1).ToString().Length) + (int.Parse(SufMask) + 1).ToString();
                foreach (DataRow dr in tb.Rows)
                {
                    OldValue = dr[FieldName].ToString();
                    SufValue = OldValue.Substring(PreMask.Length, OldValue.Length - PreMask.Length );
                    if (Char.IsNumber(SufValue, SufValue.Length - 1))
                    {
                       // NewSuff = "0".PadLeft(SufValue.Length - (int.Parse(SufValue) + 1).ToString().Length, '0') + (int.Parse(SufValue) + 1).ToString();
                        NewSuff = "0000000".Substring(0, SufMask.Length - (int.Parse(SufValue) + 1).ToString().Length) + (int.Parse(SufValue) + 1).ToString();
                        break;
                    }
                }
                string newvalue = PreMask.Replace("YY", Ngayct.Year.ToString().Substring(2, 2)).Replace("MM", Ngayct.ToString("MM")) + NewSuff;
                return newvalue;
            }
            catch
            {
                return editMask;
            }
            
        }
        private string GetNewValue(string TableName, string FieldName, string editMask, DateTime Ngayct, string MaCN)
        {
            string OldValue = "";
            try
            {
                int i = editMask.Length - 1;
                for (; i > 0; i--)
                    if (!Char.IsNumber(editMask, i))
                        break;

                string SufValue = "";
                string PreMask = editMask.Substring(0, i + 1);
                string SufMask = editMask.Substring(i + 1);
                int j = editMask.IndexOf("MM");
                int k = PreMask.IndexOf("MaCN");
                PreMask = PreMask.Replace("MaCN", Config.GetValue("MaCN").ToString());
                string sql;
                if(j>-1) sql= "select top 5 " + FieldName + " from " + TableName + " where month(NgayCT)=month('" + Ngayct.ToShortDateString() + "') and year(NgayCT)=year('" + Ngayct.ToShortDateString() + "') and " + FieldName + " like '" + PreMask.Replace("YY", Ngayct.Year.ToString().Substring(2, 2)).Replace("MM", Ngayct.Month.ToString("00")) + "%' and MaCN='" + Config.GetValue("MaCN").ToString() + "' order by " + FieldName + " desc";
                else sql = "select top 5 " + FieldName + " from " + TableName + " where  year(NgayCT)=year('" + Ngayct.ToShortDateString() + "') and " + FieldName + " like '" + PreMask.Replace("YY", Ngayct.Year.ToString().Substring(2, 2)) + "%' and MaCN='" + Config.GetValue("MaCN").ToString() + "' order by " + FieldName + " desc";
                DataTable tb = dbData.GetDataTable(sql);
                string NewSuff = "";
                if (tb.Rows.Count == 0)
                    NewSuff = "0000000".Substring(0, SufMask.Length - (int.Parse(SufMask) + 1).ToString().Length) + (int.Parse(SufMask) + 1).ToString();
                foreach (DataRow dr in tb.Rows)
                {
                    OldValue = dr[FieldName].ToString();
                    SufValue = OldValue.Substring(PreMask.Length, OldValue.Length - PreMask.Length);
                    if (Char.IsNumber(SufValue, SufValue.Length - 1))
                    {
                        NewSuff = "0000000".Substring(0, SufMask.Length - (int.Parse(SufValue) + 1).ToString().Length) + (int.Parse(SufValue) + 1).ToString();
                        break;
                    }
                }
                string newvalue = PreMask.Replace("YY", Ngayct.Year.ToString().Substring(2, 2)).Replace("MM", Ngayct.ToString("MM")) + NewSuff;
                return newvalue;
            }
            catch
            {
                return editMask;
            }

            //int k = PreValue.IndexOf("MaCN");
            //if (k >= 0)
            //{
            //    PreValue = PreValue.Replace("MaCN", Config.GetValue("MaCN").ToString());
            //}
        }
        private string GetNewValue(string TableName, string FieldName, string editMask, string MaCN)
        {
            string OldValue = "";
            try
            {
                int i = editMask.Length - 1;
                for (; i > 0; i--)
                    if (!Char.IsNumber(editMask, i))
                        break;

                string SufValue = "";

                string PreMask = editMask.Substring(0, i + 1);
                string SufMask = editMask.Substring(i + 1);

                int k = PreMask.IndexOf("MaCN");
                PreMask = PreMask.Replace("MaCN", Config.GetValue("MaCN").ToString());
                string sql = "select top 5 " + FieldName + " from " + TableName + " where " + FieldName + " like '" + PreMask + "%' and MaCN='" + Config.GetValue("MaCN").ToString() + "' order by " + FieldName + " desc";
                DataTable tb = dbData.GetDataTable(sql);
                string NewSuff = "";
                if (tb.Rows.Count == 0)
                    NewSuff = "0000000".Substring(0, SufMask.Length - (int.Parse(SufMask) + 1).ToString().Length) + (int.Parse(SufMask) + 1).ToString();
                foreach (DataRow dr in tb.Rows)
                {
                    OldValue = dr[FieldName].ToString();
                    SufValue = OldValue.Substring(PreMask.Length, OldValue.Length - PreMask.Length);
                    if (Char.IsNumber(SufValue, SufValue.Length - 1))
                    {
                        NewSuff = "0000000".Substring(0, SufMask.Length - (int.Parse(SufValue) + 1).ToString().Length) + (int.Parse(SufValue) + 1).ToString();
                        break;
                    }
                }
                string newvalue = PreMask + NewSuff;
                return newvalue;
            }
            catch
            {
                return editMask;
            }

        }
        private string GetNewValue(string TableName, string FieldName, string editMask, string valueCondition,bool isCondition)
        {
            string OldValue = "";
            try
            {
                int i = editMask.Length - 1;
                for (; i > -1; i--)
                    if (!Char.IsNumber(editMask, i))
                        break;

                string SufValue = "";

                string PreMask = editMask.Substring(0, i + 1);
                string SufMask = editMask.Substring(i + 1);
                string sql = "select top 5 " + FieldName + " from " + TableName + " where " + FieldName + " like '" + PreMask + "%'  and " + valueCondition +" order by " + FieldName + " desc";
                DataTable tb = dbData.GetDataTable(sql);
                string NewSuff = "";
                if (tb.Rows.Count == 0)
                    NewSuff = "0000000".Substring(0, SufMask.Length - (int.Parse(SufMask) + 1).ToString().Length) + (int.Parse(SufMask) + 1).ToString();
                foreach (DataRow dr in tb.Rows)
                {
                    OldValue = dr[FieldName].ToString();
                    if (PreMask != string.Empty)
                        SufValue = OldValue.Replace(PreMask, "");

                    else SufValue = OldValue;
                    if (Char.IsNumber(SufValue, SufValue.Length - 1))
                    {
                        NewSuff = "0000000".Substring(0, SufMask.Length - (int.Parse(SufValue) + 1).ToString().Length) + (int.Parse(SufValue) + 1).ToString();
                        break;
                    }
                }
                return PreMask + NewSuff;
            }
            catch (Exception ex)
            {
                return editMask;
            }

        }
        private  string GetNewValue(string TableName,string FieldName,string editMask)
        {
            string OldValue = "";
            try
            {
                int i = editMask.Length - 1;
                for (; i > -1; i--)
                    if (!Char.IsNumber(editMask, i))
                        break;
               
                string SufValue = "";

                string PreMask = editMask.Substring(0, i+1 );
                string SufMask = editMask.Substring(i+1 );
                string sql = "select top 5 " + FieldName + " from " + TableName + " where " + FieldName +" like '" +PreMask + "%'  order by " + FieldName + " desc";
                DataTable tb = dbData.GetDataTable(sql);
                string NewSuff = "";
                if (tb.Rows.Count == 0)
                    NewSuff = "00000000000000".Substring(0, SufMask.Length - (int.Parse(SufMask) + 1).ToString().Length) + (int.Parse(SufMask) + 1).ToString();
                foreach (DataRow dr in tb.Rows)
                {
                    OldValue = dr[FieldName].ToString();
                    if(PreMask !=string.Empty)
                    SufValue = OldValue.Replace(PreMask, "");

                    else SufValue = OldValue;
                    if (Char.IsNumber(SufValue, SufValue.Length - 1))
                    {
                        NewSuff = "0000000000000".Substring(0, SufMask.Length - (int.Parse(SufValue) + 1).ToString().Length) + (int.Parse(SufValue) + 1).ToString();
                        break;
                    }
                }
                    return PreMask + NewSuff;
            }
            catch (Exception ex)
            {
                return editMask;
            }
        }

        public void MakeNewStruct()
        {
            DateTime ngayct;
            ngayct = DateTime.Parse(Config.GetValue("NgayHethong").ToString());
            for (int i = 0; i < _dtStruct.Rows.Count; i++)
            {
                DataRow drField = _dtStruct.Rows[i];
                string FieldName = drField["FieldName"].ToString();
                string TableName = _drMaster.Table.TableName;
                int pType = Int32.Parse(drField["Type"].ToString());
               // string sql = "select a.EditMask from sysfield a  where   a.sysfieldID=" + drField["sysFieldID"].ToString();
               // DataTable tb = _dbStruct.GetDataTable(sql);
                string editMask = "";
                editMask = drField["EditMask"].ToString();

                    string newValue = "";
                    if ((pType != 0 && pType != 2) || editMask == string.Empty) continue;

                if (editMask.Contains("MaCN") && Config.GetValue("MaCN").ToString() != string.Empty)
                {
                    if (editMask.Contains("MM") || editMask.Contains("YY"))
                    {
                        //ngayct = DateTime.Now;
                        if (_drMaster.Table.Columns.Contains("NgayCT") && _drMaster["NgayCT"] != DBNull.Value)
                            ngayct = DateTime.Parse(_drMaster["NgayCT"].ToString());
                        newValue = GetNewValue(TableName, FieldName, editMask, ngayct, Config.GetValue("MaCN").ToString());
                    }
                    else
                    {
                        newValue = GetNewValue(TableName, FieldName, editMask, Config.GetValue("MaCN").ToString());
                    }
                }
                else
                {
                    if (editMask.Contains("MM") || editMask.Contains("YY"))
                    {
                        //ngayct = DateTime.Now;
                        if (_drMaster.Table.Columns.Contains("NgayCT") && _drMaster["NgayCT"] != DBNull.Value)
                            ngayct = DateTime.Parse(_drMaster["NgayCT"].ToString());
                        newValue = GetNewValue(TableName, FieldName, editMask, ngayct);
                    }
                    else
                    {
                        string formula = drField["Formula"] == DBNull.Value ? "" : drField["Formula"].ToString();
                        if (formula != string.Empty & formula.Contains("@"))
                        {
                            formula = formula.Replace("@", "");
                            DataRow[] ldr = _dtStruct.Select("FieldName='" + formula + "'");
                            if (ldr.Length > 0)
                            {
                                string valuecondition = ldr[0]["Defaultvalue"].ToString();
                                if (_drMaster[formula] != DBNull.Value)
                                    valuecondition = _drMaster[formula].ToString();
                                string where = formula + "='" + valuecondition + "'";
                                newValue = GetNewValue(TableName, FieldName, editMask, where,true);
                            }
                            else
                            {
                                newValue = GetNewValue(TableName, FieldName, editMask);
                            }
                        }
                        else
                        {
                            newValue = GetNewValue(TableName, FieldName, editMask);
                        }
                    }
                }

                if (newValue != string.Empty)
                {
                    drField["DefaultValue"] = newValue;
                }
               // }

            }
        }

        private void DataToStruct(DataRow drData)
        {
            foreach (DataRow drField in _dtStruct.Rows)
            {
                string fieldName = drField["FieldName"].ToString();
                int pType = Int32.Parse(drField["Type"].ToString());
                string editMask = drField["EditMask"].ToString();
                if (pType != 0 && pType != 2) continue;
                if ( editMask != string.Empty)                {
                    drField["CurrValue"] = drData[fieldName].ToString();
                    //drField["EditMask"] = drData[fieldName].ToString();
                }
            }
        }

        public void UpdateNewStruct(DataRow drData)
        {
            DataToStruct(drData);
            Database dbStruct = Database.NewStructDatabase();
            foreach (DataRow drstruct in _dtStruct.Rows)
            {
                if (drstruct["CurrValue"] == DBNull.Value || drstruct["CurrValue"].ToString() == string.Empty) continue;
                int pType = Int32.Parse(drstruct["Type"].ToString());
                if (pType != 0 && pType != 2) continue;
                string query = "update sysField set EditMask = '" + drstruct["CurrValue"].ToString() + "' where  len(EditMask)<>" + drstruct["CurrValue"].ToString().Length + " and sysFieldID = " + drstruct["sysFieldID"].ToString();
                //dbStruct.UpdateByNonQuery(query);
                if (drstruct["editMask"].ToString().Contains("MaCN"))
                {
                    query = "select count(*) from sysCurValue where sysDBID=" + Config.GetValue("sysDBID").ToString() + " and sysFieldID = " + drstruct["sysFieldID"].ToString() + " and MaCN='" + Config.GetValue("MaCN").ToString() + "'";
                }
                else
                {
                    query = "select count(*) from sysCurValue where sysDBID=" + Config.GetValue("sysDBID").ToString() + " and sysFieldID = " + drstruct["sysFieldID"].ToString();
                }
                
                object o = dbStruct.GetValue(query);
                if (double.Parse(o.ToString()) == 0)
                {
                    if (drstruct["editMask"].ToString().Contains("MaCN"))
                    {
                        query = "insert into sysCurValue(sysFieldID, sysDBID, CurrValue, MaCN) values (" + drstruct["sysFieldID"].ToString() + "," + Config.GetValue("sysDBID").ToString() + ",'" + drstruct["CurrValue"].ToString() + "','" + Config.GetValue("MaCN").ToString() + "')";
                    }
                    else
                    {
                        query = "insert into sysCurValue(sysFieldID, sysDBID, CurrValue) values (" + drstruct["sysFieldID"].ToString() + "," + Config.GetValue("sysDBID").ToString() + ",'" + drstruct["CurrValue"].ToString() + "')";
                    }
                    dbStruct.UpdateByNonQuery(query);
                }
                else
                {
                    if (drstruct["editMask"].ToString().Contains("MaCN"))
                    {
                        query = "update  sysCurValue set CurrValue = '" + drstruct["CurrValue"].ToString() + "' where sysDBID=" + Config.GetValue("sysDBID").ToString() + " and sysFieldID = " + drstruct["sysFieldID"].ToString() + " and MaCN='" + Config.GetValue("MaCN").ToString() + "'";
                    }
                    else
                    {
                        query = "update  sysCurValue set CurrValue = '" + drstruct["CurrValue"].ToString() + "' where sysDBID=" + Config.GetValue("sysDBID").ToString() + " and sysFieldID = " + drstruct["sysFieldID"].ToString();
                    }
                    
                    dbStruct.UpdateByNonQuery(query);
                }
               
            }
           
        }
    }
}
