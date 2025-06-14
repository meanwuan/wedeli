using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeDeLi1.Dbase;

namespace WeDeLi1.service
{
    public class addorders
    {
        private readonly string logfile = "don_hang_cho_xacnhan_log.json";
        private readonly string pendingLogfile = "don_hang_tam_log.json";

        public string themdonhang(DonHang donhang)
        {
            // Logic tạo mã đơn hàng tự động tăng
            List<DonHang> existingorders = Getdonhangformlog();
            int maxId = 0;
            if (existingorders.Any())
            {
                maxId = existingorders
                    .Select(o => {
                        string numericPart = new string(o.MaDonHang.Where(char.IsDigit).ToArray());
                        int.TryParse(numericPart, out int id);
                        return id;
                    })
                    .DefaultIfEmpty(0)
                    .Max();
            }
            int nextId = maxId + 1;
            donhang.MaDonHang = $"DH{nextId:D6}";
            donhang.TrangThai = "Đã xác nhận";

            // Ghi vào log file
            existingorders.Add(donhang);
            File.WriteAllText(logfile, JsonConvert.SerializeObject(existingorders, Formatting.Indented));

            // ---- BẮT ĐẦU KHỐI TRY-CATCH CHI TIẾT ----
            try
            {
                // Thêm thông tin thanh toán vào CSDL
                using (var context = new databases())
                {
                    context.DonHangs.Add(donhang);
                    var thanhToan = new ThanhToanDonHang
                    {
                        MaDonHang = donhang.MaDonHang,
                        MaNhaXe = donhang.MaNhaXe,
                        TongTien = donhang.TongTien,
                        Bac = 1
                    };
                    context.ThanhToanDonHangs.Add(thanhToan);
                    context.SaveChanges(); // Lệnh gây ra lỗi nằm ở đây
                }
                return donhang.MaDonHang;
            }
            catch (DbEntityValidationException ex)
            {
                // Đây là phần quan trọng để bắt lỗi chi tiết
                var errorMessages = new StringBuilder();
                errorMessages.AppendLine("Lỗi xác thực dữ liệu:");

                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        // Ghép thông báo lỗi: Tên thuộc tính bị lỗi và thông điệp lỗi
                        string message = $"Thuộc tính: {validationError.PropertyName}, Lỗi: {validationError.ErrorMessage}";
                        errorMessages.AppendLine(message);
                    }
                }

                // Ném ra một exception mới với thông điệp chi tiết đã thu thập được
                throw new Exception(errorMessages.ToString());
            }
            catch (Exception ex)
            {
                // Bắt các lỗi chung khác
                throw new Exception("Lỗi khi thêm đơn hàng vào CSDL: " + ex.Message);
            }
        }

        public void AddPendingOrder(XacNhanDonhang pendingOrder, string maNhaXe, double totalCost, string paymentMethod)
        {
            try
            {
                List<PendingOrder> pendingOrders = GetPendingOrders();
                // Select a suitable PhuongTien from the chosen NhaXe
                string maPhuongTien = SelectPhuongTien(maNhaXe, pendingOrder.KhoiLuong ?? 0);
                if (string.IsNullOrEmpty(maPhuongTien))
                {
                    throw new Exception("Không tìm thấy phương tiện phù hợp từ nhà xe được chọn.");
                }

                var pending = new PendingOrder
                {
                    MaDonHangTam = pendingOrder.MaDonHangTam,
                    LoaiDon = pendingOrder.LoaiDon,
                    DiaChiLayHang = pendingOrder.DiaChiLayHang,
                    ThoiGianLayHang = pendingOrder.ThoiGianLayHang,
                    DiaChiGiaoHang = pendingOrder.DiaChiGiaoHang,
                    ThoiGianGiaoHang = pendingOrder.ThoiGianGiaoHang,
                    KhoiLuong = pendingOrder.KhoiLuong,
                    tenNguoiNhan = pendingOrder.tenNguoiNhan,
                    MaNhaXe = maNhaXe,
                    MaPhuongTien = maPhuongTien,
                    TongTien = totalCost,
                    PhuongThucThanhToan = paymentMethod,
                    TrangThai = "Chờ xác nhận",
                    MaNguoiDung = sessionmanager.curentUser
                };

                pendingOrders.Add(pending);
                File.WriteAllText(pendingLogfile, JsonConvert.SerializeObject(pendingOrders, Formatting.Indented));
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm đơn hàng tạm: " + ex.Message);
            }
        }

        public string SelectPhuongTien(string maNhaXe, double khoiLuong)
        {
            using (var context = new databases())
            {
                var phuongTien = context.PhuongTiens
                    .Where(pt => pt.MaNhaXe == maNhaXe && pt.TaiTrong >= khoiLuong)
                    .OrderBy(pt => pt.TaiTrong) // Select vehicle with smallest suitable capacity
                    .Select(pt => pt.MaPhuongTien)
                    .FirstOrDefault();
                return phuongTien;
            }
        }

        public List<DonHang> Getdonhangformlog()
        {
            try
            {
                if (File.Exists(logfile))
                {
                    string json = File.ReadAllText(logfile);
                    return JsonConvert.DeserializeObject<List<DonHang>>(json) ?? new List<DonHang>();
                }
                return new List<DonHang>();
            }
            catch (Exception)
            {
                return new List<DonHang>();
            }
        }

        public List<PendingOrder> GetPendingOrders()
        {
            try
            {
                if (File.Exists(pendingLogfile))
                {
                    string json = File.ReadAllText(pendingLogfile);
                    return JsonConvert.DeserializeObject<List<PendingOrder>>(json) ?? new List<PendingOrder>();
                }
                return new List<PendingOrder>();
            }
            catch (Exception)
            {
                return new List<PendingOrder>();
            }
        }

        public void ConfirmOrder(string maDonHangTam)
        {
            try
            {
                var pendingOrders = GetPendingOrders();
                var pendingOrder = pendingOrders.FirstOrDefault(o => o.MaDonHangTam == maDonHangTam);

                if (pendingOrder == null)
                {
                    throw new Exception("Không tìm thấy đơn hàng tạm.");
                }

                var donHang = new DonHang
                {
                    LoaiDon = pendingOrder.LoaiDon,
                    KhoiLuong = pendingOrder.KhoiLuong,
                    tenNguoiNhan = pendingOrder.tenNguoiNhan,
                    DiaChiLayHang = pendingOrder.DiaChiLayHang,
                    DiaChiGiaoHang = pendingOrder.DiaChiGiaoHang,
                    ThoiGianLayHang = pendingOrder.ThoiGianLayHang,
                    ThoiGianGiaoHang = pendingOrder.ThoiGianGiaoHang,
                    MaNguoiDung = pendingOrder.MaNguoiDung,
                    MaPhuongTien = pendingOrder.MaPhuongTien,
                    MaNhaXe = pendingOrder.MaNhaXe,
                    TongTien = pendingOrder.TongTien,
                    PhuongThucThanhToan = pendingOrder.PhuongThucThanhToan
                };

                themdonhang(donHang);

                pendingOrders.Remove(pendingOrder);
                File.WriteAllText(pendingLogfile, JsonConvert.SerializeObject(pendingOrders, Formatting.Indented));
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xác nhận đơn hàng: " + ex.Message);
            }
        }

        public bool CancelOrder(string maDonHang)
        {
            try
            {
                bool removed = false;

                // Check and remove from pending orders
                var pendingOrders = GetPendingOrders();
                var pendingOrder = pendingOrders.FirstOrDefault(o => o.MaDonHangTam == maDonHang);
                if (pendingOrder != null)
                {
                    pendingOrders.Remove(pendingOrder);
                    File.WriteAllText(pendingLogfile, JsonConvert.SerializeObject(pendingOrders, Formatting.Indented));
                    removed = true;
                }

                // Check and remove from confirmed orders
                var orders = Getdonhangformlog();
                var order = orders.FirstOrDefault(o => o.MaDonHang == maDonHang);
                if (order != null)
                {
                    orders.Remove(order);
                    File.WriteAllText(logfile, JsonConvert.SerializeObject(orders, Formatting.Indented));
                    removed = true;

                    // Remove from ThanhToanDonHang in database
                    using (var context = new databases())
                    {
                        var thanhToan = context.ThanhToanDonHangs.FirstOrDefault(t => t.MaDonHang == maDonHang);
                        if (thanhToan != null)
                        {
                            context.ThanhToanDonHangs.Remove(thanhToan);
                            context.SaveChanges();
                        }
                    }
                }

                return removed;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi hủy đơn hàng: " + ex.Message);
            }
        }
    }

    public class PendingOrder : XacNhanDonhang
    {
        public string MaNhaXe { get; set; }
        public string MaPhuongTien { get; set; }
        public double? TongTien { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public string TrangThai { get; set; }
        public string MaNguoiDung { get; set; }
    }
}