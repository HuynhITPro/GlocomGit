using BlazorBootstrap;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using NFCWebBlazor.App_ClassDefine;
using System.Data;
using System.Globalization;
using System.IO;

namespace NFCWebBlazor.App_ClassDefine
{
    public partial class ButtonImportExcel
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] BrowserService browserService { get; set; }
        [Inject] IJSRuntime JS { get; set; }
       
        string textxuly = "";
       
        public string linkfile = "";
        System.Globalization.Calendar calendar;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            calendar = CultureInfo.InvariantCulture.Calendar;
            return base.OnAfterRenderAsync(firstRender);
        }
      

        string[] formats = new string[] { "dd-MM-yy hh:mm:ss tt", "dd-MM-yy" };
        

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
                data_Data = await LoadDataFromExcelAsync(inputFile);
                bool checkcolumnerr = false;
                foreach(DataColumn cl in data_Data.Columns)
                {
                    if(cl.ColumnName =="Err")
                    {
                        checkcolumnerr= true;
                        break;
                    }
                }
                if (!checkcolumnerr)
                {
                    //Console.WriteLine("Số dòng: " + data_Data.Rows.Count);
                    data_Data.Columns.Add("Err", typeof(string));
                    data_Data.Columns["Err"].DefaultValue = "";
                }
                if (getdatatble.HasDelegate)
                {
                    await  getdatatble.InvokeAsync(data_Data);
                   
                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Lỗi " + ex.Message));
                // msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
            finally
            {
                
            }

        }

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
                                            check = true;
                                            lstcolumnselect.Add(columnindex + 1);//Vì cột trong excel bắt đầu đánh chỉ số từ 1
                                            break;
                                        }
                                    }
                                    if (!check)
                                    {
                                        textxuly = string.Format("File thiếu cột {0}", it);
                                        ToastService.Notify(new ToastMessage(ToastType.Success, string.Format("File thiếu cột {0}", it)));
                                        //msgBox.Show(string.Format("File thiếu cột {0}", it));
                                        return dt;
                                    }
                                }


                            }
                            else
                            {
                                DataRow rownew = dt.NewRow();
                                foreach (int col in lstcolumnselect)
                                {
                                    var cell = row.Cell(col);
                                    //Console.WriteLine(cell.Value);
                                    // In ra địa chỉ của ô và giá trị của nó
                                    rownew[col - 1] = cell.Value.ToString();//Cột trong excel đánh chỉ số từ 1, còn datatable đánh chỉ số từ 0
                                    //if (col - 1 == columnngaythang)
                                    //{
                                    //    if (DateTime.TryParseExact(cell.Value.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                    //    {
                                    //        rownew["Ngay"] = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
                                            
                                    //    }
                                    //}
                                }

                               
                                dt.Rows.Add(rownew);
                            }
                        }

                    }
                }
            }
            return dt;
        }
    }
}
