namespace WeDeLi1.Dbase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PhuongTien")]
    public partial class PhuongTien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PhuongTien()
        {
            DonHangs = new HashSet<DonHang>();
        }

        [Key]
        [StringLength(20)]
        public string MaPhuongTien { get; set; }

        [Required]
        [StringLength(30)]
        public string BienSo { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiHang { get; set; }

        public string GiayPhepLaiXe { get; set; }
        public string TrangThai { get; set; }
        public int TaiTrong { get; set; }

        [StringLength(20)]
        public string MaNhaXe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHang> DonHangs { get; set; }

        public virtual NhaXe NhaXe { get; set; }
    }
}
