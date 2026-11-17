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
    public class CategoriesController : Controller
    {
        private CoffeeShopManagementDBEntities db = new CoffeeShopManagementDBEntities();

        public ActionResult Index(string searchString)
        {
            var categories = db.Categories.AsQueryable();

            if (!String.IsNullOrWhiteSpace(searchString))
            {
                string keyword = searchString.Trim();
                categories = categories.Where(c => c.CategoryName.Contains(keyword));
            }

            return View(categories.OrderBy(c => c.CategoryName).ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        public ActionResult Create()
        {
            return View(new Category { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName,Description,IsActive")] Category category)
        {
            TrimFields(category);
            ValidateCategory(category);

            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName,Description,IsActive")] Category category)
        {
            TrimFields(category);
            ValidateCategory(category);

            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            // Danh mục còn sản phẩm thì không xóa được (ràng buộc khóa ngoại)
            if (db.Products.Any(p => p.CategoryID == id))
            {
                ModelState.AddModelError("",
                    "Không thể xóa danh mục này vì vẫn còn sản phẩm. Hãy chuyển sản phẩm sang danh mục khác hoặc bỏ chọn \"Đang hoạt động\".");
                return View("Delete", category);
            }

            try
            {
                db.Categories.Remove(category);
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Không thể xóa danh mục này vì đang được sử dụng ở nơi khác.");
                return View("Delete", category);
            }
            return RedirectToAction("Index");
        }

        private static void TrimFields(Category category)
        {
            if (category.CategoryName != null)
            {
                category.CategoryName = category.CategoryName.Trim();
            }
            if (category.Description != null)
            {
                category.Description = category.Description.Trim();
            }
        }

        private void ValidateCategory(Category category)
        {
            if (!String.IsNullOrEmpty(category.CategoryName))
            {
                bool duplicated = db.Categories.Any(c =>
                    c.CategoryID != category.CategoryID &&
                    c.CategoryName == category.CategoryName);

                if (duplicated)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại");
                }
            }
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