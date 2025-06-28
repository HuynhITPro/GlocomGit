using BlazorBootstrap;
using DevExpress.Blazor;

using DevExpress.XtraReports.UI;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;
using static NFCWebBlazor.App_ThongTin.Urc_NvlReportDetail;


namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Page_DonDatHang_Master
    {
        [Inject] PreloadService PreloadService { get; set; }

        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        //[CascadingParameter] IModalService Modal { get; set; } = default!;
        DateTime? dtpbegin { get; set; } = DateTime.Now.AddMonths(-2);
        DateTime? dtpend { get; set; } = DateTime.Now;


        IGrid Grid { get; set; }
        List<NVLDonDatHangShow> lstDonDatHangSearchShow { get; set; } = new List<NVLDonDatHangShow>();
        NVLDonDatHangShow nVLDonDatHangShowcrr { get; set; }
        List<Users> lstuser { get; set; }
        List<DataDropDownList> lstnhamay = new List<DataDropDownList>();
        List<DataDropDownList> lsttinhtrang = new List<DataDropDownList>();
        List<Users> lstnguoidenghi { get; set; }
        List<Users> lstnguoiduyet { get; set; }
        CustomRoot customRoot { get; set; } = new CustomRoot();
        DataDropDownList? NhaMaySelected { get; set; }
        DataDropDownList? TinhTrangSelected { get; set; }
        Users? nguoiduyet { get; set; }
        
        public string serialDN { get; set; } = "";
        public string LoaiDonHang { get; set; }
        public class NVLDonDatHangShow : INotifyPropertyChanged
        {

            public int Serial { get; set; }
            public string MaDatHang { get; set; }

        
            public string KhuVuc
            {
                get; set;
            } = "";
            public string NoiDungDeNghi { get; set; }
            public string DVTT { get; set; }
            public string TenKhuVuc { get; set; }
            public string SoHD { get; set; }
            public Nullable<int> STTDH { get; set; }

            public Nullable<System.DateTime> NgayTao { get; set; }
            public Nullable<System.DateTime> NgayMax { get; set; }
      

            public Nullable<System.DateTime> NgayDatHang { get; set; }
            public Nullable<System.DateTime> NgayInsert { get; set; }
            public Nullable<int> STTDHNCC { get; set; }
            public string GhiChu { get; set; }
        
            public string PhongBan { get; set; } = string.Empty;

            public string ActiveDienGiai { get; set; }
            private int _active { get; set; }
            public int Active
            {
                get
                {
                    return _active;
                }
                set
                {
                    _active = value;
                    if (_active == 0)
                    {
                        ActiveDienGiai = "Đã dừng đặt hàng";

                    }
                    else
                    {
                        ActiveDienGiai = "";

                    }
                    NotifyPropertyChanged("ActiveDienGiai");
                }
            }

            public string PathImgDuyet { get; set; } = Model.ModelAdmin.pathurlfilepublic + "/UserImage/user.png";
            public string PathImgKiemTra { get; set; } = Model.ModelAdmin.pathurlfilepublic + "/UserImage/user.png";
            public string PathImgTao { get; set; } = Model.ModelAdmin.pathurlfilepublic + "/UserImage/user.png";
            public string UserInsert { get; set; }
            public string NguoiDN { get; set; }
            public string ShowTextDuyet { get;set; }
            private string? _khongduyet { get; set; }
            public string? KhongDuyet
            {
                get { return _khongduyet; }
                set
                {
                    _khongduyet = value;
                    if (!string.IsNullOrEmpty(_khongduyet))
                    {
                        ShowTextDuyet = "Không Duyệt";

                    }
                }
            }
            public bool EnableButtonDuyet { get; set; } = true;
            public ObservableCollection<NVLDonDatHangItemShow> lstitem { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            public NVLDonDatHangShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NVLDonDatHangShow>(json);
            }
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            public double? TyLe { get; set; }
            private string _PathIcon { get; set; }
            public string PathIcon
            {
                get { return _PathIcon; }
                set
                {
                    _PathIcon = value;

                    NotifyPropertyChanged("PathIcon");
                }
            }
            public List<FileHoSoGroup> lstfilehoso { get; set; }
            public void setlstfilehoso(List<FileHoSoGroup> lst)
            {
                lstfilehoso = lst;
            }
            public List<NvlKyDuyetShow> lstkyduyet { get; set; }
            public string PathImg { get; set; }
            public string LoaiDonHang { get; set; }
   
            public string MaNCC { get; set; } = string.Empty;
            public string TenNCC { get; set; }

            public string Err { get; set; }
            private string _tinhtrang { get; set; }
            private string _tinhTrang { get; set; }
            public string TinhTrang
            {
                get { return _tinhTrang; }
                set
                {
                    _tinhTrang = value;
                    if (_tinhTrang == "Đã duyệt")
                        PathDuyet = IconImg.CheckMark;
                    else
                        PathDuyet = IconImg.NotCheck;
                    NotifyPropertyChanged("TinhTrang");

                }
            }
            public string PathDuyet
            {
                get;
                set;

            }

            public int? CountDaMua
            {
                get; set;
            }
            public string textDaMua
            {
                get
                {
                    if (CountDaMua == null || CountDaMua == 0)
                    {
                        Imgnew = IconImg.New;
                        //return "(Mua lần đầu tiên)";
                        return "";
                    }
                        
                    return string.Format("Đã mua {0} lần", CountDaMua.Value.ToString("#,#"));
                }

            }
            public string NguoiDuyet { get; set; }
            public string NguoiKiem { get; set; }
            public string Imgnew { get; set; }
            public bool checkUpdate { get; set; }
            public int? CountTong { get; set; }
            public int? CountDuyet { get; set; }
        }

        public class NVLDonDatHangItemShow : INotifyPropertyChanged
        {
            public object this[string propertyName]
            {
                get
                {
                    // probably faster without reflection:
                    // like:  return Properties.Settings.Default.PropertyValues[propertyName] 
                    // instead of the following
                    Type myType = typeof(NVLDonDatHangItemShow);
                    PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                    return myPropInfo.GetValue(this, null);
                }
                set
                {
                    Type myType = typeof(NVLDonDatHangItemShow);
                    PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                    myPropInfo.SetValue(this, value, null);
                }
            }
            public NVLDonDatHangItemShow()
            {
                isChanged = false;

            }
            public int Serial { get; set; }
            public int SerialMaDH { get; set; }
            public int SerialDN { get; set; }

            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string TenSP { get; set; }
            private Nullable<decimal> _slDatHang { get; set; }

            public Nullable<decimal> SLDatHang
            {
                get { return _slDatHang; }
                set
                {

                    _slDatHang = value;
                    NotifyPropertyChanged("SLDatHang");
                }
            }
            public Nullable<double> SLTheoDoi { get; set; }
            public Nullable<double> SLTon { get; set; }
            public Nullable<double> MinTK { get; set; }
            public Nullable<double> SLHuy { get; set; }
            public Nullable<double> MaxTK { get; set; }
            public Nullable<decimal> SoLuong { get; set; }
            public Nullable<int> STT { get; set; }
            public Nullable<decimal> SLConLai { get; set; }
            private Nullable<decimal> _sldatthem { get; set; }
            public Nullable<decimal> SLDatThem
            {
                get { return _sldatthem; }
                set
                {
                    _sldatthem = value;
                    if (_sldatthem > SLConLai)
                    {
                        Err = "Số lượng đặt thêm không được lớn hơn số lượng còn lại";
                        _sldatthem = SLConLai;
                    }
                }
            }
            public string TextKiem { get; set; } = "";
            public string TextDuyet { get; set; } = "";
            public Nullable<double> TyLe
            {
                get
                {
                    if (SoLuong is null)
                        return null;

                    return (double)((SoLuong.Value > 0) ? (SLConLai / SoLuong) : 0);
                }
                set { }
            }

            public string DVT { get; set; }
            public bool DaDuyet { get; set; } = false;
            private Nullable<decimal> _donGia { get; set; }
            public Nullable<decimal> DonGia
            {
                get { return _donGia; }
                set
                {

                    _donGia = value;
                    ThanhTien = SLDatHang * _donGia;
                    NotifyPropertyChanged("DonGia");
                }
            }
            public Nullable<decimal> DonGiaGanNhat
            {
                get; set;
            }
            public Nullable<double> DonGiaDeNghi
            {
                get; set;
            }
            public Nullable<int> VAT { get; set; }
            public Nullable<int> SignInt { get; set; }
            public Nullable<int> SerialLink { get; set; }
            public Nullable<decimal> ThanhTien { get; set; }

            public string MaNCC
            {
                get; set;
            }
            public string TenNCC { get; set; }
            public Nullable<DateTime> NgayDKNhapKho { get; set; }
            public DateTime NgayInsert { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            private bool _ischanged { get; set; }
            public bool isChanged
            {
                get { return _ischanged; }
                set
                {

                    _ischanged = value;
                    NotifyPropertyChanged("isChanged");
                }
            }
            public string UserInsert { get; set; }
            public string NoiDung { get; set; }
            public string PhanLoai { get; set; }
            public decimal? DonGiaDuyet { get; set; }
            public List<NvlKyDuyetItemShow> lstduyetitem { get; set; }
            private string _err { get; set; }
            public string Err
            {
                get { return _err; }
                set
                {

                    _err = value;
                    NotifyPropertyChanged("Err");
                }
            }
            public NVLDonDatHangItemShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NVLDonDatHangItemShow>(json);
            }
            public string MsgKhongDuyet { get; set; }
        }

        bool CheckQuyen = false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
              
               // CheckQuyen = await phanQuyenAccess.CreateDonDatHang(ModelAdmin.users);
                List<DataDropDownList> lst = await Model.ModelData.GetDataDropDownListsAsync();
                lstnhamay = lst.Where(p => p.TypeName == "NhaMay_NVL").ToList();
                lsttinhtrang = lst.Where(p => p.TypeName == "KyDuyetStatus").ToList();
                lstuser = await Model.ModelData.Getlstusers();
                if (ModelAdmin.lstmenuitems != null)
                {
                    if (LoaiDonHang == null)
                    {
                        MenuItem query = ModelAdmin.lstmenuitems.FirstOrDefault(p => p.NameItem.Equals(ModelAdmin.pageclickcurrent));
                        if (query != null)
                        {
                            if (query.Tag != null)
                                LoaiDonHang = query.Tag.ToString();
                        }
                    }
                }
                if (LoaiDonHang == "DonHangDeNghi")
                {
                    Visilethemtukehoach = false;
                }
                if (LoaiDonHang == "DonHangKeHoach" || LoaiDonHang == "DonHangSanXuat")
                {
                    if (CheckQuyen)
                        Visilethemtukehoach = true;
                }
                var dimension = await browserService.GetDimensions();
                // var heighrow = await browserService.GetHeighWithID("divcontainer");
                int height = dimension.Height - 70;
                heightgrid = string.Format("{0}px", height);
                int width = dimension.Width;
                if (width < 768)
                {
                    Ismobile = true;
                 
                }
                else
                    Ismobile = false;
                texttaomoi = "TẠO ĐƠN HÀNG";
                //Visilethemtukehoach = true;
                //randomdivhide = prs.RandomString(10);
                lstnguoidenghi = lstuser.ToList();
                lstnguoiduyet = lstuser.ToList();
                //nguoidenghi = ModelAdmin.users.UsersName;
                // Grid.Data = lstDonDatHangSearchShow;

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load form ke hoach master " + ex.Message);
            }
            finally
            {
                PreloadService.Hide();
            }
            //return base.OnInitialized();
        }
        bool firstload = false;
        public string ShowTextDuyet { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                try
                {
                    load();
                   
                    //base.OnAfterRender(firstRender);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }
            if (!firstload)//Load lần đầu để gán biến
            {
                if (lstnguoidenghi != null)
                {

                    TinhTrangSelected = lsttinhtrang.Where(p => p.FullName == "Chưa duyệt").FirstOrDefault();
                    if (LoaiDonHang == null)
                    {
                        MenuItem query = ModelAdmin.lstmenuitems.FirstOrDefault(p => p.NameItem.Equals(ModelAdmin.pageclickcurrent));
                        if (query != null)
                        {
                            if (query.Tag != null)
                                LoaiDonHang = query.Tag.ToString();
                        }
                    }
                    if (LoaiDonHang == null)
                    {
                        LoaiDonHang = "KyDuyet";
                    }
                    if (LoaiDonHang.Contains("KyDuyet"))
                    {
                        Visileprint = true;
                        Visilechinhsua = false;
                        Visilechinhtruocin = false;
                        Visilechinhtruocin = false;
                        Visilechonnguoiduyet = false;
                        Visiledelete = false;
                        Visilethemchitiet = false;
                        VisbleTaoDeNghi = false;

                        nguoiduyet = lstnguoiduyet.FirstOrDefault(p => p.UsersName.Equals(ModelAdmin.users.UsersName));

                    }
                    else
                    {
                        nguoidenghi = ModelAdmin.users.UsersName;
                       // nguoidenghi = lstnguoidenghi.FirstOrDefault(p => p.UsersName.Equals(ModelAdmin.users.UsersName));
                    }
                    //if (!Ismobile)
                    //{

                    //    renderFragmentflowchart = builder =>
                    //    {
                    //        builder.OpenComponent<Urc_Stepper_FlowChart>(0);
                    //        builder.AddAttribute(1, "TypeName", LoaiDonHang);

                    //        builder.CloseComponent();
                    //    };
                    //}
                    firstload = true;
                    if (LoaiDonHang.Contains("KyDuyet"))
                    {
                        await searchAsync();
                    }
                    StateHasChanged();
                }

            }
            //return base.OnAfterRenderAsync(firstRender);
        }
        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NVLDonDatHangShow> lstmuahang { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetShow> lstkyduyet { get; set; }
        }
        public class CustomRootItem
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NVLDonDatHangItemShow> lstmuahangitem { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetItemShow> lstkyduyet { get; set; }
            [JsonProperty("Table2")]
            public List<NvlMsgManage> lstMsg { get; set; }
        }

        private async Task searchAsyncOld()
        {
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = "";
            string dieukienserial = "";
            string dieukienduyet = "";
            if (SerialDN != null)
            {
                dieukien = " where Serial=@SerialDN";
                lstpara.Add(new ParameterDefine("@SerialDN", SerialDN));
            }
            else
            {
                if (dtpbegin == null)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ngày để xem"));
                    return;
                }
                if (!LoaiDonHang.Contains("KyDuyet"))
                {
                    if (dieukien == "")
                        dieukien = string.Format(" where LoaiDonHang like N'%MuaHang%'", LoaiDonHang);
                    else
                        dieukien += string.Format(" and LoaiDonHang like N'%MuaHang%'", LoaiDonHang);

                }

                if (dtpbegin != null)
                {
                    if (dieukien == "")
                        dieukien = " where NgayInsert>=@DateBegin";
                    else
                        dieukien += " and NgayInsert>=@DateBegin";
                    lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));

                }
               
                if (nguoiduyet != null)
                {

                    lstpara.Add(new ParameterDefine("@UserDuyet", nguoiduyet.UsersName));
                    if (dieukienduyet == "")
                    {
                        dieukienduyet = " where tbl.Serial in (select [SerialLink] from @tblkyduyet)";
                    }
                    else
                        dieukienduyet += " and tbl.Serial in (select [SerialLink] from @tblkyduyet)";
                }
                else
                    lstpara.Add(new ParameterDefine("@UserDuyet", ""));

                if (dtpend != null)
                {
                    if (dieukien == "")
                        dieukien = " where NgayInsert<=@DateEnd";
                    else
                        dieukien += " and NgayInsert<=@DateEnd";
                    lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
                }

                if (TinhTrangSelected != null)
                {
                    string duyet = TinhTrangSelected.Name.ToString();
                    if (duyet == "Chưa duyệt")
                    {
                        if (dieukienduyet == "")
                            dieukienduyet = @" where (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)>0 or CountTong is null)";
                        else
                            dieukienduyet += @" and (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)>0 or CountTong is null)";
                        //GrvdetailGrid.Columns["DonGia"].Visible = true;
                        //GrvdetailGrid.Columns["DonGiaDat"].Visible = false;

                    }
                    if (duyet == "Đã duyệt")
                    {
                        if (dieukienduyet == "")
                            dieukienduyet = @" where (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)<=0 and CountTong is not null)";
                        else
                            dieukienduyet += @" and (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)<=0 and CountTong is not null)";


                    }

                }
                if (NhaMaySelected != null)
                {
                    if (dieukien == "")
                        dieukien = " where NhaMay=@NhaMay";
                    else
                        dieukien += " and NhaMay=@NhaMay";
                    lstpara.Add(new ParameterDefine("@NhaMay", NhaMaySelected.Name));
                }
                if (!string.IsNullOrEmpty(nguoidenghi))
                {
                    if (dieukien == "")
                        dieukien = " where UserInsert=@NguoiDN";
                    else
                        dieukien += " and UserInsert=@NguoiDN";
                    lstpara.Add(new ParameterDefine("@NguoiDN", nguoidenghi));
                }

               
            }
            if (dieukien == "")
            {

                msgBox.Show("Vui lòng chọn ít nhất 1 điều kiện để tìm kiếm", IconMsg.iconerror);
                return;
            }
            //grvKeHoach.ItemsSource = null;

            string sql = string.Format(@" USE [NVLDB]   
                                declare @tblkyduyet as Table([Serial] int,[SerialLink] int,[TableName]  nvarchar(100)
                              ,[UserYeuCau]  nvarchar(100),[UserDuyet]  nvarchar(100),[LoaiDuyet]  nvarchar(100),[DaDuyet]  nvarchar(100)
                              ,[GhiChu]  nvarchar(500),countItemDuyet int)

                             declare @dtslmua as Table(MaNCC nvarchar(100),SLMua int)

                             declare @tbldonhang as Table(Serial int,[NguoiDN] nvarchar(100),[LyDo] nvarchar(100),[KhuVuc] nvarchar(100)
                                                         ,[NgayDatHang] datetime,[PhongBan] nvarchar(100),[NhaMay] nvarchar(100),[NoiDung] nvarchar(500)
                                                         ,[GhiChu] nvarchar(200),[UserInsert] nvarchar(100),[LoaiKeHoach] nvarchar(100),[NgayInsert] datetime
                                                         ,[NoiDungDeNghi] nvarchar(500),TenKhuVuc nvarchar(100),DVTT nvarchar(100),MaNCC nvarchar(100),TenNCC nvarchar(100))
                              insert into @tbldonhang(Serial,[NguoiDN] ,[LyDo],[KhuVuc],[NgayDatHang],[PhongBan],[NhaMay],[NoiDung],[GhiChu],[UserInsert]
							                                                        ,[NgayInsert],[NoiDungDeNghi],TenKhuVuc,DVTT,MaNCC,TenNCC)
                                                         SELECT [ddh].Serial,ddh.UserInsert ,'' as [LyDo],[KhuVuc],NgayDatHang,[PhongBan],'Nhà máy A' as NhaMay,NoiDungDeNghi,ddh.[GhiChu],ddh.[UserInsert]
							                                                        ,ddh.[NgayInsert],[NoiDungDeNghi],kv.TenKhuVuc,ddh.DVTT,ddh.MaNCC,ncc.TenNCC
                                                         from (Select * from NvlDonDatHang   
							                               {0}
							                            ) as ddh  left join dbo.NvlKhuVuc kv on ddh.KhuVuc=kv.MaKhuVuc
														left join dbo.NvlNhaCungCap ncc on ddh.MaNCC=ncc.MaNCC
                            declare @minserial int
                            declare @maxserial int
                            select @minserial=min(Serial),@maxserial=max(Serial) from @tbldonhang

                        insert into @dtslmua(MaNCC,SLMua)
						select MaNCC,count(*) as SLMua from NvlDonDatHang 
						 where Serial in (select SerialLinkMaster from NvlKyDuyetItem where TableName=N'NvlDonDatHang' and LoaiDuyet=N'Duyệt')
						group by MaNCC

                            if(@UserDuyet<>'')
                            begin
                             insert into @tblkyduyet([Serial],[SerialLink],[TableName],[UserYeuCau],[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],countItemDuyet)
                                                        select [Serial],[SerialLink],[TableName],[UserYeuCau],qryduyet.[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],isnull(countItemDuyet,0) as countItemDuyet
                                                        from (select * from NvlKyDuyet where UserDuyet=@UserDuyet and TableName='NvlDonDatHang' and SerialLink>=@minserial and SerialLink<=@maxserial) as qryduyet
														left join (SELECT [SerialLinkMaster],[UserDuyet],count(*) as countItemDuyet
   
													FROM [NvlKyDuyetItem] where UserDuyet=@UserDuyet and TableName='NvlDonDatHang' and SerialLinkMaster>=@minserial and SerialLinkMaster<=@maxserial
													group by [SerialLinkMaster],[UserDuyet]) as qryitem on qryduyet.SerialLink=qryitem.SerialLinkMaster and qryduyet.UserDuyet=qryitem.UserDuyet


                            end
                            else
                            begin
	                             insert into @tblkyduyet([Serial],[SerialLink],[TableName],[UserYeuCau],[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],countItemDuyet)
                                                        select [Serial],[SerialLink],[TableName],[UserYeuCau],qryduyet.[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],isnull(countItemDuyet,0) as countItemDuyet
                                                        from (select * from NvlKyDuyet where  TableName='NvlDonDatHang' and SerialLink>=@minserial and SerialLink<=@maxserial) as qryduyet
														left join (SELECT [SerialLinkMaster],[UserDuyet],count(*) as countItemDuyet
   
													FROM [NvlKyDuyetItem] where TableName='NvlDonDatHang' and SerialLinkMaster>=@minserial and SerialLinkMaster<=@maxserial
													group by [SerialLinkMaster],[UserDuyet]) as qryitem on qryduyet.SerialLink=qryitem.SerialLinkMaster and qryduyet.UserDuyet=qryitem.UserDuyet

                            end

                           
		                            select tbl.*,isnull(CountTong,0) as CountTong,isnull(qrykyduyetitem.SLItemDuyet,0) as CountDuyet,'{2}'+isnull(usr.PathImg,'UserImage/user.png') as PathImgTao,isnull(tblmua.SLMua,0) as CountDaMua from @tbldonhang tbl 
		                            left join (select SerialDN,SLDong as CountTong from
		                            (SELECT SerialMaDH as  SerialDN,count(*) as SLDong
		                              FROM [dbo].NvlDonDatHangItem khitem where SerialMaDH>=@minserial and SerialMaDH<=@maxserial 
		                              group by SerialMaDH) as qryitem) as qryallitem on tbl.Serial=qryallitem.SerialDN
		                              left join (SELECT SerialLinkMaster,count([SerialLinkItem]) as SLItemDuyet
		                            FROM [NvlKyDuyetItem] where TableName='NvlDonDatHang' and [LoaiDuyet]=N'Duyệt' and SerialLinkMaster>=@minserial and SerialLinkMaster<=@maxserial
		                            group by SerialLinkMaster) as qrykyduyetitem
		                            on tbl.Serial=qrykyduyetitem.SerialLinkMaster 
		                            inner join [DBMaster].[dbo].[Users] usr on tbl.UserInsert=usr.UsersName 	left join @dtslmua tblmua on tbl.MaNCC=tblmua.MaNCC
		                             {1}
		                            
                                    if(@UserDuyet<>'')
										select [Serial],[SerialLink],[TableName],[UserYeuCau],[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu] from NvlKyDuyet where TableName='NvlDonDatHang' and SerialLink in (select [SerialLink] from @tblkyduyet) 
								    else
										select * from @tblkyduyet

                            ", dieukien, dieukienduyet, ModelAdmin.pathurlfilepublic);

            // ShowProgress.ShowAwait();
            try
            {

                lstDonDatHangSearchShow.Clear();
                //lstDonDatHangSearchShow.TrimExcess();
                // lstDonDatHangSearchShow.Clear();
                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {

                    customRoot = JsonConvert.DeserializeObject<CustomRoot>(json);
                    if (customRoot != null)
                    {
                        lstDonDatHangSearchShow = customRoot.lstmuahang;
                        await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
                    }
                    StateHasChanged();
                }
                ////Xử lý load ảnh

                PanelVisible = false;
               

            }
            catch (Exception ex)
            {
                msgBox.Show("Lỗi:" + ex.Message, IconMsg.iconerror);
                PanelVisible = false;
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
                //ShowProgress.CloseAwait();
            }
        }

        private async Task searchAsync()
        {
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = "";
            string dieukienserial = "";
            string dieukienduyet = "";
            if (SerialDN != null)
            {
                dieukien = " where Serial=@SerialDN";
                lstpara.Add(new ParameterDefine("@SerialDN", SerialDN));
            }
            else
            {
                if (dtpbegin == null)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ngày để xem"));
                    return;
                }
                if (!LoaiDonHang.Contains("KyDuyet"))
                {
                    if (dieukien == "")
                        dieukien = string.Format(" where LoaiDonHang like N'%MuaHang%'", LoaiDonHang);
                    else
                        dieukien += string.Format(" and LoaiDonHang like N'%MuaHang%'", LoaiDonHang);

                }

                if (dtpbegin != null)
                {
                    if (dieukien == "")
                        dieukien = " where NgayInsert>=@DateBegin";
                    else
                        dieukien += " and NgayInsert>=@DateBegin";
                    lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));

                }

               

                if (dtpend != null)
                {
                    if (dieukien == "")
                        dieukien = " where NgayInsert<=@DateEnd";
                    else
                        dieukien += " and NgayInsert<=@DateEnd";
                    lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
                }

              
                //if (NhaMaySelected != null)
                //{
                //    if (dieukien == "")
                //        dieukien = " where NhaMay=@NhaMay";
                //    else
                //        dieukien += " and NhaMay=@NhaMay";
                //    lstpara.Add(new ParameterDefine("@NhaMay", NhaMaySelected.Name));
                //}
                if (!string.IsNullOrEmpty(nguoidenghi))
                {
                    if (dieukien == "")
                        dieukien = " where UserInsert=@NguoiDN";
                    else
                        dieukien += " and UserInsert=@NguoiDN";
                    lstpara.Add(new ParameterDefine("@NguoiDN", nguoidenghi));
                }


            }
            if (nguoiduyet != null)
            {

                lstpara.Add(new ParameterDefine("@UserDuyet", nguoiduyet.UsersName));

            }
            else
                lstpara.Add(new ParameterDefine("@UserDuyet", ""));
            if (TinhTrangSelected != null)
            {
                string duyet = TinhTrangSelected.Name.ToString();
                lstpara.Add(new ParameterDefine("@TinhTrang", duyet));


            }
            if (dieukien == "")
            {

                msgBox.Show("Vui lòng chọn ít nhất 1 điều kiện để tìm kiếm", IconMsg.iconerror);
                return;
            }
            //grvKeHoach.ItemsSource = null;

            string sql = string.Format(@" USE [NVLDB]   
                                declare @tblkyduyet as Table([Serial] int,[SerialLink] int,[TableName]  nvarchar(100)
                              ,[UserYeuCau]  nvarchar(100),[UserDuyet]  nvarchar(100),[LoaiDuyet]  nvarchar(100),[DaDuyet]  nvarchar(100)
                              ,[GhiChu]  nvarchar(500),countItemDuyet int,KhongDuyet nvarchar(100))

                             declare @dtslmua as Table(MaNCC nvarchar(100),SLMua int)

                             declare @tbldonhang as Table(Serial int,[NguoiDN] nvarchar(100),[LyDo] nvarchar(100),[KhuVuc] nvarchar(100)
                                                         ,[NgayDatHang] datetime,[PhongBan] nvarchar(100),[NhaMay] nvarchar(100),[NoiDung] nvarchar(500)
                                                         ,[GhiChu] nvarchar(200),[UserInsert] nvarchar(100),[LoaiKeHoach] nvarchar(100),[NgayInsert] datetime
                                                         ,[NoiDungDeNghi] nvarchar(500),TenKhuVuc nvarchar(100),DVTT nvarchar(100),MaNCC nvarchar(100),TenNCC nvarchar(100))
                              insert into @tbldonhang(Serial,[NguoiDN] ,[LyDo],[KhuVuc],[NgayDatHang],[PhongBan],[NhaMay],[NoiDung],[GhiChu],[UserInsert]
							                                                        ,[NgayInsert],[NoiDungDeNghi],TenKhuVuc,DVTT,MaNCC,TenNCC)
                                                         SELECT [ddh].Serial,usr.TenUser as NguoiDN,'' as [LyDo],ddh.[KhuVuc],NgayDatHang,[PhongBan],'Nhà máy A' as NhaMay,NoiDungDeNghi,ddh.[GhiChu],ddh.[UserInsert]
							                                                        ,ddh.[NgayInsert],[NoiDungDeNghi],kv.TenKhuVuc,ddh.DVTT,ddh.MaNCC,ncc.TenNCC
                                                         from (Select * from NvlDonDatHang   
							                               {0}
							                            ) as ddh  left join dbo.NvlKhuVuc kv on ddh.KhuVuc=kv.MaKhuVuc
														left join dbo.NvlNhaCungCap ncc on ddh.MaNCC=ncc.MaNCC
                                                        left join DBMaster.dbo.Users usr on ddh.UserInsert=usr.UsersName
                            declare @minserial int
                            declare @maxserial int
                            select @minserial=min(Serial),@maxserial=max(Serial) from @tbldonhang

                        	
                        	IF (@TinhTrang = N'Chưa duyệt')
	                        BEGIN
		                        IF (@UserDuyet <> '')
		                        BEGIN
			                        INSERT INTO @tblkyduyet (
				                        [Serial]
				                        ,[SerialLink]
				                        ,[TableName]
				                        ,[UserYeuCau]
				                        ,[UserDuyet]
				                        ,[LoaiDuyet]
				                        ,[DaDuyet]
				                        ,[GhiChu]
				                        ,countItemDuyet,KhongDuyet
				                        )
			                        SELECT [Serial]
				                        ,[SerialLink]
				                        ,[TableName]
				                        ,[UserYeuCau]
				                        ,qryduyet.[UserDuyet]
				                        ,[LoaiDuyet]
				                        ,[DaDuyet]
				                        ,[GhiChu]
				                        ,isnull(countItemDuyet, 0) AS countItemDuyet,KhongDuyet
			                        FROM (
				                        SELECT *
				                        FROM NvlKyDuyet
				                        WHERE UserDuyet = @UserDuyet
					                        AND TableName = 'NvlDonDatHang'
					                        AND SerialLink >= @minserial
					                        AND SerialLink <= @maxserial
				                        ) AS qryduyet
			                        LEFT JOIN (
				                        SELECT [SerialLinkMaster]
					                        ,[UserDuyet]
					                        ,count(*) AS countItemDuyet
				                        FROM [NvlKyDuyetItem]
				                        WHERE UserDuyet = @UserDuyet
					                        AND TableName = 'NvlDonDatHang'
					                        AND SerialLinkMaster >= @minserial
					                        AND SerialLinkMaster <= @maxserial
				                        GROUP BY [SerialLinkMaster]
					                        ,[UserDuyet]
				                        ) AS qryitem ON qryduyet.SerialLink = qryitem.SerialLinkMaster
				                        AND qryduyet.UserDuyet = qryitem.UserDuyet

			                        --Có User Duyệt (tức là hiển thị tất cả những đề nghị người này cần kiểm tra hoặc duyệt, bất chấp đề nghị đã được người khác duyệt
			                        SELECT tbl.*
				                        ,isnull(CountTong, 0) AS CountTong
				                        ,isnull(qrykyduyetitem.SLItemDuyet, 0) AS CountDuyet
				                        ,'{2}' + isnull(usr.PathImg, 'UserImage/user.png') AS PathImgTao
			                        FROM @tbldonhang tbl
			                        LEFT JOIN (
				                        SELECT SerialDN
					                        ,SLDong AS CountTong
				                        FROM (
					                        SELECT SerialMaDH as SerialDN
						                        ,count(*) AS SLDong
					                        FROM [dbo].NvlDonDatHangItem khitem
					                        WHERE SerialMaDH >= @minserial
						                        AND SerialMaDH <= @maxserial
					                        GROUP BY SerialMaDH
					                        ) AS qryitem
				                        ) AS qryallitem ON tbl.Serial = qryallitem.SerialDN
			                        LEFT JOIN (
				                        SELECT SerialLinkMaster
					                        ,count([SerialLinkItem]) AS SLItemDuyet
				                        FROM [NvlKyDuyetItem]
				                        WHERE TableName = 'NvlDonDatHang'
					                        AND [LoaiDuyet] = N'Duyệt'
					                        AND SerialLinkMaster >= @minserial
					                        AND SerialLinkMaster <= @maxserial
				                        GROUP BY SerialLinkMaster
				                        ) AS qrykyduyetitem ON tbl.Serial = qrykyduyetitem.SerialLinkMaster
			                        INNER JOIN [DBMaster].[dbo].[Users] usr ON tbl.UserInsert = usr.UsersName
			                        INNER JOIN (
				                        SELECT [SerialLink]
					                        ,max(countItemDuyet) AS countItemDuyet
				                        FROM @tblkyduyet
				                        GROUP BY SerialLink
				                        ) AS qrydakyduyet ON tbl.Serial = qrydakyduyet.SerialLink
				                        AND (
					                        isnull(CountTong, 0) - isnull(qrydakyduyet.countItemDuyet, 0) > 0
					                        OR CountTong IS NULL
					                        ) --Chỉ cần so sánh với những đề nghị mà người này được yêu cầu duyệt, nhưng chưa xác nhận

			                        SELECT [Serial]
				                        ,[SerialLink]
				                        ,[TableName]
				                        ,[UserYeuCau]
				                        ,[UserDuyet]
				                        ,[LoaiDuyet]
				                        ,[DaDuyet]
				                        ,[GhiChu]
			                        FROM NvlKyDuyet
			                        WHERE TableName = 'NvlDonDatHang'
				                        AND SerialLink IN (
					                        SELECT [SerialLink]
					                        FROM @tblkyduyet
					                        )
		                        END
		                        ELSE --Nếu User BẰng '' thì lấy đúng tình trạng của đề nghị theo tình trạnh đã duyệt hay chưa
		                        BEGIN
			                        INSERT INTO @tblkyduyet (
				                        [Serial]
				                        ,[SerialLink]
				                        ,[TableName]
				                        ,[UserYeuCau]
				                        ,[UserDuyet]
				                        ,[LoaiDuyet]
				                        ,[DaDuyet]
				                        ,[GhiChu]
				                        ,countItemDuyet,KhongDuyet
				                        )
			                        SELECT [Serial]
				                        ,[SerialLink]
				                        ,[TableName]
				                        ,[UserYeuCau]
				                        ,qryduyet.[UserDuyet]
				                        ,[LoaiDuyet]
				                        ,[DaDuyet]
				                        ,[GhiChu]
				                        ,isnull(countItemDuyet, 0) AS countItemDuyet,KhongDuyet
			                        FROM (
				                        SELECT *
				                        FROM NvlKyDuyet
				                        WHERE TableName = 'NvlDonDatHang'
					                        AND SerialLink >= @minserial
					                        AND SerialLink <= @maxserial
				                        ) AS qryduyet
			                        LEFT JOIN (
				                        SELECT [SerialLinkMaster]
					                        ,[UserDuyet]
					                        ,count(*) AS countItemDuyet
				                        FROM [NvlKyDuyetItem]
				                        WHERE TableName = 'NvlDonDatHang'
					                        AND SerialLinkMaster >= @minserial
					                        AND SerialLinkMaster <= @maxserial
				                        GROUP BY [SerialLinkMaster]
					                        ,[UserDuyet]
				                        ) AS qryitem ON qryduyet.SerialLink = qryitem.SerialLinkMaster
				                        AND qryduyet.UserDuyet = qryitem.UserDuyet

			                        SELECT tbl.*
				                        ,isnull(CountTong, 0) AS CountTong
				                        ,isnull(qrykyduyetitem.SLItemDuyet, 0) AS CountDuyet
				                        ,'{2}' + isnull(usr.PathImg, 'UserImage/user.png') AS PathImgTao
			                        FROM @tbldonhang tbl
			                        LEFT JOIN (
				                        SELECT SerialDN
					                        ,SLDong AS CountTong
				                        FROM (
					                       SELECT SerialMaDH as SerialDN
						                        ,count(*) AS SLDong
					                        FROM [dbo].NvlDonDatHangItem khitem
					                        WHERE SerialMaDH >= @minserial
						                        AND SerialMaDH <= @maxserial
					                        GROUP BY SerialMaDH
					                        ) AS qryitem
				                        ) AS qryallitem ON tbl.Serial = qryallitem.SerialDN
			                        LEFT JOIN (
				                        SELECT SerialLinkMaster
					                        ,count([SerialLinkItem]) AS SLItemDuyet
				                        FROM [NvlKyDuyetItem]
				                        WHERE TableName = 'NvlDonDatHang'
					                        AND [LoaiDuyet] = N'Duyệt'
					                        AND SerialLinkMaster >= @minserial
					                        AND SerialLinkMaster <= @maxserial
				                        GROUP BY SerialLinkMaster
				                        ) AS qrykyduyetitem ON tbl.Serial = qrykyduyetitem.SerialLinkMaster
			                        INNER JOIN [DBMaster].[dbo].[Users] usr ON tbl.UserInsert = usr.UsersName
			                        where (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)>0 or CountTong is null)
                                    order by tbl.Serial desc
			                        SELECT *
			                        FROM @tblkyduyet
		                        END
	                        END

	                        IF (@TinhTrang = N'Đã Duyệt')
	                        BEGIN
		                        IF (@UserDuyet <> '') --Nếu USer duyệt ='' nghĩa là lấy đúng những đề nghị được ký duyệt, còn User Duyệt<>'' nghĩa là đang xem theo cụ thể những đề nghị mà USer này đã xác nhận, chứ ko liên quan đến tình trạng duyệt chính thức
		                        BEGIN
			                        INSERT INTO @tblkyduyet (
				                        [Serial]
				                        ,[SerialLink]
				                        ,[TableName]
				                        ,[UserYeuCau]
				                        ,[UserDuyet]
				                        ,[LoaiDuyet]
				                        ,[DaDuyet]
				                        ,[GhiChu]
				                        ,countItemDuyet,KhongDuyet
				                        )
			                        SELECT [Serial]
				                        ,[SerialLink]
				                        ,[TableName]
				                        ,[UserYeuCau]
				                        ,qryduyet.[UserDuyet]
				                        ,[LoaiDuyet]
				                        ,[DaDuyet]
				                        ,[GhiChu]
				                        ,isnull(countItemDuyet, 0) AS countItemDuyet,KhongDuyet
			                        FROM (
				                        SELECT *
				                        FROM NvlKyDuyet
				                        WHERE UserDuyet = @UserDuyet
					                        AND TableName = 'NvlDonDatHang'
					                        AND SerialLink >= @minserial
					                        AND SerialLink <= @maxserial
				                        ) AS qryduyet
			                        LEFT JOIN (
				                        SELECT [SerialLinkMaster]
					                        ,[UserDuyet]
					                        ,count(*) AS countItemDuyet
				                        FROM [NvlKyDuyetItem]
				                        WHERE UserDuyet = @UserDuyet --Lấy danh sách những gì liên quan đến user này thôi
					                        AND TableName = 'NvlDonDatHang'
					                        AND SerialLinkMaster >= @minserial
					                        AND SerialLinkMaster <= @maxserial
				                        GROUP BY [SerialLinkMaster]
					                        ,[UserDuyet]
				                        ) AS qryitem ON qryduyet.SerialLink = qryitem.SerialLinkMaster
				                        AND qryduyet.UserDuyet = qryitem.UserDuyet

			                        --Có User Duyệt (tức là hiển thị tất cả những đề nghị người này cần kiểm tra hoặc duyệt, bất chấp đề nghị đã được người khác duyệt
			                        SELECT tbl.*
				                        ,isnull(CountTong, 0) AS CountTong
				                        ,isnull(qrykyduyetitem.SLItemDuyet, 0) AS CountDuyet
				                        ,'{2}' + isnull(usr.PathImg, 'UserImage/user.png') AS PathImgTao
			                        FROM @tbldonhang tbl
			                        LEFT JOIN (
				                        SELECT SerialDN
					                        ,SLDong AS CountTong
				                        FROM (
					                        SELECT SerialMaDH as SerialDN
						                        ,count(*) AS SLDong
					                        FROM [dbo].NvlDonDatHangItem khitem
					                        WHERE SerialMaDH >= @minserial
						                        AND SerialMaDH <= @maxserial
					                        GROUP BY SerialMaDH
					                        ) AS qryitem
				                        ) AS qryallitem ON tbl.Serial = qryallitem.SerialDN
			                        LEFT JOIN (
				                        SELECT SerialLinkMaster
					                        ,count([SerialLinkItem]) AS SLItemDuyet
				                        FROM [NvlKyDuyetItem]
				                        WHERE TableName = 'NvlDonDatHang'
					                        AND [LoaiDuyet] = N'Duyệt'
					                        AND SerialLinkMaster >= @minserial
					                        AND SerialLinkMaster <= @maxserial
				                        GROUP BY SerialLinkMaster
				                        ) AS qrykyduyetitem ON tbl.Serial = qrykyduyetitem.SerialLinkMaster
			                        INNER JOIN [DBMaster].[dbo].[Users] usr ON tbl.UserInsert = usr.UsersName
			                        INNER JOIN (
				                        SELECT [SerialLink]
					                        ,max(countItemDuyet) AS countItemDuyet
				                        FROM @tblkyduyet
				                        GROUP BY SerialLink
				                        ) AS qrydakyduyet ON tbl.Serial = qrydakyduyet.SerialLink
				                        AND (isnull(CountTong, 0) - isnull(qrydakyduyet.countItemDuyet, 0) <= 0) --Chỉ cần so sánh với những đề nghị mà người này được yêu cầu duyệt, nhưng chưa xác nhận
			                        SELECT [Serial]
				                        ,[SerialLink]
				                        ,[TableName]
				                        ,[UserYeuCau]
				                        ,[UserDuyet]
				                        ,[LoaiDuyet]
				                        ,[DaDuyet]
				                        ,[GhiChu]
			                        FROM NvlKyDuyet
			                        WHERE TableName = 'NvlDonDatHang'
				                        AND SerialLink IN (
					                        SELECT [SerialLink]
					                        FROM @tblkyduyet
					                        )
		                        END
		                        ELSE --Khong phân biệt theo User thì lấy đúng tình trạng của đề nghị isnull(CountTong, 0) - isnull(qrykyduyetitem.SLItemDuyet, 0) <= 0 AND CountTong IS NOT NULL
		                        BEGIN
			                        --Lấy tất cả danh sách theo USer
			                        INSERT INTO @tblkyduyet (
				                        [Serial]
				                        ,[SerialLink]
				                        ,[TableName]
				                        ,[UserYeuCau]
				                        ,[UserDuyet]
				                        ,[LoaiDuyet]
				                        ,[DaDuyet]
				                        ,[GhiChu]
				                        ,countItemDuyet,KhongDuyet
				                        )
			                        SELECT [Serial]
				                        ,[SerialLink]
				                        ,[TableName]
				                        ,[UserYeuCau]
				                        ,qryduyet.[UserDuyet]
				                        ,[LoaiDuyet]
				                        ,[DaDuyet]
				                        ,[GhiChu]
				                        ,isnull(countItemDuyet, 0) AS countItemDuyet,KhongDuyet
			                        FROM (
				                        SELECT *
				                        FROM NvlKyDuyet
				                        WHERE TableName = 'NvlDonDatHang'
					                        AND SerialLink >= @minserial
					                        AND SerialLink <= @maxserial
				                        ) AS qryduyet
			                        LEFT JOIN (
				                        SELECT [SerialLinkMaster]
					                        ,[UserDuyet]
					                        ,count(*) AS countItemDuyet
				                        FROM [NvlKyDuyetItem]
				                        WHERE TableName = 'NvlDonDatHang'
					                        AND SerialLinkMaster >= @minserial
					                        AND SerialLinkMaster <= @maxserial
				                        GROUP BY [SerialLinkMaster]
					                        ,[UserDuyet]
				                        ) AS qryitem ON qryduyet.SerialLink = qryitem.SerialLinkMaster
				                        AND qryduyet.UserDuyet = qryitem.UserDuyet

			                        SELECT tbl.*
				                        ,isnull(CountTong, 0) AS CountTong
				                        ,isnull(qrykyduyetitem.SLItemDuyet, 0) AS CountDuyet
				                        ,'{2}' + isnull(usr.PathImg, 'UserImage/user.png') AS PathImgTao
			                        FROM @tbldonhang tbl
			                        LEFT JOIN (
				                        SELECT SerialDN
					                        ,SLDong AS CountTong
				                        FROM (
					                       SELECT SerialMaDH as SerialDN
						                        ,count(*) AS SLDong
					                        FROM [dbo].NvlDonDatHangItem khitem
					                        WHERE SerialMaDH >= @minserial
						                        AND SerialMaDH <= @maxserial
					                        GROUP BY SerialMaDH
					                        ) AS qryitem
				                        ) AS qryallitem ON tbl.Serial = qryallitem.SerialDN
			                        LEFT JOIN (
				                        SELECT SerialLinkMaster
					                        ,count([SerialLinkItem]) AS SLItemDuyet
				                        FROM [NvlKyDuyetItem]
				                        WHERE TableName = 'NvlDonDatHang'
					                        AND [LoaiDuyet] = N'Duyệt'
					                        AND SerialLinkMaster >= @minserial
					                        AND SerialLinkMaster <= @maxserial
				                        GROUP BY SerialLinkMaster
				                        ) AS qrykyduyetitem ON tbl.Serial = qrykyduyetitem.SerialLinkMaster
			                        INNER JOIN [DBMaster].[dbo].[Users] usr ON tbl.UserInsert = usr.UsersName
			                         where (isnull(CountTong,0)-isnull(qrykyduyetitem.SLItemDuyet,0)<=0 and CountTong is not null)
                                    order by tbl.Serial desc
			                        SELECT *
			                        FROM @tblkyduyet
		                        END 
                            End

                            ", dieukien, dieukienduyet, ModelAdmin.pathurlfilepublic);

            // ShowProgress.ShowAwait();
            try
            {

                lstDonDatHangSearchShow.Clear();
                //lstDonDatHangSearchShow.TrimExcess();
                // lstDonDatHangSearchShow.Clear();
                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {

                    customRoot = JsonConvert.DeserializeObject<CustomRoot>(json);
                    
                    if (customRoot != null)
                    {
                        lstDonDatHangSearchShow = customRoot.lstmuahang;
                        await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
                    }
                    StateHasChanged();
                }
                ////Xử lý load ảnh

                PanelVisible = false;


            }
            catch (Exception ex)
            {
                msgBox.Show("Lỗi:" + ex.Message, IconMsg.iconerror);
                PanelVisible = false;
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
                //ShowProgress.CloseAwait();
            }
        }
        private async void search()
        {
            _ = searchAsync();
           
        }


        private async void load()
        {


        }
        bool IsOpenfly { get; set; } = false;
        string img { get; set; } = IconImg.CheckMark;
        private bool checkduyet()
        {

            return true;

        }


        public void KeHoachMasterAdd()
        {

            NVLDonDatHangShow nVLDonDatHangShow = new NVLDonDatHangShow();
            nVLDonDatHangShow.Serial = 0;
            nVLDonDatHangShow.NgayDatHang = DateTime.Now;
            nVLDonDatHangShow.DVTT = "VNĐ";
            if (LoaiDonHang == null || LoaiDonHang.Contains("KyDuyet"))
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi Loại đơn hàng"));
                return;
            }
            nVLDonDatHangShow.LoaiDonHang = LoaiDonHang;
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DonDatHang_AddMaster>(0);
                builder.AddAttribute(1, "nVLDonDatHangcrr", nVLDonDatHangShow);
                builder.AddAttribute(2, "CallBackAfterSave", EventCallback.Factory.Create<int>(this, SearchDeNghi));
                builder.CloseComponent();
            };

            dxPopup.showAsync("TẠO ĐƠN HÀNG");
            dxPopup.ShowAsync();
        }

        private async void SearchDeNghi(int Serial)
        {
            await dxPopup.CloseAsync();
            TinhTrangSelected = lsttinhtrang.Where(p => p.Name == "Chưa duyệt").FirstOrDefault();
            SerialDN = Serial;
            await searchAsync();

            SerialDN = null;
        }
        public async Task KeHoachMasterEditAsync()
        {

            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;


            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DonDatHang_AddMaster>(0);
                builder.AddAttribute(1, "nVLDonDatHangcrr", nVLDonDatHangShowcrr.CopyClass());

                builder.AddAttribute(2, "AfterEdit", EventCallback.Factory.Create<NVLDonDatHangShow>(this, RefreshItemUpdateAsync));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            dxPopup.showAsync("SỬA ĐỀ NGHỊ");

            await dxPopup.ShowAsync();
            ;
        }
        public async Task KeHoachMasterDeleteAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            bool bl = await dialogMsg.Show("Xóa đề nghị", $"Bạn có chắc muốn xóa đơn hàng số {nVLDonDatHangShowcrr.Serial.ToString()}");

            if (!phanQuyenAccess.CheckDelete(nVLDonDatHangShowcrr.UserInsert, ModelAdmin.users))
            {
                msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;

            }
            if (bl)
            {
                if (!checkduyet())
                {
                    return;
                }

                try
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlDonDatHang_Delete";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    lstpara.Add(new ParameterDefine("@Serial", nVLDonDatHangShowcrr.Serial.ToString()));
                    lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Success, "Xóa thành công"));
                            // msgBox.Show("Xóa thành công", IconMsg.iconssuccess);
                            lstDonDatHangSearchShow.Remove(nVLDonDatHangShowcrr);
                            Grid.Reload();
                        }
                        else
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Warning, $"{query[0].ketqua}, {query[0].ketquaexception}"));
                            //msgBox.Show("Lỗi: " + query[0].ketqua, IconMsg.iconssuccess);

                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
                }

            }
        }

        private bool ShowTinhTrang()
        {
            if (TinhTrangSelected == null)
                return false;
            if (TinhTrangSelected.Name == "Chưa duyệt")
            {
                return false;
            }
            if (TinhTrangSelected.Name == "Đã duyệt")
            {
                return true;
            }
            return false;
        }
        async void CheckedChanged(bool value)
        {
            if (value)
            {
                TinhTrangSelected = lsttinhtrang.Where(p => p.Name == "Đã duyệt").FirstOrDefault();
            }
            else
            {
                TinhTrangSelected = lsttinhtrang.Where(p => p.Name == "Chưa duyệt").FirstOrDefault();
            }
            await searchAsync();
        }
        public async Task KeHoachMasterAddItemAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(nVLDonDatHangShowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không phải người tạo, nên không có quyền thêm"));
                return;
            }
            //NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow = new NvlKeHoachMuaHangItemShow();
            //nvlKeHoachMuaHangItemShow.SerialDN = kehoachshowcrr.Serial;
            //nvlKeHoachMuaHangItemShow.Serial = 0;


            MenuItem menuItem = new MenuItem();
            menuItem.TextItem = "Thêm mã hàng";
            menuItem.NameItem = "createtaodonhang";
            menuItem.ComponentName = "NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Page_KeHoachChuaDatHang";
            menuItem.IconCssClass = "bi bi-cart3";
            if (ModelAdmin.mainLayout != null)
            {
                RenderFragment renderFragment1;
                renderFragment1 = builder =>
                {

                    builder.OpenComponent<Page_KeHoachChuaDatHang>(0);
                    builder.AddAttribute(1, "nVLDonDatHangShowcrr", nVLDonDatHangShowcrr.CopyClass());

                    builder.CloseComponent();
                };
                ModelAdmin.mainLayout.AddDirectRenderfagment(menuItem, renderFragment1);

            }

        }



        public async Task KeHoachMasterChonNguoiDuyettAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(nVLDonDatHangShowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Bạn không có quyền duyệt dòng này do bạn không phải người tạo hoặc chưa được chọn duyệt"));
                // msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;
            }


            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;

            string KyDuyet = "KyDuyetDonHang";
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_ChonNguoiDuyet>(0);
                builder.AddAttribute(1, "nVLDonDatHangShowcrr", nVLDonDatHangShowcrr);
                builder.AddAttribute(2, "KyDuyet", KyDuyet);
                builder.AddAttribute(3, "GotoMainFormDH", EventCallback.Factory.Create<NVLDonDatHangShow>(this, RefreshListDuyet));

                builder.CloseComponent();
            };
            dxPopup.showAsync("CHỌN NGƯỜI DUYỆT");

            dxPopup.ShowAsync();


        }

        private async void RefreshListDuyet()
        {
            await dxPopup.CloseAsync();
            await searchAsync();
            Grid.Reload();
        }
        private async void RefreshDuyetItem()
        {
            await Grid.SaveChangesAsync();
        }
        private async Task RefreshItemUpdateAsync(NVLDonDatHangShow nvlDonDatHang_get)
        {
            await dxPopup.CloseAsync();
            nVLDonDatHangShowcrr.Serial = nvlDonDatHang_get.Serial;
            nVLDonDatHangShowcrr.NgayTao = nvlDonDatHang_get.NgayTao;
            nVLDonDatHangShowcrr.KhuVuc = nvlDonDatHang_get.KhuVuc;
            nVLDonDatHangShowcrr.MaNCC = nvlDonDatHang_get.MaNCC;
            nVLDonDatHangShowcrr.PhongBan = nvlDonDatHang_get.PhongBan;

            nVLDonDatHangShowcrr.NgayDatHang = nvlDonDatHang_get.NgayDatHang;
            nVLDonDatHangShowcrr.NgayMax = nvlDonDatHang_get.NgayMax;
            nVLDonDatHangShowcrr.GhiChu = nvlDonDatHang_get.GhiChu;
            nVLDonDatHangShowcrr.UserInsert = nvlDonDatHang_get.UserInsert;
            Grid.SaveChangesAsync();

        }
        public async Task ShowTruocInAsync()
        {

            //await dxFlyoutchucnang.CloseAsync();
            //string noidung = "";
            //if (String.IsNullOrEmpty(nVLDonDatHangShowcrr.NoiDungDeNghi))
            //{

            //    noidung = string.Format("{0} đề nghị mua vật tư phục vụ sản xuất như sau:", nVLDonDatHangShowcrr.PhongBan);
            //}
            //else
            //    noidung = nVLDonDatHangShowcrr.NoiDungDeNghi;

            //IsOpenfly = false;
            //renderFragment = builder =>
            //{
            //    builder.OpenComponent<Urc_KeHoachChinhSuaBanIn>(0);
            //    builder.AddAttribute(1, "GotoMainForm", EventCallback.Factory.Create<KeHoachMuaHang_Show>(this, RefreshListDuyet));
            //    builder.AddAttribute(2, "NoiDungDeNghi", "noidung");
            //    builder.AddAttribute(3, "keHoachMuaHang_Show", "kehoachshowcrr");

            //    builder.CloseComponent();
            //};

            //dxPopup.showAsync("CHỈNH SỬA BẢN IN");
            //dxPopup.ShowAsync();

            //renderFragment = builder =>
            //{
            //    builder.OpenComponent<Urc_KeHoachChinhSuaBanIn>(0);
            //    builder.AddAttribute(1, "keHoachMuaHang_Show", kehoachshowcrr);
            //    builder.AddAttribute(2, "NoiDungDeNghi", noidung);
            //    //builder.AddAttribute(3, "GotoMainForm", EventCallback.Factory.Create<KeHoachMuaHang_Show>(this, RefreshListDuyet));
            //    //builder.OpenComponent(0, componentType);
            //    builder.CloseComponent();
            //};

            //var parameters = new ModalParameters();
            //parameters.Add("keHoachMuaHang_Show", kehoachshowcrr);
            //parameters.Add("NoiDungDeNghi", noidung);
            //modal.Show<Urc_KeHoachChinhSuaBanIn>("", parameters, options);


        }
        CustomRootReport customRootrp { get; set; } = new CustomRootReport();
        public class NvlPrintMaster
        {
            public int Serial { get; set; }

            public int? SerialLink { get; set; }

            public string TableName { get; set; }

            public string UserYeuCau { get; set; }
            public string PathImg { get; set; }
            public string UserDuyet { get; set; }
            public string TenUserDuyet { get; set; }
            public string LoaiDuyet { get; set; }
            public string DVTT { get; set; }
            public DateTime? NgayApDung { get; set; }

            public DateTime? NgayKyDuyet { get; set; }

            public DateTime? NgayInsert { get; set; }

            public string DaDuyet { get; set; }
            public int countItemDuyet { get; set; }
            public string GhiChu { get; set; }
            public string TenNCC { get; set; }



        }
        public class CustomRootReport
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NVLDonDatHangItemShow> lstitemrp { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetShow> lstkyduyetrp { get; set; }
        }
        private async Task PrintAsync()
        {
            IsOpenfly = false;
            await dxFlyoutchucnang.CloseAsync();
            PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
            await Task.Delay(50);
            try
            {
                //await dxFlyoutchucnang.CloseAsync();


                string sql = "";

                //SqlConnection sqlConnection = prs.ConnectNVL();
                //sqlConnection.Open();

                string sqlghichu = "";


                sqlghichu = string.Format(@"
                            ", nVLDonDatHangShowcrr.Serial);
                sql = string.Format(@"
                                --Điều kiện ghi chú
                                use NVLDB
                                {1} 
                                 declare @NgayDatHang date
                declare @SerialDH int={0}
				 select @NgayDatHang=NgayDatHang from NvlDonDatHang where Serial=@SerialDH
                     Select donhangchitiet.Serial,MaHang,TenHang,SLDatHang,NoiDung,DVT,DonGia,case when MaHang is null then isnull([DonGia],0) else isnull([SLDatHang]*DonGia,0) end as ThanhTien,NgayDKNhapKho,GroupIndex,dbo.DonDatHangGroup([Group]) as [GroupName],PhanLoai,QuyCachChatLuong,TenNCC
                        ,donhangchitiet.MaNCC,isnull(qryncchd.SoHD,'') as SoHD,qryncchd.NgayHieuLuc,donhangchitiet.STT,	donhangchitiet.SerialMaDH
                       FROM
                       (SELECT dhitem.[Serial], dhitem.[SerialMaDH], dhitem.[MaHang], dhitem.[SLDatHang], dhitem.SignInt, dhitem.NoiDung,
                          dhitem.[SLTheoDoi], NvlHangHoa.[DVT],dhitem.[DonGia],dhitem.[SerialLink], 
                          dhitem.[MaNCC],dhitem.[NgayDKNhapKho],dhitem.[UserInsert],dhitem.[NgayInsert],
                          dhitem.[Group], dhitem.GroupIndex,dhitem.GiaTri,NvlHangHoa.TenHang, NvlNhaCungCap.TenNCC, ISNULL(dhitem.DonGia,0) * IsNull(dhitem.SLDatHang,0) as ThanhTien ,nh.PhanLoai
                           ,case when NvlHangHoa.QuyCach='' then NvlHangHoa.ChatLuong 
						            else (NvlHangHoa.QuyCach+case when NvlHangHoa.ChatLuong<>'' then + CHAR(13) + CHAR(10) + NvlHangHoa.ChatLuong else '' end) end as QuyCachChatLuong,dhitem.STT
                           from (SELECT [Serial], [SerialMaDH], [MaHang], [SLDatHang], 1 as SignInt, NULL as NoiDung,
                          [SLTheoDoi], [DVT],[DonGia],[SerialLink], 
                          [MaNCC],[NgayDKNhapKho],[UserInsert],[NgayInsert],
                          N'NvlDonDatHangItem' as [Group], 0 as [GroupIndex],GiaTri,isnull(STT,0) as STT
                   FROM [NvlDonDatHangItem]
                   Where SerialMaDH = @SerialDH ) dhitem INNER JOIN 
                   NvlHangHoa ON NvlHangHoa.MaHang = dhitem.MaHang
                   inner join dbo.NvlNhomHang nh on NvlHangHoa.MaNhom=nh.MaNhom
                   left JOIN NvlNhaCungCap ON NvlNhaCungCap.MaNCC = dhitem.MaNCC
      
               ) as donhangchitiet left join (SELECT [MaNCC],[SoHD],[NgayHieuLuc]
			             FROM [NvlNhaCungCapHD]
			            where Serial in (select max(Serial) from NvlNhaCungCapHD where NgayHieuLuc<=@NgayDatHang group by MaNCC))  as qryncchd
			            on donhangchitiet.MaNCC=qryncchd.MaNCC order  by STT
    
              select qry.*,usr.TenUser as UserYeuCau,isnull(NoiDungDeNghi,'') as NoiDungDeNghi,isnull(qry.GhiChu,'') as GhiChu
            ,ncc.MaNCC,ncc.TenNCC,ncc.DiaChi, isnull(ncc.DiDong,ncc.DienThoaiBan) as SoDienThoai,ncc.MaSoThue,ncc.Email,qryrp.*
            from 
            (SELECT  [Serial],[MaDatHang],[NgayDatHang],isnull(DVTT,N'VNĐ') as DVTT
            ,[NgayMax],[KhuVuc],[GhiChu],[UserInsert],isnull(NoiDungDeNghi,'') as NoiDungDeNghi,PhongBan,MaNCC
             FROM [NvlDonDatHang] where Serial=@SerialDH
             ) as qry 
             left join DBMaster.dbo.Users usr on qry.UserInsert=usr.UsersName
             left join dbo.NvlNhaCungCap ncc on qry.MaNCC=ncc.MaNCC
            left join dbo.GetReportDetail_DonDatHang(@SerialDH)  as qryrp on qry.Serial=qryrp.SerialLink
                                ", nVLDonDatHangShowcrr.Serial, sqlghichu);
                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();

                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {

                    DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);
                    if (dataSet == null)
                    {
                        return;
                    }
                    if (dataSet.Tables[0].Rows.Count == 0)
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                        return;
                    }
                    DataTable dtitem = dataSet.Tables[0];
                    DataTable dtmaster = dataSet.Tables[1];
                    var querysum = dtitem.AsEnumerable().Sum(p => p.Field<double>("ThanhTien"));

                    string bangchu = prs.docsothapphan(querysum);
                    if (dtmaster.Rows[0]["DVTT"].ToString() == "VNĐ")
                        bangchu = bangchu + " đồng";
                    else
                        bangchu = bangchu + dtmaster.Rows[0]["DVTT"].ToString();
                    XtraRp_DonDatHangNew xtra_KTGTonKho = new XtraRp_DonDatHangNew();

                    XRSubreport xrqtitem = xtra_KTGTonKho.FindControl("xrSubreport1", true) as XRSubreport;
                    XtraRp_DonDatHangItem xtraRp_DonDatHangItem = (XtraRp_DonDatHangItem)xrqtitem.ReportSource;
                    //xtraRp_DonDatHangItem.setGhiChu(dtmaster.Rows[0]["GhiChu"].ToString(), "");
                    xtra_KTGTonKho.setGhiChu(dtmaster.Rows[0]["GhiChu"].ToString(), "");
                    xtraRp_DonDatHangItem.setBangChu(bangchu, querysum);
                    xtraRp_DonDatHangItem.DataSource = dtitem;
                    //xtra_KTGTonKho.setNguoiDuyet("Phòng vật tư", nguoilap, nguoikiemtra, nguoiduyet, nvlDonDatHangShowcrr.DaDuyet);
                    xtra_KTGTonKho.DataSource = dtmaster;

                    //var querysum =customRootrp.lstitemrp.Sum(p => p.ThanhTien);

                    //    string bangchu = prs.docsothapphan(querysum.Value);
                    //    if (customRootrp.lstkyduyetrp[0].DVTT == "VNĐ")
                    //        bangchu = bangchu + " đồng";
                    //    else
                    //        bangchu = bangchu + customRootrp.lstkyduyetrp[0].DVTT;

                    string madenghi = string.Format("{0}/ĐĐH/{1}", prs.SerialLengthToString(nVLDonDatHangShowcrr.Serial, 4), nVLDonDatHangShowcrr.NgayDatHang.Value.ToString("yyyy"));
                    xtra_KTGTonKho.setMaDeNghi(madenghi);
                    Nullable<DateTime> ngayhieuluc;
                    ngayhieuluc = null;
                    xtra_KTGTonKho.setNoiDung(ngayhieuluc, "", dtmaster.Rows[0]["NoiDungDeNghi"].ToString(), "");
                    ModelAdmin.mainLayout.showreportAsync(xtra_KTGTonKho);

                    dataSet.Dispose();

                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi :" + ex.Message));
                Console.WriteLine("Lỗi:" + ex.Message);
            }
            finally
            {
                PreloadService.Hide();
            }
        }


        public string TextSoDeNghi(NVLDonDatHangShow keHoachMuaHang_Show)
        {
            return "Đơn hàng số " + keHoachMuaHang_Show.Serial.ToString(); ;

        }
        public string TextNgayDeNghi(NVLDonDatHangShow keHoachMuaHang_Show)
        {
            return "Ngày tạo " + keHoachMuaHang_Show.NgayDatHang.Value.ToString("dd/MM/yy");
        }
        public async void ShowFlyout(NVLDonDatHangShow keHoachMuaHang_Show)
        {
            await dxFlyoutchucnang.CloseAsync();
            CheckQuyen = phanQuyenAccess.CheckDelete(keHoachMuaHang_Show.UserInsert, ModelAdmin.users);

            nVLDonDatHangShowcrr = keHoachMuaHang_Show;
            idflychucnang = "#" + idelement(keHoachMuaHang_Show.Serial);
            Visileprint = true;

            if (phanQuyenAccess.CheckDelete(keHoachMuaHang_Show.UserInsert,ModelAdmin.users))
            {
                Visilechinhsua = true;
                //Visilechinhtruocin = true;

                Visilechonnguoiduyet = true;
                Visiledelete = true;

                Visilethemchitiet = true;

            }
            else
            {

                Visilechinhsua = false;
                Visilechinhtruocin = false;
                Visilechinhtruocin = false;
                Visilechonnguoiduyet = false;
                Visiledelete = false;
                Visilethemchitiet = false;

            }
            //IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();

        }
        private void DonHangAddFromKH()
        {
            if (!CheckQuyen)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không có quyền thao tác ở chức năng này"));
                return;
            }
            if (LoaiDonHang == "DonHangKeHoach")
            {
                renderFragment = builder =>
                {
                    builder.OpenComponent<Page_KeHoachChuaDatHang>(0);
                    builder.AddAttribute(1, "GotoMainFormKeHoach", EventCallback.Factory.Create<KeHoachMuaHang_Show>(this, getItemFromKeHoach));
                    builder.AddAttribute(2, "LoaiKeHoach", "KeHoachMuaHang");
                    builder.CloseComponent();
                };

                dxPopup.showAsync("THÊM KẾ HOẠCH THÁNG");
                dxPopup.ShowAsync();
            }
            if (LoaiDonHang == "DonHangSanXuat")
            {
                renderFragment = builder =>
                {
                    builder.OpenComponent<Page_KeHoachChuaDatHang>(0);
                    builder.AddAttribute(1, "GotoMainFormKeHoach", EventCallback.Factory.Create<KeHoachMuaHang_Show>(this, getItemFromKeHoach));
                    builder.AddAttribute(2, "LoaiKeHoach", "KeHoachSanXuat");
                    builder.CloseComponent();
                };

                dxPopup.showAsync("THÊM KẾ HOẠCH THÁNG");
                dxPopup.ShowAsync();
            }
        }
        private void getItemFromKeHoach(KeHoachMuaHang_Show keHoachMuaHang_Show)
        {
            dxPopup.CloseAsync();
            if (nVLDonDatHangShowcrr == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Chưa có đơn hàng nào được chọn"));
                return;
            }
            MenuItem menuItem = new MenuItem();
            menuItem.TextItem = "Tạo đơn hàng";
            menuItem.NameItem = "createtaodonhang";
            menuItem.ComponentName = "NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Urc_DonHang_AddKeHoach";
            menuItem.IconCssClass = "bi bi-cart3";
            if (ModelAdmin.mainLayout != null)
            {
                RenderFragment renderFragment1;
                renderFragment1 = builder =>
                {
                    builder.OpenComponent<Urc_DonHang_AddKeHoach>(0);
                    builder.AddAttribute(1, "keHoachMuaHang_Showcrr", keHoachMuaHang_Show);
                    builder.AddAttribute(2, "nVLDonDatHangShowcrr", nVLDonDatHangShowcrr);
                    builder.CloseComponent();
                };
                ModelAdmin.mainLayout.AddDirectRenderfagment(menuItem, renderFragment1);

            }


        }
        private async Task ShowItemReportAsync()
        {
            dxPopup.CloseAsync();
            await dxFlyoutchucnang.CloseAsync();
            if (nVLDonDatHangShowcrr == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Chưa có đơn hàng nào được chọn"));
                return;
            }
            NvlReportDetailShow nvlReportDetailShow = new NvlReportDetailShow();

            nvlReportDetailShow.NameReport = "XtraRp_DonDatHangNew";
            nvlReportDetailShow.SerialLink = nVLDonDatHangShowcrr.Serial;
            nvlReportDetailShow.TableName = "NvlDonDatHang";
            string sql = "";
            try
            {
                sql = string.Format(@"Use NVLDB
                        declare @SerialLink int={0}
                        declare @TableName nvarchar(100)='{1}'
	                    declare @NameReport nvarchar(100)='{2}'
                       
                       declare @MaNCC nvarchar(100)=N'{3}'

                        declare @CheckCount int
	                    select top 1 @CheckCount=count(*) from [dbo].[NvlReportDetail]  where SerialLink=@SerialLink and TableName=@TableName and NameReport=@NameReport
	                    if(@CheckCount=0)
	                    begin
		                    --Lấy dữ liệu của dòng gần nhất
		                    declare @serialrecent int
		                    SELECT TOP 1 @serialrecent=[Serial] FROM [dbo].[NvlDonDatHang]
		                    where MaNCC =@MaNCC  and Serial in (SELECT  [SerialLink]
		                    FROM [dbo].[NvlReportDetail] where TableName=@TableName and NameReport=@NameReport)
		                    order by Serial desc
		                    if(@serialrecent is null)
			                    set @SerialLink=0
		                    else
			                    set @SerialLink=@serialrecent
	                    end
	
	                    SELECT  *
                      FROM [dbo].[NvlReportDetail] where SerialLink=@SerialLink and TableName=@TableName and NameReport=@NameReport
                     order by STT
                    ", nVLDonDatHangShowcrr.Serial, nvlReportDetailShow.TableName, nvlReportDetailShow.NameReport, nVLDonDatHangShowcrr.MaNCC);
                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NvlReportDetailShow>>(json);
                    renderFragment = builder =>
                    {
                        builder.OpenComponent<App_ThongTin.Urc_NvlReportDetail>(0);

                        builder.AddAttribute(1, "nvlReportDetailShowcrr", nvlReportDetailShow);
                        builder.AddAttribute(2, "lstdata", query);
                        builder.CloseComponent();
                    };

                    dxPopup.showAsync("THÊM THÔNG TIN TRONG REPORT");
                    dxPopup.ShowAsync();

                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message}"));
                Console.Error.WriteLine(ex.ToString());
            }
            finally
            {

                //Grid.Reload();
                StateHasChanged();
            }





        }
        private void ShowNhaCungCapView(string MaNCC)
        {
            if (MaNCC != null)
            {
                renderFragment = builder =>
                {
                    builder.OpenComponent<App_ThongTin.Urc_NhaCungCapView>(0);

                    builder.AddAttribute(1, "MaNCC", MaNCC);

                    builder.CloseComponent();
                };

                dxPopup.showAsync("THÔNG TIN NHÀ CUNG CẤP");
                dxPopup.ShowAsync();
            }
           
        }

        private async Task DuyetItemAllAsync(NVLDonDatHangShow nVLDonDatHangShow)
        {
            if (nVLDonDatHangShow.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in nVLDonDatHangShow.lstkyduyet)
            {
                if (ModelAdmin.users.UsersName == it.UserDuyet)
                {
                    checkduyet = true;
                    LoaiDuyet = it.LoaiDuyet;
                    break;
                }
            }
            if (!checkduyet)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có trong danh sách được duyệt!"));
                return;
            }

            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlKyDuyetItem_InsertAll";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialLinkMaster", nVLDonDatHangShow.Serial));
                lstpara.Add(new ParameterDefine("@SerialLinkItem", 0));
                lstpara.Add(new ParameterDefine("@TableName", "NvlDonDatHang"));
                lstpara.Add(new ParameterDefine("@UserDuyet", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@GhiChu", ""));


                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        ToastService.Notify(new(ToastType.Success, $"Duyệt thành công"));
                        nVLDonDatHangShow.EnableButtonDuyet = false;
                        nVLDonDatHangShow.KhongDuyet = "";
                        if (LoaiDuyet == "Duyệt")
                        {
                            nVLDonDatHangShow.CountDuyet = nVLDonDatHangShow.CountTong;
                            if (nVLDonDatHangShow.lstitem != null)
                            {
                                foreach (var it in nVLDonDatHangShow.lstitem)
                                {
                                    it.TextDuyet = ModelAdmin.users.TenUser;
                                }
                            }
                            // nvlKeHoachMuaHangItemShow.TextDuyet = ModelAdmin.users.TenUser;
                        }
                        else
                        {
                            if (nVLDonDatHangShow.lstitem != null)
                            {
                                foreach (var it in nVLDonDatHangShow.lstitem)
                                {
                                    it.TextKiem = ModelAdmin.users.TenUser;
                                }
                            }
                        }
                        //nvlKeHoachMuaHangItemShow.TextKiem = ModelAdmin.users.TenUser;
                        await Grid.SaveChangesAsync();
                       
                    }
                    else
                    {
                        ToastService.Notify(new(ToastType.Danger, $"Lỗi: {query[0].ketquaexception} Duyệt không được"));

                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message} Duyệt không được"));
            }

        }

        private bool Visibleduyetall(NVLDonDatHangShow keHoachMuaHang_Show)
        {

            if (keHoachMuaHang_Show.lstkyduyet != null)
            {
                var query = keHoachMuaHang_Show.lstkyduyet.Where(p => p.UserDuyet == ModelAdmin.users.UsersName).FirstOrDefault();
                if (query != null)
                {
                    keHoachMuaHang_Show.ShowTextDuyet = query.LoaiDuyet;
                    return true;
                }

                return false;
                //foreach (var it in keHoachMuaHang_Show.lstkyduyet)
                //{
                //    if (ModelAdmin.users.UsersName == it.UserDuyet)
                //    {
                //        return true;
                //    }
                //}
            }
            return false;
        }
        private async Task KhongDuyetAllAsync(NVLDonDatHangShow keHoachMuaHang_Show)
        {
            if (keHoachMuaHang_Show.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in keHoachMuaHang_Show.lstkyduyet)
            {
                if (ModelAdmin.users.UsersName == it.UserDuyet)
                {
                    checkduyet = true;
                    LoaiDuyet = it.LoaiDuyet;
                    break;
                }
            }
            if (!checkduyet)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có trong danh sách được duyệt!"));
                return;
            }
            nVLDonDatHangShowcrr = keHoachMuaHang_Show;
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NhapLyDo>(0);
                builder.AddAttribute(1, "GetLyDo", EventCallback.Factory.Create<string>(this, lydoxoaCallBack));
                builder.CloseComponent();
            };

            await dxPopup.showAsync("Không duyệt vì lý do gì?");
            await dxPopup.ShowAsync();


        }
        private async void lydoxoaCallBack(string lydo)
        {
            string lydoxoa = lydo;

            if (!string.IsNullOrEmpty(lydoxoa))
            {
                dxPopup.CloseAsync();
                try
                {

                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlKyDuyet_KhongDuyetAll";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    lstpara.Add(new ParameterDefine("@SerialLink", nVLDonDatHangShowcrr.Serial));

                    lstpara.Add(new ParameterDefine("@TableName", "NvlDonDatHang"));
                    lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                    lstpara.Add(new ParameterDefine("@GhiChu", lydoxoa));


                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            //ToastService.Notify(new(ToastType.Success, $"Duyệt thành công"));
                            ToastService.Notify(new ToastMessage(ToastType.Success, "Đã Xác nhận không duyệt"));
                            foreach (var it in nVLDonDatHangShowcrr.lstkyduyet)
                            {
                                it.KhongDuyet = ModelAdmin.users.UsersName;
                            }
                            nVLDonDatHangShowcrr.KhongDuyet = lydoxoa;
                            //kehoachshowcrr.lstkyduyet.ForEach(p => p.KhongDuyet = ModelAdmin.users.UsersName);
                            StateHasChanged();
                            // kehoachshowcrr.EnableButtonDuyet = false;
                            //nvlKeHoachMuaHangItemShow.TextKiem = ModelAdmin.users.TenUser;



                        }
                        else
                        {
                            ToastService.Notify(new(ToastType.Danger, $"Lỗi: {query[0].ketqua},{query[0].ketquaexception} .Duyệt không được"));

                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message}"));
                }
            }
        }
    }
}

