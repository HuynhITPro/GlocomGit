using BlazorBootstrap;
using DevExpress.Blazor;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System;
using System.Data;
using System.Reflection;
using static NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Page_DonDatHang_Master;
using static NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Urc_DonHang_AddKeHoach;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class View_KeHoachChuaDatHang_Group
    {
        [Inject] ToastService toastService { get; set; }
        List<GridColumnVisble> lstcolumnvisible = new List<GridColumnVisble>();

        private void showcolumn(string type)
        {
            if (type == "Default")
            {
                Grid.BeginUpdate();
                foreach (DxGridDataColumn dxGridDataColumn in Grid.GetDataColumns())
                {
                    foreach (var it in lstcolumnvisible)
                    {
                        if (dxGridDataColumn.FieldName == it.FieldName)
                        {
                            dxGridDataColumn.Visible = it.Visible;
                            break;
                        }
                    }

                }
                Grid.EndUpdate();
                StateHasChanged();
                return;
            }
            if (type == "All")
            {
                Grid.BeginUpdate();
                foreach (DxGridDataColumn dxGridDataColumn in Grid.GetDataColumns())
                {
                    dxGridDataColumn.Visible = true;

                }
                Grid.EndUpdate();
                StateHasChanged();
                return;
            }


        }
        private async Task loadAsync()
        {
            string sql = "";
            if (keHoachMuaHang_Showcrr.lstdathang == null)
            {
                sql = string.Format(@"
                                 use NVLDB
                                 declare @SerialDN int={0}
                                  declare @DateEnd datetime=getdate()
                                  declare @stringlistmasp nvarchar(2000)
		                                declare @tblMaSP as Table(MaSP nvarchar(100))
		                                insert into @tblMaSP(MaSP)
		                                select MaSP from NvlKeHoachMuaHangItem where SerialDN=@SerialDN and MaSP is not null group by MaSP

		                                select
                                        @stringlistmasp=  STUFF((SELECT DISTINCT ';' + MaSP
                                                                  FROM @tblMaSP tb
                                                                    FOR XML PATH ('')), 1, 1, '') 
													                                   from @tblMaSP a
   
                                   select ROW_NUMBER() OVER(ORDER BY hh.MaHang) AS Serial,hh.MaHang,hh.DVT,hh.TenHang,nh.PhanLoai,hh.MaPDOC,qry.SLKeHoach,cast (qrytk.SLTon as float) as SLTon,isnull(qry.SLDatHang,0) as SLDatHang,cast (0 as float) as SLKiemKe,isnull(SLDHChuaVe,0) as SLDHChuaVe,isnull(qry.SLDatHang,0) as SLConLai,(qry.SLKeHoach-isnull(qry.SLDatHang,0))-isnull(qrytk.SLTon,0)-isnull(SLDHChuaVe,0) as SLThieu
                                   ,qrytkgia.MaNCC,qrytkgia.TenNCC,case when qry.DonGia>1 then qry.DonGia else qrytkgia.DonGia end as DonGia
                                   from
                                  (SELECT  [MaHang],sum([SoLuong]) as SLKeHoach,sum(SoLuong-SLTheoDoi-SLHuy) as SLConLai,sum(SLTheoDoi) as SLDatHang,min(DonGia) as DonGia
                                   FROM [NvlKeHoachMuaHangItem]
                                   Where SerialDN = @SerialDN and SLTheoDoi>0
                                   group by MaHang) as qry
                                   left join (select MaHang,SLTon from dbo.GetTonKhoExWithDateEnd(@DateEnd)) qrytk on qry.MaHang=qrytk.MaHang
                                   inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                   inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                                   left join (select MaHang,sum(SLTheoDoi) as SLDHChuaVe from NvlDonDatHangItem group by MaHang) as qrydhcv on qry.MaHang=qrydhcv.MaHang
                                   left join dbo.GetThamKhaoGiaNCC() as qrytkgia on qry.MaHang=qrytkgia.MaHang order by nh.PhanLoai,qrytkgia.TenNCC
                    ", keHoachMuaHang_Showcrr.Serial);

                try
                {
                    PanelVisible = true;

                    CallAPI callAPI = new CallAPI();
                    List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                    //parameterDefineList.Add(new ParameterDefine("@Serial", keHoachSP_Showcrr.Serial));
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<DonHangItem>>(json);
                        if (query.Count > 0)
                        {
                            foreach (var it in query)
                            {
                                if (it.SLThieu < 0)
                                    it.SLThieu = null;
                            }
                            lstdata = null;
                            lstdata = new List<DonHangItem>();
                            lstdata.AddRange(query);
                            keHoachMuaHang_Showcrr.lstdathang = lstdata;
                            query.Clear();
                            foreach (DxGridDataColumn dxGridDataColumn in Grid.GetDataColumns())
                            {
                                lstcolumnvisible.Add(new GridColumnVisble(dxGridDataColumn.FieldName, dxGridDataColumn.Visible));
                            }
                            //Grid.ExpandGroupRow(0);
                            Grid.Reload();
                            query.Clear();
                            //Grid.AutoFitColumnWidths();
                            //PanelVisible = false;

                            //Grid.AutoFitColumnWidths();
                        }
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
            else
            {
                lstdata = keHoachMuaHang_Showcrr.lstdathang;
                Grid.Reload();
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await loadAsync();

            }

            //return base.OnAfterRenderAsync(firstRender);
        }
        List<DataDropDownList> lstncc = new List<DataDropDownList>();
        protected override async void OnInitialized()
        {
            lstncc = await ModelData.Getlstnhacungcap();
            //base.OnInitialized();
        }
        private void DonGiaChanged(double? d, DonHangItem nvlKeHoachMuaHangItemShow)
        {
            nvlKeHoachMuaHangItemShow.DonGia = d;
            nvlKeHoachMuaHangItemShow.ThanhTien = nvlKeHoachMuaHangItemShow.SLDatHang * d;

        }

        private async void AddDonHang(DataDropDownList dataDropDownList)
        {
            dxFlyoutchucnang.CloseAsync();
            savedonhang(dataDropDownList.Name, 0);//Tạo mới đơn hàng
            //Grid.Reload();
        }
        private void AdddDonHangCoSan()
        {
            if (string.IsNullOrEmpty(SerialDHcrr))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn đơn hàng"));
                return;
            }
            var query = lstdonhang.Where(p => p.Name == SerialDHcrr).FirstOrDefault();
            if (query == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn đơn hàng"));
                return;
            }
            dxFlyoutchucnangadditem.CloseAsync();
            string MaNCC = query.ValueTag.ToString();
            savedonhang(MaNCC, int.Parse(SerialDHcrr));//Tạo mới đơn hàng

        }

        DataTable dtsave;
        private async Task<bool> checklogicAsync(List<DonHangItem> donHangItems)
        {
            bool bl = true;
            if (!donHangItems.Any())
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Nhà cung cấp này không có trong danh sách phía dưới"));
                return false;
            }
            foreach (var it in donHangItems)
            {
                it.Err = "";
                if (it.SLDatHang == null)
                {
                    it.Err = "Vui lòng nhập số lượng đặt hàng";
                }
                if (it.SLDatHang <= 0)
                {
                    it.Err = "Số lượng đặt hàng phải lớn hơn 0";
                }
                if (it.DonGia == null || it.DonGia == 0)
                {
                    if (it.Err != "")
                        it.Err += ", Đơn hàng phải có đơn giá";
                    else
                        it.Err = "Đơn hàng phải có đơn giá";
                }
                if (string.IsNullOrEmpty(it.MaNCC))
                {
                    if (it.Err != "")
                        it.Err += ", Nhập thiếu nhà cung cấp";
                    else
                        it.Err = "Nhập thiếu nhà cung cấp";
                }
                if (it.Err != "")
                    bl = false;
            }
            if (!bl)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, IconName.ExclamationCircle.ToString(), "Vui lòng kiểm tra lại những dòng tô màu đỏ"));
                return false;
            }
            if (dtsave == null)
            {
                CallAPI callAPI = new CallAPI();
                string sql = @"use NVLDB declare @dt Type_NvlDonDatHangItem
                   
                insert into @dt(Serial,SLDatHang,SLTheoDoi,DVT,DonGia)
                values(1,0.1,0.1,'',0.1)
                select * from @dt";
                List<ParameterDefine> lstparadt = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparadt);
                if (json == "")
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi load tablesave"));
                    return false;
                }
                dtsave = JsonConvert.DeserializeObject<DataTable>(json);
                //foreach(DataColumn cl in dtsave.Columns)
                //{
                //    Console.WriteLine(string.Format("{0} - {1}",cl.ColumnName,cl.DataType));
                //}
                dtsave.Clear();
            }
            else
                dtsave.Clear();
            return true;
        }
        List<NvlKeHoachMuaHangItemShow> lstkehoachmuahangitem = new List<NvlKeHoachMuaHangItemShow>();
        private async Task<bool> xulyKehoachmuahangAsync(List<DonHangItem> donHangItems, string MaNCC, int SerialDH)
        {
            string sql = string.Format(@"use NVLDB declare @SerialDN int={0} 
                            SELECT  [Serial],[SerialDN],[MaHang],[SLTheoDoi]
                            FROM [dbo].[NvlKeHoachMuaHangItem]
                            where SerialDN=@SerialDN", keHoachMuaHang_Showcrr.Serial);
            CallAPI callAPI = new CallAPI();

            string json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<NvlKeHoachMuaHangItemShow>>(json);
                lstkehoachmuahangitem.Clear();
                if (query.Any())
                {
                    lstkehoachmuahangitem.AddRange(query);
                    dtsave.Clear();
                    string keygroup = "";
                    //Xử lý kiểm tra, và rã ra chi tiết
                    foreach (var it in donHangItems)
                    {
                        it.SLConLai = it.SLDatHang;//Sử dụng số lượng CL để khấu trừ dần
                        if (it.SLConLai > 0)
                        {
                            var querymahang = lstkehoachmuahangitem.Where(p => p.MaHang == it.MaHang);
                            if (querymahang != null)
                            {
                                double? sltheodoi = 0;
                                foreach (var item in querymahang)
                                {
                                    DataRow row = dtsave.NewRow();
                                    keygroup = string.Format("{0}", prs.RandomString(10));
                                    row["SerialMaDH"] = SerialDH;//Đã xử lý trong procedure
                                    row["MaHang"] = it.MaHang;
                                    if (it.SLConLai > item.SLTheoDoi)
                                    {
                                        sltheodoi = item.SLTheoDoi;
                                        it.SLConLai = it.SLConLai - sltheodoi;
                                        item.SLTheoDoi = 0;
                                    }
                                    else
                                    {
                                        sltheodoi = it.SLConLai;
                                        item.SLTheoDoi = item.SLTheoDoi - sltheodoi;
                                        it.SLConLai = 0;

                                    }
                                    row["SLDatHang"] = sltheodoi;
                                    row["SLTheoDoi"] = sltheodoi;
                                    row["DVT"] = it.DVT;
                                    row["DonGia"] = it.DonGia;
                                    row["SerialLink"] = item.SerialDN;
                                    row["Serial"] = item.Serial;
                                    row["MaNCC"] = MaNCC;
                                    row["NgayDKNhapKho"] = DBNull.Value;
                                    row["UserInsert"] = Model.ModelAdmin.users.UsersName;
                                    row["Group"] = keygroup;
                                    dtsave.Rows.Add(row);

                                }
                            }
                        }
                        if (it.SLConLai > 0.01)//Vẫn >0 có nghĩa là vượt quá số lượng cho phép
                        {
                            it.Err = "Mã hàng này đặt hàng vượt quá số lượng là " + it.SLConLai;
                            toastService.Notify(new ToastMessage(ToastType.Warning, "Kiểm tra lại những dòng tô màu đỏ"));
                            StateHasChanged();
                            return false;
                        }
                    }
                }
                else
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Kế hoạch này không còn dữ liệu"));
                    return false;
                }
            }
            else
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                return false;
            }
            return true;
        }

        private async Task btadddonhangAsync()
        {
            dxFlyoutchucnangadditem.CloseAsync();
            NVLDonDatHangShow nVLDonDatHangShow = new NVLDonDatHangShow();
            nVLDonDatHangShow.Serial = 0;
            nVLDonDatHangShow.NgayDatHang = DateTime.Now;
            nVLDonDatHangShow.DVTT = "VNĐ";
            //LoaiKeHoach = "DeNghiMuaHang";

            nVLDonDatHangShow.LoaiDonHang = "DeNghiMuaHang";
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DonDatHang_AddMaster>(0);
                builder.AddAttribute(1, "nVLDonDatHangcrr", nVLDonDatHangShow);
                builder.AddAttribute(2, "CallBackAfterSave", EventCallback.Factory.Create<int>(this, SearchDeNghi));
                builder.CloseComponent();
            };

            dxPopup.showAsync("TẠO ĐƠN HÀNG");
            dxPopup.ShowAsync();
        }

        private void dxPopupClosing(PopupClosingEventArgs e)
        {

            if (CallBackAfterSave.HasDelegate)
                Grid.Reload();
        }

        private async void SearchDeNghi(int Serial)
        {
            await dxPopup.CloseAsync();
            await loaddonhangchuaduyetAsync();
        }
        private async void savedonhang(string MaNCC, int SerialDH)
        {
            var querydonhang = lstdata.Where(p => p.SLDatHang > 0 && p.MaNCC == MaNCC && p.SLDatHang > 0).ToList();

            if (!await checklogicAsync(querydonhang))
                return;
            if (!await xulyKehoachmuahangAsync(querydonhang, MaNCC, SerialDH))
                return;

            string sql = "NVLDB.dbo.NvlDonDatHangItem_InsertTableKeyGroup_Ver2";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            NvlDonDatHang donHangItem = new NvlDonDatHang();
            lstpara.Add(new ParameterDefine("@Type_NvlDonDatHangItem", prs.ConvertDataTableToJson(dtsave), "DataTable"));

            lstpara.Add(new ParameterDefine("@SerialMaDH", SerialDH));
            lstpara.Add(new ParameterDefine("@NgayTao", donHangItem.NgayTao));
            lstpara.Add(new ParameterDefine("@KhuVuc", donHangItem.KhuVuc));
            lstpara.Add(new ParameterDefine("@MaNCC", MaNCC));
            lstpara.Add(new ParameterDefine("@PhongBan", donHangItem.PhongBan));
            lstpara.Add(new ParameterDefine("@DVTT", donHangItem.Dvtt));
            lstpara.Add(new ParameterDefine("@NgayDatHang", donHangItem.NgayDatHang));
            lstpara.Add(new ParameterDefine("@NgayMax", donHangItem.NgayMax));
            lstpara.Add(new ParameterDefine("@GhiChu", keHoachMuaHang_Showcrr.NoiDung));
            lstpara.Add(new ParameterDefine("@LoaiDonHang", donHangItem.LoaiDonHang));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));


            try
            {
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<KetquaResult>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        //msgBox.Show("Lưu thành công", IconMsg.iconssuccess);
                        toastService.Notify(new(ToastType.Success, $"Lưu thành công."));
                        lstdata.RemoveAll(p => p.MaNCC == MaNCC);
                        lstnhacungcap.RemoveAll(p => p.Name == MaNCC);
                        //Grid.Reload();
                        //dxGridncc.Reload();
                        //reset();
                    }
                    else
                    {
                        string err = "";

                        if (query[0].Serial != null)
                        {
                            foreach (var it in query)
                            {
                                foreach (var row in lstdata)
                                {
                                    if (it.Serial.Value == row.Serial)
                                    {
                                        row.Err = it.ketqua;
                                        break;
                                    }
                                }
                            }
                            toastService.Notify(new ToastMessage(ToastType.Warning, err));
                            //grvSanPham.Columns["Err"].Visible = true;
                        }
                        else
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, query[0].ketqua));
                        }
                        if (query[0].ketquaexception != null)
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketquaexception));
                        }
                    }

                }
                else
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Đã xảy ra lỗi."));
                }


            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                dtsave.Clear();
            }



        }
        public async void ShowFlyout()
        {
            try
            {
                if (!dxFlyoutchucnang.IsInitialized)
                    await dxFlyoutchucnang.InitializedTask;
                await dxFlyoutchucnang.CloseAsync();

                idflychucnang = "#" + idelement(keHoachMuaHang_Showcrr.Serial);



                if (!dxFlyoutchucnang.IsInitialized)
                {
                    Console.WriteLine("Init lại cái fly");
                    await dxFlyoutchucnang.InitializedTask;
                }

                lstnhacungcap.Clear();
                var query = lstdata.Where(p => p.SLDatHang > 0).ToList();
                var querygroupncc = query.GroupBy(p => new { MaNCC = p.MaNCC, TenNCC = p.TenNCC }).Select(p => new DataDropDownList { Name = p.Key.MaNCC, FullName = p.Key.TenNCC }).OrderBy(p => p.FullName).ToList();
                lstnhacungcap.AddRange(querygroupncc);
                // dxGridncc.Reload();
                // await  JSRuntime.InvokeVoidAsync(CallNameJson.toggleCollapse.ToString(), string.Format("#{0}", randomdivhide));
                await dxFlyoutchucnang.ShowAsync();

            }
            catch (Exception ex)
            {
                //ToastService.Notify(new ToastMessage(ToastType.Danger,"Lỗi: " + ex.Message));
                Console.Error.WriteLine("Lỗi ở flyout:" + ex.Message);
            }

        }
        public string random { get; set; }
        public string idelement(int? serial)
        {
            if (serial == null)
                string.Format("flychucnang_{0}_{1}", random, serial);
            if (random == null)
            {
                random = prs.RandomString(9);
            }
            return string.Format("flychucnang_{0}_{1}", random, serial);
        }
        public string idelementtonkho(string mahang)
        {
            if (mahang == null)
                string.Format("flychucnangshowtkg_{0}_{1}", random, mahang);
            if (random == null)
            {
                random = prs.RandomString(9);
            }
            return string.Format("flychucnangshowtkg_{0}_{1}", random, mahang);
        }
        public string idelementadditem(int? serial)
        {
            if (serial == null)
                string.Format("flychucnangadditem_{0}_{1}", random, serial);
            if (random == null)
            {
                random = prs.RandomString(9);
            }
            return string.Format("flychucnangadditem_{0}_{1}", random, serial);
        }
        private async Task loaddonhangchuaduyetAsync()
        {
            string sql = string.Format(@"use NVLDB
                declare @UserInsert nvarchar(100)=N'{0}'
    
                SELECT  dh.[Serial],dh.[Serial] as [Name],cast (dh.Serial as varchar(10))+' - '+ncc.TenNCC as FullName,dh.MaNCC as ValueTag
     
          FROM [dbo].[NvlDonDatHang] dh
		  inner join dbo.NvlNhaCungCap ncc on dh.MaNCC=ncc.MaNCC
          where  dh.UserInsert=@UserInsert
          and dh.Serial not in (select SerialLinkMaster from NvlKyDuyetItem where TableName=N'NvlDonDatHang' and LoaiDuyet=N'Duyệt')
           "
        , ModelAdmin.users.UsersName);
            CallAPI callAPI = new CallAPI();

            string json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                lstdonhang.Clear();
                if (query.Any())
                {
                    lstdonhang.AddRange(query);
                    StateHasChanged();
                    // Grid.AutoFitColumnWidths();
                }

            }
        }

        string[] arrcolumncheck = new string[] { "Mã hàng", "SL đặt hàng", "Đơn giá", "Mã NCC" };
        public async Task ImportExcelAsync()
        {
            //CallAPI
            renderFragment = builder =>
            {
                builder.OpenComponent<ButtonImportExcel>(0);

                builder.AddAttribute(1, "arrcolumncheck", arrcolumncheck);
                builder.AddAttribute(2, "getdatatble", EventCallback.Factory.Create<DataTable>(this, GetTable));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
            await dxPopup.showAsync("Import hàng hóa từ excel");
            await dxPopup.ShowAsync();
        }
        private async void GetTable(DataTable dt)
        {
            //lstcolumn.Clear();

            XuLyDataImport(dt);
            //lstdata = dt;
            await dxPopup.CloseAsync();
            dt.Dispose();
            Grid.Reload();

        }
        List<DonHangItem> lstimport = new List<DonHangItem>();
        private void XuLyDataImport(DataTable dt)
        {
            double d = 0;
            lstimport.Clear();
            foreach (DataRow row in dt.Rows)
            {
                DonHangItem nvlHangHoaShow = new DonHangItem();
                nvlHangHoaShow.MaHang = (row["Mã hàng"] != DBNull.Value) ? row["Mã hàng"].ToString() : "";
                nvlHangHoaShow.MaNCC = (row["Mã NCC"] != DBNull.Value) ? row["Mã NCC"].ToString() : "";
                //nvlHangHoaShow.TenHang = (row["TenHang"] != DBNull.Value) ? row["TenHang"].ToString() : "";
                if (row["SL đặt hàng"] == DBNull.Value)
                {
                    nvlHangHoaShow.SLDatHang = 0;
                    nvlHangHoaShow.Err = "Vui lòng nhập số lượng";
                }
                else
                {
                    if (!double.TryParse(row["SL đặt hàng"].ToString().Trim(), out d))
                    {
                        nvlHangHoaShow.SLDatHang = null;
                        nvlHangHoaShow.Err = "SL đặt hàng phải là số";
                    }
                    else
                        nvlHangHoaShow.SLDatHang = d;
                }
                if (row["Đơn giá"] == DBNull.Value)
                {
                    nvlHangHoaShow.DonGia = null;

                }
                else
                {
                    if (!double.TryParse(row["Đơn giá"].ToString().Trim(), out d))
                    {
                        nvlHangHoaShow.DonGia = null;
                        nvlHangHoaShow.Err = "Đơn giá phải là số";
                    }
                    else
                        nvlHangHoaShow.DonGia = d;
                }
                if (string.IsNullOrEmpty(nvlHangHoaShow.Err))
                {
                    lstimport.Add(nvlHangHoaShow);
                }
            }
            foreach (var it in lstimport)
            {
                foreach (var item in lstdata)
                {
                    if (it.MaHang == item.MaHang)
                    {
                        item.MaNCC = it.MaNCC;
                        item.SLDatHang = it.SLDatHang;
                        break;
                    }
                }
            }
            StateHasChanged();
        }
        private void showviewfull()
        {


            //LoaiKeHoach = "DeNghiMuaHang";


            //renderFragment = builder =>
            //{
            //    builder.OpenComponent<View_KeHoachChuaDatHang_Group>(0);
            //    builder.AddAttribute(1, "keHoachMuaHang_Showcrr", keHoachMuaHang_Showcrr);
            //   // builder.AddAttribute(2, "CallBackAfterSave", EventCallback.Factory.Create<int>(this, SearchDeNghi));
            //    builder.CloseComponent();
            //};
            MenuItem menuItem = new MenuItem();
            menuItem.TextItem = "Thêm chi tiết vào đơn hàng";
            menuItem.NameItem = "View_KeHoachChuaDatHang_Group";
            menuItem.ComponentName = "NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.View_KeHoachChuaDatHang_Group";
            menuItem.IconCssClass = "bi bi-cart3";
            if (ModelAdmin.mainLayout != null)
            {
                RenderFragment renderFragment1;
                renderFragment1 = builder =>
                {

                    builder.OpenComponent<View_KeHoachChuaDatHang_Group>(0);
                    builder.AddAttribute(1, "keHoachMuaHang_Showcrr", keHoachMuaHang_Showcrr);

                    builder.CloseComponent();
                };
                ModelAdmin.mainLayout.AddDirectRenderfagment(menuItem, renderFragment1);

            }

            //dxPopup.showAsync("Kế hoạch chưa đặt hàng");
            //dxPopup.ShowAsync();
        }
        private async void btShowDonHang()
        {
            try
            {
                if (!dxFlyoutchucnangadditem.IsInitialized)
                    await dxFlyoutchucnangadditem.InitializedTask;
                await dxFlyoutchucnangadditem.CloseAsync();
                idflychucnangadditem = "#" + idelementadditem(keHoachMuaHang_Showcrr.Serial);
                if (!dxFlyoutchucnangadditem.IsInitialized)
                {
                    Console.WriteLine("Init lại cái fly");
                    await dxFlyoutchucnang.InitializedTask;
                }
                lstdonhang.Clear();
                await loaddonhangchuaduyetAsync();
                // await  JSRuntime.InvokeVoidAsync(CallNameJson.toggleCollapse.ToString(), string.Format("#{0}", randomdivhide));

                await dxFlyoutchucnangadditem.ShowAsync();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                //ToastService.Notify(new ToastMessage(ToastType.Danger,"Lỗi: " + ex.Message));
                Console.Error.WriteLine("Lỗi ở flyout:" + ex.Message);
            }
            // await loaddonhangchuaduyetAsync();
        }
        private async void ShowFlyTonKho(DonHangItem donHangItem)
        {
            try
            {
                if (!dxFlyoutchucnangshowtkg.IsInitialized)
                    await dxFlyoutchucnangshowtkg.InitializedTask;
                await dxFlyoutchucnangshowtkg.CloseAsync();
                idflychucnangshowtkg = "#" + idelementtonkho(donHangItem.MaHang);
                if (!dxFlyoutchucnangshowtkg.IsInitialized)
                {
                    Console.WriteLine("Init lại cái fly");
                    await dxFlyoutchucnangshowtkg.InitializedTask;
                }
                lstdataitemshowtkg.Clear();
                // await  JSRuntime.InvokeVoidAsync(CallNameJson.toggleCollapse.ToString(), string.Format("#{0}", randomdivhide));
                await loadtonkhohAsync(donHangItem);
                await dxFlyoutchucnangshowtkg.ShowAsync();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                //ToastService.Notify(new ToastMessage(ToastType.Danger,"Lỗi: " + ex.Message));
                Console.Error.WriteLine("Lỗi ở flyout:" + ex.Message);
            }
            // await loaddonhangchuaduyetAsync();
        }
        private async Task loadtonkhohAsync(DonHangItem nvlKeHoachMuaHangItemShow)
        {
            lstdataitemshowtkg.Clear();

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = " ";
            string sqlSearch = "";
            //string[] arrshow = new string[] { "MaHang", "TenHang", "SerialCT", "SLNhap", "SLXuat", "DVT", "LyDo", "TenSP", "MaKien", "DauTuan", "SoLo", "TenGN", "SoXe", "GhiChu", "DonGia", "NgayHetHan", "SerialLink", "KhachHang_XuatXu", "UserInsert", "SerialKHDH", "SerialDN", "NguoiDeNghi", "TenLienKet", "NhaMay", "NgayInsert" };
            bool checkshow = false;

            sqlSearch = string.Format(@"use NVLDB

                               declare @MaHang nvarchar(100)=N'{0}'
                                declare @Serial int
                                declare @datebegin datetime=dateadd(yy,-2,getdate())
                                declare @SerialCTmin int
                                select top 1 @Serial=Serial,@SerialCTmin=SerialCT from NvlNhapXuatItem where NgayInsert>=@datebegin ORDER BY Serial ASC
                                declare @tblchungtu table(Serial int,MaGN nvarchar(100),Ngay datetime)
                                insert into @tblchungtu(Serial,MaGN,Ngay)
                                select max(Serial) as Serial,MaGN,max(NgayInsert) as Ngay from NvlNhapXuat where  Serial>@SerialCTmin and STTCT>0 and Serial in (select SerialCT from NvlNhapXuatItem where Serial>@Serial and MaHang=@MaHang)
                                group by MaGN
                                SELECT SerialCT as SerialLink,[MaHang],DonGia,nx.Ngay,TenNCC
                                FROM [dbo].[NvlNhapXuatItem] it
                                inner join (select * from @tblchungtu) 
                                nx on it.SerialCT=nx.Serial
                                left join dbo.NvlNhaCungCap ncc on nx.MaGN=ncc.MaNCC
                                where DonGia>0 and SLNhap>0 and MaHang=@MaHang
                                order by nx.Serial desc

                     ", nvlKeHoachMuaHangItemShow.MaHang);
            CallAPI callAPI = new CallAPI();
            try
            {

                string json = await callAPI.ExcuteQueryEncryptAsync(sqlSearch, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemTemTK>>(json);

                    if (query.Any())
                    {


                        //var queryit = query.Where(p => p.MaHang == nvlKeHoachMuaHangItemShow.MaHang).ToList();
                        lstdataitemshowtkg.AddRange(query);
                        StateHasChanged();
                        //lstdata = keHoachMuaHangcrr.lsttemtonkho;
                    }
                    // await GotoMainForm.InvokeAsync();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



        }
        class KetquaResult
        {
            public int? Serial { get; set; }


            public string? ketqua { get; set; }

            public string? ketquaexception { get; set; }

        }
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();

    }
}
