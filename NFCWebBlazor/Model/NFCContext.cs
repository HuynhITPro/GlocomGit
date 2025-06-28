using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NFCWebBlazor.App_ClassDefine;
using static NFCWebBlazor.App_ThongTin.Page_NhaCungCapMaster;

namespace NFCWebBlazor.Models
{
    public partial class NFCNVLContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("");
                //optionsBuilder.UseSqlServer(ConnectionService.connstring);
            }
        }
       
    }
    public partial class NvlNhomHang
    {
        public NvlNhomHang CopyClass()
        {
            var json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<NvlNhomHang>(json);
        }
    }
}
