using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeDeLi1.service; // Import namespace chứa HttpServer

namespace WeDeLi1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Khởi tạo và chạy HttpServer trong một Task riêng
            // Đảm bảo địa chỉ IP và cổng trùng khớp với cấu hình của bạn
            string serverPreface = "http://*:8080/"; // Hoặc "http://localhost:8080/"
            HttpServer server = new HttpServer(serverPreface);

            // Chạy server trong một background task
            Task.Run(() => server.StartAsync());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new login());
        }
    }
}