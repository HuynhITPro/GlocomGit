using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Urc_NvlNhapXuat_BKNhanh
    {
        [Inject] ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess {  get; set; }
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        protected override async Task OnInitializedAsync()
        {
            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 70;
            heightgrid = string.Format("{0}px", height);

            //// var heighrow = await browserService.GetHeighWithID("divcontainer");
            //int height = dimension.Height - 120;

            int width = dimension.Width;
            if (width < 768)
            {
                Ismobile = true;
                idgrid = "customGridnotheader";

            }
            else
            {
                idgrid = "griddetailnhapkho";
                Ismobile = false;
            }
           nvlNhapXuatItemShowcrr.UserInsert= ModelAdmin.users.UsersName;

            await loadAsync();
            //randomdivhide = prs.RandomString(10);
            //return base.OnInitializedAsync();
        }
        private async Task loadAsync()
        {
          
            lstuser = await Model.ModelData.Getlstusers();
            lstnoigiaonhan = await Model.ModelData.Getlstnoigiaonhan();
           
        }
        bool checkfirst=false;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
           
           
            return base.OnAfterRenderAsync(firstRender);
        }
        private bool checknhap()
        {
            if (LoaiNhapXuat.Contains("Nhap"))
                return true;
            return false;

        }
        public async Task searchAsync()
        {
            if (lstdata == null)
                lstdata = new List<NvlNhapXuatItemShow>();
            lstdata.Clear();

            if (dtpbegin == null || dtpend == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Chọn ngày"));

                return;
            }

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = "";
            string dieukienct = "";
            if (nvlNhapXuatItemShowcrr.SerialLink != null || nvlNhapXuatItemShowcrr.SerialCT != null)
            {
                if(nvlNhapXuatItemShowcrr.SerialCT!=null)
                {
                    dieukienct += " and Serial=@SerialCT";
                    lstpara.Add(new ParameterDefine("@SerialCT", nvlNhapXuatItemShowcrr.SerialCT));
                }
                if(nvlNhapXuatItemShowcrr.SerialLink!=null)
                {
                    dieukien += " and SerialLink=@SerialLink";
                    lstpara.Add(new ParameterDefine("@SerialLink", nvlNhapXuatItemShowcrr.SerialLink));
                }
               
            }
            else
            {
                dieukien += " and NgayInsert>=@DateBegin and NgayInsert<=@DateEnd";
                lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
                lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.UserInsert))
                {
                    dieukien += " and UserInsert=@UserInsert";
                    lstpara.Add(new ParameterDefine("@UserInsert", nvlNhapXuatItemShowcrr.UserInsert));
                }
               
                if (checknhap())
                {
                    dieukienct += " and STTCT>0";

                }
                else
                {
                    dieukienct += " and STTCT<=0";
                }
                if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.TenGN))
                {
                    
                  
                    
                    dieukienct += " and MaGN = @MaGN";
                    lstpara.Add(new ParameterDefine("@MaGN", nvlNhapXuatItemShowcrr.TenGN));
                }
            }
            
            dieukien=StaticClass.Substringdieukien(dieukien.Trim());
            dieukienct =StaticClass.Substringdieukien(dieukienct.Trim());

            string sqlSearch = "";
                //string[] arrshow = new string[] { "MaHang", "TenHang", "SerialCT", "SLNhap", "SLXuat", "DVT", "LyDo", "TenSP", "MaKien", "DauTuan", "SoLo", "TenGN", "SoXe", "GhiChu", "DonGia", "NgayHetHan", "SerialLink", "KhachHang_XuatXu", "UserInsert", "SerialKHDH", "SerialDN", "NguoiDeNghi", "TenLienKet", "NhaMay", "NgayInsert" };
                sqlSearch = string.Format(@"use NVLDB select nxitem.Serial,nxitem.SerialLink,nxitem.SerialCT,nxitem.MaHang,nxitem.SLNhap,nx.LyDo,nxitem.SLXuat,nxitem.DonGia,(nxitem.SLNhap+nxitem.SLXuat)*nxitem.DonGia as ThanhTien
                    ,nxitem.DauTuan,nxitem.ViTri,nxitem.GhiChu,nx.NhaMay,nxitem.NgayInsert,nxitem.UserInsert,nxitem.MaKien,nxitem.SoLo,gn.TenGN,hh.TenHang,hh.DVT
                    ,nx.LyDo
                    from (SELECT  [Serial] ,[MaKho]
                          ,[MaGN],[LyDo],[PONumber],[Ngay],[NguoiDN],[NhaMay]
                      FROM [NvlNhapXuat] {0}) nx
                    inner join
                    (select * from NvlNhapXuatItem {1}) nxitem 
                    on nx.Serial=nxitem.SerialCT
                    inner JOIN View_NoiGN gn on gn.MaGN = nx.MaGN
                    inner join dbo.NvlHangHoa hh on nxitem.MaHang=hh.MaHang order by nxitem.Serial desc					
                   
                     ", dieukienct,dieukien);
           
            CallAPI callAPI = new CallAPI();
            try
            {
                PanelVisible = true;
                string json = await callAPI.ExcuteQueryEncryptAsync(sqlSearch, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemShow>>(json);
                    lstdata = query;
                    if (query.Any())
                    { await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide)); }

                    // await GotoMainForm.InvokeAsync();
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

        string lydoxoa = "";
        private async void lydoxoaCallBack(string lydo)
        {
            lydoxoa = lydo;
           
            if (!string.IsNullOrEmpty(lydoxoa))
            {
              await dxPopup.CloseAsync();
                string sql = "NVLDB.dbo.NvlNhapXuatItem_Delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlNhapXuatItemShowcrrdelete.Serial));
                lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@LyDoDelete", lydoxoa));
                try
                {
                    CallAPI callAPI = new CallAPI();
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            toastService.Notify(new ToastMessage(ToastType.Success, "Xóa thành công"));
                            //Kiểm tra chứng từ chuyển
                            int? flag= nvlNhapXuatItemShowcrrdelete.flag;
                            if (flag > 0)
                            {
                                lstdata.RemoveAll(it => it.flag == flag);
                            }
                            else
                            {
                                // msgBox.Show("Xóa thành công", IconMsg.iconssuccess);
                                var querydelete = lstdata.FirstOrDefault(p => p.Serial == nvlNhapXuatItemShowcrrdelete.Serial);
                                if (querydelete != null)
                                {
                                    lstdata.Remove(querydelete);
                                }
                            }
                            await dxGrid.SaveChangesAsync();

                            dxGrid.Reload();
                        }
                        else
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, $"{query[0].ketqua}, {query[0].ketquaexception}"));
                            //msgBox.Show("Lỗi: " + query[0].ketqua, IconMsg.iconssuccess);

                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: " + ex.Message));
                }

            }
        }
        NvlNhapXuatItemShow nvlNhapXuatItemShowcrrdelete;
        private async void deleteitem(NvlNhapXuatItemShow nvlNhapXuatItemShow)
        {
            nvlNhapXuatItemShowcrrdelete = nvlNhapXuatItemShow;
            if (!phanQuyenAccess.CheckDelete(nvlNhapXuatItemShow.UserInsert, ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không có quyền xóa"));
                return;
            }
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NhapLyDo>(0);
                builder.AddAttribute(1, "GetLyDo", EventCallback.Factory.Create<string>(this, lydoxoaCallBack));
                builder.CloseComponent();
            };
           
            await dxPopup.showAsync("Nhập lý do tại sao XÓA?");
            await dxPopup.ShowAsync();  
            


        }
    }

}
