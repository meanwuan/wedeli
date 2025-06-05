using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                string vaiTro)
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không được bỏ trống"
                    };
                }

                // Validate email format
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    if (addr.Address != email)
                    {
                        return new RegisterResult
                        {
                            Success = false,
                            Message = "Email không hợp lệ"
                        };
                    }
                }
                catch
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
                    catch (Exception ex)
                    {
                        return new RegisterResult
                        {
                            Success = false,
                            Message = $"Lỗi khi đăng ký: {ex.Message}"
                        };
                    }
                }
            }
        }
    }
