using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Page_MaKhoMaster
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        private async Task loaddatadropdownAsync()
        {
            try
            {
                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string sql = "use [NVLDB] SELECT  [MaKho],[TenKho] ,[UserInsert],[NgayInsert]\r\n  FROM [dbo].[NvlMaKho]";

                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {

                    lstdata = JsonConvert.DeserializeObject<List<NvlMaKho>>(json);

                    var query = lstdata.Select(p => new DataDropDownList { Name =p.MaKho,  FullName = p.TenKho }).ToList();
                    lsttype = query;
                    PanelVisible = false;
                    //Grid.Reload();
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;
            }


        }

        public async void searchAsync()
        {


            await loaddatadropdownAsync();



        }
        public async Task saveAsync()
        {
            if (!await phanQuyenAccess.CreateMaKho(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền thêm kho"));
                return;
            }
            NvlMaKho nvlMaKho=new NvlMaKho();
            nvlMaKho.MaKho = "";
            nvlMaKho.TenKho = "";
            nvlMaKhocrr = nvlMaKho;
            PopupVisible = true;
        }
        private async Task deleteAsync(NvlMaKho dataDropDownList)
        {

            if (!await phanQuyenAccess.CreateMaKho(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa"));
                return;
            }

            bool ketqua = await dialogMsg.Show($"XÓA  {dataDropDownList.TenKho}???", $"Bạn có chắc muốn xóa  {dataDropDownList.TenKho}??");
            if (ketqua)
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlMaKho_Delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();

                lstpara.Add(new ParameterDefine("@MaKho", dataDropDownList.MaKho));

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
                                toastService.Notify(new ToastMessage(ToastType.Success, $"Xóa thành công"));
                                lstdata.Remove(dataDropDownList);
                                Grid.Reload();

                            }
                            else
                            {
                                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketqua));
                            }
                        }
                        catch (Exception ex)
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                }
            }
        }


    }

}
