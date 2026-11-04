using System.ComponentModel.DataAnnotations;

namespace CoffeShopManagement.Models
{
    [MetadataType(typeof(CafeTableMetadata))]
    public partial class CafeTable
    {
    }

    public class CafeTableMetadata
    {
        [Required(ErrorMessage = "Vui lòng chọn khu vực")]
        public int AreaID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên bàn")]
        [StringLength(50, ErrorMessage = "Tên bàn không được vượt quá 50 ký tự")]
        public string TableName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái bàn")]
        [StringLength(30, ErrorMessage = "Trạng thái không được vượt quá 30 ký tự")]
        public string TableStatus { get; set; }
    }
}