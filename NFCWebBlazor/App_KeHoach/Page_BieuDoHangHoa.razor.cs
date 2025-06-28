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
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;

namespace NFCWebBlazor.App_KeHoach
{
    public partial class Page_BieuDoHangHoa
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] BrowserService browserService { get; set; }
        [Inject] UserGlobal userGlobal { get; set; }

        protected override Task OnInitializedAsync()
        {

            return base.OnInitializedAsync();
        }
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
        int columnsbegin = 0;
        private async Task loadAsync()
        {
            try
            {
                nhamayselected = txtnhamay.SelectedValue(userGlobal.users.NhaMay);
                var query = await Model.ModelData.GetSanPham();
                lstsanpham = query.ToList();
                var query2 = await Model.ModelData.GetListKhachHang();
                lstkhachhang = query2.ToList();
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


       List<string>lstimg64= new List<string>();
      

        private async void search()
        {
            if (nhamayselected== null || khachhangselected == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Vui lòng nhập đầy đủ thông tin LOẠI BÁO CÁO, GỘP THEO, NGÀY trước khi tìm kiếm"));
                return;
            }
            lstimg64.Clear();
            lstimg.Clear();
           
            //PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi");
            lstimg =  await searchAsync();
            //var query = lst.Select(p => new imgclass { imgBase64 = p }).ToList();
            //lstimg = query;
            //lst.Clear();
          
            //PreloadService.Hide();

            hidedivsearch();

            //dxGrid.AutoFitColumnWidths();
            StateHasChanged();
        }

        public class SvgImg
        {
            public string MaSP { get; set; }
            public string Svg { get; set; }
        }
        string ghichu = "";
        App_ClassDefine.ClassProcess prs = new ClassProcess();
        public class imgclass
        {
            public string imgBase64 { get; set; }
          
        }
        class ClassDieuKien
        {
            public string MaSP { get; set; }
            public string NhaMay { get; set; }
            public string KhachHang { get; set; }
        }
        public List<SvgImg> lstimg = new List<SvgImg>();
        private async Task<List<SvgImg>> searchAsync()
        {

            List<SvgImg> lstitem = new List<SvgImg>();
            ClassDieuKien classDieuKien = new ClassDieuKien();
            classDieuKien.MaSP = "";
            if (sanphamselected!=null)
                classDieuKien.MaSP=sanphamselected.MaSP;
                
            
            if (khachhangselected==null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Chọn khách hàng"));
                return lstitem;
            }
            if (nhamayselected==null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Chọn nhà máy"));
                return lstitem;
            }
            PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi....");
           
            classDieuKien.NhaMay = nhamayselected.Name;
            classDieuKien.KhachHang = khachhangselected.MaKh;
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = System.Text.Json.JsonSerializer.Serialize(classDieuKien);
                string json = await callAPI.GetChartEncrypt(sql);
                if (json != "")
                {
                    lstitem = JsonConvert.DeserializeObject<List<SvgImg>>(json);

                    return lstitem;
                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Lỗi:"+ex.Message));
               
            }
            finally
            {
                PreloadService.Hide();
            }
            return lstitem;


        }
        public  string ConvertSvgToBase64(string svgContent)
        {
            //var svgBytes = Encoding.UTF8.GetBytes(svgContent);
            //Console.WriteLine(svgBytes);
            //return Convert.ToBase64String(svgBytes);

           var svgBytes = Encoding.UTF8.GetBytes(svgContent);
            return Convert.ToBase64String(svgBytes);
        }
        //private string ConvertSvgToBase64()
        //{
        //    string svgString = @"<svg width=""1000"" height=""500"">
        //                        <!-- Hình chữ nhật thứ nhất -->
        //                        <rect x=""50"" y=""20"" width=""100"" height=""20"" fill=""red"" />
        //                        <text x=""50"" y=""35"" font-family=""Arial"" font-size=""12"" fill=""white"">Hello</text>
        //                    </svg>";

        //    // Chuyển chuỗi SVG thành hình ảnh (PNG)
        //    // Thay đổi định dạng hình ảnh nếu cần (PNG, JPEG, GIF)
        //    byte[] imageBytes = ConvertSvgToImage(svgString);

        //    return Convert.ToBase64String(imageBytes);
        //}




    }


}

