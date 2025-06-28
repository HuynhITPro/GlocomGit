using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class Xtra_TheKhoTheoMaHang : DevExpress.XtraReports.UI.XtraReport
    {
        public Xtra_TheKhoTheoMaHang()
        {
            InitializeComponent();

           
           
        }
       
        public void setGhiChu(string ghichu)
        {
            lbghichu.Text = ghichu;
        }
        public void setTonKho(double slton,double? thanhtienton)
        {
            lbTonCuoi.Text = slton.ToString("#,#.##");
            if(thanhtienton != null)
            {
                lbThanhTien.Text = thanhtienton.Value.ToString("#,#");
            }
           
        }

      
     
    }
}
