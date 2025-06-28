using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_ThongTin.Page_NoiBoMaster;


namespace NFCWebBlazor.App_ThongTin
{
    public partial class Urc_NoiBoAddItem
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        private bool checklogic(NvlNoiBoShow khachHangNVLShow)
        {
            return editContext.Validate();


        }
        protected override Task OnInitializedAsync()
        {
            editContext = new EditContext(khachHangNVLShow);
            editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
            visibleedit = !string.IsNullOrEmpty(khachHangNVLShow.MaNB);
            return base.OnInitializedAsync();
        }
        ValidationMessageStore validationMessages { get; set; }
        bool checksave = true;//Gán biến để tránh trường hợp mạng lag API gọi lại 2 lần
        private async void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {
            if (validationMessages == null)
                validationMessages = new ValidationMessageStore(editContext);
            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();
            if (string.IsNullOrEmpty(khachHangNVLShow.MaNB))
            {
                validationMessages.Add(() => khachHangNVLShow.MaNB, "Mã nội bộ không được để trống");


            }
            if (string.IsNullOrEmpty(khachHangNVLShow.TenNB))
            {
                validationMessages.Add(() => khachHangNVLShow.TenNB, "Tên nội bộ không được để trống");

            }
            if (string.IsNullOrEmpty(khachHangNVLShow.NhomNB))
            {
                validationMessages.Add(() => khachHangNVLShow.NhomNB, "Nhóm nội bộ không được để trống.");
            }
            //Thực hiện kiểm tra tùy chỉnh, ví dụ kiểm tra giá trị null cho KhuVuc
            if (string.IsNullOrEmpty(khachHangNVLShow.TinhThanh))
            {
                validationMessages.Add(() => khachHangNVLShow.TinhThanh, "Tỉnh thành không được để trống.");
            }
            if (string.IsNullOrEmpty(khachHangNVLShow.QuocGia))
            {
                validationMessages.Add(() => khachHangNVLShow.QuocGia, "Quốc gia không được để trống.");
            }
            if (string.IsNullOrEmpty(khachHangNVLShow.MaSoThue))
            {
                validationMessages.Add(() => khachHangNVLShow.MaSoThue, "Mã số thuế không được để trống.");
            }

            editContext.NotifyValidationStateChanged();
        }
        private async Task saveAsync()
        {
            if (!await phanQuyenAccess.CreateNoiBo(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền tạo mã nội bộ"));

                return;
            }
            if (!checklogic(khachHangNVLShow))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Vui lòng kiểm tra lại thông tin nhập"));
                return;
            }

            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlNoiBo_Insert";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@MaNB", khachHangNVLShow.MaNB));
            lstpara.Add(new ParameterDefine("@TenNB", khachHangNVLShow.TenNB));
            lstpara.Add(new ParameterDefine("@MaSoThue", khachHangNVLShow.MaSoThue));
            lstpara.Add(new ParameterDefine("@NhomNB", khachHangNVLShow.NhomNB));
            lstpara.Add(new ParameterDefine("@SoTK", khachHangNVLShow.SoTK));
            lstpara.Add(new ParameterDefine("@TenTK", khachHangNVLShow.TenTK));
            lstpara.Add(new ParameterDefine("@DiaChi", khachHangNVLShow.DiaChi));
            lstpara.Add(new ParameterDefine("@TinhThanh", khachHangNVLShow.TinhThanh));
            lstpara.Add(new ParameterDefine("@QuocGia", khachHangNVLShow.QuocGia));
            lstpara.Add(new ParameterDefine("@DienThoaiBan", khachHangNVLShow.DienThoaiBan));
            lstpara.Add(new ParameterDefine("@DiDong", khachHangNVLShow.DiDong));
            lstpara.Add(new ParameterDefine("@Email", khachHangNVLShow.Email));
            lstpara.Add(new ParameterDefine("@Website", khachHangNVLShow.Website));
            lstpara.Add(new ParameterDefine("@MaNganHang", khachHangNVLShow.MaNganHang));
            lstpara.Add(new ParameterDefine("@DaiDienPL", khachHangNVLShow.DaiDienPL));
            lstpara.Add(new ParameterDefine("@MaChungChi", khachHangNVLShow.MaChungChi));
            lstpara.Add(new ParameterDefine("@TinhTrangThue", khachHangNVLShow.TinhTrangThue));
            lstpara.Add(new ParameterDefine("@SwiftCode", khachHangNVLShow.SwiftCode));
            lstpara.Add(new ParameterDefine("@LoaiHinhKinhDoanh", khachHangNVLShow.LoaiHinhKinhDoanh));
            lstpara.Add(new ParameterDefine("@NhomMatHang", khachHangNVLShow.NhomMatHang));
            lstpara.Add(new ParameterDefine("@GhiChu", khachHangNVLShow.GhiChu));
            lstpara.Add(new ParameterDefine("@UserInsert", Model.ModelAdmin.users.UsersName));



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
                            toastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                            reset();
                            lstpara.Clear();
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
        private async Task updateAsync()
        {
            if (!await phanQuyenAccess.CreateKhachHang(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền sửa Khách hàng"));

                return;
            }
            if (!checklogic(khachHangNVLShow))
            {
                return;
            }

            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlNoiBo_Update";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();

            lstpara.Add(new ParameterDefine("@Serial", khachHangNVLShow.Serial));
            lstpara.Add(new ParameterDefine("@MaNB", khachHangNVLShow.MaNB));
            lstpara.Add(new ParameterDefine("@NhomNB", khachHangNVLShow.NhomNB));
            lstpara.Add(new ParameterDefine("@SoTK", khachHangNVLShow.SoTK));
            lstpara.Add(new ParameterDefine("@TenTK", khachHangNVLShow.TenTK));
            lstpara.Add(new ParameterDefine("@TenNB", khachHangNVLShow.TenNB));
            lstpara.Add(new ParameterDefine("@MaSoThue", khachHangNVLShow.MaSoThue));
            lstpara.Add(new ParameterDefine("@DiaChi", khachHangNVLShow.DiaChi));
            lstpara.Add(new ParameterDefine("@TinhThanh", khachHangNVLShow.TinhThanh));
            lstpara.Add(new ParameterDefine("@QuocGia", khachHangNVLShow.QuocGia));
            lstpara.Add(new ParameterDefine("@DienThoaiBan", khachHangNVLShow.DienThoaiBan));
            lstpara.Add(new ParameterDefine("@DiDong", khachHangNVLShow.DiDong));
            lstpara.Add(new ParameterDefine("@Email", khachHangNVLShow.Email));
            lstpara.Add(new ParameterDefine("@Website", khachHangNVLShow.Website));
            lstpara.Add(new ParameterDefine("@MaNganHang", khachHangNVLShow.MaNganHang));
            lstpara.Add(new ParameterDefine("@DaiDienPL", khachHangNVLShow.DaiDienPL));
            lstpara.Add(new ParameterDefine("@MaChungChi", khachHangNVLShow.MaChungChi));
            lstpara.Add(new ParameterDefine("@TinhTrangThue", khachHangNVLShow.TinhTrangThue));
            lstpara.Add(new ParameterDefine("@SwiftCode", khachHangNVLShow.SwiftCode));
            lstpara.Add(new ParameterDefine("@LoaiHinhKinhDoanh", khachHangNVLShow.LoaiHinhKinhDoanh));
            lstpara.Add(new ParameterDefine("@NhomMatHang", khachHangNVLShow.NhomMatHang));
            lstpara.Add(new ParameterDefine("@GhiChu", khachHangNVLShow.GhiChu));
            lstpara.Add(new ParameterDefine("@NguoiSoatXet", khachHangNVLShow.NguoiSoatXet));
            lstpara.Add(new ParameterDefine("@NgaySoatXet", khachHangNVLShow.NgaySoatXet));
            lstpara.Add(new ParameterDefine("@TinhTrang", khachHangNVLShow.TinhTrang));
            lstpara.Add(new ParameterDefine("@UserInsert", khachHangNVLShow.UserInsert));
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
                            toastService.Notify(new ToastMessage(ToastType.Success, $"Sửa thành công"));
                            GotoMainForm.InvokeAsync(khachHangNVLShow);
                            //reset();
                            lstpara.Clear();
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

}
