using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class Xtra_InPhieuA5 : DevExpress.XtraReports.UI.XtraReport
    {
        public Xtra_InPhieuA5()
        {
            InitializeComponent();
        }
        public void setTieuDe(string TieuDe)
        {
            xrtitle.Text = TieuDe;
        }

       

        private void xrtitle_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {


        }
    }
}
