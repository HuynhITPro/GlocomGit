using BlazorBootstrap;


using DevExpress.Blazor;

using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Components;

using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;

using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi;

using NFCWebBlazor.Model;
using NFCWebBlazor.Models;

using System.Collections.ObjectModel;
using System.ComponentModel;

using System.Data;
using System.Reflection;
using static NFCWebBlazor.App_ThongTin.Urc_NvlReportDetail;
namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public partial class Page_DuyetGiaMaster
    {
        [Inject] PreloadService PreloadService { get; set; }
        [Inject] HttpClient httpClient { get; set; }
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        //[CascadingParameter] IModalService Modal { get; set; } = default!;
        DateTime? dtpbegin { get; set; } = DateTime.Now.AddMonths(-2);
        DateTime? dtpend { get; set; } = DateTime.Now;


        IGrid Grid { get; set; }
        List<NvlDuyetGiaShow> lstdata { get; set; } = new List<NvlDuyetGiaShow>();
        NvlDuyetGiaShow nvlDuyetGiaShowcrr { get; set; }
        List<Users> lstuser { get; set; }
        List<DataDropDownList> lstnhamay = new List<DataDropDownList>();
        List<DataDropDownList> lsttinhtrang = new List<DataDropDownList>();
        List<Users> lstnguoidenghi { get; set; }
        List<Users> lstnguoiduyet { get; set; }
        CustomRoot customRoot { get; set; } = new CustomRoot();
        DataDropDownList? NhaMaySelected { get; set; }
        DataDropDownList? TinhTrangSelected { get; set; }
        Users? nguoiduyet { get; set; }
        Users? nguoidenghi { get; set; }
        public string serialDN { get; set; } = "";
        public string LoaiDonHang { get; set; }


        public partial class NvlDuyetGiaShow : INotifyPropertyChanged
        {
            public int Serial { get; set; }

            public int? STT { get; set; }

            public string MaDN { get; set; }
            public string DVT { get; set; } = "VNĐ";

            public string NguoiDN { get; set; }
            public string GhiChu { get; set; }
            public string LyDo { get; set; } = "Mua mới";

            public string PhongBan { get; set; }
            public DateTime? Ngay { get; set; }
            public string NhaMay { get; set; }

            public string DuAnHanMuc { get; set; }

            public string? CuocVanChuyen { get; set; }

            public string? ThueGTGT { get; set; }

            public string KetQua { get; set; }

            public string ChonNhaCungCap { get; set; }

            public string UserInsert { get; set; }
            public string LoaiDuyetGia { get; set; }
            public DateTime? NgayInsert { get; set; }

            public string PathImgTao { get; set; } = Model.ModelAdmin.pathurlfilepublic + "/UserImage/user.png";

            public ObservableCollection<NvlDuyetGiaItemShow> lstitem { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public NvlDuyetGiaShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlDuyetGiaShow>(json);
            }

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
            public DateTime? NgayApDung { get; set; }
            public DateTime? NgayKetThuc { get; set; }
            public List<HeaderText> lstheaderbinding { get; set; }
            public bool checkUpdate { get; set; }
            public int? CountTong { get; set; }
            public int? CountDuyet { get; set; }
        }
        //.Viết hàm để lấy các thuộc tính của clss NvlDuyetGiaItemShow thông qua string name truyền vào lstitem
        public class NvlDuyetGiaItemShow : INotifyPropertyChanged
        {
            public object this[string propertyName]//Lấy giá trị của biến trong class thông qua string name
            {
                get
                {
                    try
                    {
                        Type myType = typeof(NvlDuyetGiaItemShow);
                        PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                        if (myPropInfo != null)
                        {
                            return myPropInfo.GetValue(this, null);
                        }
                        else
                        {
                            //Xử lý khi propertyName không tồn tại
                            throw new ArgumentException($"Property '{propertyName}' does not exist.");
                        }
                    }
                    catch (Exception ex)
                    {
                       // Log lỗi hoặc xử lý ngoại lệ
                        throw new InvalidOperationException($"Error retrieving property '{propertyName}': {ex.Message}", ex);
                    }
                }
                set
                {
                    try
                    {
                        Type myType = typeof(NvlDuyetGiaItemShow);
                        PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                        if (myPropInfo != null)
                        {
                            myPropInfo.SetValue(this, value, null);
                        }
                        else
                        {
                            //Xử lý khi propertyName không tồn tại
                            throw new ArgumentException($"Property '{propertyName}' does not exist.");
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log lỗi hoặc xử lý ngoại lệ
                        throw new InvalidOperationException($"Error setting property '{propertyName}': {ex.Message}", ex);
                    }
                }
            }

            public int Serial { get; set; }
          
            public int? STT { get; set; }

            public int? SerialLink { get; set; }

            public string TableName { get; set; }

            public string MaHang { get; set; }
            public string TenHang { get; set; }

            public string XuatXu { get; set; } = "";

            public string DVT { get; set; }
           
            public decimal? SLDuToan { get; set; }

            public decimal? DonGia { get; set; }

            public decimal? DonGiaTheoDvt { get; set; }

            public decimal? GiaDangMua { get; set; }

            public decimal? DinhMuc { get; set; }

            public string GhiChu { get; set; } = "";

            public string MaNCC { get; set; }
            private string _tenNhaCungCap { get; set; }
            public string TenNCC
            {
                get
                {
                    return _tenNhaCungCap;
                }
                set
                {
                    _tenNhaCungCap = value;
                    NotifyPropertyChanged("TenNCC");
                    NotifyPropertyChanged("DonGia");
                }
            }

            public string UserInsert { get; set; }

            private string _tinhtrangduyet { get; set; }
            public string TinhTrangDuyet
            {
                get { return _tinhtrangduyet; }
                set
                {
                    _tinhtrangduyet = value;
                    NotifyPropertyChanged("TinhTrangDuyet");
                }
            }

            public DateTime? NgayInsert { get; set; }
            public NvlDuyetGiaItemShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlDuyetGiaItemShow>(json);
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            public string TextKiem { get; set; } = "";
            public string TextDuyet { get; set; } = "";
            public List<NvlKyDuyetItemShow> lstduyetitem { get; set; }
            public decimal? NCC0 { get; set; }
            public decimal? NCC1 { get; set; }
            public decimal? NCC2 { get; set; }
            public decimal? NCC3 { get; set; }
            public decimal? NCC4 { get; set; }
            public decimal? NCC5 { get; set; }
            public decimal? NCC6 { get; set; }
            public decimal? NCC7 { get; set; }
            public decimal? NCC8 { get; set; }
            public decimal? NCC9 { get; set; }

            public bool DuyetNCC0 { get; set; } = false;
            public bool DuyetNCC1 { get; set; } = false;
            public bool DuyetNCC2 { get; set; } = false;
            public bool DuyetNCC3 { get; set; } = false;
            public bool DuyetNCC4 { get; set; } = false;
            public bool DuyetNCC5 { get; set; } = false;
            public bool DuyetNCC6 { get; set; } = false;
            public bool DuyetNCC7 { get; set; } = false;
            public bool DuyetNCC8 { get; set; } = false;
            public bool DuyetNCC9 { get; set; } = false;

           
            public string Err { get; set; }
            private string _msgwait { get; set; }
            public string MsgKhongDuyet
            {
                get { return _msgwait; }
                set { _msgwait = value; NotifyPropertyChanged("MsgKhongDuyet"); }
            }
            public string ListColumns { get; set; }
            public string KeyGroup { get; set; }
            public string? NhaCungCapDuyet { get; set; }
            public decimal? DonGiaDuyet { get; set; }
            public bool boolduyet
            {
                get
                {
                    if(TinhTrangDuyet=="Đã duyệt")
                    {
                        if (NhaCungCapDuyet == TenNCC)
                            return true;
                    }
                    return false;
                }
                
            }
        }
        public class NvlDuyetGiaItemShow_Detail
        {

            public int Serial { get; set; }
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
            public string KeyGroupItem { get; set; }
            public decimal DonGia { get; set; }
            public string GhiChu { get; set; }
            public DateTime NgayInsert { get; set; }
            public NvlDuyetGiaItemShow_Detail CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlDuyetGiaItemShow_Detail>(json);
            }
        }
        public class HeaderText
        {
            public int Index { get; set; }
            public string Text { get; set; } = "";
            public string Value { get; set; } = "";
            public string FieldName { get; set; }
            public string DaMua { get; set; } = "";
            public bool Visible { get; set; } = false;
        }

        bool CheckQuyen = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
                await Task.Delay(100);
                CheckQuyen = await phanQuyenAccess.CreateTaoDuyetGia(ModelAdmin.users);
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

                if (LoaiDonHang == "DeNghiDuyetGia")
                {
                    if (CheckQuyen)
                        VisbleTaoDeNghi = true;
                }

                texttaomoi = "TẠO DUYỆT GIÁ";
                //Visilethemtukehoach = true;




                lstnguoidenghi = lstuser.ToList();
                lstnguoiduyet = lstuser.ToList();
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
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                try
                {
                    load();
                    var dimension = await browserService.GetDimensions();
                    // var heighrow = await browserService.GetHeighWithID("divcontainer");
                    int height = dimension.Height - 70;


                    heightgrid = string.Format("{0}px", height);
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

                        nguoiduyet = lstnguoiduyet.FirstOrDefault(p => p.UsersName.Equals(ModelAdmin.users.UsersName)); ;
                    }
                    else
                    {
                        nguoidenghi = lstnguoidenghi.FirstOrDefault(p => p.UsersName.Equals(ModelAdmin.users.UsersName));
                    }
                    renderFragmentflowchart = builder =>
                    {
                        builder.OpenComponent<Urc_Stepper_FlowChart>(0);
                        builder.AddAttribute(1, "TypeName", LoaiDonHang);

                        builder.CloseComponent();
                    };

                    firstload = true;
                    if (LoaiDonHang.Contains("KyDuyet"))
                    {
                      await  searchAsync();
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
            public List<NvlDuyetGiaShow> lstmuahang { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetShow> lstkyduyet { get; set; }
        }
        public class CustomRootItem
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NvlDuyetGiaItemShow> lstmuahangitem { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetItemShow> lstkyduyet { get; set; }
        }

        private async Task searchAsync()
        {
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = "";

            if (dtpbegin == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ngày để xem"));
                return;
            }


            if (dtpbegin != null)
            {
                if (dieukien == "")
                    dieukien = " where NgayInsert>=@DateBegin";
                else
                    dieukien += " and NgayInsert>=@DateBegin";
                lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));

            }
            string dieukienduyet = "";
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


            if (nguoidenghi != null)
            {
                if (dieukien == "")
                    dieukien = " where NguoiDN=@NguoiDN";
                else
                    dieukien += " and NguoiDN=@NguoiDN";
                lstpara.Add(new ParameterDefine("@NguoiDN", nguoidenghi.UsersName.ToString()));
            }
            if (dieukien == "")
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ít nhất 1 điều kiện để tìm kiếm"));
                //msgBox.Show("Vui lòng chọn ít nhất 1 điều kiện để tìm kiếm", IconMsg.iconerror);
                return;
            }

            //grvKeHoach.ItemsSource = null;



            string sql = string.Format(@" USE [NVLDB]  
                                declare @TableName nvarchar(100)='NvlDuyetGia'
                                declare @tblkyduyet as Table([Serial] int,[SerialLink] int,[TableName]  nvarchar(100)
                              ,[UserYeuCau]  nvarchar(100),[UserDuyet]  nvarchar(100),[LoaiDuyet]  nvarchar(100),[DaDuyet]  nvarchar(100)
                              ,[GhiChu]  nvarchar(500),countItemDuyet int)
                                   declare @tbldenghi as Table
								   (Serial int,[NguoiDN] nvarchar(100),LoaiDuyetGia nvarchar(100),DVT nvarchar(100),[LyDo] nvarchar(100),PhongBan nvarchar(100),[NhaMay] nvarchar(100),DuAnHanMuc nvarchar(100),CuocVanChuyen nvarchar(150),ThueGTGT nvarchar(150),KetQua nvarchar(100),
														 ChonNhaCungCap nvarchar(200),[UserInsert] nvarchar(100),Ngay date,GhiChu nvarchar(200),[NgayInsert] datetime,NgayApDung date,NgayKetThuc date)
									insert into @tbldenghi([Serial],[NguoiDN],[LoaiDuyetGia],[DVT]
									,[LyDo],[PhongBan],[NhaMay],[DuAnHanMuc],[CuocVanChuyen],[ThueGTGT],[KetQua],[ChonNhaCungCap],[UserInsert],[Ngay],[GhiChu] ,[NgayInsert],NgayApDung,NgayKetThuc)
                                     SELECT [Serial],[NguoiDN],[LoaiDuyetGia],[DVT] ,[LyDo],[PhongBan],[NhaMay],[DuAnHanMuc]
									,[CuocVanChuyen],[ThueGTGT],[KetQua],[ChonNhaCungCap],[UserInsert],[Ngay],[GhiChu] ,[NgayInsert],NgayApDung,NgayKetThuc
                                     from (Select * from NvlDuyetGia 
                                        {0}
                                      ) as ddh 
                        
						 declare @minserial int
                            declare @maxserial int
                            select @minserial=min(Serial),@maxserial=max(Serial) from @tbldenghi

                            if(@UserDuyet<>'')
                            begin
                             insert into @tblkyduyet([Serial],[SerialLink],[TableName],[UserYeuCau],[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],countItemDuyet)
                                                        select [Serial],[SerialLink],[TableName],[UserYeuCau],qryduyet.[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],isnull(countItemDuyet,0) as countItemDuyet
                                                        from (select * from NvlKyDuyet where UserDuyet=@UserDuyet and TableName=@TableName and SerialLink>=@minserial and SerialLink<=@maxserial) as qryduyet
														left join (SELECT [SerialLinkMaster],[UserDuyet],count(*) as countItemDuyet
   
													FROM [NvlKyDuyetItem] where UserDuyet=@UserDuyet and TableName=@TableName and SerialLinkMaster>=@minserial and SerialLinkMaster<=@maxserial
													group by [SerialLinkMaster],[UserDuyet]) as qryitem on qryduyet.SerialLink=qryitem.SerialLinkMaster and qryduyet.UserDuyet=qryitem.UserDuyet


                            end
                            else
                            begin
	                             insert into @tblkyduyet([Serial],[SerialLink],[TableName],[UserYeuCau],[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],countItemDuyet)
                                                        select [Serial],[SerialLink],[TableName],[UserYeuCau],qryduyet.[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu],isnull(countItemDuyet,0) as countItemDuyet
                                                        from (select * from NvlKyDuyet where  TableName=@TableName and SerialLink>=@minserial and SerialLink<=@maxserial) as qryduyet
														left join (SELECT [SerialLinkMaster],[UserDuyet],count(*) as countItemDuyet
   
													FROM [NvlKyDuyetItem] where TableName=@TableName and SerialLinkMaster>=@minserial and SerialLinkMaster<=@maxserial
													group by [SerialLinkMaster],[UserDuyet]) as qryitem on qryduyet.SerialLink=qryitem.SerialLinkMaster and qryduyet.UserDuyet=qryitem.UserDuyet

                            end

                           
		                            select tbl.*,isnull(CountTong,0) as CountTong,isnull(qrykyduyetitem.SLItemDuyet,0) as CountDuyet,'{2}'+isnull(usr.PathImg,'UserImage/user.png') as PathImgTao
									from @tbldenghi tbl 
		                            left join 
                                      (select SerialDN,SLDong as CountTong from
		                            (SELECT SerialLink as  SerialDN,count(*) as SLDong
		                              FROM [dbo].NvlDuyetGiaItem khitem where SerialLink>=@minserial and SerialLink<=@maxserial 
		                              group by SerialLink) as qryitem)
                                    as qryallitem on tbl.Serial=qryallitem.SerialDN
		                            left join (SELECT SerialLinkMaster,count([SerialLinkItem]) as SLItemDuyet
		                            FROM [NvlKyDuyetItem] where TableName=@TableName and [LoaiDuyet]=N'Duyệt' and SerialLinkMaster>=@minserial and SerialLinkMaster<=@maxserial and SerialLinkItem in (select Serial from NvlDuyetGiaItem)
		                            group by SerialLinkMaster) as qrykyduyetitem
		                            on tbl.Serial=qrykyduyetitem.SerialLinkMaster 
		                            inner join [DBMaster].[dbo].[Users] usr on tbl.NguoiDN=usr.UsersName 
		                            {1}
		                            
                                    if(@UserDuyet<>'')
										select [Serial],[SerialLink],[TableName],[UserYeuCau],[UserDuyet] ,[LoaiDuyet],[DaDuyet],[GhiChu] from NvlKyDuyet where TableName=@TableName and SerialLink in (select [SerialLink] from @tblkyduyet )
								    else
										select * from @tblkyduyet", dieukien, dieukienduyet, ModelAdmin.pathurlfilepublic);
            
            // ShowProgress.ShowAwait();
            try
            {

                lstdata.Clear();
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
                        lstdata = customRoot.lstmuahang;
                    }

                }

                ////Xử lý load ảnh

                PanelVisible = false;


            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
                Console.Error.WriteLine(ex.ToString());

                PanelVisible = false;
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
                //ShowProgress.CloseAwait();
            }
        }
        private void search()
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
        public async void KeHoachMasterAdd()
        {

            NvlDuyetGiaShow nvlDuyetGiaShow = new NvlDuyetGiaShow();

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DuyetGia_AddMaster>(0);
                builder.AddAttribute(1, "nvlDuyetGiaShowcrr", nvlDuyetGiaShow);

                builder.CloseComponent();
            };

           await dxPopup.showAsync("TẠO MẪU DUYỆT GIÁ");
           await dxPopup.ShowAsync();
        }
        public async Task KeHoachMasterEditAsync()
        {

            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;


            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DuyetGia_AddMaster>(0);
                builder.AddAttribute(1, "nvlDuyetGiaShowcrr", nvlDuyetGiaShowcrr.CopyClass());

                builder.AddAttribute(2, "AfterEdit", EventCallback.Factory.Create<NvlDuyetGiaShow>(this, RefreshItemUpdateAsync));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
           await dxPopup.showAsync("SỬA ĐỀ NGHỊ");

            await dxPopup.ShowAsync();

        }
        public async Task KeHoachMasterDeleteAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            bool bl = await dialogMsg.Show("Xóa đề nghị", $"Bạn có chắc muốn xóa duyệt giá số {nvlDuyetGiaShowcrr.Serial.ToString()}");

            if (!phanQuyenAccess.CheckDelete(nvlDuyetGiaShowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có quyền xóa dòng này do bạn không phải người tạo"));
                //msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
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
                    string sql = "NVLDB.dbo.NvlDuyetGia_Delete";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    lstpara.Add(new ParameterDefine("@Serial", nvlDuyetGiaShowcrr.Serial.ToString()));
                    lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Success, "Xóa thành công"));
                            // msgBox.Show("Xóa thành công", IconMsg.iconssuccess);
                            lstdata.Remove(nvlDuyetGiaShowcrr);
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
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));

                }

            }
        }


        public async Task KeHoachMasterAddItemAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!phanQuyenAccess.CheckDelete(nvlDuyetGiaShowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không phải người tạo, nên không có quyền thêm"));
                return;
            }



            //renderFragment = builder =>
            //{
            //    builder.OpenComponent<Urc_DuyetGia_AddItem>(0);
            //    builder.AddAttribute(1, "nvlDuyetGiaShowcrr", nvlDuyetGiaShowcrr.CopyClass());

            //    builder.AddAttribute(2, "AfterEdit", EventCallback.Factory.Create<NvlDuyetGiaShow>(this, RefreshItemUpdateAsync));
            //    //builder.OpenComponent(0, componentType);
            //    builder.CloseComponent();
            //};
            //dxPopup.show("THÊM VẬT TƯ");

            //await dxPopup.ShowAsync();
            MenuItem menuItem = new MenuItem();
            menuItem.TextItem = "Thêm vật tư";
            menuItem.NameItem = "createtaoduyetgia";
            menuItem.ComponentName = "NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.Urc_DuyetGia_AddItem";
            menuItem.IconCssClass = "bi bi-cart3";
            if (ModelAdmin.mainLayout != null)
            {
                RenderFragment renderFragment1;
                renderFragment1 = builder =>
                {

                    builder.OpenComponent<Urc_DuyetGia_AddItem>(0);
                    builder.AddAttribute(1, "nvlDuyetGiaShowcrr", nvlDuyetGiaShowcrr.CopyClass());

                    builder.CloseComponent();
                };
                ModelAdmin.mainLayout.AddDirectRenderfagment(menuItem, renderFragment1);

            }

        }



        public async Task KeHoachMasterChonNguoiDuyettAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(nvlDuyetGiaShowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Bạn không có quyền duyệt dòng này do bạn không phải người tạo hoặc chưa được chọn duyệt"));
                // msgBox.Show("Bạn không có quyền xóa dòng này do bạn không phải người tạo", IconMsg.iconerror);
                return;
            }


            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;

            string KyDuyet = "KyDuyetGia";
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_ChonNguoiDuyet>(0);
                builder.AddAttribute(1, "nvlDuyetGiaShowcrr", nvlDuyetGiaShowcrr);
                builder.AddAttribute(2, "KyDuyet", KyDuyet);
                builder.AddAttribute(3, "GotoMainFormDG", EventCallback.Factory.Create<NvlDuyetGiaShow>(this, RefreshListDuyet));

                builder.CloseComponent();
            };
        await    dxPopup.showAsync("CHỌN NGƯỜI DUYỆT");

            await  dxPopup.ShowAsync();


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
        private async Task RefreshItemUpdateAsync(NvlDuyetGiaShow nvlDuyetGiaShow_get)
        {
            await dxPopup.CloseAsync();
            nvlDuyetGiaShowcrr.NguoiDN = nvlDuyetGiaShow_get.NguoiDN;
            nvlDuyetGiaShowcrr.LoaiDuyetGia = nvlDuyetGiaShow_get.LoaiDuyetGia;
            nvlDuyetGiaShowcrr.DVT = nvlDuyetGiaShow_get.DVT;
            nvlDuyetGiaShowcrr.LyDo = nvlDuyetGiaShow_get.LyDo;
            nvlDuyetGiaShowcrr.PhongBan = nvlDuyetGiaShow_get.PhongBan;
            nvlDuyetGiaShowcrr.NhaMay = nvlDuyetGiaShow_get.NhaMay;
            nvlDuyetGiaShowcrr.DuAnHanMuc = nvlDuyetGiaShow_get.DuAnHanMuc;
            nvlDuyetGiaShowcrr.CuocVanChuyen = nvlDuyetGiaShow_get.CuocVanChuyen;
            nvlDuyetGiaShowcrr.ThueGTGT = nvlDuyetGiaShow_get.ThueGTGT;
            nvlDuyetGiaShowcrr.KetQua = nvlDuyetGiaShow_get.KetQua;
            nvlDuyetGiaShowcrr.ChonNhaCungCap = nvlDuyetGiaShow_get.ChonNhaCungCap;
            nvlDuyetGiaShowcrr.UserInsert = nvlDuyetGiaShow_get.UserInsert;
            nvlDuyetGiaShowcrr.Ngay = nvlDuyetGiaShow_get.Ngay;
            nvlDuyetGiaShowcrr.GhiChu = nvlDuyetGiaShow_get.GhiChu;
            Grid.SaveChangesAsync();

        }

        private class CustomRootPrint
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NvlDuyetGiaItemShow> lstitem { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public DataTable dtduyet { get; set; }
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
                            ", nvlDuyetGiaShowcrr.Serial);
              
                sql = string.Format(@"use NVLDB                            
                declare @SerialLink int={0}
                   SELECT it.SerialLink,it.Serial,STT,it.KeyGroup,it.[MaHang],it.[XuatXu],isnull([SLDuToan],0) as SLDuToan,isnull([DinhMuc]*detail.DonGia,0) as DonGiaTheoDvt,
                  isnull(detail.DonGia,0) as DonGia,isnull([DonGiaDuyet],0) as DonGiaDuyet,it.GhiChu,case 
                  when TinhTrangDuyet=N'Đã duyệt' then  isnull([NhaCungCapDuyet],'')
                  when TinhTrangDuyet=N'Không duyệt' then N'Không duyệt vì: '+MsgKhongDuyet
                  else isnull([NhaCungCapDuyet],'') end as [NhaCungCapDuyet]
                  ,detail.TenNCC,hh.TenHang,isnull([GiaDangMua],0) as GiaDangMua,isnull([TinhTrangDuyet],'') as TinhTrangDuyet,hh.DVT,MsgKhongDuyet
                    FROM (select * from [dbo].[NvlDuyetGiaItem] where SerialLink=@SerialLink) it
                    inner join dbo.NvlDuyetGiaItem_Detail detail on it.KeyGroup=detail.KeyGroupItem
                    inner join dbo.NvlHangHoa hh on it.MaHang=hh.MaHang
   
          select qry.*,qryrp.* ,isnull(kyduyet.LoaiDuyet,'') as LoaiDuyet,isnull(kyduyet.UserDuyet,'') as UserDuyet,'' as NguoiDuyet,'' as NguoiKiemTra,'' as NguoiDeNghi
           from 
           (SELECT  *
            FROM NvlDuyetGia where Serial=@SerialLink
            ) as qry 
           left join dbo.GetReportDetail_DuyetGia(@SerialLink)  as qryrp on qry.Serial=qryrp.SerialLink
		   left join (select * from dbo.NvlKyDuyet where TableName='NvlDuyetGia' and SerialLink=@SerialLink) kyduyet on qry.Serial=kyduyet.SerialLink", nvlDuyetGiaShowcrr.Serial);

                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();

                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {

                    CustomRootPrint lstmaster = JsonConvert.DeserializeObject<CustomRootPrint>(json);
                   if(lstmaster!=null)
                    {
                        DataTable dtduyet = lstmaster.dtduyet;
                        List<NvlDuyetGiaItemShow> lst = lstmaster.lstitem;
                        
                        var querykiemtra = dtduyet.Select("LoaiDuyet='Kiểm tra'").FirstOrDefault();
                        var queryduyet = dtduyet.Select("LoaiDuyet='Duyệt'").FirstOrDefault();
                        var querytable=dtduyet.Select().FirstOrDefault();
                      
                        string userduyet = "";
                        string userkiemtra = "";
                        string userdenghi = "";
                        if(querytable != null)
                        {
                            if(queryduyet!=null)
                                userduyet=lstuser.Where(x => x.UsersName == queryduyet.Field<string>("UserDuyet")).Select(x => x.TenUser).FirstOrDefault();
                            if(querykiemtra!=null)
                                userkiemtra=lstuser.Where(x => x.UsersName == querykiemtra.Field<string>("UserDuyet")).Select(x => x.TenUser).FirstOrDefault();
                           
                            userdenghi=lstuser.Where(x => x.UsersName == querytable.Field<string>("NguoiDN")).Select(x => x.TenUser).FirstOrDefault();
                            foreach (DataRow row in dtduyet.Rows)
                            {
                                row["NguoiDeNghi"] = userdenghi;
                                row["NguoiDuyet"] = userduyet;
                                row["NguoiKiemTra"] = userkiemtra;
                            }
                            
                        }
                        var querygroup = lst.GroupBy(p => new { tinhtrang = p.TinhTrangDuyet, keygroup = p.KeyGroup }).Select(p => new { tinhtrang = p.Key.tinhtrang, keygroup = p.Key.keygroup }).ToList();
                        var queryketqua=querygroup.Where(p=>!string.IsNullOrEmpty(p.tinhtrang))
                            .GroupBy(p => new { tinhtrang = p.tinhtrang }).Select(p => new { tinhtrang=p.Key.tinhtrang,countSL=p.Count()}).ToList();
                        string ketqua = "";
                        int sumtotal = querygroup.Count();
                        foreach (var item in queryketqua)
                        {
                            if (ketqua != "")
                                ketqua += Environment.NewLine;
                           ketqua +=string.Format("{0}: {1}/{2}",item.tinhtrang, item.countSL, sumtotal);
                            
                        }

                       
                        XtraRp_DuyetGia xtraRp_DuyetGia = new XtraRp_DuyetGia();
                        xtraRp_DuyetGia.setLoaiDuyetGia(nvlDuyetGiaShowcrr.LoaiDuyetGia);
                        XRSubreport xrqtitem = xtraRp_DuyetGia.FindControl("xrSubreport1", true) as XRSubreport;
                        //  XtraRp_DonDatHangItem xtraRp_DonDatHangItem = (XtraRp_DonDatHangItem)xrqtitem.ReportSource;
                        XtraRp_DuyetGiaDetailDauMau xtra_KTGTonKho = (XtraRp_DuyetGiaDetailDauMau)xrqtitem.ReportSource;
                        xtra_KTGTonKho.setTypereport(nvlDuyetGiaShowcrr.LoaiDuyetGia);
                        xtra_KTGTonKho.setInitList(lst);
                        querytable["KetQua"] = ketqua;
                        xtraRp_DuyetGia.DataSource = dtduyet;
                       // Console.WriteLine(querytable.Field<string>("NguoiDeNghi"));

                        ModelAdmin.mainLayout.showreportAsync(xtraRp_DuyetGia);
                    }
                   else
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                        return;
                    }

                    // XtraRp_DuyetGiaDetailItem xtra_KTGTonKho = new XtraRp_DuyetGiaDetailItem();
                 

                    //lst.Clear();

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

        
        public string TextSoDeNghi(NvlDuyetGiaShow keHoachMuaHang_Show)
        {

            return "Duyệt giá số " + keHoachMuaHang_Show.Serial.ToString(); ;

        }
        public string TextNgayDeNghi(NvlDuyetGiaShow keHoachMuaHang_Show)
        {

            return "Ngày tạo " + keHoachMuaHang_Show.Ngay.Value.ToString("dd/MM/yy");


        }

        public async void ShowFlyout(NvlDuyetGiaShow keHoachMuaHang_Show)
        {
            await dxFlyoutchucnang.CloseAsync();
            nvlDuyetGiaShowcrr = keHoachMuaHang_Show;
            idflychucnang = "#" + idelement(keHoachMuaHang_Show.Serial);
            Visileprint = true;

            if (CheckQuyen)
            {
                Visilechinhsua = true;
                Visilechinhtruocin = true;

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
            IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();

        }
        public async Task ShowTruocInAsync()
        {

            await dxFlyoutchucnang.CloseAsync();
            string noidung = "";


            IsOpenfly = false;
            //renderFragment = builder =>
            //{
            //    builder.OpenComponent<Urc_KeHoachChinhSuaBanIn>(0);
            //    builder.AddAttribute(1, "GotoMainForm", EventCallback.Factory.Create<KeHoachMuaHang_Show>(this, RefreshListDuyet));

            //    builder.AddAttribute(3, "keHoachMuaHang_Show", "kehoachshowcrr");

            //    builder.CloseComponent();
            //};

         await   dxPopup.showAsync("CHỈNH SỬA BẢN IN");
          await  dxPopup.ShowAsync();

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
        string img64 = "";
      
        private async Task ShowItemReportAsync()
        {
            await dxPopup.CloseAsync();
            await dxFlyoutchucnang.CloseAsync();
            if (nvlDuyetGiaShowcrr == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Chưa có duyệt giá nào được chọn"));
                return;
            }
            NvlReportDetailShow nvlReportDetailShow = new NvlReportDetailShow();
            
            nvlReportDetailShow.NameReport = "XtraRp_DuyetGia";
            nvlReportDetailShow.SerialLink = nvlDuyetGiaShowcrr.Serial;
            nvlReportDetailShow.TableName = "NvlDuyetGia";
            string sql = "";
            try
            {
                sql = string.Format(@"Use NVLDB
                        declare @SerialLink int={0}
                        declare @TableName nvarchar(100)='{1}'
	                    declare @NameReport nvarchar(100)='{2}'
                       
                       declare @UserInsert nvarchar(100)=N'{3}'

                        declare @CheckCount int
	                    select top 1 @CheckCount=count(*) from [dbo].[NvlReportDetail]  where SerialLink=@SerialLink and TableName=@TableName and NameReport=@NameReport
	                    if(@CheckCount=0)
	                    begin
		                    --Lấy dữ liệu của dòng gần nhất
		                    declare @serialrecent int
		                    SELECT TOP 1 @serialrecent=[Serial] FROM [dbo].[NvlDonDatHang]
		                    where UserInsert =@UserInsert  and Serial in (SELECT  [SerialLink]
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
                    ", nvlDuyetGiaShowcrr.Serial, nvlReportDetailShow.TableName, nvlReportDetailShow.NameReport, nvlDuyetGiaShowcrr.UserInsert);
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

                  await  dxPopup.showAsync("THÊM THÔNG TIN TRONG REPORT");
                  await  dxPopup.ShowAsync();

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
    }

}
