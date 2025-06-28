using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Urc_NvlNhapXuatGhiNo
    {
        [Inject]ToastService ToastService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            lstkhonvl = await Model.ModelData.GetKhoNvl();
            
           // return base.OnInitializedAsync();
        }
        private async void ClickInTem(NvlNhapXuatItemShow nvlNhapXuatItemShow)
        {
            NvlInTemShow nvlInPhieuShow = new NvlInTemShow();
            nvlInPhieuShow.Serial = 0;
            nvlInPhieuShow.BanIn = 1;
            nvlInPhieuShow.MaHang=nvlNhapXuatItemShow.MaHang;
            nvlInPhieuShow.SoLuong=(double?)nvlNhapXuatItemShow.SoLuong;

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NvlInTem>(0);
                builder.AddAttribute(1, "nvlInTemShowcrr", nvlInPhieuShow);

                // builder.AddAttribute(3, "AfterSave", EventCallback.Factory.Create<NvlNhapXuatKhoShow>(this, GotoMainFormAsync));
                builder.CloseComponent();
            };
           await dxPopup.showAsync("TẠO TEM BARCODE");
           await dxPopup.ShowAsync();
        }
        private async Task searchAsync()
        {
            if(string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn mã kho"));
                return;
            }
            List<ParameterDefine> lstpara =new  List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
            string dieukien = "";
            if(!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
            {
                lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItemShowcrr.MaHang));
                dieukien=" and hh.MaHang=@MaHang";
            }
            if (lstdata == null)
                lstdata = new List<NvlNhapXuatItemShow>();
                lstdata.Clear();
            
                try
                {
                    string sql = string.Format(@" use NVLDB
                  
                    select hh.TenHang,hh.MaHang,hh.DVT,qry.SLNhap as SoLuong,qry.LyDo,qry.MaKho,mk.TenKho as GhiChu from 
                    (SELECT  [MaHang],MaKho,sum(SLXuat-SLNhap) as SLNhap,nx.LyDo   
                      FROM 
                      (select * from [dbo].NvlNhapXuat where LyDo 
                      in (SELECT [LyDo]
                      FROM [dbo].[NvlNhapXuat_LyDo] where Tag=N'LyDoNo') ) nx 
                      inner join
                      [dbo].[NvlNhapXuatItem] item on nx.Serial=item.SerialCT where  MaKho=@MaKho 
                      group by MaHang,nx.LyDo,MaKho   union all
                      SELECT [MaHang],MaKho,sum([SoLuong]) as SoLuong,[LyDo]
                      FROM [NvlGhiNoHieuChinh]
                      where [TableName]='NvlNhapXuatItem' and MaKho=@MaKho
                      group by MaHang,LyDo,MaKho) as qry
                      inner join  NvlHangHoa hh on qry.MaHang=hh.MaHang inner join  [dbo].NvlMaKho mk on qry.MaKho=mk.MaKho   where qry.SLNhap<>0 {0}
                      order by LyDo,hh.TenHang
                    ",dieukien);

                    CallAPI callAPI = new CallAPI();
                    PanelVisible = true;
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemShow>>(json);
                        lstdata = query;
                       
                        // await GotoMainForm.InvokeAsync();
                    }
                    lstpara.Clear();
                }
                catch (Exception ex)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Success, $"Lỗi: " + ex.Message));
                }
                finally
                {
                    PanelVisible = false;

                    StateHasChanged();

                }

            
         
        }
    }
}
