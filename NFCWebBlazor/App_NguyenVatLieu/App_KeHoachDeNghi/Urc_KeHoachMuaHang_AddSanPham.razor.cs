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


namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Urc_KeHoachMuaHang_AddSanPham
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        private async Task loaddatadropdownAsync()
        {
            try
            {
           
                CallAPI callAPI = new CallAPI();
                string sql = string.Format(@"use NVLDB
                declare @DateBegin date=dateAdd(MM,-4,getdate())
                declare @DateEnd date=dateAdd(MM,1,getdate())
                SELECT Serial as [Name],[TenKHThang] as FullName,LoaiKeHoach as TypeName
                  FROM [KeHoachThang]
                  where ThangMax>=@DateBegin and ThangMax<=@DateEnd and LoaiKehoach=@LoaiKehoach");

                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                parameterDefineList.Add(new ParameterDefine("@LoaiKehoach", LoaiKeHoach));
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                     
                    lstkehoach.AddRange(query);
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
            }
            return base.OnAfterRenderAsync(firstRender);
        }
        public class KeHoachThang_Show
        {

            public bool isChanged { get; set; } = false;
            public bool Chk { get; set; } = false;
            public int Serial { get; set; }

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
                    if(query!=null)
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
          await  dxPopup.showAsync("THÊM SẢN PHẨM");

          await  dxPopup.ShowAsync();
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
        //private async Task loaddinhmucAsync()
        //{
        //    string sql = string.Format(@"use NVLDB             
        //                    IF OBJECT_ID('tempdb..##tmpdinhmuctoancuc') IS NOT NULL
        //                     DROP TABLE ##tmpdinhmuctoancuc
        //                    exec GetDinhMucNVL_SanPhamList @lstsanpham=@lstsanpham
        //                    select * from ##tmpdinhmuctoancuc
        //                    where GroupMauSP=@GroupMauSP
        //                    Drop table ##tmpdinhmuctoancuc");
        //    string dieukien = "";
        //    PanelVisible = true;
        //    lstdinhmucall.Clear();
        //    sql = sql + dieukien;
        //    try
        //    {

        //        CallAPI callAPI = new CallAPI();
        //        List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
        //        parameterDefineList.Add(new ParameterDefine("@lstsanpham", keHoachThang_Showcrr.MaSP));
        //        parameterDefineList.Add(new ParameterDefine("@GroupMauSP", keHoachThang_Showcrr.MaMau));

        //        string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
        //        if (json != "")
        //        {
        //            var query = JsonConvert.DeserializeObject<List<DinhMucVatTuShow>>(json);
        //            if (query.Count > 0)
        //            {
        //                var querycheckncc = query.Where(p => p.ChonNCC == 1).FirstOrDefault();

        //                lstdata.AddRange(query);
        //                var querygr = lstdata.GroupBy(p => new { GroupNCC = p.GroupNhaCungCap, GroupMauSP = p.GroupMauSP }).Select(p => new { GroupNCC = p.Key.GroupNCC, GroupMauSP = p.Key.GroupMauSP }).ToList();
        //                if (querygr.Count > 0)
        //                {

        //                    foreach (var item in querygr)
        //                    {
        //                        KeHoachSelected keHoachSelected = new KeHoachSelected();
        //                        keHoachSelected.GroupNhaCungCap = item.GroupNCC;
        //                        keHoachSelected.MauSP = item.GroupMauSP;
        //                        lstKeHoachSelected.Add(keHoachSelected);
        //                    }
        //                    if (lstKeHoachSelected.Count == 1)
        //                    {
        //                        lstKeHoachSelected[0].SLDeNghi = keHoachThang_Showcrr.SLPhaiDat;
        //                    }

        //                }
        //                query.Clear();

        //                //keHoachThang_Showcrr.lstitem = lstdata;
        //                //Grid.ExpandGroupRow(0);
        //                dxGrid.Reload();
        //                PanelVisible = false;

        //                //Grid.AutoFitColumnWidths();
        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        PanelVisible = false;
        //        toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));

        //    }
        //    finally
        //    {
        //        PanelVisible = false;
        //        StateHasChanged();
        //    }
        //}

    }
}

