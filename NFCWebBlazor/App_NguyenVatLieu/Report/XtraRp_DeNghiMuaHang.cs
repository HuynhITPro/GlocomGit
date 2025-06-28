using ClosedXML.Graphics;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_DeNghiMuaHang : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_DeNghiMuaHang()
        {
            InitializeComponent();
            this.RequestParameters = false;
        }
       

        private void xrLabel11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {


        }
        int rowcount = 0;
        public bool checkHideHeader = true;
        //private void xrRichText1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        //{
        //    XRRichText richText = sender as XRRichText;

        //    RichEditDocumentServer docServer = new RichEditDocumentServer();
        //    docServer.Text = richText.Text;
        //    docServer.Document.DefaultParagraphProperties.LineSpacingType = ParagraphLineSpacing.Multiple;
        //    docServer.Document.DefaultParagraphProperties.LineSpacingMultiplier = 1.4F;
        //    richText.Text = docServer.RtfText;
        //}

        private void Detail_AfterPrint(object sender, EventArgs e)
        {
            rowcount++;
        }
        public void setMaDeNghi(string MaDN)
        {
            lbMaDeNghi.Text = string.Format("Đề nghị số: {0}", MaDN);
        }
       

     
        public void setNoidung(string noidung)
        {
            xrNoiDung.Text = noidung;
        }
        public void setNguoiDuyet(string phongban, string LoaiKeHoach, string nguoilap, string nguoikiem, string nguoiduyet, string DaDuyet, string NoiDungDeNghi,DateTime Ngay)
        {
            if (NoiDungDeNghi == "")
            {
                if (LoaiKeHoach == "DeNghiMuaHang")
                    this.Parameters["NoiDungDeNghi"].Value = string.Format("{0} đề nghị BGĐ duyệt cho mua vật tư phục vụ cho sản xuất như sau:", phongban);
                if (LoaiKeHoach == "DeNghiXuatHang")
                    this.Parameters["NoiDungDeNghi"].Value = string.Format("{0} đề nghị BGĐ duyệt cho mua vật tư phục vụ cho sản xuất như sau:", phongban);

            }
            else
                this.Parameters["NoiDungDeNghi"].Value = NoiDungDeNghi;
            xrNguoiLap.Text = nguoilap;
            xrNguoiKiemTra.Text = nguoikiem;
            xrNguoiDuyet.Text = nguoiduyet;
            xrngaythangnam.Text = string.Format("Đồng Nai, ngày {0} tháng {1} năm {2}", Ngay.Day, Ngay.Month, Ngay.Year); ;
        }
        private void ReportHeader_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            rowcount = 0;
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
            //string text = "";

            //DateTime dti = DateTime.Now;
            //text = string.Format("Đồng Nai, ngày {0} tháng {1} năm {2}", dti.Day, dti.Month, dti.Year);
            //xrngaythangnam.Text = text;
        }
    }
}
