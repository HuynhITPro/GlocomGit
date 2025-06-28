using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_NhapXuatItem : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_NhapXuatItem()
        {
            InitializeComponent();
            this.FilterString = "[MaHang] = ?MaHang";
        }
    }
}
