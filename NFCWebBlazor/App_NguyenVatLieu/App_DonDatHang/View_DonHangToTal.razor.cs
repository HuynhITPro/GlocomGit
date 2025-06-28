using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat;
using NFCWebBlazor.Model;
using System.Collections.ObjectModel;
using static NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Page_DonDatHang_Master;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class View_DonHangToTal
    {
     
        public class DonHangTotalShow
        {
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string DVT { get; set; }
            public decimal SLNoCu { get; set; }
            public decimal? SLTonKho { get; set; }
            public decimal SLDatHang { get; set; }
            public decimal SLDaGiao { get; set; }
            public decimal SLTheoDoi { get; set; }
            public decimal ThanhTienTonDau { get; set; }

            public decimal ThanhTienDatHang { get; set; }
            public decimal ThanhTienDaGiao { get; set; }
            public decimal ThanhTienTheoDoi { get; set; }
            public string TenNCC { get; set; }
            public string MaNCC { get; set; }
            public double? TyLe { get; set; }
            public ObservableCollection<NVLDonDatHangItemShow> lstitem { get; set; }

        }
        public class TonKho
        {
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string DVT { get; set; }
            public decimal SLTon { get; set; }
        }
        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<DonHangTotalShow> lstdonhang { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<TonKho> lsttk { get; set; }
        }
        private async Task searchAsync()
        {

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukienphanloai = "";
            string dieukienmahang = "";
            if (lstdata == null)
                lstdata = new List<DonHangTotalShow>();
            lstdata.Clear();
            string dieukien = "";
            title = "";
            title = string.Format("Đơn hàng từ ngày {0} đến ngày {1}", dieuKienTimKiem.DateBegin.ToString("dd/MM"), dieuKienTimKiem.DateEnd.ToString("dd/MM"));
            if (dieuKienTimKiem.DateBegin == null || dieuKienTimKiem.DateEnd == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn thời gian"));
                return;
            }
            if (dieuKienTimKiem.SerialDN != null && dieuKienTimKiem.SerialDN > 0)
            {
                dieukien = string.Format(" and Serial =@SerialDN");


                lstpara.Add(new ParameterDefine("@SerialDN", dieuKienTimKiem.SerialDN));
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

                    dieukien += " and UserInsert=@NguoiDN";


                    lstpara.Add(new ParameterDefine("@NguoiDN", dieuKienTimKiem.NguoiDN));

                }
                if(!string.IsNullOrEmpty(dieuKienTimKiem.NhaCungCap))
                {
                    dieukien += " and MaNCC=@MaNCC";


                    lstpara.Add(new ParameterDefine("@MaNCC", dieuKienTimKiem.NhaCungCap));
                }

                if (dieuKienTimKiem.DateBegin != null)
                {
                    dieukien += " and NgayTao>=@DateBegin";
                    lstpara.Add(new ParameterDefine("@DateBegin", dieuKienTimKiem.DateBegin.ToString("MM/dd/yyyy 00:00")));
                }
                if (dieuKienTimKiem.DateEnd != null)
                {
                    dieukien += " and NgayTao<=@DateEnd";
                    lstpara.Add(new ParameterDefine("@DateEnd", dieuKienTimKiem.DateEnd.ToString("MM/dd/yyyy 23:59")));
                }
                if (!string.IsNullOrEmpty(dieuKienTimKiem.MaHang))
                {
                    dieukienmahang = " where hh.MaHang=@MaHang";
                    lstpara.Add(new ParameterDefine("@MaHang", dieuKienTimKiem.MaHang));
                }
            }
            if (dieukien != "")
            {
                dieukien = " where " + dieukien.Substring(5);
            }



            string sql = string.Format(@" use NVLDB
                   select qry.MaHang,hh.TenHang,qry.MaNCC,hh.DVT,qry.SLNoCu,qry.SLDatHang,qry.SLTheoDoi,qry.SLDatHang-qry.SLTheoDoi as SLDaGiao,qry.ThanhTienTonDau,qry.ThanhTienDatHang,qry.ThanhTienTheoDoi,ncc.TenNCC,case when SLDatHang=0 then 0 else (SLDatHang-SLTheoDoi)/SLDatHang end as TyLe from 
                    (
                    select MaHang,MaNCC,sum(SLNoCu) as SLNoCu,sum(SLDatHang) as SLDatHang,sum(SLTheoDoi) as SLTheoDoi,sum(ThanhTienTonDau) as ThanhTienTonDau,sum(ThanhTienDatHang) as ThanhTienDatHang,sum(ThanhTienTheoDoi) as ThanhTienTheoDoi
                    from
                    (SELECT [MaHang],sum([SLTheoDoi]) as SLNoCu,0 as SLDatHang,0 as SLTheoDoi,sum(SLTheoDoi*DonGia) as ThanhTienTonDau,0 as ThanhTienDatHang,0 as ThanhTienTheoDoi,MaNCC
                    FROM [NvlDonDatHangItem] 
                    where SLTheoDoi>0 and SerialMaDH in (select Serial from NvlDonDatHang where NgayTao<@DateBegin)
                    group by MaHang,MaNCC
                    union all
                    SELECT [MaHang],0 as SLNoCu,sum(SLDatHang) as SLDatHang,sum(SLTheoDoi) as SLTheoDoi,sum(SLTheoDoi*DonGia) as ThanhTienTonDau,sum(SLDatHang*DonGia) as ThanhTienDatHang,sum(SLTheoDoi*DonGia) as ThanhTienTheoDoi,MaNCC
                    FROM [NvlDonDatHangItem] 
                    where  SerialMaDH in (select Serial from NvlDonDatHang {0})
                    group by MaHang,MaNCC
                    ) as qrytotal group by MaHang,MaNCC) as qry  
                    inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                    inner join dbo.NvlNhaCungCap ncc on qry.MaNCC=ncc.MaNCC {1}

                    select MaHang,SLTon from dbo.GetTonKhoEx() as hh  {1} {2}"
                , dieukien, dieukienmahang,(dieukienmahang=="")? " where hh.SLTon>0": " and hh.SLTon>0");

            PanelVisible = true;
            CallAPI callAPI = new CallAPI();
            try
            {

                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {
                    var custom = JsonConvert.DeserializeObject<CustomRoot>(json);
                    lstdata.AddRange(custom.lstdonhang);
                    foreach (var it in custom.lsttk)
                    {
                        foreach (var item in lstdata)
                        {
                            if (it.MaHang == item.MaHang)
                            {
                                item.SLTonKho = it.SLTon;
                                break;
                            }
                        }
                    }
                    await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi: " + ex.Message));

            }
            finally
            {
                PanelVisible = false;
                Grid.Reload();
                StateHasChanged();

            }
        }
        private async void ShowInTemMasterAdd(DonHangTotalShow donHangTotalShow)
        {
            nvlInPhieuShowAdd.MaHang = donHangTotalShow.MaHang;
            nvlInPhieuShowAdd.SoLuong = (double)donHangTotalShow.SLTheoDoi;
            nvlInPhieuShowAdd.DVT = donHangTotalShow.DVT;
            nvlInPhieuShowAdd.GhiChu = "In từ đơn hàng";
            nvlInPhieuShowAdd.MaGN = donHangTotalShow.MaNCC;
            nvlInPhieuShowAdd.BanIn = 1;

            if (string.IsNullOrEmpty(ModelAdmin.PhanLoaiHang))
            {
                renderFragment = builder =>
                {
                    builder.OpenComponent<Urc_PhanLoaiNhomHang>(0);

                    builder.AddAttribute(1, "GotoMainForm", EventCallback.Factory.Create(this, hidePopupAsync));
                    //builder.OpenComponent(0, componentType);
                    builder.CloseComponent();
                };
                await dxPopup.showAsync("Chọn nhóm hàng / kho");
                await dxPopup.ShowAsync();
            }
            else
            {
                await dxPopupInTem.showAsync("IN TEM MỚI");
                await dxPopupInTem.ShowAsync();
            }

        }
        private async Task hidePopupAsync()
        {
            await dxPopup.CloseAsync();
            
            await dxPopupInTem.showAsync("IN TEM MỚI");
            await dxPopupInTem.ShowAsync();
        }
    }
}
