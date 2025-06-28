using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Urc_NhapXuatItemAdd;

namespace NFCWebBlazor.App_Admin
{
    public partial class Page_ChangePassWord
    {
        [Inject] ToastService toastService { get; set; }
        class UserChange
        {
            public string UserName { get; set;}
            public string Password { get; set; }
            public string PasswordNew { get; set; }
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                users = new UserChange();
                users.UserName = ModelAdmin.users.UsersName;
                editContext = new EditContext(users);
                editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
                
                
               
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
        ValidationMessageStore validationMessages { get; set; }
    
       
        private async void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {
            if (validationMessages == null)
                validationMessages = new ValidationMessageStore(editContext);
            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();
            if (string.IsNullOrEmpty(users.Password))
            {
                validationMessages.Add(() => users.Password, "Vui lòng nhập mật khẩu");
                return;
            }
            if (string.IsNullOrEmpty(users.PasswordNew))
            {
                validationMessages.Add(() => users.PasswordNew, "Vui lòng nhập mật khẩu mới");
                return;
            }

            //Thực hiện kiểm tra tùy chỉnh, ví dụ kiểm tra giá trị null cho KhuVuc

            // Lưu kết quả validation

            editContext.NotifyValidationStateChanged();
        }
        private async Task changepasswordAsync()
        {
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "dbo.UsersChangePassWord";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@UsersName", users.UserName));
                lstpara.Add(new ParameterDefine("@PassWord", users.Password));
                lstpara.Add(new ParameterDefine("@PassWordNew", users.PasswordNew));
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "ĐỔI MẬT KHẨU THÀNH CÔNG"));
                        users.UserName="";
                        users.PasswordNew = "";

                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));
                        
                        //reset();
                    }
                   
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
               
                Console.Error.WriteLine("Lỗi:" + ex.Message);
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi " + ex.Message));
                //msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
        }

    }
}
