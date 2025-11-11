using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CusAccounting
{
    public class Inv
    {
        // Fields


        // Properties
        public string windowid { get; [CompilerGenerated] set; }
        public int editmode { get; [CompilerGenerated] set; }
        public Invoice Data { get; [CompilerGenerated] set; }
    }
    public class Invoice
    {


        // Properties
        //public string inv_InvoiceAuth_id { get; [CompilerGenerated] set; }
        public string inv_invoiceAuth_id { get; [CompilerGenerated] set; }
        public string inv_invoiceSeries { get; [CompilerGenerated] set; }
        public string inv_invoiceNumber { get; [CompilerGenerated] set; }
        public string inv_invoiceIssuedDate { get; [CompilerGenerated] set; }
        public string inv_currencyCode { get; [CompilerGenerated] set; }
        public double inv_exchangeRate { get; [CompilerGenerated] set; }
        public string inv_buyerDisplayName { get; [CompilerGenerated] set; }
        public string ma_dt { get; [CompilerGenerated] set; }
        public string inv_buyerLegalName { get; [CompilerGenerated] set; }
        public string inv_buyerTaxCode { get; [CompilerGenerated] set; }
        public string inv_buyerAddressLine { get; [CompilerGenerated] set; }
        public string inv_buyerEmail { get; [CompilerGenerated] set; }
        public string inv_buyerBankAccount { get; [CompilerGenerated] set; }
        public string inv_buyerBankName { get; [CompilerGenerated] set; }
        public string inv_paymentMethodName { get; [CompilerGenerated] set; }
        public string inv_sellerBankAccount { get; [CompilerGenerated] set; }
        public string Inv_sellerBankName { get; [CompilerGenerated] set; }
        public string mau_hd { get; [CompilerGenerated] set; }
        public double inv_TotalAmountWithoutVat { get; [CompilerGenerated] set; }
        public double VATRate { get; [CompilerGenerated] set; }
        public double inv_vatAmount { get; [CompilerGenerated] set; }
        public bool isDeductionNQ43 { get; [CompilerGenerated] set; }
        public double tlptdoanhthu20 { get; [CompilerGenerated] set; }
        public double tgtck20 { get; [CompilerGenerated] set; }
        public string api_key { get; [CompilerGenerated] set; }
        public double inv_TotalAmount { get; [CompilerGenerated] set; }
        public double inv_discountAmount { get; [CompilerGenerated] set; }
        public Details[] details { get; [CompilerGenerated] set; }
        public string id { get; [CompilerGenerated] set; }
        public string trang_thai_hd { get; set; }
        public string trang_thai { get; set; }
        public string mdvqhnsach_nmua { get; set; }
        public string cccdan { get; set; }
        public string so_hchieu { get; set; }
    }
    public class InvoiceGet
    {


        // Properties
        public string hoadon68_id { get; [CompilerGenerated] set; }
        public string khieu { get; [CompilerGenerated] set; }
        public string shdon { get; [CompilerGenerated] set; }
        public string tdlap { get; [CompilerGenerated] set; }
        public string sdhang { get; [CompilerGenerated] set; }

        public string dvtte { get; [CompilerGenerated] set; }
        public double tgia { get; [CompilerGenerated] set; }
        public string tnmua { get; [CompilerGenerated] set; }
        public string ma_dt { get; [CompilerGenerated] set; }
        public string ten { get; [CompilerGenerated] set; }
        public string mst { get; [CompilerGenerated] set; }
        public string dchi { get; [CompilerGenerated] set; }
        public string email { get; [CompilerGenerated] set; }
        public string stknmua { get; [CompilerGenerated] set; }
        public string nganhang_ngmua { get; [CompilerGenerated] set; }
        public string htttoan { get; [CompilerGenerated] set; }
        public string tgtttbchu { get; [CompilerGenerated] set; }

        public string inv_sellerBankAccount { get; [CompilerGenerated] set; }
        public string Inv_sellerBankName { get; [CompilerGenerated] set; }
        public string mau_hd { get; [CompilerGenerated] set; }
        public double tgtcthue { get; [CompilerGenerated] set; }
        public double VATRate { get; [CompilerGenerated] set; }
        public double tgtthue { get; [CompilerGenerated] set; }
        public double tgtttbso { get; [CompilerGenerated] set; }
        public double ttcktmai { get; [CompilerGenerated] set; }
        public ProductGet[] details { get; [CompilerGenerated] set; }

    }
    public class InvoiceInfo
    {

        // Properties
        //public string windowid { get; [CompilerGenerated] set; }
        public int editmode { get; [CompilerGenerated] set; }
        public Invoice[] data { get; [CompilerGenerated] set; }
    }
    public class InvoiceInfoDelete
    {

        // Properties
        //public string windowid { get; [CompilerGenerated] set; }
        public int editmode { get; [CompilerGenerated] set; }
        public InvoiceDelete[] data { get; [CompilerGenerated] set; }
    }
    public class InvoiceDelete
    {
        public string inv_invoiceSeries { get; [CompilerGenerated] set; }
        public string inv_invoiceNumber { get; [CompilerGenerated] set; }
    }
     public class Invoices
    {
        // Fields


        // Properties
        public Inv[] inv { get; set; }
    }
    public class LoginInfo
    {

        // Properties
        public string username { get; [CompilerGenerated] set; }
        public string password { get; [CompilerGenerated] set; }
        public string ma_dvcs { get; [CompilerGenerated] set; }
    }
    public class ProductGet
    {
        public string id { get; [CompilerGenerated] set; }

        public int tchat { get; [CompilerGenerated] set; }

        // Properties
        public string stt { get; [CompilerGenerated] set; }
        public string ma { get; [CompilerGenerated] set; }
        public string ten { get; [CompilerGenerated] set; }
        public string dvtinh { get; [CompilerGenerated] set; }
        public string mdvtinh { get; [CompilerGenerated] set; }
        public double? sluong { get; [CompilerGenerated] set; }
        public double? dgia { get; [CompilerGenerated] set; }
        public double? thtien { get; [CompilerGenerated] set; }
        public double? tthue { get; [CompilerGenerated] set; }
        public double? tgtien { get; [CompilerGenerated] set; }
        public double? tlckhau { get; [CompilerGenerated] set; }
        public double? stckhau { get; [CompilerGenerated] set; }
        public string tsuat { get; [CompilerGenerated] set; }
    }
    public class Product
    {
        public int tchat { get; [CompilerGenerated] set; }

        // Properties
        public string stt_rec0 { get; [CompilerGenerated] set; }
        public string inv_itemCode { get; [CompilerGenerated] set; }
        public string inv_itemName { get; [CompilerGenerated] set; }
        public string inv_unitCode { get; [CompilerGenerated] set; }
        public string inv_unitName { get; [CompilerGenerated] set; }
        public double inv_quantity { get; [CompilerGenerated] set; }
        public double inv_unitPrice { get; [CompilerGenerated] set; }
        public double inv_TotalAmountWithoutVat { get; [CompilerGenerated] set; }
        public double inv_vatAmount { get; [CompilerGenerated] set; }
        public double inv_TotalAmount { get; [CompilerGenerated] set; }
        public double inv_discountPercentage { get; [CompilerGenerated] set; }
        public double inv_discountAmount { get; [CompilerGenerated] set; }
        public string ma_thue { get; [CompilerGenerated] set; }
        public string soLo { get; [CompilerGenerated] set; }
        public string hanDung { get; [CompilerGenerated] set; }
    }
    public class Details
    {

        // Properties
        public Product[] data { get; set; }
    }

    public class Result
    {

        // Properties
        public string result { get; [CompilerGenerated] set; }
    }

    public class NumberUtil
    {
        // Methods
        public static string DocSoThanhChu(string number)
        {
            string str = "";
            string str2 = number;
            while ((str2.Length > 0) && (str2.Substring(0, 1) == "0"))
            {
                str2 = str2.Substring(1);
            }
            string[] strArray = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] strArray2 = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool flag = false;
            decimal num6 = 0M;
            try
            {
                num6 = Convert.ToDecimal(str2.ToString());
            }
            catch
            {
            }
            if (num6 < decimal.Zero)
            {
                num6 = -num6;
                flag = true;
            }
            int length = str2.Length;
            if (length == 0)
            {
                str = strArray[0] + str;
            }
            else
            {
                int index = 0;
                while (length > 0)
                {
                    int num4;
                    int num5;
                    int num3 = Convert.ToInt32(str2.Substring(length - 1, 1));
                    length--;
                    if (length > 0)
                    {
                        num4 = Convert.ToInt32(str2.Substring(length - 1, 1));
                    }
                    else
                    {
                        num4 = -1;
                    }
                    length--;
                    if (length > 0)
                    {
                        num5 = Convert.ToInt32(str2.Substring(length - 1, 1));
                    }
                    else
                    {
                        num5 = -1;
                    }
                    length--;
                    if ((((num3 > 0) || (num4 > 0)) || (num5 > 0)) || (index == 3))
                    {
                        str = strArray2[index] + str;
                    }
                    index++;
                    if (index > 3)
                    {
                        index = 1;
                    }
                    if ((num3 == 1) && (num4 > 1))
                    {
                        str = "mốt " + str;
                    }
                    else if ((num3 == 5) && (num4 > 0))
                    {
                        str = "lăm " + str;
                    }
                    else if (num3 > 0)
                    {
                        str = strArray[num3] + " " + str;
                    }
                    if (num4 < 0)
                    {
                        break;
                    }
                    if ((num4 == 0) && (num3 > 0))
                    {
                        str = "linh " + str;
                    }
                    if (num4 == 1)
                    {
                        str = "mười " + str;
                    }
                    if (num4 > 1)
                    {
                        str = strArray[num4] + " mươi " + str;
                    }
                    if (num5 < 0)
                    {
                        break;
                    }
                    if (((num5 > 0) || (num4 > 0)) || (num3 > 0))
                    {
                        str = strArray[num5] + " trăm " + str;
                    }
                    str = " " + str;
                }
            }
            if (flag)
            {
                str = "âm " + str;
            }
            return (str.Trim().Substring(0, 1).ToUpper() + str.Trim().Substring(1) + " đồng chẵn");
        }
    }
    public class GetInvoiceFromDateToDate
    {
        public string tuNgay { get; set; }
        public string denNgay { get; set; }
        public string khieu { get; set; }
    }
    public class Huyhoadon
    {
        public string inv_InvoiceAuth_id { get; set; }
        public string sovb { get; set; }
        public string ngayvb { get; set; }
        public string ghi_chu { get; set; }
    }
    public class VietQRInfo
    {
        public string accountNo { get; [CompilerGenerated] set; }
        public string accountName { get; [CompilerGenerated] set; }
        public string acqId { get; [CompilerGenerated] set; }
        public string addInfo { get; [CompilerGenerated] set; }
        public double amount { get; [CompilerGenerated] set; }
        public string template { get; [CompilerGenerated] set; }
    }
}

