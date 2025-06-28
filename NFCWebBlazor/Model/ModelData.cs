using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Models;
using System;

namespace NFCWebBlazor.Model
{
    public class ModelData
    {

        public static List<DataDropDownList> dataDropDownLists;
        public static List<Users> lstusers;
        public static List<NvlHangHoaDropDown> lsthanghoa;
        public static List<DataDropDownList> lstnhomhang;
        public static List<DataDropDownList> lstdautuan;
        public static List<DataDropDownList> lstmamay;
        public static List<SanPhamDropDown> lstsanpham;
        public static List<DataDropDownList> lstdonhangmua;
        public static List<DataDropDownList> lstnoigiaonhan;
        public static List<DataDropDownList> lstphongbankhuvuc;
        public static List<DataDropDownList> lstnhacungcap;
        public static List<StepFlowChart> lstflowchart;
        public static List<DataDropDownList> lstkhonvl;
        public static List<NvlKhachHang> lstkhachhang;
        public static List<NvlKhachHang> lstkhachhangnvl;
        public static List<DataDropDownList> lstnvllydo;
        public static List<NvlViTri> lstViTri;
        public static List<User_PhanQuyen> lstphanquyen;
        public static List<DataDropDownList> lstsanphamdropdown;
        public static List<ArticleNumberProduct> lstArt;
        public static List<ThongTinTaiKhoan> lsttaikhoan;
        public static string serverlinkobject = "[SP].[DataBase_ScansiaPacific2014].";
        public static ClassProcess prs = new ClassProcess();
        public static async Task<List<DataDropDownList>> GetDataDropDownListsAsync()
        {
            if (dataDropDownLists != null) return dataDropDownLists;
            dataDropDownLists = new List<DataDropDownList>();

            CallAPI callAPI = new CallAPI();
            string sql = "select * from DataDropDownList";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                dataDropDownLists = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
            }


            return dataDropDownLists;
        }
        public static async Task<List<StepFlowChart>> GetFlowchart()
        {
            if (lstflowchart != null) return lstflowchart;
            lstflowchart = new List<StepFlowChart>();

            CallAPI callAPI = new CallAPI();
            string sql = "select * from StepFlowChart";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstflowchart = JsonConvert.DeserializeObject<List<StepFlowChart>>(json);
            }


            return lstflowchart;
        }
        public static async Task<List<User_PhanQuyen>> GetListUserPhanQuyen(string userName)
        {
            if (lstphanquyen != null) return lstphanquyen;
            lstphanquyen = new List<User_PhanQuyen>();

            CallAPI callAPI = new CallAPI();
            string sql = string.Format("SELECT [TableName],[UserName] ,[Permission]\r\nFROM [User_PhanQuyen] where UserName=@UserName");

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            parameterDefineList.Add(new ParameterDefine("@UserName", userName));
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstphanquyen = JsonConvert.DeserializeObject<List<User_PhanQuyen>>(json);
            }


            return lstphanquyen;
        }
        public static async Task<List<DataDropDownList>> GetDataDropDownListsAsync(string TypeName)
        {
            if (dataDropDownLists != null)
            {
                //Console.WriteLine(string.Format("TypeName: {0}", TypeName));
                var query = dataDropDownLists.Where(p => p.TypeName == TypeName).AsEnumerable().ToList();

                return query;
            }

            dataDropDownLists = new List<DataDropDownList>();

            CallAPI callAPI = new CallAPI();
            string sql = "select * from DataDropDownList";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                dataDropDownLists = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                var query = dataDropDownLists.Where(p => p.TypeName.Equals(TypeName)).ToList();
                return query;

            }
            List<DataDropDownList> lst = new List<DataDropDownList>();
            return lst;
        }
        public static async Task<List<DataDropDownList>> GetlstNhomhang()
        {
            if (lstnhomhang != null)
                return lstnhomhang;
            CallAPI callAPI = new CallAPI();
            string sql = "use NVLDB SELECT [MaNhom] as [Name],[TenNhom] as FullName FROM [NvlNhomHang]";
            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstnhomhang = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                return lstnhomhang;
                // StateHasChanged();
            }
            return lstnhomhang;

        }
        public static async Task<List<DataDropDownList>> Getlstdautuan()
        {
            if (lstdautuan != null)
                return lstdautuan;
            CallAPI callAPI = new CallAPI();
            string sql = "use NVLDB select Weeks as [Name],Weeks as [FullName],'wk' as TypeName from dbo.LoadWeek()";
            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstdautuan = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                return lstdautuan;
                // StateHasChanged();
            }
            return lstdautuan;

        }
        public static async Task<List<DataDropDownList>> Getlstnhacungcap()
        {
            if (lstnhacungcap != null)
                return lstnhacungcap;
            CallAPI callAPI = new CallAPI();
            string sql = "USE [NVLDB]\r\nSELECT [MaNCC] as [Name],[TenNCC]+' - '+[MaNCC] as FullName\r\n FROM [NvlNhaCungCap]";
            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstnhacungcap = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                return lstnhacungcap;
                // StateHasChanged();
            }
            return lstnhacungcap;

        }
        public static async Task<List<NvlHangHoaDropDown>> GetHangHoa()
        {
            if (lsthanghoa != null)
            {
                if (lsthanghoa.Count > 0)
                    return lsthanghoa;
            }

            lsthanghoa = new List<NvlHangHoaDropDown>();

            CallAPI callAPI = new CallAPI();
            string sql = @"Use NVLDB SELECT  hh.[MaHang],[TenHang]+' - '+hh.MaHang as TenHang,isnull(dg.DonGia,0) as DonGia,[DVT],nh.PhanLoai,nh.MaNhom
                FROM [dbo].[NvlHangHoa] hh
                left join dbo.DonGiaGanNhat() dg on hh.MaHang=dg.MaHang
                inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lsthanghoa = JsonConvert.DeserializeObject<List<NvlHangHoaDropDown>>(json);
                foreach (var it in lsthanghoa)
                {

                    it.TenHang = StaticClass.RemoveVietnamese(it.TenHang);
                }
            }
            return lsthanghoa;
        }

        public static async Task<List<NvlKhachHang>> GetKhachHangNvl()
        {
            if (lstkhachhangnvl != null) return lstkhachhangnvl;
            lstkhachhangnvl = new List<NvlKhachHang>();

            CallAPI callAPI = new CallAPI();
            string sql = " USE [NVLDB]  select MaKH,TenKH from NvlKhachHang union all SELECT [MaNB],[TenNB] FROM [NvlNoiBo]";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstkhachhangnvl = JsonConvert.DeserializeObject<List<NvlKhachHang>>(json);
            }


            return lstkhachhangnvl;
        }

        public static async Task<List<ThongTinTaiKhoan>> GetdtThongTinTaiKhoan()
        {

            if (lsttaikhoan != null)
            {
                return lsttaikhoan;
            }
            string sql = "";

            sql = @"use SPSupplier SELECT [SoTK],[TenTaiKhoan],MaNCC
                          ,[DiaChi],[NganHang]
                      FROM [ThongTinTaiKhoan]";

            CallAPI callAPI = new CallAPI();
            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lsttaikhoan = JsonConvert.DeserializeObject<List<ThongTinTaiKhoan>>(json);
            }
            return lsttaikhoan;

        }

        public static async Task<List<DataDropDownList>> Getlstnvllydo()
        {
            if (lstnvllydo != null) return lstnvllydo;
            lstnvllydo = new List<DataDropDownList>();

            CallAPI callAPI = new CallAPI();
            string sql = "Use NVLDB SELECT LyDo as FullName,Tag as [TypeName],LyDo as [Name],cast(isnull(TinhTien,0) as varchar) as PhanLoai from NvlNhapXuat_LyDo";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstnvllydo = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
            }


            return lstnvllydo;
        }
        public static async Task<List<DataDropDownList>> GetKhoNvl()
        {
            if (lstkhonvl != null) return lstkhonvl;
            lstkhonvl = new List<DataDropDownList>();

            CallAPI callAPI = new CallAPI();
            string sql = "use NVLDB select MaKho as [Name],TenKho as FullName from NvlMaKho";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstkhonvl = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
            }


            return lstkhonvl;
        }
        public static async Task<List<DataDropDownList>> Getlstnoigiaonhan()
        {
            if (lstnoigiaonhan != null) return lstnoigiaonhan;
            lstnoigiaonhan = new List<DataDropDownList>();

            CallAPI callAPI = new CallAPI();
            string sql = "use NVLDB SELECT MaGN as [Name],TenGN+' - '+MaGN as FullName,TableName as TypeName,PhanLoai from View_NoiGN";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstnoigiaonhan = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                foreach (var it in lstnoigiaonhan)
                {
                    it.FullName = StaticClass.RemoveVietnamese(it.FullName);
                }
            }


            return lstnoigiaonhan;
        }
        public static async Task<List<DataDropDownList>> Getlstphongbankhuvuc()
        {
            if (lstphongbankhuvuc != null) return lstphongbankhuvuc;
            lstphongbankhuvuc = new List<DataDropDownList>();

            CallAPI callAPI = new CallAPI();
            string sql = "use NVLDB SELECT TenGN as [Name],TenGN as FullName,TableName as TypeName,PhanLoai from View_NoiGN where  TableName='NB'";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstphongbankhuvuc = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                foreach (var it in lstnoigiaonhan)
                {
                    it.FullName = StaticClass.RemoveVietnamese(it.FullName);
                }
            }


            return lstphongbankhuvuc;
        }
        public static async Task<List<NvlHangHoaDropDown>> GetHangHoaTonKho()
        {

            List<NvlHangHoaDropDown> lsthanghoatonkho = new List<NvlHangHoaDropDown>();

            CallAPI callAPI = new CallAPI();
            string sql = @"Use NVLDB SELECT hh.[MaHang],TenHang +' - '+hh.MaHang as TenHang
                        , DVT, isnull(qry.SLTon, 0) as SLTon
                    FROM [NvlHangHoa] hh
                   inner join dbo.NvlNhomHang nh on hh.MaNhom = nh.MaNhom
                    left join(select MaHang, sum(SLNhap-SLXuat) as SLTon from dbo.NvlNhapXuatItem group by MaHang) as qry
                    on hh.MaHang = qry.MaHang";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lsthanghoatonkho = JsonConvert.DeserializeObject<List<NvlHangHoaDropDown>>(json);
            }


            return lsthanghoatonkho;
        }
        public static async Task<List<SanPhamDropDown>> GetSanPham()
        {
            if (lstsanpham != null) return lstsanpham;
            lstsanpham = new List<SanPhamDropDown>();

            CallAPI callAPI = new CallAPI();
            string sql = @"SELECT * 
            FROM OPENQUERY(SP, 'SELECT * FROM DataBase_ScansiaPacific2014.dbo.GetSanPham()')";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstsanpham = JsonConvert.DeserializeObject<List<SanPhamDropDown>>(json);
            }


            return lstsanpham;
        }
        public static async Task<List<DataDropDownList>> GetMaMay()
        {
            if (lstmamay != null) return lstmamay;
            //lstmamay = new List<DataDropDownList>();

            CallAPI callAPI = new CallAPI();
            string sql = @"SELECT * 
            FROM OPENQUERY(SP, ' SELECT [MaMay] as [Name],MaMay+'' - ''+[TenMay] as FullName FROM NguyenVatLieu.dbo.[ThietBi]')";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstmamay = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
            }


            return lstmamay;
        }
        public static async Task<List<DataDropDownList>> GetSanPhamDropDown()
        {

            if (lstsanphamdropdown != null) return lstsanphamdropdown;
            lstsanphamdropdown = new List<DataDropDownList>();

            CallAPI callAPI = new CallAPI();
            string sql = "SELECT  [MaSP] as [Name],[TenSP]+' - '+MaSP as FullName,'SanPham' as TypeName FROM [SP].[DataBase_ScansiaPacific2014].[dbo].[SanPham]";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstsanphamdropdown = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
            }


            return lstsanphamdropdown;

        }
        public static async Task<List<NvlViTri>> GetListViTri()
        {

            if (lstViTri != null) return lstViTri;
            lstViTri = new List<NvlViTri>();

            CallAPI callAPI = new CallAPI();
            string sql = string.Format("use NVLDB declare @NhaMay nvarchar(100)=N'{0}' " +
                "SELECT [ViTri],[NhaMay],MaKho ,[Ke],[Day],[Pallet],[Tang]" +
                "  FROM NVLDB.[dbo].[NvlSoDoKho] ORDER BY Ke,[Day],[Tang]", ModelAdmin.users.NhaMay);

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstViTri = JsonConvert.DeserializeObject<List<NvlViTri>>(json);
            }


            return lstViTri;

        }
        public static async Task<List<ArticleNumberProduct>> GetArticleNumber()
        {

            if (lstArt != null) return lstArt;
            lstArt = new List<ArticleNumberProduct>();

            CallAPI callAPI = new CallAPI();
            string sql = "SELECT [MaSP],[ArticleNumber],mm.Color,mm.TenMau" +
                "  FROM  [SP].[DataBase_ScansiaPacific2014].[dbo].[ArticleNumberProduct] art" +
                "  inner join [SP].[DataBase_ScansiaPacific2014].dbo.MaMau mm on art.MaMau=mm.MaMau";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstArt = JsonConvert.DeserializeObject<List<ArticleNumberProduct>>(json);

            }


            return lstArt;

        }
        public static async Task<List<DataDropDownList>> GetDonHangMua()
        {
            if (lstdonhangmua != null) return lstdonhangmua;
            lstdonhangmua = new List<DataDropDownList>();

            CallAPI callAPI = new CallAPI();
            string sql = string.Format("select sp.KhachHang as TypeName,qry.MaHD as [Name],qry.MaHD as FullName from " +
                "\r\n   (SELECT [MaHD],MaSP\r\n                      " +
                "FROM {0}dbo.[DonHangMua]\r\n                      " +
                "group by [MaHD],MaSP) as qry inner join {0}dbo.Load_cbSP sp\r\n                      " +
                "on qry.MaSP=sp.MaSP\r\n                      " +
                "group by sp.KhachHang,qry.MaHD ", serverlinkobject);

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstdonhangmua = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
            }


            return lstdonhangmua;
        }
        static List<NvlKhachHang> lstkhachhangsp;
        public static async Task<List<NvlKhachHang>> GetKhachHangSanSuatSP()
        {
            if (lstkhachhangsp == null)
            {



                lstkhachhangsp = new List<NvlKhachHang>();

                CallAPI callAPI = new CallAPI();
                string sql = string.Format("select MaKH,TenKH+' - '+MaKH as TenKH from {0}dbo.KhachHang", serverlinkobject);

                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {

                    lstkhachhangsp = JsonConvert.DeserializeObject<List<NvlKhachHang>>(json);
                }

            }
            return lstkhachhangsp;

        }
        public static async Task<List<NvlKhachHang>> GetListKhachHang()
        {
            if (lstkhachhang == null)
            {



                lstkhachhang = new List<NvlKhachHang>();

                CallAPI callAPI = new CallAPI();
                string sql = "select MaKH,TenKH+' - '+MaKH as TenKH from dbo.KhachHang where NoiBo is null";

                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {

                    lstkhachhang = JsonConvert.DeserializeObject<List<NvlKhachHang>>(json);
                }

            }
            return lstkhachhang;

        }
        public static async Task<List<Users>> Getlstusers()
        {
            if (lstusers != null) return lstusers;


            CallAPI callAPI = new CallAPI();
            string sql = string.Format("SELECT [UsersName],[GroupUser],[KhuVuc] ,[DateAccess],[Email],[TenUser],isnull('{0}'+[PathImg],'images/user.png') as [PathImg]\r\n     \r\n  FROM [Users]", ModelAdmin.pathurlfilepublic);

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstusers = JsonConvert.DeserializeObject<List<Users>>(json);
            }


            return lstusers;
        }
    }
}
