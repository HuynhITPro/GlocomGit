using BlazorBootstrap;
using DevExpress.Blazor;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Data;
using System.Reflection;
using static NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.Page_DuyetGiaMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public partial class Urc_DuyetGia_AddItem
    {
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        [Inject]
        ToastService toastService { get; set; }
        [Inject] IJSRuntime jSRuntime { get; set; }
        bool PhanQuyenCheck = false;
        bool checkloadfirst = false;
        protected override async Task OnInitializedAsync()
        {
            await loadAsync();
            // return base.OnInitializedAsync();
        }
        App_ClassDefine.ClassProcess prs = new ClassProcess();
        private async Task loadAsync()
        {

            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 150;
            heightgrid = string.Format("{0}px", height);
            PhanQuyenCheck = phanQuyenAccess.CheckDelete(nvlDuyetGiaShowcrr.UserInsert, ModelAdmin.users);
            lstnhacungcap = await Model.ModelData.Getlstnhacungcap();
            for (int i = 0; i < 10; i++)
            {
                HeaderText headerText = new HeaderText();
                headerText.FieldName = string.Format("NCC{0}", i);
                headerText.Index = i;
              
                headerText.Visible = true;
                lstheader.Add(headerText);
            }

        }
        protected override void OnAfterRender(bool firstRender)
        {
           
           
            //base.OnAfterRender(firstRender);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                Console.WriteLine(GetType().Name);
                //await xulytennhacungcapAsync();
              
                //Xử lý tên nhà cung cấp xem có chưa
                if (nvlDuyetGiaItemShowcrr == null)
                {
                    AddItem();

                }
                else
                {
                    lstdata.Add(nvlDuyetGiaItemShowcrr);
                }

                //StateHasChanged();
            }
            if (lstnhacungcap != null)
            {
                if (!checkloadfirst)
                {

                    checkloadfirst = true;
                    //checkloadthu();
                    await xulytennhacungcapAsync();
                    StateHasChanged();
                }
            }


        }

        private void checkloadthu()
        {
           
            
         
            //StateHasChanged();
        }
       
        private async Task xulytennhacungcapAsync()
        {
            //if (nvlDuyetGiaShowcrr.lstheaderbinding == null)
            //{
          
            string sql = string.Format(@"Use NVLDB
                          select TenNCC as Text from NvlDuyetGiaItem_Detail
                            where KeyGroupItem in (SELECT KeyGroup FROM [NvlDuyetGiaItem] where SerialLink={0}) group by TenNCC", nvlDuyetGiaShowcrr.Serial);
            CallAPI callAPI = new CallAPI();
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
            if (json != "")
            {
               
                var query = JsonConvert.DeserializeObject<List<HeaderText>>(json);
                if (query != null)
                {
                   
                    if (query.Count > 0)
                    {
                        if (nvlDuyetGiaShowcrr.lstheaderbinding != null)//Bắt buộc vì đảm bảo phải đúng thứ tự, tránh trường hợp lstheaderbinding và lstheadertext cùng fieldName nhưng khác thứ tự
                        {
                            foreach (var item in nvlDuyetGiaShowcrr.lstheaderbinding)
                            {
                                foreach (var itheader in lstheader)
                                {
                                    if (item.FieldName == itheader.FieldName)
                                    {
                                        itheader.Text = item.Text;
                                        itheader.Visible = true;
                                        break;
                                    }
                                }
                            }
                            bool checkfind=false;
                            for (int i = 0; i < query.Count; i++)
                            {
                                checkfind = false;
                                foreach (var itheader in lstheader)
                                {
                                   if(itheader.Text == query[i].Text)
                                    {
                                        itheader.Value = query[i].Value;
                                        itheader.Visible = true;
                                        checkfind = true;
                                        break;
                                    }
                                }
                                if(!checkfind)
                                {
                                    for (int j = 0; j < lstheader.Count; j++)
                                    {
                                        if(lstheader[j].Text == "")
                                        {
                                            lstheader[j].Value = query[i].Value;
                                            lstheader[j].Text = query[i].Text;
                                            lstheader[j].Visible = true;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            //Lưu ý: số lượng nhà cung cấp không được lớn hơn 10
                            for (int i = 0; i < query.Count; i++)
                            {
                               
                                lstheader[i].Text = query[i].Text;
                                lstheader[i].Value = query[i].Value;
                                lstheader[i].Visible = true;
                                //Console.WriteLine(i);
                            }
                        }
                        foreach(var item in lstheader)
                        {
                            item.Visible = checktext(item.Text);
                          
                        }    
                        //nvlDuyetGiaShowcrr.lstheaderbinding = new List<HeaderText>();
                        //nvlDuyetGiaShowcrr.lstheaderbinding.AddRange(query);
                    }

                }
            }
            else
            {
                for (int i = 3; i < lstheader.Count; i++)
                {
                    lstheader[i].Visible = false;
                }
            }
          

        }
        private void AddItem()
        {

            for (int i = 0; i < 10; i++)
            {
                NvlDuyetGiaItemShow nvlDuyetGiaItemShow = new NvlDuyetGiaItemShow();
                lstdata.Add(nvlDuyetGiaItemShow);
            }
            Grid.Reload();
        }
        private void showheadertext()
        {
            int index = 0;
            for (int i = 0; i < lstheader.Count; i++)
            {
                if (!lstheader[i].Visible)
                {
                    index = i; break;
                }
            }

            string fieldname = lstheader[index].FieldName;
            //Grid.BeginUpdate();
            //foreach (DxGridDataColumn dxGridDataColumn in Grid.GetDataColumns())
            //{
            //        if (dxGridDataColumn.FieldName == fieldname)
            //        {
            //            dxGridDataColumn.Visible = true;
            //            break;
            //        }
            //}
            //Grid.EndUpdate();
            lstheader[index].Visible = true;
            StateHasChanged();

        }
        private void hideheadertext()
        {
            int index = 0;
            for (int i = lstheader.Count - 1; i >= 0; i--)
            {
                if (lstheader[i].Visible && string.IsNullOrEmpty(lstheader[i].Text))
                {
                    index = i; break;
                }
            }

            string fieldname = lstheader[index].FieldName;

            lstheader[index].Visible = false;
            StateHasChanged();

        }

        public void ReloadGrid()
        {

            //Grid.MakeRowVisible(Grid.GetVisibleRowCount() - 1);
            Grid.Reload();
        }
        List<HeaderText> lstheader = new List<HeaderText>();


        async void SelectedItemChanged(NvlHangHoaDropDown hangHoaDropDown, NvlDuyetGiaItemShow nvlDuyetGiaItemShow)
        {

            if (hangHoaDropDown == null)
                return;
            nvlDuyetGiaItemShow.DVT = hangHoaDropDown.DVT;
            nvlDuyetGiaItemShow.TenHang = hangHoaDropDown.TenHang;
            nvlDuyetGiaItemShow.DonGia = hangHoaDropDown.DonGia;

            //await loadTonKhoAsync();
            //_ = loaddataAsync();

        }
        private bool checktext(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            if(text.Replace(" ","") == "")
                return false;
            return true;
        }
        DataTable dtsave;
        private async Task<bool> checklogicAsync()
        {
            if (dtsave == null)
            {
                CallAPI callAPI = new CallAPI();
                string sql = @"use NVLDB declare @dt Type_NvlDuyetGiaItem
                insert into @dt(Serial)
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

            foreach (var it in lstheader)
            {
                if (it.Visible && !checktext(it.Text))
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Có ít nhất 1 nhà cung cấp chưa nhập tên"));
                    return false;
                }

            }

            var lstfieldname = lstheader.Where(p => p.Visible).ToList();
            
            
            decimal d = 0;
            var query = lstdata.Where(p => p.MaHang != null).ToList();
            var querycheckmahang = query.GroupBy(p => new { MaHang = p.MaHang }).Select(p => new { MaHang = p.Key.MaHang, count = p.Count() }).Where(p => p.count > 1).ToList();

            foreach (var it in query)
            {
                it.Err = "";
                if (it.SLDuToan == null)
                {
                    it.Err = "Nhập thiếu số lượng";

                }
                foreach (var item in querycheckmahang)
                {
                    if (it.MaHang == item.MaHang)
                    {
                        it.Err += ", Mã hàng này bị trùng";

                    }
                }

            }

            var querycheck = query.Where(p => !string.IsNullOrEmpty(p.Err)).Any();
            if (querycheck)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng kiểm tra lại những dòng tô màu đỏ"));
                StateHasChanged();
                return false;
            }

            querycheckmahang.Clear();
            //Xử lý MaNCC qua dấu - 
            foreach (var it in lstfieldname)
            {
               var queryncc=lstnhacungcap.Where(p=>p.FullName==it.Text|| p.FullName==it.Text.Trim()).FirstOrDefault();
                if (queryncc != null)
                {
                    it.Value = queryncc.Name;
                }
                else
                    it.Value = null;

            }
            int STT = 0;
            string keygroup = "";
            foreach (var it in query)
            {
                keygroup = string.Format("{0}_{1}", nvlDuyetGiaShowcrr.Serial, prs.RandomString(10));
                foreach (var headerText in lstfieldname)
                {
                    PropertyInfo property = typeof(NvlDuyetGiaItemShow).GetProperty(headerText.FieldName);
                    if (property != null)
                    {
                        var value = property.GetValue(it);
                        property.SetValue(it, value, null);
                        if (value != null)
                        {
                            if (decimal.TryParse(value.ToString(), out d))
                            {
                                if (d > 0)
                                {
                                    DataRow rownew = dtsave.NewRow();

                                    //Lưu ý hàm này quan trọng, trong procedure có xử lý
                                    if (nvlDuyetGiaItemShowcrr != null)
                                    {
                                        rownew["Serial"] = nvlDuyetGiaItemShowcrr.Serial;
                                    }
                                    else
                                        rownew["Serial"] = 0;
                                    rownew["SerialLink"] = nvlDuyetGiaShowcrr.Serial;
                                    rownew["MaHang"] = it.MaHang;
                                    rownew["KeyGroup"] = keygroup;
                                    rownew["SLDuToan"] = it.SLDuToan;
                                    if (it.GiaDangMua == null)
                                        rownew["GiaDangMua"] = DBNull.Value;
                                    else
                                        rownew["GiaDangMua"] = it.GiaDangMua;
                                    rownew["TableName"] = "NvlDuyetGiaItem";
                                    rownew["DVT"] = it.DVT;
                                    if (it.DinhMuc != null)
                                    {
                                        rownew["DinhMuc"] = it.DinhMuc;
                                    }

                                    rownew["XuatXu"] = it.XuatXu;
                                    rownew["STT"] = STT;
                                    rownew["GhiChu"] = it.GhiChu;
                                    rownew["TenNCC"] = headerText.Text;
                                    if(headerText.Value != null)
                                        rownew["MaNCC"] = headerText.Value;
                                    else
                                        rownew["MaNCC"] = DBNull.Value;
                                    rownew["UserInsert"] = ModelAdmin.users.UsersName;
                                    rownew["DonGia"] = d;
                                    dtsave.Rows.Add(rownew);
                                    STT++;
                                }
                            }
                        }
                    }

                }
            }
            query.Clear();
            // await  prs.exportexcelAsync(jSRuntime, dtsave, 2, 1, "");
            lstfieldname.Clear();
            return true;
        }
        public class KetquaResult
        {
            public string? MaHang { get; set; }
            public string? ketqua { get; set; }
            public string? ketquaexception { get; set; } = "";

        }
        private async void saveAsyn()
        {
            if (!await checklogicAsync())
            {
                return;
            }
            CallAPI callAPI = new CallAPI();

            string sql = "NVLDB.dbo.NvlDuyetGiaItem_InsertTable";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@SerialLink", nvlDuyetGiaShowcrr.Serial));//Trong procedure đã xử lý

            lstpara.Add(new ParameterDefine("@Type_NvlDuyetGiaItem", prs.ConvertDataTableToJson(dtsave), "DataTable"));
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

                        lstdata.Clear();
                        Grid.Reload();
                    }
                    else
                    {
                        if (query[0].MaHang != null)
                        {
                            foreach (var it in lstdata)
                            {
                                if (query[0].MaHang == it.MaHang)
                                {
                                    it.Err = query[0].ketqua;
                                    break;
                                }
                            }
                        }
                        StateHasChanged();
                        toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception)));
                        Console.WriteLine(string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception));
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
                Console.WriteLine("Lỗi: " + ex.Message);
            }
            finally
            {

            }
        }
    }

}
