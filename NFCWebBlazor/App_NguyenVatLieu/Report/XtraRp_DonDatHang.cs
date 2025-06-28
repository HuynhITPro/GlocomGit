using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraRichEdit.API.Native;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_DonDatHang : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_DonDatHang()
        {
            InitializeComponent();
            this.RequestParameters = false;
        }
        int rowcount = 0;
        public void setNoiDung(Nullable<DateTime> ngayhieuluc, string SoHD, string noidungkinhgui, string DonHangSo)
        {
            if (ngayhieuluc != null)
            {
                this.Parameters["DienGiaiHopDong"].Value = string.Format("(Kèm theo hợp đồng số {0} ngày {1} tháng {2} năm {3})", SoHD, ngayhieuluc.Value.Day, ngayhieuluc.Value.Month, ngayhieuluc.Value.Year);
            }
            if (noidungkinhgui == "")
                this.Parameters["NoiDungKinhGui"].Value = string.Format("Công ty GLOCOM xin gửi tới quý công ty chi tiết đơn đặt hàng như sau:", DateTime.Now.ToString("yyyy"), DateTime.Now.AddYears(1).ToString("yyyy"), this.Parameters["DonDatHangSo"].Value);
            else
                this.Parameters["NoiDungKinhGui"].Value = noidungkinhgui;
            this.Parameters["DonDatHangSo"].Value = string.Format("{0}", DonHangSo);
        }
        public void setMaDeNghi(string KinhGui, int Serial)
        {
            lbMaDeNghi.Text = string.Format("Mã đơn hàng: {0}", Serial);
            this.Parameters["KinhGui"].Value = KinhGui;
            //lbdenghiso.Text = string.Format("Đơn đặt hàng số: {0}", Serial);
        }
        public bool checkHideHeader = true;
        private void Detail_AfterPrint(object sender, EventArgs e)
        {
            rowcount++;
        }


      

        public void setBangChu(string thanhtien, double Total)
        {
            xrBangChu.Text = thanhtien;
            xrTotalTT.Value = Total;
        }
        public void setGhiChu(string ghichu, string DVTT)
        {
            xrNoiDung.Text = ghichu;

            lb_DVTTTotal.Text = DVTT;
        }
      



     
        float Heightdetail = 70;

        private void ReportHeader_BeforePrint(object sender, CancelEventArgs e)
        {
            rowcount = 0;//Xử lý trường hợp này vì có thể sau khi load xong, người dùng thay đổi parameter, hàm beforeprint sẽ bị gọi lại 
        }

        private void xrNoiDung_BeforePrint(object sender, CancelEventArgs e)
        {
            XRRichText richText = sender as XRRichText;
        
            RichEditDocumentServer docServer = new RichEditDocumentServer();
            docServer.Text = richText.Text;
            docServer.Document.DefaultParagraphProperties.LineSpacingType = ParagraphLineSpacing.Multiple;
            docServer.Document.DefaultParagraphProperties.LineSpacingMultiplier = 1.4F;
            richText.Text = docServer.RtfText;
           
        }

        private void xrngaythangnam_BeforePrint(object sender, CancelEventArgs e)
        {
            string text = "";

            DateTime dti = DateTime.Now;
            text = string.Format("Nhơn Trạch, ngày {0} tháng {1} năm {2}", dti.Day, dti.Month, dti.Year);
            xrngaythangnam.Text = text;
        }

        private void PageHeader_BeforePrint(object sender, CancelEventArgs e)
        {
            float d = 0;
            if (float.TryParse(this.Parameters["ChieuCaoDong"].Value.ToString(), out d))
            {
                Heightdetail = d;
                this.xrTableRow2.HeightF = Heightdetail;
                this.Detail.HeightF = Heightdetail;
            }

            if ((rowcount >= this.RowCount && this.RowCount > 1))
            {

                e.Cancel = checkHideHeader;


            }
        }
    }
}
