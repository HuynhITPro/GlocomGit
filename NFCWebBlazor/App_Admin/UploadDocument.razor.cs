using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NFCWebBlazor.App_Admin
{
    public partial class UploadDocument
    {
        [Inject] ToastService toastService { get; set; }
        [Inject] IJSRuntime jSRuntime { get; set; }
        [Parameter]
        public FileHoSoAPI? fileHoSo { get; set; }//File hồ sơ này phải có mục 
        [Parameter]
        public string folderName { get; set; }

        [Parameter]
        public NvlFile_AttachGroup nvlFile_AttachGroup { get; set; }

        [Parameter]
        public EventCallback GotoMainForm { get; set; }

        DxGrid Grid;
        int numberOfInputFiles = 1;
        public string urlapi { get; set; } = "/api/Admin/Uploadfilewithgroup";
        string diengiai = "";


        List<UploadFileInfoExtend> lstdata { get; set; } = new List<UploadFileInfoExtend>();
        public class UploadFileInfoExtend : INotifyPropertyChanged
        {
            public string Id { get; set; }
            public string Status { get; set; }
            public string TextcolorStatus { get; set; }
            private bool? _uploadStatus { get; set; }
            private bool _showProgress { get; set; }

            public bool ShowProgress
            {
                get { return _showProgress; }
                set
                {
                    _showProgress = value;
                    NotifyPropertyChanged("ShowProgress");
                }
            }
            public bool? UploadStatus
            {
                get { return _uploadStatus; }
                set
                {
                    _uploadStatus = value;
                    if (_uploadStatus == null)
                    {
                        Status = "";
                        TextcolorStatus = "text-success";
                    }
                    else
                    {
                        if (_uploadStatus.Value)
                        {
                            Status = "Gửi thành công";
                            TextcolorStatus = "text-success";
                            ShowProgress = false;

                        }
                        else
                        {
                            Status = "Lỗi, chưa gửi được";
                            TextcolorStatus = "text-danger";
                            ShowProgress = false;
                        }
                    }
                    NotifyPropertyChanged("Status");
                    NotifyPropertyChanged("TextcolorStatus");

                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public IBrowserFile uploadFileInfo { get; set; }
        }
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                if (folderName == null)
                {
                    folderName = "";

                }
                if (fileHoSo == null)
                {
                    fileHoSo = new FileHoSoAPI();

                    fileHoSo.Serial = 0;
                }
            }
            base.OnAfterRender(firstRender);
        }

        string GetInputFileStyle(int index)
        {
            return index == numberOfInputFiles - 1 ? "" : "display: none";
        }
        bool isclick = true;
        string showtextall = "";
        private List<(string FileName, string Base64)> compressedImages = new();

        private double quality = 0.7; // Chất lượng nén
        private bool IsImageFile(IBrowserFile file)
        {
            var allowedMimeTypes = new HashSet<string>
    {
        "image/jpeg", "image/png", "image/gif", "image/webp", "image/bmp", "image/svg+xml"
    };

            return allowedMimeTypes.Contains(file.ContentType);
        }
        async Task OnFileChanged(InputFileChangeEventArgs e)
        {
            if (fileHoSo == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Dữ liệu truyền vào không hợp lệ"));
                return;
            }
            if (fileHoSo.TableName == null || fileHoSo.SerialLink == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Dữ liệu truyền vào không hợp lệ"));
                return;
            }
            compressedImages.Clear();
            //lstdata.Clear();
            numberOfInputFiles++;
            try
            {
                isclick = true;
                List<UploadFileInfoExtend> queryaddrange = new List<UploadFileInfoExtend>();
                foreach (IBrowserFile file in e.GetMultipleFiles(e.FileCount))
                {
                    var itcheck = lstdata.Where(p => p.uploadFileInfo.Name == file.Name).FirstOrDefault();

                    if (itcheck == null)
                    {


                        UploadFileInfoExtend uploadFileInfoExtend = new UploadFileInfoExtend();
                        uploadFileInfoExtend.Id = prs.RandomString(20);
                        uploadFileInfoExtend.uploadFileInfo = file;
                        queryaddrange.Add(uploadFileInfoExtend);

                    }


                }
                lstdata.AddRange(queryaddrange);
                showtextall = string.Format("Tải lên {0} file", lstdata.Where(p => p.UploadStatus == null || (p.UploadStatus != null && p.UploadStatus.Value == false)).Count());
            }
            catch (Exception ex)
            {
                await InvokeAsync(StateHasChanged);
            }
            finally
            {

                isclick = false;
                Grid.Reload();
                await InvokeAsync(StateHasChanged);
            }
        }
        private bool showbuttonall()
        {
            if (showtextall == "")
                return false;
            return true;
        }

        private async Task<string> ConvertToBase64Async(IBrowserFile file)
        {

            using var stream = file.OpenReadStream(20*1024 * 1024);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            byte[] bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }
        private async Task<bool> UploadFileAsync(string apiUrl, string folderName, UploadFileInfoExtend fileextend)
        {

            fileextend.ShowProgress = true;
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);
            var form = new MultipartFormDataContent();
            try
            {
                //jSRuntime.InvokeVoidAsync("nenfileimg2");
                Console.WriteLine("Đang đọc file " + fileextend.uploadFileInfo.Name);
                if (IsImageFile(fileextend.uploadFileInfo))
                {
                    //         string compressedBase64 = await jSRuntime.InvokeAsync<string>(
                    //    "nenfileimg", fileextend.uploadFileInfo, quality
                    //);
                    string base64Image = await ConvertToBase64Async(fileextend.uploadFileInfo);
                    double quality = 0.7;

                    // Gọi JavaScript để nén ảnh
                    string compressedBase64 = await jSRuntime.InvokeAsync<string>("compressImage", base64Image, fileextend.uploadFileInfo.ContentType, quality);
                    //if (!string.IsNullOrEmpty(compressedBase64))
                    //{
                    //    compressedImages.Add((fileextend.uploadFileInfo.Name, compressedBase64));
                    //}
                    var imageBytes = Convert.FromBase64String(compressedBase64);
                    using var stream = new MemoryStream(imageBytes);
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(fileextend.uploadFileInfo.ContentType);
                    form.Add(fileContent, "file", fileextend.uploadFileInfo.Name);
                    // Thêm chuỗi string vào form data
                    form.Add(new StringContent(folderName), "foldername");

                    fileHoSo.TenFile = fileextend.uploadFileInfo.Name;
                    fileHoSo.DungLuong = (double)(imageBytes.Length) / (1024.0 * 1024.0); ;
                    fileHoSo.DienGiai = diengiai;
                    var json = System.Text.Json.JsonSerializer.Serialize(fileHoSo);
                    fileHoSo.Dvt = "MB";
                    form.Add(new StringContent(json), "filehosojson");

                    // Gửi yêu cầu POST
                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, form);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        if (responseContent.Contains("done"))
                        {
                            stream.Close();
                            stream.Dispose();
                            fileextend.UploadStatus = true;
                            return true;

                        }
                        else
                        {
                            stream.Close();
                            stream.Dispose();
                            fileextend.UploadStatus = false;
                            return false;
                        }
                        //status += $"\n{selectedFile.Name} bị lỗi:{responseContent}";
                    }
                }
                else
                {
                    using (var stream = fileextend.uploadFileInfo.OpenReadStream(MaxFileSize))
                    {
                       
                        //using var form = new MultipartFormDataContent();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(fileextend.uploadFileInfo.ContentType);
                        form.Add(fileContent, "file", fileextend.uploadFileInfo.Name);
                        // Thêm chuỗi string vào form data
                        form.Add(new StringContent(folderName), "foldername");

                        fileHoSo.TenFile = fileextend.uploadFileInfo.Name;
                        fileHoSo.DungLuong = (double)(fileextend.uploadFileInfo.Size) / (1024.0 * 1024.0); ;
                        fileHoSo.DienGiai = diengiai;
                        var json = System.Text.Json.JsonSerializer.Serialize(fileHoSo);
                        fileHoSo.Dvt = "MB";
                        form.Add(new StringContent(json), "filehosojson");

                        // Gửi yêu cầu POST
                        HttpResponseMessage response = await httpClient.PostAsync(apiUrl, form);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            if (responseContent.Contains("done"))
                            {
                                stream.Close();
                                stream.Dispose();
                                fileextend.UploadStatus = true;
                                return true;

                            }
                            else
                            {
                                stream.Close();
                                stream.Dispose();
                                fileextend.UploadStatus = false;
                                return false;
                            }
                            //status += $"\n{selectedFile.Name} bị lỗi:{responseContent}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                fileextend.UploadStatus = false;
                toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi: " + ex.Message));
            }
            return false;
        }

        public string getIconImg(string Filename)
        {

            string _pathicon = "";
            if (Filename == null)
            { _pathicon = ""; }
            string extention = Path.GetExtension(Filename).ToLower();
            switch (extention)
            {
                case ".xlsx":
                    _pathicon = IconImg.xlsx;
                    break;
                case ".docx":
                    _pathicon = IconImg.docx;
                    break;
                case ".pdf":
                    _pathicon = IconImg.pdf;
                    break;
                case ".cdr":
                    _pathicon = IconImg.cdr;
                    break;
                case ".psd":
                    _pathicon = IconImg.psd;
                    break;
                case ".pptx":
                    _pathicon = IconImg.pptx;
                    break;
                case ".png":
                    _pathicon = IconImg.image;
                    break;
                case ".jpg":
                    _pathicon = IconImg.image;
                    break;
                case ".jpeg":
                    _pathicon = IconImg.image;
                    break;
                case ".bmp":
                    _pathicon = IconImg.image;
                    break;
                default:
                    _pathicon = IconImg.file;
                    break;
            }
            return _pathicon;
        }
        public string getSize(double size)
        {
            if (size < 1024.0)
            {
                return string.Format("({0} Byte)", size);
            }
            if (size < 1024.0 * 1024)
            {
                return string.Format("({0} KB)", Math.Round(size / (1024), 2));
            }
            return string.Format("({0} MB)", Math.Round(size / (1024 * 1024), 2));
        }
        public bool showUpload(bool? bl)
        {
            if (bl == null)
                return true;
            return !bl.Value;
        }
        public bool showDelete(bool? bl)
        {
            if (bl == null)
                return true;
            return !bl.Value;
        }
        public bool showReload(bool? bl)
        {
            if (bl == null)
                return false;
            return !bl.Value;
        }

        private void onRemoveButtonClick(UploadFileInfoExtend uploadFileInfoExtend)
        {
            if (uploadFileInfoExtend != null)
            {
                uploadFileInfoExtend.UploadStatus = null;
                var queryitem = lstdata.Where(p => p.Id == uploadFileInfoExtend.Id).FirstOrDefault();
                lstdata.Remove(queryitem);
                Grid.SaveChangesAsync();
                showtextbuttonall();
            }

        }
        private string showtextbuttonall()
        {
            if (lstdata.Count == 0)
                return "";
            int count = lstdata.Where(p => p.UploadStatus == null || (p.UploadStatus != null && p.UploadStatus.Value == false)).Count();
            if (count == 0)
            {
                showtextall = string.Format("Đã gửi hoàn tất");
            }
            else
                showtextall = string.Format("Gửi lên server {0} file", count);
            return showtextall;
        }
        private void onRemoveAllClick()
        {
            lstdata.Clear();
            Grid.Reload();

        }
        private async Task UploadAllAsync()
        {
            var query = lstdata.Where(p => p.UploadStatus == null || (p.UploadStatus != null && p.UploadStatus.Value == false));
            foreach (var it in query)
            {
                await UploadFileAsync(urlapi, folderName, it);
            }
            if (GotoMainForm.HasDelegate)
            {
                await GotoMainForm.InvokeAsync();
            }
        }
        async Task onUploadButtonClickAsync(UploadFileInfoExtend uploadFileInfoExtend)
        {

            if (uploadFileInfoExtend != null)
            {
                uploadFileInfoExtend.UploadStatus = null;

                try
                {

                    await UploadFileAsync(urlapi, folderName, uploadFileInfoExtend);
                    if (GotoMainForm.HasDelegate)
                    {
                        await GotoMainForm.InvokeAsync();
                    }
                }
                catch (Exception ex)
                {
                    toastService.Notify(new ToastMessage(ToastType.Warning, "Lỗi:" + ex.Message));
                    uploadFileInfoExtend.ShowProgress = false;
                }
            }
        }
    }
}

