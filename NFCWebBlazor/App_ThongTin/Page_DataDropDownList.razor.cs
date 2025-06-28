using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Page_DataDropDownList
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject]PhanQuyenAccess phanQuyenAccess { get; set; }
        private async Task loaddatadropdownAsync()
        {
            try
            {
                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string sql = "SELECT Serial,[Name],[FullName],[TypeName]  FROM [DataDropDownList] order by TypeName";

                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {

                    lstdata = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                    var query = lstdata.GroupBy(p => p.TypeName).Select(p => new DataDropDownList { Name = p.Key, FullName = p.Key, TypeName = p.Key }).ToList();
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
        public void saveAsync()
        {
            DataDropDownList dataDropDownList = new DataDropDownList();
            dataDropDownListshow = dataDropDownList;
            PopupVisible = true;
        }
        private async Task deleteAsync(DataDropDownList dataDropDownList)
        {
           
                if (!await phanQuyenAccess.CreateDropdownlist(Model.ModelAdmin.users))
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa"));
                    return;
                }
               
                bool ketqua = await dialogMsg.Show($"XÓA  {dataDropDownList.FullName}???", $"Bạn có chắc muốn xóa  {dataDropDownList.FullName}??");
                if (ketqua)
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = "DataDropDownList_delete";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();

                    lstpara.Add(new ParameterDefine("@Serial", dataDropDownList.Serial));

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
