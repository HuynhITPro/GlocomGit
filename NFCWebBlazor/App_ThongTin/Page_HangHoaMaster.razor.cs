using BlazorBootstrap;

using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;

using System.ComponentModel.DataAnnotations;
using System.Data;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Page_HangHoaMaster
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }
        NvlHangHoaShow? nvlHangHoaShowcrr { get; set; }
        bool CheckQuyen = false;
        private async Task loaddatadropdownAsync()
        {
            try
            {
                 lstHangHoaDropDown = await ModelData.GetHangHoa();
                if (lstmanhom == null)
                {
                   
                    //lstmanhom = new List<DataDropDownList>();
                    // lstmanhom =await ModelData.GetlstNhomhang();
                    CallAPI callAPI = new CallAPI();
                    string sql = "use NVLDB SELECT [MaNhom] as [Name],[TenNhom] as FullName,N'NhomHang' as TypeName FROM [NvlNhomHang]";
                    List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                    if (json != "")
                    {

                        lstmanhom = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);

                        //selectNhom = lstmanhom.Where(p => p.Name == nvlHangHoaShowcrr.MaNhom).FirstOrDefault();
                       // Console.WriteLine("Mã nhóm : {0},{1}", nvlHangHoaShowcrr.MaNhom, lstmanhom.Count);

                        StateHasChanged();
                    }
                }
         
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
            }
            finally
            {

            }


        }
        public class NvlHangHoaShow
        {
            public int Serial { get; set; }
            
           
            [Required(ErrorMessage = "Mã hàng không được để trống")]
            [StringLength(100, ErrorMessage = "Mã hàng từ 1 đến 100 ký tự")]
            public string MaHang { get; set; }

            [Required(ErrorMessage = "Tên hàng  không được để trống")]
            [StringLength(100, ErrorMessage = "Mã hàng từ 1 đến 100 ký tự")]
            public string TenHang { get; set; }

            public string QuyCach { get; set; }

            [Required(ErrorMessage = "Chọn nhóm")]
            [StringLength(100, ErrorMessage = "Mã nhóm từ 1 đến 100 ký tự")]
            public string MaNhom { get; set; }

            [Required(ErrorMessage = "Chọn ĐVT")]
            [StringLength(100, ErrorMessage = "ĐVT từ 1 đến 100 ký tự")]
            public string DVT { get; set; }

            public string ChatLuong { get; set; }
            public string TenNhom { get; set; }

            public string PhanLoaiDM { get; set; }

            public double? MinTK { get; set; }

            public double? MaxTK { get; set; }

            public string UserInsert { get; set; }
            public string DVT2 { get; set; }
            public decimal? TyLeQD { get; set; }
            public int? flag { get; set; }

            public string GhiChu { get; set; }

            public string MaPDOC { get; set; }

            public string MaKhac { get; set; }

            public string MaSPGroup { get; set; }

            public string TenSPPDOC { get; set; }

            public string MaMau { get; set; }

            public double? TyLeHaoHut { get; set; }

            public string KeySearch { get; set; }
            public string XuatXu { get; set; }

            public string PathImg { get; set; }

            public double? SLCont { get; set; }

            public DateTime? NgayInsert { get; set; }
            public string? Err { get; set; }

            public NvlHangHoaShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlHangHoaShow>(json);
            }
            public List<FileHoSoGroup> lstfilehoso
            {
                get;
                set;

            }
            public void setlstfilehoso(List<FileHoSoGroup> lst)
            {
                lstfilehoso = lst;
            }

        }
        
        public async Task AddItemAsync()
        {
          await  dxFlyoutchucnang.CloseAsync();
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền thêm"));
                return;
            }
            NvlHangHoaShow khachHangNVLShow = new NvlHangHoaShow();
            khachHangNVLShow.MaHang = "";
            khachHangNVLShow.TenHang = "";


            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_HangHoaAddItem>(0);
                builder.AddAttribute(1, "nvlHangHoaShowcrr", khachHangNVLShow);
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
           await dxPopup.showAsync("THÊM MỚI HÀNG HÓA");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
           await dxPopup.ShowAsync();
        }
        public async Task EditItemAsync()
        {

          await  dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền sửa"));
                return;
            }

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_HangHoaAddItem>(0);
                builder.AddAttribute(1, "nvlHangHoaShowcrr", nvlHangHoaShowcrr.CopyClass());
                builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<NvlHangHoaShow>(this, GotoMainForm));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
          await  dxPopup.showAsync("SỬA THÔNG TIN");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
          await  dxPopup.ShowAsync();
        }
        public async Task ImportExcelAsync()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền import"));
                return;
            }

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_HangHoaAddTable>(0);
               
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
          await  dxPopup.showAsync("Import hàng hóa từ excel");
            // dxPopup.ChildContent = renderFragment;

            //dxPopup.ChildContent= renderFragment;
            //StateHasChanged();
          await  dxPopup.ShowAsync();
        }
        public void Setclass(NvlHangHoaShow NvlHangHoa_set, NvlHangHoaShow NvlHangHoa_get)
        {
            NvlHangHoa_set.Serial = NvlHangHoa_get.Serial;
            NvlHangHoa_set.MaHang = NvlHangHoa_get.MaHang;
            NvlHangHoa_set.TenHang = NvlHangHoa_get.TenHang;
            NvlHangHoa_set.QuyCach = NvlHangHoa_get.QuyCach;
            NvlHangHoa_set.MaNhom = NvlHangHoa_get.MaNhom;
            NvlHangHoa_set.TenNhom = NvlHangHoa_get.TenNhom;
            NvlHangHoa_set.DVT = NvlHangHoa_get.DVT;
            NvlHangHoa_set.ChatLuong = NvlHangHoa_get.ChatLuong;
            NvlHangHoa_set.MinTK = NvlHangHoa_get.MinTK;
            NvlHangHoa_set.SLCont = NvlHangHoa_get.SLCont;
            NvlHangHoa_set.MaxTK = NvlHangHoa_get.MaxTK;
            NvlHangHoa_set.UserInsert = NvlHangHoa_get.UserInsert;
            NvlHangHoa_set.flag = NvlHangHoa_get.flag;
            NvlHangHoa_set.GhiChu = NvlHangHoa_get.GhiChu;
            NvlHangHoa_set.MaPDOC = NvlHangHoa_get.MaPDOC;
            NvlHangHoa_set.MaKhac = NvlHangHoa_get.MaKhac;
            NvlHangHoa_set.MaSPGroup = NvlHangHoa_get.MaSPGroup;
            NvlHangHoa_set.MaMau = NvlHangHoa_get.MaMau;
            NvlHangHoa_set.TyLeHaoHut = NvlHangHoa_get.TyLeHaoHut;
            NvlHangHoa_set.KeySearch = NvlHangHoa_get.KeySearch;
            NvlHangHoa_set.PathImg = NvlHangHoa_get.PathImg;
            NvlHangHoa_set.DVT2 = NvlHangHoa_get.DVT2;

            NvlHangHoa_set.TyLeQD = NvlHangHoa_get.TyLeQD;
        }
        public async void searchAsync()
        {
            string sql = string.Format(@"Use [NVLDB] Select hh.*,nhom.TenNhom from NvlHangHoa as hh inner join dbo.NvlNhomHang nhom on hh.MaNhom=nhom.MaNhom ");
            string dieukien = "";
            lstdata.Clear();
            if (!string.IsNullOrEmpty(MaHangSelected))
            {
                if (dieukien == "")
                {
                    dieukien += string.Format(" where hh.MaHang=N'{0}'", MaHangSelected);
                }
                else
                {
                    dieukien += string.Format(" and hh.MaHang=N'{0}'", MaHangSelected);
                }
            }
            if(selectNhom!=null)
            {
                if (dieukien == "")
                {
                    dieukien += string.Format(" where nhom.MaNhom=N'{0}'", selectNhom.Name);
                }
                else
                {
                    dieukien += string.Format(" and nhom.MaNhom=N'{0}'", selectNhom.Name);
                }
            }
            if(dateTime!=null)
            {
                if (dieukien == "")
                {
                    dieukien += string.Format(" where hh.NgayInsert>='{0}'", dateTime.Value.ToString("MM/dd/yyyy"));
                }
                else
                {
                    dieukien += string.Format(" and hh.NgayInsert>='{0}'", dateTime.Value.ToString("MM/dd/yyyy"));
                }
            }
            sql = sql + dieukien;
            PanelVisible = true;
            try
            {

                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {
                    lstdata = JsonConvert.DeserializeObject<List<NvlHangHoaShow>>(json);
                    PanelVisible = false;
                    Grid.Reload();
                }


            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));

            }
            finally
            {
                PanelVisible = false;
                StateHasChanged();
            }



        }
        private void GotoMainForm(NvlHangHoaShow nvlHangHoaShow)
        {
            Setclass(nvlHangHoaShowcrr, nvlHangHoaShow);
            Grid.SaveChangesAsync();
        }

       
        private async Task deleteAsync()
        {
            await dxFlyoutchucnang.CloseAsync();

            if (!await phanQuyenAccess.CreateMaHang(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền xóa"));
                return;
            }

            bool ketqua = await dialogMsg.Show($"XÓA  {nvlHangHoaShowcrr.TenHang}???", $"Bạn có chắc muốn xóa  {nvlHangHoaShowcrr.TenHang}??");
            if (ketqua)
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlHangHoa_Delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlHangHoaShowcrr.Serial));
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
                                lstdata.Remove(nvlHangHoaShowcrr);
                                Grid.SaveChangesAsync();

                                StateHasChanged();

                            }
                            else
                            {
                                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi {0},{1}: ",query[0].ketqua,  query[0].ketquaexception)));
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
        public async void ShowFlyout(NvlHangHoaShow nvlKhachHang)
        {
            await dxFlyoutchucnang.CloseAsync();
            //CurrentEmployee = employee;
            nvlHangHoaShowcrr = nvlKhachHang;
            idflychucnang = "#" + idelement(nvlHangHoaShowcrr.Serial);
           // IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();
        }

    }

}
