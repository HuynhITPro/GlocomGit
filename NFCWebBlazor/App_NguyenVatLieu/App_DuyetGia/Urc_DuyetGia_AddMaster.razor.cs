using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using Microsoft.IdentityModel.Tokens;
using static NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.Page_DuyetGiaMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public partial class Urc_DuyetGia_AddMaster
    {
        [Inject] PreloadService PreloadService { get; set; }

        [Inject] ToastService toastService { get; set; }
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                editContext = new EditContext(nvlDuyetGiaShowcrr);
                editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
                lstnhacungcap = await Model.ModelData.Getlstnhacungcap();
                lstnguoidenghi = await Model.ModelData.Getlstusers();

                if (nvlDuyetGiaShowcrr.Ngay == null)
                    nvlDuyetGiaShowcrr.Ngay = DateTime.Now;
                NguoiDeNghiSelected = lstnguoidenghi.FirstOrDefault(p => p.UsersName == ModelAdmin.users.UsersName);
                if (nvlDuyetGiaShowcrr.Serial > 0)
                {
                    EnableEdit = true;
                }

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
            base.OnInitialized();
        }

        bool checkset = false;
        // bool checkset = false;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!checkset)
            {
                if (lstnhacungcap != null)
                {
                    checkset = true;
                    if (nvlDuyetGiaShowcrr.Serial != 0)
                    {


                        StateHasChanged();
                    }


                }
            }
            return base.OnAfterRenderAsync(firstRender);
        }


        private async Task saveAsync()
        {
            if (!checklogic())
                return;
            try
            {

                CallAPI callAPI = new CallAPI();
            
                string sql = "NVLDB.dbo.NvlDuyetGia_Insert";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@NguoiDN", nvlDuyetGiaShowcrr.NguoiDN));
                lstpara.Add(new ParameterDefine("@LoaiDuyetGia", nvlDuyetGiaShowcrr.LoaiDuyetGia));
                lstpara.Add(new ParameterDefine("@DVT", nvlDuyetGiaShowcrr.DVT));
                lstpara.Add(new ParameterDefine("@LyDo", nvlDuyetGiaShowcrr.LyDo));
                lstpara.Add(new ParameterDefine("@PhongBan", nvlDuyetGiaShowcrr.PhongBan));
                lstpara.Add(new ParameterDefine("@NhaMay", nvlDuyetGiaShowcrr.NhaMay));
                lstpara.Add(new ParameterDefine("@DuAnHanMuc", nvlDuyetGiaShowcrr.DuAnHanMuc));
                lstpara.Add(new ParameterDefine("@CuocVanChuyen", nvlDuyetGiaShowcrr.CuocVanChuyen));
                lstpara.Add(new ParameterDefine("@ThueGTGT", nvlDuyetGiaShowcrr.ThueGTGT));
                lstpara.Add(new ParameterDefine("@KetQua", nvlDuyetGiaShowcrr.KetQua));
                lstpara.Add(new ParameterDefine("@ChonNhaCungCap", nvlDuyetGiaShowcrr.ChonNhaCungCap));
                lstpara.Add(new ParameterDefine("@UserInsert", Model.ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@Ngay", nvlDuyetGiaShowcrr.Ngay));
                lstpara.Add(new ParameterDefine("@GhiChu", nvlDuyetGiaShowcrr.GhiChu));
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "LƯU THÀNH CÔNG"));
                        reset();
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));
                        reset();
                    }
                  
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi " + ex.Message));
                //msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
        }
        private async Task updateAsync()
        {
            if (!checklogic())
            {
                StateHasChanged();
                return;
            }
            try
            {
                if (!phanQuyenAccess.CheckDelete(nvlDuyetGiaShowcrr.UserInsert, Model.ModelAdmin.users))
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không phải người tạo đề nghị này nên bạn không có quyền sửa"));
                    return;
                }
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlDuyetGia_Update";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlDuyetGiaShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@NguoiDN", nvlDuyetGiaShowcrr.NguoiDN));
                lstpara.Add(new ParameterDefine("@LoaiDuyetGia", nvlDuyetGiaShowcrr.LoaiDuyetGia));
                lstpara.Add(new ParameterDefine("@DVT", nvlDuyetGiaShowcrr.DVT));
                lstpara.Add(new ParameterDefine("@LyDo", nvlDuyetGiaShowcrr.LyDo));
                lstpara.Add(new ParameterDefine("@PhongBan", nvlDuyetGiaShowcrr.PhongBan));
                lstpara.Add(new ParameterDefine("@NhaMay", nvlDuyetGiaShowcrr.NhaMay));
                lstpara.Add(new ParameterDefine("@DuAnHanMuc", nvlDuyetGiaShowcrr.DuAnHanMuc));
                lstpara.Add(new ParameterDefine("@CuocVanChuyen", nvlDuyetGiaShowcrr.CuocVanChuyen));
                lstpara.Add(new ParameterDefine("@ThueGTGT", nvlDuyetGiaShowcrr.ThueGTGT));
                lstpara.Add(new ParameterDefine("@KetQua", nvlDuyetGiaShowcrr.KetQua));
                lstpara.Add(new ParameterDefine("@ChonNhaCungCap", nvlDuyetGiaShowcrr.ChonNhaCungCap));
                lstpara.Add(new ParameterDefine("@UserInsert", Model.ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@Ngay", nvlDuyetGiaShowcrr.Ngay));
                lstpara.Add(new ParameterDefine("@GhiChu", nvlDuyetGiaShowcrr.GhiChu));

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Sửa thành công")));
                        if (AfterEdit.HasDelegate)
                        {
                            await AfterEdit.InvokeAsync(nvlDuyetGiaShowcrr);
                        }

                        reset();
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception)));
                        //msgBox.Show(string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[1].ketquaexception), IconMsg.iconssuccess);

                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi:" + ex.Message)));
            }
        }
        private void reset()
        {

            nvlDuyetGiaShowcrr.CuocVanChuyen = null;
            nvlDuyetGiaShowcrr.ThueGTGT = null;
            nvlDuyetGiaShowcrr.GhiChu = "";

            StateHasChanged();

        }
        private ValidationMessageStore validationMessages;
        private void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {
            if(validationMessages == null)
                validationMessages = new ValidationMessageStore(editContext);

            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();

            // Thực hiện kiểm tra tùy chỉnh, ví dụ kiểm tra giá trị null cho KhuVuc
            if (string.IsNullOrEmpty(nvlDuyetGiaShowcrr.LoaiDuyetGia))
            {
                validationMessages.Add(() => nvlDuyetGiaShowcrr.LoaiDuyetGia, "Chọn loại duyệt");
            }

            if (string.IsNullOrEmpty(nvlDuyetGiaShowcrr.NguoiDN))
            {
                validationMessages.Add(() => nvlDuyetGiaShowcrr.NguoiDN, "Chọn người đề nghị");
            }
            if (string.IsNullOrEmpty(nvlDuyetGiaShowcrr.PhongBan))
            {
                validationMessages.Add(() => nvlDuyetGiaShowcrr.PhongBan, "Chọn phòng ban");
            }
            if (string.IsNullOrEmpty(nvlDuyetGiaShowcrr.LyDo))
            {
                validationMessages.Add(() => nvlDuyetGiaShowcrr.LyDo, "Vui lòng chọn Lý do");
            }
            if (string.IsNullOrEmpty(nvlDuyetGiaShowcrr.DVT))
            {
                validationMessages.Add(() => nvlDuyetGiaShowcrr.DVT, "Chọn đơn vị tính");
            }
            if (nvlDuyetGiaShowcrr.Ngay == null)
            {
                validationMessages.Add(() => nvlDuyetGiaShowcrr.Ngay, "Ngày đề nghị không được để trống.");
            }
            if (string.IsNullOrEmpty(nvlDuyetGiaShowcrr.CuocVanChuyen))
            {
                validationMessages.Add(() => nvlDuyetGiaShowcrr.CuocVanChuyen, "Chọn cước vận chuyển");
            }
            if (string.IsNullOrEmpty(nvlDuyetGiaShowcrr.ThueGTGT))
            {
                validationMessages.Add(() => nvlDuyetGiaShowcrr.ThueGTGT, "Nhập thiếu thuế GTGT");
            }
            // Lưu kết quả validation
            editContext.NotifyValidationStateChanged();
        }
        private bool checklogic()
        {
            if (NguoiDeNghiSelected == null)
            {
                nvlDuyetGiaShowcrr.NguoiDN = null;
            }
            else
                nvlDuyetGiaShowcrr.NguoiDN = NguoiDeNghiSelected.UsersName;
            //editContext = new EditContext(nvlDuyetGiaShowcrr);
            //editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
            return editContext.Validate();
        }
        public async void ShowFlyout()
        {
            await dxFlyoutchucnang.CloseAsync();


           // IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();


        }



    }

}
