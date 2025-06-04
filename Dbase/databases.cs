using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace WeDeLi1.Dbase
{
    public partial class databases : DbContext
    {
        public databases()
            : base("name=databases")
        {
        }

        public virtual DbSet<DonHang> DonHangs { get; set; }
        public virtual DbSet<GiaoHang> GiaoHangs { get; set; }
        public virtual DbSet<GiayTo> GiayToes { get; set; }
        public virtual DbSet<Luong> Luongs { get; set; }
        public virtual DbSet<MucLuong> MucLuongs { get; set; }
        public virtual DbSet<NguoiDung> NguoiDungs { get; set; }
        public virtual DbSet<NhanVien> NhanViens { get; set; }
        public virtual DbSet<NhaXe> NhaXes { get; set; }
        public virtual DbSet<PhuongTien> PhuongTiens { get; set; }
        public virtual DbSet<ThanhToanDonHang> ThanhToanDonHangs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DonHang>()
                .HasMany(e => e.GiaoHangs)
                .WithRequired(e => e.DonHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DonHang>()
                .HasMany(e => e.ThanhToanDonHangs)
                .WithRequired(e => e.DonHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NhanVien>()
                .HasMany(e => e.Luongs)
                .WithRequired(e => e.NhanVien)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NhaXe>()
                .HasMany(e => e.GiaoHangs)
                .WithRequired(e => e.NhaXe)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NhaXe>()
                .HasOptional(e => e.GiayTo)
                .WithRequired(e => e.NhaXe);

            modelBuilder.Entity<NhaXe>()
                .HasMany(e => e.MucLuongs)
                .WithRequired(e => e.NhaXe)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NhaXe>()
                .HasMany(e => e.ThanhToanDonHangs)
                .WithRequired(e => e.NhaXe)
                .WillCascadeOnDelete(false);
        }
    }
}
