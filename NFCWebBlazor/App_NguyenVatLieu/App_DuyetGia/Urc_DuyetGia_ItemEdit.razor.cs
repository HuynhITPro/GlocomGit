
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using static NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.Page_DuyetGiaMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public partial class Urc_DuyetGia_ItemEdit
    {
        [Inject] ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        bool PhanQuyenCheck = false;
        protected override Task OnInitializedAsync()
        {
            editContext = new EditContext(nvlDuyetGiaItemShowcrr);
            editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
            return base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                PhanQuyenCheck = phanQuyenAccess.CheckDelete(nvlDuyetGiaItemShowcrr.UserInsert, ModelAdmin.users);

                PanelVisible = true;
                try
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = string.Format(@"Use NVLDB
                    declare @Serial int={0}
                    SELECT [Serial]
                          ,[KeyGroupItem]
                          ,[DonGia]
                          ,[MaNCC]
                          ,[TenNCC]
                          ,[NgayInsert]
                          ,[GhiChu]
                      FROM [NvlDuyetGiaItem_Detail]
                    where KeyGroupItem in (select KeyGroup from NvlDuyetGiaItem where Serial=@Serial)", nvlDuyetGiaItemShowcrr.Serial);
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    //lstpara.Add(new ParameterDefine("@SerialLink", nvlDuyetGiaShowcrr.Serial));
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var querycustom = JsonConvert.DeserializeObject<List<NvlDuyetGiaItemShow_Detail>>(json);
                        if (querycustom == null)
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, "Đã có lỗi xảy ra"));

                        }
                        else
                        {
                            lstdata = querycustom;
                            Grid.Reload();
                        }
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, ex.Message));
                }
                finally
                {
                    PanelVisible = false;
                    StateHasChanged();
                }
            }
        }
        private ValidationMessageStore validationMessages;
        private void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {
            if(validationMessages==null)
                validationMessages = new ValidationMessageStore(editContext);

            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();

            // Thực hiện kiểm tra tùy chỉnh, ví dụ kiểm tra giá trị null cho KhuVuc
            if (string.IsNullOrEmpty(nvlDuyetGiaItemShowcrr.MaHang))
            {
                validationMessages.Add(() => nvlDuyetGiaItemShowcrr.MaHang, "Chọn Mã hàng");
            }

            if (nvlDuyetGiaItemShowcrr.SLDuToan==null)
            {
                validationMessages.Add(() => nvlDuyetGiaItemShowcrr.SLDuToan, "Nhập số lượng dự toán");
            }
         
            // Lưu kết quả validation
            editContext.NotifyValidationStateChanged();
        }
        private async Task UpdateMasterAsync()
        {
            //editContext = new EditContext(nvlDuyetGiaItemShowcrr);
            //editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
           bool b =  editContext.Validate();
            if (!b)
                return;
            try
            {
                if (!phanQuyenAccess.CheckDelete(nvlDuyetGiaItemShowcrr.UserInsert, Model.ModelAdmin.users))
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không phải người tạo đề nghị này nên bạn không có quyền sửa"));
                    return;
                }
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlDuyetGiaItem_Update";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlDuyetGiaItemShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@MaHang", nvlDuyetGiaItemShowcrr.MaHang));
                lstpara.Add(new ParameterDefine("@XuatXu", nvlDuyetGiaItemShowcrr.XuatXu));
                lstpara.Add(new ParameterDefine("@SLDuToan", nvlDuyetGiaItemShowcrr.SLDuToan));
                lstpara.Add(new ParameterDefine("@GhiChu", nvlDuyetGiaItemShowcrr.GhiChu));
                lstpara.Add(new ParameterDefine("@DinhMuc", nvlDuyetGiaItemShowcrr.DinhMuc));
                lstpara.Add(new ParameterDefine("@UserUpdate", Model.ModelAdmin.users.UsersName));

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        //toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Sửa thành công")));
                        if (GotoMainForm.HasDelegate)
                        {
                            await GotoMainForm.InvokeAsync("close");
                        }


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
        private async Task DeleteItemAsync(NvlDuyetGiaItemShow_Detail nvlDuyetGiaItemShow_Detail)
        {


            bool bl = await dialogMsg.Show("THÔNG BÁO", $"Bạn có chắc muốn xóa  {nvlDuyetGiaItemShow_Detail.TenNCC}?");
            if (bl)
            {
                try
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlDuyetGiaItem_Detail_Delete";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();

                    lstpara.Add(new ParameterDefine("@Serial", nvlDuyetGiaItemShow_Detail.Serial));
                    lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));

                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            toastService.Notify(new(ToastType.Success, $"Xóa thành công"));
                            lstdata.Remove(nvlDuyetGiaItemShow_Detail);
                            await Grid.SaveChangesAsync();
                            if (GotoMainForm.HasDelegate)
                            {
                                await GotoMainForm.InvokeAsync("");
                            }
                        }
                        else
                        {
                            toastService.Notify(new(ToastType.Danger, $"{query[0].ketqua}, {query[0].ketquaexception}"));

                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message} Xóa không được"));
                }
            }
        }
        private async Task UpdateItemAsync(NvlDuyetGiaItemShow_Detail nvlDuyetGiaItemShow_Detail)
        {
            if (!phanQuyenAccess.CheckDelete(nvlDuyetGiaItemShowcrr.UserInsert, Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không phải người tạo đề nghị này nên bạn không có quyền sửa"));
                return;
            }
            if(string.IsNullOrEmpty(nvlDuyetGiaItemShow_Detail.TenNCC))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Nhà cung cấp không được để trống"));
                return;
            }
            await Grid.SaveChangesAsync();
            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlDuyetGiaItem_Detail_Update";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Serial", nvlDuyetGiaItemShow_Detail.Serial));
            lstpara.Add(new ParameterDefine("@DonGia", nvlDuyetGiaItemShow_Detail.DonGia));
          
            lstpara.Add(new ParameterDefine("@MaNCC", nvlDuyetGiaItemShow_Detail.MaNCC));
            lstpara.Add(new ParameterDefine("@TenNCC", nvlDuyetGiaItemShow_Detail.TenNCC));
            lstpara.Add(new ParameterDefine("@GhiChu", nvlDuyetGiaItemShow_Detail.GhiChu));
            try
            {
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Sửa thành công")));
                        if (GotoMainForm.HasDelegate)
                        {
                            await GotoMainForm.InvokeAsync("");
                        }


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
    }
}
