using BlazorBootstrap;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Data.SqlClient;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace NFCWebBlazor.App_ClassDefine
{
    public class ClassProcess
    {
        string server = ConnectionService.server;
        string userSql = ConnectionService.userSql;
        string passSql = ConnectionService.passSql;
        string DataBaseMain = ConnectionService.DataBaseMain;
        public string DataBaseNVL = "NVLDB";
        public SqlConnection Connect()//Ket Noi
        {
            string Ketnoi = string.Format(@"Data Source={0};Initial Catalog={3};User ID={1};Password={2};Encrypt=False;", server, userSql, passSql, DataBaseMain);
            SqlConnection conn = new SqlConnection(Ketnoi);
            return conn;
        }
        public SqlConnection ConnectNVL()//Ket Noi
        {
            string Ketnoi = string.Format(@"Data Source={0};Initial Catalog={3};User ID={1};Password={2};Encrypt=False;", server, userSql, passSql, DataBaseNVL);
            SqlConnection conn = new SqlConnection(Ketnoi);
            return conn;
        }
        public DataTable dt_Connect(string sql, SqlConnection Conn)//Khoi tao dataset
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, Conn);
            DataTable ds1 = new DataTable();
            da.Fill(ds1);
            da.Dispose();
            return ds1;
        }
        public DataSet dts(string sql, SqlConnection Conn)//Khoi tao dataset
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, Conn);
            DataSet ds1 = new DataSet();
            da.Fill(ds1);
            da.Dispose();
            return ds1;
        }
        public DataTable dt_sqlcmd(SqlCommand cmd, SqlConnection Conn)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            da.Dispose();
            //Conn.Close();
            return dt;
        }

        public static void DeleteFileishost(string filename)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filename);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(ModelAdmin.userhost, ModelAdmin.passwordhost);
            FtpWebResponse response;
            try
            {
                response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse)ex.Response;
                if (response.StatusCode ==
                    FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    //msgBox.Show("File không có trên server", StaticClass.iconerror);
                    //Does not exist
                }
            }
        }

        public string RandomString(int length)
        {
           
            Random random = new Random();
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[random.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }
        public string ConvertDataTableToJson(DataTable dt)
        {
            //List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            //Dictionary<string, object> row;

            //foreach (DataRow dr in dt.Rows)
            //{
            //    row = new Dictionary<string, object>();
            //    foreach (DataColumn col in dt.Columns)
            //    {
            //        row.Add(col.ColumnName, dr[col]);
            //    }
            //    rows.Add(row);
            //}
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture, // Sử dụng định dạng số US
                Formatting = Formatting.Indented,
                FloatFormatHandling = FloatFormatHandling.String, // Chuyển số float/double thành chuỗi có dấu chấm
                FloatParseHandling = FloatParseHandling.Decimal  // Đảm bảo số được parse đúng
            };

           return JsonConvert.SerializeObject(dt, settings);
        }


   


        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
       
        public List<T> ConvertDataTableToClassList<T>(DataTable table) where T : new()
        {
            string[] checktypeNumber = new string[] { "System.Double", "System.Int", "System.Int32", "System.Int16", "System.UInt32", "System.UInt64", "System.Decimal" };
            List<T> list = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                T obj = new T();

                foreach (DataColumn column in table.Columns)
                {
                    PropertyInfo property = obj.GetType().GetProperty(column.ColumnName);
                    if (property != null && row[column] != DBNull.Value)
                    {
                        foreach (string it in checktypeNumber)
                        {
                            if (it == property.PropertyType.ToString())
                            {

                            }
                        }
                        Console.WriteLine(property.PropertyType);
                        try
                        {
                            Type targetType = IsNullableType(property.PropertyType)
                              ? Nullable.GetUnderlyingType(property.PropertyType)
                              : property.PropertyType;
                            object value = Convert.ChangeType(row[column], property.PropertyType);
                            Console.WriteLine(string.Format("Value là {0}", value));
                            property.SetValue(obj, value, null);
                        }
                        catch
                        {
                            Console.WriteLine(string.Format("Lỗi convert: {0}, {1}", row[column], property.PropertyType));
                            property.SetValue(obj, null, null);
                        }

                    }
                }

                list.Add(obj);
            }

            return list;
        }
        public string SerialLengthToString(int i, int len)
        {
            int length = i.ToString().Length;
            string s = "";
            for (int n = length; n < len; n++)
            {
                s += "0";
            }
            return s + i.ToString();
        }
        public int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        public async Task exportexcelAsync(IJSRuntime JS,DataTable dt, int rowStart, int columnStart, string GhiChu)
        {
            if (rowStart < 2)
                rowStart = 2;
                using (var workbook = new XLWorkbook())
                {
                    int rowEnd = rowStart + dt.Rows.Count;
                    //int rowEnd = rowStart + dt.Columns.Count - 1;

                    int columnEnd = columnStart + dt.Columns.Count - 1;
                
                    

                    var worksheet = workbook.AddWorksheet("Export");

                    worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

                  
                    List<string> lst_rowgroup = new List<string>();
                    int countrowrangebegin = rowStart + 1;//Vùng dữ liệu bắt đầu
                    int countrowrangeend = rowStart;
                    object[,] arr = new object[dt.Rows.Count + 1, dt.Columns.Count];//Tính luôn dòng Header
                                                                                    //Chuyển dữ liệu từ DataTable vào mảng đối tượng
                                                                                    //MessageBox.Show("test");
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {

                        //arr[0, c] = dt.Columns[c].ColumnName;
                        var rowtieude = worksheet.Row(rowStart);
                        rowtieude.Cell(columnStart + c).Value = dt.Columns[c].ColumnName;

                    }
                    int rowbegindata = rowStart + 1;

                    int d = 0;
                    for (int r = 0; r < dt.Rows.Count; r++)
                    {
                        DataRow dr = dt.Rows[r];
                        for (int c = 0; c < dt.Columns.Count; c++)
                        {
                            var rowdata = worksheet.Row(rowbegindata + r);
                            if (dr[c] == DBNull.Value)
                            {
                                rowdata.Cell(columnStart + c).Value = "";
                            }
                            else
                            {
                                if (int.TryParse(dr[c].ToString(), out d))
                                {
                                    rowdata.Cell(columnStart + c).Value = d;
                                }
                                else
                                    rowdata.Cell(columnStart + c).Value = dr[c].ToString();
                            }


                        }
                    }
                    var rangeborder = worksheet.RangeUsed();
                    rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    IXLCell xLCelltittle = worksheet.Row(rowStart-1).Cell(columnStart);
                    xLCelltittle.Value = string.Format("{0}", GhiChu);
                    xLCelltittle.Style.Font.Bold = true;
                    xLCelltittle.Style.Font.FontSize = 20;
                    //DateTime now = DateTime.Now;
                    //worksheet.Row(1).Cell(7).Value = now.Hour.ToString() + " giờ, " + now.Minute.ToString() + " phút, " + "Ngày " + now.Day.ToString() + " tháng " + now.Month.ToString() + " năm " + now.Year.ToString(); ;
                    ////worksheet.Cells[1, 6] = now.Hour.ToString() + " giờ, " + now.Minute.ToString() + " phút, " + "Ngày " + now.Day.ToString() + " tháng " + now.Month.ToString() + " năm " + now.Year.ToString();
                    ////Điền dữ liệu vào vùng đã thiết lập
                    //worksheet.Row(5).Cell(7).Value = GhiChu;
                    //Thiết lập vùng điền dữ liệu
                    IXLCell c1 = worksheet.Row(rowStart).Cell(columnStart);
                    IXLCell c2 = worksheet.Row(rowStart).Cell(columnStart);

                    IXLRange range = worksheet.Range(rowStart, columnStart, rowbegindata + dt.Rows.Count, columnEnd);

                    //range.Borders.LineStyle = Microsoft.Office.Interop.Excel.Constants.xlSolid;
                    foreach (var column in worksheet.ColumnsUsed())
                    {
                        column.AdjustToContents();
                    }
                    IXLRow row0 = worksheet.Row(rowStart);
                    row0.Style.Font.Bold = true;
                using (var outputStream = new MemoryStream())
                {
                    workbook.SaveAs(outputStream);
                    outputStream.Position = 0; // Đặt lại vị trí của stream để đọc từ đầu

                    // Trả về tệp Excel để người dùng tải xuống
                    var fileContent = outputStream.ToArray();

                  

                    // Convert the byte array to base64 string
                    var base64String = Convert.ToBase64String(fileContent);

                    await JS.InvokeVoidAsync("saveAsFile", "ExportToExcel.xlsx", base64String);
                }
                // row0.Style.Font.FontColor = fontlevel1;
                //row0.Ho.Horizontal = XLAlignmentHorizontalValues.Right;
                // range.Columns.AutoFit();
                //dt.Dispose();
                //workbook.SaveAs(path);
            }
           

        }
        public DateTime GetFirstDayOfWeek(DateTime date)
        {
            // Tính toán ngày đầu tiên trong tuần (thứ Hai)
            int diff = date.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
            {
                diff += 7;
            }
            return date.AddDays(-diff).Date;
        }
        public DateTime GetFirstDayOfQuarter(DateTime date)
        {
            int quarter = (date.Month - 1) / 3 + 1;
            int firstMonthOfQuarter = (quarter - 1) * 3 + 1;

            return new DateTime(date.Year, firstMonthOfQuarter, 1);
        }
        public DateTime GetFirstDayOfMonth(DateTime date)
        {
            // Trả về ngày đầu tiên trong tháng (ngày 1)
            return new DateTime(date.Year, date.Month, 1);
        }
        public DateTime GetFirstDayOfYear(DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }
        public List<QuyDoiNgay> getDayWeekMonthYear(List<DateTime> lstdate, string KieuChuyen)
        {
            List<QuyDoiNgay> lst = new List<QuyDoiNgay>();
            if (KieuChuyen == "W")
            {
                foreach (var it in lstdate)
                {

                    lst.Add(new QuyDoiNgay(it, "W" + SerialLengthToString(GetIso8601WeekOfYear(it), 2) + "-" + (it.Year % 2000).ToString()));
                }
            }
            if (KieuChuyen == "M")
            {
                foreach (var it in lstdate)
                {

                    lst.Add(new QuyDoiNgay(it, "M" + SerialLengthToString(it.Month, 2) + "-" + (it.Year % 2000).ToString()));
                }

            }
            if (KieuChuyen == "Q")
            {
                int i = 0;
                foreach (var date in lstdate)
                {
                    if (date.Month >= 4 && date.Month <= 6)
                        i = 1;
                    else if (date.Month >= 7 && date.Month <= 9)
                        i = 2;
                    else if (date.Month >= 10 && date.Month <= 12)
                        i = 3;
                    else
                        i = 4;
                    lst.Add(new QuyDoiNgay(date, "Q" + i.ToString() + "-" + (date.Year % 2000).ToString()));
                }

            }
            if (KieuChuyen == "Y")
            {
                foreach (var it in lstdate)
                {
                    lst.Add(new QuyDoiNgay(it, "Y-" + it.Year.ToString()));
                }

            }
            return lst;
        }

        private string ChuyenSoThanhChu(string gNumber)
        {
            string result = "";
            switch (gNumber)
            {
                case "0":
                    result = "không";
                    break;

                case "1":
                    result = "một";
                    break;

                case "2":
                    result = "hai";
                    break;
                case "3":
                    result = "ba";
                    break;
                case "4":
                    result = "bốn";
                    break;
                case "5":
                    result = "năm";
                    break;

                case "6":
                    result = "sáu";
                    break;
                case "7":
                    result = "bảy";
                    break;

                case "8":
                    result = "tám";
                    break;

                case "9":
                    result = "chín";
                    break;
            }
            return result;
        }
        private string Docphanthapphan(int n)//phần thập phân được tách ra thành số nguyên để đọc
        {
            string text = "";
            string so = n.ToString();
            for (int i = 0; i < so.Length; i++)
            {

                text += ChuyenSoThanhChu(so[i].ToString()) + " ";
            }
            return text.Trim();
        }
        public string dochangchuc(double so, bool daydu)
        {
            string chuoi = "";
            int chuc = (int)Math.Floor(so / 10);
            int donvi = (int)so % 10;
            if (chuc > 1)
            {
                chuoi = " " + mangso[chuc] + " mươi";
                if (donvi == 1)
                {
                    chuoi += " mốt";
                }
            }
            else if (chuc == 1)
            {
                chuoi = " mười";
                if (donvi == 1)
                {
                    chuoi += " một";
                }
            }
            else if (daydu && donvi > 0)
            {
                chuoi = " lẻ";
            }
            if (donvi == 5 && chuc >= 1)
            {
                chuoi += " lăm";
            }
            else if (donvi > 1 || (donvi == 1 && chuc == 0))
            {
                chuoi += " " + mangso[donvi];
            }
            return chuoi;
        }
        //Đọc block 3 số
        public string docblock(double so, bool daydu)
        {
            string chuoi = "";
            int tram = (int)Math.Floor(so / 100);
            so = so % 100;
            if (daydu || tram > 0)
            {
                chuoi = " " + mangso[tram] + " trăm";
                chuoi += dochangchuc(so, true);
            }
            else
            {
                chuoi = dochangchuc(so, false);
            }
            return chuoi;
        }
        //Đọc số hàng triệu
        public string dochangtrieu(double so, bool daydu)
        {
            string chuoi = "";
            int trieu = (int)Math.Floor(so / 1000000);
            so = so % 1000000;
            if (trieu > 0)
            {
                chuoi = docblock(trieu, daydu) + " triệu, ";
                daydu = true;
            }
            double nghin = Math.Floor(so / 1000);
            so = so % 1000;
            if (nghin > 0)
            {
                chuoi += docblock(nghin, daydu) + " nghìn, ";
                daydu = true;
            }
            if (so > 0)
            {
                chuoi += docblock(so, daydu);
            }
            return chuoi;
        }
        private string[] mangso = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        public string docsonguyen(double so)
        {
            if (so == 0) return mangso[0];
            string chuoi = "", hauto = "";
            do
            {
                double ty = so % 1000000000;
                so = Math.Floor(so / 1000000000);
                if (so > 0)
                {
                    chuoi = dochangtrieu(ty, true) + hauto + chuoi;
                }
                else
                {
                    chuoi = dochangtrieu(ty, false) + hauto + chuoi;
                }
                hauto = " tỷ, ";
            } while (so > 0);
            try
            {
                if (chuoi.Trim().Substring(chuoi.Trim().Length - 1, 1) == ",")
                { chuoi = chuoi.Trim().Substring(0, chuoi.Trim().Length - 1); }
            }
            catch { }
            return chuoi.Trim();
        }
        public string docsothapphan(double d)//Tối đa lấy 3 số sau dấu phẩy thôi
        {
            string chu = "";
            string so = d.ToString("######0.###");
            string[] arr = so.Split('.');
            chu = docsonguyen(double.Parse(arr[0]));
            if (arr.Length == 2)
            {
                chu += ", " + Docphanthapphan(int.Parse(arr[1].ToString()));
            }
            return FirstCharToUpper(chu);
        }
        public string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Chuyển đổi ký tự đầu tiên thành chữ in hoa
            char[] charArray = input.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);

            return new string(charArray);
        }
    }
    public static class ConnectionService
    {
        //static string server = @".\SQLEXPRESS";
        //static string server = @"
        //192.168.1.155";
        //public static string server = @"ungdungquocanh.hopto.org,1433";
       public static string server = @"";
        public static string DataBaseMain = "";
        public static string userSql = "";
        public static string passSql = "";

        public static string connstring = string.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};TrustServerCertificate=True;Encrypt=False;", server, DataBaseMain, userSql, passSql);
        //public static string connstring = ClassProcess.Connect
    }

}
