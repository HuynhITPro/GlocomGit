using DevExpress.Blazor;
using DevExpress.XtraCharts.Native;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.CompilerServices;
using static NFCWebBlazor.App_KeHoach.Page_KeHoachThang_Master;

namespace NFCWebBlazor.App_ModelClass
{
    public class AllClass
    {
    }



    public class Ketqua
    {
        public string UserDuyet { get; set; }
        public string LoaiDuyet { get; set; }
        public int Serial { get; set; }
        public string? ketqua { get; set; }
        public string? ketquaexception { get; set; } = "";

    }
    public partial class SanPhamDropDown
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string TenSPSearch { get { return TenSP + " - " + MaSP; } }
        public string KhachHang { get; set; }
        public string He { get; set; }
    }
    public partial class NvlHangHoaDropDown
    {
        public int Serial { get; set; }
        [MinLength(1)]
        [Required(ErrorMessage = "Vui lòng chọn mã hàng")]
        public string MaHang { get; set; }

        [MinLength(1)]
        [Required(ErrorMessage = "Vui lòng chọn tên hàng")]
        public string TenHang { get; set; }

        public string QuyCach { get; set; }

        public string MaNhom { get; set; }
        public string PhanLoai { get; set; }

        public string DVT { get; set; }
        public double? SLTon { get; set; }
        public decimal? DonGia { get; set; }
    }

    public class Progressbarclass
    {
        public int max { get; set; }
        public int min { get; set; }
        private double? _value { get; set; }
        public double? value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                if (_value == null)
                {
                    _value = min;
                }
            }
        }
    }
    public class FileHoSoAPI
    {
        public int Serial { get; set; }
        public string TableName { get; set; }
        public int? SerialLink { get; set; }
        public string TenFile { get; set; }
        public string UrlFile { get; set; }
        public string DienGiai { get; set; }
        public double? DungLuong { get; set; }
        public string Dvt { get; set; }
        public string UserInsert { get; set; }

        public int? SerialRoot { get; set; }
        public string? TableNameRoot { get; set; }
        public string? NoiDungRoot { get; set; }
        public DateTime? NgayInsert { get; set; }
    }
    public partial class FileHoSo
    {
        public int Serial { get; set; }

        public bool isDowload { get; set; } = false;
        private string _tenfile { get; set; }

        public string TenFile
        {
            get { return _tenfile; }
            set
            {

                _tenfile = value;
                if (_tenfile == null)
                    return;
                string extention = Path.GetExtension(_tenfile).ToLower();
                switch (extention)
                {
                    case ".xlsx":
                        PathIcon = IconImg.xlsx;
                        break;
                    case ".docx":
                        PathIcon = IconImg.docx;
                        break;
                    case ".pdf":
                        PathIcon = IconImg.pdf;
                        break;
                    case ".cdr":
                        PathIcon = IconImg.cdr;
                        break;
                    case ".psd":
                        PathIcon = IconImg.psd;
                        break;
                    case ".pptx":
                        PathIcon = IconImg.pptx;
                        break;
                    case ".png":
                        PathIcon = IconImg.image;
                        break;
                    case ".jpg":
                        PathIcon = IconImg.image;
                        break;
                    case ".jpeg":
                        PathIcon = IconImg.image;
                        break;
                    case ".bmp":
                        PathIcon = IconImg.image;
                        break;
                    default:
                        PathIcon = IconImg.file;
                        break;

                }
            }
        }

        public string UserInsert { get; set; }
        public string DienGiai { get; set; }
        public double? DungLuong { get; set; }
        public string UrlFile { get; set; }
        public string PathIcon
        {
            get; set;
        }
        public string Showtext { get; set; } = "Tải file";


    }
    public class FileHoSoGroup : FileHoSo
    {
        public int SerialGroup { get; set; }

        public string NoiDung
        {
            get;
            set;
        }
        public string TenUserUpload
        {
            get; set;
        }
        public string PathImgUser { get; set; }


    }
    public class InitDxGridDataColumn
    {
        public InitDxGridDataColumn(int _Index, string _FieldName, string _Caption)
        {
            Caption = _Caption;
            FieldName = _FieldName;
            Index = _Index;
        }
        public InitDxGridDataColumn(int _Index, string _FieldName, string _Caption, int _GroupIndex)
        {
            Caption = _Caption;
            FieldName = _FieldName;
            GroupIndex = _GroupIndex;
            Index = _Index;
        }
        public InitDxGridDataColumn(int _Index, string _FieldName, string _Caption, string _DisplayFormat)
        {
            Caption = _Caption;
            FieldName = _FieldName;
            DisplayFormat = _DisplayFormat;
            Index = _Index;
        }
        public InitDxGridDataColumn(int _Index, string _FieldName, string _Caption, string _DisplayFormat, int _Width)
        {
            Caption = _Caption;
            FieldName = _FieldName;
            DisplayFormat = _DisplayFormat;
            Width = _Width;
            Index = _Index;
        }
        public InitDxGridDataColumn(int _Index, string _FieldName, string _Caption, string _DisplayFormat, int _Width, GridTextAlignment _gridTextAlignment)
        {
            Caption = _Caption;
            FieldName = _FieldName;
            DisplayFormat = _DisplayFormat;
            Width = _Width;
            gridTextAlignment = _gridTextAlignment;
            Index = _Index;
        }
        public int Index { get; set; } = 0;
        public string Caption { get; set; }
        public string FieldName { get; set; }
        public int? GroupIndex { get; set; }

        public string? DisplayFormat { get; set; }
        public int? Width { get; set; }
        public GridTextAlignment? gridTextAlignment { get; set; }
    }

    public class QuyDoiNgay
    {
        public QuyDoiNgay(DateTime ngay, string ngayoutput)
        {
            Ngay = ngay;
            Ngayoutput = ngayoutput;
        }
        public DateTime Ngay { get; set; }
        public string Ngayoutput { get; set; }

    }
    public class User_PhanQuyen
    {
        public string TableName { get; set; }
        public string UserName { get; set; }
        public string Permission { get; set; }

    }

    public class FtpRequest
    {
        public string FtpUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class NvlKyDuyetItemShow
    {
        public int Serial { get; set; }

        public int? SerialLinkMaster { get; set; }

        public int? SerialLinkItem { get; set; }

        public string TableName { get; set; }

        public string UserDuyet { get; set; }

        public string LoaiDuyet { get; set; }
        public string TenUserDuyet { get; set; }

        public string GhiChu { get; set; }
        public int countItemDuyet { get; set; }
        public DateTime? NgayInsert { get; set; }
    }
    public class NvlKyDuyetShow
    {
        public int Serial { get; set; }

        public int? SerialLink { get; set; }

        public string TableName { get; set; }

        public string UserYeuCau { get; set; }
        public string PathImg { get; set; }
        public string UserDuyet { get; set; }
        public string TenUserDuyet { get; set; }
        public string TenUser { get; set; }
        public string LoaiDuyet { get; set; }
        public string DVTT { get; set; }
        public DateTime? NgayApDung { get; set; }

        public DateTime? NgayKyDuyet { get; set; }

        public DateTime? NgayInsert { get; set; }

        public string DaDuyet { get; set; }
        public string KhongDuyet { get; set; }
        public int countItemDuyet { get; set; }
        public string GhiChu { get; set; }

        public string UserInsert { get; set; }

    }
    public class NvlDuyetGiaReport
    {
        public int Serial { get; set; }

        public int? STT { get; set; }

        public int? SerialLink { get; set; }

        public string TableName { get; set; }

        public string MaHang { get; set; }
        public string TenHang { get; set; }

        public string XuatXu { get; set; } = "";

        public string DVT { get; set; }

        public decimal? SLDuToan { get; set; }

        public decimal? DonGia { get; set; }

        public decimal? DonGiaTheoDvt { get; set; }

        public decimal? GiaDangMua { get; set; }

        public decimal? DinhMuc { get; set; }

        public string GhiChu { get; set; } = "";

        public string MaNCC { get; set; }
        private string _tenNhaCungCap { get; set; }
        public string TenNCC
        {
            get
            {
                return _tenNhaCungCap;
            }
            set
            {
                _tenNhaCungCap = value;

            }
        }

        public string UserInsert { get; set; }

        private string _tinhtrangduyet { get; set; }
        public string TinhTrangDuyet
        {
            get { return _tinhtrangduyet; }
            set
            {
                _tinhtrangduyet = value;

            }
        }

        public DateTime? NgayInsert { get; set; }


        public string TextKiem { get; set; } = "";
        public string TextDuyet { get; set; } = "";



        public string Err { get; set; }
        private string _msgwait { get; set; }
        public string MsgKhongDuyet
        {
            get { return _msgwait; }

        }
        public bool boolduyet { get; set; } = false;
        public string ListColumns { get; set; }

    }

    public class ShowMsgInbox
    {
        public string PathImg { get; set; }
        public string DienGiai { get; set; }
        public string ComponentName { get; set; }
        public string TextItem { get; set; }
    }

    public class NvlInTemShow()
    {
        public int Serial { get; set; }
        public int? BanIn { get; set; }
        public int? SerialCT { get; set; }

        public int MaCT { get; set; }

        public string? MaHang { get; set; }

        public string DVT { get; set; }

        public double? SoLuong { get; set; }

        public string KhachHangXuatXu { get; set; }

        public DateTime? NgayHetHan { get; set; }

        public string MaKien { get; set; }

        public string SoLo { get; set; }

        public string SoXe { get; set; }

        public string GhiChu { get; set; }

        public string Barcode { get; set; }

        public string MaSp { get; set; }
        public string MaGN { get; set; }


        public string ArticleNumber { get; set; }

        public string KhuVuc { get; set; }

        public string ChatLuong { get; set; }

        public int? CheckPrint { get; set; }

        public string DauTuan { get; set; }

        public DateTime? NgaySanXuat { get; set; }

        public string UserInsert { get; set; }

        public DateTime? NgayInsert { get; set; }
    }
    public class ShowFragmentinModal
    {
        public ShowFragmentinModal() { }
        public ShowFragmentinModal(RenderFragment _renderFragment, string _title, bool _show)
        {
            this.renderFragment = _renderFragment;
            this.title = _title;
            this.show = _show;
        }
        public RenderFragment renderFragment { get; set; }
        public string title { get; set; }
        public bool show { get; set; }
    }
    public class MaSoThueAPI
    {
        public string code { get; set; }
        public string desc { get; set; }
        public DataMST data { get; set; }
    }
    public class DataMST
    {
        public string id { get; set; }
        public string name { get; set; }
        public string internationalName { get; set; }
        public string shortName { get; set; }
        public string address { get; set; }
    }
    public class VideoHuongDan
    {
        public int Serial { get; set; }
        public string NoiDung { get; set; }
        public string LinkVideo { get; set; }
        public string TypeVideo { get; set; }
    }
    public class ThongTinTaiKhoan
    {
        public int Serial { get; set; }

        public string SoTK { get; set; } = string.Empty;

        public string? TenTaiKhoan { get; set; }

        public string? ChiNhanh { get; set; }

        public string? DiaChi { get; set; }

        public string? NganHang { get; set; }

        public string? MaNCC { get; set; }

        public string? Swiftcode { get; set; }

        public string? IBAN { get; set; }

        public string? UserInsert { get; set; }

        public DateTime? NgayInsert { get; set; }
        public ThongTinTaiKhoan CopyClass()
        {
            var json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<ThongTinTaiKhoan>(json);
        }

    }
    public class NvlMsg
    {
        public NvlMsg()
        {
        }
        public int Serial { get; set; }
        public string Msg { get; set; }
        public int SerialParent { get; set; }
        public int SerialLink { get; set; }
        public string TableName { get; set; }
        private string _userinsert { get; set; }
        public string TenUser { get; set; }
        public string UserInsert
        {
            get
            {
                return _userinsert;
            }
            set
            {
                _userinsert = value;
                if (_userinsert == ModelAdmin.users.UsersName)
                {
                    IsMine = true;
                }
            }
        }
        private string _pathimg { get; set; }
        public string PathImg
        {
            get
            {
                return _pathimg;
            }
            set
            {
                _pathimg = value;
                if(string.IsNullOrEmpty(_pathimg))
                    _pathimg = IconImg.User;
                else
                    _pathimg=ModelAdmin.pathurlfilepublic + '/'+_pathimg;
               
            }
        } 

        public bool IsMine { get; set; } = false;
        public DateTime NgayInsert { get; set; } = DateTime.Now;
        public NvlMsg CopyClass()
        {
            var json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<NvlMsg>(json);
        }
    }
}
