using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;

using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Urc_KeHoachMuaHang_AddSanPham;
using System.ComponentModel;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucNVLMaster;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;
using NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang;
using static NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Page_DonDatHang_Master;


namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Page_TheoDoiKeHoachXuat
    {


        [Inject]
        ToastService toastService { get; set; }
        [Inject] BrowserService browserService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        public class KeHoachSuDung()
        {
            public string MaSP { get; set; }
            public string IDKeHoach { get; set; }
            public string TenDinhMuc { get; set; }
            public string CongDoan { get; set; }
            public string MauSP { get; set; }
            public string KhuVuc { get; set; }
            public double SoLuong { get; set; }

        }
        public class KeHoachItemXuatKho()
        {
            public string MaSP { get; set; }

            public string TenDinhMuc { get; set; }
            public string CongDoan { get; set; }
            public string MauSP { get; set; }
            public string MaHang { get; set; }
            public double SLDeNghi { get; set; }
            public double SLConLai { get; set; }
            public double TyLe { get { if (SLDeNghi <= 0) return 0; return (SLDeNghi - SLConLai) / SLDeNghi; } }

        }
        public class KeHoachGopXuatKho()
        {
            public string MaSP { get; set; }
            public string MauSP { get; set; }
            public string ID { get; set; }


        }
        bool CheckQuyen = false;
        List<KeHoachSelected> lstKeHoachSelected = new List<KeHoachSelected>();



        private void checkedchangedItem(bool bl, DinhMucVatTuShow dinhMucVatTuShow)
        {
            dinhMucVatTuShow.chk = bl;

        }
        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<DinhMucVatTuShow> lstdinhmucall { get; set; } = new List<DinhMucVatTuShow>();

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<KeHoachSuDung> lstkehoachsudung { get; set; } = new List<KeHoachSuDung>();
            [JsonProperty("Table2")]
            public List<KeHoachItemXuatKho> lstkehoachitemxuatkho { get; set; } = new List<KeHoachItemXuatKho>();
            [JsonProperty("Table3")]
            public List<KeHoachGopXuatKho> lstkehoachgopxuatkho { get; set; } = new List<KeHoachGopXuatKho>();
        }
        CustomRoot customRoot = new CustomRoot();
        protected override async Task OnInitializedAsync()
        {
            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 70;
            heightgrid = string.Format("{0}px", height);
            lstsanpham = await ModelData.GetSanPham();
            lstkhachhang = await Model.ModelData.GetKhachHangSanSuatSP();
            //return base.OnInitializedAsync();
        }

        private async Task searchAsync()
        {

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string sqlgetsp = @"SELECT  [MaSP]
            FROM [dbo].[SanPham]";
            string dieukien = @"";
            if (string.IsNullOrEmpty(makehoach))
            {
                if (!string.IsNullOrEmpty(MaSPSelected))
                {
                    if (dieukien == "")
                        dieukien += string.Format(" where MaSP=N''{0}''", MaSPSelected);
                    else
                        dieukien += string.Format(" and MaSP=N''{0}''", MaSPSelected);
                }
                if (!string.IsNullOrEmpty(khachhangselected))
                {
                    if (dieukien == "")
                        dieukien += string.Format(" where KhachHang=N''{0}''", khachhangselected);
                    else
                        dieukien += string.Format(" and KhachHang=N''{0}''", khachhangselected);
                }
                if (!string.IsNullOrEmpty(dieukien))
                {
                    dieukien = string.Format(" and MaSP in ({0} {1})", sqlgetsp, dieukien);
                }
                else
                    dieukien = "";

            }
            else
            {
                dieukien = " and ID=N''" + makehoach + "''";
            }

            customRoot.lstdinhmucall.Clear();
            customRoot.lstkehoachsudung.Clear();
            customRoot.lstkehoachitemxuatkho.Clear();
            customRoot.lstkehoachgopxuatkho.Clear();
            lstdata.Clear();

            //lstpara.Add(new ParameterDefine("@DateBegin", datebegin.ToString("MM/dd/yyyy 00:00")));
            //lstpara.Add(new ParameterDefine("@DateEnd", dateend.ToString("MM/dd/yyyy 23:59")));
            string sql = string.Format(@"use NVLDB  
							declare @lstsanpham nvarchar(max)
							declare @tblkehoach as Table(ID nvarchar(100),MaSP nvarchar(100),SoLuongSP float,KhuVuc nvarchar(100),MaMauKH nvarchar(100),TenMau nvarchar(100),Color nvarchar(100))
							insert into @tblkehoach
							exec SP.DataBase_ScansiaPacific2014.dbo.getTableformSqlString 
							@SQL_QUERY = '
								SELECT ID, MaSP, SoLuongSP, KhuVucKH, MaMauKH, 
									   ISNULL(mm.TenMau, '''') AS TenMau, mm.Color 
								FROM (
									SELECT ID, MaSP, SoLuongSP, KhuVuc AS KhuVucKH, MaMauKH 
									FROM dbo.[KeHoachSP]
									WHERE Active = 0  and SoLuongSP>0
										  AND KhuVuc IN (''KV2DH'', ''KV3'')
                                        {0}    
										 and ID in (SELECT DISTINCT  [ID_KeHoach] FROM [dbo].[KH_Theodoi] where SLTheoDoi>0)
								) as qry left join [dbo].MaMau mm on qry.MaMauKH=mm.MaMau
								'

					

                             SELECT @lstsanpham = STUFF((
                                 SELECT ';' + CAST(MaSP AS NVARCHAR)
                                 FROM (select MaSP from @tblkehoach group by MaSP) as qry
                                 FOR XML PATH('')
                             ), 1, 1, '')
							
							
                            IF OBJECT_ID('tempdb..##tmpdinhmuctoancuc') IS NOT NULL
	                            DROP TABLE ##tmpdinhmuctoancuc

                            exec GetDinhMucNVL_SanPhamList @lstsanpham=@lstsanpham
							select tbl.*,tblkh.SoLuong as SLKeHoach,tblkh.SoLuong as SLConLai from ##tmpdinhmuctoancuc tbl 
							inner join (select MaSP,MaMauKH,KhuVuc,sum(SoLuongSP) as SoLuong from @tblkehoach
							group by MaSP,MaMauKH,KhuVuc) tblkh on (tbl.MaSP=tblkh.MaSP and tbl.GroupMauSP=tblkh.MaMauKH and tbl.KhuVucKH=tblkh.KhuVuc)
							 

							   --Danh sách chi tiết định mức kế hoạch đã sử dụng
							 
							 declare @tblkhdinhmucitem Table([IDKeHoach] nvarchar(100),MaSP nvarchar(100),MauSP nvarchar(100),KhuVuc nvarchar(100),KeyGroup nvarchar(20),CongDoan nvarchar(200),TenDinhMuc nvarchar(200),SoLuong float)
							 
							 insert into @tblkhdinhmucitem([IDKeHoach],MaSP,MauSP,KhuVuc,KeyGroup,CongDoan,TenDinhMuc,SoLuong)
							 select [IDKeHoach],tblkh.MaSP,tblkh.MaMauKH as MauSP,tblkh.KhuVuc,KeyGroup,lower(CongDoan) as  CongDoan,lower([TenDinhMuc]) as TenDinhMuc,SoLuong
							 FROM [NvlKeHoachMuaHang_DinhMuc] dm
								inner join @tblkehoach tblkh on dm.IDKeHoach=tblkh.ID

								
                                --Xử lý phần đồng bộ đã đề nghị
							  SELECT [IDKeHoach],[TenDinhMuc],CongDoan,sum([SoLuong]) as SoLuong
							  ,MaSP,MauSP,KhuVuc
								from @tblkhdinhmucitem
								group by [IDKeHoach],[TenDinhMuc],CongDoan,MaSP,MauSP,KhuVuc

								--Xử lý phần tổng số lượng xuất kho
								select tbldm.MaSP,tbldm.CongDoan,tbldm.TenDinhMuc,tbldm.MauSP,MaHang,sum(khmhitem.SoLuong) as SLDeNghi,sum(SLTheoDoi) as SLConLai 
								from [NvlKeHoachMuaHangItem] khmhitem 
								inner join @tblkhdinhmucitem tbldm on khmhitem.KeyGroup=tbldm.KeyGroup
								where TableName='NvlKeHoachMuaHang_DinhMuc'
								group by tbldm.MaSP,tbldm.CongDoan,tbldm.TenDinhMuc,tbldm.MauSP,MaHang
								
                            --Gộp kế hoạch để lấy danh sách các kế hoạch sử dụng
							declare @tblgopkh as Table(MaSP nvarchar(100),MauSP nvarchar(100),ID nvarchar(1000))

							insert into @tblgopkh(MaSP,MauSP,ID)
							select MaSP,MaMauKH,ID from
							(
							SELECT MaMauKH, MaSP, 
							    STUFF((SELECT DISTINCT ';' + ID
									  FROM @tblkehoach AS b
									  WHERE b.MaMauKH = a.MaMauKH 
										AND b.MaSP = a.MaSP
									   FOR XML PATH ('')), 1, 1, '')  AS ID
						FROM @tblkehoach AS a
						GROUP BY MaMauKH, MaSP) as qrygopkh

							select MaSP,MauSP,ID from @tblgopkh

                            Drop table ##tmpdinhmuctoancuc", dieukien);

            try
            {

                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<CustomRoot>(json);
                    if (query != null)
                    {
                        customRoot.lstdinhmucall.AddRange(query.lstdinhmucall);
                        customRoot.lstkehoachsudung.AddRange(query.lstkehoachsudung);
                        customRoot.lstkehoachitemxuatkho.AddRange(query.lstkehoachitemxuatkho);
                        customRoot.lstkehoachgopxuatkho.AddRange(query.lstkehoachgopxuatkho);
                        if (customRoot.lstkehoachsudung.Any())
                        {
                            var querysd = customRoot.lstkehoachsudung.GroupBy(p => new { MaSP = p.MaSP, MauSP = p.MauSP, KhuVuc = p.KhuVuc, CongDoan = p.CongDoan, TenDinhMuc = p.TenDinhMuc })
                                .Select(p => new KeHoachSuDung { MaSP = p.Key.MaSP, KhuVuc = p.Key.KhuVuc, CongDoan = p.Key.CongDoan, TenDinhMuc = p.Key.TenDinhMuc, MauSP = p.Key.MauSP, SoLuong = p.Sum(n => n.SoLuong), IDKeHoach = string.Join(";", p.Select(n => n.IDKeHoach)) }).ToList();

                            foreach (var it in querysd)
                            {
                                //Lưu ý, chỗ này chỉ cần gán 1 phần tử thôi là được, vì nếu gán nhiều, lặp lại thì lúc sum SLConLai sẽ double lên nhiều lần ở Page ReportItem
                                foreach (var item in customRoot.lstdinhmucall)
                                {
                                    if (it.CongDoan == item.CongDoan.ToLower() && it.TenDinhMuc == item.TenDinhMuc.ToLower() && it.KhuVuc == item.KhuVucKH && it.MaSP == item.MaSP && it.MauSP == item.GroupMauSP)
                                    {
                                        item.SLSuDung = it.SoLuong;
                                        item.SLConLai = (decimal)(item.SLKeHoach - item.SLSuDung);
                                        //item.MaKH = it.IDKeHoach;
                                        break;
                                    }
                                }
                                    
                            }
                            foreach (var item in customRoot.lstdinhmucall)
                            {
                                
                                foreach (var it in customRoot.lstkehoachitemxuatkho)
                                {
                                    if (it.CongDoan == item.CongDoan.ToLower() && it.TenDinhMuc == item.TenDinhMuc.ToLower() && it.MaHang == item.MaVatTu && it.MaSP == item.MaSP && it.MauSP == item.GroupMauSP)
                                    {
                                        item.SLTTDeNghi = it.SLDeNghi;
                                        item.SLTTXuat = it.SLDeNghi - it.SLConLai;
                                        break;
                                    }
                                }
                                foreach (var it in customRoot.lstkehoachgopxuatkho)
                                {
                                    if (it.MauSP == item.GroupMauSP && it.MaSP == item.MaSP)
                                    {
                                        item.MaKH = it.ID;
                                        break;
                                    }
                                }
                            }

                        }


                        var lstgr = customRoot.lstdinhmucall.GroupBy(p => new { MaSP = p.MaSP, TenSP = p.TenSP, KhachHang = p.KhachHang }).Select(p => new SanPhamShow { MaSP = p.Key.MaSP, TenSP = p.Key.TenSP, KhachHang = p.Key.KhachHang, SLSP = 0 }).ToList();
                        lstdata.AddRange(lstgr);

                    }

                    //var query = JsonConvert.DeserializeObject<List<KeHoachThang_Show>>(json);
                    //lstdata.AddRange(query);
                }


                PanelVisible = false;
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));

                PanelVisible = false;
            }
            finally
            {
                dxGrid.Reload();
                PanelVisible = false;
                StateHasChanged();
                //ShowProgress.CloseAwait();
            }
        }
        class KetquaResult
        {
            public int? Serial { get; set; }
            public double? SLCL { get; set; }

            public string? ketqua { get; set; }

            public string? ketquaexception { get; set; }

        }
    }
      

}
