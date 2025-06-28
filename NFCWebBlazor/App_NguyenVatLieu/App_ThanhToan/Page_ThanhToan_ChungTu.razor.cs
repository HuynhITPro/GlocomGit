using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using NFCWebBlazor.Pages;
using SkiaSharp;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;
using static NFCWebBlazor.App_ThongTin.Urc_DinhMucHangHoaAdditem;

namespace NFCWebBlazor.App_NguyenVatLieu.App_ThanhToan
{
    public partial class Page_ThanhToan_ChungTu
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        bool Ismobile { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {

            try
            {
                if(nvlThanhToanShowcrr == null)
                {
                    nvlNhapXuatKhoShowcrr = new NvlNhapXuatKhoShow();
                    nvlNhapXuatKhoShowcrr.ThanhToan = "Chưa làm hồ sơ thanh toán";
                    nvlNhapXuatKhoShowcrr.LyDo = "NHẬP KHO MUA HÀNG";
                    lsttinhtrangtinhct =await ModelData.GetDataDropDownListsAsync("NvlTrangThaiChungTu");
                    lsttinhtrangtinhct.Add(new DataDropDownList() { Name = "Tất cả", FullName = "Tất cả",TypeName= "NvlTrangThaiChungTu" });

                }
                else
                {
                    SaveVisible = true;

                }
                //if(nvlThanhToanShowcrr==null)
                //{
                //    nvlThanhToanShowcrr = new Page_ThanhToanMaster.NvlThanhToanShow();
                //    nvlThanhToanShowcrr.LyDo = "NHẬP KHO MUA HÀNG";
                    
                //}
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
                await loadAsync();
                //if (heighrow!=null)
                //{
                //    height = dimension.Height - heighrow;
                //}
                Console.WriteLine("Init master Nhap Xuat");
                heightgrid = string.Format("{0}px", height);

                if (nvlThanhToanShowcrr != null)
                {
                   await searchasAsync();
                }
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
               
            }
           
            //await JS.InvokeVoidAsync("scrollToBottomLast");
            //base.OnAfterRender(firstRender);
        }



        private async Task loadAsync()
        {
            try
            {
                lstkhonvl = await Model.ModelData.GetKhoNvl();
                List<DataDropDownList> lst = await Model.ModelData.Getlstnvllydo();
                var queryngn = await Model.ModelData.Getlstnoigiaonhan();
                lstlydo = lst.Where(p => p.TypeName == "NhapKho").ToList();
                lstnoigiaonhan = queryngn.Where(p => p.TypeName == "NhaCungCap" || p.TypeName == "NB").ToList();
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
                await searchasAsync();

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
        List<NvlNhapXuatKhoShow> lstNhapXuatKhoSearchShow = new List<NvlNhapXuatKhoShow>();
        private async Task searchasAsync()
        {
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstNhapXuatKhoSearchShow.Clear();
            lstcolumn.Clear();
            string sql = "";
            //if (string.IsNullOrEmpty(nvlNhapXuatKhoShowcrr.ThanhToan))
            //{
            //    ToastService.Notify(new ToastMessage(ToastType.Warning, "Chọn tình trạng muốn xem "));
            //    return;
            //}
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
            if (sochungtu == null)
            {


                dieukien += " Where Ngay>=@DateBegin and Ngay<=@DateEnd and STTCT>0";

                if (!string.IsNullOrEmpty(nvlNhapXuatKhoShowcrr.MaKho))
                {

                    dieukien += " and MaKho = @MaKho";
                    lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatKhoShowcrr.MaKho));
                }
              
                if (!string.IsNullOrEmpty(nvlNhapXuatKhoShowcrr.LyDo))
                {
                    dieukien += " and LyDo = @LyDo";
                    lstpara.Add(new ParameterDefine("@LyDo", nvlNhapXuatKhoShowcrr.LyDo));
                }

                if (!string.IsNullOrEmpty(nvlNhapXuatKhoShowcrr.MaGN))
                {
                    dieukien += " and MaGN = @MaGN";
                    lstpara.Add(new ParameterDefine("@MaGN", nvlNhapXuatKhoShowcrr.MaGN));
                }


                lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
                lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
            }
            else
            {
                dieukien += " Where Serial = @Serial";

                lstpara.Add(new ParameterDefine("@Serial", sochungtu));
                lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
            }
            string dieukienthanhtoan = "";
            if (!string.IsNullOrEmpty(nvlNhapXuatKhoShowcrr.ThanhToan))
            {
                if (nvlNhapXuatKhoShowcrr.ThanhToan == "Chưa làm hồ sơ thanh toán")
                {
                    dieukienthanhtoan = " where tbl.ThanhToan is null";
                }
                if (nvlNhapXuatKhoShowcrr.ThanhToan == "Đã làm hồ sơ thanh toán")
                    dieukienthanhtoan = " where tbl.ThanhToan is not null";
                if (nvlNhapXuatKhoShowcrr.ThanhToan == "Chưa xác nhận")
                    dieukienthanhtoan = " where tbl.ThanhToan=N'Chưa xác nhận'";
                if(nvlNhapXuatKhoShowcrr.ThanhToan == "Đã xác nhận")
                    dieukienthanhtoan = " where tbl.ThanhToan=N'Đã xác nhận'";

                if (nvlNhapXuatKhoShowcrr.ThanhToan == "Tất cả")
                {

                }
                   
            }
            
            //sql = string.Format(@"use NVLDB 
            // declare @tbl as Table(SerialCT int,ThanhToan nvarchar(100),NguoiXacNhan nvarchar(100))
            //insert into @tbl(SerialCT,ThanhToan,NguoiXacNhan)
            //select SerialCT,case when tt.NguoiXacNhan is not null then N'Đã xác nhận' else N'Chưa xác nhận' end as ThanhToan,tt.NguoiXacNhan 
            //from NvlThanhToanItem it 
            //inner join (select * from dbo.NvlThanhToan where Ngay>=@DateBegin) tt on it.SerialTT=tt.Serial
           
             
            //Select cast(0 as bit) as CheckThanhToan,nx.*,ngn.TenGN, mk.TenKho,usr.TenUser,case when tbl.ThanhToan is null then N'Chưa làm hồ sơ thanh toán' else tbl.ThanhToan end as ThanhToan,tbl.NguoiXacNhan
            // FROM (select * from NvlNhapXuat  {0}
            // ) nx 
            // Inner join NvlMaKho mk on nx.MaKho = mk.MaKho inner join View_NoiGN ngn on nx.MaGN=ngn.MaGN 
            //inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName
            //left join @tbl tbl on nx.Serial=tbl.SerialCT
            //{1}
            //     ", dieukien,dieukienthanhtoan);

            sql = string.Format(@"use NVLDB
                                  IF OBJECT_ID('tempdb..#tmp') IS NOT NULL
									DROP TABLE #tmp
                                 IF OBJECT_ID('tempdb..#tblnt') IS NOT NULL
	                                DROP TABLE #tblnt
								
													
	                        --Tạo bảng #tmp để lưu những dòng cần
	                        CREATE TABLE #tmp([Serial] [int] primary key ,[STTCT] [int],
	                        [MaCT] [nvarchar](50),[MaKho] [nvarchar](100),[MaGN] [nvarchar](100) 
	                        ,[LyDo] [nvarchar](100) ,[PONumber] [nvarchar](100) ,
	                        [GhiChu] [nvarchar](250) ,[DienGiai] [nvarchar](200) ,
	                        [Ngay] [date] ,[Xacnhan] [int] ,[NguoiDN] [nvarchar](100) ,
	                        [MaDN] [nvarchar](100) ,[NhaMay] [nvarchar](100) ,[ChatLuong] [nvarchar](100) 
	                        ,[UserInsert] [nvarchar](100) ,[NgayInsert] [datetime] ,[NghiemThu] [nvarchar](100),[flag] [int])

                             --Dùng để xử lý nghiệm thu do ở máy chủ khác, lưu ý không nên truy vấn trực tiếp vào bảng của server khác qua Serverlink Oblink object, vì tốc độ rất chậm nếu data lớn
                              Insert into #tmp (Serial,[STTCT],[MaCT],[MaKho],[MaGN],[LyDo],[PONumber],[GhiChu],[DienGiai],[Ngay],[Xacnhan]
                                   ,[NguoiDN],[MaDN],[NhaMay]
                                   ,[ChatLuong],[UserInsert],[NgayInsert],[NghiemThu],[flag])
                             select Serial,[STTCT],[MaCT],[MaKho],[MaGN],[LyDo],[PONumber],[GhiChu],[DienGiai],[Ngay],[Xacnhan]
                                   ,[NguoiDN],[MaDN],[NhaMay]
                                   ,[ChatLuong],[UserInsert],[NgayInsert],[NghiemThu],[flag]
		                           from NvlNhapXuat  {0}
	
	                        --Vì bảng chất lượng thì định nghĩa chứng từ không kiểm thì các mã hàng bên trong phải không kiểm, nên mới phát sinh ra bảng này, nếu sau này thay đổi cách làm thì bỏ dòng truy vấn bên dưới
	                        declare  @tblnxitem as Table(SerialCT int,MaHang nvarchar(100),ThanhTien decimal(18,6)) 
	                        insert into @tblnxitem(SerialCT,MaHang,ThanhTien)
	                        select SerialCT,MaHang,sum((SLNhap+SLXuat)*DonGia) as ThanhTien from NvlNhapXuatItem nxitem where SerialCT in (select Serial from #tmp)
	                        group by SerialCT,MaHang

	                        -- danh sách những mã hàng không cần kiểm
	                        declare @tblmhknt as Table(MaHang nvarchar(100) primary key)
	                        insert into @tblmhknt(MaHang)
	                        select MaHang from SP.NguyenVatLieu.dbo.View_MaHangKhongNghiemThu
	
	                         -- chỉ cần 1 mã hàng nào đó trong chứng từ không có trong bảng @tblmhknt này, thì chứng từ đó phải kiểm
	                         declare @tblphaikiem Table(Serial int primary key)
	                         insert into @tblphaikiem(Serial)
	                         SELECT  SerialCT
	                        FROM @tblnxitem dt
	                        WHERE NOT EXISTS (
                            SELECT MaHang
                            FROM @tblmhknt tbl
                            WHERE dt.MaHang = tbl.MaHang)
	                        group by SerialCT

	                        --Lấy danh sách những chứng từ không cần kiểm
	                        declare @tblkhongcankiem table(Serial int)
	                        insert into @tblkhongcankiem(Serial)
	                        select DISTINCT dt.Serial from #tmp dt where Serial not in (select Serial from @tblphaikiem)

	                        -- Tạo bảng biến để lưu kết quả nghiệm thu từ function
                          create TABLE #tblnt(Serial int primary key, TinhTrang NVARCHAR(100))
                         declare @lstserial nvarchar(max)
                         --SELECT @lstserial = COALESCE(@lstserial + ';', '') + CAST(Serial AS NVARCHAR)
                        --FROM @tblphaikiem
                            
                        SELECT @lstserial = STUFF((
                            SELECT ';' + CAST(Serial AS NVARCHAR)
                            FROM @tblphaikiem
                            FOR XML PATH('')
                        ), 1, 1, '')


                         declare @sqlex nvarchar(max)
                         SET @sqlex = 'INSERT INTO #tblnt(Serial,TinhTrang)
                                             SELECT Serial,TinhTrang
                                             FROM OPENQUERY(SP, ''SELECT * FROM NguyenVatLieu.dbo.NvlChungTuNghiemThu(''''' + @lstserial + ''''')'')';
                                         EXEC sp_executesql @sqlex

	

                          --Đưa dữ liệu nghiệm thu vào bảng #tblnt
                         --Đưa dữ liệu nghiệm thu vào bảng #tblnt


		                                   declare @tbl as Table(SerialCT int,ThanhToan nvarchar(100),NguoiXacNhan nvarchar(100),MaCTTT nvarchar(100))
                                            insert into @tbl(SerialCT,ThanhToan,NguoiXacNhan,MaCTTT)
                                            select SerialCT,case when tt.NguoiXacNhan is not null then N'Đã xác nhận' else N'Chưa xác nhận' end as ThanhToan,tt.NguoiXacNhan,tt.MaCTTT
                                            from NvlThanhToanItem it 
                                            inner join (select * from dbo.NvlThanhToan where Ngay>=@DateBegin) tt on it.SerialTT=tt.Serial
           
             
                                            Select cast(0 as bit) as CheckThanhToan,nx.*,ngn.TenGN, mk.TenKho,usr.TenUser,case when tbl.ThanhToan is null then N'Chưa làm hồ sơ thanh toán' else tbl.ThanhToan end as ThanhToan,tbl.NguoiXacNhan,tbl.MaCTTT,
											
											case when tnlkk.Serial is not null then N'Không kiểm' else
											(case when nt.Serial is null then N'Chưa kiểm' else nt.TinhTrang end) 
											end as TinhTrang
											,ThanhTien
                                             FROM (select * from #tmp) nx 
                                             Inner join NvlMaKho mk on nx.MaKho = mk.MaKho inner join View_NoiGN ngn on nx.MaGN=ngn.MaGN 
                                            inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName
                                            left join @tbl tbl on nx.Serial=tbl.SerialCT
			                                left join #tblnt nt on nt.Serial=nx.Serial
											left join (select SerialCT,sum(ThanhTien) as ThanhTien from @tblnxitem group by SerialCT) as qrynxitem on nx.Serial=qrynxitem.SerialCT
                                            left join @tblkhongcankiem tnlkk on nx.Serial=tnlkk.Serial
											{1}

											 Drop Table #tmp
                                             DROP TABLE #tblnt
                                             
                                            ", dieukien, dieukienthanhtoan);
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<List<NvlNhapXuatKhoShow>>(json);
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

        public async void ShowFlyout(NvlNhapXuatKhoShow keHoachMuaHang_Show)
        {
            try
            {
                await dxFlyoutchucnang.CloseAsync();
                nvlNhapXuatKhoShowcrr = keHoachMuaHang_Show;
                idflychucnang = "#" + idelement(keHoachMuaHang_Show);

                // IsOpenfly = true;
                await dxFlyoutchucnang.ShowAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }

        }
      
        DataTable dtsave { get; set; }
        private async Task saveAsync()
        {
            bool bl = await phanQuyenAccess.CreateThanhToan(Model.ModelAdmin.users);
            if (!bl)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền lưu"));
                return;
            }
            
            string sql = "";
            CallAPI callAPI = new CallAPI();
            if (dtsave == null)
            {
                sql = @"use NVLDB declare @dt Type_NvlThanhToanItem
                        insert into @dt(Serial)
                        values(1)
                        select * from @dt";
                List<ParameterDefine> lstparadt = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparadt);
                if (json == "")
                    return;

              
                dtsave = JsonConvert.DeserializeObject<DataTable>(json);
                dtsave.Clear();
            }
            else
                dtsave.Clear();
            var querycheck = lstNhapXuatKhoSearchShow.Where(p => p.CheckThanhToan == true).ToList();
            foreach (var it in querycheck)
            {
                DataRow dataRow = dtsave.NewRow();
                dataRow["Serial"] = 0;
                dataRow["SerialTT"] = nvlThanhToanShowcrr.Serial;
                dataRow["SerialCT"] = it.Serial;
                dtsave.Rows.Add(dataRow);
            }
            sql = "NVLDB.dbo.NvlThanhToanItem_InsertTable";

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Type_NvlThanhToanItem", prs.ConvertDataTableToJson(dtsave), "DataTable"));
            lstpara.Add(new ParameterDefine("@UserInsert", Model.ModelAdmin.users.UsersName));
            lstpara.Add(new ParameterDefine("@SerialTT", nvlThanhToanShowcrr.Serial));
            try
            {
                string json = "";
                  json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketquatrave>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                        await  searchasAsync();
                    }
                    else
                    {
                       
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketqua));
                    }
                   
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
            }
        }

        public async Task GotoMainFormAsync(NvlNhapXuatKhoShow nVLDonDatHangShow)
        {
            await dxPopup.CloseAsync();
            try
            {
                sochungtu = null;
                await searchasAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi: {ex.Message}");
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: {ex.Message}"));
            }
            // dxGrid.Reload();
        }
    }
}
