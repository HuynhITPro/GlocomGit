using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System.ComponentModel.DataAnnotations;

using System.Data;
using Newtonsoft.Json;
using NFCWebBlazor.Pages;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components.Forms;
using static NFCWebBlazor.App_ThongTin.Page_KhachHangMaster;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Urc_KeHoachMuaHangAddMaster
    {
        [Inject] PreloadService PreloadService { get; set; }

        [Parameter]
        public NvlKehoachMuaHang nvlKehoachMuaHang { get; set; }
        [Parameter]
        public KeHoachMuaHang_Show keHoachMuaHang_Showcrr { get; set; }
        [Parameter]
        public DxGrid dxgrid { get; set; }
        [Inject]
        ToastService toastService { get; set; }
        public class NvlKehoachMuaHang
        {


            public int Serial { get; set; }
            public Nullable<int> STT { get; set; }
            public string MaDN { get; set; }
            public string LoaiKeHoach { get; set; }
            [MinLength(1)]
            [Required(ErrorMessage = "Vui lòng chọn người đề nghị")]
            public string NguoiDN { get; set; }
            [MinLength(1)]
            [Required(ErrorMessage = "Vui lòng nhập lý do")]
            public string LyDo { get; set; }

            [MinLength(1)]
            [Required(ErrorMessage = "Vui lòng chọn nhóm vật tư")]
            public string KhuVuc { get; set; }
            [Required(ErrorMessage = "Vui lòng nhập ngày đề nghị")]
            public Nullable<System.DateTime> NgayDN { get; set; }

            [MinLength(1)]
            [Required(ErrorMessage = "Chọn bộ phận đề nghị")]

            public string PhongBan { get; set; }
            [MinLength(1)]
            [Required(ErrorMessage = "Chọn bộ phận mua hàng")]

            public string BoPhanMuaHang { get; set; }

            [MinLength(1)]
            [Required(ErrorMessage = "Vui lòng chọn nhà máy")]

            public string NhaMay { get; set; }
            public Nullable<System.DateTime> NgayMax { get; set; }
            public string NoiDung { get; set; }
            public string GhiChu { get; set; }
            public string UserInsert { get; set; }
            public Nullable<System.DateTime> NgayInsert { get; set; }
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                editContext = new EditContext(nvlKehoachMuaHang);
                PreloadService.Show();
                List<Users> lstusr = await Model.ModelData.Getlstusers();
                lstnguoidenghi = lstusr.ToList();
                lstbophandenghi = await Model.ModelData.Getlstphongbankhuvuc();
               
                if (nvlKehoachMuaHang.Serial > 0)
                {
                    EnableEdit = true;
                }
                else
                {
                    string sql = string.Format(@"use NVLDB declare @UserInsert nvarchar(100)=N'{0}'
                            SELECT TOP (1) *
                              FROM [dbo].[NvlKehoachMuaHang]
                              where UserInsert=@UserInsert
                              order by Serial desc",ModelAdmin.users.UsersName);
                    //Load thông tin đề nghị cuối cùng lên để gán vào cho nhanh để đỡ mất thời gian nhập liệu
                    CallAPI callAPI = new CallAPI();
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
                    if (json != "")
                    {

                        var query = JsonConvert.DeserializeObject<List<KeHoachMuaHang_Show>>(json);

                        if(query.Any())
                        {
                            nvlKehoachMuaHang.PhongBan = query[0].PhongBan;
                            nvlKehoachMuaHang.BoPhanMuaHang = query[0].BoPhanMuaHang;
                            nvlKehoachMuaHang.NhaMay= query[0].NhaMay;
                            nvlKehoachMuaHang.KhuVuc=query[0].KhuVuc;
                        }
                        Console.WriteLine(query.Count);
                        //await InvokeAsync(StateHasChanged);
                    }
                }
                if (nvlKehoachMuaHang.LoaiKeHoach.Contains("Xuat"))
                {
                    //lstbophanmuahang = await ModelData.GetDataDropDownListsAsync("NVL_Kho");
                    var querykho= await Model.ModelData.GetKhoNvl();
                    var queryselect=querykho.Select(p=>new DataDropDownList { Name=p.FullName,FullName=p.FullName }).ToList();
                    lstbophanmuahang = queryselect.ToList();
                    bophankyduyet = "Kho xuất hàng";
                }
                else
                {

                    lstbophanmuahang = await ModelData.GetDataDropDownListsAsync("PhongBanMuaHang");
                    bophankyduyet = "Bộ phận mua hàng";
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load form ke hoach master " + ex.Message);
            }
            finally
            {
                PreloadService.Hide();
                StateHasChanged();
            }
            base.OnInitialized();
        }
        bool checkset = false;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!checkset)
            {
                if (lstnguoidenghi != null)
                {

                    if (nvlKehoachMuaHang.Serial == 0)
                    {

                        NguoiDeNghiSelected = lstnguoidenghi.Where(p => p.TenUser.Equals(nvlKehoachMuaHang.NguoiDN)).FirstOrDefault();
                        checkset = true;

                        StateHasChanged();
                    }
                    else
                    {
                        NguoiDeNghiSelected = lstnguoidenghi.Where(p => p.TenUser.Equals(nvlKehoachMuaHang.NguoiDN)).FirstOrDefault();
                        checkset = true;

                        StateHasChanged();
                    }

                }
            }
            return base.OnAfterRenderAsync(firstRender);
        }


        private async Task saveAsync()
        {
            if (!checklogic())
                return;
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlKehoachMuaHang_Insert";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", "0"));
                lstpara.Add(new ParameterDefine("@KhuVuc", nvlKehoachMuaHang.KhuVuc));
                lstpara.Add(new ParameterDefine("@LoaiKeHoach", nvlKehoachMuaHang.LoaiKeHoach));
                lstpara.Add(new ParameterDefine("@NguoiDN", nvlKehoachMuaHang.NguoiDN));
                lstpara.Add(new ParameterDefine("@LyDo", nvlKehoachMuaHang.LyDo));
                lstpara.Add(new ParameterDefine("@NgayDN", nvlKehoachMuaHang.NgayDN.Value.ToString("MM/dd/yyyy")));
                lstpara.Add(new ParameterDefine("@BoPhanMuaHang", nvlKehoachMuaHang.BoPhanMuaHang));
                lstpara.Add(new ParameterDefine("@PhongBan", nvlKehoachMuaHang.PhongBan));
                lstpara.Add(new ParameterDefine("@NhaMay", nvlKehoachMuaHang.NhaMay));
                if (nvlKehoachMuaHang.NgayMax == null)
                    lstpara.Add(new ParameterDefine("@NgayMax", null));
                else
                    lstpara.Add(new ParameterDefine("@NgayMax", nvlKehoachMuaHang.NgayMax.Value.ToString("MM/dd/yyyy")));

                lstpara.Add(new ParameterDefine("@NoiDung", nvlKehoachMuaHang.NoiDung));
                lstpara.Add(new ParameterDefine("@GhiChu", nvlKehoachMuaHang.GhiChu));
                lstpara.Add(new ParameterDefine("@UserInsert", nvlKehoachMuaHang.UserInsert));
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "LƯU THÀNH CÔNG"));
                        if (CallBackAfterSave.HasDelegate)
                        {
                            await CallBackAfterSave.InvokeAsync(query[0].Serial);
                        }
                        reset();
                       
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));
                        reset();
                        // msgBox.Show(string.Format("Lỗi:{0}, {1} " + query[0].ketqua, query[0].ketquaexception), IconMsg.iconssuccess);

                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi " + ex.Message));
                //msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
        }
        private async Task updateAsync()
        {
            if (!checklogic())
                return;
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlKehoachMuaHang_Update";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlKehoachMuaHang.Serial.ToString()));
                lstpara.Add(new ParameterDefine("@KhuVuc", nvlKehoachMuaHang.KhuVuc));
                //lstpara.Add(new ParameterDefine("@LoaiKeHoach", nvlKehoachMuaHang.LoaiKeHoach));
                lstpara.Add(new ParameterDefine("@NguoiDN", nvlKehoachMuaHang.NguoiDN));
                lstpara.Add(new ParameterDefine("@LyDo", nvlKehoachMuaHang.LyDo));
                lstpara.Add(new ParameterDefine("@NgayDN", nvlKehoachMuaHang.NgayDN.Value.ToString("MM/dd/yyyy")));
                lstpara.Add(new ParameterDefine("@BoPhanMuaHang", nvlKehoachMuaHang.BoPhanMuaHang));
                lstpara.Add(new ParameterDefine("@PhongBan", nvlKehoachMuaHang.PhongBan));
                lstpara.Add(new ParameterDefine("@NhaMay", nvlKehoachMuaHang.NhaMay));
                if (nvlKehoachMuaHang.NgayMax == null)
                    lstpara.Add(new ParameterDefine("@NgayMax", null));
                else
                    lstpara.Add(new ParameterDefine("@NgayMax", nvlKehoachMuaHang.NgayMax.Value.ToString("MM/dd/yyyy")));

                lstpara.Add(new ParameterDefine("@NoiDung", nvlKehoachMuaHang.NoiDung));
                lstpara.Add(new ParameterDefine("@GhiChu", nvlKehoachMuaHang.GhiChu));
                lstpara.Add(new ParameterDefine("@UserInsert", nvlKehoachMuaHang.UserInsert));
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        msgBox.Show("Sửa thành công", IconMsg.iconssuccess);
                        if (keHoachMuaHang_Showcrr != null)
                        {
                            keHoachMuaHang_Showcrr.KhuVuc = nvlKehoachMuaHang.KhuVuc;
                            keHoachMuaHang_Showcrr.LyDo = nvlKehoachMuaHang.LyDo;
                            keHoachMuaHang_Showcrr.GhiChu = nvlKehoachMuaHang.GhiChu;
                            keHoachMuaHang_Showcrr.NoiDung = nvlKehoachMuaHang.NoiDung;
                            keHoachMuaHang_Showcrr.PhongBan = nvlKehoachMuaHang.PhongBan;
                            keHoachMuaHang_Showcrr.NguoiDN = nvlKehoachMuaHang.NguoiDN;
                            keHoachMuaHang_Showcrr.NgayDN = nvlKehoachMuaHang.NgayDN;
                            keHoachMuaHang_Showcrr.NhaMay = nvlKehoachMuaHang.NhaMay;
                            keHoachMuaHang_Showcrr.BoPhanMuaHang = nvlKehoachMuaHang.BoPhanMuaHang;
                            keHoachMuaHang_Showcrr.NgayMax = nvlKehoachMuaHang.NgayMax;
                            dxgrid.Reload();
                        }
                        if(CallBackAfterSave.HasDelegate)
                        {
                            await CallBackAfterSave.InvokeAsync(query[0].Serial);
                        }
                        reset();
                    }
                    else
                    {
                        msgBox.Show(string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception), IconMsg.iconssuccess);

                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
        }
        private void reset()
        {
            nvlKehoachMuaHang.NoiDung = "";
            nvlKehoachMuaHang.GhiChu = "";
            nvlKehoachMuaHang.KhuVuc = "";

            StateHasChanged();

        }


        private bool checklogic()
        {
            nvlKehoachMuaHang.NguoiDN = (NguoiDeNghiSelected == null) ? "" : NguoiDeNghiSelected.TenUser;
           var querycheck=lstbophandenghi.Where(p=>p.Name== nvlKehoachMuaHang.PhongBan).FirstOrDefault();
            if(querycheck==null)
            {
                nvlKehoachMuaHang.PhongBan = null;
                return false;
            }
            return editContext.Validate();
        }




        public async void ShowFlyout()
        {
            try
            {


                await dxFlyoutchucnang.CloseAsync();
                //CurrentEmployee = employee;

                //StateHasChanged();

                //IsOpenfly = true;
                await dxFlyoutchucnang.ShowAsync();
            }
            catch(Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
            //dxFlyoutchucnang.PositionTarget = idflychucnang;
            //Console.WriteLine(idflychucnang);
            //dxFlyoutchucnang.RepositionAsync();

            // IsOpenfly = true;
            //await dxFlyoutchucnang.ShowAsync();

        }
        private string noidungghichuex()
        {
            string vat = "";
            if (checkvat)
                vat = "Đơn giá trên đã bao gồm thuế VAT, đã bao gồm cước vận chuyển";
            else
                vat = "Đơn giá trên chưa bao gồm thuế VAT, đã bao gồm cước vận chuyển";
            if (nvlKehoachMuaHang.PhongBan != null)
            {

                noidungghichu = vat + Environment.NewLine + "Lập dự trù: " + nvlKehoachMuaHang.PhongBan + Environment.NewLine + "Đơn vị thực hiện: P. Vật tư" + Environment.NewLine + "Thời gian hoàn thành: ";
            }
            else
                noidungghichu = vat + Environment.NewLine + "Lập dự trù: " + Environment.NewLine + "Đơn vị thực hiện: P. Vật tư" + Environment.NewLine + "Thời gian hoàn thành: ";
            return noidungghichu;
        }
        private void btDongClick()
        {
            nvlKehoachMuaHang.NoiDung = noidungghichuex();
            IsOpenfly = false;
            dxFlyoutchucnang.CloseAsync();

        }

    }
}
