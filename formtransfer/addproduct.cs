using System;
using System.Linq;
using System.Windows.Forms;
using WeDeLi1.Dbase; // Thêm using cho DbContext và các lớp thực thể
using WeDeLi1.Service; // Thêm using cho sessionmanager

namespace WeDeLi1.formtransfer
{
    public partial class addproduct : Form
    {
        public addproduct()
        {
            InitializeComponent();
            // Gán sự kiện cho các nút
            conf.Click += conf_Click;
            turnback.Click += (sender, e) => this.Close();
        }

        private void conf_Click(object sender, EventArgs e)
        {
            // --- 1. Lấy và kiểm tra dữ liệu đầu vào ---
            string loaiDonText = loaidon.Text.Trim(); // Lấy loại hàng của đơn hàng
            string diaChiNhan = diachinhanhang.Text.Trim();
            string diaChiGiao = diachigiaohang.Text.Trim();
            string tenNguoiNhanText = tennguoinhan.Text.Trim();

            if (string.IsNullOrEmpty(loaiDonText) || string.IsNullOrEmpty(diaChiNhan) || string.IsNullOrEmpty(diaChiGiao))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin: Loại hàng, Địa chỉ nhận, và Địa chỉ giao.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(trongtai.Text, out double khoiLuong) || khoiLuong <= 0)
            {
                MessageBox.Show("Trọng tải (khối lượng) phải là một số dương.", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(thanhtien.Text, out double tongTien) || tongTien < 0)
            {
                MessageBox.Show("Thành tiền phải là một số không âm.", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var dbContext = new databases())
                {
                    // --- 2. Tìm một xe đang rảnh VÀ PHÙ HỢP VỚI LOẠI HÀNG ---
                    var currentNhaXeId = sessionmanager.currentTransport;
                    PhuongTien availableVehicle = dbContext.PhuongTiens
                        .FirstOrDefault(p =>
                            p.MaNhaXe == currentNhaXeId &&
                            p.LoaiHang == loaiDonText && // *** ĐIỀU KIỆN MỚI: Xe phải chở đúng loại hàng
                            p.TrangThai != "Ngưng hoạt động" &&
                            !p.DonHangs.Any(d => d.TrangThai == "Đang giao"));

                    if (availableVehicle == null)
                    {
                        MessageBox.Show($"Hiện không có xe nào rảnh để chở '{loaiDonText}'. Vui lòng thử lại sau.", "Hết xe phù hợp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // --- 3. Tạo đơn hàng mới ---
                    var newOrder = new DonHang
                    {
                        // Tạo mã đơn hàng duy nhất dựa trên thời gian
                        MaDonHang = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                        LoaiDon = loaiDonText,
                        DiaChiLayHang = diaChiNhan,
                        DiaChiGiaoHang = diaChiGiao,
                        KhoiLuong = khoiLuong,
                        TongTien = tongTien,
                        tenNguoiNhan = tenNguoiNhanText,

                        // Gán trạng thái và thời gian
                        TrangThai = "Đang giao", // Gán trạng thái đang giao
                        ThoiGianLayHang = DateTime.Now,

                        // Gán mã nhà xe và mã xe đã tìm được
                        MaNhaXe = currentNhaXeId,
                        MaPhuongTien = availableVehicle.MaPhuongTien,

                        // Các trường khác có thể để null hoặc gán giá trị mặc định
                        MaNguoiDung = null, // Giả sử chưa cần gán người dùng
                        PhuongThucThanhToan = "Tiền mặt" // Giá trị mặc định
                    };

                    // --- 4. Lưu vào cơ sở dữ liệu ---
                    dbContext.DonHangs.Add(newOrder);
                    dbContext.SaveChanges();

                    MessageBox.Show($"Tạo đơn hàng thành công! Đơn hàng đã được gán cho xe có biển số: {availableVehicle.BienSo}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tạo đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}