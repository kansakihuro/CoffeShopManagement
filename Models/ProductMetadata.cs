using System.ComponentModel.DataAnnotations;

namespace CoffeShopManagement.Models
{
    [MetadataType(typeof(ProductMetadata))]
    public partial class Product
    {
    }

    public class ProductMetadata
    {
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
        public string ProductName { get; set; }

        [Range(typeof(decimal), "0", "100000000",
            ErrorMessage = "Giá sản phẩm phải từ 0 đến 100.000.000")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }
    }
}