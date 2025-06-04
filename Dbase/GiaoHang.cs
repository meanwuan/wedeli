namespace WeDeLi1.Dbase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GiaoHang")]
    public partial class GiaoHang
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string MaDonHang { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string MaNhaXe { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; }

        public double? SoTien { get; set; }

        public virtual DonHang DonHang { get; set; }

        public virtual NhaXe NhaXe { get; set; }
    }
}
