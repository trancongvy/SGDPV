using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
namespace CDTControl
{
    public class ImportExcel
    {
        string fileName;
        Workbook workbook;
        public Application application = new ApplicationClass();
        public System.Data.DataTable Db;
        public ImportExcel(string FileName)
        {
            fileName = FileName;

        }

        public List<string> GetSheets()
        {
            List<string> list = new List<string>();
            try
            {
                workbook = application.Workbooks.Open(fileName, ReadOnly: true);
                foreach (Worksheet worksheet in workbook.Sheets)
                {
                    list.Add(worksheet.Name);

                }
                workbook.Close(false, false, false);
                //application.Quit();
                return list;
            }catch
            {
                list.Add("FileOpening");
                System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
                return list;
            }
            finally
            {
                application.Quit();
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
            }

        }



        public List<string> GetCol(string p)
        {
            List<string> Collist = new List<string>();
            try
            {
                workbook = application.Workbooks.Open(fileName, Type.Missing, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
               

                foreach (Worksheet worksheet in workbook.Sheets)
                {

                    if (worksheet.Name == p)
                    {
                        Db = new System.Data.DataTable();
                        for (int i = 65; i <= 298; i++)
                        {
                            int i1 = (i - 65) / 26;
                            int i2 = (i - 65) % 26;
                            string c1 = "";
                            string c2 = "";
                            if (i1 == 0) c1 = "";
                            else c1 = ((char)(i1 + 64)).ToString();
                            c2 = ((char)(i2 + 65)).ToString();
                            string c = c1 + c2;

                            Range r = worksheet.get_Range(c + "1");
                            string t = r.Text.ToString().Trim();
                            Range r1 = worksheet.get_Range(c + "2");
                            Type type = r1.Text.GetType();
                            decimal result;
                            DateTime date;

                            //if (DateTime.TryParse(r1.Text.ToString(), out date))
                            //{
                            //    type = typeof(DateTime);
                            //}
                            //else if (decimal.TryParse(r1.Text.ToString(), out result) && !r1.Text.ToString().StartsWith("0"))
                            //{
                            //    type = typeof(decimal);
                            //}
                            //else
                            //{
                            //    type = typeof(string);
                            //}


                            if (t.Trim() != "")
                            {
                                Collist.Add(t.Trim());
                                System.Data.DataColumn col = new DataColumn(t.Trim(), type);
                                Db.Columns.Add(col);
                            }
                            else
                            {
                                break;
                            }
                        }
                        for (int j = 2; j < 35000; j++)
                        {
                            DataRow dr = Db.NewRow();
                            for (int i = 0; i < Db.Columns.Count; i++)
                            {
                                int i1 = (i) / 26;
                                int i2 = (i) % 26;
                                string c1 = "";
                                string c2 = "";
                                if (i1 == 0) c1 = "";
                                else c1 = ((char)(i1 + 64)).ToString();
                                c2 = ((char)(i2 + 65)).ToString();
                                string c = c1 + c2;
                                Range r = worksheet.get_Range(c + j.ToString());
                                string t = r.Text.ToString();
                                Type type = Db.Columns[i].DataType;

                                if (t.Trim() != "")
                                {
                                    dr[i] = r.Text;
                                }
                                if (t.Trim() == "" && i == 0)
                                {
                                    j = 35000;
                                    i = Db.Columns.Count;
                                }

                            }
                            if (j < 35000)
                                Db.Rows.Add(dr);

                        }
                        break;
                    }


                }
                workbook.Close(false, false, false);
               application.Quit();
            }
            catch (Exception ex)
                {

            }
             finally
            {
               // application.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
            }

            //OleDbDataAdapter ole = new OleDbDataAdapter(); ;
            //try
            //{
            //    string selectConnectionString = "provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + this.fileName + "';Extended Properties=Excel 14.0;";
            //    ole = new OleDbDataAdapter("select * from [" + p + "$]", selectConnectionString);
            //    Db = new System.Data.DataTable();

            //    ole.Fill(Db);
            //    for (int i = 0; i < Db.Columns.Count; i++)
            //    {
            //        Collist.Add(Db.Columns[i].ColumnName);
            //    }
            //    ole.SelectCommand.Connection.Close();
            //}
            //catch (Exception ex) {

            //    ole.SelectCommand.Connection.Close(); }
            return Collist;
        }

    }
}
