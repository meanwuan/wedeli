using System;
using System.Collections.Generic;

namespace WeDeLi1.Service
{
    public class UserService
    {
        // Giả lập cơ sở dữ liệu người dùng bằng danh sách in-memory
        private readonly List<User> _users;

        public UserService()
        {
            // Khởi tạo danh sách người dùng giả lập
            _users = new List<User>
            {
                new User
                {
                    Id = "1",
                    HoTen = "Nguyen Van A",
                    TenDangNhap = "user1",
                    MatKhau = "password123",
                    Email = "user1@example.com",
                    SoDienThoai = "0123456789",
                    NgaySinh = new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero),
                    DiaChi = "123 Đường Láng, Hà Nội",
                    VaiTro = "User"
                },
                new User
                {
                    Id = "2",
                    HoTen = "Tran Thi B",
                    TenDangNhap = "user2",
                    MatKhau = "password456",
                    Email = "user2@example.com",
                    SoDienThoai = "0987654321",
                    NgaySinh = new DateTimeOffset(1995, 5, 5, 0, 0, 0, TimeSpan.Zero),
                    DiaChi = "456 Lê Lợi, TP.HCM",
                    VaiTro = "Admin"
                }
            };
        }

        // Lấy chi tiết người dùng theo username hoặc userId
        public User GetUserDetails(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return null;
            }

            // Tìm người dùng theo TenDangNhap hoặc Id
            var user = _users.Find(u => u.TenDangNhap == identifier || u.Id == identifier);
            return user;
        }

        // Cập nhật thông tin người dùng
        public object UpdateUser(
            string hoTen,
            string tenDangNhap,
            string matKhau,
            string email,
            string soDienThoai,
            DateTimeOffset? ngaySinh,
            string diaChi,
            string vaiTro)
        {
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                return new { Success = false, Message = "Username is required" };
            }

            var user = _users.Find(u => u.TenDangNhap == tenDangNhap);
            if (user == null)
            {
                return new { Success = false, Message = "User not found" };
            }

            try
            {
                user.HoTen = hoTen ?? user.HoTen;
                user.MatKhau = matKhau ?? user.MatKhau;
                user.Email = email ?? user.Email;
                user.SoDienThoai = soDienThoai ?? user.SoDienThoai;
                user.NgaySinh = ngaySinh ?? user.NgaySinh;
                user.DiaChi = diaChi ?? user.DiaChi;
                user.VaiTro = vaiTro ?? user.VaiTro;

                return new
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = new
                    {
                        user.Id,
                        user.HoTen,
                        user.TenDangNhap,
                        user.Email,
                        user.SoDienThoai,
                        user.NgaySinh,
                        user.DiaChi,
                        user.VaiTro
                    }
                };
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = $"Error updating user: {ex.Message}" };
            }
        }
    }

    // Lớp User để đại diện cho dữ liệu người dùng
    public class User
    {
        public string Id { get; set; }
        public string HoTen { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public DateTimeOffset? NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string VaiTro { get; set; }
    }
}