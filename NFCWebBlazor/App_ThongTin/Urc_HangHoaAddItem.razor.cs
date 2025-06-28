using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using static NFCWebBlazor.App_ThongTin.Page_HangHoaMaster;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Urc_HangHoaAddItem
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        private bool checklogic(NvlHangHoaShow nvlHangHoaShow)
        {
            if (selectNhom == null)
            {
                nvlHangHoaShow.MaNhom = null;
                nvlHangHoaShow.TenNhom = null;
            }
            else
            {
                nvlHangHoaShow.MaNhom = selectNhom.Name;
                nvlHangHoaShow.TenNhom = selectNhom.FullName;
            }
               
            return editContext.Validate();


        }
        protected override async Task OnInitializedAsync()
        {
            editContext = new EditContext(nvlHangHoaShowcrr);

            visibleedit = !string.IsNullOrEmpty(nvlHangHoaShowcrr.MaHang);
            load();
            await goiyclickAsync();
            //Console.WriteLine("Mã nhóm: " + nvlHangHoaShowcrr.MaNhom);
            //return base.OnInitializedAsync();
        }
        private async void load()
        {

            if (lstmanhom==null)
            {
                //lstmanhom = new List<DataDropDownList>();
                // lstmanhom =await ModelData.GetlstNhomhang();
                CallAPI callAPI = new CallAPI();
                string sql = "use NVLDB SELECT [MaNhom] as [Name],[TenNhom] as FullName,N'NhomHang' as TypeName FROM [NvlNhomHang]";
                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {

                    lstmanhom = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                   
                    selectNhom = lstmanhom.Where(p => p.Name == nvlHangHoaShowcrr.MaNhom).FirstOrDefault();
                    //Console.WriteLine("Mã nhóm : {0},{1}", nvlHangHoaShowcrr.MaNhom, lstmanhom.Count);
                   
                     StateHasChanged();
                }
               
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if(firstRender)
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
            string sql = "NVLDB.dbo.NvlHangHoa_InsertVer2";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Serial", nvlHangHoaShowcrr.Serial));
            lstpara.Add(new ParameterDefine("@MaHang", nvlHangHoaShowcrr.MaHang));
            lstpara.Add(new ParameterDefine("@TenHang", nvlHangHoaShowcrr.TenHang));
            lstpara.Add(new ParameterDefine("@QuyCach", nvlHangHoaShowcrr.QuyCach));
            lstpara.Add(new ParameterDefine("@MaNhom", nvlHangHoaShowcrr.MaNhom));
            lstpara.Add(new ParameterDefine("@DVT", nvlHangHoaShowcrr.DVT));
            lstpara.Add(new ParameterDefine("@ChatLuong", nvlHangHoaShowcrr.ChatLuong));
            lstpara.Add(new ParameterDefine("@MinTK", nvlHangHoaShowcrr.MinTK));
            lstpara.Add(new ParameterDefine("@SLCont", nvlHangHoaShowcrr.SLCont));
            lstpara.Add(new ParameterDefine("@MaxTK", nvlHangHoaShowcrr.MaxTK));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
            lstpara.Add(new ParameterDefine("@flag", nvlHangHoaShowcrr.flag));
            lstpara.Add(new ParameterDefine("@GhiChu", nvlHangHoaShowcrr.GhiChu));
            lstpara.Add(new ParameterDefine("@MaPDOC", nvlHangHoaShowcrr.MaPDOC));
            lstpara.Add(new ParameterDefine("@MaKhac", nvlHangHoaShowcrr.MaKhac));
            lstpara.Add(new ParameterDefine("@MaSPGroup", nvlHangHoaShowcrr.MaSPGroup));
            lstpara.Add(new ParameterDefine("@MaMau", nvlHangHoaShowcrr.MaMau));
            lstpara.Add(new ParameterDefine("@TyLeHaoHut", nvlHangHoaShowcrr.TyLeHaoHut));
            lstpara.Add(new ParameterDefine("@KeySearch", nvlHangHoaShowcrr.KeySearch));
            lstpara.Add(new ParameterDefine("@DVT2", nvlHangHoaShowcrr.DVT2));
            lstpara.Add(new ParameterDefine("@TyLeQD", nvlHangHoaShowcrr.TyLeQD));
            lstpara.Add(new ParameterDefine("@PathImg", nvlHangHoaShowcrr.PathImg));
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
            string sql = "NVLDB.dbo.NvlHangHoa_Update_Ver2";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();

            lstpara.Add(new ParameterDefine("@Serial", nvlHangHoaShowcrr.Serial));
            lstpara.Add(new ParameterDefine("@MaHang", nvlHangHoaShowcrr.MaHang));
            lstpara.Add(new ParameterDefine("@TenHang", nvlHangHoaShowcrr.TenHang));
            lstpara.Add(new ParameterDefine("@QuyCach", nvlHangHoaShowcrr.QuyCach));
            lstpara.Add(new ParameterDefine("@MaNhom", nvlHangHoaShowcrr.MaNhom));
            lstpara.Add(new ParameterDefine("@DVT", nvlHangHoaShowcrr.DVT));
            lstpara.Add(new ParameterDefine("@ChatLuong", nvlHangHoaShowcrr.ChatLuong));
            lstpara.Add(new ParameterDefine("@MinTK", nvlHangHoaShowcrr.MinTK));
            lstpara.Add(new ParameterDefine("@MaxTK", nvlHangHoaShowcrr.MaxTK));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
            lstpara.Add(new ParameterDefine("@flag", nvlHangHoaShowcrr.flag));
            lstpara.Add(new ParameterDefine("@GhiChu", nvlHangHoaShowcrr.GhiChu));
            lstpara.Add(new ParameterDefine("@MaPDOC", nvlHangHoaShowcrr.MaPDOC));
            lstpara.Add(new ParameterDefine("@MaKhac", nvlHangHoaShowcrr.MaKhac));
            lstpara.Add(new ParameterDefine("@MaSPGroup", nvlHangHoaShowcrr.MaSPGroup));
            lstpara.Add(new ParameterDefine("@MaMau", nvlHangHoaShowcrr.MaMau));
            lstpara.Add(new ParameterDefine("@TyLeHaoHut", nvlHangHoaShowcrr.TyLeHaoHut));
            lstpara.Add(new ParameterDefine("@KeySearch", nvlHangHoaShowcrr.KeySearch));
            lstpara.Add(new ParameterDefine("@PathImg", nvlHangHoaShowcrr.PathImg));
            lstpara.Add(new ParameterDefine("@SLCont", nvlHangHoaShowcrr.SLCont));
            lstpara.Add(new ParameterDefine("@DVT2", nvlHangHoaShowcrr.DVT2));
            lstpara.Add(new ParameterDefine("@TyLeQD", nvlHangHoaShowcrr.TyLeQD));
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
                            nvlHangHoaShowcrr.TenNhom = selectNhom.FullName;
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
        private async Task goiyclickAsync()
        {
           
            string dieukien = "";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            if (!string.IsNullOrEmpty(nvlHangHoaShowcrr.MaNhom))
            {
                dieukien = " where MaNhom=@MaNhom";
                lstpara.Add(new ParameterDefine("@MaNhom", nvlHangHoaShowcrr.MaNhom));
            }
            CallAPI callAPI = new CallAPI();
           
            string sql = string.Format(@"use NVLDB SELECT left([MaHang],3) as FullName,max(Right([MaHang],len(MaHang)-3)) as  [Name]
                          FROM [dbo].[NvlHangHoa]
                            {0}
                          group by  left([MaHang],3)", dieukien);

            string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
            if (json != "")
            {
                try
                {
                    var query = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                    lstnguyentac= query;
                   
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                }
                //Grid.Data = lstDonDatHangSearchShow;
            }
        }
        private void nguyentacChanged(DataDropDownList dataDropDownList)
        {
            if(dataDropDownList==null) return;
            if(nvlHangHoaShowcrr.Serial==0||nvlHangHoaShowcrr.Serial==null)
            {
                if(int.TryParse(dataDropDownList.Name,out int result))
                {
                    nvlHangHoaShowcrr.MaHang = string.Format("{0}{1}", dataDropDownList.FullName, (result+1).ToString("0000"));
                }
                else 
                    nvlHangHoaShowcrr.MaHang =string.Format("{0}{1}",dataDropDownList.FullName, dataDropDownList.Name);
            }
        }
    }

}
