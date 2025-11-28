using System;
using System.Collections.Generic;

namespace DOANLTWEB.Models
{
    public class ThongKe
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public double TongDoanhThu { get; set; }
        public int SoDonHang { get; set; }
        public int DonHangThanhCong { get; set; }
        public int TongKhachHang { get; set; }
        public int TongSach { get; set; }
        public double TrungBinhDonHang { get; set; }
        public List<TopSach> TopSach { get; set; }
        public string Error { get; set; }
    }

    public class TopSach
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public int SoLuong { get; set; }
    }
}