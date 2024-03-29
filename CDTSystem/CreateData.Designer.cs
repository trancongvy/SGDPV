using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using System.ComponentModel;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace CDTSystem
{
    partial class CreateData
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.fGTTu = new DevExpress.XtraEditors.RadioGroup();
            this.sbInstallSQL = new DevExpress.XtraEditors.SimpleButton();
            this.CkUpdateLocal = new DevExpress.XtraEditors.CheckEdit();
            this.ckUpdateRemote = new DevExpress.XtraEditors.CheckEdit();
            this.txtRemoteServer = new DevExpress.XtraEditors.TextEdit();
            this.cEis2005 = new DevExpress.XtraEditors.CheckEdit();
            this.txtCDT = new DevExpress.XtraEditors.TextEdit();
            this.radioGroupCnnType = new DevExpress.XtraEditors.RadioGroup();
            this.simpleButtonOk = new DevExpress.XtraEditors.SimpleButton();
            this.textEditPassword = new DevExpress.XtraEditors.TextEdit();
            this.textEditUser = new DevExpress.XtraEditors.TextEdit();
            this.textEditServer = new DevExpress.XtraEditors.TextEdit();
            this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.radioGroupType = new DevExpress.XtraEditors.RadioGroup();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.tlo = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxErrorProviderMain = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fGTTu.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CkUpdateLocal.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckUpdateRemote.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRemoteServer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cEis2005.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCDT.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroupCnnType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditServer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroupType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.fGTTu);
            this.layoutControl1.Controls.Add(this.sbInstallSQL);
            this.layoutControl1.Controls.Add(this.CkUpdateLocal);
            this.layoutControl1.Controls.Add(this.ckUpdateRemote);
            this.layoutControl1.Controls.Add(this.txtRemoteServer);
            this.layoutControl1.Controls.Add(this.cEis2005);
            this.layoutControl1.Controls.Add(this.txtCDT);
            this.layoutControl1.Controls.Add(this.radioGroupCnnType);
            this.layoutControl1.Controls.Add(this.simpleButtonOk);
            this.layoutControl1.Controls.Add(this.textEditPassword);
            this.layoutControl1.Controls.Add(this.textEditUser);
            this.layoutControl1.Controls.Add(this.textEditServer);
            this.layoutControl1.Controls.Add(this.simpleButtonCancel);
            this.layoutControl1.Controls.Add(this.radioGroupType);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(864, 961, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(931, 493);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // fGTTu
            // 
            this.fGTTu.EditValue = 0;
            this.fGTTu.Location = new System.Drawing.Point(104, 138);
            this.fGTTu.Name = "fGTTu";
            this.fGTTu.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(0, "Theo quyết định 133"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(1, "Theo thông tư 200")});
            this.fGTTu.Size = new System.Drawing.Size(757, 44);
            this.fGTTu.StyleController = this.layoutControl1;
            this.fGTTu.TabIndex = 5;
            // 
            // sbInstallSQL
            // 
            this.sbInstallSQL.Location = new System.Drawing.Point(19, 3);
            this.sbInstallSQL.Name = "sbInstallSQL";
            this.sbInstallSQL.Size = new System.Drawing.Size(842, 22);
            this.sbInstallSQL.StyleController = this.layoutControl1;
            this.sbInstallSQL.TabIndex = 15;
            this.sbInstallSQL.Text = "Máy bạn chưa cài SQL2005, Click vào đây để cài đặt!";
            this.sbInstallSQL.Click += new System.EventHandler(this.sbInstallSQL_Click);
            // 
            // CkUpdateLocal
            // 
            this.CkUpdateLocal.Location = new System.Drawing.Point(392, 78);
            this.CkUpdateLocal.Name = "CkUpdateLocal";
            this.CkUpdateLocal.Properties.Caption = "Cập nhật máy chủ local";
            this.CkUpdateLocal.Size = new System.Drawing.Size(469, 19);
            this.CkUpdateLocal.StyleController = this.layoutControl1;
            this.CkUpdateLocal.TabIndex = 17;
            // 
            // ckUpdateRemote
            // 
            this.ckUpdateRemote.Location = new System.Drawing.Point(394, 111);
            this.ckUpdateRemote.Name = "ckUpdateRemote";
            this.ckUpdateRemote.Properties.Caption = "Cập nhật máy chủ từ xa";
            this.ckUpdateRemote.Size = new System.Drawing.Size(464, 19);
            this.ckUpdateRemote.StyleController = this.layoutControl1;
            this.ckUpdateRemote.TabIndex = 16;
            // 
            // txtRemoteServer
            // 
            this.txtRemoteServer.EditValue = ".\\SQLSGD2005";
            this.txtRemoteServer.EnterMoveNextControl = true;
            this.txtRemoteServer.Location = new System.Drawing.Point(107, 111);
            this.txtRemoteServer.Name = "txtRemoteServer";
            this.txtRemoteServer.Size = new System.Drawing.Size(277, 20);
            this.txtRemoteServer.StyleController = this.layoutControl1;
            this.txtRemoteServer.TabIndex = 6;
            // 
            // cEis2005
            // 
            this.cEis2005.EditValue = true;
            this.cEis2005.Location = new System.Drawing.Point(22, 219);
            this.cEis2005.Name = "cEis2005";
            this.cEis2005.Properties.Caption = "Database server 2005";
            this.cEis2005.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.cEis2005.Size = new System.Drawing.Size(836, 19);
            this.cEis2005.StyleController = this.layoutControl1;
            this.cEis2005.TabIndex = 13;
            // 
            // txtCDT
            // 
            this.txtCDT.EditValue = "CDTT";
            this.txtCDT.Location = new System.Drawing.Point(107, 189);
            this.txtCDT.Name = "txtCDT";
            this.txtCDT.Size = new System.Drawing.Size(751, 20);
            this.txtCDT.StyleController = this.layoutControl1;
            this.txtCDT.TabIndex = 12;
            this.txtCDT.TabStop = false;
            this.txtCDT.EditValueChanged += new System.EventHandler(this.txtCDT_EditValueChanged);
            // 
            // radioGroupCnnType
            // 
            this.radioGroupCnnType.Location = new System.Drawing.Point(107, 248);
            this.radioGroupCnnType.Name = "radioGroupCnnType";
            this.radioGroupCnnType.Properties.Columns = 2;
            this.radioGroupCnnType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Thông qua SQL"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Thông qua Windows")});
            this.radioGroupCnnType.Size = new System.Drawing.Size(751, 137);
            this.radioGroupCnnType.StyleController = this.layoutControl1;
            this.radioGroupCnnType.TabIndex = 10;
            this.radioGroupCnnType.SelectedIndexChanged += new System.EventHandler(this.radioGroupCnnType_SelectedIndexChanged);
            // 
            // simpleButtonOk
            // 
            this.simpleButtonOk.Location = new System.Drawing.Point(22, 455);
            this.simpleButtonOk.Name = "simpleButtonOk";
            this.simpleButtonOk.Size = new System.Drawing.Size(381, 22);
            this.simpleButtonOk.StyleController = this.layoutControl1;
            this.simpleButtonOk.TabIndex = 8;
            this.simpleButtonOk.Text = "Chấp nhận";
            this.simpleButtonOk.Click += new System.EventHandler(this.simpleButtonOk_Click);
            // 
            // textEditPassword
            // 
            this.textEditPassword.EditValue = "passwordcongtysgd";
            this.textEditPassword.EnterMoveNextControl = true;
            this.textEditPassword.Location = new System.Drawing.Point(107, 425);
            this.textEditPassword.Name = "textEditPassword";
            this.textEditPassword.Properties.PasswordChar = '*';
            this.textEditPassword.Size = new System.Drawing.Size(751, 20);
            this.textEditPassword.StyleController = this.layoutControl1;
            this.textEditPassword.TabIndex = 7;
            this.textEditPassword.EditValueChanged += new System.EventHandler(this.textEditPassword_EditValueChanged);
            // 
            // textEditUser
            // 
            this.textEditUser.EditValue = "sa";
            this.textEditUser.EnterMoveNextControl = true;
            this.textEditUser.Location = new System.Drawing.Point(107, 395);
            this.textEditUser.Name = "textEditUser";
            this.textEditUser.Size = new System.Drawing.Size(751, 20);
            this.textEditUser.StyleController = this.layoutControl1;
            this.textEditUser.TabIndex = 6;
            this.textEditUser.EditValueChanged += new System.EventHandler(this.textEditUser_EditValueChanged);
            // 
            // textEditServer
            // 
            this.textEditServer.EditValue = ".\\SQLSGD2005";
            this.textEditServer.EnterMoveNextControl = true;
            this.textEditServer.Location = new System.Drawing.Point(107, 81);
            this.textEditServer.Name = "textEditServer";
            this.textEditServer.Size = new System.Drawing.Size(278, 20);
            this.textEditServer.StyleController = this.layoutControl1;
            this.textEditServer.TabIndex = 5;
            this.textEditServer.EditValueChanged += new System.EventHandler(this.textEditServer_EditValueChanged);
            // 
            // simpleButtonCancel
            // 
            this.simpleButtonCancel.Location = new System.Drawing.Point(413, 455);
            this.simpleButtonCancel.Name = "simpleButtonCancel";
            this.simpleButtonCancel.Size = new System.Drawing.Size(445, 22);
            this.simpleButtonCancel.StyleController = this.layoutControl1;
            this.simpleButtonCancel.TabIndex = 9;
            this.simpleButtonCancel.Text = "Bỏ qua";
            this.simpleButtonCancel.Click += new System.EventHandler(this.simpleButtonCancel_Click);
            // 
            // radioGroupType
            // 
            this.radioGroupType.Location = new System.Drawing.Point(107, 32);
            this.radioGroupType.Name = "radioGroupType";
            this.radioGroupType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Tạo số liệu trên máy này"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Kết nối với số liệu có sẵn")});
            this.radioGroupType.Size = new System.Drawing.Size(751, 39);
            this.radioGroupType.StyleController = this.layoutControl1;
            this.radioGroupType.TabIndex = 4;
            this.radioGroupType.SelectedIndexChanged += new System.EventHandler(this.radioGroupType_SelectedIndexChanged);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem2,
            this.layoutControlItem8,
            this.layoutControlItem9,
            this.layoutControlItem12,
            this.layoutControlItem13,
            this.tlo,
            this.layoutControlItem14,
            this.emptySpaceItem6,
            this.emptySpaceItem5,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem1,
            this.layoutControlItem11,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(931, 493);
            this.layoutControlGroup1.Text = "Root";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.textEditUser;
            this.layoutControlItem3.CustomizationFormText = "Tên đăng nhập";
            this.layoutControlItem3.Location = new System.Drawing.Point(16, 389);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem3.Size = new System.Drawing.Size(846, 30);
            this.layoutControlItem3.Text = "Tên đăng nhập";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(82, 13);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.textEditPassword;
            this.layoutControlItem4.CustomizationFormText = "Mật khẩu";
            this.layoutControlItem4.Location = new System.Drawing.Point(16, 419);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem4.Size = new System.Drawing.Size(846, 30);
            this.layoutControlItem4.Text = "Mật khẩu";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(82, 13);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.textEditServer;
            this.layoutControlItem2.CustomizationFormText = "Tên máy chủ SQL";
            this.layoutControlItem2.Location = new System.Drawing.Point(16, 75);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem2.Size = new System.Drawing.Size(373, 30);
            this.layoutControlItem2.Text = "Tên máy chủ";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(82, 13);
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.txtCDT;
            this.layoutControlItem8.CustomizationFormText = "Dữ liệu Struct";
            this.layoutControlItem8.Location = new System.Drawing.Point(16, 183);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem8.Size = new System.Drawing.Size(846, 30);
            this.layoutControlItem8.Text = "Dữ liệu Struct";
            this.layoutControlItem8.TextSize = new System.Drawing.Size(82, 13);
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.cEis2005;
            this.layoutControlItem9.CustomizationFormText = "layoutControlItem9";
            this.layoutControlItem9.Location = new System.Drawing.Point(16, 213);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem9.Size = new System.Drawing.Size(846, 29);
            this.layoutControlItem9.Text = "layoutControlItem9";
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextToControlDistance = 0;
            this.layoutControlItem9.TextVisible = false;
            this.layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.txtRemoteServer;
            this.layoutControlItem12.CustomizationFormText = "Máy chủ từ xa";
            this.layoutControlItem12.Location = new System.Drawing.Point(16, 105);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem12.Size = new System.Drawing.Size(372, 30);
            this.layoutControlItem12.Text = "Máy chủ từ xa";
            this.layoutControlItem12.TextSize = new System.Drawing.Size(82, 13);
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.ckUpdateRemote;
            this.layoutControlItem13.CustomizationFormText = "layoutControlItem13";
            this.layoutControlItem13.Location = new System.Drawing.Point(388, 105);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem13.Size = new System.Drawing.Size(474, 30);
            this.layoutControlItem13.Text = "layoutControlItem13";
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextToControlDistance = 0;
            this.layoutControlItem13.TextVisible = false;
            // 
            // tlo
            // 
            this.tlo.Control = this.CkUpdateLocal;
            this.tlo.CustomizationFormText = "tlo";
            this.tlo.Location = new System.Drawing.Point(389, 75);
            this.tlo.Name = "tlo";
            this.tlo.Size = new System.Drawing.Size(473, 30);
            this.tlo.Text = "tlo";
            this.tlo.TextSize = new System.Drawing.Size(0, 0);
            this.tlo.TextToControlDistance = 0;
            this.tlo.TextVisible = false;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.sbInstallSQL;
            this.layoutControlItem14.CustomizationFormText = "layoutControlItem14";
            this.layoutControlItem14.Location = new System.Drawing.Point(16, 0);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(846, 26);
            this.layoutControlItem14.Text = "layoutControlItem14";
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextToControlDistance = 0;
            this.layoutControlItem14.TextVisible = false;
            // 
            // emptySpaceItem6
            // 
            this.emptySpaceItem6.AllowHotTrack = false;
            this.emptySpaceItem6.CustomizationFormText = "emptySpaceItem6";
            this.emptySpaceItem6.Location = new System.Drawing.Point(862, 0);
            this.emptySpaceItem6.Name = "emptySpaceItem6";
            this.emptySpaceItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.emptySpaceItem6.Size = new System.Drawing.Size(67, 481);
            this.emptySpaceItem6.Text = "emptySpaceItem6";
            this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem5
            // 
            this.emptySpaceItem5.AllowHotTrack = false;
            this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem5";
            this.emptySpaceItem5.Location = new System.Drawing.Point(0, 0);
            this.emptySpaceItem5.Name = "emptySpaceItem5";
            this.emptySpaceItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.emptySpaceItem5.Size = new System.Drawing.Size(16, 481);
            this.emptySpaceItem5.Text = "emptySpaceItem5";
            this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.simpleButtonOk;
            this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
            this.layoutControlItem5.Location = new System.Drawing.Point(16, 449);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem5.Size = new System.Drawing.Size(391, 32);
            this.layoutControlItem5.Text = "layoutControlItem5";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextToControlDistance = 0;
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.simpleButtonCancel;
            this.layoutControlItem6.CustomizationFormText = "layoutControlItem6";
            this.layoutControlItem6.Location = new System.Drawing.Point(407, 449);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem6.Size = new System.Drawing.Size(455, 32);
            this.layoutControlItem6.Text = "layoutControlItem6";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextToControlDistance = 0;
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.radioGroupCnnType;
            this.layoutControlItem7.CustomizationFormText = "Loại số liệu";
            this.layoutControlItem7.Location = new System.Drawing.Point(16, 242);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem7.Size = new System.Drawing.Size(846, 147);
            this.layoutControlItem7.Text = "Loại kết nối";
            this.layoutControlItem7.TextSize = new System.Drawing.Size(82, 13);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.radioGroupType;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(16, 26);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem1.Size = new System.Drawing.Size(846, 49);
            this.layoutControlItem1.Text = "Cách khởi tạo";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(82, 13);
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.fGTTu;
            this.layoutControlItem11.CustomizationFormText = "Phân hệ kế toán:";
            this.layoutControlItem11.Location = new System.Drawing.Point(16, 135);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(846, 48);
            this.layoutControlItem11.Text = "Phân hệ kế toán:";
            this.layoutControlItem11.TextSize = new System.Drawing.Size(82, 13);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 481);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(929, 10);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // dxErrorProviderMain
            // 
            this.dxErrorProviderMain.ContainerControl = this;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.radioGroupType;
            this.layoutControlItem10.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem10.Location = new System.Drawing.Point(88, 36);
            this.layoutControlItem10.Name = "layoutControlItem1";
            this.layoutControlItem10.Size = new System.Drawing.Size(911, 101);
            this.layoutControlItem10.Text = "Cách khởi tạo";
            this.layoutControlItem10.TextSize = new System.Drawing.Size(143, 25);
            this.layoutControlItem10.TextToControlDistance = 5;
            // 
            // CreateData
            // 
            this.ClientSize = new System.Drawing.Size(931, 493);
            this.Controls.Add(this.layoutControl1);
            this.MaximizeBox = false;
            this.Name = "CreateData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Khởi tạo số liệu";
            this.Load += new System.EventHandler(this.CreateData_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fGTTu.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CkUpdateLocal.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckUpdateRemote.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRemoteServer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cEis2005.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCDT.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroupCnnType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditServer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroupType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProviderMain;
        private EmptySpaceItem emptySpaceItem5;
        private EmptySpaceItem emptySpaceItem6;
        
        private LayoutControl layoutControl1;
        private LayoutControlGroup layoutControlGroup1;
        private LayoutControlItem layoutControlItem1;
        private LayoutControlItem layoutControlItem2;
        private LayoutControlItem layoutControlItem3;
        private LayoutControlItem layoutControlItem4;
        private LayoutControlItem layoutControlItem5;
        private LayoutControlItem layoutControlItem6;
        private LayoutControlItem layoutControlItem7;
        private LayoutControlItem layoutControlItem8;
        private LayoutControlItem layoutControlItem9;
        private RadioGroup radioGroupCnnType;
        private RadioGroup radioGroupType;
        private SimpleButton simpleButtonCancel;
        private SimpleButton simpleButtonOk;
        private TextEdit textEditPassword;
        private TextEdit textEditServer;
        private TextEdit textEditUser;
        private TextEdit txtCDT;
        private TextEdit txtRemoteServer;
        private LayoutControlItem layoutControlItem12;
        private CheckEdit ckUpdateRemote;
        private LayoutControlItem layoutControlItem13;
        private CheckEdit CkUpdateLocal;
        private LayoutControlItem tlo;
        private SimpleButton sbInstallSQL;
        private LayoutControlItem layoutControlItem14;
        private RadioGroup fGTTu;
        private LayoutControlItem layoutControlItem11;
        private LayoutControlItem layoutControlItem10;
        private EmptySpaceItem emptySpaceItem1;
    }
}