using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeDeLi1.Service;

namespace WeDeLi1.service
{
    public class HttpServer
    {
        private readonly HttpListener listener;
        private readonly LoginService loginService;
        private readonly RegisterService registerService;
        private readonly TransportMapService transportMapService;
        private readonly addorders addOrderService;
        private readonly UserService userService;
        private readonly OrderStatusService orderStatusService;
        private bool isRunning;
        private string currentCaptchaText;

        public HttpServer(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentNullException(nameof(prefix), "Prefix cannot be null or empty.");

            listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            loginService = new LoginService();
            registerService = new RegisterService();
            transportMapService = new TransportMapService();
            addOrderService = new addorders();
            userService = new UserService();
            orderStatusService = new OrderStatusService();
        }

        public async Task StartAsync()
        {
            if (isRunning) return;

            try
            {
                listener.Start();
                isRunning = true;
                Console.WriteLine("Server started at " + listener.Prefixes.First());

                while (isRunning)
                {
                    var context = await listener.GetContextAsync();
                    Task.Run(() => ProcessRequest(context));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
                throw;
            }
        }

        public void Stop()
        {
            if (!isRunning) return;

            isRunning = false;
            listener.Stop();
            listener.Close();
            Console.WriteLine("Server stopped.");
        }

        private string GenerateCaptcha(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                response.AddHeader("Access-Control-Allow-Origin", "*");
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();
                    return;
                }

                string requestBody = string.Empty;
                if (request.HasEntityBody)
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        requestBody = reader.ReadToEnd();
                    }
                }

                string responseString;
                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/login")
                {
                    var loginRequest = JsonSerializer.Deserialize<LoginRequest>(requestBody);
                    var result = loginService.Authenticate(
                        loginRequest.Username,
                        loginRequest.Password,
                        loginRequest.CaptchaInput,
                        loginRequest.CurrentCaptcha);
                    responseString = JsonSerializer.Serialize(result);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/register")
                {
                    var registerRequest = JsonSerializer.Deserialize<RegisterRequest>(requestBody);
                    var result = registerService.RegisterUser(
                        registerRequest.HoTen,
                        registerRequest.TenDangNhap,
                        registerRequest.MatKhau,
                        registerRequest.Email,
                        registerRequest.SoDienThoai,
                        registerRequest.NgaySinh,
                        registerRequest.DiaChi,
                        registerRequest.VaiTro);
                    responseString = JsonSerializer.Serialize(result);
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/api/captcha")
                {
                    currentCaptchaText = GenerateCaptcha(6);
                    responseString = JsonSerializer.Serialize(new { CaptchaText = currentCaptchaText });
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/userlocation")
                {
                    var userLocationRequest = JsonSerializer.Deserialize<UserLocationRequest>(requestBody);
                    var result = await transportMapService.GetUserLocation(userLocationRequest.UserId);
                    responseString = JsonSerializer.Serialize(result);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/coordinates")
                {
                    var coordinatesRequest = JsonSerializer.Deserialize<CoordinatesRequest>(requestBody);
                    var result = await transportMapService.GetCoordinatesFromUserAddress(coordinatesRequest.Address);
                    responseString = JsonSerializer.Serialize(result);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/nearbybusstations")
                {
                    var nearbyBusStationsRequest = JsonSerializer.Deserialize<NearbyBusStationsRequest>(requestBody);
                    var result = await transportMapService.GetNearbyBusStations(nearbyBusStationsRequest.UserId, nearbyBusStationsRequest.RadiusKm);
                    responseString = JsonSerializer.Serialize(result);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/confirmorder")
                {
                    var confirmRequest = JsonSerializer.Deserialize<ConfirmOrderRequest>(requestBody);
                    addOrderService.ConfirmOrder(confirmRequest.MaDonHangTam);
                    responseString = JsonSerializer.Serialize(new { Success = true, Message = "Đơn hàng đã được xác nhận" });
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/api/pendingorders")
                {
                    var pendingOrders = addOrderService.GetPendingOrders();
                    responseString = JsonSerializer.Serialize(new { Success = true, Data = pendingOrders });
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/api/orderstatus")
                {
                    var queryParams = request.Url.Query.TrimStart('?').Split('&')
                        .Select(param => param.Split('='))
                        .ToDictionary(param => param[0], param => param[1]);
                    string maDonHang;
                    queryParams.TryGetValue("maDonHang", out maDonHang);

                    if (string.IsNullOrEmpty(maDonHang))
                    {
                        SendErrorResponse(context, HttpStatusCode.BadRequest, "Thiếu MaDonHang");
                        return;
                    }

                    var orderStatus = orderStatusService.GetOrderStatus(maDonHang);
                    responseString = JsonSerializer.Serialize(new
                    {
                        Success = true,
                        Data = orderStatus,
                        Message = "Lấy trạng thái đơn hàng thành công"
                    });
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/cancelorder")
                {
                    var cancelRequest = JsonSerializer.Deserialize<CancelOrderRequest>(requestBody);
                    if (string.IsNullOrEmpty(cancelRequest.MaDonHang))
                    {
                        SendErrorResponse(context, HttpStatusCode.BadRequest, "Thiếu MaDonHang");
                        return;
                    }

                    bool success = orderStatusService.CancelOrder(cancelRequest.MaDonHang);
                    if (success)
                    {
                        responseString = JsonSerializer.Serialize(new { Success = true, Message = "Đơn hàng đã được hủy thành công" });
                    }
                    else
                    {
                        responseString = JsonSerializer.Serialize(new { Success = false, Message = "Không tìm thấy đơn hàng để hủy" });
                    }
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/api/editorder")
                {
                    var queryParams = request.Url.Query.TrimStart('?').Split('&')
                        .Select(param => param.Split('='))
                        .ToDictionary(param => param[0], param => param[1]);
                    string maDonHang;
                    queryParams.TryGetValue("maDonHang", out maDonHang);

                    if (string.IsNullOrEmpty(maDonHang))
                    {
                        SendErrorResponse(context, HttpStatusCode.BadRequest, "Thiếu MaDonHang");
                        return;
                    }

                    var order = orderStatusService.GetOrderForEdit(maDonHang);
                    responseString = JsonSerializer.Serialize(new
                    {
                        Success = true,
                        Data = new
                        {
                            MaDonHang = order.MaDonHang,
                            LoaiDon = order.LoaiDon,
                            KhoiLuong = order.KhoiLuong,
                            tenNguoiNhan = order.tenNguoiNhan,
                            DiaChiLayHang = order.DiaChiLayHang,
                            DiaChiGiaoHang = order.DiaChiGiaoHang,
                            ThoiGianLayHang = order.ThoiGianLayHang,
                            ThoiGianGiaoHang = order.ThoiGianGiaoHang,
                            TongTien = order.TongTien,
                            PhuongThucThanhToan = order.PhuongThucThanhToan
                        },
                        Message = "Lấy thông tin đơn hàng để chỉnh sửa thành công"
                    });
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/api/userinfo")
                {
                    var queryParams = request.Url.Query.TrimStart('?').Split('&')
                        .Select(param => param.Split('='))
                        .ToDictionary(param => param[0], param => param[1]);
                    string userId;
                    queryParams.TryGetValue("userId", out userId);
                    if (string.IsNullOrEmpty(userId))
                    {
                        SendErrorResponse(context, HttpStatusCode.BadRequest, "Thiếu UserId");
                        return;
                    }

                    var userInfo = userService.GetUserInfo(userId);
                    if (userInfo != null)
                    {
                        responseString = JsonSerializer.Serialize(new
                        {
                            Success = true,
                            Data = new
                            {
                                HoTen = userInfo.HoTen,
                                SoDienThoai = userInfo.SoDienThoai,
                                Email = userInfo.Email,
                                DiaChi = userInfo.DiaChi,
                                NgaySinh = userInfo.NgaySinh?.ToString("yyyy-MM-dd"),
                                VaiTro = userInfo.TenDangNhap
                            },
                            Message = "Lấy thông tin người dùng thành công"
                        });
                    }
                    else
                    {
                        responseString = JsonSerializer.Serialize(new { Success = false, Message = "Không tìm thấy người dùng" });
                    }
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/userinfo")
                {
                    var updateRequest = JsonSerializer.Deserialize<UserInfoUpdateRequest>(requestBody);
                    if (string.IsNullOrEmpty(updateRequest.UserId))
                    {
                        SendErrorResponse(context, HttpStatusCode.BadRequest, "Thiếu UserId");
                        return;
                    }

                    var userInfo = new UserInfo
                    {
                        HoTen = updateRequest.HoTen,
                        SoDienThoai = updateRequest.SoDienThoai,
                        Email = updateRequest.Email,
                        DiaChi = updateRequest.DiaChi,
                        NgaySinh = DateTime.TryParse(updateRequest.NgaySinh, out DateTime ngaySinh) ? ngaySinh : (DateTime?)null,
                        TenDangNhap = updateRequest.VaiTro
                    };

                    if (userService.UpdateUserInfo(updateRequest.UserId, userInfo))
                    {
                        responseString = JsonSerializer.Serialize(new { Success = true, Message = "Cập nhật thông tin thành công" });
                    }
                    else
                    {
                        responseString = JsonSerializer.Serialize(new { Success = false, Message = "Không tìm thấy người dùng để cập nhật" });
                    }
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    responseString = JsonSerializer.Serialize(new { Message = "Endpoint không tồn tại" });
                }

                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentType = "application/json; charset=utf-8";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            catch (JsonException ex)
            {
                SendErrorResponse(context, HttpStatusCode.BadRequest, $"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                SendErrorResponse(context, HttpStatusCode.InternalServerError, $"Lỗi server: {ex.Message}");
            }
        }

        private void SendErrorResponse(HttpListenerContext context, HttpStatusCode statusCode, string message)
        {
            var response = context.Response;
            response.StatusCode = (int)statusCode;
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

            var responseString = JsonSerializer.Serialize(new { Message = message });
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentType = "application/json; charset=utf-8";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        private class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string CaptchaInput { get; set; }
            public string CurrentCaptcha { get; set; }
        }

        private class RegisterRequest
        {
            public string HoTen { get; set; }
            public string TenDangNhap { get; set; }
            public string MatKhau { get; set; }
            public string Email { get; set; }
            public string SoDienThoai { get; set; }
            public DateTimeOffset? NgaySinh { get; set; }
            public string DiaChi { get; set; }
            public string VaiTro { get; set; }
        }

        private class UserLocationRequest
        {
            public string UserId { get; set; }
        }

        private class CoordinatesRequest
        {
            public string Address { get; set; }
        }

        private class NearbyBusStationsRequest
        {
            public string UserId { get; set; }
            public double RadiusKm { get; set; }
        }

        private class ConfirmOrderRequest
        {
            public string MaDonHangTam { get; set; }
        }

        private class CancelOrderRequest
        {
            public string MaDonHang { get; set; }
        }

        private class UserInfoUpdateRequest
        {
            public string UserId { get; set; }
            public string HoTen { get; set; }
            public string SoDienThoai { get; set; }
            public string Email { get; set; }
            public string DiaChi { get; set; }
            public string NgaySinh { get; set; }
            public string VaiTro { get; set; }
        }
    }
}