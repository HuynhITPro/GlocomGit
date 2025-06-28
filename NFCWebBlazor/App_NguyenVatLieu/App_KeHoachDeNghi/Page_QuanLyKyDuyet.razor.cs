using BlazorBootstrap;
using Blazored.Modal;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;

using NFCWebBlazor.Model;

using System.Data;


namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Page_QuanLyKyDuyet
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
            lstnguoiduyet = await Model.ModelData.Getlstusers();
            lstnguoidenghi = await Model.ModelData.Getlstusers();
            lsttrangthai = Model.ModelData.GetDataDropDownListsAsync("QuanLyDeNghi").Result.AsEnumerable();
            lstmahang = await Model.ModelData.GetHangHoa();
            querykho = await Model.ModelData.GetKhoNvl();
            var queryselect = querykho.Select(p => new DataDropDownList { Name = p.FullName, FullName = p.FullName }).ToList();
            lstkhonvl = queryselect.ToList();
            lstphongban = await ModelData.Getlstnoigiaonhan();
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
            public string? MaKho { get; set; }
            public string? TenKho { get; set; }
            public string? NguoiDuyet { get; set; }
            public DateTime DateBegin { get; set; } = DateTime.Now.AddMonths(-2);
            public DateTime DateEnd { get; set; } = DateTime.Now;
            public string? MaHang { get; set; }
            public string? TrangThai { get; set; } = "Đề nghị mua hàng";
        }

        public class NvlKHMHDHItemShow 
        {

            public int Serial { get; set; }
            public bool chk { get; set; }
            public Nullable<int> SerialDN { get; set; }

           
            public string? MaHang { get; set; }

            public string TenSP
            {
                get; set;
            }
           
            public double? SoLuong { get; set; }
            public Nullable<double> SLTheoDoi { get; set; }
            public double? DonGia { get; set; }
            public string NoiDung { get; set; }
            public string DVT { get; set; }
            public string TenHang { get; set; }
            public bool VisibleSL { get; set; }
            public Nullable<int> SerialLink
            {
                get { return _seriallink; }
                set
                {
                    _seriallink = value;
                    if (_seriallink == null || _seriallink == 0)
                    {

                        VisibleSL = false;
                    }
                    else
                        VisibleSL = true;
                }
            }
            private Nullable<int> _seriallink { get; set; }
            public string TableName { get; set; }

            public Nullable<double> SLDatHang { get; set; }
            public Nullable<double> SLSuDung { get; set; }
            public Nullable<double> TyLe
            {
                get
                {

                    return ((SoLuong > 0) ? (SoLuong - (SLTheoDoi + SLHuy)) / SoLuong : 0);
                }
            }
            public Nullable<int> STT { get; set; }
          
            public string GhiChu { get; set; }
          
            private string _huydathang { get; set; }
            public string HuyDatHang
            {
                get { return _huydathang; }
                set
                {
                    _huydathang = value;
      

                }
            }
            public string? TenLienKet { get; set; }
            public string? MaSP { get; set; }
            public Nullable<int> SoLuongSP { get; set; }
            public uint? Color { get { return _color; } set { _color = value; Colorhex = StaticClass.UIntToHtmlColor(_color); Colortext = StaticClass.GetContrastColor(Colorhex); } }
            private uint? _color { get; set; }
            public string Colorhex
            {
                get; set;

            }

            public string Colortext { get; set; }
            public string TextKiem { get; set; } = "";
            public string TextDuyet { get; set; } = "";
            public string TenMau { get; set; }
            public Nullable<System.DateTime> NgayInsert { get; set; }
            public Nullable<System.DateTime> NgayDKNhapKho { get; set; }
            public string UserInsert { get; set; }
            public string ArticleNumber { get; set; }
            public string PhanLoai { get; set; }
            public Nullable<double> SLTon { get; set; }

            private string _duyetItemMsg { get; set; }
            public bool ShowMsg { get; set; }
            public Nullable<double> SLHuy
            {
                get;
                set;
            } = 0;
          
            double? _thanhtien { get; set; }
            public Nullable<double> ThanhTien
            {
                get; set;


            }
            public NvlKHMHDHItemShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlKHMHDHItemShow>(json);
            }
         
            private string _err { get; set; }
            public string Err
            {
                get
                {
                    return _err;
                }
                set
                {
                    _err = value;
                  
                }
            }
          
            public string NguoiDuyet { get; set; }
            public string NguoiKiem { get; set; }
            public string NguoiDN { get; set; }

            public string UserDuyet { get; set; }
          
        
        }
        private async Task searchAsync()
        {

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string TableName = "";
            string LoaiDeNghi = "";
            string dieukien = "";
            string dieukienduyet = "";
            if (dieuKienTimKiem.TrangThai=="Đề nghị mua hàng")
            {
                TableName = "NvlKehoachMuaHang";
                LoaiDeNghi = "('DeNghiMuaHang')";
                dieukienduyet = " where TableName='NvlKehoachMuaHang' and LoaiDuyet=N'Duyệt'";
            }
           
            if (dieuKienTimKiem.TrangThai == "Đơn đặt hàng")
            {
                TableName = "NvlDonDatHang";
                LoaiDeNghi = "('DonDatHang')";
                dieukienduyet = " where TableName='NvlDonDatHang' and LoaiDuyet=N'Duyệt'";
            }
            if (dieuKienTimKiem.TrangThai == "Đề nghị xuất kho")
            {
                TableName = "NvlKehoachMuaHang";
                LoaiDeNghi = "('DeNghiXuatHang','DeNghiXuatHangTheoKHSX')";
                dieukienduyet = " where TableName='NvlKehoachMuaHang' and LoaiDuyet=N'Duyệt'";
            }
            string dieukienmahang = "";
            if (lstdata == null)
                lstdata = new List<NvlKHMHDHItemShow>();
            lstdata.Clear();
           
            if (dieuKienTimKiem.SerialDN != null && dieuKienTimKiem.SerialDN > 0)
            {
                dieukien = string.Format(" and Serial =@SerialDN");
                lstpara.Add(new ParameterDefine("@SerialDN", dieuKienTimKiem.SerialDN));
                dieukienduyet = string.Format(" where TableName='{0}' and SerialLink=@SerialDN and LoaiDuyet=N'Duyệt'", TableName);
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
                  
                        dieukien += " and UserInsert=@UserInsert";


                        lstpara.Add(new ParameterDefine("@UserInsert", dieuKienTimKiem.NguoiDN));
                    
                }
                if (!string.IsNullOrEmpty(dieuKienTimKiem.NguoiDuyet))
                {
                    if(dieukienduyet=="")
                        dieukienduyet = " where UserDuyet=@UserDuyet";
                    else
                        dieukienduyet += " and UserDuyet=@UserDuyet";


                    lstpara.Add(new ParameterDefine("@UserDuyet", dieuKienTimKiem.NguoiDuyet));
                }
               
                if (dieuKienTimKiem.DateBegin != null)
                {
                    dieukien += " and NgayDN>=@DateBegin";
                    if(dieukienduyet=="")
                    {
                        dieukienduyet = " where NgayInsert>=@DateBegin";
                    }
                    else
                        dieukienduyet += " and NgayInsert>=@DateBegin";
                    lstpara.Add(new ParameterDefine("@DateBegin", dieuKienTimKiem.DateBegin.ToString("MM/dd/yyyy 00:00")));
                }
                if (dieuKienTimKiem.DateEnd != null)
                {
                    dieukien += " and NgayDN<=@DateEnd";
                    lstpara.Add(new ParameterDefine("@DateEnd", dieuKienTimKiem.DateEnd.ToString("MM/dd/yyyy 23:59")));
                }
                if (!string.IsNullOrEmpty(dieuKienTimKiem.MaHang))
                {
                    dieukienmahang = " where hh.MaHang=@MaHang";
                    lstpara.Add(new ParameterDefine("@MaHang", dieuKienTimKiem.MaHang));
                }
                dieukien +=" and LoaiKeHoach in "+ LoaiDeNghi;
            }
            if (dieukien != "")
            {
                dieukien = " where " + dieukien.Substring(5);
            }
          
           

            string sql = string.Format(@" use NVLDB
                   declare @tbl Table(Serial int,NguoiDN nvarchar(100),LyDo nvarchar(100),NgayDN nvarchar(100),NoiDung nvarchar(200),GhiChu nvarchar(200))
                    insert into @tbl(Serial,NguoiDN,LyDo,NgayDN,NoiDung,GhiChu)
                    select Serial,NguoiDN,LyDo,NgayDN,NoiDung,GhiChu from NvlKehoachMuaHang {0}

                    declare @tblkyduyet table(SerialLinkItem int primary key, UserDuyet nvarchar(100))
                    insert into @tblkyduyet(SerialLinkItem,UserDuyet)
                    select SerialLinkItem,UserDuyet  
                    from dbo.NvlKyDuyetItem kd where Serial in (select max(serial) from NvlKyDuyetItem  {1} group by LoaiDuyet,SerialLinkItem)

                    SELECT tbl.*,item.[Serial],[SerialDN],item.[MaHang],hh.TenHang,hh.DVT,[SoLuong],[SLTheoDoi] ,[DonGia],item.[GhiChu],[MaSP],item.[NgayInsert],item.[UserInsert]   
                    ,kd.UserDuyet
                    FROM [NvlKeHoachMuaHangItem] item
                    inner join @tbl tbl on item.SerialDN=tbl.Serial
                    inner join 
                    @tblkyduyet as kd on item.Serial=kd.SerialLinkItem
                    inner join dbo.NvlHangHoa hh on item.MaHang=hh.MaHang
                    {2}
                    ", dieukien,dieukienduyet,dieukienmahang);

            PanelVisible = true;
            CallAPI callAPI = new CallAPI();
            try
            {

                string json = await callAPI.ExcuteQueryAsync(sql, lstpara);
                if (json != "")
                {
                    var custom = JsonConvert.DeserializeObject<List<NvlKHMHDHItemShow>>(json);
                    lstdata.AddRange(custom);
                    await JSRuntime.InvokeVoidAsync(CallNameJson.toggleCollapse.ToString(), string.Format("#{0}", randomdivhide));
                
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
    }

}
