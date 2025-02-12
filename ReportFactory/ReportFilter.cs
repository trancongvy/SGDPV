using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid;
using System.Data.SqlClient;
using FormFactory;
using DataFactory;
using CDTLib;
using CDTControl;

namespace ReportFactory
{
    public partial class ReportFilter : CDTForm
    {
        public ReportFilter(CDTData data)
        {
            InitializeComponent();

            this._data = data;
            this._frmDesigner = new FormDesigner(this._data);
            _frmDesigner.formAction = FormAction.Filter;
            this.SetFormCaption();
            this.Load += new EventHandler(ReportFilter_Load);
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
            fieldCount = _data.DsStruct.Tables[0].Rows.Count;
            if (_data.DsData == null)
                _data.GetData();
            _bindingSource.DataSource = this._data.DsData.Tables[0];
            _frmDesigner.bindingSource = _bindingSource;
            dxErrorProviderMain.DataSource = _frmDesigner.bindingSource;
            _bindingSource.AddNew();
            _bindingSource.EndEdit();
            if (fieldCount == 0)
            {
                isNotify = true;               
                return;
            }
        }
        public void ShowNotify()
        {
            simpleButtonAccept_Click(simpleButtonAccept, new EventArgs());
        }
        void ReportFilter_Load(object sender, EventArgs e)
        {           
            this.AddLayoutControl();
        }
        int fieldCount;
        public bool isNotify = false;
        private void AddLayoutControl()
        {
            int x = 100, y = 80;
           
            if (fieldCount < 6)
            {
                x = 200;
                y = 160;
            }
            this.Width = fieldCount * 50 + x;
            this.Height = fieldCount * 40 + y;

            LayoutControl lcMain;
            GridControl gcTmp = null;
            lcMain = _frmDesigner.GenLayout2(ref gcTmp, true);
            if (lcMain == null) return;
            lcMain.Root.DefaultLayoutType = DevExpress.XtraLayout.Utils.LayoutType.Vertical;

            LayoutControlItem lci3 = new LayoutControlItem();
            lci3.TextVisible = false;
            lcMain.AddItem(lci3);

            LayoutControlItem lci1;
            lci1 = new LayoutControlItem();
            lci1.Control = simpleButtonAccept;
            lci1.TextVisible = false;
            
            LayoutControlItem lci2;
            lci2 = new LayoutControlItem();
            lci2.Control = simpleButtonCancel;
            lci2.TextVisible = false;

            
            LayoutControlGroup lcg3 = lcMain.Root.AddGroup();
            
            lcg3.TextVisible = false;
            lcg3.GroupBordersVisible = false;
            lcg3.DefaultLayoutType = DevExpress.XtraLayout.Utils.LayoutType.Horizontal;
            lcg3.Add(lci1); lcg3.Add(lci2);

            lcg3.Size = new Size( 0, 20);
            lci1.Size = new Size((lcg3.Size.Width / 2), 0);
            //if (fieldCount > 6)
            //    lcMain = _frmDesigner.GenLayout2(ref gcTmp, false);
            //else
            //    lcMain = _frmDesigner.GenLayout1(ref gcTmp, false);
            this.Controls.Add(lcMain);
            
        }

        private void ReportFilter_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    simpleButtonCancel_Click(simpleButtonCancel, e);
                    break;
                case Keys.F12:
                    simpleButtonAccept_Click(simpleButtonAccept, e);
                    break;
            }
        }

        private void simpleButtonAccept_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", "Xem báo cáo");
            Config.NewKeyValue("MenuName", this.Text);
            _bindingSource.EndEdit();
            DataRowView drv = (_bindingSource.Current as DataRowView);
            DataReport __data = new DataReport((_data as DataReport).DrTable);
            __data.GetData();
            __data.DsData = _data.DsData;
            __data.reConfig = new ReConfig();
            __data.reConfig.Variables=(_data as DataReport).reConfig.Copy();
            if (drv != null)
                __data.DrCurrentMaster = drv.Row;
            __data.CheckRules(DataAction.IUD);
            if (dxErrorProviderMain.HasErrors)
            {
                XtraMessageBox.Show("Chưa nhập đủ thông tin yêu cầu, vui lòng kiểm tra lại!");
                return;
            }
            (__data as DataReport).SaveVariables();
            (__data as DataReport).GenFilterString();
            ReportPreview rptPre = new ReportPreview(__data);
            rptPre.MdiParent = this.MdiParent;
            rptPre.Show();
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReportFilter_Load_1(object sender, EventArgs e)
        {

        }
    }
}