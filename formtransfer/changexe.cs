using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using WeDeLi1.Dbase; // Thêm using cho DbContext

namespace WeDeLi1.formtransfer
{
    public partial class changexe : Form
    {
        private readonly string _maPhuongTien; // Biến lưu mã xe cần sửa

        // Constructor nhận mã phương tiện
        public changexe(string maPhuongTien)
        {
            InitializeComponent();
            _maPhuongTien = maPhuongTien;

            // Gán sự kiện
            this.Load += changexe_Load;
            button1.Click += (sender, e) => this.Close(); // Nút Trở về
            button2.Click += button2_Click; // Nút Xác nhận
        }

        private void changexe_Load(object sender, EventArgs e)
        {
            try
            {
                using (var dbContext = new databases())
                {
                    // Tìm phương tiện trong CSDL
                    var vehicle = dbContext.PhuongTiens.Find(_maPhuongTien);

                    if (vehicle == null)
                    {
                        MessageBox.Show("Không tìm thấy thông tin xe.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }

                    // Điền thông tin vào các textbox
                    textBox1.Text = vehicle.MaPhuongTien;
                    textBox1.ReadOnly = true; // Không cho sửa mã
                    textBox2.Text = vehicle.BienSo;
                    textBox3.Text = vehicle.LoaiHang;
                    textBox4.Text = vehicle.GiayPhepLaiXe;
                    textBox5.Text = vehicle.TrangThai;
                    textBox6.Text = vehicle.TaiTrong.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu xe: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu đã sửa
            string bienSo = textBox2.Text.Trim();
            string loaiHang = textBox3.Text.Trim();

            // Kiểm tra
            if (string.IsNullOrEmpty(bienSo) || string.IsNullOrEmpty(loaiHang))
            {
                MessageBox.Show("Biển số và Loại hàng không được để trống.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox6.Text.Trim(), out int taiTrong) || taiTrong <= 0)
            {
                MessageBox.Show("Tải trọng phải là một số nguyên dương.", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var dbContext = new databases())
                {
                    // Tìm xe cần cập nhật
                    var vehicleToUpdate = dbContext.PhuongTiens.Find(_maPhuongTien);
                    if (vehicleToUpdate == null)
                    {
                        MessageBox.Show("Không tìm thấy xe để cập nhật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Cập nhật các thuộc tính
                    vehicleToUpdate.BienSo = bienSo;
                    vehicleToUpdate.LoaiHang = loaiHang;
                    vehicleToUpdate.GiayPhepLaiXe = textBox4.Text.Trim();
                    vehicleToUpdate.TrangThai = textBox5.Text.Trim();
                    vehicleToUpdate.TaiTrong = taiTrong;

                    // Lưu thay đổi
                    dbContext.SaveChanges();

                    MessageBox.Show("Cập nhật thông tin xe thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // Đóng form
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}