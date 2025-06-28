using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_NhapXuatItemIDTem : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_NhapXuatItemIDTem()
        {
            InitializeComponent();
            this.FilterString = "[MaHang] = ?MaHang";
        }
    }
}
