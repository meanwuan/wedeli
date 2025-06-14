using System;
using System.Linq;
using System.Net.Mail;
using WeDeLi1.Dbase;

namespace WeDeLi1.Service
{
    public class RegisterService
    {
        public class RegisterResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public RegisterResult RegisterUser(
            string hoTen,
            string tenDangNhap,
            string matKhau,
            string email,
            string soDienThoai,
            DateTimeOffset? ngaySinh,
            string diaChi,
            string vaiTro,
            string captchaInput,
            string currentCaptcha)
        {
            // Kiểm tra CAPTCHA
            if (captchaInput != currentCaptcha)
            {
                return new RegisterResult
                {
                    Success = false,
                    Message = "CAPTCHA không chính xác. Vui lòng thử lại."
                };
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                return new RegisterResult
                {
                    Success = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không được bỏ trống"
                };
            }

            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(soDienThoai))
            {
                return new RegisterResult
                {
                    Success = false,
                    Message = "Họ tên, email hoặc số điện thoại không được bỏ trống"
                };
            }

            // Validate phone number
            if (soDienThoai.Length != 10 || !soDienThoai.All(char.IsDigit))
            {
                return new RegisterResult
                {
                    Success = false,
                    Message = "Số điện thoại phải có đúng 10 chữ số"
                };
            }

            // Validate email format
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return new RegisterResult
                {
                    Success = false,
                    Message = "Email không hợp lệ"
                };
            }

            using (var db = new databases())
            {
                // Check for existing username
                bool tendangnhapTonTai = db.NguoiDungs.Any(x => x.TenDangNhap == tenDangNhap)
                                      || db.NhaXes.Any(x => x.TenDangNhap == tenDangNhap);
                if (tendangnhapTonTai)
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Tên đăng nhập đã tồn tại"
                    };
                }

                // Check for existing email
                bool emailTonTai = db.NguoiDungs.Any(x => x.Email == email)
                                || db.NhaXes.Any(x => x.Email == email);
                if (emailTonTai)
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Email đã tồn tại"
                    };
                }

                // Register based on role
                if (vaiTro == "Người dùng")
                {
                    var nguoidung = new NguoiDung()
                    {
                        MaNguoiDung = Guid.NewGuid().ToString().Substring(0, 20),
                        TenDangNhap = tenDangNhap,
                        MatKhau = matKhau,
                        HoTen = hoTen,
                        NgaySinh = ngaySinh,
                        Email = email,
                        SoDienThoai = soDienThoai,
                        DiaChi = diaChi
                    };
                    db.NguoiDungs.Add(nguoidung);
                }
                else if (vaiTro == "Nhà xe")
                {
                    var nhaxe = new NhaXe()
                    {
                        MaNhaXe = Guid.NewGuid().ToString().Substring(0, 20),
                        TenDangNhap = tenDangNhap,
                        MatKhau = matKhau,
                        TenChu = hoTen,
                        Email = email,
                        SoDienThoai = soDienThoai,
                        DiaChi = diaChi
                    };
                    db.NhaXes.Add(nhaxe);
                }
                else
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Vai trò không hợp lệ"
                    };
                }

                try
                {
                    db.SaveChanges();
                    return new RegisterResult
                    {
                        Success = true,
                        Message = "Đăng ký thành công!"
                    };
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = $"Lỗi cơ sở dữ liệu: {ex.InnerException?.Message ?? ex.Message}"
                    };
                }
                catch (Exception ex)
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = $"Lỗi không xác định: {ex.Message}"
                    };
                }
            }
        }
    }
}