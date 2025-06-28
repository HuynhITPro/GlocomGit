using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat;
using NFCWebBlazor.Model;
using NFCWebBlazor.Pages;
using System.Data;
using System.Diagnostics;


namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class View_KeHoachMuaHangSP_Detail
    {
        [Inject] ToastService toastService { get; set; }
        [Inject] IJSRuntime _jsRuntime { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        List<itemnhapxuat> lstitemnhapxuat = new List<itemnhapxuat>();
        bool CheckQuyen = false;
        protected override async Task OnInitializedAsync()
        {
            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 70;
            heightgrid = string.Format("{0}px", height);
            InitcolumnRequired();
            CheckQuyen = await phanQuyenAccess.CreateDonDatHang(ModelAdmin.users);
            await loadAsync();


            //return base.OnInitializedAsync();
        }
        private void InitcolumnRequired()
        {
            lstcolumns.Add(new Classcolumn("KhachHang", "Khách hàng"));
            lstcolumns.Add(new Classcolumn("MaSP", "Mã SP"));
            lstcolumns.Add(new Classcolumn("TenSP", "Tên SP"));
            lstcolumns.Add(new Classcolumn("TenNCC", "Tên NCC"));
            lstcolumns.Add(new Classcolumn("CongDoan", "Công đoạn"));
            lstcolumns.Add(new Classcolumn("TenMau", "Tên màu"));
            lstcolumns.Add(new Classcolumn("MaVatTu", "Mã hàng"));
          
            lstcolumns.Add(new Classcolumn("TenHang", "Tên hàng"));
            lstcolumns.Add(new Classcolumn("GroupMauSP", "Mã màu"));
            lstcolumns.Add(new Classcolumn("DVT", "ĐVT"));
            lstcolumns.Add(new Classcolumn("SLQuyDoi", "SLQuyDoi"));
            lstcolumns.Add(new Classcolumn("Total_KeHoach", "Nhu cầu"));
            lstcolumns.Add(new Classcolumn("Total_TonKho", "Tồn kho"));
            lstcolumns.Add(new Classcolumn("Total_SLNhap", "Đã Nhập mới"));
            lstcolumns.Add(new Classcolumn("Total_TonMB", "Tồn MB"));
            lstcolumns.Add(new Classcolumn("Total_DHConNo", "ĐH chưa về"));
            lstcolumns.Add(new Classcolumn("TenNhom", "Nhóm hàng"));
            lstcolumns.Add(new Classcolumn("Total_SLDeNghi", "Phải đặt"));
            lstcolumns.Add(new Classcolumn("DonGia", "Đơn giá"));
        }
        string[] arrcolumncheck = new string[] { "Mã SP", "Công đoạn", "Mã hàng", "Mã màu", "Phải đặt" };
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
        RenderFragment renderFragmentcolumntotal;
        List<itemnhapxuat> lstheader = new List<itemnhapxuat>();
        List<itemnhapxuat> lsttextgroup = new List<itemnhapxuat>();
        List<itemnhapxuat> lstitem = new List<itemnhapxuat>();
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        DataTable dt = new DataTable();




        public async Task ImportExcelAsync()
        {
            //CallAPI

            renderFragment = builder =>
            {
                builder.OpenComponent<ButtonImportExcel>(0);
                builder.AddAttribute(1, "arrcolumncheck", arrcolumncheck);
                builder.AddAttribute(2, "getdatatble", EventCallback.Factory.Create<DataTable>(this, GetTable));
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
            await dxPopup.showAsync("Nạp từ file excel");
            await dxPopup.ShowAsync();
        }
        string KieuChuyen = "";
        private DataTable InitTable()
        {
            if (dt.Columns.Count == 0)
            {

                foreach (var it in arrcolumncheck)
                {
                    dt.Columns.Add(it, typeof(string));
                }
                dt.Columns["Phải đặt"].DataType = typeof(double);




            }
            return dt;
        }
        string chitietorgop = "";
        private async void GetTable(DataTable dttemp)
        {
            try
            {
                PanelVisible = true;
                StateHasChanged();
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
                dttempcopy.Columns["Phải đặt"].DataType = typeof(string);
                dttempcopy.Columns.Add("Err", typeof(string));
                dttempcopy.Columns.Add("checkid", typeof(string));

                foreach (DataRow dr in dttemp.Rows)
                {
                    Err = "";
                    DataRow dataRow = dttempcopy.NewRow();
                    SLSP = StaticClass.ConvertNumberCultureInfo(dr["Phải đặt"]);
                    if (SLSP == null)
                    {
                        Err = "Sai định dạng số lượng, ";

                    }
                    else
                        dataRow["Phải đặt"] = SLSP;
                    foreach (var column in arrcolumncheck)
                    {
                        if (dr[column] == DBNull.Value || dr[column] == null)
                        {
                            Err += "Lỗi giá trị bị rỗng, ";
                        }
                    }
                    dataRow["Err"] = Err;
                    if (!string.IsNullOrEmpty(Err))
                    {
                        foreach (var it in arrcolumncheck)
                        {
                            dataRow[it] = dr[it];
                        }
                        dttempcopy.Rows.Add(dataRow);
                    }

                    //else
                    //{
                    //    dr["checkid"] =string.Format("{0}_{1}_{2}", dr["Mã SP"], dr["Công đoạn"]
                    //}

                    i++;
                }

                //Xử lý ngày

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
                    string[] arrcolumnsosanh = new string[] { "Mã hàng;MaVatTu", "Mã SP;MaSP", "Công đoạn sử dụng;CongDoan", "Mã màu;GroupMauSP" };
                    bool checkcompare = false;

                    DataView dvsource = dtsource.DefaultView;
                    dvsource.Sort = "MaVatTu ASC";  // Có thể đổi thành "ID DESC"
                                                    //DataTable sortedDt = dv.ToTable();
                    DataView dvtemp = dttemp.DefaultView;
                    dvtemp.Sort = "[Mã hàng] ASC";

                    bool checkMaHang = false;
                    int indexbegin = 0;
                    int n = dtsource.Rows.Count;
                    foreach (DataRowView row in dvtemp)
                    {
                        DataRow rowtmp = row.Row;
                        //DataRow row = dvtemp[i].Row;
                        for (i = indexbegin; i < n; i++)
                        {
                            checkMaHang = false;
                            DataRow it = dvsource[i].Row;
                            if (rowtmp.Field<string>("Mã hàng") == it.Field<string>("MaVatTu"))
                            {
                                if (!checkMaHang)
                                {
                                    indexbegin = i;//Gán phần tử đầu tiên
                                    checkMaHang = true;
                                }
                                if (rowtmp.Field<string>("Mã SP") == it.Field<string>("MaSP") && rowtmp.Field<string>("Công đoạn") == it.Field<string>("CongDoan") && rowtmp.Field<string>("Mã màu") == it.Field<string>("GroupMauSP"))
                                {
                                    it["Total_SLDeNghi"] = rowtmp["Phải đặt"];

                                    Console.WriteLine(it["Total_SLDeNghi"].ToString());
                                    break;
                                }
                            }
                            else
                            {
                                if (checkMaHang)
                                {
                                    break;
                                }

                            }
                        }




                    }

                }
                //await dxPopup.CloseAsync();
                dttemp.Dispose();
                dxGrid.Reload();
               
                StateHasChanged();
                PanelVisible = false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Lỗi:" + ex.Message);
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
        class Classcolumn
        {
            public string columnName { get; set; }
            public string columnText { get; set; }
            public Classcolumn(string columnName, string columnText)
            {
                this.columnName = columnName;
                this.columnText = columnText;
            }
        }
        public async Task loaddataAsync(DataTable dt, DateTime mindate, DateTime maxdate, string loaibaocao)
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
                string json = await callAPI.ExcuteProcedureDatasetEncrypt(@"NVLDB.dbo.GetDinhMucNVL_KeHoach_TonKho_Ver2", lstpara);
                if (json != "")
                {
                    DataSet ds = JsonConvert.DeserializeObject<DataSet>(json);

                    DataTable dtdinhmuc = ds.Tables[0];

                    DataTable dtdonhang = ds.Tables[1];

                    await _jsRuntime.InvokeVoidAsync(CallNameJson.toggleCollapse.ToString(), string.Format("#{0}", randomdivhide));

                    DataView dv = dtsave.DefaultView;
                    dv.Sort = "Ngay ASC,MaSP ASC";  // Có thể đổi thành "ID DESC"
                    DataTable sortedDt = dv.ToTable();

                    //Xử lý để gắn vào bảng định mức tổng thể
                    dtdinhmuc.Columns.Add(string.Format("{0}_KeHoach", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_TonKho", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_SLNhap", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_TyLe", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_TyLeThieu", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_SLXuat", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_TonMB", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_DHConNo", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}_SLDeNghi", "Total"), typeof(double));
                    dtdinhmuc.Columns.Add(string.Format("{0}", "TenNCCShow"), typeof(string));
                    dtdinhmuc.Columns.Add("KeyGroup", typeof(string));
                    //
                    //Chuyển datacolumn về 0
                    foreach (DataRow row in dtdinhmuc.Rows)
                    {
                        row[string.Format("{0}_KeHoach", "Total")] = 0;
                        row[string.Format("{0}_TonKho", "Total")] = 0;
                        row[string.Format("{0}_SLNhap", "Total")] = 0;
                        row[string.Format("{0}_TyLe", "Total")] = 0;
                        row[string.Format("{0}_TyLeThieu", "Total")] = 0;
                        row[string.Format("{0}_SLXuat", "Total")] = 0;
                        row[string.Format("{0}_TonMB", "Total")] = 0;
                        row[string.Format("{0}_DHConNo", "Total")] = 0;
                        row[string.Format("{0}_SLDeNghi", "Total")] = 0;
                    }

                    //var querydinhmuctest = dtdinhmuc.Select(string.Format("MaSP='C07210040' and GroupMauSP='Brown #53'"));
                    //foreach(DataRow dataRow1 in querydinhmuctest)
                    //{
                    //    Console.WriteLine(string.Format("{0}- aaa - {1}", dataRow1["MaSP"], dataRow1["MaVatTu"]));
                    //}

                    //Console.WriteLine("Dòng: "+ querydinhmuctest.Count().ToString());

                    var qrytonkho = dtdinhmuc.AsEnumerable().Where(p => p["SLTon"] != DBNull.Value)
                        .GroupBy(p => p.Field<string>("MaVatTu"))
                        .Select(p => new TonKhoList { MaHang = p.Key, SLTon = p.Min(p => p.Field<double>("SLTon")), SLNhap = p.Min(p => p.Field<double>("SLNhap")), SLXuat = p.Min(p => p.Field<double>("SLXuat")), DonGia = p.Min(p => p.Field<double>("DonGia")) }).Select(p => new TonKhoList { MaHang = p.MaHang, SLTon = p.SLTon, SLNhap = p.SLNhap, SLNhapConLai = p.SLNhap, SLTTonKhoConLai = p.SLTon, SLXuat = p.SLXuat, SLXuatConLai = p.SLXuat, DonGia = p.DonGia }).ToList();

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

                    tonkhotitle = string.Format("Tồn kho trước {0}", mindate.ToString("dd/MM/yy"));
                    nhapkhotitle = string.Format("Nhập mua mới từ {0} - {1}", mindate.ToString("dd/MM/yy"), maxdate.ToString("dd/MM/yy"));
                    xuatkhotitle = string.Format("Xuất kho từ {0} - {1}", mindate.ToString("dd/MM/yy"), maxdate.ToString("dd/MM/yy"));
                    double dtmp = 0;
                    //var queryspcheck = dtdinhmuc.Select("MaSP='C07210040'");
                    //foreach(DataRow dr in queryspcheck)
                    //{
                    //    Console.WriteLine(string.Format("{0}-//{1}", dr["GroupMauSP"], dr["MaVatTu"]));
                    //}
                    string mamau="";
                    string masp = "";
                    foreach (DataRow dataRow in sortedDt.Rows)
                    {
                        if (dataRow.Field<double>("SLSP") <= 0)
                            continue;
                      
                       
                        var querydinhmuc = dtdinhmuc.Select(string.Format("MaSP='{0}' and GroupMauSP='{1}'", dataRow["MaSP"], dataRow["MaMau"]));
                       
                        foreach (DataRow dr in querydinhmuc)
                        {
                            DataRow dataRowdinhmuc = dtxuly.NewRow();
                            dataRowdinhmuc["Index"] = dr["Index"];
                            dataRowdinhmuc["MaSP"] = dr["MaSP"];
                           
                            //    Console.WriteLine(string.Format("{0}-{1}", dr["MaSP"], dr["MaVatTu"]));
                            
                            //if (dr["MaVatTu"].ToString()== "PAT0156")
                            //{
                            //    string s = "";
                            //}
                            dataRowdinhmuc["GroupMauSP"] = dr["GroupMauSP"];
                            dataRowdinhmuc["MaHang"] = dr["MaVatTu"];
                          
                            dataRowdinhmuc["SLKeHoach"] = dataRow.Field<double>("SLSP") * dr.Field<double>("SLQuyDoi");
                            dtmp = dataRow.Field<double>("SLSP") * dr.Field<double>("SLQuyDoi");
                            dataRowdinhmuc["SLTonKho"] = 0;
                            dataRowdinhmuc["SLNhap"] = 0;
                            dataRowdinhmuc["SLXuat"] = 0;
                            dataRowdinhmuc["TyLe"] = 0;
                            dataRowdinhmuc["Ngay"] = dataRow["Ngay"];
                            dtmp = dtmp - dr.Field<double>("SLTonDongVi");
                            if (dtmp <= 0)
                            {
                                dtxuly.Rows.Add(dataRowdinhmuc);
                                continue;
                            }
                            //Xử lý tồn kho luôn
                            //Nếu có đóng vỉ thì sẽ giảm số lượng thiếu lại, do có thêm tồn đóng vỉ

                            var querytk = qrytonkho.Where(p => p.MaHang == dr.Field<string>("MaVatTu")).FirstOrDefault();


                            if (querytk != null)
                            {
                                //Xử lý số lượng xuất trước vì số lượng xuất chỉ cần so với kế hoạch thôi
                                if (dtmp <= querytk.SLXuatConLai)
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
                                    dtmp = dtmp - querytk.SLTTonKhoConLai;
                                    //Xử lý tiếp phần nhập kho
                                    // dnhap = dtmp - querytk.SLTTonKhoConLai;
                                    querytk.SLTTonKhoConLai = 0;

                                    if (querytk.SLNhapConLai > 0)
                                    {

                                        if (dtmp <= querytk.SLNhapConLai)
                                        {

                                            dataRowdinhmuc["SLNhap"] = dtmp;
                                            querytk.SLNhapConLai = querytk.SLNhapConLai - dtmp;
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
                    //prs.exportexcelAsync(_jsRuntime, dtxuly,2,1,"");
                    //Total_TonMB
                    var querytotal = dtxuly.AsEnumerable().GroupBy(p => p.Field<int>("Index")).Select(p => new { Index = p.Key, SLKeHoach = p.Sum(p => p.Field<double>("SLKeHoach")), SLTonKho = p.Sum(p => p.Field<double>("SLTonKho")), SLNhap = p.Sum(p => p.Field<double>("SLNhap")), SLXuat = p.Sum(p => p.Field<double>("SLXuat")) }).ToList();
                 //var querycheckmh= dtxuly.Select("MaHang='PAT0156'").AsEnumerable().ToList();
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
                    int index = 0;


                   // prs.exportexcelAsync(_jsRuntime, dtdinhmuc, 2, 1, "");


                    foreach (var it in querytotal)
                    {
                       // Console.WriteLine("Index " + it.Index);
                        foreach (DataRow rowdm in dtdinhmuc.Rows)
                        {
                            if (it.Index.ToString() == rowdm["Index"].ToString())
                            {
                                //Console.WriteLine("Index trùng " + it.Index);
                                rowdm[string.Format("{0}_KeHoach", "Total")] = it.SLKeHoach;
                                rowdm[string.Format("{0}_TonKho", "Total")] = it.SLTonKho;
                                rowdm[string.Format("{0}_SLNhap", "Total")] = it.SLNhap;
                                rowdm[string.Format("{0}_SLXuat", "Total")] = it.SLXuat;
                                
                                rowdm[string.Format("{0}_SLDeNghi", "Total")] = Math.Round(it.SLKeHoach - it.SLTonKho - it.SLNhap, 3) - rowdm.Field<double>("SLTonDongVi");
                                //string s = rowdm.Field<string>("MaVatTu");
                                //Console.WriteLine(s);
                                //if (s == "BLU0188")
                                //{

                                //    Console.WriteLine(it.SLTonKho);
                                //    Console.WriteLine(rowdm.Field<double>("SLTonDongVi"));
                                //}
                                rowdm[string.Format("{0}_TyLeThieu", "Total")] = (it.SLTonKho + it.SLNhap + rowdm.Field<double>("SLTonDongVi")) / it.SLKeHoach;
                                rowdm[string.Format("{0}_TyLe", "Total")] = (it.SLTonKho + it.SLNhap+rowdm.Field<double>("SLTonDongVi")) / it.SLKeHoach;
                                rowdm[string.Format("{0}_DHConNo", "Total")] = 0;
                                break;
                            }
                        }
                    }


                    //Xử lý đơn hàng chưa về
                    //var keHoachDict = new Dictionary<string, List<DataRow>>();
                    double d = 0;
                    int indexlast = dtdinhmuc.Rows.Count - 1;
                    foreach (DataRow rowkh in dtdonhang.Rows)
                    {
                        string maHang = rowkh.Field<string>("MaHang");
                        if (rowkh["DHConNo"] == DBNull.Value)
                            rowkh["DHConNo"] = 0;
                        d = (rowkh.Field<double>("DHConNo"));

                        foreach (DataRow row in dtdinhmuc.Rows)
                        {
                            if (maHang == row.Field<string>("MaVatTu"))
                            {
                                row["TenNCCShow"] = rowkh["TenNCC"];
                                if (d <= 0) break;

                                if (d >= row.Field<double>("Total_SLDeNghi"))
                                {
                                    d = d - row.Field<double>("Total_SLDeNghi");
                                    rowkh["DHConNo"] = d;
                                    row["Total_DHConNo"] = row["Total_SLDeNghi"];
                                    row["Total_SLDeNghi"] = 0;
                                    row[string.Format("{0}_TyLe", "Total")] = 1;

                                }
                                else
                                {

                                    row["Total_SLDeNghi"] = row.Field<double>("Total_SLDeNghi") - d;
                                    row["Total_DHConNo"] = d;
                                    d = 0;
                                    rowkh["DHConNo"] = d;

                                    if (row.Field<double>("Total_KeHoach") > 0)
                                        row[string.Format("{0}_TyLe", "Total")] = (row.Field<double>("Total_KeHoach") - row.Field<double>("Total_SLDeNghi")) / row.Field<double>("Total_KeHoach");




                                }
                            }

                        }
                        if (d > 0)
                        {

                            for (int i = indexlast; i >= 0; i--)
                            {
                                DataRow dataRow = dtdinhmuc.Rows[i];
                                if (dataRow.Field<string>("MaVatTu") == maHang)
                                {
                                    if (dataRow["Total_DHConNo"] == DBNull.Value)
                                        dataRow["Total_DHConNo"] = 0;
                                    dataRow["Total_DHConNo"] = dataRow.Field<double>("Total_DHConNo") + d;
                                    rowkh["DHConNo"] = 0;
                                    break;
                                }
                            }
                        }
                    }

                    //Xử lý cột
                    int n = dtdinhmuc.Rows.Count;
                    //Xử lý vét cạn đối vs những mã hàng còn số tồn và còn nhập
                    foreach (var it in qrytonkho)
                    {
                        if (it.SLTTonKhoConLai + it.SLTTonKhoConLai == 0)
                        {
                            continue;
                        }
                        for (int i = n - 1; i >= 0; i--)
                        {
                            DataRow dataRow = dtdinhmuc.Rows[i];
                            if (dataRow.Field<string>("MaVatTu") == it.MaHang)
                            {

                                dataRow[string.Format("{0}_TonKho", "Total")] = dataRow.Field<double>(string.Format("{0}_TonKho", "Total")) + it.SLTTonKhoConLai;
                                dataRow[string.Format("{0}_SLNhap", "Total")] = dataRow.Field<double>(string.Format("{0}_SLNhap", "Total")) + it.SLNhapConLai;
                                dataRow[string.Format("{0}_SLXuat", "Total")] = dataRow.Field<double>(string.Format("{0}_SLXuat", "Total")) + it.SLXuatConLai;
                                // dataRow[string.Format("{0}_SLDeNghi", "Total")] = dataRow.Field<double>(string.Format("{0}_SLDeNghi", "Total")) + it.SLTTonKhoConLai;
                                it.SLNhapConLai = 0;
                                it.SLTTonKhoConLai = 0;
                                it.SLXuatConLai = 0;
                                break;
                            }
                        }
                    }
                    foreach (var it in lstkehoachthangselect)
                    {
                        DataRow[] dvsource = dtdinhmuc.Select(string.Format("MaSP='{0}' and GroupMauSP='{1}'", it.MaSP, it.MaMau));

                        foreach (DataRow row in dvsource)
                        {
                            row["KeyGroup"] = it.KeyGroup;
                        }
                    }



                    dtsource = dtdinhmuc;
                    dxGrid.Reload();

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
                Console.Error.WriteLine(string.Format("Lỗi ở dòng {0}: {1}", new StackTrace(ex, true).GetFrame(0).GetFileLineNumber(), ex.Message));
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi: " + ex.Message));

            }
            finally
            {
                PanelVisible = false;

                StateHasChanged();

            }
        }
        
        List<Classcolumn> lstcolumns = new List<Classcolumn>();

        private async void exporttoexcel()
        {
            if (dtsource.Rows.Count > 0)
            {
                try
                {
                    PanelVisible = true;
                    StateHasChanged();
                    DataTable dtexport = dtsource.AsEnumerable().CopyToDataTable();
                    bool checkcolumn = false;
                    int n = dtexport.Columns.Count;
                    for (int j = n - 1; j >= 0; j--)
                    {
                        DataColumn cl = dtexport.Columns[j];
                        checkcolumn = false;
                        foreach (var it in lstcolumns)
                        {
                            if (cl.ColumnName == it.columnName)
                            {
                                dtexport.Columns[j].ColumnName = it.columnText;
                                checkcolumn = true;
                                break;
                            }
                        }
                        if (!checkcolumn)
                        {
                            dtexport.Columns.RemoveAt(j);
                        }
                    }
                    await prs.exportexcelAsync(_jsRuntime, dtexport, 2, 1, "");
                    PanelVisible = false;
                    StateHasChanged();
                    dtexport.Dispose();
                }
                catch (Exception ex)
                {
                    PanelVisible = false;
                    StateHasChanged();
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
                }


            }

        }

        private void GopTheoMaHang()
        {
            if (dtsource.Rows.Count > 0)
            {

                DataTable dtgr = new DataTable();
                dtgr.Columns.Add("TenNhom", typeof(string));
                dtgr.Columns.Add("DonGia", typeof(double));
                dtgr.Columns.Add("PhanLoai", typeof(string));
                dtgr.Columns.Add("MaHang", typeof(string));
                dtgr.Columns.Add("MaPDOC", typeof(string));
                dtgr.Columns.Add("TenHang", typeof(string));
                dtgr.Columns.Add("DVT", typeof(string));
                dtgr.Columns.Add(string.Format("{0}_KeHoach", "Total"), typeof(double));
                dtgr.Columns.Add(string.Format("{0}_TonKho", "Total"), typeof(double));
                dtgr.Columns.Add(string.Format("{0}_SLNhap", "Total"), typeof(double));
                dtgr.Columns.Add(string.Format("{0}_TonMB", "Total"), typeof(double));
                dtgr.Columns.Add(string.Format("{0}_DHConNo", "Total"), typeof(double));
                //dtgr.Columns.Add(string.Format("{0}_TyLeThieu", "Total"), typeof(double));
                dtgr.Columns.Add(string.Format("{0}_TyLe", "Total"), typeof(double));
                dtgr.Columns.Add(string.Format("{0}_TyLeThieu", "Total"), typeof(double));

                dtgr.Columns.Add(string.Format("{0}_SLDeNghi", "Total"), typeof(double));
                dtgr.Columns.Add(string.Format("{0}", "TenNCCShow"), typeof(string));

                var dttotal = dtsource.AsEnumerable().GroupBy(p => new { MaHang = p.Field<string>("MaVatTu"),MaPDOC=p.Field<string>("MaPDOC"), TenNhom = p.Field<string>("TenNhom"), TenHang = p.Field<string>("TenHang"), DVT = p.Field<string>("DVT"),TenNCCShow = p.Field<string>("TenNCCShow") })
                   .Select(p => new
                   {
                       MaHang = p.Key.MaHang,
                       TenHang = p.Key.TenHang,
                       DVT = p.Key.DVT,
                       TenNhom = p.Key.TenNhom,
                       TenNCCShow = p.Key.TenNCCShow,
                       MaPDOC = p.Key.MaPDOC
                   ,
                       SLKeHoach = p.Sum(n => ((n["Total_KeHoach"] == DBNull.Value) ? 0 : n.Field<double>("Total_KeHoach")))
                   ,
                       SLTon = p.Sum(n => ((n["Total_TonKho"] == DBNull.Value) ? 0 : (n.Field<double>("Total_TonKho") + n.Field<double>("SLTonDongVi"))))
                   ,
                       SLNhap = p.Sum(n => ((n["Total_SLNhap"] == DBNull.Value) ? 0 : n.Field<double>("Total_SLNhap")))
                   ,
                       TonMB = p.Sum(n => ((n["Total_TonMB"] == DBNull.Value) ? 0 : n.Field<double>("Total_TonMB"))) //p.Sum(n => n.Field<double>("Total_TonMB"))
                   ,
                       DHNo = p.Sum(n => ((n["Total_DHConNo"] == DBNull.Value) ? 0 : n.Field<double>("Total_DHConNo"))) //p.Sum(n => n.Field<double>("Total_DHConNo"))
                   ,
                       SLDeNghi = p.Sum(n => ((n["Total_SLDeNghi"] == DBNull.Value) ? 0 : n.Field<double>("Total_SLDeNghi"))) //p.Sum(n => n.Field<double>("Total_SLDeNghi"))
                   ,
                       DonGia = p.Min(n => ((n["DonGia"] == DBNull.Value) ? 0 : n.Field<double>("DonGia")))
                   });
                // p.Min(n => n.Field<double>("DonGia")) }).ToList();
                foreach (var it in dttotal)
                {
                    DataRow rownew = dtgr.NewRow();
                    rownew["TenNhom"] = it.TenNhom;
                    rownew["DonGia"] = it.DonGia;

                    rownew["MaHang"] = it.MaHang;
                    rownew["MaPDOC"] = it.MaPDOC;
                    rownew["TenHang"] = it.TenHang;
                    rownew["DVT"] = it.DVT;
                    rownew[string.Format("{0}_KeHoach", "Total")] = it.SLKeHoach;
                    rownew[string.Format("{0}_TonKho", "Total")] = it.SLTon;
                    rownew[string.Format("{0}_SLNhap", "Total")] = it.SLNhap;
                    rownew[string.Format("{0}_TonMB", "Total")] = it.TonMB;
                    rownew[string.Format("{0}_DHConNo", "Total")] = it.DHNo;
                    rownew[string.Format("{0}_SLDeNghi", "Total")] = it.SLDeNghi;
                    rownew["TenNCCShow"] = it.TenNCCShow;
                    rownew["Total_TyLe"] = (it.SLKeHoach <= 0) ? 0 : ((it.SLNhap + it.SLTon + it.DHNo + it.TonMB) / it.SLKeHoach);
                    rownew["Total_TyLeThieu"] = (it.SLKeHoach <= 0) ? 0 : ((it.SLNhap + it.SLTon + it.TonMB) / it.SLKeHoach);
                    // rownew["Total_TyLeThieu"] = (it.SLKeHoach <= 0) ? 0 : ((it.SLNhap + it.SLTon + it.TonMB) / it.SLKeHoach);// Console.WriteLine(it.MaHang);
                    dtgr.Rows.Add(rownew);
                }


                dtsourcegr = dtgr;
                //renderFragmentcolumntotal = CreateColumns(lstheader, lstitemnhapxuat);
                dxGridtotal.Reload();
                activeindex = 1;
              
                StateHasChanged();
            }
            else
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không có dữ liệu"));

            }

        }
        private async void NapTuExcel()
        {
            await ImportExcelAsync();
        }
        DataTable dtsaveKeHoach;
        private async Task<bool> checklogicAsync()
        {


            if (dtsource.Rows.Count == 0)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                return false;
            }
            if (dtsaveKeHoach == null)
            {
                CallAPI callAPI = new CallAPI();
                string sql = @"use NVLDB declare @dt Type_NvlKeHoachSP_Ver2
                insert into @dt(STT)
                values(1)
                select * from @dt";
                List<ParameterDefine> lstparadt = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparadt);
                if (json == "")
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi load tablesave"));
                    return false;
                }
                dtsaveKeHoach = JsonConvert.DeserializeObject<DataTable>(json);
            }
            dtsaveKeHoach.Clear();

            try
            {
                int i = 0;
                //var query = lstdata.Where(p => p.isChanged.Equals(true) && p.SLPhaiDat > 0).ToList();
                //if (query.Count == 0)
                //{
                //    toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn ít nhất 1 mã hàng"));
                //    return false;
                //}
                foreach (var it in lstkehoachthangsaveitem)
                {
                    DataRow rownew = dtsaveKeHoach.NewRow();
                    rownew["STT"] = i;
                    rownew["SerialKHThangItem"] = it.Serial;
                    rownew["MaSP"] = it.MaSP;
                    rownew["ArticleNumber"] = DBNull.Value;
                    if (it.MaMau == null)
                        rownew["MaMauKH"] = DBNull.Value;
                    else
                        rownew["MaMauKH"] = it.MaMau;
                    rownew["SoLuongSP"] = it.SLPhaiDat;
                    rownew["KeyGroup"] = it.KeyGroup;
                    rownew["KhuVuc"] = it.LoaiKeHoach;
                    dtsaveKeHoach.Rows.Add(rownew);
                    i++;
                }
                await getdtitemAsync();


            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                //Console.WriteLine("Lỗi: " + ex.Message);
                return false;
            }
            if (dtsaveKeHoach.Rows.Count == 0)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                //Console.WriteLine("Lỗi: " + ex.Message);
                return false;
            }
            return true;
        }
        private void reset()
        {
            dtsource.Clear();

        }
        class KetquaResult
    {
        public int? Serial { get; set; }
        public double? SLCL { get; set; }

        public string? ketqua { get; set; }

        public string? ketquaexception { get; set; }

        }
        private async Task saveAsync()
        {
            if (await checklogicAsync())
            {
                CallAPI callAPI = new CallAPI();

                string sql = "NVLDB.dbo.NvlKeHoachMuaHang_InsertMuaHang";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialDN", keHoachMuaHang_Showcrr.Serial));//Trong procedure đã xử lý
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@Type_NvlKeHoachSP", prs.ConvertDataTableToJson(dtsaveKeHoach), "DataTable"));
                lstpara.Add(new ParameterDefine("@Type_NvlKeHoachMuaHangItem", prs.ConvertDataTableToJson(dtkehoachitem), "DataTable"));
                try
                {
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<KetquaResult>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            //msgBox.Show("Lưu thành công", IconMsg.iconssuccess);
                            toastService.Notify(new(ToastType.Success, $"Lưu thành công."));
                            reset();
                        }
                        else
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: {0},{1}", query[0].ketqua, query[0].ketquaexception));

                        }

                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, "Đã xảy ra lỗi."));
                    }


                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex));
                }
                finally
                {

                }

            }
            return;
        }


        DataTable dtkehoachitem;
        private void initTable()
        {

            //Lưu ý: thứ tự cột trong bảng khởi tạo này phải giống y hệt thứ tự cột của Type_NvlKeHoachMuaHang_DinhMuc và Type_NvlKeHoachMuaHangItemVer3, nếu sai thứ tự cột là lỗi tè le luôn
            dtkehoachitem = new DataTable();
            dtkehoachitem.Columns.Add("STT", typeof(int));
            dtkehoachitem.Columns.Add("ID", typeof(string));
            dtkehoachitem.Columns.Add("Serial", typeof(int));
            dtkehoachitem.Columns.Add("SerialDN", typeof(int));
            dtkehoachitem.Columns.Add("MaHang", typeof(string));
            dtkehoachitem.Columns.Add("SoLuong", typeof(double));
            dtkehoachitem.Columns.Add("SLTheoDoi", typeof(double));
            dtkehoachitem.Columns.Add("DonGia", typeof(double));
            dtkehoachitem.Columns.Add("DVT", typeof(string));
            dtkehoachitem.Columns.Add("VAT", typeof(int));
            dtkehoachitem.Columns.Add("GhiChu", typeof(string));
            dtkehoachitem.Columns.Add("MaSP", typeof(string));
            dtkehoachitem.Columns.Add("SerialLink", typeof(int));
            dtkehoachitem.Columns.Add("SLQuyDoiSP", typeof(double));
            dtkehoachitem.Columns.Add("TableName", typeof(string));
            dtkehoachitem.Columns.Add("NgayEdit", typeof(DateTime));
            dtkehoachitem.Columns.Add("NgayInsert", typeof(DateTime));
            dtkehoachitem.Columns.Add("UserInsert", typeof(string));
            dtkehoachitem.Columns.Add("TenLienKet", typeof(string));

        }
        private async Task ShowSanPhamAsync()
        {
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_KeHoachPopup>(0);


                builder.AddAttribute(4, "view_KeHoachMuaHangSP_Detail", this);
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            await dxPopup.showAsync("Chọn kế hoạch");

            await dxPopup.ShowAsync();
        }
        public async Task closepopupAsync()
        {
            await dxPopup.CloseAsync();
        }

        private async Task getdtitemAsync()
        {
            initTable();

            dtkehoachitem.Clear();

            var query = dtsource.Select("Total_SLDeNghi>0");
            foreach (DataRow it in query)
            {
                DataRow rownew = dtkehoachitem.NewRow();
                rownew["STT"] = it["Index"];
                rownew["Serial"] = 0;
                rownew["SerialDN"] = keHoachMuaHang_Showcrr.Serial;
                rownew["MaHang"] = it["MaVatTu"];
                rownew["SoLuong"] = it["Total_SLDeNghi"];
                rownew["SLTheoDoi"] = it["Total_SLDeNghi"];
                rownew["DonGia"] = 0;
                rownew["DVT"] = "";

                rownew["ID"] = it["KeyGroup"];
                rownew["TableName"] = "NvlKeHoachSP";
                rownew["TenLienKet"] = "";
                rownew["UserInsert"] = ModelAdmin.users.UsersName;
                dtkehoachitem.Rows.Add(rownew);
            }
        }
    }

}
