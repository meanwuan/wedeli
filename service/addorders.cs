using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeDeLi1.Dbase;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace WeDeLi1.service
{
    public class addorders
    {
        private readonly string logfile  = "don_hang_cho_xacnhan_log.json";
        public string themdonhang(DonHang donhang)
        {
            try
            {
                string prefix = $"{donhang.MaNguoiDung}_{donhang.MaPhuongTien}_{DateTime.Now: ddMMyyyy}";
                List<DonHang> existingorders = Getdonhangformlog();
                int nextNumber = existingorders.Count(o => o.MaDonHang.StartsWith(prefix)) + 1;
                donhang.MaDonHang = $"{prefix}_{nextNumber:D4}"; // Format the order number with leading zeros
                donhang.TrangThai = "Chờ xác nhận"; // Set the initial status of the order
                                                    // thêm vào file log
                existingorders.Add(donhang);
                File.WriteAllText(logfile, JsonConvert.SerializeObject(existingorders, Formatting.Indented));
                return donhang.MaDonHang;
            }
            catch (Exception ex)
            {

                throw new Exception("lỗi khi thêm đơn hàng vào file log " + ex.Message);
            }
        }
        public List<DonHang> Getdonhangformlog() {
            try
            {
                if (File.Exists(logfile))
                { 
                    string json = File.ReadAllText(logfile);
                    return JsonConvert.DeserializeObject<List<DonHang>>(json) ?? new List<DonHang>();
                }
                return new List<DonHang>();
            }
            catch (Exception)
            {

                return new List<DonHang>();
            }
        }
    }
}
