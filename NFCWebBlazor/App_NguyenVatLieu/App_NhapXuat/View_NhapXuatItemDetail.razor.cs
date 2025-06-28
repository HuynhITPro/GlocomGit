using BlazorBootstrap;
using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class View_NhapXuatItemDetail
    {
        [Inject] BrowserService browserService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

      
        private bool boolnhapkho()
        {
            if (nvlNhapXuatKhoShowcrr == null)
                return false;
            if (nvlNhapXuatKhoShowcrr.STTCT >= 0)
                return true;
            return false;
        }
        bool CheckQuyen { get; set; } = false;
        bool CheckQuyenChatLuong { get; set; } = false;
        bool showtrahang {get;set;} = false;
        protected override async Task OnInitializedAsync()
        {
            CheckQuyen = phanQuyenAccess.CheckDeleteNhapXuatKho(nvlNhapXuatKhoShowcrr.UserInsert, Model.ModelAdmin.users);
            CheckQuyenChatLuong = await phanQuyenAccess.CreateNVLChatLuongBienBan(Model.ModelAdmin.users);
            var dimension = await browserService.GetDimensions();
            //// var heighrow = await browserService.GetHeighWithID("divcontainer");
            //int height = dimension.Height - 120;
            heightgrid = string.Format("{0}px", dimension.Height - 90);
            int width = dimension.Width;
            if (width < 768)
            {
                Ismobile = true;
                idgrid = "customGridnotheader";
                showfooterthanhtien = "UserInsert";//Hiển thị tổng ở cột này
            }
            else
            {
                idgrid = "griddetailnhapkho";
                Ismobile = false;
            }
            if(!string.IsNullOrEmpty(MaHang))
            {
                var filterCriteria = CriteriaOperator.Parse(string.Format("[MaHang] = '{0}'", MaHang));
              
                dxGrid.SetFilterCriteria(filterCriteria);
            }
            //if(nvlNhapXuatKhoShowcrr.LyDo== "Nhập mua hàng")
            //{
            //    showtrahang = true;
            //}


            //if (Ismobile)
            //{
            //    idgrid = "customGridnotheader";
            //    showfooterthanhtien = "UserInsert";//Hiển thị tổng ở cột này
            //}
            // return base.OnInitializedAsync();
        }
        
        public string headerkehoach()
        {

            if (nvlNhapXuatKhoShowcrr.STTCT > 0)
            {
                return "Số ĐH";
            }
            return "Số KH";
        }
        private async void loadagain()
        {
            nvlNhapXuatKhoShowcrr.lstNhapXuatItemShows = null;
            await loaddinhmucchitietAsync();
        }
        private async Task loaddinhmucchitietAsync()
        {
            if (nvlNhapXuatKhoShowcrr == null)
                return;
            if (lstdata == null)
                lstdata = new List<NvlNhapXuatItemShow>();
            lstdata.Clear();
            if (nvlNhapXuatKhoShowcrr.lstNhapXuatItemShows == null)
            {
                nvlNhapXuatKhoShowcrr.lstNhapXuatItemShows = new List<NvlNhapXuatItemShow>();
                try
                {
                    string sql = "";


                    #region Truy vấn mới có liên kết kiểm hàng
                    sql = string.Format(@"use NVLDB
                    declare @SerialCT int={0}
                    --Xử lý dữ liệu từ function trả ra của SQL
					declare @sqlex nvarchar(3000)
					IF OBJECT_ID('tempdb..#tbl') IS NOT NULL
						DROP TABLE #tbl;
					IF OBJECT_ID('tempdb..#tblnxitem') IS NOT NULL
						DROP TABLE #tblnxitem;
					-- Tạo bảng biến để lưu kết quả
					create TABLE #tbl  (
						MaHang NVARCHAR(100), 
						KetLuanChungTu NVARCHAR(100),
						KetLuanHangHoa nvarchar(100),SoLo nvarchar(100),Loai int
					)
					SET @sqlex = '
					INSERT INTO #tbl(MaHang,KetLuanChungTu, KetLuanHangHoa,SoLo,Loai)
					SELECT MaHang,KetLuanChungTu, KetLuanHangHoa,SoLo,Loai
					FROM OPENQUERY(SP, ''SELECT * FROM NguyenVatLieu.dbo.Func_KetQuaNghiemThuDauVao_List(' + CAST(@SerialCT AS NVARCHAR) + ')'')';

				EXEC sp_executesql @sqlex

                -- Sử dụng dữ liệu từ bảng biến
				
                  select * Into #tblnxitem from NvlNhapXuatItem nxitem where SerialCT=@SerialCT 

                 declare @checktable nvarchar(100)
                 select top 1 @checktable=TableName from #tblnxitem nxitem


                 if(@checktable='NvlDonDatHangItem')
                 begin

                  declare @tblmhknt as Table(MaHang nvarchar(100) primary key)
	                insert into @tblmhknt(MaHang)
	                select MaHang from SP.NguyenVatLieu.dbo.View_MaHangKhongNghiemThu
                    where MaHang in (select MaHang from #tblnxitem group by MaHang)

	            Select nxitem.*, NvlHangHoa.TenHang,qrydgdn.DonGia as DonGiaDN
				,case when khongnt.MaHang is not null then N'Không kiểm' else
				(case when qrynt.MaHang is null then N'Chưa kiểm'
				when qrynt.MaHang is not null then qrynt.KetLuanHangHoa end) end as TinhTrang,dhh.UserInsert as NguoiDN,dhh.Serial as SerialDN

				FROM (Select * FROM NvlNhapXuatItem Where SerialCT = @SerialCT ) nxitem
			JOIN NvlHangHoa on NvlHangHoa.MaHang = nxitem.MaHang
			left join (select MaHang,KetLuanChungTu,KetLuanHangHoa,SoLo,Loai from #tbl) as qrynt

				on nxitem.MaHang=qrynt.MaHang and isnull(nxitem.SoLo,'')= case when ISNULL(Loai, 1) = 1 then qrynt.SoLo else  isnull(nxitem.SoLo,'') end
			left join dbo.NvlDonDatHangItem ddhitem on (ddhitem.Serial=nxitem.SerialKHDH and nxitem.TableName='NvlDonDatHangItem')
				left join dbo.NvlDonDatHang dhh on ddhitem.SerialMaDH=dhh.Serial
				left join  dbo.NvlKeHoachMuaHangItem
				as qrydgdn on ddhitem.SerialLink=qrydgdn.Serial
                left join @tblmhknt khongnt on nxitem.MaHang=khongnt.MaHang

            end
            else
            begin
                    Select nxitem.*, NvlHangHoa.TenHang,N'Không kiểm' as TinhTrang,khmh.NguoiDN,khmh.Serial as SerialDN
		            FROM (Select * FROM NvlNhapXuatItem Where SerialCT = @SerialCT) nxitem
                        JOIN NvlHangHoa on NvlHangHoa.MaHang = nxitem.MaHang
						   left join  dbo.NvlKeHoachMuaHangItem
						    as khmhitem on (khmhitem.Serial=nxitem.SerialKHDH
							and nxitem.TableName='NvlKeHoachMuaHangItem')
							left join dbo.NvlKehoachMuaHang khmh on khmhitem.SerialDN=khmh.Serial

                end
			Drop Table #tbl
            Select * INTO #tmp FROM NvlNhapXuatItem Where SerialCT = @SerialCT
            DROP TABLE #tmp
            DROP TABLE #tblnxitem", nvlNhapXuatKhoShowcrr.Serial);
                    #endregion
                    CallAPI callAPI = new CallAPI();
                    PanelVisible = true;
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemShow>>(json);
                        lstdata = query;
                        nvlNhapXuatKhoShowcrr.lstNhapXuatItemShows.AddRange(lstdata);
                        // await GotoMainForm.InvokeAsync();
                    }
                }
                catch (Exception ex)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Success, $"Lỗi: " + ex.Message));
                }
                finally
                {
                    PanelVisible = false;

                    StateHasChanged();

                }

            }
            else
            {

                lstdata.AddRange(nvlNhapXuatKhoShowcrr.lstNhapXuatItemShows);

                StateHasChanged();

            }
            return;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await loaddinhmucchitietAsync();


            }
            //Console.WriteLine("Nó render item");

        }
        public async void ShowFlyout(NvlNhapXuatItemShow nvlNhapXuatItemShow)
        {
            try
            {
                await dxFlyoutchucnang.CloseAsync();
                nvlNhapXuatItemShowcrr = nvlNhapXuatItemShow;
                idflychucnang = "#" + idelement(nvlNhapXuatItemShow.Serial);
                //IsOpenfly = true;
                await dxFlyoutchucnang.ShowAsync();
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine("Lỗi:"+ex.Message);

            }

        }

        string lydoxoa = "";
        private async void lydoxoaCallBack(string lydo)
        {
            lydoxoa = lydo;
            
            await  showpopup.InvokeAsync(new ShowFragmentinModal(null,"",false));
            if (!string.IsNullOrEmpty(lydoxoa))
            {
                string sql = "NVLDB.dbo.NvlNhapXuatItem_Delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlNhapXuatItemShowcrr.Serial));
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
                            ToastService.Notify(new ToastMessage(ToastType.Success, "Xóa thành công"));
                            int? flag = nvlNhapXuatItemShowcrr.flag;
                            if (flag > 0)
                            {
                                lstdata.RemoveAll(it => it.flag.Value == flag.Value);
                            }
                            else
                            {
                                lstdata.RemoveAll(it => it.Serial == nvlNhapXuatItemShowcrr.Serial);
                                // msgBox.Show("Xóa thành công", IconMsg.iconssuccess);
                               
                            }

                            // msgBox.Show("Xóa thành công", IconMsg.iconssuccess);
                           // lstdata.Remove(nvlNhapXuatItemShowcrr);
                            await dxGrid.SaveChangesAsync();

                            dxGrid.Reload();
                        }
                        else
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Warning, $"{query[0].ketqua}, {query[0].ketquaexception}"));
                            //msgBox.Show("Lỗi: " + query[0].ketqua, IconMsg.iconssuccess);

                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, $"Lỗi: " + ex.Message));
                }

            }
        }
        private async void deleteitem()
        {
            IsOpenfly = false;
            await dxFlyoutchucnang.CloseAsync();
            if (!phanQuyenAccess.CheckDeleteNhapXuatKho(nvlNhapXuatItemShowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không có quyền xóa"));
                return;
            }
            //Urc_NhapLyDo urc_NhapLyDo = new Urc_NhapLyDo();
            //urc_NhapLyDo.GetLyDo = EventCallback.Factory.Create<string>(this, lydoxoaCallBack);
            //urc_NhapLyDo.Show();
           
             renderFragment = builder =>
             {
                 builder.OpenComponent<Urc_NhapLyDo>(0);


                 builder.AddAttribute(1, "GetLyDo", EventCallback.Factory.Create<string>(this, lydoxoaCallBack));
                 builder.CloseComponent();
             };
            ShowFragmentinModal showFragmentinModal = new ShowFragmentinModal();
            showFragmentinModal.show = true;
            showFragmentinModal.title = "Nhập lý do tại sao XÓA?";
            showFragmentinModal.renderFragment = renderFragment;
            if(showpopup.HasDelegate)
            {
                await showpopup.InvokeAsync(showFragmentinModal);
            }
            

        }
        public async Task trahangClickAsync()
        {
            IsOpenfly = false;
            await dxFlyoutchucnang.CloseAsync();
            if (!phanQuyenAccess.CheckDelete(nvlNhapXuatItemShowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không phải người tạo dòng này"));
                return;
            }

            //Urc_NhapLyDo urc_NhapLyDo = new Urc_NhapLyDo();
            //urc_NhapLyDo.GetLyDo = EventCallback.Factory.Create<string>(this, lydoxoaCallBack);
            //urc_NhapLyDo.Show();
            NvlNhapXuatItemShow nvlNhapXuatItemShow = new NvlNhapXuatItemShow();
            nvlNhapXuatItemShow.Serial=nvlNhapXuatItemShowcrr.Serial;
            nvlNhapXuatItemShow.GhiChu="Trả hàng nhà cung cấp";
            nvlNhapXuatItemShow.SLNhap = null;
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NvlTraHang>(0);
                builder.AddAttribute(1, "nvlNhapXuatItemShowcrr",nvlNhapXuatItemShow);
                
                //builder.AddAttribute(1, "GetLyDo", EventCallback.Factory.Create<NvlNhapXuatItemShow>(this, lydoxoaCallBack));
                builder.CloseComponent();
            };
            ShowFragmentinModal showFragmentinModal = new ShowFragmentinModal();
            showFragmentinModal.show = true;
            showFragmentinModal.title = "Trả hàng nhà cung cấp?";
            showFragmentinModal.renderFragment = renderFragment;
            if (showpopup.HasDelegate)
            {
                await showpopup.InvokeAsync(showFragmentinModal);
            }
        }
        private async Task edititemAsync()
        {
            IsOpenfly = false;
            await dxFlyoutchucnang.CloseAsync();
            if (!phanQuyenAccess.CheckDeleteNhapXuatKho(nvlNhapXuatItemShowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không phải người tạo dòng này"));
                return;
            }

            //Urc_NhapLyDo urc_NhapLyDo = new Urc_NhapLyDo();
            //urc_NhapLyDo.GetLyDo = EventCallback.Factory.Create<string>(this, lydoxoaCallBack);
            //urc_NhapLyDo.Show();
            nvlNhapXuatItemShowcrr.SoLuong = nvlNhapXuatItemShowcrr.SLNhap + nvlNhapXuatItemShowcrr.SLXuat;
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NhapXuatItemAdd>(0);
                builder.AddAttribute(1, "nvlNhapXuatItemShowcrr", nvlNhapXuatItemShowcrr.CopyClass());
                builder.AddAttribute(2, "nvlNhapXuatKhoShowcrr", nvlNhapXuatKhoShowcrr.CopyClass());
                builder.AddAttribute(4, "LoaiNhapXuat", LoaiNhapXuat);
                builder.AddAttribute(3, "AfterEdit", EventCallback.Factory.Create<NvlNhapXuatItemShow>(this, callaftereditAsync));
                //builder.AddAttribute(1, "GetLyDo", EventCallback.Factory.Create<NvlNhapXuatItemShow>(this, lydoxoaCallBack));
                builder.CloseComponent();
            };
            ShowFragmentinModal showFragmentinModal = new ShowFragmentinModal();
            showFragmentinModal.show = true;
            showFragmentinModal.title = "Chỉnh sửa thông tin";
            showFragmentinModal.renderFragment = renderFragment;
            if (showpopup.HasDelegate)
            {
                await showpopup.InvokeAsync(showFragmentinModal);
            }
        }
        public void setNvlNhapXuatItemShow(NvlNhapXuatItemShow nvlNhapXuatItemShowcrr_get, NvlNhapXuatItemShow nvlNhapXuatItemShowcrr_set)
        {
            
            nvlNhapXuatItemShowcrr_set.SerialLink = nvlNhapXuatItemShowcrr_get.SerialLink;
            nvlNhapXuatItemShowcrr_set.SerialKHDH = nvlNhapXuatItemShowcrr_get.SerialKHDH;
            nvlNhapXuatItemShowcrr_set.TableName = nvlNhapXuatItemShowcrr_get.TableName;
            nvlNhapXuatItemShowcrr_set.MaHang = nvlNhapXuatItemShowcrr_get.MaHang;
            nvlNhapXuatItemShowcrr_set.DVT = nvlNhapXuatItemShowcrr_get.DVT;
            nvlNhapXuatItemShowcrr_set.SLNhap = nvlNhapXuatItemShowcrr_get.SLNhap;
            nvlNhapXuatItemShowcrr_set.SLXuat = nvlNhapXuatItemShowcrr_get.SLXuat;
            nvlNhapXuatItemShowcrr_set.DonGia = nvlNhapXuatItemShowcrr_get.DonGia;
            nvlNhapXuatItemShowcrr_set.SLNhapTT = nvlNhapXuatItemShowcrr_get.SLNhapTT;
            nvlNhapXuatItemShowcrr_set.SLXuatTT = nvlNhapXuatItemShowcrr_get.SLXuatTT;
            nvlNhapXuatItemShowcrr_set.DVTTT = nvlNhapXuatItemShowcrr_get.DVTTT;
            nvlNhapXuatItemShowcrr_set.TyLeQuyDoi = nvlNhapXuatItemShowcrr_get.TyLeQuyDoi;
            nvlNhapXuatItemShowcrr_set.KhachHang_XuatXu = nvlNhapXuatItemShowcrr_get.KhachHang_XuatXu;
            nvlNhapXuatItemShowcrr_set.NgayHetHan = nvlNhapXuatItemShowcrr_get.NgayHetHan;
            nvlNhapXuatItemShowcrr_set.NgaySanXuat = nvlNhapXuatItemShowcrr_get.NgaySanXuat;
            nvlNhapXuatItemShowcrr_set.MaKien = nvlNhapXuatItemShowcrr_get.MaKien;
            nvlNhapXuatItemShowcrr_set.SoLo = nvlNhapXuatItemShowcrr_get.SoLo;
            nvlNhapXuatItemShowcrr_set.SoXe = nvlNhapXuatItemShowcrr_get.SoXe;
            nvlNhapXuatItemShowcrr_set.GhiChu = nvlNhapXuatItemShowcrr_get.GhiChu;
            nvlNhapXuatItemShowcrr_set.Barcode = nvlNhapXuatItemShowcrr_get.Barcode;
            nvlNhapXuatItemShowcrr_set.MaSP = nvlNhapXuatItemShowcrr_get.MaSP;
            nvlNhapXuatItemShowcrr_set.ArticleNumber = nvlNhapXuatItemShowcrr_get.ArticleNumber;
            nvlNhapXuatItemShowcrr_set.UserInsert = nvlNhapXuatItemShowcrr_get.UserInsert;
            nvlNhapXuatItemShowcrr_set.ViTri = nvlNhapXuatItemShowcrr_get.ViTri;
   
            nvlNhapXuatItemShowcrr_set.DauTuan = nvlNhapXuatItemShowcrr_get.DauTuan;
        }
        private async Task callaftereditAsync(NvlNhapXuatItemShow nvlNhapXuatItemShow)
        {
            setNvlNhapXuatItemShow(nvlNhapXuatItemShow, nvlNhapXuatItemShowcrr);
            ShowFragmentinModal showFragmentinModal = new ShowFragmentinModal();
            showFragmentinModal.show = false;
            await showpopup.InvokeAsync(showFragmentinModal);

            StateHasChanged();
        }
    }
}
