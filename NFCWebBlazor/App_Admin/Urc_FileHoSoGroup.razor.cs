using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;


namespace NFCWebBlazor.App_Admin
{
    public partial class Urc_FileHoSoGroup
    {
        [Inject] IJSRuntime JS { get; set; }
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        [Parameter]
        public string tableName { get; set; }
        [Parameter]
        public int serialLink { get; set; }

        [Parameter]
        public EventCallback<List<FileHoSoGroup>> GotoMainForm { get; set; }
       
        [Parameter]
        public List<FileHoSoGroup> lstdata { get; set; }
        [Inject] ToastService ToastService { get; set; } = default!;

       

        private async Task<List<FileHoSoGroup>> loaddata()
        {

            if (lstdata == null)
            {
                lstdata = new List<FileHoSoGroup>();
                try
                {
                    loadfileagain();
                }
                catch (Exception ex)
                {
                    ToastService.Notify(new ToastMessage(ToastType.Success, $"Lỗi: " + ex.Message));
                }
                finally
                {
                    //PreloadService.Hide();

                }
            }
           

            return lstdata;

        }
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                if (lstdata != null)
                {
                    Console.WriteLine("Render first cái page up file");
                    Grid.Reload();
                }
            }
            
        }
       
        private async void loadfileagain()
        {
            if (tableName != null && serialLink != null)
            {


                string sql = string.Format(@"
                                declare @TableName nvarchar(100)=N'{0}'
                                declare @SerialLink int={1}
                                select isnull(item.Serial,0) as Serial,filehosogroup.Serial as SerialGroup,item.DienGiai,item.DungLuong,item.DVT,filehosogroup.NoiDung,Users.TenUser as TenUserUpload,
                                item.UrlFile,item.UserInsert,item.TenFile,'{2}'+ISNULL(Users.PathImg,'UserImage/user.png') as PathImgUser from 
                                (
								select max(Serial) as Serial,max([SerialLink]) as [SerialLink],NoiDung
								from
								(SELECT  [Serial],[SerialLink],[NoiDung]
                                  FROM [dbo].[NvlFile_AttachGroup] 
								  where TableName=@TableName and SerialLink=@SerialLink
								  union all
								  SELECT  0 as [Serial],0 as SerialLink,FullName
								  from DataDropDownList where TypeName=@TableName) as qry
								  group by NoiDung) 
								  as filehosogroup
                                  left join (select * from dbo.FileHoSo where TableName='NvlFile_AttachGroup')  as item 
                                  on filehosogroup.Serial=item.SerialLink 
                                  left join dbo.Users on item.UserInsert=Users.UsersName
                        ", tableName, serialLink, Model.ModelAdmin.pathurlfilepublic + "/");

                //try
                //{
                CallAPI callAPI = new CallAPI();
                string json = await callAPI.ExcuteQueryEncryptAsync(sql, new List<ParameterDefine>());
                if (json != "")
                {
                    if (lstdata == null)
                        lstdata = new List<FileHoSoGroup>();
                    else
                        lstdata.Clear();

                   
                    var query = JsonConvert.DeserializeObject<List<FileHoSoGroup>>(json);
                    lstdata.AddRange(query);
                    Grid.Reload();

                    if (GotoMainForm.HasDelegate)
                    {
                        await GotoMainForm.InvokeAsync(lstdata);
                    }
                 
                    //await GotoMainForm.InvokeAsync();
                }
                //}
                //catch (Exception ex)
                //{
                //    ToastService.Notify(new ToastMessage(ToastType.Success, $"Lỗi: " + ex.Message));
                //}
                //finally
                //{


                //}

            }
        }

        protected override async Task OnInitializedAsync()
        {
            await loaddata();
            // base.OnInitialized();
        }

        public void refreshdata()
        {
            loadfileagain();
        }
        private string showtextdouble(double? d)
        {
            if (d == null)
                return "";
            if (d == 0)
                return "";
            return d.Value.ToString("#,0.####");
        }



        private async void btuploadfile(FileHoSoGroup fileHoSoGroup)
        {
            FileHoSoAPI fileHoSo = new FileHoSoAPI();
            fileHoSo.TableName = "NvlFile_AttachGroup";
            fileHoSo.SerialLink = 0;//Không cần thiết. vì trong procedure của API sẽ xử lý để lấy ra SerialLink của bảng NvlFile_AttachGroup
            fileHoSo.UserInsert = Model.ModelAdmin.users.UsersName;
            fileHoSo.Serial = 0;

            //3 biến này là bắt buộc, vì để procedure kiểm tra và xử lý để lấy SerialLink
            fileHoSo.SerialRoot = serialLink;
            fileHoSo.TableNameRoot = tableName;
            fileHoSo.NoiDungRoot = fileHoSoGroup.NoiDung;

            NvlFile_AttachGroup nvlFile_AttachGroup = new NvlFile_AttachGroup();
            nvlFile_AttachGroup.Serial = fileHoSoGroup.Serial;
            //3 biến SerialLink,TableName,NoiDung dùng để procedure kiểm tra xem có data ở bảng NvlFile_AttachGroup hay chưa, nếu chưa có sẽ tạo mới dòng
            nvlFile_AttachGroup.SerialLink = serialLink;//Dùng để liên kết với ID parametter của chứng từ gốc
            nvlFile_AttachGroup.TableName = tableName;//TableName là table gốc của chứng từ
            nvlFile_AttachGroup.NoiDung = fileHoSoGroup.NoiDung;//Dùng để truyền vào parametter của proceducer để kiểm tra xem có tạo SerialLink này ở bảng NvlFile_AttachGroup này ở database hay chưa

            //fileHoSoAPI.SerialLink=
            renderFragment = builder =>
            {
                builder.OpenComponent<UploadDocument>(0);
                builder.AddAttribute(1, "fileHoSo", fileHoSo);
                builder.AddAttribute(2, "nvlFile_AttachGroup", nvlFile_AttachGroup);
                builder.AddAttribute(2, "folderName", tableName);
                builder.AddAttribute(3, "GotoMainForm", EventCallback.Factory.Create(this, refreshdata));
                //builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            };
            dxPopup.showAsync("Thêm file");

            await  dxPopup.ShowAsync();

        }
        private async void savefile(FileHoSoGroup fileHoSoGroup)
        {
            // Console.WriteLine(ModelAdmin.pathhostfile + "/" + fileHoSoNhapXuat.UrlFile);

            FTPFile fTPFile = new FTPFile();
            await fTPFile.DownloadFile(JS, Path.Combine(ModelAdmin.pathhostpublic, fileHoSoGroup.UrlFile), ModelAdmin.userhost, ModelAdmin.passwordhost, fileHoSoGroup.TenFile);


        }
        private async void deleteDeletefile(FileHoSoGroup fileHoSoGroup)
        {


            if (phanQuyenAccess.CheckDelete(fileHoSoGroup.UserInsert, ModelAdmin.users))
            {
                bool ketqua = await dialogMsg.Show($"XÓA FILE???", $"Bạn có chắc muốn xóa FILE {fileHoSoGroup.TenFile}??");
                if (ketqua)
                {

                    if (fileHoSoGroup.Serial != 0)
                    {

                        FileHoSoAPI fileHoSoAPI = new FileHoSoAPI();
                        fileHoSoAPI.UrlFile = fileHoSoGroup.UrlFile;
                        fileHoSoAPI.TenFile = fileHoSoGroup.TenFile;

                        fileHoSoAPI.SerialLink = fileHoSoGroup.SerialGroup;
                        fileHoSoAPI.Serial = fileHoSoGroup.Serial;
                        CallAPI callAPI = new CallAPI();
                        string ketquamsg = await callAPI.DeleteFileHoSoEncrypt(fileHoSoAPI);
                        if (ketquamsg.Contains("OK"))
                        {
                            ToastService.Notify(new ToastMessage(ToastType.Success, $"Xóa file thành công"));
                            //kiểm tra xem có phải dòng cuối cùng không
                            var querycheck = lstdata.Where(p => p.NoiDung == fileHoSoGroup.NoiDung).Count();
                            if (querycheck <= 1)//Dòng cuối cùng
                            {
                                fileHoSoGroup.Serial = 0;
                                fileHoSoGroup.TenFile = null;
                                fileHoSoGroup.UrlFile = null;
                                fileHoSoGroup.DienGiai = null;
                            }
                            else
                                lstdata.Remove(fileHoSoGroup);
                            Grid.Reload();
                            if (GotoMainForm.HasDelegate)
                            {
                                await GotoMainForm.InvokeAsync(lstdata);
                            }
                        }
                    }
                }
            }
            else
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Bạn không có quyền xóa file"));

        }
    }
}
