using BlazorBootstrap;
using DevExpress.Data.Filtering;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_DeNghiTheoDinhMuc;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucNVLMaster;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using NFCWebBlazor.App_Admin;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_NvlTonKhoDongBoSP
    {
        [Inject] ToastService toastService { get; set; } = default!;
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        App_ClassDefine.ClassProcess prs = new ClassProcess();
        bool CheckQuyen = false;
        protected override async Task OnInitializedAsync()
        {

            try
            {
                await loadAsync();
                CheckQuyen = await phanQuyenAccess.CreateNhapXuatKho(Model.ModelAdmin.users);
                var dimension = await browserService.GetDimensions();
                // var heighrow = await browserService.GetHeighWithID("divcontainer");
                int height = dimension.Height - 70;
                int width = dimension.Width;
                if (width < 768)
                {
                    Ismobile = true;

                }
                else
                {
                    Ismobile = false;

                }



                heightgrid = string.Format("{0}px", height);
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi browserService :" + ex.Message));
            }

        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (boolExpand == false)
            {
                if (dxGrid.GetVisibleRowCount() > 0)
                {
                    loadExpand();
                    boolExpand = true;
                }


            }
            return base.OnAfterRenderAsync(firstRender);
        }
        private void KhachHang_SelectedItemChanged(DataDropDownList e)
        {
            if (e != null)
            {
                lstsanphamfilter = null;
                lstsanphamfilter = lstsanpham.Where(p => p.KhachHang == e.Name).ToList();

            }
        }

        private async Task loadAsync()
        {
            try
            {

                //var queryngn = await Model.ModelData.Getlstnoigiaonhan();
                //lstnoigiaonhan = queryngn.Where(p => p.TypeName == "NhaCungCap").ToList();
                lstsanpham = await ModelData.GetSanPham();
                lstKhachHang = lstsanpham.Where(p => !string.IsNullOrEmpty(p.KhachHang)).GroupBy(p => p.KhachHang).Select(p => new DataDropDownList { Name = p.Key, FullName = p.Key }).ToList();

            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
            }
            //baocaoselected = lstloaibaocao.FirstOrDefault(p => p.Name.Equals("Tồn kho theo sản phẩm"));


        }

        bool checkxoancc = false;
        class GroupCount
        {
            public string Group { get; set; }
            public string GroupLevel1 { get; set; }
            public int Count { get; set; }
        }


        CustomRoot customRoot { get; set; } = new CustomRoot();



        List<DataDropDownList> listaddd = new List<DataDropDownList>();
        List<GroupCount> lstgroupTenDM = new List<GroupCount>();
        List<GroupCount> lstgroupMauSP = new List<GroupCount>();
        List<DinhMucVatTuShow> lstdata = new List<DinhMucVatTuShow>();
        bool boolExpand = false;
        private async Task SanPham_SelectedItemChanged(SanPhamDropDown e)
        {

            if (e != null)
            {
                isWait = true;
                try
                {
                    boolExpand = false;
                    string sqlkehoach = "";
                    dxGrid.ClearFilter();


                    if (customRoot.lstdinhmuc == null)
                        customRoot.lstdinhmuc = new List<DinhMucVatTuShow>();


                    if (customRoot.lstkehoach == null)
                        customRoot.lstkehoach = new List<KeHoachSuDung>();

                    customRoot.lstdinhmuc.Clear();
                    customRoot.lstkehoach.Clear();
                    customRoot.lstkehoachcongdoan.Clear();

                    dxGrid.Reload();
                    //await dxGrid.SaveChangesAsync();

                    //if (checkkehoachall)
                    //{
                    //    sqlkehoach = string.Format(@"exec SP.DataBase_ScansiaPacific2014.dbo.KeHoachSP_NVL @MaSP=@MaSP,@ShowAll=1", e.MaSP);
                    //}
                    //else
                    //{
                    //    sqlkehoach = string.Format(@"exec SP.DataBase_ScansiaPacific2014.dbo.KeHoachSP_NVL @MaSP=@MaSP,@ShowAll=0", e.MaSP);
                    //}

                    dinhMucVatTuShowcrr.MaSP = e.MaSP;
                    //               string sql = string.Format(@"use NVLDB
                    //           declare @MaSP nvarchar(100)=N'{0}'
                    //           IF OBJECT_ID('tempdb..#TempTable') IS NOT NULL
                    //           DROP TABLE #TempTable
                    //           CREATE TABLE #TempTable (
                    //               GroupMauSP NVARCHAR(100), -- Thay đổi kiểu dữ liệu và số cột theo cấu trúc kết quả trả về
                    //               GroupNhaCungCap NVARCHAR(100),
                    //               TenSP NVARCHAR(200),
                    //            MaSP nvarchar(100),
                    //            MauSP nvarchar(100),
                    //            MaVatTu nvarchar(100),
                    //            SLQuyDoi float,
                    //            KhuVuc nvarchar(100),
                    //            NhaCungCap nvarchar(100),
                    //            CongDoan nvarchar(100),
                    //            TenDinhMuc nvarchar(100),
                    //            ChonNCC int,
                    //            Color nvarchar(100),
                    //            TenMau nvarchar(100)
                    //               ,KhuVucKH nvarchar(100)
                    //           )
                    //           EXEC('
                    //               Insert Into
                    //                #TempTable select * FROM OPENQUERY(SP, ''EXEC DataBase_ScansiaPacific2014.dbo.DinhMucSanPham_NVL @MaSP=' +@MaSP + ''')')

                    //             select tbl.*,hh.TenHang,hh.DVT,ncc.TenNCC,qry.SLTon from #TempTable tbl 
                    //            inner join dbo.NvlHangHoa hh on tbl.MaVatTu=hh.MaHang
                    //            left join dbo.NvlNhaCungCap ncc on tbl.NhaCungCap=ncc.MaNCC
                    //left join (select MaHang,sum(SLNhap-SLXuat) as SLTon from NvlNhapXuatItem group by MaHang) as qry
                    //on tbl.MaVatTu=qry.MaHang

                    //           {1}

                    //            Drop table #TempTable", e.MaSP, sqlkehoach); 
                    string sql = string.Format(@" use NVLDB
                             declare @MaSP nvarchar(100)='{0}'
                            
                            exec GetDinhMucNVL_SanPhamList @lstsanpham=@MaSP
                            select * from ##tmpdinhmuctoancuc  
                            Drop table ##tmpdinhmuctoancuc
                            {2}", e.MaSP, 1, sqlkehoach);

                    kehoachselectedprev.Clear();
                    kehoachselected = null;
                    lstdata.Clear();
                    CallAPI callAPI = new CallAPI();
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);

                    checkxoancc = false;
                    if (json != "")
                    {

                        var query = JsonConvert.DeserializeObject<CustomRoot>(json);
                        if (query != null)
                        {
                            //xulytheodungchungtheomau(customRoot.lstdinhmuc); bỏ hàm này do đã xử lý trong procedure
                            //xulytheodungchungtheoncc(customRoot.lstdinhmuc); bỏ hàm này do đã xử lý trong procedure

                            //Gán index để kiểm soát dòng lỗi
                            //Kiểm tra xem có phân biệt nhà cung cấp hay không
                            var querycheckncc = query.lstdinhmuc.Where(p => p.ChonNCC == 1).FirstOrDefault();
                            if (querycheckncc != null)
                                checkxoancc = true;
                            else
                                checkxoancc = false;
                            int index = 0;
                            foreach (var it in query.lstdinhmuc)
                            {
                                it.Index = index;
                                index++;
                            }
                            lstdata.AddRange(query.lstdinhmuc);
                            //Tạo danh sách các định mức và công đoạn


                            // var querygroupcongdoan = customRoot.lstdinhmuc.GroupBy(p => new { CongDoan = p.CongDoan, TenDinhMuc = p.TenDinhMuc, MauSP = p.MauSP,KhuVucKH=p.KhuVucKH }).Select(p => new KeHoachDinhMucCongDoan { CongDoan = p.Key.CongDoan, TenDinhMuc = p.Key.TenDinhMuc, MauSP = p.Key.MauSP,KhuVucKH=p.Key.KhuVucKH }).ToList();
                            //customRoot.lstkehoach.AddRange(customtemp.lstkehoach);



                        }

                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
                }
                finally
                {

                    isWait = false;
                    dxGrid.Reload();
                    StateHasChanged();
                }
            }
        }
        private async Task searchAsync()
        {


            try
            {
                boolExpand = false;
                string sqlkehoach = "";
                dxGrid.ClearFilter();


                if (customRoot.lstdinhmuc == null)
                    customRoot.lstdinhmuc = new List<DinhMucVatTuShow>();


                if (customRoot.lstkehoach == null)
                    customRoot.lstkehoach = new List<KeHoachSuDung>();

                customRoot.lstdinhmuc.Clear();
                customRoot.lstkehoach.Clear();
                customRoot.lstkehoachcongdoan.Clear();
                string lstsp = "";
                if(string.IsNullOrEmpty(dinhMucVatTuShowcrr.MaSP)&&string.IsNullOrEmpty(dinhMucVatTuShowcrr.KhachHang))
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng nhập ít nhất 1 thông tin để tìm kiếm"));
                    return;
                }    
                if (!string.IsNullOrEmpty(dinhMucVatTuShowcrr.MaSP))
                {
                    lstsp=dinhMucVatTuShowcrr.MaSP;
                }
                else
                {
                    if(!string.IsNullOrEmpty(dinhMucVatTuShowcrr.KhachHang))
                    {
                        var query = lstsanpham.Where(p => p.KhachHang == dinhMucVatTuShowcrr.KhachHang).GroupBy(p => 1)
                            .Select(p => new {MaSP=string.Join(";", p.Select(o => o.MaSP)) });
                        lstsp = query.FirstOrDefault().MaSP;
                    }
                }

                if(string.IsNullOrEmpty(lstsp))
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng nhập ít nhất 1 thông tin để tìm kiếm"));
                    return;
                }
                PanelVisible = true;
                string sql = string.Format(@" use NVLDB
                             declare @MaSP nvarchar(100)='{0}'
                            
                            exec GetDinhMucNVL_SanPhamList_TonKho @lstsanpham=@MaSP,@dateend='{1}'
                            select * from ##tmpdinhmuctoancuc  
                            Drop table ##tmpdinhmuctoancuc
                            ", lstsp,dtpend.Value.ToString("MM/dd/yyyy"));

                kehoachselectedprev.Clear();
                kehoachselected = null;
                lstdata.Clear();
                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);

                checkxoancc = false;
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<CustomRoot>(json);
                    if (query != null)
                    {
                        //xulytheodungchungtheomau(customRoot.lstdinhmuc); bỏ hàm này do đã xử lý trong procedure
                        //xulytheodungchungtheoncc(customRoot.lstdinhmuc); bỏ hàm này do đã xử lý trong procedure

                        //Gán index để kiểm soát dòng lỗi
                        //Kiểm tra xem có phân biệt nhà cung cấp hay không
                        var querycheckncc = query.lstdinhmuc.Where(p => p.ChonNCC == 1).FirstOrDefault();
                        if (querycheckncc != null)
                            checkxoancc = true;
                        else
                            checkxoancc = false;
                        int index = 0;
                        foreach (var it in query.lstdinhmuc)
                        {
                            it.Index = index;
                            index++;
                        }
                      
                        //lstdata.AddRange(query.lstdinhmuc);
                       customRoot.lstdinhmuc.AddRange(query.lstdinhmuc);
                        await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
                    }

                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
            }
            finally
            {

                isWait = false;
                PanelVisible = false;
                dxGrid.Reload();
                StateHasChanged();
            }

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

            //Kiểm tra nếu là group GroupNhaCungCap

            dxGrid.EndUpdate();
            dxGrid.SaveChangesAsync();
            StateHasChanged();


        }

       
        private void btShowTyLeClick(DinhMucVatTuShow dinhMucVatTuShow,int groupindex)
        {
            var queryupdate= customRoot.lstdinhmuc.Where(p => p.MaSP == dinhMucVatTuShow.MaSP);
            customRoot.lstdinhmuc.Where(p => p.MaSP == dinhMucVatTuShow.MaSP).ToList()
                .ForEach(p => p.SLDeNghi = dinhMucVatTuShow.SLDeNghi
          );
            dxGrid.ExpandGroupRow(groupindex, true);
            //dxGrid.IsGroupRowExpanded(groupindex);
            dxGrid.Reload();
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
                keHoachDinhMucCongDoan.KeyGroup = string.Format("{0}_{1}", keHoachMuaHang_Showcrr.Serial, StaticClass.Randomstring(10));
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
       

      
        DataTable dtkehoachdm;
        DataTable dtkehoachitem;
        private void initTable()
        {
            if (dtkehoachdm == null)
            {
                //Lưu ý: thứ tự cột trong bảng khởi tạo này phải giống y hệt thứ tự cột của Type_NvlKeHoachMuaHang_DinhMuc và Type_NvlKeHoachMuaHangItemVer3, nếu sai thứ tự cột là lỗi tè le luôn
                dtkehoachdm = new DataTable();
                dtkehoachdm.Columns.Add("Serial", typeof(int));
                dtkehoachdm.Columns.Add("SerialLink", typeof(int));
                dtkehoachdm.Columns.Add("TableName", typeof(string));
                dtkehoachdm.Columns.Add("KeyGroup", typeof(string));
                dtkehoachdm.Columns.Add("IDKeHoach", typeof(string));

                dtkehoachdm.Columns.Add("TenDinhMuc", typeof(string));
                dtkehoachdm.Columns.Add("CongDoan", typeof(string));
                dtkehoachdm.Columns.Add("SoLuong", typeof(decimal));
                dtkehoachdm.Columns.Add("Ngay", typeof(DateTime));
                dtkehoachdm.Columns.Add("UserInsert", typeof(string));
                dtkehoachdm.Columns.Add("NgayInsert", typeof(DateTime));
                dtkehoachdm.Columns.Add("MaSP", typeof(string));
                dtkehoachdm.Columns.Add("MaMau", typeof(string));
                dtkehoachitem = new DataTable();
                dtkehoachitem.Columns.Add("STT", typeof(int));
                dtkehoachitem.Columns.Add("ID", typeof(string));
                dtkehoachitem.Columns.Add("Serial", typeof(int));
                dtkehoachitem.Columns.Add("SerialDN", typeof(int));
                dtkehoachitem.Columns.Add("MaHang", typeof(string));
                dtkehoachitem.Columns.Add("SoLuong", typeof(double));
                dtkehoachitem.Columns.Add("SLTheoDoi", typeof(double));
                dtkehoachitem.Columns.Add("DonGia", typeof(double));
                dtkehoachitem.Columns.Add("DVT", typeof(string));
                dtkehoachitem.Columns.Add("VAT", typeof(int));
                dtkehoachitem.Columns.Add("GhiChu", typeof(string));
                dtkehoachitem.Columns.Add("MaSP", typeof(string));
                dtkehoachitem.Columns.Add("SerialLink", typeof(int));
                dtkehoachitem.Columns.Add("SLQuyDoiSP", typeof(double));
                dtkehoachitem.Columns.Add("TableName", typeof(string));
                dtkehoachitem.Columns.Add("NgayEdit", typeof(DateTime));
                dtkehoachitem.Columns.Add("NgayInsert", typeof(DateTime));
                dtkehoachitem.Columns.Add("UserInsert", typeof(string));
                dtkehoachitem.Columns.Add("TenLienKet", typeof(string));
            }
        }
      
    }
}
