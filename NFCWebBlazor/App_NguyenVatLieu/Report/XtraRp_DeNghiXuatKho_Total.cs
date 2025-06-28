using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_DeNghiXuatKho_Total : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_DeNghiXuatKho_Total()
        {
            InitializeComponent();
        }
        public void setdiengiai(string diengiai)
        {
            xrlbdiengiai.Text= diengiai;
        }
    }
}
