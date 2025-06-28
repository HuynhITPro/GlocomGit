using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using static NFCWebBlazor.App_KeHoach.Page_KeHoachThang_Master;
using DevExpress.Blazor.Popup.Internal;

namespace NFCWebBlazor.App_KeHoach
{
    public partial class Urc_KeHoachThang_ItemAdd
    {
        [Inject]
        PreloadService PreloadService { get; set; }

        
        [Inject]
        ToastService toastService { get; set; }

        bool CheckQuyen = false;
        
        private async Task loadAsync()
        {
            PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
            CheckQuyen = await phanQuyenAccess.CreateKeHoachThang(ModelAdmin.users);
            try
            {
                lstsanphamdropdown = await ModelData.GetSanPhamDropDown();
                lstart =await ModelData.GetArticleNumber();
                if (keHoachThangItem_Showform.Serial > 0)
                {
                    if (lstart != null)
                    {

                        checkset = true;
                        lstArtselected = lstart.Where(p => p.MaSP.Equals(keHoachThangItem_Showform.MaSP)).ToList();
                        callselecitem = false;
                        //Đối với hàm edit thì gán ở đây để đảm bảo sau khi load xong hết dữ liệu rồi mới gán biến
                        //keHoachThangItem_ShowformeditContext = new EditContext(keHoachThangItem_Showform);
                        StateHasChanged();

                    }

                }
                if (keHoachThangItem_Showform.Serial == 0)
                {
                    EnableEdit = false;
                    checkset = true;
                }
                else
                {
                    EnableEdit = true;

                }
                PreloadService.Hide();
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
            }
            finally
            {
                PreloadService.Hide();
            }
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
               
                //if(keHoachThangItem_Showcrr.Serial==0||keHoachThangItem_Showcrr.Serial==null)//Đối với hàm thêm mới thì gán ở đây cũng được
                //{
                //    keHoachThangItem_Showform = keHoachThangItem_Showcrr.CopyClass();
                //    //editContext = new EditContext(keHoachThangItem_Showform);
                //}
                //keHoachThangItem_Showform = new KeHoachThangItem_Show();
                editContext = new EditContext(keHoachThangItem_Showform);
                editContext.OnValidationRequested += CustomValidation;//Dùng hàm Validate của editcontext ko ổn cho lắm
                await loadAsync();
                
               

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load form ke hoach master " + ex.Message);
            }
            finally
            {

            }
            base.OnInitialized();
        }

        bool checkset = false;
       
        private async Task<IEnumerable<DataDropDownList>> LoadSanPhamAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            lstsanphamdropdown = await ModelData.GetSanPhamDropDown();
           // Console.WriteLine("load sp");
            return lstsanphamdropdown;
        }
        bool callselecitem = true;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
               
            }
           
            return base.OnAfterRenderAsync(firstRender);
        }

       
        private async Task saveAsync()
        {

            if (!checklogic())
            {
                StateHasChanged();
                return;
            }
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.KeHoachThangItem_Insert";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial_KHThang", keHoachSP_Showcrr.Serial));
                lstpara.Add(new ParameterDefine("@ArticleNumber", keHoachThangItem_Showform.ArticleNumber));
                lstpara.Add(new ParameterDefine("@MaSP", keHoachThangItem_Showform.MaSP));
                lstpara.Add(new ParameterDefine("@GhiChu", keHoachThangItem_Showform.GhiChu));
                lstpara.Add(new ParameterDefine("@SLSP", keHoachThangItem_Showform.SLSP));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@MaDHMua", keHoachThangItem_Showform.MaDHMua));
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "LƯU THÀNH CÔNG"));
                        if(GotoMainForm.HasDelegate)
                        {
                           await GotoMainForm.InvokeAsync(keHoachSP_Showcrr);
                        }
                        reset();
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, string.Format("Lỗi:{0}, {1} " , query[0].ketqua, query[0].ketquaexception)));
                        //reset();
                        // msgBox.Show(string.Format("Lỗi:{0}, {1} " + query[0].ketqua, query[0].ketquaexception), IconMsg.iconssuccess);

                    }
                }



            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi Không lưu được" + ex.Message));

            }
        }
        private async Task updateAsync()
        {
            if (!checklogic())
            {
                StateHasChanged();
                return;
            }
              
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.KeHoachThangItem_Edit";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", keHoachThangItem_Showform.Serial));
                lstpara.Add(new ParameterDefine("@ArticleNumber", keHoachThangItem_Showform.ArticleNumber));
                lstpara.Add(new ParameterDefine("@MaSP", keHoachThangItem_Showform.MaSP));
                lstpara.Add(new ParameterDefine("@SLSP", keHoachThangItem_Showform.SLSP));
                lstpara.Add(new ParameterDefine("@MaDHMua", keHoachThangItem_Showform.MaDHMua));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@GhiChu", keHoachThangItem_Showform.GhiChu));
                lstpara.Add(new ParameterDefine("@LyDoUpdate", "Sửa dữ liệu"));

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, "Sửa thành công"));
                        if (GotoDetailItem.HasDelegate)
                        {
                            var queryart = lstart.Where(p => p.ArticleNumber.Equals(keHoachThangItem_Showform.ArticleNumber)).FirstOrDefault();
                            if(queryart!=null)
                            {
                                keHoachThangItem_Showform.TenMau = queryart.TenMau;
                                keHoachThangItem_Showform.ColorHex = queryart.Colorhex;
                               
                            }
                            var querysp = lstsanphamdropdown.Where(p => p.Name.Equals(keHoachThangItem_Showform.MaSP)).FirstOrDefault();
                            if(querysp!=null)
                            {
                                keHoachThangItem_Showform.TenSP = querysp.FullName;
                            }
                           
                            await GotoDetailItem.InvokeAsync(keHoachThangItem_Showform);
                        }
                        //reset();
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, query[0].ketqua +", " +query[0].ketquaexception));
                        //msgBox.Show("Lỗi: " + query[0].ketqua, IconMsg.iconssuccess);

                    }

                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi sửa không được:"+ex.Message));
            }
        }
        private async void reset()
        {

            keHoachThangItem_Showform.GhiChu = "";
            keHoachThangItem_Showform.ArticleNumber = null;
            //articleNumberProductselected = null;
            keHoachThangItem_Showform.SLSP = null;
            keHoachThangItem_Showform.MaSP = null;
            keHoachThangItem_Showform.SLNhap = null;
            await txtMaSP.FocusAsync();
            StateHasChanged();

        }


        private bool checklogic()
        {
            if (!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có quyền thao tác ở bảng này"));
                return false;
            }
            
            return editContext.Validate();
        }
        private ValidationMessageStore validationMessages;
        private void CustomValidation(object? sender, ValidationRequestedEventArgs e)
        {
            if (validationMessages == null)
                validationMessages = new ValidationMessageStore(editContext);

            // Xóa các thông báo lỗi hiện tại
            validationMessages.Clear();
            if (string.IsNullOrEmpty(keHoachThangItem_Showform.MaSP))
            {
                //validationMessages.Add(("MaHang", "Chọn mã hàng."));
                validationMessages.Add(() => keHoachThangItem_Showform.MaSP, "Chọn mã sản phẩm.");
            }
            if (keHoachThangItem_Showform.SLSP == 0 || keHoachThangItem_Showform.SLSP == null)
            {
                //validationMessages.Add(("BanIn", "Bản in phải lớn hơn 0."));

                validationMessages.Add(() => keHoachThangItem_Showform.SLSP, "Nhập số lượng");
            }
            if (string.IsNullOrEmpty(keHoachThangItem_Showform.ArticleNumber))
            {
                //validationMessages.Add(("MaHang", "Chọn mã hàng."));
                validationMessages.Add(() => keHoachThangItem_Showform.ArticleNumber, "Vui lòng chọn ArticleNumber.");
            }

          
            editContext.NotifyValidationStateChanged();
        }
        private void SanPhamDropDownSelected(DataDropDownList dataDropDownList)
        {
            if (dataDropDownList != null)
            {
                if (lstart == null)
                    return;
                if(!callselecitem)
                {
                    callselecitem = true;
                    return;
                }
                keHoachThangItem_Showform.ArticleNumber = null;
                lstArtselected = lstart.Where(p => p.MaSP.Equals(dataDropDownList.Name)).ToList();
               

                StateHasChanged();
            }
        }
        public async void ShowFlyout()
        {
            await dxFlyoutchucnang.CloseAsync();
            //CurrentEmployee = employee;

            //StateHasChanged();
            //IsOpenfly = true;
            await dxFlyoutchucnang.ShowAsync();

        }



    }
}

