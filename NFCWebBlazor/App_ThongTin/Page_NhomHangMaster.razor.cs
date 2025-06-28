using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using SkiaSharp;
using static NFCWebBlazor.App_ThongTin.Page_KhachHangMaster;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Page_NhomHangMaster
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
                string sql = "use NVLDB SELECT *  FROM [NvlNhomHang]";

                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {

                    lstdata = JsonConvert.DeserializeObject<List<NvlNhomHang>>(json);
                    var query = lstdata.Select(p => new DataDropDownList { Name = p.MaNhom, FullName = p.TenNhom }).ToList();
                    lsttype = query;
                   
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }
        }

        public async void searchAsync()
        {


            await loaddatadropdownAsync();



        }
        public async void saveAsync()
        {
            NvlNhomHang dataDropDownList = new NvlNhomHang();
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NhomHangAddItem>(0);
                builder.AddAttribute(1, "dataDropDownList", dataDropDownList);
                //builder.AddAttribute(2, "GotoMainForm", GotoFormMain);
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
           await dxPopup.showAsync("THÊM MỚI NHÓM HÀNG");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
            await dxPopup.ShowAsync();
            PopupVisible = true;
        }
        public void setClass(NvlNhomHang nvlNhomHang_set, NvlNhomHang nvlNhomHang_get)
        {
            nvlNhomHang_set.Serial = nvlNhomHang_get.Serial;
            nvlNhomHang_set.MaNhom = nvlNhomHang_get.MaNhom;
            nvlNhomHang_set.TenNhom = nvlNhomHang_get.TenNhom;
            nvlNhomHang_set.PhanLoai = nvlNhomHang_get.PhanLoai;
            nvlNhomHang_set.HuongDan = nvlNhomHang_get.HuongDan;

        }
        private void GotoFormMain(NvlNhomHang nvlNhomHang)
        {
            var query = lstdata.Where(x => x.Serial == nvlNhomHang.Serial).FirstOrDefault();
            if (query != null)
            {
                setClass(query, nvlNhomHang);
                Grid.SaveChangesAsync();
            }
        }
        private async Task deleteAsync(NvlNhomHang dataDropDownList)
        {

            if (!await phanQuyenAccess.CreateNhomHang(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa"));
                return;
            }

            bool ketqua = await dialogMsg.Show($"XÓA  {dataDropDownList.TenNhom}???", $"Bạn có chắc muốn xóa  {dataDropDownList.TenNhom}??");
            if (ketqua)
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlNhomHang_Delete";
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
                                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception)));
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

        private async Task EditItemAsync(NvlNhomHang? nvlNhomHang)
        {

            if (!await phanQuyenAccess.CreateNhomHang(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền sửa"));
                return;
            }
            NvlNhomHang nvlNhomHangcopy = new NvlNhomHang();
            setClass(nvlNhomHangcopy,nvlNhomHang);

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NhomHangAddItem>(0);

                builder.AddAttribute(1, "dataDropDownList", nvlNhomHangcopy);
                builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<NvlNhomHang>(this, GotoFormMain)); 
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
           await dxPopup.showAsync("CHỈNH SỬA NHÓM HÀNG");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
          await  dxPopup.ShowAsync();
            //nvlNhomHangcrr = nvlNhomHang.CopyClass();
            //Console.WriteLine("EditClick: "+nvlNhomHangcrr.MaNhom);

            //PopupVisible = true;

        }


    }

}
