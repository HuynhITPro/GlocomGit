using BlazorBootstrap;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using System.Linq;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;
using NFCWebBlazor.App_ModelClass;
using DevExpress.XtraPrinting;
using DevExpress.Blazor;
using NFCWebBlazor.App_ClassDefine;
using DevExpress.Data.Filtering;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_NvlTonKho
    {
        [Inject] ToastService toastService { get; set; }
        bool Ismobile { get; set; }
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        protected override async Task OnInitializedAsync()
        {
            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 90;
            heightgrid = string.Format("{0}px", height);

            int width = dimension.Width;
            if (width < 768)
            {
                Ismobile = true;
                idgrid = "customGridnotheader";
            }
            else
            {
                Ismobile = false;
                idgrid = "abc";
            }
            //randomdivhide = prs.RandomString(10);
            await loadAsync();
           
            //return base.OnInitializedAsync();
        }
        class columname
        {
            public string Name { get; set; }
            public Type DataType { get; set; }
            //Khởi tạo class columname
            public columname(string name, Type type)
            {
                Name = name;
                DataType = type;
            }

        }
        List<columname> lstcolumn = new List<columname>();
        private void InitColumn()
        {
            lstcolumn.Add(new columname("MaHang",typeof(string)));
            lstcolumn.Add(new columname("TenHang", typeof(string)));
            lstcolumn.Add(new columname("SLTonDau", typeof(decimal)));
            lstcolumn.Add(new columname("SLNhap", typeof(decimal)));
            lstcolumn.Add(new columname("SLXuat", typeof(decimal)));
            lstcolumn.Add(new columname("SLTon", typeof(decimal)));
            lstcolumn.Add(new columname("TTTonDau", typeof(decimal)));
            lstcolumn.Add(new columname("TTNhap", typeof(decimal)));
            lstcolumn.Add(new columname("TTXuat", typeof(decimal)));
            lstcolumn.Add(new columname("ThanhTien", typeof(decimal)));
            lstcolumn.Add(new columname("MinTK", typeof(double)));
            lstcolumn.Add(new columname("MaxTK", typeof(double)));
            lstcolumn.Add(new columname("MaKho", typeof(string)));
            lstcolumn.Add(new columname("TenKho", typeof(string)));
            lstcolumn.Add(new columname("DVT", typeof(string)));
            lstcolumn.Add(new columname("TenSP", typeof(string)));
            lstcolumn.Add(new columname("GroupSP", typeof(string)));
            lstcolumn.Add(new columname("TenNhom", typeof(string)));

           

        }
        private async Task loadAsync()
        {
            lstkhonvl = await Model.ModelData.GetKhoNvl();
            lstmanhom = await Model.ModelData.GetlstNhomhang();
            lstmahang =await Model.ModelData.GetHangHoa();
            InitColumn();
            try
            {
                await JSRuntime.InvokeVoidAsync("initializeTooltips");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
            }
            List<DataDropDownList> lstlydotmp = await Model.ModelData.Getlstnvllydo();
            var querytinhtien =lstlydotmp.Where(p=>p.PhanLoai=="1").ToList();
            lydotinhtien="CÁC LÝ DO ĐƯỢC TÍNH TIỀN"+Environment.NewLine+"Tất cả các lý do";
            //foreach(var item in querytinhtien)
            //{
            //    lydotinhtien += item.FullName + Environment.NewLine;
            //}

        }
        string typeReport = "";//Phân ra 2 biến đầu vào: Kho và Lý do
        class GroupSum
        {
            public string GroupName { get; set; }
            public double Sum { get; set; } = 0;
            public double TyLe { get; set; } = 0;
            public double ThanhTien { get; set; }
            public double TTTonDau { get; set; }
            public double TTNhap { get; set; }
            public double TTXuat { get; set; }
        }
        class ThanhTienNhapXuat
        {
            public decimal TTNhap { get; set; }
            public decimal TTXuat { get; set; }
            
        }
        public class LoadItemTheKho
        {
            public string MaHang { get; set; }
            public string MaCT { get; set; }
            public string MaGN { get; set; }
            public string TenHang { get; set; }
            public string DVT { get; set; }
            public double? TTTonKho { get; set; }
            public string TenGN { get; set; }
            public string SoPhieu { get; set; }
            public string SoLo { get; set; }
            public int? SerialCT { get; set; }
            public int? SerialLink { get; set; }
            public DateTime? Ngay { get; set; }
            public DateTime? NgaySanXuat { get; set; }
            public DateTime? NgayHetHan { get; set; }
            public double SLNhap { get; set; }
            public double SLXuat { get; set; }
            public double SLTon { get; set; }
        }
        List<GroupSum>lstgroup=new List<GroupSum>();
        private void xulydieukien()
        {
            dxGrid.ClearFilter();

            titleton = "Tồn cuối kỳ";
            if (string.IsNullOrEmpty(loaibaocao))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn loại báo cáo"));
                return;
            }
            if (dtpbegin == null || dtpend == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn ngày"));
                return;
            }
            lstpara.Clear();

            ghichu = "";
            dieukien = "";

            dieukienmahang = "";
            string sql = "";

            ghichutemp = string.Format("Từ {0} - {1}", dtpbegin.Value.ToString("dd/MM/yy"), dtpend.Value.ToString("dd/MM/yy"));

            lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
            lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
            headertondauky = "TRƯỚC " + dtpbegin.Value.ToString("dd/MM/yy");
            headerphatsinh = string.Format("PHÁT SINH" + Environment.NewLine + "{0} - {1}", dtpbegin.Value.ToString("dd/MM"), dtpend.Value.ToString("dd/MM"));

            bool expandgroup = false;
            if (khoselected != null)
            {
                if (khoselected.Any())
                {
                    string dieukienkho = "", ghichukho = "";
                    foreach (var it in khoselected)
                    {
                        if (dieukienkho == "")
                            dieukienkho = string.Format("N'{0}'", it.Name);
                        else
                            dieukienkho += string.Format(",N'{0}'", it.Name);
                        ghichukho += it.FullName + ", ";
                    }
                    dieukien += string.Format(" and MaKho in ({0})", dieukienkho);
                    ghichutemp += Environment.NewLine + "Kho: " + TenKho;
                    expandgroup = true;
                }
            }
            else
                expandgroup = false;
            
            dieukienmahang = " where  abs(SLTonDau+SLNhap+SLXuat)>0.01";
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.TenNhom))
            {
                lstpara.Add(new ParameterDefine("@MaNhom", nvlNhapXuatItemShowcrr.TenNhom));
                if (dieukienmahang != "")
                    dieukienmahang += " and nh.MaNhom=@MaNhom";
                else
                    dieukienmahang += " where nh.MaNhom=@MaNhom";
                ghichutemp += Environment.NewLine + "Nhóm: " + nvlNhapXuatItemShowcrr.TenNhom;
            }
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
            {
                dieukienmahang = " where hh.MaHang=@MaHang";
                lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItemShowcrr.MaHang));
                ghichutemp += Environment.NewLine + "Hàng hóa: " + nvlNhapXuatItemShowcrr.TenHang;
                expandgroup = true;
            }
            if (checktonkholonhon0)
            {
                var filterCriteria = CriteriaOperator.Parse(string.Format("[SLTon]>0"));
                //var filterCriteria =
                // new InOperator("GroupMauSP", query.Select(c => c.MaMau));
                //var filterCriteria2 =new InOperator("KhuVucKH", query.Select(c => c.KhuVucKH));
                dxGrid.SetFilterCriteria(filterCriteria);
            }
            expandgrouprow = expandgroup;


        }
        public async Task searchAsync()
        {
            dtresult.Clear();
            xulydieukien();
            string sql = "";
            
            titlegroupsp = "Kho";
            if (khuvucselected.Any())
            {
                string dieukientinhtrangsudung = xulydieukientinhtrangsudung();
                visibletinhtrangsd = true;
   
     //           sql = string.Format(@"use NVLDB  
     //               declare @tblnxitemtinhtrang Table(SerialLink int primary key,TinhTrangSuDung nvarchar(100))
					//insert into @tblnxitemtinhtrang(SerialLink,TinhTrangSuDung)
					//select SerialLink,TinhTrangSuDung
					//from
					//(SELECT ROW_NUMBER() OVER (PARTITION BY SerialLink ORDER BY Serial) AS RowNum,Serial,[SerialLink],TinhTrangSuDung from NvlNhapXuatItem
					//{2}  
					//and SLNhap>0) as qry where RowNum=1
				
     //                         select hh.MaHang,hh.TenHang,TinhTrangSuDung,qrytk.SLTonDau,qrytk.SLNhap,qrytk.SLXuat,(qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat) as SLTon,qrytk.TTTonDau,qrytk.TTNhap,qrytk.TTXuat,qrytk.ThanhTien
     //                        ,isnull(hh.MinTK,0) as MinTK,isnull(hh.MaxTK,0) as MaxTK,qrytk.MaKho,mk.TenKho,hh.DVT,mk.TenKho as TenSP,mk.TenKho as GroupSP,nh.TenNhom,case when (qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat)=0 then 0 else round((qrytk.ThanhTien/(qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat)),2) end as DonGia
     //                        from 
     //                        (select MaHang,MaKho,tr.TinhTrangSuDung,sum(SLTonDau) as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,sum(TTTonDau) as TTTonDau,sum(TTNhap) as TTNhap,sum(TTXuat) as TTXuat,sum(ThanhTien) as ThanhTien
     //                        from 
     //                        (
     //                        select nxitem.MaHang,SerialLink,(nxitem.SLNhap-nxitem.SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,MaKho
     //                       ,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as TTTonDau,0 as TTNhap,0 as TTXuat,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as ThanhTien
     //                        from (select * from NvlNhapXuat where Ngay<@DateBegin  {0} ) nx
     //                        inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT
					//		where nxitem.SerialLink in (select SerialLink from @tblnxitemtinhtrang)
     //                        union all
     //                         select nxitem.MaHang,SerialLink,0 as SLTonDau,nxitem.SLNhap,nxitem.SLXuat,MaKho
     //                         ,0 as TTTonDau,nxitem.SLNhap*DonGia as TTNhap,nxitem.SLXuat*DonGia as TTXuat,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as ThanhTien
     //                        from (select * from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd  {0} ) nx
     //                        inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT
     //                         where nxitem.SerialLink in (select SerialLink from @tblnxitemtinhtrang)
     //                       ) as qrytotal left join @tblnxitemtinhtrang tr on qrytotal.SerialLink=tr.SerialLink
     //                        group by MaHang,MaKho,TinhTrangSuDung) as qrytk
     //                        inner join dbo.NvlHangHoa hh on qrytk.MaHang=hh.MaHang
     //                        inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                           
     //                        inner join dbo.NvlMaKho mk on qrytk.MaKho=mk.MaKho {1} and qrytk.MaKho<>'K007'
     //                         order by hh.MaHang", dieukien, dieukienmahang, dieukientinhtrangsudung);

                sql = string.Format(@" use NVLDB  declare @tblnxitemtinhtrang Table(SerialLink int primary key,TinhTrangSuDung nvarchar(100))
					insert into @tblnxitemtinhtrang(SerialLink,TinhTrangSuDung)
					select SerialLink,TinhTrangSuDung
					from
					(SELECT ROW_NUMBER() OVER (PARTITION BY SerialLink ORDER BY Serial) AS RowNum,Serial,[SerialLink],TinhTrangSuDung from NvlNhapXuatItem
					{2}  
					and SLNhap>0 and SerialLink>0) as qry where RowNum=1
                  IF OBJECT_ID('tempdb..#tmptonkho') IS NOT NULL
                 DROP TABLE #tmptonkho	
 
                 create Table #tmptonkho(MaHang nvarchar(100),SerialLink int,SLTonDau decimal(18,6),SLNhap decimal(18,6),SLXuat decimal(18,6),SLTon decimal(18,6),MaKho nvarchar(100)
				                ,TTTonDau decimal(18,6),TTNhap decimal(18,6),TTXuat decimal(18,6),ThanhTien decimal(18,6))
	                CREATE NONCLUSTERED INDEX IX_tbl_SerialLink ON #tmptonkho(SerialLink)		
			
				insert into #tmptonkho(MaHang,SerialLink,SLTonDau,SLNhap,SLXuat,SLTon,MaKho,TTTonDau,TTNhap,TTXuat,ThanhTien)
				
				select MaHang,SerialLink,sum(SLTonDau) as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,sum(SLTonDau+ SLNhap-SLXuat) as SLTon,MaKho,sum(TTTonDau) as TTTonDau,sum(TTNhap) as TTNhap,sum(TTXuat) as TTXuat,sum(ThanhTien) as ThanhTien
				from
				(
				select nxitem.MaHang,SerialLink,(nxitem.SLNhap-nxitem.SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,MaKho
                            ,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as TTTonDau,0 as TTNhap,0 as TTXuat,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as ThanhTien
                             from (select * from NvlNhapXuat where Ngay<@DateBegin   {0} ) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT
							where nxitem.SerialLink in (select SerialLink from @tblnxitemtinhtrang)
                             union all
                              select nxitem.MaHang,SerialLink,0 as SLTonDau,nxitem.SLNhap,nxitem.SLXuat,MaKho
                              ,0 as TTTonDau,nxitem.SLNhap*DonGia as TTNhap,nxitem.SLXuat*DonGia as TTXuat,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as ThanhTien
                             from (select * from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd   {0} ) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT
                              where nxitem.SerialLink in (select SerialLink from @tblnxitemtinhtrang)) as qry group by MaHang,SerialLink,MaKho
							  having  sum(SLTonDau+ SLNhap-SLXuat)<>0
		
                              select hh.MaHang,hh.MaPDOC,hh.TenHang,TinhTrangSuDung,qrytk.SLTonDau,qrytk.SLNhap,qrytk.SLXuat,qrytk.SLTon,qrytk.TTTonDau,qrytk.TTNhap,qrytk.TTXuat,qrytk.ThanhTien
                             ,isnull(hh.MinTK,0) as MinTK,isnull(hh.MaxTK,0) as MaxTK,qrytk.MaKho,mk.TenKho,hh.DVT,mk.TenKho as TenSP,mk.TenKho as GroupSP,nh.TenNhom,case when (qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat)=0 then 0 else round((qrytk.ThanhTien/(qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat)),2) end as DonGia
                             from 
                             (select MaHang,MaKho,tr.TinhTrangSuDung,sum(SLTonDau) as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,sum(SLTon) as SLTon,sum(TTTonDau) as TTTonDau,sum(TTNhap) as TTNhap,sum(TTXuat) as TTXuat,sum(ThanhTien) as ThanhTien
                             from 
							 #tmptonkho as qrytotal left join @tblnxitemtinhtrang tr on qrytotal.SerialLink=tr.SerialLink
                             group by MaHang,MaKho,TinhTrangSuDung) as qrytk
                             inner join dbo.NvlHangHoa hh on qrytk.MaHang=hh.MaHang
                             inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                           
                             inner join dbo.NvlMaKho mk on qrytk.MaKho=mk.MaKho {1} 
                             and qrytk.MaKho<>'K011'
                              order by hh.MaHang

	                DROP TABLE #tmptonkho	", dieukien, dieukienmahang, dieukientinhtrangsudung); ;

            }
            else
            {
                visibletinhtrangsd = false;
                sql = string.Format(@"use NVLDB  
                                select hh.MaHang,hh.TenHang,hh.MaPDOC,qrytk.SLTonDau,qrytk.SLNhap,qrytk.SLXuat,(qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat) as SLTon,qrytk.TTTonDau,qrytk.TTNhap,qrytk.TTXuat,qrytk.ThanhTien
                             ,isnull(hh.MinTK,0) as MinTK,isnull(hh.MaxTK,0) as MaxTK,qrytk.MaKho,mk.TenKho,hh.DVT,mk.TenKho as TenSP,mk.TenKho as GroupSP,nh.TenNhom,case when (qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat)=0 then 0 else round((qrytk.ThanhTien/(qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat)),2) end as DonGia
                             from 
                             (select MaHang,MaKho,sum(SLTonDau) as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,sum(TTTonDau) as TTTonDau,sum(TTNhap) as TTNhap,sum(TTXuat) as TTXuat,sum(ThanhTien) as ThanhTien
                             from 
                             (
                             select nxitem.MaHang,(nxitem.SLNhap-nxitem.SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,MaKho
                            ,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as TTTonDau,0 as TTNhap,0 as TTXuat,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as ThanhTien
                             from (select * from NvlNhapXuat where Ngay<@DateBegin  {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT
                             union all
                              select nxitem.MaHang,0 as SLTonDau,nxitem.SLNhap,nxitem.SLXuat,MaKho
                              ,0 as TTTonDau,nxitem.SLNhap*DonGia as TTNhap,nxitem.SLXuat*DonGia as TTXuat,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as ThanhTien
                             from (select * from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd  {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT) as qrytotal
                             group by MaHang,MaKho) as qrytk
                             inner join dbo.NvlHangHoa hh on qrytk.MaHang=hh.MaHang
                             inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                           
                             inner join dbo.NvlMaKho mk on qrytk.MaKho=mk.MaKho {1} 
                              order by hh.MaHang", dieukien, dieukienmahang);
            }
            CallAPI callAPI = new CallAPI();
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    dtresult = JsonConvert.DeserializeObject<DataTable>(json);
                    dtresult.Columns.Add("lstitem", typeof(List<LoadItemTheKho>));
                    lstgroup.Clear();
                    //foreach(DataColumn cl in dtresult.Columns)
                    //{
                    //    Console.WriteLine(string.Format("{0}- {1}", cl.ColumnName, cl.DataType));
                    //}
                    lstgroup = dtresult.AsEnumerable()
                       .GroupBy(row => row.Field<string>("TenSP"))
                       .Select(group => new GroupSum
                       {
                           GroupName = group.Key,
                           Sum = group.Sum(row => row.Field<double>("ThanhTien")),
                           ThanhTien = group.Sum(row => row.Field<double>("ThanhTien")),
                           TTTonDau=group.Sum(row => row.Field<double>("TTTonDau")),
                          
                           
                       }).ToList();
                    double total=lstgroup.Sum(p => p.Sum);
                    if(total>0)
                    {
                        foreach (var it in lstgroup)
                        {
                            it.TyLe = it.Sum / total;
                        }
                    }
                   
                    //lstgroup =dtresult.AsEnumerable().GroupBy(p=>p.Field<decimal>("ThanhTien"))
                    typeReport = "KhoInput";
                    if (dtresult.Rows.Count > 0)
                    {
                        await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
                    }
                  
                        string dieukienmahangfilter = "";
                        if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
                        {
                            if(dieukienmahangfilter=="")
                                dieukienmahangfilter = " where MaHang=@MaHang";
                            else
                                dieukienmahangfilter += " and MaHang=@MaHang";
                        }
                        if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.TenNhom))
                        {
                            if(dieukienmahangfilter=="")
                                dieukienmahangfilter=" where MaHang in (select MaHang from NvlHangHoa where MaNhom in (select MaNhom from NvlNhomHang where MaNhom=@MaNhom))";
                            else
                                dieukienmahangfilter += " and  MaHang in (select MaHang from NvlHangHoa where MaNhom in (select MaNhom from NvlNhomHang where MaNhom=@MaNhom))";
                        }
                        sql = string.Format(@"Use NVLDB
                       select qry.*,mk.TenKho as GroupName from (SELECT nx.MaKho ,isnull(sum([SLNhap]*DonGia),0) as TTNhap
                              ,isnull(sum([SLXuat]*DonGia),0)  as TTXuat
                          FROM [NvlNhapXuatItem] nxitem
                         inner join
                          (select Serial,MaKho from NvlNhapXuat 
                          where Ngay>=@DateBegin and Ngay<=@DateEnd {0}) as nx on nx.Serial=nxitem.SerialCT {1}  group by MaKho) as qry inner join dbo.NvlMaKho mk on qry.MaKho=mk.MaKho", dieukien, dieukienmahangfilter);
                        json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                        if (json != "")
                        {
                            var querytt = JsonConvert.DeserializeObject<List<GroupSum>>(json);
                            if (querytt != null)
                            {
                               foreach(var it in lstgroup)
                                {
                                    foreach(var iten in querytt)
                                    {
                                        if(it.GroupName==iten.GroupName)
                                        {
                                            it.TTNhap = iten.TTNhap;
                                            it.TTXuat = iten.TTXuat;
                                            break;
                                        }
                                    }
                                }
                            }
                            DataRow dataRow = dtresult.NewRow();
                            dataRow["TenSP"] = "TỔNG CỘNG";
                            dtresult.Rows.Add(dataRow);
                        
                            var querytotal=lstgroup.GroupBy(p=>1).Select(p=>new GroupSum{GroupName="TỔNG CỘNG",TTNhap=p.Sum(p=>p.TTNhap),TTXuat=p.Sum(p=>p.TTXuat),ThanhTien=p.Sum(p=>p.ThanhTien),TTTonDau=p.Sum(p=>p.TTTonDau)}).ToList();
                            lstgroup.AddRange(querytotal);
                        }
                       
                    }
                //prs.exportexcelAsync(JSRuntime, dtresult, 2, 1, "");
                    //Xử lý thêm thành tiền nhập xuất
                   
                    // await GotoMainForm.InvokeAsync();
                
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;

                StateHasChanged();
            }

        } 
        

        string dieukien = "", dieukienmahang = "";
        string ghichu = "";
        List<ParameterDefine> lstpara = new List<ParameterDefine>();
        DataTable dtitem = new DataTable();
        string ghichutemp = "";
       private string xulydieukientinhtrangsudung()
        {
            string dieukientinhtrangsudung = "";
            string dangsudung = "";
            foreach (var it in khuvucselected)
            {
                if (it.Name == "Hàng đang sử dụng")
                {
                    ghichutemp += Environment.NewLine + it.Name;
                    dangsudung = " TinhTrangSuDung is null";
                }
                else
                {
                    if (dieukientinhtrangsudung == "")
                    {
                        dieukientinhtrangsudung = string.Format("N'{0}'", it.Name);
                    }
                    else
                    {
                        dieukientinhtrangsudung += string.Format(",N'{0}'", it.Name);
                    }
                    ghichutemp += Environment.NewLine + it.Name;
                }

            }
            if (dieukientinhtrangsudung != "" && dangsudung != "")
            {
                dieukientinhtrangsudung = string.Format(" where TinhTrangSuDung in ({0}) or {1}", dieukientinhtrangsudung, dangsudung);
            }
            else
            {
                if (dieukientinhtrangsudung != "")
                {
                    dieukientinhtrangsudung = string.Format(" where TinhTrangSuDung in ({0}) ", dieukientinhtrangsudung);
                }
                if (dangsudung != "")
                {
                    dieukientinhtrangsudung = string.Format(" where {0}", dangsudung);
                }

            }
            return dieukientinhtrangsudung;
        }
        List<LoadItemTheKho> lstitem = new List<LoadItemTheKho>();
        private async Task LoadtItemAsync()
        {

            List<ParameterDefine> lstparaitem = new List<ParameterDefine>();
          
            foreach (ParameterDefine sqlParameter in lstpara)
            {
                ParameterDefine sqlParameter1 = new ParameterDefine();
                sqlParameter1.ParameterValue = sqlParameter.ParameterValue;
                sqlParameter1.ParameterName = sqlParameter.ParameterName;
                lstparaitem.Add(sqlParameter1);
            }

            lstitem.Clear();
            //string checkMaHang
            bool checkmahang = false;
            foreach(var it in lstpara)
            {
                if(it.ParameterName=="@MaHang")
                {
                    checkmahang = true;
                    break ;
                }
            }
            string sqlmahang = "";
            if(checkmahang)
            {
                sqlmahang=" (select * from NvlNhapXuatItem where MaHang=@MaHang) ";
            }
            else
            {
                sqlmahang = " NvlNhapXuatItem ";
            }

            string sql = string.Format(@"use NVLDB
                                    select qry.MaHang,qry.SLTonDau,qry.Ngay,qry.SLTonDau,qry.SLNhap,qry.SLXuat
                                    ,sum(qry.SLTonDau+qry.SLNhap-qry.SLXuat) OVER (
                                                PARTITION BY MaHang 
                                                ORDER BY Ngay, SLXuat ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                                            ) AS SLTon
                                    ,qry.MaGN,case qry.MaGN='TonDau' then N'Tồn đầu kỳ' else ngn.TenGN  end as TenGN
                                    from
                                    (select MaHang,DateAdd(dd,-1,@DateBegin) as Ngay,sum(SLNhap-SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,'TonDau' as MaGN 
                                     from {1} nxitem where SerialCT in (select Serial from  NvlNhapXuat where Ngay<@DateBegin {0})
                                     group by MaHang
                                     union all
                                    select MaHang,qyryCT.Ngay,0 as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,qyryCT.MaGN
                                    from {1} nxitem inner join (select Serial,Ngay,MaGN from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd  {0}) as qyryCT
                                     on nxitem.SerialCT=qyryCT.Serial group by MaHang,qyryCT.Ngay,qyryCT.MaGN)
                                    as qry 
                                    left join dbo.View_NoiGN ngn on qry.MaGN=ngn.MaGN
                                    order by qry.MaHang,qry.Ngay,SLXuat", dieukien, sqlmahang);
            CallAPI callAPI = new CallAPI();
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparaitem);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<LoadItemTheKho>>(json);
                    lstitem.AddRange(query);
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }


        }
        private async Task LoadtItemAsyncIDTem()
        {

            List<ParameterDefine> lstparaitem = new List<ParameterDefine>();

            foreach (ParameterDefine sqlParameter in lstpara)
            {
                ParameterDefine sqlParameter1 = new ParameterDefine();
                sqlParameter1.ParameterValue = sqlParameter.ParameterValue;
                sqlParameter1.ParameterName = sqlParameter.ParameterName;
                lstparaitem.Add(sqlParameter1);
            }

            lstitem.Clear();

            bool checkmahang = false;
            foreach (var it in lstpara)
            {
                if (it.ParameterName == "@MaHang")
                {
                    checkmahang = true;
                    break;
                }
            }
            string sqlmahang = "";
            if (checkmahang)
            {
                sqlmahang = " (select * from NvlNhapXuatItem where MaHang=@MaHang) ";
            }
            else
            {
                sqlmahang = " NvlNhapXuatItem ";
            }

            string sql = string.Format(@"use NVLDB select qry.*,ngn.TenGN from
                                (select MaHang,qyryCT.Ngay,SLNhap,SLXuat,qyryCT.MaGN,SerialCT,SerialLink
                                from {1} nxitem 
                                inner join (select Serial,Ngay,MaGN from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd {0}) as qyryCT
                                on nxitem.SerialCT=qyryCT.Serial
                                )
                                as qry inner join dbo.View_NoiGN ngn on qry.MaGN=ngn.MaGN
							
                                order by qry.Ngay,SerialCT
                            ", dieukien,sqlmahang);
            CallAPI callAPI = new CallAPI();
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparaitem);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<LoadItemTheKho>>(json);
                    lstitem.AddRange(query);
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }


        }
        private void ExportExcel()
        {
            dxGrid.ExportToXlsxAsync(string.Format("ExporTonKho_{0}_{1}",dieukien));
        }
        private async Task LoadtItemAsyncSoLo()
        {

            List<ParameterDefine> lstparaitem = new List<ParameterDefine>();

            foreach (ParameterDefine sqlParameter in lstpara)
            {
                ParameterDefine sqlParameter1 = new ParameterDefine();
                sqlParameter1.ParameterValue = sqlParameter.ParameterValue;
                sqlParameter1.ParameterName = sqlParameter.ParameterName;
                lstparaitem.Add(sqlParameter1);
            }

            lstitem.Clear();

            bool checkmahang = false;
            foreach (var it in lstpara)
            {
                if (it.ParameterName == "@MaHang")
                {
                    checkmahang = true;
                    break;
                }
            }
            string sqlmahang = "";
            if (checkmahang)
            {
                sqlmahang = " (select * from NvlNhapXuatItem where MaHang=@MaHang) ";
            }
            else
            {
                sqlmahang = " NvlNhapXuatItem ";
            }

            string sql = string.Format(@"use NVLDB select qry.*,ngn.TenGN,left(MaCT,4) as SoPhieu from
                                (select MaHang,qyryCT.Ngay,SLNhap,SLXuat,qyryCT.MaGN,SerialCT,SerialLink,SoLo,NgaySanXuat,MaCT
                                from {1} nxitem 
                                inner join (select Serial,Ngay,MaGN,MaCT from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd {0}) as qyryCT
                                on nxitem.SerialCT=qyryCT.Serial)
                                as qry inner join dbo.View_NoiGN ngn on qry.MaGN=ngn.MaGN
							
                                order by qry.Ngay,SerialCT
                            
                            ", dieukien, sqlmahang);
            CallAPI callAPI = new CallAPI();
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparaitem);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<LoadItemTheKho>>(json);
                    lstitem.AddRange(query);
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }


        }
        private string setSqlItem(DataRowView dataRowView)
        {
            if (dataRowView == null)
                return "";
            string filter = "";
            if (typeReport == "KhoInput")
            {
                filter = string.Format(" and MaKho=N'{0}'", dataRowView["MaKho"].ToString());
            }
            if(typeReport== "LyDoInput")
            {
                filter = string.Format(" and LyDo=N'{0}'", dataRowView["LyDo"].ToString());
            }
            string sql = "";

        //    string sql = string.Format(@"use NVLDB select qry.*,ngn.TenGN,khmhitem.TenLienKet from
        //                        (select MaHang,qyryCT.Ngay,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,qyryCT.MaGN,case when SLNhap>0 then 0 else SerialKHDH end as SerialKHDH
        //                        from (select * from NvlNhapXuatItem where MaHang=N'{1}') nxitem 
        //                        inner join (select Serial,Ngay,MaGN from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd {0} {2}) as qyryCT
        //                        on nxitem.SerialCT=qyryCT.Serial
        //                        group by MaHang,qyryCT.Ngay,qyryCT.MaGN,case when SLNhap>0 then 0 else  SerialKHDH end)
        //                        as qry inner join dbo.View_NoiGN ngn on qry.MaGN=ngn.MaGN
        //left join dbo.NvlKeHoachMuaHangItem khmhitem on qry.SerialKHDH=khmhitem.Serial 
        //                        order by qry.Ngay,SLNhap desc
        //                    ", dieukien, dataRowView["MaHang"].ToString(),filter);
            sql = string.Format(@"use NVLDB
                           select qry.MaHang,qry.SLTonDau,qry.Ngay,qry.SLTonDau,qry.SLNhap,qry.SLXuat
                ,sum(qry.SLTonDau+qry.SLNhap-qry.SLXuat) OVER (
                            PARTITION BY qry.MaHang 
                            ORDER BY qry.Ngay, qry.SLXuat ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                        ) AS SLTon
                ,sum(qry.TTTonDau+TTTonKho) OVER (
                            PARTITION BY qry.MaHang  
                            ORDER BY qry.Ngay, qry.SLXuat ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                        ) AS TTTonKho
                ,qry.MaGN,case when qry.MaGN='TonDau' then N'Tồn đầu kỳ' else ngn.TenGN  end as TenGN,hh.TenHang,hh.DVT
                from
                (select MaHang,DateAdd(dd,-1,@DateBegin) as Ngay,sum(SLNhap-SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,sum((SLNhap-SLXuat)*DonGia) as TTTonDau,0 as TTTonKho,'TonDau' as MaGN 
                 from (select * from NvlNhapXuatItem where MaHang=N'{1}') nxitem where SerialCT in (select Serial from  NvlNhapXuat where Ngay<@DateBegin {0} {2})
                 group by MaHang
                 union all
                select MaHang,qyryCT.Ngay,0 as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,0 as TTTonDau,sum((SLNhap-SLXuat)*DonGia) as TTTonKho,qyryCT.MaGN
                from (select * from NvlNhapXuatItem where MaHang=N'{1}') nxitem inner join (select Serial,Ngay,MaGN from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd  {0} {2}) as qyryCT
                 on nxitem.SerialCT=qyryCT.Serial group by MaHang,qyryCT.Ngay,qyryCT.MaGN)
                as qry 
                left join dbo.View_NoiGN ngn on qry.MaGN=ngn.MaGN
                inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                order by qry.MaHang,qry.Ngay,SLXuat", dieukien, dataRowView["MaHang"].ToString(), filter);
            visbletinhtrangsd = false;
            if (khuvucselected.Any())
            {
                string dieukiensudung = xulydieukientinhtrangsudung();
                visbletinhtrangsd = true;
                //Không hỗ trợ xem tồn chạy, vì không đảm bảo được dữ liệu, do sql query tồn đầu có vấn đề
                sql = string.Format(@"use NVLDB select qry.*,ngn.TenGN,khmhitem.TenLienKet from
                                (select MaHang,qyryCT.Ngay,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,qyryCT.MaGN,case when SLNhap>0 then 0 else SerialKHDH end as SerialKHDH
                                from (select * from NvlNhapXuatItem {3} and MaHang=N'{1}') nxitem 
                                inner join (select Serial,Ngay,MaGN from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd {0} {2}) as qyryCT
                                on nxitem.SerialCT=qyryCT.Serial
                                group by MaHang,qyryCT.Ngay,qyryCT.MaGN,case when SLNhap>0 then 0 else  SerialKHDH end)
                                as qry inner join dbo.View_NoiGN ngn on qry.MaGN=ngn.MaGN
								left join dbo.NvlKeHoachMuaHangItem khmhitem on qry.SerialKHDH=khmhitem.Serial 
                                order by qry.Ngay,SLNhap desc
                            ", dieukien, dataRowView["MaHang"].ToString(), filter, dieukiensudung);

                //sql = string.Format(@"use NVLDB
                //           select qry.MaHang,qry.SLTonDau,qry.Ngay,qry.SLTonDau,qry.SLNhap,qry.SLXuat
                //,sum(qry.SLTonDau+qry.SLNhap-qry.SLXuat) OVER (
                //            PARTITION BY qry.MaHang 
                //            ORDER BY qry.Ngay, qry.SLXuat ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                //        ) AS SLTon
                //,sum(qry.TTTonDau+TTTonKho) OVER (
                //            PARTITION BY qry.MaHang  
                //            ORDER BY qry.Ngay, qry.SLXuat ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                //        ) AS TTTonKho
                //,qry.MaGN,case when qry.MaGN='TonDau' then N'Tồn đầu kỳ' else ngn.TenGN  end as TenGN,hh.TenHang,hh.DVT
                //from
                //(select MaHang,DateAdd(dd,-1,@DateBegin) as Ngay,sum(SLNhap-SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,sum((SLNhap-SLXuat)*DonGia) as TTTonDau,0 as TTTonKho,'TonDau' as MaGN 
                // from (select * from NvlNhapXuatItem {3} and MaHang=N'{1}') nxitem where SerialCT in (select Serial from  NvlNhapXuat where Ngay<@DateBegin {0})
                // group by MaHang
                // union all
                //select MaHang,qyryCT.Ngay,0 as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,0 as TTTonDau,sum((SLNhap-SLXuat)*DonGia) as TTTonKho,qyryCT.MaGN
                //from (select * from NvlNhapXuatItem {3} and MaHang=N'{1}') nxitem inner join (select Serial,Ngay,MaGN from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd  {0} {2}) as qyryCT
                // on nxitem.SerialCT=qyryCT.Serial group by MaHang,qyryCT.Ngay,qyryCT.MaGN)
                //as qry 
                //left join dbo.View_NoiGN ngn on qry.MaGN=ngn.MaGN
                //inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                //order by qry.MaHang,qry.Ngay,SLXuat", dieukien, dataRowView["MaHang"].ToString(), filter, dieukiensudung);
            }
            return sql;



        }
        private async void printtonkho()
        {
            if (dtresult.Rows.Count == 0)
            {
                await searchAsync();
            }
            XtraRp_BaoCaoTonKho xtraRp_PhieuNhapKho = new XtraRp_BaoCaoTonKho();

            xtraRp_PhieuNhapKho.setGhiChu(ghichutemp);
            xtraRp_PhieuNhapKho.DataSource = dtresult;
            ModelAdmin.mainLayout.showreportAsync(xtraRp_PhieuNhapKho);
        }
        private async void printtonkhonew()
        {
            
            xulydieukien();
            string sql = "";
            titlegroupsp = "Kho";
            if (khuvucselected.Any())
            {
                string dieukientinhtrangsudung = xulydieukientinhtrangsudung();

                sql = string.Format(@"use NVLDB 
                    declare @tblnxitemtinhtrang Table(SerialLink int primary key)
					insert into @tblnxitemtinhtrang(SerialLink)
					SELECT [SerialLink] FROM [dbo].[NvlNhapXuatItem]
					 {2}  group by SerialLink

                                select hh.MaHang,hh.TenHang,qrytk.SLTonDau,qrytk.SLNhap,qrytk.SLXuat,(qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat) as SLTon,qrytk.TTTonDau,qrytk.TTNhap,qrytk.TTXuat,qrytk.ThanhTien
                             ,isnull(hh.MinTK,0) as MinTK,isnull(hh.MaxTK,0) as MaxTK,qrytk.MaKho,mk.TenKho,hh.DVT,mk.TenKho as TenSP,nh.PhanLoai as GroupSP,nh.TenNhom
                             from 
                             (select MaHang,MaKho,sum(SLTonDau) as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,sum(TTTonDau) as TTTonDau,sum(TTNhap) as TTNhap,sum(TTXuat) as TTXuat,sum(ThanhTien) as ThanhTien
                             from 
                             (
                             select nxitem.MaHang,(nxitem.SLNhap-nxitem.SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,MaKho
                            ,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as TTTonDau,0 as TTNhap,0 as TTXuat,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as ThanhTien
                             from (select * from NvlNhapXuat where Ngay<@DateBegin  {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT
                            where nxitem.SerialLink in (select SerialLink from @tblnxitemtinhtrang)
                             union all
                              select nxitem.MaHang,0 as SLTonDau,nxitem.SLNhap,nxitem.SLXuat,MaKho
                              ,0 as TTTonDau,nxitem.SLNhap*DonGia as TTNhap,nxitem.SLXuat*DonGia as TTXuat,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as ThanhTien
                             from (select * from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd  {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT
                              where nxitem.SerialLink in (select SerialLink from @tblnxitemtinhtrang)
                            ) as qrytotal
                             group by MaHang,MaKho) as qrytk
                             inner join dbo.NvlHangHoa hh on qrytk.MaHang=hh.MaHang
                             inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                           
                             inner join dbo.NvlMaKho mk on qrytk.MaKho=mk.MaKho {1}
                              order by hh.MaHang", dieukien, dieukienmahang, dieukientinhtrangsudung);


            }
            else
            {

                sql = string.Format(@"use NVLDB  
                                select hh.MaHang,hh.TenHang,qrytk.SLTonDau,qrytk.SLNhap,qrytk.SLXuat,(qrytk.SLTonDau+qrytk.SLNhap-qrytk.SLXuat) as SLTon,qrytk.TTTonDau,qrytk.TTNhap,qrytk.TTXuat,qrytk.ThanhTien
                             ,isnull(hh.MinTK,0) as MinTK,isnull(hh.MaxTK,0) as MaxTK,qrytk.MaKho,mk.TenKho,hh.DVT,mk.TenKho as TenSP,nh.PhanLoai as GroupSP,nh.TenNhom
                             from 
                             (select MaHang,MaKho,sum(SLTonDau) as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,sum(TTTonDau) as TTTonDau,sum(TTNhap) as TTNhap,sum(TTXuat) as TTXuat,sum(ThanhTien) as ThanhTien
                             from 
                             (
                             select nxitem.MaHang,(nxitem.SLNhap-nxitem.SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,MaKho
                            ,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as TTTonDau,0 as TTNhap,0 as TTXuat,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as ThanhTien
                             from (select * from NvlNhapXuat where Ngay<@DateBegin  {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT
                             union all
                              select nxitem.MaHang,0 as SLTonDau,nxitem.SLNhap,nxitem.SLXuat,MaKho
                              ,0 as TTTonDau,nxitem.SLNhap*DonGia as TTNhap,nxitem.SLXuat*DonGia as TTXuat,(nxitem.SLNhap-nxitem.SLXuat)*DonGia as ThanhTien
                             from (select * from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd  {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT) as qrytotal
                             group by MaHang,MaKho) as qrytk
                             inner join dbo.NvlHangHoa hh on qrytk.MaHang=hh.MaHang
                             inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                           
                             inner join dbo.NvlMaKho mk on qrytk.MaKho=mk.MaKho {1}
                              order by hh.MaHang", dieukien, dieukienmahang);
            }
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.reportname = "XtraRp_BaoCaoTonKho";

            //classReport.dtparameter = new DataTable();
            getDataFromSql.sql = sql;
           getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            DataTable dt = new DataTable();
            dt.Columns.Add("GhiChu", typeof(string));
            DataRow dataRow = dt.NewRow();
            dataRow["GhiChu"] = ghichutemp; //ghichutemp;
            dt.Rows.Add(dataRow);
            getDataFromSql.dtparameter =prs.ConvertDataTableToJson(dt) ;
            //dt.Dispose();

            await  ModelAdmin.mainLayout.showreportpdfAsync(getDataFromSql);


        }
        private async void printthekho()
        {
            if(string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
            {
              await  printthekhoallAsync();
            }
            else
            {
              await  printthekhotheomahangAsync(nvlNhapXuatItemShowcrr.MaHang);
            }
        }
        private async Task printthekhoallAsync()
        {
            if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn kho"));
                return;
            }
            //lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
            //dieukien += " and MaKho=@MaKho";
            //ghichutemp += Environment.NewLine + "Kho: " + TenKho;


            await searchAsync();
            await LoadtItemAsync();

            Xtra_TheKhoTheoNhaMay xtra_BaoCaoTonKhoTheoNhaMay = new Xtra_TheKhoTheoNhaMay();
            xtra_BaoCaoTonKhoTheoNhaMay.setGhiChu(ghichutemp);
            XtraRp_NhapXuatItem xtra_ = new XtraRp_NhapXuatItem();
            xtra_.DataSource = lstitem;
            xtra_BaoCaoTonKhoTheoNhaMay.setSubItem(xtra_);
            // XRSubreport xRSubreport_cb = xtra_BaoCaoTonKhoTheoNhaMay.FindControl("xrSubItem", true) as XRSubreport;
            //// XtraRp_NhapXuatItem xtra_NghiemThuChuanBi = (XtraRp_NhapXuatItem)xRSubreport_cb.ReportSource;
            // xtra_NghiemThuChuanBi.DataSource = lstitem;

            xtra_BaoCaoTonKhoTheoNhaMay.DataSource = dtresult; //(DataTable)grvSP.VisibleItems;
            if (dtresult.Rows[dtresult.Rows.Count - 1]["TenSP"].ToString() == "TỔNG CỘNG")
            {
                dtresult.Rows.RemoveAt(dtresult.Rows.Count - 1);
            }
            xtra_BaoCaoTonKhoTheoNhaMay.DataSource = dtresult;
            ModelAdmin.mainLayout.showreportAsync(xtra_BaoCaoTonKhoTheoNhaMay);
        }
        private async Task printthekhotheomahangAsync(string MaHang)
        {
           await dxFlyoutchucnang.CloseAsync();
            xulydieukien();
            List<ParameterDefine> lstparaitem = new List<ParameterDefine>();
           
            foreach (ParameterDefine sqlParameter in lstpara)
            {
                ParameterDefine sqlParameter1 = new ParameterDefine();
                sqlParameter1.ParameterValue = sqlParameter.ParameterValue;
                sqlParameter1.ParameterName = sqlParameter.ParameterName;
                lstparaitem.Add(sqlParameter1);
            }

            lstitem.Clear();
            //string checkMaHang
            bool checkmahang = false;
           
             string  sqlmahang = string.Format("(select * from NvlNhapXuatItem where MaHang=N'{0}')",MaHang);
           if(khuvucselected.Any())
            {
                string dieukiensudung = xulydieukientinhtrangsudung();
                sqlmahang = string.Format("(select * from NvlNhapXuatItem {0} and MaHang=N'{1}')", dieukiensudung, MaHang);
            }

            string sql = string.Format(@"use NVLDB
                                   select qry.MaHang,qry.SLTonDau,qry.Ngay,qry.SLTonDau,qry.SLNhap,qry.SLXuat
                        ,sum(qry.SLTonDau+qry.SLNhap-qry.SLXuat) OVER (
                                    PARTITION BY qry.MaHang 
                                    ORDER BY qry.Ngay, qry.SLXuat ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                                ) AS SLTon
                        ,sum(qry.TTTonDau+TTTonKho) OVER (
                                    PARTITION BY qry.MaHang  
                                    ORDER BY qry.Ngay, qry.SLXuat ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                                ) AS TTTonKho
                        ,qry.MaGN,case when qry.MaGN='TonDau' then N'Tồn đầu kỳ' else ngn.TenGN  end as TenGN,hh.TenHang,hh.DVT
                        from
                        (select MaHang,DateAdd(dd,-1,@DateBegin) as Ngay,sum(SLNhap-SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,sum((SLNhap-SLXuat)*DonGia) as TTTonDau,0 as TTTonKho,'TonDau' as MaGN 
                         from {1} nxitem where SerialCT in (select Serial from  NvlNhapXuat where Ngay<@DateBegin {0})
                         group by MaHang
                         union all
                        select MaHang,qyryCT.Ngay,0 as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,0 as TTTonDau,sum((SLNhap-SLXuat)*DonGia) as TTTonKho,qyryCT.MaGN
                        from {1} nxitem inner join (select Serial,Ngay,MaGN from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd  {0}) as qyryCT
                         on nxitem.SerialCT=qyryCT.Serial group by MaHang,qyryCT.Ngay,qyryCT.MaGN)
                        as qry 
                        left join dbo.View_NoiGN ngn on qry.MaGN=ngn.MaGN
                        inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                        order by qry.MaHang,qry.Ngay,SLXuat", dieukien, sqlmahang);

            GetDataFromSql getDataFromSql= new GetDataFromSql();
            getDataFromSql.reportname = "Xtra_TheKhoTheoMaHang";
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);

            getDataFromSql.sql = sql;
            DataTable dttemp = new DataTable();
            dttemp.Columns.Add("GhiChu", typeof(string));
            DataRow dataRow=dttemp.NewRow();
            dataRow["GhiChu"] = ghichutemp;
            dttemp.Rows.Add(dataRow);
            getDataFromSql.dtparameter = prs.ConvertDataTableToJson(dttemp);
            ModelAdmin.mainLayout.showreportpdfAsync(getDataFromSql);

            //CallAPI callAPI = new CallAPI();
            //try
            //{
            //    PanelVisible = true;
            //    string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparaitem);
            //    if (json != "")
            //    {
            //        var query = JsonConvert.DeserializeObject<List<LoadItemTheKho>>(json);
            //        lstitem.AddRange(query);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
            //}
            //finally
            //{
            //    PanelVisible = false;
            //    StateHasChanged();
            //}
            //if (lstitem.Count > 0)
            //{
            //    Xtra_TheKhoTheoMaHang xtra_BaoCaoTonKhoTheoNhaMay = new Xtra_TheKhoTheoMaHang();
            //    xtra_BaoCaoTonKhoTheoNhaMay.setGhiChu(ghichutemp);
            //    xtra_BaoCaoTonKhoTheoNhaMay.setTonKho(lstitem[lstitem.Count - 1].SLTon,lstitem[lstitem.Count - 1].TTTonKho);
            //    xtra_BaoCaoTonKhoTheoNhaMay.DataSource = lstitem; //(DataTable)grvSP.VisibleItems;
            //    ModelAdmin.mainLayout.showreportAsync(xtra_BaoCaoTonKhoTheoNhaMay);
            //}
           
        }
        
        private async void printthekhoIDTem()
        {
            if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn kho"));
                return;
            }
            //lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
            //dieukien += " and MaKho=@MaKho";
            //ghichutemp += Environment.NewLine + "Kho: " + TenKho;


            await searchAsync();
            await LoadtItemAsyncIDTem();

            Xtra_TheKhoTheoNhaMay xtra_BaoCaoTonKhoTheoNhaMay = new Xtra_TheKhoTheoNhaMay();
            xtra_BaoCaoTonKhoTheoNhaMay.setGhiChu(ghichutemp);
            XtraRp_NhapXuatItemIDTem xtra_ = new XtraRp_NhapXuatItemIDTem();
            var querytotal = lstitem.GroupBy(p => p.MaHang).Select(p => new LoadItemTheKho { MaHang = p.Key,SerialCT=null,SerialLink=null,Ngay=null,TenGN="Tổng:", SLNhap = p.Sum(m => m.SLNhap), SLXuat = p.Sum(m => m.SLXuat) }).ToList();
            lstitem.AddRange(querytotal);
            xtra_.DataSource = lstitem;

            xtra_BaoCaoTonKhoTheoNhaMay.setSubItem(xtra_);
            // XRSubreport xRSubreport_cb = xtra_BaoCaoTonKhoTheoNhaMay.FindControl("xrSubItem", true) as XRSubreport;
            //// XtraRp_NhapXuatItem xtra_NghiemThuChuanBi = (XtraRp_NhapXuatItem)xRSubreport_cb.ReportSource;
            // xtra_NghiemThuChuanBi.DataSource = lstitem;
            if (dtresult.Rows[dtresult.Rows.Count - 1]["TenSP"].ToString() == "TỔNG CỘNG")
            {
                dtresult.Rows.RemoveAt(dtresult.Rows.Count - 1);
            }
            xtra_BaoCaoTonKhoTheoNhaMay.DataSource = dtresult;
            //xtra_BaoCaoTonKhoTheoNhaMay.DataSource = dtresult; //(DataTable)grvSP.VisibleItems;
            ModelAdmin.mainLayout.showreportAsync(xtra_BaoCaoTonKhoTheoNhaMay);
        }
        private async void printthekhoSoLo()
        {
            if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn kho"));
                return;
            }
            //lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
            //dieukien += " and MaKho=@MaKho";
            //ghichutemp += Environment.NewLine + "Kho: " + TenKho;


            await searchAsync();
            await LoadtItemAsyncSoLo();

            Xtra_TheKhoTheoNhaMay xtra_BaoCaoTonKhoTheoNhaMay = new Xtra_TheKhoTheoNhaMay();
            xtra_BaoCaoTonKhoTheoNhaMay.setGhiChu(ghichutemp);
            XtraRp_NhapXuatItemSoLo xtra_ = new XtraRp_NhapXuatItemSoLo();
            var querytotal = lstitem.GroupBy(p => p.MaHang).Select(p => new LoadItemTheKho { MaHang = p.Key, SerialCT = null, SerialLink = null, Ngay = null, SoPhieu = "Tổng:", SLNhap = p.Sum(m => m.SLNhap), SLXuat = p.Sum(m => m.SLXuat) }).ToList();
            lstitem.AddRange(querytotal);
            xtra_.DataSource = lstitem;

            xtra_BaoCaoTonKhoTheoNhaMay.setSubItem(xtra_);
            // XRSubreport xRSubreport_cb = xtra_BaoCaoTonKhoTheoNhaMay.FindControl("xrSubItem", true) as XRSubreport;
            //// XtraRp_NhapXuatItem xtra_NghiemThuChuanBi = (XtraRp_NhapXuatItem)xRSubreport_cb.ReportSource;
            // xtra_NghiemThuChuanBi.DataSource = lstitem;

            //xtra_BaoCaoTonKhoTheoNhaMay.DataSource = dtresult.Select("TenSP<>'TỔNG CỘNG'"); //(DataTable)grvSP.VisibleItems;
            if (dtresult.Rows[dtresult.Rows.Count-1]["TenSP"].ToString() == "TỔNG CỘNG")
            {
                dtresult.Rows.RemoveAt(dtresult.Rows.Count - 1);
            }
            xtra_BaoCaoTonKhoTheoNhaMay.DataSource = dtresult;
            ModelAdmin.mainLayout.showreportAsync(xtra_BaoCaoTonKhoTheoNhaMay);
        }
        private async void printXemHangNo(bool XuatTruNhap)
        {
            dtresult.Clear();
           
            if (string.IsNullOrEmpty(loaibaocao))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn loại báo cáo"));
                return;
            }
            if (dtpbegin == null || dtpend == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn ngày"));
                return;
            }
            lstpara.Clear();

            ghichu = "";
            dieukien = "";

            dieukienmahang = "";
            string sql = "";

            ghichutemp = string.Format("Từ {0} - {1}", dtpbegin.Value.ToString("dd/MM/yy"), dtpend.Value.ToString("dd/MM/yy"));

            lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
            lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
            headertondauky = "TRƯỚC " + dtpbegin.Value.ToString("dd/MM/yy");
            headerphatsinh = string.Format("PHÁT SINH" + Environment.NewLine + "{0} - {1}", dtpbegin.Value.ToString("dd/MM"), dtpend.Value.ToString("dd/MM"));

            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
            {
                dieukienmahang = " where hh.MaHang=@MaHang";
                lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItemShowcrr.MaHang));
                ghichutemp += Environment.NewLine + "Hàng hóa: " + nvlNhapXuatItemShowcrr.TenHang;
            }
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            {
                lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
                dieukien += " and MaKho=@MaKho";
                ghichutemp += Environment.NewLine + "Kho: " + TenKho;

            }
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.TenNhom))
            {
                lstpara.Add(new ParameterDefine("@MaNhom", nvlNhapXuatItemShowcrr.TenNhom));
                if (dieukienmahang != "")
                    dieukienmahang += " and nh.MaNhom=@MaNhom";
                else
                    dieukienmahang += " where nh.MaNhom=@MaNhom";
                ghichutemp += Environment.NewLine + "Nhóm: " + nvlNhapXuatItemShowcrr.TenNhom;
            }
            string textno = "";
            if(XuatTruNhap)
            {
                textno = @"(qrytk.SLTonDau+qrytk.SLXuat-qrytk.SLNhap)";
                titleton = "Nợ cuối kỳ (Xuất - Nhập)";
            }
            else
            {
                textno = @"(qrytk.SLTonDau-qrytk.SLXuat+qrytk.SLNhap)";
                titleton = "Nợ cuối kỳ (Nhập - Xuất)";
            }
           // lstpara.Add(new ParameterDefine("@textno", textno));
                titlegroupsp = "Kho";
                sql = string.Format(@"use NVLDB declare @dtlydono Table(LyDo nvarchar(200) primary key)
            insert into @dtlydono(LyDo)
             SELECT [LyDo]
            FROM [dbo].[NvlNhapXuat_LyDo] where Tag='LyDoNo'
              group by LyDo

        select hh.MaHang,LyDo,hh.TenHang,hh.MaPDOC,qrytk.SLTonDau,0 as DonGia,qrytk.SLNhap,qrytk.SLXuat,{2} as SLTon
                             ,isnull(hh.MinTK,0) as MinTK,isnull(hh.MaxTK,0) as MaxTK,qrytk.MaKho,mk.TenKho,hh.DVT,LyDo as TenSP,mk.TenKho as GroupSP,nh.TenNhom
                             from 
                             (select LyDo,MaHang,MaKho,sum(SLTonDau) as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat
                             from 
                             (
                             select nx.LyDo,nxitem.MaHang,(nxitem.SLNhap-nxitem.SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,MaKho
                             from (select * from NvlNhapXuat where Ngay<@DateBegin and LyDo in (select LyDo from @dtlydono)   {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT
                             union all
                              select nx.LyDo,nxitem.MaHang,0 as SLTonDau,nxitem.SLNhap,nxitem.SLXuat,MaKho
                             from (select * from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd and LyDo in (select LyDo from @dtlydono)  {0}) nx
                             inner join dbo.NvlNhapXuatItem nxitem on nx.Serial=nxitem.SerialCT) as qrytotal
                             group by MaHang,MaKho,LyDo) as qrytk
                             inner join dbo.NvlHangHoa hh on qrytk.MaHang=hh.MaHang
                            inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                           
                             inner join dbo.NvlMaKho mk on qrytk.MaKho=mk.MaKho 
                              {1}
                              order by hh.MaHang", dieukien, dieukienmahang,textno);
            CallAPI callAPI = new CallAPI();
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    dtresult = JsonConvert.DeserializeObject<DataTable>(json);
                    List<string>lstcolumnname= new List<string>();
                    //Thêm tên cột dtresult vào danh sách lstcolumnname
                    foreach(DataColumn it in dtresult.Columns)
                    {
                        lstcolumnname.Add(it.ColumnName);
                    }
                    foreach(var it in lstcolumn)
                    {
                        //Kiểm tra xem cột nào trong dtresult khác với cột nào trong lstcolumn
                        var query = lstcolumnname.Where(x => x == it.Name).FirstOrDefault();//.(p => p.ColumnName == it.Name).FirstOrDefault();
                        if (query == null)
                        {
                            dtresult.Columns.Add(it.Name,it.DataType);
                        }
                    }
                    lstcolumnname.Clear();
                    dtresult.Columns.Add("lstitem", typeof(List<LoadItemTheKho>));
                    typeReport = "LyDoInput";
                    if (dtresult.Rows.Count > 0)
                    {
                        await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
                    }
                    // await GotoMainForm.InvokeAsync();
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
            }
            finally
            {
                dxGrid.Reload();
                PanelVisible = false;

                StateHasChanged();
            }
        }
        private async void printTheKhoDauMau()
        {
            if(dataRowViewcrr==null)
                return;
          string MaHang = dataRowViewcrr["MaHang"].ToString();
            string MaKho = dataRowViewcrr["MaKho"].ToString();
            //if(string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            //{
            //    toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn kho"));
            //    return;
            //}
            await dxFlyoutchucnang.CloseAsync();

            string sql = string.Format(@"use NVLDB 
                         select qry.MaHang,SerialCT,qry.SLTonDau,qry.Ngay,qry.SLTonDau,qry.SLNhap,qry.SLXuat
                        ,sum(qry.SLTonDau+qry.SLNhap-qry.SLXuat) OVER (
                                    PARTITION BY qry.MaHang 
                                    ORDER BY qry.Ngay, qry.SLXuat ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                                ) AS SLTon
                        ,sum(qry.TTTonDau+TTTonKho) OVER (
                                    PARTITION BY qry.MaHang  
                                    ORDER BY qry.Ngay, qry.SLXuat ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                                ) AS TTTonKho
                        ,qry.MaGN,case when qry.MaGN='TonDau' then N'Tồn đầu kỳ' else ngn.TenGN  end as TenGN,hh.TenHang,hh.DVT,MaCT,SoLo,NgaySanXuat,NgayHetHan
                        from
                        (select MaHang,DateAdd(dd,-1,@DateBegin) as Ngay,sum(SLNhap-SLXuat) as SLTonDau,0 as SLNhap,0 as SLXuat,sum((SLNhap-SLXuat)*DonGia) as TTTonDau,0 as TTTonKho,'TonDau' as MaGN,'' as SoLo,'' as MaCT,0 as SerialCT,NULL as NgaySanXuat,NULL as NgayHetHan
                         from (select * from NvlNhapXuatItem where MaHang=@MaHang) nxitem where SerialCT in (select Serial from  NvlNhapXuat where Ngay<@DateBegin  and MaKho=@MaKho)
                         group by MaHang
                         union all
                        select MaHang,qyryCT.Ngay,0 as SLTonDau,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,0 as TTTonDau,sum((SLNhap-SLXuat)*DonGia) as TTTonKho,qyryCT.MaGN,SoLo,qyryCT.MaCT,SerialCT,min(nxitem.NgaySanXuat) as NgaySanXuat,min(NgayHetHan) as NgayHetHan
                        from (select * from NvlNhapXuatItem where MaHang=@MaHang) nxitem inner join (select Serial,Ngay,MaGN,MaCT from NvlNhapXuat where Ngay>=@DateBegin and Ngay<=@DateEnd   and MaKho=@MaKho) as qyryCT
                         on nxitem.SerialCT=qyryCT.Serial group by MaHang,qyryCT.Ngay,qyryCT.MaGN,SoLo,SerialCT,qyryCT.MaCT)
                        as qry 
                        left join dbo.View_NoiGN ngn on qry.MaGN=ngn.MaGN
                        inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                        order by qry.MaHang,qry.Ngay,SerialCT");
            //CallAPI callAPI = new CallAPI();
            try
            {
                PanelVisible = true;
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine() { ParameterName = "@MaKho", ParameterValue = MaKho });
                lstpara.Add(new ParameterDefine() { ParameterName = "@MaHang", ParameterValue = MaHang });
                lstpara.Add(new ParameterDefine() { ParameterName = "@DateBegin", ParameterValue = dtpbegin.Value.ToString("MM/dd/yyyy 00:00") });
                lstpara.Add(new ParameterDefine() { ParameterName = "@DateEnd", ParameterValue = dtpend.Value.ToString("MM/dd/yyyy 23:59") });

                GetDataFromSql getDataFromSql = new GetDataFromSql();
                getDataFromSql.sql = sql;
                getDataFromSql.reportname = "Xtra_TheKhoHoaChatTheoMaHang";
                getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
              
                DataTable dtghichu = new DataTable();
                dtghichu.Columns.Add("TenHang", typeof(string));
                dtghichu.Columns.Add("TenKho", typeof(string));
                dtghichu.Columns.Add("Ngay", typeof(string));
                DataRow dataRow = dtghichu.NewRow();
                dataRow["TenHang"] = string.Format("Tên hàng: {0} - {1}", dataRowViewcrr["TenHang"].ToString(), MaHang);
                dataRow["TenKho"] = string.Format("Kho: {0}", dataRowViewcrr["TenKho"].ToString());
                dataRow["Ngay"] = string.Format("Từ ngày: {0} - {1}", dtpbegin.Value.ToString("dd/MM/yy"), dtpend.Value.ToString("dd/MM/yy"));
                dtghichu.Rows.Add(dataRow);
                getDataFromSql.dtparameter = prs.ConvertDataTableToJson(dtghichu);
                await ModelAdmin.mainLayout.showreportpdfAsync(getDataFromSql);


                //string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                ////var quryhh=lstmahang.Where(p=>p.MaHang==MaHang).FirstOrDefault();
                //string ghichu=string.Format("Tên hàng: {0} - {1}", dataRowViewcrr["TenHang"].ToString(), MaHang);

                //ghichu +=Environment.NewLine+ string.Format("Kho: {0}", dataRowViewcrr["TenKho"].ToString());
                //ghichu +=Environment.NewLine+ string.Format("Từ ngày: {0} - {1}", dtpbegin.Value.ToString("dd/MM/yy"),dtpend.Value.ToString("dd/MM/yy"));

                //if (json != "")
                //{

                //    var query = JsonConvert.DeserializeObject<List<LoadItemTheKho>>(json);
                //    if (query.Any())
                //    {
                //        Xtra_TheKhoHoaChatTheoMaHang xtra_BaoCaoTonKhoTheoNhaMay = new Xtra_TheKhoHoaChatTheoMaHang();
                //        xtra_BaoCaoTonKhoTheoNhaMay.setGhiChu(ghichu);
                //        //xtra_BaoCaoTonKhoTheoNhaMay.setTonKho(lstitem[lstitem.Count - 1].SLTon, lstitem[lstitem.Count - 1].TTTonKho);
                //        xtra_BaoCaoTonKhoTheoNhaMay.DataSource = query; //(DataTable)grvSP.VisibleItems;
                //        ModelAdmin.mainLayout.showreportAsync(xtra_BaoCaoTonKhoTheoNhaMay);
                //    }
                //}

            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }
           
        }

        private void ExportXtraReportToHtml(XtraReport report)
        {
           

            //HtmlExportOptions options = new HtmlExportOptions();
            //options.PageHeader = HtmlPageHeader.OnEveryPage;
            //options.ExportMode = HtmlExportMode.SingleFile;

            //// Xuất báo cáo sang HTML
            //report.ExportToHtml("output.html", options);

            // Nếu cần, có thể xử lý việc lưu tệp hoặc trả về URL cho người dùng tải về
        }
    }
}
