using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucHangHoaMaster;
using System.Data;
using Microsoft.AspNetCore.Components.Forms;
using static NFCWebBlazor.App_ThongTin.Page_KhachHangMaster;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Urc_DinhMucHangHoa_AddItem_2
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        [Inject] IJSRuntime JS { get; set; }
        bool CheckQuyen = false;
        
        protected override async Task OnInitializedAsync()
        {
            editContext = new EditContext(hangHoaItemcrr);
            editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
            await loadAsync();
            // return base.OnInitializedAsync();
        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {

            return base.OnAfterRenderAsync(firstRender);
        }

        private async void Onkeydown(KeyboardEventArgs e)
        {
            if (e.Key == "F11")
            {
                await txtMaHang.FocusAsync();

                // await JS.InvokeVoidAsync("document.activeElement.blur");
                await saveAsync();
            }
        }
        ValidationMessageStore validationMessages { get; set; }
        private async void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {
            if (validationMessages == null)
                validationMessages = new ValidationMessageStore(editContext);
            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();
            if (string.IsNullOrEmpty(hangHoaItemcrr.MaHang))
            {
                validationMessages.Add(() => hangHoaItemcrr.MaHang, "Mã hàng không được để trống");


            }
            if (hangHoaItemcrr.SLQuyDoi==null)
            {
                validationMessages.Add(() => hangHoaItemcrr.SLQuyDoi, "So lượng quy đổi không không được để trống");


            }
            



            // Lưu kết quả validation

            editContext.NotifyValidationStateChanged();
        }

        private void reset()
        {

            hangHoaItemcrr.MaHang = "";
            hangHoaItemcrr.SLQuyDoi = null;
            hangHoaItemcrr.DinhMucHaoHut = null;
            hangHoaItemcrr.GhiChu = "";
            msgerr = "";
            // editContext = new EditContext(hangHoaItemcrr);
            StateHasChanged();
            txtMaHang.FocusAsync();
        }
        public class Ketquatrave
        {
            public int Serial { get; set; }
            public string MaHang { get; set; }
            public string ArticleNumber { get; set; }
            public string ketqua { get; set; }
            public string ketquaexception { get; set; }
        }
        DataTable dtsave;
        private async Task saveAsync()
        {
            if (!await phanQuyenAccess.CreateMaHang(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền tạo hàng hóa"));

                return;
            }
            if (!checklogic())
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Vui lòng kiểm tra lại thông tin nhập"));
                return;
            }
            CallAPI callAPI = new CallAPI();
            string sql = "";
            string json = "";
            if (dtsave == null)
            {
                sql = @"use NVLDB declare @dt Type_NvlChiTietKhuVuc_Check
                insert into @dt([Serial] ,[MaSP],[SLQuyDoi],[MaHang],[KhuVuc],[DinhMucHaoHut])
                values(1,'',1,'','',0)
                select * from @dt";
                List<ParameterDefine> lstparadt = new List<ParameterDefine>();
                json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparadt);
                if (json == "")
                    return;
                dtsave = JsonConvert.DeserializeObject<DataTable>(json);
                // dtsave.Clear();
            }
            dtsave.Clear();
            int i = 0;


            DataRow rownew = dtsave.NewRow();
            rownew["Serial"] = i;
           
            rownew["SLQuyDoi"] = hangHoaItemcrr.SLQuyDoi;
            rownew["MaHang"] = hangHoaItemcrr.MaHang;
            rownew["MaSP"] = hangHoaItemmaster.MaSP;
            rownew["KhuVuc"] = hangHoaItemmaster.KhuVuc;
            rownew["DinhMucHaoHut"] = 1;
            rownew["GhiChu"] = hangHoaItemcrr.GhiChu;
            rownew["UserInsert"] = Model.ModelAdmin.users.UsersName;
            rownew["ChatLuong"] = hangHoaItemcrr.ChatLuong;
            dtsave.Rows.Add(rownew);


            callAPI = new CallAPI();
            sql = "NVLDB.dbo.NvlChiTietKhuVucItem_InsertTable";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@type_nvlchitietkhuvuc_check", prs.ConvertDataTableToJson(dtsave), "DataTable"));
            lstpara.Add(new ParameterDefine("@SerialLink", hangHoaItemmaster.Serial));

            try
            {
                json = await callAPI.ProcedureEncryptAsync(sql, lstpara);

                if (json != "")
                {
                    try
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketquatrave>>(json);
                        if (query[0].ketqua == "OK")
                        {

                            toastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                            reset();
                            lstpara.Clear();
                            dtsave.Clear();
                        }
                        else
                        {
                            msgerr = "";
                            if (query[0].MaHang != null)
                            {
                                foreach (var it in query)
                                {
                                    foreach (DataRow itemdata in dtsave.Rows)
                                    {
                                        if (it.Serial == int.Parse(itemdata["Serial"].ToString()))
                                        {
                                            msgerr += it.ketqua;

                                            break;
                                        }
                                    }
                                }
                            }
                            else
                                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0},{1} ", query[0].ketqua, query[0].ketquaexception)));
                            
                            toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0} ", msgerr)));
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

        private bool checklogic()
        {


           return editContext.Validate();
            //return editContext.Validate();
        }


        private async Task updateAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền sửa"));
                return;
            }
            if (!checklogic())
            {
                return;
            }

            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlChiTietKhuVucItem_Update";
            string artupdate = null;
          
            List<ParameterDefine> lstpara = new List<ParameterDefine>();

            lstpara.Add(new ParameterDefine("@Serial", hangHoaItemcrr.Serial));
           // lstpara.Add(new ParameterDefine("@MaSP", hangHoaItemcrr.MaSP));
            lstpara.Add(new ParameterDefine("@SLQuyDoi", hangHoaItemcrr.SLQuyDoi));
            lstpara.Add(new ParameterDefine("@MaHang", hangHoaItemcrr.MaHang));
           // lstpara.Add(new ParameterDefine("@KhuVuc", hangHoaItemcrr.KhuVuc));

           // lstpara.Add(new ParameterDefine("@ArticleNumber", artupdate));
            lstpara.Add(new ParameterDefine("@GhiChu", hangHoaItemcrr.GhiChu));
            lstpara.Add(new ParameterDefine("@UserInsert", hangHoaItemcrr.UserInsert));
            lstpara.Add(new ParameterDefine("@ChatLuong", hangHoaItemcrr.ChatLuong));
            lstpara.Add(new ParameterDefine("@DinhMucHaoHut", hangHoaItemcrr.DinhMucHaoHut));

            try
            {
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);

                if (json != "")
                {
                    try
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketquatrave>>(json);
                        if (query[0].ketqua == "OK")
                        {

                            toastService.Notify(new ToastMessage(ToastType.Success, $"Sửa thành công"));
                            if (GotoMainForm.HasDelegate)
                            {
                                hangHoaItemcrr.TenHang = txtMaHang.Text;
                                await GotoMainForm.InvokeAsync(hangHoaItemcrr);
                            }
                            //reset();
                            //lstpara.Clear();
                            //dtsave.Clear();
                        }
                        else
                        {
                            msgerr = "";
                            if (query[0].MaHang != null)
                            {
                                foreach (var it in query)
                                {

                                    if (it.ketqua != "")
                                        msgerr += it.ketqua;
                                    else
                                        msgerr += string.Format("Mã hàng {0} và nhóm {1} đã có trong hệ thống rồi, ", it.MaHang, ((it.ArticleNumber == null) ? "Dùng chung" : it.ArticleNumber));
                                    break;
                                }
                            }
                            else
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
        public async Task ImportExcelAsync(SanPhamShow sanPhamShow)
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền import"));
                return;
            }
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DinhMucImportExcel>(0);
                builder.AddAttribute(1, "sanPhamShowcrr", sanPhamShow);
                builder.CloseComponent();
            };

          await  dxPopup.showAsync("Import hàng hóa từ excel");

           await dxPopup.ShowAsync();
        }
        public void Setclass(SanPhamShow sanPhamShow_set, SanPhamShow sanPhamShow_get)
        {

        }
        public async void searchAsync()
        {
            string sql = string.Format(@"
                SELECT ROW_NUMBER() OVER (ORDER BY sp.MaSP) AS Serial,art.[MaSP],sp.TenSP ,[ArticleNumber],mm.Color,mm.TenMau,isnull(sp.Mua,1) as Mua
    
                  FROM [SP].[DataBase_ScansiaPacific2014].[dbo].[ArticleNumberProduct] art
                  inner join [SP].[DataBase_ScansiaPacific2014].dbo.MaMau mm on art.MaMau=mm.MaMau
                  inner join [SP].[DataBase_ScansiaPacific2014].dbo.SanPham sp on sp.MaSP=art.MaSP ");
            string dieukien = "";



            sql = sql + dieukien;
            PanelVisible = true;
            try
            {

                //CallAPI callAPI = new CallAPI();
                //List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                //string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                //if (json != "")
                //{
                //    lstdata = JsonConvert.DeserializeObject<List<SanPhamShow>>(json);
                //    PanelVisible = false;
                //    //Grid.Reload();
                //}


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

    }
}
