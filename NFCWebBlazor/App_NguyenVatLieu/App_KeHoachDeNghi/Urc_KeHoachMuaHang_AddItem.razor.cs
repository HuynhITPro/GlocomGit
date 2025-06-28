using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Data;
using Microsoft.AspNetCore.Components.Web;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;
using DevExpress.Blazor;
using NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat;

namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Urc_KeHoachMuaHang_AddItem
    {
        [Inject] PreloadService PreloadService { get; set; }

        [Inject] ToastService ToastService { get; set; } = default!;
        protected override Task OnInitializedAsync()
        {
            editContext = new EditContext(nvlkhmhitem);
            Console.WriteLine(this.GetType().Name);
            _ = loadAsync();
            return base.OnInitializedAsync();

        }
        private async Task loadAsync()
        {
            try
            {
                PreloadService.Show();
                if (ModelAdmin.lsthanghoafilter.Any())
                {
                    lstmahang = ModelAdmin.lsthanghoafilter;
                }
                else
                    lstmahang = await Model.ModelData.GetHangHoa();
                if(keHoachMuaHang_ShowCrr.BoPhanMuaHang== "KHO CƠ ĐIỆN MŨI LƯỠI")//Đối với đề nghị cơ điện mũi lưỡi thì xử lý riêng
                {
                    lstTenLienKet=await Model.ModelData.GetMaMay();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load form " + ex.Message);
            }
            finally
            {
                PreloadService.Hide();
                StateHasChanged();
            }
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                if(nvlkhmhitem.Serial>0)
                {
                    EnableEdit = true;
                }
            }
            return base.OnAfterRenderAsync(firstRender);
        }
        private async Task updateAsync()
        {
            if (await checklogicAsync())
            {
                CallAPI callAPI = new CallAPI();

                string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_UpdateVer2";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", nvlkhmhitem.Serial));


                lstpara.Add(new ParameterDefine("@SerialDN", nvlkhmhitem.SerialDN));
                lstpara.Add(new ParameterDefine("@MaHang", nvlkhmhitem.MaHang));
                lstpara.Add(new ParameterDefine("@SoLuong", nvlkhmhitem.SoLuong));

                lstpara.Add(new ParameterDefine("@MaSP", nvlkhmhitem.MaSP));
                lstpara.Add(new ParameterDefine("@DonGia", nvlkhmhitem.DonGia));
                lstpara.Add(new ParameterDefine("@DVT", nvlkhmhitem.DVT));

                lstpara.Add(new ParameterDefine("@VAT", null));
                lstpara.Add(new ParameterDefine("@GhiChu", nvlkhmhitem.GhiChu));

                lstpara.Add(new ParameterDefine("@TenLienKet", nvlkhmhitem.TenLienKet));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));

                try
                {
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<KetquaResult>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            //msgBox.Show("Lưu thành công", IconMsg.iconssuccess);

                            //reset();
                            NvlHangHoaDropDown nvlHangHoaDropDown = lstmahang.FirstOrDefault(x => x.MaHang == nvlkhmhitem.MaHang);
                            if (nvlHangHoaDropDown != null)
                                nvlkhmhitem.TenHang = nvlHangHoaDropDown.TenHang;
                            ToastService.Notify(new ToastMessage(ToastType.Success, $"Sửa thành công"));
                            if(GotoMainForm.HasDelegate)
                            {
                               await GotoMainForm.InvokeAsync(nvlkhmhitem);
                            }

                        }
                        else
                        {
                            if (query[0].ketquaexception != null)
                            {
                                ToastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi: {0},{1}", query[0].ketqua, query[0].ketquaexception)));

                            }
                            else
                                ToastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Lỗi: {0}", query[0].ketqua)));

                        }
                    }
                    else
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Đã xảy ra lỗi.", IconMsg.iconerror)));
                    }
                }
                catch (Exception ex)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Danger, string.Format("Đã xảy ra lỗi." + ex.Message)));
                }
                finally
                {

                }

            }
            return;
        }
        private async Task<bool> checklogicAsync()
        {
            if (dtsave.Columns.Count == 0)
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
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi load tablesave"));
                    return false;
                }
                dtsave = JsonConvert.DeserializeObject<DataTable>(json);
            }
            dtsave.Clear();
            dtsave.Columns["SoLuong"].DataType=typeof(string);
            await txtMaHang.FocusAsync();
            try
            {
                bool b = false;
                b= editContext.Validate();
                if (!b)
                    return b;

                DataRow rownew = dtsave.NewRow();

                rownew["DonGia"] = (nvlkhmhitem.DonGia == null) ? 0 : nvlkhmhitem.DonGia;
                rownew["MaHang"] = nvlkhmhitem.MaHang;
                rownew["SoLuong"] = nvlkhmhitem.SoLuong.ToString().Replace(",",".");//Chuyển về dạng số thập phân cho chuẩn
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
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
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
        ClassProcess prs = new ClassProcess();
        private async void Onkeydown(KeyboardEventArgs e)
        {
            if (e.Key == "F11")
            {
                if(!EnableEdit)
                    await  saveAsync();
            }
        }
        private async Task saveAsync()
        {
            if (await checklogicAsync())
            {
                CallAPI callAPI = new CallAPI();

                string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_InsertTableDeNghi";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialDN", keHoachMuaHang_ShowCrr.Serial));//Trong procedure đã xử lý
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
                            ToastService.Notify(new(ToastType.Success, $"Lưu thành công."));
                            NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShownew = nvlkhmhitem.CopyClass();
                            nvlKeHoachMuaHangItemShownew.Serial = query[0].Serial.Value;
                            keHoachMuaHang_ShowCrr.lstitem.Add(nvlKeHoachMuaHangItemShownew);
                            urc_KeHoachMuaHang_Detail.ReloadGrid();
                            reset();
                            // ToastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                            if(GotoMainForm.HasDelegate)
                            {
                                await GotoMainForm.InvokeAsync();
                            }
                          
                            //await txtMaHang.FocusAsync();
                        }
                        else
                        {
                            string err = "";
                            if (query[0].MaHang != null)
                            {
                                foreach (var it in query)
                                {
                                    foreach (DataRow row in dtsave.Rows)
                                    {
                                        if (it.MaHang == row.Field<string>("MaHang"))
                                        {
                                            err = "Mã hàng không đúng";
                                            break;
                                        }
                                    }
                                }
                                ToastService.Notify(new ToastMessage(ToastType.Warning, err));

                            }
                            if (query[0].Serial != null)
                            {
                                foreach (var it in query)
                                {
                                    foreach (DataRow row in dtsave.Rows)
                                    {
                                        if (it.Serial != null)
                                        {
                                            if (row["SerialLink"] != DBNull.Value)
                                            {
                                                if (it.Serial.Value == row.Field<int>("SerialLink"))
                                                {
                                                    err = "TỔNG số lượng đề nghị đã vượt quá kế hoạch là " + row.Field<double>("SLCL").ToString();
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                ToastService.Notify(new ToastMessage(ToastType.Warning, err));
                                //grvSanPham.Columns["Err"].Visible = true;
                            }
                            if (query[0].ketquaexception != null)
                            {
                                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketquaexception));
                            }

                        }

                    }
                    else
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Đã xảy ra lỗi."));
                    }


                }
                catch (Exception ex)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex));
                }
                finally
                {

                }

            }
            return;
        }
        private void reset()
        {
            nvlkhmhitem.MaHang = null;
            nvlkhmhitem.DonGia = null;
            nvlkhmhitem.SLTon = 0;
            nvlkhmhitem.SoLuong = null;
            nvlkhmhitem.TenLienKet = null;
            nvlkhmhitem.GhiChu = "";
            nvlkhmhitem.MaSP = null;
            txtMaHang.FocusAsync();
            StateHasChanged();

        }
        class GiaThamKhao
        {
            public GiaThamKhao()
            {

            }
            public string MaHang { get; set; }
            public string TenNCC { get; set; }
            public double DonGia { get; set; }
            public int Serial { get; set; }

        }
        DataTable dtsave = new DataTable();
        private async void ChangeKho()
        {

            renderFragment = builder =>
            {
                builder.OpenComponent<Urc_PhanLoaiNhomHang>(0);

                builder.AddAttribute(1, "GotoMainForm", EventCallback.Factory.Create(this, hidePopup));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            await dxPopup.showAsync("Lọc mã hàng theo kho");
            await dxPopup.ShowAsync();
        }
        private async void hidePopup()
        {
            if (ModelAdmin.lsthanghoafilter.Count > 0)
            {
                lstmahang = ModelAdmin.lsthanghoafilter;
            }

          await dxPopup.CloseAsync();
        }
        private async Task loadTonKhoAsync()
        {
            string sql = @"	use NVLDB
					select MaHang, sum(SLNhap-SLXuat) as SLTon from dbo.NvlNhapXuatItem where MaHang=@MaHang group by MaHang
                  ";
            CallAPI callAPI = new CallAPI();
            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            parameterDefineList.Add(new ParameterDefine("@MaHang", nvlkhmhitem.MaHang));
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            //Console.WriteLine("Nó lost focus");
            if (json != "")
            {
                List<NvlHangHoaDropDown> lst
                 = JsonConvert.DeserializeObject<List<NvlHangHoaDropDown>>(json);
                if (lst.Any())
                {
                    NvlHangHoaDropDown nvlHangHoaDropDown = lst.FirstOrDefault();
                    nvlkhmhitem.SLTon = nvlHangHoaDropDown.SLTon;
                    StateHasChanged();

                }
                lst.Clear();
            }
        }

        decimal? giathamkhao = 0;
        async void SelectedItemChanged(NvlHangHoaDropDown hangHoaDropDown)
        {

            if (hangHoaDropDown == null)
                return;
            nvlkhmhitem.MaHang= hangHoaDropDown.MaHang;
            nvlkhmhitem.DVT = hangHoaDropDown.DVT;
            nvlkhmhitem.SLTon = hangHoaDropDown.SLTon;
            nvlkhmhitem.TenHang = hangHoaDropDown.TenHang;
            giathamkhao = hangHoaDropDown.DonGia;
            await loadTonKhoAsync();
            //await loadTonKhoAsync();
            //_ = loaddataAsync();

        }
        void SelectedItemChangedSanPham(SanPhamDropDown sanPhamDropDown)
        {

            if (sanPhamDropDown == null)
                return;

        }

    }

}
