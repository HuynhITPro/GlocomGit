using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;

using System.Data;
using System.Reflection;
using static NFCWebBlazor.App_KeHoach.Page_KeHoachThang_Master;

namespace NFCWebBlazor.App_KeHoach
{
    public partial class Urc_KeHoachThang_Import
    {
        string[] arrcolumncheck = new string[] { "MaSP", "TenSP", "ArticleNumber", "SLSP", "GhiChu" };
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
            dxPopup.showAsync("Import hàng hóa từ excel");
            dxPopup.ShowAsync();
        }

        private async void GetTable(DataTable dt)
        {
            lstcolumn.Clear();
            dt.Columns.Add("Serial", typeof(int));
            if (lstdata == null)
                lstdata = new List<KeHoachThangItem_Show>();
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
            int d = 0;
            int i = 1;
            foreach (DataRow row in dt.Rows)
            {
                KeHoachThangItem_Show nvlHangHoaShow = new KeHoachThangItem_Show();
                nvlHangHoaShow.Serial = i;
                nvlHangHoaShow.MaSP = (row["MaSP"] != DBNull.Value) ? row["MaSP"].ToString() : "";
                nvlHangHoaShow.ArticleNumber = (row["ArticleNumber"] != DBNull.Value) ? row["ArticleNumber"].ToString() : "";
                nvlHangHoaShow.TenSP = (row["TenSP"] != DBNull.Value) ? row["TenSP"].ToString() : "";
                if (row["SLSP"] == DBNull.Value)
                {
                    nvlHangHoaShow.SLSP = 0;
                    nvlHangHoaShow.Err = "Vui lòng nhập số lượng";
                }
                else
                {
                    if (!int.TryParse(row["SLSP"].ToString().Trim(), out d))
                    {
                        nvlHangHoaShow.SLSP = null;
                        nvlHangHoaShow.Err = "Số lượng phải là số";
                    }
                    else
                        nvlHangHoaShow.SLSP = d;
                }
                
                //nvlHangHoaShow.DonGia = (row["DonGia"] != DBNull.Value) ? double.Parse(row["DonGia"].ToString()) : 1;
                nvlHangHoaShow.GhiChu = (row["GhiChu"] != DBNull.Value) ? row["GhiChu"].ToString() : "";
                i++;
                lstdata.Add(nvlHangHoaShow);
            }
        }
        private bool checklogic()
        {
            string[] arrcheckempty = new string[] { "MaSP", "ArticleNumber" };
            string[] arrchecnumberrequired = new string[] { "SLSP" };
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
            enablesave = false;
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
                string sql = @"use NVLDB declare @dt Type_KeHoachThangItem
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

            try
            {
                foreach (var nvlkhmhitem in lstdata)
                {
                    DataRow rownew = dtsave.NewRow();
                    rownew["Serial"] = nvlkhmhitem.Serial;
                    rownew["Serial_KHThang"] = keHoachSP_Showcrr.Serial;
                    rownew["ArticleNumber"] = nvlkhmhitem.ArticleNumber;
                    rownew["MaSP"] = nvlkhmhitem.MaSP;
                    rownew["SLSP"] = nvlkhmhitem.SLSP;
                    rownew["SLTheoDoi"] = nvlkhmhitem.SLSP;
                    rownew["UserInsert"] = ModelAdmin.users.UsersName;
                    rownew["GhiChu"] = nvlkhmhitem.GhiChu;
                    rownew["MaDHMua"] = DBNull.Value;
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
           
            public string? ketqua { get; set; }

            public string? ketquaexception { get; set; }

        }
        private async Task saveAsync()
        {
            if (!enablesave)
                return;
            if (await checklogicAsync())
            {
                enablesave = false;
                CallAPI callAPI = new CallAPI();

                string sql = "NVLDB.dbo.NvlKeHoachThangItem_InsertTable";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial_KHThang", keHoachSP_Showcrr.Serial));//Trong procedure đã xử lý
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@Type_KeHoachThangItem", prs.ConvertDataTableToJson(dtsave), "DataTable"));
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
                            if (keHoachSP_Showcrr.lstKeHoachChiTiet != null)
                            {
                                foreach (var it in lstdata)
                                {
                                    keHoachSP_Showcrr.lstKeHoachChiTiet.Add(it);
                                }
                            }
                            reset();
                        }
                        else
                        {
                            string err = "";
                           
                            if (query[0].Serial != null)
                            {
                                foreach (var it in query)
                                {
                                    foreach (var row in lstdata)
                                    {

                                        if (it.Serial == row.Serial)
                                        {
                                            row.Err = it.ketqua;
                                            break;
                                        }
                                    }

                                }
                                
                                toastService.Notify(new ToastMessage(ToastType.Warning, "Đã có lỗi xảy ra. Vui lòng kiểm tra lại dữ liệu đầu vào"));
                                //grvSanPham.Columns["Err"].Visible = true;
                            }
                            if (query[0].ketquaexception != null)
                            {
                                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0}, {1}",  query[0].ketqua, query[0].ketquaexception)));
                                Console.WriteLine(string.Format("Lỗi: {0}, {1}", query[0].ketqua, query[0].ketquaexception));
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
                    Console.WriteLine("Lỗi: " + ex.Message);
                    enablesave = true;
                }
                finally
                {

                }

            }
            return;
        }

    }

}
