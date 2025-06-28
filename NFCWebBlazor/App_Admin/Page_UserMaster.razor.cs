using BlazorBootstrap;
using Blazored.Modal;
using Blazored.Modal.Services;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;

using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;

using System.Data;
namespace NFCWebBlazor.App_Admin
{
    public partial class Page_UserMaster
    {
        [Inject] UserGlobal userGlobal { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
     
       
        [Inject] ToastService ToastService { get; set; } = default!;
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
       
        //[CascadingParameter] IModalService Modal { get; set; } = default!;

        string heightgrid = "500px";
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    load();
                    var dimension = await browserService.GetDimensions();
                    // var heighrow = await browserService.GetHeighWithID("divcontainer");
                    int height = dimension.Height - 90;
                    //if (heighrow!=null)
                    //{
                    //    height = dimension.Height - heighrow;
                    //}

                    heightgrid = string.Format("{0}px", height);
                    //base.OnAfterRender(firstRender);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }
            //return base.OnAfterRenderAsync(firstRender);
        }
        private async void load()
        {
            lstusers = await Getlstusers("");
            Console.WriteLine(lstusers.Count.ToString());
            StateHasChanged();

        }
        public  async Task<List<Users>> Getlstusers(string whereusersname)
        {

           List<Users> users = new List<Users>();

            CallAPI callAPI = new CallAPI();
            string sql = string.Format("SELECT STT,[UsersName],[GroupUser],[KhuVuc] ,[DateAccess],[Email],[TenUser],isnull([PathImg],'UserImage/user.png') as [PathImg]\r\n     \r\n  FROM [Users] {0}", whereusersname);

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                users = JsonConvert.DeserializeObject<List<Users>>(json);
            }

            return users;
           
        }
        public void GotoMainForm()
        {
            Grid.Reload();
        }
        public async Task AddItemAsync()
        {
            dxFlyoutchucnang.CloseAsync();
            if (!await phanQuyenAccess.UsersInsert(Model.ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa Thêm"));
                return;
            }

            Users user = new Users();
            user.GroupUser = "";
            user.TenUser = "";
            user.KhuVuc = "";
            user.Email = "";
           
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_UserAdd>(0);
                builder.AddAttribute(1, "userscrr", user);
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
            dxPopup.showAsync("THÊM MỚI USER");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
           await dxPopup.ShowAsync();
        }
        public async Task EditItemAsync()
        {
            
           await dxFlyoutchucnang.CloseAsync();
            if (!await phanQuyenAccess.UsersInsert(Model.ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa Sửa"));
                return;
            }

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_UserAdd>(0);
                builder.AddAttribute(1, "userscrr", usercrr);
                builder.AddAttribute(1, "GotoMainForm", EventCallback.Factory.Create(this, GotoMainForm));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
            dxPopup.showAsync("SỬA THÔNG TIN");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
           await dxPopup.ShowAsync();
        }
        public void PrintItem()
        {

        }
        public async void DeleteItem(Users users)
        {
            if(!await phanQuyenAccess.UsersInsert(Model.ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa User"));
                return;
            }
           await dxFlyoutchucnang.CloseAsync();
            bool ketqua=await dialogMsg.Show($"XÓA USER {users.UsersName}???", $"Bạn có chắc muốn xóa users {users.UsersName}??");
            if(ketqua)
            {
                CallAPI callAPI = new CallAPI();
                string sql = "Users_delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
             
                lstpara.Add(new ParameterDefine("@UsersName", users.UsersName));
              
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
                                ToastService.Notify(new ToastMessage(ToastType.Success, $"Xóa thành công"));
                                lstdata.Remove(users);
                                GotoMainForm();
                                
                                //reset();
                            }
                            else
                            {
                                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketqua));
                            }
                        }
                        catch (Exception ex)
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                }
            }
        }
        private async void search()
        {
            lstdata.Clear();
            if (userselected!=null)
            {
               lstdata=await Getlstusers(string.Format(" where UsersName=N'{0}'",userselected.UsersName));
            }
            else
                lstdata = await Getlstusers(string.Format(""));
            StateHasChanged();
        }
       
    }
}
