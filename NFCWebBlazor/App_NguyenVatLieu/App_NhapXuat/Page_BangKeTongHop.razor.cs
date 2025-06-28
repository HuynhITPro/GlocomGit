
using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_BangKeTongHop
    {
        [Inject]ToastService toastService { get; set; }

        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        protected override async Task OnInitializedAsync()
        {
            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 70;
            heightgrid = string.Format("{0}px", height);
           
            //// var heighrow = await browserService.GetHeighWithID("divcontainer");
            //int height = dimension.Height - 120;
            
            int width = dimension.Width;
            if (width < 768)
            {
                Ismobile = true;
                idgrid = "customGridnotheader";
              
            }
            else
            {
                idgrid = "griddetailnhapkho";
                Ismobile = false;
            }
           
            await loadAsync();
            //randomdivhide = prs.RandomString(10);
            //return base.OnInitializedAsync();
        }
        private void showhidecolumn(string loaibaocao)
        {
            bool bl = false;
            if (loaibaocao== "Theo mã hàng")
            {
                string[] arr = new string[] { "TenNhom", "MaHang", "TenHang", "DVT", "SLNhap", "SLXuat","DonGia","ThanhTien"};
                
                dxGrid.BeginUpdate();
                foreach(DxGridDataColumn dxGridColumn in dxGrid.GetColumns())
                {
                    bl = false;
                    foreach(var item in arr)
                    {
                        if(dxGridColumn.FieldName == item)
                        {
                            bl = true;
                            break;
                        }
                    }
                    dxGridColumn.Visible=bl;
                }
                dxGrid.EndUpdate();
            }
            if (loaibaocao== "Báo cáo chi tiết")
            {
                string[] arr = new string[] { "NgaySanXuat", "NgayHetHan", "KhachHang_XuatXu", "MaKien", "ArticleNumber", "SoXe", "NgayThanhToan", "NguoiThanhToan", "TinhTrang" };
                dxGrid.BeginUpdate();
                foreach (DxGridDataColumn dxGridColumn in dxGrid.GetColumns())
                {
                    bl = true;
                    foreach (var item in arr)
                    {
                        if (dxGridColumn.FieldName == item)
                        {
                            bl = false;// = true;
                            break;
                        }
                    }
                    dxGridColumn.Visible = bl;

                }
                dxGrid.EndUpdate();

            }
            if (loaibaocao == "Báo cáo kiểm hàng")
            {
                string[] arr = new string[] { "TinhTrang","Ngay", "MaCT", "SerialCT", "TenKho", "TenGN", "DonGia", "ThanhTien", "LyDo", "UserInsert", "NgayInsert", "TenNhom", "MaHang", "TenHang", "DVT", "SLNhap", "SLXuat","NgayThanhToan","NguoiThanhToan" };
                dxGrid.BeginUpdate();
                foreach (DxGridDataColumn dxGridColumn in dxGrid.GetColumns())
                {
                    bl = false;
                    foreach (var item in arr)
                    {
                        if (dxGridColumn.FieldName == item)
                        {
                            bl = true;// = true;
                            break;
                        }
                    }
                    dxGridColumn.Visible = bl;

                }
                dxGrid.EndUpdate();
            }
                StateHasChanged();
        }
        private async Task loadAsync()
        {
            lstkhonvl = await Model.ModelData.GetKhoNvl();
            lstlydo = await Model.ModelData.Getlstnvllydo();
            lstnoigiaonhan = await Model.ModelData.Getlstnoigiaonhan();
            
        }
        public async Task searchAsync()
        {
            if (lstdata == null)
                lstdata = new List<NvlNhapXuatItemShow>();
            lstdata.Clear();
           
            if (dtpbegin == null || dtpend == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Chọn ngày"));
             
                return;
            }
            showhidecolumn(loaibaocao);
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = " ";
            string dieukienmahang = " ";
            if (nvlNhapXuatItemShowcrr.SerialLink != null || nvlNhapXuatItemShowcrr.SerialCT != null)
            {
                if (nvlNhapXuatItemShowcrr.SerialLink != null)
                {
                  
                    dieukienmahang = " where SerialLink=@SerialLink";
                    lstpara.Add(new ParameterDefine("@SerialLink", nvlNhapXuatItemShowcrr.SerialLink));
                }
                if (nvlNhapXuatItemShowcrr.SerialCT != null)
                {
                    dieukien = " where Serial=@SerialCT";
                    lstpara.Add(new ParameterDefine("@SerialCT", nvlNhapXuatItemShowcrr.SerialCT));
                }

            }
            else
            {
                dieukien = " where Ngay>=@DateBegin and Ngay<=@DateEnd";
                lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
                lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
                {
                    dieukien += " and MaKho=@MaKho";
                    lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
                }
                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.LyDo))
                {
                    dieukien += "  and LyDo = @LyDo";
                    lstpara.Add(new ParameterDefine("@LyDo", nvlNhapXuatItemShowcrr.LyDo));
                }
                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.NhaMay))
                {
                    dieukien += "  and NhaMay = @NhaMay";
                    lstpara.Add(new ParameterDefine("@NhaMay", nvlNhapXuatItemShowcrr.NhaMay));
                }

                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
                {
                    dieukienmahang += " where MaHang = @MaHang";
                    lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItemShowcrr.MaHang));
                }

                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.TenGN))
                {
                    dieukien += " and MaGN = @MaGN";
                    lstpara.Add(new ParameterDefine("@MaGN", nvlNhapXuatItemShowcrr.TenGN));
                }
                if (!chknhapkho || !chkxuatkho)
                {
                    if (chknhapkho)
                        dieukien += " and (STTCT>0)";
                    if (chkxuatkho)
                        dieukien += " and (STTCT<0)";
                }
            }

            string sqlSearch = "";

            if (loaibaocao == "Báo cáo chi tiết")
            {
                //string[] arrshow = new string[] { "MaHang", "TenHang", "SerialCT", "SLNhap", "SLXuat", "DVT", "LyDo", "TenSP", "MaKien", "DauTuan", "SoLo", "TenGN", "SoXe", "GhiChu", "DonGia", "NgayHetHan", "SerialLink", "KhachHang_XuatXu", "UserInsert", "SerialKHDH", "SerialDN", "NguoiDeNghi", "TenLienKet", "NhaMay", "NgayInsert" };
                bool checkshow = false;
               
                sqlSearch = string.Format(@"use NVLDB 
                     declare @tblsp  Table(MaSP nvarchar(100) primary key,TenSP nvarchar(200))
                    insert into @tblsp(MaSP,TenSP)
                    exec SP.DataBase_ScansiaPacific2014.[dbo].[getTableformSqlString] @SQL_QUERY='select MaSP,TenSP from SanPham'
                    select nxitem.Serial,nxitem.SerialLink,nxitem.SerialCT,nh.TenNhom,nxitem.MaHang,nxitem.SLNhap,nx.LyDo,nx.MaCT,nxitem.SLXuat,nxitem.DonGia,(nxitem.SLNhap+nxitem.SLXuat)*nxitem.DonGia as ThanhTien
                    ,nxitem.DauTuan,nxitem.GhiChu,nx.NhaMay,nxitem.NgayInsert,nxitem.UserInsert,nxitem.MaKien,nxitem.SoLo,gn.MaGN,gn.TenGN,nxitem.SerialKHDH,hh.TenHang,hh.DVT
                    ,qrykh.SerialDN,qrykh.NguoiDeNghi as NguoiDN, qrykh.TenLienKet,nxitem.ViTri,mk.TenKho,nx.Ngay,nx.ChatLuong,GhiChuDeNghi,
                    nxitem.MaSP,ArticleNumber,sp.TenSP,nxitem.Barcode
                    from (SELECT  [Serial] ,[MaKho],MaCT
                          ,[MaGN],[LyDo],[PONumber],[Ngay],[NguoiDN],[NhaMay],ChatLuong
                      FROM [NvlNhapXuat] {0}) nx
                    inner join
                    (select * from NvlNhapXuatItem {1}) nxitem 
                    on nx.Serial=nxitem.SerialCT
                    inner JOIN View_NoiGN gn on gn.MaGN = nx.MaGN
                    inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                    inner join dbo.NvlHangHoa hh on nxitem.MaHang=hh.MaHang 	
                    left join @tblsp sp on nxitem.MaSP=sp.MaSP
                    inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                    left join
					(SELECT  [Serial],[SerialDN],'NvlKeHoachMuaHangItem' as [TableName] ,[TenLienKet],UserInsert as NguoiDeNghi,TenLienKet as GhiChuDeNghi
                      FROM [NvlKeHoachMuaHangItem]
                        {1}
                      union all
                      SELECT [Serial] ,[SerialMaDH] as SerialDN,'NvlDonDatHangItem' as [TableName],'' as [TenLienKet],UserInsert as NguoiDeNghi,DienGiai as GhiChuDeNghi
                      FROM [NvlDonDatHangItem] {1}
                     ) as qrykh on (nxitem.SerialKHDH=qrykh.Serial and nxitem.TableName=qrykh.TableName) 
                     ", dieukien, dieukienmahang);

            }
            if(loaibaocao== "Theo mã hàng")
            {
                
                sqlSearch = string.Format(@"use NVLDB 

                    declare @tblct table(Serial int primary key,Ngay date,[MaGN] nvarchar(100))
                    insert @tblct(Serial,Ngay,MaGN)
                    select Serial,Ngay,MaGN FROM [NvlNhapXuat]  {0}
                    select nxitem.MaHang,hh.TenHang,nh.TenNhom,hh.DVT,sum(nxitem.SLNhap) as SLNhap,sum(nxitem.SLXuat) as SLXuat,sum((nxitem.SLNhap+nxitem.SLXuat)*DonGia)/sum(nxitem.SLNhap+nxitem.SLXuat) as DonGia
                        from @tblct nx
                        inner join
                         NvlNhapXuatItem  nxitem 
                        on nx.Serial=nxitem.SerialCT
                        inner JOIN View_NoiGN gn on gn.MaGN = nx.MaGN
                        inner join (select * from dbo.NvlHangHoa {1})  hh on nxitem.MaHang=hh.MaHang
                        inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom 
                        group by nxitem.MaHang,hh.TenHang,hh.DVT,nh.TenNhom", dieukien, dieukienmahang);
            }
            if(loaibaocao=="Báo cáo kiểm hàng")
            {
                string diuekienmahangchatluong=dieukienmahang.Replace("where", "and");
                var querycheckSerialCT =lstpara.Where(p=>p.ParameterName.Contains("SerialCT")).FirstOrDefault();
                string dieukienfilterthanhtoan = "and ms.Ngay>=@DateBegin";
                if(querycheckSerialCT!=null)
                {
                    dieukienfilterthanhtoan = " and SerialCT=@SerialCT";
                }
                sqlSearch = string.Format(@"use NVLDB 

                                IF OBJECT_ID('tempdb..#lstnt') IS NOT NULL
                                DROP TABLE #lstnt

                                create Table #lstnt  (Serial int,MaHang nvarchar(100),SoLo nvarchar(100),KetLuanChungTu nvarchar(100),KetLuanHangHoa nvarchar(100),Loai int) 

                                CREATE NONCLUSTERED INDEX IX_MaHang_SoLo
                                ON #lstnt (Serial,MaHang, SoLo); 

                                declare @tblnx Table(Serial int primary key,STTCT int,MaKho nvarchar(100),MaCT nvarchar(100),MaGN nvarchar(100),LyDo nvarchar(100),Ngay date,NguoiDN nvarchar(100),NhaMay nvarchar(100),ChatLuong nvarchar(100),UserInsert nvarchar(100),NgayInsert datetime)
                                declare @tblnxitem Table(SerialCT int,MaHang nvarchar(100),SLNhap decimal(18,6),SLXuat decimal(18,6),DonGia decimal(18,6),ThanhTien decimal(18,6),SoLo nvarchar(100),TinhTrang nvarchar(100))

                                insert into @tblnx([Serial],STTCT ,[MaKho],MaCT,[MaGN],[LyDo],[Ngay],[NguoiDN],[NhaMay],ChatLuong,UserInsert,NgayInsert)
                                SELECT  [Serial],STTCT ,[MaKho],MaCT,[MaGN],[LyDo],[Ngay],[NguoiDN],[NhaMay],ChatLuong,UserInsert,NgayInsert
                                FROM [NvlNhapXuat]  {0}


                                declare @lstserial nvarchar(max)
                                --SELECT @lstserial = COALESCE(@lstserial + ';', '') + CAST(Serial AS NVARCHAR)
                                --FROM @tblnx where STTCT>0
                            SELECT @lstserial = STUFF((
                            SELECT ';' + CAST(Serial AS NVARCHAR)
                            FROM @tblnx where STTCT>0
                            FOR XML PATH('')
                        ), 1, 1, '')


                               if(@lstserial is not null)
								begin
								
                                    INSERT INTO #lstnt(Serial,MaHang,SoLo,KetLuanChungTu,KetLuanHangHoa,Loai)
                                    EXEC SP.NguyenVatLieu.dbo.KetQuaNghiemThuDauVao_ListNew @lstserial
								end

                                --Danh sách mã hàng không nghiệm thu
                                declare @tblmhknt as Table(MaHang nvarchar(100) primary key)
                                insert into @tblmhknt(MaHang)
                                select MaHang from SP.NguyenVatLieu.dbo.View_MaHangKhongNghiemThu

                                insert into @tblnxitem(SerialCT,MaHang,SLNhap,SLXuat,DonGia,SoLo,TinhTrang)
                                select SerialCT,qry.MaHang,SLNhap,SLXuat,DonGia,qry.SoLo
                                ,case when knt.MaHang is not null then N'Không kiểm' else 
                                isnull(nt.KetLuanHangHoa,N'Chưa kiểm') end  as TinhTrang
                                from
                                (select SerialCT,MaHang,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,DonGia,isnull(SoLo,'') as SoLo 
                                from NvlNhapXuatItem where  SerialCT in (select Serial from @tblnx) {1}
                                group by SerialCT,MaHang,DonGia,isnull(SoLo,''))  as qry
                                left join #lstnt nt on (qry.SerialCT=nt.Serial and qry.MaHang=nt.MaHang and qry.SoLo = case when ISNULL(Loai, 1) = 1 then nt.SoLo else qry.SoLo end)
                                left join @tblmhknt knt on qry.MaHang=knt.MaHang

                                declare @tblthanhtoan as Table(SerialCT int primary key,NgayThanhToan date,NguoiThanhToan nvarchar(100))
                                insert into @tblthanhtoan(SerialCT,NgayThanhToan,NguoiThanhToan)
                                select SerialCT,NgayXacNhan,NguoiXacNhan from 
                                (SELECT SerialCT,[NgayXacNhan],[NguoiXacNhan]
                                      ,ROW_NUMBER() OVER (PARTITION BY item.SerialCT
                                           ORDER BY item.Serial DESC) AS RowNum
                                  FROM [NVLDB].[dbo].[NvlThanhToan] ms
                                  inner join  [NvlThanhToanItem] item
                                  on ms.Serial=item.SerialTT
                                  where NguoiXacNhan is not null {2})
                                  as qry where RowNum=1


                                select nxitem.SerialCT,nh.TenNhom,nxitem.MaHang,nxitem.SLNhap,nx.LyDo,nx.MaCT,nxitem.SLXuat,nxitem.DonGia,(nxitem.SLNhap+nxitem.SLXuat)*nxitem.DonGia as ThanhTien
                                                   ,nx.NhaMay,nxitem.SoLo,gn.TenGN,hh.TenHang,hh.DVT
                                                   ,mk.TenKho,nx.Ngay,nxitem.TinhTrang,tbltt.NgayThanhToan,tbltt.NguoiThanhToan,nx.UserInsert,nx.NgayInsert,nx.ChatLuong
                                                    from @tblnx nx
                                                    inner join
                                                    @tblnxitem nxitem 
                                                    on nx.Serial=nxitem.SerialCT
                                                    inner JOIN View_NoiGN gn on gn.MaGN = nx.MaGN
                                                    inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                                                    inner join dbo.NvlHangHoa hh on nxitem.MaHang=hh.MaHang 	
                                                    inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
					                                left join @tblthanhtoan tbltt on nx.Serial=tbltt.SerialCT
	                                DROP TABLE #lstnt


                  
                     ", dieukien, diuekienmahangchatluong,dieukienfilterthanhtoan);

            }
            CallAPI callAPI = new CallAPI();
            try
            {


                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sqlSearch, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemShow>>(json);
                    lstdata = query;
                    if(query.Any())
                    { await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide)); }

                    // await GotoMainForm.InvokeAsync();
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger,"Lỗi:"+ex.Message));
            }
            finally
            {
                PanelVisible = false;
               
                StateHasChanged();
            }

        }
        List<NvlNhapXuatItemShow> lstrp = new List<NvlNhapXuatItemShow>();
        private async Task InBangKeAsync()
        {
           
            
            string ghichu = "";
            if (dtpbegin == null || dtpend == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Chọn ngày"));

                return;
            }

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = " ";
            string dieukienmahang = " ";
            if (nvlNhapXuatItemShowcrr.SerialLink != null||nvlNhapXuatItemShowcrr.SerialCT!=null)
            {
                if(nvlNhapXuatItemShowcrr.SerialLink!=null)
                {
                    ghichu += nvlNhapXuatItemShowcrr.SerialLink.ToString();
                    dieukienmahang = " where SerialLink=@SerialLink";
                    lstpara.Add(new ParameterDefine("@SerialLink", nvlNhapXuatItemShowcrr.SerialLink));
                }
                if(nvlNhapXuatItemShowcrr.SerialCT != null)
                {
                    dieukien = " where Serial=@SerialCT";
                    lstpara.Add(new ParameterDefine("@SerialCT", nvlNhapXuatItemShowcrr.SerialCT));
                }
               
            }
            else
            {
                if((dtpend.Value-dtpbegin.Value).TotalDays>31)
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Bảng kê này hỗ trợ Ngày bắt đầu và ngày kết thúc không quá 31 ngày"));
                    return;
                }
                dieukien = " where Ngay>=@DateBegin and Ngay<=@DateEnd";
                lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
                lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
                ghichu +="Ngày: " + dtpbegin.Value.ToString("dd/MM/yy") + " - " + dtpend.Value.ToString("dd/MM/yy");
                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
                {
                    dieukien += " and MaKho=@MaKho";
                    lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
                    ghichu += Environment.NewLine+ string.Format("{0}", nvlNhapXuatItemShowcrr.TenKho);
                }
                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.LyDo))
                {
                    dieukien += "  and LyDo = @LyDo";
                    lstpara.Add(new ParameterDefine("@LyDo", nvlNhapXuatItemShowcrr.LyDo));
                    ghichu += Environment.NewLine+string.Format("Lý do: {0}",nvlNhapXuatItemShowcrr.LyDo);
                }
                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.NhaMay))
                {
                    dieukien += "  and NhaMay = @NhaMay";
                    lstpara.Add(new ParameterDefine("@NhaMay", nvlNhapXuatItemShowcrr.NhaMay));
                    ghichu += Environment.NewLine+ nvlNhapXuatItemShowcrr.NhaMay;
                }

                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
                {
                    dieukienmahang += " where MaHang = @MaHang";
                    lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItemShowcrr.MaHang));
                    ghichu += Environment.NewLine+ string.Format("Tên hàng: {0}", nvlNhapXuatItemShowcrr.TenHang);
                }

                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.TenGN))
                {
                    dieukien += " and MaGN = @MaGN";
                    lstpara.Add(new ParameterDefine("@MaGN", nvlNhapXuatItemShowcrr.TenGN));
                    ghichu += Environment.NewLine+ string.Format("Nơi giao/nhận: {0}", TenGNFullName);
                }
                if(!chknhapkho||!chkxuatkho)
                {
                    if(chknhapkho)
                        dieukien += " and (STTCT>0)";
                    if(chkxuatkho)
                        dieukien += " and (STTCT<0)";
                }
            }

            string sqlSearch = "";

            
                //string[] arrshow = new string[] { "MaHang", "TenHang", "SerialCT", "SLNhap", "SLXuat", "DVT", "LyDo", "TenSP", "MaKien", "DauTuan", "SoLo", "TenGN", "SoXe", "GhiChu", "DonGia", "NgayHetHan", "SerialLink", "KhachHang_XuatXu", "UserInsert", "SerialKHDH", "SerialDN", "NguoiDeNghi", "TenLienKet", "NhaMay", "NgayInsert" };
                bool checkshow = false;

                sqlSearch = string.Format(@"use NVLDB select nxitem.SerialCT,nh.TenNhom,nxitem.MaHang,(nxitem.SLNhap+nxitem.SLXuat) as SoLuong,nx.LyDo,nxitem.DonGia,(nxitem.SLNhap+nxitem.SLXuat)*nxitem.DonGia as ThanhTien
                  ,gn.TenGN,hh.TenHang,hh.DVT,nx.Ngay
                   ,mk.TenKho
                    from (SELECT  [Serial] ,[MaKho]
                          ,[MaGN],[LyDo],[PONumber],[Ngay],[NguoiDN],[NhaMay]
                      FROM [NvlNhapXuat]  {0} ) nx
                    inner join
                    (select MaHang,SerialCT,sum(SLNhap) as SLNhap,sum(SLXuat) as SLXuat,DonGia from NvlNhapXuatItem {1}  group by  MaHang,SerialCT,DonGia) nxitem 
                    on nx.Serial=nxitem.SerialCT
                    inner JOIN View_NoiGN gn on gn.MaGN = nx.MaGN
                    inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                    inner join dbo.NvlHangHoa hh on nxitem.MaHang=hh.MaHang 	
                    inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom order by nxitem.SerialCT
                     ", dieukien, dieukienmahang);

            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.sql = sqlSearch;
            getDataFromSql.reportname = "XtraRp_BangKeTongHop";
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            DataTable dttmp = new DataTable();
            dttmp.Columns.Add("GhiChu", typeof(string));
            DataRow row = dttmp.NewRow();
            row["GhiChu"] = ghichu;
            dttmp.Rows.Add(row);
            getDataFromSql.dtparameter = prs.ConvertDataTableToJson(dttmp);
            dttmp.Dispose();
            await ModelAdmin.mainLayout.showreportpdfAsync(getDataFromSql);

            CallAPI callAPI = new CallAPI();
            //try
            //{

            //    lstrp.Clear();
            //    PanelVisible = true;
            //    string json = await callAPI.ExcuteQueryEncryptAsync(sqlSearch, lstpara);
            //    if (json != "")
            //    {
            //        var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemShow>>(json);
                    
            //        if(query.Count==0)
            //        {
            //            toastService.Notify(new ToastMessage(ToastType.Danger, "Không tìm thấy dữ liệu"));
            //            return;
            //        }
            //        lstrp.AddRange(query);
            //        XtraRp_BangKeTongHop xtraRp_PhieuNhapKho = new XtraRp_BangKeTongHop();
            //        xtraRp_PhieuNhapKho.setGhiChu(ghichu);
            //        xtraRp_PhieuNhapKho.DataSource = lstrp;
            //        await ModelAdmin.mainLayout.showreportAsync(xtraRp_PhieuNhapKho);
            //        //query.Clear();
            //        // await GotoMainForm.InvokeAsync();
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
        }
        private async void exportexcel()
        {   PanelVisible = true;
           await dxGrid.ExportToXlsxAsync("ExporBangKe");
            PanelVisible = false;
        }
    }
}
