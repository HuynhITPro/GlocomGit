using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

using NFCWebBlazor.Model;

using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;
using NFCWebBlazor.Models;
using static NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Page_DonDatHang_Master;
using Microsoft.JSInterop;
using NFCWebBlazor.App_Admin;
using DevExpress.Blazor;
using NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi;
using NFCWebBlazor.App_ModelClass;
using System.Data;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Page_KeHoachChuaDatHang
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        bool CheckQuyen = false;

        [Parameter]
        public string LoaiKeHoach { get; set; }

        public class DieuKienTimKiem
        {
            public int? SerialDN { get; set; }
            public string? NguoiDN { get; set; }
            public string? MaKho { get; set; }
            public string? TenKho { get; set; }
            public DateTime DateBegin { get; set; } = DateTime.Now.AddMonths(-2);
            public DateTime DateEnd { get; set; } = DateTime.Now;

            public string BoPhanMuaHang { get; set; }
            public string? MaHang { get; set; }
            public string? TrangThai { get; set; } = "Đề nghị chưa hoàn thành";
        }

        private async Task loaddatadropdownAsync()
        {
            try
            {

                List<Users> lstusr = await Model.ModelData.Getlstusers();
                lstdonhang = new List<DataDropDownList>();
                lstnguoidenghi = lstusr.ToList();
                lstnhomvattu = await Model.ModelData.GetDataDropDownListsAsync("NVL_PhanLoai");
                // lsttinhtrang = await Model.ModelData.GetDataDropDownListsAsync("TypeTinhTrangHoanThanh");
                lsttinhtrang = Model.ModelData.GetDataDropDownListsAsync("NvlTrangThaiDeNghi").Result.AsEnumerable();
                await loaddonhangchuaduyetAsync();

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
        private async Task loaddonhangchuaduyetAsync()
        {
            string sql = string.Format(@"use NVLDB
                declare @UserInsert nvarchar(100)=N'{0}'
    
                SELECT  dh.[Serial],dh.[Serial] as [Name],cast (dh.Serial as varchar(10))+' - '+ncc.TenNCC as FullName,dh.MaNCC as ValueTag
     
          FROM [dbo].[NvlDonDatHang] dh
		  inner join dbo.NvlNhaCungCap ncc on dh.MaNCC=ncc.MaNCC
          where  dh.UserInsert=@UserInsert
          and dh.Serial not in (select SerialLinkMaster from NvlKyDuyetItem where TableName=N'NvlDonDatHang' and LoaiDuyet=N'Duyệt')
           "
        , ModelAdmin.users.UsersName);
            CallAPI callAPI = new CallAPI();

            string json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
                lstdonhang.Clear();
                if (query.Any())
                {
                    lstdonhang.AddRange(query);
                    if(nVLDonDatHangShowcrr!=null)
                    {
                        SerialDH = nVLDonDatHangShowcrr.Serial.ToString();
                        toastTextInput.Show();
                        StateHasChanged();
                    }
                    // Grid.AutoFitColumnWidths();
                }

            }
        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (LoaiKeHoach == null)
                {
                    if (ModelAdmin.lstmenuitems != null)
                    {

                        MenuItem query = ModelAdmin.lstmenuitems.FirstOrDefault(p => p.NameItem.Equals(ModelAdmin.pageclickcurrent));
                        if (query != null)
                        {
                            if (query.Tag != null)
                                LoaiKeHoach = query.Tag.ToString();
                        }

                    }
                    if (LoaiKeHoach != null)
                    {
                        if (LoaiKeHoach.Contains("KeHoach"))
                            kehoachvisible = true;

                    }


                }
            }
            return base.OnAfterRenderAsync(firstRender);
        }


        public async void searchAsync()
        {
            string sql = "";
            if (LoaiKeHoach == null)
                LoaiKeHoach = "DeNghiMuaHang";
            string dieukienphanloai = "";

            string dieukien = " ";
            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            if (nhomvattuselected != null)
            {
                if (nhomvattuselected.Count() > 0)
                {

                    foreach (var it in nhomvattuselected)
                    {
                        if (dieukienphanloai == "")
                            dieukienphanloai = string.Format("N'{0}'", it.Name);
                        else
                            dieukienphanloai += string.Format(",N'{0}'", it.Name);
                    }


                    dieukienphanloai = string.Format(" and  MaHang in (select MaHang from NvlHangHoa where MaNhom in (select MaNhom from NvlNhomHang where PhanLoai in ({0}))) ", dieukienphanloai);
                }
            }

            if (!string.IsNullOrEmpty(dieuKienTimKiem.NguoiDN))
            {

                dieukien += " and NguoiDN=@NguoiDN";
                parameterDefineList.Add(new ParameterDefine("@NguoiDN", dieuKienTimKiem.NguoiDN));
            }
            if (!string.IsNullOrEmpty(dieuKienTimKiem.BoPhanMuaHang))
            {


                dieukien += " and BoPhanMuaHang=@BoPhanMuaHang";
                parameterDefineList.Add(new ParameterDefine("@BoPhanMuaHang", dieuKienTimKiem.BoPhanMuaHang));

            }
            if (dieuKienTimKiem.DateBegin != null)
            {
                dieukien += " and NgayDN>=@DateBegin";
                parameterDefineList.Add(new ParameterDefine("@DateBegin", dieuKienTimKiem.DateBegin.ToString("MM/dd/yyyy 00:00")));
            }
            if (dieuKienTimKiem.DateEnd != null)
            {
                dieukien += " and NgayDN<=@DateEnd";
                parameterDefineList.Add(new ParameterDefine("@DateEnd", dieuKienTimKiem.DateEnd.ToString("MM/dd/yyyy 23:59")));
            }
            if (lstdata == null)
                lstdata = new List<KeHoachMuaHang_Show>();
            else
                lstdata.Clear();
            string dieukienserialdn = "";
            if (dieuKienTimKiem.SerialDN != 0 && dieuKienTimKiem.SerialDN != null)
            {
                dieukienserialdn += " and SerialLinkMaster=@SerialDN";
                parameterDefineList.Add(new ParameterDefine("@SerialDN", dieuKienTimKiem.SerialDN));
            }
            string dieukienchuahoanthanh = "";
            if (dieuKienTimKiem.TrangThai == "Đề nghị đã hoàn thành")
            {
                dieukienchuahoanthanh = string.Format(@"  
                    declare @tbltheodoi as Table(SerialDN int Primary Key)
                    insert into @tbltheodoi(SerialDN)
                    select SerialDN from 
					(select SerialDN,max(SLTheoDoi) as SLTheoDoi from [NvlKeHoachMuaHangItem]
					where SerialDN in (select SerialDN from @tblduyet) {0} group by SerialDN) as qry
					where SLTheoDoi<0.1 
					", dieukienphanloai);//Không có trong danh sách những Đề nghị còn dở dang

            }
            else
            {
                dieukienchuahoanthanh = string.Format(@" 
                    declare @tbltheodoi as Table(SerialDN int Primary Key)
                    insert into @tbltheodoi(SerialDN)
                    select SerialDN from [NvlKeHoachMuaHangItem] where SLTheoDoi>0.1 {0}
					and SerialDN in (select SerialDN from @tblduyet)
					group by SerialDN", dieukienphanloai);// " where SLTheoDoi>0.01";
            }
            sql = string.Format(@"
                    Use NVLDB
                  
					 declare @tblduyet as Table(SerialDN int Primary Key,UserDuyet nvarchar(100))
					    insert into @tblduyet(SerialDN,UserDuyet)
					    select SerialLinkMaster,UserDuyet from
				    (SELECT SerialLinkMaster,UserDuyet,ROW_NUMBER() OVER (PARTITION BY SerialLinkMaster ORDER BY Serial DESC) AS RowNum
				      FROM [NvlKyDuyetItem]
				      where LoaiDuyet=N'Duyệt' and TableName=N'NvlKehoachMuaHang' {3}
				      ) as qryduyet where RowNum=1

                        insert into @tblduyet(SerialDN,UserDuyet)
					   SELECT  [Serial],'Admin' as UserDuyet  FROM [NvlKehoachMuaHang] 
					   where LoaiKeHoach='KeHoachSanXuat' and Serial not in (select SerialDN from @tblduyet) 
                        --Các Serial KeHoachSanXuat không cần duyệt

					   {1}
                    --Các Serial được chọn
                     declare @tbltyle as Table(SerialDN int primary key,DaHoanThanh float,Tong float)
                    insert into @tbltyle(SerialDN,DaHoanThanh,Tong)
                    select SerialDN,1.0/count(*)*sum(TyLe) as  DaHoanThanh,count(*) as Tong   from
                    (
					select MaHang,SerialDN, case when SoLuong=0 then 1 else (SoLuong-SLTheoDoi-SLHuy)/SoLuong end as TyLe
					from
					(
                    select MaHang,SerialDN,sum(SLTheoDoi) as SLTheoDoi,sum(SoLuong) as SoLuong,sum(SLHuy) as SLHuy
                    from [NvlKeHoachMuaHangItem] 
                    where SerialDN in (select SerialDN from @tbltheodoi)
                    group by MaHang,SerialDN) as qrytl
					) as qrytheodoi
                    group by SerialDN

                   
                         Select ddh.*,'{2}'+isnull(usr.[PathImg],'UserImage/user.png') as PathImgTao,kv.TenKhuVuc, usr.TenUser as UserYeuCau, tbltl.DaHoanThanh as TyLe,duyet.UserDuyet as NguoiDuyet
                         from 
                         (Select * from NvlKehoachMuaHang  where  LoaiKeHoach like N'%MuaHang%' {0} and Serial in (select SerialDN from @tbltheodoi)) as ddh 
                            left join dbo.NvlKhuVuc kv on ddh.KhuVuc=kv.MaKhuVuc
                            left join @tblduyet duyet on ddh.Serial=duyet.SerialDN
							left join @tbltyle tbltl on ddh.Serial=tbltl.SerialDN
                         --Lưu ý chỗ này là biến truyền vào theo điều kiện kết--
						 left join DBMaster.dbo.Users usr on ddh.UserInsert=usr.UsersName", dieukien, dieukienchuahoanthanh, Model.ModelAdmin.pathurlfilepublic, dieukienserialdn);


            PanelVisible = true;
            try
            {

                CallAPI callAPI = new CallAPI();

                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<KeHoachMuaHang_Show>>(json);
                    if (query.Count > 0)
                    {
                        lstdata.AddRange(query);
                        // Grid.AutoFitColumnWidths();
                    }
                    await JSRuntime.InvokeVoidAsync(CallNameJson.hideCollapse.ToString(), string.Format("#{0}", randomdivhide));
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
        bool expand = false;
        class KetquaResult
        {
            public int? Serial { get; set; }
            public string? ketqua { get; set; }
            public string? ketquaexception { get; set; }
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
            if (LoaiKeHoach == "DeNghiMuaHang")
            {
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
                if (nhomvattuselected != null)
                {
                    string dieukiennhomvattu = "";
                    foreach (var it in nhomvattuselected)
                    {
                        if (dieukiennhomvattu == "")
                        {
                            dieukiennhomvattu = string.Format("N'{0}'", it.Name);
                        }
                        else
                            dieukiennhomvattu += string.Format(",N'{0}'", it.Name);
                    }
                    dieukien += string.Format("  and  MaHang in (select MaHang from NvlHangHoa where MaNhom in (select MaNhom from NvlNhomHang where PhanLoai in ({0}))) ", dieukiennhomvattu);
                }
                string sql = string.Format(@"use NVLDB

                          Select khmuahang.[Serial],khmuahang.[SerialDN],khmuahang.[MaHang],hh.TenHang,hh.DVT,nh.PhanLoai,isnull(tblsp.TenSP,N'') as TenSP
						,khmuahang.[SoLuong],IsNULL(khmuahang.SoLuong-khmuahang.SLTheoDoi,0) as SLTheoDoi,cast(0 as float) as DonGia,khmuahang.[SoLuong]-isnull(khmuahang.SLHuy,0)-isnull(khmuahang.SLTheoDoi,0) as SLDatHang,isnull(khmuahang.SLTheoDoi,0) as SLConLai,khmuahang.[SerialLink],isnull(khmuahang.DonGia,0) as DonGiaDeNghi,isnull(dg.DonGia,0) as DonGiaGanNhat
	                        FROM (select * from NvlKeHoachMuaHangItem {0} 
                            and Serial in (
							SELECT [SerialLinkItem] FROM [NvlKyDuyetItem] where TableName=N'NvlKehoachMuaHang' and LoaiDuyet=N'Duyệt' and SerialLinkMaster in ({1}))
                            ) khmuahang
							 
							inner join dbo.NvlHangHoa hh on khmuahang.MaHang=hh.MaHang
                             inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom  left join dbo.MaHangSanPham()  tblsp on khmuahang.MaHang=tblsp.MaHang
                            left join dbo.DonGiaGanNhat() dg on khmuahang.MaHang=dg.MaHang                    
                               where khmuahang.SLTheoDoi>0.01", dieukien, dieukienserial);
                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NVLDonDatHangItemShow>>(json);
                    if (GotoMainForm.HasDelegate)
                    {
                        await GotoMainForm.InvokeAsync(query);
                    }
                }
            }
            if (LoaiKeHoach == "KeHoachMuaHang" || LoaiKeHoach == "KeHoachSanXuat")//Lưu ý, cái này không cho phép chọn nhiều vì còn dùng serial để Link
            {
                if (lst.Count > 1)
                {
                    toastService.Notify(new ToastMessage(ToastType.Danger, "MỘT ĐƠN HÀNG chỉ cho phép tạo từ MỘT KẾ HOẠCH"));
                    return;
                }
                if (GotoMainFormKeHoach.HasDelegate)
                {
                    if (nhomvattuselected != null)
                    {
                        string dieukiennhomvattu = "";
                        foreach (var it in nhomvattuselected)
                        {
                            if (dieukiennhomvattu == "")
                            {
                                dieukiennhomvattu = string.Format("N'{0}'", it.Name);
                            }
                            else
                                dieukiennhomvattu += string.Format(",N'{0}'", it.Name);
                        }
                        dieukien += string.Format("  MaHang in (select MaHang from NvlHangHoa where MaNhom in (select MaNhom from NvlNhomHang where PhanLoai in ({0}))) ", dieukiennhomvattu);
                        lst[0].dieukiennhomhang = dieukien;
                    }
                    //Giữ lại điều kiện của nhóm vật tư

                    await GotoMainFormKeHoach.InvokeAsync(lst[0]);
                }

            }

        }
        private void btxacnhanncc()
        {

        }
        private async Task btadddonhangAsync()
        {
            dxFlyoutchucnang.CloseAsync();
            NVLDonDatHangShow nVLDonDatHangShow = new NVLDonDatHangShow();
            nVLDonDatHangShow.Serial = 0;
            nVLDonDatHangShow.NgayDatHang = DateTime.Now;
            nVLDonDatHangShow.DVTT = "VNĐ";
            LoaiKeHoach = "DeNghiMuaHang";

            nVLDonDatHangShow.LoaiDonHang = LoaiKeHoach;
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DonDatHang_AddMaster>(0);
                builder.AddAttribute(1, "nVLDonDatHangcrr", nVLDonDatHangShow);
                builder.AddAttribute(2, "CallBackAfterSave", EventCallback.Factory.Create<int>(this, SearchDeNghi));
                builder.CloseComponent();
            };

            dxPopup.showAsync("TẠO ĐƠN HÀNG");
            dxPopup.ShowAsync();
        }
        private async void SearchDeNghi(int Serial)
        {
            await dxPopup.CloseAsync();
            await loaddonhangchuaduyetAsync();
            //SerialDH = Serial.ToString();
            //DataDropDownList dataDropDownList = new DataDropDownList();
            //dataDropDownList.Name = SerialDH;
            //dataDropDownList.FullName = SerialDH;
            //lstdonhang.Add(dataDropDownList);
        }
        DataTable dtsave;
        List<NvlKeHoachMuaHangItemShow> lstsave = new List<NvlKeHoachMuaHangItemShow>();
        private async Task<bool> checklogicAsync()
        {
            bool bl = true;
            lstsave.Clear();
            if (!lstdata.Any())
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Không có dữ liệu vui lòng kiểm tra lại"));
                return false;
            }
            if(string.IsNullOrEmpty(SerialDH))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn đơn hàng muốn lưu"));
                return false;
            }
            var querycheck = lstdata.Where(p => p.isChanged).ToList();
            if (!querycheck.Any())
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn ít nhất 1 đề nghị"));
                return false;
            }
            foreach (var kh in querycheck)
            {
                if (kh.lstitem != null)
                {
                    foreach (var it in kh.lstitem)
                    {
                        it.Err = "";
                        if (it.chk)
                        {
                            if (it.SLDatHang == null || it.SLDatHang == 0)
                            {
                                it.Err = "Vui lòng nhập số lượng đặt hàng";
                            }
                            if (it.DonGia == null || it.DonGia == 0)
                            {
                                if (it.Err != "")
                                    it.Err += ", Đơn hàng phải có đơn giá";
                                else
                                    it.Err = "Đơn hàng phải có đơn giá";
                            }
                            if (string.IsNullOrEmpty(SerialDH))
                            {
                                if (it.Err != "")
                                    it.Err += ", Vui lòng chọn đơn hàng";
                                else
                                    it.Err = "Vui lòng chọn đơn hàng";
                            }
                            if (it.Err != "")
                                bl = false;

                        }
                    }

                }
            }
            if (!bl)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, IconName.ExclamationCircle.ToString(), "Vui lòng kiểm tra lại những dòng tô màu đỏ"));
                return false;
            }
            if (dtsave == null)
            {
                CallAPI callAPI = new CallAPI();
                string sql = @"use NVLDB declare @dt Type_NvlDonDatHangItem
                   
                                            insert into @dt(Serial,SLDatHang,SLTheoDoi,DVT)
                                            values(1,0,0,'')
                                            select * from @dt";
                List<ParameterDefine> lstparadt = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparadt);
                if (json == "")
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi load tablesave"));
                    return false;
                }
                dtsave = JsonConvert.DeserializeObject<DataTable>(json);
                dtsave.Clear();
            }
            else
                dtsave.Clear();

            //Thêm vào danh sách để lưu
            foreach (var kh in querycheck)
            {
                if (kh.lstitem != null)
                {
                    lstsave.AddRange(kh.lstitem.Where(p=>p.chk).ToList());
                }
            }
            return true;
        }
        private void reset()
        {
            lstdata.Clear();
            toastTextInput.Close();
            SerialDH = null;
        }
        private async Task saveAsync()
        {
            if (!await checklogicAsync())
                return;
            ////KhoTP_ContXuatKho_Insert
            string keygroup = "";
            var item=lstdonhang.Where(p=>p.Serial.ToString()==SerialDH).FirstOrDefault();
            string MaNCC = "";
            if (item != null)
                MaNCC = item.ValueTag.ToString();
            foreach (var it in lstsave)
            {
                DataRow row = dtsave.NewRow();
                keygroup = string.Format("{0}_{1}", SerialDH, prs.RandomString(10));
                row["SerialMaDH"] = SerialDH;
                row["MaHang"] = it.MaHang;
                row["SLDatHang"] = it.SLDatHang.Value.ToString().Replace(",",".");
                row["SLTheoDoi"] = it.SLDatHang;
                row["DVT"] = it.DVT;
                row["DonGia"] = it.DonGia;
                row["SerialLink"] = it.SerialDN;
                row["Serial"] = it.Serial;
                row["MaNCC"] = (it.MaNCC==null)?MaNCC:it.MaNCC;
                row["NgayDKNhapKho"] = (it.NgayDKNhapKho==null)?DBNull.Value:it.NgayDKNhapKho.Value.ToString("MM/dd/yyyy");
                row["UserInsert"] = Model.ModelAdmin.users.UsersName;
                row["Group"] = keygroup;
                dtsave.Rows.Add(row);
            }
            string sql = "NVLDB.dbo.NvlDonDatHangItem_InsertTableKeyGroup";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();

            lstpara.Add(new ParameterDefine("@Type_NvlDonDatHangItem", prs.ConvertDataTableToJson(dtsave), "DataTable"));
            lstpara.Add(new ParameterDefine("@SerialMaDH", SerialDH));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
            try
            {
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<KetquaResult>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        //msgBox.Show("Lưu thành công", IconMsg.iconssuccess);
                        toastService.Notify(new(ToastType.Success, $"Lưu thành công."));
                        reset();
                    }
                    else
                    {
                        string err = "";

                        if (query[0].Serial != null)
                        {
                            var querycheck = lstdata.Where(p => p.isChanged).ToList();

                            foreach (var it in querycheck)
                            {
                                
                                if(it.lstitem!=null)
                                {
                                    foreach(var qry in query)
                                    {
                                        foreach (var itemlst in it.lstitem)
                                        {
                                            if (itemlst.Serial == qry.Serial)
                                            {
                                                itemlst.Err = qry.ketqua;
                                                break;
                                            }
                                        }
                                    }
                                   
                                }
                            }
                            //foreach (var it in query)
                            //{
                            //    foreach (var row in lstsave)
                            //    {
                            //        if (it.Serial.Value == row.Serial)
                            //        {
                            //            row.Err = it.ketqua;
                            //            break;
                            //        }
                            //    }
                            //}
                            toastService.Notify(new ToastMessage(ToastType.Warning, err));
                            //grvSanPham.Columns["Err"].Visible = true;
                        }
                        else
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, query[0].ketqua));
                        }
                        if (query[0].ketquaexception != null)
                        {
                            toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketquaexception));
                        }


                    }

                }
                else
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Đã xảy ra lỗi."));
                }


            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                dtsave.Clear();
            }



        }
      
    }
}

