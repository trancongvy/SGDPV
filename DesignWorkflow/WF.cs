using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
namespace Designflow
{
    public class WF
    {
        Database _dataSt = Database.NewStructDatabase();
        Database _data = Database.NewDataDatabase();
        public List<Task> lTask = new List<Task>();
        public List<Action> lAction = new List<Action>();
        public List<Task> lDeleteTask = new List<Task>();
        public List<Action> lDeleteAction = new List<Action>();
        public DataRow drWF;
        public string tableID;
        public string WFName;
        public WF(string TableID)
        {
            GetWF(TableID);
            tableID = TableID;
        }
        public WF()
        {
        }
        public DataTable getSecurity(Guid id)
        {
            string sql = "select * from sysUserTask where TaskID='" + id.ToString() + "'";
            DataTable tb= _dataSt.GetDataTable(sql);
            tb.Columns["TaskID"].DefaultValue = id;
            return tb;
        }
        public DataTable getSecurityGr(Guid id)
        {
            string sql = "select * from sysUserGrTask where TaskID='" + id.ToString() + "'";
            DataTable tb = _dataSt.GetDataTable(sql);
            tb.Columns["TaskID"].DefaultValue = id;
            return tb;
        }
        public DataTable getSecurityAction(Guid id)
        {
            string sql = "select * from sysUserAction where ActionID='" + id.ToString() + "'";
            DataTable tb = _dataSt.GetDataTable(sql);
            tb.Columns["ActionID"].DefaultValue = id;
            return tb;
        }
        public DataTable getSecurityGrAction(Guid id)
        {
            string sql = "select * from sysUserGrAction where ActionID='" + id.ToString() + "'";
            DataTable tb = _dataSt.GetDataTable(sql);
            tb.Columns["ActionID"].DefaultValue = id;
            return tb;
        }
        public void GetWF(string TableID)
        {
            string sql = "select * from sysWF where systableid=" + TableID;
            DataTable wf = _dataSt.GetDataTable(sql);
           
            if (wf.Rows.Count > 0)
            {
                WFName = wf.Rows[0]["WFName"].ToString();
                drWF = wf.Rows[0];
                sql = "select * from sysTask where WFID='" + drWF["ID"].ToString() + "'";
                DataTable tbTask = _dataSt.GetDataTable(sql);
                lTask.Clear();
                foreach (DataRow dr in tbTask.Rows)
                {
                    Task ntask = new Task(dr);
                    ntask.tbSecu = getSecurity(ntask.id);
                    ntask.tbSecuGroup = getSecurityGr(ntask.id);
                    lTask.Add(ntask);
                }
                sql = "select * from sysAction where WFID='" + drWF["ID"].ToString() + "'";
                DataTable tbAction = _dataSt.GetDataTable(sql);
                lAction.Clear();
                foreach (DataRow dr in tbAction.Rows)
                {
                    Action A = new Action(dr);
                    A.tbSecu = getSecurityAction(A.Id);
                    A.tbSecuGroup = getSecurityGrAction(A.Id);
                    lAction.Add(A);
                    foreach (Task task in lTask)
                    {
                        if (task.id.ToString() == dr["BTID"].ToString()) A.BT = task;
                        if (task.id.ToString() == dr["ETID"].ToString()) A.ET = task;
                    }
                    if (A.BT.P.X == A.P[0].X) A.BPName = ConnectPoint.LeftPoint;
                    if (A.BT.P.X + A.BT.w == A.P[0].X) A.BPName = ConnectPoint.RightPoint;
                    if (A.BT.P.Y == A.P[0].Y) A.BPName = ConnectPoint.TopPoint;
                    if (A.BT.P.Y + A.BT.h == A.P[0].Y) A.BPName = ConnectPoint.BotPoint;

                    if (A.ET.P.X == A.P[A.P.Count - 1].X) A.EPName = ConnectPoint.LeftPoint;
                    if (A.ET.P.X + A.ET.w == A.P[A.P.Count - 1].X) A.EPName = ConnectPoint.RightPoint;
                    if (A.ET.P.Y == A.P[A.P.Count - 1].Y) A.EPName = ConnectPoint.TopPoint;
                    if (A.ET.P.Y + A.ET.h == A.P[A.P.Count - 1].Y) A.EPName = ConnectPoint.BotPoint;
                }
            }
        }
        internal bool UpdataData()
        {
            _dataSt.BeginMultiTrans();
            string sql;
            if (drWF != null)
            {
                foreach (Task t in lTask)
                {
                    sql = "delete sysTask where ID='" + t.id.ToString() + "'";
                    _dataSt.UpdateByNonQuery(sql);
                }
                foreach (Task t in lDeleteTask)
                {
                    sql = "delete sysTask where ID='" + t.id.ToString() + "'";
                    _dataSt.UpdateByNonQuery(sql);
                }
                foreach (Action A in lAction)
                {
                    sql = "delete sysAction where ID='" + A.Id.ToString() + "'";
                    _dataSt.UpdateByNonQuery(sql);
                }
                foreach (Action A in lDeleteAction)
                {
                    sql = "delete sysAction where ID='" + A.Id.ToString() + "'";
                    _dataSt.UpdateByNonQuery(sql);
                }
                sql = "delete sysWF where ID='" + drWF["ID"].ToString() + "'";
                _dataSt.UpdateByNonQuery(sql);
            }
             string ID ;
             if (drWF == null)
             {
                 ID = Guid.NewGuid().ToString();

             }
             else
             {
                 ID = drWF["ID"].ToString();
             }
            sql = "insert into sysWF (ID, sysTableID, WFName) values('" + ID.ToString() + "'," + tableID + ",N'" + WFName + "')";
            _dataSt.UpdateByNonQuery(sql);
            if (!_dataSt.HasErrors)
            {
                foreach (Task t in lTask)
                {
                    //sql = "select * from systask";
                   // DataTable tbtmp = _dataSt.GetDataTable(sql);
                    string isBegin = t.isBegin ? "1" : "0";
                    string isEnd = t.isEnd ? "1" : "0";
                    string isCancel = t.isCancel ? "1" : "0";
                    sql = "insert into sysTask(ID,TaskLabel,Point, width, height, WFID, isBegin,isEnd, isCancel, ApprovedStt) values ('" + t.id.ToString() + "',N'" + t.Label + "','" + t.GetPString() + "'," + t.w + "," + t.h + ",'" + ID.ToString() + "'," + isBegin + "," + isEnd + "," + isCancel + "," + t.ApprovedStt + ")";
                    _dataSt.UpdateByNonQuery(sql);
                    sql = "update sysTask set icon=@icon where ID='" + t.id.ToString() + "'";
                    if (t.Icon != null)
                    {
                        byte[] im = new byte[0];
                        using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                        {
                            t.Icon.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            stream.Close();

                            im = stream.ToArray();
                        }

                        _dataSt.UpdateData(sql, new string[] { "@icon" }, new object[] { im }, new SqlDbType[] { SqlDbType.Image });
                    }
                    if (_dataSt.HasErrors) break;
                    if(t.tbSecu!=null)
                    {
                        sql = "delete sysUserTask where TaskID='" + t.id + "'";
                        _dataSt.UpdateByNonQuery(sql);
                        foreach (DataRow drSecu in t.tbSecu.Rows)
                        {
                            if (drSecu["GetMail"] == DBNull.Value) drSecu["GetMail"] = 0;
                            string Getmail = Getmail = (bool.Parse(drSecu["GetMail"].ToString())) ? "1" : "0";
                            sql = "insert into  sysUserTask (sysUserID, TaskID, CView, CEdit, CDelete, getMail) values (";
                            sql += drSecu["sysUserID"].ToString() + ",'" + drSecu["TaskID"].ToString() + "','" + drSecu["CView"].ToString().Replace("'", "''") +
                                "','" + drSecu["CEdit"].ToString().Replace("'", "''") + "','" + drSecu["CDelete"].ToString().Replace("'", "''") + "'," + Getmail + ")";
                            _dataSt.UpdateByNonQuery(sql);
                        }
                    }
                    if (t.tbSecuGroup != null)
                    {
                        sql = "delete sysUserGrTask where TaskID='" + t.id + "'";
                        _dataSt.UpdateByNonQuery(sql);
                        foreach (DataRow drSecu in t.tbSecuGroup.Rows)
                        {
                            if (drSecu["GetMail"] == DBNull.Value) drSecu["GetMail"] = 0;
                            string Getmail = Getmail = (bool.Parse(drSecu["GetMail"].ToString())) ? "1" : "0";
                            sql = "insert into  sysUserGrTask (sysUserGroupID, TaskID, CView, CEdit, CDelete, GetMail) values (";
                            sql += drSecu["sysUserGroupID"].ToString() + ",'" + drSecu["TaskID"].ToString() + "','" + drSecu["CView"].ToString().Replace("'", "''") +
                                "','" + drSecu["CEdit"].ToString().Replace("'", "''") + "','" + drSecu["CDelete"].ToString().Replace("'", "''") + "'," + Getmail + ")";
                            _dataSt.UpdateByNonQuery(sql);
                        }
                    }
                    if (_dataSt.HasErrors) break;
                }
            }
            if (!_dataSt.HasErrors)
            {
                foreach (Action A in lAction)
                {
                    string Autodo = A.AutoDo ? "1" : "0";
                    string isRefresh = A.isRefresh ? "1" : "0";
                    string DoConfigData = A.DoconfigData ? "1" : "0";
                    if (A.Command == null) A.Command = "";
                    if (A.AfterUpdateCommand == null) A.AfterUpdateCommand = "";
                    if (A.Condition== null) A.Condition = "";
                    if (A.ShowCond == null) A.ShowCond = "";
                    if (A.Confirm == null) A.Confirm = "";
                    if (A.Message == null) A.Message = "";
                    string SendMail = A.SendMail ? "1" : "0";
                    string SendMailKH = A.SendMailKH ? "1" : "0";
                    if (A.MailContent == null) A.MailContent = "";
                    if (A.MailContentKH == null) A.MailContentKH = "";

                    sql = "insert into sysAction(ID,systableID, AutoDo, condition,ShowCond, Command, commandName,Confirm, Message,BTId, ETId, P,WFID," +
                        "isRefresh,AfterUpdate, SendMail, SendMailKH, MailContent, MailContentKH,DoConfigData ) Values ('";
                    sql += A.Id.ToString() + "'," + tableID + "," + Autodo + ",'" + A.Condition.Replace("'", "''") + "','" + A.ShowCond.Replace("'", "''") + "',N'" + A.Command.Replace("'", "''");
                    sql += "',N'" + A.Name + "',N'" + A.Confirm + "',N'" + A.Message +"','"   + A.BT.id.ToString() + "','" + A.ET.id.ToString() + "','" + A.GetPString() + "','" + ID +
                        "'," + isRefresh + ",N'" + A.AfterUpdateCommand.Replace("'","''") + "'," + SendMail + "," +SendMailKH + ",N'" + A.MailContent.Replace("'", "''") + "',N'" + A.MailContentKH.Replace("'", "''") + "'," + DoConfigData +")";
                    _dataSt.UpdateByNonQuery(sql);
                    sql = "update sysAction set icon=@icon where ID='" + A.Id.ToString() + "'";

                    if (A.Icon != null)
                    {
                        byte[] im = new byte[0];
                        using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                        {
                            A.Icon.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            stream.Close();
                            im = stream.ToArray();
                        }

                        _dataSt.UpdateData(sql, new string[] { "@icon" }, new object[] { im }, new SqlDbType[] { SqlDbType.Image });
                    }
                    if (_dataSt.HasErrors) break;
                    if(A.tbSecu!=null)
                    {
                        sql = "delete sysUserAction where ActionID='" + A.Id + "'";
                        _dataSt.UpdateByNonQuery(sql);
                        foreach (DataRow drSecu in A.tbSecu.Rows)
                        {
                            if (drSecu["GetMail"] == DBNull.Value) drSecu["GetMail"] = 0;
                            string Getmail = Getmail = (bool.Parse(drSecu["GetMail"].ToString())) ? "1" : "0";
                            sql = "insert into  sysUserAction (sysUserID, ActionID, CAllow, getMail) values (";
                            sql += drSecu["sysUserID"].ToString() + ",'" + drSecu["ActionID"].ToString() + "','" + drSecu["CAllow"].ToString().Replace("'", "''") +
                                 "'," + Getmail + ")";
                            _dataSt.UpdateByNonQuery(sql);
                        }
                    }
                    if (A.tbSecuGroup != null)
                    {
                        sql = "delete sysUserGrAction where ActionID='" + A.Id + "'";
                        _dataSt.UpdateByNonQuery(sql);
                        foreach (DataRow drSecu in A.tbSecuGroup.Rows)
                        {
                            if (drSecu["GetMail"] == DBNull.Value) drSecu["GetMail"] = 0;
                            string Getmail = Getmail = (bool.Parse(drSecu["GetMail"].ToString())) ? "1" : "0";
                            sql = "insert into  sysUserGrAction (sysUserGroupID, ActionID, CAllow, GetMail) values (";
                            sql += drSecu["sysUserGroupID"].ToString() + ",'" + drSecu["ActionID"].ToString() + "','" + drSecu["CAllow"].ToString().Replace("'", "''") +
                                "'," + Getmail + ")";
                            _dataSt.UpdateByNonQuery(sql);
                        }
                    }
                    if (_dataSt.HasErrors) break;
                }
            }
            if (_dataSt.HasErrors)
            {
                _dataSt.RollbackMultiTrans();
                return false;
            }
            else
            {
                _dataSt.EndMultiTrans();
                return true;
            }

        }
    }
}
