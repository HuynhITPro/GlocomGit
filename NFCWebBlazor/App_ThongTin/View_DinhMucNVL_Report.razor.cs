using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_DeNghiTheoDinhMuc;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucNVLMaster;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class View_DinhMucNVL_Report
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        public class KeHoachSelected()
        {
            public string MaSP { get; set; }
            public string MauSP { get; set; }
            public string GroupNhaCungCap { get; set; }
            public decimal? SLDeNghi { get; set; }
        }
        bool CheckQuyen = false;
        List<KeHoachSelected> lstKeHoachSelected = new List<KeHoachSelected>();

        public async Task ImportExcelAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền import"));
                return;
            }


        }
     
        List<KeHoachDinhMucCongDoan> lstkehoachcongdoangroup = new List<KeHoachDinhMucCongDoan>();

        public async void loaddataAsyn()
        {
            PanelVisible = true;
            try
            {
                if (sanPhamShowcrr.lstdinhmuc == null)
                {
                    //keHoachThang_Showcrr.lstdinhmuc = new List<DinhMucVatTuShow>();
                    sanPhamShowcrr.lstdinhmuc = lstdinhmucall.Where(p => p.MaSP == sanPhamShowcrr.MaSP).ToList();
                    lstdata = sanPhamShowcrr.lstdinhmuc;

                    lstkehoachcongdoan.Clear();
                    var querygr = lstdata
                         .GroupBy(p => new { GroupNhaCungCap = p.GroupNhaCungCap, MaSP = p.MaSP, GroupMauSP = p.GroupMauSP, TenDinhMuc = p.TenDinhMuc, CongDoan = p.CongDoan })
                         .Select(p => new  { GroupNhaCungCap = p.Key.GroupNhaCungCap,MaSP=p.Key.MaSP, MauSP = p.Key.GroupMauSP, TenDinhMuc = p.Key.TenDinhMuc, CongDoan = p.Key.CongDoan, SLKeHoach=p.Min(n=>n.SLKeHoach),SLConLai=p.Min(n=>n.SLConLai),SLSuDung=p.Sum(n=>n.SLSuDung) }).ToList();
                    
                    foreach(var item in querygr)
                    {
                        KeHoachDinhMucCongDoan keHoachDinhMucCongDoan=new KeHoachDinhMucCongDoan();
                        keHoachDinhMucCongDoan.MaSP = item.MaSP;
                        keHoachDinhMucCongDoan.MauSP = item.MauSP;
                        keHoachDinhMucCongDoan.TenDinhMuc= item.TenDinhMuc;
                        keHoachDinhMucCongDoan.CongDoan = item.CongDoan;
                        keHoachDinhMucCongDoan.GroupNhaCungCap = item.GroupNhaCungCap;
                        


                        KeHoachDinhMucCongDoanItem keHoachDinhMucCongDoanItem = new KeHoachDinhMucCongDoanItem();
                        keHoachDinhMucCongDoanItem.SoLuongSP = item.SLKeHoach;

                        keHoachDinhMucCongDoanItem.SLConLai = item.SLConLai;
                        keHoachDinhMucCongDoanItem.SLDeNghi = item.SLSuDung;
                        keHoachDinhMucCongDoanItem.TyLe=((keHoachDinhMucCongDoanItem.SoLuongSP==0)?0:(keHoachDinhMucCongDoanItem.SLDeNghi/keHoachDinhMucCongDoanItem.SoLuongSP));
                        keHoachDinhMucCongDoanItem.Colorhex = sanPhamShowcrr.Colorhex;
                        keHoachDinhMucCongDoanItem.Colortext = StaticClass.GetContrastColor(sanPhamShowcrr.Colorhex);
                        // keHoachDinhMucCongDoanItem.MaKH = keHoachThang_Showcrr.Serial.ToString();
                        keHoachDinhMucCongDoan.lstkehoachcongdoanitem.Add(keHoachDinhMucCongDoanItem);
                        lstkehoachcongdoan.Add(keHoachDinhMucCongDoan);
                    }
                  
                    lstkehoachcongdoan.AddRange(querygr.Select(p => new KeHoachDinhMucCongDoan { GroupNhaCungCap = p.GroupNhaCungCap, MauSP = p.MauSP, TenDinhMuc = p.TenDinhMuc, CongDoan = p.CongDoan }).ToList());
                    //Xử lý để check nếu đối vs trường hợp 1 định mức 1 công đoạn
                    var querygrcount = querygr.GroupBy(p => new { TenDinhMuc = p.TenDinhMuc, CongDoan = p.CongDoan, MauSP = p.MauSP })
                         .Select(p => new { TenDinhMuc = p.Key.TenDinhMuc, CongDoan = p.Key.CongDoan, Count = p.Count(), MauSP = p.Key.MauSP }).Where(p => p.Count > 1).ToList();
                    bool checkone = false;
                    foreach (var it in lstkehoachcongdoan)
                    {
                        checkone = true;
                        foreach (var item in querygrcount)
                        {
                            if (it.TenDinhMuc == item.TenDinhMuc && it.CongDoan == item.CongDoan && it.MauSP == item.MauSP)
                            {
                                checkone = false;
                                break;
                            }
                        }
                        it.chk = checkone;
                        foreach (var data in lstdata)
                        {
                            if (data.TenDinhMuc == it.TenDinhMuc && data.CongDoan == it.CongDoan && it.MauSP == data.GroupMauSP)
                            {
                                data.chk = checkone;
                                //if (checkone == true)
                                //{
                                //    data.SLDeNghi = (decimal)(sanPhamShowcrr.SLPhaiDat * data.SLQuyDoi);
                                //}
                            }
                        }


                    }
                   
                    sanPhamShowcrr.lstkehoachcongdoan = lstkehoachcongdoan;

                }
                else
                {
                    lstdata = sanPhamShowcrr.lstdinhmuc;
                    lstkehoachcongdoan = sanPhamShowcrr.lstkehoachcongdoan;

                }
                var querycheckncc = lstdata.Where(p => p.ChonNCC == 1).FirstOrDefault();
                if (querycheckncc != null)
                {
                    checkncc = true;
                }
                else
                    checkncc = false;
            }
            catch (Exception ex)
            {
                PanelVisible = false;
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));

            }
            finally
            {
                PanelVisible = false;
                dxGrid.Reload();
                StateHasChanged();
            }
        }
        public void GetGroupDataItems(bool bl, int visibleIndex, KeHoachDinhMucCongDoan? keHoachDinhMucCongDoan)
        {
            //Phải sử dụng thuộc tính này để gắn KeyGroup cho nhóm
            //var result = new List<DinhMucVatTuShow>();
            var rowLevel = dxGrid.GetRowLevel(visibleIndex);
            keHoachDinhMucCongDoan.chk = bl;
            double sldenghi = keHoachDinhMucCongDoan.lstkehoachcongdoanitem.Sum(n => n.SLDeNghi);//SL bộ

            //Tạo KeyGroup
            if (keHoachDinhMucCongDoan.KeyGroup == null)
            {
                keHoachDinhMucCongDoan.KeyGroup = string.Format("{0}", StaticClass.Randomstring(10));
            }
            if (dxGrid.IsGroupRow(visibleIndex))
                dxGrid.ExpandGroupRow(visibleIndex, true);

            for (var i = visibleIndex + 1; i < dxGrid.GetVisibleRowCount(); i++)
            {
                if (dxGrid.GetRowLevel(i) <= rowLevel)
                    break;
                if (!dxGrid.IsGroupRow(i))
                {
                    DinhMucVatTuShow item = (DinhMucVatTuShow)dxGrid.GetDataItem(i);
                    item.KeyGroup = keHoachDinhMucCongDoan.KeyGroup;
                    item.chk = bl;
                    if (bl) { item.SLDeNghi = (decimal)(sldenghi * item.SLQuyDoi); item.SLConLai = item.SLDeNghi; }
                    else { item.SLDeNghi = 0; }
                    // result.Add((DinhMucVatTuShow)dxGrid.GetDataItem(i));
                }
            }
            //return result;
        }
        bool boolExpand = false;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

            }
            else
            {
                if (boolExpand == false)
                {
                    if (dxGrid.GetVisibleRowCount() > 0)
                    {
                        loadExpand();
                        boolExpand = true;
                    }


                }



            }
            return base.OnAfterRenderAsync(firstRender);
        }
        public void SLDeNghiChanged(decimal? e, KeHoachSelected keHoachSelected, DinhMucVatTuShow dinhMucVatTuShow)
        {
            keHoachSelected.SLDeNghi = e;
        }
       
        public void GetGroupDataItems(bool bl, int visibleIndex)
        {
            //var result = new List<DinhMucVatTuShow>();
            var rowLevel = dxGrid.GetRowLevel(visibleIndex);

            dxGrid.ExpandGroupRow(visibleIndex, true);
            for (var i = visibleIndex + 1; i < dxGrid.GetVisibleRowCount(); i++)
            {
                if (dxGrid.GetRowLevel(i) <= rowLevel)
                    break;
                if (!dxGrid.IsGroupRow(i))
                {
                    DinhMucVatTuShow item = (DinhMucVatTuShow)dxGrid.GetDataItem(i);
                    item.chk = bl;
                    if (bl) { item.SLDeNghi = (decimal)item.SLConLai; }
                    else { item.SLDeNghi = 0; }
                    // result.Add((DinhMucVatTuShow)dxGrid.GetDataItem(i));
                }

            }
            //return result;
        }

        private void loadExpand()
        {

            int n = dxGrid.GetGroupCount();
            dxGrid.ExpandAllGroupRows();
            dxGrid.BeginUpdate();
            for (var i = dxGrid.GetVisibleRowCount() - 1; i >= 0; i--)
            {

                if (dxGrid.IsGroupRow(i))
                {

                    int rowlevel = dxGrid.GetRowLevel(i);
                    if (rowlevel < n - 1)
                        dxGrid.ExpandGroupRow(i);
                    else
                        dxGrid.CollapseGroupRow(i);


                }

            }

            dxGrid.EndUpdate();


        }
        private async Task showlstxuatkhoAsync(string MaKH)
        {
            if (string.IsNullOrEmpty(MaKH))
                return;
            string sql = string.Format(@"use NVLDB  
							declare @IDKeHoach nvarchar(100)=N'{0}'
                   declare @tblkh table(ID nvarchar(100))
            insert into @tblkh(ID)
            select [Name] from dbo.StringSplit(@IDKeHoach,';')

            declare @tbldenghi table(Serial int primary key,SerialDN int,UserDeNghi nvarchar(100),IDKeHoach nvarchar(100))
            insert into @tbldenghi(Serial,SerialDN,UserDeNghi,IDKeHoach)
            SELECT  [Serial],SerialDN,UserInsert,qrydmkh.IDKeHoach
              FROM [dbo].[NvlKeHoachMuaHangItem] it
			  inner join  (SELECT [KeyGroup],IDKeHoach
              FROM [dbo].[NvlKeHoachMuaHang_DinhMuc]
              where IDKeHoach in (select ID from @tblkh)) as qrydmkh
			  on (TableName='NvlKeHoachMuaHang_DinhMuc'  and it.KeyGroup=qrydmkh.KeyGroup)
             
              
			 
			

            select nxitem.Serial,nxitem.SerialLink,nxitem.SerialCT,nh.TenNhom,nxitem.MaHang,nxitem.SLNhap,nx.LyDo,nx.MaCT,nxitem.SLXuat,nxitem.DonGia,(nxitem.SLNhap+nxitem.SLXuat)*nxitem.DonGia as ThanhTien
                    ,nxitem.DauTuan,nx.NhaMay,nxitem.NgayInsert,nxitem.UserInsert,nxitem.MaKien,nxitem.SoLo,gn.TenGN,nxitem.SerialKHDH,hh.TenHang,hh.DVT
                    ,tbl.SerialDN,tbl.UserDeNghi as NguoiDN,nxitem.ViTri,mk.TenKho,nx.Ngay,nx.ChatLuong,tbl.IDKeHoach as GhiChu
                    from 
                    
                    (select * from NvlNhapXuatItem
					where SerialKHDH in (select Serial from @tbldenghi) and TableName='NvlKeHoachMuaHangItem') nxitem 
					inner join @tbldenghi tbl on nxitem.SerialKHDH=tbl.Serial
					inner join NvlNhapXuat nx
                    on nx.Serial=nxitem.SerialCT
                    inner JOIN View_NoiGN gn on gn.MaGN = nx.MaGN
                    inner join dbo.NvlMaKho mk on nx.MaKho=mk.MaKho
                    inner join dbo.NvlHangHoa hh on nxitem.MaHang=hh.MaHang 	
                    inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom", MaKH);

            try
            {

               
                CallAPI callAPI = new CallAPI();
                List<NvlNhapXuatItemShow> lstdata = new List<NvlNhapXuatItemShow>();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemShow>>(json);
                    if (query.Any())
                    {
                        lstdata.AddRange(query);
                        renderFragment = builder =>
                        {
                            builder.OpenComponent<Urc_BangKeNhapKhoKeHoach>(0);
                            builder.AddAttribute(1, "lstdata", lstdata);

                            builder.CloseComponent();
                        };

                     await   dxPopup.showAsync(string.Format("Bảng kê chi tiết {0}", MaKH));
                        await dxPopup.ShowAsync();
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                    }
                }
                else
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi " + ex.Message));
            }






        }



    }

}
