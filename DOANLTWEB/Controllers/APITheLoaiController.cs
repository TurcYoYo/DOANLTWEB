using DOANLTWEB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace DOANLTWEB.Controllers
{
    [RoutePrefix("api/TheLoai")]
    public class APITheLoaiController : ApiController
    {
        private DULIEU db = new DULIEU();

        // GET: api/TheLoai/GetTheLoais
        [HttpGet]
        [Route("GetTheLoais")]
        public IHttpActionResult GetTheLoais()
        {
            try
            {
                var theLoais = db.TheLoaiSaches
                    .Select(tl => new
                    {
                        tl.MaTLS,
                        tl.TenTLS
                    })
                    .ToList();

                return Ok(new { success = true, data = theLoais });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError,
                    new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: api/TheLoai/GetTheLoai/5
        [HttpGet]
        [Route("GetTheLoai/{id}")]
        public IHttpActionResult GetTheLoai(int id)
        {
            try
            {
                var theLoai = db.TheLoaiSaches
                    .Where(tl => tl.MaTLS == id)
                    .Select(tl => new
                    {
                        tl.MaTLS,
                        tl.TenTLS
                    })
                    .FirstOrDefault();

                if (theLoai == null)
                {
                    return Content(HttpStatusCode.NotFound,
                        new { success = false, message = "Không tìm thấy thể loại" });
                }

                return Ok(new { success = true, data = theLoai });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError,
                    new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: api/TheLoai/CreateTheLoai
        [HttpPost]
        [Route("CreateTheLoai")]
        public IHttpActionResult CreateTheLoai([FromBody] TheLoaiSach theLoai)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Content(HttpStatusCode.BadRequest,
                        new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                // Tạo mã thể loại mới
                var maxMaTLS = db.TheLoaiSaches.Any() ? db.TheLoaiSaches.Max(t => t.MaTLS) : 0;
                theLoai.MaTLS = maxMaTLS + 1;

                db.TheLoaiSaches.Add(theLoai);
                db.SaveChanges();

                return Ok(new
                {
                    success = true,
                    message = "Thêm thể loại thành công",
                    data = new { theLoai.MaTLS, theLoai.TenTLS }
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError,
                    new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // PUT: api/TheLoai/UpdateTheLoai/5
        [HttpPut]
        [Route("UpdateTheLoai/{id}")]
        public IHttpActionResult UpdateTheLoai(int id, [FromBody] TheLoaiSach theLoai)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Content(HttpStatusCode.BadRequest,
                        new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                if (id != theLoai.MaTLS)
                {
                    return Content(HttpStatusCode.BadRequest,
                        new { success = false, message = "Mã thể loại không khớp" });
                }

                var existingTheLoai = db.TheLoaiSaches.Find(id);
                if (existingTheLoai == null)
                {
                    return Content(HttpStatusCode.NotFound,
                        new { success = false, message = "Không tìm thấy thể loại" });
                }

                existingTheLoai.TenTLS = theLoai.TenTLS;
                db.Entry(existingTheLoai).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật thể loại thành công"
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError,
                    new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // DELETE: api/TheLoai/DeleteTheLoai/5
        [HttpDelete]
        [Route("DeleteTheLoai/{id}")]
        public IHttpActionResult DeleteTheLoai(int id)
        {
            try
            {
                var theLoai = db.TheLoaiSaches.Find(id);
                if (theLoai == null)
                {
                    return Content(HttpStatusCode.NotFound,
                        new { success = false, message = "Không tìm thấy thể loại" });
                }

                // Kiểm tra xem có sách nào đang sử dụng thể loại này không
                var sachUsing = db.Saches.Any(s => s.MaTLS == id);
                if (sachUsing)
                {
                    return Content(HttpStatusCode.BadRequest,
                        new { success = false, message = "Không thể xóa thể loại vì có sách đang sử dụng" });
                }

                db.TheLoaiSaches.Remove(theLoai);
                db.SaveChanges();

                return Ok(new
                {
                    success = true,
                    message = "Xóa thể loại thành công"
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError,
                    new { success = false, message = "Lỗi: " + ex.Message });
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