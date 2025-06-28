using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using static NFCWebBlazor.App_KeHoach.Page_KeHoachThang_Master;


namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Page_KeHoachThang_ChuaHoanThanh
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        bool CheckQuyen = false;
        private string LoaiKeHoach { get; set; }
        private async Task loaddatadropdownAsync()
        {
            try
            {
                List<Users> lstusr = await Model.ModelData.Getlstusers();
                lstnguoidenghi = lstusr.ToList();

                lsttinhtrang = await Model.ModelData.GetDataDropDownListsAsync("TypeTinhTrangHoanThanh");
                MenuItem query = ModelAdmin.lstmenuitems.FirstOrDefault(p => p.NameItem.Equals(ModelAdmin.pageclickcurrent));
                if (query != null)
                {
                    if (query.Tag != null)
                        LoaiKeHoach = query.Tag.ToString();
                }

            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
            }
            finally
            {
                PanelVisible = false;
            }


        }

        int SerialDN = 0;
        public async void searchAsync()
        {
            string sql = "";


            string dieukien = "";
            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();

            if (LoaiKeHoach != null)
            {
                if (dieukien == "")
                    dieukien = " where LoaiKeHoach=@LoaiKeHoach";
                else
                    dieukien += " and LoaiKeHoach=@LoaiKeHoach";
                parameterDefineList.Add(new ParameterDefine("@LoaiKeHoach", LoaiKeHoach));
            }
            if (nguoidenghiselected != null)
            {
                if (dieukien == "")
                    dieukien = " where khthang.UserInsert=@UserInsert";
                else
                    dieukien += " and khthang.UserInsert=@UserInsert";
                parameterDefineList.Add(new ParameterDefine("@UserInsert", nguoidenghiselected.UsersName));
            }


            if (lstdata == null)
                lstdata = new List<KeHoachSP_Show>();
            else
                lstdata.Clear();

            if (typeappselected == "Đã hoàn thành")
            {

                dieukien += " and SLSP-SLSuDung<=0";// " where SLTheoDoi>0.01";
            }
            else
                dieukien += " and SLSP-SLSuDung>0";// " where SLTheoDoi>0.01";

            sql = string.Format(@"
                        use [NVLDB]
                        SELECT  [Serial],[TenKHThang],[UserInsert],ThangMin,ThangMax,[GhiChu],[NgayInsert],'{1}'+isnull(usr.[PathImg],'UserImage/user.png') as PathImgTao ,case when SLSP=0 then 0 else cast(SLSuDung as float)/SLSP end as TyLe
                          FROM [dbo].[KeHoachThang] khthang
                          left join (select [Serial_KHThang],sum(SLSP) as SLSP,sum(SLSuDung) as SLSuDung
                        from
                        (SELECT [Serial_KHThang],khitem.SLSP,isnull(khsp.SoLuongSP,0) as SLSuDung
                         FROM KeHoachThangItem khitem
                         left join (select SerialKHThangItem,sum(SoLuongSP) as SoLuongSP from NvlKeHoachSP group by  SerialKHThangItem)  khsp
                         on khsp.SerialKHThangItem=khitem.Serial) as qry
                         group by [Serial_KHThang]) as qrysoluong on khthang.Serial=qrysoluong.Serial_KHThang
                         left join DBMaster.dbo.Users usr on khthang.UserInsert=usr.UsersName {0}

                   ", dieukien, Model.ModelAdmin.pathurlfilepublic);
            if (SerialDN != 0)
            {

            }

            PanelVisible = true;
            try
            {

                CallAPI callAPI = new CallAPI();

                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<KeHoachSP_Show>>(json);
                    if (query.Count > 0)
                    {
                        lstdata.AddRange(query);
                        // Grid.AutoFitColumnWidths();
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
                Grid.Reload();
                StateHasChanged();
            }
        }


        private async Task GetListItemAsync()
        {
            string dieukien = "";
            string dieukienserial = "";
            var lst = lstdata.Where(p => p.isChanged == true).ToList();
            if (lst.Count == 0)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn ít nhất 1 đề nghị"));
                return;
            }

            foreach (var it in lst)
            {
                if (dieukienserial == "")
                {
                    dieukienserial = string.Format("{0}", it.Serial);
                }
                else
                    dieukienserial += string.Format(",{0}", it.Serial);
            }
            dieukien = string.Format("where SerialDN in ({0}) and SLTheoDoi>=0.01", dieukienserial);


            string sql = string.Format(@"use NVLDB

                        Select khmuahang.[Serial],khmuahang.[SerialDN],khmuahang.[MaHang],hh.TenHang,hh.DVT,nh.PhanLoai,isnull(tblsp.TenSP,N'') as TenSP
						,khmuahang.[SoLuong],IsNULL(ddhitem.SLDatHang,0) as SLTheoDoi,cast(0 as float) as DonGia,khmuahang.[SoLuong]-isnull(khmuahang.SLHuy,0)-isnull(ddhitem.SLDatHang,0) as SLDatHang,khmuahang.[SoLuong]-isnull(khmuahang.SLHuy,0)-isnull(ddhitem.SLDatHang,0) as SLConLai,khmuahang.[SerialLink],isnull(khmuahang.DonGia,0) as DonGiaDeNghi,isnull(dg.DonGia,0) as DonGiaGanNhat
	                        FROM (select * from NvlKeHoachMuaHangItem {0} 
                            and Serial in (
							SELECT [SerialLinkItem] FROM [NvlKyDuyetItem] where TableName=N'NvlKehoachMuaHang' and LoaiDuyet=N'Duyệt' and SerialLinkMaster in ({1}))
                            ) khmuahang
							 Left join (SELECT sum([SLDatHang]) as SLDatHang,SerialLink as SerialKHLink
							FROM [NvlDonDatHangItem]
							group by SerialLink) ddhitem ON ddhitem.SerialKHLink = khmuahang.Serial
							inner join dbo.NvlHangHoa hh on khmuahang.MaHang=hh.MaHang
                             inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom  left join dbo.MaHangSanPham()  tblsp on khmuahang.MaHang=tblsp.MaHang
                            left join dbo.DonGiaGanNhat() dg on khmuahang.MaHang=dg.MaHang                    
                            where khmuahang.[SoLuong]-isnull(khmuahang.SLHuy,0)-isnull(ddhitem.SLDatHang,0)>0.01", dieukien, dieukienserial);
            CallAPI callAPI = new CallAPI();
            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<KeHoachThangItem_Show>>(json);

            }

        }


    }
}