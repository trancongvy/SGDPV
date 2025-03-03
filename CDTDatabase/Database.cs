using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CDTLib;
using ErrorManager;
using System.Windows.Forms;

namespace CDTDatabase
{
    public class Database
    {
        SqlConnection connection;
        public SqlTransaction strMain = null;
       public bool multiStatement = false;
        bool hasErrors = false;
        public string MasterPk = string.Empty;
        public string QueryMaster = string.Empty;
        public string DetailPk = string.Empty;
        public string QueryDetail = string.Empty;
        private const int _timeOut = 120;
        public bool HasErrors
        {
            get { return hasErrors; }
            set { hasErrors = value;
            }
        }

        

        public SqlConnection Connection
        {
            get { return connection; }
        }

        public void BeginMultiTrans()
        {
            OpenConnection();
            multiStatement = true;
            hasErrors = false;
        }

        public void EndMultiTrans()
        {
            if (hasErrors == false && strMain.Connection!=null )
                strMain.Commit();
            CloseConnection();
            multiStatement = false;
            hasErrors = false;
        }

        public void RollbackMultiTrans()
        {
            strMain.Rollback();
            CloseConnection();
            multiStatement = false;
            hasErrors = false;
        }

        #region Các hàm khởi tạo

        public static Database NewStructDatabase()
        {
            string StructConn = Config.GetValue("StructConnection").ToString();
            return (new Database(StructConn));
        }

        public static Database NewDataDatabase()
        {
            //Config cf = Config.Instance();
            string DataConn = Config.GetValue("DataConnection").ToString();
            return (new Database(DataConn));
        }

        public static Database NewCustomDatabase(string strConnection)
        {
            return (new Database(strConnection));
        }
        private Database(string strConnection)
        {
            connection = new SqlConnection(strConnection);
            
        }
        #endregion

        #region Các chức năng lấy số liệu
        public DataSet GetDataSet(string queryString)
        {
            if (!multiStatement)
                if (!OpenConnection())
                    return null;
            QueryMaster = queryString;
            try
            {
                DataSet dsMain = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(QueryMaster, connection);
                sda.SelectCommand.Transaction = strMain;
                sda.SelectCommand.CommandTimeout = _timeOut;
                sda.Fill(dsMain);

                if (!multiStatement)
                    strMain.Commit();
                LogFile.LogTruyXuatDL(queryString);
                return dsMain;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    strMain.Rollback();
                hasErrors = true;LogFile.SqlError(queryString, se);
                return null;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }

        public DataSet GetDataSetDetail(string queryMaster, string queryDetail)
        {
            if (!multiStatement)
                if (!OpenConnection())
                    return null;

            QueryMaster = queryMaster;
            QueryDetail = queryDetail;
            try
            {
                DataSet dsMain = new DataSet();
                SqlDataAdapter sdaDetail = new SqlDataAdapter(QueryDetail, connection);
                sdaDetail.SelectCommand.Transaction = strMain;
                sdaDetail.SelectCommand.CommandTimeout = _timeOut;
                SqlCommandBuilder scbDetail = new SqlCommandBuilder(sdaDetail);
                dsMain.Relations.Clear();
                dsMain.Tables.Clear();
                sdaDetail.Fill(dsMain);
                //sdaDetail.RowUpdated += new SqlRowUpdatedEventHandler(sdaDetail_RowUpdated);

                SqlDataAdapter sdaMaster = new SqlDataAdapter(QueryMaster, connection);
                sdaMaster.SelectCommand.Transaction = strMain;
                sdaMaster.SelectCommand.CommandTimeout = _timeOut;
                SqlCommandBuilder scbMaster = new SqlCommandBuilder(sdaMaster);
                DataTable dtTmp = new DataTable();
                sdaMaster.Fill(dtTmp);
                //sdaMaster.RowUpdated += new SqlRowUpdatedEventHandler(sdaMaster_RowUpdated);
                dsMain.Tables.Add(dtTmp);

                if (!multiStatement)
                    strMain.Commit();
                LogFile.LogTruyXuatDL(queryMaster + "/n" + queryDetail);
                return dsMain;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    strMain.Rollback();
                hasErrors = true;LogFile.SqlError(queryMaster, se);
                
                return null;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }

        public DataSet GetDataSetMasterDetail(string queryMaster, string queryDetail)
        {
            if (!multiStatement)
                if (!OpenConnection())
                    return null;

            QueryMaster = queryMaster;
            QueryDetail = queryDetail;
            try
            {
                DataSet dsMain = new DataSet();
                SqlDataAdapter sdaMaster = new SqlDataAdapter(QueryMaster, connection);
                sdaMaster.SelectCommand.Transaction = strMain;
                sdaMaster.SelectCommand.CommandTimeout = _timeOut;
                SqlCommandBuilder scbMaster = new SqlCommandBuilder(sdaMaster);
                dsMain.Relations.Clear();
                dsMain.Tables.Clear();
                sdaMaster.Fill(dsMain);
                //sdaMaster.RowUpdated += new SqlRowUpdatedEventHandler(sdaMaster_RowUpdated);

                SqlDataAdapter sdaDetail = new SqlDataAdapter(QueryDetail, connection);
                sdaDetail.SelectCommand.Transaction = strMain;
                sdaDetail.SelectCommand.CommandTimeout = _timeOut;
                SqlCommandBuilder scbDetail = new SqlCommandBuilder(sdaDetail);
                DataTable dtTmp = new DataTable();
                sdaDetail.Fill(dtTmp);
                //sdaDetail.RowUpdated += new SqlRowUpdatedEventHandler(sdaDetail_RowUpdated);
                dsMain.Tables.Add(dtTmp);

                if (!multiStatement)
                    strMain.Commit();
                LogFile.LogTruyXuatDL(queryMaster + "/n" + queryDetail);
                return dsMain;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    strMain.Rollback();
                hasErrors = true;
               LogFile.SqlError(QueryMaster, se);
                
                return null;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }
        #endregion

        #region Các chức năng cập nhật số liệu
        /// <summary>
        /// Dung cho Type1: cap nhat so lieu mot lan
        /// </summary>
        public bool UpdateDataSet(DataSet dsMain)
        {
            if (!multiStatement)
                if (!OpenConnection())
                    return false;
            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(QueryMaster, connection);
                sda.SelectCommand.Transaction = strMain;
                SqlCommandBuilder scb = new SqlCommandBuilder(sda);
                //sda.RowUpdated += new SqlRowUpdatedEventHandler(sdaMaster_RowUpdated);
                if (dsMain == null) return true;
                sda.Update(dsMain.Tables[0]);

                if (!multiStatement)
                    strMain.Commit();
                LogFile.LogTruyXuatDL(QueryMaster + "/n" + QueryDetail);
                return true;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    strMain.Rollback();
                hasErrors = true;
                LogFile.SqlError(QueryMaster, se);
                return false;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }

        #endregion

        #region Các chức năng hỗ trợ cho class
        private bool OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                if (strMain == null||strMain.Connection==null)
                    strMain = connection.BeginTransaction();
                return true;
            }
            catch (SqlException se)
            {
                LogFile.SqlError(connection.ConnectionString, se);
                return false;
            }
        }
        private bool OpenConnectionNotrans()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
               
                return true;
            }
            catch (SqlException se)
            {
                LogFile.SqlError(connection.ConnectionString, se);
                return false;
            }
        }
        private bool OpenConnection(IsolationLevel Iso)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                if (strMain == null || strMain.Connection == null)
                    strMain = connection.BeginTransaction(Iso);
                return true;
            }
            catch (SqlException se)
            {
                LogFile.SqlError(connection.ConnectionString, se);
                return false;
            }
        }
        private void CloseConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                if (strMain != null)
                    strMain = null;
            }
            catch (SqlException se)
            {
                LogFile.SqlError(connection.ConnectionString, se);
            }
        }
        #endregion

        #region Các hàm tiện ích

        public bool ChangeDatabase(string newDbName)
        {
            connection.Open();

            try
            {
                connection.ChangeDatabase(newDbName);
                return true;
            }
            catch (SqlException se)
            {
                hasErrors = true;
                LogFile.SqlError(se.Message, se);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Ham thuc thi cau lenh Sql bat ky
        /// </summary>
        public bool UpdateByNonQuery(string strNonQuery)
        {
            if (!multiStatement)
                if (!OpenConnection())
                    return false;

            try
            {
                SqlCommand sCmd = new SqlCommand(strNonQuery, connection);
                if (sCmd.Connection.State == ConnectionState.Closed)
                    return false;
                sCmd.Transaction = strMain;
                sCmd.CommandTimeout = _timeOut;
                sCmd.ExecuteNonQuery();

                if (!multiStatement)
                    strMain.Commit();
                if (strNonQuery.ToLower().Contains("create table")) DesignLog.SqlError(strNonQuery);
                if (strNonQuery.ToLower().Contains("alter table")) DesignLog.SqlError(strNonQuery);
                //if (strNonQuery.ToLower().Contains("hdb1")) DesignLog.SqlError(strNonQuery);
                LogFile.LogTruyXuatDL(strNonQuery);
                return true;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    strMain.Rollback();
                hasErrors = true;
                LogFile.SqlError(strNonQuery, se);
                return false;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }
        public bool UpdateByNonQuery(string strNonQuery, bool showMess)
        {
            if (!multiStatement)
                if (!OpenConnection())
                    return false;

            try
            {
                SqlCommand sCmd = new SqlCommand(strNonQuery, connection);
                sCmd.Transaction = strMain;
                sCmd.CommandTimeout = _timeOut;
                sCmd.ExecuteNonQuery();

                if (!multiStatement)
                    strMain.Commit();
                if (strNonQuery.ToLower().Contains("create table")) DesignLog.SqlError(strNonQuery);
                if (strNonQuery.ToLower().Contains("alter table")) DesignLog.SqlError(strNonQuery);
                //if (strNonQuery.ToLower().Contains("hdb1")) DesignLog.SqlError(strNonQuery);
                LogFile.LogTruyXuatDL(strNonQuery);
                return true;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    strMain.Rollback();
                hasErrors = true;
                //if (showMess)
                    LogFile.SqlError(strNonQuery, se,showMess);
                return false;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }
        /// <summary>
        /// Ham thuc thi cau lenh Sql bat ky
        /// </summary>
        public bool UpdateByNonQuery(string strNonQuery, ref int recAffect)
        {
            if (!multiStatement)
                if (!OpenConnection())
                    return false;

            try
            {
                SqlCommand sCmd = new SqlCommand(strNonQuery, connection);
                sCmd.Transaction = strMain;
                sCmd.CommandTimeout = _timeOut;
                recAffect = sCmd.ExecuteNonQuery();

                if (!multiStatement)
                    strMain.Commit();
                LogFile.LogTruyXuatDL(strNonQuery);
                return true;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    strMain.Rollback();
                hasErrors = true;
                LogFile.SqlError(strNonQuery, se);
                return false;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }

        /// <summary>
        /// Ham thuc thi cau lenh Sql bat ky (khong transaction)
        /// </summary>
        public bool UpdateByNonQueryNoTrans(string strNonQuery)
        {

            try
            {
                connection.Open();
                SqlCommand sCmd = new SqlCommand(strNonQuery, connection);
                sCmd.CommandTimeout = _timeOut;
                sCmd.ExecuteNonQuery();
                LogFile.LogTruyXuatDL(strNonQuery);
                return true;
            }
            catch (SqlException se)
            {
                hasErrors = true; LogFile.SqlError(strNonQuery, se);
                
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Ham thuc thi cau lenh Sql bat ky
        /// </summary>
        public object GetValue(string strNonQuery)
        {
            if (!multiStatement)
                if (!OpenConnection())
                    return null;

            try
            {
                SqlCommand sCmd = new SqlCommand(strNonQuery, connection);
                if (sCmd.Connection.State == ConnectionState.Closed)
                {
                    if (!multiStatement)
                        strMain.Rollback();
                    hasErrors = true;
                    //LogFile.SqlError(strNonQuery, se);
                    return null;
                }
                sCmd.Transaction = strMain;
                sCmd.CommandTimeout = _timeOut;
                object o = sCmd.ExecuteScalar();

                if (!multiStatement)
                    strMain.Commit();
                LogFile.LogTruyXuatDL(strNonQuery);
                return o;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    strMain.Rollback();
                hasErrors = true;
                LogFile.SqlError(strNonQuery, se);
                return null;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }

        /// <summary>
        /// Ham lay mot bang theo cau truy van bat ky
        /// </summary>
        public DataTable GetDataTable(string queryString)
        {
            if (!multiStatement)
                if (!OpenConnection())
                    return null;

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(queryString, connection);
                DataTable dt = new DataTable();
                sda.SelectCommand.Transaction = strMain;
                sda.SelectCommand.CommandTimeout = _timeOut;
                sda.Fill(dt); 

                if (!multiStatement)
                    strMain.Commit();
                LogFile.LogTruyXuatDL(queryString);
                return dt;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    if (strMain.Connection != null)
                        strMain.Rollback();
                hasErrors = true;
                LogFile.SqlError(queryString, se);
                return null;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }
        public DataTable GetDataTableNotrans(string queryString)
        {
           
                if (!OpenConnectionNotrans())
                    return null;

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(queryString, connection);
                DataTable dt = new DataTable();
                sda.SelectCommand.CommandTimeout = _timeOut;
                sda.Fill(dt);
                
                LogFile.LogTruyXuatDL(queryString);
                return dt;
            }
            catch (SqlException se)
            {
                hasErrors = true;
                LogFile.SqlError(queryString, se);
                return null;
            }
            finally
            {
                    CloseConnection();
            }
        }
        /// <summary>
        /// Ham cap nhat mot bang theo cau truy van bat ky
        /// </summary>
        public bool UpdateDataTable(string queryString, DataTable dtData)
        {
            if (!multiStatement)
                if (!OpenConnection())
                    return false;

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(queryString, connection);
                sda.SelectCommand.Transaction = strMain;
                sda.SelectCommand.CommandTimeout = _timeOut;
                SqlCommandBuilder scb = new SqlCommandBuilder(sda);
                sda.Update(dtData);

                if (!multiStatement)
                    strMain.Commit();
                LogFile.LogTruyXuatDL(queryString);
                return false;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    strMain.Rollback();
                hasErrors = true;
                LogFile.SqlError(queryString, se);
                return false;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }
        #endregion
        public bool UpdateDataTableNotrans(string queryString, DataTable dtData)
        {

                if (!OpenConnectionNotrans())
                    return false;

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(queryString, connection);
                sda.SelectCommand.Transaction = strMain;
                sda.SelectCommand.CommandTimeout = _timeOut;
                SqlCommandBuilder scb = new SqlCommandBuilder(sda);
                sda.Update(dtData);

                LogFile.LogTruyXuatDL(queryString);
                return false;
            }
            catch (SqlException se)
            {

                hasErrors = true;
                LogFile.SqlError(queryString, se);
                return false;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }
        public DataTable GetDataSetByStore(string storeName, string[] paraNames, object[] paraValues)
        {
            DataTable dtData = new DataTable();
            SqlCommand sqlcm = new SqlCommand();
            sqlcm.CommandText = storeName;
            sqlcm.CommandType = CommandType.StoredProcedure;
            sqlcm.CommandTimeout = _timeOut;
            sqlcm.Connection = connection;
            if (multiStatement) { sqlcm.Transaction = this.strMain; }
            if (paraNames != null)
            {
                for (int i = 0; i < paraNames.Length; i++)
                {
                    SqlParameter sqlpara = new SqlParameter(paraNames[i], paraValues[i]);
                    sqlcm.Parameters.Add(sqlpara);
                }
            }
            SqlDataAdapter sda = new SqlDataAdapter(sqlcm);
            LogFile.LogTruyXuatDL(storeName + "/n" + paraNames.ToString() + "/n" + paraValues.ToString());
            try
            {
                sda.Fill(dtData);
            }
            catch(Exception ex) 
            { }
            return dtData;
        }
        public DataSet GetDataSetByStore1(string storeName, string[] paraNames, object[] paraValues)
        {
            DataSet dtData = new DataSet();
            SqlCommand sqlcm = new SqlCommand();
            sqlcm.CommandText = storeName;
            sqlcm.CommandType = CommandType.StoredProcedure;
            sqlcm.CommandTimeout = _timeOut;
            sqlcm.Connection = connection;
            if (multiStatement) { sqlcm.Transaction = this.strMain; }
            if (paraNames != null)
            {
                for (int i = 0; i < paraNames.Length; i++)
                {
                    SqlParameter sqlpara = new SqlParameter(paraNames[i], paraValues[i]);
                    sqlcm.Parameters.Add(sqlpara);
                }
            }
            SqlDataAdapter sda = new SqlDataAdapter(sqlcm);
            LogFile.LogTruyXuatDL(storeName + "/n" + paraNames.ToString() + "/n" + paraValues.ToString());
            sda.Fill(dtData);
            return dtData;
        }
        public object GetValueByStoreObject(string storeName, string[] paraNames, object[] paraValues, ParameterDirection[] Direction, int r)
        {
            try
            {
                DataTable dtData = new DataTable();
                SqlCommand sqlcm = new SqlCommand();
                sqlcm.CommandText = storeName;
                sqlcm.CommandType = CommandType.StoredProcedure;
                sqlcm.CommandTimeout = _timeOut;
                sqlcm.Connection = connection;
                if (multiStatement) { sqlcm.Transaction = this.strMain; }
                if (paraNames != null)
                {
                    for (int i = 0; i < paraNames.Length; i++)
                    {
                        SqlParameter sqlpara;
                        sqlpara = new SqlParameter(paraNames[i], paraValues[i]);
                        sqlpara.Direction = Direction[i];
                        if (i == r && paraValues[i]!=null && paraValues[i].GetType() == typeof(string))
                        {
                            sqlpara.SqlDbType = SqlDbType.NVarChar;
                            sqlpara.Size = 4000;
                        }
                            sqlcm.Parameters.Add(sqlpara);

                    }
                }
                if (!multiStatement)
                {
                    connection.Open();
                }

                sqlcm.ExecuteNonQuery();
                if (!multiStatement)
                {
                    connection.Close();
                }

                LogFile.LogTruyXuatDL(storeName + "/n" + paraNames.ToString() + "/n" + paraValues.ToString());
                if (Direction[r] == ParameterDirection.Output)
                {

                    return sqlcm.Parameters[r].SqlValue;
                }
                return null;
            }
            catch (Exception e)
            {
                if (!multiStatement)
                {
                    connection.Close();
                }
                // MessageBox.Show(e.Message);
                return null;
            }
        }
        public double GetValueByStore(string storeName, string[] paraNames, object[] paraValues, ParameterDirection[] Direction, int r)
        {
            try
            {
                DataTable dtData = new DataTable();
                SqlCommand sqlcm = new SqlCommand();
                sqlcm.CommandText = storeName;
                sqlcm.CommandType = CommandType.StoredProcedure;
                sqlcm.CommandTimeout = _timeOut;
                sqlcm.Connection = connection;
                if (multiStatement) { sqlcm.Transaction = this.strMain; }
                if (paraNames != null)
                {
                    for (int i = 0; i < paraNames.Length; i++)
                    {
                        SqlParameter sqlpara;
                        sqlpara = new SqlParameter(paraNames[i], paraValues[i]);
                        sqlpara.Direction = Direction[i];
                        sqlcm.Parameters.Add(sqlpara);

                    }
                }
                if (!multiStatement)
                {
                    connection.Open();
                }

                sqlcm.ExecuteNonQuery();
                if (!multiStatement)
                {
                    connection.Close();
                }
                
                LogFile.LogTruyXuatDL(storeName + "/n" + paraNames.ToString() + "/n" + paraValues.ToString());
                if (Direction[r] == ParameterDirection.Output)
                {
                    
                    return double.Parse(sqlcm.Parameters[r].SqlValue.ToString());
                }
                return double.MinValue;
            }
            catch (Exception e)
            {
                if (!multiStatement)
                {
                    connection.Close();
                }
               // MessageBox.Show(e.Message);
                return double.MinValue;
            }
        }

        public bool UpdateDatabyStore(string storeName, string[] paraNames, object[] paraValues)
        {
            DataTable dtData = new DataTable();
            SqlCommand sqlcm = new SqlCommand();
            sqlcm.CommandText = storeName;
            sqlcm.CommandType = CommandType.StoredProcedure;
            sqlcm.CommandTimeout = _timeOut;
            sqlcm.Connection = connection;
           if (multiStatement) { sqlcm.Transaction = this.strMain; }
            
           
            if (paraNames != null)
            {
                for (int i = 0; i < paraNames.Length; i++)
                {
                    SqlParameter sqlpara;
                    sqlpara = new SqlParameter(paraNames[i], paraValues[i]);
                    sqlcm.Parameters.Add(sqlpara);

                }
            }

            LogFile.LogTruyXuatDL(storeName + "/n" + paraNames.ToString() + "/n" + paraValues.ToString());
            try
            {
               if (connection.State == ConnectionState.Closed)
                    connection.Open();
               int result = sqlcm.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                LogFile.AppendToFile("log.txt",ex.Message);
                hasErrors = true;
                return false;
            }
            if (!multiStatement)
                connection.Close();
            return true;
        }

        public bool UpdateDatabyPara(string sql, string[] paraNames, object[] paraValues)
        {
            SqlCommand sqlcm = new SqlCommand();
            sqlcm.CommandText = sql;
            sqlcm.CommandType = CommandType.Text;
            sqlcm.CommandTimeout = _timeOut;
            
            if (!multiStatement)
                if (!OpenConnection())
                    return false;
            sqlcm.Connection = connection;
            sqlcm.Transaction = this.strMain;
            if (paraNames != null)
            {
                for (int i = 0; i < paraNames.Length; i++)
                {
                    SqlParameter sqlpara;
                    sqlpara = new SqlParameter(paraNames[i], paraValues[i]);
                    sqlcm.Parameters.Add(sqlpara);
                }
            }
            try
            {
                LogFile.LogTruyXuatDL(sql + "/n" + paraNames.ToString() + "/n" + paraValues.ToString());
              int r=  sqlcm.ExecuteNonQuery();
                if (!multiStatement)
                    strMain.Commit();
                return true;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    strMain.Rollback();
                hasErrors = true;
                string sql1 = sql;
                foreach (object o in paraValues)
                {string value;
                if (o == null) value = "";
                else value = o.ToString();
                    sql1 += "-" + value;
                }
                LogFile.SqlError(sql1, se);
                return false;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }

        public bool UpdateData(string sql, string[] paraNames, object[] paraValues, SqlDbType[] paraTypes)
        {
            if (!multiStatement)
                if (!OpenConnection())
                    return false;
            string sqlUpdate = sql;
            try
            {
                
                SqlCommand sqlcm = new SqlCommand();
                sqlcm.CommandText = sql;
                sqlcm.CommandType = CommandType.Text;
                sqlcm.CommandTimeout = _timeOut;
                sqlcm.Connection = connection;
                sqlcm.Transaction = strMain;
                if (paraNames != null)
                {
                    for (int i = 0; i < paraNames.Length; i++)
                    {
                        SqlParameter sqlpara;
                        sqlpara = new SqlParameter(paraNames[i], paraValues[i]);
                        sqlpara.SqlDbType = paraTypes[i];
                        sqlcm.Parameters.Add(sqlpara);
                        string quote = "";
                        List<DbType> lstType = new List<DbType>() { DbType.String, DbType.Guid, DbType.DateTime };
                        if (lstType.Contains(sqlpara.DbType)) 
                            quote = "'";
                        if (sqlpara.Value == DBNull.Value)
                            sqlUpdate = sqlUpdate.Replace("@" + sqlpara.ParameterName, "Null");
                        else if (sqlpara.DbType == DbType.Boolean)
                        {
                            if (bool.Parse(sqlpara.Value.ToString())) sqlUpdate = sqlUpdate.Replace("@" + sqlpara.ParameterName, "1");
                            else sqlUpdate = sqlUpdate.Replace("@" + sqlpara.ParameterName, "0");
                        }
                        else if(sqlpara.DbType == DbType.String)
                            sqlUpdate = sqlUpdate.Replace("@" + sqlpara.ParameterName, "N" + quote + sqlpara.Value.ToString() + quote);
                        else
                            sqlUpdate = sqlUpdate.Replace("@" + sqlpara.ParameterName, quote + sqlpara.Value.ToString() + quote);
                    }
                }

                sqlcm.ExecuteNonQuery(); 
                if (sql.ToLower().Contains("insert into systable")) DesignLog.SqlError(sql + paraValues[0].ToString());
                if (sql.ToLower().Contains("insert into sysfield")) DesignLog.SqlError(sql + paraValues[0].ToString());
                if (sql.ToLower().Contains("update systable")) DesignLog.SqlError(sql + paraValues[0].ToString());
                if (sql.ToLower().Contains("update sysfield")) DesignLog.SqlError(sql + paraValues[0].ToString());
                if (!multiStatement)
                    strMain.Commit();
                //LogFile.LogTruyXuatDL(sql + "/n" + paraNames.ToString() + "/n" + paraValues.ToString());
                return true;
            }
            catch (SqlException se)
            {
                if (!multiStatement)
                    strMain.Rollback();
                hasErrors = true;                
                LogFile.SqlError(sqlUpdate, se);
                return false;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }
        }

        public bool insertRow(string tableName, List<string> fieldName, List<string> values)
        {
            if (fieldName.Count != values.Count)
            {
                return false;
            }
            string sql = " insert into " + tableName + "(";
            for (int i = 0; i < fieldName.Count; i++)
            {
                sql += fieldName[i] + ",";
            }
            sql = sql.Substring(0, sql.Length - 1) + ") values(";
            for (int i = 0; i < fieldName.Count; i++)
            {
                sql += values[i] + ",";
            }
            if (!multiStatement)
                if (!OpenConnection())
                    return false;
            sql = sql.Substring(0, sql.Length - 1) + ")";
            SqlCommand sqlcm = new SqlCommand();
            sqlcm.CommandText = sql;
            sqlcm.Transaction = this.strMain;
            sqlcm.CommandType = CommandType.Text;
            sqlcm.CommandTimeout = _timeOut;
            sqlcm.Connection = connection;
            try
            {
                sqlcm.ExecuteNonQuery();
                if (!multiStatement)
                    strMain.Commit();
                return true;
            }
            catch (SqlException se)
            {
                hasErrors = true;
                LogFile.SqlError(sql, se);
                strMain.Rollback();
                return false;
            }
            finally
            {
                if (!multiStatement)
                    CloseConnection();
            }

        }
    }
}
