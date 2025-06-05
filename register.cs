using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeDeLi1.Dbase;
using WeDeLi1.Service;

namespace WeDeLi1
{
    public partial class register : Form
    {
       private readonly RegisterService registerService;
        public register()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.OK;
            registerService = new RegisterService();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            username.Text = "";
            pass.Text = "";
            repass.Text = "";
            Email.Text = "";
            sdt.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            role.Text = "Người đặt Hàng";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string hoTen = name.Text.Trim();
            string tenDangNhap = username.Text.Trim();
            string matKhau = pass.Text.Trim();
            string email = Email.Text.Trim(); // Fixed: Corrected from repass.Text to Email.Text
            string soDienThoai = sdt.Text.Trim();
            DateTimeOffset? ngaySinh = dateTimePicker1.Value;
            string diaChi = address.Text.Trim();
            string vaiTro = role.Text.Trim(); // "Người dùng" hoặc "Nhà xe"

            // Validate password confirmation
            if (matKhau != repass.Text.Trim())
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp");
                return;
            }

            var result = registerService.RegisterUser(hoTen, tenDangNhap, matKhau, email, soDienThoai, ngaySinh, diaChi, vaiTro);

            MessageBox.Show(result.Message);

            if (result.Success)
            {
                this.Hide();
                var loginForm = new login();
                loginForm.Show();
                this.Close();
            }
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            pass.PasswordChar = checkBox1.Checked ? '*' : '\0';
            repass.PasswordChar = checkBox1.Checked ? '*' : '\0';
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            var loginForm = new login();
            loginForm.Show();
            this.Close();
        }

        private void register_Load(object sender, EventArgs e)
        {
            role.Items.Add("Người dùng");
            role.Items.Add("Nhà xe");
            role.SelectedIndex = 0;
        }
    }
}
