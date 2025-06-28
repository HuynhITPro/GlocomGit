using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Collections.ObjectModel;

using static NFCWebBlazor.App_ThongTin.Page_DinhMucHangHoaMaster;


namespace NFCWebBlazor.App_ThongTin
{
    public partial class Detail_DinhMucHangHoaRow
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        HangHoaItem? hanghoaitemcrr { get; set; }
        bool CheckQuyen = false;
        public async Task EditItemAsync()
        {
          await  dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền sửa"));
                return;
            }
            SanPhamShow sanPhamShow = new SanPhamShow();
            sanPhamShow.MaSP = sanPhamShowcrr.MaSP;
            sanPhamShow.TenSP = sanPhamShowcrr.TenSP;
            sanPhamShow.ArticleNumber = hanghoaitemcrr.ArticleNumber;
            sanPhamShow.TenMau = hanghoaitemcrr.TenMau;
            sanPhamShow.Color = hanghoaitemcrr.Color;
            List<SanPhamShow> lstsp = new List<SanPhamShow>();
            lstsp.Add(sanPhamShow);
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DinhMucHangHoaAdditem>(0);
                builder.AddAttribute(1, "lstdata", lstsp);
                builder.AddAttribute(2, "sanPhamShowcrr", sanPhamShow);
                builder.AddAttribute(3, "hangHoaItemcrr", hanghoaitemcrr.CopyClass());
                builder.AddAttribute(4, "GotoMainForm", EventCallback.Factory.Create<HangHoaItem>(this, GotoMainForm));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
          await  dxPopup.showAsync("THÊM ĐỊNH MỨC SẢN PHẨM");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
           await dxPopup.ShowAsync();

        }
       
        public async Task ImportExcelAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền import"));
                return;
            }

          
        }
       
        public void Setclass(HangHoaItem hangHoaItem_set, HangHoaItem hangHoaItem_get)
        {
            //hangHoaItem_set.Serial = hangHoaItem_get.Serial;
            hangHoaItem_set.MaSP = hangHoaItem_get.MaSP;
            hangHoaItem_set.SLQuyDoi = hangHoaItem_get.SLQuyDoi;
            hangHoaItem_set.MaHang = hangHoaItem_get.MaHang;
            hangHoaItem_set.KhuVuc = hangHoaItem_get.KhuVuc;
            hangHoaItem_set.ArticleNumber = hangHoaItem_get.ArticleNumber;
            hangHoaItem_set.GhiChu = hangHoaItem_get.GhiChu;
            hangHoaItem_set.UserInsert = hangHoaItem_get.UserInsert;
            hangHoaItem_set.ChatLuong = hangHoaItem_get.ChatLuong;
            hangHoaItem_set.DinhMucHaoHut = hangHoaItem_get.DinhMucHaoHut;
        }
        public async void searchAsync()
        {
            string sql = string.Format(@" use NVLDB
                        select qry.Serial,qry.MaSP,qry.MaHang,qry.SLQuyDoi,qry.ChatLuong,qry.KhuVuc,qry.DinhMucHaoHut,qry.NgayInsert,qry.GhiChu,qry.UserInsert,qry.ArticleNumber,hh.TenHang, mm.Color,isnull(mm.TenMau,N'') as TenMau,nh.PhanLoai
                        ,case when qrylog.UserInsert is not null then qrylog.UserInsert+N' đã sửa lúc '+FORMAT(qrylog.NgayUpdate, 'dd/MM/yy HH:mm', 'en-US' ) else '' end as NguoiSua  from 
                        (select * from NvlChiTietKhuVuc where MaSP=@MaSP
                        ) as qry
                        inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                        left join
						(select Serial,UserInsert,NgayUpdate from NvlChiTietKhuVuc_Log log_
						where Serial_Log in
						(select MAX(Serial_Log) from NvlChiTietKhuVuc_Log
						where Serial in (select Serial from NvlChiTietKhuVuc where MaSP=@MaSP)
						group by Serial)) as qrylog on qry.Serial=qrylog.Serial
                        left join [SP].[DataBase_ScansiaPacific2014].dbo.ArticleNumberProduct art
                        on qry.ArticleNumber=art.ArticleNumber
                        left join [SP].[DataBase_ScansiaPacific2014].dbo.MaMau mm on art.MaMau=mm.MaMau left join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom order by mm.TenMau,hh.MaHang ");
            
            string dieukien = "";
            PanelVisible = true;
          
            sql = sql + dieukien;
            try
            {

                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                parameterDefineList.Add(new ParameterDefine("@MaSP", sanPhamShowcrr.MaSP));
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<HangHoaItem>>(json);
                    if (query.Count > 0)
                    {
                        lstdata = new List<HangHoaItem>(query);
                        //sanPhamShowcrr.lsthanghoaitem = new ObservableCollection<HangHoaItem>();
                        query.Clear();
                       sanPhamShowcrr.lsthanghoaitem=lstdata;
                        //Grid.ExpandGroupRow(0);
                        Grid.Reload();
                        PanelVisible = false;
                        
                        //Grid.AutoFitColumnWidths();
                    }
                }


            }
            catch (Exception ex)
            {
                PanelVisible = false;
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));

            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }



        }
        private void GotoMainForm(HangHoaItem hangHoaItem)
        {
            Setclass(hanghoaitemcrr, hangHoaItem);
            Grid.SaveChangesAsync();
        }

        private async void AddItem(HangHoaItem hangHoaItem)
        {
          await  dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền thêm"));
                return;
            }
            SanPhamShow sanPhamShow = new SanPhamShow();
            sanPhamShow.MaSP = sanPhamShowcrr.MaSP;
            sanPhamShow.TenSP = sanPhamShowcrr.TenSP;
            sanPhamShow.ArticleNumber = hangHoaItem.ArticleNumber;
            sanPhamShow.TenMau = hangHoaItem.TenMau;
            sanPhamShow.Color = hangHoaItem.Color;
            List<SanPhamShow>lstsp=new List<SanPhamShow>();
            lstsp.Add(sanPhamShow);
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DinhMucHangHoaAdditem>(0);
                builder.AddAttribute(1, "lstdata",lstsp);
                builder.AddAttribute(2, "sanPhamShowcrr", sanPhamShow);
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
          await  dxPopup.showAsync("THÊM ĐỊNH MỨC SẢN PHẨM");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
          await  dxPopup.ShowAsync();
        }
        private async Task deleteAsync()
        {
            await dxFlyoutchucnang.CloseAsync();

            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa"));
                return;
            }

            bool ketqua = await dialogMsg.Show($"XÓA  {hanghoaitemcrr.TenHang}???", $"Bạn có chắc muốn xóa  {hanghoaitemcrr.TenHang}??");
            if (ketqua)
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlChiTietKhuVuc_Delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", hanghoaitemcrr.Serial));
                lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));
                try
                {
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);

                    if (json != "")
                    {
                        try
                        {
                            var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                            if (query[0].ketqua == "OK")
                            {
                                toastService.Notify(new ToastMessage(ToastType.Success, $"Xóa thành công"));
                                lstdata.Remove(hanghoaitemcrr);
                               //Grid.SaveChangesAsync();

                            }
                            else
                            {
                                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi {0},{1}: ", query[0].ketqua, query[0].ketquaexception)));
                            }
                        }
                        catch (Exception ex)
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                }
            }
        }
        public async Task DongBoDongViAsync()
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền đồng bộ"));
                return;
            }

            string sql = "NVLDB.dbo.DongBoFittingDongViNVL";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            CallAPI callAPI = new CallAPI();
            lstpara.Add(new ParameterDefine("@MaSP", hanghoaitemcrr.MaSP));
            lstpara.Add(new ParameterDefine("@SerialLink", hanghoaitemcrr.Serial));
         
            string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                if (query[0].ketqua == "OK")
                {
                    toastService.Notify(new ToastMessage(ToastType.Success, "Đồng bộ thành công"));
                  

                }
                else
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));

                    //reset();
                }

                //Grid.Data = lstDonDatHangSearchShow;
            }

        }
        public async void ShowFlyout(HangHoaItem sanPhamShow)
        {
            await dxFlyoutchucnang.CloseAsync();
            //CurrentEmployee = employee;
            hanghoaitemcrr = sanPhamShow;
            idflychucnang = "#" + idelement(hanghoaitemcrr.Serial);
           // IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();
        }
        private async void AddItem2()
        {
           
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền thêm"));
                return;
            }
            await  dxFlyoutchucnang.CloseAsync();
            HangHoaItem hangHoaItem = new HangHoaItem();
            
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DinhMucHangHoa_AddItem_2>(0);
               
                builder.AddAttribute(2, "hangHoaItemmaster", hanghoaitemcrr.CopyClass());
                builder.AddAttribute(3, "hangHoaItemcrr", hangHoaItem);
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
           await dxPopup.showAsync("THÊM ĐỊNH MỨC TRONG NHÓM");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
           await dxPopup.ShowAsync();
        }
        private async void ImportItem()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền thêm"));
                return;
            }
            await dxFlyoutchucnang.CloseAsync();
            HangHoaItem hangHoaItem = new HangHoaItem();

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DinhMucHangHoa_ImportExcel_2>(0);

            
                builder.AddAttribute(3, "hangHoaItemmaster", hanghoaitemcrr.CopyClass());
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
          await  dxPopup.showAsync("THÊM ĐỊNH MỨC TRONG NHÓM");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
           await dxPopup.ShowAsync();
        }
    }

}
