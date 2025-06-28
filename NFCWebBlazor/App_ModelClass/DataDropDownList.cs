using System.ComponentModel.DataAnnotations;

namespace NFCWebBlazor.App_ModelClass
{
    public class DataDropDownList
    {
        public int Serial { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập dữ liệu")]
        public string? Name { get; set; }
        public string? FullName { get; set; }
        public string? TypeName { get; set; }
        public object? ValueTag { get; set; }
        public string? PhanLoai { get; set; }
        public string? FullNameKhongDau { get; set; }
       
    }
    public class DataDropDownListWithChecked : DataDropDownList
    {
        public bool Checked { get; set; } = false;
    }
}
