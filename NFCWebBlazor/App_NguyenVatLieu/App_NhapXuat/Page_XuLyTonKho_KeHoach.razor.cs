using BlazorBootstrap;
using DevExpress.Blazor;


using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;

using System.Data;
using System.Globalization;


namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_XuLyTonKho_KeHoach
    {
        [Inject] ToastService toastService { get; set; }
        [Inject] IJSRuntime _jsRuntime { get; set; }
        List<itemnhapxuat> lstitemnhapxuat = new List<itemnhapxuat>();
        protected override async Task OnInitializedAsync()
        {
            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 70;
            heightgrid = string.Format("{0}px", height);
            await loadAsync();
            //return base.OnInitializedAsync();
        }
        private async Task loadAsync()
        {
            string[] arritem = new string[] { "KeHoach", "TonKho" };
            foreach (var it in arritem)
            {
                itemnhapxuat itemnhapxuat = new itemnhapxuat();
                itemnhapxuat.value = it;
                itemnhapxuat.text = it.Replace("KeHoach", "Nhu cầu").Replace("TonKho", "Kho đáp ứng được");//Thực hiện bao gồm tồn kho và nhập mới
                lstitemnhapxuat.Add(itemnhapxuat);
            }
        }

        class ItemReport
        {
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string NameGroup { get; set; }
            public string TextGroup { get; set; }
            public string DVT { get; set; }
            public DateTime Ngay { get; set; }
            public string QuyCach { get; set; }
            public string TenNhom { get; set; }
            public double SLNhap { get; set; }
            public double SLXuat { get; set; }
        }
        class itemnhapxuat
        {
            public string value;
            public string text;
            public itemnhapxuat(string _value, string _text)
            {
                value = _value;
                text = _text;

            }
            public itemnhapxuat()
            {

            }
        }
        bool expand = true;

        List<QuyDoiNgay> lstquydoi = new List<QuyDoiNgay>();
        string dieukienshow = "";

        RenderFragment renderFragmentcolumntotal;
        RenderFragment renderFragmentcolumn;
        private RenderFragment CreateColumns(List<itemnhapxuat> lstheader, List<itemnhapxuat> lstitem)
        {
            return builder =>
            {
                int sequence = 0;

                // Tạo DxGridBandColumn
                foreach (itemnhapxuat header in lstheader)
                {
                    //Console.WriteLine(header.text);
                    builder.OpenComponent<DxGridBandColumn>(sequence++);
                    builder.AddAttribute(sequence++, "Caption", header.text);
                    // Render các cột bên trong BandColumn
                    builder.AddAttribute(sequence++, "Columns", (RenderFragment)(nestedBuilder =>
                    {
                        foreach (var it in lstitem)
                        {
                            nestedBuilder.OpenComponent<DxGridDataColumn>(sequence++);
                            nestedBuilder.AddAttribute(sequence++, "FieldName", string.Format("{0}_{1}", header.value, it.value));
                            nestedBuilder.AddAttribute(sequence++, "Caption", it.text);
                            nestedBuilder.AddAttribute(sequence++, "Width", string.Format("{0}px", "85")); //" 110);
                            nestedBuilder.AddAttribute(sequence++, "DisplayFormat", "#,0.####;-#,0.##;''");
                            nestedBuilder.CloseComponent();
                        }
                    }));

                    builder.CloseComponent();
                }

            };
        }

        List<itemnhapxuat> lstheader = new List<itemnhapxuat>();
        List<itemnhapxuat> lsttextgroup = new List<itemnhapxuat>();
        List<itemnhapxuat> lstitem = new List<itemnhapxuat>();
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        DataTable dt = new DataTable();

        private async Task ShowImportAsync()
        {
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_ExportSanPham>(0);
                builder.CloseComponent();
            };
            await dxPopup.showAsync("IMPORT SẢN PHẨM");
            await dxPopup.ShowAsync();
        }
        string[] arrcolumncheck = new string[] { "MaSP", "SLSP", "MaMau", "Ngay" };

        public async Task ImportExcelAsync(string chitietorgop_)
        {
            chitietorgop = chitietorgop_;
            //CallAPI
            renderFragment = builder =>
            {
                builder.OpenComponent<ButtonImportExcel>(0);

                builder.AddAttribute(1, "arrcolumncheck", arrcolumncheck);
                builder.AddAttribute(2, "getdatatble", EventCallback.Factory.Create<DataTable>(this, GetTable));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
            await dxPopup.showAsync("Import hàng hóa từ excel");
            await dxPopup.ShowAsync();
        }
        string KieuChuyen = "";
        private DataTable InitTable()
        {
            if (dt.Columns.Count == 0)
            {
                dt.Columns.Add("Serial", typeof(int));
                dt.Columns.Add("MaSP", typeof(string));
                dt.Columns.Add("SLSP", typeof(double));
                dt.Columns.Add("MaMau", typeof(string));
                dt.Columns.Add("Ngay", typeof(DateTime));
                dt.Columns.Add("Err", typeof(string));

               
               

            }
            return dt;
        }
        string chitietorgop = "";
        private async void GetTable(DataTable dttemp)
        {
            try
            {
                PanelVisible = true;
                InitTable();
                dt.Clear();
                //dttemp.Columns.Add("Serial", typeof(int));
                bool check = false;
                string Err = "";
                foreach (string it in arrcolumncheck)
                {
                    check = false;
                    foreach (DataColumn cl in dt.Columns)
                    {

                        if (cl.ColumnName == it)
                        {
                            check = true;
                            break;
                        }
                    }
                    if (!check)
                    {
                        Err += string.Format("{0},", it);
                    }
                }
                if (Err != "")
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: file import thiếu cột: " + Err));
                    return;
                }
                await dxPopup.CloseAsync();
                int i = 1;
                double? SLSP = 0;
                DateTime? Ngay = DateTime.Now;
                DataTable dttempcopy = new DataTable();
                foreach (DataColumn cl in dt.Columns)
                {
                    dttempcopy.Columns.Add(cl.ColumnName, cl.DataType);
                }
                foreach (DataRow dr in dttemp.Rows)
                {
                    Err = "";
                    DataRow dataRow = dttempcopy.NewRow();
                    dataRow["Serial"] = i;
                    dataRow["MaSP"] = dr["MaSP"];
                    SLSP = StaticClass.ConvertNumberCultureInfo(dr["SLSP"]);

                    if (SLSP == null)
                    {
                        Err = "Sai định dạng số lượng, ";
                    }
                    else
                        dataRow["SLSP"] = SLSP;
                    dataRow["MaMau"] = dr["MaMau"];
                    Ngay = StaticClass.ConvertDateCultureInfo(dr["Ngay"]);
                    //Console.WriteLine(dr["Ngay"].ToString());
                    if (Ngay == null)
                    {
                        Err += "Sai định dạng ngày " + dr["Ngay"].ToString();
                    }
                    else
                        dataRow["Ngay"] = Ngay;
                    dataRow["Err"] = Err;
                    dttempcopy.Rows.Add(dataRow);
                    i++;
                }

                //Xử lý ngày
                if (string.IsNullOrEmpty(loaibaocao))
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Chọn loại gộp"));

                    return;
                }


                var querycheck = dttempcopy.AsEnumerable().FirstOrDefault(p => !string.IsNullOrEmpty(p.Field<string>("Err")));
                //Console.WriteLine("count: " + dt.Rows.Count.ToString());
                if (querycheck != null)
                {

                    renderFragment = builder =>
                    {
                        builder.OpenComponent<Urc_PageShowGrid>(0);
                        builder.AddAttribute(2, "dataTable", dttempcopy);
                        builder.CloseComponent();
                    };
                    //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
                    await dxPopup.showAsync("Dữ liệu Import bị lỗi");
                    await dxPopup.ShowAsync();
                }
                else
                {
                    var minmaxdate = dttempcopy.AsEnumerable().GroupBy(p => 1).Select(p => new { Mindate = p.Min(n => n.Field<DateTime>("Ngay")), Maxdate = p.Max(n => n.Field<DateTime>("Ngay")) }).FirstOrDefault();

                    DateTime mindate = minmaxdate.Mindate;
                    DateTime maxdate = minmaxdate.Maxdate;
                    if (loaibaocao == "Ngay")
                    {


                    }
                    if (loaibaocao == "Tuan")
                    {
                        foreach (DataRow row in dttempcopy.Rows)
                        {
                            row["Ngay"] = prs.GetFirstDayOfWeek(row.Field<DateTime>("Ngay"));
                        }
                        KieuChuyen = "W";
                    }
                    if (loaibaocao == "Thang")
                    {
                        foreach (DataRow row in dttempcopy.Rows)
                        {
                            row["Ngay"] = prs.GetFirstDayOfMonth(row.Field<DateTime>("Ngay"));
                        }
                        KieuChuyen = "M";
                    }
                    if (loaibaocao == "Quy")
                    {
                        foreach (DataRow row in dttempcopy.Rows)
                        {
                            row["Ngay"] = prs.GetFirstDayOfQuarter(row.Field<DateTime>("Ngay"));
                        }
                        KieuChuyen = "Q";
                    }
                    if (loaibaocao == "Nam")
                    {
                        foreach (DataRow row in dttempcopy.Rows)
                        {
                            row["Ngay"] = prs.GetFirstDayOfYear(row.Field<DateTime>("Ngay"));
                        }
                        KieuChuyen = "Y";
                    }
                  
                    //Gom nhóm theo ngày của bảng dt
                    //var querygr= dt.AsEnumerable().GroupBy(p => new { Ngay = p.Field<DateTime>("Ngay") }).Select(p => new { Ngay = p.Key.Ngay, }).FirstOrDefault();
                    var query = dttempcopy.AsEnumerable().GroupBy(p => new { Ngay = p.Field<DateTime>("Ngay"), MaSP = p.Field<string>("MaSP"), MaMau = p.Field<string>("MaMau") }).Select(p => new { Ngay = p.Key.Ngay, MaMau = p.Key.MaMau, MaSP = p.Key.MaSP, SLSP = p.Sum(n => n.Field<double>("SLSP")) }).ToList();
                    i = 1;
                    foreach(var it  in query)
                    {
                        DataRow dataRow=dt.NewRow();
                        dataRow["Serial"] = i;
                        dataRow["Ngay"] = it.Ngay;
                        dataRow["MaMau"] = it.MaMau;
                        dataRow["MaSP"] = it.MaSP;
                        dataRow["SLSP"] = it.SLSP;
                        dt.Rows.Add(dataRow);
                        i++;
                    }
                    await loaddataAsync(dt,mindate,maxdate, chitietorgop);
                }
                //await dxPopup.CloseAsync();
                dttemp.Dispose();
                PanelVisible = false;
            }
            catch (Exception ex)
            {

                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }
            //StateHasChanged();

        }
       public class TonKhoList
        {
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string DVT { get; set; }
            public double SLKeHoach { get; set; }
            public double SLTon { get; set; }
            public double SLNhap { get; set; }
            public double DonGia { get; set; }
            public double SLXuat { get; set; }
            public double TyLe { get; set; }
            public double SLNhapConLai { get; set; }
            public double SLTTonKhoConLai { get; set; }
            public double SLXuatConLai { get; set; }
        }
        List<DateTime> lstdate = new List<DateTime>();

        private async Task loaddataAsync(DataTable dt,DateTime mindate,DateTime maxdate,string loaibaocao)
        {
            string lstsp = "";
            var query = dt.AsEnumerable().GroupBy(p => new { MaSP = p.Field<string>("MaSP") }).Select(p => new { MaSP = p.Key.MaSP }).GroupBy(p => 1).Select(p => new { sp = string.Join(";", p.Select(p => p.MaSP)) }).FirstOrDefault();
            lstsp = query.sp;
            //string sql= string.Format(@"use NVLDB EXEC GetDinhMucNVL_SanPhamList_TonKho  @lstsanpham = N'{0}',@dateend = '{1}'
            //select * from ##tmpdinhmuctoancuc
            //DROP TABLE ##tmpdinhmuctoancuc",lstsp,dtpend.Value.ToString("MM/dd/yyyy 23:59"));

            DataTable dtsave = new DataTable(); //Khai báo cho khớp cột vs Type_KeHoachTonKho
            dtsave.Columns.Add("MaSP", typeof(string));
            dtsave.Columns.Add("SLSP", typeof(double));
            dtsave.Columns.Add("MaMau", typeof(string));
            dtsave.Columns.Add("Ngay", typeof(DateTime));
            foreach (DataRow dr in dt.Rows)
            {
                DataRow dataRow = dtsave.NewRow();
                dataRow["MaSP"] = dr["MaSP"];
                dataRow["SLSP"] = dr["SLSP"];
                dataRow["MaMau"] = dr["MaMau"];
                dataRow["Ngay"] = dr["Ngay"];
                dtsave.Rows.Add(dataRow);
            }

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@lstsanpham", lstsp));
            lstpara.Add(new ParameterDefine("@datebegin", mindate.ToString("MM/dd/yyyy 00:00")));
            lstpara.Add(new ParameterDefine("@dateend", maxdate.ToString("MM/dd/yyyy 23:59")));
            lstpara.Add(new ParameterDefine("@Type_KeHoachTonKho", prs.ConvertDataTableToJson(dtsave), "DataTable"));
            // dtsave.Dispose();
            PanelVisible = true;
            CallAPI callAPI = new CallAPI();
            try
            {
                string json = await callAPI.ProcedureEncryptAsync("NVLDB.dbo.GetDinhMucNVL_KeHoach_TonKho_Ver2", lstpara);
                if (json != "")
                {
                    DataTable dtdinhmuc =Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(json);
                   // dtdinhmuc.Columns["SLTon"].DataType = typeof(double);
                    await _jsRuntime.InvokeVoidAsync(CallNameJson.toggleCollapse.ToString(), string.Format("#{0}", randomdivhide));

                    DataView dv = dtsave.DefaultView;
                    dv.Sort = "Ngay ASC,MaSP ASC";  // Có thể đổi thành "ID DESC"
                    DataTable sortedDt = dv.ToTable();
                    
                    //foreach(DataColumn cl in dtdinhmuc.Columns)
                    //{
                    //    Console.WriteLine(string.Format("Name:{0},Type: {1}",cl.ColumnName,cl.DataType));
                    //}
                  
                    var qrytonkho = dtdinhmuc.AsEnumerable().Where(p => p["SLTon"] != DBNull.Value)
                        .GroupBy(p => p.Field<string>("MaVatTu"))
                        .Select(p => new TonKhoList { MaHang = p.Key, SLTon = p.Min(p => double.Parse(p["SLTon"].ToString(), CultureInfo.InvariantCulture)) , SLNhap = p.Min(p => p.Field<double>("SLNhap")),SLXuat = p.Min(p => p.Field<double>("SLXuat")),DonGia=p.Min(p=>p.Field<double>("DonGia")) }).Select(p=>new TonKhoList { MaHang = p.MaHang, SLTon = p.SLTon, SLNhap = p.SLNhap,SLNhapConLai=p.SLNhap,SLTTonKhoConLai=p.SLTon,SLXuat=p.SLXuat,SLXuatConLai=p.SLXuat,DonGia=p.DonGia }).ToList();
                    DataTable dtxuly = new DataTable();
                    dtxuly.Columns.Add("Index", typeof(int));
                    dtxuly.Columns.Add("MaSP", typeof(string));
                    dtxuly.Columns.Add("GroupMauSP", typeof(string));
                    dtxuly.Columns.Add("MaHang", typeof(string));
                    dtxuly.Columns.Add("SLKeHoach", typeof(double));
                    dtxuly.Columns.Add("SLTonKho", typeof(double));
                    dtxuly.Columns.Add("SLNhap", typeof(double));
                    dtxuly.Columns.Add("SLXuat", typeof(double));
                    dtxuly.Columns.Add("TyLe", typeof(double));
                    dtxuly.Columns.Add("Ngay", typeof(DateTime));
                   
                    tonkhotitle=string.Format("Tồn kho trước {0}",mindate.ToString("dd/MM/yy"));
                    nhapkhotitle=string.Format("Nhập mua mới từ {0} - {1}", mindate.ToString("dd/MM/yy"),maxdate.ToString("dd/MM/yy"));
                    xuatkhotitle = string.Format("Xuất kho từ {0} - {1}", mindate.ToString("dd/MM/yy"), maxdate.ToString("dd/MM/yy"));
                    double dtmp = 0;
                 
                    foreach (DataRow dataRow in sortedDt.Rows)
                    {
                        if (dataRow.Field<double>("SLSP") <= 0)
                            continue;
                        //Lấy danh sách mã hàng trong định mức
                        var querydinhmuc = dtdinhmuc.Select(string.Format("MaSP='{0}' and GroupMauSP='{1}'", dataRow["MaSP"], dataRow["MaMau"]));
                        foreach (DataRow dr in querydinhmuc)
                        {
                            DataRow dataRowdinhmuc = dtxuly.NewRow();
                            dataRowdinhmuc["Index"] = dr["Index"];
                            dataRowdinhmuc["MaSP"] = dr["MaSP"];
                            dataRowdinhmuc["GroupMauSP"] = dr["GroupMauSP"];
                            dataRowdinhmuc["MaHang"] = dr["MaVatTu"];
                            dtmp = dataRow.Field<double>("SLSP") * dr.Field<double>("SLQuyDoi");
                            dataRowdinhmuc["SLKeHoach"] = dtmp;
                            dataRowdinhmuc["SLTonKho"] = 0;
                            dataRowdinhmuc["SLNhap"] =0;
                            dataRowdinhmuc["SLXuat"] = 0;
                            dataRowdinhmuc["TyLe"] = 0;
                            dataRowdinhmuc["Ngay"] = dataRow["Ngay"];
                            if (dtmp == 0) break;
                            //Xử lý tồn kho luôn
                            var querytk = qrytonkho.Where(p => p.MaHang == dr.Field<string>("MaVatTu")).FirstOrDefault();
                           
                           
                            if (querytk != null)
                            {
                               //Xử lý số lượng xuất trước vì số lượng xuất chỉ cần so với kế hoạch thôi
                               if(dtmp<=querytk.SLXuatConLai)
                                {
                                    querytk.SLXuatConLai = querytk.SLXuatConLai - dtmp;
                                    dataRowdinhmuc["SLXuat"] = dtmp;
                                    
                                }
                                else
                                {
                                    dataRowdinhmuc["SLXuat"] = querytk.SLXuatConLai;
                                    querytk.SLXuatConLai = 0;
                                    
                                }
                               //Xử lý tồn kho và SLNhap
                                if (dtmp <= querytk.SLTTonKhoConLai)
                                {
                                    querytk.SLTTonKhoConLai = querytk.SLTTonKhoConLai - dtmp;
                                    dataRowdinhmuc["SLTonKho"] = dtmp;
                                    dataRowdinhmuc["TyLe"] = 1;

                                    dtmp = 0;
                                }
                                else
                                {
                                    dataRowdinhmuc["SLTonKho"] = dataRowdinhmuc.Field<double>("SLTonKho") + querytk.SLTTonKhoConLai;
                                    dtmp=dtmp-querytk.SLTTonKhoConLai;
                                    //Xử lý tiếp phần nhập kho
                                   // dnhap = dtmp - querytk.SLTTonKhoConLai;
                                    querytk.SLTTonKhoConLai = 0;
                                   
                                    if (querytk.SLNhapConLai>0)
                                    {
                                        
                                        if (dtmp <= querytk.SLNhapConLai)
                                        {
                                           
                                            dataRowdinhmuc["SLNhap"] = dtmp;
                                            querytk.SLNhapConLai=querytk.SLNhapConLai- dtmp;
                                            dtmp = 0;
                                        }
                                        else
                                        {
                                            dataRowdinhmuc["SLNhap"] = querytk.SLNhapConLai;
                                            dtmp = dtmp - querytk.SLNhapConLai;
                                            querytk.SLNhapConLai = 0;
                                           
                                        }
                                    }
                                    
                                 
                                }

                            }
                            dtxuly.Rows.Add(dataRowdinhmuc);
                        }

                    }
                    //Xử lý để gắn vào bảng định mức tổng thể
                    dtdinhmuc.Columns.Add(string.Format("{0}_KeHoach", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_TonKho", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_SLNhap", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_TyLe", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_SLXuat", "Total"), typeof(double));

                    var querytotal = dtxuly.AsEnumerable().GroupBy(p => p.Field<int>("Index")).Select(p => new { Index = p.Key, SLKeHoach = p.Sum(p => p.Field<double>("SLKeHoach")), SLTonKho = p.Sum(p => p.Field<double>("SLTonKho")),SLNhap=p.Sum(p => p.Field<double>("SLNhap")),SLXuat=p.Sum(p => p.Field<double>("SLXuat")) }).ToList();
                    var queryngay = sortedDt.AsEnumerable().GroupBy(p => p.Field<DateTime>("Ngay")).Select(p => new { Ngay = p.Key }).OrderBy(p => p.Ngay).ToList();
                   
                    lstdate.Clear();
                    foreach (var it in queryngay)
                    {
                        lstdate.Add(it.Ngay);
                    }
                   

                    //Xử lý hiển thị text ở Band
                   

                    List<itemnhapxuat> lstheader = new List<itemnhapxuat>();
                    foreach (var it in queryngay)
                    {
                        itemnhapxuat item = new itemnhapxuat();
                        item.value = it.Ngay.ToString("yyyy-MM-dd");
                        item.text = it.Ngay.ToString("yyyy-MM-dd");
                        lstheader.Add(item);
                    }
                    if (KieuChuyen != "Ngay")
                    {
                        lstquydoi = prs.getDayWeekMonthYear(lstdate, KieuChuyen);
                        foreach (var it in lstquydoi)
                        {
                            foreach (itemnhapxuat band in lstheader)
                            {
                                if (band.value == it.Ngay.ToString("yyyy-MM-dd"))
                                {
                                    band.text = it.Ngayoutput;
                                    break;
                                }
                            }
                        }

                    }
                    //else
                    //{
                    //    foreach (var it in lstdate)
                    //    {
                    //        QuyDoiNgay quyDoiNgay = new QuyDoiNgay(it, it.ToString("dd-MM-yy"));
                    //        lstquydoi.Add(quyDoiNgay);
                    //    }
                    //}
                    //await prs.exportexcelAsync(_jsRuntime, dtdinhmuc, 2, 1, "Test");

                    int index = 0;
                    foreach (var it in querytotal)
                    {
                        foreach (DataRow rowdm in dtdinhmuc.Rows)
                        {
                            if (it.Index == rowdm.Field<Int64>("Index"))
                            {
                               
                                rowdm[string.Format("{0}_KeHoach", "Total")] = it.SLKeHoach;
                                rowdm[string.Format("{0}_TonKho", "Total")] = it.SLTonKho;
                                rowdm[string.Format("{0}_SLNhap", "Total")] = it.SLNhap;
                                rowdm[string.Format("{0}_SLXuat", "Total")] = it.SLXuat;
                                rowdm[string.Format("{0}_TyLe", "Total")] = (it.SLTonKho+it.SLNhap) / it.SLKeHoach;
                                break;
                            }

                        }
                    }
                   
                    //Xử lý cột
                    int n=dtdinhmuc.Rows.Count;
                    //Xử lý vét cạn đối vs những mã hàng còn số tồn và còn nhập
                    foreach (var it in qrytonkho)
                    {
                        if (it.SLTTonKhoConLai +it.SLTTonKhoConLai== 0)
                        {
                            continue;
                        }
                        for(int i=n-1;i>=0;i--)
                        {
                            DataRow dataRow=dtdinhmuc.Rows[i];
                            if(dataRow.Field<string>("MaVatTu") ==it.MaHang)
                            {
                                dataRow[string.Format("{0}_TonKho", "Total")] = dataRow.Field<double>(string.Format("{0}_TonKho", "Total"))+it.SLTTonKhoConLai;
                                dataRow[string.Format("{0}_SLNhap", "Total")] = dataRow.Field<double>(string.Format("{0}_SLNhap", "Total")) + it.SLNhapConLai;
                                dataRow[string.Format("{0}_SLXuat", "Total")] = dataRow.Field<double>(string.Format("{0}_SLXuat", "Total")) + it.SLXuatConLai;
                                it.SLNhapConLai = 0;
                                it.SLTTonKhoConLai = 0;
                                it.SLXuatConLai = 0;
                                break;
                            }
                        }
                    }
                    if(loaibaocao=="ChiTiet")
                    {
                        foreach (var it in queryngay)
                        {
                            dtdinhmuc.Columns.Add(string.Format("{0}_KeHoach", it.Ngay.ToString("yyyy-MM-dd")), typeof(double));
                            dtdinhmuc.Columns.Add(string.Format("{0}_TonKho", it.Ngay.ToString("yyyy-MM-dd")), typeof(double));

                            //dtdinhmuc.Columns.Add(string.Format("TyLe_{0}", it.Ngay.ToString("yyyy-MM-dd"), typeof(double)));
                        }
                        //Xử lý gán cột
                        foreach (DataRow rowxuly in dtxuly.Rows)
                        {

                            index = rowxuly.Field<int>("Index");

                            foreach (DataRow rowdm in dtdinhmuc.Rows)
                            {
                                if (rowdm.Field<Int64>("Index") == index)
                                {
                                    rowdm[string.Format("{0}_KeHoach", rowxuly.Field<DateTime>("Ngay").ToString("yyyy-MM-dd"))] = rowxuly["SLKeHoach"];
                                    rowdm[string.Format("{0}_TonKho", rowxuly.Field<DateTime>("Ngay").ToString("yyyy-MM-dd"))] = rowxuly.Field<double>("SLTonKho") + rowxuly.Field<double>("SLNhap");
                                    //rowdm[string.Format("TyLe_{0}", rowxuly.Field<DateTime>("Ngay").ToString("yyyy-MM-dd"))] = rowxuly["TyLe"];
                                    break;
                                }
                            }
                        }
                        renderFragmentcolumn = CreateColumns(lstheader, lstitemnhapxuat);
                        dtsource = dtdinhmuc;
                        dxGrid.Reload();
                    }
                    else
                    {
                        DataTable dtgr= new DataTable();
                        dtgr.Columns.Add("TenNhom", typeof(string));
                        dtgr.Columns.Add("DonGia", typeof(double));
                        dtgr.Columns.Add("PhanLoai", typeof(string));
                        dtgr.Columns.Add("MaHang", typeof(string));
                        dtgr.Columns.Add("TenHang", typeof(string));
                        dtgr.Columns.Add("DVT", typeof(string));
                        dtgr.Columns.Add(string.Format("{0}_KeHoach", "Total"), typeof(double));
                        dtgr.Columns.Add(string.Format("{0}_TonKho", "Total"), typeof(double));
                        dtgr.Columns.Add(string.Format("{0}_KHNhap", "Total"), typeof(double));
                        dtgr.Columns.Add(string.Format("{0}_SLNhap", "Total"), typeof(double));
                        dtgr.Columns.Add(string.Format("{0}_TyLe", "Total"), typeof(double));
                        dtgr.Columns.Add(string.Format("{0}_SLXuat", "Total"), typeof(double));
                        foreach (var it in queryngay)
                        {
                            dtgr.Columns.Add(string.Format("{0}_KeHoach", it.Ngay.ToString("yyyy-MM-dd")), typeof(double));
                            dtgr.Columns.Add(string.Format("{0}_TonKho", it.Ngay.ToString("yyyy-MM-dd")), typeof(double));

                            //dtdinhmuc.Columns.Add(string.Format("TyLe_{0}", it.Ngay.ToString("yyyy-MM-dd"), typeof(double)));
                        }
                       
                        var dttotal = dtdinhmuc.AsEnumerable().GroupBy(p => new { MaHang = p.Field<string>("MaVatTu"),PhanLoai=p.Field<string>("PhanLoai"),TenNhom=p.Field<string>("TenNhom"), TenHang = p.Field<string>("TenHang"), DVT = p.Field<string>("DVT") })
                           .Select(p => new  { MaHang = p.Key.MaHang, TenHang = p.Key.TenHang, DVT = p.Key.DVT,PhanLoai=p.Key.PhanLoai,TenNhom=p.Key.TenNhom, SLKeHoach = p.Sum(n => n.Field<double>("Total_KeHoach")), SLTon = p.Sum(n => n.Field<double>("Total_TonKho")), SLNhap = p.Sum(n => n.Field<double>("Total_SLNhap")), SLXuat = p.Sum(n => n.Field<double>("Total_SLXuat")),DonGia=p.Min(n => n.Field<double>("DonGia")) }).ToList();

                        foreach (var it in dttotal)
                        {
                            DataRow rownew = dtgr.NewRow();
                            rownew["TenNhom"] = it.TenNhom;
                            rownew["DonGia"] = it.DonGia;
                            rownew["PhanLoai"] = it.PhanLoai;
                            rownew["MaHang"] = it.MaHang;
                            rownew["TenHang"] = it.TenHang;
                            rownew["DVT"] = it.DVT;
                            rownew["Total_KeHoach"] = it.SLKeHoach;
                            rownew["Total_TonKho"] = it.SLTon;
                            rownew["Total_KHNhap"] = (it.SLKeHoach - it.SLTon<0)?0:(it.SLKeHoach - it.SLTon);
                            rownew["Total_SLNhap"] = it.SLNhap;
                            rownew["Total_SLXuat"] = it.SLXuat;
                            rownew["Total_TyLe"] = (it.SLKeHoach<=0)?0:((it.SLNhap+it.SLTon)/it.SLKeHoach);
                           // Console.WriteLine(it.MaHang);
                            dtgr.Rows.Add(rownew);
                        }
                       
                        var queryxulygr = dtxuly.AsEnumerable().GroupBy(p => new { MaHang = p.Field<string>("MaHang"), Ngay = p.Field<DateTime>("Ngay") })
                           .Select(p => new {
                               MaHang = p.Key.MaHang,
                               Ngay = p.Key.Ngay,
                               SLTon = p.Sum(p => p.Field<double>("SLTonKho")),
                               SLNhap = p.Sum(p => p.Field<double>("SLNhap")),
                               SLXuat = p.Sum(p => p.Field<double>("SLXuat")),
                               SLKeHoach = p.Sum(p => p.Field<double>("SLKeHoach"))
                           }).ToList();
                        foreach (var rowxuly in queryxulygr)
                        {
                            foreach (DataRow rowdm in dtgr.Rows)
                            {
                                if (rowdm.Field<string>("MaHang") == rowxuly.MaHang)
                                {
                                    rowdm[string.Format("{0}_KeHoach", rowxuly.Ngay.ToString("yyyy-MM-dd"))] = rowxuly.SLKeHoach;
                                    rowdm[string.Format("{0}_TonKho", rowxuly.Ngay.ToString("yyyy-MM-dd"))] = rowxuly.SLTon + rowxuly.SLNhap;
                                    //rowdm[string.Format("TyLe_{0}", rowxuly.Field<DateTime>("Ngay").ToString("yyyy-MM-dd"))] = rowxuly["TyLe"];
                                    break;
                                }
                            }
                        }
                        dtsourcegr = dtgr;
                        renderFragmentcolumntotal = CreateColumns(lstheader, lstitemnhapxuat);
                        dxGridtotal.Reload();
                    }
                    //await prs.exportexcelAsync(_jsRuntime, dtdinhmuc, 2, 1, "Test");
                    dtxuly.Dispose();
                    querytotal.Clear();
                    StateHasChanged();
                    //string s = "";
                    // await GotoMainForm.InvokeAsync();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi: " + ex.Message));

            }
            finally
            {
                PanelVisible = false;

                StateHasChanged();

            }
        }
    }

}
