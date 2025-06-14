using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using WeDeLi1.Dbase;

namespace WeDeLi1.formtransfer
{
    public partial class changenhanvien : Form
    {
        private const double LUONG_NHAN_VIEN = 12000000;
        private const double LUONG_QUAN_LY = 30000000;
        private readonly string _maNhanVien;

        // MỚI: Biến để lưu trữ thông tin mức lương được chọn
        private MucLuong selectedMucLuong;

        public changenhanvien(string maNhanVien)
        {
            InitializeComponent();
            _maNhanVien = maNhanVien;
            this.Load += changenhanvien_Load;
            button1.Click += (sender, e) => this.Close();
            button2.Click += button2_Click;
            comboBoxChucVu.SelectedIndexChanged += comboBoxChucVu_SelectedIndexChanged;
        }

        private void changenhanvien_Load(object sender, EventArgs e)
        {
            comboBoxChucVu.Items.Add("Nhân viên");
            comboBoxChucVu.Items.Add("Quản lý");

            using (var dbContext = new databases())
            {
                var nhanVien = dbContext.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == _maNhanVien);

                if (nhanVien == null)
                {
                    this.Close();
                    return;
                }

                // Điền các thông tin
                textBox1.Text = nhanVien.MaNhanVien;
                textBox2.Text = nhanVien.HoTen;
                //...
                comboBoxChucVu.SelectedItem = nhanVien.ChucVu;
            }
        }

        private void comboBoxChucVu_SelectedIndexChanged(object sender, EventArgs e)
        {
            // THAY ĐỔI: Logic tương tự form addnhanvien
            if (comboBoxChucVu.SelectedItem == null) return;

            string chucVu = comboBoxChucVu.SelectedItem.ToString();
            double targetBacLuong = (chucVu == "Quản lý") ? 2.0 : 1.0;
            double luongValue = (chucVu == "Quản lý") ? LUONG_QUAN_LY : LUONG_NHAN_VIEN;

            using (var dbContext = new databases())
            {
                var currentNhaXeId = sessionmanager.currentTransport;
                selectedMucLuong = dbContext.MucLuongs.FirstOrDefault(ml => ml.MaNhaXe == currentNhaXeId && ml.BacLuong == targetBacLuong);

                if (selectedMucLuong == null)
                {
                    MessageBox.Show($"Chưa thiết lập mức lương cho chức vụ '{chucVu}' (Bậc {targetBacLuong}) trong CSDL.", "Lỗi Cấu Hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxLuong.Text = "Lỗi cấu hình";
                    button2.Enabled = false;
                }
                else
                {
                    textBoxLuong.Text = luongValue.ToString("N0", CultureInfo.InvariantCulture);
                    button2.Enabled = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedMucLuong == null)
            {
                MessageBox.Show("Không thể lưu do mức lương chưa được cấu hình đúng trong CSDL.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(textBoxLuong.Text.Replace(",", ""), out double newLuongValue))
            {
                MessageBox.Show("Giá trị lương không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var dbContext = new databases())
                {
                    var employeeToUpdate = dbContext.NhanViens.Include(nv => nv.Luongs).FirstOrDefault(nv => nv.MaNhanVien == _maNhanVien);
                    if (employeeToUpdate == null) return;

                    employeeToUpdate.HoTen = textBox2.Text.Trim();
                    //...
                    employeeToUpdate.ChucVu = comboBoxChucVu.SelectedItem.ToString();

                    var currentLuong = employeeToUpdate.Luongs.OrderByDescending(l => l.ngaydientai).FirstOrDefault();

                    if (currentLuong == null || currentLuong.SoTien != newLuongValue || currentLuong.MaLuong != selectedMucLuong.MaLuong)
                    {
                        var newLuongRecord = new Luong
                        {
                            MaNhanVien = _maNhanVien,
                            // THAY ĐỔI: Lấy MaLuong từ CSDL
                            MaLuong = selectedMucLuong.MaLuong,
                            SoTien = newLuongValue,
                            TrangThai = "Cập nhật",
                            ngaydientai = DateTime.Now
                        };
                        dbContext.Luongs.Add(newLuongRecord);
                    }

                    dbContext.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi cập nhật: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}