
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;

using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_DeNghiTheoDinhMuc;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucNVLMaster;




namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Urc_KeHoachMuaHang_AddSanPham_Detail
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
                if (keHoachThang_Showcrr.lstdinhmuc == null)
                {
                    //keHoachThang_Showcrr.lstdinhmuc = new List<DinhMucVatTuShow>();
                    keHoachThang_Showcrr.lstdinhmuc = lstdinhmucall.Where(p => p.GroupMauSP == keHoachThang_Showcrr.MaMau && p.MaSP == keHoachThang_Showcrr.MaSP).ToList();
                    lstdata = keHoachThang_Showcrr.lstdinhmuc;
                    var querycheckncc = lstdata.Where(p => p.ChonNCC == 1).FirstOrDefault();
                    if (querycheckncc != null)
                    {
                        checkncc = true;
                    }
                    else
                        checkncc = false;
                    lstkehoachcongdoan.Clear();
                    var querygr = lstdata
                         .GroupBy(p => new { GroupNhaCungCap = p.GroupNhaCungCap, GroupMauSP = p.GroupMauSP, TenDinhMuc = p.TenDinhMuc, CongDoan = p.CongDoan })
                         .Select(p => new KeHoachDinhMucCongDoan { GroupNhaCungCap = p.Key.GroupNhaCungCap, MauSP = p.Key.GroupMauSP, TenDinhMuc = p.Key.TenDinhMuc, CongDoan = p.Key.CongDoan }).ToList();
                    lstkehoachcongdoan.AddRange(querygr);
                    //Xử lý để check nếu đối vs trường hợp 1 định mức 1 công đoạn
                    var querygrcount = querygr.GroupBy(p => new { TenDinhMuc = p.TenDinhMuc, CongDoan = p.CongDoan, MauSP = p.MauSP })
                         .Select(p => new { TenDinhMuc = p.Key.TenDinhMuc, CongDoan = p.Key.CongDoan, Count = p.Count(),MauSP=p.Key.MauSP }).Where(p=>p.Count>1).ToList();
                    bool checkone = false;
                    foreach(var it in lstkehoachcongdoan)
                    {
                        checkone = true;
                        foreach(var item in querygrcount)
                        {
                            if(it.TenDinhMuc==item.TenDinhMuc && it.CongDoan==item.CongDoan&&it.MauSP==item.MauSP)
                            {
                                checkone = false;
                                break;
                            }
                        }
                        it.chk = checkone;
                        foreach(var data in lstdata)
                        {
                            if(data.TenDinhMuc==it.TenDinhMuc&&data.CongDoan==it.CongDoan && it.MauSP == data.MauSP)
                            {
                                data.chk=checkone;
                                if(checkone==true)
                                {
                                    data.SLDeNghi =(decimal)(keHoachThang_Showcrr.SLPhaiDat * data.SLQuyDoi);
                                }
                            }
                        }
                            
                        
                    }
                    foreach(var it in lstkehoachcongdoan)
                    {
                        
                        KeHoachDinhMucCongDoanItem keHoachDinhMucCongDoanItem = new KeHoachDinhMucCongDoanItem();
                        keHoachDinhMucCongDoanItem.SoLuongSP = keHoachThang_Showcrr.SLConLai;
                        keHoachDinhMucCongDoanItem.SLDeNghi = keHoachThang_Showcrr.SLPhaiDat;
                        keHoachDinhMucCongDoanItem.Colorhex = keHoachThang_Showcrr.Colorhex;
                        keHoachDinhMucCongDoanItem.Colortext = StaticClass.GetContrastColor(keHoachThang_Showcrr.Colorhex);
                        keHoachDinhMucCongDoanItem.MaKH = keHoachThang_Showcrr.Serial.ToString();
                        it.lstkehoachcongdoanitem.Add(keHoachDinhMucCongDoanItem);

                    }
                    keHoachThang_Showcrr.lstkehoachcongdoan = lstkehoachcongdoan;
                   
                }
                else
                {
                    lstdata = keHoachThang_Showcrr.lstdinhmuc;
                    lstkehoachcongdoan = keHoachThang_Showcrr.lstkehoachcongdoan;
                  
                }
            }
            //string sql = string.Format(@"use NVLDB
                           
            //                IF OBJECT_ID('tempdb..##tmpdinhmuctoancuc') IS NOT NULL
	           //                 DROP TABLE ##tmpdinhmuctoancuc
            //                exec GetDinhMucNVL_SanPhamList @lstsanpham=@lstsanpham
            //                select * from ##tmpdinhmuctoancuc
            //                where GroupMauSP=@GroupMauSP

            //                Drop table ##tmpdinhmuctoancuc");
            //string dieukien = "";
           
            //lstdata = new List<DinhMucVatTuShow>();
            //sql = sql + dieukien;
            //try
            //{

            //    CallAPI callAPI = new CallAPI();
            //    List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            //    parameterDefineList.Add(new ParameterDefine("@lstsanpham", keHoachThang_Showcrr.MaSP));
            //    parameterDefineList.Add(new ParameterDefine("@GroupMauSP", keHoachThang_Showcrr.MaMau));
            
            //    string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            //    if (json != "")
            //    {
            //        var query = JsonConvert.DeserializeObject<List<DinhMucVatTuShow>>(json);
            //        if (query.Count > 0)
            //        {
            //            var querycheckncc = query.Where(p=>p.ChonNCC==1).FirstOrDefault();
            //            if (querycheckncc != null)
            //            {
            //                checkncc = true;
            //            }
            //            else
            //                checkncc= false;
            //            lstdata.AddRange(query);
            //            var querygr = lstdata.GroupBy(p => new { GroupNCC=p.GroupNhaCungCap, GroupMauSP = p.GroupMauSP }).Select(p => new { GroupNCC = p.Key.GroupNCC, GroupMauSP = p.Key.GroupMauSP }).ToList();
            //            if (querygr.Count > 0)
            //            {

            //                foreach (var item in querygr)
            //                {
            //                    KeHoachSelected keHoachSelected = new KeHoachSelected();
            //                    keHoachSelected.GroupNhaCungCap = item.GroupNCC;
            //                    keHoachSelected.MauSP = item.GroupMauSP;
            //                    lstKeHoachSelected.Add(keHoachSelected);
            //                }
            //                if (lstKeHoachSelected.Count == 1)
            //                {
            //                    lstKeHoachSelected[0].SLDeNghi = keHoachThang_Showcrr.SLPhaiDat;
            //                }

            //            }
            //            query.Clear();

            //            //keHoachThang_Showcrr.lstitem = lstdata;
            //            //Grid.ExpandGroupRow(0);
            //            dxGrid.Reload();
            //            PanelVisible = false;

            //            //Grid.AutoFitColumnWidths();
            //        }
            //    }
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
                keHoachDinhMucCongDoan.KeyGroup = string.Format("{0}_{1}", keHoachThang_Showcrr.Serial, StaticClass.Randomstring(10));
            }

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
            if(firstRender)
            {
               
            }
            else
            {
                if(boolExpand==false)
                {
                    if(dxGrid.GetVisibleRowCount()>0)
                    {
                        loadExpand();
                        boolExpand = true;
                    }
                   

                }
                
                

            }
            return base.OnAfterRenderAsync(firstRender);
        }
        public void SLDeNghiChanged(decimal? e,KeHoachSelected keHoachSelected, DinhMucVatTuShow dinhMucVatTuShow)
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
               
                int rowlevel = dxGrid.GetRowLevel(i);
                if (rowlevel <n-1)
                    dxGrid.ExpandGroupRow(i);
                else
                    dxGrid.CollapseGroupRow(i);
            }
            dxGrid.EndUpdate();
          

        }
    }


}
