using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using Microsoft.AspNetCore.Components.Forms;
using static NFCWebBlazor.App_ThongTin.Page_KhachHangMaster;
using Microsoft.IdentityModel.Tokens;


namespace NFCWebBlazor.App_ThongTin
{
    public partial class Urc_NhomHangAddItem
    {
        [Parameter]
        public NvlNhomHang? dataDropDownList { get; set; }
        [Parameter]
        public EventCallback<NvlNhomHang> GotoMainForm { get; set; }
        [Inject]
        ToastService ToastService { get; set; }
        DxTextBox txtName, txtFullName;
        bool visibleedit { get; set; } = false;
        string namecheck, fullnamecheck, typenamecheck;
        protected override Task OnInitializedAsync()
        {
           

            visibleedit = !string.IsNullOrEmpty(dataDropDownList.MaNhom);
            return base.OnInitializedAsync();
        }
        private bool checklogic(NvlNhomHang dataDropDownList)
        {
            if (dataDropDownList == null)
                return false;
            namecheck = "";
            fullnamecheck = "";
            typenamecheck = "";
            if (String.IsNullOrEmpty(dataDropDownList.MaNhom))
            {
                namecheck = "Vui lòng nhập Mã nhóm";
                return false;
            }
            if (String.IsNullOrEmpty(dataDropDownList.TenNhom))
            {
                fullnamecheck = "Vui lòng nhập Tên nhóm";
                return false;
            }
            if (String.IsNullOrEmpty(dataDropDownList.PhanLoai))
            {
                typenamecheck = "Vui lòng chọn phân loại";
                return false;
            }
            return true;
        }
        private async void Onkeydown(KeyboardEventArgs e)
        {
            if (e.Key == "F11")
            {
                saveAsync(dataDropDownList);
            }
        }
        private async void saveAsync(NvlNhomHang dataDropDownList)
        {
            if (!checklogic(dataDropDownList))
                return;
            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlNhomHang_Insert";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@MaNhom", dataDropDownList.MaNhom));
            lstpara.Add(new ParameterDefine("@TenNhom", dataDropDownList.TenNhom));
            lstpara.Add(new ParameterDefine("@PhanLoai", dataDropDownList.PhanLoai));
            lstpara.Add(new ParameterDefine("@HuongDan", dataDropDownList.HuongDan));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
            try
            {
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                        reset();
                    }
                    else
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception)));


                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
            }
            return;
        }
        private void reset()
        {
            dataDropDownList.MaNhom = "";
            dataDropDownList.TenNhom = "";
            dataDropDownList.HuongDan = "";
            StateHasChanged();
            txtName.FocusAsync();

        }
        private async Task updateAsync()
        {
           if (!checklogic(dataDropDownList))
                return;
            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlNhomHang_Update";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Serial", dataDropDownList.Serial));
            lstpara.Add(new ParameterDefine("@MaNhom", dataDropDownList.MaNhom));
            lstpara.Add(new ParameterDefine("@TenNhom", dataDropDownList.TenNhom));
            lstpara.Add(new ParameterDefine("@PhanLoai", dataDropDownList.PhanLoai));
            lstpara.Add(new ParameterDefine("@HuongDan", dataDropDownList.HuongDan));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
            try
            {
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Success, $"Sửa thành công"));
                       
                        if(GotoMainForm.HasDelegate)
                        {
                           await GotoMainForm.InvokeAsync(dataDropDownList);
                        }
                        reset();
                    }
                    else
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: {0}, {1}" + query[0].ketquaexception, query[0].ketquaexception));


                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
            }
            return;
        }
    }
}
