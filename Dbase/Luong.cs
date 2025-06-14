namespace WeDeLi1.Dbase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Luong")]
    public partial class Luong
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string MaNhanVien { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string MaLuong { get; set; }

        public DateTime? NgayTra { get; set; }

        public double? SoTien { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        public DateTime? ngaydientai { get; set; }
        public virtual NhanVien NhanVien { get; set; }
    }
}
