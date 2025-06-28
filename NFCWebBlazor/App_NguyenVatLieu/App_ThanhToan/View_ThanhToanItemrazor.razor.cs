
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using SkiaSharp;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Page_NhapXuat_Master;

namespace NFCWebBlazor.App_NguyenVatLieu.App_ThanhToan
{
    public partial class View_ThanhToanItemrazor
    {
        [Inject] ToastService toastService { get; set; }
        [Inject]PhanQuyenAccess phanQuyenAccess { get; set; }
        protected override async Task OnInitializedAsync()
        {
            _ = loaddinhmucchitietAsync();
         
        }
        
        private async Task loaddinhmucchitietAsync()
        {
            if (nvlThanhToanShowcrr == null)
                return;
            if (lstNhapXuatKhoSearchShow == null)
                lstNhapXuatKhoSearchShow = new List<NvlNhapXuatKhoShow>();
            lstNhapXuatKhoSearchShow.Clear();
            if (nvlThanhToanShowcrr.lstnhapxuat == null)
            {
                nvlThanhToanShowcrr.lstnhapxuat = new List<NvlNhapXuatKhoShow>();
                try
                {
                    string sql = string.Format(@" use NVLDB
                        IF OBJECT_ID('tempdb..#tmp') IS NOT NULL
						DROP TABLE #tmp
                        IF OBJECT_ID('tempdb..#tblnt') IS NOT NULL
						                        DROP TABLE #tblnt
                        declare @SerialTT int={0}
                      declare @tblitem Table(Serial int,SerialCT int,UserInsert nvarchar(100))
                    insert into @tblitem(Serial,SerialCT,UserInsert)
                    SELECT  [Serial],SerialCT,UserInsert
                      FROM [NvlThanhToanItem]
                      where SerialTT=@SerialTT

                     SELECT * INTO #tmp
                        FROM (select * from NvlNhapXuat where Serial in (select SerialCT from @tblitem)) as qry
                       
                    --Vì bảng chất lượng thì định nghĩa chứng từ không kiểm thì các mã hàng bên trong phải không kiểm, nên mới phát sinh ra bảng này, nếu sau này thay đổi cách làm thì bỏ dòng truy vấn bên dưới
	                declare  @tblnxitem as Table(SerialCT int,MaHang nvarchar(100),ThanhTien decimal(18,6)) 
	                insert into @tblnxitem(SerialCT,MaHang,ThanhTien)
	                select SerialCT,MaHang,sum((SLNhap+SLXuat)*DonGia) as ThanhTien from NvlNhapXuatItem nxitem where SerialCT in (select Serial from #tmp)
	                group by SerialCT,MaHang

	                -- danh sách những mã hàng không cần kiểm
	                declare @tblmhknt as Table(MaHang nvarchar(100) primary key)
	                insert into @tblmhknt(MaHang)
	                select MaHang from SP.NguyenVatLieu.dbo.View_MaHangKhongNghiemThu
	
	                 -- chỉ cần 1 mã hàng nào đó trong chứng từ không có trong bảng @tblmhknt này, thì chứng từ đó phải kiểm
	                 declare @tblphaikiem Table(Serial int primary key)
	                 insert into @tblphaikiem(Serial)
	                 SELECT  SerialCT
	                FROM @tblnxitem dt
	                WHERE NOT EXISTS (
                    SELECT MaHang
                    FROM @tblmhknt tbl
                    WHERE dt.MaHang = tbl.MaHang)
	                group by SerialCT

	                --Lấy danh sách những chứng từ không cần kiểm
	                declare @tblkhongcankiem table(Serial int)
	                insert into @tblkhongcankiem(Serial)
	                select DISTINCT dt.Serial from #tmp dt where Serial not in (select Serial from @tblphaikiem)

	                -- Tạo bảng biến để lưu kết quả nghiệm thu từ function
                  create TABLE #tblnt(Serial int primary key, TinhTrang NVARCHAR(100))
                 declare @lstserial nvarchar(max)
                 SELECT @lstserial = COALESCE(@lstserial + ';', '') + CAST(Serial AS NVARCHAR)
                 FROM @tblphaikiem



                     declare @sqlex nvarchar(max)
                     SET @sqlex = 'INSERT INTO #tblnt(Serial,TinhTrang)
                                         SELECT Serial,TinhTrang
                                         FROM OPENQUERY(SP, ''SELECT * FROM NguyenVatLieu.dbo.NvlChungTuNghiemThu(''''' + @lstserial + ''''')'')';
                                     EXEC sp_executesql @sqlex

                                            declare @tbl as Table(SerialCT int,ThanhToan nvarchar(100),NguoiXacNhan nvarchar(100),MaCTTT nvarchar(100))
                                            insert into @tbl(SerialCT,ThanhToan,NguoiXacNhan,MaCTTT)
                                            select SerialCT,case when tt.NguoiXacNhan is not null then N'Đã xác nhận' else N'Chưa xác nhận' end as ThanhToan,tt.NguoiXacNhan,tt.MaCTTT
                                            from NvlThanhToanItem it 
                                            inner join (select * from dbo.NvlThanhToan where Serial=@SerialTT) tt on it.SerialTT=tt.Serial
           
                   
                                                  Select cast(0 as bit) as CheckThanhToan,nx.*,ngn.TenGN, mk.TenKho,usr.TenUser,case when tbl.ThanhToan is null then N'Chưa làm hồ sơ thanh toán' else tbl.ThanhToan end as ThanhToan,tbl.NguoiXacNhan,tbl.MaCTTT,
											
											case when tnlkk.Serial is not null then N'Không kiểm' else
											(case when nt.Serial is null then N'Chưa kiểm' else nt.TinhTrang end) 
											end as TinhTrang
											,ThanhTien,tblitem.Serial as SerialTTItem
                                             FROM (select * from #tmp) nx 
                                             Inner join NvlMaKho mk on nx.MaKho = mk.MaKho inner join View_NoiGN ngn on nx.MaGN=ngn.MaGN 
                                            inner join DBMaster.dbo.Users usr on nx.UserInsert=usr.UsersName
                                            left join @tbl tbl on nx.Serial=tbl.SerialCT
			                                left join #tblnt nt on nt.Serial=tbl.SerialCT
											left join (select SerialCT,sum(ThanhTien) as ThanhTien from @tblnxitem group by SerialCT) as qrynxitem on nx.Serial=qrynxitem.SerialCT
                                            left join @tblkhongcankiem tnlkk on nx.Serial=tnlkk.Serial
											left join @tblitem tblitem on tblitem.SerialCT=nx.Serial

											 Drop Table #tmp
                                             DROP TABLE #tblnt
                    
                    ", nvlThanhToanShowcrr.Serial);

                    CallAPI callAPI = new CallAPI();
                    PanelVisible = true;
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<NvlNhapXuatKhoShow>>(json);
                        lstNhapXuatKhoSearchShow = query;
                        nvlThanhToanShowcrr.lstnhapxuat.AddRange(query);//.lstNhapXuatItemShows.AddRange(query);
                        // await GotoMainForm.InvokeAsync();
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Success, $"Lỗi: " + ex.Message));
                }
                finally
                {
                    PanelVisible = false;

                    StateHasChanged();

                }

            }
            else
            {
                lstNhapXuatKhoSearchShow.AddRange(nvlThanhToanShowcrr.lstnhapxuat);
                StateHasChanged();
            }
            return;
        }

        private async void deleteitem(NvlNhapXuatKhoShow nvlNhapXuatKhoShow)
        {

            if (!phanQuyenAccess.CheckDelete(nvlThanhToanShowcrr.UserInsert, ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không có quyền xóa. Do bạn không phải người tạo"));
                return;
            }
            bool bl = await dialogMsg.Show("THÔNG BÁO", $"Bạn có chắc muốn xóa dòng {nvlNhapXuatKhoShow.Serial}?");
            if (bl)
            {
                string sql = "NVLDB.dbo.NvlThanhToanItem_Delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlNhapXuatKhoShow.SerialTTItem));
                lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));
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
                            lstNhapXuatKhoSearchShow.Remove(nvlNhapXuatKhoShow);
                            nvlThanhToanShowcrr.lstnhapxuat.RemoveAll(p => p.SerialTTItem == nvlNhapXuatKhoShow.SerialTTItem);

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
       

    }
}
