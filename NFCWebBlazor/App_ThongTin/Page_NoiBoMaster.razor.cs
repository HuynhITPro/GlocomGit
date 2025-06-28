using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.ComponentModel.DataAnnotations;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Page_NoiBoMaster
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        NvlNoiBoShow? nvlKhachHangcrr { get; set; }
        private async Task loaddatadropdownAsync()
        {
            try
            {

                CallAPI callAPI = new CallAPI();
                string sql = "USE [NVLDB]\r\nSELECT [MaNB] as [Name],[TenNB] as FullName\r\n FROM [NvlNoiBo]";

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
        public class NvlNoiBoShow
        {
            public int Serial { get; set; }
       
            public string MaNB { get; set; }

            public string NhomNB { get; set; }

            public string TenNB { get; set; }
            public string DiaChi { get; set; }
           
            public string TinhThanh { get; set; }
            
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
            public NvlNoiBoShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlNoiBoShow>(json);
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
            await Grid.ExportToXlsxAsync("ExportNoiBo");
        }
        public async Task AddItemAsync()
        {
           await dxFlyoutchucnang.CloseAsync();
            if (!await phanQuyenAccess.CreateKhachHang(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền thêm"));
                return;
            }
            NvlNoiBoShow khachHangNVLShow = new NvlNoiBoShow();
            khachHangNVLShow.MaNB = "";
            khachHangNVLShow.TenNB = "";


            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NoiBoAddItem>(0);
                builder.AddAttribute(1, "khachHangNVLShow", khachHangNVLShow);
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
          await  dxPopup.showAsync("THÊM MỚI MÃ NỘI BỘ");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
           await dxPopup.ShowAsync();
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
                builder.OpenComponent<Urc_NoiBoAddItem>(0);
                builder.AddAttribute(1, "khachHangNVLShow", nvlKhachHangcrr.CopyClass());
                builder.AddAttribute(1, "GotoMainForm", EventCallback.Factory.Create<NvlNoiBoShow>(this, GotoMainForm));
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
        public void Setclass(NvlNoiBoShow nvlKhachHang_set, NvlNoiBoShow nvlKhachHang_get)
        {
            nvlKhachHang_set.Serial = nvlKhachHang_get.Serial;
            nvlKhachHang_set.MaNB = nvlKhachHang_get.MaNB;
            nvlKhachHang_set.TenNB = nvlKhachHang_get.TenNB;
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


            string sql = string.Format(@"Use [NVLDB] Select * from NvlNoiBo as kh ");
            string dieukien = "";
            lstdata.Clear();
            if (txtkhachang.GetValue != null)
            {


                if (dieukien == "")
                {
                    dieukien += string.Format(" where kh.MaNB=N'{0}'", txtkhachang.GetValue.Name);
                }
                else
                {
                    dieukien += string.Format(" and kh.MaNB=N'{0}'", txtkhachang.GetValue.Name);
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
                    lstdata = JsonConvert.DeserializeObject<List<NvlNoiBoShow>>(json);
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
        private void GotoMainForm(NvlNoiBoShow khachHangNVLShow)
        {
            Setclass(nvlKhachHangcrr, khachHangNVLShow);
            Grid.SaveChangesAsync();
        }
        private async Task deleteAsync()
        {
            await dxFlyoutchucnang.CloseAsync();

            if (!await phanQuyenAccess.CreateMaKho(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa"));
                return;
            }

            bool ketqua = await dialogMsg.Show($"XÓA  {nvlKhachHangcrr.TenNB}???", $"Bạn có chắc muốn xóa  {nvlKhachHangcrr.TenNB}??");
            if (ketqua)
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlNoiBo_Delete";
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
        public async void ShowFlyout(NvlNoiBoShow nvlKhachHang)
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
