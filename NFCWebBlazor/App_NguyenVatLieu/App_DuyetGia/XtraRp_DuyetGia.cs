using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public partial class XtraRp_DuyetGia : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_DuyetGia()
        {
            InitializeComponent();
            this.xrSubreport1.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("SerialLink", null, "Serial"));
        }
        public void setLoaiDuyetGia(string LoaiDuyetGia)
        {
            if(LoaiDuyetGia.Contains("Dầu màu"))
                xrtitle.Text = "BẢNG SO SÁNH DUYỆT GIÁ DẦU MÀU";
            else
                xrtitle.Text = "BẢNG SO SÁNH DUYỆT GIÁ";
        }
        private void lbngay_BeforePrint(object sender, CancelEventArgs e)
        {
            DateTime now=DateTime.Now;
            lbngay.Text = string.Format("Nhơn trạch, ngày {0} tháng {1} năm {2}",now.ToString("dd"), now.ToString("MM"), now.ToString("yyyy"));
        }
    }
}
