﻿using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_BaoCaoTonKho : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_BaoCaoTonKho()
        {
            InitializeComponent();
        }
        public void setGhiChu(string ghichu)
        {
            lbghichu.Text= ghichu;
        }
    }
}
