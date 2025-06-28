using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;

using System.ComponentModel.DataAnnotations;
using static NFCWebBlazor.App_KeHoach.Page_KeHoachThang_Master;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;
using DevExpress.XtraRichEdit.Import.Html;
using System.Data;

namespace NFCWebBlazor.App_KeHoach
{
    public partial class Urc_KeHoachThang_AddMaster
    {

        [Inject]
        PreloadService PreloadService { get; set; }

        [Parameter]
        public KeHoachSP_Show keHoachSP_Showcrr { get; set; }
        [Parameter]
        public EventCallback<KeHoachSP_Show> GotoMainForm { get; set; }
        [Inject]
        ToastService toastService { get; set; }
      
        class GetThang
        {
            public string Name { get; set; }
            public DateTime dtpbegin { get; set; }
            public DateTime dtpend { get; set; }
        }
        List<GetThang> lstthang = new List<GetThang>();
        bool CheckQuyen = false;
        private async Task loadAsync()
        {
            PreloadService.Show(SpinnerColor.Success, "Vui lòng đợi...");
            CheckQuyen =await phanQuyenAccess.CreateKeHoachThang(ModelAdmin.users);
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = @"declare @DateTime date=getdate()
                declare @tbl Table([Name] nvarchar(100),dtpbegin date,dtpend date)
                declare @date date
                declare @i int=-8
                while(@i<9)
                begin
	                select @date=DateAdd(MM,@i,@DateTime)
	                insert into @tbl([Name],dtpbegin,dtpend)
	                select 'T'+Format(@date,'MM-yyyy'),dbo.GetFisrtDayOfMonth(@date),dbo.GetLastDayOfMonth(@date) 
	                set @i=@i+1
                end
                select * from @tbl";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<List<GetThang>>(json);

                    lstthang.AddRange(query);
                   
                    //await InvokeAsync(StateHasChanged);
                }
                
                PreloadService.Hide();
            }
            catch(Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
                Console.WriteLine(ex.Message);
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
                editContext = new EditContext(keHoachSP_Showcrr);
                await loadAsync();
                StateHasChanged();

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
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

            }
            if(!checkset)
            {
                if (keHoachSP_Showcrr.Serial == 0)
                {
                    EnableEdit = false;
                }
                else
                {
                    EnableEdit = true;
                    if (lstthang.Count > 0)
                    {
                        string[] arr = keHoachSP_Showcrr.MaKHThang.Split(",");
                        List<GetThang> lstselect = new List<GetThang>();
                        foreach (string s in arr)
                        {
                            foreach (var it in lstthang)
                            {
                                if (s == it.Name)
                                {
                                    lstselect.Add(it);

                                }
                            }
                        }
                        makehoachselected = lstselect;
                        checkset = true;
                    }
                }
               
            }
            return base.OnAfterRenderAsync(firstRender);
        }

        private void Selectedmakehoachchanged(IEnumerable<GetThang>getThangSelected)
        {
            var query = getThangSelected.GroupBy(p => 1).Select(p => new { maxdate = p.Max(n => n.dtpend), mindate = p.Min(n=>n.dtpbegin) }).FirstOrDefault();
            if (query != null)
            {
                keHoachSP_Showcrr.ThangMin = query.mindate;
                keHoachSP_Showcrr.ThangMax = query.maxdate;
            }
        }
        private async Task saveAsync()
        {
            
            if (!checklogic())
                return;
           
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.KeHoachThang_Insert";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@MaKHThang", keHoachSP_Showcrr.MaKHThang));
                lstpara.Add(new ParameterDefine("@TenKHThang", keHoachSP_Showcrr.TenKHThang));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@NhaMay", keHoachSP_Showcrr.NhaMay));
                lstpara.Add(new ParameterDefine("@ThangMin", keHoachSP_Showcrr.ThangMin));
                lstpara.Add(new ParameterDefine("@ThangMax", keHoachSP_Showcrr.ThangMax));
                lstpara.Add(new ParameterDefine("@LoaiKeHoach", keHoachSP_Showcrr.LoaiKeHoach));
                lstpara.Add(new ParameterDefine("@GhiChu", keHoachSP_Showcrr.GhiChu));
                string json = await callAPI.ProcedureAsync(sql, lstpara);
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
                        toastService.Notify(new ToastMessage(ToastType.Success, string.Format("Lỗi:{0}, {1} " , query[0].ketqua, query[0].ketquaexception)));
                        //reset();
                        // msgBox.Show(string.Format("Lỗi:{0}, {1} " + query[0].ketqua, query[0].ketquaexception), IconMsg.iconssuccess);

                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }


                
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi " + ex.Message));
                //msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
        }
        private async Task updateAsync()
        {
            if (!checklogic())
                return;
            try
            {
                CallAPI callAPI = new CallAPI();
                string sql = "NVLDB.dbo.KeHoachThang_Update";
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@Serial", keHoachSP_Showcrr.Serial));
                lstpara.Add(new ParameterDefine("@MaKHThang", keHoachSP_Showcrr.MaKHThang));
                lstpara.Add(new ParameterDefine("@TenKHThang", keHoachSP_Showcrr.TenKHThang));
                lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));
                lstpara.Add(new ParameterDefine("@NguoiDuyet", keHoachSP_Showcrr.NguoiDuyet));
                lstpara.Add(new ParameterDefine("@NhaMay", keHoachSP_Showcrr.NhaMay));
                lstpara.Add(new ParameterDefine("@ThangMin", keHoachSP_Showcrr.ThangMin));
                lstpara.Add(new ParameterDefine("@ThangMax", keHoachSP_Showcrr.ThangMax));
                lstpara.Add(new ParameterDefine("@GhiChu", keHoachSP_Showcrr.GhiChu));
              
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, "Sửa thành công"));
                        if(GotoMainForm.HasDelegate)
                        {
                            await  GotoMainForm.InvokeAsync(keHoachSP_Showcrr);
                        }
                        reset();
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, query[0].ketqua+", " + query[0].ketquaexception));
                        Console.WriteLine(query[0].ketquaexception);
                       // msgBox.Show("Lỗi: " + query[0].ketqua, IconMsg.iconssuccess);

                    }

                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi "+ex.Message));
                //msgBox.Show("Lỗi: " + ex.Message, IconMsg.iconerror);
            }
        }
        private void reset()
        {
            makehoachselected = null;
            keHoachSP_Showcrr.GhiChu = "";
            StateHasChanged();

        }


        private  bool checklogic()
        {
           if(!CheckQuyen)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có quyền thao tác ở bảng này"));
                return false;
            }
            keHoachSP_Showcrr.MaKHThang = "";
            if (makehoachselected != null)
            {
                foreach (var it in makehoachselected)
                {
                    if (keHoachSP_Showcrr.MaKHThang == "")
                        keHoachSP_Showcrr.MaKHThang = it.Name;
                    else
                        keHoachSP_Showcrr.MaKHThang += "," + it.Name;
                }
            }
            //editContext=new EditContext(keHoachSP_Showcrr);
            return editContext.Validate();
        }
        public async void ShowFlyout()
        {
            await dxFlyoutchucnang.CloseAsync();
            //CurrentEmployee = employee;

            //StateHasChanged();
          
            await dxFlyoutchucnang.ShowAsync();
            //dxFlyoutchucnang.PositionTarget = idflychucnang;
            //Console.WriteLine(idflychucnang);
            //dxFlyoutchucnang.RepositionAsync();
            // IsOpenfly = true;
            //await dxFlyoutchucnang.ShowAsync();

        }
       
       

    }
}

