using DOANLTWEB.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DOANLTWEB.Controllers
{
    public class SachController : Controller
    {
        private DULIEU db = new DULIEU();

        // GET: Sach/Index - Trang chủ
        public ActionResult Index()
        {
            try
            {
                ViewBag.Title = "Trang chủ - BookStore";

                // Lấy danh sách sách để hiển thị
                var sachList = db.Saches
                    .OrderBy(s => s.TenSach)
                    .Take(12)
                    .ToList();

                return View(sachList);
            }
            catch (Exception ex)
            {
                ViewBag.Title = "Lỗi - BookStore";
                System.Diagnostics.Debug.WriteLine("Lỗi: " + ex.Message);
                return View("Error");
            }
        }

        // GET: Sach/Search - Tìm kiếm sách
        public ActionResult Search(string searchString, int? theLoaiId)
        {
            try
            {
                ViewBag.Title = "Tìm kiếm sách - BookStore";

                var sachList = db.Saches.AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sachList = sachList.Where(s =>
                        s.TenSach.Contains(searchString) ||
                        s.TacGias.Any(tg => tg.HoTenTG.Contains(searchString) || tg.ButDanh.Contains(searchString)) ||
                        s.Mota.Contains(searchString));
                }

                if (theLoaiId.HasValue)
                {
                    sachList = sachList.Where(s => s.MaTLS == theLoaiId.Value);
                }

                var result = sachList.OrderBy(s => s.TenSach).ToList();

                ViewBag.SearchString = searchString;
                ViewBag.TheLoaiId = theLoaiId;
                ViewBag.TheLoaiList = new SelectList(db.TheLoaiSaches, "MaTLS", "TenTLS");

                return View(result);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi tìm kiếm: " + ex.Message;
                return View(new System.Collections.Generic.List<Sach>());
            }
        }

        // GET: Sach/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.Title = "Chi tiết sách - BookStore";

            var sach = db.Saches.Find(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            return View(sach);
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