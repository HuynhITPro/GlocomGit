using BlazorBootstrap;
using DevExpress.Data.Filtering;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucNVLMaster;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_DeNghiTheoDinhMuc;
using SkiaSharp;

namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Page_DeNghiTheoDinhMuc_NotPlan
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
        //private void KhachHang_SelectedItemChanged(DataDropDownList e)
        //{
        //    if (e != null)
        //    {
        //        lstsanphamfilter = null;
        //        lstsanphamfilter = lstsanpham.Where(p => p.KhachHang == e.Name).ToList();

        //    }
        //}

        private async Task loadAsync()
        {
            try
            {

                //var queryngn = await Model.ModelData.Getlstnoigiaonhan();
                //lstnoigiaonhan = queryngn.Where(p => p.TypeName == "NhaCungCap").ToList();
                lstsanpham = await ModelData.GetSanPham();
                lstKhachHang = lstsanpham.Where(p => !string.IsNullOrEmpty(p.KhachHang)).GroupBy(p => p.KhachHang).Select(p => new DataDropDownList { Name = p.Key, FullName = p.Key }).ToList();
                lstsanphamfilter = lstsanpham;
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
     
        private void XacNhanClick()
        {
            if(SLDeNghi==null||SLDeNghi==0)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng nhập số lượng đề nghị"));
                return;
            }
            KeHoachSuDung keHoachSuDung = new KeHoachSuDung();
            keHoachSuDung.MaSP= dinhMucVatTuShowcrr.MaSP;
            
                keHoachSuDung.SoLuongSP =SLDeNghi;
            keHoachSuDung.Colorhex = "";
            keHoachSuDung.Colortext = "#0000ff";
            customRoot.lstdinhmuc.Clear();
            customRoot.lstkehoach.Clear();
            customRoot.lstkehoachcongdoan.Clear();
            dxGrid.SaveChangesAsync();
            LoadSelectedKeHoachItem(keHoachSuDung);
         
        }
        private void LoadSelectedKeHoachItem(KeHoachSuDung query)
        {
            if(lstdata==null) return;
            if(lstdata.Any()==false) return;
            var querygr = lstdata
                          .GroupBy(p => new { GroupNhaCungCap = p.GroupNhaCungCap, GroupMauSP = p.GroupMauSP, TenDinhMuc = p.TenDinhMuc, CongDoan = p.CongDoan, KhuVucKH = p.KhuVucKH })
                          .Select(p => new KeHoachDinhMucCongDoan { GroupNhaCungCap = p.Key.GroupNhaCungCap, MauSP = p.Key.GroupMauSP, TenDinhMuc = p.Key.TenDinhMuc, CongDoan = p.Key.CongDoan, KhuVucKH = p.Key.KhuVucKH }).ToList();
            customRoot.lstkehoachcongdoan.AddRange(querygr);
            customRoot.lstdinhmuc.AddRange(lstdata.ToList());
            
            boolExpand = true;
            //var queryaddcongdoan = customRoot.lstkehoachcongdoan.Where(p => p.MauSP == query.MaMauKH && p.KhuVucKH == query.KhuVucKH).ToList();
            foreach (var item in customRoot.lstkehoachcongdoan)
            {

                var querycheck = item.lstkehoachcongdoanitem.Where(p => p.MaKH == query.ID).FirstOrDefault();
                if (querycheck != null)//Có rồi
                    continue;
                KeHoachDinhMucCongDoanItem keHoachDinhMucCongDoanItem = new KeHoachDinhMucCongDoanItem();
                //keHoachDinhMucCongDoanItem.MaKH = query.ID;
                keHoachDinhMucCongDoanItem.SoLuongSP = query.SoLuongSP;
                keHoachDinhMucCongDoanItem.SLDeNghi = query.SoLuongSP;
                keHoachDinhMucCongDoanItem.Colorhex = query.Colorhex;
                keHoachDinhMucCongDoanItem.Colortext = query.Colortext;
                //Xử lý số lượng đã sử dụng
                   
                item.lstkehoachcongdoanitem.Add(keHoachDinhMucCongDoanItem);
            }
            dxGrid.Reload();
            loadExpand();
            StateHasChanged();
        }
      
      

    

        private void checkedchangedItem(bool bl, DinhMucVatTuShow dinhMucVatTuShow)
        {
            var querydataview = customRoot.lstkehoachcongdoan.Where(p => p.GroupNhaCungCap == dinhMucVatTuShow.GroupNhaCungCap && p.MauSP == dinhMucVatTuShow.GroupMauSP && p.TenDinhMuc == dinhMucVatTuShow.TenDinhMuc && p.CongDoan == dinhMucVatTuShow.CongDoan && p.KhuVucKH.Equals(dinhMucVatTuShow.KhuVucKH)).FirstOrDefault();
            if (!querydataview.chk)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Công đoạn {0} phải được tick chọn trước", dinhMucVatTuShow.CongDoan)));
                return;
            }
            dinhMucVatTuShow.chk = bl;
            if (bl) { dinhMucVatTuShow.SLDeNghi = (decimal)dinhMucVatTuShow.SLConLai; }
            else { dinhMucVatTuShow.SLDeNghi = 0; }
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
        private void SLDeNghiChanged(decimal? e, DinhMucVatTuShow dinhMucVatTuShow)
        {
            if (e == null)
                return;

            if (e.Value > (decimal)dinhMucVatTuShow.SLConLai)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Số lượng đề nghị không được lớn hơn số lượng cho phép là {0}", dinhMucVatTuShow.SLConLai)));
                dinhMucVatTuShow.SLDeNghi = (decimal)dinhMucVatTuShow.SLConLai;
            }

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
        private void filterrow(IEnumerable<KeHoachSuDung> lstkhsd)
        {
            var query = lstkhsd.GroupBy(p => new { MaMau = p.MaMauKH, KhuVucKH = p.KhuVucKH }).Select(p => new { MaMau = p.Key.MaMau, KhuVucKH = p.Key.KhuVucKH }).ToList();
            dxGrid.ClearFilter();
            string mamaufilter = "";
            string khuvucfilter = "";
            foreach (var it in query)
            {
                if (mamaufilter == "")
                    mamaufilter = string.Format("'{0}'", it.MaMau);
                else
                    mamaufilter += string.Format(",'{0}'", it.MaMau);
                if (khuvucfilter == "")
                    khuvucfilter = string.Format("'{0}'", it.KhuVucKH);
                else
                    khuvucfilter += string.Format(",'{0}'", it.KhuVucKH);

            }
            mamaufilter = string.Format("({0})", mamaufilter);
            khuvucfilter = string.Format("({0})", khuvucfilter);

            var filterCriteria = CriteriaOperator.Parse(string.Format("[GroupMauSP] in {0} AND [KhuVucKH] in {1}", mamaufilter, khuvucfilter));
            //var filterCriteria =
            // new InOperator("GroupMauSP", query.Select(c => c.MaMau));
            //var filterCriteria2 =new InOperator("KhuVucKH", query.Select(c => c.KhuVucKH));
            dxGrid.SetFilterCriteria(filterCriteria);



            //dxGrid.SetFilterCriteria(filterCriteria2);
            //dxGrid.FilterBy("MauSP", GridFilterRowOperatorType.Equal, query.Select(p => p.MaMau).ToList());
        }
        private bool checklogic()
        {
            if (!checksave)
                return false;
            if (customRoot.lstdinhmuc == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                return false;
            }
            if (!customRoot.lstdinhmuc.Any())
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                return false;
            }
            var querycheckkehoach = customRoot.lstkehoachcongdoan.Where(p => p.chk.Equals(true)).ToList();
            if (!querycheckkehoach.Any())
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ít nhất 1 công đoạn"));
                return false;
            }

            var query = customRoot.lstdinhmuc.Where(p => p.chk.Equals(true));
            if (!query.Any())
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng Đề nghị ít nhất 1 loại định mức"));
                return false;
            }
            var checksl = query.Where(p => p.SLDeNghi > (decimal)p.SLConLai);
            if (checksl.Any())
            {
                foreach (var item in checksl)
                {
                    item.Err = "SL đề nghị không được vượt quá SL Còn lại";
                }
                return false;
            }
            return true;
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
        bool checksave = true;
        private async Task saveAsync()
        {
            initTable();
            dtkehoachdm.Clear();
            dtkehoachitem.Clear();
            //KeyGroup tạo ở sự kiện khi checkbox của công đoạn được click
            if (checklogic())
            {
                var querycheccongdoan = customRoot.lstkehoachcongdoan.Where(p => p.chk.Equals(true)).ToList();
                foreach (var it in querycheccongdoan)
                {
                    foreach (var item in it.lstkehoachcongdoanitem)
                    {
                        if (item.SLDeNghi > 0)
                        {
                            //item.KeyGroup= string.Format("{0}_{1}", keHoachMuaHang_Showcrr.Serial, StaticClass.Randomstring(10));
                            DataRow dataRow = dtkehoachdm.NewRow();
                            dataRow["Serial"] = 0;
                            dataRow["SerialLink"] = keHoachMuaHang_Showcrr.Serial;
                            //Thay Mã KH bằng MaSP SP luôn
                            dataRow["IDKeHoach"] = item.MaKH;
                            //dataRow["IDKeHoach"] = dinhMucVatTuShowcrr.MaSP;
                            dataRow["TableName"] = "NvlKehoachMuaHang";
                            dataRow["TenDinhMuc"] = it.TenDinhMuc;
                            dataRow["KeyGroup"] = it.KeyGroup;
                            dataRow["CongDoan"] = it.CongDoan;
                            dataRow["SoLuong"] = item.SLDeNghi;
                            dataRow["UserInsert"] = ModelAdmin.users.UsersName;
                            dataRow["MaSP"] = dinhMucVatTuShowcrr.MaSP;
                            dataRow["MaMau"] = it.MauSP;
                            dtkehoachdm.Rows.Add(dataRow);
                        }
                    }
                }
                var queryitem = customRoot.lstdinhmuc.Where(p => p.chk.Equals(true));
                foreach (var it in queryitem)
                {
                    DataRow rownew = dtkehoachitem.NewRow();
                    rownew["STT"] = it.Index;
                    rownew["Serial"] = 0;
                    rownew["SerialDN"] = keHoachMuaHang_Showcrr.Serial;
                    rownew["MaHang"] = it.MaVatTu;
                    rownew["SoLuong"] = it.SLDeNghi;
                    rownew["SLTheoDoi"] = it.SLDeNghi;
                    rownew["DonGia"] = 0;
                    rownew["DVT"] = it.DVT;

                    rownew["ID"] = it.KeyGroup;
                    rownew["TableName"] = "NvlKeHoachMuaHang_DinhMuc";
                    rownew["TenLienKet"] = it.MaKH;
                    rownew["UserInsert"] = ModelAdmin.users.UsersName;
                    dtkehoachitem.Rows.Add(rownew);
                }
                checksave = false;//Khóa lại

                CallAPI callAPI = new CallAPI();

                string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_InsertTableDinhMuc_NotPlan";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Type_NvlKeHoachMuaHang_DinhMuc_Ver2", prs.ConvertDataTableToJson(dtkehoachdm), "DataTable"));
                lstpara.Add(new ParameterDefine("@Type_NvlKeHoachMuaHangItem", prs.ConvertDataTableToJson(dtkehoachitem), "DataTable"));
                lstpara.Add(new ParameterDefine("@SerialDN", keHoachMuaHang_Showcrr.Serial));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));

                try
                {
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<KetquaResult>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            //msgBox.Show("Lưu thành công", IconMsg.iconssuccess);
                            toastService.Notify(new(ToastType.Success, $"Lưu thành công."));
                            if(GotoMainForm.HasDelegate)
                            {
                               await GotoMainForm.InvokeAsync(keHoachMuaHang_Showcrr.Serial);
                            }
                            //reset();

                            checksave = true;
                        }
                        else
                        {
                            string err = "";
                            if (query[0].TenDinhMuc != null)
                            {
                                err = string.Format("Lỗi: {0} của {1}:{2}", query[0].CongDoan, query[0].TenDinhMuc, query[0].ketqua);
                                toastService.Notify(new ToastMessage(ToastType.Warning, err));

                            }
                            else
                                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception));
                            //if (query[0].ketquaexception != null)
                            //{
                            //    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception));
                            //}

                        }
                        checksave = true;

                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, "Đã xảy ra lỗi."));
                    }


                }
                catch (Exception ex)
                {
                   
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex));
                }
                finally
                {
                    checksave = true;
                }
            }
        }
       
    }
}

