using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using WeDeLi1.Dbase;
using WeDeLi1.service;

namespace WeDeLi1
{
    public partial class historypro : Form
    {
        private readonly string sessionManager = sessionmanager.curentUser;
        private readonly string connectionString = "Server=DESKTOP-TPT1T42;Initial Catalog=WeDeli;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;App=EntityFramework;";

        public historypro(string maNguoiDung)
        {
            InitializeComponent();
            sessionManager = maNguoiDung;
        }

        private void historypro_Load(object sender, EventArgs e)
        {
            LoadLichSuDonHang();
        }

        /// <summary>
        /// Tải và hiển thị lịch sử đơn hàng của người dùng từ cơ sở dữ liệu.
        /// Chỉ lấy những thông tin cơ bản và cần thiết.
        /// </summary>
        private void LoadLichSuDonHang()
        {
            try
            {
                var danhSachDonHang = new List<DonHangViewModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Truy vấn để lấy các thông tin cần thiết từ bảng DonHang
                    string query = @"
                        SELECT MaDonHang, LoaiDon, KhoiLuong, DiaChiLayHang, DiaChiGiaoHang, 
                               tenNguoiNhan, TongTien, TrangThai
                        FROM DonHang
                        WHERE MaNguoiDung = @MaNguoiDung";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaNguoiDung", sessionManager ?? (object)DBNull.Value);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                danhSachDonHang.Add(new DonHangViewModel
                                {
                                    MaDonHang = reader["MaDonHang"].ToString(),
                                    LoaiDon = reader["LoaiDon"].ToString(),
                                    KhoiLuong = reader["KhoiLuong"] != DBNull.Value ? Convert.ToDouble(reader["KhoiLuong"]) : (double?)null,
                                    DiaChiLayHang = reader["DiaChiLayHang"].ToString(),
                                    DiaChiGiaoHang = reader["DiaChiGiaoHang"].ToString(),
                                    tenNguoiNhan = reader["tenNguoiNhan"].ToString(),
                                    TongTien = reader["TongTien"] != DBNull.Value ? Convert.ToDouble(reader["TongTien"]) : (double?)null,
                                    TrangThai = reader["TrangThai"].ToString()
                                });
                            }
                        }
                    }
                }

                // Gán dữ liệu vào DataGridView
                dataGridView1.DataSource = danhSachDonHang;

                // Tùy chỉnh tên cột để hiển thị thân thiện hơn
                dataGridView1.Columns["MaDonHang"].HeaderText = "Mã Đơn Hàng";
                dataGridView1.Columns["LoaiDon"].HeaderText = "Loại Đơn";
                dataGridView1.Columns["KhoiLuong"].HeaderText = "Khối Lượng";
                dataGridView1.Columns["DiaChiLayHang"].HeaderText = "Địa Chỉ Lấy";
                dataGridView1.Columns["DiaChiGiaoHang"].HeaderText = "Địa Chỉ Giao";
                dataGridView1.Columns["tenNguoiNhan"].HeaderText = "Tên Người Nhận";
                dataGridView1.Columns["TongTien"].HeaderText = "Tổng Tiền";
                dataGridView1.Columns["TrangThai"].HeaderText = "Trạng Thái";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lịch sử đơn hàng: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

    /// <summary>
    /// Lớp ViewModel để chứa dữ liệu đơn hàng sẽ hiển thị trên Grid.
    /// </summary>
    public class DonHangViewModel
    {
        public string MaDonHang { get; set; }
        public string LoaiDon { get; set; }
        public double? KhoiLuong { get; set; }
        public string DiaChiLayHang { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public string tenNguoiNhan { get; set; }
        public double? TongTien { get; set; }
        public string TrangThai { get; set; }
    }
}