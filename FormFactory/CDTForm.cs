using System;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList;
using CDTLib;
using DataFactory;
using CDTControl;
using Plugins;
using DevExpress.XtraGrid.Views.BandedGrid;
using System.IO;
using CusCDTData;
using System.Runtime.Remoting;
using System.Collections.Generic;

namespace FormFactory
{
    public enum FormAction { New, Edit, Delete, Copy, Filter, View, Approve };
    public class CDTForm : DevExpress.XtraEditors.XtraForm
    {
        protected CDTData _data;
        public BindingSource _bindingSource = new BindingSource();
        public FormDesigner _frmDesigner;
        protected GridControl gcMain = new GridControl();
        protected DevExpress.XtraGrid.Views.Grid.GridView gvMain = new DevExpress.XtraGrid.Views.Grid.GridView();
        protected AdvBandedGridView gbMain = new AdvBandedGridView();
        protected TreeList tlMain = new TreeList();
        private bool treeListFirst = true;
        protected PluginManager _plugins = new PluginManager();
        protected List<ICDTData> _lstICDTData = new List<ICDTData>();
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CDTForm));
            this.SuspendLayout();
            // 
            // CDTForm
            // 
            resources.ApplyResources(this, "$this");
            this.KeyPreview = true;
            this.Name = "CDTForm";
            this.ResumeLayout(false);
        }
        public CDTData Data
        {
            get { return _data; }
            set
            {
                CDTData tmp = value;
                _data = value;
                _data.DsData = tmp.DsData;

                foreach (ICDTData pl in _lstICDTData)
                    pl.AddEvent();
            }
        }
        string _pluginPath = "";
        protected void AddICDTData(CDTData Data)
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
                        pluginClass.gc = this.gcMain;
                        pluginClass.gv = this.gvMain;
                        pluginClass.be = this._frmDesigner._BaseList;
                        pluginClass.lo = this._frmDesigner._LayoutList;
                        pluginClass.glist = this._frmDesigner._glist;
                        pluginClass.rlist = this._frmDesigner.rlist;
                        pluginClass.gridList = this._frmDesigner._gcDetail;
                        pluginClass.Refresh += new EventHandler(pluginClass_Refresh);
                        pluginClass.data = Data;
                    }
                }
            }
        }
        void pluginClass_Refresh(object sender, EventArgs e)
        {
            this._frmDesigner.RefreshDataLookupForColChanged();
            //this._frmDesigner.RefreshDataForLookup();
        }
        protected void SetFormCaption()
        {
            if (_data.DrTable.Table.Columns.Contains("MenuName"))
                this.Text = Config.GetValue("Language").ToString() == "0" ? _data.DrTable["MenuName"].ToString() : _data.DrTable["MenuName2"].ToString();
            else if (_data.tbWF!=null && _data.tbWF.Rows.Count > 0)
            {
                this.Text = _data.tbWF.Rows[0]["WFName"].ToString();
            }
            else
            {
                if (_data.DrTable.Table.Columns.Contains("DienGiai"))
                    this.Text = Config.GetValue("Language").ToString() == "0" ? _data.DrTable["DienGiai"].ToString() : _data.DrTable["DienGiai2"].ToString();
                else
                    this.Text = Config.GetValue("Language").ToString() == "0" ? _data.DrTable["ReportName"].ToString() : _data.DrTable["ReportName2"].ToString();
            }
            
        }

        /// <summary>
        /// Chuyển từ kiểu hiển thị Grid sang Tree (chỉ xuất hiện khi bảng đang mở có dạng Tree: có ParentPk)
        /// </summary>
        protected void CheckTreeList()
        {
            if (treeListFirst)
            {
                if (_data.dataType == DataType.Single)
                {
                    tlMain = _frmDesigner.GenTreeListControl(_data.DrTable, _data.DsStruct.Tables[0]);
                    tlMain.DataSource = _bindingSource;
                }
                else
                {
                    tlMain = _frmDesigner.GenTreeListControl(_data.DrTableMaster, _data.DsStruct.Tables[1]);
                    tlMain.DataSource = _bindingSource.DataSource;
                    tlMain.Width = this.Width / 3;
                    tlMain.Dock = DockStyle.Left;
                }
                this.Controls.Add(tlMain);
                tlMain.BringToFront();
                tlMain.ExpandAll();
                treeListFirst = false;
            }
            gcMain.Visible = !gcMain.Visible;
            tlMain.Visible = !gcMain.Visible;
            tlMain.BestFitColumns();
        }
    }
}
