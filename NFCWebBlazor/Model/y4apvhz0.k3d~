using Newtonsoft.Json;
using NFCWebBlazor.Models;
using static System.Net.WebRequestMethods;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using DevExpress.XtraPrinting;
using NFCWebBlazor.App_ModelClass;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace NFCWebBlazor.Model
{
    public class CallAPI
    {

        
        //HttpClient httpClient { get; set; } = new HttpClient();
       
        //ExcuteQuery
        public async Task<string> ExcuteQueryAsync(string sql,List<ParameterDefine> lstpara)
        {
            HttpClient httpClient=new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            string str = "";
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.sql= sql;
            if (lstpara == null)
                getDataFromSql.json = "";
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.PostAsync("/api/Admin/ExcuteQuery", content);
            
            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();
                str = article;
                //str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str= "";

            }
            if(str=="[]")
                str="";
            httpClient.Dispose();
            return str;
        }
        public async Task<string> ExcuteQueryEncryptAsync(string sql, List<ParameterDefine> lstpara)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            string str = "";
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.sql =ModelAdmin.Encrypt(sql);
            if (lstpara == null)
                getDataFromSql.json = "";
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/Admin/ExcuteQueryEncrypt", content);

            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();
                str = article;
                //str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str = "";

            }
            if (str == "[]")
                str = "";
            httpClient.Dispose();
            return str;
        }
        public async Task<string> ExcuteQueryEncryptMsgAsync(string sql, List<ParameterDefine> lstpara,string topic, string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(topic))
            {

                return "Vui lòng nhập topic và id";
            }
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            string str = "";
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.sql = ModelAdmin.Encrypt(sql);
            if (lstpara == null)
                getDataFromSql.json = "";

            getDataFromSql.id = id;
            getDataFromSql.topic = topic;

            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
        
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/Admin/ExcuteQueryEncrypt", content);

            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();
                str = article;
                //str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str = "";

            }
            if (str == "[]")
                str = "";
            httpClient.Dispose();
            return str;
        }

        public async Task<string> ExcuteQueryDatasetEncrypt(string sql, List<ParameterDefine> lstpara)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            string str = "";
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.sql = ModelAdmin.Encrypt(sql);
            if (lstpara == null)
                getDataFromSql.json = "";
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/Admin/ExcuteQueryDatasetEncrypt", content);

            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();
                str = article;
                //str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str = "";

            }
            if (str == "[]")
                str = "";
            httpClient.Dispose();
            return str;
        }
        public async Task<string> ExcuteProcedureDatasetEncrypt(string sql, List<ParameterDefine> lstpara)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            string str = "";
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.sql = ModelAdmin.Encrypt(sql);
            if (lstpara == null)
                getDataFromSql.json = "";
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/Admin/JsonFromSqlProcedureDatasetEncrypt", content);

            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();
                str = article;
                //str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str = "";

            }
            if (str == "[]")
                str = "";
            httpClient.Dispose();
            return str;
        }

        public async Task<string> ExcuteQueryDatasetMsgEncrypt(string sql, List<ParameterDefine> lstpara, string topic, string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(topic))
            {

                return "Vui lòng nhập topic và id";
            }
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            string str = "";
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.sql = ModelAdmin.Encrypt(sql);
            if (lstpara == null)
                getDataFromSql.json = "";
            getDataFromSql.id = id;
            getDataFromSql.topic = topic;
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/Admin/ExcuteQueryDatasetEncrypt", content);

            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();
                str = article;
                //str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str = "";

            }
            if (str == "[]")
                str = "";
            httpClient.Dispose();
            return str;
        }
        //JsonFromSqlProcedureWithException
        public async  Task<string> ProcedureAsync(string sql, List<ParameterDefine> lstpara)
        {
            string str = "";
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.sql = sql;
            if (lstpara == null)
                getDataFromSql.json = "";
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/Admin/JsonFromSqlProcedureWithException", content);

            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();
                str = article;
                //str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str= "";
            }
            httpClient.Dispose();
            return str;
        }
        public async Task<string> ProcedureEncryptAsync(string sql, List<ParameterDefine> lstpara)
        {
            string str = "";
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.sql = ModelAdmin.Encrypt(sql);
            if (lstpara == null)
                getDataFromSql.json = "";
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);
           
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/Admin/JsonFromSqlProcedureWithExceptionEncrypt", content);

            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();
                str = article;
                //str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str = "";
            }
            httpClient.Dispose();
            return str;
        }
        public async Task<string> ProcedureEncryptMsgAsync(string sql, List<ParameterDefine> lstpara,string topic, string id)
        {
            string str = "";
            if(string.IsNullOrEmpty(id)||string.IsNullOrEmpty(topic))
            {
                
                return "Vui lòng nhập topic và id";
            }
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            getDataFromSql.sql = ModelAdmin.Encrypt(sql);
            if (lstpara == null)
                getDataFromSql.json = "";
            getDataFromSql.id = id;
            getDataFromSql.topic = topic;
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/Admin/JsonFromSqlProcedureWithExceptionEncrypt", content);
        
            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();
                str = article;
                //str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str = "";
            }
            httpClient.Dispose();
            return str;
        }
        ///api/admin/UploadFileImg
        public async Task<byte[]?> JsonFromSqlExportPdfEncrypt(GetDataFromSql getDataFromSql)
        {
            byte[]? pdfBytes=null;
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            string str = "";
           
            getDataFromSql.sql = ModelAdmin.Encrypt(getDataFromSql.sql);
           
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/Admin/ExportToPdf", content);

           
            
            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                
                //var article = await response.Content.ReadAsStringAsync();

                 pdfBytes = await response.Content.ReadAsByteArrayAsync();
              
            }
            else
            {
              string s=  await response.Content.ReadAsStringAsync();

            }
           
            httpClient.Dispose();
            return pdfBytes;
        }
        public async  Task<string> UploadFileAsync(IFormFile file, string name)
        {
            string str = "";
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StreamContent(file.OpenReadStream())
                {
                    Headers =
                {
                    ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "\"file\"",
                        FileName = "\"" + file.FileName + "\""
                    }
                }
                }, "file");
                formData.Add(new StringContent(name), "name");
                // Handle the response if needed
               
               
                var response = await httpClient.PostAsync("/api/admin/UploadFileImg", formData);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    object ob = response;
                    var article = await response.Content.ReadAsStringAsync();
                    str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                    // Xử lý phản hồi khi yêu cầu thành công
                }
                else
                {
                    str = "";
                }
                httpClient.Dispose();
                return str;
              
            }
        }


        ///api/admin/DeleteFileHoSoEncrypt
        public async Task<string> DeleteFileHoSoEncrypt(FileHoSoAPI fileHoSoAPI)
        {
            string str = "";
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            string sql = System.Text.Json.JsonSerializer.Serialize(fileHoSoAPI);
            getDataFromSql.sql = ModelAdmin.Encrypt(sql);
            getDataFromSql.json = "";
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/Admin/DeleteFileHoSoEncrypt", content);

            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();
                str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str = "";
            }
            httpClient.Dispose();
            return str;

        }
        public async Task<MaSoThueAPI> GetBusinessInfoAsync(string businessId)
        {
            HttpClient httpClient = new HttpClient();
            MaSoThueAPI maSoThueAPI = new MaSoThueAPI();
            try
            {
                string url = $"https://api.vietqr.io/v2/business/{businessId}";

                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(json))
                    {
                        maSoThueAPI = JsonConvert.DeserializeObject<MaSoThueAPI>(json);

                    }



                }
                else
                {
                    throw new Exception($"API call failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: API MST" + ex.ToString());
            }
            return maSoThueAPI;


        }
        ///api/admin/GetChartEncrypt
        public async Task<string> GetChartEncrypt(string sql)
        {

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            string str = "";
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            getDataFromSql.sql = ModelAdmin.Encrypt(sql);
            if (lstpara == null)
                getDataFromSql.json = "";
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/Admin/getchart", content);

            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();

                str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str = "";

            }
            if (str == "[]")
                str = "";
            httpClient.Dispose();
            return str;

        }

        ///api/admin/GetChartEncrypt
        public async Task<string> Gettonmatbang(string sql)
        {

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            string str = "";
            GetDataFromSql getDataFromSql = new GetDataFromSql();
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            getDataFromSql.sql = ModelAdmin.Encrypt(sql);
            if (lstpara == null)
                getDataFromSql.json = "";
            getDataFromSql.json = System.Text.Json.JsonSerializer.Serialize(lstpara);
            var json = System.Text.Json.JsonSerializer.Serialize(getDataFromSql);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/Admin/gettonmatbang", content);

            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();

                str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str = "";

            }
            if (str == "[]")
                str = "";
            httpClient.Dispose();
            return str;

        }

        public async Task<string> SendMsgJson(JsonMsgAndroid jsonMsgAndroid)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            string str = "";
            var json = jsonMsgAndroid.ToJson();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
          
            var response = await httpClient.PostAsync("/api/Chat/SendMsgJson", content);
            if (response.IsSuccessStatusCode)
            {
                object ob = response;
                var article = await response.Content.ReadAsStringAsync();
                str = article;
                //str = Regex.Unescape(article).Replace("\"[", "[").Replace("]\"", "]");
                // Xử lý phản hồi khi yêu cầu thành công
            }
            else
            {
                str = "";

            }
            if (str == "[]")
                str = "";
            httpClient.Dispose();
            return str;
        }

    }
}
