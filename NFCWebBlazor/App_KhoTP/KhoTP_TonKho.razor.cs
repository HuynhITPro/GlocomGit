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

namespace NFCWebBlazor.App_KhoTP
{
    public partial class KhoTP_TonKho
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] BrowserService browserService { get; set; }
        protected override Task OnInitializedAsync()
        {
           
            return base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
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
            catch(Exception ex)
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
                var query = await Model.ModelData.GetSanPham();
                lstsanpham = query.ToList();
                var query2 = await Model.ModelData.GetListKhachHang();
                lstkhachhang = query2.ToList();
                columnsbegin = 0;
                foreach (var it in dxGrid.GetColumns())
                {
                    columnsbegin++;
                }
                baocaoselected = txtloaibaocao.SelectedValue("Tồn kho theo sản phẩm");
            }
            catch(Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning,"Lỗi loadasyn :"+ ex.Message));
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


                //DxGridDataColumn dxGridDataColumn = new DxGridDataColumn();
                //RenderFragment<GridDataColumnCellDisplayTemplateContext> CellDisplayTemplate;
                //CellDisplayTemplate = (RenderFragment<GridDataColumnCellDisplayTemplateContext>)((context) => (renderTreeBuilder) =>
                //{
                //    renderTreeBuilder.OpenComponent<GridColumnTemplateChart>(0);
                //    renderTreeBuilder.AddAttribute(1, "datarow", context.Value);
                //    //renderTreeBuilder.AddContent(1, context.Value);
                //    renderTreeBuilder.CloseElement();
                //});



                //b.OpenComponent(counter, typeof(DxGridDataColumn));
                //b.AddAttribute(0, "FieldName", "Chart");
                //b.AddAttribute(0, "VisibleIndex", 4);
                ////RenderFragment renderFragment;


                ////renderFragment = builder =>
                ////{
                ////    builder.OpenComponent<GridColumnTemplateChart>(0);
                ////    builder.CloseComponent();
                ////};
                //b.AddAttribute(0, "CellDisplayTemplate", CellDisplayTemplate);

                ////b.AddAttribute(0, "Width", "270px");

                //b.CloseComponent();

            };
            return columns;
        }

        class ThanhTienNhaMay
        {
            public ThanhTienNhaMay()
            {

            }
            public ThanhTienNhaMay(string _nhamay,double _thanhtien)
            {
                NhaMay = _nhamay;
                ThanhTien = _thanhtien;
            }
            public string NhaMay { get; set; }
            public double ThanhTien { get; set; } = 0;
        }
        List<ThanhTienNhaMay> lstthanhtien = new List<ThanhTienNhaMay>();
      
        private void inittonkho()
        {

            InitDxGridDataColumn initDxGridDataColumn = new InitDxGridDataColumn(0,"TenKH", "Tên KH",0);
            InitDxGridDataColumn initDxGridDataColumn1 = new InitDxGridDataColumn(1,"MaSP", "Mã SP",null,140);
            InitDxGridDataColumn initDxGridDataColumn2 = new InitDxGridDataColumn(2,"TenSP", "Tên SP", null, 240);
            //InitDxGridDataColumn initDxGridDataColumn3 = new InitDxGridDataColumn(3,"ArticleNumber", "Art. No");
            //InitDxGridDataColumn initDxGridDataColumn4 = new InitDxGridDataColumn(4,"Type_Other", "Thị trường");
            //InitDxGridDataColumn initDxGridDataColumn5 = new InitDxGridDataColumn(5,"TenMau", "Tên màu");
            //InitDxGridDataColumn initDxGridDataColumn6 = new InitDxGridDataColumn(6,"Chart", "Chart");
            //InitDxGridDataColumn initDxGridDataColumn7 = new InitDxGridDataColumn(7,"DauTuan", "Dấu tuần");
            InitDxGridDataColumn initDxGridDataColumn8 = new InitDxGridDataColumn(8,"TonDau", "Tồn đầu", "#,#",120);
            InitDxGridDataColumn initDxGridDataColumn9 = new InitDxGridDataColumn(9,"SLNhap", "Nhập TK", "#,#", 120);
            InitDxGridDataColumn initDxGridDataColumn10 = new InitDxGridDataColumn(10,"SLXuat", "Xuất TK", "#,#", 120);
            InitDxGridDataColumn initDxGridDataColumn11 = new InitDxGridDataColumn(11,"SanSang", "Đạt", "#,#", 120);
            InitDxGridDataColumn initDxGridDataColumn12 = new InitDxGridDataColumn(12,"ChuaSanSang", "Hàng mẫu/ K.đạt", "#,#", 120);
            InitDxGridDataColumn initDxGridDataColumn13 = new InitDxGridDataColumn(13,"SLTon", "Tổng tồn", "#,#", 120);
            InitDxGridDataColumn initDxGridDataColumn14 = new InitDxGridDataColumn(14,"SuaChua", "Xưởng nợ", "#,#", 120);
            lstcolumn.Add(initDxGridDataColumn);
            lstcolumn.Add(initDxGridDataColumn1);
            lstcolumn.Add(initDxGridDataColumn2);
            //lstcolumn.Add(initDxGridDataColumn3);
            //lstcolumn.Add(initDxGridDataColumn4);
            //lstcolumn.Add(initDxGridDataColumn5);
            //lstcolumn.Add(initDxGridDataColumn6);
            //lstcolumn.Add(initDxGridDataColumn7);
            lstcolumn.Add(initDxGridDataColumn8);
            lstcolumn.Add(initDxGridDataColumn9);
            lstcolumn.Add(initDxGridDataColumn10);
            lstcolumn.Add(initDxGridDataColumn11);
            lstcolumn.Add(initDxGridDataColumn12);
            lstcolumn.Add(initDxGridDataColumn13);
            lstcolumn.Add(initDxGridDataColumn14);
         
        }
        private void inittonkhoArt()
        {

            InitDxGridDataColumn initDxGridDataColumn = new InitDxGridDataColumn(0,"TenKH", "Tên KH",0);
            InitDxGridDataColumn initDxGridDataColumn1 = new InitDxGridDataColumn(1,"MaSP", "Mã SP",null,140);
            InitDxGridDataColumn initDxGridDataColumn2 = new InitDxGridDataColumn(2,"TenSP", "Tên SP",null,240);
            InitDxGridDataColumn initDxGridDataColumn3 = new InitDxGridDataColumn(3,"ArticleNumber", "Art. No",null, 120);
            InitDxGridDataColumn initDxGridDataColumn4 = new InitDxGridDataColumn(4,"Type_Other", "Thị trường",null, 120);
            InitDxGridDataColumn initDxGridDataColumn5 = new InitDxGridDataColumn(5,"TenMau", "Tên màu",null,120);
            //InitDxGridDataColumn initDxGridDataColumn6 = new InitDxGridDataColumn(6,"Chart", "Chart");
            //InitDxGridDataColumn initDxGridDataColumn7 = new InitDxGridDataColumn(7,"DauTuan", "Dấu tuần");
            InitDxGridDataColumn initDxGridDataColumn8 = new InitDxGridDataColumn(8,"TonDau", "Tồn đầu","#,#", 120);
            InitDxGridDataColumn initDxGridDataColumn9 = new InitDxGridDataColumn(9,"SLNhap", "Nhập TK", "#,#", 120);
            InitDxGridDataColumn initDxGridDataColumn10 = new InitDxGridDataColumn(10,"SLXuat", "Xuất TK", "#,#", 120);
            InitDxGridDataColumn initDxGridDataColumn11 = new InitDxGridDataColumn(11,"SanSang", "Đạt", "#,#", 120);
            InitDxGridDataColumn initDxGridDataColumn12 = new InitDxGridDataColumn(12,"ChuaSanSang", "Hàng mẫu/ K.đạt", "#,#", 120);
            InitDxGridDataColumn initDxGridDataColumn13 = new InitDxGridDataColumn(13,"SLTon", "Tổng tồn", "#,#", 120);
            InitDxGridDataColumn initDxGridDataColumn14 = new InitDxGridDataColumn(14,"SuaChua", "Xưởng nợ", "#,#", 120);
            lstcolumn.Add(initDxGridDataColumn);
            lstcolumn.Add(initDxGridDataColumn1);
            lstcolumn.Add(initDxGridDataColumn2);
            lstcolumn.Add(initDxGridDataColumn3);
            lstcolumn.Add(initDxGridDataColumn4);
            lstcolumn.Add(initDxGridDataColumn5);
            //lstcolumn.Add(initDxGridDataColumn6);
            //lstcolumn.Add(initDxGridDataColumn7);
            lstcolumn.Add(initDxGridDataColumn8);
            lstcolumn.Add(initDxGridDataColumn9);
            lstcolumn.Add(initDxGridDataColumn10);
            lstcolumn.Add(initDxGridDataColumn11);
            lstcolumn.Add(initDxGridDataColumn12);
            lstcolumn.Add(initDxGridDataColumn13);
            lstcolumn.Add(initDxGridDataColumn14);

        }
        private void inittonkhoDauTuan()
        {

            InitDxGridDataColumn initDxGridDataColumn = new InitDxGridDataColumn(0, "TenKH", "Tên KH", 0);
            InitDxGridDataColumn initDxGridDataColumn1 = new InitDxGridDataColumn(1, "MaSP", "Mã SP", null, 140);
            InitDxGridDataColumn initDxGridDataColumn2 = new InitDxGridDataColumn(2, "TenSP", "Tên SP", null, 240);
            InitDxGridDataColumn initDxGridDataColumn3 = new InitDxGridDataColumn(3, "ArticleNumber", "Art. No", null, 120);
            InitDxGridDataColumn initDxGridDataColumn4 = new InitDxGridDataColumn(4, "Type_Other", "Thị trường", null, 140);
            InitDxGridDataColumn initDxGridDataColumn5 = new InitDxGridDataColumn(5, "TenMau", "Tên màu", null, 120);
            //InitDxGridDataColumn initDxGridDataColumn6 = new InitDxGridDataColumn(6,"Chart", "Chart");
            InitDxGridDataColumn initDxGridDataColumn7 = new InitDxGridDataColumn(7,"DauTuan", "Dấu tuần", null, 120);
            InitDxGridDataColumn initDxGridDataColumn8 = new InitDxGridDataColumn(8, "NhaMayHienTai", "Nhà máy", null,120);
            //InitDxGridDataColumn initDxGridDataColumn9 = new InitDxGridDataColumn(9, "SLNhap", "Nhập TK", "#,#");
            //InitDxGridDataColumn initDxGridDataColumn10 = new InitDxGridDataColumn(10, "SLXuat", "Xuất TK", "#,#");
            //InitDxGridDataColumn initDxGridDataColumn11 = new InitDxGridDataColumn(11, "SanSang", "Đạt", "#,#");
            //InitDxGridDataColumn initDxGridDataColumn12 = new InitDxGridDataColumn(12, "ChuaSanSang", "Hàng mẫu/ K.đạt", "#,#");
            InitDxGridDataColumn initDxGridDataColumn13 = new InitDxGridDataColumn(13, "SLTon", "Tổng tồn", "#,#", 120);
            //InitDxGridDataColumn initDxGridDataColumn14 = new InitDxGridDataColumn(14, "SuaChua", "Xưởng nợ", "#,#");
            //InitDxGridDataColumn initDxGridDataColumn14 = new InitDxGridDataColumn(14, "SuaChua", "Xưởng nợ", "#,#");
            lstcolumn.Add(initDxGridDataColumn);
            lstcolumn.Add(initDxGridDataColumn1);
            lstcolumn.Add(initDxGridDataColumn2);
            lstcolumn.Add(initDxGridDataColumn3);
            lstcolumn.Add(initDxGridDataColumn4);
            lstcolumn.Add(initDxGridDataColumn5);
            //lstcolumn.Add(initDxGridDataColumn6);
            lstcolumn.Add(initDxGridDataColumn7);
            lstcolumn.Add(initDxGridDataColumn8);
            //lstcolumn.Add(initDxGridDataColumn9);
            //lstcolumn.Add(initDxGridDataColumn10);
            //lstcolumn.Add(initDxGridDataColumn11);
            //lstcolumn.Add(initDxGridDataColumn12);
            lstcolumn.Add(initDxGridDataColumn13);
            //lstcolumn.Add(initDxGridDataColumn14);

        }
        private void inittonkhoPallet()
        {


            InitDxGridDataColumn initDxGridDataColumn = new InitDxGridDataColumn(0, "TenKH", "Tên KH", 0);
            InitDxGridDataColumn initDxGridDataColumn1 = new InitDxGridDataColumn(1, "MaSP", "Mã SP", null, 140);
            InitDxGridDataColumn initDxGridDataColumn2 = new InitDxGridDataColumn(2, "TenSP", "Tên SP", null, 240);
            InitDxGridDataColumn initDxGridDataColumn3 = new InitDxGridDataColumn(3, "ArticleNumber", "Art. No", null, 120);
            InitDxGridDataColumn initDxGridDataColumn4 = new InitDxGridDataColumn(4, "Type_Other", "Thị trường", null, 140);
            //InitDxGridDataColumn initDxGridDataColumn5 = new InitDxGridDataColumn(5, "TenMau", "Tên màu", null, 120);
            //InitDxGridDataColumn initDxGridDataColumn6 = new InitDxGridDataColumn(6,"Chart", "Chart");
            InitDxGridDataColumn initDxGridDataColumn7 = new InitDxGridDataColumn(7, "DauTuan", "Dấu tuần", null, 120);
            InitDxGridDataColumn initDxGridDataColumn8 = new InitDxGridDataColumn(8, "NhaMayHienTai", "Nhà máy", null, 120);
            //InitDxGridDataColumn initDxGridDataColumn9 = new InitDxGridDataColumn(9, "SLNhap", "Nhập TK", "#,#");
            //InitDxGridDataColumn initDxGridDataColumn10 = new InitDxGridDataColumn(10, "SLXuat", "Xuất TK", "#,#");
            //InitDxGridDataColumn initDxGridDataColumn11 = new InitDxGridDataColumn(11, "SanSang", "Đạt", "#,#");
            //InitDxGridDataColumn initDxGridDataColumn12 = new InitDxGridDataColumn(12, "ChuaSanSang", "Hàng mẫu/ K.đạt", "#,#");
            InitDxGridDataColumn initDxGridDataColumn13 = new InitDxGridDataColumn(13, "SLTon", "Tổng tồn", "#,#", 120);
            //InitDxGridDataColumn initDxGridDataColumn14 = new InitDxGridDataColumn(14, "SuaChua", "Xưởng nợ", "#,#");
            //InitDxGridDataColumn initDxGridDataColumn14 = new InitDxGridDataColumn(14, "SuaChua", "Xưởng nợ", "#,#");
            lstcolumn.Add(initDxGridDataColumn);
            lstcolumn.Add(initDxGridDataColumn1);
            lstcolumn.Add(initDxGridDataColumn2);
            lstcolumn.Add(initDxGridDataColumn3);
            lstcolumn.Add(initDxGridDataColumn4);
            //lstcolumn.Add(initDxGridDataColumn5);
            //lstcolumn.Add(initDxGridDataColumn6);
            lstcolumn.Add(initDxGridDataColumn7);
            lstcolumn.Add(initDxGridDataColumn8);
            //lstcolumn.Add(initDxGridDataColumn9);
            //lstcolumn.Add(initDxGridDataColumn10);
            //lstcolumn.Add(initDxGridDataColumn11);
            //lstcolumn.Add(initDxGridDataColumn12);
            lstcolumn.Add(initDxGridDataColumn13);
            //lstcolumn.Add(initDxGridDataColumn14);

        }
        private async void search()
        {
            if(baocaoselected.Name==null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Vui lòng chọn loại báo cáo"));
                return;
            }
            lstcolumn.Clear();
            dtresultfinal.Clear();
            dtresultfinal.Columns.Clear();
            lstthanhtien.Clear();
            DataTable dt = await searchasAsync();
            titlechart = string.Format("Đơn hàng chưa xuất từ {0} - {1}", dtpbegin.Value.ToString("dd/MM"), dtpbegin.Value.AddDays(7).ToString("dd/MM"));
            if (baocaoselected.Name == "Tồn kho theo sản phẩm")
            {
                visiblechart = true;
                
                inittonkho();
            }
            if (baocaoselected.Name == "Tồn kho theo ArticleNumber")
            {
                visiblechart = true;
                inittonkhoArt();
            }
            if (baocaoselected.Name == "Tồn kho theo dấu tuần")
            {
                visiblechart = false;
                inittonkhoDauTuan();
            }
            if (baocaoselected.Name == "Tồn kho theo Vị trí")
            {
                visiblechart = false;
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Không hỗ trợ báo cáo này"));
                return;
                //inittonkho();
            }
            if (baocaoselected.Name == "Hàng Phải Trả")
            {
                visiblechart = false;
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Không hỗ trợ báo cáo này"));
                return;
            }
            if (baocaoselected.Name == "Tồn kho theo Pallet-Top")
            {
                visiblechart = false;
                inittonkhoPallet();
            }
            bool checkname = false;
            foreach (var it in lstcolumn)
            {
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
                    dt.Columns.Add(it.FieldName, typeof(string));
                }
            }
            dt.Columns.Add("Chart", typeof(string));
          


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
        private async Task<DataTable> searchasAsync()
        {
           

           
            string columnsnamedautuan = "";
            string dieukienngay = "";
            string dieukientondau = "";
            string dieukiensp = "";
            string khachhang = "";
            string sqldonhang = "";
            double widthtotal = 200;
            lstcolumnloi.Clear();
            double dpixel = 0;
            DataTable dtresult = new DataTable();
            if (dtpbegin == null || dtpend == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Vui lòng chọn ngày"));
                return dtresult;
              
            }
           
            ghichu = $"Từ ngày {dtpbegin.Value.ToString("dd-MM-yy")} đến - {dtpend.Value.ToString("dd-MM-yy")}";
          
            dieukienngay += " where Ngay>=@DateBegin and Ngay<=@DateEnd";
            dieukientondau += " where Ngay<@DateBegin";
            string dieukienkhachhang = "";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy") + " 00:00"));
            lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy") + " 23:59"));
            DataTable dtdonhangxuat = new DataTable();
            CallAPI callAPI = new CallAPI();
            if (khachhangselected != null)
            {
                if (dieukienkhachhang == "")
                {
                    dieukienkhachhang += string.Format(" where sp.KhachHang in {0}", khachhangselected.MaKh);

                }
                else
                {
                    dieukienkhachhang += string.Format(" and sp.KhachHang in {0}", khachhangselected.MaKh);
                }
                ghichu += Environment.NewLine + "Khách hàng: " +khachhangselected.TenKh;
            }
            if (sanphamselected!=null)
            {
                dieukiensp = string.Format(" and MaSP=@MaSP", sanphamselected.MaSP.ToString());
               
                lstpara.Add(new ParameterDefine("@MaSP", sanphamselected.MaSP));
                ghichu += Environment.NewLine + "Sản phẩm: " + sanphamselected.TenSP;
            }

            string sql = "";
            string json = "";
            try
            {
                PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
                //Các câu truy vấn giống nhau, chỉ khác nhau ở chỗ Group by
                if (baocaoselected.Name == "Tồn kho theo sản phẩm")
            {
                sql = string.Format(@"
                                --Xử lý bảng kho thành phẩm
                                select sp.MaSP,sp.TenKH,sp.TenSP,'' as ArticleNumber,'' as Type_Other,'' as TenMau,'' as Colorhex,TonDau,SLNhap,SLXuat,(SLNhap-SLXuat+TonDau) as SLTon,(SLNhap-SLXuat+TonDau)*isnull(art.Gia,0)  as ThanhTien,isnull(art.Gia,0) as Gia,[NhaMayHienTai]
                                from
                                (select MaSP,sum(TonDau) as TonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,[NhaMayHienTai] from 
                                (--Xử lý tồn đầu
                                SELECT [MaSP],sum([SLNhap]) as TonDau,0 as SLNhap,0 as SLXuat,[NhaMayHienTai]
                                  FROM [KhoTP_NK] 
                                  {0}{1} group by MaSP,NhaMayHienTai
                                  union all
                                  select [MaSP],-1*sum([SLNhap]) as TonDau,0 as SLNhap,0 as SLXuat,[NhaMayHienTai]
                                  from [KhoTP_NK] nk where Exists(
                                  SELECT [IDNumber]
                                  FROM [KhoTP_XK] xk
                                  where ChungTu in (SELECT  [MaCT]
                                  FROM [KhoTP_ChungTu] ct
                                  {0} ) and nk.IDNumber=xk.IDNumber {1}) 
                                  group by MaSP,NhaMayHienTai

                                  union all

                                  --Xử lý phát sinh trong kỳ
                                SELECT [MaSP],0 as TonDau,sum([SLNhap]) as SLNhap,0 as SLXuat,[NhaMayHienTai]
                                  FROM [KhoTP_NK] 
                                  {2}{1} group by MaSP,NhaMayHienTai

                                  union all

                                  select [MaSP],0 as TonDau,0 as SLNhap,sum([SLNhap]) as SLXuat,[NhaMayHienTai] from [KhoTP_NK] nk where Exists(
                                  SELECT [IDNumber]
                                  FROM [KhoTP_XK] xk
                                  where ChungTu in (SELECT  [MaCT]
                                  FROM [KhoTP_ChungTu] ct
                                  where Ngay>=@DateBegin and Ngay<=@DateEnd) and nk.IDNumber=xk.IDNumber {1}) 
                                  group by MaSP,NhaMayHienTai) as qry
                                  group by MaSP,NhaMayHienTai) as qryktp
                                  inner join dbo.Load_cbSP sp on qryktp.MaSP=sp.MaSP 
                                 left join (select MaSP,avg(Gia) as Gia from dbo.ArticleNumberProduct group by MaSP) art on sp.MaSP=art.MaSP {3}
                                  ", dieukientondau, dieukiensp, dieukienngay, dieukienkhachhang);

                sqldonhang = string.Format(@"declare @DateBegin datetime=cast('{0}' as date)
                            declare @DateEnd datetime=dateadd(dd,7,@DateBegin)

                            select MaSP ,sum(SLDonHang) as SLDonHang from
                            (SELECT [ArticleNumber],sum([SLDonHang]) as SLDonHang
                             FROM [KeHoachXuatHang]
                              where NgayXuat>=@DateBegin and NgayXuat<=@DateEnd and DaXuat=0 
                              group by ArticleNumber) as qry
                              inner join ArticleNumberProduct art 
                              on qry.ArticleNumber=art.ArticleNumber
                              group by MaSP",dtpbegin.Value.ToString("MM/dd/yyyy"));
                await Task.Delay(100);
               
                json = await callAPI.ExcuteQueryEncryptAsync(sqldonhang, new List<ParameterDefine>());
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<DataTable>(json);
                    dtdonhangxuat = query;
                    //await InvokeAsync(StateHasChanged);
                }

              
            }
            if (txtloaibaocao.Text == "Tồn kho theo ArticleNumber")
            {

                sql = string.Format(@"select art.MaSP,art.KhachHang as TenKH,art.TenSP,art.ArticleNumber,art.Type_Other as Type_Other,isnull(mm.TenMau,'') as TenMau,isnull(mm.Colorhex,'#FFFFFF') as Colorhex,TonDau,SLNhap,SLXuat,(SLNhap-SLXuat+TonDau) as SLTon,(SLNhap-SLXuat+TonDau)*isnull(art.Gia,0)  as ThanhTien,isnull(art.Gia,0) as Gia,[NhaMayHienTai]
                            from
                            (select isnull(ArticleNumberID,'') as ArticleNumberID,sum(TonDau) as TonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,[NhaMayHienTai] from 
                            (--Xử lý tồn đầu
                            SELECT isnull(ArticleNumberID,'') as ArticleNumberID,sum([SLNhap]) as TonDau,0 as SLNhap,0 as SLXuat,[NhaMayHienTai]
                              FROM [KhoTP_NK] 
                              {0}{1} group by ArticleNumberID,NhaMayHienTai
                              union all
                              select isnull(ArticleNumberID,'') as ArticleNumberID,-1*sum([SLNhap]) as TonDau,0 as SLNhap,0 as SLXuat,[NhaMayHienTai]
                              from [KhoTP_NK] nk where Exists(
                              SELECT [IDNumber]
                              FROM [KhoTP_XK] xk
                              where ChungTu in (SELECT  [MaCT]
                              FROM [KhoTP_ChungTu] ct
                             {0}) and nk.IDNumber=xk.IDNumber {1}) 
                              group by ArticleNumberID,NhaMayHienTai

                              union all

                              --Xử lý phát sinh trong kỳ
                            SELECT ArticleNumberID,0 as TonDau,sum([SLNhap]) as SLNhap,0 as SLXuat,[NhaMayHienTai]
                              FROM [KhoTP_NK] 
                              {2}{1} group by ArticleNumberID,NhaMayHienTai
                              union all
                              select ArticleNumberID,0 as TonDau,0 as SLNhap,sum([SLNhap]) as SLXuat,[NhaMayHienTai] from [KhoTP_NK] nk where Exists(
                              SELECT [IDNumber]
                              FROM [KhoTP_XK] xk
                              where ChungTu in (SELECT  [MaCT]
                              FROM [KhoTP_ChungTu] ct
                              where Ngay>=@DateBegin and Ngay<=@DateEnd) and nk.IDNumber=xk.IDNumber {1}) 
                              group by ArticleNumberID,NhaMayHienTai) as qry
                              group by ArticleNumberID,NhaMayHienTai) as qryktp
                              left join (select * from SanPham_Art sp {3}) art on qryktp.ArticleNumberID=art.ArticleNumber left join dbo.MaMau mm on art.MaMau=mm.MaMau", dieukientondau, dieukiensp, dieukienngay, dieukienkhachhang);

                sqldonhang = string.Format(@"declare @DateBegin datetime=cast('{0}' as date)
                        declare @DateEnd datetime=dateadd(dd,7,@DateBegin)

                        select ArticleNumber,SLDonHang from
                        (SELECT [ArticleNumber],sum([SLDonHang]) as SLDonHang
                         FROM [KeHoachXuatHang]
                          where NgayXuat>=@DateBegin and NgayXuat<=@DateEnd and DaXuat=0 
                          group by ArticleNumber) as qry", dtpbegin.Value.ToString("MM/dd/yyyy"));
                
               
                 json = await callAPI.ExcuteQueryEncryptAsync(sqldonhang, new List<ParameterDefine>());
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<DataTable>(json);
                    dtdonhangxuat = query;
                    //await InvokeAsync(StateHasChanged);
                }
                //dtdonhangxuat = prs.dt_Connect(sqldonhang, sqlConnection);

            }
            if (txtloaibaocao.Text == "Tồn kho theo dấu tuần")//Báo cáo tồn theo dấu tuần thì chỉ chốt được theo ngày kết thúc
            {
                columnsnamedautuan = "Dấu tuần";
              
              

                //sql mới với tốc độ khác
                sql = string.Format(@"declare @tbl as Table(ArticleNumberID nvarchar(100),DauTuan nvarchar(8),SoLuong float,[NhaMayHienTai] nvarchar(100))
                        insert into @tbl(ArticleNumberID,DauTuan,SoLuong,[NhaMayHienTai])
                        select ArticleNumberID,DauTuan,SoLuong,[NhaMayHienTai]
                        from
                        (select ArticleNumberID,DauTuan,sum(SoLuong) as SoLuong,[NhaMayHienTai]
                                                    from
                                                    (SELECT isnull(ArticleNumberID,'') as ArticleNumberID,DauTuan,sum([SLNhap]) as SoLuong,[NhaMayHienTai]
                                                      FROM [KhoTP_NK] 
                                                      where Ngay<=@DateEnd  group by ArticleNumberID,NhaMayHienTai,DauTuan

                                                      union all
                                                      select isnull(ArticleNumberID,'') as ArticleNumberID,DauTuan,-1*sum([SLNhap]) as SoLuong,[NhaMayHienTai]
                                                      from [KhoTP_NK] nk where Exists(
                                                      SELECT [IDNumber]
                                                      FROM [KhoTP_XK] xk
                                                      where ChungTu in (SELECT  [MaCT]
                                                      FROM [KhoTP_ChungTu] ct
                                                      where Ngay<=@DateEnd) and nk.IDNumber=xk.IDNumber)   
                                                      group by ArticleNumberID,NhaMayHienTai,DauTuan) as qry group by ArticleNumberID,DauTuan,[NhaMayHienTai])
							                          as qry where SoLuong>0

							
                        select sp.TenKH,sp.MaSP,sp.TenSP,art.ArticleNumber,art.Type_Other,qryktp.SoLuong as SLTon,qryktp.DauTuan,qryktp.NhaMayHienTai,mm.TenMau,isnull(mm.Colorhex,'#FFFFFF') as Colorhex 
                        from 
                                                  @tbl  as qryktp
                                                      inner join dbo.ArticleNumberProduct art on qryktp.ArticleNumberID=art.ArticleNumber
                                                      inner join (select * from Load_cbSP where 1=1 {0}) sp on art.MaSP=sp.MaSP inner join dbo.MaMau mm on art.MaMau=mm.MaMau
                                                      where qryktp.SoLuong>0", dieukiensp);

            }
            if (txtloaibaocao.Text == "Tồn kho theo Pallet-Top")
            {
                columnsnamedautuan = "Loại pallet";
                sql = string.Format(@"select sp.TenKH,sp.MaSP,sp.TenSP,art.ArticleNumber,art.Type_Other,qryktp.SoLuong as SLTon,qryktp.Kien as DauTuan,qryktp.NhaMayHienTai  from 

                            (select ArticleNumberID,Kien,sum(SoLuong) as SoLuong,[NhaMayHienTai]
                            from
                            (SELECT ArticleNumberID,Kien,sum([SLNhap]) as SoLuong,[NhaMayHienTai]
                              FROM [KhoTP_NK] 
                              where Ngay<=@DateEnd  group by ArticleNumberID,NhaMayHienTai,Kien
                              union all
                              select ArticleNumberID,Kien,-1*sum([SLNhap]) as SoLuong,[NhaMayHienTai]
                              from [KhoTP_NK] nk where Exists(
                              SELECT [IDNumber]
                              FROM [KhoTP_XK] xk
                              where ChungTu in (SELECT  [MaCT]
                              FROM [KhoTP_ChungTu] ct
                              where Ngay<=@DateEnd) and nk.IDNumber=xk.IDNumber)  
                              group by ArticleNumberID,NhaMayHienTai,Kien) as qry 
							  group by ArticleNumberID,Kien,[NhaMayHienTai]) 
                              as qryktp
                              inner join dbo.ArticleNumberProduct art on qryktp.ArticleNumberID=art.ArticleNumber
                              inner join (select * from Load_cbSP where 1=1 {0}) sp on art.MaSP=sp.MaSP
                              where qryktp.SoLuong>0  order by sp.TenSP",dieukiensp);
            }
            if (txtloaibaocao.Text == "Tồn kho theo Vị trí")
            {
                //ShowProgress.CloseAwait();
                //Urc_TonKho_VitriArt urc_TonKho_VitriArt = new Urc_TonKho_VitriArt();
                //WindowDialog windowDialog = new WindowDialog();
                //windowDialog.showUsercontrol(urc_TonKho_VitriArt, "Vị trí tồn kho");
                //windowDialog.ShowDialog();
                //sqlConnection.Close();
                //sqlCommand.Dispose();
                return dtresult;
            }
            if (txtloaibaocao.Text == "Hàng Phải Trả")
            {
                //ShowProgress.CloseAwait();
                //Urc_KhoTP_HangPhaiTra urc_TonKho_VitriArt = new Urc_KhoTP_HangPhaiTra();
                //WindowDialog windowDialog = new WindowDialog();
                //windowDialog.showUsercontrol(urc_TonKho_VitriArt, "Hàng phải trả");
                //windowDialog.ShowDialog();
                //sqlConnection.Close();
                //sqlCommand.Dispose();
                return dtresult;
            }
          
         
           
            //for (int j = grvDonHang.Columns.Count - 1; j >= columnsbegin; j--)
            //{
            //    grvDonHang.Columns.RemoveAt(j);
            //}
            //try
            //{
                DataTable dataTable = new DataTable();
                //PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
                 json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<DataTable>(json);
                    dataTable = query;
                    //await InvokeAsync(StateHasChanged);
                }
                //sqlCommand.CommandText = sql;
                //dataTable = prs.dt_sqlcmd(sqlCommand, sqlConnection);
                //Hàng sửa chữa, xưởng nợ
                sql = string.Format(@"with dtchungtu
                                ([MaCT],[LyDo]) as
                                (SELECT  [MaCT],[LyDo]
                                FROM [KhoTP_ChungTu]
                                 where LyDo not in (N'Chuyển nội bộ',N'Xuất bán',N'Hiệu chỉnh') and Ngay<='{0}')
                                  select sp.TenSP,art.ArticleNumber,art.Type_Other,SLNhap,LyDo,sp.MaSP  from
                                 (select LyDo,ArticleNumberID,sum(SLNhap) as SLNhap from
                                 (select dtchungtu.LyDo,nk.ArticleNumberID,sum(nk.SLNhap) as SLNhap
                                 from (select * from KhoTP_NK ) nk
                                 inner join
                                (select * from KhoTP_XK xk
                                where ChungTu in 
                                (select MaCT from dtchungtu)
                                ) as qryxk on nk.IDNumber=qryxk.IDNumber
                                inner join dtchungtu on qryxk.ChungTu=dtchungtu.MaCT
                                group by dtchungtu.LyDo,nk.ArticleNumberID
                                union all
                                SELECT LyDo,[ArticleNumberTraNo]
                                      ,-1*sum([SLTra]) as SLNhap
                                  FROM [KhoTP_NhapTraNo] where NgayInsert<='{0}'  group by [ArticleNumberTraNo],LyDo) as qrytotal
                                  group by LyDo,ArticleNumberID) as qryno 
                                inner join dbo.ArticleNumberProduct art on art.ArticleNumber=qryno.ArticleNumberID
                                inner join (select * from dbo.Load_cbSP ) sp on art.MaSP=sp.MaSP where SLNhap!=0 ", dtpend.Value.ToString("MM/dd/yyyy 23:59"));
                 json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                DataTable dtsuachua = new DataTable();
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<DataTable>(json);
                    dtsuachua = query;
                    //await InvokeAsync(StateHasChanged);
                }

                //sql = @"select MaSP,ArticleNumberID as ArticleNumber,sum(SLNhap) as SLNhap from KhoTP_NK nk where TrangThai is not null
                //        and IDNumber not in (select IDNumber from KhoTP_XK) group by MaSP,ArticleNumberID";

                sql = @"select MaSP,ArticleNumberID as ArticleNumber,sum(SLNhap) as SLNhap,TrangThai
                        from KhoTP_NK nk 
                        where TrangThai is not null 
                        and IDNumber not in (select IDNumber from KhoTP_XK) group by MaSP,ArticleNumberID,TrangThai";
                json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                DataTable dthangloi = new DataTable();
              
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<DataTable>(json);
                    dthangloi = query;
                    //await InvokeAsync(StateHasChanged);
                }

                //DataTable dthangloi = prs.dt_Connect(sql, sqlConnection);

                //GridControlBand gridControlBandSL = new GridControlBand();
                //gridControlBandSL.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
                //gridControlBandSL.Header = "CHI TIẾT TỪNG KHO";
                if (txtloaibaocao.Text == "Tồn kho theo sản phẩm" || txtloaibaocao.Text == "Tồn kho theo ArticleNumber")
                {
                    if (dataTable.Rows.Count > 0)
                    {
                        var query = dataTable.AsEnumerable().GroupBy(p => new { MaSP = p["MaSP"], TenKH = p["TenKH"], TenSP = p["TenSP"], ArticleNumber = p["ArticleNumber"], Type_Other = p["Type_Other"], TenMau = p["TenMau"], ColorHex = p["Colorhex"] })
                            .Select(p => new { MaSP = p.Key.MaSP, TenSP = p.Key.TenSP, TenKH = p.Key.TenKH, ArticleNumber = p.Key.ArticleNumber, Type_Other = p.Key.Type_Other, TenMau = p.Key.TenMau, ColorHex = p.Key.ColorHex, TonDau = p.Sum(n => n.Field<double>("TonDau")), SLNhap = p.Sum(n => n.Field<double>("SLNhap")), SLXuat = p.Sum(n => n.Field<double>("SLXuat")), TonKho = p.Sum(n => n.Field<double>("SLTon")), ThanhTien = p.Sum(n => n.Field<double>("ThanhTien")) }).ToList();
                        var querygroupnhamay = dataTable.AsEnumerable().GroupBy(p => new { NhaMay = p.Field<string>("NhaMayHienTai") }).Select(p => new { NhaMay = p.Key.NhaMay }).OrderBy(p => p.NhaMay).ToList();

                        dtresult.Columns.Add("TenKH", typeof(string));
                        dtresult.Columns.Add("MaSP", typeof(string));
                        dtresult.Columns.Add("TenSP", typeof(string));
                        dtresult.Columns.Add("ArticleNumber", typeof(string));
                        dtresult.Columns.Add("Type_Other", typeof(string));
                        dtresult.Columns.Add("TenMau", typeof(string));
                        dtresult.Columns.Add("Colorhex", typeof(string));
                        dtresult.Columns.Add("TonDau", typeof(double));
                        dtresult.Columns.Add("SLNhap", typeof(double));
                        dtresult.Columns.Add("SLXuat", typeof(double));
                        dtresult.Columns.Add("SLTon", typeof(double));
                        dtresult.Columns.Add("SanSang", typeof(double));
                        dtresult.Columns.Add("ChuaSanSang", typeof(double));
                        dtresult.Columns.Add("SuaChua", typeof(double));
                        dtresult.Columns.Add("ThanhTien", typeof(double));

                        dtresult.Columns.Add("PhaiXuat", typeof(double));
                        dtresult.Columns.Add("wPhaiXuat", typeof(int));
                        dtresult.Columns.Add("wSLTon", typeof(int));
                        dtresult.Columns.Add("PhaiNhap", typeof(double));
                        dtresult.Columns.Add("wPhaiNhap", typeof(int));
                        dtresult.Columns.Add("wTotal", typeof(int));
                        dtresult.Columns["PhaiXuat"].DefaultValue = 0;
                        dtresult.Columns["wPhaiXuat"].DefaultValue = 0;
                        dtresult.Columns["wSLTon"].DefaultValue = 0;
                        dtresult.Columns["PhaiNhap"].DefaultValue = 0;
                        dtresult.Columns["wPhaiNhap"].DefaultValue = 0;
                        dtresult.Columns["wTotal"].DefaultValue = 0;



                        foreach (var it in querygroupnhamay)
                        {
                            dtresult.Columns.Add(it.NhaMay, typeof(double));
                            lstcolumn.Add(new InitDxGridDataColumn(1000, it.NhaMay, it.NhaMay, "#,#", 100));
                            dtresult.Columns.Add("TT" + it.NhaMay, typeof(double));
                         
                        }
                       

                        foreach (var it in query)
                        {
                            DataRow row = dtresult.NewRow();
                            row["TenKH"] = it.TenKH;
                            row["MaSP"] = it.MaSP;
                            row["TenSP"] = it.TenSP;
                            row["ArticleNumber"] = it.ArticleNumber;
                            row["TenMau"] = it.TenMau;
                            row["Colorhex"] = it.ColorHex;
                            row["Type_Other"] = it.Type_Other;
                            row["TonDau"] = it.TonDau;
                            row["SLNhap"] = it.SLNhap;
                            row["SLXuat"] = it.SLXuat;
                            row["SLTon"] = it.TonKho;
                            row["SanSang"] = it.TonKho;
                            row["ChuaSanSang"] = 0;
                            row["SuaChua"] = 0;
                            row["ThanhTien"] = it.ThanhTien;
                            dtresult.Rows.Add(row);
                        }
                        string masp = "", art = "", nhamay = "";
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            masp = dataRow["MaSP"].ToString();
                            art = dataRow["ArticleNumber"].ToString();
                            foreach (DataRow row in dtresult.Rows)
                            {
                                if (masp == row["MaSP"].ToString() && art == row["ArticleNumber"].ToString())
                                {
                                    nhamay = dataRow["NhaMayHienTai"].ToString();
                                    row[nhamay] = dataRow["SLTon"];
                                   
                                    row["TT" + nhamay] = double.Parse(dataRow["SLTon"].ToString()) * double.Parse(dataRow["Gia"].ToString());
                                   
                                    break;
                                }
                            }
                        }
                        var querytt = dataTable.AsEnumerable().GroupBy(p => p.Field<string>("NhaMayHienTai")).Select(p => new ThanhTienNhaMay { NhaMay = p.Key.ToString(), ThanhTien = p.Sum(n=>n.Field<double>("ThanhTien")) }).ToList();
                        lstthanhtien.AddRange(querytt);

                        if (dtsuachua.Rows.Count > 0)
                        {
                            if (txtloaibaocao.Text == "Tồn kho theo sản phẩm")
                            {
                                widthtotal = 250;
                                var querysc = dtsuachua.AsEnumerable().GroupBy(p => p.Field<string>("MaSP")).Select(p => new { MaSP = p.Key.ToString(), SLNo = p.Sum(n => n.Field<double>("SLNhap")) }).ToList();
                                foreach (var it in querysc)
                                {
                                    foreach (DataRow dataRow in dtresult.Rows)
                                    {
                                        if (it.MaSP == dataRow["MaSP"].ToString())
                                        {
                                            dataRow["SuaChua"] = it.SLNo;
                                            break;
                                        }
                                    }
                                }

                                foreach (DataRow row in dtresult.Rows)
                                {
                                    foreach (DataRow rowxk in dtdonhangxuat.Rows)
                                    {
                                        if (row.Field<string>("MaSP") == rowxk.Field<string>("MaSP"))
                                        {
                                            row["PhaiXuat"] = rowxk["SLDonHang"];
                                            row["PhaiNhap"] = row.Field<double>("PhaiXuat") - row.Field<double>("SLTon");
                                            if (row.Field<double>("PhaiNhap") < 0)
                                            {
                                                row["PhaiNhap"] = 0;
                                            }
                                            break;
                                        }
                                    }
                                    if (row.Field<double>("PhaiXuat") > 0)
                                    {
                                        row["wPhaiXuat"] = widthtotal;

                                        dpixel = widthtotal / row.Field<double>("PhaiXuat");
                                        if (row.Field<double>("PhaiNhap") <= 0)
                                            row["wSLTon"] = widthtotal;
                                        else
                                        {
                                            row["wSLTon"] = (int)(Math.Round(dpixel * row.Field<double>("SLTon"), 0));

                                            if (row.Field<int>("wSLTon") > 0 && row.Field<int>("wSLTon") < 40)
                                            {
                                                row["wSLTon"] = 40;

                                            }
                                            row["wPhaiNhap"] = (int)(Math.Round(dpixel * row.Field<double>("PhaiNhap"), 0));
                                            if (row.Field<int>("wPhaiNhap") > 0 && row.Field<int>("wPhaiNhap") < 40)
                                            {
                                                row["wPhaiNhap"] = 40;
                                                if (row.Field<int>("wSLTon") > 0)
                                                    row["wSLTon"] = widthtotal - row.Field<int>("wPhaiNhap");
                                            }
                                        }
                                        row["wSLTon"] =(int)Math.Round(row.Field<int>("wSLTon") / widthtotal,0)*100;
                                        row["wPhaiNhap"] =100- row.Field<int>("wSLTon");
                                        row["wPhaiXuat"] = 100;
                                        row["wTotal"] = widthtotal;
                                    }

                                }
                            }
                            if (txtloaibaocao.Text == "Tồn kho theo ArticleNumber")
                            {
                                var querysc = dtsuachua.AsEnumerable().GroupBy(p => p.Field<string>("ArticleNumber")).Select(p => new { ArticleNumber = p.Key.ToString(), SLNo = p.Sum(n => n.Field<double>("SLNhap")) }).ToList();
                                foreach (var it in querysc)
                                {
                                    foreach (DataRow dataRow in dtresult.Rows)
                                    {
                                        if (it.ArticleNumber == dataRow["ArticleNumber"].ToString())
                                        {
                                            dataRow["SuaChua"] = it.SLNo;
                                            break;
                                        }
                                    }
                                }
                                widthtotal = 250;
                                foreach (DataRow row in dtresult.Rows)
                                {
                                    foreach (DataRow rowxk in dtdonhangxuat.Rows)
                                    {
                                        if (row.Field<string>("ArticleNumber") == rowxk.Field<string>("ArticleNumber"))
                                        {
                                            row["PhaiXuat"] = rowxk["SLDonHang"];
                                            row["PhaiNhap"] = row.Field<double>("PhaiXuat") - row.Field<double>("SLTon");
                                            if (row.Field<double>("PhaiNhap") < 0)
                                            {
                                                row["PhaiNhap"] = 0;
                                            }
                                            break;
                                        }
                                    }
                                    if (row.Field<double>("PhaiXuat") > 0)
                                    {
                                        row["wPhaiXuat"] = widthtotal;

                                        dpixel = widthtotal / row.Field<double>("PhaiXuat");
                                        if (row.Field<double>("PhaiNhap") <= 0)
                                            row["wSLTon"] = widthtotal;
                                        else
                                        {
                                            row["wSLTon"] = (int)(Math.Round(dpixel * row.Field<double>("SLTon"), 0));

                                            if (row.Field<int>("wSLTon") > 0 && row.Field<int>("wSLTon") < 40)
                                            {
                                                row["wSLTon"] = 40;

                                            }
                                            row["wPhaiNhap"] = (int)(Math.Round(dpixel * row.Field<double>("PhaiNhap"), 0));
                                            if (row.Field<int>("wPhaiNhap") > 0 && row.Field<int>("wPhaiNhap") < 40)
                                            {
                                                row["wPhaiNhap"] = 40;
                                                if (row.Field<int>("wSLTon") > 0)
                                                    row["wSLTon"] = widthtotal - row.Field<int>("wPhaiNhap");
                                            }
                                        }
                                        row["wSLTon"] = (int)Math.Round(row.Field<int>("wSLTon") / widthtotal, 0) * 100;
                                        row["wPhaiNhap"] = 100 - row.Field<int>("wSLTon");
                                        row["wPhaiXuat"] = 100;
                                        row["wTotal"] = widthtotal;
                                    }

                                }
                               
                            }
                            dtsuachua.Dispose();
                        }
                        if (dthangloi.Rows.Count > 0)
                        {
                            var querycolumnloi = dthangloi.AsEnumerable().GroupBy(p => new { TrangThai = p.Field<string>("TrangThai") }).Select(p => new { TrangThai = p.Key.TrangThai }).ToList();
                            foreach (var it in querycolumnloi)
                            {
                                dtresult.Columns.Add(it.TrangThai, typeof(double));
                                lstcolumnloi.Add(it.TrangThai);
                            }

                            if (txtloaibaocao.Text == "Tồn kho theo sản phẩm")
                            {

                                var querysc = dthangloi.AsEnumerable().GroupBy(p => new { MaSP = p.Field<string>("MaSP"), TrangThai = p.Field<string>("TrangThai") }).Select(p => new { MaSP = p.Key.MaSP.ToString(), SLNo = p.Sum(n => n.Field<double>("SLNhap")), TrangThai = p.Key.TrangThai }).ToList();
                                foreach (var it in querysc)
                                {
                                    foreach (DataRow dataRow in dtresult.Rows)
                                    {
                                        if (it.MaSP == dataRow["MaSP"].ToString())
                                        {
                                            dataRow[it.TrangThai] = it.SLNo;
                                            dataRow["SanSang"] = double.Parse(dataRow["SanSang"].ToString()) - it.SLNo;
                                            dataRow["ChuaSanSang"] = dataRow.Field<double>("ChuaSanSang") + it.SLNo;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (txtloaibaocao.Text == "Tồn kho theo ArticleNumber")
                            {

                                foreach (DataRow it in dthangloi.Rows)
                                {
                                    foreach (DataRow dataRow in dtresult.Rows)
                                    {
                                        if (it["MaSP"].ToString() == dataRow["MaSP"].ToString() && it["ArticleNumber"].ToString() == dataRow["ArticleNumber"].ToString())
                                        {
                                            dataRow[it["TrangThai"].ToString()] = it["SLNhap"];
                                            // dataRow["ChuaSanSang"] = it["SLNhap"];
                                            dataRow["SanSang"] = double.Parse(dataRow["SanSang"].ToString()) - double.Parse(it["SLNhap"].ToString());// it.SLNo;
                                            dataRow["ChuaSanSang"] = dataRow.Field<double>("ChuaSanSang") + it.Field<double>("SLNhap");
                                            break;
                                        }
                                    }
                                }
                            }
                            dthangloi.Dispose();
                        }

                    }
                }
                if (txtloaibaocao.Text == "Tồn kho theo dấu tuần")
                {
                    // addGridColumn("NhaMayHienTai");
                    dtresult = dataTable;
                    //grvDonHang.Columns["DauTuan"].Header = columnsnamedautuan;
                    //grvDonHang.Columns["NhaMayHienTai"].Header = "Nhà máy";


                }
                if (txtloaibaocao.Text == "Tồn kho theo Pallet-Top")
                {
                    //addGridColumn("NhaMayHienTai");
                    dtresult = dataTable;
                    //grvDonHang.Columns["DauTuan"].Header = columnsnamedautuan;
                    //grvDonHang.Columns["NhaMayHienTai"].Header = "Nhà máy";

                }
            //createbandtonkho();
                

            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi " + ex.Message));
                //msgBox.Show("Lỗi:" + ex.Message, IconMsg.iconerror);
            }
            finally
            {
                PreloadService.Hide();


            }
            return dtresult;
        }
    }
}
