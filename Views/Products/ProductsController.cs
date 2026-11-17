using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoffeShopManagement.Models;

namespace CoffeShopManagement.Controllers
{
    public class ProductsController : Controller
    {
        private CoffeeShopManagementDBEntities db = new CoffeeShopManagementDBEntities();

        public ActionResult Index(string searchString, int? categoryId, string status)
        {
            var products = db.Products.Include(p => p.Category).AsQueryable();

            if (!String.IsNullOrWhiteSpace(searchString))
            {
                string keyword = searchString.Trim();
                products = products.Where(p => p.ProductName.Contains(keyword));
            }

            if (categoryId != null)
            {
                products = products.Where(p => p.CategoryID == categoryId);
            }

            if (status == "active")
            {
                products = products.Where(p => p.IsActive);
            }
            else if (status == "inactive")
            {
                products = products.Where(p => !p.IsActive);
            }

            ViewBag.CategoryID = new SelectList(
                db.Categories.OrderBy(c => c.CategoryName),
                "CategoryID", "CategoryName", categoryId);

            ViewBag.Status = status;

            return View(products.OrderBy(p => p.Category.CategoryName)
                                .ThenBy(p => p.ProductName)
                                .ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Create()
        {
            PopulateCategories(null);
            return View(new Product { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,Price,IsActive")] Product product)
        {
            if (product.ProductName != null)
            {
                product.ProductName = product.ProductName.Trim();
            }

            ValidateProduct(product);

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateCategories(product.CategoryID);
            return View(product);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            PopulateCategories(product.CategoryID);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,Price,IsActive")] Product product)
        {
            if (product.ProductName != null)
            {
                product.ProductName = product.ProductName.Trim();
            }

            ValidateProduct(product);

            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateCategories(product.CategoryID);
            return View(product);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Sản phẩm đã có trong đơn hàng thì không xóa được (ràng buộc khóa ngoại)
            if (db.OrderDetails.Any(od => od.ProductID == id))
            {
                ModelState.AddModelError("",
                    "Không thể xóa sản phẩm này vì đã có trong đơn hàng. Hãy sửa và bỏ chọn \"Đang kinh doanh\" để ngừng bán.");
                return View("Delete", product);
            }

            try
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Không thể xóa sản phẩm này vì đang được sử dụng ở nơi khác.");
                return View("Delete", product);
            }
            return RedirectToAction("Index");
        }

        private void ValidateProduct(Product product)
        {
            if (product.CategoryID > 0 && db.Categories.Find(product.CategoryID) == null)
            {
                ModelState.AddModelError("CategoryID", "Danh mục không tồn tại");
            }

            if (!String.IsNullOrEmpty(product.ProductName) && product.CategoryID > 0)
            {
                bool duplicated = db.Products.Any(p =>
                    p.ProductID != product.ProductID &&
                    p.CategoryID == product.CategoryID &&
                    p.ProductName == product.ProductName);

                if (duplicated)
                {
                    ModelState.AddModelError("ProductName",
                        "Sản phẩm này đã tồn tại trong danh mục đã chọn");
                }
            }
        }

        private void PopulateCategories(int? selectedId)
        {
            var categories = db.Categories
                .Where(c => c.IsActive || c.CategoryID == selectedId)
                .OrderBy(c => c.CategoryName);

            ViewBag.CategoryID = new SelectList(
                categories, "CategoryID", "CategoryName", selectedId);
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