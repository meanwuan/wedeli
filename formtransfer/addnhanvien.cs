using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeDeLi1.Dbase;

namespace WeDeLi1.formtransfer
{
    public partial class addnhanvien : Form
    {
        private const double LUONG_NHAN_VIEN = 12000000;
        private const double LUONG_QUAN_LY = 30000000;
        private MucLuong selectedMucLuong;

        public addnhanvien()
        {
            InitializeComponent();
            this.Load += addnhanvien_Load;
            button1.Click += (sender, e) => this.Close();
            button2.Click += button2_Click;
            comboBoxChucVu.SelectedIndexChanged += comboBoxChucVu_SelectedIndexChanged;
        }

        private void addnhanvien_Load(object sender, EventArgs e)
        {
            comboBoxChucVu.Items.Add("Nhân viên");
            comboBoxChucVu.Items.Add("Quản lý");
            if (comboBoxChucVu.Items.Count > 0)
                comboBoxChucVu.SelectedIndex = 0;
        }

        private void comboBoxChucVu_SelectedIndexChanged(object sender, EventArgs e)
        {
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

            string maNhanVien = textBox1.Text.Trim();
            string hoTen = textBox2.Text.Trim();

            if (!double.TryParse(textBoxLuong.Text.Replace(",", ""), out double luongValue))
            {
                MessageBox.Show("Giá trị lương không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ====================================================================
            // THAY ĐỔI DUY NHẤT NẰM Ở KHỐI CATCH BÊN DƯỚI
            // ====================================================================
            try
            {
                using (var dbContext = new databases())
                {
                    if (dbContext.NhanViens.Any(nv => nv.MaNhanVien == maNhanVien))
                    {
                        MessageBox.Show("Mã nhân viên này đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var newNhanVien = new NhanVien
                    {
                        MaNhanVien = maNhanVien,
                        HoTen = hoTen,
                        SoDienThoai = textBox3.Text.Trim(),
                        Email = textBox4.Text.Trim(),
                        DiaChiThuongTru = textBox5.Text.Trim(),
                        DiaChiTamTru = textBox6.Text.Trim(),
                        ChucVu = comboBoxChucVu.SelectedItem.ToString(),
                        NgayVaoLam = DateTime.TryParse(textBox7.Text, out var tempDate) ? (DateTime?)tempDate : null,
                        MaNhaXe = sessionmanager.currentTransport
                    };

                    var newLuong = new Luong
                    {
                        MaNhanVien = maNhanVien,
                        MaLuong = selectedMucLuong.MaLuong,
                        SoTien = luongValue,
                        TrangThai = "Ban đầu",
                        ngaydientai = DateTime.Now
                    };

                    dbContext.NhanViens.Add(newNhanVien);
                    dbContext.Luongs.Add(newLuong);
                    dbContext.SaveChanges();

                    MessageBox.Show("Thêm nhân viên mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // MỚI: Khối catch này sẽ hiển thị TOÀN BỘ thông tin lỗi
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine("Đã xảy ra lỗi không xác định khi lưu dữ liệu.");
                errorMessage.AppendLine("Vui lòng chụp lại màn hình lỗi này và gửi lại.");
                errorMessage.AppendLine("--------------------------------");

                Exception innerEx = ex;
                int level = 1;
                while (innerEx != null)
                {
                    errorMessage.AppendLine($"\nLỗi cấp {level}: {innerEx.GetType().Name}");
                    errorMessage.AppendLine($"Thông điệp: {innerEx.Message}");
                    innerEx = innerEx.InnerException;
                    level++;
                }

                MessageBox.Show(errorMessage.ToString(), "Lỗi Chi Tiết", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}