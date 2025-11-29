using System.Net;
using DOANLTWEB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

using System.Web;
using System.Web.Mvc;

namespace DOANLTWEB.Controllers
{
    public class AdminController : Controller
    {
        private DULIEU db = new DULIEU();

        // GET: Admin/Login
        public ActionResult Login()
        {
            if (Session["Admin"] != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

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

                var nhanVien = db.NhanViens.FirstOrDefault(n => n.Email == email && n.MatKhauNV == password);

                if (nhanVien != null)
                {
                    Session["Admin"] = nhanVien;
                    Session["AdminName"] = nhanVien.HoTenNV;
                    Session["AdminId"] = nhanVien.MaNV;
                    return RedirectToAction("Dashboard");
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

        // GET: Admin/Dashboard
        // GET: Admin/Dashboard
        public ActionResult Dashboard()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                ViewBag.TongKhachHang = db.KhachHangs.Count();
                ViewBag.TongDonHang = db.DonDatHangs.Count();
                ViewBag.TongSach = db.Saches.Count();

                // FIX: Xử lý null cho TongTien
                double tongDoanhThu = db.DonDatHangs.Sum(d => d.TongTien ?? 0);
                ViewBag.TongDoanhThu = tongDoanhThu;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi tải dashboard: " + ex.Message;
                return View();
            }
        }

        // GET: Admin/QuanLySach
        public ActionResult QuanLySach()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Sử dụng raw SQL để tránh EF mapping issues
                var sachList = db.Database.SqlQuery<Sach>("SELECT * FROM Sach").ToList();

                // Load thông tin quan hệ thủ công
                foreach (var sach in sachList)
                {
                    if (sach.MaTLS.HasValue)
                    {
                        sach.TheLoaiSach = db.TheLoaiSaches.Find(sach.MaTLS.Value);
                    }
                    if (sach.MaNXB.HasValue)
                    {
                        sach.NhaXuatBan = db.NhaXuatBans.Find(sach.MaNXB.Value);
                    }
                }

                return View(sachList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi QuanLySach: " + ex.Message);
                return View(new List<Sach>());
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult ThemSach(Sach sach, HttpPostedFileBase Hinh, int[] SelectedTacGia/*, int[] SelectedTheLoai*/)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Xử lý upload hình ảnh
                    if (Hinh != null && Hinh.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = System.IO.Path.GetExtension(Hinh.FileName).ToLower();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("Hinh", "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif)");
                            LoadViewBags();
                            return View(sach);
                        }

                        var fileName = Guid.NewGuid().ToString() + fileExtension;
                        var path = System.IO.Path.Combine(Server.MapPath("~/Content/images"), fileName);
                        Hinh.SaveAs(path);
                        sach.Hinh = fileName;
                    }

                    // Tạo mã sách mới
                    var maxMaSach = db.Saches.Any() ? db.Saches.Max(s => s.MaSach) : 1000;
                    sach.MaSach = maxMaSach + 1;

                    // Thêm tác giả
                    if (SelectedTacGia != null)
                    {
                        foreach (var maTG in SelectedTacGia)
                        {
                            var tacGia = db.TacGias.Find(maTG);
                            if (tacGia != null)
                            {
                                sach.TacGias.Add(tacGia);
                            }
                        }
                    }
                   
                    //if (SelectedTheLoai != null)
                    //{
                    //    foreach (var maTLS in SelectedTheLoai)
                    //    {
                    //        var theLoai = db.TheLoaiSaches.Find(maTLS);
                    //        if (theLoai != null)
                    //        {
                    //            sach.TheLoais.Add(theLoai);
                    //        }
                    //    }
                    //}


                    db.Saches.Add(sach);
                    db.SaveChanges();

                    TempData["Success"] = "Thêm sách thành công!";
                    return RedirectToAction("QuanLySach");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi thêm sách: " + ex.Message;
            }

            LoadViewBags();
            return View(sach);
        }

        private void LoadViewBags()
        {
            ViewBag.MaTLS = new SelectList(db.TheLoaiSaches, "MaTLS", "TenTLS");
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB");
            ViewBag.TacGiaList = new MultiSelectList(db.TacGias, "MaTG", "HoTenTG");
            ViewBag.TheLoaiList = new MultiSelectList(db.TheLoaiSaches, "MaTLS", "TenTLS"); // THÊM DÒNG NÀY
        }

        // GET: Admin/ThemSach (cập nhật)
        public ActionResult ThemSach()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.MaTLS = new SelectList(db.TheLoaiSaches, "MaTLS", "TenTLS");
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB");
            ViewBag.TacGiaList = new MultiSelectList(db.TacGias, "MaTG", "HoTenTG");
            ViewBag.TheLoaiList = new MultiSelectList(db.TheLoaiSaches, "MaTLS", "TenTLS");
            return View();
        }


        [Route("SuaSach/{id}")] // Thêm attribute route
        public ActionResult SuaSach(int? id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            if (id == null)
            {
                TempData["Error"] = "ID không hợp lệ!";
                return RedirectToAction("QuanLySach");
            }

            try
            {
                // Sử dụng Find thay vì FirstOrDefault để tránh lỗi
                var sach = db.Saches.Find(id);

                if (sach == null)
                {
                    TempData["Error"] = "Không tìm thấy sách!";
                    return RedirectToAction("QuanLySach");
                }

                // Load TacGias riêng
                db.Entry(sach).Collection(s => s.TacGias).Load();

                ViewBag.MaTLS = new SelectList(db.TheLoaiSaches, "MaTLS", "TenTLS", sach.MaTLS);
                ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach.MaNXB);

                var selectedTacGia = sach.TacGias?.Select(t => t.MaTG).ToArray() ?? new int[] { };
                ViewBag.TacGiaList = new MultiSelectList(db.TacGias, "MaTG", "HoTenTG", selectedTacGia);

                return View(sach);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message + " - " + ex.InnerException?.Message;
                return RedirectToAction("QuanLySach");
            }
        }

        // POST: Admin/SuaSach/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaSach(Sach sach, HttpPostedFileBase Hinh, int[] SelectedTacGia)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Bỏ qua ModelState validation vì có thể có lỗi với navigation properties
                ModelState.Remove("TheLoaiSach");
                ModelState.Remove("NhaXuatBan");
                ModelState.Remove("TacGias");

                var existingSach = db.Saches
                    .Include(s => s.TacGias)
                    .FirstOrDefault(s => s.MaSach == sach.MaSach);

                if (existingSach != null)
                {
                    // Xử lý upload hình ảnh
                    if (Hinh != null && Hinh.ContentLength > 0)
                    {
                        var fileName = System.IO.Path.GetFileName(Hinh.FileName);
                        var path = System.IO.Path.Combine(Server.MapPath("~/Content/images"), fileName);
                        Hinh.SaveAs(path);
                        existingSach.Hinh = fileName;
                    }

                    existingSach.TenSach = sach.TenSach;
                    existingSach.Gia = sach.Gia;
                    existingSach.Mota = sach.Mota;
                    existingSach.MaTLS = sach.MaTLS;
                    existingSach.MaNXB = sach.MaNXB;
                    existingSach.SoTrang = sach.SoTrang;
                    existingSach.AddNgonNgu = sach.AddNgonNgu;

                    // Cập nhật tác giả
                    existingSach.TacGias.Clear();

                    if (SelectedTacGia != null)
                    {
                        foreach (var maTG in SelectedTacGia)
                        {
                            var tacGia = db.TacGias.Find(maTG);
                            if (tacGia != null)
                            {
                                existingSach.TacGias.Add(tacGia);
                            }
                        }
                    }

                    db.SaveChanges();
                    TempData["Success"] = "Cập nhật sách thành công!";
                    return RedirectToAction("QuanLySach");
                }

                TempData["Error"] = "Không tìm thấy sách!";
                return RedirectToAction("QuanLySach");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi cập nhật sách: " + ex.Message;
                ViewBag.MaTLS = new SelectList(db.TheLoaiSaches, "MaTLS", "TenTLS", sach.MaTLS);
                ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach.MaNXB);
                ViewBag.TacGiaList = new MultiSelectList(db.TacGias, "MaTG", "HoTenTG", SelectedTacGia);
                return View(sach);
            }
        }


        private void LoadViewBagsForEdit(int maSach)
        {
            var sach = db.Saches.Find(maSach);
            if (sach != null)
            {
                var selectedTacGia = sach.TacGias.Select(t => t.MaTG).ToArray();
                ViewBag.TacGiaList = new MultiSelectList(db.TacGias, "MaTG", "HoTenTG", selectedTacGia);
            }
            else
            {
                ViewBag.TacGiaList = new MultiSelectList(db.TacGias, "MaTG", "HoTenTG");
            }

            ViewBag.MaTLS = new SelectList(db.TheLoaiSaches, "MaTLS", "TenTLS", sach?.MaTLS);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach?.MaNXB);
        }

        [HttpGet]
        public ActionResult XoaSach(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                var sach = db.Saches.Find(id);
                if (sach == null)
                {
                    TempData["Error"] = "Không tìm thấy sách!";
                    return RedirectToAction("QuanLySach");
                }

                // Kiểm tra xem sách có trong đơn hàng không
                var coTrongDonHang = db.ChiTietDonDHs.Any(ct => ct.MaSach == id);

                if (coTrongDonHang)
                {
                    TempData["Error"] = "Không thể xóa sách vì sách đã có trong đơn hàng!";
                    return RedirectToAction("QuanLySach");
                }

                // Xóa sách
                db.Saches.Remove(sach);
                db.SaveChanges();

                TempData["Success"] = "Xóa sách thành công!";
                return RedirectToAction("QuanLySach");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi xóa sách: " + ex.Message;
                return RedirectToAction("QuanLySach");
            }
        }

        [HttpPost, ActionName("XoaSach")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaSachConfirmed(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                var sach = db.Saches.Find(id);
                if (sach == null)
                {
                    TempData["Error"] = "Không tìm thấy sách!";
                    return RedirectToAction("QuanLySach");
                }

                // Kiểm tra xem sách có trong đơn hàng không
                var coTrongDonHang = db.ChiTietDonDHs.Any(ct => ct.MaSach == id);

                if (coTrongDonHang)
                {
                    TempData["Error"] = "Không thể xóa sách vì sách đã có trong đơn hàng!";
                    return RedirectToAction("QuanLySach");
                }

                // Xóa sách (các bảng liên quan sẽ tự động xóa nếu có cascade delete)
                db.Saches.Remove(sach);
                db.SaveChanges();

                TempData["Success"] = "Xóa sách thành công!";
                return RedirectToAction("QuanLySach");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi xóa sách: " + ex.Message;
                return RedirectToAction("QuanLySach");
            }
        }

        // GET: Admin/QuanLyTheLoai
        public ActionResult QuanLyTheLoai()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            var theLoaiList = db.TheLoaiSaches.ToList();
            return View(theLoaiList);
        }

        // POST: Admin/ThemTheLoai
        [HttpPost]
        public ActionResult ThemTheLoai(string tenTheLoai)
        {
            if (Session["Admin"] == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            try
            {
                if (string.IsNullOrEmpty(tenTheLoai))
                {
                    return Json(new { success = false, message = "Tên thể loại không được để trống" });
                }

                var maxMaTLS = db.TheLoaiSaches.Any() ? db.TheLoaiSaches.Max(t => t.MaTLS) : 0;
                var theLoai = new TheLoaiSach
                {
                    MaTLS = maxMaTLS + 1,
                    TenTLS = tenTheLoai
                };

                db.TheLoaiSaches.Add(theLoai);
                db.SaveChanges();

                return Json(new { success = true, message = "Thêm thể loại thành công", data = theLoai });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi thêm thể loại: " + ex.Message });
            }
        }

        // POST: Admin/SuaTheLoai
        [HttpPost]
        public ActionResult SuaTheLoai(int maTLS, string tenTheLoai)
        {
            if (Session["Admin"] == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            try
            {
                var theLoai = db.TheLoaiSaches.Find(maTLS);
                if (theLoai != null)
                {
                    theLoai.TenTLS = tenTheLoai;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Cập nhật thể loại thành công" });
                }
                return Json(new { success = false, message = "Không tìm thấy thể loại" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi cập nhật thể loại: " + ex.Message });
            }
        }

        // POST: Admin/XoaTheLoai
        [HttpPost]
        public ActionResult XoaTheLoai(int maTLS)
        {
            if (Session["Admin"] == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            try
            {
                var theLoai = db.TheLoaiSaches.Find(maTLS);
                if (theLoai != null)
                {
                    // Kiểm tra xem có sách nào đang sử dụng thể loại này không
                    var sachUsing = db.Saches.Any(s => s.MaTLS == maTLS);
                    if (sachUsing)
                    {
                        return Json(new { success = false, message = "Không thể xóa thể loại vì có sách đang sử dụng" });
                    }

                    db.TheLoaiSaches.Remove(theLoai);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa thể loại thành công" });
                }
                return Json(new { success = false, message = "Không tìm thấy thể loại" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi xóa thể loại: " + ex.Message });
            }
        }

        // GET: Admin/QuanLyTacGia
        public ActionResult QuanLyTacGia()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            var tacGiaList = db.TacGias.ToList();
            return View(tacGiaList);
        }

        // POST: Admin/ThemTacGia
        [HttpPost]
        public ActionResult ThemTacGia(string hoTenTG, string butDanh, string ngaySinh, string gioiTinh)
        {
            if (Session["Admin"] == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            try
            {
                if (string.IsNullOrEmpty(hoTenTG))
                {
                    return Json(new { success = false, message = "Họ tên tác giả không được để trống" });
                }

                var maxMaTG = db.TacGias.Any() ? db.TacGias.Max(t => t.MaTG) : 0;
                var tacGia = new TacGia
                {
                    MaTG = maxMaTG + 1,
                    HoTenTG = hoTenTG,
                    ButDanh = butDanh,
                    GioiTinh = gioiTinh
                };

                if (DateTime.TryParse(ngaySinh, out DateTime ngaySinhDate))
                {
                    tacGia.NgaySinh = ngaySinhDate;
                }

                db.TacGias.Add(tacGia);
                db.SaveChanges();

                return Json(new { success = true, message = "Thêm tác giả thành công", data = tacGia });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi thêm tác giả: " + ex.Message });
            }
        }

        // POST: Admin/SuaTacGia
        [HttpPost]
        public ActionResult SuaTacGia(int maTG, string hoTenTG, string butDanh, string ngaySinh, string gioiTinh)
        {
            if (Session["Admin"] == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            try
            {
                var tacGia = db.TacGias.Find(maTG);
                if (tacGia != null)
                {
                    tacGia.HoTenTG = hoTenTG;
                    tacGia.ButDanh = butDanh;
                    tacGia.GioiTinh = gioiTinh;

                    if (DateTime.TryParse(ngaySinh, out DateTime ngaySinhDate))
                    {
                        tacGia.NgaySinh = ngaySinhDate;
                    }

                    db.SaveChanges();
                    return Json(new { success = true, message = "Cập nhật tác giả thành công" });
                }
                return Json(new { success = false, message = "Không tìm thấy tác giả" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi cập nhật tác giả: " + ex.Message });
            }
        }

        // POST: Admin/XoaTacGia
        [HttpPost]
        public ActionResult XoaTacGia(int maTG)
        {
            if (Session["Admin"] == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            try
            {
                var tacGia = db.TacGias.Find(maTG);
                if (tacGia != null)
                {
                    // Kiểm tra xem có sách nào đang sử dụng tác giả này không
                    var sachUsing = db.Saches.Any(s => s.TacGias.Any(t => t.MaTG == maTG));
                    if (sachUsing)
                    {
                        return Json(new { success = false, message = "Không thể xóa tác giả vì có sách đang sử dụng" });
                    }

                    db.TacGias.Remove(tacGia);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa tác giả thành công" });
                }
                return Json(new { success = false, message = "Không tìm thấy tác giả" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi xóa tác giả: " + ex.Message });
            }
        }

        // GET: Admin/QuanLyDonHang
        public ActionResult QuanLyDonHang()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            var donHangList = db.DonDatHangs
                .Include(d => d.KhachHang)
                .ToList();

            var viewModelList = new List<DonHangViewModel>();
            foreach (var donHang in donHangList)
            {
                var trangThai = db.ChiTietTrangThais
                    .Where(ct => ct.MaDon == donHang.MaDon)
                    .OrderByDescending(ct => ct.NgayCapNhatTT)
                    .FirstOrDefault();

                // FIX: Xử lý null cho TongTien
                decimal tongTien = donHang.TongTien.HasValue ? (decimal)donHang.TongTien.Value : 0;

                // FIX: Xử lý null cho MaTT
                int maTT = trangThai?.MaTT ?? 1001;
                string tenTrangThai = db.TrangThais.FirstOrDefault(t => t.MaTT == maTT)?.TenTT ?? "Chờ xác nhận";

                viewModelList.Add(new DonHangViewModel
                {
                    MaDon = donHang.MaDon,
                    NgayDat = donHang.NgayDat,
                    TenKhachHang = donHang.KhachHang?.HoTenKH,
                    TongTien = tongTien, // ĐÃ FIX
                    MaTT = maTT, // ĐÃ FIX
                    TenTrangThai = tenTrangThai,
                    DanhSachTrangThai = db.TrangThais.ToList()
                });
            }

            return View(viewModelList);
        }

        // POST: Admin/CapNhatTrangThai
        [HttpPost]
        public ActionResult CapNhatTrangThai(int maDon, int maTT, string ghiChu = "")
        {
            // THÊM LOG DEBUG
            System.Diagnostics.Debug.WriteLine($"=== CapNhatTrangThai được gọi ===");
            System.Diagnostics.Debug.WriteLine($"MaDon: {maDon}, MaTT: {maTT}, GhiChu: {ghiChu}");

            if (Session["Admin"] == null)
            {
                System.Diagnostics.Debug.WriteLine("=== Chưa đăng nhập ===");
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            try
            {
                // Kiểm tra đơn hàng tồn tại
                var donHang = db.DonDatHangs.Find(maDon);
                if (donHang == null)
                {
                    System.Diagnostics.Debug.WriteLine($"=== Không tìm thấy đơn hàng {maDon} ===");
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Kiểm tra trạng thái hợp lệ
                var trangThai = db.TrangThais.Find(maTT);
                if (trangThai == null)
                {
                    System.Diagnostics.Debug.WriteLine($"=== Trạng thái {maTT} không hợp lệ ===");
                    return Json(new { success = false, message = "Trạng thái không hợp lệ" });
                }

                // Kiểm tra xem đã có trạng thái này chưa (tránh duplicate key)
                var existingStatus = db.ChiTietTrangThais
                    .FirstOrDefault(ct => ct.MaDon == maDon && ct.MaTT == maTT);

                if (existingStatus != null)
                {
                    // Nếu đã tồn tại, chỉ cập nhật thời gian và ghi chú
                    existingStatus.NgayCapNhatTT = DateTime.Now;
                    existingStatus.GhiChuTT = string.IsNullOrEmpty(ghiChu) ? "Cập nhật bởi admin" : ghiChu;
                    System.Diagnostics.Debug.WriteLine("=== Cập nhật trạng thái hiện có ===");
                }
                else
                {
                    // Thêm chi tiết trạng thái mới
                    var chiTietTT = new ChiTietTrangThai
                    {
                        MaDon = maDon,
                        MaTT = maTT,
                        NgayCapNhatTT = DateTime.Now,
                        GhiChuTT = string.IsNullOrEmpty(ghiChu) ? "Cập nhật bởi admin" : ghiChu
                    };

                    db.ChiTietTrangThais.Add(chiTietTT);
                    System.Diagnostics.Debug.WriteLine("=== Thêm mới chi tiết trạng thái ===");
                }

                // ✅ CHỈ CẬP NHẬT MaTT NẾU CỘT TỒN TẠI
                // Kiểm tra xem model có property MaTT không
                var propertyInfo = typeof(DonDatHang).GetProperty("MaTT");
                if (propertyInfo != null)
                {
                    donHang.MaTT = maTT;
                    System.Diagnostics.Debug.WriteLine($"=== Cập nhật MaTT = {maTT} cho đơn hàng ===");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("=== Cột MaTT không tồn tại trong DonDatHang, bỏ qua cập nhật ===");
                }

                db.SaveChanges();
                System.Diagnostics.Debug.WriteLine("=== Lưu database thành công ===");

                var tenTrangThai = trangThai.TenTT;
                return Json(new
                {
                    success = true,
                    message = "Cập nhật trạng thái thành công",
                    tenTrangThai = tenTrangThai
                });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Log chi tiết lỗi validation
                var errorMessages = string.Join("; ", ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.PropertyName + ": " + x.ErrorMessage));

                System.Diagnostics.Debug.WriteLine($"=== Lỗi Validation: {errorMessages} ===");
                return Json(new { success = false, message = "Lỗi validation: " + errorMessages });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== Lỗi: {ex.Message} ===");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"=== Inner Exception: {ex.InnerException.Message} ===");
                }
                return Json(new { success = false, message = "Lỗi cập nhật: " + ex.Message });
            }
        }

        // GET: Admin/ChiTietDonHang/5
        public ActionResult ChiTietDonHang(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                var donHang = db.DonDatHangs
                    .Include(d => d.KhachHang)
                    .Include(d => d.HinhThucThanhToan)
                    .Include(d => d.ChiTietDonDHs)
                    .Include(d => d.ChiTietDonDHs.Select(ct => ct.Sach))
                    .Include(d => d.ChiTietDonDHs.Select(ct => ct.Sach.TheLoaiSach))
                    .Include(d => d.ChiTietTrangThais)
                    .Include(d => d.ChiTietTrangThais.Select(ct => ct.TrangThai))
                    .FirstOrDefault(d => d.MaDon == id);

                if (donHang == null)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng!";
                    return RedirectToAction("QuanLyDonHang");
                }

                return View(donHang);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi tải chi tiết đơn hàng: " + ex.Message;
                return RedirectToAction("QuanLyDonHang");
            }
        }

        // GET: Admin/ThongKe
        // GET: Admin/ThongKe
        // GET: Admin/ThongKe
        public ActionResult ThongKe(DateTime? fromDate, DateTime? toDate)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Mặc định lấy tất cả đơn hàng (không lọc thời gian)
                if (!fromDate.HasValue)
                    fromDate = DateTime.MinValue; // Lấy từ ngày đầu tiên
                if (!toDate.HasValue)
                    toDate = DateTime.MaxValue; // Lấy đến ngày cuối cùng

                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;

                // DEBUG: In ra để kiểm tra
                System.Diagnostics.Debug.WriteLine($"=== THỐNG KÊ: fromDate={fromDate}, toDate={toDate}");

                // Lấy TẤT CẢ đơn hàng (không lọc thời gian)
                var donHangs = db.DonDatHangs.ToList();
                System.Diagnostics.Debug.WriteLine($"=== Tổng số đơn hàng: {donHangs.Count}");

                // Tính tổng doanh thu
                double tongDoanhThu = 0;
                foreach (var donHang in donHangs)
                {
                    if (donHang.TongTien.HasValue)
                    {
                        tongDoanhThu += (double)donHang.TongTien.Value;
                    }
                }
                ViewBag.TongDoanhThu = tongDoanhThu;
                System.Diagnostics.Debug.WriteLine($"=== Tổng doanh thu: {tongDoanhThu}");

                // Số đơn hàng
                ViewBag.SoDonHang = donHangs.Count;

                // Đếm đơn hàng thành công - Sửa lại cách đếm
                int donHangThanhCong = 0;
                foreach (var donHang in donHangs)
                {
                    // Cách 1: Kiểm tra trực tiếp MaTT trong DonDatHang (nếu đã thêm cột)
                    if (donHang.MaTT == 1003) // Hoàn tất
                    {
                        donHangThanhCong++;
                    }
                    // Cách 2: Hoặc kiểm tra qua ChiTietTrangThai
                    else
                    {
                        var trangThaiCuoi = db.ChiTietTrangThais
                            .Where(ct => ct.MaDon == donHang.MaDon)
                            .OrderByDescending(ct => ct.NgayCapNhatTT)
                            .FirstOrDefault();

                        if (trangThaiCuoi != null && trangThaiCuoi.MaTT == 1003)
                        {
                            donHangThanhCong++;
                        }
                    }
                }
                ViewBag.DonHangThanhCong = donHangThanhCong;
                System.Diagnostics.Debug.WriteLine($"=== Đơn hàng hoàn thành: {donHangThanhCong}");

                // Thống kê tổng quan
                ViewBag.TongKhachHang = db.KhachHangs.Count();
                ViewBag.TongSach = db.Saches.Count();
                System.Diagnostics.Debug.WriteLine($"=== Tổng khách hàng: {ViewBag.TongKhachHang}, Tổng sách: {ViewBag.TongSach}");

                // Tính trung bình giá trị đơn hàng
                ViewBag.TrungBinhDonHang = donHangs.Count > 0 ? tongDoanhThu / donHangs.Count : 0;

                // Top 5 sách bán chạy - Lấy TẤT CẢ không lọc thời gian
                var topSachQuery = from ct in db.ChiTietDonDHs
                                   group ct by ct.MaSach into g
                                   select new
                                   {
                                       MaSach = g.Key,
                                       SoLuong = g.Sum(ct => ct.SoLuongCTDDH)
                                   };

                var topSachList = topSachQuery
                    .OrderByDescending(x => x.SoLuong)
                    .Take(5)
                    .ToList();

                var topSachWithName = new List<TopSachViewModel>();
                foreach (var item in topSachList)
                {
                    var sach = db.Saches.FirstOrDefault(s => s.MaSach == item.MaSach);
                    if (sach != null)
                    {
                        topSachWithName.Add(new TopSachViewModel
                        {
                            MaSach = item.MaSach,
                            TenSach = sach.TenSach,
                            SoLuong = (int)item.SoLuong
                        });
                    }
                }

                ViewBag.TopSach = topSachWithName;
                System.Diagnostics.Debug.WriteLine($"=== Top sách: {topSachWithName.Count} sản phẩm");

                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== LỖI THỐNG KÊ: {ex.Message}");

                // Gán giá trị mặc định
                ViewBag.TongDoanhThu = 0;
                ViewBag.SoDonHang = 0;
                ViewBag.DonHangThanhCong = 0;
                ViewBag.TongKhachHang = 0;
                ViewBag.TongSach = 0;
                ViewBag.TrungBinhDonHang = 0;
                ViewBag.TopSach = new List<TopSachViewModel>();
                ViewBag.Error = "Lỗi tải thống kê: " + ex.Message;

                return View();
            }
        }

        // Thêm class này trong AdminController (đặt ngoài các action)
        public class TopSachViewModel
        {
            public int MaSach { get; set; }
            public string TenSach { get; set; }
            public int SoLuong { get; set; }
        }

        // GET: Admin/QuanLyKhachHang
        public ActionResult QuanLyKhachHang()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            var khachHangList = db.KhachHangs.ToList();
            return View(khachHangList);
        }

        // GET: Admin/Logout
        public ActionResult Logout()
        {
            Session.Remove("Admin");
            Session.Remove("AdminName");
            Session.Remove("AdminId");
            Session.Abandon();
            return RedirectToAction("");
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