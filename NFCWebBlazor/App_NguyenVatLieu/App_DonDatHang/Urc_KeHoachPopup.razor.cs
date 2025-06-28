using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Page_KeHoachMuaHang_AddKeHoachSP;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucNVLMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Urc_KeHoachPopup
    {
        [Inject] ToastService toastService { get; set; }
        DataDropDownList? kehoachselected { get; set; }
        KeHoachThang KeHoachThangsearch = new KeHoachThang();
        List<KeHoachThang> lstkhthang = new List<KeHoachThang>();
        List<DinhMucVatTuShow> lstdinhmucall = new List<DinhMucVatTuShow>();
        protected override void OnInitialized()
        {
            loaddatadropdownAsync();
            base.OnInitialized();
        }
        private async Task loaddatadropdownAsync()
        {
            try
            {


                CallAPI callAPI = new CallAPI();
                string sql = string.Format(@"use NVLDB
                declare @DateBegin date=dateAdd(MM,-4,getdate())
                declare @DateEnd date=dateAdd(MM,7,getdate())
                SELECT Serial,[TenKHThang],LoaiKeHoach ,ThangMin,ThangMax
                  FROM [KeHoachThang]
                  where ThangMax>=@DateBegin and ThangMax<=@DateEnd");

                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
             
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<List<KeHoachThang>>(json);
                    lstkhthang = query;
                    foreach (var it in query)
                    {
                        DataDropDownList ddl = new DataDropDownList();
                        ddl.Name = it.Serial.ToString();
                        ddl.FullName = it.TenKHThang;
                        ddl.TypeName = it.LoaiKeHoach;
                        lstkehoach.Add(ddl);
                    }
                    ///lstkehoach.AddRange(query);
                    //lstkehoachselected = lstkehoach.ToList();
                }

            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }
        }
        private async Task searchAsync()
        {
            if (kehoachselected == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng nhập đầy đủ thông tin"));
                return;
            }

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = "";



            lstpara.Add(new ParameterDefine("@Serial", kehoachselected.Name));

            KeHoachThangsearch = lstkhthang.Where(p => p.Serial.ToString() == kehoachselected.Name).FirstOrDefault();
            //lstpara.Add(new ParameterDefine("@DateBegin", datebegin.ToString("MM/dd/yyyy 00:00")));
            //lstpara.Add(new ParameterDefine("@DateEnd", dateend.ToString("MM/dd/yyyy 23:59")));
            string sql = string.Format(@"use NVLDB  

                            declare @tbl Table(Serial int,Serial_KHThang int,MaSP nvarchar(100),ArticleNumber nvarchar(100),SLSP int,SLTheoDoi int)
                            insert into @tbl(Serial,Serial_KHThang,MaSP,ArticleNumber,SLSP,SLTheoDoi)
                            select Serial,Serial_KHThang,MaSP,ArticleNumber,SLSP,SLTheoDoi
                            from KeHoachThangItem where Serial_KHThang=@Serial

                            select khtitem.Serial,sp.TenSP,khtitem.MaSP,khtitem.ArticleNumber,mm.MaMau,khtitem.SLSP,isnull(nvlsp.SoLuongSP,0) as SLThucHien,khtitem.SLSP-isnull(nvlsp.SoLuongSP,0) as SLConLai,khtitem.SLSP as SLPhaiDat,mm.TenMau,mm.Color
                                                    from @tbl khtitem 
                                                    left join (select SerialKHThangItem,sum(SoLuongSP) as SoLuongSP from dbo.NvlKeHoachSP group by SerialKHThangItem) nvlsp  on khtitem.Serial=nvlsp.SerialKHThangItem
                                                    left join [SP].[DataBase_ScansiaPacific2014].[dbo].[ArticleNumberProduct] art
                                                    on khtitem.ArticleNumber=art.ArticleNumber
                                                    left join [SP].[DataBase_ScansiaPacific2014].dbo.MaMau mm on art.MaMau=mm.MaMau
                                                    inner join [SP].[DataBase_ScansiaPacific2014].[dbo].SanPham sp on khtitem.MaSP=sp.MaSP order by sp.TenSP
                            declare @lstsanpham nvarchar(max)
                             SELECT @lstsanpham = STUFF((
                                 SELECT ';' + CAST(MaSP AS NVARCHAR)
                                 FROM (select MaSP from @tbl group by MaSP) as qry
                                 FOR XML PATH('')
                             ), 1, 1, '')

                            IF OBJECT_ID('tempdb..##tmpdinhmuctoancuc') IS NOT NULL
	                            DROP TABLE ##tmpdinhmuctoancuc
                            exec GetDinhMucNVL_SanPhamList @lstsanpham=@lstsanpham
                            select * from ##tmpdinhmuctoancuc
                            
                            Drop table ##tmpdinhmuctoancuc", dieukien);

            try
            {
                if (lstdata == null)
                    lstdata = new List<KeHoachThang_Show>();
                else
                    lstdata.Clear();
                lstdinhmucall.Clear();
                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<CustomRoot>(json);
                    if (query != null)
                    {
                        lstdata.AddRange(query.lstkehoachthang);
                        lstdinhmucall.AddRange(query.lstdinhmuc);
                    }
                    //var query = JsonConvert.DeserializeObject<List<KeHoachThang_Show>>(json);
                    //lstdata.AddRange(query);
                }
               

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
        private void XacNhan()
        {
            setLoaddata();
            view_KeHoachMuaHangSP_Detail.closepopupAsync();
        }
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        private async void setLoaddata()
        {
            var query = lstdata.Where(p => p.SLPhaiDat > 0).GroupBy(p => new { MaSP = p.MaSP, MaMau = p.MaMau })
                .Select(p => new {
                    MaSP = p.Key.MaSP,
                    MaMau = p.Key.MaMau,
                    SLSP = p.Sum(n => n.SLSP),
                    SLPhaiDat = p.Sum(n => n.SLPhaiDat),
                    SLDatHang = p.Sum(n => n.SLPhaiDat)
                }).ToList();
            if (query.Count > 0)
            {

                DataTable dtload = new DataTable(); //Khai báo cho khớp cột vs Type_KeHoachTonKho
                dtload.Columns.Add("MaSP", typeof(string));
                dtload.Columns.Add("SLSP", typeof(double));
                dtload.Columns.Add("MaMau", typeof(string));
                dtload.Columns.Add("Ngay", typeof(DateTime));
                view_KeHoachMuaHangSP_Detail.lstkehoachthangselect.Clear();
                view_KeHoachMuaHangSP_Detail.lstkehoachthangsaveitem.Clear();
                foreach (var it in query)
                {
                    DataRow dataRow = dtload.NewRow();
                    dataRow["MaSP"] = it.MaSP;
                    dataRow["SLSP"] = it.SLPhaiDat;
                    dataRow["MaMau"] = it.MaMau;
                    dataRow["Ngay"] = KeHoachThangsearch.ThangMin;
                    dtload.Rows.Add(dataRow);
                    KeHoachThang_Show keHoachThang_Show = new KeHoachThang_Show();
                    keHoachThang_Show.MaSP = it.MaSP;

                    keHoachThang_Show.SLSP = it.SLSP;
                    keHoachThang_Show.KeyGroup = string.Format("{0}_{1}", 100, prs.RandomString(10));
                    keHoachThang_Show.SLPhaiDat = it.SLPhaiDat;
                    keHoachThang_Show.MaMau = it.MaMau;
                    keHoachThang_Show.LoaiKeHoach = KeHoachThangsearch.LoaiKeHoach;
                    view_KeHoachMuaHangSP_Detail.lstkehoachthangselect.Add(keHoachThang_Show);
                }
                dtload.Rows[dtload.Rows.Count - 1]["Ngay"] = KeHoachThangsearch.ThangMax;

                var queryitem = lstdata.Where(p => p.Chk.Equals(true) && p.SLPhaiDat > 0).ToList();
                foreach (var item in queryitem)
                {
                    var querykey = view_KeHoachMuaHangSP_Detail.lstkehoachthangselect
                        .Where(p => p.MaSP == item.MaSP && p.MaMau == item.MaMau).FirstOrDefault();
                    item.KeyGroup = querykey.KeyGroup;
                    view_KeHoachMuaHangSP_Detail.lstkehoachthangsaveitem.Add(item);

                }
                await view_KeHoachMuaHangSP_Detail.loaddataAsync(dtload, KeHoachThangsearch.ThangMin, KeHoachThangsearch.ThangMax, "ChiTiet");

               
                StateHasChanged();
            }

        }
    }
}
