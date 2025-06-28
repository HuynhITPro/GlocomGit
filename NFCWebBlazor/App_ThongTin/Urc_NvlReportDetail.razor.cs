using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Data;

namespace NFCWebBlazor.App_ThongTin
{
    public partial class Urc_NvlReportDetail
    {
        [Inject] ToastService toastService { get; set; }
        public class NvlReportDetailShow
        {
            public int Serial { get; set; }

            public int? SerialLink { get; set; }
            public int? STT { get; set; }
            public string TableName { get; set; }
            public string ShowNameRp { get; set; }


            public string NameRp { get; set; }

            public string TextRp { get; set; }

            public string NameReport { get; set; }

            public string UserInsert { get; set; }
            public NvlReportDetailShow CopyClass()
            {
                var json = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<NvlReportDetailShow>(json);
            }
        }
        DataTable dtsave { get; set; }
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();
        protected override async Task OnInitializedAsync()
        {
            if (lstdata != null)
            {
                dxGrid.Reload();
                return;
            }
            if (nvlReportDetailShowcrr != null)
            {
                string sql = "";
                try
                {
                    sql = string.Format(@"Use NVLDB
                        declare @SerialLink int={0}
                        declare @TableName nvarchar(100)='{1}'
	                    declare @NameReport nvarchar(100)='{2}'
                       
                        declare @CheckCount int
						select top 1 @CheckCount=count(*) from [dbo].[NvlReportDetail]  where SerialLink=@SerialLink and TableName=@TableName and NameReport=@NameReport
						if(@CheckCount=0)
						begin
							set @SerialLink=0
						end
						SELECT  *
                         FROM [dbo].[NvlReportDetail] where SerialLink=@SerialLink and TableName=@TableName and NameReport=@NameReport
                        order by STT
                    ", nvlReportDetailShowcrr.Serial, nvlReportDetailShowcrr.TableName, nvlReportDetailShowcrr.NameReport);
                    CallAPI callAPI = new CallAPI();
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<NvlReportDetailShow>>(json);
                        if (query != null)
                        {
                            lstdata = query;
                        }
                        dxGrid.Reload();
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message}"));
                    Console.Error.WriteLine(ex.ToString());
                }
                finally
                {

                    //Grid.Reload();
                    StateHasChanged();
                }
            }


        }
        private async Task saveAsync()
        {
            if (lstdata.Count == 0)
                return;
            CallAPI callAPI = new CallAPI();
            string sql = "";
            string json = "";
            if (dtsave == null)
            {
                sql = string.Format(@"use NVLDB 
                declare @Type_NvlReportDetail Type_NvlReportDetail

                insert into @Type_NvlReportDetail(Serial)
                values(1)
                select * from @Type_NvlReportDetail");
                List<ParameterDefine> lstparadt = new List<ParameterDefine>();
                json = await callAPI.ExcuteQueryEncryptAsync(sql, lstparadt);
                if (json == "")
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi load tablesave"));

                }

                dtsave = JsonConvert.DeserializeObject<DataTable>(json);
            }
            dtsave.Clear();
            foreach (var it in lstdata)
            {
                DataRow rownew = dtsave.NewRow();
                rownew["Serial"] = 0;
                rownew["STT"] = it.STT;
                rownew["SerialLink"] = nvlReportDetailShowcrr.SerialLink;
                rownew["TableName"] = it.TableName;
                rownew["NameRp"] = it.NameRp;
                rownew["ShowNameRp"] = it.ShowNameRp;
                rownew["TextRp"] = it.TextRp;
                rownew["NameReport"] = it.NameReport;
                rownew["UserInsert"] = Model.ModelAdmin.users.UsersName;
                dtsave.Rows.Add(rownew);

            }

            sql = "NVLDB.dbo.NvlReportDetail_Insert";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Type_NvlReportDetail", prs.ConvertDataTableToJson(dtsave), "DataTable"));
            lstpara.Add(new ParameterDefine("@SerialLink", nvlReportDetailShowcrr.SerialLink));
            lstpara.Add(new ParameterDefine("@NameReport", nvlReportDetailShowcrr.NameReport));
            json = "";
            json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
            if (json != "")
            {
                var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                if (query[0].ketqua == "OK")
                {
                    lstdata.Clear();
                    toastService.Notify(new(ToastType.Success, "Lưu thành công"));
                    StateHasChanged();
                }
                else
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Đã xảy ra lỗi"));
                }
            }
            else
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, "Đã xảy ra lỗi"));
            }
        }

    }
}
