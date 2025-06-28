using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Import.Html;
using System.ComponentModel;



namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public class InitReport
    {
        public class cellXtra
        {
            public string Text { get; set; }
            public string Name { get; set; }
            public string FieldName { get; set; }

        }
        public string RandomString(int length)
        {

            Random random = new Random();
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[random.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }

        public void InitCellMearge(DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader1
            , DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader2, DevExpress.XtraReports.UI.XRTableRow xrTabledetail
            , string headermerge, string header1, string header2, string FieldNameCell1, string FieldNameCell2, float WidthCell)
        {

            DevExpress.XtraReports.UI.XRTableCell xrHeadermearge1 = new DevExpress.XtraReports.UI.XRTableCell();
            DevExpress.XtraReports.UI.XRTableCell xrHeaderItem1 = new DevExpress.XtraReports.UI.XRTableCell();
            DevExpress.XtraReports.UI.XRTableCell xrHeaderItem2 = new DevExpress.XtraReports.UI.XRTableCell();
            DevExpress.XtraReports.UI.XRTableCell xrCellItem1 = new DevExpress.XtraReports.UI.XRTableCell();
            DevExpress.XtraReports.UI.XRTableCell xrCellItem2 = new DevExpress.XtraReports.UI.XRTableCell();

            // xrHeadermearge1
            // 
            xrHeadermearge1.Dpi = 254F;
            xrHeadermearge1.Multiline = true;
            xrHeadermearge1.Name = RandomString(9);
            xrHeadermearge1.StylePriority.UseTextAlignment = false;
            xrHeadermearge1.Text = headermerge;
            xrHeadermearge1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            xrHeadermearge1.WidthF = 2 * WidthCell;
            // 
            // xrHeaderItem1
            // T
            xrHeaderItem1.Dpi = 254F;
            xrHeaderItem1.Multiline = true;
            xrHeaderItem1.Name = RandomString(9);
            xrHeaderItem1.StylePriority.UseTextAlignment = false;
            xrHeaderItem1.Text = header1;
            xrHeaderItem1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            xrHeaderItem1.WidthF = WidthCell;
            //xrHeaderItem1.WidthF = 150F;
            // 
            // xrHeaderItem2
            // 
            xrHeaderItem2.Dpi = 254F;
            xrHeaderItem2.Multiline = true;
            xrHeaderItem2.Name = RandomString(9);
            xrHeaderItem2.StylePriority.UseTextAlignment = false;
            xrHeaderItem2.Text = header2;
            xrHeaderItem2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            //xrHeaderItem2.WidthF = 150F;
            xrHeaderItem2.WidthF = WidthCell;
            // 
            // xrCellItem1
            // 
            xrCellItem1.Dpi = 254F;
            xrCellItem1.Multiline = true;
            xrCellItem1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", FieldNameCell1)});
            xrCellItem1.Name = RandomString(9);
            xrCellItem1.TextFormatString = "{0:#,#.##}";
            xrCellItem1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            //xrCellItem1.WidthF = 150F;
            xrCellItem1.WidthF = WidthCell;
            // 
            // xrCellItem2
            // 
            xrCellItem2.Dpi = 254F;
            xrCellItem2.Multiline = true;
            xrCellItem2.WidthF = WidthCell;
            xrCellItem2.TextFormatString = "{0:#,#.##}";
            xrCellItem2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", FieldNameCell2)});
            xrCellItem2.Name = RandomString(9);
            xrCellItem2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;

            //xrCellItem2.Weight = 1D;

            xrHeadermearge1.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right)));
            xrHeaderItem1.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right)
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            xrHeaderItem2.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right)
            | DevExpress.XtraPrinting.BorderSide.Bottom)));

            xrCellItem1.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));
            xrCellItem2.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));

            xrHeadermearge1.Font = new DevExpress.Drawing.DXFont("Times New Roman", 11F);
            xrHeaderItem1.Font = new DevExpress.Drawing.DXFont("Times New Roman", 11F);
            xrHeaderItem2.Font = new DevExpress.Drawing.DXFont("Times New Roman", 11F);
            xrCellItem1.Font = new DevExpress.Drawing.DXFont("Times New Roman", 11F);
            xrCellItem2.Font = new DevExpress.Drawing.DXFont("Times New Roman", 11F);
            //xrHeadermearge1.CanGrow = false;
            //xrHeaderItem1.CanGrow = false;
            //xrHeaderItem2.CanGrow = false;
            xrCellItem1.CanGrow = false;
            xrCellItem2.CanGrow = false;

            //xrHeadermearge1.CanShrink = true;
            //xrHeaderItem1.CanShrink = true;
            //xrHeaderItem2.CanShrink = true;
            xrCellItem1.CanShrink = true;
            xrCellItem2.CanShrink = true;
            xrCellItem1.TextFitMode = DevExpress.XtraReports.UI.TextFitMode.ShrinkOnly;
            xrCellItem2.TextFitMode = DevExpress.XtraReports.UI.TextFitMode.ShrinkOnly;
            xRTableRowHeader1.Cells.Add(xrHeadermearge1);
            xRTableRowHeader2.Cells.Add(xrHeaderItem1);
            xRTableRowHeader2.Cells.Add(xrHeaderItem2);
            xrTabledetail.Cells.Add(xrCellItem1);
            xrTabledetail.Cells.Add(xrCellItem2);
        }

        public void InitCellMeargeBlank(DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader1
            , DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader2, DevExpress.XtraReports.UI.XRTableRow xrTabledetail, float WidthCell)
        {
            DevExpress.XtraReports.UI.XRTableCell xrHeadermearge1 = new DevExpress.XtraReports.UI.XRTableCell();
            DevExpress.XtraReports.UI.XRTableCell xrHeaderItem1 = new DevExpress.XtraReports.UI.XRTableCell();
            DevExpress.XtraReports.UI.XRTableCell xrHeaderItem2 = new DevExpress.XtraReports.UI.XRTableCell();
            DevExpress.XtraReports.UI.XRTableCell xrCellItem1 = new DevExpress.XtraReports.UI.XRTableCell();
            DevExpress.XtraReports.UI.XRTableCell xrCellItem2 = new DevExpress.XtraReports.UI.XRTableCell();

            // xrHeadermearge1
            // 
            xrHeadermearge1.Dpi = 254F;
            xrHeadermearge1.Multiline = true;
            xrHeadermearge1.Name = RandomString(9);
            xrHeadermearge1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            xrHeadermearge1.WidthF = WidthCell;
            // 
            // xrHeaderItem1
            // T
            xrHeaderItem1.Dpi = 254F;

            xrHeaderItem1.Name = RandomString(9);


            xrHeaderItem1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            xrHeaderItem1.WidthF = WidthCell / 2;
            //xrHeaderItem1.WidthF = 150F;
            // 
            // xrHeaderItem2
            // 
            xrHeaderItem2.Dpi = 254F;
            xrHeaderItem2.Multiline = true;
            xrHeaderItem2.Name = RandomString(9);
            //xrHeaderItem2.WidthF = 150F;
            xrHeaderItem2.WidthF = WidthCell / 2;
            // 
            // xrCellItem1
            // 
            xrCellItem1.Dpi = 254F;
            xrCellItem1.Multiline = true;

            xrCellItem1.Name = RandomString(9);

            //xrCellItem1.WidthF = 150F;
            xrCellItem1.WidthF = WidthCell / 2;
            xrCellItem2.Dpi = 254F;
            xrCellItem2.Multiline = true;
            xrCellItem2.WidthF = WidthCell / 2;
            xrCellItem2.TextFormatString = "{0:#,#.##}";

            xrCellItem2.Name = RandomString(9);

            //xrHeadermearge1.CanGrow = false;
            //xrHeaderItem1.CanGrow = false;
            xRTableRowHeader1.Cells.Add(xrHeadermearge1);
            xRTableRowHeader2.Cells.Add(xrHeaderItem1);
            xRTableRowHeader2.Cells.Add(xrHeaderItem2);
            xrTabledetail.Cells.Add(xrCellItem1);
            xrTabledetail.Cells.Add(xrCellItem2);
        }
        public void InitCellMeargeSingle(DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader1
           , DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader2, DevExpress.XtraReports.UI.XRTableRow xrTabledetail
           , string headermerge, string FieldNameCell, float WidthCell)
        {
            DevExpress.XtraReports.UI.XRTableCell xrHeadermearge1 = new DevExpress.XtraReports.UI.XRTableCell();
            xrHeadermearge1.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right)
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            xrHeadermearge1.Dpi = 254F;
            xrHeadermearge1.Multiline = true;
            xrHeadermearge1.Name = RandomString(10);
            xrHeadermearge1.RowSpan = 2;
            xrHeadermearge1.StylePriority.UseBorders = false;
            xrHeadermearge1.StylePriority.UseTextAlignment = false;
            xrHeadermearge1.Text = headermerge;
            xrHeadermearge1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            xrHeadermearge1.WidthF = WidthCell;

            DevExpress.XtraReports.UI.XRTableCell xrHeaderItem1 = new DevExpress.XtraReports.UI.XRTableCell();
            xrHeaderItem1.Dpi = 254F;
            xrHeaderItem1.WidthF = WidthCell;

            DevExpress.XtraReports.UI.XRTableCell xrCellItem1 = new DevExpress.XtraReports.UI.XRTableCell();
            xrCellItem1.Dpi = 254F;
            xrCellItem1.Multiline = true;
            xrCellItem1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", FieldNameCell)});
            xrCellItem1.Name = RandomString(9);

            xrCellItem1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            //xrCellItem1.WidthF = 150F;
            xrCellItem1.WidthF = WidthCell;

            xrHeadermearge1.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top|DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));


            xrCellItem1.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));

            xRTableRowHeader1.Cells.Add(xrHeadermearge1);
            xRTableRowHeader2.Cells.Add(xrHeaderItem1);
            xrTabledetail.Cells.Add(xrCellItem1);
        }
        public void InitCellMeargeSingleRepaet(DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader1
           , DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader2, DevExpress.XtraReports.UI.XRTableRow xrTabledetail
           , string headermerge, string FieldNameCell, float WidthCell)
        {
            //DevExpress.XtraReports.UI.XRTableCell xrHeadermearge1 = new DevExpress.XtraReports.UI.XRTableCell();

            //xrHeadermearge1.Dpi = 254F;
            //xrHeadermearge1.Multiline = true;
            //xrHeadermearge1.Name = RandomString(10);
            //xrHeadermearge1.RowSpan = 2;
            //xrHeadermearge1.StylePriority.UseBorders = false;
            //xrHeadermearge1.StylePriority.UseTextAlignment = false;
            //xrHeadermearge1.Text = headermerge;
            //xrHeadermearge1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            //xrHeadermearge1.WidthF = WidthCell;

            DevExpress.XtraReports.UI.XRTableCell xrHeaderItem1 = new DevExpress.XtraReports.UI.XRTableCell();
            xrHeaderItem1.Dpi = 254F;
            xrHeaderItem1.WidthF = WidthCell;
            xrHeaderItem1.Text = headermerge;
            xrHeaderItem1.Name = RandomString(10);

            DevExpress.XtraReports.UI.XRTableCell xrCellItem1 = new DevExpress.XtraReports.UI.XRTableCell();
            xrCellItem1.Dpi = 254F;
            xrCellItem1.Multiline = true;
            xrCellItem1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", FieldNameCell)});
            xrCellItem1.Name = RandomString(9);

            xrCellItem1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            //xrCellItem1.WidthF = 150F;
            xrCellItem1.WidthF = WidthCell;

            //xrHeadermearge1.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom| DevExpress.XtraPrinting.BorderSide.Left)));

            xrHeaderItem1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            xrHeaderItem1.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Left)));

            xrCellItem1.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Left)));

            //xRTableRowHeader1.Cells.Add(xrHeadermearge1);
            xRTableRowHeader2.Cells.Add(xrHeaderItem1);
            xrTabledetail.Cells.Add(xrCellItem1);
        }
        public class FiledItem
        {
            public string text { get; set; }
            public string fieldName { get; set; }
            public string fieldNamedecription { get; set; }
            public string fieldimgduyet{ get; set; }
        }
        public void InitCellColumn(string tyepreport,DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader1
           , DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader2, DevExpress.XtraReports.UI.XRTableRow xrTabledetail
           , string headermerge, List<FiledItem> lstfileds, float WidthCell, ref float pagewidth, ref float totalWidth, float columnrepeatWidth, System.ComponentModel.ComponentResourceManager resources)
        {

            DevExpress.XtraReports.UI.XRTableCell xrHeadermearge1 = new DevExpress.XtraReports.UI.XRTableCell();

            bool firstpage = false;
            int count = lstfileds.Count;
            xrHeadermearge1.Dpi = 254F;
            xrHeadermearge1.Multiline = true;
            xrHeadermearge1.Name = RandomString(9);
            xrHeadermearge1.StylePriority.UseTextAlignment = false;
            xrHeadermearge1.Text = headermerge;
            xrHeadermearge1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            xrHeadermearge1.Font = new DevExpress.Drawing.DXFont("Times New Roman", 11F);
            //xrHeadermearge1.WidthF = WidthCell;
            //xrHeadermearge1.WidthF = count * WidthCell;
            xrHeadermearge1.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right)));


            int i = 0;
            float widthadd = 0;
            float totalwidthbegin = totalWidth;
            foreach (var item in lstfileds)
            {
                totalWidth += WidthCell;
                widthadd = 0;
                firstpage = false;

                if (i < count - 1)//Kiểm trả xem cell kế tiếp có chịu nổi không
                {
                    int phannguyen = (int)Math.Floor(totalWidth / pagewidth);
                    // Console.WriteLine(string.Format("{0}", +totalWidth));
                    if (totalWidth + WidthCell > ((phannguyen + 1) * pagewidth - 30))
                    {
                        //hết trang rồi
                        firstpage = true;
                        // Console.WriteLine(string.Format("Thỏa ở đây:{0}",+totalWidth));
                        widthadd = ((phannguyen + 1) * pagewidth) - totalWidth - 125;
                        //Console.WriteLine(widthadd);
                        if (widthadd > 0)
                        {
                            //xrTabledetail.WidthF+= widthadd;
                            totalWidth += widthadd;
                            //WidthCell += widthadd;//Tăng kích thước lên để phủ hết trang
                            //xrHeadermearge1.WidthF += widthadd;
                        }
                        else
                            widthadd = 0;
                    }
                }

                DevExpress.XtraReports.UI.XRTableCell xrHeaderItem = new DevExpress.XtraReports.UI.XRTableCell();

                xrHeaderItem.Dpi = 254F;
                xrHeaderItem.Multiline = true;
                xrHeaderItem.Name = RandomString(9);
                xrHeaderItem.StylePriority.UseTextAlignment = false;
                xrHeaderItem.Text = item.text;
                xrHeaderItem.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                xrHeaderItem.WidthF = WidthCell + widthadd;
                xrHeaderItem.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right)
                | DevExpress.XtraPrinting.BorderSide.Bottom)));

                DevExpress.XtraReports.UI.XRTableCell xrCellItem = new DevExpress.XtraReports.UI.XRTableCell();
                xrCellItem.Dpi = 254F;
                xrCellItem.Multiline = true;
                xrCellItem.WidthF = WidthCell + widthadd;
                XRPanel xRPanel;
                //xrCellItem.Text= item.text;
                if (tyepreport.Contains("Dầu màu"))
                {
                     xRPanel = InitXrpanel(resources, item);
                }
                else
                {
                    xRPanel = InitXrpanelVTKhac(resources, item);
                }
                xrCellItem.Controls.Add(xRPanel);

                xRPanel.Borders = DevExpress.XtraPrinting.BorderSide.None;
              
                xrCellItem.Name = RandomString(9);
                xrCellItem.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
                xrCellItem.Font = new DevExpress.Drawing.DXFont("Times New Roman", 12F);
                xrCellItem.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));

                //xrCellItem.CanGrow = false;
                //xrCellItem.CanShrink = true;
                //xrCellItem.TextFitMode = DevExpress.XtraReports.UI.TextFitMode.ShrinkOnly;

                xRTableRowHeader2.Cells.Add(xrHeaderItem);

                xrTabledetail.Cells.Add(xrCellItem);
                if (firstpage)//Tức là cột cuối cùng rồi, bắt đầu đầu trang thì phải thêm cột lặp lại
                {
                    totalWidth += columnrepeatWidth;
                    xrHeadermearge1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;//Chuyển chữ về đầu, không thôi dễ bị mất chữ ở 2 trang
                    InitCellMeargeSingleRepaet(xRTableRowHeader1, xRTableRowHeader2, xrTabledetail, "Tên hàng", "TenHang", columnrepeatWidth);

                }
                i++;

            }

            xrHeadermearge1.WidthF = totalWidth - totalwidthbegin;
            xRTableRowHeader1.Cells.Add(xrHeadermearge1);


        }
        private void xrPictureBox_BeforePrint(object sender, CancelEventArgs e)
        {
            XRPictureBox xRPictureBox = (XRPictureBox)sender;
            string s="";
            //xRPictureBox.ImageSource = new DevExpress.XtraPrinting.Drawing.ImageSource("img", @"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");
            //xRPictureBox.I
        }
      

        public void InitCellMeargeMultiColumn(DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader1
          , DevExpress.XtraReports.UI.XRTableRow xRTableRowHeader2, DevExpress.XtraReports.UI.XRTableRow xrTabledetail
          , string headermerge, List<FiledItem> lstfileds, float WidthCell, ref float pagewidth, ref float totalWidth, float columnrepeatWidth)
        {

            DevExpress.XtraReports.UI.XRTableCell xrHeadermearge1 = new DevExpress.XtraReports.UI.XRTableCell();

            bool firstpage = false;
            int count = lstfileds.Count;
            xrHeadermearge1.Dpi = 254F;
            xrHeadermearge1.Multiline = true;
            xrHeadermearge1.Name = RandomString(9);
            xrHeadermearge1.StylePriority.UseTextAlignment = false;
            xrHeadermearge1.Text = headermerge;
            xrHeadermearge1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            xrHeadermearge1.Font = new DevExpress.Drawing.DXFont("Times New Roman", 11F);
            //xrHeadermearge1.WidthF = WidthCell;
            //xrHeadermearge1.WidthF = count * WidthCell;
            xrHeadermearge1.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right)));


            int i = 0;
            float widthadd = 0;
            float totalwidthbegin = totalWidth;
            foreach (var item in lstfileds)
            {
                totalWidth += WidthCell;
                widthadd = 0;
                firstpage = false;

                if (i < count - 1)//Kiểm trả xem cell kế tiếp có chịu nổi không
                {
                    int phannguyen = (int)Math.Floor(totalWidth / pagewidth);
                    // Console.WriteLine(string.Format("{0}", +totalWidth));
                    if (totalWidth + WidthCell > ((phannguyen + 1) * pagewidth - 30))
                    {
                        //hết trang rồi
                        firstpage = true;
                        // Console.WriteLine(string.Format("Thỏa ở đây:{0}",+totalWidth));
                        widthadd = ((phannguyen + 1) * pagewidth) - totalWidth - 125;
                      
                        if (widthadd > 0)
                        {
                            //xrTabledetail.WidthF+= widthadd;
                            totalWidth += widthadd;
                            //WidthCell += widthadd;//Tăng kích thước lên để phủ hết trang
                            //xrHeadermearge1.WidthF += widthadd;
                        }
                        else
                            widthadd = 0;
                    }
                }

                DevExpress.XtraReports.UI.XRTableCell xrHeaderItem = new DevExpress.XtraReports.UI.XRTableCell();

                xrHeaderItem.Dpi = 254F;
                xrHeaderItem.Multiline = true;
                xrHeaderItem.Name = RandomString(9);
                xrHeaderItem.StylePriority.UseTextAlignment = false;
                xrHeaderItem.Text = item.text;
                xrHeaderItem.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                xrHeaderItem.WidthF = WidthCell + widthadd;
                xrHeaderItem.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right)
                | DevExpress.XtraPrinting.BorderSide.Bottom)));

                DevExpress.XtraReports.UI.XRTableCell xrCellItem = new DevExpress.XtraReports.UI.XRTableCell();
                xrCellItem.Dpi = 254F;
                xrCellItem.Multiline = true;
                xrCellItem.WidthF = WidthCell + widthadd;
                //xrCellItem.Text= item.text;
                xrCellItem.TextFormatString = "{0:#,#.##}";
                xrCellItem.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", item.fieldName)});
                xrCellItem.Name = RandomString(9);
                xrCellItem.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
                xrCellItem.Font = new DevExpress.Drawing.DXFont("Times New Roman", 12F);
                xrCellItem.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));

                xrCellItem.CanGrow = false;
                xrCellItem.CanShrink = true;
                xrCellItem.TextFitMode = DevExpress.XtraReports.UI.TextFitMode.ShrinkOnly;

                xRTableRowHeader2.Cells.Add(xrHeaderItem);

                xrTabledetail.Cells.Add(xrCellItem);
                if (firstpage)//Tức là cột cuối cùng rồi, bắt đầu đầu trang thì phải thêm cột lặp lại
                {
                    totalWidth += columnrepeatWidth;
                    xrHeadermearge1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;//Chuyển chữ về đầu, không thôi dễ bị mất chữ ở 2 trang
                    InitCellMeargeSingleRepaet(xRTableRowHeader1, xRTableRowHeader2, xrTabledetail, "Tên hàng", "TenHang", columnrepeatWidth);

                }
                i++;

            }

            xrHeadermearge1.WidthF = totalWidth - totalwidthbegin;
            xRTableRowHeader1.Cells.Add(xrHeadermearge1);


        }

        public XRPanel InitXrpanel(System.ComponentModel.ComponentResourceManager resources, FiledItem filedItem)
        {
            DevExpress.XtraReports.UI.XRPanel xrPanel1 = new XRPanel();
            DevExpress.XtraReports.UI.XRPictureBox xrPictureBox = new XRPictureBox();
            DevExpress.XtraReports.UI.XRLabel xrlabel_description = new XRLabel();
            DevExpress.XtraReports.UI.XRLabel xrlabel_main = new XRLabel();


            xrlabel_main.CanGrow = false;
            xrlabel_main.CanShrink = true;
            xrlabel_main.Dpi = 254F;
            xrlabel_main.TextFormatString = "{0:#,#.##}";
            xrlabel_main.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", filedItem.fieldName)});
            xrlabel_main.Font = new DevExpress.Drawing.DXFont("Times New Roman", 12F, DevExpress.Drawing.DXFontStyle.Bold);
            xrlabel_main.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            xrlabel_main.Borders = DevExpress.XtraPrinting.BorderSide.None;
            xrlabel_main.Multiline = true;
          
            xrlabel_main.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            xrlabel_main.SizeF = new System.Drawing.SizeF(240F, 57F);
            xrlabel_main.StylePriority.UseFont = false;
            xrlabel_main.StylePriority.UseTextAlignment = false;
           
            xrlabel_main.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            xrlabel_main.TextFitMode = DevExpress.XtraReports.UI.TextFitMode.ShrinkOnly;

            xrPictureBox.Dpi = 254F;
            //Chỗ này phải tạo 1 image trong report, sau đó disable nó đi, để lấy image đó sử dụng cho cái này
            //xrPictureBox.ImageSource = new DevExpress.XtraPrinting.Drawing.ImageSource("img", resources.GetString("xrPictureBox1.ImageSource"));
            xrPictureBox.LocationFloat = new DevExpress.Utils.PointFloat(0, 40F);
            xrPictureBox.Name = "xrPictureBox";
            xrPictureBox.BeforePrint += xrPictureBox_BeforePrint;
            xrPictureBox.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageSource",filedItem.fieldimgduyet )});
            xrPictureBox.Borders = DevExpress.XtraPrinting.BorderSide.None;
            xrPictureBox.SizeF = new System.Drawing.SizeF(60F, 60F);
            xrPictureBox.Sizing = DevExpress.XtraPrinting.ImageSizeMode.ZoomImage;

            xrlabel_description.CanGrow = false;
            xrlabel_description.CanShrink = true;
            xrlabel_description.Dpi = 254F;
            xrlabel_description.Borders = DevExpress.XtraPrinting.BorderSide.None;
            xrlabel_description.TextFormatString = "{0:#,#.##}";
            xrlabel_description.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", filedItem.fieldNamedecription)});
            xrlabel_description.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Italic);
            xrlabel_description.LocationFloat = new DevExpress.Utils.PointFloat(40F, 60F);
            xrlabel_description.Multiline = true;
          
            xrlabel_description.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            xrlabel_description.SizeF = new System.Drawing.SizeF(200F, 40F);
            xrlabel_description.StylePriority.UseFont = false;
            xrlabel_description.StylePriority.UseTextAlignment = false;
         
            xrlabel_description.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            xrlabel_description.TextFitMode = DevExpress.XtraReports.UI.TextFitMode.ShrinkOnly;
          

            xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            xrPictureBox,
            xrlabel_description,
            xrlabel_main});
            xrPanel1.Dpi = 254F;
            xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
           
            xrPanel1.SizeF = new System.Drawing.SizeF(240F, 100F);
            return xrPanel1;

        }

        public XRPanel InitXrpanelVTKhac(System.ComponentModel.ComponentResourceManager resources, FiledItem filedItem)
        {
            DevExpress.XtraReports.UI.XRPanel xrPanel1 = new XRPanel();
            DevExpress.XtraReports.UI.XRPictureBox xrPictureBox = new XRPictureBox();
          
            DevExpress.XtraReports.UI.XRLabel xrlabel_main = new XRLabel();


            xrlabel_main.CanGrow = false;
            xrlabel_main.CanShrink = true;
            xrlabel_main.Dpi = 254F;
            xrlabel_main.TextFormatString = "{0:#,#.##}";
            xrlabel_main.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", filedItem.fieldName)});
            xrlabel_main.Font = new DevExpress.Drawing.DXFont("Times New Roman", 12F, DevExpress.Drawing.DXFontStyle.Bold);
            xrlabel_main.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            xrlabel_main.Borders = DevExpress.XtraPrinting.BorderSide.None;
            xrlabel_main.Multiline = true;
            xrlabel_main.LocationFloat = new DevExpress.Utils.PointFloat(60F, 0F);
            xrlabel_main.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 96F);
            xrlabel_main.SizeF = new System.Drawing.SizeF(180F, 60F);
            xrlabel_main.StylePriority.UseFont = false;
            xrlabel_main.StylePriority.UseTextAlignment = false;

            xrlabel_main.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            xrlabel_main.TextFitMode = DevExpress.XtraReports.UI.TextFitMode.ShrinkOnly;

            xrPictureBox.Dpi = 254F;
            //Chỗ này phải tạo 1 image trong report, sau đó disable nó đi, để lấy image đó sử dụng cho cái này
            //xrPictureBox.ImageSource = new DevExpress.XtraPrinting.Drawing.ImageSource("img", resources.GetString("xrPictureBox1.ImageSource"));
            xrPictureBox.LocationFloat = new DevExpress.Utils.PointFloat(0, 0F);
            xrPictureBox.Name = "xrPictureBox";
            xrPictureBox.BeforePrint += xrPictureBox_BeforePrint;
            xrPictureBox.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageSource",filedItem.fieldimgduyet )});
            xrPictureBox.Borders = DevExpress.XtraPrinting.BorderSide.None;
            xrPictureBox.SizeF = new System.Drawing.SizeF(60F, 60F);
            xrPictureBox.Sizing = DevExpress.XtraPrinting.ImageSizeMode.ZoomImage;
            xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            xrPictureBox,
          
            xrlabel_main});
            xrPanel1.Dpi = 254F;
            xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            xrPanel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 96F);
            xrPanel1.SizeF = new System.Drawing.SizeF(240F, 60F);
            return xrPanel1;

        }
    }
}
