namespace WeDeLi1.Dbase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("XacNhanDonhang")]
    public class XacNhanDonhang
    {
        public string MaDonHangTam { get; set; }
        public string LoaiDon { get; set; }
        public string DiaChiLayHang { get; set; }
        public DateTime? ThoiGianLayHang { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public DateTime? ThoiGianGiaoHang { get; set; }
        public double? KhoiLuong { get; set; }
        public string tenNguoiNhan { get; set; }

    }
}
