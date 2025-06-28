using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_ViTriMaster
    {
        [Inject] ToastService toastService { get; set; }

        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        bool Ismobile { get; set; } = false;
        bool firstload = false;
        protected override async Task OnInitializedAsync()
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
            {
                Ismobile = false;
                
            }
            heightgrid = string.Format("{0}px", height);
            await loadAsync();
            //randomdivhide = prs.RandomString(10);
            //return base.OnInitializedAsync();
        }
        private async Task loadAsync()
        {
            lstkhonvl = await Model.ModelData.GetKhoNvl();
            lstuser = await Model.ModelData.Getlstusers();

        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _ = NFCWebBlazor.Model.ModelData.GetHangHoa();
            }
            if (Ismobile)
            {
                if (!firstload)//Load lần đầu để gán biến
                {
                    if (lstuser != null)
                    {
                        userselect = lstuser.FirstOrDefault(p => p.UsersName.Equals(ModelAdmin.users.UsersName));
                        firstload = true;
                        StateHasChanged();
                    }

                }
            }
            //await JS.InvokeVoidAsync("scrollToBottomLast");
            //base.OnAfterRender(firstRender);
        }
        private async void ShowInTemViTri()
        {

            NvlNhapXuatItemShow nvlNhapXuatItemShow = new NvlNhapXuatItemShow();
            nvlNhapXuatItemShow.NhaMay = ModelAdmin.users.NhaMay;
            nvlNhapXuatItemShow.MaKho = ModelAdmin.MaKhoSelected;

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_ViTriAdd>(0);
                builder.AddAttribute(1, "nvlNhapXuatItemShowcrr", nvlNhapXuatItemShow);

                // builder.AddAttribute(3, "AfterSave", EventCallback.Factory.Create<NvlNhapXuatKhoShow>(this, GotoMainFormAsync));
                builder.CloseComponent();
            };

            await dxPopup.showAsync("THÊM VÀO VỊ TRÍ");
            await dxPopup.ShowAsync();
        }
        public async Task searchAsync()
        {
            if (lstdata == null)
                lstdata = new List<NvlNhapXuatItemShow>();
            lstdata.Clear();

            if (dtpbegin == null || dtpend == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Chọn ngày"));

                return;
            }

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = " ";

            string dieukienmahang = " ";
            dieukien = " where NgayInsert>=@DateBegin and NgayInsert<=@DateEnd";
            lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
            lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
            
           
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.ViTri))
            {
                dieukien += "  and ViTri = @ViTri";
                lstpara.Add(new ParameterDefine("@ViTri", nvlNhapXuatItemShowcrr.ViTri));
            }

            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            {
                dieukienmahang += " where MaKho = @MaKho";
                lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
            }
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
            {
                dieukien += " where MaHang = @MaHang";
                lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItemShowcrr.MaHang));
            }
            if (userselect!=null)
            {
                dieukien += "  and UserInsert = @UserInsert";
                lstpara.Add(new ParameterDefine("@UserInsert", userselect.UsersName));
            }

           


            string sqlSearch = string.Format(@"Use NVLDB
                select qry.*,hh.TenHang,hh.DVT from
                (SELECT  [Serial],[SerialNK] as SerialLink,[MaHang],[ViTri],[GroupPallet],[UserInsert],[MaKho],[NhaMay],[NgayInsert]
                  FROM [NvlViTri] {0}) as qry inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang  order by qry.Serial desc", dieukien);

        
                //string[] arrshow = new string[] { "MaHang", "TenHang", "SerialCT", "SLNhap", "SLXuat", "DVT", "LyDo", "TenSP", "MaKien", "DauTuan", "SoLo", "TenGN", "SoXe", "GhiChu", "DonGia", "NgayHetHan", "SerialLink", "KhachHang_XuatXu", "UserInsert", "SerialKHDH", "SerialDN", "NguoiDeNghi", "TenLienKet", "NhaMay", "NgayInsert" };
                bool checkshow = false;



           
            CallAPI callAPI = new CallAPI();
            try
            {


                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sqlSearch, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemShow>>(json);
                    lstdata = query;
                    if (query.Any())
                    { await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide)); }

                    // await GotoMainForm.InvokeAsync();
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;

                StateHasChanged();
            }

        }
    }
}
