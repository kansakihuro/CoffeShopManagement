
using CoffeShopManagement.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CoffeShopManagement.Controllers
{
    public class RevenueController : Controller
    {
        private CoffeeShopManagementDBEntities db =
            new CoffeeShopManagementDBEntities();

        public ActionResult Index(int? month, int? year)
        {
            var orders = db.Orders
                .Where(o => o.OrderStatus == "Completed"
                         && o.CompletedAt.HasValue);

            // Lọc theo tháng và năm
            if (month.HasValue)
            {
                orders = orders.Where(o =>
                    o.CompletedAt.Value.Month == month.Value);
            }

            if (year.HasValue)
            {
                orders = orders.Where(o =>
                    o.CompletedAt.Value.Year == year.Value);
            }

            var revenue = orders
                .SelectMany(o => o.OrderDetails)
                .GroupBy(d =>
                    System.Data.Entity.DbFunctions.TruncateTime(
                        d.Order.CompletedAt))
                .Select(g => new RevenueViewModel
                {
                    Ngay = g.Key,
                    DoanhThu = g.Sum(d =>
                        d.Quantity * d.UnitPrice)
                })
                .OrderByDescending(x => x.Ngay)
                .ToList();

            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;

            return View(revenue);
        }
    }
}
