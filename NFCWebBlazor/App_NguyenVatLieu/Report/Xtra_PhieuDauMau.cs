using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class Xtra_PhieuDauMau : DevExpress.XtraReports.UI.XtraReport
    {
        public Xtra_PhieuDauMau()
        {
            InitializeComponent();
        }
        
       

        private void xrtitle_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {


        }
    }
}
