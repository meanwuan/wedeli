using System;
using System.Linq;
using System.Windows.Forms;
using WeDeLi1.Dbase; // Thêm using cho DbContext

namespace WeDeLi1.formtransfer
{
    // Đổi tên class từ Form1 thành addxe
    public partial class addxe : Form
    {
        public addxe()
        {
            InitializeComponent();
            // Gán sự kiện cho các nút
            button1.Click += (sender, e) => this.Close(); // Nút Trở về
            button2.Click += button2_Click; // Nút Xác nhận
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ các textbox
            string maPhuongTien = textBox1.Text.Trim();
            string bienSo = textBox2.Text.Trim();
            string loaiHang = textBox3.Text.Trim();
            string giayPhep = textBox4.Text.Trim();
            string trangThai = textBox5.Text.Trim();

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(maPhuongTien) || string.IsNullOrEmpty(bienSo) || string.IsNullOrEmpty(loaiHang))
            {
                MessageBox.Show("Mã phương tiện, Biển số và Loại hàng không được để trống.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra tải trọng
            if (!int.TryParse(textBox6.Text.Trim(), out int taiTrong) || taiTrong <= 0)
            {
                MessageBox.Show("Tải trọng phải là một số nguyên dương.", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var dbContext = new databases())
                {
                    // Kiểm tra xem mã phương tiện đã tồn tại chưa
                    if (dbContext.PhuongTiens.Any(p => p.MaPhuongTien == maPhuongTien))
                    {
                        MessageBox.Show("Mã phương tiện này đã tồn tại. Vui lòng chọn mã khác.", "Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Tạo đối tượng PhuongTien mới
                    var newVehicle = new PhuongTien
                    {
                        MaPhuongTien = maPhuongTien,
                        BienSo = bienSo,
                        LoaiHang = loaiHang,
                        GiayPhepLaiXe = giayPhep,
                        TrangThai = trangThai,
                        TaiTrong = taiTrong,
                        MaNhaXe = sessionmanager.currentTransport // Lấy mã nhà xe từ session
                    };

                    // Thêm vào DbContext và lưu thay đổi
                    dbContext.PhuongTiens.Add(newVehicle);
                    dbContext.SaveChanges();

                    MessageBox.Show("Thêm xe mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // Đóng form sau khi thêm thành công
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}