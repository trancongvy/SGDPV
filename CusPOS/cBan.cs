using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
using CDTControl;
using CDTLib;
namespace CusPOS
{
    public partial class cBan :DevExpress.XtraEditors.XtraUserControl
    {
       public DataTable tb;

        Database db = Database.NewDataDatabase();
        public string tSoBan = "";
        public double tTienH = 0;
        public double tTien = 0;
        public Guid id;
        public string MaKH;
        public bool LayHD = false;
        public int HTTT = 0;
        public string TK;
        private double _ptck = 0;
        public string SoCT;
        public event EventHandler TienChange;
        public string MaLoaiGia="BT";
        public string MaCa;
        public double TTTM = 0;
        public double TTThe = 0;
        public string Ghichu;
        public bool isPrinted = false;
        public double PtPhi
        {
            get { return _ptphi; }
            set
            {
                _ptphi = value;
                PhiThe = Math.Round(TTThe * _ptphi / 100, 0);
            }
        }
        double _ptphi = 0;
        public string MaLoaiThe = null;
        public double PhiThe
        {
            get { return _phithe; }
            set { _phithe = value;
                TienThe = TTThe - _phithe;
            }
        }
        double _phithe = 0;
        public double TienThe = 0;
        public double PtCK
        {
            get { return _ptck; }
            set { _ptck = value;
                GetSum();
            }
        }

        public string MaPOSLoaiGia { get; internal set; }
        public string MaPOSArea;
        public double Ck;
        public cBan(string soban)
        {
            InitializeComponent();
            tSoBan = soban;
            this.Width = 146;
            this.Height = 110;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.MouseMove += new MouseEventHandler(cBan_MouseHover);
            this.MouseLeave += new EventHandler(cBan_MouseLeave);
            this.MouseHover += new EventHandler(cBan_MouseHover);
            this.MouseDown += new MouseEventHandler(cBan_MouseDown);
            this.MouseUp += new MouseEventHandler(cBan_MouseUp);
            this.Click += new EventHandler(cBan_Click);
            getdata();
        }

        private void getdata()
        {
            string sql = "select a.mamon,b.tenmon,a.PtCkCt,a.CkCt, soluong, dongia, thanhtien, ctposid, c.*, LayHD from ctpos a inner join dmmon b on a.mamon=b.mamon inner join  mtpos c on a.mtposid=c.mtposid where  c.maban='" + tSoBan + "' and (c.DaTT=0 or c.daTT is null) ";
            tb = db.GetDataTable(sql);
            if (tb.Rows.Count > 0)
            {
                id = Guid.Parse( tb.Rows[0]["mtposid"].ToString());
                MaKH = tb.Rows[0]["MaKH"].ToString();LayHD = bool.Parse(tb.Rows[0]["LayHD"].ToString());
                PtCK= double.Parse(tb.Rows[0]["Ptck"].ToString());
                SoCT = tb.Rows[0]["SoPhieu"].ToString();
                TK= tb.Rows[0]["Tk"]==DBNull.Value? null: tb.Rows[0]["Tk"].ToString();
                Ghichu= tb.Rows[0]["Ghichu"] == DBNull.Value ? "" : tb.Rows[0]["Ghichu"].ToString();
                isPrinted= tb.Rows[0]["Printed"] == DBNull.Value ? false : bool.Parse(tb.Rows[0]["Printed"].ToString());
            }
            else
            {
                id = Guid.NewGuid(); MaKH = "KL"; LayHD = false;PtCK = 0;Ghichu = "";TK = "1111";
                
            }
            GetSum();
            tb.ColumnChanged += new DataColumnChangeEventHandler(tb_ColumnChanged);
        }
        internal void Save()
        {
            //Kiểm tra bàn này có chưa
            if (Config.GetValue("MaCa") != null)
            {
                MaCa = Config.GetValue("MaCa").ToString();
            }
            else
            { MaCa = null; }
            db.BeginMultiTrans();
            try
            {
                string sql = "select count(mtposid) from mtpos where mtposid='" + id.ToString() + "'";
                object ex = db.GetValue(sql);
                if (tb.Rows.Count == 0) return;
                if (ex.ToString() == "" || ex.ToString() == "0") //chưa tồn tại
                {
                    sql = "select max(sophieu) from mtpos where year(ngayct)=year(getdate()) and month(ngayct)=month(getdate())";
                    object soct = db.GetValue(sql);
                    if (soct == null || soct == DBNull.Value)
                    {
                        SoCT = "PTT" + DateTime.Now.Year.ToString("00").Substring(2, 2) + DateTime.Now.Month.ToString("00") + "00001";
                    }
                    else
                    {
                        SoCT = "PTT" + DateTime.Now.Year.ToString("00").Substring(2, 2) + DateTime.Now.Month.ToString("00") + (int.Parse(soct.ToString().Substring(7, 5)) + 1).ToString("00000");
                    }

                    sql = "insert into Mtpos (MTPOSID, ngayct,Sophieu, Maban, TTien, TTienH, Ptck, Ck, Datt, MaKH, layHD, Tk,MaCa, TTTM, TTThe, PhiThe, TienThe, LoaiThe, Ghichu,Printed) values(";
                    sql += "@MTPOSID, @ngayct,@Sophieu, @Maban, @TTien, @TTienH, @Ptck, @Ck, @Datt, @MaKH, @layHD, @Tk, @MaCa, @TTTM, @TTThe, @PhiThe, @TienThe, @LoaiThe,@Ghichu,@Printed)";
                    string[] para = new string[] { "@MTPOSID", "@ngayct", "@Sophieu", "@Maban", "@TTien", "@TTienH", "@Ptck", "@Ck", "@Datt", "@MaKH", "@layHD", "@Tk", "@MaCa", "@TTTM", "@TTThe", "@PhiThe", "@TienThe", "@LoaiThe", "@Ghichu", "@Printed"};

                    object[] values = new object[] { id, DateTime.Now, SoCT, tSoBan, tTien, tTienH, PtCK, Ck, 0, (object)MaKH ?? "KL", LayHD, (object)TK ?? DBNull.Value, (object)MaCa ?? DBNull.Value, TTTM, TTThe, PhiThe, TienThe, (object)MaLoaiThe ?? DBNull.Value,Ghichu,0 };
                    
                    db.UpdateDatabyPara(sql, para, values);
                    if (db.HasErrors)
                    {
                        db.RollbackMultiTrans();
                        return;
                    }
                }
                else //đã tồn tại
                {
                    sql = "update mtpos set ngayct=@ngayct, TTienH= @TTienH, TTien=@tTien,  PtCk=@PtCK, Ck=@Ck, MaKH=@MaKH, LayHD=@LayHD, Tk=@TK, MaCa=@MaCa,TTTM=@TTTM, TTThe=@TTThe, PhiThe=@PhiThe, TienThe=@TienThe, LoaiThe=@LoaiThe,Ghichu=@Ghichu";
                    sql += " where mtposid=@MTPOSID";
                    string[] para1 = new string[] { "@MTPOSID", "@ngayct",   "@TTien", "@TTienH", "@Ptck", "@Ck", "@Datt", "@MaKH", "@layHD", "@Tk", "@MaCa", "@TTTM", "@TTThe", "@PhiThe", "@TienThe", "@LoaiThe", "@Ghichu" };
                    object[] values1 = new object[] { id, DateTime.Now,  tTien, tTienH, PtCK, Ck, 0, (object)MaKH ?? "KL", LayHD, (object)TK??DBNull.Value, (object)MaCa ?? DBNull.Value, TTTM, TTThe, PhiThe, TienThe, (object)MaLoaiThe ?? DBNull.Value ,Ghichu};

                    db.UpdateDatabyPara(sql, para1, values1);
                    if (db.HasErrors)
                    {
                        db.RollbackMultiTrans();
                        return;
                    }
                }
                foreach (DataRow dr in tb.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        dr.RejectChanges();
                        string ctid = dr["ctposid"].ToString();
                        sql = "delete ctpos where ctposid='" + ctid + "'";
                        db.UpdateByNonQuery(sql);
                        dr.Delete();
                        if (db.HasErrors)
                        {
                            db.RollbackMultiTrans();
                            return;
                        }
                    }
                    else
                    {
                        sql = "select count(ctposid) from ctpos where ctposid='" + dr["ctposid"].ToString() + "'";
                        object exct = db.GetValue(sql);
                        if (exct.ToString() == "" || exct.ToString() == "0") //chưa tồn tại
                        {
                            sql = "insert into ctpos(mtposid, ctposid,maban,mamon, soluong, dongia, thanhtien,datt, ptckct, ckct) values ('";
                            sql += id.ToString() + "','" + dr["ctposid"].ToString() + "','" + tSoBan + "','" + dr["mamon"].ToString() + "'," + dr["soluong"].ToString() + "," + dr["dongia"].ToString() + "," + dr["thanhtien"].ToString() + ",0," + dr["PtckCT"].ToString() + "," + dr["ckCT"].ToString() +" )";
                            db.UpdateByNonQuery(sql);
                            if (db.HasErrors)
                            {
                                db.RollbackMultiTrans();
                                return;
                            }
                        }
                        else
                        {
                            sql = "update ctpos set soluong=" + dr["soluong"].ToString() + ", thanhtien=" + dr["thanhtien"].ToString() + ", PtCkCt=" + dr["PtckCT"].ToString() + ", CkCt=" + dr["ckCT"].ToString() + " where ctposid='" + dr["ctposid"].ToString() + "'";
                            db.UpdateByNonQuery(sql);
                            if (db.HasErrors)
                            {
                                db.RollbackMultiTrans();
                                return;
                            }
                        }
                    }
                }//end for
                db.EndMultiTrans();
                tb.AcceptChanges();
            }catch
            {
                MessageBox.Show("udpate không thành công");
            }
        }
        internal void Thanhtoan()
        {

            this.Save();
           // fThanhtoan f = new fThanhtoan();
           // f.ShowDialog();
           // if (f.returnValue == -1) return;
            string sql ;
           if ( HTTT== 0 || HTTT == 1)
            {
                sql = "Update mtpos set ngayct=getdate(),DaTT=1, tk='" + TK + "' where MTPOSID='" + id.ToString() + "'";
            }
            else
            {
                sql = "Update mtpos set ngayct=getdate(),DaTT=1,Maphong='" + TK + "' where MTPOSID='" + id.ToString() + "'";
            }
            db.UpdateByNonQuery(sql);
            this.getdata();

        }
        void tb_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (isPrinted) tb.RejectChanges();
            try
            {
                if (e.Column.ColumnName == "soluong")
                {
                   // GetSum();
                }
            }
            catch { }
        }

        public void GetSum()
        {
            tTienH = 0;
            Ck = 0;
            foreach (DataRow dr in tb.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (PtCK == 0)
                {
                    dr["thanhtien"] = double.Parse(dr["Soluong"].ToString()) * double.Parse(dr["Dongia"].ToString());
                    dr["Ckct"]= Math.Round(double.Parse(dr["thanhtien"].ToString()) * double.Parse(dr["ptckct"].ToString()) / 100000, 0) * 1000;
                    Ck += double.Parse(dr["Ckct"].ToString());
                }
                // dr["thanhtien"] = double.Parse(e.Row["dongia"].ToString()) * double.Parse(e.Row["soluong"].ToString());
                if (dr["thanhtien"] == DBNull.Value) dr["thanhtien"] = 0;
                tTienH += double.Parse(dr["thanhtien"].ToString());
            }
            if (PtCK > 0) Ck = Math.Round(tTienH * PtCK / 100000, 0)*1000;
            tTien = tTienH - Ck;
            if (TienChange != null)
                TienChange(this, new EventArgs());
        }
        public event EventHandler ChonBan;
        void cBan_Click(object sender, EventArgs e)
        {
            ChonBan(this, e);
        }

        #region Hiệu ứng
        
        void cBan_MouseUp(object sender, MouseEventArgs e)
        {
            Graphics g = this.CreateGraphics();
            LinearGradientBrush myBrush = null;
            myBrush = new LinearGradientBrush(ClientRectangle, Color.AliceBlue, Color.CornflowerBlue, LinearGradientMode.ForwardDiagonal);
            g.FillRectangle(myBrush, ClientRectangle);
            Brush brush = new SolidBrush(Color.Navy);

            g.DrawString("Bàn số", new Font("Times New Roman", 10), brush, new PointF(20, 17));
            g.DrawString(tSoBan, new Font("Times New Roman", 15, FontStyle.Bold), brush, new PointF(60, 13));
            g.DrawString("Số tiền", new Font("Times New Roman", 10), brush, new PointF(20, 41));
            brush = new SolidBrush(Color.Red);
            g.DrawString(tTien.ToString("### ### ### ###"), new Font("Times New Roman", 15, FontStyle.Bold), brush, new PointF(20, 64));

        }

        void cBan_MouseDown(object sender, MouseEventArgs e)
        {
            Graphics g = this.CreateGraphics();
            LinearGradientBrush myBrush = null;
            myBrush = new LinearGradientBrush(ClientRectangle,   Color.AliceBlue, Color.CornflowerBlue,LinearGradientMode.BackwardDiagonal);
            g.FillRectangle(myBrush, ClientRectangle);
            Brush brush = new SolidBrush(Color.Navy);

            g.DrawString("Bàn số", new Font("Times New Roman", 10), brush, new PointF(20, 17));
            g.DrawString(tSoBan, new Font("Times New Roman", 15, FontStyle.Bold), brush, new PointF(60, 13));
            g.DrawString("Số tiền", new Font("Times New Roman", 10), brush, new PointF(20, 41));
            brush = new SolidBrush(Color.Red);
            g.DrawString(tTien.ToString("### ### ### ###"), new Font("Times New Roman", 15, FontStyle.Bold), brush, new PointF(20, 64));

        }

        void cBan_MouseHover(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            LinearGradientBrush myBrush = null;
            myBrush = new LinearGradientBrush(ClientRectangle, Color.CornflowerBlue, Color.AliceBlue,  LinearGradientMode.ForwardDiagonal);
            g.FillRectangle(myBrush, ClientRectangle);
             Brush brush = new SolidBrush(Color.Navy);
            
            g.DrawString("Bàn số", new Font("Times New Roman", 10), brush, new PointF(20, 17));
            g.DrawString(tSoBan, new Font("Times New Roman", 15,FontStyle.Bold), brush, new PointF(60, 13));
            g.DrawString("Số tiền", new Font("Times New Roman", 10), brush, new PointF(20, 41));
            brush = new SolidBrush(Color.Red);
            g.DrawString(tTien.ToString("### ### ### ###"), new Font("Times New Roman", 15, FontStyle.Bold), brush, new PointF(20, 64));

        }

        internal void Printed()
        {
            string sql = "update mtpos set Printed=1 where MTPOSID='" + id.ToString() + "'";
            db.UpdateByNonQuery(sql);
            isPrinted = true;
        }
        
        void cBan_MouseLeave(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            LinearGradientBrush myBrush = null;
            myBrush = new LinearGradientBrush(ClientRectangle, Color.AliceBlue, Color.CornflowerBlue, LinearGradientMode.ForwardDiagonal);
            g.FillRectangle(myBrush, ClientRectangle);
            Brush brush = new SolidBrush(Color.Navy);
           
            g.DrawString("Bàn số", new Font("Times New Roman", 10), brush, new PointF(20, 17));
            g.DrawString(tSoBan, new Font("Times New Roman", 12), brush, new PointF(76, 13));
            g.DrawString("Số tiền", new Font("Times New Roman", 10), brush, new PointF(20, 41));
            brush = new SolidBrush(Color.Red);
            g.DrawString(tTien.ToString("### ### ### ###"), new Font("Times New Roman", 15, FontStyle.Bold), brush, new PointF(20, 64));

        }
        
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            LinearGradientBrush myBrush = null;
            myBrush = new LinearGradientBrush(ClientRectangle, Color.AliceBlue, Color.CornflowerBlue, LinearGradientMode.ForwardDiagonal);
            pevent.Graphics.FillRectangle(myBrush, ClientRectangle);
            Brush brush = new SolidBrush(Color.Navy);
            Graphics g = this.CreateGraphics();
            g.DrawString("Bàn số", new Font("Times New Roman", 10), brush, new PointF(20, 17));
            g.DrawString(tSoBan, new Font("Times New Roman", 12), brush, new PointF(76, 13));
            g.DrawString("Số tiền", new Font("Times New Roman", 10), brush, new PointF(20, 41));
            brush = new SolidBrush(Color.Red);
            g.DrawString(tTien.ToString("### ### ### ###"), new Font("Times New Roman", 15, FontStyle.Bold), brush, new PointF(20, 64));

        }
        #endregion








        
    }
}
