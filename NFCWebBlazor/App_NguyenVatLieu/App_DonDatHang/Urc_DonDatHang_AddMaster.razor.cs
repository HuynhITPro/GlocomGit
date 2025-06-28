using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;

using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;


namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Urc_DonDatHang_AddMaster
    {
        [Inject] PreloadService PreloadService { get; set; }

        [Inject]ToastService toastService { get; set; }
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                editContext = new EditContext(nVLDonDatHangcrr);
                editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
                lstnhacungcap = await Model.ModelData.Getlstnhacungcap();
                if (nVLDonDatHangcrr.Serial > 0)
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
                    if (nVLDonDatHangcrr.Serial != 0)
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
                string sql = "NVLDB.dbo.NvlDonDatHang_Insert";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nVLDonDatHangcrr.Serial));
                lstpara.Add(new ParameterDefine("@NgayTao", nVLDonDatHangcrr.NgayDatHang));
                lstpara.Add(new ParameterDefine("@KhuVuc", nVLDonDatHangcrr.KhuVuc));
                lstpara.Add(new ParameterDefine("@MaNCC", nVLDonDatHangcrr.MaNCC));
                lstpara.Add(new ParameterDefine("@PhongBan", nVLDonDatHangcrr.PhongBan));
                lstpara.Add(new ParameterDefine("@DVTT", nVLDonDatHangcrr.DVTT));
                lstpara.Add(new ParameterDefine("@NgayDatHang", nVLDonDatHangcrr.NgayDatHang));
                lstpara.Add(new ParameterDefine("@NgayMax", nVLDonDatHangcrr.NgayMax));
                lstpara.Add(new ParameterDefine("@GhiChu", nVLDonDatHangcrr.GhiChu));
                lstpara.Add(new ParameterDefine("@LoaiDonHang", nVLDonDatHangcrr.LoaiDonHang));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "LƯU THÀNH CÔNG"));
                        if (CallBackAfterSave.HasDelegate)
                        {
                            await CallBackAfterSave.InvokeAsync(query[0].Serial);
                        }
                        reset();
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));
                        reset();
                        // msgBox.Show(string.Format("Lỗi:{0}, {1} " + query[0].ketqua, query[0].ketquaexception), IconMsg.iconssuccess);

                    }
                    //Grid.Data = lstDonDatHangSearchShow;
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
                if(!phanQuyenAccess.CheckDelete(nVLDonDatHangcrr.UserInsert,Model.ModelAdmin.users))
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không phải người tạo đề nghị này nên bạn không có quyền sửa"));
                    return;
                }
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlDonDatHang_Update";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nVLDonDatHangcrr.Serial));
                lstpara.Add(new ParameterDefine("@NgayTao", nVLDonDatHangcrr.NgayTao));
                lstpara.Add(new ParameterDefine("@DVTT", nVLDonDatHangcrr.DVTT));
                lstpara.Add(new ParameterDefine("@KhuVuc", nVLDonDatHangcrr.KhuVuc));
                lstpara.Add(new ParameterDefine("@PhongBan", nVLDonDatHangcrr.PhongBan));
                lstpara.Add(new ParameterDefine("@MaNCC", nVLDonDatHangcrr.MaNCC));
                lstpara.Add(new ParameterDefine("@NgayDatHang", nVLDonDatHangcrr.NgayDatHang));
                lstpara.Add(new ParameterDefine("@NgayMax", nVLDonDatHangcrr.NgayMax));
                lstpara.Add(new ParameterDefine("@GhiChu", nVLDonDatHangcrr.GhiChu));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Sửa thành công")));
                        if (AfterEdit.HasDelegate)
                        {
                            await AfterEdit.InvokeAsync(nVLDonDatHangcrr);
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
            
            nVLDonDatHangcrr.NoiDungDeNghi = "";
            nVLDonDatHangcrr.KhuVuc = null;
            
            StateHasChanged();

        }
        private ValidationMessageStore validationMessages;
        private void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {
            if(validationMessages==null)
                validationMessages = new ValidationMessageStore(editContext);

            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();

         
         
            if (string.IsNullOrEmpty(nVLDonDatHangcrr.PhongBan))
            {
                validationMessages.Add(() => nVLDonDatHangcrr.PhongBan, "Phòng ban không được để trống.");
            }
            if (string.IsNullOrEmpty(nVLDonDatHangcrr.MaNCC))
            {
                validationMessages.Add(() => nVLDonDatHangcrr.MaNCC, "Mã nhà cung cấp không được để trống.");
            }
            if (string.IsNullOrEmpty(nVLDonDatHangcrr.DVTT))
            {
                validationMessages.Add(() => nVLDonDatHangcrr.DVTT, "Đơn vị thanh toán không được để trống.");
            }
            if (nVLDonDatHangcrr.NgayDatHang==null)
            {
                validationMessages.Add(() => nVLDonDatHangcrr.NgayDatHang, "Ngày đặt hàng không được để trống.");
            }
            // Lưu kết quả validation
            editContext.NotifyValidationStateChanged();
        }
        private bool checklogic()
        {
            //editContext = new EditContext(nVLDonDatHangcrr);
            //editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
            return editContext.Validate();
        }
        public async void ShowFlyout()
        {
            await dxFlyoutchucnang.CloseAsync();


            //IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();


        }
      
       

    }

}
