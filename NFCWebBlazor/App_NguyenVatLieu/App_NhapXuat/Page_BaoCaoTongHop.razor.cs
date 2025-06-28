using BlazorBootstrap;
using DevExpress.Blazor;
using DevExpress.CodeParser;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Pages;
using System.Data;


namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_BaoCaoTongHop
    {
        [Inject] ToastService toastService { get; set; }
        [Inject] IJSRuntime _jsRuntime { get; set; }
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
            lstkhonvl = await Model.ModelData.GetKhoNvl();
            lstlydo = await Model.ModelData.Getlstnvllydo();
            lstnoigiaonhan = await Model.ModelData.Getlstnoigiaonhan();

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
        private async void expandall()
        {
            dxGrid.BeginUpdate();
            dxGrid.AutoExpandAllGroupRows = expand;
            dxGrid.EndUpdate();
            expand = !expand;
            StateHasChanged();
        }
        List<QuyDoiNgay> lstquydoi = new List<QuyDoiNgay>();
        string dieukienshow = "";
    

        RenderFragment renderFragmentcolumn;
        private RenderFragment CreateColumns(List<itemnhapxuat> lstheader, List<itemnhapxuat> lstitem)
        {
            return builder =>
            {
                int sequence = 0;

                // Tạo DxGridBandColumn
                foreach (itemnhapxuat header in lstheader)
                {
                    Console.WriteLine(header.text);
                    builder.OpenComponent<DxGridBandColumn>(sequence++);
                    builder.AddAttribute(sequence++, "Caption", header.text);
                    // Render các cột bên trong BandColumn
                    builder.AddAttribute(sequence++, "Columns", (RenderFragment)(nestedBuilder =>
                    {
                        foreach (var it in lstitem)
                        {
                            nestedBuilder.OpenComponent<DxGridDataColumn>(sequence++);
                            nestedBuilder.AddAttribute(sequence++, "FieldName", header.value + it.value);
                            nestedBuilder.AddAttribute(sequence++, "Caption", it.text);
                            nestedBuilder.AddAttribute(sequence++, "Width",string.Format("{0}px", "85")); //" 110);
                            nestedBuilder.AddAttribute(sequence++, "DisplayFormat", "#,0.##;-#,0.##;''");
                            nestedBuilder.CloseComponent();
                        }
                    }));

                    builder.CloseComponent();
                }

            };
        }
        List<itemnhapxuat> lstheader = new List<itemnhapxuat>();
        List<itemnhapxuat> lsttextgroup= new List<itemnhapxuat>();
        List<itemnhapxuat> lstitem = new List<itemnhapxuat>();
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
       
        public async void searchAsync()
        {
            dieukienshow = "";
            if (dtpbegin == null || dtpend == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Chọn ngày báo cáo"));
                return;
            }
            dieukienshow = string.Format("Từ ngày {0} - {1}", dtpbegin.Value.ToString("dd/MM/yy"), dtpend.Value.ToString("dd/MM/yy"));

            if (string.IsNullOrEmpty(loaibaocao))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Chọn loại gộp"));

                return;
            }
            if (loaibaocao == "Ngay")
            {
                if ((dtpbegin.Value - dtpend.Value).TotalDays > 31)
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Xin lỗi, thời điểm báo cáo dài hơn 1 tháng, nên kiểu gộp không cho phép GỘP THEO NGÀY. Vui lòng chọn lại KIỂU GỘP", IconMsg.iconwarning));

                    return;
                }
            }
            if (string.IsNullOrEmpty(tenbaocao))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Chọn tên báo cáo", IconMsg.iconinfomation));

                return;
            }

            string KieuChuyen = "", sqlreplace = "";
            if (loaibaocao == "Ngay")
            {
                sqlreplace = "Ngay";

            }
            if (loaibaocao == "Tuan")
            {
                sqlreplace = "dbo.GetFisrtDayOfWeek(Ngay)";
                KieuChuyen = "W";
            }
            if (loaibaocao == "Thang")
            {
                sqlreplace = "dbo.GetFisrtDayOfMonth(Ngay)";
                KieuChuyen = "M";
            }
            if (loaibaocao == "Quy")
            {
                sqlreplace = "dbo.GetFisrtDayOfQuarter(Ngay)";
                KieuChuyen = "Q";
            }
            if (loaibaocao == "Nam")
            {
                sqlreplace = "dbo.GetFisrtDayOfYear(Ngay)";
                KieuChuyen = "Y";
            }

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = " ";

            string dieukienmahang = " ";

            dtsource.Clear();
            lsttextgroup.Clear();
            renderFragmentcolumn = null;

            dieukien = " where Ngay>=@DateBegin and Ngay<=@DateEnd";
            lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
            lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            {
                dieukien += " and MaKho=@MaKho";
                lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
                dieukienshow += Environment.NewLine + string.Format("Kho: {0}", nvlNhapXuatItemShowcrr.MaKho);
            }
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.LyDo))
            {
                dieukien += "  and LyDo = @LyDo";
                lstpara.Add(new ParameterDefine("@LyDo", nvlNhapXuatItemShowcrr.LyDo));
                dieukienshow += Environment.NewLine + string.Format("Kho: {0}", nvlNhapXuatItemShowcrr.LyDo.ToString());
            }
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.NhaMay))
            {
                dieukien += "  and NhaMay = @NhaMay";
                lstpara.Add(new ParameterDefine("@NhaMay", nvlNhapXuatItemShowcrr.NhaMay));
                dieukienshow += Environment.NewLine + string.Format("Nhà máy: {0}", nvlNhapXuatItemShowcrr.NhaMay);
            }
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.TenGN))
            {
                dieukien += "  and MaGN = @MaGN";
                lstpara.Add(new ParameterDefine("@MaGN", nvlNhapXuatItemShowcrr.TenGN));
                //dieukienshow += Environment.NewLine + string.Format("Nơi giao nhận: {0}", txtMaKho.Text);
            }

            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
            {
                dieukienmahang += " where hh.MaHang = @MaHang";
                lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItemShowcrr.MaHang));
                dieukienshow += Environment.NewLine + string.Format("Mã hàng: {0}", nvlNhapXuatItemShowcrr.MaHang);
            }


            string sqlSearch = string.Format(@"use NVLDB select hh.MaHang,hh.TenHang,hh.DVT,hh.QuyCach,qrynxkho.SLNhap,qrynxkho.SLXuat,ngn.MaGN as NameGroup,ngn.TenGN as TextGroup,qrynxkho.Ngay from 
                                (
                                SELECT[MaHang], sum([SLNhap]) as SLNhap, sum([SLXuat]) as SLXuat,{1} as Ngay,qryct.MaGN
                                   FROM[NvlNhapXuatItem] it
                                  inner join(select * from NvlNhapXuat {0})
                                  as qryct on it.SerialCT = qryct.Serial

                                  group by MaHang,{1},qryct.MaGN) as qrynxkho
                                  inner join dbo.NvlHangHoa hh on qrynxkho.MaHang = hh.MaHang
                                  inner join dbo.View_NoiGN ngn on qrynxkho.MaGN = ngn.MaGN {2}", dieukien, sqlreplace, dieukienmahang);
            if (tenbaocao == "Tổng hợp theo giao nhận")
            {
                sqlSearch = string.Format(@"Use NVLDB select hh.MaHang,hh.TenHang,hh.DVT,hh.QuyCach,qrynxkho.SLNhap,qrynxkho.SLXuat,ngn.MaGN as NameGroup,ngn.TenGN as TextGroup,qrynxkho.Ngay,nh.TenNhom from 
                                (
                                SELECT[MaHang], sum([SLNhap]) as SLNhap, sum([SLXuat]) as SLXuat,{1} as Ngay,qryct.MaGN
                                   FROM[NvlNhapXuatItem] it
                                  inner join(select * from NvlNhapXuat {0})
                                  as qryct on it.SerialCT = qryct.Serial

                                  group by MaHang,{1},qryct.MaGN) as qrynxkho
                                  inner join dbo.NvlHangHoa hh on qrynxkho.MaHang = hh.MaHang 
                                  inner join dbo.View_NoiGN ngn on qrynxkho.MaGN = ngn.MaGN 
                                    inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom {2} order by hh.MaHang", dieukien, sqlreplace, dieukienmahang);
            }
            if (tenbaocao == "Tổng hợp theo lý do")
            {
                sqlSearch = string.Format(@"Use NVLDB select hh.MaHang,hh.DVT,hh.TenHang,qrynxkho.SLNhap,hh.QuyCach,qrynxkho.SLXuat,LyDo as NameGroup,LyDo  as TextGroup,qrynxkho.Ngay,nh.TenNhom from 
                                            (
                                            SELECT [MaHang],sum([SLNhap]) as SLNhap,sum([SLXuat]) as SLXuat,Ngay,qryct.LyDo
                                              FROM [NvlNhapXuatItem] it
                                              inner join (select * from NvlNhapXuat {0})
                                              as qryct on it.SerialCT=qryct.Serial
                                              group by MaHang,Ngay,qryct.LyDo) as qrynxkho 
                                              inner join dbo.NvlHangHoa hh on qrynxkho.MaHang=hh.MaHang 
                                                inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                                            {2} order by hh.MaHang", dieukien, sqlreplace, dieukienmahang);
            }
            List<ItemReport> lst = new List<ItemReport>();
            CallAPI callAPI = new CallAPI();

            try
            {
                DataTable dtresult = new DataTable();
                PanelVisible = true;
                lstheader.Clear();
                lstitem.Clear();
                string json = await callAPI.ExcuteQueryEncryptAsync(sqlSearch, lstpara);
                if (json != "")
                {
                    lst = JsonConvert.DeserializeObject<List<ItemReport>>(json);
                   
                    lstquydoi.Clear();


                    if (lst.Count == 0)
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, "Không có dữ liệu, vui lòng kiểm tra lại thông tin tìm kiếm"));
                        return;
                    }
                    var querycolumn = lst.GroupBy(p => p.Ngay).OrderBy(p => p.Key).Select(p => new { Ngay = p.Key.ToString("dd-MM-yyyy"), NgayDT = p.Key }).ToList();
                    List<DateTime> lstdate = new List<DateTime>();


                    //var querygroupngay = lst.GroupBy(p => p.GroupName).Select(p => new { Ngaydatetime = convertdatetime(p.Key.ToString()) }).ToList();
                    foreach (var it in querycolumn)
                    {
                        lstdate.Add(it.NgayDT);
                    }
                    var querychecksumnhapxuat = lst.GroupBy(p => 1).Select(p => new { SLNhap = p.Sum(n => n.SLNhap), SLXuat = p.Sum(n => n.SLXuat) }).ToList();
                  
                    if (querychecksumnhapxuat[0].SLNhap > 0)
                        lstitem.Add(new itemnhapxuat("SLNhap", "Nhận"));
                    if (querychecksumnhapxuat[0].SLXuat > 0)
                        lstitem.Add(new itemnhapxuat("SLXuat", "Giao"));
                    var queryrow = lst.GroupBy(p => new { MaHang = p.MaHang, TextGroup = p.TextGroup, NameGroup = p.NameGroup })
                        .Select(p => new ItemReport { MaHang = p.Key.MaHang, TextGroup = p.Key.TextGroup, NameGroup = p.Key.NameGroup, SLNhap = p.Sum(n => n.SLNhap), SLXuat = p.Sum(n => n.SLXuat) }).OrderBy(p => p.MaHang).ToList();
                    lsttextgroup = queryrow.GroupBy(p => new { NameGroup = p.NameGroup, TextGroup = p.TextGroup }).Select(p => new itemnhapxuat { value = p.Key.NameGroup, text = p.Key.TextGroup }).ToList();
                    dtresult.Columns.Add("MaHang", typeof(string));
                    dtresult.Columns.Add("TenHang", typeof(string));
                    dtresult.Columns.Add("DVT", typeof(string));
                    dtresult.Columns.Add("TenNhom", typeof(string));
                    dtresult.Columns.Add("NameGroup", typeof(string));
                    dtresult.Columns.Add("TextGroup", typeof(string));
                    dtresult.Columns.Add("TongSLNhap", typeof(double));
                    dtresult.Columns.Add("TongSLXuat", typeof(double));
                    
                    foreach (var it in queryrow)
                    {
                        DataRow dataRow = dtresult.NewRow();
                        dataRow["MaHang"] = it.MaHang;
                        // dataRow["TenHang"] = it.TenHang;
                        //dataRow["DVT"] = it.DVT;
                        //dataRow["TenNhom"] = it.TenNhom;
                        dataRow["NameGroup"] = it.NameGroup;
                        dataRow["TextGroup"] = it.TextGroup;
                        dataRow["TongSLNhap"] = it.SLNhap;
                        dataRow["TongSLXuat"] = it.SLXuat;
                        dtresult.Rows.Add(dataRow);
                    }
                   

                    foreach (var it in querycolumn)
                    {
                        lstheader.Add(new itemnhapxuat(it.Ngay, it.Ngay));
                        foreach (var item in lstitem)
                        {
                            dtresult.Columns.Add(it.Ngay + item.value, typeof(double));

                        }
                    }
                    //Xử lý hiển thị text ở Band
                    if (KieuChuyen != "Ngay")
                    {
                        lstquydoi = prs.getDayWeekMonthYear(lstdate, KieuChuyen);
                        foreach (var it in lstquydoi)
                        {
                            foreach (itemnhapxuat band in lstheader)
                            {
                                if (band.value == it.Ngay.ToString("dd-MM-yyyy"))
                                {
                                    band.text = it.Ngayoutput;
                                    break;
                                }
                            }
                        }

                    }
                    else
                    {
                        foreach (var it in lstdate)
                        {
                            QuyDoiNgay quyDoiNgay = new QuyDoiNgay(it, it.ToString("dd-MM-yy"));
                            lstquydoi.Add(quyDoiNgay);
                        }
                    }
                    renderFragmentcolumn = CreateColumns(lstheader, lstitem);
                    bool check = false;
                    int indexbegin = 0;
                    int k = dtresult.Rows.Count;


                    foreach (var it in lst)//Tìm kiếm 2 mảng đã được sắp thứ tự theo mã hàng
                    {
                        check = false;
                        for (int i = indexbegin; i < k; i++)
                        {
                            if (it.MaHang == dtresult.Rows[i].Field<string>("MaHang"))
                            {
                                if (!check)
                                {
                                    indexbegin = i;//Đánh dấu phần tử đầu tiên
                                    check = true;
                                }
                                if (it.TextGroup == dtresult.Rows[i].Field<string>("TextGroup"))
                                {
                                    dtresult.Rows[i]["TenHang"] = it.TenHang;
                                    dtresult.Rows[i]["DVT"] = it.DVT;
                                    dtresult.Rows[i]["TenNhom"] = it.TenNhom;
                                    foreach (var item in lstitem)
                                    {
                                        if (item.value == "SLNhap")
                                            dtresult.Rows[i][it.Ngay.ToString("dd-MM-yyyy") + item.value] = (dtresult.Rows[i][it.Ngay.ToString("dd-MM-yyyy") + item.value] == DBNull.Value) ? it.SLNhap : (dtresult.Rows[i].Field<double>(it.Ngay.ToString("dd-MM-yyyy") + item.value) + it.SLNhap);
                                        if (item.value == "SLXuat")
                                            dtresult.Rows[i][it.Ngay.ToString("dd-MM-yyyy") + item.value] = (dtresult.Rows[i][it.Ngay.ToString("dd-MM-yyyy") + item.value] == DBNull.Value) ? it.SLXuat : (dtresult.Rows[i].Field<double>(it.Ngay.ToString("dd-MM-yyyy") + item.value) + it.SLXuat);
                                    }
                                    break;

                                }
                            }
                            else
                            {
                                if (check)//Đã tìm thấy ở phía trên rồi, do mảng sắp thứ tự, mà xuống đây lại không thấy thì ngắt luôn
                                {
                                    break;
                                }
                            }
                        }
                    }
                    dtsource = dtresult;
                    querycolumn.Clear();
                    lst.Clear();
                    querychecksumnhapxuat.Clear();
                  
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                toastService.Notify(new ToastMessage(ToastType.Danger,"Lỗi :"+ex.Message));
            }
            finally
            {
                PanelVisible = false;
                dxGrid.Reload();
                
                StateHasChanged();
                await _jsRuntime.InvokeVoidAsync("hideCollapse", string.Format("#{0}", randomdivhide));
                //CallJson callJson = new CallJson(_jsRuntime);

                //await callJson.CollapseDiv("divshowhide");
            }
        }
    }
}
