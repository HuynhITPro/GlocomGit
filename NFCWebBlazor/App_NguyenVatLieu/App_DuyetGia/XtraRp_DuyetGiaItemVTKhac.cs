using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using static NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.InitReport;
using static NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.Page_DuyetGiaMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public partial class XtraRp_DuyetGiaItemVTKhac : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_DuyetGiaItemVTKhac()
        {
            InitializeComponent();
        }
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        public void setInitList(List<NvlDuyetGiaItemShow> lst)
        {
            InitReport initReport = new InitReport();
            xrTableHeader.BeginInit();
            //xrTableGroup.BeginInit();
            xrTableDetail.BeginInit();
            float totalWidth = xrTableHeader.WidthF;
            //float totalWidthgroup = xrTableGroup.WidthF;
            float WidthCell = 220F;
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

            var query = lst.GroupBy(p => p.TenNCC).Select(p => new { key = prs.RandomString(10), TenNCC = p.Key.ToString() }).ToList();
            //Chuyển về tên cột dễ nhận dạng

            foreach (var it in query)
            {
                dt.Columns.Add(string.Format("DG_{0}", it.key), typeof(decimal));
                //dt.Columns.Add(string.Format("DGDVT_{0}", it.key), typeof(decimal));
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
                            //row[string.Format("DGDVT_{0}", keystring.key)] = it.DonGiaTheoDvt;
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
                FiledItem item= new FiledItem();
                item.text = it.TenNCC;
                item.fieldName = string.Format("DG_{0}",it.key);
                lstitem.Add(item);
            }
            Console.WriteLine(totalWidth);
            initReport.InitCellMeargeMultiColumn(xrTableRowHeader1, xrTableRowHeader2, xrTableRowDetail, "ĐƠN GIÁ", lstitem, WidthCell,ref pagewidth,ref totalWidth,xrTenHangHeader.WidthF);
            //totalWidth += lstitem.Count * WidthCell;
            //foreach (var it in query)
            //{

            //    int phannguyen = (int)Math.Floor(totalWidth / pagewidth);

            //    totalWidth += 2 * WidthCell;
            //    initReport.InitCellMeargeMultiColumn(xrTableRowHeader1, xrTableRowHeader2, xrTableRowDetail,
            //        string.Format("{0}", it.TenNCC), "Đơn giá", "Đơn giá/M2", string.Format("DG_{0}", it.key), string.Format("DGDVT_{0}", it.key), WidthCell);

            //    if (i < count - 1)//Kiểm trả xem cell kế tiếp có chịu nổi không
            //    {
            //        // Console.WriteLine(string.Format("{0}", +totalWidth));
            //        if (totalWidth + 2 * WidthCell > ((phannguyen + 1) * pagewidth - 30))
            //        {
            //            // Console.WriteLine(string.Format("Thỏa ở đây:{0}",+totalWidth));
            //            float widthadd = ((phannguyen + 1) * pagewidth) - totalWidth;
            //            initReport.InitCellMeargeBlank(xrTableRowHeader1, xrTableRowHeader2, xrTableRowDetail, ((phannguyen + 1) * pagewidth) - totalWidth);

            //            totalWidth += widthadd;
            //        }
            //    }
            //    i++;
            //}

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
            Console.WriteLine(totalWidth);
            xrTableHeader.WidthF = totalWidth;
            
            //float cellgroupadd = totalWidth - totalWidthgroup;
            //if (cellgroupadd > 0)
            //{

            //    DevExpress.XtraReports.UI.XRTableCell xrcelllastgroup = new DevExpress.XtraReports.UI.XRTableCell();
            //    xrcelllastgroup.Dpi = 254F;
            //    xrcelllastgroup.Multiline = true;
            //    xrcelllastgroup.Name = "Testgroup";
            //    xrcelllastgroup.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));
            //    xrcelllastgroup.WidthF = cellgroupadd;
            //    //xrTableRowGroup.Cells.Add(xrcelllastgroup);
            //}

            //xrTableGroup.WidthF = totalWidth;
            xrTableDetail.WidthF = totalWidth;
            //xrTableGroup.EndInit();
            xrTableDetail.EndInit();
            xrTableHeader.EndInit();
            this.DataSource = dt;
        }
    }
}
