using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net;
using NFCWebBlazor.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using NFCWebBlazor.Shared;
using System.Collections.ObjectModel;
using NFCWebBlazor.App_ModelClass;

namespace NFCWebBlazor.Model
{
    public class ModelAdmin
    {
        public static string IPServer = "123.31.41.25";
        public static string HostName = "scansia.ddns.net";
        
        public static string urlAPI = "https://scansia.ddns.net:8006";

      

        public static string pathhostsoft = string.Format("ftp://{0}:220/app", IPServer);
        public static string pathhostfileexample = string.Format("ftp://{0}:220/appdoc", IPServer);
        public static string pathhostpublic = string.Format("ftp://{0}:220/", IPServer);//Port 220 set cho FTP
        public static string pathurlfilepublic = string.Format("https://{0}:8003/", HostName);//Port 8003, set public sẵn
        public static string pathurlimgpublic = string.Format("https://{0}:8004/", HostName);//Port 8004, set public sẵn, vì cái này tham chiếu thẳng đến thư viện ảnh luôn, nên sẽ replace cái document/NvlHangHoa phía trước
        public static string pathurlfilepublicAPI = string.Format(Path.Combine(string.Format("https://{0}:8003/", HostName)));

        public static string pathhostdocumnet = string.Format("ftp://{0}:220/document/", IPServer);
       
        public static bool isMobile = false;

        public static string useNVLDB = "Use [NVLDB]";

        public static string userhost = "";
        public static string passwordhost = "";
       
        public static Users users;
        public static MainLayout mainLayout;
      
        public static string MaKhoSelected = "";
        public static string PhanLoaiHang = "";
       
        public static string PhanLoaiNoiGN = "";
        public static string ConnectionID ="";
        public static List<NvlHangHoaDropDown> lsthanghoafilter { get; set; } = new List<NvlHangHoaDropDown>();
        public static List<DataDropDownList> lstnoigiaonhanfilter { get; set; } = new List<DataDropDownList>();
        public static bool CreateFTPDirectory(string directory)
        {
            try
            {
                //create the directory
                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(directory));
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = new NetworkCredential(userhost, passwordhost);
                requestDir.UsePassive = true;
                requestDir.UseBinary = true;
                requestDir.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
            }
        }
        public static object ConvertjsontoClass(string s)
        {
            return JsonConvert.DeserializeObject<object>(s);
        }
        public int windowHeight, windowWidth;
        public static WindowDimensions windowDimensions;
        public static string pageclickcurrent { get; set; } = "";
        public static List<MenuItem> lstmenuitems { get; set; }
        public static string Encrypt(string plainText)
        {
            string key = "huynhit1111111111111111111111111";
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);

            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/ECB/PKCS7Padding");
            cipher.Init(true, new KeyParameter(keyBytes));

            byte[] outputBytes = cipher.DoFinal(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

    }
    public class WindowDimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

