
using System.ComponentModel.DataAnnotations;

namespace CoffeShopManagement.Models
{
    [MetadataType(typeof(OrderDetailMetadata))]
    public partial class OrderDetail
    {
    }

    public class OrderDetailMetadata
    {
        [Required(ErrorMessage = "Vui lòng chọn đơn hàng")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int ProductID { get; set; }

        [Range(1, 1000,
            ErrorMessage = "Số lượng phải từ 1 đến 1000")]
        public int Quantity { get; set; }

        [Range(typeof(decimal), "0", "100000000",
            ErrorMessage = "Đơn giá phải từ 0 đến 100.000.000")]
        public decimal UnitPrice { get; set; }
    }
}
