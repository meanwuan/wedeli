using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeDeLi1.Dbase;

namespace WeDeLi1
{
    public partial class userform : Form
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public userform()
        {
            InitializeComponent();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "WeDeLiApp/1.0 (your-email@domain.com)");
        }

        private async void userform_Load(object sender, EventArgs e)
        {
            await InitializeWebViewAndLoadMap();
        }

        private async Task InitializeWebViewAndLoadMap()
        {
            try
            {
                await webView21.EnsureCoreWebView2Async(null);

                // Lấy danh sách nhà xe gần đó
                string userId = sessionmanager.curentUser;
                if (string.IsNullOrEmpty(userId))
                {
                    MessageBox.Show("Người dùng chưa đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Chuẩn bị JSON body cho yêu cầu
                var requestBody = new
                {
                    UserId = userId,
                    RadiusKm = 10.0
                };
                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                // Gửi yêu cầu POST đến endpoint đúng
                var nearbyStationsResponse = await _httpClient.PostAsync(
                    "http://localhost:8080/api/nearbybusstations",
                    jsonContent);

                if (!nearbyStationsResponse.IsSuccessStatusCode)
                {
                    var errorContent = await nearbyStationsResponse.Content.ReadAsStringAsync();
                    MessageBox.Show($"Không thể lấy danh sách nhà xe gần đó! Status: {nearbyStationsResponse.StatusCode}, Error: {errorContent}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var nearbyStationsJson = await nearbyStationsResponse.Content.ReadAsStringAsync();
                var nearbyStationsData = JObject.Parse(nearbyStationsJson);

                if (!nearbyStationsData["Success"].ToObject<bool>())
                {
                    MessageBox.Show(nearbyStationsData["Message"].ToString(), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tạo HTML bản đồ
                string html = GenerateMapHtml(nearbyStationsData);
                await LoadMapToWebView(html);

                // Hiển thị danh sách nhà xe
                await DisplayBusStations(nearbyStationsData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateMapHtml(JObject data)
        {
            var userAddress = data["Data"]["UserAddress"]?.ToString() ?? "Địa chỉ không xác định";
            var userLat = data["Data"]["UserLatitude"]?.ToObject<double?>();
            var userLng = data["Data"]["UserLongitude"]?.ToObject<double?>();
            var busStations = data["Data"]["NearbyBusStations"]?.ToObject<List<JObject>>() ?? new List<JObject>();

            string markersScript = @"
        // Định nghĩa icon tùy chỉnh cho người dùng (màu xanh dương)
        var userIcon = L.icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-blue.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.3/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });

        // Định nghĩa icon tùy chỉnh cho nhà xe (màu đỏ)
        var busStationIcon = L.icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.3/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });

        // Căn giữa bản đồ dựa trên vị trí người dùng hoặc mặc định
        var centerLat = " + (userLat.HasValue ? userLat.Value.ToString() : "21.0285") + @";
        var centerLng = " + (userLng.HasValue ? userLng.Value.ToString() : "105.8542") + @";
        map.setView([centerLat, centerLng], 13);
    ";

            if (userLat.HasValue && userLng.HasValue)
            {
                markersScript += $@"
        // Marker cho người dùng đang đăng nhập
        L.marker([{userLat}, {userLng}], {{icon: userIcon}}).addTo(map)
            .bindPopup('<b>Vị trí của bạn</b><br>{userAddress}')
            .openPopup();
    ";
            }
            else
            {
                markersScript += @"
        // Thông báo nếu không có tọa độ người dùng
        console.log('Không tìm thấy tọa độ người dùng');
    ";
            }

            if (busStations.Count > 0)
            {
                foreach (var station in busStations)
                {
                    var lat = station["Latitude"]?.ToObject<double?>();
                    var lng = station["Longitude"]?.ToObject<double?>();
                    if (lat.HasValue && lng.HasValue)
                    {
                        var tenChu = station["TenChu"]?.ToString() ?? "Nhà xe không xác định";
                        var diaChi = station["DiaChi"]?.ToString() ?? "Địa chỉ không xác định";
                        var distance = station["Distance"]?.ToString() ?? "N/A";
                        markersScript += $@"
                L.marker([{lat}, {lng}], {{icon: busStationIcon}}).addTo(map)
                    .bindPopup('<b>{tenChu}</b><br>{diaChi}<br>Khoảng cách: {distance}');
            ";
                    }
                }
            }
            else
            {
                markersScript += @"
        // Thông báo nếu không có nhà xe gần đó
        console.log('Không tìm thấy nhà xe gần đó');
    ";
            }

            string mapHtmlPath = @"C:\Users\Admin\source\repos\WeDeLi\map.html";
            if (!File.Exists(mapHtmlPath))
            {
                Console.WriteLine("File map.html không tồn tại tại: " + mapHtmlPath);
                return "";
            }
            string htmlContent = File.ReadAllText(mapHtmlPath);

            int scriptEndIndex = htmlContent.LastIndexOf("</script>");
            if (scriptEndIndex != -1)
            {
                htmlContent = htmlContent.Insert(scriptEndIndex, markersScript + "\n");
            }
            else
            {
                Console.WriteLine("Không tìm thấy thẻ </script> trong map.html");
            }

            return htmlContent;
        }

        private async Task LoadMapToWebView(string html)
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "map_dynamic.html");
                File.WriteAllText(filePath, html);
                Console.WriteLine($"Đã ghi file tại: {filePath}");
                webView21.Source = new Uri($"file:///{filePath.Replace("\\", "/")}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"Lỗi khi load map: {ex.Message}");
            }
        }

        private async Task DisplayBusStations(JObject data)
        {
            ConfigureDataGridView();
            var busStations = data["Data"]["NearbyBusStations"]?.ToObject<List<JObject>>() ?? new List<JObject>();

            foreach (var station in busStations)
            {
                dataGridView1.Rows.Add(
                    station["TenChu"]?.ToString(),
                    "", // SoLuongXe
                    "", // LoaiHangCho
                    "", // KhoBi
                    station["DiaChi"]?.ToString(),
                    station["Distance"]?.ToString()
                );
            }
        }

        private void ConfigureDataGridView()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("NhaXeGanBan", "nhà xe gần bạn");
            dataGridView1.Columns.Add("SoLuongXe", "số lượng xe");
            dataGridView1.Columns.Add("LoaiHangCho", "loại hàng chở");
            dataGridView1.Columns.Add("KhoBi", "kho bì");
            dataGridView1.Columns.Add("DiaChi", "địa chỉ");
            dataGridView1.Columns.Add("TinhTrang", "Tình trạng");
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void webview_Click(object sender, EventArgs e)
        {
            // Phương thức này trống, bạn có thể thêm logic nếu cần
        }

        private void add_goods_Click(object sender, EventArgs e)
        {
            var addProductForm = new addproduct();
            addProductForm.Show();
        }

        private void history_diliver_Click(object sender, EventArgs e)
        {
            var historyForm = new historypro(sessionmanager.curentUser);
            historyForm.Show();
        }

        internal void ShowDiaLog()
        {
            throw new NotImplementedException();
        }

        private void edit_info_Click(object sender, EventArgs e)
        {
            var infoForm = new infor();
            infoForm.Show();
        }
    }
}