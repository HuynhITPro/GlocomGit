using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;
using NFCWebBlazor.Models;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucHangHoaMaster;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;
using Microsoft.AspNetCore.Components.Web;
using NFCWebBlazor.App_ClassDefine;
using Microsoft.IdentityModel.Tokens;


namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Urc_NhapXuatItemAdd
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        [Inject] BrowserService browserService { get; set; }

        bool CheckQuyen = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                editContext = new EditContext(nvlNhapXuatItemShowcrr);
                editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
                CheckQuyen = await phanQuyenAccess.CreateNhapXuatKho(Model.ModelAdmin.users);
                lstghichu = await Model.ModelData.GetDataDropDownListsAsync("Type_GhiChuNhapXuatItem");
                if (nvlNhapXuatItemShowcrr.Serial > 0)
                {
                    EnableEdit = true;
                    EnableSerial = false;
                    if (nvlNhapXuatItemShowcrr.SerialKHDH != null)
                    {
                        DonHangSelect donHangSelect = new DonHangSelect();
                        donHangSelect.Serial = nvlNhapXuatItemShowcrr.SerialKHDH.Value;
                        donHangSelect.SLTheoDoi = nvlNhapXuatItemShowcrr.SoLuong.Value;
                        donHangSelect.TenUser = ModelAdmin.users.TenUser;
                        lstdonhang.Add(donHangSelect);
                        donhangselected = new List<DonHangSelect>() { donHangSelect };

                    }

                }
                if (nvlNhapXuatKhoShowcrr.flag > 0)
                {
                    lstsanpham = await Model.ModelData.GetSanPham();
                }
                if (nvlNhapXuatKhoShowcrr.STTCT < 0)
                {
                    displaycapvuot = "flex";
                }
               
                await loadAsync();
                var dimension = await browserService.GetDimensions();
                // var heighrow = await browserService.GetHeighWithID("divcontainer");
                int height = dimension.Height - 70;
                int width = dimension.Width;
                if (width < 768)
                {
                    Ismobile = true;
                    
                }
                else
                {
                    Ismobile = false;
                   
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
                if (nvlNhapXuatItemShowcrr.Serial > 0)
                {
                   
                }
            }
            if (!checkset)
            {
                if (lstdautuan != null && lstViTri != null)
                {
                    if (nvlNhapXuatItemShowcrr.ViTri == null)
                        nvlNhapXuatItemShowcrr.ViTri = "Temp";
                    if (nvlNhapXuatItemShowcrr.DauTuan != null)
                    {
                        string[] arr = nvlNhapXuatItemShowcrr.DauTuan.Split(';');
                        dautuanselected = new List<DataDropDownList>();
                        dautuanselected = arr.Select(p => new DataDropDownList { Name = p, FullName = p }).ToList();
                    }
                    checkset = true;
                    StateHasChanged();
                }
            }

            //return base.OnAfterRenderAsync(firstRender);
        }
        private async Task loadAsync()
        {
            lstdautuan = await Model.ModelData.Getlstdautuan();
            if (lstViTri == null)
            {
                var query = await Model.ModelData.GetListViTri();
                string MaKho = nvlNhapXuatKhoShowcrr.MaKho;
                lstViTri = query.Where(p => p.MaKho.Equals(MaKho)).ToList();
            }
            if (nvlNhapXuatKhoShowcrr.STTCT > 0)//Form nhập
            {
                await LoadIDTemAsync();
            }
        }
        private async Task loadlaivitriAsync()
        {
            string MaKho = "";
            if (nvlNhapXuatKhoShowcrr != null)
            {
                if (nvlNhapXuatKhoShowcrr.MaKho != null)
                {
                    MaKho = nvlNhapXuatKhoShowcrr.MaKho;
                }
            }
            if (MaKho != "")
            {
                var query = await Model.ModelData.GetListViTri();
                lstViTri = query.Where(p => p.MaKho.Equals(MaKho)).ToList();
            }
            else
                lstViTri = await Model.ModelData.GetListViTri();
            StateHasChanged();
        }
        ValidationMessageStore validationMessages { get; set; }
        bool checksave = true;//Gán biến để tránh trường hợp mạng lag API gọi lại 2 lần
        private async void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {
            if (validationMessages == null)
                validationMessages = new ValidationMessageStore(editContext);
            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();
            if (nvlNhapXuatKhoShowcrr.STTCT == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không xác định được chứng từ nhập hay xuất"));
                return;
            }

            if (nvlNhapXuatItemShowcrr.SerialLink == null || nvlNhapXuatItemShowcrr.SerialLink <= 0)
            {
                validationMessages.Add(() => nvlNhapXuatItemShowcrr.SerialLink, "Vui lòng quét tem.");
            }
            //Thực hiện kiểm tra tùy chỉnh, ví dụ kiểm tra giá trị null cho KhuVuc
            if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
            {
                validationMessages.Add(() => nvlNhapXuatItemShowcrr.MaHang, "Mã hàng không được để trống.");
            }
            if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.ViTri))
            {
                validationMessages.Add(() => nvlNhapXuatItemShowcrr.ViTri, "Chọn vị trí.");
            }
            if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.DVT))
            {
                validationMessages.Add(() => nvlNhapXuatItemShowcrr.DVT, "Thiếu ĐVT.");
            }
            if (nvlNhapXuatItemShowcrr.SoLuong == null || (nvlNhapXuatItemShowcrr.SoLuong < 0&& !CheckDuplicate))//Do binding theo cột số lượng
            {
                validationMessages.Add(() => nvlNhapXuatItemShowcrr.SoLuong, "Vui lòng nhập  số lượng.");
            }
            if (!chkNgoaiKeHoach)
            {
                if (donhangselected == null)
                {
                    validationMessages.Add(() => nvlNhapXuatItemShowcrr.SerialKHDH, "Vui lòng chọn đơn hàng/kế hoạch.");
                }
                else
                {
                    if (tongdonhangselect == null)
                    {
                        validationMessages.Add(() => nvlNhapXuatItemShowcrr.SerialKHDH, "Vui lòng chọn đơn hàng/kế hoạch.");
                    }
                    else
                    {
                        if (!chkVuotDeNghi)//Không xin cấp vượt
                        {
                            if (tongdonhangselect.Value < nvlNhapXuatItemShowcrr.SoLuong.Value)
                                validationMessages.Add(() => nvlNhapXuatItemShowcrr.SerialKHDH, "Số lượng đơn hàng/kế hoạch đang nhỏ hơn số lượng nhập.");
                        }
                    }
                }
            }
            if (LoaiNhapXuat == "NhapGiaCong")
            {
                if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaSP))
                {
                    validationMessages.Add(() => nvlNhapXuatItemShowcrr.MaSP, "Vui lòng nhập Mã SP");
                }

            }

            // Lưu kết quả validation

            editContext.NotifyValidationStateChanged();
        }
        private bool checklogic()
        {
            if (!checksave)
                return false;
            return editContext.Validate();

        }
        public class DonHangSelect
        {
            public int? Serial { get; set; }
            public int? SerialDN { get; set; }
            public decimal? SLTheoDoi { get; set; }
            public decimal? DonGia { get; set; }
            private string _pathimg { get; set; }
            public string PathImg
            {
                get { return _pathimg; }
                set
                {
                    _pathimg = value;
                    if (string.IsNullOrEmpty(_pathimg))
                    {
                        _pathimg = IconImg.User;
                    }
                    else
                    {
                        _pathimg = Model.ModelAdmin.pathurlfilepublic + value;
                    }
                }
            }
            public DonHangSelect CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<DonHangSelect>(json);
            }
            public string TenUser { get; set; }
        }
        public class SelectSerial
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NvlNhapXuatItemShow> lstnhapxuat { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<DonHangSelect> lstdonhangselect { get; set; }

            [JsonProperty("Table2")]
            public List<SanphamSelect> lstsp { get; set; }

        }
        decimal? giathamkhao = 0;
        private async Task checkSerialAsync(string Serial)
        {
            if(string.IsNullOrEmpty(Serial))
            {
                nvlNhapXuatItemShowcrr.SerialLink = null;
                return;
            }
            if (!int.TryParse(Serial, out int serialout))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Giá trị nhập phải là số"));
                return;
            }
            nvlNhapXuatItemShowcrr.SerialLink = serialout;
            string typenhapxuat = "nhapkho";
            if (nvlNhapXuatKhoShowcrr.STTCT < 0)
            {
                typenhapxuat = "xuatkho";

                //Tạm thời thì xuất kho cho ngoài kế hoạch hết
                //chkNgoaiKeHoach = true;
            }
            if (LoaiNhapXuat.Contains("GhiNo"))
            {
                //chkNgoaiKeHoach = true;
            }
            giathamkhao = 0;

            string sql = string.Format(@"Use NVLDB declare @SerialIn int={0}
                        declare @MaNCCIn nvarchar(100)=N'{1}'
                        declare @TypeKeHoachIn nvarchar(100)=N'{2}'
                        declare @NhaMayIn nvarchar(100)=N'{3}'
                        declare @MaKhoIn nvarchar(100)=N'{4}'
                        exec NvlNhapXuat_GetBarcode_Ver3 @Serial=@SerialIn,@MaNCC=@MaNCCIn,@TypeKeHoach=@TypeKeHoachIn,@NhaMay=@NhaMayIn,@MaKho=@MaKhoIn

", Serial, nvlNhapXuatKhoShowcrr.MaGN, typenhapxuat, nvlNhapXuatKhoShowcrr.NhaMay, nvlNhapXuatKhoShowcrr.MaKho);

            //lstdonhang.Clear();
            if(donhangselected==null)
                donhangselected= new List<DonHangSelect>();
            
            //donhangselected = new List<DonHangSelect>();
            dautuanselected = null;
            tongdonhangselect = 0;
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            //lstpara.Add(new ParameterDefine("@Serial", Serial));
            //lstpara.Add(new ParameterDefine("@MaNCC", nvlNhapXuatKhoShowcrr.MaGN)); 
            CallAPI callAPI = new CallAPI();
            string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);

            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<SelectSerial>(json);
                if (query != null)
                {
                    if (query.lstnhapxuat.Any())
                    {

                        nvlNhapXuatItemShowcrr = query.lstnhapxuat.FirstOrDefault();
                        txtDauTuan.SetSelectedValue = nvlNhapXuatItemShowcrr.DauTuan;
                        lstdonhang = query.lstdonhangselect;
                        donhangselected = null;

                        List<DonHangSelect> lstdonhangselectnew = new List<DonHangSelect>();
                        //Xử lý gán đơn hàng cũ
                        foreach (var it in lstdonhangkeep)
                        {
                            foreach (var item in lstdonhang)
                            {
                                if (item.SerialDN == it.SerialDN)
                                {
                                    lstdonhangselectnew.Add(item.CopyClass());
                                }
                            }
                        }
                        
                        if (nvlNhapXuatKhoShowcrr.STTCT > 0)//Chứng từ nhập kho
                            nvlNhapXuatItemShowcrr.ViTri = vitrikeep;
                        MaHangImg = nvlNhapXuatItemShowcrr.MaHang;
                        if (nvlNhapXuatItemShowcrr.SLNo > 0 && nvlNhapXuatKhoShowcrr.STTCT < 0)//Chứng từ xuất kho và có ghi nợ
                        {
                            visibleTheoDoiNo = true;
                        }
                        else
                        {
                            visibleTheoDoiNo = false;
                        }
                        if (nvlNhapXuatKhoShowcrr.flag > 0)
                        {
                            if (query.lstsp != null)
                            {
                                lstsanphamfilter.Clear();

                                foreach (var it in query.lstsp)
                                {
                                    foreach (var item in lstsanpham)
                                    {
                                        if (it.MaSP == item.MaSP)
                                        {
                                            SanPhamDropDown sanPhamDropDown = new SanPhamDropDown();
                                            sanPhamDropDown.MaSP = item.MaSP;
                                            sanPhamDropDown.TenSP = item.TenSP;
                                            lstsanphamfilter.Add(sanPhamDropDown);
                                            break;
                                        }
                                    }
                                }
                                //lstsanphamfilter.AddRange(query.lstsp.ToList());
                                if (lstsanphamfilter.Count == 1)
                                {
                                    nvlNhapXuatItemShowcrr.MaSP = lstsanphamfilter[0].MaSP;
                                }
                                if (lstsanphamfilter.Count == 0)
                                {
                                    foreach (var it in query.lstsp)
                                    {
                                        SanPhamDropDown sanPhamDropDown = new SanPhamDropDown();
                                        sanPhamDropDown.MaSP = it.MaSP;
                                        sanPhamDropDown.TenSP = it.MaSP;
                                        lstsanphamfilter.Add(sanPhamDropDown);
                                        nvlNhapXuatItemShowcrr.MaSP = it.MaSP;
                                        break;
                                    }
                                }
                            }

                            //await loadsanphamdinhmucAsync(nvlHangHoaDropDown.MaHang);
                        }
                        if (lstdonhangselectnew.Any())
                        {
                           
                            donhangselected = lstdonhang.Where(p=>p.SerialDN==lstdonhangselectnew[0].SerialDN);
                            await dxTagBoxdonhang.SelectedItemsChanged.InvokeAsync(donhangselected);
                          
                           
                        }
                        await dxTagBoxdonhang.FocusAsync();
                        StateHasChanged();
                        //Tạm thời sử dụng hàm này:

                    }
                    else
                        toastService.Notify(new ToastMessage(ToastType.Warning, "Mã vạch không đúng"));

                    //query.lstdonhangselect.Clear();


                }
               // StateHasChanged();


            }


        }
        public async void ShowFlyout()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
           
            if (string.IsNullOrEmpty(MaHangImg))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng nhập mã hàng"));
                return;
            }
            IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();
            await loadtonkhohAsync();
           

        }
        private void SelectDonHangclick(IEnumerable<DonHangSelect> lstselected)
        {
            
            try
            {
                //donhangselected= lstselected;
                tongdonhangselect = lstselected.Sum(p => p.SLTheoDoi.Value);
                if (tongdonhangselect <= nvlNhapXuatItemShowcrr.SLConLai)
                {
                    nvlNhapXuatItemShowcrr.SoLuong = tongdonhangselect;
                }
                if(lstselected.Count()==0)
                {
                    lstdonhang=lstdonhang.ToList();
                    //nvlNhapXuatItemShowcrr.DonGia = 0;
                }
                else
                {
                    var query = lstdonhang.LastOrDefault();
                    if(query!=null)
                    {
                        if(query.DonGia!=null)
                        {
                            nvlNhapXuatItemShowcrr.DonGia = query.DonGia;
                        }
                    }
                }
                
                //Console.WriteLine("lst don hang: "+lstdonhang.Count());
                //Console.WriteLine("don hang selected: " + donhangselected.Count());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Lỗi:"+ex.Message.ToString());
            }
            StateHasChanged();
        }
        List<ParameterDefine> lstpara = new List<ParameterDefine>();
        string lydochapnhan = "";
        private void setParameter(NvlNhapXuatItemShow nvlNhapXuatItem)
        {

            lstpara.Clear();
            lstpara.Add(new ParameterDefine("@Serial", nvlNhapXuatItem.Serial));
            lstpara.Add(new ParameterDefine("@SerialCT", nvlNhapXuatKhoShowcrr.Serial));
            lstpara.Add(new ParameterDefine("@SerialLink", nvlNhapXuatItem.SerialLink));
            string SerialKHDH = "";
            if (chkNgoaiKeHoach == false)//có yêu cầu kế hoạch
            {
                foreach (var it in donhangselected)
                {
                    SerialKHDH = string.Join(";", donhangselected.Select(p => p.Serial.Value.ToString()));
                }
                if (string.IsNullOrEmpty(SerialKHDH))
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Chưa chọn đơn hàng"));
                    return;
                }
            }

            nvlNhapXuatItemShowcrr.DVTTT = nvlNhapXuatItemShowcrr.DVT;
            nvlNhapXuatItem.TyLeQuyDoi = 1;
            if (nvlNhapXuatKhoShowcrr.STTCT > 0)//Phiếu nhập
            {
                nvlNhapXuatItemShowcrr.SLNhap = nvlNhapXuatItemShowcrr.SoLuong;
                nvlNhapXuatItemShowcrr.SLNhapTT = nvlNhapXuatItemShowcrr.SLNhap;
                nvlNhapXuatItemShowcrr.SLXuat = 0;
                nvlNhapXuatItemShowcrr.SLXuatTT = nvlNhapXuatItemShowcrr.SLXuat;
                nvlNhapXuatItemShowcrr.TableName = "NvlDonDatHangItem";
            }
            else
            {
                nvlNhapXuatItemShowcrr.SLNhap = 0;
                nvlNhapXuatItemShowcrr.SLNhapTT = nvlNhapXuatItemShowcrr.SLNhap;
                nvlNhapXuatItemShowcrr.SLXuat = nvlNhapXuatItemShowcrr.SoLuong;
                nvlNhapXuatItemShowcrr.SLXuatTT = nvlNhapXuatItemShowcrr.SLXuat;
                nvlNhapXuatItemShowcrr.TableName = "NvlKeHoachMuaHangItem";
            }
            if (string.IsNullOrEmpty(SerialKHDH))//Nếu là dữ liệu không phải được nhập từ kế hoạch hoặc đơn hàng thì đơn giá tính theo người nhập input
            {
                if (nvlNhapXuatItemShowcrr.DonGia == null)
                    nvlNhapXuatItemShowcrr.DonGia = 0;
            }
            else
            {
                nvlNhapXuatItemShowcrr.DonGia = 0;//Đơn giá sẽ được hệ thống xử lý trực tiếp từ đơn hàng
            }

            //@SerialKHDH đây là 1 chuỗi gồm các mã đơn hàng được ngăn cách bằng dấu ;
            lstpara.Add(new ParameterDefine("@SerialKHDH", SerialKHDH));
            lstpara.Add(new ParameterDefine("@TableName", nvlNhapXuatItem.TableName));
            lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItem.MaHang));
            lstpara.Add(new ParameterDefine("@DVT", nvlNhapXuatItem.DVT));
            lstpara.Add(new ParameterDefine("@SLNhap", nvlNhapXuatItem.SLNhap));
            lstpara.Add(new ParameterDefine("@SLXuat", nvlNhapXuatItem.SLXuat));
            lstpara.Add(new ParameterDefine("@DonGia", nvlNhapXuatItem.DonGia));
            lstpara.Add(new ParameterDefine("@SLNhapTT", nvlNhapXuatItem.SLNhapTT));
            lstpara.Add(new ParameterDefine("@SLXuatTT", nvlNhapXuatItem.SLXuatTT));
            lstpara.Add(new ParameterDefine("@DVTTT", nvlNhapXuatItem.DVTTT));
            lstpara.Add(new ParameterDefine("@TyLeQuyDoi", nvlNhapXuatItem.TyLeQuyDoi));
            lstpara.Add(new ParameterDefine("@KhachHang_XuatXu", nvlNhapXuatItem.KhachHang_XuatXu));
            lstpara.Add(new ParameterDefine("@NgayHetHan", nvlNhapXuatItem.NgayHetHan));
            lstpara.Add(new ParameterDefine("@MaKien", nvlNhapXuatItem.MaKien));
            lstpara.Add(new ParameterDefine("@SoLo", nvlNhapXuatItem.SoLo));
            lstpara.Add(new ParameterDefine("@SoXe", nvlNhapXuatItem.SoXe));
            lstpara.Add(new ParameterDefine("@GhiChu", nvlNhapXuatItem.GhiChu));
            lstpara.Add(new ParameterDefine("@Barcode", nvlNhapXuatItem.Barcode));
            lstpara.Add(new ParameterDefine("@MaSP", nvlNhapXuatItem.MaSP));
            lstpara.Add(new ParameterDefine("@ArticleNumber", nvlNhapXuatItem.ArticleNumber));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
            lstpara.Add(new ParameterDefine("@DauTuan", txtDauTuan.GetText));
            lstpara.Add(new ParameterDefine("@ViTri", nvlNhapXuatItem.ViTri));

            lstpara.Add(new ParameterDefine("@NgaySanXuat", nvlNhapXuatItem.NgaySanXuat));
            if (CheckDuplicate)//Cho phép trùng
                lydochapnhan = "Nhập vào tem có sẵn";
            lstpara.Add(new ParameterDefine("@LyDoChapNhan", lydochapnhan));
        }
        private void setparameterChuyen(NvlNhapXuatItemShow nvlNhapXuatItem)
        {


            lstpara.Clear();
            lstpara.Add(new ParameterDefine("@Serial", nvlNhapXuatItem.Serial));
            lstpara.Add(new ParameterDefine("@SerialCT", nvlNhapXuatKhoShowcrr.Serial));
            lstpara.Add(new ParameterDefine("@SerialLink", nvlNhapXuatItem.SerialLink));
            string SerialKHDH = "";
            if (chkNgoaiKeHoach == false)//có yêu cầu kế hoạch
            {
                foreach (var it in donhangselected)
                {
                    SerialKHDH = string.Join(";", donhangselected.Select(p => p.Serial.Value.ToString()));
                }
                if (string.IsNullOrEmpty(SerialKHDH))
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Chưa chọn đơn hàng"));
                    return;
                }
            }

            nvlNhapXuatItemShowcrr.DVTTT = nvlNhapXuatItemShowcrr.DVT;
            nvlNhapXuatItem.TyLeQuyDoi = 1;
            if (nvlNhapXuatKhoShowcrr.STTCT > 0)//Phiếu nhập
            {
                nvlNhapXuatItemShowcrr.SLNhap = nvlNhapXuatItemShowcrr.SoLuong;
                nvlNhapXuatItemShowcrr.SLNhapTT = nvlNhapXuatItemShowcrr.SLNhap;
                nvlNhapXuatItemShowcrr.SLXuat = 0;
                nvlNhapXuatItemShowcrr.SLXuatTT = nvlNhapXuatItemShowcrr.SLXuat;
                nvlNhapXuatItemShowcrr.TableName = "NvlDonDatHangItem";
            }
            else
            {
                nvlNhapXuatItemShowcrr.SLNhap = 0;
                nvlNhapXuatItemShowcrr.SLNhapTT = nvlNhapXuatItemShowcrr.SLNhap;
                nvlNhapXuatItemShowcrr.SLXuat = nvlNhapXuatItemShowcrr.SoLuong;
                nvlNhapXuatItemShowcrr.SLXuatTT = nvlNhapXuatItemShowcrr.SLXuat;
                nvlNhapXuatItemShowcrr.TableName = "NvlKeHoachMuaHangItem";
            }
            if (string.IsNullOrEmpty(SerialKHDH))//Nếu là dữ liệu không phải được nhập từ kế hoạch hoặc đơn hàng thì đơn giá tính theo người nhập input
            {
                if (nvlNhapXuatItemShowcrr.DonGia == null)
                    nvlNhapXuatItemShowcrr.DonGia = 0;
            }
            else
            {
                nvlNhapXuatItemShowcrr.DonGia = 0;//Đơn giá sẽ được hệ thống xử lý trực tiếp từ đơn hàng
            }

            //@SerialKHDH đây là 1 chuỗi gồm các mã đơn hàng được ngăn cách bằng dấu ;
            lstpara.Add(new ParameterDefine("@SerialKHDH", SerialKHDH));
            lstpara.Add(new ParameterDefine("@TableName", nvlNhapXuatItem.TableName));
            lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItem.MaHang));
            lstpara.Add(new ParameterDefine("@DVT", nvlNhapXuatItem.DVT));
            lstpara.Add(new ParameterDefine("@SLNhap", nvlNhapXuatItem.SLNhap));
            lstpara.Add(new ParameterDefine("@SLXuat", nvlNhapXuatItem.SLXuat));
            lstpara.Add(new ParameterDefine("@DonGia", nvlNhapXuatItem.DonGia));
            lstpara.Add(new ParameterDefine("@SLNhapTT", nvlNhapXuatItem.SLNhapTT));
            lstpara.Add(new ParameterDefine("@SLXuatTT", nvlNhapXuatItem.SLXuatTT));
            lstpara.Add(new ParameterDefine("@DVTTT", nvlNhapXuatItem.DVTTT));
            lstpara.Add(new ParameterDefine("@TyLeQuyDoi", nvlNhapXuatItem.TyLeQuyDoi));
            lstpara.Add(new ParameterDefine("@KhachHang_XuatXu", nvlNhapXuatItem.KhachHang_XuatXu));
            lstpara.Add(new ParameterDefine("@NgayHetHan", nvlNhapXuatItem.NgayHetHan));
            lstpara.Add(new ParameterDefine("@MaKien", nvlNhapXuatItem.MaKien));
            lstpara.Add(new ParameterDefine("@SoLo", nvlNhapXuatItem.SoLo));
            lstpara.Add(new ParameterDefine("@SoXe", nvlNhapXuatItem.SoXe));
            lstpara.Add(new ParameterDefine("@GhiChu", nvlNhapXuatItem.GhiChu));
            lstpara.Add(new ParameterDefine("@Barcode", nvlNhapXuatItem.Barcode));
            lstpara.Add(new ParameterDefine("@MaSP", nvlNhapXuatItem.MaSP));
            lstpara.Add(new ParameterDefine("@ArticleNumber", nvlNhapXuatItem.ArticleNumber));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
            if (chkVuotDeNghi)
            {
                lstpara.Add(new ParameterDefine("@CapVuot", 1));
            }
            else
            {
                lstpara.Add(new ParameterDefine("@CapVuot", 0));
            }

            if (CheckDuplicate)//Cho phép trùng
                lydochapnhan = "Dồn vào tem có sẵn";
            lstpara.Add(new ParameterDefine("@LyDoChapNhan", lydochapnhan));



        }
        List<DataDropDownList> lstIDTem = new List<DataDropDownList>();
        private async Task LoadIDTemAsync()
        {
            string sql = string.Format(@"use NVLDB declare @UserInsert nvarchar(100)=N'{0}'
                        declare @Serial int
                        declare @DateBegin date=dateadd(dd,-15,getdate())
                        select top 1 @Serial=Serial from NvlNhapXuatItem where NgayInsert>=@DateBegin
                        SELECT top 150  [Serial] as [Name],Serial as FullName,'ID' as TypeName
                        FROM [NvlInPhieu]
                        where UserInsert=@UserInsert and NgayInsert>=@DateBegin and Serial not in (select SerialLink from NvlNhapXuatItem where Serial>@Serial)
                        order by Serial desc", ModelAdmin.users.UsersName);
            CallAPI callAPI = new CallAPI();


            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {
                lstIDTem.Clear();
                var query = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                lstIDTem.AddRange(query);
            }


            //ModelData.GetID
        }
        string vitrikeep = "";
        private async Task saveAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không có quyền sử dụng chức năng này"));
                return;
            }
            if (!checklogic())
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Dữ liệu nhập không đúng. Vui lòng kiểm tra những dòng tô màu đỏ"));
                return;
            }
            try
            {
                checksave = false;//Tránh để call 2 lần do API chậm
                await dxTextBoxMaKien.FocusAsync();
                string sql = "";
                CallAPI callAPI = new CallAPI();

                //Xử lý kiểm tra xem phải chứng từ chuyển không
                if (nvlNhapXuatKhoShowcrr.flag > 0)//Chứng từ chuyển hoặc nhập đóng vỉ, sẽ nhập hoặc xuất ở kho này, và trừ vào kho kia
                {
                    if (LoaiNhapXuat == "ChuyenKho")
                    {
                        if (nvlNhapXuatKhoShowcrr.STTCT > 0)
                        {
                            toastService.Notify(new ToastMessage(ToastType.Danger, "Đây là chứng từ nhận, không được phép nhập vào chứng từ này"));
                            return;
                        }
                        sql = "NVLDB.dbo.NvlNhapXuatItem_InsertChuyen_Ver2";
                        setparameterChuyen(nvlNhapXuatItemShowcrr);
                    }
                    if (LoaiNhapXuat == "NhapGiaCong")
                    {
                        if (nvlNhapXuatKhoShowcrr.STTCT < 0)
                        {
                            toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng nhập ở chứng từ nhập"));
                            return;
                        }

                        sql = "NVLDB.dbo.NvlNhapXuatItem_InsertGiaCongDongVi";
                        setParameter(nvlNhapXuatItemShowcrr);
                    }

                }
                else
                {
                    sql = "NVLDB.dbo.NvlNhapXuatItem_Insert_Ver3";

                    setParameter(nvlNhapXuatItemShowcrr);
                    //Xử lý thêm cho cấp vượt
                    if (chkVuotDeNghi)
                    {
                        lstpara.Add(new ParameterDefine("@CapVuot", 1));
                    }
                    else
                    {
                        lstpara.Add(new ParameterDefine("@CapVuot", 0));
                    }
                }
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {

                        toastService.Notify(new ToastMessage(ToastType.Success, "LƯU THÀNH CÔNG"));
                        checksave = true;
                        MaHangImg = nvlNhapXuatItemShowcrr.MaHang;
                        vitrikeep = nvlNhapXuatItemShowcrr.ViTri;
                        if (nvlNhapXuatKhoShowcrr.LyDo == "NHẬP KHO MUA HÀNG")
                        {
                            if (ModelData.lsthanghoa != null)
                            {
                                ModelData.lsthanghoa.FirstOrDefault(p => p.MaHang == nvlNhapXuatItemShowcrr.MaHang).DonGia = nvlNhapXuatItemShowcrr.DonGia; ;
                            }
                        }
                        lstdonhangkeep.Clear();
                        if (donhangselected != null)
                        {
                            foreach (var item in donhangselected)
                            {
                                lstdonhangkeep.Add(item);
                            }
                        }
                        reset();

                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));
                        if (query[0].ketqua.ToLower().Contains("nhập lý do"))
                        {
                            showLyDoChapNhan(query[0].ketqua);
                        }
                        //reset();
                    }
                    checksave = true;
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                checksave = true;
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
                nvlNhapXuatItemShowcrr.DauTuan = txtDauTuan.GetText;
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlNhapXuatItem_Update_Ver2";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlNhapXuatItemShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@SerialCT", nvlNhapXuatItemShowcrr.SerialCT));
                lstpara.Add(new ParameterDefine("@SerialLink", nvlNhapXuatItemShowcrr.SerialLink));
                lstpara.Add(new ParameterDefine("@SerialKHDH", nvlNhapXuatItemShowcrr.SerialKHDH));
                lstpara.Add(new ParameterDefine("@TableName", nvlNhapXuatItemShowcrr.TableName));
                lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItemShowcrr.MaHang));
                lstpara.Add(new ParameterDefine("@DVT", nvlNhapXuatItemShowcrr.DVT));
                lstpara.Add(new ParameterDefine("@SLNhap", nvlNhapXuatItemShowcrr.SLNhap));
                lstpara.Add(new ParameterDefine("@SLXuat", nvlNhapXuatItemShowcrr.SLXuat));
                lstpara.Add(new ParameterDefine("@DonGia", nvlNhapXuatItemShowcrr.DonGia));
                lstpara.Add(new ParameterDefine("@SLNhapTT", nvlNhapXuatItemShowcrr.SLNhapTT));
                lstpara.Add(new ParameterDefine("@SLXuatTT", nvlNhapXuatItemShowcrr.SLXuatTT));
                lstpara.Add(new ParameterDefine("@DVTTT", nvlNhapXuatItemShowcrr.DVTTT));
                lstpara.Add(new ParameterDefine("@TyLeQuyDoi", nvlNhapXuatItemShowcrr.TyLeQuyDoi));
                lstpara.Add(new ParameterDefine("@KhachHang_XuatXu", nvlNhapXuatItemShowcrr.KhachHang_XuatXu));
                lstpara.Add(new ParameterDefine("@NgayHetHan", nvlNhapXuatItemShowcrr.NgayHetHan));
                lstpara.Add(new ParameterDefine("@NgaySanXuat", nvlNhapXuatItemShowcrr.NgaySanXuat));
                lstpara.Add(new ParameterDefine("@MaKien", nvlNhapXuatItemShowcrr.MaKien));
                lstpara.Add(new ParameterDefine("@SoLo", nvlNhapXuatItemShowcrr.SoLo));
                lstpara.Add(new ParameterDefine("@SoXe", nvlNhapXuatItemShowcrr.SoXe));
                lstpara.Add(new ParameterDefine("@GhiChu", nvlNhapXuatItemShowcrr.GhiChu));
                lstpara.Add(new ParameterDefine("@Barcode", nvlNhapXuatItemShowcrr.Barcode));
                lstpara.Add(new ParameterDefine("@MaSP", nvlNhapXuatItemShowcrr.MaSP));
                lstpara.Add(new ParameterDefine("@ArticleNumber", nvlNhapXuatItemShowcrr.ArticleNumber));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@ViTri", nvlNhapXuatItemShowcrr.ViTri));
                lstpara.Add(new ParameterDefine("@TypeNhapXuat", ""));//Phía dưới procedure đã xử lý rồi
                lstpara.Add(new ParameterDefine("@DauTuan", nvlNhapXuatItemShowcrr.DauTuan));

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "SỬA thành công"));
                        if (AfterEdit.HasDelegate)
                        {
                            await AfterEdit.InvokeAsync(nvlNhapXuatItemShowcrr);
                        }
                        //reset();

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
        string MaHangImg = "";
        private void reset()
        {
            
            nvlNhapXuatItemShowcrr.Serial = 0;
            nvlNhapXuatItemShowcrr.SerialLink = null;
            donhangselected = null;
            lstdonhang.Clear();
            tongdonhangselect = 0;
            nvlNhapXuatItemShowcrr.SoLuong = null;
            nvlNhapXuatItemShowcrr.SLNhap = 0;
            nvlNhapXuatItemShowcrr.SLXuat = 0;
            nvlNhapXuatItemShowcrr.MaHang = null;
            nvlNhapXuatItemShowcrr.DauTuan = "";
            giathamkhao = 0;
            nvlNhapXuatItemShowcrr.GhiChu = "";
            nvlNhapXuatItemShowcrr.NgaySanXuat = null;
            nvlNhapXuatItemShowcrr.NgayHetHan = null;
            nvlNhapXuatItemShowcrr.MaKien = "";
            chkVuotDeNghi = false;
            nvlNhapXuatItemShowcrr.DonGia = null;
            dautuanselected = null;
            visibleTheoDoiNo = false;
            view_BarcodeScan.setFocus();
            CheckDuplicate = false;//Tránh trường hợp người dùng sử dụng chức năng thêm vào tem có sẵn, sau đó sử dụng cho những tem quét sau mà ko đóng form
            lydochapnhan = "";//reset lại phần lý do chấp nhận đối vs những tem vi phạm
            checksave = true;
            
            StateHasChanged();


        }

        private async Task saveCanTruNoAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không có quyền sử dụng chức năng này"));
                return;
            }
            if (!checklogic())
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Dữ liệu nhập không đúng. Vui lòng kiểm tra những dòng tô màu đỏ"));
                return;
            }
            try
            {
                checksave = false;//Tránh để call 2 lần do API chậm
                chkVuotDeNghi = false;

                string sql = "";
                CallAPI callAPI = new CallAPI();

                //Xử lý kiểm tra xem phải chứng từ chuyển không

                sql = "NVLDB.dbo.NvlNhapXuatItem_Insert_CanTruNo";

                setParameter(nvlNhapXuatItemShowcrr);
                //Xử lý thêm cho cấp vượt

                lstpara.Add(new ParameterDefine("@CapVuot", 0));


                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {

                        toastService.Notify(new ToastMessage(ToastType.Success, "CẤN TRỪ NỢ THÀNH CÔNG"));
                        MaHangImg = nvlNhapXuatItemShowcrr.MaHang;
                        vitrikeep = nvlNhapXuatItemShowcrr.ViTri;
                        reset();
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));
                        //if (query[0].ketqua.ToLower().Contains("nhập lý do"))
                        //{
                        //    showLyDoChapNhan(query[0].ketqua);
                        //}
                        //reset();
                    }
                    checksave = true;
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                checksave = true;
                Console.Error.WriteLine("Lỗi:" + ex.Message);
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi " + ex.Message));
                //msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
        }
        private async void SelectedItemChanged(NvlHangHoaDropDown nvlHangHoaDropDown)
        {
            if (nvlHangHoaDropDown != null)
            {
                nvlNhapXuatItemShowcrr.DVT = nvlHangHoaDropDown.DVT;
                MaHangImg = nvlHangHoaDropDown.MaHang;
                giathamkhao = nvlHangHoaDropDown.DonGia;

                //if (nvlNhapXuatKhoShowcrr.flag > 0)
                //{
                //    await loadsanphamdinhmucAsync(nvlHangHoaDropDown.MaHang);
                //}
                //nvlNhapXuatItemShowcrr.DonGia = nvlHangHoaDropDown.DonGia;//Xài tạm, khi nào có đơn hàng rồi thì bỏ
            }
        }
        //Xử lý load sản phẩm theo định mức
        public class SanphamSelect
        {
            public string MaSP { get; set; }
            public string TenSP { get; set; }
        }

        private async Task loadsanphamdinhmucAsync(string MaHang)
        {
            string sql = string.Format(@"Use NVLDB declare @MaHang nvarchar(100)=N'{0}'
                                    SELECT [MaSP] FROM [NvlChiTietKhuVuc]
                                      where MaHang=@MaHang
                                      group by MaSP", MaHang);
            try
            {

                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<SanphamSelect>>(json);
                    if (query.Count > 0)
                    {
                        foreach (var it in query)
                        {
                            foreach (var item in lstsanpham)
                            {
                                if (it.MaSP == item.MaSP)
                                {
                                    SanPhamDropDown sanPhamDropDown = new SanPhamDropDown();
                                    sanPhamDropDown.MaSP = item.MaSP;
                                    sanPhamDropDown.TenSP = item.TenSP;
                                    lstsanphamfilter.Add(sanPhamDropDown);
                                    break;
                                }
                            }
                        }
                        if (lstsanphamfilter.Count == 1)
                        {
                            nvlNhapXuatItemShowcrr.MaSP = lstsanphamfilter[0].MaSP;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi: " + ex.Message, IconMsg.iconerror));
            }
        }
        private bool CheckChungTuNhap(NvlNhapXuatKhoShow nvlNhapXuatKhoShow)
        {
            if (nvlNhapXuatKhoShow.STTCT >= 0)
            {
                return true;
            }
            return false;
        }
        private void dongiaclick()
        {
            //editprice = !editprice;
            nvlNhapXuatItemShowcrr.DonGia = giathamkhao;
        }
        private async void showLyDoChapNhan(string title)
        {
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NhapLyDo>(0);
                builder.AddAttribute(1, "GetLyDo", EventCallback.Factory.Create<string>(this, savegain));
                builder.CloseComponent();
            };
            await dxPopup.showAsync(title.ToUpper());
            await dxPopup.ShowAsync();
        }

        private async void savegain(string lydo)
        {
            await dxPopup.CloseAsync();
            lydochapnhan = lydo;
            await saveAsync();

        }
        private async Task showchitietnoAsync()
        {
            //toastService.Notify(new ToastMessage(ToastType.Danger, "Bảng chi tiết này tạm thời chưa được hỗ trợ. "));
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NvlChiTietNo>(0);
                builder.AddAttribute(1, "MaHang", MaHangImg);
                builder.CloseComponent();
            };
            await dxPopup.showAsync(string.Format("Chi tiết nợ của mã hàng {0}", MaHangImg)); ;
            await dxPopup.ShowAsync();
        }
        private async Task ShowimgAsync()
        {
            if (string.IsNullOrEmpty(MaHangImg))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Nhập mã hàng trước"));
                return;
            }
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_ImgNhanDien>(0);
                builder.AddAttribute(1, "MaHang", MaHangImg);
                builder.CloseComponent();
            };
            await dxPopup.showAsync(string.Format("Thêm ảnh cho mã hàng {0}", MaHangImg)); ;
            await dxPopup.ShowAsync();
        }
        private async void setSerialClick(string serialLink)
        {
            if(int.TryParse(serialLink, out int serialLinkOut))
            {
                nvlNhapXuatItemShowcrr.SerialLink = serialLinkOut;
               await checkSerialAsync(serialLink);
               await dxFlyoutchucnang.CloseAsync();
            }
           
        }
        private async void Onkeydown(KeyboardEventArgs e)
        {
           //Console.WriteLine($"SerialLink: {nvlNhapXuatItemShowcrr.SerialLink}"); // Output: nvlNhapXuatItemShowcrr.SerialLink
            if (e.Key == "F11")
            {
                //Ghi chuyển khỏi các text đang sửa, để focus về text box Mä kiện, cho các text đang sửa commit
                //Console.WriteLine("vừa nhấn F11");
                if (!EnableEdit)
                    await saveAsync();
            }
        }

        private async Task loadtonkhohAsync()
        {
            lstdataitem.Clear();


            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = " ";



            string sqlSearch = "";


            //string[] arrshow = new string[] { "MaHang", "TenHang", "SerialCT", "SLNhap", "SLXuat", "DVT", "LyDo", "TenSP", "MaKien", "DauTuan", "SoLo", "TenGN", "SoXe", "GhiChu", "DonGia", "NgayHetHan", "SerialLink", "KhachHang_XuatXu", "UserInsert", "SerialKHDH", "SerialDN", "NguoiDeNghi", "TenLienKet", "NhaMay", "NgayInsert" };


            sqlSearch = string.Format(@"use NVLDB

                           declare @MaHang nvarchar(100)=N'{0}'
                          declare @MaKho nvarchar(100)
					
					select @MaKho=MaKho from NvlNhapXuat where Serial={1}

                      select qry.MaHang,SLTon,SerialLink, qry.Serial,hh.TenHang,hh.DVT
                      from
                      (select MaHang,sum(SLNhap-SLXuat) as SLTon,SerialLink,min(case when SLNhap>0 then Serial end) as Serial
                      from NvlNhapXuatItem
                      where MaHang =@MaHang
                      and SerialCT in (select Serial from NvlNhapXuat where MaKho=@MaKho)
                      group by MaHang,SerialLink) as qry 
                      inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                      where SLTon<>0
                      order by SerialLink


                     ", MaHangImg,nvlNhapXuatKhoShowcrr.Serial);
            CallAPI callAPI = new CallAPI();
            try
            {



                string json = await callAPI.ExcuteQueryEncryptAsync(sqlSearch, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemTemTK>>(json);

                    if (query.Any())
                    {

                        //var queryit = query.Where(p => p.MaHang == nvlKeHoachMuaHangItemShow.MaHang).ToList();
                        lstdataitem.AddRange(query);
                        dxGridTK.Reload();
                        //lstdata = keHoachMuaHangcrr.lsttemtonkho;
                    }



                    // await GotoMainForm.InvokeAsync();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            StateHasChanged();
        }


    }
}
