using System;
using System.Collections.Generic;
using System.Text;
using Humanizer;
namespace CDTLib
{
    public class NumberReader
    {
        public static string ReadMoney(string Money)
        {
            string str0 = "";
            if (Money.Trim() == "0" || Money.Trim() == "")
            {
                return "Không";
            }
            Money = Money.Replace(" ", "");
            Money = Money.Replace(",", "");
            Money = Money.Replace(".", "");
            while (Money.Length < 12)
            {
                Money = "0" + Money;
            }
            string G3 = Money.Substring(0, 3);
            string text3 = Money.Substring(3, 3);
            string text4 = Money.Substring(6, 3);
            string text5 = Money.Substring(9, 3);
            if (G3 != "000")
            {
                str0 = ReadGroup3(G3);
                str0 = str0 + " tỷ";
            }
            if (text3 != "000")
            {
                str0 = str0 + ReadGroup3(text3);
                str0 = str0 + " triệu";
            }
            if (text4 != "000")
            {
                str0 = str0 + ReadGroup3(text4);
                str0 = str0 + " ngàn";
            }
            str0 = str0 + ReadGroup3(text5).Replace("không trăm lẻ", "lẻ");
            str0 = str0.Replace("một mươi", "mười");
            str0 = str0.Trim();
            if (str0.IndexOf("không trăm") == 0)
            {
                str0 = str0.Remove(0, 10);
            }
            str0 = str0.Trim();
            if (str0.IndexOf("lẻ") == 0)
            {
                str0 = str0.Remove(0, 2);
            }
            str0 = str0.Trim();
            str0 = str0.Replace("mươi một", "mươi mốt");
            str0 = str0.Trim();
            str0 = str0.Substring(0, 1).ToUpper() + str0.Substring(1, str0.Length - 1).ToLower();
            return str0.Trim();
        }
        public static double ConvertDb(string a)
        {
            a = a.Replace(" ","");
            double r = 0;
            try
            {
                r=double.Parse(a);
            }
            catch
            {
            }
            return r;
        }
        public static int ConvertInt(string a)
        {
            a = a.Replace(" ", "");
            int r = 0;
            try
            {
                r = int.Parse(a);
            }
            catch
            {
            }
            return r;
        }
        private static string ReadGroup3(string G3)
        {
            string[] textArray1 = new string[] { " không", " một", " hai", " ba", " bốn", " năm", " sáu", " bảy", " tám", " chín" };
            string str0 = "";
            if (G3 == "000")
            {
                return "";
            }
            char chr1 = G3[0];
            str0 = textArray1[int.Parse(chr1.ToString())] + " trăm";
            chr1 = G3[1];
            if (chr1.ToString() == "0")
            {
                chr1 = G3[2];
                if (chr1.ToString() == "0")
                {
                    return str0;
                }
                chr1 = G3[2];
                return str0 + " lẻ" + textArray1[int.Parse(chr1.ToString())];
            }
            chr1 = G3[1];
            str0 = str0 + textArray1[int.Parse(chr1.ToString())] + " mươi";
            chr1 = G3[2];
            if (chr1.ToString() == "5")
            {
                str0 = str0 + " lăm";
            }
            else
            {
                chr1 = G3[2];
                if (chr1.ToString() != "0")
                {
                    chr1 = G3[2];
                    str0 = str0 + textArray1[int.Parse(chr1.ToString())];
                }
            }
            return str0;
        }
        public static string ReadMoneyE(string Money)
        {
            Money = Money.Replace(" ", "");
            Money = Money.Replace(",", "");
            Money = Money.Replace(".", "");
            try
            {
                int mn = int.Parse(Money);
                return mn.ToWords().Transform(To.SentenceCase);
            }
            catch
            { return ""; }
        }
    }
}
