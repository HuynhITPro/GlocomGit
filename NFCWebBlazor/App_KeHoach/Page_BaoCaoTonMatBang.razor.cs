using BlazorBootstrap;
using ClosedXML.Excel;
using DevExpress.Blazor;
using DevExpress.Export.Xl;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.Model;
using System.Data;
using static NFCWebBlazor.App_KeHoach.Page_BieuDoHangHoa;

namespace NFCWebBlazor.App_KeHoach
{

    public partial class Page_BaoCaoTonMatBang
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] BrowserService browserService { get; set; }
        [Inject] IJSRuntime JS { get; set; }
        public class Indexmearge
        {
            public string key { get; set; }
            public int indexbegin { get; set; }
            public int indexend { get; set; }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _ = loadAsync();

            }
            try
            {
                var dimension = await browserService.GetDimensions();
                // var heighrow = await browserService.GetHeighWithID("divcontainer");
                int height = dimension.Height - 70;
                heightgrid = string.Format("{0}px", height);
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi browserService :" + ex.Message));
            }
            //await JS.InvokeVoidAsync("scrollToBottomLast");

            //base.OnAfterRender(firstRender);
        }
        private async Task loadAsync()
        {
            try
            {
                var query = await Model.ModelData.GetSanPham();
                lstsanpham = query.ToList();
                var query2 = await Model.ModelData.GetListKhachHang();
                lstkhachhang = query2.ToList();
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
            }
            //baocaoselected = lstloaibaocao.FirstOrDefault(p => p.Name.Equals("Tồn kho theo sản phẩm"));
            StateHasChanged();

        }
        public class TonMatBangList
        {
            public string NhaMay { get; set; }
            public string KhachHang { get; set; }
            public List<string> lstmasp { get; set; } = new List<string>();
            public int DongBo { get; set; } = 1;
            public int SPMua { get; set; } = 1;
            public bool Quydoichitiet { get; set; } = false;
        }
        TonMatBangList tonMatBangList = new TonMatBangList();
        private async Task getTonMatBangAsync()
        {
            
            if (khachhangselected == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Vui lòng chọn khách hàng"));
                return;
            }
            if (nhamayselected == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Vui lòng chọn nhà máy"));
                return;
            }
            if(sanphamselected!=null)
            {
                tonMatBangList.lstmasp.Add(sanphamselected.MaSP);
            }
            PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
            try
            {
                textxuly = "Đang xử lý file....";
                linkfile = "";
                CallAPI callAPI = new CallAPI();
                
                tonMatBangList.NhaMay = nhamayselected.Name;
                tonMatBangList.KhachHang = khachhangselected.MaKh;
                string diengiai = "";
                tonMatBangList.Quydoichitiet = false;
                if(sanphamtype== "Sản phẩm đang sản xuất")
                    tonMatBangList.SPMua = 1;
                else
                    tonMatBangList.SPMua = 0;
                if (DVT == "ĐVT (bộ)")
                {
                    tonMatBangList.DongBo = 1;
                    diengiai = "ĐVT: bộ";
                }
                else
                {
                    tonMatBangList.DongBo = 0;
                    diengiai = "ĐVT: cái";
                }
                string sql = System.Text.Json.JsonSerializer.Serialize(tonMatBangList);
                string json = await callAPI.Gettonmatbang(sql);
                if (json != "")
                {
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                    diengiai += $", Nhà máy {tonMatBangList.NhaMay}, Khách hàng {tonMatBangList.KhachHang}";
                    await ExportExcel_TonKhoNhomAsync(dt, "", 6, 2,diengiai );
                    // return lstitem;
                }
                textxuly = "File đã xử lý xong.Vui lòng nhấn TẢI FILE !!!";
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Lỗi " + ex.Message));
                return;
            }
            finally
            {

                PreloadService.Hide();
            }
        }
        public string linkfile = "";
        public async Task ExportExcel_TonKhoNhomAsync(DataTable dt, string path, int rowStart, int columnStart, string GhiChu)
        {
            string string_temp = "";

            XLColor lightyelllow = XLColor.FromHtml("#ffffb2");
            XLColor gray = XLColor.FromHtml("#e5f2e5");
            XLColor lighgreen = XLColor.LightGreen;
            XLColor yellow = XLColor.Yellow;
            XLColor colorfont = XLColor.FromHtml("#0074bd");
            XLColor lighttotal = XLColor.FromHtml("#cce5cc");
            int rowEnd = rowStart + dt.Rows.Count;
            //int rowEnd = rowStart + dt.Columns.Count - 1;

            int columnEnd = columnStart + dt.Columns.Count - 1;

            string[] arr1 = new string[] { "KhachHang;Khách hàng", "MaCT;Mã CT", "MaSP;Mã SP", "TenCT;Tên chi tiết", "SoLuongCT;SLCT/SP", "SLCTTrongVe;SLCT/Vế", "SoKhoiCT;Số khối", "KV1X;Kho tinh chế", "KV1X DinhViHangMuon;Hàng mượn", "KV2 DinhVi;Tồn KĐục-ĐV", "KV3 KhoChoRap;Kho chờ ráp", "KV3 TonMBRap;Tồn mặt bằng ráp", "KV4 KhoNhung;Kho chờ nhúng", "KV4 TonMB;Tồn mặt bằng nhúng", "TonKTP;Tồn kho TP", "SLNKTP;Đơn hàng còn", "TongTonTinhChe;Tổng tồn tinh chế", "TongDongBo;Tổng Đồng bộ", "ChieuDayTC;Dày", "ChieuDaiTC;Dài", "ChieuRongTC;Rộng" };
            //Đánh dấu chỉ số để format
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet("Sheet1");
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
                //List<string>lstnumber=new List<string>();
                //for (int j = 0; j < dt.Columns.Count; j++)
                //{
                //    if (dt.Columns[j].DataType==typeof(int)|| dt.Columns[j].DataType == typeof(double))
                //    {
                //        lstnumber.Add(dt.Columns[j].ColumnName);
                //    }
                //}
                double d = 0;
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
                            if (c > 7)
                            {
                                rowdata.Cell(columnStart + c).Value = ((double.TryParse(dr[c].ToString(), out d) ? d : ""));
                                rowdata.Cell(columnStart + c).Style.NumberFormat.SetFormat("#,#");
                            }
                            else
                                rowdata.Cell(columnStart + c).Value = dr[c].ToString();

                        }


                    }
                }
                var rangeborder = worksheet.RangeUsed();
                rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                IXLCell xLCelltittle = worksheet.Row(3).Cell(7);
                xLCelltittle.Value = "BÁO CÁO TỔNG HỢP TỒN MẶT BẰNG";
                xLCelltittle.Style.Font.Bold = true;
                xLCelltittle.Style.Font.FontSize = 20;
                DateTime now = DateTime.Now;
                worksheet.Row(1).Cell(7).Value = now.Hour.ToString() + " giờ, " + now.Minute.ToString() + " phút, " + "Ngày " + now.Day.ToString() + " tháng " + now.Month.ToString() + " năm " + now.Year.ToString(); ;
                //worksheet.Cells[1, 6] = now.Hour.ToString() + " giờ, " + now.Minute.ToString() + " phút, " + "Ngày " + now.Day.ToString() + " tháng " + now.Month.ToString() + " năm " + now.Year.ToString();
                //Điền dữ liệu vào vùng đã thiết lập
                worksheet.Row(5).Cell(7).Value = GhiChu;
                //Thiết lập vùng điền dữ liệu
                IXLCell c1 = worksheet.Row(rowStart).Cell(columnStart);
                IXLCell c2 = worksheet.Row(rowStart).Cell(columnStart);

                IXLRange range = worksheet.Range(rowStart, columnStart, rowbegindata + dt.Rows.Count, columnEnd);

                //range.Borders.LineStyle = Microsoft.Office.Interop.Excel.Constants.xlSolid;
                object ob1 = null;
                IXLRow row0 = worksheet.Row(rowStart);
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
                        if (string_temp.Equals("-1"))
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
                            row.Style.Font.FontColor = colorfont;
                            //row.EntireRow.Font.Color = colorfont;

                        }
                        if (string_temp.Equals("0"))
                        {
                            //row.EntireRow.Font.Bold = true;
                            row.Style.Font.Bold = true;
                            row.Style.Font.FontColor = colorfont;
                            // row.EntireRow.Font.Color = colorfont;


                        }
                        if (string_temp.Equals("2"))
                        {
                            row.Cells(columnStart, columnEnd).Style.Fill.BackgroundColor = lighttotal;
                            row.Style.NumberFormat.Format = "0.###";
                            //row.Interior.Color = gray;
                            //row.NumberFormat = "#,0.0##";
                            row.Style.Font.Bold = true;
                            //row.EntireRow.Font.Bold = true;
                        }

                    }
                    countrowrangeend++;
                }
                lst_rowgroup.Add(countrowrangebegin.ToString() + ":" + (countrowrangeend + 1).ToString());
                foreach (string it in lst_rowgroup)
                {
                    worksheet.Rows(it).Group();
                    //worksheet.Rows[it].Group();

                }
                worksheet.CollapseRows(1);

                //Xử lý cột màu. mearge trong excel
                List<Indexmearge> lstmearge = new List<Indexmearge>();
                int count = 0;
                bool checkmearge = false;
                foreach (DataColumn cl in dt.Columns)
                {
                    checkmearge = false;
                    if (cl.ColumnName.Contains("KhoNhung_"))
                    {
                        foreach (var it in lstmearge)
                        {
                            if (it.key == "KhoNhung_")
                            {
                                it.indexend = columnStart + count;
                                checkmearge = true;
                                break;
                            }
                        }
                        if (!checkmearge)
                        {
                            Indexmearge indexmearge = new Indexmearge();
                            indexmearge.key = "KhoNhung_";
                            indexmearge.indexbegin = columnStart + count;
                            indexmearge.indexend = columnStart + count;
                            lstmearge.Add(indexmearge);
                        }
                    }

                    checkmearge = false;
                    if (cl.ColumnName.Contains("MBNhung_"))
                    {
                        foreach (var it in lstmearge)
                        {
                            if (it.key == "MBNhung_")
                            {
                                it.indexend = columnStart + count;
                                checkmearge = true;
                                break;
                            }
                        }
                        if (!checkmearge)
                        {
                            Indexmearge indexmearge = new Indexmearge();
                            indexmearge.key = "MBNhung_";
                            indexmearge.indexbegin = columnStart + count;
                            indexmearge.indexend = columnStart + count;
                            lstmearge.Add(indexmearge);
                        }
                    }

                    count++;
                }

                //Microsoft.Office.Interop.Excel.Range rowbegin = range.Rows[0];
                foreach (var it in lstmearge)
                {
                    IXLCell cellbegin = worksheet.Cell(rowStart - 1, it.indexbegin);
                    IXLCell celleend = worksheet.Cell(rowStart - 1, it.indexend);
                    IXLRange rangemerge = worksheet.Range(cellbegin, celleend).Merge();
                    //    Microsoft.Office.Interop.Excel.Range rangebegin = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowStart - 1, it.indexbegin];
                    //Microsoft.Office.Interop.Excel.Range rangeend = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowStart - 1, it.indexend];
                    //Microsoft.Office.Interop.Excel.Range rangemerge = worksheet.get_Range(rangebegin, rangeend);
                    //rangemerge.Merge();
                    if (it.key == "KhoNhung_")
                        rangemerge.Value = "Kho Chờ nhúng";
                    if (it.key == "MBNhung_")
                        rangemerge.Value = "Mặt bằng nhúng";
                    rangemerge.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    //Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    rangemerge.Style.Fill.BackgroundColor = gray;
                    rangemerge.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    //rangemerge.Interior.Color = Color.LightGray;
                    //rangemerge.Borders.LineStyle = Microsoft.Office.Interop.Excel.Constants.xlSolid;
                    //rowbegin.Cells[1,it.indexbegin]
                }
                string s = "";
                bool checkhide = false;
                for (int j = columnStart; j <= columnEnd; j++)
                {
                    IXLCell xLCell = row0.Cell(j);
                    if (!row0.Cell(j).Value.IsBlank)
                    {
                        //Console.WriteLine(xLCell.Value.GetText());
                        checkhide = true;
                        string text = row0.Cell(j).Value.GetText();
                        foreach (var it in arr1)
                        {
                            if (it.Contains(text))
                            {
                                checkhide = false;
                                break;
                            }
                        }
                        if (checkhide)
                        {
                            worksheet.Column(j).Hide();
                        }

                    }
                    if (xLCell != null)
                    {
                        //Console.WriteLine(xLCell.Value);

                        if (!xLCell.Value.IsBlank)
                        {
                            s = xLCell.Value.GetText();
                            if (s.Contains("KhoNhung_") || s.Contains("MBNhung_"))
                            {
                                xLCell.Value = s.Replace("KhoNhung_", "").Replace("MBNhung_", "");
                            }
                        }
                    }

                }

                foreach (var column in worksheet.ColumnsUsed())
                {
                    column.AdjustToContents();
                }
                for (int j = 8; j <= columnEnd; j++)
                {

                    //worksheet.Column(j).Cells().Style.NumberFormat.Format = "#,#";
                    worksheet.Column(j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                }

                // range.Columns.AutoFit();
                dt.Dispose();
                using (var outputStream = new MemoryStream())
                {
                    workbook.SaveAs(outputStream);
                    outputStream.Position = 0; // Đặt lại vị trí của stream để đọc từ đầu

                    // Trả về tệp Excel để người dùng tải xuống
                    var fileContent = outputStream.ToArray();
                  
                    await File.WriteAllBytesAsync(fileName, fileContent);
                    linkfile=  Convert.ToBase64String(fileContent);
                   
                }

            }
        }
        string fileName = "TonMatBang.xlsx";
        private async Task downloadfileAsync()
        {
           
            await JS.InvokeVoidAsync("saveAsFile", fileName, linkfile);
        }
    }
}
