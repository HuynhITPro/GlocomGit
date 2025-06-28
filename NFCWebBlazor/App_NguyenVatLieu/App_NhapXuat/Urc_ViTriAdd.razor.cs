using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using static NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat.Urc_NhapXuatItemAdd;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Urc_ViTriAdd
    {
        [Inject]
        ToastService toastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        class ViTriGoiY
        {
            public string ViTri { get; set; }
            public string ketqua { get; set; }
        }
        bool CheckQuyen = false;
        private async Task checkSerialAsync(string Serial)
        {
            if(Serial==null)
                return;
            if (!int.TryParse(Serial, out int serialout))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Giá trị nhập phải là số"));
                return;
            }
            if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho)||string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.NhaMay))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Chọn nhà máy và kho trước"));
                return;
            }
            nvlNhapXuatItemShowcrr.SerialLink = serialout;
            string sql = "NVLDB.dbo.NvlGoiyViTri";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@SerialLink", nvlNhapXuatItemShowcrr.SerialLink));
            lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
            lstpara.Add(new ParameterDefine("@NhaMay", nvlNhapXuatItemShowcrr.NhaMay));
            CallAPI callAPI = new CallAPI();
            string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
            lstgoiyvitri.Clear();
            goiyselected = null;
            if (json != "")
            {
                lstgoiyvitri = JsonConvert.DeserializeObject<List<ViTriGoiY>>(json);
                if (lstgoiyvitri.Any())
                {
                    if (lstgoiyvitri[0].ketqua=="OK")
                    {
                        goiyselected=lstgoiyvitri.AsEnumerable();
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, string.Format("{0}", lstgoiyvitri[0].ketqua)));
                    }
                }

            }
            StateHasChanged();

        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                editContext = new EditContext(nvlNhapXuatItemShowcrr);
                editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
                CheckQuyen = await phanQuyenAccess.CreateNhapXuatKho(Model.ModelAdmin.users);
               if(!CheckQuyen)
                {
                    CheckQuyen= await phanQuyenAccess.CreateViTriKho(Model.ModelAdmin.users);
                }
                await loadAsync();
                // StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load form ke hoach master " + ex.Message);
            }
            finally
            {
            }
        }
        ValidationMessageStore validationMessages { get; set; }
        bool checksave = true;//Gán biến để tránh trường hợp mạng lag API gọi lại 2 lần
        private async void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {
            if (validationMessages == null)
                validationMessages = new ValidationMessageStore(editContext);
            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();
            if (nvlNhapXuatItemShowcrr.SerialLink == null || nvlNhapXuatItemShowcrr.SerialLink <= 0)
            {
                validationMessages.Add(() => nvlNhapXuatItemShowcrr.SerialLink, "Vui lòng quét tem.");
            }
            //Thực hiện kiểm tra tùy chỉnh, ví dụ kiểm tra giá trị null cho KhuVuc
           
            if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.ViTri))
            {
                validationMessages.Add(() => nvlNhapXuatItemShowcrr.ViTri, "Chọn vị trí.");
            }
            if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.NhaMay))
            {
                validationMessages.Add(() => nvlNhapXuatItemShowcrr.NhaMay, "Chọn nhà máy.");
            }
            if (string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            {
                validationMessages.Add(() => nvlNhapXuatItemShowcrr.MaKho, "Chọn Kho.");
            }
            editContext.NotifyValidationStateChanged();
        }
        List<NvlViTri>lstvitriall=new List<NvlViTri>();
        private async Task loadAsync()
        {
            lstkhonvl = await Model.ModelData.GetKhoNvl();
            lstvitriall = await Model.ModelData.GetListViTri();
        }
        private bool checklogic()
        {
          
            //editContext = new EditContext(nvlDuyetGiaShowcrr);
            //editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
            return editContext.Validate();
        }
        private void khoselected(DataDropDownList dataDropDownList)
        {
            if(dataDropDownList==null) return;
            nvlNhapXuatItemShowcrr.MaKho = dataDropDownList.Name;
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho) && !string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.NhaMay))
            {
                nvlNhapXuatItemShowcrr.ViTri = null;
                lstgoiyvitri.Clear();
                goiyselected=null;
                if (lstvitriall != null)
                {
                    lstViTri.Clear();
                    string MaKho = dataDropDownList.Name;
                    lstViTri = lstvitriall.Where(p => p.MaKho == MaKho && p.NhaMay == nvlNhapXuatItemShowcrr.NhaMay).ToList();
                }
            }


        }
        private void clickgoiy(string str)
        {
            nvlNhapXuatItemShowcrr.ViTri = str;

        }
        private async Task saveAsync()
        {
            if (!checklogic())
                return;
            try
            {

                CallAPI callAPI = new CallAPI();

                string sql = "NVLDB.dbo.NvlViTri_Insert";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@SerialNK", nvlNhapXuatItemShowcrr.SerialLink));
                lstpara.Add(new ParameterDefine("@ViTri", nvlNhapXuatItemShowcrr.ViTri));
                lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
                lstpara.Add(new ParameterDefine("@NhaMay", nvlNhapXuatItemShowcrr.NhaMay));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
              
                ModelAdmin.MaKhoSelected = nvlNhapXuatItemShowcrr.MaKho;
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "LƯU THÀNH CÔNG"));
                        reset();
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, string.Format("Lỗi:{0}, {1} ", query[0].ketqua, query[0].ketquaexception)));
                        reset();
                    }

                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi " + ex.Message));
                //msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
        }

        private void reset()
        {
            nvlNhapXuatItemShowcrr.SerialLink= null;
            lstgoiyvitri.Clear();
            goiyselected = null;
            StateHasChanged();

        }
    }
}
