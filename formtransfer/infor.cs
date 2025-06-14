using System;
using System.Linq;
using System.Windows.Forms;
using WeDeLi1.Dbase; // Đảm bảo using cho DbContext và các lớp thực thể
using WeDeLi1.Service; // Đảm bảo using cho sessionmanager

namespace WeDeLi1.formtransfer
{
    public partial class infor : Form
    {
        public infor()
        {
            InitializeComponent();
            // Gán các sự kiện cho control khi form được khởi tạo
            this.Load += infor_Load;
            button4.Click += button4_Click; // Nút "Lưu thông tin"
            button3.Click += (sender, e) => this.Close(); // Nút "Trở về"
            button1.Click += button1_Click; // Nút "Danh sách xe"
            button2.Click += button2_Click; // Nút "Danh sách nhân viên"
        }

        // Sự kiện khi form được tải
        private void infor_Load(object sender, EventArgs e)
        {
            LoadNhaXeInfo();
        }

        /// <summary>
        /// Tải thông tin của nhà xe hiện tại từ CSDL và hiển thị lên các textbox.
        /// </summary>
        private void LoadNhaXeInfo()
        {
            try
            {
                using (var dbContext = new databases())
                {
                    // Lấy mã nhà xe đang đăng nhập từ session
                    var currentNhaXeId = sessionmanager.currentTransport;

                    if (string.IsNullOrEmpty(currentNhaXeId))
                    {
                        MessageBox.Show("Không thể xác định thông tin nhà xe. Vui lòng đăng nhập lại.", "Lỗi phiên", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }

                    // Tìm nhà xe trong CSDL
                    var nhaXe = dbContext.NhaXes.Find(currentNhaXeId);

                    if (nhaXe == null)
                    {
                        MessageBox.Show("Không tìm thấy dữ liệu cho nhà xe.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }

                    // Điền thông tin vào các control trên form
                    textBox1.Text = nhaXe.MaNhaXe;
                    textBox1.ReadOnly = true; // Không cho phép sửa Mã Nhà Xe (Khóa chính)
                    textBox2.Text = nhaXe.TenChu;
                    textBox4.Text = nhaXe.TenDangNhap;
                    textBox3.Text = nhaXe.MatKhau;
                    textBox6.Text = nhaXe.Email;
                    textBox5.Text = nhaXe.SoDienThoai;
                    textBox7.Text = nhaXe.DiaChi;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin nhà xe: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện cho nút "Lưu thông tin" (button4)
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            // Lấy các giá trị từ textbox
            string maNhaXe = textBox1.Text;
            string tenChu = textBox2.Text.Trim();
            string tenDangNhap = textBox4.Text.Trim();
            string matKhau = textBox3.Text.Trim();
            string email = textBox6.Text.Trim();
            string soDienThoai = textBox5.Text.Trim();
            string diaChi = textBox7.Text.Trim();

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(tenChu) || string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(soDienThoai))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các trường thông tin bắt buộc.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var dbContext = new databases())
                {
                    // Tìm lại nhà xe trong CSDL để cập nhật
                    var nhaXeToUpdate = dbContext.NhaXes.Find(maNhaXe);
                    if (nhaXeToUpdate == null)
                    {
                        MessageBox.Show("Không tìm thấy nhà xe để cập nhật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Cập nhật các thuộc tính
                    nhaXeToUpdate.TenChu = tenChu; //
                    nhaXeToUpdate.TenDangNhap = tenDangNhap; //
                    nhaXeToUpdate.MatKhau = matKhau; //
                    nhaXeToUpdate.Email = email; //
                    nhaXeToUpdate.SoDienThoai = soDienThoai; //
                    nhaXeToUpdate.DiaChi = diaChi; //

                    // Lưu thay đổi vào CSDL
                    dbContext.SaveChanges();

                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi lưu thông tin: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Mở form danh sách xe
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            using (var listXeForm = new listxe())
            {
                listXeForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Mở form danh sách nhân viên
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            using (var listNhanVienForm = new listnhanvien())
            {
                listNhanVienForm.ShowDialog(this);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form hiện tại
            var maintranfer = new maintransfer();
            maintranfer.Show(); // Mở form chính của ứng dụng

        }
    }
}