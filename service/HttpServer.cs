using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeDeLi1.Services;

namespace WeDeLi1.service
{
    public class HttpServer
    {
        private readonly HttpListener listener;
        private readonly LoginService loginService;
        private readonly RegisterService registerService;
        private bool isRunning;

        public HttpServer(string prefix)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(prefix); // Ví dụ: "http://localhost:8080/"
            loginService = new LoginService();
            registerService = new RegisterService();
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
            }
        }

        public void Stop()
        {
            isRunning = false;
            listener.Stop();
            listener.Close();
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                // Đọc body của request
                string requestBody;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    requestBody = reader.ReadToEnd();
                }

                // Xử lý endpoint
                string responseString = "";
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
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    responseString = JsonSerializer.Serialize(new { Message = "Endpoint không tồn tại" });
                }

                // Gửi response
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentType = "application/json";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var responseString = JsonSerializer.Serialize(new { Message = $"Lỗi server: {ex.Message}" });
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentType = "application/json";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
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
    }
}
