using BlazorBootstrap;
using DevExpress.Data.Filtering;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;

using NFCWebBlazor.Model;
using NFCWebBlazor.Pages;


namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_KhoThanhPhamShow
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

       public class Dieukiensearch
        {
            public DateTime? dtpdatebegin { get; set; } = DateTime.Now;
            public DateTime? dtpdateend { get; set; } = DateTime.Now;
        }

        public class KeHoachThangItem_Show
        {
            public KeHoachThangItem_Show()
            {

            }

            public string ArticleNumber { get; set; }
            public bool chk { get; set; } = false;
            public string MaSP { get; set; }
            public string TenSP { get; set; }


            public Nullable<double> SLNhap { get; set; }
            public double? ThanhTien { get; set; }
            public string ColorHex { get; set; }
            private uint? _color { get; set; }
            public uint? Color
            {
                get { return _color; }
                set
                {
                    _color = value;
                    ColorHex = StaticClass.UIntToHtmlColor(_color);
                }
            }

            public string TenMau { get; set; }
            public string MaMau { get; set; }
            public string Type_Other { get; set; }
            public string UserInsert { get; set; }
            public string Err { get; set; }
            public string GhiChu { get; set; }

            public KeHoachThangItem_Show CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<KeHoachThangItem_Show>(json);
            }
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (CheckQuyen)
                {

                }
            }
            return base.OnAfterRenderAsync(firstRender);
        }
        private async Task searchAsync()
        {
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = "";

            if (dieukiensearch.dtpdatebegin == null || dieukiensearch.dtpdateend == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ngày để xem"));
                return;
            }


            if (dieukien == "")
                dieukien = string.Format(" where Ngay>='{0}' and Ngay<='{1}' and LyDo='TPNM' ", dieukiensearch.dtpdatebegin.Value.ToString("MM/dd/yyyy 00:00"), dieukiensearch.dtpdateend.Value.ToString("MM/dd/yyyy 23:59"));
            else
                dieukien += string.Format(" and Ngay>='{0}' and Ngay<='{1}'  and LyDo='TPNM'", dieukiensearch.dtpdatebegin.Value.ToString("MM/dd/yyyy 00:00"), dieukiensearch.dtpdateend.Value.ToString("MM/dd/yyyy 23:59"));

            //lstpara.Add(new ParameterDefine("@DateBegin", datebegin.ToString("MM/dd/yyyy 00:00")));
            //lstpara.Add(new ParameterDefine("@DateEnd", dateend.ToString("MM/dd/yyyy 23:59")));

            string para = string.Format(@"select art.ArticleNumber,qry.MaSP,sp.TenSP,qry.SLNhap,qry.ThanhTien,mm.TenMau,mm.Color,LyDo,art.Type_Other from
                        (SELECT [MaSP],[ArticleNumberID],sum([SLNhap]) as SLNhap,sum(SLNhap*[Gia]) as ThanhTien,LyDo
                          FROM [dbo].[KhoTP_NK]
                          {0}
                          group by  [MaSP],[ArticleNumberID],LyDo)  as qry inner join dbo.ArticleNumberProduct art
                          on qry.[ArticleNumberID]=art.ArticleNumber
                          inner join dbo.MaMau mm on art.MaMau=mm.MaMau
                          inner join dbo.SanPham sp on qry.MaSP=sp.MaSP", dieukien);

            string sql = string.Format(@"use NVLDB 
                        exec SP.DataBase_ScansiaPacific2014.dbo.getTableformSqlString @SQL_QUERY=@sql
                        ", dieukien);
            lstpara.Add(new ParameterDefine("@sql", para));
            try
            {
                if (lstdata == null)
                    lstdata = new List<KeHoachThangItem_Show>();
                else
                    lstdata.Clear();
                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<List<KeHoachThangItem_Show>>(json);
                    lstdata.AddRange(query);
                }
                Grid.Reload();
                PanelVisible = false;
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));

                PanelVisible = false;
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
                //ShowProgress.CloseAwait();
            }
        }






    }
}
