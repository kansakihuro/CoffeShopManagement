using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
namespace CoffeShopManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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