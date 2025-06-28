using BlazorBootstrap;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using NFCWebBlazor.App_ClassDefine;
using System.Data;
using System.Globalization;
using System.IO;

namespace NFCWebBlazor.App_Admin
{
    public partial class Page_XuLyExcel
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] BrowserService browserService { get; set; }
        [Inject] IJSRuntime JS { get; set; }
        [Inject] PreloadService PreloadService { get; set; }
        string textxuly = "";
        string fileName = "BaoCaoCLS.xlsx";
        public string linkfile = "";
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            calendar = CultureInfo.InvariantCulture.Calendar;
            return base.OnAfterRenderAsync(firstRender);
        }
        private async Task downloadfileAsync()
        {

            await JS.InvokeVoidAsync("saveAsFile", fileName, linkfile);
        }

        string[] formats = new string[] { "dd-MM-yy hh:mm:ss tt", "dd-MM-yy" };
        string[] arrcolumncheck = new string[] { "STT", "Ngày Tháng", "Khoa Chỉ Định", "Người C.Định" };

        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        DataTable data_Data = new DataTable();
        DataTable dttotal = new DataTable();
        public async void getdataimport(InputFileChangeEventArgs inputFile)
        {
           // PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
            try
            {


                textxuly = "Đang xử lý file....";
                linkfile = "";
                data_Data =await LoadDataFromExcelAsync(inputFile);
                Console.WriteLine("Số dòng: "+data_Data.Rows.Count);
                data_Data.Columns.Add("Err", typeof(string));
                data_Data.Columns["Err"].DefaultValue = "";
                double d = 0;
                DateTime date;
                string canlamsan = "";
                List<int> lstremove = new List<int>();
                int i = 0;
                foreach (DataRow row in data_Data.Rows)
                {

                    if (row["Ngày Tháng"] != null && row["Ngày Tháng"] != DBNull.Value)
                    {
                        if (row.Field<string>("Ngày Tháng").ToLower().Contains("tổng số ca"))
                        {
                            lstremove.Add(i);
                            //Sẽ remove dòng total này
                            if (row["STT"] != null && row["Ngày Tháng"] != DBNull.Value)
                            {
                                canlamsan = row.Field<string>("STT");
                            }
                        }
                        else
                        {

                            row["STT"] = canlamsan;
                        }
                    }
                    if (row["Khoa Chỉ Định"] == DBNull.Value || row["Khoa Chỉ Định"] == null)
                    {
                        lstremove.Add(i);
                    }
                    else if (String.IsNullOrEmpty(row.Field<string>("Khoa Chỉ Định")))
                    {
                        lstremove.Add(i);
                    }
                    i++;
                }
                var queryremove = lstremove.GroupBy(p => p).Select(p => p.Key).ToList();
                for (int k = queryremove.Count() - 1; k >= 0; k--)
                {
                    data_Data.Rows.RemoveAt(queryremove[k]);
                }
                data_Data.AcceptChanges();
                bool checkcolumn = false;
                for (int j = columnsfile; j >= 0; j--)
                {
                    checkcolumn = false;
                    foreach (string it in arrcolumncheck)
                    {
                        if (data_Data.Columns[j].ColumnName == it)
                        {
                            checkcolumn = true;
                            break;
                        }

                    }
                    if (!checkcolumn)
                    {
                        data_Data.Columns.RemoveAt(j);
                    }
                }
                data_Data.AcceptChanges();
                //Xử lý group theo người chỉ định và khoa chỉ định
                var queryngay = data_Data.AsEnumerable().GroupBy(p => new { STT = p.Field<string>("STT"), Ngay = p.Field<DateTime>("Ngay"), NguoiCD = p.Field<string>("Người C.Định"), KhoaCD = p.Field<string>("Khoa Chỉ Định") })
                   .Select(p => new { NguoiCD = p.Key.NguoiCD, STT = p.Key.STT, KhoaCD = p.Key.KhoaCD, Ngay = p.Key.Ngay, count = p.Count() }).ToList();
                var querynguoicd = queryngay.GroupBy(p => new { STT = p.STT, NguoiCD = p.NguoiCD, KhoaCD = p.KhoaCD })
                    .Select(p => new { NguoiCD = p.Key.NguoiCD, STT = p.Key.STT, KhoaCD = p.Key.KhoaCD, count = p.Sum(n => n.count) }).ToList();
                var querykhoacd = querynguoicd.GroupBy(p => new { STT = p.STT, KhoaCD = p.KhoaCD })
                   .Select(p => new { STT = p.Key.STT, KhoaCD = p.Key.KhoaCD, count = p.Sum(n => n.count) }).OrderBy(p => p.KhoaCD).ToList();

                var queryKhoa = querykhoacd.GroupBy(p => p.KhoaCD).Select(p => new { Khoa = p.Key.ToString(), Total = p.Sum(n => n.count) }).ToList();
                var querymaxmin = queryngay.GroupBy(p => 1).Select(p => new { Maxdate = p.Max(n => n.Ngay), Mindate = p.Min(n => n.Ngay) }).ToList();
                // Hiển thị DataTable đã được sắp xếp
                string ghichu = string.Format(" TỪ {0} ĐẾN {1}", querymaxmin[0].Mindate.ToString("dd-MM-yy"), querymaxmin[0].Maxdate.ToString("dd-MM-yy"));
                dttotal.Clear();
                dttotal.Columns.Clear();

                dttotal.Columns.Add("STT", typeof(string));
                dttotal.Columns.Add("Tên phòng khám", typeof(string));
                dttotal.Columns.Add("Số lượng CLS", typeof(int));
                i = 1;
                foreach (var it in queryKhoa)
                {
                    DataRow rownew = dttotal.NewRow();
                    rownew["STT"] = i;
                    rownew["Tên phòng khám"] = it.Khoa;
                    rownew["Số lượng CLS"] = it.Total;
                    dttotal.Rows.Add(rownew);
                    i++;
                }


                DataTable dtresult = new DataTable();
                dtresult.Columns.Add("Index", typeof(int));
                dtresult.Columns.Add("Khoa", typeof(string));
                dtresult.Columns.Add("Nguoi", typeof(string));
                dtresult.Columns.Add("Ngay", typeof(string));
                dtresult.Columns.Add("DienGiai", typeof(string));
                var querycolumn = querynguoicd.GroupBy(p => new { STT = p.STT }).Select(p => new { DienGiai = p.Key.STT }).OrderBy(p => p.DienGiai).ToList();
                foreach (var it in querycolumn)
                {
                    dtresult.Columns.Add(it.DienGiai, typeof(int));
                }
                dtresult.Columns.Add("TỔNG SỐ", typeof(int));

                //Add group khoa

                foreach (var it in queryKhoa)
                {
                    DataRow rownew = dtresult.NewRow();
                    rownew["Index"] = -2;
                    rownew["Khoa"] = it.Khoa;
                    rownew["DienGiai"] = it.Khoa;
                    var khoaitem = querykhoacd.Where(p => p.KhoaCD.Equals(it.Khoa));
                    foreach (var item in khoaitem)
                    {
                        rownew[item.STT] = item.count;
                    }
                    rownew["TỔNG SỐ"] = khoaitem.Sum(p => p.count);
                    dtresult.Rows.Add(rownew);
                }
                var queryNguoi = querynguoicd.GroupBy(p => new { Khoa = p.KhoaCD, Nguoi = p.NguoiCD }).Select(p => new { NguoiCD = p.Key.Nguoi.ToString(), Khoa = p.Key.Khoa, Total = p.Sum(n => n.count) }).ToList();
                foreach (var it in queryNguoi)
                {
                    DataRow rownew = dtresult.NewRow();
                    rownew["Index"] = -1;
                    rownew["Khoa"] = it.Khoa;
                    rownew["Nguoi"] = it.NguoiCD;
                    rownew["DienGiai"] = it.NguoiCD;
                    var nguoiitem = querynguoicd.Where(p => p.NguoiCD.Equals(it.NguoiCD) && p.KhoaCD == it.Khoa);//Kiểm tra tình trạng 1 người ở 2 khoa
                    foreach (var item in nguoiitem)
                    {
                        rownew[item.STT] = item.count;
                    }
                    rownew["TỔNG SỐ"] = nguoiitem.Sum(p => p.count);
                    dtresult.Rows.Add(rownew);
                }
                var queryNgaygroup = queryngay.GroupBy(p => new { Khoa = p.KhoaCD, Nguoi = p.NguoiCD, Ngay = p.Ngay }).Select(p => new { NguoiCD = p.Key.Nguoi.ToString(), Ngay = p.Key.Ngay, KhoaCD = p.Key.Khoa, Total = p.Sum(n => n.count) }).ToList();
                foreach (var it in queryNgaygroup)
                {
                    DataRow rownew = dtresult.NewRow();
                    rownew["Index"] = 0;
                    rownew["Khoa"] = it.KhoaCD;
                    rownew["Nguoi"] = it.NguoiCD;
                    rownew["Ngay"] = it.Ngay;
                    rownew["DienGiai"] = it.Ngay.ToString("dd-MM-yy");
                    var ngayitem = queryngay.Where(p => p.NguoiCD.Equals(it.NguoiCD) && p.KhoaCD == it.KhoaCD && p.Ngay.Equals(it.Ngay));//Kiểm tra tình trạng 1 người ở 2 khoa
                    foreach (var item in ngayitem)
                    {
                        rownew[item.STT] = item.count;
                    }
                    rownew["TỔNG SỐ"] = ngayitem.Sum(p => p.count);
                    dtresult.Rows.Add(rownew);
                }
                dtresult.DefaultView.Sort = "Khoa ASC,Nguoi ASC, Index ASC,Ngay ASC";
                dtresult = dtresult.DefaultView.ToTable();

                exportexcelAsync(dtresult, "", 5, 2, ghichu);
                //foreach (DataRowView row in table.DefaultView)
                //foreach(var it )
                data_Data.Clear();

                //prs.ExportExcel_HeaderwithNote(dtresult, Model.ModelAdmin.pathexcelexport, 2, 1, "", 1);
                //Kiểm tra kiểu dữ liệu, trường hợp không ép được thì trả ra bảng lỗi
                //string strtype = "";

            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Lỗi " + ex.Message));
                // msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
            finally
            {
                PreloadService.Hide();
            }

        }
        private async Task exportexcelAsync(DataTable dt, string path, int rowStart, int columnStart, string GhiChu)
        {
            XLColor lightyelllow = XLColor.FromHtml("#ffffb2");
            XLColor gray = XLColor.FromHtml("#e5f2e5");
            XLColor fontlevel1 = XLColor.FromHtml("#660033");
            XLColor lighgreen = XLColor.LightGreen;
            XLColor yellow = XLColor.Yellow;
            XLColor colorfont = XLColor.FromHtml("#0074bd");
            XLColor lighttotal = XLColor.FromHtml("#cce5cc");
            int rowEnd = rowStart + dt.Rows.Count;
            //int rowEnd = rowStart + dt.Columns.Count - 1;

            int columnEnd = columnStart + dt.Columns.Count - 1;
            string string_temp = "";
            string[] arr1 = new string[] { "DienGiai;Diễn giải" };
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet("Item");

                worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

                foreach (DataColumn cl in dt.Columns)
                {
                    string_temp = cl.ColumnName;
                    foreach (string it in arr1)
                    {
                        if (it.StartsWith(string_temp))
                        {
                            cl.ColumnName = it.Split(';')[1];
                            break;
                        }
                    }
                }
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
                IXLCell xLCelltittle = worksheet.Row(4).Cell(6);
                xLCelltittle.Value = string.Format("THỐNG KÊ CẬN LÂM SÀN {0}", GhiChu);
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
                object ob1 = null;
                IXLRow row0 = worksheet.Row(rowStart);
                row0.Style.Font.Bold = true;
                row0.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                // row0.Style.Font.FontColor = fontlevel1;

                for (int i = rowStart + 1; i < rowStart + dt.Rows.Count; i++)
                {
                    IXLRow row = worksheet.Row(i);
                    IXLCell cell = row.Cell(columnStart);
                    //row.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    //Microsoft.Office.Interop.Excel.Range cell = (Microsoft.Office.Interop.Excel.Range)row.Cells[1, 1];
                    //row.Interior.Color = Excel.XlRgbColor.rgbBlue;
                    //row.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
                    ob1 = cell.Value;
                    if (ob1 == null)
                    {
                        row.Style.Fill.BackgroundColor = lighgreen;
                        row.Style.Font.Bold = true;
                        //row.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbLightGreen;
                        //row.Font.Bold = true;
                    }
                    else
                    {
                        string_temp = ob1.ToString();
                        if (string_temp.Equals("-2"))
                        {
                            if (countrowrangeend - countrowrangebegin > 0)
                            {
                                if (countrowrangebegin > rowStart)
                                {
                                    lst_rowgroup.Add(countrowrangebegin.ToString() + ":" + (countrowrangeend).ToString());
                                    // Console.WriteLine(countrowrangebegin.ToString() + ":" + (countrowrangeend).ToString());
                                }
                            }
                            countrowrangebegin = countrowrangeend + 2;

                            row.Cells(columnStart, columnEnd).Style.Fill.BackgroundColor = lightyelllow;
                            // row.Interior.Color = lightyelllow;
                            row.Style.Font.Bold = true;
                            //row.EntireRow.Font.Bold = true;
                            row.Style.Font.FontColor = fontlevel1;
                            //row.EntireRow.Font.Color = colorfont;

                        }
                        if (string_temp.Equals("-1"))
                        {
                            //row.EntireRow.Font.Bold = true;
                            row.Style.Font.Bold = true;
                            row.Style.Font.FontColor = colorfont;
                            // row.EntireRow.Font.Color = colorfont;


                        }
                        //if (string_temp.Equals("2"))
                        //{
                        //    row.Cells(columnStart, columnEnd).Style.Fill.BackgroundColor = lighttotal;
                        //    row.Style.NumberFormat.Format = "0.###";
                        //    //row.Interior.Color = gray;
                        //    //row.NumberFormat = "#,0.0##";
                        //    row.Style.Font.Bold = true;
                        //    //row.EntireRow.Font.Bold = true;
                        //}

                    }
                    countrowrangeend++;
                }
                lst_rowgroup.Add(countrowrangebegin.ToString() + ":" + (countrowrangeend + 1).ToString());
                foreach (string it in lst_rowgroup)
                {
                    worksheet.Rows(it).Group();
                    //worksheet.Rows[it].Group();

                }
                for (int j = 2; j < 6; j++)
                {
                    worksheet.Column(j).Hide();
                }
                worksheet.CollapseRows(1);


                //foreach (var column in worksheet.ColumnsUsed())
                //{
                //    //column.AdjustToContents();
                //}
                worksheet.Column(6).Width = 30;
                for (int j = 7; j <= columnEnd; j++)
                {
                    worksheet.Column(j).Width = 10;
                    worksheet.Column(j).Style.Alignment.WrapText = true;
                    //worksheet.Column(j).Cells().Style.NumberFormat.Format = "#,#";
                    worksheet.Column(j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                }

                exportexcelTotalAsync(dttotal, "", 5, 2, GhiChu, workbook);
                //row0.Ho.Horizontal = XLAlignmentHorizontalValues.Right;
                // range.Columns.AutoFit();
                using (var outputStream = new MemoryStream())
                {
                    workbook.SaveAs(outputStream);
                    outputStream.Position = 0; // Đặt lại vị trí của stream để đọc từ đầu

                    // Trả về tệp Excel để người dùng tải xuống
                    var fileContent = outputStream.ToArray();

                    await File.WriteAllBytesAsync(fileName, fileContent);
                    linkfile = Convert.ToBase64String(fileContent);

                }
                // workbook.SaveAs(path);
                dt.Dispose();
            }

        }
        private void exportexcelTotalAsync(DataTable dt, string path, int rowStart, int columnStart, string GhiChu, XLWorkbook workbook)
        {
            try
            {
                XLColor lightyelllow = XLColor.FromHtml("#ffffb2");
                XLColor gray = XLColor.FromHtml("#e5f2e5");
                XLColor fontlevel1 = XLColor.FromHtml("#660033");
                XLColor lighgreen = XLColor.LightGreen;
                XLColor yellow = XLColor.Yellow;
                XLColor colorfont = XLColor.FromHtml("#0074bd");
                XLColor lighttotal = XLColor.FromHtml("#cce5cc");
                int rowEnd = rowStart + dt.Rows.Count;
                //int rowEnd = rowStart + dt.Columns.Count - 1;

                int columnEnd = columnStart + dt.Columns.Count - 1;
                string string_temp = "";
                string[] arr1 = new string[] { "DienGiai;Diễn giải" };

                var worksheet = workbook.AddWorksheet("Total");

                worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

                foreach (DataColumn cl in dt.Columns)
                {
                    string_temp = cl.ColumnName;
                    foreach (string it in arr1)
                    {
                        if (it.StartsWith(string_temp))
                        {
                            cl.ColumnName = it.Split(';')[1];
                            break;
                        }
                    }
                }
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
                IXLCell xLCelltittle = worksheet.Row(4).Cell(3);
                xLCelltittle.Value = string.Format("TỔNG HỢP {0}", GhiChu);
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
                // row0.Style.Font.FontColor = fontlevel1;
                //row0.Ho.Horizontal = XLAlignmentHorizontalValues.Right;
                // range.Columns.AutoFit();
                //dt.Dispose();
                //workbook.SaveAs(path);
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Lỗi SheetTotal: " + ex.Message));
                // msgBox.Show("Lỗi SheetTotal:" + ex.Message);
            }

        }
        int columnsfile = 0;
        private async Task<DataTable> LoadDataFromExcelAsync(InputFileChangeEventArgs e)
        {
            // Đường dẫn đến tệp Excel
            // Tạo DataTable để lưu dữ liệu từ Excel
            var file = e.File;
            DataTable dt = new DataTable();
            if (file != null && (file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || file.ContentType == "application/vnd.ms-excel"))
            {
                //using var stream = file.OpenReadStream(MaxFileSize);
                using var ms = new MemoryStream();
                using (var fileStream = file.OpenReadStream(MaxFileSize))
                {
                    await fileStream.CopyToAsync(ms);

                    columnsfile = 0;
                    using (var workbook = new XLWorkbook(ms))
                    {
                        var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên

                        // Thêm các cột vào DataTable
                        bool firstRow = true;
                        DateTime date;
                        int n = worksheet.LastColumnUsed().ColumnNumber();
                        List<int> lstcolumnselect = new List<int>();
                        int columnngaythang = 0;

                        foreach (var row in worksheet.RowsUsed())
                        {
                            if (firstRow)
                            {
                                for (int col = 1; col <= n; col++)
                                {
                                    var cell = row.Cell(col);
                                    // In ra địa chỉ của ô và giá trị của nó
                                    // Console.WriteLine($"Cell Address: {cell.Address}, Value: {cell.Value}");
                                    dt.Columns.Add(cell.Value.ToString());
                                }

                                firstRow = false;
                                bool check = false;
                                //Kiểm tra tên cột
                                int columnindex = 0;
                                foreach (string it in arrcolumncheck)
                                {
                                    check = false;

                                    for (columnindex = 0; columnindex < dt.Columns.Count; columnindex++)
                                    {
                                        DataColumn cl = dt.Columns[columnindex];
                                        if (cl.ColumnName == it)
                                        {

                                            lstcolumnselect.Add(columnindex + 1);//Vì cột trong excel bắt đầu đánh chỉ số từ 1
                                            check = true;
                                            if (cl.ColumnName == "Ngày Tháng")
                                            {
                                                columnngaythang = columnindex;
                                            }
                                            break;
                                        }
                                    }
                                    if (!check)
                                    {
                                        ToastService.Notify(new ToastMessage(ToastType.Success, string.Format("File thiếu cột {0}", it)));
                                        //msgBox.Show(string.Format("File thiếu cột {0}", it));
                                        return dt;
                                    }
                                }
                                columnsfile = dt.Columns.Count - 1;
                                //Để sau hàm duyệt này để tránh lỗi xảy ra 
                                dt.Columns.Add("Ngay", typeof(DateTime));
                                dt.Columns.Add("Wk", typeof(int));

                            }
                            else
                            {
                                DataRow rownew = dt.NewRow();


                                foreach (int col in lstcolumnselect)
                                {
                                    var cell = row.Cell(col);
                                    // In ra địa chỉ của ô và giá trị của nó
                                    rownew[col - 1] = cell.Value.ToString();//Cột trong excel đánh chỉ số từ 1, còn datatable đánh chỉ số từ 0
                                    if (col - 1 == columnngaythang)
                                    {
                                        if (DateTime.TryParseExact(cell.Value.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                        {
                                            rownew["Ngay"] = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
                                            rownew["Wk"] = getweek(date);
                                        }
                                    }
                                }

                                dt.Rows.Add(rownew);


                            }
                        }

                    }
                }
            }
            return dt;
        }
        System.Globalization.Calendar calendar;
        CalendarWeekRule weekRule = CalendarWeekRule.FirstFourDayWeek;
        DayOfWeek firstDayOfWeek = DayOfWeek.Wednesday;
        public int getweek(DateTime date)
        {
            // Lấy số tuần của ngày
            int weekOfYear = calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek);
            return weekOfYear;
            // Console.WriteLine($"Ngày {date.ToString("dd/MM/yyyy")} nằm trong tuần thứ {weekOfYear} của năm.");
            // return 0;
        }
        private bool checklogic()
        {
            foreach (DataRow row in data_Data.Rows)
            {

            }
            return true;
        }
    }
}
