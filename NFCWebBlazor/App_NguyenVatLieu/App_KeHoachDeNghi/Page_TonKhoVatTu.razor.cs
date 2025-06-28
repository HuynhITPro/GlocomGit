using BlazorBootstrap;
using DevExpress.Blazor;

using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Data;


namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Page_TonKhoVatTu
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
             
                lsthanghoa = await Model.ModelData.GetHangHoa();
                lstkhonvl = await Model.ModelData.GetKhoNvl();
                lstSanPham = await ModelData.GetSanPham();
                baocaoselected = txtloaibaocao.SelectedValue("Báo cáo theo sản phẩm");
                nhamayselected = txtnhamay.SelectedValue(userGlobal.users.NhaMay);
              
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
            public ThanhTienNhaMay(string _nhamay, double _thanhtien)
            {
                NhaMay = _nhamay;
                ThanhTien = _thanhtien;
            }
            public string NhaMay { get; set; }
            public double ThanhTien { get; set; } = 0;
        }
        List<ThanhTienNhaMay> lstthanhtien = new List<ThanhTienNhaMay>();

       
     
     
    
      

        string ghichu = "";
        App_ClassDefine.ClassProcess prs = new ClassProcess();
        List<string> lstcolumnloi = new List<string>();
        string ghichutemp = "";
        string dieukien = "", dieukienmahang = "";
       
        class TonKhoNVL
        {
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string TenNhom { get; set; }
            public string NhaMay { get; set; }
            public string TenSP { get; set; }
            public string GroupSP { get; set; }
            public double SLNhap { get; set; }
            public double SLNhapTT { get; set; }
            public double SLXuat { get; set; }
            public double SLXuatTT { get; set; }
            public double TonDau { get; set; }

            public double SLTon { get; set; }
            public double SLTonDau { get; set; }
            public string DVT { get; set; }
            public string DVTTT { get; set; }
            public string MaKho { get; set; }
            public string TenKho { get; set; }
            public double? MinTK { get; set; }
            public double? MaxTK { get; set; }

        }
        List<ParameterDefine> lstpara = new List<ParameterDefine>();
        DataTable dtitem = new DataTable();
        
     
        private async Task<List<TonKhoNVL>> getsqlserachAsync(CallAPI callAPI)
        {
            List<TonKhoNVL> lst = new List<TonKhoNVL>();
         
            if (baocaoselected==null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Vui lòng chọn loại báo cáo"));
                return lst;

              
            }
            if(dtpbegin==null||dtpend==null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Vui lòng chọn ngày"));
                return lst;
            }

            lstpara.Clear();
            ghichu = "";
            dieukien = "";
          
            dieukienmahang = "";
            string sql = "";

            ghichutemp = string.Format("Từ {0} - {1}", dtpbegin.Value.ToString("dd/MM/yy"), dtpend.Value.ToString("dd/MM/yy"));
           
           


            lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
            lstpara.Add(new ParameterDefine("@DateEnd", dtpbegin.Value.ToString("MM/dd/yyyy 23:59")));


            if (nhamayselected!=null)
            {
                if (nhamayselected.Name != "All")
                {
                    lstpara.Add(new ParameterDefine("@NhaMay",nhamayselected.Name));
                    dieukien += " and NhaMay=@NhaMay";
                   
                    ghichutemp += Environment.NewLine + "Nhà máy: " + nhamayselected.Name;
                }
                else
                    ghichutemp += Environment.NewLine + "Nhà máy: Tất cả";

            }
            else
                ghichutemp += Environment.NewLine + "Nhà máy: Tất cả";

            if (maHangselected!=null)
            {
                dieukienmahang = " where hh.MaHang=@MaHang";
                lstpara.Add(new ParameterDefine("@MaHang", maHangselected.MaHang.ToString()));
                ghichutemp += Environment.NewLine + "Hàng hóa: " + maHangselected.TenHang;
            }
            if (khonvlselected!=null)
            {
                lstpara.Add(new ParameterDefine("@MaKho", khonvlselected.Name));
                dieukien += " and MaKho=@MaKho";
                ghichutemp += Environment.NewLine + "Kho: " + khonvlselected.FullName;
              
            }


            if (baocaoselected.Name == "Báo cáo theo sản phẩm")
            {
                titlesp = "Tên SP";
                titlenhom = "Tên SP";
                string sqldieukiensp = "";
                if (sanphamselected!=null)
                {
                     sqldieukiensp = string.Format(@" where hh.MaHang in (SELECT MaHang
                              FROM [NvlChiTietKhuVuc]
                              where MaSP=N'{0}')", sanphamselected.MaSP);
                }
                sql = string.Format(@" 
                         --Xử lý hiển thị tên sản phẩm
                            use NFCNVL
                                            declare @tbldb Table (MaSP nvarchar(100),MaHang nvarchar(100))
	                                        insert into @tbldb(MaSP,MaHang)
	                                        select MaSP,MaHang 
	                                        from (SELECT [MaSP],[MaHang]
	                                        FROM [NFCNVL].[dbo].[NvlChiTietKhuVuc] where KhuVuc='DMNVL'
	                                        group by MaSP,MaHang) as qry

 
				                            declare @tblmahangsp Table(MaHang nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(100))
									                            insert into @tblmahangsp (MaHang,MaSP,TenSP)
									                            select qrytbl.MaHang,qrytbl.MaSP,sp.TenSP from
									                            (select * from @tbldb
									                            where MaHang in 
									                            (select MaHang from (select MaHang,count(*) as SLMaHang from @tbldb group by MaHang) as qryslc))
									                            as qrytbl inner join  [NFCDB].[dbo].[SanPham] sp on qrytbl.MaSP=sp.MaSP
					                        declare @tblgroupmahang Table(MaHang nvarchar(100),CountMaHang int,TenSP nvarchar(200),GroupSP nvarchar(2000))

					                        insert into @tblgroupmahang (MaHang,CountMaHang,TenSP,GroupSP)

					                        select MaHang,countmahang,case when countmahang=1 then TenSP else 'z. Dùng chung' end as TenSP,TenSP
					                        from (
					                        select MaHang,count(*) as countmahang,
					                        STUFF((SELECT DISTINCT ',' + TenSP
                                                                FROM @tblmahangsp tb
                                                                 WHERE tb.MaHang = a.MaHang 
                                                                  FOR XML PATH ('')), 1, 1, '')  AS TenSP
										                          from @tblmahangsp a
										                          group by MaHang) as qry
                                        -- đã xong phần hiển thị tên sản phẩm


                            select hh.MaHang,hh.TenHang,qrytk.SLTonDau,qrytk.SLNhap,qrytk.SLXuat,(qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat) as SLTon
                             ,isnull(hh.MinTK,0) as MinTK,isnull(hh.MaxTK,0) as MaxTK,qrytk.MaKho,mk.TenKho,qrytk.NhaMay,hh.DVT,tblsp.TenSP,tblsp.GroupSP,nh.TenNhom
                             from 
                             (select MaHang,NhaMay,MaKho,sum(SLTonDau) as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat
                             from 
                             (
                             select nxitem.MaHang,(nxitem.SLNhap-nxitem.SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,NhaMay,MaKho
                             from (select * from NvlNhapXuat where Ngay<@DateBegin {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT
                             union all
                              select nxitem.MaHang,0 as SLTonDau,nxitem.SLNhap,nxitem.SLXuat,NhaMay,MaKho
                             from (select * from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT) as qrytotal
                             group by MaHang,NhaMay,MaKho) as qrytk
                             inner join dbo.NvlHangHoa hh on qrytk.MaHang=hh.MaHang
                            inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                             inner join dbo.NvlMaKho mk on qrytk.MaKho=mk.MaKho
                            left join @tblgroupmahang tblsp on qrytk.MaHang=tblsp.MaHang  {1} 
                            {2}
                            order by hh.TenHang
                         ", dieukien, dieukienmahang,sqldieukiensp);
              
            }
            else
            {
                titlenhom = "Phân loại";
                titlesp = "Kho";
                sql = string.Format(@"use NFCNVL select hh.MaHang,hh.TenHang,qrytk.SLTonDau,qrytk.SLNhap,qrytk.SLXuat,(qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat) as SLTon
                             ,isnull(hh.MinTK,0) as MinTK,isnull(hh.MaxTK,0) as MaxTK,qrytk.MaKho,mk.TenKho,qrytk.NhaMay,hh.DVT,mk.TenKho as TenSP,nh.PhanLoai as GroupSP,nh.TenNhom
                             from 
                             (select MaHang,NhaMay,MaKho,sum(SLTonDau) as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat
                             from 
                             (
                             select nxitem.MaHang,(nxitem.SLNhap-nxitem.SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,NhaMay,MaKho
                             from (select * from NvlNhapXuat where Ngay<@DateBegin {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT
                             union all
                              select nxitem.MaHang,0 as SLTonDau,nxitem.SLNhap,nxitem.SLXuat,NhaMay,MaKho
                             from (select * from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT) as qrytotal
                             group by MaHang,NhaMay,MaKho) as qrytk
                             inner join dbo.NvlHangHoa hh on qrytk.MaHang=hh.MaHang
                            inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                           
                             inner join dbo.NvlMaKho mk on qrytk.MaKho=mk.MaKho 
                             {1} order by hh.TenHang", dieukien, dieukienmahang);
               
            }
            
          
             string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
            if (json != "")
            {

                var query = JsonConvert.DeserializeObject<List<TonKhoNVL>>(json);
                lst.AddRange(query);
                return lst;
                //await InvokeAsync(StateHasChanged);
            }
            return lst;
        }
        private async void searchClick()
        {
           // PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
            dtresultfinal.Clear();
            try
            {
                PreloadService.Show(SpinnerColor.Primary, "Vui lòng đợi....");

                DataTable dt = await searchasAsync();
                foreach (string it in lstnhamay)
                {
                    InitDxGridDataColumn initDxGridDataColumn = new InitDxGridDataColumn(0, it, it, "#,#", 120);
                    lstcolumn.Add(initDxGridDataColumn);
                }

                bool checkname = false;
                foreach (var it in dxGrid.GetDataColumns())
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
                        //it.Visible = false;
                        dt.Columns.Add(it.FieldName, typeof(string));
                    }
                }
                dtresultfinal = dt;
            }
            catch(Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
            finally
            {
                PreloadService.Hide();
            }
            StateHasChanged();
            //PreloadService.Hide();
        }
        List<string> lstnhamay = new List<string>();
        private async Task<DataTable> searchasAsync()
        {
            DataTable dtresult = new DataTable();
            CallAPI callAPI = new CallAPI();
            lstcolumn.Clear();
            List<TonKhoNVL> query = await getsqlserachAsync(callAPI);
            lstnhamay.Clear();
            var querygroup = query.GroupBy(p => new
            {
                MaHang = p.MaHang,
                DVT = p.DVT
            }).Select(p => new TonKhoNVL
            {
                MaHang = p.Key.MaHang,
                MinTK = p.Min(n => n.MinTK),
                MaxTK = p.Min(n => n.MaxTK),
                DVT = p.Key.DVT,
                SLTonDau = p.Sum(n => n.SLTonDau),
                SLNhap = p.Sum(n => n.SLNhap),
                SLXuat = p.Sum(n => n.SLXuat),
                SLTon = p.Sum(n => n.SLTon)

            }).ToList();
          
            dtresult.Columns.Add("TenSP", typeof(string));
            dtresult.Columns.Add("GroupSP", typeof(string));
            dtresult.Columns.Add("MaHang", typeof(string));
            dtresult.Columns.Add("TenHang", typeof(string));
            dtresult.Columns.Add("TenNhom", typeof(string));
            dtresult.Columns.Add("TenKho", typeof(string));
            dtresult.Columns.Add("DVT", typeof(string));
            dtresult.Columns.Add("NhaMay", typeof(double));
            dtresult.Columns.Add("SLTonDau", typeof(double));
            dtresult.Columns.Add("SLNhap", typeof(double));
            dtresult.Columns.Add("SLXuat", typeof(double));
            dtresult.Columns.Add("SLTon", typeof(double));
            dtresult.Columns.Add("MinTK", typeof(double));
            dtresult.Columns.Add("MaxTK", typeof(double));
            foreach (var it in querygroup)
            {
                DataRow rownew = dtresult.NewRow();
                rownew["MaHang"] = it.MaHang;
                rownew["DVT"] = it.DVT;
                rownew["SLTonDau"] = it.SLTonDau;
                rownew["SLNhap"] = it.SLNhap;
                rownew["SLXuat"] = it.SLXuat;
                rownew["SLTon"] = it.SLTon;
                rownew["MinTK"] = it.MinTK;
                rownew["MaxTK"] = it.MaxTK;
                dtresult.Rows.Add(rownew);
            }
            var querycolumngroup = query.GroupBy(p => new { NhaMay = p.NhaMay, MaKho = p.MaKho, TenKho = p.TenKho }).Select(p => new { NhaMay = p.Key.NhaMay.ToString(), MaKho = p.Key.MaKho, TenKho = p.Key.TenKho, SLTon = p.Sum(n => n.SLTon) }).ToList();
            var querynhamay = querycolumngroup.GroupBy(p => new { NhaMay = p.NhaMay }).Select(p => new { NhaMay = p.Key.NhaMay, SLTon = p.Sum(n => n.SLTon) }).OrderBy(p => p.NhaMay).ToList();
          
            foreach (var it in querynhamay)
            {
                dtresult.Columns.Add(it.NhaMay, typeof(double));
                lstnhamay.Add(it.NhaMay);
               

            }
          
            foreach (var it in query)
            {
                foreach (DataRow row in dtresult.Rows)
                {
                    if (row["MaHang"].ToString() == it.MaHang)
                    {
                        if (row["TenHang"] == DBNull.Value)
                        {
                            row["TenHang"] = it.TenHang;
                            row["TenNhom"] = it.TenNhom;

                        }
                        row[it.NhaMay] = ((row[it.NhaMay] == DBNull.Value) ? 0 : double.Parse(row[it.NhaMay].ToString())) + it.SLTon;
                        row["TenSP"] = it.TenSP;
                        row["GroupSP"] = it.GroupSP;

                        //row[it.TenKho] = ((row[it.TenKho] == DBNull.Value) ? 0 : double.Parse(row[it.TenKho].ToString())) + it.SLTon;
                        break;
                    }
                }
            }
           
         
            dtresult.Columns.Add("ListItem", typeof(object));
            ghichu = ghichutemp;
          
            query.Clear();
            querycolumngroup.Clear();
            querygroup.Clear();
            querynhamay.Clear();
            return dtresult;
            
        }

        bool visiblesanpham = true;
        void SelectedItemChangedLoaiBaoCao(DataDropDownList dataDropDownList)
        {

            if (dataDropDownList == null)
                return;
            if (dataDropDownList.Name == "Báo cáo theo sản phẩm")
            {
                visiblesanpham = true;
            }
            else
                visiblesanpham = false;

        }
    }

}
