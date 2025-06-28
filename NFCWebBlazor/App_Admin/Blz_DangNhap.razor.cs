
using Microsoft.AspNetCore.Components;

using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;

using NFCWebBlazor.Shared;


namespace NFCWebBlazor.App_Admin
{
    public partial class Blz_DangNhap
    {
        [Inject] CallAPI? callAPI { get; set; }
     
        Users? users { get; set; }
        [Inject] UserGlobal? userGlobal { get; set; }
        [Parameter]
        public EventCallback GotoMainForm { get; set; }
        [Inject] Blazored.SessionStorage.ISessionStorageService? session { get; set; }
        //[Inject] IJSRuntime jSRuntime { get; set; }

        [Parameter]
        public MainLayout? mainLayout { get; set; }
       
        public string? textmsg { get; set; }
        public bool visiblemsg=false;
        public Users? userslogin { get; set; } = new Users();
        App_ClassDefine.ClassProcess prs=new App_ClassDefine.ClassProcess();
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
           
            if(firstRender)
            {
               
                userslogin.TypeApp = typeappselected;
              
                userslogin.NhaMay = "Nhà máy A";
             
                //await jSRuntime.InvokeVoidAsync("disableAutocomplete", "txtuser");
                //await jSRuntime.InvokeVoidAsync("disableAutocomplete", "txtpassword");
            }
            else
            {
                //Console.WriteLine("dangnhap AAAAA");
                //shouldrender = false;
            }
            //return base.OnAfterRenderAsync(firstRender);
        }
        
        protected override async Task OnInitializedAsync()
        {
            //List<DataDropDownList> lst = await Model.ModelData.GetDataDropDownListsAsync();
            //lstnhamay = lst.Where(p => p.TypeName == "NhaMay_NVL").ToList();
            //lstphanmem = lst.Where(p => p.TypeName == "TypeApp").AsEnumerable();
            lstnhamay = new List<DataDropDownList>();
           
            string[] arrnhamay = new string[] { "Nhà máy A" };
            string[] arrtypeapp = new string[] { "NVL;Nguyên vật liệu", "SP;Sản xuất" };
            //Sử dụng code động tránh load xuống database để việc vào trang đầu tiên sẽ nhanh hơn
            foreach (string it in arrnhamay)
            {
                DataDropDownList dataDropDownList = new DataDropDownList();
                dataDropDownList.Name = it;
                dataDropDownList.FullName = it;
                dataDropDownList.TypeName = "NhaMay_NVL";
                lstnhamay.Add(dataDropDownList);
            }
            List<DataDropDownList> lstpm = new List<DataDropDownList>();
            foreach(string it in arrtypeapp)
            {
                DataDropDownList dataDropDownList = new DataDropDownList();
                string[] arr = it.Split(';');
                dataDropDownList.Name = arr[0];
                dataDropDownList.FullName = arr[1];
                dataDropDownList.TypeName = "TypeApp";
                lstpm.Add(dataDropDownList);
            }
            lstphanmem = lstpm.AsEnumerable();
           
            //typeapp = lstphanmem.FirstOrDefault();
            //Console.WriteLine(typeapp);
            //return base.OnInitializedAsync();
        }
        private async Task<bool> checkUserAsync()
        {

           // string sql = "select * from Users where UsersName=@UsersName and Password=@Password";
            string sql = "UsersLogin";
            List<ParameterDefine> parameters = new List<ParameterDefine>();
            parameters.Add(new ParameterDefine("@UsersName", userslogin.UsersName));
            parameters.Add(new ParameterDefine("@Password", userslogin.Password));
            parameters.Add(new ParameterDefine("@GhiChu", "Web"));
            //string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameters);
            string json = await callAPI.ProcedureEncryptAsync(sql, parameters);
            //if (json != "")
            //{
            //    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
           
            users =  JsonConvert.DeserializeObject<List<Users>>(json).FirstOrDefault();
            if (users != null)
            {
                try
                {
                    _= Model.ModelData.GetDataDropDownListsAsync();//Tải dropdownlist ở đây
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Lỗi load combobox "+ex.Message);
                }
                userGlobal.users.UsersName = users.UsersName;
                userGlobal.users.TenUser = users.TenUser;
                userGlobal.users.GroupUser = users.GroupUser;
                userGlobal.users.KhuVuc = users.KhuVuc;
                userGlobal.users.PathImg = users.PathImg;
                userGlobal.users.TypeApp= typeappselected;
                userGlobal.users.NhaMay = userslogin.NhaMay;
                ModelSession modelSession = new ModelSession();
                modelSession.setUserSession(userGlobal.users, session);
                Model.ModelAdmin.users = userGlobal.users;

            }

            else
                return false;

            
            return true;
        }
        private async void ClickCheckUser()
        {
            userslogin.TypeApp = typeappselected;
            if(String.IsNullOrEmpty(userslogin.UsersName) || String.IsNullOrEmpty(userslogin.Password)||String.IsNullOrEmpty(userslogin.NhaMay)||String.IsNullOrEmpty(userslogin.TypeApp))
            {
                textmsg = "Nhập tên mật khẩu";
                StateHasChanged();
            }
            else
            {
                //textmsg = "";

                if (await checkUserAsync())
                {
                    textmsg = "Xin chào " + userslogin.UsersName;
                    await GotoMainForm.InvokeAsync();
                    //StateHasChanged();
                }
                else
                {
                    visiblemsg = true; textmsg = "Tên hoặc mật khẩu không đúng";
                 
                    StateHasChanged();
                }
            }
           
        }

        void HandleValidSubmit()
        {
           
            ClickCheckUser();
           // textmsg = "";
            //if (checkUser())
            //{
            //    textmsg = "Xin chào chú " + users.UserName;
            //}
            //else
            //    textmsg = "Sai tên hoặc mật khẩu";
        }
        void HandleInvalidSubmit()
        {
          object ob=userslogin;
        }
        
    }
}
