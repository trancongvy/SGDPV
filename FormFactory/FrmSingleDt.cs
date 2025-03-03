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
using DataFactory;
using CDTLib;
using CDTControl;
using DevControls;
using System.IO;
using Plugins;
using CusCDTData;
using System.Runtime.Remoting;
using System.Xml;

namespace FormFactory
{
    public partial class FrmSingleDt : CDTForm
    {
        private bool _x;
        LayoutControl lcMain;
        
        public FrmSingleDt(CDTData data,BindingSource bs)
        {
            InitializeComponent();

            this._data = data;
            
            this._frmDesigner = new FormDesigner(this._data);
            _frmDesigner.formAction = FormAction.New;
            AddICDTData(_data);
            _bindingSource = bs;
            //_bindingSource.DataSource = this._data.DsData.Tables[0];

            _frmDesigner.bindingSource = _bindingSource;
            dataNavigatorMain.DataSource = _frmDesigner.bindingSource;
            dxErrorProviderMain.DataSource = _frmDesigner.bindingSource;
            _bindingSource.AddNew();
            _bindingSource.EndEdit();
            //_data.DsData.Tables[0].Rows[_data.DsData.Tables[0].Rows.Count - 1]["MaKH"] = "XXXX";

            InitializeLayout();
            this.Load += new EventHandler(FrmSingleDt_Load);
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
            else
                this.dataNavigatorMain.TextStringFormat = "Mục {0} của {1}";
            dataNavigatorMain.PositionChanged += new EventHandler(dataNavigatorMain_PositionChanged);
            this._data.DsData.Tables[0].ColumnChanged += Table_ColumnChanged;
            foreach (ICDTData pl in _lstICDTData)
                pl.AddEvent();
        }

        string _pluginPath = "";
        

        public FrmSingleDt(CDTData data, BindingSource bs,bool isEdit)
        {
            InitializeComponent();

            this._data = data;
            this._frmDesigner = new FormDesigner(this._data);
            _frmDesigner.formAction = FormAction.Edit;
            _bindingSource = bs;
            //_bindingSource.DataSource = this._data.DsData.Tables[0];
            AddICDTData(_data);
            _frmDesigner.bindingSource = _bindingSource;
            dataNavigatorMain.DataSource = _frmDesigner.bindingSource;
            dxErrorProviderMain.DataSource = _frmDesigner.bindingSource;
            this._data.DsData.Tables[0].ColumnChanged += Table_ColumnChanged;
            //_data.DsData.Tables[0].Rows[_data.DsData.Tables[0].Rows.Count - 1]["MaKH"] = "XXXX";

            InitializeLayout();
            this.Load += new EventHandler(FrmSingleDt_Load);
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
            else
                this.dataNavigatorMain.TextStringFormat = "Mục {0} của {1}";
            dataNavigatorMain.PositionChanged += new EventHandler(dataNavigatorMain_PositionChanged);
            this._data.DsData.Tables[0].ColumnChanged += Table_ColumnChanged;
            foreach (ICDTData pl in _lstICDTData)
                pl.AddEvent();
        }

        private void Table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            foreach (DataRow dr in _data.DsStruct.Tables[0].Rows)
            {
                if (dr["Editable"] == DBNull.Value || dr["Editable"].ToString() == "0" || dr["Editable"].ToString() == "1") { }
                else if (dr["Editable"].ToString().ToUpper().Contains(e.Column.ColumnName.ToUpper()))
                    setEdit(dr["Editable"].ToString(), dr["FieldName"].ToString());
                if (dr["Visible"] == DBNull.Value || dr["Visible"].ToString() == "0" || dr["Visible"].ToString() == "1") { }
                else if (dr["Visible"].ToString().ToUpper().Contains(e.Column.ColumnName.ToUpper()))
                    setVisible(dr["Visible"].ToString(), dr["FieldName"].ToString());
            }
        }

        public FrmSingleDt(FormDesigner frmDesigner)
        {
            InitializeComponent();

            this._data = frmDesigner.Data;

            this._frmDesigner = frmDesigner;
            AddICDTData(_data);
            this._bindingSource = frmDesigner.bindingSource;
            dataNavigatorMain.DataSource = this._bindingSource;
            dxErrorProviderMain.DataSource = this._bindingSource;
            this._data.DsData.Tables[0].ColumnChanged += Table_ColumnChanged;
            InitializeLayout();
            this.Load += new EventHandler(FrmSingleDt_Load);
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
            dataNavigatorMain.PositionChanged += new EventHandler(dataNavigatorMain_PositionChanged);
            this._data.DsData.Tables[0].ColumnChanged += Table_ColumnChanged;
            foreach (ICDTData pl in _lstICDTData)
                pl.AddEvent();
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
            
            GridControl gcTmp = null;
            lcMain = new LayoutControl();
            string path;
            string English = Config.GetValue("Language").ToString() == "1" ? "_E" : "";



            if (Config.GetValue("DuongDanLayout") == null)
                path = Application.StartupPath + "\\Layouts\\" + Config.GetValue("Package").ToString() + English + "\\" + _data.DrTable["TableName"].ToString() + ".xml";
            else
                path = Config.GetValue("DuongDanLayout").ToString() + "\\" + Config.GetValue("Package").ToString() + English + "\\" + _data.DrTable["TableName"].ToString() + ".xml";
            lcMain = _frmDesigner.GenLayout3(ref gcTmp, true);
            //if (fieldCount > 12)
            //    lcMain = _frmDesigner.GenLayout3(ref gcTmp, true);
            //else
            //    lcMain = _frmDesigner.GenLayout2(ref gcTmp, true);
            if (_data.DrTable["FileLayout" + English] == DBNull.Value)
            {
                if (System.IO.File.Exists(path))
                {
                    lcMain.RestoreLayoutFromXml(path);
                    //UpLoad Layout to database
                    System.IO.MemoryStream ms = new MemoryStream();
                    lcMain.SaveLayoutToStream(ms);
                    _data.DrTable["FileLayout" + English] = ms.ToArray();
                    _data.updateLayoutFile(_data.DrTable);
                    lcMain.ShowCustomization += lcMain_ShowCustomization;
                }
            }
            else
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream(_data.DrTable["FileLayout" + English] as byte[]);
                XmlDocument xmlDoc = new XmlDocument();

                // Tạo XmlReader từ MemoryStream
                using (XmlReader xmlReader = XmlReader.Create(ms))
                {
                    xmlDoc.Load(xmlReader);
                }
                using (MemoryStream modifiedMemoryStream = new MemoryStream())
                {
                    xmlDoc.Save(modifiedMemoryStream);

                    XmlNodeList nodes = xmlDoc.SelectNodes("//property[@name='Visibility'][. = 'OnlyInCustomization']");
                    foreach (XmlNode node in nodes)
                    {
                        node.InnerText = "Always";
                    }
                }
                // Đưa vị trí của MemoryStream về đầu để chuẩn bị cho việc đọc
                using (MemoryStream ms1 = new MemoryStream())
                {
                    xmlDoc.Save(ms1);

                    // Đưa vị trí của MemoryStream về đầu để chuẩn bị cho việc đọc
                    ms1.Position = 0;

                    // Hiển thị nội dung mới trong MemoryStream
                    //using (StreamReader reader = new StreamReader(ms1))
                    //{
                    //    string modifiedXmlContent = reader.ReadToEnd();
                    //    Console.WriteLine("Nội dung XML sau khi thay đổi:");
                    //    Console.WriteLine(modifiedXmlContent);
                    //}
                    try
                    {
                        lcMain.RestoreLayoutFromStream(ms1);

                    }
                    catch
                    {
                        lcMain.RestoreLayoutFromStream(ms);
                    }
                }
                
               
               
                
                
                lcMain.ShowCustomization += lcMain_ShowCustomization;

            }

            lcMain.MouseUp += FrmSingleDt_MouseUp;
            this.Controls.Add(lcMain);
        }
        void lcMain_ShowCustomization(object sender, EventArgs e)
        {
            LayoutControl lcM = (sender as LayoutControl);

            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            if (!admin)
            {
                lcM.CustomizationForm.Close();
                return;
            }

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
                //if (dxErrorProviderMain.HasErrors)
                //{
                //    XtraMessageBox.Show("Số liệu chưa hợp lệ, vui lòng kiểm tra lại trước khi lưu!");
                //    return;
                //}

                if (this._data.UpdateData(dataAction))
                {
                    _x = false;
                    this.DialogResult = DialogResult.OK;
                }
                else if (dxErrorProviderMain.HasErrors)
                {
                    string Mess = "Số liệu chưa hợp lệ, vui lòng kiểm tra lại trước khi lưu!";
                    if (Config.GetValue("Language").ToString() == "1")
                        Mess = "Data is not valid, please check data before saving!";
                    XtraMessageBox.Show(Mess);
                }
                else
                {
                    string Mess = "Số liệu chưa hợp lệ, vui lòng kiểm tra lại trước khi lưu! ";
                    if (Config.GetValue("Language").ToString() == "1")
                        Mess = "Data is not valid, please check data before saving!, Check authorities";
                    XtraMessageBox.Show(Mess);
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
            {
                string Mess = "Số liệu chưa được lưu, bạn có thật sự muốn quay ra?";
                if (Config.GetValue("Language").ToString() == "1")
                    Mess = "Data is not saved, Do you want to cancel?";

                if (XtraMessageBox.Show(Mess, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _data.CancelUpdate();
                    if (_data.dataType == DataType.Single)
                    {
                        if (this._frmDesigner.formAction == FormAction.New)
                        {
                            _bindingSource.RemoveCurrent();
                            _bindingSource.DataSource = this._data.DsData.Tables[0];
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
                    this._data.DsData.Tables[0].ColumnChanged += Table_ColumnChanged;
                    foreach (ICDTData pl in _lstICDTData)
                        pl.AddEvent();
                    this.DialogResult = DialogResult.Cancel;
                }
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
            //_data.DrCurrentMaster["MaVT"] = "02AHR00STxxHS0001";
            //_data.DrCurrentMaster.EndEdit();
           DataRowView drv = (_bindingSource.Current as DataRowView);
            if (drv != null)
                this._data.DrCurrentMaster = drv.Row;
            this._data.Get_fileData4Record();
            for(int i=0; i< this._data._fileData.Count; i++)
           
            {
                    FileData fData = this._data._fileData[i];
                foreach (fileContener fC in this._frmDesigner._lFileContener)
                {
                    if (fC.drField["sysFieldID"].ToString() == fData.drField["sysFieldID"].ToString())
                    {
                        fC.data = fData.fData;
                    }
                }

                foreach (ImageContener IC in this._frmDesigner._lImageContener)
                {
                    if (IC.drField["sysFieldID"].ToString() == fData.drField["sysFieldID"].ToString())
                    {
                        IC.data = fData.fData;
                        IC.EditValue = fData.fData;
                        drv[IC.drField["FieldName"].ToString()] = IC.data;
                        drv.EndEdit();
                    }
                }
            }
            //Set Allow edit
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            if (!admin)
            {
                foreach (DataRow dr in _data.DsStruct.Tables[0].Rows)
                {
                    if (this._frmDesigner.formAction == FormAction.View)
                    {
                        setEdit("1=0", dr["FieldName"].ToString());
                        continue;
                    }
                    if (dr["Editable1"] != DBNull.Value) continue;
                    if (dr["Editable"] == DBNull.Value)
                    {
                        setEdit("1=0", dr["FieldName"].ToString());
                    }
                    else if (dr["Editable"].ToString() == "0")
                    {
                        setEdit("1=0", dr["FieldName"].ToString());
                    }
                    else if (dr["Editable"].ToString() == "1")
                    {
                        setEdit("1=1", dr["FieldName"].ToString());
                    }
                    else
                    {
                        try
                        {
                            setEdit(dr["Editable"].ToString(), dr["FieldName"].ToString());
                        }
                        catch
                        {
                        }
                    }
                    //Set Visible
                    // if (dr["Visible1"] != DBNull.Value) continue;
                    if (dr["Visible"] == DBNull.Value)
                    {
                        setVisible("1=0", dr["FieldName"].ToString());
                    }
                    else if (dr["Visible"].ToString() == "0")
                    {
                        setVisible("1=0", dr["FieldName"].ToString());
                    }
                    else if (dr["Visible"].ToString() == "1")
                    {
                        setVisible("1=1", dr["FieldName"].ToString());
                    }
                    else
                    {
                        try
                        {
                            setVisible(dr["Visible"].ToString(), dr["FieldName"].ToString());
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
        void setEdit(string condition, string fieldName)
        {
            if (_data.DrCurrentMaster == null) return;

            DataTable tableTmp = _data.DrCurrentMaster.Table.Clone();
            DataRow drtmp = tableTmp.NewRow();
            drtmp.ItemArray = _data.DrCurrentMaster.ItemArray;
            drtmp.EndEdit();
            //tableTmp.Rows.Add(drtmp);
            tableTmp.Rows.Add(drtmp);
            tableTmp.AcceptChanges();
            if (drtmp[_data.PkMaster.FieldName] == DBNull.Value || drtmp[_data.PkMaster.FieldName].ToString() == string.Empty) return;
            DataRow[] lstDr = tableTmp.Select("(" + condition + ") and " + _data.PkMaster.FieldName + "=" + _data.quote + drtmp[_data.PkMaster.FieldName].ToString() + _data.quote);
            BaseEdit it = null;
            foreach (BaseEdit be in _frmDesigner._BaseList)
            {
                if (be.Name == fieldName || be.Name == fieldName + "001")
                {

                    if (lstDr.Length == 0)
                    {
                        be.Properties.ReadOnly = true;
                    }
                    else
                    {
                        be.Properties.ReadOnly = false;
                    }
                }
            }


        }
        void setVisible(string condition, string fieldName)
        {
            if (_data.DrCurrentMaster == null) return;

            DataTable tableTmp = _data.DrCurrentMaster.Table.Clone();
            DataRow drtmp = tableTmp.NewRow();
            drtmp.ItemArray = _data.DrCurrentMaster.ItemArray;
            drtmp.EndEdit(); tableTmp.Rows.Add(drtmp);
            tableTmp.AcceptChanges();
            if (drtmp[_data.PkMaster.FieldName] == DBNull.Value || drtmp[_data.PkMaster.FieldName].ToString() == string.Empty) return;

            DataRow[] lstDr = tableTmp.Select("(" + condition + ") and " + _data.PkMaster.FieldName + "= " + _data.quote + drtmp[_data.PkMaster.FieldName].ToString() + _data.quote) ;
            BaseEdit it = null;


            foreach (BaseLayoutItem l in lcMain.Items)
            {
                LayoutControlItem li = l as LayoutControlItem;
                if (li == null || li.Control == null) continue;
                if (li.Control.Name == fieldName || li.Control.Name == fieldName + "001")
                {
                    if (lstDr.Length > 0)
                    {
                        li.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                    else
                    {
                        li.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.OnlyInCustomization;

                    }
                }
            }

        }
        private void FrmSingleDt_Load(object sender, EventArgs e)
        {
            _x = true;
            SetCurrentData();
            _bindingSource.EndEdit();
            //_data.DrCurrentMaster.EndEdit();
            
            if (_frmDesigner.formAction == FormAction.Copy)
            {
                _data.CloneData();
                if (_bindingSource.Count != _data.DsData.Tables[0].Rows.Count)
                { _bindingSource.DataSource = _data.DsData.Tables[0]; }
                        
                //_bindingSource.Position = _data.DsData.Tables[0].Rows.Count - 1;
            }
            _frmDesigner.RefreshViewForLookup();
            
        }

        void FrmSingleDt_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.pMInsetToDetail.ShowPopup(new Point(e.X, e.Y + 50 ));
            }
        }

        private void FrmSingleDt_KeyDown(object sender, KeyEventArgs e)
    {
            switch (e.KeyCode)
            {
                //case Keys.PageUp:
                //    if (e.Modifiers == Keys.Control)
                //        dataNavigatorMain.Buttons.First.DoClick();
                //    else
                //        dataNavigatorMain.Buttons.Prev.DoClick();
                //    break;
                //case Keys.PageDown:
                //    if (e.Modifiers == Keys.Control)
                //        dataNavigatorMain.Buttons.Last.DoClick();
                //    else
                //        dataNavigatorMain.Buttons.Next.DoClick();
                //    break;
                case Keys.Escape:
                    CancelUpdate();
                    break;
                case Keys.F12:
                    UpdateData();
                    break;
                case Keys.S:
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {
                        UpdateData();                       
                    }
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

        private void FrmSingleDt_Load_1(object sender, EventArgs e)
        {

        }

        private void btSaveLayout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                lcMain.SaveLayoutToStream(ms);
                string English = Config.GetValue("Language").ToString() == "1" ? "_E" : "";
                _data.DrTable["FileLayout" + English] = ms.ToArray();
                _data.updateLayoutFile(_data.DrTable);
            }
            catch { }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            oFD.ShowDialog();
            if (oFD.FileName != string.Empty)
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                lcMain.RestoreLayoutFromXml(oFD.FileName);
                lcMain.SaveLayoutToStream(ms);
                //UpLoad Layout to database
                string English = Config.GetValue("Language").ToString() == "1" ? "_E" : "";
                _data.DrTable["FileLayout" + English] = ms.ToArray();
                _data.updateLayoutFile(_data.DrTable);
            }
        }
    }
}
