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
using static NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Page_KeHoachChuaXuatKho;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Page_KeHoachChuaDatHang_DinhMucRp
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
            public double? DBSoLuong
            {
                get
                {
                    if (SLQuyDoi == null)
                        return SoLuong;
                    if (SLQuyDoi > 0)
                        return SoLuong / SLQuyDoi;
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
            public double? DBTon
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

                      declare @tblsanpham Table(SerialLink int,MaSP nvarchar(100),TenSP nvarchar(200),CongDoan nvarchar(100),SLCongDoan float)
					insert into @tblsanpham(SerialLink,MaSP,TenSP,CongDoan,SLCongDoan)
					select [SerialLink],qry.[MaSP],dt.TenSP,CongDoan,SoLuong from
					(SELECT  [SerialLink],[MaSP],CongDoan,TenDinhMuc,SoLuong FROM [NvlKeHoachMuaHang_DinhMuc]
					where TableName='NvlKehoachMuaHang' and SerialLink in (select Serial from @tbltheodoi)) as qry
					left join dbo.[GetSanPham]() dt on qry.MaSP=dt.MaSP

					

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

                         Select ddh.*,gr.TenSP,'{1}'+isnull(usr.[PathImg],'UserImage/user.png') as PathImgTao,kv.TenKhuVuc,  kyduyet.DaDuyet, kyduyet.UserYeuCau, kyduyet.SerialLink,tbltd.DaHoanThanh as TyLe,CongDoan,SLCongDoan
                         from 
                         (Select * from  @tbltheodoi) as ddh 
                            left join dbo.NvlKhuVuc kv on ddh.KhuVuc=kv.MaKhuVuc
                          inner join --Lưu ý chỗ này là biến truyền vào theo điều kiện kết--
                          (select * from  @tbl where RowNum=1) as kyduyet on ddh.Serial = kyduyet.SerialLink
						  inner join @tbltyle tbltd on ddh.Serial=tbltd.SerialDN   left join DBMaster.dbo.Users usr on ddh.UserInsert=usr.UsersName
                      left join @tblsanpham gr on ddh.Serial=gr.SerialLink        
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
            catch (Exception ex)
            {
                //ToastService.Notify(new ToastMessage(ToastType.Danger,"Lỗi: " + ex.Message));
                Console.Error.WriteLine("Lỗi ở flyout:" + ex.Message);
            }

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

