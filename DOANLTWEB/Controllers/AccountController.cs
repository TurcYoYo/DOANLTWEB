using DOANLTWEB.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOANLTWEB.Controllers
{
    public class AccountController : Controller
    {
        private DULIEU db = new DULIEU();

        // GET: Account/Login
        public ActionResult Login()
        {
            if (Session["KhachHang"] != null)
            {
                return RedirectToAction("Index", "Sach");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu!";
                    return View();
                }

                var khachHang = db.KhachHangs?.FirstOrDefault(k => k.email == email && k.MatKhau == password);

                if (khachHang != null)
                {
                    Session["KhachHang"] = khachHang;
                    Session["TenKH"] = khachHang.HoTenKH;
                    Session["MaKH"] = khachHang.MaKH;

                    TempData["Success"] = "Đăng nhập thành công!";
                    return RedirectToAction("Index", "Sach");
                }
                else
                {
                    ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi đăng nhập: " + ex.Message;
                return View();
            }
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            if (Session["KhachHang"] != null)
            {
                return RedirectToAction("Index", "Sach");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string hoTen, string email, string sdt, string diaChi, string matKhau, string confirmPassword)
        {
            try
            {
                // Kiểm tra dữ liệu
                if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
                {
                    ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                    return View();
                }

                if (matKhau != confirmPassword)
                {
                    ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                    return View();
                }

                // Kiểm tra email đã tồn tại
                var existingUser = db.KhachHangs?.FirstOrDefault(k => k.email == email);
                if (existingUser != null)
                {
                    ViewBag.Error = "Email đã được sử dụng!";
                    return View();
                }

                // Tạo giỏ hàng mới
                var maxMaGH = db.GioHangs?.Any() == true ? db.GioHangs.Max(g => g.MaGH) : 400;
                var gioHang = new GioHang
                {
                    MaGH = maxMaGH + 1,
                    NgayUpdateCuoiGH = DateTime.Now
                };

                db.GioHangs?.Add(gioHang);
                db.SaveChanges();

                // Tạo khách hàng mới
                var maxMaKH = db.KhachHangs?.Any() == true ? db.KhachHangs.Max(k => k.MaKH) : 500;
                var khachHang = new KhachHang
                {
                    MaKH = maxMaKH + 1,
                    HoTenKH = hoTen,
                    email = email,
                    SDT = sdt,
                    DiaChiKH = diaChi,
                    MatKhau = matKhau,
                    MaGH = gioHang.MaGH
                };

                db.KhachHangs?.Add(khachHang);
                db.SaveChanges();

                ViewBag.Success = "Đăng ký thành công! Vui lòng đăng nhập.";
                return View("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi đăng ký: " + ex.Message;
                return View();
            }
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Remove("KhachHang");
            Session.Remove("TenKH");
            Session.Remove("MaKH");
            Session.Abandon();

            TempData["Success"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Sach");
        }

        // GET: Account/Profile
        public ActionResult Profile()
        {
            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("Login");
            }

            var maKH = (int)Session["MaKH"];
            var khachHang = db.KhachHangs?.Find(maKH);

            if (khachHang == null)
            {
                return HttpNotFound();
            }

            return View(khachHang);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}