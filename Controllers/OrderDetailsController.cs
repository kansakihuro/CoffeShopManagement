using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoffeShopManagement.Models;

namespace CoffeShopManagement.Controllers
{
    public class OrderDetailsController : Controller
    {
        private CoffeeShopManagementDBEntities db = new CoffeeShopManagementDBEntities();

        // GET: OrderDetails
        public ActionResult Index(string searchString)
        {
            var orderDetails = db.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Product)
                .AsQueryable();

            // Tìm theo mã đơn hàng
            if (!string.IsNullOrEmpty(searchString))
            {
                int orderID;

                if (int.TryParse(searchString, out orderID))
                {
                    orderDetails = orderDetails.Where(
                        o => o.OrderID == orderID);
                }
                else
                {
                    // Tìm theo tên sản phẩm
                    orderDetails = orderDetails.Where(
                        o => o.Product.ProductName.Contains(searchString));
                }
            }

            ViewBag.SearchString = searchString;

            return View(orderDetails.ToList());
        }

        // GET: OrderDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public ActionResult Create()
        {
            ViewBag.OrderID = new SelectList(
                db.Orders
                    .ToList()
                    .Select(o => new
                    {
                        o.OrderID,
                        DisplayName = "Đơn #" + o.OrderID + " - " +
                            (o.OrderType == "TakeAway" ? "Mang đi" : "Dùng tại quán")
                    }),
                "OrderID",
                "DisplayName"
            );

            ViewBag.ProductID = new SelectList(
                db.Products, "ProductID", "ProductName");

            return View();
        }
        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "OrderID,ProductID,Quantity")] OrderDetail orderDetail)
        {
            // Kiểm tra số lượng
            if (orderDetail.Quantity <= 0)
            {
                ModelState.AddModelError(
                    "Quantity", "Số lượng phải lớn hơn 0");
            }

            // Lấy sản phẩm từ database
            var product = db.Products.Find(orderDetail.ProductID);

            if (product == null)
            {
                ModelState.AddModelError(
                    "ProductID", "Sản phẩm không tồn tại");
            }
            else
            {
                // Tự động lấy giá sản phẩm
                orderDetail.UnitPrice = product.Price;
            }

            if (ModelState.IsValid)
            {
                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.OrderID = new SelectList(
                db.Orders, "OrderID", "OrderType", orderDetail.OrderID);

            ViewBag.ProductID = new SelectList(
                db.Products, "ProductID", "ProductName", orderDetail.ProductID);

            return View(orderDetail);
        }


        // GET: OrderDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderType", orderDetail.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", orderDetail.ProductID);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "OrderDetailID,OrderID,ProductID,Quantity")]
    OrderDetail orderDetail)
        {
            // Lấy chi tiết đơn hàng cũ
            var existingDetail = db.OrderDetails.Find(orderDetail.OrderDetailID);

            if (existingDetail == null)
                return HttpNotFound();

            // Kiểm tra số lượng
            if (orderDetail.Quantity <= 0)
            {
                ModelState.AddModelError(
                    "Quantity", "Số lượng phải lớn hơn 0");
            }

            // Kiểm tra đơn hàng
            var order = db.Orders.Find(orderDetail.OrderID);

            if (order == null)
            {
                ModelState.AddModelError(
                    "OrderID", "Đơn hàng không tồn tại");
            }

            // Kiểm tra sản phẩm và lấy giá
            var product = db.Products.Find(orderDetail.ProductID);

            if (product == null)
            {
                ModelState.AddModelError(
                    "ProductID", "Sản phẩm không tồn tại");
            }

            if (ModelState.IsValid)
            {
                existingDetail.OrderID = orderDetail.OrderID;
                existingDetail.ProductID = orderDetail.ProductID;
                existingDetail.Quantity = orderDetail.Quantity;

                // Tự động cập nhật giá từ sản phẩm
                existingDetail.UnitPrice = product.Price;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.OrderID = new SelectList(
                db.Orders, "OrderID", "OrderType", orderDetail.OrderID);

            ViewBag.ProductID = new SelectList(
                db.Products, "ProductID", "ProductName", orderDetail.ProductID);

            return View(orderDetail);
        }


        // GET: OrderDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
