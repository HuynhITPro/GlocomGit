using Microsoft.AspNetCore.Components;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;

namespace NFCWebBlazor
{


    public class PhanQuyenAccess
    {


        public string thongkemanger = "thongkemanager";
        public string kythuatmanger = "kythuatkemanager";
        public string ketoanmanger = "ketoanmanager";
        public string kehoachmanger = "kehoachmanger";
        public string khovtmanager = "khovtmanager";
        public string khotpmanager = "khotpmanager";
        public string khotrunggianmanager = "khotrunggianmanager";
        public string khogomanager = "khogomanager";
        public string khochorapmanager = "khochorapmanager";
        public string khochonhungmanager = "khochonhungmanager";
        public bool CheckDelete(string userinsert, Users users)
        {
            if ((users.UsersName == userinsert || users.GroupUser.Contains("admin")))
                return true;
            return false;
        }
        public bool CheckDeleteNhapXuatKho(string userinsert, Users users)
        {
            if ((users.UsersName == userinsert || users.GroupUser.Contains("admin") || users.GroupUser.Contains(ketoanmanger)||users.GroupUser.Contains(khovtmanager))) 
                return true;
            return false;
        }
        public  bool NVLKeHoachMuaHangDelete(string userinsert,Users users)
        {
            if ((users.UsersName == userinsert || users.GroupUser.Contains("admin")))
                return true;
            return false;
        }
        public async Task<bool> checkphanquyen(string tableName,Users user)
        {
            if(user.GroupUser.Contains("admin"))
                return true;
            List<User_PhanQuyen> lst = await ModelData.GetListUserPhanQuyen(user.UsersName);
            var query = lst.Where(p => p.Permission.Equals("Write") && p.TableName.Equals(tableName)).FirstOrDefault();
            if (query == null)
                return false;
            return true;

        }
        public async Task<bool> NVLUploadhoadon (string userinsert, Users users)
        {
            if(await checkphanquyen("NVLNhapXuatKho",users)|| await checkphanquyen("NVLDonDatHang",users))
                return true;
            return false;
        }
        public async Task<bool> UsersInsert(Users users)//Tạo user
        {
            if (await checkphanquyen("createuser",users))
                return true;
            return false;
        }
        public async Task<bool> CreateDropdownlist(Users users)//Tạo user
        {
            if (await checkphanquyen("createdropdownlist", users))
                return true;
            return false;
        }
        public async Task<bool> CreatePhanquyen(Users users)//Tạo user
        {
            if (await checkphanquyen("createphanquyen", users))
                return true;
            return false;
        }
        public async Task<bool> CreateKhachHang(Users users)//Tạo user
        {
            if (await checkphanquyen("createkhachhang", users))
                return true;
            return false;
        }
        public async Task<bool> CreateNhaCungCap(Users users)//Tạo user
        {
            if (await checkphanquyen("createnhacungcap", users))
                return true;
            return false;
        }
        public async Task<bool> CreateMaKho(Users users)//Tạo user
        {
            if (await checkphanquyen("createmakho", users))
                return true;
            return false;
        }
        public async Task<bool> CreateNoiBo(Users users)//Tạo user
        {
            if (await checkphanquyen("creategiaonhannoibo", users))
                return true;
            return false;
        }
        public async Task<bool> CreateNhomHang(Users users)//Tạo user
        {
            if (await checkphanquyen("createnhomhang", users))
                return true;
            return false;
        }
        public async Task<bool> CreateMaHang(Users users)//Tạo user
        {
            if (await checkphanquyen("createhanghoa", users))
                return true;
            return false;
        }
        public async Task<bool> CreateDinhMucVatTu(Users users)//Tạo user
        {
            if (await checkphanquyen("createdinhmucvattu", users))
                return true;
            return false;
        }
        public async Task<bool> CreateKeHoachThang(Users users)//Tạo user
        {
            if (await checkphanquyen("createkehoachthang", users))
                return true;
            return false;
        }
        public async Task<bool> CreateKeHoachMuaHang(Users users)//Tạo user
        {
            if (await checkphanquyen("createkehoachmuahang", users))
                return true;
            return false;
        }
        public async Task<bool> CreateDonDatHang(Users users)//Tạo user
        {
            if (await checkphanquyen("createdonhang", users))
                return true;
            return false;
        }
        public async Task<bool> CreateHanMucThanhToan(Users users)//Tạo user
        {
            if (await checkphanquyen("nvlHanMucThanhToan", users))
                return true;
            return false;
        }
        public async Task<bool> CreateInPhieuNVL(Users users)//Tạo user
        {
            if (await checkphanquyen("NVLInPhieu", users))
                return true;
            return false;
        }
        public async Task<bool> CreateNhapXuatKho(Users users)//Tạo user
        {
            if (await checkphanquyen("NVLNhapXuatKho", users))
                return true;
            return false;
        }
        public async Task<bool> CreateViTriKho(Users users)//Tạo user
        {
            if (await checkphanquyen("NvlQuanLyViTri", users))
                return true;
            return false;
        }
        public async Task<bool> CreateInPhieu(Users users)//Tạo user
        {
            if (await checkphanquyen("NVLInPhieu", users))
                return true;
            if (await checkphanquyen("NVLNhapXuatKho", users))
                return true;
            return false;
        }
        public async Task<bool> CreateNVLChatLuongBienBan(Users users)//Tạo user
        {
            if (await checkphanquyen("NVLChatLuongBienBan", users))
                return true;
            return false;
            
        }
        public async Task<bool> CreateThanhToan(Users users)//Tạo user
        {
            if (await checkphanquyen("NVLThanhToan", users))
                return true;
            return false;

        }
        public async Task<bool> XacNhanThanhToan(Users users)//Tạo user
        {
            if (await checkphanquyen("NVLXacNhanThanhToan", users))
                return true;
            return false;

        }
        public async Task<bool> CreateTaoDuyetGia(Users users)//Tạo user
        {
            //if (await checkphanquyen("createdonhang", users))
            //    return true;
            return true;
        }
    }
}
