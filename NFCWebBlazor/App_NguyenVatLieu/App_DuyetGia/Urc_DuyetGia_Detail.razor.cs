using BlazorBootstrap;
using DevExpress.Blazor;
using DocumentFormat.OpenXml.Office2019.Excel.RichData;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;

using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System.Data;
using System.Reflection;

using static NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.Page_DuyetGiaMaster;
using static NFCWebBlazor.App_ThongTin.Page_NhaCungCapMaster;


namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public partial class Urc_DuyetGia_Detail
    {
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        [Inject]
        ToastService ToastService { get; set; }
        bool PhanQuyenCheck = false;
        bool checkload = false;
        bool CheckDuyet = false;
        DataTable? dtkyduyet;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                PhanQuyenCheck = phanQuyenAccess.CheckDelete(nvlDuyetGiaShowcrr.UserInsert, ModelAdmin.users);
                if (nvlDuyetGiaShowcrr.lstitem == null)
                {
                  loadagainAsync();
                }
                else
                {
                    lstdata = nvlDuyetGiaShowcrr.lstitem;
                }
                if(nvlDuyetGiaShowcrr.lstkyduyet!=null)
                {
                    foreach (var it in nvlDuyetGiaShowcrr.lstkyduyet)
                    {
                        if (it.UserDuyet == ModelAdmin.users.UsersName)
                        {
                            CheckDuyet = true;
                          
                            break;
                        }
                    }
                }
                StateHasChanged();
            }
            if (!checkload)
            {
                if (nvlDuyetGiaShowcrr.lstheaderbinding != null)
                {
                    checkload = true;

                    Grid.BeginUpdate();
                    foreach (DxGridDataColumn dxGridDataColumn in Grid.GetDataColumns())
                    {
                        foreach (var it in nvlDuyetGiaShowcrr.lstheaderbinding)
                        {
                            if (dxGridDataColumn.FieldName == it.FieldName)
                            {
                                dxGridDataColumn.Caption = it.Text;
                                dxGridDataColumn.Visible = true;
                                break;
                            }
                        }
                    }
                    Grid.EndUpdate();
            
                    StateHasChanged();
                }
            }


        }
        private string showtextheader(string fieldname)
        {
            foreach (var it in nvlDuyetGiaShowcrr.lstheaderbinding)
            {
                if (fieldname == it.FieldName)
                {
                    return it.Text;
                }
            }
            return "";
        }
        private async Task loadagainAsync()
        {
           
            try
            {
                PanelVisible = true;
                CallAPI callAPI = new CallAPI();
                string sql = string.Format("use NVLDB exec  dbo.NvlDuyetGiaItem_Pivot @SerialLink={0}", nvlDuyetGiaShowcrr.Serial);
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                //lstpara.Add(new ParameterDefine("@SerialLink", nvlDuyetGiaShowcrr.Serial));
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sql, lstpara);
                if (json != "")
                {
                    CustomRoot querycustom = JsonConvert.DeserializeObject<CustomRoot>(json);
                    if (querycustom == null)
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Đã có lỗi xảy ra"));
                        return;
                    }
                    DataTable? query = querycustom.dtitem;
                    dtkyduyet = querycustom.dtkyduyet;
                    DataTable? dtdamua = querycustom.dtcheckmua;
                    if (query == null)
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Đã có lỗi xảy ra"));
                        return;
                    }
                    if (query.Rows.Count > 0)
                    {

                        if (query.Rows[0]["TenHang"] == DBNull.Value)
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Warning, query.Rows[0]["MaHang"].ToString()));
                        }
                        else
                        {
                            List<NvlDuyetGiaItemShow> lst = ConvertDataTableToClassList(query);

                            if (properties == null)
                            {
                                NvlDuyetGiaItemShow nvlDuyetGiaItemShow = new NvlDuyetGiaItemShow();
                                Type type = nvlDuyetGiaItemShow.GetType();
                                properties = type.GetProperties();
                            }
                            nvlDuyetGiaShowcrr.lstitem = new System.Collections.ObjectModel.ObservableCollection<NvlDuyetGiaItemShow>(lst);
                            if (dtkyduyet != null)
                            {
                                if (dtkyduyet.Rows.Count > 0)
                                {
                                    foreach (var it in nvlDuyetGiaShowcrr.lstitem)
                                    {
                                        foreach (DataRow dataRow in dtkyduyet.Rows)
                                        {
                                            if (dataRow.Field<string>("MaHang") == it.MaHang)
                                            {
                                                it.TenNCC = dataRow.Field<string>("TenNCC");
                                                if (dataRow["DonGia"] != DBNull.Value)
                                                {
                                                    it.DonGia = decimal.Parse(dataRow["DonGia"].ToString());
                                                }
                                                foreach (var bindingitem in nvlDuyetGiaShowcrr.lstheaderbinding)
                                                {
                                                    if (dataRow.Field<string>("TenNCC") == bindingitem.Text)
                                                    {
                                                        string fieldname = string.Format("Duyet{0}", bindingitem.FieldName);
                                                        foreach (var pro in properties)
                                                        {
                                                            if (pro.Name == fieldname)
                                                            {
                                                                pro.SetValue(it, true);
                                                                break;
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //Kiểm tra duyệt

                                }
                            }
                            if(dtdamua!=null)
                            {
                                foreach (DataRow rowdamua in dtdamua.Rows)
                                {
                                    if (rowdamua["TenNCC"]==DBNull.Value)
                                    {
                                        continue;
                                    }
                                    foreach(var it in nvlDuyetGiaShowcrr.lstheaderbinding)
                                    {

                                        if(rowdamua.Field<string>("TenNCC")==it.Text)
                                        {
                                            if (rowdamua["MaNCC"] != DBNull.Value)
                                                it.Value = rowdamua["MaNCC"].ToString();
                                            else
                                                it.Value = "";
                                            it.DaMua = rowdamua.Field<string>("DaMua");

                                            break;
                                        }
                                    }
                                }
                            }
                            lstdata = nvlDuyetGiaShowcrr.lstitem;
                        }

                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
            }
            finally
            {
                showtextload = false;
                PanelVisible = false;
                Grid.Reload();
                StateHasChanged();

                //Grid.Reload();
            }
        }

        private async void refreshdata()
        {
           await loadagainAsync();
           // StateHasChanged();
        }
        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public DataTable? dtitem { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public DataTable? dtkyduyet { get; set; }
            [JsonProperty("Table2")]
            public DataTable? dtcheckmua { get; set; }
        }

        private async Task DeleteItemAsync(NvlDuyetGiaItemShow nvlDuyetGiaItemShow)
        {
            if (!phanQuyenAccess.CheckDelete(nvlDuyetGiaItemShow.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new(ToastType.Warning, $"Bạn không có quyền xóa dòng này do bạn không phải người tạo"));
                return;
            }
           await dxFlyoutchucnang.CloseAsync();
            bool bl = await dialogMsg.Show("THÔNG BÁO", $"Bạn có chắc muốn xóa mã hàng {nvlDuyetGiaItemShow.TenHang}?");
            if (bl)
            {
                try
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlDuyetGiaItem_Delete";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();

                    lstpara.Add(new ParameterDefine("@Serial", nvlDuyetGiaItemShow.Serial));
                    lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));

                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            ToastService.Notify(new(ToastType.Success, $"Xóa thành công"));
                            lstdata.Remove(nvlDuyetGiaItemShow);
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
        private async Task EditItemAsync(NvlDuyetGiaItemShow nvlDuyetGiaItemShow)
        {
            if (!phanQuyenAccess.CheckDelete(nvlDuyetGiaItemShow.UserInsert, ModelAdmin.users))
            {
                ToastService.Notify(new(ToastType.Warning, $"Bạn không có quyền sửa dòng này do bạn không phải người tạo"));
                return;
            }
            if(nvlDuyetGiaItemShow.TenNCC!=null)
            {
                ToastService.Notify(new(ToastType.Warning, $"Bạn không được sửa nữa do dòng này đã được duyệt rồi"));
                return;
            }
            await dxFlyoutchucnang.CloseAsync();
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_DuyetGia_ItemEdit>(0);
                builder.AddAttribute(1, "nvlDuyetGiaItemShowcrr", nvlDuyetGiaItemShow.CopyClass());
                // builder.AddAttribute(2, "LoaiKeHoach", LoaiKeHoach);
            
                builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<string>(this, GotoMainForm));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
         await   dxPopup.showAsync("SỬA CHI TIẾT");
         await   dxPopup.ShowAsync();

        }
        class Ketquaduyet
        {
            public string ketqua { get; set; }
            public string ketquaexception { get; set; }
            public decimal? DonGia { get; set; }
            public string? TenNCC { get; set; }
        }
        private async Task DuyetItemAsync(NvlDuyetGiaItemShow nvlDuyetGiaItemShow, string fieldname)
        {
            if (nvlDuyetGiaShowcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in nvlDuyetGiaShowcrr.lstkyduyet)
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
                string sql = "NVLDB.dbo.NvlKyDuyetItem_GiaInsert";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialLinkMaster", nvlDuyetGiaShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@TenNCC", nvlDuyetGiaItemShow.TenNCC));
                lstpara.Add(new ParameterDefine("@SerialLinkItem", nvlDuyetGiaItemShow.Serial));
                lstpara.Add(new ParameterDefine("@TableName", "NvlDuyetGia"));
                lstpara.Add(new ParameterDefine("@UserDuyet", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@GhiChu", ""));
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketquaduyet>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        string fieldnameduyet = string.Format("Duyet{0}",fieldname);//Vì trong class có thuộc tính DuyetNCC này
                     
                        Type type = nvlDuyetGiaItemShow.GetType();
                        properties = type.GetProperties();

                        foreach (PropertyInfo property in properties)
                        {
                            string propertyName = property.Name; // Tên của thuộc tính

                            if (propertyName.Contains("DuyetNCC"))
                            {
                                property.SetValue(nvlDuyetGiaItemShow, false);
                            }
                            if (propertyName == fieldnameduyet)
                            {
                                property.SetValue(nvlDuyetGiaItemShow, true);

                            }
                           
                        }
                        nvlDuyetGiaItemShow.DonGia = query[0].DonGia;
                        nvlDuyetGiaItemShow.TenNCC = query[0].TenNCC;
                        
                        nvlDuyetGiaItemShow.TinhTrangDuyet = null;
                        nvlDuyetGiaItemShow.MsgKhongDuyet = null;
                        if (LoaiDuyet == "Duyệt")
                            nvlDuyetGiaShowcrr.CountDuyet += 1;
                        await Grid.SaveChangesAsync();
                    }
                    else
                    {
                        nvlDuyetGiaItemShow.TenNCC = null;
                        nvlDuyetGiaItemShow.DonGia = null;
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
        private async void DuyetItem(string fieldname, NvlDuyetGiaItemShow nvlDuyetGiaItemShow)
        {
            if(!CheckDuyet)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Bạn không có quyền duyệt"));
                return;
            }
            string nhacungcap = "";
            foreach (var it in nvlDuyetGiaShowcrr.lstheaderbinding)
            {
                if (it.FieldName == fieldname)
                {
                    nhacungcap = it.Text;
                    break;
                }
            }
            nvlDuyetGiaItemShow.TenNCC = nhacungcap;
            if (nvlDuyetGiaItemShow.TenNCC != "")
            {
                await DuyetItemAsync(nvlDuyetGiaItemShow, fieldname);
            }
            Console.WriteLine(nhacungcap);
        }
        private async Task DuyetItemAllAsync()
        {
            if (nvlDuyetGiaShowcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in nvlDuyetGiaShowcrr.lstkyduyet)
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
                lstpara.Add(new ParameterDefine("@SerialLinkMaster", nvlDuyetGiaShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@SerialLinkItem", 0));
                lstpara.Add(new ParameterDefine("@TableName", "NvlDuyetGia"));
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
                            nvlDuyetGiaShowcrr.CountDuyet = nvlDuyetGiaShowcrr.CountTong;
                            foreach (var it in nvlDuyetGiaShowcrr.lstitem)
                            {
                                it.TextDuyet = ModelAdmin.users.TenUser;
                            }
                            // nvlKeHoachMuaHangItemShow.TextDuyet = ModelAdmin.users.TenUser;
                        }
                        else
                        {
                            foreach (var it in nvlDuyetGiaShowcrr.lstitem)
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
        private async Task HuyDuyetItemAsync(NvlDuyetGiaItemShow nvlDuyetGiaItemShow)
        {
            if (nvlDuyetGiaShowcrr.lstkyduyet == null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn chưa được phân quyền duyệt ở đề nghị này"));
                return;
            }
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in nvlDuyetGiaShowcrr.lstkyduyet)
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
                string sql = "NVLDB.dbo.NvlKyDuyetItemGia_Delete";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialLinkMaster", nvlDuyetGiaShowcrr.Serial));
                lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@TableName", "NvlDuyetGia"));
                lstpara.Add(new ParameterDefine("@SerialLinkItem", nvlDuyetGiaItemShow.Serial));

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        //ToastService.Notify(new(ToastType.Success, $"Duyệt thành công"));
                        nvlDuyetGiaItemShow.TenNCC = null;
                        nvlDuyetGiaItemShow.DonGia = 0;
                        foreach (PropertyInfo property in properties)
                        {
                            string propertyName = property.Name;
                            if(propertyName.StartsWith("DuyetNCC"))
                            {
                                property.SetValue(nvlDuyetGiaItemShow, false);
                            }
                        }
                        if(LoaiDuyet=="Duyệt")
                        {
                            nvlDuyetGiaShowcrr.CountDuyet -= 1;
                        }
                      
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

            if (nvlDuyetGiaShowcrr.lstkyduyet != null)
            {
                foreach (var it in nvlDuyetGiaShowcrr.lstkyduyet)
                {
                    if (ModelAdmin.users.UsersName == it.UserDuyet)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        

        private void Setclass(NvlDuyetGiaItemShow nvlKeHoachMuaHangItem_set, NvlDuyetGiaItemShow nvlKeHoachMuaHangItem_get)
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
        private async void GotoMainForm(string checkedit)
        {
            showtextload = true;
            if (checkedit=="close")
            {
               await dxPopup.CloseAsync();
              
                //ToastService.Notify(new ToastMessage(ToastType.Success, "Dữ liệu đã được sửa, Bạn cần tải lại dữ liệu để xem"));
               
                //StateHasChanged();
            }
            //Setclass(nvlDuyetGiaItemShowcrr, nvlKeHoachMuaHangItemShow);
            //Grid.SaveChangesAsync();
        }
        async void SelectedItemChanged(NvlHangHoaDropDown hangHoaDropDown, NvlDuyetGiaItemShow nvlDuyetGiaItemShow)
        {

            if (hangHoaDropDown == null)
                return;
            nvlDuyetGiaItemShow.DVT = hangHoaDropDown.DVT;
            //await loadTonKhoAsync();
            //_ = loaddataAsync();

        }
      
        public async void ShowFlyout(NvlDuyetGiaItemShow nvlDuyetGiaItemShow)
        {
            await dxFlyoutchucnang.CloseAsync();
            nvlDuyetGiaItemShowcrr = nvlDuyetGiaItemShow;
            idflychucnang = "#" + idelement(nvlDuyetGiaItemShowcrr.Serial);
         
          
            //IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();

        }
        PropertyInfo[] properties;
        private void SetValueClassWithString(NvlDuyetGiaItemShow nvlDuyetGiaItemShow)
        {
            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
            }
        }
        public List<NvlDuyetGiaItemShow> ConvertDataTableToClassList(DataTable? dataTable)
        {
            string[] checktypeNumber = new string[] { "System.Double", "System.Int", "System.Int32", "System.Int16", "System.UInt32", "System.UInt64", "System.Decimal" };
            List<NvlDuyetGiaItemShow> list = new List<NvlDuyetGiaItemShow>();
            //Chỉnh sửa Lại tên cột để map qua
            string columnslist = dataTable.Rows[0]["ListColumns"].ToString();
            nvlDuyetGiaShowcrr.lstheaderbinding = new List<HeaderText>();

            //Rename tên cột để map
            int index = 0;
            //Tối đa 10 nhà cung cấp thôi
            List<string> columns = new List<string>();
            foreach (DataColumn cl in dataTable.Columns)
            {
                if (columnslist.Contains(string.Format("[{0}]", cl.ColumnName)))
                {
                    HeaderText headerText = new HeaderText();
                    headerText.Text = cl.ColumnName;
                    headerText.Visible = true;
                    headerText.FieldName = string.Format("NCC{0}", index);
                    
                    cl.ColumnName = headerText.FieldName;
                    nvlDuyetGiaShowcrr.lstheaderbinding.Add(headerText);
                    index++;
                }
                columns.Add(cl.ColumnName);
            }
            NvlDuyetGiaItemShow nvlDuyetGiaItemShow = new NvlDuyetGiaItemShow();
            Type type = nvlDuyetGiaItemShow.GetType();
            properties = type.GetProperties();
            //prs.ConvertDataTableToClassList
            foreach (DataRow row in dataTable.Rows)
            {
                NvlDuyetGiaItemShow nvlDuyetGiaItemShowadd = new NvlDuyetGiaItemShow();
                foreach (PropertyInfo property in properties)
                {
                    string propertyName = property.Name; // Tên của thuộc tính

                    foreach (string it in columns)
                    {
                        if (propertyName == it)
                        {
                            if (row[it] == DBNull.Value)
                                property.SetValue(nvlDuyetGiaItemShowadd, null);
                            else
                            {
                                object convertedValue;
                                //Console.WriteLine(string.Format("{0}:{1}", propertyName, property.PropertyType));
                                Type typeproperties = property.PropertyType;
                               
                                while(true)
                                {
                                    if (property.PropertyType == typeof(decimal?))
                                    {
                                        //object value = Convert.ChangeType(row[it], property.PropertyType);
                                        convertedValue = Convert.ToDecimal(row[it]);
                                        break;
                                    }
                                    if (property.PropertyType == typeof(System.Int32))
                                    {
                                        convertedValue = Convert.ToInt32(row[it]);
                                        break;
                                    }
                                    convertedValue = null;
                                    break;
                                }
                                if(convertedValue==null)
                                    convertedValue = row[it];
                                property.SetValue(nvlDuyetGiaItemShowadd, convertedValue);
                            }
                            break;
                        }
                    }
                }
                list.Add(nvlDuyetGiaItemShowadd);

            }

            return list;
        }

        private async void showmsg()
        {
           await dxFlyoutchucnang.CloseAsync();
            NvlMsgManage nvlMsgManage = new NvlMsgManage();
            nvlMsgManage.TableName = "NvlDuyetGiaItem";
            nvlMsgManage.SerialLink = nvlDuyetGiaItemShowcrr.Serial;
            nvlMsgManage.UserInsert = ModelAdmin.users.UsersName;
            renderFragment = builder =>
            {
                builder.OpenComponent<Page_SendMsg>(0);
                builder.AddAttribute(1, "nvlMsgManage", nvlMsgManage);
                builder.AddAttribute(1, "sqlprocedure", "NVLDB.dbo.NvlDuyetGiaItem_KhongDuyet_Insert");
                builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<NvlMsgManage>(this, aftershowmsg));
                builder.CloseComponent();
            };

          await  dxPopup.showAsync("GỬI LỜI NHẮN");
          await  dxPopup.ShowAsync();
        }
        private async void aftershowmsg(NvlMsgManage nvlMsgManage)
        {
           await dxPopup.CloseAsync();
            if (!string.IsNullOrEmpty(nvlMsgManage.MsgWait))
            {
                nvlDuyetGiaItemShowcrr.MsgKhongDuyet = nvlMsgManage.MsgWait;
                if(nvlDuyetGiaItemShowcrr.TenNCC!=null)
                    nvlDuyetGiaShowcrr.CountDuyet -= 1;
                nvlDuyetGiaItemShowcrr.TinhTrangDuyet = "Không duyệt";
                nvlDuyetGiaItemShowcrr.TenNCC = null;
                nvlDuyetGiaItemShowcrr.DonGia = 0;
               
                await Grid.SaveChangesAsync();
            }
        }
        private void addItem()
        {
           
            MenuItem menuItem = new MenuItem();
            menuItem.TextItem = "Thêm vật tư";
            menuItem.NameItem = "createtaoduyetgia";
            menuItem.ComponentName = "NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.Urc_DuyetGia_AddItem";
            menuItem.IconCssClass = "bi bi-cart3";
            if (ModelAdmin.mainLayout != null)
            {
                RenderFragment renderFragment1;
                renderFragment1 = builder =>
                {

                    builder.OpenComponent<Urc_DuyetGia_AddItem>(0);
                    builder.AddAttribute(1, "nvlDuyetGiaShowcrr", nvlDuyetGiaShowcrr.CopyClass());
                    builder.AddAttribute(1, "nvlDuyetGiaItemShowcrr", nvlDuyetGiaItemShowcrr.CopyClass());
                    builder.CloseComponent();
                };
                ModelAdmin.mainLayout.AddDirectRenderfagment(menuItem, renderFragment1);

            }
        }
        private async Task ApDungSetAsync()
        {
            bool checkduyet = false;
            string LoaiDuyet = "";
            foreach (var it in nvlDuyetGiaShowcrr.lstkyduyet)
            {
                if (ModelAdmin.users.UsersName == it.UserDuyet)
                {
                    if (it.LoaiDuyet == "Duyệt")
                    {
                        checkduyet = true;
                        LoaiDuyet = it.LoaiDuyet;

                        break;
                    }
                }
            }
            if (!checkduyet)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không phải người duyệt nên không có quyền sử dụng chức năng này!"));
                return;
            }
            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlDuyetGia_NgayApDung";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Serial", nvlDuyetGiaShowcrr.Serial));

            lstpara.Add(new ParameterDefine("@DateBegin", nvlDuyetGiaShowcrr.NgayApDung));
            lstpara.Add(new ParameterDefine("@DateEnd", nvlDuyetGiaShowcrr.NgayKetThuc));
            string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                if (query[0].ketqua == "OK")
                {
                    //ToastService.Notify(new(ToastType.Success, $"Duyệt thành công"));
                    ToastService.Notify(new ToastMessage(ToastType.Success, "Áp dụng thành công"));

                }
                else
                {
                    ToastService.Notify(new(ToastType.Danger, $"Lỗi: {query[0].ketquaexception} Duyệt không được"));

                }
            }
        }
       List<DataDropDownList> lstnhacungcap { get; set; }
        private async Task ShowNhaCungCapViewAsync(string FullNameNCC)
        {
            if (FullNameNCC != null)
            {
                lstnhacungcap = await Model.ModelData.Getlstnhacungcap();
                var query = lstnhacungcap.Where(x => x.FullName == FullNameNCC).FirstOrDefault();
                string MaNCC = "";
                if (query != null)
                {
                    MaNCC = query.Name;
                }
                if (MaNCC != "")
                {
                    renderFragment = builder =>
                    {
                        builder.OpenComponent<App_ThongTin.Urc_NhaCungCapView>(0);

                        builder.AddAttribute(1, "MaNCC", MaNCC);

                        builder.CloseComponent();
                    };
                   await dxPopup.showAsync("THÔNG TIN NHÀ CUNG CẤP");
                  await  dxPopup.ShowAsync();
                }
                else
                {
                    //< NFCWebBlazor.App_Admin.Urc_FileHoSoGroup serialLink = "@keHoachMuaHang_Show.Serial" tableName = "NvlDuyetGia" lstdata = "keHoachMuaHang_Show.lstfilehoso" GotoMainForm = "@keHoachMuaHang_Show.setlstfilehoso" ></ NFCWebBlazor.App_Admin.Urc_FileHoSoGroup >
                    renderFragment = builder =>
                    {
                        builder.OpenComponent<App_Admin.Urc_FileHoSoGroup>(0);

                        builder.AddAttribute(1, "serialLink", nvlDuyetGiaShowcrr.Serial);
                        builder.AddAttribute(2, "tableName", "NvlDuyetGia");
                        builder.AddAttribute(3, "lstdata", nvlDuyetGiaShowcrr.lstfilehoso);
                        //builder.AddAttribute(4, "GotoMainForm", nvlDuyetGiaShowcrr.setlstfilehoso);
                        builder.CloseComponent();
                    };
                   await dxPopup.showAsync("HỒ SƠ ĐÍNH KÈM");
                  await  dxPopup.ShowAsync();
                }
            }
            

        }
        string nhacungcapcrr = "";
        private async void SuaNhaCungCapAsync(NvlDuyetGiaShow nvlDuyetGiaShow,string nhacungcap)
        {
            if (!await phanQuyenAccess.CreateNhaCungCap(Model.ModelAdmin.users))
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Bạn không có quyền thêm"));
                return;
            }
             nhacungcapcrr=nhacungcap;
            NVLNhaCungCapShow khachHangNVLShow = new NVLNhaCungCapShow();
            khachHangNVLShow.MaNCC = "";
            khachHangNVLShow.TenNCC = nhacungcapcrr;
            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_NhaCungCapSelected>(0);

                builder.AddAttribute(1, "nhaCungCapShowcrr", khachHangNVLShow);
                builder.AddAttribute(2, "GotoMainForm", EventCallback.Factory.Create<NVLNhaCungCapShow>(this, GotoMainForm));

                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
           await dxPopup.showAsync("SỬA NHÀ CUNG CẤP");
           await dxPopup.ShowAsync();
        }
        private async void editMaNCC(NVLNhaCungCapShow nVLNhaCungCapShow, NvlDuyetGiaShow nvlDuyetGiaShow)
        {
           await dxPopup.CloseAsync();
            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlDuyetGiaItem_UpdateMaNCC";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();

            lstpara.Add(new ParameterDefine("@SerialLink", nvlDuyetGiaShow.Serial));
            lstpara.Add(new ParameterDefine("@TenNCCDuyetOld", nhacungcapcrr));
            lstpara.Add(new ParameterDefine("@TenNCCDuyetNew", nVLNhaCungCapShow.TenNCC));
            lstpara.Add(new ParameterDefine("@MaNCCDuyet", nVLNhaCungCapShow.MaNCC));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));

            string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                if (query[0].ketqua == "OK")
                {
                    await loadagainAsync();
                    ToastService.Notify(new(ToastType.Success, $"Tạo liên kết thành công"));
                    StateHasChanged();

                }
                else
                {
                    ToastService.Notify(new(ToastType.Danger, $"{query[0].ketqua}, {query[0].ketquaexception}"));

                }
                //Grid.Data = lstDonDatHangSearchShow;
            }
        }
        private void GotoMainForm(NVLNhaCungCapShow nVLNhaCungCapShow)
        {
            editMaNCC(nVLNhaCungCapShow, nvlDuyetGiaShowcrr);
        }
    }

}
