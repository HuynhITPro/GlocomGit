using BlazorBootstrap;
using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using SkiaSharp;
using System.Data;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucNVLMaster;



namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Page_DeNghiTheoDinhMuc
    {
        [Inject] ToastService toastService { get; set; } = default!;
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        App_ClassDefine.ClassProcess prs = new ClassProcess();

        public class KeHoachSuDung
        {
            public string ID { get; set; }
            public double SoLuongSP { get; set; }
            public string MaSP { get; set; }
            public string KhuVucKH { get; set; }
            public string MaMauKH { get; set; }
            public string TenMau { get; set; }
            public uint? Color { get { return _color; } set { _color = value; Colorhex = StaticClass.UIntToHtmlColor(_color); Colortext = StaticClass.GetContrastColor(Colorhex); } }
            private uint? _color { get; set; }
            public string Colortext { get; set; }
            public string Colorhex
            {
                get; set;

            }
        }

        public class KeHoachLoadUsed()
        {
            public string IDKeHoach { get; set; }
            public string TenDinhMuc { get; set; }

            public string CongDoan { get; set; }
            public decimal SoLuong { get; set; }
        }
        public class KeHoachDinhMucCongDoanItem
        {


            public string MaKH { get; set; }

            public double SoLuongSP { get; set; } = 0;
            public double SLDeNghi { get; set; } = 0;
            public decimal? SLConLai { get; set; } = 0;
            public double? TyLe { get; set; } = 0;
            public string Colortext { get; set; }
            public string Colorhex { get; set; }

        }
        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<DinhMucVatTuShow> lstdinhmuc { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<KeHoachSuDung> lstkehoach { get; set; }

            public List<KeHoachDinhMucCongDoan> lstkehoachcongdoan { get; set; } = new List<KeHoachDinhMucCongDoan>();//Sử dụng để lưu danh sách các Kế hoạch khi phân bổ
        }
        public class KetquaResult
        {
            public string? TenDinhMuc { get; set; }
            public string? CongDoan { get; set; }
            public string? ketqua { get; set; }
            public string? ketquaexception { get; set; } = "";

        }
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
                    if (checkkehoachall)
                    {
                        sqlkehoach = string.Format(@"exec SP.DataBase_ScansiaPacific2014.dbo.KeHoachSP_NVL @MaSP=@MaSP,@ShowAll=1", e.MaSP);
                    }
                    else
                    {
                        sqlkehoach = string.Format(@"exec SP.DataBase_ScansiaPacific2014.dbo.KeHoachSP_NVL @MaSP=@MaSP,@ShowAll=0", e.MaSP);
                    }

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
                    if (customRoot.lstdinhmuc == null)
                        customRoot.lstdinhmuc = new List<DinhMucVatTuShow>();
                    customRoot.lstdinhmuc.Clear();
                    if (customRoot.lstkehoach == null)
                        customRoot.lstkehoach = new List<KeHoachSuDung>();
                    customRoot.lstkehoach.Clear();
                    kehoachselectedprev.Clear();
                    kehoachselected = null;
                    customRoot.lstkehoachcongdoan.Clear();
                    CallAPI callAPI = new CallAPI();
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);

                    checkxoancc = false;
                    if (json != "")
                    {

                        customRoot = JsonConvert.DeserializeObject<CustomRoot>(json);
                        if (customRoot != null)
                        {
                            //xulytheodungchungtheomau(customRoot.lstdinhmuc); bỏ hàm này do đã xử lý trong procedure
                            //xulytheodungchungtheoncc(customRoot.lstdinhmuc); bỏ hàm này do đã xử lý trong procedure

                            //Gán index để kiểm soát dòng lỗi
                            //Kiểm tra xem có phân biệt nhà cung cấp hay không
                            var querycheckncc = customRoot.lstdinhmuc.Where(p => p.ChonNCC == 1).FirstOrDefault();
                            if (querycheckncc != null)
                                checkxoancc = true;
                            else
                                checkxoancc = false;
                            int index = 0;
                            foreach (var it in customRoot.lstdinhmuc)
                            {
                                it.Index = index;
                                index++;
                            }

                            //Tạo danh sách các định mức và công đoạn
                            var querygr = customRoot.lstdinhmuc
                            .GroupBy(p => new { GroupNhaCungCap = p.GroupNhaCungCap, GroupMauSP = p.GroupMauSP, TenDinhMuc = p.TenDinhMuc, CongDoan = p.CongDoan, KhuVucKH = p.KhuVucKH, MaSP = p.MaSP })
                            .Select(p => new KeHoachDinhMucCongDoan { GroupNhaCungCap = p.Key.GroupNhaCungCap, MauSP = p.Key.GroupMauSP, TenDinhMuc = p.Key.TenDinhMuc, CongDoan = p.Key.CongDoan, KhuVucKH = p.Key.KhuVucKH, MaSP = p.Key.MaSP }).ToList();

                            // var querygroupcongdoan = customRoot.lstdinhmuc.GroupBy(p => new { CongDoan = p.CongDoan, TenDinhMuc = p.TenDinhMuc, MauSP = p.MauSP,KhuVucKH=p.KhuVucKH }).Select(p => new KeHoachDinhMucCongDoan { CongDoan = p.Key.CongDoan, TenDinhMuc = p.Key.TenDinhMuc, MauSP = p.Key.MauSP,KhuVucKH=p.Key.KhuVucKH }).ToList();
                            customRoot.lstkehoachcongdoan.AddRange(querygr);

                        }

                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
                }
                finally
                {
                    dxGrid.Reload();
                    isWait = false;
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


        }
        private async Task<List<KeHoachLoadUsed>>? LoadkehoachdasudungAsync(string idkehoach)
        {
            string sql = @"Use NVLDB
           
            SELECT [IDKeHoach],[TenDinhMuc],[CongDoan],sum([SoLuong]) as SoLuong   
              FROM [dbo].[NvlKeHoachMuaHang_DinhMuc] where IDKeHoach=@IDKeHoach
              group by [IDKeHoach],[TenDinhMuc],[CongDoan]";
            CallAPI callAPI = new CallAPI();
            List<KeHoachLoadUsed>? lst = new List<KeHoachLoadUsed>();
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@IDKeHoach", idkehoach));
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<KeHoachLoadUsed>>(json);
                lst = query;

            }
            return lst;
        }
        private async void SelectedKeHoachItemChanged(IEnumerable<KeHoachSuDung> e)
        {

            if (e == null)
            {
                kehoachselectedprev.Clear();
                foreach (var item in customRoot.lstdinhmuc)
                {
                    item.SLKeHoach = 0;
                    item.chk = false;
                    item.SLDeNghi = 0;
                    item.MaKH = "";
                }
                dxGrid.ClearFilter();
                foreach (var it in customRoot.lstkehoachcongdoan)
                {
                    it.lstkehoachcongdoanitem.Clear();
                }

                return;
            }
            else
            {
                //string msg = e.Last().ID + " " + e.Last().TenMau + " is selected";
                // Console.WriteLine(msg);
                if (customRoot.lstdinhmuc == null) return;

                //Xác định Item vừa thay đổi
                if (e.Count() == 0)
                {
                    kehoachselectedprev.Clear();
                    foreach (var item in customRoot.lstdinhmuc)
                    {
                        item.SLKeHoach = 0;
                        item.SLDeNghi = 0;
                        item.chk = false;
                        item.MaKH = "";
                    }
                    foreach (var it in customRoot.lstkehoachcongdoan)
                    {
                        it.lstkehoachcongdoanitem.Clear();
                    }
                    dxGrid.ClearFilter();
                    return;

                }
                if (e.Count() > kehoachselectedprev.Count())
                {
                    //Add thêm vào phần tử cuối
                    var query = e.Last<KeHoachSuDung>();
                    if (query == null)
                        return;
                    kehoachselectedprev.Add(query);

                    customRoot.lstdinhmuc.ForEach(p => p.chk = false);
                    customRoot.lstkehoachcongdoan.ForEach(p => p.chk = false);

                    List<KeHoachLoadUsed> lstkehoachsd = await LoadkehoachdasudungAsync(query.ID);


                    //Xử lý những công đoạn có mã màu và khu vực trùng khớp với kế hoạch đầu vào
                    var queryaddcongdoan = customRoot.lstkehoachcongdoan.Where(p => p.MauSP == query.MaMauKH && p.KhuVucKH == query.KhuVucKH).ToList();
                    foreach (var item in queryaddcongdoan)
                    {

                        var querycheck = item.lstkehoachcongdoanitem.Where(p => p.MaKH == query.ID).FirstOrDefault();
                        if (querycheck != null)//Có rồi
                            continue;
                        KeHoachDinhMucCongDoanItem keHoachDinhMucCongDoanItem = new KeHoachDinhMucCongDoanItem();
                        keHoachDinhMucCongDoanItem.MaKH = query.ID;
                        keHoachDinhMucCongDoanItem.SoLuongSP = query.SoLuongSP;
                        keHoachDinhMucCongDoanItem.SLDeNghi = query.SoLuongSP;
                        keHoachDinhMucCongDoanItem.Colorhex = query.Colorhex;
                        keHoachDinhMucCongDoanItem.Colortext = query.Colortext;
                        //Xử lý số lượng đã sử dụng
                        foreach (var itkh in lstkehoachsd)
                        {
                            if (itkh.IDKeHoach == keHoachDinhMucCongDoanItem.MaKH)
                            {
                                if (item.TenDinhMuc.ToLower() == itkh.TenDinhMuc.ToLower() && item.CongDoan.ToLower() == itkh.CongDoan.ToLower())
                                {
                                    keHoachDinhMucCongDoanItem.SLDeNghi = keHoachDinhMucCongDoanItem.SLDeNghi - (double)itkh.SoLuong;
                                    keHoachDinhMucCongDoanItem.SoLuongSP = keHoachDinhMucCongDoanItem.SLDeNghi;
                                    break;
                                }
                            }

                        }
                        item.lstkehoachcongdoanitem.Add(keHoachDinhMucCongDoanItem);
                    }

                    //var querygr = customRoot.lstdinhmuc.Where(p => p.KhuVucKH == query.KhuVucKH && query.MaMauKH == p.GroupMauSP)
                    //    .GroupBy(p => new { TenDinhMuc = p.TenDinhMuc, CongDoan = p.CongDoan, GroupMauSP = p.GroupMauSP, KhuVucKH = p.KhuVucKH,GroupNhaCungCap=p.GroupNhaCungCap })
                    //    .Select(p => new KeHoachDinhMucCongDoan { TenDinhMuc = p.Key.TenDinhMuc,GroupNhaCungCap=p.Key.GroupNhaCungCap, CongDoan = p.Key.CongDoan, MauSP = p.Key.GroupMauSP, KhuVucKH = p.Key.KhuVucKH, MaKH = query.ID, SoLuongSP = query.SoLuongSP, SLDeNghi = query.SoLuongSP, Colorhex = query.Colorhex, Colortext = query.Colortext }).ToList();
                    //customRoot.lstkehoachcongdoan.AddRange(querygr);
                    filterrow(e);
                    loadExpand();
                    return;
                }
                if (e.Count() < kehoachselectedprev.Count())
                {
                    //Có 1 phần tử đã bị xóa
                    //Đánh chỉ mục cho list để tăng tốc độ tìm kiếm
                    var list2Ids = new HashSet<string>(e.Select(y => y.ID));
                    // Lọc phần tử chỉ có trong list1
                    var onlyInListPrev = kehoachselectedprev.Where(x => !list2Ids.Contains(x.ID)).ToList();//Contains của Hashset là so sánh tuyệt đối, khác với Contains của string
                    kehoachselectedprev = e.ToList();
                    var grkhuvuc = onlyInListPrev.GroupBy(p => new { KhuVucKH = p.KhuVucKH, MaMauKH = p.MaMauKH }).Select(p => new { KhuVucKH = p.Key.KhuVucKH, SoLuong = p.Sum(n => n.SoLuongSP), KeHoach = string.Join(";", p.Select(n => n.ID)), MaMauKH = p.Key.MaMauKH }).ToList();

                    customRoot.lstdinhmuc.ForEach(p => p.chk = false);
                    //Xử lý xóa phần tử trong lstkehoachcongdoan
                    foreach (var it in onlyInListPrev)
                    {
                        var queryremovecongdoan = customRoot.lstkehoachcongdoan.Where(p => p.MauSP == it.MaMauKH && p.KhuVucKH == it.KhuVucKH).ToList();
                        foreach (var item in queryremovecongdoan)
                        {
                            item.lstkehoachcongdoanitem.RemoveAll(p => p.MaKH == it.ID);
                        }

                    }

                    filterrow(e);
                    loadExpand();
                    return;
                }

            }
        }
        private void xulytheodungchungtheomau(List<DinhMucVatTuShow> lst)
        {
            if (lst != null)
            {
                var query = lst.Where(p => string.IsNullOrEmpty(p.MauSP)).ToList();//Nhóm dùng chung
                if (query.Any())
                {
                    //Xử lý tạo ra nhóm mã màu
                    var queryMauSPgroup = lst.Where(p => !string.IsNullOrEmpty(p.MauSP))
                         .GroupBy(p => new { MauSP = p.MauSP, TenMau = p.TenMau, Colorhex = p.Colorhex }).Select(p => new { MauSP = p.Key.MauSP, TenMau = p.Key.TenMau, Colorhex = p.Key.Colorhex }).ToList();

                    foreach (var item in queryMauSPgroup)
                    {
                        foreach (var it in query)
                        {
                            DinhMucVatTuShow dinhmuc = it.CopyClass();

                            dinhmuc.GroupMauSP = item.MauSP;
                            dinhmuc.TenMau = item.TenMau;
                            dinhmuc.Colorhex = item.Colorhex;
                            lst.Add(dinhmuc);
                        }

                    }
                    lst.RemoveAll(p => string.IsNullOrEmpty(p.GroupMauSP));//Xóa các mã màu dùng chung đi, vì đã chuyển từ mã màu về dạng màu cụ thể rồi

                }
            }
        }
        private void xulytheodungchungtheoncc(List<DinhMucVatTuShow> lst)
        {
            if (lst != null)
            {
                var querycheck = lst.Where(p => p.ChonNCC == 1).ToList();//Có phân loại nhà cung cấp

                if (!querycheck.Any())
                {
                    //Nếu không phân biệt nhà cung cấp thì group NCC để trống hết
                    lst.ForEach(p => p.GroupNhaCungCap = "");
                    checkxoancc = false;
                    return;
                }
                else //Có phân biệt nhà cung cấp
                {
                    var query = lst.Where(p => string.IsNullOrEmpty(p.NhaCungCap)).ToList();//Nhóm dùng chung nhà cung cấp

                    if (query.Any())
                    {

                        var queryNCCgroup = lst.Where(p => !string.IsNullOrEmpty(p.NhaCungCap)).GroupBy(p => new { NhaCungCap = p.NhaCungCap }).Select(p => new { NhaCungCap = p.Key.NhaCungCap }).ToList();
                        if (queryNCCgroup.Any())
                            checkxoancc = true;
                        foreach (var item in queryNCCgroup)
                        {
                            foreach (var it in query)
                            {
                                DinhMucVatTuShow dinhmuc = it.CopyClass();
                                dinhmuc.GroupNhaCungCap = item.NhaCungCap;
                                lst.Add(dinhmuc);
                            }

                        }
                        lst.RemoveAll(p => string.IsNullOrEmpty(p.GroupNhaCungCap));//Xóa mã nhà cung cấp đi, vì đã chuyển mã nhà cung cấp về nhà cung cấp cụ thể rồi
                    }
                }


            }
        }


        private async void loadcheckallAsync(bool chk)
        {
            string sql = "";
            checkkehoachall = chk;
            //List<DinhMucVatTuShow>lst= new List<DinhMucVatTuShow>();
            if (customRoot == null)
                return;
            if (customRoot.lstdinhmuc == null)
                return;
            //lst = customRoot.lstdinhmuc.ToList();

            if (chk)
            {
                sql = string.Format(@"declare @MaSP nvarchar(100)=N'{0}'  select qry.*,isnull(mm.TenMau,'') as TenMau,mm.Color from
                 (select ID,MaSP,SoLuongSP,KhuVuc as KhuVucKH,MaMauKH from SP.DataBase_ScansiaPacific2014.dbo.[KeHoachSP]
		                where Active=0 and KhuVuc in ('KV2DH','KV3')
		                and MaSP=@MaSP
		                and datediff(MM,Ngay,getdate())<6
		                ) as qry left join SP.DataBase_ScansiaPacific2014.[dbo].MaMau mm on qry.MaMauKH=mm.MaMau", dinhMucVatTuShowcrr.MaSP);
            }
            else
            {
                sql = string.Format(@"declare @MaSP nvarchar(100)=N'{0}' select qry.*,isnull(mm.TenMau,'') as TenMau,mm.Color from
                    (select ID,MaSP,SoLuongSP,KhuVuc as KhuVucKH,MaMauKH from SP.DataBase_ScansiaPacific2014.dbo.[KeHoachSP]
		            where Active=0 and KhuVuc in ('KV2DH','KV3')
		            and MaSP=@MaSP
		            and ID in (SELECT DISTINCT  [ID_KeHoach] FROM SP.DataBase_ScansiaPacific2014.[dbo].[KH_Theodoi] where SLTheoDoi>0)
		            ) as qry left join SP.DataBase_ScansiaPacific2014.[dbo].MaMau mm on qry.MaMauKH=mm.MaMau", dinhMucVatTuShowcrr.MaSP);
            }
            //customRoot = new CustomRoot();
            CallAPI callAPI = new CallAPI();
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<KeHoachSuDung>>(json);
                customRoot.lstkehoach = query;

            }

            //dxGrid.Reload(); 
            StateHasChanged();


        }

        private void checkedchangedItem(bool bl, DinhMucVatTuShow dinhMucVatTuShow)
        {
            //Kiểm tra xem check ở group đã được check chưa
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
            // kiểm tra, tránh API lưu lại 2 lần do mạng chậm
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
                            dataRow["IDKeHoach"] = item.MaKH;
                            dataRow["TableName"] = "NvlKehoachMuaHang";
                            dataRow["TenDinhMuc"] = it.TenDinhMuc;
                            dataRow["KeyGroup"] = it.KeyGroup;
                            dataRow["CongDoan"] = it.CongDoan;
                            dataRow["SoLuong"] = item.SLDeNghi;
                            dataRow["UserInsert"] = ModelAdmin.users.UsersName;
                            dataRow["MaSP"] = it.MaSP;
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

                CallAPI callAPI = new CallAPI();

                string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_InsertTableDinhMuc_Ver2";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Type_NvlKeHoachMuaHang_DinhMuc", prs.ConvertDataTableToJson(dtkehoachdm), "DataTable"));
                lstpara.Add(new ParameterDefine("@Type_NvlKeHoachMuaHangItem", prs.ConvertDataTableToJson(dtkehoachitem), "DataTable"));
                lstpara.Add(new ParameterDefine("@SerialDN", keHoachMuaHang_Showcrr.Serial));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));

                try
                {



                    checksave = false;//Khóa lại để tránh lưu 2 lần do API bất đồng bộ

                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<KetquaResult>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            //msgBox.Show("Lưu thành công", IconMsg.iconssuccess);
                            toastService.Notify(new(ToastType.Success, $"Lưu thành công."));

                            reset();


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

                    checksave = true;

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
        private void reset()
        {
            customRoot.lstdinhmuc.Clear();
            customRoot.lstkehoachcongdoan.Clear();
            customRoot.lstkehoach.Clear();
            kehoachselected = null;

            dxGrid.Reload();
            StateHasChanged();
        }
    }
}
