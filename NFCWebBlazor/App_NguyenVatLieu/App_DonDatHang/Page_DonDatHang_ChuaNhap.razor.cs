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

using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Page_DonDatHang_Master;


namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Page_DonDatHang_ChuaNhap
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
            lstnhacungcap = await Model.ModelData.Getlstnhacungcap();
        }
        protected override async Task OnInitializedAsync()
        {
            await loadAsync();
            // return base.OnInitializedAsync();
        }
       
       
        DieuKienTimKiem dieuKienTimKiem { get; set; } = new DieuKienTimKiem();
        public class DieuKienTimKiem
        {
            public int? SerialDN { get; set; }
            public string? NguoiDN { get; set; }
            public string? NhaCungCap { get; set; }
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
            public List<NVLDonDatHangShow> lstmuahang { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetShow> lstkyduyet { get; set; }
            //[JsonProperty("Table2")]
            //public List<NvlKeHoachMuaHangItemTotalShow> lsttotal { get; set; }
        }
        public class CustomRootMaster
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NVLDonDatHangShow> lstkehoachshow { get; set; }

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
                lstdata = new List<NVLDonDatHangShow>();
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
                        dieukien += " and UserInsert=@NguoiDN";


                        lstpara.Add(new ParameterDefine("@NguoiDN", query.UsersName));
                    }
                }
                //if (!string.IsNullOrEmpty(dieuKienTimKiem.TenKho))
                //{
                //    dieukien += " and BoPhanMuaHang=@TenKho";
                //    lstpara.Add(new ParameterDefine("@TenKho", dieuKienTimKiem.TenKho));
                //}
                if (dieuKienTimKiem.DateBegin != null)
                {
                    dieukien += " and NgayTao>=@DateBegin";
                    lstpara.Add(new ParameterDefine("@DateBegin", dieuKienTimKiem.DateBegin.ToString("MM/dd/yyyy 00:00")));
                }
                if (dieuKienTimKiem.DateEnd != null)
                {
                    dieukien += " and NgayTao<=@DateEnd";
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
            dieukienchuahoanthanh = string.Format(@" declare @tbltheodoi as Table
					 ([Serial] [int] primary key,PhongBan nvarchar(100),NgayTao date,NgayDatHang date,NgayMax date,GhiChu nvarchar(200),UserInsert nvarchar(100),DVTT nvarchar(100),NgayInsert datetime,[NoiDungDeNghi] nvarchar(150),MaNCC nvarchar(100),SoHD nvarchar(100))


                    insert into @tbltheodoi(Serial,PhongBan,NgayTao,NgayDatHang,NgayMax,GhiChu,UserInsert,DVTT,NgayInsert,[NoiDungDeNghi],MaNCC,SoHD)
                   SELECT  Serial,PhongBan,NgayTao,NgayDatHang,NgayMax,GhiChu,UserInsert,DVTT,NgayInsert,[NoiDungDeNghi],MaNCC,SoHD
					FROM [dbo].[NvlDonDatHang]  {0}
                   
                    and Serial in
                    (select SerialMaDH from (select SerialMaDH,max(SLTheoDoi) as SLTheoDoi 
                    from NvlDonDatHangItem {2}
                    group by SerialMaDH) as qry  {1}) 
                                                and Serial in 
												(SELECT SerialLinkMaster
						                      FROM [NvlKyDuyetItem]
						                      where TableName='NvlDonDatHang' and LoaiDuyet=N'Duyệt'
						                      group by SerialLinkMaster)", dieukien, dieukiensltheodoi, dieukienmahang);

     //       dieukienchuahoanthanh = string.Format(@" declare @tbltheodoi as Table([Serial] [int] primary key,[STT] [int] NULL,[MaDN] [nvarchar](100) NULL,[NguoiDN] [nvarchar](100) NULL,[LyDo] [nvarchar](100) NULL,[KhuVuc] [nvarchar](100) NULL,[NgayDN] [date] NULL,[PhongBan] [nvarchar](100) NULL,[NhaMay] [nvarchar](100) NULL,[NgayMax] [date] NULL,[NoiDung] [nvarchar](2000) NULL,[GhiChu] [nvarchar](100) NULL,[UserInsert] [nvarchar](100) NULL,[LoaiKeHoach] [nvarchar](100) NULL,[NgayInsert] [datetime] NULL,[NoiDungDeNghi] [nvarchar](1500) NULL,[BoPhanMuaHang] [nvarchar](150) NULL)

     //               insert into @tbltheodoi(Serial,[STT],[MaDN],[NguoiDN],[LyDo],[KhuVuc],[NgayDN],[PhongBan],[NhaMay],[NgayMax],[NoiDung],[GhiChu],[UserInsert],[LoaiKeHoach],[NgayInsert],[NoiDungDeNghi],[BoPhanMuaHang])
     //               select Serial,[STT],[MaDN],[NguoiDN],[LyDo],[KhuVuc],[NgayDN],[PhongBan],[NhaMay],[NgayMax],[NoiDung],[GhiChu],[UserInsert],[LoaiKeHoach],[NgayInsert],[NoiDungDeNghi],[BoPhanMuaHang] from NvlKeHoachMuaHang {0}
     //               and LoaiKeHoach in ('DeNghiXuatHang','DeNghiXuatHangTheoKHSX')
     //               and Serial in
     //               (select SerialDN from (select SerialDN,max(SLTheoDoi) as SLTheoDoi 
     //               from [NvlKeHoachMuaHangItem] {2}
     //               group by SerialDN) as qry {1}) 
     //                                           and Serial in (SELECT SerialLinkMaster
					//	                      FROM [NvlKyDuyetItem]
					//	                      where TableName='NvlKehoachMuaHang' and LoaiDuyet=N'Duyệt'
					//	                      group by SerialLinkMaster)
					//", dieukien, dieukiensltheodoi, dieukienmahang);//Không có trong danh sách những Đề nghị còn dở dang


            string sql = string.Format(@" use NVLDB
                    {0}
                    --Lưu ý Kế hoạch xuất hàng không cần ký duyệt

                    declare @tbltyle as Table(SerialMaDH int,DaHoanThanh float,Tong float)
                    insert into @tbltyle(SerialMaDH,DaHoanThanh,Tong)
                    
                    select SerialMaDH,1.0/count(*)*sum(TyLe) as  DaHoanThanh,count(*) as Tong   from

					(select MaHang,SerialMaDH, case when SoLuong=0 then 1 else (SoLuong-SLTheoDoi)/SoLuong end as TyLe
					from
                    (
                    select MaHang,SerialMaDH,sum(SLTheoDoi) as SLTheoDoi,sum(SLDatHang) as SoLuong
                    from NvlDonDatHangItem 
                    where SerialMaDH in (select SerialMaDH from @tbltheodoi)
                    group by MaHang,SerialMaDH) as qrytheodoi) 
					as qrytl
                    group by SerialMaDH
                         

                        declare @tbl as Table(SerialLink int,[UserYeuCau] nvarchar(100),[UserDuyet] nvarchar(100),LoaiDuyet nvarchar(100),DaDuyet nvarchar(100),NgayApDung date,NgayKyDuyet date,RowNum int)
                        

						insert into @tbl(SerialLink, [UserYeuCau], [UserDuyet], [LoaiDuyet], [DaDuyet],NgayApDung, NgayKyDuyet,RowNum)

						select SerialLink, [UserYeuCau], [UserDuyet], [LoaiDuyet], [DaDuyet], NgayApDung, NgayKyDuyet,RowNum
						from
						(SELECT SerialLink, [UserYeuCau], [UserDuyet], [LoaiDuyet], [DaDuyet], NgayApDung, NgayKyDuyet,
							   ROW_NUMBER() OVER (PARTITION BY SerialLink ORDER BY NgayApDung ASC) AS RowNum
						FROM [NvlKyDuyet] where TableName=N'NvlDonDatHang'  and SerialLink in (select Serial from @tbltheodoi))
						as qry  

                         Select ddh.*,'{1}'+isnull(usr.[PathImg],'UserImage/user.png') as PathImgTao,  kyduyet.DaDuyet, kyduyet.UserYeuCau, kyduyet.SerialLink,tbltd.DaHoanThanh as TyLe,ncc.TenNCC
                         from 
                         (Select * from  @tbltheodoi) as ddh 
						 left join dbo.NvlNhaCungCap ncc on ddh.MaNCC=ncc.MaNCC
                          
                          inner join --Lưu ý chỗ này là biến truyền vào theo điều kiện kết--
                          (select * from  @tbl where RowNum=1) as kyduyet on ddh.Serial = kyduyet.SerialLink
						  inner join @tbltyle tbltd on ddh.Serial=tbltd.SerialMaDH   left join DBMaster.dbo.Users usr on ddh.UserInsert=usr.UsersName
                    
                    order by ddh.Serial
                     select tbl.SerialLink,tbl.UserDuyet,tbl.LoaiDuyet,usr.TenUser from @tbl tbl inner join DBMaster.dbo.Users usr on tbl.UserDuyet=usr.UsersName order by  tbl.SerialLink", dieukienchuahoanthanh, Model.ModelAdmin.pathurlfilepublic);

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

        private async Task PrintAsync()
        {
            IsOpenfly = false;
            await dxFlyoutchucnang.CloseAsync();
            PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
            await Task.Delay(50);
            try
            {
                //await dxFlyoutchucnang.CloseAsync();


                string sql = "";

                //SqlConnection sqlConnection = prs.ConnectNVL();
                //sqlConnection.Open();

                string sqlghichu = "";


                sqlghichu = string.Format(@"
                            ", keHoachMuaHangShowcrr.Serial);
                sql = string.Format(@"
                                --Điều kiện ghi chú
                                use NVLDB
                                {1} 
                                 declare @NgayDatHang date
                declare @SerialDH int={0}
				 select @NgayDatHang=NgayDatHang from NvlDonDatHang where Serial=@SerialDH
                     Select donhangchitiet.Serial,MaHang,TenHang,SLDatHang,NoiDung,DVT,DonGia,case when MaHang is null then isnull([DonGia],0) else isnull([SLDatHang]*DonGia,0) end as ThanhTien,NgayDKNhapKho,GroupIndex,dbo.DonDatHangGroup([Group]) as [GroupName],PhanLoai,QuyCachChatLuong,TenNCC
                        ,donhangchitiet.MaNCC,isnull(qryncchd.SoHD,'') as SoHD,qryncchd.NgayHieuLuc,donhangchitiet.STT,	donhangchitiet.SerialMaDH
                       FROM
                       (SELECT dhitem.[Serial], dhitem.[SerialMaDH], dhitem.[MaHang], dhitem.[SLDatHang], dhitem.SignInt, dhitem.NoiDung,
                          dhitem.[SLTheoDoi], NvlHangHoa.[DVT],dhitem.[DonGia],dhitem.[SerialLink], 
                          dhitem.[MaNCC],dhitem.[NgayDKNhapKho],dhitem.[UserInsert],dhitem.[NgayInsert],
                          dhitem.[Group], dhitem.GroupIndex,dhitem.GiaTri,NvlHangHoa.TenHang, NvlNhaCungCap.TenNCC, ISNULL(dhitem.DonGia,0) * IsNull(dhitem.SLDatHang,0) as ThanhTien ,nh.PhanLoai
                           ,case when NvlHangHoa.QuyCach='' then NvlHangHoa.ChatLuong 
						            else (NvlHangHoa.QuyCach+case when NvlHangHoa.ChatLuong<>'' then + CHAR(13) + CHAR(10) + NvlHangHoa.ChatLuong else '' end) end as QuyCachChatLuong,dhitem.STT
                           from (SELECT [Serial], [SerialMaDH], [MaHang], [SLDatHang], 1 as SignInt, NULL as NoiDung,
                          [SLTheoDoi], [DVT],[DonGia],[SerialLink], 
                          [MaNCC],[NgayDKNhapKho],[UserInsert],[NgayInsert],
                          N'NvlDonDatHangItem' as [Group], 0 as [GroupIndex],GiaTri,isnull(STT,0) as STT
                   FROM [NvlDonDatHangItem]
                   Where SerialMaDH = @SerialDH ) dhitem INNER JOIN 
                   NvlHangHoa ON NvlHangHoa.MaHang = dhitem.MaHang
                   inner join dbo.NvlNhomHang nh on NvlHangHoa.MaNhom=nh.MaNhom
                   left JOIN NvlNhaCungCap ON NvlNhaCungCap.MaNCC = dhitem.MaNCC
      
               ) as donhangchitiet left join (SELECT [MaNCC],[SoHD],[NgayHieuLuc]
			             FROM [NvlNhaCungCapHD]
			            where Serial in (select max(Serial) from NvlNhaCungCapHD where NgayHieuLuc<=@NgayDatHang group by MaNCC))  as qryncchd
			            on donhangchitiet.MaNCC=qryncchd.MaNCC order  by STT
    
              select qry.*,usr.TenUser as UserYeuCau,isnull(NoiDungDeNghi,'') as NoiDungDeNghi,isnull(qry.GhiChu,'') as GhiChu
            ,ncc.MaNCC,ncc.TenNCC,ncc.DiaChi, isnull(ncc.DiDong,ncc.DienThoaiBan) as SoDienThoai,ncc.MaSoThue,ncc.Email,qryrp.*
            from 
            (SELECT  [Serial],[MaDatHang],[NgayDatHang],isnull(DVTT,N'VNĐ') as DVTT
            ,[NgayMax],[KhuVuc],[GhiChu],[UserInsert],isnull(NoiDungDeNghi,'') as NoiDungDeNghi,PhongBan,MaNCC
             FROM [NvlDonDatHang] where Serial=@SerialDH
             ) as qry 
             left join DBMaster.dbo.Users usr on qry.UserInsert=usr.UsersName
             left join dbo.NvlNhaCungCap ncc on qry.MaNCC=ncc.MaNCC
            left join dbo.GetReportDetail_DonDatHang(@SerialDH)  as qryrp on qry.Serial=qryrp.SerialLink
                                ", keHoachMuaHangShowcrr.Serial, sqlghichu);
                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();

                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {

                    DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);
                    if (dataSet == null)
                    {
                        return;
                    }
                    if (dataSet.Tables[0].Rows.Count == 0)
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                        return;
                    }
                    DataTable dtitem = dataSet.Tables[0];
                    DataTable dtmaster = dataSet.Tables[1];
                    var querysum = dtitem.AsEnumerable().Sum(p => p.Field<double>("ThanhTien"));

                    string bangchu = prs.docsothapphan(querysum);
                    if (dtmaster.Rows[0]["DVTT"].ToString() == "VNĐ")
                        bangchu = bangchu + " đồng";
                    else
                        bangchu = bangchu + dtmaster.Rows[0]["DVTT"].ToString();
                    XtraRp_DonDatHangNew xtra_KTGTonKho = new XtraRp_DonDatHangNew();

                    XRSubreport xrqtitem = xtra_KTGTonKho.FindControl("xrSubreport1", true) as XRSubreport;
                    XtraRp_DonDatHangItem xtraRp_DonDatHangItem = (XtraRp_DonDatHangItem)xrqtitem.ReportSource;
                    //xtraRp_DonDatHangItem.setGhiChu(dtmaster.Rows[0]["GhiChu"].ToString(), "");
                    xtra_KTGTonKho.setGhiChu(dtmaster.Rows[0]["GhiChu"].ToString(), "");
                    xtraRp_DonDatHangItem.setBangChu(bangchu, querysum);
                    xtraRp_DonDatHangItem.DataSource = dtitem;
                    //xtra_KTGTonKho.setNguoiDuyet("Phòng vật tư", nguoilap, nguoikiemtra, nguoiduyet, nvlDonDatHangShowcrr.DaDuyet);
                    xtra_KTGTonKho.DataSource = dtmaster;

                    //var querysum =customRootrp.lstitemrp.Sum(p => p.ThanhTien);

                    //    string bangchu = prs.docsothapphan(querysum.Value);
                    //    if (customRootrp.lstkyduyetrp[0].DVTT == "VNĐ")
                    //        bangchu = bangchu + " đồng";
                    //    else
                    //        bangchu = bangchu + customRootrp.lstkyduyetrp[0].DVTT;

                    string madenghi = string.Format("{0}/ĐĐH/{1}", prs.SerialLengthToString(keHoachMuaHangShowcrr.Serial, 4), keHoachMuaHangShowcrr.NgayDatHang.Value.ToString("yyyy"));
                    xtra_KTGTonKho.setMaDeNghi(madenghi);
                    Nullable<DateTime> ngayhieuluc;
                    ngayhieuluc = null;
                    xtra_KTGTonKho.setNoiDung(ngayhieuluc, "", dtmaster.Rows[0]["NoiDungDeNghi"].ToString(), "");
                    ModelAdmin.mainLayout.showreportAsync(xtra_KTGTonKho);

                    dataSet.Dispose();

                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi :" + ex.Message));
                Console.WriteLine("Lỗi:" + ex.Message);
            }
            finally
            {
                PreloadService.Hide();
            }
        }

        public async void ShowFlyout(NVLDonDatHangShow keHoachMuaHang_Show)
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
    }

}
