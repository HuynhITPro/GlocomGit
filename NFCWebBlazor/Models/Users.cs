using DocumentFormat.OpenXml.Spreadsheet;
using System.ComponentModel.DataAnnotations;

namespace NFCWebBlazor.Models
{
    public class Users: IValidatableObject
    {

        public Users() { 
        }
        
        public string? UsersName { get; set; }
       
        public  string? Password { get; set; }
      
        public string? NhaMay { get; set; }
        public int? STT { get; set; }

        public string? TypeApp { get; set; }
       
        public string? GroupUser { get; set; }

      
        public string? KhuVuc { get; set; }
        public Nullable<System.DateTime> DateAccess { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string? Email { get; set; }
        public string? Ten { get; set; }
       
        public string? TenUser { get; set; }
        public Nullable<int> DuyetCap { get; set; }
        public string? PathImg { get; set; }
        public Nullable<int> LevelPhanQuyen { get; set; }
        public string? Mautemin { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if(String.IsNullOrEmpty(UsersName))
            {
                yield return new ValidationResult("UsersName không được để trống", new[] { nameof(UsersName) });
            }
            if (String.IsNullOrEmpty(Password))
            {
                yield return new ValidationResult("Password không được để trống", new[] { nameof(Password) });
            }
            if (TenUser != null && string.IsNullOrEmpty(TenUser))
            {
                yield return new ValidationResult("Tên không được để trống", new[] { nameof(TenUser) });
            }
            if (GroupUser != null && string.IsNullOrEmpty(GroupUser))
            {
                yield return new ValidationResult("Vui lòng chọn nhóm", new[] { nameof(GroupUser) });
            }
            if (KhuVuc != null && string.IsNullOrEmpty(KhuVuc))
            {
                yield return new ValidationResult("Vui lòng chọn phòng ban", new[] { nameof(KhuVuc) });
            }
            if (Email != null && string.IsNullOrEmpty(Email))
            {
                if(IsValidEmail(Email))
                    yield return new ValidationResult("Email không đúng", new[] { nameof(Email) });
            }

        }
        private bool IsValidEmail(string email)
        {
            // Kiểm tra định dạng email hợp lệ (sử dụng Regex hoặc bất kỳ phương pháp nào khác)
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public bool Checklogic()
        {
            // Tạo ValidationContext từ đối tượng user
            var validationContext = new ValidationContext(this, serviceProvider: null, items: null);
            // Gọi Validate để kiểm tra ràng buộc và lưu kết quả kiểm tra vào một danh sách
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(this, validationContext, validationResults, validateAllProperties: true);
            return isValid;

        }
    }
}
