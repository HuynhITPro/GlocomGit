using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucHangHoaMaster;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Detail_DinhMucHangHoaRowItem
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        
        bool CheckQuyen = false;
        public async Task EditItemAsync()
        {
           await dxFlyoutchucnang.CloseAsync();
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền sửa"));
                return;
            }
            HangHoaItem hangHoaItem = new HangHoaItem();

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DinhMucHangHoa_AddItem_2>(0);

                builder.AddAttribute(2, "hangHoaItemmaster", hanghoaitemcrrmaster.CopyClass());
                builder.AddAttribute(3, "hangHoaItemcrr", hanghoaitemcrrlocal.CopyClass());
                builder.AddAttribute(4, "GotoMainForm", EventCallback.Factory.Create<HangHoaItem>(this, GotoMainForm));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
           await dxPopup.showAsync("SỬA ĐỊNH MỨC TRONG NHÓM");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
            await dxPopup.ShowAsync();
        }
        private async void GotoMainForm(HangHoaItem hangHoaItem)
        {
            await dxPopup.CloseAsync();
            Setclass(hanghoaitemcrrlocal, hangHoaItem);
            await Grid.SaveChangesAsync();
            Grid.Reload();

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
                       

                        select qry.Serial,qry.MaHang,qry.SLQuyDoi,qry.ChatLuong,qry.DinhMucHaoHut,qry.NgayInsert,qry.GhiChu,qry.UserInsert,hh.TenHang,nh.PhanLoai
                        from 
                        (select * from NvlChiTietKhuVucItem where SerialLink=@SerialLink
                        ) as qry
                        inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                         left join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom 
						 order by hh.MaHang ");

            string dieukien = "";
            PanelVisible = true;

            sql = sql + dieukien;
            try
            {

                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                parameterDefineList.Add(new ParameterDefine("@SerialLink", hanghoaitemcrrmaster.Serial));
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<HangHoaItem>>(json);
                    if (query.Count > 0)
                    {
                        lstdata = new List<HangHoaItem>(query);
                        //sanPhamShowcrr.lsthanghoaitem = new ObservableCollection<HangHoaItem>();
                        query.Clear();
                       
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
     
        private async Task deleteAsync()
        {
            await dxFlyoutchucnang.CloseAsync();

            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa"));
                return;
            }

            bool ketqua = await dialogMsg.Show($"XÓA  {hanghoaitemcrrlocal.TenHang}???", $"Bạn có chắc muốn xóa  {hanghoaitemcrrlocal.TenHang}??");
            if (ketqua)
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlChiTietKhuVucItem_Delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", hanghoaitemcrrlocal.Serial));
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
                                lstdata.Remove(hanghoaitemcrrlocal);
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
        public async void ShowFlyout(HangHoaItem sanPhamShow)
        {
            await dxFlyoutchucnang.CloseAsync();
            //CurrentEmployee = employee;
            hanghoaitemcrrlocal = sanPhamShow;
            idflychucnang = "#" + idelement(hanghoaitemcrrlocal.Serial);
            //IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();
        }
    
      
    }

}
