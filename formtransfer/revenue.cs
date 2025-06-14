using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WeDeLi1.Dbase;

namespace WeDeLi1.formtransfer
{
    public partial class revenue : Form
    {
        private readonly databases dbContext = new databases();
        private readonly string currentNhaXeId = sessionmanager.currentTransport;

        public revenue()
        {
            InitializeComponent();
            this.Load += new EventHandler(revenue_Load);
        }

        private void revenue_Load(object sender, EventArgs e)
        {
            // Tải dữ liệu từ CSDL
            var completedOrders = dbContext.DonHangs
                                           .Where(d => d.TrangThai == "Hoàn thành" && d.MaNhaXe == currentNhaXeId)
                                           .Include(d => d.PhuongTien)
                                           .ToList();

            // Cập nhật các hàm vẽ biểu đồ mới
            LoadTotalRevenue(completedOrders);
            LoadMonthlyRevenueChart(completedOrders);
            LoadDailyOrderCountChart(completedOrders);
            LoadGoodsTypeCountChart(completedOrders);
            LoadVehicleTripCountChart(completedOrders);

            LoadVehicleStatus();
        }

        #region Updated Chart Functions

        private void LoadMonthlyRevenueChart(List<DonHang> completedOrders)
        {
            chart1.Series.Clear();
            chart1.Titles.Clear();
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.Title = "Doanh thu (VNĐ)";
            chart1.ChartAreas[0].AxisX.Title = "Tháng";

            var series = new Series("Doanh thu") { ChartType = SeriesChartType.Column };

            var revenueByMonth = completedOrders
                .Where(d => d.ThoiGianGiaoHang.HasValue)
                .GroupBy(d => new { Year = d.ThoiGianGiaoHang.Value.Year, Month = d.ThoiGianGiaoHang.Value.Month })
                .Select(g => new
                {
                    MonthYear = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Total = g.Sum(d => d.TongTien ?? 0)
                })
                .OrderByDescending(x => x.MonthYear)
                .Take(12)
                .OrderBy(x => x.MonthYear)
                .ToList();

            foreach (var monthlyData in revenueByMonth)
            {
                series.Points.AddXY(monthlyData.MonthYear.ToString("MM/yyyy"), monthlyData.Total);
            }

            chart1.Series.Add(series);
            chart1.Titles.Add("Doanh thu theo Tháng");
        }

        private void LoadDailyOrderCountChart(List<DonHang> completedOrders)
        {
            chart2.Series.Clear();
            chart2.Titles.Clear();
            chart2.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart2.ChartAreas[0].AxisY.Title = "Số lượng đơn";
            chart2.ChartAreas[0].AxisX.Title = "Ngày";

            var series = new Series("Số đơn hàng") { ChartType = SeriesChartType.Line, BorderWidth = 3 };

            var ordersByDay = completedOrders
                .Where(d => d.ThoiGianGiaoHang.HasValue)
                // *** SỬA LỖI TẠI ĐÂY ***
                // Thay thế DbFunctions.TruncateTime bằng thuộc tính .Date
                .GroupBy(d => d.ThoiGianGiaoHang.Value.Date)
                .Select(g => new
                {
                    Day = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Day)
                .Take(7)
                .OrderBy(x => x.Day)
                .ToList();

            foreach (var dailyData in ordersByDay)
            {
                series.Points.AddXY(dailyData.Day.ToString("dd/MM"), dailyData.Count);
            }

            chart2.Series.Add(series);
            chart2.Titles.Add("Số đơn hàng hoàn thành (7 ngày qua)");
        }

        private void LoadGoodsTypeCountChart(List<DonHang> completedOrders)
        {
            chart3.Series.Clear();
            chart3.Titles.Clear();

            var series = new Series("Loại hàng")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true
            };

            var countByType = completedOrders
                .GroupBy(d => d.LoaiDon)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var type in countByType)
            {
                DataPoint dataPoint = new DataPoint();
                dataPoint.SetValueXY(type.Key, type.Value);
                dataPoint.Label = $"{type.Key} ({type.Value})";
                series.Points.Add(dataPoint);
            }

            chart3.Series.Add(series);
            chart3.Titles.Add("Thống kê loại hàng vận chuyển");
        }

        private void LoadVehicleTripCountChart(List<DonHang> completedOrders)
        {
            chart4.Series.Clear();
            chart4.Titles.Clear();
            chart4.ChartAreas[0].AxisX.Interval = 1;

            var series = new Series("Số lượt chạy") { ChartType = SeriesChartType.Bar };

            var tripsByVehicle = completedOrders
                .Where(d => d.PhuongTien != null)
                .GroupBy(d => d.PhuongTien.BienSo)
                .Select(g => new
                {
                    BienSo = g.Key,
                    TripCount = g.Count()
                })
                .OrderByDescending(x => x.TripCount)
                .Take(5)
                .ToList();

            foreach (var vehicleData in tripsByVehicle)
            {
                series.Points.AddXY(vehicleData.BienSo, vehicleData.TripCount);
            }

            chart4.Series.Add(series);
            chart4.Titles.Add("Top 5 xe có số lượt chạy nhiều nhất");
        }

        #endregion

        #region Original Functions

        private void LoadTotalRevenue(List<DonHang> completedOrders)
        {
            double? totalRevenue = completedOrders.Sum(d => d.TongTien);
            groupBox3.Controls.Clear();
            Label lblTitle = new Label { Text = "Tổng Doanh Thu", Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold), Location = new Point(10, 20), AutoSize = true };
            Label lblRevenue = new Label { Text = $"{totalRevenue:N0} VNĐ", Font = new Font("Microsoft Sans Serif", 14, FontStyle.Regular), ForeColor = Color.Green, Location = new Point(10, 50), AutoSize = true };
            groupBox3.Controls.Add(lblTitle);
            groupBox3.Controls.Add(lblRevenue);
        }

        private void LoadVehicleStatus()
        {
            var groupBoxes = new List<GroupBox> { groupBox2, groupBox4, groupBox5, groupBox6, groupBox7 };
            var buttons = new List<Button> { button1, button2, button3, button4, button5 };
            var radioButtons = new List<RadioButton> { radioButton1, radioButton2, radioButton3, radioButton4, radioButton5 };

            foreach (var box in groupBoxes)
            {
                box.Visible = false;
                box.Text = "";
            }

            var vehicles = dbContext.PhuongTiens
                .Where(p => p.MaNhaXe == currentNhaXeId)
                .Include(p => p.DonHangs)
                .Take(5)
                .ToList();

            for (int i = 0; i < vehicles.Count; i++)
            {
                var vehicle = vehicles[i];
                buttons[i].Text = !string.IsNullOrEmpty(vehicle.BienSo) ? vehicle.BienSo : vehicle.BienSo;
                radioButtons[i].Enabled = false;
                if (vehicle.TrangThai == "Ngưng hoạt động")
                {
                    radioButtons[i].BackColor = Color.Red;
                }
                else if (vehicle.DonHangs.Any(d => d.TrangThai == "Đang giao"))
                {
                    radioButtons[i].BackColor = Color.Green;
                }
                else
                {
                    radioButtons[i].BackColor = Color.Yellow;
                }
                groupBoxes[i].Visible = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            dbContext?.Dispose();
        }

        private void revenue_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            var maintransfer = new maintransfer();
            maintransfer.Show();
        }

        #endregion
    }
}