using DevExpress.DataProcessing.InMemoryDataProcessor.GraphGenerator;
using DevExpress.XtraReports.UI;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public partial class XtraRp_DuyetGiaDetail : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_DuyetGiaDetail()
        {
            InitializeComponent();

            setInitList();

        }
        public void setInitList()
        {
            InitReport initReport = new InitReport();
            xrTableHeader.BeginInit();
            xrTableGroup.BeginInit();
            xrTableDetail.BeginInit();
            float totalWidth = xrTableHeader.WidthF;
            float totalWidthgroup = xrTableGroup.WidthF;
            float WidthCell = 190F;
           
            for (int i = 0; i < 10; i++)
            {
                totalWidth += 2 * WidthCell;
                initReport.InitCellMearge(xrTableRowHeader1, xrTableRowHeader2, xrTableRowDetail,
                    string.Format("Tên NCC {0}", i), string.Format("Don giá {0}", i), string.Format("Đơn giá{0}/M2", i), "DonGia", "DonGiaTheoDVT",WidthCell);
              
            }
            
            xrTableHeader.WidthF = totalWidth;
            float cellgroupadd = totalWidth - totalWidthgroup;
            if (cellgroupadd > 0)
            {

                DevExpress.XtraReports.UI.XRTableCell xrcelllastgroup = new DevExpress.XtraReports.UI.XRTableCell();
                xrcelllastgroup.Dpi = 254F;
                xrcelllastgroup.Multiline = true;
                xrcelllastgroup.Name = "Testgroup";
                xrcelllastgroup.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));
                xrcelllastgroup.WidthF = cellgroupadd;
                xrTableRowGroup.Cells.Add(xrcelllastgroup);
            }
            xrTableGroup.WidthF = totalWidth;
            xrTableDetail.WidthF = totalWidth;
            xrTableGroup.EndInit();
            xrTableDetail.EndInit();
            xrTableHeader.EndInit();
        }
        public XRTableRow getxrTableRowHeader1()
        {
            return xrTableRowHeader1;
        }
        public XRTableRow getxrTableRowHeader2()
        {
            return xrTableRowHeader2;
        }
        public XRTableRow getxrTableRowDetail()
        {
            return xrTableRowDetail;
        }
        public XRTable getTableHeader()
        {
            return xrTableHeader;
        }
        public XRTable getTableDetail()
        {
            return xrTableDetail;
        }
        public float getTableWidth()
        {
            return xrTableHeader.WidthF;
        }
    }
}
