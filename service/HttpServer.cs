using System;
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

        private class UserRequest
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
    }
}