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
using CusData;
using CDTLib;
using CDTControl;

namespace CusForm
{
    internal partial class FrmSingleDt : CDTForm
    {
        private bool _x;
        public FrmSingleDt(CDTData data,BindingSource bs)
        {
            InitializeComponent();

            this._data = data;
            this._frmDesigner = new FormDesigner(this._data);
            _frmDesigner.formAction = FormAction.New;
            _bindingSource = bs;
            //_bindingSource.DataSource = this._data.DsData.Tables[0];

            _frmDesigner.bindingSource = _bindingSource;
            dataNavigatorMain.DataSource = _frmDesigner.bindingSource;
            dxErrorProviderMain.DataSource = _frmDesigner.bindingSource;
            _bindingSource.AddNew();
            _bindingSource.EndEdit();
            InitializeLayout();
            this.Load += new EventHandler(FrmSingleDt_Load);
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
            else
                this.dataNavigatorMain.TextStringFormat = "Mục {0} của {1}";
            dataNavigatorMain.PositionChanged += new EventHandler(dataNavigatorMain_PositionChanged);
        }

        public FrmSingleDt(FormDesigner frmDesigner)
        {
            InitializeComponent();

            this._data = frmDesigner.Data;
            this._frmDesigner = frmDesigner;
            this._bindingSource = frmDesigner.bindingSource;
            dataNavigatorMain.DataSource = this._bindingSource;
            dxErrorProviderMain.DataSource = this._bindingSource;

            InitializeLayout();
            this.Load += new EventHandler(FrmSingleDt_Load);
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
            dataNavigatorMain.PositionChanged += new EventHandler(dataNavigatorMain_PositionChanged);
        }

        void dataNavigatorMain_PositionChanged(object sender, EventArgs e)
        {
            if (_frmDesigner.formAction != FormAction.Delete && _frmDesigner.formAction != FormAction.View)
                SetCurrentData();
        }

        private void InitializeLayout()
        {
            this.SetFormCaption();
            this.AddLayoutControl();
            
            dataNavigatorMain.SendToBack();
        }

        private void AddLayoutControl()
        {
            int x = 100, y = 80;
            int fieldCount = _data.DsStruct.Tables[0].Rows.Count;
            if (fieldCount < 6)
            {
                x = 200;
                y = 160;
            }
            this.Width = fieldCount * 50 + x;
            this.Height = fieldCount * 40 + y;

            LayoutControl lcMain;
            GridControl gcTmp = null;
            if (fieldCount > 12)
                lcMain = _frmDesigner.GenLayout2(ref gcTmp, true);
            else
                lcMain = _frmDesigner.GenLayout1(ref gcTmp, true);
            this.Controls.Add(lcMain);
        }

        private void UpdateData()
        {
            _bindingSource.EndEdit();
            if (!_data.DataChanged)
                this.DialogResult = DialogResult.Cancel;
            else
            {                
                DataAction dataAction = (_frmDesigner.formAction == FormAction.Edit) ? DataAction.Update : DataAction.Insert;
                _data.CheckRules(dataAction);
                if (dxErrorProviderMain.HasErrors)
                {
                    XtraMessageBox.Show("Số liệu chưa hợp lệ, vui lòng kiểm tra lại trước khi lưu!");
                    return;
                }

                if (this._data.UpdateData(dataAction))
                {
                    _x = false;
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    XtraMessageBox.Show("Số liệu chưa hợp lệ, vui lòng kiểm tra lại trước khi lưu! Xem quyền update Item");
                }
            }
        }

        private void CancelUpdate()
        {
            _x = false;

           // _data.DataChanged = true;
            if (!_data.DataChanged)
                this.DialogResult = DialogResult.Cancel;
            else
                if (XtraMessageBox.Show("Số liệu chưa được lưu, bạn có thật sự muốn quay ra?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _data.CancelUpdate();
                    if (_data.dataType == DataType.Single)
                    {
                        if (this._frmDesigner.formAction == FormAction.New )
                        {
                            _bindingSource.RemoveCurrent();
                        }
                        else if (this._frmDesigner.formAction == FormAction.Copy)
                        {
                            _bindingSource.DataSource = this._data.DsData.Tables[0];
                        }
                        else
                        {
                            _bindingSource.CancelEdit();
                        }
                    }
                    this.DialogResult = DialogResult.Cancel;
                }
            
        }

        private void dataNavigatorMain_ButtonClick(object sender, NavigatorButtonClickEventArgs e)
        {
            if (e.Button == dataNavigatorMain.Buttons.EndEdit)
            {
                Config.NewKeyValue("Operation", "F12-Lưu");
                e.Handled = true;
                UpdateData();
            }
            if (e.Button == dataNavigatorMain.Buttons.CancelEdit)
            {
                Config.NewKeyValue("Operation", "ESC-Hủy");
                e.Handled = true;
                CancelUpdate();
            }
        }

        private void SetCurrentData()
        {
            DataRowView drv = (_bindingSource.Current as DataRowView);
            if (drv != null)
                this._data.DrCurrentMaster = drv.Row;
        }

        private void FrmSingleDt_Load(object sender, EventArgs e)
        {
            _x = true;
            SetCurrentData();

            if (_frmDesigner.formAction == FormAction.Copy)
            {
                _data.CloneData();
                if (_bindingSource.Count != _data.DsData.Tables[0].Rows.Count)
                { _bindingSource.DataSource = _data.DsData.Tables[0]; }
                        
                //_bindingSource.Position = _data.DsData.Tables[0].Rows.Count - 1;
            }
            _frmDesigner.RefreshViewForLookup();
        }

        private void FrmSingleDt_KeyDown(object sender, KeyEventArgs e)
    {
            switch (e.KeyCode)
            {
                case Keys.PageUp:
                    if (e.Modifiers == Keys.Control)
                        dataNavigatorMain.Buttons.First.DoClick();
                    else
                        dataNavigatorMain.Buttons.Prev.DoClick();
                    break;
                case Keys.PageDown:
                    if (e.Modifiers == Keys.Control)
                        dataNavigatorMain.Buttons.Last.DoClick();
                    else
                        dataNavigatorMain.Buttons.Next.DoClick();
                    break;
                case Keys.Escape:
                    CancelUpdate();
                    break;
                case Keys.F12:
                    UpdateData();
                    break;
            }
        }

        private void FrmSingleDt_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_x)
                CancelUpdate();
            _data.Reset();
            _frmDesigner.formAction = FormAction.View;
            _frmDesigner.FirstControl.Focus();
        }
    }
}
