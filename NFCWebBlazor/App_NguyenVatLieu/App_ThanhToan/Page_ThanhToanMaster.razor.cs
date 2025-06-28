using BlazorBootstrap;
using DevExpress.XtraReports.UI;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;
using static NFCWebBlazor.App_NguyenVatLieu.App_ThanhToan.Page_TaoUyNhiemChi;


namespace NFCWebBlazor.App_NguyenVatLieu.App_ThanhToan
{
    public partial class Page_ThanhToanMaster
    {
        public class NvlThanhToanShow
        {
            public int Serial { get; set; }

            public string NoiTT { get; set; }
            public string TenNTT { get; set; }

            public string MaGN { get; set; }
            public string TenGN { get; set; }

            public string MaCTTT { get; set; }
            public decimal? ThanhTien { get; set; }
            public string LyDo { get; set; }

            public DateTime? Ngay { get; set; }

            public string DienGiai { get; set; }

            public string UserInsert { get; set; }

            public string UserUp { get; set; }
            public string ChungTuGroup { get; set; }

            public int? Type { get; set; }
            private string _maUNC { get; set; }
            public string MaUNC
            {
                get { return _maUNC; }
                set
                {
                    _maUNC = value;
                    if (!string.IsNullOrEmpty(_maUNC))
                        ShowUNC = IconImg.pdf;
                    else
                        ShowUNC = IconImg.Calendar;
                }
            }

            public int? Flag { get; set; }

            public int? SignType { get; set; }

            public DateTime? NgayInsert { get; set; }
            public string _nguoixacnhan { get; set; }
            public string NguoiXacNhan
            {
                get
                {
                    return _nguoixacnhan;
                }
                set
                {
                    _nguoixacnhan = value;
                    if (_nguoixacnhan != null)
                        ShowIcon = IconImg.CheckMark;
                    else
                        ShowIcon = IconImg.NotCheck;
                }
            }
            public string ShowIcon { get; set; }
            public string ShowUNC { get; set; } = IconImg.NotCheck;

            public DateTime? NgayXacNhan { get; set; }
            public List<NvlThanhToanItemShow> lstitem { get; set; }
            public List<NvlNhapXuatKhoShow> lstnhapxuat { get; set; }
            public List<FileHoSoGroup>? lstfilehoso { get; set; }
            public void setlstfilehoso(List<FileHoSoGroup> lst)
            {
                lstfilehoso = lst;
            }
            public NvlThanhToanShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlThanhToanShow>(json);
            }


        }
        public class NvlThanhToanItemShow
        {
            public int Serial { get; set; }

            public int? SerialTT { get; set; }

            public int? SerialCT { get; set; }

            public decimal? SoTien { get; set; }

            public string UserInsert { get; set; }

            public DateTime? NgayInsert { get; set; }

            public NvlThanhToanItemShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlThanhToanItemShow>(json);
            }
        }
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        bool Ismobile { get; set; } = true;
        protected override async Task OnInitializedAsync()
        {

            try
            {
                await loadAsync();
                CheckQuyen = await phanQuyenAccess.CreateThanhToan(Model.ModelAdmin.users);

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
                {
                    Ismobile = false;
                    idgrid = "griddetailnhapkhoms";
                }
                lstuser = await Model.ModelData.Getlstusers();
                //if (heighrow!=null)
                //{
                //    height = dimension.Height - heighrow;
                //}
                var queryngn = await Model.ModelData.Getlstnoigiaonhan();

                lstnoigiaonhan = queryngn;
                heightgrid = string.Format("{0}px", height);
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi browserService :" + ex.Message));
            }

        }
        bool CheckQuyen = false;
        bool firstload = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _ = NFCWebBlazor.Model.ModelData.GetHangHoa();
            }
            if (Ismobile)
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
            }
            //await JS.InvokeVoidAsync("scrollToBottomLast");
            //base.OnAfterRender(firstRender);
        }
        int columnsbegin = 0;
        string? loaiNhapXuat { get; set; }
        List<NvlViTri> lstvitri { get; set; }
        private async Task loadAsync()
        {
            try
            {
                lstkhonvl = await Model.ModelData.GetKhoNvl();
                List<DataDropDownList> lst = await Model.ModelData.Getlstnvllydo();


            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
            }
            //baocaoselected = lstloaibaocao.FirstOrDefault(p => p.Name.Equals("Tồn kho theo sản phẩm"));


        }



        private async void search()
        {
            try
            {
                await searchasAsync("");

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));

            }

        }



        string ghichu = "";
        App_ClassDefine.ClassProcess prs = new ClassProcess();

        string chucnang = "";
        List<NvlThanhToanShow> lstNhapXuatKhoSearchShow = new List<NvlThanhToanShow>();
        private async Task searchasAsync(string Serial)
        {
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstNhapXuatKhoSearchShow.Clear();
            lstcolumn.Clear();
            string sql = "";

            CallAPI callAPI = new CallAPI();
            if (dtpbegin == null || dtpend == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Chọn ngày "));
                return;
            }
            TimeSpan difference = dtpend.Value - dtpbegin.Value;

            // Lấy số ngày
            int days = difference.Days;
            if (days == 0)
            {
                showallgroup = true;
            }
            else
                showallgroup = false;

            string dieukien = "";



            dieukien += " Where Ngay>=@DateBegin and Ngay<=@DateEnd";
            lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
            lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));

            if (!string.IsNullOrEmpty(tinhtrang))
            {
                if (tinhtrang == "Chưa xác nhận")
                    dieukien += " and NguoiXacNhan is null";
                if (tinhtrang == "Đã xác nhận")
                    dieukien += " and NguoiXacNhan is not null";
            }
            if (!string.IsNullOrEmpty(magiaonhan))
            {

                dieukien += " and MaGN=@MaGN";
                lstpara.Add(new ParameterDefine("@MaGN", magiaonhan));
                expanded = true;
            }
            else
                expanded = false;
            sql = string.Format(@"use NVLDB  
                                                        declare @tbl table([Serial] [int] primary key,[NoiTT] [nvarchar](100) NULL,
	                        [MaGN] [nvarchar](100) NULL,[MaCTTT] [nvarchar](100) NOT NULL,[LyDo] [nvarchar](100) NULL,[Ngay] [date] NULL,
	                        [DienGiai] [nvarchar](100) NULL,[UserInsert] [nvarchar](100) NULL,[type_] [int] NULL,
	                        [flag] [int] NULL,[sign_type] [int] NULL,[NgayInsert] [datetime] NULL,[NguoiXacNhan] [nvarchar](100) NULL,[NgayXacNhan] [datetime] NULL)
                        insert into @tbl(Serial,[NoiTT],[MaGN],[MaCTTT],[LyDo],[Ngay],[DienGiai],[UserInsert]
                                   ,[type_],[flag],[sign_type],[NgayInsert],[NguoiXacNhan],[NgayXacNhan])
                        select Serial,[NoiTT],[MaGN],[MaCTTT],[LyDo],[Ngay],[DienGiai],[UserInsert]
                                   ,[type_],[flag],[sign_type],[NgayInsert],[NguoiXacNhan],[NgayXacNhan] 
		                            from [dbo].[NvlThanhToan]  {0}
						
						declare @tblthanhtoanitem Table(SerialTT int,SerialCT int,ThanhTien decimal(18,6))
						insert into @tblthanhtoanitem(SerialTT,SerialCT,ThanhTien)
						select SerialTT,qry.SerialCT,sum(item.DonGia*item.SLNhap) as ThanhTien
						from
						(select SerialTT,SerialCT FROM [dbo].[NvlThanhToanItem] AS InnerTable

						where SerialTT in (select Serial from @tbl)) as qry inner join NvlNhapXuatItem item on item.SerialCT=qry.SerialCT
						group by SerialTT,qry.SerialCT

                        declare @tblitem as Table(SerialTT int Not null primary key,ChungTuGroup nvarchar(3000),ThanhTien decimal(18,6))
                        insert into @tblitem(SerialTT,ChungTuGroup,ThanhTien)
                        SELECT 
                            [SerialTT],
                            STUFF((
                                SELECT ', ' + cast(SerialCT as nvarchar(10))
                                FROM @tblthanhtoanitem AS InnerTable
                                WHERE InnerTable.SerialTT = OuterTable.SerialTT and  SerialTT in (select Serial from @tbl)
                                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS GopChungTu
								,sum(ThanhTien) as ThanhTien
                        FROM 
                           @tblthanhtoanitem AS OuterTable
						

                         where SerialTT in (select Serial from @tbl)
                        GROUP BY [SerialTT]

                        SELECT tt.[Serial],[NoiTT],tt.[MaGN],ngn.TenGN,[MaCTTT],[LyDo],[Ngay],[DienGiai],tt.[UserInsert]
                       ,[type_],[flag],[sign_type],tt.[NgayInsert],[NguoiXacNhan],[NgayXacNhan],item.ChungTuGroup,item.ThanhTien,unc.MaUNC
                        FROM @tbl tt 
                        left join @tblitem item on tt.Serial=item.SerialTT		  
                        inner join dbo.View_NoiGN ngn on tt.MaGN=ngn.MaGN 
                        left join SPSupplier.dbo.UyNhiemChi_Item unc on tt.Serial= unc.SerialHD and unc.TableName='NvlThanhToan'
                        
                        ", dieukien);
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<List<NvlThanhToanShow>>(json);
                    if (query.Count > 0)
                    {

                        lstNhapXuatKhoSearchShow.AddRange(query);
                        await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
                    }
                    //lstNhapXuatKhoSearchShow=lst;
                }


            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));

            }
            finally
            {
                PanelVisible = false;
                dxGrid.Reload();
                StateHasChanged();
            }

        }
        int visibleindexcrr = 0;
        public async void ShowFlyout(NvlThanhToanShow keHoachMuaHang_Show,int indexcrr)
        {
            try
            {
                await dxFlyoutchucnang.CloseAsync();
                nvlNhapXuatKhoShowcrr = keHoachMuaHang_Show;
                visibleindexcrr=indexcrr;
                idflychucnang = "#" + idelement(keHoachMuaHang_Show);
                if (nvlNhapXuatKhoShowcrr.NguoiXacNhan == null)
                    textshowxacnhan = "XÁC NHẬN";
                else
                    textshowxacnhan = "HỦY";
                if (!string.IsNullOrEmpty(nvlNhapXuatKhoShowcrr.MaUNC))
                {
                    textUNC = "Sửa/ Xóa ủy nhiệm chi";
                    ShowUNC = true;
                }
                else
                {
                    textUNC = "Thêm ủy nhiệm chi";
                    ShowUNC = false;
                }
                //IsOpenfly = true;
                await dxFlyoutchucnang.ShowAsync();
            }
            catch(Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                Console.WriteLine(ex.Message);
            }

        }
        private async void ShowMasterAdd()
        {
            IsOpenfly = false;
            NvlThanhToanShow nVLDonDatHangShow = new NvlThanhToanShow();
            nVLDonDatHangShow.Serial = 0;
            nVLDonDatHangShow.Ngay = DateTime.Now;
            nVLDonDatHangShow.LyDo = "THANH TOÁN TIỀN HÀNG";
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_ThanhToanMasterAdd>(0);
                builder.AddAttribute(1, "nvlThanhToanShowcrr", nVLDonDatHangShow);

                builder.AddAttribute(3, "AfterSave", EventCallback.Factory.Create<NvlThanhToanShow>(this, GotoMainFormAsync));
                builder.CloseComponent();
            };

          await  dxPopup.showAsync("THÊM CHỨNG TỪ");
          await  dxPopup.ShowAsync();
        }
        private async void ShowMasterEdit()
        {
            IsOpenfly = false;
           await dxFlyoutchucnang.CloseAsync();

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_ThanhToanMasterAdd>(0);
                builder.AddAttribute(1, "nvlThanhToanShowcrr", nvlNhapXuatKhoShowcrr.CopyClass());

                builder.AddAttribute(3, "AfterEdit", EventCallback.Factory.Create<NvlThanhToanShow>(this, AfterEditAsync));
                builder.CloseComponent();
            };

          await  dxPopup.showAsync("SỬA CHỨNG TỪ");
          await  dxPopup.ShowAsync();
        }

        public async Task GotoMainFormAsync(NvlThanhToanShow nVLDonDatHangShow)
        {
            await dxPopup.CloseAsync();
            try
            {

                await searchasAsync("");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
            // dxGrid.Reload();
        }
        public void setClassafterEdit(NvlThanhToanShow nvlThanhToanShowcrr_set, NvlThanhToanShow nvlThanhToanShowcrr_get)
        {
            nvlThanhToanShowcrr_set.Serial = nvlThanhToanShowcrr_get.Serial;
            nvlThanhToanShowcrr_set.NoiTT = nvlThanhToanShowcrr_get.NoiTT;
            nvlThanhToanShowcrr_set.MaGN = nvlThanhToanShowcrr_get.MaGN;
            nvlThanhToanShowcrr_set.MaCTTT = nvlThanhToanShowcrr_get.MaCTTT;
            nvlThanhToanShowcrr_set.LyDo = nvlThanhToanShowcrr_get.LyDo;
            nvlThanhToanShowcrr_set.Ngay = nvlThanhToanShowcrr_get.Ngay;
            nvlThanhToanShowcrr_set.DienGiai = nvlThanhToanShowcrr_get.DienGiai;
            nvlThanhToanShowcrr_set.UserInsert = nvlThanhToanShowcrr_get.UserInsert;
            nvlThanhToanShowcrr_set.UserUp = nvlThanhToanShowcrr_get.UserUp;
            nvlThanhToanShowcrr_set.TenGN = nvlThanhToanShowcrr_get.TenGN;
            nvlThanhToanShowcrr_set.NguoiXacNhan = nvlThanhToanShowcrr_get.NguoiXacNhan;
            nvlThanhToanShowcrr_set.NgayXacNhan = nvlThanhToanShowcrr_get.NgayXacNhan;

        }
        public async Task AfterEditAsync(NvlThanhToanShow nVLDonDatHangShow)
        {
            await dxPopup.CloseAsync();
            try
            {
                setClassafterEdit(nvlNhapXuatKhoShowcrr, nVLDonDatHangShow);
                await dxGrid.SaveChangesAsync();
                // dxGrid.Reload();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
            // dxGrid.Reload();
        }



        public async Task NhapXuatMasterDeleteAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            bool bl = await dialogMsg.Show("Xóa chứng từ", $"Bạn có chắc muốn chứng từ số {nvlNhapXuatKhoShowcrr.Serial.ToString()}");

            if (!phanQuyenAccess.CheckDelete(nvlNhapXuatKhoShowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không có quyền xóa dòng này do bạn không phải người tạo"));
                //msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;

            }
            if (bl)
            {

                try
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlThanhToan_Delete";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    lstpara.Add(new ParameterDefine("@Serial", nvlNhapXuatKhoShowcrr.Serial));
                    lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));
                    lstpara.Add(new ParameterDefine("@LyDoDelete", ModelAdmin.users.UsersName + " đã xóa"));
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<App_ModelClass.Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Success, "Xóa thành công"));
                            lstNhapXuatKhoSearchShow.Remove(nvlNhapXuatKhoShowcrr);
                            await dxGrid.SaveChangesAsync();

                            dxGrid.Reload();
                        }
                        else
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Warning, $"{query[0].ketqua}, {query[0].ketquaexception}"));
                            //msgBox.Show("Lỗi: " + query[0].ketqua, IconMsg.iconssuccess);

                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, $"Lỗi: " + ex.Message));
                }

            }
        }
        public async void XacNhan()
        {
            bool bl = await phanQuyenAccess.XacNhanThanhToan(Model.ModelAdmin.users);
            if (!bl)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền xác nhậ thanh toán"));
                return;
            }
            string sql = "NVLDB.dbo.NvlThanhToan_XacNhan";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Serial", nvlNhapXuatKhoShowcrr.Serial));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
            CallAPI callAPI = new CallAPI();
            string textxacnhan = "";
            if (nvlNhapXuatKhoShowcrr.NguoiXacNhan == null)
            {
                lstpara.Add(new ParameterDefine("@XacNhan", "xacnhan"));
                textxacnhan = "Xác nhận";
            }
            else
            {
                lstpara.Add(new ParameterDefine("@XacNhan", "hủy"));
                textxacnhan = "Hủy";
            }

            string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<App_ModelClass.Ketqua>>(json);
                if (query[0].ketqua == "OK")
                {
                    ToastService.Notify(new ToastMessage(ToastType.Success, $"{textxacnhan} thành công"));
                    if (textxacnhan == "Xác nhận")
                    {
                        nvlNhapXuatKhoShowcrr.NguoiXacNhan = ModelAdmin.users.UsersName;
                        nvlNhapXuatKhoShowcrr.NgayXacNhan = DateTime.Now;
                    }
                    else
                    {
                        nvlNhapXuatKhoShowcrr.NguoiXacNhan = null;
                        nvlNhapXuatKhoShowcrr.NgayXacNhan = null;

                    }


                    await dxGrid.SaveChangesAsync();

                    dxGrid.Reload();
                }
                else
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, $"{query[0].ketqua}, {query[0].ketquaexception}"));
                    //msgBox.Show("Lỗi: " + query[0].ketqua, IconMsg.iconssuccess);

                }
                //Grid.Data = lstDonDatHangSearchShow;
            }
        }
        private async void ShowNhapXuatItemAdd()
        {
            IsOpenfly = false;
           await dxFlyoutchucnang.CloseAsync();
            //nvlNhapXuatItemShow.ViTri = "Temp";
            NvlNhapXuatKhoShow nvlNhapXuatKhoShow = new NvlNhapXuatKhoShow();
            nvlNhapXuatKhoShow.ThanhToan = "Chưa làm hồ sơ thanh toán";
            nvlNhapXuatKhoShow.MaGN = nvlNhapXuatKhoShowcrr.MaGN;
            renderFragment = builder =>
           {
               builder.OpenComponent<Page_ThanhToan_ChungTu>(0);

               builder.AddAttribute(1, "nvlNhapXuatKhoShowcrr", nvlNhapXuatKhoShow);
               builder.AddAttribute(1, "nvlThanhToanShowcrr", nvlNhapXuatKhoShowcrr.CopyClass());
               // builder.AddAttribute(2, "nvlNhapXuatItemShowcrr", nvlNhapXuatItemShow)

               builder.CloseComponent();
           };

          await  dxPopup.showAsync("THÊM CHỨNG TỪ");
          await  dxPopup.ShowAsync();
        }
        private async Task<UyNhiemChi> loadUNCAsync()
        {
            string sql= string.Format(@"use [SPSupplier]
                    declare @SerialHD int={0}

                    select * from UyNhiemChi where MaUNC
                    in (SELECT MaUNC
                      FROM [dbo].[UyNhiemChi_Item]
                     where  SerialHD=@SerialHD and TableName='NvlThanhToan')
                    ",nvlNhapXuatKhoShowcrr.Serial);
            CallAPI callAPI = new CallAPI();
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            
            //lstpara.Add(new ParameterDefine("@TableName", "NvlThanhToan"));
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<UyNhiemChi>>(json);
               if(query.Count>0)
                {
                   return query.FirstOrDefault();
                }
                //Grid.Data = lstDonDatHangSearchShow;
            }
            return null;

        }
        private async void XuatUNC()
        {
            IsOpenfly = false;
            await dxFlyoutchucnang.CloseAsync();

            //Xử lý tải UNC
            
            UyNhiemChi uyNhiemChi= new UyNhiemChi();
            uyNhiemChi= await loadUNCAsync();
            if (uyNhiemChi == null)
            {
                 uyNhiemChi = new UyNhiemChi();
                uyNhiemChi.Serial = 0;
                uyNhiemChi.Ngay = nvlNhapXuatKhoShowcrr.Ngay;
                uyNhiemChi.NoiDung = string.Format("Thanh toán tiền hàng theo hóa đơn {0} ({1})", nvlNhapXuatKhoShowcrr.MaCTTT,nvlNhapXuatKhoShowcrr.Ngay.Value.ToString("dd/MM/yy"));
                uyNhiemChi.DVT = "VND";
                uyNhiemChi.SoTKNo = "007 0000 118 023";
                uyNhiemChi.SerialTT = nvlNhapXuatKhoShowcrr.Serial;
                uyNhiemChi.SoHD = nvlNhapXuatKhoShowcrr.MaCTTT;
                uyNhiemChi.MaNCC = nvlNhapXuatKhoShowcrr.MaGN;
                uyNhiemChi.MaUNC = nvlNhapXuatKhoShowcrr.MaUNC;
                
            }
            uyNhiemChi.ThanhTien = Convert.ToDouble(nvlNhapXuatKhoShowcrr.ThanhTien);
            //nvlNhapXuatItemShow.ViTri = "Temp";

            //if(nvlNhapXuatKhoShowcrr.lstnhapxuat == null)
            //{
            //    dxGrid.ExpandDetailRow(visibleindexcrr);

            //}


            renderFragment = builder =>
            {
                builder.OpenComponent<Page_TaoUyNhiemChi>(0);

                builder.AddAttribute(1, "uyNhiemChi", uyNhiemChi);
                builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<UyNhiemChi>(this, GotoMainForm));
                //builder.AddAttribute(1, "nvlThanhToanShowcrr", nvlNhapXuatKhoShowcrr.CopyClass());
                // builder.AddAttribute(2, "nvlNhapXuatItemShowcrr", nvlNhapXuatItemShow)
                builder.CloseComponent();
            };

            await dxPopup.showAsync("TẠO ỦY NHIỆM CHI");
            await dxPopup.ShowAsync();
        }
        private void GotoMainForm(UyNhiemChi uyNhiemChi)
        {
            nvlNhapXuatKhoShowcrr.MaUNC=uyNhiemChi.MaUNC;
            //nvlNhapXuatKhoShowcrr. = uyNhiemChi.CopyClass();
            dxPopup.CloseAsync();
            StateHasChanged();
        }
        private async Task printAsync()
        {
            dxFlyoutchucnang.CloseAsync();
            string sql = string.Format(@"
                use [SPSupplier]
                    declare @dttemp Table
                    ([MaUNC] nvarchar(100),[SoTKNo] nvarchar(100),[SoTKCo] nvarchar(100),[ThanhTien] float,[DVT] nvarchar(100),[NoiDung] nvarchar(300),[Ngay] datetime,NguoiThanhToan nvarchar(100))

					insert into @dttemp([MaUNC],[SoTKNo],[SoTKCo]
                          ,[ThanhTien],[DVT],[NoiDung],[Ngay], NguoiThanhToan)
                    (SELECT
                          [MaUNC],[SoTKNo],[SoTKCo]
                          ,[ThanhTien],[DVT],[NoiDung],[Ngay],isnull(NguoiThanhToan,'') as NguoiThanhToan
                      FROM [UyNhiemChi]
                      where MaUNC=N'{0}')

                      select tttk.TenTaiKhoan as TenTKNo,tttk.NganHang as NganHangTKNo,tttk.ChiNhanh as ChiNhanhTKNo,tttk.DiaChi as DiaChiTKNo,tttk.NganHang as NganHangTKNo
                      ,dttemp.MaUNC,dttemp.SoTKNo,dttemp.SoTKCo,dttemp.Ngay,dttemp.DVT,dttemp.NoiDung as DienGiai,dttemp.ThanhTien,'' as BangChu,NguoiThanhToan,
                      tttkco.TenTaiKhoan as TenTKCo,tttkco.ChiNhanh as ChiNhanhTKCo,tttkco.NganHang as NganHangTKCo,tttkco.DiaChi as DiaChiTKCo,cast(1 as bit) as Chk
                      from @dttemp dttemp
                      inner join dbo.ThongTinTaiKhoan tttk on dttemp.SoTKNo=tttk.SoTK
                      inner join dbo.ThongTinTaiKhoan tttkco on dttemp.SoTKCo=tttkco.SoTK", nvlNhapXuatKhoShowcrr.MaUNC);

            
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            GetDataFromSql getDataFromSql = new GetDataFromSql();
           
            getDataFromSql.reportname = "XRp_UNC";

            //classReport.dtparameter = new DataTable();
            getDataFromSql.sql = sql;
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
           
            //dt.Dispose();

            await ModelAdmin.mainLayout.showreportpdfAsync(getDataFromSql);

            //CallAPI callAPI = new CallAPI();
            //string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
            //if (json != "")
            //{
            //    var query = JsonConvert.DeserializeObject<DataTable>(json);
            //    DataTable dataTable = query;
            //    if (dataTable.Rows.Count > 0)
            //    {
            //        DataRow row0 = dataTable.Rows[0];
            //        row0["BangChu"] = prs.FirstCharToUpper(prs.docsothapphan(double.Parse(row0["ThanhTien"].ToString())) + ((row0["DVT"].ToString() == "VND" || row0["DVT"].ToString() == "VNĐ") ? " đồng" : " ") + "./.");
            //        if (row0["NganHangTKNo"].ToString() == row0["NganHangTKCo"].ToString())
            //        {
            //            row0["Chk"] = false;
            //        }
            //        Showreport(row0["NganHangTKNo"].ToString().ToLower(), dataTable);
            //        dataTable.Dispose();
                   
            //    }
            //    //Grid.Data = lstDonDatHangSearchShow;
            //}
        }
        private async void ThemMoiTaiKhoanNganHang()
        {
            IsOpenfly = false;
            await dxFlyoutchucnang.CloseAsync();

            //Xử lý tải UNC

            ThongTinTaiKhoan thongTinTaiKhoan = new ThongTinTaiKhoan();
            thongTinTaiKhoan.Serial = 0;
            thongTinTaiKhoan.MaNCC=nvlNhapXuatKhoShowcrr.MaGN;


            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_TaiKhoanAdd>(0);

                builder.AddAttribute(1, "thongTinTaiKhoan", thongTinTaiKhoan);
               // builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<UyNhiemChi>(this, GotoMainForm));
                //builder.AddAttribute(1, "nvlThanhToanShowcrr", nvlNhapXuatKhoShowcrr.CopyClass());
                // builder.AddAttribute(2, "nvlNhapXuatItemShowcrr", nvlNhapXuatItemShow)
                builder.CloseComponent();
            };

            await dxPopup.showAsync("THÊM MỚI SỐ TÀI KHOẢN");
            await dxPopup.ShowAsync();
        }



    }

}
