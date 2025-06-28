using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;

using System.ComponentModel.DataAnnotations;
using DevExpress.XtraRichEdit.Import.Html;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Urc_NhapXuatMasterAdd
    {



        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        bool CheckQuyen = false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                editContext = new EditContext(nvlNhapXuatKhoShowcrr);
                editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
                CheckQuyen = await phanQuyenAccess.CreateNhapXuatKho(Model.ModelAdmin.users);
                lstdiengiai=await Model.ModelData.GetDataDropDownListsAsync("Type_DienGiaiCapPhat");
                lstchatluong= await Model.ModelData.GetDataDropDownListsAsync("NVL_ChatLuong");
                if (nvlNhapXuatKhoShowcrr.Serial > 0)
                {
                    EnableEdit = true;
                }
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
            if (lstnoigiaonhan != null && lstlydo != null)
            {
                if (!checkset)
                {
                    checkset = true;

                    StateHasChanged();
                }
            }
            //return base.OnAfterRenderAsync(firstRender);
        }
        private async Task loadAsync()
        {
            lstkhonvl = await Model.ModelData.GetKhoNvl();
            var querylydo = await Model.ModelData.Getlstnvllydo();
            var queryngn = await Model.ModelData.Getlstnoigiaonhan();

            if (LoaiNhapXuat == "NhapKho")
            {
                textgiaonhan = "Nhà cung cấp";
                lstnoigiaonhan = queryngn.Where(p => p.TypeName == "NhaCungCap").ToList();
                lstlydo = querylydo.Where(p => p.TypeName == "NhapKho").ToList();
                KhoTittle = "Tên kho nhập";
            
            }
            if (LoaiNhapXuat == "NhapKhoAll")
            {
                textgiaonhan = "Nhà cung cấp/ nơi giao";
                lstnoigiaonhan = queryngn.Where(p => p.TypeName == "NhaCungCap" || p.TypeName == "NB").ToList();
                lstlydo = querylydo.Where(p => p.TypeName == "NhapKho"|| p.TypeName == "LyDoNo" || p.TypeName == "ChuyenKho").ToList();
                KhoTittle = "Tên kho nhập";

            }
            if (LoaiNhapXuat == "XuatKho")
            {
                textgiaonhan = "Nơi nhận";
                lstnoigiaonhan = queryngn.Where(p => p.TypeName == "KhachHang" || p.TypeName == "NB"||p.TypeName=="Kho").ToList();
                lstlydo = querylydo.Where(p => p.TypeName == "XuatKho").ToList();
                KhoTittle = "Tên kho xuất";


            }
            if (LoaiNhapXuat == "XuatKhoAll")
            {
                textgiaonhan = "Nơi nhận";

                lstnoigiaonhan = queryngn.Where(p => p.TypeName == "KhachHang" || p.TypeName == "NB").ToList();
                lstlydo = querylydo.Where(p => p.TypeName == "XuatKho"|| p.TypeName == "LyDoNo" || p.TypeName == "ChuyenKho" || p.TypeName == "XuatHuyTra").ToList();
                KhoTittle = "Tên kho xuất";
            }
            if (LoaiNhapXuat == "XuatGhiNo")
            {
                textgiaonhan = "Nơi nhận";
                KhoTittle = "Tên kho xuất";
                lstnoigiaonhan = queryngn.Where(p => p.TypeName == "KhachHang" || p.TypeName == "NB").ToList();
                lstlydo = querylydo.Where(p => p.TypeName == "LyDoNo").ToList();
                

            }
            if (LoaiNhapXuat == "XuatHuyTra")
            {
                textgiaonhan = "Nơi nhận";
                lstnoigiaonhan = queryngn.Where(p => p.TypeName == "NhaCungCap" || p.TypeName == "NB").ToList();
                lstlydo = querylydo.Where(p => p.TypeName == "XuatHuyTra").ToList();
                KhoTittle = "Tên kho xuất";

            }

            if (LoaiNhapXuat == "NhapGhiNo")
            {
                textgiaonhan = "Nhà cung cấp/ người trả";
                lstnoigiaonhan = queryngn.Where(p => p.TypeName == "NhaCungCap" || p.TypeName == "NB").ToList();
                lstlydo = querylydo.Where(p => p.TypeName == "LyDoNo").ToList();
                KhoTittle = "Tên kho nhập";


            }
            if (LoaiNhapXuat == "NhapHuyTra")
            {
                textgiaonhan = "Nhà cung cấp/ người trả";
                lstnoigiaonhan = queryngn.Where(p => p.TypeName == "NhaCungCap" || p.TypeName == "NB").ToList();
                lstlydo = querylydo.Where(p => p.TypeName == "XuatHuyTra").ToList();
                KhoTittle = "Tên kho nhập";

            }
            if (LoaiNhapXuat == "NhapKiemKe")
            {
                textgiaonhan = "Nhà cung cấp/ nơi giao";
                lstnoigiaonhan = queryngn.Where(p => p.TypeName == "NhaCungCap" || p.TypeName == "NB").ToList();
                lstlydo = querylydo.Where(p => p.TypeName == "NhapKiemKe").ToList();

                KhoTittle = "Tên kho nhập";
            }
            if (LoaiNhapXuat == "ChuyenKho")
            {
                textgiaonhan = "Kho nhận hàng";
                lstnoigiaonhan = queryngn.Where(p =>  p.TypeName == "Kho").ToList();
                lstlydo = querylydo.Where(p => p.TypeName == "ChuyenKho").ToList();
               
                if (nvlNhapXuatKhoShowcrr.Serial == 0)//Mặc định cho 2 kho này luôn cho tránh nhầm lẫn
                {
                    KhoTittle = "Tên kho xuất";
                    nvlNhapXuatKhoShowcrr.MaKho = "K002";
                    nvlNhapXuatKhoShowcrr.MaGN = "K007";
                }
                   


            }
            if (LoaiNhapXuat == "NhapGiaCong")
            {
                textgiaonhan = "Kho giao hàng/ nơi đóng vỉ";
                lstnoigiaonhan = queryngn.Where(p => p.TypeName == "Kho").ToList();
                lstlydo = querylydo.Where(p => p.TypeName == "NhapGiaCong").ToList();
               
                if (nvlNhapXuatKhoShowcrr.Serial == 0)//Mặc định cho 2 kho này luôn cho tránh nhầm lẫn
                {
                    KhoTittle = "Tên kho nhập";
                    nvlNhapXuatKhoShowcrr.MaKho = "K002";
                    nvlNhapXuatKhoShowcrr.MaGN = "K007";
                }
            }
        }

        private void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {

            var validationMessages = new ValidationMessageStore(editContext);

            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();

            // Thực hiện kiểm tra tùy chỉnh, ví dụ kiểm tra giá trị null cho KhuVuc
            if (string.IsNullOrEmpty(nvlNhapXuatKhoShowcrr.LyDo))
            {
                validationMessages.Add(() => nvlNhapXuatKhoShowcrr.LyDo, "Lý do không được để trống.");
            }
            if (string.IsNullOrEmpty(nvlNhapXuatKhoShowcrr.MaGN))
            {
                validationMessages.Add(() => nvlNhapXuatKhoShowcrr.MaGN, "Nơi giao nhận không được để trống.");
            }
            if (string.IsNullOrEmpty(nvlNhapXuatKhoShowcrr.NhaMay))
            {
                validationMessages.Add(() => nvlNhapXuatKhoShowcrr.NhaMay, "Nhà máy không được để trống.");
            }
            if (string.IsNullOrEmpty(nvlNhapXuatKhoShowcrr.MaKho))
            {
                validationMessages.Add(() => nvlNhapXuatKhoShowcrr.MaKho, "Mã kho không được để trống.");
            }
            if (nvlNhapXuatKhoShowcrr.Ngay == null)
            {
                validationMessages.Add(() => nvlNhapXuatKhoShowcrr.Ngay, "Ngày chứng từ không được để trống.");
            }
            // Lưu kết quả validation
            editContext.NotifyValidationStateChanged();
        }
        private bool checklogic()
        {
            editContext = new EditContext(nvlNhapXuatKhoShowcrr);
            editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
            return editContext.Validate();

        }
       

        private async Task saveAsync()
        {
            if(!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không có quyền sử dụng chức năng này"));
                return;
            }
            if (!checklogic())
                return;
            try
            {
                CallAPI callAPI = new CallAPI();

                ModelAdmin.MaKhoSelected = nvlNhapXuatKhoShowcrr.MaKho;//Sẽ lưu lại mã kho để sử dụng măc định cho những form sau
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string sql = "NVLDB.dbo.NvlNhapXuat_Insert_Ver3";

                lstpara.Add(new ParameterDefine("@Serial", nvlNhapXuatKhoShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatKhoShowcrr.MaKho));
                lstpara.Add(new ParameterDefine("@MaGN", nvlNhapXuatKhoShowcrr.MaGN));
                lstpara.Add(new ParameterDefine("@LyDo", nvlNhapXuatKhoShowcrr.LyDo));
                lstpara.Add(new ParameterDefine("@PONumber", ""));
                lstpara.Add(new ParameterDefine("@GhiChu", nvlNhapXuatKhoShowcrr.GhiChu));
                lstpara.Add(new ParameterDefine("@DienGiai", nvlNhapXuatKhoShowcrr.DienGiai));
                lstpara.Add(new ParameterDefine("@Ngay", nvlNhapXuatKhoShowcrr.Ngay.Value));
                lstpara.Add(new ParameterDefine("@NhaMay", nvlNhapXuatKhoShowcrr.NhaMay));
                lstpara.Add(new ParameterDefine("@Xacnhan", "0"));
                lstpara.Add(new ParameterDefine("@NguoiDN",""));
                lstpara.Add(new ParameterDefine("@ChatLuong", nvlNhapXuatKhoShowcrr.ChatLuong));
                lstpara.Add(new ParameterDefine("@MaDN", ""));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                string typeNX = "nhap";
                if (LoaiNhapXuat=="XuatKho" || LoaiNhapXuat=="XuatGhiNo" || LoaiNhapXuat=="XuatHuyTra")
                {
                    typeNX = "xuat";
                }
                if (LoaiNhapXuat=="NhapKho" || LoaiNhapXuat=="NhapGhiNo")
                {
                    typeNX = "nhap";
                }
                if (LoaiNhapXuat == "ChuyenKho")
                    typeNX = "chuyen";
                if(LoaiNhapXuat=="NhapGiaCong")
                {
                    typeNX = "nhapgiacong";
                }
                lstpara.Add(new ParameterDefine("@TypeNhapXuat", typeNX));

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (int.TryParse(query[0].ketqua, out int result))
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "LƯU THÀNH CÔNG"));
                        //reset();
                        nvlNhapXuatKhoShowcrr.Serial = int.Parse(query[0].ketqua);
                        if (AfterSave.HasDelegate)
                        {
                          await  AfterSave.InvokeAsync(nvlNhapXuatKhoShowcrr);
                        }
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, string.Format("Lỗi:{0}, {1} " , query[0].ketqua, query[0].ketquaexception)));
                        //reset();
                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Lỗi:" + ex.Message);
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi " + ex.Message));
                //msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
        }
        private async Task updateAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không có quyền sử dụng chức năng này"));
                return;
            }
            if (!checklogic())
                return;
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlNhapXuat_Update_Ver2";

                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlNhapXuatKhoShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatKhoShowcrr.MaKho));
                lstpara.Add(new ParameterDefine("@MaGN", nvlNhapXuatKhoShowcrr.MaGN));
                lstpara.Add(new ParameterDefine("@LyDo", nvlNhapXuatKhoShowcrr.LyDo));
                lstpara.Add(new ParameterDefine("@PONumber", ""));
                lstpara.Add(new ParameterDefine("@GhiChu", nvlNhapXuatKhoShowcrr.GhiChu));
                lstpara.Add(new ParameterDefine("@DienGiai", nvlNhapXuatKhoShowcrr.DienGiai));
                lstpara.Add(new ParameterDefine("@Ngay", nvlNhapXuatKhoShowcrr.Ngay.Value));
                lstpara.Add(new ParameterDefine("@Xacnhan", "0"));
                lstpara.Add(new ParameterDefine("@NguoiDN", ""));
                lstpara.Add(new ParameterDefine("@MaDN", ""));
                lstpara.Add(new ParameterDefine("@ChatLuong", nvlNhapXuatKhoShowcrr.ChatLuong));
                lstpara.Add(new ParameterDefine("@NhaMay", nvlNhapXuatKhoShowcrr.NhaMay));
                lstpara.Add(new ParameterDefine("@UserInsert",ModelAdmin.users.UsersName));
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "SỬA thành công"));
                        if (AfterEdit.HasDelegate)
                        {
                            await AfterEdit.InvokeAsync(nvlNhapXuatKhoShowcrr);
                        }
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception)));
                    }
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi: " + ex.Message, IconMsg.iconerror));
            }
        }
        private void reset()
        {


            StateHasChanged();

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
