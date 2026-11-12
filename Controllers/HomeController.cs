using CoffeShopManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace CoffeShopManagement.Controllers
{
    public class HomeController : Controller
    {
        private CoffeeShopManagementDBEntities db =
            new CoffeeShopManagementDBEntities();

        public ActionResult Index()
        {
            // Tổng số đơn hàng
            ViewBag.TotalOrders = db.Orders.Count();

            // Số đơn đang xử lý
            ViewBag.ProcessingOrders = db.Orders.Count(o =>
                o.OrderStatus == "New" ||
                o.OrderStatus == "Preparing");

            // Số bàn trống
            ViewBag.AvailableTables = db.CafeTables.Count(t =>
                t.TableStatus == "Available");

            // Số bàn đang sử dụng
            ViewBag.OccupiedTables = db.CafeTables.Count(t =>
                t.TableStatus == "Occupied");

            // Doanh thu hôm nay
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            ViewBag.TodayRevenue = db.Orders
                .Where(o => o.OrderStatus == "Completed"
                         && o.CompletedAt.HasValue
                         && o.CompletedAt.Value >= today
                         && o.CompletedAt.Value < tomorrow)
                            .SelectMany(o => o.OrderDetails)
                .Sum(d => (decimal?)(d.Quantity * d.UnitPrice)) ?? 0;

            // Doanh thu tháng hiện tại
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            ViewBag.MonthRevenue = db.Orders
                .Where(o => o.OrderStatus == "Completed"
                         && o.CompletedAt.HasValue
                         && o.CompletedAt.Value >= firstDayOfMonth
                         && o.CompletedAt.Value < firstDayOfNextMonth)
                .SelectMany(o => o.OrderDetails)
                .Sum(d => (decimal?)(d.Quantity * d.UnitPrice)) ?? 0;

            return View();
        }

        public ActionResult DbTest()
        {
            try
            {
                string cs = ConfigurationManager
                    .ConnectionStrings["CoffeeShopDb"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(cs))
                {
                    conn.Open();

                    return Content(
                        "Ket noi SQL Server thanh cong! Database: "
                        + conn.Database
                    );
                }
            }
            catch (Exception ex)
            {
                return Content("Ket noi that bai: " + ex.Message);
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}