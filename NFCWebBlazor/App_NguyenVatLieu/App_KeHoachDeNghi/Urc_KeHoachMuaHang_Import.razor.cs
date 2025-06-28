using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using SkiaSharp;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucHangHoaMaster;
using System.Data;
using System.Reflection;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;
using Microsoft.AspNetCore.Components.Forms;
using static NFCWebBlazor.App_ThongTin.Page_HangHoaMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Urc_KeHoachMuaHang_Import
    {

        string[] arrcolumncheck = new string[] { "MaHang", "TenHang", "SoLuong", "DonGia", "GhiChu" };
        int[] arrcolumnwidth = new int[] { 140, 220, 100, 110, 140, 140, 200, 100, 120, 100, 100, 100, 100, 100 };

        List<InitDxGridDataColumn> lstcolumn = new List<InitDxGridDataColumn>();
        ClassProcess prs = new ClassProcess();
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

        private async void GetTable(DataTable dt)
        {
            lstcolumn.Clear();
            dt.Columns.Add("Serial", typeof(int));
            if (lstdata == null)
                lstdata = new List<NvlKeHoachMuaHangItemShow>();
            else
                lstdata.Clear();
            ConvertDataTableToClassList(dt);
            //lstdata = dt;

            //string stt = "STT";
            int j = 1;//Index 0 đã dành cho cột Serial
            foreach (DataColumn cl in dt.Columns)
            {

                lstcolumn.Add(new InitDxGridDataColumn(j, cl.ColumnName, cl.ColumnName, null, arrcolumnwidth[j]));
                j++;
            }

            checklogic();

            await dxPopup.CloseAsync();
            dt.Dispose();
            Grid.Reload();
            //StateHasChanged();
            Grid.AutoFitColumnWidths();

        }
        private void ConvertDataTableToClassList(DataTable dt)
        {
            double d = 0;
            foreach (DataRow row in dt.Rows)
            {
                NvlKeHoachMuaHangItemShow nvlHangHoaShow = new NvlKeHoachMuaHangItemShow();
                nvlHangHoaShow.MaHang = (row["MaHang"] != DBNull.Value) ? row["MaHang"].ToString() : "";

                nvlHangHoaShow.TenHang = (row["TenHang"] != DBNull.Value) ? row["TenHang"].ToString() : "";
                if (row["SoLuong"] == DBNull.Value)
                {
                    nvlHangHoaShow.SoLuong = 0;
                    nvlHangHoaShow.Err = "Vui lòng nhập số lượng";
                }
                else
                {
                    if (!double.TryParse(row["SoLuong"].ToString().Trim(), out d))
                    {
                        nvlHangHoaShow.SoLuong = null;
                        nvlHangHoaShow.Err = "Số lượng phải là số";
                    }
                    else
                        nvlHangHoaShow.SoLuong = d;
                }
                if (row["DonGia"] == DBNull.Value)
                {
                    nvlHangHoaShow.DonGia = null;

                }
                else
                {
                    if (!double.TryParse(row["DonGia"].ToString().Trim(), out d))
                    {
                        nvlHangHoaShow.DonGia = null;
                        nvlHangHoaShow.Err = "Đơn giá phải là số";
                    }
                    else
                        nvlHangHoaShow.DonGia = d;
                }
                //nvlHangHoaShow.DonGia = (row["DonGia"] != DBNull.Value) ? double.Parse(row["DonGia"].ToString()) : 1;
                nvlHangHoaShow.GhiChu = (row["GhiChu"] != DBNull.Value) ? row["GhiChu"].ToString() : "";
                lstdata.Add(nvlHangHoaShow);
            }
        }
        private bool checklogic()
        {
            string[] arrcheckempty = new string[] { "MaHang" };
            string[] arrchecnumberrequired = new string[] { "SoLuong" };
            string[] arrchecnumber = new string[] { "DonGia" };
            double d = 0;
            int i = 1;

            if (lstdata.Count == 0)
                return false;
            //Kiểm tra trùng lặp


            Type type = lstdata[0].GetType();
            PropertyInfo[] properties = type.GetProperties();
            //Duyệt để lấy index xử lý cho nhanh
            List<int> indexarrcheckempty = new List<int>();
            List<int> indexarrchecnumberrequired = new List<int>();
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
                foreach (string s in arrchecnumberrequired)
                {
                    if (propertyName == s)
                    {
                        indexarrchecnumberrequired.Add(index);
                        break;
                    }
                }
                index++;
            }
            //Kiểm tra mã hàng trùng

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
                foreach (int j in indexarrchecnumberrequired)
                {
                    object propertyValue = type.GetProperties()[j].GetValue(row);
                    if (propertyValue != null)
                    {
                        if (!double.TryParse(propertyValue.ToString(), out d))
                            row.Err += string.Format("{0} phải là số, ", type.GetProperties()[j].Name);
                    }
                    else
                    {
                        row.Err += string.Format("{0} phải là số, ", type.GetProperties()[j].Name);
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


                i++;
            }

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
            public string MaHang { get; set; }
            public string ArticleNumber { get; set; }
            public string ketqua { get; set; }
            public string ketquaexception { get; set; }
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
                foreach (var nvlkhmhitem in lstdata)
                {


                    DataRow rownew = dtsave.NewRow();
                    rownew["Serial"] = nvlkhmhitem.Serial;
                    rownew["DonGia"] = (nvlkhmhitem.DonGia == null) ? 0 : nvlkhmhitem.DonGia.Value.ToString().Replace(",", ".");
                    rownew["MaHang"] = nvlkhmhitem.MaHang;
                    rownew["SoLuong"] = (nvlkhmhitem.SoLuong==null)?0:nvlkhmhitem.SoLuong.Value.ToString().Replace(",",".");//Chuyển số thập phân dạn dấu phẩy về dấu chấm theo chuẩn của máy chủ
                    if (String.IsNullOrEmpty(nvlkhmhitem.MaSP))
                        rownew["MaSP"] = DBNull.Value;
                    else
                        rownew["MaSP"] = nvlkhmhitem.MaSP;
                    rownew["SLQuyDoiSP"] = DBNull.Value;
                    rownew["GhiChu"] = nvlkhmhitem.GhiChu;
                    if (nvlkhmhitem.SerialLink == null)
                    {
                        rownew["SerialLink"] = DBNull.Value;
                        rownew["TableName"] = DBNull.Value;
                    }
                    else
                    {
                        rownew["SerialLink"] = nvlkhmhitem.SerialLink;
                        rownew["TableName"] = "NvlKeHoachMuaHangItem";
                    }
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
        class KetquaResult
        {
            public int? Serial { get; set; }
            public double? SLCL { get; set; }
            public string? MaHang { get; set; }
            public string? ketqua { get; set; }

            public string? ketquaexception { get; set; }

        }
        private async Task saveAsync()
        {
            if (await checklogicAsync())
            {
                CallAPI callAPI = new CallAPI();

                string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_InsertTableDeNghi";
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
                            if (keHoachMuaHangcrr.lstitem != null)
                            {
                                foreach(var it in lstdata)
                                {
                                    keHoachMuaHangcrr.lstitem.Add(it);
                                }
                            }
                            reset();
                           

                        }
                        else
                        {
                            string err = "";
                            if (query[0].MaHang != null)
                            {
                                foreach (var it in query)
                                {
                                    foreach (var row in lstdata)
                                    {
                                        if (it.MaHang == row.MaHang)
                                        {
                                            row.Err = it.ketqua;
                                            break;
                                        }
                                    }
                                }
                                toastService.Notify(new ToastMessage(ToastType.Warning, err));

                            }
                            if (query[0].Serial != null)
                            {
                                foreach (var it in query)
                                {
                                    foreach (var row in lstdata)
                                    {
                                        if (it.Serial != null)
                                        {
                                            if (row.SerialLink !=null)
                                            {
                                                if (it.Serial.Value == row.SerialLink)
                                                {
                                                    row.Err = "TỔNG số lượng đề nghị đã vượt quá kế hoạch là " + it.SLCL;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                toastService.Notify(new ToastMessage(ToastType.Warning, err));
                                //grvSanPham.Columns["Err"].Visible = true;
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
