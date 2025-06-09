using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeDeLi1.Dbase;
using WeDeLi1.service;
using WeDeLi1.Service;
using WeDeLi1.style;

namespace WeDeLi1
{
    public partial class addproduct : Form
    {
        private readonly addorders addOrderService = new addorders();
        private readonly TransportMapService transportMapService = new TransportMapService();
        private readonly string sessionManager = sessionmanager.curentUser;
        private DonHang donHang;
        private const double DEFAULT_PRICE_PER_KG = 10000; // 10,000 VND per kg

        public addproduct(string maNguoiDung = null, DonHang donHang = null)
        {
            InitializeComponent();
            borderbutton.bogocbovien(turnback, 12);
            borderbutton.bogocbovien(conf, 12);
            this.donHang = donHang;
            if (maNguoiDung != null) sessionManager = maNguoiDung;
            InitializePaymentMethods();
            LoadNhaXeComboBox();
            GanThongTinDonHang();
            // Gắn sự kiện TextChanged cho trongtai
            trongtai.TextChanged += TrongTai_TextChanged;
        }

        private void InitializePaymentMethods()
        {
            var paymentMethods = new List<string> { "Tiền mặt", "Chuyển khoản", "Ví điện tử" };
            phuongthucthanhtoan.DataSource = paymentMethods;
            phuongthucthanhtoan.SelectedIndex = 0;
        }

        private async void LoadNhaXeComboBox()
        {
            try
            {
                var result = await transportMapService.GetNearbyBusStations(sessionManager, 10); // 10km radius
                var nearbyStations = ((dynamic)result).Data.NearbyBusStations;
                var nhaXeList = new List<dynamic>();

                foreach (var station in nearbyStations)
                {
                    nhaXeList.Add(new { MaNhaXe = station.MaNhaXe, Display = $"{station.TenChu} ({station.Distance})" });
                }

                nhaxe.DataSource = nhaXeList;
                nhaxe.DisplayMember = "Display";
                nhaxe.ValueMember = "MaNhaXe";

                if (nhaXeList.Any())
                {
                    nhaxe.SelectedIndex = 0; // Select nearest by default
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách nhà xe: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addproduct_Load(object sender, EventArgs e)
        {
        }

        private void TrongTai_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(trongtai.Text, out double weight) && weight > 0)
            {
                double totalCost = CalculateTotalCost(weight);
                thanhtien.Text = totalCost.ToString("N0") + " VND"; // Hiển thị định dạng tiền tệ
            }
            else
            {
                thanhtien.Text = "0 VND"; // Nếu khối lượng không hợp lệ
            }
        }

        private void turnback_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn dừng lại quá trình đặt đơn không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void conf_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                XacNhanDonhang pendingOrder = new XacNhanDonhang
                {
                    LoaiDon = loaidon.Text.Trim(),
                    KhoiLuong = double.Parse(trongtai.Text),
                    tenNguoiNhan = tennguoinhan.Text.Trim(),
                    DiaChiLayHang = diachinhanhang.Text.Trim(),
                    DiaChiGiaoHang = diachigiaohang.Text.Trim(),
                    MaDonHangTam = GenerateTempOrderId()
                };

                double totalCost = CalculateTotalCost(pendingOrder.KhoiLuong.Value);

                // Save to pending orders
                addOrderService.AddPendingOrder(pendingOrder, (string)nhaxe.SelectedValue, totalCost, phuongthucthanhtoan.SelectedItem.ToString());

                MessageBox.Show("Đơn hàng đã được tạo và đang chờ xác nhận từ nhà xe.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open tinhtrangdonhang form with MaDonHangTam
                var tinhtrangForm = new tinhtrangdonhang(pendingOrder.MaDonHangTam);
                tinhtrangForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(loaidon.Text) ||
                string.IsNullOrWhiteSpace(trongtai.Text) ||
                string.IsNullOrWhiteSpace(tennguoinhan.Text) ||
                string.IsNullOrWhiteSpace(diachinhanhang.Text) ||
                string.IsNullOrWhiteSpace(diachigiaohang.Text) ||
                nhaxe.SelectedValue == null ||
                phuongthucthanhtoan.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin đơn hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!double.TryParse(trongtai.Text, out double weight) || weight <= 0)
            {
                MessageBox.Show("Khối lượng phải là số dương hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private double CalculateTotalCost(double weight)
        {
            return weight * DEFAULT_PRICE_PER_KG;
        }

        private string GenerateTempOrderId()
        {
            return $"TEMP_{sessionManager}_{DateTime.Now:yyyyMMddHHmmss}";
        }

        private void GanThongTinDonHang()
        {
            if (donHang != null)
            {
                loaidon.Text = donHang.LoaiDon;
                trongtai.Text = donHang.KhoiLuong?.ToString();
                diachinhanhang.Text = donHang.DiaChiLayHang;
                diachigiaohang.Text = donHang.DiaChiGiaoHang;
                tennguoinhan.Text = donHang.tenNguoiNhan;
                // Cập nhật thành tiền nếu có khối lượng
                if (donHang.KhoiLuong.HasValue)
                {
                    thanhtien.Text = CalculateTotalCost(donHang.KhoiLuong.Value).ToString("N0") + " VND";
                }
            }
        }
    }
}