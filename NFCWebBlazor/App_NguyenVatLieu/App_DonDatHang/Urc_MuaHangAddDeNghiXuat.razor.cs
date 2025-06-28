using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi;
using NFCWebBlazor.Model;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Urc_MuaHangAddDeNghiXuat
    {
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        [Inject]
        ToastService toastService { get; set; }
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
                if (keHoachMuaHangcrr.NguoiDuyet == ModelAdmin.users.UsersName)
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
                                        declare @DateBegin date=dateAdd(dd,-30,getdate())
                                         IF OBJECT_ID('tempdb..#tmpitem') IS NOT NULL
                                         DROP TABLE #tmpitem
                                          IF OBJECT_ID('tempdb..#tmpdinhmuc') IS NOT NULL
                                         DROP TABLE #tmpdinhmuc

                                        SELECT [Serial],[SerialDN],[MaHang],isnull(STT,0) as STT
                                        ,[SoLuong],SLTheoDoi,[NgayInsert],[UserInsert],KeyGroup
                                         INTO #tmpitem FROM [NvlKeHoachMuaHangItem]
                                         Where SLTheoDoi>0.1 and NgayInsert>@DateBegin

										
                                        select qry.*,hh.TenHang,hh.DVT,isnull(qrytk.SLTon,0) as SLTon,dm.MaSP,dm.MaMau from 
                                                (SELECT  * from #tmpitem) as qry
									                  left join (select MaHang,SLTon from dbo.GetTonKhoEx()) as qrytk
										                on qry.MaHang=qrytk.MaHang
									                  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                                       inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom 
									left join dbo.NvlKeHoachMuaHang_DinhMuc dm on qry.KeyGroup=dm.KeyGroup
									                 order by qry.STT,qry.Serial
                                         DROP TABLE #tmpitem
                                       ", keHoachMuaHangcrr.Serial);


                CallAPI callAPI = new CallAPI();
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<CustomRootItem>(json);
                    if (query != null)
                    {
                        Console.WriteLine("Load xong");


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
                toastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message}"));
            }
            finally
            {
                PanelVisible = false;
                Grid.Reload();

                StateHasChanged();
            }
        }
        class KetquaResult
        {
            public int? Serial { get; set; }

            public string? ketqua { get; set; }
            public double? SLCL { get; set; }

            public string? ketquaexception { get; set; }

        }

        DataTable dtsave;
        private async Task<bool> checklogicAsync()
        {

            if (dtsave == null)
            {
                CallAPI callAPI = new CallAPI();
                string sql = @"use NVLDB declare @dt Type_NvlKeHoachMuaHangItemVer3
                insert into @dt(STT)
                values(1)
                select * from @dt";
                List<ParameterDefine> lstparadt = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparadt);
                if (json == "")
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi load tablesave"));
                    return false;
                }
                dtsave = JsonConvert.DeserializeObject<DataTable>(json);
            }
            dtsave.Clear();

            try
            {
                var query =lstdata.Where(p=>p.chk == true).ToList();
                foreach (var nvlkhmhitem in query)
                {
                    DataRow rownew = dtsave.NewRow();
                    rownew["Serial"] = nvlkhmhitem.Serial;
                    rownew["DonGia"] = (nvlkhmhitem.DonGia == null) ? 0 : nvlkhmhitem.DonGia;
                    rownew["MaHang"] = nvlkhmhitem.MaHang;
                    rownew["SoLuong"] = nvlkhmhitem.SoLuong;
                    if (String.IsNullOrEmpty(nvlkhmhitem.MaSP))
                        rownew["MaSP"] = DBNull.Value;
                    else
                        rownew["MaSP"] = nvlkhmhitem.MaSP;
                    rownew["SLQuyDoiSP"] = DBNull.Value;
                    rownew["GhiChu"] = "Thêm từ đề nghị xuất kho "+nvlkhmhitem.SerialDN;
                   
                        
                   
                        rownew["SerialLink"] = nvlkhmhitem.Serial;
                        rownew["TableName"] = "NvlKeHoachMuaHangItem";
                    
                    if (String.IsNullOrEmpty(nvlkhmhitem.TenLienKet))
                        rownew["TenLienKet"] = DBNull.Value;
                    else
                        rownew["TenLienKet"] = nvlkhmhitem.TenLienKet;
                    rownew["NgayInsert"] = DBNull.Value;
                    rownew["NgayEdit"] = DBNull.Value;
                    dtsave.Rows.Add(rownew);
                }

            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                //Console.WriteLine("Lỗi: " + ex.Message);
                return false;
            }
            //Kiểm tra mã trùng
            return true;
        }
        private void reset()
        {
            lstdata.Clear();
            keHoachMuaHangcrr.lstitem = null;
           
            StateHasChanged();
        }
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        private async Task saveAsync()
        {
            if (await checklogicAsync())
            {
                CallAPI callAPI = new CallAPI();

                string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_InsertTableMuaHang";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialDN", keHoachMuaHangcrr.Serial));//Trong procedure đã xử lý
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@Type_NvlKeHoachMuaHangItem", prs.ConvertDataTableToJson(dtsave), "DataTable"));
                try
                {
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
                            toastService.Notify(new(ToastType.Danger, "Lỗi: " + query[0].ketquaexception));


                        }

                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, "Đã xảy ra lỗi."));
                    }


                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex));
                }
                finally
                {

                }

            }
            return;
        }
    }
}

