namespace WeDeLi1.Dbase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GiayTo")]
    public partial class GiayTo
    {
        [Key]
        [StringLength(20)]
        public string MaNhaXe { get; set; }

        public string GiayPhepHoatDong { get; set; }

        public string ThongTinCongTy { get; set; }

        public virtual NhaXe NhaXe { get; set; }
    }
}
