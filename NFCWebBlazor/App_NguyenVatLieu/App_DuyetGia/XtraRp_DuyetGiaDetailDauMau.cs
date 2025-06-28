using DevExpress.XtraReports.UI;
using DocumentFormat.OpenXml.Vml;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using static NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.InitReport;
using static NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.Page_DuyetGiaMaster;
using System.Net.Http;
using Microsoft.AspNetCore.Components;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public partial class XtraRp_DuyetGiaDetailDauMau : DevExpress.XtraReports.UI.XtraReport
    {
       
        public XtraRp_DuyetGiaDetailDauMau()
        {
            InitializeComponent();
            
        }
        string typereport = "";
        public void setTypereport(string _typereport)
        {
           
            typereport= _typereport;

        }
        string img64 = "";
       
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        public void setInitList(List<NvlDuyetGiaItemShow> lst)
        {
            InitReport initReport = new InitReport();
            xrTableHeader.BeginInit();
            //xrTableGroup.BeginInit();
            xrTableDetail.BeginInit();
            float totalWidth = xrTableHeader.WidthF;
            //float totalWidthgroup = xrTableGroup.WidthF;
            float WidthCell = 240F;
            float pagewidth = this.PageWidth;

            DataTable dt = new DataTable();
            dt.Columns.Add("Serial", typeof(int));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("TenHang", typeof(string));
            dt.Columns.Add("DVT", typeof(string));
            dt.Columns.Add("XuatXu", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("SLDuToan", typeof(decimal));
            dt.Columns.Add("GiaDangMua", typeof(decimal));
            dt.Columns.Add("NhaCungCapDuyet", typeof(string));
            dt.Columns.Add("TinhTrangDuyet", typeof(string));
           
            //dt.Columns.Add("htmlreportDauMau", typeof(string));
            var query = lst.GroupBy(p => p.TenNCC).Select(p => new { key = prs.RandomString(10), TenNCC = p.Key.ToString() }).ToList();
            //Chuyển về tên cột dễ nhận dạng
            //string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "checkicon.png");
            //Image myImage = Image.FromFile(imagePath);
            foreach (var it in query)
            {
                dt.Columns.Add(string.Format("DG_{0}", it.key), typeof(decimal));//Sử dụng chuỗi html để tạo
                dt.Columns.Add(string.Format("DGDVT_{0}", it.key), typeof(decimal));
                dt.Columns.Add(string.Format("imgduyet_{0}", it.key), typeof(string));
            }

            var querylst = lst.OrderBy(p => p.Serial);
            int SerialOld = 0;
            foreach (var it in querylst)
            {
                if (SerialOld != it.Serial)
                {
                    SerialOld = it.Serial;
                    DataRow rownew = dt.NewRow();
                    rownew["Serial"] = it.Serial;
                    rownew["MaHang"] = it.MaHang;
                    rownew["TenHang"] = it.TenHang;
                    rownew["DVT"] = it.DVT;
                    rownew["XuatXu"] = it.XuatXu;
                    rownew["GhiChu"] = it.GhiChu;
                    rownew["SLDuToan"] = it.SLDuToan;
                    rownew["GiaDangMua"] = it.GiaDangMua;
                    rownew["NhaCungCapDuyet"] = it.NhaCungCapDuyet;
                    rownew["TinhTrangDuyet"] = it.TinhTrangDuyet;
                   
                    // rownew["Serial"] = it.Serial;
                    dt.Rows.Add(rownew);

                }

            }
            foreach (var it in lst)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (it.Serial == row.Field<int>("Serial"))
                    {
                        var keystring = query.Where(p => p.TenNCC == it.TenNCC).FirstOrDefault();
                        if (keystring != null)
                        {
                            
                            row[string.Format("DG_{0}", keystring.key)] = it.DonGia;
                         
                            row[string.Format("DGDVT_{0}", keystring.key)] = it.DonGiaTheoDvt;
                            if(it.boolduyet)
                            {
                                row[string.Format("imgduyet_{0}", keystring.key)] = ImgBase64();
                            }
                        
                                //Console.WriteLine(row[string.Format("imgduyet_{0}", keystring.key)].ToString());
                            
                            
                        }

                        break;
                    }
                }
            }

            int count = query.Count;
            int i = 0;
            List<FiledItem> lstitem = new List<FiledItem>();
            foreach (var it in query)
            {
                FiledItem item = new FiledItem();
                item.text = it.TenNCC;
                item.fieldName = string.Format("DG_{0}", it.key);
                item.fieldNamedecription=string.Format("DGDVT_{0}", it.key);
                item.fieldimgduyet = string.Format("imgduyet_{0}", it.key);
                lstitem.Add(item);
            }
            //Console.WriteLine(totalWidth);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XtraRp_DuyetGiaDetailDauMau));
            initReport.InitCellColumn(typereport,xrTableRowHeader1, xrTableRowHeader2, xrTableRowDetail, "ĐƠN GIÁ", lstitem, WidthCell, ref pagewidth, ref totalWidth, xrTenHangHeader.WidthF,resources);
           
            //Add thêm các cột phía sau pivot
            int phannguyenlast = (int)Math.Floor(totalWidth / pagewidth);
            float widthconlai = (phannguyenlast + 1) * pagewidth - totalWidth - 120;
            if (widthconlai > 0)
            {
                if (widthconlai > 120)
                {
                    if (widthconlai > 250)
                        widthconlai = 250;
                    initReport.InitCellMeargeSingle(xrTableRowHeader1, xrTableRowHeader2, xrTableRowDetail, "Ghi chú", "GhiChu", widthconlai);
                    totalWidth += widthconlai;
                }
            }
            
            xrTableHeader.WidthF = totalWidth;
            xrTableDetail.WidthF = totalWidth;
            //xrTableGroup.EndInit();
            xrTableDetail.EndInit();
            xrTableHeader.EndInit();
            this.DataSource = dt;
        }
        private string ImgBase64()
        {
            //Chuyển ảnh về dạng base string 64
            if(img64=="")
            {
                img64 = @"iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAAdcAAAHXAYySCGgAAAAZdEVYdFNvZnR3YXJlAHd3dy5pbmtzY2FwZS5vcmeb7jwaAAAEbklEQVR4Xu2aW2wUVRjHp3KrgRjbSEBIQB98MDwYAeGJUONegFZbhQaxlYKkUMrFtnIRlaZqaWkszM4WKiuX1hpabd1UCCEkBCVc0vaBSwgECqk2oSkg9BLfNISP8y2zZPfst7szu+vO6TD/5Pe05ztz/v/MnFsrWbJkyZIls6khI1Wqz0wjMb1cjqWSy36fARyPJNlWorYyoXY70yXF3kYYRx5Kim2l2tKEcr3zFjPZw5n28y8jV21pQsn2MmbwvwDDweYV+yK1pcnkXjiOGWzgDAfy0DcfmFKy82VmsJMzHMgjZn6V2tpkcjvfYAb7OMPByLa1amuTSXG+zb7pYdK0H8VRqbbWLw94xhwZlPNEJOtEoes5xRFusvPTIoGUotrRr/Yh+cXfhmQQjVXnN0KKQhoO5JRUkTtWtRKbRAyg4OwGyixPj1Sb8ZJqI3aJFsDyM+spsxy2f9gucIZqIT6JFMCHfxQTZjkUGy53Werw45coAeT+vpY2zDHbm9+iDj0xEiEArebnePPBOyR/rA49MTI6gNUdJaRZnon734Of7tZA+6ArXx16YmRkAGUXN2tZ6mBMnRNqb1X4akwTwNfXvoTRdQ7SME9RZ8nTOsMCqLpRDr88qCV/00vNzXIYV7eANMsz70hBUK0hAWy+tAVGuR0w6UA2VHeXk2204v7rW5jw/SLSLM+Uhhxo/vu7oPqkB1ByYROw/fjTQWEQy04Xg3dgN9k+Eo39O32TWaDJcOAbgmHxfSQ1gM/YJIWGqQG+dngJ1PdWknUU3oFdMKNlKdkXRVFXKdlP0gJofbAL0vZlkYPzk7pnARR3lYXUUtiPf0L2QTGr7SNoH6LfsKS+AeVXv4AUV/SZ+k024EP91WQfSGHHp2QdxQv7MqHhzk6yHyTpc0DmiUJyoDw48G1XPg+px9VD63KHbLm0NaSPQJIeQBv7FF758QNysBQZR1fAz/efzNw/3K7yBUO1o8DPhH8+T9IDQHA2Hqtx3UYmH8yBCrbRma4jOKzxBxcJQwJAtO7ZYwFXGtwcUc/lMSwAZC47jVEG4mXJqSLyeRSGBtB0pwbSPe+SJmJlKtvt4ZJLPY/C0ACQHTe2B+0M4wFPgtXd28nnhMPwAJCck2tIQ3rJPrma7D8SQgTwZGlcTJrSyqSD2WzW13+6FCIAxLc0up2kuWjgq4+fEtVvNIQJANF2hR0K7i6p/rQgVADtg7KuEx6S5smC5nvRNzzhECoAxHN7Bzy/V/sucevlyHv9aAgXALKOHYkpszwz2amRqteDkAEgs3/NI037Sd2zEPb3VZG1ehA2ALwPGF8f/q5v5bmNZJ1ehA0AWdNZSpqf1vi+70qMqtGL0AHgqvB6c+iqUHn9K7J9LAgdALK3tzJogzT/aPC9frwIHwCC1+ZoHpfHSHeFsTAiAsC/GbzatBhWnNtA/h4PIyIARPnzm4RNfIGMmAD+L6wArACsAKwA1KEnRs98AACQcmy4Pm2k0AoV8f1rrCVLliw925Kkxw49sORIO3KDAAAAAElFTkSuQmCC";
            }
                //img64 = @"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";
            return img64;
        }

       
    }
}
