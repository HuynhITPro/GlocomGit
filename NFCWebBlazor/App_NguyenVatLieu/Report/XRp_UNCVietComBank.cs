using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.Report
{
    public partial class XRp_UNCVietComBank : DevExpress.XtraReports.UI.XtraReport
    {
        public XRp_UNCVietComBank()
        {
            InitializeComponent();
          
            //xrTableCell14.TextFormatString = "{0:n2}";

        }

        private void xrCheckBox1_BeforePrint(object sender, CancelEventArgs e)
        {
            object ob = xrCheckBox1.Value;
            if ((bool)(xrCheckBox1.Value) == true)
            {
                xrCheckBox1.Text = "";
                xrCheckBox1.Checked = true;
            }
        }
        public void setSoTK(string tkno, string tkco)
        {
            this.Parameters["SoTKNo"].Value = tkno;
            this.Parameters["SoTKCo"].Value = tkco;
        }
    }
}
