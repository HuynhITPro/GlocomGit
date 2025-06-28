using BlazorBootstrap;
using DevExpress.Blazor;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Components;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using SkiaSharp;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_DeNghiTheoDinhMuc;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucNVLMaster;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class View_DinhMucNVLItem
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
        private void checkedchangedItem(bool bl, DinhMucVatTuShow dinhMucVatTuShow)
        {
            dinhMucVatTuShow.chk = bl;

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
                    sanPhamShowcrr.lstdinhmuc = lstdinhmucall.Where(p =>p.MaSP == sanPhamShowcrr.MaSP).ToList();
                    lstdata = sanPhamShowcrr.lstdinhmuc;
                   
                    lstkehoachcongdoan.Clear();
                    var querygr = lstdata
                         .GroupBy(p => new { GroupNhaCungCap = p.GroupNhaCungCap, GroupMauSP = p.GroupMauSP, TenDinhMuc = p.TenDinhMuc, CongDoan = p.CongDoan })
                         .Select(p => new KeHoachDinhMucCongDoan { GroupNhaCungCap = p.Key.GroupNhaCungCap, MauSP = p.Key.GroupMauSP, TenDinhMuc = p.Key.TenDinhMuc, CongDoan = p.Key.CongDoan }).ToList();
                    lstkehoachcongdoan.AddRange(querygr);
                    //Xử lý để check nếu đối vs trường hợp 1 định mức 1 công đoạn
                    var querygrcount = querygr.GroupBy(p => new {TenDinhMuc = p.TenDinhMuc, CongDoan = p.CongDoan, MauSP = p.MauSP })
                         .Select(p => new { TenDinhMuc = p.Key.TenDinhMuc, CongDoan = p.Key.CongDoan, Count = p.Count(),MauSP=p.Key.MauSP }).Where(p => p.Count > 1).ToList();
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
                    foreach (var it in lstkehoachcongdoan)
                    {

                        KeHoachDinhMucCongDoanItem keHoachDinhMucCongDoanItem = new KeHoachDinhMucCongDoanItem();
                        keHoachDinhMucCongDoanItem.SoLuongSP = 1;
                        keHoachDinhMucCongDoanItem.SLDeNghi = 1;
                        keHoachDinhMucCongDoanItem.Colorhex = sanPhamShowcrr.Colorhex;
                        keHoachDinhMucCongDoanItem.Colortext = StaticClass.GetContrastColor(sanPhamShowcrr.Colorhex);
                       // keHoachDinhMucCongDoanItem.MaKH = keHoachThang_Showcrr.Serial.ToString();
                        it.lstkehoachcongdoanitem.Add(keHoachDinhMucCongDoanItem);

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
                keHoachDinhMucCongDoan.KeyGroup = string.Format("{0}",  StaticClass.Randomstring(10));
            }
            if(dxGrid.IsGroupRow(visibleIndex))
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
        private void SLCongDoanChanged(double d, KeHoachDinhMucCongDoan keHoachDinhMucCongDoan, KeHoachDinhMucCongDoanItem keHoachDinhMucCongDoanItem, DinhMucVatTuShow dinhMucVatTuShow, int visibleIndex)
        {
            keHoachDinhMucCongDoanItem.SLDeNghi = d;
            if (keHoachDinhMucCongDoanItem.SLDeNghi > keHoachDinhMucCongDoanItem.SoLuongSP)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Số lượng đề nghị không được lớn hơn {0}", keHoachDinhMucCongDoanItem.SoLuongSP)));
                keHoachDinhMucCongDoanItem.SLDeNghi = keHoachDinhMucCongDoanItem.SoLuongSP;
                return;
            }
            keHoachDinhMucCongDoan.chk = false;
            GetGroupDataItems(false, visibleIndex);
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
               
                if(dxGrid.IsGroupRow(i))
                {
                    
                    int rowlevel = dxGrid.GetRowLevel(i);
                    if (rowlevel < n - 1)
                        dxGrid.ExpandGroupRow(i);
                    else
                        dxGrid.CollapseGroupRow(i);
                   
                   
                }
                
            }
           
            //Kiểm tra nếu là group GroupNhaCungCap

            dxGrid.EndUpdate();


        }
       

    }

}
