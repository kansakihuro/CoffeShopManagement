
# Coffee Shop Management

## 1. Giới thiệu
Coffee Shop Management là website quản lý quán cà phê, hỗ trợ quản lý sản phẩm, danh mục, nhân viên, bàn, đơn hàng và doanh thu.

## 2. Công nghệ sử dụng
- C#
- ASP.NET MVC 5
- .NET Framework 4.8
- SQL Server
- Entity Framework 6
- HTML, CSS, Bootstrap

## 3. Các chức năng chính
- Quản lý sản phẩm
- Quản lý danh mục
- Quản lý nhân viên
- Quản lý khu vực và bàn
- Quản lý đơn hàng và chi tiết đơn hàng
- Thống kê doanh thu
- Dashboard tổng quan

## 4. Cài đặt và chạy project

### Bước 1: Chuẩn bị
Cài đặt:
- Visual Studio
- SQL Server
- SQL Server Management Studio (SSMS)

### Bước 2: Tạo Database
1. Mở SQL Server Management Studio.
2. Chạy file script SQL đi kèm project.
3. Kiểm tra database `CoffeeShopManagementDB` đã được tạo.

### Bước 3: Cấu hình kết nối
Mở file `Web.config`, tìm connection string của Entity Framework và kiểm tra tên SQL Server phù hợp với máy đang sử dụng.

### Bước 4: Chạy project
1. Mở file solution bằng Visual Studio.
2. Restore NuGet Packages nếu cần.
3. Build Solution.
4. Nhấn `Ctrl + F5` để chạy website.

## 5. Database
Database: `CoffeeShopManagementDB`

Các bảng chính:
- Categories
- Products
- Staff
- Areas
- CafeTables
- Orders
- OrderDetails

## 6. Thành viên thực hiện
Đồ án được thực hiện theo nhóm sinh viên HUFLIT.
