using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeDeLi1.Dbase;

namespace WeDeLi1.service
{
    public class LoginService
    {
        public class LoginResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string UserId { get; set; }
            public string UserType { get; set; } // "User" or "Transport"
        }

        public LoginResult Authenticate(string username, string password, string captchaInput, string currentCaptcha)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không được bỏ trống"
                };
            }

            if (captchaInput != currentCaptcha)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "CAPTCHA không chính xác. Vui lòng thử lại."
                };
            }

            try
            {
                using (var db = new databases())
                {
                    // Check NguoiDung (User)
                    var nguoiDung = db.NguoiDungs.FirstOrDefault(x => x.TenDangNhap == username && x.MatKhau == password);
                    if (nguoiDung != null)
                    {
                        sessionmanager.curentUser = nguoiDung.MaNguoiDung;
                        return new LoginResult
                        {
                            Success = true,
                            Message = "Đăng nhập thành công (Người dùng)",
                            UserId = nguoiDung.MaNguoiDung,
                            UserType = "User"
                        };
                    }

                    // Check NhaXe (Transport)
                    var nhaXe = db.NhaXes.FirstOrDefault(x => x.TenDangNhap == username && x.MatKhau == password);
                    if (nhaXe != null)
                    {
                        sessionmanager.currentTransport = nhaXe.MaNhaXe;
                        return new LoginResult
                        {
                            Success = true,
                            Message = "Đăng nhập thành công (Nhà xe)",
                            UserId = nhaXe.MaNhaXe,
                            UserType = "Transport"
                        };
                    }

                    // Authentication failed
                    return new LoginResult
                    {
                        Success = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng"
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }
    }
}