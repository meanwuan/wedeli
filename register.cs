using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeDeLi1.Dbase;
using WeDeLi1.Service;
using WeDeLi1.style;
namespace WeDeLi1
{
    
    public partial class register : Form
    {
        string currentCaptcha;
        private readonly RegisterService registerService;
        public register()
        {
            InitializeComponent();
            borderbutton.bogocbovien(button1,12);
            borderbutton.bogocbovien(button2, 12);
            this.DialogResult = DialogResult.OK;
            registerService = new RegisterService();
            LoadCaptcha();
            
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

        private string GenerateCaptcha(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void LoadCaptcha()
        {
            currentCaptcha = GenerateCaptcha(5);
            pictureBox1.Image = GenerateCaptchaImage(currentCaptcha);
        }
        private Bitmap GenerateCaptchaImage(string captchaText)
        {
            Bitmap bitmap = new Bitmap(200, 50);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                Font font = new Font("Arial", 24, FontStyle.Bold);
                Brush brush = new SolidBrush(Color.Black);
                g.DrawString(captchaText, font, brush, 10, 10);

                // Thêm nhiễu
                Random rand = new Random();
                for (int i = 0; i < 20; i++)
                {
                    int x = rand.Next(bitmap.Width);
                    int y = rand.Next(bitmap.Height);
                    bitmap.SetPixel(x, y, Color.Gray);
                }
            }
            return bitmap;
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
