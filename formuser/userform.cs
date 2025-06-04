using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using WeDeLi1.Dbase;

namespace WeDeLi1
{
    public partial class userform : Form
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly Dictionary<string, (double? lat, double? lng)> _geocodeCache = new Dictionary<string, (double? lat, double? lng)>();
        private object userAddress;

        public userform()
        {
            InitializeComponent();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "WeDeLiApp/1.0 (your-email@domain.com)"); // Thay bằng email của bạn
        }

        private async void userform_Load(object sender, EventArgs e)
        {
            await InitializeWebViewAndLoadMap();
        }

        private async Task InitializeWebViewAndLoadMap()
        {
            await webView21.EnsureCoreWebView2Async(null);

            using (var context = new databases())
            {
                // Lấy địa chỉ người dùng đã đăng nhập
                string userAddress = await GetUserAddress(context);
                if (string.IsNullOrEmpty(userAddress))
                {
                    MessageBox.Show("Không tìm thấy địa chỉ người dùng hoặc người dùng chưa đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Geocode địa chỉ người dùng
                var userCoords = await GeocodeAddress(userAddress);
                if (!userCoords.lat.HasValue || !userCoords.lng.HasValue)
                {
                    MessageBox.Show($"Không thể xác định tọa độ cho địa chỉ người dùng: {userAddress}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy danh sách nhà xe và lọc theo bán kính 10km
                var busStations = await GetBusStationsWithCoordinates(context);
                var nearbyBusStations = await FilterBusStationsWithinRadius(userCoords.lat.Value, userCoords.lng.Value, busStations, 10);

                // Tạo HTML bản đồ với marker cho người dùng và các nhà xe gần đó
                string html = GenerateMapHtml(userCoords, nearbyBusStations);
                await LoadMapToWebView(html);
                await CalculateAndDisplayDistances(userAddress, nearbyBusStations);
            }
        }
        private async Task<List<(string MaNhaXe, string TenChu, string DiaChi, double? Lat, double? Lng)>> FilterBusStationsWithinRadius(double userLat, double userLng, List<(string MaNhaXe, string TenChu, string DiaChi, double? Lat, double? Lng)> busStations, double radiusKm)
        {
            var nearbyStations = new List<(string MaNhaXe, string TenChu, string DiaChi, double? Lat, double? Lng)>();

            foreach (var station in busStations)
            {
                if (station.Lat.HasValue && station.Lng.HasValue)
                {
                    double distance = CalculateDistance(userLat, userLng, station.Lat.Value, station.Lng.Value);
                    if (distance <= radiusKm)
                    {
                        nearbyStations.Add(station);
                    }
                }
            }

            return nearbyStations;
        }

        // Hàm tính khoảng cách Haversine (khoảng cách địa lý giữa hai tọa độ)
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Bán kính Trái Đất (km)
            var dLat = ToRadian(lat2 - lat1);
            var dLon = ToRadian(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;
            return distance;
        }

        private double ToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }
        private async Task<string> GetUserAddress(databases context)
        {
            // Check if a user is logged in
            if (string.IsNullOrEmpty(sessionmanager.curentUser))
            {
                return null; // Or throw an exception if preferred
            }

            return await context.NguoiDungs
                .Where(u => u.MaNguoiDung == sessionmanager.curentUser)
                .Select(u => u.DiaChi)
                .FirstOrDefaultAsync();
        }

        private async Task<List<(string MaNhaXe, string TenChu, string DiaChi, double? Lat, double? Lng)>> GetBusStationsWithCoordinates(databases context)
        {
            var busStations = await context.NhaXes
                .Select(nx => new { nx.MaNhaXe, nx.TenChu, nx.DiaChi })
                .ToListAsync();

            var busStationsWithCoords = new List<(string MaNhaXe, string TenChu, string DiaChi, double? Lat, double? Lng)>();
            foreach (var station in busStations)
            {
                try
                {
                    var (lat, lng) = await GeocodeAddress(station.DiaChi);
                    busStationsWithCoords.Add((station.MaNhaXe, station.TenChu, station.DiaChi, lat, lng));
                }
                catch (Exception ex)
                {
                    busStationsWithCoords.Add((station.MaNhaXe, station.TenChu, station.DiaChi, null, null));
                    MessageBox.Show($"Không thể lấy tọa độ cho {station.DiaChi}: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                await Task.Delay(1000); // Đợi 1 giây để tránh vượt giới hạn Nominatim
            }
            return busStationsWithCoords;
        }

        private string GenerateMapHtml((double? lat, double? lng) userCoords, List<(string MaNhaXe, string TenChu, string DiaChi, double? Lat, double? Lng)> busStations)
        {
            string markersScript = $@"
        // Marker cho địa chỉ người dùng
        L.marker([{userCoords.lat}, {userCoords.lng}]).addTo(map)
            .bindPopup('<b>Địa chỉ người dùng</b><br>{userAddress}')
            .openPopup();
    ";

            // Thêm marker cho các nhà xe gần đó
            foreach (var station in busStations)
            {
                if (station.Lat.HasValue && station.Lng.HasValue)
                {
                    markersScript += $@"
                L.marker([{station.Lat}, {station.Lng}]).addTo(map)
                    .bindPopup('<b>{station.TenChu}</b><br>{station.DiaChi}')
                    .openPopup();
            ";
                }
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

        private async Task<(double? lat, double? lng)> GeocodeAddress(string address)
        {
            if (_geocodeCache.TryGetValue(address, out var cachedCoords))
            {
                Console.WriteLine($"Sử dụng tọa độ từ cache cho {address}: lat={cachedCoords.lat}, lng={cachedCoords.lng}");
                return cachedCoords;
            }

            int maxRetries = 3;
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    // Thêm "Việt Nam" vào địa chỉ nếu chưa có
                    string fullAddress = string.IsNullOrEmpty(address) ? "Việt Nam" : $"{address.Trim()}, Việt Nam";
                    string url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(fullAddress)}&addressdetails=1&limit=1";
                    var response = await _httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase})");
                    }

                    var json = JArray.Parse(await response.Content.ReadAsStringAsync());
                    if (json.Count > 0)
                    {
                        double lat = json[0]["lat"].ToObject<double>();
                        double lng = json[0]["lon"].ToObject<double>();
                        Console.WriteLine($"Geocoded {fullAddress} to lat: {lat}, lng: {lng}");
                        _geocodeCache[address] = (lat, lng);
                        return (lat, lng);
                    }
                    else
                    {
                        Console.WriteLine($"Không tìm thấy địa chỉ: {fullAddress}");
                        _geocodeCache[address] = (null, null);
                        return (null, null);
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("403"))
                {
                    if (retry == maxRetries - 1)
                    {
                        throw new Exception($"Không thể lấy tọa độ cho {address}: 403 Forbidden sau {maxRetries} lần thử. Kiểm tra User-Agent hoặc giới hạn yêu cầu.");
                    }
                    Console.WriteLine($"Lỗi 403 Forbidden khi geocoding {address}. Thử lại sau 2 giây...");
                    await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi geocoding {address}: {ex.Message}");
                    throw;
                }
            }
            return (null, null);
        }

        private async Task CalculateAndDisplayDistances(string userAddress, List<(string MaNhaXe, string TenChu, string DiaChi, double? Lat, double? Lng)> busStations)
        {
            ConfigureDataGridView();

            foreach (var station in busStations)
            {
                try
                {
                    string distance = await GetDistance(userAddress, station.DiaChi);
                    dataGridView1.Rows.Add(
                        station.TenChu,
                        "",
                        "",
                        "",
                        station.DiaChi,
                        distance
                    );
                }
                catch (Exception ex)
                {
                    dataGridView1.Rows.Add(
                        station.TenChu,
                        "",
                        "",
                        "",
                        station.DiaChi,
                        $"Lỗi: {ex.Message}"
                    );
                }
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

        private async Task<string> GetDistance(string origin, string destination)
        {
            var originCoords = await GeocodeAddress(origin);
            var destCoords = await GeocodeAddress(destination);
            string url = $"http://router.project-osrm.org/route/v1/driving/{originCoords.lng},{originCoords.lat};{destCoords.lng},{destCoords.lat}?overview=false";
            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            if (json["code"].ToString() == "Ok")
            {
                double distanceMeters = json["routes"][0]["distance"].ToObject<double>();
                return $"{distanceMeters / 1000} km";
            }
            else
            {
                throw new Exception("Không tính được khoảng cách.");
            }
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
    }
}