using BlazorBootstrap;
using Blazored.Modal;
using Blazored.Modal.Services;
using DevExpress.Blazor;
using DevExpress.CodeParser;
using DevExpress.XtraReports.UI;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;

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
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

using System.Runtime.CompilerServices;

using static NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Urc_DonHang_AddKeHoach;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;






namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Page_KeHoachMuaHangMaster
    {

        [Inject] PreloadService PreloadService { get; set; }
        [Inject] BrowserService browserService { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] ToastService ToastService { get; set; } = default!;
        PhanQuyenAccess phanQuyenAccess { get; set; } = new PhanQuyenAccess();
        //[CascadingParameter] IModalService Modal { get; set; } = default!;
        DateTime? dtpbegin { get; set; } = DateTime.Now.AddMonths(-2);
        DateTime? dtpend { get; set; } = DateTime.Now;


        IGrid Grid { get; set; }
        List<KeHoachMuaHang_Show> lstDonDatHangSearchShow { get; set; } = new List<KeHoachMuaHang_Show>();
        KeHoachMuaHang_Show kehoachshowcrr { get; set; }
        List<Users> lstuser { get; set; }
        List<DataDropDownList> lstnhamay = new List<DataDropDownList>();
        List<DataDropDownList> lsttinhtrang = new List<DataDropDownList>();
        List<Users> lstnguoidenghi { get; set; }
        List<Users> lstnguoiduyet { get; set; }
        CustomRoot customRoot { get; set; } = new CustomRoot();
        DataDropDownList? NhaMaySelected { get; set; }
        DataDropDownList? TinhTrangSelected { get; set; }
        Users? nguoiduyet { get; set; }
        Users? nguoidenghi { get; set; }
       
        public string LoaiKeHoach { get; set; }
       bool Ismobile { get; set; }
        public string idgrid { get; set; }
                   
        public class KeHoachMuaHang_Show
        {
            public KeHoachMuaHang_Show()
            {
                isChanged = false;
            }
            public int Serial { get; set; }

            public string MaDN { get; set; }
            public string NguoiDN { get; set; }
            public string LyDo { get; set; }
            public Nullable<System.DateTime> NgayDN { get; set; }
            public string PhongBan { get; set; }

            public string NhaMay { get; set; }
            public string LoaiKeHoach { get; set; }

            public string NoiDungDeNghi { get; set; }
            public Nullable<System.DateTime> NgayMax { get; set; }
            public Nullable<double> TyLe { get; set; }
            public string NoiDung { get; set; }
            public string dieukiennhomhang { get; set; }
            public string GhiChu { get; set; }
            public string KhuVuc { get; set; }
            public string TenKhuVuc { get; set; }
            public string TextDuyet { get; set; }
            public string UserInsert { get; set; }
            public Nullable<System.DateTime> NgayInsert { get; set; }

            public string TenSP { get; set; }
            public string PathDuyet
            {
                get;
                set;

            }


            public string PathImgTao { get; set; } = "images/user.png";
            public List<FileHoSoGroup> lstfilehoso { get; set; }
            public ObservableCollection<NvlKeHoachMuaHangItemShow> lstitem { get; set; }
            public List<NvlKyDuyetShow> lstkyduyet { get; set; }
            public List<NvlNhapXuatItemTemTK> lsttemtonkho { get; set; }
            public void setlstfilehoso(List<FileHoSoGroup> lst)
            {
                lstfilehoso = lst;
            }
            public  List<DonHangItem>lstdathang{get;set;}
            private string _nguoiDuyet { get; set; }
            private string _nguoiKiem { get; set; }
            private string _daDuyet { get; set; }
            private string _daKiem { get; set; }
            public string NguoiDuyet
            {
                get { return _nguoiDuyet; }
                set
                {
                    _nguoiDuyet = value;

                }
            }
            public string NguoiKiem
            {
                get { return _nguoiKiem; }
                set
                {
                    _nguoiKiem = value;

                }
            }
            public string DaDuyet
            {
                get { return _daDuyet; }
                set
                {
                    _daDuyet = value;


                }
            }
            public string DaKiem
            {
                get { return _daKiem; }
                set
                {
                    _daKiem = value;

                }
            }
            private string _tinhTrang { get; set; }
            public string TinhTrang
            {
                get { return _tinhTrang; }
                set
                {
                    _tinhTrang = value;
                    if (_tinhTrang == "Đã duyệt")
                        PathDuyet = IconImg.CheckMark;
                    else
                        PathDuyet = IconImg.NotCheck;


                }
            }
            public string BoPhanMuaHang { get; set; }
            private bool _ischanged { get; set; } = false;
            public bool isChanged
            {
                get { return _ischanged; }
                set
                {

                    _ischanged = value;

                }
            }
            public KeHoachMuaHang_Show CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<KeHoachMuaHang_Show>(json);
            }
            public int? CountTong { get; set; }
            public int? CountDuyet { get; set; }
            public string ShowTextDuyet { get; set; }
            public bool EnableButtonDuyet { get; set; } = true;
            private string? _khongduyet { get; set; }
            public string? Msg { get; set; }
            public string? KhongDuyet
            {
                get { return _khongduyet; }
                set
                {
                    _khongduyet = value;
                    if(!string.IsNullOrEmpty(_khongduyet))
                    {
                        TextDuyet = "Không Duyệt";
                        
                    }
                }
            }
            //Thêm vào do item
           
            public string MaSP { get; set; }
            public string CongDoan { get; set; }
            public double? SLCongDoan { get; set; }
            public string KeyGroup { get; set; }
        }


        public class NvlKeHoachMuaHangItemShow : INotifyPropertyChanged
        {

            public int Serial { get; set; }
            public bool chk { get; set; }
            public Nullable<int> SerialDN { get; set; }

            [Required(ErrorMessage = "Mã hàng không được để trống")]
            [StringLength(100, MinimumLength = 1, ErrorMessage = "Mã hàng từ 1 đến 100 ký tự")]
            public string? MaHang { get; set; }

            public string TenSP
            {
                get; set;
            }
            [Required(ErrorMessage = "Nhập số lượng")]
            [Range(0.000001, 10000000000, ErrorMessage = "Số lượng phải lớn hơn 0.000001")]
            public double? SoLuong { get; set; }
            public Nullable<double> SLTheoDoi { get; set; }
            public double? DonGia { get; set; }
            public string DVT { get; set; }
            public string TenHang { get; set; }
            public bool VisibleSL { get; set; }
            public Nullable<int> SerialLink
            {
                get { return _seriallink; }
                set
                {
                    _seriallink = value;
                    if (_seriallink == null || _seriallink == 0)
                    {

                        VisibleSL = false;
                    }
                    else
                        VisibleSL = true;
                }
            }
            private Nullable<int> _seriallink { get; set; }
            public string TableName { get; set; }

            public Nullable<double> SLDatHang { get; set; }
            public Nullable<double> SLSuDung {get;set;}
            public Nullable<double> TyLe
            {
                get {

                    return ((SoLuong > 0) ? (SoLuong - (SLTheoDoi+SLHuy)) / SoLuong : 0); }
            }
            public Nullable<int> STT { get; set; }
            public Nullable<int> VAT { get; set; }
            public string VATShow
            {
                get
                {
                    if (VAT == 1)
                        return "Đã bao gồm VAT";
                    if (VAT == 0)
                        return "Chưa bao gồm VAT";
                    return "";
                }
            }
            public string GhiChu { get; set; }
            public string? GroupName {
                get
                {
                    string textgroup = "";
                    if (SerialLink == null)
                        textgroup = "0";
                    else
                    {
                        if (TableName == "NvlKeHoachMuaHangItem")
                        {
                            textgroup = "0";
                        }
                        if (TableName == "NvlKeHoachSP")
                            textgroup = SerialLink.ToString();
                    }
                    return (TenSP + textgroup);
                }
            }
            private string _huydathang { get; set; }
            public string HuyDatHang
            {
                get { return _huydathang; }
                set
                {
                    _huydathang = value;
                    NotifyPropertyChanged("GhiChu");
                    NotifyPropertyChanged("SLTheoDoi");
                    NotifyPropertyChanged("SLHuy");
                    NotifyPropertyChanged("TyLe");

                }
            }
            public string? TenLienKet { get; set; }
            public string? MaSP { get; set; }
            public Nullable<int> SoLuongSP { get; set; }
            public uint? Color { get { return _color; } set { _color = value; Colorhex = StaticClass.UIntToHtmlColor(_color);Colortext = StaticClass.GetContrastColor(Colorhex); } }
            private uint? _color { get; set; }
            public string Colorhex
            {
                get; set;

            }
           
            public string Colortext { get; set; }
            public string TextKiem { get; set; } = "";
            public string TextDuyet { get; set; } = "";
            public string TenMau { get; set; }
            public Nullable<System.DateTime> NgayInsert { get; set; }
            public Nullable<System.DateTime> NgayDKNhapKho { get; set; }
            public string UserInsert { get; set; }
            public string ArticleNumber { get; set; }
            public string PhanLoai { get; set; }
            public Nullable<double> SLTon { get; set; }

            private string _duyetItemMsg { get; set; }
            public bool ShowMsg { get; set; }
            public Nullable<double> SLHuy
            {
                get;
                set;
            } = 0;
            public string DuyetItemMsg
            {
                get { return _duyetItemMsg; }
                set
                {
                    _duyetItemMsg = value;
                    if (_duyetItemMsg == null)
                        return;
                    if (_duyetItemMsg == "Đồng ý")
                        ForeGroundMsg = "#008000";
                    else
                        ForeGroundMsg = "#FF0000";

                    if (_duyetItemMsg != null)
                        ShowMsg = true;

                }
            }
            double? _thanhtien { get; set; }
            public Nullable<double> ThanhTien
            {
                get;set;


            }
            public NvlKeHoachMuaHangItemShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlKeHoachMuaHangItemShow>(json);
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private string _err { get; set; }
            public string Err
            {
                get
                {
                    return _err;
                }
                set
                {
                    _err = value;
                    NotifyPropertyChanged("Err");
                }
            }
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            public List<NvlKyDuyetItemShow> lstduyetitem { get; set; }

            public string ForeGroundMsg { get; set; }
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
            public string KeyGroup { get; set; }
            public string CongDoan { get; set; }
            public string TenDinhMuc { get; set; }
        }

        public class NvlKeHoachMuaHangItemTotalShow
        {

            
            public string? MaHang { get; set; }

           
            public double? SoLuong { get; set; }
            public Nullable<double> SLTheoDoi { get; set; }
            public double? XuongNo { get; set; }
            public double? KhoNo { get; set; }
            public string DVT { get; set; }
            public string TenHang { get; set; }
            public bool VisibleSL { get; set; }
           
        }


        public class NvlNhapXuatItemTemTK
        {
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string DVT { get; set; }
            public int SerialLink { get; set; }
            public string ViTri { get; set; }
            public decimal SLTon { get; set; }
            public DateTime Ngay { get; set; }
            public string TenNCC { get; set; }
            public decimal DonGia { get; set; }
            public decimal SLDeNghi { get; set; }
           
        }
        string bophanmuahang = "Bộ phận mua hàng";
        protected override async Task OnInitializedAsync()
        {
            try
            {
                PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");

                await Task.Delay(100);
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
                    Ismobile = false;
                heightgrid = string.Format("{0}px", height);
                Console.WriteLine("Test đến đây");
                List<DataDropDownList> lst = await Model.ModelData.GetDataDropDownListsAsync();
                if (lst != null)
                {
                    //lstnhamay = lst.Where(p => p.TypeName == "NhaMay_NVL").ToList();
                    lsttinhtrang = lst.Where(p => p.TypeName == "KyDuyetStatus").ToList();
                }
                lstuser = await Model.ModelData.Getlstusers();
                Console.WriteLine("Test đến đây 2");
                if (ModelAdmin.lstmenuitems != null)
                {
                    if (LoaiKeHoach == null)
                    {
                        MenuItem query = ModelAdmin.lstmenuitems.FirstOrDefault(p => p.NameItem.Equals(ModelAdmin.pageclickcurrent));
                        if (query != null)
                        {
                            if (query.Tag != null)
                                LoaiKeHoach = query.Tag.ToString();
                        }
                    }
                }
                Visilethemtukehoach = true;
                if (LoaiKeHoach.Contains("MuaHang"))
                {
                    texttaomoi = "TẠO ĐỀ NGHỊ";
                    //Visilethemtukehoach = true;
                    bophanmuahang="Bộ phận mua hàng";
                    textshowKeHoachAddItem = "Thêm từ kế hoạch tháng";
                    visiblemuahang = true;
                }
                if(LoaiKeHoach== "DeNghiXuatHang")
                {
                    bophanmuahang = "Kho xuất hàng";
                    textshowKeHoachAddItem = "Thêm từ định mức sản phẩm";
                }
                if(LoaiKeHoach== "DeNghiXuatHangTheoKHSX")
                {
                    texttaomoi = "TẠO ĐỀ NGHỊ";
                    //Visilethemtukehoach = true;
                    bophanmuahang = "Kho xuất hàng";
                    textshowKeHoachAddItem = "Thêm từ kế hoạch sản xuất";
                }
               
                await loadvideohuongdanAsync();

                //randomdivhide = prs.RandomString(10);
                lstnguoidenghi = lstuser.ToList();
                lstnguoiduyet = lstuser.ToList();
                // Grid.Data = lstDonDatHangSearchShow;
               
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load form ke hoach master " + ex.Message);
            }
            finally
            {
                PreloadService.Hide();
            }
            //return base.OnInitialized();
        }
        bool loadvideo = false;
        bool firstload = false;
        private async Task loadvideohuongdanAsync()
        {
            CallAPI callAPI = new CallAPI();
            string sql = @"use [NVLDB]
            SELECT  [Serial],[NoiDung],[LinkVideo],[TypeVideo]
              FROM [dbo].[NvlVideoHuongDan] where [TypeVideo]='NvlDeNghi'";

            string json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
            if (json != "")
            {

                var query = JsonConvert.DeserializeObject<List<VideoHuongDan>>(json);

               videoHuongDans.AddRange(query);
                Console.WriteLine(query.Count);
                //await InvokeAsync(StateHasChanged);
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    await Model.ModelData.Getlstphongbankhuvuc();

                    //base.OnAfterRender(firstRender);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }
            if(!firstload)//Load lần đầu để gán biến
            {
                if (lstnguoidenghi != null)
                {

                    TinhTrangSelected = lsttinhtrang.Where(p => p.FullName == "Chưa duyệt").FirstOrDefault();
                    if (LoaiKeHoach == null)
                    {
                        MenuItem query = ModelAdmin.lstmenuitems.FirstOrDefault(p => p.NameItem.Equals(ModelAdmin.pageclickcurrent));
                        if (query != null)
                        {
                            if (query.Tag != null)
                                LoaiKeHoach = query.Tag.ToString();
                        }
                    }
                    if (LoaiKeHoach == null)
                    {
                        LoaiKeHoach = "KyDuyetMuaHang";
                    }
                    if (LoaiKeHoach.Contains("KyDuyet"))
                    {
                        Visileprint = true;
                        Visilechinhsua = false;
                        Visilechinhtruocin = false;
                        Visilechinhtruocin = false;
                        Visilechonnguoiduyet = false;
                        Visiledelete = false;
                        Visilethemchitiet = false;
                        VisbleTaoDeNghi = false;

                        nguoiduyet = lstnguoiduyet.FirstOrDefault(p => p.UsersName.Equals(ModelAdmin.users.UsersName)); ;
                    }
                    else
                    {
                        nguoidenghi = lstnguoidenghi.FirstOrDefault(p => p.UsersName.Equals(ModelAdmin.users.UsersName));
                    }
                    if (nguoidenghi != null)
                    {
                        firstload = true;
                    }
                    if (LoaiKeHoach == "KeHoachMuaHang")
                    {
                        VisbleTaoDeNghi = false;
                        Visilechonnguoiduyet = true;
                    }
                    if (!Ismobile)
                    {
                        if (!LoaiKeHoach.Contains("KyDuyet"))
                        {
                            renderFragmentflowchart = builder =>
                            {
                                builder.OpenComponent<Urc_Stepper_FlowChart>(0);
                                builder.AddAttribute(1, "TypeName", LoaiKeHoach);

                                builder.CloseComponent();
                            };
                        }
                    }


                    firstload = true;
                    if (LoaiKeHoach.Contains("KyDuyet"))
                    {
                        await searchAsync();
                    }
                    StateHasChanged();

                }

            }
            //return base.OnAfterRenderAsync(firstRender);
        }
        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<KeHoachMuaHang_Show> lstmuahang { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetShow> lstkyduyet { get; set; }
        }
        public class CustomRootItem
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NvlKeHoachMuaHangItemShow> lstmuahangitem { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetItemShow> lstkyduyet { get; set; }
        }

        private async Task searchAsyncold()
        {
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = "";
            string dieukienduyet = "";
            if (dtpbegin == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ngày để xem"));
                return;
            }

            if (LoaiKeHoach == "KyDuyetMuaHang")
            {
                dieukien = " where LoaiKeHoach in ('KeHoachMuaHang','DeNghiMuaHang','DeNghiXuatHang','DeNghiXuatHangTheoKHSX')";
            }
            else
                dieukien = " where LoaiKeHoach=@LoaiKeHoach";
            lstpara.Add(new ParameterDefine("@LoaiKeHoach", LoaiKeHoach));
            if (SerialDN != null && SerialDN > 0)
            {
                if (dieukien == "")
                    dieukien = " where Serial=@Serial";
                else
                    dieukien += " and Serial=@Serial";
                lstpara.Add(new ParameterDefine("@Serial", SerialDN.ToString()));
            }
            else
            {
                if (dtpbegin != null)
                {
                    if (dieukien == "")
                        dieukien = " where NgayInsert>=@DateBegin";
                    else
                        dieukien += " and NgayInsert>=@DateBegin";
                    lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));

                }



                if (dtpend != null)
                {
                    if (dieukien == "")
                        dieukien = " where NgayInsert<=@DateEnd";
                    else
                        dieukien += " and NgayInsert<=@DateEnd";
                    lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
                }
                if (NhaMaySelected != null)
                {
                    if (dieukien == "")
                        dieukien = " where NhaMay=@NhaMay";
                    else
                        dieukien += " and NhaMay=@NhaMay";
                    lstpara.Add(new ParameterDefine("@NhaMay", NhaMaySelected.Name));
                }
                if (nguoidenghi != null)
                {
                    if (dieukien == "")
                        dieukien = " where NguoiDN=@NguoiDN";
                    else
                        dieukien += " and NguoiDN=@NguoiDN";
                    lstpara.Add(new ParameterDefine("@NguoiDN", nguoidenghi.TenUser.ToString()));
                }
            }
            if (nguoiduyet != null)
            {

                lstpara.Add(new ParameterDefine("@UserDuyet", nguoiduyet.UsersName));
                if (dieukienduyet == "")
                {
                    dieukienduyet = " where tbl.Serial in (select [SerialLink] from @tblkyduyet)";
                }
                else
                    dieukienduyet += " and tbl.Serial in (select [SerialLink] from @tblkyduyet)";
            }
            else
                lstpara.Add(new ParameterDefine("@UserDuyet", ""));
            if (TinhTrangSelected != null)
            {
                string duyet = TinhTrangSelected.Name.ToString();
                if (duyet == "Chưa duyệt")
                {
                    if (dieukienduyet == "")
                        dieukienduyet = @" where (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)>0 or CountTong is null)";
                    else
                        dieukienduyet += @" and (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)>0 or CountTong is null)";
                    //GrvdetailGrid.Columns["DonGia"].Visible = true;
                    //GrvdetailGrid.Columns["DonGiaDat"].Visible = false;
                }
                if (duyet == "Đã duyệt")
                {
                    if (dieukienduyet == "")
                        dieukienduyet = @" where (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)<=0 and CountTong is not null)";
                    else
                        dieukienduyet += @" and (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)<=0 and CountTong is not null)";


                }

            }

            if (dieukien == "")
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ít nhất 1 điều kiện để tìm kiếm"));
                // msgBox.Show("Vui lòng chọn ít nhất 1 điều kiện để tìm kiếm", IconMsg.iconerror);
                return;
            }

            //grvKeHoach.ItemsSource = null;

            string sql = string.Format(@" USE [NVLDB]  declare @tblkyduyet as Table([Serial] int,[SerialLink] int,[TableName]  nvarchar(100)
                              ,[UserYeuCau]  nvarchar(100),[UserDuyet]  nvarchar(100),[LoaiDuyet]  nvarchar(100),[DaDuyet]  nvarchar(100)
                              ,[GhiChu]  nvarchar(500),countItemDuyet int)

                                declare @tbldonhang as Table(Serial int,[NguoiDN] nvarchar(100),[LyDo] nvarchar(100),[KhuVuc] nvarchar(100)
                                                         ,[NgayDN] datetime,[PhongBan] nvarchar(100),[NhaMay] nvarchar(100),[NoiDung] nvarchar(500)
                                                         ,[GhiChu] nvarchar(200),[UserInsert] nvarchar(100),[LoaiKeHoach] nvarchar(100),[NgayInsert] datetime
                                                         ,[NoiDungDeNghi] nvarchar(500),TenKhuVuc nvarchar(100),BoPhanMuaHang nvarchar(100))
                              insert into @tbldonhang(Serial,[NguoiDN] ,[LyDo],[KhuVuc],[NgayDN],[PhongBan],[NhaMay],[NoiDung],[GhiChu],[UserInsert]
							                                                        ,[LoaiKeHoach],[NgayInsert],[NoiDungDeNghi],TenKhuVuc,BoPhanMuaHang)
                                                         SELECT [ddh].Serial,[NguoiDN] ,[LyDo],[KhuVuc],[NgayDN],[PhongBan],[NhaMay],[NoiDung],[GhiChu],[UserInsert]
							                                                        ,[LoaiKeHoach],[NgayInsert],[NoiDungDeNghi],kv.TenKhuVuc,BoPhanMuaHang
                                                         from (Select * from NvlKehoachMuaHang   
							                             {0}
							                            ) as ddh  left join dbo.NvlKhuVuc kv on ddh.KhuVuc=kv.MaKhuVuc
                            declare @minserial int
                            declare @maxserial int
                            select @minserial=min(Serial),@maxserial=max(Serial) from @tbldonhang

                           if(@UserDuyet<>'')
                            begin
                             insert into @tblkyduyet([Serial],[SerialLink],[TableName],[UserYeuCau],[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],countItemDuyet)
                                                        select [Serial],[SerialLink],[TableName],[UserYeuCau],qryduyet.[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],isnull(countItemDuyet,0) as countItemDuyet
                                                        from (select * from NvlKyDuyet where UserDuyet=@UserDuyet and TableName='NvlKehoachMuaHang' and SerialLink>=@minserial and SerialLink<=@maxserial) as qryduyet
														left join (SELECT [SerialLinkMaster],[UserDuyet],count(*) as countItemDuyet
   
													FROM [NvlKyDuyetItem] where UserDuyet=@UserDuyet and TableName='NvlKehoachMuaHang' and SerialLinkMaster>=@minserial and SerialLinkMaster<=@maxserial
													group by [SerialLinkMaster],[UserDuyet]) as qryitem on qryduyet.SerialLink=qryitem.SerialLinkMaster and qryduyet.UserDuyet=qryitem.UserDuyet


                            end
                            else
                            begin
	                             insert into @tblkyduyet([Serial],[SerialLink],[TableName],[UserYeuCau],[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],countItemDuyet)
                                                        select [Serial],[SerialLink],[TableName],[UserYeuCau],qryduyet.[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],isnull(countItemDuyet,0) as countItemDuyet
                                                        from (select * from NvlKyDuyet where  TableName='NvlKehoachMuaHang' and SerialLink>=@minserial and SerialLink<=@maxserial) as qryduyet
														left join (SELECT [SerialLinkMaster],[UserDuyet],count(*) as countItemDuyet
   
													FROM [NvlKyDuyetItem] where TableName='NvlKehoachMuaHang' and SerialLinkMaster>=@minserial and SerialLinkMaster<=@maxserial
													group by [SerialLinkMaster],[UserDuyet]) as qryitem on qryduyet.SerialLink=qryitem.SerialLinkMaster and qryduyet.UserDuyet=qryitem.UserDuyet

                            end
		                            select tbl.*,isnull(CountTong,0) as CountTong,isnull(qrykyduyetitem.SLItemDuyet,0) as CountDuyet,'{2}'+isnull(usr.PathImg,'UserImage/user.png') as PathImgTao from @tbldonhang tbl 
		                            left join (select SerialDN,SLDong as CountTong from
		                            (SELECT SerialDN,count(*) as SLDong
		                              FROM [dbo].[NvlKeHoachMuaHangItem] khitem where SerialDN>=@minserial and SerialDN<=@maxserial 
		                              group by SerialDN) as qryitem) as qryallitem on tbl.Serial=qryallitem.SerialDN
		                              left join (SELECT SerialLinkMaster,count([SerialLinkItem]) as SLItemDuyet
		                            FROM [NvlKyDuyetItem] where TableName='NvlKehoachMuaHang' and [LoaiDuyet]=N'Duyệt' and SerialLinkMaster>=@minserial and SerialLinkMaster<=@maxserial
		                            group by SerialLinkMaster) as qrykyduyetitem
		                            on tbl.Serial=qrykyduyetitem.SerialLinkMaster 
		                            inner join [DBMaster].[dbo].[Users] usr on tbl.UserInsert=usr.UsersName 
		                            {1}
		                            
                                    if(@UserDuyet<>'')
										select [Serial],[SerialLink],[TableName],[UserYeuCau],[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu] from NvlKyDuyet where TableName='NvlKehoachMuaHang' and SerialLink in (select [SerialLink] from @tblkyduyet )
								    else
										select * from @tblkyduyet

                            ", dieukien, dieukienduyet, ModelAdmin.pathurlfilepublic);

            // ShowProgress.ShowAwait();
            try
            {

                lstDonDatHangSearchShow.Clear();
                //lstDonDatHangSearchShow.TrimExcess();
                // lstDonDatHangSearchShow.Clear();
                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {

                    customRoot = JsonConvert.DeserializeObject<CustomRoot>(json);
                    if (customRoot != null)
                    {
                        lstDonDatHangSearchShow.AddRange(customRoot.lstmuahang);
                        if (customRoot.lstmuahang.Any())
                        {
                            await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
                        }
                    }

                }

                ////Xử lý load ảnh

                PanelVisible = false;


            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
                //msgBox.Show("Lỗi:" + ex.Message, IconMsg.iconerror);
                PanelVisible = false;
            }
            finally
            {

                PanelVisible = false;
                Grid.Reload();
                StateHasChanged();
                //ShowProgress.CloseAwait();
            }
        }
        private async Task searchAsync()
        {
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = "";
            string dieukienduyet = "";
            if (dtpbegin == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ngày để xem"));
                return;
            }

            if (LoaiKeHoach == "KyDuyetMuaHang"||LoaiKeHoach=="KyDuyetXuatKho")
            {
                dieukien = " where LoaiKeHoach in ('KeHoachMuaHang','DeNghiMuaHang','DeNghiXuatHang','DeNghiXuatHangTheoKHSX')";
            }
            else
                dieukien = " where LoaiKeHoach=@LoaiKeHoach";
            lstpara.Add(new ParameterDefine("@LoaiKeHoach", LoaiKeHoach));
            if (SerialDN != null && SerialDN > 0)
            {
                if (dieukien == "")
                    dieukien = " where Serial=@Serial";
                else
                    dieukien += " and Serial=@Serial";
                lstpara.Add(new ParameterDefine("@Serial", SerialDN.ToString()));
            }
            else
            {
                if (dtpbegin != null)
                {
                    if (dieukien == "")
                        dieukien = " where NgayInsert>=@DateBegin";
                    else
                        dieukien += " and NgayInsert>=@DateBegin";
                    lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));

                }



                if (dtpend != null)
                {
                    if (dieukien == "")
                        dieukien = " where NgayInsert<=@DateEnd";
                    else
                        dieukien += " and NgayInsert<=@DateEnd";
                    lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
                }
                if (NhaMaySelected != null)
                {
                    if (dieukien == "")
                        dieukien = " where NhaMay=@NhaMay";
                    else
                        dieukien += " and NhaMay=@NhaMay";
                    lstpara.Add(new ParameterDefine("@NhaMay", NhaMaySelected.Name));
                }
                if (nguoidenghi != null)
                {
                    if (dieukien == "")
                        dieukien = " where NguoiDN=@NguoiDN";
                    else
                        dieukien += " and NguoiDN=@NguoiDN";
                    lstpara.Add(new ParameterDefine("@NguoiDN", nguoidenghi.TenUser.ToString()));
                }
            }
            if (nguoiduyet != null)
            {

                lstpara.Add(new ParameterDefine("@UserDuyet", nguoiduyet.UsersName));
               
            }
            else
                lstpara.Add(new ParameterDefine("@UserDuyet", ""));
            if (TinhTrangSelected != null)
            {
                string duyet = TinhTrangSelected.Name.ToString();
                lstpara.Add(new ParameterDefine("@TinhTrang", duyet));
               

            }

            if (dieukien == "")
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ít nhất 1 điều kiện để tìm kiếm"));
                // msgBox.Show("Vui lòng chọn ít nhất 1 điều kiện để tìm kiếm", IconMsg.iconerror);
                return;
            }

            //grvKeHoach.ItemsSource = null;

           
            string sql = string.Format(@"
	--Thuật toán: Nếu có truyền vào User cụ thể, thì hiển thị đúng những đề nghị mà User này chưa kiểm tra hoặc chưa duyệt, Cho dù đề nghị đó đã được duyệt chính thức rồi, nhưng nếu user này chưa kiểm tra thì vẫn hiện lên cho user này như thường
	--Trường hợp để trống User thì lấy đúng theo tình trạng của đề nghị, tính theo User Duyệt
       Use NVLDB
        DECLARE @tblkyduyet AS TABLE (
		[Serial] INT
		,[SerialLink] INT
		,[TableName] NVARCHAR(100)
		,[UserYeuCau] NVARCHAR(100)
		,[UserDuyet] NVARCHAR(100)
		,[LoaiDuyet] NVARCHAR(100)
		,[DaDuyet] NVARCHAR(100)
		,[GhiChu] NVARCHAR(500)
		,countItemDuyet INT,KhongDuyet nvarchar(100)
		)
	DECLARE @tbldonhang AS TABLE (
		Serial INT
		,[NguoiDN] NVARCHAR(100)
		,[LyDo] NVARCHAR(100)
		,[KhuVuc] NVARCHAR(100)
		,[NgayDN] DATETIME
		,[PhongBan] NVARCHAR(100)
		,[NhaMay] NVARCHAR(100)
		,[NoiDung] NVARCHAR(500)
		,[GhiChu] NVARCHAR(200)
		,[UserInsert] NVARCHAR(100)
		,[LoaiKeHoach] NVARCHAR(100)
		,[NgayInsert] DATETIME
		,[NoiDungDeNghi] NVARCHAR(500)
		,TenKhuVuc NVARCHAR(100)
		,BoPhanMuaHang NVARCHAR(100)
		)

	INSERT INTO @tbldonhang (
		Serial
		,[NguoiDN]
		,[LyDo]
		,[KhuVuc]
		,[NgayDN]
		,[PhongBan]
		,[NhaMay]
		,[NoiDung]
		,[GhiChu]
		,[UserInsert]
		,[LoaiKeHoach]
		,[NgayInsert]
		,[NoiDungDeNghi]
		,TenKhuVuc
		,BoPhanMuaHang
		)
	SELECT [ddh].Serial
		,[NguoiDN]
		,[LyDo]
		,[KhuVuc]
		,[NgayDN]
		,[PhongBan]
		,[NhaMay]
		,[NoiDung]
		,[GhiChu]
		,[UserInsert]
		,[LoaiKeHoach]
		,[NgayInsert]
		,[NoiDungDeNghi]
		,kv.TenKhuVuc
		,BoPhanMuaHang
	FROM (
		SELECT *
		FROM NvlKehoachMuaHang
		{0}
		) AS ddh
	LEFT JOIN dbo.NvlKhuVuc kv ON ddh.KhuVuc = kv.MaKhuVuc

	DECLARE @minserial INT
	DECLARE @maxserial INT

	SELECT @minserial = min(Serial)
		,@maxserial = max(Serial)
	FROM @tbldonhang

	IF (@TinhTrang = N'Chưa duyệt')
	BEGIN
		IF (@UserDuyet <> '')
		BEGIN
			INSERT INTO @tblkyduyet (
				[Serial]
				,[SerialLink]
				,[TableName]
				,[UserYeuCau]
				,[UserDuyet]
				,[LoaiDuyet]
				,[DaDuyet]
				,[GhiChu]
				,countItemDuyet,KhongDuyet
				)
			SELECT [Serial]
				,[SerialLink]
				,[TableName]
				,[UserYeuCau]
				,qryduyet.[UserDuyet]
				,[LoaiDuyet]
				,[DaDuyet]
				,[GhiChu]
				,isnull(countItemDuyet, 0) AS countItemDuyet,KhongDuyet
			FROM (
				SELECT *
				FROM NvlKyDuyet
				WHERE UserDuyet = @UserDuyet
					AND TableName = 'NvlKehoachMuaHang'
					AND SerialLink >= @minserial
					AND SerialLink <= @maxserial
				) AS qryduyet
			LEFT JOIN (
				SELECT [SerialLinkMaster]
					,[UserDuyet]
					,count(*) AS countItemDuyet
				FROM [NvlKyDuyetItem]
				WHERE UserDuyet = @UserDuyet
					AND TableName = 'NvlKehoachMuaHang'
					AND SerialLinkMaster >= @minserial
					AND SerialLinkMaster <= @maxserial
				GROUP BY [SerialLinkMaster]
					,[UserDuyet]
				) AS qryitem ON qryduyet.SerialLink = qryitem.SerialLinkMaster
				AND qryduyet.UserDuyet = qryitem.UserDuyet

			--Có User Duyệt (tức là hiển thị tất cả những đề nghị người này cần kiểm tra hoặc duyệt, bất chấp đề nghị đã được người khác duyệt
			SELECT tbl.*
				,isnull(CountTong, 0) AS CountTong
				,isnull(qrykyduyetitem.SLItemDuyet, 0) AS CountDuyet
				,'{2}' + isnull(usr.PathImg, 'UserImage/user.png') AS PathImgTao
			FROM @tbldonhang tbl
			LEFT JOIN (
				SELECT SerialDN
					,SLDong AS CountTong
				FROM (
					SELECT SerialDN
						,count(*) AS SLDong
					FROM [dbo].[NvlKeHoachMuaHangItem] khitem
					WHERE SerialDN >= @minserial
						AND SerialDN <= @maxserial
					GROUP BY SerialDN
					) AS qryitem
				) AS qryallitem ON tbl.Serial = qryallitem.SerialDN
			LEFT JOIN (
				SELECT SerialLinkMaster
					,count([SerialLinkItem]) AS SLItemDuyet
				FROM [NvlKyDuyetItem]
				WHERE TableName = 'NvlKehoachMuaHang'
					AND [LoaiDuyet] = N'Duyệt'
					AND SerialLinkMaster >= @minserial
					AND SerialLinkMaster <= @maxserial
				GROUP BY SerialLinkMaster
				) AS qrykyduyetitem ON tbl.Serial = qrykyduyetitem.SerialLinkMaster
			INNER JOIN [DBMaster].[dbo].[Users] usr ON tbl.UserInsert = usr.UsersName
			INNER JOIN (
				SELECT [SerialLink]
					,max(countItemDuyet) AS countItemDuyet
				FROM @tblkyduyet
				GROUP BY SerialLink
				) AS qrydakyduyet ON tbl.Serial = qrydakyduyet.SerialLink
				AND (
					isnull(CountTong, 0) - isnull(qrydakyduyet.countItemDuyet, 0) > 0
					OR CountTong IS NULL
					) --Chỉ cần so sánh với những đề nghị mà người này được yêu cầu duyệt, nhưng chưa xác nhận

			SELECT [Serial]
				,[SerialLink]
				,[TableName]
				,[UserYeuCau]
				,[UserDuyet]
				,[LoaiDuyet]
				,[DaDuyet]
				,[GhiChu]
			FROM NvlKyDuyet
			WHERE TableName = 'NvlKehoachMuaHang'
				AND SerialLink IN (
					SELECT [SerialLink]
					FROM @tblkyduyet
					)
		END
		ELSE --Nếu User BẰng '' thì lấy đúng tình trạng của đề nghị theo tình trạnh đã duyệt hay chưa
		BEGIN
			INSERT INTO @tblkyduyet (
				[Serial]
				,[SerialLink]
				,[TableName]
				,[UserYeuCau]
				,[UserDuyet]
				,[LoaiDuyet]
				,[DaDuyet]
				,[GhiChu]
				,countItemDuyet,KhongDuyet
				)
			SELECT [Serial]
				,[SerialLink]
				,[TableName]
				,[UserYeuCau]
				,qryduyet.[UserDuyet]
				,[LoaiDuyet]
				,[DaDuyet]
				,[GhiChu]
				,isnull(countItemDuyet, 0) AS countItemDuyet,KhongDuyet
			FROM (
				SELECT *
				FROM NvlKyDuyet
				WHERE TableName = 'NvlKehoachMuaHang'
					AND SerialLink >= @minserial
					AND SerialLink <= @maxserial
				) AS qryduyet
			LEFT JOIN (
				SELECT [SerialLinkMaster]
					,[UserDuyet]
					,count(*) AS countItemDuyet
				FROM [NvlKyDuyetItem]
				WHERE TableName = 'NvlKehoachMuaHang'
					AND SerialLinkMaster >= @minserial
					AND SerialLinkMaster <= @maxserial
				GROUP BY [SerialLinkMaster]
					,[UserDuyet]
				) AS qryitem ON qryduyet.SerialLink = qryitem.SerialLinkMaster
				AND qryduyet.UserDuyet = qryitem.UserDuyet

			SELECT tbl.*
				,isnull(CountTong, 0) AS CountTong
				,isnull(qrykyduyetitem.SLItemDuyet, 0) AS CountDuyet
				,'{2}' + isnull(usr.PathImg, 'UserImage/user.png') AS PathImgTao
			FROM @tbldonhang tbl
			LEFT JOIN (
				SELECT SerialDN
					,SLDong AS CountTong
				FROM (
					SELECT SerialDN
						,count(*) AS SLDong
					FROM [dbo].[NvlKeHoachMuaHangItem] khitem
					WHERE SerialDN >= @minserial
						AND SerialDN <= @maxserial
					GROUP BY SerialDN
					) AS qryitem
				) AS qryallitem ON tbl.Serial = qryallitem.SerialDN
			LEFT JOIN (
				SELECT SerialLinkMaster
					,count([SerialLinkItem]) AS SLItemDuyet
				FROM [NvlKyDuyetItem]
				WHERE TableName = 'NvlKehoachMuaHang'
					AND [LoaiDuyet] = N'Duyệt'
					AND SerialLinkMaster >= @minserial
					AND SerialLinkMaster <= @maxserial
				GROUP BY SerialLinkMaster
				) AS qrykyduyetitem ON tbl.Serial = qrykyduyetitem.SerialLinkMaster
			INNER JOIN [DBMaster].[dbo].[Users] usr ON tbl.UserInsert = usr.UsersName
			where (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)>0 or CountTong is null)
            order by tbl.Serial desc
			SELECT *
			FROM @tblkyduyet
		END
	END

	IF (@TinhTrang = N'Đã Duyệt')
	BEGIN
		IF (@UserDuyet <> '') --Nếu USer duyệt ='' nghĩa là lấy đúng những đề nghị được ký duyệt, còn User Duyệt<>'' nghĩa là đang xem theo cụ thể những đề nghị mà USer này đã xác nhận, chứ ko liên quan đến tình trạng duyệt chính thức
		BEGIN
			INSERT INTO @tblkyduyet (
				[Serial]
				,[SerialLink]
				,[TableName]
				,[UserYeuCau]
				,[UserDuyet]
				,[LoaiDuyet]
				,[DaDuyet]
				,[GhiChu]
				,countItemDuyet,KhongDuyet
				)
			SELECT [Serial]
				,[SerialLink]
				,[TableName]
				,[UserYeuCau]
				,qryduyet.[UserDuyet]
				,[LoaiDuyet]
				,[DaDuyet]
				,[GhiChu]
				,isnull(countItemDuyet, 0) AS countItemDuyet,KhongDuyet
			FROM (
				SELECT *
				FROM NvlKyDuyet
				WHERE UserDuyet = @UserDuyet
					AND TableName = 'NvlKehoachMuaHang'
					AND SerialLink >= @minserial
					AND SerialLink <= @maxserial
				) AS qryduyet
			LEFT JOIN (
				SELECT [SerialLinkMaster]
					,[UserDuyet]
					,count(*) AS countItemDuyet
				FROM [NvlKyDuyetItem]
				WHERE UserDuyet = @UserDuyet --Lấy danh sách những gì liên quan đến user này thôi
					AND TableName = 'NvlKehoachMuaHang'
					AND SerialLinkMaster >= @minserial
					AND SerialLinkMaster <= @maxserial
				GROUP BY [SerialLinkMaster]
					,[UserDuyet]
				) AS qryitem ON qryduyet.SerialLink = qryitem.SerialLinkMaster
				AND qryduyet.UserDuyet = qryitem.UserDuyet

			--Có User Duyệt (tức là hiển thị tất cả những đề nghị người này cần kiểm tra hoặc duyệt, bất chấp đề nghị đã được người khác duyệt
			SELECT tbl.*
				,isnull(CountTong, 0) AS CountTong
				,isnull(qrykyduyetitem.SLItemDuyet, 0) AS CountDuyet
				,'{2}' + isnull(usr.PathImg, 'UserImage/user.png') AS PathImgTao
			FROM @tbldonhang tbl
			LEFT JOIN (
				SELECT SerialDN
					,SLDong AS CountTong
				FROM (
					SELECT SerialDN
						,count(*) AS SLDong
					FROM [dbo].[NvlKeHoachMuaHangItem] khitem
					WHERE SerialDN >= @minserial
						AND SerialDN <= @maxserial
					GROUP BY SerialDN
					) AS qryitem
				) AS qryallitem ON tbl.Serial = qryallitem.SerialDN
			LEFT JOIN (
				SELECT SerialLinkMaster
					,count([SerialLinkItem]) AS SLItemDuyet
				FROM [NvlKyDuyetItem]
				WHERE TableName = 'NvlKehoachMuaHang'
					AND [LoaiDuyet] = N'Duyệt'
					AND SerialLinkMaster >= @minserial
					AND SerialLinkMaster <= @maxserial
				GROUP BY SerialLinkMaster
				) AS qrykyduyetitem ON tbl.Serial = qrykyduyetitem.SerialLinkMaster
			INNER JOIN [DBMaster].[dbo].[Users] usr ON tbl.UserInsert = usr.UsersName
			INNER JOIN (
				SELECT [SerialLink]
					,max(countItemDuyet) AS countItemDuyet
				FROM @tblkyduyet
				GROUP BY SerialLink
				) AS qrydakyduyet ON tbl.Serial = qrydakyduyet.SerialLink
				AND (isnull(CountTong, 0) - isnull(qrydakyduyet.countItemDuyet, 0) <= 0) --Chỉ cần so sánh với những đề nghị mà người này được yêu cầu duyệt, nhưng chưa xác nhận
			SELECT [Serial]
				,[SerialLink]
				,[TableName]
				,[UserYeuCau]
				,[UserDuyet]
				,[LoaiDuyet]
				,[DaDuyet]
				,[GhiChu]
			FROM NvlKyDuyet
			WHERE TableName = 'NvlKehoachMuaHang'
				AND SerialLink IN (
					SELECT [SerialLink]
					FROM @tblkyduyet
					)
		END
		ELSE --Khong phân biệt theo User thì lấy đúng tình trạng của đề nghị isnull(CountTong, 0) - isnull(qrykyduyetitem.SLItemDuyet, 0) <= 0 AND CountTong IS NOT NULL
		BEGIN
			--Lấy tất cả danh sách theo USer
			INSERT INTO @tblkyduyet (
				[Serial]
				,[SerialLink]
				,[TableName]
				,[UserYeuCau]
				,[UserDuyet]
				,[LoaiDuyet]
				,[DaDuyet]
				,[GhiChu]
				,countItemDuyet,KhongDuyet
				)
			SELECT [Serial]
				,[SerialLink]
				,[TableName]
				,[UserYeuCau]
				,qryduyet.[UserDuyet]
				,[LoaiDuyet]
				,[DaDuyet]
				,[GhiChu]
				,isnull(countItemDuyet, 0) AS countItemDuyet,KhongDuyet
			FROM (
				SELECT *
				FROM NvlKyDuyet
				WHERE TableName = 'NvlKehoachMuaHang'
					AND SerialLink >= @minserial
					AND SerialLink <= @maxserial
				) AS qryduyet
			LEFT JOIN (
				SELECT [SerialLinkMaster]
					,[UserDuyet]
					,count(*) AS countItemDuyet
				FROM [NvlKyDuyetItem]
				WHERE TableName = 'NvlKehoachMuaHang'
					AND SerialLinkMaster >= @minserial
					AND SerialLinkMaster <= @maxserial
				GROUP BY [SerialLinkMaster]
					,[UserDuyet]
				) AS qryitem ON qryduyet.SerialLink = qryitem.SerialLinkMaster
				AND qryduyet.UserDuyet = qryitem.UserDuyet

			SELECT tbl.*
				,isnull(CountTong, 0) AS CountTong
				,isnull(qrykyduyetitem.SLItemDuyet, 0) AS CountDuyet
				,'{2}' + isnull(usr.PathImg, 'UserImage/user.png') AS PathImgTao
			FROM @tbldonhang tbl
			LEFT JOIN (
				SELECT SerialDN
					,SLDong AS CountTong
				FROM (
					SELECT SerialDN
						,count(*) AS SLDong
					FROM [dbo].[NvlKeHoachMuaHangItem] khitem
					WHERE SerialDN >= @minserial
						AND SerialDN <= @maxserial
					GROUP BY SerialDN
					) AS qryitem
				) AS qryallitem ON tbl.Serial = qryallitem.SerialDN
			LEFT JOIN (
				SELECT SerialLinkMaster
					,count([SerialLinkItem]) AS SLItemDuyet
				FROM [NvlKyDuyetItem]
				WHERE TableName = 'NvlKehoachMuaHang'
					AND [LoaiDuyet] = N'Duyệt'
					AND SerialLinkMaster >= @minserial
					AND SerialLinkMaster <= @maxserial
				GROUP BY SerialLinkMaster
				) AS qrykyduyetitem ON tbl.Serial = qrykyduyetitem.SerialLinkMaster
			INNER JOIN [DBMaster].[dbo].[Users] usr ON tbl.UserInsert = usr.UsersName
			 where (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)<=0 and CountTong is not null)
            order by tbl.Serial desc
			SELECT *
			FROM @tblkyduyet
		END 
    End", dieukien,"", ModelAdmin.pathurlfilepublic);
            // ShowProgress.ShowAwait();
            try
            {

                lstDonDatHangSearchShow.Clear();
                //lstDonDatHangSearchShow.TrimExcess();
                // lstDonDatHangSearchShow.Clear();
                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {

                    customRoot = JsonConvert.DeserializeObject<CustomRoot>(json);
                    if (customRoot != null)
                    {
                        lstDonDatHangSearchShow.AddRange(customRoot.lstmuahang);
                        if (customRoot.lstmuahang.Any())
                        {
                            await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
                        }
                    }

                }

                ////Xử lý load ảnh

                PanelVisible = false;


            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
                //msgBox.Show("Lỗi:" + ex.Message, IconMsg.iconerror);
                PanelVisible = false;
            }
            finally
            {

                PanelVisible = false;
                Grid.Reload();
                StateHasChanged();
                //ShowProgress.CloseAwait();
            }
        }
        private void search()
        {
            _ = searchAsync();
           // JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), "#divhide");
        }
        bool IsOpenfly { get; set; } = false;
        string img { get; set; } = IconImg.CheckMark;
        private bool checkduyet()
        {

            if (LoaiKeHoach == "KeHoachXuatHang")
                return true;
            if (kehoachshowcrr.DaDuyet != null)
            {
                msgBox.Show(string.Format("Kế hoạch này đã được {0} duyệt rồi. Bạn không được thay đổi nữa", kehoachshowcrr.DaDuyet), IconMsg.iconwarning);
                return false;
            }
            return true;
        }
       

        public async void KeHoachMasterAdd()
        {

            Urc_KeHoachMuaHangAddMaster.NvlKehoachMuaHang nvlKehoachMuaHang = new Urc_KeHoachMuaHangAddMaster.NvlKehoachMuaHang();

            nvlKehoachMuaHang.Serial = 0;
            nvlKehoachMuaHang.NguoiDN =ModelAdmin.users.TenUser;
            nvlKehoachMuaHang.NgayDN = DateTime.Now;
            nvlKehoachMuaHang.UserInsert = ModelAdmin.users.UsersName;
            nvlKehoachMuaHang.NhaMay = ModelAdmin.users.NhaMay;
            nvlKehoachMuaHang.LoaiKeHoach = LoaiKeHoach;
            nvlKehoachMuaHang.LyDo = "Cấp mới";
            nvlKehoachMuaHang.GhiChu = "";
            nvlKehoachMuaHang.BoPhanMuaHang = "";

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_KeHoachMuaHangAddMaster>(0);
                builder.AddAttribute(1, "nvlKehoachMuaHang", nvlKehoachMuaHang);
                builder.AddAttribute(2, "CallBackAfterSave", EventCallback.Factory.Create<int>(this,SearchDeNghi));
                builder.CloseComponent();
            };
           
          await  dxPopup.showAsync("TẠO ĐỀ NGHỊ");
          await  dxPopup.ShowAsync();
        }
        public async void ThemTuDeNghiXuat()
        {
           await dxFlyoutchucnang.CloseAsync();
            try
            {
                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_MuaHangAddDeNghiXuat>(0);
                    builder.AddAttribute(1, "keHoachMuaHangcrr", kehoachshowcrr.CopyClass());

                    builder.CloseComponent();
                };
                await dxPopup.showAsync("Thêm mã hàng");
                await dxPopup.ShowAsync();
            }
            catch(Exception ex)
            {

            }
        }
        public async Task KeHoachMasterEditAsync()
        {
            //ModalOptions options = new ModalOptions()
            //{
            //    Size = Blazored.Modal.ModalSize.Automatic,
            //    Position = ModalPosition.Middle,
            //    DisableBackgroundCancel = true,

            //    HideHeader = false
            //};
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            Urc_KeHoachMuaHangAddMaster.NvlKehoachMuaHang nvlKehoachMuaHang = new Urc_KeHoachMuaHangAddMaster.NvlKehoachMuaHang();


            nvlKehoachMuaHang.NguoiDN = kehoachshowcrr.NguoiDN;
            nvlKehoachMuaHang.NgayDN = kehoachshowcrr.NgayDN;
            nvlKehoachMuaHang.UserInsert = kehoachshowcrr.UserInsert;
            nvlKehoachMuaHang.NhaMay = kehoachshowcrr.NhaMay;//.users.NhaMay;
            nvlKehoachMuaHang.LoaiKeHoach = kehoachshowcrr.LoaiKeHoach;
            nvlKehoachMuaHang.LyDo = kehoachshowcrr.LyDo;
            nvlKehoachMuaHang.GhiChu = kehoachshowcrr.GhiChu;
            nvlKehoachMuaHang.KhuVuc = kehoachshowcrr.KhuVuc;
            nvlKehoachMuaHang.NgayMax = kehoachshowcrr.NgayMax;
            nvlKehoachMuaHang.NoiDung = kehoachshowcrr.NoiDung;
            nvlKehoachMuaHang.PhongBan = kehoachshowcrr.PhongBan;
            nvlKehoachMuaHang.BoPhanMuaHang = kehoachshowcrr.BoPhanMuaHang;
            nvlKehoachMuaHang.Serial = kehoachshowcrr.Serial;

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_KeHoachMuaHangAddMaster>(0);
                builder.AddAttribute(1, "nvlKehoachMuaHang", nvlKehoachMuaHang);
                builder.AddAttribute(2, "keHoachMuaHang_Showcrr", kehoachshowcrr);
                builder.AddAttribute(3, "dxgrid", Grid);
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
           await dxPopup.showAsync("SỬA ĐỀ NGHỊ");

           await dxPopup.ShowAsync();
;
        }
        public async Task KeHoachMasterDeleteAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            bool bl = await dialogMsg.Show("Xóa đề nghị", $"Bạn có chắc muốn xóa đề nghị số {kehoachshowcrr.Serial.ToString()}");

            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(kehoachshowcrr.UserInsert, ModelAdmin.users))
            {
                msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;

            }
            if (bl)
            {
                if (!checkduyet())
                {
                    return;
                }

                try
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlKehoachMuaHang_Delete";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    lstpara.Add(new ParameterDefine("@Serial", kehoachshowcrr.Serial.ToString()));
                    lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Success,"Xóa thành công"));
                           // msgBox.Show("Xóa thành công", IconMsg.iconssuccess);
                            lstDonDatHangSearchShow.Remove(kehoachshowcrr);
                            Grid.Reload();
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
                    msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
                }

            }
        }
        public async Task KeHoachAddItemFormSamPham()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(kehoachshowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không phải người tạo, nên không có quyền thêm"));
                return;
            }

            if (LoaiKeHoach.Contains("MuaHang") || LoaiKeHoach == "KeHoachSanXuat")
            {
                MenuItem menuItem = new MenuItem();
                menuItem.TextItem = "Thêm mã hàng";
                menuItem.NameItem = "createtaodonhang";
                menuItem.ComponentName = "NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Urc_KeHoachMuaHang_AddSanPham";
                menuItem.IconCssClass = "bi bi-cart3";
                if (ModelAdmin.mainLayout != null)
                {
                    RenderFragment renderFragment1;
                    renderFragment1 = builder =>
                    {

                        builder.OpenComponent<Page_KeHoachMuaHang_AddKeHoachSP>(0);
                        builder.AddAttribute(1, "keHoachMuaHang_ShowCrr", kehoachshowcrr.CopyClass());
                        builder.AddAttribute(2, "LoaiKeHoach", LoaiKeHoach);
                        builder.CloseComponent();
                    };
                    ModelAdmin.mainLayout.AddDirectRenderfagment(menuItem, renderFragment1);

                }

                //renderFragment = builder =>
                //{
                //    builder.OpenComponent<Urc_KeHoachMuaHang_AddSanPham>(0);
                //    builder.AddAttribute(1, "keHoachMuaHang_ShowCrr", kehoachshowcrr);
                //    builder.AddAttribute(2, "LoaiKeHoach", LoaiKeHoach);
                //    builder.CloseComponent();
                //};
            }
            if (LoaiKeHoach == "DeNghiXuatHangTheoKHSX")
            {
                renderFragment = builder =>
                {
                    builder.OpenComponent <Page_DeNghiTheoDinhMuc>(0);
                    builder.AddAttribute(1, "keHoachMuaHang_Showcrr", kehoachshowcrr);
                  
                    builder.CloseComponent();
                };
              await  dxPopup.showAsync("THÊM MÃ HÀNG");

              await  dxPopup.ShowAsync();
            }
            if(LoaiKeHoach == "DeNghiXuatHang")
            {
                renderFragment = builder =>
                {
                    builder.OpenComponent<Page_DeNghiTheoDinhMuc_NotPlan>(0);
                    builder.AddAttribute(1, "keHoachMuaHang_Showcrr", kehoachshowcrr);
                    builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<int>(this, SearchDeNghi));
                    builder.CloseComponent();
                };
              await  dxPopup.showAsync("THÊM MÃ HÀNG");

              await  dxPopup.ShowAsync();
            }
           
           
           

        }
        public async Task KeHoachAddItemFormSamPhamNEw()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(kehoachshowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không phải người tạo, nên không có quyền thêm"));
                return;
            }

         
            if (LoaiKeHoach == "DeNghiXuatHangTheoKHSX")
            {
                renderFragment = builder =>
                {
                    builder.OpenComponent<Page_DeNghiTheoDinhMuc_New>(0);
                    builder.AddAttribute(1, "keHoachMuaHang_Showcrr", kehoachshowcrr);

                    builder.CloseComponent();
                };
                await dxPopup.showAsync("THÊM MÃ HÀNG");

                await dxPopup.ShowAsync();
            }
            if (LoaiKeHoach == "DeNghiXuatHang")
            {
                renderFragment = builder =>
                {
                    builder.OpenComponent<Page_DeNghiTheoDinhMuc_NotPlan>(0);
                    builder.AddAttribute(1, "keHoachMuaHang_Showcrr", kehoachshowcrr);
                    builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<int>(this, SearchDeNghi));
                    builder.CloseComponent();
                };
                await dxPopup.showAsync("THÊM MÃ HÀNG");

                await dxPopup.ShowAsync();
            }




        }
        public async Task KeHoachMasterAddItemAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if(!phanQuyenAccess.NVLKeHoachMuaHangDelete(kehoachshowcrr.UserInsert,ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không phải người tạo, nên không có quyền thêm"));
                return;
            }
            NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow = new NvlKeHoachMuaHangItemShow();
            nvlKeHoachMuaHangItemShow.SerialDN = kehoachshowcrr.Serial;
            nvlKeHoachMuaHangItemShow.Serial = 0;
           
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_KeHoachMuaHang_AddItem>(0);
                builder.AddAttribute(1, "keHoachMuaHang_ShowCrr", kehoachshowcrr);
                builder.AddAttribute(2, "LoaiKeHoach", LoaiKeHoach);
                builder.AddAttribute(3, "nvlkhmhitem", nvlKeHoachMuaHangItemShow);
                builder.AddAttribute(4, "visibledetail", true);
                //builder.AddAttribute(4, "GotoMainForm", EventCallback.Factory.Create(this, RefreshRowCurrent));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
           await dxPopup.showAsync("THÊM MÃ HÀNG");
           
           await dxPopup.ShowAsync();

        }
        public async Task KeHoachMasterImportAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(kehoachshowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không phải người tạo, nên không có quyền thêm"));
                return;
            }
            NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow = new NvlKeHoachMuaHangItemShow();
            nvlKeHoachMuaHangItemShow.SerialDN = kehoachshowcrr.Serial;
            nvlKeHoachMuaHangItemShow.Serial = 0;

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_KeHoachMuaHang_Import>(0);
                builder.AddAttribute(1, "keHoachMuaHangcrr", kehoachshowcrr);
                
                //builder.AddAttribute(4, "GotoMainForm", EventCallback.Factory.Create(this, RefreshRowCurrent));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
           await dxPopup.showAsync("THÊM MÃ HÀNG");

           await dxPopup.ShowAsync();

        }

        public async Task KeHoachMasterDuyettAsync()
        {

            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(kehoachshowcrr.UserInsert, ModelAdmin.users) && kehoachshowcrr.NguoiDuyet != ModelAdmin.users.UsersName)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền duyệt dòng này do bạn không phải người tạo hoặc chưa được chọn duyệt"));
                // msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;
            }
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;


        }
        public async Task KeHoachMasterHuyDuyettAsync()
        {
            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(kehoachshowcrr.UserInsert, ModelAdmin.users) && kehoachshowcrr.NguoiDuyet != ModelAdmin.users.UsersName)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền hủy duyệt dòng này do bạn không phải người tạo hoặc chưa được chọn duyệt"));
                // msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;
            }
            if (string.IsNullOrEmpty(kehoachshowcrr.DaDuyet))
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Đề nghị này chưa duyệt, nên không cần hủy"));
                // msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;
            }
            await dxFlyoutchucnang.CloseAsync();
            CallAPI callAPI = new CallAPI();
            string sql = "NFCNVL.dbo.NvlKyDuyet_HuyDuyet";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();

            lstpara.Add(new ParameterDefine("@SerialLink", kehoachshowcrr.Serial));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
            lstpara.Add(new ParameterDefine("@NgayApDung", null));
            lstpara.Add(new ParameterDefine("@LoaiDuyet", "Duyệt"));
            lstpara.Add(new ParameterDefine("@TableName", "NvlKehoachMuaHang"));

            string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                if (query[0].ketqua == "OK")
                {
                    ToastService.Notify(new ToastMessage(ToastType.Success, $"HỦY DUYỆT thành công"));
                    kehoachshowcrr.DaDuyet = null;
                    kehoachshowcrr.NguoiDuyet = "";
                    kehoachshowcrr.TinhTrang = "Chưa duyệt";

                    Grid.Reload();
                }
                else
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketqua));

                }
                //Grid.Data = lstDonDatHangSearchShow;
            }


        }
        public async Task KeHoachMasterChonNguoiDuyettAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(kehoachshowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Bạn không có quyền duyệt dòng này do bạn không phải người tạo hoặc chưa được chọn duyệt"));
                // msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;
            }
            if(LoaiKeHoach == "KeHoachSanXuat")
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Kế hoạch này không cần duyệt"));
                // msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;
            }
            string KyDuyet = "";
            if (LoaiKeHoach == "DeNghiMuaHang"||LoaiKeHoach== "KeHoachMuaHang")
                KyDuyet = "KyDuyetMuaHang";
            if(LoaiKeHoach == "DeNghiXuatHang" || LoaiKeHoach == "DeNghiXuatHangTheoKHSX")
            {
                KyDuyet = "KyDuyetXuatKho";
            }

            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_ChonNguoiDuyet>(0);
                builder.AddAttribute(1, "kehoachmuahangcrr", kehoachshowcrr);
                builder.AddAttribute(2, "KyDuyet", KyDuyet);
                builder.AddAttribute(3, "GotoMainForm", EventCallback.Factory.Create< KeHoachMuaHang_Show>(this, RefreshListDuyet));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            dxPopup.showAsync("CHỌN NGƯỜI DUYỆT");

            dxPopup.ShowAsync();
           

        }
       
        private async void RefreshListDuyet()
        {
           await dxPopup.CloseAsync();
             await searchAsync();
            Grid.Reload();
        }
        private async void RefreshDuyetItem()
        {
           
           await Grid.SaveChangesAsync();
        }
        public async Task ShowTruocInAsync()
        {
            ModalOptions options = new ModalOptions()
            {
                Size = Blazored.Modal.ModalSize.Automatic,

                DisableBackgroundCancel = true,

                HideHeader = false
            };
            await dxFlyoutchucnang.CloseAsync();
            string noidung = "";
            if (String.IsNullOrEmpty(kehoachshowcrr.NoiDungDeNghi))
            {
                if (kehoachshowcrr.LoaiKeHoach.Contains("MuaHang") || kehoachshowcrr.LoaiKeHoach.Contains("NhapKho"))
                    noidung = string.Format("{0} đề nghị BGĐ duyệt cho mua vật tư phục vụ cho sản xuất như sau:", kehoachshowcrr.PhongBan);
                else
                    noidung = string.Format("{0} đề nghị xuất kho vật tư phục vụ sản xuất như sau:", kehoachshowcrr.PhongBan);
            }
            else
                noidung = kehoachshowcrr.NoiDungDeNghi;
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;


            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_KeHoachChinhSuaBanIn>(0);
                builder.AddAttribute(1, "keHoachMuaHang_Show", kehoachshowcrr);
                builder.AddAttribute(2, "NoiDungDeNghi", noidung);
                //builder.AddAttribute(3, "GotoMainForm", EventCallback.Factory.Create<KeHoachMuaHang_Show>(this, RefreshListDuyet));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
          await  dxPopup.showAsync("CHỈNH SỬA ĐỀ NGHỊ");

           await dxPopup.ShowAsync();
            //var parameters = new ModalParameters();
            //parameters.Add("keHoachMuaHang_Show", kehoachshowcrr);
            //parameters.Add("NoiDungDeNghi", noidung);
            //modal.Show<Urc_KeHoachChinhSuaBanIn>("", parameters, options);


        }
        private bool ShowTinhTrang() 
        {
            if (TinhTrangSelected == null)
                return false;
            if(TinhTrangSelected.Name== "Chưa duyệt")
            {
                return false;
            }
            if(TinhTrangSelected.Name== "Đã duyệt")
            {
                return true;
            }
            return false;
        }
        async void CheckedChanged(bool value)
        {
            if(value)
            {
                TinhTrangSelected = lsttinhtrang.Where(p => p.Name == "Đã duyệt").FirstOrDefault();
            }
            else
            {
                TinhTrangSelected = lsttinhtrang.Where(p => p.Name == "Chưa duyệt").FirstOrDefault();
            }
           await searchAsync();
        }
       
        List<NvlKeHoachMuaHangItemShow> lstprint = new List<NvlKeHoachMuaHangItemShow>();
        public class CustomRootPrint
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NvlKeHoachMuaHangItemShow> lstmuahang { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetShow> lstkyduyet { get; set; }
            [JsonProperty("Table2")]
            public List<NvlKeHoachMuaHangItemTotalShow> lsttotal { get; set; }
        }
        private async Task PrintAsync()
        {
            //IsOpenfly = false;
            //await dxFlyoutchucnang.CloseAsync();
            PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
            dxFlyoutchucnang.CloseAsync();
            await Task.Delay(50);
            try
            {
                //await dxFlyoutchucnang.CloseAsync();
                lstprint.Clear();
                string sql = string.Format(@"Use NVLDB

                select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT
                from 
                                (SELECT  [Serial],[SerialDN],[MaHang],MaSP
                                          ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet 
                                      FROM [NvlKeHoachMuaHangItem]
                                      Where SerialDN = @SerialDN) as qry
									  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                       inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom 

                 SELECT 
                                SerialLinkMaster,[UserDuyet],[LoaiDuyet],item.NgayInsert
                                 ,usr.TenUser
                              FROM (select  SerialLinkMaster,[UserDuyet],[LoaiDuyet],max(NgayInsert) as NgayInsert from 
                              [dbo].[NvlKyDuyetItem]
                                where SerialLinkMaster=@SerialDN group by SerialLinkMaster,[UserDuyet],[LoaiDuyet]
                              ) item
                              inner join DBMaster.dbo.Users usr on item.UserDuyet=usr.UsersName
                    ", kehoachshowcrr.Serial);

                switch (kehoachshowcrr.LoaiKeHoach)
                {
                    case "DeNghiMuaHang":
                        sql = @"Use NVLDB
                                         
                                          IF OBJECT_ID('tempdb..#tmpitem') IS NOT NULL
                                         DROP TABLE #tmpitem
                                          IF OBJECT_ID('tempdb..#tmpdinhmuc') IS NOT NULL
                                         DROP TABLE #tmpdinhmuc
                                        SELECT [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
                                        ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet,KeyGroup
                                         INTO #tmpitem FROM [NvlKeHoachMuaHangItem]
                                         Where SerialDN = @SerialDN
	                                
                                    SELECT TenKeHoach,SerialDN as SerialLink,'NvlKehoachMuaHang' as TableName, KeyGroup, MaSP,MaMauKH, sum(SoLuongSP) as SoLuong
                                    INTO #tmpdinhmuc
                                    FROM dbo.NvlKeHoachSP t2
                                    WHERE SerialDN = @SerialDN
                                    GROUP BY SerialDN,KeyGroup, MaSP,MaMauKH,TenKeHoach

									declare @tblkh table(ID nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(200),KhuVucKH nvarchar(100),MaMauKH nvarchar(100),SoLuongSP float,TenMau nvarchar(100),Color nvarchar(10))
						
                                        
										declare @tblkehoachfinal Table(KeyGroup nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(200),KhuVucKH nvarchar(100),MaMauKH nvarchar(100),SoLuongSP float,TenMau nvarchar(100),Color nvarchar(10),TenKeHoach nvarchar(100))
										insert into @tblkehoachfinal(KeyGroup,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color,TenKeHoach)
										
									select KeyGroup ,tmpdm.MaSP,sp.TenSP,'',MaMauKH,tmpdm.SoLuong as SoLuongSP,TenMau,Color,TenKeHoach
										from #tmpdinhmuc tmpdm left join [dbo].[GetMaMau]() db on (tmpdm.MaMauKH=db.MaMau)
										left  join dbo.GetSanPham() sp on tmpdm.MaSP=sp.MaSP
										
									
										

                                        select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT,isnull(qrytk.SLTon,0) as SLTon,hh.MinTK,hh.MaxTK,isnull(tblkh.TenSP,'') as TenSP,'' TenDinhMuc,'' as CongDoan,tblkh.TenKeHoach as IDKeHoach,qry.KeyGroup,cast(tblkh.SoLuongSP as int) as SoLuongSP,tblkh.TenMau,tblkh.Color,isnull(tblkh.TenKeHoach,N'Ngoài kế hoạch') as TenLienKet from 
                                                (SELECT  [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
                                                          ,[SoLuong],SLTheoDoi,[DonGia],SoLuong*DonGia as ThanhTien,SLHuy,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,KeyGroup
                                                      FROM [NvlKeHoachMuaHangItem]
                                                      Where SerialDN = @SerialDN) as qry
									                  left join (select MaHang,SLTon from dbo.GetTonKhoEx()) as qrytk on qry.MaHang=qrytk.MaHang
									                  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                                      inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom 
									                
									                  left join @tblkehoachfinal tblkh on qry.KeyGroup=tblkh.KeyGroup
									                 order by qry.STT,qry.Serial


                                        SELECT  [Serial],[SerialLinkMaster],[SerialLinkItem],[TableName],[UserDuyet],[LoaiDuyet],[GhiChu],usr.TenUser,usr.TenUser as TenUserDuyet,isnull(usr.PathImg,'UserImage/user.png') as PathImg,[NgayInsert]
                                        FROM [dbo].[NvlKyDuyetItem]  it 
										inner join DBMaster.dbo.Users usr on it.UserDuyet=usr.UsersName
                                        where SerialLinkMaster=@SerialDN and TableName='NvlKehoachMuaHang'

                                         DROP TABLE #tmpitem
                                        Drop Table #tmpdinhmuc
									                                  ";
                        break;
                    case "DeNghiXuatHangTheoKHSX":
                        sql = @"Use NVLDB

                                  							 IF OBJECT_ID('tempdb..#tmpitem') IS NOT NULL
                                         DROP TABLE #tmpitem
                                          IF OBJECT_ID('tempdb..#tmpdinhmuc') IS NOT NULL
                                         DROP TABLE #tmpdinhmuc

                                        SELECT [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
                                        ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet,KeyGroup
                                         INTO #tmpitem FROM [NvlKeHoachMuaHangItem]
                                         Where SerialDN = @SerialDN

	                                    SELECT SerialLink, TableName, KeyGroup, TenDinhMuc, CongDoan, sum(SoLuong) as SoLuong,
                                        STUFF((
                                            SELECT ';' + CAST(t1.IDKeHoach AS NVARCHAR)
                                            FROM dbo.NvlKeHoachMuaHang_DinhMuc t1
                                            WHERE SerialLink=@SerialDN and  t1.SerialLink = t2.SerialLink 
                                                AND t1.TableName = t2.TableName 
                                                AND t1.KeyGroup = t2.KeyGroup 
                                                AND t1.TenDinhMuc = t2.TenDinhMuc 
                                                AND t1.CongDoan = t2.CongDoan 
                                       FOR XML PATH('')), 1, 1, '') AS IDKeHoach
                                    INTO #tmpdinhmuc
                                    FROM dbo.NvlKeHoachMuaHang_DinhMuc t2
                                    WHERE TableName = 'NvlKehoachMuaHang' AND SerialLink = @SerialDN
                                    GROUP BY SerialLink, TableName, KeyGroup, TenDinhMuc, CongDoan

                                        
                                        declare @lstkehoach nvarchar(max)
                                         SELECT @lstkehoach = COALESCE(@lstkehoach + ';', '') + CAST(ID AS NVARCHAR)
                                         from
                                         (select Distinct(IDKeHoach) as ID
                                         FROM #tmpdinhmuc) as qry

									
                                        declare @tblkh table(ID nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(200),KhuVucKH nvarchar(100),MaMauKH nvarchar(100),SoLuongSP float,TenMau nvarchar(100),Color nvarchar(10))
                                        
                                        if(@lstkehoach is not null)
                                        begin           
                                            insert into @tblkh(ID,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color)
                                            exec SP.DataBase_ScansiaPacific2014.dbo.KeHoachSP_NVLListID @lstkehoach=@lstkehoach
                                        end
										declare @tblkehoachfinal Table(ID nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(200),KhuVucKH nvarchar(100),MaMauKH nvarchar(100),SoLuongSP float,TenMau nvarchar(100),Color nvarchar(10))
										insert into @tblkehoachfinal(ID,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color)
										
										select IDKeHoach,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color
										from
										(
										(select tmpdm.IDKeHoach,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color, 
										ROW_NUMBER() OVER (PARTITION BY tmpdm.IDKeHoach ORDER BY tmpdm.IDKeHoach ASC) AS RowNum
										from #tmpdinhmuc tmpdm inner join @tblkh kh on tmpdm.IDKeHoach like '%'+kh.ID+'%')
										) as qry where RowNum=1
									
									
										

                                        select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT,isnull(qrytk.SLTon,0) as SLTon,hh.MinTK,hh.MaxTK,tblkh.MaSP,tblkh.TenSP,khdm.TenDinhMuc,khdm.CongDoan,khdm.IDKeHoach,qry.KeyGroup,cast(khdm.SoLuong as int) as SoLuongSP,tblkh.TenMau,tblkh.Color,case when tblkh.ID is not null then tblkh.ID else  N'- Đề nghị ngoài kế hoạch * '+isnull(tblkh.MaSP,'') end as TenLienKet from 
                                                (SELECT  [Serial],[SerialDN],[MaHang],isnull(STT,0) as STT
                                                          ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,KeyGroup
										  
                                                      FROM [NvlKeHoachMuaHangItem]
                                                      Where SerialDN = @SerialDN) as qry
									                  left join (select MaHang,SLTon from dbo.GetTonKhoEx()) as qrytk
										                on qry.MaHang=qrytk.MaHang
									                  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                                       inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom 
									
									                  left join #tmpdinhmuc khdm on qry.KeyGroup=khdm.KeyGroup
									                  left join @tblkehoachfinal tblkh on khdm.IDKeHoach=tblkh.ID
									                 order by qry.STT,qry.Serial


                               SELECT 
                                SerialLinkMaster,[UserDuyet],[LoaiDuyet],item.NgayInsert
                                 ,usr.TenUser
                              FROM (select  SerialLinkMaster,[UserDuyet],[LoaiDuyet],max(NgayInsert) as NgayInsert from 
                              [dbo].[NvlKyDuyetItem]
                                where SerialLinkMaster=@SerialDN group by SerialLinkMaster,[UserDuyet],[LoaiDuyet]
                              ) item
                              inner join DBMaster.dbo.Users usr on item.UserDuyet=usr.UsersName

                        --Xử lý phần tổng hợp
							  declare @tbl as Table(MaHang nvarchar(100) primary key,SerialDN int)

							  insert into @tbl(MaHang,SerialDN)
							  select MaHang,SerialDN from NvlKeHoachMuaHangItem where SerialDN=@SerialDN group by MaHang,SerialDN



				                select tmp.MaHang,tmp.SLDeNghi as SoLuong,qrytotal.KhoNo,qrytotal.XuongNo,hh.TenHang,hh.DVT from

				                (select MaHang,sum(SoLuong) as SLDeNghi from #tmpitem group by MaHang) tmp left join
				                  (select qry.MaHang,sum(qry.KhoNo) as KhoNo,sum(XuongNo) as XuongNo from 
				                  (select MaHang,sum(SLTheoDoi) as KhoNo,0 as XuongNo
				                  from NvlKeHoachMuaHangItem 
				                  where  MaHang in (select MaHang from @tbl) and SLTheoDoi>0
				                  and SerialDN in (select Serial from NvlKehoachMuaHang where Serial<@SerialDN and LoaiKeHoach in ('DeNghiXuatHangTheoKHSX','DeNghiXuatHang'))
                                   and Serial in (select SerialLinkItem from NvlKyDuyetItem  where LoaiDuyet=N'Duyệt' and TableName='NvlKehoachMuaHang')
				                  group by MaHang
				                  union all
				                  SELECT [MaHang],0 as KhoNo,sum([SLGhiNo]-[SLTra]) as XuongNo
				                FROM [NvlNhapXuatGhiNo] where MaHang in (select MaHang from @tbl) 
				                group by MaHang
				                  ) as qry 
				 
				                  group by MaHang) as qrytotal on tmp.MaHang=qrytotal.MaHang
				                  inner join dbo.NvlHangHoa hh on tmp.MaHang=hh.MaHang

                                 DROP TABLE #tmpitem
                                  DROP TABLE #tmpdinhmuc
									                                  ";
                        break;
                 

                   
                    case "DeNghiXuatHang":
                        sql = @"Use NVLDB
                                   IF OBJECT_ID('tempdb..#tmpitem') IS NOT NULL
                                 DROP TABLE #tmpitem
                                  IF OBJECT_ID('tempdb..#tmpitemdm') IS NOT NULL
                                 DROP TABLE #tmpitemdm
                                SELECT [MaHang],sum([SoLuong]) as SoLuong,KeyGroup
                                INTO #tmpitem
                                FROM [NvlKeHoachMuaHangItem]
                                Where SerialDN = @SerialDN
                                group by KeyGroup,[MaHang]

								 select tmitem.MaHang,sum(tmitem.SoLuong) as SoLuong,khdm.CongDoan,khdm.TenDinhMuc,khdm.IDKeHoach,khdm.MaSP,sp.TenSP,khdm.MaMau,sum(khdm.SoLuong) as SoLuongSP
                                into #tmpitemdm
                                from #tmpitem tmitem left join dbo.NvlKeHoachMuaHang_DinhMuc khdm on tmitem.KeyGroup=khdm.KeyGroup
								left join dbo.GetSanPham() sp on khdm.MaSP=sp.MaSP
                                group by tmitem.MaHang,khdm.CongDoan,khdm.TenDinhMuc,khdm.IDKeHoach,khdm.MaSP,khdm.MaMau,sp.TenSP

								

                                    --Xử lý kiểm tra xem đề nghị này có lẫn lộn giữa đề nghị theo định mức và đề nghị lẻ tẻ không
                                    declare @checkdinhmuc int
                                    select top 1 @checkdinhmuc=Serial from NvlKeHoachMuaHang_DinhMuc where SerialLink=@SerialDN and TableName='NvlKehoachMuaHang'
                                    if(@checkdinhmuc is null) --Nếu là đề nghị không có định mức
                                    begin
                                    select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT
                                    from 
                                                    (SELECT  [Serial],[SerialDN],[MaHang],MaSP
                                                              ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet 
                                                          FROM [NvlKeHoachMuaHangItem]
                                                          Where SerialDN = @SerialDN) as qry
								                                      inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                                           inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom


                                    end
                                    else --Nếu là đề nghị có chứa định mức
                                    begin

                                

                               

                                 declare @lstkehoach nvarchar(max)
                                 -- SELECT @lstkehoach = COALESCE(@lstkehoach + ';', '') + CAST(ID AS NVARCHAR)
                                  --from
                                 -- (select Distinct(IDKeHoach) as ID
                                  --FROM #tmpitemdm) as qry
                            SELECT @lstkehoach = STUFF((
                            SELECT ';' + CAST(ID AS NVARCHAR)
                            FROM (select Distinct(IDKeHoach) as ID
                                  FROM #tmpitemdm) as qry
                            FOR XML PATH('')
                                ), 1, 1, '')

                                select qry.MaHang,qry.SoLuong,qry.CongDoan,TenDinhMuc,IDKeHoach,isnull(IDKeHoach,'')+'-'+isnull(TenDinhMuc,'')+'-'+isnull(CongDoan,'') as PhanLoai,case when qry.IDKeHoach is not null then qry.IDKeHoach else  N'- Đề nghị ngoài kế hoạch * '+isnull(qry.MaSP,'') end as TenLienKet,hh.TenHang,hh.DVT,isnull(qry.TenSP,N'Đề nghị ngoài kế hoạch') as TenSP,qry.MaSP,cast(isnull(qry.SoLuongSP,0) as int) as SoLuongSP,qry.MaMau
                                from 
                                (SELECT  * from #tmpitemdm) as qry
                                inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                             
                       
                               
	                        end	
							 SELECT 
                                SerialLinkMaster,[UserDuyet],[LoaiDuyet],item.NgayInsert
                                 ,usr.TenUser
                              FROM (select  SerialLinkMaster,[UserDuyet],[LoaiDuyet],max(NgayInsert) as NgayInsert from 
                              [dbo].[NvlKyDuyetItem]
                                where SerialLinkMaster=@SerialDN group by SerialLinkMaster,[UserDuyet],[LoaiDuyet]
                              ) item
                              inner join DBMaster.dbo.Users usr on item.UserDuyet=usr.UsersName

                            --Xử lý phần tổng hợp
							  declare @tbl as Table(MaHang nvarchar(100) primary key,SerialDN int)

							  insert into @tbl(MaHang,SerialDN)
							  select MaHang,SerialDN from NvlKeHoachMuaHangItem where SerialDN=@SerialDN group by MaHang,SerialDN



				                select tmp.MaHang,tmp.SLDeNghi as SoLuong,qrytotal.KhoNo,qrytotal.XuongNo,hh.TenHang,hh.DVT from

				                (select MaHang,sum(SoLuong) as SLDeNghi from #tmpitemdm group by MaHang) tmp left join
				                  (select qry.MaHang,sum(qry.KhoNo) as KhoNo,sum(XuongNo) as XuongNo from 
				                  (select MaHang,sum(SLTheoDoi) as KhoNo,0 as XuongNo
				                  from NvlKeHoachMuaHangItem 
				                  where  MaHang in (select MaHang from @tbl) and SLTheoDoi>0
				                  and SerialDN in (select Serial from NvlKehoachMuaHang where Serial<@SerialDN and LoaiKeHoach in ('DeNghiXuatHangTheoKHSX','DeNghiXuatHang'))
                                    and Serial in (select SerialLinkItem from NvlKyDuyetItem  where LoaiDuyet=N'Duyệt' and TableName='NvlKehoachMuaHang')
				                  group by MaHang
				                  union all
				                  SELECT [MaHang],0 as KhoNo,sum([SLGhiNo]-[SLTra]) as XuongNo
				                FROM [NvlNhapXuatGhiNo] where MaHang in (select MaHang from @tbl) 
				                group by MaHang
				                  ) as qry 
				 
				                  group by MaHang) as qrytotal on tmp.MaHang=qrytotal.MaHang
				                  inner join dbo.NvlHangHoa hh on tmp.MaHang=hh.MaHang
  

                                 DROP TABLE #tmpitem
                                  DROP TABLE #tmpitemdm
                            

                           ";
                        break;

                    default:

                        break;
                }

                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialDN", kehoachshowcrr.Serial));
                //string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);

                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                //if (json != "")
                //{

                //    customRoot = JsonConvert.DeserializeObject<CustomRoot>(json);
                //    if (customRoot != null)
                //    {
                //        lstDonDatHangSearchShow = customRoot.lstmuahang;
                //    }

                //}

                if (json != "")
                {
                    var custom = JsonConvert.DeserializeObject<CustomRootPrint>(json);
                    var query = custom.lstmuahang;
                    foreach (var it in query)
                    {
                        if (!string.IsNullOrEmpty(it.TenMau))
                        {
                            it.TenMau = StaticClass.RemoveVietnamese(it.TenMau);//Remove dấu tiếng việt đi để làm  TenMau trong xtrareport ko bị lỗi khi xuất trang In ra máy in, còn nguyên nhân thì ko hiểu tại sao
                        }
                    }
                    lstprint.AddRange(query);

                    //kehoachshowcrr.lstitem.AddRange(query);
                    if (custom.lstkyduyet != null)
                    {
                        foreach (var it in custom.lstkyduyet)
                        {
                            if (it.LoaiDuyet == "Duyệt")
                            {
                                if (string.IsNullOrEmpty(kehoachshowcrr.NguoiDuyet))
                                    kehoachshowcrr.NguoiDuyet = it.TenUser;
                                else
                                {
                                    if(!kehoachshowcrr.NguoiDuyet.Contains(it.TenUser))
                                        kehoachshowcrr.NguoiDuyet += ";" + it.TenUser;
                                }
                                   
                            }

                            if (it.LoaiDuyet == "Kiểm tra")
                            {
                                if (string.IsNullOrEmpty(kehoachshowcrr.NguoiKiem))
                                {
                                    kehoachshowcrr.NguoiKiem = it.TenUser;
                                }
                                else
                                {
                                    if (!kehoachshowcrr.NguoiKiem.Contains(it.TenUser))
                                        kehoachshowcrr.NguoiKiem += ";" + it.TenUser;
                                }
                                   
                            }

                        }
                    }
                    printreport(kehoachshowcrr, custom);


                }



            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
            finally
            {
                PreloadService.Hide();
            }



        }
        private void printreport(KeHoachMuaHang_Show kehoachshowcrr, CustomRootPrint customRootit)
        {
            var parameters = new ModalParameters();
            string diengialydo = "";
            diengialydo = string.Format("Lý do cấp vật tư: {0}", kehoachshowcrr.LyDo);
            if (!string.IsNullOrEmpty(kehoachshowcrr.NoiDung))
            {
                diengialydo += Environment.NewLine + "Nội dung: " + kehoachshowcrr.NoiDung;
            }
            switch (kehoachshowcrr.LoaiKeHoach)
            {
                case "KeHoachMuaHang":
                    XtraRp_KeHoachMuaHang xtra_KTGTonKho = new XtraRp_KeHoachMuaHang();
                    xtra_KTGTonKho.DataSource = lstprint;
                    xtra_KTGTonKho.setNoidung(kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                    //xtra_KTGTonKho.setTotal(querytotal[0].SoLuong, querytotal[0].ThanhTien);
                    xtra_KTGTonKho.setMaDeNghi(kehoachshowcrr.MaDN, kehoachshowcrr.Serial);
                    xtra_KTGTonKho.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, kehoachshowcrr.NoiDungDeNghi);
                    //parameters.Add("report", xtra_KTGTonKho);
                    //modal.Show<ReportShow>("", parameters, options);

                    ModelAdmin.mainLayout.showreportAsync(xtra_KTGTonKho);
                    break;
                case "DeNghiMuaHang":
                    var querycheckrp = lstprint.Where(p => p.KeyGroup != null).FirstOrDefault();
                    if(querycheckrp != null)
                    {
                        App_NguyenVatLieu.Report.XtraRp_DeNghiMuaHangDinhMuc xtraRp_DuTruVatTu = new XtraRp_DeNghiMuaHangDinhMuc();
                        xtraRp_DuTruVatTu.DataSource = lstprint;
                        xtraRp_DuTruVatTu.setNoidung((kehoachshowcrr.NoiDung == null) ? "" : kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                        xtraRp_DuTruVatTu.setMaDeNghi(kehoachshowcrr.Serial.ToString());
                        xtraRp_DuTruVatTu.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, diengialydo, kehoachshowcrr.NgayDN);
                        //xtraRp_DuTruVatTu.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, kehoachshowcrr.NoiDungDeNghi);
                        //parameters.Add("report", xtraRp_DuTruVatTu);
                        //modal.Show<ReportShow>("", parameters, options);
                        ModelAdmin.mainLayout.showreportAsync(xtraRp_DuTruVatTu);
                    }
                    else
                    {
                        App_NguyenVatLieu.Report.XtraRp_DuTruVatTu xtraRp_DuTruVatTu = new XtraRp_DuTruVatTu();
                        xtraRp_DuTruVatTu.DataSource = lstprint;
                        xtraRp_DuTruVatTu.setNoidung((kehoachshowcrr.NoiDung == null) ? "" : kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                        xtraRp_DuTruVatTu.setMaDeNghi(kehoachshowcrr.Serial.ToString());
                        xtraRp_DuTruVatTu.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, kehoachshowcrr.NoiDungDeNghi);
                        //parameters.Add("report", xtraRp_DuTruVatTu);
                        //modal.Show<ReportShow>("", parameters, options);
                        ModelAdmin.mainLayout.showreportAsync(xtraRp_DuTruVatTu);
                    }
                  
                    break;
                case "DeNghiXuatHang":

                    var querycheckform = lstprint.Where(x => !string.IsNullOrEmpty(x.CongDoan)).FirstOrDefault();
                    if (querycheckform == null)
                    {
                        //Đề nghị có chứa định mức
                        XtraRp_DeNghiCapVatTu xtraRp_DeNghiCapVatTu = new XtraRp_DeNghiCapVatTu();
                        XRSubreport xrqtitem1 = xtraRp_DeNghiCapVatTu.FindControl("xrSubreport1", true) as XRSubreport;
                        //  XtraRp_DonDatHangItem xtraRp_DonDatHangItem = (XtraRp_DonDatHangItem)xrqtitem.ReportSource;
                        XtraRp_DeNghiXuatKho_Total xtra_KTGTonKhotoatl1 = (XtraRp_DeNghiXuatKho_Total)xrqtitem1.ReportSource;
                        xtra_KTGTonKhotoatl1.setdiengiai(string.Format("(Số lượng nợ cũ trước Đề nghị số {0})", kehoachshowcrr.Serial));
                        xtra_KTGTonKhotoatl1.DataSource = customRootit.lsttotal;
                        xtraRp_DeNghiCapVatTu.DataSource = lstprint;
                        xtraRp_DeNghiCapVatTu.setNoidung((kehoachshowcrr.NoiDung == null) ? "" : kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                        xtraRp_DeNghiCapVatTu.setMaDeNghi(kehoachshowcrr.Serial.ToString());
                        xtraRp_DeNghiCapVatTu.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, diengialydo, kehoachshowcrr.NgayDN);
                        //parameters.Add("report", xtra_KTGTonKho);
                        //modal.Show<ReportShow>("", parameters, options);
                        ModelAdmin.mainLayout.showreportAsync(xtraRp_DeNghiCapVatTu);
                    }
                    else
                    {
                        XtraRp_DeNghiXuatKho rp_DeNghiXuatKhoDinhMuc = new XtraRp_DeNghiXuatKho();
                        XRSubreport xrqtitem1 = rp_DeNghiXuatKhoDinhMuc.FindControl("xrSubreport1", true) as XRSubreport;
                        //  XtraRp_DonDatHangItem xtraRp_DonDatHangItem = (XtraRp_DonDatHangItem)xrqtitem.ReportSource;
                        XtraRp_DeNghiXuatKho_Total xtra_KTGTonKhotoatl1 = (XtraRp_DeNghiXuatKho_Total)xrqtitem1.ReportSource;
                        //var querytotal1 = lstprint.GroupBy(p => new { MaHang = p.MaHang, TenHang = p.TenHang, DVT = p.DVT })
                        //   .Select(p => new { MaHang = p.Key.MaHang, TenHang = p.Key.TenHang, DVT = p.Key.DVT, SoLuong = p.Sum(n => n.SoLuong) }).OrderBy(p => p.MaHang).ToList();
                        xtra_KTGTonKhotoatl1.DataSource = customRootit.lsttotal;
                        xtra_KTGTonKhotoatl1.setdiengiai(string.Format("(Số lượng nợ cũ trước Đề nghị số {0})", kehoachshowcrr.Serial));
                        rp_DeNghiXuatKhoDinhMuc.DataSource = lstprint;
                        rp_DeNghiXuatKhoDinhMuc.setNoidung((kehoachshowcrr.NoiDung == null) ? "" : kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                        rp_DeNghiXuatKhoDinhMuc.setMaDeNghi(kehoachshowcrr.Serial.ToString());
                        rp_DeNghiXuatKhoDinhMuc.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, diengialydo, kehoachshowcrr.NgayDN);
                        //parameters.Add("report", xtra_KTGTonKho);
                        //modal.Show<ReportShow>("", parameters, options);
                        ModelAdmin.mainLayout.showreportAsync(rp_DeNghiXuatKhoDinhMuc);
                    }
                    break;
                case "DeNghiXuatHangTheoKHSX":
                    XtraRp_DeNghiXuatKho rp_DeNghiXuatKho = new XtraRp_DeNghiXuatKho();
                    XRSubreport xrqtitem2 = rp_DeNghiXuatKho.FindControl("xrSubreport1", true) as XRSubreport;
                    //  XtraRp_DonDatHangItem xtraRp_DonDatHangItem = (XtraRp_DonDatHangItem)xrqtitem.ReportSource;
                    XtraRp_DeNghiXuatKho_Total xtra_KTGTonKhotoatl2 = (XtraRp_DeNghiXuatKho_Total)xrqtitem2.ReportSource;
                    //var querytotal = lstprint.GroupBy(p => new { MaHang = p.MaHang, TenHang = p.TenHang, DVT = p.DVT })
                    //  .Select(p => new { MaHang = p.Key.MaHang, TenHang = p.Key.TenHang, DVT = p.Key.DVT, SoLuong = p.Sum(n => n.SoLuong) }).OrderBy(p => p.MaHang).ToList();
                    xtra_KTGTonKhotoatl2.DataSource = customRootit.lsttotal;
                    rp_DeNghiXuatKho.DataSource = lstprint;
                    xtra_KTGTonKhotoatl2.setdiengiai(string.Format("(Số lượng nợ cũ trước Đề nghị số {0})", kehoachshowcrr.Serial));
                    rp_DeNghiXuatKho.setNoidung((kehoachshowcrr.NoiDung == null) ? "" : kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                    rp_DeNghiXuatKho.setMaDeNghi(kehoachshowcrr.Serial.ToString());
                    rp_DeNghiXuatKho.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, diengialydo, kehoachshowcrr.NgayDN);
                    //parameters.Add("report", xtra_KTGTonKho);
                    //modal.Show<ReportShow>("", parameters, options);
                    ModelAdmin.mainLayout.showreportAsync(rp_DeNghiXuatKho);
                    break;
                default:
                    XtraRp_DeNghiCapVatTu xtraRp_DeNghiCapVatTu1 = new XtraRp_DeNghiCapVatTu();
                    XRSubreport xrqtitem3 = xtraRp_DeNghiCapVatTu1.FindControl("xrSubreport1", true) as XRSubreport;
                    //  XtraRp_DonDatHangItem xtraRp_DonDatHangItem = (XtraRp_DonDatHangItem)xrqtitem.ReportSource;
                    XtraRp_DeNghiXuatKho_Total xtra_KTGTonKhotoatl3 = (XtraRp_DeNghiXuatKho_Total)xrqtitem3.ReportSource;
                    //var querytotal = lstprint.GroupBy(p => new { MaHang = p.MaHang, TenHang = p.TenHang, DVT = p.DVT })
                    //  .Select(p => new { MaHang = p.Key.MaHang, TenHang = p.Key.TenHang, DVT = p.Key.DVT, SoLuong = p.Sum(n => n.SoLuong) }).OrderBy(p => p.MaHang).ToList();
                    xtra_KTGTonKhotoatl3.DataSource = customRootit.lsttotal;
                    xtraRp_DeNghiCapVatTu1.DataSource = lstprint;
                    xtraRp_DeNghiCapVatTu1.setNoidung((kehoachshowcrr.NoiDung == null) ? "" : kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                    xtraRp_DeNghiCapVatTu1.setMaDeNghi(kehoachshowcrr.Serial.ToString());
                    xtraRp_DeNghiCapVatTu1.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, diengialydo, kehoachshowcrr.NgayDN);
                    //parameters.Add("report", xtra_KTGTonKho);
                    //modal.Show<ReportShow>("", parameters, options);
                    ModelAdmin.mainLayout.showreportAsync(xtraRp_DeNghiCapVatTu1);
                    break;
            }




        }
       
        public string TextSoDeNghi(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
            switch (keHoachMuaHang_Show.LoaiKeHoach)
            {
                case "DeNghiMuaHang":
                    return "Đề nghị mua hàng số: " + keHoachMuaHang_Show.Serial.ToString();
                case "DeNghiXuatHang":
                    return "Đề nghị xuất hàng số: " + keHoachMuaHang_Show.Serial.ToString();
                case "KeHoachMuaHang":
                    return "Kế hoạch mua hàng số: " + keHoachMuaHang_Show.Serial.ToString();
                case "KeHoachSanXuat":
                    return "Kế hoạch mua hàng số: " + keHoachMuaHang_Show.Serial.ToString();
                default: 
                    return "Đề nghị số: " + keHoachMuaHang_Show.Serial.ToString();
            }
        }
        public string TextNgayDeNghi(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {

            return "Ngày đề nghị " + keHoachMuaHang_Show.NgayDN.Value.ToString("dd/MM/yy");


        }
        private bool Visibleduyetall(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
          
            if (keHoachMuaHang_Show.lstkyduyet != null)
            {
                var query=keHoachMuaHang_Show.lstkyduyet.Where(p=>p.UserDuyet==ModelAdmin.users.UsersName).FirstOrDefault();
                if(query!=null)
                {
                    keHoachMuaHang_Show.ShowTextDuyet=query.LoaiDuyet;
                    return true;
                }
                    
                return false;
                //foreach (var it in keHoachMuaHang_Show.lstkyduyet)
                //{
                //    if (ModelAdmin.users.UsersName == it.UserDuyet)
                //    {
                //        return true;
                //    }
                //}
            }
            return false;
        }
        public async void ShowFlyout(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
            try
            {
                await dxFlyoutchucnang.CloseAsync();
                kehoachshowcrr = keHoachMuaHang_Show;
                idflychucnang = "#" + idelement(keHoachMuaHang_Show.Serial);
                Visileprint = true;
                if (LoaiKeHoach.Contains("KyDuyet"))
                {

                    if (kehoachshowcrr.NguoiDuyet == ModelAdmin.users.UsersName)
                    {
                        Visilehuykyduyet = true;
                        Visilekyduyet = true;
                    }
                    else
                    {
                        Visilehuykyduyet = false;
                        Visilekyduyet = false;
                    }
                    //IsOpenfly = true;
                    await dxFlyoutchucnang.ShowAsync();
                    return;
                }
                if (kehoachshowcrr.UserInsert == ModelAdmin.users.UsersName || ModelAdmin.users.GroupUser.Contains("admin"))
                {
                    Visilechinhsua = true;
                    Visilechinhtruocin = true;
                    Visilechinhtruocin = true;
                    Visilechonnguoiduyet = true;
                    Visiledelete = true;
                    Visilehuykyduyet = true;
                    Visilekyduyet = true;

                    Visilethemchitiet = true;

                }
                else
                {

                    Visilechinhsua = false;
                    Visilechinhtruocin = false;
                    Visilechinhtruocin = false;
                    Visilechonnguoiduyet = false;
                    Visiledelete = false;
                    Visilethemchitiet = false;
                    if (kehoachshowcrr.NguoiDuyet == ModelAdmin.users.UsersName)
                    {
                        Visilehuykyduyet = true;
                        Visilekyduyet = true;
                    }
                    else
                    {
                        Visilehuykyduyet = false;
                        Visilekyduyet = false;
                    }
                }
                //IsOpenfly = true;
                await dxFlyoutchucnang.ShowAsync();
            }
            catch(Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Loi: " + ex.Message));

            }

        }
       
        private async void lydoxoaCallBack(string lydo)
        {
           string lydoxoa = lydo;

            if (!string.IsNullOrEmpty(lydoxoa))
            {
                dxPopup.CloseAsync();
                try
                {

                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlKyDuyet_KhongDuyetAll";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    lstpara.Add(new ParameterDefine("@SerialLink", kehoachshowcrr.Serial));

                    lstpara.Add(new ParameterDefine("@TableName", "NvlKehoachMuaHang"));
                    lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                    lstpara.Add(new ParameterDefine("@GhiChu", lydoxoa));


                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            //ToastService.Notify(new(ToastType.Success, $"Duyệt thành công"));
                            ToastService.Notify(new ToastMessage(ToastType.Success, "Đã Xác nhận không duyệt"));
                            foreach(var it in kehoachshowcrr.lstkyduyet)
                            {
                                it.KhongDuyet = ModelAdmin.users.UsersName;
                            }
                          
                            kehoachshowcrr.KhongDuyet = lydoxoa;
                            //kehoachshowcrr.lstkyduyet.ForEach(p => p.KhongDuyet = ModelAdmin.users.UsersName);
                            StateHasChanged();
                           // kehoachshowcrr.EnableButtonDuyet = false;
                            //nvlKeHoachMuaHangItemShow.TextKiem = ModelAdmin.users.TenUser;
                            
                             

                        }
                        else
                        {
                            ToastService.Notify(new(ToastType.Danger, $"Lỗi: {query[0].ketqua},{query[0].ketquaexception} .Duyệt không được"));

                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message}"));
                }
            }
        }
        private async Task KhongDuyetAllAsync(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
            if (keHoachMuaHang_Show.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in keHoachMuaHang_Show.lstkyduyet)
            {
                if (ModelAdmin.users.UsersName == it.UserDuyet)
                {
                    checkduyet = true;
                    LoaiDuyet = it.LoaiDuyet;
                    break;
                }
            }
            if (!checkduyet)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có trong danh sách được duyệt!"));
                return;
            }
            kehoachshowcrr = keHoachMuaHang_Show;
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NhapLyDo>(0);
                builder.AddAttribute(1, "GetLyDo", EventCallback.Factory.Create<string>(this, lydoxoaCallBack));
                builder.CloseComponent();
            };

            await dxPopup.showAsync("Không duyệt vì lý do gì?");
            await dxPopup.ShowAsync();
            
           
        }
        private async Task DuyetItemAllAsync(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
            if (keHoachMuaHang_Show.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in keHoachMuaHang_Show.lstkyduyet)
            {
                if (ModelAdmin.users.UsersName == it.UserDuyet)
                {
                    checkduyet = true;
                    LoaiDuyet = it.LoaiDuyet;
                    break;
                }
            }
            if (!checkduyet)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có trong danh sách được duyệt!"));
                return;
            }

            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlKyDuyetItem_InsertAll";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialLinkMaster", keHoachMuaHang_Show.Serial));
                lstpara.Add(new ParameterDefine("@SerialLinkItem", 0));
                lstpara.Add(new ParameterDefine("@TableName", "NvlKehoachMuaHang"));
                lstpara.Add(new ParameterDefine("@UserDuyet", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@GhiChu", ""));


                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        //ToastService.Notify(new(ToastType.Success, $"Duyệt thành công"));
                        if (LoaiDuyet == "Duyệt")
                        {
                            keHoachMuaHang_Show.CountDuyet = keHoachMuaHang_Show.CountTong;
                            if(keHoachMuaHang_Show.lstitem!=null)
                            {
                                foreach (var it in keHoachMuaHang_Show.lstitem)
                                {
                                    it.TextDuyet = ModelAdmin.users.TenUser;
                                }
                            }
                            
                            // nvlKeHoachMuaHangItemShow.TextDuyet = ModelAdmin.users.TenUser;
                        }
                        else
                        {
                            if (keHoachMuaHang_Show.lstitem != null)
                            {
                                foreach (var it in keHoachMuaHang_Show.lstitem)
                                {
                                    it.TextKiem = ModelAdmin.users.TenUser;
                                }
                            }
                        }
                        ToastService.Notify(new ToastMessage(ToastType.Success, "Xác nhận thành công"));
                        keHoachMuaHang_Show.EnableButtonDuyet = false;
                        keHoachMuaHang_Show.KhongDuyet = "";
                        //nvlKeHoachMuaHangItemShow.TextKiem = ModelAdmin.users.TenUser;
                        await Grid.SaveChangesAsync();
                       // Grid.Reload();
                        
                    }
                    else
                    {
                        ToastService.Notify(new(ToastType.Danger, $"Lỗi: {query[0].ketqua},{query[0].ketquaexception} .Duyệt không được"));

                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message}"));
            }

        }
        private async void SearchDeNghi(int Serial)
        {
            await dxPopup.CloseAsync();
            TinhTrangSelected = lsttinhtrang.Where(p => p.Name == "Chưa duyệt").FirstOrDefault();
            SerialDN=Serial;
            await searchAsync();
            
            SerialDN = null;
        }
        private async Task ShowVideoAsync(string link)
        {
            var url = link;
            await JSRuntime.InvokeVoidAsync("window.open", url, "_blank");
            //var url = link;
            //Navigation.NavigateTo(url, forceLoad: true);
        }
    }
}
