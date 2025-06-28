using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_ThongTin.Page_DinhMucHangHoaMaster;
using System.Data;
using System.Reflection;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Urc_DinhMucHangHoa_ImportExcel_2
    {
        string[] arrcolumncheck = new string[] { "MaHang", "SLQuyDoi", "GhiChu", "ChatLuong" };
        int[] arrcolumnwidth = new int[] { 140, 100, 140, 150, 100, 140, 220, 100, 120, 100, 100, 100, 100, 100 };

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
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

              
                // lstart.Add(new DataDropDownList());

            }
            return base.OnAfterRenderAsync(firstRender);
        }
        private async void GetTable(DataTable dt)
        {
            lstcolumn.Clear();
            dt.Columns.Add("Serial", typeof(int));
            if (lstdata == null)
                lstdata = new List<HangHoaItem>();
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
            int i = 1;
            foreach (DataRow row in dt.Rows)
            {
                HangHoaItem nvlHangHoaShow = new HangHoaItem();
                nvlHangHoaShow.Serial = i;
                nvlHangHoaShow.MaHang = (row["MaHang"] != DBNull.Value) ? row["MaHang"].ToString() : "";
                nvlHangHoaShow.SLQuyDoi = (row["SLQuyDoi"] != DBNull.Value) ? double.Parse(row["SLQuyDoi"].ToString()) : 0;
                nvlHangHoaShow.DinhMucHaoHut = 1;
                nvlHangHoaShow.DinhMucHaoHut = (row["DinhMucHaoHut"] != DBNull.Value) ? double.Parse(row["DinhMucHaoHut"].ToString()) : 1;
                nvlHangHoaShow.GhiChu = (row["GhiChu"] != DBNull.Value) ? row["GhiChu"].ToString() : "";
                nvlHangHoaShow.ChatLuong = (row["ChatLuong"] != DBNull.Value) ? row["ChatLuong"].ToString() : "";
                lstdata.Add(nvlHangHoaShow);
                i++;
            }
        }
        private bool checklogic()
        {
            string[] arrcheckempty = new string[] { "MaHang" };
            string[] arrchecnumber = new string[] { "SLQuyDoi" };
            double d = 0;
            int i = 1;

            if (lstdata.Count == 0)
                return false;
            //Kiểm tra trùng lặp
            var querycheckduplicate = lstdata.GroupBy(p => new { MaHang = p.MaHang })
                .Select(p => new { MaHang = p.Key.MaHang, count = p.Count() }).Where(p => p.count > 1);
            if (querycheckduplicate.Any())
            {

                foreach (var item in lstdata)
                {
                    foreach (var it in querycheckduplicate)
                    {
                        if (it.MaHang == item.MaHang)
                        {
                            item.Err = "Mã hàng  bị trùng lặp.";
                            break;
                        }

                    }
                }

                return false;
            }
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
                //row.Serial = i;
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
            public string MaHang { get; set; }
            public string ArticleNumber { get; set; }
            public string ketqua { get; set; }
            public string ketquaexception { get; set; }
        }
        DataTable dtsave;
        private async Task saveAsync()
        {
            if (!await phanQuyenAccess.CreateDinhMucVatTu(Model.ModelAdmin.users))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền thêm hàng hóa"));

                return;
            }
            if (!checklogic())
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Vui lòng kiểm tra lại thông tin nhập"));
                return;
            }
            CallAPI callAPI = new CallAPI();
            string sql = "";
            string json = "";
            if (dtsave == null)
            {
                sql = @"use NVLDB declare @dt Type_NvlChiTietKhuVuc_Check
                insert into @dt([Serial] ,[MaSP],[SLQuyDoi],[MaHang],[KhuVuc],[DinhMucHaoHut])
                values(1,'',1,'','',0)
                select * from @dt";
                List<ParameterDefine> lstparadt = new List<ParameterDefine>();
                json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparadt);
                if (json == "")
                    return;
                dtsave = JsonConvert.DeserializeObject<DataTable>(json);
                // dtsave.Clear();
            }
            dtsave.Clear();
            int i = 0;
          
                foreach (var it in lstdata)
                {
                    DataRow rownew = dtsave.NewRow();
                    rownew["Serial"] = it.Serial;
                //rownew["ArticleNumber"] = DBNull.Value;
                //if (art.Name == "Dùng chung")
                //    rownew["ArticleNumber"] = DBNull.Value;
                //else
                //    rownew["ArticleNumber"] = art.Name;
                rownew["MaSP"] = hangHoaItemmaster.MaSP;
                rownew["SLQuyDoi"] = it.SLQuyDoi;
                    rownew["MaHang"] = it.MaHang;
                    rownew["KhuVuc"] = hangHoaItemmaster.KhuVuc;
                    rownew["DinhMucHaoHut"] = (it.DinhMucHaoHut == null) ? 1 : it.DinhMucHaoHut;
                    rownew["GhiChu"] = it.GhiChu;
                    rownew["UserInsert"] = Model.ModelAdmin.users.UsersName;
                    rownew["ChatLuong"] = it.ChatLuong;
                    dtsave.Rows.Add(rownew);
                    i++;
                }


            
            callAPI = new CallAPI();
            sql = "NVLDB.dbo.NvlChiTietKhuVucItem_InsertTable";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@type_nvlchitietkhuvuc_check", prs.ConvertDataTableToJson(dtsave), "DataTable"));
            lstpara.Add(new ParameterDefine("@SerialLink", hangHoaItemmaster.Serial));

            try
            {
                json = await callAPI.ProcedureEncryptAsync(sql, lstpara);

                if (json != "")
                {
                    try
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketquatrave>>(json);
                        if (query[0].ketqua == "OK")
                        {

                            toastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                            reset();
                            lstpara.Clear();
                            dtsave.Clear();
                        }
                        else
                        {
                            msgerr = "Đã có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu";
                            if (query[0].MaHang != null)
                            {
                                foreach (var it in query)
                                {
                                    foreach (var itemdata in lstdata)
                                    {
                                        if (it.Serial == itemdata.Serial)
                                        {
                                            if (it.ketqua != "")
                                                itemdata.Err = it.ketqua;
                                            else
                                                itemdata.Err = string.Format("Mã hàng {0} và nhóm Art phía trên đã có trong hệ thống", it.MaHang);
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0},{1} ", query[0].ketqua, query[0].ketquaexception)));
                        }
                    }
                    catch (Exception ex)
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
                    }
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
