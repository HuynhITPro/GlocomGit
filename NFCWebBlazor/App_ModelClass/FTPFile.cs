using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using NFCWebBlazor.Model;

namespace NFCWebBlazor.App_ModelClass
{
    public class FTPFile
    {
       
        public  async Task DownloadFile(IJSRuntime JS,string ftpUrl, string username, string password,string fileName)
        {
            //var ftpUrl = "ftp://yourftpserver.com/path/to/file";
            //var username = "yourUsername";
            //var password = "yourPassword";

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            var ftpRequest = new
            {
                FtpUrl = ftpUrl,
                Username = username,
                Password = password
            };

            var jsonRequest = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(ftpRequest),
                System.Text.Encoding.UTF8,
                "application/json");

            //API download này viết sẵn ở trên API FileController
            var response = await httpClient.PostAsync("api/file/download", jsonRequest);

            if (response.IsSuccessStatusCode)
            {
                var base64 = Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync());

                await JS.InvokeVoidAsync("saveAsFile", fileName, base64);
              
            }
        }
     
    }
}
