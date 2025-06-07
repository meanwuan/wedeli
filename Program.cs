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
            string serverPreface = "http://*:8080/";
            HttpServer server = new HttpServer(serverPreface);
            Task.Run(() => server.StartAsync()); // Chạy server trong background

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);   

            // Sử dụng ApplicationContext để quản lý vòng đời ứng dụng
            // Ứng dụng sẽ chạy cho đến khi Form chính (hoặc tất cả các form khác) bị đóng
            Application.Run(new ApplicationContext(new login())); // Bắt đầu với form login
        }
    }
}