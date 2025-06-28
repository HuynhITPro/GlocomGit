using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.Report;
using NFCWebBlazor.Model;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_NvlNhapXuatGhiNo
    {
        [Inject] ToastService toastService { get; set; }
        bool Ismobile { get; set; }
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        protected override async Task OnInitializedAsync()
        {
            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 90;
            heightgrid = string.Format("{0}px", height);

            int width = dimension.Width;
            if (width < 768)
            {
                Ismobile = true;

            }
            else
            {
                Ismobile = false;

            }
            //randomdivhide = prs.RandomString(10);
            await loadAsync();

            //return base.OnInitializedAsync();
        }
       public class NvlNhapXuatGhiNo
        {
           
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string DVT { get; set; }
            public string TenNhom { get; set; }
            public decimal? SLTonDau { get; set; }
            public decimal? SLGhiNo { get; set; }
            public decimal? SLTra { get; set; }
            public decimal? TonCuoi { get; set; }
            public NvlNhapXuatGhiNo CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlNhapXuatGhiNo>(json);
            }
            
            public List<NvlNhapXuatGhiNoItem> lstitem{ get; set; }
        }
       public class NvlNhapXuatGhiNoItem
        {
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string TenGN { get; set; }
            public string DVT { get; set; }
            public int Serial { get; set; }
            public int? SerialCT { get; set; }
            public decimal? SLGhiNo { get; set; }
            public decimal? SLTra { get; set; }
            public DateTime? Ngay { get; set; }
            public decimal? TonCuoi { get; set; }
           
            public int? SerialNXItem { get; set; }
            public int? SerialDN { get; set; }
            public int? SerialDNItem { get; set; }
            public int? SerialLink { get; set; }
            public string NguoiDeNghi { get; set; }
            public string UserInsert { get; set; }
            public string TenUserDuyet { get; set; }
            public string UserDuyet { get; set; }
            public DateTime? NgayInsert { get; set; }
        }
        class columname
        {
            public string Name { get; set; }
            public Type DataType { get; set; }
            //Khởi tạo class columname
            public columname(string name, Type type)
            {
                Name = name;
                DataType = type;
            }

        }
        List<columname> lstcolumn = new List<columname>();

        private async Task loadAsync()
        {
            lstkhonvl = await Model.ModelData.GetKhoNvl();
            lstmanhom = await Model.ModelData.GetlstNhomhang();
        }
        string typeReport = "";//Phân ra 2 biến đầu vào: Kho và Lý do
        class GroupSum
        {
            public string GroupName { get; set; }
            public double Sum { get; set; } = 0;
            public double TyLe { get; set; } = 0;
            public double ThanhTien { get; set; }
            public double TTTonDau { get; set; }
            public double TTNhap { get; set; }
            public double TTXuat { get; set; }
        }
        class ThanhTienNhapXuat
        {
            public decimal TTNhap { get; set; }
            public decimal TTXuat { get; set; }

        }
        public class LoadItemTheKho
        {
            public string MaHang { get; set; }
            public string MaGN { get; set; }
            public string TenHang { get; set; }
            public string DVT { get; set; }
            public double? TTTonKho { get; set; }
            public string TenGN { get; set; }
            public string SoPhieu { get; set; }
            public string SoLo { get; set; }
            public int? SerialCT { get; set; }
            public int? SerialLink { get; set; }
            public DateTime? Ngay { get; set; }
            public DateTime? NgaySanXuat { get; set; }
            public double SLNhap { get; set; }
            public double SLXuat { get; set; }
            public double SLTon { get; set; }
        }
        List<GroupSum> lstgroup = new List<GroupSum>();
        private void xulydieukien()
        {

           
          
            if (dtpbegin == null || dtpend == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn ngày"));
                return;
            }
            lstpara.Clear();

            ghichu = "";
            dieukien = "";

            dieukienmahang = "";
            string sql = "";

            ghichutemp = string.Format("Từ {0} - {1}", dtpbegin.Value.ToString("dd/MM/yy"), dtpend.Value.ToString("dd/MM/yy"));

            lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
            lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
            headertondauky = "Nợ trước " + dtpbegin.Value.ToString("dd/MM/yy");
            headerphatsinh = string.Format("PHÁT SINH" + Environment.NewLine + "{0} - {1}", dtpbegin.Value.ToString("dd/MM"), dtpend.Value.ToString("dd/MM"));
            
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
            {
                dieukienmahang = " where hh.MaHang=@MaHang";
                lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItemShowcrr.MaHang));
                ghichutemp += Environment.NewLine + "Hàng hóa: " + nvlNhapXuatItemShowcrr.TenHang;
            }

        }
        public async Task searchAsync()
        {
            lstdata.Clear();
            xulydieukien();
            string sql = "";

               
                sql = string.Format(@"use NVLDB  declare @tblghino as Table(MaHang nvarchar(100),SLTonDau decimal(18,6),SLGhiNo decimal(18,6),SLTra decimal(18,6),TonCuoi decimal(18,6))

                          insert into @tblghino(MaHang,SLTonDau,SLGhiNo,SLTra,TonCuoi)
                          select MaHang,sum(SLTonDau) as SLTonDau,sum(SLGhiNo) as SLGhiNo,sum(SLTra) as SLTra,sum(SLTonDau+SLGhiNo-SLTra) as TonCuoi
                          from
                          (
                          select MaHang,sum(SLGhiNo-SLTra) as SLTonDau,0 as SLGhiNo,0 as SLTra from NvlNhapXuatGhiNo where NgayInsert<@DateBegin group by MaHang
                          union all
                            select MaHang,0 as SLTonDau,sum(SLGhiNo) as SLGhiNo,sum(SLTra) as SLTra from NvlNhapXuatGhiNo where NgayInsert>=@DateBegin  and NgayInsert<=@DateEnd group by MaHang
                        ) as qry group by MaHang

                          SELECT  gn.[MaHang],hh.TenHang,nh.TenNhom,hh.DVT,SLTonDau,[SLGhiNo],[SLTra],TonCuoi
                          from @tblghino gn 
                          inner join NvlHangHoa hh on gn.MaHang=hh.MaHang
                          inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
                            {1}
                              order by gn.MaHang", dieukien, dieukienmahang);
            
            CallAPI callAPI = new CallAPI();
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NvlNhapXuatGhiNo>>(json);
                    lstdata.AddRange(query);
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;

                StateHasChanged();
            }

        }


        string dieukien = "", dieukienmahang = "";
        string ghichu = "";
        List<ParameterDefine> lstpara = new List<ParameterDefine>();
        DataTable dtitem = new DataTable();
        string ghichutemp = "";

    
     
     
    
        private async void printthekho(NvlNhapXuatGhiNo nvlNhapXuatGhiNo)
        {

            await printthekhotheomahangAsync(nvlNhapXuatGhiNo);

        }

        List<ParameterDefine> lstparaitem = new List<ParameterDefine>();
        private string setsqlItem(NvlNhapXuatGhiNo nvlNhapXuatGhiNo)
        {
            //xulydieukien();

            lstparaitem.Clear();
            foreach (ParameterDefine sqlParameter in lstpara)
            {
                ParameterDefine sqlParameter1 = new ParameterDefine();
                sqlParameter1.ParameterValue = sqlParameter.ParameterValue;
                sqlParameter1.ParameterName = sqlParameter.ParameterName;
                lstparaitem.Add(sqlParameter1);
            }
            lstparaitem.Add(new ParameterDefine("@MaHang", nvlNhapXuatGhiNo.MaHang));

            //string checkMaHang

            string sql = string.Format(@"use NVLDB   
                declare @tblnxghino table([Serial] [int],[MaHang] [nvarchar](100),[SerialNXItem] [int],[SerialDNItem] [int],[SLGhiNo] [decimal](18, 6),[SLTra] [decimal](18, 6),NgayInsert date)
                insert into @tblnxghino([Serial],[MaHang],[SerialNXItem],[SerialDNItem],[SLGhiNo],[SLTra],NgayInsert)
                select [Serial],[MaHang],[SerialNXItem],[SerialDNItem],[SLGhiNo],[SLTra],NgayInsert from
                (select [Serial],[MaHang],[SerialNXItem],[SerialDNItem],[SLGhiNo],[SLTra],NgayInsert
                from NvlNhapXuatGhiNo 
                where MaHang=@MaHang and NgayInsert>=@DateBegin and NgayInsert<=@DateEnd) as qry 

                --Xử lý ký duyệt
                declare @tbldnitem table(SerialDNItem int primary key)
                insert into @tbldnitem(SerialDNItem)
                select [SerialDNItem] from @tblnxghino group by SerialDNItem

                declare @tblkyduyet Table([Serial] int,[SerialLinkMaster] int,[SerialLinkItem] int,[UserDuyet] nvarchar(100),TenUserDuyet nvarchar(100))

                insert into @tblkyduyet([Serial] ,[SerialLinkMaster],[SerialLinkItem],[UserDuyet],[TenUserDuyet])
                select [Serial] ,[SerialLinkMaster],[SerialLinkItem],[UserDuyet],usr.TenUser
                from (SELECT  [Serial] ,[SerialLinkMaster],[SerialLinkItem],[UserDuyet]
                ,ROW_NUMBER() OVER (PARTITION BY [SerialLinkItem] ORDER BY [Serial] DESC) AS RowNum
                FROM [NvlKyDuyetItem] where TableName=N'NvlKehoachMuaHang' and LoaiDuyet=N'Duyệt'
                and SerialLinkItem in (select SerialDNItem from @tbldnitem)
                ) as qry  inner join  DBMaster.dbo.Users usr on qry.UserDuyet=usr.UsersName
                where RowNum=1


                select qry.MaHang,TenUserDuyet,qry.SLTonDau,qry.Ngay,qry.SLTonDau,qry.SLGhiNo,qry.SLTra
                        ,sum(qry.SLTonDau+qry.SLGhiNo-qry.SLTra) OVER (
                                    PARTITION BY qry.MaHang 
                                    ORDER BY qry.Ngay, qry.SLTra ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                                ) AS TonCuoi
                        
                        ,qry.MaGN,case when qry.MaGN='TonDau' then N'Nợ đầu kỳ' else ngn.TenGN  end as TenGN,hh.TenHang,hh.DVT
                        from
                        (select MaHang,DateAdd(dd,-1,@DateBegin) as Ngay,sum(SLGhiNo-SLTra) as SLTonDau,0 as SLGhiNo,0 as SLTra,'TonDau' as MaGN,'' as TenUserDuyet
                         from (select * from NvlNhapXuatGhiNo where MaHang=@MaHang and NgayInsert<@DateBegin) nxitem 
                         group by MaHang
                         union all
                        select gn.MaHang,gn.NgayInsert as Ngay,0 as SLTonDau,sum(SLGhiNo) as SLNhap,sum(SLTra) as SLXuat,nx.MaGN,tblduyet.TenUserDuyet
                        from (select * from @tblnxghino)  gn 
						left join dbo.NvlNhapXuatItem nxitem on gn.SerialNXItem=nxitem.Serial
						left join dbo.NvlNhapXuat nx on nxitem.SerialCT=nx.Serial
						left join @tblkyduyet tblduyet on gn.SerialDNItem=tblduyet.SerialLinkItem
						group by gn.MaHang,nx.MaGN,gn.NgayInsert,tblduyet.TenUserDuyet
						)
                        as qry 
                        left join dbo.View_NoiGN ngn on qry.MaGN=ngn.MaGN
                        inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                        order by qry.MaHang,qry.Ngay,SLTra
                            
                            
                            ");
            return sql;
        }
        private async Task printthekhotheomahangAsync(NvlNhapXuatGhiNo nvlNhapXuatGhiNo)
        {
            if (nvlNhapXuatGhiNo.lstitem == null)
            {


                xulydieukien();

                lstparaitem.Clear();
                foreach (ParameterDefine sqlParameter in lstpara)
                {
                    ParameterDefine sqlParameter1 = new ParameterDefine();
                    sqlParameter1.ParameterValue = sqlParameter.ParameterValue;
                    sqlParameter1.ParameterName = sqlParameter.ParameterName;
                    lstparaitem.Add(sqlParameter1);
                }


                //string checkMaHang

                string sql = string.Format(@"  select qry.MaHang,qry.SLTonDau,qry.Ngay,qry.SLTonDau,qry.SLGhiNo,qry.SLTra
                        ,sum(qry.SLTonDau+qry.SLGhiNo-qry.SLTra) OVER (
                                    PARTITION BY qry.MaHang 
                                    ORDER BY qry.Ngay, qry.SLTra ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                                ) AS TonCuoi
                        
                        ,qry.MaGN,case when qry.MaGN='TonDau' then N'Nợ đầu kỳ' else ngn.TenGN  end as TenGN,hh.TenHang,hh.DVT
                        from
                        (select MaHang,DateAdd(dd,-1,@DateBegin) as Ngay,sum(SLGhiNo-SLTra) as SLTonDau,0 as SLGhiNo,0 as SLTra,'TonDau' as MaGN 
                         from (select * from NvlNhapXuatGhiNo where MaHang=@MaHang and NgayInsert<@DateBegin) nxitem 
                         group by MaHang
                         union all
                        select gn.MaHang,cast(nxitem.NgayInsert as Date) as Ngay,0 as SLTonDau,sum(SLGhiNo) as SLNhap,sum(SLTra) as SLXuat,nx.MaGN
                        from (select * from NvlNhapXuatGhiNo where MaHang=@MaHang' and NgayInsert>=@DateBegin and NgayInsert<=@DateEnd)  gn 
						left join dbo.NvlNhapXuatItem nxitem on gn.SerialNXItem=nxitem.Serial
						left join dbo.NvlNhapXuat nx on nxitem.SerialCT=nx.Serial
						group by gn.MaHang,nx.MaGN,cast(nxitem.NgayInsert as Date)
						)
                        as qry 
                        left join dbo.View_NoiGN ngn on qry.MaGN=ngn.MaGN
                        inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                        order by qry.MaHang,qry.Ngay,SLTra
                            ");
                CallAPI callAPI = new CallAPI();
                try
                {
                   
                    nvlNhapXuatGhiNo.lstitem=new List<NvlNhapXuatGhiNoItem>();
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparaitem);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<NvlNhapXuatGhiNoItem>>(json);
                        nvlNhapXuatGhiNo.lstitem.AddRange(query);
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi:" + ex.Message));
                }
                finally
                {
                   
                    StateHasChanged();
                }
                //if (lstitem.Count > 0)
                //{
                //    //Xtra_TheKhoTheoMaHang xtra_BaoCaoTonKhoTheoNhaMay = new Xtra_TheKhoTheoMaHang();
                //    //xtra_BaoCaoTonKhoTheoNhaMay.setGhiChu(ghichutemp);
                //    //xtra_BaoCaoTonKhoTheoNhaMay.setTonKho(lstitem[lstitem.Count - 1].SLTon, lstitem[lstitem.Count - 1].TTTonKho);
                //    //xtra_BaoCaoTonKhoTheoNhaMay.DataSource = lstitem; //(DataTable)grvSP.VisibleItems;
                //    //ModelAdmin.mainLayout.showreportAsync(xtra_BaoCaoTonKhoTheoNhaMay);
                //}
            }

        }





    }

}
