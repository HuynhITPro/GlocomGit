using BlazorBootstrap;
using DevExpress.Blazor;
using DevExpress.Utils.Filtering.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace NFCWebBlazor.App_DonHang
{
    public partial class Page_DonHangMua
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] BrowserService browserService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _ = loadAsync();

            }
            try
            {

             
                var dimension = await browserService.GetDimensions();
                // var heighrow = await browserService.GetHeighWithID("divcontainer");
                int height = dimension.Height - 70;
                //if (heighrow!=null)
                //{
                //    height = dimension.Height - heighrow;
                //}

                heightgrid = string.Format("{0}px", height);
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi browserService :" + ex.Message));
            }


            //await JS.InvokeVoidAsync("scrollToBottomLast");

            //base.OnAfterRender(firstRender);
        }
        List<DataDropDownList> lstkehoachmua = new List<DataDropDownList>();
        private async Task loadAsync()
        {
            try
            {
                var query = await Model.ModelData.GetDonHangMua();
              
                var query2 = await Model.ModelData.GetKhachHangSanSuatSP();
                lstkhachhang = query2.ToList();
                lstloaibaocao.AddRange(query.ToList());
                lstkehoachmua.AddRange(query.ToList());



            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
            }
            //baocaoselected = lstloaibaocao.FirstOrDefault(p => p.Name.Equals("Tồn kho theo sản phẩm"));
            StateHasChanged();

        }
        public RenderFragment buildrender()
        {
            return BuildColumns(lstcolumn);
        }
        public static RenderFragment BuildColumns(List<InitDxGridDataColumn> _lstcolumn)
        {
            RenderFragment columns = b =>
            {
                int counter = 0;
                foreach (InitDxGridDataColumn col in _lstcolumn.OrderBy(p => p.Index))
                {
                    b.OpenComponent(counter, typeof(DxGridDataColumn));
                    b.AddAttribute(0, "FieldName", col.FieldName);

                    b.AddAttribute(0, "Caption", col.Caption);
                    if (col.gridTextAlignment != null)
                        b.AddAttribute(0, "TextAlignment", col.gridTextAlignment);
                    if (col.DisplayFormat != null)
                    {
                        b.AddAttribute(0, "DisplayFormat", col.DisplayFormat);
                    }
                    if (col.Width != null)
                        b.AddAttribute(0, "Width", string.Format("{0}px", col.Width));
                    else
                    if (col.Width != null)
                        b.AddAttribute(0, "MinWidth", string.Format("90px"));
                    if (col.GroupIndex != null)
                    {
                        b.AddAttribute(0, "GroupIndex", col.GroupIndex.Value);
                    }
                    b.CloseComponent();

                    counter++;
                }


            };
            return columns;
        }

        class ThanhTienNhaMay
        {
            public ThanhTienNhaMay()
            {

            }
            public ThanhTienNhaMay(string _nhamay, double _thanhtien)
            {
                NhaMay = _nhamay;
                ThanhTien = _thanhtien;
            }
            public string NhaMay { get; set; }
            public double ThanhTien { get; set; } = 0;
        }
        List<ThanhTienNhaMay> lstthanhtien = new List<ThanhTienNhaMay>();

        int totaldaxuat = 0, totaltonkho = 0, totalphailam = 0, wtotaldaxuat = 0, wtotaltonkho = 0, wtotalphailam = 0;
        private async void search()
        {
           
            lstcolumn.Clear();
            dtresultfinal.Clear();
            dtresultfinal.Columns.Clear();
            lstthanhtien.Clear();
            DataTable dt = await searchAsync();
            bool checkname = false;
            foreach (var it in dxGrid.GetDataColumns())
            {
                checkname = false;
                foreach (DataColumn cl in dt.Columns)
                {
                    if (cl.ColumnName == it.FieldName)
                    {
                        checkname = true;
                        break;
                    }
                }
                if (!checkname)
                {
                    //it.Visible = false;
                    dt.Columns.Add(it.FieldName, typeof(string));
                }
            }


            //dt.Columns.Add("Chart", typeof(string));

            dtresultfinal = dt;
            var querythanhtien = dt.Select("TenSP='TỔNG ĐƠN HÀNG'").FirstOrDefault();
          
            if(querythanhtien!=null)
            {
                ThanhTienNhaMay thanhTienNhaMay = new ThanhTienNhaMay();
                lstthanhtien.Add(thanhTienNhaMay);
                //row["wSLDaXuat"] = (int)Math.Round(double.Parse(row["wSLDaXuat"].ToString()) / widthtotal * 100, 0);
                //row["wSLTonKho"] = (int)Math.Round(double.Parse(row["wSLTonKho"].ToString()) / widthtotal * 100, 0);
                //row["wSLPhaiNhapTongDH"] = (int)Math.Round(double.Parse(row["wSLPhaiNhapTongDH"].ToString()) / widthtotal * 100, 0);
                totaldaxuat =(int)Math.Round(querythanhtien.Field<double>("SLDaXuat"),0);
                totaltonkho = (int)Math.Round(querythanhtien.Field<double>("SLTonKho"), 0);
                totalphailam = (int)Math.Round(querythanhtien.Field<double>("SLPhaiNhapTongDH"), 0);
               
                double total = totaldaxuat + totaltonkho + totalphailam;
                if (total > 0)
                {
                    wtotaldaxuat = (int)Math.Round(totaldaxuat / total * 100, 0);
                    wtotaltonkho = (int)Math.Round(totaltonkho / total * 100, 0);
                    wtotalphailam = (int)Math.Round(totalphailam / total * 100, 0);
                }

            }
            dxGrid.AutoFitColumnWidths();
          
            StateHasChanged();


          
        }

        string ghichu = "";
        App_ClassDefine.ClassProcess prs = new ClassProcess();
        List<string> lstcolumnloi = new List<string>();
       
        private async Task<DataTable>searchAsync()
        {
            DataTable dtresult = new DataTable();
            if (baocaoselected == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Vui lòng chọn đơn hàng mùa trước"));
                return dtresult;
            }
            if (baocaoselected.ToList().Count==0)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Vui lòng chọn đơn hàng mùa"));
                return dtresult;
             
            }
            XuLyDonHang xuLyDonHang = new XuLyDonHang();
            PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
            CallAPI callAPI = new CallAPI();
         
            List<string> lst = new List<string>();
            foreach (var it in baocaoselected)
            {
                lst.Add(it.Name);
            }
            try
            {

                if (SelectedChecked == "Gộp theo sản phẩm")
                {
                    showart = false;
                    dtresult = await xuLyDonHang.DonHangMua(callAPI, lst);
                    //dt.Columns.Add("MaHDGroup",typeof(string));
                    //grvNhapXuatkho.Columns["ArticleNumber"].Visible = false;
                    //grvNhapXuatkho.Columns["TenMau"].Visible = false;
                }
                else
                {
                    showart = true;
                    dtresult = await xuLyDonHang.DonHangMuaArt(callAPI, lst);
                    //grvNhapXuatkho.Columns["ArticleNumber"].Visible = true;
                    //grvNhapXuatkho.Columns["TenMau"].Visible = true;
                }


                dtresult.Columns.Add("wSLDaXuat", typeof(int));
                dtresult.Columns.Add("wSLTonKho", typeof(int));
                dtresult.Columns.Add("wSLPhaiNhapTongDH", typeof(int));
                dtresult.Columns.Add("wTyLe", typeof(int));
                dtresult.Columns.Add("TyLe", typeof(double));
                dtresult.Columns.Add("ColorValue", typeof(string));
                dtresult.Columns.Add("VisibleTemp", typeof(bool));

                int widthtotal = 300;
                int temp = 0;
                //Xử lý Drawchart
                foreach (DataRow row in dtresult.Rows)
                {
                    if (row.Field<double>("TongDH") > 0)
                    {

                        row["VisibleTemp"] = true;
                        if (row.Field<double>("SLPhaiNhapTongDH") < 0)
                            row["SLPhaiNhapTongDH"] = 0;
                        double total = row.Field<double>("SLPhaiNhapTongDH") + row.Field<double>("SLDaXuat") + row.Field<double>("SLTonKho");

                        double onepixel = widthtotal / total;
                        temp = (int)(Math.Round(onepixel * row.Field<double>("SLDaXuat"), 0));
                        if (temp > 0 && temp < 40)
                        {
                            temp = 40;
                        }
                        row["wSLDaXuat"] = temp;

                        temp = (int)(Math.Round(onepixel * row.Field<double>("SLPhaiNhapTongDH"), 0));

                        if (temp > 0 && temp < 40)
                        {
                            row["wSLDaXuat"] = row.Field<int>("wSLDaXuat") - (40 - temp);//Giảm kích thước của đã xuất xuống
                            temp = 40;

                        }
                        row["wSLPhaiNhapTongDH"] = temp;

                        temp = widthtotal - row.Field<int>("wSLDaXuat") - row.Field<int>("wSLPhaiNhapTongDH");

                        if (temp > 0 && temp < 40)
                        {
                            if (row.Field<int>("wSLDaXuat") > 100)
                                row["wSLDaXuat"] = row.Field<int>("wSLDaXuat") - (40 - temp);
                            else
                            {

                                row["wSLPhaiNhapTongDH"] = row.Field<int>("wSLPhaiNhapTongDH") - (40 - temp);
                            }
                            temp = 40;

                        }

                        row["wSLTonKho"] = temp;

                        double phainhap = row.Field<double>("SLPhaiNhapTongDH");
                        if (phainhap <= 0)
                        {
                            row["wTyLe"] = 100;
                            row["ColorValue"] = "#008000";
                            row["TyLe"] = 1;
                        }
                        else
                        {
                            //row["wTyLe"] = 100 - (int)Math.Round(phainhap* onepixel,0);
                            row["TyLe"] = 1 - phainhap / row.Field<double>("TongDH");

                            double tyle = row.Field<double>("TyLe");
                            row["wTyLe"] = Math.Round(tyle * 100, 0);
                            if (tyle < 0.6)
                            {
                                row["ColorValue"] = "#c3352b";
                            }
                            if (tyle >= 0.6 && tyle <= 0.9)
                            {
                                row["ColorValue"] = "#f77b72";
                            }
                            if (tyle > 0.9)
                                row["ColorValue"] = "#66cc00";
                        }
                        //Chuyển về tỷ lệ
                        row["wSLDaXuat"] = (int)Math.Round(double.Parse(row["wSLDaXuat"].ToString()) / widthtotal * 100, 0);
                        row["wSLTonKho"] = (int)Math.Round(double.Parse(row["wSLTonKho"].ToString()) / widthtotal * 100, 0);
                        row["wSLPhaiNhapTongDH"] = (int)Math.Round(double.Parse(row["wSLPhaiNhapTongDH"].ToString()) / widthtotal * 100, 0);

                    }
                    else
                    {
                        row["VisibleTemp"] = false;
                    }
                }
                PreloadService.Hide();
            }
            catch(Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning,"Lỗi: "+ ex.Message));
                Console.WriteLine(ex.Message);
            }
            finally
            {
                PreloadService.Hide();
            }
           // PreloadService.Hide();
            lst.Clear();
            return dtresult;
        }
        void txtkhachhangselected(NvlKhachHang nvlkhachhang)
        {
            if(nvlkhachhang!=null)
            {
                baocaoselected = null;
                 lstloaibaocao.Clear();
                var query = lstkehoachmua.Where(p => p.TypeName.Equals(nvlkhachhang.MaKh)).ToList();
                lstloaibaocao.AddRange(query);
                StateHasChanged();
            }
        }
        public class NvlTonKhoItemShow
        {
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string PhanLoai { get; set; }
            public string DVT { get; set; }
            public Nullable<double> SLQuyDoi { get; set; }
            public Nullable<double> SLTon { get; set; }
            public Nullable<double> DBTon { get; set; }

        }
    }
}
