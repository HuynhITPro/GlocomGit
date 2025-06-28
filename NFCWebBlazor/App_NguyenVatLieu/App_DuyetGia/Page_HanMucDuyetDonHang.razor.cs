using BlazorBootstrap;
using Newtonsoft.Json;
using NFCWebBlazor.Model;

using System.Data;

using Microsoft.AspNetCore.Components;
using NFCWebBlazor.App_ModelClass;
using System.Text;
using static NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia.Page_HanMucDuyetDonHang;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia
{
    public partial class Page_HanMucDuyetDonHang
    {
        [Inject] ToastService toastService { get; set; }
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        public  class NvlKyDuyetHanMucShow
        {
            public int Serial { get; set; }

            public string UserName { get; set; }
            public string PathImg { get; set; }
            public string TenUser { get; set; }
            public long? HanMucCu { get; set; }

            public long? HanMuc { get; set; }

            public string GhiChu { get; set; }

            public string UserInsert { get; set; }
         
            public DateTime? NgayInsert { get; set; }
            public NvlKyDuyetHanMucShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlKyDuyetHanMucShow>(json);
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                PhanQuyenCheck = await phanQuyenAccess.CreateHanMucThanhToan(ModelAdmin.users);
                await searchAsync();
            }
           
        }
        private async Task searchAsync()
        {
            CallAPI callAPI = new CallAPI();
            string sql = string.Format(@"use NVLDB select isnull(hm.Serial,0) as Serial,case when hm.Serial is null then 0 else hm.HanMuc end as HanMucCu,0 as HanMuc,hm.NgayInsert,usr.UsersName as UserName ,hm.[UserInsert]
            ,usr.TenUser,'{0}'+'/'+isnull(usr.PathImg,'UserImage/user.png') as PathImg,hm.GhiChu from
        (SELECT UserName from DBMaster.dbo.User_PhanQuyen 
        where TableName='KyDuyetDonHang' and Permission='Write') as qry
         inner join DBMaster.dbo.Users usr on qry.UserName=usr.UsersName
         left join 
        (select * from
         [dbo].[NvlKyDuyetHanMuc]
          where Serial in (select max(Serial) from [NvlKyDuyetHanMuc] group by UserName)
         ) hm on qry.UserName=hm.UserName

            ", Model.ModelAdmin.pathurlfilepublic);
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            //lstpara.Add(new ParameterDefine("@SerialLink", nvlDuyetGiaShowcrr.Serial));
            string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var querycustom = JsonConvert.DeserializeObject<List<NvlKyDuyetHanMucShow>>(json);
                if (querycustom == null)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Đã có lỗi xảy ra"));
                    return;
                }
                lstdata = querycustom;
                Grid.Reload();
                StateHasChanged();
            }
        }
        private async Task DeleteItemAsync(NvlKyDuyetHanMucShow nvlKyDuyetHanMucShow)
        {
            if (!phanQuyenAccess.CheckDelete(nvlKyDuyetHanMucShow.UserInsert, ModelAdmin.users))
            {
                toastService.Notify(new(ToastType.Warning, $"Bạn không có quyền xóa dòng này do bạn không phải người tạo"));
                return;
            }

            bool bl = await dialogMsg.Show("THÔNG BÁO", $"Bạn có chắc muốn xóa {nvlKyDuyetHanMucShow.TenUser}?");
            if (bl)
            {
                try
                {
                    CallAPI callAPI = new CallAPI();
                    string sql = "NVLDB.dbo.NvlKyDuyetHanMuc_Delete";
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();

                    lstpara.Add(new ParameterDefine("@Serial", nvlKyDuyetHanMucShow.Serial));
                    lstpara.Add(new ParameterDefine("@UserDelete", ModelAdmin.users.UsersName));

                    
                    string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                        if (query[0].ketqua == "OK")
                        {
                            toastService.Notify(new(ToastType.Success, $"Xóa thành công"));
                            await searchAsync();
                            //lstdata.Remove(nvlKyDuyetHanMucShow);
                            //await Grid.SaveChangesAsync();
                        }
                        else
                        {
                            toastService.Notify(new(ToastType.Danger, $"{query[0].ketqua}, {query[0].ketquaexception}"));

                        }
                        //Grid.Data = lstDonDatHangSearchShow;
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message} Xóa không được"));
                }


            }
        }
        private async Task saveAsynAsync()
        {
            if (!PhanQuyenCheck)
            {
                toastService.Notify(new(ToastType.Warning, $"Bạn không có quyền tạo hạn mức"));
                return;
            }
            var query =lstdata.Where(p=>p.HanMuc>0).ToList();
            if(query.Count==0)
                {
                toastService.Notify(new(ToastType.Warning,"Chưa nhập hạn mức"));
                return; }
            
            string jsoninsert =System.Text.Json.JsonSerializer.Serialize(query);
            
           // Console.WriteLine(jsoninsert);
            string sql = "NVLDB.dbo.NvlKyDuyetHanMuc_Insert";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            CallAPI callAPI = new CallAPI();
            lstpara.Add(new ParameterDefine("@jsoninsert", jsoninsert));
            lstpara.Add(new ParameterDefine("@UserInsert", ModelAdmin.users.UsersName));

            string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var queryitem = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                if (queryitem[0].ketqua == "OK")
                {
                    toastService.Notify(new(ToastType.Success, $"Lưu thành công"));
                  await  searchAsync();
                    //await Grid.SaveChangesAsync();
                }
                else
                {
                    toastService.Notify(new(ToastType.Danger, $"{queryitem[0].ketqua}, {queryitem[0].ketquaexception}"));

                }
                //Grid.Data = lstDonDatHangSearchShow;
            }
        }
    }
}
