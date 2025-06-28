using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_BangKeTongHop : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_BangKeTongHop()
        {
            InitializeComponent();
        }
        public void setGhiChu(string ghiChu)
        {
            lbGhiChu.Text = ghiChu;
        }
        string previousValue = string.Empty;
        private void xrTableCell10_BeforePrint(object sender, CancelEventArgs e)
        {
         
            var cell = sender as XRTableCell;
            //Console.WriteLine(cell.Text);
            if (cell != null)
            {
                if (cell.Text == previousValue)
                {
                    cell.Visible = false; // Dòng này có thể gây ẩn nhầm
                }
               else
                {
                    cell.Visible = true;
                  
                }
                    
            }
           
            //Console.WriteLine(string.Format("Cũ {0}, mới {1}", previousValue, cell.Text));
            previousValue = cell.Text;
           
        }
    }
}
