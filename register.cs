using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeDeLi1.Dbase;
using WeDeLi1.Service;
using WeDeLi1.style;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WeDeLi1
{
    public partial class register : Form
    {
        string currentCaptcha;
        private readonly RegisterService registerService;

        public register()
        {
            InitializeComponent();
            borderbutton.bogocbovien(button1, 12);
            borderbutton.bogocbovien(button2, 12);
            this.DialogResult = DialogResult.OK;
            registerService = new RegisterService();
            LoadCaptcha();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            name.Text = "";
            username.Text = "";
            pass.Text = "";
            repass.Text = "";
            Email.Text = "";
            sdt.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            address.Text = "";
            role.Text = "Người dùng";
            textBoxSotien.Text = "";
            LoadCaptcha();
        }

        private void LoadCaptcha()
        {
            currentCaptcha = GenerateCaptcha(5);
            pictureBox1.Image = GenerateCaptchaImage(currentCaptcha);
        }

        private string GenerateCaptcha(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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
            string email = Email.Text.Trim();
            string soDienThoai = sdt.Text.Trim();
            DateTimeOffset? ngaySinh = dateTimePicker1.Value;
            string diaChi = address.Text.Trim();
            string vaiTro = role.Text.Trim();
            string captchaInput = textBoxSotien.Text.Trim();

            // Kiểm tra CAPTCHA giống như trong login.cs
            if (string.IsNullOrWhiteSpace(captchaInput) || captchaInput != currentCaptcha)
            {
                MessageBox.Show("CAPTCHA không chính xác hoặc để trống. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadCaptcha();
                return;
            }

            // Kiểm tra mật khẩu xác nhận
            if (matKhau != repass.Text.Trim())
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = registerService.RegisterUser(hoTen, tenDangNhap, matKhau, email, soDienThoai, ngaySinh, diaChi, vaiTro, captchaInput, currentCaptcha);

            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (result.Success)
            {
                this.Hide();
                var loginForm = new login();
                loginForm.Show();
                this.Close();
            }
            else
            {
                LoadCaptcha();
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

        private void reloadCaptcha_Click(object sender, EventArgs e)
        {
            LoadCaptcha();
        }

        private void register_Load(object sender, EventArgs e)
        {
            role.Items.Add("Người dùng");
            role.Items.Add("Nhà xe");
            role.SelectedIndex = 0;
        }
    }
}