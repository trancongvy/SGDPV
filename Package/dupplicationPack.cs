using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using System.Windows.Forms;
namespace Package
{
    class dupplicationPack
    {
        private string SPack;
        private string TPack;
        private int SPid;
        private int TPid;
        private Database _data;
        public dupplicationPack(string strConnection, string spack, string tpack)
        {
            _data = Database.NewCustomDatabase(strConnection);
            SPack = spack;
            TPack = tpack;
            
            //  
            try
            {
                SPid = int.Parse(_data.GetValue("select syspackageid from syspackage where Package = '" + SPack + "'").ToString());

                string sql = "insert into syspackage (package,PackageName,Version,DBName,DbPath,BackGround,CopyRight,PackageName2)";
                sql += " select '" + TPack.ToString() + "' as Package,PackageName + '" + tpack.ToString() + "', version,'" + tpack.ToString() + "' as DBName,replace(dbPath,'" + SPack.ToString() + "','" + TPack.ToString() + "'),BackGround,CopyRight,PackageName2 ";
                sql += " from syspackage where syspackageid=" + SPid.ToString();
                _data.UpdateByNonQuery(sql);
                TPid = int.Parse(_data.GetValue("select syspackageid from syspackage where Package = '" + TPack + "'").ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show(" Không tồn tại Gói phần mềm nguồn");
                return;
            }
                 

            try
            {

                CopyDatabase();
                _data.BeginMultiTrans();
                CopyTable();
                CopyField();
                CopyConfig();
                CopyConfigDt();
                CopyReport();
                CopyMenu();
                CopyReportFilter();
                CopyFormReport();
                CopyPara();
                //Xóa các cột tạm
                string sql = "alter table sysTable drop column OldId ";
                _data.UpdateByNonQuery(sql);
                sql = "alter table sysField drop column OldId ";
                _data.UpdateByNonQuery(sql);
                sql = "alter table sysDataconfig drop column OldId ";
                _data.UpdateByNonQuery(sql);
                sql = "alter table sysDataconfigdt drop column OldId ";
                _data.UpdateByNonQuery(sql);
                sql = "alter table sysReport drop column OldId ";
                _data.UpdateByNonQuery(sql);
                sql = "alter table sysMenu drop column OldId ";
                _data.UpdateByNonQuery(sql);
                sql = "alter table sysReportFilter drop column OldId ";
                _data.UpdateByNonQuery(sql);
                sql = "alter table sysFormReport drop column OldId ";
                _data.UpdateByNonQuery(sql);

                 _data.EndMultiTrans();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Không tạo được dữ liệu");
                 _data.RollbackMultiTrans();
            }
            
            //_data.RollbackMultiTrans();
        }
        private bool CopyDatabase()
        {           
            string sql;
            sql = "BACKUP DATABASE " + SPack.ToString() + "  TO DISK = 'c:\\" + SPack.ToString() + ".bak' \n";
            sql += "RESTORE FILELISTONLY   FROM DISK = 'c:\\" + SPack.ToString() + ".bak' \n";
            sql += "RESTORE DATABASE " + TPack + "  FROM DISK = 'c:\\" + SPack.ToString() + ".bak' \n ";
            sql += "   WITH MOVE '" + SPack.ToString() + "' TO '" + Application.StartupPath + "\\Data\\" + TPack.ToString() + ".mdf' \n";
            sql += "   , MOVE '" + SPack.ToString() + "_log' TO '" + Application.StartupPath + "\\Data\\" + TPack.ToString() + ".ldf'";
            _data.UpdateByNonQueryNoTrans(sql);
            return true;
        }
        private bool CopyTable()
        {
            string sql = "alter table sysTable add OldId int null ";
            _data.UpdateByNonQuery(sql);
            sql = " insert into systable ([TableName],[DienGiai],[DienGiai2],[Pk] ,[ParentPk],[MasterTable],[Type],[SortOrder] ,[DetailField],[System],[MaCT],[sysPackageID] ,[Report],[CollectType],[OldId] )";
            sql += " select [TableName],[DienGiai],[DienGiai2],[Pk] ,[ParentPk],[MasterTable],[Type],[SortOrder] ,[DetailField],[System],[MaCT]," + TPid.ToString() + " as [sysPackageID] ,[Report],[CollectType],systableID as [OldId]  from systable ";
            sql += " where syspackageId=" + SPid.ToString();
            _data.UpdateByNonQuery(sql);
            return true;
        }
        private bool CopyField()
        {   //Copy trường
            string sql = "alter table sysField add OldId int  null";
            _data.UpdateByNonQuery(sql);
            sql = " insert into sysField ([sysTableID],[FieldName],[AllowNull],[RefField],[RefTable],[DisplayMember],[RefCriteria],[Type],[LabelName],[LabelName2],[TabIndex],[Formula] ,[FormulaDetail],[MaxValue],[MinValue],[DefaultValue],[Tip],[TipE],[Visible],[IsBottom],[IsFixCol],[IsGroupCol],[Editable],[RefName],[DefaultName],[EditMask],[CasUpdate],[CasDelete],[IsUnique],[OldId]) ";
            sql += "select [sysTableID],[FieldName],[AllowNull],[RefField],[RefTable],[DisplayMember],[RefCriteria],[Type],[LabelName],[LabelName2],[TabIndex],[Formula] ,[FormulaDetail],[MaxValue],[MinValue],[DefaultValue],[Tip],[TipE],[Visible],[IsBottom],[IsFixCol],[IsGroupCol],[Editable],[RefName],[DefaultName],[EditMask],[CasUpdate],[CasDelete],[IsUnique],[SysFieldID] ";
            sql += " from sysField where systableid in (select systableid from systable where syspackageid=" + SPid + ") ";
            _data.UpdateByNonQuery(sql);
            // Đổi các tableid mới 
            sql = "update sysfield set systableid =t.NewID from sysField,";
            sql += "  ( select systableid as Newid , OldID from  systable where OldID>0 ) t";
            sql += " where sysField.sysTableID= t.OldID and sysField.OldID>0";
            _data.UpdateByNonQuery(sql);
            //sql = "alter table sysField drop column OldID";
            //_data.UpdateByNonQuery(sql);
            return true;
        }
        private bool CopyConfig()
        {
            string sql = "alter table sysDataConfig add OldId int  null";
            _data.UpdateByNonQuery(sql);
            sql="Insert into sysDataConfig (sysTableid, mtTableid,dtTableID,Nhomdk, RootIDName, EditSync, Condition,DTID,OldID)";
            sql += " select systableid, mtTableid,dtTableID,Nhomdk, RootIDName, EditSync, Condition,DTID,blconfigID ";
            sql += " from sysDataConfig where sysTableID in (select sysTableID from sysTable where sysPackageid =" + SPid.ToString() + ")";
            _data.UpdateByNonQuery(sql);
            //Đổi các TableID -- Systable
            sql = "update sysDataConfig set sysTableid =t.NewID from sysDataConfig,";
            sql += "  ( select systableid as Newid , OldID from  systable where OldID>0 ) t";
            sql += " where sysDataConfig.sysTableID= t.OldID and sysDataConfig.OldID>0";
            _data.UpdateByNonQuery(sql);
            //Đổi các TableID -- MTtable
            sql = "update sysDataConfig set mtTableid =t.NewID from sysDataConfig,";
            sql += "  ( select systableid as Newid , OldID from  systable where OldID>0 ) t";
            sql += " where sysDataConfig.mtTableID= t.OldID and sysDataConfig.OldID>0";
            _data.UpdateByNonQuery(sql);
            //Đổi các TableID -- dtTable
            sql = "update sysDataConfig set dtTableid =t.NewID from sysDataConfig,";
            sql += "  ( select systableid as Newid , OldID from  systable where OldID>0 ) t";
            sql += " where sysDataConfig.dtTableID= t.OldID and sysDataConfig.OldID>0";
            _data.UpdateByNonQuery(sql);
            return true;
        }
        private bool CopyConfigDt()
        {
            string sql = "alter table  sysDataConfigDt add OldId int  null";
            _data.UpdateByNonQuery(sql);
            sql = "Insert into sysDataConfigdt (blConfigID,blFieldID,MtfieldID, DtFieldID,Formula,OldID)";
            sql += " select blConfigID,blFieldID,MtfieldID, DtFieldID,Formula,blConfigDetailID ";
            sql += " from sysDataConfigdt where BlConfigID in (select BlConfigID from sysDataConfig where ";
            sql += " sysTableID in (select sysTableID from sysTable where sysPackageid =" + SPid.ToString() + "))";
            _data.UpdateByNonQuery(sql);
            //Đổi các ID -- blConfigID
            sql = "update sysDataConfigDt set blConfigID =t.NewID from sysDataConfigDt,";
            sql += "  ( select blConfigID as NewID, OldID from sysDataConfig where OldID>0) t";
            sql += " where sysDataConfigdt.blConfigID= t.OldID and sysDataConfigdt.OldID>0";
            _data.UpdateByNonQuery(sql);
            //Đổi các ID -- blFiedldID
            sql = "update sysDataConfigDt set blFieldID =t.NewID from sysDataConfigDt,";
            sql += "  ( select sysFieldid as Newid , OldID from  sysField where OldID>0 ) t";
            sql += " where sysDataConfigdt.blFieldID= t.OldID and sysDataConfigdt.OldID>0";
            _data.UpdateByNonQuery(sql);
            //Đổi các ID -- mtFiedldID
            sql = "update sysDataConfigDt set mtFieldID =t.NewID from sysDataConfigDt,";
            sql += "  ( select sysFieldid as Newid , OldID from  sysField where OldID>0 ) t";
            sql += " where sysDataConfigdt.mtFieldID= t.OldID and sysDataConfigdt.OldID>0";
            _data.UpdateByNonQuery(sql);
            //Đổi các ID -- dtFiedldID
            sql = "update sysDataConfigDt set dtFieldID =t.NewID from sysDataConfigDt,";
            sql += "  ( select sysFieldid as Newid , OldID from  sysField where OldID>0 ) t";
            sql += " where sysDataConfigdt.dtFieldID= t.OldID and sysDataConfigdt.OldID>0";
            _data.UpdateByNonQuery(sql);
            return true;
        }   
        private bool CopyReport()
        {
            string sql = "alter table  sysReport add OldId int  null";
            _data.UpdateByNonQuery(sql);
            sql = " Insert into SysReport (ReportName ,RpType ,mtTableID,dtTableID,Query ,ReportFile,ReportName2,ReportFile2,	sysReportParentID,LinkField,ColField ,ChartField1,ChartField2,ChartField3,sysPackageID,	mtAlias ,dtAlias ,TreeData,OldID) ";
            sql += " select ReportName ,RpType ,mtTableID,dtTableID,Query ,ReportFile,ReportName2,ReportFile2,	sysReportParentID,LinkField,ColField ,ChartField1,ChartField2,ChartField3," + TPid.ToString() + "sysPackageID,	mtAlias ,dtAlias ,TreeData,sysReportID as OldID from sysReport  ";
            sql += " where sysPackageID=" + SPid.ToString();
            _data.UpdateByNonQuery(sql);
            //Đổi ParentID
            sql = "update sysReport set sysReportParentID =t.NewID from sysReport,";
            sql += "  ( select sysReportid as Newid , OldID from  sysReport where OldID>0 ) t";
            sql += " where sysReport.sysReportParentID= t.OldID and sysRePort.OldID>0";
            _data.UpdateByNonQuery(sql);
            //Đổi mtTableID 
            sql = "update sysReport set mtTableID =t.NewID from sysReport,";
            sql += "  ( select sysTableid as Newid , OldID from  sysTable where OldID>0 ) t";
            sql += " where sysReport.mtTableID= t.OldID and sysRePort.OldID>0";
            _data.UpdateByNonQuery(sql);
            //Đổi dtTableID 
            sql = "update sysReport set dtTableID =t.NewID from sysReport,";
            sql += "  ( select sysTableid as Newid , OldID from  sysTable where OldID>0 ) t";
            sql += " where sysReport.dtTableID= t.OldID and sysRePort.OldID>0";
            _data.UpdateByNonQuery(sql);
            return true;
        }
        private bool CopyMenu()
        {
            string sql = "alter table  sysMenu add OldId int  null";
            _data.UpdateByNonQuery(sql);
            sql = " insert into sysMenu ( MenuName ,MenuName2,ShortKey ,Image ,sysPackageID,CustomType ,sysTableID ,sysReportID,MenuOrder ,ExtraSql ,	sysMenuParent,isToolbar ,MenuPluginID,PluginName,OldID) ";
            sql += " select  MenuName ,MenuName2,ShortKey ,Image ," + TPid.ToString() + "sysPackageID,CustomType ,sysTableID ,sysReportID,MenuOrder ,ExtraSql ,	sysMenuParent,isToolbar ,MenuPluginID,PluginName,sysMenuID as OldID from sysMenu ";
            sql += " where sysPackageID=" + SPid.ToString();
            _data.UpdateByNonQuery(sql);
            //Đổi ParentID
            sql = "update sysMenu set sysMenuParent =t.NewID from sysMenu,";
            sql += "  ( select sysMenuid as Newid , OldID from  sysMenu where OldID>0 ) t";
            sql += " where sysMenu.sysMenuParent= t.OldID and sysMenu.OldID>0";
            _data.UpdateByNonQuery(sql);
            //Đổi sysTableID 
            sql = "update sysMenu set sysTableID =t.NewID from sysMenu,";
            sql += "  ( select sysTableid as Newid , OldID from  sysTable where OldID>0 ) t";
            sql += " where sysMenu.sysTableID= t.OldID and sysMenu.OldID>0";
            _data.UpdateByNonQuery(sql);
            //Đổi sysReportid
            sql = "update sysMenu set sysReportID =t.NewID from sysMenu,";
            sql += "  ( select sysReportid as Newid , OldID from  sysReport where OldID>0 ) t";
            sql += " where sysMenu.sysReportID= t.OldID and sysMenu.OldID>0";
            _data.UpdateByNonQuery(sql);
            return true;
        }

        private bool CopyReportFilter()
        {
            string sql = "alter table  sysReportFilter add OldId int  null";
            _data.UpdateByNonQuery(sql);
            sql = "insert into sysReportFilter (sysFieldID, allowNull, DefaultValue,sysReportID, isBetween,TabIndex,Visible,isMaster,SpecialCond,OldID) ";
            sql += " select  sysFieldID, allowNull, DefaultValue,sysReportID, isBetween,TabIndex,Visible,isMaster,SpecialCond,sysReportFilterID as OldID ";
            sql += " from  sysReportFilter where sysReportID in (select sysReportID from sysReport where sysPackageid=" + SPid.ToString() + ")";
            _data.UpdateByNonQuery(sql);
            //Đổi reportid
            sql = "update sysReportFilter set sysReportID =t.NewID from sysReportFilter,";
            sql += "  ( select sysReportid as Newid , OldID from  sysReport where OldID>0 ) t";
            sql += " where sysReportFilter.sysReportID= t.OldID and sysReportFilter.OldID>0";
            _data.UpdateByNonQuery(sql);
            //Đổi các ID -- sysFiedldID
            sql = "update sysReportFilter set sysFieldID =t.NewID from sysReportFilter,";
            sql += "  ( select sysFieldid as Newid , OldID from  sysField where OldID>0 ) t";
            sql += " where sysReportFilter.sysFieldID= t.OldID and sysReportFilter.OldID>0";
            _data.UpdateByNonQuery(sql);
            return true;
        }
        private bool CopyFormReport()
        {
            string sql = "alter table  sysFormReport add OldId int  null";
            _data.UpdateByNonQuery(sql);
            sql = "insert into sysFormReport (sysReportID, reportName,ReportFile, reportName2,ReportFile2,OldID) ";
            sql += " select  sysReportID, reportName,ReportFile, reportName2,ReportFile2,sysFormReportID as OldId ";
            sql += " from sysFormReport where sysReportID in (select sysReportID from sysReport where sysPackageid=" + SPid.ToString() + ")";
            _data.UpdateByNonQuery(sql);
            sql = "update sysFormReport set sysReportID =t.NewID from sysFormReport,";
            sql += "  ( select sysReportid as Newid , OldID from  sysReport where OldID>0 ) t";
            sql += " where sysFormReport.sysReportID= t.OldID and sysFormReport.OldID>0";
            _data.UpdateByNonQuery(sql);
            return true;
        }
        private bool CopyPara()
        { 
            string sql=" insert into sysconfig (_key,_value,isUser, sysPackageId, StartConfig)";
            sql += " select  _key,_value,isUser," + TPid.ToString() + " as  sysPackageId, StartConfig from sysconfig where sysPackageid =" + SPid.ToString();
            _data.UpdateByNonQuery(sql);
            return true;
        }
    }
}
