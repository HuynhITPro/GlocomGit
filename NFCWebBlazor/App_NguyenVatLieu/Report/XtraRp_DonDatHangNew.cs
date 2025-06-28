using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_DonDatHangNew : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_DonDatHangNew()
        {
            InitializeComponent();
            this.xrSubreport1.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("SerialMaDH", null, "Serial"));
        }
      
        public void setNoiDung(Nullable<DateTime> ngayhieuluc, string SoHD, string noidungkinhgui, string DonHangSo)
        {
            //if (ngayhieuluc != null)
            //{
            //    this.Parameters["DienGiaiHopDong"].Value = string.Format("(Kèm theo hợp đồng số {0} ngày {1} tháng {2} năm {3})", SoHD, ngayhieuluc.Value.Day, ngayhieuluc.Value.Month, ngayhieuluc.Value.Year);
            //}
            //if (noidungkinhgui == "")
            //    this.Parameters["NoiDungKinhGui"].Value = string.Format("Công ty GLOCOM xin gửi tới quý công ty chi tiết đơn đặt hàng như sau:", DateTime.Now.ToString("yyyy"), DateTime.Now.AddYears(1).ToString("yyyy"), this.Parameters["DonDatHangSo"].Value);
            //else
            //    this.Parameters["NoiDungKinhGui"].Value = noidungkinhgui;
            //this.Parameters["DonDatHangSo"].Value = string.Format("{0}", DonHangSo);
        }
        public void setMaDeNghi(string madenghi)
        {
            lbMaDeNghi.Text = madenghi;
            //lbdenghiso.Text = string.Format("Đơn đặt hàng số: {0}", Serial);
        }

        public void setGhiChu(string ghichu, string DVTT)
        {
            xrNoiDung.Text = ghichu;

           
        }


        private void xrngaythangnam_BeforePrint(object sender, CancelEventArgs e)
        {
            string text = "";

            DateTime dti = DateTime.Now;
            text = string.Format("Nhơn Trạch, ngày {0} tháng {1} năm {2}", dti.Day, dti.Month, dti.Year);
            xrngaythangnam.Text = text;
        }

      
    }
}
