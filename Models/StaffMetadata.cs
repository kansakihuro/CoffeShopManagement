using System.ComponentModel.DataAnnotations;

namespace CoffeShopManagement.Models
{
    [MetadataType(typeof(StaffMetadata))]
    public partial class Staff
    {
    }

    public class StaffMetadata
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên nhân viên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^0\d{9}$",
            ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 0")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chức vụ")]
        [StringLength(50, ErrorMessage = "Chức vụ không được vượt quá 50 ký tự")]
        public string RoleName { get; set; }
    }
}