using BlazorBootstrap;
using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucNVLMaster;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_DeNghiTheoDinhMuc;


namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Page_DeNghiTheoDinhMuc_New
    {
        [Inject] ToastService toastService { get; set; } = default!;
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        App_ClassDefine.ClassProcess prs = new ClassProcess();


        bool CheckQuyen = false;

        protected override async Task OnInitializedAsync()
        {

            try
            {
                await loadAsync();
                CheckQuyen = await phanQuyenAccess.CreateNhapXuatKho(Model.ModelAdmin.users);
                var dimension = await browserService.GetDimensions();
                Initcolumnsresult();
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
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi browserService :" + ex.Message));
            }

        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (boolExpand == false)
            {
                if (dxGrid.GetVisibleRowCount() > 0)
                {
                    loadExpand();
                    boolExpand = true;
                }
            }
            return base.OnAfterRenderAsync(firstRender);
        }
        private void KhachHang_SelectedItemChanged(DataDropDownList e)
        {
            if (e != null)
            {
                lstsanphamfilter = null;
                lstsanphamfilter = lstsanpham.Where(p => p.KhachHang == e.Name).ToList();

            }
        }

        private async Task loadAsync()
        {
            try
            {

                //var queryngn = await Model.ModelData.Getlstnoigiaonhan();
                //lstnoigiaonhan = queryngn.Where(p => p.TypeName == "NhaCungCap").ToList();
                lstsanpham = await ModelData.GetSanPham();
                lstKhachHang = lstsanpham.Where(p => !string.IsNullOrEmpty(p.KhachHang)).GroupBy(p => p.KhachHang).Select(p => new DataDropDownList { Name = p.Key, FullName = p.Key }).ToList();

            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
            }
            //baocaoselected = lstloaibaocao.FirstOrDefault(p => p.Name.Equals("Tồn kho theo sản phẩm"));


        }

        bool checkxoancc = false;
        public class KeHoachDinhMucShow
        {
            public int Serial { get; set; }
            public string SoKH { get; set; }
            public string MaSP { get; set; }
            public string MaHang { get; set; }
            public string MaMauKH { get; set; }
            public int SoLuongSP { get; set; }
            public string KhuVuc { get; set; }
            public string CongDoan { get; set; }
            public double SLKH { get; set; }
            public double SLTheoDoi { get; set; }
            public double SLQuyDoi { get; set; }
        }
        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<DinhMucVatTuShow> lstdinhmuc { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<KeHoachDinhMucShow> lstkhdm { get; set; }


        }
        CustomRoot customRoot { get; set; } = new CustomRoot();

        bool boolExpand = false;
        private async Task SanPham_SelectedItemChanged(SanPhamDropDown e)
        {

            if (e != null)
            {
                isWait = true;
                try
                {
                    boolExpand = false;
                    string sqlkehoach = "";
                    dxGrid.ClearFilter();
                    if (checkkehoachall)
                    {
                        sqlkehoach = string.Format(@"exec SP.DataBase_ScansiaPacific2014.dbo.KeHoachSP_NVL @MaSP=@MaSP,@ShowAll=1", e.MaSP);
                    }
                    else
                    {
                        sqlkehoach = string.Format(@"exec SP.DataBase_ScansiaPacific2014.dbo.KeHoachSP_NVL @MaSP=@MaSP,@ShowAll=0", e.MaSP);
                    }

                    dinhMucVatTuShowcrr.MaSP = e.MaSP;
                    CallAPI callAPI = new CallAPI();
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    lstpara.Add(new ParameterDefine("@MaSP", dinhMucVatTuShowcrr.MaSP));
                    string json = await callAPI.ExcuteQueryEncryptAsync(sqlkehoach, lstpara);

                    checkxoancc = false;
                    if (json != "")
                    {

                        lstkehoach = JsonConvert.DeserializeObject<List<KeHoachSuDung>>(json);


                    }

                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
                }
                finally
                {
                    //dxGrid.Reload();
                    isWait = false;
                    StateHasChanged();
                }
            }
        }
        private void loadExpand()
        {

            int n = dxGrid.GetGroupCount();
            dxGrid.ExpandAllGroupRows();
            dxGrid.BeginUpdate();
            for (var i = dxGrid.GetVisibleRowCount() - 1; i >= 0; i--)
            {

                if (dxGrid.IsGroupRow(i))
                {

                    int rowlevel = dxGrid.GetRowLevel(i);
                    if (rowlevel < n - 1)
                        dxGrid.ExpandGroupRow(i);
                    else
                        dxGrid.CollapseGroupRow(i);

                }

            }

            //Kiểm tra nếu là group GroupNhaCungCap

            dxGrid.EndUpdate();


        }
        private async Task<List<KeHoachLoadUsed>>? LoadkehoachdasudungAsync(string idkehoach)
        {
            string sql = @"Use NVLDB
           
            SELECT [IDKeHoach],[TenDinhMuc],[CongDoan],sum([SoLuong]) as SoLuong   
              FROM [dbo].[NvlKeHoachMuaHang_DinhMuc] where IDKeHoach=@IDKeHoach
              group by [IDKeHoach],[TenDinhMuc],[CongDoan]";
            CallAPI callAPI = new CallAPI();
            List<KeHoachLoadUsed>? lst = new List<KeHoachLoadUsed>();
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@IDKeHoach", idkehoach));
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<KeHoachLoadUsed>>(json);
                lst = query;

            }
            return lst;
        }
        private void Initcolumnsresult()
        {
            dtresult.Columns.Add("chk", typeof(bool));
            dtresult.Columns["chk"].DefaultValue = false;

            dtresult.Columns.Add("STT", typeof(string));
            dtresult.Columns.Add("MaHang", typeof(string));
            dtresult.Columns.Add("TenHang", typeof(string));
            dtresult.Columns.Add("TenSP", typeof(string));
            dtresult.Columns.Add("MaSP", typeof(string));
            dtresult.Columns.Add("TenDinhMuc", typeof(string));
            dtresult.Columns.Add("KhuVuc", typeof(string));
            dtresult.Columns.Add("CongDoan", typeof(string));
            dtresult.Columns.Add("DVT", typeof(string));
            dtresult.Columns.Add("SLQuyDoi", typeof(double));
            dtresult.Columns.Add("TongKH", typeof(double));
            dtresult.Columns.Add("TongDN", typeof(double));
            dtresult.Columns.Add("TongConLai", typeof(double));
            dtresult.Columns.Add("SLTon", typeof(double));
            dtresult.Columns.Add("TenMau", typeof(string));
            dtresult.Columns.Add("Colorhex", typeof(string));
            dtresult.Columns.Add("SLDeNghi", typeof(double));
            dtresult.Columns.Add("Err", typeof(string));

        }
        class itemnhapxuat
        {
            public string value;
            public string text;
            public itemnhapxuat(string _value, string _text)
            {
                value = _value;
                text = _text;

            }
            public itemnhapxuat()
            {

            }
        }
        RenderFragment renderFragmentcolumn;
        private RenderFragment CreateColumns(List<itemnhapxuat> lstheader)
        {
           
            return builder =>
            {
                int sequence = 0;
                // Tạo DxGridBandColumn
                foreach (itemnhapxuat header in lstheader)
                {
                    builder.OpenComponent<DevExpress.Blazor.DxGridDataColumn>(0);
                    builder.AddAttribute(sequence++, "FieldName", header.value);
                    builder.AddAttribute(sequence++, "Width", string.Format("{0}px", "150"));
                    builder.AddAttribute(sequence++, "Caption", header.text);

                    builder.AddAttribute(sequence++, "CellDisplayTemplate", (RenderFragment<GridDataColumnCellDisplayTemplateContext>)(context => builder2 =>
                    {

                        var dataRowView = (DataRowView)context.DataItem;
                        string mahang = "";
                        double d = dataRowView[header.value] == DBNull.Value ? 0 : dataRowView.Row.Field<double>(header.value);
                        bool valuecheck = (bool)dataRowView.Row.Field<bool>("chk");
                        
                        if (dataRowView["MaHang"] != DBNull.Value)
                        {
                            mahang = dataRowView["MaHang"].ToString();
                        }
                        if (mahang == "Mã màu" || mahang == "Tên định mức"||mahang== "Nhà cung cấp")
                        {
                           // builder2.AddContent(0, text);
                        }
                        else if(mahang == "Công đoạn")
                        {
                            double dongbo = dataRowView[header.value] == DBNull.Value ? 0 : dataRowView.Row.Field<double>(header.value+"-Max");
                            builder2.OpenElement(sequence++, "div");
                                builder2.AddAttribute(sequence++, "class", "d-flex align-items-center text-danger");

                                // DxCheckBox
                                builder2.OpenComponent<DevExpress.Blazor.DxCheckBox<bool>>(sequence++);
                                builder2.AddAttribute(sequence++, "Checked", valuecheck);
                                builder2.AddAttribute(sequence++, "CheckedChanged", EventCallback.Factory.Create<bool>(this, (bool e) => CongDoanChecked(e, dataRowView)));
                                builder2.CloseComponent();

                                // Custom span wrapper
                                builder2.OpenElement(sequence++, "span");
                                builder2.AddAttribute(sequence++, "class", "custom-card ms-1");
                                builder2.AddAttribute(sequence++, "style", "background-color:#FF8000;");

                                // Inner span (label)
                                builder2.OpenElement(sequence++, "span");
                                builder2.AddContent(sequence++, "S.L ");
                                builder2.CloseElement(); // </span>

                                // DxSpinEdit
                                builder2.OpenComponent<DevExpress.Blazor.DxSpinEdit<double>>(sequence++);
                                builder2.AddAttribute(sequence++, "Value", d);
                                builder2.AddAttribute(sequence++, "MaxValue", d);
                             
                                builder2.AddAttribute(sequence++, "CssClass", "text-danger custom-spin-edit");
                                builder2.AddAttribute(sequence++, "AllowMouseWheel", false);
                                builder2.AddAttribute(sequence++, "ShowSpinButtons", false);
                                builder2.AddAttribute(sequence++, "ValueChanged", EventCallback.Factory.Create<double>(this, (double e) => ChangedCongDoan(e)));
                                builder2.CloseComponent();

                                // Inner span (unit)
                                builder2.OpenElement(sequence++, "span");
                                builder2.AddContent(sequence++,string.Format(" / {0} bộ", dongbo));
                                builder2.CloseElement(); // </span>

                                builder2.CloseElement(); // </span>
                                builder2.CloseElement(); // </div>
                            
                        }    
                        else
                        {

                            if (d > 0)
                            {
                                builder2.OpenElement(sequence++, "div");
                                builder2.AddAttribute(sequence++, "class", "d-flex row");

                                // Left Column - SpinEdit
                                builder2.OpenElement(sequence++, "div");
                                builder2.AddAttribute(sequence++, "class", "col-6");

                                builder2.OpenComponent<DxSpinEdit<double>>(sequence++);
                                builder2.AddAttribute(sequence++, "Value", d);
                                builder2.AddAttribute(sequence++, "ShowSpinButtons", false);
                                builder2.AddAttribute(sequence++, "MaxValue", d);
                                builder2.AddAttribute(sequence++, "ValueChanged", EventCallback.Factory.Create<double>(this, (double e) =>
                                {
                                    dataRowView[header.value] = e;
                                }));
                                builder2.AddAttribute(sequence++, "AllowMouseWheel", false);
                                builder2.CloseComponent();

                                builder2.CloseElement(); // Close col-6

                                // Right Column - Text
                                builder2.OpenElement(sequence++, "div");
                                builder2.AddAttribute(sequence++, "class", "col-6");
                                builder2.AddAttribute(sequence++, "style", "align-items:end;align-content:center");
                                builder2.AddContent(sequence++, string.Format("/ {0}", d));
                                builder2.CloseElement(); // Close col-6

                                builder2.CloseElement(); // Close row
                            }

                            //builder2.OpenComponent<DevExpress.Blazor.DxSpinEdit<double>>(sequence++);
                            //builder2.AddAttribute(sequence++, "Value", d);
                            //builder2.AddAttribute(sequence++, "ShowSpinButtons", false);
                            //builder2.AddAttribute(sequence++, "MaxValue", d);
                            //builder2.AddAttribute(sequence++, "ValueChanged", EventCallback.Factory.Create<double>(this, (double e) =>
                            //{
                            //    dataRowView[header.value] = e;
                            //}));
                            //builder2.AddAttribute(sequence++, "AllowMouseWheel", false);
                            //builder2.CloseComponent();
                        }
                       
                    }));
                    builder.CloseComponent();
                }
            };
        }
        List<itemnhapxuat> lstheader = new List<itemnhapxuat>();
        private async Task searchAsync()
        {
            if (string.IsNullOrEmpty(dinhMucVatTuShowcrr.MaSP))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn sản phần"));
                return;
            }
            if (!kehoachselected.Any())
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn kế hoạch"));
                return;
            }
            var grouped = string.Join(";", kehoachselected.Select(p => p.ID));
           
            string sql = string.Format(@"use NVLDB
                             declare @MaSP nvarchar(100)=N'{0}'
                            declare @lstkehoach nvarchar(100)=N'{1}'
                             exec GetDinhMucNVL_SanPhamList_Root @lstsanpham=@MaSP
							declare @tblkh table(Serial int,SoKH nvarchar(100),MaSP nvarchar(100),MaHang nvarchar(100),MaMauKH nvarchar(100),SoLuongSP int,KhuVuc nvarchar(200),CongDoan nvarchar(200),SLKH float,SLTheoDoi float,SLQuyDoi float)
							
							insert into @tblkh
							exec SP.DataBase_ScansiaPacific2014.dbo.KeHoachDinhMuc_NVLListID @lstkehoach=@lstkehoach
							
							--qry.Id as Serial,qry.MaSP,qry.MaHang,khsp.MaMauKH,khsp.SoLuongSP,qry.KhuVuc,qry.CongDoan,qry.SLKH,qry.SLTheoDoi,qry.SLQuyDoi
								select * from ##tmpdinhmuctoancuc tmp 
							where GroupMauSP in (select MaMauKH from @tblkh group by MaMauKH) order by GroupMauSP,TenDinhMuc,NhaCungCap,CongDoan
							select * from @tblkh
                            Drop table ##tmpdinhmuctoancuc", dinhMucVatTuShowcrr.MaSP, grouped);
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            //lstpara.Add(new ParameterDefine("@Serial", Serial));
            //lstpara.Add(new ParameterDefine("@MaNCC", nvlNhapXuatKhoShowcrr.MaGN)); 
            CallAPI callAPI = new CallAPI();
            PanelVisible = true;
            try
            {
                renderFragmentcolumn = null;
             
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
               
                dtresult.Clear();
                dtresult.Columns.Clear();
                lstheader.Clear();
              
             
                Initcolumnsresult();
                initTable();
                if (json != "")
                {
                    customRoot = JsonConvert.DeserializeObject<CustomRoot>(json);
                    if (customRoot != null)
                    {
                        if (customRoot.lstdinhmuc.Any())
                        {
                            //Xử lý Tên định mức

                            string groupmausp = "-begin";
                            string tendinhmuc = "-1";
                            int indextendinhmuc = 0;
                            string tenncc = "-1";
                            int indextenncc = 0;
                            string congdoan = "-1";
                            int indexcongdoan = 0;
                            int index = 0;
                            double dongbocongdoan = 0;
                            DataRow rownewcongdoanrow=dtresult.NewRow();
                            foreach (var it in customRoot.lstdinhmuc)
                            {
                                if (groupmausp != it.GroupMauSP)
                                {
                                    index++;
                                    groupmausp = it.GroupMauSP;
                                    tendinhmuc = "-1";
                                    indextendinhmuc = 0;
                                    DataRow rownew = dtresult.NewRow();
                                    rownew["STT"] = string.Format("{0}", index);
                                    rownew["MaSP"] = it.MaSP;
                                    rownew["TenSP"] = it.TenSP;
                                    rownew["KhuVuc"] = it.KhuVuc;
                                    rownew["SLQuyDoi"] = 0;
                                    rownew["CongDoan"] = it.CongDoan;
                                    rownew["TenDinhMuc"] = it.TenDinhMuc;
                                    rownew["MaHang"] = "Mã màu";
                                    rownew["TenHang"] = it.TenMau;
                                    rownew["Colorhex"] = it.Colorhex;
                                    //rownew["ParentSort"] = it.TenMau;
                                    dtresult.Rows.Add(rownew);

                                }
                                if (tendinhmuc != it.TenDinhMuc)
                                {
                                    tendinhmuc = it.TenDinhMuc;
                                    indextendinhmuc++;


                                    tenncc = "-1";
                                    indextenncc = 0;

                                    congdoan = "-1";
                                    indexcongdoan = 0;

                                    DataRow rownewdinhmuc = dtresult.NewRow();
                                    rownewdinhmuc["STT"] = string.Format("{0}.{1}", index, indextendinhmuc.ToString("D2"));
                                    rownewdinhmuc["MaSP"] = it.MaSP;
                                    rownewdinhmuc["TenSP"] = it.TenSP;
                                    rownewdinhmuc["KhuVuc"] = it.KhuVuc;
                                    rownewdinhmuc["CongDoan"] = it.CongDoan;
                                    rownewdinhmuc["TenDinhMuc"] = it.TenDinhMuc;
                                    rownewdinhmuc["SLQuyDoi"] = 0;
                                    rownewdinhmuc["MaHang"] = "Tên định mức";
                                    rownewdinhmuc["TenHang"] = it.TenDinhMuc;
                                    rownewdinhmuc["TenSP"] = it.TenSP;
                                    rownewdinhmuc["Colorhex"] = "#FFFF00";//Màu vàng
                                                                          //rownew["ParentSort"] = it.TenMau;
                                    dtresult.Rows.Add(rownewdinhmuc);



                                }
                                if (!string.IsNullOrEmpty(it.TenNCC))
                                {
                                    if (tenncc != it.TenNCC)
                                    {
                                        indextenncc++;
                                        tenncc = it.TenNCC;
                                        congdoan = "-1";
                                        DataRow rownewncc = dtresult.NewRow();
                                        rownewncc["STT"] = string.Format("{0}.{1}.{2}", index, indextendinhmuc.ToString("D2"), indextenncc.ToString("D2"));
                                        rownewncc["MaSP"] = it.MaSP;
                                        rownewncc["TenSP"] = it.TenSP;
                                        rownewncc["SLQuyDoi"] = 0;
                                        rownewncc["KhuVuc"] = it.KhuVuc;
                                        rownewncc["CongDoan"] = it.CongDoan;
                                        rownewncc["TenDinhMuc"] = it.TenDinhMuc;
                                        rownewncc["MaHang"] = "Nhà cung cấp";
                                        rownewncc["TenHang"] = it.TenNCC;
                                        rownewncc["TenSP"] = it.TenSP;
                                        rownewncc["Colorhex"] = "#7db0df";
                                        dtresult.Rows.Add(rownewncc);//#6BB94A

                                    }
                                }
                                if (congdoan != it.CongDoan)
                                {
                                    indexcongdoan++;
                                    dongbocongdoan = 0;
                                    congdoan = it.CongDoan;
                                    DataRow rownewcongdoan = dtresult.NewRow();
                                    rownewcongdoan["STT"] = string.Format("{0}.{1}.{2}.{3}", index, indextendinhmuc.ToString("D2"), indextenncc.ToString("D2"), indexcongdoan.ToString("D2"));
                                    rownewcongdoan["MaSP"] = it.MaSP;
                                    rownewcongdoan["TenSP"] = it.TenSP;
                                    rownewcongdoan["SLQuyDoi"] = 0;
                                    rownewcongdoan["KhuVuc"] = it.KhuVuc;
                                    rownewcongdoan["CongDoan"] = it.CongDoan;
                                    rownewcongdoan["TenDinhMuc"] = it.TenDinhMuc;
                                    rownewcongdoan["MaHang"] = "Công đoạn";
                                    rownewcongdoan["TenHang"] = it.CongDoan;
                                    rownewcongdoan["TenSP"] = it.TenSP;
                                    rownewcongdoan["Colorhex"] = "#6BB94A";//Màu vàng
                                                                           //rownew["ParentSort"] = it.TenMau;
                                    dtresult.Rows.Add(rownewcongdoan);
                                    rownewcongdoanrow = rownewcongdoan;
                                }
                                DataRow rownewitem = dtresult.NewRow();
                                rownewitem["STT"] = string.Format("{0}.{1}.{2}.{3}.{4}", index, indextendinhmuc.ToString("D2"), indextenncc.ToString("D2"), indexcongdoan.ToString("D2"), it.Index);
                                rownewitem["MaSP"] = it.MaSP;
                                rownewitem["TenSP"] = it.TenSP;
                                rownewitem["KhuVuc"] = it.KhuVuc;
                                rownewitem["CongDoan"] = it.CongDoan;
                                rownewitem["TenDinhMuc"] = it.TenDinhMuc;
                                rownewitem["MaHang"] = it.MaVatTu;
                                rownewitem["TenHang"] = it.TenHang;
                                rownewitem["DVT"] = it.DVT;
                                rownewitem["SLQuyDoi"] = it.SLQuyDoi;
                                rownewitem["TongKH"] = 0;
                                rownewitem["TongDN"] = 0;
                                rownewitem["TongConLai"] = 0;
                                rownewitem["SLTon"] = (it.SLTon == null) ? 0 : it.SLTon;
                                rownewitem["TenSP"] = it.TenSP;
                              
                                dtresult.Rows.Add(rownewitem);

                            }
                            if (customRoot.lstkhdm.Any())
                            {
                                var querygroupkh = customRoot.lstkhdm.GroupBy(p => p.SoKH).Select(p => new { SoKH = p.Key }).ToList();
                                foreach (var it in querygroupkh)
                                {
                                    dtresult.Columns.Add(it.SoKH, typeof(double));
                                    dtresult.Columns.Add(it.SoKH+"-Max", typeof(double));
                                    lstheader.Add(new itemnhapxuat(it.SoKH, it.SoKH.Substring(0, 5)));
                                }
                                renderFragmentcolumn = CreateColumns(lstheader);
                                foreach (var it in customRoot.lstkhdm)
                                {
                                    foreach (DataRow row in dtresult.Rows)
                                    {
                                        if (it.MaHang == row.Field<string>("MaHang"))
                                        {
                                            if (it.CongDoan == row.Field<string>("CongDoan") && it.MaSP == row.Field<string>("MaSP") && it.KhuVuc == row.Field<string>("KhuVuc"))
                                            {
                                                row[it.SoKH] = it.SLTheoDoi;
                                                row[it.SoKH + "-Max"] = it.SLTheoDoi;
                                                break;
                                            }
                                        }

                                    }
                                }

                            }
                            //Tính đồng bộ theo công đoạn
                            var querydongbocongdoan=customRoot.lstkhdm.GroupBy(p=>new { CongDoan = p.CongDoan, MaSP = p.MaSP,SoKH=p.SoKH,MaHang=p.MaHang,KhuVuc=p.KhuVuc})
                                .Select(p => new { CongDoan = p.Key.CongDoan, MaSP = p.Key.MaSP,SoKH=p.Key.SoKH,KhuVuc=p.Key.KhuVuc,DongBo=Math.Round(p.Min(n=>n.SLTheoDoi/n.SLQuyDoi),2)}).ToList();
                            var querycongdoan = dtresult.Select(string.Format("MaHang='{0}'", "Công đoạn"));
                            foreach(DataRow row in querycongdoan)
                            {
                                foreach(var it in querydongbocongdoan)
                                {
                                    if(it.MaSP==row.Field<string>("MaSP")&&it.CongDoan==row.Field<string>("CongDoan")&&it.KhuVuc==row.Field<string>("KhuVuc"))
                                    {
                                        //Console.WriteLine(string.Format("{0}-{1},",it.CongDoan, it.DongBo));
                                        row[it.SoKH+"-Max"] =Math.Round(it.DongBo,0);
                                        row[it.SoKH] = Math.Round(it.DongBo, 0);
                                        //break;
                                        
                                    }
                                }
                            }
                            //lstheader.Add(new itemnhapxuat("Err", "Err"));
                            PanelVisible = false;
                           // dxGrid.Data = dtresult;
                            dxGrid.Reload();
                           

                        }
                        else
                        {
                            //Không có dữ liệu
                            toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                PanelVisible = false;
                StateHasChanged();
            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }
        }


        List<KeHoachSuDung> lstkehoach = new List<KeHoachSuDung>();
        public List<KeHoachDinhMucCongDoan> lstkehoachcongdoan { get; set; }
        private async void loadcheckallAsync(bool chk)
        {
            string sql = "";
            checkkehoachall = chk;
            //List<DinhMucVatTuShow>lst= new List<DinhMucVatTuShow>();
            if (customRoot == null)
                return;
            if (customRoot.lstdinhmuc == null)
                return;
            //lst = customRoot.lstdinhmuc.ToList();

            if (chk)
            {
                sql = string.Format(@"declare @MaSP nvarchar(100)=N'{0}'  select qry.*,isnull(mm.TenMau,'') as TenMau,mm.Color from
                 (select ID,MaSP,SoLuongSP,KhuVuc as KhuVucKH,MaMauKH from SP.DataBase_ScansiaPacific2014.dbo.[KeHoachSP]
		                where Active=0 and KhuVuc in ('KV2DH','KV3')
		                and MaSP=@MaSP
		                and datediff(MM,Ngay,getdate())<6
		                ) as qry left join SP.DataBase_ScansiaPacific2014.[dbo].MaMau mm on qry.MaMauKH=mm.MaMau", dinhMucVatTuShowcrr.MaSP);
            }
            else
            {
                sql = string.Format(@"declare @MaSP nvarchar(100)=N'{0}' select qry.*,isnull(mm.TenMau,'') as TenMau,mm.Color from
                    (select ID,MaSP,SoLuongSP,KhuVuc as KhuVucKH,MaMauKH from SP.DataBase_ScansiaPacific2014.dbo.[KeHoachSP]
		            where Active=0 and KhuVuc in ('KV2DH','KV3')
		            and MaSP=@MaSP
		            and ID in (SELECT DISTINCT  [ID_KeHoach] FROM SP.DataBase_ScansiaPacific2014.[dbo].[KH_Theodoi] where SLTheoDoi>0)
		            ) as qry left join SP.DataBase_ScansiaPacific2014.[dbo].MaMau mm on qry.MaMauKH=mm.MaMau", dinhMucVatTuShowcrr.MaSP);
            }
            //customRoot = new CustomRoot();
            CallAPI callAPI = new CallAPI();
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<KeHoachSuDung>>(json);
                lstkehoach.Clear();
                lstkehoach.AddRange(query);

            }

            //dxGrid.Reload(); 
            StateHasChanged();


        }

        //private void checkedchangedItem(bool bl, DinhMucVatTuShow dinhMucVatTuShow)
        //{
        //    //Kiểm tra xem check ở group đã được check chưa
        //    var querydataview = customRoot.lstkehoachcongdoan.Where(p => p.GroupNhaCungCap == dinhMucVatTuShow.GroupNhaCungCap && p.MauSP == dinhMucVatTuShow.GroupMauSP && p.TenDinhMuc == dinhMucVatTuShow.TenDinhMuc && p.CongDoan == dinhMucVatTuShow.CongDoan && p.KhuVucKH.Equals(dinhMucVatTuShow.KhuVucKH)).FirstOrDefault();
        //    if (!querydataview.chk)
        //    {
        //        toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Công đoạn {0} phải được tick chọn trước", dinhMucVatTuShow.CongDoan)));
        //        return;
        //    }
        //    dinhMucVatTuShow.chk = bl;
        //    if (bl) { dinhMucVatTuShow.SLDeNghi = (decimal)dinhMucVatTuShow.SLConLai; }
        //    else { dinhMucVatTuShow.SLDeNghi = 0; }
        //}


        private void SLDeNghiChanged(decimal? e, DinhMucVatTuShow dinhMucVatTuShow)
        {
            if (e == null)
                return;

            if (e.Value > (decimal)dinhMucVatTuShow.SLConLai)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Số lượng đề nghị không được lớn hơn số lượng cho phép là {0}", dinhMucVatTuShow.SLConLai)));
                dinhMucVatTuShow.SLDeNghi = (decimal)dinhMucVatTuShow.SLConLai;
            }

        }



        private bool checklogic()
        {
            if (!checksave)
                return false;
            if (customRoot.lstdinhmuc == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                return false;
            }
            if (!customRoot.lstdinhmuc.Any())
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Không có dữ liệu"));
                return false;
            }
            var querycheckkehoach = lstkehoachcongdoan.Where(p => p.chk.Equals(true)).ToList();
            if (!querycheckkehoach.Any())
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ít nhất 1 công đoạn"));
                return false;
            }

            var query = customRoot.lstdinhmuc.Where(p => p.chk.Equals(true));
            if (!query.Any())
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng Đề nghị ít nhất 1 loại định mức"));
                return false;
            }
            var checksl = query.Where(p => p.SLDeNghi > (decimal)p.SLConLai);
            if (checksl.Any())
            {
                foreach (var item in checksl)
                {
                    item.Err = "SL đề nghị không được vượt quá SL Còn lại";
                }
                return false;
            }
            return true;
        }
        DataTable dtkehoachdm;
        DataTable dtkehoachitem;
        private void initTable()
        {
            if (dtkehoachdm == null)
            {
                //Lưu ý: thứ tự cột trong bảng khởi tạo này phải giống y hệt thứ tự cột của Type_NvlKeHoachMuaHang_DinhMuc và Type_NvlKeHoachMuaHangItemVer3, nếu sai thứ tự cột là lỗi tè le luôn
                dtkehoachdm = new DataTable();
                dtkehoachdm.Columns.Add("Serial", typeof(int));
                dtkehoachdm.Columns.Add("SerialLink", typeof(int));
                dtkehoachdm.Columns.Add("TableName", typeof(string));
                dtkehoachdm.Columns.Add("KeyGroup", typeof(string));
                dtkehoachdm.Columns.Add("IDKeHoach", typeof(string));

                dtkehoachdm.Columns.Add("TenDinhMuc", typeof(string));
                dtkehoachdm.Columns.Add("CongDoan", typeof(string));
                dtkehoachdm.Columns.Add("SoLuong", typeof(decimal));
                dtkehoachdm.Columns.Add("Ngay", typeof(DateTime));
                dtkehoachdm.Columns.Add("UserInsert", typeof(string));
                dtkehoachdm.Columns.Add("NgayInsert", typeof(DateTime));
                dtkehoachdm.Columns.Add("MaSP", typeof(string));
                dtkehoachdm.Columns.Add("MaMau", typeof(string));
                dtkehoachitem = new DataTable();
                dtkehoachitem.Columns.Add("STT", typeof(int));
                dtkehoachitem.Columns.Add("ID", typeof(string));
                dtkehoachitem.Columns.Add("Serial", typeof(int));
                dtkehoachitem.Columns.Add("SerialDN", typeof(int));
                dtkehoachitem.Columns.Add("MaHang", typeof(string));
                dtkehoachitem.Columns.Add("SoLuong", typeof(double));
                dtkehoachitem.Columns.Add("SLTheoDoi", typeof(double));
                dtkehoachitem.Columns.Add("DonGia", typeof(double));
                dtkehoachitem.Columns.Add("DVT", typeof(string));
                dtkehoachitem.Columns.Add("VAT", typeof(int));
                dtkehoachitem.Columns.Add("GhiChu", typeof(string));
                dtkehoachitem.Columns.Add("MaSP", typeof(string));
                dtkehoachitem.Columns.Add("SerialLink", typeof(int));
                dtkehoachitem.Columns.Add("SLQuyDoiSP", typeof(double));
                dtkehoachitem.Columns.Add("TableName", typeof(string));
                dtkehoachitem.Columns.Add("NgayEdit", typeof(DateTime));
                dtkehoachitem.Columns.Add("NgayInsert", typeof(DateTime));
                dtkehoachitem.Columns.Add("UserInsert", typeof(string));
                dtkehoachitem.Columns.Add("TenLienKet", typeof(string));
            }
        }
        bool checksave = true;
        private async Task saveAsync()
        {
            initTable();
            dtkehoachdm.Clear();
            dtkehoachitem.Clear();
            // kiểm tra, tránh API lưu lại 2 lần do mạng chậm
            //KeyGroup tạo ở sự kiện khi checkbox của công đoạn được click
            if (checklogic())
            {

                var querycheccongdoan = lstkehoachcongdoan.Where(p => p.chk.Equals(true)).ToList();
                foreach (var it in querycheccongdoan)
                {
                    foreach (var item in it.lstkehoachcongdoanitem)
                    {
                        if (item.SLDeNghi > 0)
                        {
                            //item.KeyGroup= string.Format("{0}_{1}", keHoachMuaHang_Showcrr.Serial, StaticClass.Randomstring(10));
                            DataRow dataRow = dtkehoachdm.NewRow();
                            dataRow["Serial"] = 0;
                            dataRow["SerialLink"] = keHoachMuaHang_Showcrr.Serial;
                            dataRow["IDKeHoach"] = item.MaKH;
                            dataRow["TableName"] = "NvlKehoachMuaHang";
                            dataRow["TenDinhMuc"] = it.TenDinhMuc;
                            dataRow["KeyGroup"] = it.KeyGroup;
                            dataRow["CongDoan"] = it.CongDoan;
                            dataRow["SoLuong"] = item.SLDeNghi;
                            dataRow["UserInsert"] = ModelAdmin.users.UsersName;
                            dataRow["MaSP"] = it.MaSP;
                            dataRow["MaMau"] = it.MauSP;
                            dtkehoachdm.Rows.Add(dataRow);
                        }
                    }
                }
                var queryitem = customRoot.lstdinhmuc.Where(p => p.chk.Equals(true));
                foreach (var it in queryitem)
                {
                    DataRow rownew = dtkehoachitem.NewRow();
                    rownew["STT"] = it.Index;
                    rownew["Serial"] = 0;
                    rownew["SerialDN"] = keHoachMuaHang_Showcrr.Serial;
                    rownew["MaHang"] = it.MaVatTu;
                    rownew["SoLuong"] = it.SLDeNghi;
                    rownew["SLTheoDoi"] = it.SLDeNghi;
                    rownew["DonGia"] = 0;
                    rownew["DVT"] = it.DVT;

                    rownew["ID"] = it.KeyGroup;
                    rownew["TableName"] = "NvlKeHoachMuaHang_DinhMuc";
                    rownew["TenLienKet"] = it.MaKH;
                    rownew["UserInsert"] = ModelAdmin.users.UsersName;
                    dtkehoachitem.Rows.Add(rownew);
                }

                CallAPI callAPI = new CallAPI();

                string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_InsertTableDinhMuc_Ver2";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Type_NvlKeHoachMuaHang_DinhMuc", prs.ConvertDataTableToJson(dtkehoachdm), "DataTable"));
                lstpara.Add(new ParameterDefine("@Type_NvlKeHoachMuaHangItem", prs.ConvertDataTableToJson(dtkehoachitem), "DataTable"));
                lstpara.Add(new ParameterDefine("@SerialDN", keHoachMuaHang_Showcrr.Serial));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));

                try
                {



                    checksave = false;//Khóa lại để tránh lưu 2 lần do API bất đồng bộ

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
                            string err = "";
                            if (query[0].TenDinhMuc != null)
                            {
                                err = string.Format("Lỗi: {0} của {1}:{2}", query[0].CongDoan, query[0].TenDinhMuc, query[0].ketqua);
                                toastService.Notify(new ToastMessage(ToastType.Warning, err));

                            }
                            else
                                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception));
                            //if (query[0].ketquaexception != null)
                            //{
                            //    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception));
                            //}

                        }
                        checksave = true;

                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, "Đã xảy ra lỗi."));
                    }

                    checksave = true;

                }
                catch (Exception ex)
                {

                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex));
                }
                finally
                {
                    checksave = true;
                }
            }
        }
        private void reset()
        {
            customRoot.lstdinhmuc.Clear();
            lstkehoachcongdoan.Clear();
            lstkehoach.Clear();
            kehoachselected = null;

            dxGrid.Reload();
            StateHasChanged();
        }
    }

}
