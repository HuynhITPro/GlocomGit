using BlazorBootstrap;
using DevExpress.Data.Filtering;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi;
using NFCWebBlazor.App_ThongTin;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;





namespace NFCWebBlazor.App_KeHoach
{
    public partial class Page_KeHoachThang_Master
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        private string LoaiKeHoach;
        public class KeHoachSP_Show
        {

            public int Serial { get; set; }
            [Required(ErrorMessage = "Mã kế hoạch không được để trống")]
            [StringLength(100, MinimumLength = 1, ErrorMessage = "Mã kế hoạch từ 1 đến 100 ký tự")]
            public string MaKHThang { get; set; }
            [Required(ErrorMessage = "Tên kế hoạch không được để trống")]
            [StringLength(100, MinimumLength = 1, ErrorMessage = "Tên kế hoạch từ 1 đến 100 ký tự")]
            public string TenKHThang { get; set; }
            public string UserInsert { get; set; }
            public string PathImgTao { get; set; }
            public string NguoiDuyet { get; set; }
            public string GhiChu { get; set; }
            [Required(ErrorMessage = "Nhà máy không được để trống")]
            [StringLength(100, MinimumLength = 1, ErrorMessage = "Nhà máy từ 1 đến 100 ký tự")]
            public string NhaMay { get; set; }
            public bool isChanged { get; set; } = false;
            public string LoaiKeHoach { get; set; }
            public bool checkUpdate { get; set; }
            public double? TyLe { get; set; }
            [Required(ErrorMessage = "Ngày Min không được bỏ trống")]

            public Nullable<System.DateTime> ThangMin { get; set; }
            [Required(ErrorMessage = "Ngày Max không được bỏ trống")]
            public Nullable<System.DateTime> ThangMax { get; set; }
            public Nullable<System.DateTime> NgayInsert { get; set; }
            public List<KeHoachThangItem_Show> lstKeHoachChiTiet;
            public KeHoachSP_Show CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<KeHoachSP_Show>(json);
            }
        }
        public class KeHoachThangItem_Show : INotifyPropertyChanged
        {
            public KeHoachThangItem_Show()
            {

            }
            public int Serial { get; set; }
            public Nullable<int> Serial_KHThang { get; set; }
           
            public string ArticleNumber { get; set; }
            public bool chk { get; set; } = false;
            public string MaSP { get; set; }
            public string TenSP { get; set; }
            
            public Nullable<int> SLSP { get; set; }
            public Nullable<double> SLConLai { get; set; }
            public string ColorHex { get; set; }
            private uint? _color { get; set; }
            public uint? Color
            {
                get { return _color; }
                set
                {
                    _color = value;
                    ColorHex = StaticClass.UIntToHtmlColor(_color);
                }
            }
            public Nullable<double> SLNhap { get; set; }
            public double? SLTonMB { get; set; } = 0;
            public Nullable<double> TyLe { get; set; }
            public string TenMau { get; set; }
            public string MaMau { get; set; }
            public string Type_Other { get; set; }
            public string UserInsert { get; set; }
            public string Err { get; set; }
            public string GhiChu { get; set; }
            public Nullable<System.DateTime> NgayInsert { get; set; }
            public string MaDHMua { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            public KeHoachThangItem_Show CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<KeHoachThangItem_Show>(json);
            }
        }
        private async Task loaddatadropdownAsync()
        {
            try
            {
                if (LoaiKeHoach == null)
                {
                    if (ModelAdmin.lstmenuitems != null)
                    {
                        MenuItem query = ModelAdmin.lstmenuitems.FirstOrDefault(p => p.NameItem.Equals(ModelAdmin.pageclickcurrent));
                        if (query != null)
                        {
                            if (query.Tag != null)
                                LoaiKeHoach = query.Tag.ToString();

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;
            }
        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (CheckQuyen)
                {

                }
            }
            return base.OnAfterRenderAsync(firstRender);
        }
        private async Task searchAsync()
        {
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = "";

            if (datebegin == null || dateend == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ngày để xem"));
                return;
            }
            dieukien = " where LoaiKeHoach=@LoaiKeHoach";
            lstpara.Add(new ParameterDefine("@LoaiKeHoach", LoaiKeHoach));

            if (dieukien == "")
                dieukien = " where NgayInsert>=@DateBegin and NgayInsert<=@DateEnd";
            else
                dieukien += " and NgayInsert>=@DateBegin and NgayInsert<=@DateEnd";
            lstpara.Add(new ParameterDefine("@DateBegin", datebegin.ToString("MM/dd/yyyy 00:00")));
            lstpara.Add(new ParameterDefine("@DateEnd", dateend.ToString("MM/dd/yyyy 23:59")));


            if (txtnhamay.GetValue != null)
            {
                if (dieukien == "")
                    dieukien = " where NhaMay=@NhaMay";
                else
                    dieukien += " and NhaMay=@NhaMay";
                lstpara.Add(new ParameterDefine("@NhaMay", txtnhamay.GetValue));
            }
            string sql = string.Format(@"use NVLDB SELECT [Serial],[MaKHThang],[TenKHThang],[UserInsert],[NhaMay],[NguoiDuyet],[NgayInsert],GhiChu,ThangMin,ThangMax
                                FROM [KeHoachThang] {0}", dieukien);

            try
            {
                if (lstdata == null)
                    lstdata = new List<KeHoachSP_Show>();
                else
                    lstdata.Clear();
                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<List<KeHoachSP_Show>>(json);
                    lstdata.AddRange(query);
                }
                Grid.AutoFitColumnWidths();
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

        public void saveAsync()
        {
            KeHoachSP_Show dataDropDownList = new KeHoachSP_Show();
            //renderFragment = builder =>
            //{
            //    builder.OpenComponent<Urc_NhomHangAddItem>(0);
            //    builder.AddAttribute(1, "dataDropDownList", dataDropDownList);
            //    //builder.AddAttribute(2, "GotoMainForm", GotoFormMain);
            //    //builder.OpenComponent(0, componentType);
            //    builder.CloseComponent();
            //};

            //dxPopup.show("THÊM MỚI NHÓM HÀNG");

            //dxPopup.ShowAsync();
            //PopupVisible = true;
        }
        private void AddMasterItem()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có quyền thêm"));
                return;
            }
            KeHoachSP_Show keHoachSP_Show = new KeHoachSP_Show();
            keHoachSP_Show.Serial = 0;
            keHoachSP_Show.LoaiKeHoach = LoaiKeHoach;
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_KeHoachThang_AddMaster>(0);
                builder.AddAttribute(1, "keHoachSP_Showcrr", keHoachSP_Show);
                builder.CloseComponent();
            };
            dxPopup.showAsync("THÊM KẾ HOẠCH THÁNG");

            dxPopup.ShowAsync();
        }

        public async Task KeHoachMasterAddItemAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có quyền thêm"));
                return;
            }
            KeHoachThangItem_Show keHoachThangItem_Show = new KeHoachThangItem_Show();
            keHoachThangItem_Show.Serial_KHThang = keHoachSP_Showcrr.Serial;
            keHoachThangItem_Show.Serial = 0;

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_KeHoachThang_ItemAdd>(0);
                builder.AddAttribute(1, "keHoachSP_Showcrr", keHoachSP_Showcrr);
                builder.AddAttribute(2, "keHoachThangItem_Showform", keHoachThangItem_Show.CopyClass());

                builder.AddAttribute(3, "GotoMainForm", EventCallback.Factory.Create<KeHoachSP_Show>(this, InitListItem));
                builder.CloseComponent();
            };
            dxPopup.showAsync("THÊM SẢN PHẨM");

            dxPopup.ShowAsync();

        }
        private void RefreshList()
        {
            Grid.SaveChangesAsync();

        }
        private async Task ImportTuFileAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(keHoachSP_Showcrr.UserInsert, ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không phải người tạo, nên không có quyền thêm"));
                return;
            }
            KeHoachThangItem_Show nvlKeHoachMuaHangItemShow = new KeHoachThangItem_Show();
            nvlKeHoachMuaHangItemShow.Serial_KHThang = keHoachSP_Showcrr.Serial;
            nvlKeHoachMuaHangItemShow.Serial = 0;

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_KeHoachThang_Import>(0);
                builder.AddAttribute(1, "keHoachSP_Showcrr", keHoachSP_Showcrr);

                //builder.AddAttribute(4, "GotoMainForm", EventCallback.Factory.Create(this, RefreshRowCurrent));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            dxPopup.showAsync("THÊM SẢN PHẨM");

            dxPopup.ShowAsync();
        }
        private async Task CreateDeNghiDongVi()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            //if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(keHoachSP_Showcrr.UserInsert, ModelAdmin.users))
            //{
            //    toastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không phải người tạo, nên không có quyền thêm"));
            //    return;
            //}
            KeHoachThangItem_Show nvlKeHoachMuaHangItemShow = new KeHoachThangItem_Show();
            nvlKeHoachMuaHangItemShow.Serial_KHThang = keHoachSP_Showcrr.Serial;
            nvlKeHoachMuaHangItemShow.Serial = 0;

            renderFragment = builder =>
            {
                builder.OpenComponent<Page_TaoDeNghiDongVi>(0);
                builder.AddAttribute(1, "keHoachSP_Showcrr", keHoachSP_Showcrr);

                //builder.AddAttribute(4, "GotoMainForm", EventCallback.Factory.Create(this, RefreshRowCurrent));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            dxPopup.showAsync("Tạo đề nghị đóng vỉ");

            dxPopup.ShowAsync();
        }
        private void InitListItem(KeHoachSP_Show keHoachSP_Show)
        {
            keHoachSP_Show.lstKeHoachChiTiet = null;
            Grid.CollapseAllDetailRows();
            Grid.Reload();

        }
        public void setClass(KeHoachSP_Show NvlKeHoachThang_set, KeHoachSP_Show NvlKeHoachThang_get)
        {
            //NvlKeHoachThang_set.Serial = NvlKeHoachThang_get.Serial;
            NvlKeHoachThang_set.MaKHThang = NvlKeHoachThang_get.MaKHThang;
            NvlKeHoachThang_set.TenKHThang = NvlKeHoachThang_get.TenKHThang;
            NvlKeHoachThang_set.UserInsert = NvlKeHoachThang_get.UserInsert;
            NvlKeHoachThang_set.NguoiDuyet = NvlKeHoachThang_get.NguoiDuyet;
            NvlKeHoachThang_set.NhaMay = NvlKeHoachThang_get.NhaMay;
            NvlKeHoachThang_set.ThangMin = NvlKeHoachThang_get.ThangMin;
            NvlKeHoachThang_set.ThangMax = NvlKeHoachThang_get.ThangMax;
            NvlKeHoachThang_set.GhiChu = NvlKeHoachThang_get.GhiChu;
        }

        private async void GotoFormMain(KeHoachSP_Show nvlNhomHang)
        {
            var query = lstdata.Where(x => x.Serial == nvlNhomHang.Serial).FirstOrDefault();
            if (query != null)
            {
                setClass(query, nvlNhomHang);
                await Grid.SaveChangesAsync();
            }
        }
        private async Task deleteAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!phanQuyenAccess.CheckDelete(keHoachSP_Showcrr.UserInsert, ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa"));
                return;
            }

            bool ketqua = await dialogMsg.Show($"XÓA  {keHoachSP_Showcrr.TenKHThang}???", $"Bạn có chắc muốn xóa Serial {keHoachSP_Showcrr.TenKHThang}??");
            if (ketqua)
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.KeHoachThang_delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();

                lstpara.Add(new ParameterDefine("@Serial", keHoachSP_Showcrr.Serial));

                try
                {
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);

                    if (json != "")
                    {
                        try
                        {
                            var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                            if (query[0].ketqua == "OK")
                            {
                                toastService.Notify(new ToastMessage(ToastType.Success, $"Xóa thành công"));
                                lstdata.Remove(keHoachSP_Showcrr);
                                Grid.Reload();

                            }
                            else
                            {
                                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception)));
                            }
                        }
                        catch (Exception ex)
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                }
            }
        }

        private async Task EditMasterAsync()
        {

            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền sửa"));
                return;
            }
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            KeHoachSP_Show nvlNhomHangcopy = new KeHoachSP_Show();
            nvlNhomHangcopy = keHoachSP_Showcrr.CopyClass();
            //setClass(nvlNhomHangcopy, nvlNhomHang);

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_KeHoachThang_AddMaster>(0);

                builder.AddAttribute(1, "keHoachSP_Showcrr", keHoachSP_Showcrr.CopyClass());
                builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<KeHoachSP_Show>(this, GotoFormMain));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            dxPopup.showAsync("CHỈNH SỬA NHÓM HÀNG");
            dxPopup.ShowAsync();


        }
        public async void ShowFlyout(KeHoachSP_Show keHoachSP_Show)
        {
            await dxFlyoutchucnang.CloseAsync();
            keHoachSP_Showcrr = keHoachSP_Show;
            idflychucnang = "#" + idelement(keHoachSP_Showcrr.Serial);

            IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();

        }


    }

}
