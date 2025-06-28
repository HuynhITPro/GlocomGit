using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Urc_KeHoachDuyet
    {
        PhanQuyenAccess phanQuyenAccess { get; set; } = new PhanQuyenAccess();
        [Inject] UserGlobal userGlobal { get; set; }
        [Parameter]
        public DxGrid dxgrid { get; set; }
        [Parameter]
        public KeHoachMuaHang_Show keHoachMuaHang_Showcrr { get; set; }
        [Parameter]
        public EventCallback GotoMainForm { get; set; }

        protected override async Task OnInitializedAsync()
        {
            List<Users> lstusr = await Model.ModelData.Getlstusers();
            lstnguoiduyet = lstusr.ToList();
            nguoiduyet = lstnguoiduyet.Where(p => p.UsersName.Equals(userGlobal.users.UsersName)).FirstOrDefault();
            base.OnInitialized();
        }
        public async Task xacnhanduyetAsync()
        {


            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(keHoachMuaHang_Showcrr.UserInsert, userGlobal.users) && keHoachMuaHang_Showcrr.NguoiDuyet != userGlobal.users.UsersName)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền duyệt dòng này do bạn không phải người tạo hoặc chưa được chọn duyệt"));
                // msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;

            }
            if (nguoiduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Vui lòng chọn người duyệt"));
                // msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;
            }
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NFCNVL.dbo.FileHoSoKyDuyet_InsertNofile";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                if (keHoachMuaHang_Showcrr != null)
                {
                    lstpara.Add(new ParameterDefine("@SerialKyDuyet", keHoachMuaHang_Showcrr.Serial));

                    lstpara.Add(new ParameterDefine("@TableNameKyDuyet", "NvlKehoachMuaHang"));
                }

                lstpara.Add(new ParameterDefine("@UserDuyet", nguoiduyet.UsersName));

                lstpara.Add(new ParameterDefine("@UserInsert", userGlobal.users.UsersName));


                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Success, $"Duyệt thành công"));

                        keHoachMuaHang_Showcrr.NguoiDuyet = nguoiduyet.UsersName;
                        keHoachMuaHang_Showcrr.DaDuyet = nguoiduyet.TenUser;
                        keHoachMuaHang_Showcrr.TinhTrang = "Đã duyệt";
                        diengiai = "";
                        nguoiduyet = null;
                        StateHasChanged();
                       
                        dxgrid.Reload();
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
                ToastService.Notify(new ToastMessage(ToastType.Warning, ("Lỗi: " + ex.Message)));
            }

        }
    }
}
