using BlazorBootstrap;
using DevExpress.Blazor;
using DevExpress.Utils.Filtering.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;

namespace NFCWebBlazor.App_Bom
{
    public partial class Page_DuyetDinhMuc
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] BrowserService browserService { get; set; }
        protected override Task OnInitializedAsync()
        {

            return base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _ = loadAsync();

            }
            try
            {
                var dimension = await browserService.GetDimensions();
                // var heighrow = await browserService.GetHeighWithID("divcontainer");
                int height = dimension.Height - 70;
                //if (heighrow!=null)
                //{
                //    height = dimension.Height - heighrow;
                //}

                heightgrid = string.Format("{0}px", height);
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi browserService :" + ex.Message));
            }


            //await JS.InvokeVoidAsync("scrollToBottomLast");

            //base.OnAfterRender(firstRender);
        }
        int columnsbegin = 0;
        private async Task loadAsync()
        {
            try
            {
                var query = await Model.ModelData.GetSanPham();
                lstsanpham = query.ToList();
                var query2 = await Model.ModelData.GetListKhachHang();
              
                foreach (var it in dxGrid.GetColumns())
                {
                    columnsbegin++;
                }
                baocaoselected = txtloaibaocao.SelectedValue("Đã duyệt");
            }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi loadasyn :" + ex.Message));
            }
            //baocaoselected = lstloaibaocao.FirstOrDefault(p => p.Name.Equals("Tồn kho theo sản phẩm"));
            StateHasChanged();

        }
        public RenderFragment buildrender()
        {
            return BuildColumns(lstcolumn);
        }
        public static RenderFragment BuildColumns(List<InitDxGridDataColumn> _lstcolumn)
        {
            RenderFragment columns = b =>
            {
                int counter = 0;
                foreach (InitDxGridDataColumn col in _lstcolumn.OrderBy(p => p.Index))
                {
                    b.OpenComponent(counter, typeof(DxGridDataColumn));
                    b.AddAttribute(0, "FieldName", col.FieldName);

                    b.AddAttribute(0, "Caption", col.Caption);
                    if (col.gridTextAlignment != null)
                        b.AddAttribute(0, "TextAlignment", col.gridTextAlignment);
                    if (col.DisplayFormat != null)
                    {
                        b.AddAttribute(0, "DisplayFormat", col.DisplayFormat);
                    }
                    if (col.Width != null)
                        b.AddAttribute(0, "Width", string.Format("{0}px", col.Width));
                    else
                    if (col.Width != null)
                        b.AddAttribute(0, "MinWidth", string.Format("90px"));
                    if (col.GroupIndex != null)
                    {
                        b.AddAttribute(0, "GroupIndex", col.GroupIndex.Value);
                    }
                    b.CloseComponent();

                    counter++;
                }



            };
            return columns;
        }

       


        private async void search()
        {
            if (baocaoselected.Name == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Vui lòng chọn loại báo cáo"));
                return;
            }
            lstcolumn.Clear();


           await searchasAsync();
            
          



            
            hidedivsearch();

            //dxGrid.AutoFitColumnWidths();
            StateHasChanged();


            //foreach (InitDxGridDataColumn it in lstcolumn)
            //{

            //}
        }
        public class ChiTiet_PhanHeShow
        {
            public int? STT { get; set; }
            public string? MaChiTiet { get; set; }
            public string? MaChiTietParent { get; set; }
            public string? MaSP { get; set; }
            public string? He { get; set; }
            public string? MaNhom { get; set; }
            public string? TenChiTiet { get; set; }
            public double? SLCT { get; set; }
            public int? Level { get; set; }
            public Nullable<double> ChieuDayTC { get; set; }
            public Nullable<double> ChieuRongTC { get; set; }
            public Nullable<double> ChieuDaiTC { get; set; }
            public Nullable<double> SLTGshow
            {
                get
                {
                    if (NhomChiTiet == "Thanh ghép")
                        return SLCT;
                    else return null;
                }
            }
            public Nullable<double> SLCTshow
            {
                get
                {
                    if (NhomChiTiet != "Thanh ghép")
                        return SLCT;
                    else return null;
                }
            }
            public Nullable<double> ChieuDaySC { get; set; }
            public Nullable<double> ChieuRongSC { get; set; }
            public Nullable<double> ChieuDaiSC { get; set; }
            public Nullable<double> SoKhoiSC { get; set; }
            public Nullable<double> SoKhoiTC { get; set; }
            public int? SignKhoiSC { get; set; }
            public int? SignKhoiTC { get; set; }
            public string? ChatLuong { get; set; }
            public string? LoaiGo { get; set; }
            public Nullable<int> LanSuaDoi { get; set; }
            public Nullable<double> KLTC { get; set; }
            public Nullable<double> KLSC { get; set; }
            public Nullable<double> SXquanhTC { get; set; }
            public string? STTLevel { get; set; }
            public bool? CheckItem { get; set; }
            public Nullable<double> STraiKeo { get; set; }
            public string? LoaiGhep { get; set; }
            public string? GhiChu { get; set; }
            public string? NhomChiTiet { get; set; }
            private string? _PathImg { get; set; }
            public string? PathImg
            {
                get { return _PathImg; }
                set
                {
                    _PathImg = value;
                    if (string.IsNullOrEmpty(PathImg))
                    {
                        ImgShow = IconImg.NotCheck;
                    }
                    else
                        ImgShow = IconImg.CheckMark;

                 
                }
            }
            public string? ImgShow { get; set; }



          
        }
        public class DinhMuc_Show 
        {
            public int Serial { get; set; }
            public string MaSP { get; set; }
            public string TenSP { get; set; }
            public string PathImgKiemTra { get; set; } = Model.ModelAdmin.pathurlfilepublic + "/" + "UserImage/user.png";
            public string PathImgDuyet { get; set; } = Model.ModelAdmin.pathurlfilepublic + "/" + "UserImage/user.png";
            public string KichThuoc { get; set; }
            public int Version { get; set; }
            public string _checkKiem { get; set; }
            public string _checkDuyet { get; set; }
            private Nullable<DateTime> _ngayApDung { get; set; }
            private Nullable<DateTime> _ngayBanHanh { get; set; }
            public string? NameDuyet { get; set; }
            public string? NameKiem { get; set; }

            public string CheckKiem
            {
                get { return _checkKiem; }
                set
                {
                    _checkKiem = value;
                   
                }
            }
            public string CheckDuyet
            {
                get { return _checkDuyet; }
                set
                {
                    _checkDuyet = value;
                  
                }
            }
            public string? ChucNang { get; set; }
            public Nullable<DateTime> NgayApDung
            {
                get { return _ngayApDung; }
                set
                {
                    _ngayApDung = value;
                   
                }
            }
            public Nullable<DateTime> NgayBanHanh
            {
                get { return _ngayBanHanh; }
                set
                {
                    _ngayBanHanh = value;
                   
                }
            }
            private string? _duyetTruoc { get; set; }
            public string? DuyetTruoc
            {
                get { return _duyetTruoc; }
                set
                {
                    _duyetTruoc = value;

                }
            }
            private string? _duyetChinhThuc { get; set; }
            public string? DuyetChinhThuc
            {
                get { return _duyetChinhThuc; }
                set
                {
                    _duyetChinhThuc = value;

                }
            }
            public string? PathIcon
            {
                get { return _PathIcon; }
                set
                {
                    _PathIcon = value;
                  
                }
            }
            public string? KhachHang { get; set; }
            public string? TinhTrang { get; set; }

            public string? NguoiLap { get; set; }
            public List<ChiTiet_PhanHeShow> lstPhanHeShow;
          
            

          
            public string? PathImg { get; set; }
            private string? _PathIcon { get; set; }
        }
        public partial class KyDuyet
        {
            public int? Serial { get; set; }
            public string? MaSP { get; set; }
            public string? KhuVuc { get; set; }
            public string? UsersName { get; set; }
            public string? LoaiDuyet { get; set; }
            public Nullable<System.DateTime> Ngay { get; set; }
            public string? UserYeuCau { get; set; }
            public Nullable<System.DateTime> NgayInsert { get; set; }
            public string? TenQuyTrinh { get; set; }
        }
        private class usertmp
        {
            public string UsersName { get; set; }
            public string TenUser { get; set; }
            public string PathImg { get; set; }
        }

        string ghichu = "";
        App_ClassDefine.ClassProcess prs = new ClassProcess();
      
        string chucnang = "";
        List <DinhMuc_Show> lstDinhMucShow = new List<DinhMuc_Show>();
        private async Task searchasAsync()
        {
            lstDinhMucShow.Clear();
            string dieukien = "";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
           
            if (sanphamselected!=null)
            {
                if (dieukien == "")
                    dieukien = " where MaSP = @MaSP";
                else
                    dieukien += " and MaSP=@MaSP";
                lstpara.Add(new ParameterDefine("@MaSP", sanphamselected.MaSP));

            }
            if (baocaoselected!=null)
            {
                if (baocaoselected.Name == "Chưa duyệt")
                {
                    if (dieukien == "")
                        dieukien = " where DuyetChinhThuc is null";
                    else
                        dieukien += " and DuyetChinhThuc is null";
                }
                else
                {
                    if (baocaoselected.Name == "Đã duyệt")
                    {
                        if (dieukien == "")
                            dieukien = " where DuyetChinhThuc is not null";
                        else
                            dieukien += " and DuyetChinhThuc is not null";
                    }

                }
            }

            string sql = string.Format(@"Select ctsd.* , sp.KhachHang,sp.TenSP,case when DuyetChinhThuc is null then N'Chưa duyệt' else N'Đã duyệt' end as TinhTrang,case when DuyetChinhThuc is null then '{0}' else N'{1}' end as PathIcon  
                ,case when DuyetTruoc is null then N'Chưa kiểm' else N'Đã kiểm' end as CheckKiem,case when DuyetChinhThuc is null then N'Chưa duyệt' else N'Đã duyệt' end as CheckDuyet ,N'{3}' as ChucNang  
                from (select * from ChiTietSP_SuaDoi {2}) ctsd inner join SanPham sp on sp.MaSP = ctsd.MaSP order by sp.TenSP,ctsd.[Version]
                    
            ", IconImg.NotCheck, IconImg.CheckMark, dieukien, chucnang);
            CallAPI callAPI = new CallAPI();
           string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
            if (json != "")
            {

                var query = JsonConvert.DeserializeObject<List<DinhMuc_Show>>(json);
              
                //var query = cTEntities.Database.SqlQuery<DinhMuc_Show>(sql, lstpara.ToArray()).ToList();

                if (query.Count > 0)
                {

                    sql = string.Format(@"SELECT kd.Serial,kd.[MaSP],kd.[KhuVuc],kd.[TenQuyTrinh],kd.[UsersName],kd.[LoaiDuyet],kd.Ngay,kd.UserYeuCau,kd.NgayInsert
                                FROM [KyDuyet] kd
                                where KhuVuc=N'Ký duyệt định mức gỗ'");
                    json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
                    List<KyDuyet> lstkyduyet = new List<KyDuyet>();
                    if(json!="")
                    {
                        lstkyduyet = JsonConvert.DeserializeObject<List<KyDuyet>>(json);
                    }
                    
                    //List<KyDuyet> lstkyduyet = cTEntities.Database.SqlQuery<KyDuyet>(sql, "").ToList();
                    string sqluser = string.Format(@"SELECT [UsersName],'{0}'+isnull([PathImg],'UserImage/user.png') as PathImg,TenUser
                              FROM [dbo].[Users]
                              where [UsersName] in (select UsersName from [KyDuyet] group by UsersName)", Model.ModelAdmin.pathurlfilepublic + "/");

                    json = await callAPI.ExcuteQueryEncryptAsync(sqluser, new List<ParameterDefine>());
                    List<usertmp> lstuser = new List<usertmp>();
                    if (json!="")
                    {
                        lstuser = JsonConvert.DeserializeObject<List<usertmp>>(json);
                    }
                   

                    //List<usertmp> lstuser = cTEntities.Database.SqlQuery<usertmp>(sqluser, new List<ParameterDefine>()).ToList();
                    foreach (var it in query)
                    {
                        if (it.DuyetChinhThuc == null)
                        {
                            var queryuser = lstkyduyet.Where(p => p.MaSP.Equals(it.MaSP)).ToList();
                            if (queryuser.Count > 0)
                            {

                                foreach (var item in queryuser)
                                {
                                    if (item.LoaiDuyet == "Kiểm tra")
                                    {
                                        if (it.DuyetTruoc == null)
                                        {
                                            it.DuyetTruoc = item.UsersName;
                                        }
                                    }
                                    if (item.LoaiDuyet == "Duyệt")
                                    {
                                        it.DuyetChinhThuc = item.UsersName;
                                    }

                                }
                            }
                            queryuser.Clear();
                        }
                        foreach (var itusr in lstuser)
                        {
                            if (it.DuyetTruoc == itusr.UsersName)
                            {
                                it.PathImgKiemTra = itusr.PathImg;
                                it.NameKiem = itusr.TenUser;
                                if (it.PathImgDuyet != null)
                                    break;
                            }
                            if (it.DuyetChinhThuc == itusr.UsersName)
                            {
                                it.PathImgDuyet = itusr.PathImg;
                                it.NameDuyet = itusr.TenUser;
                                if (it.PathImgKiemTra != null)
                                    break;
                            }

                        }

                    }

                }

                lstDinhMucShow.AddRange(query);
              

            }
         
        }
     
        public async Task GotoMainFormAsync()
        {
            dxGrid.Reload();

        }
    }
}
