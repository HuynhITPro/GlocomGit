using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.Model;
using Microsoft.AspNetCore.Components.Web;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using System.Linq.Expressions;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Urc_NvlInTem
    {
       
        [Inject]
        ToastService toastService { get; set; }
        [Inject] SignalRConnect signalRConnect { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        bool CheckQuyen = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                editContext = new EditContext(nvlInTemShowcrr);
                editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
                CheckQuyen = await phanQuyenAccess.CreateInPhieuNVL(Model.ModelAdmin.users);

                if (ModelAdmin.lsthanghoafilter.Any())
                {
                    lstmahang = ModelAdmin.lsthanghoafilter;
                   
                }
                else
                    lstmahang = await Model.ModelData.GetHangHoa();

                //if(nvlInTemShowcrr!=null)
                //{
                //    if(!string.IsNullOrEmpty(nvlInTemShowcrr.MaHang))
                //    {
                //        var querycheck= lstmahang.Where(p=>p.MaHang.Equals(nvlInTemShowcrr.MaHang)).FirstOrDefault();
                //        if(querycheck==null)
                //        {
                //            var querymahang= lstmahang.FirstOrDefault(x => x.MaHang == nvlInTemShowcrr.MaHang);
                //            ModelAdmin.lsthanghoafilter.Add(querymahang);
                //        }
                //    }
                //}
                if (ModelAdmin.lstnoigiaonhanfilter.Any())
                    lstnoigiaonhan = ModelAdmin.lstnoigiaonhanfilter;
                else
                    lstnoigiaonhan = await Model.ModelData.Getlstnoigiaonhan();

                
                lstdautuan = await Model.ModelData.Getlstdautuan();
                lstchatluong = await Model.ModelData.GetDataDropDownListsAsync("Type_DienGiaiChatLuong");

                // StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load form ke hoach master " + ex.Message);
            }
            finally
            {
            }
        }
        bool checkset = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await loadAsync();

            }
        }
        private async Task loadAsync()
        {
            
            StateHasChanged();
        }

        // Dictionary để ánh xạ tên trường với biểu thức Lambda
        // Dictionary để ánh xạ tên trường với biểu thức Lambda

        private ValidationMessageStore validationMessages;
        private void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {
            if(validationMessages==null)
                validationMessages = new ValidationMessageStore(editContext);

            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();
            if (string.IsNullOrEmpty(nvlInTemShowcrr.MaHang))
            {
                //validationMessages.Add(("MaHang", "Chọn mã hàng."));
                validationMessages.Add(() => nvlInTemShowcrr.MaHang, "Chọn mã hàng.");
            }
            //if (string.IsNullOrEmpty(nvlInTemShowcrr.MaGN))
            //{
            //    //validationMessages.Add(("MaHang", "Chọn mã hàng."));
            //    validationMessages.Add(() => nvlInTemShowcrr.MaGN, "Mã giao nhận không được để trống");
            //}
            if (nvlInTemShowcrr.BanIn==0||nvlInTemShowcrr.BanIn==null)
            {
                //validationMessages.Add(("BanIn", "Bản in phải lớn hơn 0."));

                validationMessages.Add(() => nvlInTemShowcrr.BanIn, "Bản in phải lớn hơn 0.");
            }
            if (nvlInTemShowcrr.SoLuong == 0 || nvlInTemShowcrr.SoLuong == null)
            {
                //validationMessages.Add(("SoLuong", "Nhập số lượng."));
                validationMessages.Add(() => nvlInTemShowcrr.SoLuong, "Nhập số lượng.");
            }
            var querycheck=lstmahang.FirstOrDefault(x => x.MaHang == nvlInTemShowcrr.MaHang);
            if(querycheck!=null)
            {
                if (querycheck.MaNhom == "NVLP_DM" || querycheck.MaNhom == "NVLP_KEO")
                {


                    if (string.IsNullOrEmpty(nvlInTemShowcrr.DauTuan) && nvlInTemShowcrr.NgaySanXuat == null)
                        validationMessages.Add(() => nvlInTemShowcrr.DauTuan, "Nhập dấu tuần hoặc ngày sản xuất.");
                    if (string.IsNullOrEmpty(nvlInTemShowcrr.SoLo))
                    {
                        validationMessages.Add(() => nvlInTemShowcrr.SoLo, "Nhập số lô");
                    }
                }
            }
         
           
            editContext.NotifyValidationStateChanged();
        }
        private async Task<bool> checklogicAsync()
        {
            await txtMaHang.FocusAsync();//Chuyển focus đi để nó commit các giá trị đang focus
            nvlInTemShowcrr.DauTuan = txtDauTuan.GetText;
            //editContext = new EditContext(nvlInTemShowcrr);
            //editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm 
            return  editContext.Validate();
           

        }
        private async Task saveAsync(bool checkprint)
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không có quyền sử dụng chức năng này"));
                return;
            }
            if (!await checklogicAsync())
                return;
            try
            {

                if (checkprint)
                {
                    if(string.IsNullOrEmpty(view_PrintConnectshow.printername))
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, "Chưa kết nối máy in"));
                        return;
                    }
                    if(string.IsNullOrEmpty(view_PrintConnectshow.printbieumau))
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, "In giấy A5 hay in TEM"));
                        return;
                    }
                }
                
                Console.WriteLine(ModelAdmin.users.Mautemin);
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlInPhieu_Insert_Ver2";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@MaHang", nvlInTemShowcrr.MaHang));
                lstpara.Add(new ParameterDefine("@DVT", nvlInTemShowcrr.DVT));
                lstpara.Add(new ParameterDefine("@SoLuong", nvlInTemShowcrr.SoLuong.Value));
                lstpara.Add(new ParameterDefine("@KhachHang_XuatXu", nvlInTemShowcrr.KhachHangXuatXu));
                lstpara.Add(new ParameterDefine("@NgayHetHan", nvlInTemShowcrr.NgayHetHan));
                lstpara.Add(new ParameterDefine("@MaKien", nvlInTemShowcrr.MaKien));
                lstpara.Add(new ParameterDefine("@SoLo", nvlInTemShowcrr.SoLo));
                lstpara.Add(new ParameterDefine("@SoXe", nvlInTemShowcrr.SoXe));
                lstpara.Add(new ParameterDefine("@GhiChu", nvlInTemShowcrr.GhiChu));
                lstpara.Add(new ParameterDefine("@Barcode", nvlInTemShowcrr.Barcode));
                lstpara.Add(new ParameterDefine("@MaGN", nvlInTemShowcrr.MaGN));
                if (checkprint)
                    lstpara.Add(new ParameterDefine("@CheckPrint", "1"));
                else
                    lstpara.Add(new ParameterDefine("@CheckPrint", "0"));
                lstpara.Add(new ParameterDefine("@ChatLuong", nvlInTemShowcrr.ChatLuong));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@DauTuan", nvlInTemShowcrr.DauTuan));
                lstpara.Add(new ParameterDefine("@BanIn", nvlInTemShowcrr.BanIn));
                lstpara.Add(new ParameterDefine("@NgaySanXuat", nvlInTemShowcrr.NgaySanXuat));
                string json = "";
               
                if (checkprint)
                {
                    string typeprint = "";
                    if (view_PrintConnectshow.printbieumau == "Tem 5x10")
                        typeprint = "nvlnhapxuatintem";
                    if (view_PrintConnectshow.printbieumau == "Giấy A5")
                        typeprint = "nvlnhapxuatinphieu";
                    if (view_PrintConnectshow.printbieumau == "Tem 2x5")
                        typeprint = "nvlnhapxuatintem25";
                    if (typeprint=="")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, "In giấy A5 hay in TEM"));
                        return;
                    }
                   
                    json = await callAPI.ProcedureEncryptMsgAsync(sql, lstpara,view_PrintConnectshow.printername, typeprint);
                }
                else
                     json = await callAPI.ProcedureEncryptAsync(sql, lstpara);

                if (json != "")
                {
                    if(json== "Vui lòng nhập topic và id")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng nhập topic và id"));
                        return;
                    }
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query == null)
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi không lưu được")));
                    }
                    else
                    {
                        if (query[0].ketqua=="OK")
                        {
                          
                            toastService.Notify(new ToastMessage(ToastType.Success, "LƯU THÀNH CÔNG"));
                            reset();
                        }
                        else
                        {
                            toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("{0}. Lỗi không lưu được {1}", query[0].ketqua, query[0].ketquaexception)));
                        }
                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Lỗi:" + ex.Message);
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi " + ex.Message));
                //msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
        }
     
        private void reset()
        {
            nvlInTemShowcrr.MaHang = null;
            nvlInTemShowcrr.DauTuan = "";
            nvlInTemShowcrr.SoLuong = null;
            StateHasChanged();
            txtMaHang.FocusAsync();

        }
        private async void Onkeydown(KeyboardEventArgs e)
        {
            if (e.Key == "F11")
            {
                await saveAsync(false);
            }
        }
        private async void ChangeKho()
        {
           
                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_PhanLoaiNhomHang>(0);

                    builder.AddAttribute(1, "GotoMainForm", EventCallback.Factory.Create(this, hidePopup));
                    //builder.OpenComponent(0, componentType);
                    builder.CloseComponent();
                };
           await dxPopup.showAsync("Lọc mã hàng theo kho");
            await dxPopup.ShowAsync();
        }
        private async void ChangeNhomGiaoNhan()
        {
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_PhanLoaiNoiGiaoNhan>(0);

                builder.AddAttribute(1, "GotoMainForm", EventCallback.Factory.Create(this, hidePopupNGN));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            await dxPopup.showAsync("Lọc theo nơi giao nhận");
            await dxPopup.ShowAsync();
        }
        private void hidePopup()
        {
            if(ModelAdmin.lsthanghoafilter.Count > 0) 
                {
                    lstmahang=ModelAdmin.lsthanghoafilter;
                }
           
            dxPopup.CloseAsync();
        }
        private void hidePopupNGN()
        {
            if (ModelAdmin.lstnoigiaonhanfilter.Any())
            {
                lstnoigiaonhan = ModelAdmin.lstnoigiaonhanfilter;
            }

            dxPopup.CloseAsync();
        }
    }

}
