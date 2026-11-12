
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
    public class OrdersController : Controller
    {
        private CoffeeShopManagementDBEntities db =
            new CoffeeShopManagementDBEntities();

        // GET: Orders
        public ActionResult Index(string searchString, string orderStatus, string orderType)
        {
            var orders = db.Orders
                .Include(o => o.CafeTable)
                .Include(o => o.Staff)
                .AsQueryable();

            // Tìm kiếm theo mã đơn hàng
            if (!string.IsNullOrEmpty(searchString))
            {
                int orderID;

                if (int.TryParse(searchString, out orderID))
                {
                    orders = orders.Where(o => o.OrderID == orderID);
                }
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(orderStatus))
            {
                orders = orders.Where(o => o.OrderStatus == orderStatus);
            }

            // Lọc theo loại đơn
            if (!string.IsNullOrEmpty(orderType))
            {
                orders = orders.Where(o => o.OrderType == orderType);
            }

            ViewBag.SearchString = searchString;
            ViewBag.OrderStatus = orderStatus;
            ViewBag.OrderType = orderType;

            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Order order = db.Orders
                .Include(o => o.CafeTable)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.TableID = new SelectList(
                db.CafeTables, "TableID", "TableName");

            ViewBag.StaffID = new SelectList(
                db.Staffs, "StaffID", "FullName");

            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "StaffID,TableID,OrderType,OrderStatus,Notes")]
            Order order)
        {
            // Tự động tạo thời gian
            order.CreatedAt = DateTime.Now;

            // Xóa lỗi validation thời gian do form không gửi lên
            ModelState.Remove("CreatedAt");

            // Kiểm tra loại đơn
            if (order.OrderType == "TakeAway")
            {
                order.TableID = null;
            }
            else if (order.OrderType == "DineIn")
            {
                if (order.TableID == null)
                {
                    ModelState.AddModelError(
                        "TableID", "Vui lòng chọn bàn");
                }
                else
                {
                    var table = db.CafeTables.Find(order.TableID.Value);

                    if (table == null)
                    {
                        ModelState.AddModelError(
                            "TableID", "Bàn không tồn tại");
                    }
                    else if (table.TableStatus == "Occupied")
                    {
                        ModelState.AddModelError(
                            "TableID", "Bàn này đang được sử dụng");
                    }
                }
            }
            else
            {
                ModelState.AddModelError(
                    "OrderType", "Vui lòng chọn loại đơn hợp lệ");
            }

            // Kiểm tra trạng thái đơn
            string[] validStatuses =
            {
                "New", "Preparing", "Completed", "Cancelled"
            };

            if (!validStatuses.Contains(order.OrderStatus))
            {
                ModelState.AddModelError(
                    "OrderStatus", "Vui lòng chọn trạng thái hợp lệ");
            }

            // Thời gian hoàn thành
            if (order.OrderStatus == "Completed")
            {
                order.CompletedAt = DateTime.Now;
            }
            else
            {
                order.CompletedAt = null;
            }

            if (ModelState.IsValid)
            {
                db.Orders.Add(order);

                // Nếu là đơn dùng tại quán và chưa hoàn thành/hủy
                if (order.OrderType == "DineIn" &&
                    order.OrderStatus != "Completed" &&
                    order.OrderStatus != "Cancelled")
                {
                    var table = db.CafeTables.Find(order.TableID.Value);
                    table.TableStatus = "Occupied";
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.TableID = new SelectList(
                db.CafeTables, "TableID", "TableName", order.TableID);

            ViewBag.StaffID = new SelectList(
                db.Staffs, "StaffID", "FullName", order.StaffID);

            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Order order = db.Orders.Find(id);

            if (order == null)
                return HttpNotFound();

            ViewBag.TableID = new SelectList(
                db.CafeTables, "TableID", "TableName", order.TableID);

            ViewBag.StaffID = new SelectList(
                db.Staffs, "StaffID", "FullName", order.StaffID);

            return View(order);
        }

        // POST: Orders/Edit/5


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "OrderID,StaffID,TableID,OrderType,OrderStatus,Notes")]Order order)
        {
            // Lấy đơn hàng hiện tại từ database
            var existingOrder = db.Orders.Find(order.OrderID);

            if (existingOrder == null)
                return HttpNotFound();

            // Kiểm tra loại đơn
            if (order.OrderType == "TakeAway")
            {
                order.TableID = null;
            }
            else if (order.OrderType == "DineIn")
            {
                if (order.TableID == null)
                {
                    ModelState.AddModelError(
                        "TableID", "Vui lòng chọn bàn");
                }
                else
                {
                    var newTable = db.CafeTables.Find(order.TableID.Value);

                    if (newTable == null)
                    {
                        ModelState.AddModelError(
                            "TableID", "Bàn không tồn tại");
                    }
                    else if (
                        newTable.TableStatus == "Occupied" &&
                        existingOrder.TableID != order.TableID)
                    {
                        ModelState.AddModelError(
                            "TableID", "Bàn này đang được sử dụng");
                    }
                }
            }
            else
            {
                ModelState.AddModelError(
                    "OrderType", "Loại đơn không hợp lệ");
            }

            // Kiểm tra trạng thái
            string[] validStatuses ={"New", "Preparing", "Completed", "Cancelled"};

            if (!validStatuses.Contains(order.OrderStatus))
            {
                ModelState.AddModelError(
                    "OrderStatus", "Trạng thái không hợp lệ");
            }

            if (ModelState.IsValid)
            {
                // Lấy bàn cũ
                int? oldTableID = existingOrder.TableID;

                // Xác định đơn mới có còn sử dụng bàn không
                bool needsTable =
                    order.OrderType == "DineIn" &&
                    order.OrderStatus != "Completed" &&
                    order.OrderStatus != "Cancelled";

                // Giải phóng bàn cũ nếu đơn không còn sử dụng bàn đó
                if (oldTableID.HasValue &&
                    (!needsTable || oldTableID != order.TableID))
                {
                    var oldTable = db.CafeTables.Find(oldTableID.Value);

                    if (oldTable != null)
                    {
                        oldTable.TableStatus = "Available";
                    }
                }

                // Chiếm bàn mới nếu đơn vẫn đang hoạt động
                if (needsTable && order.TableID.HasValue)
                {
                    var newTable = db.CafeTables.Find(order.TableID.Value);
                    newTable.TableStatus = "Occupied";
                }

                // Cập nhật thông tin đơn hàng
                existingOrder.StaffID = order.StaffID;
                existingOrder.TableID = order.TableID;
                existingOrder.OrderType = order.OrderType;
                existingOrder.OrderStatus = order.OrderStatus;
                existingOrder.Notes = order.Notes;

                // Xử lý thời gian hoàn thành
                if (order.OrderStatus == "Completed")
                {
                    if (!existingOrder.CompletedAt.HasValue)
                    {
                        existingOrder.CompletedAt = DateTime.Now;
                    }
                }
                else
                {
                    existingOrder.CompletedAt = null;
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nạp lại danh sách khi validation thất bại
            ViewBag.TableID = new SelectList(
                db.CafeTables, "TableID", "TableName", order.TableID);

            ViewBag.StaffID = new SelectList(
                db.Staffs, "StaffID", "FullName", order.StaffID);

            // Giữ thời gian cũ
            order.CreatedAt = existingOrder.CreatedAt;
            order.CompletedAt = existingOrder.CompletedAt;

            return View(order);
        }


        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Order order = db.Orders.Find(id);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);

            if (order != null)
            {
                db.Orders.Remove(order);
                db.SaveChanges();
            }

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
