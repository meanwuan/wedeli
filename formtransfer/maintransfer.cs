using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeDeLi1.Dbase;
using WeDeLi1.service;
using WeDeLi1.Service;

namespace WeDeLi1.formtransfer
{
    public partial class maintransfer : Form
    {
        private databases dbContext = new databases();
        private readonly TransportMapService _transportMapService = new TransportMapService();

        // --- CÁC BIẾN MỚI ĐỂ LẮNG NGHE ĐƠN HÀNG ---
        private System.Windows.Forms.Timer _pendingOrderTimer;
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };
        // Danh sách để theo dõi các đơn hàng đã hiển thị, tránh pop-up lặp lại
        private HashSet<string> _processedOrderIds = new HashSet<string>();
        // -----------------------------------------

        public maintransfer()
        {
            InitializeComponent();
            InitializeMapAsync();
            LoadVehicleData();

            // Khởi tạo cơ chế kiểm tra đơn hàng chờ xác nhận
            InitializePendingOrderChecker();
        }

        #region Order Confirmation Polling (Cơ chế lắng nghe và xác nhận đơn hàng)

        /// <summary>
        /// Khởi tạo và bắt đầu Timer để kiểm tra đơn hàng đang chờ.
        /// </summary>
        private void InitializePendingOrderChecker()
        {
            _pendingOrderTimer = new System.Windows.Forms.Timer();
            _pendingOrderTimer.Interval = 5000; // Cứ 5 giây kiểm tra một lần
            _pendingOrderTimer.Tick += PendingOrderTimer_Tick; // Gán sự kiện sẽ chạy khi Timer tick
            _pendingOrderTimer.Start(); // Bắt đầu Timer

            // Gọi sự kiện ngay lần đầu tiên để không phải chờ
            PendingOrderTimer_Tick(null, EventArgs.Empty);
        }

        /// <summary>
        /// Phương thức này được Timer gọi định kỳ để lấy và hiển thị các đơn hàng chờ.
        /// </summary>
        private async void PendingOrderTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Gửi yêu cầu GET đến endpoint /api/pendingorders trên HttpServer
                HttpResponseMessage response = await _httpClient.GetAsync("/api/pendingorders");
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if ((bool)result.Success)
                    {
                        // Lấy danh sách các đơn hàng chờ từ file log
                        List<PendingOrder> pendingOrders = JsonConvert.DeserializeObject<List<PendingOrder>>(result.Data.ToString());

                        // Lọc ra những đơn hàng MỚI mà chúng ta chưa từng xử lý
                        var newOrders = pendingOrders.Where(p => !_processedOrderIds.Contains(p.MaDonHangTam)).ToList();

                        foreach (var order in newOrders)
                        {
                            // Đánh dấu là đã xử lý để không hiển thị lại
                            _processedOrderIds.Add(order.MaDonHangTam);

                            // Hiển thị form conf_pro dưới dạng MỘT HỘP THOẠI (modal)
                            // .ShowDialog() sẽ khóa tất cả các form khác cho đến khi form này được đóng
                            using (var confirmationForm = new conf_pro(order)) //
                            {
                                confirmationForm.ShowDialog();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi ra console và tiếp tục ở lần kiểm tra sau
                Console.WriteLine("Lỗi khi kiểm tra đơn hàng chờ: " + ex.Message);
            }
        }
        #endregion

        #region Original Main Functions (Các chức năng chính có sẵn)
        /// <summary>
        /// (ĐÃ CẬP NHẬT) Tải và đánh dấu vị trí của CHỈ NHÀ XE HIỆN TẠI lên bản đồ.
        /// </summary>
        private async Task LoadNhaXeMarkers()
        {
            // Lấy mã nhà xe hiện tại từ session
            var currentNhaXeId = sessionmanager.currentTransport;
            if (string.IsNullOrEmpty(currentNhaXeId)) return; // Không làm gì nếu không có mã

            // Tìm nhà xe hiện tại trong CSDL
            var currentNhaXe = dbContext.NhaXes.Find(currentNhaXeId);

            // Chỉ xử lý nếu nhà xe tồn tại và có địa chỉ
            if (currentNhaXe != null && !string.IsNullOrWhiteSpace(currentNhaXe.DiaChi))
            {
                try
                {
                    // Sử dụng service để chuyển đổi địa chỉ thành tọa độ
                    dynamic result = await _transportMapService.GetCoordinatesFromUserAddress(currentNhaXe.DiaChi);

                    // Kiểm tra xem việc chuyển đổi có thành công không
                    if (result.Success)
                    {
                        double lat = result.Data.Latitude;
                        double lng = result.Data.Longitude;

                        // Tạo nội dung popup để hiển thị khi nhấn vào marker
                        string popupText = $"<b>{currentNhaXe.TenChu}</b><br>{currentNhaXe.DiaChi}";

                        // Thêm marker vào bản đồ
                        await AddMarkerToMap(lat, lng, popupText);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not get coordinates for {currentNhaXe.DiaChi}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// (ĐÃ CẬP NHẬT) Tải dữ liệu xe của CHỈ NHÀ XE HIỆN TẠI.
        /// </summary>
        private void LoadVehicleData()
        {
            try
            {
                // Lấy mã nhà xe hiện tại từ session
                var currentNhaXeId = sessionmanager.currentTransport;

                // Nếu không có mã nhà xe, không tải gì cả
                if (string.IsNullOrEmpty(currentNhaXeId))
                {
                    MessageBox.Show("Không thể xác định nhà xe hiện tại.", "Lỗi phiên", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lọc danh sách xe chỉ thuộc nhà xe hiện tại
                var vehicleList = dbContext.PhuongTiens
                                         .Where(p => p.MaNhaXe == currentNhaXeId) // LỌC THEO MÃ NHÀ XE
                                         .Select(p => new
                                         {
                                             MaSoXe = p.MaPhuongTien,
                                             BienSo = p.BienSo,
                                             LoaiHang = p.LoaiHang,
                                             TaiTrong = p.TaiTrong
                                         }).ToList();
                dataGridView1.DataSource = vehicleList;
                CustomizeDataGridViewHeaders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu xe: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomizeDataGridViewHeaders()
        {
            if (dataGridView1.Columns["MaSoXe"] != null)
                dataGridView1.Columns["MaSoXe"].HeaderText = "Mã Số Xe";
            if (dataGridView1.Columns["BienSo"] != null)
                dataGridView1.Columns["BienSo"].HeaderText = "Biển Số";
            if (dataGridView1.Columns["LoaiHang"] != null)
                dataGridView1.Columns["LoaiHang"].HeaderText = "Loại Hàng Chuyên Chở";
            if (dataGridView1.Columns["TaiTrong"] != null)
                dataGridView1.Columns["TaiTrong"].HeaderText = "Tải Trọng (kg)";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _httpClient?.Dispose();
            _pendingOrderTimer?.Stop();
            _pendingOrderTimer?.Dispose();
            dbContext?.Dispose();
        }

        #endregion

        #region Map Functions (Các hàm xử lý bản đồ)

        private async void InitializeMapAsync()
        {
            await webView21.EnsureCoreWebView2Async(null);
            // Lưu ý: Đường dẫn tuyệt đối có thể gây lỗi trên máy khác. Cần được thay thế bằng đường dẫn tương đối.
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string mapPath = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\map.html"));


            if (File.Exists(mapPath))
            {
                webView21.CoreWebView2.Navigate(new Uri(mapPath).AbsoluteUri);
            }
            else
            {
                MessageBox.Show($"Lỗi: Không tìm thấy tệp map.html tại đường dẫn:\n{mapPath}", "Lỗi Tải Bản Đồ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            webView21.NavigationCompleted += WebView21_NavigationCompleted;
        }

        private async void WebView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                await ClearAllMarkers();
                await LoadNhaXeMarkers(); // Gọi hàm đã được cập nhật
            }
        }

        private async Task AddMarkerToMap(double lat, double lng, string popupText)
        {
            if (webView21 != null && webView21.CoreWebView2 != null)
            {
                string sanitizedText = System.Text.Json.JsonSerializer.Serialize(popupText);
                string script = $"addMarker({lat}, {lng}, {sanitizedText});";
                await webView21.CoreWebView2.ExecuteScriptAsync(script);
            }
        }

        private async Task ClearAllMarkers()
        {
            if (webView21 != null && webView21.CoreWebView2 != null)
            {
                await webView21.CoreWebView2.ExecuteScriptAsync("clearMarkers();");
            }
        }

        #endregion

        #region Navigation Buttons (Các nút điều hướng)
        private void changecar_Click(object sender, EventArgs e)
        {
            this.Hide();
            var chngeForm = new changeform();
            chngeForm.ShowDialog();
            this.Show(); // Hiển thị lại form chính sau khi form kia đóng
        }

        private void revenue_Click(object sender, EventArgs e)
        {
            this.Hide();
            var revenueForm = new revenue();
            revenueForm.ShowDialog();
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            var listNhanVienForm = new listnhanvien();
            listNhanVienForm.ShowDialog();
            this.Show();
        }

        private void about_Click(object sender, EventArgs e)
        {
            this.Hide();
            var infor = new infor();
            infor.ShowDialog();
            this.Show();
        }
        #endregion

        private void logout_Click(object sender, EventArgs e)
        {
            // Bước 1: Hỏi xác nhận người dùng
            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất không?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                // Bước 2: Xóa thông tin phiên làm việc. 
                sessionmanager.curentUser = null;
                sessionmanager.currentTransport = null;

                // Bước 3: Khởi động lại ứng dụng để quay về màn hình đăng nhập.
                Application.Restart();
            }
        }
    }
}