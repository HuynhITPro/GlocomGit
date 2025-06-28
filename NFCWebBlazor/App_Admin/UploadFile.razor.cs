using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static NFCWebBlazor.App_Admin.UploadFile;

namespace NFCWebBlazor.App_Admin
{
    public partial class UploadFile
    {
        [Inject] ToastService toastService { get; set; }
        [Parameter]
        public FileHoSoAPI? fileHoSo { get; set; }
        [Parameter]
        public string FolderName { get; set; }

        DxGrid Grid;

        public string urlapi { get; set; } = "/api/Admin/Uploadfilewithgroup";
        string diengiai = "";
        int SelectedFilesCount { get; set; }
        IEnumerable<UploadFileInfo> ListFileinfo { get; set; }
        List<UploadFileInfoExtend> lstdata { get; set; } = new List<UploadFileInfoExtend>();
        public class UploadFileInfoExtend : INotifyPropertyChanged
        {
            public string Status { get; set; }
            public string TextcolorStatus { get; set; }
            private bool? _uploadStatus { get; set; }
            private bool _showProgress { get; set; }

            public bool ShowProgress
            {
                get { return _showProgress; }
                set
                {
                    _showProgress=value;
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
            public UploadFileInfo uploadFileInfo { get; set; }
        }
        protected void SelectedFilesChanged(IEnumerable<UploadFileInfo> files)
        {
            SelectedFilesCount = files.ToList().Count;
            ListFileinfo = files;

            bool check = false;
            //Kiểm tra file chưa có
            foreach (UploadFileInfo uploadFileInfo in ListFileinfo)
            {
                check = false;
                foreach (var it in lstdata)
                {
                    if (uploadFileInfo.Guid == it.uploadFileInfo.Guid)
                    {
                        check = true;
                        break;
                    }
                }
                if (!check)
                {
                    UploadFileInfoExtend uploadFileInfoExtend = new UploadFileInfoExtend();
                    UploadFileInfo uploadFileInfoadd = new UploadFileInfo();
                    uploadFileInfoadd.Name = uploadFileInfo.Name;
                    uploadFileInfoadd.LoadedBytes = uploadFileInfo.LoadedBytes;
                    uploadFileInfoadd.Guid = uploadFileInfo.Guid;
                    uploadFileInfoadd.Size = uploadFileInfo.Size;
                    uploadFileInfoadd.Type = uploadFileInfo.Type;
                    uploadFileInfoadd.LastModified = uploadFileInfo.LastModified;
                    uploadFileInfoExtend.uploadFileInfo = uploadFileInfoadd;
                    lstdata.Add(uploadFileInfoExtend);

                }
            }
            //Xóa file không tồn tại
            if (ListFileinfo.Count() == 0)
                lstdata.Clear();
            //for (int i = lstdata.Count() - 1; i >= 0; i--)
            //{
            //    check = false;
            //    UploadFileInfoExtend uploadFileInfoExtend = lstdata[i];
            //    foreach (UploadFileInfo uploadFileInfo in ListFileinfo)
            //    {
            //        if (uploadFileInfoExtend.uploadFileInfo.Guid == uploadFileInfo.Guid)
            //        {
            //            check = true;
            //            break;
            //        }
            //    }
            //    if (!check)
            //    {
            //        lstdata.Remove(uploadFileInfoExtend);
            //    }
            //}
            //Grid.Reload();
            InvokeAsync(StateHasChanged);
        }
        DxFileInput? dxFileInput;
        private string settext;
        List<IFileInputSelectedFile> lstfileinput = new List<IFileInputSelectedFile>();
        protected async Task OnFilesUploading(FilesUploadingEventArgs args)
        {
            List<string> lsterr = new List<string>();
            fileHoSo = new FileHoSoAPI();
            FolderName = "";
            fileHoSo.Serial = 0;
            foreach (IFileInputSelectedFile? file in args.Files)
            {
                Console.WriteLine(file.Name);

                // using var stream = new System.IO.MemoryStream();
                //await file.OpenReadStream(file.Size).CopyToAsync(stream);
                lstfileinput.Add(file);

                bool check = await UploadFileAsync(urlapi, FolderName, file);

            }
            //Console.WriteLine("Xong vòng lặp");

            StateHasChanged();
        }

        private async Task<bool> UploadFileAsync(string apiUrl, string folderName, IFileInputSelectedFile? file)
        {
            var queryfile = lstdata.Where(p => p.uploadFileInfo.Guid == file.Guid).FirstOrDefault();
            if (queryfile == null)
                return false;
            queryfile.ShowProgress = true;
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ModelAdmin.urlAPI);

            using var form = new MultipartFormDataContent();
            Stream stream = new System.IO.MemoryStream();
            await file.OpenReadStream(file.Size).CopyToAsync(stream);

            // Thêm nội dung tệp vào form data
            var fileContent = new StreamContent(stream);


            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.Type);

            form.Add(fileContent, "file", file.Name);
            // Thêm chuỗi string vào form data
            form.Add(new StringContent(folderName), "foldername");


            fileHoSo.TenFile = file.Name;
            fileHoSo.DungLuong = (double)(file.Size) / (1024.0 * 1024.0); ;
            fileHoSo.DienGiai = diengiai;
            //fileHoSo.UrlFile = urlfilehost;

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

                    if (queryfile != null)
                        queryfile.UploadStatus = true;
                    return true;
                    // status += "\nFile được gửi thành công: " + selectedFile.Name;
                }
                else
                {
                    stream.Close();
                    stream.Dispose();
                    //var query = ListFileinfo.Where(p => p.Guid == file.Guid).FirstOrDefault();


                    if (queryfile != null)
                        queryfile.UploadStatus = false;
                    return false;
                }
                //status += $"\n{selectedFile.Name} bị lỗi:{responseContent}";
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
        private void onReloadClick(UploadFileInfo uploadFileInfo)
        {

            if (uploadFileInfo != null)
                dxFileInput.ReloadFile(uploadFileInfo);

        }
        private void onRemoveButtonClick(UploadFileInfoExtend uploadFileInfoExtend)
        {
            if (uploadFileInfoExtend != null)
            {
                uploadFileInfoExtend.UploadStatus = null;

                dxFileInput.RemoveFile(uploadFileInfoExtend.uploadFileInfo);
                var queryitem = lstdata.Where(p => p.uploadFileInfo.Guid == uploadFileInfoExtend.uploadFileInfo.Guid).FirstOrDefault();
                lstdata.Remove(queryitem);
            }

        }
        async Task onUploadButtonClickAsync(UploadFileInfoExtend uploadFileInfoExtend)
        {

            if (uploadFileInfoExtend != null)
            {
                uploadFileInfoExtend.UploadStatus = null;
                var query = lstfileinput.Where(p => p.Guid == uploadFileInfoExtend.uploadFileInfo.Guid).FirstOrDefault();
                try
                {

                    await UploadFileAsync(urlapi, FolderName, query);
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
