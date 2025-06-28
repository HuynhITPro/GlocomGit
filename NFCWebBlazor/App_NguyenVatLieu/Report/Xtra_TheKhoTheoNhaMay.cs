using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class Xtra_TheKhoTheoNhaMay : DevExpress.XtraReports.UI.XtraReport
    {
        public Xtra_TheKhoTheoNhaMay()
        {
            InitializeComponent();

           
           
        }
        public void setSubItem(XtraReport xtraReport)
        {
            this.xrSubItem.Dpi = 254F;
            this.xrSubItem.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubItem.Name = "xrSubItem";
            this.xrSubItem.ReportSource = xtraReport;
           // this.xrSubItem.ReportSource = new NFCWebBlazor.App_NguyenVatLieu.Report.XtraRp_NhapXuatItem();
            this.xrSubItem.SizeF = new System.Drawing.SizeF(2079F, 58.42F);
            this.xrSubItem.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("MaHang", null, "MaHang"));
        }
        public void setGhiChu(string ghichu)
        {
            lbghichu.Text = ghichu;
        }

      
        private void xrSubItem_BeforePrint(object sender, CancelEventArgs e)
        {
            var subReport = (XRSubreport)sender;

            // Lấy Report Instance của SubReport
            var subReportInstance = subReport.ReportSource;

            // Kiểm tra dữ liệu của SubReport
            var dataSource = subReportInstance.DataSource as IList; // Hoặc kiểu dữ liệu của bạn
            if (dataSource == null || dataSource.Count == 0) // Không có dữ liệu
            {
                subReport.Visible = false; // Ẩn SubReport
               
            }
            else
            {
                subReport.Visible = true; // Hiển thị SubReport nếu có dữ liệu
              
            }
        }
    }
}
