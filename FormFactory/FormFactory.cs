using System;
using System.Collections.Generic;
using System.Data;
using DataFactory;
using CDTControl;
using CusCDTData;
using System.IO;
using System.Runtime.Remoting;
using System.Xml;
using CDTLib;
namespace FormFactory
{
    public enum FormType { Single, Detail, MasterDetail };
    public class FormFactory
    {
       
        public static CDTForm Create(FormType formType, string sysTableID)
        {
            CDTForm tmp = null;
            CDTData data = null;
            switch (formType)
            {
                case FormType.Detail:
                    data = DataFactory.DataFactory.Create(DataType.Detail, sysTableID);                   
                    tmp = new FrmDetail(data);

                    break;
                case FormType.MasterDetail:
                    data = DataFactory.DataFactory.Create(DataType.MasterDetail, sysTableID);
                    tmp = new FrmMasterDetail(data);

                    break;
                case FormType.Single:
                    data = DataFactory.DataFactory.Create(DataType.Single, sysTableID);
                    tmp = new FrmSingle(data);

                    break;

                        
            }
            //if (tmp != null) tmp.Text += "    " + data.DrTable["TableName"].ToString();
            return tmp;
        }


        public static CDTForm Create(FormType formType, DataRow drTable)
        {
            CDTForm tmp = null;
            CDTData data ;
            switch (formType)
            {
                case FormType.Detail:
                    data = DataFactory.DataFactory.Create(DataType.Detail, drTable);
                    tmp = new FrmDetail(data);
                    break;
                case FormType.MasterDetail:
                    DateTime t1 = DateTime.Now;
                    data = DataFactory.DataFactory.Create(DataType.MasterDetail, drTable);
                   // ErrorManager.LogFile.AppendToFile("log.txt", (DateTime.Now - t1).TotalMilliseconds.ToString());
                    t1 = DateTime.Now;
                    tmp = new FrmMasterDetail(data);
                   // ErrorManager.LogFile.AppendToFile("log.txt", (DateTime.Now - t1).TotalMilliseconds.ToString());
                    break;
                case FormType.Single:
                    data = DataFactory.DataFactory.Create(DataType.Single, drTable);
                    tmp = new FrmSingle(data);
                    break;
            }
           // if (tmp != null) tmp.Text += "    " + drTable["TableName"].ToString();
            return tmp;
        }

        public static CDTForm Create(FormType formType, CDTData data)
        {
            CDTForm tmp = null;
            switch (formType)
            {
                case FormType.Detail:
                    tmp = new FrmDetail(data);
                    break;
                case FormType.MasterDetail:
                    tmp = new FrmMasterDetail(data);
                    break;
                case FormType.Single:
                    tmp = new FrmSingle(data);
                    break;
            }
           // if (tmp != null) tmp.Text += "    " + data.DrTable["TableName"].ToString();
            return tmp;
        }

        private List<ICDTData> _lstICDTData = new List<ICDTData>();
        string _pluginPath = "";
        private void AddICDTData(CDTData Data)
        {
            if (Config.GetValue("DuongDanPlugins") != null)
                _pluginPath = Config.GetValue("DuongDanPlugins").ToString() + "\\" + Config.GetValue("Package").ToString() + "\\";
            else
                _pluginPath = System.Windows.Forms.Application.StartupPath + "\\Plugins\\" + Config.GetValue("Package").ToString() + "\\";

            if (!Directory.Exists(_pluginPath))
                return;
            string[] dllFiles = Directory.GetFiles(_pluginPath, "*.dll");
            foreach (string str in dllFiles)
            {
                FileInfo f = new FileInfo(str);
                string t = f.Name.Split(".".ToCharArray())[0];
                string pluginName = t + "." + t;
                ObjectHandle oh = Activator.CreateComInstanceFrom(str, pluginName);
                ICDTData pluginClass = oh.Unwrap() as ICDTData;
                if (pluginClass != null)
                {
                    if (!_lstICDTData.Contains(pluginClass))
                    {
                        _lstICDTData.Add(pluginClass);
                        pluginClass.data = Data;
                        pluginClass.AddEvent();
                    }
                }
            }
        }
        
    }
}
