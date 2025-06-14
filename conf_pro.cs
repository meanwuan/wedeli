using System;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using WeDeLi1.service;     // Cần using để truy cập lớp PendingOrder
using Newtonsoft.Json; // Cần using thư viện này
using WeDeLi1.style;

namespace WeDeLi1
{
    public partial class conf_pro : Form
    {
        private readonly PendingOrder _pendingOrder;
        private readonly HttpClient _httpClient;

        // Constructor nhận vào một đối tượng PendingOrder để hiển thị
        public conf_pro(PendingOrder pendingOrder)
        {
            InitializeComponent();
            _pendingOrder = pendingOrder;
            // Địa chỉ của HttpServer của bạn
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };

            this.Load += Conf_pro_Load;
            this.conf.Click += Conf_Click;         // Nút Xác nhận
            this.turnback.Click += Turnback_Click;   // Nút Hủy
        }

        private void Conf_pro_Load(object sender, EventArgs e)
        {
            
            if (_pendingOrder == null)
            {
                MessageBox.Show("Không có thông tin đơn hàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Hiển thị thông tin của đơn hàng lên các textbox
            loaidon.Text = _pendingOrder.LoaiDon;
            trongtai.Text = _pendingOrder.KhoiLuong?.ToString();
            diachinhanhang.Text = _pendingOrder.DiaChiLayHang;
            diachigiaohang.Text = _pendingOrder.DiaChiGiaoHang;
            tennguoinhan.Text = _pendingOrder.tenNguoiNhan;
            thanhtien.Text = _pendingOrder.TongTien?.ToString("N0") + " VND";

            // Chuyển các ô sang chế độ chỉ đọc
            loaidon.ReadOnly = true;
            trongtai.ReadOnly = true;
            diachinhanhang.ReadOnly = true;
            diachigiaohang.ReadOnly = true;
            tennguoinhan.ReadOnly = true;
            thanhtien.ReadOnly = true;
        }

        // Sự kiện khi nhấn nút "Xác Nhận"
        private async void Conf_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo đối tượng yêu cầu với MaDonHangTam
                var confirmRequest = new { MaDonHangTam = _pendingOrder.MaDonHangTam };
                string jsonRequest = JsonConvert.SerializeObject(confirmRequest);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Gửi yêu cầu POST đến endpoint /api/confirmorder
                HttpResponseMessage response = await _httpClient.PostAsync("/api/confirmorder", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xác nhận đơn hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // Đóng form sau khi thành công
                }
                else
                {
                    MessageBox.Show($"Xác nhận thất bại: {await response.Content.ReadAsStringAsync()}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi kết nối đến server: {ex.Message}", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện khi nhấn nút "Hủy"
        private async void Turnback_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn HỦY đơn hàng này không?", "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            try
            {
                var cancelRequest = new { MaDonHang = _pendingOrder.MaDonHangTam };
                string jsonRequest = JsonConvert.SerializeObject(cancelRequest);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Gửi yêu cầu POST đến endpoint /api/cancelorder
                HttpResponseMessage response = await _httpClient.PostAsync("/api/cancelorder", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Hủy đơn hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // Đóng form sau khi thành công
                }
                else
                {
                    MessageBox.Show($"Hủy thất bại: {await response.Content.ReadAsStringAsync()}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi kết nối đến server: {ex.Message}", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}