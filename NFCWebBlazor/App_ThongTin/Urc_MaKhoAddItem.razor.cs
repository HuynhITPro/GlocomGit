using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Urc_MaKhoAddItem
    {
        [Parameter]
        public NvlMaKho? nvlMaKhopara { get; set; }
       
        [Inject]
        ToastService ToastService { get; set; }
        DxTextBox txtName, txtFullName;
        bool visibleedit = false;
        string namecheck, fullnamecheck;
        private bool checklogic(NvlMaKho dataDropDownList)
        {
            if (dataDropDownList == null)
                return false;
            namecheck = "";
            fullnamecheck = "";
          
            if (String.IsNullOrEmpty(dataDropDownList.MaKho))
            {
                namecheck = "Vui lòng nhập Mã kho";
                return false;
            }
            if (String.IsNullOrEmpty(dataDropDownList.TenKho))
            {
                fullnamecheck = "Vui lòng nhập Tên kho";
                return false;
            }
           
            return true;
        }
        private async void Onkeydown(KeyboardEventArgs e)
        {
            if (e.Key == "F11")
            {
                saveAsync(nvlMaKhopara);
            }
        }
        private async void saveAsync(NvlMaKho dataDropDownList)
        {
            if (!checklogic(dataDropDownList))
                return;
            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlMaKho_Insert";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@MaKho", dataDropDownList.MaKho));
            lstpara.Add(new ParameterDefine("@TenKho", dataDropDownList.TenKho));
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
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketqua));


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
            nvlMaKhopara.MaKho = "";
            nvlMaKhopara.TenKho = "";
            StateHasChanged();
            txtName.FocusAsync();

        }
        private void updateAsync()
        {

        }
    }
}
