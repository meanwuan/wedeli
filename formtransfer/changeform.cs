using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeDeLi1.Dbase;
using WeDeLi1.style;

namespace WeDeLi1.formtransfer
{
    public partial class changeform : Form
    {
        private readonly databases dbContext = new databases();

        public changeform()
        {
            InitializeComponent();
        }

        private void changeform_Load(object sender, EventArgs e)
        {
            LoadVehicleInfo();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            dbContext?.Dispose();
        }

        private void LoadVehicleInfo()
        {
            borderbutton.bogocbovien(button1, 12);
            borderbutton.bogocbovien(button2, 12);
            try
            {
                flowLayoutPanel1.Controls.Clear();
                var allVehicles = dbContext.PhuongTiens
                                           .Include(p => p.DonHangs)
                                           .ToList();

                if (allVehicles.Any())
                {
                    foreach (var vehicle in allVehicles)
                    {
                        GroupBox vehicleBox = CreateVehicleGroupBox(vehicle);
                        flowLayoutPanel1.Controls.Add(vehicleBox);
                    }
                }
                else
                {
                    var lbl = new Label { Text = "Không có thông tin xe để hiển thị.", AutoSize = true };
                    flowLayoutPanel1.Controls.Add(lbl);
                }

                UpdateStatistics(allVehicles);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin xe: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private GroupBox CreateVehicleGroupBox(PhuongTien vehicle)
        {
            GroupBox groupBox = new GroupBox
            {
                Text = $"Xe: {vehicle.BienSo}", //
                Size = new Size(233, 239),
                BackColor = SystemColors.ControlLightLight
            };

            // Lọc ra các đơn hàng chưa hoàn thành của xe
            var activeOrders = vehicle.DonHangs.Where(d => d.TrangThai != "Hoàn thành").ToList();

            // Giờ xuất phát: Lấy thời gian lấy hàng sớm nhất từ các đơn đang hoạt động
            DateTime? departureTime = activeOrders.Any() ? activeOrders.Min(d => d.ThoiGianLayHang) : (DateTime?)null;

            // Thời gian dự kiến kết thúc: Lấy thời gian giao hàng muộn nhất từ các đơn đang hoạt động
            DateTime? arrivalTime = activeOrders.Any() ? activeOrders.Max(d => d.ThoiGianGiaoHang) : (DateTime?)null;

            // Số đơn hàng còn lại
            int remainingOrders = activeOrders.Count();

            //--- Tạo và thêm các Labels vào GroupBox ---

            groupBox.Controls.Add(new Label { Text = "Giờ xuất phát:", Location = new Point(6, 44), AutoSize = true });
            groupBox.Controls.Add(new Label { Text = departureTime?.ToString("HH:mm dd/MM") ?? "N/A", Location = new Point(163, 44), AutoSize = true });

            groupBox.Controls.Add(new Label { Text = "Dự kiến kết thúc:", Location = new Point(6, 79), AutoSize = true });
            groupBox.Controls.Add(new Label { Text = arrivalTime?.ToString("HH:mm dd/MM") ?? "N/A", Location = new Point(163, 79), AutoSize = true });

            groupBox.Controls.Add(new Label { Text = "Đơn hàng còn lại:", Location = new Point(6, 115), AutoSize = true });
            groupBox.Controls.Add(new Label { Text = remainingOrders.ToString(), Location = new Point(163, 115), AutoSize = true });

            groupBox.Controls.Add(new Label { Text = "Trọng tải tối đa:", Location = new Point(6, 150), AutoSize = true });
            groupBox.Controls.Add(new Label { Text = $"{vehicle.TaiTrong} kg", Location = new Point(163, 150), AutoSize = true }); //

            groupBox.Controls.Add(new Label { Text = "Tài Xế:", Location = new Point(6, 188), AutoSize = true });
            groupBox.Controls.Add(new Label { Text = "Chưa có thông tin", Location = new Point(163, 188), AutoSize = true });

            return groupBox;
        }

        private void UpdateStatistics(List<PhuongTien> vehicles)
        {
            // Giả định trạng thái của đơn hàng: "Đang giao", "Hoàn thành"
            int soXeDangChay = vehicles.Count(v => v.DonHangs.Any(d => d.TrangThai == "Đang giao"));
            int soXeHoanThanh = vehicles.Count(v => v.DonHangs.Any() && v.DonHangs.All(d => d.TrangThai == "Hoàn thành"));

            // Giả định cần có trường TrangThai trên PhuongTien để xác định xe dừng khai thác
            // Ví dụ: public string TrangThai { get; set; } trong class PhuongTien
            int soXeDungKhaiThac = 0; // vehicles.Count(v => v.TrangThai == "Dừng khai thác");
            int soXeHoatDong = vehicles.Count - soXeDungKhaiThac;

            // Cập nhật các labels trên giao diện
            label3.Text = soXeDangChay.ToString();
            label4.Text = soXeHoatDong.ToString();
            label6.Text = soXeHoanThanh.ToString();
            label8.Text = soXeDungKhaiThac.ToString();
        }

        private void changeform_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            var mainTransferForm = new maintransfer();
            mainTransferForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var addxe = new addxe();
            addxe.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var listxe = new listxe();
            listxe.ShowDialog();
        }
    }
}