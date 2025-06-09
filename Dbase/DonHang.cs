namespace WeDeLi1.Dbase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DonHang")]
    public partial class DonHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonHang()
        {
            GiaoHangs = new HashSet<GiaoHang>();
            ThanhToanDonHangs = new HashSet<ThanhToanDonHang>();
        }

        [Key]
        [StringLength(20)]
        public string MaDonHang { get; set; }

        [StringLength(50)]
        public string LoaiDon { get; set; }

        public string DiaChiLayHang { get; set; }

        public DateTime? ThoiGianLayHang { get; set; }

        public string DiaChiGiaoHang { get; set; }

        public DateTime? ThoiGianGiaoHang { get; set; }

        public double? KhoiLuong { get; set; }

        [StringLength(20)]
        public string MaNguoiDung { get; set; }

        [StringLength(20)]
        public string MaPhuongTien { get; set; }

        [StringLength(20)]
        public string MaNhaXe { get; set; }

        public double? TongTien { get; set; }

        [StringLength(50)]
        public string PhuongThucThanhToan { get; set; }

        public virtual NguoiDung NguoiDung { get; set; }

        public virtual PhuongTien PhuongTien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaoHang> GiaoHangs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThanhToanDonHang> ThanhToanDonHangs { get; set; }

        public string TrangThai { get; set; }
        public string tenNguoiNhan { get; set; }
    }
}