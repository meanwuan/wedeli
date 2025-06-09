using System;
using System.Windows.Forms;
using WeDeLi1.Dbase;
using WeDeLi1.service;

namespace WeDeLi1
{
    public partial class infor : Form
    {
        private readonly UserService _userService = new UserService();

        public infor()
        {
            InitializeComponent();
        }

        private void infor_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            try
            {
                string userId = sessionmanager.curentUser;
                Console.WriteLine($"Loading user info for userId: {userId}");
                if (string.IsNullOrEmpty(userId))
                {
                    MessageBox.Show("Người dùng chưa đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var userInfo = _userService.GetUserInfo(userId);
                if (userInfo != null)
                {
                    textBox1.Text = userInfo.HoTen ?? "";
                    txtSoDienThoai.Text = userInfo.SoDienThoai ?? "";
                    txtEmail.Text = userInfo.Email ?? "";
                    txtDiaChi.Text = userInfo.DiaChi ?? "";
                    txtNgaySinh.Text = userInfo.NgaySinh?.ToString("dd/MM/yyyy") ?? "";
                    tendangnhap.Text = userInfo.TenDangNhap ?? "";
                    Console.WriteLine("User info loaded successfully");
                }
                else
                {
                    Console.WriteLine("No user info found");
                    MessageBox.Show("Không tìm thấy thông tin người dùng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user info: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải thông tin người dùng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string userId = sessionmanager.curentUser;
                Console.WriteLine($"Saving user info for userId: {userId}");
                if (string.IsNullOrEmpty(userId))
                {
                    MessageBox.Show("Người dùng chưa đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var userInfo = new UserInfo
                {
                    HoTen = textBox1.Text,
                    SoDienThoai = txtSoDienThoai.Text,
                    Email = txtEmail.Text,
                    DiaChi = txtDiaChi.Text,
                    NgaySinh = DateTime.TryParse(txtNgaySinh.Text, out DateTime ngaySinh) ? ngaySinh : (DateTime?)null,
                    TenDangNhap = tendangnhap.Text
                };

                if (_userService.UpdateUserInfo(userId, userInfo))
                {
                    Console.WriteLine("User info updated successfully");
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Console.WriteLine("Failed to update user info");
                    MessageBox.Show("Không tìm thấy người dùng để cập nhật!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user info: {ex.Message}");
                MessageBox.Show($"Lỗi khi lưu thông tin: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}