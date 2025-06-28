
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using NFCWebBlazor.Shared;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Urc_KeHoachMuaHangAddMaster;

namespace NFCWebBlazor.App_Admin
{
    public partial class Page_PhanQuyenMaster
    {
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] UserGlobal? userGlobal { get; set; }
        [Inject] PhanQuyenAccess? phanQuyenAccess { get; set; }
        App_ClassDefine.ClassProcess prs=new App_ClassDefine.ClassProcess();
        protected override async Task OnInitializedAsync()
        {
            await loadAsync();
            // return base.OnInitializedAsync();
        }
        string heightgrid = "500px";
        public class User_PhanQuyenShow : INotifyPropertyChanged
        {
            public int Serial { get; set; }
            public string? Menu { get; set; }
            public string? TableID { get; set; }
            public string? TableName { get; set; }
            public string? UserName { get; set; }
            public string? Permission { get; set; }
            public string? UserCapQuyen { get; set; }
            private bool _ChkRead;
            private bool _ChkWrite;
            public Nullable<System.DateTime> NgayInsert { get; set; }
            public bool ChkRead
            {
                get { return _ChkRead; }
                set
                {

                    //_ChkRead = value;
                    SetPropertyValue(ref _ChkRead, value);
                }
            }
            public bool ChkWrite
            {
                get { return _ChkWrite; }
                set
                {

                    //_ChkWrite = value;
                    SetPropertyValue(ref _ChkWrite, value);
                    
                    //NotifyPropertyChanged("ChkWrite");
                }
            }


            public event PropertyChangedEventHandler? PropertyChanged;
            protected void SetPropertyValue<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
            {
                if (EqualityComparer<T>.Default.Equals(property, value))
                    return;
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        private async Task loadAsync()
        {

            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 90;
            heightgrid = string.Format("{0}px", height);
            await loaduserAsync();
            // var query=ModelData.L
        }
        private async Task loaduserAsync()
        {
            CallAPI callAPI = new CallAPI();
            string sql = "SELECT [UsersName] as [Name],UsersName as FullName,KhuVuc as TypeName FROM [Users]";

            List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
            if (json != "")
            {

                lstuser = JsonConvert.DeserializeObject<List<DataDropDownList>>(json);
            }
            await loadmenu();

        }
        List<MenuItem> lstmenuitems = new List<MenuItem>();
        private async Task<bool> loadmenu()
        {
            string[] arr = userGlobal.users.KhuVuc.Split(';');
            string dieukien = "";
            lstmenuitems.Clear();
            foreach (string it in arr)
            {
                if (it != "")
                {
                    if (dieukien == "")
                        dieukien += string.Format(" where [UserArea] like '%{0}%'", it);
                    else
                        dieukien += string.Format(" or [UserArea] like '%{0}%'", it);
                }

            }
            string sql = string.Format(@"SELECT  [Serial],[NameItem]
                  ,[TextItem],[NavigateUrl],[ComponentName]
                  ,[IconCssClass],[UserArea]
                  ,[NodeParent],[Tag],isnull([STT],1000) as STT,isnull(PhanQuyen,'') as PhanQuyen
                    FROM MenuItemWeb {0} or [UserArea] like '%all%'  order by isnull([STT],1000),Serial", dieukien);


            List<ParameterDefine> parameters = new List<ParameterDefine>();
            CallAPI callAPI = new CallAPI();
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameters);
            if (String.IsNullOrEmpty(json))
            {
                return false;
            }
            else
            {
                lstmenuitems = JsonConvert.DeserializeObject<List<MenuItem>>(json);
                //navMenu.shouldrender = false;

            }
            return true;
        }
        private void selectednhom(DataDropDownList dataDropDownList)
        {
            if (dataDropDownList != null)
            {
                string s = dataDropDownList.Name.ToString() + ";";
                List<DataDropDownList> list = new List<DataDropDownList>();
                var query = lstuser.Where(p => p.TypeName.Contains(s));
                userselected = query;
                StateHasChanged();
            }
            else
            {
                userselected = null;
                StateHasChanged();

            }


        }
        private async Task searchAsync()
        {
            string dieukien = "";
            if(userselected==null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn user muốn xem"));
                return;
            }
            if(userselected.Count()==0)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn user muốn xem"));
                return;
            }
          
                foreach (var it in userselected)
                {
                    if (dieukien == "")
                    {
                        dieukien += string.Format("N'{0}'", it.Name);

                    }
                    else
                    {
                        dieukien += string.Format(",N'{0}'", it.Name);
                    }
                }
                lstdata.Clear();
                
                dieukien = string.Format(" where UserName in ({0}) and Permission='Write'", dieukien);
                string sql = string.Format("SELECT [TableName],TableName as TableID,Permission,cast(0 as bit) as ChkWrite,[UserName],[UserCapQuyen] FROM [User_PhanQuyen] {0}", dieukien);
            PanelVisible = true;
            CallAPI callAPI = new CallAPI();

            try
            {
                List<ParameterDefine> parameterDefineList = new List<ParameterDefine>();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, parameterDefineList);
                if (json != "")
                {

                    lstdata = JsonConvert.DeserializeObject<List<User_PhanQuyenShow>>(json);
                    PanelVisible = false;
                    Grid.Reload();
                }
            }
            catch(Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
            }
            finally
            {
                PanelVisible = false;
            }
           
           
        }
        private void additem(MenuItem menuItem)
        {
            var query = lstdata.Where(p => p.TableID == menuItem.PhanQuyen).FirstOrDefault();
            if (query != null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, "Nhóm quyền này đã có ở danh sách phía dưới rồi"));
                return;
            }

            User_PhanQuyenShow user_PhanQuyenShow = new User_PhanQuyenShow();

            user_PhanQuyenShow.TableID = menuItem.PhanQuyen;
            user_PhanQuyenShow.TableName = menuItem.PhanQuyen;
            user_PhanQuyenShow.UserCapQuyen = ModelAdmin.users.UsersName;
            user_PhanQuyenShow.ChkWrite = true;
            lstdata.Add(user_PhanQuyenShow);
            Grid.SaveChangesAsync();


            //Grid.AutoFitColumnWidths();
        }
        private bool checklogic()
        {
            if(userselected==null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn User"));
                return false;
            }
            if(userselected.Count()==0)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn User"));
                return false;

            }
            if(lstdata.Count()==0)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng thêm quyền"));
                return false;

            }
            return true;
        }
        private async Task saveAsync()
        {

            if (!checklogic())
                return;
            try
            {
                var querysave = lstdata.Where(p => p.ChkWrite.Equals(true)).GroupBy(p => new { tblid = p.TableID, ChkWrite = p.ChkWrite }).Select(p => new { TableID = p.Key.tblid, ChkWrite = p.Key.ChkWrite }).ToList();
                if(querysave.Count==0)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Vui lòng chọn ít nhất 1 user có quyền Ghi"));
                    return;
                }
                DataTable dtsave = new DataTable();
                CallAPI callAPI = new CallAPI();
                string sql = "UserPhanQuyen_InsertTableNew";
                dtsave.Columns.Add("Serial", typeof(int));
                dtsave.Columns.Add("TableName", typeof(string));
                dtsave.Columns.Add("UserName", typeof(string));
                dtsave.Columns.Add("Permission", typeof(string));
                //var querysave = lstdata.Where(p => p.ChkWrite.Equals(true)).GroupBy(p => new { tblid = p.TableID, ChkWrite = p.ChkWrite }).Select(p => new { TableID = p.Key.tblid, ChkWrite = p.Key.ChkWrite }).ToList();

                foreach (var useritem in userselected)
                {
                    foreach (var it in querysave)
                    {
                        DataRow rownew = dtsave.NewRow();
                        rownew["Serial"] = 0;
                        rownew["TableName"] = it.TableID;
                        rownew["UserName"] = useritem.Name;
                        rownew["Permission"] = "Write";
                        dtsave.Rows.Add(rownew);
                    }
                }

                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("@dtUserPhanQuyen", prs.ConvertDataTableToJson(dtsave), "DataTable"));
                lstpara.Add(new ParameterDefine("@UserCapQuyen", ModelAdmin.users.UsersName));
                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                        reset();
                    }
                    else
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query[0].ketqua));


                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
        }
            catch (Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
            }
}
        private void reset()
        {
            lstdata.Clear();
            userselected = null;
            khuvucselected = null;
            StateHasChanged();
        }
        private async Task userpermissionAsync(User_PhanQuyenShow user_PhanQuyenShow)
        {
            lstuserpermission.Clear();
             string sql = string.Format(@"select Users.UsersName,Users.TenUser,'{0}'+isnull([PathImg],'UserImage/user.png') as PathImg from (SELECT  [Serial],[TableName],[UserName],[Permission]
      
                            FROM [User_PhanQuyen] where TableName=@TableName and Permission=@Permission) as qry inner join dbo.Users on qry.UserName=Users.UsersName", Model.ModelAdmin.pathurlfilepublic + "/");


            //PanelVisible = true;
            try
            {
                user_PhanQuyenShowcrr = user_PhanQuyenShow;
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                lstpara.Add(new ParameterDefine("TableName", user_PhanQuyenShow.TableID));
                lstpara.Add(new ParameterDefine("Permission", "Write"));
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {

                    var query = JsonConvert.DeserializeObject<List<Users>>(json);
                    if(query.Count==0)
                    {
                        ToastService.Notify(new ToastMessage(ToastType.Warning, "Chưa phân quyền cho chức năng này"));
                    }
                    lstuserpermission.AddRange(query);
                   
                }
               
               
            }
            catch(Exception ex)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
            }
            finally
            {
                await Grid.SaveChangesAsync();
                PopupVisible = true;
                //PanelVisible = false;
            }
            
        }



    }

}
