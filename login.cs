using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeDeLi1.Dbase;
using WeDeLi1.formtransfer;
using WeDeLi1.service;
using WeDeLi1.style;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WeDeLi1
{
    public partial class login : Form
    {
        string currentCaptcha;
        private readonly LoginService loginService;


        public login()
        {
            
            InitializeComponent();
            loginService = new LoginService();
            LoadCaptcha();
            pass.PasswordChar = checkBox1.Checked ? '\0' : '*';
            imageborder.BogocPictureBox(logo, 50);
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

        private void clear_Click(object sender, EventArgs e)
        {
            username.Text = "";
            pass.Text = "";
            capcha.Text = "";
            LoadCaptcha();
        }

        private void conf_Click(object sender, EventArgs e)
        {
            string user = username.Text.Trim();
            string pwd = pass.Text.Trim();
            string captchaInput = capcha.Text.Trim();

            var result = loginService.Authenticate(user, pwd, captchaInput, currentCaptcha);

            MessageBox.Show(result.Message);

            if (result.Success)
            {
                this.Hide();
                if (result.UserType == "User")
                {
                    var userForm = new userform();

                    // *** THÊM DÒNG NÀY VÀO ***
                    // Dòng này đảm bảo khi userForm đóng lại (vì bất cứ lý do gì),
                    // form login sẽ được hiển thị trở lại.
                    userForm.FormClosed += (s, args) => this.Show();

                    userForm.Show();
                }
                else if (result.UserType == "Transport")
                {
                    var nhaXeForm = new maintransfer();

                    // *** BẠN CŨNG NÊN THÊM DÒNG TƯƠNG TỰ CHO FORM NHÀ XE ***
                    nhaXeForm.FormClosed += (s, args) => this.Show();

                    nhaXeForm.Show();
                }
            }
            else
            {
                LoadCaptcha();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            pass.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        private void reloadCaptcha_Click(object sender, EventArgs e)
        {
            LoadCaptcha();
        }

        private void register_Click(object sender, EventArgs e)
        {
            this.Hide(); // chỉ Close là đủ
            var loginForm = new register();
            loginForm.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }

        private void reloandcap_Click(object sender, EventArgs e)
        {
            LoadCaptcha();
        }

    }
}