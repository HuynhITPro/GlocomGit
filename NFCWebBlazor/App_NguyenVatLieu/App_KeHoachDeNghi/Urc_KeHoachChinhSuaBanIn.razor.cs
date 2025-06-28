using BlazorBootstrap;
using Blazored.Modal;
using Blazored.Modal.Services;
using DevExpress.Blazor;
using DevExpress.Blazor.Internal;
using DevExpress.XtraReports;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Text;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Urc_KeHoachChinhSuaBanIn
    {
        ObservableCollection<NvlKeHoachMuaHangItemShow> Items { get; set; } = new ObservableCollection<NvlKeHoachMuaHangItemShow>();
        bool ReInitializeDragging { get; set; }
        [Inject] IJSRuntime JS { get; set; }
        [Inject] IModalService modal { get; set; }
        [Inject]
        ToastService ToastService { get; set; }
        Guid myKey { get; set; } = Guid.NewGuid();
        DotNetObjectReference<Urc_KeHoachChinhSuaBanIn> DotNetHelper { get; set; }
        IJSObjectReference JsModule { get; set; }
        void Grid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            if (e.ElementType == GridElementType.DataRow)
                e.Attributes["data-visible-index"] = e.VisibleIndex;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
               // Console.WriteLine(" lan thu 0");
                JsModule = await JS.InvokeAsync<IJSObjectReference>("import", "../App_NguyenVatLieu/App_KeHoachDeNghi/Urc_KeHoachChinhSuaBanIn.razor.js");

                DotNetHelper = DotNetObjectReference.Create(this);
               
              // Console.WriteLine(" lan thu 1");
                await JsModule.InvokeVoidAsync("setDotNetHelper", DotNetHelper);
                await JsModule.InvokeVoidAsync("initialize");
              


            }
            else
            {
               // Console.WriteLine(" lan thu n chua drag");
                if (ReInitializeDragging)
                {
                   // Console.WriteLine(" lan thu n");
                    ReInitializeDragging = false;
                    Console.WriteLine("call render");
                    await JsModule.InvokeVoidAsync("initialize");
                    
                }
            }
        }
        protected override async Task OnInitializedAsync()
        {

            if (keHoachMuaHang_Show.lstitem == null)
            {
                
                keHoachMuaHang_Show.lstitem = new ObservableCollection<NvlKeHoachMuaHangItemShow>();
                string sql = string.Format(@"Use NVLDB
                  select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT
                  ,isnull(qrytk.SLTon,0) as SLTon,hh.MinTK,hh.MaxTK,qrydathang.DonGiaDat from 
                                  (SELECT  isnull(STT,0) as STT,[Serial],[SerialDN],[MaHang],MaSP
                                            ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet

                                        FROM [NvlKeHoachMuaHangItem]
                                        Where SerialDN = {0}) as qry
                                          left join (SELECT min([DonGia]) as DonGiaDat,[SerialLink] FROM [NvlDonDatHangItem]
                                                    where DonGia>0 group by SerialLink) as	qrydathang on qry.Serial=qrydathang.SerialLink
             left join (select MaHang,sum(SLNhap-SLXuat) as SLTon from NvlNhapXuatItem group by MaHang) as qrytk
            on qry.MaHang=qrytk.MaHang
             inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                         inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom  order by qry.STT
            ", keHoachMuaHang_Show.Serial);

                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<List<NvlKeHoachMuaHangItemShow>>(json);
                    //foreach(var item in query)
                    //{
                    //    keHoachMuaHang_Show.lstitem.Add(item);
                    //}
                    //keHoachMuaHang_Show.lstitem.AddRange(query);
                    Items = new ObservableCollection<NvlKeHoachMuaHangItemShow>(query);
                    keHoachMuaHang_Show.lstitem = Items;
                    //dxGrid.Reload();không nên gọi lại hàm này, vì nó xung đột luồng
                    //Console.WriteLine("oninitcommit");
                    //dxGrid.Data = keHoachMuaHang_Show.lstitem;

                    //await JS.InvokeVoidAsync("scrollGridToLastRow");
                    //Grid.Data = lstDonDatHangSearchShow;

                }

            }
            else
            {
                Items = keHoachMuaHang_Show.lstitem;

                //Console.WriteLine("oninit2:"+ keHoachMuaHang_Show.lstitem.Count);
            }

            // base.OnInitialized();
        }

        [JSInvokable]
        public void ReorderGridRows(int draggableRowVisibleIndex, int prevRowVisibleIndex, int nextRowVisibleIndex)
        {
            shouldrender = true;
         
            var hasPrevRow = prevRowVisibleIndex > -1;
            var hasNextRow = nextRowVisibleIndex > -1;

            var sourceItem = (NvlKeHoachMuaHangItemShow)dxGrid.GetDataItem(draggableRowVisibleIndex);
            var hasSamePosition = hasPrevRow && draggableRowVisibleIndex == prevRowVisibleIndex + 1 || hasNextRow && draggableRowVisibleIndex == nextRowVisibleIndex - 1;
            if (hasSamePosition)
                return;

            var moveTop = false;
            if (!hasPrevRow || prevRowVisibleIndex <= draggableRowVisibleIndex)
                moveTop = true;
            var newVisibleIndex = moveTop ? nextRowVisibleIndex : prevRowVisibleIndex;

            var sourceItemIndex = Items.IndexOf(sourceItem);
            var newItemIndex = Items.IndexOf((NvlKeHoachMuaHangItemShow)dxGrid.GetDataItem(newVisibleIndex));
            Items.Move(sourceItemIndex, newItemIndex);

            myKey = Guid.NewGuid();
            ReInitializeDragging = true;
            Console.WriteLine("Call reoder");
            InvokeAsync(StateHasChanged);
        }
        //public record NvlKeHoachMuaHangItemShow;
        private void print()
        {
            //PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");

            //Task.Delay(100);
            try
            {

                _ = saveAsync();
                XtraRp_DuTruVatTu xtraRp_DuTruVatTu = new XtraRp_DuTruVatTu();
                xtraRp_DuTruVatTu.DataSource = Items;

                xtraRp_DuTruVatTu.setNoidung(keHoachMuaHang_Show.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
                xtraRp_DuTruVatTu.setMaDeNghi(keHoachMuaHang_Show.Serial.ToString());
                xtraRp_DuTruVatTu.setNguoiDuyet(keHoachMuaHang_Show.PhongBan, keHoachMuaHang_Show.LoaiKeHoach, keHoachMuaHang_Show.NguoiDN, keHoachMuaHang_Show.NguoiKiem, keHoachMuaHang_Show.NguoiDuyet, keHoachMuaHang_Show.DaDuyet, NoiDungDeNghi);

               
                //parameters.Add("report", xtraRp_DuTruVatTu);
                //modal.Show<ReportShow>("", parameters, options);
                ModelAdmin.mainLayout.showreportAsync(xtraRp_DuTruVatTu);

                //ModalOptions options = new ModalOptions()
                //{
                //    Size = Blazored.Modal.ModalSize.Automatic,

                //    Position = ModalPosition.Custom,
                //    PositionCustomClass = "custom-modal-top",

                //    DisableBackgroundCancel = true,


                //};


                //var parameters = new ModalParameters();
                //parameters.Add("report", xtraRp_DuTruVatTu);
                //modal.Show<ReportShow>("", parameters, options);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Lỗi:"+ex.ToString());
            }
            finally
            {
                //PreloadService.Hide();
            }
        }
        private async Task saveAsync()
        {
            try
            {
              
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<rows>");
                int i = 1;
                foreach (NvlKeHoachMuaHangItemShow it in Items)
                {

                    stringBuilder.Append("<row>");
                    stringBuilder.Append(string.Format("<Serial>{0}</Serial><STT>{1}</STT>",it.Serial, i));
                    stringBuilder.Append("</row>");
                    i++;

                }
                stringBuilder.Append("</rows>");
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_UpdateSTTTable";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@NoiDungDeNghi", NoiDungDeNghi));
                lstpara.Add(new ParameterDefine("@SerialDN", keHoachMuaHang_Show.Serial));
                lstpara.Add(new ParameterDefine("@xmlinput", stringBuilder.ToString()));
               
                Console.WriteLine(stringBuilder);
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                    }
                    else
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketqua));

                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                //if (ex.Message.Contains("duplicate key"))
                //{
                //    msgBox.Show("Mã này đã tồn tại rồi", IconMsg.iconerror);

                //}
                //else
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message, IconMsg.iconerror));
            }
        }
        private void reload()
        {
            dxGrid.Reload();
        }
        private async Task exportexcelAsync()
        {
            _ = saveAsync();
            XtraRp_DuTruVatTu xtraRp_DuTruVatTu = new XtraRp_DuTruVatTu();
            xtraRp_DuTruVatTu.DataSource = Items;

            xtraRp_DuTruVatTu.setNoidung(keHoachMuaHang_Show.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
            xtraRp_DuTruVatTu.setMaDeNghi(keHoachMuaHang_Show.Serial.ToString());
            xtraRp_DuTruVatTu.setNguoiDuyet(keHoachMuaHang_Show.PhongBan, keHoachMuaHang_Show.LoaiKeHoach, keHoachMuaHang_Show.NguoiDN, keHoachMuaHang_Show.NguoiKiem, keHoachMuaHang_Show.NguoiDuyet, keHoachMuaHang_Show.DaDuyet, NoiDungDeNghi);
            xtraRp_DuTruVatTu.checkHideHeader = false;
            DevExpress.XtraPrinting.XlsxExportOptions exportOptions = new DevExpress.XtraPrinting.XlsxExportOptions();
            exportOptions.TextExportMode = DevExpress.XtraPrinting.TextExportMode.Text;
            xtraRp_DuTruVatTu.checkHideHeader = false;
            // Export the report to XLSX format
            using (MemoryStream stream = new MemoryStream())
            {
                xtraRp_DuTruVatTu.ExportToXlsx(stream, exportOptions);

                // Convert the stream to a byte array
                var buffer = stream.ToArray();

                // Convert the byte array to base64 string
                var base64String = Convert.ToBase64String(buffer);

                // Invoke JavaScript function to save the file
                await JS.InvokeVoidAsync("saveAsFile", "Report.xlsx", base64String);
            }
          
            //xtraRp_DuTruVatTu.ExportToXlsx("C:\\Temp.xlsx",null);
        }
        private async Task exportpdfAsync()
        {
            XtraRp_DuTruVatTu xtraRp_DuTruVatTu = new XtraRp_DuTruVatTu();
            xtraRp_DuTruVatTu.DataSource = Items;

            xtraRp_DuTruVatTu.setNoidung(keHoachMuaHang_Show.NoiDung.TrimEnd(Environment.NewLine.ToCharArray()));
            xtraRp_DuTruVatTu.setMaDeNghi(keHoachMuaHang_Show.Serial.ToString());
            xtraRp_DuTruVatTu.setNguoiDuyet(keHoachMuaHang_Show.PhongBan, keHoachMuaHang_Show.LoaiKeHoach, keHoachMuaHang_Show.NguoiDN, keHoachMuaHang_Show.NguoiKiem, keHoachMuaHang_Show.NguoiDuyet, keHoachMuaHang_Show.DaDuyet, NoiDungDeNghi);
            xtraRp_DuTruVatTu.checkHideHeader = false;
            DevExpress.XtraPrinting.PdfExportOptions exportOptions = new DevExpress.XtraPrinting.PdfExportOptions();
            //exportOptions. = DevExpress.XtraPrinting.TextExportMode.Text;
           
            // Export the report to XLSX format
            using (MemoryStream stream = new MemoryStream())
            {
                xtraRp_DuTruVatTu.ExportToPdf(stream, exportOptions);

                // Convert the stream to a byte array
                var buffer = stream.ToArray();

                // Convert the byte array to base64 string
                var base64String = Convert.ToBase64String(buffer);

                // Invoke JavaScript function to save the file
                await JS.InvokeVoidAsync("saveAsFile", "Report.pdf", base64String);
            }

            //xtraRp_DuTruVatTu.ExportToXlsx("C:\\Temp.xlsx",null);
        }

    }
}
