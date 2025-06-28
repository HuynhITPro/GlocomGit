using BlazorBootstrap;
using DevExpress.Blazor;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using NFCWebBlazor.Pages;
using System.Data;




namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_NhapXuat_Master
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        [Inject] SignalRConnect signalRConnect { get; set; }
        bool Ismobile { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {

            try
            {
                await loadAsync();
                CheckQuyen = await phanQuyenAccess.CreateNhapXuatKho(Model.ModelAdmin.users);
                var dimension = await browserService.GetDimensions();
                // var heighrow = await browserService.GetHeighWithID("divcontainer");
                int height = dimension.Height - 70;
                int width = dimension.Width;
                if (width < 768)
                {
                    Ismobile = true;
                    idgrid = "customGridnotheader";
                }
                else
                {
                    Ismobile = false;
                    idgrid = "griddetailnhapkhoms";
                }
                lstuser = await Model.ModelData.Getlstusers();
                //if (heighrow!=null)
                //{
                //    height = dimension.Height - heighrow;
                //}
                Console.WriteLine("Init master Nhap Xuat");
                heightgrid = string.Format("{0}px", height);
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi browserService :" + ex.Message));
            }

        }
        bool CheckQuyen = false;
        bool firstload = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _ = NFCWebBlazor.Model.ModelData.GetHangHoa();
            }
            if (Ismobile)
            {
                if (!firstload)//Load lần đầu để gán biến
                {
                    if (lstuser != null)
                    {
                        userselect = lstuser.FirstOrDefault(p => p.UsersName.Equals(ModelAdmin.users.UsersName));
                        firstload = true;
                        StateHasChanged();
                    }

                }
            }
            //await JS.InvokeVoidAsync("scrollToBottomLast");
            //base.OnAfterRender(firstRender);
        }
        int columnsbegin = 0;
        string? loaiNhapXuat { get; set; }
        List<NvlViTri> lstvitri { get; set; }
        bool visiblenhapmuahang { get; set; } = false;
        private async Task loadAsync()
        {
            try
            {
                lstkhonvl = await Model.ModelData.GetKhoNvl();
                List<DataDropDownList> lst = await Model.ModelData.Getlstnvllydo();
                lstvitri = await ModelData.GetListViTri();
                var queryngn = await Model.ModelData.Getlstnoigiaonhan();
                lstnoigiaonhan = queryngn;

                if (loaiNhapXuat == null)
                {
                    if (ModelAdmin.lstmenuitems != null)
                    {
                        MenuItem? query = ModelAdmin.lstmenuitems.FirstOrDefault(p => p.NameItem.Equals(ModelAdmin.pageclickcurrent));
                        if (query != null)
                        {
                            if (query.Tag != null)
                            {
                                loaiNhapXuat = query.Tag.ToString();
                                lstlydo = lst.Where(p => p.TypeName == loaiNhapXuat).ToList();
                            }
                            Console.WriteLine(loaiNhapXuat);
                            if (loaiNhapXuat != null)
                            {
                                if (loaiNhapXuat == "NhapKho")
                                {
                                    tengiaonhanheader = "Nhà cung cấp/nơi giao";
                                    visiblenhapmuahang = true;
                                }
                                if (loaiNhapXuat == "NhapKhoAll")
                                {
                                    VisibleThemChungTu = false;
                                    tengiaonhanheader = "Nhà cung cấp/nơi giao";
                                }
                                if (loaiNhapXuat == "XuatKho")
                                {
                                    tengiaonhanheader = "Nơi nhận hàng";
                                }
                                if (loaiNhapXuat == "XuatKhoAll")
                                {
                                    VisibleThemChungTu = false;
                                    tengiaonhanheader = "Nơi nhận hàng";
                                }
                                if (loaiNhapXuat == "XuatGhiNo")
                                {
                                    tengiaonhanheader = "Nơi nhận hàng";

                                }
                                if (loaiNhapXuat == "XuatHuyTra")
                                {
                                    tengiaonhanheader = "Nơi nhận hàng";

                                }
                                if (loaiNhapXuat == "NhapGhiNo")
                                {
                                    VisibleHangNo = true;
                                    tengiaonhanheader = "Nhà cung cấp/nơi giao";

                                }
                                if (loaiNhapXuat == "NhapHuyTra")
                                {
                                    tengiaonhanheader = "Nhà cung cấp/nơi giao";
                                    // VisibleHangNo = true;
                                    //grvViewDetail.Columns["SLXuat"].Visible = false;
                                    //grvViewDetail.Columns["SerialKHDH"].Header = "Mã đơn hàng";
                                    //loaiNhapXuat = "XuatHuyTra";
                                    //btXuatKho.Visibility = Visibility.Collapsed;
                                }
                                if (loaiNhapXuat == "NhapKiemKe")
                                {
                                    IsKiemKem = true;
                                    tengiaonhanheader = "Nơi giao nhận";
                                }
                                if (loaiNhapXuat == "ChuyenKho")
                                {
                                    tengiaonhanheader = "Tên kho chuyển/nhận";
                                    await Model.ModelData.GetSanPham();
                                }
                                if (loaiNhapXuat == "NhapGiaCong")
                                {
                                    await Model.ModelData.GetSanPham();
                                }

                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
            }
            //baocaoselected = lstloaibaocao.FirstOrDefault(p => p.Name.Equals("Tồn kho theo sản phẩm"));


        }

        public class NvlNhapXuatKhoShow
        {
            public int Serial { get; set; }
            public int? STTCT { get; set; }
            public string MaCT { get; set; }
            public string MaKho { get; set; }
            public string TenKho { get; set; }
            public string MaGN { get; set; }
            public string TenGN { get; set; }
            public string ChatLuong { get; set; }
            public string LyDo { get; set; }
            public string PONumber { get; set; }
            public string GhiChu { get; set; }
            public string DienGiai { get; set; }
            public DateTime? Ngay { get; set; }
            public int? Xacnhan { get; set; }
            public string NguoiDN { get; set; }
            public string MaDN { get; set; }
            public string TenUser { get; set; }
            public string NhaMay { get; set; }
            public string UserInsert { get; set; }
            public string NguoiXacNhan { get; set; }
            public string ThanhToan { get; set; }
            public decimal? ThanhTien { get; set; }
            public bool? CheckThanhToan { get; set; }
            public DateTime NgayInsert { get; set; }
            public List<NvlNhapXuatItemShow> lstNhapXuatItemShows;
            public List<FileHoSoGroup>? lstfilehoso { get; set; }
            public void setlstfilehoso(List<FileHoSoGroup> lst)
            {
                lstfilehoso = lst;
            }
            public Nullable<int> flag { get; set; }
            public string PathFileHoaDon { get; set; }
            private Nullable<int> _CheckHoaDon { get; set; }
            public bool VisibleHoadon { get; set; } = false;
            public Nullable<int> CheckHoaDon
            {
                get
                {
                    return _CheckHoaDon;
                }
                set
                {
                    _CheckHoaDon = value;
                    if (_CheckHoaDon != null)
                    {
                        PathFileHoaDon = IconImg.pdf;
                        VisibleHoadon = true;
                    }
                }
            }
            public string PathImgTinhTrang { get; set; }
            private string _tinhTrang { get; set; }
            public string TinhTrang
            {
                get { return _tinhTrang; }
                set
                {
                    _tinhTrang = value;
                    if (_tinhTrang != null)
                    {
                        switch (_tinhTrang)
                        {
                            case "Chưa kiểm":
                                PathImgTinhTrang = IconImg.Calendar;
                                break;
                            case "Đang kiểm":
                                PathImgTinhTrang = IconImg.Hourglass;
                                break;
                            case "Không đạt":
                                PathImgTinhTrang = IconImg.NotCheck;
                                break;
                            case "Đạt":
                                PathImgTinhTrang = IconImg.CheckMark;
                                break;
                            case "Không kiểm":
                                PathImgTinhTrang = IconImg.Disconnect;
                                break;
                        }
                    }
                }
            }
            public int SerialTTItem { get; set; }
            public string MaCTTT { get; set; }
            public NvlNhapXuatKhoShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlNhapXuatKhoShow>(json);
            }

        }
        public class NvlNhapXuatItemShow
        {
            public int Serial { get; set; }
            public int? SerialCT { get; set; }
            public int? SerialLink { get; set; }
            public bool chk { get; set; } = false;
            public int? SerialKHDH { get; set; }
            public Nullable<int> SerialDN { get; set; }
            public string NguoiDN { get; set; }
            public string MaCT { get; set; }
            public string TenLienKet { get; set; }
            public string SelectedKHItem { get; set; }
            public string TableName { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string TenNhom { get; set; }
            public string DVT { get; set; }
            public Nullable<DateTime> NgaySanXuat { get; set; }
            public decimal? SoLuong { get; set; }
            public decimal? SLNhap { get; set; } = 0;//Nên khởi tạo sẵn và 2 biến này không cho phép null
            public decimal? SLXuat { get; set; } = 0;//Nên khởi tạo sẵn và 2 biến này không cho phép null
            public decimal? SLTon { get; set; } = 0;
            public decimal? SLNo { get; set; } = 0;
            public Nullable<decimal> DonGiaDN { get; set; }
            public decimal? DonGia { get; set; }
            public decimal? SLNhapTT { get; set; }

            public decimal? ThanhTien
            {
                get { return DonGia * (SLNhap + SLXuat); }
            }
            public decimal? SLXuatTT { get; set; }
            public string DVTTT { get; set; }
            public double? TyLeQuyDoi { get; set; }
            public string KhachHang_XuatXu { get; set; }
            public Nullable<DateTime> NgayHetHan { get; set; }
            public string MaKien { get; set; }
            public string SoLo { get; set; }

            public string SoXe { get; set; }
            public string GhiChu { get; set; }
            public string Barcode { get; set; }
            public string MaSP { get; set; }
            public string ArticleNumber { get; set; }
            public string UserInsert { get; set; }
            public DateTime NgayInsert { get; set; }
            public DateTime Ngay { get; set; }
            public string LyDo { get; set; }
            public string TenSP { get; set; }
            public string MaGN { get; set; }
            public string TenGN { get; set; }
            public string ChatLuong { get; set; }
            public decimal? SLConLai { get; set; }
            public string NhaMay { get; set; }
            public string MaKho { get; set; }
            public string TenKho { get; set; }
            public Nullable<int> flag { get; set; }
            public string MaDatHang { get; set; }
            private string _tinhTrang { get; set; }
            public string PathImgTinhTrang { get; set; }
            public string GhiChuDeNghi { get; set; }
            public string TinhTrang
            {
                get { return _tinhTrang; }
                set
                {
                    _tinhTrang = value;
                    if (_tinhTrang != null)
                    {
                        switch (_tinhTrang)
                        {
                            case "Chưa kiểm":
                                PathImgTinhTrang = IconImg.Calendar;
                                break;
                            case "Đang kiểm":
                                PathImgTinhTrang = IconImg.Hourglass;
                                break;
                            case "Không đạt":
                                PathImgTinhTrang = IconImg.NotCheck;
                                break;
                            case "Đạt":
                                PathImgTinhTrang = IconImg.CheckMark;
                                break;
                            case "Không kiểm":
                                PathImgTinhTrang = IconImg.Disconnect;
                                break;
                        }
                    }
                }
            }
            public string DauTuan { get; set; }
            public string ViTri { get; set; }
            public string NguoiThanhToan { get; set; }
            public DateTime? NgayThanhToan { get; set; }
            public string TinhTrangSuDung { get; set; }
            public NvlNhapXuatItemShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlNhapXuatItemShow>(json);
            }
        }

        public static RenderFragment BuildColumns(List<InitDxGridDataColumn> _lstcolumn)
        {
            RenderFragment columns = b =>
            {
                int counter = 0;
                foreach (InitDxGridDataColumn col in _lstcolumn.OrderBy(p => p.Index))
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
                        b.AddAttribute(0, "Width", string.Format("{0}px", col.Width));
                    else
                    if (col.Width != null)
                        b.AddAttribute(0, "MinWidth", string.Format("90px"));
                    if (col.GroupIndex != null)
                    {
                        b.AddAttribute(0, "GroupIndex", col.GroupIndex.Value);
                    }
                    b.CloseComponent();

                    counter++;
                }



            };
            return columns;
        }
        private async void search()
        {
            try
            {
                await searchasAsync();

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));

            }

        }



        string ghichu = "";
        App_ClassDefine.ClassProcess prs = new ClassProcess();

        string chucnang = "";
        List<NvlNhapXuatKhoShow> lstNhapXuatKhoSearchShow = new List<NvlNhapXuatKhoShow>();
        private async Task searchasAsync()
        {
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstNhapXuatKhoSearchShow.Clear();
            lstcolumn.Clear();
            string sql = "";

            CallAPI callAPI = new CallAPI();
            if (dtpbegin == null || dtpend == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Chọn ngày "));
                return;
            }
            TimeSpan difference = dtpend.Value - dtpbegin.Value;

            // Lấy số ngày
            int days = difference.Days;
            if (days == 0)
            {
                showallgroup = true;
            }
            else
                showallgroup = false;

            string dieukien = "";
            string dieukienmahang = "";
            if (sochungtu == null)
            {


                dieukien += " Where Ngay>=@DateBegin and Ngay<=@DateEnd";

                if (khonvlselected != null)
                {

                    dieukien += " and MaKho = @MaKho";
                    lstpara.Add(new ParameterDefine("@MaKho", khonvlselected.Name.ToString()));
                }
                if (nhamayselected != null)
                {

                    dieukien += " and NhaMay = @NhaMay";
                    lstpara.Add(new ParameterDefine("@NhaMay", nhamayselected.Name.ToString()));
                }
                if (lydoselected != null)
                {
                    dieukien += " and LyDo = @LyDo";
                    lstpara.Add(new ParameterDefine("@LyDo", lydoselected.Name));
                }
                if (userselect != null)
                {
                    dieukien += " and UserInsert = @UserInsert";
                    lstpara.Add(new ParameterDefine("@UserInsert", userselect.UsersName));
                }
                if (!string.IsNullOrEmpty(noigiaonhan))
                {
                    dieukien += " and MaGN = @MaGN";
                    lstpara.Add(new ParameterDefine("@MaGN", noigiaonhan));
                }

                if (!string.IsNullOrEmpty(MaHangSearch))
                {
                    dieukienmahang = @" and Serial in (SELECT  Distinct([SerialCT])
                                  FROM [NvlNhapXuatItem]
                                  where MaHang=@MaHang)";
                    lstpara.Add(new ParameterDefine("@MaHang", MaHangSearch));
                }


                switch (loaiNhapXuat)
                {
                    case "NhapKho":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'NhapKho')";
                        break;
                    case "NhapKhoAll":
                        dieukien += " and STTCT >=0";
                        break;
                    case "XuatKho":
                        dieukien += " and STTCT <0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'XuatKho')";
                        break;
                    case "XuatKhoAll":
                        dieukien += " and STTCT <0";
                        break;
                    case "XuatGhiNo":
                        dieukien += " and STTCT <0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'LyDoNo')";
                        break;
                    case "NhapGhiNo":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'LyDoNo')";
                        break;
                    case "XuatHuyTra":
                        dieukien += " and STTCT <0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'XuatHuyTra')";
                        break;
                    case "NhapHuyTra":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'XuatHuyTra')";
                        break;
                    case "NhapKiemKe":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'NhapKiemKe')";
                        break;
                    case "ChuyenKho":
                        dieukien += "  and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'ChuyenKho')";
                        break;
                    case "NhapGiaCong":
                        dieukien += "  and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'NhapGiaCong')";
                        break;
                    default:
                        dieukien += " and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'LyDoNo')";
                        break;
                }

                lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
                lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
            }
            else
            {
                dieukien += " Where Serial = @Serial";
                switch (loaiNhapXuat)
                {
                    case "NhapKho":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'NhapKho')";
                        break;
                    case "NhapKhoAll":
                        dieukien += " and STTCT >=0";
                        break;
                    case "XuatKho":
                        dieukien += " and STTCT <0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'XuatKho')";
                        break;
                    case "XuatKhoAll":
                        dieukien += " and STTCT <0";
                        break;
                    case "XuatGhiNo":
                        dieukien += " and STTCT <0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'LyDoNo')";
                        break;
                    case "NhapGhiNo":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'LyDoNo')";
                        break;
                    case "XuatHuyTra":
                        dieukien += " and STTCT <0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'XuatHuyTra')";
                        break;
                    case "NhapHuyTra":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'XuatHuyTra')";
                        break;
                    case "NhapKiemKe":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'NhapKiemKe')";
                        break;
                    case "ChuyenKho":
                        dieukien += "  and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'ChuyenKho')";
                        break;
                    case "NhapGiaCong":
                        dieukien += "  and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'NhapGiaCong')";
                        break;
                    default:
                        dieukien += " and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'LyDoNo')";
                        break;
                }
                lstpara.Add(new ParameterDefine("@Serial", sochungtu));
                lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
            }
            //sql này chưa gắn kết quả nghiệm thu
            //sql = string.Format(@"use NVLDB Select nx.*,ngn.TenGN, mk.TenKho,usr.TenUser,qryhoadon.SerialLink as CheckHoaDon FROM (select * from NvlNhapXuat {0}) nx 
            //Inner join NvlMaKho mk on nx.MaKho = mk.MaKho inner join View_NoiGN ngn on nx.MaGN=ngn.MaGN inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName
            //     left join (SELECT SerialLink

            //  FROM DBMaster.dbo.NvlFile_AttachGroup
            //  where NoiDung=N'Hóa đơn' and NgayInsert>=@DateBegin
            //  and Serial in (SELECT  SerialLink FROM DBMaster.[dbo].[FileHoSo]
            //  where NgayInsert>=@DateBegin and TableName='NvlFile_AttachGroup')
            //  group by SerialLink
            //  ) as qryhoadon on nx.Serial=qryhoadon.SerialLink", dieukien);

            //Sql mới sẽ gắn nghiệm thu vào
            if (!Ismobile)
            {


                sql = string.Format(@" use NVLDB 
                        IF OBJECT_ID('tempdb..#tmp') IS NOT NULL
						DROP TABLE #tmp
                        IF OBJECT_ID('tempdb..#tblnt') IS NOT NULL
						                        DROP TABLE #tblnt

                        --Dùng để xử lý nghiệm thu do ở máy chủ khác, lưu ý không nên truy vấn trực tiếp vào bảng của server khác qua Serverlink Oblink object, vì tốc độ rất chậm nếu data lớn
                        --Tạo bảng #tmp để lưu những dòng cần
	                        CREATE TABLE #tmp([Serial] [int] primary key ,[STTCT] [int],
	                        [MaCT] [nvarchar](50),[MaKho] [nvarchar](100),[MaGN] [nvarchar](100) 
	                        ,[LyDo] [nvarchar](100) ,[PONumber] [nvarchar](100) ,
	                        [GhiChu] [nvarchar](250) ,[DienGiai] [nvarchar](200) ,
	                        [Ngay] [date] ,[Xacnhan] [int] ,[NguoiDN] [nvarchar](100) ,
	                        [MaDN] [nvarchar](100) ,[NhaMay] [nvarchar](100) ,[ChatLuong] [nvarchar](100) 
	                        ,[UserInsert] [nvarchar](100) ,[NgayInsert] [datetime] ,[NghiemThu] [nvarchar](100),[flag] [int])

                             --Dùng để xử lý nghiệm thu do ở máy chủ khác, lưu ý không nên truy vấn trực tiếp vào bảng của server khác qua Serverlink Oblink object, vì tốc độ rất chậm nếu data lớn
                              Insert into #tmp (Serial,[STTCT],[MaCT],[MaKho],[MaGN],[LyDo],[PONumber],[GhiChu],[DienGiai],[Ngay],[Xacnhan]
                                   ,[NguoiDN],[MaDN],[NhaMay]
                                   ,[ChatLuong],[UserInsert],[NgayInsert],[NghiemThu],[flag])
                             select Serial,[STTCT],[MaCT],[MaKho],[MaGN],[LyDo],[PONumber],[GhiChu],[DienGiai],[Ngay],[Xacnhan]
                                   ,[NguoiDN],[MaDN],[NhaMay]
                                   ,[ChatLuong],[UserInsert],[NgayInsert],[NghiemThu],[flag]
		                           from NvlNhapXuat  {0}{1}
                        
                            --Vì bảng chất lượng thì định nghĩa chứng từ không kiểm thì các mã hàng bên trong phải không kiểm, nên mới phát sinh ra bảng này, nếu sau này thay đổi cách làm thì bỏ dòng truy vấn bên dưới
	                        declare  @tblnxitem as Table(SerialCT int,MaHang nvarchar(100),ThanhTien decimal(18,6)) 
	                        insert into @tblnxitem(SerialCT,MaHang,ThanhTien)
	                        select SerialCT,MaHang,sum((SLNhap+SLXuat)*DonGia) as ThanhTien from NvlNhapXuatItem nxitem where SerialCT in (select Serial from #tmp)
	                        group by SerialCT,MaHang

	                        -- danh sách những mã hàng không cần kiểm
	                        declare @tblmhknt as Table(MaHang nvarchar(100) primary key)
	                        insert into @tblmhknt(MaHang)
	                        select MaHang from SP.NguyenVatLieu.dbo.View_MaHangKhongNghiemThu
	
	                         -- chỉ cần 1 mã hàng nào đó trong chứng từ không có trong bảng @tblmhknt này, thì chứng từ đó phải kiểm
	                         declare @tblphaikiem Table(Serial int primary key)
	                         insert into @tblphaikiem(Serial)
	                         SELECT  SerialCT
	                        FROM @tblnxitem dt
	                        WHERE NOT EXISTS (
                            SELECT MaHang
                            FROM @tblmhknt tbl
                            WHERE dt.MaHang = tbl.MaHang)
	                        group by SerialCT

	                        --Lấy danh sách những chứng từ không cần kiểm
	                        declare @tblkhongcankiem table(Serial int)
	                        insert into @tblkhongcankiem(Serial)
	                        select DISTINCT dt.Serial from #tmp dt where Serial not in (select Serial from @tblphaikiem)

	                        -- Tạo bảng biến để lưu kết quả nghiệm thu từ function
                          create TABLE #tblnt(Serial int primary key, TinhTrang NVARCHAR(100))
                         declare @lstserial nvarchar(max)
                         --SELECT @lstserial = COALESCE(@lstserial + ';', '') + CAST(Serial AS NVARCHAR)
                        -- FROM @tblphaikiem
                        SELECT @lstserial = STUFF((
                            SELECT ';' + CAST(Serial AS NVARCHAR)
                            FROM @tblphaikiem
                            FOR XML PATH('')
                        ), 1, 1, '')
                        declare @sqlex nvarchar(max)
                        SET @sqlex = 'INSERT INTO #tblnt(Serial,TinhTrang)
					                        SELECT Serial,TinhTrang
					                        FROM OPENQUERY(SP, ''SELECT * FROM NguyenVatLieu.dbo.NvlChungTuNghiemThu(''''' + @lstserial + ''''')'')';
				                        EXEC sp_executesql @sqlex

                        --Đưa dữ liệu nghiệm thu vào bảng #tblnt

                        Select nx.*,ngn.TenGN, mk.TenKho,usr.TenUser,qryhoadon.SerialLink as CheckHoaDon,case when tnlkk.Serial is not null then N'Không kiểm' else
											(case when nt.Serial is null then N'Chưa kiểm' else nt.TinhTrang end) 
											end as TinhTrang
                                        ,isnull(qrynxitem.ThanhTien,0) as ThanhTien
                        FROM #tmp nx 
                                    Inner join NvlMaKho mk on nx.MaKho = mk.MaKho inner join View_NoiGN ngn on nx.MaGN=ngn.MaGN inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName
                                         left join (SELECT SerialLink
                                      FROM DBMaster.dbo.NvlFile_AttachGroup
                                      where NoiDung=N'Hóa đơn' and NgayInsert>=@DateBegin
                                      and Serial in (SELECT  SerialLink FROM DBMaster.[dbo].[FileHoSo]
                                      where NgayInsert>=@DateBegin and TableName='NvlFile_AttachGroup')
                                      group by SerialLink
                                      ) as qryhoadon on nx.Serial=qryhoadon.SerialLink
			                          left join #tblnt nt on nx.Serial=nt.Serial
                                    left join (select SerialCT,sum(ThanhTien) as ThanhTien from @tblnxitem group by SerialCT)
									  as qrynxitem on nx.Serial=qrynxitem.SerialCT
                                left join @tblkhongcankiem tnlkk on nx.Serial=tnlkk.Serial
                        -- Drop bảng tạm sau khi sử dụng xong
                        DROP TABLE #tmp
                        DROP TABLE #tblnt", dieukien, dieukienmahang);
            }
            else//Đối vs điện thoại thì không cần hiển thị nghiệm thu
            {
                sql = string.Format(@" use NVLDB 
                        IF OBJECT_ID('tempdb..#tmp') IS NOT NULL
						DROP TABLE #tmp
                        IF OBJECT_ID('tempdb..#tblnt') IS NOT NULL
						                        DROP TABLE #tblnt

                        --Dùng để xử lý nghiệm thu do ở máy chủ khác, lưu ý không nên truy vấn trực tiếp vào bảng của server khác qua Serverlink Oblink object, vì tốc độ rất chậm nếu data lớn
                        --Tạo bảng #tmp để lưu những dòng cần
	                        CREATE TABLE #tmp([Serial] [int] primary key ,[STTCT] [int],
	                        [MaCT] [nvarchar](50),[MaKho] [nvarchar](100),[MaGN] [nvarchar](100) 
	                        ,[LyDo] [nvarchar](100) ,[PONumber] [nvarchar](100) ,
	                        [GhiChu] [nvarchar](250) ,[DienGiai] [nvarchar](200) ,
	                        [Ngay] [date] ,[Xacnhan] [int] ,[NguoiDN] [nvarchar](100) ,
	                        [MaDN] [nvarchar](100) ,[NhaMay] [nvarchar](100) ,[ChatLuong] [nvarchar](100) 
	                        ,[UserInsert] [nvarchar](100) ,[NgayInsert] [datetime] ,[NghiemThu] [nvarchar](100),[flag] [int])

                             --Dùng để xử lý nghiệm thu do ở máy chủ khác, lưu ý không nên truy vấn trực tiếp vào bảng của server khác qua Serverlink Oblink object, vì tốc độ rất chậm nếu data lớn
                              Insert into #tmp (Serial,[STTCT],[MaCT],[MaKho],[MaGN],[LyDo],[PONumber],[GhiChu],[DienGiai],[Ngay],[Xacnhan]
                                   ,[NguoiDN],[MaDN],[NhaMay]
                                   ,[ChatLuong],[UserInsert],[NgayInsert],[NghiemThu],[flag])
                             select Serial,[STTCT],[MaCT],[MaKho],[MaGN],[LyDo],[PONumber],[GhiChu],[DienGiai],[Ngay],[Xacnhan]
                                   ,[NguoiDN],[MaDN],[NhaMay]
                                   ,[ChatLuong],[UserInsert],[NgayInsert],[NghiemThu],[flag]
		                           from NvlNhapXuat  {0}{1}

                        -- Tạo bảng biến để lưu kết quả nghiệm thu từ function
                        create TABLE #tblnt(Serial int primary key, TinhTrang NVARCHAR(100))
                       
                        --Đưa dữ liệu nghiệm thu vào bảng #tblnt

                        Select nx.*,ngn.TenGN, mk.TenKho,usr.TenUser,qryhoadon.SerialLink as CheckHoaDon,case when nt.Serial is null then N'Chưa kiểm' else nt.TinhTrang end as TinhTrang,isnull(qrynxitem.ThanhTien,0) as ThanhTien
                        FROM #tmp nx 
                                    Inner join NvlMaKho mk on nx.MaKho = mk.MaKho inner join View_NoiGN ngn on nx.MaGN=ngn.MaGN inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName
                                         left join (SELECT SerialLink
                                      FROM DBMaster.dbo.NvlFile_AttachGroup
                                      where NoiDung=N'Hóa đơn' and NgayInsert>=@DateBegin
                                      and Serial in (SELECT  SerialLink FROM DBMaster.[dbo].[FileHoSo]
                                      where NgayInsert>=@DateBegin and TableName='NvlFile_AttachGroup')
                                      group by SerialLink
                                      ) as qryhoadon on nx.Serial=qryhoadon.SerialLink
			                          left join #tblnt nt on nx.Serial=nt.Serial
                                    left join (select SerialCT,sum((SLNhap+SLXuat)*DonGia) as ThanhTien from NvlNhapXuatItem where SerialCT in (select Serial from #tmp) group by SerialCT)
									  as qrynxitem on nx.Serial=qrynxitem.SerialCT

                        -- Drop bảng tạm sau khi sử dụng xong
                        DROP TABLE #tmp
                        DROP TABLE #tblnt", dieukien, dieukienmahang);
            }
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NvlNhapXuatKhoShow>>(json);
                    if (query.Count > 0)
                    {
                        lstNhapXuatKhoSearchShow.AddRange(query);
                        await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
                      // await ModelAdmin.mainLayout.ScrollBottom();
                    }
                    //lstNhapXuatKhoSearchShow=lst;
                }


            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));

            }
            finally
            {
                PanelVisible = false;
                dxGrid.Reload();
                StateHasChanged();
            }

        }

        public async void ShowFlyout(NvlNhapXuatKhoShow keHoachMuaHang_Show)
        {
            try
            {
                if (loaiNhapXuat == "NhapKhoAll" || loaiNhapXuat == "XuatKhoAll")
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Chức năng này bị khóa khi ở form này, vui lòng vào đúng form nhập xuất"));
                    return;
                }


                if (!dxFlyoutchucnang.IsInitialized)
                    await dxFlyoutchucnang.InitializedTask;
                await dxFlyoutchucnang.CloseAsync();
                nvlNhapXuatKhoShowcrr = keHoachMuaHang_Show;
                idflychucnang = "#" + idelement(keHoachMuaHang_Show);

                //IsOpenfly = true;
                await dxFlyoutchucnang.ShowAsync();
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi: " + ex.Message));
                Console.WriteLine("Lỗi:"+ex.Message);
            }

        }
        private async void ShowMasterAdd()
        {
            try
            {
                IsOpenfly = false;
                await dxFlyoutchucnang.CloseAsync();
                NvlNhapXuatKhoShow nVLDonDatHangShow = new NvlNhapXuatKhoShow();
                nVLDonDatHangShow.Serial = 0;
                nVLDonDatHangShow.Ngay = DateTime.Now;
                nVLDonDatHangShow.NhaMay = "Nhà máy A";

                nVLDonDatHangShow.MaKho = ModelAdmin.MaKhoSelected;
                if (loaiNhapXuat == null)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi Loại nhập xuất"));
                    return;
                }

                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_NhapXuatMasterAdd>(0);
                    builder.AddAttribute(1, "nvlNhapXuatKhoShowcrr", nVLDonDatHangShow);
                    builder.AddAttribute(2, "LoaiNhapXuat", loaiNhapXuat);
                    builder.AddAttribute(3, "AfterSave", EventCallback.Factory.Create<NvlNhapXuatKhoShow>(this, GotoMainFormAsync));
                    builder.CloseComponent();
                };

                await dxPopup.showAsync("TẠO CHỨNG TỪ");
                await dxPopup.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
        }
        private async void ShowHangNoAdd()
        {
            try
            {
                IsOpenfly = false;

                await dxFlyoutchucnang.CloseAsync();
                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_NvlNhapXuatGhiNo>(0);
                    // builder.AddAttribute(3, "AfterSave", EventCallback.Factory.Create<NvlNhapXuatKhoShow>(this, GotoMainFormAsync));
                    builder.CloseComponent();
                };

                await dxPopup.showAsync("DANH SÁCH HÀNG NỢ");
                await dxPopup.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
        }
        private async void ShowInTemMasterAdd()
        {
            try
            {

                NvlInTemShow nvlInTemShow = new NvlInTemShow();
                nvlInTemShow.Serial = 0;
                nvlInTemShow.BanIn = 1;

                if (loaiNhapXuat == null)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi Loại nhập xuất"));
                    return;
                }

                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_NvlInTem>(0);
                    builder.AddAttribute(1, "nvlInTemShowcrr", nvlInTemShow);

                    // builder.AddAttribute(3, "AfterSave", EventCallback.Factory.Create<NvlNhapXuatKhoShow>(this, GotoMainFormAsync));
                    builder.CloseComponent();
                };

                await dxPopup.showAsync("TẠO TEM BARCODE");
                await dxPopup.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
        }

        private async void ShowInTemViTri()
        {
            try
            {
                NvlNhapXuatItemShow nvlNhapXuatItemShow = new NvlNhapXuatItemShow();
                nvlNhapXuatItemShow.NhaMay = ModelAdmin.users.NhaMay;
                nvlNhapXuatItemShow.MaKho = ModelAdmin.MaKhoSelected;

                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_ViTriAdd>(0);
                    builder.AddAttribute(1, "nvlNhapXuatItemShowcrr", nvlNhapXuatItemShow);

                    // builder.AddAttribute(3, "AfterSave", EventCallback.Factory.Create<NvlNhapXuatKhoShow>(this, GotoMainFormAsync));
                    builder.CloseComponent();
                };

                await dxPopup.showAsync("THÊM VÀO VỊ TRÍ");
                await dxPopup.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
        }
        private async void ShowBangke()
        {
            try
            {
                NvlNhapXuatItemShow nvlNhapXuatItemShow = new NvlNhapXuatItemShow();
                nvlNhapXuatItemShow.NhaMay = ModelAdmin.users.NhaMay;
                nvlNhapXuatItemShow.MaKho = ModelAdmin.MaKhoSelected;

                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_NvlNhapXuat_BKNhanh>(0);
                    builder.AddAttribute(1, "LoaiNhapXuat", loaiNhapXuat);

                    // builder.AddAttribute(3, "AfterSave", EventCallback.Factory.Create<NvlNhapXuatKhoShow>(this, GotoMainFormAsync));
                    builder.CloseComponent();
                };

                await dxPopup.showAsync("BẢNG KÊ");
                await dxPopup.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
        }
        private async void ShowTRACUU()
        {

            try
            {

                renderFragment = builder =>
                {
                    builder.OpenComponent<Page_NvlTonKhoDauTuan>(0);
                    // builder.AddAttribute(3, "AfterSave", EventCallback.Factory.Create<NvlNhapXuatKhoShow>(this, GotoMainFormAsync));
                    builder.CloseComponent();
                };

                await dxPopup.showAsync("TEM ĐANG TỒN KHO");
                await dxPopup.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
        }
        private async void ShowMasterEdit()
        {
            try
            {


                IsOpenfly = false;
                if (loaiNhapXuat == null)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi Loại nhập xuất"));
                    return;
                }

                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_NhapXuatMasterAdd>(0);
                    builder.AddAttribute(1, "nvlNhapXuatKhoShowcrr", nvlNhapXuatKhoShowcrr.CopyClass());
                    builder.AddAttribute(2, "LoaiNhapXuat", loaiNhapXuat);
                    builder.AddAttribute(3, "AfterEdit", EventCallback.Factory.Create<NvlNhapXuatKhoShow>(this, AfterEditAsync));
                    builder.CloseComponent();
                };

                await dxPopup.showAsync("TẠO CHỨNG TỪ");
                await dxPopup.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
        }

        public async Task GotoMainFormAsync(NvlNhapXuatKhoShow nVLDonDatHangShow)
        {
            try
            {
                await dxPopup.CloseAsync();

                MaHangSearch = null;
                sochungtu = nVLDonDatHangShow.Serial;

                await searchasAsync();
                sochungtu = null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
            // dxGrid.Reload();
        }
        public void setClassafterEdit(NvlNhapXuatKhoShow nvlNhapXuatShow_set, NvlNhapXuatKhoShow nvlNhapXuatShow_get)
        {
            // nvlNhapXuatShow_set.Serial = nvlNhapXuatShow_get.Serial;
            nvlNhapXuatShow_set.MaKho = nvlNhapXuatShow_get.MaKho;
            nvlNhapXuatShow_set.MaGN = nvlNhapXuatShow_get.MaGN;

            nvlNhapXuatShow_set.TenGN = nvlNhapXuatShow_get.TenGN;
            nvlNhapXuatShow_set.TenKho = nvlNhapXuatShow_get.TenKho;
            nvlNhapXuatShow_set.LyDo = nvlNhapXuatShow_get.LyDo;
            nvlNhapXuatShow_set.PONumber = nvlNhapXuatShow_get.PONumber;
            nvlNhapXuatShow_set.ChatLuong = nvlNhapXuatShow_get.ChatLuong;
            nvlNhapXuatShow_set.GhiChu = nvlNhapXuatShow_get.GhiChu;
            nvlNhapXuatShow_set.DienGiai = nvlNhapXuatShow_get.DienGiai;
            nvlNhapXuatShow_set.Ngay = nvlNhapXuatShow_get.Ngay;
            nvlNhapXuatShow_set.Xacnhan = nvlNhapXuatShow_get.Xacnhan;
            nvlNhapXuatShow_set.NguoiDN = nvlNhapXuatShow_get.NguoiDN;
            nvlNhapXuatShow_set.MaDN = nvlNhapXuatShow_get.MaDN;
            nvlNhapXuatShow_set.NhaMay = nvlNhapXuatShow_get.NhaMay;
            nvlNhapXuatShow_set.UserInsert = nvlNhapXuatShow_get.UserInsert;
        }
        public async Task AfterEditAsync(NvlNhapXuatKhoShow nVLDonDatHangShow)
        {
            await dxPopup.CloseAsync();
            try
            {
                setClassafterEdit(nvlNhapXuatKhoShowcrr, nVLDonDatHangShow);
                await dxGrid.SaveChangesAsync();
                // dxGrid.Reload();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
            // dxGrid.Reload();
        }
        private async void viewConnectPrint()
        {

            IsOpenfly = false;
           await dxFlyoutchucnang.CloseAsync();
            if (loaiNhapXuat == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi Loại nhập xuất"));
                return;
            }

            renderFragment = builder =>
            {
                builder.OpenComponent<View_ConnectPrinter>(0);
                //builder.AddAttribute(1, "nvlNhapXuatKhoShowcrr", nvlNhapXuatKhoShowcrr.CopyClass());
                //builder.AddAttribute(2, "LoaiNhapXuat", loaiNhapXuat);
                builder.AddAttribute(3, "GetBarcode", EventCallback.Factory.Create<string>(this, getBarcode));
                builder.CloseComponent();
            };

          await  dxPopup.showAsync("KẾT NỐI MÁY IN");
          await  dxPopup.ShowAsync();

        }
        public async void getBarcode(string barcode)
        {
            await dxPopup.CloseAsync();
            //JsonMsgAndroid jsonMsgAndroid = new JsonMsgAndroid();
            //jsonMsgAndroid.topic = barcode;
            //jsonMsgAndroid.typemsg = TypemsgAPI.joingroup.ToString();
            //await  signalRConnect.SendMsg(jsonMsgAndroid);

        }
        private async void ShowSignalR()
        {
            IsOpenfly = false;
           await dxFlyoutchucnang.CloseAsync();
            if (loaiNhapXuat == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi Loại nhập xuất"));
                return;
            }

            renderFragment = builder =>
            {
                builder.OpenComponent<Page_SignalRTest>(0);
                //builder.AddAttribute(1, "nvlNhapXuatKhoShowcrr", nvlNhapXuatKhoShowcrr.CopyClass());
                //builder.AddAttribute(2, "LoaiNhapXuat", loaiNhapXuat);
                //builder.AddAttribute(3, "AfterEdit", EventCallback.Factory.Create<NvlNhapXuatKhoShow>(this, AfterEditAsync));
                builder.CloseComponent();
            };

           await dxPopup.showAsync("CONNECT PRINTER");
           await dxPopup.ShowAsync();
        }
        public async Task NhapXuatMasterDeleteAsync()
        {
            try
            {
                IsOpenfly = false;
                await dxFlyoutchucnang.CloseAsync();
                bool bl = await dialogMsg.Show("Xóa chứng từ", $"Bạn có chắc muốn chứng từ số {nvlNhapXuatKhoShowcrr.Serial.ToString()}");

                if (!phanQuyenAccess.CheckDeleteNhapXuatKho(nvlNhapXuatKhoShowcrr.UserInsert, ModelAdmin.users))
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không có quyền xóa dòng này do bạn không phải người tạo"));
                    //msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                    return;

                }
                if (bl)
                {

                    try
                    {
                        CallAPI callAPI = new CallAPI();
                        string sql = "NVLDB.dbo.NvlNhapXuat_Delete";
                        List<ParameterDefine> lstpara = new List<ParameterDefine>();
                        lstpara.Add(new ParameterDefine("@Serial", nvlNhapXuatKhoShowcrr.Serial));
                        lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));
                        lstpara.Add(new ParameterDefine("@LyDoDelete", ModelAdmin.users.UsersName + " đã xóa"));
                        string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                        if (json != "")
                        {
                            var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                            if (query[0].ketqua == "OK")
                            {
                                ToastService.Notify(new ToastMessage(ToastType.Success, "Xóa thành công"));
                                int? flag = nvlNhapXuatKhoShowcrr.flag;



                                lstNhapXuatKhoSearchShow.Remove(nvlNhapXuatKhoShowcrr);

                                if (nvlNhapXuatKhoShowcrr.flag > 0)//Chứng từ chuyển
                                {
                                    var querydup = lstNhapXuatKhoSearchShow.FirstOrDefault(p => p.flag == flag);
                                    if (querydup != null)
                                        lstNhapXuatKhoSearchShow.Remove(querydup);
                                }
                                await dxGrid.SaveChangesAsync();

                                dxGrid.Reload();
                            }
                            else
                            {
                                ToastService.Notify(new ToastMessage(ToastType.Warning, $"{query[0].ketqua}, {query[0].ketquaexception}"));
                                //msgBox.Show("Lỗi: " + query[0].ketqua, IconMsg.iconssuccess);

                            }
                            //Grid.Data = lstDonDatHangSearchShow;
                        }
                    }
                    catch (Exception ex)
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Danger, $"Lỗi: " + ex.Message));
                    }

                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Lỗi: " + ex.Message));
            }
        }
        private async void ShowNhapXuatItemAdd()
        {
            try
            {
                IsOpenfly = false;
                await dxFlyoutchucnang.CloseAsync();
                if (loaiNhapXuat == null)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi Loại nhập xuất"));
                    return;
                }
                //Kiểm tra chứng từ chuyển
                if (nvlNhapXuatKhoShowcrr.flag > 0)//Chứng từ chuyển
                {
                    if (loaiNhapXuat == "ChuyenKho")
                    {
                        if (nvlNhapXuatKhoShowcrr.STTCT > 0)
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Danger, "Đây là chứng từ nhận, bạn phải xuất hàng ở chứng từ chuyển"));
                            return;
                        }
                    }
                    if (loaiNhapXuat == "NhapGiaCong")
                    {
                        if (nvlNhapXuatKhoShowcrr.STTCT < 0)
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Danger, "Đây là chứng từ xuất, bạn phải nhập hàng ở chứng từ Nhập"));
                            return;
                        }
                    }

                }
                if (!phanQuyenAccess.CheckDelete(nvlNhapXuatKhoShowcrr.UserInsert, ModelAdmin.users))
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Đây không phải chứng từ bạn tạo, bạn không được thêm vào đây"));
                    return;
                }

                NvlNhapXuatItemShow nvlNhapXuatItemShow = new NvlNhapXuatItemShow();
                //nvlNhapXuatItemShow.ViTri = "Temp";
                var queryvt = lstvitri.Where(p => p.MaKho == nvlNhapXuatKhoShowcrr.MaKho).ToList();
                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_NhapXuatItemAdd>(0);
                    builder.AddAttribute(1, "nvlNhapXuatKhoShowcrr", nvlNhapXuatKhoShowcrr.CopyClass());
                    builder.AddAttribute(2, "nvlNhapXuatItemShowcrr", nvlNhapXuatItemShow);
                    builder.AddAttribute(3, "LoaiNhapXuat", loaiNhapXuat);
                    builder.AddAttribute(4, "lstViTri", queryvt);


                    builder.CloseComponent();
                };

                await dxPopup.showAsync("THÊM HÀNG HÓA");
                await dxPopup.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Lỗi:" + ex.Message);
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Lỗi: " + ex.Message));
            }
        }
        private async Task ShowNhapXuatItemAddAsync(NvlNhapXuatKhoShow nvlNhapXuatKhoShow)
        {
            try
            {
                IsOpenfly = false;
                await dxFlyoutchucnang.CloseAsync();
                if (loaiNhapXuat == null)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi Loại nhập xuất"));
                    return;
                }
                nvlNhapXuatKhoShowcrr = nvlNhapXuatKhoShow;
                if (nvlNhapXuatKhoShow.flag > 0)//Chứng từ chuyển
                {
                    if (loaiNhapXuat == "ChuyenKho")
                    {
                        if (nvlNhapXuatKhoShowcrr.STTCT > 0)
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Danger, "Đây là chứng từ nhận, bạn phải xuất hàng ở chứng từ chuyển"));
                            return;
                        }
                    }
                    if (loaiNhapXuat == "NhapGiaCong")
                    {
                        if (nvlNhapXuatKhoShowcrr.STTCT < 0)
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Danger, "Đây là chứng từ xuất, bạn phải nhập hàng ở chứng từ Nhập"));
                            return;
                        }
                    }
                }
                if (!phanQuyenAccess.CheckDelete(nvlNhapXuatKhoShowcrr.UserInsert, ModelAdmin.users))
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Đây không phải chứng từ bạn tạo, bạn không được thêm vào đây"));
                    return;
                }
                NvlNhapXuatItemShow nvlNhapXuatItemShow = new NvlNhapXuatItemShow();
                nvlNhapXuatItemShow.ViTri = "Temp";
                var queryvt = lstvitri.Where(p => p.MaKho == nvlNhapXuatKhoShow.MaKho).ToList();
                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_NhapXuatItemAdd>(0);
                    builder.AddAttribute(1, "nvlNhapXuatKhoShowcrr", nvlNhapXuatKhoShow.CopyClass());
                    builder.AddAttribute(2, "nvlNhapXuatItemShowcrr", nvlNhapXuatItemShow);
                    builder.AddAttribute(3, "LoaiNhapXuat", loaiNhapXuat);

                    builder.AddAttribute(4, "lstViTri", queryvt);

                    builder.CloseComponent();
                };

                await dxPopup.showAsync("THÊM HÀNG HÓA");
                await dxPopup.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Lỗi:" + ex.Message);
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Lỗi: " + ex.Message));
            }
        }
        private async void ShowNhapXuatItemAddDuplicate()
        {
            try
            {
                IsOpenfly = false;
                await dxFlyoutchucnang.CloseAsync();
                if (loaiNhapXuat == null)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi Loại nhập xuất"));
                    return;
                }
                if (nvlNhapXuatKhoShowcrr.flag > 0)//Chứng từ chuyển
                {
                    if (nvlNhapXuatKhoShowcrr.STTCT > 0)
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Danger, "Đây là chứng từ nhận, bạn phải xuất hàng ở chứng từ chuyển"));
                        return;
                    }
                }
                if (!phanQuyenAccess.CheckDelete(nvlNhapXuatKhoShowcrr.UserInsert, ModelAdmin.users))
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Đây không phải chứng từ bạn tạo, bạn không được thêm vào đây"));
                    return;
                }
                NvlNhapXuatItemShow nvlNhapXuatItemShow = new NvlNhapXuatItemShow();
                nvlNhapXuatItemShow.ViTri = "Temp";
                var queryvt = lstvitri.Where(p => p.MaKho == nvlNhapXuatKhoShowcrr.MaKho).ToList();
                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_NhapXuatItemAdd>(0);
                    builder.AddAttribute(1, "nvlNhapXuatKhoShowcrr", nvlNhapXuatKhoShowcrr.CopyClass());
                    builder.AddAttribute(1, "CheckDuplicate", true);
                    builder.AddAttribute(2, "nvlNhapXuatItemShowcrr", nvlNhapXuatItemShow);
                    builder.AddAttribute(3, "LoaiNhapXuat", loaiNhapXuat);
                    builder.AddAttribute(4, "lstViTri", queryvt);
                    builder.CloseComponent();
                };

                await dxPopup.showAsync("THÊM HÀNG HÓA");
                await dxPopup.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Lỗi:" + ex.Message);
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Lỗi: " + ex.Message));
            }
        }
        public async void ShowPopupinDetail(ShowFragmentinModal showFragmentinModal)
        {
            if (showFragmentinModal.show)
            {
                renderFragmentdetail = showFragmentinModal.renderFragment;// = _renderFragment;
                await dxPopupDetail.showAsync(showFragmentinModal.title);
                await dxPopupDetail.ShowAsync();
            }
            else
            {
                await dxPopupDetail.CloseAsync();
            }
        }
        public async Task PrintPhieuAsync(bool type)
        {
            try
            {
                string sql = "";
                IsOpenfly = false;
                await dxFlyoutchucnang.CloseAsync();

                string tenbaocao = "";
                if (nvlNhapXuatKhoShowcrr.STTCT > 0)
                {
                    if (type)
                    {
                        tenbaocao = "nhapkhocogia";
                    }
                    else
                        tenbaocao = "nhapkhokhonggia";
                }
                else
                {
                    if (type)
                    {
                        tenbaocao = "xuatkhochitiet";
                    }
                    else
                        tenbaocao = "xuatkhogop";
                }
                if (tenbaocao == "xuatkhochitiet")
                {
                    sql = string.Format(@"use NVLDB  declare @serial int={0}
                                declare @tblserialink Table(SerialLink int primary key)
                                insert into @tblserialink(SerialLink)
                                (select SerialLink from NvlNhapXuatItem where SerialCT=@serial group by SerialLink)

                                declare @tbl Table(SerialLink int,TenNCC nvarchar(200))
                                insert into @tbl(SerialLink,TenNCC)

                                select qryminserial.SerialLink,ngn.TenGN as TenNCC from 
                                (select * from 
                                (select  ROW_NUMBER() OVER (PARTITION BY SerialLink ORDER BY Serial) as Rownum,Serial,SerialCT,SerialLink,SerialKHDH
                                from NvlNhapXuatItem where SerialLink in (select SerialLink from @tblserialink) and SLNhap>0) as qry 
                                where Rownum=1) as qryminserial inner join dbo.NvlNhapXuat nx on qryminserial.SerialCT=nx.Serial
                                inner join dbo.View_NoiGN ngn on nx.MaGN =ngn.MaGN
 


                                Select nxitem.SerialCT,nxitem.SerialLink,kh.UserInsert,nxitem.SerialKHDH,nxitem.SLNhap,nxitem.DonGia,hh.DVT,nxitem.MaHang, hh.TenHang,nx.GhiChu, nx.MaCT,nx.DienGiai,nx.ChatLuong, nx.MaGN,mk.TenKho,nx.LyDo, nx.NhaMay, nx.Ngay, nxitem.DonGia*nxitem.SLNhap as ThanhTien, View_NoiGN.TenGN as TenGN, hh.QuyCach,usr.TenUser,usrkh.TenUser as NguoiDeNghi,tbl.TenNCC
                                from (select * from NvlNhapXuat where Serial=serial) nx 
	                                inner JOIN (Select SerialCT,MaHang,(SLNhap+SLXuat) as SLNhap,DonGia as DonGia,SerialKHDH,SerialLink from NvlNhapXuatItem where SerialCT = @serial ) nxitem  on nx.Serial = nxitem.SerialCT 
                                     inner JOIN  NvlHangHoa as hh on nxitem.MaHang = hh.MaHang 
	                                 inner JOIN View_NoiGN on nx.MaGN = View_NoiGN.MaGN inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                                    inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName
									left join dbo.NvlKeHoachMuaHangItem kh on nxitem.SerialKHDH=kh.Serial
									left join DBMaster.dbo.Users as usrkh on kh.UserInsert=usrkh.UsersName
									left join @tbl tbl on nxitem.SerialLink=tbl.SerialLink", nvlNhapXuatKhoShowcrr.Serial);
                }
                if (tenbaocao == "xuatkhogop")
                {
                    sql = string.Format(@"use NVLDB  
                            declare @serial int={0}
                            declare @tblxk as Table(SerialCT int,SerialLink int,MaHang nvarchar(100),SLXuat decimal(18,6))
                            insert into @tblxk(SerialCT,SerialLink,MaHang,SLXuat)
                            select SerialCT,SerialLink,MaHang,SLXuat from NvlNhapXuatItem where SerialCT=@serial

                            --Xử lý nguồn gốc
                            declare @tblSerial as table(SerialLink int primary key)
                            insert into @tblSerial(SerialLink) 
                            select SerialLink from @tblxk group by SerialLink

                            --Xử lý nguồn gốc của SerialLink
                            declare @tblnguongoc table(SerialLink int primary key,NguonGoc nvarchar(200))
                            insert into @tblnguongoc(SerialLink,NguonGoc)
                            select SerialLink,case when nx.MaGN='CC0000' then '' else  ngn.TenGN end as NguonGoc from 
                            (select *,ROW_NUMBER() OVER (PARTITION BY SerialLink ORDER BY SerialCT asc) as STT from
                            (select MaHang,SerialLink,SLNhap,SerialCT 
                            from NvlNhapXuatItem where SLNhap>0 and SerialLink 
                            in (select SerialLink from @tblSerial)) as qry) as qryct
                            inner join dbo.NvlNhapXuat nx on nx.Serial=qryct.SerialCT
                            inner join dbo.View_NoiGN ngn on nx.MaGN=ngn.MaGN
                            where STT=1

                            declare @tblxuatkho table(SerialCT int,MaHang nvarchar(100),SLXuat decimal(18,6),NguonGoc nvarchar(100))
                            insert into @tblxuatkho(SerialCT,MaHang,SLXuat,NguonGoc)
                            select tblxk.SerialCT,tblxk.MaHang,sum(tblxk.SLXuat) as SLXuat,tblnguongoc.NguonGoc
                            from @tblxk tblxk left join @tblnguongoc tblnguongoc on tblxk.SerialLink=tblnguongoc.SerialLink
                            group by tblxk.MaHang,tblnguongoc.NguonGoc,tblxk.SerialCT

                                Select  nxitem.SerialCT,nxitem.SLXuat as  SLNhap,hh.DVT,nxitem.MaHang,nx.DienGiai, hh.TenHang,nx.GhiChu, nx.MaCT, nx.MaGN,mk.TenKho,nx.LyDo, nx.NhaMay, nx.Ngay,nx.ChatLuong, View_NoiGN.TenGN as TenGN, hh.QuyCach,usr.TenUser,NguonGoc
                                from (select * from NvlNhapXuat where Serial=@serial) nx 
	                                inner JOIN
									@tblxuatkho nxitem  on nx.Serial=nxitem.SerialCT
                                    inner JOIN  NvlHangHoa as hh on nxitem.MaHang = hh.MaHang 
	                                inner JOIN View_NoiGN on nx.MaGN = View_NoiGN.MaGN inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                                    inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName", nvlNhapXuatKhoShowcrr.Serial);

                }
                if (tenbaocao == "nhapkhocogia" || tenbaocao == "nhapkhokhonggia")
                {
                    sql = string.Format(@"use NVLDB  declare @serial int={0}
                                Select nxitem.SerialCT,nxitem.SLNhap,nxitem.DonGia,hh.DVT,nxitem.MaHang,nx.DienGiai, hh.TenHang,nx.GhiChu, nx.MaCT, nx.MaGN,mk.TenKho,nx.LyDo, nx.NhaMay, nx.Ngay, nxitem.DonGia*nxitem.SLNhap as ThanhTien,nx.ChatLuong, View_NoiGN.TenGN as TenGN, hh.QuyCach,usr.TenUser
                                from (select * from NvlNhapXuat where Serial=serial) nx 
	                                inner JOIN (Select SerialCT,MaHang,sum(SLNhap+SLXuat) as SLNhap,avg(DonGia) as DonGia from NvlNhapXuatItem where SerialCT = @serial group by MaHang,SerialCT) nxitem  on nx.Serial = nxitem.SerialCT 
                                    inner JOIN  NvlHangHoa as hh on nxitem.MaHang = hh.MaHang 
	                                inner JOIN View_NoiGN on nx.MaGN = View_NoiGN.MaGN inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                                    inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName", nvlNhapXuatKhoShowcrr.Serial);
                }
                CallAPI callAPI = new CallAPI();

                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string json = "";
                if (!string.IsNullOrEmpty(signalRConnect.PrinterID) && Ismobile)//Xuất thẳng trang in nếu có kết nối
                {
                    json = await callAPI.ExcuteQueryEncryptMsgAsync(sql, lstpara, signalRConnect.PrinterID, tenbaocao);
                }
                else
                {
                    json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                }
                if (string.IsNullOrEmpty(signalRConnect.PrinterID) || !Ismobile)//Nếu không phải điện thoại hoặc không có kết nối máy in
                {
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<DataTable>(json);
                        if (tenbaocao == "nhapkhocogia")
                        {
                            XtraRp_PhieuNhapKho xtraRp_PhieuNhapKho = new XtraRp_PhieuNhapKho();

                            var querytotal = query.Compute("sum(ThanhTien)", string.Empty);
                            string bangchu = "";
                            if (querytotal != null)
                            {
                                double d = double.Parse(querytotal.ToString());
                                bangchu = string.Format("{0} đồng", prs.docsothapphan(d));
                            }
                            query.Columns.Add("BangChu", typeof(string));
                            query.Rows[query.Rows.Count - 1]["BangChu"] = bangchu;
                            xtraRp_PhieuNhapKho.DataSource = query;
                            await ModelAdmin.mainLayout.showreportAsync(xtraRp_PhieuNhapKho);

                        }
                        if (tenbaocao == "nhapkhokhonggia")
                        {
                            XtraRp_PhieuNhapKho_KhongGia xtraRp_PhieuNhapKho = new XtraRp_PhieuNhapKho_KhongGia();

                            xtraRp_PhieuNhapKho.DataSource = query;
                            await ModelAdmin.mainLayout.showreportAsync(xtraRp_PhieuNhapKho);

                        }
                        if (tenbaocao == "xuatkhochitiet")
                        {
                            XtraRp_PhieuXuatKho_ChiTiet xtraRp_PhieuNhapKho = new XtraRp_PhieuXuatKho_ChiTiet();

                            xtraRp_PhieuNhapKho.DataSource = query;
                            await ModelAdmin.mainLayout.showreportAsync(xtraRp_PhieuNhapKho);
                        }
                        if (tenbaocao == "xuatkhogop")
                        {

                            XtraRp_PhieuXuatKho_KhongGia xtraRp_PhieuNhapKho = new XtraRp_PhieuXuatKho_KhongGia();

                            xtraRp_PhieuNhapKho.DataSource = query;
                            await ModelAdmin.mainLayout.showreportAsync(xtraRp_PhieuNhapKho);
                        }
                        query.Dispose();
                    }
                }


            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "LỖI: " + ex.Message));
                Console.Error.WriteLine(ex.Message);
            }
        }
        public async Task PrintPhieuAsyncNew(bool type)
        {
            string sql = "";
            await dxFlyoutchucnang.CloseAsync();
            
            string tenbaocao = "";
            if (nvlNhapXuatKhoShowcrr.STTCT > 0)
            {
                if (type)
                {
                    tenbaocao = "nhapkhocogia";
                }
                else
                    tenbaocao = "nhapkhokhonggia";
            }
            else
            {
                if (type)
                {
                    tenbaocao = "xuatkhochitiet";
                }
                else
                    tenbaocao = "xuatkhogop";
            }
            if (tenbaocao == "xuatkhochitiet")
            {
                sql = string.Format(@"use NVLDB  declare @serial int={0}
                                declare @tblserialink Table(SerialLink int primary key)
                                insert into @tblserialink(SerialLink)
                                (select SerialLink from NvlNhapXuatItem where SerialCT=@serial group by SerialLink)

                                declare @tbl Table(SerialLink int,TenNCC nvarchar(200))
                                insert into @tbl(SerialLink,TenNCC)

                                select qryminserial.SerialLink,ngn.TenGN as TenNCC from 
                                (select * from 
                                (select  ROW_NUMBER() OVER (PARTITION BY SerialLink ORDER BY Serial) as Rownum,Serial,SerialCT,SerialLink,SerialKHDH
                                from NvlNhapXuatItem where SerialLink in (select SerialLink from @tblserialink) and SLNhap>0) as qry 
                                where Rownum=1) as qryminserial inner join dbo.NvlNhapXuat nx on qryminserial.SerialCT=nx.Serial
                                inner join dbo.View_NoiGN ngn on nx.MaGN =ngn.MaGN
 


                                Select nxitem.SerialCT,nxitem.SerialLink,kh.UserInsert,nxitem.SerialKHDH,nxitem.SLNhap,nxitem.DonGia,hh.DVT,nxitem.MaHang, hh.TenHang,nx.GhiChu, nx.MaCT,nx.DienGiai,nx.ChatLuong, nx.MaGN,mk.TenKho,nx.LyDo, nx.NhaMay, nx.Ngay, nxitem.DonGia*nxitem.SLNhap as ThanhTien, View_NoiGN.TenGN as TenGN, hh.QuyCach,usr.TenUser,usrkh.TenUser as NguoiDeNghi,tbl.TenNCC
                                from (select * from NvlNhapXuat where Serial=serial) nx 
	                                inner JOIN (Select SerialCT,MaHang,(SLNhap+SLXuat) as SLNhap,DonGia as DonGia,SerialKHDH,SerialLink from NvlNhapXuatItem where SerialCT = @serial ) nxitem  on nx.Serial = nxitem.SerialCT 
                                     inner JOIN  NvlHangHoa as hh on nxitem.MaHang = hh.MaHang 
	                                 inner JOIN View_NoiGN on nx.MaGN = View_NoiGN.MaGN inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                                    inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName
									left join dbo.NvlKeHoachMuaHangItem kh on nxitem.SerialKHDH=kh.Serial
									left join DBMaster.dbo.Users as usrkh on kh.UserInsert=usrkh.UsersName
									left join @tbl tbl on nxitem.SerialLink=tbl.SerialLink", nvlNhapXuatKhoShowcrr.Serial);
            }
            if (tenbaocao == "xuatkhogop")
            {
                //Code cũ
                //       sql = string.Format(@"use NVLDB  
                //                   declare @serial int={0}
                //                   declare @tblxk as Table(SerialCT int,SerialLink int,MaHang nvarchar(100),SLXuat decimal(18,6))
                //                   insert into @tblxk(SerialCT,SerialLink,MaHang,SLXuat)
                //                   select SerialCT,SerialLink,MaHang,SLXuat from NvlNhapXuatItem where SerialCT=@serial

                //                   --Xử lý nguồn gốc
                //                   declare @tblSerial as table(SerialLink int primary key)
                //                   insert into @tblSerial(SerialLink) 
                //                   select SerialLink from @tblxk group by SerialLink

                //                   --Xử lý nguồn gốc của SerialLink
                //                   declare @tblnguongoc table(SerialLink int primary key,NguonGoc nvarchar(200))
                //                   insert into @tblnguongoc(SerialLink,NguonGoc)
                //                   select SerialLink,case when nx.MaGN='CC0000' then '' else  ngn.TenGN end as NguonGoc from 
                //                   (select *,ROW_NUMBER() OVER (PARTITION BY SerialLink ORDER BY SerialCT asc) as STT from
                //                   (select MaHang,SerialLink,SLNhap,SerialCT 
                //                   from NvlNhapXuatItem where SLNhap>0 and SerialLink 
                //                   in (select SerialLink from @tblSerial)) as qry) as qryct
                //                   inner join dbo.NvlNhapXuat nx on nx.Serial=qryct.SerialCT
                //                   inner join dbo.View_NoiGN ngn on nx.MaGN=ngn.MaGN
                //                   where STT=1

                //                   declare @tblxuatkho table(SerialCT int,MaHang nvarchar(100),SLXuat decimal(18,6),NguonGoc nvarchar(100))
                //                   insert into @tblxuatkho(SerialCT,MaHang,SLXuat,NguonGoc)
                //                   select tblxk.SerialCT,tblxk.MaHang,sum(tblxk.SLXuat) as SLXuat,tblnguongoc.NguonGoc
                //                   from @tblxk tblxk left join @tblnguongoc tblnguongoc on tblxk.SerialLink=tblnguongoc.SerialLink
                //                   group by tblxk.MaHang,tblnguongoc.NguonGoc,tblxk.SerialCT

                //                       Select  nxitem.SerialCT,nxitem.SLXuat as  SLNhap,hh.DVT,nxitem.MaHang,nx.DienGiai, hh.TenHang,nx.GhiChu, nx.MaCT, nx.MaGN,mk.TenKho,nx.LyDo, nx.NhaMay, nx.Ngay,nx.ChatLuong, View_NoiGN.TenGN as TenGN, hh.QuyCach,usr.TenUser,NguonGoc
                //                       from (select * from NvlNhapXuat where Serial=@serial) nx 
                //                        inner JOIN
                //@tblxuatkho nxitem  on nx.Serial=nxitem.SerialCT
                //                           inner JOIN  NvlHangHoa as hh on nxitem.MaHang = hh.MaHang 
                //                        inner JOIN View_NoiGN on nx.MaGN = View_NoiGN.MaGN inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                //                           inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName", nvlNhapXuatKhoShowcrr.Serial);
                
                sql = string.Format(@"use NVLDB  
                            declare @serial int={0}
                            declare @tblxk as Table(SerialCT int,SerialLink int,SerialKHDH int,MaHang nvarchar(100),SLXuat decimal(18,6),GhiChu nvarchar(200))
                            insert into @tblxk(SerialCT,SerialLink,SerialKHDH,MaHang,SLXuat,GhiChu)
                            select SerialCT,SerialLink,SerialKHDH,MaHang,SLXuat,GhiChu from NvlNhapXuatItem where SerialCT=@serial
							

								--Xử lý lấy đồng bộ và định mức
								declare @tblketquafinaldm table(Serial int,TenSP nvarchar(200),TieuDe nvarchar(500),SLQuyDoi float)
								declare @tblSerialKH Table(SerialKH int primary key)

								insert into @tblSerialKH(SerialKH)
								select SerialKHDH from NvlNhapXuatItem where SerialCT=@serial group by SerialKHDH
								declare @tbldinhmuc table(MaSP nvarchar(100),MaMau nvarchar(100),KeyGroup nvarchar(100),TenDinhMuc nvarchar(300),CongDoan nvarchar(100),SoLuong decimal(18,6))

								declare @tblKeHoachItem table(Serial int,KeyGroup nvarchar(100),MaHang nvarchar(100))
								insert into @tblKeHoachItem(Serial,KeyGroup,MaHang)
								select Serial,KeyGroup,MaHang from NvlKeHoachMuaHangItem where Serial in (select SerialKH from @tblSerialKH)

								insert into @tbldinhmuc(MaSP,MaMau,KeyGroup,TenDinhMuc,CongDoan,SoLuong)
								SELECT [MaSP],[MaMau],[KeyGroup],[TenDinhMuc],[CongDoan],[SoLuong]
								FROM [NvlKeHoachMuaHang_DinhMuc]
								where KeyGroup in (
								select KeyGroup from @tblKeHoachItem)
								declare @checkdinhmuc nvarchar(100)
								select Top 1 @checkdinhmuc=MaSP from @tbldinhmuc where MaSP is not null
								if(@checkdinhmuc is not null)
								begin

								declare @lstmasp nvarchar(max)
								SELECT @lstmasp=STUFF((
									SELECT DISTINCT ';' + MaSP
									FROM @tbldinhmuc
									FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 
								1, 1, '')

								exec GetDinhMucNVL_SanPhamList @lstsanpham=@lstmasp

								declare @tblketquaquydoi Table(Serial int,TenSP nvarchar(100),TenDinhMuc nvarchar(200),CongDoan nvarchar(200),SLDongBo float,SLQuyDoi float,MaHang nvarchar(100))

								insert into @tblketquaquydoi(Serial,TenSP,TenDinhMuc,CongDoan,SLDongBo,SLQuyDoi,MaHang)
								select tblkh.Serial,dmtc.TenSP,tbldm.TenDinhMuc,tbldm.CongDoan,tbldm.SoLuong as SLDongBo,dmtc.SLQuyDoi,dmtc.MaVatTu
								from 
								@tblKeHoachItem tblkh left join
								@tbldinhmuc tbldm on tblkh.KeyGroup=tbldm.KeyGroup
								left join (select MaSP,GroupMauSP,CongDoan,MaVatTu,TenSP,min(SLQuyDoi) as SLQuyDoi from ##tmpdinhmuctoancuc group by MaSP,GroupMauSP,CongDoan,MaVatTu,TenSP) dmtc
								on 
								(tbldm.MaSP=dmtc.MaSP and tbldm.MaMau=dmtc.GroupMauSP  and tbldm.CongDoan=dmtc.CongDoan
								and tblkh.MaHang=dmtc.MaVatTu)
								insert into @tblketquafinaldm(Serial,TenSP,TieuDe,SLQuyDoi)
								SELECT Serial,TenSP,TenSP+': '+TenDinhMuc+' - '+CongDoan+' ('+CAST(round(SLDongBo,0) as nvarchar(100))+N' bộ)',SLQuyDoi
								FROM (SELECT *,  ROW_NUMBER() OVER (PARTITION BY Serial ORDER BY Serial) AS rn
									FROM @tblketquaquydoi
								) AS T
								WHERE rn = 1
								drop table ##tmpdinhmuctoancuc
								end
								--Xử lý giá trị trùng
								
								
								
                            --Xử lý nguồn gốc
                            declare @tblSerial as table(SerialLink int primary key)
                            insert into @tblSerial(SerialLink) 
                            select SerialLink from @tblxk group by SerialLink

						


                            --Xử lý nguồn gốc của SerialLink
                            declare @tblnguongoc table(SerialLink int primary key,NguonGoc nvarchar(200))
                            insert into @tblnguongoc(SerialLink,NguonGoc)
                            select SerialLink,case when nx.MaGN='CC0000' then '' else  ngn.TenGN end as NguonGoc from 
                            (select *,ROW_NUMBER() OVER (PARTITION BY SerialLink ORDER BY SerialCT asc) as STT from
                            (select MaHang,SerialLink,SerialKHDH,SLNhap,SerialCT 
                            from NvlNhapXuatItem where SLNhap>0 and SerialLink 
                            in (select SerialLink from @tblSerial)) as qry) as qryct
                            inner join dbo.NvlNhapXuat nx on nx.Serial=qryct.SerialCT
                            inner join dbo.View_NoiGN ngn on nx.MaGN=ngn.MaGN
                            where STT=1

                            declare @tblxuatkho table(SerialCT int,MaHang nvarchar(100),TieuDe nvarchar(500),SLQuyDoi float,SLXuat decimal(18,6),NguonGoc nvarchar(100))
                            insert into @tblxuatkho(SerialCT,MaHang,TieuDe,SLXuat,NguonGoc,SLQuyDoi)
                            select tblxk.SerialCT,tblxk.MaHang,case when tblxk.SerialKHDH=0 then 'zzzz.'+GhiChu else  TieuDe end as TieuDe,sum(tblxk.SLXuat) as SLXuat,tblnguongoc.NguonGoc,min(SLQuyDoi) as SLQuyDoi
                            from @tblxk tblxk left join @tblnguongoc tblnguongoc on tblxk.SerialLink=tblnguongoc.SerialLink
							left join @tblketquafinaldm tbldm on tblxk.SerialKHDH=tbldm.Serial
                            group by tblxk.MaHang,tblnguongoc.NguonGoc,tblxk.SerialCT,case when tblxk.SerialKHDH=0 then 'zzzz.'+GhiChu else  TieuDe end

							

                                Select  nxitem.SerialCT,nxitem.SLXuat as  SLNhap,case when (SLQuyDoi is null or SLQuyDoi=0) then null else nxitem.SLXuat/SLQuyDoi end as SLDongBo,hh.DVT,nxitem.MaHang,nx.DienGiai,isnull(nxitem.TieuDe,N'zzzz. Đề nghị ngoài định mức') as TieuDe, hh.TenHang,nx.GhiChu, nx.MaCT, nx.MaGN,mk.TenKho,nx.LyDo, nx.NhaMay, nx.Ngay,nx.ChatLuong, View_NoiGN.TenGN as TenGN, hh.QuyCach,usr.TenUser,NguonGoc
                                from (select * from NvlNhapXuat where Serial=@serial) nx 
	                                inner JOIN
									@tblxuatkho nxitem  on nx.Serial=nxitem.SerialCT
									
                                    inner JOIN  NvlHangHoa as hh on nxitem.MaHang = hh.MaHang 
	                                inner JOIN View_NoiGN on nx.MaGN = View_NoiGN.MaGN inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                                    inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName
									
									
									", nvlNhapXuatKhoShowcrr.Serial);

            }
            if (tenbaocao == "nhapkhocogia" || tenbaocao == "nhapkhokhonggia")
            {
                sql = string.Format(@"use NVLDB  declare @serial int={0}
                                Select nxitem.SerialCT,nxitem.SLNhap,nxitem.DonGia,hh.DVT,nxitem.MaHang,nx.DienGiai, hh.TenHang,nx.GhiChu, nx.MaCT, nx.MaGN,mk.TenKho,nx.LyDo, nx.NhaMay, nx.Ngay, nxitem.DonGia*nxitem.SLNhap as ThanhTien,nx.ChatLuong, View_NoiGN.TenGN as TenGN, hh.QuyCach,usr.TenUser
                                from (select * from NvlNhapXuat where Serial=serial) nx 
	                                inner JOIN (Select SerialCT,MaHang,sum(SLNhap+SLXuat) as SLNhap,DonGia as DonGia from NvlNhapXuatItem where SerialCT = @serial group by MaHang,SerialCT,DonGia) nxitem  on nx.Serial = nxitem.SerialCT 
                                    inner JOIN  NvlHangHoa as hh on nxitem.MaHang = hh.MaHang 
	                                inner JOIN View_NoiGN on nx.MaGN = View_NoiGN.MaGN inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                                    inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName", nvlNhapXuatKhoShowcrr.Serial);
            }

            try
            {
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string json = "";
                if (!string.IsNullOrEmpty(signalRConnect.PrinterID) && Ismobile)//Xuất thẳng trang in nếu có kết nối
                {
                    CallAPI callAPI = new CallAPI();
                    json = await callAPI.ExcuteQueryEncryptMsgAsync(sql, lstpara, signalRConnect.PrinterID, tenbaocao);
                }
                else//Nếu không phải điện thoại hoặc không có kết nối máy in
                {

                    GetDataFromSql getDataFromSql = new GetDataFromSql();
                    getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);

                    getDataFromSql.sql = sql;

                    if (tenbaocao == "nhapkhocogia")
                    {
                        getDataFromSql.reportname = "XtraRp_PhieuNhapKho";
                    }
                    if (tenbaocao == "nhapkhokhonggia")
                    {
                        getDataFromSql.reportname = "XtraRp_PhieuNhapKho_KhongGia";
                    }
                    if (tenbaocao == "xuatkhochitiet")
                    {
                        getDataFromSql.reportname = "XtraRp_PhieuXuatKho_ChiTiet";
                    }
                    if (tenbaocao == "xuatkhogop")
                    {
                        getDataFromSql.reportname = "XtraRp_PhieuXuatKho_KhongGia";
                    }
                    await ModelAdmin.mainLayout.showreportpdfAsync(getDataFromSql);


                }


            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "LỖI: " + ex.Message));
                Console.Error.WriteLine(ex.Message);
            }
        }
        bool CheckGia = false;
        string titlegia = "Không giá";
        private string showtittle(NvlNhapXuatKhoShow nvlNhapXuatKhoShow)
        {
            if (nvlNhapXuatKhoShow.STTCT > 0)
            {
                if (!CheckGia)
                    return "Không giá";
                return "Có giá";
            }
            else
            {
                if (!CheckGia)
                    return "Gộp mã hàng";
                return "Chi tiết";
            }
        }
        void CheckedGiaChanged(bool value)
        {
            CheckGia = value;

            showtittle(nvlNhapXuatKhoShowcrr);


        }
        private async Task ExportKiemKeAsync()
        {
            if (dtpbegin == null || dtpend == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn ngày"));
                return;
            }

            string dieukien = " where LyDo=@LyDo and Ngay>=@DateBegin and Ngay<=@DateEnd";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@LyDo", "NHẬP KHO KIỂM KÊ"));
            lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy")));
            lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy")));
            if (khonvlselected != null)
            {
                dieukien += " and MaKho=@MaKho";
                lstpara.Add(new ParameterDefine("@MaKho", khonvlselected.Name));
            }
            if (userselect != null)
            {
                dieukien += " and UserInsert=@UserInsert";
                lstpara.Add(new ParameterDefine("@UserInsert", userselect.UsersName));
            }
            string sql = string.Format(@"use NVLDB   
          
            Select qrynx.SLNhap,hh.DVT,qrynx.MaHang, hh.TenHang,mk.TenKho,qrynx.LyDo, hh.QuyCach,usr.TenUser
            from 
            (select MaHang,sum(SLNhap) as SLNhap,LyDo,MaKho,nx.UserInsert from
            ((select * from NvlNhapXuat {0}) nx 
            inner JOIN (Select SerialCT,MaHang,sum(SLNhap+SLXuat) as SLNhap 
            from NvlNhapXuatItem  group by MaHang,SerialCT) nxitem  on nx.Serial = nxitem.SerialCT ) group by MaHang,LyDo,MaKho,nx.UserInsert) as qrynx
            inner JOIN  NvlHangHoa as hh on qrynx.MaHang = hh.MaHang 
            inner join dbo.NvlMaKho mk on qrynx.MaKho=mk.MaKho
            inner join DBMaster.dbo.Users usr on qrynx.UserInsert=usr.UsersName order by  hh.TenHang", dieukien);
            CallAPI callAPI = new CallAPI();
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<DataTable>(json);

                    XtraRp_BienBanKiemKe xtraRp_PhieuNhapKho = new XtraRp_BienBanKiemKe();
                    xtraRp_PhieuNhapKho.DataSource = query;
                    await ModelAdmin.mainLayout.showreportAsync(xtraRp_PhieuNhapKho);
                    query.Dispose();

                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "LỖI: " + ex.Message));
                Console.Error.WriteLine(ex.Message);
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }
        }

        List<NvlNhapXuatItemShow> lstrp = new List<NvlNhapXuatItemShow>();
        private async Task InBangKeAsync()
        {


            string ghichu = "";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();

            lstcolumn.Clear();
            string sql = "";


            if (dtpbegin == null || dtpend == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Chọn ngày "));
                return;
            }
            TimeSpan difference = dtpend.Value - dtpbegin.Value;

            // Lấy số ngày
            int days = difference.Days;
            if (days == 0)
            {
                showallgroup = true;
            }
            else
                showallgroup = false;

            string dieukien = "";

            if (sochungtu == null)
            {

                if ((dtpend.Value - dtpbegin.Value).TotalDays > 31)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Bảng kê này hỗ trợ Ngày bắt đầu và ngày kết thúc không quá 31 ngày"));
                    return;
                }
                dieukien += " Where Ngay>=@DateBegin and Ngay<=@DateEnd";
                ghichu += " Ngày: " + dtpbegin.Value.ToString("dd/MM/yyyy") + " - " + dtpend.Value.ToString("dd/MM/yyyy");
                if (khonvlselected != null)
                {
                    ghichu += Environment.NewLine + " Kho: " + khonvlselected.FullName.ToString();
                    dieukien += " and MaKho = @MaKho";
                    lstpara.Add(new ParameterDefine("@MaKho", khonvlselected.Name.ToString()));
                }

                if (lydoselected != null)
                {
                    ghichu += Environment.NewLine + " Lý do: " + lydoselected.Name.ToString();
                    dieukien += " and LyDo = @LyDo";
                    lstpara.Add(new ParameterDefine("@LyDo", lydoselected.Name));
                }
                if (userselect != null)
                {
                    ghichu += Environment.NewLine + " Người nhập: " + userselect.UsersName.ToString();
                    dieukien += " and UserInsert = @UserInsert";
                    lstpara.Add(new ParameterDefine("@UserInsert", userselect.UsersName));
                }
                if (!string.IsNullOrEmpty(noigiaonhan))
                {
                    ghichu += Environment.NewLine + " Nơi giao nhận: " + noigiaonhan.ToString();
                    dieukien += " and MaGN = @MaGN";
                    lstpara.Add(new ParameterDefine("@MaGN", noigiaonhan));
                }
                switch (loaiNhapXuat)
                {
                    case "NhapKho":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'NhapKho')";
                        break;
                    case "NhapKhoAll":
                        dieukien += " and STTCT >=0";
                        break;
                    case "XuatKho":
                        dieukien += " and STTCT <0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'XuatKho')";
                        break;
                    case "XuatKhoAll":
                        dieukien += " and STTCT <0";
                        break;
                    case "XuatGhiNo":
                        dieukien += " and STTCT <0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'LyDoNo')";
                        break;
                    case "NhapGhiNo":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'LyDoNo')";
                        break;
                    case "XuatHuyTra":
                        dieukien += " and STTCT <0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'XuatHuyTra')";
                        break;
                    case "NhapHuyTra":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'XuatHuyTra')";
                        break;
                    case "NhapKiemKe":
                        dieukien += " and STTCT >=0 and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'NhapKiemKe')";
                        break;
                    case "ChuyenKho":
                        dieukien += "  and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'ChuyenKho')";
                        break;
                    default:
                        dieukien += " and LyDo in (Select LyDo from NvlNhapXuat_LyDo where Tag = 'LyDoNo')";
                        break;
                }

                lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
                lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
            }
            else
            {
                ghichu += Environment.NewLine + "Số chứng từ: " + sochungtu;
                dieukien += " Where Serial = @Serial";
                lstpara.Add(new ParameterDefine("@Serial", sochungtu));
                lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
            }

            string sqlSearch = "";


            //string[] arrshow = new string[] { "MaHang", "TenHang", "SerialCT", "SLNhap", "SLXuat", "DVT", "LyDo", "TenSP", "MaKien", "DauTuan", "SoLo", "TenGN", "SoXe", "GhiChu", "DonGia", "NgayHetHan", "SerialLink", "KhachHang_XuatXu", "UserInsert", "SerialKHDH", "SerialDN", "NguoiDeNghi", "TenLienKet", "NhaMay", "NgayInsert" };
            bool checkshow = false;
            //Lưu ý, phải order by theo số SerialCT, không thôi xtrareport nó mearge Serial bị sai
            sqlSearch = string.Format(@"use NVLDB select nxitem.SerialCT,nh.TenNhom,nxitem.MaHang,(nxitem.SLNhap+nxitem.SLXuat) as SoLuong,nx.LyDo,nxitem.DonGia,(nxitem.SLNhap+nxitem.SLXuat)*nxitem.DonGia as ThanhTien
                  ,gn.TenGN,hh.TenHang,hh.DVT,nx.Ngay
                   ,mk.TenKho
                    from (SELECT  [Serial] ,[MaKho]
                          ,[MaGN],[LyDo],[PONumber],[Ngay],[NguoiDN],[NhaMay]
                      FROM [NvlNhapXuat]  {0} ) nx
                    inner join
                    (select MaHang,SerialCT,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,DonGia from NvlNhapXuatItem {1}  group by  MaHang,SerialCT,DonGia) nxitem 
                    on nx.Serial=nxitem.SerialCT
                    inner JOIN View_NoiGN gn on gn.MaGN = nx.MaGN
                    inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                    inner join dbo.NvlHangHoa hh on nxitem.MaHang=hh.MaHang 	
                    inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom order by nxitem.SerialCT
                     ", dieukien, "");

            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.sql = sqlSearch;
            getDataFromSql.reportname = "XtraRp_BangKeTongHop";
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            DataTable dttmp = new DataTable();
            dttmp.Columns.Add("GhiChu", typeof(string));
            DataRow row = dttmp.NewRow();
            row["GhiChu"] = ghichu;
            dttmp.Rows.Add(row);
            getDataFromSql.dtparameter = prs.ConvertDataTableToJson(dttmp);
            dttmp.Dispose();
            await ModelAdmin.mainLayout.showreportpdfAsync(getDataFromSql);

            //XtraRp_BangKeTongHop xtraRp_PhieuNhapKho = new XtraRp_BangKeTongHop();
            //xtraRp_PhieuNhapKho.setGhiChu(ghichu);
            //xtraRp_PhieuNhapKho.DataSource = lstrp;
            //await ModelAdmin.mainLayout.showreportAsync(xtraRp_PhieuNhapKho);


            //CallAPI callAPI = new CallAPI();
            //try
            //{

            //    lstrp.Clear();
            //    PanelVisible = true;
            //    string json = await callAPI.ExcuteQueryEncryptAsync(sqlSearch, lstpara);
            //    if (json != "")
            //    {
            //        var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemShow>>(json);

            //        if (query.Count == 0)
            //        {
            //            ToastService.Notify(new ToastMessage(ToastType.Danger, "Không tìm thấy dữ liệu"));
            //            return;
            //        }
            //        lstrp.AddRange(query);
            //        XtraRp_BangKeTongHop xtraRp_PhieuNhapKho = new XtraRp_BangKeTongHop();
            //        xtraRp_PhieuNhapKho.setGhiChu(ghichu);
            //        xtraRp_PhieuNhapKho.DataSource = lstrp;
            //        await ModelAdmin.mainLayout.showreportAsync(xtraRp_PhieuNhapKho);
            //        //query.Clear();
            //        // await GotoMainForm.InvokeAsync();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
            //}
            //finally
            //{
            //    PanelVisible = false;

            //    StateHasChanged();
            //}
        }
    }
}
