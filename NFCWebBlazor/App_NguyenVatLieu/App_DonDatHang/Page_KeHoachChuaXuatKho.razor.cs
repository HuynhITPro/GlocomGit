using BlazorBootstrap;
using Blazored.Modal;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;

using NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using System.Collections.ObjectModel;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Page_KeHoachChuaXuatKho
    {

        [Inject] PreloadService PreloadService { get; set; }


        [Inject] ToastService ToastService { get; set; } = default!;
        bool IsMobile { get; set; } = false;
        List<DataDropDownList> lstphongban = new List<DataDropDownList>();
        List<DataDropDownList> querykho = new List<DataDropDownList>();
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
            lsttrangthai = Model.ModelData.GetDataDropDownListsAsync("NvlTrangThaiDeNghi").Result.AsEnumerable();
            lstmahang = await Model.ModelData.GetHangHoa();
            querykho = await Model.ModelData.GetKhoNvl();
            var queryselect = querykho.Select(p => new DataDropDownList { Name = p.FullName, FullName = p.FullName }).ToList();
            lstkhonvl = queryselect.ToList();
            lstphongban = await ModelData.Getlstnoigiaonhan();
        }
        protected override async Task OnInitializedAsync()
        {
            await loadAsync();
            // return base.OnInitializedAsync();
        }
        public string TextSoDeNghi(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
            switch (keHoachMuaHang_Show.LoaiKeHoach)
            {
                case "DeNghiMuaHang":
                    return "Đề nghị mua hàng số: " + keHoachMuaHang_Show.Serial.ToString();
                case "DeNghiXuatHang":
                    return "Đề nghị xuất hàng số: " + keHoachMuaHang_Show.Serial.ToString();
                case "KeHoachMuaHang":
                    return "Kế hoạch mua hàng số: " + keHoachMuaHang_Show.Serial.ToString();
                case "KeHoachSanXuat":
                    return "Kế hoạch mua hàng số: " + keHoachMuaHang_Show.Serial.ToString();
                default:
                    return "Đề nghị số: " + keHoachMuaHang_Show.Serial.ToString();
            }
        }
        public string TextNgayDeNghi(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
            return "Ngày đề nghị " + keHoachMuaHang_Show.NgayDN.Value.ToString("dd/MM/yy");
        }
        DieuKienTimKiem dieuKienTimKiem { get; set; } = new DieuKienTimKiem();
        public class DieuKienTimKiem
        {
            public int? SerialDN { get; set; }
            public string? NguoiDN { get; set; }
            public string? MaKho { get; set; }
            public string? TenKho { get; set; }
            public DateTime DateBegin { get; set; } = DateTime.Now.AddMonths(-2);
            public DateTime DateEnd { get; set; } = DateTime.Now;
            public string? MaHang { get; set; }
            public string? TrangThai { get; set; } = "Đề nghị chưa hoàn thành";
        }
        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NvlKeHoachMuaHangItemShow> lstmuahang { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetShow> lstkyduyet { get; set; }
            [JsonProperty("Table2")]
            public List<NvlKeHoachMuaHangItemTotalShow> lsttotal { get; set; }
        }
        public class CustomRootMaster
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<KeHoachMuaHang_Show> lstkehoachshow { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetShow> lstkyduyet { get; set; }
        }
        CustomRoot customRoot { get; set; }
        private async Task searchAsync()
        {

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukienphanloai = "";
            string dieukienmahang = "";
            if (lstdata == null)
                lstdata = new List<KeHoachMuaHang_Show>();
            lstdata.Clear();
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

                    declare @tblsanpham Table(SerialLink int,MaSP nvarchar(100),TenSP nvarchar(200))
					insert into @tblsanpham(SerialLink,MaSP,TenSP)
					select [SerialLink],qry.[MaSP],dt.TenSP from
					(SELECT  [SerialLink],[MaSP] FROM [NvlKeHoachMuaHang_DinhMuc]
					where TableName='NvlKehoachMuaHang' and SerialLink in (select Serial from @tbltheodoi)) as qry
					left join dbo.[GetSanPham]() dt on qry.MaSP=dt.MaSP

					declare @tblsanphamgr Table(SerialLink int,TenSP nvarchar(max))
					insert into @tblsanphamgr(SerialLink,TenSP)
					SELECT t.SerialLink,STUFF(
						(SELECT DISTINCT ';' + dt.TenSP
						 FROM @tblsanpham dt
						 WHERE dt.SerialLink = t.SerialLink
						 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 
						1, 1, '') AS TenSP
				FROM @tblsanpham t
				GROUP BY t.SerialLink

                    declare @tbltyle as Table(SerialDN int,DaHoanThanh float,Tong float)
                    insert into @tbltyle(SerialDN,DaHoanThanh,Tong)
                    
                    select SerialDN,1.0/count(*)*sum(TyLe) as  DaHoanThanh,count(*) as Tong   from

					(select MaHang,SerialDN, case when SoLuong=0 then 1 else (SoLuong-SLTheoDoi)/SoLuong end as TyLe
					from
                    (
                    select MaHang,SerialDN,sum(SLTheoDoi) as SLTheoDoi,sum(SoLuong) as SoLuong
                    from [NvlKeHoachMuaHangItem] 
                    where SerialDN in (select SerialDN from @tbltheodoi)
                    group by MaHang,SerialDN) as qrytheodoi) 
					as qrytl
                    group by SerialDN
                         
                        declare @tbl as Table(SerialLink int,[UserYeuCau] nvarchar(100),[UserDuyet] nvarchar(100),LoaiDuyet nvarchar(100),DaDuyet nvarchar(100),NgayApDung date,NgayKyDuyet date,RowNum int)
                        

						insert into @tbl(SerialLink, [UserYeuCau], [UserDuyet], [LoaiDuyet], [DaDuyet],NgayApDung, NgayKyDuyet,RowNum)

						select SerialLink, [UserYeuCau], [UserDuyet], [LoaiDuyet], [DaDuyet], NgayApDung, NgayKyDuyet,RowNum
						from
						(SELECT SerialLink, [UserYeuCau], [UserDuyet], [LoaiDuyet], [DaDuyet], NgayApDung, NgayKyDuyet,
							   ROW_NUMBER() OVER (PARTITION BY SerialLink ORDER BY NgayApDung ASC) AS RowNum
						FROM [NvlKyDuyet] where TableName=N'NvlKehoachMuaHang'  and SerialLink in (select Serial from @tbltheodoi))
						as qry  

                         Select ddh.*,gr.TenSP,'{1}'+isnull(usr.[PathImg],'UserImage/user.png') as PathImgTao,kv.TenKhuVuc,  kyduyet.DaDuyet, kyduyet.UserYeuCau, kyduyet.SerialLink,tbltd.DaHoanThanh as TyLe
                         from 
                         (Select * from  @tbltheodoi) as ddh 
                            left join dbo.NvlKhuVuc kv on ddh.KhuVuc=kv.MaKhuVuc
                          inner join --Lưu ý chỗ này là biến truyền vào theo điều kiện kết--
                          (select * from  @tbl where RowNum=1) as kyduyet on ddh.Serial = kyduyet.SerialLink
						  inner join @tbltyle tbltd on ddh.Serial=tbltd.SerialDN   left join DBMaster.dbo.Users usr on ddh.UserInsert=usr.UsersName
                      left join @tblsanphamgr gr on ddh.Serial=gr.SerialLink        
                    order by ddh.Serial
                     select tbl.SerialLink,tbl.UserDuyet,tbl.LoaiDuyet,usr.TenUser from @tbl tbl inner join DBMaster.dbo.Users usr on tbl.UserDuyet=usr.UsersName order by  tbl.SerialLink
                    ", dieukienchuahoanthanh, Model.ModelAdmin.pathurlfilepublic);

            PanelVisible = true;
            CallAPI callAPI = new CallAPI();
            try
            {

                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {
                    var custom = JsonConvert.DeserializeObject<CustomRootMaster>(json);
                    if (custom.lstkehoachshow != null)
                    {
                        bool bl = false;
                        foreach (var it in custom.lstkehoachshow)
                        {
                            bl = false;
                            foreach (var item in custom.lstkyduyet)
                            {
                                if (it.Serial == item.SerialLink)
                                {
                                    bl = true;
                                    if (item.LoaiDuyet == "Duyệt")
                                    {
                                        if (string.IsNullOrEmpty(it.NguoiDuyet))
                                        {
                                            it.NguoiDuyet = item.TenUser;
                                        }
                                        else
                                            it.NguoiDuyet += "," + item.TenUser;
                                    }
                                    if (item.LoaiDuyet == "Kiểm tra")
                                    {
                                        if (string.IsNullOrEmpty(it.NguoiKiem))
                                        {
                                            it.NguoiKiem = item.TenUser;
                                        }
                                        else
                                            it.NguoiKiem += "," + item.TenUser;
                                    }

                                }
                                else
                                {
                                    if (bl)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        lstdata.AddRange(custom.lstkehoachshow);
                        await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));

                    }
                    //var query = JsonConvert.DeserializeObject<List<KeHoachMuaHang_Show>>(json);


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
        List<NvlKeHoachMuaHangItemShow> lstprint = new List<NvlKeHoachMuaHangItemShow>();
        private async Task PrintAsync(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
            //IsOpenfly = false;
            //await dxFlyoutchucnang.CloseAsync();
            try
            {
                IsOpenfly = false;
                await dxFlyoutchucnang.CloseAsync();

                if (CheckPrint && string.IsNullOrEmpty(signalRConnect.PrinterID))
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Nhập tên máy in"));
                    return;
                }
                PanelVisible = true;


                //await dxFlyoutchucnang.CloseAsync();
                lstprint.Clear();
                string sql = string.Format(@"Use NVLDB

                select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT
                from 
                                (SELECT  [Serial],[SerialDN],[MaHang],MaSP
                                          ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet 
                                      FROM [NvlKeHoachMuaHangItem]
                                      Where SerialDN = @SerialDN) as qry
									  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                       inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom 

                 SELECT 
                                SerialLinkMaster,[UserDuyet],[LoaiDuyet],item.NgayInsert
                                 ,usr.TenUser
                              FROM (select  SerialLinkMaster,[UserDuyet],[LoaiDuyet],max(NgayInsert) as NgayInsert from 
                              [dbo].[NvlKyDuyetItem]
                                where SerialLinkMaster=@SerialDN group by SerialLinkMaster,[UserDuyet],[LoaiDuyet]
                              ) item
                              inner join DBMaster.dbo.Users usr on item.UserDuyet=usr.UsersName
                    ", keHoachMuaHang_Show.Serial);

                switch (keHoachMuaHang_Show.LoaiKeHoach)
                {
                    case "KeHoachMuaHang":
                        sql = @"Use NVLDB

                                 IF OBJECT_ID('tempdb..#tmpitem') IS NOT NULL
                                 DROP TABLE #tmpitem
                                  IF OBJECT_ID('tempdb..#tmpitemdm') IS NOT NULL
                                 DROP TABLE #tmpitemdm
                                SELECT [MaHang],sum([SoLuong]) as SoLuong,KeyGroup
                                INTO #tmpitem
                                FROM [NvlKeHoachMuaHangItem]
                                Where SerialDN = @SerialDN
                                group by KeyGroup,[MaHang]

                                select tmitem.MaHang,sum(tmitem.SoLuong) as SoLuong,khdm.CongDoan,khdm.TenDinhMuc,khdm.IDKeHoach,sum(khdm.SoLuong) as SoLuongSP
                                into #tmpitemdm
                                from #tmpitem tmitem left join dbo.NvlKeHoachMuaHang_DinhMuc khdm on tmitem.KeyGroup=khdm.KeyGroup
                                group by tmitem.MaHang,khdm.CongDoan,khdm.TenDinhMuc,khdm.IDKeHoach

                                 declare @lstkehoach nvarchar(max)
                                  --SELECT @lstkehoach = COALESCE(@lstkehoach + ';', '') + CAST(ID AS NVARCHAR)
                                  --from
                                  --(select Distinct(IDKeHoach) as ID
                                  --FROM #tmpitemdm) as qry

                            SELECT @lstkehoach = STUFF((
                            SELECT ';' + CAST(ID AS NVARCHAR)
                            FROM (select Distinct(IDKeHoach) as ID
                                  FROM #tmpitemdm) as qry
                            FOR XML PATH('')
                                ), 1, 1, '')
                                 declare @tblkh table(ID nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(200),KhuVucKH nvarchar(100),MaMauKH nvarchar(100),SoLuongSP float,TenMau nvarchar(100),Color nvarchar(10))
                                 insert into @tblkh(ID,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color)
                                 exec SP.DataBase_ScansiaPacific2014.dbo.KeHoachSP_NVLListID @lstkehoach=@lstkehoach



                                select qry.MaHang,qry.SoLuong,qry.CongDoan,TenDinhMuc,IDKeHoach,isnull(tblkh.MaSP,'')+'-'+TenDinhMuc+'-'+ISNULL(tblkh.MaMauKH,'')+'-'+isnull(TenDinhMuc,'')+'-'+isnull(CongDoan,'') as PhanLoai,case when qry.IDKeHoach is not null then qry.IDKeHoach else  N'- Đề nghị ngoài kế hoạch * '+isnull(qry.MaSP,'') end as TenLienKet,hh.TenHang,hh.DVT,isnull(tblkh.TenSP,N'Đề nghị ngoài kế hoạch') as TenSP,cast(isnull(qry.SoLuongSP,0) as int) as SoLuongSP,tblkh.TenMau
                                from 
                                (SELECT  * from #tmpitemdm) as qry
                                inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                left join @tblkh tblkh on qry.IDKeHoach=tblkh.ID
                            SELECT 
                                SerialLinkMaster,[UserDuyet],[LoaiDuyet],item.NgayInsert
                                 ,usr.TenUser
                              FROM (select  SerialLinkMaster,[UserDuyet],[LoaiDuyet],max(NgayInsert) as NgayInsert from 
                              [dbo].[NvlKyDuyetItem]
                                where SerialLinkMaster=@SerialDN group by SerialLinkMaster,[UserDuyet],[LoaiDuyet]
                              ) item
                              inner join DBMaster.dbo.Users usr on item.UserDuyet=usr.UsersName
                                 DROP TABLE #tmpitem
                                  DROP TABLE #tmpitemdm
									                                  ";
                        break;
                    case "DeNghiXuatHangTheoKHSX":
                        sql = @"Use NVLDB

                                 IF OBJECT_ID('tempdb..#tmpitem') IS NOT NULL
                                         DROP TABLE #tmpitem
                                          IF OBJECT_ID('tempdb..#tmpdinhmuc') IS NOT NULL
                                         DROP TABLE #tmpdinhmuc

                                        SELECT [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
                                        ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet,KeyGroup
                                         INTO #tmpitem FROM [NvlKeHoachMuaHangItem]
                                         Where SerialDN = @SerialDN

	                                    SELECT SerialLink, TableName, KeyGroup, TenDinhMuc, CongDoan, sum(SoLuong) as SoLuong,
                                        STUFF((
                                            SELECT ';' + CAST(t1.IDKeHoach AS NVARCHAR)
                                            FROM dbo.NvlKeHoachMuaHang_DinhMuc t1
                                            WHERE SerialLink=@SerialDN and  t1.SerialLink = t2.SerialLink 
                                                AND t1.TableName = t2.TableName 
                                                AND t1.KeyGroup = t2.KeyGroup 
                                                AND t1.TenDinhMuc = t2.TenDinhMuc 
                                                AND t1.CongDoan = t2.CongDoan 
                                       FOR XML PATH('')), 1, 1, '') AS IDKeHoach
                                    INTO #tmpdinhmuc
                                    FROM dbo.NvlKeHoachMuaHang_DinhMuc t2
                                    WHERE TableName = 'NvlKehoachMuaHang' AND SerialLink = @SerialDN
                                    GROUP BY SerialLink, TableName, KeyGroup, TenDinhMuc, CongDoan

                                        
                                        declare @lstkehoach nvarchar(max)
                                         SELECT @lstkehoach = COALESCE(@lstkehoach + ';', '') + CAST(ID AS NVARCHAR)
                                         from
                                         (select Distinct(IDKeHoach) as ID
                                         FROM #tmpdinhmuc) as qry

									
                                        declare @tblkh table(ID nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(200),KhuVucKH nvarchar(100),MaMauKH nvarchar(100),SoLuongSP float,TenMau nvarchar(100),Color nvarchar(10))
                                        
                                        if(@lstkehoach is not null)
                                        begin           
                                            insert into @tblkh(ID,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color)
                                            exec SP.DataBase_ScansiaPacific2014.dbo.KeHoachSP_NVLListID @lstkehoach=@lstkehoach
                                        end
										declare @tblkehoachfinal Table(ID nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(200),KhuVucKH nvarchar(100),MaMauKH nvarchar(100),SoLuongSP float,TenMau nvarchar(100),Color nvarchar(10))
										insert into @tblkehoachfinal(ID,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color)
										
										select IDKeHoach,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color
										from
										(
										(select tmpdm.IDKeHoach,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color, 
										ROW_NUMBER() OVER (PARTITION BY tmpdm.IDKeHoach ORDER BY tmpdm.IDKeHoach ASC) AS RowNum
										from #tmpdinhmuc tmpdm inner join @tblkh kh on tmpdm.IDKeHoach like '%'+kh.ID+'%')
										) as qry where RowNum=1
									
									
										

                                        select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT,isnull(qrytk.SLTon,0) as SLTon,hh.MinTK,hh.MaxTK,tblkh.MaSP,tblkh.TenSP,khdm.TenDinhMuc,khdm.CongDoan,khdm.IDKeHoach,qry.KeyGroup,cast(khdm.SoLuong as int) as SoLuongSP,tblkh.TenMau,tblkh.Color,case when tblkh.ID is not null then tblkh.ID else  N'- Đề nghị ngoài kế hoạch * '+isnull(tblkh.MaSP,'') end as TenLienKet from 
                                                (SELECT  [Serial],[SerialDN],[MaHang],isnull(STT,0) as STT
                                                          ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,KeyGroup
										  
                                                      FROM [NvlKeHoachMuaHangItem]
                                                      Where SerialDN = @SerialDN) as qry
									                  left join (select MaHang,SLTon from dbo.GetTonKhoEx()) as qrytk
										                on qry.MaHang=qrytk.MaHang
									                  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                                       inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom 
									
									                  left join #tmpdinhmuc khdm on qry.KeyGroup=khdm.KeyGroup
									                  left join @tblkehoachfinal tblkh on khdm.IDKeHoach=tblkh.ID
									                 order by qry.STT,qry.Serial


                               SELECT 
                                SerialLinkMaster,[UserDuyet],[LoaiDuyet],item.NgayInsert
                                 ,usr.TenUser
                              FROM (select  SerialLinkMaster,[UserDuyet],[LoaiDuyet],max(NgayInsert) as NgayInsert from 
                              [dbo].[NvlKyDuyetItem]
                                where SerialLinkMaster=@SerialDN group by SerialLinkMaster,[UserDuyet],[LoaiDuyet]
                              ) item
                              inner join DBMaster.dbo.Users usr on item.UserDuyet=usr.UsersName

                        --Xử lý phần tổng hợp
							  declare @tbl as Table(MaHang nvarchar(100) primary key,SerialDN int)

							  insert into @tbl(MaHang,SerialDN)
							  select MaHang,SerialDN from NvlKeHoachMuaHangItem where SerialDN=@SerialDN group by MaHang,SerialDN



				                select tmp.MaHang,tmp.SLDeNghi as SoLuong,qrytotal.KhoNo,qrytotal.XuongNo,hh.TenHang,hh.DVT from

				                (select MaHang,sum(SoLuong) as SLDeNghi from #tmpitem group by MaHang) tmp left join
				                  (select qry.MaHang,sum(qry.KhoNo) as KhoNo,sum(XuongNo) as XuongNo from 
				                  (select MaHang,sum(SLTheoDoi) as KhoNo,0 as XuongNo
				                  from NvlKeHoachMuaHangItem 
				                  where  MaHang in (select MaHang from @tbl) and SLTheoDoi>0
				                  and SerialDN in (select Serial from NvlKehoachMuaHang where Serial<@SerialDN and LoaiKeHoach in ('DeNghiXuatHangTheoKHSX','DeNghiXuatHang'))
                                   and Serial in (select SerialLinkItem from NvlKyDuyetItem  where LoaiDuyet=N'Duyệt' and TableName='NvlKehoachMuaHang')
				                  group by MaHang
				                  union all
				                  SELECT [MaHang],0 as KhoNo,sum([SLGhiNo]-[SLTra]) as XuongNo
				                FROM [NvlNhapXuatGhiNo] where MaHang in (select MaHang from @tbl) 
				                group by MaHang
				                  ) as qry 
				 
				                  group by MaHang) as qrytotal on tmp.MaHang=qrytotal.MaHang
				                  inner join dbo.NvlHangHoa hh on tmp.MaHang=hh.MaHang

                                 DROP TABLE #tmpitem
                                  DROP TABLE #tmpdinhmuc
									                                  ";
                        break;
                    case "DeNghiMuaHang":

                        break;
                    case "DeNghiXuatHang":
                        sql = @"Use NVLDB

                                   IF OBJECT_ID('tempdb..#tmpitem') IS NOT NULL
                                 DROP TABLE #tmpitem
                                  IF OBJECT_ID('tempdb..#tmpitemdm') IS NOT NULL
                                 DROP TABLE #tmpitemdm
                                SELECT [MaHang],sum([SoLuong]) as SoLuong,KeyGroup
                                INTO #tmpitem
                                FROM [NvlKeHoachMuaHangItem]
                                Where SerialDN = @SerialDN
                                group by KeyGroup,[MaHang]

								 select tmitem.MaHang,sum(tmitem.SoLuong) as SoLuong,khdm.CongDoan,khdm.TenDinhMuc,khdm.IDKeHoach,khdm.MaSP,sp.TenSP,khdm.MaMau,sum(khdm.SoLuong) as SoLuongSP
                                into #tmpitemdm
                                from #tmpitem tmitem left join dbo.NvlKeHoachMuaHang_DinhMuc khdm on tmitem.KeyGroup=khdm.KeyGroup
								left join dbo.GetSanPham() sp on khdm.MaSP=sp.MaSP
                                group by tmitem.MaHang,khdm.CongDoan,khdm.TenDinhMuc,khdm.IDKeHoach,khdm.MaSP,khdm.MaMau,sp.TenSP

								

                                    --Xử lý kiểm tra xem đề nghị này có lẫn lộn giữa đề nghị theo định mức và đề nghị lẻ tẻ không
                                    declare @checkdinhmuc int
                                    select top 1 @checkdinhmuc=Serial from NvlKeHoachMuaHang_DinhMuc where SerialLink=@SerialDN and TableName='NvlKehoachMuaHang'
                                    if(@checkdinhmuc is null) --Nếu là đề nghị không có định mức
                                    begin
                                    select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT
                                    from 
                                                    (SELECT  [Serial],[SerialDN],[MaHang],MaSP
                                                              ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet 
                                                          FROM [NvlKeHoachMuaHangItem]
                                                          Where SerialDN = @SerialDN) as qry
								                                      inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                                           inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom


                                    end
                                    else --Nếu là đề nghị có chứa định mức
                                    begin

                                

                               

                                 declare @lstkehoach nvarchar(max)
                                 -- SELECT @lstkehoach = COALESCE(@lstkehoach + ';', '') + CAST(ID AS NVARCHAR)
                                  --from
                                 -- (select Distinct(IDKeHoach) as ID
                                  --FROM #tmpitemdm) as qry
                            SELECT @lstkehoach = STUFF((
                            SELECT ';' + CAST(ID AS NVARCHAR)
                            FROM (select Distinct(IDKeHoach) as ID
                                  FROM #tmpitemdm) as qry
                            FOR XML PATH('')
                                ), 1, 1, '')

                                select qry.MaHang,qry.SoLuong,qry.CongDoan,TenDinhMuc,IDKeHoach,isnull(IDKeHoach,'')+'-'+isnull(TenDinhMuc,'')+'-'+isnull(CongDoan,'') as PhanLoai,case when qry.IDKeHoach is not null then qry.IDKeHoach else  N'- Đề nghị ngoài kế hoạch * '+isnull(qry.MaSP,'') end as TenLienKet,hh.TenHang,hh.DVT,isnull(qry.TenSP,N'Đề nghị ngoài kế hoạch') as TenSP,qry.MaSP,cast(isnull(qry.SoLuongSP,0) as int) as SoLuongSP,qry.MaMau
                                from 
                                (SELECT  * from #tmpitemdm) as qry
                                inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                             
                       
                               
	                        end	
							 SELECT 
                                SerialLinkMaster,[UserDuyet],[LoaiDuyet],item.NgayInsert
                                 ,usr.TenUser
                              FROM (select  SerialLinkMaster,[UserDuyet],[LoaiDuyet],max(NgayInsert) as NgayInsert from 
                              [dbo].[NvlKyDuyetItem]
                                where SerialLinkMaster=@SerialDN group by SerialLinkMaster,[UserDuyet],[LoaiDuyet]
                              ) item
                              inner join DBMaster.dbo.Users usr on item.UserDuyet=usr.UsersName

                            --Xử lý phần tổng hợp
							  declare @tbl as Table(MaHang nvarchar(100) primary key,SerialDN int)

							  insert into @tbl(MaHang,SerialDN)
							  select MaHang,SerialDN from NvlKeHoachMuaHangItem where SerialDN=@SerialDN group by MaHang,SerialDN



				                select tmp.MaHang,tmp.SLDeNghi as SoLuong,qrytotal.KhoNo,qrytotal.XuongNo,hh.TenHang,hh.DVT from

				                (select MaHang,sum(SoLuong) as SLDeNghi from #tmpitemdm group by MaHang) tmp left join
				                  (select qry.MaHang,sum(qry.KhoNo) as KhoNo,sum(XuongNo) as XuongNo from 
				                  (select MaHang,sum(SLTheoDoi) as KhoNo,0 as XuongNo
				                  from NvlKeHoachMuaHangItem 
				                  where  MaHang in (select MaHang from @tbl) and SLTheoDoi>0
				                  and SerialDN in (select Serial from NvlKehoachMuaHang where Serial<@SerialDN and LoaiKeHoach in ('DeNghiXuatHangTheoKHSX','DeNghiXuatHang'))
                                and Serial in (select SerialLinkItem from NvlKyDuyetItem  where LoaiDuyet=N'Duyệt' and TableName='NvlKehoachMuaHang')
				                  group by MaHang
				                  union all
				                  SELECT [MaHang],0 as KhoNo,sum([SLGhiNo]-[SLTra]) as XuongNo
				                FROM [NvlNhapXuatGhiNo] where MaHang in (select MaHang from @tbl) 
				                group by MaHang
				                  ) as qry 
				 
				                  group by MaHang) as qrytotal on tmp.MaHang=qrytotal.MaHang
				                  inner join dbo.NvlHangHoa hh on tmp.MaHang=hh.MaHang
  

                                 DROP TABLE #tmpitem
                                  DROP TABLE #tmpitemdm
                            

                           ";
                        break;

                    default:

                        break;
                }

                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialDN", keHoachMuaHang_Show.Serial));
                //string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);

                if (!CheckPrint || string.IsNullOrEmpty(signalRConnect.PrinterID))
                {
                    string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                    if (json != "")
                    {
                        var custom = JsonConvert.DeserializeObject<CustomRoot>(json);
                        var query = custom.lstmuahang;
                        foreach (var it in query)
                        {
                            if (!string.IsNullOrEmpty(it.TenMau))
                            {
                                it.TenMau = StaticClass.RemoveVietnamese(it.TenMau);//Remove dấu tiếng việt đi để làm  TenMau trong xtrareport ko bị lỗi khi xuất trang In ra máy in, còn nguyên nhân thì ko hiểu tại sao
                            }
                        }
                        lstprint.AddRange(query);

                        //kehoachshowcrr.lstitem.AddRange(query);
                        if (custom.lstkyduyet != null)
                        {
                            foreach (var it in custom.lstkyduyet)
                            {
                                if (it.LoaiDuyet == "Duyệt")
                                {
                                    if (string.IsNullOrEmpty(keHoachMuaHang_Show.NguoiDuyet))
                                        keHoachMuaHang_Show.NguoiDuyet = it.TenUser;
                                    else
                                    {
                                        if (!keHoachMuaHang_Show.NguoiDuyet.Contains(it.TenUser))
                                            keHoachMuaHang_Show.NguoiDuyet += ";" + it.TenUser;
                                    }

                                }

                                if (it.LoaiDuyet == "Kiểm tra")
                                {
                                    if (string.IsNullOrEmpty(keHoachMuaHang_Show.NguoiKiem))
                                    {
                                        keHoachMuaHang_Show.NguoiKiem = it.TenUser;
                                    }
                                    else
                                    {
                                        if (!keHoachMuaHang_Show.NguoiKiem.Contains(it.TenUser))
                                            keHoachMuaHang_Show.NguoiKiem += ";" + it.TenUser;
                                    }

                                }

                            }
                        }
                        printreport(keHoachMuaHang_Show, custom);


                    }

                }
                else
                {
                    sql = sql + " " + Environment.NewLine + string.Format(@" select N'{0}' as PhongBan,N'{1}'  as NguoiDN,N'{2}' as NguoiKiem,N'{3}' as NguoiDuyet,N'{4}' as LyDo,N'{5}' as NoiDungDeNghi,N'{6}' as NgayDN,{7} as Serial", keHoachMuaHang_Show.PhongBan, keHoachMuaHang_Show.NguoiDN, keHoachMuaHang_Show.NguoiKiem, keHoachMuaHang_Show.NguoiDuyet, keHoachMuaHang_Show.LyDo, keHoachMuaHang_Show.NoiDungDeNghi, keHoachMuaHang_Show.NgayDN.Value.ToString("O"), keHoachMuaHang_Show.Serial);//Chuyển NgayDN về dạng datetime theo chuẩn ISO
                    string json = await callAPI.ExcuteQueryDatasetMsgEncrypt(sql, lstpara, signalRConnect.PrinterID, "denghixuatkho");
                }
                PanelVisible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }



        }
        private async void printreport(KeHoachMuaHang_Show kehoachshowcrr, CustomRoot customRootit)
        {
            var parameters = new ModalParameters();
            string diengialydo = "";
            diengialydo = string.Format("Lý do cấp vật tư: {0}", kehoachshowcrr.LyDo);
            if(!string.IsNullOrEmpty(kehoachshowcrr.NoiDung))
            {
                diengialydo += Environment.NewLine + "Nội dung: " + kehoachshowcrr.NoiDung;
            }
            switch (kehoachshowcrr.LoaiKeHoach)
            {
                case "KeHoachMuaHang":
                    XtraRp_KeHoachMuaHang xtra_KTGTonKho = new XtraRp_KeHoachMuaHang();
                    xtra_KTGTonKho.DataSource = lstprint;
                    xtra_KTGTonKho.setNoidung(kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                    //xtra_KTGTonKho.setTotal(querytotal[0].SoLuong, querytotal[0].ThanhTien);
                    xtra_KTGTonKho.setMaDeNghi(kehoachshowcrr.MaDN, kehoachshowcrr.Serial);
                    xtra_KTGTonKho.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, kehoachshowcrr.NoiDungDeNghi);
                    //parameters.Add("report", xtra_KTGTonKho);
                    //modal.Show<ReportShow>("", parameters, options);

                    await ModelAdmin.mainLayout.showreportAsync(xtra_KTGTonKho);
                    break;
                case "DeNghiMuaHang":
                    App_NguyenVatLieu.Report.XtraRp_DuTruVatTu xtraRp_DuTruVatTu = new XtraRp_DuTruVatTu();
                    xtraRp_DuTruVatTu.DataSource = lstprint;
                    xtraRp_DuTruVatTu.setNoidung((kehoachshowcrr.NoiDung == null) ? "" : kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                    xtraRp_DuTruVatTu.setMaDeNghi(kehoachshowcrr.Serial.ToString());
                    xtraRp_DuTruVatTu.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, kehoachshowcrr.NoiDungDeNghi);
                    //parameters.Add("report", xtraRp_DuTruVatTu);
                    //modal.Show<ReportShow>("", parameters, options);
                    await ModelAdmin.mainLayout.showreportAsync(xtraRp_DuTruVatTu);
                    break;
                case "DeNghiXuatHang":
                    var querycheckform = lstprint.Where(x => !string.IsNullOrEmpty(x.CongDoan)).FirstOrDefault();
                    if (querycheckform == null)
                    {
                        //Đề nghị có chứa định mức
                        XtraRp_DeNghiCapVatTu xtraRp_DeNghiCapVatTu = new XtraRp_DeNghiCapVatTu();
                        XRSubreport xrqtitem1 = xtraRp_DeNghiCapVatTu.FindControl("xrSubreport1", true) as XRSubreport;
                        //  XtraRp_DonDatHangItem xtraRp_DonDatHangItem = (XtraRp_DonDatHangItem)xrqtitem.ReportSource;
                        XtraRp_DeNghiXuatKho_Total xtra_KTGTonKhotoatl1 = (XtraRp_DeNghiXuatKho_Total)xrqtitem1.ReportSource;
                        xtra_KTGTonKhotoatl1.setdiengiai(string.Format("(Số lượng nợ cũ trước Đề nghị số {0})", kehoachshowcrr.Serial));
                        xtra_KTGTonKhotoatl1.DataSource = customRootit.lsttotal;
                        xtraRp_DeNghiCapVatTu.DataSource = lstprint;
                        xtraRp_DeNghiCapVatTu.setNoidung((kehoachshowcrr.NoiDung == null) ? "" : kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                        xtraRp_DeNghiCapVatTu.setMaDeNghi(kehoachshowcrr.Serial.ToString());
                        xtraRp_DeNghiCapVatTu.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, diengialydo, kehoachshowcrr.NgayDN);
                        //parameters.Add("report", xtra_KTGTonKho);
                        //modal.Show<ReportShow>("", parameters, options);
                        await ModelAdmin.mainLayout.showreportAsync(xtraRp_DeNghiCapVatTu);
                    }
                    else
                    {
                        XtraRp_DeNghiXuatKho rp_DeNghiXuatKhoDinhMuc = new XtraRp_DeNghiXuatKho();
                        XRSubreport xrqtitem1 = rp_DeNghiXuatKhoDinhMuc.FindControl("xrSubreport1", true) as XRSubreport;
                        //  XtraRp_DonDatHangItem xtraRp_DonDatHangItem = (XtraRp_DonDatHangItem)xrqtitem.ReportSource;
                        XtraRp_DeNghiXuatKho_Total xtra_KTGTonKhotoatl1 = (XtraRp_DeNghiXuatKho_Total)xrqtitem1.ReportSource;
                        //var querytotal1 = lstprint.GroupBy(p => new { MaHang = p.MaHang, TenHang = p.TenHang, DVT = p.DVT })
                        //   .Select(p => new { MaHang = p.Key.MaHang, TenHang = p.Key.TenHang, DVT = p.Key.DVT, SoLuong = p.Sum(n => n.SoLuong) }).OrderBy(p => p.MaHang).ToList();
                        xtra_KTGTonKhotoatl1.DataSource = customRootit.lsttotal;
                        xtra_KTGTonKhotoatl1.setdiengiai(string.Format("(Số lượng nợ cũ trước Đề nghị số {0})", kehoachshowcrr.Serial));
                        rp_DeNghiXuatKhoDinhMuc.DataSource = lstprint;
                        rp_DeNghiXuatKhoDinhMuc.setNoidung((kehoachshowcrr.NoiDung == null) ? "" : kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                        rp_DeNghiXuatKhoDinhMuc.setMaDeNghi(kehoachshowcrr.Serial.ToString());
                        rp_DeNghiXuatKhoDinhMuc.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, diengialydo , kehoachshowcrr.NgayDN);
                        //parameters.Add("report", xtra_KTGTonKho);
                        //modal.Show<ReportShow>("", parameters, options);
                        await ModelAdmin.mainLayout.showreportAsync(rp_DeNghiXuatKhoDinhMuc);
                    }
                    break;
                case "DeNghiXuatHangTheoKHSX":
                    XtraRp_DeNghiXuatKho rp_DeNghiXuatKho = new XtraRp_DeNghiXuatKho();
                    XRSubreport xrqtitem2 = rp_DeNghiXuatKho.FindControl("xrSubreport1", true) as XRSubreport;
                    //  XtraRp_DonDatHangItem xtraRp_DonDatHangItem = (XtraRp_DonDatHangItem)xrqtitem.ReportSource;
                    XtraRp_DeNghiXuatKho_Total xtra_KTGTonKhotoatl2 = (XtraRp_DeNghiXuatKho_Total)xrqtitem2.ReportSource;
                    //var querytotal = lstprint.GroupBy(p => new { MaHang = p.MaHang, TenHang = p.TenHang, DVT = p.DVT })
                    //  .Select(p => new { MaHang = p.Key.MaHang, TenHang = p.Key.TenHang, DVT = p.Key.DVT, SoLuong = p.Sum(n => n.SoLuong) }).OrderBy(p => p.MaHang).ToList();
                    xtra_KTGTonKhotoatl2.DataSource = customRootit.lsttotal;
                    rp_DeNghiXuatKho.DataSource = lstprint;
                    xtra_KTGTonKhotoatl2.setdiengiai(string.Format("(Số lượng nợ cũ trước Đề nghị số {0})", kehoachshowcrr.Serial));
                    rp_DeNghiXuatKho.setNoidung((kehoachshowcrr.NoiDung == null) ? "" : kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                    rp_DeNghiXuatKho.setMaDeNghi(kehoachshowcrr.Serial.ToString());
                    rp_DeNghiXuatKho.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, diengialydo, kehoachshowcrr.NgayDN);
                    //parameters.Add("report", xtra_KTGTonKho);
                    //modal.Show<ReportShow>("", parameters, options);
                    await ModelAdmin.mainLayout.showreportAsync(rp_DeNghiXuatKho);
                    break;
                default:
                    XtraRp_DeNghiCapVatTu xtraRp_DeNghiCapVatTu1 = new XtraRp_DeNghiCapVatTu();
                    XRSubreport xrqtitem3 = xtraRp_DeNghiCapVatTu1.FindControl("xrSubreport1", true) as XRSubreport;
                    //  XtraRp_DonDatHangItem xtraRp_DonDatHangItem = (XtraRp_DonDatHangItem)xrqtitem.ReportSource;
                    XtraRp_DeNghiXuatKho_Total xtra_KTGTonKhotoatl3 = (XtraRp_DeNghiXuatKho_Total)xrqtitem3.ReportSource;
                    //var querytotal = lstprint.GroupBy(p => new { MaHang = p.MaHang, TenHang = p.TenHang, DVT = p.DVT })
                    //  .Select(p => new { MaHang = p.Key.MaHang, TenHang = p.Key.TenHang, DVT = p.Key.DVT, SoLuong = p.Sum(n => n.SoLuong) }).OrderBy(p => p.MaHang).ToList();
                    xtra_KTGTonKhotoatl3.DataSource = customRootit.lsttotal;
                    xtraRp_DeNghiCapVatTu1.DataSource = lstprint;
                    xtraRp_DeNghiCapVatTu1.setNoidung((kehoachshowcrr.NoiDung == null) ? "" : kehoachshowcrr.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                    xtraRp_DeNghiCapVatTu1.setMaDeNghi(kehoachshowcrr.Serial.ToString());
                    xtraRp_DeNghiCapVatTu1.setNguoiDuyet(kehoachshowcrr.PhongBan, kehoachshowcrr.LoaiKeHoach, kehoachshowcrr.NguoiDN, kehoachshowcrr.NguoiKiem, kehoachshowcrr.NguoiDuyet, kehoachshowcrr.DaDuyet, diengialydo, kehoachshowcrr.NgayDN);
                    //parameters.Add("report", xtra_KTGTonKho);
                    //modal.Show<ReportShow>("", parameters, options);
                    await ModelAdmin.mainLayout.showreportAsync(xtraRp_DeNghiCapVatTu1);
                    break;
            }
            //customRootit.lstmuahang.Clear();
            //customRootit.lstkyduyet.Clear();
            //customRootit.lsttotal.Clear();




        }
        public async void ShowFlyout(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
            try
            {
                if (!dxFlyoutchucnang.IsInitialized)
                    await dxFlyoutchucnang.InitializedTask;
                await dxFlyoutchucnang.CloseAsync();
                keHoachMuaHangShowcrr = keHoachMuaHang_Show;
                idflychucnang = "#" + idelement(keHoachMuaHang_Show.Serial);



                if (!dxFlyoutchucnang.IsInitialized)
                {
                    Console.WriteLine("Init lại cái fly");
                    await dxFlyoutchucnang.InitializedTask;
                }


                await dxFlyoutchucnang.ShowAsync();
            }
            catch(Exception ex)
            {
                //ToastService.Notify(new ToastMessage(ToastType.Danger,"Lỗi: " + ex.Message));
                Console.Error.WriteLine("Lỗi ở flyout:"+ex.Message);
            }

        }

        private async void printDeNghiTonKho(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
            try
            {
                await dxFlyoutchucnang.CloseAsync();
                IsOpenfly = false;
                if (!lstdata.Any())
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, "Không có dữ liệu"));
                    return;
                }
                if (CheckPrint && string.IsNullOrEmpty(signalRConnect.PrinterID))
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Nhập tên máy in"));
                    return;
                }
                if (!CheckPrint || string.IsNullOrEmpty(signalRConnect.PrinterID))
                {
                    await loadtonkhohAsync(keHoachMuaHang_Show);

                    XtraRp_DeNghiCapVatTuViTri xtraRp_DeNghiCapVatTu1 = new XtraRp_DeNghiCapVatTuViTri();

                    xtraRp_DeNghiCapVatTu1.DataSource = ConvertToDataTable(keHoachMuaHang_Show.lsttemtonkho);
                    xtraRp_DeNghiCapVatTu1.setNoidung((keHoachMuaHang_Show.NoiDung == null) ? "" : keHoachMuaHang_Show.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                    xtraRp_DeNghiCapVatTu1.setMaDeNghi(keHoachMuaHang_Show.Serial.ToString());
                    xtraRp_DeNghiCapVatTu1.setNguoiDuyet(keHoachMuaHang_Show.PhongBan, keHoachMuaHang_Show.LoaiKeHoach, keHoachMuaHang_Show.NguoiDN, keHoachMuaHang_Show.NguoiKiem, keHoachMuaHang_Show.NguoiDuyet, keHoachMuaHang_Show.DaDuyet, keHoachMuaHang_Show.NoiDungDeNghi, keHoachMuaHang_Show.NgayDN);
                    //parameters.Add("report", xtra_KTGTonKho);
                    //modal.Show<ReportShow>("", parameters, options);
                   await ModelAdmin.mainLayout.showreportAsync(xtraRp_DeNghiCapVatTu1);
                }
                else
                {
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    string dieukien = " ";


                    dieukien = " where SerialKHDH=@SerialKHDH";
                    lstpara.Add(new ParameterDefine("@SerialKHDH", keHoachMuaHang_Show.Serial));
                    string sql = "";


                    //string[] arrshow = new string[] { "MaHang", "TenHang", "SerialCT", "SLNhap", "SLXuat", "DVT", "LyDo", "TenSP", "MaKien", "DauTuan", "SoLo", "TenGN", "SoXe", "GhiChu", "DonGia", "NgayHetHan", "SerialLink", "KhachHang_XuatXu", "UserInsert", "SerialKHDH", "SerialDN", "NguoiDeNghi", "TenLienKet", "NhaMay", "NgayInsert" };


                    sql = string.Format(@"use NVLDB

                                declare @SerialDN int={0}
                                declare @MaKhoEx nvarchar(100)=N'K011'

                                declare @tblserialdn table(MaHang nvarchar(100) primary key,SLDeNghi decimal(18,6))
                                insert into @tblserialdn(MaHang,SLDeNghi)
                                select MaHang,sum(SoLuong) as SLDeNghi from NvlKeHoachMuaHangItem where SerialDN=@SerialDN group  by MaHang

                                declare @tbltonkho Table(MaHang nvarchar(100),SLTon decimal(18,6),SerialLink int,Serial int primary key)
                                insert @tbltonkho (MaHang,SLTon,SerialLink,Serial)

                                select qry.MaHang,SLTon,SerialLink, Serial
                                from
                                (select MaHang,sum(SLNhap-SLXuat) as SLTon,SerialLink,min(case when SLNhap>0 then Serial end) as Serial
                                from NvlNhapXuatItem
                                where MaHang  in (select MaHang from @tblserialdn)
                                 and SerialCT in (select Serial from NvlNhapXuat where MaKho<>@MaKhoEx)
                                group by MaHang,SerialLink) as qry where SLTon<>0

                                select tbl.*,hh.TenHang,hh.DVT,item.ViTri,tbldn.SLDeNghi from @tbltonkho tbl
                                inner join dbo.NvlNhapXuatItem item on tbl.Serial=item.Serial
                                inner join dbo.NvlHangHoa hh on tbl.MaHang=hh.MaHang
                                inner join @tblserialdn tbldn on tbl.MaHang=tbldn.MaHang
                                order by ViTri


                     ", keHoachMuaHang_Show.Serial);
                    sql = sql + " " + Environment.NewLine + string.Format(@" select N'{0}' as PhongBan,N'{1}'  as NguoiDN,N'{2}' as NguoiKiem,N'{3}' as NguoiDuyet,N'{4}' as LyDo,N'{5}' as NoiDungDeNghi,N'{6}' as NgayDN,{7} as Serial", keHoachMuaHang_Show.PhongBan, keHoachMuaHang_Show.NguoiDN, keHoachMuaHang_Show.NguoiKiem, keHoachMuaHang_Show.NguoiDuyet, keHoachMuaHang_Show.LyDo, keHoachMuaHang_Show.NoiDungDeNghi, keHoachMuaHang_Show.NgayDN.Value.ToString("O"), keHoachMuaHang_Show.Serial);//Chuyển NgayDN về dạng datetime theo chuẩn ISO
                    CallAPI callAPI = new CallAPI();
                    string json = await callAPI.ExcuteQueryDatasetMsgEncrypt(sql, lstpara, signalRConnect.PrinterID, "denghixuatkhotonkho");

                }
            }
            catch(Exception ex)
            {

                Console.Error.WriteLine("Lỗi:" + ex.Message);
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
            }
        }
        private DataTable ConvertToDataTable(List<NvlNhapXuatItemTemTK> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TenHang", typeof(string));
            dt.Columns.Add("DVT", typeof(string));
            dt.Columns.Add("SLDeNghi", typeof(decimal));
            dt.Columns.Add("SerialLink", typeof(int));
            dt.Columns.Add("SerialLink1", typeof(int));
            dt.Columns.Add("ViTri", typeof(string));
            dt.Columns.Add("ViTri1", typeof(string));
            dt.Columns.Add("SLTon", typeof(decimal));
            dt.Columns.Add("SLTon1", typeof(decimal));
            var query = list.OrderBy(p => p.MaHang).ToList();
            string MaHangold = "";
            int old = 0;
            foreach(var it in query)
            {
                if(MaHangold!=it.MaHang)
                {
                    old = 1;
                    DataRow row = dt.NewRow();
                    row["TenHang"] =it.TenHang;
                    row["DVT"] = it.DVT;
                    row["SLDeNghi"] = it.SLDeNghi;
                    row["SerialLink"] = it.SerialLink;
                    row["ViTri"] = it.ViTri;
                    row["SLTon"] = it.SLTon;
                    dt.Rows.Add(row);
                }
                else
                {
                    if(old<2)
                    {
                        old++;
                        DataRow row = dt.Rows[dt.Rows.Count-1];
                       
                        row["SerialLink1"] = it.SerialLink;
                        row["ViTri1"] = it.ViTri;
                        row["SLTon1"] = it.SLTon;

                       
                    }
                    else
                    {
                        old = 1;
                        DataRow row = dt.NewRow();
                        row["TenHang"] = it.TenHang;
                        row["DVT"] = it.DVT;
                        row["SLDeNghi"] = it.SLDeNghi;
                        row["SerialLink"] = it.SerialLink;
                        row["ViTri"] = it.ViTri;
                        row["SLTon"] = it.SLTon;
                        dt.Rows.Add(row);
                        
                    }
                }
            }
            //int n = list.Count;
            //int half = (list.Count + 1) / 2; // Chia đôi danh sách
            //var firstHalf = list.Take(half).ToList();
            //var secondHalf = list.Skip(half).ToList();

            //for (int i = 0; i < half; i++)
            //{
            //    DataRow row = dt.NewRow();
            //    row["TenHang"] = firstHalf[i].TenHang;
            //    row["DVT"] = firstHalf[i].DVT;
            //    row["SLDeNghi"] = firstHalf[i].SLDeNghi;
            //    row["SerialLink"] = firstHalf[i].SerialLink;
            //    row["ViTri"] = firstHalf[i].ViTri;
            //    row["SLTon"] = firstHalf[i].SLTon;
            //    //row["SLDeNghi"] = firstHalf[i].SLDeNghi;

            //    if (i < secondHalf.Count)
            //    {
            //        row["SerialLink1"] = secondHalf[i].SerialLink;

            //        row["ViTri1"] = secondHalf[i].ViTri;
            //        row["SLTon1"] = secondHalf[i].SLTon;
            //        // row["SLDeNghi1"] = secondHalf[i].SLDeNghi;
            //    }
              

            //    dt.Rows.Add(row);
            //}
            //foreach(DataRow row in dt.Rows)
            //{
            //    Console.WriteLine(row["TenHang"].ToString());
            //}

            return dt;
        }
        private void createTaoDeNghiMuaHang()
        {

        }

        private async Task loadtonkhohAsync(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {

            if (keHoachMuaHang_Show.lsttemtonkho == null)
            {

                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string dieukien = " ";


                dieukien = " where SerialKHDH=@SerialKHDH";
                lstpara.Add(new ParameterDefine("@SerialKHDH", keHoachMuaHang_Show.Serial));
                string sqlSearch = "";


                //string[] arrshow = new string[] { "MaHang", "TenHang", "SerialCT", "SLNhap", "SLXuat", "DVT", "LyDo", "TenSP", "MaKien", "DauTuan", "SoLo", "TenGN", "SoXe", "GhiChu", "DonGia", "NgayHetHan", "SerialLink", "KhachHang_XuatXu", "UserInsert", "SerialKHDH", "SerialDN", "NguoiDeNghi", "TenLienKet", "NhaMay", "NgayInsert" };
                bool checkshow = false;

                sqlSearch = string.Format(@"use NVLDB

                                declare @SerialDN int={0}
                                declare @MaKhoEx nvarchar(100)=N'K011'

                                declare @tblserialdn table(MaHang nvarchar(100) primary key,SLDeNghi decimal(18,6))
                                insert into @tblserialdn(MaHang,SLDeNghi)
                                select MaHang,sum(SoLuong) as SLDeNghi from NvlKeHoachMuaHangItem where SerialDN=@SerialDN group  by MaHang

                                declare @tbltonkho Table(MaHang nvarchar(100),SLTon decimal(18,6),SerialLink int,Serial int primary key)
                                insert @tbltonkho (MaHang,SLTon,SerialLink,Serial)

                                select qry.MaHang,SLTon,SerialLink, Serial
                                from
                                (select MaHang,sum(SLNhap-SLXuat) as SLTon,SerialLink,isnull(min(case when SLNhap>0 then Serial end),0) as Serial
                                from NvlNhapXuatItem
                                where MaHang  in (select MaHang from @tblserialdn)
                                  and SerialCT in (select Serial from NvlNhapXuat where MaKho<>@MaKhoEx)
                                group by MaHang,SerialLink) as qry where SLTon<>0

                                select tbl.*,hh.TenHang,hh.DVT,item.ViTri,tbldn.SLDeNghi from @tbltonkho tbl
                                inner join dbo.NvlNhapXuatItem item on tbl.Serial=item.Serial
                                inner join dbo.NvlHangHoa hh on tbl.MaHang=hh.MaHang
                                inner join @tblserialdn tbldn on tbl.MaHang=tbldn.MaHang
                                order by ViTri


                     ", keHoachMuaHang_Show.Serial);
                CallAPI callAPI = new CallAPI();
                try
                {

                    string json = await callAPI.ExcuteQueryEncryptAsync(sqlSearch, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemTemTK>>(json);

                        if (query.Any())
                        {
                            keHoachMuaHang_Show.lsttemtonkho = new List<NvlNhapXuatItemTemTK>();
                            keHoachMuaHang_Show.lsttemtonkho.AddRange(query);
                            //var queryit = query.Where(p => p.MaHang == nvlKeHoachMuaHangItemShow.MaHang).ToList();

                            //lstdata = keHoachMuaHangcrr.lsttemtonkho;
                        }



                        // await GotoMainForm.InvokeAsync();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            StateHasChanged();

        }

        private async void ShowMasterAdd(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
            try
            {
                IsOpenfly = false;
                await dxFlyoutchucnang.CloseAsync();
                NvlNhapXuatKhoShow nVLDonDatHangShow = new NvlNhapXuatKhoShow();
                nVLDonDatHangShow.Serial = 0;
                nVLDonDatHangShow.Ngay = DateTime.Now;
                nVLDonDatHangShow.NhaMay = "Nhà máy A";
                var it = querykho.Where(p => p.FullName == keHoachMuaHangShowcrr.BoPhanMuaHang).FirstOrDefault();
                if (it != null)
                    nVLDonDatHangShow.MaKho = it.Name;
                string phongban = StaticClass.RemoveVietnamese(keHoachMuaHangShowcrr.PhongBan) + " - ";
                var pb = lstphongban.Where(p => p.FullName.StartsWith(phongban)).FirstOrDefault();
                if (pb != null)
                {
                    nVLDonDatHangShow.MaGN = pb.Name;
                }
                nVLDonDatHangShow.DienGiai = string.Format("Xuất theo đề nghị số {0}", keHoachMuaHangShowcrr.Serial);
                nVLDonDatHangShow.LyDo = "XUẤT KHO SẢN XUẤT";



                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_NhapXuatMasterAdd>(0);
                    builder.AddAttribute(1, "nvlNhapXuatKhoShowcrr", nVLDonDatHangShow);
                    builder.AddAttribute(2, "LoaiNhapXuat", "XuatKho");
                    //builder.AddAttribute(3, "AfterSave", EventCallback.Factory.Create<NvlNhapXuatKhoShow>(this, GotoMainFormAsync));
                    builder.CloseComponent();
                };

                await  dxPopup.showAsync("TẠO CHỨNG TỪ");
                await dxPopup.ShowAsync();
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine("Lỗi:" + ex.Message);
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
            }
        }

    }
}
