using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using System.Data;
using CDTLib;
namespace CusAccounting
{
    public class DbTaoPTT
    {
        public DataTable MT31;
        public DataTable dmVt1;
        public DataTable dmVt2;
        public DataTable dmmon1;
        public DataTable dmmon2;
        public DataTable dmmon3;
        public DataTable dmmon4;
        public DataTable dmmon5;
        int MaxRand1 = 0;
        int MaxRand2 = 0;
        int MaxRand3 = 0;
        int MaxRand4 = 0;
        int MaxRand5 = 0;
        int MaxRandTU;
        int MaxRandNN;
        public DataTable dmHH0;
        public DataTable dmHH1;
        public DataTable dmHH5;
        public DataTable MT37;
        public DataTable DtThucUong;
        public DataTable DtThucAn;
        private DataRow drmaster;
        public DataRow drMaster
        {
            get { return drmaster; }
            set
            {
                drmaster = value;
                if (drmaster != null)
                    tinhTile();
            }
        }
        public Database db = Database.NewDataDatabase();
        public double TileTU = 60;
        public double Saisochapnhan = 1000;
        public double SaitileChapnhan = 10;
        public double GtLe = 200000;
        public double suatan = 12000;
        public double CurTileTU = 0;
        public double CurTileTA = 0;
        public double tileTU = 0;
        public double tileTA = 0;
        public double TongtileTU = 0;
        public double TongtileTA = 0;
        public event EventHandler sumChange;
        private double sumtu;
        private double sumta;
        public double SumPNTA
        {
            get { return sumta; }
            set { sumta = value; }

        }
        public double SumPNTU
        {
            get { return sumtu; }
            set { sumtu = value; }

        }
        public DbTaoPTT()
        {
            string sql = "select * from wTonkhotucthoi where isDV=0 and tkkho like '156%'";
            dmVt1 = db.GetDataTable(sql);
            sql = "select * from wTonkhotucthoi where isDV=1";
            dmVt2 = db.GetDataTable(sql);
            sql = "select *,9999999999 as minRand,9999999999 as maxRand   from wTonkhotucthoi where isDV=1 and loaimon=1 and tisuat>0";
            dmmon1 = db.GetDataTable(sql);
            sql = "select *,9999999999 as minRand,9999999999 as maxRand  from wTonkhotucthoi where isDV=1 and loaimon=2 and tisuat>0";
            dmmon2 = db.GetDataTable(sql);
            sql = "select *,9999999999 as minRand,9999999999 as maxRand  from wTonkhotucthoi where isDV=1 and loaimon=3 and tisuat>0";
            dmmon3 = db.GetDataTable(sql);
            sql = "select *,9999999999 as minRand,9999999999 as maxRand  from wTonkhotucthoi where isDV=1 and loaimon=4 and tisuat>0";
            dmmon4 = db.GetDataTable(sql);
            sql = "select *,9999999999 as minRand,9999999999 as maxRand  from wTonkhotucthoi where isDV=1 and loaimon=5 and tisuat>0";
            dmmon5 = db.GetDataTable(sql);
            sql = "select *,9999999999 as minRand,9999999999 as maxRand  from wTonkhotucthoi where isDV=0 and tkkho like '156%' and loaimon=0 and tisuat>0";
            dmHH0 = db.GetDataTable(sql);
            sql = "select *,9999999999 as minRand,9999999999 as maxRand  from wTonkhotucthoi where isDV=0 and tkkho like '156%' and loaimon=1 and tisuat>0";
            dmHH1 = db.GetDataTable(sql);

            sql = "select *,9999999999 as minRand,9999999999 as maxRand from wTonkhotucthoi where isDV=0 and tkkho like '156%' and loaimon=5 and tisuat>0";
            dmHH5 = db.GetDataTable(sql);

            for (int i = 0; i < dmmon1.Rows.Count; i++)
            {
                //MaxRand1 = 0;
                DataRow dr = dmmon1.Rows[i];
                dr["minRand"] = MaxRand1;
                MaxRand1 = MaxRand1 + (int)(double.Parse(dr["tisuat"].ToString()));
                dr["maxRand"] = MaxRand1;
            }
            for (int i = 0; i < dmmon2.Rows.Count; i++)
            {
                DataRow dr = dmmon2.Rows[i];
                dr["minRand"] = MaxRand2;
                MaxRand2 = MaxRand2 + (int)(double.Parse(dr["tisuat"].ToString()));
                dr["maxRand"] = MaxRand2;
            }
            for (int i = 0; i < dmmon3.Rows.Count; i++)
            {
                DataRow dr = dmmon3.Rows[i];
                dr["minRand"] = MaxRand3;
                MaxRand3 = MaxRand3 + (int)(double.Parse(dr["tisuat"].ToString()));
                dr["maxRand"] = MaxRand3;
            }
            for (int i = 0; i < dmmon4.Rows.Count; i++)
            {
                DataRow dr = dmmon4.Rows[i];
                dr["minRand"] = MaxRand4;
                MaxRand4 = MaxRand4 + (int)(double.Parse(dr["tisuat"].ToString()));
                dr["maxRand"] = MaxRand4;
            }
            for (int i = 0; i < dmmon5.Rows.Count; i++)
            {
                DataRow dr = dmmon5.Rows[i];
                dr["minRand"] = MaxRand5;
                MaxRand5 = MaxRand5 + (int)(double.Parse(dr["tisuat"].ToString()));
                dr["maxRand"] = MaxRand5;
            }
            for (int i = 0; i < dmHH1.Rows.Count; i++)
            {
                //MaxRand1 = 0;
                DataRow dr = dmHH1.Rows[i];
                dr["minRand"] = MaxRandTU;
                MaxRandTU = MaxRandTU + (int)(double.Parse(dr["tisuat"].ToString()));
                dr["maxRand"] = MaxRandTU;
            }
            for (int i = 0; i < dmHH5.Rows.Count; i++)
            {
                //MaxRand1 = 0;
                DataRow dr = dmHH5.Rows[i];
                dr["minRand"] = MaxRandNN;
                MaxRandNN = MaxRandNN + (int)(double.Parse(dr["tisuat"].ToString()));
                dr["maxRand"] = MaxRandNN;
            }
        }
        public void Getdata()
        {
            string sql = "select * from wmt31 order by ngayct, sohoadon";
            MT31 = db.GetDataTable(sql);
            if (MT31.Rows.Count > 0) drMaster = MT31.Rows[0];
            sql = "select * from dt37 where 1=0";
            DtThucUong = db.GetDataTable(sql);
            DtThucAn = db.GetDataTable(sql);
            DtThucAn.ColumnChanged += new DataColumnChangeEventHandler(DtThucAn_ColumnChanged);
            DtThucAn.RowDeleted += new DataRowChangeEventHandler(DtThucAn_RowDeleted);
            DtThucAn.Columns["PSNT"].DefaultValue = 0;
            DtThucAn.Columns["Soluong"].DefaultValue = 0;
            DtThucAn.Columns["GiaNT"].DefaultValue = 0;
            DtThucUong.ColumnChanged += new DataColumnChangeEventHandler(DtThucUong_ColumnChanged);
            DtThucUong.RowDeleted += new DataRowChangeEventHandler(DtThucUong_RowDeleted);
            DtThucUong.Columns["PSNT"].DefaultValue = 0;
            DtThucUong.Columns["Soluong"].DefaultValue = 0;
            DtThucUong.Columns["GiaNT"].DefaultValue = 0;
            object o = db.GetValueByStore("GetSuatAn", new string[] { "sa" }, new object[] { 0 }, new ParameterDirection[] { ParameterDirection.Output }, 0);
            if (o != null && double.Parse(o.ToString()) > 0) suatan = double.Parse(o.ToString());
        }

        void DtThucUong_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            sumtu = 0;
            foreach (DataRow dr in DtThucUong.Rows)
            {
                sumtu += double.Parse(dr["PSNT"].ToString());
            }
            tinhTile();
            sumChange(sumtu, new EventArgs());
        }

        void DtThucAn_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            sumta = 0;
            foreach (DataRow dr in DtThucAn.Rows)
            {
                sumta += double.Parse(dr["PSNT"].ToString());
            }
            tinhTile();
            sumChange(sumta, new EventArgs());
        }



        void DtThucUong_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (e.Column.ColumnName.ToUpper() == "SOLUONG" || e.Column.ColumnName.ToUpper() == "GIANT")
                {
                    e.Row["PSNT"] = double.Parse(e.Row["Soluong"].ToString()) * double.Parse(e.Row["GiaNT"].ToString());
                }
                if (e.Column.ColumnName.ToUpper() == "MAVT")
                {
                    DataRow[] lstdr = dmVt1.Select("MaVT='" + e.Row["MaVT"].ToString() + "'");
                    if (lstdr.Length > 0)
                    {
                        e.Row["GiaNT"] = double.Parse(lstdr[0]["GiaBan"].ToString());
                    }
                }
                if (e.Column.ColumnName.ToUpper() == "PSNT")
                {

                    if (e.Row.RowState == DataRowState.Detached) DtThucUong.Rows.Add(e.Row);
                    sumtu = 0;
                    e.Row.EndEdit();
                    foreach (DataRow dr in DtThucUong.Rows)
                    {
                        sumtu += double.Parse(dr["PSNT"].ToString());

                    }
                    tinhTile();


                }
            }
            catch
            { }
        }

        void DtThucAn_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (e.Column.ColumnName.ToUpper() == "SOLUONG" || e.Column.ColumnName.ToUpper() == "GIANT")
                {
                    e.Row["PSNT"] = double.Parse(e.Row["Soluong"].ToString()) * double.Parse(e.Row["GiaNT"].ToString());
                }
                if (e.Column.ColumnName.ToUpper() == "MAVT")
                {
                    DataRow[] lstdr = dmVt2.Select("MaVT='" + e.Row["MaVT"].ToString() + "'");
                    if (lstdr.Length > 0)
                    {
                        e.Row["GiaNT"] = double.Parse(lstdr[0]["GiaBan"].ToString());
                    }
                }
                if (e.Column.ColumnName.ToUpper() == "PSNT")
                {
                    if (e.Row.RowState == DataRowState.Detached) DtThucAn.Rows.Add(e.Row);
                    sumta = 0;
                    e.Row.EndEdit();
                    foreach (DataRow dr in DtThucAn.Rows)
                    {
                        sumta += double.Parse(dr["PSNT"].ToString());
                    }
                    tinhTile();

                }
            }
            catch { }
        }
        public void tinhTile()
        {
            if (drMaster == null || drMaster.RowState == DataRowState.Deleted) return;
            if (sumta + sumtu != 0)
            {
                tileTU = 100 * sumtu / (sumtu + sumta);
                tileTA = 100 * sumta / (sumtu + sumta);
            }
            else
            {
                tileTA = 0;
                tileTU = 0;
            }
            try
            {
                TongtileTA = 100 * (double.Parse(drmaster["TienTA"].ToString()) + sumta) / double.Parse(drmaster["TTien"].ToString());
                TongtileTU = 100 * (double.Parse(drmaster["TienTU"].ToString()) + sumtu) / double.Parse(drmaster["TTien"].ToString());

            }
            catch { }
            CurTileTA = 100 * sumta / double.Parse(drMaster["TTien"].ToString());
            CurTileTU = 100 * sumtu / double.Parse(drMaster["TTien"].ToString());
            if (sumChange != null)
                sumChange(sumta, new EventArgs());
        }
        public bool CreatePTT()
        {
            string sql = "";
            db.BeginMultiTrans();

            Guid id = Guid.NewGuid();

            db.UpdateDatabyStore("TaoPTT", new string[] { "@mt31id", "@TTien", "@id" }, new object[] { drMaster["MT31ID"].ToString(), sumta + sumtu, id });

            //sql = "select mt37id from mt37 where mt31id='" + drMaster["MT31ID"].ToString() +"'";
            //object o = db.GetValue(sql);
            //if (o == null)
            //{
            //    db.RollbackMultiTrans();
            //     db.HasErrors = false;
            //     return false;
            //}
            foreach (DataRow dr in DtThucAn.Rows)
            {
                db.UpdateDatabyStore("taoChitietPTT", new string[] { "@mt37id", "@MaVT", "@Soluong", "@Giant", "@psnt" },
                     new object[] { id.ToString(), dr["MaVT"], dr["Soluong"], dr["GiaNT"], dr["PSNT"] });
                if (db.HasErrors)
                {
                    // db.HasErrors = false;
                    db.RollbackMultiTrans();
                    return false;
                }
            }
            foreach (DataRow dr in DtThucUong.Rows)
            {
                db.UpdateDatabyStore("taoChitietPTT", new string[] { "@mt37id", "@MaVT", "@Soluong", "@Giant", "@psnt" },
                     new object[] { id.ToString(), dr["MaVT"], dr["Soluong"], dr["GiaNT"], dr["PSNT"] });
                if (db.HasErrors)
                {
                    // db.HasErrors = false;
                    db.RollbackMultiTrans();
                    return false;
                }
            }
            if (db.HasErrors)
            {
                //db.HasErrors = false;
                db.RollbackMultiTrans();
                return false;
            }
            else
            {
                // db.HasErrors = false;
                db.EndMultiTrans();
                drmaster["DataoPTT"] = double.Parse(drmaster["DataoPTT"].ToString()) + sumta + sumtu;
                drmaster["TienTA"] = double.Parse(drmaster["TienTA"].ToString()) + sumta;
                drmaster["TienTU"] = double.Parse(drmaster["TienTU"].ToString()) + sumtu;
                drmaster["Conlai"] = double.Parse(drmaster["Ttien"].ToString()) - double.Parse(drmaster["DataoPTT"].ToString());
                return true;
            }
            //insert vào phiếu xuất mt37
            //insert vào dt37
            //insert vào bltk
            //insert v
        }

        internal bool checkRule()
        {
            if (bool.Parse(drMaster["Banle"].ToString()))
            {
                if (sumta + sumtu >= GtLe) return false;
            }
            else
             if (Math.Abs(double.Parse(drMaster["TTien"].ToString()) - sumta - sumtu) > Saisochapnhan) return false;
            //if (Math.Abs(tileTU - TileTU) > SaitileChapnhan) return false;
            foreach (DataRow dr in DtThucUong.Rows)
            {
                if (dr["MaVT"] == DBNull.Value)
                    dr.SetColumnError("MaVT", "Phải nhập");
                else dr.SetColumnError("MaVT", string.Empty);
                if (dr["Soluong"] == DBNull.Value || double.Parse(dr["Soluong"].ToString()) == 0)
                    dr.SetColumnError("Soluong", "Phải nhập");
                else dr.SetColumnError("Soluong", string.Empty);
                if (dr["GiaNT"] == DBNull.Value || double.Parse(dr["GiaNT"].ToString()) == 0)
                    dr.SetColumnError("GiaNT", "Phải nhập");
                else dr.SetColumnError("GiaNT", string.Empty);
                if (dr["PSNT"] == DBNull.Value || double.Parse(dr["PSNT"].ToString()) == 0)
                    dr.SetColumnError("PSNT", "Phải nhập");
                else dr.SetColumnError("PSNT", string.Empty);

            }
            if (DtThucUong.HasErrors) return false;
            foreach (DataRow dr in DtThucAn.Rows)
            {
                if (dr["MaVT"] == DBNull.Value)
                    dr.SetColumnError("MaVT", "Phải nhập");
                else dr.SetColumnError("MaVT", string.Empty);
                if (dr["Soluong"] == DBNull.Value || double.Parse(dr["Soluong"].ToString()) == 0)
                    dr.SetColumnError("Soluong", "Phải nhập");
                else dr.SetColumnError("Soluong", string.Empty);
                if (dr["GiaNT"] == DBNull.Value || double.Parse(dr["GiaNT"].ToString()) == 0)
                    dr.SetColumnError("GiaNT", "Phải nhập");
                else dr.SetColumnError("GiaNT", string.Empty);
                if (dr["PSNT"] == DBNull.Value || double.Parse(dr["PSNT"].ToString()) == 0)
                    dr.SetColumnError("PSNT", "Phải nhập");
                else dr.SetColumnError("PSNT", string.Empty);

            }
            if (DtThucAn.HasErrors) return false;
            return true;
        }

        internal void autoRun(double Tongtien)
        {
            DtThucUong.Rows.Clear();
            DtThucAn.Rows.Clear();
            //double Tongtien = double.Parse(drMasterLe["TTien"].ToString());
            double songuoi = 0;
            songuoi = Math.Round(Tongtien * (100 - TileTU) / (suatan * 100), 0);
            int sn = (int)songuoi;
            if (sn == 0) sn = 1;
            //Chọn thức ăn
            switch (sn)
            {
                case 1:
                    chonMonan(1, 1); chonMonan(1, 3);
                    break;
                case 2:
                    chonMonan(1, 1); chonMonan(1, 3);
                    break;
                case 3:
                    chonMonan(1, 1); chonMonan(1, 4);
                    break;
                case 4:
                    chonMonan(1, 1); chonMonan(1, 2); chonMonan(1, 4);
                    break;
                case 5:
                    chonMonan(2, 1); chonMonan(1, 2); chonMonan(1, 3); chonMonan(1, 4);
                    break;
                case 6:
                    chonMonan(2, 1); chonMonan(1, 2); chonMonan(1, 3); chonMonan(1, 4); chonMonan(1, 5);
                    break;
                case 7:
                    chonMonan(2, 1); chonMonan(1, 2); chonMonan(2, 3); chonMonan(1, 4); chonMonan(1, 5);
                    break;
                default:
                    if (sn < 12)
                    {
                        chonMonan(Math.Round(songuoi / 6, 0), 1); chonMonan(Math.Round(songuoi / 6, 0), 2); chonMonan(Math.Round(songuoi / 6, 0), 3); chonMonan(Math.Round(songuoi / 6, 0), 4); chonMonan(Math.Round(songuoi / 6, 0), 5);
                    }
                    else if (sn < 16)
                    {
                        chonMonan(Math.Round(songuoi / 6, 0), 1); chonMonan(Math.Round(songuoi / 6, 0), 2); chonMonan(Math.Round(songuoi / 6, 0), 3); chonMonan(Math.Round(songuoi / 6, 0), 3); chonMonan(Math.Round(songuoi / 6, 0), 4); chonMonan(Math.Round(songuoi / 6, 0), 5);
                    }
                    else if (sn < 24)
                    {
                        chonMonan(Math.Round(songuoi / 6, 0), 1); chonMonan(Math.Round(songuoi / 8, 0), 1); chonMonan(Math.Round(songuoi / 4, 0), 2); chonMonan(Math.Round(songuoi / 6, 0), 3); chonMonan(Math.Round(songuoi / 8, 0), 3); chonMonan(Math.Round(songuoi / 6, 0), 4); chonMonan(Math.Round(songuoi / 6, 0), 5);
                    }
                    else
                    {
                        chonMonan(Math.Round(songuoi / 6, 0), 1); chonMonan(Math.Round(songuoi / 8, 0), 1); chonMonan(Math.Round(songuoi / 8, 0), 1); chonMonan(Math.Round(songuoi / 6, 0), 2); chonMonan(Math.Round(songuoi / 6, 0), 3); chonMonan(Math.Round(songuoi / 8, 0), 3); chonMonan(Math.Round(songuoi / 6, 0), 4); chonMonan(Math.Round(songuoi / 6, 0), 5);
                    }
                    break;
            }
            //Chọn thức uống
            Random rand = new Random();
            double TienTU = Tongtien - sumta;

            int stt = rand.Next(MaxRandTU);
            string maTU = "";
            double giaTU = 1;
            DataRow[] ldr = dmHH1.Select("minRand<=" + stt.ToString() + " and maxRand>" + stt.ToString());
            if (ldr.Length > 0)
            {
                maTU = ldr[0]["MaVT"].ToString();
                giaTU = double.Parse(ldr[0]["GiaBan"].ToString());
            }



            double soluongTU = 0;
            int nuocngot = rand.Next(10);
            if (nuocngot < 7)
                soluongTU = (int)Math.Round(0.9 * TienTU / giaTU, 0);
            else
                soluongTU = (int)Math.Round(0.95 * TienTU / giaTU, 0);

            if (soluongTU > 0) AddTUong(maTU, soluongTU);
            //Chọn nước ngọt
            double conlai = Tongtien - sumta - sumtu;
            if (nuocngot < 7)
            {
                double giaNN = 1;
                string maNN = "";
                stt = rand.Next(MaxRandNN);
                DataRow[] ldrNN = dmHH5.Select("minRand<=" + stt.ToString() + " and maxRand>" + stt.ToString());
                if (ldrNN.Length > 0)
                {
                    maNN = ldrNN[0]["MaVT"].ToString();
                    giaNN = double.Parse(ldrNN[0]["GiaBan"].ToString());
                    double slnngot = Math.Round(0.7 * conlai / giaNN, 0);
                    if (maNN != "" && slnngot > 0) AddTUong(maNN, slnngot);
                    conlai = Tongtien - sumta - sumtu;
                }

            }
            //Chọn các đồ linh tinh 
            //1.Thuốc ngựa
            if (dmHH0.Select("MaVT='NGUALON'").Length > 0 && soluongTU > 30)
            {
                double giaNL = double.Parse(dmHH0.Select("MaVT='NGUALON'")[0]["Giaban"].ToString());
                if (conlai > giaNL)
                {
                    AddTUong("NGUALON", 1);
                    conlai = Tongtien - sumta - sumtu;
                }
            }
            double mindu = conlai;
            string MAVT = "";
            double GIABAN = 1;
            double sl = 0;
            for (int i = 0; i < 100; i++)
            {
                DataRow dr = dmHH0.Rows[rand.Next(dmHH0.Rows.Count)];

                string mavt = dr["MaVT"].ToString();
                double Giaban = double.Parse(dr["Giaban"].ToString());
                if (mavt == "NGUALON") continue;
                double sodu = conlai % Giaban;
                if (sodu < mindu)
                {
                    mindu = sodu;
                    MAVT = mavt;
                    GIABAN = Giaban;
                }
                if (mindu == 0)
                {
                    sl = Math.Round(conlai / Giaban, 0);
                    if (sl > 0 && mavt != "") AddTUong(mavt, sl);
                    conlai = Tongtien - sumta - sumtu;
                    break;
                }
            }
            if (conlai > 0)
            {
                sl = Math.Round(conlai / GIABAN, 0);
                if (sl > 0 && MAVT != "") AddTUong(MAVT, sl);
            }


        }
        private void chonMonan(double soluong, int loaimon)
        {
            Random rand = new Random();
            int stt; string mavt;
            string dvt;
            switch (loaimon)
            {
                case 1:
                    for (int i = 0; i < 5; i++)
                    {
                        stt = rand.Next(MaxRand1);
                        DataRow[] ldr = dmmon1.Select("minRand<=" + stt.ToString() + " and maxRand>" + stt.ToString());
                        if (ldr.Length > 0)
                        {
                            mavt = ldr[0]["Mavt"].ToString();
                            dvt = ldr[0]["Madvt"].ToString();
                            if (dvt == "KG") soluong = Math.Round(soluong, 0) / 2;
                            if (DtThucAn.Select("MaVT='" + mavt + "'").Length == 0)
                            {
                                AddMonan(mavt, soluong);
                                break;
                            }
                        }
                        else continue;
                    }
                    break;
                case 2:
                    for (int i = 0; i < 5; i++)
                    {
                        stt = rand.Next(MaxRand2);
                        DataRow[] ldr = dmmon2.Select("minRand<=" + stt.ToString() + " and maxRand>" + stt.ToString());
                        if (ldr.Length > 0)
                        {
                            mavt = ldr[0]["Mavt"].ToString();
                            dvt = ldr[0]["Madvt"].ToString();
                            if (dvt == "KG") soluong = Math.Round(soluong, 0) / 2;
                            if (DtThucAn.Select("MaVT='" + mavt + "'").Length == 0)
                            {
                                AddMonan(mavt, soluong);
                                break;
                            }
                        }
                        else continue;
                    }
                    break;
                case 3:
                    for (int i = 0; i < 5; i++)
                    {
                        stt = rand.Next(MaxRand3);
                        DataRow[] ldr = dmmon3.Select("minRand<=" + stt.ToString() + " and maxRand>" + stt.ToString());
                        if (ldr.Length > 0)
                        {
                            mavt = ldr[0]["Mavt"].ToString();
                            dvt = ldr[0]["Madvt"].ToString();
                            if (dvt == "KG") soluong = Math.Round(soluong, 0) / 2;
                            if (DtThucAn.Select("MaVT='" + mavt + "'").Length == 0)
                            {
                                AddMonan(mavt, soluong);
                                break;
                            }
                        }
                        else continue;
                    }
                    break;
                case 4:
                    for (int i = 0; i < 5; i++)
                    {
                        stt = rand.Next(MaxRand4);
                        DataRow[] ldr = dmmon4.Select("minRand<=" + stt.ToString() + " and maxRand>" + stt.ToString());
                        if (ldr.Length > 0)
                        {
                            mavt = ldr[0]["Mavt"].ToString();
                            dvt = ldr[0]["Madvt"].ToString();
                            if (dvt == "KG") soluong = Math.Round(soluong, 0) / 2;
                            if (DtThucAn.Select("MaVT='" + mavt + "'").Length == 0)
                            {
                                AddMonan(mavt, soluong);
                                break;
                            }
                        }
                        else continue;
                    }
                    break;
                case 5:
                    for (int i = 0; i < 5; i++)
                    {
                        stt = rand.Next(MaxRand5);
                        DataRow[] ldr = dmmon5.Select("minRand<=" + stt.ToString() + " and maxRand>" + stt.ToString());
                        if (ldr.Length > 0)
                        {
                            mavt = ldr[0]["Mavt"].ToString();
                            dvt = ldr[0]["Madvt"].ToString();
                            if (dvt == "KG") soluong = Math.Round(soluong, 0) / 2;
                            if (DtThucAn.Select("MaVT='" + mavt + "'").Length == 0)
                            {
                                AddMonan(mavt, soluong);
                                break;
                            }
                        }
                        else continue;
                    }
                    break;
            }
        }
        private void AddMonan(string mavt, double soluong)
        {
            DataRow drTA = DtThucAn.NewRow();
            drTA["MaVT"] = mavt;
            drTA["soluong"] = soluong;

        }
        private void AddTUong(string mavt, double soluong)
        {
            DataRow drTU = DtThucUong.NewRow();
            drTU["MaVT"] = mavt;
            drTU["soluong"] = soluong;

        }
    }
}
