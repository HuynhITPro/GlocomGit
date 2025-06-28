using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

using NFCWebBlazor.Model;

using System.ComponentModel;
using NFCWebBlazor.App_ClassDefine;
using System.Data;

using DevExpress.Blazor;

using Microsoft.JSInterop;



namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Urc_DonHang_AddKeHoach
    {
        [Inject] ToastService toastService { get; set; }
        [Inject] IJSRuntime jSRuntime { get; set; }
        public class DonHangItem : INotifyPropertyChanged
        {
            public int? Serial { get; set; }

            public int? SerialMaDH { get; set; }
            public string? MaPDOC { get; set; }
            public string? PhanLoai { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public double? SLSuDung { get; set; }
            public double? SLKeHoach { get; set; }
            public double? SLDatHang { get; set; }
            public double? SLThieu { get; set; }
            public double? SLTon { get; set; }
            public double? SLKiemKe { get; set; }
            public double? SLTheoDoi { get; set; }
            public double? SLDHChuaVe { get; set; }
            public double? SLConLai { get; set; }
            public double? ThanhTien { get; set; }

            public double? TyLe
            {
                get
                {
                    if (SLKeHoach == 0)
                        return 0;
                    return (double?)SLSuDung / SLKeHoach;
                }

            }
            public string DVT { get; set; }

            public double? DonGia { get; set; }

            public int? SerialLink { get; set; }
            public string GroupSP { get; set; }
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
            public DateTime? NgayDKNhapKho { get; set; }

            public DateTime? NgayEdit { get; set; }

            public string UserInsert { get; set; }

            public DateTime? NgayInsert { get; set; }

            public int? GiaTri { get; set; }

            public string DienGiai { get; set; }
            public string? Err { get; set; }
            public string DuyetItemMsg { get; set; }

            public int? STT { get; set; }

            public double? SLHuy { get; set; }
            public DonHangItem CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<DonHangItem>(json);
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }

        private async Task loadAsync()
        {



            string sql = "";
            if (string.IsNullOrEmpty(keHoachMuaHang_Showcrr.dieukiennhomhang))
            {


                sql = string.Format(@"
                                use NVLDB
                               declare @SerialDN int={0}
                                declare @DateEnd datetime='{1}'
                              
                                declare @stringlistmasp nvarchar(2000)
								declare @tblMaSP as Table(MaSP nvarchar(100))
								insert into @tblMaSP(MaSP)
								select MaSP from NvlKeHoachMuaHangItem where SerialDN=@SerialDN and MaSP is not null group by MaSP

								select
					                  @stringlistmasp=  STUFF((SELECT DISTINCT ';' + MaSP
                                                                FROM @tblMaSP tb
                                                                  FOR XML PATH ('')), 1, 1, '') 
																   from @tblMaSP a
                                 select ROW_NUMBER() OVER(ORDER BY hh.MaHang) AS Serial,hh.MaHang,hh.DVT,hh.TenHang,nh.PhanLoai,hh.MaPDOC,qry.SLKeHoach,cast (qrytk.SLTon as float) as SLTon,isnull(qry.SLDatHang,0) as SLSuDung,cast (0 as float) as SLKiemKe,isnull(SLDHChuaVe,0) as SLDHChuaVe,qry.SLKeHoach-isnull(qry.SLDatHang,0) as SLConLai,(qry.SLKeHoach-isnull(qry.SLDatHang,0))-isnull(qrytk.SLTon,0)-isnull(SLDHChuaVe,0) as SLThieu,qrysp.GroupSP
                                 from
                                (SELECT  [MaHang],sum([SoLuong]) as SLKeHoach,sum(SoLuong-SLTheoDoi-SLHuy) as SLDatHang
                                 FROM [NvlKeHoachMuaHangItem]
                                 Where SerialDN = @SerialDN
                                 group by MaHang) as qry
                                 left join (select MaHang,sum(SLNhap-SLXuat) as SLTon from NvlNhapXuatItem where SerialCT in (select Serial from NvlNhapXuat where Ngay<=@DateEnd) group by MaHang) qrytk on qry.MaHang=qrytk.MaHang
                                 inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                 inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                                 
                                 left join (select MaHang,sum(SLTheoDoi) as SLDHChuaVe from NvlDonDatHangItem group by MaHang) as qrydhcv on qry.MaHang=qrydhcv.MaHang
                                 left join dbo.MaHangSanPhamWithCondition(@stringlistmasp) as qrysp on qry.MaHang=qrysp.MaHang order by qrysp.GroupSP", keHoachMuaHang_Showcrr.Serial, dtpend.ToString("MM/dd/yyyy 23:59"));
            }
            else
            {
                sql = string.Format(@"
                                use NVLDB
                               declare @SerialDN int={0}
                                declare @DateEnd datetime='{1}'
                                declare @stringlistmasp nvarchar(2000)
								declare @tblMaSP as Table(MaSP nvarchar(100))
								insert into @tblMaSP(MaSP)
								select MaSP from NvlKeHoachMuaHangItem where SerialDN=@SerialDN and MaSP is not null group by MaSP

								select
					                  @stringlistmasp=  STUFF((SELECT DISTINCT ';' + MaSP
                                                                FROM @tblMaSP tb
                                                                  FOR XML PATH ('')), 1, 1, '') 
																   from @tblMaSP a
                                   select ROW_NUMBER() OVER(ORDER BY hh.MaHang) AS Serial,hh.MaHang,hh.DVT,hh.TenHang,nh.PhanLoai,hh.MaPDOC,qry.SLKeHoach,cast (qrytk.SLTon as float) as SLTon,isnull(qry.SLDatHang,0) as SLSuDung,cast (0 as float) as SLKiemKe,isnull(SLDHChuaVe,0) as SLDHChuaVe,qry.SLKeHoach-isnull(qry.SLDatHang,0) as SLConLai,(qry.SLKeHoach-isnull(qry.SLDatHang,0))-isnull(qrytk.SLTon,0)-isnull(SLDHChuaVe,0) as SLThieu,qrysp.GroupSP
                                 from
                 
                                  (SELECT  [MaHang],sum([SoLuong]) as SLKeHoach,sum(SoLuong-SLTheoDoi-SLHuy) as SLDatHang
                                 FROM [NvlKeHoachMuaHangItem]
                                 Where SerialDN = @SerialDN
                                 group by MaHang) as qry
                                 left join (select MaHang,sum(SLNhap-SLXuat) as SLTon from NvlNhapXuatItem where SerialCT in (select Serial from NvlNhapXuat where Ngay<=@DateEnd) group by MaHang) qrytk on qry.MaHang=qrytk.MaHang
                                 inner join (select * from dbo.NvlHangHoa where {2}) hh on qry.MaHang=hh.MaHang
                                 inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                                
                                 left join (select MaHang,sum(SLTheoDoi) as SLDHChuaVe from NvlDonDatHangItem group by MaHang) as qrydhcv on qry.MaHang=qrydhcv.MaHang
                                 left join dbo.MaHangSanPhamWithCondition(@stringlistmasp) as qrysp on qry.MaHang=qrysp.MaHang order by qrysp.GroupSP", keHoachMuaHang_Showcrr.Serial, dtpend.ToString("MM/dd/yyyy 23:59"), keHoachMuaHang_Showcrr.dieukiennhomhang);

            }
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
                            it.MaNCC = nVLDonDatHangShowcrr.MaNCC;
                            if (it.SLThieu < 0)
                                it.SLThieu = null;
                        }
                        lstdata = null;
                        lstdata = new List<DonHangItem>();
                        lstdata.AddRange(query);
                        if (lstdatadathang == null)
                            lstdatadathang = new List<DonHangItem>();
                        lstdatadathang.Clear();
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
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await loadAsync();

            }

            //return base.OnAfterRenderAsync(firstRender);
        }
        private async void searchAsync()
        {
            await loadAsync();
        }

        string[] arrcolumncheck = new string[] { "Serial", "Mã hàng", "Đặt hàng", "Mã NCC" };
        public async Task ImportExcelAsync()
        {
            if (lstdatadathang == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn những dòng cần đặt hàng trước"));
                return;
            }
            if (lstdatadathang.Count() == 0)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn những dòng cần đặt hàng trước"));
                return;
            }
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
            dxPopup.showAsync("Import hàng hóa từ excel");
            dxPopup.ShowAsync();
        }
        List<DonHangItem> lstimport = new List<DonHangItem>();
        private async void GetTable(DataTable dt)
        {


            ConvertDataTableToClassList(dt);
            //lstdata = dt;

            //string stt = "STT";
            await dxPopup.CloseAsync();
            dt.Dispose();
            StateHasChanged();

        }
         public  class GridColumnVisble
        {
            public GridColumnVisble(string _fieldname, bool _visible)
            {
                FieldName = _fieldname;
                Visible = _visible;
            }
            public string FieldName { get; set; }
            public bool Visible { get; set; }
        }
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
        private void showcolumndh(string type)
        {
            if (type == "Default")
            {
                GridDatHang.BeginUpdate();
                foreach (DxGridDataColumn dxGridDataColumn in GridDatHang.GetDataColumns())
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
                GridDatHang.EndUpdate();
                StateHasChanged();
                return;
            }
            if (type == "All")
            {
                GridDatHang.BeginUpdate();
                foreach (DxGridDataColumn dxGridDataColumn in GridDatHang.GetDataColumns())
                {
                    dxGridDataColumn.Visible = true;

                }
                GridDatHang.EndUpdate();
                StateHasChanged();
                return;
            }


        }
        private void ConvertDataTableToClassList(DataTable dt)
        {
            double d = 0;


            foreach (DataRow row in dt.Rows)
            {
                if (row["Serial"] != DBNull.Value)
                {

                    foreach (var it in lstdatadathang)
                    {

                        if (it.Serial.ToString() == row["Serial"].ToString())
                        {
                            if (row["Đặt hàng"] == DBNull.Value)
                                it.SLDatHang = null;
                            else
                            {
                                if (double.TryParse(row["Đặt hàng"].ToString(), out d))
                                {
                                    it.SLDatHang = d;
                                }
                                else
                                    it.SLDatHang = null;
                            }
                            if (row["Mã NCC"] == DBNull.Value)
                                it.MaNCC = null;
                            else
                            {
                                it.MaNCC = row["Mã NCC"].ToString();
                            }
                            break;
                        }
                    }
                }
                // DonHangItem donHangItem = new DonHangItem();

            }
        }

        private void addLstDonHang()
        {

            foreach (var it in SelectedDataItems.Cast<DonHangItem>())
            {
                it.SLDatHang = it.SLThieu;
                lstdatadathang.Add(it.CopyClass());
                lstdata.Remove(it);
            }
            Grid.Reload();
            GridDatHang.Reload();
        }
        private void RemoveLstDonHang()
        {
            foreach (var it in SelectedDataItemsDatHang.Cast<DonHangItem>())
            {

                lstdata.Add(it.CopyClass());
                lstdatadathang.Remove(it);
            }
            Grid.Reload();
            GridDatHang.Reload();
        }
        private void reset()
        {
            lstdata.Clear();
            lstdatadathang.Clear();
            StateHasChanged();
        }
        private void SelectedItemsChangedDonHang(IReadOnlyList<object> lstobj)
        {
            SelectedDataItemsDatHang = lstobj;
            if (SelectedDataItemsDatHang == null)
                return;
            if (SelectedDataItemsDatHang.Count > 1)
            {
                var query = SelectedDataItemsDatHang.Cast<DonHangItem>().Where(p => p.MaNCC != null).FirstOrDefault();
                if (query != null)
                    nhacungcapselected = lstnhacungcap.Where(p => p.Name == query.MaNCC).FirstOrDefault();
                toastTextInput.Show();
            }
            else
                toastTextInput.Close();
        }

        private void btxacnhanncc()
        {
            if (SelectedDataItemsDatHang != null)
            {
                string select = "";
                if (nhacungcapselected == null)
                    select = null;
                else
                    select = nhacungcapselected.Name;
                foreach (var it in SelectedDataItemsDatHang.Cast<DonHangItem>())
                {
                    it.MaNCC = select;

                }
                toastTextInput.Close();
                GridDatHang.SaveChangesAsync();
            }
        }

        DataTable dtsave;
        private async Task<bool> checklogicAsync()
        {
            if(nVLDonDatHangShowcrr.Serial==null)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Không có đơn hàng nào được chọn"));
                return false;
            }
            if(!await phanQuyenAccess.CreateDonDatHang(ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có quyền thêm đơn hàng"));
                return false;
            }
            foreach (var it in lstdatadathang)
            {
                it.Err = "";
            }
            List<DonHangItem> query = new List<DonHangItem>();

            var queryselect = lstdatadathang.Where(p => p.SLDatHang > 0).OrderBy(p => p.MaHang).ToList();//bắt buộc phải order by, vì phía dưới hàm tìm kiếm có sử dung6 tính năng này
            foreach (var it in queryselect)
            {
                query.Add(it.CopyClass());
            }
            var checkncc = query.Where(p => p.MaNCC == null || p.MaNCC == "");
            foreach (var it in lstdatadathang)
            {
                foreach (var itncc in checkncc)
                {
                    if (it.Serial == itncc.Serial)
                    {
                        it.Err = "Nhà cung cấp không được để trống";
                        break;
                    }
                }
            }
            if (checkncc.Any())
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng  kiểm tra lại những dòng bị tô màu đỏ"));
                return false;
            }
            if (query.Count == 0)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng nhập số lượng đặt hàng"));
                return false;
            }
            CallAPI callAPI = new CallAPI();
            string sql = string.Format(@"use NVLDB declare @Serial int={0}
                declare @Type_NvlDonDatHangItem Type_NvlDonDatHangItem

                insert into @Type_NvlDonDatHangItem(Serial,SerialLink,MaHang,SLDatHang,SLTheoDoi,DVT)
                select Serial,SerialDN,MaHang,SoLuong,SLTheoDoi,'' from NvlKeHoachMuaHangItem where SerialDN=@Serial
                select * from @Type_NvlDonDatHangItem order by MaHang", keHoachMuaHang_Showcrr.Serial);
            List<ParameterDefine> lstparadt = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparadt);
            if (json == "")
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi load tablesave"));
                return false;
            }
            DataTable dttemp = new DataTable();
            dttemp = JsonConvert.DeserializeObject<DataTable>(json);
            if (dtsave == null)
            {
                dtsave = new DataTable();
                foreach (DataColumn cl in dttemp.Columns)
                {
                    dtsave.Columns.Add(cl.ColumnName, cl.DataType);
                }
            }
            dtsave.Clear();
            bool checkfind = false;
            int indexbegin = 0;
            int count = dttemp.Rows.Count;
            string keygroup = "";
            string MaNCCold = "";
            //Tạo group để xử lý tách dòng
            //2 bảng đều đã sắp xếp theo thứ tự mã hàng rồi
            foreach (var it in query)
            {
                checkfind = false;
                keygroup = string.Format("{0}_{1}", nVLDonDatHangShowcrr.Serial, prs.RandomString(10));
                MaNCCold = it.MaNCC;
                //Bảng phía dưới đã có mã hàng sắp theo thứ tự
                for (int i = indexbegin; i < count; i++)
                {
                    DataRow row = dttemp.Rows[i];
                    if (row.Field<double>("SLTheoDoi") <= 0)
                        continue;
                    if (it.SLDatHang <= 0)
                        break;
                    if (it.MaHang == row.Field<string>("MaHang"))
                    {
                        DataRow rownew = dtsave.NewRow();
                        if (!checkfind)//Gán lần đâu tiên khi mới gặp
                        {
                            checkfind = true;
                            indexbegin = i;
                        }
                        if (it.SLDatHang >= row.Field<double>("SLTheoDoi"))
                        {
                           
                            it.SLDatHang = it.SLDatHang - row.Field<double>("SLTheoDoi");
                            rownew["Serial"] = row["Serial"];//Lấy Serial của bảng này, vì nó là Serial của NvlKeHoachMuaHangItem
                            rownew["MaHang"] = it.MaHang;
                            rownew["SerialMaDH"] = nVLDonDatHangShowcrr.Serial;
                            rownew["SLDatHang"] = row["SLTheoDoi"];
                            rownew["SLTheoDoi"] = row["SLTheoDoi"];
                           
                            rownew["MaNCC"] = it.MaNCC;
                            if(it.MaNCC!=MaNCCold)
                            {
                                keygroup= string.Format("{0}_{1}", nVLDonDatHangShowcrr.Serial, prs.RandomString(10));
                                MaNCCold= it.MaNCC;
                            }
                            rownew["Group"] = keygroup;

                            rownew["DVT"] = "";
                            rownew["SerialLink"] = row["SerialLink"];
                            dtsave.Rows.Add(rownew);
                            row["SLTheoDoi"] = 0;
                        }
                        else
                        {
                            row["SLTheoDoi"] = row.Field<double>("SLTheoDoi") - it.SLDatHang;
                            rownew["Serial"] = row["Serial"];//Lấy Serial của bảng này, vì nó là Serial của NvlKeHoachMuaHangItem
                            rownew["MaHang"] = it.MaHang;
                            rownew["SLDatHang"] = it.SLDatHang;
                            rownew["SerialMaDH"] = nVLDonDatHangShowcrr.Serial;
                            rownew["SLTheoDoi"] = it.SLDatHang;
                            rownew["MaNCC"] = it.MaNCC;
                            if (it.MaNCC != MaNCCold)
                            {
                                keygroup = string.Format("{0}_{1}", nVLDonDatHangShowcrr.Serial, prs.RandomString(10));
                                MaNCCold = it.MaNCC;
                            }
                            rownew["Group"] = keygroup;
                            rownew["DVT"] = "";
                            rownew["SerialLink"] = row["SerialLink"];
                            dtsave.Rows.Add(rownew);
                            it.SLDatHang = 0;
                        }
                    }
                    else
                    {
                        if (checkfind)
                        {
                            break;
                        }
                    }


                }
                //Nếu mã hàng không có
                if (!checkfind)
                {
                    it.Err = "Mã hàng này không tồn tại trong kế hoạch";
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng  kiểm tra lại những dòng bị tô màu đỏ"));
                    return false;
                }
            }
            var querychecksoluongvuot = query.Where(p => p.SLDatHang > 0);
            if(querychecksoluongvuot.Any())
            {
                foreach (var item in querychecksoluongvuot)
                {
                    foreach (var it in lstdatadathang)
                    {
                        if(item.Serial==it.Serial)
                        {
                            it.Err = "Mã hàng này đã vượt quá số lượng kế hoạch rồi";
                        }
                    }
                }
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng  kiểm tra lại những dòng bị tô màu đỏ"));
                return false;
            }
            query.Clear();

            dttemp.Dispose();
            //Xử lý lưu
           // await prs.exportexcelAsync(jSRuntime, dtsave, 1, 1, "Export excel");
            //Kiểm tra mã trùng
            return true;
        }
        class KetquaResult
        {
            public int? Serial { get; set; }
            public string? MaHang { get; set; }
            public string? ketqua { get; set; }

            public string? ketquaexception { get; set; }

        }
        private async void save()
        {
            
            if (!await checklogicAsync())
            {
                GridDatHang.Reload();
                return;
            }
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlDonDatHangItem_InsertTableKeyGroup";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Type_NvlDonDatHangItem", prs.ConvertDataTableToJson(dtsave), "DataTable"));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@SerialMaDH", nVLDonDatHangShowcrr.Serial));
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<KetquaResult>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        lstdatadathang.Clear();
                        GridDatHang.Reload();
                        toastService.Notify(new(ToastType.Success,"Lưu thành công"));
                    }
                    else
                    {
                        if (query[0].Serial!=null)
                        {
                            foreach(var it in query)
                            {
                                foreach(var item in lstdatadathang)
                                {
                                    if(it.MaHang==item.MaHang)
                                    {
                                       item.Err=it.ketqua;
                                        return;
                                    }
                                }
                            }
                            GridDatHang.Reload();
                        }
                        else
                            toastService.Notify(new(ToastType.Danger, $"Lỗi:{query[0].ketqua}, {query[0].ketquaexception} "));

                    }
                    
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message} không lưu được"));
            }
        }

    }
}

