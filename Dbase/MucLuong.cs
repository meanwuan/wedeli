namespace WeDeLi1.Dbase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MucLuong")]
    public partial class MucLuong
    {
        public double? BacLuong { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string MaLuong { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string MaNhaXe { get; set; }

        public virtual NhaXe NhaXe { get; set; }
    }
}
