namespace WeDeLi1.Dbase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NhaXe")]
    public partial class NhaXe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhaXe()
        {
            GiaoHangs = new HashSet<GiaoHang>();
            MucLuongs = new HashSet<MucLuong>();
            NhanViens = new HashSet<NhanVien>();
            PhuongTiens = new HashSet<PhuongTien>();
            ThanhToanDonHangs = new HashSet<ThanhToanDonHang>();
        }

        [Key]
        [StringLength(20)]
        public string MaNhaXe { get; set; }

        [Required]
        [StringLength(50)]
        public string TenChu { get; set; }

        [Required]
        public int sotien { get; set; }

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [Required]
        [StringLength(30)]
        public string MatKhau { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(10)]
        public string SoDienThoai { get; set; }

        public string DiaChi { get; set; }

        [StringLength(50)]
        public string Kho { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaoHang> GiaoHangs { get; set; }

        public virtual GiayTo GiayTo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MucLuong> MucLuongs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NhanVien> NhanViens { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhuongTien> PhuongTiens { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThanhToanDonHang> ThanhToanDonHangs { get; set; }
    }
}
