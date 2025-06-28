using BlazorBootstrap;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Data;
using System.Reflection;
using static NFCWebBlazor.App_ThongTin.Page_HangHoaMaster;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Urc_HangHoaAddTable
    {
        string[] arrcolumncheck = new string[] { "MaHang", "TenHang", "QuyCach", "MaNhom", "DVT","DVT2","TyLeQD", "ChatLuong", "MinTK", "MaxTK", "GhiChu" };
        int[] arrcolumnwidth = new int[] { 140, 250, 140, 150, 100, 140, 100, 100, 120, 100, 100, 100, 100, 100 };
        public async Task ImportExcelAsync()
        {
            //CallAPI
            renderFragment = builder =>
            {
                builder.OpenComponent<ButtonImportExcel>(0);

                builder.AddAttribute(1, "arrcolumncheck", arrcolumncheck);
                builder.AddAttribute(2, "getdatatble", EventCallback.Factory.Create<DataTable>(this, GetTable));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            //dxPopup.showpage("TẠO ĐỀ NGHỊ", renderFragment);
           await dxPopup.showAsync("Import hàng hóa từ excel");
           await dxPopup.ShowAsync();
        }
        List<InitDxGridDataColumn> lstcolumn = new List<InitDxGridDataColumn>();
        ClassProcess prs = new ClassProcess();
        private async void GetTable(DataTable dt)
        {
            lstcolumn.Clear();
            dt.Columns.Add("Serial", typeof(int));
            if (lstdata == null)
                lstdata = new List<NvlHangHoaShow>();
            else
                lstdata.Clear();
            ConvertDataTableToClassList(dt);
            //lstdata = dt;

            //string stt = "STT";
            int j = 0;
            foreach (DataColumn cl in dt.Columns)
            {

                lstcolumn.Add(new InitDxGridDataColumn(0, cl.ColumnName, cl.ColumnName, null, arrcolumnwidth[j]));

            }

            checklogic();

            await dxPopup.CloseAsync();
            dt.Dispose();
            //StateHasChanged();
            Grid.AutoFitColumnWidths();
            Grid.Reload();
        }
        private void ConvertDataTableToClassList(DataTable dt)
        {
            double d = 0;
            int i = 0;
            decimal d_ = 0;
            foreach (DataRow row in dt.Rows)
            {
                NvlHangHoaShow nvlHangHoaShow = new NvlHangHoaShow();
                nvlHangHoaShow.MaHang = (row["MaHang"] != DBNull.Value) ? row["MaHang"].ToString() : "";
                nvlHangHoaShow.TenHang = (row["TenHang"] != DBNull.Value) ? row["TenHang"].ToString() : "";
                nvlHangHoaShow.DVT = (row["DVT"] != DBNull.Value) ? row["DVT"].ToString() : "";
                nvlHangHoaShow.QuyCach = (row["QuyCach"] != DBNull.Value) ? row["QuyCach"].ToString() : "";
                nvlHangHoaShow.DVT2 = (row["DVT2"] != DBNull.Value) ? row["DVT2"].ToString() : "";
                nvlHangHoaShow.TyLeQD = (row["TyLeQD"] != DBNull.Value) ? (decimal.TryParse(row["TyLeQD"].ToString(),out d_) ? d_ : null) : null;
                nvlHangHoaShow.MaNhom = (row["MaNhom"] != DBNull.Value) ? row["MaNhom"].ToString() : "";
                nvlHangHoaShow.ChatLuong = (row["ChatLuong"] != DBNull.Value) ? row["ChatLuong"].ToString() : "";
                nvlHangHoaShow.MinTK = (row["MinTK"] != DBNull.Value) ? (double.TryParse(row["MinTK"].ToString(), out d) ? d : null) : null;
                nvlHangHoaShow.MaxTK = (row["MaxTK"] != DBNull.Value) ? (double.TryParse(row["MaxTK"].ToString(), out d) ? d : null) : null;
                nvlHangHoaShow.Serial = (row["MaxTK"] != DBNull.Value) ? (int.TryParse(row["MaxTK"].ToString(), out i) ? i : 0) : 0;
                nvlHangHoaShow.GhiChu = (row["GhiChu"] != DBNull.Value) ? row["GhiChu"].ToString() : "";
                lstdata.Add(nvlHangHoaShow);
            }
        }
        private bool checklogic()
        {
            string[] arrcheckempty = new string[] { "MaHang", "TenHang", "DVT", "MaNhom" };
            string[] arrchecnumber = new string[] { "MinTK", "MaxTK" };
            double d = 0;
            int i = 1;

            if (lstdata.Count == 0)
                return false;
            Type type = lstdata[0].GetType();
            PropertyInfo[] properties = type.GetProperties();
            //Duyệt để lấy index xử lý cho nhanh
            List<int> indexarrcheckempty = new List<int>();
            List<int> indexarrchecnumber = new List<int>();
            int index = 0;
            foreach (var property in properties)
            {
                string propertyName = property.Name;
                foreach (string s in arrcheckempty)
                {
                    if (propertyName == s)
                    {
                        indexarrcheckempty.Add(index);
                        break;
                    }
                }
                foreach (string s in arrchecnumber)
                {
                    if (propertyName == s)
                    {
                        indexarrchecnumber.Add(index);
                        break;
                    }
                }
                index++;
            }
            //Kiểm tra mã hàng trùng
            var querycheckdupplicate = lstdata.GroupBy(p => new { MaHang = p.MaHang }).Select(p => new { MaHang = p.Key.MaHang, count = p.Count() }).Where(p => p.count > 1).ToList();

            foreach (var row in lstdata)
            {
                row.Err = "";
                row.Serial = i;
                type = row.GetType();

                //PropertyInfo[] propertiesitem = type.GetProperties();
                //object propertyValue = propertiesitem.GetValue(row);
                foreach (int j in indexarrcheckempty)
                {
                    object propertyValue = type.GetProperties()[j].GetValue(row);
                    if (propertyValue == null)
                    {
                        row.Err += string.Format("{0} không được để trống, ", type.GetProperties()[j].Name);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(propertyValue.ToString().Trim()))
                        {
                            row.Err += string.Format("{0} không được để trống, ", type.GetProperties()[j].Name);
                            break;
                        }
                    }
                }
                foreach (int j in indexarrchecnumber)
                {
                    object propertyValue = type.GetProperties()[j].GetValue(row);
                    if (propertyValue != null)
                    {
                        if (!double.TryParse(propertyValue.ToString(), out d))
                            row.Err += string.Format("{0} phải là số, ", type.GetProperties()[j].Name);
                    }

                }
                foreach (var it in querycheckdupplicate)
                {
                    if (it.MaHang == row.MaHang)
                    {
                        row.Err += string.Format("Mã hàng {0} bị trùng, ", it.MaHang);
                    }
                }

                i++;
            }
            querycheckdupplicate.Clear();
            var query = lstdata.Where(p => !string.IsNullOrEmpty(p.Err)).Count();
            if (query > 0)
            {
                msgerr = string.Format("Có {0} lỗi. Vui lòng kiểm tra lại", query);
                enablesave = false;
            }
            else
            {
                msgerr = "";
                enablesave = true;
            }
            return true;
        }
        private void checkagain()
        {
            checklogic();
        }
        private void reset()
        {
            lstdata.Clear();
            Grid.Reload();
        }
        class Ketquatrave
        {
            public int Serial { get; set; }
            public string ketqua { get; set; }
        }

        private async Task saveAsync()
        {
            CallAPI callAPI = new CallAPI();
            string sql = @"use NVLDB declare @dt Type_NvlHangHoa_Ver2
            insert into @dt(Serial,MaHang,TenHang,MaNhom,DVT)
            values(1,'','','','')
            select * from @dt";
            List<ParameterDefine> lstparadt = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparadt);
            if (json == "")
                return;
            
            DataTable dtsave = new DataTable();
            dtsave= JsonConvert.DeserializeObject<DataTable>(json);
            dtsave.Clear();
            foreach (var it in lstdata)
            {
                DataRow rownew = dtsave.NewRow();
                rownew["Serial"] = it.Serial;
                rownew["MaHang"] = it.MaHang;
                rownew["TenHang"] = it.TenHang;
                rownew["QuyCach"] = it.QuyCach;
                rownew["MaNhom"] = it.MaNhom;
                rownew["DVT"] = it.DVT;
                rownew["DVT2"] = string.IsNullOrEmpty(it.DVT2) ? DBNull.Value : it.DVT2;
                rownew["TyLeQD"] = (it.TyLeQD == null) ? DBNull.Value : it.TyLeQD;
                rownew["ChatLuong"] = it.ChatLuong;
                if (it.MinTK == null)
                    rownew["MinTK"] = DBNull.Value;
                else
                    rownew["MinTK"] = it.MinTK;
                if (it.MaxTK == null)
                    rownew["MaxTK"] = DBNull.Value;
                else
                    rownew["MaxTK"] = it.MaxTK;

                rownew["MaPDOC"] = it.MaHang;
                rownew["TenSPPDoc"] = it.TenHang;
                rownew["GhiChu"] = it.GhiChu;

                dtsave.Rows.Add(rownew);
            }
            
            sql = "NVLDB.dbo.NvlHangHoa_InsertDataTable_Ver2";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Type_NvlHangHoa", prs.ConvertDataTableToJson(dtsave),"DataTable"));
            lstpara.Add(new ParameterDefine("@UserInsert", Model.ModelAdmin.users.UsersName));
            try
            {
                json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketquatrave>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                        reset();
                    }
                    else
                    {
                        foreach (Ketquatrave it in query)
                        {
                            foreach (var itemdata in lstdata)
                            {
                                if (it.Serial == itemdata.Serial)
                                {
                                    itemdata.Err = it.ketqua;
                                    break;
                                }
                            }
                        }
                        var querycheck = lstdata.Where(p => !string.IsNullOrEmpty(p.Err)).Count();
                        if (querycheck > 0)
                        {
                            msgerr = string.Format("Có {0} lỗi. Vui lòng kiểm tra lại", querycheck);
                            enablesave = false;

                        }
                        else
                        {
                            msgerr = "";
                            enablesave = true;
                        }
                        toastService.Notify(new ToastMessage(ToastType.Warning, "Đã có lỗi xảy ra. Vui lòng kiểm tra lại dữ liệu"));
                    }
                    Grid.AutoFitColumnWidths();
                    Grid.Reload();
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
            }



        }
    }
}
