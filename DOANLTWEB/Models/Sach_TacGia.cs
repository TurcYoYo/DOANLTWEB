using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DOANLTWEB.Models
{
  
        [Table("Sach_TacGia")]
        public class Sach_TacGia
        {
            [Key]
            [Column(Order = 1)]
            [Display(Name = "Mã sách")]
            public int MaSach { get; set; }

            [Key]
            [Column(Order = 2)]
            [Display(Name = "Mã tác giả")]
            public int MaTG { get; set; }

            // Navigation properties
            [ForeignKey("MaSach")]
            public virtual Sach Sach { get; set; }

            [ForeignKey("MaTG")]
            public virtual TacGia TacGia { get; set; }
        };
    }
