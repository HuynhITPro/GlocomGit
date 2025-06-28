using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.Model;
using System.Collections.ObjectModel;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang.Page_DonDatHang_Master;


namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Page_DonDatHang_AddItem
    {
        [Inject] ToastService toastService { get; set; }
      bool checkloadfirst = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
               
            }
            if (!checkloadfirst)
            {
                if (lstnhacungcap != null)
                {
                    checkloadfirst = true;
                    nhacungcapselected = lstnhacungcap.FirstOrDefault(p => p.Name.Equals(nVLDonDatHangShowcrr.MaNCC));
                   
                }
            }


        }
        private void AddMaHang()
        {
            if (nhacungcapselected == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Vui lòng chọn nhà cung cấp trước"));
                return;
            }
            renderFragment = builder =>
           {
               builder.OpenComponent<Page_KeHoachChuaDatHang>(0);
               builder.AddAttribute(1, "GotoMainForm", EventCallback.Factory.Create<List<NVLDonDatHangItemShow>>(this, getItemFromKeHoach));
               builder.AddAttribute(2, "LoaiKeHoach", "DeNghiMuaHang");
               builder.CloseComponent();
           };
            dxPopup.showAsync("THÊM MÃ HÀNG");
            dxPopup.ShowAsync();

          
        }
        private void getItemFromKeHoach(List<NVLDonDatHangItemShow> lstitem)
        {
            widthpanel = 50;
            setFullScreen = "Mở rộng";
            classhide = "p-1";

            if (lstdata == null)
                lstdata = new List<NVLDonDatHangItemShow>();
            lstdata.Clear();
            if (lstdatadathang == null)
                lstdatadathang = new List<NVLDonDatHangItemShow>();
            lstdatadathang.Clear();
            lstdata.AddRange(lstitem);

            //foreach (var it in lstitem)
            //{
            //    it.MaNCC = nhacungcapselected.Name;
            //    it.SLDatThem = it.SLDatHang;
            //    lstdata.Add(it);
            //}
            lstitem.Clear();
            dxPopup.CloseAsync();

        }
        private void addLstDonHang()
        {
           
            foreach (var it in SelectedDataItems.Cast<NVLDonDatHangItemShow>())
            {
                if (nhacungcapselected != null)
                {
                    if (it.MaNCC == null)
                        it.MaNCC = nhacungcapselected.Name;
                }
                it.SLDatThem = it.SLConLai;
                lstdatadathang.Add(it.CopyClass());
                lstdata.Remove(it);
            }
            Grid.Reload();
            GridDatHang.Reload();
        }
        private void RemoveLstDonHang()
        {

            foreach (var it in SelectedDataItemsDatHang.Cast<NVLDonDatHangItemShow>())
            {

                lstdata.Add(it.CopyClass());
                lstdatadathang.Remove(it);
            }
            Grid.Reload();
            GridDatHang.Reload();
        }
        private void reset()
        {
            lstdata.Clear();
            lstdatadathang.Clear();
            GridDatHang.Reload();
            StateHasChanged();
        }
        class KetquaResult
        {
            public int? Serial { get; set; }


            public string? ketqua { get; set; }

            public string? ketquaexception { get; set; }

        }
        DataTable dtsave;
        private async Task<bool> checklogicAsync()
        {
            bool bl = true;
            foreach (var it in lstdatadathang)
            {
                it.Err = "";
                if(it.SLDatThem==null)
                {
                    it.Err = "Vui lòng nhập số lượng đặt hàng";
                }
                if (it.SLDatThem <= 0)
                {
                    it.Err = "Số lượng đặt hàng phải lớn hơn 0";
                }
                if (it.DonGia == null || it.DonGia == 0)
                {
                    if (it.Err != "")
                        it.Err += ", Đơn hàng phải có đơn giá";
                    else
                        it.Err = "Đơn hàng phải có đơn giá";
                }
                if (string.IsNullOrEmpty(it.MaNCC))
                {
                    if (it.Err != "")
                        it.Err += ", Nhập thiếu nhà cung cấp";
                    else
                        it.Err = "Nhập thiếu nhà cung cấp";
                }
                if (it.Err != "")
                    bl = false;
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
            return true;
        }
        private async void save()
        {
            if (!await checklogicAsync())
                return;
            ////KhoTP_ContXuatKho_Insert
            string keygroup = "";
            foreach (var it in lstdatadathang)
            {
                DataRow row = dtsave.NewRow();
                keygroup = string.Format("{0}_{1}", nVLDonDatHangShowcrr.Serial, prs.RandomString(10));
                row["SerialMaDH"] = nVLDonDatHangShowcrr.Serial;
                row["MaHang"] = it.MaHang;
                row["SLDatHang"] = it.SLDatThem;
                row["SLTheoDoi"] = it.SLDatThem;
                row["DVT"] = it.DVT;
                row["DonGia"] = it.DonGia;
                row["SerialLink"] = it.SerialDN;
                row["Serial"] = it.Serial;
                row["MaNCC"] = it.MaNCC;
                row["NgayDKNhapKho"] = DBNull.Value;
                row["UserInsert"] = Model.ModelAdmin.users.UsersName;
                row["Group"] = keygroup;
                dtsave.Rows.Add(row);
            }
            string sql = "NVLDB.dbo.NvlDonDatHangItem_InsertTableKeyGroup";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();

            lstpara.Add(new ParameterDefine("@Type_NvlDonDatHangItem", prs.ConvertDataTableToJson(dtsave), "DataTable"));
            lstpara.Add(new ParameterDefine("@SerialMaDH", nVLDonDatHangShowcrr.Serial));
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
                            foreach (var it in query)
                            {
                                foreach (var row in lstdatadathang)
                                {
                                    if (it.Serial.Value == row.Serial)
                                    {
                                        row.Err = it.ketqua;
                                        break;
                                    }
                                }
                            }
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
        private void btxacnhanncc()
        {
            if (SelectedDataItemsDatHang != null)
            {
                string select = "";
                if (nhacungcapselected == null)
                    select = null;
                else
                    select = nhacungcapselected.Name;
                foreach (var it in SelectedDataItemsDatHang.Cast<NVLDonDatHangItemShow>())
                {
                    it.MaNCC = select;

                }
                toastTextInput.Close();
                GridDatHang.SaveChangesAsync();
            }
        }
        private void SelectedItemsChangedDonHang(IReadOnlyList<object> lstobj)
        {
            SelectedDataItemsDatHang = lstobj;
            if (SelectedDataItemsDatHang == null)
                return;
            if (SelectedDataItemsDatHang.Count > 1)
            {
                var query = SelectedDataItemsDatHang.Cast<NVLDonDatHangItemShow>().Where(p => p.MaNCC != null).FirstOrDefault();
                if (query != null)
                    nhacungcapselected = lstnhacungcap.Where(p => p.Name == query.MaNCC).FirstOrDefault();
                toastTextInput.Show();
            }
            else
                toastTextInput.Close();
        }

    }
}
