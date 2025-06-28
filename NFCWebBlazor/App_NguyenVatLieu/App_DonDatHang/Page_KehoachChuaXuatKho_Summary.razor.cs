using BlazorBootstrap;
using Blazored.Modal;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;

using NFCWebBlazor.Model;

using System.Data;

using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;



namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Page_KehoachChuaXuatKho_Summary
    {
        [Inject] PreloadService PreloadService { get; set; }


        [Inject] ToastService ToastService { get; set; } = default!;
        bool IsMobile { get; set; } = false;

        public class NvlKeHoachMuaHangItemSummary
        {

            public int Serial { get; set; }
            public Nullable<int> SerialDN { get; set; }

          
            public string? MaHang { get; set; }

            public string TenSP
            {
                get; set;
            }
           
            public double? SoLuong { get; set; }
            public double? DBSoLuong {
                get
                {
                    if (SLQuyDoi == null)
                        return SoLuong;
                    if (SLQuyDoi>0)
                        return SoLuong/SLQuyDoi;
                    else
                        return SoLuong;
                }
            }
            public Nullable<double> SLTheoDoi { get; set; }
            public double? DBTheoDoi
            {
                get
                {
                    if (SLQuyDoi == null)
                        return SoLuong;
                    if (SLQuyDoi > 0)
                        return SLTheoDoi / SLQuyDoi;
                    else
                        return SLTheoDoi;
                }
            }
            public double?DBTon
            {
                get
                {
                    if (SLQuyDoi == null)
                        return SLTon;
                    if (SLQuyDoi > 0)
                        return SLTon / SLQuyDoi;
                    else
                        return SLTon;
                }
            }
            public Nullable<double> DonGia { get; set; }
            public string DVT { get; set; }
            public string TenHang { get; set; }
            public bool VisibleSL { get; set; }
         
            public Nullable<double> SLSuDung { get; set; }
            public Nullable<double> TyLe
            {
                get
                {

                    return ((SoLuong > 0) ? (SoLuong - SLTheoDoi) / SoLuong : 0);
                }
            }
         
           
        
       
          
            public string? MaSP { get; set; }
            public Nullable<int> SoLuongSP { get; set; }
            public uint? Color { get { return _color; } set { _color = value; Colorhex = StaticClass.UIntToHtmlColor(_color); Colortext = StaticClass.GetContrastColor(Colorhex); } }
            private uint? _color { get; set; }
            public string Colorhex
            {
                get; set;

            }
            public string Colortext { get; set; }
          public double? SLQuyDoi { get; set; }
           

        
            public string PhanLoai { get; set; }
            public Nullable<double> SLTon { get; set; }

            public Nullable<double> SLHuy { get; set; }

            double? _thanhtien { get; set; }
            public Nullable<double> ThanhTien
            {
                get
                {

                    return SoLuong * DonGia;

                }


            }
            public NvlKeHoachMuaHangItemSummary CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlKeHoachMuaHangItemSummary>(json);
            }
            public List<NvlKeHoachMuaHangItemShow> lstitem { get; set; }
            public string Err { get; set; }
           
            public List<NvlKyDuyetItemShow> lstduyetitem { get; set; }

            public string ForeGroundMsg { get; set; }

            public string KeyGroup { get; set; }
            public string CongDoan { get; set; }
            public string TenDinhMuc { get; set; }
            public string? TenMau { get; set; }
            public string? MaMau { get; set; }
        }
       

        private async Task loadAsync()
        {

            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 70;
            int width = dimension.Width;
            if (width < 768)
            {
                Ismobile = true;

            }
            else
                Ismobile = false;

            heightgrid = string.Format("{0}px", height);
            lstnguoidenghi = await Model.ModelData.Getlstusers();
            //lsttrangthai = Model.ModelData.GetDataDropDownListsAsync("NvlTrangThaiDeNghi").Result.AsEnumerable();

            //querykho = await Model.ModelData.GetKhoNvl();
            //var queryselect = querykho.Select(p => new DataDropDownList { Name = p.FullName, FullName = p.FullName }).ToList();
            //lstkhonvl = queryselect.ToList();
            //lstphongban = await ModelData.Getlstnoigiaonhan();
        }
        protected override async Task OnInitializedAsync()
        {
            await loadAsync();
            // return base.OnInitializedAsync();
        }
     
       

        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NvlKeHoachMuaHangItemSummary> lstdataitem { get; set; } = new List<NvlKeHoachMuaHangItemSummary>();

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<KeHoachMuaHang_Show> lstdenghi { get; set; } = new List<KeHoachMuaHang_Show>();
           
        }

        CustomRoot customRoot { get; set; } = new CustomRoot();


        private async Task searchAsync()
        {

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukienphanloai = "";
            string dieukienmahang = "";
            
            ListSerialDN = "";
            customRoot.lstdenghi.Clear();
            customRoot.lstdataitem.Clear();
            string dieukien = "";
            if (dieuKienTimKiem.SerialDN != null && dieuKienTimKiem.SerialDN > 0)
            {
                dieukien = string.Format(" and Serial =@SerialDN");
                lstpara.Add(new ParameterDefine("@SerialDN", dieuKienTimKiem.SerialDN));
                if (dieuKienTimKiem.DateBegin != null)
                {
                   
                    lstpara.Add(new ParameterDefine("@DateBegin", dieuKienTimKiem.DateBegin.ToString("MM/dd/yyyy 00:00")));
                }
                if (dieuKienTimKiem.DateEnd != null)
                {
                   
                    lstpara.Add(new ParameterDefine("@DateEnd", dieuKienTimKiem.DateEnd.ToString("MM/dd/yyyy 23:59")));
                }
              
            }
            else
            {

                if (!string.IsNullOrEmpty(dieuKienTimKiem.NguoiDN))
                {
                    var query = lstnguoidenghi.FirstOrDefault(p => p.UsersName.Equals(dieuKienTimKiem.NguoiDN));
                    if (query != null)
                    {
                        dieukien += " and NguoiDN=@NguoiDN";


                        lstpara.Add(new ParameterDefine("@NguoiDN", query.TenUser));
                    }
                }
                if (!string.IsNullOrEmpty(dieuKienTimKiem.TenKho))
                {
                    dieukien += " and BoPhanMuaHang=@TenKho";
                    lstpara.Add(new ParameterDefine("@TenKho", dieuKienTimKiem.TenKho));
                }
                if (dieuKienTimKiem.DateBegin != null)
                {
                    dieukien += " and NgayDN>=@DateBegin";
                    lstpara.Add(new ParameterDefine("@DateBegin", dieuKienTimKiem.DateBegin.ToString("MM/dd/yyyy 00:00")));
                }
                if (dieuKienTimKiem.DateEnd != null)
                {
                    dieukien += " and NgayDN<=@DateEnd";
                    lstpara.Add(new ParameterDefine("@DateEnd", dieuKienTimKiem.DateEnd.ToString("MM/dd/yyyy 23:59")));
                }
                if (!string.IsNullOrEmpty(dieuKienTimKiem.MaHang))
                {
                    dieukienmahang = " where MaHang=@MaHang";
                    lstpara.Add(new ParameterDefine("@MaHang", dieuKienTimKiem.MaHang));
                }
            }
            if (dieukien != "")
            {
                dieukien = " where " + dieukien.Substring(5);
            }
            string dieukienchuahoanthanh = "";
            string dieukiensltheodoi = "";
            if (dieuKienTimKiem.TrangThai == "Đề nghị đã hoàn thành")
            {
                dieukiensltheodoi = " where SLTheoDoi<=0.01";

            }
            else
            {
                dieukiensltheodoi = " where SLTheoDoi>0.01";
            }

            dieukienchuahoanthanh = string.Format(@" declare @tbltheodoi as Table([Serial] [int] primary key,[STT] [int] NULL,[MaDN] [nvarchar](100) NULL,[NguoiDN] [nvarchar](100) NULL,[LyDo] [nvarchar](100) NULL,[KhuVuc] [nvarchar](100) NULL,[NgayDN] [date] NULL,[PhongBan] [nvarchar](100) NULL,[NhaMay] [nvarchar](100) NULL,[NgayMax] [date] NULL,[NoiDung] [nvarchar](2000) NULL,[GhiChu] [nvarchar](100) NULL,[UserInsert] [nvarchar](100) NULL,[LoaiKeHoach] [nvarchar](100) NULL,[NgayInsert] [datetime] NULL,[NoiDungDeNghi] [nvarchar](1500) NULL,[BoPhanMuaHang] [nvarchar](150) NULL)

                    insert into @tbltheodoi(Serial,[STT],[MaDN],[NguoiDN],[LyDo],[KhuVuc],[NgayDN],[PhongBan],[NhaMay],[NgayMax],[NoiDung],[GhiChu],[UserInsert],[LoaiKeHoach],[NgayInsert],[NoiDungDeNghi],[BoPhanMuaHang])
                    select Serial,[STT],[MaDN],[NguoiDN],[LyDo],[KhuVuc],[NgayDN],[PhongBan],[NhaMay],[NgayMax],[NoiDung],[GhiChu],[UserInsert],[LoaiKeHoach],[NgayInsert],[NoiDungDeNghi],[BoPhanMuaHang] from NvlKeHoachMuaHang {0}
                    and LoaiKeHoach in ('DeNghiXuatHang','DeNghiXuatHangTheoKHSX')
                    and Serial in
                    (select SerialDN from (select SerialDN,max(SLTheoDoi) as SLTheoDoi 
                    from [NvlKeHoachMuaHangItem] {2}
                    group by SerialDN) as qry {1}) 
                                                and Serial in (SELECT SerialLinkMaster
						                      FROM [NvlKyDuyetItem]
						                      where TableName='NvlKehoachMuaHang' and LoaiDuyet=N'Duyệt'
						                      group by SerialLinkMaster)
					", dieukien, dieukiensltheodoi, dieukienmahang);//Không có trong danh sách những Đề nghị còn dở dang


            string sql = string.Format(@" use NVLDB
                    {0}
                    --Lưu ý Kế hoạch xuất hàng không cần ký duyệt
                  IF OBJECT_ID('tempdb..#tblsp') IS NOT NULL
	                DROP TABLE #tblsp

                    declare @sqlex nvarchar(max)
                    create table #tblsp(MaSP nvarchar(100),TenSP nvarchar(200),MaMau nvarchar(100),TenMau nvarchar(100),Color nvarchar(100))
                   SET @sqlex = 
                        'insert into #tblsp(MaSP,TenSP,MaMau,TenMau,Color)
                        select MaSP,TenSP,MaMau,TenMau,Color 
                         from OpenQuery(SP,
                        ''
                        SELECT MaSP, TenSP, MaMau, TenMau, Color
                        FROM (
                        (select sp.MaSP,sp.TenSP,art.MaMau,mm.TenMau,mm.Color, ROW_NUMBER() OVER (PARTITION BY sp.MaSP, art.MaMau ORDER BY sp.MaSP) AS rn
                        from DataBase_ScansiaPacific2014.dbo.SanPham sp
                        inner join DataBase_ScansiaPacific2014.dbo.ArticleNumberProduct art on sp.MaSP=art.MaSP
                        inner join DataBase_ScansiaPacific2014.dbo.MaMau mm on art.MaMau=mm.MaMau)) as qry WHERE rn = 1'')'
                    EXEC sp_executesql @sqlex                    

                    declare @tbldenghi table(MaHang nvarchar(100),SLDeNghi decimal(18,6),SLSuDung decimal(18,6),SLTheoDoi decimal(18,6),SLHuy decimal(18,6),MaSP nvarchar(100),MaMau nvarchar(100),CongDoan nvarchar(100),TenDinhMuc nvarchar(200))

                    insert into @tbldenghi(MaHang,SLDeNghi,SLSuDung,SLTheoDoi,MaSP,MaMau,CongDoan,TenDinhMuc,SLHuy)

                    SELECT [MaHang],sum(khitem.[SoLuong]) as SLDeNghi,sum(khitem.[SoLuong]-([SLTheoDoi]-SLHuy)) as SLSuDung,sum(case when SLTheoDoi<0 then 0 else SLTheoDoi end) as SLTheoDoi,khdm.MaSP,khdm.MaMau,khdm.CongDoan,khdm.TenDinhMuc,sum(khitem.SLHuy) as SLHuy

                      FROM [NvlKeHoachMuaHangItem] khitem
                      left join dbo.NvlKeHoachMuaHang_DinhMuc khdm on khitem.KeyGroup=khdm.KeyGroup
                      where khitem.SerialDN in (select Serial from @tbltheodoi) 
                      group by MaHang,khdm.MaSP,khdm.MaMau,khdm.CongDoan,khdm.TenDinhMuc

                      declare @lstsanpham nvarchar(max)
                      SELECT @lstsanpham=
                     STUFF((SELECT ';' + MaSP
                        FROM (SELECT DISTINCT isnull(MaSP,'') as MaSP FROM @tbldenghi) AS T
                        FOR XML PATH('')), 1, 1, '')
                    if(@lstsanpham is  null)
					    set @lstsanpham=''
                    EXEC GetDinhMucNVL_SanPhamList_TonKho  @lstsanpham=@lstsanpham,@dateend=@dateend

                  select ROW_NUMBER() OVER (ORDER BY dn.MaHang) AS Serial,dn.MaHang,dn.SLDeNghi as SoLuong,dn.SLHuy,dn.SLTheoDoi as SLTheoDoi,SLSuDung,dn.MaSP,dn.MaMau,isnull(sp.TenSP+' - '+sp.TenMau,N'z.Đề Nghị ngoài định mức') as PhanLoai,dn.CongDoan,dn.TenDinhMuc,dn.CongDoan+' - '+dn.TenDinhMuc as KeyGroup,sp.TenSP,sp.TenMau,sp.Color,qry.SLTon,hh.TenHang,hh.DVT,qry.SLQuyDoi
                    from @tbldenghi dn left join #tblsp sp on (sp.MaSP=dn.MaSP and sp.MaMau=dn.MaMau)
                    left join (select MaSP,GroupMauSP as MaMau,CongDoan,TenDinhMuc,Sum(SLQuyDoi) as SLQuyDoi,min(SLTon) as SLTon,MaVatTu from ##tmpdinhmuctoancuc group by MaSP,GroupMauSP,CongDoan,TenDinhMuc,MaVatTu) as qry
                    on (dn.MaSP=qry.MaSP and dn.CongDoan=qry.CongDoan and dn.TenDinhMuc=qry.TenDinhMuc and dn.MaMau=qry.MaMau and dn.MaHang=qry.MaVatTu)
                    inner join dbo.NvlHangHoa hh on dn.MaHang=hh.MaHang

                  select * from @tbltheodoi
                    drop table #tblsp 
                    DROP TABLE ##tmpdinhmuctoancuc", dieukienchuahoanthanh, Model.ModelAdmin.pathurlfilepublic);

            PanelVisible = true;
            CallAPI callAPI = new CallAPI();
            try
            {

                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {
                    var custom = JsonConvert.DeserializeObject<CustomRoot>(json);
                    customRoot.lstdataitem.AddRange(custom.lstdataitem);
                    customRoot.lstdenghi.AddRange(custom.lstdenghi);
                    var query = custom.lstdenghi.GroupBy(p => 1).Select(p => new { lstserial = string.Join(",", p.Select(x => x.Serial.ToString()))});
                    if (query.Count() > 0)
                    {
                        ListSerialDN = query.FirstOrDefault().lstserial;
                    }
                    //var query = JsonConvert.DeserializeObject<List<KeHoachMuaHang_Show>>(json);
                   await JSRuntime.InvokeVoidAsync(CallNameJson.toggleCollapse.ToString(), string.Format("#{0}", randomdivhide));

                    // await GotoMainForm.InvokeAsync();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi: " + ex.Message));

            }
            finally
            {
                PanelVisible = false;
                Grid.Reload();
                StateHasChanged();

            }
        }
        public async void ShowFlyout(NvlKeHoachMuaHangItemSummary nvlKeHoachMuaHangItemShow)
        {
            await dxFlyoutchucnang.CloseAsync();
            //IsOpenfly = false;

            idflychucnang = "#" + idelement(nvlKeHoachMuaHangItemShow.Serial);
            await loadtonkhohAsync(nvlKeHoachMuaHangItemShow);

           // IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();

        }
        private async Task loadtonkhohAsync(NvlKeHoachMuaHangItemSummary nvlKeHoachMuaHangItemShow)
        {
            lstdataitem.Clear();
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = " ";
            string sqlSearch = "";
            //string[] arrshow = new string[] { "MaHang", "TenHang", "SerialCT", "SLNhap", "SLXuat", "DVT", "LyDo", "TenSP", "MaKien", "DauTuan", "SoLo", "TenGN", "SoXe", "GhiChu", "DonGia", "NgayHetHan", "SerialLink", "KhachHang_XuatXu", "UserInsert", "SerialKHDH", "SerialDN", "NguoiDeNghi", "TenLienKet", "NhaMay", "NgayInsert" };
            bool checkshow = false;

            sqlSearch = string.Format(@"use NVLDB

                               
                                declare @MaHang nvarchar(100)=N'{0}'
                                declare @MaKhoEx nvarchar(100)='K011'
                                declare @tbltonkho Table(MaHang nvarchar(100),SLTon decimal(18,6),SerialLink int,Serial int primary key)
                                insert @tbltonkho (MaHang,SLTon,SerialLink,Serial)

                                select qry.MaHang,SLTon,SerialLink, Serial
                                from
                                (select MaHang,sum(SLNhap-SLXuat) as SLTon,SerialLink,min(case when SLNhap>0 then Serial end) as Serial
                                from NvlNhapXuatItem
                                where MaHang =@MaHang
                                 and SerialCT in (select Serial from NvlNhapXuat where MaKho<>@MaKhoEx)
                                group by MaHang,SerialLink) as qry where SLTon<>0

                                select tbl.*,hh.TenHang,hh.DVT,item.ViTri from @tbltonkho tbl
                                inner join dbo.NvlNhapXuatItem item on tbl.Serial=item.Serial
                                inner join dbo.NvlHangHoa hh on tbl.MaHang=hh.MaHang

                                order by ViTri

                     ", nvlKeHoachMuaHangItemShow.MaHang);
            CallAPI callAPI = new CallAPI();
            try
            {



                string json = await callAPI.ExcuteQueryEncryptAsync(sqlSearch, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemTemTK>>(json);
                    lstdataitem.AddRange(query);
                }
                StateHasChanged();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}

