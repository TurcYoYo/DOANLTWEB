using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DOANLTWEB.Models
{
    public partial class DULIEU : DbContext
    {
        public DULIEU()
            : base("name=DULIEU")
        {
        }

        public virtual DbSet<ChiTietDonDH> ChiTietDonDHs { get; set; }
        public virtual DbSet<ChiTietGH> ChiTietGHs { get; set; }
        public virtual DbSet<ChiTietTrangThai> ChiTietTrangThais { get; set; }
        public virtual DbSet<DiaChiGiaoHang> DiaChiGiaoHangs { get; set; }
        public virtual DbSet<DonDatHang> DonDatHangs { get; set; }
        public virtual DbSet<GioHang> GioHangs { get; set; }
        public virtual DbSet<HinhThucThanhToan> HinhThucThanhToans { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<NhanVien> NhanViens { get; set; }
        public virtual DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public virtual DbSet<QuanHuyen> QuanHuyens { get; set; }
        public virtual DbSet<Sach> Saches { get; set; }
        public virtual DbSet<TacGia> TacGias { get; set; }

      //  public virtual DbSet<Sach_TacGia> Sach_TacGias { get; set; }

        public virtual DbSet<ThanhPho> ThanhPhoes { get; set; }
        public virtual DbSet<TheLoaiSach> TheLoaiSaches { get; set; }
        public virtual DbSet<TrangThai> TrangThais { get; set; }
        public virtual DbSet<XaPhuong> XaPhuongs { get; set; }
        public IEnumerable<object> Sach_TacGia { get; internal set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DonDatHang>()
                .HasMany(e => e.ChiTietDonDHs)
                .WithRequired(e => e.DonDatHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DonDatHang>()
                .HasMany(e => e.ChiTietTrangThais)
                .WithRequired(e => e.DonDatHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<GioHang>()
                .HasMany(e => e.ChiTietGHs)
                .WithRequired(e => e.GioHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<KhachHang>()
                .Property(e => e.SDT)
                .IsUnicode(false);

            modelBuilder.Entity<NhanVien>()
                .Property(e => e.SDTNV)
                .IsUnicode(false);

            modelBuilder.Entity<NhaXuatBan>()
                .Property(e => e.SDT)
                .IsUnicode(false);

            modelBuilder.Entity<Sach>()
                .Property(e => e.Hinh)
                .IsUnicode(false);

            
         

            

            modelBuilder.Entity<Sach>()
                .HasMany(e => e.ChiTietDonDHs)
                .WithRequired(e => e.Sach)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Sach>()
                .HasMany(e => e.ChiTietGHs)
                .WithRequired(e => e.Sach)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrangThai>()
                .HasMany(e => e.ChiTietTrangThais)
                .WithRequired(e => e.TrangThai)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Sach>()
             .HasMany(s => s.TacGias)
             .WithMany(t => t.Saches)
             .Map(m =>
             {
                 m.ToTable("Sach_TacGia");
                 m.MapLeftKey("MaSach");
                 m.MapRightKey("MaTG");
             });

            //modelBuilder.Entity<Sach>()
            //    .HasMany(s => s.TheLoais)
            //    .WithMany(t => t.Saches)
            //    .Map(m =>
            //    {
            //        m.ToTable("Sach_TheLoai");
            //        m.MapLeftKey("MaSach");
            //        m.MapRightKey("MaTLS");
            //    });





        }


    }
}