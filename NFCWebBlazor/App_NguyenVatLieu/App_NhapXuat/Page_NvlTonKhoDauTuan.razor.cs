using BlazorBootstrap;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_NvlTonKhoDauTuan
    {
        [Inject] ToastService toastService { get; set; }
        bool Ismobile { get; set; }
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        protected override async Task OnInitializedAsync()
        {
            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 70;
            heightgrid = string.Format("{0}px", height);
            nvlNhapXuatItemShowcrr.NhaMay = ModelAdmin.users.NhaMay;
            int width = dimension.Width;
            if (width < 768)
            {
                Ismobile = true;
                idgrid = "customGridnotheader";
            }
            else
            {
                Ismobile = false;
                idgrid = "abc";
            }
            //randomdivhide = prs.RandomString(10);
            await loadAsync();
            //return base.OnInitializedAsync();
        }
        private async Task loadAsync()
        {
            lstkhonvl = await Model.ModelData.GetKhoNvl();

            lstmanhom = await Model.ModelData.GetlstNhomhang();
        }
        private string xulydieukientinhtrangsudung()
        {
            string dieukientinhtrangsudung = "";
            string dangsudung = "";
            foreach (var it in khuvucselected)
            {
                if (it.Name == "Hàng đang sử dụng")
                {
                    ghichutemp += Environment.NewLine + it.Name;
                    dangsudung = " TinhTrangSuDung is null";
                }
                else
                {
                    if (dieukientinhtrangsudung == "")
                    {
                        dieukientinhtrangsudung = string.Format("N'{0}'", it.Name);
                    }
                    else
                    {
                        dieukientinhtrangsudung += string.Format(",N'{0}'", it.Name);
                    }
                    ghichutemp += Environment.NewLine + it.Name;
                }

            }
            if (dieukientinhtrangsudung != "" && dangsudung != "")
            {
                dieukientinhtrangsudung = string.Format(" where (TinhTrangSuDung in ({0}) or {1})", dieukientinhtrangsudung, dangsudung);
            }
            else
            {
                if (dieukientinhtrangsudung != "")
                {
                    dieukientinhtrangsudung = string.Format(" where TinhTrangSuDung in ({0}) ", dieukientinhtrangsudung);
                }
                if (dangsudung != "")
                {
                    dieukientinhtrangsudung = string.Format(" where {0}", dangsudung);
                }

            }
            return dieukientinhtrangsudung;
        }

        private async Task searchAsync()
        {



            string dieukien = "";

            string dieukiensanpham = "";
            lstdata.Clear();
            
            string ghichutemp = "";
            //if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            //{
            //    toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn kho"));

            //    return;
            //}
            //ghichutemp += Environment.NewLine + "Nhà máy: Tất cả";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaSP))
            {
                dieukiensanpham = " where hh.MaHang in (SELECT [MaHang] FROM [NvlChiTietKhuVuc] where MaSP=@MaSP)";
                lstpara.Add(new ParameterDefine("@MaSP", nvlNhapXuatItemShowcrr.MaSP));
                ghichutemp += Environment.NewLine + "Sản phẩm: " + TenSP;

            }
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            {
                lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
                if (dieukien == "")
                    dieukien = " where MaKho=@MaKho";
                else
                    dieukien += " and MaKho=@MaKho";
                ghichutemp += Environment.NewLine + "Kho: " + TenKho;
            }
            else
            {
                lstpara.Add(new ParameterDefine("@MaKho", "K011"));//Không lấy kho đóng vỉ
                if (dieukien == "")
                    dieukien = " where MaKho<>@MaKho";
                else
                    dieukien += " and MaKho<>@MaKho";
                //ghichutemp += Environment.NewLine + "Kho: " + TenKho;
            }
            string dieukiennhomhang = "";
            if (!string.IsNullOrEmpty(NhomHang))
            {

                dieukiennhomhang = string.Format(" where PhanLoai=@MaNhom");
                lstpara.Add(new ParameterDefine("@MaNhom", NhomHang));
                ghichutemp += Environment.NewLine + string.Format("Nhóm: {0}", NhomHang);
            }
            string dieukienmahang = "";
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
            {
                dieukienmahang = " and MaHang=@MaHang";
                lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItemShowcrr.MaHang));
                ghichutemp += Environment.NewLine + string.Format("Mã hàng: {0}", nvlNhapXuatItemShowcrr.MaHang);
            }
           
            string sql = "";
            string dieukientinhtrangsudung = xulydieukientinhtrangsudung();
            sql = string.Format(@"Use NVLDB  select qry.*,gn.TenGN 
                from (SELECT qrytk.SerialCT,hh.MaHang,hh.DVT,hh.TenHang,nh.PhanLoai,hh.QuyCach,itemnx.SerialLink,qrytk.SoLuong as SLNhap,case when NgaySanXuat is not null then dbo.ConvertDateToWeeks(NgaySanXuat) else DauTuan end as DauTuan,itemnx.ViTri,itemnx.NgayInsert,itemnx.NgaySanXuat,itemnx.NgayHetHan,itemnx.TinhTrangSuDung,itemnx.DonGia

                      FROM 
					  (select Serial,MaHang,SerialLink,SoLuong,SerialCT from 
					   (select min(Serial) as Serial,min(case when SLNhap>0 then SerialCT else NULL end) as SerialCT,MaHang,sum(SLNhap-SLXuat) as SoLuong,SerialLink
					   from 
						dbo.NvlNhapXuatItem nvlitem 
						where SerialCT in (select Serial from NvlNhapXuat {0}) {3}
						group by MaHang,SerialLink) as qrykho where abs(SoLuong)>0.1) as qrytk
						inner join dbo.NvlNhapXuatItem itemnx on qrytk.Serial=itemnx.Serial
                      inner join dbo.NvlHangHoa hh on qrytk.MaHang=hh.MaHang  
					  inner join (select * from NvlNhomHang {1}) nh on hh.MaNhom=nh.MaNhom
                        {2}) as qry
                        inner join dbo.NvlNhapXuat nx on qry.SerialCT=nx.Serial
						inner join dbo.View_NoiGN gn on nx.MaGN=gn.MaGN
                        {4}
					  order by TenHang,SerialLink", dieukien, dieukiennhomhang, dieukiensanpham, dieukienmahang,dieukientinhtrangsudung);
            CallAPI callAPI = new CallAPI();
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    lstdata = JsonConvert.DeserializeObject<List<NvlNhapXuatItemShow>>(json);

                    if (lstdata.Any())
                    {
                        await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
                    }
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
        string dieukien = "", dieukienmahang = "";
        string ghichu = "";
        string ghichutemp = "";


    }

}
