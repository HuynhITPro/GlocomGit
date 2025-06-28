using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using NFCWebBlazor.Pages;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_DeNghiTheoDinhMuc;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Urc_KeHoachMuaHang_AddSanPham;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucHangHoaMaster;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Page_DinhMucNVLMaster
    {
        public class DinhMucVatTuShow
        {
            public int Index { get; set; }
            public string MaSP { get; set; }
            public string TenSP { get; set; }
            public string? MaMau { get; set; }
            public string TenMau { get; set; }
           
            public string GroupMauSP { get; set; }
            public string MauSP { get; set; }
            public string MaKH { get; set; } = "";
            public string MaVatTu { get; set; }
            public string KhuVucKH { get; set; }
            public string TenHang { get; set; }
            public double SLQuyDoi { get; set; } = 0;
            public double SLKeHoach { get; set; } = 0;

            public double SLSuDung { get; set; } = 0;
            public decimal? SLConLai
            {
                get; set;
            }
            public decimal? SLTon { get; set; }
            decimal? _dbton { get; set; }
            public decimal? DBTon
            {
                get
                {
                    if (SLQuyDoi == null || SLTon == null) return null;
                    if (SLQuyDoi == 0)
                    {
                        TyLeDB = 0;
                        return 0;
                    }
                    else
                    {
                        _dbton= (decimal)(SLTon.Value / (decimal)SLQuyDoi); ;
                        if (SLDeNghi > 0)
                            TyLeDB = (double)((_dbton) / SLDeNghi);
                        return _dbton;
                    }
                        
                }
            }
            public double? TyLeDB { get; set; }
            
            public decimal? SLDeNghi
            { get; set; }
            public string KhuVuc { get; set; }
            public string DVT { get; set; }
            public string NhaCungCap { get; set; }
            public string GroupNhaCungCap { get; set; }
            public string CongDoan { get; set; }
            public string TenDinhMuc { get; set; }
            public string KhachHang { get; set; }
            public string TenNCC { get; set; }

            public int? ChonNCC { get; set; }
            public uint? Color { get { return _color; } set { _color = value; Colorhex = StaticClass.UIntToHtmlColor(_color); } }
            private uint? _color { get; set; }
            public string Colorhex
            {
                get; set;

            }
            public DinhMucVatTuShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<DinhMucVatTuShow>(json);
            }
            public double? SLTTDeNghi
            { get; set; }
            public double? SLTTXuat
            { get; set; }
            public string Colortext { get; set; }
            public bool chk { get; set; } = false;

            public string Err { get; set; }
            public string KeyGroup { get; set; }
        }
        public class KeHoachDinhMucCongDoan
        {
            public bool chk { get; set; } = false;
            public string GroupNhaCungCap { get; set; }

            public string MaSP { get; set; }
            public string MauSP { get; set; }
            public string TenDinhMuc { get; set; }
            public string KhuVucKH { get; set; }
            public string CongDoan { get; set; }
            public string KeyGroup { get; set; }


            public List<KeHoachDinhMucCongDoanItem> lstkehoachcongdoanitem = new List<KeHoachDinhMucCongDoanItem>();

        }
        public class SanPhamShow
        {
            public bool Chk { get; set; }
            public int? Serial { get; set; }
            public string MaSP { get; set; }

            public string TenSP { get; set; }
            public string KhachHang { get; set; }
            public int _mua { get; set; }
            public string PathImg { get; set; }
            public string TinhTrang { get; set; }
            public string ArticleNumber { get; set; }
            public string MaMau { get; set; }
            public DateTime? Ngay { get; set; }
            public decimal? SLSP { get; set; }
            public string TenMau { get; set; }
            public string Colorhex { get; set; }
            private uint? _color { get; set; }
            public uint? Color
            {
                get { return _color; }
                set
                {
                    _color = value;
                    Colorhex = StaticClass.UIntToHtmlColor(_color);
                }
            }
            public List<KeHoachDinhMucCongDoan> lstkehoachcongdoan { get; set; }
            public int Mua
            {
                get
                {
                    return _mua;
                }
                set
                {
                    _mua = value;
                    if (_mua == 1)
                    {
                        PathImg = IconImg.CheckMark;
                        TinhTrang = "Đang sản xuất";

                        //Foreground = Model.ModelAdmin.ColorPrimary;

                    }
                    else
                    {
                        PathImg = IconImg.NotCheck;
                        TinhTrang = "Không sản xuất";

                        //Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fe5a1d"));
                    }


                }
            }
            public List<DinhMucVatTuShow> lstdinhmuc { get; set; }

        }
        [Inject]
        ToastService toastService { get; set; }
        [Inject] BrowserService browserService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        public class KeHoachSelected()
        {
            public string MaSP { get; set; }
            public string MauSP { get; set; }
            public string GroupNhaCungCap { get; set; }
            public decimal? SLDeNghi { get; set; }
            public int Option { get; set; } = 1;
        }
        bool CheckQuyen = false;
        List<KeHoachSelected> lstKeHoachSelected = new List<KeHoachSelected>();



        private void checkedchangedItem(bool bl, DinhMucVatTuShow dinhMucVatTuShow)
        {
            dinhMucVatTuShow.chk = bl;

        }
        public class KeHoachThangItem : INotifyPropertyChanged
        {


            public int Serial { get; set; }

            public string MaHang
            {
                get; set;
            }

            public string TenHang
            {
                get; set;
            }
            public string Err { get; set; }

            public string KhuVuc
            {
                get; set;
            }
            public string DVT { get; set; }

            public string? ChatLuong
            {
                get; set;
            }
            public double SLQuyDoi
            {
                get; set;
            }
            public double SLKHCan { get; set; }
            public double SLTonKho { get; set; }
            public double TonMB { get; set; }
            public double? DonHangChuaVe { get; set; }
            public double? SLCan { get; set; }
            public double? SLDatThem { get; set; }
            public string MaNCC { get; set; }
            public KeHoachThangItem CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<KeHoachThangItem>(json);
            }
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;

        }
        protected override async Task OnInitializedAsync()
        {
            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 70;
            heightgrid = string.Format("{0}px", height);
            lstsanpham = await ModelData.GetSanPham();
            lstkhachhang = await Model.ModelData.GetKhachHangSanSuatSP();
            //return base.OnInitializedAsync();
        }

        private async Task searchAsync()
        {

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = "";
            if (!string.IsNullOrEmpty(MaSPSelected))
            {
                if (dieukien == "")
                    dieukien += string.Format(" where MaSP=N'{0}'", MaSPSelected);
                else
                    dieukien += string.Format(" and MaSP=N'{0}'", MaSPSelected);
            }
            if (!string.IsNullOrEmpty(khachhangselected))
            {
                if (dieukien == "")
                    dieukien += string.Format(" where KhachHang=N'{0}'", khachhangselected);
                else
                    dieukien += string.Format(" and KhachHang=N'{0}'", khachhangselected);
            }
            //lstpara.Add(new ParameterDefine("@DateBegin", datebegin.ToString("MM/dd/yyyy 00:00")));
            //lstpara.Add(new ParameterDefine("@DateEnd", dateend.ToString("MM/dd/yyyy 23:59")));
            string sql = string.Format(@"use NVLDB  
							declare @lstsanpham nvarchar(max)
							declare @tblsp table(MaSP nvarchar(100),TenSP nvarchar(200),KhachHang nvarchar(200))
							insert into @tblsp
							exec SP.DataBase_ScansiaPacific2014.dbo.getTableformSqlString 
							@SQL_QUERY='select MaSP,TenSP,KhachHang from SanPham where KhachHang is not null' 

						

                             SELECT @lstsanpham = STUFF((
                                 SELECT ';' + CAST(MaSP AS NVARCHAR)
                                 FROM (select MaSP from @tblsp) as qry
                                 FOR XML PATH('')
                             ), 1, 1, '')
							
							
                            IF OBJECT_ID('tempdb..##tmpdinhmuctoancuc') IS NOT NULL
	                            DROP TABLE ##tmpdinhmuctoancuc
                            exec GetDinhMucNVL_SanPhamList @lstsanpham=@lstsanpham
							select * from ##tmpdinhmuctoancuc
                          {0}
							
                            Drop table ##tmpdinhmuctoancuc", dieukien);

            try
            {
                lstdinhmucall.Clear();

                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<DinhMucVatTuShow>>(json);
                    if (query != null)
                    {
                        lstdinhmucall.AddRange(query);
                        lstdata.Clear();
                        var lstgr = lstdinhmucall.GroupBy(p => new { MaSP = p.MaSP, TenSP = p.TenSP, KhachHang = p.KhachHang }).Select(p => new SanPhamShow { MaSP = p.Key.MaSP, TenSP = p.Key.TenSP, KhachHang = p.Key.KhachHang, SLSP = 1 }).ToList();
                        lstdata.AddRange(lstgr);


                    }
                    dxGrid.Reload();
                    //var query = JsonConvert.DeserializeObject<List<KeHoachThang_Show>>(json);
                    //lstdata.AddRange(query);
                }


                PanelVisible = false;
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));

                PanelVisible = false;
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
                //ShowProgress.CloseAwait();
            }
        }





        class KetquaResult
        {
            public int? Serial { get; set; }
            public double? SLCL { get; set; }

            public string? ketqua { get; set; }

            public string? ketquaexception { get; set; }

        }


        private void checkChanged(bool chk, KeHoachThang_Show keHoachThang_Show)
        {
            keHoachThang_Show.Chk = chk;
        }
        private void selecteditemnhomkehoach(DataDropDownList dataDropDownList)
        {


        }
        private async Task PrintdinhmucAsync()
        {
            if(lstdinhmucall.Count==0)
            {
               await searchAsync();
            }
            XtraRp_DinhMucNVL xtraRp_DinhMucNVL = new XtraRp_DinhMucNVL();
            //xtraRp_PhieuNhapKho.setGhiChu(ghichu);
            xtraRp_DinhMucNVL.DataSource = lstdinhmucall;
            await ModelAdmin.mainLayout.showreportAsync(xtraRp_DinhMucNVL);
        }
        private async Task PrintdinhmucSPAsync(SanPhamShow sanPhamShow)
        {

            XtraRp_DinhMucNVL xtraRp_DinhMucNVL = new XtraRp_DinhMucNVL();
            //xtraRp_PhieuNhapKho.setGhiChu(ghichu);
            xtraRp_DinhMucNVL.DataSource = lstdinhmucall.Where(p => p.MaSP == sanPhamShow.MaSP).ToList(); ;
            await ModelAdmin.mainLayout.showreportAsync(xtraRp_DinhMucNVL);
        }
        private async Task ShowMaSPAsync()
        {
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DinhMucSelectSanPham>(0);
                builder.CloseComponent();
            };
            await dxPopup.showAsync("Chọn sản phẩm");
            await dxPopup.ShowAsync();
        }


    }
}
