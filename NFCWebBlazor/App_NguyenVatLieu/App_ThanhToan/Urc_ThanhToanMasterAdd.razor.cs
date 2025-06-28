using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;

using NFCWebBlazor.Model;

namespace NFCWebBlazor.App_NguyenVatLieu.App_ThanhToan
{
    public partial class Urc_ThanhToanMasterAdd
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        bool CheckQuyen = false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                editContext = new EditContext(nvlThanhToanShowcrr);
                editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
                CheckQuyen = await phanQuyenAccess.CreateThanhToan(Model.ModelAdmin.users);
                await loadAsync();
                if (nvlThanhToanShowcrr.Serial > 0)
                {
                    EnableEdit = true;
                }
                // StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load form ke hoach master " + ex.Message);
            }
            finally
            {
            }
        }
        bool checkset = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

               

            }
            if (lstnoigiaonhan != null && lstlydo != null)
            {
                if (!checkset)
                {
                    checkset = true;

                    StateHasChanged();
                }
            }
            //return base.OnAfterRenderAsync(firstRender);
        }
        private async Task loadAsync()
        {
            
           
            var queryngn = await Model.ModelData.Getlstnoigiaonhan();

            lstnoigiaonhan = queryngn.ToList();
            lstnoithanhtoan =  queryngn.Where(p =>  p.TypeName == "NB").ToList();
        }

        private void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {

            var validationMessages = new ValidationMessageStore(editContext);

            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();

            // Thực hiện kiểm tra tùy chỉnh, ví dụ kiểm tra giá trị null cho KhuVuc
            if (string.IsNullOrEmpty(nvlThanhToanShowcrr.LyDo))
            {
                validationMessages.Add(() => nvlThanhToanShowcrr.LyDo, "Lý do không được để trống.");
            }
            if (string.IsNullOrEmpty(nvlThanhToanShowcrr.MaGN))
            {
                validationMessages.Add(() => nvlThanhToanShowcrr.MaGN, "Mã nhà cung cấp không được để trống.");
            }
           
            if (string.IsNullOrEmpty(nvlThanhToanShowcrr.MaCTTT))
            {
                validationMessages.Add(() => nvlThanhToanShowcrr.MaCTTT, "Mã thanh toán không được để trống.");
            }
            if (nvlThanhToanShowcrr.Ngay == null)
            {
                validationMessages.Add(() => nvlThanhToanShowcrr.Ngay, "Ngày chứng từ không được để trống.");
            }
            // Lưu kết quả validation
            editContext.NotifyValidationStateChanged();
        }
        private bool checklogic()
        {
            editContext = new EditContext(nvlThanhToanShowcrr);
            editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
            return editContext.Validate();

        }


        private async Task saveAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không có quyền sử dụng chức năng này"));
                return;
            }
            if (!checklogic())
                return;
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlThanhToan_Insert";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlThanhToanShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@NoiTT", nvlThanhToanShowcrr.NoiTT));
                lstpara.Add(new ParameterDefine("@MaGN", nvlThanhToanShowcrr.MaGN));
                lstpara.Add(new ParameterDefine("@MaCTTT", nvlThanhToanShowcrr.MaCTTT));
                lstpara.Add(new ParameterDefine("@LyDo", nvlThanhToanShowcrr.LyDo));
                lstpara.Add(new ParameterDefine("@Ngay", nvlThanhToanShowcrr.Ngay));
                lstpara.Add(new ParameterDefine("@DienGiai", nvlThanhToanShowcrr.DienGiai));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@UserUp", nvlThanhToanShowcrr.UserUp));
                lstpara.Add(new ParameterDefine("@type_", "-1"));
                lstpara.Add(new ParameterDefine("@flag", 0));
                lstpara.Add(new ParameterDefine("@sign_type", 1));
                lstpara.Add(new ParameterDefine("@NguoiXacNhan", nvlThanhToanShowcrr.NguoiXacNhan));
                lstpara.Add(new ParameterDefine("@NgayXacNhan", nvlThanhToanShowcrr.NgayXacNhan));

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua=="OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "LƯU THÀNH CÔNG"));
                        reset();

                        //if (AfterSave.HasDelegate)
                        //{
                        //    await AfterSave.InvokeAsync(nvlThanhToanShowcrr);
                        //}
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));
                    }
                  
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Lỗi:" + ex.Message);
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi " + ex.Message));
                //msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
        }
        private async Task updateAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không có quyền sử dụng chức năng này"));
                return;
            }
            if (!checklogic())
                return;
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlThanhToan_Update";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlThanhToanShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@NoiTT", nvlThanhToanShowcrr.NoiTT));
                lstpara.Add(new ParameterDefine("@MaGN", nvlThanhToanShowcrr.MaGN));
                lstpara.Add(new ParameterDefine("@MaCTTT", nvlThanhToanShowcrr.MaCTTT));
                lstpara.Add(new ParameterDefine("@LyDo", nvlThanhToanShowcrr.LyDo));
                lstpara.Add(new ParameterDefine("@Ngay", nvlThanhToanShowcrr.Ngay));
                lstpara.Add(new ParameterDefine("@DienGiai", nvlThanhToanShowcrr.DienGiai));
                lstpara.Add(new ParameterDefine("@UserInsert", nvlThanhToanShowcrr.UserInsert));
                lstpara.Add(new ParameterDefine("@UserUp", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@type_", "-1"));
                lstpara.Add(new ParameterDefine("@flag", 0));
                lstpara.Add(new ParameterDefine("@sign_type", 1));
                lstpara.Add(new ParameterDefine("@NguoiXacNhan", nvlThanhToanShowcrr.NguoiXacNhan));
                lstpara.Add(new ParameterDefine("@NgayXacNhan", nvlThanhToanShowcrr.NgayXacNhan));

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "SỬA thành công"));
                        if (AfterEdit.HasDelegate)
                        {
                             await AfterEdit.InvokeAsync(nvlThanhToanShowcrr);
                        }
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception)));
                    }
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi: " + ex.Message, IconMsg.iconerror));
            }
        }
        private void reset()
        {


            StateHasChanged();

        }
      
      
    }
}
