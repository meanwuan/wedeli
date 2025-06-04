namespace WeDeLi1.Dbase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NhanVien")]
    public partial class NhanVien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhanVien()
        {
            Luongs = new HashSet<Luong>();
        }

        [Key]
        [StringLength(20)]
        public string MaNhanVien { get; set; }

        [Required]
        [StringLength(50)]
        public string HoTen { get; set; }

        [Required]
        [StringLength(10)]
        public string SoDienThoai { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        public string DiaChiThuongTru { get; set; }

        public string DiaChiTamTru { get; set; }

        public DateTime? NgayVaoLam { get; set; }

        [StringLength(30)]
        public string ChucVu { get; set; }

        [StringLength(20)]
        public string MaNhaXe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Luong> Luongs { get; set; }

        public virtual NhaXe NhaXe { get; set; }
    }
}
