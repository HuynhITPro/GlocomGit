using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_KeHoachMuaHang : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_KeHoachMuaHang()
        {
            InitializeComponent(); this.RequestParameters = false;
        }
       

        private void xrLabel11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {


        }
        int rowcount = 0;

        CultureInfo culture = new CultureInfo("vi-VN");
        private void bindingValueWithFormat(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell xRTableCell = (XRTableCell)sender;
            if (xRTableCell.Value == null)
            {
                return;
            }
            xRTableCell.Text = ((double)(xRTableCell.Value)).ToString("#,#.#", culture);
        }

        private void xrRichText1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRRichText richText = sender as XRRichText;

            RichEditDocumentServer docServer = new RichEditDocumentServer();
            docServer.Text = richText.Text;
            docServer.Document.DefaultParagraphProperties.LineSpacingType = ParagraphLineSpacing.Multiple;
            docServer.Document.DefaultParagraphProperties.LineSpacingMultiplier = 1.4F;
            richText.Text = docServer.RtfText;
        }
        public void setMaDeNghi(string MaDN, int Serial)
        {
            lbMaDeNghi.Text = string.Format("Đề nghị số: {0}", Serial);
            // lbdenghiso.Text = string.Format("Đề nghị số: {0}", Serial);
        }

        private void Detail_AfterPrint(object sender, EventArgs e)
        {
            rowcount++;
        }
        public bool checkHideHeader = true;
       

       
        public void setNoidung(string noidung)
        {


            RichEditDocumentServer docServer = new RichEditDocumentServer();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("- Yêu cầu: Chất lượng, quy cách, kích thước theo mẫu đã phê duyệt." + Environment.NewLine);
            stringBuilder.Append("- Đơn vị thực hiện: Phòng Vật tư." + Environment.NewLine);
            stringBuilder.Append("- Yêu cầu phụ kiện nhập kho đồng bộ đảm bảo tồn kho tối thiểu 2 tuần và không vượt quá 4 tuần cho KHSX tháng kế tiếp" + Environment.NewLine);
            stringBuilder.Append(noidung);
            xrRichText2.Text = stringBuilder.ToString();


        }
        public void setNguoiDuyet(string phongban, string nguoilap, string nguoikiem, string nguoiduyet, string DaDuyet, string NoiDungDeNghi)
        {
            if (NoiDungDeNghi == "")
                this.Parameters["KinhGui"].Value = string.Format("{0} đề nghị BGĐ duyệt cho mua vật tư phục vụ cho sản xuất như sau:", phongban);
            else
                this.Parameters["KinhGui"].Value = NoiDungDeNghi;
            //xrPhongBan.Text = string.Format("{0} đề nghị Ông duyệt cho mua phụ kiện nội địa phục vụ cho sản xuất như sau:", phongban);
            xrNguoiLap.Text = nguoilap;
            xrNguoiKiemTra.Text = nguoikiem;
            xrNguoiDuyet.Text = nguoiduyet;
            //if (DaDuyet == null || DaDuyet == "")
            //{
            //    this.Watermark.Text = "";

            //}
            //else
            //{
            //    if (DaDuyet.Replace(" ", "") != "")
            //    {
            //        this.Watermark.Text = "ĐÃ DUYỆT";

            //    }
            //}
        }

        private void xrNoiDung_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRRichText richText = sender as XRRichText;

            RichEditDocumentServer docServer = new RichEditDocumentServer();
            docServer.Text = richText.Text;
            docServer.Document.DefaultParagraphProperties.LineSpacingType = ParagraphLineSpacing.Multiple;
            docServer.Document.DefaultParagraphProperties.LineSpacingMultiplier = 1.4F;
            richText.Text = docServer.RtfText;
        }

      

       

        private void xrRichText2_BeforePrint(object sender, CancelEventArgs e)
        {
            XRRichText richText = sender as XRRichText;

            RichEditDocumentServer docServer = new RichEditDocumentServer();
            docServer.Text = richText.Text;
            docServer.Document.DefaultParagraphProperties.LineSpacingType = ParagraphLineSpacing.Multiple;
            docServer.Document.DefaultParagraphProperties.LineSpacingMultiplier = 1.4F;
            richText.Text = docServer.RtfText;
        }

        private void PageHeader_BeforePrint(object sender, CancelEventArgs e)
        {
            if ((rowcount >= this.RowCount && this.RowCount > 1))
            {

                e.Cancel = checkHideHeader;


            }
        }

        private void xrngaythangnam_BeforePrint(object sender, CancelEventArgs e)
        {
            string text = "";

            DateTime dti = DateTime.Now;
            text = string.Format("Nam Định, ngày {0} tháng {1} năm {2}", dti.Day, dti.Month, dti.Year);
            xrngaythangnam.Text = text;
        }

        private void ReportHeader_BeforePrint(object sender, CancelEventArgs e)
        {
            rowcount = 0;
        }
    }
}
