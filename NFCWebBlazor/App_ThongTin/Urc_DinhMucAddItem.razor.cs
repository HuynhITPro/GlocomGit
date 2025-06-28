using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;

using static NFCWebBlazor.App_ThongTin.Page_DinhMucHangHoaMaster;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Urc_DinhMucAddItem
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        private bool checklogic(HangHoaItem nvlHangHoaShow)
        {
            //if (selectNhom == null)
            //{
            //    nvlHangHoaShow.MaNhom = null;
            //    nvlHangHoaShow.TenNhom = null;
            //}
            //else
            //{
            //    nvlHangHoaShow.MaNhom = selectNhom.Name;
            //    nvlHangHoaShow.TenNhom = selectNhom.FullName;
            //}

            return editContext.Validate();


        }
        protected override Task OnInitializedAsync()
        {
            editContext = new EditContext(nvlHangHoaShowcrr);

            visibleedit = !string.IsNullOrEmpty(nvlHangHoaShowcrr.MaHang);
            load();
            //Console.WriteLine("Mã nhóm: " + nvlHangHoaShowcrr.MaNhom);
            return base.OnInitializedAsync();
        }
        private async void load()
        {

           
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                //load();


                // StateHasChanged();
            }

            base.OnAfterRender(firstRender);
        }
        private async Task saveAsync()
        {
            if (!await phanQuyenAccess.CreateMaHang(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền tạo hàng hóa"));

                return;
            }
            if (!checklogic(nvlHangHoaShowcrr))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Vui lòng kiểm tra lại thông tin nhập"));
                return;
            }

            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Serial", nvlHangHoaShowcrr.Serial));
            lstpara.Add(new ParameterDefine("@MaHang", nvlHangHoaShowcrr.MaHang));
           
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
                            toastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                            reset();
                            lstpara.Clear();
                        }
                        else
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0},{1} ", query[0].ketqua, query[0].ketquaexception)));
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
        private async Task updateAsync()
        {
            if (!await phanQuyenAccess.CreateMaHang(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền sửa hàng hóa"));

                return;
            }
            if (!checklogic(nvlHangHoaShowcrr))
            {
                return;
            }

            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();

            lstpara.Add(new ParameterDefine("@Serial", nvlHangHoaShowcrr.Serial));
            lstpara.Add(new ParameterDefine("@MaHang", nvlHangHoaShowcrr.MaHang));
            lstpara.Add(new ParameterDefine("@TenHang", nvlHangHoaShowcrr.TenHang));
          
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
                            toastService.Notify(new ToastMessage(ToastType.Success, $"Sửa thành công"));
                          
                            await GotoMainForm.InvokeAsync(nvlHangHoaShowcrr);
                            reset();
                            lstpara.Clear();
                        }
                        else
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0},{1} ", query[0].ketqua, query[0].ketquaexception)));
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
