using BlazorBootstrap;
using DevExpress.Blazor;
using DevExpress.Utils.Filtering.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System;
using System.Data;
using System.Globalization;
namespace NFCWebBlazor.App_KeHoach
{
    public partial class Page_BaoCaoNangSuat
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] BrowserService browserService { get; set; }
        [Inject] UserGlobal userGlobal { get; set; }
       
        protected override Task OnInitializedAsync()
        {

            return base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _ = loadAsync();

            }
            try
            {


                var dimension = await browserService.GetDimensions();
                // var heighrow = await browserService.GetHeighWithID("divcontainer");
                int height = dimension.Height - 70;
                //if (heighrow!=null)
                //{
                //    height = dimension.Height - heighrow;
                //}

                heightgrid = string.Format("{0}px", height);
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi browserService :" + ex.Message));
            }


            //await JS.InvokeVoidAsync("scrollToBottomLast");

            //base.OnAfterRender(firstRender);
        }
        int columnsbegin = 0;
        private async Task loadAsync()
        {
            try
            {
                nhamayselected = txtnhamay.SelectedValue(userGlobal.users.NhaMay);
                baocaoselected = txtloaibaocao.SelectedValue("NhapKhoTP");
                goptheoselected = txtgoptheo.SelectedValue("Ngay");
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
            }
            //baocaoselected = lstloaibaocao.FirstOrDefault(p => p.Name.Equals("Tồn kho theo sản phẩm"));
            StateHasChanged();

        }
        public RenderFragment buildrender()
        {
            return BuildColumns(lstcolumn);
        }
        public static RenderFragment BuildColumns(List<InitDxGridDataColumn> _lstcolumn)
        {
            RenderFragment columns = b =>
            {
                int counter = 0;
                foreach (InitDxGridDataColumn col in _lstcolumn.OrderBy(p => p.Index))
                {
                    b.OpenComponent(counter, typeof(DxGridDataColumn));
                    b.AddAttribute(0, "FieldName", col.FieldName);

                    b.AddAttribute(0, "Caption", col.Caption);
                    if (col.gridTextAlignment != null)
                        b.AddAttribute(0, "TextAlignment", col.gridTextAlignment);
                    if (col.DisplayFormat != null)
                    {
                        b.AddAttribute(0, "DisplayFormat", col.DisplayFormat);
                    }
                    if (col.Width != null)
                        b.AddAttribute(0, "Width", string.Format("{0}px", col.Width));
                    else
                    if (col.Width != null)
                        b.AddAttribute(0, "MinWidth", string.Format("90px"));
                    if (col.GroupIndex != null)
                    {
                        b.AddAttribute(0, "GroupIndex", col.GroupIndex.Value);
                    }
                    b.CloseComponent();

                    counter++;
                }


             

            };
            return columns;
        }


        public class NhapkhotpItem
        {
            public string MaSP { get; set; }
            public string TenSP { get; set; }
            public double SLKH { get; set; }
            public double SLThucHien { get; set; }
            public double SLNhap { get; set; }
            public double SLConLai { get; set; }
            public double ThanhTien { get; set; }
            public double SoKhoi { get; set; }
            public string TenMau { get; set; }
            public string Colorhex { get; set; }
            public string NhaMay { get; set; }
            public DateTime Ngay { get; set; }

            public string DienGiai { get; set; }

        }
        class DonHangItemDenNgay
        {
            public string MaSP { get; set; }
            public string TenMau { get; set; }
            public Nullable<double> SLConLai { get; set; }
            public Nullable<double> SLTonKho { get; set; }
        }
       
        private async void search()
        {
            if (baocaoselected.Name == null || goptheoselected == null || dtpbegin == null || dtpend == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Vui lòng nhập đầy đủ thông tin LOẠI BÁO CÁO, GỘP THEO, NGÀY trước khi tìm kiếm"));
                return;
            }

            lstcolumn.Clear();
            dtresultfinal.Clear();
            dtresultfinal.Columns.Clear();
            lstsokhoi.Clear();
           
            Thread.Sleep(100);
            DataTable dt = await searchAsync();

            bool checkname = false;
            foreach (var it in dxGrid.GetDataColumns())
            {
                if (it.FieldName == null)
                    continue;
                //Console.WriteLine(it.FieldName);
                checkname = false;
                foreach (DataColumn cl in dt.Columns)
                {
                    if (cl.ColumnName == it.FieldName)
                    {
                        checkname = true;
                        break;
                    }
                }
                if (!checkname)
                {
                    //it.Visible = false;
                    dt.Columns.Add(it.FieldName, typeof(string));
                }
            }
         
            dtresultfinal = dt;

            //foreach (DataColumn cl in dtresultfinal.Columns)
            //{
            //    Console.WriteLine(cl.ColumnName);
            //}
            //Console.WriteLine(dtresultfinal.Rows.Count);
            //dt.Dispose();
            // dxGrid.Data = dtresultfinal;
            //dxGrid.Reload();
            hidedivsearch();

            //dxGrid.AutoFitColumnWidths();
            StateHasChanged();


            //foreach (InitDxGridDataColumn it in lstcolumn)
            //{

            //}
        }


        string ghichu = "";
        App_ClassDefine.ClassProcess prs = new ClassProcess();
        List<string> lstcolumnloi = new List<string>();

        private async Task<DataTable> searchAsync()
        {

            //dtbangke.Clear();
            //dtbangke.Dispose();
            //dtbangke.Columns.Clear();
            // dtbangke = new DataTable();
            //tvView.FormatConditions.Clear();
            DataTable dt = new DataTable();

            if (goptheoselected.FullName == "Gộp theo tuần")
            {
                //dtBatDau.SelectedDate = StartOfWeek(dtBatDau.SelectedDate.Value, DayOfWeek.Sunday);

                //dtKetThuc.SelectedDate = EndOfWeek(dtKetThuc.SelectedDate.Value, DayOfWeek.Sunday);
            }
            if (goptheoselected.FullName == "Gộp theo tháng")
            {

                dtpbegin = new DateTime(dtpbegin.Value.Year, dtpbegin.Value.Month, 1, 0, 0, 0);
                int lastminth = DateTime.DaysInMonth(dtpend.Value.Year, dtpend.Value.Month);
                dtpend = new DateTime(dtpend.Value.Year, dtpend.Value.Month, lastminth, 23, 59, 0);
            }
            if (goptheoselected.FullName == "Gộp theo năm")
            {
                dtpbegin = new DateTime(dtpbegin.Value.Year, 1, 1, 0, 0, 0);
                int lastminth = DateTime.DaysInMonth(dtpend.Value.Year, dtpend.Value.Month);
                dtpend = new DateTime(dtpend.Value.Year, 12, 31, 23, 59, 0);
            }
            try
            {
                PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");

                if (baocaoselected.FullName == "Nhập kho thành phẩm")
                {
                    if (nhamayselected == null)
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, $"Vui lòng chọn nhà máy"));

                        return dt;
                    }
                    if (nhamayselected.Name == "All")
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, $"Báo cáo này xuất theo nhà máy cụ thể"));
                        return dt;
                    }
                    //lbdiengiaichart.Text = string.Format("Nhập kho từ {0} - {1}", dtBatDau.SelectedDate.Value.ToString("dd/MM"), dtKetThuc.SelectedDate.Value.ToString("dd/MM"));
                    //lbtonkhochart.Text = string.Format("Tồn kho cuối ngày {0}", dtBatDau.SelectedDate.Value.AddDays(-1).ToString("dd/MM"));
                    //lbdonhangchart.Text = string.Format("Tổng Đơn Hàng chưa xuất cuối ngày {0}", dtBatDau.SelectedDate.Value.AddDays(-1).ToString("dd/MM"));
                    //grdchuthich.Visibility = Visibility.Visible;
                    dt = await xulykehoachthangnewAsync(goptheoselected.Name);

                    //nhapkhotp(txtLoaiBaoCao.SelectedValue.ToString());
                }
                if (baocaoselected.FullName == "Kế hoạch tháng đã duyệt")
                {
                    if (nhamayselected == null)
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, $"Vui lòng chọn nhà máy"));
                        return dt;
                    }
                    if (nhamayselected.Name == "All")
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Báo cáo này xuất theo nhà máy cụ thể"));
                        return dt;
                    }
                    dtpbegin = new DateTime(dtpbegin.Value.Year, dtpbegin.Value.Month, 1, 0, 0, 0);
                    int lastminth = DateTime.DaysInMonth(dtpend.Value.Year, dtpend.Value.Month);
                    dtpend = new DateTime(dtpend.Value.Year, dtpend.Value.Month, lastminth, 23, 59, 0);  //new DateTime(dtBatDau.SelectedDate.Value.Year, dtBatDau.SelectedDate.Value.Month, 1, 0, 0, 0);                                                                              //grdchuthich.Visibility = Visibility.Visible;
                    dt = await KeHoachSanXuatThangAsync();
                }
                if (baocaoselected.FullName == "Năng suất sản xuất")
                {
                    dt = await NangSuatTuNgayDenNgay(goptheoselected.Name);
                }
                if (baocaoselected.FullName == "Giá trị xuất kho thành phẩm")
                {
                    goptheoselected = txtgoptheo.SelectedValue("Thang");
                    dtpbegin = new DateTime(dtpbegin.Value.Year, dtpbegin.Value.Month, 1, 0, 0, 0);
                    int lastminth = DateTime.DaysInMonth(dtpend.Value.Year, dtpend.Value.Month);
                    dtpend = new DateTime(dtpend.Value.Year, dtpend.Value.Month, lastminth, 23, 59, 0);
                    dt = await XuatKhoTuNgayDenNgay();
                }
                if (baocaoselected.FullName == "Giá trị nhập kho thành phẩm")
                {
                    goptheoselected = txtgoptheo.SelectedValue("Thang");
                    dtpend = new DateTime(dtpend.Value.Year, dtpend.Value.Month, 1, 0, 0, 0);
                    int lastminth = DateTime.DaysInMonth(dtpend.Value.Year, dtpend.Value.Month);
                    dtpend = new DateTime(dtpend.Value.Year, dtpend.Value.Month, lastminth, 23, 59, 0);
                    dt = await NhapKhoTuNgayDenNgay();
                }
            }
            catch(Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Lỗi "+ex.Message));
            }
            finally
            {
                PreloadService.Hide();
            }
           
            return dt;

        }
        class SLNgayJson
        {
           public int SLNgay { get; set; }
        }
        List<NhapkhotpItem> lstsokhoi = new List<NhapkhotpItem>();
       
        private async Task<DataTable> xulykehoachthangnewAsync(string loaibaocao)
        {


            visiblechart = true;
            titlechart = string.Format("Tổng đơn hàng chưa xuất cuối ngày {0}", dtpbegin.Value.AddDays(-1).ToString("dd-MM-yy"));
            titlecharttonkho = string.Format("Tồn kho cuối ngày {0}", dtpbegin.Value.AddDays(-1).ToString("dd-MM-yy"));
            titlechartthuchien = string.Format("Nhập kho từ {0} - {1}", dtpbegin.Value.ToString("dd-MM"),dtpend.Value.ToString("dd/MM"));
            string sqlreplace = "";
            if (loaibaocao == "Ngay")
            {
                sqlreplace = "Ngay";
            }
            if (loaibaocao == "Tuan")
            {
                sqlreplace = "dbo.GetFisrtDayOfWeek(Ngay)";
            }
            if (loaibaocao == "Thang")
            {
                sqlreplace = "dbo.GetFisrtDayOfMonth(Ngay)";
            }
            if (loaibaocao == "Quy")
            {
                sqlreplace = "dbo.GetFisrtDayOfQuarter(Ngay)";
            }
            if (loaibaocao == "Nam")
            {
                sqlreplace = "dbo.GetFisrtDayOfYear(Ngay)";
            }
            string sql = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                            declare @DateBegin datetime='{1}'
                            declare @DateEnd datetime='{2}'
                          
                            declare @DateMonthBegin int=datepart(MM,@DateBegin)
                            declare @DateMonthEnd int=datepart(MM,@DateEnd)
                            declare @DienGiai nvarchar(100)=''
                           
                            declare @tbl Table(MaSP nvarchar(100),SLNhap float,SLConLai float,ThanhTien float,Ngay date,TenMau nvarchar(100))
                           
                              insert into @tbl
							select MaSP,sum(SLNhap) as SLNhap,sum(SLNhap) as SLConLai,sum(ThanhTien) as ThanhTien,Ngay,TenMau
							from(
                            select qrynk.MaSP,SLNhap,ThanhTien,Ngay,isnull(mm.TenMau,'') as TenMau
							from
							(SELECT [MaSP],sum([SLNhap]) as SLNhap,sum(SLNhap*Gia) as ThanhTien,{3} as Ngay,ArticleNumberID 
                              FROM [KhoTP_NK] where LyDo=N'Nhập mới' and Ngay>=@DateBegin and Ngay<=@DateEnd and NhaMay=@NhaMay
                              group by MaSP,Ngay,ArticleNumberID) as qrynk
							  inner join dbo.ArticleNumberProduct art on qrynk.ArticleNumberID=art.ArticleNumber
							  inner join dbo.MaMau mm on art.MaMau=mm.MaMau) as qry group by MaSP,TenMau,Ngay
                            
                           select * from @tbl
                            ", nhamayselected.Name, dtpbegin.Value.ToString("MM/dd/yyyy 00:00"), dtpend.Value.ToString("MM/dd/yyyy 23:59"), sqlreplace);
            DataTable dtbangke = new DataTable();
            CallAPI callAPI = new CallAPI();
            string json = "";
            json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
            if(json=="")
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Không có dữ liệu, vui lòng kiểm tra lại thông tin tìm kiếm"));

                return dtbangke;
            }
            var queryktp = JsonConvert.DeserializeObject<List<NhapkhotpItem>>(json);
            //Kehoach
            sql = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                            declare @DateBegin datetime='{1}'
                            declare @DateEnd datetime='{2}'
                            declare @DateMonthBegin int=datepart(MM,@DateBegin)
                            declare @DateMonthEnd int=datepart(MM,@DateEnd)
                            declare @DienGiai nvarchar(100)=''

                            select qry.MaSP,cast(SLNhap as float) as SLKH,case when ThanhTien=0 then SLNhap*qryGia.Gia else ThanhTien end as ThanhTien,cast(0 as float) as SLThucHien,'KeHoach' as DienGiai,TenMau,Colorhex from 
							(select MaSP,sum(SLNhap) as SLNhap,sum(ThanhTien) as ThanhTien,TenMau,Colorhex from
							(select qry.MaSP,qry.SLNhap,isnull(mm.TenMau,'') as TenMau,isnull(Colorhex,'#FFFFFF') as Colorhex,(SLNhap*isnull(Gia,0)) as ThanhTien from
                            (
                            SELECT [MaSP],sum([SoLuong]) as SLNhap,ArticleNumber
                              FROM [KeHoachNgayItem]
                              where datepart(MM,Ngay)>=@DateMonthBegin and datepart(MM,Ngay)<=@DateMonthEnd  and SerialKHNgay in (select STT from [KeHoachNgay]  where KhuVuc='KhoTP' and NhaMay=@NhaMay)
                               and datepart(yy,Ngay)>=datepart(yy,@DateBegin) and datepart(yy,Ngay)<=datepart(yy,@DateEnd) group by MaSP,ArticleNumber
                            ) as qry
							left join ArticleNumberProduct art  on qry.ArticleNumber=art.ArticleNumber
							left  join dbo.MaMau mm on art.MaMau=mm.MaMau) as qrytotal
							group by MaSP,TenMau,Colorhex) as qry
                            left join
							(SELECT [MaSP],avg(Gia) as Gia
							 FROM [ArticleNumberProduct]  group by MaSP) as qryGia
							 on qry.MaSP=qryGia.MaSP",
                       nhamayselected.Name, dtpbegin.Value.ToString("MM/dd/yyyy 00:00"), dtpend.Value.ToString("MM/dd/yyyy 23:59"), sqlreplace);
            //var querykh = cTEntities.Database.SqlQuery<NhapkhotpItem>(sql, "").ToList();
            json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());


            var querykh = JsonConvert.DeserializeObject<List<NhapkhotpItem>>(json);
          
            if (lstsokhoi.Count == 0)
            {
                sql = @"select sp.MaSP,sp.TenSP,qry.SoKhoi from 
                            (select MaSP,sum(SoLuongCT*ChieuDayTC*ChieuRongTC*ChieuDaiTC)/1000000000 as SoKhoi
											                            from dbo.ChiTietSP where MaChiTiet 
											                            in (select MaCT from ChiTiet_KhuVuc where KhuVuc='KV1X') 
																		group by MaSP) as qry
																		inner join dbo.SanPham sp on qry.MaSP=sp.MaSP";
                json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
                var querysk = JsonConvert.DeserializeObject<List<NhapkhotpItem>>(json);
                //var querysk = cTEntities.Database.SqlQuery<NhapkhotpItem>(sql, "").ToList();
                lstsokhoi.AddRange(querysk);
            }
            // var querygroup = queryktp.GroupBy(p => new { MaSP = p.MaSP }).Select(p => new { MaSP = p.Key.MaSP }).ToList(); ;
            var querycolumn = queryktp.GroupBy(p => p.Ngay).Select(p => new { Ngay = p.Key.ToString("yyyy-MM-dd") }).OrderBy(p => p.Ngay).ToList();

            //Xử lý phần đơn hàng
            sql = string.Format(@" declare @dateend date='{0}'
 
                      select MaSP,TenMau,case when SLDH<SLDHThucNhan then SLDHThucNhan else SLDH end as SLDH,SLDaXuat,case when SLDH<SLDHThucNhan then SLDHThucNhan-SLDaXuat else SLDH-SLDaXuat end as SLConLai,SLTonKho  from 
                (select art.MaSP,mm.TenMau,sum(qry.SLDonHang) as SLDH,sum(qry.SLDHThucNhan) as SLDHThucNhan,sum(SLDaXuat) as SLDaXuat,sum(SLTonKho) as SLTonKho
                 from (select ArticleNumber,qry.SoLuong as SLDonHang,0 as SLDHThucNhan,0 as SLDaXuat,0 as SLTonKho
                from (SELECT [MaSP],sum([SoLuong]) as SoLuong,ArticleNumber FROM [DonHangMua]   group by [MaSP],ArticleNumber) as qry
                                     union all
                                    SELECT ArticleNumber,0 as SLDonHang,sum(SLDonHang) as SLDHThucNhan,sum(SLDaXuat) as SLDaXuat,0 as SLTonKho
                                    FROM [KeHoachXuatHang]
                                    where NgayXuat<@dateend group by ArticleNumber
                                    union all
                                    select ArticleNumberID,0 as SLDonHang,0 as SLDHThucNhan,0 as SLDaXuat,SLTon as SLTonKho
                                    from dbo.KhoTP_TonKhoDenNgay(@dateend))
                                    as qry   inner join dbo.ArticleNumberProduct art on qry.ArticleNumber= art.ArticleNumber
                                    inner join dbo.MaMau mm on art.MaMau=mm.MaMau
                                    group by art.MaSP,mm.TenMau
					) as qryall", dtpbegin.Value.ToString("MM/dd/yyyy 00:00"));

            List<DonHangItemDenNgay> lstdonhang = new List<DonHangItemDenNgay>();
            json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
            var querydh = JsonConvert.DeserializeObject<List<DonHangItemDenNgay>>(json);
            lstdonhang.AddRange(querydh);

            dtbangke.Columns.Add("MaSP", typeof(string));
            dtbangke.Columns.Add("TenSP", typeof(string));
            dtbangke.Columns.Add("SoKhoi", typeof(double));
            dtbangke.Columns.Add("TenMau", typeof(string));
            dtbangke.Columns.Add("Colorhex", typeof(string));
            dtbangke.Columns.Add("VisibleChart", typeof(bool));
            dtbangke.Columns["VisibleChart"].DefaultValue = false;
            lstcolumn.Add(new InitDxGridDataColumn(0, "MaSP", "Mã SP", null, 100));
            lstcolumn.Add(new InitDxGridDataColumn(0, "TenSP", "Tên SP", null, 240));
            lstcolumn.Add(new InitDxGridDataColumn(0, "TenMau", "Tên màu", null, 120));
            lstcolumn.Add(new InitDxGridDataColumn(0, "SoKhoi", "Số khối / SP", "#,0.####", 100));
            //grvSanPham.Columns.Add(addGridColumnPercent("TyLe", "% Thực hiện"));


            dtbangke.Columns.Add("SLDHConLai", typeof(double));
            dtbangke.Columns.Add("SLTonKho", typeof(double));
            dtbangke.Columns.Add("wSLDHConLai", typeof(double));
            dtbangke.Columns.Add("wSLTonKho", typeof(double));
            dtbangke.Columns.Add("wSLThucHien", typeof(double));
            dtbangke.Columns.Add("chart2", typeof(double));
            dtbangke.Columns.Add("wchart2", typeof(double));
          

            dtbangke.Columns["wSLDHConLai"].DefaultValue = 0;
            dtbangke.Columns["wSLTonKho"].DefaultValue = 0;
            dtbangke.Columns["wSLThucHien"].DefaultValue = 0;
            dtbangke.Columns["chart2"].DefaultValue = 0;
            dtbangke.Columns["wchart2"].DefaultValue = 0;

            //GridControlBand gridControlBanddonhang = addGridControlBand("DonHang", "Tổng đơn hàng");
            titlechart = string.Format("Số liệu chốt cuối ngày {0}", dtpbegin.Value.AddDays(-1).ToString("dd-MM-yy"));
            //GridColumn columndh = addGridColumn("DonHang", string.Format("Số liệu chốt cuối ngày {0}", dtBatDau.SelectedDate.Value.AddDays(-1).ToString("dd-MM-yy")));
            //columndh.CellTemplate = dataTemplatenhapkho;

            //grvSanPham.Columns.Add(columndh);

           // lstcolumn.Add(new InitDxGridDataColumn(0, "DonHang", string.Format("Số liệu chốt cuối ngày {0}", dtpbegin.Value.AddDays(-1).ToString("dd-MM-yy")), "#,#.0", 260));

            DateTime dtcheck = new DateTime();
            List<DateTime> lstdate = new List<DateTime>();
            foreach (var it in querycolumn)
            {
                if (DateTime.TryParseExact(it.Ngay, "yyyy-MM-dd", CultureInfo.CurrentCulture,
                                                  DateTimeStyles.None, out dtcheck))
                {
                    lstdate.Add(dtcheck);
                    dtbangke.Columns.Add(it.Ngay, typeof(double));
                    lstcolumn.Add(new InitDxGridDataColumn(0, it.Ngay, it.Ngay,"#,#",110));
                    //grvSanPham.Columns.Add(addGridColumn(it.Ngay, it.Ngay));
                }
            }

            dtbangke.Columns.Add("SLKH", typeof(double));
            dtbangke.Columns.Add("SLThucHien", typeof(double));
            dtbangke.Columns.Add("SLDatKH", typeof(double));
            lstcolumn.Add(new InitDxGridDataColumn(0, "SLThucHien", "Tổng nhập kho", "#,#.#",110));
            lstcolumn.Add(new InitDxGridDataColumn(0, "SLKH", "Kế hoạch", "#,#.#",110));
            lstcolumn.Add(new InitDxGridDataColumn(0, "SLDatKH", "Đạt kế hoạch", "#,#.#",110));
            //lstcolumn.Add(new InitDxGridDataColumn(0, "TyLe", "% Thực hiện","#,#",130));
            dtbangke.Columns.Add("TyLe", typeof(double));

            //InitGrid("TyLe");
            //Xử lý phần màu trước
            foreach (var itkehoach in querykh)
            {
                if (itkehoach.TenMau != "")
                {
                    foreach (var nk in queryktp)
                    {
                        if (itkehoach.MaSP == nk.MaSP)
                        {
                            if (itkehoach.TenMau == nk.TenMau)
                            {
                                //DataRow dataRow = new DataRow();
                                itkehoach.SLThucHien += nk.SLConLai;
                                //itkehoach.ThanhTien =(nk.SLNhap==0)?0:((nk.ThanhTien / nk.SLNhap) * itkehoach.SLKH);
                                nk.DienGiai = "Mau";//Đánh dấu lại những dòng có màu khi đã đổ vào kế hoach
                                nk.SLConLai = 0;
                            }
                        }
                    }
                }
            }
            //Xử lý đến không màu
            foreach (var itkehoach in querykh)//Cộng tất cả số  dư không phân biệt màu vào
            {
                if (itkehoach.TenMau == "")
                {
                    foreach (var nk in queryktp)
                    {
                        if (itkehoach.MaSP == nk.MaSP)
                        {
                            //DataRow dataRow = new DataRow();
                            itkehoach.SLThucHien += nk.SLConLai;
                            //itkehoach.ThanhTien = (nk.SLNhap == 0) ? 0 : ((nk.ThanhTien / nk.SLNhap) * itkehoach.SLKH);
                            nk.SLConLai = 0;
                        }
                    }
                }
            }
            //Xử lý đến đám mà không có kế hoạch nhưng vẫn phải vét vào
            var querydamconlai = queryktp.Where(p => p.SLConLai > 0).GroupBy(p => new { MaSP = p.MaSP }).Select(p => new NhapkhotpItem { MaSP = p.Key.MaSP, SLKH = 0, SLThucHien = p.Sum(n => n.SLConLai), TenMau = "" }).ToList();
            querykh.AddRange(querydamconlai);
            foreach (var it in querykh)
            {
                foreach (var sk in lstsokhoi)
                {
                    if (it.MaSP == sk.MaSP)
                    {
                        it.SoKhoi = sk.SoKhoi;
                        it.TenSP = sk.TenSP;
                        break;
                    }
                }
                DataRow row = dtbangke.NewRow();

                row["MaSP"] = it.MaSP;
                row["TenSP"] = it.TenSP;
                row["SoKhoi"] = it.SoKhoi;
                row["SLThucHien"] = it.SLThucHien;
                row["SLKH"] = it.SLKH;
                row["SLDatKH"] = (it.SLThucHien > it.SLKH) ? it.SLKH : it.SLThucHien;
                row["TyLe"] = (it.SLKH == 0) ? 0 : (double.Parse(row["SLDatKH"].ToString()) / it.SLKH);
                row["TenMau"] = it.TenMau;
                row["Colorhex"] = it.Colorhex;
                dtbangke.Rows.Add(row);
            }
            foreach (var it in queryktp)
            {
                foreach (var sk in lstsokhoi)
                {
                    if (it.MaSP == sk.MaSP)
                    {
                        it.SoKhoi = sk.SoKhoi;
                        it.TenSP = sk.TenSP;
                        break;
                    }
                }
            }
            //Xử lý đám có màu trước
            var querycomau = queryktp.Where(p => p.DienGiai == "Mau").ToList();
            foreach (var it in querycomau)
            {
                foreach (DataRow rowbk in dtbangke.Rows)
                {
                    if (it.MaSP == rowbk["MaSP"].ToString())
                    {
                        if (it.TenMau == rowbk["TenMau"].ToString())
                        {
                            rowbk[it.Ngay.ToString("yyyy-MM-dd")] = ((rowbk[it.Ngay.ToString("yyyy-MM-dd")] == DBNull.Value) ? 0 : rowbk.Field<double>(it.Ngay.ToString("yyyy-MM-dd"))) + it.SLNhap;
                            break;
                        }
                    }
                }
            }
            //Xử lý đám không màu
            var querykhongmau = queryktp.Where(p => p.DienGiai != "Mau").ToList();
            {
                foreach (var it in querykhongmau)
                {
                    foreach (DataRow rowbk in dtbangke.Rows)
                    {
                        if (it.MaSP == rowbk["MaSP"].ToString() && rowbk["TenMau"].ToString() == "")
                        {

                            rowbk[it.Ngay.ToString("yyyy-MM-dd")] = ((rowbk[it.Ngay.ToString("yyyy-MM-dd")] == DBNull.Value) ? 0 : rowbk.Field<double>(it.Ngay.ToString("yyyy-MM-dd"))) + it.SLNhap;
                            break;
                        }
                    }
                }
            }
            // ,SKTHKH = p.Sum(n => ((n.SLThucHien > n.SLKH) ? n.SLKH : n.SLThucHien) * n.SoKhoi)
            var querygroupsumktp = queryktp.GroupBy(p => new { Ngay = p.Ngay }).Select(p => new { Ngay = p.Key.Ngay.ToString("yyyy-MM-dd"), SoKhoiKH = p.Sum(n => n.SoKhoi * n.SLKH), SKThucHien = p.Sum(n => n.SLNhap * n.SoKhoi), ThanhTienTH = p.Sum(n => n.ThanhTien), ThanhTienKH = p.Sum(n => (n.SLThucHien == 0) ? 0 : ((n.ThanhTien / n.SLThucHien) * n.SLKH)) }).ToList();
            var querygrouptotal = querykh.GroupBy(p => 1).Select(p => new { TongKH = p.Sum(n => n.SLKH * n.SoKhoi), TongThucHien = p.Sum(n => n.SLThucHien * n.SoKhoi), SKDatKH = p.Sum(n => ((n.SLThucHien > n.SLKH) ? n.SLKH : n.SLThucHien) * n.SoKhoi), ThanhTienKH = p.Sum(n => n.ThanhTien) }).ToList();
            sql = string.Format(@"declare @Date datetime='{0}'

                        declare @DateBegin datetime
                        declare @DateEnd datetime
                        select @DateBegin=dbo.GetFisrtDayOfMonth(@Date)
                        select @DateEnd=dbo.GetLastDayOfMonth(@Date)
                        select cast(dbo.datediffexcludesunday(@DateBegin,@DateEnd) as int) as SLNgay", dtpbegin.Value.ToString("MM/dd/yyyy"));

            json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
            var qryslngay = JsonConvert.DeserializeObject<List<SLNgayJson>>(json);

            // var qryslngay = cTEntities.Database.SqlQuery<int>(sql, "").ToList();
            int slngay = 1;
            if (qryslngay.Count > 0)
            {
                slngay = qryslngay[0].SLNgay;
            }

            DataRow rowthanhtien = dtbangke.NewRow();
            DataRow rowtotal = dtbangke.NewRow();
           
            double sokhoingaykh = querygrouptotal[0].TongKH / slngay;
            rowtotal["MaSP"] = "---";
            rowtotal["TenSP"] = "Tổng khối lượng";
          
            rowthanhtien["MaSP"] = "---";
            rowthanhtien["TenSP"] = "Thành tiền (USD)";

            double dskkh = 0, dskthuchien = 0;
            double thanhtienth = 0;
            foreach (var it in querygroupsumktp)
            {
                rowtotal[it.Ngay] = it.SKThucHien;
                rowthanhtien[it.Ngay] = it.ThanhTienTH;

                //rowtylekhoi[it.Ngay] = it.SKThucHien / sokhoingaykh;

                dskkh += it.SoKhoiKH;

                // dskthkh += it.SKTHKH;
                dskthuchien += it.SKThucHien;
                // thanhtienkh += it.ThanhTienKH;
                thanhtienth += it.ThanhTienTH;

            }

            rowtotal["SLKH"] = querygrouptotal[0].TongKH;
            rowtotal["SLThucHien"] = querygrouptotal[0].TongThucHien;
            rowtotal["SLDatKH"] = querygrouptotal[0].SKDatKH;
            rowtotal["TyLe"] = (querygrouptotal[0].TongKH == 0) ? 0 : (querygrouptotal[0].SKDatKH / querygrouptotal[0].TongKH);
            //rowtotal["TyLe"] =(dskkh==0)?0:(dskthkh / dskthkh);
            //rowtotal["SLKH"] = dskkh;
            //rowtotal["SLThucHien"] = dskthuchien;
            rowthanhtien["SLKH"] = querygrouptotal[0].ThanhTienKH; ;
            rowthanhtien["SLThucHien"] = thanhtienth;
            //rowthanhtien["SLDatKH"] = querygrouptotal[0].SKDatKH;
            dtbangke.Rows.Add(rowtotal);
            //dtbangke.Rows.Add(rowtylekhoi);
            dtbangke.Rows.Add(rowthanhtien);

            List<QuyDoiNgay> lstquydoi = new List<QuyDoiNgay>();
            if (goptheoselected.FullName == "Gộp theo ngày")
            {
                foreach (var it in lstdate)
                {
                    foreach (InitDxGridDataColumn cl in lstcolumn)
                    {
                        if (it.ToString("yyyy-MM-dd") == cl.FieldName)
                        {
                            cl.Caption = it.ToString("dd/MM/yy");
                            break;
                        }
                    }
                }
            }
            else
            {
                if (goptheoselected.FullName == "Gộp theo tuần")
                    lstquydoi = prs.getDayWeekMonthYear(lstdate, "W");
                if (goptheoselected.FullName == "Gộp theo tháng")
                    lstquydoi = prs.getDayWeekMonthYear(lstdate, "M");
                if (goptheoselected.FullName == "Gộp theo quý")
                    lstquydoi = prs.getDayWeekMonthYear(lstdate, "Q");
                if (goptheoselected.FullName == "Gộp theo năm")
                    lstquydoi = prs.getDayWeekMonthYear(lstdate, "Y");
                foreach (var it in lstquydoi)
                {
                    foreach (InitDxGridDataColumn cl in lstcolumn)
                    {
                        if (it.Ngay.ToString("yyyy-MM-dd") == cl.FieldName)
                        {
                            cl.Caption = it.Ngayoutput;
                            break;
                        }
                    }
                }
            }
            int Monthbegin = dtpbegin.Value.Month;
            int Monthend = dtpend.Value.Month;
            foreach (InitDxGridDataColumn dxGridDataColumn in lstcolumn)
            {
                if (dxGridDataColumn.FieldName == "SLKH")
                {
                    if (Monthbegin - Monthend == 0)
                        dxGridDataColumn.Caption = string.Format("Kế hoạch tháng {0}", Monthbegin);
                    else
                    {
                        dxGridDataColumn.Caption = string.Format("Kế hoạch tháng {0} đến {1}", Monthbegin, Monthend);
                    }
                }

            }
            lstquydoi.Clear();

            double dpixel = 0;
            double totalwidth = 300;
            //Xử lý đơn hàng
            foreach (DataRow row in dtbangke.Rows)
            {
               
                foreach (var it in lstdonhang)
                {
                    if (row.Field<string>("MaSP") == it.MaSP && row.Field<string>("TenMau") == it.TenMau)
                    {
                        row["SLDHConLai"] = it.SLConLai;
                        row["SLTonKho"] = it.SLTonKho;
                        if (it.SLConLai <= 0)
                            row["wSLDHConLai"] = 0;
                        else
                            row["wSLDHConLai"] = totalwidth;


                        if (it.SLConLai <= 0)
                            dpixel = 1;
                        else
                            dpixel = totalwidth / it.SLConLai.Value;

                        row["wSLTonKho"] = dpixel * it.SLTonKho;
                        if (row.Field<double>("wSLTonKho") > totalwidth)
                            row["wSLTonKho"] = totalwidth;
                        if (row.Field<double>("wSLTonKho") < 40 && row.Field<double>("wSLTonKho") > 0)
                            row["wSLTonKho"] = 40;
                        row["wSLThucHien"] = dpixel * row.Field<double>("SLThucHien");
                        if (row.Field<double>("wSLThucHien") > totalwidth)
                            row["wSLThucHien"] = totalwidth;
                        if (row.Field<double>("wSLThucHien") < 40 && row.Field<double>("wSLThucHien") > 0)
                            row["wSLThucHien"] = 40;
                        it.SLConLai = 0;
                        it.SLTonKho = 0;

                        //Chuyển về %
                        //double total = row.Field<double>("wSLTonKho") + row.Field<double>("wSLThucHien");
                        row["VisibleChart"] = true;
                        row["wSLTonKho"] =Math.Round(row.Field<double>("wSLTonKho") / totalwidth * 100,0);
                        row["wSLThucHien"] = Math.Round(row.Field<double>("wSLThucHien") / totalwidth * 100, 0);
                        row["wchart2"] = row["wSLThucHien"];
                        row["chart2"] = row["SLThucHien"];
                        //row["TyLe"] = Math.Round(row.Field<double>("TyLe") * 100, 0);
                        break;
                    }
                   

                    //if (getdoubleofobject(row["SLDHConLai"]) <= 0)
                    //row["VisibleChart"] = false;
                }
            }

            querygroupsumktp.Clear();
            //prs.ExportExcel_HeaderwithNote(dtbangke, Model.ModelAdmin.pathexcelexport, 2, 1, "AAAA", 1);
            querycolumn.Clear();
            querykhongmau.Clear();
            querydamconlai.Clear();
            querykh.Clear();
            queryktp.Clear();
            return dtbangke;
        }
        private async Task<DataTable> KeHoachSanXuatThangAsync()
        {
            visiblechart = true;
            string sqlreplace = "";
            DataTable dtbangke = new DataTable();
            goptheoselected = txtgoptheo.SelectedValue("Thang");
            sqlreplace = "dbo.GetFisrtDayOfMonth(Ngay)";

            string sql = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                            declare @DateBegin datetime='{1}'
                            declare @DateEnd datetime='{2}'

                           

                            declare @DateMonthBegin int=datepart(MM,@DateBegin)
                            declare @DateMonthEnd int=datepart(MM,@DateEnd)
                            declare @DienGiai nvarchar(100)=''
                           
                            declare @tbl Table(MaSP nvarchar(100),SLNhap float,SLConLai float,ThanhTien float,Ngay date,TenMau nvarchar(100),Colorhex nvarchar(100))
                           
                              insert into @tbl
							select MaSP,sum(SLNhap) as SLNhap,sum(SLNhap) as SLConLai,sum(ThanhTien) as ThanhTien,Ngay,TenMau,Colorhex
							from(
                            select qrynk.MaSP,SLNhap,ThanhTien,Ngay,isnull(mm.TenMau,'') as TenMau,isnull(Colorhex,'#FFFFFF') as Colorhex
							from
							(SELECT [MaSP],sum([SLNhap]) as SLNhap,sum(SLNhap*Gia) as ThanhTien,{3} as Ngay,ArticleNumberID 
                              FROM [KhoTP_NK] where LyDo=N'Nhập mới' and Ngay>=@DateBegin and Ngay<=@DateEnd and NhaMay=@NhaMay
                              group by MaSP,Ngay,ArticleNumberID) as qrynk
							  inner join dbo.ArticleNumberProduct art on qrynk.ArticleNumberID=art.ArticleNumber
							  inner join dbo.MaMau mm on art.MaMau=mm.MaMau) as qry group by MaSP,TenMau,Ngay,Colorhex
                            
                           select * from @tbl
                            ", nhamayselected.Name, dtpbegin.Value.ToString("MM/dd/yyyy 00:00"), dtpend.Value.ToString("MM/dd/yyyy 23:59"), sqlreplace);
            CallAPI callAPI = new CallAPI();
            string json = "";
            json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());

            if (json != "")
            {
                var queryktp = JsonConvert.DeserializeObject<List<NhapkhotpItem>>(json);
                //var queryktp = cTEntities.Database.SqlQuery<NhapkhotpItem>(sql, "").ToList();
                //Kehoach
                sql = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                            declare @DateBegin datetime='{1}'
                            declare @DateEnd datetime='{2}'
                          
                            declare @DateMonthBegin int=datepart(MM,@DateBegin)
                            declare @DateMonthEnd int=datepart(MM,@DateEnd)
                            declare @DienGiai nvarchar(100)=''
                           
                          
                                                   
                         select qry.MaSP,cast(SLNhap as float) as SLKH,cast(0 as float) as SLThucHien,case when ThanhTien=0 then SLNhap*isnull(art.Gia,0) else ThanhTien end as ThanhTien,'KeHoach' as DienGiai,TenMau,Ngay,Colorhex from 
							(select MaSP,sum(SLNhap) as SLNhap,sum(ThanhTien) as ThanhTien,TenMau,Colorhex,Ngay from
							(select qry.MaSP,qry.SLNhap,(qry.SLNhap*isnull(art.Gia,0)) as ThanhTien,isnull(mm.TenMau,'') as TenMau,isnull(Colorhex,'#FFFFFF') as Colorhex,Ngay from
                            (
                            SELECT [MaSP],sum([SoLuong]) as SLNhap,ArticleNumber,dbo.GetFisrtDayOfMonth(Ngay) as Ngay
                              FROM [KeHoachNgayItem]
                              where datepart(MM,Ngay)>=@DateMonthBegin and datepart(MM,Ngay)<=@DateMonthEnd  and SerialKHNgay 
							  in (select STT from [KeHoachNgay]  where KhuVuc='KhoTP' and NhaMay=@NhaMay)
                             and Ngay>=@DateBegin and Ngay<=@DateEnd group by MaSP,ArticleNumber,dbo.GetFisrtDayOfMonth(Ngay)
                            ) as qry
							left join ArticleNumberProduct art  on qry.ArticleNumber=art.ArticleNumber
							left  join dbo.MaMau mm on art.MaMau=mm.MaMau) as qrytotal
							group by MaSP,TenMau,Ngay,Colorhex) as qry
							left join (select MaSP,Avg(Gia) as Gia from ArticleNumberProduct where Gia>0 group by MaSP)
							art on qry.MaSP=art.MaSP",
                            nhamayselected.Name, dtpbegin.Value.ToString("MM/dd/yyyy 00:00"), dtpend.Value.ToString("MM/dd/yyyy 23:59"), sqlreplace);


                json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
                var querykhitem = JsonConvert.DeserializeObject<List<NhapkhotpItem>>(json);

                //var querykhitem = cTEntities.Database.SqlQuery<NhapkhotpItem>(sql, "").ToList();

                var querykh = querykhitem.GroupBy(p => new { MaSP = p.MaSP, TenMau = p.TenMau, Colorhex = p.Colorhex }).Select(p => new NhapkhotpItem { MaSP = p.Key.MaSP, Colorhex = p.Key.Colorhex, TenMau = p.Key.TenMau, SLKH = p.Sum(n => n.SLKH), SLThucHien = 0, ThanhTien = p.Sum(n => n.ThanhTien) }).ToList();
                dtbangke.Clear();
                dtbangke.Columns.Clear();
                if (lstsokhoi.Count == 0)
                {
                    sql = @"select sp.MaSP,sp.TenSP,qry.SoKhoi from 
                            (select MaSP,sum(SoLuongCT*ChieuDayTC*ChieuRongTC*ChieuDaiTC)/1000000000 as SoKhoi
											                            from dbo.ChiTietSP where MaChiTiet 
											                            in (select MaCT from ChiTiet_KhuVuc where KhuVuc='KV1X') 
																	
																		group by MaSP) as qry
																		inner join dbo.SanPham sp on qry.MaSP=sp.MaSP";

                    json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
                    var querysk = JsonConvert.DeserializeObject<List<NhapkhotpItem>>(json);
                    // var querysk = cTEntities.Database.SqlQuery<NhapkhotpItem>(sql, "").ToList();
                    lstsokhoi.AddRange(querysk);
                }

                // var querygroup = queryktp.GroupBy(p => new { MaSP = p.MaSP }).Select(p => new { MaSP = p.Key.MaSP }).ToList(); ;
                var querycolumn = queryktp.GroupBy(p => p.Ngay).Select(p => new { Ngay = p.Key.ToString("yyyy-MM-dd") }).OrderBy(p => p.Ngay).ToList();
                var columnkehoach = querykhitem.GroupBy(p => p.Ngay).Select(p => new { Ngay = p.Key.ToString("yyyy-MM-dd"), ThanhTien = p.Sum(n => n.ThanhTien) }).OrderBy(p => p.Ngay).ToList();
                //var querysum = query.GroupBy(p => new { Diengiai = p.DienGiai }).Select(p => new { DienGiai = p.Key.Diengiai, SoKhoi = p.Sum(n => n.SoKhoi * n.SLNhap) }).ToList();
                //Xử lý phần đơn hàng
                sql = string.Format(@" declare @dateend date='{0}'
 
                       select MaSP,TenMau,case when SLDH<SLDHThucNhan then SLDHThucNhan else SLDH end as SLDH,SLDaXuat,case when SLDH<SLDHThucNhan then SLDHThucNhan-SLDaXuat else SLDH-SLDaXuat end as SLConLai,SLTonKho  from 
                        (select art.MaSP,mm.TenMau,sum(qry.SLDonHang) as SLDH,sum(qry.SLDHThucNhan) as SLDHThucNhan,sum(SLDaXuat) as SLDaXuat,sum(SLTonKho) as SLTonKho
                         from (select ArticleNumber,qry.SoLuong as SLDonHang,0 as SLDHThucNhan,0 as SLDaXuat,0 as SLTonKho
                        from (SELECT [MaSP],sum([SoLuong]) as SoLuong,ArticleNumber FROM [DonHangMua]   group by [MaSP],ArticleNumber) as qry
                                             union all
                                            SELECT ArticleNumber,0 as SLDonHang,sum(SLDonHang) as SLDHThucNhan,sum(SLDaXuat) as SLDaXuat,0 as SLTonKho
                                            FROM [KeHoachXuatHang]
                                            where NgayXuat<@dateend group by ArticleNumber
                                            union all
                                            select ArticleNumberID,0 as SLDonHang,0 as SLDHThucNhan,0 as SLDaXuat,SLTon as SLTonKho
                                            from dbo.KhoTP_TonKhoDenNgay(@dateend))
                                            as qry   inner join dbo.ArticleNumberProduct art on qry.ArticleNumber= art.ArticleNumber
                                            inner join dbo.MaMau mm on art.MaMau=mm.MaMau
                                            group by art.MaSP,mm.TenMau
					                        ) as qryall", dtpbegin.Value.ToString("MM/dd/yyyy 00:00"));

                List<DonHangItemDenNgay> lstdonhang = new List<DonHangItemDenNgay>();

                json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
                var querydh = JsonConvert.DeserializeObject<List<DonHangItemDenNgay>>(json);

                // var querydh = cTEntities.Database.SqlQuery<DonHangItemDenNgay>(sql, "").ToList();

                lstdonhang.AddRange(querydh);

                dtbangke.Columns.Add("MaSP", typeof(string));
                dtbangke.Columns.Add("TenSP", typeof(string));
                dtbangke.Columns.Add("SoKhoi", typeof(double));
                dtbangke.Columns.Add("TenMau", typeof(string));
                dtbangke.Columns.Add("Colorhex", typeof(string));
                dtbangke.Columns.Add("VisibleChart", typeof(bool));
                dtbangke.Columns["VisibleChart"].DefaultValue = false;
                // lstcolumn.Add(new InitDxGridDataColumn(0, "TieuDe", "Tiêu đề",null,250));
                //GridControlBand gridControlBandMaster = addGridControlBand("TieuDe", "");
                //gridControlBandMaster.OverlayHeaderByChildren = true;
                lstcolumn.Add(new InitDxGridDataColumn(0, "MaSP", "Mã SP", null, 120));
                lstcolumn.Add(new InitDxGridDataColumn(0, "TenSP", "Tên SP", null, 240));
                lstcolumn.Add(new InitDxGridDataColumn(0, "TenMau", "Tên màu", null, 120));
                lstcolumn.Add(new InitDxGridDataColumn(0, "SoKhoi", "Số khối/ SP", "#,0.####", 120));

                titlechart = string.Format("Tổng đơn hàng chưa xuất cuối ngày {0}", dtpbegin.Value.AddDays(-1).ToString("dd-MM-yy"));
                titlecharttonkho= string.Format("Tồn kho cuối ngày {0}", dtpbegin.Value.AddDays(-1).ToString("dd-MM-yy"));
                titlechartthuchien = string.Format("Số lượng kế hoạch tháng", dtpbegin.Value.ToString("MM"));
                //lstcolumn.Add(new InitDxGridDataColumn(0, "DonHang", string.Format("Số liệu chốt cuối ngày {0}", dtpbegin.Value.AddDays(-1).ToString("dd-MM-yy"), null, 300)));


                DateTime dtcheck = new DateTime();
                List<DateTime> lstdate = new List<DateTime>();
                foreach (var it in querycolumn)
                {
                    DateTime.TryParseExact(it.Ngay, "yyyy-MM-dd", CultureInfo.CurrentCulture,
                                                      DateTimeStyles.None, out dtcheck);
                    lstdate.Add(dtcheck);
                    dtbangke.Columns.Add("KH-" + it.Ngay, typeof(double));
                    dtbangke.Columns.Add("TH-" + it.Ngay, typeof(double));
                    dtbangke.Columns.Add("DatKH-" + it.Ngay, typeof(double));
                    dtbangke.Columns.Add("TyLe-" + it.Ngay, typeof(double));

                    lstcolumn.Add(new InitDxGridDataColumn(0, "KH-" + it.Ngay, "Kế hoạch T" + dtcheck.Month.ToString(),"#,#",110));
                    lstcolumn.Add(new InitDxGridDataColumn(0, "TH-" + it.Ngay, "Thực hiện T" + dtcheck.Month.ToString(), "#,#", 110));
                    lstcolumn.Add(new InitDxGridDataColumn(0, "DatKH-" + it.Ngay, "Đạt kế hoạch T" + dtcheck.Month.ToString(), "#,#", 110));
                    lstcolumn.Add(new InitDxGridDataColumn(0, "TyLe-" + it.Ngay, "%Hoàn thành T" + dtcheck.Month.ToString(), "#,#", 110));
                    lstcolumn.Add(new InitDxGridDataColumn(0, "TH-" + it.Ngay, "Thực hiện T" + dtcheck.Month.ToString(), "#,#", 110));



                }
                bool check = false;
                foreach (var it in columnkehoach)
                {
                    check = false;
                    foreach (var itkhotp in querycolumn)
                    {
                        if (it.Ngay == itkhotp.Ngay)
                        {
                            check = true;
                            break;
                        }

                    }
                    if (!check)
                    {
                        DateTime.TryParseExact(it.Ngay, "yyyy-MM-dd", CultureInfo.CurrentCulture,
                                                      DateTimeStyles.None, out dtcheck);
                        lstdate.Add(dtcheck);
                        dtbangke.Columns.Add("KH-" + it.Ngay, typeof(double));
                        dtbangke.Columns.Add("TH-" + it.Ngay, typeof(double));
                        dtbangke.Columns.Add("DatKH-" + it.Ngay, typeof(double));
                        dtbangke.Columns.Add("TyLe-" + it.Ngay, typeof(double));

                        lstcolumn.Add(new InitDxGridDataColumn(0, "KH-" + it.Ngay, "Kế hoạch T" + dtcheck.Month.ToString(), "#,#", 110));
                        lstcolumn.Add(new InitDxGridDataColumn(0, "TH-" + it.Ngay, "Thực hiện T" + dtcheck.Month.ToString(), "#,#", 110));
                        lstcolumn.Add(new InitDxGridDataColumn(0, "DatKH-" + it.Ngay, "Đạt kế hoạch T" + dtcheck.Month.ToString(), "#,#", 110));
                        lstcolumn.Add(new InitDxGridDataColumn(0, "TyLe-" + it.Ngay, "%Hoàn thành T" + dtcheck.Month.ToString(), "#,#", 110));
                        lstcolumn.Add(new InitDxGridDataColumn(0, "TH-" + it.Ngay, "Thực hiện T" + dtcheck.Month.ToString(), "#,#", 110));
                    }
                }

                dtbangke.Columns.Add("SLKH", typeof(double));
                dtbangke.Columns.Add("SLThucHien", typeof(double));
                dtbangke.Columns.Add("SLDatKH", typeof(double));
                dtbangke.Columns.Add("TyLe", typeof(double));
                //lstcolumn.Add(new InitDxGridDataColumn(0, "Total", "Tổng", null, 120));
                lstcolumn.Add(new InitDxGridDataColumn(0, "SLThucHien", "Tổng nhập kho", null, 120));
                lstcolumn.Add(new InitDxGridDataColumn(0, "SLKH", "Tổng K/hoạch", null, 120));
                lstcolumn.Add(new InitDxGridDataColumn(0, "SLDatKH", "Đạt kế hoạch", null, 120));
                //lstcolumn.Add(new InitDxGridDataColumn(0, "TyLe", "% Thực hiện", null, 120));

                dtbangke.Columns.Add("SLDHConLai", typeof(double));
                dtbangke.Columns.Add("SLTonKho", typeof(double));
                dtbangke.Columns.Add("wSLDHConLai", typeof(double));
                dtbangke.Columns.Add("wSLTonKho", typeof(double));
                dtbangke.Columns.Add("wSLKH", typeof(double));

                dtbangke.Columns.Add("chart2", typeof(double));
                dtbangke.Columns.Add("wchart2", typeof(double));
               
                dtbangke.Columns["wSLDHConLai"].DefaultValue = 0;
                dtbangke.Columns["wSLTonKho"].DefaultValue = 0;
                dtbangke.Columns["wSLKH"].DefaultValue = 0;

                dtbangke.Columns["chart2"].DefaultValue = 0;
                dtbangke.Columns["wchart2"].DefaultValue = 0;

                //Xử lý phần màu trước
                foreach (var itkehoach in querykh)
                {
                    if (itkehoach.TenMau != "")
                    {
                        foreach (var nk in queryktp)
                        {
                            if (itkehoach.MaSP == nk.MaSP)
                            {
                                if (itkehoach.TenMau == nk.TenMau)
                                {
                                    //DataRow dataRow = new DataRow();
                                    itkehoach.SLThucHien += nk.SLConLai;
                                    nk.DienGiai = "Mau";//Đánh dấu lại những dòng có màu khi đã đổ vào kế hoach
                                    nk.SLConLai = 0;
                                }
                            }
                        }
                    }
                }
                //Xử lý đến không màu
                foreach (var itkehoach in querykh)//Cộng tất cả số  dư không phân biệt màu vào
                {
                    if (itkehoach.TenMau == "")
                    {
                        foreach (var nk in queryktp)
                        {
                            if (itkehoach.MaSP == nk.MaSP)
                            {
                                //DataRow dataRow = new DataRow();
                                itkehoach.SLThucHien += nk.SLConLai;
                                nk.SLConLai = 0;
                            }
                        }
                    }
                }
                //Xử lý đến đám mà không có kế hoạch nhưng vẫn phải vét vào
                var querydamconlai = queryktp.Where(p => p.SLConLai > 0).GroupBy(p => new { MaSP = p.MaSP }).Select(p => new NhapkhotpItem { MaSP = p.Key.MaSP, SLKH = 0, SLThucHien = p.Sum(n => n.SLConLai), TenMau = "" }).ToList();
                querykh.AddRange(querydamconlai);
                foreach (var it in querykh)
                {
                    foreach (var sk in lstsokhoi)
                    {
                        if (it.MaSP == sk.MaSP)
                        {
                            it.SoKhoi = sk.SoKhoi;
                            it.TenSP = sk.TenSP;
                            break;
                        }
                    }
                    DataRow row = dtbangke.NewRow();

                    row["MaSP"] = it.MaSP;
                    row["TenSP"] = it.TenSP;
                    row["SoKhoi"] = it.SoKhoi;
                    row["SLThucHien"] = it.SLThucHien;
                    row["SLKH"] = it.SLKH;
                    row["SLDatKH"] = (it.SLThucHien > it.SLKH) ? it.SLKH : it.SLThucHien;
                    row["TyLe"] = (it.SLKH == 0) ? 0 : (double.Parse(row["SLDatKH"].ToString()) / it.SLKH);
                    row["TenMau"] = it.TenMau;
                    row["Colorhex"] = it.Colorhex;
                    dtbangke.Rows.Add(row);
                }
                foreach (var it in queryktp)
                {
                    foreach (var sk in lstsokhoi)
                    {
                        if (it.MaSP == sk.MaSP)
                        {
                            it.SoKhoi = sk.SoKhoi;
                            it.TenSP = sk.TenSP;
                            break;
                        }
                    }
                }
                //Xử lý đám có màu trước
                var querycomau = queryktp.Where(p => p.DienGiai == "Mau").ToList();
                foreach (var it in querycomau)
                {
                    foreach (DataRow rowbk in dtbangke.Rows)
                    {
                        if (it.MaSP == rowbk["MaSP"].ToString())
                        {
                            if (it.TenMau == rowbk["TenMau"].ToString())
                            {
                                if (rowbk["TH-" + it.Ngay.ToString("yyyy-MM-dd")] == DBNull.Value)
                                    rowbk["TH-" + it.Ngay.ToString("yyyy-MM-dd")] = it.SLNhap;
                                else
                                    rowbk["TH-" + it.Ngay.ToString("yyyy-MM-dd")] = rowbk.Field<double>("TH-" + it.Ngay.ToString("yyyy-MM-dd")) + it.SLNhap;
                                //break;
                            }
                        }
                    }
                }
                //Xử lý đám không màu
                var querykhongmau = queryktp.Where(p => p.DienGiai != "Mau").ToList();
                {
                    foreach (var it in querykhongmau)
                    {
                        foreach (DataRow rowbk in dtbangke.Rows)
                        {
                            if (it.MaSP == rowbk["MaSP"].ToString())
                            {
                                if (rowbk["TH-" + it.Ngay.ToString("yyyy-MM-dd")] == DBNull.Value)
                                    rowbk["TH-" + it.Ngay.ToString("yyyy-MM-dd")] = it.SLNhap;
                                else
                                    rowbk["TH-" + it.Ngay.ToString("yyyy-MM-dd")] = rowbk.Field<double>("TH-" + it.Ngay.ToString("yyyy-MM-dd")) + it.SLNhap;
                                //break;
                            }
                        }
                    }
                }
                //Xử lý đám kế hoạch item
                foreach (var it in querykhitem)
                {
                    foreach (DataRow row in dtbangke.Rows)
                    {
                        if (row["MaSP"].ToString() == it.MaSP && row["TenMau"].ToString() == it.TenMau)
                        {
                            row["KH-" + it.Ngay.ToString("yyyy-MM-dd")] = it.SLKH;
                            if (it.SLKH > 0)
                            {
                                if (row["TH-" + it.Ngay.ToString("yyyy-MM-dd")] != DBNull.Value)
                                {
                                    double dth = double.Parse(row["TH-" + it.Ngay.ToString("yyyy-MM-dd")].ToString());
                                    row["DatKH-" + it.Ngay.ToString("yyyy-MM-dd")] = (dth > it.SLKH) ? it.SLKH : dth;
                                    row["TyLe-" + it.Ngay.ToString("yyyy-MM-dd")] = dth / it.SLKH;
                                }
                            }
                            break;
                        }
                    }
                }

                //var querygroupsumktp = queryktp.GroupBy(p => new { Ngay = p.Ngay }).Select(p => new { Ngay = p.Key.Ngay.ToString("yyyy-MM-dd"), SoKhoiKH = p.Sum(n => n.SoKhoi * n.SLKH), SKThucHien = p.Sum(n => n.SLNhap * n.SoKhoi), ThanhTienTH = p.Sum(n => n.ThanhTien), ThanhTienKH = p.Sum(n => (n.SLThucHien == 0) ? 0 : ((n.ThanhTien / n.SLThucHien) * n.SLKH)) }).ToList();
                //var querygrouptotal = querykh.GroupBy(p => 1).Select(p => new { TongKH = p.Sum(n => n.SLKH * n.SoKhoi), TongThucHien = p.Sum(n => n.SLThucHien * n.SoKhoi), SKDatKH = p.Sum(n => ((n.SLThucHien > n.SLKH) ? n.SLKH : n.SLThucHien) * n.SoKhoi), ThanhTienKH = p.Sum(n => n.ThanhTien) }).ToList();
                // ,SKTHKH = p.Sum(n => ((n.SLThucHien > n.SLKH) ? n.SLKH : n.SLThucHien) * n.SoKhoi)
                var querygroupsumktp = queryktp.GroupBy(p => new { Ngay = p.Ngay }).Select(p => new { Ngay = p.Key.Ngay.ToString("yyyy-MM-dd"), SoKhoiKH = p.Sum(n => n.SoKhoi * n.SLKH), SKThucHien = p.Sum(n => n.SLNhap * n.SoKhoi), ThanhTienTH = p.Sum(n => n.ThanhTien), ThanhTienKH = p.Sum(n => (n.SLThucHien == 0) ? 0 : ((n.ThanhTien / n.SLThucHien) * n.SLKH)) }).ToList();
                //var querygroupsumkh = querykhitem.GroupBy(p => new { Ngay = p.Ngay }).Select(p => new { Ngay = p.Key.Ngay.ToString("yyyy-MM-dd"), SoKhoiKH = p.Sum(n => n.SoKhoi * n.SLKH)}).ToList();
                //var querykhitemtotal = querykhitem.GroupBy(p => new { Ngay = p.Ngay }).Select(p => new { Ngay = p.Key.Ngay.ToString("yyyy-MM-dd") });
                var querygrouptotal = querykh.GroupBy(p => 1).Select(p => new { TongKH = p.Sum(n => n.SLKH * n.SoKhoi), TongThucHien = p.Sum(n => n.SLThucHien * n.SoKhoi), SKDatKH = p.Sum(n => ((n.SLThucHien > n.SLKH) ? n.SLKH : n.SLThucHien) * n.SoKhoi), ThanhTienKH = p.Sum(n => n.ThanhTien) }).ToList();

                DataRow rowtotal = dtbangke.NewRow();
                DataRow rowthanhtien = dtbangke.NewRow();
                double thanhtienth = 0;
                rowtotal["MaSP"] = "---";
                rowtotal["TenSP"] = "Tổng khối lượng";
                rowthanhtien["MaSP"] = "---";
                rowthanhtien["TenSP"] = "Thành tiền (USD)";
                double dskkh = 0, dskthuchien = 0;
                foreach (var it in querygroupsumktp)
                {
                    rowtotal["TH-" + it.Ngay] = it.SKThucHien;
                    rowthanhtien["TH-" + it.Ngay] = it.ThanhTienTH;


                    dskkh += it.SoKhoiKH;
                    // dskthkh += it.SKTHKH;
                    dskthuchien += it.SKThucHien;
                    thanhtienth += it.ThanhTienTH;
                }
                foreach (var ittotalkh in columnkehoach)
                {

                    rowthanhtien["KH-" + ittotalkh.Ngay] = ittotalkh.ThanhTien;


                }
                //foreach(var it in querygroupsumkh)
                //{
                //    rowtotal["KH-" + it.Ngay] = it.SoKhoiKH;

                //}
                dskkh = 0;
                dskthuchien = 0;
                double skdatkh = 0;
                //Xử lý đạt kế hoạch
                foreach (var it in lstdate)
                {
                    dskkh = 0;
                    skdatkh = 0;
                    foreach (DataRow row in dtbangke.Rows)
                    {
                        if (row["KH-" + it.ToString("yyyy-MM-dd")] != DBNull.Value)
                        {
                            if (double.TryParse(row["KH-" + it.ToString("yyyy-MM-dd")].ToString(), out dskthuchien))
                            {
                                dskkh += dskthuchien * double.Parse(row["SoKhoi"].ToString());
                            }
                        }
                        if (row["DatKH-" + it.ToString("yyyy-MM-dd")] != DBNull.Value)
                        {
                            if (double.TryParse(row["DatKH-" + it.ToString("yyyy-MM-dd")].ToString(), out dskthuchien))
                            {
                                skdatkh += dskthuchien * double.Parse(row["SoKhoi"].ToString());
                            }
                        }

                    }
                    rowtotal["KH-" + it.ToString("yyyy-MM-dd")] = dskkh;
                    rowtotal["DatKH-" + it.ToString("yyyy-MM-dd")] = skdatkh;
                    if (rowtotal["KH-" + it.ToString("yyyy-MM-dd")] != DBNull.Value)
                    {
                        rowtotal["TyLe-" + it.ToString("yyyy-MM-dd")] = skdatkh / dskkh;
                    }
                }

                rowtotal["SLKH"] = querygrouptotal[0].TongKH;
                rowtotal["SLThucHien"] = querygrouptotal[0].TongThucHien;
                rowtotal["SLDatKH"] = querygrouptotal[0].SKDatKH;
                rowtotal["TyLe"] = (querygrouptotal[0].TongKH == 0) ? 0 : (querygrouptotal[0].SKDatKH / querygrouptotal[0].TongKH);
                rowthanhtien["SLKH"] = querygrouptotal[0].ThanhTienKH; ;
                rowthanhtien["SLThucHien"] = thanhtienth;

                //rowtotal["TyLe"] =(dskkh==0)?0:(dskthkh / dskthkh);
                //rowtotal["SLKH"] = dskkh;
                //rowtotal["SLThucHien"] = dskthuchien;
                dtbangke.Rows.Add(rowtotal);
                dtbangke.Rows.Add(rowthanhtien);
                double dpixel = 0;
                double totalwidth = 400;
                //Xử lý đơn hàng
                foreach (DataRow row in dtbangke.Rows)
                {
                    foreach (var it in lstdonhang)
                    {
                        if (row.Field<string>("MaSP") == it.MaSP && row.Field<string>("TenMau") == it.TenMau)
                        {
                            row["SLDHConLai"] = it.SLConLai;
                            row["SLTonKho"] = it.SLTonKho;
                            if (it.SLConLai <= 0)
                                row["wSLDHConLai"] = 0;
                            else
                                row["wSLDHConLai"] = totalwidth;


                            if (it.SLConLai <= 0)
                                dpixel = 1;
                            else
                                dpixel = totalwidth / it.SLConLai.Value;

                            row["wSLTonKho"] = dpixel * it.SLTonKho;
                            if (row.Field<double>("wSLTonKho") > totalwidth)
                                row["wSLTonKho"] = totalwidth;
                            if (row.Field<double>("wSLTonKho") < 40 && row.Field<double>("wSLTonKho") > 0)
                                row["wSLTonKho"] = 40;
                            row["wSLKH"] = dpixel * row.Field<double>("SLKH");
                            if (row.Field<double>("wSLKH") > totalwidth)
                                row["wSLKH"] = totalwidth;
                            if (row.Field<double>("wSLKH") < 40 && row.Field<double>("wSLKH") > 0)
                                row["wSLKH"] = 40;
                            it.SLConLai = 0;
                            it.SLTonKho = 0;

                            row["VisibleChart"] = true;
                            row["wSLTonKho"] = Math.Round(row.Field<double>("wSLTonKho") / totalwidth * 100, 0);
                            row["wSLKH"] = Math.Round(row.Field<double>("wSLKH") / totalwidth * 100, 0);
                         

                            row["wchart2"] = row["wSLKH"];
                            row["chart2"] = row["SLKH"];

                            break;
                        }
                    }
                }

                List<QuyDoiNgay> lstquydoi = new List<QuyDoiNgay>();



                lstquydoi = prs.getDayWeekMonthYear(lstdate, "M");

                //foreach (var it in lstquydoi)
                //{
                //    foreach (GridControlBand cl in grvSanPham.Bands)
                //    {
                //        if (it.Ngay.ToString("yyyy-MM-dd") == cl.Tag.ToString())
                //        {
                //            cl.Header = "Tháng " + it.Ngay.Month.ToString();
                //            break;
                //        }
                //    }
                //}




                lstquydoi.Clear();

                querygroupsumktp.Clear();
                //prs.ExportExcel_HeaderwithNote(dtbangke, Model.ModelAdmin.pathexcelexport, 2, 1, "AAAA", 1);
                querycolumn.Clear();
                querykhongmau.Clear();
                querydamconlai.Clear();
                querygrouptotal.Clear();
                querygroupsumktp.Clear();

                querykh.Clear();
                queryktp.Clear();
            }
            return dtbangke;
        }
        private async Task<DataTable> NangSuatTuNgayDenNgay(string loaibaocao)
        {
            visiblechart = false;
            DataTable dtbangke = new DataTable();
            //grvSanPham.ClearGrouping();
            string sqlreplace = "";
            if (loaibaocao == "Ngay")
            {
                sqlreplace = "cast(Ngay as Date)";
            }
            if (loaibaocao == "Tuan")
            {
                sqlreplace = "cast(dbo.GetFisrtDayOfWeek(Ngay) as Date)";
            }
            if (loaibaocao == "Thang")
            {
                sqlreplace = "cast(dbo.GetFisrtDayOfMonth(Ngay) as Date)";
            }
            if (loaibaocao == "Quy")
            {
                sqlreplace = "cast(dbo.GetFisrtDayOfQuarter(Ngay) as Date)";
            }
            if (loaibaocao == "Nam")
            {
                sqlreplace = "cast(dbo.GetFisrtDayOfYear(Ngay) as Date)";
            }
            string dieukiennhamay = "";
            if (nhamayselected != null)
            {
                if (nhamayselected.Name != "All")
                    dieukiennhamay += " and NhaMay=@NhaMay";
            }
            string sql = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                            declare @DateBegin datetime='{1}'
                            declare @DateEnd datetime='{2}'
                            declare @LyDo nvarchar(100)=N'Hàng mới'
                            declare @DateMonthBegin int=datepart(MM,@DateBegin)
                            declare @DateMonthEnd int=datepart(MM,@DateEnd)
                            declare @DienGiai nvarchar(100)=''
                           
                           select N'- Phôi giao sản xuất' as DienGiai, sum(isnull(pldm.DinhMucSuDung,1)*SoKhoi) as SoKhoi,{3} as Ngay,NhaMay  from
										  (SELECT sum(SoKhoiXuat) as SoKhoi,PhanLoai,ctnhap.Ngay,NhaMay
										FROM    KhoGo_Item it 
										inner join (select * from dbo.ChungTuNhap where Ngay>=@DateBegin and Ngay<=@DateEnd and LyDo=N'Giao sản xuất' {4})
										ctnhap on it.MaCT=ctnhap.MaCT
										group by PhanLoai,ctnhap.NhaMay,ctnhap.Ngay) as qry 
										left join dbo.PhanLoaiDinhMuc pldm on qry.PhanLoai=pldm.PhanLoai
										group by  {3},NhaMay
                                        
                                        union all

                                        select qry.DienGiai,sum(qry.SoLuong*ctsp.SoKhoiTC) as SoKhoi, Ngay as Ngay,NhaMay 
                                        from
                                        (
                                            SELECT case when SLNhap!=0 then N'1. Nhập kho tinh chế' else N'2. Xuất kho tinh chế' end as DienGiai ,[MaCT],sum(SLNhap+SLXuat) as SoLuong,{3} as Ngay,NhaMay   
                                          FROM [KV1_KTG]  where Ngay>=@DateBegin and Ngay<=@DateEnd and LyDo=@LyDo {4} 
										  group by MaCT,{3}, case when SLNhap!=0 then N'1. Nhập kho tinh chế' else N'2. Xuất kho tinh chế' end,NhaMay
											
										union all
                                          
										  SELECT N'3. Xuống phiếu' as DienGiai, [MaChiTiet],sum([SLXP]) as SoLuong,{3} as Ngay,NhaMay
                                          FROM [KV2_LXP]  where Ngay>=@DateBegin and Ngay<=@DateEnd {4}
										  group by [MaChiTiet],{3},NhaMay
                                          union all

                                          SELECT case when SLNhap!=0 then N'4. Nhập kho chờ ráp' else N'5. Xuất kho chờ ráp' end as DienGiai,MaCT,sum([SLNhap]+SLXuat) as SoLuong,{3} as Ngay,NhaMay    
                                          FROM [KV3_KhoChoRap]  where Ngay>=@DateBegin and Ngay<=@DateEnd and LyDo=@LyDo {4}
										  group by MaCT,{3},case when SLNhap!=0 then N'4. Nhập kho chờ ráp' else N'5. Xuất kho chờ ráp' end,NhaMay
                                          
										  union all

                                          SELECT case when SLNhap!=0 then N'6. Nhập kho nhúng ' else  N'7. Xuất kho nhúng ' end as DienGiai,[MaCT],sum([SLNhap]+SLXuat) as SoLuong,{3} as Ngay,NhaMay
                                          FROM KV4_KhoNhung  where Ngay>=@DateBegin and Ngay<=@DateEnd and LyDo=@LyDo {4}
										  group by MaCT,{3}, case when SLNhap!=0 then N'6. Nhập kho nhúng ' else  N'7. Xuất kho nhúng ' end,NhaMay
											
											) as qry
                                          inner join (select MaChiTiet,ChieuDayTC*ChieuRongTC*ChieuDaiTC/1000000000 as SoKhoiTC from dbo.Load_CTSP)
                                          ctsp on qry.MaCT=ctsp.MaChiTiet 
										  group by Ngay, qry.DienGiai,NhaMay 
										  union all
										  select N'8. Nhập kho thành phẩm' as DienGiai,sum(SLNhap*qrySoKhoiSP.SoKhoi)/1000000000 as SoKhoi,Ngay as Ngay,NhaMay from 
                                            (SELECT MaSP,sum([SLNhap]) as SLNhap,{3} as Ngay,NhaMay
											FROM [KhoTP_NK] 
											where Ngay>=@DateBegin and Ngay<=@DateEnd  
											and LyDo=N'Nhập mới' {4}
											group by MaSP,{3},NhaMay)
											as qryktp inner join (select MaSP,sum(SoLuongCT*ChieuDayTC*ChieuRongTC*ChieuDaiTC) as SoKhoi
											from dbo.ChiTietSP where MaChiTiet 
											in (select MaCT from ChiTiet_KhuVuc where KhuVuc='KV1X') group by MaSP) as qrySoKhoiSP
											on qryktp.MaSP=qrySoKhoiSP.MaSP
											group by Ngay,NhaMay
                            ", (nhamayselected == null) ? "" : nhamayselected.Name.ToString(), dtpbegin.Value.ToString("MM/dd/yyyy 00:00"), dtpend.Value.ToString("MM/dd/yyyy 23:59"), sqlreplace, dieukiennhamay);

            CallAPI callAPI = new CallAPI();
            string json = "";
            json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());

            if (json != "")
            {
                var querynangsuat = JsonConvert.DeserializeObject<List<NhapkhotpItem>>(json);
                //var querynangsuat = cTEntities.Database.SqlQuery<NhapkhotpItem>(sql, "").ToList();
                //Kehoach
                sql = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                            declare @DateBegin datetime='{1}'
                            declare @DateEnd datetime='{2}'
                            declare @LyDo nvarchar(100)=N'Hàng mới'
                            declare @DateMonthBegin int=datepart(MM,@DateBegin)
                            declare @DateMonthEnd int=datepart(MM,@DateEnd)
                            declare @DienGiai nvarchar(100)=''	
                            select qrykh.NhaMay,sum(qrykh.SLNhap*qrysokhoi.SoKhoi) as SoKhoi from

                            (SELECT [MaSP],sum([SoLuong]) as SLNhap,khngay.NhaMay
                              FROM 
							 (select * from [KeHoachNgayItem] where Ngay>=@DateBegin and Ngay<=@DateEnd) as khitem inner join dbo.KeHoachNgay khngay on (khitem.SerialKHNgay=khngay.STT and khngay.KhuVuc='KhoTP')
                              where datepart(MM,khitem.Ngay) in (@DateMonthBegin,@DateMonthEnd) 
                              group by MaSP,NhaMay) as qrykh
							  inner join  (select MaSP,sum(ChieuDaiTC*ChieuRongTC*ChieuDayTC*SoLuongCT)/1000000000 as SoKhoi
							  from ChiTietSP ctsp where MaChiTiet in (select MaCT from ChiTiet_KhuVuc where KhuVuc='KV1X')  group by MaSP) as qrysokhoi
							  on qrykh.MaSP=qrysokhoi.MaSP
							  group by qrykh.NhaMay"
                            , (nhamayselected == null) ? "" : nhamayselected.Name.ToString(), dtpbegin.Value.ToString("MM/dd/yyyy 00:00"), dtpend.Value.ToString("MM/dd/yyyy 23:59"), sqlreplace, dieukiennhamay);
                // var querykh = cTEntities.Database.SqlQuery<NhapkhotpItem>(sql, "").ToList();
                json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
                var querykh = JsonConvert.DeserializeObject<List<NhapkhotpItem>>(json);

                sql = string.Format(@"declare @DateBegin datetime='{0}'
                      declare @DateEnd datetime='{1}' 
					  select NhaMay,cast(count(*) as float) as SLKH 
					  from (
					  select Ngay,NhaMay from
					  (
					  SELECT  [Ngay],NhaMay
                      FROM [KV1_KTG] where Ngay>=@DateBegin and Ngay<=@DateEnd  
                      group by Ngay,NhaMay
                      union all
                      select Ngay,NhaMay from KhoTP_NK  
                      where Ngay>=@DateBegin and Ngay<=@DateEnd  
                      group by Ngay,NhaMay) as qry group by NhaMay,Ngay) as qryall
                      group by NhaMay", dtpbegin.Value.ToString("MM/dd/yyyy 00:00"), dtpend.Value.ToString("MM/dd/yyyy 23:59"));
                //var queryngalv = cTEntities.Database.SqlQuery<NhapkhotpItem>(sql, "").ToList();
                json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
                var queryngalv = JsonConvert.DeserializeObject<List<NhapkhotpItem>>(json);




                // var querygroup = queryktp.GroupBy(p => new { MaSP = p.MaSP }).Select(p => new { MaSP = p.Key.MaSP }).ToList(); ;
                var querycolumn = querynangsuat.GroupBy(p => p.Ngay).Select(p => new { Ngay = p.Key.ToString("yyyy-MM-dd") }).OrderBy(p => p.Ngay).ToList();
                var querygrouprow = querynangsuat.GroupBy(p => new { DienGiai = p.DienGiai, NhaMay = p.NhaMay }).Select(p => new { DienGiai = p.Key.DienGiai, NhaMay = p.Key.NhaMay, SoKhoi = p.Sum(n => n.SoKhoi) }).OrderBy(p => p.DienGiai).OrderBy(p => p.NhaMay).ToList();
                //var querysum = query.GroupBy(p => new { Diengiai = p.DienGiai }).Select(p => new { DienGiai = p.Key.Diengiai, SoKhoi = p.Sum(n => n.SoKhoi * n.SLNhap) }).ToList();
                dtbangke.Columns.Add("DienGiai", typeof(string));
                dtbangke.Columns.Add("NhaMay", typeof(string));
                dtbangke.Columns.Add("VisibleChart", typeof(bool));
                dtbangke.Columns["VisibleChart"].DefaultValue = false;
                lstcolumn.Add(new InitDxGridDataColumn(0, "DienGiai", "Diễn giải",null,210));
                lstcolumn.Add(new InitDxGridDataColumn(0, "NhaMay", "Nhà máy"));
                //lstcolumn.Add(new InitDxGridDataColumn(0, "TyLe", "%Hoàn thành"));

                //grvSanPham.Columns["NhaMay"].GroupIndex = 0;
                // InitGrid("TyLe");
                DateTime dtcheck = new DateTime();
                List<DateTime> lstdate = new List<DateTime>();
                foreach (var it in querycolumn)
                {
                    dtbangke.Columns.Add(it.Ngay, typeof(double));
                    lstcolumn.Add(new InitDxGridDataColumn(0, it.Ngay, it.Ngay,"#,#",100));
                    if (DateTime.TryParseExact(it.Ngay, "yyyy-MM-dd", CultureInfo.CurrentCulture,
                                                     DateTimeStyles.None, out dtcheck))
                    {
                        lstdate.Add(dtcheck);
                    }
                }
                dtbangke.Columns.Add("Total", typeof(double));
                dtbangke.Columns.Add("TongNgay", typeof(double));
                dtbangke.Columns.Add("KeHoachThang", typeof(double));
                dtbangke.Columns.Add("ConLai", typeof(double));
                dtbangke.Columns.Add("TyLe", typeof(double));
                lstcolumn.Add(new InitDxGridDataColumn(0, "Total", "Tổng", "#,#", 100));
                lstcolumn.Add(new InitDxGridDataColumn(0, "TongNgay", "Ngày Làm việc", "#,#", 100));
                lstcolumn.Add(new InitDxGridDataColumn(0, "KeHoachThang", "Kế hoạch tháng", "#,#", 100));
                lstcolumn.Add(new InitDxGridDataColumn(0, "ConLai", "Phải làm", "#,#", 100));

                //grvSanPham.Columns.Add(addGridColumnPercent("TyLe", "% Thực hiện"));



                foreach (var it in querygrouprow)
                {
                    DataRow rownew = dtbangke.NewRow();
                    rownew["DienGiai"] = it.DienGiai;
                    //rownew["TongNgay"] = tongngay; ;
                    rownew["Total"] = it.SoKhoi;
                    rownew["NhaMay"] = it.NhaMay;
                    foreach (var itkh in querykh)
                    {
                        if (it.NhaMay == itkh.NhaMay)
                        {
                            rownew["KeHoachThang"] = itkh.SoKhoi;
                            rownew["ConLai"] = (itkh.SoKhoi < it.SoKhoi) ? 0 : (itkh.SoKhoi - it.SoKhoi);
                            rownew["TyLe"] = (it.SoKhoi > itkh.SoKhoi) ? 1 : (it.SoKhoi / itkh.SoKhoi);
                            break;
                        }
                    }
                    foreach (var itngay in queryngalv)
                    {
                        if (it.NhaMay == itngay.NhaMay)
                        {
                            rownew["TongNgay"] = itngay.SLKH;
                            break;
                        }
                    }
                    dtbangke.Rows.Add(rownew);
                }
                //Xử lý item
                foreach (var it in querynangsuat)
                {
                    foreach (DataRow row in dtbangke.Rows)
                    {
                        if (row["DienGiai"].ToString() == it.DienGiai && row["NhaMay"].ToString() == it.NhaMay)
                        {
                            row[it.Ngay.ToString("yyyy-MM-dd")] = it.SoKhoi;
                            break;
                        }
                    }
                }
                List<QuyDoiNgay> lstquydoi = new List<QuyDoiNgay>();
                if (goptheoselected.FullName == "Gộp theo ngày")
                {
                    foreach (var it in lstdate)
                    {
                        foreach (InitDxGridDataColumn cl in lstcolumn)
                        {
                            if (it.ToString("yyyy-MM-dd") == cl.FieldName)
                            {
                                //Console.WriteLine(cl.Header);
                                cl.Caption = it.ToString("dd/MM/yy");

                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (goptheoselected.FullName == "Gộp theo tuần")
                        lstquydoi = prs.getDayWeekMonthYear(lstdate, "W");
                    if (goptheoselected.FullName == "Gộp theo tháng")
                        lstquydoi = prs.getDayWeekMonthYear(lstdate, "M");
                    if (goptheoselected.FullName == "Gộp theo quý")
                        lstquydoi = prs.getDayWeekMonthYear(lstdate, "Q");
                    if (goptheoselected.FullName == "Gộp theo năm")
                        lstquydoi = prs.getDayWeekMonthYear(lstdate, "Y");
                    foreach (var it in lstquydoi)
                    {
                        foreach (InitDxGridDataColumn cl in lstcolumn)
                        {
                            if (it.Ngay.ToString("yyyy-MM-dd") == cl.FieldName)
                            {
                                cl.Caption = it.Ngayoutput;
                                break;
                            }
                        }
                    }
                }
                int Monthbegin = dtpbegin.Value.Month;
                int Monthend = dtpend.Value.Month;
                foreach (InitDxGridDataColumn it in lstcolumn)
                {
                    if (it.FieldName == "KeHoachThang")
                    {
                        if (Monthbegin - Monthend == 0)
                            it.Caption = string.Format("Kế hoạch tháng {0}", Monthbegin);
                        else
                        {
                            it.Caption = string.Format("Kế hoạch tháng {0} đến {1}", Monthbegin, Monthend);
                        }
                    }


                }
                foreach (InitDxGridDataColumn initDxGridDataColumn in lstcolumn)
                {
                    if (initDxGridDataColumn.FieldName == "NhaMay")
                    {
                        initDxGridDataColumn.GroupIndex = 0;
                        break;
                    }
                }

                querycolumn.Clear();
                querygrouprow.Clear();
                querynangsuat.Clear();
                querykh.Clear();

            }
            return dtbangke;


        }

        private async Task<DataTable> XuatKhoTuNgayDenNgay()
        {
            visiblechart = false;
            string sqlreplace = "";
            DataTable dtbangke = new DataTable();
            string dieukiennhamay = "";
            if (nhamayselected == null)
            {
                nhamayselected = txtnhamay.SelectedValue("All");
            }
            if (nhamayselected != null)
            {
                if (nhamayselected.Name != "All")
                    dieukiennhamay += " and NhaMay=@NhaMay";
            }

            string sql = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                            declare @DateBegin datetime='{1}'
                            declare @DateEnd datetime='{2}'
                            declare @LyDo nvarchar(100)=N'Xuất bán'
                            declare @DateMonthBegin int=datepart(MM,@DateBegin)
                            declare @DateMonthEnd int=datepart(MM,@DateEnd)
                            declare @DienGiai nvarchar(100)=''
                           
                            select sum(nk.SLNhap*qry.DonGia) as SLThucHien,qry.NhaMay,sp.KhachHang as DienGiai,qry.Ngay from
                            (SELECT [IDNumber],[DonGia],[ArticleNumberID],ct.NhaMay,dbo.GetFisrtDayOfMonth(ct.Ngay) as Ngay
                              FROM [KhoTP_XK] xk
                              inner join (SELECT  [MaCT],NhaMay,Ngay
                              FROM [KhoTP_ChungTu]
                              where LyDo=@LyDo and Ngay>=@DateBegin and Ngay<=@DateEnd {4}) as ct on xk.ChungTu=ct.MaCT) as qry
                              inner join dbo.KhoTP_NK nk on qry.IDNumber=nk.IDNumber

                              inner join dbo.SanPham sp on nk.MaSP=sp.MaSP
                              group by qry.NhaMay,sp.KhachHang,qry.Ngay
                            ", (nhamayselected == null) ? "" : nhamayselected.Name.ToString(), dtpbegin.Value.ToString("MM/dd/yyyy 00:00"), dtpend.Value.ToString("MM/dd/yyyy 23:59"), sqlreplace, dieukiennhamay);

            CallAPI callAPI = new CallAPI();
            string json = "";
            json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());

            if (json != "")
            {
                var querynangsuat = JsonConvert.DeserializeObject<List<NhapkhotpItem>>(json);
                if (querynangsuat.Count == 0)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, $"Không có dữ liệu, vui lòng kiểm tra lại thông tin tìm kiếm"));
                    return dtbangke;
                }
                //Kehoach
                // var querygroup = queryktp.GroupBy(p => new { MaSP = p.MaSP }).Select(p => new { MaSP = p.Key.MaSP }).ToList(); ;
                var querycolumn = querynangsuat.GroupBy(p => p.DienGiai).Select(p => new { DienGiai = p.Key.ToString() }).OrderBy(p => p.DienGiai).ToList();
                var querygrouprow = querynangsuat.GroupBy(p => new { NhaMay = p.NhaMay, Ngay = p.Ngay }).Select(p => new { NhaMay = p.Key.NhaMay, SLThucHien = p.Sum(n => n.SLThucHien), Ngay = p.Key.Ngay }).OrderBy(p => p.Ngay).OrderBy(p => p.NhaMay).ToList();
                //var querysum = query.GroupBy(p => new { Diengiai = p.DienGiai }).Select(p => new { DienGiai = p.Key.Diengiai, SoKhoi = p.Sum(n => n.SoKhoi * n.SLNhap) }).ToList();
                var querygrouptotal = querynangsuat.GroupBy(p => new { NhaMay = p.NhaMay, DienGiai = p.DienGiai }).Select(p => new { NhaMay = p.Key.NhaMay, DienGiai = p.Key.DienGiai, SLThucHien = p.Sum(n => n.SLThucHien) }).OrderBy(p => p.NhaMay).ToList();
                dtbangke.Columns.Add("Thang", typeof(string));
                dtbangke.Columns.Add("DVT", typeof(string));
                dtbangke.Columns.Add("NhaMay", typeof(string));
                dtbangke.Columns.Add("VisibleChart", typeof(bool));
                dtbangke.Columns["VisibleChart"].DefaultValue = false;
                dtbangke.Columns.Add("TyLe", typeof(double));
                dtbangke.Columns["TyLe"].DefaultValue = 0;
                lstcolumn.Add(new InitDxGridDataColumn(0, "Thang", "Tháng"));
                lstcolumn.Add(new InitDxGridDataColumn(0, "DVT", "ĐVT"));
                lstcolumn.Add(new InitDxGridDataColumn(0, "NhaMay", ""));

                List<DateTime> lstdate = new List<DateTime>();
                foreach (var it in querycolumn)
                {
                    dtbangke.Columns.Add(it.DienGiai, typeof(double));
                    lstcolumn.Add(new InitDxGridDataColumn(0, it.DienGiai, it.DienGiai));


                }
                dtbangke.Columns.Add("Total", typeof(double));
                lstcolumn.Add(new InitDxGridDataColumn(0, "Total", "Tổng Xuất", "#,#.#"));
                //grvSanPham.Columns.Add(addGridColumnPercent("TyLe", "% Thực hiện"));


                foreach (var it in querygrouprow)
                {
                    DataRow row = dtbangke.NewRow();
                    row["Thang"] = it.Ngay.Month;
                    row["NhaMay"] = it.NhaMay;
                    row["DVT"] = "USD";
                    row["Total"] = it.SLThucHien;
                    dtbangke.Rows.Add(row);
                }
                foreach (var it in querynangsuat)
                {
                    foreach (DataRow row in dtbangke.Rows)
                    {
                        if (it.Ngay.Month.ToString() == row["Thang"].ToString() && row["NhaMay"].ToString() == it.NhaMay)
                        {
                            row[it.DienGiai] = it.SLThucHien;
                            break;
                        }
                    }
                }
                var querynhamay = querygrouptotal.GroupBy(p => p.NhaMay).Select(p => new { NhaMay = p.Key.ToString(), SLThucHien = p.Sum(n => n.SLThucHien) }).ToList();
                foreach (var itnhamay in querynhamay)
                {
                    DataRow row = dtbangke.NewRow();
                    row["Total"] = itnhamay.SLThucHien;
                    foreach (var it in querygrouptotal)
                    {
                        if (itnhamay.NhaMay == it.NhaMay)
                        {
                            row["Thang"] = "Cộng";
                            row["NhaMay"] = it.NhaMay;
                            row[it.DienGiai] = it.SLThucHien;
                        }
                    }
                    dtbangke.Rows.Add(row);
                }

                if (nhamayselected.Name == "All")
                {
                    var querytotalall = querygrouptotal.GroupBy(p => new { DienGiai = p.DienGiai }).Select(p => new { DienGiai = p.Key.DienGiai, SLThucHien = p.Sum(n => n.SLThucHien) }).ToList();
                    DataRow rowtotal = dtbangke.NewRow();
                    var d = querytotalall.GroupBy(p => 1).Select(p => new { SLThucHien = p.Sum(n => n.SLThucHien) }).ToList();
                    rowtotal["Total"] = d[0].SLThucHien;
                    foreach (var it in querytotalall)
                    {
                        rowtotal[it.DienGiai] = it.SLThucHien;

                        //row[it.DienGiai] = it.SLThucHien;
                    }
                    rowtotal["Thang"] = "Cộng";
                    rowtotal["NhaMay"] = " Tổng cty";
                    dtbangke.Rows.Add(rowtotal);
                }
                foreach (InitDxGridDataColumn initDxGridDataColumn in lstcolumn)
                {
                    if (initDxGridDataColumn.FieldName == "NhaMay")
                    {
                        initDxGridDataColumn.GroupIndex = 0;
                    }
                }



                querycolumn.Clear();
                querygrouptotal.Clear();
                querygrouptotal.Clear();
                querynhamay.Clear();
                querygrouprow.Clear();
                querynangsuat.Clear();

                //Xử lý phần màu trước

            }
            return dtbangke;

        }

        private async Task<DataTable> NhapKhoTuNgayDenNgay()
        {
            visiblechart = false;
            DataTable dtbangke = new DataTable();
            string sqlreplace = "";

            string dieukiennhamay = "";
            if (nhamayselected != null)
            {
                if (nhamayselected.Name != "All")
                    dieukiennhamay += " and NhaMay=@NhaMay";
            }
            string sql = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                            declare @DateBegin datetime='{1}'
                            declare @DateEnd datetime='{2}'
                            declare @LyDo nvarchar(100)=N'Nhập mới'
                            declare @DateMonthBegin int=datepart(MM,@DateBegin)
                            declare @DateMonthEnd int=datepart(MM,@DateEnd)
                            declare @DienGiai nvarchar(100)=''
                           
                             select sum(nk.SLNhap*qry.Gia) as SLThucHien,qry.NhaMay,sp.KhachHang as DienGiai,qry.Ngay from
                            (SELECT [IDNumber],Gia,[ArticleNumberID],NhaMay,dbo.GetFisrtDayOfMonth(Ngay) as Ngay
                              FROM [KhoTP_NK] xk
                            
                              where LyDo=@LyDo and Ngay>=@DateBegin and Ngay<=@DateEnd  {4}) as qry
                              
							  inner join dbo.KhoTP_NK nk on qry.IDNumber=nk.IDNumber

                              inner join dbo.SanPham sp on nk.MaSP=sp.MaSP
                              group by qry.NhaMay,sp.KhachHang,qry.Ngay
                            ", (nhamayselected == null) ? "" : nhamayselected.Name.ToString(), dtpbegin.Value.ToString("MM/dd/yyyy 00:00"), dtpend.Value.ToString("MM/dd/yyyy 23:59"), sqlreplace, dieukiennhamay);

            CallAPI callAPI = new CallAPI();
            string json = "";
            json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());

            if (json != "")
            {
                var querynangsuat = JsonConvert.DeserializeObject<List<NhapkhotpItem>>(json);


                if (querynangsuat.Count == 0)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, $"Không có dữ liệu, vui lòng kiểm tra lại thông tin tìm kiếm"));
                    return dtbangke;
                }
                //Kehoach
                // var querygroup = queryktp.GroupBy(p => new { MaSP = p.MaSP }).Select(p => new { MaSP = p.Key.MaSP }).ToList(); ;
                var querycolumn = querynangsuat.GroupBy(p => p.DienGiai).Select(p => new { DienGiai = p.Key.ToString() }).OrderBy(p => p.DienGiai).ToList();
                var querygrouprow = querynangsuat.GroupBy(p => new { NhaMay = p.NhaMay, Ngay = p.Ngay }).Select(p => new { NhaMay = p.Key.NhaMay, SLThucHien = p.Sum(n => n.SLThucHien), Ngay = p.Key.Ngay }).OrderBy(p => p.Ngay).OrderBy(p => p.NhaMay).ToList();
                //var querysum = query.GroupBy(p => new { Diengiai = p.DienGiai }).Select(p => new { DienGiai = p.Key.Diengiai, SoKhoi = p.Sum(n => n.SoKhoi * n.SLNhap) }).ToList();
                var querygrouptotal = querynangsuat.GroupBy(p => new { NhaMay = p.NhaMay, DienGiai = p.DienGiai }).Select(p => new { NhaMay = p.Key.NhaMay, DienGiai = p.Key.DienGiai, SLThucHien = p.Sum(n => n.SLThucHien) }).OrderBy(p => p.NhaMay).ToList();
                dtbangke.Columns.Add("Thang", typeof(string));
                dtbangke.Columns.Add("DVT", typeof(string));
                dtbangke.Columns.Add("NhaMay", typeof(string));
                dtbangke.Columns.Add("VisibleChart", typeof(bool));
                dtbangke.Columns["VisibleChart"].DefaultValue = false;
                dtbangke.Columns.Add("TyLe", typeof(double));
                dtbangke.Columns["TyLe"].DefaultValue = 0;
                lstcolumn.Add(new InitDxGridDataColumn(0, "Thang", "Tháng"));
                lstcolumn.Add(new InitDxGridDataColumn(0, "Thang", "Tháng"));
                lstcolumn.Add(new InitDxGridDataColumn(0, "NhaMay", "Nhà máy", 0));



                //
                List<DateTime> lstdate = new List<DateTime>();
                foreach (var it in querycolumn)
                {
                    dtbangke.Columns.Add(it.DienGiai, typeof(double));
                    lstcolumn.Add(new InitDxGridDataColumn(0, it.DienGiai, it.DienGiai));


                }
                dtbangke.Columns.Add("Total", typeof(double));
                lstcolumn.Add(new InitDxGridDataColumn(0, "Total", "Tổng Nhập"));
                //grvSanPham.Columns.Add(addGridColumnPercent("TyLe", "% Thực hiện"));


                foreach (var it in querygrouprow)
                {
                    DataRow row = dtbangke.NewRow();
                    row["Thang"] = it.Ngay.Month;
                    row["NhaMay"] = it.NhaMay;
                    row["DVT"] = "USD";
                    row["Total"] = it.SLThucHien;
                    dtbangke.Rows.Add(row);
                }
                foreach (var it in querynangsuat)
                {
                    foreach (DataRow row in dtbangke.Rows)
                    {
                        if (it.Ngay.Month.ToString() == row["Thang"].ToString() && row["NhaMay"].ToString() == it.NhaMay)
                        {
                            row[it.DienGiai] = it.SLThucHien;
                            break;
                        }
                    }
                }
                var querynhamay = querygrouptotal.GroupBy(p => p.NhaMay).Select(p => new { NhaMay = p.Key.ToString(), SLThucHien = p.Sum(n => n.SLThucHien) }).ToList();
                foreach (var itnhamay in querynhamay)
                {
                    DataRow row = dtbangke.NewRow();
                    row["Total"] = itnhamay.SLThucHien;
                    foreach (var it in querygrouptotal)
                    {
                        if (itnhamay.NhaMay == it.NhaMay)
                        {
                            row["Thang"] = "Cộng";
                            row["NhaMay"] = it.NhaMay;
                            row[it.DienGiai] = it.SLThucHien;
                        }
                    }
                    dtbangke.Rows.Add(row);
                }
                if (nhamayselected.Name == "All")
                {
                    var querytotalall = querygrouptotal.GroupBy(p => new { DienGiai = p.DienGiai }).Select(p => new { DienGiai = p.Key.DienGiai, SLThucHien = p.Sum(n => n.SLThucHien) }).ToList();
                    DataRow rowtotal = dtbangke.NewRow();
                    var d = querytotalall.GroupBy(p => 1).Select(p => new { SLThucHien = p.Sum(n => n.SLThucHien) }).ToList();
                    rowtotal["Total"] = d[0].SLThucHien;
                    foreach (var it in querytotalall)
                    {
                        rowtotal[it.DienGiai] = it.SLThucHien;

                        //row[it.DienGiai] = it.SLThucHien;
                    }
                    rowtotal["Thang"] = "Cộng";
                    rowtotal["NhaMay"] = " Tổng Cty";
                    dtbangke.Rows.Add(rowtotal);
                    querytotalall.Clear();
                }

                querycolumn.Clear();
                querygrouptotal.Clear();

                querygrouptotal.Clear();
                querynhamay.Clear();
                querygrouprow.Clear();
                querynangsuat.Clear();

                //Xử lý phần màu trước

            }
            return dtbangke;
        }
    }


}