using ClosedXML.Graphics;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_DeNghiCapVatTuViTri : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_DeNghiCapVatTuViTri()
        {
            InitializeComponent();
            this.RequestParameters = false;
           
        }
       
     



        private void xrLabel11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {


        }
        public float Heightdetail = 70;

        int rowcount = 0;
        private void xrRichText1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //XRRichText richText = sender as XRRichText;

            //RichEditDocumentServer docServer = new RichEditDocumentServer();
            //docServer.Text = richText.Text;

            //Font font = new Font("Times New Roman", 12, FontStyle.Regular);
            ////CharacterProperties props = docServer.Document.BeginUpdateCharacters(docServer.Document.Range);
            ////props.FontName = font.FontFamily.Name;
            ////props.FontSize = font.Size;
            ////props.Bold = font.Bold;
            //// props.Italic = font.Italic;
            //docServer.Document.DefaultParagraphProperties.LineSpacingType = ParagraphLineSpacing.Multiple;
            //docServer.Document.DefaultParagraphProperties.LineSpacingMultiplier = 1.4F;
            ////docServer.Document.EndUpdateCharacters(props);


            //richText.Text = docServer.RtfText;
            //richText.Font = font;
        }


        public void setMaDeNghi(string MaDN)
        {
            lbMaDeNghi.Text = string.Format("Đề nghị số: {0}", MaDN);
        }
        public bool checkHideHeader = true;



        public void setNoidung(string noidung)
        {
            
            DateTime dti = DateTime.Now;
            string text = "";
            text = string.Format("Đồng Nai, ngày {0} tháng {1} năm {2}", dti.Day, dti.Month, dti.Year);
            this.Parameters["NgayThangNam"].Value = text;
        }
        public void setTotal(double SoLuong, double ThanhTien)
        {
            //xrTotalSL.Value = SoLuong;
            //xrToTalTT.Value = ThanhTien;
        }
        public void setNguoiDuyet(string phongban, string LoaiKeHoach, string nguoilap, string nguoikiem, string nguoiduyet, string DaDuyet, string NoiDungDeNghi,DateTime? Ngay)
        {
            if (string.IsNullOrEmpty(NoiDungDeNghi))
            {
                if (LoaiKeHoach == "DeNghiMuaHang")
                    this.Parameters["NoiDungDeNghi"].Value = string.Format("{0}", NoiDungDeNghi);
                if (LoaiKeHoach == "DeNghiXuatHang")
                    this.Parameters["NoiDungDeNghi"].Value = string.Format("{0}", NoiDungDeNghi);
            }
            else
                this.Parameters["NoiDungDeNghi"].Value = NoiDungDeNghi;
            this.Parameters["NguoiKiemTra2"].Value = "BỘ PHẬN CHUYÊN TRÁCH";
            xrPhongBanDeNghi.Text= string.Format("Bộ phận/ khu vực sản xuất: {0}", phongban);

            xrLabel15.Text = nguoikiem;
            xrNguoiLap.Text = nguoilap;
            xrNguoiDuyet.Text = nguoiduyet;
            if (!string.IsNullOrEmpty(nguoiduyet))
            {
                xrPictureBox1.Visible = true;
            }
            else
                xrPictureBox1.Visible = false;
            DateTime dateTime= DateTime.Now;
            if (Ngay != null)
                dateTime = Ngay.Value;
            string text = string.Format("Đồng Nai, ngày {0} tháng {1} năm {2}", dateTime.Day, dateTime.Month, dateTime.Year);
            this.Parameters["NgayThangNam"].Value = text;
            //xrNguoiDuyet.Text = nguoiduyet;
            this.Watermark.Text = "";
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {


            // this.Detail.HeightF = Heightdetail;
        }

      

       
        private void Detail_AfterPrint(object sender, EventArgs e)
        {
            rowcount++;
        }

        private void PageHeader_BeforePrint_1(object sender, CancelEventArgs e)
        {
            if ((rowcount >= this.RowCount && this.RowCount > 1))
            {
                e.Cancel = checkHideHeader;

            }
        }

        private void ReportHeader_BeforePrint(object sender, CancelEventArgs e)
        {
            rowcount = 0;
        }
    }
}
