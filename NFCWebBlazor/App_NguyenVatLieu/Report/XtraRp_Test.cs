using DevExpress.XtraReports.UI;
using NFCWebBlazor.App_ClassDefine;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XtraRp_Test : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_Test()
        {
            InitializeComponent();
        }
        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.ToLower(); // Chuyển toàn bộ chuỗi thành chữ thường
            return char.ToUpper(input[0]) + input.Substring(1); // Viết hoa chữ cái đầu
        }

        int rowcountoption = 0;
        string colorhex = "";
        private void xrTableCell16_BeforePrint(object sender, CancelEventArgs e)
        {
            colorhex = ((XRTableCell)sender).Text;

        }
        private void xrTableCell1_BeforePrint(object sender, CancelEventArgs e)
        {
            xrTableCell1.BackColor = ColorTranslator.FromHtml(colorhex);
            xrTableCell1.ForeColor = ColorTranslator.FromHtml(StaticClass.GetContrastColor(colorhex));
          
        }

        private void xrTableCell6_BeforePrint(object sender, CancelEventArgs e)
        {
            XRTableCell xRTableCell = sender as XRTableCell;
            xRTableCell.Text = CapitalizeFirstLetter(xRTableCell.Text);
        }
        private void xrTableCell4_BeforePrint(object sender, CancelEventArgs e)
        {
            XRTableCell xRTableCell = sender as XRTableCell;
            xRTableCell.Text = CapitalizeFirstLetter(xRTableCell.Text);
            rowcountoption = 0;
        }
        private void xrTableCell17_BeforePrint(object sender, CancelEventArgs e)
        {
            rowcountoption++;
            XRTableCell xRTableCell = sender as XRTableCell;
            xRTableCell.Text = string.Format("Lựa chọn {0}: ", rowcountoption);

        }
    }
}
