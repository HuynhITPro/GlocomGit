using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.Model;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;

namespace NFCWebBlazor.App_DonHang
{
    public class XuLyDonHang
    {
        string string_temp = "";
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        string serverobject = "[SP].[DataBase_ScansiaPacific2014].";
        public async Task<DataTable> DonHangMua(CallAPI callAPI, List<string> lstdonhang)
        {
            string json = "";
            DataTable dt_resultDH = new DataTable();
            DataTable dtdonhangmaster = new DataTable();
            string DieuKienDonHang = "";
            if (lstdonhang.Count > 0)
            {
                foreach (string it in lstdonhang)
                {
                    if (DieuKienDonHang == "")
                        DieuKienDonHang += string.Format("N'{0}'", it);
                    else
                        DieuKienDonHang += string.Format(",N'{0}'", it);
                }
                DieuKienDonHang = string.Format(" where MaHD in ({0})", DieuKienDonHang);
            }
            else
                DieuKienDonHang += " where Mua=1";

           
            string_temp = string.Format(@"
                        select sp.KhachHang,qry.MaSP,sp.TenSP,qry.MaHD,qry.PhanLoai,qry.SoLuong,qry.BufferDonHang,sp.He from

                        (SELECT
                                                      [MaSP]
                                                      ,[PhanLoai]
                                                      ,sum([SoLuong]) as SoLuong,sum(SoLuong*(1+isnull(Buffer,0))) as BufferDonHang,MaHD
                                                      
                                                      FROM {1}dbo.[DonHangMua] {0}
            group by [MaSP],[PhanLoai],MaHD) as qry inner join {1}dbo.Load_cbSP sp on sp.MaSP=qry.MaSP order by MaHD,sp.He", DieuKienDonHang,serverobject);
            json = await callAPI.ExcuteQueryEncryptAsync(string_temp, new List<ParameterDefine>());
            if (json != "")
            {

                var query = JsonConvert.DeserializeObject<DataTable>(json);
                dtdonhangmaster = query;
                //await InvokeAsync(StateHasChanged);
            }
           // DataTable dtdonhangmaster = prs.dt_Connect(string_temp, Conn);
            if (dtdonhangmaster.Rows.Count > 0)
            {
                string_temp = string.Format(@"select MaSP,SLDonHang,SLDaXuat,SLPhaiXuat, 0  as DD,0 as DC,SLPhaiXuat as Other,SoHD,round(isnull(Gia,0),4) as Gia,cast(0 as float) as TonKho from 
                                (SELECT art.MaSP,avg(case when art.Cost >0 then art.Cost else NULL end) as Gia,SoHD,sum(SLDonHang) as SLDonHang,sum(SLDaXuat) as SLDaXuat,sum([SLDonHang]-[SLDaXuat]) as SLPhaiXuat
                                FROM {1}dbo.[KeHoachXuatHang] khxh
                                inner join {1}dbo.ArticleNumberProduct art on art.ArticleNumber=khxh.ArticleNumber
                                where SoHD in (SELECT [MaHD] FROM {1}dbo.[DonHangMua]  {0} group by MaHD)    
								group by art.MaSP,SoHD) 
                                as qry_KeHoach order by MaSP,SoHD", DieuKienDonHang,serverobject);
                DataTable dtdonhangthucnhan = new DataTable();
                json = await callAPI.ExcuteQueryEncryptAsync(string_temp, new List<ParameterDefine>());
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<DataTable>(json);
                    dtdonhangthucnhan = query;
                    //await InvokeAsync(StateHasChanged);
                }

                //var query_MaHD = dt_.AsEnumerable().GroupBy(p => new{MaHD=p.Field<string>("MaHD").ToString()}).Select(p => new { MaHD=p.Key.MaHD}).ToList();
                //var query_PhanLoai = dt_.AsEnumerable().GroupBy(p => new { PhanLoai = p.Field<string>("PhanLoai") }).Select(p => new { PhanLoai = p.Key.PhanLoai }).OrderByDescending(p => p.PhanLoai).ToList();
                dt_resultDH.Columns.Add("MaHDSort", typeof(string));
                dt_resultDH.Columns.Add("MaHDGroup", typeof(string));
                dt_resultDH.Columns.Add("MaHD", typeof(string));
                dt_resultDH.Columns.Add("KhachHang", typeof(string));
                dt_resultDH.Columns.Add("MaSP", typeof(string));
                dt_resultDH.Columns.Add("TenSP", typeof(string));

                dt_resultDH.Columns.Add("LowSeason", typeof(double));
                dt_resultDH.Columns["LowSeason"].DefaultValue = 0;
                dt_resultDH.Columns.Add("HighSeason", typeof(double));
                dt_resultDH.Columns["HighSeason"].DefaultValue = 0;

                dt_resultDH.Columns.Add("TongDH", typeof(double));
                dt_resultDH.Columns["TongDH"].DefaultValue = 0;

                dt_resultDH.Columns.Add("SLDonHangNhan", typeof(double));
                dt_resultDH.Columns["SLDonHangNhan"].DefaultValue = 0;

                dt_resultDH.Columns.Add("SLDonHangNo", typeof(double));
                dt_resultDH.Columns["SLDonHangNo"].DefaultValue = 0;

                dt_resultDH.Columns.Add("SLDaXuat", typeof(double));
                dt_resultDH.Columns["SLDaXuat"].DefaultValue = 0;


                dt_resultDH.Columns.Add("SLPhaiXuat", typeof(double));
                dt_resultDH.Columns["SLPhaiXuat"].DefaultValue = 0;

                dt_resultDH.Columns.Add("DD", typeof(double));
                dt_resultDH.Columns["DD"].DefaultValue = 0;
                dt_resultDH.Columns.Add("DC", typeof(double));
                dt_resultDH.Columns["DC"].DefaultValue = 0;
                dt_resultDH.Columns.Add("Other", typeof(double));
                dt_resultDH.Columns["Other"].DefaultValue = 0;


                dt_resultDH.Columns.Add("SLTonKho", typeof(double));
                dt_resultDH.Columns["SLTonKho"].DefaultValue = 0;
                dt_resultDH.Columns.Add("SLPhaiNhap", typeof(double));
                dt_resultDH.Columns["SLPhaiNhap"].DefaultValue = 0;


                dt_resultDH.Columns.Add("SLPhaiNhapTongDH", typeof(double));
                dt_resultDH.Columns["SLPhaiNhapTongDH"].DefaultValue = 0;
                dt_resultDH.Columns.Add("BufferDonHang", typeof(double));
                dt_resultDH.Columns.Add("BufferNhapKho", typeof(double));
                dt_resultDH.Columns["BufferDonHang"].DefaultValue = 0;
                dt_resultDH.Columns["BufferNhapKho"].DefaultValue = 0;
                dt_resultDH.Columns.Add("Gia", typeof(double));
                dt_resultDH.Columns["Gia"].DefaultValue = 0;
                int i = 0;


                var query_DonHangSP = dtdonhangmaster.AsEnumerable().GroupBy(p => new { KhachHang = p.Field<string>("KhachHang"), MaDH = p.Field<string>("MaHD"), MaSP = p.Field<string>("MaSP"), TenSP = p.Field<string>("TenSP"), He = p.Field<string>("He") }).Select(p => new { MaHD = p.Key.MaDH.ToString(), KhachHang = p.Key.KhachHang, MaSP = p.Key.MaSP, TongSP = p.Sum(n => n.Field<double>("SoLuong")), BufferDonHang = p.Sum(n => n.Field<double>("BufferDonHang")), TenSP = p.Key.TenSP, He = p.Key.He }).OrderBy(p => p.He.ToString()).OrderBy(p => p.MaHD.ToString()).ToList();

                foreach (var it in query_DonHangSP)
                {

                    DataRow row = dt_resultDH.NewRow();
                    row["KhachHang"] = it.KhachHang;
                    row["TenSP"] = it.TenSP;
                    row["MaHDGroup"] = it.MaHD;
                    row["MaHDSort"] = it.MaHD;
                    row["MaHD"] = it.MaHD;
                    row["MaSP"] = it.MaSP;
                    row["TongDH"] = it.TongSP;
                    row["BufferDonHang"] = it.BufferDonHang;
                    dt_resultDH.Rows.Add(row);

                }

                string MaSPDH = "";
                foreach (DataRow row in dtdonhangmaster.Rows)
                {
                    string_temp = row["MaHD"].ToString();
                    MaSPDH = row["MaSP"].ToString();
                    foreach (DataRow rowresult in dt_resultDH.Rows)
                    {
                        if (string_temp == rowresult.Field<string>("MaHD") && MaSPDH == rowresult.Field<string>("MaSP"))
                        {
                            if (row.Field<string>("PhanLoai") == "LowSeason")
                                rowresult["LowSeason"] = row["SoLuong"];
                            else
                                rowresult["HighSeason"] = row["SoLuong"];
                            break;
                        }
                    }
                }

                string_temp = string.Format(@"SELECT  MaSP, SUM(SLNhap) AS TonKho
                                                        FROM    {0}dbo.KhoTP_SPChuaXuat
                                                        GROUP BY MaSP", serverobject);
                //}
                DataTable dt_TonKhoTP = new DataTable();
                json = await callAPI.ExcuteQueryEncryptAsync(string_temp, new List<ParameterDefine>());
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<DataTable>(json);
                    dt_TonKhoTP = query;
                    //await InvokeAsync(StateHasChanged);
                }
                double dtk = 0;
                //Xử lý thực nhận với donhangtong
                foreach (DataRow rowresult in dt_resultDH.Rows)
                {
                    string_temp = rowresult["MaHD"].ToString();
                    MaSPDH = rowresult["MaSP"].ToString();
                    foreach (DataRow rowtn in dtdonhangthucnhan.Rows)
                    {
                        if (MaSPDH == rowtn.Field<string>("MaSP"))
                        {
                            rowresult["Gia"] = rowtn["Gia"];
                            if (string_temp == rowtn.Field<string>("SoHD"))
                            {
                                rowresult["SLDonHangNhan"] = rowtn["SLDonHang"];
                                rowresult["SLDaXuat"] = rowtn["SLDaXuat"];
                                rowresult["SLPhaiXuat"] = (rowtn.Field<double>("SLPhaiXuat") < 0) ? 0 : rowtn["SLPhaiXuat"];
                                rowresult["SLDonHangNo"] = rowresult.Field<double>("TongDH") - rowresult.Field<double>("SLDonHangNhan");//.ToString());
                                rowresult["Other"] = rowresult["SLPhaiXuat"];
                                if (rowresult.Field<double>("TongDH") <= 0)
                                {
                                    rowresult["TongDH"] = rowresult["SLDonHangNhan"];
                                }
                                break;
                            }
                        }
                    }


                }
                foreach (DataRow rowresult in dt_resultDH.Rows)
                {
                    foreach (DataRow rowtk in dt_TonKhoTP.Rows)
                    {

                        if (rowresult.Field<string>("MaSP") == rowtk.Field<string>("MaSP"))
                        {
                            dtk = rowtk.Field<double>("TonKho");

                            if (dtk >= rowresult.Field<double>("SLPhaiXuat"))
                            {
                                rowresult["SLTonKho"] = rowresult.Field<double>("SLPhaiXuat");
                                rowtk["TonKho"] = dtk - rowresult.Field<double>("SLPhaiXuat");

                            }
                            else
                            {
                                rowresult["SLTonKho"] = dtk;
                                rowtk["TonKho"] = 0;

                            }

                            break;
                        }
                    }
                    double d = rowresult.Field<double>("SLPhaiXuat") - rowresult.Field<double>("SLTonKho");

                    rowresult["SLPhaiNhap"] = (d < 0) ? 0 : d;

                    rowresult["SLPhaiNhapTongDH"] = rowresult.Field<double>("TongDH") - rowresult.Field<double>("SLDaXuat") - rowresult.Field<double>("SLTonKho");

                }
                //Xử lý nếu phần tồn vẫn còn
                var querytk = dt_TonKhoTP.Select("TonKho>0");
                if (querytk.Any())
                {
                    DataTable dttkcl = querytk.CopyToDataTable();
                    foreach (DataRow rowtk in dttkcl.Rows)
                    {
                        for (int k = dt_resultDH.Rows.Count - 1; k >= 0; k--)
                        {
                            if (rowtk.Field<string>("MaSP") == dt_resultDH.Rows[k].Field<string>("MaSP"))
                            {
                                DataRow rowresult = dt_resultDH.Rows[k];
                                rowresult["SLTonKho"] = rowresult.Field<double>("SLTonKho") + rowtk.Field<double>("TonKho");
                                rowtk["TonKho"] = 0;
                                double d = rowresult.Field<double>("SLPhaiXuat") - rowresult.Field<double>("SLTonKho");

                                rowresult["SLPhaiNhap"] = (d < 0) ? 0 : d;

                                //rowresult["SLPhaiNhap"] = rowresult.Field<double>("SLPhaiXuat") - rowresult.Field<double>("SLTonKho");
                                rowresult["SLPhaiNhapTongDH"] = rowresult.Field<double>("TongDH") - rowresult.Field<double>("SLDaXuat") - rowresult.Field<double>("SLTonKho");
                                break;
                            }
                        }
                    }

                }

                //Duyệt cho đơn loại kho đến
                //Xử lý dòng tổng //Đảm bảo mảng 
                var querygroupDH = dt_resultDH.AsEnumerable().GroupBy(p => new { MaDH = p.Field<string>("MaHD") })

                       .Select(p => new {
                           MaDH = p.Key.MaDH,
                           LowSeason = p.Sum(n => n.Field<double>("LowSeason") * n.Field<double>("Gia")),
                           HighSeason = p.Sum(n => n.Field<double>("HighSeason") * n.Field<double>("Gia")),
                           TongDH = p.Sum(n => n.Field<double>("TongDH") * n.Field<double>("Gia")),
                           SLDonHangNhan = p.Sum(n => n.Field<double>("SLDonHangNhan") * n.Field<double>("Gia")),
                           SLDonHangNo = p.Sum(n => n.Field<double>("SLDonHangNo") * n.Field<double>("Gia")),
                           SLDaXuat = p.Sum(n => n.Field<double>("SLDaXuat") * n.Field<double>("Gia")),
                           SLPhaiXuat = p.Sum(n => n.Field<double>("SLPhaiXuat") * n.Field<double>("Gia")),
                           DD = p.Sum(n => n.Field<double>("DD") * n.Field<double>("Gia")),
                           DC = p.Sum(n => n.Field<double>("DC") * n.Field<double>("Gia")),
                           Other = p.Sum(n => n.Field<double>("Other") * n.Field<double>("Gia")),
                           SLTonKho = p.Sum(n => n.Field<double>("SLTonKho") * n.Field<double>("Gia")),
                           SLPhaiNhap = p.Sum(n => n.Field<double>("SLPhaiNhap") * n.Field<double>("Gia")),
                           SLPhaiNhapTongDH = p.Sum(n => n.Field<double>("SLPhaiNhapTongDH") * n.Field<double>("Gia"))
                       }).ToList();
                foreach (var it in querygroupDH)
                {
                    //DataRow row = dt_resultDH.NewRow();
                    DataRow rowheader = dt_resultDH.NewRow();
                    rowheader["KhachHang"] = "";
                    rowheader["TenSP"] = it.MaDH;
                    rowheader["MaHDSort"] = it.MaDH + "-Total";
                    rowheader["MaHD"] = "Break";
                    rowheader["MaHDGroup"] = it.MaDH;
                    rowheader["MaSP"] = "";
                    rowheader["LowSeason"] = it.LowSeason;
                    rowheader["HighSeason"] = it.HighSeason;
                    rowheader["TongDH"] = it.TongDH;
                    rowheader["SLDonHangNhan"] = it.SLDonHangNhan;
                    rowheader["SLDonHangNo"] = it.SLDonHangNo;
                    rowheader["SLDaXuat"] = it.SLDaXuat;
                    rowheader["SLPhaiXuat"] = it.SLPhaiXuat;
                    rowheader["DD"] = it.DD;
                    rowheader["DC"] = it.DC;
                    rowheader["Other"] = it.Other;
                    rowheader["SLTonKho"] = it.SLTonKho;
                    rowheader["SLPhaiNhap"] = it.SLPhaiNhap;
                    rowheader["SLPhaiNhapTongDH"] = it.SLPhaiNhapTongDH;

                    dt_resultDH.Rows.Add(rowheader);
                    //check_ = it.MaHD;
                }
                var querytotal = querygroupDH.GroupBy(p => 1)
                    .Select(p => new
                    {
                        LowSeason = p.Sum(n => n.LowSeason),
                        HighSeason = p.Sum(n => n.HighSeason),
                        TongDH = p.Sum(n => n.TongDH),
                        SLDonHangNhan = p.Sum(n => n.SLDonHangNhan),
                        SLDonHangNo = p.Sum(n => n.SLDonHangNo),
                        SLDaXuat = p.Sum(n => n.SLDaXuat),
                        SLPhaiXuat = p.Sum(n => n.SLPhaiXuat),
                        DD = p.Sum(n => n.DD),
                        DC = p.Sum(n => n.DC),
                        Other = p.Sum(n => n.Other),
                        SLTonKho = p.Sum(n => n.SLTonKho),
                        SLPhaiNhap = p.Sum(n => n.SLPhaiNhap),
                        SLPhaiNhapTongDH = p.Sum(n => n.SLPhaiNhapTongDH)

                    }).ToList();
                foreach (var it in querytotal)
                {
                    //DataRow row = dt_resultDH.NewRow();
                    DataRow rowheader = dt_resultDH.NewRow();
                    rowheader["MaHDSort"] = "zzzzzzz";
                    rowheader["KhachHang"] = "";
                    rowheader["MaHDGroup"] = "z. TỔNG ĐƠN HÀNG";
                    rowheader["TenSP"] = "TỔNG ĐƠN HÀNG";
                    rowheader["MaHD"] = "Break";
                    rowheader["MaSP"] = "";
                    rowheader["LowSeason"] = it.LowSeason;
                    rowheader["HighSeason"] = it.HighSeason;
                    rowheader["TongDH"] = it.TongDH;
                    rowheader["SLDonHangNhan"] = it.SLDonHangNhan;
                    rowheader["SLDonHangNo"] = it.SLDonHangNo;
                    rowheader["SLDaXuat"] = it.SLDaXuat;
                    rowheader["SLPhaiXuat"] = it.SLPhaiXuat;
                    rowheader["DD"] = it.DD;
                    rowheader["DC"] = it.DC;
                    rowheader["Other"] = it.Other;
                    rowheader["SLTonKho"] = it.SLTonKho;
                    rowheader["SLPhaiNhap"] = it.SLPhaiNhap;
                    rowheader["SLPhaiNhapTongDH"] = it.SLPhaiNhapTongDH;

                    dt_resultDH.Rows.Add(rowheader);
                    //check_ = it.MaHD;
                }
                dt_resultDH.DefaultView.Sort = $"MaHDSort asc";
                dt_resultDH = dt_resultDH.DefaultView.ToTable();
                DataColumn cl = dt_resultDH.Columns["MaHDSort"];
                dt_resultDH.Columns.Remove(cl);


                querygroupDH.Clear();
                querytotal.Clear();

                dt_TonKhoTP.Clear();
                dtdonhangthucnhan.Clear();
                dt_TonKhoTP.Dispose();
                query_DonHangSP.Clear();
                //query_PhanLoai.Clear();
                //var query_TotalHD = dt_resultDH.AsEnumerable().GroupBy(p => new { MaHD = p.Field<string>("MaHD") }).Select(p => new {MaHD=p.Key.MaHD, });
                //Code file excel
            }

            dtdonhangmaster.Clear();
            return dt_resultDH;
        }

        public async Task<DataTable> DonHangMuaArt(CallAPI callAPI, List<string> lstdonhang)
        {
            DataTable dt_resultDH = new DataTable();
            string DieuKienDonHang = "";
            if (lstdonhang.Count > 0)
            {
                foreach (string it in lstdonhang)
                {
                    if (DieuKienDonHang == "")
                        DieuKienDonHang += string.Format("N'{0}'", it);
                    else
                        DieuKienDonHang += string.Format(",N'{0}'", it);
                }
                DieuKienDonHang = string.Format(" where MaHD in ({0})", DieuKienDonHang);
            }
            else
                DieuKienDonHang += " where Mua=1";

            string_temp =string.Format(@"
                            declare @tblart as Table(MaSP nvarchar(100),ArticleNumber nvarchar(500),[Gia] float,MaMau nvarchar(100))
                            declare @tblArticle as Table(MaSP nvarchar(100),ArticleNumber nvarchar(100) primary key,[Gia] float,MaMau nvarchar(100))
                            insert  into @tblArticle(MaSP,ArticleNumber,Gia,MaMau)
                            select MaSP,ArticleNumber,Cost,MaMau
                            from {0}dbo.[ArticleNumberProduct]

                        insert into @tblart(MaSP,ArticleNumber,Gia,MaMau)
                        SELECT [MaSP],';'+STUFF((SELECT DISTINCT ';' + [ArticleNumber]
                         FROM @tblArticle item
                         WHERE item.MaMau = art.MaMau AND item.MaSP = art.MaSP
                         FOR XML PATH (''))
                         , 1, 1, '')+';'  AS ArticleNumber
                        ,avg(case when Gia>0 then Gia else NULL end) as Gia,[MaMau] 
                        FROM @tblArticle as art
                        group by MaSP,MaMau
                        select tbl.*,sp.TenSP,sp.KhachHang as TenKH,mm.TenMau,mm.Color,sp.He from @tblart tbl 
						inner join {0}dbo.SanPham sp on tbl.MaSP=sp.MaSP
                        inner join {0}dbo.MaMau mm on tbl.MaMau=mm.MaMau
                        ", serverobject);
            DataTable dtart = new DataTable();
           string json = await callAPI.ExcuteQueryEncryptAsync(string_temp, new List<ParameterDefine>());
            if (json != "")
            {

                var query = JsonConvert.DeserializeObject<DataTable>(json);
                dtart = query;
                //await InvokeAsync(StateHasChanged);
            }
            string_temp = string.Format(@"
                        
                     select art.MaSP,MaHD,art.MaMau,qrydh.PhanLoai,sum(qrydh.SoLuong) as SoLuong,sum(qrydh.[Buffer]) as  [Buffer] from
                    (SELECT [MaHD]
                          ,[ArticleNumber]
                          ,[PhanLoai]
                          ,sum([SoLuong]) as SoLuong
                          ,sum([Buffer]) as [Buffer]
                      FROM {1}dbo.[DonHangMua]
                       {0}
                      group by MaHD,ArticleNumber,PhanLoai) as qrydh 
                      inner join {1}dbo.ArticleNumberProduct art on qrydh.ArticleNumber=art.ArticleNumber
                      group by art.MaSP,MaHD,art.MaMau,qrydh.PhanLoai", DieuKienDonHang,serverobject);

            DataTable dtdonhangmaster = new DataTable();
            json = await callAPI.ExcuteQueryEncryptAsync(string_temp, new List<ParameterDefine>());
            if (json != "")
            {

                var query = JsonConvert.DeserializeObject<DataTable>(json);
                dtdonhangmaster = query;
                //await InvokeAsync(StateHasChanged);
            }
            if (dtdonhangmaster.Rows.Count > 0)
            {
                string_temp = string.Format(@"select MaSP,MaMau,SLDonHang,SLDaXuat,SLPhaiXuat, 0  as DD,0 as DC,SLPhaiXuat as Other,SoHD,round(Gia,3) as Gia,cast(0 as float) as TonKho from 
                                (SELECT art.MaSP,art.MaMau,avg(case when art.Cost >0 then art.Cost else NULL end) as Gia,SoHD,sum(SLDonHang) as SLDonHang,sum(SLDaXuat) as SLDaXuat,sum([SLDonHang]-[SLDaXuat]) as SLPhaiXuat
                                FROM {1}dbo.[KeHoachXuatHang] khxh
                                inner join {1}dbo.ArticleNumberProduct art on art.ArticleNumber=khxh.ArticleNumber
                                where khxh.SoHD in (SELECT [MaHD] FROM {1}dbo.[DonHangMua] {0} group by MaHD)    
								group by art.MaSP,art.MaMau,SoHD) 
                                as qry_KeHoach order by MaSP,SoHD", DieuKienDonHang,serverobject);
               // DataTable dtdonhangthucnhan = prs.dt_Connect(string_temp, Conn);
                DataTable dtdonhangthucnhan = new DataTable();
                json = await callAPI.ExcuteQueryEncryptAsync(string_temp, new List<ParameterDefine>());
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<DataTable>(json);
                    dtdonhangthucnhan = query;
                    //await InvokeAsync(StateHasChanged);
                }

                //var query_MaHD = dt_.AsEnumerable().GroupBy(p => new{MaHD=p.Field<string>("MaHD").ToString()}).Select(p => new { MaHD=p.Key.MaHD}).ToList();
                //var query_PhanLoai = dt_.AsEnumerable().GroupBy(p => new { PhanLoai = p.Field<string>("PhanLoai") }).Select(p => new { PhanLoai = p.Key.PhanLoai }).OrderByDescending(p => p.PhanLoai).ToList();


                dt_resultDH.Columns.Add("MaMau", typeof(string));
                dt_resultDH.Columns.Add("MaHDSort", typeof(string));
                dt_resultDH.Columns.Add("MaHDGroup", typeof(string));
                dt_resultDH.Columns.Add("MaHD", typeof(string));
                dt_resultDH.Columns.Add("KhachHang", typeof(string));
                dt_resultDH.Columns.Add("MaSP", typeof(string));
                dt_resultDH.Columns.Add("TenSP", typeof(string));
                dt_resultDH.Columns.Add("ArticleNumber", typeof(string));

                dt_resultDH.Columns.Add("TenMau", typeof(string));
                dt_resultDH.Columns.Add("ColorHex", typeof(string));
                //Bỏ luôn phân, lấy phân loại mặc định
                //foreach (var it in query_PhanLoai)
                //{
                //    dt_resultDH.Columns.Add(it.PhanLoai.ToString(), typeof(double));
                //    dt_resultDH.Columns[it.PhanLoai.ToString()].DefaultValue = 0;

                //}
                dt_resultDH.Columns.Add("LowSeason", typeof(double));
                dt_resultDH.Columns["LowSeason"].DefaultValue = 0;
                dt_resultDH.Columns.Add("HighSeason", typeof(double));
                dt_resultDH.Columns["HighSeason"].DefaultValue = 0;

                dt_resultDH.Columns.Add("TongDH", typeof(double));
                dt_resultDH.Columns["TongDH"].DefaultValue = 0;

                dt_resultDH.Columns.Add("SLDonHangNhan", typeof(double));
                dt_resultDH.Columns["SLDonHangNhan"].DefaultValue = 0;

                dt_resultDH.Columns.Add("SLDonHangNo", typeof(double));
                dt_resultDH.Columns["SLDonHangNo"].DefaultValue = 0;

                dt_resultDH.Columns.Add("SLDaXuat", typeof(double));
                dt_resultDH.Columns["SLDaXuat"].DefaultValue = 0;


                dt_resultDH.Columns.Add("SLPhaiXuat", typeof(double));
                dt_resultDH.Columns["SLPhaiXuat"].DefaultValue = 0;

                dt_resultDH.Columns.Add("DD", typeof(double));
                dt_resultDH.Columns["DD"].DefaultValue = 0;
                dt_resultDH.Columns.Add("DC", typeof(double));
                dt_resultDH.Columns["DC"].DefaultValue = 0;
                dt_resultDH.Columns.Add("Other", typeof(double));
                dt_resultDH.Columns["Other"].DefaultValue = 0;

                //for(int k=1;k<dt_LoaiKho.Columns.Count;k++)
                //{
                //    dt_resultDH.Columns.Add(dt_LoaiKho.Columns[k].ColumnName, typeof(double));
                //    dt_resultDH.Columns[dt_LoaiKho.Columns[k].ColumnName].DefaultValue = 0;
                //}
                dt_resultDH.Columns.Add("SLTonKho", typeof(double));
                dt_resultDH.Columns["SLTonKho"].DefaultValue = 0;
                dt_resultDH.Columns.Add("SLPhaiNhap", typeof(double));
                dt_resultDH.Columns["SLPhaiNhap"].DefaultValue = 0;


                dt_resultDH.Columns.Add("SLPhaiNhapTongDH", typeof(double));
                dt_resultDH.Columns["SLPhaiNhapTongDH"].DefaultValue = 0;
                dt_resultDH.Columns.Add("BufferDonHang", typeof(double));
                dt_resultDH.Columns.Add("BufferNhapKho", typeof(double));
                dt_resultDH.Columns["BufferDonHang"].DefaultValue = 0;
                dt_resultDH.Columns["BufferNhapKho"].DefaultValue = 0;
                dt_resultDH.Columns.Add("Gia", typeof(double));
                dt_resultDH.Columns["Gia"].DefaultValue = 0;


                //Tạm thời ko tính theo Art, tính theo tên màu và mã sp

                //var query_DonHangSP = dt_.AsEnumerable().GroupBy(p => new { KhachHang = p.Field<string>("KhachHang"), MaDH = p.Field<string>("MaHD"), MaSP = p.Field<string>("MaSP"), TenSP = p.Field<string>("TenSP"), He = p.Field<string>("He"),ArticleNumber=p.Field<string>("ArticleNumber") }).Select(p => new { MaHD = p.Key.MaDH.ToString(), KhachHang = p.Key.KhachHang, MaSP = p.Key.MaSP, TongSP = p.Sum(n => n.Field<double>("SoLuong")), BufferDonHang = p.Sum(n => n.Field<double>("BufferDonHang")), TenSP = p.Key.TenSP, He = p.Key.He,ArticleNumber=p.Key.ArticleNumber }).OrderBy(p => p.TenSP).OrderBy(p => p.MaHD.ToString()).ToList();
                var query_DonHangSP = dtdonhangmaster.AsEnumerable().GroupBy(p => new { MaDH = p.Field<string>("MaHD"), MaSP = p.Field<string>("MaSP"), MaMau = p.Field<string>("MaMau") }).Select(p => new { MaHD = p.Key.MaDH.ToString(), MaSP = p.Key.MaSP, TongSP = p.Sum(n => n.Field<double>("SoLuong")), BufferDonHang = p.Sum(n => n.Field<double>("Buffer")), MaMau = p.Key.MaMau }).OrderBy(p => p.MaHD.ToString()).ToList();

                foreach (var it in query_DonHangSP)
                {

                    DataRow row = dt_resultDH.NewRow();

                    row["MaHDGroup"] = it.MaHD;
                    row["MaHDSort"] = it.MaHD;
                    row["MaMau"] = it.MaMau;

                    row["MaHD"] = it.MaHD;
                    row["MaSP"] = it.MaSP;
                    row["TongDH"] = it.TongSP;
                    row["BufferDonHang"] = it.BufferDonHang;
                    dt_resultDH.Rows.Add(row);

                }


                string MaSP = "", MaMau;
                foreach (DataRow row in dtdonhangmaster.Rows)
                {
                    string_temp = row["MaHD"].ToString();
                    MaSP = row.Field<string>("MaSP");
                    MaMau = row.Field<string>("MaMau");//.ToString();
                    foreach (DataRow rowresult in dt_resultDH.Rows)
                    {
                        if (string_temp == rowresult["MaHD"].ToString() && MaSP == rowresult.Field<string>("MaSP") && MaMau == rowresult.Field<string>("MaMau"))
                        {
                            if (row.Field<string>("PhanLoai") == "LowSeason")
                                rowresult["LowSeason"] = rowresult.Field<double>("LowSeason") + row.Field<double>("SoLuong");
                            else
                                rowresult["HighSeason"] = rowresult.Field<double>("HighSeason") + row.Field<double>("SoLuong");
                            break;
                            //rowresult[row["PhanLoai"].ToString()] = double.Parse(rowresult[row["PhanLoai"].ToString()].ToString()) + double.Parse(row["SoLuong"].ToString());
                            //break;
                        }
                    }
                }

                string_temp = string.Format(@"	select art.MaSP,art.MaMau,sum(qry.TonKho) as TonKho from 
								(SELECT  ArticleNumberID, SUM(SLNhap) AS TonKho
                                                        FROM    {1}dbo.KhoTP_NK
														where IDNumber not in (select IDNumber from {1}dbo.KhoTP_XK)
                                                        GROUP BY ArticleNumberID) as qry
														inner join {1}dbo.ArticleNumberProduct art
														on qry.ArticleNumberID= art.ArticleNumber
														group by art.MaSP,art.MaMau", DieuKienDonHang,serverobject);
                //}
               
                DataTable dt_TonKhoTP = new DataTable();
                json = await callAPI.ExcuteQueryEncryptAsync(string_temp, new List<ParameterDefine>());
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<DataTable>(json);
                    dt_TonKhoTP = query;
                    //await InvokeAsync(StateHasChanged);
                }
                //Xử lý Thông tin Art trước
                string MaSPDH = "";
                foreach (DataRow rowresult in dt_resultDH.Rows)
                {
                    MaSPDH = rowresult["MaSP"].ToString();
                    MaMau = rowresult.Field<string>("MaMau");
                    foreach (DataRow rowart in dtart.Rows)
                    {
                        if (MaSPDH == rowart.Field<string>("MaSP") && MaMau == rowart.Field<string>("MaMau"))
                        {
                            rowresult["Gia"] = rowart["Gia"];
                            rowresult["ArticleNumber"] = rowart["ArticleNumber"];
                            rowresult["TenSP"] = rowart["TenSP"];
                            rowresult["KhachHang"] = rowart["TenKH"];
                            rowresult["TenMau"] = rowart["TenMau"];
                            //rowresult["He"] = rowart["He"];
                            if(rowart["Color"]==DBNull.Value)
                            {
                                rowresult["ColorHex"] = null;
                            }
                            else
                                rowresult["ColorHex"] =StaticClass.UIntToHtmlColor((uint.Parse(rowart["Color"].ToString())));
                            break;
                        }
                    }
                }

                foreach (DataRow rowresult in dt_resultDH.Rows)
                {
                    string_temp = rowresult["MaHD"].ToString();
                    MaSPDH = rowresult["MaSP"].ToString();
                    MaMau = rowresult.Field<string>("MaMau");
                    foreach (DataRow rowtn in dtdonhangthucnhan.Rows)
                    {
                        if (MaSPDH == rowtn.Field<string>("MaSP") && MaMau == rowtn.Field<string>("MaMau"))
                        {

                            if (string_temp == rowtn.Field<string>("SoHD"))
                            {
                                rowresult["SLDonHangNhan"] = rowtn["SLDonHang"];
                                rowresult["SLDaXuat"] = rowtn["SLDaXuat"];
                                rowresult["SLPhaiXuat"] = (rowtn.Field<double>("SLPhaiXuat") < 0) ? 0 : rowtn["SLPhaiXuat"];
                                rowresult["SLDonHangNo"] = rowresult.Field<double>("TongDH") - rowresult.Field<double>("SLDonHangNhan");//.ToString());
                                rowresult["Other"] = rowresult["SLPhaiXuat"];
                                if (rowresult.Field<double>("TongDH") <= 0)
                                {
                                    rowresult["TongDH"] = rowresult["SLDonHangNhan"];
                                }
                                break;
                            }
                        }
                    }
                }
                double dtk = 0;
                foreach (DataRow rowresult in dt_resultDH.Rows)
                {
                    string_temp = rowresult["MaHD"].ToString();
                    MaSPDH = rowresult["MaSP"].ToString();
                    MaMau = rowresult.Field<string>("MaMau");

                    foreach (DataRow rowtk in dt_TonKhoTP.Rows)
                    {

                        if (MaSPDH == rowtk.Field<string>("MaSP") && MaMau == rowtk.Field<string>("MaMau"))
                        {
                            dtk = rowtk.Field<double>("TonKho");

                            if (dtk >= rowresult.Field<double>("SLPhaiXuat"))
                            {
                                rowresult["SLTonKho"] = rowresult.Field<double>("SLPhaiXuat");
                                rowtk["TonKho"] = dtk - rowresult.Field<double>("SLPhaiXuat");

                            }
                            else
                            {
                                rowresult["SLTonKho"] = dtk;
                                rowtk["TonKho"] = 0;

                            }

                            break;
                        }
                    }
                    double d = rowresult.Field<double>("SLPhaiXuat") - rowresult.Field<double>("SLTonKho");

                    rowresult["SLPhaiNhap"] = (d < 0) ? 0 : d;

                    //rowresult["SLPhaiNhap"] = rowresult.Field<double>("SLPhaiXuat") - rowresult.Field<double>("SLTonKho");
                    rowresult["SLPhaiNhapTongDH"] = rowresult.Field<double>("TongDH") - rowresult.Field<double>("SLDaXuat") - rowresult.Field<double>("SLTonKho");
                }
                //Xử lý nếu phần tồn vẫn còn
                var querytk = dt_TonKhoTP.Select("TonKho>0");
                if (querytk.Any())
                {
                    DataTable dttkcl = querytk.CopyToDataTable();
                    foreach (DataRow rowtk in dttkcl.Rows)
                    {
                        for (int k = dt_resultDH.Rows.Count - 1; k >= 0; k--)
                        {

                            if (rowtk.Field<string>("MaSP") == dt_resultDH.Rows[k].Field<string>("MaSP") && rowtk.Field<string>("MaMau") == dt_resultDH.Rows[k].Field<string>("MaMau"))
                            {
                                DataRow rowresult = dt_resultDH.Rows[k];
                                rowresult["SLTonKho"] = rowresult.Field<double>("SLTonKho") + rowtk.Field<double>("TonKho");
                                rowtk["TonKho"] = 0;
                                double d = rowresult.Field<double>("SLPhaiXuat") - rowresult.Field<double>("SLTonKho");

                                rowresult["SLPhaiNhap"] = (d < 0) ? 0 : d;

                                //rowresult["SLPhaiNhap"] = rowresult.Field<double>("SLPhaiXuat") - rowresult.Field<double>("SLTonKho");
                                rowresult["SLPhaiNhapTongDH"] = rowresult.Field<double>("TongDH") - rowresult.Field<double>("SLDaXuat") - rowresult.Field<double>("SLTonKho");
                                break;
                            }
                        }
                    }

                }

                //Duyệt cho đơn loại kho đến
                //Xử lý dòng tổng //Đảm bảo mảng 
                var querygroupDH = dt_resultDH.AsEnumerable().GroupBy(p => new { MaDH = p.Field<string>("MaHD") })

                       .Select(p => new {
                           MaDH = p.Key.MaDH,
                           LowSeason = p.Sum(n => n.Field<double>("LowSeason") * n.Field<double>("Gia")),
                           HighSeason = p.Sum(n => n.Field<double>("HighSeason") * n.Field<double>("Gia")),
                           TongDH = p.Sum(n => n.Field<double>("TongDH") * n.Field<double>("Gia")),
                           SLDonHangNhan = p.Sum(n => n.Field<double>("SLDonHangNhan") * n.Field<double>("Gia")),
                           SLDonHangNo = p.Sum(n => n.Field<double>("SLDonHangNo") * n.Field<double>("Gia")),
                           SLDaXuat = p.Sum(n => n.Field<double>("SLDaXuat") * n.Field<double>("Gia")),
                           SLPhaiXuat = p.Sum(n => n.Field<double>("SLPhaiXuat") * n.Field<double>("Gia")),
                           DD = p.Sum(n => n.Field<double>("DD") * n.Field<double>("Gia")),
                           DC = p.Sum(n => n.Field<double>("DC") * n.Field<double>("Gia")),
                           Other = p.Sum(n => n.Field<double>("Other") * n.Field<double>("Gia")),
                           SLTonKho = p.Sum(n => n.Field<double>("SLTonKho") * n.Field<double>("Gia")),
                           SLPhaiNhap = p.Sum(n => n.Field<double>("SLPhaiNhap") * n.Field<double>("Gia")),
                           SLPhaiNhapTongDH = p.Sum(n => n.Field<double>("SLPhaiNhapTongDH") * n.Field<double>("Gia"))
                       }).ToList();
                foreach (var it in querygroupDH)
                {
                    //DataRow row = dt_resultDH.NewRow();
                    DataRow rowheader = dt_resultDH.NewRow();
                    rowheader["KhachHang"] = "";
                    rowheader["TenSP"] = it.MaDH;
                    rowheader["MaHDSort"] = it.MaDH + "-Total";
                    rowheader["MaHD"] = "Break";
                    rowheader["MaHDGroup"] = it.MaDH;
                    rowheader["MaSP"] = "";
                    rowheader["LowSeason"] = it.LowSeason;
                    rowheader["HighSeason"] = it.HighSeason;
                    rowheader["TongDH"] = it.TongDH;
                    rowheader["SLDonHangNhan"] = it.SLDonHangNhan;
                    rowheader["SLDonHangNo"] = it.SLDonHangNo;
                    rowheader["SLDaXuat"] = it.SLDaXuat;
                    rowheader["SLPhaiXuat"] = it.SLPhaiXuat;
                    rowheader["DD"] = it.DD;
                    rowheader["DC"] = it.DC;
                    rowheader["Other"] = it.Other;
                    rowheader["SLTonKho"] = it.SLTonKho;
                    rowheader["SLPhaiNhap"] = it.SLPhaiNhap;
                    rowheader["SLPhaiNhapTongDH"] = it.SLPhaiNhapTongDH;

                    dt_resultDH.Rows.Add(rowheader);
                    //check_ = it.MaHD;
                }
                var querytotal = querygroupDH.GroupBy(p => 1)
                    .Select(p => new
                    {
                        LowSeason = p.Sum(n => n.LowSeason),
                        HighSeason = p.Sum(n => n.HighSeason),
                        TongDH = p.Sum(n => n.TongDH),
                        SLDonHangNhan = p.Sum(n => n.SLDonHangNhan),
                        SLDonHangNo = p.Sum(n => n.SLDonHangNo),
                        SLDaXuat = p.Sum(n => n.SLDaXuat),
                        SLPhaiXuat = p.Sum(n => n.SLPhaiXuat),
                        DD = p.Sum(n => n.DD),
                        DC = p.Sum(n => n.DC),
                        Other = p.Sum(n => n.Other),
                        SLTonKho = p.Sum(n => n.SLTonKho),
                        SLPhaiNhap = p.Sum(n => n.SLPhaiNhap),
                        SLPhaiNhapTongDH = p.Sum(n => n.SLPhaiNhapTongDH)

                    }).ToList();
                foreach (var it in querytotal)
                {
                    //DataRow row = dt_resultDH.NewRow();
                    DataRow rowheader = dt_resultDH.NewRow();
                    rowheader["MaHDSort"] = "zzzzzzz";
                    rowheader["KhachHang"] = "";
                    rowheader["MaHDGroup"] = "z. TỔNG ĐƠN HÀNG";
                    rowheader["TenSP"] = "TỔNG ĐƠN HÀNG";
                    rowheader["MaHD"] = "Break";
                    rowheader["MaSP"] = "";
                    rowheader["LowSeason"] = it.LowSeason;
                    rowheader["HighSeason"] = it.HighSeason;
                    rowheader["TongDH"] = it.TongDH;
                    rowheader["SLDonHangNhan"] = it.SLDonHangNhan;
                    rowheader["SLDonHangNo"] = it.SLDonHangNo;
                    rowheader["SLDaXuat"] = it.SLDaXuat;
                    rowheader["SLPhaiXuat"] = it.SLPhaiXuat;
                    rowheader["DD"] = it.DD;
                    rowheader["DC"] = it.DC;
                    rowheader["Other"] = it.Other;
                    rowheader["SLTonKho"] = it.SLTonKho;
                    rowheader["SLPhaiNhap"] = it.SLPhaiNhap;
                    rowheader["SLPhaiNhapTongDH"] = it.SLPhaiNhapTongDH;

                    dt_resultDH.Rows.Add(rowheader);
                    //check_ = it.MaHD;
                }
                dt_resultDH.DefaultView.Sort = $"MaHDSort asc";
                dt_resultDH = dt_resultDH.DefaultView.ToTable();
                DataColumn cl = dt_resultDH.Columns["MaHDSort"];
                dt_resultDH.Columns.Remove(cl);


                querygroupDH.Clear();
                querytotal.Clear();

                dt_TonKhoTP.Clear();
                dtdonhangthucnhan.Clear();
                dt_TonKhoTP.Dispose();
                query_DonHangSP.Clear();
                //query_PhanLoai.Clear();
                //var query_TotalHD = dt_resultDH.AsEnumerable().GroupBy(p => new { MaHD = p.Field<string>("MaHD") }).Select(p => new {MaHD=p.Key.MaHD, });
                //Code file excel
            }
            dtart.Clear();
            dtdonhangmaster.Clear();
            return dt_resultDH;
        }
    }
}
