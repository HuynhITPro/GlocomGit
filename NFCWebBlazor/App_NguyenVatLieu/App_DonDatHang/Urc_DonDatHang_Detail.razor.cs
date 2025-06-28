using BlazorBootstrap;

using Microsoft.AspNetCore.Components;

using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using static NFCWebBlazor.App_Bom.Page_DuyetDinhMuc;
using static NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Page_DonDatHang_Master;


namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Urc_DonDatHang_Detail
    {
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        [Inject]
        ToastService ToastService { get; set; }
        bool PhanQuyenCheck = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                Console.WriteLine(this.GetType().Name);
                PhanQuyenCheck = phanQuyenAccess.CheckDelete(nVLDonDatHangShowcrr.UserInsert, ModelAdmin.users);


                if (nVLDonDatHangShowcrr.lstitem == null)
                {
                    _ = searchAsync();
                }
                else
                {
                    lstdata = nVLDonDatHangShowcrr.lstitem;
                    //Grid.Reload();
                    StateHasChanged();
                }
            }

        }
        private async Task searchAsync()
        {
            PanelVisible = true;

            string sql = "";
            try
            {

                sql = string.Format(@"Use NVLDB
                            declare @Serial int={0}
                    declare @NgayDatHang date
					 declare @MaNCCDuyet nvarchar(100)

						
						 select @NgayDatHang=NgayDatHang,@MaNCCDuyet=MaNCC from NvlDonDatHang where Serial=@Serial
	 
						 declare @tbldongiaduyet Table(MaHang nvarchar(100),DonGiaDuyet decimal(18,4))

						 insert into @tbldongiaduyet(MaHang,DonGiaDuyet)
						 select MaHang,DonGiaDuyet from
					(SELECT ROW_NUMBER() Over (Partition by MaHang Order by Serial desc) as [Index],[MaHang],DonGiaDuyet
					  FROM [dbo].[NvlDuyetGiaItem]
					  where DonGiaDuyet >0 and
					  SerialLink in (select Serial from NvlDuyetGia 
					  where ISNULL(NgayApDung,@NgayDatHang)<=@NgayDatHang and ISNULL(NgayKetThuc,@NgayDatHang)>=@NgayDatHang)
					  and MaNCCDuyet=@MaNCCDuyet)  as qry
					  where [Index]=1

                        Select *,SLDatHang-SLTheoDoi as SLConLai,SLDatHang as SoLuong,tblgiaduyet.DonGiaDuyet  FROM
						(							
						SELECT dhitem.[Serial], dhitem.[SerialMaDH], dhitem.[MaHang], dhitem.[SLDatHang], dhitem.SignInt, dhitem.NoiDung,
                                   dhitem.[SLTheoDoi], dhitem.[DVT],dhitem.[DonGia],dhitem.[SerialLink], 
                                   dhitem.[MaNCC],dhitem.[NgayDKNhapKho],dhitem.[UserInsert],dhitem.[NgayInsert],
                                   dhitem.[Group], dhitem.GroupIndex,dhitem.GiaTri,hh.TenHang, NvlNhaCungCap.TenNCC, ISNULL(dhitem.DonGia,0) * IsNull(dhitem.SLDatHang,0) as ThanhTien 
                                    ,isnull(qrytk.SLTon,0) as SLTon,hh.MinTK,hh.MaxTK,dhitem.DuyetItemMsg,nh.PhanLoai,STT,SLHuy
									from (SELECT [Serial], [SerialMaDH], [MaHang], [SLDatHang], 1 as SignInt, DienGiai as NoiDung,
                                   [SLTheoDoi], [DVT],[DonGia],[SerialLink], 
                                   [MaNCC],[NgayDKNhapKho],[UserInsert],[NgayInsert],
                                   N'NvlDonDatHangItem' as [Group], 0 as [GroupIndex],GiaTri,DuyetItemMsg,isnull(STT,0) as STT,SLHuy
								   
                            FROM [NvlDonDatHangItem]
                            Where SerialMaDH = @Serial) dhitem 
							left join (select MaHang,SLTon from dbo.GetTonKhoEx()) as qrytk
							on dhitem.MaHang=qrytk.MaHang
							INNER JOIN NvlHangHoa hh ON hh.MaHang = dhitem.MaHang 
							inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
							Inner JOIN NvlNhaCungCap ON NvlNhaCungCap.MaNCC = dhitem.MaNCC
                          
                        ) as donhangchitiet left join @tbldongiaduyet tblgiaduyet on donhangchitiet.MaHang=tblgiaduyet.MaHang
                        order by [Serial]

                        SELECT  [Serial],[SerialLinkMaster],[SerialLinkItem],[TableName],[UserDuyet],[LoaiDuyet],[GhiChu],usr.TenUser as TenUserDuyet,isnull(usr.PathImg,'UserImage/user.png') as PathImg,[NgayInsert]
                        FROM [dbo].[NvlKyDuyetItem]  it inner join DBMaster.dbo.Users usr on it.UserDuyet=usr.UsersName
                        where SerialLinkMaster=@Serial and TableName='NvlDonDatHang' 
						SELECT [SerialLink],[TableName],[MsgWait],[MsgReply]
						,[UserInsert],[UserReply],[NgayInsert],[NgayReply]
						FROM [dbo].[NvlMsgManage]
						where TableName='NvlDonDatHangItem' and SerialLink in (select Serial from [NvlDonDatHangItem] where SerialMaDH=@Serial)
                    ", nVLDonDatHangShowcrr.Serial);


                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<CustomRootItem>(json);
                    if (query != null)
                    {

                        if (query.lstmuahangitem != null)
                        {
                            lstdata = new System.Collections.ObjectModel.ObservableCollection<NVLDonDatHangItemShow>(query.lstmuahangitem);


                            nVLDonDatHangShowcrr.lstitem = lstdata;

                            if (query.lstkyduyet != null)
                            {
                                foreach (var it in query.lstkyduyet)
                                {
                                    foreach (var item in nVLDonDatHangShowcrr.lstitem)
                                    {
                                        if (it.SerialLinkItem == item.Serial)
                                        {
                                            if (item.lstduyetitem == null)
                                                item.lstduyetitem = new List<NvlKyDuyetItemShow>();
                                            item.lstduyetitem.Add(it);
                                            if (it.LoaiDuyet.Contains("Kiểm"))
                                                item.TextKiem += it.TenUserDuyet + "; ";
                                            else
                                            {
                                                item.TextDuyet += it.TenUserDuyet + "; ";
                                                item.DaDuyet = true;
                                            } 
                                            break;
                                        }
                                    }
                                }
                            }
                            if(query.lstMsg!=null)
                            {
                                foreach (var it in query.lstMsg)
                                {
                                    foreach (var item in nVLDonDatHangShowcrr.lstitem)
                                    {
                                        if (it.SerialLink == item.Serial)
                                        {
                                            if (!item.DaDuyet)
                                            {
                                                item.TextDuyet = "Không duyệt";
                                                item.MsgKhongDuyet = it.MsgWait;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        query.lstmuahangitem.Clear();
                        query.lstkyduyet.Clear();
                    }
                    Grid.Reload();
                }
                PanelVisible = false;
                //keHoachMuaHang_Show.lstitem = query;

                //keHoachMuaHang_Show.lstitem.AddRange(query);

            }
            catch (Exception ex)
            {
                ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message}"));
            }
            finally
            {
                PanelVisible = false;
                //Grid.Reload();
                StateHasChanged();
            }
        }
        public async void ShowFlyout(NVLDonDatHangItemShow nVLDonDatHangItemShow)
        {
            await dxFlyoutchucnang.CloseAsync();
            nvlKeHoachMuaHangItemShowcrr = nVLDonDatHangItemShow;
            idflychucnang = "#" + idelement(nvlKeHoachMuaHangItemShowcrr.Serial);
            if (phanQuyenAccess.CheckDelete(nvlKeHoachMuaHangItemShowcrr.UserInsert, ModelAdmin.users))
            {
                PhanQuyenCheck = true;
            }
            else
                PhanQuyenCheck = false;


            await dxFlyoutchucnang.ShowAsync();

        }
        private async Task DeleteItemAsync()
        {

            if (!phanQuyenAccess.CheckDelete(nvlKeHoachMuaHangItemShowcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new(ToastType.Warning, $"Bạn không có quyền xóa dòng này do bạn không phải người tạo"));
                return;
            }

            bool bl = await dialogMsg.Show("THÔNG BÁO", string.Format("Bạn có chắc muốn xóa mã hàng {0}?", nvlKeHoachMuaHangItemShowcrr.MaHang));
            if (bl)
            {
                dxFlyoutchucnang.CloseAsync();
                try
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlDonDatHangChiTiet_Delete";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();

                    lstpara.Add(new ParameterDefine("@Serial", nvlKeHoachMuaHangItemShowcrr.Serial));
                    lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));

                    lstpara.Add(new ParameterDefine("@Group", "NvlDonDatHangItem"));
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            ToastService.Notify(new(ToastType.Success, $"Xóa thành công"));
                            lstdata.Remove(nvlKeHoachMuaHangItemShowcrr);
                            //await Grid.SaveChangesAsync();
                        }
                        else
                        {
                            ToastService.Notify(new(ToastType.Danger, $"{query[0].ketqua}, {query[0].ketquaexception}"));

                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message} Xóa không được"));
                }


            }


        }

        private async Task DuyetItemAsync(NVLDonDatHangItemShow nvlKeHoachMuaHangItemShow)
        {
            if (nVLDonDatHangShowcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in nVLDonDatHangShowcrr.lstkyduyet)
            {
                if (ModelAdmin.users.UsersName == it.UserDuyet)
                {
                    checkduyet = true;
                    LoaiDuyet = it.LoaiDuyet;
                    break;
                }
            }
            if (!checkduyet)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có trong danh sách được duyệt!"));
                return;
            }

            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlKyDuyetItem_Insert";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialLinkMaster", nvlKeHoachMuaHangItemShow.SerialMaDH));
                lstpara.Add(new ParameterDefine("@SerialLinkItem", nvlKeHoachMuaHangItemShow.Serial));
                lstpara.Add(new ParameterDefine("@TableName", "NvlDonDatHang"));
                lstpara.Add(new ParameterDefine("@UserDuyet", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@GhiChu", ""));


                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        //ToastService.Notify(new(ToastType.Success, $"Duyệt thành công"));
                        if (LoaiDuyet == "Duyệt")
                        {
                            nVLDonDatHangShowcrr.CountDuyet += 1;
                            nvlKeHoachMuaHangItemShow.TextDuyet = ModelAdmin.users.TenUser;
                        }
                        else
                            nvlKeHoachMuaHangItemShow.TextKiem = ModelAdmin.users.TenUser;
                        await Grid.SaveChangesAsync();

                    }
                    else
                    {
                        ToastService.Notify(new(ToastType.Danger, $"Lỗi:{query[0].ketqua}, {query[0].ketquaexception} Duyệt không được"));

                    }
                    if (GotoMasterGrid.HasDelegate)
                    {
                        await GotoMasterGrid.InvokeAsync();
                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message} Duyệt không được"));
            }

        }
        private async Task DuyetItemAllAsync()
        {
            if (nVLDonDatHangShowcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in nVLDonDatHangShowcrr.lstkyduyet)
            {
                if (ModelAdmin.users.UsersName == it.UserDuyet)
                {
                    checkduyet = true;
                    LoaiDuyet = it.LoaiDuyet;
                    break;
                }
            }
            if (!checkduyet)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có trong danh sách được duyệt!"));
                return;
            }

            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlKyDuyetItem_InsertAll";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialLinkMaster", nVLDonDatHangShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@SerialLinkItem", 0));
                lstpara.Add(new ParameterDefine("@TableName", "NvlDonDatHang"));
                lstpara.Add(new ParameterDefine("@UserDuyet", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@GhiChu", ""));


                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        ToastService.Notify(new(ToastType.Success, $"Duyệt thành công"));
                        if (LoaiDuyet == "Duyệt")
                        {
                            nVLDonDatHangShowcrr.CountDuyet = nVLDonDatHangShowcrr.CountTong;
                            if(nVLDonDatHangShowcrr.lstitem!=null)
                            {
                                foreach (var it in nVLDonDatHangShowcrr.lstitem)
                                {
                                    it.TextDuyet = ModelAdmin.users.TenUser;
                                }
                            }
                            
                            // nvlKeHoachMuaHangItemShow.TextDuyet = ModelAdmin.users.TenUser;
                        }
                        else
                        {
                            if (nVLDonDatHangShowcrr.lstitem != null)
                            {
                                foreach (var it in nVLDonDatHangShowcrr.lstitem)
                                {
                                    it.TextKiem = ModelAdmin.users.TenUser;
                                }
                            }
                        }
                        //nvlKeHoachMuaHangItemShow.TextKiem = ModelAdmin.users.TenUser;
                        await Grid.SaveChangesAsync();
                        if (GotoMasterGrid.HasDelegate)
                        {
                            GotoMasterGrid.InvokeAsync();
                        }
                    }
                    else
                    {
                        ToastService.Notify(new(ToastType.Danger, $"Lỗi: {query[0].ketquaexception} Duyệt không được"));

                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message} Duyệt không được"));
            }

        }
        private async Task HuyDuyetItemAllAsync()
        {
            if (nVLDonDatHangShowcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in nVLDonDatHangShowcrr.lstkyduyet)
            {
                if (ModelAdmin.users.UsersName == it.UserDuyet)
                {
                    checkduyet = true;
                    LoaiDuyet = it.LoaiDuyet;
                    break;
                }
            }
            if (!checkduyet)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có trong danh sách được duyệt!"));
                return;
            }

            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlKyDuyetItem_DeleteAll";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialLinkMaster", nVLDonDatHangShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@SerialLinkItem", 0));
                lstpara.Add(new ParameterDefine("@TableName", "NvlDonDatHang"));
                lstpara.Add(new ParameterDefine("@UserDuyet", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@GhiChu", ""));


                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        //ToastService.Notify(new(ToastType.Success, $"Duyệt thành công"));
                        if (LoaiDuyet == "Duyệt")
                        {
                            nVLDonDatHangShowcrr.CountDuyet = nVLDonDatHangShowcrr.CountTong;
                            foreach (var it in nVLDonDatHangShowcrr.lstitem)
                            {
                                it.TextDuyet = "";
                            }
                            // nvlKeHoachMuaHangItemShow.TextDuyet = ModelAdmin.users.TenUser;
                        }
                        else
                        {
                            foreach (var it in nVLDonDatHangShowcrr.lstitem)
                            {
                                it.TextKiem = "";
                            }
                        }
                        //nvlKeHoachMuaHangItemShow.TextKiem = ModelAdmin.users.TenUser;
                        await Grid.SaveChangesAsync();
                        if (GotoMasterGrid.HasDelegate)
                        {
                            GotoMasterGrid.InvokeAsync();
                        }
                        ToastService.Notify(new ToastMessage(ToastType.Secondary, "Hủy duyệt thành công"));
                    }
                    else
                    {
                        ToastService.Notify(new(ToastType.Danger, $"Lỗi: {query[0].ketquaexception} Duyệt không được"));

                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message} Duyệt không được"));
            }

        }
        private async Task showmsgAsync(NVLDonDatHangItemShow nvlKeHoachMuaHangItemShow)
        {
            if (nVLDonDatHangShowcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            NvlMsgManage nvlMsgManage = new NvlMsgManage();
            nvlMsgManage.TableName = "NvlDonDatHangItem";
            nvlMsgManage.SerialLink = nvlKeHoachMuaHangItemShow.Serial;
            nvlMsgManage.UserInsert = ModelAdmin.users.UsersName;
            nvlKeHoachMuaHangItemShowcrr = nvlKeHoachMuaHangItemShow;
            renderFragment = builder =>
            {
                builder.OpenComponent<Page_SendMsg>(0);
                builder.AddAttribute(1, "nvlMsgManage", nvlMsgManage);
                builder.AddAttribute(1, "sqlprocedure", "NVLDB.dbo.NvlDuyetGiaItem_KhongDuyet_Insert");
                builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<NvlMsgManage>(this, aftershowmsg));
                builder.CloseComponent();
            };

            await dxPopup.showAsync("GỬI LỜI NHẮN");
            await dxPopup.ShowAsync();
         
        }
        private async void EditItem()
        {
            try
            {
                dxFlyoutchucnang.CloseAsync();
                NVLDonDatHangItemShow nVLDonDatHangItemShow = nvlKeHoachMuaHangItemShowcrr.CopyClass();
                renderFragment = builder =>
               {
                   builder.OpenComponent<Page_DonHangItem_Edit>(0);
                   builder.AddAttribute(1, "nVLDonDatHangItemShowcrr", nVLDonDatHangItemShow);
                   builder.AddAttribute(2, "AfterEdit", EventCallback.Factory.Create<NVLDonDatHangItemShow>(this, afteredit));
                   builder.CloseComponent();
               };

                await dxPopup.showAsync("SỬA CHI TIẾT");
                await dxPopup.ShowAsync();
            }
            catch(Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, ex.Message));
                return;
            }
        }
        private void afteredit(NVLDonDatHangItemShow nVLDonDatHangItemShow)
        {
             dxPopup.CloseAsync();
            nvlKeHoachMuaHangItemShowcrr.DonGia = nVLDonDatHangItemShow.DonGia;
            nvlKeHoachMuaHangItemShowcrr.MaNCC = nVLDonDatHangItemShow.MaNCC;
            nvlKeHoachMuaHangItemShowcrr.TenNCC = nVLDonDatHangItemShow.TenNCC;
            nvlKeHoachMuaHangItemShowcrr.NoiDung = nVLDonDatHangItemShow.NoiDung;
            Grid.Reload();
        }
        private async Task HuyDuyetItemAsync(NVLDonDatHangItemShow nvlKeHoachMuaHangItemShow)
        {
            if (nVLDonDatHangShowcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in nVLDonDatHangShowcrr.lstkyduyet)
            {
                if (ModelAdmin.users.UsersName == it.UserDuyet)
                {
                    checkduyet = true;
                    LoaiDuyet = it.LoaiDuyet;
                    break;
                }
            }
            if (!checkduyet)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có trong danh sách được duyệt!"));
                return;
            }

            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.NvlKyDuyetItem_Delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialLinkItem", nvlKeHoachMuaHangItemShow.Serial));
                lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@TableName", "NvlDonDatHang"));


                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        //ToastService.Notify(new(ToastType.Success, $"Duyệt thành công"));
                        if (LoaiDuyet == "Duyệt")
                        {
                            nvlKeHoachMuaHangItemShow.TextDuyet = "";
                            nVLDonDatHangShowcrr.CountDuyet -= 1;
                        }
                        else
                            nvlKeHoachMuaHangItemShow.TextKiem = "";
                        await Grid.SaveChangesAsync(); 

                    }
                    //else
                    //{
                    //    ToastService.Notify(new(ToastType.Danger, $"{query[0].ketqua},{query[0].ketquaexception}"));

                    //}
                    if (GotoMasterGrid.HasDelegate)
                    {
                        GotoMasterGrid.InvokeAsync();
                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                //ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message} Hủy không được"));
            }

        }
        private async void aftershowmsg(NvlMsgManage nvlMsgManage)
        {
            await dxPopup.CloseAsync();
            if (!string.IsNullOrEmpty(nvlMsgManage.MsgWait))
            {
                nvlKeHoachMuaHangItemShowcrr.MsgKhongDuyet = nvlMsgManage.MsgWait;

                nvlKeHoachMuaHangItemShowcrr.TextDuyet = "Không duyệt";
               
                await  HuyDuyetItemAsync(nvlKeHoachMuaHangItemShowcrr);
              
                 await Grid.SaveChangesAsync();
                StateHasChanged();
            }
        }
        public void ReloadGrid()
        {

            Grid.MakeRowVisible(Grid.GetVisibleRowCount() - 1);
            Grid.Reload();
        }

        private bool Visibleduyetall()
        {

            if (nVLDonDatHangShowcrr.lstkyduyet != null)
            {
                foreach (var it in nVLDonDatHangShowcrr.lstkyduyet)
                {
                    if (ModelAdmin.users.UsersName == it.UserDuyet)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private async void EditItem(NVLDonDatHangItemShow nvlKeHoachMuaHangItemShow)
        {
            nvlKeHoachMuaHangItemShowcrr = nvlKeHoachMuaHangItemShow;


            // PanelVisible = true;
            //renderFragment = builder =>
            //{
            //    builder.OpenComponent<Urc_KeHoachMuaHang_AddItem>(0);
            //    builder.AddAttribute(1, "keHoachMuaHang_ShowCrr", keHoachMuaHangcrr);
            //    // builder.AddAttribute(2, "LoaiKeHoach", LoaiKeHoach);
            //    builder.AddAttribute(2, "nvlkhmhitem", nvlKeHoachMuaHangItemShow.CopyClass());
            //    builder.AddAttribute(3, "visibledetail", false);
            //    builder.AddAttribute(4, "GotoMainForm", EventCallback.Factory.Create<NvlKeHoachMuaHangItemShow>(this, GotoMainForm));
            //    //builder.OpenComponent(0, componentType);
            //    builder.CloseComponent();
            //};
            await dxPopup.showAsync("SỬA CHI TIẾT");
            await dxPopup.ShowAsync();
        }

        private void Setclass(NVLDonDatHangItemShow nvlKeHoachMuaHangItem_set, NVLDonDatHangItemShow nvlKeHoachMuaHangItem_get)
        {
            ////nvlKeHoachMuaHangItem_set.Serial = nvlKeHoachMuaHangItem_get.Serial;
            //nvlKeHoachMuaHangItem_set.SerialDN = nvlKeHoachMuaHangItem_get.SerialDN;
            //nvlKeHoachMuaHangItem_set.MaHang = nvlKeHoachMuaHangItem_get.MaHang;
            //nvlKeHoachMuaHangItem_set.TenHang = nvlKeHoachMuaHangItem_get.TenHang;
            //nvlKeHoachMuaHangItem_set.SoLuong = nvlKeHoachMuaHangItem_get.SoLuong;
            //nvlKeHoachMuaHangItem_set.DonGia = nvlKeHoachMuaHangItem_get.DonGia;
            //nvlKeHoachMuaHangItem_set.DVT = nvlKeHoachMuaHangItem_get.DVT;
            //nvlKeHoachMuaHangItem_set.VAT = nvlKeHoachMuaHangItem_get.VAT;
            //nvlKeHoachMuaHangItem_set.GhiChu = nvlKeHoachMuaHangItem_get.GhiChu;
            //nvlKeHoachMuaHangItem_set.UserInsert = nvlKeHoachMuaHangItem_get.UserInsert;
            //nvlKeHoachMuaHangItem_set.TenLienKet = nvlKeHoachMuaHangItem_get.TenLienKet;
            //nvlKeHoachMuaHangItem_set.MaSP = nvlKeHoachMuaHangItem_get.MaSP;
        }
        class KetquaResult
        {
            public int? Serial { get; set; }

            public string? ketqua { get; set; }
            public double? SLCL { get; set; }

            public string? ketquaexception { get; set; }

        }
        private async Task HuyDatHangAsync()
        {
            if (!PhanQuyenCheck && !ModelAdmin.users.GroupUser.Contains("ketoanmanger") && !ModelAdmin.users.GroupUser.Contains("khovtmanager"))
            {
                ToastService.Notify(new(ToastType.Warning, $"Bạn không có quyền hủy dòng này"));
                return;
            }
            string showtext = "";
            if(nvlKeHoachMuaHangItemShowcrr.SLHuy>0)
            {
                showtext = "Tiếp tục mua hàng?";
            }
            else
            {
                showtext = "Bạn có chắc muốn hủy mua hàng?";
            }
            
            bool bl = await dialogMsg.Show("THÔNG BÁO", showtext);
            if (bl)
            {
                try
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlDonDatHangItem_HuyDatHang_Ver2";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    lstpara.Add(new ParameterDefine("@Serial", nvlKeHoachMuaHangItemShowcrr.Serial));
                    lstpara.Add(new ParameterDefine("@UserHuy", ModelAdmin.users.UsersName));


                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<KetquaResult>>(json);
                        if (query[0].ketqua.Contains("thành công"))
                        {


                            if (query[0].ketqua == "Hủy thành công")
                            {
                                ToastService.Notify(new(ToastType.Success, query[0].ketqua));
                                nvlKeHoachMuaHangItemShowcrr.SLHuy = nvlKeHoachMuaHangItemShowcrr.SLTheoDoi;
                                nvlKeHoachMuaHangItemShowcrr.SLTheoDoi = 0;
                                //var querylst = new System.Collections.ObjectModel.ObservableCollection<NvlKeHoachMuaHangItemShow>(lstdata.Where(p => p.SerialLink != nvlKeHoachMuaHangItemShow.SerialLink));

                                // await Grid.SaveChangesAsync();
                            }
                            if (query[0].ketqua == "Khôi phục thành công")
                            {
                                ToastService.Notify(new(ToastType.Success, query[0].ketqua));
                                nvlKeHoachMuaHangItemShowcrr.SLTheoDoi = nvlKeHoachMuaHangItemShowcrr.SLHuy;
                                nvlKeHoachMuaHangItemShowcrr.SLHuy = 0;
                            
                            }
                            await Grid.SaveChangesAsync();
                        }
                        else
                        {
                            ToastService.Notify(new(ToastType.Warning, "Đã có lỗi xảy ra"));
                        }

                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message} Hủy không được"));
                }


            }
        }
        private void GotoMainForm(NVLDonDatHangItemShow nvlKeHoachMuaHangItemShow)
        {
            Setclass(nvlKeHoachMuaHangItemShowcrr, nvlKeHoachMuaHangItemShow);
            Grid.SaveChangesAsync();
        }

    }
}

