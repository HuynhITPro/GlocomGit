using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_DonDatHangItem : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_DonDatHangItem()
        {
            InitializeComponent();
            this.RequestParameters = false;
            this.FilterString = "[SerialMaDH] = ?SerialMaDH";
        }
        int rowcount = 0;
        public void setBangChu(string thanhtien, double Total)
        {
            xrBangChu.Text = thanhtien;
            xrTotalTT.Value = Total;
        }
        public void setGhiChu(string ghichu, string DVTT)
        {
            //xrNoiDung.Text = ghichu;

            lb_DVTTTotal.Text = DVTT;
        }
        public bool checkHideHeader = true;
        private void Detail_AfterPrint(object sender, EventArgs e)
        {
            rowcount++;
        }
        float Heightdetail = 70;

        private void ReportHeader_BeforePrint(object sender, CancelEventArgs e)
        {
            rowcount = 0;//Xử lý trường hợp này vì có thể sau khi load xong, người dùng thay đổi parameter, hàm beforeprint sẽ bị gọi lại 
        }
        private void PageHeader_BeforePrint(object sender, CancelEventArgs e)
        {
            //float d = 0;
            //if (float.TryParse(this.Parameters["ChieuCaoDong"].Value.ToString(), out d))
            //{
            //    Heightdetail = d;
            //    this.xrTableRow2.HeightF = Heightdetail;
            //    this.Detail.HeightF = Heightdetail;
            //}

            if ((rowcount >= this.RowCount && this.RowCount > 1))
            {

                e.Cancel = checkHideHeader;


            }
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
    }
}
