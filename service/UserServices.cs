using System;
using System.Data.Entity;
using System.Linq;
using WeDeLi1.Dbase;

namespace WeDeLi1.service
{
    public class UserService
    {
        public UserInfo GetUserInfo(string userId)
        {
            try
            {
                using (var context = new databases())
                {
                    var user = context.NguoiDungs
                        .Where(u => u.MaNguoiDung == userId)
                        .Select(u => new UserInfo
                        {
                            HoTen = u.HoTen,
                            SoDienThoai = u.SoDienThoai,
                            Email = u.Email,
                            DiaChi = u.DiaChi,
                            NgaySinh = u.NgaySinh,
                            TenDangNhap = u.TenDangNhap
                        })
                        .FirstOrDefault();

                    return user;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin người dùng: {ex.Message}");
            }
        }

        public bool UpdateUserInfo(string userId, UserInfo userInfo)
        {
            try
            {
                using (var context = new databases())
                {
                    var user = context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == userId);
                    if (user == null)
                    {
                        return false;
                    }

                    user.HoTen = userInfo.HoTen;
                    user.SoDienThoai = userInfo.SoDienThoai;
                    user.Email = userInfo.Email;
                    user.DiaChi = userInfo.DiaChi;
                    user.NgaySinh = userInfo.NgaySinh;
                    user.TenDangNhap = userInfo.TenDangNhap;

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật thông tin người dùng: {ex.Message}");
            }
        }
    }

    public class UserInfo
    {
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public DateTimeOffset? NgaySinh { get; set; }
        public string TenDangNhap { get; set; }
    }
}