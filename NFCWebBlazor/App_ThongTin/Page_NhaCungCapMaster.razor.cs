using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.ComponentModel.DataAnnotations;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Page_NhaCungCapMaster
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        NVLNhaCungCapShow? nvlKhachHangcrr { get; set; }
        private async Task loaddatadropdownAsync()
        {
            try
            {

                CallAPI callAPI = new CallAPI();
                string sql = "USE [NVLDB]\r\nSELECT [MaNCC] as [Name],[TenNCC] as FullName\r\n FROM [NvlNhaCungCap]";

                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {

                    lstkhachhang = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);

                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
            }
            finally
            {

            }


        }
        public class NVLNhaCungCapShow
        {
            public int Serial { get; set; }
            //[Required(ErrorMessage = "Mã nhà cung cấp không được để trống")]
            //[StringLength(100, ErrorMessage = "Mã nhà cung cấp từ 1 đến 100 ký tự")]
            public string MaNCC { get; set; }
            public string NhomNCC { get; set; }

            //[Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
            //[StringLength(100, ErrorMessage = "Tên nhà cung cấp từ 1 đến 100 ký tự")]
            public string TenNCC { get; set; }
            public string DiaChi { get; set; }
            //[Required(ErrorMessage = "Tỉnh thành không được để trống")]
            //[StringLength(100, ErrorMessage = "Tỉnh thành từ 1 đến 100 ký tự")]
            public string TinhThanh { get; set; }
            //[Required(ErrorMessage = "Quốc gia không được để trống")]
            //[StringLength(100, ErrorMessage = "Quốc gia từ 1 đến 100 ký tự")]
            public string QuocGia { get; set; }
            public string Email { get; set; }
            public string DienThoaiBan { get; set; }
            public string DiDong { get; set; }
            public string Website { get; set; }
            public string DaiDienPL { get; set; }
            public string MaNganHang { get; set; }
            public string SoTK { get; set; }
            public string TenTK { get; set; }
            public string TenNH { get; set; }
            //[Required(ErrorMessage = "Mã số thuế không được để trống")]
            //[StringLength(100, ErrorMessage = "Mã số thuế từ 1 đến 100 ký tự")]
            public string MaSoThue { get; set; }
            public string TinhTrangThue { get; set; }
            public string LoaiHinhKinhDoanh { get; set; }
            public string MaChungChi { get; set; }
            public Nullable<DateTime> NgaySoatXet { get; set; }
            public string NguoiSoatXet { get; set; }
            public string SwiftCode { get; set; }
            public string NhomMatHang { get; set; }
            public string TinhTrang { get; set; }
            public string GhiChu { get; set; }
            public string UserInsert { get; set; }
            public DateTime NgayInsert { get; set; }
            public string PathImg { get; set; }
            public NVLNhaCungCapShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NVLNhaCungCapShow>(json);
            }
            public List<FileHoSoGroup> lstfilehoso
            {
                get;
                set;

            }
            public void setlstfilehoso(List<FileHoSoGroup> lst)
            {
                lstfilehoso = lst;
            }

        }
        async Task ExportXlsx_Click()
        {
            await Grid.ExportToXlsxAsync("ExportNhaCungCap");
        }
        public async Task AddItemAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            if (!await phanQuyenAccess.CreateNhaCungCap(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền thêm"));
                return;
            }
            NVLNhaCungCapShow khachHangNVLShow = new NVLNhaCungCapShow();
            khachHangNVLShow.MaNCC = "";
            khachHangNVLShow.TenNCC = "";
            khachHangNVLShow.NhomNCC = "Nhà cung cấp trong nước";
            khachHangNVLShow.QuocGia= "Việt Nam";
            khachHangNVLShow.TinhTrangThue = "Đang hoạt động";
           renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NhaCungCapAddItem>(0);
                builder.AddAttribute(1, "khachHangNVLShow", khachHangNVLShow);
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
           await dxPopup.showAsync("THÊM MỚI NHÀ CUNG CẤP");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
          await  dxPopup.ShowAsync();
        }
        public async Task EditItemAsync()
        {

          await  dxFlyoutchucnang.CloseAsync();
            if (!await phanQuyenAccess.CreateKhachHang(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền sửa"));
                return;
            }

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NhaCungCapAddItem>(0);
                builder.AddAttribute(1, "khachHangNVLShow", nvlKhachHangcrr.CopyClass());
                builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<NVLNhaCungCapShow>(this, GotoMainForm));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
           await dxPopup.showAsync("SỬA THÔNG TIN");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
          await  dxPopup.ShowAsync();
        }
        public void Setclass(NVLNhaCungCapShow nvlKhachHang_set, NVLNhaCungCapShow nvlKhachHang_get)
        {
            nvlKhachHang_set.Serial = nvlKhachHang_get.Serial;
            nvlKhachHang_set.MaNCC = nvlKhachHang_get.MaNCC;
            nvlKhachHang_set.TenNCC = nvlKhachHang_get.TenNCC;
            nvlKhachHang_set.MaSoThue = nvlKhachHang_get.MaSoThue;
            nvlKhachHang_set.DiaChi = nvlKhachHang_get.DiaChi;
            nvlKhachHang_set.TinhThanh = nvlKhachHang_get.TinhThanh;
            nvlKhachHang_set.QuocGia = nvlKhachHang_get.QuocGia;
            nvlKhachHang_set.DienThoaiBan = nvlKhachHang_get.DienThoaiBan;
            nvlKhachHang_set.DiDong = nvlKhachHang_get.DiDong;
            nvlKhachHang_set.Email = nvlKhachHang_get.Email;
            nvlKhachHang_set.Website = nvlKhachHang_get.Website;
            nvlKhachHang_set.MaNganHang = nvlKhachHang_get.MaNganHang;
            nvlKhachHang_set.DaiDienPL = nvlKhachHang_get.DaiDienPL;
            nvlKhachHang_set.MaChungChi = nvlKhachHang_get.MaChungChi;
            nvlKhachHang_set.TinhTrangThue = nvlKhachHang_get.TinhTrangThue;
            nvlKhachHang_set.SwiftCode = nvlKhachHang_get.SwiftCode;
            nvlKhachHang_set.LoaiHinhKinhDoanh = nvlKhachHang_get.LoaiHinhKinhDoanh;
            nvlKhachHang_set.NhomMatHang = nvlKhachHang_get.NhomMatHang;
            nvlKhachHang_set.GhiChu = nvlKhachHang_get.GhiChu;
            nvlKhachHang_set.NguoiSoatXet = nvlKhachHang_get.NguoiSoatXet;
            nvlKhachHang_set.NgaySoatXet = nvlKhachHang_get.NgaySoatXet;
            nvlKhachHang_set.TinhTrang = nvlKhachHang_get.TinhTrang;
            nvlKhachHang_set.UserInsert = nvlKhachHang_get.UserInsert;
        }
        public async void searchAsync()
        {


            string sql = string.Format(@"Use [NVLDB] Select * from NvlNhaCungCap as kh ");
            string dieukien = "";
            lstdata.Clear();
            if (txtkhachang.GetValue != null)
            {


                if (dieukien == "")
                {
                    dieukien += string.Format(" where kh.MaNCC=N'{0}'", txtkhachang.GetValue.Name);
                }
                else
                {
                    dieukien += string.Format(" and kh.MaNCC=N'{0}'", txtkhachang.GetValue.Name);
                }
            }
            sql = sql + dieukien;
            PanelVisible = true;
            try
            {

                CallAPI callAPI = new CallAPI();


                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {
                    lstdata = JsonConvert.DeserializeObject<List<NVLNhaCungCapShow>>(json);
                    PanelVisible = false;
                    //Grid.Reload();
                }


            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));

            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }



        }
        private void GotoMainForm(NVLNhaCungCapShow khachHangNVLShow)
        {
            Setclass(nvlKhachHangcrr, khachHangNVLShow);
            Grid.SaveChangesAsync();
        }
        private async Task deleteAsync()
        {
            await dxFlyoutchucnang.CloseAsync();

            if (!await phanQuyenAccess.CreateNhaCungCap(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa"));
                return;
            }

            bool ketqua = await dialogMsg.Show($"XÓA  {nvlKhachHangcrr.TenNCC}???", $"Bạn có chắc muốn xóa  {nvlKhachHangcrr.TenNCC}??");
            if (ketqua)
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlNhaCungCap_Delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();

                lstpara.Add(new ParameterDefine("@Serial", nvlKhachHangcrr.Serial));

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
                                lstdata.Remove(nvlKhachHangcrr);
                                Grid.SaveChangesAsync();

                            }
                            else
                            {
                                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketquaexception));
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
        public async void ShowFlyout(NVLNhaCungCapShow nvlKhachHang)
        {
            await dxFlyoutchucnang.CloseAsync();
            //CurrentEmployee = employee;
            nvlKhachHangcrr = nvlKhachHang;
            idflychucnang = "#" + idelement(nvlKhachHangcrr.Serial);
            IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();


        }
       
    }

}
