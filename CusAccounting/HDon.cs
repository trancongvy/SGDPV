using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;
namespace CusAccounting
{
   public  class HDon
    {
       public DLHDon DLHDon{ get; [CompilerGenerated] set; }
        public MCCQT MCCQT { get; [CompilerGenerated] set; }

    }
    public class MCCQT
    { 
        public string @id { get; [CompilerGenerated] set; }
        public string text { get; [CompilerGenerated] set; }
    }

        public class DLHDon
    {
        public string @id { get; [CompilerGenerated] set; }
        public TTChung TTChung { get; [CompilerGenerated] set; }
        public NDHDon NDHDon { get; [CompilerGenerated] set; }

    }
    public class TTChung
    {
        public string PBan { get; [CompilerGenerated] set; }
        public string THDon { get; [CompilerGenerated] set; }
        public int KHMSHDon{ get; [CompilerGenerated] set; }
        public string KHHDon { get; [CompilerGenerated] set; }
        public string SHDon { get; [CompilerGenerated] set; }
        public DateTime NLap { get; [CompilerGenerated] set; }
        public string DVTTe{ get; [CompilerGenerated] set; }
        double TGia{ get; [CompilerGenerated] set; }
        public string HTTToan{ get; [CompilerGenerated] set; }
        public string MSTTCGP{ get; [CompilerGenerated] set; }
    }
    public class NDHDon
    {
        public NBan NBan{ get; [CompilerGenerated] set; }
        public NMua NMua{ get; [CompilerGenerated] set; }
        //public DSHHDVu DSHHDVu{ get; [CompilerGenerated] set; }
        public TToan TToan{ get; [CompilerGenerated] set; }

    }
   
    public class NBan
    {
        public string Ten{ get; [CompilerGenerated] set; }
        public string MST{ get; [CompilerGenerated] set; }
        public string DChi{ get; [CompilerGenerated] set; }
        public string SDThoai { get; [CompilerGenerated] set; }
        public string DCTDTu{ get; [CompilerGenerated] set; }//dia chi thu dien tu
        public string STKNHang{ get; [CompilerGenerated] set; }
        public string TNHang{ get; [CompilerGenerated] set; }
           
    }
    public class NMua
    {
        public string Ten{ get; [CompilerGenerated] set; }
        public string MST{ get; [CompilerGenerated] set; }
        public string DChi{ get; [CompilerGenerated] set; }
        public string SDThoai { get; [CompilerGenerated] set; }
        public string HVTNMHang { get; [CompilerGenerated] set; }
    }
    public class DSHHDVu
    {
        public List<HHDVu> HHDVu{ get; [CompilerGenerated] set; }
    }
    public class HHDVu
    {
        public int TChat{ get; [CompilerGenerated] set; }
        public int STT{ get; [CompilerGenerated] set; }
        public string THHDVu{ get; [CompilerGenerated] set; }//ten hang hoa
        public string DVTinh{ get; [CompilerGenerated] set; }//
        public string SLuong { get; [CompilerGenerated] set; }
        public string DGia { get; [CompilerGenerated] set; }
        public string TLCKhau { get; [CompilerGenerated] set; }
        public double? STCKhau { get; [CompilerGenerated] set; } = 0;
        public double? ThTien { get; [CompilerGenerated] set; } = 0;
        public string TSuat { get; [CompilerGenerated] set; } 
    }
    public class TToan
    {
        public THTTLTSuat THTTLTSuat{ get; [CompilerGenerated] set; }
        public double? TgTCThue { get; [CompilerGenerated] set; } = 0;//tong tien co thue
        public double? TgTThue { get; [CompilerGenerated] set; } = 0;//tong tien thue
        public double? TgTTTBSo { get; [CompilerGenerated] set; } = 0;//Tong tien hang banng so
        public string TgTTTBChu { get; [CompilerGenerated] set; }//Tong tien bang chu
    }
    public class THTTLTSuat
    {
        public LTSuat TTSuat { get; [CompilerGenerated] set; }
    }
    public class LTSuat
    {
        public string TSuat { get; [CompilerGenerated] set; }
        public double? ThTien { get; [CompilerGenerated] set; } = 0;
        public double? TThue{ get; [CompilerGenerated] set; } = 0;
    }
    public static class ThietKedulieu 
    {
        public static DataTable CreateHoadon()
        {
            DataTable tb= new DataTable();
            DataColumn tb0 = new DataColumn("MCCQT", typeof(string));
            DataColumn tb1 = new DataColumn("MTID", typeof(Guid));
            DataColumn tb2 = new DataColumn("Ngayhd", typeof(DateTime));
            DataColumn tb3 = new DataColumn("Sohoadon", typeof(string));
            DataColumn tb4 = new DataColumn("kyhieu", typeof(string));
            DataColumn tb5 = new DataColumn("HTTToan", typeof(string));
            DataColumn tb6 = new DataColumn("MaHTTT", typeof(string));
            DataColumn tb7 = new DataColumn("MaKH", typeof(string));
            DataColumn tb8 = new DataColumn("TenKH", typeof(string));
            DataColumn tb9 = new DataColumn("MST", typeof(string));
            DataColumn tb10 = new DataColumn("DiaChi", typeof(string));
            DataColumn tb11 = new DataColumn("DCTDTu", typeof(string));
            DataColumn tb12 = new DataColumn("SDThoai", typeof(string));
            DataColumn tb13 = new DataColumn("STKNHang", typeof(string));
            DataColumn tb18 = new DataColumn("TNHang", typeof(string));
            
            DataColumn tb14 = new DataColumn("TTienH", typeof(double));
            DataColumn tb15 = new DataColumn("TThue", typeof(double));
            DataColumn tb16 = new DataColumn("TTien", typeof(double));
            DataColumn tb17 = new DataColumn("MaThue", typeof(string));
            DataColumn tb19 = new DataColumn("TkNo", typeof(string));
            DataColumn tb22= new DataColumn("TkCo", typeof(string));
            DataColumn tb20 = new DataColumn("Ongba", typeof(string));
            DataColumn tb21 = new DataColumn("KieuHD", typeof(int));//0. Hàng hóa, 1. Dịch vụ ,2 Chi trực tiếp, 3. Chi từ Ngân hàng
            DataColumn tb23 = new DataColumn("TkThue", typeof(string));
            DataColumn tb24 = new DataColumn("DienGiai", typeof(string));
            tb.Columns.AddRange(new DataColumn[] { tb1, tb2, tb3, tb4, tb5, tb6, tb7, tb8, tb9, tb10, tb11, tb12, tb13, tb14, tb15, tb16,tb17, tb18, tb19,tb20,tb21,tb22, tb23,tb0, tb24 });
            return tb;
        }
        public static DataTable CreateHHDV()
        {
            DataTable tb = new DataTable();
            DataColumn tb1 = new DataColumn("MTID", typeof(Guid));
            DataColumn tb2 = new DataColumn("MTIDDT", typeof(Guid));
            DataColumn tb3 = new DataColumn("MaVT", typeof(string));
            DataColumn tb4 = new DataColumn("MaKho", typeof(string));
            DataColumn tb5 = new DataColumn("TenVT", typeof(string));
            DataColumn tb6 = new DataColumn("MaDVT", typeof(string));
            DataColumn tb7 = new DataColumn("DVT", typeof(string));
            DataColumn tb8 = new DataColumn("Soluong", typeof(double));
            DataColumn tb9 = new DataColumn("DonGia", typeof(double));
            DataColumn tb10 = new DataColumn("TileCK", typeof(double));
            DataColumn tb11 = new DataColumn("CK", typeof(double));
            DataColumn tb12 = new DataColumn("TTien", typeof(double));
            DataColumn tb19 = new DataColumn("MaThueCT", typeof(string));
            DataColumn tb13 = new DataColumn("Thuesuat", typeof(double));
            DataColumn tb14 = new DataColumn("TienThue", typeof(double));
            DataColumn tb15 = new DataColumn("TkDthu", typeof(string));
            DataColumn tb16 = new DataColumn("Tkgv", typeof(string)); 
            DataColumn tb17 = new DataColumn("Tkkho", typeof(string));
            DataColumn tb18= new DataColumn("isDV", typeof(bool));
            DataColumn tb20 = new DataColumn("dictance", typeof(double));
            DataColumn tb21 = new DataColumn("Sohoadon", typeof(string));
            tb.Columns.AddRange(new DataColumn[] { tb1, tb2, tb3, tb4, tb5, tb6, tb7, tb8, tb9, tb10, tb11, tb12, tb13,tb14, tb15, tb16, tb17, tb18,tb19,tb20,tb21 });
            return tb;
        }
        static int EditDistance(string s, string t)
        {
            t = System.Text.RegularExpressions.Regex.Replace(t, "\\s{2,}", " ");
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= m; j++)
            {
                d[0, j] = j;
            }

            for (int j = 1; j <= m; j++)
            {
                for (int i = 1; i <= n; i++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(Math.Min(
                        d[i - 1, j] + 1,
                        d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        public static double LevenshteinWithWeight(string s, string t)
        {// Độ dài của các chuỗi đầu vào
            s = ReplacSignWithSpace(s);
            t = t.Replace("_", " ").Replace("-", " ").Replace(",", " ");
            s = s.Replace("_", " ").Replace("-", " ").Replace(",", " ");

            t = System.Text.RegularExpressions.Regex.Replace(t, "\\s{2,}", " ").Trim();
            s = System.Text.RegularExpressions.Regex.Replace(s, "\\s{2,}", " ").Trim();
            string[] tList = t.Split(" ".ToArray());
            string[] sList = s.Split(" ".ToArray());
            int n = sList.Length;
            int m = tList.Length;

            // Nếu một trong hai chuỗi là chuỗi rỗng, trả về độ dài của chuỗi khác
            if (n == 0)
                return m * 100;
            if (m == 0)
                return n * 100;

            // Khởi tạo ma trận giá trị
            double[,] distance = new double[n + 1, m + 1];

            // Khởi tạo giá trị cho cột đầu tiên và hàng đầu tiên của ma trận
            for (int i = 0; i <= n; i++)
                distance[i, 0] = i * 100;
            for (int j = 0; j <= m; j++)
                distance[0, j] = j * 100;

            // Tính toán giá trị của ma trận
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {

                    // Tính toán giá trị trọng số của ký tự hiện tại
                    double cost = EditDistance(sList[i - 1], tList[j - 1]);
                    double trongso = Math.Sqrt((sList[i - 1].Length * sList[i - 1].Length + tList[j - 1].Length * tList[j - 1].Length));
                    double Dig = 0;
                    bool isDig = double.TryParse(tList[j - 1], out Dig);
                    Dig = isDig ? -50 : -20;
                    cost = sList[i - 1] == tList[j - 1] ? Dig : 100 * cost / trongso;//(double.TryParse(s[i-1], || char.IsDigit(t[j - 1])) ? 10 : 1;
                    // Tính toán các giá trị cho các phép biến đổi và chọn phép biến đổi tốt nhất
                    double insertion = distance[i, j - 1] + cost;
                    double deletion = distance[i - 1, j] + cost;
                    double substitution = distance[i - 1, j - 1] + cost;
                    distance[i, j] = Math.Min(Math.Min(insertion, deletion), substitution);
                }
            }
            //for (int i = 0; i < distance.GetLength(0); i++)
            //{
            //    for (int j = 0; j < distance.GetLength(1); j++)
            //    {
            //        Console.Write(Math.Round(distance[i, j], 0) + "         ");
            //    }
            //    Console.WriteLine();
            //}
            //Trả về giá trị Levenshtein distance giữa hai chuỗi
            double trongsoTu = Math.Sqrt(sList.Length * sList.Length + tList.Length * tList.Length);
            return distance[n, m] / trongsoTu;
        }
        public static int BestMaxIndex(string[] arr, string pattern)
        {
            //string[] arr = {"Kho hàng hóa",
            //            "Kho ký gởi",
            //            "Kho hàng tại cảng chờ chuyển về",
            //            "Kho Nhà Cung Cấp",
            //            "Nvl",
            //            "Kho Thành phẩm"};

            //string pattern = "Kho ở Cảng";
            pattern=pattern.ToLower();
            int bestMatchIndex = -1;
            int smallestDifference = int.MaxValue;
            int dis;
            string kq = "";
            // Duyệt qua tất cả các chuỗi trong mảng
            for (int i = 0; i < arr.Length; i++)
            {
                string s = arr[i];
                int difference = int.MaxValue;

                // Duyệt qua tất cả các ký tự trong chuỗi s
                for (int j = 0; j <= s.Length - pattern.Length; j++)
                {
                    int tempDifference = 0;
                    // So sánh từng ký tự trong chuỗi pattern với các ký tự tương ứng trong chuỗi s
                    for (int k = 0; k < pattern.Length; k++)
                    {
                        tempDifference += Math.Abs((int)s[j + k] - (int)pattern[k]);
                    }

                    // Lưu trữ vị trí bắt đầu gần nhất và sự khác biệt nhỏ nhất
                    if (tempDifference < difference)
                    {
                        difference = tempDifference;
                    }
                }
                if (difference < smallestDifference)
                {
                    smallestDifference = difference;
                    dis = difference;
                    kq = s;
                }
                    if (difference < smallestDifference && difference<100)
                {
                    smallestDifference = difference;
                    bestMatchIndex = i;
                }
            }
            
                return bestMatchIndex;
        }
        public static int LevantEdit(string[] arr,string patten)
        {
            int i = int.MaxValue;
            string re = "";
            
            foreach(string s in arr)
            {
                if (s == "lần")
                {

                }
                int d = EditDistance(s, patten.ToLower());

                
                if (d < i && d <patten.Length/3)
                {
                    i = d; re = s;
                }
                    if (i == 0) return Array.IndexOf(arr, s);
               
            }
             return Array.IndexOf(arr,re);
        }
        public  static int LevantEditWithWeight(string[] arr, string patten)
        {
            patten = RemoveVietnameseSigns(patten.ToLower());
            patten = ReplacSignWithSpace(patten);
            double i = double.MaxValue;
            string re = "";
            Dictionary<string, double> dis = new Dictionary<string, double>();
            
            int idx = 0;
            foreach (string s in arr)
            {

                double d = LevenshteinWithWeight(s,patten);
                if (!dis.Keys.Contains(arr[idx]))
                    dis.Add(arr[idx], d);
                idx++;
                if (d < i )
                {
                    i =  d;
                    if (i < 20)
                    {
                        re = s;
                    }
                }
                
                //if (i == 0) 
                //    return Array.IndexOf(arr, s);

            }
            return Array.IndexOf(arr, re);
        }
        public  static  int LevantEditWithWeightHasDic(string[] arr, string patten,double disIn, out double distance)
        {
            patten = RemoveVietnameseSigns(patten.ToLower());
            patten = ReplacSignWithSpace(patten);

            double i = disIn;
            string re = "";
            Dictionary<string, double> dis = new Dictionary<string, double>();

            int idx = 0;
            foreach (string s in arr)
            {

                double d = LevenshteinWithWeight(s, patten);
                if (!dis.Keys.Contains(arr[idx]))
                    dis.Add(arr[idx], d);
                idx++;
                if (d < i)
                {
                    i = d;
                    if (i < 20)
                    {
                        re = s;
                    }
                }

                //if (i == 0) 
                //    return Array.IndexOf(arr, s);

            }
            distance = i;
            return Array.IndexOf(arr, re);
        }
        public static string ReplacSignWithSpace(string input)
        {
            StringBuilder output = new StringBuilder(input);

            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] == 'x' || output[i] == '+' || output[i] == '-' || output[i] == ':' || output[i] == '/' || output[i] == '*')
                {
                    bool isDigitBefore = false;
                    bool isDigitAfter = false;

                    if (i > 0)
                    {
                        isDigitBefore = Char.IsDigit(output[i - 1]);
                    }

                    if (i < output.Length - 1)
                    {
                        isDigitAfter = Char.IsDigit(output[i + 1]);
                    }

                    if (isDigitBefore && isDigitAfter)
                    {
                        output.Replace(output[i].ToString(), " " + output[i] + " ", i, 1);
                        i += 2;
                    }
                }
                if (i > 0 && Char.IsDigit(output[i]) && !Char.IsDigit(output[i - 1]) && output[i - 1] != '.')
                {
                    output.Replace(output[i].ToString(), " " + output[i], i, 1);
                    i += 1;
                }
                if (i < output.Length - 1 && Char.IsDigit(output[i]) && !Char.IsDigit(output[i + 1]) && output[i + 1] != '.')
                {
                    output.Replace(output[i].ToString(), output[i] + " ", i, 1);
                    i += 1;
                }
            }

            return output.ToString();
        }
        public static int FindClosestString(string [] strings, string searchTerm)
        {
            searchTerm = System.Text.RegularExpressions.Regex.Replace(searchTerm, "\\s{2,}", " ");
            Dictionary<string, double[]> vectors = new Dictionary<string, double[]>();
            double[] searchTermVector = new double[searchTerm.Length];

            for (int i = 0; i < searchTerm.Length; i++)
            {
                searchTermVector[i] = Convert.ToInt32(searchTerm[i]);
            }
            for(int index=0; index<strings.Length; index ++)           
            {
                string str = strings[index];
                double[] vector = new double[str.Length];

                for (int i = 0; i < str.Length; i++)
                {
                    vector[i] = Convert.ToInt32(str[i]);
                }
                if(!vectors.Keys.Contains(str))
                vectors.Add(str, vector);
            }

            double maxSimilarity = double.MinValue;
            string closestString = "";

            foreach (KeyValuePair<string, double[]> vector in vectors)
            {
                double dotProduct = DotProduct(searchTermVector, vector.Value);
                double magnitudeProduct = Magnitude(searchTermVector) * Magnitude(vector.Value);

                double cosineSimilarity = dotProduct / magnitudeProduct;

                if (cosineSimilarity > maxSimilarity && cosineSimilarity>0.98)
                {
                    maxSimilarity = cosineSimilarity;
                    closestString = vector.Key;
                }
            }
            int idx = Array.IndexOf(strings, closestString);
                return  idx;
        }
        static double CosineSimilarityScore(double[] vectorA, double[] vectorB)
        {
            double dotProduct = DotProduct(vectorA, vectorB);
            double magnitudeA = Magnitude(vectorA);
            double magnitudeB = Magnitude(vectorB);

            return dotProduct / (magnitudeA * magnitudeB);
        }
        static double DotProduct(double[] vectorA, double[] vectorB)
        {
            double dotProduct = 0;

            for (int i = 0; i < vectorA.Length && i< vectorB.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
            }

            return dotProduct;
        }

        static double Magnitude(double[] vector)
        {
            double sumOfSquares = 0;

            for (int i = 0; i < vector.Length; i++)
            {
                sumOfSquares += vector[i] * vector[i];
            }

            return Math.Sqrt(sumOfSquares);
        }
        public static string[] ConvertDataTableToArray(DataTable tb, string ColName)
        {
            

            string[] arr = new string[tb.Rows.Count];
            int i = 0;
            foreach (DataRow dr in tb.Rows)
            {
                arr[i] = dr[ColName].ToString().Replace("'", "").ToLower();
                arr[i] = RemoveTextBetweenParentheses(arr[i]);
                arr[i] = RemoveVietnameseSigns(arr[i]);
                i++;
            }
            return arr;
        }
        public static string[] ConvertDataRowsToArray(DataRow[] tb, string ColName)
        {


            string[] arr = new string[tb.Length];
            int i = 0;
            foreach (DataRow dr in tb)
            {
                arr[i] = dr[ColName].ToString().Replace("'", "").ToLower();
                arr[i] = RemoveTextBetweenParentheses(arr[i]);
                arr[i] = RemoveVietnameseSigns(arr[i]);
                i++;
            }
            return arr;
        }
        public static string RemoveDiacritics(string inputString)
        {
            // Sử dụng bảng mã Unicode Normalization Form D để chuyển đổi ký tự có dấu thành ký tự không dấu tương ứng
            string normalizedString = inputString.Normalize(NormalizationForm.FormD);

            // Xóa các ký tự phụ trợ không liên quan đến việc chuyển đổi
            Regex regex = new Regex(@"[^\p{ASCII}]");
            string asciiString = regex.Replace(normalizedString, "");

            // Trả về chuỗi kết quả
            return asciiString;
            // Trả về chuỗi kết quả

        }
        public static string RemoveVietnameseSigns(string str)
        {
            string[] fromArray = new string[] {"á", "à", "ả", "ã", "ạ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ",
                                        "đ", "Đ",
                                        "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ",
                                        "í", "ì", "ỉ", "ĩ", "ị",
                                        "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ",
                                        "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự",
                                        "ý", "ỳ", "ỷ", "ỹ", "ỵ"};

            string[] toArray = new string[] {"a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", 
                                        "d", "d",
                                        "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e",
                                        "i", "i", "i", "i", "i",
                                        "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o",
                                        "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u",
                                        "y", "y", "y", "y", "y"};

            for (int i = 0; i < fromArray.Length; i++)
            {
                str = str.Replace(fromArray[i], toArray[i]);
            }

            return str;
        }
        public static string RemoveTextBetweenParentheses(string text)
        {
            int startIndex = text.IndexOf('(');
            int endIndex = text.IndexOf(')');

            while (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                text = text.Remove(startIndex, endIndex - startIndex + 1);
                startIndex = text.IndexOf('(');
                endIndex = text.IndexOf(')');
            }

            return text;
        }
    }
    public class MInvoiceList
    {
        public int? status { get; [CompilerGenerated] set; }
        public int? totalInvoice { get; [CompilerGenerated] set; }
        public int? totalPage { get; [CompilerGenerated] set; }
        public int? currentPage { get; [CompilerGenerated] set; }
        public MInvoice[] listInvoice { get; [CompilerGenerated] set; }

    }

    public class MInvoice
    {
        public string id { get; [CompilerGenerated] set; } //ID hóa đơn tức MTID
        public string nbmst { get; [CompilerGenerated] set; } //MST người bán
        public string khmshdon { get; [CompilerGenerated] set; }//Ký hiệu mẫu
        public DateTime ntao { get; [CompilerGenerated] set; }//Ngày tạo
        public DateTime? tdlap { get; [CompilerGenerated] set; }//Ngày lập
        public DateTime? nky { get; [CompilerGenerated] set; }//Ngày tạo
        public DateTime ntnhan { get; [CompilerGenerated] set; }//Ngày tiếp nhận
        public string khhdon { get; [CompilerGenerated] set; }// Ký hiệu số serie
        public int? shdon { get; [CompilerGenerated] set; } // Số hóa đon
        public string cqt { get; [CompilerGenerated] set; } //Số cơ quan thuế
        public string dvtte { get; [CompilerGenerated] set; }
        public string hthdon { get; [CompilerGenerated] set; }
        public string thtttoan { get; [CompilerGenerated] set; }//Hình thức thanh toán
        public string nbdchi { get; [CompilerGenerated] set; }// Địa chỉ
        public string nbstkhoan { get; [CompilerGenerated] set; }//STK ngân hàng người bán
        public string nbten { get; [CompilerGenerated] set; } //Tên người bán
        public string nbtnhang { get; [CompilerGenerated] set; } //Tên Ngân hàng bên bán
        public string nmmst { get; [CompilerGenerated] set; } // MST Người mua
        public string nmdchi { get; [CompilerGenerated] set; } // Địa chỉ người mua
        public string nmten { get; [CompilerGenerated] set; } // Tên đơn vị mua hàng
        public string nmstkhoan { get; [CompilerGenerated] set; } //Số tài khoản người mua
        public string nmtnhang { get; [CompilerGenerated] set; } //Tên ngân hàng bên mua
        public string nmtnmua { get; [CompilerGenerated] set; } //Tên người mua hàng
        public int? tchat { get; [CompilerGenerated] set; } //Tính chất hóa đơn
        public double? tgia { get; [CompilerGenerated] set; }//Tỉ giá
        public double? ttcktmai { get; [CompilerGenerated] set; } //Tổng chiết khấu
        public double? tgtcthue { get; [CompilerGenerated] set; } //Tổng Tiền trước thuế
        public double? tgtthue { get; [CompilerGenerated] set; } //Tổng Tiền thuế
        public double? tgtttbso { get; [CompilerGenerated] set; } //Tổng tiền thanh toán
        public string tgtttbchu { get; [CompilerGenerated] set; } //Tổng tiền bằng chữ
        public string tsuat { get; [CompilerGenerated] set; } //Thuế suất
        public int? tthai { get; [CompilerGenerated] set; } //Trạng thái hóa đơn 
        public string mkhang { get; [CompilerGenerated] set; }
        public string nbsdthoai { get; [CompilerGenerated] set; }
        public string mhdon { get; [CompilerGenerated] set; } //Mã hóa đơn CQT
        public ThueSuat[] thttltsuat { get; [CompilerGenerated] set; }
        public HHDVu_Minv[] hdhhdvu { get; [CompilerGenerated] set; }
        public ttkhac[] ttkhac { get; [CompilerGenerated] set; }

    }
    public class ttkhac
    {
        public string ttruong { get; [CompilerGenerated] set; }
        public string kdlieu { get; [CompilerGenerated] set; }
        public string dlieu { get; [CompilerGenerated] set; }

    }
    public class ThueSuat
    {
        public string tsuat { get; [CompilerGenerated] set; }
    }
    public class HHDVu_Minv
    {

        public int? stt { get; [CompilerGenerated] set; } //Số thứ tự bản ghi
        
        public string idhdon { get; [CompilerGenerated] set; } //MTID Hóa đơn
        public string id { get; [CompilerGenerated] set; } //DTID Hóa đơn
        public string ten { get; [CompilerGenerated] set; } //DTID Hóa đơn
        public string dvtinh { get; [CompilerGenerated] set; } //DTID Hóa đơn
        public string ltsuat { get; [CompilerGenerated] set; } //mã thuế suất
        public double? sluong { get; [CompilerGenerated] set; } = 0;//DTID Hóa đơn
        public double? dgia { get; [CompilerGenerated] set; } = 0; //DTID Hóa đơn
        public double? thtien { get; [CompilerGenerated] set; } = 0;//DTID Hóa đơn

        public double? tlckhau { get; [CompilerGenerated] set; } = 0;//tỉ lệ ck
        public double? stckhau { get; [CompilerGenerated] set; } = 0;//tiền ck
        public double? tsuat { get; [CompilerGenerated] set; } = 0;//thuế suất ck        
        public string tchat { get; [CompilerGenerated] set; } //Tính chất hhdv
        

    }

}
