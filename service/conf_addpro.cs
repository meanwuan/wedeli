using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using WeDeLi1.Dbase;

namespace WeDeLi1.service
{
    public class conf_addpro
    {
        private readonly string connectionString = "Server=your_server;Database=your_database;Trusted_Connection=True;";
        private readonly string logFilePath = "donhang_cho_xacnhan.json";

        /// <summary>
        /// Xác nhận hoặc hủy đơn hàng dựa trên mã đơn hàng và trạng thái.
        /// </summary>
        /// <param name="maDonHang">Mã đơn hàng cần xử lý</param>
        /// <param name="trangThai">Trạng thái mới ('XacNhan' hoặc 'TuChoi')</param>
        public void XacNhanDonHang(string maDonHang, string trangThai)
        {
            try
            {
                // Lấy danh sách đơn hàng từ file log
                List<DonHang> orders = GetDonHangFromLog();
                DonHang donHang = orders.Find(o => o.MaDonHang == maDonHang);

                if (donHang == null)
                    throw new Exception("Không tìm thấy đơn hàng trong file log!");

                if (trangThai == "XacNhan")
                {
                    // Chuyển dữ liệu vào bảng DonHangChoXacNhan để trigger xử lý
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO DonHangChoXacNhan (MaDonHangTam, LoaiDon, DiaChiLayHang, ThoiGianLayHang, DiaChiGiaoHang, ThoiGianGiaoHang, KhoiLuong, MaNguoiDung, MaPhuongTien, MaNhaXe, TrangThai) " +
                            "VALUES (@MaDonHangTam, @LoaiDon, @DiaChiLayHang, @ThoiGianLayHang, @DiaChiGiaoHang, @ThoiGianGiaoHang, @KhoiLuong, @MaNguoiDung, @MaPhuongTien, @MaNhaXe, @TrangThai)",
                            conn))
                        {
                            cmd.Parameters.AddWithValue("@MaDonHangTam", donHang.MaDonHang ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@LoaiDon", donHang.LoaiDon ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@DiaChiLayHang", donHang.DiaChiLayHang ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ThoiGianLayHang", DBNull.Value); // Không có thời gian, đặt null
                            cmd.Parameters.AddWithValue("@DiaChiGiaoHang", donHang.DiaChiGiaoHang ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ThoiGianGiaoHang", DBNull.Value); // Không có thời gian, đặt null
                            cmd.Parameters.AddWithValue("@KhoiLuong", donHang.KhoiLuong.HasValue ? (object)donHang.KhoiLuong.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@MaNguoiDung", donHang.MaNguoiDung ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@MaPhuongTien", donHang.MaPhuongTien ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@TrangThai", "XacNhan");
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Xóa đơn hàng khỏi file log sau khi chuyển sang database
                    orders.Remove(donHang);
                    File.WriteAllText(logFilePath, JsonConvert.SerializeObject(orders, Formatting.Indented));
                }
                else if (trangThai == "TuChoi")
                {
                    // Cập nhật trạng thái thành "TuChoi" trong file log
                    donHang.TrangThai = "TuChoi";
                    File.WriteAllText(logFilePath, JsonConvert.SerializeObject(orders, Formatting.Indented));
                }
                else
                {
                    throw new ArgumentException("Trạng thái không hợp lệ. Chỉ chấp nhận 'XacNhan' hoặc 'TuChoi'.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xác nhận đơn hàng: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng từ file log.
        /// </summary>
        /// <returns>Danh sách các đơn hàng</returns>
        private List<DonHang> GetDonHangFromLog()
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    string json = File.ReadAllText(logFilePath);
                    return JsonConvert.DeserializeObject<List<DonHang>>(json) ?? new List<DonHang>();
                }
                return new List<DonHang>();
            }
            catch (Exception)
            {
                return new List<DonHang>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }
    }
}

    /// <summary>
    /// Class đại diện cho đơn hàng.
    /// </summary>