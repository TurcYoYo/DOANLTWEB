using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DOANLTWEB.Models
{
    [Table("Sach_TheLoai")]
    public class Sach_TheLoai
    {
        [Key]
        [Column(Order = 1)]
        public int MaSach { get; set; }

        [Key]
        [Column(Order = 2)]
        public int MaTLS { get; set; }

        [ForeignKey("MaSach")]
        public virtual Sach Sach { get; set; }

        [ForeignKey("MaTLS")]
        public virtual TheLoaiSach TheLoaiSach { get; set; }
    }
}