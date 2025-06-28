using BlazorBootstrap;

using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static NFCWebBlazor.App_ThongTin.Page_KhachHangMaster;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Page_DinhMucHangHoaMaster
    {
      
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        SanPhamShow? sanPhamshowcrr { get; set; }
        bool CheckQuyen = false;
        List<SanPhamShow> lstall = new List<SanPhamShow>();
        private async Task loaddatadropdownAsync()
        {
            try
            {
                txtTinhTrang.SetValue = "Đang sản xuất";
                lstsanpham=await ModelData.GetSanPham();
                searchAsync();
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;
            }


        }
        private List<SanPhamShow> InitDinhMucSanPham(string MaSP)
        {
            var query = lstall.Where(p => p.MaSP == MaSP)
                .GroupBy(p => new { MaSP = p.MaSP, TenSP = p.TenSP, TenMau = p.TenMau })
                .Select(p => new SanPhamShow { MaSP = p.Key.MaSP, TenSP = p.Key.TenSP,Serial=p.Max(n=>n.Serial), TenMau = p.Key.TenMau, Color=p.Max(n=>n.Color),ArticleNumber= string.Join(";", p.Select(n => n.ArticleNumber)) })
                .ToList();
          
            List<SanPhamShow> lstshow = new List<SanPhamShow>();
            SanPhamShow sanPhamShow=new SanPhamShow();
            sanPhamShow.Serial = 0;
            sanPhamShow.MaSP = query[0].MaSP;
            sanPhamShow.TenSP = query[0].TenSP;
            sanPhamShow.ArticleNumber = "Dùng chung";
            sanPhamShow.TenMau = "";
            lstshow.Add(sanPhamShow);
            lstshow.AddRange(query);
            return lstshow;
        }
        public class SanPhamShow : INotifyPropertyChanged
        {
            public int? Serial { get; set; }
            public string MaSP { get; set; }

            public string TenSP { get; set; }
            public string KhachHang { get; set; }
            public int _mua { get; set; }
            public string PathImg { get; set; }
            public string TinhTrang { get; set; }
            public string ArticleNumber { get; set; }
            public string MaMau { get; set; }
            public string TenMau { get; set; }
            public string Colorhex { get; set; }
            private uint? _color { get; set; }
            public uint? Color
            {
                get { return _color; }
                set
                {
                    _color = value;
                    Colorhex = StaticClass.UIntToHtmlColor(_color);
                }
            }

            public int Mua
            {
                get
                {
                    return _mua;
                }
                set
                {
                    _mua = value;
                    if (_mua == 1)
                    {
                        PathImg = IconImg.CheckMark;
                        TinhTrang = "Đang sản xuất";

                        //Foreground = Model.ModelAdmin.ColorPrimary;

                    }
                    else
                    {
                        PathImg = IconImg.NotCheck;
                        TinhTrang = "Không sản xuất";

                        //Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fe5a1d"));
                    }
                    NotifyPropertyChanged("Mua");
                    NotifyPropertyChanged("TinhTrang");
                    NotifyPropertyChanged("PathImg");

                }
            }


            public List<HangHoaItem> lsthanghoaitem { get; set; }
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }
        public class HangHoaItem : INotifyPropertyChanged
        {

            public int Serial { get; set; }
            private string _maHang { get; set; }
           
            public string MaHang
            {
                get { return _maHang; }
                set
                {
                    _maHang = value;

                    NotifyPropertyChanged("MaHang");
                }
            }
            private string _tenHang { get; set; }
            public string TenHang
            {
                get { return _tenHang; }
                set
                {
                    _tenHang = value;

                    NotifyPropertyChanged("TenHang");
                }
            }
            public string Err { get; set; }
            private string _khuVuc { get; set; }
          
            public string KhuVuc
            {
                get { return _khuVuc; }
                set
                {
                    _khuVuc = value;

                    NotifyPropertyChanged("KhuVuc");
                }
            }
            public string DVT { get; set; }
            private string _chatLuong { get; set; }
            public string? ChatLuong
            {
                get { return _chatLuong; }
                set
                {
                    _chatLuong = value;

                    NotifyPropertyChanged("ChatLuong");
                }
            }
            private string? _articleNumber { get; set; }
            public string? ArticleNumber
            {
                get { return _articleNumber; }
                set
                {
                    _articleNumber = value;
                    if (_articleNumber == null)
                        _articleNumber = "Dùng chung";
                    NotifyPropertyChanged("ArticleNumber");
                }
            }
            public string TenSP { get; set; }
            public string MaSP { get; set; }
            public string? TenMau { get; set; }
            public string? PhanLoai { get; set; }

            public string? GhiChu { get; set; }

            public double? _slQuyDoi { get; set; }
          
           
            public double? SLQuyDoi
            {
                get { return _slQuyDoi; }
                set
                {
                    _slQuyDoi = value;

                    NotifyPropertyChanged("SLQuyDoi");
                }
            }
            private double? _dinhMucHaoHut { get; set; }
            public double? DinhMucHaoHut
            {
                get { return _dinhMucHaoHut; }
                set
                {
                    _dinhMucHaoHut = value;

                    NotifyPropertyChanged("DinhMucHaoHut");
                }
            }
            public uint? Color { get; set; }
            public string UserInsert { get; set; }
            public DateTime NgayInsert { get; set; }
            public string? NguoiSua { get; set; }
            public HangHoaItem CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<HangHoaItem>(json);
            }
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }

        public async Task AddItemAsync()
        {
           await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền thêm"));
                return;
            }
            SanPhamShow sanPhamShow = new SanPhamShow();
            sanPhamShow.MaSP = sanPhamshowcrr.MaSP;
            sanPhamShow.TenSP = sanPhamshowcrr.TenSP;


            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DinhMucHangHoaAdditem>(0);
                builder.AddAttribute(1, "lstdata", InitDinhMucSanPham(sanPhamshowcrr.MaSP));
                builder.AddAttribute(2, "sanPhamShowcrr", sanPhamShow);
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
            await dxPopup.showAsync("THÊM ĐỊNH MỨC SẢN PHẨM");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
         await   dxPopup.ShowAsync();
        }
     
        public async Task EditItemAsync()
        {

          await  dxFlyoutchucnang.CloseAsync();
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền sửa"));
                return;
            }

            //renderFragment = builder =>
            //{
            //    builder.OpenComponent<Urc_HangHoaAddItem>(0);
            //    builder.AddAttribute(1, "nvlHangHoaShowcrr", sanPhamshowcrr.CopyClass());
            //    builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<SanPhamShow>(this, GotoMainForm));
            //    //builder.OpenComponent(0, componentType);
            //    builder.CloseComponent();
            //};

            //dxPopup.show("SỬA THÔNG TIN");

            //dxPopup.ShowAsync();
        }
        public async Task ImportExcelAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền import"));
                return;
            }

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DinhMucImportExcel>(0);

                builder.CloseComponent();
            };

          await  dxPopup.showAsync("Import định mức từ excel");

          await  dxPopup.ShowAsync();
        }
        public void Setclass(SanPhamShow sanPhamShow_set, SanPhamShow sanPhamShow_get)
        {
            
        }
        public async void searchAsync()
        {
            string sql = string.Format(@"
                declare @tblsp Table(MaSP nvarchar(100)  primary key,TenSP nvarchar(150),KhachHang nvarchar(100),Mua int)
                  insert into @tblsp(MaSP,TenSP,KhachHang,Mua)
                  select sp.MaSP,sp.TenSP,sp.KhachHang,sp.Mua from [DataBase_ScansiaPacific2014].dbo.SanPham sp
                  where MaSP in(
	                SELECT [MaSP] FROM [DataBase_ScansiaPacific2014].[dbo].[ChiTiet_KhuVuc] where KhuVuc=''KV1N''
                 group by MaSP)
                  SELECT ROW_NUMBER() OVER (ORDER BY sp.MaSP) AS Serial,art.[MaSP],sp.TenSP ,[ArticleNumber],mm.Color,mm.TenMau,isnull(sp.Mua,1) as Mua,sp.KhachHang
                    FROM [DataBase_ScansiaPacific2014].[dbo].[ArticleNumberProduct] art
                    inner join [DataBase_ScansiaPacific2014].dbo.MaMau mm on art.MaMau=mm.MaMau
                    inner join @tblsp sp on sp.MaSP=art.MaSP ");
            string dieukien = "";
            lstdata.Clear();
            if (!string.IsNullOrEmpty(MaSPSelected))
            {
                if (dieukien == "")
                {
                    dieukien += string.Format(" where sp.MaSP=N'{0}'", MaSPSelected);
                }
                else
                {
                    dieukien += string.Format(" and sp.MaSP=N'{0}'", MaSPSelected);
                }
            }
            if (txtTinhTrang.GetValue != null)
            {
                if (txtTinhTrang.GetValue == "Đang sản xuất")
                {
                    if (dieukien == "")
                    {
                        dieukien += string.Format(" where (sp.Mua=1 or sp.Mua is null)");
                    }
                    else
                    {
                        dieukien += string.Format(" and (sp.Mua=1 or sp.Mua is null)");
                    }
                }
                if (txtTinhTrang.GetValue == "Không sản xuất")
                {
                    if (dieukien == "")
                    {
                        dieukien += string.Format(" where sp.Mua=0 ");
                    }
                    else
                    {
                        dieukien += string.Format(" and sp.Mua=0");
                    }
                }
            }


            sql = sql + dieukien;
            string sqlshowall = "";
            if(reportselectde!="Tất cả")
            {
                sqlshowall = " where MaSP in (select MaSP from NvlChiTietKhuVuc group by MaSP)";
            }
            string sqlresult = string.Format(@"use NVLDB declare @sql nvarchar(max)
                                set @sql='{0}'

                                declare @tbl table(Serial int,MaSP nvarchar(100),TenSP nvarchar(100),ArticleNumber nvarchar(100),Color nvarchar(100),
                                TenMau nvarchar(100),Mua int,KhachHang nvarchar(200))

                                insert into @tbl
                                exec SP.DataBase_ScansiaPacific2014.dbo.getTableformSqlString @SQL_QUERY=@sql

                                select * from @tbl
                                {1}", sql,sqlshowall);
            PanelVisible = true;
            try
            {

                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sqlresult, parameterDefineList);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<SanPhamShow>>(json);
                    if (query.Count > 0)
                    {
                        lstdata.Clear();
                        lstall.Clear();
                        var querysp = query.GroupBy(p => new { KhachHang = p.KhachHang, MaSP = p.MaSP, TenSP = p.TenSP }).Select(p => new SanPhamShow { KhachHang = p.Key.KhachHang, MaSP = p.Key.MaSP, TenSP = p.Key.TenSP, Mua = p.Max(n => n.Mua), Serial = p.Max(n => n.Serial) }).ToList();
                        lstdata.AddRange(querysp.Where(p => p.Mua == 1));
                        //if (lstsanpham == null)
                        //{
                        //    lstsanpham = new List<DataDropDownList>();
                        //    lstsanpham.AddRange(querysp.Select(p => new DataDropDownList { Name = p.MaSP, FullName = p.TenSP + " - " + p.MaSP, TypeName = "SP" }).ToList());
                        //}

                        lstall = query;
                        //PanelVisible = false;
                        Grid.Reload();
                       // Grid.AutoFitColumnWidths();
                    }
                }


            }
            catch (Exception ex)
            {
                PanelVisible = false;
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));

            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }



        }
        private void GotoMainForm(SanPhamShow sanPhamShow)
        {
            Setclass(sanPhamshowcrr, sanPhamShow);
            Grid.SaveChangesAsync();
        }


        private async Task deleteAsync()
        {
            await dxFlyoutchucnang.CloseAsync();

            if (!await phanQuyenAccess.CreateMaHang(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa"));
                return;
            }

            bool ketqua = await dialogMsg.Show($"XÓA  {sanPhamshowcrr.TenSP}???", $"Bạn có chắc muốn xóa  {sanPhamshowcrr.TenSP}??");
            if (ketqua)
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@MaSP", sanPhamshowcrr.MaSP));
                try
                {
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);

                    if (json != "")
                    {
                        try
                        {
                            var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                            if (query[0].ketqua == "OK")
                            {
                                toastService.Notify(new ToastMessage(ToastType.Success, $"Xóa thành công"));
                                lstdata.Remove(sanPhamshowcrr);
                                Grid.SaveChangesAsync();

                            }
                            else
                            {
                                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi {0},{1}: ", query[0].ketqua, query[0].ketquaexception)));
                            }
                        }
                        catch (Exception ex)
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                }
            }
        }
        public async void ShowFlyout(SanPhamShow sanPhamShow)
        {
            await dxFlyoutchucnang.CloseAsync();
            //CurrentEmployee = employee;
            sanPhamshowcrr = sanPhamShow;
            idflychucnang = "#" + idelement(sanPhamshowcrr.Serial);
            //IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();
        }

    }
}

