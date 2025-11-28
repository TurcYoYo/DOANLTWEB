using DOANLTWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace DOANLTWEB.Controllers
{
    public class CartController : Controller
    {
        private DULIEU db = new DULIEU();

        // GET: Cart/Index - Xem giỏ hàng
        public ActionResult Index()
        {
            if (Session["KhachHang"] == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem giỏ hàng";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var maKH = (int)Session["MaKH"];

                // Load giỏ hàng với chi tiết và thông tin sách
                var gioHang = db.GioHangs
                    .Include(g => g.ChiTietGHs)
                    .Include(g => g.ChiTietGHs.Select(ct => ct.Sach))
                    .Include(g => g.ChiTietGHs.Select(ct => ct.Sach.TheLoaiSach))
                    .Include(g => g.ChiTietGHs.Select(ct => ct.Sach.NhaXuatBan))
                    .FirstOrDefault(g => g.KhachHangs.Any(k => k.MaKH == maKH));

                if (gioHang == null || !gioHang.ChiTietGHs.Any())
                {
                    ViewBag.EmptyMessage = "Giỏ hàng của bạn đang trống";
                    return View(new List<ChiTietGH>());
                }

                return View(gioHang.ChiTietGHs.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi tải giỏ hàng: " + ex.Message;
                return View(new List<ChiTietGH>());
            }
        }

        // POST: Cart/AddToCart - Thêm vào giỏ hàng
        [HttpPost]
        public ActionResult AddToCart(int maSach, int soLuong = 1)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"AddToCart called: MaSach={maSach}, SoLuong={soLuong}");

                if (Session["KhachHang"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thêm vào giỏ hàng" });
                }

                var maKH = (int)Session["MaKH"];
                var khachHang = db.KhachHangs
                    .Include(k => k.GioHang)
                    .Include(k => k.GioHang.ChiTietGHs)
                    .FirstOrDefault(k => k.MaKH == maKH);

                if (khachHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng" });
                }

                var sach = db.Saches.Find(maSach);
                if (sach == null)
                {
                    return Json(new { success = false, message = "Sách không tồn tại" });
                }

                // Tạo giỏ hàng nếu chưa có
                if (khachHang.GioHang == null)
                {
                    var maxMaGH = db.GioHangs.Any() ? db.GioHangs.Max(g => g.MaGH) : 400;
                    khachHang.GioHang = new GioHang
                    {
                        MaGH = maxMaGH + 1,
                        NgayUpdateCuoiGH = DateTime.Now
                    };
                    db.GioHangs.Add(khachHang.GioHang);
                }

                var chiTietGH = khachHang.GioHang.ChiTietGHs
                    .FirstOrDefault(ct => ct.MaSach == maSach);

                if (chiTietGH != null)
                {
                    chiTietGH.SoLuongSachCTGH += soLuong;
                }
                else
                {
                    chiTietGH = new ChiTietGH
                    {
                        MaGH = khachHang.GioHang.MaGH,
                        MaSach = maSach,
                        SoLuongSachCTGH = soLuong
                    };
                    db.ChiTietGHs.Add(chiTietGH);
                }

                khachHang.GioHang.NgayUpdateCuoiGH = DateTime.Now;
                db.SaveChanges();

                var tongSoLuong = khachHang.GioHang.ChiTietGHs.Sum(ct => ct.SoLuongSachCTGH);

                return Json(new
                {
                    success = true,
                    message = "Đã thêm '" + sach.TenSach + "' vào giỏ hàng",
                    tongSoLuong = tongSoLuong
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddToCart: {ex.Message}");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: Cart/UpdateCart - Cập nhật giỏ hàng
        [HttpPost]
        public ActionResult UpdateCart(int maSach, int soLuong)
        {
            try
            {
                if (Session["KhachHang"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var maKH = (int)Session["MaKH"];
                var khachHang = db.KhachHangs
                    .Include(k => k.GioHang)
                    .Include(k => k.GioHang.ChiTietGHs)
                    .FirstOrDefault(k => k.MaKH == maKH);

                var chiTietGH = khachHang?.GioHang?.ChiTietGHs
                    .FirstOrDefault(ct => ct.MaSach == maSach);

                if (chiTietGH == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sách trong giỏ hàng" });
                }

                if (soLuong <= 0)
                {
                    // Xóa khỏi giỏ hàng nếu số lượng <= 0
                    db.ChiTietGHs.Remove(chiTietGH);
                }
                else
                {
                    chiTietGH.SoLuongSachCTGH = soLuong;
                }

                // Cập nhật ngày update
                khachHang.GioHang.NgayUpdateCuoiGH = DateTime.Now;
                db.SaveChanges();

                // Tính lại tổng
                var tongSoLuong = khachHang.GioHang.ChiTietGHs.Sum(ct => ct.SoLuongSachCTGH);
                var thanhTien = soLuong > 0 ? chiTietGH.SoLuongSachCTGH * (chiTietGH.Sach?.Gia ?? 0) : 0;
                var tongTien = khachHang.GioHang.ChiTietGHs.Sum(ct =>
                    ct.SoLuongSachCTGH * (ct.Sach?.Gia ?? 0));

                return Json(new
                {
                    success = true,
                    message = "Đã cập nhật giỏ hàng",
                    tongSoLuong = tongSoLuong,
                    thanhTien = thanhTien,
                    tongTien = tongTien
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: Cart/RemoveFromCart - Xóa khỏi giỏ hàng
        [HttpPost]
        public ActionResult RemoveFromCart(int maSach)
        {
            try
            {
                if (Session["KhachHang"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var maKH = (int)Session["MaKH"];
                var khachHang = db.KhachHangs
                    .Include(k => k.GioHang)
                    .Include(k => k.GioHang.ChiTietGHs)
                    .FirstOrDefault(k => k.MaKH == maKH);

                var chiTietGH = khachHang?.GioHang?.ChiTietGHs
                    .FirstOrDefault(ct => ct.MaSach == maSach);

                if (chiTietGH == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sách trong giỏ hàng" });
                }

                var tenSach = chiTietGH.Sach?.TenSach ?? "Sách";
                db.ChiTietGHs.Remove(chiTietGH);

                // Cập nhật ngày update
                khachHang.GioHang.NgayUpdateCuoiGH = DateTime.Now;
                db.SaveChanges();

                var tongSoLuong = khachHang.GioHang.ChiTietGHs.Sum(ct => ct.SoLuongSachCTGH);
                var tongTien = khachHang.GioHang.ChiTietGHs.Sum(ct =>
                    ct.SoLuongSachCTGH * (ct.Sach?.Gia ?? 0));

                return Json(new
                {
                    success = true,
                    message = "Đã xóa '" + tenSach + "' khỏi giỏ hàng",
                    tongSoLuong = tongSoLuong,
                    tongTien = tongTien
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: Cart/GetCartCount - Lấy số lượng sách trong giỏ hàng
        [HttpGet]
        
        public ActionResult GetCartCount()
        {
            try
            {
                if (Session["KhachHang"] == null)
                {
                    return Json(new { count = 0 }, JsonRequestBehavior.AllowGet);
                }

                var maKH = (int)Session["MaKH"];
                var khachHang = db.KhachHangs
                    .Include(k => k.GioHang)
                    .Include(k => k.GioHang.ChiTietGHs)
                    .FirstOrDefault(k => k.MaKH == maKH);

                var count = khachHang?.GioHang?.ChiTietGHs.Sum(ct => ct.SoLuongSachCTGH) ?? 0;

                return Json(new { count = count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { count = 0 }, JsonRequestBehavior.AllowGet);
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