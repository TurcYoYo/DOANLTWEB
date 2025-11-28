using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DOANLTWEB.Models
{
    public class DonHangViewModel
    {
        public int MaDon { get; set; }

        [Display(Name = "Ngày đặt")]
        [DataType(DataType.Date)]
        public DateTime? NgayDat { get; set; }

        [Display(Name = "Khách hàng")]
        public string TenKhachHang { get; set; }

        [Display(Name = "Tổng tiền")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TongTien { get; set; }

        [Display(Name = "Mã trạng thái")]
        public int MaTT { get; set; }

        [Display(Name = "Trạng thái")]
        public string TenTrangThai { get; set; }

        public List<TrangThai> DanhSachTrangThai { get; set; }
    }
}