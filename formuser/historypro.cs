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

        private void LoadLichSuDonHang()
        {
            try
            {
                // Lấy danh sách đơn hàng đã xác nhận từ database
                var confirmedOrders = new List<DonHang>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT d.MaDonHang, d.LoaiDon, d.DiaChiLayHang, d.DiaChiGiaoHang, d.KhoiLuong, 
                               d.MaNguoiDung, p.MaPhuongTien, n.MaNhaXe
                        FROM DonHang d
                        JOIN PhuongTien p ON d.MaPhuongTien = p.MaPhuongTien
                        JOIN NhaXe n ON p.MaNhaXe = n.MaNhaXe
                        WHERE d.MaNguoiDung = @MaNguoiDung";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaNguoiDung", sessionManager);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                confirmedOrders.Add(new DonHang
                                {
                                    MaDonHang = reader["MaDonHang"].ToString(),
                                    LoaiDon = reader["LoaiDon"].ToString(),
                                    DiaChiLayHang = reader["DiaChiLayHang"].ToString(),
                                    DiaChiGiaoHang = reader["DiaChiGiaoHang"].ToString(),
                                    KhoiLuong = reader["KhoiLuong"] != DBNull.Value ? Convert.ToDouble(reader["KhoiLuong"]) : (double?)null,
                                    MaNguoiDung = reader["MaNguoiDung"].ToString(),
                                    MaPhuongTien = reader["MaPhuongTien"].ToString(),
                                    //MaNhaXe = reader["MaNhaXe"].ToString(),
                                    TrangThai = "XacNhan", // Giả định đơn hàng trong DonHang đều đã xác nhận
                                    tenNguoiNhan = null,
                                    //ThanhTien = null
                                });
                            }
                        }
                    }
                }

                // Gán dữ liệu vào DataGridView
                dataGridView1.DataSource = confirmedOrders;
                dataGridView1.Columns["MaDonHang"].HeaderText = "Mã Đơn Hàng";
                dataGridView1.Columns["LoaiDon"].HeaderText = "Loại Đơn";
                dataGridView1.Columns["KhoiLuong"].HeaderText = "Trọng Tải";
                dataGridView1.Columns["DiaChiLayHang"].HeaderText = "Địa Chỉ Nhận";
                dataGridView1.Columns["DiaChiGiaoHang"].HeaderText = "Địa Chỉ Giao";
                dataGridView1.Columns["tenNguoiNhan"].HeaderText = "Tên Người Nhận";
                //dataGridView1.Columns["ThanhTien"].HeaderText = "Thành Tiền";
                //dataGridView1.Columns["MaNhaXe"].HeaderText = "Nhà Xe";
                dataGridView1.Columns["TrangThai"].HeaderText = "Trạng Thái";

                // Ẩn các cột không cần thiết
                dataGridView1.Columns["MaNguoiDung"].Visible = false;
                dataGridView1.Columns["MaPhuongTien"].Visible = false;

                //// Ẩn cột nếu không có dữ liệu
                //if (dataGridView1.Columns.Contains("tenNguoiNhan") && confirmedOrders.All(o => o.tenNguoiNhan == null))
                //    dataGridView1.Columns["tenNguoiNhan"].Visible = false;
                //if (dataGridView1.Columns.Contains("ThanhTien") && confirmedOrders.All(o => o.ThanhTien == null))
                //    dataGridView1.Columns["ThanhTien"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lịch sử đơn hàng đã xác nhận: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}