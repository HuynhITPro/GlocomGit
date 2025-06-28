using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System.ComponentModel;
using System.Data;
using System.Text;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_InPhieu;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_InPhieu
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        [Inject] SignalRConnect signalRConnect { get; set; }
        public class NvlInPhieuShow : INotifyPropertyChanged
        {
            private bool _chk { get; set; }
            public string? MayIn { get; set; }
            public int Serial { get; set; }
            public int MaCT { get; set; }
            public string? MaHang { get; set; }
            public string? DVT { get; set; }
            public double SoLuong { get; set; }
            public string? KhachHang_XuatXu { get; set; }
            public Nullable<DateTime> NgayHetHan { get; set; }
            public Nullable<DateTime> NgaySanXuat { get; set; }
            public string? MaKien { get; set; }
            public string? SoLo { get; set; }
            public string? MaGN { get; set; }
            public string? SoXe { get; set; }
            public string? PhanLoai { get;set; }
            public string? GhiChu { get; set; }
            public string? Barcode { get; set; }
            public string? DauTuan { get; set; }
            public string? MaSP { get; set; }
            public string? ArticleNumber { get; set; }
            public string? KhuVuc { get; set; }
            public string? TenKhuVuc { get; set; }
            public string? ChatLuong { get; set; }
            public Nullable<int> CheckPrint { get; set; }

            public string? UserInsert { get; set; }
            public DateTime NgayInsert { get; set; }
            public string? TenSP { get; set; }
            public string? MaNhom { get; set; }
            public string? TenHang { get; set; }
            public string? ketqua { get; set; }
            public string? ketquaexception { get; set; }
            public string? PathImg { get; set; }
            public int BanIn { get; set; }
            public string NoiDungDetail { get; set; }
            public bool chk
            {
                get { return _chk; }
                set
                {
                    _chk = value;
                    NotifyPropertyChanged("chk");
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }


        }
        bool Ismobile;
        protected override async Task OnInitializedAsync()
        {

            try
            {

                var dimension = await browserService.GetDimensions();
                // var heighrow = await browserService.GetHeighWithID("divcontainer");
                int height = dimension.Height - 70;
                int width = dimension.Width;
                if (width < 768)
                {
                    Ismobile = true;
                    idgrid = "customGridnotheader";
                }
                else
                    Ismobile = false;
                await loadAsync();
                nvlInPhieuShowAdd = new NvlInTemShow();
                nvlInPhieuShowAdd.Serial = 0;
                nvlInPhieuShowAdd.BanIn = 1;
                CheckQuyen = await phanQuyenAccess.CreateInPhieu(Model.ModelAdmin.users);
                //if (heighrow!=null)
                //{
                //    height = dimension.Height - heighrow;
                //}
                lstimg = new List<imgserial>();
                heightgrid = string.Format("{0}px", height);
                lstimg.Add(new imgserial(IconImg.equal, "="));
                lstimg.Add(new imgserial(IconImg.greater, ">="));
                lstimg.Add(new imgserial(IconImg.less, "<="));
                indexconditionserial = 0;
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi browserService :" + ex.Message));
            }

        }

        bool CheckQuyen = false;
        private async Task loadAsync()
        {
            try
            {
                lstuser = await Model.ModelData.Getlstusers();
                _ = Model.ModelData.Getlstnoigiaonhan();
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
            }
            //baocaoselected = lstloaibaocao.FirstOrDefault(p => p.Name.Equals("Tồn kho theo sản phẩm"));


        }
        bool firstload = false;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstload)//Load lần đầu để gán biến
            {
                if (lstuser != null)
                {
                    userselect = lstuser.FirstOrDefault(p => p.UsersName.Equals(ModelAdmin.users.UsersName));
                    firstload = true;
                    StateHasChanged();
                }

            }
            return base.OnAfterRenderAsync(firstRender);
        }


        private async Task searchAsync()
        {

            string dieukien = "";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string condition = "";
            if (!string.IsNullOrEmpty(Serial))
            {
                condition = lstimg[indexconditionserial].stringcondition;
                if (dieukien != "")
                {

                    dieukien += string.Format(" and inphieu.Serial {0} @Serial", condition);
                }
                else
                {
                    dieukien = string.Format(" where inphieu.Serial {0} @Serial", condition);
                }
                lstpara.Add(new ParameterDefine("@Serial", Serial));
            }
            if (!string.IsNullOrEmpty(Serial) && condition == "=")
            {
                //Nếu tìm đích danh số Serial
            }
            else
            {
                if (dtpbegin != null)
                {
                    if (dieukien != "")
                    {
                        dieukien += " and inphieu.NgayInsert>=@NgayInsertBegin";
                    }
                    else
                    {
                        dieukien = " Where inphieu.NgayInsert>=@NgayInsertBegin";
                    }
                    lstpara.Add(new ParameterDefine("@NgayInsertBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
                }
                if (dtpend != null)
                {
                    if (dieukien != "")
                    {
                        dieukien += " and inphieu.NgayInsert<=@NgayInsertEnd";
                    }
                    else
                    {
                        dieukien = " Where inphieu.NgayInsert<=@NgayInsertEnd";
                    }
                    lstpara.Add(new ParameterDefine("@NgayInsertEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
                }
                if (userselect != null)
                {
                    if (dieukien != "")
                    {
                        dieukien += " and inphieu.UserInsert=@UserInsert";
                    }
                    else
                    {
                        dieukien = " Where inphieu.UserInsert=@UserInsert";
                    }
                    lstpara.Add(new ParameterDefine("@UserInsert", userselect.UsersName));
                }
                if (!string.IsNullOrEmpty(TinhTrang))
                {
                    string print = "";
                    if (TinhTrang == "Chưa in")
                        print = "inphieu.CheckPrint=0";
                    if (TinhTrang == "Đã in")
                        print = "inphieu.CheckPrint>0";
                    if (print != "")
                    {
                        if (dieukien != "")
                        {
                            dieukien += " and " + print;
                        }
                        else
                        {
                            dieukien = " Where " + print;
                        }
                    }

                }
            }
            string sql = string.Format(@"Use NVLDB SELECT cast(0 as bit) as chk, inphieu.*, hh.TenHang, nh.PhanLoai as TenKhuVuc
                            FROM NvlInPhieu inphieu inner join NvlHangHoa hh on inphieu.MaHang=hh.MaHang 
                                   inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                                    {0} order by inphieu.Serial ", dieukien);
            try
            {
                if (lstInPhieu != null)
                    lstInPhieu.Clear();
                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {

                    var queryresult = JsonConvert.DeserializeObject<List<NvlInPhieuShow>>(json);
                    if (queryresult != null)
                    {

                        lstInPhieu = new System.Collections.ObjectModel.ObservableCollection<NvlInPhieuShow>(queryresult);

                    }

                }
                ////Xử lý load ảnh

                PanelVisible = false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi" + ex.Message));

                PanelVisible = false;
            }
            finally
            {
                PanelVisible = false;

                StateHasChanged();
                //ShowProgress.CloseAwait();
            }

        }
        private async Task PrintAgainAsync(bool printdirect)
        {
            if (lstInPhieu == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Chưa có dữ liệu"));
                return;
            }
            var querycheck = lstInPhieu.Where(p => p.chk.Equals(true)).ToList();
            if (!querycheck.Any())
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn ít nhất 1 tem để in lại"));

                return;
            }
            if (printdirect)
            {
                if (string.IsNullOrEmpty(view_PrintConnectshow.printername))
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Chưa kết nối máy in"));

                    return;
                }
                if (string.IsNullOrEmpty(view_PrintConnectshow.printbieumau))
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Chưa chọn biểu mẫu in"));

                    return;
                }
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<rows>");
            foreach (var it in querycheck)
            {
                stringBuilder.Append("<row>");
                stringBuilder.Append(string.Format("<Serial>{0}</Serial><LyDo>{1}</LyDo>", it.Serial, ""));
                stringBuilder.Append("</row>");
            }
            stringBuilder.Append("</rows>");
            CallAPI callAPI = new CallAPI();
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string sql = "NVLDB.dbo.NvlInphieu_Reprint";//Nhớ them 1 trường OK ở procedure này
            lstpara.Add(new ParameterDefine("@xmlinput", stringBuilder.ToString()));

            string typeprint = "";
            if (view_PrintConnectshow.printbieumau == "Tem 5x10")
                typeprint = "nvlnhapxuatintem";
            if (view_PrintConnectshow.printbieumau == "Giấy A5")
                typeprint = "nvlnhapxuatinphieu";
            if (view_PrintConnectshow.printbieumau == "Tem 2x5")
                typeprint = "nvlnhapxuatintem25";
            if (typeprint == "")
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "In giấy A5 hay in TEM"));
                return;
            }
            string json = "";
            if (printdirect)
                json = await callAPI.ProcedureEncryptMsgAsync(sql, lstpara, view_PrintConnectshow.printername, typeprint);
            else
                json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
            if (json != "")
            {

                if (json == "Vui lòng nhập topic và id")
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng nhập topic và id"));
                    return;
                }
                var query = JsonConvert.DeserializeObject<List<NvlInPhieuShow>>(json);
                if (query == null)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi không IN được")));
                }
                else
                {
                    if (query[0].ketqua == "OK")
                    {
                        if (printdirect)
                            ToastService.Notify(new ToastMessage(ToastType.Success, "GỬI LỆNH IN THÀNH CÔNG"));
                        else
                        {
                            if (view_PrintConnectshow.printbieumau == "Tem 5x10")
                            {
                                XtraRp_InPhieu xtra_KTGTonKho = new XtraRp_InPhieu();
                                string text = "";
                                foreach (var it in query)
                                {
                                    text = "";
                                    if (!string.IsNullOrEmpty(it.DauTuan))
                                    {

                                        text = string.Format("Dấu tuần: {0}", it.DauTuan);

                                    }
                                    if (it.NgaySanXuat != null)
                                    {
                                        if (text != "")
                                            text += string.Format(", Ngày SX: {0}", it.NgaySanXuat.Value.ToString("dd/MM/yy"));
                                        else
                                            text = string.Format("Ngày SX: {0}", it.NgaySanXuat.Value.ToString("dd/MM/yy"));
                                    }
                                    if (it.NgayHetHan != null)
                                    {
                                        if (text != "")
                                            text +=string.Format(", Ngày HH: {0}", it.NgayHetHan.Value.ToString("dd/MM/yy"));
                                        else
                                            text =string.Format("Ngày HH: {0}", it.NgayHetHan.Value.ToString("dd/MM/yy"));
                                    }
                                    it.NoiDungDetail = text;
                                    //it["NoiDungDetail"] = text;
                                }


                                xtra_KTGTonKho.DataSource = query;


                                //parameters.Add("report", xtra_KTGTonKho);
                                //modal.Show<ReportShow>("", parameters, options);
                                _ = ModelAdmin.mainLayout.showreportAsync(xtra_KTGTonKho);
                            }
                            if (view_PrintConnectshow.printbieumau == "Giấy A5")
                            {
                                if (query.Count > 0)
                                {
                                    if (query[0].MaNhom == "NVLP_KEO")
                                    {
                                        Xtra_PhieuDauMau xtra_KTGTonKho = new Xtra_PhieuDauMau();

                                        xtra_KTGTonKho.DataSource = query;
                                        //xtra_KTGTonKho.setTieuDe("PHIẾU NHẬN DẠNG HÀNG HÓA");

                                        //parameters.Add("report", xtra_KTGTonKho);
                                        //modal.Show<ReportShow>("", parameters, options);
                                        _ = ModelAdmin.mainLayout.showreportAsync(xtra_KTGTonKho);
                                    }
                                    else
                                    {

                                        Xtra_InPhieuA5 xtra_KTGTonKho = new Xtra_InPhieuA5();

                                        xtra_KTGTonKho.DataSource = query;
                                        xtra_KTGTonKho.setTieuDe("PHIẾU NHẬN DẠNG HÀNG HÓA");

                                        //parameters.Add("report", xtra_KTGTonKho);
                                        //modal.Show<ReportShow>("", parameters, options);
                                        _ = ModelAdmin.mainLayout.showreportAsync(xtra_KTGTonKho);
                                    }
                                }
                            }
                            if (view_PrintConnectshow.printbieumau == "Tem 2x5")
                            {
                                var it = query.FirstOrDefault();
                                if (it != null)
                                {
                                    if (it.PhanLoai == "KHO THIẾT BỊ VĂN PHÒNG")
                                    {
                                        XtraRp_InPhieu2x5_ThietBiVP xtraRp_InPhieu2X5_ThietBiVP = new XtraRp_InPhieu2x5_ThietBiVP();
                                        xtraRp_InPhieu2X5_ThietBiVP.DataSource = query;
                                        //parameters.Add("report", xtra_KTGTonKho);
                                        //modal.Show<ReportShow>("", parameters, options);
                                        _ = ModelAdmin.mainLayout.showreportAsync(xtraRp_InPhieu2X5_ThietBiVP);
                                    }
                                    else
                                    {
                                        XtraRp_InPhieu2x5 xtra_KTGTonKho = new XtraRp_InPhieu2x5();
                                        xtra_KTGTonKho.DataSource = query;
                                        //parameters.Add("report", xtra_KTGTonKho);
                                        //modal.Show<ReportShow>("", parameters, options);
                                        _ = ModelAdmin.mainLayout.showreportAsync(xtra_KTGTonKho);
                                    }

                                }
                            }

                        }

                    }
                    else
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi không in được " + query[0].ketquaexception)));
                    }
                }
                //Grid.Data = lstDonDatHangSearchShow;
            }
        }


        private async void ShowInTemMasterAdd()
        {
            if (string.IsNullOrEmpty(ModelAdmin.PhanLoaiHang))
            {
                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_PhanLoaiNhomHang>(0);

                    builder.AddAttribute(1, "GotoMainForm", EventCallback.Factory.Create(this, hidePopupAsync));
                    //builder.OpenComponent(0, componentType);
                    builder.CloseComponent();
                };
                await dxPopup.showAsync("Chọn nhóm hàng / kho");
                await dxPopup.ShowAsync();
            }
            else
            {
                await dxPopupInTem.showAsync("IN TEM MỚI");
                await dxPopupInTem.ShowAsync();
            }



        }
        private async Task hidePopupAsync()
        {
            await dxPopup.CloseAsync();
            await dxPopupInTem.showAsync("IN TEM MỚI");
            await dxPopupInTem.ShowAsync();
        }
    }
}
