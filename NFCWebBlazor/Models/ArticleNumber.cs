using NFCWebBlazor.App_ClassDefine;
using System.ComponentModel.DataAnnotations;
using System.Reactive;

namespace NFCWebBlazor.Models
{
    public class ArticleNumberProduct
    {
        [Required(ErrorMessage = "ArticleNumber không được để trống")]
        public string ArticleNumber { get; set; }
        public string MaSP { get; set; }
        public string TenMau { get; set; }
        private uint? _color { get; set; }
        public uint? Color
        {
            get { return _color; }
            set
            {
                _color = value;
                Colorhex= StaticClass.UIntToHtmlColor(_color);

            }
        }
        public string Colorhex { get; set; }
    }
}
