using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Data;
using System.Globalization;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Urc_NhapXuatItemAdd;
using static NFCWebBlazor.App_NguyenVatLieu.App_ThanhToan.Page_ThanhToanMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_ThanhToan
{
    public partial class Page_TaoUyNhiemChi
    {
        [Inject] ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        public class UyNhiemChi
        {
            public int Serial { get; set; }
            public int SerialTT { get; set; }
            public Nullable<int> STT { get; set; }
            public string MaUNC { get; set; }
            public string TenTKNo { get; set; }
            public string SoTKNo { get; set; }
            public string SoTKCo { get; set; }
            public string SoHD { get; set; }
            public string TenTKCo { get; set; }
            public string DiaChiTKNo { get; set; }
            public string MaNCC { get; set; }
            public string DiaChiTKCo { get; set; }
            public string NgangHangTKNo { get; set; }
            public string NgangHangTKCo { get; set; }
            public double? ThanhTien { get; set; }
            public string DVT { get; set; }
            public string NoiDung { get; set; }
            public Nullable<System.DateTime> Ngay { get; set; }
            public string UserInsert { get; set; }
            public Nullable<System.DateTime> NgayInsert { get; set; }
            public string NguoiKiemTra { get; set; }
            public Nullable<System.DateTime> NgayKiemTra { get; set; }
            public string NguoiThanhToan { get; set; }

            public Nullable<System.DateTime> NgayThanhToan { get; set; }
            public string NoiDungThanhToan { get; set; }
            public UyNhiemChi CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<UyNhiemChi>(json);
            }
        }
        List<ThongTinTaiKhoan> lstTaiKhoan = new List<ThongTinTaiKhoan>();
        List<ThongTinTaiKhoan> lstTaiKhoanNo = new List<ThongTinTaiKhoan>();
        List<ThongTinTaiKhoan> lstTaiKhoanCo = new List<ThongTinTaiKhoan>();
        List<DataDropDownList> lstDVT = new List<DataDropDownList>();
        bool CheckQuyen = false;
        public async void LoadData()
        {

            lstTaiKhoan = await ModelData.GetdtThongTinTaiKhoan();
            lstTaiKhoanNo = lstTaiKhoan.Where(p => p.TenTaiKhoan.Contains("SCANSIA")).ToList();
            lstTaiKhoanCo = lstTaiKhoan;
            lstDVT = await ModelData.GetDataDropDownListsAsync("DonViThanhToan");
            if (uyNhiemChi.Serial == 0)
            {
                var qyery = lstTaiKhoan.Where(p => p.MaNCC == uyNhiemChi.MaNCC).FirstOrDefault();
                if (qyery != null)
                {
                    uyNhiemChi.SoTKCo = qyery.SoTK;
                }
            }
            if(uyNhiemChi.Serial>0)
            {
                EnableEdit = true;
            }
            CheckQuyen = await phanQuyenAccess.CreateThanhToan(Model.ModelAdmin.users);
            StateHasChanged();
            //txtTaiKhoanNo.setdt(Model.ModelData.GetdtThongTinTaiKhoan().Select("TenTaiKhoan like '%SCANSIA%'").CopyToDataTable(), "SoTK", "SoTK");
            //txtTenTaiKhoanCo.setdt(Model.ModelData.GetdtThongTinTaiKhoan().AsEnumerable().CopyToDataTable(), "SoTK", "TenTaiKhoan");
            //txtTaiKhoanCo.setdt(Model.ModelData.GetdtThongTinTaiKhoan().AsEnumerable().CopyToDataTable(), "SoTK", "SoTK");
            //txtDVT.setdt(Model.ModelData.Getdtdropdownlist().Select("TypeName='DonViThanhToan'").CopyToDataTable(), "Name", "Name");
            //btthemnDVT.InitButton(txtDVT, "Thêm đơn vị tính", "DonViThanhToan");
            //// txtThanhTien.Text = "100000000000";
            //txtTaiKhoanNo.setValueItem = "007 0000 118 023";
        }
        protected override Task OnInitializedAsync()
        {
            try
            {
                LoadData();
                editContext = new EditContext(uyNhiemChi);
                editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load form ke hoach master " + ex.Message);
            }
            finally
            {
            }


            return base.OnInitializedAsync();
        }
        ValidationMessageStore validationMessages { get; set; }
        bool checksave = true;//Gán biến để tránh trường hợp mạng lag API gọi lại 2 lần
        private async void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {
            if (validationMessages == null)
                validationMessages = new ValidationMessageStore(editContext);
            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();
            if (string.IsNullOrEmpty(uyNhiemChi.DiaChiTKCo))
            {
                validationMessages.Add(() => uyNhiemChi.DiaChiTKCo, "Vui lòng nhập địa chỉ.");
                return;
            }

            if (string.IsNullOrEmpty(uyNhiemChi.DiaChiTKNo))
            {
                validationMessages.Add(() => uyNhiemChi.DiaChiTKNo, "Vui lòng nhập địa chỉ.");
                return;
            }
            if (uyNhiemChi.ThanhTien == null || uyNhiemChi.ThanhTien <= 0)
            {
                validationMessages.Add(() => uyNhiemChi.ThanhTien, "Vui lòng số tiền.");
                return;
            }
            if (string.IsNullOrEmpty(uyNhiemChi.DVT))
            {
                validationMessages.Add(() => uyNhiemChi.DVT, "Vui lòng nhập ĐVT.");
                return;
            }
            if (string.IsNullOrEmpty(uyNhiemChi.TenTKCo))
            {
                validationMessages.Add(() => uyNhiemChi.TenTKCo, "Vui lòng nhập Tên TK.");
                return;
            }
            if (string.IsNullOrEmpty(uyNhiemChi.TenTKNo))
            {
                validationMessages.Add(() => uyNhiemChi.TenTKNo, "Vui lòng Tên TK.");
                return;
            }
            if (string.IsNullOrEmpty(uyNhiemChi.NoiDung))
            {
                validationMessages.Add(() => uyNhiemChi.NoiDung, "Vui lòng nhập Nội dung.");
                return;
            }
            if (string.IsNullOrEmpty(uyNhiemChi.SoTKNo))
            {
                validationMessages.Add(() => uyNhiemChi.SoTKNo, "Vui lòng nhập Số TK.");
                return;
            }
            if (string.IsNullOrEmpty(uyNhiemChi.SoTKCo))
            {
                validationMessages.Add(() => uyNhiemChi.SoTKCo, "Vui lòng nhập số TK.");
                return;
            }
            // Lưu kết quả validation

            editContext.NotifyValidationStateChanged();
        }
        private void SelectedTaiKhoanNo(ThongTinTaiKhoan thongTinTaiKhoan)
        {
            if (thongTinTaiKhoan == null)
            {
                uyNhiemChi.DiaChiTKNo = "";
                uyNhiemChi.SoTKNo = "";
                uyNhiemChi.TenTKNo = "";
                uyNhiemChi.NgangHangTKNo = "";
                return;
            }
            uyNhiemChi.DiaChiTKNo = thongTinTaiKhoan.DiaChi;
            uyNhiemChi.SoTKNo = thongTinTaiKhoan.SoTK;
            uyNhiemChi.TenTKNo = thongTinTaiKhoan.TenTaiKhoan;
            uyNhiemChi.NgangHangTKNo = thongTinTaiKhoan.NganHang;
        }
        private void SelectedTaiKhoanCo(ThongTinTaiKhoan thongTinTaiKhoan)
        {
            if (thongTinTaiKhoan == null)
            {
                uyNhiemChi.DiaChiTKCo = "";
                uyNhiemChi.SoTKCo = "";
                uyNhiemChi.TenTKCo = "";
                uyNhiemChi.NgangHangTKCo = "";
                return;
            }
            uyNhiemChi.DiaChiTKCo = thongTinTaiKhoan.DiaChi;
            uyNhiemChi.SoTKCo = thongTinTaiKhoan.SoTK;
            uyNhiemChi.TenTKCo = thongTinTaiKhoan.TenTaiKhoan;
            uyNhiemChi.NgangHangTKCo = thongTinTaiKhoan.NganHang;
        }
        private void setSoLuong(double? d)
        {
            if(d!=null)
                uyNhiemChi.ThanhTien =d;
        }
        private bool checklogic(UyNhiemChi uyNhiemChi)
        {
            if (!checksave)
                return false;
            return editContext.Validate();
        }
        public class Ketqua
        {
            public string MaUNC { get; set; }

            public int Serial { get; set; }
            public string? ketqua { get; set; }
            public string? ketquaexception { get; set; } = "";

        }
        private async Task saveAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không có quyền sử dụng chức năng này"));
                return;
            }
            if (!checklogic(uyNhiemChi))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Dữ liệu nhập không đúng. Vui lòng kiểm tra những dòng tô màu đỏ"));
                return;
            }
            try
            {
                checksave = false;//Tránh để call 2 lần do API chậm
                string xml = "<rows>";

                xml += string.Format("<row><SerialHD>{0}</SerialHD><SoHD>{1}</SoHD><ThanhToan>{2}</ThanhToan></row>", uyNhiemChi.SerialTT, uyNhiemChi.SoHD, uyNhiemChi.ThanhTien.Value.ToString(CultureInfo.GetCultureInfo("en-US")));


                xml += "</rows>";
                string sql = "";
                
                    sql = "SPSupplier.dbo.UyNhiemChi_Item_Insert_NVL";
                    CallAPI callAPI = new CallAPI();
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    lstpara.Add(new ParameterDefine("@xmlinput", xml));
                    lstpara.Add(new ParameterDefine("@SoTKNo", uyNhiemChi.SoTKNo));
                    lstpara.Add(new ParameterDefine("@SoTKCo", uyNhiemChi.SoTKCo));
                    lstpara.Add(new ParameterDefine("@ThanhTien", uyNhiemChi.ThanhTien));
                    lstpara.Add(new ParameterDefine("@DVT", uyNhiemChi.DVT));
                    lstpara.Add(new ParameterDefine("@NoiDung", uyNhiemChi.NoiDung));
                    lstpara.Add(new ParameterDefine("@Ngay", uyNhiemChi.Ngay.Value.ToString("MM/dd/yyyy")));
                    lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                    lstpara.Add(new ParameterDefine("@TableName", "NvlThanhToan"));
                    lstpara.Add(new ParameterDefine("@PhanLoaiUNC", "Nguyên phụ liệu"));
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            toastService.Notify(new ToastMessage(ToastType.Success, "LƯU THÀNH CÔNG"));
                            checksave = true;
                            if (GotoMainForm.HasDelegate)
                            {
                                uyNhiemChi.MaUNC = query[0].MaUNC;
                                GotoMainForm.InvokeAsync(uyNhiemChi);
                            }// reset();

                        }
                        else
                        {
                            toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));

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
            if (!checklogic(uyNhiemChi))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Dữ liệu nhập không đúng. Vui lòng kiểm tra những dòng tô màu đỏ"));
                return;
            }
            try
            {
                checksave = false;//Tránh để call 2 lần do API chậm
                string xml = "<rows>";

                xml += string.Format("<row><SerialHD>{0}</SerialHD><SoHD>{1}</SoHD><ThanhToan>{2}</ThanhToan></row>", uyNhiemChi.SerialTT, uyNhiemChi.SoHD, uyNhiemChi.ThanhTien);


                xml += "</rows>";
                string sql = "";

                sql = "SPSupplier.dbo.UyNhiemChi_Item_Update_NVL";
                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@xmlinput", xml));
                lstpara.Add(new ParameterDefine("@MaUNC", uyNhiemChi.MaUNC));
                lstpara.Add(new ParameterDefine("@SoTKNo", uyNhiemChi.SoTKNo));
                lstpara.Add(new ParameterDefine("@SoTKCo", uyNhiemChi.SoTKCo));
                lstpara.Add(new ParameterDefine("@ThanhTien", uyNhiemChi.ThanhTien));
                lstpara.Add(new ParameterDefine("@DVT", uyNhiemChi.DVT));
                lstpara.Add(new ParameterDefine("@NoiDung", uyNhiemChi.NoiDung));
                lstpara.Add(new ParameterDefine("@Ngay", uyNhiemChi.Ngay.Value.ToString("MM/dd/yyyy")));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                //lstpara.Add(new ParameterDefine("@TableName", "NvlThanhToan"));
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "SỬA THÀNH CÔNG"));
                        checksave = true;
                        if (GotoMainForm.HasDelegate)
                        {
                            
                            GotoMainForm.InvokeAsync(uyNhiemChi);
                        }// reset();

                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));

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
        private async Task deleteAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không có quyền sử dụng chức năng này"));
                return;
            }
            if (!checklogic(uyNhiemChi))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Dữ liệu nhập không đúng. Vui lòng kiểm tra những dòng tô màu đỏ"));
                return;
            }
            try
            {
                checksave = false;//Tránh để call 2 lần do API chậm
              

               string sql = "SPSupplier.dbo.UyNhiemChi_Item_delete";
                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
               
                lstpara.Add(new ParameterDefine("@MaUNC", uyNhiemChi.MaUNC));
                lstpara.Add(new ParameterDefine("@LyDoDelete", "Xóa"));
               
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                //lstpara.Add(new ParameterDefine("@TableName", "NvlThanhToan"));
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "XÓA THÀNH CÔNG"));
                        checksave = true;
                        if (GotoMainForm.HasDelegate)
                        {
                            uyNhiemChi.MaUNC = "";
                            GotoMainForm.InvokeAsync(uyNhiemChi);
                        }// reset();

                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));

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
        private void addthue(double d)
        {
            uyNhiemChi.ThanhTien = uyNhiemChi.ThanhTien*(1 + d);
        }

    }
}
