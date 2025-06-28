using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Page_DonHangItem_Edit
    {
        bool EnableEdit = true;
        [Inject] ToastService toastService {  get; set; }
        protected override async Task OnInitializedAsync()
        {
            lstncc =await ModelData.Getlstnhacungcap();
            //return base.OnInitializedAsync();
        }
        private async Task updateAsync()
        {
            if(string.IsNullOrEmpty(nVLDonDatHangItemShowcrr.MaNCC))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Nhập nhà cung cấp"));
                return;
            }
            if(nVLDonDatHangItemShowcrr.DonGia<=0)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Nhập đơn giá"));
                return;
            }
            string sql = "NVLDB.dbo.NvlDonDatHangItem_Update";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Serial", nVLDonDatHangItemShowcrr.Serial));
            lstpara.Add(new ParameterDefine("@MaNCC", nVLDonDatHangItemShowcrr.MaNCC));
            lstpara.Add(new ParameterDefine("@DonGia", nVLDonDatHangItemShowcrr.DonGia));
            lstpara.Add(new ParameterDefine("@DienGiai", nVLDonDatHangItemShowcrr.NoiDung));
            lstpara.Add(new ParameterDefine("@UserUpdate", ModelAdmin.users.UsersName));
            CallAPI callAPI = new CallAPI();
            string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                if (query[0].ketqua == "OK")
                {
                    toastService.Notify(new ToastMessage(ToastType.Success, $"Sửa thành công"));
                    if (AfterEdit.HasDelegate)
                    {
                       await AfterEdit.InvokeAsync(nVLDonDatHangItemShowcrr);
                    }
                }
                else
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketqua));


                }
                //Grid.Data = lstDonDatHangSearchShow;
            }

        }
    }
}
