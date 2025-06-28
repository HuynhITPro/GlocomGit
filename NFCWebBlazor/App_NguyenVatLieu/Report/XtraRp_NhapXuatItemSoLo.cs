using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_NhapXuatItemSoLo : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_NhapXuatItemSoLo()
        {
            InitializeComponent();
            this.FilterString = "[MaHang] = ?MaHang";
        }
    }
}
