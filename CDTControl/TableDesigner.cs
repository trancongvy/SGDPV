using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;

namespace CDTControl
{
    internal class TableDesigner
    {
        /// <summary>
        /// Lớp Database đi kèm
        /// </summary>
        private Database CurrentDb;
        /// <summary>
        /// Tên bảng đang mở
        /// </summary>
        private string TableName = string.Empty;
        /// <summary>
        /// Khóa chính của bảng
        /// </summary>
        private string Pk = string.Empty;

        public TableDesigner(Database currentDb, string tableName, string pk)
        {
            CurrentDb = currentDb;
            TableName = tableName;
            Pk = pk;
        }

        /// <summary>
        /// Kiếm tra dữ liệu trước khi cho tạo bảng
        /// </summary>
        public bool IsValid(DataRowView drvTable, DataView dvField)
        {
            string moTaLoi = string.Empty;
            string tenBang = "- Bảng " + drvTable["TableName"].ToString();
            string khoaChinh = drvTable["Pk"].ToString();
            int tableType = Int32.Parse(drvTable["Type"].ToString());
            bool validKhoaChinh = false;

            //kiểm tra trường bảng chính
            if (tableType >= 3 && tableType <= 5 && drvTable["MasterTable"].ToString() == string.Empty)
                moTaLoi += tenBang + ": cần xác định bảng chính \n";

            int i = 0;
            foreach (DataRowView drField in dvField)
            {
                i++;
                if (drField.Row.RowState == DataRowState.Deleted)
                    continue;
                string tenField = drField["FieldName"].ToString();
                //kiểm tra các giá trị null
                if (tenField == string.Empty)
                {
                    moTaLoi += tenBang + ": danh sách trường có trường chưa nhập tên trường \n";
                    continue;
                }
                tenField = "- Trường " + tenField;

                if (drField["Type"].ToString() == string.Empty)
                {
                    moTaLoi += tenField + ": chưa xác định giá trị Kiểu \n";
                    continue;
                }
                if (drField["AllowNull"].ToString() == string.Empty)
                    moTaLoi += tenField + ": chưa xác định giá trị Rỗng \n";
                if (drField["LabelName"].ToString() == string.Empty)
                    moTaLoi += tenField + ": chưa xác định giá trị Diễn giải cho trường \n";
                if (drField["TabIndex"].ToString() == string.Empty)
                    moTaLoi += tenField + ": chưa xác định giá trị Tab \n";
                if (drField["Visible"].ToString() == string.Empty)
                    moTaLoi += tenField + ": chưa xác định giá trị Hiện \n";
                if (drField["Editable"].ToString() == string.Empty)
                    moTaLoi += tenField + ": chưa xác định giá trị Sửa \n";

                int typeField = Int32.Parse(drField["Type"].ToString());
                if (drField["FieldName"].ToString().ToUpper() == khoaChinh.ToUpper())
                {
                    //kiểm tra tồn tại khóa chính
                    validKhoaChinh = true;

                    //kiểm tra khóa chính
                    if (typeField == 0 || typeField == 3 || typeField == 6)
                    {
                        if (drField["AllowNull"].ToString() == string.Empty || drField["AllowNull"].ToString()=="1")
                            moTaLoi += tenField + ": khóa chính không được rỗng \n";
                    }
                    else
                        moTaLoi += tenField + ": khóa chính phải thuộc một trong các kiểu 0, 3, 6 \n";
                }

                //kiểm tra mặc định
                if (drField["DefaultValue"].ToString() != string.Empty && drField["DefaultName"].ToString() == string.Empty)
                    drField["DefaultName"] = "DF_" + drvTable["TableName"].ToString() + "_" + drField["FieldName"].ToString();
                if (drField["DefaultValue"].ToString() != string.Empty && drField["AllowNull"].ToString() == string.Empty && Boolean.Parse(drField["AllowNull"].ToString()) != true)
                    moTaLoi += tenField + ": có giá trị mặc định thì không được rỗng \n";

                //kiểm tra khóa ngoại
                if (typeField == 1 || typeField == 4 || typeField == 7)
                {
                    if (drField["RefField"].ToString() == string.Empty
                        || drField["RefTable"].ToString() == string.Empty
                        || drField["CasUpdate"].ToString() == string.Empty
                        || drField["CasDelete"].ToString() == string.Empty)
                        moTaLoi += tenField + ": kiểu dữ liệu khóa ngoại cần xác định Trường tham chiếu, Bảng tham chiếu, \n" +
                            "Cập nhật tham chiếu, Xóa tham chiếu \n";
                    else
                        if (drField["RefName"].ToString() == string.Empty && drvTable.Row.RowState==DataRowState.Added)
                            if (XtraMessageBox.Show("Bạn có muốn tạo ràng buộc cho " + tenField + " trên SQL?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                drField["RefName"] = "FK_" + drvTable["TableName"].ToString() + "_" + drField["RefTable"].ToString() + i.ToString();
                }
                else
                    if (drField["RefField"].ToString() != string.Empty
                        && drField["RefTable"].ToString() != string.Empty
                        && drField["RefName"].ToString() != string.Empty)
                        moTaLoi += tenField + ": có thông tin về khóa ngoại (Trường tham chiếu, Bảng tham chiếu, Tên ràng buộc tham chiếu) \n " +
                            " nhưng kiểu dữ liệu không thuộc kiểu khóa ngoại (1,4,7) \n";
            }

            //kiểm tra tồn tại khóa chính
            if (!validKhoaChinh)
                moTaLoi += tenBang + ": cần kiểm tra lại khóa chính: chưa được mô tả trong danh sách trường";

            if (moTaLoi == string.Empty)
                return true;
            else
            {
                XtraMessageBox.Show(moTaLoi);
                return false;
            }
        }

        /// <summary>
        /// Phát sinh chuỗi cú pháp tạo bảng
        /// </summary>
        private string GenStructString(DataView drColumns)
        {
            string s = "create table " + TableName + "( ";
            for (int i = 0; i < drColumns.Count; i++)
            {
                DataRowView dr = drColumns[i];
                if (dr.Row.RowState == DataRowState.Deleted)
                    continue;
                s += GenField(dr, false) + ",";
               // if (i != drColumns.Count - 1)
                 //   s += ",";
            }
            s += " ws nvarchar(4000),Grws nvarchar(4000) )";
           // s += ")";
            return s;
        }

        /// <summary>
        /// Phát sinh chuỗi cú pháp tạo cột trong bảng từ số liệu 1 dòng trong sysField
        /// </summary>
        private string GenField(DataRowView dr, bool withConstraint)
        {
            string s = " " + dr["FieldName"].ToString() + " ";

            string strType = string.Empty;
            int pType = Int32.Parse(dr["Type"].ToString());
            //0: text(pk); 1: text(fk); 2: text; 3: int(pk); 4: int(fk); 5: int; 6: unique identifier; 
            //7: unique identifier(fk); 8: decimal; 9: date; 10: boolean; 11: time; 12: image;
            switch (pType)
            {
                case 0:
                case 1:
                    strType = "nvarchar(32)";
                    break;
                case 2:
                case 16:
                    strType = "nvarchar(128)";
                    break;
                case 3:
                    strType = "int IDENTITY ";
                    break;
                case 4:
                case 5:
                    strType = "int ";
                    break;
                case 6:
                case 7:
                case 15:
                    strType = "uniqueidentifier ";
                    break;
                case 8:
                    strType = "decimal(20,6)";
                    break;
                case 9:
                case 14:
                    strType = "smalldatetime";
                    break;
                case 10:
                    strType = "bit";
                    break;
                case 11:
                    strType = "smalldatetime";
                    break;
                case 12:  
                    strType = "image";
                    break;
                case 13:
                    strType = "ntext";
                    break;
            }
            s += strType;

            if (dr["AllowNull"].ToString() != string.Empty)
            {
                bool isNull = dr["AllowNull"].ToString()=="1";
                if (isNull == false)
                    s += " not null ";
            }

            if (withConstraint)
            {
                if (dr["DefaultValue"].ToString() != string.Empty)
                    s += " default " + dr["DefaultValue"].ToString();
                if (dr["RefTable"].ToString() != string.Empty)
                {
                    string RefTable = dr["RefTable"].ToString();
                    string RefField = dr["RefField"].ToString();
                    s += " references " + RefTable + "(" + RefField + ")";
                }
            }
            return s;
        }

        /// <summary>
        /// Tạo một bảng vào CSDL
        /// </summary>
        public bool CreateTable(DataView drColumns)
        {
            if (TableName == string.Empty || drColumns.Count == 0)
            {
                XtraMessageBox.Show("Thiếu tên bảng hoặc bảng không có cột nào!");
                return false;
            }
            string queryString = GenStructString(drColumns);
            if (!CurrentDb.UpdateByNonQuery(queryString))
                return false;

            //tiếp tục tạo các ràng buộc khóa chính, mặc định và khóa ngoại
            AddPrimary();
            bool result = true;
            foreach (DataRowView dr in drColumns)
            {
                if (dr.Row.RowState == DataRowState.Deleted)
                    continue;
                if (dr["RefName"].ToString() != string.Empty)
                    result = AddReference(dr);
                if (dr["DefaultName"].ToString() != string.Empty)
                    result = AddDefault(dr);
                if (!result)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Kiểm tra bảng có dữ liệu không
        /// </summary>
        private bool TableHasData()
        {
            string s = "select * from " + TableName;
            DataTable dt = CurrentDb.GetDataTable(s);
            if (dt != null)
                if (dt.Rows.Count > 0)
                    return true;
            return false;
        }

        /// <summary>
        /// Kiểm tra 1 cột có số liệu không
        /// </summary>
        private bool ColumnHasData(string ColumnName)
        {
            string s = "select * from " + TableName + " where " + ColumnName + " is not null";
            DataTable dt = CurrentDb.GetDataTable(s);
            if (dt != null)
                if (dt.Rows.Count > 0)
                    return true;
            return false;
        }

        /// <summary>
        /// Xóa bảng khỏi CSDL
        /// </summary>
        public bool DropTable(DataView drColumns)
        {
            if (TableHasData())
                if (XtraMessageBox.Show("Bảng đã có số liệu, vui lòng xác nhận xóa bảng này?", "Xac nhan", MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;
            // xóa các ràng buộc trên bảng trước
            if (!DeleteConstraint("PK_" + TableName))
                return false;

            bool resutl = true;
            foreach (DataRowView dr in drColumns)
            {
                if (dr["RefName"].ToString() != string.Empty)
                    resutl = DeleteConstraint(dr["RefName"].ToString());
                if (dr["DefaultName"].ToString() != string.Empty)
                    resutl = DeleteConstraint(dr["DefaultName"].ToString());
                if (!resutl)
                    return false;
            }

            return (CurrentDb.UpdateByNonQuery("DROP TABLE " + TableName));
        }

        /// <summary>
        /// Thêm một cột vào bảng trong CSDL
        /// </summary>
        public bool AddColumn(DataRowView drColumn, bool withConstraint)
        {
            string s = "ALTER TABLE " + TableName + " ADD ";
            s += GenField(drColumn, withConstraint);
            if (!CurrentDb.UpdateByNonQuery(s))
                return false;
            if (withConstraint)
                return true;
            if (drColumn["RefName"].ToString() != string.Empty)
                return (AddReference(drColumn));
            if (drColumn["DefaultName"].ToString() != string.Empty)
                return (AddDefault(drColumn));
            return true;
        }

        /// <summary>
        /// Sửa một cột trong bảng CSDL
        /// </summary>
        public bool AlterColumn(DataRowView drColumnNew, DataRowView drColumnOld)
        {
            if (drColumnNew["RefName"].ToString() != string.Empty && drColumnOld["RefName"].ToString() == string.Empty)
                return(AddReference(drColumnNew));
            //if (drColumnNew["DefaultName"].ToString() != string.Empty && drColumnOld["DefaultName"].ToString() == string.Empty )
            //   if(MessageBox.Show("Create Default?"+drColumnNew["fieldName"].ToString(),"?",MessageBoxButtons.YesNo)==DialogResult.Yes)
            //    return(AddDefault(drColumnNew));
            if (drColumnNew["RefName"].ToString() == string.Empty && drColumnOld["RefName"].ToString() != string.Empty)
                return(DeleteConstraint(drColumnOld["RefName"].ToString()));
            if (drColumnNew["DefaultName"].ToString() == string.Empty && drColumnOld["DefaultName"].ToString() != string.Empty)
                return(DeleteConstraint(drColumnOld["DefaultName"].ToString()));
            return true;
        }

        /// <summary>
        /// Xóa một cột khỏi bảng trong CSDL
        /// </summary>
        public bool DropColumn(string TableName, DataRowView drColumn)
        {
            string FieldName = drColumn["FieldName"].ToString();

            if (ColumnHasData(FieldName))
                if (XtraMessageBox.Show("Cột đã có số liệu, vui lòng xác nhận xóa cột này?", "Xac nhan", MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;

            string sql = "SELECT constraint_name FROM information_schema.constraint_column_usage WHERE table_name = '" + TableName + "' AND column_name = '" + FieldName + "'";
            DataTable tbConstraint = CurrentDb.GetDataTable(sql);
            foreach(DataRow dr in tbConstraint.Rows)
            {
                DeleteConstraint(dr["constraint_name"].ToString());
            }
            sql = "SELECT name FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID('" + TableName + "')";
            DataTable tbDfConstraint = CurrentDb.GetDataTable(sql);
            {
                foreach (DataRow dr in tbDfConstraint.Rows)
                {
                    string Name = dr["name"].ToString();
                    string[] lstName = Name.Split("_".ToCharArray());
                    if (lstName.Length < 3 || lstName[2].ToLower() != FieldName.ToLower()) continue;
                    else
                        DeleteConstraint(Name);
                }
            }
            //string RefName = drColumn["RefName"].ToString();
            //string DefaultName = drColumn["DefaultName"].ToString();


            ////xóa các ràng buộc trên cột trước
            //if (FieldName == Pk)
            //    if (!DeleteConstraint("PK_" + TableName)) { }
            //        //return false;
            //if (RefName != string.Empty)
            //    if (!DeleteConstraint(RefName)) { }
            ////return false;
            //if (DefaultName != string.Empty)
            //    if (!DeleteConstraint(DefaultName)) { }
            // return false;
            return (CurrentDb.UpdateByNonQuery("ALTER TABLE " + TableName + " DROP COLUMN " + FieldName ));
        }

        /// <summary>
        /// Thêm một ràng buộc khóa chính
        /// </summary>
        private bool AddPrimary()
        {
            if (TableName == string.Empty || Pk == string.Empty)
            {
                XtraMessageBox.Show("Không đủ thông tin để tạo ràng buộc khóa chính");
                return false;
            }
            string s = "ALTER TABLE " + TableName + " ADD CONSTRAINT PK_" + TableName + " PRIMARY KEY CLUSTERED (" + Pk + ")";
            return (CurrentDb.UpdateByNonQuery(s));
        }

        /// <summary>
        /// Thêm một ràng buộc khóa ngoại
        /// </summary>
        private bool AddReference(DataRowView drColumn)
        {
            string FieldName = drColumn["FieldName"].ToString();
            string RefName = drColumn["RefName"].ToString();
            string RefTable = drColumn["RefTable"].ToString();
            string RefField = drColumn["RefField"].ToString();
            if (FieldName == string.Empty || RefName == string.Empty || RefTable == string.Empty || RefField == string.Empty)
            {
                XtraMessageBox.Show("Không đủ thông tin để tạo ràng buộc tham chiếu");
                return false;
            }
            string s = "ALTER TABLE " + TableName + " ADD CONSTRAINT " + RefName + " FOREIGN KEY(" + FieldName + ") REFERENCES " +
                RefTable + "(" + RefField + ")";
            if (Boolean.Parse(drColumn["CasUpdate"].ToString()))
                s += " ON UPDATE CASCADE";
            if (Boolean.Parse(drColumn["CasDelete"].ToString()))
                s += " ON DELETE CASCADE";
            return (CurrentDb.UpdateByNonQuery(s));
        }

        /// <summary>
        /// Thêm một ràng buộc mặc định
        /// </summary>
        private bool AddDefault(DataRowView drColumn)
        {
            string FieldName = drColumn["FieldName"].ToString();
            string DefaultName = drColumn["DefaultName"].ToString();
            string DefaultValue = drColumn["DefaultValue"].ToString();
            if (FieldName == string.Empty || DefaultName == string.Empty || DefaultValue == string.Empty)
            {
                XtraMessageBox.Show("Không đủ thông tin để tạo ràng buộc mặc định");
                return false;
            }
            string s = "ALTER TABLE " + TableName + " ADD CONSTRAINT " + DefaultName + " DEFAULT ('" + DefaultValue + "') FOR " + FieldName;
                
            return (CurrentDb.UpdateByNonQuery(s));
        }

        /// <summary>
        /// Gỡ bỏ một ràng buộc bất kỳ
        /// </summary>
        private bool DeleteConstraint(string ConstName)
        {
            string s = "ALTER TABLE " + TableName + " DROP CONSTRAINT " + ConstName;
            if (!CurrentDb.UpdateByNonQuery(s))
            {
                XtraMessageBox.Show("Có lỗi trong quá trình gỡ bỏ ràng buộc của bảng");
                return false;
            }
            return true;
        }
    }
}
