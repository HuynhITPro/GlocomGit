
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Data;
using System;
using NFCWebBlazor.Pages;
using NFCWebBlazor.App_ClassDefine;
using DevExpress.Blazor;
using NFCWebBlazor.App_NguyenVatLieu.App_DuyetGia;

namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class Urc_ChonNguoiDuyet
    {

        [Inject] ToastService ToastService { get; set; }
        [Inject] PhanQuyenAccess phanQuyenAccess { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                try
                {
                    string sql = "";
                    if (nVLDonDatHangShowcrr == null)
                    {
                        sql = string.Format(@"SELECT UsersName,isnull(Email,N'Thêm email') as Email,TenUser,'{1}'+isnull([PathImg],'UserImage/user.png') as PathImg  
                                FROM [Users] 
                                where UsersName in (select UserName from User_PhanQuyen where TableName=N'{0}' and Permission='Write')", KyDuyet, ModelAdmin.pathurlfilepublic + "/");
                    }
                    else
                    {
                        sql = string.Format(@" use [NVLDB]
                                    declare @DateBegin date
                                    declare @DateEnd date
                                    select @DateBegin=dbo.GetFisrtDayOfMonth(getdate())
                                    select @DateEnd=dateAdd(dd,1,dbo.GetLastDayOfMonth(getdate()))

                                    select qryuser.UsersName,qryuser.Email,qryuser.TenUser,qryuser.PathImg
                                    ,qryHanMuc.UserName as UserHanMuc,qryHanMuc.HanMuc,ISNULL(qrysudung.ThanhTien,0) as ThanhTien
                                     from
                                    (SELECT UsersName,isnull(Email,N'Thêm email') as Email,TenUser,'{1}'+isnull([PathImg],'UserImage/user.png') as PathImg  
                                    FROM DBMaster.dbo.[Users] 
                                    where UsersName in (select UserName from DBMaster.dbo.User_PhanQuyen where TableName=N'{0}' and Permission='Write'))
                                    as qryuser left join (
                                    select UserName,HanMuc 
                                    from (select UserName,HanMuc,ROW_NUMBER() OVER (PARTITION BY UserName ORDER BY Serial DESC) AS RowNum
                                    from [dbo].NvlKyDuyetHanMuc) as qryhm where RowNum=1) as qryHanMuc
                                    on qryuser.UsersName=qryHanMuc.UserName
                                    left join 
                                    (SELECT  sum(([SLDatHang]-SLHuy)*DonGia) as ThanhTien,qryduyet.UserDuyet
                                      FROM [dbo].[NvlDonDatHangItem] itemdathang
                                      inner join
                                    (SELECT [SerialLinkItem],[UserDuyet]
                                      FROM [dbo].[NvlKyDuyetItem]
                                      where TableName='NvlDonDatHang' and LoaiDuyet=N'Duyệt' and NgayInsert>=@DateBegin and NgayInsert<@DateEnd)
                                      as qryduyet on itemdathang.Serial=qryduyet.SerialLinkItem
                                      group by qryduyet.UserDuyet) as qrysudung
                                      on qryuser.UsersName=qrysudung.UserDuyet ", KyDuyet, ModelAdmin.pathurlfilepublic + "/");
                    }
                    CallAPI callAPI = new CallAPI();
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<UserDuyet>>(json);
                        lstuser.AddRange(query.OrderBy(p=>p.TenUser));
                        //if(!string.IsNullOrEmpty(kehoachmuahangcrr.NguoiDuyet)|| !string.IsNullOrEmpty(kehoachmuahangcrr.NguoiKiem))
                        //{
                        //    foreach(var item in query)
                        //    {
                        //        if(item.UsersName==kehoachmuahangcrr.NguoiDuyet)
                        //            item.chkDuyet = true;
                        //        if (item.UsersName == kehoachmuahangcrr.NguoiKiem)
                        //            item.chkKiemTra = true;
                        //    }
                        //}
                        dxGrid.Reload();
                        //Grid.Data = lstDonDatHangSearchShow;
                    }

                }
                catch (Exception ex)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));

                }
                finally
                {

                }
            }


        }
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        private async void Xacnhan()
        {
            var querycheckduyet = lstuser.Where(p => p.chkDuyet.Equals(true)).GroupBy(p => p.UsersName).Select(p => new { user = p.Key, count = p.Count() }).Where(p => p.count > 1).FirstOrDefault();
            if (querycheckduyet != null)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Người duyệt chỉ được chọn một"));

                return;
            }
            var query = lstuser.Where(p => p.chkDuyet.Equals(true) || p.chkKiemTra.Equals(true)).ToList();
            if (query.Count() == 0)
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, $"Vui lòng xác nhận người ký duyệt"));
                return;
            }
            DataTable dtsave = new DataTable();
            dtsave.Columns.Add("Serial", typeof(int));
            dtsave.Columns.Add("SerialLink", typeof(int));
            dtsave.Columns.Add("TableName", typeof(string));
            dtsave.Columns.Add("UserDuyet", typeof(string));
            dtsave.Columns.Add("LoaiDuyet", typeof(string));
            dtsave.Columns.Add("NgayApDung", typeof(DateTime));
            dtsave.Columns.Add("NgayKyDuyet", typeof(DateTime));
            dtsave.Columns.Add("UserYeuCau", typeof(string));
            string tablename = "";
            int serial = 0;
            if (kehoachmuahangcrr != null)
            {
                tablename = "NvlKehoachMuaHang";
                serial= kehoachmuahangcrr.Serial;
            }
            if (nVLDonDatHangShowcrr != null)
            {
                tablename = "NvlDonDatHang";
                serial = nVLDonDatHangShowcrr.Serial;
            }
            if(nvlDuyetGiaShowcrr!=null)
            {
                tablename = "NvlDuyetGia";
                serial = nvlDuyetGiaShowcrr.Serial;
            }
           
            foreach (var it in query)
            {
                DataRow dataRow = dtsave.NewRow();
               
                dataRow["SerialLink"] = serial;
                dataRow["TableName"] = tablename;
                dataRow["UserDuyet"] = it.UsersName;
                dataRow["NgayApDung"] = DateTime.Now;
                dataRow["NgayKyDuyet"] = DateTime.Now;
                dataRow["LoaiDuyet"] = (it.chkDuyet) ? "Duyệt" : "Kiểm tra";
                dataRow["UserYeuCau"] = ModelAdmin.users.UsersName;
                //if ((it.chkDuyet))
                //{
                //    if (kehoachmuahangcrr != null)
                //        kehoachmuahangcrr.NguoiDuyet = it.UsersName;
                //    if (nVLDonDatHangShowcrr != null)
                //        nVLDonDatHangShowcrr.Nguoi
                //}
                //else
                //{
                //    if (kehoachmuahangcrr != null)
                //        kehoachmuahangcrr.NguoiKiem = it.UsersName;
                //}
                dtsave.Rows.Add(dataRow);
            }
            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlKyDuyet_InsertTable_Ver1";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();

            lstpara.Add(new ParameterDefine("@SerialLink", serial));
            lstpara.Add(new ParameterDefine("@TableName", tablename));
            lstpara.Add(new ParameterDefine("@Type_NvlKyDuyet", prs.ConvertDataTableToJson(dtsave), "DataTable"));
            //string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
            string json = await callAPI.ProcedureEncryptMsgAsync(sql, lstpara,StaticClass.topickyduyet,"kyduyet");//ID="kyduyet" này sẽ dùng để nhận dạng trong HÀm OnMessageReceived khi có kết quả trả về từ máy chủ sẽ có ID này
            if (json != "")
            {
                var query1 = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                if (query1[0].ketqua == "OK")
                {
                    ToastService.Notify(new ToastMessage(ToastType.Success, $"Xác nhận thành công"));
                    if (GotoMainForm.HasDelegate)
                    {
                        await GotoMainForm.InvokeAsync(kehoachmuahangcrr);
                    }
                    if (GotoMainFormDH.HasDelegate)
                    {
                        await GotoMainFormDH.InvokeAsync(nVLDonDatHangShowcrr);
                    }
                    if(GotoMainFormDG.HasDelegate)
                    {
                        await GotoMainFormDG.InvokeAsync(nvlDuyetGiaShowcrr);
                    }
                }
                else
                {
                    ToastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + query1[0].ketqua));

                }

                try
                {

                    //List<string> lstusersendmqtt = new List<string>();
                    //foreach (var it in query)
                    //{
                    //    lstusersendmqtt.Add(it.UsersName);
                    //}
                    //sendmsgMQTT(lstusersendmqtt);
                    //lstusersendmqtt.Clear();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Lỗi khi gửi tin nhắn MQTT: " + ex.Message);
                }
                //Grid.Data = lstDonDatHangSearchShow;
            }
            dtsave.Dispose();
        }
        private async void sendmsgMQTT(List<string>lstuser)
        {
            if(ModelAdmin.mainLayout.mqttService!= null)
            {
                if(!ModelAdmin.mainLayout.mqttService._mqttClient.IsConnected)
                {
                    await ModelAdmin.mainLayout.mqttService.ConnectAsync();

                }
                if(ModelAdmin.mainLayout.mqttService._mqttClient.IsConnected)
                {
                   await ModelAdmin.mainLayout.mqttService.PublishAsync(StaticClass.topickyduyet,"Bạn có đề nghị cần Kiểm tra hoặc duyệt",ModelAdmin.users.UsersName,lstuser);
                }
            }
        }
        private async void showSetHamMuc()
        {
            if(await phanQuyenAccess.CreateHanMucThanhToan(ModelAdmin.users))
            {
                renderFragment = builder =>
                {
                    builder.OpenComponent<Page_HanMucDuyetDonHang>(0);
                    //builder.AddAttribute(1, "nVLDonDatHangcrr", nVLDonDatHangShow);
                    // builder.AddAttribute(2, "CallBackAfterSave", EventCallback.Factory.Create<int>(this, SearchDeNghi));
                    builder.CloseComponent();
                };

                dxPopup.showAsync("set hạn mức");
                dxPopup.ShowAsync();
            }
           else
            {
                ToastService.Notify(new ToastMessage(ToastType.Warning, "Bạn không có quyền chỉnh sửa hạn mức"));
            }
        }
        private void sendMsgSignalR()
        {   
            
        }


    }
}
