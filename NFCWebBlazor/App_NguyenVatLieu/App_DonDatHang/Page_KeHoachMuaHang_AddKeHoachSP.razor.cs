using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucNVLMaster;
namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Page_KeHoachMuaHang_AddKeHoachSP
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        public  class KeHoachThang
        {
            public int Serial { get; set; }
            public string TenKHThang { get; set; }
            public DateTime ThangMin { get; set; }
            public DateTime ThangMax { get; set; }
            public string LoaiKeHoach { get; set; }

        }
        List<KeHoachThang> lstkhthang = new List<KeHoachThang>();
        KeHoachThang KeHoachThangsearch=new KeHoachThang();
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
                parameterDefineList.Add(new ParameterDefine("@LoaiKehoach", LoaiKeHoach));
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<List<KeHoachThang>>(json);
                    lstkhthang = query;
                    foreach(var it in query)
                    {
                        DataDropDownList ddl = new DataDropDownList();
                        ddl.Name = it.Serial.ToString();
                        ddl.FullName= it.TenKHThang;
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
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (CheckQuyen)
                {

                }
                view_KeHoachMuaHangSP_Detail.keHoachMuaHang_Showcrr = keHoachMuaHang_Showcrr;
            }
            return base.OnAfterRenderAsync(firstRender);
        }
        public class KeHoachThang_Show
        {

            public bool isChanged { get; set; } = false;
            public bool Chk { get; set; } = false;
            public int Serial { get; set; }
            public string KeyGroup { get; set; }
            public string MaSP { get; set; }
            public string TenSP { get; set; }
            public string? ArticleNumber { get; set; }

            public int SLSP { get; set; }
            public int SLThucHien { get; set; }
            public int SLConLai { get; set; }
            public int SLPhaiDat { get; set; }
            public double TyLe
            {
                get
                {
                    if (SLSP == 0)
                        return 0;
                    return ((SLThucHien / (double)SLSP));
                }
            }
            public string TenMau { get; set; }
            public string MaMau { get; set; }
            public string LoaiKeHoach { get; set; }
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
            public ObservableCollection<KeHoachThangItem> lstitem { get; set; }
            public List<DinhMucVatTuShow> lstdinhmuc { get; set; }
            public List<KeHoachDinhMucCongDoan> lstkehoachcongdoan { get; set; }

            public string GhiChu { get; set; }
            public string Err { get; set; }
            public string Colorhex
            { get; set; }
        }
        public class KeHoachThangItem : INotifyPropertyChanged
        {


            public int Serial { get; set; }

            public string MaHang
            {
                get; set;
            }

            public string TenHang
            {
                get; set;
            }
            public string Err { get; set; }

            public string KhuVuc
            {
                get; set;
            }
            public string DVT { get; set; }

            public string? ChatLuong
            {
                get; set;
            }
            public double SLQuyDoi
            {
                get; set;
            }
            public double SLKHCan { get; set; }
            public double SLTonKho { get; set; }
            public double TonMB { get; set; }
            public double? DonHangChuaVe { get; set; }
            public double? SLCan { get; set; }
            public double? SLDatThem { get; set; }
            public string MaNCC { get; set; }
            public KeHoachThangItem CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<KeHoachThangItem>(json);
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
        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<KeHoachThang_Show> lstkehoachthang { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<DinhMucVatTuShow> lstdinhmuc { get; set; }
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

                            select khtitem.Serial,sp.TenSP,khtitem.MaSP,khtitem.ArticleNumber,mm.MaMau,khtitem.SLSP,isnull(nvlsp.SoLuongSP,0) as SLThucHien,khtitem.SLSP-isnull(nvlsp.SoLuongSP,0) as SLConLai,khtitem.SLSP-isnull(nvlsp.SoLuongSP,0) as SLPhaiDat,mm.TenMau,mm.Color
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
                checkall = true;
                Grid.AutoFitColumnWidths();

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


        private async Task ImportTuFileAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(keHoachMuaHang_Showcrr.UserInsert, ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không phải người tạo, nên không có quyền thêm"));
                return;
            }


            //renderFragment = builder =>
            //{
            //    builder.OpenComponent<Urc_KeHoachThang_Import>(0);
            //    builder.AddAttribute(1, "keHoachSP_Showcrr", keHoachSP_Showcrr);

            //    //builder.AddAttribute(4, "GotoMainForm", EventCallback.Factory.Create(this, RefreshRowCurrent));
            //    //builder.OpenComponent(0, componentType);
            //    builder.CloseComponent();
            //};
            await dxPopup.showAsync("THÊM SẢN PHẨM");

            await dxPopup.ShowAsync();
        }
        DataTable dtsave;
        private async Task<bool> checklogicAsync()
        {
            if (lstdata == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                return false;
            }

            if (lstdata.Count == 0)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                return false;
            }
            if (dtsave == null)
            {
                CallAPI callAPI = new CallAPI();
                string sql = @"use NVLDB declare @dt Type_NvlKeHoachSP
                insert into @dt(STT)
                values(1)
                select * from @dt";
                List<ParameterDefine> lstparadt = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparadt);
                if (json == "")
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi load tablesave"));
                    return false;
                }
                dtsave = JsonConvert.DeserializeObject<DataTable>(json);
            }
            dtsave.Clear();

            try
            {
                int i = 0;
                var query = lstdata.Where(p => p.isChanged.Equals(true) && p.SLPhaiDat > 0).ToList();
                if (query.Count == 0)
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn ít nhất 1 mã hàng"));
                    return false;
                }
                foreach (var nvlkhmhitem in query)
                {


                    DataRow rownew = dtsave.NewRow();
                    rownew["STT"] = i;
                    rownew["SerialKHThangItem"] = nvlkhmhitem.Serial;

                    rownew["MaSP"] = nvlkhmhitem.MaSP;
                    if (nvlkhmhitem.ArticleNumber == null)
                        rownew["ArticleNumber"] = DBNull.Value;
                    else
                        rownew["ArticleNumber"] = nvlkhmhitem.ArticleNumber;
                    rownew["SoLuongSP"] = nvlkhmhitem.SLPhaiDat;

                    rownew["KhuVuc"] = LoaiKeHoach;
                    dtsave.Rows.Add(rownew);
                    i++;
                }
                query.Clear();

            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                //Console.WriteLine("Lỗi: " + ex.Message);
                return false;
            }
            if (dtsave.Rows.Count == 0)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                //Console.WriteLine("Lỗi: " + ex.Message);
                return false;
            }
            return true;
        }
        private void reset()
        {
            lstdata.Clear();
            StateHasChanged();
        }
        class KetquaResult
        {
            public int? Serial { get; set; }
            public double? SLCL { get; set; }

            public string? ketqua { get; set; }

            public string? ketquaexception { get; set; }

        }
        private async Task saveAsync()
        {
            if (await checklogicAsync())
            {
                CallAPI callAPI = new CallAPI();

                string sql = "NVLDB.dbo.NvlKeHoachSP_InsertTable";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialDN", keHoachMuaHang_Showcrr.Serial));//Trong procedure đã xử lý
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@Type_NvlKeHoachSP", prs.ConvertDataTableToJson(dtsave), "DataTable"));
                try
                {
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<KetquaResult>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            //msgBox.Show("Lưu thành công", IconMsg.iconssuccess);
                            toastService.Notify(new(ToastType.Success, $"Lưu thành công."));
                            reset();
                        }
                        else
                        {


                            if (query[0].Serial != null)
                            {
                                foreach (var it in query)
                                {
                                    foreach (var row in lstdata)
                                    {

                                        if (it.Serial == row.Serial)
                                        {
                                            row.Err = it.ketqua;
                                            //break;
                                        }
                                    }

                                }
                            }
                            toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng kiểm tra lại những dòng tô màu đỏ"));
                            //grvSanPham.Columns["Err"].Visible = true;

                            if (query[0].ketquaexception != null)
                            {
                                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketquaexception));
                            }
                            Grid.SaveChangesAsync();

                        }

                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, "Đã xảy ra lỗi."));
                    }


                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex));
                }
                finally
                {

                }

            }
            return;
        }
        private async void setLoaddata()
        {
            var query = lstdata.Where(p => p.Chk.Equals(true) && p.SLPhaiDat > 0).GroupBy(p => new { MaSP = p.MaSP, MaMau = p.MaMau})
                .Select(p => new { MaSP = p.Key.MaSP, MaMau = p.Key.MaMau, SLSP = p.Sum(n => n.SLSP), SLPhaiDat = p.Sum(n => n.SLPhaiDat), SLDatHang = p.Sum(n=>n.SLPhaiDat)
                 }).ToList();
            if(query.Count > 0)
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
                    dataRow["MaMau"] =it.MaMau;
                    dataRow["Ngay"] = KeHoachThangsearch.ThangMin; 
                    dtload.Rows.Add(dataRow);
                    KeHoachThang_Show keHoachThang_Show = new KeHoachThang_Show();
                    keHoachThang_Show.MaSP=it.MaSP;
                   
                    keHoachThang_Show.SLSP=it.SLSP;
                    keHoachThang_Show.KeyGroup= string.Format("{0}_{1}", keHoachMuaHang_Showcrr.Serial, prs.RandomString(10));
                    keHoachThang_Show.SLPhaiDat=it.SLPhaiDat;
                    keHoachThang_Show.MaMau=it.MaMau;
                    keHoachThang_Show.LoaiKeHoach = KeHoachThangsearch.LoaiKeHoach;
                    view_KeHoachMuaHangSP_Detail.lstkehoachthangselect.Add(keHoachThang_Show);
                }
                dtload.Rows[dtload.Rows.Count - 1]["Ngay"] = KeHoachThangsearch.ThangMax;

                var queryitem = lstdata.Where(p => p.Chk.Equals(true) && p.SLPhaiDat > 0).ToList();
                foreach(var item in queryitem)
                {
                    var querykey = view_KeHoachMuaHangSP_Detail.lstkehoachthangselect
                        .Where(p => p.MaSP == item.MaSP && p.MaMau == item.MaMau).FirstOrDefault();
                    item.KeyGroup = querykey.KeyGroup;
                    view_KeHoachMuaHangSP_Detail.lstkehoachthangsaveitem.Add(item);
                   
                }    
                await view_KeHoachMuaHangSP_Detail.loaddataAsync(dtload, KeHoachThangsearch.ThangMin, KeHoachThangsearch.ThangMax, "ChiTiet");
               
                tabactive = 1;
                StateHasChanged();
            }
            
        }
        private void checkChanged(bool chk, KeHoachThang_Show keHoachThang_Show)
        {
            keHoachThang_Show.Chk = chk;
        }
        private void selecteditemnhomkehoach(DataDropDownList dataDropDownList)
        {
            //if (dataDropDownList == null)
            //    return;
            //var query = lstkehoach.Where(p => p.TypeName == dataDropDownList.Name).ToList();
            //lstkehoachselected.Clear();
            //kehoachselected = null;
            //lstkehoachselected.AddRange(query);
            //StateHasChanged();

        }
        List<DinhMucVatTuShow> lstdinhmucall = new List<DinhMucVatTuShow>();
      

    }

}
