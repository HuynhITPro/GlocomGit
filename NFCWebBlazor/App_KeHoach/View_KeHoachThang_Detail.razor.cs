using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Collections.ObjectModel;
using static NFCWebBlazor.App_KeHoach.Page_KeHoachThang_Master;


namespace NFCWebBlazor.App_KeHoach
{
    public partial class View_KeHoachThang_Detail
    {

        private async Task loadAsync()
        {
            if(ShowView!=null)
            {
                showtyle = true;
            }
            if (keHoachSP_Showcrr.lstKeHoachChiTiet == null)
            {

                string sql = "";
                if (ShowView == "viewchuahoanthanh")
                {
                    sql = string.Format(@"use NVLDB
                      
                        select khtitem.Serial,sp.TenSP,khtitem.MaSP,art.Type_Other,khtitem.ArticleNumber,khtitem.SLSP,isnull(nvlsp.SoLuongSP,0) as SLThucHien,khtitem.SLSP-isnull(nvlsp.SoLuongSP,0) as SLConLai,case when khtitem.SLSP=0 then 0 else cast(isnull(nvlsp.SoLuongSP,0) as float)/khtitem.SLSP end as TyLe,mm.MaMau
                        ,mm.TenMau,mm.Color
                        from (select * from KeHoachThangItem where Serial_KHThang=@Serial) khtitem 
                        left join (select SerialKHThangItem,sum(SoLuongSP) as SoLuongSP from dbo.NvlKeHoachSP group by SerialKHThangItem) nvlsp  on khtitem.Serial=nvlsp.SerialKHThangItem
                       left join dbo.GetArticleNumber() art
                        on khtitem.ArticleNumber=art.ArticleNumber
                        left join dbo.GetMaMau() mm on art.MaMau=mm.MaMau
                        inner join dbo.GetSanPham() sp on khtitem.MaSP=sp.MaSP order by sp.TenSP", keHoachSP_Showcrr.Serial);


                }
                else
                {
                    sql = string.Format(@"
                                use NVLDB
                                SELECT  it.[Serial],[Serial_KHThang],art.Type_Other,it.[ArticleNumber] ,it.[MaSP],sp.TenSP,[SLSP],cast(SLSP as float) as SLConLai,mm.TenMau,[MaDHMua],it.[NgayInsert],it.GhiChu,mm.Color,it.UserInsert,mm.MaMau
								
                                    FROM (select * from [KeHoachThangItem] where Serial_KHThang=@Serial) 
									it inner join dbo.GetSanPham()  sp on it.MaSP=sp.MaSP 			
                                    left join dbo.GetArticleNumber() art on art.ArticleNumber=it.ArticleNumber
									left join dbo.GetMaMau() mm on art.MaMau=mm.MaMau", keHoachSP_Showcrr.Serial);
                }
                try
                {
                    PanelVisible = true;

                    CallAPI callAPI = new CallAPI();
                    List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                    parameterDefineList.Add(new ParameterDefine("@Serial", keHoachSP_Showcrr.Serial));
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<KeHoachThangItem_Show>>(json);
                        if (query.Count > 0)
                        {
                            lstdata = new List<KeHoachThangItem_Show>();
                            lstdata.AddRange(query);
                           
                            keHoachSP_Showcrr.lstKeHoachChiTiet = lstdata;
                            //Grid.ExpandGroupRow(0);
                            Grid.Reload();
                            PanelVisible = false;
                           
                            //Grid.AutoFitColumnWidths();
                        }
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
            else
            {
                lstdata = keHoachSP_Showcrr.lstKeHoachChiTiet;
                Grid.Reload();
                StateHasChanged();
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
              await  loadAsync();
            }
            //return base.OnAfterRenderAsync(firstRender);
        }

        private async Task deleteAsync(KeHoachThangItem_Show keHoachThangItem_Show)
        {
            if (!phanQuyenAccess.CheckDelete(keHoachThangItem_Show.UserInsert, ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không phải người tạo dòng này, nên không có quyền xóa"));
                return;
            }

            bool ketqua = await dialogMsg.Show($"XÓA  {keHoachThangItem_Show.Serial}???", $"Bạn có chắc muốn xóa  {keHoachThangItem_Show.TenSP}??");
            if (ketqua)
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.KeHoachThangItem_Delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();

                lstpara.Add(new ParameterDefine("@Serial", keHoachThangItem_Show.Serial));
                lstpara.Add(new ParameterDefine("@UserDelele", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@LyDoDelete", "Xóa"));
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
                                lstdata.Remove(keHoachThangItem_Show);
                                Grid.Reload();

                            }
                            else
                            {
                                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception)));
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
        private void EditItemAsync(KeHoachThangItem_Show keHoachThangItem_Show)
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có quyền sửa"));
                return;
            }
            if (!phanQuyenAccess.CheckDelete(keHoachThangItem_Show.UserInsert, ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không phải người tạo dòng này, nên không có quyền xóa"));
                return;
            }
            keHoachThangItemcrr = keHoachThangItem_Show;
             //keHoachSP_Show.LoaiKeHoach = LoaiKeHoach;
             renderFragment = builder =>
            {
                builder.OpenComponent<Urc_KeHoachThang_ItemAdd>(0);
                builder.AddAttribute(1, "keHoachSP_Showcrr", keHoachSP_Showcrr);
                builder.AddAttribute(2, "keHoachThangItem_Showform", keHoachThangItem_Show.CopyClass());
                builder.AddAttribute(3, "GotoDetailItem", EventCallback.Factory.Create<KeHoachThangItem_Show>(this, RefreshItem));
                builder.CloseComponent();
            };
            dxPopup.showAsync("THÊM KẾ HOẠCH THÁNG");

            dxPopup.ShowAsync();
        }
        private async void RefreshItem(KeHoachThangItem_Show keHoachThangItem_get)
        {
            setClass(keHoachThangItemcrr, keHoachThangItem_get);
           await Grid.SaveChangesAsync();
           await dxPopup.CloseAsync();
        }
        private void setClass(KeHoachThangItem_Show keHoachThangItem_set, KeHoachThangItem_Show keHoachThangItem_get)
        {
            //keHoachThangItem_set.Serial = keHoachThangItem_get.Serial;
            keHoachThangItem_set.ArticleNumber = keHoachThangItem_get.ArticleNumber;
            keHoachThangItem_set.MaSP = keHoachThangItem_get.MaSP;
            keHoachThangItem_set.SLSP = keHoachThangItem_get.SLSP;
            keHoachThangItem_set.MaDHMua = keHoachThangItem_get.MaDHMua;
            keHoachThangItem_set.UserInsert = keHoachThangItem_get.UserInsert;
            keHoachThangItem_set.GhiChu = keHoachThangItem_get.GhiChu;

        }
    }
}
