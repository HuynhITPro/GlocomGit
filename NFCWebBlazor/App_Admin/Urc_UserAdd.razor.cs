

using BlazorBootstrap;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Urc_KeHoachMuaHangAddMaster;

namespace NFCWebBlazor.App_Admin
{
    public partial class Urc_UserAdd
    {
        [Parameter]
        public Users? userscrr { get; set; }
        [Parameter]
        public EventCallback GotoMainForm { get; set; }
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        [Inject]
        ToastService ToastService { get; set; }
        private EditContext editContext;
        private async void load()
        {
            editContext = new EditContext(userscrr);//Khởi tạo EditContext để gọi sự kiện Onsubmit c
            visibleedit = false;
            if (userscrr.UsersName != null)
            {
                await setItemAsync(userscrr);
                visibleedit = true;
            }
        }
        private async Task setItemAsync(Users users)
        {
            CallAPI callAPI = new CallAPI();
            string sql = string.Format("SELECT * from Users where UsersName=@UserName");

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            parameterDefineList.Add(new ParameterDefine("@UserName", users.UsersName));
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                List<Users> lst = JsonConvert.DeserializeObject<List<Users>>(json);
                if (lst != null)
                {
                    Users? users1 = lst.FirstOrDefault();
                    userscrr.Password = users1.Password;
                    userscrr.KhuVuc = users1.KhuVuc;
                    userscrr.GroupUser = users1.GroupUser;
                    userscrr.Email = users1.Email;
                    dxtagkhuvuc.SetSelectedValue = users1.KhuVuc;
                    dxtagnhomquyen.SetSelectedValue = users1.GroupUser;
                    //khuvucselected = dxtagkhuvuc.setSelected(users1.KhuVuc);
                    //nhomquyenselected = dxtagnhomquyen.setSelected(users1.GroupUser);
                    txtusername.Enabled = false;
                    StateHasChanged();
                }

            }
        }
        protected override Task OnInitializedAsync()
        {
            load();
            return base.OnInitializedAsync();
        }
       
        private void reset()
        {
            userscrr.UsersName = "";
            userscrr.Password = "";
            userscrr.TenUser = "";
            nhomquyenselected = new List<DataDropDownList>();
            khuvucselected = new List<DataDropDownList>();
            userscrr.Email = "";
            StateHasChanged();
            txtusername.FocusAsync();
        }
        private async Task saveAsync()
        {

            if (!await phanQuyenAccess.UsersInsert(Model.ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền tạo User"));

                return;
            }
            userscrr.KhuVuc = dxtagkhuvuc.GetText;
            userscrr.GroupUser = dxtagnhomquyen.GetText;
            if (!editContext.Validate())
            {
                return;
            }

            CallAPI callAPI = new CallAPI();
            string sql = "Users_Insert";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();

            lstpara.Add(new ParameterDefine("@UsersName", userscrr.UsersName));
            lstpara.Add(new ParameterDefine("@Password", userscrr.Password));
            lstpara.Add(new ParameterDefine("@KhuVuc", userscrr.KhuVuc));
            lstpara.Add(new ParameterDefine("@GroupUser", userscrr.GroupUser));
            lstpara.Add(new ParameterDefine("@Email", userscrr.Email));
            lstpara.Add(new ParameterDefine("@TenUser", userscrr.TenUser));
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
                            ToastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                            reset();
                        }
                        else
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketqua));
                        }
                    }
                    catch(Exception ex)
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

        private async Task updateAsync()
        {

            if (!await phanQuyenAccess.UsersInsert(Model.ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền sửa User"));

                return;
            }
            userscrr.KhuVuc = dxtagkhuvuc.GetText;
            userscrr.GroupUser = dxtagnhomquyen.GetText;
            if (!editContext.Validate())
            {
                return;
            }

            CallAPI callAPI = new CallAPI();
            string sql = "Users_Update";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@STT", userscrr.STT));
            lstpara.Add(new ParameterDefine("@UsersName", userscrr.UsersName));
            lstpara.Add(new ParameterDefine("@Password", userscrr.Password));
            lstpara.Add(new ParameterDefine("@KhuVuc", userscrr.KhuVuc));
            lstpara.Add(new ParameterDefine("@GroupUser", userscrr.GroupUser));
            lstpara.Add(new ParameterDefine("@Email", userscrr.Email));
            lstpara.Add(new ParameterDefine("@TenUser", userscrr.TenUser));
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
                            ToastService.Notify(new ToastMessage(ToastType.Success, $"Sửa thành công"));
                            if(GotoMainForm.HasDelegate)
                            {
                                await  GotoMainForm.InvokeAsync();
                            }
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
}
