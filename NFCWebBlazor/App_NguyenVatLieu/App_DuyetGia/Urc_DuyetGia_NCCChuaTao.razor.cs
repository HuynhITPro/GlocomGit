using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using static NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.Page_DuyetGiaMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public partial class Urc_DuyetGia_NCCChuaTao
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
        bool CheckQuyen = false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
                await Task.Delay(100);
                CheckQuyen = await phanQuyenAccess.CreateNhaCungCap(ModelAdmin.users);
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
                        await searchAsync();
                    }
                    StateHasChanged();
                }

            }
            //return base.OnAfterRenderAsync(firstRender);
        }
        private void load()
        {

        }
        public string TextSoDeNghi(NvlDuyetGiaShow keHoachMuaHang_Show)
        {

            return "Duyệt giá số " + keHoachMuaHang_Show.Serial.ToString(); ;

        }
        public string TextNgayDeNghi(NvlDuyetGiaShow keHoachMuaHang_Show)
        {

            return "Ngày tạo " + keHoachMuaHang_Show.Ngay.Value.ToString("dd/MM/yy");


        }
        public class NvlNhaCungCapAddLink
        {
            public string KeyGroup { get; set; }
            public int SerialLink { get; set; }
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
        }
        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<NvlDuyetGiaShow> lstmuahang { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlNhaCungCapAddLink> lstnhacungcapaddlink { get; set; }
        }
        List<NvlNhaCungCapAddLink> listNvlNhaCungCap { get; set; }
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
          

            if (dtpend != null)
            {
                if (dieukien == "")
                    dieukien = " where NgayInsert<=@DateEnd";
                else
                    dieukien += " and NgayInsert<=@DateEnd";
                lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
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
            string dieukientinhtrang = "  where NhaCungCapDuyet is not null and MaNCCDuyet is null";
            if(tinhtrangselected== "Tất cả")
            {
                dieukientinhtrang = " where NhaCungCapDuyet is not null";
            }
            //grvKeHoach.ItemsSource = null;
            string sql = string.Format(@" USE [NVLDB]  
                                 declare @TableName nvarchar(100)='NvlDuyetGia'
                              
							declare @tblnccselect Table(KeyGroup nvarchar(100),SerialLink int,TenNCC nvarchar(100),MaNCC nvarchar(100))
								insert into @tblnccselect(KeyGroup,SerialLink,TenNCC,MaNCC)
								SELECT KeyGroup,[SerialLink],NhaCungCapDuyet as TenNCC,MaNCCDuyet
								  FROM [dbo].[NvlDuyetGiaItem]

								 {2}
                             
                                   declare @tbldenghi as Table
								   (Serial int,[NguoiDN] nvarchar(100),LoaiDuyetGia nvarchar(100),DVT nvarchar(100),[LyDo] nvarchar(100),PhongBan nvarchar(100),[NhaMay] nvarchar(100),DuAnHanMuc nvarchar(100),CuocVanChuyen nvarchar(150),ThueGTGT nvarchar(150),KetQua nvarchar(100),
														 ChonNhaCungCap nvarchar(200),[UserInsert] nvarchar(100),Ngay date,GhiChu nvarchar(200),[NgayInsert] datetime,NgayApDung date,NgayKetThuc date)
									insert into @tbldenghi([Serial],[NguoiDN],[LoaiDuyetGia],[DVT]
									,[LyDo],[PhongBan],[NhaMay],[DuAnHanMuc],[CuocVanChuyen],[ThueGTGT],[KetQua],[ChonNhaCungCap],[UserInsert],[Ngay],[GhiChu] ,[NgayInsert],NgayApDung,NgayKetThuc)
                                     SELECT [Serial],[NguoiDN],[LoaiDuyetGia],[DVT] ,[LyDo],[PhongBan],[NhaMay],[DuAnHanMuc]
									,[CuocVanChuyen],[ThueGTGT],[KetQua],[ChonNhaCungCap],[UserInsert],[Ngay],[GhiChu] ,[NgayInsert],NgayApDung,NgayKetThuc
                                     from (Select * from NvlDuyetGia 
                                        {0}
										 and Serial in (select SerialLink from @tblnccselect group by SerialLink)
                                      ) as ddh 
		                            select tbl.*,0 as CountTong,0 as CountDuyet,'{1}'+isnull(usr.PathImg,'UserImage/user.png') as PathImgTao
									from @tbldenghi tbl 
		                           
		                            inner join [DBMaster].[dbo].[Users] usr on tbl.NguoiDN=usr.UsersName 

		                               select MaNCC,SerialLink,TenNCC from @tblnccselect group by SerialLink,TenNCC,MaNCC
                                    ", dieukien, ModelAdmin.pathurlfilepublic,dieukientinhtrang);

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
                        listNvlNhaCungCap = customRoot.lstnhacungcapaddlink;
                    }

                }
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
    }
}
