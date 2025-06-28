using BlazorBootstrap;

using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;

using NFCWebBlazor.Model;

using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;



namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Urc_KeHoachMuaHang_Detail
    {
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        [Inject]
        ToastService ToastService { get; set; }
        bool PhanQuyenCheck = false;
        bool PhanQuyenDuyet = false;
        List<NvlKeHoachMuaHangItemShow> lstgroup = new List<NvlKeHoachMuaHangItemShow>();
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                Console.WriteLine(this.GetType().Name);
                PhanQuyenCheck = phanQuyenAccess.NVLKeHoachMuaHangDelete(keHoachMuaHangcrr.UserInsert, ModelAdmin.users);
               
                VisibleKeHoachMuaHang = (keHoachMuaHangcrr.LoaiKeHoach.ToLower().Contains("muahang"));
                if(keHoachMuaHangcrr.NguoiDuyet==ModelAdmin.users.UsersName)
                    PhanQuyenDuyet = true;
                if (keHoachMuaHangcrr.lstitem == null)
                {
                    await searchAsync();
                }
                else
                {
                    lstgroup.Clear();
                    var querygroup = keHoachMuaHangcrr.lstitem.GroupBy(p => new { serialgroup = p.SerialLink }).Select(p => new { serialink = p.Key.serialgroup }).ToList();
                    foreach (var it in querygroup)
                    {

                        var item = keHoachMuaHangcrr.lstitem.Where(p => p.SerialLink == it.serialink).FirstOrDefault();
                        if (item != null)
                        {
                            lstgroup.Add(item);
                        }

                    }
                    lstdata = keHoachMuaHangcrr.lstitem;
                    Grid.Reload();
                    StateHasChanged();
                }
            }

        }
        private async Task searchAsync()
        {
            PanelVisible = true;
            if (lstdata == null)
                lstdata = new System.Collections.ObjectModel.ObservableCollection<NvlKeHoachMuaHangItemShow>();
            lstdata.Clear();
            string sql = "";
            try
            {
                sql = string.Format(@"Use NVLDB
                        select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT,isnull(qrytk.SLTon,0) as SLTon,hh.MinTK,hh.MaxTK,qrydathang.DonGiaDat from 
                                (SELECT  [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
                                          ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet
										   
                                      FROM [NvlKeHoachMuaHangItem]
                                      Where SerialDN = {0}) as qry
                                        left join (SELECT min([DonGia]) as DonGiaDat,[SerialLink] FROM [NvlDonDatHangItem]
										                                        where DonGia>0 group by SerialLink) as	qrydathang on qry.Serial=qrydathang.SerialLink
									  left join (select MaHang,SLTon from dbo.GetTonKhoEx()) as qrytk
										on qry.MaHang=qrytk.MaHang
									  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                       inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom 
									  left join (select * from dbo.NvlKeHoachSP where SerialDN={0}) khsp on (qry.SerialLink=khsp.STT and qry.TableName='NvlKeHoachSP')
            
									 order by qry.STT,qry.Serial

                        SELECT  [Serial],[SerialLinkMaster],[SerialLinkItem],[TableName],[UserDuyet],[LoaiDuyet],[GhiChu],usr.TenUser as TenUserDuyet,isnull(usr.PathImg,'UserImage/user.png') as PathImg,[NgayInsert]
                        FROM [dbo].[NvlKyDuyetItem]  it inner join DBMaster.dbo.Users usr on it.UserDuyet=usr.UsersName
                        where SerialLinkMaster={0} and TableName='NvlKehoachMuaHang'
                    ", keHoachMuaHangcrr.Serial);


                if (keHoachMuaHangcrr.LoaiKeHoach.ToLower().Contains("muahang"))
                {

                    lstgroup.Clear();
                    sql = string.Format(@"Use NVLDB
                                        declare @SerialDN int={0}
                                         IF OBJECT_ID('tempdb..#tmpitem') IS NOT NULL
                                         DROP TABLE #tmpitem
                                          IF OBJECT_ID('tempdb..#tmpdinhmuc') IS NOT NULL
                                         DROP TABLE #tmpdinhmuc
                                        SELECT [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
                                        ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet,KeyGroup
                                         INTO #tmpitem FROM [NvlKeHoachMuaHangItem]
                                         Where SerialDN = @SerialDN
	                                
                                    SELECT SerialDN as SerialLink,'NvlKehoachMuaHang' as TableName, KeyGroup, MaSP,MaMauKH, sum(SoLuongSP) as SoLuong
                                    INTO #tmpdinhmuc
                                    FROM dbo.NvlKeHoachSP t2
                                    WHERE SerialDN = @SerialDN
                                    GROUP BY SerialDN,KeyGroup, MaSP,MaMauKH

									declare @tblkh table(ID nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(200),KhuVucKH nvarchar(100),MaMauKH nvarchar(100),SoLuongSP float,TenMau nvarchar(100),Color nvarchar(10))
						
                                        
										declare @tblkehoachfinal Table(KeyGroup nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(200),KhuVucKH nvarchar(100),MaMauKH nvarchar(100),SoLuongSP float,TenMau nvarchar(100),Color nvarchar(10))
										insert into @tblkehoachfinal(KeyGroup,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color)
										
									select KeyGroup ,tmpdm.MaSP,sp.TenSP,'',MaMauKH,tmpdm.SoLuong as SoLuongSP,TenMau,Color
										from #tmpdinhmuc tmpdm left join [dbo].[GetMaMau]() db on (tmpdm.MaMauKH=db.MaMau)
										left  join dbo.GetSanPham() sp on tmpdm.MaSP=sp.MaSP
										
									
										

                                        select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT,isnull(qrytk.SLTon,0) as SLTon,hh.MinTK,hh.MaxTK,tblkh.TenSP,'' TenDinhMuc,'' as CongDoan,'' as IDKeHoach,qry.KeyGroup,cast(tblkh.SoLuongSP as int) as SoLuongSP,tblkh.TenMau,tblkh.Color,isnull(tblkh.TenSP,'') as TenLienKet from 
                                                (SELECT  [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
                                                          ,[SoLuong],SLTheoDoi,[DonGia],SLHuy,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,KeyGroup
                                                      FROM [NvlKeHoachMuaHangItem]
                                                      Where SerialDN = @SerialDN) as qry
									                  left join (select MaHang,SLTon from dbo.GetTonKhoEx()) as qrytk on qry.MaHang=qrytk.MaHang
									                  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                                      inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom 
									                
									                  left join @tblkehoachfinal tblkh on qry.KeyGroup=tblkh.KeyGroup
									                 order by qry.STT,qry.Serial


                                        SELECT  [Serial],[SerialLinkMaster],[SerialLinkItem],[TableName],[UserDuyet],[LoaiDuyet],[GhiChu],usr.TenUser as TenUserDuyet,isnull(usr.PathImg,'UserImage/user.png') as PathImg,[NgayInsert]
                                        FROM [dbo].[NvlKyDuyetItem]  it 
										inner join DBMaster.dbo.Users usr on it.UserDuyet=usr.UsersName
                                        where SerialLinkMaster=@SerialDN and TableName='NvlKehoachMuaHang'

                                         DROP TABLE #tmpitem
                                        Drop Table #tmpdinhmuc", keHoachMuaHangcrr.Serial,(SerialDH==null)?"0":SerialDH);

         //           sql = string.Format(@"Use NVLDB

         //                       select khsp.TenSP,qry.*,hh.TenHang,nh.PhanLoai,hh.DVT,0 as SLTon,hh.MinTK,hh.MaxTK,0 as DonGiaDat,khsp.SoLuongSP,khsp.ArticleNumber,mm.Color from 
         //                       (SELECT  [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
         //                                 ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet
										   
         //                             FROM [NvlKeHoachMuaHangItem]
         //                             Where SerialDN = {0}) as qry
                                        
									//  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
         //                              inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom 
									//  left join (select kh.*,tb.TenSP from dbo.NvlKeHoachSP kh
									//  inner join [SP].[DataBase_ScansiaPacific2014].[dbo].SanPham tb on kh.MaSP=tb.MaSP
									//  where SerialDN={0}) 
									//  khsp on (qry.SerialLink=khsp.STT and qry.TableName='NvlKeHoachSP')
									  
									//  left join [SP].[DataBase_ScansiaPacific2014].[dbo].[ArticleNumberProduct] art on khsp.ArticleNumber=art.ArticleNumber
									//left join [SP].[DataBase_ScansiaPacific2014].dbo.MaMau mm on art.MaMau=mm.MaMau
									// order by qry.STT,qry.Serial

         //               SELECT  [Serial],[SerialLinkMaster],[SerialLinkItem],[TableName],[UserDuyet],[LoaiDuyet],[GhiChu],usr.TenUser as TenUserDuyet,isnull(usr.PathImg,'UserImage/user.png') as PathImg,[NgayInsert]
         //               FROM [dbo].[NvlKyDuyetItem]  it inner join DBMaster.dbo.Users usr on it.UserDuyet=usr.UsersName
         //               where SerialLinkMaster={0} and TableName='NvlKehoachMuaHang'
         //           ", keHoachMuaHangcrr.Serial);
                }
                if (keHoachMuaHangcrr.LoaiKeHoach == "DeNghiXuatHangTheoKHSX"||keHoachMuaHangcrr.LoaiKeHoach== "DeNghiXuatHang")
                {
                    sql = string.Format(@"Use NVLDB
                                        declare @SerialDN int={0}
                                         IF OBJECT_ID('tempdb..#tmpitem') IS NOT NULL
                                         DROP TABLE #tmpitem
                                          IF OBJECT_ID('tempdb..#tmpdinhmuc') IS NOT NULL
                                         DROP TABLE #tmpdinhmuc

                                        SELECT [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
                                        ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet,KeyGroup
                                         INTO #tmpitem FROM [NvlKeHoachMuaHangItem]
                                         Where SerialDN = @SerialDN

	                                     SELECT SerialLink,t2.MaSP, TableName, KeyGroup, TenDinhMuc, CongDoan, sum(SoLuong) as SoLuong,
                                        STUFF((
                                            SELECT ';' + CAST(t1.IDKeHoach AS NVARCHAR)
                                            FROM dbo.NvlKeHoachMuaHang_DinhMuc t1
                                            WHERE SerialLink=@SerialDN and  t1.SerialLink = t2.SerialLink 
                                                AND t1.TableName = t2.TableName 
                                                AND t1.KeyGroup = t2.KeyGroup 
                                                AND t1.TenDinhMuc = t2.TenDinhMuc 
                                                AND t1.CongDoan = t2.CongDoan 
                                       FOR XML PATH('')), 1, 1, '') AS IDKeHoach
                                    INTO #tmpdinhmuc
                                    FROM dbo.NvlKeHoachMuaHang_DinhMuc t2
                                    WHERE TableName = 'NvlKehoachMuaHang' AND SerialLink = @SerialDN
                                    GROUP BY SerialLink, TableName, KeyGroup, TenDinhMuc, CongDoan,MaSP

                                        
                                        declare @lstkehoach nvarchar(max)
                                         SELECT @lstkehoach = COALESCE(@lstkehoach + ';', '') + CAST(ID AS NVARCHAR)
                                         from
                                         (select Distinct(IDKeHoach) as ID
                                         FROM #tmpdinhmuc) as qry

									
                                        declare @tblkh table(ID nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(200),KhuVucKH nvarchar(100),MaMauKH nvarchar(100),SoLuongSP float,TenMau nvarchar(100),Color nvarchar(10))
                                        
                                        if(@lstkehoach is not null)
                                        begin           
                                            insert into @tblkh(ID,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color)
                                            exec SP.DataBase_ScansiaPacific2014.dbo.KeHoachSP_NVLListID @lstkehoach=@lstkehoach
                                        end
										declare @tblkehoachfinal Table(ID nvarchar(100),MaSP nvarchar(100),TenSP nvarchar(200),KhuVucKH nvarchar(100),MaMauKH nvarchar(100),SoLuongSP float,TenMau nvarchar(100),Color nvarchar(10))
										insert into @tblkehoachfinal(ID,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color)
										
										select IDKeHoach,MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color
										from
										(
										(select tmpdm.IDKeHoach,tmpdm.MaSP,TenSP,KhuVucKH,MaMauKH,SoLuongSP,TenMau,Color, 
										ROW_NUMBER() OVER (PARTITION BY tmpdm.IDKeHoach ORDER BY tmpdm.IDKeHoach ASC) AS RowNum
										from #tmpdinhmuc tmpdm inner join @tblkh kh on tmpdm.IDKeHoach like '%'+kh.ID+'%')
										) as qry where RowNum=1
										
									
										

                                        select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT,isnull(qrytk.SLTon,0) as SLTon,hh.MinTK,hh.MaxTK,sp.TenSP,khdm.TenDinhMuc,khdm.CongDoan,khdm.IDKeHoach,qry.KeyGroup,cast(khdm.SoLuong as int) as SoLuongSP,tblkh.TenMau,tblkh.Color from 
                                                (SELECT  [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
                                                          ,[SoLuong],SLTheoDoi,[DonGia],SLHuy,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet,KeyGroup
										  
                                                      FROM [NvlKeHoachMuaHangItem]
                                                      Where SerialDN = @SerialDN) as qry
									                  left join (select MaHang,SLTon from dbo.GetTonKhoEx()) as qrytk
										                on qry.MaHang=qrytk.MaHang
									                  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                                       inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom 
									
									                  left join #tmpdinhmuc khdm on qry.KeyGroup=khdm.KeyGroup
									                  left join @tblkehoachfinal tblkh on khdm.IDKeHoach=tblkh.ID
	                                                    left join dbo.GetSanPham() sp on khdm.MaSP=sp.MaSP
									                 order by qry.STT,qry.Serial


                                        SELECT  [Serial],[SerialLinkMaster],[SerialLinkItem],[TableName],[UserDuyet],[LoaiDuyet],[GhiChu],usr.TenUser as TenUserDuyet,isnull(usr.PathImg,'UserImage/user.png') as PathImg,[NgayInsert]
                                        FROM [dbo].[NvlKyDuyetItem]  it 
										inner join DBMaster.dbo.Users usr on it.UserDuyet=usr.UsersName
                                        where SerialLinkMaster=@SerialDN and TableName='NvlKehoachMuaHang'

                                         DROP TABLE #tmpitem
                                        Drop Table #tmpdinhmuc", keHoachMuaHangcrr.Serial);
                }

                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<CustomRootItem>(json);
                    if (query != null)
                    {
                       // Console.WriteLine("Load xong");


                        if (query.lstmuahangitem != null)
                        {
                            var querygroup = query.lstmuahangitem.GroupBy(p => new { serialgroup = p.SerialLink }).Select(p => new { serialink = p.Key.serialgroup }).ToList();
                            foreach (var it in querygroup)
                            {

                                var item = query.lstmuahangitem.Where(p => p.SerialLink == it.serialink).FirstOrDefault();
                                if (item != null)
                                {
                                    lstgroup.Add(item);
                                }

                            }
                            if (keHoachMuaHangcrr.isChanged)
                            {
                                foreach (var it in query.lstmuahangitem)
                                {
                                    it.chk = true;
                                    it.SLDatHang = it.SLTheoDoi;
                                }

                            }


                            //lstdata.Ad
                            //Trường hợp gán kiểu này, sẽ gây ra lỗi ở giao diện nếu lstdata ban đầu đang khác null, gán như vầy sẽ làm cho lstdata trở về null trước, làm sao các context binding bên trong sẽ bị null, gây lỗi
                            lstdata = new System.Collections.ObjectModel.ObservableCollection<NvlKeHoachMuaHangItemShow>(query.lstmuahangitem);
                            //Sẽ sử dụng phương thức để ngắt kết nối onserverable với giao diện trước, sau khi thêm data xong sẽ kết nối lại
                            // Ngắt liên kết giao diện
                            //BindingOperations.DisableCollectionSynchronization(lstdata);
                            //  BindingOperations.DisableCollectionSynchronization(collection);
                            
                            keHoachMuaHangcrr.lstitem = lstdata;

                            if (query.lstkyduyet != null)
                            {

                                foreach (var it in query.lstkyduyet)
                                {
                                    foreach (var item in keHoachMuaHangcrr.lstitem)
                                    {
                                        if (it.SerialLinkItem == item.Serial)
                                        {
                                            if (item.lstduyetitem == null)
                                                item.lstduyetitem = new List<NvlKyDuyetItemShow>();
                                            item.lstduyetitem.Add(it);
                                            // Console.WriteLine(it.Serial);
                                            if (it.LoaiDuyet.Contains("Kiểm"))
                                                item.TextKiem += it.TenUserDuyet + "; ";
                                            else
                                                item.TextDuyet += it.TenUserDuyet + "; ";
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                       
                    }
                    //Grid.GetDataColumns().First(i => i.FieldName == "SerialLink").GroupIndex = 0;
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
                Grid.Reload();

                StateHasChanged();
            }
        }
        private async Task DeleteItemAsync(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow)
        {
            if (!phanQuyenAccess.CheckDelete(keHoachMuaHangcrr.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new(ToastType.Warning, $"Bạn không có quyền xóa dòng này"));
                return;
            }

            bool bl = await dialogMsg.Show("THÔNG BÁO", "Bạn có chắc muốn xóa dòng này?");
            if (bl)
            {
                try
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_Delete";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();

                    lstpara.Add(new ParameterDefine("@Serial", nvlKeHoachMuaHangItemShow.Serial));
                    lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));


                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            ToastService.Notify(new(ToastType.Success, $"Xóa thành công"));
                            lstdata.Remove(nvlKeHoachMuaHangItemShow);
                            await Grid.SaveChangesAsync();
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
        private async Task DeleteItemKeyGroupAsync(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow)
        {
            if (!phanQuyenAccess.NVLKeHoachMuaHangDelete(nvlKeHoachMuaHangItemShow.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new(ToastType.Warning, $"Bạn không có quyền xóa dòng này"));
                return;
            }

            bool bl = await dialogMsg.Show("THÔNG BÁO", string.Format("Bạn có chắc muốn nhóm {0} {1}?",nvlKeHoachMuaHangItemShow.TenDinhMuc,nvlKeHoachMuaHangItemShow.CongDoan));
            if (bl)
            {
                try
                {
                    if(nvlKeHoachMuaHangItemShow.KeyGroup==null)
                    {
                        ToastService.Notify(new(ToastType.Warning, $"Nhóm này không tồn tại"));
                        return;
                    }
                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_DeleteKeyGroup";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();

                    lstpara.Add(new ParameterDefine("@KeyGroup", nvlKeHoachMuaHangItemShow.KeyGroup));
                    lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));


                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            ToastService.Notify(new(ToastType.Success, $"Xóa thành công"));
                            int n = lstdata.Count;
                           for(int i=n-1;i>=0;i--)
                            {
                                if(lstdata[i].KeyGroup==nvlKeHoachMuaHangItemShow.KeyGroup)
                                {
                                    lstdata.RemoveAt(i);
                                }
                            }
                            
                            await Grid.SaveChangesAsync();
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

        private async Task DuyetItemAsync(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow)
        {
            if (keHoachMuaHangcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in keHoachMuaHangcrr.lstkyduyet)
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
                lstpara.Add(new ParameterDefine("@SerialLinkMaster", nvlKeHoachMuaHangItemShow.SerialDN));
                lstpara.Add(new ParameterDefine("@SerialLinkItem", nvlKeHoachMuaHangItemShow.Serial));
                lstpara.Add(new ParameterDefine("@TableName", "NvlKehoachMuaHang"));
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
                            keHoachMuaHangcrr.CountDuyet += 1;
                            nvlKeHoachMuaHangItemShow.TextDuyet = ModelAdmin.users.TenUser;
                        }
                        else
                            nvlKeHoachMuaHangItemShow.TextKiem = ModelAdmin.users.TenUser;
                        await Grid.SaveChangesAsync();

                    }
                    else
                    {
                        ToastService.Notify(new(ToastType.Danger, $"Lỗi: {query[0].ketquaexception} Duyệt không được"));

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
            if (keHoachMuaHangcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in keHoachMuaHangcrr.lstkyduyet)
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
                lstpara.Add(new ParameterDefine("@SerialLinkMaster", keHoachMuaHangcrr.Serial));
                lstpara.Add(new ParameterDefine("@SerialLinkItem", 0));
                lstpara.Add(new ParameterDefine("@TableName", "NvlKehoachMuaHang"));
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
                            keHoachMuaHangcrr.CountDuyet = keHoachMuaHangcrr.CountTong;
                            foreach (var it in keHoachMuaHangcrr.lstitem)
                            {
                                it.TextDuyet = ModelAdmin.users.TenUser;
                            }
                            // nvlKeHoachMuaHangItemShow.TextDuyet = ModelAdmin.users.TenUser;
                        }
                        else
                        {
                            foreach (var it in keHoachMuaHangcrr.lstitem)
                            {
                                it.TextKiem = ModelAdmin.users.TenUser;
                            }
                        }
                        //nvlKeHoachMuaHangItemShow.TextKiem = ModelAdmin.users.TenUser;
                        await Grid.SaveChangesAsync();
                        if (GotoMasterGrid.HasDelegate)
                        {
                            GotoMasterGrid.InvokeAsync();
                        }
                        ToastService.Notify(new ToastMessage(ToastType.Success, "Duyệt thành công"));
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
            if (keHoachMuaHangcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in keHoachMuaHangcrr.lstkyduyet)
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
                lstpara.Add(new ParameterDefine("@SerialLinkMaster", keHoachMuaHangcrr.Serial));
                lstpara.Add(new ParameterDefine("@SerialLinkItem", 0));
                lstpara.Add(new ParameterDefine("@TableName", "NvlKehoachMuaHang"));
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
                            keHoachMuaHangcrr.CountDuyet = keHoachMuaHangcrr.CountTong;
                            foreach (var it in keHoachMuaHangcrr.lstitem)
                            {
                                it.TextDuyet = "";
                            }
                            // nvlKeHoachMuaHangItemShow.TextDuyet = ModelAdmin.users.TenUser;
                        }
                        else
                        {
                            foreach (var it in keHoachMuaHangcrr.lstitem)
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
                        ToastService.Notify(new ToastMessage(ToastType.Primary, "Hủy duyệt thành công"));
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
        private async Task HuyDuyetItemAsync(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow)
        {
            if (keHoachMuaHangcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in keHoachMuaHangcrr.lstkyduyet)
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
                lstpara.Add(new ParameterDefine("@TableName", "NvlKehoachMuaHang"));


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
                            keHoachMuaHangcrr.CountDuyet -= 1;
                        }
                        else
                            nvlKeHoachMuaHangItemShow.TextKiem = "";
                        await Grid.SaveChangesAsync();
                    }
                    else
                    {
                        ToastService.Notify(new(ToastType.Danger, $"{query[0].ketqua},{query[0].ketquaexception}"));

                    }
                    if (GotoMasterGrid.HasDelegate)
                    {
                        GotoMasterGrid.InvokeAsync();
                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message} Hủy không được"));
            }

        }
        public void ReloadGrid()
        {

            Grid.MakeRowVisible(Grid.GetVisibleRowCount() - 1);
            Grid.Reload();
        }

        private bool Visibleduyetall()
        {
            //if (keHoachMuaHangcrr.LoaiKeHoach == "KeHoachSanXuat")//Kế hoạch mua hàng không cần duyệt
            //{
            //    return false;
            //}
            if (keHoachMuaHangcrr.lstkyduyet != null)
            {
                foreach (var it in keHoachMuaHangcrr.lstkyduyet)
                {
                    if (ModelAdmin.users.UsersName == it.UserDuyet)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private async void EditItem(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow)
        {
            nvlKeHoachMuaHangItemShowcrr = nvlKeHoachMuaHangItemShow;


            // PanelVisible = true;
            renderFragment = builder =>
           {
               builder.OpenComponent<Urc_KeHoachMuaHang_AddItem>(0);
               builder.AddAttribute(1, "keHoachMuaHang_ShowCrr", keHoachMuaHangcrr);
               // builder.AddAttribute(2, "LoaiKeHoach", LoaiKeHoach);
               builder.AddAttribute(2, "nvlkhmhitem", nvlKeHoachMuaHangItemShow.CopyClass());
               builder.AddAttribute(3, "visibledetail", false);
               builder.AddAttribute(4, "GotoMainForm", EventCallback.Factory.Create<NvlKeHoachMuaHangItemShow>(this, GotoMainForm));
               //builder.OpenComponent(0, componentType);
               builder.CloseComponent();
           };
           await dxPopup.showAsync("SỬA CHI TIẾT");
          await  dxPopup.ShowAsync();
        }

        private void Setclass(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItem_set, NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItem_get)
        {
            //nvlKeHoachMuaHangItem_set.Serial = nvlKeHoachMuaHangItem_get.Serial;
            nvlKeHoachMuaHangItem_set.SerialDN = nvlKeHoachMuaHangItem_get.SerialDN;
            nvlKeHoachMuaHangItem_set.MaHang = nvlKeHoachMuaHangItem_get.MaHang;
            nvlKeHoachMuaHangItem_set.TenHang = nvlKeHoachMuaHangItem_get.TenHang;
            nvlKeHoachMuaHangItem_set.SoLuong = nvlKeHoachMuaHangItem_get.SoLuong;
            nvlKeHoachMuaHangItem_set.DonGia = nvlKeHoachMuaHangItem_get.DonGia;
            nvlKeHoachMuaHangItem_set.DVT = nvlKeHoachMuaHangItem_get.DVT;
            nvlKeHoachMuaHangItem_set.VAT = nvlKeHoachMuaHangItem_get.VAT;
            nvlKeHoachMuaHangItem_set.GhiChu = nvlKeHoachMuaHangItem_get.GhiChu;
            nvlKeHoachMuaHangItem_set.UserInsert = nvlKeHoachMuaHangItem_get.UserInsert;
            nvlKeHoachMuaHangItem_set.TenLienKet = nvlKeHoachMuaHangItem_get.TenLienKet;
            nvlKeHoachMuaHangItem_set.MaSP = nvlKeHoachMuaHangItem_get.MaSP;
        }
        private void GotoMainForm(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow)
        {
            Setclass(nvlKeHoachMuaHangItemShowcrr, nvlKeHoachMuaHangItemShow);
            Grid.SaveChangesAsync();
            dxPopup.CloseAsync();
        }
       

     

        class KetquaResult
        {
            public int? Serial { get; set; }

            public string? ketqua { get; set; }
            public double? SLCL { get; set; }

            public string? ketquaexception { get; set; }

        }
   
        private bool ShowHuyCapHang()
        {
            if (PhanQuyenCheck || PhanQuyenDuyet)
                return true;
            return false;
        }
        private async Task HuyCapHangAsync(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow)
        {
            if (!PhanQuyenCheck&&!PhanQuyenDuyet&&!ModelAdmin.users.GroupUser.Contains("ketoanmanger") && !ModelAdmin.users.GroupUser.Contains("khovtmanager"))
            {
                ToastService.Notify(new(ToastType.Warning, $"Bạn không có quyền hủy dòng này này"));
                return;
            }

            bool bl = await dialogMsg.Show("THÔNG BÁO", "Bạn có chắc muốn hủy cấp hàng?");
            if (bl)
            {
                try
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_HuyDatHang_Ver2";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    lstpara.Add(new ParameterDefine("@Serial", nvlKeHoachMuaHangItemShow.Serial));
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
                                nvlKeHoachMuaHangItemShow.SLHuy = nvlKeHoachMuaHangItemShow.SLTheoDoi;
                                nvlKeHoachMuaHangItemShow.HuyDatHang = "Hủy";
                                //var querylst = new System.Collections.ObjectModel.ObservableCollection<NvlKeHoachMuaHangItemShow>(lstdata.Where(p => p.SerialLink != nvlKeHoachMuaHangItemShow.SerialLink));
                              
                                // await Grid.SaveChangesAsync();
                            }
                            if (query[0].ketqua == "Khôi phục thành công")
                            {
                                ToastService.Notify(new(ToastType.Success, query[0].ketqua));
                                nvlKeHoachMuaHangItemShow.SLTheoDoi = nvlKeHoachMuaHangItemShow.SLHuy;
                                nvlKeHoachMuaHangItemShow.HuyDatHang ="";
                                //var querylst = new System.Collections.ObjectModel.ObservableCollection<NvlKeHoachMuaHangItemShow>(lstdata.Where(p => p.SerialLink != nvlKeHoachMuaHangItemShow.SerialLink));
                               
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
        public async void ShowFlyout(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow)
        {
            await dxFlyoutchucnang.CloseAsync();
            IsOpenfly = false;

            idflychucnang = "#" + idelement(nvlKeHoachMuaHangItemShow.Serial);
            await loadtonkhohAsync(nvlKeHoachMuaHangItemShow);
         
            IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();

        }
        private async Task loadtonkhohAsync(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow)
        {
            lstdataitem.Clear();
            if (keHoachMuaHangcrr.lsttemtonkho == null)
            {
               
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string dieukien = " ";


                dieukien = " where SerialKHDH=@SerialKHDH";
                lstpara.Add(new ParameterDefine("@SerialKHDH", keHoachMuaHangcrr.Serial));
                string sqlSearch = "";


                //string[] arrshow = new string[] { "MaHang", "TenHang", "SerialCT", "SLNhap", "SLXuat", "DVT", "LyDo", "TenSP", "MaKien", "DauTuan", "SoLo", "TenGN", "SoXe", "GhiChu", "DonGia", "NgayHetHan", "SerialLink", "KhachHang_XuatXu", "UserInsert", "SerialKHDH", "SerialDN", "NguoiDeNghi", "TenLienKet", "NhaMay", "NgayInsert" };
                bool checkshow = false;

                sqlSearch = string.Format(@"use NVLDB

                                declare @SerialDN int={0}
                                declare @MaKhoEx nvarchar(100)=N'K011'

                                declare @tblserialdn table(MaHang nvarchar(100) primary key,SLDeNghi decimal(18,6))
                                insert into @tblserialdn(MaHang,SLDeNghi)
                                select MaHang,sum(SoLuong) as SLDeNghi from NvlKeHoachMuaHangItem where SerialDN=@SerialDN group  by MaHang

                                declare @tbltonkho Table(MaHang nvarchar(100),SLTon decimal(18,6),SerialLink int,Serial int primary key)
                                insert @tbltonkho (MaHang,SLTon,SerialLink,Serial)

                                select qry.MaHang,SLTon,SerialLink, Serial
                                from
                                (select MaHang,sum(SLNhap-SLXuat) as SLTon,SerialLink,min(case when SLNhap>0 then Serial end) as Serial
                                from NvlNhapXuatItem
                                where MaHang  in (select MaHang from @tblserialdn)
                                 and SerialCT in (select Serial from NvlNhapXuat where MaKho<>@MaKhoEx)
                                group by MaHang,SerialLink) as qry where SLTon<>0

                                select tbl.*,hh.TenHang,hh.DVT,item.ViTri,tbldn.SLDeNghi from @tbltonkho tbl
                                inner join dbo.NvlNhapXuatItem item on tbl.Serial=item.Serial
                                inner join dbo.NvlHangHoa hh on tbl.MaHang=hh.MaHang
                                inner join @tblserialdn tbldn on tbl.MaHang=tbldn.MaHang
                                order by ViTri


                     ", nvlKeHoachMuaHangItemShow.SerialDN);
                CallAPI callAPI = new CallAPI();
                try
                {


                 
                    string json = await callAPI.ExcuteQueryEncryptAsync(sqlSearch, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<NvlNhapXuatItemTemTK>>(json);

                        if (query.Any())
                        {
                            keHoachMuaHangcrr.lsttemtonkho = new List<NvlNhapXuatItemTemTK>();
                            keHoachMuaHangcrr.lsttemtonkho.AddRange(query);
                            //var queryit = query.Where(p => p.MaHang == nvlKeHoachMuaHangItemShow.MaHang).ToList();
                            lstdataitem.AddRange(query.Where(p => p.MaHang == nvlKeHoachMuaHangItemShow.MaHang).ToList());
                            //lstdata = keHoachMuaHangcrr.lsttemtonkho;
                        }



                        // await GotoMainForm.InvokeAsync();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                lstdataitem.AddRange(keHoachMuaHangcrr.lsttemtonkho.Where(p => p.MaHang == nvlKeHoachMuaHangItemShow.MaHang).ToList());
            }
            StateHasChanged();

        }

    }

}
