using System.ComponentModel.DataAnnotations;

namespace CoffeShopManagement.Models
{
    [MetadataType(typeof(AreaMetadata))]
    public partial class Area
    {
    }

    public class AreaMetadata
    {
        [Required(ErrorMessage = "Vui lòng nhập tên khu vực")]
        [StringLength(100, ErrorMessage = "Tên khu vực không được vượt quá 100 ký tự")]
        public string AreaName { get; set; }
    }
}