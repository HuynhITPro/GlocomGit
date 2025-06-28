using DevExpress.Blazor;
using DevExpress.XtraReports.UI;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using NFCWebBlazor.App_ModelClass;


using System.Globalization;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NFCWebBlazor.App_ClassDefine
{
    public class StaticClass
    {

        public static string UIntToHtmlColor(uint? argb)
        {
            if (argb == null)
                return "#FFFFFF";
            // Tách các thành phần Alpha, Red, Green, Blue
            byte alpha = (byte)(argb >> 24);
            byte red = (byte)(argb >> 16);
            byte green = (byte)(argb >> 8);
            byte blue = (byte)(argb);

            // Chỉ cần Red, Green, Blue để tạo mã màu hex HTML
            string htmlColor = $"#{red:X2}{green:X2}{blue:X2}";

            return htmlColor;
        }
       public static string GetContrastColor(string hexColor)
        {
            if(hexColor==null)
                return "#000000";
            // Loại bỏ ký tự "#" nếu có
            hexColor = hexColor.Replace("#", "");

            // Chuyển đổi từ hex sang RGB
            int r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
           
            int g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            // Tính độ sáng (luminance)
            double luminance = 0.299 * r + 0.587 * g + 0.114 * b;

            // Nếu độ sáng lớn hơn 128, trả về màu đen; ngược lại, trả về màu trắng
            return luminance > 128 ? "#000000" : "#FFFFFF";
        }

        public static async Task ExportPdfAsync(IJSRuntime JS,DevExpress.XtraReports.UI.XtraReport report, string fileName)
        {
            DevExpress.XtraPrinting.PdfExportOptions exportOptions = new DevExpress.XtraPrinting.PdfExportOptions();

            //exportOptions. = DevExpress.XtraPrinting.TextExportMode.Text;

            // Export the report to XLSX format
            using (MemoryStream stream = new MemoryStream())
            {
                report.ExportToPdf(stream, exportOptions);

                // Convert the stream to a byte array
                var buffer = stream.ToArray();

                // Convert the byte array to base64 string
                var base64String = Convert.ToBase64String(buffer);

                // Invoke JavaScript function to save the file
                await JS.InvokeVoidAsync("saveAsFile", fileName, base64String);
               
            }
        }

        public static DxGridDataColumn gridColumn(string captain,string format)
        {
            DxGridDataColumn dxGridColumn = new DxGridDataColumn();
            dxGridColumn.Caption = captain;
            dxGridColumn.DisplayFormat=format;
            return dxGridColumn;
        }

        static string[] formatdatetimes = { "dd-MM-yyyy", "dd-MM-yy", "dd-MM-yyyy hh:mm:ss tt", "dd-MM-yy hh:mm:ss tt", "dd/MM/yyyy", "dd/MM/yy", "dd/MM/yyyy hh:mm:ss tt", "dd/MM/yy hh:mm:ss tt" };
        static CultureInfo cultureInfo = CultureInfo.CurrentCulture;
        public static double? ConvertNumberCultureInfo(object numberString)
        {
            if(numberString==null||numberString==DBNull.Value) return null;
            
            if (double.TryParse(numberString.ToString(), NumberStyles.Number, cultureInfo, out double number))
                return number;
            return null;
        }

        public static DateTime? ConvertDateCultureInfo(object dateString)
        {
           if(dateString==null||dateString==DBNull.Value) return null;
            string onlyDate = dateString.ToString().Split(' ')[0];
            if (DateTime.TryParseExact(onlyDate, formatdatetimes, cultureInfo, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            return null;
        }
        public static RenderFragment BuildColumns(List<InitDxGridDataColumn> _lstcolumn)
        {
            RenderFragment columns = b =>
            {
                int counter = 0;
                foreach (InitDxGridDataColumn col in _lstcolumn.OrderBy(p=>p.Index))
                {
                    b.OpenComponent(counter, typeof(DxGridDataColumn));
                    b.AddAttribute(0, "FieldName", col.FieldName);
                  
                    b.AddAttribute(0, "Caption", col.Caption);
                    if (col.gridTextAlignment != null)
                        b.AddAttribute(0, "TextAlignment", col.gridTextAlignment);
                    if (col.DisplayFormat != null)
                    {
                        b.AddAttribute(0, "DisplayFormat", col.DisplayFormat);
                    }
                    if (col.Width != null)
                        b.AddAttribute(0, "Width", string.Format("{0}px",col.Width));
                   else
                    if (col.Width != null)
                        b.AddAttribute(0, "MinWidth", string.Format("90px"));
                    if (col.GroupIndex!=null)
                    {
                        b.AddAttribute(0, "GroupIndex", col.GroupIndex.Value);
                    }
                    b.CloseComponent();

                    counter++;
                }

            };
            return columns;
        }

       public static string Randomstring(int length)
        {
            Random random = new Random();
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[random.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }
        public static string Substringdieukien(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            if(input.Length<4) return " where "+ input;
            else return  " where " + input.Substring(4);
        }
        public static string RemoveVietnamese(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            input = input.Replace("Đ", "D").Replace("đ", "d");
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public static string showdouble(double? d)
        {
            if (d == null)
                return "";
            return d.Value.ToString("#,#.#");
        }
        public static string showdecimal(decimal? d)
        {
            if (d == null)
                return "";
            return d.Value.ToString("#,#.#");
        }
        public static string topickyduyet = "sp/all/kyduyet";
        
    }
}
